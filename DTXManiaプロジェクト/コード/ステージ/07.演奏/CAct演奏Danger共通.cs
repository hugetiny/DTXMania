using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;
using SlimDX;

namespace DTXMania
{
	internal class CAct演奏Danger共通 : CActivity
	{
		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 3; i++)
			{
				this.bDanger中[i] = false;
			}
			this.ct移動用 = null;
			this.ct透明度用 = null;

			base.On活性化();
		}

		public override void On非活性化()
		{
			this.ct移動用 = null;
			this.ct透明度用 = null;
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.txDANGER = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums danger.png"), false);
				}
				else
				{
					this.txDANGER = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayGuitar danger.png"), false);
				}
				base.OnManagedリソースの作成();
			}
		}


		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
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
		public int t進行描画(STDGBVALUE<bool> isDanger)
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					if (!isDanger.Drums)
					{
						this.bDanger中[(int)E楽器パート.DRUMS] = false;
						return 0;
					}
					if (!this.bDanger中[(int)E楽器パート.DRUMS])
					{
						this.ct移動用 = new CCounter(0, 0x7f, 7, CDTXMania.Instance.Timer);
						this.ct透明度用 = new CCounter(0, 0x167, 4, CDTXMania.Instance.Timer);
					}
					this.bDanger中[(int)E楽器パート.DRUMS] = isDanger.Drums;
					this.ct移動用.t進行Loop();
					this.ct透明度用.t進行Loop();
					if (!this.bDanger中[(int)E楽器パート.DRUMS])
					{
						return 0;
					}
					int num = this.ct透明度用.n現在の値;
					if (this.txDANGER != null)
					{
						this.txDANGER.n透明度 = 60 + ((num < 180) ? num : (360 - num));
					}
					num = this.ct移動用.n現在の値;
					int num2 = CDTXMania.Instance.ConfigIni.bReverse.Drums ? (0x7f - num) : num;
					float[,] n基準X座標 = new float[,] { { 38, 298 }, { 211f, 405.5f } };
					for (int i = -1; i < 4; i++)
					{
						if (this.txDANGER != null)
						{
							float d = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1.0f : 0.75f;
							this.txDANGER.vc拡大縮小倍率 = new Vector3(d, d, d);
							this.txDANGER.t2D描画(CDTXMania.Instance.Device, n基準X座標[(CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 0 : 1), 0] * Scale.X, ((i * 0x80) + num2) * Scale.Y);
							this.txDANGER.t2D描画(CDTXMania.Instance.Device, n基準X座標[(CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 0 : 1), 1] * Scale.X, ((i * 0x80) + num2) * Scale.Y);
						}
					}
				}
				else
				{
					if (this.ct透明度用 == null)
					{
						this.ct透明度用 = new CCounter(0, n波長, 8, CDTXMania.Instance.Timer);
					}
					if (this.ct透明度用 != null)
					{
						this.ct透明度用.t進行Loop();
					}
					for (E楽器パート ePart = E楽器パート.GUITAR; ePart <= E楽器パート.BASS; ePart++)
					{
						if (isDanger[(int)ePart])
						{
							if (this.txDANGER != null)
							{
								int d = this.ct透明度用.n現在の値;
								this.txDANGER.n透明度 = n透明度MIN + ((d < n波長 / 2) ?
									(n透明度MAX - n透明度MIN) * d / (n波長 / 2) :
									(n透明度MAX - n透明度MIN) * (n波長 - d) / (n波長 / 2));		// 60-200
								this.txDANGER.t2D描画(
									CDTXMania.Instance.Device,
									nGaugeX[(int)ePart] * Scale.X,
									0
								);
							}
						}

					}
				}
			}
			return 0;
		}

		private CTextureAf txDANGER;

		protected bool[] bDanger中 = { false, false, false };
		protected CCounter ct移動用;
		protected CCounter ct透明度用;

		private const int n波長 = 40;
		private const int n透明度MAX = 180;
		private const int n透明度MIN = 20;
		private readonly int[] nGaugeX = { 0, 168, 328 };
	}
}
