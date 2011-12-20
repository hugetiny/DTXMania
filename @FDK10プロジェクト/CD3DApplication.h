#pragma once

#include "CD3DDisplay.h"
#include "CTimer.h"

namespace FDK {
	namespace AppBase {

class CD3DApplication
{
public:
	CD3DApplication();

	// 起動
	virtual HRESULT Create( HINSTANCE hInst, LPCTSTR strCmdLine );		// 初期化
	virtual LRESULT WndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam );	// ウィンドウメッセージハンドラ。
	INT		Run();														// スレッドループ

	// Create() を呼び出す前に以下のメンバ変数を設定可能。
	static HWND hWnd;								// ウィンドウハンドル（NULLならデフォルトウィンドウが作成される）
	static int	nBpp;								// ビット深度（16 or 32）
	bool		b全画面モード;						// 全画面モードなら true
	int			nWidth, nHeight;					// ウィンドウサイズ、深度
	bool		b垂直帰線同期;						// フリップを垂直帰線同期させるならtrue
	DWORD		dw描画間隔;							// Present 時のスリープ数[ms]（デフォルト 3 ms；任意のタイミングで変更可能）
    LPCTSTR		strWindowTitle;						// アプリウィンドウのタイトル名
    LPCTSTR		strWindowClass;						// アプリウィンドウのクラス名

	// その他
	static LPDIRECTDRAW7			pDD;				// DirectDraw7
	static LPDIRECTDRAWSURFACE7		pddsFrontBuffer;	// フロントバッファ
	static LPDIRECTDRAWSURFACE7		pddsBackBuffer;		// バックバッファ
	static LPDIRECT3D7				pD3D;				// Direct3D7
	static LPDIRECT3DDEVICE7		pD3DDevice;			// Direct3DDevice7
	static CD3DDisplay				D3DDisplay;			// アダプタ,デバイス,モード管理
	HINSTANCE						hInstance;			// インスタンス
	bool							bReady;				// 描画準備が整ったら true
	bool							bActive;			// ウィンドウがアクティブなら true、最小化or全部隠れている場合は false
	CTimer							timer;				// Flip|Blt用タイマ
	int								nFPS;				// Frame / sec,

	static void	GetProjectionParam( float* d, float* t, float* a ) { *d=100.0f; *t=24.0f/180.0f*(float)PI; *a=1.0f; }

protected:
	HRESULT t3D環境の構築();					// ３Ｄ環境の構築（Create(), ForceWindowed()から呼び出される）
	HRESULT t3D環境の描画();					// ３Ｄの描画処理
	void	t3D環境の破棄();					// ３Ｄ環境の破棄
	HRESULT t3D環境の再構築( bool b新全画面モード );	// スレッド駆動の場合は、ここに来るまでにクリティカルセクションでスレッド動作を止めておくこと。

	virtual HRESULT OneTimeSceneInit()				{ return S_OK; }	// 最初１回だけの初期化（ウィンドウ、DD/D3D初期化後）
	virtual HRESULT InitDeviceObjects()				{ return S_OK; }	// デバイス依存オブジェクトの内部状態初期化
	virtual HRESULT RestoreDeviceObjects()			{ return S_OK; }	// 　　　　　　〃　　　　　　サーフェイス構築
	virtual HRESULT InvalidateDeviceObjects()		{ return S_OK; }	// 　　　　　　〃　　　　　　サーフェイス破棄（内部状態維持）
	virtual HRESULT DeleteDeviceObjects()			{ return S_OK; }	// 　　　　　　〃　　　　　　内部状態破棄
	virtual HRESULT FinalCleanup()					{ return S_OK; }	// 最後１回だけの終了処理（WM_CLOSE内で呼ばれる）
	virtual bool	Render()						{ return false; }	// シーンの進行＆描画（t3D環境の描画()から呼び出される; trueを返すとアプリ終了）
	virtual	HRESULT	tFlipまたはBlt();

	virtual void tWaitAMomentの描画();		// デバイス切替時の Wait a moment 画像の描画（必要あれば実装する）

	void	t全画面・ウィンドウモードを切り替える();							// ウィンドウスレッドでなければ切りかえられないので、ゲームスレッドからは呼ばないこと。
	void	t現在のウィンドウのウィンドウ領域とクライアント領域を取得する();	// どちらもスクリーン座標で取得する。
	HRESULT	t垂直同期を使わないウェイトとBlt( LPRECT prcクライアント領域 );
	void	tFPSを算出する();

	DWORD	dwウィンドウスタイル;		// モード切替用にウィンドウスタイルを保存
	DWORD	dw全画面スタイル;			// モード切替用に全画面スタイルを保存
	RECT	rcウィンドウ領域;			// モード切替用にウィンドウ領域を保存
	RECT	rcクライアント領域;			// モード切替用にクライアント領域を保存

	bool	bScreenSaverEnable;			// 起動時、スクリーンセーバが有効だったならtrue
	EXECUTION_STATE	exeState;			// モニタ電源設定保存
};

	}//AppBase
}//FDK

using namespace FDK::AppBase;
