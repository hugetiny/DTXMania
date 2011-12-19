#pragma once

namespace FDK {
	namespace Graphics {

// 指定された PNG ファイルを読み込み、DDS 形式に変換して返す。
// 　pddsd2 ...... DDSURFACEDESC2 の値が設定されて返される。
// 　pppbyData ... image[][] が返される。不要になれば free() すること。
// 　　　　　　　　（行単位で free() すること！）
extern bool LoadPNGtoDDS( LPCTSTR fileName, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData );
extern bool LoadPNGtoDDS( FILE* fp, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData );

// 指定された PNG データから、DDS 形式に変換して返す。
// 　pData ....... PNG データ（ファイルイメージ）へのポインタ
// 　dwDataSize .. PNG データ（ファイルイメージ）のサイズ[bytes]
// 　pddsd2 ...... DDSURFACEDESC2 の値が設定されて返される。
// 　pppbyData ... image[][] が返される。不要になれば free() すること。
// 　　　　　　　　（行単位で free() すること！）
extern bool LoadPNGtoDDS( BYTE* pData, DWORD dwDataSize, DDSURFACEDESC2* pddsd2, BYTE*** pppbyData );

// PNG を読み込み、DIB に変換して返す。
// PNG のαはピクセルの色と合成されてなくなる。
extern bool LoadPNGtoDIB( LPCTSTR fileName, BITMAPINFO** ppInfo, BYTE** ppBMP );
extern bool LoadPNGtoDIB( FILE* fp, BITMAPINFO** ppInfo, BYTE** ppBMP );

// PNG をメモリイメージから読み込み、DIB に変換して返す。
// PNG のαはピクセルの色と合成されてなくなる。
extern bool LoadPNGtoDIB( BYTE* pData, DWORD dwDataSize, BITMAPINFO** ppInfo, BYTE** ppBMP );

	}//Graphics
}//FDK

using namespace FDK::Graphics;
