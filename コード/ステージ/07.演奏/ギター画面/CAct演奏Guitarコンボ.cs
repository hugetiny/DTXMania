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
			if( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 0xff;
			}
			base.tコンボ表示・ベース( nCombo値, x, y, nジャンプインデックス );
		}
		protected override void tコンボ表示・ドラム( int nCombo値, int nジャンプインデックス )
		{
		}
		protected override void tコンボ表示・ベース( int nCombo値, int nジャンプインデックス )
		{
			int x = 410;
			int y = CDTXMania.ConfigIni.bReverse.Bass ? 0x103 : 150;
			if( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 0xff;
			}
			base.tコンボ表示・ベース( nCombo値, x, y, nジャンプインデックス );
		}
	}
}
