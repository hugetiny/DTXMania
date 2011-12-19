#include "stdafx.h"
#include "CArrayList.h"

namespace FDK {
	namespace General {

CArrayList::CArrayList( CArrayList::ARRAYLISTTYPE Type, UINT n要素サイズ )
{
    this->arrayListType				= Type;
    this->p動的配列					= NULL;
	this->n要素サイズ				= (Type == ALT_REFERENCE) ? sizeof(LPVOID) : n要素サイズ;
    this->n現在の要素数				= 0;
	this->n現在の動的配列のサイズ	= 0;
}

CArrayList::~CArrayList()
{
    if( ! this->p動的配列 )
        delete[] this->p動的配列;
}

HRESULT CArrayList::t要素を追加する( void* pEntry )
{
    if( this->n要素サイズ == 0 )
        return E_FAIL;	// 初期化されてない

	// 配列をオーバーフローするなら新しく領域を確保する。
	if( ! this->p動的配列 || this->n現在の要素数 + 1 > this->n現在の動的配列のサイズ )
	{
		HRESULT hr = this->t新しい動的配列を確保する();
		if( FAILED(hr) )
			return hr;
	}

	// 動的配列に新要素を追加する。
	switch( this->arrayListType )
	{
	case ALT_REFERENCE:
        *(((void**)this->p動的配列) + this->n現在の要素数) = pEntry;
		break;

	case ALT_VALUE:
        CopyMemory( (BYTE*)this->p動的配列 + (this->n現在の要素数 * this->n要素サイズ), pEntry, this->n要素サイズ );
		break;
	}
    this->n現在の要素数++;

    return S_OK;
}

void CArrayList::t要素を削除する( UINT nIndex )
{
	if( this->n現在の要素数 <= 0 )
		return;

    this->n現在の要素数 --;

	BYTE* pData = (BYTE*)this->p動的配列 + (nIndex * this->n要素サイズ);
    MoveMemory( pData, pData + this->n要素サイズ, ( this->n現在の要素数 - nIndex) * this->n要素サイズ );
}

void* CArrayList::p要素を取得する( UINT nIndex )
{
	switch( this->arrayListType )
	{
		case ALT_REFERENCE:
	        return *(((void**)this->p動的配列) + nIndex);

		case ALT_VALUE:
	        return (BYTE*)this->p動的配列 + (nIndex * this->n要素サイズ);
	}

	return NULL;
}

bool CArrayList::b要素が配列内に存在する( void* pEntryData )
{
    for( UINT i = 0; i < this->n現在の要素数; i++ )
    {
		switch( this->arrayListType )
		{
		case ALT_REFERENCE:
			if( this->p要素を取得する(i) == pEntryData )
                return true;
			break;

		case ALT_VALUE:
			if( memcmp( this->p要素を取得する(i), pEntryData, this->n要素サイズ ) == 0 )
                return true;
			break;
		}
    }
    return false;
}

HRESULT CArrayList::t新しい動的配列を確保する()
{
	// 新しい配列のサイズはこれまでの２倍
	UINT 新しい動的配列のサイズ = ( this->n現在の動的配列のサイズ == 0 ) ? 16 : this->n現在の動的配列のサイズ * 2;

	// 新配列のメモリを確保
	void* 新しい動的配列 = new BYTE[ 新しい動的配列のサイズ * this->n要素サイズ ];
	if( ! 新しい動的配列 )
		return E_OUTOFMEMORY;	// 失敗

	// 旧配列があるなら中身をコピーする
	if( this->p動的配列  )
	{
		CopyMemory( 新しい動的配列, this->p動的配列, this->n現在の要素数 * this->n要素サイズ );
		delete[] this->p動的配列;
	}

	// 移行完了
	this->p動的配列               = 新しい動的配列;
	this->n現在の動的配列のサイズ = 新しい動的配列のサイズ;

	return S_OK;
}

	}//General
}//FDK