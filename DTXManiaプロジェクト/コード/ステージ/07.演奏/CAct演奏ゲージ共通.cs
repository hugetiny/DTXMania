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
		// プロパティ
		public CActLVLNFont actLVLNFont { get; protected set; }

		// コンストラクタ
		public CAct演奏ゲージ共通()
		{
		}

		// CActivity 実装
		public override void On活性化()
		{
			actLVLNFont = new CActLVLNFont();
			actLVLNFont.On活性化();
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct本体振動 = null;
			this.ct本体移動 = null;
			if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				for (int i = 0; i < 24; i++)
				{
					this.st白い星[i].ct進行 = null;
				}
			}
			actLVLNFont.On非活性化();
			actLVLNFont = null;
			base.On非活性化();
		}

		const double GAUGE_MAX = 1.0;
		const double GAUGE_INITIAL = 2.0 / 3;
		const double GAUGE_MIN = -0.1;
		const double GAUGE_ZERO = 0.0;
		const double GAUGE_DANGER = 0.3;

		// Riskyモードか否か
		public bool bRisky
		{
			get;
			private set;
		}
		// Risky初期値
		public int nRiskyTimes_Initial
		{
			get;
			private set;
		}
		// 残Miss回数
		public int nRiskyTimes
		{
			get;
			private set;
		}
		// 閉店状態になったかどうか
		public bool IsFailed(E楽器パート part)
		{
			if (bRisky)
			{
				return (nRiskyTimes <= 0);
			}
			return this.db現在のゲージ値[(int)part] <= GAUGE_MIN;
		}
		// DANGERかどうか
		public bool IsDanger(E楽器パート part)
		{
			if (bRisky)
			{
				switch (nRiskyTimes_Initial)
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
			return (this.db現在のゲージ値[(int)part] <= GAUGE_DANGER);
		}

		// Drums専用
		public double dbゲージ値
		{
			get
			{
				return this.db現在のゲージ値.Drums;
			}
			set
			{
				this.db現在のゲージ値.Drums = value;
				if (this.db現在のゲージ値.Drums > GAUGE_MAX)
				{
					this.db現在のゲージ値.Drums = GAUGE_MAX;
				}
			}
		}

		/// <summary>
		/// ゲージの初期化
		/// </summary>
		/// <param name="nRiskyTimes_Initial_">Riskyの初期値(0でRisky未使用)</param>
		public void Init(int nRiskyTimes_InitialVal)		// ゲージ初期化
		{
			nRiskyTimes_Initial = nRiskyTimes_InitialVal;
			nRiskyTimes = nRiskyTimes_InitialVal;
			bRisky = (this.nRiskyTimes > 0);

			for (int i = 0; i < 3; i++)
			{
				if (!bRisky)
				{
					this.db現在のゲージ値[i] = GAUGE_INITIAL;
				}
				else if (nRiskyTimes_InitialVal == 1)
				{
					this.db現在のゲージ値[i] = GAUGE_ZERO;
				}
				else
				{
					this.db現在のゲージ値[i] = GAUGE_MAX;
				}
			}
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					int num;
					int num9;
					if (base.b初めての進行描画)
					{
						this.ct本体移動 = new CCounter(0, 0x1a, 20, CDTXMania.Instance.Timer);
						this.ct本体振動 = new CCounter(0, 360, 4, CDTXMania.Instance.Timer);
						base.b初めての進行描画 = false;
					}
					this.ct本体移動.t進行Loop();
					this.ct本体振動.t進行Loop();

					#region [ ギターのゲージ ]
					if (this.db現在のゲージ値.Guitar == 1.0)	// ギターのゲージ
					{
						num = (int)(128.0 * this.db現在のゲージ値.Guitar);
					}
					else
					{
						num = (int)((128.0 * this.db現在のゲージ値.Guitar) + (2.0 * Math.Sin(Math.PI * 2 * (((double)this.ct本体振動.n現在の値) / 360.0))));
					}
					if (num > 0)
					{
						Rectangle rectangle;
						int num2 = 0x1a - this.ct本体移動.n現在の値;
						int x = 0xb2 - num2;
						int num4 = num + num2;
						while (num4 > 0)
						{
							if (this.db現在のゲージ値.Guitar == 1.0)
							{
								rectangle = new Rectangle(0x1b, 0, 0x1b, 0x10);
							}
							else
							{
								rectangle = new Rectangle(0, 0, 0x1b, 0x10);
							}
							if (x < 0xb2)
							{
								int num5 = 0xb2 - x;
								rectangle.X += num5;
								rectangle.Width -= num5;
								x += num5;
							}
							if ((x + rectangle.Width) > (0xb2 + num))
							{
								int num6 = (x + rectangle.Width) - (0xb2 + num);
								rectangle.Width -= num6;
							}
							if (rectangle.Left >= rectangle.Right)
							{
								break;
							}
							if (this.txゲージ != null)
							{
								Rectangle rectangle1 = rectangle;
								rectangle1.X = (int)(rectangle1.X * Scale.X);
								rectangle1.Y = (int)(rectangle1.Y * Scale.Y);
								rectangle1.Width = (int)(rectangle1.Width * Scale.X);
								rectangle1.Height = (int)(rectangle1.Height * Scale.Y);

								this.txゲージ.b加算合成 = false;
								this.txゲージ.t2D描画(
									CDTXMania.Instance.Device,
									x * Scale.X,
									8 * Scale.Y,
									rectangle1
								);
							}
							num4 -= rectangle.Width;
							x += rectangle.Width;
						}
						rectangle = new Rectangle(0, 0x10, 0x40, 0x10);
						x = (0xb2 + num) - 0x40;
						if (x < 0xb2)
						{
							int num7 = 0xb2 - x;
							rectangle.X += num7;
							rectangle.Width -= num7;
							x += num7;
						}
						if ((x + rectangle.Width) > (0xb2 + num))
						{
							int num8 = (x + rectangle.Width) - (0xb2 + num);
							rectangle.Width -= num8;
						}
						if ((rectangle.Left < rectangle.Right) && (this.txゲージ != null))
						{
							this.txゲージ.b加算合成 = true;
							rectangle.X = (int)(rectangle.X * Scale.X);
							rectangle.Y = (int)(rectangle.Y * Scale.Y);
							rectangle.Width = (int)(rectangle.Width * Scale.X);
							rectangle.Height = (int)(rectangle.Height * Scale.Y);
							this.txゲージ.t2D描画(CDTXMania.Instance.Device,
								x * Scale.X,
								8 * Scale.Y,
								rectangle
							);
						}
						if (this.bRisky && this.actLVLNFont != null)		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
						{
							CActLVLNFont.EFontColor efc = this.IsDanger(E楽器パート.GUITAR) ?
								CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
							actLVLNFont.t文字列描画(
								(int)(196 * Scale.X),
								(int)(6 * Scale.Y),
								nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Left);
						}
					}
					#endregion

					#region [ ベースのゲージ ]
					if (this.db現在のゲージ値.Bass == 1.0)
					{
						num9 = (int)(128.0 * this.db現在のゲージ値.Bass);
					}
					else
					{
						num9 = (int)((128.0 * this.db現在のゲージ値.Bass) + (2.0 * Math.Sin(Math.PI * 2 * (((double)this.ct本体振動.n現在の値) / 360.0))));
					}
					if (num9 > 0)
					{
						Rectangle rectangle2;
						int num10 = this.ct本体移動.n現在の値;
						int num11 = (0x1cf - num9) - num10;
						int num12 = num9 + num10;
						while (num12 > 0)
						{
							if (this.db現在のゲージ値.Bass == 1.0)
							{
								rectangle2 = new Rectangle(10, 0x30, 0x1b, 0x10);
							}
							else
							{
								rectangle2 = new Rectangle(0x25, 0x30, 0x1b, 0x10);
							}
							if (num11 < (0x1cf - num9))
							{
								int num13 = (0x1cf - num9) - num11;
								rectangle2.X += num13;
								rectangle2.Width -= num13;
								num11 += num13;
							}
							if ((num11 + rectangle2.Width) > 0x1cf)
							{
								int num14 = (num11 + rectangle2.Width) - 0x1cf;
								rectangle2.Width -= num14;
							}
							if (rectangle2.Left >= rectangle2.Right)
							{
								break;
							}
							if (this.txゲージ != null)
							{
								Rectangle rectangle3 = rectangle2;
								rectangle3.X = (int)(rectangle3.X * Scale.X);
								rectangle3.Y = (int)(rectangle3.Y * Scale.Y);
								rectangle3.Width = (int)(rectangle3.Width * Scale.X);
								rectangle3.Height = (int)(rectangle3.Height * Scale.Y);
								this.txゲージ.b加算合成 = false;
								this.txゲージ.t2D描画(
									CDTXMania.Instance.Device,
									num11 * Scale.X,
									8 * Scale.Y,
									rectangle3
								);
							}
							num12 -= rectangle2.Width;
							num11 += rectangle2.Width;
						}
						rectangle2 = new Rectangle(0, 0x20, 0x40, 0x10);
						num11 = 0x1cf - num9;
						if ((num11 + rectangle2.Width) > 0x1cf)
						{
							int num15 = (num11 + rectangle2.Width) - 0x1cf;
							rectangle2.Width -= num15;
						}
						if ((rectangle2.Left < rectangle2.Right) && (this.txゲージ != null))
						{
							this.txゲージ.b加算合成 = true;
							rectangle2.X = (int)(rectangle2.X * Scale.X);
							rectangle2.Y = (int)(rectangle2.Y * Scale.Y);
							rectangle2.Width = (int)(rectangle2.Width * Scale.X);
							rectangle2.Height = (int)(rectangle2.Height * Scale.Y);
							this.txゲージ.t2D描画(
								CDTXMania.Instance.Device,
								num11 * Scale.X,
								8 * Scale.Y,
								rectangle2
							);
						}
						if (this.bRisky && this.actLVLNFont != null)		// #23599 2011.7.30 yyagi Risky残りMiss回数表示
						{
							CActLVLNFont.EFontColor efc = this.IsDanger(E楽器パート.GUITAR) ?
								CActLVLNFont.EFontColor.Red : CActLVLNFont.EFontColor.Yellow;
							actLVLNFont.t文字列描画(
								(int)(445 * Scale.X),
								(int)(6 * Scale.Y),
								nRiskyTimes.ToString(), efc, CActLVLNFont.EFontAlign.Right);
						}
					}
					#endregion
				}
				else
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
			}
			return 0;
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.txゲージ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums gauge.png"));
					this.txゲージ白 = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlayDrums_gauge_white.png "));
				}
				else
				{
					this.txゲージ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar gauge.png"));
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txゲージ);
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					TextureFactory.tテクスチャの解放(ref this.txゲージ白);
				}
				base.OnManagedリソースの解放();
			}
		}

		#region [ DAMAGE ]
		// DAMAGELEVELTUNING
		#region [ DAMAGELEVELTUNING ]
		public float[,] fDamageGaugeDelta = {
			// #23625 2011.1.10 ickw_284: tuned damage/recover factors
			// drums,   guitar,  bass
			{  0.004f,  0.006f,  0.006f  },
			{  0.002f,  0.003f,  0.003f  },
			{  0.000f,  0.000f,  0.000f  },
			{ -0.020f, -0.030f,	-0.030f  },
			{ -0.050f, -0.050f, -0.050f  }
		};
		public float[] fDamageLevelFactor = {
			0.5f, 1.0f, 1.5f
		};
		#endregion

		public void Damage(E楽器パート screenmode, E楽器パート part, E判定 e今回の判定)
		{
			double fDamage;

			// DAMAGELEVELTUNING
			// before applying #23625 modifications
			switch (e今回の判定)
			{
				case E判定.Perfect:
				case E判定.Great:
				case E判定.Good:
					fDamage = bRisky ? 0 : fDamageGaugeDelta[(int)e今回の判定, (int)part];
					break;
				case E判定.Poor:
				case E判定.Miss:
					if (bRisky)
					{
						fDamage = (nRiskyTimes == 1) ? 0 : -GAUGE_MAX / (nRiskyTimes_Initial - 1);	// Risky=1のときは1Miss即閉店なのでダメージ計算しない
						if (nRiskyTimes >= 0) nRiskyTimes--;		// 念のため-1未満には減らないようにしておく
					}
					else
					{
						fDamage = fDamageGaugeDelta[(int)e今回の判定, (int)part];
					}
					if (e今回の判定 == E判定.Miss && !bRisky)
					{
						fDamage *= fDamageLevelFactor[(int)CDTXMania.Instance.ConfigIni.eダメージレベル];
					}
					break;

				default:
					fDamage = 0.0f;
					break;
			}

			if (screenmode == E楽器パート.DRUMS)		// ドラム演奏画面なら、ギター/ベースのダメージも全部ドラムのゲージに集約する
			{
				part = E楽器パート.DRUMS;
				this.db現在のゲージ値[(int)part] += fDamage;
			}
			else
			{
				if (this.bRisky)						// ギター画面且つRISKYなら、ギターとベースのゲージをセットで減少
				{
					this.db現在のゲージ値[(int)E楽器パート.GUITAR] += fDamage;
					this.db現在のゲージ値[(int)E楽器パート.BASS] += fDamage;
				}
				else
				{
					this.db現在のゲージ値[(int)part] += fDamage;
				}
			}

			if (this.db現在のゲージ値[(int)part] > GAUGE_MAX)		// RISKY時は決してゲージが増加しないので、ギタレボモード時のギター/ベース両チェック(上限チェック)はしなくて良い
				this.db現在のゲージ値[(int)part] = GAUGE_MAX;
		}
		#endregion

		public STDGBVALUE<double> db現在のゲージ値;
		protected CCounter ct本体移動;
		protected CCounter ct本体振動;
		protected CTexture txゲージ;


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
	}
}
