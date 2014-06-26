using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏GuitarRGB : CAct演奏RGB共通
	{
		// コンストラクタ

		public CAct演奏GuitarRGB()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装（共通クラスからの差分のみ）

		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(C演奏判定ライン座標共通 演奏判定ライン共通 ) のほうを使用してください。" );
		}
		public override int t進行描画( C演奏判定ライン座標共通 演奏判定ライン座標 )
		{
			if( !base.b活性化してない )
			{
				if( !CDTXMania.ConfigIni.bGuitar有効 )
				{
					return 0;
				}
				if( CDTXMania.DTX.bチップがある.Guitar )
				{
					for( int j = 0; j < 3; j++ )
					{
						int index = CDTXMania.ConfigIni.bLeft.Guitar ? ( 2 - j ) : j;
						Rectangle rectangle = new Rectangle( index * 0x18, 0, 0x18, 0x20 );
						if( base.b押下状態[ index ] )
						{
							rectangle.Y += 0x20;
						}
						if( base.txRGB != null )
						{
							int y = 演奏判定ライン座標.n演奏RGBボタンY座標( E楽器パート.GUITAR, true, CDTXMania.ConfigIni.bReverse.Guitar );
						//	int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, true, CDTXMania.ConfigIni.bReverse.Guitar, false, false );
							base.txRGB.t2D描画( CDTXMania.app.Device, 0x1f + ( j * 0x24 ), y, rectangle );
						}
					}
				}
				if( CDTXMania.DTX.bチップがある.Bass )
				{
					for( int k = 0; k < 3; k++ )
					{
						int index = CDTXMania.ConfigIni.bLeft.Bass ? ( 2 - k ) : k;
						Rectangle rectangle2 = new Rectangle( index * 0x18, 0, 0x18, 0x20 );
						if( base.b押下状態[ index + 3 ] )
						{
							rectangle2.Y += 0x20;
						}
						if( base.txRGB != null )
						{
							int y = 演奏判定ライン座標.n演奏RGBボタンY座標( E楽器パート.BASS, true, CDTXMania.ConfigIni.bReverse.Bass );
							//int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, true, CDTXMania.ConfigIni.bReverse.Bass, false, false );
							base.txRGB.t2D描画( CDTXMania.app.Device, 0x1e5 + ( k * 0x24 ), y, rectangle2 );
						}
					}
				}
				for( int i = 0; i < 6; i++ )
				{
					base.b押下状態[ i ] = false;
				}
			}
			return 0;
		}
	}
}
