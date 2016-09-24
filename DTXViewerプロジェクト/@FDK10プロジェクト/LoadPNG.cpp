#include "stdafx.h"
#include <windows.h>
#include <string.h>
#include <setjmp.h>
#include <tchar.h>
#include <ddraw.h>
#include "png.h"
#include "LoadPNG.h"
#include "Debug.h"

namespace FDK {
	namespace Graphics {

static void ReadPNGFunc( png_struct* pPng, png_bytep buf, png_size_t size );

// png ライブラリがデータを読み込むためのコールバック関数。
void ReadPNGFunc( png_struct* pPng, png_bytep buf, png_size_t size )
{
	unsigned char** p = (unsigned char**) png_get_io_ptr( pPng );
	memcpy( buf, *p, size );
	*p += size;
}

bool LoadPNGtoDDS( BYTE* pData, DWORD dwDataSize, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData )
{
	png_struct*	png_ptr;
	png_info*	info_ptr;

	//------------------------------------
	// PNG の読み込み

	// シグネチャ確認
	if( !png_check_sig( pData, dwDataSize ) )
		return false;				// PNG ではない

	// PNG構造体の初期化
	png_ptr  = png_create_read_struct( PNG_LIBPNG_VER_STRING, NULL, NULL, NULL );
	info_ptr = png_create_info_struct( png_ptr );

	// libpng にメモリイメージを読み込む関数を通知
	png_set_read_fn( png_ptr, (png_voidp)&pData, (png_rw_ptr)ReadPNGFunc );
	// ※参考：
	// FILE* fp から読み込む場合は
	// png_init_io( png_ptr, fp );
	// と設定する。

	// PNGファイルのヘッダを読み込む
	png_read_info( png_ptr, info_ptr );

	// IHDR チャンクを取得する
	png_uint_32 width, height;
	int bit_depth, color_type;
	png_get_IHDR( png_ptr, info_ptr,
		&width,				// 幅[ドット]
		&height,			// 高さ[ドット]
		&bit_depth,			// 色深度(1,2,4,8,16 [bits/channel] )
		&color_type,		// 色種別( PNG_COLOR_TYPE_GRAY | GRAY_ALPHA | PALETTE | RGB | RGB_ALPHA )
		NULL,				// インターレースタイプ（png_read_image() で一括読み込みするならこの情報は不要）
		NULL,				// 圧縮形式
		NULL				// フィルタタイプ
	);

	static TCHAR *s_coltype[7] = { _T("GRAY"),_T("1"),_T("RGB"),_T("PALETTE"),_T("GRYA_ALPHA"),_T("5"),_T("RGB_ALPHA") };

	// 入力変換指定
	//   最終的に、8bits/channel (32bpp) の RGBA に仕立て上げる。
	switch( color_type )
	{
	case PNG_COLOR_TYPE_GRAY:			// 0
		if( bit_depth < 8 ) png_set_gray_to_rgb( png_ptr );
		if( bit_depth == 16 ) png_set_strip_16( png_ptr );
		png_set_add_alpha( png_ptr, 0xff, PNG_FILLER_AFTER );
		png_set_gray_to_rgb( png_ptr );
		break;

	case PNG_COLOR_TYPE_GRAY_ALPHA:		// 4
		if( bit_depth < 8 ) png_set_gray_to_rgb( png_ptr );
		if( bit_depth == 16 ) png_set_strip_16( png_ptr );
		png_set_gray_to_rgb( png_ptr );
		break;

	case PNG_COLOR_TYPE_PALETTE:		// 3
		png_set_palette_to_rgb( png_ptr );						// α付パレットはちゃんと RGBA-8bits/channel になる。
		png_set_add_alpha( png_ptr, 0xff, PNG_FILLER_BEFORE );	// RGB だったら RGBA にする。(すでに RGBA なら影響しない）
		break;

	case PNG_COLOR_TYPE_RGB:			// 2
	    if( bit_depth < 8 ) png_set_packing( png_ptr );
		if( bit_depth == 16 ) png_set_strip_16( png_ptr );
		png_set_add_alpha( png_ptr, 0xff, PNG_FILLER_BEFORE );
		break;

	case PNG_COLOR_TYPE_RGB_ALPHA:		// 6
	    if( bit_depth < 8 ) png_set_packing( png_ptr );
		if( bit_depth == 16 ) png_set_strip_16( png_ptr );
		break;

	default:							// その他の組み合わせは未対応
		png_destroy_read_struct( &png_ptr, &info_ptr, (png_info**) NULL );
		return false;
	}

	// RGBA → ARGB にする
	png_set_swap_alpha( png_ptr );

	// 設定反映
	png_read_update_info( png_ptr, info_ptr );
//	Debug::Out(_T(__FUNCTION__"; 色深度=%d, 色種別=%s\n"),
//		png_get_bit_depth( png_ptr, info_ptr ), s_coltype[ png_get_color_type( png_ptr, info_ptr )]  );

	// 画像の読み込み
	png_byte** image = (png_byte**) malloc( height * png_get_rowbytes( png_ptr, info_ptr ) );
	if( ! image )
		return false;		// メモリ不足

	for( int i = 0; i < (int)height; i++ )
	{
		image[i] = (png_byte*) malloc( png_get_rowbytes( png_ptr, info_ptr ) );
		if( ! image[i] )
		{
			for( int j = 0; j < i; j++ )
				SAFE_FREE( image[j] );
			SAFE_FREE( image );
			return false;			// メモリ不足
		}
	}
	png_read_image( png_ptr, image );

	//------------------------------------
	// image[][] から DDS を作成する
	ZeroMemory( pddsd2, sizeof(pddsd2) );
	pddsd2->dwSize			= 124;
	pddsd2->dwFlags			= DDSD_CAPS | DDSD_WIDTH | DDSD_HEIGHT | DDSD_PITCH | DDSD_PIXELFORMAT;
	pddsd2->dwHeight		= height;
	pddsd2->dwWidth			= width;
	pddsd2->lPitch			= png_get_rowbytes( png_ptr, info_ptr );
	pddsd2->dwDepth			= 0;
	pddsd2->dwMipMapCount	= 0;
	pddsd2->ddpfPixelFormat.dwSize				= 32;
	pddsd2->ddpfPixelFormat.dwFlags				= DDPF_RGB | DDPF_ALPHAPIXELS;
	pddsd2->ddpfPixelFormat.dwFourCC			= 0;
	pddsd2->ddpfPixelFormat.dwRGBBitCount		= 32;
	pddsd2->ddpfPixelFormat.dwRBitMask			= 0x00FF0000;
	pddsd2->ddpfPixelFormat.dwGBitMask			= 0x0000FF00;
	pddsd2->ddpfPixelFormat.dwBBitMask			= 0x000000FF;
	pddsd2->ddpfPixelFormat.dwRGBAlphaBitMask	= 0xFF000000;
	pddsd2->ddsCaps.dwCaps	= DDSCAPS_TEXTURE;
	pddsd2->ddsCaps.dwCaps2	= 0;

	*pppbyData = image;

	//------------------------------------
	// PNG の解放
	png_destroy_read_struct( &png_ptr, &info_ptr, (png_info**) NULL );

	return true;
}

bool LoadPNGtoDDS( FILE* fp, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData )
{
	if( fp == NULL ) 
		return false;

	// ファイルをメモリイメージに読み込む
	fseek( fp, 0, SEEK_END );
	size_t size = ftell( fp );
	fseek( fp, 0, SEEK_SET );
	BYTE* pData = (BYTE*) malloc( size );
	if( ! pData )
		return false;		// メモリ不足
	fread( pData, size, 1, fp );

	// メモリイメージから PNG を生成
	bool br = LoadPNGtoDDS( pData, (DWORD)size, pddsd2, pppbyData );
	
	free( pData );
	return br;
}

bool LoadPNGtoDDS( LPCTSTR fileName, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData )
{
	bool bRet = true;
	HANDLE hFile = NULL;
	HANDLE hFileMapping = NULL;

	// ファイルをメモリマップでオープン
	hFile = CreateFile( fileName, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL ); 
	if( hFile == INVALID_HANDLE_VALUE )
		goto error;
	DWORD dwFileSize = GetFileSize( hFile, NULL );	// ファイルサイズ
	hFileMapping = CreateFileMapping( hFile, NULL, PAGE_READONLY, 0, dwFileSize, NULL );
	if( hFileMapping == NULL )
		goto error;
	LPBYTE pFileAddress = (LPBYTE) MapViewOfFile( hFileMapping, FILE_MAP_READ, 0, 0, 0 );	// ファイル全体をマッピング
	if( pFileAddress == NULL )
		goto error;

	// メモリイメージから PNG を生成
	bRet = LoadPNGtoDDS( pFileAddress, dwFileSize, pddsd2, pppbyData );

ret:
	if( pFileAddress ) UnmapViewOfFile( pFileAddress );
	if( hFileMapping ) CloseHandle( hFileMapping );
	if( hFile ) CloseHandle( hFile );
	return bRet;

error:
	bRet = false;
	goto ret;
}

bool LoadPNGtoDIB( BYTE* pData, DWORD dwDataSize, BITMAPINFO** ppInfo, BYTE** ppBMP )
{
	// DDS で読み込む
	DDSURFACEDESC2 ddsd2;
	BYTE** ppImage = NULL;
	if( ! LoadPNGtoDDS( pData, dwDataSize, &ddsd2, &ppImage ) )
		return false;

	// DDS (ARGB-32bits/pixel) から DIB を生成
	*ppInfo = (BITMAPINFO*) malloc( sizeof( BITMAPINFOHEADER ) );		// パレットなしBITMAPINFO = BITMAPINFOHEADER
	if( ! ppInfo )
		return false;		// メモリ不足
	ZeroMemory( *ppInfo, sizeof( BITMAPINFOHEADER ) );
	(*ppInfo)->bmiHeader.biSize			= sizeof( BITMAPINFOHEADER );
	(*ppInfo)->bmiHeader.biWidth		= ddsd2.dwWidth;
	(*ppInfo)->bmiHeader.biHeight		= ddsd2.dwHeight;
	(*ppInfo)->bmiHeader.biPlanes		= 1;
	(*ppInfo)->bmiHeader.biBitCount		= 32;
	(*ppInfo)->bmiHeader.biCompression	= BI_RGB;
	(*ppInfo)->bmiHeader.biSizeImage	= 4 * ddsd2.dwWidth * ddsd2.dwHeight;
	
	// イメージデータのコピー（WINDOWS では上下が逆なので注意）
	BYTE* dest = *ppBMP = (BYTE*) malloc( (*ppInfo)->bmiHeader.biSizeImage );
	if( ! dest )
	{
		free( ppInfo );
		return false;		// メモリ不足
	}
	for( int y = ((int)ddsd2.dwHeight)-1; y >= 0; y-- )
	{
		for( int x = 0; (ULONG) x < ddsd2.dwWidth; x++ )
		{
			*dest++ = ppImage[y][x*4+3];	// B
			*dest++ = ppImage[y][x*4+2];	// G
			*dest++ = ppImage[y][x*4+1];	// R
			*dest++ = 0x00;					// Reserved
		}
		free( ppImage[y] );
	}
	free( ppImage );

	return true;
}

bool LoadPNGtoDIB( FILE* fp, BITMAPINFO** ppInfo, BYTE** ppBMP )
{
	if( fp == NULL ) 
		return false;

	// ファイルをメモリイメージに読み込む
	fseek( fp, 0, SEEK_END );
	size_t size = ftell( fp );
	fseek( fp, 0, SEEK_SET );
	BYTE* pData = (BYTE*) malloc( size );
	if( ! pData )
		return false;	// メモリ不足
	fread( pData, size, 1, fp );

	// メモリイメージから PNG を生成
	bool br = LoadPNGtoDIB( pData, (DWORD)size, ppInfo, ppBMP );
	
	free( pData );
	return br;
}

bool LoadPNGtoDIB( LPCTSTR fileName, BITMAPINFO** ppInfo, BYTE** ppBMP )
{
	bool bRet = true;
	HANDLE hFile = NULL;
	HANDLE hFileMapping = NULL;

	// ファイルをメモリマップでオープン
	hFile = CreateFile( fileName, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL ); 
	if( hFile == INVALID_HANDLE_VALUE )
		goto error;
	DWORD dwFileSize = GetFileSize( hFile, NULL );	// ファイルサイズ
	hFileMapping = CreateFileMapping( hFile, NULL, PAGE_READONLY, 0, dwFileSize, NULL );
	if( hFileMapping == NULL )
		goto error;
	LPBYTE pFileAddress = (LPBYTE) MapViewOfFile( hFileMapping, FILE_MAP_READ, 0, 0, 0 );	// ファイル全体をマッピング
	if( pFileAddress == NULL )
		goto error;

	// メモリイメージから PNG を生成
	bRet = LoadPNGtoDIB( pFileAddress, dwFileSize, ppInfo, ppBMP );
	
ret:
	if( pFileAddress ) UnmapViewOfFile( pFileAddress );
	if( hFileMapping ) CloseHandle( hFileMapping );
	if( hFile ) CloseHandle( hFile );
	return bRet;

error:
	bRet = false;
	goto ret;
}

	}//Graphics
}//FDK