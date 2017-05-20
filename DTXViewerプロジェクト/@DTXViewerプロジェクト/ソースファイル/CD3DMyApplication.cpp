#include "stdafx.h"
// FDK includes
#include "Activity.h"
#include "Debug.h"
#include "FDKError.h"
#include "CTimer.h"
#include "CSoundManager.h"
#include "CDirectInput.h"
#include "CTexture.h"
// DTXViewer includes
#include "../resource.h"
#include "DTXViewer.h"
#include "Version.h"
#include "DirectXVersion.h"
#include "CD3DMyApplication.h"
#include "CStagePlay.h"

namespace DTXViewer {
	namespace General {

static CD3DMyApplication* s_pApp = NULL;
LRESULT CALLBACK MyWndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam )
{
	if( s_pApp ) return s_pApp->WndProc( hWnd, uMsg, wParam, lParam );
	return 0;
}
//
CD3DMyApplication::CD3DMyApplication() : CD3DApplication()
{
	s_pApp = this;
}
HRESULT	CD3DMyApplication::Create( HINSTANCE hInst, LPCTSTR strCmdLine )
{
	this->hInstance		= hInst;

	// 初期情報の登録
	{
		this->b全画面モード			= false;
		this->nWidth				= 640;
		this->nHeight				= 480;
		this->nBpp					= ( g_bFullcolor ) ? 32 : 16;
		this->b垂直帰線同期			= false;
		this->dw描画間隔			= 2;
		this->strWindowTitle		= _T("DTXViewer" VERSION );
		this->strWindowClass		= _T("DTXVIEWERCLASS" );
		this->dwウィンドウスタイル	= WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_VISIBLE | WS_MINIMIZEBOX;
	}
	//
	// ウィンドウの新規生成
	{
		WNDCLASS wndClass = {
			0, MyWndProc, 0, 0, hInst,
			LoadIcon( this->hInstance, MAKEINTRESOURCE( IDI_DTXVIEWER ) ),
			LoadCursor( NULL, IDC_ARROW ),
			(HBRUSH)GetStockObject( WHITE_BRUSH ),
			NULL, this->strWindowClass
		};
		RegisterClass( &wndClass );

		RECT rc = { 0, 0, this->nWidth, this->nHeight };
		::AdjustWindowRect( &rc, this->dwウィンドウスタイル, FALSE );

		this->hWnd = CreateWindow(
			this->strWindowClass, this->strWindowTitle,
			(this->b全画面モード) ? this->dw全画面スタイル : this->dwウィンドウスタイル, 
			CW_USEDEFAULT, CW_USEDEFAULT,
			(rc.right-rc.left), (rc.bottom-rc.top), 0L,
			NULL, hInst, 0L );	

		if( this->hWnd == NULL ) {
			Debug::Msg( _T("Error: メインウィンドウの作成に失敗しました。(%08x)\n"), GetLastError() );
			return E_FAIL;
		}
	}
	//
	// DirectX ランタイムのバージョンチェック
	{
		DirectXVersion	dxver;

		dxver.tDirectXのバージョン情報を取得する();
		if( dxver.dwMajor < 7 ) {
			Debug::MsgDialog( this->hWnd, _T("DTXViewer runtime error"), _T("Error: DTXViewer の実行には、DirectX 7.0a 以上のランタイムライブラリが必要です。\n") );
			return E_FAIL;
		}
	}
	//
	// 親クラスの呼び出し
	{
		HRESULT hr;
		if( FAILED( hr = CD3DApplication::Create( hInst, strCmdLine ) ) )
			return hr;
	}
	//
	// 最初のステージの設定;
	// 上記Create()でトップレベルActivity の RestoreDeviceObjectsまで完了するので、
	// ここでようやくトップレベルActivity（最初のステージ）を Activate できる。
	{
		g_StagePlay.Activate();
		g_Timer.Flush();
	}
	return S_OK;
}
HRESULT CD3DMyApplication::OneTimeSceneInit()
{
	HRESULT hr;

	this->b演奏開始時にウィンドウを前面に持ってくる = true;

	// 乱数の初期化
	{
		srand( (unsigned) time( NULL ) );
	}
	//
	// タイマの初期化;
	// CD3DApplication の生成時に CD3CMyApplication::timer が Init() されてしまっているので、
	// タイマ種別を指定するために、一度 Term() して Init() しなおす。
	{
		g_Timer.Term();
		g_Timer.Init( TIMERTYPE_TIMEGTTIME );
	}
	//
	// DirectInput の初期化
	{
		if( FAILED( hr = g_DirectInput.Init( this->hWnd ) ) )
		{
			Debug::Msg( _T("Error: DirectInput の初期化に失敗しました。(%s)\n"), HRMSG(hr) );
			return hr;
		}
	}
	//
	// トップレベルActivity の登録と初期化
	{
		this->actTopLevel.AddChild( &g_StagePlay );
		this->actTopLevel.OneTimeSceneInit();
	}
	//
	return S_OK;
}
HRESULT CD3DMyApplication::FinalCleanup()
{
	// トップレベルActivity の終了
	{
		this->actTopLevel.FinalCleanup();
	}
	//
	// DirectInput の終了
	{
		g_DirectInput.Term();
	}
	//
	// タイマの終了
	{
		g_Timer.Term();
	}
	//
	return S_OK;
}
HRESULT CD3DMyApplication::InitDeviceObjects()
{
	// テクスチャフォーマットの事前調査＆事前取得
	CTexture::EnumTextureFormats();

	// トップレベル Activity のデバイス依存オブジェクトの初期化
	this->actTopLevel.InitDeviceObjects();

	return S_OK;
}
HRESULT CD3DMyApplication::DeleteDeviceObjects()
{
	// トップレベル Activity のデバイス依存オブジェクトの破棄
	this->actTopLevel.DeleteDeviceObjects();

	return S_OK;
}
HRESULT CD3DMyApplication::RestoreDeviceObjects()
{
	// トップレベル Activity のデバイス依存オブジェクトの構築
	this->actTopLevel.RestoreDeviceTextures();
	this->actTopLevel.RestoreDeviceSurfaces();
	
	return S_OK;
}
HRESULT CD3DMyApplication::InvalidateDeviceObjects()
{
	// トップレベル Activity のデバイス依存オブジェクトの解放
	this->actTopLevel.InvalidateDeviceObjects();

	return S_OK;
}

//
LRESULT CD3DMyApplication::WndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam )
{
	// 必要あればここにメッセージハンドリングコードを追加する。
	switch( uMsg )
	{
	case WM_COPYDATA:
		{
			if( g_bAction )		// 既にアクション待ちなら先にそれを処理する
				Action();

			COPYDATASTRUCT* pcds = (COPYDATASTRUCT*) lParam;		// 受信文字列を g_strCmdLine にコピー
			lstrcpyn( g_strCmdLine, (LPCTSTR)pcds->lpData, COMLINESIZE );
			//SetWindowPos( m_hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE );	// ウィンドウを最前面へ
			//SetWindowPos( m_hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE );
			g_bAction = true;		// アクションを実行する
			break;
		}

	case WM_SYSKEYDOWN:
		if( wParam == VK_F4 )	// Al4 + F4
			SendMessage( this->hWnd, WM_CLOSE, 0, 0 );
		return 0;

	case WM_CLOSE:
		break;	// break → 続けて親クラスの処理へ
	}

	return CD3DApplication::WndProc( hWnd, uMsg, wParam, lParam );
}
//	
void	CD3DMyApplication::tバックサーフェイスをクリアする()
{
	if( this->pddsBackBuffer == NULL )
		return;

	DDBLTFX ddbltfx;
	::ZeroMemory( &ddbltfx, sizeof( ddbltfx ) );
	ddbltfx.dwSize = sizeof( ddbltfx );
	ddbltfx.dwFillColor = 0;
	this->pddsBackBuffer->Blt( NULL, NULL, NULL, DDBLT_COLORFILL, &ddbltfx );
}
bool	CD3DMyApplication::Render()
{
	// 戻り値: true:アプリ終了, false:継続

	// 状態更新
	g_Timer.Flush();
	g_DirectInput.Poll();

	// シーン描画
	CTexture::BeginScene();
	this->tバックサーフェイスをクリアする();
	if( g_StagePlay.MainWork() == -1 )
	{
		g_StagePlay.UnActivate();
		CTexture::EndScene();
		return true;		// アプリ終了
	}
	CTexture::EndScene();

	// アクション処理
	if( g_bAction )
		this->Action();

	return false;
}
//
void CD3DMyApplication::Action()
{
	Debug::Out( _T("受信文字列 = [%s]\n"), g_strCmdLine );

	// 内部状態の初期化
	g_DTX.SetWAVCache( true );
	g_DTX.SetBMPCache( true );

	// コマンドライン引数リスト
	struct CmdList {
		TCHAR	str[ _MAX_PATH ];
		CmdList	*prev, *next;
	} *cmdList, *cmdListLast, *cell;
	cmdList = cmdListLast = cell = NULL;

	Debug::Out( _T("受信文字列 = [%s]\n"), g_strCmdLine );

	// g_strCmdList をコマンドライン引数リスト（cmdList）に分解して格納する。
	TCHAR* p = g_strCmdLine;
	while( *p != _T('\0') )
	{
		while( *p==_T(' ') || *p==_T('\t') )
			p++;

		// (1) オプションの場合
		if( *p == _T('-') )
		{
			cell = new CmdList();
			int i = 0;
			while( *p != _T(' ') && *p != _T('\t') && *p != _T('\0') )
				cell->str[i++] = *p++;
			cell->str[i] = _T('\0');
		}

		// (2) ファイル名の場合
		else
		{
			cell = new CmdList();
			int i = 0;
			while( *p != _T('\0') )
			{
				if( *p == _T('-') && i > 0 && (*(p-1) == _T(' ') || *(p-1) == _T('\t') ) ) 	// オプションが来たら終端
					break;
				cell->str[i++] = *p++;
			}
			while( i > 0 && ( cell->str[i-1] == _T(' ') || cell->str[i-1] == _T('\t') ) )
				i--;	// 末尾の空白は全削除
			cell->str[i] = _T('\0');

			// ダブルクォテーション("...")で囲まれている場合ははずす
			if( cell->str[0] == _T('\"') )
			{
				int i = 1;
				while( cell->str[i] != _T('\"') && cell->str[i] != _T('\0') ) {
					cell->str[i-1] = cell->str[i];
					i++;
				}
				cell->str[i-1] = _T('\0');
			}
		}

		// (1)(2)どちらもリストへ接続
		cell->prev = cell->next = NULL;
		APPENDLIST( cmdList, cmdListLast, cell );

		Debug::Out( _T("受信セル:「%s」\n"), cell->str );
	}

	//
	// 引数リストの中にファイル名があればそれを g_strFileName にコピーする。
	// ファイル名が複数ある場合は、最初の指定のみ有効とする。
	for( cell = cmdList; cell != NULL; cell=cell->next )
	{
		if( cell->str[0] != _T('-') )
		{
			lstrcpyn( g_strFileName, cell->str, _MAX_PATH );
			break;	// 最初のものだけ有効。
		}
	}

	//
	// 引数リスト内のオプションを順番にマークする。
	bool b演奏開始 = false;
	int  n演奏開始位置 = -1;
	for( cell = cmdList; cell != NULL; cell=cell->next )
	{
		switch( cell->str[1] )
		{

		//////////////////
		// -N[num] … num小節からの再生開始

		case _T('N'):
		case _T('n'): {
			
			// num の取得
			TCHAR* p = cell->str+2;
			int n正負 = 1;
			while( *p == _T('-') ) { n正負 *= -1; p++; }	// マイナスも考慮
			n演奏開始位置 = atoi( p ) * n正負;
			b演奏開始 = true;	// 実際の演奏はこの switch を抜けたあとで…（演奏前に全部のオプションを処理するため）
			break;
		}

		//////////////////
		// -S … 再生停止
		case _T('S'):
		case _T('s'):
			g_StagePlay.Stop();
			break;

/*		//////////////////
		// -R … 再生再開		※未実装
		case _T('R'):
		case _T('r'):
			g_StagePlay.Replay();
			break;
*/
		//////////////////
		// -W[0|1] … WAVキャッシュ
		//   -W0 で WAVキャッシュOFF, その他で ON。省略時は -W1。
		case _T('W'):
		case _T('w'):
			g_DTX.SetWAVCache( ( cell->str[2] == _T('0') ) ? false : true );
			break;

		//////////////////
		// -B[0|1] … BMPキャッシュ
		//   -B0 で BMPキャッシュOFF, その他で ON。省略時は -B1。
		case _T('B'):
		case _T('b'):
			g_DTX.SetBMPCache( ( cell->str[2] == _T('0') ) ? false : true );
			break;

		//////////////////
		// -P[0|1]
		//   -P0 で前面にもってこない、-P1 でもってくる。省略時は -P1。
		case _T('P'):
		case _T('p'):
			this->b演奏開始時にウィンドウを前面に持ってくる = ( cell->str[2] == _T('0') ) ? false : true;
			break;

		default:
			break;
		}
	}
	//
	// 演奏開始？
	if( b演奏開始 )
	{
		if( this->b演奏開始時にウィンドウを前面に持ってくる )
			this->tウィンドウを前面にもってくる();

		if( g_StagePlay.Load( g_strFileName ) )
			g_StagePlay.Play( n演奏開始位置 );
	}
	//
	// コマンドライン引数リストの解放
	cell = cmdList;
	while( cell != NULL )
	{
		CmdList *next = cell->next;
		delete cell;
		cell = next;
	}

	// 処理完了
	g_bAction = false;
}
void	CD3DMyApplication::tウィンドウを前面にもってくる()
{
	SetWindowPos( this->hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE );
	SetWindowPos( this->hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE );
}
	}//General
}//DTXViewer
