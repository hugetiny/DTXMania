#include "stdafx.h"
#include "CDirectInput.h"
#include "FDKError.h"

namespace FDK {
	namespace IO {

// JOYSTICKコールバック関数
static BOOL CALLBACK EnumJoySticksCallback( LPCDIDEVICEINSTANCE lpddi, LPVOID pvRef ) {
	return ((CDirectInput*) pvRef)->EnumJoySticksCallback( lpddi );
}
CDirectInput::CDirectInput()
{
	// COM の初期化
	m_bNeedCleanupCOM	= SUCCEEDED( CoInitialize( NULL ) ) ? true : false;
	this->pDI				= NULL;
	this->pDIDKeyboard		= NULL;
	this->nJoySticks		= 0;
}

CDirectInput::~CDirectInput()
{
	// COM の終了（COM の初期化に失敗している場合はスキップ）
	if( m_bNeedCleanupCOM )
		CoUninitialize();
}

HRESULT CDirectInput::Init( HWND hWnd )
{
	HRESULT hr;

	// DirectInput7 の生成
	if( FAILED( hr = DirectInputCreateEx( GetModuleHandle(NULL), DIRECTINPUT_VERSION, IID_IDirectInput7, (void**)&this->pDI, NULL ) ) )
		return hr;	// 失敗

	// キーボードデバイスの初期化
	if( FAILED( hr = InitKeyboard( hWnd ) ) )
	{
		Term();
		return hr;	// 失敗
	}

	// ジョイスティックデバイスの初期化
	if( FAILED( hr = InitJoySticks( hWnd ) ) )
	{
		Term();
		return hr;	//	失敗
	}

	return S_OK;
}

HRESULT CDirectInput::InitKeyboard( HWND hWnd )
{
	HRESULT hr;

	// 内部変数の初期化
	for( int i = 0; i < 256; i++ )
	{
		this->byKeyState[i] = 0x00;
		this->bKeyPushDown[i] = false;
		this->bKeyPullUp[i] = false;
	}

	// キーボードデバイスの生成
	if( FAILED( hr = this->pDI->CreateDeviceEx( GUID_SysKeyboard, IID_IDirectInputDevice7, (LPVOID*)&this->pDIDKeyboard, NULL ) ) )
	{
		this->pDIDKeyboard = NULL;
		return hr;		// 失敗
	}
	
	// キーボードのデータ形式の設定
	if( FAILED( hr = this->pDIDKeyboard->SetDataFormat( &c_dfDIKeyboard ) ) )
	{
		SAFE_RELEASE( this->pDIDKeyboard );
		return hr;		// 失敗
	}

	// キーボードの協調レベルの設定
	// WindowNTで動作させるには、ここは DISCL_FOREGROUND | DISCL_NONEXCLUSIVE でなくてはいけない。
	if( FAILED( hr = this->pDIDKeyboard->SetCooperativeLevel( hWnd, DISCL_FOREGROUND | DISCL_NONEXCLUSIVE | DISCL_NOWINKEY ) ) )
	{
		SAFE_RELEASE( this->pDIDKeyboard );
		return hr;		// 失敗
	}

	// キーボードのイベント取得用バッファサイズ MAX_DIEVENT_BUFFER を設定。
	DIPROPDWORD dipdw;
	dipdw.diph.dwSize       = sizeof( DIPROPDWORD );
	dipdw.diph.dwHeaderSize = sizeof( DIPROPHEADER );
	dipdw.diph.dwObj        = 0;
	dipdw.diph.dwHow        = DIPH_DEVICE;
	dipdw.dwData            = MAX_DIEVENT_BUFFER;
	if( FAILED( hr = this->pDIDKeyboard->SetProperty( DIPROP_BUFFERSIZE, &dipdw.diph ) ) )
	{
		SAFE_RELEASE( this->pDIDKeyboard );
		return hr;		// 失敗
	}

	// Acquire
	//this->pDIDKeyboard->Acquire();		Acquire は行わずに終了。（→ Init() 後にプロパティを設定できる。）

	return S_OK;
}

HRESULT CDirectInput::InitJoySticks( HWND hWnd )
{
	HRESULT hr;

	// ジョイスティックの列挙
	this->nJoySticks = 0;
	if( FAILED( hr = this->pDI->EnumDevices( DIDEVTYPE_JOYSTICK, FDK::IO::EnumJoySticksCallback, (void*)this, DIEDFL_ATTACHEDONLY ) ) )
		return hr;	// 失敗

	// それぞれのジョイスティックデバイスについて...
	for( int i = 0; i < this->nJoySticks; i++ )
	{
		// 初期化
		ZeroMemory( (LPVOID) &m_JoyStickInfo[i].state, sizeof( DIJOYSTATE2 ) );
		for( int j = 0; j < 128; j++ ) {
			m_JoyStickInfo[i].bPushDown[j] = false;
			m_JoyStickInfo[i].bPullUp[j] = false;
		}
		for( int j = 0; j < 3; j++ )
			m_JoyStickInfo[i].lMin[j] = m_JoyStickInfo[i].lMax[j] = 0;
		
		for( int j = 0; j < 6; j++ ) {
			m_JoyStickInfo[i].bAxisDown[0][j] = false;
			m_JoyStickInfo[i].bAxisDown[1][j] = false;
		}
		m_JoyStickInfo[i].nAxisDownIndex = 0;

		// ジョイスティックデバイスの生成
		if( FAILED( hr = this->pDI->CreateDeviceEx( m_JoyStickInfo[i].instance.guidInstance, IID_IDirectInputDevice7, (LPVOID*)&(m_JoyStickInfo[i].pDID), NULL ) ) )
		{
			m_JoyStickInfo[i].pDID = NULL;
			continue;		// 中断せず次のジョイスティックへ遷移
		}

		// ジョイスティックのデータ形式の設定
		if( FAILED( hr = m_JoyStickInfo[i].pDID->SetDataFormat( &c_dfDIJoystick2 ) ) )
		{
			SAFE_RELEASE( m_JoyStickInfo[i].pDID );
			continue;		// 中断せず次のジョイスティックへ遷移
		}

		// ジョイスティックの協調レベルの設定
		if( FAILED( hr = m_JoyStickInfo[i].pDID->SetCooperativeLevel( hWnd, DISCL_BACKGROUND | DISCL_EXCLUSIVE ) ) )
		{
			SAFE_RELEASE( m_JoyStickInfo[i].pDID );
			continue;		// 中断せず次のジョイスティックへ遷移
		}

		// イベント取得用バッファサイズの設定
		DIPROPDWORD dipdw;
		dipdw.diph.dwSize       = sizeof( DIPROPDWORD );
		dipdw.diph.dwHeaderSize = sizeof( DIPROPHEADER );
		dipdw.diph.dwObj        = 0;
		dipdw.diph.dwHow        = DIPH_DEVICE;
		dipdw.dwData            = MAX_DIEVENT_BUFFER;
		if( FAILED( hr = m_JoyStickInfo[i].pDID->SetProperty( DIPROP_BUFFERSIZE, &dipdw.diph ) ) )
		{
			SAFE_RELEASE( m_JoyStickInfo[i].pDID );
			continue;		// 中断せず次のジョイスティックへ遷移
		}
 
		// X,Y,Z軸の値域の取得
		for( int j = 0; j < 3; j++ )
		{
			static const DWORD dwObj[3] = { DIJOFS_X, DIJOFS_Y, DIJOFS_Z };
			DIPROPRANGE dipr;
			dipr.diph.dwSize = sizeof( DIPROPRANGE );
			dipr.diph.dwHeaderSize = sizeof( DIPROPHEADER );
			dipr.diph.dwObj = dwObj[j];
			dipr.diph.dwHow = DIPH_BYOFFSET;
			dipr.lMin = dipr.lMax = 0;
			if( SUCCEEDED( m_JoyStickInfo[i].pDID->GetProperty( DIPROP_RANGE, &dipr.diph ) ) ) {
				m_JoyStickInfo[i].lMin[j] = dipr.lMin;
				m_JoyStickInfo[i].lMax[j] = dipr.lMax;
			}
		}

		// Acquire
		//m_JoyStickInfo[i].pDID->Acquire();	//		Acquire は行わない。（→ Init() 後にプロパティを設定できる。）
	}

	return S_OK;
}

BOOL CDirectInput::EnumJoySticksCallback( LPCDIDEVICEINSTANCE lpddi )
{
	if( this->nJoySticks >= MAX_JOYSTICKS )
		return DIENUM_STOP;		// 数が多い

	CopyMemory( &m_JoyStickInfo[ this->nJoySticks ].instance, lpddi, sizeof( DIDEVICEINSTANCE ) );
	this->nJoySticks ++;

	return DIENUM_CONTINUE;
}

bool CDirectInput::Poll()
{
	bool bRet = true;
	
	if( ! Pollキーボード() )
		bRet = false;

	if(  ! Pollジョイスティック() )
		bRet = false;

	return bRet;
}


bool CDirectInput::Pollキーボード()
{
	// 状態クリア
	for( int i = 0; i < 256; i++ ) 
	{
		this->bKeyPullUp[i] = false;
		this->bKeyPushDown[i] = false;
	}

	if( ! this->pDIDKeyboard )
		return true;	// 未初期化

	// Acquire してみる
	HRESULT hr = this->pDIDKeyboard->Acquire();
	if( hr != DI_OK && hr != S_FALSE )		// 成功すれば DI_OK, 取得済みなら S_FALSE が返る。
		return false;

	// ポーリング
	if( FAILED( this->pDIDKeyboard->Poll() ) )
		return false;

	// 1.状態取得
	if( FAILED( this->pDIDKeyboard->GetDeviceState( 256, this->byKeyState ) ) )
		ZeroMemory( this->byKeyState, 256 );		// 状態取得に失敗したら全OFF扱い

	// 2.イベント取得
	DIDEVICEOBJECTDATA ddod[ MAX_DIEVENT_BUFFER ];
	DWORD dwItems = MAX_DIEVENT_BUFFER;
	if( SUCCEEDED( this->pDIDKeyboard->GetDeviceData( sizeof( DIDEVICEOBJECTDATA ), ddod, &dwItems, 0 ) ) )
	{
		for( DWORD i = 0; i < dwItems; i++ )
		{
			if( ( ddod[i].dwData & 0xFF ) == 0 )
				this->bKeyPullUp[ ddod[i].dwOfs ] = true;		// キーが離された
			else
				this->bKeyPushDown[ ddod[i].dwOfs ] = true;		// キーが押された
		}
	}
	return true;
}

bool CDirectInput::Pollジョイスティック()
{
	for( int i = 0; i < this->nJoySticks; i++ )
	{
		// 状態クリア
		for( int j = 0; j < 128; j++ ) 
		{
			m_JoyStickInfo[i].bPushDown[j] = false;
			m_JoyStickInfo[i].bPullUp[j] = false;
		}

		if( ! m_JoyStickInfo[i].pDID )
			continue;

		// Acquire してみる
		HRESULT hr = m_JoyStickInfo[i].pDID->Acquire();
		if( hr != DI_OK && hr != S_FALSE )		// 成功すれば DI_OK, 取得済みなら S_FALSE が返る。
			continue;

		// ポーリング
		if( FAILED( m_JoyStickInfo[i].pDID->Poll() ) )
			continue;

		// 1. 状態取得
		if( FAILED( m_JoyStickInfo[i].pDID->GetDeviceState( sizeof( DIJOYSTATE2 ), &m_JoyStickInfo[i].state ) ) )
			ZeroMemory( &m_JoyStickInfo[i].state, sizeof(DIJOYSTATE2) );			// 状態取得に失敗したら全ボタンOFF扱い
		m_JoyStickInfo[i].nAxisDownIndex = 1 - m_JoyStickInfo[i].nAxisDownIndex;	// 軸状態のイベントチェック
		for( int k = 0; k < 6; k++ )
			m_JoyStickInfo[i].bAxisDown[ m_JoyStickInfo[i].nAxisDownIndex ][ k ] = IsAxisDown( i, k );

		// 2.イベント取得
		DIDEVICEOBJECTDATA ddod[ MAX_DIEVENT_BUFFER ];
		DWORD dwItems = MAX_DIEVENT_BUFFER;
		if( SUCCEEDED( m_JoyStickInfo[i].pDID->GetDeviceData( sizeof( DIDEVICEOBJECTDATA ), ddod, &dwItems, 0 ) ) )
		{
			for( DWORD ev = 0; ev < dwItems; ev++ )
			{
				for( int k = 0; k < 128; k++ )
				{
					if( ddod[ ev ].dwOfs == DIJOFS_BUTTON( k ) )
					{
						if( ( ddod[ ev ].dwData & 0xFF ) == 0 )
							m_JoyStickInfo[i].bPullUp[ k ] = true;			// ボタンが離された
						else
							m_JoyStickInfo[i].bPushDown[ k ] = true;		// ボタンが押された
					}
				}
			}
		}
	}
	return true;
}

void CDirectInput::Term()
{
	// キーボードデバイスの解放
	if( this->pDIDKeyboard )
	{
		this->pDIDKeyboard->Unacquire();
		SAFE_RELEASE( this->pDIDKeyboard );
	}

	// ジョイスティックデバイスの解放
	for( int i = 0; i < this->nJoySticks; i++ )
	{
		if( m_JoyStickInfo[i].pDID )
		{
			m_JoyStickInfo[i].pDID->Unacquire();
			SAFE_RELEASE( m_JoyStickInfo[i].pDID );
		}
	}
	this->nJoySticks = 0;

	// DirectInput7 の解放
	SAFE_RELEASE( this->pDI );
}

bool CDirectInput::IsKeyDown( int nDIK )
{
	return ( nDIK >= 0 && nDIK < 256 && (this->byKeyState[ nDIK ] & 0x80) != 0x00 );
}

bool CDirectInput::IsKeyUp( int nDIK )
{
	return ( nDIK >= 0 && nDIK < 256 && (this->byKeyState[ nDIK ] & 0x80) == 0x00 );
}

bool CDirectInput::IsKeyPushDown( int nDIK )
{
	return ( nDIK >= 0 && nDIK < 256 && this->bKeyPushDown[ nDIK ] );
}

bool CDirectInput::IsKeyPullUp( int nKey )
{
	return ( nKey >= 0 && nKey < 256 && this->bKeyPullUp[ nKey ] );
}

bool CDirectInput::IsButtonDown( int nID, int nButton )
{
	if( nID < 0 || nID >= this->nJoySticks || nButton < 0 || nButton > 127 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	return ( ( m_JoyStickInfo[ nID ].state.rgbButtons[ nButton ] & 0x80 ) != 0x00 );
}

bool CDirectInput::IsButtonUp( int nID, int nButton )
{
	if( nID < 0 || nID >= this->nJoySticks || nButton < 0 || nButton > 127 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	return ( ( m_JoyStickInfo[ nID ].state.rgbButtons[ nButton ] & 0x80 ) == 0x00 );
}

bool CDirectInput::IsButtonPushDown( int nID, int nButton )
{
	return ( nID >= 0 && nID < this->nJoySticks && nButton >= 0 && nButton < 128 && m_JoyStickInfo[ nID ].bPushDown[ nButton ] );
}

bool CDirectInput::IsButtonPullUp( int nID, int nButton )
{
	return ( nID >= 0 && nID < this->nJoySticks && nButton >= 0 && nButton < 128 && m_JoyStickInfo[ nID ].bPullUp[ nButton ] );
}

bool CDirectInput::IsAxisDown( int nID, int nAxis )
{
	if( nID < 0 || nID >= this->nJoySticks || nAxis < 0 || nAxis > 5 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	if( nAxis == 0 && m_JoyStickInfo[ nID ].state.lX < ( 3 * m_JoyStickInfo[ nID ].lMin[0] +     m_JoyStickInfo[ nID ].lMax[0] ) / 4 ) return true;
	if( nAxis == 1 && m_JoyStickInfo[ nID ].state.lX > (     m_JoyStickInfo[ nID ].lMin[0] + 3 * m_JoyStickInfo[ nID ].lMax[0] ) / 4 ) return true;
	if( nAxis == 2 && m_JoyStickInfo[ nID ].state.lY < ( 3 * m_JoyStickInfo[ nID ].lMin[1] +     m_JoyStickInfo[ nID ].lMax[1] ) / 4 ) return true;
	if( nAxis == 3 && m_JoyStickInfo[ nID ].state.lY > (     m_JoyStickInfo[ nID ].lMin[1] + 3 * m_JoyStickInfo[ nID ].lMax[1] ) / 4 ) return true;
	if( nAxis == 4 && m_JoyStickInfo[ nID ].state.lZ < ( 3 * m_JoyStickInfo[ nID ].lMin[2] +     m_JoyStickInfo[ nID ].lMax[2] ) / 4 ) return true;
	if( nAxis == 5 && m_JoyStickInfo[ nID ].state.lZ > (     m_JoyStickInfo[ nID ].lMin[2] + 3 * m_JoyStickInfo[ nID ].lMax[2] ) / 4 ) return true;
	return false;
}

bool CDirectInput::IsAxisUp( int nID, int nAxis )
{
	if( nID < 0 || nID >= this->nJoySticks || nAxis < 0 || nAxis > 5 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	if( nAxis == 0 && m_JoyStickInfo[ nID ].state.lX >= ( 3 * m_JoyStickInfo[ nID ].lMin[0] +     m_JoyStickInfo[ nID ].lMax[0] ) / 4 ) return true;
	if( nAxis == 1 && m_JoyStickInfo[ nID ].state.lX <= (     m_JoyStickInfo[ nID ].lMin[0] + 3 * m_JoyStickInfo[ nID ].lMax[0] ) / 4 ) return true;
	if( nAxis == 2 && m_JoyStickInfo[ nID ].state.lY >= ( 3 * m_JoyStickInfo[ nID ].lMin[1] +     m_JoyStickInfo[ nID ].lMax[1] ) / 4 ) return true;
	if( nAxis == 3 && m_JoyStickInfo[ nID ].state.lY <= (     m_JoyStickInfo[ nID ].lMin[1] + 3 * m_JoyStickInfo[ nID ].lMax[1] ) / 4 ) return true;
	if( nAxis == 4 && m_JoyStickInfo[ nID ].state.lZ >= ( 3 * m_JoyStickInfo[ nID ].lMin[2] +     m_JoyStickInfo[ nID ].lMax[2] ) / 4 ) return true;
	if( nAxis == 5 && m_JoyStickInfo[ nID ].state.lZ <= (     m_JoyStickInfo[ nID ].lMin[2] + 3 * m_JoyStickInfo[ nID ].lMax[2] ) / 4 ) return true;
	return false;
}

bool CDirectInput::IsAxisPushDown( int nID, int nAxis )
{
	if( nID < 0 || nID >= this->nJoySticks || nAxis < 0 || nAxis > 5 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	return     ! m_JoyStickInfo[ nID ].bAxisDown[ 1 - m_JoyStickInfo[ nID ].nAxisDownIndex ][ nAxis ]
			&&   m_JoyStickInfo[ nID ].bAxisDown[     m_JoyStickInfo[ nID ].nAxisDownIndex ][ nAxis ];
}

bool CDirectInput::IsAxisPullUp( int nID, int nAxis )
{
	if( nID < 0 || nID >= this->nJoySticks || nAxis < 0 || nAxis > 5 || m_JoyStickInfo[ nID ].pDID == NULL )
		return false;

	return       m_JoyStickInfo[ nID ].bAxisDown[ 1 - m_JoyStickInfo[ nID ].nAxisDownIndex ][ nAxis ]
			&& ! m_JoyStickInfo[ nID ].bAxisDown[     m_JoyStickInfo[ nID ].nAxisDownIndex ][ nAxis ];
}

LPDIJOYSTATE2 CDirectInput::GetJoyState( int nID )
{
	if( nID < 0 || nID >= this->nJoySticks || m_JoyStickInfo[ nID ].pDID == NULL )
		return NULL;

	return &(m_JoyStickInfo[ nID ].state);
}

HRESULT	CDirectInput::GetJoyProperty( int nID, REFGUID rguidProp, LPDIPROPHEADER pdiph )
{
	if( nID < 0 || nID >= this->nJoySticks || m_JoyStickInfo[ nID ].pDID == NULL )
		return S_FALSE;

	return m_JoyStickInfo[ nID ].pDID->GetProperty( rguidProp, pdiph );
}

	}//IO
}//FDK