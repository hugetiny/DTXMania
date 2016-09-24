#include "stdafx.h"
#include "CSound.h"
#include "CSoundManager.h"
#include "FDKError.h"
#include "CFileMM.h"
#include "xadec.h"
#include "vorbis/vorbisfile.h"
#include "COgg.h"
#include "Debug.h"

namespace FDK {
	namespace Sound {

CSound::CSound()
{
	this->pDSBuffer = NULL;
}
void	CSound::Init()
{
	SAFE_RELEASE( this->pDSBuffer );
	this->dwDSBufferSize = 0;
	this->strFileName[0] = _T('\0');
	this->lVolume = DSBVOLUME_MAX;
	this->lPan = 0;
	this->fSpeed = 1.0f;
	this->fPitch = 1.0f;
	this->bLoop = false;
	this->nPause = 0;
	this->dwCurrentPosition = 0;
}
void	CSound::Term()
{
	Init();
}
HRESULT CSound::DuplicateFromSound( LPDIRECTSOUND8 pDS, CSound* srcSound )
{
	HRESULT hr;

	// 条件チェック
	if( ! pDS )
		return FDKERR_DirectSoundがNULL;
	if( ! srcSound )
		return FDKERR_サウンドバッファがNULL;
	if( this->pDSBuffer )
		Term();		// 利用中なので先に終了処理を行う

	// サウンドの複製
	if( FAILED( hr = pDS->DuplicateSoundBuffer( srcSound->GetDirectSoundBuffer(), &this->pDSBuffer ) ) )
		return FDKERR_サウンドバッファの作成に失敗;
	
	// サウンドバッファサイズの取得
	this->dwDSBufferSize = srcSound->GetDirectSoundBufferSize();

	return S_OK;
}
HRESULT	CSound::CreateFromFile( LPDIRECTSOUND8 pDS, LPCTSTR filename )
{
	HRESULT hr;

	// 条件チェック
	if( ! pDS )
		return FDKERR_DirectSoundがNULL;

	// ファイルの読み込み
	CFileMM file;
	if( ! file.Load( filename ) )
	{
		file.Term();
		return FDKERR_ファイルの読み込み失敗;
	}

	// ファイルデータからのサウンドバッファ作成
	if( FAILED( hr = CreateFromMemory( pDS, file.GetData(), file.GetSize() ) ) )
	{
		file.Term();
		return FDKERR_サウンドバッファの作成に失敗;
	}

	lstrcpyn( this->strFileName, filename, _MAX_PATH );
	file.Term();

	return S_OK;
}
HRESULT	CSound::CreateFromMemory( LPDIRECTSOUND8 pDS, BYTE* pData, DWORD dwSize )
{
	HRESULT hr;

	// 条件チェック
	if( ! pData || dwSize == 0)
		return E_INVALIDARG;
	
	if( ! pDS )
		return FDKERR_DirectSoundがNULL;
	
	if( this->pDSBuffer )
		Term();	// 利用中なので、先に終了処理を行う

	// サウンドのデコード
	if( FAILED( hr = DecodeFromOgg( pDS, pData, dwSize ) ) )
	if( FAILED( hr = DecodeFromXA(  pDS, pData, dwSize ) ) )
	if( FAILED( hr = DecodeFromMP3( pDS, pData, dwSize ) ) ) 
	if( FAILED( hr = DecodeFromWAV( pDS, pData, dwSize ) ) )
		return FDKERR_サウンドバッファの作成に失敗;		// 未サポートデータ

	return hr;
}

HRESULT CSound::CreateAndCopyBuffer( LPDIRECTSOUND8 pDS, WAVEFORMATEX* pwfx, BYTE* pData, DWORD dwSize )
{
	HRESULT hr;

	this->dwDSBufferSize = dwSize;

	// サウンドバッファの生成
    DSBUFFERDESC dsbd;
    ZeroMemory( &dsbd, sizeof(DSBUFFERDESC) );
	dsbd.dwSize          = sizeof(DSBUFFERDESC);
	dsbd.dwFlags         = DSBCAPS_CTRLVOLUME | DSBCAPS_CTRLPAN | DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS_GLOBALFOCUS | DSBCAPS_CTRLFREQUENCY | DSBCAPS_LOCDEFER;
	dsbd.dwBufferBytes   = dwSize;
	dsbd.guid3DAlgorithm = DS3DALG_DEFAULT;
	dsbd.lpwfxFormat     = pwfx;
	if( FAILED( hr = pDS->CreateSoundBuffer( &dsbd, &this->pDSBuffer, NULL ) ) )
		return FDKERR_サウンドバッファの作成に失敗;

	// サウンドバッファの Lost チェック
	if( FAILED( hr = RestoreBuffer( this->pDSBuffer, NULL ) ) )
		return hr;

	// デコードデータをサウンドバッファに転送
	VOID* pDSLockedBuffer		= NULL;
	DWORD dwDSLockedBufferSize	= 0;
    if( FAILED( hr = this->pDSBuffer->Lock( 0, dwSize, &pDSLockedBuffer, &dwDSLockedBufferSize, NULL, NULL, DSBLOCK_ENTIREBUFFER ) ) )
		return FDKERR_Lockに失敗;
	else
	{
		CopyMemory( pDSLockedBuffer, pData, dwSize );
		this->pDSBuffer->Unlock( pDSLockedBuffer, dwDSLockedBufferSize, NULL, 0 );
	}
	return S_OK;
}

HRESULT CSound::RestoreBuffer( LPDIRECTSOUNDBUFFER pDSB, BOOL* pbWasRestored )
{
    HRESULT hr;

	// 条件チェック
    if( pDSB == NULL )
        return CO_E_NOTINITIALIZED;
	
	if( pbWasRestored ) *pbWasRestored = FALSE;

	// ステータスで Lost チェック
    DWORD dwStatus;
    if( FAILED( hr = pDSB->GetStatus( &dwStatus ) ) )
        return hr;

    if( dwStatus & DSBSTATUS_BUFFERLOST )
    {
		// サウンドバッファが Lost しているので復旧する。
		// アプリケーションは活性化したばかりの場合、まだ DirectSound は制御権を
		// 渡してくれていないため、バッファの Restore が失敗することがある。
		// もしそうなったら、DirectSound が制御権をくれるまでスリープする。
		while( ( hr = pDSB->Restore() ) == DSERR_BUFFERLOST )
			Sleep( 100 );

		// ※ここではバッファの復元だけ。データの内容は呼び出し元で別途復元すること。

		if( pbWasRestored )	*pbWasRestored = TRUE;
	}
	return S_OK;
}

HRESULT	CSound::DecodeFromXA(  LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize )
{
	XAHEADER xah;
	HXASTREAM hxas;
	XASTREAMHEADER xash;
	UCHAR *ps;
	ULONG dlen;

	// ヘッダチェック
	CopyMemory( &xah, pSrcData, sizeof(XAHEADER) );
	if( xah.id != _XAID )
		return FDKERR_SoundTypeが無効;		// XA ではなかった

	ps = (UCHAR *)pSrcData + sizeof(XAHEADER);
	WAVEFORMATEX* pwfx = (WAVEFORMATEX*) malloc( sizeof(WAVEFORMATEX) );
	hxas = xaDecodeOpen( &xah, pwfx );
	if( hxas == NULL )
		return FDKERR_SoundTypeが無効;		// XA ではなかった

	// デコード後サイズの取得
	if( ! xaDecodeSize( hxas, xah.nDataLen, &dlen ) )
	{
		xaDecodeClose( hxas );
		return FDKERR_SoundTypeが無効;		// XA ではなかった？
	}

	// デコード後サイズ分のメモリを確保
	BYTE* pDestData = (LPBYTE) malloc( dlen );
	if( ! pDestData )
	{
		xaDecodeClose( hxas );
		return E_OUTOFMEMORY;		// メモリ不足
	}

	// XA → WAV 変換
	xash.pSrc = ps;
	xash.nSrcLen = xah.nDataLen;
	xash.nSrcUsed = 0;
	xash.pDst = (UCHAR *) pDestData;
	xash.nDstLen = dlen;
	xash.nDstUsed = 0;
	if( ! xaDecodeConvert( hxas, &xash ) )
	{
		xaDecodeClose( hxas );
		return FDKERR_SoundTypeが無効;		// XA ではなかった？
	}

	// XA 変換完了
	xaDecodeClose( hxas );

	// 実の変換後サイズの計算
//	lpSrcInfo->dwPCMSize = dlen;		// xadec のバグ
	DWORD dwTrueLen = xah.nSamples * xah.nChannels * 2;

	// 作成と書き込み
	HRESULT hr;
	if( FAILED( hr = CreateAndCopyBuffer( pDS, pwfx, pDestData, dwTrueLen ) ) )
	{
		SAFE_FREE( pwfx );
		SAFE_FREE( pDestData );
		return hr;
	}
	SAFE_FREE( pwfx );
	SAFE_FREE( pDestData );

	m_SoundType = SOUNDTYPE_XA;

	return S_OK;
}

HRESULT	CSound::DecodeFromMP3( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize )
{
	// ヘッダチェック
	if( dwSrcSize <= 128 )
		return FDKERR_SoundTypeが無効;		// MP3じゃない

	//  ID3v2タグがついているならば、読み飛ばす
	if( (pSrcData[0] == 'I') && (pSrcData[1] == 'D') && (pSrcData[2] == '3') )
	{
		DWORD dwID3Size = pSrcData[9] + (pSrcData[8]<<7) + (pSrcData[7]<<14) + (pSrcData[6]<<21);
		if( pSrcData[3] >= 0x04)
		{
			if( pSrcData[5] & 0x10 )
				dwID3Size += 20; // ID3v2.4.0以降・フッタあり
			else
				dwID3Size += 10; // ID3v2.4.0以降・フッタなし
		}
		else
			dwID3Size += 10;	 // ID3v2.3.0以前・フッタなし
		
		if( dwSrcSize <= dwID3Size + 128 )
			return FDKERR_SoundTypeが無効;		// MP3じゃない

		pSrcData += dwID3Size;
		dwSrcSize -= dwID3Size;
	}

	//	MP3チェック
	if( pSrcData[0] != 0xff || (pSrcData[1] & 0xf0) != 0xf0 )
		return FDKERR_SoundTypeが無効;		// MP3じゃない


	static const int anBitrate[2][3][16] = {
		{
			// MPEG-1
			{ 0,32,64,96,128,160,192,224,256,288,320,352,384,416,448,0 },	//	32000Hz(layer1)
			{ 0,32,48,56, 64, 80, 96,112,128,160,192,224,256,320,384,0 },	//	44100Hz(layer2)
			{ 0,32,40,48, 56, 64, 80, 96,112,128,160,192,224,256,320,0 },	//	48000Hz(layer3)
		},
		{
			// MPEG-2, 2.5
			{ 0,32,48,56, 64, 80, 96,112,128,144,160,176,192,224,256,0 },	//	32000Hz(layer1)
			{ 0, 8,16,24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0 },	//	44100Hz(layer2)
			{ 0, 8,16,24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0 },	//	48000Hz(layer3)
			},
	};
	static const int anFreq[2][4] = {
		{ 44100,48000,32000,0 },
		{ 22050,24000,16000,0 },
	};

	// レイヤー番号のチェック
	int nLayer = 4-((pSrcData[1] >> 1) & 3);
	if( nLayer == 4 )
		return FDKERR_SoundTypeが無効;			// MP3 じゃなかった？
	
	int nMpeg		= ((pSrcData[1] & 8) == 0) ? 1 : 0;
	int nBitrate	= anBitrate[ nMpeg ][ nLayer-1 ][ pSrcData[2]>>4 ];
	int nFreq		= anFreq[ nMpeg ][ (pSrcData[2] >> 2) & 3];
	int nChannel	= ((pSrcData[3] >> 6) == 3) ? 1 : 2;
	int nFrameSize	= 144000 * nBitrate / nFreq;

	// MP3のタグを用意する
	MPEGLAYER3WAVEFORMAT wfxMP3;
	ZeroMemory( &wfxMP3, sizeof(MPEGLAYER3WAVEFORMAT) );
	wfxMP3.wfx.cbSize			= MPEGLAYER3_WFX_EXTRA_BYTES;
	wfxMP3.wfx.wFormatTag		= WAVE_FORMAT_MPEGLAYER3;
	wfxMP3.wfx.nChannels		= nChannel;
	wfxMP3.wfx.nSamplesPerSec	= nFreq;
	wfxMP3.wfx.nAvgBytesPerSec	= nBitrate * 1000 / 8;
	wfxMP3.wfx.nBlockAlign		= 1;
	wfxMP3.wfx.wBitsPerSample	= 0;
	wfxMP3.wID					= MPEGLAYER3_ID_MPEG;
	wfxMP3.fdwFlags				= MPEGLAYER3_FLAG_PADDING_OFF;
	wfxMP3.nBlockSize			= nFrameSize;
	wfxMP3.nFramesPerBlock		= 1;
	wfxMP3.nCodecDelay			= 0x0571;

	//	ID3タグがついているならば、その分を除外する
	if( (pSrcData[ dwSrcSize-128 ] == 'T') && (pSrcData[ dwSrcSize-127 ] == 'A') && (pSrcData[ dwSrcSize-126 ] == 'G') )
		dwSrcSize-= 128;

	// ソースから PCM に変換可能な codec があるか調べ、wfxDest を設定
	MMRESULT mmr;
	WAVEFORMATEX wfxDest;
	wfxDest.wFormatTag = WAVE_FORMAT_PCM;
	if( ( mmr = acmFormatSuggest( NULL, (WAVEFORMATEX*)(&wfxMP3), &wfxDest, sizeof(WAVEFORMATEX), ACM_FORMATSUGGESTF_WFORMATTAG ) ) != 0 )
		return FDKERR_SoundTypeが無効;		// Error: CODEC がなかった

	BYTE* pDestData = NULL;
	DWORD dwDestSize = 0;

	// ACM をオープンする
	HACMSTREAM hAcm;
	if( ( mmr = acmStreamOpen( &hAcm, NULL, (WAVEFORMATEX*)(&wfxMP3), &wfxDest, NULL, NULL, NULL, ACM_STREAMOPENF_NONREALTIME ) ) != 0 )
		return FDKERR_SoundTypeが無効;		// Error: オープン失敗

	// 変換後のサイズを取得し、dwDestSize に設定
	if( ( mmr = acmStreamSize( hAcm, dwSrcSize, &dwDestSize, ACM_STREAMSIZEF_SOURCE ) ) != 0 )
	{
		acmStreamClose( hAcm, NULL );
		return FDKERR_SoundTypeが無効;		// Error: サイズ取得失敗
	}
	if( dwDestSize == 0 )
	{
		acmStreamClose( hAcm, NULL );
		return FDKERR_SoundTypeが無効;		// Error: 変換後のサイズがゼロ
	}

	// PCMバッファの確保
	pDestData = (LPBYTE) malloc( dwDestSize );
	if( ! pDestData ) 
	{
		acmStreamClose( hAcm, NULL );
		return E_OUTOFMEMORY;				// Error: メモリ不足
	}

	// 変換用ヘッダ準備
	ACMSTREAMHEADER	acmHeader;
	ZeroMemory( &acmHeader, sizeof(acmHeader) );
	acmHeader.cbStruct		= sizeof(acmHeader);
	acmHeader.pbSrc			= pSrcData;
	acmHeader.cbSrcLength	= dwSrcSize;
	acmHeader.pbDst			= pDestData;
	acmHeader.cbDstLength	= dwDestSize;
	if( ( mmr = acmStreamPrepareHeader( hAcm, &acmHeader, NULL ) ) != 0 )
	{
		acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
		acmStreamClose( hAcm, NULL );
		SAFE_FREE( pDestData );
		return FDKERR_SoundTypeが無効;		// ACMヘッダ登録失敗
	}

	// 変換
	if( ( mmr = acmStreamConvert( hAcm, &acmHeader, NULL ) ) != 0 )
	{
		acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
		acmStreamClose( hAcm, NULL );
		SAFE_FREE( pDestData );
		return FDKERR_SoundTypeが無効;		// ACM変換失敗
	}

	// 真の変換後サイズを取得
	dwDestSize = acmHeader.cbDstLengthUsed;

	// ACM を閉じる
	acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
	acmStreamClose( hAcm, NULL );

	// 作成と書き込み
	HRESULT hr;
	if( FAILED( hr = CreateAndCopyBuffer( pDS, &wfxDest, pDestData, dwDestSize ) ) )
	{
		SAFE_FREE( pDestData );
		return hr;
	}
	SAFE_FREE( pDestData );
	
	m_SoundType = SOUNDTYPE_MP3;

	return S_OK;
}

HRESULT	CSound::DecodeFromWAV( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize )
{
	MMRESULT mmr;

	// チェック
	MMIOINFO mmio;
	ZeroMemory( &mmio, sizeof(MMIOINFO) );
	mmio.pchBuffer = (LPSTR)pSrcData;
	mmio.fccIOProc = FOURCC_MEM;
	mmio.cchBuffer = dwSrcSize;

	HMMIO hmmio;
	if( ( hmmio = mmioOpen( NULL, &mmio, MMIO_READ ) ) == NULL )	// メモリからMMIOオープン
		return FDKERR_SoundTypeが無効;		// RIFF ではなかった

	// WAVEチャンクへ移動
	MMCKINFO ckiParent;
	ckiParent.fccType = mmioFOURCC('W','A','V','E');
	if( ( mmr = mmioDescend( hmmio, &ckiParent, NULL, MMIO_FINDRIFF ) ) != 0 )
	{
		mmioClose( hmmio, 0 );
		return FDKERR_SoundTypeが無効;		// WAVEチャンク移動失敗
	}

	// WAVEのfmtチャンクへの移動
	MMCKINFO ckiChild;
	ckiChild.ckid = mmioFOURCC('f','m','t',' ');
	if( ( mmr = mmioDescend( hmmio, &ckiChild, &ckiParent, MMIO_FINDCHUNK ) ) != 0 )
	{
		mmioClose( hmmio, 0 );
		return FDKERR_SoundTypeが無効;		// fmt チャンク移動失敗
	}

	// WAVEFORMATEX を取得し、fmtチャンクから抜ける
	WAVEFORMATEX* pw = (WAVEFORMATEX *) (((LPBYTE)pSrcData) + ((int)mmioSeek( hmmio, 0, SEEK_CUR )) );
	if( ( mmr = mmioAscend( hmmio, &ckiChild, 0 ) ) != MMSYSERR_NOERROR )
	{
		mmioClose( hmmio, 0 );
		return FDKERR_SoundTypeが無効;		// fmt チャンク脱出失敗
	}

	// WAVEFORMATEX を新しく malloc した領域に複写する
	WORD wfxsize = sizeof(WAVEFORMATEX) + ( ( pw->wFormatTag != WAVE_FORMAT_PCM ) ? pw->cbSize : 0 );
	WAVEFORMATEX* pwfx = (WAVEFORMATEX*) malloc( wfxsize );
	if( ! pwfx )
	{
		mmioClose( hmmio, 0 );
		return E_OUTOFMEMORY;				// メモリ不足
	}
	CopyMemory( pwfx, pw, wfxsize );

	// dataチャンクへ移動
	ckiChild.ckid = mmioFOURCC('d','a','t','a');
	if( ( mmr = mmioDescend( hmmio, &ckiChild, &ckiParent, MMIO_FINDCHUNK ) ) != 0 )
	{
		mmioClose( hmmio, 0 );
		SAFE_FREE( pwfx );
		return FDKERR_SoundTypeが無効;		// dataチャンクへの移動失敗
	}

	// サイズとポインタを取得し、MMIOを閉じる
	BYTE* pDestData = (LPBYTE) ((LPBYTE)pSrcData + mmioSeek( hmmio, 0, SEEK_CUR ) );
	DWORD dwDestSize = ckiChild.cksize;
	mmioClose( hmmio, 0 );

	/* ここまでで、pwfx, pDestData, dwDestSize に RiffWAV の内容が得られた。*/

	// WAVE フォーマットでない場合は ACM でデコード
	if( pwfx->wFormatTag != WAVE_FORMAT_PCM )
	{
		WAVEFORMATEX* pSrcWfx	= pwfx;
		BYTE* pSrcData			= pDestData;
		DWORD dwSrcSize			= dwDestSize;

		// ソースから PCM に変換可能な codec があるか調べ、wfxDest を設定
		WAVEFORMATEX wfxDest;
		wfxDest.wFormatTag = WAVE_FORMAT_PCM;
		if( ( mmr = acmFormatSuggest( NULL, pSrcWfx, &wfxDest, sizeof(WAVEFORMATEX), ACM_FORMATSUGGESTF_WFORMATTAG ) ) != 0 )
		{
			SAFE_FREE( pSrcWfx );
			return FDKERR_SoundTypeが無効;		// CODEC がなかった
		}


		// ACM をオープンする
		HACMSTREAM hAcm;
		if( ( mmr = acmStreamOpen( &hAcm, NULL, pSrcWfx, &wfxDest, NULL, NULL, NULL, ACM_STREAMOPENF_NONREALTIME ) ) != 0 )
		{
			SAFE_FREE( pSrcWfx );
			return FDKERR_SoundTypeが無効;		// ACMのオープンに失敗
		}

		// 変換後のサイズを取得し、dwDestSize に設定
		if( ( mmr = acmStreamSize( hAcm, dwSrcSize, &dwDestSize, ACM_STREAMSIZEF_SOURCE ) ) != 0 )
		{
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			return FDKERR_SoundTypeが無効;		// 変換後のサイズの取得に失敗
		}
		if( dwDestSize == 0 )
		{
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			return FDKERR_SoundTypeが無効;		// 変換後のサイズがゼロ
		}

		// 変換後フォーマットの取得＆設定
		pwfx = (WAVEFORMATEX*) malloc( sizeof(WAVEFORMATEX) );
		if( ! pwfx )
		{
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			return E_OUTOFMEMORY;				// メモリ不足
		}
		CopyMemory( pwfx, &wfxDest, sizeof(WAVEFORMATEX) );

		// PCMバッファの確保
		pDestData = (LPBYTE) malloc( dwDestSize );
		if( ! pDestData )
		{
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			return E_OUTOFMEMORY;				// メモリ不足
		}

		// 変換用ヘッダ準備
		ACMSTREAMHEADER	acmHeader;
		ZeroMemory( &acmHeader, sizeof(acmHeader) );
		acmHeader.cbStruct		= sizeof(acmHeader);
		acmHeader.pbSrc			= pSrcData;
		acmHeader.cbSrcLength	= dwSrcSize;
		acmHeader.pbDst			= pDestData;
		acmHeader.cbDstLength	= dwDestSize;
		if( ( mmr = acmStreamPrepareHeader( hAcm, &acmHeader, NULL ) ) != 0 )
		{
			acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			SAFE_FREE( pwfx );
			SAFE_FREE( pDestData );
			return FDKERR_SoundTypeが無効;		// ACMヘッダ登録失敗
		}

		// 変換
		if( ( mmr = acmStreamConvert( hAcm, &acmHeader, NULL ) ) != 0 )
		{
			acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
			acmStreamClose( hAcm, NULL );
			SAFE_FREE( pSrcWfx );
			SAFE_FREE( pwfx );
			SAFE_FREE( pDestData );
			return FDKERR_SoundTypeが無効;		// ACM変換失敗
		}

		// 真の変換後サイズを取得
		dwDestSize = acmHeader.cbDstLengthUsed;

		// ACM を閉じる
		acmStreamUnprepareHeader( hAcm, &acmHeader, NULL );
		acmStreamClose( hAcm, NULL );

		// 作成と書き込み
		HRESULT hr;
		if( FAILED( hr = CreateAndCopyBuffer( pDS, pwfx, pDestData, dwDestSize ) ) )
		{
			SAFE_FREE( pwfx );
			SAFE_FREE( pSrcWfx );
			SAFE_FREE( pDestData );
			return hr;
		}
		SAFE_FREE( pwfx );
		SAFE_FREE( pSrcWfx );
		SAFE_FREE( pDestData );
	}
	else
	{
		// 作成と書き込み
		HRESULT hr;
		if( FAILED( hr = CreateAndCopyBuffer( pDS, pwfx, pDestData, dwDestSize ) ) )
		{
			SAFE_FREE( pwfx );
			return hr;
		}
		SAFE_FREE( pwfx );
	}

	m_SoundType = SOUNDTYPE_WAV;

	return S_OK;
}

HRESULT	CSound::DecodeFromOgg( LPDIRECTSOUND8 pDS, BYTE* pSrcData, DWORD dwSrcSize )
{
	COgg ogg;

	// チェック
	if( ! ogg.Init( pSrcData, dwSrcSize ) )
		return FDKERR_SoundTypeが無効;		// Ogg じゃない

	// デコードバッファの確保
	size_t szDestDataSize = ogg.GetDestSize();
	if( szDestDataSize == 0 )
		return FDKERR_SoundTypeが無効;

	LPBYTE pDestData = (LPBYTE) malloc( szDestDataSize );
	if( ! pDestData )
	{
		ogg.Term();
		return E_OUTOFMEMORY;				// メモリ不足
	}
	
	// デコード
	if( ! ogg.ConvertToWav( pDestData, szDestDataSize ) )
	{
		ogg.Term();
		SAFE_FREE( pDestData );
		return FDKERR_SoundTypeが無効;		// 変換失敗
	}

	// WAVEフォーマットの取得
	WAVEFORMATEX wfx;
	if( ! ogg.GetFormat( &wfx ) )
	{
		ogg.Term();
		SAFE_FREE( pDestData );
		return FDKERR_SoundTypeが無効;		// 取得失敗
	}

	// サウンドの作成と書き込み
	HRESULT hr;
	if( FAILED( hr = CreateAndCopyBuffer( pDS, &wfx, pDestData, (DWORD)szDestDataSize ) ) )
	{
		ogg.Term();
		SAFE_FREE( pDestData );
		return hr;
	}

	// 完了
	SAFE_FREE( pDestData );
	ogg.Term();

	m_SoundType = SOUNDTYPE_OGG;

	return S_OK;
}

void	CSound::Play( bool bLoop )
{
	HRESULT hr;

	// チェック
	if( this->pDSBuffer == NULL )
		return;		// セカンダリバッファが無効

	// サウンドバッファのリストアチェック
	BOOL bRestored = FALSE;
    if( FAILED( hr = this->RestoreBuffer( this->pDSBuffer, &bRestored ) ) )
		return;		// サウンドバッファの復旧に失敗
	if( bRestored )
	{
		Term();
		TCHAR filename[_MAX_PATH];
		lstrcpyn( filename, this->strFileName, _MAX_PATH );
		if( FAILED( hr = CreateFromFile( CSoundManager::GetDirectSound(), filename ) ) )
			return;	// サウンドバッファの復旧に失敗
	}

	// 再生開始位置の決定
	DWORD dwStartPos = (m_SoundType == SOUNDTYPE_MP3) ? GetPositionFromTime( CSoundManager::dbMP3再生遅延時間ms ) : 0;

	// 再生カーソル初期化
	DWORD dwStatus;
	this->pDSBuffer->SetCurrentPosition( dwStartPos );
	this->pDSBuffer->GetStatus( &dwStatus );
	if( dwStatus & DSBSTATUS_PLAYING )
	{
		this->pDSBuffer->Stop();
		this->pDSBuffer->SetCurrentPosition( dwStartPos );
	}

	// 再生
	DWORD dwFreq;
	this->pDSBuffer->SetFrequency( DSBFREQUENCY_ORIGINAL );
	this->pDSBuffer->GetFrequency( &dwFreq );
	this->pDSBuffer->SetFrequency( (DWORD)(dwFreq * this->fSpeed * this->fPitch ) );
	this->pDSBuffer->SetVolume( this->lVolume );
	this->pDSBuffer->SetPan( this->lPan );
	this->pDSBuffer->Play( 0, 0, (bLoop) ? DSBPLAY_LOOPING : 0 );

	this->bLoop = bLoop;
	this->nPause = 0;
}

void	CSound::Stop()
{
	if( this->pDSBuffer )
	{
		this->pDSBuffer->Stop();
		this->pDSBuffer->SetCurrentPosition( 0 );
		this->nPause = 0;
	}
}

void	CSound::Pause()
{
	if( this->pDSBuffer && IsPlay() )
	{
		this->pDSBuffer->GetCurrentPosition( &this->dwCurrentPosition, NULL );
		this->pDSBuffer->Stop();
		this->nPause ++;
	}
}

void	CSound::Cont()
{
	if( this->nPause == 0 )
		return;
	this->nPause --;

	HRESULT hr;

	// チェック
	if( this->pDSBuffer == NULL )
		return;	// セカンダリバッファが無効

	// サウンドバッファのリストアチェック
	BOOL bRestored = FALSE;
    if( FAILED( hr = RestoreBuffer( this->pDSBuffer, &bRestored ) ) )
		return;	// サウンドバッファの復旧に失敗
	if( bRestored )
	{
		Term();
		TCHAR	filename[_MAX_PATH];
		lstrcpyn( filename, this->strFileName, _MAX_PATH );
		if( FAILED( hr = CreateFromFile( CSoundManager::GetDirectSound(), filename ) ) )
			return;	// サウンドバッファの復旧に失敗
	}

	// 再生開始位置の決定
	DWORD dwStartPos = this->dwCurrentPosition;

	// 再生カーソル初期化
	DWORD dwStatus;
	this->pDSBuffer->SetCurrentPosition( dwStartPos );
	this->pDSBuffer->GetStatus( &dwStatus );
	if( dwStatus & DSBSTATUS_PLAYING )
	{
		this->pDSBuffer->Stop();
		this->pDSBuffer->SetCurrentPosition( dwStartPos );
	}

	// 再生
	DWORD dwFreq;
	this->pDSBuffer->SetFrequency( DSBFREQUENCY_ORIGINAL );
	this->pDSBuffer->GetFrequency( &dwFreq );
	this->pDSBuffer->SetFrequency( (DWORD)(dwFreq * this->fSpeed * this->fPitch ) );
	this->pDSBuffer->SetVolume( this->lVolume );
	this->pDSBuffer->SetPan( this->lPan );
	this->pDSBuffer->Play( 0, 0, (this->bLoop) ? DSBPLAY_LOOPING : 0 );
}

void	CSound::Cont( double dbContTime )
{
	// カーソル移動(this->dwCurrentPositionも移動)
	SetPosition( GetPositionFromTime( dbContTime ) );
	
	// this->dwCurrentPosition から再開
	Cont();
}

void	CSound::SetPosition( DWORD dwNewPosition )
{
	if( ! this->pDSBuffer )
		return;

	// ブロック境界にそろえる
	DWORD dwSize;
	this->pDSBuffer->GetFormat( NULL, 0, &dwSize );
	WAVEFORMATEX* pWF = (LPWAVEFORMATEX) malloc( dwSize );
	this->pDSBuffer->GetFormat( pWF, dwSize, NULL );
	dwNewPosition -= dwNewPosition % pWF->nBlockAlign;

	// カーソルを移動
	DSBCAPS dsbc;
	ZeroMemory( &dsbc, sizeof(dsbc) );
	dsbc.dwSize = sizeof(DSBCAPS);
	this->pDSBuffer->GetCaps( &dsbc );
	if( dwNewPosition < dsbc.dwBufferBytes )
		this->pDSBuffer->SetCurrentPosition( dwNewPosition );

	this->dwCurrentPosition = dwNewPosition;	// 一応反映...
	free( pWF );
}

DWORD	CSound::GetPositionFromTime( double dbTime )
{
	if( ! this->pDSBuffer )
		return 0;

	// 周波数スピードを加味
	dbTime = dbTime * this->fSpeed * this->fPitch;

	// MP3 なら、dwTime に遅延時間を加算。
	if( m_SoundType == SOUNDTYPE_MP3 ) 
		dbTime += CSoundManager::dbMP3再生遅延時間ms;

	// dbTime [ms] から dwCurPos [byte] を算出
	DWORD dwCurPos, dwSize;
	this->pDSBuffer->GetFormat( NULL, 0, &dwSize );
	WAVEFORMATEX* pWF = (LPWAVEFORMATEX) malloc( dwSize );
	this->pDSBuffer->GetFormat( pWF, dwSize, NULL );
	dwCurPos = (DWORD)(dbTime * 0.001 * (pWF->nSamplesPerSec/* * this->fSpeedw*/) * pWF->nBlockAlign);	//pWF->nAvgBytesPerSec は小さい値を返すことがあり、計算式次第ではオーバーフローするので注意

	// ブロック境界にそろえる
	dwCurPos -= dwCurPos % pWF->nBlockAlign;

	free( pWF );
	return dwCurPos;
}

void	CSound::SetVolume( long lVolume )
{
	if( lVolume == 0 )
		lVolume = 1;		// Vol = 0 はなぜか音量100%になってしまうため...

	this->lVolume = (long)(( 20.0 * log10( lVolume / 100.0 ) ) * 100.0);

	// 演奏中なら即反映
	if( IsPlay() )
		this->pDSBuffer->SetVolume( this->lVolume );
}

void	CSound::SetPan( long lPan )
{
	if( lPan == 0 ) this->lPan = 0;
	else if( lPan == -100 ) this->lPan = DSBPAN_LEFT;
	else if( lPan ==  100 ) this->lPan = DSBPAN_RIGHT;
	else if( lPan < 0 ) this->lPan = (long)( (20.0 * log10((lPan+100)/100.0)) * 100.0 );
	else this->lPan = (long)( (-20.0 * log10((100-lPan)/100.0)) * 100.0 );

	// 演奏中なら即反映
	if( IsPlay() )
		this->pDSBuffer->SetPan( lPan );
}

bool	CSound::IsPlay()
{
	if( ! this->pDSBuffer )
		return false;

	DWORD dwStatus;
	this->pDSBuffer->GetStatus( &dwStatus );
	return dwStatus & DSBSTATUS_PLAYING;
}

DWORD	CSound::GetFrequency()
{
	if( ! this->pDSBuffer )
		return 0;

	DWORD dwFreq;
	this->pDSBuffer->GetFrequency( &dwFreq );
	return dwFreq;
}

DWORD	CSound::SetFrequency( DWORD dwFreq )
{
	if( ! this->pDSBuffer )
		return 0;

	DWORD dwOldFreq;
	this->pDSBuffer->GetFrequency( &dwOldFreq );
	this->pDSBuffer->SetFrequency( dwFreq );
	return dwOldFreq;
}

double	CSound::GetTotalTime()
{
	if( this->pDSBuffer == NULL )
		return 0.0;

	// WAVEFORMATEX 取得
	DWORD dwSize;
	this->pDSBuffer->GetFormat( NULL, 0, &dwSize );
	LPWAVEFORMATEX pWF = (LPWAVEFORMATEX) malloc( dwSize );
	this->pDSBuffer->GetFormat( pWF, dwSize, NULL );

	// 総時間の計算
	double dbTotalTime = (double)( this->dwDSBufferSize / ( pWF->nAvgBytesPerSec * 0.001 ) );

	free( pWF );
	return dbTotalTime;
}

	}//Sound
}//FDK