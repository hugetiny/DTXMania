using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpDX;
using FDK;

using Point = System.Drawing.Point;

namespace DTXMania
{
	internal class CAct演奏チップファイアGB : CActivity
	{
		static ELane[] lanes = new ELane[] { ELane.GtR, ELane.GtG, ELane.GtB, ELane.BsR, ELane.BsG, ELane.BsB };

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
					EPart e楽器パート = (nLane < 3) ? EPart.Guitar : EPart.Bass;
					int x = CDTXMania.Instance.ConfigIni.GetLaneX(lanes[nLane])
						+ CDTXMania.Instance.ConfigIni.GetLaneW(lanes[nLane]) / 2;

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
			if (b活性化してる && CDTXMania.Instance.ConfigIni.bGuitar有効 &&
					(CDTXMania.Instance.DTX.bチップがある.Guitar || CDTXMania.Instance.DTX.bチップがある.Bass))
			{
				for (int i = 0; i < 6; i++)
				{
					ct進行[i].t進行();
					if (ct進行[i].b終了値に達した)
					{
						ct進行[i].t停止();
					}
				}
				for (int j = 0; j < 6; j++)
				{
					if ((ct進行[j].n現在の経過時間ms != -1) && (tx火花[j % 3] != null))
					{
						float scale = (float)(3.0 * Math.Cos((Math.PI * (90.0 - (90.0 * (((double)ct進行[j].n現在の値) / 56.0)))) / 180.0));
						int x = (int)(pt中央位置[j].X) - ((int)((tx火花[j % 3].sz画像サイズ.Width * scale) / 2f));
						int y = (int)(pt中央位置[j].Y) - ((int)((tx火花[j % 3].sz画像サイズ.Height * scale) / 2f));
						tx火花[j % 3].n透明度 = (ct進行[j].n現在の値 < 0x1c) ? 0xff : (0xff - ((int)(255.0 * Math.Cos((Math.PI * (90.0 - (90.0 * (((double)(ct進行[j].n現在の値 - 0x1c)) / 28.0)))) / 180.0))));
						tx火花[j % 3].vc拡大縮小倍率 = new Vector3(scale, scale, 1f);

						tx火花[j % 3].t2D描画(CDTXMania.Instance.Device, x, y);
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
