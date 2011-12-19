
// CSoundStream - ストリーム対応サウンド
//   Play() のあと、メインルーチンから StreamWork() を継続的に呼び出すこと。
//   ストリーム対応してないサウンドは CreateFrom…() で失敗するので判別できる。

#pragma once

#include "CFileMM.h"
#include "COgg.h"
#include "CSound.h"

namespace FDK {
	namespace Sound {

class CSoundStream 
{
public:
	CSoundStream();
	void	Init();				// 初期化
	void	Term();				// 終了

	void	Play( bool bLoop=false );
	void	Stop();
	void	Pause();
	void	Cont();														// 現在位置（m_dwCurrentPosition）から演奏再開
	void	Cont( double dbContTime );									// 指定した時刻から演奏再開
	void	SetVolume( long lVolume=100 );			// 音量の設定（lVolume = 0～100 [%]）
	void	SetPosition( DWORD dwNewPosition );							// 指定位置[byte]へ移動する。
	DWORD	GetPositionFromTime( double dbTime );						// 時刻から位置を取得する。
	void	SetSpeed( float fSpeed=1.0f )	{ this->fSpeed = fSpeed; }		// 再生スピードの設定
	void	SetPitch( float fPitch=1.0f )	{ this->fPitch = fPitch; }		// ピッチの設定
	void	StreamWork();
	bool	b再生中;

	HRESULT	CreateFromFile( LPDIRECTSOUND8 pDS, LPCTSTR filename );				// ファイルからサウンドを生成する
	HRESULT	CreateFromMemory( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );	// メモリデータからサウンドを生成する（呼出し後もメモリデータは解放しないこと）

protected:
	LPDIRECTSOUNDBUFFER	pDSBuffer;
	tstring				strFileName;
	bool				b最後に書き込んだバッファ;		// false=前半, true=後半
	long				l現在の音量;
	float				fSpeed;
	float				fPitch;
	bool				bLoop;
	int					nPause;
	DWORD				dwCurrentPosition;	// Pause() 時の位置
	SoundType			soundType;
	CFileMM				file;
	WAVEFORMATEX		wfx;
	int					n非ループ時の無音再生数;
	COgg				ogg;

protected:
	HRESULT	tサウンドの識別とサウンドバッファの作成_OGG( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );
	bool	t次のブロックを変換してバッファにセットする_OGG();
//	HRESULT	tサウンドの識別とサウンドバッファの作成_MP3( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );
//	HRESULT	tサウンドの識別とサウンドバッファの作成_WAV( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );
//	HRESULT	tサウンドの識別とサウンドバッファの作成_XA(  LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize );
};

	}//Sound
}//FDK

using namespace FDK::Sound;
