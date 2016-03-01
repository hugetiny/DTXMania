using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏スコア共通 : CActivity
	{
		// プロパティ
		protected STDGBVALUE<long> nスコアの増分;
		protected STDGBVALUE<double> n現在の本当のスコア;
		protected STDGBVALUE<long> n現在表示中のスコア;
		protected long n進行用タイマ;
		protected CTexture txScore;
		private readonly Point[] ptSCORE = new Point[] { new Point(0x1f, 0x1a9), new Point(0x1e9, 0x1a9) };

		// コンストラクタ
		public CAct演奏スコア共通()
		{
			base.b活性化してない = true;
		}

		// メソッド
		public double Get(E楽器パート part)
		{
			return this.n現在の本当のスコア[(int)part];
		}

		public void Set(E楽器パート part, double nScore)
		{
			int nPart = (int)part;
			if (this.n現在の本当のスコア[nPart] != nScore)
			{
				this.n現在の本当のスコア[nPart] = nScore;
				this.nスコアの増分[nPart] = (long)(((double)(this.n現在の本当のスコア[nPart] - this.n現在表示中のスコア[nPart])) / 20.0);
				if (this.nスコアの増分[nPart] < 1L)
				{
					this.nスコアの増分[nPart] = 1L;
				}
			}
		}
		
		/// <summary>
		/// 点数を加える(各種AUTO補正つき)
		/// </summary>
		/// <param name="part"></param>
		/// <param name="bAutoPlay"></param>
		/// <param name="delta"></param>
		public void Add(E楽器パート part, STAUTOPLAY bAutoPlay, long delta)
		{
			double rev = 1.0;
			switch (part)
			{
				#region [ Unknown ]
				case E楽器パート.UNKNOWN:
					throw new ArgumentException();
				#endregion
				#region [ Drums ]
				case E楽器パート.DRUMS:
					if (!CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである)
					{
						#region [ Auto BD ]
						if (bAutoPlay.BD == true)
						{
							rev /= 2;
						}
						#endregion
					}
					break;
				#endregion
				#region [ Gutiar ]
				case E楽器パート.GUITAR:
					if (!CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである)
					{
						#region [ Auto Wailing ]
						if (bAutoPlay.GtW)
						{
							rev /= 2;
						}
						#endregion
						#region [ Auto Pick ]
						if (bAutoPlay.GtPick)
						{
							rev /= 3;
						}
						#endregion
						#region [ Auto Neck ]
						if (bAutoPlay.GtR || bAutoPlay.GtG || bAutoPlay.GtB)
						{
							rev /= 4;
						}
						#endregion
					}
					break;
				#endregion
				#region [ Bass ]
				case E楽器パート.BASS:
					if (!CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである)
					{
						#region [ Auto Wailing ]
						if (bAutoPlay.BsW)
						{
							rev /= 2;
						}
						#endregion
						#region [ Auto Pick ]
						if (bAutoPlay.BsPick)
						{
							rev /= 3;
						}
						#endregion
						#region [ Auto Neck ]
						if (bAutoPlay.BsR || bAutoPlay.BsG || bAutoPlay.BsB)
						{
							rev /= 4;
						}
						#endregion
					}
					break;
				#endregion
			}
			this.Set(part, this.Get(part) + delta * rev);
		}


		// CActivity 実装
		public override void On活性化()
		{
			this.n進行用タイマ = -1;
			for (int i = 0; i < 3; i++)
			{
				this.n現在表示中のスコア[i] = 0L;
				this.n現在の本当のスコア[i] = 0L;
				this.nスコアの増分[i] = 0L;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txScore = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay score numbers.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txScore);
				base.OnManagedリソースの解放();
			}
		}

		// CActivity 実装（共通クラスからの差分のみ）
		public override unsafe int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					if (base.b初めての進行描画)
					{
						n進行用タイマ = CDTXMania.Instance.Timer.n現在時刻;
						b初めての進行描画 = false;
					}
					long num = CDTXMania.Instance.Timer.n現在時刻;
					if (num < n進行用タイマ)
					{
						n進行用タイマ = num;
					}
					while ((num - n進行用タイマ) >= 10)
					{
						for (int j = 0; j < 3; j++)
						{
							this.n現在表示中のスコア[j] += this.nスコアの増分[j];

							if (this.n現在表示中のスコア[j] > (long)this.n現在の本当のスコア[j])
								this.n現在表示中のスコア[j] = (long)this.n現在の本当のスコア[j];
						}
						n進行用タイマ += 10;
					}
					for (int i = 1; i < 3; i++)
					{
						string str = this.n現在表示中のスコア[i].ToString("0000000000");
						for (int k = 0; k < 10; k++)
						{
							Rectangle rectangle;
							char ch = str[k];
							if (ch.Equals(' '))
							{
								rectangle = new Rectangle(
									0,
									0,
									(int)(12 * Scale.X),
									(int)(0x18 * Scale.Y)
									);
							}
							else
							{
								int num5 = int.Parse(str.Substring(k, 1));
								if (num5 < 5)
								{
									rectangle = new Rectangle(
										(int)(num5 * 12 * Scale.X),
										0,
										(int)(12 * Scale.X),
										(int)(0x18 * Scale.Y)
									);
								}
								else
								{
									rectangle = new Rectangle(
										(int)((num5 - 5) * 12 * Scale.X),
										(int)(0x18 * Scale.Y),
										(int)(12 * Scale.X),
										(int)(0x18 * Scale.Y)
									);
								}
							}
							if (txScore != null)
							{
								txScore.t2D描画(
									CDTXMania.Instance.Device,
									(this.ptSCORE[i - 1].X + (k * 12)) * Scale.X,
									this.ptSCORE[i - 1].Y * Scale.Y,
									rectangle
								);
							}
						}
					}

				}
				else
				{
					if (base.b初めての進行描画)
					{
						n進行用タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
						base.b初めての進行描画 = false;
					}
					long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					if (num < n進行用タイマ)
					{
						n進行用タイマ = num;
					}
					while ((num - n進行用タイマ) >= 10)
					{
						for (int j = 0; j < 3; j++)
						{
							this.n現在表示中のスコア[j] += this.nスコアの増分[j];

							if (this.n現在表示中のスコア[j] > (long)this.n現在の本当のスコア[j])
								this.n現在表示中のスコア[j] = (long)this.n現在の本当のスコア[j];
						}
						n進行用タイマ += 10;
					}
					string str = this.n現在表示中のスコア.Drums.ToString("0000000000");
					for (int i = 0; i < 10; i++)
					{
						Rectangle rectangle;
						char ch = str[i];
						if (ch.Equals(' '))
						{
							rectangle = new Rectangle(
								0,
								0,
								(int)(12 * Scale.X),
								(int)(0x18 * Scale.Y)
							);
						}
						else
						{
							int num4 = int.Parse(str.Substring(i, 1));
							if (num4 < 5)
							{
								rectangle = new Rectangle(
									(int)(num4 * 12 * Scale.X),
									0,
									(int)(12 * Scale.X),
									(int)(0x18 * Scale.Y)
								);
							}
							else
							{
								rectangle = new Rectangle(
									(int)((num4 - 5) * 12 * Scale.X),
									(int)(0x18 * Scale.Y),
									(int)(12 * Scale.X),
									(int)(0x18 * Scale.Y)
								);
							}
						}
						if (txScore != null)
						{
							txScore.t2D描画(
								CDTXMania.Instance.Device,
								(CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1068 + (i * 36) : 1490 + i * 36,
								30,
								rectangle
							);
						}
					}
				}
			}
			return 0;
		}

	}
}
