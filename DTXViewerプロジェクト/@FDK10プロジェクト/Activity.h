
//
// Name: Activity クラス
// Desc: 内部状態と内部画像を保持し、進行と描画を行う実体の基本クラス。
//
// ・内部状態とは、進行にかかわる状態変数、そして内部画像などをさす。（画像は状態の一部）
// ・内部画像とは、テクスチャやサーフェイスをさす。
//
// ・Activity の派生クラスは、My... メソッドならびに MainWork() メソッドをオーバーライドする。
// ・１つの Activity は、複数の子Activity、子テクスチャを保持することができる。
//     ・AddChild() / RemoveChild() で追加削除
//   　・Activity、テクスチャの AddChild() は、MyOneTimeSceneInit() 内で行うこと。
//   　・My...メソッドでは、子Activity の操作を何も行わないこと。
//
// ・アプリケーション（CD3DApplication またはその派生）が保有する Activity を、
//  「トップレベル Activity」と呼ぶ。
//
// ・アプリケーションは、以下のタイミングでトップレベルActivity を処理すること。
//     ・OneTimeSceneInit() 時			→　すべてのトップレベルActivity の OneTimeSceneInit() を呼び出す。
//     ・InitDeviceObjects() 時			→　　　　　　　　　〃　　　　　　　InitDeviceObjects() を呼び出す。
//     ・RestoreDeviceObjects() 時		→　　　　　　　　　〃　　　　　　　RestoreDeviceTextures(), RestoreDeviceSurfaces() の順に呼び出す。
//     ・InvalidateDeviceObjects() 時	→　　　　　　　　　〃　　　　　　　InvalidateDeviceObjects() を呼び出す。
//     ・DeleteDeviceObjects() 時		→　　　　　　　　　〃　　　　　　　DeleteDeviceObjects() を呼び出す。
//     ・FinalCleanup() 時				→　　　　　　　　　〃　　　　　　　FinalCleanup() を呼び出す。
//     ・Activity を有効化したい時		→　　　　　　　　　〃　　　　　　　Activate() を呼び出す。
//     ・Activity を無効化したい時		→　　　　　　　　　〃　　　　　　　UnActivate() を呼び出す。
//

#pragma once

#include "CTexture.h"
#include "CSurface.h"
#include "CSet.h"

namespace FDK {
	namespace AppBase {

class Activity
{
public:
	void	AddChild( Activity* c ) 	{Activities.insert(c);}		// 子Activityの追加
	void	AddChild( CTexture* c ) 	{Textures.insert(c);}		// 子Textureの追加
	void	AddChild( CSurface* c ) 	{Surfaces.insert(c);}		// 子Surfaceの追加
	void	RemoveChild( Activity* c )	{Activities.erase(c);}		// 子Activityの削除
	void	RemoveChild( CTexture* c )	{Textures.erase(c);}		// 子Textureの削除
	void	RemoveChild( CSurface* c )	{Surfaces.erase(c);}		// 子Surfaceの削除

	virtual bool	OneTimeSceneInit();								// MyObeTimeSceneInit()＋子集合のすべての OneTimeSceneInit() を呼び出す。
	virtual bool	Activate();										// MyActivate()＋子集合のすべての Activate() を呼び出す。
	virtual bool	UnActivate();									// MyUnActivate()＋子集合のすべての UnActivate() を呼び出す。
	virtual bool	FinalCleanup();									// MyFinalCleanup()＋子集合のすべての FinalCleanup() を呼び出す。
	virtual bool	InitDeviceObjects();							// MyInitDeviceObjects()＋子集合のすべての InitDeviceObjects() を呼び出す。
	virtual bool	RestoreDeviceTextures();						// MyRestoreDeviceTextures()＋子集合のすべての RestoreDeviceTextures() を呼び出す。
	virtual bool	RestoreDeviceSurfaces();						// MyRestoreDeviceSurfaces()＋子集合のすべての RestoreDeviceSurfaces() を呼び出す。
	virtual bool	InvalidateDeviceObjects();						// MyInvalidateDeviceObjects()＋子集合のすべての InvalidateDeviceObjects() を呼び出す。
	virtual bool	DeleteDeviceObjects();							// MyDeleteDeviceObjects()＋子集合のすべての DeleteDeviceObjects() を呼び出す。

	virtual int		MainWork()				{return 0;}				// 進行と描画
	bool			IsActive()				{return bActivate;}		// 現在活性化状態ならtrueを返す。

protected:
	virtual bool	MyOneTimeSceneInit()	{return true;}			// 初期化
	virtual bool	MyActivate();									// 活性化； 　MyActivate() は MyRestoreDevice～() を実行する。
	virtual bool	MyUnActivate();									// 非活性化； MyUnActivate() は MyInvalidateDeviceObjects() を実行する。
	virtual bool	MyFinalCleanup()		{return true;}			// 終了処理
	virtual bool	MyInitDeviceObjects()	{return true;}			// 画像の登録
	virtual bool	MyRestoreDeviceTextures();						// 子テクスチャ画像の構築
	virtual bool	MyRestoreDeviceSurfaces();						// 子サーフェイス画像の構築
	virtual bool	MyInvalidateDeviceObjects();					// 画像の無効化
	virtual bool	MyDeleteDeviceObjects();						// 画像の削除

public:
	Activity();

protected:
	bool			bActivate;			// 活性化状態ならtrue
	bool			bFirstWork;

	CSet<Activity>	Activities;			// Activitiy 子集合
	CSet<CTexture>	Textures;				// CTexture 子集合
	CSet<CSurface>	Surfaces;				// CSurface 子集合
};
	}//AppBase
}//FDK

using namespace FDK::AppBase;
