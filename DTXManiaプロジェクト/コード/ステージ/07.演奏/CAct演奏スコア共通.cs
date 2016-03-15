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
		private readonly Point[] ptSCORE = new Point[] { new Point(0x1f, 0x1a9), new Point(0x1e9, 0x1a9) };
		private CActDigit actDigit;

		// コンストラクタ
		public CAct演奏スコア共通()
		{
			base.list子Activities.Add(actDigit = new CActDigit(Color.Orange, Color.Black, Color.OrangeRed, Color.DarkOrange, 30));
			base.b活性化してない = true;
		}

		// メソッド
		public double Get(E楽器パート part)
		{
			return this.n現在の本当のスコア[part];
		}

		public void Set(E楽器パート part, double nScore)
		{
			if (this.n現在の本当のスコア[part] != nScore)
			{
				this.n現在の本当のスコア[part] = nScore;
				this.nスコアの増分[part] = (long)(((double)(this.n現在の本当のスコア[part] - this.n現在表示中のスコア[part])) / 20.0);
				if (this.nスコアの増分[part] < 1L)
				{
					this.nスコアの増分[part] = 1L;
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
				case E楽器パート.UNKNOWN:
					throw new ArgumentException();
				case E楽器パート.DRUMS:
					if (!CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである)
					{
						if (bAutoPlay.BD == true)
						{
							rev /= 2;
						}
					}
					break;
				case E楽器パート.GUITAR:
					if (!CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである)
					{
						if (bAutoPlay.GtW)
						{
							rev /= 2;
						}
						if (bAutoPlay.GtPick)
						{
							rev /= 3;
						}
						// Auto Neck
						if (bAutoPlay.GtR || bAutoPlay.GtG || bAutoPlay.GtB)
						{
							rev /= 4;
						}
					}
					break;
				case E楽器パート.BASS:
					if (!CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである)
					{
						if (bAutoPlay.BsW)
						{
							rev /= 2;
						}
						if (bAutoPlay.BsPick)
						{
							rev /= 3;
						}
						if (bAutoPlay.BsR || bAutoPlay.BsG || bAutoPlay.BsB)
						{
							rev /= 4;
						}
					}
					break;
			}
			this.Set(part, this.Get(part) + delta * rev);
		}


		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.n進行用タイマ = -1;
				for (E楽器パート i = E楽器パート.DRUMS; i <= E楽器パート.BASS; i++)
				{
					this.n現在表示中のスコア[i] = 0L;
					this.n現在の本当のスコア[i] = 0L;
					this.nスコアの増分[i] = 0L;
				}
				base.On活性化();
			}
		}

		// CActivity 実装（共通クラスからの差分のみ）
		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (base.b初めての進行描画)
				{
					n進行用タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					base.b初めての進行描画 = false;
				}

				for (E楽器パート inst = E楽器パート.DRUMS; inst <= E楽器パート.BASS; inst++)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効[inst])
					{
						long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
						if (num < n進行用タイマ)
						{
							n進行用タイマ = num;
						}
						while ((num - n進行用タイマ) >= 10)
						{
							this.n現在表示中のスコア[inst] += this.nスコアの増分[inst];

							if (this.n現在表示中のスコア[inst] > (long)this.n現在の本当のスコア[inst])
								this.n現在表示中のスコア[inst] = (long)this.n現在の本当のスコア[inst];

							n進行用タイマ += 10;
						}

						int x = CDTXMania.Instance.Coordinates.Score[inst].X;
						int y = CDTXMania.Instance.Coordinates.Score[inst].Y;

						if (CDTXMania.Instance.ConfigIni.bReverse[inst])
						{
							y = SampleFramework.GameWindowSize.Height - y - actDigit.MaximumHeight;
						}
						int w = actDigit.Measure(this.n現在表示中のスコア[inst]);

						actDigit.Draw(this.n現在表示中のスコア[inst], x - w / 2, y);
					}
				}
			}
			return 0;
		}
	}
}
