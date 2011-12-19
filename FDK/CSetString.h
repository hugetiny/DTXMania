//
// CSetString
//   文字列専用の集合。複数の文字列にインデックス番号でアクセスできる。
//   文字列削除後のガベージコレクションは行わない。
//   １バッファのサイズ(4096)を超える文字列も扱えない。
//
#pragma once

namespace FDK {
	namespace General {

class CSetString
{
public:
	CSetString();
	CSetString( DWORD dw初期バッファサイズbytes );
	virtual ~CSetString();

	int  n要素数();
	bool t追加( LPTSTR p追加する文字列 );							// 追加に成功したらtrueを返す
	bool t削除( int n削除する文字列の0から始まるインデックス );		// 削除に成功したらtrueを返す
	LPCTSTR p指定番目の文字列を返す( int n0から始まるインデックス );

protected:
	struct Index {
		LPTSTR	p文字列;
		Index	*prev, *next;
	} *pIndexFirst, *pIndexLast;

	struct StringBuffer {
		LPTSTR	p文字列バッファ;
		DWORD	dwバッファサイズ;
		DWORD	dw現在使用可能な残りのサイズ;
		StringBuffer	*prev, *next;
	} *pStringBufferFirst, *pStringBufferLast;

	StringBuffer* p新しいStringBufferを作成して返す( DWORD dwバッファサイズ = 4096 );
	bool b文字列をバッファに格納可能( LPTSTR p格納したい文字列, StringBuffer* p格納したいバッファ );
};
	}//General
}//FDK

using namespace FDK::General;
