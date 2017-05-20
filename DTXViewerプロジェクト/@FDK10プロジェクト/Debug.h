
// Debug - デバッグログ
//
// ログには以下の２つがある。
// 　1.トレースファイル(Out系) ... デバッグコンソール＋Traceファイルに出力。
// 　2.レポートファイル(Msg系) ... Out系＋Reportファイルに出力。

#pragma once

#include <tchar.h>
#include <string>
#include <stdlib.h>

#ifndef tstring
using namespace std;
typedef basic_string<TCHAR> tstring;
#endif

namespace FDK {
	namespace General {

#ifndef bDEBUG
 #ifdef _DEBUG
  #define bDEBUG	TRUE
 #else
  #define bDEBUG	FALSE
 #endif
#endif

#ifndef FNAME
# ifdef _UNICODE
# define FNAME	WFunction(__FUNCTION__)
# else
# define FNAME	_T(__FUNCTION__)
# endif
#endif

class Debug
{

public:
	// 初期化。
	// 　strTraceLog ... トレースログファイル名
	// 　strReportLog .. レポートログファイル名
	static void Init( LPCTSTR strTraceLog, LPCTSTR strReportLog );

	static void IndexUp();		// 行頭空白を１つ増やす
	static void IndexDown();	// 行頭空白を１つ減らす

	// トレースログの出力（日付、インデックス付）
	// 　format ... 出力文字列フォーマット
	static void Out( LPCTSTR format, ...);

	// 関数名付きトレースログの出力（日付、インデックス付）
	// 　funcname ... 関数名
	// 　format ..... 出力文字列フォーマット
	static void OutFN( LPCTSTR funcname, LPCTSTR format, ...);

	// ヘッダ（日付、インデックス）なしトレースログの出力
	// 　format ... 出力文字列フォーマット
	static void OutRaw( LPCSTR format, ...);

	// レポートファイルの出力
	// 　format ... 出力文字列フォーマット
	static void Msg( LPCTSTR format, ...);

	// 関数名つきレポートファイルの出力
	// 　funcname ... 関数名
	// 　format ..... 出力文字列フォーマット
	static void MsgFN( LPCTSTR funcname, LPCTSTR format, ...);

	// メッセージダイアログ(OK|CANCEL)表示
	// 　hWnd ....... ウィンドウハンドル
	// 　title ...... ウィンドウのタイトル
	// 　format ..... 出力文字列フォーマット
	static void MsgDialog( HWND hWnd, LPCTSTR title, LPCTSTR format, ... );

	static void CutOut( bool b = true )			{ m_b出力停止Out=b;}		// Out() 系のファイル出力を停止ならtrue
	static void CutOutConsole( bool b = true )	{ m_b出力停止OutConsole=b;}	// Out() 系のコンソール出力を停止ならtrue
	static void CutMsg( bool b = true )			{ m_b出力停止Msg=b;}		// Msg() 系を出力停止ならtrue

	static void METHOD_START( LPCTSTR methodname, LPCSTR format, ...);		// メソッド開始
	static void METHOD_END( LPCTSTR methodname );							// メソッド終了
	static void FUNCTION_START( LPCTSTR funcname, LPCSTR format, ...);		// 関数開始
	static void FUNCTION_END( LPCTSTR funcname );							// 関数終了
	static void WORK_START( LPCTSTR format, ...);							// ワーク開始
	static void WORK_END();													// ワーク終了

public:
	Debug() {}	// コンストラクタ

protected:
	// 文字列を指定対象へ出力する。ヘッダあり。
	static void	OutWithHeadIndex( LPCTSTR str出力する文字列 );

	// 文字列を指定対象へ出力する。ヘッダなし。
	static void	OutWithoutHeadIndex( LPCTSTR str出力する文字列 );

private:
	static const int MSGLEN = 2048;
	static bool m_b初期化済;
	static int	m_n行頭空白;
	static bool	m_b出力停止Out;
	static bool	m_b出力停止OutConsole;
	static bool	m_b出力停止Msg;
	static bool	m_b初めてのTraceLog出力;
	static bool	m_b初めてのReportLog出力;
	static TCHAR m_strTraceLog[ _MAX_PATH ];
	static TCHAR m_strReportLog[ _MAX_PATH ];
};

	}//General
}//FDK

using namespace FDK::General;
