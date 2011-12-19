#pragma once

#include "CD3DApplication.h"
#include "FDKError.h"

namespace FDK {
	namespace Graphics {
		
enum LOADPLACE {
	LOADTO_VIDEO,				// VIDEO MEMORY に作成し、失敗したらエラー。
	LOADTO_SYSTEM,				// SYSTEM MEMORY に作成し、失敗したらエラー。
	LOADTO_VIDEO_OR_SYSTEM,		// VIDEO MEMORY に作成し、失敗したら SYSTEM MEMORY に作成し、さらに失敗したらエラー。
	LOADTO_NULL

	// 注意：
	// (1) サーフェイスもなるべく VIDEO に作るべき（LOADTO_SYSTEM を使うと表示がクソ重たくなるグラボがある（ATI ALL-IN-WONDER128など））だが、
	//     サーフェイスにはテクスチャのような自動管理機能はないため、あまり VIDEO を占有するとテクスチャの活用できる VIDEO が減るので
	// 　　バランスを考えること。
	// 　　ビデオメモリが足らない場合、テクスチャはその残り少ない領域でまわさなければならなくなる。
	// (2) CSurface へ Blt するためのサーフェイスは必ず LOADTO_VIDEO で作成すること。(SYSTEM不可)
};

class CSurface
{
public:
	// 空サーフェイス作成のための初期設定(1)。
	// ここではサーフェイスの 名前、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	void InitDeviceObjects( LPCTSTR name, DWORD width, DWORD height, LOADPLACE place=LOADTO_VIDEO_OR_SYSTEM, LPDDPIXELFORMAT pFormat=NULL );

	// 空サーフェイス作成のための初期設定(2)。
	// ここではサーフェイスの 名前、ファイル名、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	void InitDeviceObjectsFromFile( LPCTSTR name, LPCTSTR filename, DWORD width=0, DWORD height=0, LOADPLACE place=LOADTO_VIDEO_OR_SYSTEM, LPDDPIXELFORMAT pFormat=NULL );

	// メモリファイルからサーフェイスを作成するための初期設定。
	// ここではサーフェイスの 名前、データ、データサイズ、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	void InitDeviceObjectsFromMemory( LPCTSTR name, BYTE* pData, DWORD dwDataSize, DWORD width=0, DWORD height=0, LOADPLACE place=LOADTO_VIDEO_OR_SYSTEM, LPDDPIXELFORMAT pFormat=NULL );

	// リソースからサーフェイスを作成するための初期設定。
	// ここではサーフェイスの 名前、データ、データサイズ、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	void InitDeviceObjectsFromResource( LPCTSTR name, LPCTSTR lpResourceName, LPCTSTR lpResourceType, DWORD width=0, DWORD height=0, LOADPLACE place=LOADTO_VIDEO_OR_SYSTEM, LPDDPIXELFORMAT pFormat=NULL );

	// サーフェイスの読み込みと構築。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjects();

	HRESULT	InvalidateDeviceObjects();		// サーフェイスの無効化（内部状態は維持）
	HRESULT	DeleteDeviceObjects();			// サーフェイスの破棄

	void	SetAlphaConst( int a );			// 固定α値の登録（a = 0～255）
	HRESULT	SetColorKey();					// 左上隅の色をカラーキーに設定する
	HRESULT	SetColorKey( DWORD dwColor );	// 指定色をカラーキーに設定する
	void	ColorKeyEnable( bool f=true )	{ this->bColorKeyEnable = f; }		// カラーキーの ON/OFF。true で ON。
	HRESULT GetDC( HDC *phdc );				// デバイスコンテキストの取得
	HRESULT ReleaseDC( HDC &hdc );			// デバイスコンテキストの解放
	HRESULT Clear( LPRECT prc=NULL );		// サーフェイスのクリア
	
	// 指定サーフェイスへの描画(1)
	// Appx: CTexture::m_pTexture へ描画する場合は、CSurface は LOADTO_VIDEO であること。
	// 　　　(Blt は同じフォーマットでないと転送できないっぽい。DD_OK は返すが。）
	HRESULT Draw( LPDIRECTDRAWSURFACE7 pDest, int x, int y, LPCRECT pSrc=NULL );
	
	// 指定サーフェイスへの描画(2)
	// Appx: CTexture::m_pTexture へ描画する場合は、CSurface は LOADTO_VIDEO であること。
	// 　　　(Blt は同じフォーマットでないと転送できないっぽい。DD_OK は返すが。）
	HRESULT Draw( int x, int y, LPCRECT pSrc=NULL )	{
		return Draw( CD3DApplication::pddsBackBuffer, x, y, pSrc );
	}
	
	// 指定サーフェイスへの描画(3)
	// Appx: CTexture::m_pTexture へ描画する場合は、CSurface は LOADTO_VIDEO であること。
	// 　　　(Blt は同じフォーマットでないと転送できないっぽい。DD_OK は返すが。）
	HRESULT Draw( CSurface *pDest, int x, int y, LPRECT pSrc=NULL ) {
		return Draw( pDest->pSurface, x, y, pSrc );
	}
	
	// m_nAlphaConst(<255) を反映させた描画（ソフト処理）。
	// 転送先がアルファを持つ場合（テクスチャなど）、転送先のアルファは
	// 常に最大値となり、色値の方で m_nAlphaConst が反映される。
	// ただし転送先が 24bit の場合は、アルファを持っていてもサポートはしない。
	HRESULT BlendDraw( LPDIRECTDRAWSURFACE7 pDest, int x, int y, LPCRECT pSrc=NULL );

	// サーフェイスへの文字列描画(1)
	HRESULT	Text( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color=RGB(255,255,255) );

	// サーフェイスへの文字列描画(2)
	HRESULT	Text( int x, int y, HFONT hFont, tstring &str, COLORREF color=RGB(255,255,255) ) {
		return Text(x,y,hFont,str.c_str(),color);
	}
	// サーフェイスへの文字列描画に必要な幅（ドット）を返す
	int	GetTextWidthDot( HFONT hFont, LPCTSTR str文字列 );

	// サーフェイスへの文字列描画(3)アンチエイリアス付き
	// ※フォントは面積４倍の大きさのものを指定すること！
	HRESULT	TextSharp( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color=RGB(255,255,255) );

	// サーフェイスサイズの取得
	DWORD GetSurfaceSize();

	CSurface();
	virtual ~CSurface();

public:
	LPDIRECTDRAWSURFACE7	pSurface;

	int				nAlphaConst;			// アルファ値（定数 0～255）
	DWORD			dwWidth;				// サーフェイスの幅
	DWORD			dwHeight;				// サーフェイスの高さ
	LOADPLACE		place;					// サーフェイスの配置場所
	bool			bColorKeyEnable;		// カラーキーが有効なら true
	DWORD			dwColorKey;				// カラーキー(32bit)
	tstring			strSurfaceName;			// サーフェイス名（任意）
	tstring			strFileName;			// ファイル名（ファイル生成じゃないなら "" ）
	LPCTSTR			lpResourceName;			// リソース名（リソース生成じゃないなら NULL）
	LPCTSTR			lpResourceType;			// リソースタイプ（リソース生成じゃないなら NULL）
	BYTE*			pData;					// データへのポインタ（メモリからの生成じゃないならNULL）
	DWORD			dwDataSize;				// データサイズ（メモリからの生成じゃないなら0）
	DDPIXELFORMAT	ddpfReference;			// Create時のリファレンスフォーマット

protected:
	// サーフェイスの名前、ファイル名、サイズ、配置場所を内部に記憶する。
	// その他のパラメータはデフォルト値に初期化する。
	void	InitParameters( LPCTSTR name, LPCTSTR fname, LPCTSTR lpResourceName, LPCTSTR lpResourceType, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, LOADPLACE place, LPDDPIXELFORMAT pFormat );

	// ファイルからのサーフェイスの読み込みと構築。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromFile();

	// メモリイメージからのサーフェイスの読み込みと構築。
	// ※ PNG のみ対応
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromFileInMemory();

	// リソースからのサーフェイスの読み込みと構築。
	// ※ PNG のみ対応。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromResource();

	// ビットマップ hBitmap からサーフェイスを構築する。
	HRESULT	RestoreFromBitmap( HBITMAP hBitmap );

	// JPEG/PNG データからサーフェイスを構築する。
	HRESULT	RestoreFromBitmap( BITMAPINFO* pbinfo, BYTE* pbdata );

	// 空のサーフェイスの作成；
	// サイズ（m_dwWidth × m_dwHeight）の空のサーフェイスを作成する。
	HRESULT Create();

	void	BlendDraw16to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw16to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw16to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw24to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw24to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw24to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw32to16( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw32to24( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	BlendDraw32to32( LPDDSURFACEDESC2 pddsdSrc, LPCRECT prcSrc, LPDDSURFACEDESC2 pddsdDst, int x, int y );
	void	CopyFromBMP32x4( int x, int y, BITMAP* bmp, bool bSkipBlack );

	DWORD	BitCount( DWORD dwNum );		// dwNum のビット'1'の数を数える。
	DWORD	ShiftCount( DWORD dwNum );		// dwNum のLSBから0の続く個数を返す。（例：0x0020 → 5）

	void	LostCheck();		// サーフェイスがロストしてたら RestoreDeviceObject()を呼び出す。
};

	}//Graphics
}//FDK

using namespace FDK::Graphics;
