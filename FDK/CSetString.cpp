#include "stdafx.h"
#include "CSetString.h"
#include "FDKError.h"
#include "Debug.h"

namespace FDK {
	namespace General {

CSetString::CSetString()
{
	this->pIndexFirst = this->pIndexLast = NULL;
	this->pStringBufferFirst = this->pStringBufferLast = this->p新しいStringBufferを作成して返す();
}
CSetString::CSetString( DWORD dw初期バッファサイズbytes )
{
	if( dw初期バッファサイズbytes == 0 )
		dw初期バッファサイズbytes = 4096;

	this->pIndexFirst = this->pIndexLast = NULL;
	this->pStringBufferFirst = this->pStringBufferLast = this->p新しいStringBufferを作成して返す( dw初期バッファサイズbytes );
}
CSetString::~CSetString()
{
	Index* pi = this->pIndexFirst;
	while( pi != NULL )
	{
		Index* pn = pi->next;
		SAFE_FREE( pi );
		pi = pn;
	}
	this->pIndexFirst = this->pIndexLast = NULL;

	StringBuffer* psb = this->pStringBufferFirst;
	while( psb != NULL )
	{
		StringBuffer* psn = psb->next;
		SAFE_FREE( psb->p文字列バッファ );
		SAFE_FREE( psb );
		psb = psn;
	}
	this->pStringBufferFirst = this->pStringBufferLast = NULL;
}
CSetString::StringBuffer* CSetString::p新しいStringBufferを作成して返す( DWORD dwバッファサイズ )
{
	_ASSERT( dwバッファサイズ >= 0 );

	StringBuffer* pSB;
	if( ( pSB = (StringBuffer*) malloc( sizeof( StringBuffer ) ) ) == NULL )
		return NULL;

	if( ( pSB->p文字列バッファ = (LPTSTR) malloc( dwバッファサイズ ) ) == NULL ) {
		SAFE_FREE( pSB );
		return NULL;
	}

	pSB->dwバッファサイズ = dwバッファサイズ;
	pSB->dw現在使用可能な残りのサイズ = dwバッファサイズ;
	pSB->prev = pSB->next = NULL;

	return pSB;
}
bool CSetString::t追加( LPTSTR p追加する文字列 )
{
	// 戻り値: 追加できれば true

	StringBuffer* pSB;

	// pSB ← 追加対象（リストの一番最後）のバッファ
	{
		if( ( pSB = this->pStringBufferLast ) == NULL )
			return false;
	}
	//
	// pSB に文字列が格納できることを保障する
	{
		if( ! this->b文字列をバッファに格納可能( p追加する文字列, pSB ) )
		{
			if( ( pSB = this->p新しいStringBufferを作成して返す() ) == NULL )
				return false;

			APPENDLIST( this->pStringBufferFirst, this->pStringBufferLast, pSB );

			if( ! this->b文字列をバッファに格納可能( p追加する文字列, pSB ) )
				return false;	// 追加する文字列がバッファ１個のサイズよりでかいときは失敗。
		}
	}
	//
	// 文字列を pSB へ追加する
	{
		LPTSTR p格納位置 = pSB->p文字列バッファ + ( pSB->dwバッファサイズ - pSB->dw現在使用可能な残りのサイズ );

		// インデックス作成
		Index* pi = (Index*) malloc( sizeof(Index) );
		pi->p文字列 = p格納位置;
		pi->prev = pi->next = NULL;
		APPENDLIST( this->pIndexFirst, this->pIndexLast, pi );

		// StringBuffer へ書き込み
		int nLen = lstrlen( p追加する文字列 );
		for( int i = 0; i < nLen; i++ )
			*p格納位置++ = *p追加する文字列++;
		*p格納位置 = _T('\0');

		pSB->dw現在使用可能な残りのサイズ -= nLen + 1;
	}

	return true;
}
bool CSetString::b文字列をバッファに格納可能( LPTSTR p格納したい文字列, StringBuffer* p格納したいバッファ )
{
	_ASSERT( p格納したい文字列 );
	_ASSERT( p格納したいバッファ );

	int n必要なサイズ;
	if( ( n必要なサイズ = lstrlen( p格納したい文字列 ) + 1 ) <= 0 )
		return false;
	
	return ( p格納したいバッファ->dw現在使用可能な残りのサイズ >= (DWORD) n必要なサイズ ) ? true : false;
}
bool CSetString::t削除( int n削除する文字列の0から始まるインデックス )
{
	// 戻り値: 追加できれば true

	int n;
	if( ( n = n削除する文字列の0から始まるインデックス ) < 0 )
		return false;		// 範囲外

	// n番目のインデックスセルを探す
	Index* pi;
	if( ( pi = this->pIndexFirst ) == NULL )
		return false;

	while( n > 0 )
	{
		pi = pi->next;
		n --;

		if( pi == NULL )
			return false;	// 範囲外
	}

	// インデックスセルを削除する
	REMOVELIST( this->pIndexFirst, this->pIndexLast, pi );
	SAFE_FREE( pi );

	return true;
}
int  CSetString::n要素数()
{
	int n = 0;
	for( Index* pi = this->pIndexFirst; pi != NULL; pi = pi->next )
		n ++;

	return n;
}
LPCTSTR CSetString::p指定番目の文字列を返す( int n0から始まるインデックス )
{
	if( n0から始まるインデックス < 0 )
		return NULL;	// 範囲外

	Index* pi;
	if( ( pi = this->pIndexFirst ) == NULL )
		return NULL;

	while( n0から始まるインデックス > 0 )
	{
		pi = pi->next;
		n0から始まるインデックス --;

		if( pi == NULL )
			return NULL;	// 範囲外
	}

	return pi->p文字列;
}
	}//General
}//FDK
