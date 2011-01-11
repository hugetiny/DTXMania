using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX;

namespace DTXMania
{
	internal class CAct演奏Drums判定文字列 : CAct演奏判定文字列共通
	{
		// コンストラクタ

		public CAct演奏Drums判定文字列()
		{
			STレーンサイズ[] stレーンサイズArray = new STレーンサイズ[ 12 ];
			STレーンサイズ stレーンサイズ = new STレーンサイズ();
			stレーンサイズ.x = 0x24;
			stレーンサイズ.w = 0x24;
			stレーンサイズArray[ 0 ] = stレーンサイズ;
			STレーンサイズ stレーンサイズ2 = new STレーンサイズ();
			stレーンサイズ2.x = 0x4d;
			stレーンサイズ2.w = 30;
			stレーンサイズArray[ 1 ] = stレーンサイズ2;
			STレーンサイズ stレーンサイズ3 = new STレーンサイズ();
			stレーンサイズ3.x = 0x6f;
			stレーンサイズ3.w = 30;
			stレーンサイズArray[ 2 ] = stレーンサイズ3;
			STレーンサイズ stレーンサイズ4 = new STレーンサイズ();
			stレーンサイズ4.x = 0x92;
			stレーンサイズ4.w = 0x2a;
			stレーンサイズArray[ 3 ] = stレーンサイズ4;
			STレーンサイズ stレーンサイズ5 = new STレーンサイズ();
			stレーンサイズ5.x = 0xc1;
			stレーンサイズ5.w = 30;
			stレーンサイズArray[ 4 ] = stレーンサイズ5;
			STレーンサイズ stレーンサイズ6 = new STレーンサイズ();
			stレーンサイズ6.x = 0xe3;
			stレーンサイズ6.w = 30;
			stレーンサイズArray[ 5 ] = stレーンサイズ6;
			STレーンサイズ stレーンサイズ7 = new STレーンサイズ();
			stレーンサイズ7.x = 0x105;
			stレーンサイズ7.w = 30;
			stレーンサイズArray[ 6 ] = stレーンサイズ7;
			STレーンサイズ stレーンサイズ8 = new STレーンサイズ();
			stレーンサイズ8.x = 0x127;
			stレーンサイズ8.w = 0x24;
			stレーンサイズArray[ 7 ] = stレーンサイズ8;
			STレーンサイズ stレーンサイズ9 = new STレーンサイズ();
			stレーンサイズ9.x = 0;
			stレーンサイズ9.w = 0;
			stレーンサイズArray[ 8 ] = stレーンサイズ9;
			STレーンサイズ stレーンサイズ10 = new STレーンサイズ();
			stレーンサイズ10.x = 0;
			stレーンサイズ10.w = 0;
			stレーンサイズArray[ 9 ] = stレーンサイズ10;
			STレーンサイズ stレーンサイズ11 = new STレーンサイズ();
			stレーンサイズ11.x = 0x1fb;
			stレーンサイズ11.w = 80;
			stレーンサイズArray[ 10 ] = stレーンサイズ11;
			STレーンサイズ stレーンサイズ12 = new STレーンサイズ();
			stレーンサイズ12.x = 0x18e;
			stレーンサイズ12.w = 80;
			stレーンサイズArray[ 11 ] = stレーンサイズ12;
			this.stレーンサイズ = stレーンサイズArray;
			base.b活性化してない = true;
		}
		
		
		// CActivity 実装（共通クラスからの差分のみ）

		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 12; i++ )
				{
					if( !base.st状態[ i ].ct進行.b停止中 )
					{
						base.st状態[ i ].ct進行.t進行();
						if( base.st状態[ i ].ct進行.b終了値に達した )
						{
							base.st状態[ i ].ct進行.t停止();
						}
						int num2 = base.st状態[ i ].ct進行.n現在の値;
						if( ( base.st状態[ i ].judge != E判定.Miss ) && ( base.st状態[ i ].judge != E判定.Bad ) )
						{
							if( num2 < 50 )
							{
								base.st状態[ i ].fX方向拡大率 = 1f + ( 1f * ( 1f - ( ( (float) num2 ) / 50f ) ) );
								base.st状態[ i ].fY方向拡大率 = ( (float) num2 ) / 50f;
								base.st状態[ i ].n相対X座標 = 0;
								base.st状態[ i ].n相対Y座標 = 0;
								base.st状態[ i ].n透明度 = 0xff;
							}
							else if( num2 < 130 )
							{
								base.st状態[ i ].fX方向拡大率 = 1f;
								base.st状態[ i ].fY方向拡大率 = 1f;
								base.st状態[ i ].n相対X座標 = 0;
								base.st状態[ i ].n相対Y座標 = ( ( num2 % 6 ) == 0 ) ? ( CDTXMania.Random.Next( 6 ) - 3 ) : base.st状態[ i ].n相対Y座標;
								base.st状態[ i ].n透明度 = 0xff;
							}
							else if( num2 >= 240 )
							{
								base.st状態[ i ].fX方向拡大率 = 1f;
								base.st状態[ i ].fY方向拡大率 = 1f - ( ( 1f * ( num2 - 240 ) ) / 60f );
								base.st状態[ i ].n相対X座標 = 0;
								base.st状態[ i ].n相対Y座標 = 0;
								base.st状態[ i ].n透明度 = 0xff;
							}
							else
							{
								base.st状態[ i ].fX方向拡大率 = 1f;
								base.st状態[ i ].fY方向拡大率 = 1f;
								base.st状態[ i ].n相対X座標 = 0;
								base.st状態[ i ].n相対Y座標 = 0;
								base.st状態[ i ].n透明度 = 0xff;
							}
						}
						else if( num2 < 50 )
						{
							base.st状態[ i ].fX方向拡大率 = 1f;
							base.st状態[ i ].fY方向拡大率 = ( (float) num2 ) / 50f;
							base.st状態[ i ].n相対X座標 = 0;
							base.st状態[ i ].n相対Y座標 = 0;
							base.st状態[ i ].n透明度 = 0xff;
						}
						else if( num2 >= 200 )
						{
							base.st状態[ i ].fX方向拡大率 = 1f - ( ( (float) ( num2 - 200 ) ) / 100f );
							base.st状態[ i ].fY方向拡大率 = 1f - ( ( (float) ( num2 - 200 ) ) / 100f );
							base.st状態[ i ].n相対X座標 = 0;
							base.st状態[ i ].n相対Y座標 = 0;
							base.st状態[ i ].n透明度 = 0xff;
						}
						else
						{
							base.st状態[ i ].fX方向拡大率 = 1f;
							base.st状態[ i ].fY方向拡大率 = 1f;
							base.st状態[ i ].n相対X座標 = 0;
							base.st状態[ i ].n相対Y座標 = 0;
							base.st状態[ i ].n透明度 = 0xff;
						}
					}
				}
				for( int j = 0; j < 12; j++ )
				{
					if( !base.st状態[ j ].ct進行.b停止中 )
					{
						int index = base.st判定文字列[ (int) base.st状態[ j ].judge ].n画像番号;
						int num5 = 0;
						int num6 = 0;
						if( j < 8 )
						{
							num5 = this.stレーンサイズ[ j ].x;
							num6 = CDTXMania.ConfigIni.bReverse.Drums ? ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.レーン上 ) ? ( 240 + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : 50 ) : ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.レーン上 ) ? ( 180 + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : 450 );
						}
						else if( j == 11 )
						{
							if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Bass ) == E判定文字表示位置.表示OFF )
							{
								goto Label_07FC;
							}
							num5 = this.stレーンサイズ[ j ].x;
							num6 = CDTXMania.ConfigIni.bReverse.Bass ? ( ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Bass ) == E判定文字表示位置.レーン上 ) ? 240 : 100 ) + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : ( ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Bass ) == E判定文字表示位置.レーン上 ) ? 180 : 300 ) + ( this.n文字の縦表示位置[ j ] * 0x20 ) );
						}
						else if( j == 10 )
						{
							if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) == E判定文字表示位置.表示OFF )
							{
								goto Label_07FC;
							}
							if( !CDTXMania.DTX.bチップがある.Bass && ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) == E判定文字表示位置.判定ライン上または横 ) )
							{
								num5 = 0x198;
								num6 = 0x93;
							}
							else
							{
								num5 = this.stレーンサイズ[ j ].x;
								num6 = CDTXMania.ConfigIni.bReverse.Guitar ? ( ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) == E判定文字表示位置.レーン上 ) ? 240 : 100 ) + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : ( ( ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) == E判定文字表示位置.レーン上 ) ? 180 : 300 ) + ( this.n文字の縦表示位置[ j ] * 0x20 ) );
							}
						}
						int x = ( ( num5 + base.st状態[ j ].n相対X座標 ) + ( this.stレーンサイズ[ j ].w / 2 ) ) - ( (int) ( ( 64f * base.st状態[ j ].fX方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) );
						int y = ( num6 + base.st状態[ j ].n相対Y座標 ) - ( (int) ( ( ( 43f * base.st状態[ j ].fY方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) / 2.0 ) );
						if( base.tx判定文字列[ index ] != null )
						{
							base.tx判定文字列[ index ].n透明度 = base.st状態[ j ].n透明度;
							base.tx判定文字列[ index ].vc拡大縮小倍率 = new Vector3( (float) ( base.st状態[ j ].fX方向拡大率 * ( ( j < 10 ) ? 1.0 : 0.7 ) ), (float) ( base.st状態[ j ].fY方向拡大率 * ( ( j < 10 ) ? 1.0 : 0.7 ) ), 1f );
							base.tx判定文字列[ index ].t2D描画( CDTXMania.app.Device, x, y, base.st判定文字列[ (int) base.st状態[ j ].judge ].rc );
						}
					Label_07FC: ;
					}
				}
			}
			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STレーンサイズ
		{
			public int x;
			public int w;
		}

		private readonly int[] n文字の縦表示位置 = new int[] { 1, 2, 0, 1, 3, 2, 1, 0, 0, 0, 1, 1 };
		private STレーンサイズ[] stレーンサイズ;
		//-----------------
		#endregion
	}
}
