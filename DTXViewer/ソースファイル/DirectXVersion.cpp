#include "stdafx.h"
#include "GetDXVersion.h"
#include "DirectXVersion.h"

namespace DTXMania {
	namespace General {

DirectXVersion::DirectXVersion()
{
	this->dwMajor = 0;
	this->dwMinor = 0;
	this->dwAppx  = 0;
}

void DirectXVersion::tDirectX‚Ìƒo[ƒWƒ‡ƒ“î•ñ‚ðŽæ“¾‚·‚é()
{
	DWORD dwDirectXVersion;
	if( FAILED( GetDXVersion( &dwDirectXVersion ) ) )
		return;

	this->dwMajor =  HIWORD( dwDirectXVersion );
	this->dwMinor =  LOWORD( dwDirectXVersion ) & 0xFF00;
	this->dwAppx  = (LOWORD( dwDirectXVersion ) & 0x00FF) - 1;
}

	}//General
}//DTXMania
