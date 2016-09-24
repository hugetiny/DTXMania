// クラス情報
/*

	DTX クラス

	演奏ファイルに対応するクラス。以下の情報を持つ。

		(1) オブジェクト情報

			Chip		オブジェクトリスト

		(2) ヘッダ情報①（リテラル）

			タイトル	文字列
			レベル値	数値
			etc.,

		(3) ヘッダ情報②（リスト＆リソース）：

			BPM			BPM（数値）のリスト
			Wave 		音声リスト；リソース本体は CSoundManager で管理し、本構造体ではポインタだけを管理する。）
			BMP 		２Ｄ画像リスト；リソース本体は CSurfaceManager で管理し、本構造体ではポインタだけを管理する。）
			BMPTEX		３Ｄ画像リスト：リソース本体は CTextureManager で管理し、本構造体ではポインタだけを管理する。）
			BGA			数値と{BMP|BMPTEX}への参照のリスト
			BGAPan		数値と{BMP|BMPTEX}への参照のリスト
			AVI			動画リスト：CAvi を内包

		(4) Wave に対する各種メソッド：

			WAV（単音）の再生開始、停止、音量設定、再生速度設定、再生位置補正
			WAV（全音）の中途再生、停止、一時停止、再開

		(5) リソースデータをキャッシュするための各 Manager 。

			CSoundManager		音声（CSound）のキャッシュ
			CSurfaceManager		２Ｄ画像（CSurface）のキャッシュ
			CTextureManager		３Ｄ画像（CTexture）のキャッシュ

	本クラスは Activity クラスから継承している。
		→ DTX は Activity のルールに従い、適切なタイミングで Init, Restore, Invalidate, Delete に連動すること。
		　（どこへも AddChild されない場合は特に注意。）
		→ ただし、本クラスの持つ情報は UnActivate() 後にもアクセスされるので、UnActivate() では何もせず、
		   別途内部情報をリセット（リスト解放など）するためのメソッド Clear() を準備する。
*/
// 使い方
/*
	【使い方】

	(0) 下準備を行う。
			・DrumMIDI ライブラリの初期化
			・タイマ（CTimerEx）、MIDI Out（HMIDOUT）の事前設定（Set～メソッド参照）

	(1) OneTimeSceneInit() を呼び出す。
			・各 Manager が初期化される。

	(2) Activate() を呼び出す。
			・内部で Clear() が呼び出され、内部情報がリセットされる。

	(3) Load() で演奏ファイルを読み込む。
			・MIDIファイルを読み込む場合は、事前に DTX::m_nMIDILevel を設定すること。
			・ヘッダ情報①（リテラル）だけ読み込みたい場合は、bHeaderOnly = true にする。

	(4) LoadBMP(), LoadAVI() でデバイス依存オブジェクトと動画を構築する。
			・Load() 後に実行すること。

	(5) 演奏前に Random() でギターチップに RANDOM 加工を適用する。（オプション）
			・Load() 後に実行すること。

	(6) 演奏または演奏情報の表示。アプリ側で DTX の各 public メンバに自由にアクセスする。

	(7) UnActivate() を呼び出す。
			・空メソッド。何も行われない。

	(8) Clear() を呼び出し、内部情報をリセットする。
			・DTX::bWAVCache, DTX::bBMPCache が false の場合、各 Manager の持つ実データについては、ここでは何もしない。
			 （各 Manager それぞれのライフサイクルに従う。）
			 true の場合はここで実データをクリアする。

	(9) FinalCleanup() を呼び出す。
			・各 Manager を終了する。ここでは内部情報については何もしないので、事前に Clear() しておくこと。

	その他、Activity ルールに従って、下記メソッドを呼び出す。
		・MyInitDeviceObjects()				各 Manager に接続。
		・MyRestoreDeviceObjects()			同上
		・MyInvalidateDeviceObjects()		同上
		・MyDeleteDeviceObjects()			同上
*/

#pragma once

#include "Activity.h"
#include "CTimer.h"
#include "CSound.h"
#include "CSoundManager.h"
#include "CSurfaceManager.h"
#include "CTextureManager.h"
#include "CAvi.h"
#include "CD3DApplication.h"

namespace FDK {
	namespace General {

// 諸パラメータ定義
#define	DTX_TEXTLEN		1024		// 文字列メンバの最大長
#define	DTX_LINELEN		32768		// 読み込み用文字列（演奏ファイル内の１行）の最大長
#define	DTX_MAX_SOUND	2			// サウンドごとの最大同時発声数
#define	DTX_RESOLVE		384			// １小節の解像度
#define	DTX_PLAYVOLUME	100			// 手動演奏パートの基準音量[％]
#define	DTX_AUTOVOLUME	80			// 自動演奏パートの基準音量[％]

// ScoreType 定義 - 演奏データの種別
enum ScoreType {
	SCORETYPE_UNKNOWN,
	SCORETYPE_DTX,
	SCORETYPE_BMS,
	SCORETYPE_GDA,
	SCORETYPE_G2D,
	SCORETYPE_MID
};

// BGAType 定義 - BGAコマンド識別用
enum BGAType {
	BGATYPE_UNKNOWN,
	BGATYPE_BMP,
	BGATYPE_BMPTEX,
	BGATYPE_BGA,
	BGATYPE_BGAPAN
};

// AVIType 定義 - AVIコマンド識別用
enum AVIType {
	AVITYPE_UNKNOWN,
	AVITYPE_AVI,
	AVITYPE_AVIPAN
};

// DTXCPT_xxx 定義 - DTX::Chip の引数(nParam=zz)の種類 (DWORD)
#define DTXCPT_WAV		(1 << 0)
#define	DTXCPT_WAVVOL	(1 << 1)
#define	DTXCPT_WAVPAN	(1 << 2)
#define	DTXCPT_BPM		(1 << 3)
#define	DTXCPT_BPMEX	(1 << 4)
#define	DTXCPT_BMP		(1 << 5)
#define	DTXCPT_BMPTEX	(1 << 6)
#define	DTXCPT_BGA		(1 << 7)
#define	DTXCPT_BGAPAN	(1 << 8)
#define	DTXCPT_AVI		(1 << 9)

class DTX : public Activity
{
public:
	void	Clear();	// 内部状態の初期化、リソース(WAV,BMP)の解放、内部リストのクリア

	void	SetTimer( CTimerEx* pc ) {this->pTimer = pc;}
	void	SetMIDIOut( HMIDIOUT hMidiOut ) {this->hMidiOut = hMidiOut;}
	void	SetWAVCache( bool bON );
	void	SetBMPCache( bool bON );

	// 演奏ファイルの読み込み；
	// Activate()後に呼び出すこと。
	// 　fname ,........ ファイル名（フルパス）
	// 　bHeaderOnly ... ヘッダのみの読み込みならtrue
	// 　dbSpeed ....... 演奏速度（BPMに乗じる数値；デフォルト1.0）
	bool	Load( LPCTSTR fname, bool bHeaderOnly=false, double dbPlaySpeed=1.0 );

	// 演奏ファイルの読み込み；
	// Activate()後に呼び出すこと。
	// 　fname ,........ ファイル名（フルパス）
	// 　bHeaderOnly ... ヘッダのみの読み込みならtrue
	// 　dbSpeed ....... 演奏速度（BPMに乗じる数値；デフォルト1.0）
	//   bLv100 ........ 10以下のレベルを10倍する場合はtrue
	bool	Load( tstring &fname, bool bHeaderOnly=false, double dbPlaySpeed=1.0 )
			{ return Load( fname.c_str(), bHeaderOnly, dbPlaySpeed ); }
	
	// BMP, BMPTEX の登録。
	// Load()後に実行すること。
	// ここでは m_TextureManager|m_SurfaceManager.CreateCachedSurface() を使用する。
	void	LoadBMP();
	
	// AVI ファイルの読み込み
	// Load()後に実行すること。
	// ここでは、CAvi::Init() と CAvi::Open() までを行う。
	void	LoadAVI();
	
	// ギター／ベースについて、ランダムを適用する。
	// 　nType = 0:Guitar, 1:Bass
	// 　bSuper = true: SUPER RANDOM, false: RANDOM
	void	Random( int nType, bool bSuper=false );

	class	Chip;

	// 全チップのWAVE/BGAについて dwStartTime 時点で再生中か否か調査し、
	// 再生中なら適切な個所から再生を開始する。
	void	SkipStart( double dbStartTime );

	// WAVE の発声音量の設定
	// 　nLane = レーン番号（Drums=0～6, Guitar=7, Bass=8, BGM=9）
	// 　lVolume = 音量 0..100 [%]
	void	SetVolume( int nWave, long lVolume=DTX_AUTOVOLUME, int nLane=9 );
	
	//
	// 現在のWAVリストにあるすべてのWaveの再生スピードを変更する。
	void	SetSpeed( float fSpeed=1.0f );

	// すべてのWAVEの時間調整；
	// 開始時刻から判断して、再生カーソルを現在あるべき正しい位置に移動する。
	void 	AdjustWaves();

	//
	// 曲データハッシュを計算して m_strHash に格納する。
	void	CalcHash();
	
	// 指定されたChipの nParam を Wave 実番号とみなして発声
	// 　pChip = Chip へのポインタ
	// 　nLane = レーン番号（Drums=0～7, Guitar=8, Bass=9, BGM=10）
	// 　lVol  = 基本音量[%]（DTX::Wave.volume を反映する前の音量）
	// 　bMonitor = MIDIの音量UPならtrue
	// 　bBad  = 音程をずらすなら true
	void 	PlayChip( Chip* pChip, int nLane=10, long lVol=DTX_AUTOVOLUME, bool bMIDIMonitor=false, bool bBad=false );

	// WAVE の発声停止
	// 　nWave = Wave番号
	// 　nLane = レーン番号（Drums=0～6, Guitar=7, Bass=8, BGM=9）
	void	StopWave( int nWave, int nLane );

	void	StopAllWaves();			// すべてのWAVEの発声停止
	void	PauseWaves();			// 全 Wave の発声一時停止
	void	ContWaves();			// 全 Wave の発声再開

	bool	MyOneTimeSceneInit();				// 初期化；各 Manager の初期化
	bool	MyActivate();						// 活性化；Clear() を呼び出すだけ
	bool	MyUnActivate();						// 非活性化；WAV,BMP,BMPTEX 以外のリソース（AVI, Chipなど）の解放（これらは Clear()内でも行われるが、一応ここでも…。）
	bool	MyFinalCleanup();					// 終了処理；各 Manager の解放
	bool	MyInitDeviceObjects();				// デバイス依存オブジェクトの初期化
	bool	MyRestoreDeviceTextures();			// デバイス依存オブジェクトの構築（テクスチャ）。事前に SetDirect3DDevice() で Direct3D デバイスを登録しておくこと。
	bool	MyRestoreDeviceSurfaces();			// デバイス依存オブジェクトの構築（サーフェイス）。事前に SetDirect3DDevice() で Direct3D デバイスを登録しておくこと。
	bool	MyInvalidateDeviceObjects();		// デバイス依存オブジェクトの解放
	bool	MyDeleteDeviceObjects();			// Desc: デバイス依存オブジェクトの破棄

	DTX();
	virtual ~DTX();

public:
	bool			bDTXV;						// DTXV での利用なら、生成直後に true にする。
	tstring			strFullPath;				// ファイル名（フルパス）（例："C:\DTXFiles\50000t\50000t.gda")
	tstring			strFolder;					// ファイルパス（例："C:\DTXFiles\50000t\")
	TCHAR			strFileExt[ _MAX_EXT ];		// ファイルの拡張子（例: "gda"）
	ScoreType		scoreType;					// 拡張子に対応するスコアタイプ
	tstring			strTitle;					// タイトル名
	tstring			strArtist;					// アーティスト名
	tstring			strComment;					// コメント
	tstring			strPanel;					// パネルコメント
	tstring			strGenre;					// ジャンル
	bool			bHiddenLevel;				// レベルを不明として表示する場合に true 。
	int				nLevel[ 3 ];				// レベル(易:1-100:難)（注：最大値は制限してない）(0:Drums, 1:Guitar, 2:Bass)
	double			dbBPM;						// BPM値 → #BPM: の値が入る。
	double			dbBaseBPM;					// 基本BPM値
	tstring			strStage;					// Stageファイル名
	tstring			strPresound;				// プレビュー音ファイル名
	tstring			strPreimage;				// プレビュー画像ファイル名
	tstring			strPremovie;				// プレビュー動画ファイル名
	tstring			strBackground;				// 背景ファイル名（ドラム画面）
	tstring			strBackgroundGR;			// 背景ファイル名（ＧＲ画面）
	tstring			strResultImage[7];			// リザルト画像ファイル名（rank SS,S,A,B,C,D,E の順に７つ）
	tstring			strResultMovie[7];			// リザルト動画ファイル名（rank SS,S,A,B,C,D,E の順に７つ）
	tstring			strResultSound[7];			// リザルト音ファイル名（rank SS,S,A,B,C,D,E の順に７つ）
	tstring			strMIDI;					// MIDIファイル名
	bool			bMIDINote;					// MIDIノート
	int				nMIDILevel;					// MIDIレベル(0:EASY, 1:REAL, 2:RAW;  MIDでのみ有効,他は0)	--> MyActivate() の後 かつ Load() の前に設定すること！
	int				nMIDIMap;					// MIDIマップ（DM_MIDIMAP_...; DrumMIDI.h 参照）
	bool			bBlackColorKey;				// BMP 画像の黒値を透過するならtrue
	tstring			strPathWav;					// WAV,BMP,etc. の存在するフォルダ（#PATH_WAV 指定）
	float			fSpeed;						// 再生速度（デフォルト：1.0f）(DTXV専用）
	bool			bHeaderOnly;				// Header だけの読み込みなら true

	// BPM 構造体
	struct BPM {
		int		num;								// BPM実番号
		int		vnum;								// 表記上のBPM番号(00～ZZ; 00 は#BPM: の値)
		double	bpm;								// BPM値
		BPM		*prev, *next;						// 前後のセル
	} *pBPM, *pBPMLast;

	// WAVE 構造体
	struct Wave {
		int		num;								// WAV実番号
		int		vnum;								// 表記上のWAV番号(01～ZZ)
		int		volume;								// 音量(0～100[%])
		int		pan;								// パン(-100(左)～0～100(右))
		bool	bUse;								// これが false ならば open に失敗したことを意味する。
		tstring	strFileName;						// WAVファイル名
		// 以下、[0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM
		bool	bUseByLane[ 11 ];					// 各レーンでこのWaveが使われるならtrue。これが false なら、たとえ bUse=true でも sound[] は使えない。([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		int		nCurSound[ 11 ];					// 現在再生中のサウンド番号(0..MAX_SOUND-1; 初期値は-1) ([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		CSound*	sound[ 11 ][ DTX_MAX_SOUND ];		// サウンド（１レーンにつきMAX_SOUND個ずつ。 ）([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		bool	bPlaying[ 11 ][ DTX_MAX_SOUND ];	// 再生中なら true([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		double	dbStartTime[ 11 ][ DTX_MAX_SOUND ];	// 再生開始時刻([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		double	dbPauseTime[ 11 ][ DTX_MAX_SOUND ];	// 一時停止時刻([0..7]:ドラム、[8]:ギター、[9]:ベース、[10]:BGM)
		DWORD	dwBufferSize;						// サウンドバッファのサイズ
		double	dbTotalTime;						// サウンドバッファの総演奏時間[ms]
		Wave	*prev, *next;						// 前後のセル
	} *pWave, *pWaveLast;

	// BMP 構造体
	struct BMP {
		int			num;							// BMP番号(00～ZZ; 00 は初期画像)
		bool		bUse;							// これが false ならば open に失敗したことを意味する。
		tstring		strFileName;					// BMPファイル名
		CSurface*	pSurface;						// 画像
		BMP			*prev, *next;					// 前後のセル
	} *pBMP, *pBMPLast;
	bool	bUseBMP;							// BMPが１つでもあるならtrue
	DWORD	dwBMPWidthMax;						// BPM の最大幅
	DWORD	dwBMPHeightMax;						// BPM の最大高

	// BMPTEX 構造体
	struct BMPTEX {
		int			num;							// BMPTEX番号(01～ZZ)
		bool		bUse;							// これが false ならば open に失敗したことを意味する。
		tstring		strFileName;					// BMPTEXファイル名
		CTexture*	pTexture;						// 画像
		BMPTEX		*prev, *next;					// 前後のセル
	} *pBMPTEX, *pBMPTEXLast;
	bool	bUseBMPTEX;							// BMPTEXが１つでもあるならtrue
	DWORD	dwBMPTEXWidthMax;					// BPMTEX の最大幅
	DWORD	dwBMPTEXHeightMax;					// BPMTEX の最大高

	// BGA 構造体
	struct BGA {
		int		num;								// BGA番号(01～ZZ)
		int		bmp;								// BMP番号(01～ZZ)
		int		x1, y1;								// BGAの左上座標(BMP画像内相対座標)
		int		x2, y2;								// BGAの右下座標(        〃       )
		int		ox, oy;								// BGAの表示位置(DTXManiaのBGA領域相対)
		BGA		*prev, *next;						// 前後のセル
	} *pBGA, *pBGALast;

	// BGAPan 構造体
	struct BGAPan {
		int		num;								// BGAPAN番号(01～ZZ)
		int		bmp;								// BMP番号(01～ZZ)
		int		sw, sh;								// 開始転送サイズ
		int		ew, eh;								// 終了転送サイズ
		int		ssx, ssy;							// 画像側開始座標(BMP画像内相対座標)
		int		sex, sey;							// 画像側終了座標(        〃       )
		int		dsx, dsy;							// 表示側開始位置(DTXManiaのBGA領域相対座標)
		int		dex, dey;							// 表示側終了位置(            〃           )
		int		len;								// 開始位置から終了位置への移動にかける時間[ct]（１小節＝PART_RESOLV ct）
		BGAPan	*prev, *next;						// 前後のセル
	} *pBGAPan, *pBGAPanLast;

	// AVI 構造体
	struct AVI {
		int		num;								// AVI番号(01～ZZ)
		bool	bUse;								// これが false なら open に失敗したことを意味する。
		tstring	strFileName;						// AVIファイル名
		CAvi	avi;								// AVIストリーム
		AVI		*prev, *next;						// 前後のセル
	} *pAVI, *pAVILast;
	bool bUseAVI;								// AVIが１つでもあるならtrue

	// AVIPan 構造体
	struct AVIPan {
		int		num;								// AVIPAN番号(01～ZZ)
		int		avi;								// AVI番号(01～ZZ)
		int		sw, sh;								// 開始転送サイズ
		int		ew, eh;								// 終了転送サイズ
		int		ssx, ssy;							// 画像側開始座標(AVI画像内相対座標)
		int		sex, sey;							// 画像側終了座標(        〃       )
		int		dsx, dsy;							// 表示側開始位置(DTXManiaのBGA領域相対座標)
		int		dex, dey;							// 表示側終了位置(            〃           )
		int		len;								// 開始位置から終了位置への移動にかける時間[ct]（１小節＝PART_RESOLV ct）
		AVIPan	*prev, *next;						// 前後のセル
	} *pAVIPan, *pAVIPanLast;

	// Chip クラス
	class Chip
	{												// ●CHIP リスト
	public:
		DWORD	dwPosition;							// 発声位置（先頭からの絶対位置；１小節の長さは小節長倍率に関わらず PART_RESOLV で固定）; MIDの場合、小節線・拍線以外ここは 0
		int		nChannel;							// チャンネル番号
		int		nParam;								// [整数時] Wave,BMP,BGA,BGAPAN,AVI,BPM 番号(01～ZZ)
		int		nRealParam;							//          Wave,AVI,BPM の実番号(1～...無限)
		double	dbParam;							// [実数時] パラメータ値(ch.02)
		Chip	*prev, *next;						// 前後のセル
		double	dbTime;								// 発声時刻[ms]（MID では g_dmCHIP_Event から取得）

		// MID専用; g_dmCHIP_Event から取得（nChannel がドラムチップの時のみ有効）
		union {              
			DWORD dwMsg;							// DWORDアクセス用イベントデータ
			BYTE bData[4];							// BYTEアクセス用イベントデータ [0]:ステータス
		} message;									// メッセージ変換用共用体

		// BMP,BGA,BGAPAN 制御用;
		// (1) nChannel = BGAレイヤチャネル(04,07,55～59,60) の場合：
		//   BGAType==BGATYPE_BMP    の場合、pBMP だけが有効。（他３つはNULL）
		//            BGATYPE_BMPTEX の場合、pBMPTEX だけが有効。（他３つはNULL）
		//            BGATYPE_BGA    の場合、(pBMP|pBMPTEX)とpBGAが有効。（他２つはNULL）
		//	          BGATYPE_BGAPAN の場合、(pBMP|pBMPTEX)とpBGAPANが有効。（他２つはNULL）
		// (2) nChannel = BGAスコープ画像切替チャンネル(C4,C7,D5～D9,E0) の場合：
		//   BGAType==BGATYPE_BMP    の場合、pBMP だけが有効。（他３つはNULL）
		//            BGATYPE_BMPTEX の場合、pBMPTEX だけが有効。（他３つはNULL）
		//   BGAType が BGATYPE_BGA|BGAPAN の場合はありえない（無効）。
		BGAType		BGAtype;						//
		BMP*		pBMP;							//
		BMPTEX*		pBMPTEX;						// 
		BGA*		pBGA;							//
		BGAPan*		pBGAPan;						//

		// AVI,AVIPAN 制御用：
		// nChannel = AVIチャネル(54) の場合、
		//   AVIType==AVITYPE_AVI    の場合、pAVI だけが有効。（pAVIPanはNULL）
		//            AVITYPE_AVIPAN の場合、pAVI, pAVIPan の両方が有効。
		AVIType		AVItype;						//
		AVI*		pAVI;							//
		AVIPan*		pAVIPan;						//

		// BGAPAN/AVIPAN 制御用
		double		dbLong;							// 総移動時間

		// 以下は演奏制御用
		bool	bHit;								// 処理済ならtrue;
		bool	bVisible;							// 不可視ならtrue;
		long	nDotFromBarD;						// バーからの距離[dot] １チップにつき、この３つを全部計算する。
		long	nDotFromBarG;						// バーからの距離[dot]
		long	nDotFromBarB;						// バーからの距離[dot]
		int		nType;								// 0:Drum, 1:Guitar, 2:Bass, -1:Other

	public:
		Chip() {
			dwPosition 		= 0;
			nChannel		= -1;
			nParam			= 0;
			nRealParam		= 0;
			dbParam			= 0.0;
			prev = next		= NULL;
			dbTime			= 0;
			message.dwMsg	= 0;
			dbLong			= 0;
			BGAtype			= BGATYPE_UNKNOWN;
			pBMP			= NULL;
			pBMPTEX			= NULL;
			pBGA			= NULL;
			pBGAPan			= NULL;
			AVItype			= AVITYPE_UNKNOWN;
			pAVI			= NULL;
			pAVIPan			= NULL;
			bHit			= false;
			bVisible		= true;
			nDotFromBarD	= 500;
			nDotFromBarG	= 500;
			nDotFromBarB	= 500;
			nType			= -1;
		}
	} *pChip, *pChipLast;
	
	int		nTotalChipsD[ 10 ];					// 全チップ数（ドラム）; AutoPlay のチップも含む（→Autoか否かのフラグはアプリ依存であり FDK にはない）
	int		nTotalChipsG;						// 全チップ数（ギター）; 同上
	int		nTotalChipsB;						// 全チップ数（ベース）; 同上
	bool	bUseDrum;							// ドラムパートにチップがあるなら true
	bool	bUseGuitar;							// ギターパートにチップがあるなら true
	bool	bUseBass;							// ベースパートにチップがあるなら true
	bool	bUseHHOpen;							// HHOpen チップがあるなら true
	bool	bUseRide;							// Ride チップがあるなら true
	bool	bUseLeftCymal;						// LeftCymbal チップがあるなら true
	TCHAR	strHash[33];						// 曲データハッシュ

protected:
	void			LoadDTX();					// DTX 形式スコアファイルの読み込み
	virtual void	LoadMID() {};				// MIDI 形式スコアファイルの読み込み（オーバーライド用）

	// WAV の読み込み
	// ここでは m_pSoundManager.CreateCachedSound() のみ使用する。
	void	LoadWAV();
	
	void	InsertChip( Chip *cell );			// 位置（dwPosition）が昇順になるように、適切な場所に Chip を挿入する。
	void	InsertMIDIChip( Chip *cell );		// 位置（dwTime）が昇順になるように、適切な場所に cell を挿入する。MIDI用。
	void	InsertLines();						// 小節線、拍線の挿入

	// チップの発声時刻の算出。
	// ここでは、演奏速度（BPMに乗じる割合）は 1.0 で固定とし、
	// 後で AdjustChipTimeByPlaySpeed() で反映させる。（DTX と MID で共通のため）
	void	CalcChipTime();

	// 全チップの発声時刻を、dbPlaySpeed にあわせて修正する。
	// 　dbSpeed ... 演奏速度（BPMに乗じる数値；デフォルト1.0）
	void	AdujstChipTimeByPlaySpeed( double dbPlaySpeed=1.0 );

	// 指定されたチャンネル nCh の nParam (zz) が何を指定するものであるかを返す。
	// Ret:  以下のマスクの論理輪を返す。
	//       DTXCPT_WAV
	//       DTXCPT_BPM
	//       DTXCPT_BPMEX
	//       DTXCPT_BMP
	//       DTXCPT_BMPTEX
	//       DTXCPT_BGA
	//       DTXCPT_BGAPAN
	//       DTXCPT_AVI
	DWORD	GetChipParamType( int nCh );

	void	AdjustWave( DTX::Wave* pWave, double dbStartTime, CSound* pSound );		// Wave 単位での再生位置自動調整

	void	SkipStartWave( double dbStartTime, Chip* cell );	// 指定WAVEについて dbStartTime 時点から再生を開始する。
	void	SkipStartBGA( double dbStartTime, Chip* cell );		// 全チップのBGAについて dbStartTime 時点から再生を開始する。

	// 優先順位を見ながら、適切な m_strResultImage[] にファイル名を設定する。
	// 　rank ... 0:SS,S,A,B,C,D,E:6
	void	SetResultImage( int rank, LPCTSTR fname, int arPriority[] );
	
	// 優先順位を見ながら、適切な m_strResultMovie[] にファイル名を設定する。
	// 　rank ... 0:SS,S,A,B,C,D,E:6
	void	SetResultMovie( int rank, LPCTSTR fname, int arPriority[] );

	// 優先順位を見ながら、適切な m_strResultSound[] にファイル名を設定する。
	// 　rank ... 0:SS,S,A,B,C,D,E:6
	void	SetResultSound( int rank, LPCTSTR fname, int arPriority[] );

	// コマンド文字列比較
	bool	IsCommand( LPTSTR *p, LPCTSTR cmd );

	// コマンドチェック
	// (1) 文字列 p から指定されたコマンド文字列が続くか判定　→　続かないなら 0 を返す
	// (2) その後に [空白][':'][空白] があればスキップする
	// (3) その後に36進数文字列２ケタが続くか判断　→　続かないなら -1 を返す
	// (4) (3)で記憶した値を戻り値として返す
	int		GetCommand( LPTSTR *p, LPCTSTR cmd );

	int		GetZex( LPCTSTR p );					// 36進数２ケタの取得
	int		GetHex( LPCTSTR p );					// 16進数２ケタの取得
	int		GetDec( LPCTSTR p );					// 10進数（無限桁）の取得
	int		GetDec3( LPCTSTR p );					// 10進数３桁の取得（1桁目は 0～Z まで対応 (2002/11/03)）
	int		GetChannel( LPCTSTR p );				// チャンネル番号の取得

protected:
	CSoundManager		soundManager;				// サウンドマネージャ
	CSurfaceManager		surfaceManager;				// サーフェイスマネージャ
	CTextureManager		textureManager;				// テクスチャマネージャ
	CTimerEx*			pTimer;						// CTimerEx へのポインタ
	HMIDIOUT			hMidiOut;					// MIDI 出力
	bool				bWAVCache;					// WAVをキャッシュする場合は true
	bool				bBMPCache;					// BMPをキャッシュする場合は true

private:
	void	tAVIリストの解放と初期化();
	void	tAVIPANリストの解放と初期化();
	void	tCHIPリストの解放と初期化();

	// 以下は作業用変数（ LoadDTX() 用）
	int		m_nLine;						// 現在処理中の行番号
	int		m_nResultImagePriority[7];		// #RESULTIMAGE 用 優先順位バッファ
	int		m_nResultMoviePriority[7];		// #RESULTMOVIE 用 優先順位バッファ
	int		m_nResultSoundPriority[7];		// #RESULTSOUND 用 優先順位バッファ
	int		m_nWaveNum[36*36];				// #WAV 用の実番号スタック
	int		m_nWaveNumCur;					// 現在の#WAV 実番号
	int		m_nWaveVol[36*36];				// #WAVVOL 用スタック[36*36]
	int		m_nWavePan[36*36];				// #WAVPAN 用スタック[36*36]
	int		m_nBPMNum[36*36];				// #BPM 用の実番号スタック
	int		m_nBPMNumCur;					// 現在の#BPM 実番号
	int		m_nBMPNum[36*36];				// #BMP 用の実番号スタック
	int		m_nBMPTEXNum[36*36];			// #BMPTEX 用の実番号スタック
	int		m_nBGANum[36*36];				// #BGA 用の実番号スタック
	int		m_nBGAPanNum[36*36];			// #BGAPAN 用の実番号スタック
	int		m_nBMPNumCur;					// 現在の#BMP,BMPTEX,BGA,BGAPAN 実番号（４つで共通）
	int		m_nRand;						// 現在の乱数
	bool	m_bSkip[256];					// IF～ENDIFをスキップするなら true（最大255ネスト）
	int		m_nSkip;						// IFのネスト数（bSkip[] の引数値）
};

	}//General
}//FDK

using namespace FDK::General;
