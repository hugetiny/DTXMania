#include "stdafx.h"
#include <windows.h>
#include <stdio.h>
#include <stdarg.h>
#include <time.h>
#include <crtdbg.h>
#include <locale.h>
#include <direct.h>
#include "Debug.h"

namespace FDK {
	namespace General {

bool	Debug::m_b初期化済 = false;
int		Debug::m_n行頭空白 = 0;
bool	Debug::m_b出力停止Out = false;
bool	Debug::m_b出力停止OutConsole = false;
bool	Debug::m_b出力停止Msg = false;
bool	Debug::m_b初めてのTraceLog出力 = true;
bool	Debug::m_b初めてのReportLog出力 = true;
TCHAR	Debug::m_strTraceLog[ _MAX_PATH ];
TCHAR	Debug::m_strReportLog[ _MAX_PATH ];

void Debug::Init( LPCTSTR strTraceLog, LPCTSTR strReportLog )
{
	// ファイル名の作成
	tstring trace = _T("log\\");
	trace += strTraceLog;
	lstrcpyn( m_strTraceLog, trace.c_str(), _MAX_PATH );

	tstring report = _T("log\\");
	report += strReportLog;
	lstrcpyn( m_strReportLog, report.c_str(), _MAX_PATH );

	// log フォルダが未作成なら作る。
	_tmkdir( _T("log") );

	m_b初期化済 = true;
}

void Debug::Out( LPCTSTR format, ...)
{
	if( ! m_b初期化済 )
		return;

	if( ! format )
	{
		OutputDebugString( "No output format string !!\n" );
		return;
	}

	TCHAR strMsg[ MSGLEN ];

	// メッセージを解析
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 出力
	OutWithHeadIndex( strMsg );
}

void Debug::OutFN( LPCTSTR funcname, LPCTSTR format, ...)
{
	if( ! m_b初期化済 )
		return;

	if( ! format )
	{
		OutputDebugString( "No output format string !!\n" );
		return;
	}

	if( ! funcname )
		funcname = _T("(no function name)");

	// メッセージを解析
	TCHAR strMsg[ MSGLEN ];
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 関数名を加える
	tstring buf = funcname;
	buf += _T("; ");
	buf += strMsg;

	// 出力
	OutWithHeadIndex( buf.c_str() );
}

void Debug::OutRaw( LPCTSTR format, ...)
{
	if( ! m_b初期化済 )
		return;

	if( ! format )
	{
		OutputDebugString( "No output format string !!\n" );
		return;
	}

	TCHAR strMsg[ MSGLEN ];

	// メッセージを解析
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 出力
	OutWithoutHeadIndex( strMsg );
}

void Debug::Msg( LPCTSTR format, ...)
{
	if( ! m_b初期化済 || m_b出力停止Msg )
		return;

	if( ! format )
	{
		OutputDebugString( "No output format string !!\n" );
		return;
	}

	TCHAR strMsg[ MSGLEN ];

	// メッセージを解析
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// ファイルへ出力
	FILE *fp;
	if( _tfopen_s( &fp, m_strReportLog, m_b初めてのReportLog出力 ?_T("wt"):_T("at") ) == 0 )
	{
		_ftprintf_s( fp, _T("%s"), strMsg );
		fclose( fp );
	}
	m_b初めてのReportLog出力 = false;

	// Out系へ出力
	Out( _T("%s"), strMsg );
}

void Debug::MsgFN( LPCTSTR funcname, LPCTSTR format, ...)
{
	if( ! m_b初期化済 || m_b出力停止Msg )
		return;

	if( ! format )
	{
		OutputDebugString( "No output format string !!\n" );
		return;
	}

	TCHAR strMsg[ MSGLEN ];

	// メッセージを解析
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 関数名を加える
	tstring buf = funcname;
	buf += _T("; ");
	buf += strMsg;

	// ファイルへ出力
	FILE *fp;
	if( _tfopen_s( &fp, m_strReportLog, m_b初めてのReportLog出力 ?_T("wt"):_T("at") ) == 0 )
	{
		_ftprintf_s( fp, _T("%s"), buf.c_str() );
		fclose( fp );
	}
	m_b初めてのReportLog出力 = false;

	// Out系へ出力
	Out( _T("%s"), buf.c_str() );
}

void Debug::MsgDialog( HWND hWnd, LPCTSTR title, LPCTSTR format, ... )
{
	if( ! m_b初期化済 )
		return;

	if( !hWnd || !title || !format )
	{
		OutputDebugString( "MsgDialog parameter error !!\n" );
		return;
	}

	// メッセージを解析
	TCHAR strMsg[ MSGLEN ];
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// ファイルへ出力
	FILE *fp;
	if( _tfopen_s( &fp, m_strReportLog, m_b初めてのReportLog出力 ?_T("wt"):_T("at") ) == 0 )
	{
		_ftprintf_s( fp, _T("%s"), strMsg );
		fclose( fp );
	}
	m_b初めてのReportLog出力 = false;

	// ダイアログ出力
	MessageBox( hWnd, strMsg, title, MB_OK );

	// Out系へ出力
	Out( _T("%s: %s"), title, strMsg );
}

void Debug::OutWithHeadIndex( LPCTSTR str出力する文字列 )
{
	if( ! m_b初期化済 || ! str出力する文字列 )
		return;

	tstring buf;

	// インデックス付き文字列の作成
	time_t 出力時刻;
	TCHAR str時刻[32];

	time( &出力時刻 );
	_tctime_s( str時刻, 32, &出力時刻 );

	buf = str時刻;
	buf.erase( 24, 1 );	// 改行コードを削除
	buf += _T("| ");
	for( int i = 0; i < m_n行頭空白; i++ )
		buf += _T("+--");
	if( m_n行頭空白 > 0 )
		buf += _T(" ");
	buf += str出力する文字列;

	// 出力
	OutWithoutHeadIndex( buf.c_str() );
}

void Debug::OutWithoutHeadIndex( LPCTSTR str出力する文字列 )
{
	if( ! m_b初期化済 || ! str出力する文字列 )
		return;

	// デバックコンソールへの出力
	if( ! m_b出力停止OutConsole )
		OutputDebugString( str出力する文字列 );

	// トレースファイルへの出力
	if( bDEBUG && ! m_b出力停止Out )
	{
		FILE *fp;
		if( _tfopen_s( &fp, m_strTraceLog, m_b初めてのTraceLog出力 ?_T("wt"):_T("at") ) == 0 )
		{
			_ftprintf_s( fp, _T("%s"), str出力する文字列 );
			fclose( fp );
		}
		m_b初めてのTraceLog出力 = false;
	}
}

void Debug::IndexUp()
{
	m_n行頭空白++;
}

void Debug::IndexDown()
{
	if( m_n行頭空白 > 0 ) m_n行頭空白--;
}

void Debug::METHOD_START( LPCTSTR methodname, LPCSTR format, ...)
{
	// メッセージを解析
	TCHAR strMsg[ MSGLEN ];
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 出力＆インデックスUP
	OutFN( methodname, _T("%s"), strMsg );
	IndexUp();
}

void Debug::METHOD_END( LPCTSTR methodname )
{
	IndexDown();
	tstring msg = _T("完了(");
	msg += methodname;
	msg += _T(")\n");
	Out( msg.c_str() );
}

void Debug::FUNCTION_START( LPCTSTR funcname, LPCSTR format, ...)
{
	// メッセージを解析
	TCHAR strMsg[ MSGLEN ];
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 出力＆インデックスUP
	OutFN( funcname, _T("%s"), strMsg );
	IndexUp();
}

void Debug::FUNCTION_END( LPCTSTR funcname )
{
	IndexDown();
	tstring msg = _T("完了(");
	msg += funcname;
	msg += _T(")\n");
	Out( msg.c_str() );
}

void Debug::WORK_START( LPCTSTR format, ...)
{
	// メッセージを解析
	TCHAR strMsg[ MSGLEN ];
	va_list	args;
	va_start( args, format );
	_vstprintf_s( strMsg, MSGLEN, format, args );
	va_end( args );

	// 出力＆インデックスUP
	Out( _T("%s"), strMsg );
	IndexUp();
}

void Debug::WORK_END()
{
	IndexDown();
	Out( _T("完了\n") );
}

	}//General
}//FDK
