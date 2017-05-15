using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SharpDX;
using FDK;

using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActResultSongBar : CActivity
	{
		private CCounter ct登場用;
		private Font ft曲名用フォント;
		private int n本体X;
		private int n本体Y;
		private CTextureAf txバー;
		private CTexture tx曲名;

		public CActResultSongBar()
		{
			base.b活性化してない = true;
		}

		public void tアニメを完了させる()
		{
			this.ct登場用.n現在の値 = this.ct登場用.n終了値;
		}

		public override void On活性化()
		{
			if (b活性化してない)
			{
				this.n本体X = 0;
				this.n本体Y = 0x18b;
				this.ft曲名用フォント = new Font("MS PGothic", 44f * Scale.Y, FontStyle.Bold, GraphicsUnit.Pixel);
				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (b活性化してる)
			{
				if (this.ft曲名用フォント != null)
				{
					this.ft曲名用フォント.Dispose();
					this.ft曲名用フォント = null;
				}
				if (this.ct登場用 != null)
				{
					this.ct登場用 = null;
				}
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.txバー = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenResult song bar.png"), false);
				try
				{
					Bitmap image = new Bitmap((int)(0x3a8 * Scale.X), (int)(0x36 * Scale.Y));
					Graphics graphics = Graphics.FromImage(image);
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					graphics.DrawString(CDTXMania.Instance.DTX.TITLE, this.ft曲名用フォント, Brushes.White, (float)8f * Scale.X, (float)0f);
					this.tx曲名 = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);
					this.tx曲名.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
					graphics.Dispose();
					image.Dispose();
				}
				catch (CTextureCreateFailedException)
				{
					Trace.TraceError("曲名テクスチャの生成に失敗しました。");
					this.tx曲名 = null;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txバー);
				TextureFactory.tテクスチャの解放(ref this.tx曲名);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (base.b初めての進行描画)
				{
					this.ct登場用 = new CCounter(0, 270, 4, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				this.ct登場用.t進行();
				int num = 0x1d4;
				int num2 = num - 0x40;
				if (this.ct登場用.b進行中)
				{
					if (this.ct登場用.n現在の値 <= 100)
					{
						double num3 = 1.0 - (((double)this.ct登場用.n現在の値) / 100.0);
						this.n本体X = -((int)(num * Math.Sin(Math.PI / 2 * num3)));
						this.n本体Y = 0x18b;
					}
					else if (this.ct登場用.n現在の値 <= 200)
					{
						double num4 = ((double)(this.ct登場用.n現在の値 - 100)) / 100.0;
						this.n本体X = -((int)((((double)num) / 6.0) * Math.Sin(Math.PI * num4)));
						this.n本体Y = 0x18b;
					}
					else if (this.ct登場用.n現在の値 <= 270)
					{
						double num5 = ((double)(this.ct登場用.n現在の値 - 200)) / 70.0;
						this.n本体X = -((int)((((double)num) / 18.0) * Math.Sin(Math.PI * num5)));
						this.n本体Y = 0x18b;
					}
				}
				else
				{
					this.n本体X = 0;
					this.n本体Y = 0x18b;
				}
				int num6 = this.n本体X;
				int y = this.n本体Y;
				int num8 = 0;
				while (num8 < num2)
				{
					Rectangle rectangle = new Rectangle(0, 0, 0x40, 0x40);
					if ((num8 + rectangle.Width) >= num2)
					{
						rectangle.Width -= (num8 + rectangle.Width) - num2;
					}
					if (this.txバー != null)
					{
						Rectangle rectangle1 = rectangle;
						rectangle1.X = (int)(rectangle1.X * Scale.X);
						rectangle1.Y = (int)(rectangle1.Y * Scale.Y);
						rectangle1.Width = (int)(rectangle1.Width * Scale.X);
						rectangle1.Height = (int)(rectangle1.Height * Scale.Y);
						this.txバー.t2D描画(
							CDTXMania.Instance.Device,
							(num6 + num8) * Scale.X,
							y * Scale.Y,
							rectangle1
						);
					}
					num8 += rectangle.Width;
				}
				if (this.txバー != null)
				{
					this.txバー.t2D描画(
						CDTXMania.Instance.Device,
						(num6 + num8) * Scale.X,
						y * Scale.Y,
						new Rectangle(
							(int)(0x40 * Scale.X),
							0,
							(int)(0x40 * Scale.X),
							(int)(0x40 * Scale.Y)
							)
						);
				}
				if (this.tx曲名 != null)
				{
					this.tx曲名.t2D描画(
						CDTXMania.Instance.Device,
						this.n本体X * Scale.X,
						(this.n本体Y + 20) * Scale.Y
					);
				}
				if (!this.ct登場用.b終了値に達した)
				{
					return 0;
				}
				return 1;
			}
			return 0;
		}
	}
}
