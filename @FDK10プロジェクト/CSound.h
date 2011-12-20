
// CSound - サウンドクラス

#pragma once

namespace FDK {
	namespace Sound {

enum SoundType
{
	SOUNDTYPE_UNKNOWN,
	SOUNDTYPE_WAV,
	SOUNDTYPE_XA,
	SOUNDTYPE_MP3,
	SOUNDTYPE_OGG
};

class CSound
{
public:
	void	Init();														// 初期化
	virtual void	Term();												// 終了

	void	Play( bool bLoop=false );									// 演奏開始
	void	Stop();														// 演奏停止
	void	Pause();													// 一時停止
	void	Cont();														// 現在位置（m_dwCurrentPosition）から演奏再開
	void	Cont( double dbContTime );									// 指定した時刻から演奏再開
	bool	IsPlay();													// 再生中なら true

	void	SetPan( long lPan=0 );										// パンポットの設定（lPan = -100[左]～0[中]～100[右]）
	void	SetVolume( long lVolume=100 );								// 音量の設定（lVolume = 0～100 [%]）
	void	SetSpeed( float fSpeed=1.0f )	{ this->fSpeed = fSpeed; }	// 再生スピードの設定
	void	SetPitch( float fPitch=1.0f )	{ this->fPitch = fPitch; }	// ピッチ（周波数倍率）の設定
	DWORD	GetFrequency();												// サウンドの周波数を返す。
	DWORD	SetFrequency( DWORD dwFreq );								// サウンドの周波数を設定し、設定前の値を返す。
	double	GetTotalTime();												// 総演奏時間[ms]を計算して返す。
	void	SetPosition( DWORD dwNewPosition );							// 指定位置[byte]へ移動する。
	DWORD	GetPositionFromTime( double dbTime );						// 時刻から位置を取得する。

	LPDIRECTSOUNDBUFFER GetDirectSoundBuffer() { return this->pDSBuffer; }
	DWORD				GetDirectSoundBufferSize() { return this->dwDSBufferSize; }

	HRESULT	DuplicateFromSound( LPDIRECTSOUND8 pDS, CSound* srcSound );			// 複製の生成
	HRESULT	CreateFromFile( LPDIRECTSOUND8 pDS, LPCTSTR filename );				// ファイルからサウンドを生成する
	HRESULT	CreateFromMemory( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );	// メモリデータからサウンドを生成する

public:
	CSound();

public:
	SoundType	m_SoundType;

protected:
	LPDIRECTSOUNDBUFFER	pDSBuffer;
	DWORD				dwDSBufferSize;
	TCHAR				strFileName[_MAX_PATH];
	long				lVolume;
	long				lPan;
	float				fSpeed;
	float				fPitch;
	bool				bLoop;
	int					nPause;
	DWORD				dwCurrentPosition;	// Pause() 時の位置

protected:
	// サウンドバッファが Lost しているか否か確認し、Lost しているなら復旧する。
	HRESULT	RestoreBuffer( LPDIRECTSOUNDBUFFER pDSB, BOOL* pbWasRestored );

	HRESULT	DecodeFromMP3( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize );	// MP3 データのデコード
	HRESULT	DecodeFromXA(  LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize );	// XA データのデコード
	HRESULT	DecodeFromWAV( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize );	// WAV データのデコード
	HRESULT	DecodeFromOgg( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize );	// Ogg データのデコード

	// サウンドバッファを作成し、そこへデータを書き込む。
	HRESULT CreateAndCopyBuffer( LPDIRECTSOUND8 pDS, WAVEFORMATEX* pwfx, BYTE* pData, DWORD dwSize );
};

	}//Sound
}//FDK

using namespace FDK::Sound;
