#include "stdafx.h"
#include "Debug.h"
#include "DTX.h"
#include "CFileMM.h"
#include "MD5.h"
#include "str2float.h"
#include "CAvi.h"

namespace FDK {
	namespace General {

#ifndef SKIPSP
#define SKIPSP(p)	{while(*p==' '||*p=='\t')p++;}
#endif
#ifndef SKIPSP_BGA
#define SKIPSP_BGA(p)	{while(*p==' '||*p=='\t'||*p==','||*p=='('||*p==')'||*p=='['||*p==']'||*p=='x'||*p=='|')p++;}
#endif
#ifndef SKIPDEC
#define SKIPDEC(p)	{while((*p>='0'&&*p<='9')||*p=='-'||*p=='+')p++;}
#endif
#ifndef SKIPHEX
#define SKIPHEX(p)	{while((*p>='0'&&*p<='9')||(*p>='a'&&*p<='f')||(*p>='A'&&*p<='F')||*p=='-'||*p=='+')p++;}
#endif
#ifndef SKIPZEX
#define SKIPZEX(p)	{while((*p>='0'&&*p<='9')||(*p>='a'&&*p<='z')||(*p>='A'&&*p<='Z')||*p=='-'||*p=='+')p++;}
#endif

static const int s_nChannelToLane[10] = { 1,2,3,4,5,7,6,1,7,0 };		// Chに対応するレーン番号

DTX::DTX()
{
	// Clear() より前に初期化が必要なものだけここで初期化する
	this->pBPM		= this->pBPMLast	= NULL;
	this->pWave		= this->pWaveLast	= NULL;
	this->pBMP		= this->pBMPLast	= NULL;
	this->pBMPTEX	= this->pBMPTEXLast	= NULL;
	this->pBGA		= this->pBGALast	= NULL;
	this->pBGAPan	= this->pBGAPanLast	= NULL;
	this->pAVI		= this->pAVILast	= NULL;
	this->pAVIPan	= this->pAVIPanLast	= NULL;
	this->pChip		= this->pChipLast	= NULL;

	this->pTimer					= NULL;
	this->bDTXV						= false;
	this->bWAVCache					= true;
	this->bBMPCache					= true;
}

DTX::~DTX()
{
	Clear();
}

// 初期化
void DTX::Clear()
{
	// 内部状態の初期化 
	this->strFullPath.clear();
	this->strFolder.clear();
	this->strFileExt[0]			= _T('\0');
	this->scoreType				= SCORETYPE_UNKNOWN;
	this->strTitle.clear();
	this->strArtist.clear();
	this->strComment.clear();
	this->strPanel.clear();
	this->strGenre.clear();
	this->bHiddenLevel = false;
	for( int i = 0; i < 3; i++ )
		this->nLevel[i]			= 0;
	this->dbBPM					= 0.0;
	this->dbBaseBPM				= 0.0;
	this->strStage.clear();
	this->strPresound.clear();
	this->strPreimage.clear();
	this->strPremovie.clear();
	this->strBackground.clear();
	this->strBackgroundGR.clear();
	for( int i = 0; i < 7; i++ ) {
		this->strResultImage[i].clear();
		this->strResultMovie[i].clear();
	}
	this->strMIDI.clear();
	this->bMIDINote				= false;
	this->nMIDILevel			= 0;
	this->bBlackColorKey		= true;
	this->strPathWav.clear();
	this->fSpeed				= 1.0f;
	this->bHeaderOnly			= false;

	for( int i = 0; i < 10; i++)
		this->nTotalChipsD[ i ] = 0;
	this->nTotalChipsG			= 0;
	this->nTotalChipsB			= 0;
	this->bUseDrum				= false;
	this->bUseGuitar			= false;
	this->bUseBass				= false;
	this->bUseHHOpen			= false;
	this->bUseRide				= false;
	this->bUseLeftCymal			= false;
	this->strHash[0]			= _T('\0');
	// BPMリストの解放と初期化 
	{
		BPM* b = this->pBPM;
		while( b != NULL )
			{
			BPM* n = b->next;
			SAFE_DELETE( b );
			b = n;
		}
		this->pBPM = this->pBPMLast = NULL;
	}
	// WAVリストの解放と初期化 
	{
		Wave* w = this->pWave;
		while( w != NULL )
		{
			// Wave::sound[][] の解放はサウンドマネージャで行うため、ここでは解放しない。
			Wave* n = w->next;
			SAFE_DELETE( w );
			w = n;
		}
		this->pWave = this->pWaveLast = NULL;
	}
	// BMPリストの解放と初期化 
	{
		BMP* bm = this->pBMP;
		while( bm != NULL )
		{
			// BMP::pSurface の解放はサーフェイスマネージャで行うため、ここでは何もしない。
			BMP* n = bm->next;
			SAFE_DELETE( bm );
			bm = n;
		}
		this->pBMP = this->pBMPLast = NULL;
		this->bUseBMP				= false;
		this->dwBMPWidthMax			= 0;
		this->dwBMPHeightMax		= 0;
	}
	// BMPTEXリストの解放と初期化 
	{
		BMPTEX* bmt = this->pBMPTEX;
		while( bmt != NULL )
		{
			// BMPTEX::pTexture の解放はテクスチャマネージャで行うため、ここでは何もしない。
			BMPTEX* n = bmt->next;
			SAFE_DELETE( bmt );
			bmt = n;
		}
		this->pBMPTEX = this->pBMPTEXLast = NULL;
		this->bUseBMPTEX			= false;
		this->dwBMPTEXWidthMax		= 0;
		this->dwBMPTEXHeightMax		= 0;
	}
	// BGAリストの解放と初期化 
	{
		BGA* bg = this->pBGA;
		while( bg != NULL )
		{
			BGA* n = bg->next;
			SAFE_DELETE( bg );
			bg = n;
		}
		this->pBGA = this->pBGALast = NULL;
	}
	// BGAPANリストの解放と初期化 
	{
		BGAPan* bp = this->pBGAPan;
		while( bp != NULL )
		{
			BGAPan* n = bp->next;
			SAFE_DELETE( bp );
			bp = n;
		}
		this->pBGAPan = this->pBGAPanLast = NULL;
	}
	// AVIリストの解放と初期化 
	{
		this->tAVIリストの解放と初期化();
	}
	// AVIPANリストの解放と初期化 
	{
		this->tAVIPANリストの解放と初期化();
	}
	// CHIPリストの解放と初期化 
	{
		this->tCHIPリストの解放と初期化();
	}
	// Cache OFF の場合は、実データをここですべて解放する。 
	if( ! this->bWAVCache ) {
		this->soundManager.t使用フラグをクリアし複製セルを削除する();
		this->soundManager.t未使用サウンドを削除する();
	}
	if( ! this->bBMPCache ) {
		this->textureManager.ResetCache();
		this->textureManager.RemoveUnusedTextures();
		this->surfaceManager.ResetCache();
		this->surfaceManager.RemoveUnusedSurfaces();
	}
}
void DTX::tAVIリストの解放と初期化()
{
	AVI* a = this->pAVI;
	while( a != NULL )
	{
		if( a->bUse )
			a->avi.Close();
		AVI* n = a->next;
		SAFE_DELETE( a );
		a = n;
	}
	this->pAVI = this->pAVILast = NULL;
	this->bUseAVI	= false;
}
void DTX::tAVIPANリストの解放と初期化()
{
	AVIPan* ap = this->pAVIPan;
	while( ap != NULL )
	{
		AVIPan* n = ap->next;
		SAFE_DELETE( ap );
		ap = n;
	}
	this->pAVIPan = this->pAVIPanLast = NULL;
}
void DTX::tCHIPリストの解放と初期化()
{
	Chip* c = this->pChip;
	while( c != NULL )
	{
		Chip* n = c->next;
		SAFE_DELETE( c );
		c = n;
	}
	this->pChip = this->pChipLast = NULL;
}
// Activity
bool DTX::MyOneTimeSceneInit()
{
	// サウンドマネージャの初期化
	this->soundManager.Init( CD3DApplication::hWnd, DSSCL_PRIORITY );

	// テクスチャマネージャの初期化
	this->textureManager.OneTimeSceneInit();

	// サーフェイスマネージャの初期化
	this->surfaceManager.OneTimeSceneInit();


	return Activity::MyOneTimeSceneInit();
}

bool DTX::MyActivate()
{
	// 既に活性化してたら何もしない
	if( this->bActivate )
		return true;

	// 内部リソースの解放
	this->Clear();

	return Activity::MyActivate();
}

bool DTX::MyUnActivate()
{
	// 活性化してないなら何もしない
	if( ! this->bActivate ) 
		return true;

	this->tAVIリストの解放と初期化();
	this->tAVIPANリストの解放と初期化();
	this->tCHIPリストの解放と初期化();

	return Activity::MyUnActivate();
}

bool DTX::MyFinalCleanup()
{
	this->soundManager.Term();				// サウンドマネージャの終了
	this->textureManager.FinalCleanup();	// テクスチャマネージャの終了
	this->surfaceManager.FinalCleanup();	// サーフェイスマネージャの終了

	return Activity::MyFinalCleanup();
}

bool DTX::MyInitDeviceObjects()
{
	this->textureManager.InitDeviceObjects();	// テクスチャマネージャ
	this->surfaceManager.InitDeviceObjects();	// サーフェイスマネージャ

	return Activity::MyInitDeviceObjects();
}

bool DTX::MyRestoreDeviceTextures()
{
	// 活性化してないなら無効
	if( ! this->bActivate ) 
		return true;

	// テクスチャマネージャの構築
	this->textureManager.RestoreDeviceObjects();

	return Activity::MyRestoreDeviceTextures();
}

bool DTX::MyRestoreDeviceSurfaces()
{
	// 活性化してないなら無効
	if( ! this->bActivate ) 
		return true;

	// サーフェイスマネージャの構築
	this->surfaceManager.RestoreDeviceObjects();

	// 黒透過が有効なら、全BMPのカラーキーを 0xFF000000 に設定する。
	if( this->bBlackColorKey )
	{
		for( BMP* bmp = this->pBMP; bmp != NULL; bmp = bmp->next )
		{
			if( bmp->pSurface )
				bmp->pSurface->SetColorKey( 0xff000000 );
		}
	}

	return Activity::MyRestoreDeviceSurfaces();
}

bool DTX::MyInvalidateDeviceObjects()
{
	// 活性化してないなら無効
	if( ! this->bActivate ) 
		return true;

	this->textureManager.InvalidateDeviceObjects();		// テクスチャマネージャの解放
	this->surfaceManager.InvalidateDeviceObjects();		// サーフェイスマネージャの解放

	return Activity::MyInvalidateDeviceObjects();
}

bool DTX::MyDeleteDeviceObjects()
{
	this->textureManager.DeleteDeviceObjects();		// テクスチャマネージャの破棄
	this->surfaceManager.DeleteDeviceObjects();		// サーフェイスマネージャの破棄

	return Activity::MyDeleteDeviceObjects();
}

// 演奏ファイルの読み込み
bool DTX::Load( LPCTSTR fname, bool bHeaderOnly, double dbPlaySpeed )
{
	// (0) 前準備 
	{
		this->bHeaderOnly = bHeaderOnly;

		if( this->nMIDILevel > 2 )
			this->nMIDILevel = 2;

		// ファイルが開けなければここで終了 
		FILE *fp;
		if( _tfopen_s( &fp, fname, _T("rt") ) != 0 )
			return false;
		fclose( fp );
	}
	// (1) ファイル名、拡張子、フォルダ名を保存 → strFullPath, strFolder, strFileExt 
	{
		TCHAR strDrive[ _MAX_DRIVE ];
		TCHAR strDir[_MAX_DIR ];
		if( _tsplitpath_s( fname, strDrive, _MAX_DRIVE, strDir, _MAX_DIR, NULL, 0, NULL, 0 ) != 0 )
			return false;

		// ファイル名
		this->strFullPath = fname;

		// フォルダ名
		this->strFolder  = strDrive;
		this->strFolder += strDir;

		// 拡張子
		CFileMM::GetExt( this->strFullPath.c_str(), this->strFileExt );
	}
	// (2) 拡張子からデータタイプを決定 → scoreType 
	{
		static struct _dataType {
			LPTSTR			ext;
			ScoreType		type;
			LPTSTR			typeName;
		} s_dataType[] = {
			{ _T("dtx"), SCORETYPE_DTX, _T("DTX") },
			{ _T("bms"), SCORETYPE_BMS, _T("BMS") },
			{ _T("bme"), SCORETYPE_BMS, _T("BMS(BME)") },
			{ _T("gda"), SCORETYPE_GDA, _T("GDA") },
			{ _T("g2d"), SCORETYPE_G2D, _T("G2D") },
			{ _T("mid"), SCORETYPE_MID, _T("MIDI") },
			{ _T("tmp"), SCORETYPE_DTX, _T("DTX") },		// for GDAC2
			{ _T(""), SCORETYPE_UNKNOWN, _T("UNKNOWN") }
		};
		for( int i = 0; i < 99; i++)
		{
			if( s_dataType[i].type == SCORETYPE_UNKNOWN )
			{
				this->scoreType = SCORETYPE_UNKNOWN;		// 未知
				break;
			}
			else if( lstrcmpi( this->strFileExt, s_dataType[i].ext ) == 0 )
			{
				this->scoreType = s_dataType[i].type;
				break;
			}
		}

		// DTXV では、DTX 以外は読み込まない(2004.01.12)
		if( this->bDTXV && this->scoreType != SCORETYPE_DTX )
			return false;
	}
	// (3) ファイルを読み込む 
	switch( this->scoreType )
	{
	case SCORETYPE_DTX:
	case SCORETYPE_BMS:
	case SCORETYPE_G2D:
	case SCORETYPE_GDA:
		this->LoadDTX();
		break;

	case SCORETYPE_MID:
		this->LoadMID();
		break;
	}
	// (4) 読み込み後の処理 
	if( ! this->bHeaderOnly )
	{
		// (4-1) 小節線と拍線を挿入する（MIDI以外）
		if( this->scoreType != SCORETYPE_MID )
			this->InsertLines();

		// (4-2) チップの発声時刻を生成する（MIDI以外）
		if( this->scoreType != SCORETYPE_MID )
			CalcChipTime();

		// (4-3) 各チップの発声時刻を dbPlaySpeed にあわせて調整する（DTX, MID 共通）
		AdujstChipTimeByPlaySpeed( dbPlaySpeed );

		// (4-4) 可視チップ数を数える → nTotalChipsD/G/B 
		{
			for( int i = 0; i < 10; i++ )
				this->nTotalChipsD[ i ] = 0;
			this->nTotalChipsG = 0;
			this->nTotalChipsB = 0;
			
			for( Chip* cell = this->pChip; cell != NULL; cell = cell->next )
			{
				if( cell->nChannel >= 0x11 && cell->nChannel <= 0x1A )
					this->nTotalChipsD[ cell->nChannel - 0x11 ] ++;
				else if( cell->nChannel >= 0x20 && cell->nChannel <= 0x27 )
					this->nTotalChipsG ++;
				else if( cell->nChannel >= 0xA0 && cell->nChannel <= 0xA7 ) 
					this->nTotalChipsB ++;
			}
		}
		// (4-5) レーンごとに使用するWAVを登録する（LoadWAV()の前に行うこと！）
		if( !this->bHeaderOnly )
		{
			// (4-5-1) ビットマスク準備 
			WORD wWaveToLane[ 36*36 ];	// 00..ZZ = 0～36*36-1
			for( int i = 0; i < 36*36; i++ )
				wWaveToLane[ i ] = 0;

			// (4-5-2) 全チップを走査し、使用するレーンの該当ビットを立ててゆく。 
			for( Chip *cp = this->pChip; cp != NULL; cp = cp->next )
			{
				switch( cp->nChannel )
				{
				case 0x11: case 0x31: case 0xB1: wWaveToLane[ cp->nParam ] |= 0x002; break;		// HH
				case 0x12: case 0x32: case 0xB2: wWaveToLane[ cp->nParam ] |= 0x004; break;		// SD
				case 0x13: case 0x33: case 0xB3: wWaveToLane[ cp->nParam ] |= 0x008; break;		// BD
				case 0x14: case 0x34: case 0xB4: wWaveToLane[ cp->nParam ] |= 0x010; break;		// HT
				case 0x15: case 0x35: case 0xB5: wWaveToLane[ cp->nParam ] |= 0x020; break;		// LT
				case 0x16: case 0x36: case 0xB6: wWaveToLane[ cp->nParam ] |= 0x080; break;		// CY
				case 0x17: case 0x37: case 0xB7: wWaveToLane[ cp->nParam ] |= 0x040; break;		// FT
				case 0x18: case 0x38: case 0xB8: wWaveToLane[ cp->nParam ] |= 0x002; break;		// HH
				case 0x19: case 0x39: case 0xB9: wWaveToLane[ cp->nParam ] |= 0x080; break;		// RD
				case 0x1A: case 0x3A: case 0xBC: wWaveToLane[ cp->nParam ] |= 0x001; break;		// LC
				case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25: case 0x26: case 0x27: case 0xBA:
					wWaveToLane[ cp->nParam ] |= 0x100; break;																	// Guitar
				case 0xA0: case 0xA1: case 0xA2: case 0xA3: case 0xA4: case 0xA5: case 0xA6: case 0xA7: case 0xBB:
					wWaveToLane[ cp->nParam ] |= 0x200; break;																	// Bass
				case 0x01: case 0x1F: case 0x2F: case 0xAF:
				case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67: case 0x68: case 0x69: case 0x70:
				case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x76: case 0x77: case 0x78: case 0x79: case 0x80:
				case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87: case 0x88: case 0x89: case 0x90:
				case 0x91: case 0x92:
					wWaveToLane[ cp->nParam ] |= 0x400; break;																	// BGM
				}
			}
			// (4-5-3) 全Wavについて、使用するレーンだけ true にする。 
			for( Wave *wp = this->pWave; wp != NULL; wp = wp->next ) {
				for( int i = 0; i < 11; i++ )
					wp->bUseByLane[ i ] = ( wWaveToLane[ wp->vnum ] & (0x001 << i) ) ? true : false;
			}
		}
		// (4-6) WAV の登録＆構築 
		this->LoadWAV();
	}
	// (4-7) 曲データハッシュの計算 
	this->CalcHash();

	return true;
}
void DTX::LoadDTX()
{
	FILE *fp;
	if( _tfopen_s( &fp, this->strFullPath.c_str(), _T("rt") ) != 0 )
		return;		// 失敗

	// (1) 初期化 
	{
		// (1-1) 読み込み作業用変数の初期化 
		for( int i = 0; i < 36*36; i++ )
		{
			m_nWaveNum[i]	= -i;
			m_nWaveVol[i]	= -i;
			m_nWavePan[i]	= -10000-i;		// pan は -100～100なので、バッティングしないよう値域をずらす。
			m_nBPMNum[i]	= -i;
			m_nBMPNum[i]	= -i;
			m_nBMPTEXNum[i]	= -i;
			m_nBGANum[i]	= -i;
			m_nBGAPanNum[i]	= -i;
		}
		m_nWaveNumCur	= 0;
		m_nBPMNumCur	= 0;
		m_nBMPNumCur	= 0;

		// (1-2) #RESULTIMAGE/MOVIE/SOUND 用 優先順位バッファの初期化 
		for( int i = 0; i < 7; i++ )
		{
			m_nResultImagePriority[i] = 0;
			m_nResultMoviePriority[i] = 0;
			m_nResultSoundPriority[i] = 0;
		}
	}
	// (2) 行が尽きるまで読み込み＆処理する 
	m_nLine = 0;
	m_nRand   = 0;
	m_nSkip   = 0;
	m_bSkip[ m_nSkip ] = false;

	TCHAR* strLine = (TCHAR*) malloc( DTX_LINELEN );		// ※読み込み行は malloc で確保
	
	while( _fgetts( strLine, DTX_LINELEN, fp ) != NULL )
	{
		TCHAR* p;
		int zz;

		m_nLine ++;

		// 改行コードとコメントを削除する 
		{
			TCHAR* pLineTop = strLine;
			for( TCHAR* p = strLine; *p != _T('\0'); p++ )
			{
				// a. ';' 以降はコメントとして無視 
				if( *p == _T(';') )
				{
					*p = _T('\0');
			
					// ';' の直前の空白も削除
					if( p != pLineTop )
						p --;
					while( p != pLineTop && ( *p == _T(' ') || *p == _T('\t') ) )
					{
						*p = _T('\0');
						p --;
					}
					break;
				}

				// b. 改行コードは削除
				else if( *p == _T('\n') )
				{
					*p = _T('\0');
					break;
				}
			}
		}

		// 行頭が # でない行はスキップする 
		p = strLine;
		SKIPSP( p );
		if( *p != _T('#') )
			continue;
		p++;


		// #IF 
		if( IsCommand( &p, _T("IF") ) )
		{
			if( m_nSkip == 255 )
				continue;	// #IF の入れ子数が 255 を超えた → この #IF は無効

			if( m_bSkip[ m_nSkip ] )
				m_bSkip[ ++m_nSkip ] = true;			// スキップ中に現れたIFはスキップ
			else
				m_bSkip[ ++m_nSkip ] = ( _tstoi( p ) == m_nRand ) ? false : true;
			continue;
		}

		// #ENDIF 
		if( IsCommand( &p, _T("ENDIF") ) )
		{
			if( m_nSkip > 0 )	// 対応する #IF がある
				m_nSkip--;
			continue;
		}

		
		// これ以降は、スキップ中なら無視する。
		if( m_bSkip[ m_nSkip ] ) continue;
		
		// #PATH_WAV 
		if( IsCommand( &p, _T("PATH_WAV") ) )
		{
			this->strPathWav = p;
			continue;
		}
		// #TITLE 
		if( IsCommand( &p, _T("TITLE") ) )
		{
			this->strTitle = p;
			continue;
		}
		// #ARTIST 
		if( IsCommand( &p, _T("ARTIST") ) )
		{
			this->strArtist = p;
			continue;
		}
		// #COMMENT 
		if( IsCommand( &p, _T("COMMENT") ) )
		{
			this->strComment = p;
			continue;
		}
		// #DLEVEL, #PLAYLEVEL 
		if( IsCommand( &p, _T("DLEVEL") ) || IsCommand( &p, _T("PLAYLEVEL") ) )
		{
			this->nLevel[ 0 ] = _ttoi( p );

			// 常に 100 段階だとみなす。（10段階データのサポートは終了！(07.05.27)）
			if( this->nLevel[ 0 ] <   0 ) this->nLevel[ 0 ] =   0;
			if( this->nLevel[ 0 ] > 100 ) this->nLevel[ 0 ] = 100;
			continue;
		}
		// #GLEVEL 
		if( IsCommand( &p, _T("GLEVEL") ) )
		{
			this->nLevel[ 1 ] = _ttoi( p );

			// 常に 100 段階だとみなす。（10段階データのサポートは終了！(07.05.27)）
			if( this->nLevel[ 1 ] <   0 ) this->nLevel[ 1 ] =   0;
			if( this->nLevel[ 1 ] > 100 ) this->nLevel[ 1 ] = 100;
			continue;
		}
		// #BLEVEL 
		if( IsCommand( &p, _T("BLEVEL") ) )
		{
			this->nLevel[ 2 ] = _ttoi( p );

			// 常に 100 段階だとみなす。（10段階データのサポートは終了！(07.05.27)）
			if( this->nLevel[ 2 ] <   0 ) this->nLevel[ 2 ] =   0;
			if( this->nLevel[ 2 ] > 100 ) this->nLevel[ 2 ] = 100;
			continue;
		}
		// #GENRE 
		if( IsCommand( &p, _T("GENRE") ) )
		{
			this->strGenre = p;
			continue;
		}
		// #HIDDENLEVEL 
		if( IsCommand( &p, _T("HIDDENLEVEL") ) )
		{
			this->bHiddenLevel = ((*p==_T('o') || *p==_T('O')) && (*(p+1)==_T('n') || *(p+1)==_T('N'))) ? true : false;
			continue;
		}
		// #STAGEFILE 
		if( IsCommand( &p, _T("STAGEFILE") ) )
		{
			this->strStage = p;
			continue;
		}
		// #PREVIEW 
		if( IsCommand( &p, _T("PREVIEW") ) )
		{
			this->strPresound = p;
			continue;
		}
		// #PREIMAGE 
		if( IsCommand( &p, _T("PREIMAGE") ) )
		{
			this->strPreimage = p;
			continue;
		}
		// #PREMOVIE 
		if( IsCommand( &p, _T("PREMOVIE") ) )
		{
			this->strPremovie = p;
			continue;
		}
		// #BACKGROUND_GR 
		if( IsCommand( &p, _T("BACKGROUND_GR") ) )
		{
			this->strBackgroundGR = p;
			continue;
		}
		// #BACKGROUND, #WALL 
		if( IsCommand( &p, _T("BACKGROUND") ) || IsCommand( &p, _T("WALL") ) )
		{
			this->strBackground = p;
			continue;
		}
		// #RANDOM 
		if( IsCommand( &p, _T("RANDOM") ) )
		{
			m_nRand = (rand() % _ttoi(p)) + 1;
			continue;
		}
		// #BPM 
		if( (zz = GetCommand( &p, _T("BPM") ) ) != 0 )
		{
			// #BPM: は #BPM00: と見なす。
			if( zz < 0 ) zz = 0;

			// BPM値を取得。
			double bpm = str2float( p );
			if( bpm <= 0.0 )
				continue;	// #BPM に 0 以下の値または不正な文字列を指定した → この #BPM を無効

			// #BPM: なら代表 BPM 値として保存
			if( zz == 0 )
				this->dbBPM = bpm;
			
			// BPM セル追加
			BPM *bc = new BPM();
			bc->num		= ++ m_nBPMNumCur;
			bc->vnum	= zz;
			bc->bpm		= bpm;
			bc->prev = bc->next = NULL;
			APPENDLIST( this->pBPM, this->pBPMLast, bc );

			// 無限定義対応；
			// #BPMzz が初定義、かつ、この#BPMよりも前の行にオブジェクト記述があり、zz を使ったBPMチャンネルがあるなら更新する。
			if( m_nBPMNum[ zz ] == -zz )
			{
				for( Chip* cp = this->pChip; cp != NULL; cp=cp->next )
					if( ( GetChipParamType( cp->nChannel ) & DTXCPT_BPM ) && cp->nRealParam == -zz )
						cp->nRealParam = m_nBPMNumCur;	// BPM実番号
			}
			
			// 最後に BPM実番号を更新。
			m_nBPMNum[ zz ] = m_nBPMNumCur;

			continue;
		}

		// bHeaderOnly = true の時は、ここから下は無視。
		if( this->bHeaderOnly )	continue;

		// #MIDIFILE 
		if( IsCommand( &p, _T("MIDIFILE") ) )
		{
			this->strMIDI = p;
			continue;
		}
		// #PANEL
		TCHAR *q = p;
		if( IsCommand( &p, _T("PANEL") ) )
		{
			int isNotNum = 0;								// #26010 2011.12.23 yyagi: #PAN EL (WAV番号=ELの#PAN)を#PANELと誤解しないよう、
			for( int i = 0; i < strlen(p); i++) {			// 続きが数値かどうかを判断する
				if( !_istdigit( p[i] ) && p[i] != '-' && p[i] != '+' )
				{
					isNotNum = 1;
					break;
				}
			}
			if( isNotNum )			// 文字が含まれていたなら#PANEL, 数値のみなら#PAN
			{
				this->strPanel = p;
				continue;
			}
			p = q;
		}
		// #DTXVPLAYSPEED 
		if( IsCommand( &p, _T("DTXVPLAYSPEED") ) && this->bDTXV )
		{
			this->fSpeed = str2float( p );
			continue;
		}
		// #MIDINOTE 
		if( IsCommand( &p, _T("MIDINOTE") ) )
		{
			this->bMIDINote = ((*p==_T('o') || *p==_T('O')) && (*(p+1)==_T('n') || *(p+1)==_T('N'))) ? true : false;
			continue;
		}
		// #BLACKCOLORKEY 
		if( IsCommand( &p, _T("BLACKCOLORKEY") ) )
		{
			this->bBlackColorKey = ((*p==_T('o') || *p==_T('O')) && (*(p+1)==_T('n') || *(p+1)==_T('N'))) ? true : false;
			continue;
		}
		// #RESULTIMAGE_SS 
		if( IsCommand( &p, _T("RESULTIMAGE_SS") ) )	{ SetResultImage( 0, p, m_nResultImagePriority ); continue; }	// SS 以上

		// #RESULTIMAGE_S 
		if( IsCommand( &p, _T("RESULTIMAGE_S") ) )	{ SetResultImage( 1, p, m_nResultImagePriority ); continue; }	//  S 以上

		// #RESULTIMAGE_A 
		if( IsCommand( &p, _T("RESULTIMAGE_A") ) )	{ SetResultImage( 2, p, m_nResultImagePriority ); continue; }	//  A 以上

		// #RESULTIMAGE_B 
		if( IsCommand( &p, _T("RESULTIMAGE_B") ) )	{ SetResultImage( 3, p, m_nResultImagePriority ); continue;	}	//  B 以上

		// #RESULTIMAGE_C 
		if( IsCommand( &p, _T("RESULTIMAGE_C") ) )	{ SetResultImage( 4, p, m_nResultImagePriority ); continue; }	//  C 以上

		// #RESULTIMAGE_D 
		if( IsCommand( &p, _T("RESULTIMAGE_D") ) )	{ SetResultImage( 5, p, m_nResultImagePriority ); continue;	}	//  D 以上

		// #RESULTIMAGE_E 
		if( IsCommand( &p, _T("RESULTIMAGE_E") ) )	{ SetResultImage( 6, p, m_nResultImagePriority ); continue; }	//  E 以上

		// #RESULTIMAGE 
		if( IsCommand( &p, _T("RESULTIMAGE") ) )
		{
			// 全RANK書き換え（先に指定されていたものはすべて上書きされない）
			for( int i = 0; i < 7; i++ )
				if( m_nResultImagePriority[i] == 0 )
					this->strResultImage[i] = p;
			continue;
		}
		// #RESULTMOVIE_SS 
		if( IsCommand( &p, _T("RESULTMOVIE_SS") ) ) { SetResultMovie( 0, p, m_nResultMoviePriority ); continue; }	// SS 以上

		// #RESULTMOVIE_S 
		if( IsCommand( &p, _T("RESULTMOVIE_S") ) )	{ SetResultMovie( 1, p, m_nResultMoviePriority ); continue; }	//  S 以上

		// #RESULTMOVIE_A 
		if( IsCommand( &p, _T("RESULTMOVIE_A") ) )	{ SetResultMovie( 2, p, m_nResultMoviePriority ); continue; }	//  A 以上

		// #RESULTMOVIE_B 
		if( IsCommand( &p, _T("RESULTMOVIE_B") ) )	{ SetResultMovie( 3, p, m_nResultMoviePriority ); continue; }	//  B 以上

		// #RESULTMOVIE_C 
		if( IsCommand( &p, _T("RESULTMOVIE_C") ) )	{ SetResultMovie( 4, p, m_nResultMoviePriority ); continue; }	//  C 以上

		// #RESULTMOVIE_D 
		if( IsCommand( &p, _T("RESULTMOVIE_D") ) )	{ SetResultMovie( 5, p, m_nResultMoviePriority ); continue; }	//  D 以上

		// #RESULTMOVIE_E 
		if( IsCommand( &p, _T("RESULTMOVIE_E") ) )	{ SetResultMovie( 6, p, m_nResultMoviePriority ); continue; }	//  E 以上

		// #RESULTMOVIE 
		if( IsCommand( &p, _T("RESULTMOVIE") ) )
		{
			// 全RANK書き換え（先に指定されていたものはすべて上書きされない）
			for( int i = 0; i < 7; i++ )
				if( m_nResultMoviePriority[i] == 0 )
					this->strResultMovie[i] = p;
			continue;
		}
		// #RESULTSOUND_SS 
		if( IsCommand( &p, _T("RESULTSOUND_SS") ) ) { SetResultSound( 0, p, m_nResultSoundPriority ); continue; }	// SS 以上

		// #RESULTSOUND_S 
		if( IsCommand( &p, _T("RESULTSOUND_S") ) )	{ SetResultSound( 1, p, m_nResultSoundPriority ); continue; }	//  S 以上

		// #RESULTSOUND_A 
		if( IsCommand( &p, _T("RESULTSOUND_A") ) )	{ SetResultSound( 2, p, m_nResultSoundPriority ); continue; }	//  A 以上

		// #RESULTSOUND_B 
		if( IsCommand( &p, _T("RESULTSOUND_B") ) )	{ SetResultSound( 3, p, m_nResultSoundPriority ); continue; }	//  B 以上

		// #RESULTSOUND_C 
		if( IsCommand( &p, _T("RESULTSOUND_C") ) )	{ SetResultSound( 4, p, m_nResultSoundPriority ); continue; }	//  C 以上

		// #RESULTSOUND_D 
		if( IsCommand( &p, _T("RESULTSOUND_D") ) )	{ SetResultSound( 5, p, m_nResultSoundPriority ); continue; }	//  D 以上

		// #RESULTSOUND_E 
		if( IsCommand( &p, _T("RESULTSOUND_E") ) )	{ SetResultSound( 6, p, m_nResultSoundPriority ); continue; }	//  E 以上

		// #RESULTSOUND 
		if( IsCommand( &p, _T("RESULTSOUND") ) )
		{
			// 全RANK書き換え（先に指定されていたものはすべて上書きされない）
			for( int i = 0; i < 7; i++ )
				if( m_nResultSoundPriority[i] == 0 )
					this->strResultSound[i] = p;
			continue;
		}
		// #BASEBPM 
		if( IsCommand( &p, _T("BASEBPM") ) )
		{
			double n = str2float( p );

			if( n < 0.0 )
				continue;	// #BASEBPM に負数または不正な文字列を指定した → この #BASEBPM を無効

			this->dbBaseBPM = n;
			continue;
		}
		// #VOLUME, #WAVVOL 
		if( ( zz = this->GetCommand( &p, _T("VOLUME") ) ) != 0 || ( zz = this->GetCommand( &p, _T("WAVVOL") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #VOLUME(WAVVOL)の番号が範囲外

			// 音量値の取得
			int n = _ttoi( p );
			if( n > 100 ) n = 100;
			if( n <   0 ) n =   0;

			// 無限対応；過去に定義した #WAV のうち、volume が未定義のについて割り当てる。
			if( this->m_nWaveVol[ zz ] == -zz )
			{
				for( Wave* wp = this->pWave; wp != NULL; wp = wp->next )
					if( wp->volume == -zz )
						wp->volume = n;
			}

			// 最後に、WAVVOL スタックを更新。
			this->m_nWaveVol[ zz ] = n;
			continue;
		}
		// #PAN, #WAVPAN 
		if( ( zz = this->GetCommand( &p, _T("PAN") ) ) != 0 || ( zz = this->GetCommand( &p, _T("WAVPAN") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #WAVPAN(PAN)の番号が範囲外

			// 数値の取得
			int n = _ttoi( p );
			if( n >  100 ) n =  100;
			if( n < -100 ) n = -100;

			// 無限対応；過去に定義した #WAV のうち、pan が未定義のについて割り当てる。
			if( this->m_nWavePan[ zz ] == -10000-zz )
			{
				for( Wave* wp = this->pWave; wp != NULL; wp = wp->next )
					if( wp->pan == -10000-zz )
						wp->pan = n;
			}

			// 最後に、WAVPAN スタックを更新。
			this->m_nWavePan[ zz ] = n;
			continue;
		}
		// #WAV 
		if( ( zz = this->GetCommand( &p, _T("WAV") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// WAV番号が範囲外

			// セル追加（ここではまだ Wave::sound[][] は NULL ）
			Wave *wc		= new Wave();
			wc->num			= ++ m_nWaveNumCur;
			wc->vnum		= zz;
			wc->volume		= this->m_nWaveVol[ zz ];
			wc->pan			= this->m_nWavePan[ zz ];
			wc->bUse		= false;
			wc->strFileName = p;
			for( int i = 0; i < 11; i++ )
			{
				wc->bUseByLane[ i ]	= false;
				wc->nCurSound[ i ]	= -1;
				for( int j = 0; j < DTX_MAX_SOUND; j++ )
				{
					wc->sound[ i ][ j ]			= NULL;
					wc->bPlaying[ i ][ j ]		= false;
					wc->dbStartTime[ i ][ j ]	= 0;
					wc->dbPauseTime[ i ][ j ]	= 0;
				}
			}
			wc->dwBufferSize	= 0;
			wc->dbTotalTime		= 0;
			wc->prev = wc->next = NULL;
			APPENDLIST( this->pWave, this->pWaveLast, wc );	

			// #WAVzz が初定義、かつ
			// この#WAVよりも前の行にオブジェクト記述があり、zz を使ったWAVチャンネルがあるなら更新する。
			if( this->m_nWaveNum[ zz ] == -zz )
			{
				for( Chip* cp = this->pChip; cp != NULL; cp=cp->next )
					if( ( this->GetChipParamType( cp->nChannel ) & DTXCPT_WAV ) && cp->nRealParam == -zz )
						cp->nRealParam = this->m_nWaveNumCur;	// Wave実番号
			}
				
			// 最後に Wave実番号を更新。
			this->m_nWaveNum[ zz ] = this->m_nWaveNumCur;
			continue;
		}
		// #BMPTEX 
		if( ( zz = this->GetCommand( &p, _T("BMPTEX") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #BMPTEX番号が範囲外

			// セル追加（ここではまだ BMPTEX::pTexture は NULL → 設定は後処理の LoadBMP() で ）
			BMPTEX *bm		= new BMPTEX();
			bm->num 		= zz;
			bm->bUse		= false;
			bm->strFileName = p;
			bm->pTexture	= NULL;
			bm->prev = bm->next = NULL;
			APPENDLIST( this->pBMPTEX, this->pBMPTEXLast, bm );

			// BMPTEX使用フラグON
			this->bUseBMPTEX = true;
			continue;
		}
		// #BMP 
		if( ( zz = this->GetCommand( &p, _T("BMP") ) ) != 0 )
		{
			if( zz >= 36*36 )	// zz は省略可（省略時、zz==-1）
				continue;		// #BMP番号が範囲外

			// セル追加（ここではまだ BMP::pSurface は NULL → 設定は後処理の LoadBMP() で ）
			BMP *bm			= new BMP();
			bm->num 		= (zz < 0) ? 0 : zz;		// 番号2桁省略 → #BMP00: (初期BMP画像) (2006/04/23)
			bm->bUse		= false;
			bm->strFileName = p;
			bm->pSurface	= NULL;
			bm->prev = bm->next = NULL;
			APPENDLIST( this->pBMP, this->pBMPLast, bm );

			// BMP使用フラグON
			this->bUseBMP = true;
			continue;
		}
		// #BGAPAN 
		if( ( zz = this->GetCommand( &p, _T("BGAPAN") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #BGAPAN番号が範囲外

			// パラメータ取得
			int bmp, sw, sh, ew, eh, ssx, ssy, sex, sey, dsx, dsy, dex, dey, len;
			bmp = GetZex(p); SKIPZEX(p); SKIPSP_BGA(p);
			sw  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sh  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ew  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			eh  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ssx = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ssy = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sex = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sey = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dsx = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dsy = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dex = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dey = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			len = GetDec(p);
				
			// BMP番号の省略ならびに 00 の指定は無効
			if( bmp <= 0 )
				continue;	// 不正なBMP番号が指定されています。

			// クリッピング
			// → BMPのサイズは実行時まで判らないので、クリッピングは実行時に行う。

			// セル追加
			BGAPan *bc = new BGAPan();
			bc->num = zz;
			bc->bmp = bmp;
			bc->sw  =  sw;	bc->sh  =  sh;
			bc->ew  =  ew;	bc->eh  =  eh;
			bc->ssx = ssx;	bc->ssy = ssy;
			bc->sex = sex;	bc->sey = sey;
			bc->dsx = dsx;	bc->dsy = dsy;
			bc->dex = dex;	bc->dey = dey;
			bc->len = len;
			bc->prev = bc->next = NULL;
			APPENDLIST( this->pBGAPan, this->pBGAPanLast, bc );
			continue;
		}
		// #BGA 
		if( ( zz = this->GetCommand( &p, _T("BGA") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #BGA番号が範囲外

			// パラメータ取得
			int bmp, x1, y1, x2, y2, ox, oy;
			bmp = GetZex(p); SKIPZEX(p); SKIPSP_BGA(p);
			x1  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			y1  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			x2  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			y2  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ox  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			oy  = GetDec(p);

			// BMP番号の省略ならびに 00 の指定は無効
			if( bmp <= 0 ) 
				continue;	// 不正なBMP番号が指定されている

			// 座標調整
			// → BMPのサイズは実行時まで判らないので、BMP側のクリッピングは実行時に行うものとする。
			if( x1 > x2  ) {int t=x1; x1=x2; x2=t;}		// 必ず(x1,y1)が(x2,y2)の左上に来るようにする
			if( y1 > y2  ) {int t=y1; y1=y2; y2=t;}

			// セル追加
			BGA *bc  = new BGA();
			bc->num  = zz;
			bc->bmp  = bmp;
			bc->x1   = x1;	bc->y1 = y1;
			bc->x2   = x2;	bc->y2 = y2;
			bc->ox   = ox;	bc->oy = oy;
			bc->prev = bc->next = NULL;
			APPENDLIST( this->pBGA, this->pBGALast, bc);
			continue;	
		}
		// #AVIPAN 
		if( ( zz = this->GetCommand( &p, _T("AVIPAN") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36)
				continue;	// #AVIPAN番号が範囲外

			// パラメータ取得
			int avi, sw, sh, ew, eh, ssx, ssy, sex, sey, dsx, dsy, dex, dey, len;
			avi = GetZex(p); SKIPZEX(p); SKIPSP_BGA(p);
			sw  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sh  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ew  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			eh  = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ssx = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			ssy = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sex = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			sey = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dsx = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dsy = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dex = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			dey = GetDec(p); SKIPDEC(p); SKIPSP_BGA(p);
			len = GetDec(p);
				
			// AVI番号の省略ならびに 00 の指定は無効。
			if( avi == 0 ) 
				continue;	// 不正なAVI番号が指定されている

			// クリッピング；
			// → AVIのサイズは実行時まで判らないので、クリッピングは実行時に行う。

			// セル追加
			AVIPan *ac = new AVIPan();
			ac->num = zz;
			ac->avi = avi;
			ac->sw  =  sw;	ac->sh  =  sh;
			ac->ew  =  ew;	ac->eh  =  eh;
			ac->ssx = ssx;	ac->ssy = ssy;
			ac->sex = sex;	ac->sey = sey;
			ac->dsx = dsx;	ac->dsy = dsy;
			ac->dex = dex;	ac->dey = dey;
			ac->len = len;
			ac->prev = ac->next = NULL;
			APPENDLIST( this->pAVIPan, this->pAVIPanLast, ac );
			continue;
		}
		// #AVI, #VIDEO 
		if( ( zz = this->GetCommand( &p, _T("AVI") ) ) != 0 || ( zz = this->GetCommand( &p, _T("VIDEO") ) ) != 0 )
		{
			if( zz < 0 || zz >= 36*36 )
				continue;	// #AVI(VIDEO)番号が範囲外

			// セル追加（ここではまだ AVI::avi は初期化しない）
			AVI *ac = new AVI();
			ac->num  = zz;//++ nAVINumCur;	--> #AVIPAN 導入にともない、無限化を解除。(2006/5/5)
			ac->bUse = false;
			ac->strFileName = p;
			ac->prev = ac->next = NULL;
			APPENDLIST( this->pAVI, this->pAVILast, ac );

			// AVI使用フラグON
			this->bUseAVI = true;
			continue;
		}

		// その他：チップ配置 
		{
			// (1) 小節番号を取得 → nPart 
			int nPart = GetDec3( p );
			p += 3;
			if( nPart < 0 )
				continue;	// 小節番号が不正

			nPart ++;	// 1小節ずらす（曲開始時の -1 小節を作るため）

			// (2) チャンネル番号取得 → nCh 
			int nCh = GetChannel( p );
			p += 2;
			if( nCh < 0 )
				continue;	// チャンネル番号が不正

			// (3) ':' と空白をスキップ 
			SKIPSP( p );
			if( *p == _T(':') )
			{
				p++;
				SKIPSP( p );
			}
			// (4) 使用フラグ；該当するものを ON 
			if     ( nCh >= 0x11 && nCh <= 0x1A ) this->bUseDrum		= true;
			else if( nCh >= 0x20 && nCh <= 0x27 ) this->bUseGuitar		= true;
			else if( nCh >= 0xA0 && nCh <= 0xA7 ) this->bUseBass		= true;
			if( nCh == 0x18	)					  this->bUseHHOpen		= true;
			if( nCh == 0x19 )					  this->bUseRide		= true;
			if( nCh == 0x1A )					  this->bUseLeftCymal	= true;

			// (5) パラメータ取得(ch.02の場合): 実数指定 
			if( nCh == 0x02 )
			{
				double d = str2float( p );
				if( d <= 0.0 )
					continue;

				Chip *cell = new Chip();
				cell->dwPosition	= nPart * DTX_RESOLVE;
				cell->nChannel		= nCh;
				cell->dbParam		= d;
				this->InsertChip( cell );
				continue;
			}
			// (6) パラメータ取得(ch.02以外) 
			{	
				// (6-1) オブジェクト列の長さを数える → n桁数 
				int n桁数 = 0;
				TCHAR* q = p;
				while( *q != _T('\n') && *q != _T('\0') && *q != _T(' ') && *q != _T('\t') )
				{
					// '_' は無視
					if( *q == _T('_') ) {
						q++;
						continue;
					}
					if( ( *q >= _T('0') && *q <= _T('9') ) || ( *q >= _T('a') && *q <= _T('z') ) || (*q >= _T('A') && *q <= _T('Z') ) ) {
						n桁数++;
						q ++;
						continue;
					}
					else 
					{
						n桁数 = -1;
						break;		// オブジェクト配置文字列に 0～9,A～Z 以外の文字(%02x)が使われている → この行を無視
					}
				}
				if( ( n桁数 % 2 ) != 0 )
					n桁数 --;		// 桁数が奇数なら、最後の一桁は無視
				if( n桁数 <= 0 )
					continue;		// 桁がないかエラーなら無視

				// (6-2) オブジェクトを２桁ずつ Chip 化 
				for( int i = 0; i < n桁数 / 2; i++ )
				{
					// (6-2-1) '_' は無視 
					if( *p == _T('_') )
					{
						p++; i--;
						continue;
					}
					// (6-2-2) '00' なら無視 
					if( *p == _T('0') && *(p+1) == _T('0') ) {
						p += 2;
						continue;
					}

					// (6-2-3) zz 値の取得；Ch.03 のみ16進数、その他は36進数 
					int zz = ( nCh == 0x03 ) ? GetHex( p ) : GetZex( p );
					
					// (6-2-4) チップを生成 
					Chip *cc = new Chip();
					cc->dwPosition	= ( nPart * DTX_RESOLVE ) + ( DTX_RESOLVE * i) / ( n桁数 / 2 );
					cc->nChannel	= nCh;
					cc->nParam		= zz;
					cc->nRealParam	= zz; 
					if( nCh >= 0x11 && nCh <= 0x1A ) cc->nType = 0;		// ドラムチャンネル
					if( nCh >= 0x20 && nCh <= 0x27 ) cc->nType = 1;		// ギターチャンネル
					if( nCh >= 0xA0 && nCh <= 0xA7 ) cc->nType = 2;		// ギターチャンネル
					// (6-2-5) WAV系チャンネルなら zz を WAVE実番号に変換する 
					if( this->GetChipParamType( cc->nChannel ) & DTXCPT_WAV )
						cc->nRealParam = this->m_nWaveNum[ zz ];
					// (6-2-6) BPMEX系チャンネルなら zz を BPM実番号に変換する 
					else if( this->GetChipParamType( cc->nChannel ) & DTXCPT_BPMEX )
						cc->nRealParam = this->m_nBPMNum[ zz ];
					// (6-2-7) フィルイン系チャンネルならポジションを修正する 
					else if( nCh == 0x53 )
					{
						if( zz > 0 && zz != 2 )				// フィルイン開始 → ポジションを 32 だけ前にずらす
							cc->dwPosition -= 32;
						else if( zz == 2 )					// フィルイン終了 → ポジションを 32 だけ後にずらす
							cc->dwPosition += 32;
					}
					// (6-2-8) チップの追加 
					{
						// InsertChip() は cc->dwPosition でソートするため、cc->dwPosition の修正が完了した後に呼び出すこと。
						this->InsertChip( cc );
					}

					p += 2;		// 次のオブジェクトへ	
				}
			}
		}
	}

	SAFE_FREE( strLine );

	// (3) LoadDTX() だけの後処理 
	{
		// (3-1) 開始BPMチップを追加する。
		{
			// Chipリストの先頭に、#00008:00 のチップを追加する。（#BPM: を曲の頭で有効にするため） 
			BPM* pb;
			for( pb = this->pBPMLast; pb != NULL; pb = pb->prev )	// #BPM: が複数ある場合は後置優先
				if( pb->vnum == 0 )	break;
		
			// (A) #BPM: が存在しない → #BPM00:120 を BPMリストの末尾に追加し、そのBPM実番号を持った Chip を追加する。 
			if( pb == NULL )
			{
				pb = new BPM();
				pb->num		= ++ m_nBPMNumCur;
				pb->vnum	= 0;
				pb->bpm		= 120.0;
				pb->prev = pb->next = NULL;
				APPENDLIST( this->pBPM, this->pBPMLast, pb );
				
				Chip *cc = new Chip();
				cc->dwPosition	= 0;
				cc->nChannel	= 0x08;
				cc->nParam		= 0;
				cc->nRealParam	= pb->num;	// #BPM00: の BPM 実番号
				this->InsertChip( cc );
			}
			// (B) #BPM: が存在する → そのBPM実番号を持ったChipを追加する。 
			else
			{
				Chip *cc = new Chip();
				cc->dwPosition	= 0;
				cc->nChannel	= 0x08;
				cc->nParam		= 0;
				cc->nRealParam	= pb->num;	// #BPM00: の BPM 実番号
				this->InsertChip( cc );
			}
		}
		// (3-2) WAVVOL と WAVPAN の省略されている Wave についてデフォルト値を適用する。 
		{
			for( Wave* wp = this->pWave; wp != NULL; wp = wp->next )
			{
				if( wp->volume < 0 )
					wp->volume = 100;
				if( wp->pan < -10000 )
					wp->pan = 0;
			}
		}
		// (3-3) 空打ち指定がある場合、最初の空打ち指定チップを曲の先頭へ複写する。 
		{
			for( int i = 0xB1; i <= 0xBB; i++ )
			{
				for( Chip* c = this->pChip; c != NULL; c = c->next )
				{
					if( c->nChannel == i ) 
					{
						Chip *cc = new Chip();
						cc->dwPosition	= 0;		// 先頭
						cc->nChannel	= c->nChannel;
						cc->nParam		= c->nParam;
						cc->nRealParam	= c->nRealParam;
						this->InsertChip( cc );
						break;
					}
				}
			}
		}
		// (3-4) 初期BMP画像(#BMP00:)がある場合、BMPチップを曲の先頭に挿入する。 
		{
			for( BMP* pBmp = this->pBMPLast; pBmp != NULL; pBmp = pBmp->prev )
			{
				if( pBmp->num == 0 )
				{
					Chip *cc = new Chip();
					cc->dwPosition	= 0;			// 先頭
					cc->nChannel	= 0x04;			// ch.04
					cc->nParam		= 0;
					cc->nRealParam	= 0;
					this->InsertChip( cc );
					break;
				}
			}
		}
	}

	fclose( fp );
}
void DTX::InsertChip( Chip *cell )
{
	// 表：同位置の場合のチャンネル間の優先順位（小さい数字ほど前に挿入される） 
	static const UCHAR byPriority[ 256 ] = {
		5,5,3,3,5,5,5,5,3,5,5,5,5,5,5,5,	// 0x	小節線変更、BPM は優先
		5,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,	// 1x	チップは後ろへ（後ろのものほど上位描画）
		7,7,7,7,7,7,7,7,5,5,5,5,5,5,5,5,	// 2x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 3x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 4x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 5x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 6x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 7x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 8x
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// 9x
		7,7,7,7,7,7,7,7,5,5,5,5,5,5,5,5,	// Ax
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// Bx
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// Cx
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// Dx
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,	// Ex
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5		// Fx
	};

	// (1) 最初のセルの場合 → リストの先頭に追加。 
	if( this->pChipLast == NULL ) {
		this->pChip = this->pChipLast = cell;
		return;
	}
	// (2) 適切な場所をリスト「最後尾」から検索し、挿入する。 
	for( Chip *t = this->pChipLast; t != NULL; t = t->prev )
	{
		if( t->dwPosition < cell->dwPosition ||
			( t->dwPosition == cell->dwPosition	&& byPriority[ t->nChannel ] <= byPriority[ cell->nChannel ] ) )
		{
			// t の直後に挿入する
			cell->prev = t;
			cell->next = t->next;
			if( t->next == NULL ) {
				t->next = cell;
				this->pChipLast = cell;
			} else {
				t->next->prev = cell;
				t->next = cell;
			}
			return;	// 完了
		}
	}
	// (3) 先頭まで達した → リストの先頭に挿入する。 
	cell->prev = NULL;
	cell->next = this->pChip;
	if( this->pChip != NULL ) {
		this->pChip->prev = cell;
		this->pChip = cell;
	} else {
		this->pChip = this->pChipLast = cell;
	}
}
void DTX::InsertMIDIChip( Chip *cell )
{
	// (1) 最初のセルの場合 → リストの先頭に追加。 
	if( this->pChipLast == NULL )
	{
		this->pChip = this->pChipLast = cell;
		return;
	}
	// (2) 適切な場所をリストの「最後尾」から検索し、挿入する。 
	for( Chip *t = this->pChipLast; t != NULL; t = t->prev )
	{
		// 位置が同じの場合には、0x11～0x1A は後ろに持っていく。（後で描画される→より上に描画される）
		if( t->dbTime < cell->dbTime || ( t->dbTime == cell->dbTime && ( cell->nChannel >= 0x11 && cell->nChannel <= 0x1A ) ) )
		{
			// t → cell → ... の順になるよう挿入する
			cell->prev	= t;
			cell->next	= t->next;
			if (t->next == NULL) {
				t->next		 = cell;
				this->pChipLast = cell;
			} else {
				t->next->prev = cell;
				t->next		  = cell;
			}
			return; // 完了
		}
	}
	// (3) 先頭まで達した → リストの先頭に挿入する。 
	cell->prev = NULL;
	cell->next = this->pChip;
	if( this->pChip->prev != NULL ) {
		this->pChip->prev = cell;
		this->pChip = cell;
	} else {
		this->pChip = this->pChipLast = cell;
	}
}
void DTX::InsertLines()
{
	if( !this->pChip || !this->pChipLast )
		return;		// チップがない

	double	dbBai = 1.0;	// 小節長倍率
	DWORD	dwLast;			// 最後のチップが存在する小節の、その次の小節の先頭位置
	dwLast  = this->pChipLast->dwPosition;
	dwLast += DTX_RESOLVE - ( dwLast % DTX_RESOLVE );

	// (1) 小節線の挿入 
	{
		for( DWORD pos = 0; pos <= dwLast; pos += DTX_RESOLVE )
		{
			Chip *nc = new Chip();
			nc->dwPosition	= pos;
			nc->nChannel	= 0x50;
			nc->nParam		= 36*36-1;		// システムが置いた線は nParam = zz
			InsertChip( nc );
		}
	}
	// (2) 拍線の挿入 
	{
		Chip *p02 = this->pChip;	// for ch.02（小節長倍率変更）
		Chip *pC1 = this->pChip;	// for ch.C1（拍線シフト；02 を置くと、その小節内に限り、そこから拍線が始まる）
		
		for( DWORD pos = 0; pos < dwLast; pos += DTX_RESOLVE )		// pos は小節単位で増加
		{
			// 拍線シフト；
			// この小節内（pos ～ pos+DTX_RESOLVE の範囲内）に ch.C1セル があるかどうかを、先読みして確認する。
			// あったら、dw開始位置 にその相対位置（小節アタマからの距離[ct]）を格納しておく。
			// なければ、dw開始位置 は 0 。
			
			DWORD dw開始位置 = 0;

			while( pC1 && pC1->dwPosition < pos + DTX_RESOLVE )
			{
				if( pC1->nChannel == 0xC1 )
					dw開始位置 = pC1->dwPosition - pos;
				pC1 = pC1->next;
			}

			// 小節長倍率変更その①； BMSでの自然復帰
			// BMS,BME では、１小節ごとに小節長倍率が 1.0 に戻る。

			if( this->scoreType == SCORETYPE_BMS )
				dbBai = 1.0;

			// 小節長倍率変更その②； Ch.02 での変更があれば対応する。
			while( p02 && p02->dwPosition <= pos )	// Ch.02 は小節頭にしか現れない（＝ dwPosition が必ず DTX_RESOLVE で割り切れる）
			{
				if( p02->nChannel == 0x02 )
					dbBai = p02->dbParam;
				p02 = p02->next;
			}

			// 拍線の挿入

			for( int n拍線連番 = 0; n拍線連番 < 100; n拍線連番++ )	// < 100 は無限ループ防止の保険
			{
				DWORD dw拍線位置 = (DWORD)( ( DTX_RESOLVE * n拍線連番 ) / ( 4.0 * dbBai ) );
				
				// 位置が小節を越えたら抜ける
				if( dw拍線位置 + dw開始位置 >= DTX_RESOLVE )
					break;

				// 拍線チップの作成と挿入
				if( ( dw拍線位置 + dw開始位置 ) % DTX_RESOLVE != 0 )	// 小節線と同じ場所には置かない
				{
					Chip *nc = new Chip();
					nc->dwPosition	= pos + dw拍線位置 + dw開始位置;
					nc->nChannel	= 0x51;
					nc->nParam		= 36*36-1;	// システムが置いた線は nParam = zz
					InsertChip( nc );
				}
			}
		}
	}
	// (3) 拍線・小節線の非表示指示(ch.C2)があるなら対応する。 
	{
		bool b線が可視 = true;

		// すべてのチップについて...
		for( Chip* cp = this->pChip; cp != NULL; cp = cp->next )
		{
			// 同じ position に複数のチップがある場合は、ch.C2 を優先的に読む。
			for( Chip* p同位置Chip = cp; p同位置Chip && p同位置Chip->dwPosition == cp->dwPosition; p同位置Chip = p同位置Chip->next )
			{
				if( p同位置Chip->nChannel == 0xC2 && p同位置Chip->nParam == 1 ) b線が可視 = true;
				if( p同位置Chip->nChannel == 0xC2 && p同位置Chip->nParam == 2 ) b線が可視 = false;
			}
			
			// その次に、拍線・小節線の可視属性を設定する。ただし、ユーザが置いたもの（nParam!=ZZ のもの）は影響を受けない。
			if( ( cp->nChannel == 0x50 || cp->nChannel == 0x51 ) && cp->nParam == 36*36-1 )
				cp->bVisible = b線が可視;
		}
	}
}
void DTX::CalcChipTime()
{
	// MID 形式は算出不要（DrumMIDI 側ですでに時刻が計算済み）
	if( this->scoreType == SCORETYPE_MID )
		return;

	double db現在のBPM			= 120.0;	// [拍/分]
	double db現在の小節長倍率	= 1.0;
	DWORD  dw基準カウント値		= 0;		// db現在の小節長倍率, db現在のBPM の最後に変化したカウント値（１小節＝ DTX_RESOLVE カウントで固定）
	double db基準時刻			= 0;		// 　　　　　　　　　　　〃　　　　　　　　　　　　時刻[ms]
	DWORD  dw前小節				= 0;		// 小節長変更用

	// すべてのチップについて...
	for( Chip* cell = this->pChip; cell != NULL; cell = cell->next )
	{
		// (1) 発声時刻の計算 → cell->dbTime 
		cell->dbTime = (DWORD)( db基準時刻 + ((240000 / DTX_RESOLVE) * (cell->dwPosition - dw基準カウント値) * db現在の小節長倍率) / (db現在のBPM * this->fSpeed) );

		// (2) 小節が変わったら、小節長を 1.0 に戻す。(BMSのみ) 
		if( this->scoreType == SCORETYPE_BMS && db現在の小節長倍率 != 1.0 && (cell->dwPosition / DTX_RESOLVE) != dw前小節 )
		{
			dw基準カウント値	= cell->dwPosition;
			db基準時刻			= cell->dbTime;
			db現在の小節長倍率	= 1.0;
		}
		dw前小節 = cell->dwPosition / DTX_RESOLVE;

		// (3) 以下、チャンネルごとの個別処理
		switch( cell->nChannel )
		{
		// (3-1) 小節長変更 
		case 0x02: 
			{
				dw基準カウント値	= cell->dwPosition;
				db基準時刻			= cell->dbTime;
				db現在の小節長倍率	= cell->dbParam;
			}
			break;

		// (3-2) BPM変更(1)整数BPM 
		case 0x03: 
			{
				dw基準カウント値	= cell->dwPosition;
				db基準時刻			= cell->dbTime;
				db現在のBPM			= this->dbBaseBPM + (double)cell->nParam;
			}
			break;

		// (3-3) BPM変更(2)実数BPM 
		case 0x08: 
			{
				dw基準カウント値 = cell->dwPosition;
				db基準時刻		 = cell->dbTime;

				for( BPM *bc = this->pBPMLast; bc != NULL; bc = bc->prev )
				{
					if( bc->num == cell->nRealParam )
					{
						db現在のBPM = ( ( bc->vnum == 0 ) ? 0 : this->dbBaseBPM ) + bc->bpm;	// bc->vnum = 0 のとき（#BPM:のとき）は BaseBPM は加えない！
						break;
					}
				}
			}
			break;

		// (3-4) BGAPAN 
		case 0x04: case 0x07:
		case 0x55: case 0x56: case 0x57: case 0x58: case 0x59: case 0x60:
			{
				for( BGAPan *bp = this->pBGAPanLast; bp != NULL; bp = bp->prev )
				{
					if( bp->num == cell->nParam )
					{
						double st = db基準時刻 + ((240000 / DTX_RESOLVE) * (cell->dwPosition           - dw基準カウント値) * db現在の小節長倍率) / db現在のBPM;
						double et = db基準時刻 + ((240000 / DTX_RESOLVE) * (cell->dwPosition + bp->len - dw基準カウント値) * db現在の小節長倍率) / db現在のBPM;
						cell->dbLong = et - st;
						break;
					}
				}
			}
			break;

		// (3-5) AVIPAN 
		case 0x54:
			{
				for( AVIPan *ap = this->pAVIPanLast; ap != NULL; ap = ap->prev )
				{
					if( ap->num == cell->nParam )
					{
						double st = db基準時刻 + ((240000 / DTX_RESOLVE) * (cell->dwPosition           - dw基準カウント値) * db現在の小節長倍率) / db現在のBPM;
						double et = db基準時刻 + ((240000 / DTX_RESOLVE) * (cell->dwPosition + ap->len - dw基準カウント値) * db現在の小節長倍率) / db現在のBPM;
						cell->dbLong = et - st;
						break;
					}
				}
			}
			break;
		}
	}
}
void DTX::AdujstChipTimeByPlaySpeed( double dbPlaySpeed )
{
	if( dbPlaySpeed <= 0 )
		return;

	// すべてのチップの発声時刻を修正
	for( Chip* p = this->pChip; p != NULL; p = p->next )
		p->dbTime = p->dbTime / dbPlaySpeed;
}
// BMP, AVI, WAV の読み込み
void DTX::LoadBMP()
{
	this->dwBMPWidthMax		= this->dwBMPHeightMax		= -1;
	this->dwBMPTEXWidthMax	= this->dwBMPTEXHeightMax	= -1;

	// (1) 各マネージャのキャッシュ内のフラグをリセット 
	this->textureManager.ResetCache();
	this->surfaceManager.ResetCache();

	// (2) BMPTEX の読み込み；VRAM 優先のため、BMP より先に読み込む。 
	{
		int nテクスチャ生成数 = 0;

		for( BMPTEX *btc = this->pBMPTEX; btc != NULL; btc = btc->next )
		{
			// #PATH_WAV の指定がある場合はそこから、それ以外は DTX のある場所から読み込む。
			tstring path = ( this->strPathWav.empty() ) ? this->strFolder : this->strPathWav;
			path += btc->strFileName;

			// テクスチャの生成
			TCHAR name[ 32 ];
			_stprintf_s( name, 32, _T("BMPTEX(%d)画像"), btc->num );
			if( ( btc->pTexture = this->textureManager.GetCachedTexture( name, path.c_str(), 0, 0 ) ) == NULL )
				continue;	// 作成に失敗
			btc->bUse = true;

			// 最大幅・高さの更新
			if( btc->pTexture->dwWidth  > this->dwBMPTEXWidthMax ) this->dwBMPTEXWidthMax  = btc->pTexture->dwWidth;
			if( btc->pTexture->dwHeight > this->dwBMPTEXHeightMax) this->dwBMPTEXHeightMax = btc->pTexture->dwHeight;

			nテクスチャ生成数 ++;
		}
	}
	// (3) BMP の読み込み 
	{
		int nサーフェイス生成数 = 0;

		for( BMP *bc = this->pBMP; bc != NULL; bc = bc->next )
		{
			// #PATH_WAV の指定がある場合はそこから、それ以外は DTX のある場所から読み込む。
			tstring path = ( this->strPathWav.empty() ) ? this->strFolder : this->strPathWav;
			path += bc->strFileName;

			// サーフェイスの生成
			TCHAR name[ 32 ];
			_stprintf_s( name, 32, _T("BMP(%d)画像"), bc->num );
			if( ( bc->pSurface = this->surfaceManager.GetCachedSurface( name, path.c_str(), 0, 0, LOADTO_VIDEO_OR_SYSTEM ) ) == NULL )
				continue;	// 作成に失敗
			bc->bUse = true;

			// 最大幅・高さの更新
			if( bc->pSurface->dwWidth  > this->dwBMPWidthMax ) this->dwBMPWidthMax  = bc->pSurface->dwWidth;
			if( bc->pSurface->dwHeight > this->dwBMPHeightMax) this->dwBMPHeightMax = bc->pSurface->dwHeight;

			nサーフェイス生成数 ++;
		}
	}
	// (4) 各マネージャのキャッシュから不要なリソースを削除 
	this->surfaceManager.RemoveUnusedSurfaces();
	this->textureManager.RemoveUnusedTextures();

	
	// ヘッダ読み込みのみならここで終了
	if( this->bHeaderOnly ) return;

	// (5) BMP, BMPTEX, BGA, BGAPAN の事前最適化 
	for( Chip *c = this->pChip; c != NULL; c = c->next )
	{
		// (5-1) BGAレイヤチャネルチップに、BMP/BMPTEX/BGA/BGAPANへのポインタを割り当てる。 
		if( c->nChannel == 0x04 || c->nChannel == 0x07 || ( c->nChannel >= 0x55 && c->nChannel <= 0x59 ) || c->nChannel == 0x60 )
		{
			// 初期化 
			c->BGAtype	= BGATYPE_UNKNOWN;
			c->pBMP		= NULL;
			c->pBMPTEX	= NULL;
			c->pBGA		= NULL;
			c->pBGAPan	= NULL;

			// (5-1-1) BGAPAN から検索し、あれば処理する。 
			for( BGAPan *bgapan = this->pBGAPanLast; bgapan != NULL; bgapan = bgapan->prev )
			{
				if( bgapan->num == c->nParam )
				{
					BMPTEX* bmptex;
					for( bmptex = this->pBMPTEXLast; bmptex != NULL; bmptex = bmptex->prev )
					{
						if( bmptex->num == bgapan->bmp && bmptex->bUse )
						{
							c->BGAtype	= BGATYPE_BGAPAN;
							c->pBMPTEX	= bmptex;
							c->pBGAPan	= bgapan;
							break;
						}
					}
					if( bmptex ) break;
					BMP* bmp;
					for( bmp = this->pBMPLast; bmp != NULL; bmp = bmp->prev )
					{
						if( bmp->num == bgapan->bmp && bmp->bUse )
						{
							c->BGAtype	= BGATYPE_BGAPAN;
							c->pBMP		= bmp;
							c->pBGAPan	= bgapan;
							break;
						}
					}
					if( bmp ) break;
				}
			}
			if( c->BGAtype != BGATYPE_UNKNOWN )
				continue;

			// (5-1-2) BGA から検索し、あれば処理する。 
			for( BGA *bga = this->pBGALast; bga != NULL; bga = bga->prev ) {
				if( bga->num == c->nParam ) 	{
					BMPTEX* bmptex;
					for( bmptex = this->pBMPTEXLast; bmptex != NULL; bmptex = bmptex->prev ) {
						if( bmptex->num == bga->bmp && bmptex->bUse ) {
							c->BGAtype	= BGATYPE_BGA;
							c->pBMPTEX	= bmptex;
							c->pBGA		= bga;
							break;
						}
					}
					if( bmptex ) break;
					BMP* bmp;
					for( bmp = this->pBMPLast; bmp != NULL; bmp = bmp->prev ) {
						if( bmp->num == bga->bmp && bmp->bUse ) {
							c->BGAtype	= BGATYPE_BGA;
							c->pBMP		= bmp;
							c->pBGA		= bga;
							break;
						}
					}
					if( bmp ) break;
				}
			}
			if( c->BGAtype != BGATYPE_UNKNOWN )
				continue;

			// (5-1-3) BMPTEX から検索し、あれば処理する。 
			for( BMPTEX* bmptex = this->pBMPTEXLast; bmptex != NULL; bmptex = bmptex->prev ) {
				if( bmptex->num == c->nParam && bmptex->bUse ) {
					c->BGAtype	= BGATYPE_BMPTEX;
					c->pBMPTEX	= bmptex;
					break;
				}
			}
			if( c->BGAtype != BGATYPE_UNKNOWN )
				continue;

			// (5-1-4) BMP から検索し、あれば処理する。 
			for( BMP* bmp = this->pBMPLast; bmp != NULL; bmp = bmp->prev ) {
				if( bmp->num == c->nParam && bmp->bUse ) {
					c->BGAtype	= BGATYPE_BMP;
					c->pBMP		= bmp;
					break;
				}
			}
		}
		// (5-2) DTX::Chip の BGAスコープ画像切替チャンネルセルに、BMP/BMPTEX へのポインタを割り当てる。 
		if( c->nChannel == 0xC4 || c->nChannel == 0xC7 || ( c->nChannel >= 0xD5 && c->nChannel <= 0xD9 ) || c->nChannel == 0xE0 )
		{
			// 初期化 
			c->BGAtype	= BGATYPE_UNKNOWN;
			c->pBMP		= NULL;
			c->pBMPTEX	= NULL;
			c->pBGA		= NULL;
			c->pBGAPan	= NULL;

			// (5-2-1) BMPTEX から検索し、あれば処理する。 
			for( BMPTEX* bmptex = this->pBMPTEXLast; bmptex != NULL; bmptex = bmptex->prev )
			{
				// ※BGAスコープ画像切替チャンネルの場合、nParam は "BMP番号" であり、BGA, BGAPAN の番号は無関係である。
				if( bmptex->num == c->nParam && bmptex->bUse )
				{
					c->BGAtype	= BGATYPE_BMPTEX;
					c->pBMPTEX	= bmptex;
					break;
				}
			}
			if( c->BGAtype != BGATYPE_UNKNOWN )
				continue;

			// (5-2-2) BMP から検索し、あれば処理する。 
			for( BMP* bmp = this->pBMPLast; bmp != NULL; bmp = bmp->prev )
			{
				// ※BGAスコープ画像切替チャンネルの場合、nParam は "BMP番号" であり、BGA, BGAPAN の番号は無関係である。
				if( bmp->num == c->nParam && bmp->bUse ) {
					c->BGAtype	= BGATYPE_BMP;
					c->pBMP		= bmp;
					break;
				}
			}
		}
	}
}
void DTX::LoadAVI()
{
	// (1) AVI のオープン 
	int nAVIオープン数 = 0;
	for( AVI *ac = this->pAVI; ac != NULL; ac = ac->next )
	{
		// #PATH_WAV の指定がある場合はそこから、それ以外は DTX のある場所から読み込む。
		tstring path = ( this->strPathWav.empty() ) ? this->strFolder : this->strPathWav;
		path += ac->strFileName;

		// AVI をオープン
		ac->avi._初期化();
		ac->bUse = ( SUCCEEDED( ac->avi.Open( path ) ) ) ? true : false;		// open に成功したら true

		if( ac->bUse )
			nAVIオープン数 ++;
		else
			ac->avi.Close();	// 失敗
	}

	// ヘッダ読み込みのみならここで終了
	if( this->bHeaderOnly ) return;

	// (2) AVIPAN の事前最適化 
	for( Chip *c = this->pChip; c != NULL; c = c->next )
	{
		// AVIチャネルチップに、AVI/AVIPANへのポインタを割り当てる。
		if( c->nChannel == 0x54 ) 
		{
			// 初期化 
			c->AVItype	= AVITYPE_UNKNOWN;
			c->pAVI		= NULL;
			c->pAVIPan	= NULL;

			// (1) AVIPAN から検索し、あれば処理する。 
			for( AVIPan *avipan = this->pAVIPanLast; avipan != NULL; avipan = avipan->prev )
			{
				if( avipan->num == c->nParam )
				{
					AVI* avi;
					for( avi = this->pAVILast; avi != NULL; avi = avi->prev )
					{
						if( avi->num == avipan->avi && avi->bUse )
						{
							c->AVItype	= AVITYPE_AVIPAN;
							c->pAVI		= avi;
							c->pAVIPan	= avipan;
							break;
						}
					}
					if( avi ) break;
				}
			}
			if( c->AVItype != AVITYPE_UNKNOWN )
				continue;

			// (2) AVI から検索し、あれば処理する。 
			for( AVI* avi = this->pAVILast; avi != NULL; avi = avi->prev )
			{
				if( avi->num == c->nParam && avi->bUse )
				{
					c->AVItype	= AVITYPE_AVI;
					c->pAVI		= avi;
					break;
				}
			}
		}
	}
}
void DTX::LoadWAV()
{
	// (1) サウンドマネージャのキャッシュのリセット 
	this->soundManager.t使用フラグをクリアし複製セルを削除する();

	// (2) 全サウンドを読み込む 
	for( Wave *wc = this->pWave; wc != NULL; wc = wc->next )
	{
		// path を決定；#PATH_WAV の指定がある場合はそこから、それ以外は DTX のある場所から読み込む。 
		tstring path = ( this->strPathWav.empty() ) ? this->strFolder : this->strPathWav;
		path += wc->strFileName;

		// サウンドを使用するレーンごとに生成する
		for( int i = 0; i < 11; i++ )
		{
			if( ! wc->bUseByLane[i] ) continue;		// このレーンで使用されないならスキップ

			// １レーンに付きMAX_SOUND 個のバッファを作成する
			for( int j = 0; j < DTX_MAX_SOUND; j++ )
			{
				// サウンドを生成する
				if( ( wc->sound[i][j] = this->soundManager.pキャッシュ対応サウンドを作成して返す( path.c_str() ) ) == NULL )
				{
					wc->bUse = false;	// １個でも失敗したらこのWAV自体使わない
					break;
				}
				wc->bUse = true;

				// 音量の初期化
				wc->sound[i][j]->SetVolume( 100 );

				// 総演奏時間[ms]の計算
				wc->dwBufferSize = wc->sound[i][j]->GetDirectSoundBufferSize();
				DWORD dwSize;
				wc->sound[i][j]->GetDirectSoundBuffer()->GetFormat( NULL, 0, &dwSize );
				LPWAVEFORMATEX pWF = (LPWAVEFORMATEX) malloc( dwSize );
				wc->sound[i][j]->GetDirectSoundBuffer()->GetFormat( pWF, dwSize, NULL );
				wc->dbTotalTime = (double)( wc->dwBufferSize / ( pWF->nAvgBytesPerSec * 0.001 ) );
				SAFE_FREE( pWF );
			}
			if( ! wc->bUse )
				break;	// 読み込みに失敗したので抜ける
		}
	}
	// (3) サウンドマネージャのキャッシュから未使用サウンドを削除する。 
	this->soundManager.t未使用サウンドを削除する();
}
// 途中からの再生開始
void DTX::SkipStart( double dbStartTime )
{
	// Wave に関連付けられるチップは 1, BMP関係は 2。
	// ただしスキップ再生しないときは 0 にしておく。
	// 空打ち指定など、発声されないWAV関係は無視。
	static const char bTarget[ 256 ] = {
		0,1,0,0,2,0,0,2,0,0,0,0,0,0,0,0,	// 0x	01: BGM / 04,07: BGA
		0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,	// 1x	11～1A: Drums
		1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,	// 2x	20～27: Guitar
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// 3x
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// 4x
		0,0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,	// 5x	55～59,60: BGA
		2,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,	// 6x	61～69,
		1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,	// 7x	70～79,
		1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,	// 8x	80～89,
		1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,	// 9x	90～92: BGM
		1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,	// ax	a0～a7: Bass
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// bx
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// cx
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// dx
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// ex
		0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};	// fx

	for( Chip* cell = this->pChip; cell != NULL; cell = cell->next )
	{
		// 開始時刻より後ろなら、そのチップで調査打ち切り。
		if( cell->dbTime > dbStartTime ) 
			break;

		// 処理分岐
		switch( bTarget[ cell->nChannel % 0xff ] )
		{
		case 1:
			SkipStartWave( dbStartTime, cell );		// WAVE のスキップ再生
			break;

		case 2:
			SkipStartBGA( dbStartTime, cell );		// BGA のスキップ再生
			break;
		}
	}
}

void DTX::SkipStartWave( double dbStartTime, Chip* cell )
{
	// (1) レーン番号算出 → l 
	int ch = cell->nChannel;
	int l;
	if( ch >= 0x11 && ch <= 0x1A )
		l = s_nChannelToLane[ ch - 0x11 ];		// Drums

	else if( ch >= 0x20 && ch <= 0x27 )
		l = 8;	// Guitar

	else if( ch >= 0xA0 && ch <= 0xA7 )
		l = 9;	// Bass

	else if( ch == 0x01 || (ch >= 0x61 && ch <= 0x69) || (ch >= 0x70 && ch <= 0x79) || (ch >= 0x80 && ch <= 0x89) || (ch >= 0x90 && ch <= 0x92) )
		l = 10;	// BGM

	else		
		return;	// それ以外（発声なし）

	// (2) 対応するWaveを検索 → wc 
	Wave* wc;
	for( wc = this->pWave; wc != NULL; wc = wc->next )
	{
		if( wc->num == cell->nRealParam && wc->bUse )
			break;
	}
	if( wc == NULL )
		return;		// 対応するWaveがなかった
	if( ! wc->bUseByLane[ l ] )
		return;		// このレーンでは再生しない
	if( cell->dbTime + ( wc->dbTotalTime / this->fSpeed ) <= dbStartTime )
		return;		// 再生中じゃない

	// (3) 指定位置に移動し、即座に再生を開始する。 
	if( ++ wc->nCurSound[ l ] >= DTX_MAX_SOUND )
		wc->nCurSound[ l ] = 0;
	if( wc->sound[l][ wc->nCurSound[l] ] )
	{
		wc->sound[l][ wc->nCurSound[l] ]->SetVolume( (long)((DTX_AUTOVOLUME * wc->volume)/100.0) );
		wc->sound[l][ wc->nCurSound[l] ]->SetPan( wc->pan );
		wc->sound[l][ wc->nCurSound[l] ]->Play();
		wc->sound[l][ wc->nCurSound[l] ]->SetPosition( wc->sound[l][ wc->nCurSound[l] ]->GetPositionFromTime( dbStartTime - cell->dbTime ) );
	}
	wc->dbStartTime[l][ wc->nCurSound[l] ] = cell->dbTime;		// 再生開始時刻
	wc->bPlaying[l][ wc->nCurSound[l] ] = true;
}
void DTX::SkipStartBGA( double dbStartTime, Chip* cell )
{
	// 実装は BGA モジュール（アプリ側）に依存するため、ここでは何もしない。アプリ側で実装すること。
}
// チップ(WAV)の再生、停止
void DTX::PlayChip( Chip* pChip, int nLane, long lVol, bool bMIDIMonitor, bool bBad )
{
	// (0) 事前チェック 
	if( this->pTimer == NULL ) return;
	if( pChip == NULL ) return;
	if( pChip->nRealParam < 0 ) return;
	if( nLane < 0 || nLane > 10 ) return;

	// (1) リストの末尾から検索し、再生する（同一番号での重複登録がある場合、後方の設定が優先される） 
	for( Wave *wc = this->pWaveLast; wc != NULL; wc = wc->prev )
	{
		if( wc->num == pChip->nRealParam && wc->bUse && wc->bUseByLane[ nLane ] )
		{
			// (1-1) サウンドローテーション
			if( ++ wc->nCurSound[ nLane ] >= DTX_MAX_SOUND )
				wc->nCurSound[ nLane ] = 0;

			// (1-2) 再生
			int s = wc->nCurSound[ nLane ];
			if( wc->sound[ nLane ][ s ] )
			{
				wc->sound[ nLane ][ s ]->SetVolume( (long)((lVol * wc->volume)/100.0) );
				wc->sound[ nLane ][ s ]->SetPan( wc->pan );

				if( ! bBad )
				{
					wc->sound[ nLane ][ s ]->SetPitch( 1.0f );
					wc->sound[ nLane ][ s ]->Play();
				}
				else
				{
					// BAD は周波数変調
					wc->sound[ nLane ][ s ]->SetPitch( ( 100+ (((rand()%3)+1)*7) * (1-(rand()%2)*2) )  /100.0f );
					wc->sound[ nLane ][ s ]->Play();
				}
			}
			wc->dbStartTime[ nLane ][ s ] = this->pTimer->db前回Resetした時刻() + pChip->dbTime;	// 再生開始時刻 … 現在時刻ではなく、Chip::dbTime に一致させる(FPS遅延の防止)(2007.6.9)
			wc->bPlaying[ nLane ][ s ] = true;

			// (1-3) pChip->dbTime と実時間との差を埋めるため、いきなりWAVE再生位置修正。
			this->AdjustWave( wc, wc->dbStartTime[ nLane ][ s ], wc->sound[ nLane ][ s ] );

			break;
		}
	}
	// (2) MIDI Note ON の場合は MIDI ノートとしても発声（DTXV では未対応(2004.01.12)） 
	if( ! this->bDTXV )
	{
		DWORD dwMsg = pChip->message.dwMsg;
		if( this->bMIDINote && dwMsg != 0 )
		{
			if( bMIDIMonitor )								// bMonitor = true の時は
				dwMsg = (dwMsg & 0x00FFFF) | 0x7F0000;		// 音量を127にする
			midiOutShortMsg( this->hMidiOut, dwMsg );
		}
	}
}
void DTX::StopWave( int nWave, int nLane )
{
	if( nLane < 0 || nLane > 10 ) return;

	for( Wave *wc = this->pWaveLast; wc != NULL; wc = wc->prev )
	{
		if( wc->num == nWave && wc->bUse && wc->bUseByLane[ nLane ] )
		{
			for( int i = 0; i < DTX_MAX_SOUND; i++ ) {
				if( wc->sound[ nLane ][ i ] )
					wc->sound[ nLane ][ i ]->Stop();
				wc->bPlaying[ nLane ][ i ] = false;
			}
			wc->nCurSound[ nLane ] = -1;
			break;
		}
	}
}

void DTX::StopAllWaves()
{
	for( Wave* pw = this->pWave; pw != NULL; pw = pw->next ) {
		for( int i = 0; i < 11; i++ ) {
			for( int j = 0; j < DTX_MAX_SOUND; j++ ) {
				if( pw->bPlaying[i][j] && pw->sound[i][j] )
					pw->sound[i][j]->Stop();
			}
		}
	}
}

void DTX::PauseWaves()
{
	for( Wave *wc = this->pWaveLast; wc != NULL; wc = wc->prev ) {
		if( wc->bUse ) {
			for( int j = 0; j < 11; j++ ) {
				if( wc->bUseByLane[j] ) {
					for( int i = 0; i < DTX_MAX_SOUND; i++ ) {
						if( wc->sound[ j ][ i ] )
							wc->sound[ j ][ i ]->Pause();
						wc->bPlaying[ j ][ i ] = false;
						wc->dbPauseTime[ j ][ i ] = this->pTimer->GetSysTime();	// 実時間
					}
				}
			}
		}
	}
}

void DTX::ContWaves()
{
	for( Wave *wc = this->pWaveLast; wc != NULL; wc = wc->prev ) {
		if( wc->bUse ) {
			for( int j = 0; j < 11; j++ ) {
				if( wc->bUseByLane[j] ) {
					for( int i = 0; i < DTX_MAX_SOUND; i++ ) {
						if( wc->sound[ j ][ i ] ) {
							wc->sound[ j ][ i ]->Cont( wc->dbPauseTime[j][i] - wc->dbStartTime[j][i] );	// Pause() されてない場合は何も影響なし
							wc->dbStartTime[j][i] += this->pTimer->GetSysTime() - wc->dbPauseTime[j][i];
						}
						wc->bPlaying[ j ][ i ] = true;
					}
				}
			}
		}
	}
}

//
void DTX::AdjustWaves()
{
	for( Wave *wc = this->pWave; wc != NULL; wc = wc->next )
	{
		if( wc->bUse )
		{
			for( int j = 0; j < 11; j++ )
			{
				if( ! wc->bUseByLane[j] )
					continue;

				for( int i = 0; i < DTX_MAX_SOUND; i++ )
				{
					if( ! wc->sound[j][i] || ! wc->sound[j][i]->IsPlay() ) {
						wc->bPlaying[j][i] = false;		// 再生中じゃない
						continue;
					}

					// 再生位置補正
					this->AdjustWave( wc, wc->dbStartTime[j][i], wc->sound[j][i] );
				}
			}
		}
	}
}
void DTX::AdjustWave( DTX::Wave* pWave, double dbStartTime, CSound* pSound )
{
	if( !pWave || dbStartTime < 0.0 || !pSound)
		return;

	// 総演奏時間が 5000ms 以上の音のみ調整する 
	if( pWave->dbTotalTime < 5000.0 )
		return;

	// ※サウンドの再生はCPUとは無関係に進んでいくので、
	//   全サウンドで１つの dbNow じゃなく、１つ１つのサウンドごとに g_Timer.GetSysTime() して比べる必要がある。(2007.6.9)
	double dbNow = this->pTimer->GetSysTime();

	if( dbNow > dbStartTime )
	{
		// ※ this->fSpeed は CSound::GetPositionFromTime() 内で考慮されるのでここではいらない。
		double dbPos = ( dbNow - dbStartTime ) * 1/*this->fSpeed*/;
		pSound->SetPosition( pSound->GetPositionFromTime( dbPos ) );
	}
}
//
void DTX::SetVolume( int nWave, long lVolume, int nLane )
{
	if( nLane < 0 || nLane > 10 ) return;

	for( Wave *wc = this->pWaveLast; wc != NULL; wc = wc->prev )		// リストの末尾から検索... 同一番号での重複登録がある場合、後方の設定が優先される。
	{
		if( wc->num == nWave && wc->bUse && wc->bUseByLane[ nLane ] )
		{
			int next = wc->nCurSound[ nLane ] + 1;					// 次に発声すべきサウンドについて設定
			if( next >= DTX_MAX_SOUND )
				next = 0;
			if( wc->sound[ nLane ][ next ] )
				wc->sound[ nLane ][ next ]->SetVolume( lVolume );
			break;
		}
	}
}

void DTX::SetWAVCache( bool bON )
{
	this->bWAVCache = bON;
}
void DTX::SetBMPCache( bool bON )
{
	this->bBMPCache = bON;
}
void DTX::SetResultImage( int rank, LPCTSTR fname, int arPriority[] )
{
	if( rank < 0 || rank > 6 )
		return;

	for( int i = rank; i >= 0; i-- )
	{
		if( arPriority[i] < 7-rank )
		{
			arPriority[i] = 7-rank;
			this->strResultImage[i] = fname;
		}
	}
}

void DTX::SetResultMovie( int rank, LPCTSTR fname, int arPriority[] )
{
	if( rank < 0 || rank > 6 )
		return;

	for( int i = rank; i >= 0; i-- )
	{
		if( arPriority[i] < 7-rank )
		{
			arPriority[i] = 7-rank;
			this->strResultMovie[i] = fname;
		}
	}
}

void DTX::SetResultSound( int rank, LPCTSTR fname, int arPriority[] )
{
	if( rank < 0 || rank > 6 )
		return;

	for( int i = rank; i >= 0; i-- )
	{
		if( arPriority[i] < 7-rank )
		{
			arPriority[i] = 7-rank;
			this->strResultSound[i] = fname;
		}
	}
}
//
bool DTX::IsCommand( LPTSTR *p, LPCTSTR cmd )
{
	static TCHAR buf[256];
	int len = lstrlen( cmd );
	if( len > 255 ) len = 255;
	TCHAR *q = *p;
	
	int i = 0;
	while( *q != _T(':') && *q != _T('\n') && *q != _T(' ') && *q != _T('\t') && i < len )
		buf[i++] = *q++;
	buf[i] = _T('\0');

	if( i != len || _tcsnicmp( buf, cmd, i ) != 0 )
		return false;

	SKIPSP( q );
	if( *q == _T(':') ) {
		q++;
		SKIPSP( q );
	}
	*p = q;
	return true;
}

int  DTX::GetCommand( LPTSTR *p, LPCTSTR cmd )
{
	static TCHAR buf[256];			// コマンドは最大256文字
	int len = lstrlen( cmd );
	if( len > 255 ) len = 255;

	TCHAR *q = *p;
	int i;
	for( i = 0; *q != _T(':') && *q != _T('\n') && *q != _T(' ') && *q != _T('\t') && i < len; i++ )	// bufへはcmdと同じ文字数だけコピーする
		buf[i] = *q++;
	buf[i] = _T('\0');

	if( lstrcmpi( buf, cmd ) != 0 ) return 0;

	int zz = GetZex( q );

	if( zz >= 0 )
		while((*q>=_T('0')&&*q<=_T('9'))||(*q>=_T('a')&&*q<=_T('z'))||(*q>=_T('A')&&*q<=_T('Z'))||*q==_T('-')||*q==_T('+')) q++;

	SKIPSP( q );
	if( *q == _T(':') )
	{
		q++;
		SKIPSP( q );
	}
	*p = q;
	return ( zz < 0 ) ? -1 : zz;
}

int  DTX::GetZex( LPCTSTR p )
{
	int num = 0;
	for( int i = 0; i < 2; i++ )
	{
		num *= 36;
		if( *p >= _T('0') && *p <= _T('9') )		num += *p - _T('0');
		else if( *p >= _T('a') && *p <= _T('z') )	num += *p - _T('a') + 10;
		else if( *p >= _T('A') && *p <= _T('Z') )	num += *p - _T('A') + 10;
		else
			return -1;	// 0～9,A～Z 以外の文字が使われている
		p++;
	}
	return num;
}

int  DTX::GetHex( LPCTSTR p )
{
	int num = 0;
	for( int i = 0; i < 2; i++ )
	{
		num *= 16;
		if( *p >= _T('0') && *p <= _T('9') )		num += *p - _T('0');
		else if( *p >= _T('a') && *p <= _T('f') )	num += *p - _T('a') + 10;
		else if( *p >= _T('A') && *p <= _T('F') )	num += *p - _T('A') + 10;
		else 
			return -1;	// 0～9,A～F 以外の文字が使われている
		p++;
	}
	return num;
}

int  DTX::GetDec( LPCTSTR p )
{
	int f = 1;
	if( *p == _T('-') )
	{
		f = -1;
		p++;
	}
	else if( *p == _T('+') )
	{
		f = 1;
		p++;
	}
	int num = 0;
	while( *p >= _T('0') && *p <= _T('9') )
	{
		num = num * 10 + (*p - _T('0'));
		p++;
	}
	return num * f;
}

int  DTX::GetDec3( LPCTSTR p )
{
	int num = 0;
	for( int i = 0; i < 3; i++ )
	{
		num *= 10;
		if( *p >= _T('0') && *p <= _T('9') )				num += *p - _T('0');
		else if( i == 0 && *p >= _T('a') && *p <= _T('z') )	num += *p - _T('a') + 10;
		else if( i == 0 && *p >= _T('A') && *p <= _T('Z') ) num += *p - _T('A') + 10;
		else
			return -1;	// 0～9 以外の文字が使われている
		p++;
	}
	return num;
}

int  DTX::GetChannel( LPCTSTR p )
{
	// GDA, G2D 以外は16 進数２ケタ
	if( this->scoreType != SCORETYPE_GDA && this->scoreType != SCORETYPE_G2D )
		return GetHex( p );

	// GDA, G2D は特殊文字列
	#define GDACH	( 5*12+1 )	//この数はこれ参照--------------------------------↓
	static const struct ChMap {
		LPCTSTR strCh;
		int nCh;
	} chmap[ GDACH ] = {
		{"TC", 0x03}, {"BL", 0x02}, {"GS", 0x29}, {"DS", 0x30}, {"FI", 0x53},	// 1
		{"HH", 0x11}, {"SD", 0x12}, {"BD", 0x13}, {"HT", 0x14}, {"LT", 0x15},	// 2
		{"CY", 0x16}, {"G1", 0x21}, {"G2", 0x22}, {"G3", 0x23}, {"G4", 0x24},	// 3
		{"G5", 0x25}, {"G6", 0x26}, {"G7", 0x27}, {"GW", 0x28}, {"01", 0x61},	// 4
		{"02", 0x62}, {"03", 0x63}, {"04", 0x64}, {"05", 0x65}, {"06", 0x66},	// 5
		{"07", 0x67}, {"08", 0x68}, {"09", 0x69}, {"0A", 0x70}, {"0B", 0x71},	// 6
		{"0C", 0x72}, {"0D", 0x73}, {"0E", 0x74}, {"0F", 0x75}, {"10", 0x76},	// 7
		{"11", 0x77}, {"12", 0x78}, {"13", 0x79}, {"14", 0x80}, {"15", 0x81},	// 8
		{"16", 0x82}, {"17", 0x83}, {"18", 0x84}, {"19", 0x85}, {"1A", 0x86},	// 9
		{"1B", 0x87}, {"1C", 0x88}, {"1D", 0x89}, {"1E", 0x90}, {"1F", 0x91},	// 10
		{"20", 0x92}, {"B1", 0xA1}, {"B2", 0xA2}, {"B3", 0xA3}, {"B4", 0xA4},	// 11
		{"B5", 0xA5}, {"B6", 0xA6}, {"B7", 0xA7}, {"BW", 0xA8}, {"G0", 0x20},	// 12
		{"B0", 0xA0}															// +1
	};

	if( *p == _T('\0') || *(p+1) == _T('\0') )
		return -1;	// チャンネル番号が２ケタない

	TCHAR buf[3];
	buf[0] = *p++;
	buf[1] = *p++;
	buf[2] = _T('\0');
	for( int i = 0; i < GDACH; i++ )
	{
		if( lstrcmpi( chmap[i].strCh, buf ) == 0 )
			return chmap[i].nCh;
	}
	return -1;
}

void DTX::SetSpeed( float fSpeed )
{
	for( Wave *w = this->pWave; w != NULL; w = w->next )
	{
		if( w->bUse )
		{
			for( int i = 0; i < 11; i++ )
			{
				if( w->bUseByLane[ i ] )
				{
					for( int j = 0; j < DTX_MAX_SOUND; j++ )
					{
						if( w->sound[ i ][ j ] != NULL )
							w->sound[ i ][ j ]->SetSpeed( fSpeed );
					}
				}
			}
		}
	}
}

void DTX::CalcHash()
{
	CFileMM file;

	// ファイルの読み込み
	if( ! file.Load( this->strFullPath ) )
		return;		// 失敗

	// ファイル内容のハッシュ値を計算して this->strHash へ格納
	MD5 md5;
	md5.Init();
	md5.Update( file.GetData(), file.GetSize() );
	md5.Final( this->strHash );

	file.Term();
}

static int s_nChangeRGB[6][8] = {
	{ 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 },		// RGB → RGB
	{ 0x00, 0x02, 0x01, 0x03, 0x04, 0x06, 0x05, 0x07 },		//        RBG
	{ 0x00, 0x01, 0x04, 0x05, 0x02, 0x03, 0x06, 0x07 },		//        GRB
	{ 0x00, 0x02, 0x04, 0x06, 0x01, 0x03, 0x05, 0x07 },		//        GBR
	{ 0x00, 0x04, 0x01, 0x05, 0x02, 0x06, 0x03, 0x07 },		//        BRG
	{ 0x00, 0x04, 0x02, 0x06, 0x01, 0x05, 0x03, 0x07 }		//        BGR
};
void DTX::Random( int nType, bool bSuper )
{
	int nPart    = -10000;	// 小節番号
	int nPattern = 0;		// 小節単位の入れ替え法則

	int n適用チップ数 = 0;

	for( Chip* cp = this->pChip; cp != NULL; cp = cp->next )
	{
		// 小節が変わったら入れ替え法則を変更
		if( (int)(cp->dwPosition / DTX_RESOLVE) != nPart ) {
			nPart = (int)(cp->dwPosition / DTX_RESOLVE);
			nPattern = rand() % 6;
		}

		// レーン入れ替え
		int ch = cp->nChannel;
		if( ( nType == 0 && ch >= 0x20 && ch <= 0x27 ) || ( nType == 1 && ch >= 0xA0 && ch <= 0xA7 ) )
		{
			if( bSuper )
				cp->nChannel = (ch & 0xF0) | ( s_nChangeRGB[ rand() % 6 ][ ch & 0x07 ] );
			else
				cp->nChannel = (ch & 0xF0) | ( s_nChangeRGB[ nPattern   ][ ch & 0x07 ] );

			n適用チップ数 ++;
		}
	}
}

DWORD DTX::GetChipParamType( int nCh )
{
	switch( nCh )
	{
		// バックコーラス
		case 0x01:
			return DTXCPT_WAV;

		// BPM変更
		case 0x03:
			return DTXCPT_BPM;

		// BPM変更（拡張）
		case 0x08:
			return DTXCPT_BPMEX;

		// ＢＧＡ
		case 0x04: case 0x07: case 0x55: case 0x56: case 0x57: case 0x58: case 0x59: case 0x60:
			return DTXCPT_BMP | DTXCPT_BMPTEX | DTXCPT_BGA | DTXCPT_BGAPAN;

		// ＢＧＡスコープ画像切替
		case 0xC4: case 0xC7: case 0xD5: case 0xD6: case 0xD7: case 0xD8: case 0xD9: case 0xE0:
			return DTXCPT_BMP | DTXCPT_BMPTEX;

		// ドラムパート（通常、不可視、空うち）
		case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17: case 0x18: case 0x19: case 0x1A:
		case 0x31: case 0x32: case 0x33: case 0x34: case 0x35: case 0x36: case 0x37: case 0x38: case 0x39: case 0x3A:
		case 0xB1: case 0xB2: case 0xB3: case 0xB4: case 0xB5: case 0xB6: case 0xB7: case 0xB8: case 0xB9: case 0xBC:
			return DTXCPT_WAV;

		// ギターパート（通常、空うち）
		case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25: case 0x26: case 0x27:
		case 0xBA:
			return DTXCPT_WAV;

		// ベースパート（通常、空打ち）
		case 0xA0: case 0xA1: case 0xA2: case 0xA3: case 0xA4: case 0xA5: case 0xA6: case 0xA7:
		case 0xBB:
			return DTXCPT_WAV;

		// ＢＧＭ
		case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67: case 0x68: case 0x69: case 0x70:
		case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x76: case 0x77: case 0x78: case 0x79: case 0x80:
		case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87: case 0x88: case 0x89: case 0x90:
		case 0x91: case 0x92:
			return DTXCPT_WAV;

		// AVI
		case 0x54:
			return DTXCPT_AVI;

		// 歓声
		case 0x1F: case 0x2F: case 0xAF:
			return DTXCPT_WAV;
	}
	
	return 0;
}

	}//General
}//FDK