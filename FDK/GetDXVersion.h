/****************************************************************************
    getdxver.h
 ****************************************************************************/
#pragma once

#pragma comment( lib, "version.lib" )

namespace FDK {
	namespace General {
		
		extern	HRESULT		GetDXVersion(DWORD* pdwDirectXVersion, TCHAR* strDirectXVersion = NULL, int cchDirectXVersion = 0);

	}// General
}//FDK

using namespace FDK::General;
