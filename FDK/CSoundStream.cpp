#include "stdafx.h"
#include "CSoundStream.h"
#include "FDKError.h"
#include "CFileMM.h"
#include "xadec.h"
#include "vorbis/vorbisfile.h"
#include "COgg.h"

namespace FDK {
	namespace Sound {

CSoundStream::CSoundStream()
{
	this->pDSBuffer = NULL;
}
//
void CSoundStream::Init()
{
	SAFE_RELEASE( this->pDSBuffer );
	this->strFileName.clear();
	this->b最後に書き込んだバッファ = true;		// 後半
	this->b再生中 = false;
	this->soundType = SOUNDTYPE_UNKNOWN;
	this->l現在の音量 = 0;
	this->bLoop = false;
	this->n非ループ時の無音再生数 = 0;
	this->fSpeed = 1.0f;
	this->fPitch = 1.0f;
	this->nPause = 0;
	this->dwCurrentPosition = 0;
}
void CSoundStream::Term()
{
	switch( this->soundType )
	{
	case SOUNDTYPE_OGG:
		this->ogg.Term();
		break;
	}
	this->Stop();
	this->file.Term();
	Init();
}
void CSoundStream::Play( bool bLoop )
{
	if( this->pDSBuffer )
	{
		switch( this->soundType )
		{
		case SOUNDTYPE_OGG:
			this->t次のブロックを変換してバッファにセットする_OGG();
			break;
		}
		this->pDSBuffer->SetVolume( this->l現在の音量 );
		this->pDSBuffer->Play( 0, 0, DSBPLAY_LOOPING );
		this->b再生中 = true;
		this->bLoop = bLoop;
		this->n非ループ時の無音再生数 = 0;
	}
}
void CSoundStream::Stop()
{
	if( this->b再生中 && this->pDSBuffer )
	{
		this->pDSBuffer->Stop();
		this->b再生中 = false;
	}
}
void CSoundStream::Pause()
{
	// 未実装
}
void CSoundStream::Cont()
{
	// 未実装
}
void CSoundStream::Cont( double dbContTime )
{
	// 未実装
}
void CSoundStream::SetVolume( long lVolume )
{
	if( lVolume == 0 )
		lVolume = 1;		// Vol = 0 はなぜか音量100%になってしまうため...

	this->l現在の音量 = (long)(( 20.0 * log10( lVolume / 100.0 ) ) * 100.0);

	if( this->b再生中 )
		this->pDSBuffer->SetVolume( this->l現在の音量 );	// 演奏中なら即反映
}
void CSoundStream::SetPosition( DWORD dwNewPosition )
{
	// 未実装
}
DWORD CSoundStream::GetPositionFromTime( double dbTime )
{
	// 未実装
	return 0;
}
void CSoundStream::StreamWork()
{
	if( ! this->pDSBuffer || ! this->b再生中 ) 
		return;

	DWORD dw演奏位置, dw書き込み位置;
	if( FAILED( this->pDSBuffer->GetCurrentPosition( &dw演奏位置, &dw書き込み位置 ) ) )
		return;

	switch( this->soundType )
	{
	case SOUNDTYPE_OGG:
		if( ( dw演奏位置 <  this->wfx.nAvgBytesPerSec && ! this->b最後に書き込んだバッファ )
		 || ( dw演奏位置 >= this->wfx.nAvgBytesPerSec &&   this->b最後に書き込んだバッファ ) )
		{
			if( this->ogg.bEOF )
			{
				if( this->bLoop ) {
					this->ogg.RawSeek( 0 );		// 最初からループ再生
					this->ogg.bEOF = false;
					this->t次のブロックを変換してバッファにセットする_OGG();
				}
				else if( ++ this->n非ループ時の無音再生数 >= 2 )		// ループしないときは、EOF になってから２回バッファを通過したら再生を停止する。
					this->Stop();										// EOF 直後はまだ演奏が終わってなく、停止できないため。
				else
					this->t次のブロックを変換してバッファにセットする_OGG();
			}
			else
				this->t次のブロックを変換してバッファにセットする_OGG();
		}
		break;
	}
}
HRESULT	CSoundStream::CreateFromFile( LPDIRECTSOUND8 pDS, LPCTSTR filename )
{
	HRESULT hr = S_OK;
	
	if( ! pDS )
		return FDKERR_DirectSoundがNULL;
	
	// ファイルの読み込み
	if( ! this->file.Load( filename ) )
	{
		this->file.Term();
		return FDKERR_ファイルの読み込み失敗;
	}
	
	// ファイルデータからのサウンドバッファ作成
	if( FAILED( hr = CreateFromMemory( pDS, file.GetData(), file.GetSize() ) ) )
	{
		this->file.Term();
		return FDKERR_サウンドバッファの作成に失敗;
	}

	this->strFileName = filename;
	return hr;
}
HRESULT	CSoundStream::CreateFromMemory( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize )
{
	HRESULT hr = S_OK;

	if( ! pData || dwSize == 0 )
		return E_INVALIDARG;

	if( ! pDS )
		return FDKERR_DirectSoundがNULL;

	if( this->pDSBuffer )
		Term();	// 利用中 → 先に終了処理を行う

	// サウンドの識別とサウンドバッファの作成
	if( FAILED( hr = this->tサウンドの識別とサウンドバッファの作成_OGG( pDS, pData, dwSize ) ) )
//	if( FAILED( hr = this->tサウンドの識別とサウンドバッファの作成_XA(  pDS, pData, dwSize ) ) )
//	if( FAILED( hr = this->tサウンドの識別とサウンドバッファの作成_WAV( pDS, pData, dwSize ) ) )
//	if( FAILED( hr = this->tサウンドの識別とサウンドバッファの作成_MP3( pDS, pData, dwSize ) ) )
		return FDKERR_サウンドバッファの作成に失敗;		// 未サポートデータ

	return hr;
}
HRESULT	CSoundStream::tサウンドの識別とサウンドバッファの作成_OGG( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize )
{
	HRESULT hr = S_OK;

	// チェック
	if( ! this->ogg.Init( pData, dwSize ) )
		return FDKERR_SoundTypeが無効;		// Ogg じゃない

	// WAVEFORMATEX の取得
	if( ! this->ogg.GetFormat( &this->wfx ) )
	{
		this->ogg.Term();
		return FDKERR_SoundTypeが無効;		// 取得失敗
	}

	// サウンドバッファの作成
    DSBUFFERDESC dsbd;
    ZeroMemory( &dsbd, sizeof(DSBUFFERDESC) );
	dsbd.dwSize          = sizeof(DSBUFFERDESC);
	dsbd.dwFlags         = DSBCAPS_CTRLVOLUME | DSBCAPS_CTRLPAN | DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS_GLOBALFOCUS | DSBCAPS_CTRLFREQUENCY;
	dsbd.dwBufferBytes   = this->wfx.nAvgBytesPerSec * 2;		// ２秒分
	dsbd.guid3DAlgorithm = DS3DALG_DEFAULT;
	dsbd.lpwfxFormat     = &this->wfx;
	if( FAILED( hr = pDS->CreateSoundBuffer( &dsbd, &this->pDSBuffer, NULL ) ) )
		return FDKERR_サウンドバッファの作成に失敗;

	this->soundType = SOUNDTYPE_OGG;
	return S_OK;
}
bool	CSoundStream::t次のブロックを変換してバッファにセットする_OGG()
{
	LPBYTE  p書き込み先;
	DWORD	dw書き込みバイト数;
	
	if( this->b最後に書き込んだバッファ )
	{
		// 前半をロック
		if( FAILED( this->pDSBuffer->Lock( 0, this->wfx.nAvgBytesPerSec, (void**)&p書き込み先, &dw書き込みバイト数, NULL, NULL, 0 ) ) )
			return false;	// ロック失敗
	}
	else
	{
		// 後半をロック
		if( FAILED( this->pDSBuffer->Lock( this->wfx.nAvgBytesPerSec, this->wfx.nAvgBytesPerSec, (void**)&p書き込み先, &dw書き込みバイト数, NULL, NULL, 0 ) ) )
			return false;	// ロック失敗
	}
	
	this->ogg.ConvertToWav( p書き込み先, dw書き込みバイト数 );
	
	this->pDSBuffer->Unlock( p書き込み先, dw書き込みバイト数, NULL, 0 );
	this->b最後に書き込んだバッファ = ! this->b最後に書き込んだバッファ;

	return true;
}
	}//Sound
}//FDK
