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
		private CTextureAf txDANGER;
		private bool[] bDanger中 = { false, false, false };
		private CCounter ct移動用;
		private CCounter ct透明度用;
		private const int n波長 = 40;
		private const int n透明度MAX = 180;
		private const int n透明度MIN = 20;
		
		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				for (int i = 0; i < 3; i++)
				{
					this.bDanger中[i] = false;
				}
				this.ct移動用 = null;
				this.ct透明度用 = null;

				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				this.ct移動用 = null;
				this.ct透明度用 = null;
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.txDANGER = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums danger.png"), false);
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
		public int t進行描画(bool isDanger)
		{
			if (base.b活性化してる)
			{
				if (!isDanger)
				{
					this.bDanger中[(int)E楽器パート.DRUMS] = false;
					return 0;
				}
				if (!this.bDanger中[(int)E楽器パート.DRUMS])
				{
					this.ct移動用 = new CCounter(0, 0x7f, 7, CDTXMania.Instance.Timer);
					this.ct透明度用 = new CCounter(0, 0x167, 4, CDTXMania.Instance.Timer);
				}
				this.bDanger中[(int)E楽器パート.DRUMS] = isDanger;
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
