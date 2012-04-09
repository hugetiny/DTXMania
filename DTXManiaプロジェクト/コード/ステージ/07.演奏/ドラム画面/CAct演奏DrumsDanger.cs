using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏DrumsDanger : CAct演奏Danger共通
	{
		// コンストラクタ

		//public CAct演奏DrumsDanger()
		//{
		//    base.b活性化してない = true;
		//}


		// CActivity 実装

		//public override void On活性化()
		//{
		//    this.bDanger中 = false;
		//    this.ct移動用 = new CCounter();
		//    this.ct透明度用 = new CCounter();
		//    base.On活性化();
		//}
		//public override void On非活性化()
		//{
		//    this.ct移動用 = null;
		//    this.ct透明度用 = null;
		//    base.On非活性化();
		//}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txDANGER = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums danger.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txDANGER );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(bool)のほうを使用してください。" );
		}
		/// <summary>
		/// ドラム画面のDANGER描画
		/// </summary>
		/// <param name="bIsDangerDrums">DrumsのゲージがDangerかどうか(Guitar/Bassと共用のゲージ)</param>
		/// <param name="bIsDangerGuitar">Guitarのゲージ(未使用)</param>
		/// <param name="bIsDangerBass">Bassのゲージ(未使用)</param>
		/// <returns></returns>
		public override int t進行描画( bool bIsDangerDrums, bool bIsDangerGuitar, bool bIsDangerBass )
		{
			if( !base.b活性化してない )
			{
				if( !bIsDangerDrums )
				{
					this.bDanger中[(int)E楽器パート.DRUMS] = false;
					return 0;
				}
				if( !this.bDanger中[(int)E楽器パート.DRUMS] )
				{
					this.ct移動用 = new CCounter( 0, 0x7f, 7, CDTXMania.Timer );
					this.ct透明度用 = new CCounter( 0, 0x167, 4, CDTXMania.Timer );
				}
				this.bDanger中[(int)E楽器パート.DRUMS] = bIsDangerDrums;
				this.ct移動用.t進行Loop();
				this.ct透明度用.t進行Loop();
				if( !this.bDanger中[(int)E楽器パート.DRUMS] )
				{
					return 0;
				}
				int num = this.ct透明度用.n現在の値;
				if( this.txDANGER != null )
				{
					this.txDANGER.n透明度 = 60 + ( ( num < 180 ) ? num : ( 360 - num ) );
				}
				num = this.ct移動用.n現在の値;
				int num2 = CDTXMania.ConfigIni.bReverse.Drums ? ( 0x7f - num ) : num;
				for( int i = -1; i < 4; i++ )
				{
					if( this.txDANGER != null )
					{
						this.txDANGER.t2D描画( CDTXMania.app.Device, 0x26, ( i * 0x80 ) + num2, this.rc領域[ 0 ] );
						this.txDANGER.t2D描画( CDTXMania.app.Device, 0x26, ( ( i * 0x80 ) + num2 ) + 0x40, this.rc領域[ 1 ] );
						this.txDANGER.t2D描画( CDTXMania.app.Device, 0x12a, ( i * 0x80 ) + num2, this.rc領域[ 0 ] );
						this.txDANGER.t2D描画( CDTXMania.app.Device, 0x12a, ( ( i * 0x80 ) + num2 ) + 0x40, this.rc領域[ 1 ] );
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		//private bool bDanger中;
		//private CCounter ct移動用;
		//private CCounter ct透明度用;
//		private const int n右位置 = 0x12a;
//		private const int n左位置 = 0x26;
		private readonly Rectangle[] rc領域 = new Rectangle[] { new Rectangle( 0, 0, 0x20, 0x40 ), new Rectangle( 0x20, 0, 0x20, 0x40 ) };
		private CTexture txDANGER;
		//-----------------
		#endregion
	}
}
