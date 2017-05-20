
// CArrayList - 動的配列
// 　値型と参照型の２種類がある（生成時に指定）。

#pragma once

namespace FDK {
	namespace General {

class CArrayList
{
public:
	static const enum ARRAYLISTTYPE {
		ALT_VALUE,		// 値型
		ALT_REFERENCE	// 参照型
	};

public:
	// コンストラクタ
	// 　Type ... 値型 or 参照型。
	// 　n要素サイズ ... １要素のサイズ。参照型では意味なし。
	CArrayList( ARRAYLISTTYPE Type, UINT n要素サイズ = 0 );

	// デストラクタ
	// 　動的配列を解放する。
	~CArrayList();

	// 要素を追加する。
	// 　正常なら S_OK を返す。
	HRESULT t要素を追加する( void* pEntry );

	// 要素を削除する。
	// 　nIndex ... 要素のインデックス(0..)
    void t要素を削除する( UINT nIndex );

	// 要素を取得する。
	// 　nIndex ... 要素のインデックス(0..)
    void* p要素を取得する( UINT nIndex );

	// 指定した要素が配列内に存在するなら true を返す。
    bool b要素が配列内に存在する( void* pEntryData );

	UINT Count()	{ return n現在の要素数; }		// 現在の要素数を返す。
    void Clear()	{ n現在の要素数 = 0; }		// 動的配列をクリアする。

protected:
	ARRAYLISTTYPE	arrayListType;			// 値型か参照型か？
    void*			p動的配列;
	UINT			n要素サイズ;				// １要素の大きさ。参照型では sizeof(LPVOID) となる。
    UINT			n現在の要素数;
    UINT			n現在の動的配列のサイズ;

private:
	HRESULT			t新しい動的配列を確保する();
};

	}//General
}//FDK

using namespace FDK::General;
