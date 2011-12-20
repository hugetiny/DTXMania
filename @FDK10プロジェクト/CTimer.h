#pragma once

// CTimer: 汎用タイマ
//    普段は CTimerEx を使うべし。

namespace FDK {
	namespace General {

#define	INIT_TIME	(-1.0)

#define	TIMERTYPE_PERFORMANCECOUNTER	0	// 高精度タイマ
#define TIMERTYPE_TIMEGTTIME			1	// マルチメディアタイマ
#define TIMERTYPE_GETTICKCOUNT			2	// 旧式タイマ
#define TIMERTYPE_UNKNOWN				99	// 不定

class CTimer
{
public:

public:
	void	Init( int timerType=TIMERTYPE_UNKNOWN );	// 初期化
	void	Term();										// 終了処理
	void	Reset();									// タイマをリセットする。PAUSE はすべて解除される。
	double	Get();										// 現在時刻の取得。PAUSE中ならPAUSE時点の時刻が返される。
	void	Set( double dbTime );						// 現在時刻の設定。PAUSE中ならPAUSE時点の時刻も変更される。
	void	Pause();									// 一時停止。
	void	Restart();									// 一時停止後の再開。
	double	GetSysTime()	{return GetTime();}		// 絶対時刻（システム起動後からの経過時刻）を返す。
	int		GetTimerType() { return timerType; }

public:
	CTimer() {};

protected:
	double	dbOffsetTime;		//	オフセット値
	double	dbPauseTime;		//	PauseかけたときのTime
	int		nPaused;			//	pause中なら0より大きい数
	UINT	uTimerID;			//  タイマ割り込みID

protected:
	static int				nRefCount;		// 参照回数
	static int				timerType;		// 仕様するタイマ
	static TIMECAPS			dwTimeCaps;		// timeGetTime 用
	static LARGE_INTEGER	liFrequency;		// 高精度タイマの解像度

protected:
	// システム起動後の経過時間をミリ秒で返す。
	static double GetTime();

	// 高精度タイマの使用可否を確認し、使えるなら設定する。
	// 使えるなら true、使えないなら false を返す。
	static bool CheckAndSetA_HighPerformanceTimer();
	
	// マルチメディアタイマの使用可否を確認し、使えるなら設定する。
	// 使えるなら true、使えないなら false を返す。
	static bool CheckAndSetB_MultimediaTimer();
	
	// 旧式タイマの設定を行う。
	// 常に true を返す。
	static bool SetC_OldTimer();
};

/////////////////////////////
// CTimerEx: 
//    (1) Reset(), Set(time) からの経過時間(ms)を Get() で取得。
//    (2) Flush() を呼ばない限り時刻は更新されない。
//    (3) Set(time) で時刻を設定できる。
//    (4) Pause(), Restart() で一時停止・解除ができる。
//    (5) GetSysTime() で絶対時刻を取得できる。
//    (6) SetTimer(proc), KillTimer() でタイマ割り込みの設定と解除ができる。
class CTimerEx : public CTimer
{
public:
	void   Init( int timerType=TIMERTYPE_UNKNOWN )
		{CTimer::Init(timerType); Reset();}
	void   Reset()				{Flush();dbOffsetTime=dbPauseTime=dbTimeGetTime;nPaused=0;}
	double Get()				{return (nPaused>0)? dbPauseTime-dbOffsetTime : dbTimeGetTime-dbOffsetTime;}
	double GetReal()			{return CTimer::Get();}
	void   Set( double dbTime )	{if(nPaused>0){dbOffsetTime=dbPauseTime-dbTime;}else{dbOffsetTime=dbTimeGetTime-dbTime;}}
	void   Pause()				{if(nPaused++ == 0){dbPauseTime=dbTimeGetTime;}}
	void   Restart()			{if(--nPaused == 0){dbOffsetTime+=dbTimeGetTime-dbPauseTime;}}

	void Flush()				{dbTimeGetTime=CTimer::GetTime();}
	double db前回Resetした時刻()		{return dbOffsetTime;}

protected:
	double dbTimeGetTime;			//	前回Flushした時刻
};

	}//General
}//FDK

using namespace FDK::General;
