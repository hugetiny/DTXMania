
// CCounter - アニメ用タイマとカウンタのセット

#pragma once

#include "CTimer.h"

namespace FDK {
	namespace General {

class CCounter
{
public:
	CCounter();
	void Start( int n開始値, int n終了値, int n間隔ms, CTimer* pTimer );	// アニメ開始
	void Step();															// カウントを進める。終了値に達している場合は終了値を維持する。
	void StepLoop();														// カウントを進める。終了値に達している場合は開始値に戻る。
	void Stop();															// アニメ終了（タイマ停止）
	bool b終了値に達した();													// 現在の nValue が n終了値 と同値の場合に true を返す。
	bool b動作中();															// 現在動作中なら true を返す。

public:
	double	dbTimer;	// タイマ値（直接参照）
	int		nValue;		// カウント値（直接参照）

protected:
	CTimer*	pTimer;
	int		n開始値;
	int		n終了値;
	double	db間隔;
};

	}//General
}//FDK

using namespace FDK::General;
