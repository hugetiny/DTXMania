#pragma once

#include "CSound.h"
#include "CSoundStream.h"
#include "CSet.h"
#include "CSetString.h"

namespace FDK {
	namespace Sound {

class CSoundManager
{
public:
	HRESULT	Init( HWND hWnd, DWORD dwCoopLevel=DSSCL_PRIORITY );	// 初期化
	void	Term();									// 終了処理

	void	tストリームサウンドの再生処理();		// ストリームサウンドを使う場合はメインルーチンから継続的に呼び出すこと。

	static double	dbMP3再生遅延時間ms;

	//-----------------------------------------
	// キャッシュ未対応サウンド

	CSound*			pキャッシュされないサウンドを生成して返す( LPCTSTR filename );					// 使い終わったら自分で delete すること。

	CSoundStream*	pキャッシュされないストリームサウンドを生成して返す( LPCTSTR filename );		// 自分で delete しないこと。
	void			tキャッシュされないストリームサウンドを削除する( CSoundStream* pSoundStream );
	
	//-----------------------------------------
	// キャッシュ対応サウンド

	void	t使用フラグをクリアし複製セルを削除する();								// STEP1
	CSound*	pキャッシュ対応サウンドを作成して返す( LPCTSTR filename );				// STEP2
	void	t未使用サウンドを削除する();											// STEP3

	//-----------------------------------------
	// その他

	// プライマリバッファのフォーマットを設定/取得する。(例: SetPrimaryBufferFormat( 2, 22050, 16 ); )
	HRESULT SetPrimaryBufferFormat( DWORD dwPrimaryChannels, DWORD dwPrimaryFreq, DWORD dwPrimaryBitRate );
	HRESULT GetPrimaryBufferFormat( DWORD* pdwPrimaryChannels, DWORD* pdwPrimaryFreq, DWORD* pdwPrimaryBitRate );

	static LPDIRECTSOUND8	GetDirectSound() { return CSoundManager::pDS; }

	CSoundManager();
	virtual ~CSoundManager();

protected:
	static int				nインスタンス数;	// CSoundManager が作られるごとに１加算
	static LPDIRECTSOUND8	pDS;				// DirectSound

	struct SoundList {
		bool		b使用する;
		bool		b複製である;
		TCHAR		strファイル名[_MAX_PATH];
		FILETIME	ft最終更新時刻;
		CSound*		pSound;
		SoundList	*prev, *next;
	} *soundList, *soundListLast;

	CSet<CSoundStream>	setSoundStream;			// CreateSoundStreamFromFile() で作られたストリームサウンドの集合。

protected:
	// g_SampleMP3[] を解析し、MP3のDECODEに何 ms かかるか求めて dbMP3再生遅延時間ms に格納する。
	void tMP3遅延時間を計算し記憶する();
};

	}//Sound
}//FDK

using namespace FDK::Sound;
