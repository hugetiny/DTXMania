#pragma once

namespace FDK {
	namespace General {

class WindowPositions
{
public:
	static void	Save();		// 全ウィンドウの位置とサイズを保存
	static void	Load();		// 全ウィンドウの位置とサイズを復元
	static void	Clear();	// 内部ウィンドウ情報リストのクリア

	static BOOL	EnumWindowsSaveCallbackMethod( HWND hWnd );
	static BOOL	EnumWindowsLoadCallbackMethod( HWND hWnd );
	
public:
	WindowPositions() 			{ m_pWinPos = m_pWinPosLast = NULL; }
	virtual ~WindowPositions()	{ Clear(); }

protected:
	static struct WinPos
	{
		HWND			hWnd;
		WINDOWPLACEMENT	wp;
		RECT			rcMax;			// 最大化されているウィンドウのみ有効
		WinPos			*prev, *next;
	} *m_pWinPos, *m_pWinPosLast;
};

	}//General
}//FDK

using namespace FDK::General;
