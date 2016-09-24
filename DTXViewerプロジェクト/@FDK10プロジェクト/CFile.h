#pragma once

namespace FDK {
	namespace General {

class CFile
{
public:
	void	Init();		// 初期化
	void	Term();		// 終了処理

	bool	Load( LPCTSTR filename );										// ファイルの読み込み
	bool	Load( tstring &filename )	{return Load( filename.c_str() );}	// ファイルの読み込み

	LPBYTE GetData()			{return this->pData;}				// ファイルデータへのポインタを返す
	DWORD  GetSize()			{return this->dwFileSizeLow;}		// ファイルサイズの取得
	FILETIME GetLastWriteTime()	{return this->ftLastWriteTime;}	// 最終更新時刻を返す

	static void GetExt( LPCTSTR filename, LPTSTR ext );		// 拡張子を調べて ext[_MAX_EXT] に格納（例："txt")

public:
	CFile();
	virtual ~CFile();

protected:
	tstring		strFileName;				// ファイル名
	TCHAR		strExt[_MAX_EXT];			// 拡張子
	BYTE*		pData;						// データ
	FILETIME	ftLastWriteTime;			// 最終更新時刻
	DWORD		dwFileSizeHigh;				// ファイルサイズ（上位32ビット）
	DWORD		dwFileSizeLow;				// ファイルサイズ（下位32ビット）
};

	}//General
}//FDK

using namespace FDK::General;
