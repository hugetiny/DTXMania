using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using FDK;
using DTXMania.Coordinates;

using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

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
												(p + q) % 2 == 1 ? Color.FromArgb(CDTXMania.Instance.ConfigIni.nBGAlpha, 10, 10, 10) :
												Color.FromArgb(CDTXMania.Instance.ConfigIni.nBGAlpha, 20, 20, 20));
										}
									}

									Func<int, CRect, bool> drawer = (x, w) =>
									{
										using (TextureBrush tex = new TextureBrush(lanebg))
										{
											g.FillRectangle(tex, new Rectangle(x, 0, w.W, SampleFramework.GameWindowSize.Height));
											g.DrawLine(pen, x, 0, x, SampleFramework.GameWindowSize.Height);
											g.DrawLine(grayPen, x + 2, 0, x + 2, SampleFramework.GameWindowSize.Height);
											g.DrawLine(pen, x + w.W, 0, x + w.W, SampleFramework.GameWindowSize.Height);
											g.DrawLine(grayPen, x + w.W + 2, 0, x + w.W + 2, SampleFramework.GameWindowSize.Height);
										}
										return false;
									};

									if (CDTXMania.Instance.ConfigIni.bDrums有効 && CDTXMania.Instance.DTX.bチップがある[EPart.Drums])
									{
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.LC), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.CY]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.HH), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.HH]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.SD), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.SD]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BD), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.BD]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.HT), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.HT]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.LT), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.LT]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.FT), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.FT]);
										drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.CY), CDTXMania.Instance.Coordinates.ImgDrChip[EPad.CY]);
									}
									if (CDTXMania.Instance.ConfigIni.bGuitar有効)
									{
										if (CDTXMania.Instance.DTX.bチップがある[EPart.Guitar])
										{
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.GtR), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.GtG), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.GtB), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.GtW), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										}
										if (CDTXMania.Instance.DTX.bチップがある[EPart.Bass])
										{
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BsR), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BsG), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BsB), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
											drawer(CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BsW), CDTXMania.Instance.Coordinates.ImgGtRGBButton);
										}
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
			if (b活性化してる && CDTXMania.Instance.ConfigIni.eDark == EDark.Off)
			{
				txLane.t2D描画(CDTXMania.Instance.Device, 0, 0);
				return base.On進行描画();
			}
			return 0;
		}
	}
}
