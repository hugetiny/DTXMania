#pragma once

#include "Activity.h"
#include "DTX.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

class ActPlayBGA : public Activity
{
public:
	void	Start( int nチャンネル, DTX::BMP *bmp, DTX::BMPTEX* bmptex,	int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, double db総移動時間ms, double db移動開始時刻ms=INIT_TIME );
	void	SkipStart( double db移動開始時刻ms );
	void	Stop();
	void	Cont( double db再開時刻ms );
	void	ChangeScope( int nチャンネル, DTX::BMP* bmp, DTX::BMPTEX* bmptex );		// bmp, bmptex .. 新しい画像（いずれかが非NULL） 

	ActPlayBGA();
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
	struct _structLayer {
		DTX::BMP		*pBMP;			// BMPへのポインタ			（※ pBMP | pBMPTEX のどちらかが非 NULL）
		DTX::BMPTEX		*pBMPTEX;		// BMPTEX へのポインタ
		SIZE	sz開始サイズ;
		SIZE	sz終了サイズ;
		POINT	pt画像側開始位置;		// BMP画像内相対座標
		POINT	pt画像側終了位置;
		POINT	pt表示側開始位置;		// DTXManiaのBGA領域相対座標
		POINT	pt表示側終了位置;
		double	db総移動時間ms;
		double	db移動開始時刻ms;
	} structLayer[ 8 ];					// 最大8重ねまで可能。（小さい添字のものほど下位に描画）
};
		}//Play
	}//Stage
}//DTXViewer
