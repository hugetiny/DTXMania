using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;
using SlimDX;

namespace DTXMania
{
	internal class CAct演奏判定文字列共通 : CActivity
	{
		protected STSTATUS[] st状態 = new STSTATUS[12];

		[StructLayout(LayoutKind.Sequential)]
		protected struct STSTATUS
		{
			public CCounter ct進行;
			public E判定 judge;
			public float fX方向拡大率;
			public float fY方向拡大率;
			public int n相対Y座標;
			public int n透明度;

			/// <summary>
			/// #25370 2011.2.1 yyagi
			/// </summary>
			public int nLag;
		}

		private CTexture tex;

		/// <summary>
		/// #25370 2011.2.1 yyagi
		/// </summary>
		/// 
		private CActDigit actPositiveLag;
		private CActDigit actNegativeLag;

		/// <summary>
		/// #25370 2011.6.3 yyagi
		/// </summary>
		public int nShowLagType
		{
			get;
			set;
		}

		public CAct演奏判定文字列共通()
		{
			base.list子Activities.Add(actPositiveLag = new CActDigit(Color.White, Color.Gray, 25));
			base.list子Activities.Add(actNegativeLag = new CActDigit(Color.Red, Color.Black, 25));
			base.b活性化してない = true;
		}

		public virtual void Start(int nLane, E判定 judge, int lag)
		{
			if (nLane < 0 || nLane > 11)
			{
				throw new IndexOutOfRangeException("有効範囲は 0～11 です。");
			}

			if (((nLane >= 8) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) != E判定文字表示位置.表示OFF)) &&
				(((nLane != 10) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar) != E判定文字表示位置.表示OFF)) &&
				((nLane != 11) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass) != E判定文字表示位置.表示OFF))))
			{
				this.st状態[nLane].ct進行 = new CCounter(0, 300, 1, CDTXMania.Instance.Timer);
				this.st状態[nLane].judge = judge;
				this.st状態[nLane].fX方向拡大率 = 1f;
				this.st状態[nLane].fY方向拡大率 = 1f;
				this.st状態[nLane].n相対Y座標 = 0;
				this.st状態[nLane].n透明度 = 0xff;
				this.st状態[nLane].nLag = lag;
			}
		}


		public override void On活性化()
		{
			for (int i = 0; i < 12; i++)
			{
				this.st状態[i].ct進行 = new CCounter();
			}
			this.nShowLagType = CDTXMania.Instance.ConfigIni.nShowLagType;
			base.On活性化();
		}

		public override void On非活性化()
		{
			for (int i = 0; i < 12; i++)
			{
				this.st状態[i].ct進行 = null;
			}
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.tex = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay judge strings 1.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref tex);
				base.OnManagedリソースの解放();
			}
		}


		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				#region [ 表示拡大率の設定 ]
				for (int i = 0; i < 12; i++)
				{
					if (!st状態[i].ct進行.b停止中)
					{
						st状態[i].ct進行.t進行();
						if (st状態[i].ct進行.b終了値に達した)
						{
							st状態[i].ct進行.t停止();
						}
						int num2 = st状態[i].ct進行.n現在の値;
						if ((st状態[i].judge != E判定.Miss) && (st状態[i].judge != E判定.Bad))
						{
							if (num2 < 50)
							{
								st状態[i].fX方向拡大率 = 1f + (1f * (1f - (((float)num2) / 50f)));
								st状態[i].fY方向拡大率 = ((float)num2) / 50f;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else if (num2 < 130)
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = 1f;
								st状態[i].n相対Y座標 = ((num2 % 6) == 0) ? (CDTXMania.Instance.Random.Next(6) - 3) : st状態[i].n相対Y座標;
								st状態[i].n透明度 = 0xff;
							}
							else if (num2 >= 240)
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = 1f - ((1f * (num2 - 240)) / 60f);
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = 1f;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
						}
						else if (num2 < 50)
						{
							st状態[i].fX方向拡大率 = 1f;
							st状態[i].fY方向拡大率 = ((float)num2) / 50f;
							st状態[i].n相対Y座標 = 0;
							st状態[i].n透明度 = 0xff;
						}
						else if (num2 >= 200)
						{
							st状態[i].fX方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
							st状態[i].fY方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
							st状態[i].n相対Y座標 = 0;
							st状態[i].n透明度 = 0xff;
						}
						else
						{
							st状態[i].fX方向拡大率 = 1f;
							st状態[i].fY方向拡大率 = 1f;
							st状態[i].n相対Y座標 = 0;
							st状態[i].n透明度 = 0xff;
						}
					}
				}
				#endregion

				for (int j = 0; j < 12; j++)
				{
					if (!st状態[j].ct進行.b停止中)
					{
						int baseX = 0;
						int baseY = 0;
						Rectangle rc = CDTXMania.Instance.Coordinates.ImgJudgeString.ApplyCounterY((int)st状態[j].judge, 0);

						// int w;
						#region [ Drums 判定文字列 baseX/Y生成 ]
						if (j < 8)			// Drums
						{
							if (
								!CDTXMania.Instance.ConfigIni.bDrums有効 ||
								CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums == E判定文字表示位置.表示OFF)
							{
								continue;
							}

							if (j == 0)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.LCY.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.LCY.Y;
							}
							else if (j == 1)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.HH.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.HH.Y;
							}
							else if (j == 2)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.SD.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.SD.Y;
							}
							else if (j == 3)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.BD.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.BD.Y;
							}
							else if (j == 4)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.HT.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.HT.Y;
							}
							else if (j == 5)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.LT.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.LT.Y;
							}
							else if (j == 6)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.FT.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.FT.Y;
							}
							else if (j == 7)
							{
								baseX = CDTXMania.Instance.Coordinates.Judge.CY.X;
								baseY = CDTXMania.Instance.Coordinates.Judge.CY.Y;
							}
							else
							{
								continue;
							}

							if (CDTXMania.Instance.ConfigIni.bReverse.Drums)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}
						}
						#endregion
						#region [ Guitar 判定文字列描画 baseX/Y生成 ]
						else if (j == 10)	// Guitar
						{
							if (
								!CDTXMania.Instance.ConfigIni.bGuitar有効 ||
								CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar == E判定文字表示位置.表示OFF)
							{
								continue;
							}

							baseX = CDTXMania.Instance.Coordinates.Judge.Gt.X;
							baseY = CDTXMania.Instance.Coordinates.Judge.Gt.Y;

							if (CDTXMania.Instance.ConfigIni.bReverse.Guitar)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}
						}
						#endregion
						#region [ Bass 判定文字列描画 baseX/Y生成 ]
						else if (j == 11)	// Bass
						{
							if(
 								!CDTXMania.Instance.ConfigIni.bGuitar有効 ||
								CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass == E判定文字表示位置.表示OFF)
							{
								continue;
							}

							baseX = CDTXMania.Instance.Coordinates.Judge.Bs.X;
							baseY = CDTXMania.Instance.Coordinates.Judge.Bs.Y;

							if (CDTXMania.Instance.ConfigIni.bReverse.Bass)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}
						}
						#endregion

						int xc = baseX;
						int x = (int)(xc - CDTXMania.Instance.Coordinates.ImgJudgeString.W / 2.0 * st状態[j].fX方向拡大率);
						int y = (int)(baseY + st状態[j].n相対Y座標 * Scale.Y - rc.Height / 2.0 * st状態[j].fY方向拡大率);
						if (tex != null)
						{
							tex.n透明度 = st状態[j].n透明度;
							tex.vc拡大縮小倍率.X = st状態[j].fX方向拡大率;
							tex.vc拡大縮小倍率.Y = st状態[j].fY方向拡大率;
							tex.t2D描画(CDTXMania.Instance.Device, x, y, rc);

							#region [ #25370 2011.6.3 yyagi ShowLag support ]
							if (nShowLagType == (int)EShowLagType.ON ||
								 ((nShowLagType == (int)EShowLagType.GREAT_POOR) && (st状態[j].judge != E判定.Perfect)))
							{
								// #25370 2011.2.1 yyagi
								if (st状態[j].judge != E判定.Auto)
								{
									CActDigit useAct = actPositiveLag;
									int digit = st状態[j].nLag;
									if (digit < 0)
									{
										useAct = actNegativeLag;
										digit = -digit;
									}

									int w = useAct.Measure(digit);
									useAct.Draw(digit, xc-w/2, y + CDTXMania.Instance.Coordinates.ImgJudgeString.H);
								}
							}
							#endregion
						}
					}
				}
			}
			return 0;
		}
	}
}
