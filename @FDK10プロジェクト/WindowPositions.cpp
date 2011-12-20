#include "stdafx.h"
#include "WindowPositions.h"

namespace FDK {
	namespace General {

#define	NAMELEN		1024		// クラス名、ウィンドウ名の最大文字数

// static なやつら
WindowPositions::WinPos* WindowPositions::m_pWinPos     = NULL;
WindowPositions::WinPos* WindowPositions::m_pWinPosLast = NULL;

// 局所関数；コールバック
static BOOL CALLBACK EnumWindowsSaveCallbackFunc( HWND hWnd, LPARAM lParam );
static BOOL CALLBACK EnumWindowsLoadCallbackFunc( HWND hWnd, LPARAM lParam );

void WindowPositions::Clear()
{
	WinPos* p = m_pWinPos;
	while( p )
	{
		WinPos* n = p->next;
		delete p;
		p = n;
	}
	m_pWinPos = m_pWinPosLast = NULL;
}

void WindowPositions::Save()
{
	Clear();
	EnumWindows( (WNDENUMPROC) EnumWindowsSaveCallbackFunc, (LPARAM) NULL );
}

BOOL WindowPositions::EnumWindowsSaveCallbackMethod( HWND hWnd )
{
	TCHAR szWindowName[ NAMELEN ];
	TCHAR szClassName [ NAMELEN ];

	// ウインドウ名とクラス名の取得
	GetWindowText( hWnd, szWindowName, NAMELEN );
	GetClassName ( hWnd, szClassName,  NAMELEN );

	// 見えているウインドウだけを対象とする
	if( IsWindowVisible( hWnd )
		 && GetWindow( hWnd, GW_OWNER ) == NULL
		 && lstrlen( szWindowName ) > 0
		 && lstrcmp( szClassName, "Progman" ) != 0 )
	{
		// ウィンドウの位置とサイズを取得
		WinPos* cell = new WinPos();
		cell->wp.length = sizeof( WINDOWPLACEMENT );
		GetWindowPlacement( hWnd, &(cell->wp) );
		cell->hWnd = hWnd;
		cell->prev = cell->next = NULL;
		if( cell->wp.showCmd & SW_SHOWMAXIMIZED )				// 最大化されてるなら
			GetWindowRect( hWnd, &cell->rcMax );				// サイズも取得する
		else
			SetRect( &cell->rcMax, 0, 0, 0, 0 );
		
		// リストに接続
		APPENDLIST( m_pWinPos, m_pWinPosLast, cell );
	}
	return TRUE;
}

void WindowPositions::Load()
{
	EnumWindows( (WNDENUMPROC) EnumWindowsLoadCallbackFunc, (LPARAM) NULL );
}

BOOL WindowPositions::EnumWindowsLoadCallbackMethod( HWND hWnd )
{
	TCHAR szWindowName[ NAMELEN ];

	// ウィンドウ名の取得
	GetWindowText( hWnd, szWindowName, NAMELEN );

	// すべてのウィンドウについて...
	for( WinPos* cell = m_pWinPos; cell != NULL; cell = cell->next )
	{
		if( cell->hWnd == hWnd )
		{
			// 元の場所に戻す
			SetWindowPlacement( hWnd, &(cell->wp) );
			if( cell->wp.showCmd == SW_SHOWMAXIMIZED )		// 最大化されてたならサイズも復元する
			{
				int w = cell->rcMax.right  - cell->rcMax.left;
				int h = cell->rcMax.bottom - cell->rcMax.top;
				MoveWindow( hWnd, cell->rcMax.left, cell->rcMax.top, w, h, TRUE );
			}
			break;
		}
	}
	return TRUE;
}

static BOOL CALLBACK EnumWindowsSaveCallbackFunc( HWND hWnd, LPARAM lParam )
{
	return WindowPositions::EnumWindowsSaveCallbackMethod( hWnd );
}
static BOOL CALLBACK EnumWindowsLoadCallbackFunc( HWND hWnd, LPARAM lParam )
{
	return WindowPositions::EnumWindowsLoadCallbackMethod( hWnd );
}

	}//General
}//FDK