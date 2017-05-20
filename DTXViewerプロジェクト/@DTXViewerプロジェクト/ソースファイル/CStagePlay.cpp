#include "stdafx.h"
// FDK includes
#include "Debug.h"
#include "DTX.h"
// App includes
#include "../resource.h"
#include "DTXViewer.h"
#include "CStage.h"
#include "CStagePlay.h"
#include "ActPlayAVI.h"
#include "ActPlayBGA.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

static RECT s_rcChips[28] = {
	{ 640,2,651,5 }, { 651,2,662,5 },												// 0:BPM, 1:BPx
	{ 662,2,673,5 }, { 673,2,684,5 }, { 684,2,695,5 }, { 695,2,706,3 },				// 2:HH, 3:SD, 4:BD, 5:HT
	{ 706,2,717,5 }, { 717,2,728,5 }, { 728,2,739,5 },								// 6:LT, 7:FT, 8;CY
	{ 739,2,746,5 },																// 9:FI
	{ 746,2,757,5 }, { 757,2,768,5 }, { 768,2,779,5 },								// 10:BGM, 11:BGA, 12:AVI
	{ 779,2,790,5 }, { 790,2,801,5 }, { 801,2,812,5 }, { 812,2,819,3 },				// 13:R1, 14:G1, 15:B1. 16:W1
	{ 819,2,830,5 }, { 830,2,841,5 }, { 841,2,852,5 }, { 852,2,859,3 },				// 17:R1, 18:G1, 19:B1. 20:W1
	{ 859,2,870,5 }, { 870,2,881,5 }, { 881,2,892,5 }, { 892,2,903,3 }, { 903,2,914,5 },	// 21～25: SE1～5
	{ 879,2,914,5 },																// 26: GuitarOPEN
	{ 914,2,925,5 }																	// 27: LC
};
static RECT s_rcRing  = { 640, 5, 640+12, 5+12 };									// HHO,RD の輪
static RECT s_rcLine  = { 640, 0, 640+244, 1 };										// 小節線
static RECT s_rcSubLine  = { 640, 1, 640+244, 2 };									// 拍線
static RECT s_rcNums[10] = {
	{   0,   0,  18,  31 },	{  18,   0,  36,  31 },	{  36,   0,  54,  31 },	{  54,   0,  72,  31 },	{  72,   0,  90,  31 },
	{   0,  31,  18,  62 },	{  18,  31,  36,  62 },	{  36,  31,  54,  62 },	{  54,  31,  72,  62 },	{  72,  31,  90,  62 }
};
static const int s_nチャンネル0Atoレーン07[10] = { 1,2,3,4,5,7,6,1,7,0 };

CStagePlay::CStagePlay()
{
	this->strステージ名 = _T("演奏画面");
	this->stageID = STAGE_PLAY;
	this->phaseID = PHASE_COMMON_通常状態;
	this->bFirstWork = true;
	this->bActivate = false;
}
//
bool CStagePlay::Load( LPCTSTR strFileName )
{
	this->bLoading要請あり = true;
	this->strFileName = strFileName;

	return true;
}
void CStagePlay::Play( int nPart )
{
	this->bPlaying要請あり = true;
	this->n開始小節番号 = nPart;
	if( this->n開始小節番号 < 0 ) this->n開始小節番号 = 0;
}
void CStagePlay::Stop()
{
	this->struct演奏情報.b演奏中 = false;

	g_DTX.StopAllWaves();
	this->actAVI.Stop();			// AVI 停止
	this->actBGA.Stop();			// BGA 停止
}
void CStagePlay::SkipStart( double db開始時刻ms )
{
	DTX::Chip* pChip;
	for( pChip = g_DTX.pChip; pChip != NULL; pChip = pChip->next )
	{
		if( pChip->dbTime >= db開始時刻ms )
			break;

		this->ProcessCell( pChip, true );
	}
	this->struct演奏情報.p現在のトップChip = pChip;
}
//
int	 CStagePlay::MainWork()
{
	// 戻り値: 0=継続, 1=演奏終了, -1=アプリ終了

	// 背景描画
	RECT rc = { 0, 0, 640, 480 };
	this->sfMap.Draw( 0, 0, &rc );

	if( this->bLoading要請あり )
	{
		this->bLoading要請あり = false;
		this->ctNowLoading表示.Start( 0, 50, 1, &g_Timer );
		this->phaseID = PHASE_PLAY_LOADING1;
	}

	switch( this->phaseID )
	{
	case PHASE_PLAY_IDLE:
		break;

	case PHASE_PLAY_LOADING1: {
		RECT rcNowLoading = { 640, 17, 640+229, 17+40 };
		this->sfMap.Draw( 176-225/2, 180-41/2, &rcNowLoading );

		this->ctNowLoading表示.Step();
		if( this->ctNowLoading表示.b終了値に達した() )
			this->phaseID = PHASE_PLAY_LOADING2;
		break;
	}
	case PHASE_PLAY_LOADING2:
		if( this->tDTXファイルの読み込み() )
			this->phaseID = PHASE_PLAY_PLAYING1;
		else {
			this->phaseID = PHASE_PLAY_IDLE;
			this->bPlaying要請あり = false;
		}
		break;

	case PHASE_PLAY_PLAYING1:
		if( this->bPlaying要請あり ) {
			this->t再生開始();
			this->phaseID = PHASE_PLAY_PLAYING2;
		}
		break;

	case PHASE_PLAY_PLAYING2:
		if( this->struct演奏情報.b演奏中 )
		{
			// BGA領域をクリア
			{
				if( g_DTX.bUseAVI || g_DTX.bUseBMP || g_DTX.bUseBMPTEX )
				{
					DDBLTFX ddbfx;
					ZeroMemory( &ddbfx, sizeof( DDBLTFX ) );
					ddbfx.dwSize = sizeof( DDBLTFX );
					ddbfx.dwFillColor = 0xFF000000;
					RECT rc = { BGA_X, BGA_Y, BGA_X+BGA_W, BGA_Y+BGA_H };
					g_App.pddsBackBuffer->Blt( &rc, NULL, NULL, DDBLT_COLORFILL | DDBLT_WAIT, &ddbfx );
				}
			}
			//
			// チップ進行描画
			{
				if( this->struct演奏情報.p現在のトップChip == NULL ) {
					this->Stop();
					this->phaseID = PHASE_PLAY_IDLE;
					return 1;	// 演奏終了
				}

				// スクロール速度[dot/ms] 算出
				static const double BPM = 150.0;			// BPM = １小節の長さが、150 BPM で
				static const double LEN = 264.0;			// LEN = 264 ドット
				static const double s_dbScrollSpeed = 2.0;	// 譜面スクロール速度（固定）
				double dbDotPerMS_D = ( s_dbScrollSpeed * 0.5       ) * ( BPM / 4.0 ) * LEN / 60000.0;
				double dbDotPerMS_G = ( s_dbScrollSpeed * 0.5 * 0.5 ) * ( BPM / 4.0 ) * LEN / 60000.0;
				double dbDotPerMS_B = ( s_dbScrollSpeed * 0.5 * 0.5 ) * ( BPM / 4.0 ) * LEN / 60000.0;

				// トップセルから順番に、画面に入るだけ描画。
				for( DTX::Chip* cell = this->struct演奏情報.p現在のトップChip; cell != NULL; cell = cell->next )
				{
					// バーからの距離[dot] 算出
					cell->nDotFromBarD = (long)( (cell->dbTime - g_Timer.Get() ) * dbDotPerMS_D );	// ドラム関係、その他
					cell->nDotFromBarG = (long)( (cell->dbTime - g_Timer.Get() ) * dbDotPerMS_G );	// ギター関係
					cell->nDotFromBarB = (long)( (cell->dbTime - g_Timer.Get() ) * dbDotPerMS_B );	// ベース関係
				
					if( MIN( MIN( cell->nDotFromBarD, cell->nDotFromBarG ), cell->nDotFromBarB ) > 480 )
						break;	// ドラム、ギター、ベースのうち、一番短い距離が画面幅より大きいならそこで終了。
				
					// トップセル入れ替えチェック；
					// 画面下端より下ならトップセル入れ替え（ヒットして無いならまだ入れ替えない）。
					// あるいは、ヒットしてなくても、-400を超えたら強制的に入れ替える（予防策）
					if( cell == this->struct演奏情報.p現在のトップChip )
					{
						if( ( cell->nDotFromBarD < -65 && cell->bHit ) || cell->nDotFromBarD < -400 )		// nDotFromBar はドラムでもギターでもいい
						{
							this->struct演奏情報.p現在のトップChip = this->struct演奏情報.p現在のトップChip->next;
							continue;
						}
					}

					// チャンネルごとに処理
					this->ProcessCell( cell, false );
				}
			}
			//
			// AVI 進行描画
			{
				if( g_DTX.bUseAVI )
					this->actAVI.MainWork( BGA_X, BGA_Y );
			}
			//
			// BGA 進行描画
			{
				if( g_DTX.bUseBMP || g_DTX.bUseBMPTEX )
					this->actBGA.MainWork( BGA_X, BGA_Y );
			}
		}
		break;
	}

	if( g_DirectInput.IsKeyPushDown( DIK_ESCAPE ) )
		return -1;		// アプリ終了

	return 0;
}
bool CStagePlay::tDTXファイルの読み込み()
{
	this->Stop();

	// 演奏情報を初期化する
	{
		this->t演奏情報を初期化する();
		g_DTX.Clear();
	}
	//
	// DTXの読み込み
	{
		if( ! g_DTX.Load( this->strFileName ) )
		{
			Debug::Out( _T("DTXファイルの読み込みに失敗しました。(%s)\n"), this->strFileName.c_str() );
			return false;
		}
		g_DTX.LoadBMP();
		g_DTX.LoadAVI();
	}
	//
	// 再生速度の設定
	{
		g_DTX.SetSpeed( g_DTX.fSpeed );	// #DTXVPLAYSPEED: 
	}
	//
	// ウィンドウタイトルを「曲名 [作者]」に変更
	{
		TCHAR buf[ 1024 ];
		_stprintf_s( buf, 1024, _T("%s [%s]"), g_DTX.strTitle.c_str(), g_DTX.strArtist.c_str() );
		::SetWindowText( g_App.hWnd, buf );
	}
	//
	return true;
}
void CStagePlay::t再生開始()
{
	this->Stop();

	// 小節番号 nPart から再生開始時刻と再生開始チップを算出;
	// 再生開始時刻は小節線の dbTime から取得。（分解能の倍数の位置には必ずオブジェクト（小節線）があるという前提）
	{
		this->struct演奏情報.db開始時刻ms = INIT_TIME;
		this->struct演奏情報.p現在のトップChip = NULL;

		for( DTX::Chip* cell = g_DTX.pChip; cell != NULL; cell = cell->next )
		{
			if( cell->dwPosition >= (DWORD)( (this->n開始小節番号+1) * DTX_RESOLVE ) )
			{
				this->struct演奏情報.db開始時刻ms = cell->dbTime;
				break;
			}
		}
	}
	//
	// 演奏開始
	{
		if( this->struct演奏情報.db開始時刻ms != INIT_TIME )
		{
			Debug::Out( _T("演奏開始時刻 = %e [ms]\n"), this->struct演奏情報.db開始時刻ms );

			// タイマリセット＆描画開始
			g_Timer.Reset();
			g_Timer.Set( this->struct演奏情報.db開始時刻ms );				// 再生開始時刻セット
			g_DTX.SkipStart( this->struct演奏情報.db開始時刻ms );			// 全チップについて dwStartTime に再生中か否か調査し、再生中なら適切な個所に各再生カーソルを移動して再生
			this->actAVI.SkipStart( this->struct演奏情報.db開始時刻ms );	// AVI の演奏開始
			this->actBGA.SkipStart( this->struct演奏情報.db開始時刻ms );	// BGA の演奏開始
			this->SkipStart( this->struct演奏情報.db開始時刻ms );			// チップの演奏開始(actBGAより後であること; ChangeScopeのため)
			this->struct演奏情報.b演奏中 = true;

			// まだ Loading 前の可能性が高いので、ここでは this->phaseID はいじらない。
		}
	}
}
void CStagePlay::ProcessCell( DTX::Chip* pChip, bool bSkip )
{
	switch( pChip->nChannel )
	{
	// バックコーラス
	case 0x01: {

		// チップ描画
		if( ! pChip->bHit && ! bSkip )
			this->sfMap.Draw( 146, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[10] );

		// 発声してなければ発声する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;
			if( ! bSkip )
				g_DTX.PlayChip( pChip, 10, DTX_AUTOVOLUME );
		}
		break;
	}
	// BPM変更
	case 0x03: {
		
		// 表示
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 2, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[0] );

		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			// 表示用 BPM の更新
			this->struct演奏情報.db現在のBPM・デバッグ表示用 = pChip->nParam * g_DTX.fSpeed + g_DTX.dbBaseBPM;
			pChip->bHit = true;
		}
		break;
	}
	// BPM変更（拡張）
	case 0x08: {

		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 14, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[1] );

		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;
			
			for( DTX::BPM* bc = g_DTX.pBPMLast; bc != NULL; bc = bc->prev )
			{
				if( bc->num == pChip->nRealParam )
				{
					// 表示用 BPM の更新
					this->struct演奏情報.db現在のBPM・デバッグ表示用 = bc->bpm * g_DTX.fSpeed + g_DTX.dbBaseBPM;
					break;
				}
			}
		}
		break;
	}
	// BGA
	case 0x04: case 0x07: case 0x55: case 0x56: case 0x57: case 0x58: case 0x59: case 0x60: {
	
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 158, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[11] );

		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;
			if( bSkip ) break;

			switch( pChip->BGAtype )
			{
			case BGATYPE_BMP: {
				
				if( pChip->pBMP ) 
				{
					// len = 0 なら転送元情報のみ有効。（転送先情報は 0 でいい。）
					this->actBGA.Start( pChip->nChannel, pChip->pBMP, NULL,
						pChip->pBMP->pSurface->dwWidth, pChip->pBMP->pSurface->dwHeight, 0, 0,
						0, 0, 0, 0,
						0, 0, 0, 0,
						0.0 );
				}
				break;
			}
			case BGATYPE_BMPTEX: {
				
				if( pChip->pBMPTEX )
				{
					// len = 0 なら転送元情報のみ有効。（転送先情報は 0 でいい。）
					this->actBGA.Start( pChip->nChannel, NULL, pChip->pBMPTEX,
						pChip->pBMPTEX->pTexture->dwWidth, pChip->pBMPTEX->pTexture->dwHeight, 0, 0,
						0, 0, 0, 0,
						0, 0, 0, 0,
						0.0 );
				}
				break;
			}
			case BGATYPE_BGA: {
				
				if( pChip->pBGA && ( pChip->pBMP || pChip->pBMPTEX ) )
				{
					// len = 0 なら転送元情報のみ有効。（転送先情報は 0 でいい。）
					this->actBGA.Start( pChip->nChannel, pChip->pBMP, pChip->pBMPTEX,
						pChip->pBGA->x2 - pChip->pBGA->x1, pChip->pBGA->y2 - pChip->pBGA->y1, 0, 0, 
						pChip->pBGA->x1, pChip->pBGA->y1, 0, 0,
						pChip->pBGA->ox, pChip->pBGA->oy, 0, 0,
						0.0 );
				}
				break;
			}
			case BGATYPE_BGAPAN: {
				
				if( pChip->pBGAPan && ( pChip->pBMP || pChip->pBMPTEX ) )
				{
					this->actBGA.Start( pChip->nChannel, pChip->pBMP, pChip->pBMPTEX,
						pChip->pBGAPan->sw, pChip->pBGAPan->sh, pChip->pBGAPan->ew, pChip->pBGAPan->eh,
						pChip->pBGAPan->ssx, pChip->pBGAPan->ssy, pChip->pBGAPan->sex, pChip->pBGAPan->sey,
						pChip->pBGAPan->dsx, pChip->pBGAPan->dsy, pChip->pBGAPan->dex, pChip->pBGAPan->dey,
						pChip->dbLong );
                }
				break;
			}
			}
		}
		break;
	}
	// BGAスコープ画像切り替え
	case 0xC4: case 0xC7: case 0xD5: case 0xD6: case 0xD7: case 0xD8: case 0xD9: case 0xE0: {

		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 158, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[11] );

		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;

			if( pChip->BGAtype == BGATYPE_BMP || pChip->BGAtype == BGATYPE_BMPTEX )
			{
				static const int nCh[2][8] = {
					{ 0xC4, 0xC7, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xE0 },
					{ 0x04, 0x07, 0x55, 0x56, 0x57, 0x58, 0x59, 0x60 }
				};
				for( int i = 0; i < 8; i++)
					if( nCh[0][i] == pChip->nChannel )
						this->actBGA.ChangeScope( nCh[1][i], pChip->pBMP, pChip->pBMPTEX );
			}
		}
		break;
	}
	// ドラムチップ
	case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17: case 0x18: case 0x19: case 0x1A: {

		int l = s_nチャンネル0Atoレーン07[ pChip->nChannel - 0x11 ];

		// 発声してなければ発声する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) ) {
			if( ! bSkip )
				this->tサウンド再生( pChip, PLAYPART_DRUMS, DTX_AUTOVOLUME );
			pChip->bHit   = true;
		}

		// チップの描画
		if( ! pChip->bHit && ! bSkip && pChip->bVisible )
		{
			// チップ描画
			this->sfMap.Draw( 33+12*l, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[ (pChip->nChannel!=0x1A)? 1+l : 27 ] );

			// HHO,RDはリングも描画
			if( pChip->nChannel == 0x18 || pChip->nChannel == 0x19 )
				this->sfMap.Draw( 33+12*l, BAR_Y-pChip->nDotFromBarD-5, &s_rcRing );
		}
		break;
	}
	// ドラムチップ（不可視）
	case 0x31: case 0x32: case 0x33: case 0x34: case 0x35: case 0x36: case 0x37: case 0x38: case 0x39: case 0x3A: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// ドラムチップ（空うち指定）
	case 0xB1: case 0xB2: case 0xB3: case 0xB4: case 0xB5: case 0xB6: case 0xB7: case 0xB8: case 0xB9: case 0xBC: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// ギターチップ
	case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25: case 0x26: case 0x27: {

		// 発声してなければ発声する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarG < 0 ) )
		{
			if( ! bSkip )
				this->tサウンド再生( pChip, PLAYPART_GUITAR, DTX_AUTOVOLUME );
			pChip->bHit = true;
		}

		// チップの描画
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
		{
			int y = 38 + pChip->nDotFromBarG-1;
			if( y >= 0 && y < 480 )
			{
				if( pChip->nChannel == 0x20 )	// OPEN
					this->sfMap.Draw( 188, y, &s_rcChips[26] );
				else
				{								// RGB
					if( pChip->nChannel & 0x04 ) this->sfMap.Draw( 188,      y, &s_rcChips[13] );
					if( pChip->nChannel & 0x02 ) this->sfMap.Draw( 188+12,   y, &s_rcChips[14] );
					if( pChip->nChannel & 0x01 ) this->sfMap.Draw( 188+12*2, y, &s_rcChips[15] );
				}
			}
		}
		break;
	}		   
	// ギターチップ（空うち指定）
	case 0xBA: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarG < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// ベースチップ
	case 0xA0: case 0xA1: case 0xA2: case 0xA3: case 0xA4: case 0xA5: case 0xA6: case 0xA7: {

		// 発声してなければ発声する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarB < 0 ) )
		{
			if( ! bSkip )
				this->tサウンド再生( pChip, PLAYPART_BASS, DTX_AUTOVOLUME );
			pChip->bHit = true;
		}

		// チップの描画
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
		{
			int y = 38 + pChip->nDotFromBarB-1;
			if( y >= 0 && y < 480 )
			{
				if( pChip->nChannel == 0xA0 )	// OPEN
					this->sfMap.Draw( 234, y, &s_rcChips[26] );
				else
				{								// RGB
					if( pChip->nChannel & 0x04 ) this->sfMap.Draw( 234,      y, &s_rcChips[17] );
					if( pChip->nChannel & 0x02 ) this->sfMap.Draw( 234+12,   y, &s_rcChips[18] );
					if( pChip->nChannel & 0x01 ) this->sfMap.Draw( 234+12*2, y, &s_rcChips[19] );
				}
			}
		}
		break;
	}
	// ベースチップ（空うち指定）
	case 0xBB: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarB < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// ギターWiling
	case 0x28: {
		// ヒットしていなければヒット処理する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarG < 0 ) )
			pChip->bHit = true;

		// チップ描画
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 224, 38+pChip->nDotFromBarG, &s_rcChips[16] );
		break;
	}
	// ベースWiling
	case 0xA8: {
		// ヒットしていなければヒット処理する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarB < 0 ) )
			pChip->bHit = true;

		// チップ描画
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
			this->sfMap.Draw( 270, 38+pChip->nDotFromBarB, &s_rcChips[20] );
		break;
	}
	// BGM
	case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67: case 0x68: case 0x69: case 0x70:
	case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x76: case 0x77: case 0x78: case 0x79: case 0x80:
	case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87: case 0x88: case 0x89: case 0x90:
	case 0x91: case 0x92: {

		// チップ描画（0x61～0x65までだけ）
		if( ! bSkip && ! pChip->bHit && pChip->bVisible && pChip->nChannel >= 0x61 && pChip->nChannel <= 0x65 )
			this->sfMap.Draw( 290+(pChip->nChannel-0x61)*12, BAR_Y-pChip->nDotFromBarD-1, &s_rcChips[21+pChip->nChannel-0x61] );

		// 発声してなければ発声する
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			if( ! bSkip ) {
				g_DTX.StopWave( this->struct演奏情報.n最後に再生したBGMの実WAV番号[ pChip->nChannel-0x61 ], 10 );
				g_DTX.PlayChip( pChip, 10, DTX_AUTOVOLUME );
				this->struct演奏情報.n最後に再生したBGMの実WAV番号[ pChip->nChannel-0x61 ] = pChip->nRealParam;
			}
			pChip->bHit = true;
		}
		break;
	}
	// 小節線
	case 0x50: {
			
		// part = 小節番号(表示する番号+1)
		int part = (int)( pChip->dwPosition / 384 );

		// ヒット処理してなければする
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;

			// 表示用小節番号の更新
			this->struct演奏情報.n現在の小節番号・デバッグ表示用 = part - 1;

			// Wave再生位置自動補正
			if( ! bSkip )
				g_DTX.AdjustWaves();
		}
		// 小節線表示
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
		{
			// BPM/BPx レーン小節線表示
			RECT rc;
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 23, s_rcLine.top + 1 );
			this->sfMap.Draw( 2, BAR_Y-pChip->nDotFromBarD, &rc );

			// ドラムレーン小節線表示
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 103, s_rcLine.top + 1 );
			this->sfMap.Draw( 33, BAR_Y-pChip->nDotFromBarD, &rc );

			// BGM/BGA/AVI レーン小節線表示
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 34, s_rcLine.top + 1 );
			this->sfMap.Draw( 146, BAR_Y-pChip->nDotFromBarD, &rc );

			// ギターレーン小節線表示
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 45, s_rcLine.top + 1 );
			this->sfMap.Draw( 187, 38+pChip->nDotFromBarG, &rc );

			// ベースレーン小節線表示
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 45, s_rcLine.top + 1 );
			this->sfMap.Draw( 233, 38+pChip->nDotFromBarB, &rc );

			// SE1-5 レーン小節線表示
			SetRect( &rc, s_rcLine.left, s_rcLine.top, s_rcLine.left + 61, s_rcLine.top + 1 );
			this->sfMap.Draw( 289, BAR_Y-pChip->nDotFromBarD, &rc );
		}
		// 小節番号表示
		if( ! bSkip && ! pChip->bHit ) 
		{
			this->txNumbers.SetScale( 0.5f, 0.5f );
			this->t数字描画( 125, BAR_Y-pChip->nDotFromBarD, part-1 );	// 小節線の横
			
			this->txNumbers.SetScale( 1.0f, 1.0f );
			this->t数字描画( 640, 480, this->struct演奏情報.n現在の小節番号・デバッグ表示用 );	// 画面右下のでかいの
		}
		break;
	}
	// 拍線
	case 0x51: {
		
		// ヒット処理してなければする
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;

		// レーン拍線表示
		if( ! bSkip && ! pChip->bHit && pChip->bVisible )
		{
			// BPM/BPx レーン拍線表示
			RECT rc;
			SetRect( &rc, s_rcSubLine.left, s_rcSubLine.top, s_rcSubLine.left + 23, s_rcSubLine.top + 1 );
			this->sfMap.Draw( 2, BAR_Y-pChip->nDotFromBarD, &rc );

			// ドラムレーン拍線表示
			SetRect( &rc, s_rcSubLine.left, s_rcSubLine.top, s_rcSubLine.left + 103, s_rcSubLine.top + 1 );
			this->sfMap.Draw( 33, BAR_Y-pChip->nDotFromBarD, &rc );

			// BGM/BGA/AVI レーン拍線表示
			SetRect( &rc, s_rcSubLine.left, s_rcSubLine.top, s_rcSubLine.left + 34, s_rcSubLine.top + 1 );
			this->sfMap.Draw( 146, BAR_Y-pChip->nDotFromBarD, &rc );

			// SE1-5 レーン拍線表示
			SetRect( &rc, s_rcSubLine.left, s_rcSubLine.top, s_rcSubLine.left + 61, s_rcSubLine.top + 1 );
			this->sfMap.Draw( 289, BAR_Y-pChip->nDotFromBarD, &rc );
		}
		break;
	}
	// MIDI ドラムコーラス
	case 0x52: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// フィルイン
	case 0x53: {
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;		// 何もしない
		break;
	}
	// AVI
	case 0x54: {
		
		if( ! pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
		{
			pChip->bHit = true;

			switch( pChip->AVItype )
			{
			case AVITYPE_AVI:
				if( pChip->pAVI )
				{
					// len = 0 なら転送元情報のみ有効
					this->actAVI.Start( pChip->nChannel, pChip->pAVI,
						BGA_W, BGA_H, 0, 0,			// sw, sh, ew, eh		; 領域   初期サイズ、終了サイズ
						0, 0, 0, 0,					// ssx, ssy, sex, sey	; 画像側 初期位置、最終位置
						0, 0, 0, 0,					// dsx, dsy, dex, dey	; 表示側 初期位置、最終位置
						0, pChip->dbTime );			// len, start
				}
				break;

			case AVITYPE_AVIPAN:
				if( pChip->pAVIPan )
				{
					this->actAVI.Start( pChip->nChannel, pChip->pAVI,
						pChip->pAVIPan->sw, pChip->pAVIPan->sh, pChip->pAVIPan->ew, pChip->pAVIPan->eh,		// 領域   初期サイズ、終了サイズ
						pChip->pAVIPan->ssx, pChip->pAVIPan->ssy, pChip->pAVIPan->sex, pChip->pAVIPan->sey,	// 画像側 初期位置、最終位置
						pChip->pAVIPan->dsx, pChip->pAVIPan->dsy, pChip->pAVIPan->dex, pChip->pAVIPan->dey,	// 表示側 初期位置、最終位置
						pChip->dbLong, pChip->dbTime );													// len, start
				}
				break;
			}
		}
		break;
	}
	// 未知
	default: {
		if( pChip->bHit && ( bSkip || pChip->nDotFromBarD < 0 ) )
			pChip->bHit = true;
		break;
	}
	}
}
void CStagePlay::tサウンド再生( DTX::Chip* pChip, EPlayPart part, long l音量 )
{
	if( ! pChip )
		return;

	switch( part )
	{

	// ドラム：
	// HHC, HHO については、前に発生した音を停止してから再生する。LC の場合は無関係。
	// ただし、前の音と今回の音が両方とも HHO の場合は停止しない。(HHOロール対策）
	case PLAYPART_DRUMS:
		if( pChip->nChannel >= 0x11 && pChip->nChannel <= 0x1A )
		{
			int nLane = s_nチャンネル0Atoレーン07[ pChip->nChannel - 0x11 ];

			if( nLane == 1 && ( pChip->nChannel == 0x11 || ( pChip->nChannel == 0x18 && this->struct演奏情報.n最後に再生したHHのチャンネル番号 != 0x18 ) ) )
			{
				g_DTX.StopWave( this->struct演奏情報.n最後に再生したHHのWAV番号, nLane );
				this->struct演奏情報.n最後に再生したHHのWAV番号			= pChip->nRealParam;
				this->struct演奏情報.n最後に再生したHHのチャンネル番号	= pChip->nChannel;
			}
			g_DTX.PlayChip( pChip, nLane, l音量 );
		}
		break;

	// ギター：
	// 前の音を停止してから発声
	case PLAYPART_GUITAR:
		g_DTX.StopWave( this->struct演奏情報.n最後に再生したギターのWAV番号, 8 );
		g_DTX.PlayChip( pChip, 8, l音量 );
		this->struct演奏情報.n最後に再生したギターのWAV番号 = pChip->nRealParam;
		break;

	// ベース：
	// 前の音を停止してから発声
	case PLAYPART_BASS:
		g_DTX.StopWave( this->struct演奏情報.n最後に再生したベースのWAV番号, 9 );
		g_DTX.PlayChip( pChip, 9, l音量 );
		this->struct演奏情報.n最後に再生したベースのWAV番号 = pChip->nRealParam;
		break;
	}
}
void CStagePlay::t数字描画( int rx, int by, int num )
{
	// 負数は表示しない
	if( num < 0 ) return;

	// 数値を桁ごとに分解（６桁まで；それ以上は無視）
	int pn[ 6 ];
	for( int i = 0; i < 6; i++ )
	{
		int n = num % 10;
		num = (num - n) / 10;
		pn[ 5-i ] = n;
	}

	// 画像の合計サイズの算出
	int w = 0;
	int h = 0;
	float fw, fh;
	bool bf = true;		// 最初の０は無視

	this->txNumbers.GetScale( &fw, &fh );
	
	for( int i = 0; i < 6; i++ )
	{
		if( pn[i] == 0 && bf && i < 5 ) continue;
		w += (int)( (s_rcNums[ pn[i] ].right - s_rcNums[ pn[i] ].left) * fw );
		h = (int)( MAX( h, (s_rcNums[ pn[i] ].bottom - s_rcNums[ pn[i] ].top) ) * (fh+0.2f) );
		bf = false;
	}

	// 描画
	int x = rx - w;
	int y = by - h;
	bf = true;
	for( int i = 0; i < 6; i++ )
	{
		if( pn[i] == 0 && bf && i < 5 ) continue;
		this->txNumbers.Draw( x, y, &s_rcNums[ pn[i] ] );
		x += (int)( (s_rcNums[ pn[i] ].right - s_rcNums[ pn[i] ].left) * fw );
		bf = false;
	}
}
void CStagePlay::t演奏情報を初期化する()
{
	this->struct演奏情報.b演奏中 = false;
	this->struct演奏情報.p現在のトップChip = NULL;
	this->struct演奏情報.n現在の小節番号・デバッグ表示用 = 0;
	this->struct演奏情報.db現在のBPM・デバッグ表示用 = 0.0;
	this->struct演奏情報.db開始時刻ms = INIT_TIME;
	for( int i = 0; i < 50; i++ )
		this->struct演奏情報.n最後に再生したBGMの実WAV番号[i] = -1;
	this->struct演奏情報.n最後に再生したHHのWAV番号			= -1;
	this->struct演奏情報.n最後に再生したHHのチャンネル番号	= 0;
	this->struct演奏情報.n最後に再生したギターのWAV番号		= -1;
	this->struct演奏情報.n最後に再生したベースのWAV番号		= -1;
}
//
bool CStagePlay::MyOneTimeSceneInit()
{
	// ここにコードを記述する。AddChild() など。
	AddChild( &this->sfMap );
	AddChild( &this->txNumbers );
	AddChild( &g_DTX );
	AddChild( &this->actAVI );

	g_DTX.bDTXV = true;
	g_DTX.SetTimer( &g_Timer );

	return CStage::MyOneTimeSceneInit();
}
bool CStagePlay::MyFinalCleanup()
{
	// ここにコードを記述する。

	return CStage::MyFinalCleanup();
}
bool CStagePlay::MyInitDeviceObjects()
{
	// ここにコードを記述する。CTexture/CSurface::InitDeviceObjects() など。
	this->sfMap.InitDeviceObjectsFromResource( _T("マップ"), MAKEINTRESOURCE(IDR_PNG_MAP), _T("PNG") );
	this->sfMap.ColorKeyEnable();
	this->txNumbers.InitDeviceObjectsFromResource( _T("数字"), MAKEINTRESOURCE(IDR_PNG_NUMBERS), _T("PNG") );

	return CStage::MyInitDeviceObjects();
}
bool CStagePlay::MyDeleteDeviceObjects()
{
	// ここにコードを記述する。

	// 次の CStage::MyDeleteDeviceObjects() では、子テクスチャ・子サーフェイスの Delete (CTexture/CSurface::Delete) が行われる。
	return CStage::MyDeleteDeviceObjects();
}
bool CStagePlay::MyActivate()
{
	this->bFirstWork = true;
	// ここにコードを記述する。
	this->t演奏情報を初期化する();

	this->phaseID = PHASE_PLAY_IDLE;
	this->bLoading要請あり = false;
	this->bPlaying要請あり = false;
	this->n開始小節番号 = 0;

	// 次の CStage::MyActivate() では、this->bActivity のセットならびに子サーフェイス・子テクスチャの Restore (MyRestoreDeviceTexture/Surface) が行われる。
	// なお、子 Activity の活性化は行われない。（このメソッドを抜けてから行われる。）
	return CStage::MyActivate();
}
bool CStagePlay::MyUnActivate()
{
	// ここにコードを記述する。

	// 次の CStage::MyUnActivate() では、this->bActivity のリセットならびに子サーフェイス・子テクスチャの Invalidate (MyInvalidateDeviceObjects) が行われる。
	// なお、子Activity の非活性化は行われない。（このメソッドを抜けてから行われる。）
	return CStage::MyUnActivate();
}
bool CStagePlay::MyRestoreDeviceTextures()
{
	// ここにコードを記述する。

	// 次の CStage::MyRestoreDeviceTextures() では、子テクスチャの Restore (CTexture::Restore) が行われる。
	return CStage::MyRestoreDeviceTextures();
}
bool CStagePlay::MyRestoreDeviceSurfaces()
{
	// ここにコードを記述する。

	// 次の CStage::MyRestoreDeviceSurfaces() では、子サーフェイスの Restore (CSurface::Restore) が行われる。
	return CStage::MyRestoreDeviceSurfaces();
}
bool CStagePlay::MyInvalidateDeviceObjects()
{
	// ここにコードを記述する。

	// 次の CStage::MyInvalidateDeviceObjects() では、子テクスチャ・子サーフェイスの Invalidate (CTexture/CSurface::Invalidate) が行われる。
	return CStage::MyRestoreDeviceSurfaces();
}
		}//Play
	}//Stage
}//DTXViewer
