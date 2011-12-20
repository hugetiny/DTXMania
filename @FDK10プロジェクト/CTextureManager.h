#pragma once

#include "CTexture.h"

namespace FDK {
	namespace Graphics {

class CTextureManager
{
public:
	// キャッシュの使用フラグのリセット
	void ResetCache();
	
	// キャッシュされたテクスチャを返す。
	// 返されたテクスチャは、RestoreDeviceObjects() まで完了している状態。
	CTexture* GetCachedTexture( LPCTSTR name, LPCTSTR filename, DWORD width=0, DWORD height=0 );

	// キャッシュ内の未使用テクスチャの削除
	void RemoveUnusedTextures();

	HRESULT	OneTimeSceneInit();						// 内部リストの初期化
	HRESULT	InitDeviceObjects();					// 内部リストの各テクスチャの初期化
	HRESULT	RestoreDeviceObjects();					// 内部リストの各テクスチャの構築
	HRESULT	InvalidateDeviceObjects();				// 内部リストの各テクスチャの無効化
	HRESULT	DeleteDeviceObjects();					// 内部リストの各テクスチャの破棄
	HRESULT	FinalCleanup();							// 内部リストの破棄

public:
	CTextureManager();
	virtual ~CTextureManager() {};

protected:
	struct TextureList {
		bool		bUse;							// 使用するなら true
		TCHAR		strTextureName[ _MAX_PATH ];	// テクスチャ名
		TCHAR		strFileName[ _MAX_PATH ];		// ファイル名
		DWORD		width, height;					// サイズ
		FILETIME	ftLastWriteTime;				// 最終更新時刻
		CTexture	texture;						// テクスチャ
		TextureList	*prev, *next;					// 前／次のセル
	} *pTextureList, *pTextureListLast;
};

	}//Graphics
}//FDK

using namespace FDK::Graphics;
