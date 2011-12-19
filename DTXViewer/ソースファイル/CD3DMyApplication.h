#pragma once

#include "Activity.h"
#include "CD3DApplication.h"

namespace DTXViewer {
	namespace General {

class CD3DMyApplication : public FDK::AppBase::CD3DApplication
{
public:
	void	tバックサーフェイスをクリアする();

	virtual HRESULT	Create( HINSTANCE hInst, LPCTSTR strCmdLine );
	virtual HRESULT OneTimeSceneInit();					// 最初１回だけの初期化（ウィンドウ、DD/D3D初期化後）
	virtual HRESULT InitDeviceObjects();				// デバイス依存オブジェクトの内部状態初期化
	virtual HRESULT RestoreDeviceObjects();				// 　　　　　　〃　　　　　　サーフェイス構築
	virtual HRESULT InvalidateDeviceObjects();			// 　　　　　　〃　　　　　　サーフェイス破棄（内部状態維持）
	virtual HRESULT DeleteDeviceObjects();				// 　　　　　　〃　　　　　　内部状態破棄
	virtual HRESULT FinalCleanup();						// 最後１回だけの終了処理（WM_CLOSE内で呼ばれる）
	virtual bool	Render();							// シーンの進行＆描画（t3D環境の描画()から呼び出される; trueを返すとアプリ終了）
	virtual LRESULT WndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam );	// ウィンドウメッセージプロシージャ
	CD3DMyApplication();

protected:
	Activity		actTopLevel;						// トップレベル Activity
	bool			b演奏開始時にウィンドウを前面に持ってくる;

	void			Action();							// メッセージの処理
	void			tウィンドウを前面にもってくる();
};

	}//General
}//DTXViewer

using namespace DTXViewer::General;
