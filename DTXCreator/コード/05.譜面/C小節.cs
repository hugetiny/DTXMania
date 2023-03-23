using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DTXCreator.譜面
{
	public class C小節 : IComparable<C小節>
	{
		public float f小節長倍率 = 1f;
		public List<Cチップ> listチップ = new List<Cチップ>();
		public static int n基準の高さdot  = CWholeNoteDivision.n分解能_表示用; //0xc0;
		public static int n基準の高さgrid = CWholeNoteDivision.n分解能; //0xc0;
		public static float n１グリッドの高さdot
		{
			get
			{
				//return ( n基準の高さdot < n基準の高さgrid ) ? 1 : (n基準の高さdot / n基準の高さgrid);
				return ( n基準の高さdot / n基準の高さgrid );
			}
		}
		public int n小節長倍率を考慮した現在の小節の高さdot
		{
			get
			{
				return (int) ( n基準の高さdot * this.f小節長倍率 );
			}
		}
		public int n小節長倍率を考慮した現在の小節の高さgrid
		{
			get
			{
				return (int) ( n基準の高さgrid * this.f小節長倍率 );
			}
		}
		public int n小節番号0to3599
		{
			get
			{
				return this._n小節番号0to3599;
			}
			set
			{
				if( ( value < 0 ) || ( value > 0xe0f ) )
				{
					throw new Exception( "値が範囲(0～3599)を超えています。-->[" + value + "]" );
				}
				this._n小節番号0to3599 = value;
			}
		}

		public C小節( int n小節番号0to3599 )
		{
			this._n小節番号0to3599 = n小節番号0to3599;
		}
		public int CompareTo( C小節 other )
		{
			return ( this._n小節番号0to3599 - other._n小節番号0to3599 );
		}
		public int n位置変換count2grid( int nCount )
		{
			return (int) ( ( ( n基準の高さgrid * this.f小節長倍率 ) * nCount ) / CWholeNoteDivision.n分解能*2f );
		}
		public static int n位置変換dot2grid( int nDot )
		{
			return ( ( nDot * n基準の高さgrid ) / n基準の高さdot );
		}
		public int n位置変換grid2count( int nGrid )
		{
			return (int) ( ( nGrid * CWholeNoteDivision.n分解能*2f ) / ( n基準の高さgrid * this.f小節長倍率 ) );
		}
		public static int n位置変換grid2dot( int nGrid )
		{
			return ( ( nGrid * n基準の高さdot ) / n基準の高さgrid );
		}
		public void t小節内の全チップの移動済フラグをリセットする()
		{
			for( int i = 0; i < this.listチップ.Count; i++ )
			{
				this.listチップ[ i ].b移動済 = false;
			}
		}
		public void t小節内の全チップの選択を解除する()
		{
			for( int i = 0; i < this.listチップ.Count; i++ )
			{
				Cチップ cチップ = this.listチップ[ i ];
				if( cチップ.b確定選択中 )
				{
					this.listチップ[ i ].bドラッグで選択中 = false;
					this.listチップ[ i ].b確定選択中 = false;
				}
			}
		}

		#region [ private ]
		//-----------------
		private int _n小節番号0to3599;
		//-----------------
		#endregion
	}
}
