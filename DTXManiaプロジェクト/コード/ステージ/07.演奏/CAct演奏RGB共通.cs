using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏RGB共通 : CActivity
	{
		// プロパティ
		protected bool[] b押下状態 = new bool[6];
		protected CTexture txRGB;


		// コンストラクタ

		public CAct演奏RGB共通()
		{
			base.b活性化してない = true;
		}

		// メソッド
		public void Push(int nLane)
		{
			this.b押下状態[nLane] = true;
		}


		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.b押下状態[i] = false;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txRGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay RGB buttons.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txRGB);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				for (int i = 0; i < 6; i++)
				{
					E楽器パート inst = i < 3 ? E楽器パート.GUITAR : E楽器パート.BASS;
					if (CDTXMania.Instance.DTX.bチップがある[(int)inst])
					{
						int x = 0;
						Rectangle rc;

						if (i == 0)
						{
							x = CDTXMania.Instance.Coordinates.Lane.GtR.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonR : CDTXMania.Instance.Coordinates.ImgGtButtonR;
						}
						else if (i == 1)
						{
							x = CDTXMania.Instance.Coordinates.Lane.GtG.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonG : CDTXMania.Instance.Coordinates.ImgGtButtonG;
						}
						else if (i == 2)
						{
							x = CDTXMania.Instance.Coordinates.Lane.GtB.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonB : CDTXMania.Instance.Coordinates.ImgGtButtonB;
						}
						else if (i == 3)
						{
							x = CDTXMania.Instance.Coordinates.Lane.BsR.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonR : CDTXMania.Instance.Coordinates.ImgGtButtonR;
						}
						else if (i == 4)
						{
							x = CDTXMania.Instance.Coordinates.Lane.BsG.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonG : CDTXMania.Instance.Coordinates.ImgGtButtonG;
						}
						else if (i == 5)
						{
							x = CDTXMania.Instance.Coordinates.Lane.BsB.X;
							rc = b押下状態[i] ? CDTXMania.Instance.Coordinates.ImgGtPressingButtonB : CDTXMania.Instance.Coordinates.ImgGtButtonB;
						}
						else
						{
							rc = new Rectangle();
						}

						if (txRGB != null)
						{
							int y = C演奏判定ライン座標共通.n演奏RGBボタンY座標(inst);
							if( CDTXMania.Instance.ConfigIni.bReverse[inst] )
							{
								y -= rc.Height/2;
							}
							txRGB.t2D描画(CDTXMania.Instance.Device, x, y, rc);
						}
					}
				}
				for(int i = 0; i < 6; ++i)
				{
					b押下状態[i] = false;
				}
			}

			return 0;
		}
	}
}
