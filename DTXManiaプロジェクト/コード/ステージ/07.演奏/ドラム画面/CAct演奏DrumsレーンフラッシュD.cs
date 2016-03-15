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
		[StructLayout(LayoutKind.Sequential)]
		private struct STレーンサイズ
		{
			public int x;
			public int w;
			public STレーンサイズ(int x_, int w_)
			{
				x = x_;
				w = w_;
			}
		}

		private CCounter[] ct進行 = new CCounter[8];
		private CTexture tx = new CTexture();

		// コンストラクタ
		public CAct演奏DrumsレーンフラッシュD()
		{
			base.b活性化してない = true;
		}


		// メソッド
		public void Start(Eレーン lane, float f強弱度合い)
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
			if (!base.b活性化してない)
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
					int x = 0;
					int w = 0;

					if (i == 0)
					{
						x = CDTXMania.Instance.Coordinates.Lane.LCY.X;
						w = CDTXMania.Instance.Coordinates.Lane.LCY.W;
					}
					else if (i == 1)
					{
						x = CDTXMania.Instance.Coordinates.Lane.HHC.X;
						w = CDTXMania.Instance.Coordinates.Lane.HHC.W;
					}
					else if (i == 2)
					{
						x = CDTXMania.Instance.Coordinates.Lane.SD.X;
						w = CDTXMania.Instance.Coordinates.Lane.SD.W;
					}
					else if (i == 3)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BD.X;
						w = CDTXMania.Instance.Coordinates.Lane.BD.W;
					}
					else if (i == 4)
					{
						x = CDTXMania.Instance.Coordinates.Lane.HT.X;
						w = CDTXMania.Instance.Coordinates.Lane.HT.W;
					}
					else if (i == 5)
					{
						x = CDTXMania.Instance.Coordinates.Lane.LT.X;
						w = CDTXMania.Instance.Coordinates.Lane.LT.W;
					}
					else if (i == 6)
					{
						x = CDTXMania.Instance.Coordinates.Lane.FT.X;
						w = CDTXMania.Instance.Coordinates.Lane.FT.W;
					}
					else if (i == 7)
					{
						x = CDTXMania.Instance.Coordinates.Lane.CY.X;
						w = CDTXMania.Instance.Coordinates.Lane.CY.W;
					}

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