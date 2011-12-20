#pragma once

namespace FDK {
	namespace General {

class CommandLineParser
{
public:
	void Init();
	void Init( LPCTSTR str解析するコマンドライン文 );
	int GetItemNum();			// アイテム数を返す
	LPCTSTR Get( int index );	// index 番目のアイテムを返す（ index = 0～ GetItemNum()-1 ）。返されるのは内部の tstring の c_str() なので、長時間は扱えないことに注意。
	void Term();

	CommandLineParser();

protected:

	// (1) '-' で始まる項目はオプションとみなす。この場合、strItem には先頭の '-' 文字も含まれる。（例："-N1"）
	// (2) 項目間は空白文字( SPC, TAB )で区切られる。全角SPCは空白とみなさない。
	// (3) ただし、ダブルクォートされている場合はそれを１つの項目とみなす。

	struct CmdLineItem {
		tstring		strItem;
		CmdLineItem	*prev, *next;
	} *pFirstItem, *pLastItem;

	int nアイテム数;
};

	}//General
}//FDK