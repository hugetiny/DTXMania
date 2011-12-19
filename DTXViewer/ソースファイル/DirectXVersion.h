#pragma once

namespace DTXMania {
	namespace General {

class DirectXVersion
{
public:
	DWORD	dwMajor;		// メジャー番号
	DWORD	dwMinor;		// マイナー番号
	DWORD	dwAppx;			// Appendix ('a'～'z' → 0～25)

public:
	void tDirectXのバージョン情報を取得する();

public:
	DirectXVersion();
};

	}//General
}//DTXMania

using namespace DTXMania::General;
