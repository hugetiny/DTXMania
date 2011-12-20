// SerializeFileIO
// ファイルへのバイトストリーム入出力。

#pragma once

#include "CFile.h"
#include "CFileMM.h"

namespace FDK {
	namespace General {

class SerializeFileIO
{
public:
	static void PutString( FILE *fp, LPCTSTR str );
	static void PutBYTE( FILE *fp, BYTE b );
	static void PutWORD( FILE *fp, WORD w );
	static void PutDWORD( FILE *fp, DWORD dw );

	static bool GetString( CFile *file, DWORD *p読出開始位置, LPTSTR str文字列格納バッファ, DWORD n文字列格納バッファの最大長 );
	static bool GetString( CFileMM *file, DWORD *s, LPTSTR str, DWORD str_maxlen );
	static bool GetBYTE( CFile *file, DWORD *p, BYTE *var );
	static bool GetBYTE( CFileMM *file, DWORD *p, BYTE *var );
	static bool GetWORD( CFile *file, DWORD *p, DWORD *var );
	static bool GetWORD( CFileMM *file, DWORD *p, DWORD *var );
	static bool GetDWORD( CFile *file, DWORD *p, DWORD *var );
	static bool GetDWORD( CFileMM *file, DWORD *p, DWORD *var );

	static void TEST();
};

	}//General
}//FDK

using namespace FDK::General;
