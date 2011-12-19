#include "stdafx.h"
#include "MD5.h"

namespace FDK {
	namespace General {

#define	F(X,Y,Z)	(((X) & (Y)) | ((~X) & (Z)))
#define	G(X,Y,Z)	(((X) & (Z)) | ((Y) & (~Z)))
#define	H(X,Y,Z)	((X) ^ (Y) ^ (Z))
#define	I(X,Y,Z)	((Y) ^ ((X) | (~Z)))
#define	ROL(x,n)	(((x) << (n)) | ((x) >> (32-(n))))
#define	FF(a,b,c,d,x,s,ac) {\
 (a) += F((b),(c),(d)) + (x) + (UINT4)(ac);\
 (a)  = ROL((a),(s));\
 (a) += (b);\
}
#define	GG(a,b,c,d,x,s,ac) {\
 (a) += G((b),(c),(d)) + (x) + (UINT4)(ac);\
 (a)  = ROL((a),(s));\
 (a) += (b);\
}
#define	HH(a,b,c,d,x,s,ac) {\
 (a) += H((b),(c),(d)) + (x) + (UINT4)(ac);\
 (a)  = ROL((a),(s));\
 (a) += (b);\
}
#define	II(a,b,c,d,x,s,ac) {\
 (a) += I((b),(c),(d)) + (x) + (UINT4)(ac);\
 (a)  = ROL((a),(s));\
 (a) += (b);\
}

void	MD5::Init()
{
	m_input_len = 0;
	m_buflen	= 0;
	m_state[0]	= 0x67452301;
	m_state[1]	= 0xefcdab89;
	m_state[2]	= 0x98badcfe;
	m_state[3]	= 0x10325476;
}

void	MD5::Update( UCHAR* input, ULONG input_len )
{
	// 入力がなくなるまで...
	while( input_len > 0 )
	{
		// m_buffer[] に入力を64バイトコピー
		while( m_buflen < 64 && input_len > 0 )
		{
			m_buffer[ m_buflen ] = *input++;
			m_buflen ++;
			m_input_len ++;
			input_len --;
		}
		
		// 64バイトコピーする前に入力がなくなれば、その状態で終了する。
		// （バッファの内容は、次回の Update() あるいは Final() に引き継がれる。）
		if( m_buflen < 64 )
			return;

		// コピーされた64バイトを処理
		Transform();

		// 入力バッファをクリア
		m_buflen = 0;
	}
}

void	MD5::Final( TCHAR outbuf[33] )
{
	static const TCHAR hexcode[] = _T("0123456789abcdef");

	// パッディング
	m_buffer[ m_buflen++ ] = (UCHAR) 0x80;
	if( m_buflen > 56 )
	{
		while( m_buflen < 64 )
			m_buffer[ m_buflen++ ] = (UCHAR) 0x00;
		Transform();
		m_buflen = 0;
	}
	while( m_buflen < 56 )
		m_buffer[ m_buflen++ ] = (UCHAR) 0x00;

	// 入力の総サイズの８バイト表記を末尾に追加
	m_input_len *= 8;
	m_buffer[ m_buflen++ ] = (UCHAR) ((m_input_len >>  0) & 0xFF);
	m_buffer[ m_buflen++ ] = (UCHAR) ((m_input_len >>  8) & 0xFF);
	m_buffer[ m_buflen++ ] = (UCHAR) ((m_input_len >> 16) & 0xFF);
	m_buffer[ m_buflen++ ] = (UCHAR) ((m_input_len >> 24) & 0xFF);
	m_buffer[ m_buflen++ ] = (UCHAR) 0x00;
	m_buffer[ m_buflen++ ] = (UCHAR) 0x00;
	m_buffer[ m_buflen++ ] = (UCHAR) 0x00;
	m_buffer[ m_buflen++ ] = (UCHAR) 0x00;

	// 最後の処理
	Transform();

	// 128ビットの出力（A,B,C,D）を32文字の16進数文字列に変換して
	// 出力バッファに格納する。
	for( int i = 0; i < 4; i++ )
	{
		UCHAR c;
		c = (UCHAR) ((m_state[i] >>  0) & 0xFF);
		outbuf[i*8+0] = hexcode[ c >> 4 ];
		outbuf[i*8+1] = hexcode[ c & 0x0F ];
		c = (UCHAR) ((m_state[i] >>  8) & 0xFF);
		outbuf[i*8+2] = hexcode[ c >> 4 ];
		outbuf[i*8+3] = hexcode[ c & 0x0F ];
		c = (UCHAR) ((m_state[i] >> 16) & 0xFF);
		outbuf[i*8+4] = hexcode[ c >> 4 ];
		outbuf[i*8+5] = hexcode[ c & 0x0F ];
		c = (UCHAR) ((m_state[i] >> 24) & 0xFF);
		outbuf[i*8+6] = hexcode[ c >> 4 ];
		outbuf[i*8+7] = hexcode[ c & 0x0F ];
	}
	outbuf[32] = _T('\0');
}


void	MD5::Transform()
{
	UINT4	a,b,c,d;
	UINT4	x[16];

	a = m_state[0];
	b = m_state[1];
	c = m_state[2];
	d = m_state[3];

	for( int i = 0; i < 16; i++ )
	{
		x[i] = (((UINT4) m_buffer[i*4+0]) <<  0)
			 | (((UINT4) m_buffer[i*4+1]) <<  8)
			 | (((UINT4) m_buffer[i*4+2]) << 16)
			 | (((UINT4) m_buffer[i*4+3]) << 24);
	}

	FF(a, b, c, d, x[ 0],  7, 0xd76aa478); /* 1 */
	FF(d, a, b, c, x[ 1], 12, 0xe8c7b756); /* 2 */
	FF(c, d, a, b, x[ 2], 17, 0x242070db); /* 3 */
	FF(b, c, d, a, x[ 3], 22, 0xc1bdceee); /* 4 */
	FF(a, b, c, d, x[ 4],  7, 0xf57c0faf); /* 5 */
	FF(d, a, b, c, x[ 5], 12, 0x4787c62a); /* 6 */
	FF(c, d, a, b, x[ 6], 17, 0xa8304613); /* 7 */
	FF(b, c, d, a, x[ 7], 22, 0xfd469501); /* 8 */
	FF(a, b, c, d, x[ 8],  7, 0x698098d8); /* 9 */
	FF(d, a, b, c, x[ 9], 12, 0x8b44f7af); /* 10 */
	FF(c, d, a, b, x[10], 17, 0xffff5bb1); /* 11 */
	FF(b, c, d, a, x[11], 22, 0x895cd7be); /* 12 */
	FF(a, b, c, d, x[12],  7, 0x6b901122); /* 13 */
	FF(d, a, b, c, x[13], 12, 0xfd987193); /* 14 */
	FF(c, d, a, b, x[14], 17, 0xa679438e); /* 15 */
	FF(b, c, d, a, x[15], 22, 0x49b40821); /* 16 */

	GG(a, b, c, d, x[ 1],  5, 0xf61e2562); /* 17 */
	GG(d, a, b, c, x[ 6],  9, 0xc040b340); /* 18 */
	GG(c, d, a, b, x[11], 14, 0x265e5a51); /* 19 */
	GG(b, c, d, a, x[ 0], 20, 0xe9b6c7aa); /* 20 */
	GG(a, b, c, d, x[ 5],  5, 0xd62f105d); /* 21 */
	GG(d, a, b, c, x[10],  9,  0x2441453); /* 22 */
	GG(c, d, a, b, x[15], 14, 0xd8a1e681); /* 23 */
	GG(b, c, d, a, x[ 4], 20, 0xe7d3fbc8); /* 24 */
	GG(a, b, c, d, x[ 9],  5, 0x21e1cde6); /* 25 */
	GG(d, a, b, c, x[14],  9, 0xc33707d6); /* 26 */
	GG(c, d, a, b, x[ 3], 14, 0xf4d50d87); /* 27 */
	GG(b, c, d, a, x[ 8], 20, 0x455a14ed); /* 28 */
	GG(a, b, c, d, x[13],  5, 0xa9e3e905); /* 29 */
	GG(d, a, b, c, x[ 2],  9, 0xfcefa3f8); /* 30 */
	GG(c, d, a, b, x[ 7], 14, 0x676f02d9); /* 31 */
	GG(b, c, d, a, x[12], 20, 0x8d2a4c8a); /* 32 */

	HH(a, b, c, d, x[ 5],  4, 0xfffa3942); /* 33 */
	HH(d, a, b, c, x[ 8], 11, 0x8771f681); /* 34 */
	HH(c, d, a, b, x[11], 16, 0x6d9d6122); /* 35 */
	HH(b, c, d, a, x[14], 23, 0xfde5380c); /* 36 */
	HH(a, b, c, d, x[ 1],  4, 0xa4beea44); /* 37 */
	HH(d, a, b, c, x[ 4], 11, 0x4bdecfa9); /* 38 */
	HH(c, d, a, b, x[ 7], 16, 0xf6bb4b60); /* 39 */
	HH(b, c, d, a, x[10], 23, 0xbebfbc70); /* 40 */
	HH(a, b, c, d, x[13],  4, 0x289b7ec6); /* 41 */
	HH(d, a, b, c, x[ 0], 11, 0xeaa127fa); /* 42 */
	HH(c, d, a, b, x[ 3], 16, 0xd4ef3085); /* 43 */
	HH(b, c, d, a, x[ 6], 23,  0x4881d05); /* 44 */
	HH(a, b, c, d, x[ 9],  4, 0xd9d4d039); /* 45 */
	HH(d, a, b, c, x[12], 11, 0xe6db99e5); /* 46 */
	HH(c, d, a, b, x[15], 16, 0x1fa27cf8); /* 47 */
	HH(b, c, d, a, x[ 2], 23, 0xc4ac5665); /* 48 */

	II(a, b, c, d, x[ 0],  6, 0xf4292244); /* 49 */
	II(d, a, b, c, x[ 7], 10, 0x432aff97); /* 50 */
	II(c, d, a, b, x[14], 15, 0xab9423a7); /* 51 */
	II(b, c, d, a, x[ 5], 21, 0xfc93a039); /* 52 */
	II(a, b, c, d, x[12],  6, 0x655b59c3); /* 53 */
	II(d, a, b, c, x[ 3], 10, 0x8f0ccc92); /* 54 */
	II(c, d, a, b, x[10], 15, 0xffeff47d); /* 55 */
	II(b, c, d, a, x[ 1], 21, 0x85845dd1); /* 56 */
	II(a, b, c, d, x[ 8],  6, 0x6fa87e4f); /* 57 */
	II(d, a, b, c, x[15], 10, 0xfe2ce6e0); /* 58 */
	II(c, d, a, b, x[ 6], 15, 0xa3014314); /* 59 */
	II(b, c, d, a, x[13], 21, 0x4e0811a1); /* 60 */
	II(a, b, c, d, x[ 4],  6, 0xf7537e82); /* 61 */
	II(d, a, b, c, x[11], 10, 0xbd3af235); /* 62 */
	II(c, d, a, b, x[ 2], 15, 0x2ad7d2bb); /* 63 */
	II(b, c, d, a, x[ 9], 21, 0xeb86d391); /* 64 */

	m_state[0] += a;
	m_state[1] += b;
	m_state[2] += c;
	m_state[3] += d;
}

BYTE	MD5::CalcSum( TCHAR* hash )
{
	DWORD sum = 0;
	for( TCHAR* p = hash; *p != _T('\0'); p++ )
		sum += (DWORD)(*p);
	return (BYTE)(sum & 0xff);
}

//--------------------------------------------
// ハッシュ作成用補助関数
void	MD5::PutBool( BYTE** p, bool b )
{
	*(*p)++ = (b) ? '1' : '0';
}

void	MD5::PutInt( BYTE** p, int n )
{
	char buf[128];
	sprintf_s( buf, 128, "%d", n );
	for( int i = 0; i < 16 && buf[i]!='\0'; i++ )
		*(*p)++ = buf[i];
}

void	MD5::PutInt64( BYTE** p, __int64 n )
{
	char buf[128];
	sprintf_s( buf, 128, "%I64d", n );
	for( int i = 0; i < 16 && buf[i]!='\0'; i++ )
		*(*p)++ = buf[i];
}

void	MD5::PutFloat( BYTE** p, float f )
{
	char buf[128];
	sprintf_s( buf, 128, "%f", f );
	for( int i = 0; i < 16 && buf[i]!='\0'; i++ )
		*(*p)++ = buf[i];
}

void	MD5::PutString( BYTE** p, TCHAR* str )
{
	lstrcpyn( (TCHAR*)(*p), str, lstrlen(str) );
	*p += lstrlen( str ) * sizeof(TCHAR);
}


	}//General
}//FDK