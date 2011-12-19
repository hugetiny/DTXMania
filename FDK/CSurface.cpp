#include "stdafx.h"
#include "FDKError.h"
#include "CSurface.h"
#include "LoadPNG.h"
#include "LoadJPEG.h"
#include "Debug.h"

namespace FDK {
	namespace Graphics {

CSurface::CSurface()
{
	this->pSurface			= NULL;
	this->nAlphaConst		= 255;
	this->dwWidth			= 0;
	this->dwHeight			= 0;
	this->place				= LOADTO_VIDEO_OR_SYSTEM;
	this->bColorKeyEnable	= false;
	this->dwColorKey		= 0xFF000000;
	this->strSurfaceName.clear();
	this->strFileName.clear();
	this->lpResourceName	= NULL;
	this->lpResourceType	= NULL;
	this->pData				= NULL;
	this->dwDataSize		= 0;

	::ZeroMemory( &this->ddpfReference, sizeof( DDPIXELFORMAT ) );
}

CSurface::~CSurface()
{
	SAFE_RELEASE( this->pSurface );
}

void	CSurface::InitDeviceObjects( LPCTSTR name, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat )
{
	this->InitParameters( name, NULL, NULL, NULL, NULL, 0, width, height, place, pFormat );
}

void	CSurface::InitDeviceObjectsFromFile( LPCTSTR name, LPCTSTR filename, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat )
{
	this->InitParameters( name, filename, NULL, NULL, NULL, 0, width, height, place, pFormat );
}

void	CSurface::InitDeviceObjectsFromMemory( LPCTSTR name, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat )
{
	this->InitParameters( name, NULL, NULL, NULL, pData, dwDataSize, width, height, place, pFormat );
}

void	CSurface::InitDeviceObjectsFromResource( LPCTSTR name, LPCTSTR lpResourceName, LPCTSTR lpResourceType, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat )
{
	this->InitParameters( name, NULL, lpResourceName, lpResourceType, NULL, 0, width, height, place, pFormat );
}

void	CSurface::InitParameters( LPCTSTR name, LPCTSTR fname, LPCTSTR lpResourceName, LPCTSTR lpResourceType, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat )
{
	// 名前
	this->strSurfaceName.clear();
	if( name )
		this->strSurfaceName = name;
	
	this->strFileName.clear();
	if( fname )
		this->strFileName = fname;

	// リソース
	this->lpResourceName	= lpResourceName;
	this->lpResourceType	= lpResourceType;

	// データポインタ、データサイズ
	this->pData				= pData;
	this->dwDataSize		= dwDataSize;

	// サイズ
	this->dwWidth			= width;
	this->dwHeight			= height;

	// 配置場所
	this->place				= place;

	// ピクセルフォーマット
	if( pFormat != NULL )
		this->ddpfReference = *pFormat;
	else
		this->ddpfReference.dwSize = 0;		// 無効のしるし

	// その他（デフォルト値）
	this->nAlphaConst		= 255;
	this->bColorKeyEnable	= false;
	this->dwColorKey		= 0xFF000000;

	SAFE_RELEASE( this->pSurface );
}

HRESULT	CSurface::RestoreDeviceObjects()
{
	HRESULT hr = D3D_OK;

	// (1) 指定ファイルからの生成( BMP, PNG, JPEG )
	if( ! this->strFileName.empty() )
		hr = this->RestoreDeviceObjectsFromFile();

	// (2) メモリ内ファイルからの生成( PNG のみ )
	else if( this->pData != NULL && this->dwDataSize > 0 )
		hr = this->RestoreDeviceObjectsFromFileInMemory();

	// (3) リソースからの生成( PNG のみ )
	else if( this->lpResourceName != NULL && this->lpResourceType != NULL )
		hr = this->RestoreDeviceObjectsFromResource();

	// (4) 空生成
	else if( FAILED( hr = this->Create() ) )
		return FDKERR_サーフェイスの作成に失敗;

	return hr;
}

HRESULT	CSurface::RestoreDeviceObjectsFromResource()
{
	HMODULE hModule = (HMODULE)::GetClassLongPtr( CD3DApplication::hWnd, GCL_HMODULE );

	HRSRC hSrc = ::FindResource( hModule, this->lpResourceName, this->lpResourceType );
	if( ! hSrc )
		return FDKERR_ファイルの読み込み失敗;

	HGLOBAL hPNG = LoadResource( hModule, hSrc );
	if( ! hPNG )
		return FDKERR_ファイルの読み込み失敗;

	this->dwDataSize = SizeofResource( hModule, hSrc );
	if( this->dwDataSize == 0 )
		return FDKERR_ファイルの読み込み失敗;

	this->pData = (BYTE*) LockResource( hPNG );
	if( ! this->pData )
		return FDKERR_ファイルの読み込み失敗;

	return this->RestoreDeviceObjectsFromFileInMemory();
}

HRESULT	CSurface::RestoreDeviceObjectsFromFileInMemory()
{
	HRESULT hr = S_OK;

	SAFE_RELEASE( this->pSurface );

	// 状態チェック
	if( ! CD3DApplication::pDD ) 
		return FDKERR_DirectDrawがNULL;

	// メモリイメージファイルからの生成
	if( this->pData && this->dwDataSize > 0 )
	{
		BITMAPINFO* pbinfo = NULL;
		BYTE*		pbdata = NULL;

		// PNG からの生成
		if( LoadPNGtoDIB( this->pData, this->dwDataSize, &pbinfo, &pbdata ) )
		{
			hr = RestoreFromBitmap( pbinfo, pbdata );
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
		}
		else
			hr = FDKERR_DIBの作成に失敗;
	}
	return hr;
}

HRESULT	CSurface::RestoreDeviceObjectsFromFile()
{
	HRESULT hr;

	SAFE_RELEASE( this->pSurface );

	// 状態チェック
	if( ! CD3DApplication::pDD )
		return FDKERR_DirectDrawがNULL;

	// ファイルからの画像読み込み;
	//	BMP ... LoadImage() でBITMAPハンドルを作成して RestoreFromBitmap()
	//	JPG ... LoadJPEG() で BITMAPINFO を作成して　　　　〃
	//	PNG ... LoadPNG()  で BITMAPINFO を作成して　　　　〃
	if( ! this->strFileName.empty() )
	{
		//-----------------------------
		// (A) ビットマップの場合
		HBITMAP hBitmap;
		if( ( hBitmap = (HBITMAP) ::LoadImage( NULL, this->strFileName.c_str(), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE ) ) != NULL )
		{
			hr = RestoreFromBitmap( hBitmap );
			DeleteObject( hBitmap );
			return hr;
		}

		//-----------------------------
		// (B) その他の場合 → ファイルをオープンして種別チェック
		BITMAPINFO* pbinfo = NULL;
		BYTE*		pbdata = NULL;

		FILE* fp;
		if( _tfopen_s( &fp, this->strFileName.c_str(), _T("rb") ) != 0 )
			return FDKERR_ファイルの読み込み失敗;

		//-----------------------------
		// (B1) JPEG の場合
		int h = fgetc( fp ) * 256 + fgetc( fp );
		fseek( fp, 0, SEEK_SET );
		if( h == 0xFFD8 && LoadJPEG( fp, &pbinfo, &pbdata ) != 0 )
		{
			hr = RestoreFromBitmap( pbinfo, pbdata );
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
			fclose( fp );
			return hr;
		}
		else
		{
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
		}

		//-----------------------------
		// (B2) PNG の場合
		fclose( fp );
		if( LoadPNGtoDIB( this->strFileName.c_str(), &pbinfo, &pbdata ) )
		{
			hr = RestoreFromBitmap( pbinfo, pbdata );
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
			return hr;
		}
		else
		{
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
		}
	}
	else
		return FDKERR_ファイルの読み込み失敗;		// ファイル名の指定がない

	return S_OK;
}

HRESULT CSurface::RestoreFromBitmap( HBITMAP hBitmap )
{
	HRESULT hr;

	// Bitmap 情報の取得 → bm
	BITMAP bm;
	::GetObject( hBitmap, sizeof( BITMAP ), &bm );

	// メモリDC の作成 → hBDC
	HDC hBDC;
	if( ( hBDC = ::CreateCompatibleDC( NULL ) ) == NULL )
		return FDKERR_DCの取得に失敗;

	// サイズ調整； this->dwWidth, this->dwHeight がともに 0 なら画像サイズに等しくする。
	if( this->dwWidth == 0 && this->dwHeight == 0 )
	{
		this->dwWidth = bm.bmWidth;
		this->dwHeight = bm.bmHeight;
	}

	// サイズが（this->dwWidth, this->dwHeight）である空のサーフェイスの作成
	if( FAILED( hr = Create() ) )
	{
		::DeleteDC( hBDC );
		return hr;	// 失敗
	}

	// サーフェイスの DC の取得 → hSDC
	HDC hSDC;
	if( FAILED( hr = this->pSurface->GetDC( &hSDC ) ) )
	{
		::DeleteDC( hBDC );
		return hr;	// 失敗
	}

	// Bitmap DC から サーフェイスDC へ画像を転送；
	// StretchBlt() なので、サイズが異なる場合はサーフェイスのサイズに拡大・縮小される。
	::SelectObject( hBDC, hBitmap );
	if( ::StretchBlt(
			hSDC, 0, 0, this->dwWidth, this->dwHeight,		// 転送先
			hBDC, 0, 0, bm.bmWidth, bm.bmHeight,	// 転送元
			SRCCOPY ) == 0 )
	{
		this->pSurface->ReleaseDC( hSDC );
		::DeleteDC( hBDC );
		return FDKERR_StretchBltに失敗;
	}

	// 成功、帰還
	this->pSurface->ReleaseDC( hSDC );
	::DeleteDC( hBDC );
	return S_OK;
}

HRESULT CSurface::RestoreFromBitmap( BITMAPINFO* pbinfo, BYTE* pbdata )
{
	HRESULT hr;

	// サイズ調整； this->dwWidth, this->dwHeight がともに 0 なら画像サイズに等しくする。
	if( this->dwWidth == 0 && this->dwHeight == 0 )
	{
		this->dwWidth = pbinfo->bmiHeader.biWidth;
		this->dwHeight = pbinfo->bmiHeader.biHeight;
	}

	// サイズが（this->dwWidth, this->dwHeight）である空のサーフェイスの作成
	if( FAILED( hr = Create() ) )
		return hr;	// 失敗

	// サーフェイスの DC の取得 → hSDC
	HDC hSDC;
	if( FAILED( hr = this->pSurface->GetDC( &hSDC ) ) )
		return hr;	// 失敗

	// Jpeg DC から サーフェイスDC へ画像を転送；
	// 　StretchBlt() なので、サイズが異なる場合はサーフェイスのサイズに拡大・縮小される。
	if( StretchDIBits(
			hSDC,															// 転送先 DC
			0, 0, this->dwWidth, this->dwHeight,									// 転送先領域
			0, 0, pbinfo->bmiHeader.biWidth, pbinfo->bmiHeader.biHeight,	// 転送元領域
			(LPVOID) pbdata, pbinfo,										// ビットマップ
			DIB_RGB_COLORS, SRCCOPY	) == GDI_ERROR )
	{
		this->pSurface->ReleaseDC( hSDC );
		return FDKERR_StretchBltに失敗;
	}

	// 成功、帰還
	this->pSurface->ReleaseDC( hSDC );
	return S_OK;
}

HRESULT CSurface::Create()
{
	HRESULT hr;

	for( int i = 0; i < 3; i++ )	// 3通りの生成に挑戦
	{
		DDSURFACEDESC2 ddsd;
		::ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
		ddsd.dwSize	= sizeof( DDSURFACEDESC2 );

		switch( i )
		{
		case 0:		// (1) 指定されたPIXELFRMAT
		{
			if( this->ddpfReference.dwSize == 0 )
				continue;
			ddsd.dwFlags	= DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT;
			ddsd.dwWidth	= this->dwWidth;
			ddsd.dwHeight	= this->dwHeight;
			ddsd.ddpfPixelFormat.dwFlags			= this->ddpfReference.dwFlags;
			ddsd.ddpfPixelFormat.dwFourCC			= this->ddpfReference.dwFourCC;
			ddsd.ddpfPixelFormat.dwRGBBitCount		= this->ddpfReference.dwRGBBitCount;
			ddsd.ddpfPixelFormat.dwRBitMask			= this->ddpfReference.dwRBitMask;
			ddsd.ddpfPixelFormat.dwGBitMask			= this->ddpfReference.dwGBitMask;
			ddsd.ddpfPixelFormat.dwBBitMask			= this->ddpfReference.dwBBitMask;
			ddsd.ddpfPixelFormat.dwRGBAlphaBitMask	= this->ddpfReference.dwRGBAlphaBitMask;
			break;
		}

		case 1: 	// (2) 画面と同じPIXELFORMAT
		{
			DDSURFACEDESC2 ddsdscr;
			ZeroMemory( &ddsdscr, sizeof( DDSURFACEDESC2 ) );
			ddsdscr.dwSize = sizeof( DDSURFACEDESC2 );
			CD3DApplication::pDD->GetDisplayMode( &ddsdscr );
			ddsd.dwFlags	= DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT;
			ddsd.dwWidth	= this->dwWidth;
			ddsd.dwHeight	= this->dwHeight;
			ddsd.ddpfPixelFormat.dwFlags			= ddsdscr.ddpfPixelFormat.dwFlags;
			ddsd.ddpfPixelFormat.dwFourCC			= ddsdscr.ddpfPixelFormat.dwFourCC;
			ddsd.ddpfPixelFormat.dwRGBBitCount		= ddsdscr.ddpfPixelFormat.dwRGBBitCount;
			ddsd.ddpfPixelFormat.dwRBitMask			= ddsdscr.ddpfPixelFormat.dwRBitMask;
			ddsd.ddpfPixelFormat.dwGBitMask			= ddsdscr.ddpfPixelFormat.dwGBitMask;
			ddsd.ddpfPixelFormat.dwBBitMask			= ddsdscr.ddpfPixelFormat.dwBBitMask;
			ddsd.ddpfPixelFormat.dwRGBAlphaBitMask	= ddsdscr.ddpfPixelFormat.dwRGBAlphaBitMask;
			break;
		}
		case 2:		// (3) PIXELFORMAT なし
			ddsd.dwFlags	= DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH;
			ddsd.dwWidth	= this->dwWidth;
			ddsd.dwHeight	= this->dwHeight;
			break;

		}
retry:
		ddsd.ddsCaps.dwCaps = DDSCAPS_OFFSCREENPLAIN | ( (this->place == LOADTO_SYSTEM) ? DDSCAPS_SYSTEMMEMORY : DDSCAPS_VIDEOMEMORY );

		if( FAILED( hr = CD3DApplication::pDD->CreateSurface( &ddsd, &this->pSurface, NULL ) ) )
		{
			if( this->place == LOADTO_VIDEO_OR_SYSTEM )		// LOADTO_VIDEO_OR_SYSTEM が指定されているときは、
			{
				this->place = LOADTO_SYSTEM;				// VIDEO に失敗したら SYSTEM で作り直す。
				goto retry;
			}
		}

		// 作成に成功したら、黒を透過キーに設定して帰還。
		if( SUCCEEDED( hr ) )
		{
			this->SetColorKey( /*( CD3DApplication::nBpp == 16 ) ? 0x0000F000 :*/ 0xFF000000 );
			break;
		}
	}

	return hr;
}

HRESULT	CSurface::InvalidateDeviceObjects()
{
	SAFE_RELEASE( this->pSurface );
	return S_OK;
}

HRESULT	CSurface::DeleteDeviceObjects()
{
	SAFE_RELEASE( this->pSurface );
	this->InitParameters( NULL, NULL, NULL, NULL, NULL, 0, 0, 0, LOADTO_SYSTEM, NULL );
	return S_OK;
}

void	CSurface::SetAlphaConst( int a )
{
	if( a <   0 ) a =   0;
	if( a > 255 ) a = 255;
	this->nAlphaConst = a;
}

HRESULT CSurface::SetColorKey()
{
	HRESULT hr = S_OK;

	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;

	// 左上隅の点の色を取得
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof(ddsd) );
	ddsd.dwSize = sizeof(ddsd);
	if( SUCCEEDED( hr = this->pSurface->Lock( NULL, &ddsd, DDLOCK_WAIT, NULL ) ) )
	{
		DWORD dwBits = ddsd.ddpfPixelFormat.dwRGBBitCount;
		this->dwColorKey = (*(DWORD*)ddsd.lpSurface) & ((dwBits == 16) ? 0x0000FFFF : 0xFFFFFFFF);
		this->pSurface->Unlock( NULL );

        // カラーキーとして設定
		DDCOLORKEY ddck;
		ddck.dwColorSpaceLowValue  = this->dwColorKey;
		ddck.dwColorSpaceHighValue = this->dwColorKey;
		this->pSurface->SetColorKey( DDCKEY_SRCBLT, &ddck );
	}
	else
		return hr;

	return S_OK;
}

HRESULT CSurface::SetColorKey( DWORD dwColor )
{
	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;

	// カラーキーとして設定
	DDCOLORKEY ddck;
	ddck.dwColorSpaceLowValue  = dwColor;
	ddck.dwColorSpaceHighValue = dwColor;
	this->pSurface->SetColorKey( DDCKEY_SRCBLT, &ddck );
	this->dwColorKey = dwColor;

	return S_OK;
}

HRESULT CSurface::GetDC( HDC *phdc )
{
	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;
	if( ! phdc )
		return E_INVALIDARG;

	this->LostCheck();		// ロストしていれば復旧する

	return this->pSurface->GetDC( phdc );
}

HRESULT CSurface::ReleaseDC( HDC &phdc )
{
	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;

	return this->pSurface->ReleaseDC( phdc );
}

HRESULT CSurface::Clear( LPRECT prc )
{
	if( this->pSurface == NULL )
		return FDKERR_サーフェイスがNULL;

	this->LostCheck();		// ロストしていれば復旧する

	DDBLTFX ddbltfx;
	::ZeroMemory( &ddbltfx, sizeof( ddbltfx ) );
	ddbltfx.dwSize = sizeof( ddbltfx );
	ddbltfx.dwFillColor = 0;
	this->pSurface->Blt( prc, NULL, NULL, DDBLT_COLORFILL, &ddbltfx );
	return S_OK;
}

DWORD	CSurface::GetSurfaceSize()
{
	if( ! this->pSurface )
		return 0;

	this->LostCheck();		// ロストしていれば復旧する

	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( ddsd ) );
	ddsd.dwSize	= sizeof( ddsd );
	if( FAILED( this->pSurface->GetSurfaceDesc( &ddsd ) ) )
		return 0;

	DWORD dwSize = 0;
	for( DWORD y = 0; y < ddsd.dwHeight; y++ )
		dwSize += ddsd.lPitch;

	return dwSize;
}
HRESULT CSurface::Draw( LPDIRECTDRAWSURFACE7 pDest, int x, int y, LPCRECT pSrc )
{
	HRESULT hr;

	// αが 255 未満の場合は、ソフトウェアで合成描画する。
	if( this->nAlphaConst != 255 )
		return this->BlendDraw( pDest, x, y, pSrc );

	if( ! this->pSurface || ! pDest )
		return FDKERR_サーフェイスがNULL;

	this->LostCheck();		// ロストしていれば復旧する

	// 転送元領域(rcSrc)の算出
	RECT rcSrc = { 0, 0, this->dwWidth, this->dwHeight };
	if( pSrc )
		rcSrc = *pSrc;

	// 転送先領域(rcDest)の算出
	RECT rcDest;
	DDSURFACEDESC2 ddsd;
	::ZeroMemory( &ddsd, sizeof( ddsd ) );
	ddsd.dwSize = sizeof( ddsd );
	pDest->GetSurfaceDesc( &ddsd );
	::SetRect( &rcDest, 0, 0, ddsd.dwWidth, ddsd.dwHeight );

	// クリッピング
	int nSrcWidth	= rcSrc.right   - rcSrc.left;
	int nSrcHeight	= rcSrc.bottom  - rcSrc.top;
	int nDestWidth	= rcDest.right  - rcDest.left;
	int nDestHeight	= rcDest.bottom - rcDest.top;
	if( ( x + nSrcWidth ) < 0 || ( y + nSrcHeight ) < 0 || x > nDestWidth || y > nDestHeight ) return S_OK;
	if( x < 0 ) { rcSrc.left += (-x); x = 0; }
	if( y < 0 ) { rcSrc.top  += (-y); y = 0; }
	if( x + nSrcWidth  > nDestWidth  ) { rcSrc.right  -= x + nSrcWidth  - nDestWidth;  }
	if( y + nSrcHeight > nDestHeight ) { rcSrc.bottom -= y + nSrcHeight - nDestHeight; }
	if( rcSrc.right - rcSrc.left <= 0 || rcSrc.bottom - rcSrc.top <= 0 ) return S_OK;

	// 描画
	SetRect( &rcDest, x, y, x+rcSrc.right-rcSrc.left, y+rcSrc.bottom-rcSrc.top );
	DWORD dwFlags = DDBLT_WAIT | ( this->bColorKeyEnable ? DDBLT_KEYSRC : 0L );
	hr = pDest->Blt( &rcDest, this->pSurface, &rcSrc, dwFlags, NULL );

/*
	// 転送先がビデオメモリなら bDestOnVideo = true
	bool bDestOnVideo;
	DDSCAPS2 ddsc;
	ZeroMemory( &ddsc, sizeof( DDSCAPS2 ) );
	pDest->GetCaps( &ddsc );
	bDestOnVideo = ( ddsc.dwCaps & DDSCAPS_SYSTEMMEMORY ) ? false : true;

	// 転送（描画）
	HRESULT hr;
	if( bDestOnVideo )
	{
		// 転送先がビデオメモリならBltFast()を使う
		DWORD dwFlags = DDBLTFAST_WAIT | ( this->bColorKeyEnable ? DDBLTFAST_SRCCOLORKEY : DDBLTFAST_NOCOLORKEY );
		if( FAILED( hr = pDest->BltFast( x, y, m_pSurface, &rcSrc, dwFlags ) ) )
		{
			// 失敗したらBlt()を使う
			RECT rcDest = { x, y, x+rcSrc.right-rcSrc.left, y+rcSrc.bottom-rcSrc.top };
			dwFlags = DDBLT_WAIT | ( this->bColorKeyEnable ? DDBLT_KEYSRC : 0L );
			hr = pDest->Blt( &rcDest, m_pSurface, &rcSrc, dwFlags, NULL );
			return hr;
		}
	}
	else
	{
		// 転送先がシステムメモリならBlt()を使う
		RECT rcDest = { x, y, x+rcSrc.right-rcSrc.left, y+rcSrc.bottom-rcSrc.top };
		DWORD dwFlags = DDBLT_WAIT | ( this->bColorKeyEnable ? DDBLT_KEYSRC : 0L );
		hr = pDest->Blt( &rcDest, m_pSurface, &rcSrc, dwFlags, NULL );
		return hr;
	}
*/
	return hr;
}

HRESULT CSurface::BlendDraw( LPDIRECTDRAWSURFACE7 pDest, int x, int y, LPCRECT prcSrc )
{
	if( ! this->pSurface || ! pDest )
		return FDKERR_サーフェイスがNULL;

	RECT rcSrc = { 0, 0, this->dwWidth, this->dwHeight };
	if( prcSrc == NULL ) prcSrc = &rcSrc;

	this->LostCheck();		// ロストしていれば復旧する

	// 転送先サーフェイスのロック
	DDSURFACEDESC2 ddsdDst;
	::ZeroMemory( &ddsdDst, sizeof(ddsdDst) );
	ddsdDst.dwSize = sizeof(ddsdDst);
	if( FAILED( pDest->Lock( NULL, &ddsdDst, DDLOCK_WAIT | DDLOCK_SURFACEMEMORYPTR, NULL ) ) )
		return FDKERR_Lockに失敗;

	// 転送元サーフェイスのロック
	DDSURFACEDESC2 ddsdSrc;
	::ZeroMemory( &ddsdSrc, sizeof(ddsdSrc) );
	ddsdSrc.dwSize = sizeof(ddsdSrc);
	if( FAILED( this->pSurface->Lock( NULL, &ddsdSrc, DDLOCK_WAIT | DDLOCK_SURFACEMEMORYPTR, NULL ) ) )
		return FDKERR_Lockに失敗;

	switch( ddsdDst.ddpfPixelFormat.dwRGBBitCount )
	{
	case 4:	//	16色非対応
	case 8:	//	256色非対応
		break;

	case 16:
		switch( ddsdSrc.ddpfPixelFormat.dwRGBBitCount )
		{
		case 16: BlendDraw16to16( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 24: BlendDraw24to16( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 32: BlendDraw32to16( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		}
		break;

	case 24: 
	{
		switch( ddsdSrc.ddpfPixelFormat.dwRGBBitCount )
		{
		case 16: BlendDraw16to24( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 24: BlendDraw24to24( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 32: BlendDraw32to24( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		}
	} break;

	case 32:
		switch( ddsdSrc.ddpfPixelFormat.dwRGBBitCount )
		{
		case 16: BlendDraw16to32( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 24: BlendDraw24to32( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		case 32: BlendDraw32to32( &ddsdSrc, prcSrc, &ddsdDst, x, y ); break;
		}
		break;
	}

	this->pSurface->Unlock( NULL );
	pDest->Unlock( NULL );
	return S_OK;
}

void	CSurface::BlendDraw32to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	DWORD *ps = (DWORD*)pddsdSrc->lpSurface;
	DWORD *pd = (DWORD*)pddsdDst->lpSurface;

	ps = (DWORD*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = (DWORD*)((BYTE*)pd+(lPitchDst *y));								// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = pixel;
		}
		ps = (DWORD*)((BYTE*)ps+lPitchSrc );
		pd = (DWORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw32to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	DWORD *ps = (DWORD*)pddsdSrc->lpSurface;
	 BYTE *pd = ( BYTE*)pddsdDst->lpSurface;

	ps = (DWORD*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = ( BYTE*)((BYTE*)pd+(lPitchDst *y));								// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = ( pd[x3*3] | (pd[x3*3+1]<<8) | (pd[x3*3+2]<<16) ) & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3*3+0] = (BYTE)((pixel >>  0) & 0xFF);
			pd[x3*3+1] = (BYTE)((pixel >>  8) & 0xFF);
			pd[x3*3+2] = (BYTE)((pixel >> 16) & 0xFF);
		}
		ps = (DWORD*)((BYTE*)ps+lPitchSrc );
		pd = ( BYTE*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw32to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	DWORD *ps = (DWORD*)pddsdSrc->lpSurface;
	 WORD *pd = ( WORD*)pddsdDst->lpSurface;

	ps = (DWORD*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = ( WORD*)((BYTE*)pd+(lPitchDst *y));								// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				pixel |= ( ( (DWORD)srcpx[i]*(a) + (DWORD)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = (WORD) pixel;
		}
		ps = (DWORD*)((BYTE*)ps+lPitchSrc );
		pd = ( WORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}

}

void	CSurface::BlendDraw24to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	 BYTE *ps = ( BYTE*)pddsdSrc->lpSurface;
	DWORD *pd = (DWORD*)pddsdDst->lpSurface;

	ps = ( BYTE*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = (DWORD*)((BYTE*)pd+(lPitchDst *y));								// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ( ps[x3*3] | (ps[x3*3+1]<<8) | (ps[x3*3+2]<<16) ) & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = pixel;
		}
		ps = ( BYTE*)((BYTE*)ps+lPitchSrc );
		pd = (DWORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw24to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	BYTE *ps = (BYTE*)pddsdSrc->lpSurface;
	BYTE *pd = (BYTE*)pddsdDst->lpSurface;

	ps = (BYTE*)ps+(lPitchSrc*prcSrc->top) + (prcSrc->left - x)*3;		// 転送元の開始 (x,y) 座標
	pd = (BYTE*)pd+(lPitchDst*y);										// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x*3; x3 < x2*3 ; x3 += 3 )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( (ps[x3] | (ps[x3+1] << 8) | (ps[x3+2] << 16)) & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。（実際は24bitだからあふれないと思うが一応。）
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3+0] = (BYTE)((pixel >>  0) & 0xFF);
			pd[x3+1] = (BYTE)((pixel >>  8) & 0xFF);
			pd[x3+2] = (BYTE)((pixel >> 16) & 0xFF);
		}
		ps = (BYTE*)(ps+lPitchSrc );
		pd = (BYTE*)(pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw24to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	WORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	WORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = (WORD)pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = (WORD)pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = (WORD)pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = (WORD)pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = (WORD)pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = (WORD)pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = (WORD)pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = (WORD)pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = (WORD)BitCount( MaskSrc[i] );
		MaskSrcShift[i] = (WORD)ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = (WORD)BitCount( MaskDst[i] );
		MaskDstShift[i] = (WORD)ShiftCount( MaskDst[i] );
	}

	BYTE *ps = (BYTE*)pddsdSrc->lpSurface;
	WORD *pd = (WORD*)pddsdDst->lpSurface;

	ps = (BYTE*)((BYTE*)ps+(lPitchSrc*prcSrc->top)) + prcSrc->left - x;		// 転送元の開始 (x,y) 座標
	pd = (WORD*)((BYTE*)pd+(lPitchDst*y));									// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			WORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ( ps[x3*3] | (ps[x3*3+1]<<8) | (ps[x3*3+2]<<16) ) & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に WORD×a するので、桁あふれしないよう DWORD でラッピングする。
				pixel |= ( ( (DWORD)srcpx[i]*(a) + (DWORD)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = pixel;
		}
		ps = (BYTE*)((BYTE*)ps+lPitchSrc );
		pd = (WORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw16to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	 WORD *ps = ( WORD*)pddsdSrc->lpSurface;
	DWORD *pd = (DWORD*)pddsdDst->lpSurface;

	ps = ( WORD*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = (DWORD*)((BYTE*)pd+(lPitchDst *y));								// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel = 0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = pixel;
		}
		ps = ( WORD*)((BYTE*)ps+lPitchSrc );
		pd = (DWORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw16to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	DWORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	DWORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = BitCount( MaskSrc[i] );
		MaskSrcShift[i] = ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = BitCount( MaskDst[i] );
		MaskDstShift[i] = ShiftCount( MaskDst[i] );
	}

	WORD *ps = (WORD*)pddsdSrc->lpSurface;
	BYTE *pd = (BYTE*)pddsdDst->lpSurface;

	ps = (WORD*)((BYTE*)ps+(lPitchSrc *prcSrc->top)) + prcSrc->left - x;	// 転送元の開始 (x,y) 座標
	pd = (BYTE*)((BYTE*)pd+(lPitchDst *y));									// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			DWORD srcpx[3], dstpx[3], pixel=0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx[i] = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx[i] <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx[i] >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx[i] <<= MaskDstShift[i];

				dstpx[i] = ( pd[x3*3] | (pd[x3*3+1]<<8) | (pd[x3*3+2]<<16) ) & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に DWORD×a するので、桁あふれしないよう DWORDLONG でラッピングする。
				pixel |= ( ( (DWORDLONG)srcpx[i]*(a) + (DWORDLONG)dstpx[i]*(256-a) ) >> 8 ) & MaskDst[i];
			}

			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3*3+0] = (BYTE)((pixel >>  0) & 0xFF);
			pd[x3*3+1] = (BYTE)((pixel >>  8) & 0xFF);
			pd[x3*3+2] = (BYTE)((pixel >> 16) & 0xFF);
		}
		ps = (WORD*)((BYTE*)ps+lPitchSrc );
		pd = (BYTE*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

void	CSurface::BlendDraw16to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y )
{
	int x2 = x + prcSrc->right  - prcSrc->left; /* x2 = x + Width  */
	int y2 = y + prcSrc->bottom - prcSrc->top;  /* y2 = y + Height */

	int a = this->nAlphaConst;
	if( a == 255 ) a = 256;

	LONG lPitchDst = pddsdDst->lPitch;
	LONG lPitchSrc = pddsdSrc->lPitch;

	// 以下、色関係の配列変数の添え字は [0]=R, [1]=G, [2]=B, [3]=A とする。
	WORD MaskSrc[4], MaskSrcBit[4], MaskSrcShift[4];
	WORD MaskDst[4], MaskDstBit[4], MaskDstShift[4];

	MaskSrc[0] = (WORD)pddsdSrc->ddpfPixelFormat.dwRBitMask;
	MaskSrc[1] = (WORD)pddsdSrc->ddpfPixelFormat.dwGBitMask;
	MaskSrc[2] = (WORD)pddsdSrc->ddpfPixelFormat.dwBBitMask;
	MaskSrc[3] = (WORD)pddsdSrc->ddpfPixelFormat.dwRGBAlphaBitMask;
	MaskDst[0] = (WORD)pddsdDst->ddpfPixelFormat.dwRBitMask;
	MaskDst[1] = (WORD)pddsdDst->ddpfPixelFormat.dwGBitMask;
	MaskDst[2] = (WORD)pddsdDst->ddpfPixelFormat.dwBBitMask;
	MaskDst[3] = (WORD)pddsdDst->ddpfPixelFormat.dwRGBAlphaBitMask;
	for( int i = 0; i < 4; i++ ) {
        MaskSrcBit[i]   = (WORD)BitCount( MaskSrc[i] );
		MaskSrcShift[i] = (WORD)ShiftCount( MaskSrc[i] );
        MaskDstBit[i]   = (WORD)BitCount( MaskDst[i] );
		MaskDstShift[i] = (WORD)ShiftCount( MaskDst[i] );
	}

	WORD *ps = (WORD*)pddsdSrc->lpSurface;
	WORD *pd = (WORD*)pddsdDst->lpSurface;

	ps = (WORD*)((BYTE*)ps+(lPitchSrc*prcSrc->top)) + prcSrc->left - x;		// 転送元の開始 (x,y) 座標
	pd = (WORD*)((BYTE*)pd+(lPitchDst*y));									// 転送先の開始 (0,y) 座標

	for( int y3 = y; y3 < y2; y3++ )
	{
		for( int x3 = x; x3 < x2; x3++ )
		{
			// srcpx[] → 転送元の色を転送先のRGBAフォーマットに増減色
			// dstpx[] → 転送先の色
			WORD srcpx, dstpx, pixel = 0;
			for( int i = 0; i < 3; i++ )
			{
				srcpx = ( ps[x3] & MaskSrc[i] ) >> MaskSrcShift[i];

				if( MaskSrcBit[i] < MaskDstBit[i] )					// 例: Bit数 4 → 5 なら
					srcpx <<= ( MaskDstBit[i] - MaskSrcBit[i] );	//     1 だけ左にシフトする。
				else if( MaskSrcBit[i] > MaskDstBit[i] )			// 例: BitCount 5 → 4 なら
					srcpx >>= ( MaskSrcBit[i] - MaskDstBit[i] );	//     1 だけ右にシフトする。
				srcpx <<= MaskDstShift[i];

				dstpx = pd[x3] & MaskDst[i];

				// 新転送先 = 転送元×α + 転送先×(1-α);
				//   先に WORD×a するので、桁あふれしないよう DWORD でラッピングする。
				pixel |= ( ( (DWORD)srcpx*(a) + (DWORD)dstpx*(256-a) ) >> 8 ) & MaskDst[i];
			}
			if( MaskDst[3] != 0 ) pixel |= MaskDst[3];		// 転送先にアルファビットがあるなら常にMAXを設定
			pd[x3] = pixel;
		}
		ps = (WORD*)((BYTE*)ps+lPitchSrc );
		pd = (WORD*)((BYTE*)pd+lPitchDst ); // １ラスタ分の増量
		// BYTEにキャストしておかないと計算間違う
	}
}

DWORD	CSurface::BitCount( DWORD dwNum )
{
	DWORD dwCount;
	for( dwCount = 0; dwNum != 0; dwNum >>= 1 )
		dwCount += ( dwNum & 0x1 );
	return dwCount;
}

DWORD	CSurface::ShiftCount( DWORD dwNum )
{
	if( dwNum == 0 ) return 0;		// ゼロのときはゼロを返す。

	DWORD dwCount;
	for( dwCount = 0; (dwNum & 0x1) == 0; dwNum >>= 1 )
		dwCount ++;

	return dwCount;
}

HRESULT	CSurface::Text( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color )
{
	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;
		
	if( !str || *str == _T('\0') )
		return S_OK;

	this->LostCheck();		// ロストしていれば復旧する

	HRESULT hr;

	HDC hDC;
	if( FAILED( hr = this->pSurface->GetDC( &hDC ) ) )
		return FDKERR_DCの取得に失敗;

	HFONT hOldFont = (HFONT) ::SelectObject( hDC, hFont );

	::SetBkMode( hDC, TRANSPARENT );
	::SetTextColor( hDC, color );
	::TextOut( hDC, x, y, str, lstrlen( str ) );

	::SelectObject( hDC, hOldFont );
	this->pSurface->ReleaseDC( hDC );

	return S_OK;
}

HRESULT	CSurface::TextSharp( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color )
{
	if( ! this->pSurface )
		return FDKERR_サーフェイスがNULL;
		
	if( !str || *str == _T('\0') )
		return S_OK;

	this->LostCheck();		// ロストしていれば復旧する

	// 面積４倍の BITMAP を作成
	BITMAPINFOHEADER bmi;
	ZeroMemory( &bmi, sizeof( BITMAPINFOHEADER ) );
	bmi.biSize		= sizeof( BITMAPINFOHEADER );
	bmi.biWidth		= (    (LONG)(this->dwWidth  - x) ) * 2;
	bmi.biHeight	= ( - ((LONG)(this->dwHeight - y)) ) * 2;
	bmi.biPlanes	= 1;
	bmi.biBitCount	= 32;
	HDC hDC = CreateCompatibleDC( 0 );
	HBITMAP hBmp = CreateDIBSection( hDC, (BITMAPINFO *)&bmi, DIB_RGB_COLORS, NULL, NULL, 0 );

	// メモリDC を使って BITMAP へテキスト描画
	HBITMAP hOldBmp = (HBITMAP) SelectObject( hDC, hBmp  );
	HFONT hOldFont  = (HFONT) SelectObject( hDC, hFont );
	SetTextColor( hDC, color );
	SetBkColor( hDC, RGB(0,0,0) );
	RECT rc = { 0, 0, (this->dwWidth-x)*2, (this->dwHeight-y)*2 };
	DrawText( hDC, str, lstrlen(str), &rc, DT_LEFT|DT_NOPREFIX );
	SelectObject( hDC, hOldFont );
	SelectObject( hDC, hOldBmp );
	DeleteDC( hDC );

	// ビットマップからサーフェイスへ転送
	BITMAP bmp;
	GetObject( hBmp, sizeof(BITMAP), &bmp );
	CopyFromBMP32x4( x, y, &bmp, true );
	DeleteObject( hBmp );

	return S_OK;
}
int		CSurface::GetTextWidthDot( HFONT hFont, LPCTSTR str文字列 )
{
	HDC hdc = ::CreateCompatibleDC( 0 );
	HFONT hOldFont = (HFONT) ::SelectObject( hdc, hFont );
	SIZE size;
	::GetTextExtentPoint32( hdc, str文字列, lstrlen(str文字列), &size );
	::SelectObject( hdc, hOldFont );
	::DeleteDC( hdc );
	return (int) size.cx;
}
void CSurface::CopyFromBMP32x4( int x, int y, BITMAP* bmp, bool bSkipBlack )
{
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
	ddsd.dwSize = sizeof( DDSURFACEDESC2 );

	if( SUCCEEDED( this->pSurface->Lock( NULL, &ddsd, DDLOCK_WAIT|DDLOCK_WRITEONLY, NULL ) ) )
	{
		BYTE* src_line = (BYTE*) bmp->bmBits;
		BYTE* dst_line = (BYTE*) ddsd.lpSurface;

		// A. 転送先サーフェイスが 32bpp の場合
		if( ddsd.ddpfPixelFormat.dwRGBBitCount == 32 )
		{
			for( int ly = 0; ly < (LONG)this->dwHeight; ly++ )
			{
				if( ly >= y )
				{
					DWORD* src = (DWORD*) src_line;
					DWORD* dst = (DWORD*) dst_line;
					for( int lx = 0; lx < (LONG)this->dwWidth; lx++ )
					{
						if( lx >= x ) 
						{
							DWORD s, s1[4], s2[4], s3[4];
							s = *src;
							s1[0] = s & 0x00FF0000;
							s2[0] = s & 0x0000FF00;
							s3[0] = s & 0x000000FF;
							s = *(src + 1);
							s1[1] = s & 0x00FF0000;
							s2[1] = s & 0x0000FF00;
							s3[1] = s & 0x000000FF;
							s = *((DWORD*)(((BYTE*)src) + bmp->bmWidthBytes));
							s1[2] = s & 0x00FF0000;
							s2[2] = s & 0x0000FF00;
							s3[2] = s & 0x000000FF;
							s = *((DWORD*)(((BYTE*)src) + bmp->bmWidthBytes) + 1);
							s1[3] = s & 0x00FF0000;
							s2[3] = s & 0x0000FF00;
							s3[3] = s & 0x000000FF;

							DWORD A = 255;	// アルファは常に255

							bool bAllBlack = false;		// bSkipBlack 時にスキップするか否かは、４つの色が「すべて黒」か「ひとつでも黒でない」かだけで決める。
							for( int i = 0; i < 4; i++ ) {
								if( ! ( s1[i] == 0 && s2[i] == 0 && s3[i] == 0 ) ) {
									bAllBlack = true;
									break;
								}
							}
						
							if( ! ( bSkipBlack && bAllBlack == 0 ) )	// ４つの色がすべて黒 かつ bSkipBlack=true なら転送しない。
							{
								DWORD c1 = ((( s1[0] + s1[1] + s1[2] + s1[3] ) >> 2) & ddsd.ddpfPixelFormat.dwRBitMask );
								DWORD c2 = ((( s2[0] + s2[1] + s2[2] + s2[3] ) >> 2) & ddsd.ddpfPixelFormat.dwGBitMask );
								DWORD c3 = ((( s3[0] + s3[1] + s3[2] + s3[3] ) >> 2) & ddsd.ddpfPixelFormat.dwBBitMask );
								DWORD c4 = (A << 24);
								*dst = c1 | c2 | c3 | c4;
							}
							src += 2;
						}
						dst ++;
					}
					src_line += bmp->bmWidthBytes * 2;
				}
				dst_line += ddsd.lPitch;
			}
		}
		// B. 転送先サーフェイスが 16/24bpp の場合
		else if ( ddsd.ddpfPixelFormat.dwRGBBitCount == 16 || ddsd.ddpfPixelFormat.dwRGBBitCount == 24 )
		{
			// 転送先マスクのそれぞれのビット数とシフト数を求める
			int nBit数R = this->BitCount( ddsd.ddpfPixelFormat.dwRBitMask );
			int nBit数G = this->BitCount( ddsd.ddpfPixelFormat.dwGBitMask );
			int nBit数B = this->BitCount( ddsd.ddpfPixelFormat.dwBBitMask );
			int nBit数A = this->BitCount( ddsd.ddpfPixelFormat.dwRGBAlphaBitMask );
			int nShift数R = this->ShiftCount( ddsd.ddpfPixelFormat.dwRBitMask );
			int nShift数G = this->ShiftCount( ddsd.ddpfPixelFormat.dwGBitMask );
			int nShift数B = this->ShiftCount( ddsd.ddpfPixelFormat.dwBBitMask );
			int nShift数A = this->ShiftCount( ddsd.ddpfPixelFormat.dwRGBAlphaBitMask );

			for( int ly = 0; ly < (LONG)this->dwHeight; ly++ )
			{
				if( ly >= y )
				{
					DWORD* src = (DWORD*) src_line;
					WORD*  dst = (WORD*)  dst_line;
					for( int lx = 0; lx < (LONG)this->dwWidth; lx++ )
					{
						if( lx >= x ) 
						{
							DWORD s;
							DWORD s1[4], s2[4], s3[4];
							s = *src;
							s1[0] = (s & 0x00FF0000) >> (16 + (8 - nBit数R));
							s2[0] = (s & 0x0000FF00) >> ( 8 + (8 - nBit数G));
							s3[0] = (s & 0x000000FF) >> ( 0 + (8 - nBit数B));
							s = *(src + 1);
							s1[1] = (s & 0x00FF0000) >> (16 + (8 - nBit数R));
							s2[1] = (s & 0x0000FF00) >> ( 8 + (8 - nBit数G));
							s3[1] = (s & 0x000000FF) >> ( 0 + (8 - nBit数B));
							s = *((DWORD*)(((BYTE*)src) + bmp->bmWidthBytes));
							s1[2] = (s & 0x00FF0000) >> (16 + (8 - nBit数R));
							s2[2] = (s & 0x0000FF00) >> ( 8 + (8 - nBit数G));
							s3[2] = (s & 0x000000FF) >> ( 0 + (8 - nBit数B));
							s = *((DWORD*)(((BYTE*)src) + bmp->bmWidthBytes) + 1);
							s1[3] = (s & 0x00FF0000) >> (16 + (8 - nBit数R));
							s2[3] = (s & 0x0000FF00) >> ( 8 + (8 - nBit数G));
							s3[3] = (s & 0x000000FF) >> ( 0 + (8 - nBit数B));

							WORD A = (WORD) ddsd.ddpfPixelFormat.dwRGBAlphaBitMask;		// アルファは常に最大

							bool bAllBlack = false;		// bSkipBlack 時にスキップするか否かは、４つの色が「すべて黒」か「ひとつでも黒でない」かだけで決める。
							for( int i = 0; i < 4; i++ ) {
								if( ! ( s1[i] == 0 && s2[i] == 0 && s3[i] == 0 ) ) {
									bAllBlack = true;
									break;
								}
							}
						
							if( ! ( bSkipBlack && bAllBlack == 0 ) )	// ４つの色がすべて黒 かつ bSkipBlack=true なら転送しない。
							{
								WORD c1 = (WORD)(((( s1[0] + s1[1] + s1[2] + s1[3] ) >> 2) << nShift数R ) & ddsd.ddpfPixelFormat.dwRBitMask );
								WORD c2 = (WORD)(((( s2[0] + s2[1] + s2[2] + s2[3] ) >> 2) << nShift数G ) & ddsd.ddpfPixelFormat.dwGBitMask );
								WORD c3 = (WORD)(((( s3[0] + s3[1] + s3[2] + s3[3] ) >> 2) << nShift数B ) & ddsd.ddpfPixelFormat.dwBBitMask );
								*dst = c1 | c2 | c3 | A;
							}
							src += 2;
						}
						dst ++;
					}
					src_line += bmp->bmWidthBytes * 2;
				}
				dst_line += ddsd.lPitch;
			}
		}

		this->pSurface->Unlock( NULL );
	}
}
void CSurface::LostCheck()
{
	if( this->pSurface != NULL 
			&& this->pSurface->IsLost() == DDERR_SURFACELOST
			&& ( this->place == LOADTO_VIDEO || this->place == LOADTO_VIDEO_OR_SYSTEM ) )
		this->RestoreDeviceObjects();
}
	}//Graphics
}//FDK