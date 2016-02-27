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

		// コンストラクタ

		public CAct演奏Drumsゲージ()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			// CAct演奏ゲージ共通.Init()に移動
			// this.dbゲージ値 = ( CDTXMania.Instance.ConfigIni.nRisky > 0 ) ? 1.0 : 0.66666666666666663;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct本体振動 = null;
			this.ct本体移動 = null;
			for (int i = 0; i < 24; i++)
			{
				this.st白い星[i].ct進行 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txゲージ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums gauge.png"));
				this.txゲージ白 = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums_gauge_white.png "));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txゲージ白);
				TextureFactory.tテクスチャの解放(ref this.txゲージ);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				#region [ 初めての進行描画 ]
				if (base.b初めての進行描画)
				{
					for (int k = 0; k < 0x18; k++)
					{
						this.st白い星[k].x = 2 + CDTXMania.Instance.Random.Next(4);
						this.st白い星[k].fScale = 0.2f + (CDTXMania.Instance.Random.Next(2) * 0.05f);
						this.st白い星[k].ct進行 = new CCounter(0, 0x160, 8 + CDTXMania.Instance.Random.Next(4), CDTXMania.Instance.Timer);
						this.st白い星[k].ct進行.n現在の値 = CDTXMania.Instance.Random.Next(0x160);
					}
					this.ct本体移動 = new CCounter(0, 0x1a, 20, CDTXMania.Instance.Timer);
					this.ct本体振動 = new CCounter(0, 360, 4, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				#endregion
				this.ct本体移動.t進行Loop();
				this.ct本体振動.t進行Loop();

				#region [ Risky残りMiss回数表示 ]
				if (this.bRisky && this.actLVLNFont != null)		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
				{
					CActLVLNFont.EFontColor efc = this.IsDanger(E楽器パート.DRUMS) ?
						CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
					actLVLNFont.t文字列描画((int)(12 * Scale.X), (int)(408 * Scale.Y), nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Right);
				}
				#endregion

				#region [ 緑orオレンジのゲージ表示 ]
				const double dbゲージ最大値 = 352.0 * Scale.Y;
				int n表示するゲージの高さ = (this.dbゲージ値 == 1.0) ?
					((int)(dbゲージ最大値 * this.dbゲージ値)) :
					((int)((dbゲージ最大値 * this.dbゲージ値) + (2.0 * Math.Sin(Math.PI * 2 * (((double)this.ct本体振動.n現在の値) / 360.0)))));

				if (n表示するゲージの高さ <= 0)
				{
					return 0;
				}
				if (this.txゲージ != null)
				{
					this.txゲージ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
					this.txゲージ.n透明度 = 0xff;
					this.txゲージ.b加算合成 = false;
				}
				int nCtゲージ内部上昇スクロール現在値 = (int)(this.ct本体移動.n現在の値 * Scale.Y);
				int n表示ゲージ最大値 = (int)(0x195 * Scale.Y);
				int ybar = (n表示ゲージ最大値 - n表示するゲージの高さ) - nCtゲージ内部上昇スクロール現在値;
				int height = n表示するゲージの高さ + nCtゲージ内部上昇スクロール現在値;
				while (height > 0)
				{
					Rectangle rect = (this.dbゲージ値 == 1.0) ?
						new Rectangle(48, 0, 48, 61) :
						new Rectangle(0, 0, 48, 61);
					#region [ clipping ]
					if (ybar < (n表示ゲージ最大値 - n表示するゲージの高さ))
					{
						int d = (n表示ゲージ最大値 - n表示するゲージの高さ) - ybar;
						rect.Y += d;
						rect.Height -= d;
						ybar += d;
					}
					if ((ybar + rect.Height) > n表示ゲージ最大値)
					{
						int num7 = (ybar + rect.Height) - n表示ゲージ最大値;
						rect.Height -= num7;
					}
					if (rect.Top >= rect.Bottom)
					{
						break;
					}
					#endregion
					if (this.txゲージ != null)
					{
						this.txゲージ.t2D描画(
							CDTXMania.Instance.Device,
							6 * Scale.X,
							ybar,
							rect
						);
					}
					height -= rect.Height;
					ybar += rect.Height;
				}
				#endregion
				#region [ 光彩 ]
				if (this.txゲージ白 != null)
				{
					this.txゲージ白.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
					this.txゲージ白.n透明度 = 180;
					this.txゲージ白.b加算合成 = true;
				}
				{
					Rectangle rect = new Rectangle(
						0,
						0,
						(int)(0x10 * Scale.X),
						(int)(0x40 * 4 * Scale.Y)
					);
					#region [ clipping1 ]
					int ylight = (n表示ゲージ最大値 - n表示するゲージの高さ) + (int)(0 * 0x40 * Scale.Y);
					if (ylight >= n表示ゲージ最大値)
					{
						//break;
					}
					#endregion
					else
					{
						#region [ Clipping2 ]
						if ((ylight + rect.Height) > n表示ゲージ最大値)
						{
							int d = (ylight + rect.Height) - n表示ゲージ最大値;
							rect.Height -= d;
						}
						#endregion
						if ((rect.Top < rect.Bottom) && (this.txゲージ白 != null))
						{
							this.txゲージ白.t2D描画(
								CDTXMania.Instance.Device,
								(int)(6 * Scale.X),
								(int)(ylight + 0.5f),
								rect
							);
						}
					}
				}
				#endregion
				#region [ ゲージ頂上の光源 ]
				if (this.txゲージ != null)
				{
					//this.txゲージ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
					this.txゲージ.n透明度 = 0xff;
					this.txゲージ.b加算合成 = false;
				}
				{
					Rectangle rect = new Rectangle(
						(int)(0x30 * Scale.X),
						0,
						(int)(0x10 * Scale.X),
						(int)(0x10 * Scale.Y)
					);
					int yゲージ頂上 = n表示ゲージ最大値 - n表示するゲージの高さ;
					if (yゲージ頂上 < n表示ゲージ最大値)
					{
						#region [ clipping ]
						if ((yゲージ頂上 + rect.Height) > n表示ゲージ最大値)
						{
							int d = (yゲージ頂上 + rect.Height) - n表示ゲージ最大値;
							rect.Height -= d;
						}
						#endregion
						if ((rect.Top < rect.Bottom) && (this.txゲージ != null))
						{
							this.txゲージ.t2D描画(
								CDTXMania.Instance.Device,
								(int)(6 * Scale.X),
								(int)(yゲージ頂上 + 0.5f),
								rect
							);
						}
					}
				}
				#endregion
				#region [ 泡 ]
				if (this.txゲージ != null)
				{
					this.txゲージ.b加算合成 = true;
				}
				for (int j = 0; j < 24; j++)
				{
					this.st白い星[j].ct進行.t進行Loop();
					int x = 6 + this.st白い星[j].x;
					int y泡 = (n表示ゲージ最大値 - n表示するゲージの高さ) + (int)((0x160 - this.st白い星[j].ct進行.n現在の値) * Scale.Y);
					int n透明度 = (this.st白い星[j].ct進行.n現在の値 < 0xb0) ? 0 : ((int)(255.0 * (((double)(this.st白い星[j].ct進行.n現在の値 - 0xb0)) / 176.0)));
					if ((n透明度 != 0) && (y泡 < (int)(0x191 * Scale.Y)))
					{
						Rectangle rect = new Rectangle(
							(int)(0 * Scale.X),
							(int)(0x20 * Scale.Y),
							(int)(0x20 * Scale.X),
							(int)(0x20 * Scale.Y)
						);
						if (this.txゲージ != null)
						{
							this.txゲージ.vc拡大縮小倍率 = new Vector3(this.st白い星[j].fScale, this.st白い星[j].fScale, 1f);
							this.txゲージ.n透明度 = n透明度;
							this.txゲージ.t2D描画(
								CDTXMania.Instance.Device,
								(int)(x * Scale.X),
								(int)(y泡 + 0.5f),
								rect
							);
						}
					}
				}
				#endregion
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout(LayoutKind.Sequential)]
		private struct ST白い星
		{
			public int x;
			public float fScale;
			public CCounter ct進行;
		}

		private CTextureAf txゲージ白;
		private const int STAR_MAX = 0x18;
		private ST白い星[] st白い星 = new ST白い星[0x18];
		//-----------------
		#endregion
	}
}
