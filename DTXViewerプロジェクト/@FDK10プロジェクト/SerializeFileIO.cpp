#include "stdafx.h"
#include "Debug.h"
#include "SerializeFileIO.h"
#include "CFile.h"
#include "CFileMM.h"

namespace FDK {
	namespace General {

void SerializeFileIO::TEST()
{
	FILE *fp;
	if( _tfopen_s( &fp, _T("SerializeFileIOテスト.bin"), _T("wb") ) == 0 )
	{
		SerializeFileIO::PutDWORD( fp, 0x12345678 );
		SerializeFileIO::PutWORD( fp, 0xabcd );
		SerializeFileIO::PutString( fp, _T("おけ。") );
		SerializeFileIO::PutString( fp, _T("かよ？") );
		LPCTSTR buf = _T("本当にそうおもうのですかあなたは？SoWhy?");
		SerializeFileIO::PutString( fp, buf );
		SerializeFileIO::PutBYTE( fp, 0xef );
		fclose( fp );
	}
	CFile file;
	file.Init();
	if( file.Load( _T("SerializeFileIOテスト.bin") ) )
	{
		DWORD dw;
		BYTE by;
		TCHAR s[128];
		DWORD p = 0;
		dw = 0; SerializeFileIO::GetDWORD( &file, &p, &dw );
		Debug::Out(_T("%08x\n"), dw );
		dw = 0; SerializeFileIO::GetWORD( &file, &p, &dw );
		Debug::Out(_T("%08x\n"), dw );
		SerializeFileIO::GetString( &file, &p, s, 128 );
		Debug::Out(_T("%s\n"), s );
		SerializeFileIO::GetString( &file, &p, s, 3 );
		Debug::Out(_T("%s\n"), s );
		TCHAR buf[1024];
		SerializeFileIO::GetString( &file, &p, buf, 1024 );
		Debug::Out(_T("%s\n"), buf );
		dw = 0; SerializeFileIO::GetBYTE( &file, &p, &by );
		Debug::Out(_T("%08x\n"), by );
	}
}

void SerializeFileIO::PutBYTE( FILE *fp, BYTE b )
{
	_ASSERT( fp );
	fwrite( &b, 1, 1, fp );
}

bool SerializeFileIO::GetBYTE( CFile *file, DWORD *p, BYTE *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 1 ) {	// 足らない
		*var = 0;
		return false;
	}

	*var = *(file->GetData() + *p);
	*p = *p + 1;
	return true;
}
bool SerializeFileIO::GetBYTE( CFileMM *file, DWORD *p, BYTE *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 1 ) {	// 足らない
		*var = 0;
		return false;
	}

	*var = *(file->GetData() + *p);
	*p = *p + 1;
	return true;
}
void SerializeFileIO::PutWORD( FILE *fp, WORD w )
{
	_ASSERT( fp );

	BYTE buf[2];
	buf[0] = (BYTE)((w>>8) & 0xFF);
	buf[1] = (BYTE)(w & 0xFF);
	fwrite( buf, 2, 1, fp );
}

bool SerializeFileIO::GetWORD( CFile *file, DWORD *p, DWORD *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 2 ) {	// 足らない
		*var = 0;
		return false;
	}

	LPBYTE s = file->GetData() + *p;
	*var = (((WORD)(*s))<<8) + (WORD)(*(s+1));
	*p = *p + 2;
	return true;
}

bool SerializeFileIO::GetWORD( CFileMM *file, DWORD *p, DWORD *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 2 ) {	// 足らない
		*var = 0;
		return false;
	}

	LPBYTE s = file->GetData() + *p;
	*var = (((WORD)(*s))<<8) + (WORD)(*(s+1));
	*p = *p + 2;
	return true;
}

void SerializeFileIO::PutDWORD( FILE *fp, DWORD dw )
{
	_ASSERT( fp );

	BYTE buf[4];
	buf[0] = (BYTE)((dw>>24) & 0xFF);
	buf[1] = (BYTE)((dw>>16) & 0xFF);
	buf[2] = (BYTE)((dw>>8) & 0xFF);
	buf[3] = (BYTE)(dw & 0xFF);
	fwrite( buf, 4, 1, fp );
}

bool SerializeFileIO::GetDWORD( CFile *file, DWORD *p, DWORD *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 4 ) {	// 足らない
		*var = 0;
		return false;
	}

	LPBYTE src = file->GetData() + *p;
	*var = (((DWORD)(*src))<<24) + (((DWORD)(*(src+1)))<<16) + (((DWORD)(*(src+2)))<<8) + (DWORD)(*(src+3));
	*p = *p + 4;
	return true;
}

bool SerializeFileIO::GetDWORD( CFileMM *file, DWORD *p, DWORD *var )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( var );

	if( file->GetSize() - *p < 4 ) {	// 足らない
		*var = 0;
		return false;
	}

	LPBYTE src = file->GetData() + *p;
	*var = (((DWORD)(*src))<<24) + (((DWORD)(*(src+1)))<<16) + (((DWORD)(*(src+2)))<<8) + (DWORD)(*(src+3));
	*p = *p + 4;
	return true;
}

void SerializeFileIO::PutString( FILE *fp, LPCTSTR str )
{
	_ASSERT( fp );
	
	if( str == NULL )		// 文字列が NULL なら 空文字列("")とみなして出力する。
	{
		PutWORD( fp, 0 );
		PutBYTE( fp, 0x00 );
		return;
	}

	// 文字数（null含まず）の出力
	WORD len = lstrlen( str );		// lstrlen() は TCHAR 単位で文字数を返す
	PutWORD( fp, len );

	// TCHAR を BYTE[] に直しつつ PutBYTE する
	union {
		TCHAR	w;
		BYTE	b[4];
	} cv;

	for( int i = 0; i < len+1; i++ )
	{
		cv.w = str[i];
		for( int j = 0; j < sizeof(TCHAR); j++ )
			PutBYTE( fp, cv.b[ sizeof(TCHAR) - j - 1 ] );
	}
}

bool SerializeFileIO::GetString( CFile *file, DWORD *p読出開始位置, LPTSTR p文字列格納バッファ, DWORD n文字列格納バッファの最大長 )
{
	_ASSERT( file );
	_ASSERT( p読出開始位置 );
	_ASSERT( p文字列格納バッファ );
	_ASSERT( n文字列格納バッファの最大長 != 0 );

	// 文字数（null含まず）の取得
	DWORD len;
	if( ! GetWORD( file, p読出開始位置, &len ) ) {	// 取得失敗
		*p文字列格納バッファ = _T('\0');
		return false;
	}

	// 文字列の取得; BYTE[] で取得しつつ TCHAR に直す
	if( file->GetSize() - *p読出開始位置 < len ) {	// 足らない
		*p文字列格納バッファ = _T('\0');
		return false;
	}
	union {
		TCHAR w;
		BYTE  b[4];
	} cv;

	LPBYTE s = file->GetData() + *p読出開始位置;
	for( DWORD i = 0; i < len+1; i++ )
	{
		for( int j = 0; j < sizeof(TCHAR); j++ )
		{
			BYTE by;
			if( ! GetBYTE( file, p読出開始位置, &by ) ) {
				*(p文字列格納バッファ+i) = _T('\0');
				return false;
			}
			cv.b[ sizeof(TCHAR) - j - 1 ] = by;
		}
		if( i < n文字列格納バッファの最大長 -1 )
			*(p文字列格納バッファ+i) = cv.w;
		else
			*(p文字列格納バッファ+n文字列格納バッファの最大長-1) = _T('\0');		// 最大長を越えた文字はすべて無視
	}
	return true;
}

bool SerializeFileIO::GetString( CFileMM *file, DWORD *p, LPTSTR str, DWORD str_maxlen )
{
	_ASSERT( file );
	_ASSERT( p );
	_ASSERT( str );
	_ASSERT( str_maxlen != 0 );

	// 文字数（null含まず）の取得
	DWORD len;
	if( ! GetWORD( file, p, &len ) ) {	// 取得失敗
		*str = _T('\0');
		return false;
	}

	// 文字列の取得; BYTE[] で取得しつつ TCHAR に直す
	if( file->GetSize() - *p < len ) {	// 足らない
		*str = _T('\0');
		return false;
	}
	union {
		TCHAR w;
		BYTE  b[4];
	} cv;

	LPBYTE s = file->GetData() + *p;
	for( DWORD i = 0; i < len+1; i++ )
	{
		for( int j = 0; j < sizeof(TCHAR); j++ )
		{
			BYTE by;
			if( ! GetBYTE( file, p, &by ) ) {
				*(str+i) = _T('\0');
				return false;
			}
			cv.b[ sizeof(TCHAR) - j - 1 ] = by;
		}
		if( i < str_maxlen -1 )
			*(str+i) = cv.w;
		else
			*(str+str_maxlen-1) = _T('\0');		// str_maxlen を越えた文字はすべて無視
	}
	return true;
}

	}//General
}//FDK