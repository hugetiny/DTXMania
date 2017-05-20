#include "stdafx.h"
#include "Debug.h"
#include "CD3DDisplay.h"
#include "CTimer.h"
#include "WindowPositions.h"
#include "FDKError.h"
#include "CTexture.h"
#include "CD3DApplication.h"

namespace FDK {
	namespace AppBase {

// static なやつら
LPDIRECTDRAW7			CD3DApplication::pDD = NULL;
LPDIRECTDRAWSURFACE7	CD3DApplication::pddsFrontBuffer = NULL;
LPDIRECTDRAWSURFACE7	CD3DApplication::pddsBackBuffer = NULL;
LPDIRECT3D7				CD3DApplication::pD3D = NULL;
LPDIRECT3DDEVICE7		CD3DApplication::pD3DDevice = NULL;
CD3DDisplay				CD3DApplication::D3DDisplay;
HWND					CD3DApplication::hWnd = NULL;
int						CD3DApplication::nBpp = 16;

// 局所変数
static CD3DApplication* s_pD3DApp = NULL;		// WndProc用

// static WndProc
LRESULT CALLBACK WndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam )
{
	return ( s_pD3DApp ) ? s_pD3DApp->WndProc( hWnd, uMsg, wParam, lParam ) : DefWindowProc( hWnd, uMsg, wParam, lParam );
}
//
CD3DApplication::CD3DApplication()
{
	// 外部変数の初期化
	s_pD3DApp					= this;

	// 内部変数の初期化
	this->nWidth				= 640;
	this->nHeight				= 480;
	this->nBpp					= 16;
	this->b全画面モード			= false;
	this->hWnd					= NULL;
	this->b垂直帰線同期			= true;
	this->dw描画間隔			= 3;
	this->bReady				= false;
	this->bActive				= false;
    this->strWindowTitle		= _T("FDK10 Application");
    this->strWindowClass		= _T("FDK10 WindowClass");
	this->dwウィンドウスタイル	= WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_VISIBLE;
	this->dw全画面スタイル		= WS_POPUP | WS_VISIBLE;
	this->bScreenSaverEnable	= false;
}
HRESULT CD3DApplication::Create( HINSTANCE hInst, LPCTSTR strCmdLine )
{
	HRESULT hr;

	this->hInstance   = hInst;

	// IME を無効化
	{
		WINNLSEnableIME( NULL, FALSE );
	}
	//
	// ウィンドウ作成がまだならここで作成する
	{
		if( this->hWnd == NULL )
		{
			WNDCLASS wndClass = {
				0,										// クラススタイル
				FDK::AppBase::WndProc,					// メッセージプロシージャ
				0,										// ClsExtra
				0,										// WndExtra
				this->hInstance,						// インスタンス
				NULL,									// ウィンドウアイコン
				LoadCursor( NULL, IDC_ARROW ),			// マウスアイコン
				(HBRUSH)GetStockObject( WHITE_BRUSH ),	// 背景ブラシ
				NULL,									// メニュー名
				this->strWindowClass					// クラス名
			};
			::RegisterClass( &wndClass );

			RECT rc = { 0, 0, this->nWidth, this->nHeight };
			::AdjustWindowRect( &rc, this->dwウィンドウスタイル, FALSE );
			this->hWnd = ::CreateWindow(
				this->strWindowClass,
				this->strWindowTitle,
				(this->b全画面モード) ? this->dw全画面スタイル : this->dwウィンドウスタイル,
				CW_USEDEFAULT, CW_USEDEFAULT,
				(rc.right-rc.left), (rc.bottom-rc.top), 0L,
				NULL, this->hInstance, 0L );
		}
	}
	//
	// ウィンドウ領域（スクリーン座標）の取得・保存
	{
		this->t現在のウィンドウのウィンドウ領域とクライアント領域を取得する();
	}
	//
	// アダプタ／デバイス／モードの列挙
	{
		if( FAILED( hr = this->D3DDisplay.EnumerateDevices() ) )
			return hr;	// 失敗
	}
	//
	// ３Ｄ環境の構築
	{
		if( FAILED( hr = t3D環境の構築() ) )
			return hr;
	}
	//
	// アプリの一度だけの初期化
	{
		if( FAILED( hr = OneTimeSceneInit() ) ) {
			t3D環境の破棄();
			return hr;
		}
	}
	//
	// デバイス依存オブジェクトの初期化
	{
		if( FAILED( hr = InitDeviceObjects() ) ) {
			t3D環境の破棄();
			return hr;
		}
	}
	//
	// デバイス依存オブジェクトの構築
	{
		if( FAILED( hr = RestoreDeviceObjects() ) ) {
			t3D環境の破棄();
			return hr;
		}
	}	
	//
	// 準備完了
	this->bReady = true;

	return S_OK;
}
HRESULT CD3DApplication::t3D環境の構築()
{
	HRESULT hr;
	
	D3DAdapterInfo* pAdapter = this->D3DDisplay.pAdapter;
	D3DDeviceInfo*  pDevice  = pAdapter->pDevice;

	// モニタの電源設定を一時的に無効にする
	{
		this->exeState = ::SetThreadExecutionState( ES_DISPLAY_REQUIRED | ES_CONTINUOUS );
	}
	//
	// スクリーンセーバの有効・無効を取得し、その後一時的に無効にする
	{
		HKEY	hKeyScreenSaver = NULL;
		long	lReturn = 0;
		long	lScreenSaver = 0;
		DWORD	dwData = 0;

		if( RegOpenKeyEx( HKEY_CURRENT_USER, TEXT("Control Panel\\Desktop"), 0, KEY_QUERY_VALUE, &hKeyScreenSaver ) == ERROR_SUCCESS )
		{
			this->bScreenSaverEnable = 
				( RegQueryValueEx( hKeyScreenSaver, TEXT("SCRNSAVE.EXE"), NULL, NULL, NULL, &dwData ) == ERROR_SUCCESS ) ? true : false;
		}
		RegCloseKey( hKeyScreenSaver );
		hKeyScreenSaver = NULL;

		// 無効にする
		SystemParametersInfo( SPI_SETSCREENSAVEACTIVE, FALSE, 0, SPIF_SENDWININICHANGE );
	}
	//
	// 全ウィンドウ位置の保存
	{
		WindowPositions::Save();
	}
	//
	// タイマの初期化; タイマ種別を変更する場合は、一度 Term() して Init() しなおすこと。
	{
		this->timer.Init( TIMERTYPE_TIMEGTTIME );
	}
	//
	// D3DX の初期化
	{
		if( FAILED( hr = D3DXInitialize() ) )
			return hr;
	}
	//
	// DirectDraw7 の作成
	{
		if( FAILED( hr = DirectDrawCreateEx( NULL, (VOID**)&this->pDD, IID_IDirectDraw7, NULL ) ) ) {
			D3DXUninitialize();
			return hr;
		}
	}
	//
	// 協調モードの設定
	{
		if( FAILED( hr = this->pDD->SetCooperativeLevel( this->hWnd, this->b全画面モード ? (DDSCL_EXCLUSIVE|DDSCL_FULLSCREEN) : (DDSCL_NORMAL|DDSCL_NOWINDOWCHANGES) ) ) ) 
		{
			SAFE_RELEASE( this->pDD );
			D3DXUninitialize();
			return hr;
		}
	}
	//
	// ディスプレイモードの設定（全画面モード時のみ）
	{
		if( this->b全画面モード )
		{
			if( FAILED( hr = this->pDD->SetDisplayMode( this->nWidth, this->nHeight, this->nBpp, 0, 0 ) ) )
			{
				SAFE_RELEASE( this->pDD );
				D3DXUninitialize();
				return hr;
			}
		}
	}
	//
	// プライマリ＆バックサーフェイスの作成
	{
		// a. 全画面モードの場合
		if( this->b全画面モード )
		{
			DDSURFACEDESC2 ddsd;
			ZeroMemory( &ddsd, sizeof( ddsd ) );
			ddsd.dwSize	= sizeof( ddsd );
			ddsd.dwFlags = DDSD_CAPS | DDSD_BACKBUFFERCOUNT;
			ddsd.ddsCaps.dwCaps	= DDSCAPS_PRIMARYSURFACE | DDSCAPS_3DDEVICE | DDSCAPS_FLIP | DDSCAPS_COMPLEX;
			ddsd.dwBackBufferCount = 1;
			if( FAILED( hr = this->pDD->CreateSurface( &ddsd, &this->pddsFrontBuffer, NULL ) ) ) {
				D3DXUninitialize();
				SAFE_RELEASE( this->pDD );
				return hr;	// プライマリサーフェイスの作成に失敗
			}
			DDSCAPS2 ddscaps;
			ZeroMemory( &ddscaps, sizeof( ddscaps ) );
			ddscaps.dwCaps = DDSCAPS_BACKBUFFER;
			if( FAILED( hr = this->pddsFrontBuffer->GetAttachedSurface( &ddscaps, &this->pddsBackBuffer ) ) )
			{
				D3DXUninitialize();
				SAFE_RELEASE( this->pddsFrontBuffer );
				SAFE_RELEASE( this->pDD );
				return hr;	// バックサーフェイスの取得に失敗
			}
		}
		//
		// b. ウィンドウモードの場合
		else
		{
			DDSURFACEDESC2 ddsd;
			ZeroMemory( &ddsd, sizeof( ddsd ) );
			ddsd.dwSize = sizeof( ddsd );
			ddsd.dwFlags = DDSD_CAPS;
			ddsd.ddsCaps.dwCaps = DDSCAPS_PRIMARYSURFACE | DDSCAPS_3DDEVICE;
			if( FAILED( hr = this->pDD->CreateSurface( &ddsd, &this->pddsFrontBuffer, NULL ) ) ) {
				SAFE_RELEASE( this->pDD );
				D3DXUninitialize();
				return hr;	// プライマリサーフェイスの作成に失敗
			}
			ddsd.dwFlags = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH;
			ddsd.ddsCaps.dwCaps = DDSCAPS_3DDEVICE | DDSCAPS_OFFSCREENPLAIN;
			ddsd.dwWidth = this->nWidth;
			ddsd.dwHeight = this->nHeight;
			if( FAILED( hr = this->pDD->CreateSurface( &ddsd, &this->pddsBackBuffer, NULL ) ) ) {
				SAFE_RELEASE( this->pddsFrontBuffer );
				SAFE_RELEASE( this->pDD );
				D3DXUninitialize();
				return hr;	// バックサーフェイスの作成に失敗
			}
		}
	}
	//
	// クリッパーの作成（ウィンドウモード時のみ）
	{
		if( ! this->b全画面モード )
		{
			LPDIRECTDRAWCLIPPER pClipper;
			if( FAILED( hr = this->pDD->CreateClipper( 0, &pClipper, NULL ) ) )
			{
				SAFE_RELEASE( this->pddsBackBuffer );
				SAFE_RELEASE( this->pddsFrontBuffer );
				SAFE_RELEASE( this->pDD );
				D3DXUninitialize();
				return hr;	// クリッパーの作成に失敗
			}
			pClipper->SetHWnd( 0, this->hWnd );
			this->pddsFrontBuffer->SetClipper( pClipper );
			SAFE_RELEASE( pClipper );
		}
	}
	//
	// Direct3D の取得
	{
		if( FAILED( hr = this->pDD->QueryInterface( IID_IDirect3D7, (LPVOID *) &this->pD3D ) ) )
		{
			if( ! this->b全画面モード ) SAFE_RELEASE( this->pddsBackBuffer );
			SAFE_RELEASE( this->pddsFrontBuffer );
			SAFE_RELEASE( this->pDD );
			D3DXUninitialize();
			return hr;	// Direct3D7 の取得に失敗
		}
	}
	//
	// Direct3D デバイスの作成
	{
		if( FAILED( hr = this->pD3D->CreateDevice( IID_IDirect3DTnLHalDevice, this->pddsBackBuffer, &this->pD3DDevice ) ) )				// TnLHAL でトライ
		{
			if( FAILED( hr = this->pD3D->CreateDevice( IID_IDirect3DHALDevice, this->pddsBackBuffer, &this->pD3DDevice ) ) )			// HAL でトライ
			{
				if( FAILED( hr = this->pD3D->CreateDevice( IID_IDirect3DMMXDevice, this->pddsBackBuffer, &this->pD3DDevice ) ) )		// MMX でトライ
				{
					if( FAILED( hr = this->pD3D->CreateDevice( IID_IDirect3DRGBDevice, this->pddsBackBuffer, &this->pD3DDevice ) ) )	// RGB でトライ
					{
						SAFE_RELEASE( this->pD3D );
						if( ! this->b全画面モード ) SAFE_RELEASE( this->pddsBackBuffer );
						SAFE_RELEASE( this->pddsFrontBuffer );
						SAFE_RELEASE( this->pDD );
						D3DXUninitialize();
						return hr;	// Direct3DDevice の作成に失敗
					} else
						Debug::Msg( _T("Direct3D RGB デバイスを作成しました。\n") );
				} else
					Debug::Msg( _T("Direct3D MMX デバイスを作成しました。\n") );
			} else
				Debug::Msg( _T("Direct3D HAL デバイスを作成しました。\n") );
		} else
			Debug::Msg( _T("Direct3D T&L HAL デバイスを作成しました。\n") );
	}
	//
	// デバイスにビューポートを設定
	{
		D3DVIEWPORT7 vp = { 0, 0, this->nWidth, this->nHeight, 0.0f, 1.0f };
		if( FAILED( hr = this->pD3DDevice->SetViewport( &vp ) ) )
		{
			SAFE_RELEASE( this->pD3DDevice );
			SAFE_RELEASE( this->pD3D );
			if( ! this->b全画面モード ) SAFE_RELEASE( this->pddsBackBuffer );
			SAFE_RELEASE( this->pddsFrontBuffer );
			SAFE_RELEASE( this->pDD );
			D3DXUninitialize();
			return hr;	// ビューポートの設定に失敗
		}
	}
	//
	// 不変のレンダリングステータスの設定
	{
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_LIGHTING, FALSE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ZENABLE, FALSE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ANTIALIAS, FALSE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_TEXTUREPERSPECTIVE, TRUE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHATESTENABLE, TRUE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHAREF, 10 );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHAFUNC, D3DCMP_GREATER );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_COLOROP,		D3DTOP_SELECTARG1 );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_COLORARG1,	D3DTA_TEXTURE );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_ALPHAOP,		D3DTOP_MODULATE );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG1,	D3DTA_TEXTURE );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG2,	D3DTA_DIFFUSE );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_MINFILTER,	D3DTFN_POINT );
		this->pD3DDevice->SetTextureStageState( 0, D3DTSS_MAGFILTER,	D3DTFN_POINT );
	}
	//
	// 適宜変更されるレンダリングステータスの設定
	{
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_COLORKEYENABLE, FALSE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_ALPHABLENDENABLE, FALSE );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_SRCBLEND, D3DBLEND_SRCALPHA );
		this->pD3DDevice->SetRenderState( D3DRENDERSTATE_DESTBLEND, D3DBLEND_INVSRCALPHA );
	}
	//
	// 射影行列をデフォルトのものから変更
	{
		float d, t, a;
		D3DXMATRIX proj;
		CD3DApplication::GetProjectionParam( &d, &t, &a );
		D3DXMatrixPerspectiveFovLH( &proj, t, 1.0f, 0.0000001f, 50.0f );
		D3DMATRIX pm = proj;
		this->pD3DDevice->SetTransform( D3DTRANSFORMSTATE_PROJECTION, &pm );
	}
	//
	// マウスカーソル表示ON/OFF
	{
		if( this->b全画面モード ) {
			while( ShowCursor( FALSE ) >= 0 )
				Sleep(2);
		} else {
			ShowCursor( TRUE );
		}
	}

	return S_OK;
}
void	CD3DApplication::t3D環境の破棄()
{
	this->bActive = false;
	this->bReady  = false;

	// アプリ側の終了処理
	{
		InvalidateDeviceObjects();
		DeleteDeviceObjects();
		FinalCleanup();
	}
	//
	// ディスプレイモードの復元
	{
		if( this->b全画面モード )
			this->pDD->RestoreDisplayMode();
	}
	//
	// 各 DirectX オブジェクトの解放
	{
		this->pDD->SetCooperativeLevel( this->hWnd, DDSCL_NORMAL | DDSCL_NOWINDOWCHANGES );
		SAFE_RELEASE( this->pD3DDevice );
		SAFE_RELEASE( this->pD3D );
		if( ! this->b全画面モード ) SAFE_RELEASE( this->pddsBackBuffer );
		SAFE_RELEASE( this->pddsFrontBuffer );
		SAFE_RELEASE( this->pDD );
		D3DXUninitialize();
	}
	//
	// 全ウィンドウの位置を復元（全画面モード時のみ）
	{
		if( this->b全画面モード )
			WindowPositions::Load();
		WindowPositions::Clear();		// 内部リスト解放
	}
	//
	// タイマ終了
	{
		this->timer.Term();
	}
	//
	// スクリーンセーバが有効だったならもとに戻す
	{
		if( this->bScreenSaverEnable )
			SystemParametersInfo( SPI_SETSCREENSAVEACTIVE, TRUE, 0, SPIF_SENDWININICHANGE );
	}
	//
	// モニタ電源設定の復元
	{
		::SetThreadExecutionState( this->exeState );
	}
}
HRESULT CD3DApplication::t3D環境の再構築( bool b新全画面モード )
{
	HRESULT hr;

	this->bReady = false;
	
	// すべてのデバイス依存オブジェクトを解放する
	{
		InvalidateDeviceObjects();
	}
	//
	// 現在全画面モードならディスプレイモードを復元する
	{
		if( this->b全画面モード )
			this->pDD->RestoreDisplayMode();
	}
	//
	// 各 DirectX オブジェクトの解放
	{
		this->pDD->SetCooperativeLevel( this->hWnd, DDSCL_NORMAL | DDSCL_NOWINDOWCHANGES );
		SAFE_RELEASE( this->pD3DDevice );
		SAFE_RELEASE( this->pD3D );
		if( ! this->b全画面モード ) SAFE_RELEASE( this->pddsBackBuffer );
		SAFE_RELEASE( this->pddsFrontBuffer );
		SAFE_RELEASE( this->pDD );
		D3DXUninitialize();
	}
	//
	// ウィンドウスタイルの設定
	{
		// ウィンドウスタイルを変更した場合は、SetWindowsPos() で内部キャッシュをクリアする必要がある。
		if( b新全画面モード )
		{
			::SetWindowLongPtr( this->hWnd, GWL_STYLE, this->dw全画面スタイル );
			::SetWindowPos( this->hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED );
		}
		else
		{
			::SetWindowLongPtr( this->hWnd, GWL_STYLE, this->dwウィンドウスタイル );
			::SetWindowPos( this->hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_SHOWWINDOW );
		}
	}
	//
	// 全ウィンドウ位置とサイズの復元（全画面モード時のみ）
	{
		if( this->b全画面モード )
		{
			// ウィンドウ位置復元
			WindowPositions::Load();
			WindowPositions::Clear();

			this->t現在のウィンドウのウィンドウ領域とクライアント領域を取得する();
		}
	}
	//
	// デバイスの構築
	{
		bool b保存 = this->b全画面モード;
		this->b全画面モード = b新全画面モード;		// t3D環境の構築() の前に this->b全画面モード の設定が必要
	
		if( FAILED( hr = this->t3D環境の構築() ) ) {
			this->b全画面モード = b保存;
			return hr;
		}
	}
	//
	// デバイス依存オブジェクトの復元の前に、Wait a moment 画像を出しておく。（復元が長引いたときのため）
	{
		CTexture::BeginScene();
		this->tWaitAMomentの描画();
		CTexture::EndScene();
		this->tFlipまたはBlt();
	}
	//
	// デバイス依存オブジェクトの復元
	{
		if( FAILED( hr = this->RestoreDeviceObjects() ) )
			return hr;
	}
	//
	// 準備完了
	this->bReady = true;

	return S_OK;
}
HRESULT CD3DApplication::t3D環境の描画()
{
	HRESULT hr;

	// 協調レベルをチェックする
	{
		if( FAILED( hr = this->pDD->TestCooperativeLevel() ) )
		{
			switch( hr )
			{
			case DDERR_EXCLUSIVEMODEALREADYSET:		// （自分がウィンドウモードのとき）他のアプリが排他モードに入った
			case DDERR_NOEXCLUSIVEMODE:				// 排他を失った（Alt+TABなど）
				Sleep( 1000 );		// 他のアプリが排他モードに入っているので待機する。無駄にCPUを使わない(2006/5/6)
				return S_OK;

			case DDERR_WRONGMODE:					// ディスプレイモードが変更された。全サーフェイスを破棄して作り直しが必要。
				if( this->D3DDisplay.pAdapter->pDevice->bWindowed )
				{
					hr = this->t3D環境の再構築( this->b全画面モード );
					return hr;
				}
				break;
			}
			return hr;
		}
	}
	//
	// 進行描画
	{
		if( this->Render() )
		{
			// Render() が true を返したら WM_CLOSE を送信する。
			SendMessage( this->hWnd, WM_CLOSE, 0, 0 );
			return S_OK;
		}

		this->tFPSを算出する();
	}
	//
	// 表示
	{
		if( FAILED( hr = this->tFlipまたはBlt() ) )
		{
			if( hr == DDERR_SURFACELOST )
			{
				// サーフェイスをすべて復元し、再構築。
				this->pDD->RestoreAllSurfaces();
				return this->RestoreDeviceObjects();
			}
			else
				return hr;	// その他のエラー
		}
	}
	//
	return S_OK;
}
HRESULT CD3DApplication::tFlipまたはBlt()
{
	HRESULT hr = S_OK;

    if( this->pddsFrontBuffer == NULL )
		return FDKERR_初期化されてない;

    // 全画面モード時：
	if( this->b全画面モード )
    {
		if( this->b垂直帰線同期 )
		{
			hr = this->pddsFrontBuffer->Flip( NULL, DDFLIP_WAIT );
			return hr;
		}
		else
		{
			return this->t垂直同期を使わないウェイトとBlt( NULL );
		}
    }

    // ウィンドウモード時：
    else
	{
		return this->t垂直同期を使わないウェイトとBlt( &this->rcクライアント領域 );
	}

	return hr;
}
HRESULT	CD3DApplication::t垂直同期を使わないウェイトとBlt( LPRECT prcクライアント領域 )
{
	static bool   bBltFastを使う = true;		// 初回のみ BltFast で試し、ダメならそれ以降はBltを使う。
	static double db前回の描画開始時刻 = INIT_TIME;

	HRESULT hr = S_OK;

	if( db前回の描画開始時刻 != INIT_TIME )
	{
		double db経過時間 = this->timer.GetSysTime() - db前回の描画開始時刻;
		double db余剰時間 = (double)this->dw描画間隔 - db経過時間;
		if( db余剰時間 > 0 )
			::Sleep( (DWORD)db余剰時間 );		// 余剰時間だけ眠る
	}

retry:
	if( bBltFastを使う )
	{
		RECT rcBack = { 0, 0, this->nWidth, this->nHeight };
		if( FAILED( hr = this->pddsFrontBuffer->BltFast( 0, 0, this->pddsBackBuffer, &rcBack, DDBLTFAST_WAIT ) ) )
		{
			bBltFastを使う = false;	// BltFast() に失敗したので、以後は Blt() を使う。
			goto retry;
		}
	}
	else
		hr = this->pddsFrontBuffer->Blt( prcクライアント領域, this->pddsBackBuffer, NULL, DDBLT_WAIT, NULL );

	db前回の描画開始時刻 = this->timer.GetSysTime();
	
	return hr;
}
//
void	CD3DApplication::t全画面・ウィンドウモードを切り替える()
{
	HRESULT hr;

	// 現在ウィンドウモードなら全ウィンドウの位置を保存する
	if( ! this->b全画面モード )
		WindowPositions::Save();

	// デバイス切り替え
	if( FAILED( hr = t3D環境の再構築( ! this->b全画面モード ) ) )
	{
		// 失敗したら元に戻す（１回だけ）
		if( FAILED( hr = t3D環境の再構築( this->b全画面モード ) ) )
			return;	// これでもエラーが出たら無視
	}
}

void	CD3DApplication::t現在のウィンドウのウィンドウ領域とクライアント領域を取得する()
{
	// ウィンドウ領域の取得
	GetWindowRect( this->hWnd, &this->rcウィンドウ領域 );

	// クライアント領域の取得
	GetClientRect( this->hWnd, &this->rcクライアント領域 );
	ClientToScreen( this->hWnd, (POINT*)&this->rcクライアント領域.left );		// left, top	 をスクリーン座標へ
	ClientToScreen( this->hWnd, (POINT*)&this->rcクライアント領域.right );		// right, bottom をスクリーン座標へ
}

void	CD3DApplication::tWaitAMomentの描画()
{
/*
	以下はサンプル

	CSurface sf;
	sf.InitDeviceObjectsFromFile( _T("Wait a moment"), _T("sysdata/ses_frame.jpg") );
	if( SUCCEEDED( sf.RestoreDeviceObjects() ) )
	{
		sf.Draw( 0, 0 );
		sf.InvalidateDeviceObjects();
	}
	sf.DeleteDeviceObjects();
*/
}

void	CD3DApplication::tFPSを算出する()
{
	static double fpstime = INIT_TIME;
	static int fps = 0;

	// 初期化
	if( fpstime == INIT_TIME )
	{
		fpstime = this->timer.GetSysTime();
		this->nFPS = fps = 0;
	}

	// １秒経過ごとに計算
	while( this->timer.GetSysTime() - fpstime >= 1000.0 )
	{
		this->nFPS = fps;
		fps = 0;
		fpstime += 1000.0;
	}
	fps ++;
}
//
INT		CD3DApplication::Run()
{
    MSG  msg;

	while( true )
    {
		if( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
		{
			if( msg.message == WM_QUIT )
				break;
			TranslateMessage( &msg );
			DispatchMessage( &msg );
		}
		else
		{
			if( this->bReady && this->bActive )
			{
				if( FAILED( t3D環境の描画() ) ) {
					Debug::Msg( _T("3D環境の描画に失敗しました。終了します。\n") );
					SendMessage( this->hWnd, WM_CLOSE, 0, 0 );
				}
			}
		}
	}
    return (INT)msg.wParam;
}
LRESULT CD3DApplication::WndProc( HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam )
{
    switch( uMsg )
    {

	case WM_CLOSE:
		this->bActive = false;
		this->t3D環境の破棄();
		break;

	case WM_DESTROY:
		PostQuitMessage( 0 );
		return 0;

	case WM_SIZE:
		// ウィンドウが全部隠れたり最小化されたりしたら this->bActive = false にする。
		this->bActive = ( wParam == SIZE_MAXHIDE || wParam == SIZE_MINIMIZED ) ? false : true;
		break;

	case WM_MOVE:
		// 新しい領域座標を取得しておく
		this->t現在のウィンドウのウィンドウ領域とクライアント領域を取得する();
		break;

	case WM_GETMINMAXINFO:
		// アプリの最小サイズの問い合わせへの回答。
		((MINMAXINFO*)lParam)->ptMinTrackSize.x = 100;
		((MINMAXINFO*)lParam)->ptMinTrackSize.y = 100;
		return 0;

	case WM_NCHITTEST:
		if( this->b全画面モード )
			return HTCLIENT;		// すべてがクライアント領域
		break;
	}
	return DefWindowProc( hWnd, uMsg, wParam, lParam );
}
	}//AppBase
}//FDK