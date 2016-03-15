using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using FDK;
using System.Drawing;

namespace DTXMania
{
	class CAct演奏Lane : CActivity
	{
		private CTexture txLane;

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref txLane);
				base.OnManagedリソースの解放();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				using (Bitmap lanetex = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height))
				{
					using (Graphics g = Graphics.FromImage(lanetex))
					{
						using (Pen pen = new Pen(Brushes.White, 3f))
						{
							using (Pen grayPen = new Pen(Brushes.Gray, 1f))
							{
								using (Bitmap lanebg = new Bitmap(20, 20))
								{
									for (int y = 0; y < 20; ++y)
									{
										for (int x = 0; x < 20; ++x)
										{
											int p = (x / 5);
											int q = (y / 5);

											lanebg.SetPixel(x, y,
												(p + q) % 2 == 1 ? Color.FromArgb(CDTXMania.Instance.ConfigIni.n背景の透過度, 10, 10, 10) :
												Color.FromArgb(CDTXMania.Instance.ConfigIni.n背景の透過度, 20, 20, 20));
										}
									}

									Func<STX, STRect, bool> drawer = (x, w) =>
									{
										using (TextureBrush tex = new TextureBrush(lanebg))
										{
											g.FillRectangle(tex, new Rectangle(x.X, 0, w.W, SampleFramework.GameWindowSize.Height));
											g.DrawLine(pen, x.X, 0, x.X, SampleFramework.GameWindowSize.Height);
											g.DrawLine(grayPen, x.X + 2, 0, x.X + 2, SampleFramework.GameWindowSize.Height);
											g.DrawLine(pen, x.X + w.W, 0, x.X + w.W, SampleFramework.GameWindowSize.Height);
											g.DrawLine(grayPen, x.X + w.W + 2, 0, x.X + w.W + 2, SampleFramework.GameWindowSize.Height);
										}
										return false;
									};

									if (CDTXMania.Instance.ConfigIni.bDrums有効)
									{
										drawer(CDTXMania.Instance.Coordinates.Lane.LCY, CDTXMania.Instance.Coordinates.ImgDrChipCY);
										drawer(CDTXMania.Instance.Coordinates.Lane.HHC, CDTXMania.Instance.Coordinates.ImgDrChipHHC);
										drawer(CDTXMania.Instance.Coordinates.Lane.SD, CDTXMania.Instance.Coordinates.ImgDrChipSD);
										drawer(CDTXMania.Instance.Coordinates.Lane.BD, CDTXMania.Instance.Coordinates.ImgDrChipBD);
										drawer(CDTXMania.Instance.Coordinates.Lane.HT, CDTXMania.Instance.Coordinates.ImgDrChipHT);
										drawer(CDTXMania.Instance.Coordinates.Lane.LT, CDTXMania.Instance.Coordinates.ImgDrChipLT);
										drawer(CDTXMania.Instance.Coordinates.Lane.FT, CDTXMania.Instance.Coordinates.ImgDrChipFT);
										drawer(CDTXMania.Instance.Coordinates.Lane.CY, CDTXMania.Instance.Coordinates.ImgDrChipCY);
									}
									if (CDTXMania.Instance.ConfigIni.bGuitar有効)
									{
										drawer(CDTXMania.Instance.Coordinates.Lane.GtR, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.GtG, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.GtB, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.GtW, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.BsR, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.BsG, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.BsB, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										drawer(CDTXMania.Instance.Coordinates.Lane.BsW, CDTXMania.Instance.Coordinates.ImgGtRGBButton);
									}
									this.txLane = TextureFactory.tテクスチャの生成(lanetex, true);
								}
							}
						}
					}
				}
				base.OnManagedリソースの作成();
			}
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				txLane.t2D描画(CDTXMania.Instance.Device, 0, 0);
				return base.On進行描画();
			}
			return 0;
		}
	}
}
