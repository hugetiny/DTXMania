using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏レーンフラッシュGB共通 : CActivity
	{
		protected CCounter[] ct進行 = new CCounter[6];
		CTexture tx = new CTexture();
		static ELane[] lanes = new ELane[] { ELane.GtR, ELane.GtG, ELane.GtB, ELane.BsR, ELane.BsG, ELane.BsB };

		public void Start(int nLane)
		{
			this.ct進行[nLane] = new CCounter(0, 100, 1, CDTXMania.Instance.Timer);
		}

		public override void On活性化()
		{
			if (b活性化してない)
			{
				for (int i = 0; i < 6; i++)
				{
					this.ct進行[i] = new CCounter();
				}
				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (b活性化してる)
			{
				for (int i = 0; i < 6; i++)
				{
					this.ct進行[i] = null;
				}
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				tx = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay guitar lane flush.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref tx);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (b活性化してる &&
				CDTXMania.Instance.ConfigIni.bGuitar有効 &&
				CDTXMania.Instance.ConfigIni.eDark == EDark.Off)
			{
				int imgX = CDTXMania.Instance.Coordinates.ImgGtLaneFlash.X;
				for (int i = 0; i < 6; i++)
				{
					if (i % 3 == 0)
					{
						imgX = CDTXMania.Instance.Coordinates.ImgGtLaneFlash.X;
					}
					EPart inst = (i < 3) ? EPart.Guitar : EPart.Bass;

					int x = CDTXMania.Instance.ConfigIni.GetLaneX(lanes[i]);
					int w = CDTXMania.Instance.ConfigIni.GetLaneW(lanes[i]);

					if (!ct進行[i].b停止中 && CDTXMania.Instance.DTX.bチップがある[inst])
					{

						if (tx != null)
						{
							if (CDTXMania.Instance.ConfigIni.bReverse[inst])
							{
								tx.vc拡大縮小倍率.Y = -1;
							}
							else
							{
								tx.vc拡大縮小倍率.Y = 1;
							}
							int y = CDTXMania.Instance.Coordinates.LaneFlash[inst].Y;
							tx.t2D描画(
								CDTXMania.Instance.Device,
								x,
								CDTXMania.Instance.ConfigIni.bReverse[inst] ? SampleFramework.GameWindowSize.Height - y - CDTXMania.Instance.Coordinates.ImgGtLaneFlash.H : y,
								new Rectangle(
									imgX,
									CDTXMania.Instance.Coordinates.ImgGtLaneFlash.Y,
									(int)(w * (100 - ct進行[i].n現在の値) / 100),
									(int)(CDTXMania.Instance.Coordinates.ImgGtLaneFlash.H * tx.vc拡大縮小倍率.Y)
								)
							);
						}


						ct進行[i].t進行();
						if (ct進行[i].b終了値に達した)
						{
							ct進行[i].t停止();
						}
					}

					imgX += w;
				}
			}
			return 0;
		}
	}
}
