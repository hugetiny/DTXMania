using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏チップファイアGB : CActivity
	{
		public CAct演奏チップファイアGB()
		{
			base.b活性化してない = true;
		}

		public void Start(int nLane)
		{
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				if (0 <= nLane && nLane < 6)
				{
					E楽器パート e楽器パート = (nLane < 3) ? E楽器パート.GUITAR : E楽器パート.BASS;
					int x = 0;

					if (nLane == 0)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtR.X + CDTXMania.Instance.Coordinates.Lane.GtR.W / 2;
					}
					else if (nLane == 1)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtG.X + CDTXMania.Instance.Coordinates.Lane.GtG.W / 2;
					}
					else if (nLane == 2)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtB.X + CDTXMania.Instance.Coordinates.Lane.GtB.W / 2;
					}
					else if (nLane == 3)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsR.X + CDTXMania.Instance.Coordinates.Lane.BsR.W / 2;
					}
					else if (nLane == 4)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsG.X + CDTXMania.Instance.Coordinates.Lane.BsG.W / 2;
					}
					else if (nLane == 5)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsB.X + CDTXMania.Instance.Coordinates.Lane.BsB.W / 2;
					}

					int offsety = CDTXMania.Instance.Coordinates.ImgJudgeLine.H / 2;
					int y = C演奏判定ライン座標共通.n判定ラインY座標(e楽器パート, false, true);
					if (!CDTXMania.Instance.ConfigIni.bReverse[e楽器パート])
					{
						offsety -= offsety;
					}
					this.pt中央位置[nLane].X = x;
					this.pt中央位置[nLane].Y = y + offsety;
					// #24736 2011.2.17 yyagi: (0, 0x38, 4,..) -> (24, 0x38, 8) に変更 ギターチップの光り始めを早くするため
					this.ct進行[nLane].t開始(28, 56, 8, CDTXMania.Instance.Timer);
				}
			}
		}

		public override void On活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.pt中央位置[i] = new Point(0, 0);
				this.ct進行[i] = new CCounter();
			}
			base.On活性化();
		}

		public override void On非活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.ct進行[i] = null;
			}
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx火花[0] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay chip fire red.png"));
				if (this.tx火花[0] != null)
				{
					this.tx火花[0].b加算合成 = true;
				}
				this.tx火花[1] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay chip fire green.png"));
				if (this.tx火花[1] != null)
				{
					this.tx火花[1].b加算合成 = true;
				}
				this.tx火花[2] = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay chip fire blue.png"));
				if (this.tx火花[2] != null)
				{
					this.tx火花[2].b加算合成 = true;
				}
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.tx火花[0]);
				TextureFactory.tテクスチャの解放(ref this.tx火花[1]);
				TextureFactory.tテクスチャの解放(ref this.tx火花[2]);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 6; i++)
				{
					this.ct進行[i].t進行();
					if (this.ct進行[i].b終了値に達した)
					{
						this.ct進行[i].t停止();
					}
				}
				for (int j = 0; j < 6; j++)
				{
					if ((this.ct進行[j].n現在の経過時間ms != -1) && (this.tx火花[j % 3] != null))
					{
						float scale = (float)(3.0 * Math.Cos((Math.PI * (90.0 - (90.0 * (((double)this.ct進行[j].n現在の値) / 56.0)))) / 180.0));
						int x = (int)(this.pt中央位置[j].X) - ((int)((this.tx火花[j % 3].sz画像サイズ.Width * scale) / 2f));
						int y = (int)(this.pt中央位置[j].Y) - ((int)((this.tx火花[j % 3].sz画像サイズ.Height * scale) / 2f));
						this.tx火花[j % 3].n透明度 = (this.ct進行[j].n現在の値 < 0x1c) ? 0xff : (0xff - ((int)(255.0 * Math.Cos((Math.PI * (90.0 - (90.0 * (((double)(this.ct進行[j].n現在の値 - 0x1c)) / 28.0)))) / 180.0))));
						this.tx火花[j % 3].vc拡大縮小倍率 = new Vector3(scale, scale, 1f);

						this.tx火花[j % 3].t2D描画(CDTXMania.Instance.Device, x, y);
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		private CCounter[] ct進行 = new CCounter[6];
		private Point[] pt中央位置 = new Point[6];
		private CTexture[] tx火花 = new CTexture[3];
		#endregion
	}
}
