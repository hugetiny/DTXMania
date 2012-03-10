using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏DrumsレーンフラッシュD : CActivity
	{
		// コンストラクタ

		public CAct演奏DrumsレーンフラッシュD()
		{
			STレーンサイズ[] stレーンサイズArray = new STレーンサイズ[ 8 ];
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
			this.stレーンサイズ = stレーンサイズArray;
			this.strファイル名 = new string[] { @"Graphics\ScreenPlayDrums lane flush cymbal.png", @"Graphics\ScreenPlayDrums lane flush hihat.png", @"Graphics\ScreenPlayDrums lane flush snare.png", @"Graphics\ScreenPlayDrums lane flush bass.png", @"Graphics\ScreenPlayDrums lane flush hitom.png", @"Graphics\ScreenPlayDrums lane flush lowtom.png", @"Graphics\ScreenPlayDrums lane flush floortom.png", @"Graphics\ScreenPlayDrums lane flush cymbal.png", @"Graphics\ScreenPlayDrums lane flush cymbal reverse.png", @"Graphics\ScreenPlayDrums lane flush hihat reverse.png", @"Graphics\ScreenPlayDrums lane flush snare reverse.png", @"Graphics\ScreenPlayDrums lane flush bass reverse.png", @"Graphics\ScreenPlayDrums lane flush hitom reverse.png", @"Graphics\ScreenPlayDrums lane flush lowtom reverse.png", @"Graphics\ScreenPlayDrums lane flush floortom reverse.png", @"Graphics\ScreenPlayDrums lane flush cymbal reverse.png" };
			base.b活性化してない = true;
		}
		
		
		// メソッド

		public void Start( Eレーン lane, float f強弱度合い )
		{
			int num = (int) ( ( 1f - f強弱度合い ) * 55f );
			this.ct進行[ (int) lane ] = new CCounter( num, 100, 4, CDTXMania.Timer );
		}


		// CActivity 実装

		public override void On活性化()
		{
			for( int i = 0; i < 8; i++ )
			{
				this.ct進行[ i ] = new CCounter();
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			for( int i = 0; i < 8; i++ )
			{
				this.ct進行[ i ] = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 0x10; i++ )
				{
					this.txFlush[ i ] = CDTXMania.tテクスチャの生成( CSkin.Path( this.strファイル名[ i ] ) );
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 0x10; i++ )
				{
					CDTXMania.tテクスチャの解放( ref this.txFlush[ i ] );
				}
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 8; i++ )
				{
					if( !this.ct進行[ i ].b停止中 )
					{
						this.ct進行[ i ].t進行();
						if( this.ct進行[ i ].b終了値に達した )
						{
							this.ct進行[ i ].t停止();
						}
					}
				}
				for( int j = 0; j < 8; j++ )
				{
					if( !this.ct進行[ j ].b停止中 )
					{
						int x = this.stレーンサイズ[ j ].x;
						int w = this.stレーンサイズ[ j ].w;
						for( int k = 0; k < 3; k++ )
						{
							if( CDTXMania.ConfigIni.bReverse.Drums )
							{
								int y = ( k * 0x80 ) - ( ( this.ct進行[ j ].n現在の値 * 0x180 ) / 100 );
								for( int m = 0; m < w; m += 0x2a )
								{
									if( this.txFlush[ j + 8 ] != null )
									{
										this.txFlush[ j + 8 ].t2D描画( CDTXMania.app.Device, x + m, y, new Rectangle( ( k * 0x2a ) + 2, 0, ( ( w - m ) < 0x2a ) ? ( w - m ) : 0x2a, 0x80 ) );
									}
								}
							}
							else
							{
								int num8 = ( 0x60 + ( k * 0x80 ) ) + ( ( this.ct進行[ j ].n現在の値 * 0x180 ) / 100 );
								if( num8 < 480 )
								{
									for( int n = 0; n < w; n += 0x2a )
									{
										if( this.txFlush[ j ] != null )
										{
											this.txFlush[ j ].t2D描画( CDTXMania.app.Device, x + n, num8, new Rectangle( k * 0x2a, 0, ( ( w - n ) < 0x2a ) ? ( w - n ) : 0x2a, 0x80 ) );
										}
									}
								}
							}
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
		private struct STレーンサイズ
		{
			public int x;
			public int w;
		}

		private CCounter[] ct進行 = new CCounter[ 8 ];
		private readonly string[] strファイル名;
		private readonly STレーンサイズ[] stレーンサイズ;
		private CTexture[] txFlush = new CTexture[ 0x10 ];
		//-----------------
		#endregion
	}
}
