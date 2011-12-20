#include "stdafx.h"
#include "CAvi.h"
#include "FDKError.h"

namespace FDK {
	namespace Graphics {

int CAvi::n参照数 = 0;

CAvi::CAvi()
{
	this->hFile 		= NULL;
	this->hAvi 			= NULL;
	this->hFrame		= NULL;
	this->hDrawDib		= NULL;
}
CAvi::~CAvi()
{
	_解放();
}
void	CAvi::_初期化()
{
	this->hFile 		= NULL;
	this->hAvi 			= NULL;
	this->hFrame		= NULL;
	this->hDrawDib		= NULL;
	this->lFrameWidth	= 0;
	this->lFrameHeight	= 0;

	this->bErrorAtDrawFrame = false;

	// 初めての利用ならAVIライブラリも初期化する。
	if( this->n参照数++ == 0 )
		::AVIFileInit();
}

void	CAvi::_解放()
{
	// ファイルが開かれたままなら閉じる。
	if( this->hFile )
		this->Close();

	// CAvi を利用するインスタンスがなくなったらAVIライブラリも終了する。
	if( --this->n参照数 == 0 )
		::AVIFileExit();
}

HRESULT CAvi::Open( LPCTSTR fname )
{
	HRESULT hr;

	// ファイルが開かれたままなら閉じる。
	if( this->hFile )
		this->Close();

	// AVIファイルを読み取りモードでオープンする。
	if(( hr = ::AVIFileOpen( &this->hFile, fname, OF_READ, NULL ) ) != 0 )			// 成功すると S_OK ではなく 0 が返る。以下同じ。
		return hr;

	// 画像ストリームを取得する。
	if( (hr = ::AVIFileGetStream( this->hFile, &this->hAvi, streamtypeVIDEO, 0 )) != 0 )
	{
		::AVIFileRelease( this->hFile );
		this->hFile = NULL;
		return hr;
	}

	// 動画情報を取得する。
	AVISTREAMINFO Info;
	if( (hr = ::AVIStreamInfo( this->hAvi, &Info, sizeof(Info) )) != 0 )
	{
		::AVIStreamRelease( this->hAvi );
		::AVIFileRelease( this->hFile );
		this->hAvi = NULL;
		this->hFile = NULL;
		return FDKERR_動画情報の取得に失敗;
	}
	this->dwレート			= Info.dwRate;
	this->dwスケール		= Info.dwScale;
	this->dw総フレーム数	= ::AVIStreamLength( this->hAvi );
	this->lFrameWidth		= Info.rcFrame.right  - Info.rcFrame.left;
	this->lFrameHeight		= Info.rcFrame.bottom - Info.rcFrame.top;

	// フレームのオープン
	if( ( this->hFrame = ::AVIStreamGetFrameOpen( this->hAvi, NULL ) ) == NULL )
	//if( ( m_hFrame = ::AVIStreamGetFrameOpen( m_hAvi, (LPBITMAPINFOHEADER) AVIGETFRAMEF_BESTDISPLAYFMT ) ) == NULL )		// こっちだと重くなる…RADEON9800でも…
	{
		::AVIStreamRelease( this->hAvi );
		::AVIFileRelease( this->hFile );
		this->hAvi = NULL;
		this->hFile = NULL;
		return FDKERR_動画フレームのオープンに失敗;
	}

	// DrawDib のオープン
	if( (this->hDrawDib = ::DrawDibOpen()) == NULL )
	{
		::AVIStreamGetFrameClose( this->hFrame );
		::AVIStreamRelease( this->hAvi );
		::AVIFileRelease( this->hFile );
		this->hAvi = NULL;
		this->hFile = NULL;
		return FDKERR_DRAWDIBのオープンに失敗;
	}

	return S_OK;
}

void	CAvi::Close()
{
	if( this->hDrawDib )
	{
		::DrawDibEnd( this->hDrawDib );
		::DrawDibClose( this->hDrawDib );
		this->hDrawDib = NULL;
	}
	
	if( this->hFrame )
	{
		::AVIStreamGetFrameClose( this->hFrame );
		this->hFrame = NULL;
	}

	if( this->hAvi )
	{
		::AVIStreamRelease( this->hAvi ); 
		this->hAvi = NULL;
	}

	if( this->hFile )
	{
		::AVIFileRelease( this->hFile );
		this->hFile = NULL;
	}
}

DWORD	CAvi::_時刻からフレーム番号を返す( double time )
{
	if( !this->hAvi || !this->dw総フレーム数 || !this->dwレート || !this->dwスケール )
		return 0;	// 初期化されてない

	// フレームの計算
	return (DWORD)(time * ((double)(this->dwレート) / (1000.0 * this->dwスケール)));
}

void	CAvi::_フレームサイズを取得する( LONG* plWidth, LONG* plHeight )
{
	if( plWidth  ) *plWidth  = this->lFrameWidth;
	if( plHeight ) *plHeight = this->lFrameHeight;
}

int		CAvi::_指定時刻のフレームをDCへ描画する( HWND hWnd, HDC hDC, double time, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh )
{
	// エラー発生済みなら何もしないで終了
	if( this->bErrorAtDrawFrame )
		return CAVI_再生終了;
	
	// 初期化されてなければエラー終了
	if( !this->hAvi || !this->dw総フレーム数 || !this->dwレート || !this->dwスケール )
	{
		this->bErrorAtDrawFrame = true;
		return CAVI_エラー終了;
	}

	// 指定時刻から、表示すべきフレーム番号を計算
	DWORD FrameNo = _時刻からフレーム番号を返す( time );
	if( this->dw総フレーム数 <= FrameNo )
		return CAVI_再生終了;

	// フレームの取得
	BITMAPINFOHEADER *pBmpInfo;		// DIBヘッダ
	BYTE* pBmpData;					// DIBデータ
	if(( pBmpInfo = (BITMAPINFOHEADER*) ::AVIStreamGetFrame( this->hFrame, FrameNo ) ) == NULL )
	{
		// フレーム画像の取得に失敗
		this->bErrorAtDrawFrame = true;
		return CAVI_エラー終了;
	}

	// AVIStreamGetFrame で返される BITMAPINFOHEADER には biCompression==BI_BITFIELDS 時の biColors[] が入っていないため、
	// 内容をコピーして自分で biColors[] を追加する必要がある。
	struct PACKDIB {
		BITMAPINFOHEADER	header;
		DWORD				r, g, b;
	} static pd;
	pBmpData = (LPBYTE)pBmpInfo + (WORD)(pBmpInfo->biSize);
	::CopyMemory( &(pd.header), pBmpInfo, sizeof(BITMAPINFOHEADER) );
	if( pd.header.biCompression == BI_BITFIELDS )
	{
		if( pd.header.biBitCount == 16 )
		{
			pd.r = 0xf800;	// 5
			pd.g = 0x07e0;	// 6
			pd.b = 0x001f;	// 5
		}
		else
		{
			pd.r = 0xff0000;	// 8
			pd.g = 0x00ff00;	// 8
			pd.b = 0x0000ff;	// 8
		}
	}
	pBmpInfo = &(pd.header);

	// 圧縮されているなら展開する
	if( pBmpInfo->biCompression != BI_RGB && pBmpInfo->biCompression != BI_BITFIELDS )
	{
		this->hDrawDib = DrawDibOpen();
		HDC hdc = ::GetDC( hWnd );
		if( ! ::DrawDibDraw( this->hDrawDib, hdc, dx, dy, dw, dh, pBmpInfo, pBmpData, sx, sy, sw, sh, DDF_DONTDRAW ) )	// 展開のみ・描画なし
		{
			// 圧縮DIBの展開に失敗
			this->bErrorAtDrawFrame = true;
			return CAVI_エラー終了;
		}
		pBmpData = (LPBYTE) ::DrawDibGetBuffer( this->hDrawDib, pBmpInfo, sizeof(BITMAPINFOHEADER), 0 );
		::ReleaseDC( hWnd, hdc );
	}

	// 描画
	::DrawDibDraw( this->hDrawDib, hDC, dx, dy, dw, dh, pBmpInfo, pBmpData, sx, sy, sw, sh, 0 );

/*
	if( StretchDIBits( hDC, dx, dy, dw, dh, sx, sy, sw, sh, pBmpData, (BITMAPINFO*)(pBmpInfo), DIB_RGB_COLORS, SRCCOPY ) == GDI_ERROR )
	{
		m_bErrorAtDrawFrame = true;
		if( m_hDrawDib ) {
			DrawDibEnd( m_hDrawDib );
			DrawDibClose( m_hDrawDib );
		}
		return false;
	}
*/
	return 0;
}

	}//Graphics
}//FDK
