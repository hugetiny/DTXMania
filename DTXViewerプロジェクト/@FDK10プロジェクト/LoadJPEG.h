#pragma once

namespace FDK {
	namespace Graphics {

		// 指定された JPEG ファイルを読み込み、DIB 形式に変換して返す。
		// 　lppInfo ... BITMAPINFO へのアドレスが返される。不要になれば free() すること。
		// 　lppBMP .... 画像データが返される。不要になれば free() すること。
		extern int LoadJPEG( FILE* fp, BITMAPINFO** lppInfo, BYTE** lppBMP );

	}//Graphics
}//FDK

using namespace FDK::Graphics;
