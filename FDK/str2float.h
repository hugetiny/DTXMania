
#pragma once

namespace FDK {
	namespace General {
		
		// 文字列を小数値に変換する。_tstof() の前に事前チェックを入れる形。
		extern float str2float( LPCTSTR p );

	}// General
}//FDK

using namespace FDK::General;
