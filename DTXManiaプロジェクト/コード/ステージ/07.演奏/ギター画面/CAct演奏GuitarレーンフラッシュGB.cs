using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏GuitarレーンフラッシュGB : CAct演奏レーンフラッシュGB共通
	{
		// コンストラクタ

		public CAct演奏GuitarレーンフラッシュGB()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装（共通クラスからの差分のみ）

		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 6; i++ )
				{
					if( !base.ct進行[ i ].b停止中 )
					{
						E楽器パート e楽器パート = ( i < 3 ) ? E楽器パート.GUITAR : E楽器パート.BASS;
						CTexture texture = CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] ? base.txFlush[ ( i % 3 ) + 3 ] : base.txFlush[ i % 3 ];
						int num2 = CDTXMania.ConfigIni.bLeft[ (int) e楽器パート ] ? 1 : 0;
						for( int j = 0; j < 3; j++ )
						{
							int x = ( ( ( i < 3 ) ? 0x1a : 480 ) + this.nRGBのX座標[ num2, i ] ) + ( ( 0x10 * base.ct進行[ i ].n現在の値 ) / 100 );
							int y = CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] ? ( 0x37 + ( j * 0x76 ) ) : ( j * 0x76 );
							if( texture != null )
							{
								texture.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( j * 0x20, 0, ( 0x20 * ( 100 - base.ct進行[ i ].n現在の値 ) ) / 100, 0x76 ) );
							}
						}
						base.ct進行[ i ].t進行();
						if( base.ct進行[ i ].b終了値に達した )
						{
							base.ct進行[ i ].t停止();
						}
					}
				}
			}
			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
		private readonly int[,] nRGBのX座標 = new int[ , ] { { 0, 0x24, 0x48, 0, 0x24, 0x48 }, { 0x48, 0x24, 0, 0x48, 0x24, 0 } };
		//-----------------
		#endregion
	}
}
