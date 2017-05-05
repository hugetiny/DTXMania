using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;
using SharpDX;

using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CAct演奏判定文字列共通 : CActivity
	{
		private STLaneValue<STSTATUS> st状態 = new STLaneValue<STSTATUS>();

		private class STSTATUS
		{
			public CCounter ct進行;
			public EJudge judge;
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

		public CAct演奏判定文字列共通()
		{
			base.list子Activities.Add(actPositiveLag = new CActDigit(Color.White, Color.Gray, 25));
			base.list子Activities.Add(actNegativeLag = new CActDigit(Color.Red, Color.Black, 25));
			for (ELane i = ELane.DrumsLaneMin; i < ELane.Max; ++i)
			{
				st状態[i] = new STSTATUS();
			}
			base.b活性化してない = true;
		}

		/// <summary>
		/// アクティビティ開始メソッド。
		/// </summary>
		/// <param name="nLane">判定文字列を表示するレーン。ギターバスのときはRを渡します。</param>
		/// <param name="judge">表示する判定です。</param>
		/// <param name="lag">この判定に対する入力との差 (ms 単位) です。</param>
		public virtual void Start(ELane nLane, EJudge judge, int lag)
		{
			if
					(
					(nLane >= ELane.DrumsLaneMax || CDTXMania.Instance.ConfigIni.bDisplayJudge.Drums) &&
					(nLane != ELane.GtR || CDTXMania.Instance.ConfigIni.bDisplayJudge.Guitar) &&
					(nLane != ELane.BsR || CDTXMania.Instance.ConfigIni.bDisplayJudge.Bass)
					)
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
			if (b活性化してない)
			{
				for (ELane i = ELane.DrumsLaneMin; i < ELane.BassLaneMax; i++)
				{
					this.st状態[i].ct進行 = new CCounter();
				}
				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (b活性化してる)
			{
				for (ELane i = ELane.DrumsLaneMin; i < ELane.BassLaneMax; i++)
				{
					this.st状態[i].ct進行 = null;
				}
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				this.tex = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay judge strings 1.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref tex);
				base.OnManagedリソースの解放();
			}
		}


		public override int On進行描画()
		{
			if (b活性化してる)
			{
				#region [ 表示拡大率の設定 ]
				for (ELane i = ELane.DrumsLaneMin; i < ELane.BassLaneMax; ++i)
				{
					if (!st状態[i].ct進行.b停止中)
					{
						st状態[i].ct進行.t進行();
						if (st状態[i].ct進行.b終了値に達した)
						{
							st状態[i].ct進行.t停止();
						}
						int num2 = st状態[i].ct進行.n現在の値;
						if ((st状態[i].judge != EJudge.Miss) && (st状態[i].judge != EJudge.Bad))
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

				for (ELane i = ELane.DrumsLaneMin; i < ELane.BassLaneMax; ++i)
				{
					if (!st状態[i].ct進行.b停止中)
					{
						int baseX = 0;
						int baseY = 0;
						Rectangle rc = CDTXMania.Instance.Coordinates.ImgJudgeString.ApplyCounterY((int)st状態[i].judge, 0);

						#region [ Drums 判定文字列 baseX/Y生成 ]
						if (i < ELane.DrumsLaneMax)
						{
							if (!CDTXMania.Instance.ConfigIni.bDrums有効 ||
								!CDTXMania.Instance.DTX.bチップがある.Drums ||
								!CDTXMania.Instance.ConfigIni.bDisplayJudge.Drums)
							{
								continue;
							}

							baseX = CDTXMania.Instance.ConfigIni.GetLaneX(i) + CDTXMania.Instance.ConfigIni.GetLaneW(i) / 2;
							baseY = CDTXMania.Instance.ConfigIni.cdJudgeY[EPart.Drums];

							if (CDTXMania.Instance.ConfigIni.bReverse.Drums)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}

							tex.fZ軸中心回転 = (float)Math.PI / 4;
						}
						#endregion
						#region [ Guitar 判定文字列描画 baseX/Y生成 ]
						else if (i == ELane.GtR)  // Guitar
						{
							if (!CDTXMania.Instance.ConfigIni.bGuitar有効 ||
								!CDTXMania.Instance.DTX.bチップがある.Guitar ||
								!CDTXMania.Instance.ConfigIni.bDisplayJudge.Guitar)
							{
								continue;
							}

							baseX = CDTXMania.Instance.ConfigIni.cdInstX.Guitar[CDTXMania.Instance.ConfigIni.eActiveInst]
								+ CDTXMania.Instance.Coordinates.Instrument.Guitar.W / 2;
							baseY = CDTXMania.Instance.ConfigIni.cdJudgeY[EPart.Guitar];

							if (CDTXMania.Instance.ConfigIni.bReverse.Guitar)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}
							tex.fZ軸中心回転 = 0;
						}
						#endregion
						#region [ Bass 判定文字列描画 baseX/Y生成 ]
						else if (i == ELane.BsR)  // Bass
						{
							if (!CDTXMania.Instance.ConfigIni.bGuitar有効 ||
								!CDTXMania.Instance.DTX.bチップがある.Bass ||
								!CDTXMania.Instance.ConfigIni.bDisplayJudge.Bass)
							{
								continue;
							}

							baseX = CDTXMania.Instance.ConfigIni.cdInstX.Bass[CDTXMania.Instance.ConfigIni.eActiveInst]
								+ CDTXMania.Instance.Coordinates.Instrument.Bass.W / 2;
							baseY = CDTXMania.Instance.ConfigIni.cdJudgeY[EPart.Bass];

							if (CDTXMania.Instance.ConfigIni.bReverse.Bass)
							{
								baseY = SampleFramework.GameWindowSize.Height - baseY - rc.Height;
							}
							tex.fZ軸中心回転 = 0;
						}
						else
						{
							continue;
						}
						#endregion

						int xc = baseX;
						int x = (int)(xc - CDTXMania.Instance.Coordinates.ImgJudgeString.W / 2.0 * st状態[i].fX方向拡大率);
						int y = (int)(baseY + st状態[i].n相対Y座標 * Scale.Y - rc.Height / 2.0 * st状態[i].fY方向拡大率);
						if (tex != null)
						{
							tex.n透明度 = st状態[i].n透明度;
							tex.vc拡大縮小倍率.X = st状態[i].fX方向拡大率;
							tex.vc拡大縮小倍率.Y = st状態[i].fY方向拡大率;
							tex.t2D描画(CDTXMania.Instance.Device, x, y, rc);

							#region [ #25370 2011.6.3 yyagi ShowLag support ]
							if (CDTXMania.Instance.ConfigIni.nShowLagType == EShowLagType.On ||
									 ((CDTXMania.Instance.ConfigIni.nShowLagType == EShowLagType.UGreat) && (st状態[i].judge != EJudge.Perfect)))
							{
								// #25370 2011.2.1 yyagi
								if (st状態[i].judge != EJudge.Auto)
								{
									CActDigit useAct = actPositiveLag;
									int digit = st状態[i].nLag;
									if (digit < 0)
									{
										useAct = actNegativeLag;
										digit = -digit;
									}

									int w = useAct.Measure(digit);
									useAct.Draw(digit, xc - w / 2, y + CDTXMania.Instance.Coordinates.ImgJudgeString.H);
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
