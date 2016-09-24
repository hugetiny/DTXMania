
// stdafx.h
// めったに修正されないものをプリコンパイルしておくことでコンパイル速度を上げる。

#pragma once

#include "fdk10.h"

// zlib で出る「vc80.pdb にシンボルがない」警告を抑制
#pragma warning(disable: 4099)

#define	TEXTLEN	1024
#define READLEN 16384

