#include "stdafx.h"
#include "ActPlayAVI.h"
#include "CSurface.h"
#include "DTX.h"
#include "CStage.h"
#include "DTXViewer.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

ActPlayAVI::ActPlayAVI()
{
	this->bFirstWork = true;
	this->bActivate = false;
}

bool ActPlayAVI::MyOneTimeSceneInit()
{
	// ここにコードを記述する。AddChild() など。
	AddChild( &this->sfAVI );

	return Activity::MyOneTimeSceneInit();
}
bool ActPlayAVI::MyFinalCleanup()
{
	// ここにコードを記述する。

	return Activity::MyFinalCleanup();
}
bool ActPlayAVI::MyInitDeviceObjects()
{
	// ここにコードを記述する。CTexture/CSurface::InitDeviceObjects() など。
	this->sfAVI.InitDeviceObjects( _T("AVI"), BGA_W, BGA_H );

	return Activity::MyInitDeviceObjects();
}
bool ActPlayAVI::MyDeleteDeviceObjects()
{
	// ここにコードを記述する。

	// 次の Activity::MyDeleteDeviceObjects() では、子テクスチャ・子サーフェイスの Delete (CTexture/CSurface::Delete) が行われる。
	return Activity::MyDeleteDeviceObjects();
}
bool ActPlayAVI::MyActivate()
{
	this->bFirstWork = true;
	// ここにコードを記述する。
	for( int i = 0; i < MAX_AVI_LAYER; i++ )
	{
		this->structLayer[i].pAVI						= NULL;
		this->structLayer[i].db移動開始時刻ms			=INIT_TIME;
		this->structLayer[i].dw前回表示したフレーム番号	= 0;
	}

	// 次の Activity::MyActivate() では、this->bActivity のセットならびに子サーフェイス・子テクスチャの Restore (MyRestoreDeviceTexture/Surface) が行われる。
	// なお、子 Activity の活性化は行われない。（このメソッドを抜けてから行われる。）
	return Activity::MyActivate();
}
bool ActPlayAVI::MyUnActivate()
{
	// ここにコードを記述する。

	// 次の Activity::MyUnActivate() では、this->bActivity のリセットならびに子サーフェイス・子テクスチャの Invalidate (MyInvalidateDeviceObjects) が行われる。
	// なお、子Activity の非活性化は行われない。（このメソッドを抜けてから行われる。）
	return Activity::MyUnActivate();
}
bool ActPlayAVI::MyRestoreDeviceTextures()
{
	// ここにコードを記述する。

	// 次の Activity::MyRestoreDeviceTextures() では、子テクスチャの Restore (CTexture::Restore) が行われる。
	return Activity::MyRestoreDeviceTextures();
}
bool ActPlayAVI::MyRestoreDeviceSurfaces()
{
	// ここにコードを記述する。

	// 次の Activity::MyRestoreDeviceSurfaces() では、子サーフェイスの Restore (CSurface::Restore) が行われる。
	return Activity::MyRestoreDeviceSurfaces();
}
bool ActPlayAVI::MyInvalidateDeviceObjects()
{
	// ここにコードを記述する。

	// 次の Activity::MyInvalidateDeviceObjects() では、子テクスチャ・子サーフェイスの Invalidate (CTexture/CSurface::Invalidate) が行われる。
	return Activity::MyRestoreDeviceSurfaces();
}
//
void ActPlayAVI::Start( int nチャンネル番号, DTX::AVI* pAVI, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, double db総移動時間ms, double db移動開始時刻ms )
{
	// チャンネルチェック
	if( nチャンネル番号 != 0x54 )
		return;

	// セルに値を設定
	int i = 0;
	this->structLayer[i].pAVI	= pAVI;
	this->structLayer[i].n開始サイズW	= n開始サイズW;
	this->structLayer[i].n開始サイズH	= n開始サイズH;
	this->structLayer[i].n画像側開始位置X	= n画像側開始位置X;
	this->structLayer[i].n画像側開始位置Y	= n画像側開始位置Y;
	this->structLayer[i].n表示側開始位置X	= n表示側開始位置X;
	this->structLayer[i].n表示側開始位置Y	= n表示側開始位置Y;
	this->structLayer[i].db総移動時間ms		= db総移動時間ms;
	this->structLayer[i].db移動開始時刻ms	= ( db移動開始時刻ms != INIT_TIME ) ? db移動開始時刻ms : g_Timer.Get();
	this->structLayer[i].dw前回表示したフレーム番号 = 0;
}
void ActPlayAVI::SkipStart( double db移動開始時刻ms )
{
	for( DTX::Chip* cell = g_DTX.pChip; cell != NULL; cell = cell->next )
	{
		// 開始時刻より後ろなら、そのチップで調査打ち切り。
		if( cell->dbTime > db移動開始時刻ms ) 
			break;

		// AVI, AVIPAN なら再生指示
		switch( cell->AVItype )
		{

		case AVITYPE_AVI:
			if( cell->pAVI )
				this->Start(
					cell->nChannel, cell->pAVI,
					BGA_W, BGA_H, 0, 0,			// sw, sh, ew, eh		; 領域   初期サイズ、終了サイズ
					0, 0, 0, 0,					// ssx, ssy, sex, sey	; 画像側 初期位置、最終位置
					0, 0, 0, 0,					// dsx, dsy, dex, dey	; 表示側 初期位置、最終位置
					0, cell->dbTime );			// len, start
			break;

		case AVITYPE_AVIPAN:
			if( cell->pAVIPan )
				this->Start(
					cell->nChannel, cell->pAVI,
					cell->pAVIPan->sw, cell->pAVIPan->sh, cell->pAVIPan->ew, cell->pAVIPan->eh,		// 領域   初期サイズ、終了サイズ
					cell->pAVIPan->ssx, cell->pAVIPan->ssy, cell->pAVIPan->sex, cell->pAVIPan->sey,	// 画像側 初期位置、最終位置
					cell->pAVIPan->dsx, cell->pAVIPan->dsy, cell->pAVIPan->dex, cell->pAVIPan->dey,	// 表示側 初期位置、最終位置
					cell->dbLong, cell->dbTime );													// len, start
			break;
		}
	}
}
void ActPlayAVI::Stop()
{
	for( int i = 0; i < MAX_AVI_LAYER; i++ ) {
		if( this->structLayer[i].pAVI && this->structLayer[i].pAVI->bUse )
			this->structLayer[i].db移動開始時刻ms = INIT_TIME;
	}
}
void ActPlayAVI::Cont( double db再開時刻 )
{
	for( int i = 0; i < MAX_AVI_LAYER; i++ ) {
		if( this->structLayer[i].pAVI && this->structLayer[i].pAVI->bUse )
			this->structLayer[i].db移動開始時刻ms = db再開時刻;
	}
}
int  ActPlayAVI::MainWork( int x, int y )
{
	// AVI 指定なしなら何もしない
	if( ! g_DTX.bUseAVI )
		return 0;

	// 進行＆描画
	for( int i = 0; i < MAX_AVI_LAYER; i++ )
	{
		// 未使用ならスキップ
		if( this->structLayer[i].db移動開始時刻ms == INIT_TIME )
			continue;
		if( ! this->structLayer[i].pAVI || ! this->structLayer[i].pAVI->bUse )
			continue;

		// パラメータ抽出・算出
		DTX::AVI*	 pAVI		= this->structLayer[i].pAVI;
		SIZE szAVIサイズ;		pAVI->avi._フレームサイズを取得する( &(szAVIサイズ.cx), &(szAVIサイズ.cy) );
		SIZE szBGAサイズ		= { BGA_W, BGA_H };
		SIZE sz開始サイズ		= { this->structLayer[i].n開始サイズW, this->structLayer[i].n開始サイズH };
		SIZE sz終了サイズ		= { this->structLayer[i].n終了サイズW, this->structLayer[i].n終了サイズH };
		POINT pt画像側開始位置	= { this->structLayer[i].n画像側開始位置X, this->structLayer[i].n画像側終了位置Y };
		POINT pt画像側終了位置	= { this->structLayer[i].n画像側終了位置X, this->structLayer[i].n画像側終了位置Y };
		POINT pt表示側開始位置	= { this->structLayer[i].n表示側開始位置X, this->structLayer[i].n表示側開始位置Y };
		POINT pt表示側終了位置	= { this->structLayer[i].n表示側終了位置X, this->structLayer[i].n表示側終了位置Y };
		double db総移動時間ms	= this->structLayer[i].db総移動時間ms;
		double db移動開始時刻ms	= this->structLayer[i].db移動開始時刻ms; if( g_Timer.Get() < db移動開始時刻ms ) db移動開始時刻ms = g_Timer.Get();

		double db現在時刻ms		= ( g_Timer.Get() - db移動開始時刻ms ) * g_DTX.fSpeed;	// 開始からの経過時間[ms]
		DWORD  dwフレーム番号	= pAVI->avi._時刻からフレーム番号を返す( db現在時刻ms );

		// 終了判定（db総移動時間ms==0 のときはAVI再生終了まで）
		if( db総移動時間ms != 0.0 && db総移動時間ms < db現在時刻ms )
		{
			this->structLayer[i].db総移動時間ms   = 0;
			this->structLayer[i].db移動開始時刻ms = INIT_TIME;	// 再生終了
			return 0;
		}

		// 現在座標の計算；以下の２変数を算出する。
		RECT rc画像側領域;		// 画像の左上が 0, 0
		RECT rc表示側領域;		// BGA領域の左上が 0, 0

		if( db総移動時間ms == 0.0 )
		{
			// 初期位置・初期サイズをずっと保持
			SetRect( &rc画像側領域,
				pt画像側開始位置.x,
				pt画像側開始位置.y,
				pt画像側開始位置.x + sz開始サイズ.cx,
				pt画像側開始位置.y + sz開始サイズ.cy );
			
			SetRect( &rc表示側領域,
				pt表示側開始位置.x,
				pt表示側開始位置.y,
				pt表示側開始位置.x + sz開始サイズ.cx,
				pt表示側開始位置.y + sz開始サイズ.cy );
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
			SetRect( &rc画像側領域,
				(LONG)( ( pt画像側終了位置.x - pt画像側開始位置.x ) * db割合 ),
				(LONG)( ( pt画像側終了位置.y - pt画像側開始位置.y ) * db割合 ),
				(LONG)( ( pt画像側終了位置.x - pt画像側開始位置.x ) * db割合 ) + sz現在のサイズ.cx,
				(LONG)( ( pt画像側終了位置.y - pt画像側開始位置.y ) * db割合 ) + sz現在のサイズ.cy );

			// 転送先領域
			SetRect( &rc表示側領域,
				(LONG)( ( pt表示側終了位置.x - pt表示側開始位置.x ) * db割合 ),
				(LONG)( ( pt表示側終了位置.y - pt表示側開始位置.y ) * db割合 ),
				(LONG)( ( pt表示側終了位置.x - pt表示側開始位置.x ) * db割合 ) + sz現在のサイズ.cx,
				(LONG)( ( pt表示側終了位置.y - pt表示側開始位置.y ) * db割合 ) + sz現在のサイズ.cy );

			// 転送部分がないならスキップ① クリッピング前
			if( rc画像側領域.right <= 0 || rc画像側領域.bottom <= 0 || rc画像側領域.left >= szAVIサイズ.cx || rc画像側領域.top >= szAVIサイズ.cy ) continue;
			if( rc表示側領域.right <= 0 || rc表示側領域.bottom <= 0 || rc表示側領域.left >= szBGAサイズ.cx || rc表示側領域.top >= szBGAサイズ.cy ) continue;

			// クリッピング① 転送元
			if( rc画像側領域.left < 0 )	{ rc表示側領域.left += -rc画像側領域.left; rc画像側領域.left = 0; }
			if( rc画像側領域.top  < 0 )	{ rc表示側領域.top  += -rc画像側領域.top;  rc画像側領域.top  = 0; }
			if( rc画像側領域.right  > szAVIサイズ.cx ) { rc表示側領域.right  -= rc画像側領域.right  - szAVIサイズ.cx; rc画像側領域.right  = szAVIサイズ.cx; }
			if( rc画像側領域.bottom > szAVIサイズ.cy ) { rc表示側領域.bottom -= rc画像側領域.bottom - szAVIサイズ.cy; rc画像側領域.bottom = szAVIサイズ.cy; }

			// クリッピング② 転送先
			if( rc表示側領域.left < 0 ) { rc画像側領域.left += -rc表示側領域.left; rc表示側領域.left = 0; }
			if( rc表示側領域.top  < 0 ) { rc画像側領域.top  += -rc表示側領域.top;  rc表示側領域.top  = 0; }
			if( rc表示側領域.right  > szBGAサイズ.cx ) { rc画像側領域.right  -= rc表示側領域.right  - szBGAサイズ.cx; rc表示側領域.right  = szBGAサイズ.cx; }
			if( rc表示側領域.bottom > szBGAサイズ.cy ) { rc画像側領域.bottom -= rc表示側領域.bottom - szBGAサイズ.cy; rc表示側領域.bottom = szBGAサイズ.cy; }

			// 転送部分がないならスキップ② クリッピング後
			if( rc画像側領域.left >= rc画像側領域.right || rc画像側領域.top >= rc画像側領域.bottom ) continue;
			if( rc表示側領域.left >= rc表示側領域.right || rc表示側領域.top >= rc表示側領域.bottom ) continue;
			if( rc画像側領域.right < 0 || rc画像側領域.bottom < 0 || rc画像側領域.left > szAVIサイズ.cx || rc画像側領域.top > szAVIサイズ.cy ) continue;
			if( rc表示側領域.right < 0 || rc表示側領域.bottom < 0 || rc表示側領域.left > szBGAサイズ.cx || rc表示側領域.top > szBGAサイズ.cy ) continue;
		}
	
		// 描画
		this->structLayer[i].dw前回表示したフレーム番号 = dwフレーム番号;

		HDC hdc;
		int ret = 0;
		
		this->sfAVI.Clear();
		
		if( SUCCEEDED( this->sfAVI.GetDC( &hdc ) ) )
		{
			ret = pAVI->avi._指定時刻のフレームをDCへ描画する( g_App.hWnd, hdc, db現在時刻ms, 
				rc画像側領域.left, rc画像側領域.top, rc画像側領域.right - rc画像側領域.left, rc画像側領域.bottom - rc画像側領域.top,
				rc表示側領域.left, rc表示側領域.top, rc表示側領域.right - rc表示側領域.left, rc表示側領域.bottom - rc表示側領域.top );
			this->sfAVI.ReleaseDC( hdc );
			this->sfAVI.Draw( x, y );
		}
		if( ret != 0 )
		{
			// 再生完了
			this->structLayer[i].db移動開始時刻ms = INIT_TIME;
		}
	}
	return 0;
}

		}//Play
	}//Stage
}//DTXViewer
