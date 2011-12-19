#pragma once

#include "Activity.h"

namespace DTXViewer {
	namespace Stage {

enum EStage 
{
	STAGE_IDLE,				// 何もしない
	STAGE_PLAY				// 演奏画面（ドラム、ギター）
};
enum EPhase
{
	// 共通
	PHASE_COMMON_通常状態,
	PHASE_COMMON_フェードイン,
	PHASE_COMMON_フェードアウト,
	PHASE_COMMON_終了状態,

	// StagePlay
	PHASE_PLAY_IDLE,
	PHASE_PLAY_LOADING1,	// "NowLoading" 表示期間（まだ読み込まず）
	PHASE_PLAY_LOADING2,	// 読み込み開始
	PHASE_PLAY_PLAYING1,	// 演奏開始待ち
	PHASE_PLAY_PLAYING2		// 演奏中
};

static const int	BAR_Y			= 480-38;
static const int	BGA_X			= 361;
static const int	BGA_Y			=   2;
static const int	BGA_W			= 278;
static const int	BGA_H			= 355;
static const int	MAX_AVI_LAYER	= 1;

class CStage : public Activity
{
public:
	EStage	stageID;
	LPTSTR	strステージ名;

	EPhase	phaseID;			// 現在のフェーズ
	bool	bFirstWork;			// Activate() 後まだ MainWork() が呼ばれてないなら true

public:
	CStage(){}
};

	}//Stage
}//DTXViewer

using namespace DTXViewer::Stage;
