using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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
			//int y = this.pt中央[ index ].Y;
			int y = 演奏判定ライン座標.n判定ラインY座標( e楽器パート, false, CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] );
			//if ( CDTXMania.ConfigIni.bReverse[ (int)e楽器パート ] )
			//{
			//    y = 374;

			//}
			base.Start( nLane, x, y, 演奏判定ライン座標 );

		}


		// その他

		#region [ private ]
		//-----------------
		private readonly Point[] pt中央 = new Point[] {
			new Point( 519, 95 ),	// GtR
			new Point( 545, 95 ),	// GtG
			new Point( 571, 95 ),	// GtB
			new Point( 410, 95 ),		// BsR
			new Point( 436, 95 ),	// BsG
			new Point( 462, 95 )	// BsB
		};
		//-----------------
		#endregion
	}
}
