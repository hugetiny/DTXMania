using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏DrumsレーンフラッシュD : CActivity
	{
		static ELane[] lanes = new ELane[] { ELane.LC, ELane.HH, ELane.SD, ELane.BD, ELane.HT, ELane.LT, ELane.FT, ELane.CY };
		CCounter[] ct進行 = new CCounter[8];
		CTexture tx = new CTexture();

		// コンストラクタ
		public CAct演奏DrumsレーンフラッシュD()
		{
			base.b活性化してない = true;
		}


		// メソッド
		public void Start(ELane lane, float f強弱度合い)
		{
			int num = (int)((1f - f強弱度合い) * 55f);
			this.ct進行[(int)lane] = new CCounter(num, 100, 4, CDTXMania.Instance.Timer);
		}


		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 8; i++)
			{
				this.ct進行[i] = new CCounter();
			}
			base.On活性化();
		}

		public override void On非活性化()
		{
			for (int i = 0; i < 8; i++)
			{
				this.ct進行[i] = null;
			}
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				tx = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay drums lane flush.png"));
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
			if (b活性化してる &&
				CDTXMania.Instance.ConfigIni.bDrums有効 &&
				CDTXMania.Instance.DTX.bチップがある.Drums &&
				CDTXMania.Instance.ConfigIni.eDark == EDark.Off)
			{
				for (int i = 0; i < 8; i++)
				{
					if (!this.ct進行[i].b停止中)
					{
						this.ct進行[i].t進行();
						if (this.ct進行[i].b終了値に達した)
						{
							this.ct進行[i].t停止();
						}
					}
				}

				int imgX = CDTXMania.Instance.Coordinates.ImgDrLaneFlash.X;
				for (int i = 0; i < 8; i++)
				{
					int x = CDTXMania.Instance.ConfigIni.GetLaneX(lanes[i]);
					int w = CDTXMania.Instance.ConfigIni.GetLaneW(lanes[i]);
					if (!this.ct進行[i].b停止中)
					{
						if (tx != null)
						{
							if (CDTXMania.Instance.ConfigIni.bReverse.Drums)
							{
								tx.vc拡大縮小倍率.Y = -1;
							}
							else
							{
								tx.vc拡大縮小倍率.Y = 1;
							}
							int y = CDTXMania.Instance.ConfigIni.bReverse.Drums ? 0 : CDTXMania.Instance.Coordinates.LaneFlash.Drums.Y;
							y += (CDTXMania.Instance.ConfigIni.bReverse.Drums ? -1 : 1) * (int)(CDTXMania.Instance.Coordinates.ImgDrLaneFlash.H * ((ct進行[i].n現在の値) / 100.0));

							tx.t2D描画(
								CDTXMania.Instance.Device,
								x,
								y,
								new Rectangle(
									imgX,
									CDTXMania.Instance.Coordinates.ImgDrLaneFlash.Y,
									w,
									(int)(CDTXMania.Instance.Coordinates.ImgDrLaneFlash.H * tx.vc拡大縮小倍率.Y)
								)
							);
						}
					}

					imgX += w;
				}
			}
			return 0;
		}
	}
}