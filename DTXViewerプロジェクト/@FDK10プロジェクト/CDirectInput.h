/*
	CDirectInput クラス

	DirectInput のラッパークラス。キーボード、ジョイスティックを扱う。（マウスは未対応。）
		・キーボード		…　標準キーボードのみ（２台目以降は無視）
		・ジョイスティック	…　初期化（Init() ）時にアタッチ済みのもののみ対象。Init() 後にアタッチされたものは無視。
								フォースフィードバックに関するサポートはなし。
	【使い方】

	(1) CD3DApplication::OneTimeSceneInit() で、Init() を呼び出す。
		これにより、以下のような初期化処理が行われる。
			・DirectInput を生成し、ウィンドウと関連付ける。
			・標準キーボード、標準マウス、アタッチ済みジョイスティックを列挙し、デバイスを生成する。
			　各デバイスとも、協調レベルは Foreground | NonExclusive | NoWinKey で固定。
	
	(2) 適当な周期で、Poll() を呼び出す。
		これにより、各デバイスの現在の状態、ならびに前回のPoll() 以降に発生したイベントデータが CDirectInput 内部の配列変数に取り込まれる。

	(3) 各デバイスの入力データを取得する。
		入力データには、次の２種類がある。
			・瞬間状態	…　Poll() が実行された瞬間の、各デバイスの状態。
			・イベント	…　前回のPoll() 以降に発生したイベントデータ。（ボタンのUp, Down など）

			※イベントデータを扱う場合には、フォーカスの消失や再取得に注意する。
			（例：SHIFTキーDownイベント取得後にフォーカスを失い、別アプリ上でSHIFTキーが離され、再びフォーカスが戻った場合、
			　　　アプリは SHIFT キーの Up イベントを取得し損ねてしまう。）
			　→ WM_ACTIVATE をフェッチし、適切なリセットなどを行うこと。
				（CDirectInput クラスでは、WM_ACTIVATE に関するイベントは何も行わない。）


		IsAxis系は、CDirectInput 内部で瞬間状態をチェックしてイベントを検出する従来のやり方での実装。
		　nAxis=0(左),1(右),2(上),3(下),4(前),5(後)

	(4) CD3DApplication::FinalCleanup() で、Term() を呼び出す。
		これにより、DirectInput 関係の各オブジェクトが解放される。

	【Acquireについて】

	・本クラスでは、Poll() の最初に常に Acquire を実行し、これに失敗したら何もせず帰還する。
	　（Acquire 可能になるまでウェイトしたりはしない。）
	・本クラスでは WM_ACTIVATE に関する処理は何も行わないので、イベントデータで状態管理する必要があるアプリは自分で処理すること。
		
*/

#pragma once

namespace FDK {
	namespace IO {

#define	MAX_JOYSTICKS		32			// 検出する最大のジョイスティック数
#define	MAX_DIEVENT_BUFFER	32			// 入力イベントのバッファサイズ

class CDirectInput
{
public:
	HRESULT	Init( HWND hWnd );								// 初期化
	void	Term();											// 終了処理
	bool	Poll();											// 状態更新（入力のポーリング）

	bool	IsKeyDown( int nDIK );							// 瞬間状態(Down)取得；キーが押下状態であれば true を返す。(nDIK ... 0～255 (DIK_...; dinput.h参照))
	bool	IsKeyUp( int nDIK );							// 瞬間状態(Up)取得；キーが非押下状態であれば true を返す。(nDIK ... 0～255 (DIK_...; dinput.h参照))
	bool	IsKeyPushDown( int nDIK );						// イベント(Down)取得；キーが非押下から押下状態へ変遷すれば true を返す。(nDIK ... 0～255 (DIK_...; dinput.h参照))
	bool	IsKeyPullUp( int nDIK );						// イベント(Up)取得；キーが押下から非押下状態へ変遷すれば true を返す。(nKey ... 0～255 (DIK_...; dinput.h参照))
	
	bool	IsButtonDown( int nID, int nButton );			// 瞬間状態取得；ボタンが押下状態であれば true を返す。(nID ... JoyStick 番号(0..), nButton ... 0..127)
	bool	IsButtonUp( int nID, int nButton );				// 瞬間状態取得；ボタンが解放状態であれば true を返す。(nID ... JoyStick 番号(0..), nButton ... 0..127)
	bool	IsButtonPushDown( int nID, int nButton );		// イベント取得；ボタンが非押下から押下状態へ変遷すれば true を返す。(nID ... JoyStick 番号(0..), nButton ... 0..127)
	bool	IsButtonPullUp( int nID, int nButton );			// イベント取得；ボタンが押下から非押下状態へ変遷すれば true を返す。(nID ... JoyStick 番号(0..), nButton ... 0..127)

	bool	IsAxisDown( int nID, int nAxis );				// 軸が押下状態であれば true を返す。(nID ... JoyStick 番号(0..), nAxis ... 0(左),1(右),2(上),3(下),4(前),5(後))
	bool	IsAxisUp( int nID, int nAxis );					// 軸が解放状態であれば true を返す。(nID ... JoyStick 番号(0..), nAxis ... 0(左),1(右),2(上),3(下),4(前),5(後))
	bool	IsAxisPushDown( int nID, int nAxis );			// 軸が解放から押下状態へ変遷すれば true を返す。(nID ... JoyStick 番号(0..), nAxis ... 0(左),1(右),2(上),3(下),4(前),5(後))
	bool	IsAxisPullUp( int nID, int nAxis );				// 軸が押下から解放状態へ変遷すれば true を返す。(nID ... JoyStick 番号(0..), nAxis ... 0(左),1(右),2(上),3(下),4(前),5(後))
	
	int		GetJoysticksNum()	 { return this->nJoySticks; }	// ジョイスティックの数
	LPDIJOYSTATE2 GetJoyState( int nID );					// 瞬間状態取得；現在の DIJOYSTATE2 を返す。(nID ... JoyStick 番号(0..))
	HRESULT	GetJoyProperty( int nID, REFGUID rguidProp, LPDIPROPHEADER pdiph );	// デバイス情報の取得
public:
	CDirectInput();
	virtual ~CDirectInput();

protected:
	LPDIRECTINPUT7			pDI;							// DirectInput オブジェクト

	// キーボード
	LPDIRECTINPUTDEVICE7	pDIDKeyboard;
	BYTE					byKeyState[ 256 ];				// 瞬間状態記憶用
	bool					bKeyPushDown[ 256 ];			// イベント記憶用；キーが PushDown されれば true	→ Poll() 時に更新。
	bool					bKeyPullUp[ 256 ];				// イベント記憶用；キーが PullUp されれば true		→       〃

	// ジョイスティック
	int						nJoySticks;						// 検出されたジョイスティックの数
	bool					m_bNeedCleanupCOM;				// COM が初期化成功していれば true
	struct JoyStickInfo {
		DIDEVICEINSTANCE		instance;					// Joystick インスタンス
		LPDIRECTINPUTDEVICE7	pDID;						// Joystick デバイス (NULL なら生成中にエラー発生したことを示す→そのデバイスは使えない）
		DIJOYSTATE2				state;						// 瞬間状態記憶用
		bool					bPushDown[ 128 ];			// イベント記憶用；ボタンが PushDown されれば true
		bool					bPullUp[ 128 ];				// イベント記憶用；ボタンが PullUp   されれば true
		LONG					lMin[3],lMax[3];			// 値域([]=x,y,z軸)
		bool					bAxisDown[2][6];			// 軸状態記憶用
		int						nAxisDownIndex;				// フリップ用
	} m_JoyStickInfo[ MAX_JOYSTICKS ];

protected:
	HRESULT	InitKeyboard( HWND hWnd );						// キーボードデバイスの初期化
	HRESULT	InitJoySticks( HWND hWnd );						// ジョイスティックデバイスの初期化
	bool	Pollキーボード();								// キーボード状態更新（入力のポーリング）
	bool	Pollジョイスティック();							// ジョイスティック状態更新（入力のポーリング）

public:
	// ジョイスティック列挙コールバック関数;
	// ここでは DIDEVICEINSTANCE をひたすらコピーするだけ。
	// デバイス生成などの必要な処理はInitJoysticks() の中でやる。
	BOOL	EnumJoySticksCallback( LPCDIDEVICEINSTANCE lpddi );
};

	}//IO
}//FDK

using namespace FDK::IO;
