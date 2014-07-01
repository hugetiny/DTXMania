using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	internal class CAct演奏Guitarコンボ : CAct演奏Combo共通
	{
		// CAct演奏Combo共通 実装

		protected override void tコンボ表示・ギター( int nCombo値, int nジャンプインデックス )
		{
			int x = 230;
			int y = CDTXMania.ConfigIni.bReverse.Guitar ? 0x103 : 150;
			//int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.ConfigIni.bReverse.Guitar );
			//y += CDTXMania.ConfigIni.bReverse.Guitar ? -134 : +174;
			if ( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 0xff;
			}
			base.tコンボ表示・ギター( nCombo値, x, y, nジャンプインデックス );
		}
		protected override void tコンボ表示・ドラム( int nCombo値, int nジャンプインデックス )
		{
		}
		protected override void tコンボ表示・ベース( int nCombo値, int nジャンプインデックス )
		{
			int x = 410;
			int y = CDTXMania.ConfigIni.bReverse.Bass ? 0x103 : 150;
			//int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.ConfigIni.bReverse.Bass );
			//y += CDTXMania.ConfigIni.bReverse.Bass ? -134 : +174;
			if ( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 0xff;
			}
			base.tコンボ表示・ベース( nCombo値, x, y, nジャンプインデックス );
		}
	}
}
