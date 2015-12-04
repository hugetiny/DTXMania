using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace DTXMania
{
	internal class CAct演奏DrumsチップファイアGB : CAct演奏チップファイアGB
	{
		// メソッド

		public override void Start( int nLane, C演奏判定ライン座標共通 演奏判定ライン座標 )
		{
			if( ( nLane < 0 ) && ( nLane > 5 ) )
			{
				throw new IndexOutOfRangeException();
			}
			E楽器パート e楽器パート = ( nLane < 3 ) ? E楽器パート.GUITAR : E楽器パート.BASS;
			int index = nLane;
			if( CDTXMania.ConfigIni.bLeft[ (int) e楽器パート ] )
			{
				index = ( ( index / 3 ) * 3 ) + ( 2 - ( index % 3 ) );
			}
			int x = this.pt中央[ index ].X;
			if (CDTXMania.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
			{
				x -= ( e楽器パート == E楽器パート.GUITAR ) ? 71 : 994;
			}
			//int y = this.pt中央[ index ].Y;
			int y = 演奏判定ライン座標.n判定ラインY座標( e楽器パート, false, CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] );

			base.Start( nLane, x, y, 演奏判定ライン座標 );
		}


		// その他

		#region [ private ]
		//-----------------
		private readonly Point[] pt中央 = new Point[] {
			new Point( 519 * 3, (int) (95 * 2.25) ),	// GtR
			new Point( 545 * 3, (int) (95 * 2.25) ),	// GtG
			new Point( 571 * 3, (int) (95 * 2.25) ),	// GtB
			new Point( 410 * 3, (int) (95 * 2.25) ),	// BsR
			new Point( 436 * 3, (int) (95 * 2.25) ),	// BsG
			new Point( 462 * 3, (int) (95 * 2.25) )		// BsB
		};
		//-----------------
		#endregion
	}
}
