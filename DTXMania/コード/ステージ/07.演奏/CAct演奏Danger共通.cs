using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;
using SharpDX;

using Color = System.Drawing.Color;

namespace DTXMania
{
	internal class CAct演奏Danger共通 : CActivity
	{
		private CTextureAf txDANGER;
		private CTexture txRedScreen;
		private bool bDanger中;
		// = { false, false, false };
		private CCounter ct移動用;
		private CCounter ct透明度用;
		private const int n波長 = 40;
		private const int n透明度MAX = 180;
		private const int n透明度MIN = 20;

		public override void On活性化()
		{
			if (b活性化してない)
			{
				bDanger中 = false;
				ct移動用 = null;
				ct透明度用 = null;
				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (b活性化してる)
			{
				this.ct移動用 = null;
				this.ct透明度用 = null;
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				this.txDANGER = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums danger.png"), false);

				using ( Bitmap bmp = new Bitmap( SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height ) )
				{
					using ( var g = Graphics.FromImage( bmp ) )
					{
						using ( var brush = new SolidBrush( Color.Red ) )
						{
							g.FillRectangle( brush, 0, 0, bmp.Width, bmp.Height );
						}
					}
					this.txRedScreen = TextureFactory.tテクスチャの生成( bmp );
				}
 

				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txRedScreen);
				TextureFactory.tテクスチャの解放(ref this.txDANGER);
				base.OnManagedリソースの解放();
			}
		}

		/// <summary>
		/// DANGER描画
		/// </summary>
		/// <param name="bIsDangerDrums">DrumsがDangerならtrue</param>
		/// <param name="bIsDamgerGuitar">GuitarがDangerならtrue</param>
		/// <param name="bIsDangerBass">BassがDangerならtrue</param>
		/// <returns></returns>
		public int t進行描画(bool isDanger)
		{
			if (b活性化してる)
			{
				if (!isDanger)
				{
					bDanger中 = false;
					return 0;
				}
				if (!bDanger中)
				{
					this.ct移動用 = new CCounter(0, 0x7f, 7, CDTXMania.Instance.Timer);
					this.ct透明度用 = new CCounter(0, 0x167, 4, CDTXMania.Instance.Timer);
				}
				bDanger中 = isDanger;
				this.ct移動用.t進行Loop();
				this.ct透明度用.t進行Loop();
				if (!bDanger中)
				{
					return 0;
				}
				int num = this.ct透明度用.n現在の値;
				if ( txRedScreen != null )
				{
					this.txRedScreen.n透明度 = ( ( ( num < 180 ) ? num : ( 360 - num ) ) * 256 / 180) / 3 ;
					this.txRedScreen.t2D描画( CDTXMania.Instance.Device, 0, 0 );
				}
				if ( this.txDANGER != null )
				{
					this.txDANGER.n透明度 = 60 + ((num < 180) ? num : (360 - num));
				}
				num = this.ct移動用.n現在の値;
				int num2 = CDTXMania.Instance.ConfigIni.bReverse.Drums ? (0x7f - num) : num;
				for (int i = -1; i < 4; i++)
				{
					if (this.txDANGER != null)
					{
						//float d = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1.0f : 0.75f;
						//this.txDANGER.vc拡大縮小倍率 = new Vector3(d, d, d);
						int y = (int)(((i * 0x80) + num2) * Scale.Y);
						this.txDANGER.t2D描画(CDTXMania.Instance.Device,
							CDTXMania.Instance.Coordinates.Danger.Min, y);
						this.txDANGER.t2D描画(CDTXMania.Instance.Device,
							CDTXMania.Instance.Coordinates.Danger.Max, y);
					}
				}
			}
			return 0;
		}

	}
}
