#include "stdafx.h"
#include "str2float.h"

namespace FDK {
	namespace General {

float str2float( LPCTSTR p )
{
	TCHAR buf[ 64 ];
	int nb = 0;
	bool bs = false;		// 小数点が現れたら true

	while( _istdigit( *p ) || *p == _T('.') )
	{
		if( *p == _T('.') ) {
			if( bs ) break;		// 小数点が２回以上でたらそこで終了
			bs = true;
		}
		buf[ nb++ ] = *p++;
		if( nb >= 63 ) break;	// 文字列制限（一応）
	}
	buf[ nb ] = _T('\0');

	// 小数に直して返還
	return (float) _tstof( buf );
}
	}//General
}//FDK