#include "stdafx.h"
#include "CD3DApplication.h"
#include "CTexture.h"
#include "FDKError.h"
#include "LoadPNG.h"
#include "LoadJPEG.h"

namespace FDK {
	namespace Graphics {

// static なやつら
DDPIXELFORMAT CTexture::ddpfARGB8888;
DDPIXELFORMAT CTexture::ddpfARGB4444;

// 局所変数
static bool s_bEnumDone = false;		// 列挙済みなら true
static bool s_bBeginScene = false;		// BeginScene() 済みなら true

CTexture::CTexture()
{
	this->pTexture			= NULL;

	this->nAlphaConst		= 255;
	this->bUseAddBlend		= false;
	this->bColorKeyEnable	= false;
	this->dwColorKey		= 0;
	this->dwWidth			= 0;
	this->dwHeight			= 0;
	this->dwImageHeight		= 0;
	this->dwImageWidth		= 0;
	this->fScaleW			= 1.0f;
	this->fScaleH			= 1.0f;
	this->strTextureName.clear();
	this->strFileName.clear();
	this->bSkipBlack		= true;
	this->lpResourceName	= NULL;
	this->lpResourceType	= NULL;
	this->pData				= NULL;
	this->dwDataSize		= 0;
}

CTexture::~CTexture()
{
	SAFE_RELEASE( this->pTexture );
}

HRESULT CTexture::InitDeviceObjects( TCHAR* name, DWORD width, DWORD height, bool bSkipBlack )
{
	InitParameters( name, NULL, NULL, NULL, NULL, 0, width, height, bSkipBlack );
	return S_OK;
}

HRESULT CTexture::InitDeviceObjectsFromFile( LPCTSTR name, LPCTSTR filename, DWORD width, DWORD height, bool bSkipBlack )
{
	InitParameters( name, filename, NULL, NULL, NULL, 0, width, height, bSkipBlack );
	return S_OK;
}

HRESULT CTexture::InitDeviceObjectsFromFileInMemory( TCHAR* name, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, bool bSkipBlack )
{
	InitParameters( name, NULL, NULL, NULL, pData, dwDataSize, width, height, bSkipBlack );
	return S_OK;
}

HRESULT	CTexture::InitDeviceObjectsFromResource( TCHAR* name, LPCTSTR lpName, LPCTSTR lpType, DWORD width, DWORD height, bool bSkipBlack )
{
	InitParameters( name, NULL, lpName, lpType, NULL, 0, width, height, bSkipBlack );
	return S_OK;
}

void	CTexture::InitParameters( LPCTSTR name, LPCTSTR fname, LPCTSTR lpResourceName, LPCTSTR lpResourceType, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, bool bSkipBlack )
{
	// 名前
	this->strTextureName.clear();
	if( name )
		this->strTextureName = name;

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
	this->dwImageWidth		= width;
	this->dwImageHeight		= height;

	// 黒のスキップ
	this->bSkipBlack		= bSkipBlack;

	// その他（デフォルト値）
	this->nAlphaConst		= 255;
	this->bUseAddBlend		= false;
	this->bColorKeyEnable	= false;
	this->dwColorKey		= 0xFF000000;
	this->fScaleW			= 1.0f;
	this->fScaleH			= 1.0f;

	SAFE_RELEASE( this->pTexture );
}

HRESULT	CTexture::RestoreDeviceObjects()
{
	HRESULT hr = D3D_OK;

	// (1) 指定ファイルからの生成( BMP, PNG, JPEG )
	if( ! this->strFileName.empty() )
		hr = RestoreDeviceObjectsFromFile();

	// (2) メモリ内ファイルからの生成( PNG のみ )
	else if( this->pData != NULL && this->dwDataSize > 0 )
		hr = RestoreDeviceObjectsFromFileInMemory();

	// (3) リソースからの生成( PNG のみ )
	else if( this->lpResourceName != NULL && this->lpResourceType != NULL )
		hr = RestoreDeviceObjectsFromResource();

	// (4) 空生成
	else if( FAILED( hr = Create() ) )
		return FDKERR_テクスチャの作成に失敗;

	return hr;
}

HRESULT	CTexture::RestoreDeviceObjectsFromResource()
{
	HMODULE hModule = (HMODULE)GetClassLongPtr( CD3DApplication::hWnd, GCL_HMODULE );

	HRSRC hSrc = FindResource( hModule, this->lpResourceName, this->lpResourceType );
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

	return RestoreDeviceObjectsFromFileInMemory();
}

HRESULT	CTexture::RestoreDeviceObjectsFromFileInMemory()
{
	HRESULT hr = S_OK;

	SAFE_RELEASE( this->pTexture );

	// テクスチャフォーマットの列挙がまだならやる。
	if( ! s_bEnumDone )
		EnumTextureFormats();

	// メモリイメージファイルからの生成
	// ※テクスチャは PNG のみサポート
	if( this->pData  && this->dwDataSize > 0 )
	{
		DDSURFACEDESC2 ddsd;
		BYTE**	ppimage = NULL;

		// PNG からの生成
		if( LoadPNGtoDDS( this->pData, this->dwDataSize, &ddsd, &ppimage ) )
		{
			hr = RestoreFromDDS( &ddsd, ppimage );

			for( DWORD i = 0; i < ddsd.dwHeight; i++ )
				SAFE_FREE( ppimage[i] );
			SAFE_FREE( ppimage );
		}
		else
			hr = FDKERR_DDSの作成に失敗;
	}
	return hr;
}

HRESULT	CTexture::RestoreDeviceObjectsFromFile()
{
	HRESULT hr;

	SAFE_RELEASE( this->pTexture );

	// テクスチャフォーマットの列挙がまだならやる。
	if( ! s_bEnumDone )
		EnumTextureFormats();

	// ファイルからの画像読み込み
	// ※テクスチャは PNG のみサポート
	if( ! this->strFileName.empty() )
	{
		//-----------------------------
		// (A) BMP の場合
		HBITMAP hSrcBmp;
		if( ( hSrcBmp = (HBITMAP) LoadImage( NULL, this->strFileName.c_str(), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE ) ) != NULL )
		{
			// ビットマップ情報の取得
			BITMAP bmSrc;
			GetObject( hSrcBmp, sizeof( BITMAP ), &bmSrc );

			// this->dwWidth, this->dwHeight がともに 0 なら画像サイズに等しくする
			if( this->dwWidth == 0 && this->dwHeight == 0 )
			{
				this->dwWidth  = bmSrc.bmWidth;
				this->dwHeight = bmSrc.bmHeight;
				this->dwImageWidth  = bmSrc.bmWidth;
				this->dwImageHeight = bmSrc.bmHeight;
			}
			// this->dwWidth, this->dwHeight が画像より小さいなら画像のサイズに合わせる
			if( this->dwWidth < (DWORD) bmSrc.bmWidth ) this->dwWidth = bmSrc.bmWidth;
			if( this->dwHeight < (DWORD) bmSrc.bmHeight ) this->dwHeight = bmSrc.bmHeight;

			// 空のテクスチャを作成
			if( FAILED( hr = Create() ) )
			{
				DeleteObject( hSrcBmp );
				return hr;	// 失敗
			}

			// メモリ内に RGB-32bit な BITMAP を作る
			BITMAPINFOHEADER bmi;
			ZeroMemory( &bmi, sizeof( BITMAPINFOHEADER ) );
			bmi.biSize		= sizeof( BITMAPINFOHEADER );
			bmi.biWidth		= this->dwWidth;
			bmi.biHeight	= - ((int)this->dwHeight);		// トップダウンBitmap
			bmi.biPlanes	= 1;
			bmi.biBitCount	= 32;
			HDC hDstDC = CreateCompatibleDC( 0 );
			HBITMAP hDstBmp = CreateDIBSection( hDstDC, (BITMAPINFO *)&bmi, DIB_RGB_COLORS, NULL, NULL, 0 );

			// BITMAP から RGB-32bit BITMAP へ転送する（拡大縮小なし）
			HDC hSrcDC = CreateCompatibleDC( 0 );
			HBITMAP hDstOldBmp = (HBITMAP) SelectObject( hDstDC, hDstBmp );
			HBITMAP hSrcOldBmp = (HBITMAP) SelectObject( hSrcDC, hSrcBmp );
			if( StretchBlt(
				hDstDC,									// 転送先 DC
				0, 0, this->dwWidth, this->dwHeight,	// 転送先領域
				hSrcDC,									// 転送元 DC
				0, 0, this->dwWidth, this->dwHeight,	// 転送元領域
				SRCCOPY ) == 0 )
			{
				SelectObject( hDstDC, hDstOldBmp );
				SelectObject( hSrcDC, hSrcOldBmp );
				DeleteObject( hDstDC );
				DeleteObject( hSrcDC );
				DeleteObject( hDstBmp );
				DeleteObject( hSrcBmp );
				return FDKERR_StretchBltに失敗;
			}
			SelectObject( hDstDC, hDstOldBmp );
			SelectObject( hSrcDC, hSrcOldBmp );
			DeleteObject( hSrcDC );
			DeleteObject( hSrcBmp );
			DeleteObject( hDstDC );

			// RGB-32bit BITMAP からテクスチャへ転送
			BITMAP bmDst;
			GetObject( hDstBmp, sizeof(BITMAP), &bmDst );
			CopyFromBMP32( 0, 0, &bmDst, this->bSkipBlack );
			DeleteObject( hDstBmp );

			return S_OK;
		}

		//-----------------------------
		// (B) JPEG の場合
		FILE* fp;
		if( _tfopen_s( &fp, this->strFileName.c_str(), _T("rb") ) != 0 )
			return FDKERR_ファイルの読み込み失敗;

		BITMAPINFO* pbinfo = NULL;
		BYTE* pbdata = NULL;
		int h = fgetc( fp ) * 256 + fgetc( fp );
		fseek( fp, 0, SEEK_SET );
		if( h == 0xFFD8 && LoadJPEG( fp, &pbinfo, &pbdata ) != 0 )
		{
			// this->dwWidth, this->dwHeight がともに 0 なら画像サイズに等しくする
			if( this->dwWidth == 0 && this->dwHeight == 0 ) {
				this->dwWidth  = pbinfo->bmiHeader.biWidth;
				this->dwHeight = pbinfo->bmiHeader.biHeight;
				this->dwImageWidth  = pbinfo->bmiHeader.biWidth;
				this->dwImageHeight = pbinfo->bmiHeader.biHeight;
			}
			// this->dwWidth, this->dwHeight が画像より小さいなら画像のサイズに合わせる
			if( this->dwWidth < (DWORD) pbinfo->bmiHeader.biWidth ) {
				this->dwWidth = pbinfo->bmiHeader.biWidth;
				this->dwImageWidth = pbinfo->bmiHeader.biWidth;
			}
			if( this->dwHeight < (DWORD) pbinfo->bmiHeader.biHeight ) {
				this->dwHeight = pbinfo->bmiHeader.biHeight;
				this->dwImageHeight = pbinfo->bmiHeader.biHeight;
			}

			// 空のテクスチャを作成
			if( FAILED( hr = Create() ) )
			{
				SAFE_FREE( pbinfo );
				SAFE_FREE( pbdata );
				fclose( fp );
				return hr;	// 失敗
			}

			// メモリ内に RGB-32bit な BITMAP を作る
			BITMAPINFOHEADER bmi;
			ZeroMemory( &bmi, sizeof( BITMAPINFOHEADER ) );
			bmi.biSize		= sizeof( BITMAPINFOHEADER );
			bmi.biWidth		= this->dwWidth;
			bmi.biHeight	= this->dwHeight;		// トップダウンBitmap
			bmi.biPlanes	= 1;
			bmi.biBitCount	= 32;
			HDC hDC = CreateCompatibleDC( 0 );
			HBITMAP hBmp = CreateDIBSection( hDC, (BITMAPINFO *)&bmi, DIB_RGB_COLORS, NULL, NULL, 0 );

			// JPEG-BITMAP から RGB-32bit BITMAP へ転送する（拡大縮小なし）
			HBITMAP hOldBmp = (HBITMAP) SelectObject( hDC, hBmp );
			pbinfo->bmiHeader.biHeight = - pbinfo->bmiHeader.biHeight;	// JPEG-Bitmap をボトムアップで指示
			if( StretchDIBits(
				hDC,							// 転送先 DC
				0, 0, this->dwWidth, this->dwHeight,	// 転送先領域
				0, 0, this->dwWidth, this->dwHeight,	// 転送元領域
				(LPVOID)pbdata, pbinfo,			// 転送元DIB
				DIB_RGB_COLORS, SRCCOPY ) == GDI_ERROR )
			{
				SelectObject( hDC, hOldBmp );
				DeleteObject( hDC );
				DeleteObject( hBmp );
				SAFE_FREE( pbinfo );
				SAFE_FREE( pbdata );
				fclose( fp );
				return FDKERR_StretchBltに失敗;
			}
			SelectObject( hDC, hOldBmp );
			DeleteDC( hDC );

			// RGB-32bit BITMAP からテクスチャへ転送
			BITMAP bmp;
			GetObject( hBmp, sizeof(BITMAP), &bmp );
			CopyFromBMP32( 0, 0, &bmp, this->bSkipBlack );
			DeleteObject( hBmp );
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
			return S_OK;
		}
		else
		{
			SAFE_FREE( pbinfo );
			SAFE_FREE( pbdata );
		}

		//-----------------------------
		// (C) PNG の場合
		DDSURFACEDESC2 ddsd;
		BYTE** ppimage = NULL;
		fclose( fp );
		if( LoadPNGtoDDS( this->strFileName.c_str(), &ddsd, &ppimage ) )
		{
			hr = RestoreFromDDS( &ddsd, ppimage );
			if( ppimage )
			{
				for( DWORD i = 0; i < ddsd.dwHeight; i++ )
					SAFE_FREE( ppimage[i] );
				SAFE_FREE( ppimage );
			}
			return hr;
		}
	}
	return S_OK;
}

HRESULT CTexture::RestoreFromDDS( DDSURFACEDESC2* pddsd, BYTE** ppimage )
{
	if( pddsd == NULL || ppimage == NULL )
		return FDKERR_パラメータがNULL;

	HRESULT hr = S_OK;

	// this->dwWidth, this->dwHeight がともに 0 なら画像サイズに等しくする
	if( this->dwWidth == 0 && this->dwHeight == 0 )
	{
		this->dwWidth  = pddsd->dwWidth;
		this->dwHeight = pddsd->dwHeight;
		this->dwImageWidth  = pddsd->dwWidth;
		this->dwImageHeight = pddsd->dwHeight;
	}

	// this->dwWidth, this->dwHeight が画像サイズより小さいなら画像サイズに等しくする
	if( this->dwWidth  < pddsd->dwWidth  ) {
		this->dwWidth  = pddsd->dwWidth;
		this->dwImageWidth  = pddsd->dwWidth;
	}
	if( this->dwHeight < pddsd->dwHeight ) {
		this->dwHeight = pddsd->dwHeight;
		this->dwImageHeight = pddsd->dwHeight;
	}

	// 空のテクスチャを作成
	if( FAILED( hr = Create() ) )
		return hr;	// 失敗

	// テクスチャをロック
	DDSURFACEDESC2 ddsdtex;
	ZeroMemory( &ddsdtex, sizeof( DDSURFACEDESC2 ) );
	ddsdtex.dwSize = sizeof( DDSURFACEDESC2 );
	if( FAILED( hr = this->pTexture->Lock( NULL, &ddsdtex, DDLOCK_WAIT | DDLOCK_WRITEONLY, NULL ) ) )
		return FDKERR_Lockに失敗;

	// 順番に転送
	switch( CD3DApplication::nBpp )
	{

	case 16:
		for( DWORD y = 0; y < pddsd->dwHeight; y++ )
		{
			BYTE* pDst = ((BYTE*) ddsdtex.lpSurface) + y * ddsdtex.lPitch;
			for( DWORD x = 0; x < pddsd->dwWidth; x++ )
			{
				WORD c = ( ( ppimage[y][x*4+0] << 8 ) & 0xF000 )	// A
						|( ( ppimage[y][x*4+1] << 4 ) & 0x0F00 )	// R
						|( ( ppimage[y][x*4+2]      ) & 0x00F0 )	// G
						|( ( ppimage[y][x*4+3] >> 4 ) & 0x000F );	// B
				*pDst++ = c & 0x00FF;
				*pDst++ = ( c & 0xFF00 ) >> 8;
			}
		}
		break;

	case 32:
		for( DWORD y = 0; y < pddsd->dwHeight; y++ )
		{
			BYTE* pDst = ((BYTE*) ddsdtex.lpSurface) + y * ddsdtex.lPitch;
			for( DWORD x = 0; x < pddsd->dwWidth; x++ )
			{
				*pDst++ = ppimage[y][x*4+3];	// B
				*pDst++ = ppimage[y][x*4+2];	// G
				*pDst++ = ppimage[y][x*4+1];	// R
				*pDst++ = ppimage[y][x*4+0];	// A
			}
		}
		break;
	}

	// テクスチャのアンロック
	this->pTexture->Unlock( NULL );
	return S_OK;
}

HRESULT	CTexture::InvalidateDeviceObjects()
{
	SAFE_RELEASE( this->pTexture );
	return S_OK;
}

HRESULT	CTexture::DeleteDeviceObjects()
{
	SAFE_RELEASE( this->pTexture );
	return S_OK;
}

HRESULT CALLBACK TextureEnumerationCallback( DDPIXELFORMAT* pddpf, VOID* )
{
	DWORD dwFlags         = pddpf->dwFlags;
	DWORD dwFourCC        = pddpf->dwFourCC;
	DWORD dwTotalBitCount = pddpf->dwRGBBitCount;
	DWORD dwAlphaBitCount = 0;
	DWORD dwRedBitCount   = 0;
	DWORD dwGreenBitCount = 0;
	DWORD dwBlueBitCount  = 0;
	
	// 各色のビット数を数える
	DWORD mask;
	for( mask = pddpf->dwRGBAlphaBitMask; mask; mask>>=1 )
		dwAlphaBitCount += ( mask & 0x1 );
	for( mask = pddpf->dwRBitMask; mask; mask>>=1 )
		dwRedBitCount += ( mask & 0x1 );
	for( mask = pddpf->dwGBitMask; mask; mask>>=1 )
		dwGreenBitCount += ( mask & 0x1 );
	for( mask = pddpf->dwBBitMask; mask; mask>>=1 )
		dwBlueBitCount += ( mask & 0x1 );
	
	// 不正なフォーマットをチェックする
	if( ( dwFlags & DDPF_ALPHAPIXELS ) && ( dwAlphaBitCount == 0 ) )
		return DDENUMRET_OK;
	if( !( dwFlags & DDPF_ALPHAPIXELS ) && ( dwAlphaBitCount != 0 ) )
		return DDENUMRET_OK;
	if( !(dwFlags & DDPF_FOURCC ) && dwTotalBitCount == 0 )
		return DDENUMRET_OK;
	
	// ARGB8888 と ARGB4444 だけ抽出
	if( dwFlags & DDPF_RGB )
	{
		if( dwTotalBitCount == 32 && dwAlphaBitCount == 8 && dwRedBitCount == 8 && dwGreenBitCount == 8 && dwBlueBitCount == 8 )
			CTexture::ddpfARGB8888 = (*pddpf);
		if( dwTotalBitCount == 16 && dwAlphaBitCount == 4 && dwRedBitCount == 4 && dwGreenBitCount == 4 && dwBlueBitCount == 4 )
			CTexture::ddpfARGB4444 = (*pddpf);
		//Debug::OutFN( FNAME, _T("A%dR%dG%dB%d (%d-bit) format.\n"), dwAlphaBitCount, dwRedBitCount, dwGreenBitCount, dwBlueBitCount, dwTotalBitCount );
	}

	return DDENUMRET_OK;
}

void	CTexture::EnumTextureFormats()
{
	if( ! CD3DApplication::pD3DDevice )
		return;		// 3Dデバイスが無効

	CTexture::ddpfARGB8888.dwSize = 0;
	CTexture::ddpfARGB4444.dwSize = 0;

	HRESULT hr = CD3DApplication::pD3DDevice->EnumTextureFormats( TextureEnumerationCallback, (LPVOID) NULL );
	if( FAILED( hr ) )
		return;		// テクスチャフォーマットの列挙に失敗

	s_bEnumDone = true;
	return;
}

HRESULT CTexture::Create()
{
	HRESULT hr;

	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
	ddsd.dwSize				= sizeof( DDSURFACEDESC2 );
	ddsd.dwFlags			= DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT | DDSD_TEXTURESTAGE;
	ddsd.dwWidth			= this->dwWidth;
	ddsd.dwHeight			= this->dwHeight;
	ddsd.ddsCaps.dwCaps		= DDSCAPS_TEXTURE;
	ddsd.ddpfPixelFormat	= (CD3DApplication::nBpp == 16) ? CTexture::ddpfARGB4444 : CTexture::ddpfARGB8888;
	
	if( ddsd.ddpfPixelFormat.dwSize == 0 )
		return FDKERR_テクスチャフォーマットがない;

	// ３Ｄデバイスの能力を取得
	D3DDEVICEDESC7 ddDesc;
	if( FAILED( hr = CD3DApplication::pD3DDevice->GetCaps( &ddDesc ) ) )
		return FDKERR_Capsの取得に失敗;

	// ハードウェアデバイスだったらテクスチャの割り当て管理を任せる
	if( ddDesc.deviceGUID == IID_IDirect3DTnLHalDevice )
		ddsd.ddsCaps.dwCaps2 = DDSCAPS2_TEXTUREMANAGE;
	else if( ddDesc.deviceGUID == IID_IDirect3DHALDevice )
		ddsd.ddsCaps.dwCaps2 = DDSCAPS2_TEXTUREMANAGE;
    else
        ddsd.ddsCaps.dwCaps |= DDSCAPS_SYSTEMMEMORY;		// HEL だったらシステムメモリ上に作る

    // ドライバに合わせてサイズを調整
    if( ddDesc.dpcTriCaps.dwTextureCaps & D3DPTEXTURECAPS_POW2 )
    {
        for( ddsd.dwWidth  = 1; this->dwWidth  > ddsd.dwWidth;  ddsd.dwWidth  <<= 1 );
        for( ddsd.dwHeight = 1; this->dwHeight > ddsd.dwHeight; ddsd.dwHeight <<= 1 );
    }
	if( ddsd.dwWidth > ddDesc.dwMaxTextureWidth )
		ddsd.dwWidth = ddDesc.dwMaxTextureWidth;
	if( ddsd.dwHeight > ddDesc.dwMaxTextureHeight )
		ddsd.dwHeight = ddDesc.dwMaxTextureHeight;
    if( ddDesc.dpcTriCaps.dwTextureCaps & D3DPTEXTURECAPS_SQUAREONLY )
    {
        if( ddsd.dwWidth > ddsd.dwHeight ) ddsd.dwHeight = ddsd.dwWidth;
        else                               ddsd.dwWidth  = ddsd.dwHeight;
    }
	this->dwWidth  = ddsd.dwWidth;
	this->dwHeight = ddsd.dwHeight;

	// テクスチャの作成
	if( FAILED( hr = CD3DApplication::pDD->CreateSurface( &ddsd, &this->pTexture, NULL ) ) )
		return FDKERR_テクスチャの作成に失敗;

	// テクスチャのクリア
	Clear();

	// カラーキーの設定（黒）
	SetColorKey( ( CD3DApplication::nBpp == 16 ) ? 0x0000F000 : 0xFF000000 );

	return S_OK;
}

DWORD	CTexture::GetSurfaceSize()
{
	if( ! this->pTexture )
		return 0;

	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( ddsd ) );
	ddsd.dwSize	= sizeof( ddsd );
	if( FAILED( this->pTexture->GetSurfaceDesc( &ddsd ) ) )
		return 0;

	DWORD dwSize = 0;
	for( DWORD y = 0; y < ddsd.dwHeight; y++ )
		dwSize += ddsd.lPitch;

	return dwSize;
}
HRESULT CTexture::Draw( int x, int y, LPRECT pSrc )
{
	if( this->pTexture == NULL || CD3DApplication::pddsBackBuffer == NULL )
		return FDKERR_サーフェイスがNULL;

	this->LostCheck();	// ロストしていれば復旧する

	// レンダリングステート
	if( ! s_bBeginScene )
		BeginScene();
	SetRenderStates();
	
	// 描画
	float w = (pSrc == NULL) ? (float)this->dwWidth  : (float)(pSrc->right  - pSrc->left);
	float h = (pSrc == NULL) ? (float)this->dwHeight : (float)(pSrc->bottom - pSrc->top );
	float tu1 = (pSrc == NULL) ? 0.0f : pSrc->left / (float)this->dwWidth;
	float tv1 = (pSrc == NULL) ? 0.0f : pSrc->top / (float)this->dwHeight;
	float tu2 = (pSrc == NULL) ? 1.0f : pSrc->right / (float)this->dwWidth;
	float tv2 = (pSrc == NULL) ? 1.0f : pSrc->bottom / (float)this->dwHeight;
	
	D3DTLVERTEX vt[4];
	vt[0].sx  = (D3DVALUE) x - 0.5f;							// 左上
	vt[0].sy  = (D3DVALUE) y - 0.5f;
	vt[0].sz  = (D3DVALUE) 0.0f;
	vt[0].rhw = (D3DVALUE) 1.0f;
	vt[0].color = RGBA_MAKE( 255, 255, 255, this->nAlphaConst );
	vt[0].specular = RGB_MAKE( 0, 0, 0 );
	vt[0].tu  = (D3DVALUE) tu1;
	vt[0].tv  = (D3DVALUE) tv1;
	
	vt[1].sx  = (D3DVALUE) x + w * this->fScaleW - 0.5f;			// 右上
	vt[1].sy  = (D3DVALUE) y - 0.5f;
	vt[1].sz  = (D3DVALUE) 0.0f;
	vt[1].rhw = (D3DVALUE) 1.0f;
	vt[1].color = RGBA_MAKE( 255, 255, 255, this->nAlphaConst );
	vt[1].specular = RGB_MAKE( 0, 0, 0 );
	vt[1].tu  = (D3DVALUE) tu2;
	vt[1].tv  = (D3DVALUE) tv1;
	
	vt[2].sx  = (D3DVALUE) x - 0.5f;							// 左下
	vt[2].sy  = (D3DVALUE) y + h * this->fScaleH - 0.5f;
	vt[2].sz  = (D3DVALUE) 0.0f;
	vt[2].rhw = (D3DVALUE) 1.0f;
	vt[2].color = RGBA_MAKE( 255, 255, 255, this->nAlphaConst );
	vt[2].specular = RGB_MAKE( 0, 0, 0 );
	vt[2].tu  = (D3DVALUE) tu1;
	vt[2].tv  = (D3DVALUE) tv2;

	vt[3].sx  = (D3DVALUE) x + w * this->fScaleW - 0.5f;			// 右下
	vt[3].sy  = (D3DVALUE) y + h * this->fScaleH - 0.5f;
	vt[3].sz  = (D3DVALUE) 0.0f;
	vt[3].rhw = (D3DVALUE) 1.0f;
	vt[3].color = RGBA_MAKE( 255, 255, 255, this->nAlphaConst );
	vt[3].specular = RGB_MAKE( 0, 0, 0 );
	vt[3].tu  = (D3DVALUE) tu2;
	vt[3].tv  = (D3DVALUE) tv2;

	WORD pti[4] = { 0, 1, 2, 3 };
	CD3DApplication::pD3DDevice->SetTexture( 0, this->pTexture );
	CD3DApplication::pD3DDevice->DrawIndexedPrimitive( D3DPT_TRIANGLESTRIP, D3DFVF_TLVERTEX, vt, 4, pti, 4, 0 );
	return S_OK;
}

HRESULT CTexture::Draw3D( int x, int y, LPRECT prcSrc, D3DXMATRIX *pMat )
{
	if( CD3DApplication::pD3DDevice == NULL )
		return FDKERR_3DデバイスがNULL;

	this->LostCheck();	// ロストしていれば復旧する

	RECT rc = { 0, 0, this->dwWidth, this->dwHeight };
	if( ! prcSrc ) prcSrc = &rc;

	D3DXMATRIX world;
	if( pMat )
		world = *pMat;
	else
		D3DXMatrixIdentity( &world );

	// 現在の射影変換値を取得（前提は不変）
	float d, t, a;
	CD3DApplication::GetProjectionParam( &d, &t ,&a );

	// その他パラメータ計算
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
	ddsd.dwSize = sizeof( DDSURFACEDESC2 );
	CD3DApplication::pddsBackBuffer->GetSurfaceDesc( &ddsd );
	float devw = ddsd.dwWidth  / 2.0f;						// 320
	float devh = ddsd.dwHeight / 2.0f;						// 240
	float h = d * tan( t / 2.0f );							// 視野角の高さ/2
	float w = h / a;										// 視野角の幅/2
	float hw = (prcSrc->right - prcSrc->left) / 2.0f;		// 描画矩形の幅/2
	float hh = (prcSrc->bottom - prcSrc->top) / 2.0f;		// 描画矩形の高さ/2

	// カメラをスプライト用にセット
	D3DXMATRIX view;
	D3DXMatrixLookAtLH( &view, &D3DXVECTOR3( 0.0f, 0.0f, -d ), &D3DXVECTOR3( 0.0f, 0.0f, 0.0f ), &D3DXVECTOR3( 0.0f, 1.0f, 0.0f ) );
	CD3DApplication::pD3DDevice->SetTransform( D3DTRANSFORMSTATE_VIEW, (D3DMATRIX*)&view );

	// ワールド変換登録
	D3DXMATRIX mat, aux;
	float dx = -devw + hw + (float)x;
	float dy = -devh + hh + (float)y;
	D3DXMatrixTranslation( &aux, w * dx / devw, h * -dy / devh, 0.0f );
	mat = world * aux;		// world 適用後に (x,y) へ移動（ (x,y)は左上隅座標[dot] ）
	CD3DApplication::pD3DDevice->SetTransform( D3DTRANSFORMSTATE_WORLD, (D3DMATRIX*) &mat );

	// 描画
	D3DLVERTEX tv[4];

	tv[0].x			= -(w * hw / devw);			// 左上
	tv[0].y			= (h * hh / devh);
	tv[0].z			= 0.0f;
	tv[0].color		= RGBA_MAKE( 255, 255, 255, this->nAlphaConst );
	tv[0].specular	= RGB_MAKE( 0, 0, 0 );
	tv[0].tu		= (float)((float)prcSrc->left / (float)this->dwWidth);
	tv[0].tv		= (float)((float)prcSrc->top / (float)this->dwHeight);

	tv[1].x			= (w * hw / devw);			// 右上
	tv[1].y			= (h * hh / devh);
	tv[1].z			= 0.0f;
	tv[1].color		= tv[0].color;
	tv[1].specular	= RGB_MAKE( 0, 0, 0 );
	tv[1].tu		= (float)((float)prcSrc->right / (float)this->dwWidth);
	tv[1].tv		= (float)((float)prcSrc->top / (float)this->dwHeight);

	tv[2].x			= -(w * hw / devw);			// 左下
	tv[2].y			= -(h * hh / devh);
	tv[2].z			= 0.0f;			
	tv[2].color		= tv[0].color;
	tv[2].specular	= RGB_MAKE( 0, 0, 0 );
	tv[2].tu		= (float)((float)prcSrc->left / (float)this->dwWidth);
	tv[2].tv		= (float)((float)prcSrc->bottom / (float)this->dwHeight);

	tv[3].x			= (w * hw / devw);			// 右下
	tv[3].y			= -(h * hh / devh);
	tv[3].z			= 0.0f;
	tv[3].color		= tv[0].color;
	tv[3].tu		= (float)((float)prcSrc->right / (float)this->dwWidth);
	tv[3].tv		= (float)((float)prcSrc->bottom / (float)this->dwHeight);

//	m_sbAddBlend = this->bUseAddBlend;
	SetRenderStates();
	WORD pti[4] = { 0, 1, 2, 3 };
	CD3DApplication::pD3DDevice->SetTexture( 0, this->pTexture );
	CD3DApplication::pD3DDevice->DrawIndexedPrimitive( D3DPT_TRIANGLESTRIP, D3DFVF_LVERTEX, tv, 4, pti, 4, 0 );

	return S_OK;
}

void	CTexture::BeginScene()
{
	if( ! CD3DApplication::pD3DDevice )
		return;

	CD3DApplication::pD3DDevice->BeginScene();
	s_bBeginScene = true;
}

void	CTexture::EndScene()
{
	if( ! CD3DApplication::pD3DDevice )
		return;

	CD3DApplication::pD3DDevice->EndScene();
	s_bBeginScene = false;
}

void	CTexture::SetRenderStates()
{
	CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_COLORKEYENABLE, this->bColorKeyEnable ? TRUE : FALSE );

	if( this->bUseAddBlend )
	{
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHABLENDENABLE, TRUE );
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_SRCBLEND, D3DBLEND_SRCALPHA );	// アルファ値で加算度を調節可能
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_DESTBLEND, D3DBLEND_ONE );
	}
	else
	{
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHABLENDENABLE, TRUE );
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_SRCBLEND, D3DBLEND_SRCALPHA );	// アルファ値で加算度を調節可能
		CD3DApplication::pD3DDevice->SetRenderState( D3DRENDERSTATE_DESTBLEND, D3DBLEND_INVSRCALPHA );
	}
}

HRESULT CTexture::SetColorKey()
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;

	// 左上隅の点の色を取得
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof(ddsd) );
	ddsd.dwSize = sizeof(ddsd);
	if( SUCCEEDED( this->pTexture->Lock( NULL, &ddsd, DDLOCK_WAIT, NULL ) ) )
	{
		DWORD dwBits = ddsd.ddpfPixelFormat.dwRGBBitCount;
		this->dwColorKey = (*(DWORD*)ddsd.lpSurface) & ((dwBits == 16) ? 0x0000FFFF : 0xFFFFFFFF);
		this->pTexture->Unlock( NULL );

        // カラーキーとして設定
		DDCOLORKEY ddck;
		ddck.dwColorSpaceLowValue  = this->dwColorKey;
		ddck.dwColorSpaceHighValue = this->dwColorKey;
		this->pTexture->SetColorKey( DDCKEY_SRCBLT, &ddck );
	}
	return S_OK;
}

HRESULT CTexture::SetColorKey( DWORD dwColor )
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;

    // カラーキーとして設定
	DDCOLORKEY ddck;
	ddck.dwColorSpaceLowValue  = dwColor;
	ddck.dwColorSpaceHighValue = dwColor;
	this->pTexture->SetColorKey( DDCKEY_SRCBLT, &ddck );
	this->dwColorKey = dwColor;
	return S_OK;
}
HRESULT	CTexture::SetScale( float w, float h )
{
	this->fScaleW = w;
	this->fScaleH = h;
	return S_OK;
}

HRESULT CTexture::GetDC( HDC *phdc )
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;
	if( ! phdc )
		return E_INVALIDARG;

	this->LostCheck();	// ロストしていれば復旧する

	return this->pTexture->GetDC( phdc );
}

HRESULT CTexture::ReleaseDC( HDC &phdc )
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;

	return this->pTexture->ReleaseDC( phdc );
}

HRESULT	CTexture::SetAlphaConst( int a )
{
	if( a < 0 ) a = 0;
	if( a > 255 ) a = 255;
	this->nAlphaConst = a;
	return S_OK;
}

HRESULT CTexture::Clear( LPRECT prc )
{
	if( this->pTexture )
	{
		this->LostCheck();	// ロストしていれば復旧する

		DDBLTFX ddbltfx;
		ZeroMemory( &ddbltfx, sizeof( ddbltfx ) );
		ddbltfx.dwSize = sizeof( ddbltfx );
		ddbltfx.dwFillColor = 0;
		this->pTexture->Blt( prc, NULL, NULL, DDBLT_COLORFILL, &ddbltfx );
	}
	return S_OK;
}

DWORD	CTexture::BitCount( DWORD dwNum )
{
	DWORD dwCount;
	for( dwCount = 0; dwNum != 0; dwNum >>= 1 )
		dwCount += ( dwNum & 0x1 );
	return dwCount;
}
DWORD	CTexture::ShiftCount( DWORD dwNum )
{
	if( dwNum == 0 ) return 0;		// ゼロのときはゼロを返す。

	DWORD dwCount;
	for( dwCount = 0; (dwNum & 0x1) == 0; dwNum >>= 1 )
		dwCount ++;

	return dwCount;
}

HRESULT	CTexture::Text( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color )
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;
		
	this->LostCheck();	// ロストしていれば復旧する

	if( ! str || *str == _T('\0') )
		return S_OK;

	// BITMAP の作成
	BITMAPINFOHEADER bmi;
	ZeroMemory( &bmi, sizeof( BITMAPINFOHEADER ) );
	bmi.biSize = sizeof( BITMAPINFOHEADER );
	bmi.biWidth = (LONG) (this->dwWidth-x);
	bmi.biHeight =  - ((LONG)(this->dwHeight-y));
	bmi.biPlanes = 1;
	bmi.biBitCount = 32;

	HDC hDC = CreateCompatibleDC( 0 );
	HBITMAP hBmp = CreateDIBSection( hDC, (BITMAPINFO *)&bmi, DIB_RGB_COLORS, NULL, NULL, 0 );

	// メモリDC を取得して BITMAP へテキスト描画
	HBITMAP hOldBmp = (HBITMAP) SelectObject( hDC, hBmp  );
	HFONT hOldFont = (HFONT) SelectObject( hDC, hFont );
	SetTextColor( hDC, color );
	SetBkColor( hDC, RGB(0,0,0) );
	RECT rc = { 0, 0, this->dwWidth-x, this->dwHeight-y };
	DrawText( hDC, str, lstrlen(str), &rc, DT_LEFT|DT_NOPREFIX );
	SelectObject( hDC, hOldFont );
	SelectObject( hDC, hOldBmp );
	DeleteDC( hDC );

	// ビットマップからテクスチャへ転送
	BITMAP bmp;
	GetObject( hBmp, sizeof(BITMAP), &bmp );
	CopyFromBMP32( x, y, &bmp, true );
	DeleteObject( hBmp );

	return S_OK;
}

HRESULT	CTexture::TextSharp( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color )
{
	if( ! this->pTexture )
		return FDKERR_テクスチャがNULL;
		
	this->LostCheck();	// ロストしていれば復旧する

	if( ! str || *str == _T('\0') )
		return S_OK;

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

	// ビットマップからテクスチャへ転送
	BITMAP bmp;
	GetObject( hBmp, sizeof(BITMAP), &bmp );
	CopyFromBMP32x4( x, y, &bmp, true );
	DeleteObject( hBmp );

	return S_OK;
}
int		CTexture::GetTextWidthDot( HFONT hFont, LPCTSTR str文字列 )
{
	HDC hdc = ::CreateCompatibleDC( 0 );
	HFONT hOldFont = (HFONT) ::SelectObject( hdc, hFont );
	SIZE size;
	::GetTextExtentPoint32( hdc, str文字列, lstrlen(str文字列), &size );
	::SelectObject( hdc, hOldFont );
	::DeleteDC( hdc );
	return (int) size.cx;
}
void	CTexture::CopyFromBMP32( int x, int y, BITMAP* bmp, bool bSkipBlack )
{
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
	ddsd.dwSize = sizeof( DDSURFACEDESC2 );
	if( SUCCEEDED( this->pTexture->Lock( NULL, &ddsd, DDLOCK_WAIT|DDLOCK_WRITEONLY, NULL ) ) )
	{
		BYTE* src_line = (BYTE*) bmp->bmBits;
		BYTE* dst_line = (BYTE*) ddsd.lpSurface;

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
							DWORD c = *src & 0x00FFFFFF;
							if( ! bSkipBlack || c != 0x00000000 )		// 色 0 をスキップ？
								*dst = c | 0xFF000000;					// 文字のαは常に 255
							src ++;
						}
						dst ++;
					}
					src_line += bmp->bmWidthBytes;
				}
				dst_line += ddsd.lPitch;
			}
		}
		else if ( ddsd.ddpfPixelFormat.dwRGBBitCount == 16 )
		{
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
							WORD rgb = (WORD)(((*src & 0x00FF0000) >> 20) << 8)
									 | (WORD)(((*src & 0x0000FF00) >> 12) << 4)
									 | (WORD)( (*src & 0x000000FF) >>  4);
							if( ! bSkipBlack || rgb != 0x0000 )			// 色 0 をスキップ？
								*dst = rgb | 0xF000;					// 文字のαは常に 255(15)
							src ++;
						}
						dst ++;
					}
					src_line += bmp->bmWidthBytes;
				}
				dst_line += ddsd.lPitch;
			}
		}

		this->pTexture->Unlock( NULL );
	}
}

void	CTexture::CopyFromBMP32x4( int x, int y, BITMAP* bmp, bool bSkipBlack )
{
	DDSURFACEDESC2 ddsd;
	ZeroMemory( &ddsd, sizeof( DDSURFACEDESC2 ) );
	ddsd.dwSize = sizeof( DDSURFACEDESC2 );
	if( SUCCEEDED( this->pTexture->Lock( NULL, &ddsd, DDLOCK_WAIT|DDLOCK_WRITEONLY, NULL ) ) )
	{
		BYTE* src_line = (BYTE*) bmp->bmBits;
		BYTE* dst_line = (BYTE*) ddsd.lpSurface;

		// A. 転送先テクスチャが 32bpp の場合
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
		// B. 転送先テクスチャが 16bpp の場合
		else if ( ddsd.ddpfPixelFormat.dwRGBBitCount == 16 )
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

		this->pTexture->Unlock( NULL );
	}
}

void	CTexture::LostCheck()
{
	if( this->pTexture != NULL && this->pTexture->IsLost() == DDERR_SURFACELOST )
		this->RestoreDeviceObjects();
}
	}//Graphics
}//FDK