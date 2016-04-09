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
	/// <summary>
	/// CAct演奏Drumsゲージ と CAct演奏Gutiarゲージ のbaseクラス。ダメージ計算やDanger/Failed判断もこのクラスで行う。
	/// </summary>
	internal class CAct演奏ゲージ共通 : CActivity
	{
		const double GAUGE_MAX = 1.0;
		const double GAUGE_INITIAL = 2.0 / 3;
		const double GAUGE_MIN = -0.1;
		const double GAUGE_ZERO = 0.0;
		const double GAUGE_DANGER = 0.3;

		private float[,] fDamageGaugeDelta = {
			// #23625 2011.1.10 ickw_284: tuned damage/recover factors
			// drums,   guitar,  bass
			{  0.004f,  0.006f,  0.006f  },
			{  0.002f,  0.003f,  0.003f  },
			{  0.000f,  0.000f,  0.000f  },
			{ -0.020f, -0.030f, -0.030f  },
			{ -0.050f, -0.050f, -0.050f  }
		};
		private float[] fDamageLevelFactor = {
			0.5f, 1.0f, 1.5f
		};

		private double db現在のゲージ値;
		private CCounter ct本体移動;
		private CCounter ct本体振動;
		private CTexture txゲージ;
		private CTextureAf txゲージ白;
		private ST白い星[] st白い星 = new ST白い星[24];

		private CActDigit actDigit;

		/// <summary>
		/// Riskyモードか否か
		/// </summary>
		public bool bRisky { get; private set; }

		/// <summary>
		/// Risky初期値
		/// </summary>
		public int nRiskyTimesInitial { get; private set; }

		/// <summary>
		/// 残Miss回数
		/// </summary>
		public int nRiskyTimes { get; private set; }

		/// <summary>
		/// 閉店状態になったかどうか
		/// </summary>
		/// <returns></returns>
		public bool IsFailed
		{
			get
			{
				if (bRisky)
				{
					return (nRiskyTimes <= 0);
				}
				return this.db現在のゲージ値 <= GAUGE_MIN;
			}
		}

		/// <summary>
		/// DANGERかどうか
		/// </summary>
		/// <returns></returns>
		public bool IsDanger
		{
			get
			{
				if (bRisky)
				{
					switch (nRiskyTimesInitial)
					{
						case 1:
							return false;
						case 2:
						case 3:
							return (nRiskyTimes <= 1);
						default:
							return (nRiskyTimes <= 2);
					}
				}
				return (this.db現在のゲージ値 <= GAUGE_DANGER);
			}
		}

		public CAct演奏ゲージ共通()
		{
			base.list子Activities.Add(actDigit = new CActDigit(Color.Red, Color.Black, 24));
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				this.ct本体振動 = null;
				this.ct本体移動 = null;
				for (int i = 0; i < 24; i++)
				{
					this.st白い星[i].ct進行 = null;
				}
				base.On非活性化();
			}
		}

		public override void OnUnmanagedリソースの作成()
		{
			if (b活性化してる)
			{
				this.txゲージ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums gauge.png"));
				this.txゲージ白 = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums_gauge_white.png "));
				base.OnUnmanagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txゲージ);
				TextureFactory.tテクスチャの解放(ref this.txゲージ白);
				base.OnUnmanagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (b活性化してる && CDTXMania.Instance.ConfigIni.eDark == EDark.Off)
			{
				#region [ 初めての進行描画 ]
				if (base.b初めての進行描画)
				{
					for (int k = 0; k < 24; k++)
					{
						this.st白い星[k].x = 6 + CDTXMania.Instance.Random.Next(12);
						this.st白い星[k].fScale = 0.2f + (CDTXMania.Instance.Random.Next(2) * 0.05f);
						this.st白い星[k].ct進行 = new CCounter(0, CDTXMania.Instance.Coordinates.GaugeMax, 8 + CDTXMania.Instance.Random.Next(4), CDTXMania.Instance.Timer);
						this.st白い星[k].ct進行.n現在の値 = CDTXMania.Instance.Random.Next(CDTXMania.Instance.Coordinates.GaugeMax);
					}
					this.ct本体移動 = new CCounter(0, 0x1a, 20, CDTXMania.Instance.Timer);
					this.ct本体振動 = new CCounter(0, 360, 4, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				#endregion
				this.ct本体移動.t進行Loop();
				this.ct本体振動.t進行Loop();


				#region [ 緑orオレンジのゲージ表示 ]
				double dbゲージ最大値 = CDTXMania.Instance.Coordinates.GaugeMax;
				int n表示するゲージの高さ = (this.db現在のゲージ値 == 1.0) ?
					((int)(dbゲージ最大値 * this.db現在のゲージ値)) :
					((int)((dbゲージ最大値 * this.db現在のゲージ値) + (2.0 * Math.Sin(Math.PI * 2 * (((double)this.ct本体振動.n現在の値) / 360.0)))));

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
				int n表示ゲージ最大値 = CDTXMania.Instance.Coordinates.Gauge.Y;
				int ybar = (n表示ゲージ最大値 - n表示するゲージの高さ) - nCtゲージ内部上昇スクロール現在値;
				int height = n表示するゲージの高さ + nCtゲージ内部上昇スクロール現在値;
				while (height > 0)
				{
					Rectangle rect = (this.db現在のゲージ値 == 1.0) ?
						CDTXMania.Instance.Coordinates.ImgGaugeOrange :
						CDTXMania.Instance.Coordinates.ImgGaugeNormal;
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
							CDTXMania.Instance.Coordinates.Gauge.X,
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
					Rectangle rect = CDTXMania.Instance.Coordinates.ImgGaugeLight;
					int ylight = (n表示ゲージ最大値 - n表示するゲージの高さ);
					if (ylight < n表示ゲージ最大値)
					{
						if ((ylight + rect.Height) > n表示ゲージ最大値)
						{
							int d = (ylight + rect.Height) - n表示ゲージ最大値;
							rect.Height -= d;
						}
						if ((rect.Top < rect.Bottom) && (this.txゲージ白 != null))
						{
							this.txゲージ白.t2D描画(
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Gauge.X,
								ylight,
								rect
							);
						}
					}
				}
				#endregion
				#region [ ゲージ頂上の光源 ]
				if (this.txゲージ != null)
				{
					this.txゲージ.n透明度 = 0xff;
					this.txゲージ.b加算合成 = false;
				}
				{
					Rectangle rect = CDTXMania.Instance.Coordinates.ImgGaugeTopLight;
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
								CDTXMania.Instance.Coordinates.Gauge.X,
								yゲージ頂上,
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
					int x = CDTXMania.Instance.Coordinates.Gauge.X + this.st白い星[j].x;
					int y泡 = (n表示ゲージ最大値 - n表示するゲージの高さ) + CDTXMania.Instance.Coordinates.GaugeMax - this.st白い星[j].ct進行.n現在の値;
					if (y泡 + CDTXMania.Instance.Coordinates.ImgGaugeStar.H / 2 < n表示ゲージ最大値)
					{
						int n透明度 = (this.st白い星[j].ct進行.n現在の値 < 176) ? 0 : ((int)(255.0 * (((double)(this.st白い星[j].ct進行.n現在の値 - 176)) / 176.0)));
						if ((n透明度 != 0) && (y泡 < CDTXMania.Instance.Coordinates.Gauge.Y))
						{
							Rectangle rect = CDTXMania.Instance.Coordinates.ImgGaugeStar;
							if (this.txゲージ != null)
							{
								this.txゲージ.vc拡大縮小倍率 = new Vector3(this.st白い星[j].fScale, this.st白い星[j].fScale, 1f);
								this.txゲージ.n透明度 = n透明度;
								this.txゲージ.t2D描画(
									CDTXMania.Instance.Device,
									x,
									y泡,
									rect
								);
							}
						}
					}
				}
				#endregion

				#region [ Risky残りMiss回数表示 ]
				if (this.bRisky)    // #23599 2011.7.30 yyagi Risky残りMiss回数表示
				{
					int w = actDigit.Measure(nRiskyTimes);
					actDigit.Draw(nRiskyTimes,
						CDTXMania.Instance.Coordinates.Gauge.X + (CDTXMania.Instance.Coordinates.ImgGaugeNormal.W / 2 - w / 2),
						CDTXMania.Instance.Coordinates.Gauge.Y - actDigit.MaximumHeight);
				}
				#endregion
			}
			return 0;
		}


		/// <summary>
		/// ゲージの初期化
		/// </summary>
		/// <param name="nRiskyTimes_Initial_">Riskyの初期値(0でRisky未使用)</param>
		public void Init(int nRiskyTimes_InitialVal)
		{
			nRiskyTimesInitial = nRiskyTimes_InitialVal;
			nRiskyTimes = nRiskyTimes_InitialVal;
			bRisky = (this.nRiskyTimes > 0);

			if (!bRisky)
			{
				this.db現在のゲージ値 = GAUGE_INITIAL;
			}
			else
			{
				this.db現在のゲージ値 = GAUGE_MAX;
			}
		}

		public void Damage(EPart part, EJudge e今回の判定)
		{
			double fDamage;

			// DAMAGELEVELTUNING
			// before applying #23625 modifications
			switch (e今回の判定)
			{
				case EJudge.Perfect:
				case EJudge.Great:
				case EJudge.Good:
					fDamage = bRisky ? 0 : fDamageGaugeDelta[(int)e今回の判定, (int)part];
					break;
				case EJudge.Poor:
				case EJudge.Miss:
					if (bRisky)
					{
						// Risky=1のときは1Miss即閉店なのでダメージ計算しない
						fDamage = (nRiskyTimes == 1) ? 0 : -GAUGE_MAX / (nRiskyTimesInitial - 1);

						if (nRiskyTimes >= 0)
						{
							// 念のため-1未満には減らないようにしておく
							nRiskyTimes--;
						}
					}
					else
					{
						fDamage = fDamageGaugeDelta[(int)e今回の判定, (int)part];
					}
					if (e今回の判定 == EJudge.Miss && !bRisky)
					{
						fDamage *= fDamageLevelFactor[(int)CDTXMania.Instance.ConfigIni.eDamageLevel.Value];
					}
					break;

				default:
					fDamage = 0.0f;
					break;
			}

			this.db現在のゲージ値 += fDamage;

			if (this.db現在のゲージ値 > GAUGE_MAX)
			{
				// RISKY時は決してゲージが増加しないので、上限チェックはしなくて良い
				this.db現在のゲージ値 = GAUGE_MAX;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct ST白い星
		{
			public int x;
			public float fScale;
			public CCounter ct進行;
		}
	}
}
