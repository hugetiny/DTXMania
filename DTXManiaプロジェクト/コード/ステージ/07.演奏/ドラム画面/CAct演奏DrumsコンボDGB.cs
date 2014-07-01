using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	internal class CAct演奏DrumsコンボDGB : CAct演奏Combo共通
	{
		// CAct演奏Combo共通 実装

		protected override void tコンボ表示・ギター( int nCombo値, int nジャンプインデックス )
		{
			int x, y;
			if( CDTXMania.DTX.bチップがある.Bass )
			{
				x = 0x222;
				//y = CDTXMania.ConfigIni.bReverse.Guitar ? 0xaf : 270;
				y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.ConfigIni.bReverse.Guitar );
				y += CDTXMania.ConfigIni.bReverse.Guitar ? -134 : +174;
				if ( base.txCOMBOギター != null )
				{
					base.txCOMBOギター.n透明度 = 120;
				}
			}
			else
			{
				x = 0x1c0;
				//y = CDTXMania.ConfigIni.bReverse.Guitar ? 0xee : 0xcf;
				y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.ConfigIni.bReverse.Guitar );
				y += CDTXMania.ConfigIni.bReverse.Guitar ? -134 : +174;
				if ( base.txCOMBOギター != null )
				{
					base.txCOMBOギター.n透明度 = 0xff;
				}
			}
			base.tコンボ表示・ギター( nCombo値, x, y, nジャンプインデックス );
		}
		protected override void tコンボ表示・ドラム( int nCombo値, int nジャンプインデックス )
		{
			base.tコンボ表示・ドラム( nCombo値, nジャンプインデックス );
		}
		protected override void tコンボ表示・ベース( int nCombo値, int nジャンプインデックス )
		{
			int x = 0x1b5;
			//int y = CDTXMania.ConfigIni.bReverse.Bass ? 0xaf : 270;
			int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.ConfigIni.bReverse.Bass );
			y += CDTXMania.ConfigIni.bReverse.Bass ? -134 : +174;
			if ( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 120;
			}
			base.tコンボ表示・ベース( nCombo値, x, y, nジャンプインデックス );
		}
	}
}
