using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏Guitarチップファイア : CAct演奏チップファイアGB
	{
		// コンストラクタ

		public CAct演奏Guitarチップファイア()
		{
			base.b活性化してない = true;
		}
		
		
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
			int y = 演奏判定ライン座標.n判定ラインY座標( e楽器パート, true, CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] );
			//if ( CDTXMania.ConfigIni.bReverse[ (int)e楽器パート ] )
			//{
			//    y = 369;
			//}
			base.Start( nLane, x, y, 演奏判定ライン座標 );
		}


		// その他

		#region [ private ]
		//-----------------
		private readonly Point[] pt中央 = new Point[] {
			new Point(  42, 40 ),	// GtR
			new Point(  78, 40 ),	// GtG
			new Point( 114, 40 ),	// GtB
			new Point( 496, 40 ),	// BsR
			new Point( 532, 40 ),	// BsG
			new Point( 568, 40 )	// BsB
		};
		//-----------------
		#endregion
	}
}
