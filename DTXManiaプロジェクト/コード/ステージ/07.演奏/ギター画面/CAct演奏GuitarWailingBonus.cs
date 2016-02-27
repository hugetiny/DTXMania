using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏GuitarWailingBonus : CAct演奏WailingBonus共通
	{
		// メソッド

		public CAct演奏GuitarWailingBonus()
		{
			base.b活性化してない = true;
		}
		//public override void Start( E楽器パート part )
		//{
		//    this.Start( part, null );
		//}
		public override void Start(E楽器パート part, CChip r歓声Chip)
		{
			if (part != E楽器パート.DRUMS)
			{
				for (int i = 0; i < 4; i++)
				{
					if ((this.ct進行用[(int)part, i] == null) || this.ct進行用[(int)part, i].b停止中)
					{
						this.ct進行用[(int)part, i] = new CCounter(0, 300, 2, CDTXMania.Instance.Timer);
						if (CDTXMania.Instance.ConfigIni.b歓声を発声する)
						{
							if (r歓声Chip != null)
							{
								CDTXMania.Instance.DTX.tチップの再生(r歓声Chip, CSound管理.rc演奏用タイマ.nシステム時刻, (int)Eレーン.BGM, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
								return;
							}
							CDTXMania.Instance.Skin.sound歓声音.n位置_次に鳴るサウンド = (part == E楽器パート.GUITAR) ? -50 : 50;
							CDTXMania.Instance.Skin.sound歓声音.t再生する();
							return;
						}
						break;
					}
				}
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.ct進行用[i, j] = null;
				}
			}
			base.On活性化();
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 2; i++)
				{
					E楽器パート e楽器パート = (i == 0) ? E楽器パート.GUITAR : E楽器パート.BASS;
					for (int k = 0; k < 4; k++)
					{
						if ((this.ct進行用[(int)e楽器パート, k] != null) && !this.ct進行用[(int)e楽器パート, k].b停止中)
						{
							if (this.ct進行用[(int)e楽器パート, k].b終了値に達した)
							{
								this.ct進行用[(int)e楽器パート, k].t停止();
							}
							else
							{
								this.ct進行用[(int)e楽器パート, k].t進行();
							}
						}
					}
				}
				for (int j = 0; j < 2; j++)
				{
					E楽器パート e楽器パート2 = (j == 0) ? E楽器パート.GUITAR : E楽器パート.BASS;
					for (int m = 0; m < 4; m++)
					{
						if ((this.ct進行用[(int)e楽器パート2, m] != null) && !this.ct進行用[(int)e楽器パート2, m].b停止中)
						{
							int x = ((e楽器パート2 == E楽器パート.GUITAR) ? 0x1a : 480) + 0x71;
							int num6 = 0;
							int num7 = 0;
							int num8 = this.ct進行用[(int)e楽器パート2, m].n現在の値;
							if (num8 < 100)
							{
								num6 = (int)(64.0 + (290.0 * Math.Cos(Math.PI / 2 * (((double)num8) / 100.0))));
							}
							else if (num8 < 150)
							{
								num6 = (int)(64.0 + ((150 - num8) * Math.Sin((Math.PI * ((num8 - 100) % 0x19)) / 25.0)));
							}
							else if (num8 < 200)
							{
								num6 = 0x40;
							}
							else
							{
								num6 = (int)(64.0 - (((double)(290 * (num8 - 200))) / 100.0));
							}
							if (CDTXMania.Instance.ConfigIni.bReverse[(int)e楽器パート2])
							{
								num6 = (0x199 - num6) - 0xf4;
							}
							Rectangle rectangle = new Rectangle(0, 0, 0x1a, 0x7a * 2);
							if ((0x199 - num6) < rectangle.Bottom)
							{
								rectangle.Height = (0x199 - num6) - rectangle.Top;
							}
							if (num6 < 0)
							{
								rectangle.Y = -num6;
								rectangle.Height -= -num6;
								num7 = -num6;
							}
							if ((rectangle.Top < rectangle.Bottom) && (this.txWailingBonus != null))
							{
								rectangle.X = (int)(rectangle.X * Scale.X);
								rectangle.Y = (int)(rectangle.Y * Scale.Y);
								rectangle.Width = (int)(rectangle.Width * Scale.X);
								rectangle.Height = (int)(rectangle.Height * Scale.Y);

								this.txWailingBonus.t2D描画(
									CDTXMania.Instance.Device,
									x * Scale.X,
									(num6 + num7) * Scale.Y,
									rectangle
								);
							}
						}
					}
				}
			}
			return 0;
		}
	}
}
