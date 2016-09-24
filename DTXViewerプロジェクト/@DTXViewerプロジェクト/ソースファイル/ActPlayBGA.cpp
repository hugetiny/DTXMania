#include "stdafx.h"
#include "ActPlayBGA.h"
#include "DTX.h"
#include "DTXViewer.h"
#include "CTimer.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

static const int s_nChannel[8] = {
	0x04, 0x07, 0x55, 0x56, 0x57, 0x58, 0x59, 0x60,
};

ActPlayBGA::ActPlayBGA()
{
	this->bFirstWork = true;
	this->bActivate = false;
}

bool ActPlayBGA::MyOneTimeSceneInit()
{
	// ここにコードを記述する。AddChild() など。

	return Activity::MyOneTimeSceneInit();
}
bool ActPlayBGA::MyFinalCleanup()
{
	// ここにコードを記述する。

	return Activity::MyFinalCleanup();
}
bool ActPlayBGA::MyInitDeviceObjects()
{
	// ここにコードを記述する。CTexture/CSurface::InitDeviceObjects() など。

	return Activity::MyInitDeviceObjects();
}
bool ActPlayBGA::MyDeleteDeviceObjects()
{
	// ここにコードを記述する。

	// 次の Activity::MyDeleteDeviceObjects() では、子テクスチャ・子サーフェイスの Delete (CTexture/CSurface::Delete) が行われる。
	return Activity::MyDeleteDeviceObjects();
}
bool ActPlayBGA::MyActivate()
{
	this->bFirstWork = true;
	// ここにコードを記述する。
	for( int i = 0; i < 8; i++ ) {
		this->structLayer[i].pBMP = NULL;
		this->structLayer[i].pBMPTEX = NULL;
		this->structLayer[i].db移動開始時刻ms = INIT_TIME;
	}

	// 次の Activity::MyActivate() では、this->bActivity のセットならびに子サーフェイス・子テクスチャの Restore (MyRestoreDeviceTexture/Surface) が行われる。
	// なお、子 Activity の活性化は行われない。（このメソッドを抜けてから行われる。）
	return Activity::MyActivate();
}
bool ActPlayBGA::MyUnActivate()
{
	// ここにコードを記述する。

	// 次の Activity::MyUnActivate() では、this->bActivity のリセットならびに子サーフェイス・子テクスチャの Invalidate (MyInvalidateDeviceObjects) が行われる。
	// なお、子Activity の非活性化は行われない。（このメソッドを抜けてから行われる。）
	return Activity::MyUnActivate();
}
bool ActPlayBGA::MyRestoreDeviceTextures()
{
	// ここにコードを記述する。

	// 次の Activity::MyRestoreDeviceTextures() では、子テクスチャの Restore (CTexture::Restore) が行われる。
	return Activity::MyRestoreDeviceTextures();
}
bool ActPlayBGA::MyRestoreDeviceSurfaces()
{
	// ここにコードを記述する。

	// 次の Activity::MyRestoreDeviceSurfaces() では、子サーフェイスの Restore (CSurface::Restore) が行われる。
	return Activity::MyRestoreDeviceSurfaces();
}
bool ActPlayBGA::MyInvalidateDeviceObjects()
{
	// ここにコードを記述する。

	// 次の Activity::MyInvalidateDeviceObjects() では、子テクスチャ・子サーフェイスの Invalidate (CTexture/CSurface::Invalidate) が行われる。
	return Activity::MyRestoreDeviceSurfaces();
}
//
void ActPlayBGA::Start( int nチャンネル, DTX::BMP *bmp, DTX::BMPTEX* bmptex, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, double db総移動時間ms, double db移動開始時刻ms )
{
	// チャンネルチェック
	for( int i = 0; i < 8; i++ )
	{
		if( nチャンネル == s_nChannel[i] )
		{
			// セルに値を設定												BMP/BMPTEX	BGA		BGAPAN
			this->structLayer[i].pBMP				= bmp;				//		○		○		○
			this->structLayer[i].pBMPTEX			= bmptex;			//		○		○		○
			this->structLayer[i].sz開始サイズ.cx	= n開始サイズW;		//		○		○		○
			this->structLayer[i].sz開始サイズ.cy	= n開始サイズH;		//		○		○		○
			this->structLayer[i].sz終了サイズ.cx	= n終了サイズW;		//		--		--		○
			this->structLayer[i].sz終了サイズ.cy	= n終了サイズH;		//		--		--		○
			this->structLayer[i].pt画像側開始位置.x	= n画像側開始位置X;	//		--		○		○
			this->structLayer[i].pt画像側開始位置.y	= n画像側開始位置Y;	//		--		○		○
			this->structLayer[i].pt画像側終了位置.x	= n画像側終了位置X;	//		--		--		○
			this->structLayer[i].pt画像側終了位置.y	= n画像側終了位置Y;	//		--		--		○
			this->structLayer[i].pt表示側開始位置.x	= n表示側開始位置X;	//		--		○		○
			this->structLayer[i].pt表示側開始位置.y	= n表示側開始位置Y;	//		--		○		○
			this->structLayer[i].pt表示側終了位置.x	= n表示側終了位置X;	//		--		--		○
			this->structLayer[i].pt表示側終了位置.y	= n表示側終了位置Y;	//		--		--		○
			this->structLayer[i].db総移動時間ms		= db総移動時間ms;	//		--		--		○
			this->structLayer[i].db移動開始時刻ms	= (db移動開始時刻ms != INIT_TIME) ? db移動開始時刻ms : g_Timer.Get();
		}
	}
}
void ActPlayBGA::SkipStart( double db移動開始時刻ms )
{
	for( DTX::Chip* cell = g_DTX.pChip; cell != NULL; cell = cell->next )
	{
		// 開始時刻より後ろなら、そのチップで調査打ち切り。
		if( cell->dbTime > db移動開始時刻ms ) 
			break;

		// BMP, BMPTEX, BGA, BGAPAN なら再生指示
		switch( cell->BGAtype )
		{

		case BGATYPE_BMP:
			if( cell->pBMP )
				Start( cell->nChannel, cell->pBMP, NULL,
					   cell->pBMP->pSurface->dwWidth, cell->pBMP->pSurface->dwHeight, 0, 0,
					   0, 0, 0, 0,
					   0, 0, 0, 0, 0, cell->dbTime );
			break;

		case BGATYPE_BMPTEX:
			if( cell->pBMPTEX )
				Start( cell->nChannel, NULL, cell->pBMPTEX,
					   cell->pBMPTEX->pTexture->dwWidth, cell->pBMPTEX->pTexture->dwHeight, 0, 0,
					   0, 0, 0, 0,
					   0, 0, 0, 0, 0, cell->dbTime );
			break;

		case BGATYPE_BGA:
			if( cell->pBGA )
				Start( cell->nChannel, cell->pBMP, cell->pBMPTEX,
					   cell->pBGA->x2 - cell->pBGA->x1, cell->pBGA->y2 - cell->pBGA->y1, 0, 0, 
					   cell->pBGA->x1, cell->pBGA->y1, 0, 0,
					   cell->pBGA->ox, cell->pBGA->oy, 0, 0, 0, cell->dbTime );
			break;

		case BGATYPE_BGAPAN:
			if( cell->pBGAPan )
				Start( cell->nChannel, cell->pBMP, cell->pBMPTEX,
						cell->pBGAPan->sw, cell->pBGAPan->sh, cell->pBGAPan->ew, cell->pBGAPan->eh,
						cell->pBGAPan->ssx, cell->pBGAPan->ssy, cell->pBGAPan->sex, cell->pBGAPan->sey,
						cell->pBGAPan->dsx, cell->pBGAPan->dsy, cell->pBGAPan->dex, cell->pBGAPan->dey,
						cell->dbLong, cell->dbTime );
			break;
		}
	}
}
void ActPlayBGA::Stop()
{
	for( int i = 0; i < 8; i++ )
		this->structLayer[i].db移動開始時刻ms = INIT_TIME;
}
void ActPlayBGA::Cont( double db再開時刻ms )
{
	for( int i = 0; i < 8; i++ )
	{
		if(  ( this->structLayer[i].pBMP    == NULL && this->structLayer[i].pBMPTEX == NULL ) 
		  || ( this->structLayer[i].pBMP    != NULL && ( ! this->structLayer[i].pBMP->bUse    || this->structLayer[i].pBMP->pSurface    == NULL ) )
		  || ( this->structLayer[i].pBMPTEX != NULL && ( ! this->structLayer[i].pBMPTEX->bUse || this->structLayer[i].pBMPTEX->pTexture == NULL ) ) )
			continue;

		this->structLayer[i].db移動開始時刻ms = db再開時刻ms;
	}
}
void ActPlayBGA::ChangeScope( int nチャンネル, DTX::BMP* bmp, DTX::BMPTEX* bmptex )
{
	// チャンネルチェック
	for( int i = 0; i < 8; i++ )
	{
		if( nチャンネル == s_nChannel[i] )
		{
			// セルに値を設定(bmp, bmptex のみ)
			this->structLayer[i].pBMP		= bmp;
			this->structLayer[i].pBMPTEX	= bmptex;
		}
	}
}
int  ActPlayBGA::MainWork( int x, int y )
{
	for( int i = 0; i < 8; i++ )
	{
		// 未使用ならスキップ
		if( this->structLayer[i].db移動開始時刻ms == INIT_TIME
			|| ( this->structLayer[i].pBMP    == NULL && this->structLayer[i].pBMPTEX == NULL ) 
			|| ( this->structLayer[i].pBMP    != NULL && ( ! this->structLayer[i].pBMP->bUse    || this->structLayer[i].pBMP->pSurface    == NULL ) )
			|| ( this->structLayer[i].pBMPTEX != NULL && ( ! this->structLayer[i].pBMPTEX->bUse || this->structLayer[i].pBMPTEX->pTexture == NULL ) ) )
			continue;

		// パラメータ抽出・算出
		SIZE	sz開始サイズ		= this->structLayer[i].sz開始サイズ;
		SIZE	sz終了サイズ		= this->structLayer[i].sz終了サイズ;
		POINT	pt画像側開始位置	= this->structLayer[i].pt画像側開始位置;
		POINT	pt画像側終了位置	= this->structLayer[i].pt画像側終了位置;
		POINT	pt表示側開始位置	= this->structLayer[i].pt表示側開始位置;
		POINT	pt表示側終了位置	= this->structLayer[i].pt表示側終了位置;
		double	db総移動時間ms		= this->structLayer[i].db総移動時間ms;
		double	db移動開始時刻ms	= this->structLayer[i].db移動開始時刻ms; if( g_Timer.Get() < db移動開始時刻ms ) db移動開始時刻ms = g_Timer.Get();
		SIZE	szBGAサイズ			= { BGA_W, BGA_H };
		SIZE	szBMPサイズ			= { (this->structLayer[i].pBMP) ? this->structLayer[i].pBMP->pSurface->dwWidth  : this->structLayer[i].pBMPTEX->pTexture->dwImageWidth,
										(this->structLayer[i].pBMP) ? this->structLayer[i].pBMP->pSurface->dwHeight : this->structLayer[i].pBMPTEX->pTexture->dwImageHeight };

		double	db現在時刻ms		= ( g_Timer.Get() - db移動開始時刻ms ) * g_DTX.fSpeed;	// 開始からの経過時間[ms]

		// 終了判定 (db総移動時刻ms==0 のときは終わりなし）
		if( db総移動時間ms != 0.0 && db総移動時間ms < db現在時刻ms )
		{
			this->structLayer[i].pt画像側開始位置 = pt画像側開始位置 = pt画像側終了位置;
			this->structLayer[i].pt表示側開始位置 = pt表示側開始位置 = pt表示側終了位置;
			this->structLayer[i].sz開始サイズ     = sz開始サイズ     = sz終了サイズ;
			this->structLayer[i].db総移動時間ms	  = db総移動時間ms   = 0.0;
			//db移動開始時刻nms	= this->structLayer[i].db移動開始時刻ms = INIT_TIME;		タイマを初期化したらレイヤ自体スキップされるからダメ
		}

		/////////////
		// 現在時刻から画像領域と表示領域を計算する

		RECT rc画像側領域;		// 画像の左上が 0, 0
		RECT rc表示側領域;		// BGA領域の左上が 0, 0

		if( db総移動時間ms == 0.0 )
		{
			// 開始位置・開始サイズをずっと保持
			rc画像側領域.left   = pt画像側開始位置.x;
			rc画像側領域.top    = pt画像側開始位置.y;
			rc画像側領域.right  = rc画像側領域.left + sz開始サイズ.cx;
			rc画像側領域.bottom = rc画像側領域.top  + sz開始サイズ.cy;
			
			rc表示側領域.left   = pt表示側開始位置.x;
			rc表示側領域.top    = pt表示側開始位置.y;
			rc表示側領域.right  = rc表示側領域.left + sz開始サイズ.cx;
			rc表示側領域.bottom = rc表示側領域.top  + sz開始サイズ.cy;
		}
		else
		{
			// db割合; 開始→終了 のとき 0.0 → 1.0
			double db割合 = db現在時刻ms / db総移動時間ms;
		
			// 現在のサイズ（画像・表示共通）
			SIZE sz現在のサイズ = {
				sz開始サイズ.cx + (LONG)( ( sz終了サイズ.cx - sz開始サイズ.cx ) * db割合 ),
				sz開始サイズ.cy + (LONG)( ( sz終了サイズ.cy - sz開始サイズ.cy ) * db割合 )
			};

			// 転送元領域
			rc画像側領域.left   = pt画像側開始位置.x + (LONG)( ( pt画像側終了位置.x - pt画像側開始位置.x ) * db割合 );
			rc画像側領域.top    = pt画像側開始位置.y + (LONG)( ( pt画像側終了位置.y - pt画像側開始位置.y ) * db割合 );
			rc画像側領域.right  = rc画像側領域.left + sz現在のサイズ.cx;
			rc画像側領域.bottom = rc画像側領域.top  + sz現在のサイズ.cy;

			// 転送先領域
			rc表示側領域.left   = pt表示側開始位置.x + (LONG)( ( pt表示側終了位置.x - pt表示側開始位置.x ) * db割合 );
			rc表示側領域.top    = pt表示側開始位置.y + (LONG)( ( pt表示側終了位置.y - pt表示側開始位置.y ) * db割合 );
			rc表示側領域.right  = rc表示側領域.left + sz現在のサイズ.cx;
			rc表示側領域.bottom = rc表示側領域.top  + sz現在のサイズ.cy;
		}

		// 転送部分がないならスキップ① クリッピング前
		if( rc画像側領域.right <= 0 || rc画像側領域.bottom <= 0 || rc画像側領域.left >= szBMPサイズ.cx || rc画像側領域.top >= szBMPサイズ.cy ) continue;
		if( rc表示側領域.right <= 0 || rc表示側領域.bottom <= 0 || rc表示側領域.left >= szBGAサイズ.cx || rc表示側領域.top >= szBGAサイズ.cy ) continue;

		// クリッピング① 転送元
		if( rc画像側領域.left < 0 )	{ rc表示側領域.left += -rc画像側領域.left; rc画像側領域.left = 0; }
		if( rc画像側領域.top  < 0 )	{ rc表示側領域.top  += -rc画像側領域.top;  rc画像側領域.top  = 0; }
		if( rc画像側領域.right  > szBMPサイズ.cx ) { rc表示側領域.right  -= rc画像側領域.right  - szBMPサイズ.cx; rc画像側領域.right  = szBMPサイズ.cx; }
		if( rc画像側領域.bottom > szBMPサイズ.cy ) { rc表示側領域.bottom -= rc画像側領域.bottom - szBMPサイズ.cy; rc画像側領域.bottom = szBMPサイズ.cy; }

		// クリッピング② 転送先
		if( rc表示側領域.left < 0 ) { rc画像側領域.left += -rc表示側領域.left; rc表示側領域.left = 0; }
		if( rc表示側領域.top  < 0 ) { rc画像側領域.top  += -rc表示側領域.top;  rc表示側領域.top  = 0; }
		if( rc表示側領域.right  > szBGAサイズ.cx ) { rc画像側領域.right  -= rc表示側領域.right  - szBGAサイズ.cx; rc表示側領域.right  = szBGAサイズ.cx; }
		if( rc表示側領域.bottom > szBGAサイズ.cy ) { rc画像側領域.bottom -= rc表示側領域.bottom - szBGAサイズ.cy; rc表示側領域.bottom = szBGAサイズ.cy; }

		// 転送部分がないならスキップ② クリッピング後
		if( rc画像側領域.left >= rc画像側領域.right || rc画像側領域.top >= rc画像側領域.bottom ) continue;
		if( rc表示側領域.left >= rc表示側領域.right || rc表示側領域.top >= rc表示側領域.bottom ) continue;
		if( rc画像側領域.right < 0 || rc画像側領域.bottom < 0 || rc画像側領域.left > szBMPサイズ.cx || rc画像側領域.top > szBMPサイズ.cy ) continue;
		if( rc表示側領域.right < 0 || rc表示側領域.bottom < 0 || rc表示側領域.left > szBGAサイズ.cx || rc表示側領域.top > szBGAサイズ.cy ) continue;

		// ※センタリングは、BGAPAN では意味ない（ずれる）ので廃止(2003/03/02)

		// 描画
		if( this->structLayer[i].pBMP )
		{
			this->structLayer[i].pBMP->pSurface->ColorKeyEnable( (i == 0) ? false : (g_DTX.bBlackColorKey ? true : false) );	// 最下層レイヤは透過しない
			this->structLayer[i].pBMP->pSurface->Draw( x + rc表示側領域.left, y + rc表示側領域.top, &rc画像側領域 );
		}
		else if( this->structLayer[i].pBMPTEX )
		{
			this->structLayer[i].pBMPTEX->pTexture->Draw( x + rc表示側領域.left, y + rc表示側領域.top, &rc画像側領域 );
		}
	}
	return 0;
}

		}//Play
	}//Stage
}//DTXViewer
