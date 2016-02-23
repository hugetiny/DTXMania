using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	internal class CAct演奏DrumsコンボDGB : CAct演奏Combo共通
	{
		// CAct演奏Combo共通 実装

		protected override void tコンボ表示_ギター( int nCombo値, int nジャンプインデックス )
		{
			int x, y;
			if( CDTXMania.app.DTX.bチップがある.Bass || CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center )
			{
				x = (CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)? 1638 : 1567+5;
				//y = CDTXMania.app.ConfigIni.bReverse.Guitar ? 0xaf : 270;
				y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.app.ConfigIni.bReverse.Guitar );
				y += CDTXMania.app.ConfigIni.bReverse.Guitar ? (int) ( -134 * Scale.Y ) : (int) ( +174 * Scale.Y );
				if ( base.txCOMBOギター != null )
				{
					base.txCOMBOギター.n透明度 = 120;
				}
			}
			else
			{
				x = 1344;
				//y = CDTXMania.app.ConfigIni.bReverse.Guitar ? 0xee : 0xcf;
				y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.app.ConfigIni.bReverse.Guitar );
				y += CDTXMania.app.ConfigIni.bReverse.Guitar ? (int) ( -134 * Scale.Y ) : (int) ( +174 * Scale.Y );
				if ( base.txCOMBOギター != null )
				{
					base.txCOMBOギター.n透明度 = 0xff;
				}
			}
			base.tコンボ表示_ギター( nCombo値, x, y, nジャンプインデックス );
		}
		protected override void tコンボ表示_ドラム( int nCombo値, int nジャンプインデックス )
		{
			base.tコンボ表示_ドラム( nCombo値, nジャンプインデックス );
		}
		protected override void tコンボ表示_ベース( int nCombo値, int nジャンプインデックス )
		{
			int x = ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ) ? 1311 : 1311-994+5;
			//int y = CDTXMania.app.ConfigIni.bReverse.Bass ? 0xaf : 270;
			int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.app.ConfigIni.bReverse.Bass );
			y += CDTXMania.app.ConfigIni.bReverse.Bass ? (int) ( -134 * Scale.Y ) : (int) ( +174 * Scale.Y );
			if ( base.txCOMBOギター != null )
			{
				base.txCOMBOギター.n透明度 = 120;
			}
			base.tコンボ表示_ベース( nCombo値, x, y, nジャンプインデックス );
		}
	}
}
