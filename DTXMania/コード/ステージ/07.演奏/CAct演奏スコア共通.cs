using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏スコア共通 : CActivity
	{
		protected STDGBSValue<long> nスコアの増分;
		protected STDGBSValue<double> n現在の本当のスコア;
		protected STDGBSValue<long> n現在表示中のスコア;
		protected long n進行用タイマ;
		private readonly Point[] ptSCORE = new Point[] { new Point(0x1f, 0x1a9), new Point(0x1e9, 0x1a9) };
		private CActDigit actDigit;

		public CAct演奏スコア共通()
		{
			base.list子Activities.Add(actDigit = new CActDigit(Color.Orange, Color.Black, Color.OrangeRed, Color.DarkOrange, 30));
			base.b活性化してない = true;
		}

		public double Get(EPart part)
		{
			return this.n現在の本当のスコア[part];
		}

		public void Set(EPart part, double nScore)
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
		public void Add(EPart part, long delta)
		{
			double rev = 1.0;
			switch (part)
			{
				case EPart.Unknown:
					throw new ArgumentException();
				case EPart.Drums:
					if (!CDTXMania.Instance.ConfigIni.bIsAutoPlay(part))
					{
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BD == true)
						{
							rev /= 2;
						}
					}
					break;
				case EPart.Guitar:
					if (!CDTXMania.Instance.ConfigIni.bIsAutoPlay(part))
					{
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail)
						{
							rev /= 2;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick)
						{
							rev /= 3;
						}
						// Auto Neck
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtR || CDTXMania.Instance.ConfigIni.bAutoPlay.GtG || CDTXMania.Instance.ConfigIni.bAutoPlay.GtB)
						{
							rev /= 4;
						}
					}
					break;
				case EPart.Bass:
					if (!CDTXMania.Instance.ConfigIni.bIsAutoPlay(part))
					{
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsWail)
						{
							rev /= 2;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick)
						{
							rev /= 3;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsR ||
														CDTXMania.Instance.ConfigIni.bAutoPlay.BsG ||
														CDTXMania.Instance.ConfigIni.bAutoPlay.BsB)
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
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
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
			if (b活性化してる)
			{
				if (b初めての進行描画)
				{
					n進行用タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					b初めての進行描画 = false;
				}

				for (EPart inst = EPart.Drums; inst <= EPart.Bass; inst++)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効(inst) && CDTXMania.Instance.DTX.bチップがある[inst])
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

						int x = CDTXMania.Instance.ConfigIni.cdInstX[inst][CDTXMania.Instance.ConfigIni.eActiveInst].Value
						+ CDTXMania.Instance.Coordinates.Instrument[inst].W / 2;
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
