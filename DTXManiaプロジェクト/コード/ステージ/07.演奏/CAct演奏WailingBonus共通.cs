using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏WailingBonus共通 : CActivity
	{
		public CAct演奏WailingBonus共通()
		{
			for (int i = 0; i < 3; ++i)
			{
				for (int j = 0; j < 4; ++j)
				{
					ct進行用[i, j] = null;
				}
			}
			base.b活性化してない = true;
		}

		public void Start(E楽器パート part, CChip r歓声Chip = null)
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

		public override void On活性化()
		{
			base.On活性化();
		}

		public override void On非活性化()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.ct進行用[i, j] = null;
				}
			}
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txWailingBonus = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay wailing bonus.png"), false);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txWailingBonus);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				for (int i = 0; i < 2; i++)
				{
					E楽器パート e楽器パート = (i == 0) ? E楽器パート.GUITAR : E楽器パート.BASS;
					if (CDTXMania.Instance.ConfigIni.b楽器有効[e楽器パート])
					{
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

							if ((this.ct進行用[(int)e楽器パート, k] != null) && !this.ct進行用[(int)e楽器パート, k].b停止中)
							{
								Rectangle rc = CDTXMania.Instance.Coordinates.ImgGtWailingBonus;

								int x = (e楽器パート == E楽器パート.GUITAR) ?
									CDTXMania.Instance.Coordinates.Lane.GtW.X + (CDTXMania.Instance.Coordinates.Lane.GtW.W - rc.Width) / 2 :
									CDTXMania.Instance.Coordinates.Lane.BsW.X + (CDTXMania.Instance.Coordinates.Lane.BsW.W - rc.Width) / 2;

								int y = 0;
								int ct = this.ct進行用[(int)e楽器パート, k].n現在の値;

								int yj = C演奏判定ライン座標共通.n判定ラインY座標(e楽器パート, false, true, C演奏判定ライン座標共通.Reverse.NotReverse);

								if (ct < 100)
								{
									y = (int)(yj + (rc.Height * Math.Cos(Math.PI / 2 * (((double)ct) / 100.0))));
								}
								else if (ct < 150)
								{
									y = (int)(yj + ((150 - ct) * Math.Sin((Math.PI * ((ct - 100) % 25)) / 25.0)));
								}
								else if (ct < 200)
								{
									y = yj;
								}
								else
								{
									y = (int)(yj - (((double)(rc.Height * (ct - 200))) / 100.0));
								}

								if (CDTXMania.Instance.ConfigIni.bReverse[e楽器パート])
								{
									y = SampleFramework.GameWindowSize.Height - y - rc.Height;
								}

								if (this.txWailingBonus != null)
								{
									this.txWailingBonus.t2D描画(CDTXMania.Instance.Device, x, y, rc);
								}
							}

						}
					}
				}
			}
			return 0;
		}

		protected CCounter[,] ct進行用 = new CCounter[3, 4];
		protected CTextureAf txWailingBonus;
	}
}
