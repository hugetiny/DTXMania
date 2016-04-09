using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActSelectステータスパネル : CActivity
	{
		// メソッド

		public CActSelectステータスパネル()
		{
			base.b活性化してない = true;
		}
		public void t選択曲が変更された()
		{
			C曲リストノード c曲リストノード = CDTXMania.Instance.stage選曲.r現在選択中の曲;
			Cスコア cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;
			if ((c曲リストノード != null) && (cスコア != null))
			{
				this.n現在選択中の曲の難易度 = CDTXMania.Instance.stage選曲.n現在選択中の曲の難易度;
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					int nLevel = cスコア.譜面情報.レベル[i];
					if (nLevel < 0)
					{
						nLevel = 0;
					}
					if (nLevel > 99)
					{
						nLevel = 99;
					}
					this.n現在選択中の曲のレベル[i] = nLevel;
					this.n現在選択中の曲の最高ランク[i] = cスコア.譜面情報.最大ランク[i];
					this.b現在選択中の曲がフルコンボ[i] = cスコア.譜面情報.フルコンボ[i];
					this.db現在選択中の曲の最高スキル値[i] = cスコア.譜面情報.最大スキル[i];
				}
				for (int i = 0; i < 5; i++)
				{
					this.str難易度ラベル[i] = c曲リストノード.ar難易度ラベル[i];
				}
				if (this.r直前の曲 != c曲リストノード)
				{
					this.n難易度開始文字位置 = 0;
				}
				this.r直前の曲 = c曲リストノード;
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n本体X = (int)(3 * Scale.X);
			this.n本体Y = (int)(0x15d * Scale.Y);
			this.n現在選択中の曲の難易度 = 0;
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
			{
				this.n現在選択中の曲のレベル[i] = 0;
				this.n現在選択中の曲の最高ランク[i] = CScoreIni.ERANK.UNKNOWN;
				this.b現在選択中の曲がフルコンボ[i] = false;
				this.db現在選択中の曲の最高スキル値[i] = 0.0;
			}
			for (int j = 0; j < 5; j++)
			{
				this.str難易度ラベル[j] = "";
			}
			this.n難易度開始文字位置 = 0;
			this.r直前の曲 = null;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct登場アニメ用 = null;
			this.ct難易度スクロール用 = null;
			this.ct難易度矢印用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txパネル本体 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect status panel.png"), true);
				this.txレベル数字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect level numbers.png"), false);
				this.txスキルゲージ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill gauge.png"), false);
				this.txゲージ用数字他 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill number on gauge etc.png"), false);
				this.tx難易度用矢印 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect triangle arrow.png"), false);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txパネル本体);
				TextureFactory.tテクスチャの解放(ref this.txレベル数字);
				TextureFactory.tテクスチャの解放(ref this.txスキルゲージ);
				TextureFactory.tテクスチャの解放(ref this.txゲージ用数字他);
				TextureFactory.tテクスチャの解放(ref this.tx難易度用矢印);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				#region [ 初めての進行描画 ]
				//-----------------
				if (base.b初めての進行描画)
				{
					this.ct登場アニメ用 = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
					this.ct難易度スクロール用 = new CCounter(0, 20, 1, CDTXMania.Instance.Timer);
					this.ct難易度矢印用 = new CCounter(0, 5, 80, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				//-----------------
				#endregion

				// 進行

				this.ct登場アニメ用.t進行();

				this.ct難易度スクロール用.t進行();
				if (this.ct難易度スクロール用.b終了値に達した)
				{
					int num = this.n現在の難易度ラベルが完全表示されているかを調べてスクロール方向を返す();
					if (num < 0)
					{
						this.n難易度開始文字位置--;
					}
					else if (num > 0)
					{
						this.n難易度開始文字位置++;
					}
					this.ct難易度スクロール用.n現在の値 = 0;
				}

				this.ct難易度矢印用.t進行Loop();

				// 描画

				#region [ パネル本体の描画 ]
				//-----------------
				if (this.txパネル本体 != null)
				{
					if (this.ct登場アニメ用.b終了値に達した)
					{
						this.n本体X = (int)(3 * Scale.X);
						this.n本体Y = (int)(0x15d * Scale.Y);
					}
					else
					{
						double num2 = ((double)((int)(100 * Scale.X) - this.ct登場アニメ用.n現在の値)) / 100.0;
						double num3 = Math.Sin(Math.PI / 2 * num2);
						this.n本体X = 3 - ((int)((this.txパネル本体.sz画像サイズ.Width * num3) * num3));
						this.n本体Y = (int)(0x15d * Scale.Y);
					}
					this.txパネル本体.t2D描画(CDTXMania.Instance.Device, this.n本体X, this.n本体Y);
				}
				//-----------------
				#endregion

				#region [ 難易度文字列の描画 ]
				//-----------------

				#region [ chArray ← 難易度文字列を並べたもの、index ← その文字数 ]
				//-----------------
				char[] chArray = new char[0x100];
				C文字コンソール.Eフォント種別[] eフォント種別Array = new C文字コンソール.Eフォント種別[0x100];
				bool flag = false;
				bool flag2 = true;
				int index = 0;
				for (int i = 0; i < 5; i++)
				{
					if ((this.str難易度ラベル[i] != null) && (this.str難易度ラベル[i].Length > 0))
					{
						string str = this.str難易度ラベル[i];
						char[] chArray2 = new char[0x100];
						int num6 = 0;
						while ((num6 < 0xff) && (num6 < this.str難易度ラベル[i].Length))
						{
							chArray2[num6] = str[num6];
							num6++;
						}
						chArray2[num6] = '\0';
						if (!flag2)
						{
							if (index < 0xff)
							{
								chArray[index] = ' ';
								eフォント種別Array[index] = C文字コンソール.Eフォント種別.白;
								index++;
							}
							if (index < 0xff)
							{
								chArray[index] = ' ';
								eフォント種別Array[index] = C文字コンソール.Eフォント種別.白;
								index++;
							}
						}
						flag2 = false;
						num6 = 0;
						while ((chArray2[num6] != '\0') && (index < 0xff))
						{
							chArray[index] = chArray2[num6++];
							eフォント種別Array[index] = (this.n現在選択中の曲の難易度 == i) ? C文字コンソール.Eフォント種別.赤 : C文字コンソール.Eフォント種別.白;
							index++;
						}
						chArray[index] = '\0';
					}
				}
				//-----------------
				#endregion

				if (index > 0)
				{
					int x = this.n本体X + (int)(20 * Scale.X);
					int y = this.n本体Y + (int)(0x10 * Scale.Y);
					index = this.n難易度開始文字位置;
					flag = true;
					while (index < (this.n難易度開始文字位置 + 55))   // 0x24 -> 55
					{
						CDTXMania.Instance.act文字コンソール.tPrint(
							x,
							y + 5,
							eフォント種別Array[index],
							chArray[index].ToString()
						);
						x += 8 * 2;
						index++;
						if (chArray[index] == '\0')
						{
							flag = false;
							break;
						}
					}
					if (this.n難易度開始文字位置 > 0)
					{
						int num9 = (this.n本体X + (int)(12 * Scale.X)) - (int)(this.ct難易度矢印用.n現在の値 * Scale.X);
						int num10 = (this.n本体Y + (int)(0x10 * Scale.Y)) + (int)(5 * Scale.Y);
						if (this.tx難易度用矢印 != null)
						{
							this.tx難易度用矢印.t2D描画(CDTXMania.Instance.Device, num9, num10, new Rectangle(0, 0, (int)(8 * Scale.X), (int)(0x10 * Scale.Y)));
						}
					}
					if (flag)
					{
						int num11 = ((this.n本体X + (int)(20 * Scale.X)) + (int)(0x120 * Scale.X)) + (int)(this.ct難易度矢印用.n現在の値 * Scale.Y);
						int num12 = (this.n本体Y + (int)(0x10 * Scale.Y)) + (int)(5 * Scale.Y);
						if (this.tx難易度用矢印 != null)
						{
							this.tx難易度用矢印.t2D描画(CDTXMania.Instance.Device, num11, num12, new Rectangle((int)(8 * Scale.X), 0, (int)(8 * Scale.X), (int)(0x10 * Scale.Y)));
						}
					}
				}
				//-----------------
				#endregion

				Cスコア cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;

				#region [ 選択曲の Lv の描画 ]
				//-----------------
				if ((cスコア != null) && (this.txレベル数字 != null))
				{
					for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
					{
						Rectangle rect十の位;
						Rectangle rect一の位;
						int nDispPosX = this.n本体X + (int)(66 * Scale.X);
						int nDispPosY = this.n本体Y + (int)(50 * Scale.Y) + CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
						int nLevel = this.n現在選択中の曲のレベル[i];
						if (nLevel < 0)
						{
							nLevel = 0;
						}
						else if (nLevel > 99)
						{
							nLevel = 99;
						}
						// Lv25刻みで、白→オレンジ→黄色→赤、と色を変える
						// 
						int nRectOffsetX = ((nLevel / 25) < 2) ? (int)(64 * Scale.X) : 0;
						int nRectOffsetY = (((nLevel / 25) % 2) == 0) ? (int)(64 * Scale.Y) : 0;
						if (nLevel == 0)
						{
							rect十の位 = this.rc数字[11];    // "--"
							rect一の位 = this.rc数字[11];    // "-- "
						}
						else if (cスコア.譜面情報.レベルを非表示にする)
						{
							rect十の位 = this.rc数字[10];    // "?"
							rect一の位 = this.rc数字[10];    // "?"
						}
						else
						{
							rect十の位 = this.rc数字[nLevel / 10];
							rect一の位 = this.rc数字[nLevel % 10];
						}
						rect十の位.X += nRectOffsetX;
						rect十の位.Y += nRectOffsetY;
						rect一の位.X += nRectOffsetX;
						rect一の位.Y += nRectOffsetY;
						this.txレベル数字.t2D描画(CDTXMania.Instance.Device, nDispPosX, nDispPosY, rect十の位);
						this.txレベル数字.t2D描画(CDTXMania.Instance.Device, nDispPosX + (int)(13 * Scale.X), nDispPosY, rect一の位);
					}
				}
				//-----------------
				#endregion
				#region [ 選択曲の 最高スキル値ゲージ＋数値の描画 ]
				//-----------------
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					if (this.n現在選択中の曲のレベル[i] != 0)
					{
						double dMaxSkill = this.db現在選択中の曲の最高スキル値[i];
						if (dMaxSkill != 0.0)
						{
							int nDispPosX = this.n本体X + (int)(100 * Scale.X);
							int nDispPosY = this.n本体Y + (int)(53 * Scale.Y) + +CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
							this.txスキルゲージ.t2D描画(CDTXMania.Instance.Device, nDispPosX, nDispPosY,
														new Rectangle(0, 0, (int)(170.0 * Scale.X * dMaxSkill / 100.0), (int)(10 * Scale.Y)));
						}
						string sMaxSkillString = dMaxSkill.ToString("##0.00");
						int nMaxSkillStringWidth = 0;
						foreach (char ch in sMaxSkillString)
						{
							for (int j = 0; j < 12; j++)
							{
								if (ch == this.st数字[j].ch)
								{
									nMaxSkillStringWidth += this.st数字[j].rc.Width - 1;
									break;
								}
							}
						}
						int x = this.n本体X + (int)(182 * Scale.X) - nMaxSkillStringWidth / 2;
						int y = this.n本体Y + (int)(53 * Scale.Y) + +CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
						foreach (char ch in sMaxSkillString)
						{
							for (int j = 0; j < 12; j++)
							{
								if (ch == this.st数字[j].ch)
								{
									if (this.txゲージ用数字他 != null)
									{
										this.txゲージ用数字他.t2D描画(CDTXMania.Instance.Device, x, y, this.st数字[j].rc);
									}
									x += this.st数字[j].rc.Width - 1;
									break;
								}
							}
						}
					}
					else
					{
						int x = this.n本体X + (int)((182 - 20) * Scale.X);
						int y = this.n本体Y + (int)(53 * Scale.Y) + +CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
						if (this.txゲージ用数字他 != null)
						{
							this.txゲージ用数字他.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(0, (int)(22 * Scale.Y), (int)(42 * Scale.X), (int)(10 * Scale.Y)));
						}
					}
				}
				//-----------------
				#endregion
				#region [ 選択曲の 最高ランクの描画 ]
				//-----------------
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					CScoreIni.ERANK nMaxRank = this.n現在選択中の曲の最高ランク[i];
					if (nMaxRank != CScoreIni.ERANK.UNKNOWN)
					{
						int x = this.n本体X + (int)(278 * Scale.X);
						int y = this.n本体Y + (int)(55 * Scale.Y) + +CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
						if (this.txゲージ用数字他 != null)
						{
							this.txゲージ用数字他.t2D描画(CDTXMania.Instance.Device, x, y, this.rcランク[(int)nMaxRank]);
						}
					}
				}
				//-----------------
				#endregion
				#region [ 選択曲の FullCombo の 描画 ]
				//-----------------
				Rectangle rectFullCombo = new Rectangle((int)(30 * Scale.X), (int)(32 * Scale.Y), (int)(30 * Scale.X), (int)(16 * Scale.Y));
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					if (this.b現在選択中の曲がフルコンボ[i])
					{
						int x = this.n本体X + (int)(290 * Scale.X);
						int y = this.n本体Y + (int)(53 * Scale.Y) + +CDTXMania.Instance.Coordinates.OffsetGuitarBassSwap[(CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass ? 1 : 0)][i].Y;
						if (this.txゲージ用数字他 != null)
						{
							this.txゲージ用数字他.t2D描画(CDTXMania.Instance.Device, x, y, rectFullCombo);
						}
					}
				}
				//-----------------
				#endregion
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout(LayoutKind.Sequential)]
		private struct ST数字
		{
			public char ch;
			public Rectangle rc;
			public ST数字(char ch, Rectangle rc)
			{
				this.ch = ch;
				this.rc = rc;
			}
		}

		private STDGBSValue<bool> b現在選択中の曲がフルコンボ;
		private CCounter ct登場アニメ用;
		private CCounter ct難易度スクロール用;
		private CCounter ct難易度矢印用;
		private STDGBSValue<double> db現在選択中の曲の最高スキル値;
		private STDGBSValue<int> n現在選択中の曲のレベル;
		private STDGBSValue<CScoreIni.ERANK> n現在選択中の曲の最高ランク;
		private int n現在選択中の曲の難易度;
		private int n難易度開始文字位置;
		private const int n難易度表示可能文字数 = 0x24;
		private int n本体X;
		private int n本体Y;
		private readonly Rectangle[] rcランク = new Rectangle[] {
			new Rectangle( (int)(0 * Scale.X), (int)(0x20 * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(10 * Scale.X), (int)(0x20 * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(20 * Scale.X), (int)(0x20 * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(0 * Scale.X), (int)(0x2a * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(10 * Scale.X), (int)(0x2a * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(20 * Scale.X), (int)(0x2a * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) ),
			new Rectangle( (int)(0 * Scale.X), (int)(0x34 * Scale.Y), (int)(10 * Scale.X), (int)(10 * Scale.Y) )
		};
		private readonly Rectangle[] rc数字 = new Rectangle[] {
			new Rectangle( (int)(0 * Scale.X), (int)(0 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(15 * Scale.X), (int)(0 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(30 * Scale.X), (int)(0 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(0x2d * Scale.X), (int)(0 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(0 * Scale.X), (int)(0x13 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(15 * Scale.X), (int)(0x13 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(30 * Scale.X), (int)(0x13 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(0x2d * Scale.X), (int)(0x13 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(0 * Scale.X), (int)(0x26 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(15 * Scale.X),(int)(0x26 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(30 * Scale.X), (int)(0x26 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) ),
			new Rectangle( (int)(0x2d * Scale.X), (int)(0x26 * Scale.Y), (int)(15 * Scale.X), (int)(0x13 * Scale.Y) )
		};
		private C曲リストノード r直前の曲;
		private string[] str難易度ラベル = new string[] { "", "", "", "", "" };
		private readonly ST数字[] st数字 = new ST数字[] {
			new ST数字( '0', new Rectangle( (int)(0 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '1', new Rectangle( (int)(8 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '2', new Rectangle( (int)(0x10 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '3', new Rectangle( (int)(0x18 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '4', new Rectangle( (int)(0x20 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '5', new Rectangle( (int)(40 * Scale.X), (int)(0 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '6', new Rectangle( (int)(0 * Scale.X), (int)(11 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '7', new Rectangle( (int)(8 * Scale.X), (int)(11 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '8', new Rectangle( (int)(0x10 * Scale.X), (int)(11 * Scale.Y), (int)(8 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( '9', new Rectangle( (int)(0x18 * Scale.X), (int)(11 * Scale.Y), (int)(8 * Scale.X),(int)( 11 * Scale.Y) ) ),
			new ST数字( '.', new Rectangle( (int)(0x20 * Scale.X), (int)(11 * Scale.Y), (int)(4 * Scale.X), (int)(11 * Scale.Y) ) ),
			new ST数字( 'p', new Rectangle( (int)(0x24 * Scale.X), (int)(11 * Scale.Y), (int)(15 * Scale.X), (int)(11 * Scale.Y) ) )
		};
		private CTexture txゲージ用数字他;
		private CTexture txスキルゲージ;
		private CTexture txパネル本体;
		private CTexture txレベル数字;
		private CTexture tx難易度用矢印;

		private int n現在の難易度ラベルが完全表示されているかを調べてスクロール方向を返す()
		{
			int num = 0;
			int length = 0;
			for (int i = 0; i < 5; i++)
			{
				if ((this.str難易度ラベル[i] != null) && (this.str難易度ラベル[i].Length > 0))
				{
					length = this.str難易度ラベル[i].Length;
				}
				if (this.n現在選択中の曲の難易度 == i)
				{
					break;
				}
				if ((this.str難易度ラベル[i] != null) && (this.str難易度ラベル.Length > 0))
				{
					num += length + 2;
				}
			}
			if (num >= (this.n難易度開始文字位置 + 55))  // 0x24 -> 55
			{
				return 1;
			}
			if ((num + length) <= this.n難易度開始文字位置)
			{
				return -1;
			}
			if (((num + length) - 1) >= (this.n難易度開始文字位置 + 55)) // 0x24 -> 55
			{
				return 1;
			}
			if (num < this.n難易度開始文字位置)
			{
				return -1;
			}
			return 0;
		}
		//-----------------
		#endregion
	}
}
