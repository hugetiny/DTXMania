
// CAvi, CAviExpandBuf - AVI再生クラス

#pragma once

#pragma comment(lib, "vfw32.lib")

namespace FDK {
	namespace Graphics {

class CAvi
{
public:
	void _初期化();
	void _解放();

	HRESULT Open( LPCTSTR fname );									// ファイルをオープンする。
	HRESULT Open( tstring &fname ) {return Open( fname.c_str() );}	// ファイルをオープンする。
	void    Close();												// ファイルをクローズする。

	DWORD _時刻からフレーム番号を返す( double time );				// time ... 時刻(CTimer表記)

	// フレームサイズの取得（Open後に有効）
	void _フレームサイズを取得する( LONG* plWidth, LONG* plHeight );

	// 指定時刻のフレームを DC へ描画する。
	// 　hWnd ... ウィンドウハンドル
	// 　hDC .... デバイスコンテクスト
	// 　time ... 時刻
	// 　sx, sy ... 転送元座標
	// 　sw, sh ... 転送元サイズ
	// 　dx, dy ... 転送先座標
	// 　dw, dh ... 転送先サイズ
	// 戻り値: CAVI_正常終了 ..... 正常終了
	// 　　　　CAVI_再生終了 ..... 再生終了（総再生時間より time の方が大きい）
	// 　　　　CAVI_エラー終了 ... エラー終了
	int _指定時刻のフレームをDCへ描画する( HWND hWnd, HDC hDC, double time, int sx=0, int sy=0, int sw=-1, int sh=-1, int dx=0, int dy=0, int dw=-1, int dh=-1 );
	static const int CAVI_正常終了 = 0;
	static const int CAVI_再生終了 = 1;
	static const int CAVI_エラー終了 = -1;

public:
	CAvi();
	virtual ~CAvi();

protected:

	static int n参照数;		// 現在のインスタンス数

	PAVIFILE	 hFile;		// AVIのファイルハンドル
	PAVISTREAM	 hAvi;		// AVIのストリームハンドル
	PGETFRAME	 hFrame;	// GetFrameオブジェクト
	HDRAWDIB	 hDrawDib;

	DWORD dwレート;			// 動画のレート [サンプル/秒]
	DWORD dwスケール;		// 動画のスケール [サンプル/フレーム]
	DWORD dw総フレーム数;	// 動画のフレーム長
	LONG  lFrameWidth;		// 動画の横幅
	LONG  lFrameHeight;		// 動画の縦幅

	bool bErrorAtDrawFrame;	// Draw中にエラーが発生したらtrueになる。
};

	}//Graphics
}//FDK

using namespace FDK::Graphics;
