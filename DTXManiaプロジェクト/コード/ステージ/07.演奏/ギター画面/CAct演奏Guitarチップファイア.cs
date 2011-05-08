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

		public override void Start( int nLane )
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
			int y = this.pt中央[ index ].Y;
			if( CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] )
			{
				y = ( nLane < 3 ) ? 0x171 : 0x171;
			}
			base.Start( nLane, x, y );
		}


		// その他

		#region [ private ]
		//-----------------
		private readonly Point[] pt中央 = new Point[] { new Point( 0x2a, 40 ), new Point( 0x4e, 40 ), new Point( 0x72, 40 ), new Point( 0x1f0, 40 ), new Point( 0x214, 40 ), new Point( 0x238, 40 ) };
		//-----------------
		#endregion
	}
}
