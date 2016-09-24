#pragma once

#include "vorbis/vorbisfile.h"

namespace FDK {
	namespace Sound {

class COgg
{
public:
	bool	bEOF;		// ソースデータがEOFに達したらtrue

public:
	bool	Init( LPBYTE pOggRawData, DWORD dwOggRawDataSize );		// Oggメモリイメージからのオープン
	void	Term();													// 終了処理
	size_t	GetDestSize();											// 変換後に必要なバッファのサイズを得る。
	bool	ConvertToWav( LPBYTE pDestBuf, size_t szDestBufSize );	// Wave へ変換する。
	bool	GetFormat( WAVEFORMATEX* pwfx );						// WAVサウンドフォーマットを設定して返す。
	void	RawSeek( long pos );
	void	PcmSeek( long pos );
	COgg();

public:
	static size_t	read( void* ptr, size_t size, size_t nmemb, void* datasource );
	static int		seek( void* datasource, ogg_int64_t offset, int whence );
	static int		close( void* datasource );
	static long		tell( void *datasource );

protected:
	bool	bOpened;			// 現在Open中（m_vf, m_vi が有効）
	LPBYTE	pOggData;			// Ogg生データへのポインタ（このクラスではメモリ管理は行わない）
	DWORD	dwTotalSize;		// Ogg生データの総サイズ(bytes); コンストラクタの dwOggRawDataSize を保持
	DWORD	dwCurrentPos;		// 次に読み出されるデータの位置(0..m_dwTotalSize; m_dwTotalSize の位置で EOF)

	OggVorbis_File	vf;
	vorbis_info*	vi;

};

	}//Sound
}//FDK

using namespace FDK::Sound;
