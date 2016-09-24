#pragma once

namespace FDK {
	namespace Graphics {

#define MAX_NUM_FORMATS		64		// 列挙する最大のピクセルフォーマット数

class CTexture
{
public:
	// 空テクスチャ作成のための初期設定(1)。
	// ここではテクスチャの 名前、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	HRESULT InitDeviceObjects( TCHAR* name, DWORD width, DWORD height, bool bSkipBlack=true );
	
	// 空テクスチャ作成のための初期設定(2)。
	// ここではテクスチャの 名前、ファイル名、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	HRESULT InitDeviceObjectsFromFile( LPCTSTR name, LPCTSTR filename, DWORD width=0, DWORD height=0, bool bSkipBlack=true );

	// メモリファイルからテクスチャを作成するための初期設定。
	// ここではテクスチャの 名前、データ、データサイズ、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	HRESULT InitDeviceObjectsFromFileInMemory( TCHAR* name, BYTE* pData, DWORD dwDataSize, DWORD width=0, DWORD height=0, bool bSkipBlack=true );

	// リソースからテクスチャを作成するための初期設定。
	// ここではテクスチャの 名前、データ、データサイズ、サイズ、配置場所 を内部に記憶させるだけで、デバイス関係の処理は一切行わない。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	// ※ width == 0 & height == 0 の場合は、ファイル画像のサイズになる。
	HRESULT	InitDeviceObjectsFromResource( TCHAR* name, LPCTSTR lpResourceName, LPCTSTR lpResourceType, DWORD width=0, DWORD height=0, bool bSkipBlack=true );

	// テクスチャの読み込みと構築。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjects();

	HRESULT	InvalidateDeviceObjects();		// テクスチャの無効化。
	HRESULT	DeleteDeviceObjects();			// テクスチャの破棄

	HRESULT	SetAlphaConst( int a );													// 固定α値の登録(a = 0～255)
	HRESULT	AddBlendEnable( bool f=true )	{ this->bUseAddBlend = f; return S_OK; }
	HRESULT	SetColorKey();															// 左上隅の色をカラーキーにする。
	HRESULT	SetColorKey( DWORD dwColor );											// 指定色をカラーキーに設定する。
	void	ColorKeyEnable( bool f=true )	{ this->bColorKeyEnable = f; }			// カラーキーの ON/OFF。true で ON。
	HRESULT GetDC( HDC *phdc );														// デバイスコンテキストの取得
	HRESULT ReleaseDC( HDC &hdc );													// デバイスコンテキストの解放
	HRESULT	SetScale( float w, float h );											// スケールの変更
	void	GetScale( float* w, float* h ) { *w=this->fScaleW; *h=this->fScaleH; }	// スケールの取得

	HRESULT Clear( LPRECT prc=NULL );												// サーフェイスのクリア
	HRESULT Draw( int x, int y, LPRECT pSrc=NULL );									// ２Ｄ描画：バックバッファへの書き込みしかサポートしない
	HRESULT Draw3D( int x, int y, LPRECT prcSrc, D3DXMATRIX *pMat );				// ３Ｄ空間への描画：バックバッファへの書き込みしかサポートしない

	// テクスチャへの文字列の描画(1)
	HRESULT	Text( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color=RGB(255,255,255) );
	int		GetTextWidthDot( HFONT hFont, LPCTSTR str文字列 );

	// テクスチャへの文字列の描画(2)アンチエイリアス付き
	// ※フォントは面積４倍の大きさのものを指定すること！
	// ※アルファは常に255なので注意すること！（スキップされる黒部分は除く）
	HRESULT	TextSharp( int x, int y, HFONT hFont, LPCTSTR str, COLORREF color=RGB(255,255,255) );

	// テクスチャサーフェイスサイズの取得
	DWORD	GetSurfaceSize();

	static void EnumTextureFormats();	// テクスチャフォーマットリストの作成
	static void BeginScene();			// 描画開始
	static void EndScene();				// 描画終了

	CTexture();
	virtual ~CTexture();

public:
	LPDIRECTDRAWSURFACE7	pTexture;

	int				nAlphaConst;
	bool			bUseAddBlend;
	bool			bColorKeyEnable;
	DWORD			dwColorKey;
	DWORD			dwWidth;		// テクスチャの実サイズ
	DWORD			dwHeight;	
	DWORD			dwImageWidth;	// 読み込み元画像のサイズ
	DWORD			dwImageHeight;
	float			fScaleW;		// 横方向倍率
	float			fScaleH;		// 縦方向倍率
	tstring			strTextureName;					// テクスチャ名（任意）
	tstring			strFileName;					// ファイル名（ファイル生成じゃないなら "" ）
	bool			bSkipBlack;		// 画像構築時(RestoreDevice～) に、画像の黒(0)をスキップするか(BMP,JPEGのみ有効)
	LPCTSTR			lpResourceName;					// リソース名（リソース生成じゃないなら NULL）
	LPCTSTR			lpResourceType;					// リソースタイプ（リソース生成じゃないなら NULL）
	BYTE*			pData;							// データへのポインタ（メモリからの生成じゃないならNULL）
	DWORD			dwDataSize;						// データサイズ（メモリからの生成じゃないなら0）

	static DDPIXELFORMAT	ddpfARGB8888;				// A8R8G8B8 (32-bit) フォーマット。dwSize = 0 なら使えない。
	static DDPIXELFORMAT	ddpfARGB4444;				// A4R4G4B4 (16-bit) フォーマット。dwSize = 0 なら使えない。

protected:
	// 空のテクスチャの作成
	HRESULT Create();
	
	// レンダリングステータスの設定
	void	SetRenderStates();

	// RGB-32bit の BITMAP からテクスチャへコピーする。
	// テクスチャのアルファは 0xFF となる。
	// 　bSkipBlack ... true なら黒色は描画せずスキップする。
	void	CopyFromBMP32( int x, int y, BITMAP* bmp, bool bSkipBlack=true );

	// RGB-32bit の４倍面積 BITMAP からテクスチャへ縮小コピーする。
	// 縮小は、2x2の４ドット→１ドット変換で行われる。
	// そんため、テクスチャのアルファは 0%, 25%, 50%, 75%, 100% の５種類となる。
	// 　bSkipBlack ... true なら黒色は描画せずスキップする。
	void	CopyFromBMP32x4( int x, int y, BITMAP* bmp, bool bSkipBlack=true );

	// テクスチャの名前、ファイル名、サイズ、配置場所を内部に記憶する。
	// その他のパラメータはデフォルト値に初期化する。
	void	InitParameters( LPCTSTR name, LPCTSTR fname, LPCTSTR lpResourceName, LPCTSTR lpResourceType, BYTE* pData, DWORD dwDataSize, DWORD width, DWORD height, bool bSkipBlack );

	// ファイルからのテクスチャの構築。
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromFile();

	// メモリイメージからのテクスチャの読み込みと構築
	// ※ PNG のみ対応
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromFileInMemory();

	// リソースからのテクスチャの読み込みと構築
	// ※ PNG のみ対応
	// ※ width, height で指定したサイズに画像が拡大／縮小されて読み込まれる。
	HRESULT	RestoreDeviceObjectsFromResource();

	// DDS からテクスチャを構築する。
	HRESULT RestoreFromDDS( DDSURFACEDESC2* pddsd, BYTE** ppimage );

	DWORD	BitCount( DWORD dwNum );		// dwNum のビット'1'の数を数える。
	DWORD	ShiftCount( DWORD dwNum );		// dwNum のLSBから0の続く個数を返す。（例：0x0020 → 5）

	void LostCheck();		// サーフェイスがロストしている場合は RestoreDeviceObjest() を呼び出す。
};

	}//Graphics
}//FDK

using namespace FDK::Graphics;
