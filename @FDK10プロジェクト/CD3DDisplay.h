#pragma once

namespace FDK {
	namespace AppBase {

//--------------------------------------------------------------------------
// Name: D3DModeInfo
// Desc: モード情報
//--------------------------------------------------------------------------

typedef DDSURFACEDESC2	D3DModeInfo;

//--------------------------------------------------------------------------
// Name: D3DDeviceInfo
// Desc: デバイス情報（３Ｄの使えるもののみ）
//--------------------------------------------------------------------------

enum D3DAPPDEV {
	D3DAPPDEV_HALTnL,
	D3DAPPDEV_HAL,
	D3DAPPDEV_HEL,
	D3DAPPDEV_REF
};

struct D3DDeviceInfo
{
	// デバイス情報
	GUID			guidDevice;				// GUID
	GUID*			pDeviceGUID;			// guidDevice へのポインタ（あるいはNULL）
	D3DAPPDEV		type;					// D3DAPPDEV_...
	bool			bHardware;				// type が HAL or HALTnL なら true
	D3DDEVICEDESC7	ddDeviceDesc;			// デバイス能力
	TCHAR			strDesc[40];			// デバイス名
	bool			bStereoCompatible;		// 複合フリッピングが必要なら true
	
	// モードリスト（D3DAdapterInfo::modes[] からこの３Ｄデイバスに対応しているもののみ抜粋したリスト）
	D3DModeInfo		modes[150];				// モード配列
	DWORD			dwNumModes;				// モード数
	
	// ステータス
	bool			bWindowed;				// 現在のウィンドウ／全画面モード
	D3DModeInfo*	pMode;					// 現在のモード（全画面時）
};

//--------------------------------------------------------------------------
// Name: D3DAdapterInfo
// Desc: アダプタ情報（DirectDraw情報）
//--------------------------------------------------------------------------

struct D3DAdapterInfo 
{
	// アダプタ情報
	GUID			guidAdapter;			// GUID
	GUID*			pAdapterGUID;			// guidAdapter へのポインタ（あるいはNULL）
	TCHAR			strDriver[MAX_DDDEVICEID_STRING];	// アダプタ名
	TCHAR			strDesc[40];			// アダプタの説明
	DWORD			dwTotalMemory;			// 全容量
	DDCAPS			ddHALCaps;				// HAL の能力
	DDCAPS			ddHELCaps;				// HEL の能力
	bool			bDesktopCompatible;		// デスクトップ互換なら true

	// DirectDraw の対応する全モード（全デバイスの最小公倍数）
	D3DModeInfo		modes[150];				// モードリスト
	DWORD			dwNumModes;				// モード数

	// デバイスリスト
	D3DDeviceInfo	devices[5];				// デバイス配列
	DWORD			dwNumDevices;			// デバイス数

	// ステータス
	D3DDeviceInfo*	pDevice;				// 現在のデバイス
};

//--------------------------------------------------------------------------
// Name: CD3DDisplay
// Desc: アダプタ／デバイス／モードの管理クラス
//--------------------------------------------------------------------------

class CD3DDisplay
{
// ※大半のメンバがコールバック関数からアクセスされるため、public になっている。
public:
	D3DAdapterInfo	adapters[20];			// アダプタ配列
	DWORD			dwNumAdapters;			// アダプタ数
	D3DAdapterInfo*	pAdapter;				// 現在のアダプタ

public:
	HRESULT	EnumerateDevices();							// アダプタ／デバイス／モードの列挙。
	bool	FindMode( DWORD w, DWORD h, DWORD bpp );	// 現在のデバイスから指定されたモード（w×h×bpp）を検索し、内部状態をセットする。

	virtual HRESULT	ConfirmDevice( DDCAPS* pddHALCaps, D3DDEVICEDESC7* pd3dd )	{ return S_OK; }    // ３Ｄデバイスの判断（オーバーライド用）

public:
	CD3DDisplay();
	virtual ~CD3DDisplay() {};
};

	}//AppBase
}//FDK

using namespace FDK::AppBase;
