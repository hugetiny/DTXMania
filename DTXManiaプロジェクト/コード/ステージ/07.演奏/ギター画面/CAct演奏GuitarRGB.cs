﻿using System;
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
				if( !CDTXMania.Instance.ConfigIni.bGuitar有効 )
				{
					return 0;
				}
				if( CDTXMania.Instance.DTX.bチップがある.Guitar )
				{
					for( int j = 0; j < 3; j++ )
					{
						int index = CDTXMania.Instance.ConfigIni.bLeft.Guitar ? ( 2 - j ) : j;
						Rectangle rectangle = new Rectangle(
							(int) ( index * 0x18 * Scale.X ),
							0,
							(int) ( 0x18 * Scale.X ),
							(int) ( 0x20 * Scale.Y )
						);
						if( base.b押下状態[ index ] )
						{
							rectangle.Y += (int) ( 0x20 * Scale.Y );
						}
						if( base.txRGB != null )
						{
							int y = 演奏判定ライン座標.n演奏RGBボタンY座標( E楽器パート.GUITAR, true, CDTXMania.Instance.ConfigIni.bReverse.Guitar );
						//	int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, true, CDTXMania.Instance.ConfigIni.bReverse.Guitar, false, false );
							base.txRGB.t2D描画(
								CDTXMania.Instance.Device,
								( 0x1f + ( j * 0x24 ) ) * Scale.X,
								y,
								rectangle
							);
						}
					}
				}
				if( CDTXMania.Instance.DTX.bチップがある.Bass )
				{
					for( int k = 0; k < 3; k++ )
					{
						int index = CDTXMania.Instance.ConfigIni.bLeft.Bass ? ( 2 - k ) : k;
						Rectangle rectangle2 = new Rectangle(
							(int) ( index * 0x18 * Scale.X ),
							0,
							(int) ( 0x18 * Scale.X ),
							(int) ( 0x20 * Scale.Y )
						);
						if( base.b押下状態[ index + 3 ] )
						{
							rectangle2.Y += (int) ( 0x20 * Scale.Y );
						}
						if( base.txRGB != null )
						{
							int y = 演奏判定ライン座標.n演奏RGBボタンY座標( E楽器パート.BASS, true, CDTXMania.Instance.ConfigIni.bReverse.Bass );
							//int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, true, CDTXMania.Instance.ConfigIni.bReverse.Bass, false, false );
							base.txRGB.t2D描画(
								CDTXMania.Instance.Device,
								( 0x1e5 + ( k * 0x24 ) ) * Scale.X,
								y,
								rectangle2
							);
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
