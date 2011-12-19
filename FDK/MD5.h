
/*
 * MD5 - MD5文字列の作成
 *
 *【概要】
 *  任意のメッセージ（バイナリ可）のMD5文字列（16進数32文字）を作成する。
 *  ただし、本コードでは（手を抜いているので）メッセージは最大 2^32 バイト
 *  までしか対応していない。
 *
 *【使い方】
 *  (2) MD5::Init() を呼び出す。
 *  (3) 何回かにわけて MD5::Update() を呼び出し、メッセージをすべて処理する。
 *  (4) すべて処理が終わったら MD5::Final() を呼び出し、最終的な MD5 文字列を受け取る。
 */

#pragma once

namespace FDK {
	namespace General {

typedef USHORT	UINT2;		// UINT2: 符号なし16bit整数
typedef	ULONG	UINT4;		// UINT4: 符号なし32bit整数

class MD5
{
public:
	void	Init();										// 初期化
	void 	Update( UCHAR* input, ULONG input_len );	// メッセージの処理

	// 最後のメッセージ変換。作成された MD5 コード（128bit）を16進数表記に
	// 変換した文字列（32文字＋'\0'）が outbuf に格納される。
	// (∴outbuf[] は最低33文字以上入ること。）
	void	Final( TCHAR outbuf[] );

	static BYTE		CalcSum( TCHAR* hash );				// 指定されたハッシュのサムを計算して返す。
	static void		PutBool( BYTE** p, bool b );		// bool 出力
	static void		PutInt( BYTE** p, int n );			// int 出力
	static void		PutInt64( BYTE** p, __int64 n );	// __int64 出力
	static void		PutFloat( BYTE** p, float f );		// float 出力
	static void		PutString( BYTE** p, TCHAR* str );	// 文字列出力

public:
	MD5() {}
	virtual ~MD5() {}

protected:
	void	Transform();		// 入力バッファを変換処理し、その結果を m_state[4] に格納する。

protected:
	ULONG	m_input_len;		// 入力の総サイズ
	UINT4	m_state[4];			// 出力(A,B,C,D)
	UCHAR	m_buffer[64+1];		// 入力バッファ(16ワード)
	ULONG	m_buflen;			// 入力バッファの有効バイト数

};

	}//General
}//FDK

using namespace FDK::General;
