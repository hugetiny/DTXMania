#include "stdafx.h"
#include "Activity.h"

namespace FDK {
	namespace AppBase {

Activity::Activity()
{
	this->bActivate = false;
	this->bFirstWork = true;
}

bool Activity::OneTimeSceneInit()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyOneTimeSceneInit();
	
	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( ! pa->OneTimeSceneInit() )
			bRet = false;
	}
	return bRet;
}

bool Activity::Activate()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyActivate();
	
	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( ! pa->Activate() )
			bRet = false;
	}
	return bRet;
}

bool Activity::UnActivate()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyUnActivate();
	
	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( !pa->UnActivate() )
			bRet = false;
	}
	return bRet;
}

bool Activity::FinalCleanup()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyFinalCleanup();
	
	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( !pa->FinalCleanup() )
			bRet = false;
	}
	return bRet;
}

bool Activity::InitDeviceObjects()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyInitDeviceObjects();

	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( !pa->InitDeviceObjects() )
			bRet = false;
	}
	return bRet;
}

bool Activity::RestoreDeviceTextures()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyRestoreDeviceTextures();

	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( !pa->RestoreDeviceTextures() )
			bRet = false;
	}
	return bRet;
}

bool Activity::RestoreDeviceSurfaces()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyRestoreDeviceSurfaces();

	// (2)子Activity集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( !pa->RestoreDeviceSurfaces() )
			bRet = false;
	}
	return bRet;
}

bool Activity::InvalidateDeviceObjects()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyInvalidateDeviceObjects();

	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( ! pa->InvalidateDeviceObjects() )
			bRet = false;
	}
	return bRet;
}

bool Activity::DeleteDeviceObjects()
{
	// 自分＋子集合全部について、エラーが出たら記憶しておく。
	// エラーがでてても、呼び出しは全部についてひと通り行う。
	bool bRet = true;

	// (1)自分
	bRet = MyDeleteDeviceObjects();

	// (2)子集合
	Activity* pa;
	this->Activities.resetList();
	while( ( pa = this->Activities.getNext() ) != NULL )
	{
		if( ! pa->DeleteDeviceObjects() )
			bRet = false;
	}
	return bRet;
}

bool Activity::MyActivate()
{
	if( this->bActivate )
		return true;		// すでに活性化してるなら何もしない

	this->bActivate = true;		// MyResoreDeviceObjects() の呼び出しの前に設定

	bool bTex成功 = MyRestoreDeviceTextures();
	bool bSur成功 = MyRestoreDeviceSurfaces();
	if( ! bTex成功 || ! bSur成功 )		// エラーが出てもひと通りぜんぶ回す
		this->bActivate = false;

	return this->bActivate;
}

bool Activity::MyUnActivate()
{
	if( ! this->bActivate )
		return true;		// 活性化してないなら何もしない

	MyInvalidateDeviceObjects();

	this->bActivate = false;	// MyInvalidateDeviceObjects() の呼び出しの後に設定
	return true;
}

bool Activity::MyRestoreDeviceTextures()
{
	if( ! this->bActivate )
		return true;		// 活性化してないなら何もしない

	// 子テクスチャの構築
	bool bRet = true;
	CTexture* pt;
	this->Textures.resetList();
	while( ( pt = this->Textures.getNext() ) != NULL )
	{
		if( FAILED( pt->RestoreDeviceObjects() ) )
			bRet = false;
	}
	return bRet;
}

bool Activity::MyRestoreDeviceSurfaces()
{
	if( ! this->bActivate )
		return true;		// 活性化してないなら何もしない

	// 子サーフェイスの構築
	bool bRet = true;
	CSurface* ps;
	this->Surfaces.resetList();
	while( ( ps = this->Surfaces.getNext() ) != NULL )
	{
		if( FAILED( ps->RestoreDeviceObjects() ) )
			bRet = false;
	}
	return bRet;
}

bool Activity::MyInvalidateDeviceObjects()
{
	if( ! this->bActivate )
		return true;		// 活性化してないなら何もしない

	bool bRet = true;

	// 子テクスチャの無効化
	CTexture* pt;
	this->Textures.resetList();
	while( ( pt = this->Textures.getNext() ) != NULL )
	{
		if( FAILED( pt->InvalidateDeviceObjects() ) )
			bRet = false;
	}

	// 子サーフェイスの無効化
	CSurface* ps;
	this->Surfaces.resetList();
	while( ( ps = this->Surfaces.getNext() ) != NULL )
	{
		if( FAILED( ps->InvalidateDeviceObjects() ) )
			bRet = false;
	}

	return bRet;
}

bool Activity::MyDeleteDeviceObjects()
{
	bool bRet = true;

	// 子テクスチャの削除
	CTexture* pt;
	this->Textures.resetList();
	while( ( pt = this->Textures.getNext() ) != NULL )
	{
		if( FAILED( pt->DeleteDeviceObjects() ) )
			bRet = false;
	}

	// 子サーフェイスの削除
	CSurface* ps;
	this->Surfaces.resetList();
	while( ( ps = this->Surfaces.getNext() ) != NULL )
	{
		if( FAILED( ps->DeleteDeviceObjects() ) )
			bRet = false;
	}

	return bRet;
}

	}//AppBase
}//FDK