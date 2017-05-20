#include "stdafx.h"
#include "vorbis/vorbisfile.h"
#include "COgg.h"

namespace FDK {
	namespace Sound {

static const WORD	s_bps = 2;	// byte / sample; 固定値

COgg::COgg()
{
	this->bOpened		= false;

	this->pOggData		= NULL;
	this->dwTotalSize	= 0;
	this->dwCurrentPos	= 0;

	this->vi = NULL;
}


bool	COgg::Init( LPBYTE pOggRawData, DWORD dwOggRawDataSize )
{
	if( ! pOggRawData )
		return false;	// データポインタがNULL

	if( this->bOpened )
		return false;	// 既にオープン済み

	this->pOggData		= pOggRawData;
	this->dwTotalSize	= dwOggRawDataSize;
	this->dwCurrentPos	= 0;

	// ogg のメモリイメージからのコールバック付きオープン
	static ov_callbacks s_callbackFunctions = {
		COgg::read,
		COgg::seek,
		COgg::close,
		COgg::tell
	};

	if( ov_open_callbacks( (void*)this, &this->vf, (char*)pOggRawData, (long)dwOggRawDataSize, s_callbackFunctions ) < 0 ) 
	{
		//Debug::OutFN( FNAME, _T("ov_open() に失敗しました。\n") );		ヘッダチェックもかねてるのでエラーメッセージは出さない。
		return false;
	}

	// ogg の情報取得
	if( ( this->vi = ov_info( &this->vf, -1 ) ) == NULL )
	{
		ov_clear( &this->vf );
		//Debug::OutFN( FNAME, _T("ov_info() に失敗しました。\n") );		ヘッダチェックもかねてるのでエラーメッセージは出さない。
		return false;
	}

	this->bOpened = true;
	this->bEOF = false;
	return true;
}
size_t	COgg::GetDestSize()
{
	if( ! this->bOpened )
		return 0;	// オープンされてない

	_ASSERT( this->vi );

	// デコード後のサイズを計算
	double dbDestSize = ceil( this->vi->channels * this->vi->rate * ov_time_total( &this->vf, -1) * s_bps );
	if( dbDestSize < 0.0 )
	{
		ov_clear( &this->vf );
		return 0;	// 失敗
	}
	return (size_t)dbDestSize;
}

bool	COgg::ConvertToWav( LPBYTE pDestBuf, size_t szDestBufSize )
{
	if( ! pDestBuf || szDestBufSize <= 0 )
		return false;	// パラメータが不正

	if( this->bEOF )
		return true;	// すでに EOF 到達ずみ

	// デコード；
	// １回ですべてがデコードされるわけではないことに注意。（１回につき 256～4096 bytesらしい）
	long lDecodedSize = 0;
	int  nCurrent = 0;
	while( szDestBufSize > (size_t)lDecodedSize )
	{
		long ret = ov_read(
			&this->vf,								// OggVobis_File
			(char*)(pDestBuf + lDecodedSize),		// バッファ；書き込む位置
			(int)(szDestBufSize - lDecodedSize),	// バッファのサイズ
			0,										// ビッグエンディアンなら 1 にする（x86 は 0）
			s_bps,									// 出力データの量子化ビット数をバイト単位で指定する。例：16bit量子化の場合は2。
			1,										// 出力データの符号形式。0:符号なし、1:符号あり。DirectSound では、8bitならunsigned, 16bitならsigned となる。
			&nCurrent );							// 論理ビットストリームのポインタ。

		if( ret < 0 )
			return false;	// ogg のデコード中にエラーが発生
		else if( ret == 0 )
		{
			// eof に到達した→まだ書き込みバッファが余っていれば消音を書き込む
			if( szDestBufSize > (size_t)lDecodedSize )
				ZeroMemory( (void*)(pDestBuf + lDecodedSize), (int)(szDestBufSize - lDecodedSize) );
			this->bEOF = true;
			break;
		}

		lDecodedSize += ret;
	}
	return true;
}

bool	COgg::GetFormat( WAVEFORMATEX* pwfx )
{
	if( ! pwfx )
		return false;	// パラメータが不正

	if( ! this->bOpened )
		return false;	// ogg がオープンされてない

	ZeroMemory( pwfx, sizeof( WAVEFORMATEX ) );
	pwfx->cbSize				= 0;
	pwfx->wFormatTag			= WAVE_FORMAT_PCM;
	pwfx->nChannels				= this->vi->channels;
	pwfx->nSamplesPerSec		= this->vi->rate;
	pwfx->nAvgBytesPerSec		= this->vi->rate * this->vi->channels * s_bps;
	pwfx->nBlockAlign			= this->vi->channels * s_bps;
	pwfx->wBitsPerSample		= s_bps * 8;
	return true;
}

void	COgg::Term()
{
	if( this->bOpened )
	{
		ov_clear( &this->vf );
		this->bOpened = false;
	}
}

void	COgg::RawSeek( long pos )
{
	if( ! this->bOpened )
		return;	// ogg がオープンされてない

	ov_raw_seek( &this->vf, pos );
}
void	COgg::PcmSeek( long pos )
{
	if( ! this->bOpened )
		return;	// ogg がオープンされてない

	ov_pcm_seek( &this->vf, pos );
}
size_t	COgg::read( void* ptr, size_t size, size_t nmemb, void* datasource )
{
	if( ! datasource || ! ptr )
		return 0;	// EOF; パラメータが不正

	COgg* pOgg = (COgg*)datasource;
	_ASSERT( pOgg->pOggData );

	if( pOgg->dwCurrentPos >= pOgg->dwTotalSize )
		return 0;	// EOF

	size_t	readsize = size * nmemb;
	if( pOgg->dwCurrentPos + readsize > pOgg->dwTotalSize )
		readsize = pOgg->dwTotalSize - pOgg->dwCurrentPos;

	memcpy( ptr, pOgg->pOggData + pOgg->dwCurrentPos, readsize );
	pOgg->dwCurrentPos += (DWORD) readsize;

	return readsize;
}

int		COgg::seek( void* datasource, ogg_int64_t offset, int whence )
{
	_ASSERT( datasource );
	
	COgg* pOgg = (COgg*)datasource;

	switch( whence )
	{
	// a. 最初の位置から
	case SEEK_SET:
		if( offset < 0 )
			pOgg->dwCurrentPos = 0;
		else
		{
			pOgg->dwCurrentPos = (DWORD) offset;
			if( pOgg->dwCurrentPos > pOgg->dwTotalSize )
				pOgg->dwCurrentPos = pOgg->dwTotalSize;
		}
		break;

	// b. 最後の位置から
	case SEEK_END:
		if( offset > 0 )
			pOgg->dwCurrentPos = pOgg->dwTotalSize;
		else if( -offset > pOgg->dwTotalSize )
			pOgg->dwCurrentPos = 0;
		else
			pOgg->dwCurrentPos = (DWORD)(pOgg->dwTotalSize + offset);
		break;

	// c. 現在の位置から
	case SEEK_CUR:
		if( pOgg->dwCurrentPos + offset > pOgg->dwTotalSize )
			pOgg->dwCurrentPos = pOgg->dwTotalSize;
		else if( pOgg->dwCurrentPos < -offset )
			pOgg->dwCurrentPos = 0;
		else
			pOgg->dwCurrentPos = (DWORD)(pOgg->dwCurrentPos + offset);
		break;

	default:
		return -1;	// 失敗
	}

	return 0;	// 成功
}

int		COgg::close( void* datasource )
{
	_ASSERT( datasource );
	COgg* pOgg = (COgg*)datasource;
	
	// 特に何もしない
	
	return 0;
}

long	COgg::tell( void *datasource )
{
	_ASSERT( datasource );
	COgg* pOgg = (COgg*)datasource;

	return pOgg->dwCurrentPos;
}

	}//Sound
}//FDK