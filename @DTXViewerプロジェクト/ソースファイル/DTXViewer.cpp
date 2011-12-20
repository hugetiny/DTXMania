#include "stdafx.h"
// FDK includes
#include "CTimer.h"
#include "CDirectInput.h"
#include "DTX.h"
#include "Debug.h"
#include "CommandLineParser.h"
// DTXViewer includes
#include "DTXViewer.h"
#include "CD3dMyApplication.h"

namespace DTXViewer {
	namespace General {

CTimerEx			g_Timer;
CDirectInput		g_DirectInput;
DTX					g_DTX;
CD3DMyApplication	g_App;
CStagePlay			g_StagePlay;
tstring				g_strModulePath;
bool				g_bAction;						// true なら、次のフェーズで Action() が実行される。
TCHAR				g_strCmdLine[ COMLINESIZE ];	// コマンドライン引数（受信文字列）
TCHAR				g_strFileName[ _MAX_PATH ];		// 受信文字列で指定されたファイル名
bool				g_bFullcolor;					// 32bpp なら true

	}//General
}//DTXViewer

static void s_tEXEのあるパスを取得しそこへ移動する()
{
	TCHAR buf[ _MAX_PATH ];
	TCHAR strDrive[ _MAX_DRIVE ];
	TCHAR strDir[ _MAX_DIR ];

	::GetModuleFileName( NULL, buf, _MAX_PATH );
	_tsplitpath_s( buf, strDrive, _MAX_DRIVE, strDir, _MAX_DIR, NULL, 0, NULL, 0 );

	g_strModulePath  = strDrive;
	g_strModulePath += strDir;		// strDir の末尾には / が付いている
	::SetCurrentDirectory( g_strModulePath.c_str() );

	//	Debug::Out( _T("Init(); モジュールパス = \"%s\"\n"), g_strModulePath.c_str() );
	//	--> DEBUG 時は c:\ に出力されてしまう。(g_Config 前だから。) RELEASE 時は大丈夫→ …くないようだ。
}
//
INT WINAPI WinMain( HINSTANCE hInst, HINSTANCE hPrevInst, LPSTR lpCmdLine, int nCmdShow )
{
	_CrtSetDbgFlag ( _CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );	// プログラム終了時に自動的に _CrtDumpMemoryLeaks() を呼び出すようにする設定。 	
	_CrtSetBreakAlloc( 0 );												// 以下の関数でメモリ割当回数を指定すれば、そこにブレークポイントが設定される。

	g_bFullcolor = false;

	s_tEXEのあるパスを取得しそこへ移動する();

	Debug::Init( _T("trace.txt"), _T("DTXViewer.txt") );	// デバッグ関数初期化（カレントフォルダを移動してから呼ぶこと）
	Debug::CutOut( true );									// trace.txt の出力カット
	Debug::CutOutConsole( false );							// デバッグ出力の出力カット
	Debug::CutMsg( false );									// DTXViewer.txt の出力カット

	// 受信メッセージ：なし
	g_bAction = false;

	// コマンドラインを g_strCmdLine[] へコピー
	lstrcpyn( g_strCmdLine, lpCmdLine, COMLINESIZE );
	Debug::Out(_T("起動コマンドライン：\"%s\"\n"), g_strCmdLine );

	// 既に DTXV が起動しているか探し、起動していたら COPYDATASTRUCT で
	// そいつにコマンドライン文字列を丸々投げて終了する。
	HWND hDTXV;
	if( ( hDTXV = FindWindow( "DTXVIEWERCLASS", NULL ) ) )		// クラス名で検索
	{
		// 文字列を送信
	    COPYDATASTRUCT cds;
	    cds.dwData = 0;
		cds.lpData = (void*) g_strCmdLine;
		cds.cbData = lstrlen( g_strCmdLine ) + 1; //  終端の '\0' も送る
		SendMessage( hDTXV, WM_COPYDATA, (WPARAM)NULL, (LPARAM)&cds );
		Debug::Out(_T("DTXV が存在しているので、コマンドライン「%s」を送信しました。\n"), g_strCmdLine );
		return 0;		// 終了
	}

	// いなかったので自分で処理する。
	g_bAction = true;

	// 先にコマンドラインを解析し、'-F' オプションがあるかどうかを調べる。
	CommandLineParser clp;
	clp.Init( g_strCmdLine );
	int num = clp.GetItemNum();
	for( int i = 0; i < num; i++ ) {
		if( strncmp( _T("-F"), clp.Get( i ), _MAX_PATH ) == 0 || strncmp( _T("-f"), clp.Get( i ), _MAX_PATH ) == 0 ) {
			g_bFullcolor = true;
			break;
		}
	}
	clp.Term();

	// ウィンドウ作成ののちメインループへ
	if( FAILED( g_App.Create( hInst, (LPCTSTR)lpCmdLine ) ) )
		return 0;

	return g_App.Run();
}
