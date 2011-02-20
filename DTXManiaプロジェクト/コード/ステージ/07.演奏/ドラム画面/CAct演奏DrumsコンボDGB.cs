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
				y = CDTXMania.ConfigIni.bReverse.Guitar ? 0xaf : 270;
				if( base.txCOMBOギター != null )
				{
					base.txCOMBOギター.n透明度 = 120;
				}
			}
			else
			{
				x = 0x1c0;
				y = CDTXMania.ConfigIni.bReverse.Guitar ? 0xee : 0xcf;
				if( base.txCOMBOギター != null )
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
			int y = CDTXMania.ConfigIni.bReverse.Bass ? 0xaf : 270;
			if( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 120;
			}
			base.tコンボ表示・ベース( nCombo値, x, y, nジャンプインデックス );
		}
	}
}
