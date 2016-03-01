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
		// プロパティ

		protected STSTATUS[] st状態 = new STSTATUS[12];
		[StructLayout(LayoutKind.Sequential)]
		protected struct STSTATUS
		{
			public CCounter ct進行;
			public E判定 judge;
			public float fX方向拡大率;
			public float fY方向拡大率;
			public int n相対X座標;
			public int n相対Y座標;
			public int n透明度;
			public int nLag;								// #25370 2011.2.1 yyagi
		}

		protected readonly ST判定文字列[] st判定文字列;
		[StructLayout(LayoutKind.Sequential)]
		protected struct ST判定文字列
		{
			public int n画像番号;
			public Rectangle rc;
		}

		protected readonly STlag数値[] stLag数値;			// #25370 2011.2.1 yyagi
		[StructLayout(LayoutKind.Sequential)]
		protected struct STlag数値
		{
			public Rectangle rc;
		}


		protected CTexture[] tx判定文字列 = new CTexture[3];
		protected CTexture txlag数値 = new CTexture();		// #25370 2011.2.1 yyagi

		public int nShowLagType							// #25370 2011.6.3 yyagi
		{
			get;
			set;
		}

		// コンストラクタ

		public CAct演奏判定文字列共通()
		{
			this.st判定文字列 = new ST判定文字列[7];
			Rectangle[] r = new Rectangle[] {
				new Rectangle( 0, 256 / 3 * 0, 256, 256 / 3 ),		// Perfect
				new Rectangle( 0, 256 / 3 * 1, 256, 256 / 3 ),		// Great
				new Rectangle( 0, 256 / 3 * 2, 256, 256 / 3 ),		// Good
				new Rectangle( 0, 256 / 3 * 0, 256, 256 / 3 ),		// Poor
				new Rectangle( 0, 256 / 3 * 1, 256, 256 / 3 ),		// Miss
				new Rectangle( 0, 256 / 3 * 2, 256, 256 / 3 ),		// Bad
				new Rectangle( 0, 256 / 3 * 0, 256, 256 / 3 )		// Auto

			};
			for (int i = 0; i < 7; i++)
			{
				this.st判定文字列[i] = new ST判定文字列();
				this.st判定文字列[i].n画像番号 = i / 3;
				this.st判定文字列[i].rc = r[i];
			}

			this.stLag数値 = new STlag数値[12 * 2];		// #25370 2011.2.1 yyagi

			for (int i = 0; i < 12; i++)
			{
				this.stLag数値[i].rc = new Rectangle(
												(int)((i % 4) * 15 * Scale.X),
												(int)((i / 4) * 19 * Scale.Y),
												(int)(15 * Scale.X),
												(int)(19 * Scale.Y)
												);	// plus numbers
				this.stLag数値[i + 12].rc = new Rectangle(
												(int)(((i % 4) * 15 + 64) * Scale.X),
												(int)(((i / 4) * 19 + 64) * Scale.Y),
												(int)(15 * Scale.X),
												(int)(19 * Scale.Y)
												);	// minus numbers
			}
			base.b活性化してない = true;
		}


		// メソッド
		public virtual void Start(int nLane, E判定 judge, int lag)
		{
			if ((nLane < 0) || (nLane > 11))
			{
				throw new IndexOutOfRangeException("有効範囲は 0～11 です。");
			}
			if (((nLane >= 8) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) != E判定文字表示位置.表示OFF)) && (((nLane != 10) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar) != E判定文字表示位置.表示OFF)) && ((nLane != 11) || (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass) != E判定文字表示位置.表示OFF))))
			{
				this.st状態[nLane].ct進行 = new CCounter(0, 300, 1, CDTXMania.Instance.Timer);
				this.st状態[nLane].judge = judge;
				this.st状態[nLane].fX方向拡大率 = 1f;
				this.st状態[nLane].fY方向拡大率 = 1f;
				this.st状態[nLane].n相対X座標 = 0;
				this.st状態[nLane].n相対Y座標 = 0;
				this.st状態[nLane].n透明度 = 0xff;
				this.st状態[nLane].nLag = lag;
			}
		}


		// CActivity 実装
		public override void On活性化()
		{
			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				this.stレーンサイズ = new STレーンサイズ[12];
				STレーンサイズ stレーンサイズ = new STレーンサイズ();
				int[,] sizeXW = new int[,] {
				{ 0x24, 0x24 },
				{ 0x4d, 30 },
				{ 0x6f, 30 },
				{ 0x92, 0x2a },
				{ 0xc1, 30 },
				{ 0xe3, 30 },
				{ 0x105, 30 },
				{ 0x127, 0x24 },
				{ 0, 0 },
				{ 0, 0 },
				{ 0x1a, 0x6f },		// 最後2つ(Gt, Bs)がドラムスと異なる
				{ 480, 0x6f }		// 
			};
				for (int i = 0; i < 12; i++)
				{
					this.stレーンサイズ[i] = new STレーンサイズ();
					this.stレーンサイズ[i].x = sizeXW[i, 0];
					this.stレーンサイズ[i].w = sizeXW[i, 1];
				}
			}
			else
			{
				this.stレーンサイズ = new STレーンサイズ[12]
			{
				new STレーンサイズ(  36 - 36, 36 ),
				new STレーンサイズ(  77 - 36, 30 ),
				new STレーンサイズ( 111 - 36, 30 ),
				new STレーンサイズ( 146 - 36, 42 ),
				new STレーンサイズ( 192 - 36, 30 ),
				new STレーンサイズ( 227 - 36, 30 ),
				new STレーンサイズ( 261 - 36, 30 ),
				new STレーンサイズ( 295 - 36, 36 ),
				new STレーンサイズ(   0 - 36, 0 ),
				new STレーンサイズ(   0 - 36, 0 ),
				new STレーンサイズ( 507 - 36, 80 ),
				new STレーンサイズ( 398 - 36, 80 )
			};
			}
			for (int i = 0; i < 12; i++)
			{
				this.st状態[i].ct進行 = new CCounter();
			}
			base.On活性化();
			this.nShowLagType = CDTXMania.Instance.ConfigIni.nShowLagType;
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
			if (!base.b活性化してない)
			{
				this.tx判定文字列[0] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay judge strings 1.png"));
				this.tx判定文字列[1] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay judge strings 2.png"));
				this.tx判定文字列[2] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay judge strings 3.png"));
				this.txlag数値 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect level numbers.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.tx判定文字列[0]);
				TextureFactory.tテクスチャの解放(ref this.tx判定文字列[1]);
				TextureFactory.tテクスチャの解放(ref this.tx判定文字列[2]);
				TextureFactory.tテクスチャの解放(ref this.txlag数値);
				base.OnManagedリソースの解放();
			}
		}


		public int t進行描画(C演奏判定ライン座標共通 演奏判定ライン座標)
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
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
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
								else if (num2 < 130)
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f;
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = ((num2 % 6) == 0) ? (CDTXMania.Instance.Random.Next(6) - 3) : st状態[i].n相対Y座標;
									st状態[i].n透明度 = 0xff;
								}
								else if (num2 >= 240)
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f - ((1f * (num2 - 240)) / 60f);
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
								else
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f;
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
							}
							else if (num2 < 50)
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = ((float)num2) / 50f;
								st状態[i].n相対X座標 = 0;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else if (num2 >= 200)
							{
								st状態[i].fX方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
								st状態[i].fY方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
								st状態[i].n相対X座標 = 0;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = 1f;
								st状態[i].n相対X座標 = 0;
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
							int index = st判定文字列[(int)st状態[j].judge].n画像番号;
							int baseX = 0;
							int baseY = 0;
							#region [ Drums 判定文字列 baseX/Y生成 ]
							if (j < 8)			// Drums
							{
								if (CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums == E判定文字表示位置.表示OFF)
								{
									continue;
								}
								baseX = this.stレーンサイズ[j].x;
								baseX = (int)(baseX * (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 1.0 : 0.75));
								baseX += (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 36 : 205);
								baseY = CDTXMania.Instance.ConfigIni.bReverse.Drums ?
									((((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) == E判定文字表示位置.レーン上) ? (240 + (this.n文字の縦表示位置[j] * 0x20)) : 50) :
									((((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) == E判定文字表示位置.レーン上) ? (180 + (this.n文字の縦表示位置[j] * 0x20)) : 450);
								baseY = (int)(baseY * Scale.Y);
							}
							#endregion
							#region [ Bass 判定文字列描画 baseX/Y生成 ]
							else if (j == 11)	// Bass
							{
								if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass) == E判定文字表示位置.表示OFF)
								{
									continue;
								}
								int yB;
								switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass)
								{
									case E判定文字表示位置.コンボ下:
										baseX = this.stレーンサイズ[j].x + 36;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX -= 331 + 1;
										}
										yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
										baseY = (
													CDTXMania.Instance.ConfigIni.bReverse.Bass ?
														yB + (int)((-124 + 0) * Scale.Y) :
														yB + (int)((+184 + 0) * Scale.Y)
												)
												+ (int)(this.n文字の縦表示位置[j] * 0x20 * Scale.Y);
										break;
									case E判定文字表示位置.レーン上:
										baseX = this.stレーンサイズ[j].x + 36;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX -= 331 + 1;
										}
										//baseY = ( CDTXMania.Instance.ConfigIni.bReverse.Bass ? 240 : 180 ) + ( this.n文字の縦表示位置[ j ] * 0x20 );
										yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
										baseY = (
													CDTXMania.Instance.ConfigIni.bReverse.Bass ?
														yB - (int)(134 * Scale.Y) :
														yB + (int)(75 * Scale.Y)
												)
												+ (int)(this.n文字の縦表示位置[j] * 0x20 * Scale.Y);
										break;
									case E判定文字表示位置.判定ライン上:
										baseX = this.stレーンサイズ[j].x + 36;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX -= 331 + 1;
										}
										yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ?
											yB + (int)(20 * Scale.Y) :
											yB - (int)(24 * Scale.Y);
										break;
								}
							}
							#endregion
							#region [ Guitar 判定文字列描画 baseX/Y生成 ]
							else if (j == 10)	// Guitar
							{
								if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar) == E判定文字表示位置.表示OFF)
								{
									continue;
								}
								int yG;
								switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar)
								{
									case E判定文字表示位置.コンボ下:
										baseX = (CDTXMania.Instance.DTX.bチップがある.Bass) ? this.stレーンサイズ[j].x + 36 : 0x198;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX = this.stレーンサイズ[j].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
											baseX -= 24 + 1;
										}
										yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
										baseY = (
													CDTXMania.Instance.ConfigIni.bReverse.Guitar ?
														yG + (int)((-124 + 0) * Scale.Y) :
														yG + (int)((+184 + 0) * Scale.Y)
												)
												+ (int)(this.n文字の縦表示位置[j] * 0x20 * Scale.Y);
										break;
									case E判定文字表示位置.レーン上:
										baseX = (CDTXMania.Instance.DTX.bチップがある.Bass) ? this.stレーンサイズ[j].x + 36 : 0x198;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX = this.stレーンサイズ[j].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
											baseX -= 24 + 1;
										}
										//baseY = ( CDTXMania.Instance.ConfigIni.bReverse.Guitar ? 240 : 180 ) + ( this.n文字の縦表示位置[ j ] * 0x20 );
										yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
										baseY = (
													CDTXMania.Instance.ConfigIni.bReverse.Guitar ?
														yG - (int)(134 * Scale.Y) :
														yG + (int)(75 * Scale.Y)
												)
												+ (int)(this.n文字の縦表示位置[j] * 0x20 * Scale.Y);
										break;
									case E判定文字表示位置.判定ライン上:
										baseX = (CDTXMania.Instance.DTX.bチップがある.Bass) ? this.stレーンサイズ[j].x + 36 : 0x198;
										if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
										{
											baseX = this.stレーンサイズ[j].x + 36;	// 判定表示がドラムレーンにかぶらないよう、ベース有りの時と同じ表示方法にする
											baseX -= 24 + 1;
										}
										yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
										baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ?
									yG + (int)(20 * Scale.Y) :
									yG - (int)(24 * Scale.Y);
										break;
								}
							}
							#endregion
							int xc = (int)((((baseX + st状態[j].n相対X座標) + (this.stレーンサイズ[j].w / 2))) * Scale.X);	// Xcenter座標
							int x = xc - ((int)(((256f / 2) * st状態[j].fX方向拡大率) * ((j < 10) ? 1.0 : 0.7)));
							int y = (int)(baseY + (st状態[j].n相対Y座標) * Scale.Y) - ((int)((((256f / 3) * st状態[j].fY方向拡大率) * ((j < 10) ? 1.0 : 0.7)) / 2.0));
							if (tx判定文字列[index] != null)
							{
								tx判定文字列[index].n透明度 = st状態[j].n透明度;
								tx判定文字列[index].vc拡大縮小倍率 = new Vector3((float)(st状態[j].fX方向拡大率 * ((j < 10) ? 1.0 : 0.7)), (float)(st状態[j].fY方向拡大率 * ((j < 10) ? 1.0 : 0.7)), 1f);
								tx判定文字列[index].t2D描画(
									CDTXMania.Instance.Device,
									x,
									y,
									st判定文字列[(int)st状態[j].judge].rc
								);

								#region [ #25370 2011.6.3 yyagi ShowLag support ]
								if (nShowLagType == (int)EShowLagType.ON ||
									 ((nShowLagType == (int)EShowLagType.GREAT_POOR) && (st状態[j].judge != E判定.Perfect)))
								{
									if (st状態[j].judge != E判定.Auto && txlag数値 != null)		// #25370 2011.2.1 yyagi
									{
										bool minus = false;
										int offsetX = 0;
										string strDispLag = st状態[j].nLag.ToString();
										if (st状態[j].nLag < 0)
										{
											minus = true;
										}
										//x = xc - strDispLag.Length * 15 / 2;
										x = xc - (int)((strDispLag.Length * 15 / 2) * Scale.X);
										for (int i = 0; i < strDispLag.Length; i++)
										{
											int p = (strDispLag[i] == '-') ? 11 : (int)(strDispLag[i] - '0');	//int.Parse(strDispLag[i]);
											p += minus ? 0 : 12;		// change color if it is minus value
											//base.txlag数値.t2D描画( CDTXMania.Instance.app.Device, x + offsetX, y + 34, base.stLag数値[ p ].rc );
											txlag数値.t2D描画(
												CDTXMania.Instance.Device,
												x + offsetX * Scale.X,
												y + 38 * Scale.Y,
												stLag数値[p].rc
											);
											offsetX += 12;	// 15 -> 12
										}
									}
								}
								#endregion
							}
							// Label_07FC: ;
						}
					}
				}
				else
				{
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
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
								else if (num2 < 130)
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f;
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = ((num2 % 6) == 0) ? (CDTXMania.Instance.Random.Next(6) - 3) : st状態[i].n相対Y座標;
									st状態[i].n透明度 = 0xff;
								}
								else if (num2 >= 240)
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f - ((1f * (num2 - 240)) / 60f);
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
								else
								{
									st状態[i].fX方向拡大率 = 1f;
									st状態[i].fY方向拡大率 = 1f;
									st状態[i].n相対X座標 = 0;
									st状態[i].n相対Y座標 = 0;
									st状態[i].n透明度 = 0xff;
								}
							}
							else if (num2 < 50)
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = ((float)num2) / 50f;
								st状態[i].n相対X座標 = 0;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else if (num2 >= 200)
							{
								st状態[i].fX方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
								st状態[i].fY方向拡大率 = 1f - (((float)(num2 - 200)) / 100f);
								st状態[i].n相対X座標 = 0;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
							else
							{
								st状態[i].fX方向拡大率 = 1f;
								st状態[i].fY方向拡大率 = 1f;
								st状態[i].n相対X座標 = 0;
								st状態[i].n相対Y座標 = 0;
								st状態[i].n透明度 = 0xff;
							}
						}
					}
					for (int j = 0; j < 12; j++)
					{
						if (!st状態[j].ct進行.b停止中)
						{
							int index = st判定文字列[(int)st状態[j].judge].n画像番号;
							int baseX = 0;
							int baseY = 0;
							if (j >= 8)
							{
								if (j == 11)		// Bass
								{
									if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass) == E判定文字表示位置.表示OFF)
									{
										continue;
									}
									int yB;
									switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Bass)
									{
										case E判定文字表示位置.コンボ下:
											baseX = 0x163;
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? 0x12b : 190;
											break;
										case E判定文字表示位置.レーン上:
											baseX = this.stレーンサイズ[j].x;
											yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? yB - 95 - 52 + 10 : yB + 95 + 52;
											//baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? 0x12b : 190;
											break;
										case E判定文字表示位置.判定ライン上:
											baseX = this.stレーンサイズ[j].x;
											yB = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, CDTXMania.Instance.ConfigIni.bReverse.Bass);
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Bass ? yB + 30 : yB - 20;
											break;
									}
								}
								else if (j == 10)	// Guitar
								{
									if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar) == E判定文字表示位置.表示OFF)
									{
										continue;
									}
									int yG;
									switch (CDTXMania.Instance.ConfigIni.判定文字表示位置.Guitar)
									{
										case E判定文字表示位置.コンボ下:
											baseX = 0xaf;
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? 0x12b : 190;
											break;
										case E判定文字表示位置.レーン上:
											baseX = this.stレーンサイズ[j].x;
											yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? yG - 95 - 52 + 10 : yG + 95 + 52;
											//baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? 0x12b : 190;
											break;
										case E判定文字表示位置.判定ライン上:
											baseX = this.stレーンサイズ[j].x;
											yG = 演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
											baseY = CDTXMania.Instance.ConfigIni.bReverse.Guitar ? yG + 30 : yG - 20;
											break;
									}
								}
								int xc = (int)((((baseX + st状態[j].n相対X座標) + (this.stレーンサイズ[j].w / 2))) * Scale.X);
								int x = xc - (int)(((256f / 2) * st状態[j].fX方向拡大率) * 0.8);
								int y = (int)((baseY + st状態[j].n相対Y座標) * Scale.Y) - ((int)((((256f / 3) * st状態[j].fY方向拡大率) * 0.8) / 2.0));

								if (tx判定文字列[index] != null)
								{
									tx判定文字列[index].n透明度 = st状態[j].n透明度;
									tx判定文字列[index].vc拡大縮小倍率 = new Vector3((float)(st状態[j].fX方向拡大率 * 0.8), (float)(st状態[j].fY方向拡大率 * 0.8), 1f);
									tx判定文字列[index].t2D描画(CDTXMania.Instance.Device, x, y, st判定文字列[(int)st状態[j].judge].rc);

									#region [ #25370 2011.6.3 yyagi ShowLag support ]
									if (nShowLagType == (int)EShowLagType.ON ||
										 ((nShowLagType == (int)EShowLagType.GREAT_POOR) && (st状態[j].judge != E判定.Perfect)))
									{
										if (st状態[j].judge != E判定.Auto && txlag数値 != null)		// #25370 2011.2.1 yyagi
										{
											bool minus = false;
											int offsetX = 0;
											string strDispLag = st状態[j].nLag.ToString();
											if (st状態[j].nLag < 0)
											{
												minus = true;
											}
											x = xc - (int)(strDispLag.Length * 15 / 2 * Scale.X);
											for (int i = 0; i < strDispLag.Length; i++)
											{
												int p = (strDispLag[i] == '-') ? 11 : (int)(strDispLag[i] - '0');	//int.Parse(strDispLag[i]);
												p += minus ? 0 : 12;		// change color if it is minus value
												txlag数値.t2D描画(
													CDTXMania.Instance.Device,
													x + offsetX * Scale.X,
													y + 35 * Scale.Y,
													stLag数値[p].rc
												);
												offsetX += 12;
											}
										}
									}
									#endregion
								}
							}
						}
					}
				}
			}
			return 0;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct STレーンサイズ
		{
			public int x;
			public int w;
			public STレーンサイズ(int x_, int w_)
			{
				x = x_;
				w = w_;
			}

		}

		private readonly int[] n文字の縦表示位置 = new int[] { 1, 2, 0, 1, 3, 2, 1, 0, 0, 0, 1, 1 };
		private STレーンサイズ[] stレーンサイズ;

	}


}
