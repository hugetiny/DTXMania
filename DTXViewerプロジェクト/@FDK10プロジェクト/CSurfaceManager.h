#pragma once

#include "CSurface.h"

namespace FDK {
	namespace Graphics {

class CSurfaceManager
{
public:
	// キャッシュの使用フラグのリセット
	void		ResetCache();

	// キャッシュされたサーフェイスを返す。
	// 返されたサーフェイスは、RestoreDeviceObjects() まで完了している状態。
	CSurface*	GetCachedSurface( LPCTSTR name, LPCTSTR filename, DWORD width=0, DWORD height=0, LOADPLACE place=LOADTO_SYSTEM );

	// キャッシュ内の未使用サーフェイスの削除
	void		RemoveUnusedSurfaces();

	HRESULT	OneTimeSceneInit();						// 内部リストの初期化
	HRESULT	InitDeviceObjects();					// 内部リストの各サーフェイスの初期化
	HRESULT	RestoreDeviceObjects();					// 内部リストの各サーフェイスの構築
	HRESULT	InvalidateDeviceObjects();				// 内部リストの各サーフェイスの無効化
	HRESULT	DeleteDeviceObjects();					// 内部リストの各サーフェイスの破棄
	HRESULT	FinalCleanup();							// 内部リストの破棄

	CSurfaceManager();

protected:
	struct SurfaceList {
		bool		bUse;							// 使用するなら true
		TCHAR		strSurfaceName[ _MAX_PATH ];	// サーフェイス名
		TCHAR		strFileName[ _MAX_PATH ];		// ファイル名
		DWORD		width, height;					// サイズ
		LOADPLACE	place;							// 生成場所
		FILETIME	ftLastWriteTime;				// 最終更新時刻
		CSurface	surface;						// サーフェイス
		SurfaceList	*prev, *next;					// 前／次のセル
	} *pSurfaceList, *pSurfaceListLast;
};

	}//Graphics
}//FDK

using namespace FDK::Graphics;
