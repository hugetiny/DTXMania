
#pragma once

namespace FDK {
	namespace General {

#define _FACFDK		0x0266

#define FDKERR_初期化されてない				MAKE_HRESULT( 1, _FACFDK,  1 )
#define FDKERR_アダプタがない				MAKE_HRESULT( 1, _FACFDK,  2 )
#define	FDKERR_DirectSoundがNULL			MAKE_HRESULT( 1, _FACFDK,  3 )
#define	FDKERR_サウンドバッファがNULL		MAKE_HRESULT( 1, _FACFDK,  4 )
#define	FDKERR_SoundTypeが無効				MAKE_HRESULT( 1, _FACFDK,  5 )
#define	FDKERR_サウンドバッファの作成に失敗	MAKE_HRESULT( 1, _FACFDK,  6 )
#define	FDKERR_ファイルの読み込み失敗		MAKE_HRESULT( 1, _FACFDK,  7 )
#define	FDKERR_DirectDrawがNULL				MAKE_HRESULT( 1, _FACFDK,  8 )
#define	FDKERR_サーフェイスがNULL			MAKE_HRESULT( 1, _FACFDK,  9 )
#define	FDKERR_サーフェイスの作成に失敗		MAKE_HRESULT( 1, _FACFDK, 10 )
#define	FDKERR_DCの取得に失敗				MAKE_HRESULT( 1, _FACFDK, 11 )
#define FDKERR_StretchBltに失敗				MAKE_HRESULT( 1, _FACFDK, 12 )
#define FDKERR_Bltに失敗					MAKE_HRESULT( 1, _FACFDK, 13 )
#define FDKERR_Lockに失敗					MAKE_HRESULT( 1, _FACFDK, 14 )
#define FDKERR_Capsの取得に失敗				MAKE_HRESULT( 1, _FACFDK, 15 )
#define	FDKERR_テクスチャフォーマットがない	MAKE_HRESULT( 1, _FACFDK, 16 )
#define	FDKERR_BeginSceneされてない			MAKE_HRESULT( 1, _FACFDK, 17 )
#define	FDKERR_3DデバイスがNULL				MAKE_HRESULT( 1, _FACFDK, 18 )
#define	FDKERR_パラメータがNULL				MAKE_HRESULT( 1, _FACFDK, 19 )
#define	FDKERR_DIBの作成に失敗				MAKE_HRESULT( 1, _FACFDK, 20 )
#define	FDKERR_テクスチャがNULL				MAKE_HRESULT( 1, _FACFDK, 21 )
#define	FDKERR_動画情報の取得に失敗			MAKE_HRESULT( 1, _FACFDK, 22 )
#define	FDKERR_動画フレームのオープンに失敗	MAKE_HRESULT( 1, _FACFDK, 23 )
#define	FDKERR_DRAWDIBのオープンに失敗		MAKE_HRESULT( 1, _FACFDK, 24 )
#define	FDKERR_DDSの作成に失敗				MAKE_HRESULT( 1, _FACFDK, 25 )
#define	FDKERR_テクスチャの作成に失敗		MAKE_HRESULT( 1, _FACFDK, 26 )
#define	FDKERR_DirectSoundの作成に失敗		MAKE_HRESULT( 1, _FACFDK, 27 )
#define	FDKERR_DirectSoundの協調レベルの設定に失敗	MAKE_HRESULT( 1, _FACFDK, 28 )

// HRESULT に該当するエラーメッセージを取得して返す。
extern LPCTSTR HRMSG( HRESULT hr );

// GetLastError() で得られたエラー値からメッセージを取得して返す。
extern LPCTSTR LASTERRMSG();
	
// エラーメッセージテキスト
extern tstring g_strFDKErrMsg;

	}//General
}//FDK

using namespace FDK::General;
