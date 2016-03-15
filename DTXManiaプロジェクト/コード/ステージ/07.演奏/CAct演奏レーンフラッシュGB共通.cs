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
		private CTexture tx = new CTexture();

		// コンストラクタ
		public CAct演奏レーンフラッシュGB共通()
		{
			base.b活性化してない = true;
		}

		// メソッド
		public void Start(int nLane)
		{
			this.ct進行[nLane] = new CCounter(0, 100, 1, CDTXMania.Instance.Timer);
		}

		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 6; i++)
			{
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
				tx = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay guitar lane flush.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref tx);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				int imgX = CDTXMania.Instance.Coordinates.ImgGtLaneFlash.X;
				for (int i = 0; i < 6; i++)
				{
					if (i % 3 == 0)
					{
						imgX = CDTXMania.Instance.Coordinates.ImgGtLaneFlash.X;
					}
					E楽器パート e楽器パート = (i < 3) ? E楽器パート.GUITAR : E楽器パート.BASS;

					int x = 0;
					int w = 0;
					if (i == 0)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtR.X;
						w = CDTXMania.Instance.Coordinates.Lane.GtR.W;
					}
					else if (i == 1)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtG.X;
						w = CDTXMania.Instance.Coordinates.Lane.GtG.W;
					}
					else if (i == 2)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtB.X;
						w = CDTXMania.Instance.Coordinates.Lane.GtB.W;
					}
					else if (i == 3)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsR.X;
						w = CDTXMania.Instance.Coordinates.Lane.BsR.W;
					}
					else if (i == 4)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsG.X;
						w = CDTXMania.Instance.Coordinates.Lane.BsG.W;
					}
					else if (i == 5)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsB.X;
						w = CDTXMania.Instance.Coordinates.Lane.BsB.W;
					}

					if (!ct進行[i].b停止中)
					{

						if (tx != null)
						{
							if (CDTXMania.Instance.ConfigIni.bReverse[e楽器パート])
							{
								tx.vc拡大縮小倍率.Y = -1;
							}
							else
							{
								tx.vc拡大縮小倍率.Y = 1;
							}
							int y = CDTXMania.Instance.Coordinates.LaneFlash[e楽器パート].Y;
							tx.t2D描画(
								CDTXMania.Instance.Device,
								x,
								CDTXMania.Instance.ConfigIni.bReverse[e楽器パート] ? SampleFramework.GameWindowSize.Height - y - CDTXMania.Instance.Coordinates.ImgGtLaneFlash.H : y,
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
