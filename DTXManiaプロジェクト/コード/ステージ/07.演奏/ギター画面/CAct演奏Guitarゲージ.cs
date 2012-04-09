using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Guitarゲージ : CAct演奏ゲージ共通
	{
		// プロパティ

//		public STDGBVALUE<double> db現在のゲージ値;


		// コンストラクタ

		public CAct演奏Guitarゲージ()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			// CAct演奏ゲージ共通.Init()に移動
//			this.db現在のゲージ値.Guitar = ( CDTXMania.ConfigIni.nRisky > 0 ) ? 1.0 : 0.66666666666666663;
//			this.db現在のゲージ値.Bass   = ( CDTXMania.ConfigIni.nRisky > 0 ) ? 1.0 : 0.66666666666666663;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct本体移動 = null;
			this.ct本体振動 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txゲージ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayGuitar gauge.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txゲージ );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				int num;
				int num9;
				if( base.b初めての進行描画 )
				{
					this.ct本体移動 = new CCounter( 0, 0x1a, 20, CDTXMania.Timer );
					this.ct本体振動 = new CCounter( 0, 360, 4, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				this.ct本体移動.t進行Loop();
				this.ct本体振動.t進行Loop();

				#region [ ギターのゲージ ]
				if ( this.db現在のゲージ値.Guitar == 1.0 )	// ギターのゲージ
				{
					num = (int) ( 128.0 * this.db現在のゲージ値.Guitar );
				}
				else
				{
					num = (int) ( ( 128.0 * this.db現在のゲージ値.Guitar ) + ( 2.0 * Math.Sin( Math.PI * 2 * ( ( (double) this.ct本体振動.n現在の値 ) / 360.0 ) ) ) );
				}
				if( num > 0 )
				{
					Rectangle rectangle;
					int num2 = 0x1a - this.ct本体移動.n現在の値;
					int x = 0xb2 - num2;
					int num4 = num + num2;
					while( num4 > 0 )
					{
						if( this.db現在のゲージ値.Guitar == 1.0 )
						{
							rectangle = new Rectangle( 0x1b, 0, 0x1b, 0x10 );
						}
						else
						{
							rectangle = new Rectangle( 0, 0, 0x1b, 0x10 );
						}
						if( x < 0xb2 )
						{
							int num5 = 0xb2 - x;
							rectangle.X += num5;
							rectangle.Width -= num5;
							x += num5;
						}
						if( ( x + rectangle.Width ) > ( 0xb2 + num ) )
						{
							int num6 = ( x + rectangle.Width ) - ( 0xb2 + num );
							rectangle.Width -= num6;
						}
						if( rectangle.Left >= rectangle.Right )
						{
							break;
						}
						if( this.txゲージ != null )
						{
							this.txゲージ.b加算合成 = false;
							this.txゲージ.t2D描画( CDTXMania.app.Device, x, 8, rectangle );
						}
						num4 -= rectangle.Width;
						x += rectangle.Width;
					}
					rectangle = new Rectangle( 0, 0x10, 0x40, 0x10 );
					x = ( 0xb2 + num ) - 0x40;
					if( x < 0xb2 )
					{
						int num7 = 0xb2 - x;
						rectangle.X += num7;
						rectangle.Width -= num7;
						x += num7;
					}
					if( ( x + rectangle.Width ) > ( 0xb2 + num ) )
					{
						int num8 = ( x + rectangle.Width ) - ( 0xb2 + num );
						rectangle.Width -= num8;
					}
					if( ( rectangle.Left < rectangle.Right ) && ( this.txゲージ != null ) )
					{
						this.txゲージ.b加算合成 = true;
						this.txゲージ.t2D描画( CDTXMania.app.Device, x, 8, rectangle );
					}
					if (this.bRisky && this.actLVLNFont != null)		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
					{
						CActLVLNFont.EFontColor efc = this.IsDanger( E楽器パート.GUITAR ) ?
							CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
						actLVLNFont.t文字列描画( 196, 6, nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Left );
					}
				}
				#endregion

				#region [ ベースのゲージ ]
				if ( this.db現在のゲージ値.Bass == 1.0 )
				{
					num9 = (int) ( 128.0 * this.db現在のゲージ値.Bass );
				}
				else
				{
					num9 = (int) ( ( 128.0 * this.db現在のゲージ値.Bass ) + ( 2.0 * Math.Sin( Math.PI * 2 * ( ( (double) this.ct本体振動.n現在の値 ) / 360.0 ) ) ) );
				}
				if( num9 > 0 )
				{
					Rectangle rectangle2;
					int num10 = this.ct本体移動.n現在の値;
					int num11 = ( 0x1cf - num9 ) - num10;
					int num12 = num9 + num10;
					while( num12 > 0 )
					{
						if( this.db現在のゲージ値.Bass == 1.0 )
						{
							rectangle2 = new Rectangle( 10, 0x30, 0x1b, 0x10 );
						}
						else
						{
							rectangle2 = new Rectangle( 0x25, 0x30, 0x1b, 0x10 );
						}
						if( num11 < ( 0x1cf - num9 ) )
						{
							int num13 = ( 0x1cf - num9 ) - num11;
							rectangle2.X += num13;
							rectangle2.Width -= num13;
							num11 += num13;
						}
						if( ( num11 + rectangle2.Width ) > 0x1cf )
						{
							int num14 = ( num11 + rectangle2.Width ) - 0x1cf;
							rectangle2.Width -= num14;
						}
						if( rectangle2.Left >= rectangle2.Right )
						{
							break;
						}
						if( this.txゲージ != null )
						{
							this.txゲージ.b加算合成 = false;
							this.txゲージ.t2D描画( CDTXMania.app.Device, num11, 8, rectangle2 );
						}
						num12 -= rectangle2.Width;
						num11 += rectangle2.Width;
					}
					rectangle2 = new Rectangle( 0, 0x20, 0x40, 0x10 );
					num11 = 0x1cf - num9;
					if( ( num11 + rectangle2.Width ) > 0x1cf )
					{
						int num15 = ( num11 + rectangle2.Width ) - 0x1cf;
						rectangle2.Width -= num15;
					}
					if( ( rectangle2.Left < rectangle2.Right ) && ( this.txゲージ != null ) )
					{
						this.txゲージ.b加算合成 = true;
						this.txゲージ.t2D描画( CDTXMania.app.Device, num11, 8, rectangle2 );
					}
					if (this.bRisky && this.actLVLNFont != null)		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
					{
						CActLVLNFont.EFontColor efc = this.IsDanger( E楽器パート.GUITAR ) ?
							CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
						actLVLNFont.t文字列描画( 445, 6, nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Right);
					}
				}
				#endregion
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		//private CCounter ct本体移動;
		//private CCounter ct本体振動;
		//private CTexture txゲージ;
		//-----------------
		#endregion
	}
}
