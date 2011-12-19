#pragma once

// FDK includes
#include "DTX.h"
#include "CSurface.h"
#include "CTexture.h"
#include "CCounter.h"
// App includes
#include "CStage.h"
#include "ActPlayAVI.h"
#include "ActPlayBGA.h"

namespace DTXViewer {
	namespace Stage {
		namespace Play {

enum EPlayPart
{
	PLAYPART_UNKNOWN	= -1,
	PLAYPART_DRUMS		= 0,
	PLAYPART_GUITAR		= 1,
	PLAYPART_BASS		= 2
};

class CStagePlay : public CStage
{
public:
	bool	Load( LPCTSTR strFileName );
	void	Play( int nPart );
	void	Stop();

	virtual int		MainWork();
	CStagePlay();

protected:
	struct _struct演奏情報 {
		bool			b演奏中;
		DTX::Chip*		p現在のトップChip;
		int				n現在の小節番号・デバッグ表示用;
		double			db現在のBPM・デバッグ表示用;
		double			db開始時刻ms;
		int				n最後に再生したBGMの実WAV番号[50];	// -1=未再生
		int				n最後に再生したHHのWAV番号;			// LCは無関係, -1=未再生
		int				n最後に再生したHHのチャンネル番号;	// LCは無関係,  0=未再生
		int				n最後に再生したギターのWAV番号;		// -1=未再生
		int				n最後に再生したベースのWAV番号;		// -1=未再生
	} struct演奏情報;

	CSurface		sfMap;
	CTexture		txNumbers;

	ActPlayAVI		actAVI;
	ActPlayBGA		actBGA;

	CCounter	ctNowLoading表示;
	bool		bLoading要請あり;
	tstring		strFileName;
	bool		bPlaying要請あり;
	int			n開始小節番号;

	void	SkipStart( double db開始時刻ms );
	void	ProcessCell( DTX::Chip* pChip, bool bSkip );
	void	t数字描画( int rx, int by, int num );
	void	tサウンド再生( DTX::Chip* pChip, EPlayPart part, long l音量=DTX_PLAYVOLUME );
	bool	tDTXファイルの読み込み();
	void	t再生開始();
	void	t演奏情報を初期化する();

	virtual bool	MyOneTimeSceneInit();
	virtual bool	MyActivate();
	virtual bool	MyUnActivate();
	virtual bool	MyFinalCleanup();
	virtual bool	MyInitDeviceObjects();
	virtual bool	MyRestoreDeviceTextures();
	virtual bool	MyRestoreDeviceSurfaces();
	virtual bool	MyInvalidateDeviceObjects();
	virtual bool	MyDeleteDeviceObjects();
};

		}//Play
	}//Stage
}//DTXViewer

using namespace DTXViewer::Stage::Play;
