#include "stdafx.h"
#include "CD3DDisplay.h"
#include "FDKError.h"

namespace FDK {
	namespace AppBase {

// Direct3D デバイス列挙用のコールバック関数。
static HRESULT WINAPI DeviceEnumCallback( TCHAR* strDesc, TCHAR* strName, D3DDEVICEDESC7* pDesc, VOID* pContext )
{
	CD3DDisplay* app		= (CD3DDisplay*) pContext;
	D3DAdapterInfo* adapter	= app->pAdapter;
	D3DDeviceInfo* device	= adapter->pDevice;

	// デバイス情報をコピーする。
	ZeroMemory( device, sizeof(D3DDeviceInfo) );
	device->guidDevice			= pDesc->deviceGUID;
	device->pDeviceGUID			= &device->guidDevice;
	device->bHardware			= (pDesc->dwDevCaps & D3DDEVCAPS_HWRASTERIZATION) ? true : false;
	memcpy( &device->ddDeviceDesc, pDesc, sizeof(D3DDEVICEDESC7) );
	if( adapter->pAdapterGUID )
		lstrcpyn( device->strDesc, adapter->strDesc, 39 );							// strDesc[]
	else
		lstrcpyn( device->strDesc, strName, 39 );
	if( device->bHardware )															// type
		device->type = ( device->guidDevice == IID_IDirect3DTnLHalDevice ) ? D3DAPPDEV_HALTnL : D3DAPPDEV_HAL;
	else
		device->type = ( device->guidDevice == IID_IDirect3DRefDevice ) ? D3DAPPDEV_REF : D3DAPPDEV_HEL;
	device->bStereoCompatible = false;					// bStereoCompatible（後で再チェックする）
	device->dwNumModes = 0;								// dwNumModes
	device->bWindowed = adapter->bDesktopCompatible;	// bWindowed（初期でウィンドウになれるならtrue）
	device->pMode = &(device->modes[0]);				// pMode（後で640x480のモードを設定する）

	// 重複回避；セカンダリアダプタ以降については、HAL/HALTnL のみリストアップする。
    if( adapter->pAdapterGUID != NULL && ! device->bHardware )
		return D3DENUMRET_OK;
	
    // アプリにこのデバイスの許可／不許可を判断させる。
    if( FAILED( app->ConfirmDevice( &adapter->ddHALCaps, &device->ddDeviceDesc ) ) )
		return D3DENUMRET_OK;
	
	// DirectDraw の全モードリスト（Adapterのmodes[]）のうち、Direct3D デバイスと互換性のあるモードを抜粋したリストを作成する。
    for( DWORD i=0; i<adapter->dwNumModes; i++ )
    {
        DDSURFACEDESC2 ddsdDDMode	= adapter->modes[i];
        DWORD dwDDDepth				= ddsdDDMode.ddpfPixelFormat.dwRGBBitCount;			// アダプタの深度
        DWORD dwD3DDepths			= device->ddDeviceDesc.dwDeviceRenderBitDepth;		// D3Dデバイスの深度

		// Direct3D デバイスのモードと互換性のあるモードだけを認める。
        if( ( ( dwDDDepth == 32 ) && ( dwD3DDepths & DDBD_32 ) ) ||
            ( ( dwDDDepth == 24 ) && ( dwD3DDepths & DDBD_24 ) ) ||
            ( ( dwDDDepth == 16 ) && ( dwD3DDepths & DDBD_16 ) ) )
        {
			// モードをデバイスモードのリストにコピーする。
            device->modes[device->dwNumModes++] = ddsdDDMode;

			// ステレオフリッピングチェーンであるかを記録する。（モード中１つでもそうなら trur になる）
            if( ddsdDDMode.ddsCaps.dwCaps2 & DDSCAPS2_STEREOSURFACELEFT )
                device->bStereoCompatible = true;
        }
    }

	// デバイスがモードを１つもサポートしないならここで釈放。
    if( device->dwNumModes == 0 )
        return D3DENUMRET_OK;

	// デフォルトの全画面モードとして 640x480x16 モードを検索する。
	for( DWORD i=0; i < device->dwNumModes; i++ )
	{
		if( ( device->modes[i].dwWidth == 640 ) &&
			( device->modes[i].dwHeight == 480 ) &&
			( device->modes[i].ddpfPixelFormat.dwRGBBitCount == 16 ) )
		{
			device->pMode = &( device->modes[i] );
			break;
		}
	}

	// デバイスを認め、戻る。
    if( adapter->dwNumDevices < 4 )
	{
		adapter->dwNumDevices++;
		adapter->pDevice++;
	}

    return D3DENUMRET_OK;
}

// DirectDraw のフルスクリーンモード列挙用のコールバック関数。
static HRESULT WINAPI ModeEnumCallback( DDSURFACEDESC2* pddsd, VOID* pContext )
{
	D3DAdapterInfo* adapter = (D3DAdapterInfo*) pContext;
	adapter->modes[ adapter->dwNumModes ] = (*pddsd);
	if( adapter->dwNumModes < 149 ) adapter->dwNumModes++;
    return DDENUMRET_OK;
}

// ディスプレイモード順にソートするためのコールバック関数。
static int SortModesCallback( const VOID* arg1, const VOID* arg2 )
{
    DDSURFACEDESC2* p1 = (DDSURFACEDESC2*)arg1;
    DDSURFACEDESC2* p2 = (DDSURFACEDESC2*)arg2;

    if( p1->dwWidth < p2->dwWidth )
        return -1;
    if( p1->dwWidth > p2->dwWidth )
        return +1;

    if( p1->dwHeight < p2->dwHeight )
        return -1;
    if( p1->dwHeight > p2->dwHeight )
        return +1;

    if( p1->ddpfPixelFormat.dwRGBBitCount < p2->ddpfPixelFormat.dwRGBBitCount )
        return -1;
    if( p1->ddpfPixelFormat.dwRGBBitCount > p2->ddpfPixelFormat.dwRGBBitCount )
        return +1;

    return 0;
}

// アダプタ列挙用のコールバック関数。
static BOOL WINAPI AdapterEnumCallback( GUID* pGUID, TCHAR* strDesc, TCHAR* strName, VOID* pContext )
{
    LPDIRECTDRAW7	pDD;
    LPDIRECT3D7		pD3D;
    HRESULT			hr;
	TCHAR			strErrorMsg[2048];
	
	CD3DDisplay* app		= (CD3DDisplay*) pContext;
	D3DAdapterInfo* adapter = app->pAdapter;
	
    // GUID を指定して DirectDraw を作成する。
    hr = DirectDrawCreateEx( pGUID, (VOID**)&pDD, IID_IDirectDraw7, NULL );
	if( FAILED(hr) )
	{
		D3DXGetErrorString( hr, 2048, strErrorMsg );
        return D3DENUMRET_OK;	// 失敗
    }
	
    // D3Dデバイスを列挙するために、Direct3D を取得する。
    hr = pDD->QueryInterface( IID_IDirect3D7, (VOID**)&pD3D );
    if( FAILED(hr) )
	{
        pDD->Release();
		D3DXGetErrorString( hr, 2048, strErrorMsg );
        return D3DENUMRET_OK;	// 失敗
	}

	// アダプタ情報構造体へデータをコピーする。
	ZeroMemory( adapter, sizeof(D3DAdapterInfo) );
    if( pGUID )
	{
        adapter->guidAdapter = (*pGUID);				// 列挙が終わると GUID も無効になるので、GUID の本体ごと控えておく。
        adapter->pAdapterGUID = &adapter->guidAdapter;	// といっても GUID が NULL の場合もあるので、識別のために GUID へのポインタも用意する。
    }
	DDDEVICEIDENTIFIER2 dddi;
	pDD->GetDeviceIdentifier( &dddi, 0 );
	lstrcpyn( adapter->strDriver, dddi.szDescription, 512 );	// strDriver[]
	lstrcpyn( adapter->strDesc, strDesc, 39 );					// strDesc[]
	DWORD dwTotal, dwFree;
	DDSCAPS2 ddsc;
	ZeroMemory( &ddsc, sizeof(ddsc) );
	ddsc.dwCaps = DDSCAPS_VIDEOMEMORY;
	pDD->GetAvailableVidMem( &ddsc, &dwTotal, &dwFree );
	adapter->dwTotalMemory = dwTotal;							// dwTotalMemory
	adapter->ddHALCaps.dwSize = sizeof(DDCAPS);
    adapter->ddHELCaps.dwSize = sizeof(DDCAPS);
    pDD->GetCaps( &adapter->ddHALCaps, &adapter->ddHELCaps );	// ddHALCaps, ddHELCaps
    if( adapter->ddHALCaps.dwCaps2 & DDCAPS2_CANRENDERWINDOWED )	// ウィンドウモードの描画ができ、
        if( adapter->pAdapterGUID == NULL )							// ドライバの GUID が NULL であるなら
            adapter->bDesktopCompatible = true;						// デスクトップ互換アダプタである。

	adapter->dwNumModes = 0;						// dwNumModes
	adapter->dwNumDevices = 0;						// dwNumDevices
	adapter->pDevice = &(adapter->devices[0]);		// pDevice
	
	// DirectDraw で利用可能な全モードを列挙し、ソートして内部に記憶しておく。
	// → このモードリストを母集合として、各３Ｄデバイスで利用可能なモードをピックアップする。
    pDD->EnumDisplayModes( 0, NULL, adapter, ModeEnumCallback );
    qsort( adapter->modes, adapter->dwNumModes, sizeof(DDSURFACEDESC2), SortModesCallback );

	// ３Ｄデバイスをすべて列挙する。
    pD3D->EnumDevices( DeviceEnumCallback, app );

	// デフォルトデバイスを選択する。
	int nRank = -1;
	for( DWORD d=0; d<adapter->dwNumDevices; d++ )
	{
		if( adapter->devices[d].type == D3DAPPDEV_HALTnL )						// 最優先は HALTnL
		{
			nRank = 4;
			adapter->pDevice = &( adapter->devices[d] );
		}
		else if( adapter->devices[d].type == D3DAPPDEV_HAL && nRank < 4 )		// 次に HAL
		{
			nRank = 3;
			adapter->pDevice = &( adapter->devices[d] );
		}
		else if( adapter->devices[d].type == D3DAPPDEV_HEL && nRank < 3 )		// 次に HEL
		{
			nRank = 2;
			adapter->pDevice = &( adapter->devices[d] );
		}
		else
		{
			nRank = 1;
			adapter->pDevice = &( adapter->devices[d] );						// 最後にその他
		}
	}
	if( nRank < 0 )
		adapter->pDevice = &( adapter->devices[0] );
	
	// 掃除して次のアダプタへ。
    pD3D->Release();
    pDD->Release();
	if( app->dwNumAdapters < 19 )
	{
		app->dwNumAdapters ++;
		app->pAdapter ++;
	}

	return DDENUMRET_OK;
}

CD3DDisplay::CD3DDisplay()
{
	this->pAdapter = &(this->adapters[0]);
	this->dwNumAdapters = 0;
}

HRESULT CD3DDisplay::EnumerateDevices()
{
	// 列挙する
    DirectDrawEnumerate( AdapterEnumCallback, this );

	if( this->dwNumAdapters == 0 )
		return FDKERR_アダプタがない;

	// m_pAdapter にデフォルトのアダプタへのポインタをセットする。
	for( DWORD a = 0; a < this->dwNumAdapters; a++ )
	{
		if( this->adapters[a].bDesktopCompatible )
		{
			this->pAdapter = &(this->adapters[a]);
			break;
		}
	}
	return S_OK;
}

bool	CD3DDisplay::FindMode( DWORD w, DWORD h, DWORD bpp )
{
	D3DAdapterInfo* a = this->pAdapter;
	D3DDeviceInfo*  d = a->pDevice;
	
	for( DWORD i = 0; i < d->dwNumModes; i++ )
	{
		if( ( d->modes[i].dwWidth == w ) &&
			( d->modes[i].dwHeight == h ) &&
			( d->modes[i].ddpfPixelFormat.dwRGBBitCount == bpp ) )
		{
			d->pMode = &( d->modes[i] );
			return true;		// 見つかった
		}
	}
	return false;	// なかった
}


	}//AppBase
}//FDK
