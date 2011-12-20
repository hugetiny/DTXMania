#pragma once

#include "Activity.h"
#include "CSurface.h"
#include "DTX.h"
#include "CStage.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

class ActPlayAVI : public Activity
{
public:
	void	Start( int nチャンネル番号, DTX::AVI* pAVI, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, double db総移動時間ms, double db移動開始時刻ms );
	void	SkipStart( double db移動開始時刻ms );
	void	Stop();
	void	Cont( double db再開時刻ms );

	ActPlayAVI();
	bool	MyOneTimeSceneInit();
	bool	MyActivate();
	bool	MyUnActivate();
	bool	MyFinalCleanup();
	bool	MyInitDeviceObjects();
	bool	MyRestoreDeviceTextures();
	bool	MyRestoreDeviceSurfaces();
	bool	MyInvalidateDeviceObjects();
	bool	MyDeleteDeviceObjects();
	int		MainWork( int x, int y );

protected:
	CSurface	sfAVI;		// AVI描画用サーフェイス

	struct _structLayer
	{
		DTX::AVI*	pAVI;
		int		n開始サイズW, n開始サイズH;
		int		n終了サイズW, n終了サイズH;
		int		n画像側開始位置X, n画像側開始位置Y;		// BMP画像内相対座標
		int		n画像側終了位置X, n画像側終了位置Y;		// BMP画像内相対座標
		int		n表示側開始位置X, n表示側開始位置Y;		// DTXManiaのBGA領域相対座標
		int		n表示側終了位置X, n表示側終了位置Y;		// DTXManiaのBGA領域相対座標
		double	db総移動時間ms;	
		double	db移動開始時刻ms;
		DWORD	dw前回表示したフレーム番号;
	}
	structLayer[ MAX_AVI_LAYER ];

};
		}//Play
	}//Stage
}//DTXViewer
