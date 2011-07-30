using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsゲージ : CAct演奏ゲージ共通
	{
		// プロパティ

//		public double db現在のゲージ値
//		{
//			get
//			{
//				return this.dbゲージ値;
//			}
//			set
//			{
//				this.dbゲージ値 = value;
//				if( this.dbゲージ値 > 1.0 )
//				{
//					this.dbゲージ値 = 1.0;
//				}
//			}
//		}

		
		// コンストラクタ

		public CAct演奏Drumsゲージ()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			// CAct演奏ゲージ共通.Init()に移動
			// this.dbゲージ値 = ( CDTXMania.ConfigIni.nRisky > 0 ) ? 1.0 : 0.66666666666666663;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct本体振動 = null;
			this.ct本体移動 = null;
			for( int i = 0; i < 24; i++ )
			{
				this.st白い星[ i ].ct進行 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txゲージ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums gauge.png" ) );
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
			if ( !base.b活性化してない )
			{
				if ( base.b初めての進行描画 )
				{
					for( int k = 0; k < 0x18; k++ )
					{
						this.st白い星[ k ].x = 2 + CDTXMania.Random.Next( 4 );
						this.st白い星[ k ].fScale = 0.2f + ( CDTXMania.Random.Next( 2 ) * 0.05f );
						this.st白い星[ k ].ct進行 = new CCounter( 0, 0x160, 8 + CDTXMania.Random.Next( 4 ), CDTXMania.Timer );
						this.st白い星[ k ].ct進行.n現在の値 = CDTXMania.Random.Next( 0x160 );
					}
					this.ct本体移動 = new CCounter( 0, 0x1a, 20, CDTXMania.Timer );
					this.ct本体振動 = new CCounter( 0, 360, 4, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				this.ct本体移動.t進行Loop();
				this.ct本体振動.t進行Loop();

				if ( this.bRisky && this.actLVLNFont != null )		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
				{
					CActLVLNFont.EFontColor efc = this.IsDanger( E楽器パート.DRUMS ) ?
						CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
					actLVLNFont.t文字列描画( 15, 408, nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Right );
				}

				int num2 = ( this.dbゲージ値 == 1.0 ) ? ( (int) ( 352.0 * this.dbゲージ値 ) ) : ( (int) ( ( 352.0 * this.dbゲージ値 ) + ( 2.0 * Math.Sin( Math.PI * 2 * ( ( (double) this.ct本体振動.n現在の値 ) / 360.0 ) ) ) ) );
				if( num2 <= 0 )
				{
					return 0;
				}
				if( this.txゲージ != null )
				{
					this.txゲージ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
					this.txゲージ.n透明度 = 0xff;
					this.txゲージ.b加算合成 = false;
				}
				int num3 = this.ct本体移動.n現在の値;
				int y = ( 0x195 - num2 ) - num3;
				int num5 = num2 + num3;
				while( num5 > 0 )
				{
					Rectangle rectangle = ( this.dbゲージ値 == 1.0 ) ? new Rectangle( 0x10, 0, 0x10, 0x1b ) : new Rectangle( 0, 0, 0x10, 0x1b );
					if( y < ( 0x195 - num2 ) )
					{
						int num6 = ( 0x195 - num2 ) - y;
						rectangle.Y += num6;
						rectangle.Height -= num6;
						y += num6;
					}
					if( ( y + rectangle.Height ) > 0x195 )
					{
						int num7 = ( y + rectangle.Height ) - 0x195;
						rectangle.Height -= num7;
					}
					if( rectangle.Top >= rectangle.Bottom )
					{
						break;
					}
					if( this.txゲージ != null )
					{
						this.txゲージ.t2D描画( CDTXMania.app.Device, 6, y, rectangle );
					}
					num5 -= rectangle.Height;
					y += rectangle.Height;
				}
				if( this.txゲージ != null )
				{
					this.txゲージ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
					this.txゲージ.n透明度 = 180;
					this.txゲージ.b加算合成 = true;
				}
				for( int i = 0; i < 4; i++ )
				{
					Rectangle rectangle2 = new Rectangle( 0x40 + ( i * 0x10 ), 0, 0x10, 0x40 );
					int num9 = ( 0x195 - num2 ) + ( i * 0x40 );
					if( num9 >= 0x195 )
					{
						break;
					}
					if( ( num9 + rectangle2.Height ) > 0x195 )
					{
						int num10 = ( num9 + rectangle2.Height ) - 0x195;
						rectangle2.Height -= num10;
					}
					if( ( rectangle2.Top < rectangle2.Bottom ) && ( this.txゲージ != null ) )
					{
						this.txゲージ.t2D描画( CDTXMania.app.Device, 6, num9, rectangle2 );
					}
				}
				if( this.txゲージ != null )
				{
					this.txゲージ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
					this.txゲージ.n透明度 = 0xff;
					this.txゲージ.b加算合成 = false;
				}
				Rectangle rectangle3 = new Rectangle( 0x30, 0, 0x10, 0x10 );
				int num11 = 0x195 - num2;
				if( num11 < 0x195 )
				{
					if( ( num11 + rectangle3.Height ) > 0x195 )
					{
						int num12 = ( num11 + rectangle3.Height ) - 0x195;
						rectangle3.Height -= num12;
					}
					if( ( rectangle3.Top < rectangle3.Bottom ) && ( this.txゲージ != null ) )
					{
						this.txゲージ.t2D描画( CDTXMania.app.Device, 6, num11, rectangle3 );
					}
				}
				if( this.txゲージ != null )
				{
					this.txゲージ.b加算合成 = true;
				}
				for( int j = 0; j < 24; j++ )
				{
					this.st白い星[ j ].ct進行.t進行Loop();
					int x = 6 + this.st白い星[ j ].x;
					int num15 = ( 0x195 - num2 ) + ( 0x160 - this.st白い星[ j ].ct進行.n現在の値 );
					int num16 = ( this.st白い星[ j ].ct進行.n現在の値 < 0xb0 ) ? 0 : ( (int) ( 255.0 * ( ( (double) ( this.st白い星[ j ].ct進行.n現在の値 - 0xb0 ) ) / 176.0 ) ) );
					if( ( num16 != 0 ) && ( num15 < 0x191 ) )
					{
						Rectangle rectangle4 = new Rectangle( 0, 0x20, 0x20, 0x20 );
						if( this.txゲージ != null )
						{
							this.txゲージ.vc拡大縮小倍率 = new Vector3( this.st白い星[ j ].fScale, this.st白い星[ j ].fScale, 1f );
							this.txゲージ.n透明度 = num16;
							this.txゲージ.t2D描画( CDTXMania.app.Device, x, num15, rectangle4 );
						}
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct ST白い星
		{
			public int x;
			public float fScale;
			public CCounter ct進行;
		}

		//private CCounter ct本体移動;
		//private CCounter ct本体振動;
		//private double dbゲージ値;
		private const int STAR_MAX = 0x18;
		private ST白い星[] st白い星 = new ST白い星[ 0x18 ];
		//private CTexture txゲージ;
		//-----------------
		#endregion
	}
}
