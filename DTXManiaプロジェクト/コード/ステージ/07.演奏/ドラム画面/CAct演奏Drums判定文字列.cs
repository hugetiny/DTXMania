using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			this.stレーンサイズ = new STレーンサイズ[ 12 ]
			{
				new STレーンサイズ(  36 - 36, 36 ),
				new STレーンサイズ(  77 - 36, 30 ),
				new STレーンサイズ( 111 - 36, 30 ),
				new STレーンサイズ( 146 - 36, 42 ),
				new STレーンサイズ( 192 - 36, 30 ),
				new STレーンサイズ( 227 - 36, 30 ),
				new STレーンサイズ( 261 - 36, 30 ),
				new STレーンサイズ( 295 - 36, 36 ),
				new STレーンサイズ(   0 - 36, 0 ),
				new STレーンサイズ(   0 - 36, 0 ),
				new STレーンサイズ( 507 - 36, 80 ),
				new STレーンサイズ( 398 - 36, 80 )
			};
			//for ( int i = 0; i < 12; i++ )
			//{
			//	this.stレーンサイズ[i] = new STレーンサイズ();
			//	this.stレーンサイズ[i].x = sizeXW[i, 0];
			//	this.stレーンサイズ[i].w = sizeXW[i, 1];
			//}
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
				#region [ 表示拡大率の設定 ]
				for ( int i = 0; i < 12; i++ )
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
								base.st状態[ i ].n相対Y座標 = ( ( num2 % 6 ) == 0 ) ? ( CDTXMania.app.Random.Next( 6 ) - 3 ) : base.st状態[ i ].n相対Y座標;
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
				#endregion
				for( int j = 0; j < 12; j++ )
				{
					if( !base.st状態[ j ].ct進行.b停止中 )
					{
						int index = base.st判定文字列[ (int) base.st状態[ j ].judge ].n画像番号;
						int baseX = 0;
						int baseY = 0;
						#region [ Drums 判定文字列 baseX/Y生成 ]
						if ( j < 8 )			// Drums
						{
							if ( CDTXMania.app.ConfigIni.判定文字表示位置.Drums == E判定文字表示位置.表示OFF )
							{
								continue;
							}
							baseX = this.stレーンサイズ[ j ].x;
							baseX = (int) ( baseX * ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 1.0 : 0.75 ) );
							baseX += ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left? 36 : 205 );
							baseY = CDTXMania.app.ConfigIni.bReverse.Drums ?
								( ( ( (E判定文字表示位置) CDTXMania.app.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.レーン上 ) ? ( 240 + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : 50 ) :
								( ( ( (E判定文字表示位置) CDTXMania.app.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.レーン上 ) ? ( 180 + ( this.n文字の縦表示位置[ j ] * 0x20 ) ) : 450 );
							baseY = (int) ( baseY * Scale.Y );
						}
						#endregion
						#region [ Bass 判定文字列描画 baseX/Y生成 ]
						else if( j == 11 )	// Bass
						{
							if( ( (E判定文字表示位置) CDTXMania.app.ConfigIni.判定文字表示位置.Bass ) == E判定文字表示位置.表示OFF )
							{
								continue;
							}
							int yB;
							switch ( CDTXMania.app.ConfigIni.判定文字表示位置.Bass )
							{
								case E判定文字表示位置.コンボ下:
									baseX = this.stレーンサイズ[ j ].x + 36;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX -= 331+1;
									}
									yB = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.app.ConfigIni.bReverse.Bass );
									baseY = (
												CDTXMania.app.ConfigIni.bReverse.Bass ?
													yB + (int) ( ( -124 + 0 ) * Scale.Y ) :
													yB + (int) ( ( +184 + 0 ) * Scale.Y )
											)
											+ (int) (  this.n文字の縦表示位置[ j ] * 0x20 * Scale.Y );
									break;
								case E判定文字表示位置.レーン上:
									baseX = this.stレーンサイズ[ j ].x + 36;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX -= 331+1;
									}
									//baseY = ( CDTXMania.app.ConfigIni.bReverse.Bass ? 240 : 180 ) + ( this.n文字の縦表示位置[ j ] * 0x20 );
									yB = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.app.ConfigIni.bReverse.Bass );
									baseY = (
												CDTXMania.app.ConfigIni.bReverse.Bass ?
													yB - (int) ( 134 * Scale.Y ) :
													yB + (int) (  75 * Scale.Y )
											)
											+ (int) ( this.n文字の縦表示位置[ j ] * 0x20 * Scale.Y );
									break;
								case E判定文字表示位置.判定ライン上:
									baseX = this.stレーンサイズ[ j ].x + 36;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX -= 331+1;
									}
									yB = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, CDTXMania.app.ConfigIni.bReverse.Bass );
									baseY = CDTXMania.app.ConfigIni.bReverse.Bass ?
										yB + (int) ( 20 * Scale.Y ) :
										yB - (int) ( 24 * Scale.Y );
									break;
							}
						}
						#endregion
						#region [ Guitar 判定文字列描画 baseX/Y生成 ]
						else if( j == 10 )	// Guitar
						{
							if( ( (E判定文字表示位置) CDTXMania.app.ConfigIni.判定文字表示位置.Guitar ) == E判定文字表示位置.表示OFF )
							{
								continue;
							}
							int yG;
							switch ( CDTXMania.app.ConfigIni.判定文字表示位置.Guitar )
							{
							    case E判定文字表示位置.コンボ下:
									baseX = ( CDTXMania.app.DTX.bチップがある.Bass ) ? this.stレーンサイズ[ j ].x + 36 : 0x198;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX = this.stレーンサイズ[ j ].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
										baseX -= 24 + 1;
									}
									yG = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.app.ConfigIni.bReverse.Guitar );
									baseY = (
												CDTXMania.app.ConfigIni.bReverse.Guitar ?
													yG + (int) ( ( -124 + 0 ) * Scale.Y ) :
													yG + (int) ( ( +184 + 0 ) * Scale.Y )
											)
											+ (int) (  this.n文字の縦表示位置[ j ] * 0x20 * Scale.Y );
									break;
							    case E判定文字表示位置.レーン上:
									baseX = ( CDTXMania.app.DTX.bチップがある.Bass ) ? this.stレーンサイズ[ j ].x + 36 : 0x198;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX = this.stレーンサイズ[ j ].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
										baseX -= 24+1;
									}
									//baseY = ( CDTXMania.app.ConfigIni.bReverse.Guitar ? 240 : 180 ) + ( this.n文字の縦表示位置[ j ] * 0x20 );
									yG = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.app.ConfigIni.bReverse.Guitar );
									baseY = (
												CDTXMania.app.ConfigIni.bReverse.Guitar ?
													yG - (int) ( 134 * Scale.Y ):
													yG + (int) (  75 * Scale.Y )
											)
											+ (int) ( this.n文字の縦表示位置[ j ] * 0x20 * Scale.Y );
									break;
							    case E判定文字表示位置.判定ライン上:
									baseX = ( CDTXMania.app.DTX.bチップがある.Bass ) ? this.stレーンサイズ[ j ].x + 36 : 0x198;
									if ( CDTXMania.app.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
									{
										baseX = this.stレーンサイズ[ j ].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
										baseX -= 24 + 1;
									}
									yG = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, CDTXMania.app.ConfigIni.bReverse.Guitar );
							        baseY = CDTXMania.app.ConfigIni.bReverse.Guitar ?
										yG + (int) ( 20 * Scale.Y ) :
										yG - (int) ( 24 * Scale.Y );
									break;
							}
						}
						#endregion
						int xc = (int) ( ( ( ( baseX + base.st状態[ j ].n相対X座標 ) + ( this.stレーンサイズ[ j ].w / 2 ) ) ) * Scale.X );	// Xcenter座標
						int x = xc - ( (int) ( ( ( 256f / 2 ) * base.st状態[ j ].fX方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) );
						int y = (int) (  baseY + ( base.st状態[ j ].n相対Y座標 ) * Scale.Y ) - ( (int) ( ( ( ( 256f / 3 ) * base.st状態[ j ].fY方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) / 2.0 ) );

						//int xc = ( ( baseX + base.st状態[ j ].n相対X座標 ) + ( this.stレーンサイズ[ j ].w / 2 ) );	// Xcenter座標
						//int x = xc - ( (int) ( ( 64f * base.st状態[ j ].fX方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) );
						//int y = ( baseY + base.st状態[ j ].n相対Y座標 ) - ( (int) ( ( ( 43f * base.st状態[ j ].fY方向拡大率 ) * ( ( j < 10 ) ? 1.0 : 0.7 ) ) / 2.0 ) );
						if( base.tx判定文字列[ index ] != null )
						{
							base.tx判定文字列[ index ].n透明度 = base.st状態[ j ].n透明度;
							base.tx判定文字列[ index ].vc拡大縮小倍率 = new Vector3( (float) ( base.st状態[ j ].fX方向拡大率 * ( ( j < 10 ) ? 1.0 : 0.7 ) ), (float) ( base.st状態[ j ].fY方向拡大率 * ( ( j < 10 ) ? 1.0 : 0.7 ) ), 1f );
							base.tx判定文字列[ index ].t2D描画(
								CDTXMania.app.Device,
								x,
								y,
								base.st判定文字列[ (int) base.st状態[ j ].judge ].rc
							);

							#region [ #25370 2011.6.3 yyagi ShowLag support ]
							if ( base.nShowLagType == (int) EShowLagType.ON ||
								 ( ( base.nShowLagType == (int) EShowLagType.GREAT_POOR ) && ( base.st状態[ j ].judge != E判定.Perfect ) ) )
							{
								if ( base.st状態[ j ].judge != E判定.Auto && base.txlag数値 != null )		// #25370 2011.2.1 yyagi
								{
									bool minus = false;
									int offsetX = 0;
									string strDispLag = base.st状態[ j ].nLag.ToString();
									if ( st状態[ j ].nLag < 0 )
									{
										minus = true;
									}
									//x = xc - strDispLag.Length * 15 / 2;
									x = xc - (int) ( ( strDispLag.Length * 15 / 2 ) * Scale.X );
									for ( int i = 0; i < strDispLag.Length; i++ )
									{
										int p = ( strDispLag[ i ] == '-' ) ? 11 : (int) ( strDispLag[ i ] - '0' );	//int.Parse(strDispLag[i]);
										p += minus ? 0 : 12;		// change color if it is minus value
										//base.txlag数値.t2D描画( CDTXMania.app.Device, x + offsetX, y + 34, base.stLag数値[ p ].rc );
										base.txlag数値.t2D描画(
											CDTXMania.app.Device,
											x + offsetX * Scale.X,
											y + 38 * Scale.Y,
											base.stLag数値[ p ].rc
										);
										offsetX += 12;	// 15 -> 12
									}
								}
							}
							#endregion
						}
					// Label_07FC: ;
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
			public STレーンサイズ( int x_, int w_ )
			{
				x = x_;
				w = w_;
			}

		}

		private readonly int[] n文字の縦表示位置 = new int[] { 1, 2, 0, 1, 3, 2, 1, 0, 0, 0, 1, 1 };
		private STレーンサイズ[] stレーンサイズ;
		//-----------------
		#endregion
	}
}
