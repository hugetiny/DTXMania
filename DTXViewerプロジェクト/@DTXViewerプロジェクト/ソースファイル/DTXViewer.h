#pragma once

#include "CTimer.h"
#include "CDirectInput.h"
#include "DTX.h"
#include "CStagePlay.h"
#include "CD3DMyApplication.h"

#define	COMLINESIZE		2048

namespace DTXViewer {
	namespace General {

extern CTimerEx				g_Timer;
extern CDirectInput			g_DirectInput;
extern DTX					g_DTX;
extern CD3DMyApplication	g_App;
extern CStagePlay			g_StagePlay;
extern tstring				g_strModulePath;
extern bool					g_bAction;						// true なら、次のフェーズで Action() が実行される。
extern TCHAR				g_strCmdLine[ COMLINESIZE ];	// コマンドライン引数（受信文字列）
extern TCHAR				g_strFileName[ _MAX_PATH ];		// 受信文字列で指定されたファイル名
extern bool					g_bFullcolor;					// 32bpp なら true

	}//General
}//DTXViewer
extern INT WINAPI WinMain( HINSTANCE hInst, HINSTANCE hPrevInst, LPSTR lpCmdLine, int nCmdShow );
