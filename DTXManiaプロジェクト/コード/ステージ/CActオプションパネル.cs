using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX.Direct3D9;
using FDK;
using DTXMania.Coordinates;

namespace DTXMania
{
	internal class CActオプションパネル : CActivity
	{
		public STDGBSValue<CXY> Pos;

		EOptionPanelDirection direction;
		Rectangle rect;

		readonly Point[] ptDark = new Point[] { new Point(1, 0), new Point(1, 1), new Point(1, 7) };
		/// <summary>
		/// Off, Hid, Sud, HidSud, SInv, FInv
		/// </summary>
		readonly Point[] ptSudHidInv = new Point[] { new Point( 0, 0 ), new Point( 0, 2 ), new Point( 0, 1 ), new Point( 0, 3 ), new Point( 4, 7 ), new Point( 5, 7 ) };
		readonly Point[] ptLight = new Point[] { new Point(5, 4), new Point(5, 5) };
		readonly Point[] ptRandom = new Point[] { new Point(3, 4), new Point(3, 5), new Point(3, 6), new Point(3, 7) };
		readonly Point[] ptReverse = new Point[] { new Point(1, 2), new Point(1, 3) };
		readonly Point[] ptTight = new Point[] { new Point(4, 4), new Point(4, 5) };
		readonly Point[] ptScrollSpeed = new Point[]
		{
			new Point(2, 0), new Point(2, 1), new Point(2, 2), new Point(2, 3),
			new Point(3, 0), new Point(3, 1), new Point(3, 2), new Point(3, 3),
			new Point(4, 0), new Point(4, 1), new Point(4, 2), new Point(4, 3),
			new Point(5, 0), new Point(5, 1), new Point(5, 2), new Point(5, 3)
		};

		private CTexture txオプションパネル;

		public CActオプションパネル(EOptionPanelDirection dir)
		{
			direction = dir;
			rect = new Rectangle();
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txオプションパネル);
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.txオプションパネル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\Screen option panels.png"), false);
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref txオプションパネル);
				base.OnManagedリソースの解放();
			}
		}

		private void IncrementXY(ref int x, ref int y)
		{
			if (direction == EOptionPanelDirection.Horizontal)
			{
				x += rect.Width;
			}
			else if (direction == EOptionPanelDirection.Vertical)
			{
				y += rect.Height;
			}
		}

		private void DrawOption(int idx, Point[] pts, int x, int y)
		{
			if (idx >= pts.Length)
			{
				idx = pts.Length - 1;
			}
			if (idx < 0)
			{
				idx = 0;
			}
			Point p = pts[idx];
			rect.X = p.X * rect.Width;
			rect.Y = p.Y * rect.Height;
			txオプションパネル.t2D描画(CDTXMania.Instance.Device, x, y, rect);
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (this.txオプションパネル != null)
				{

					for (EPart part = EPart.Drums; part <= EPart.Bass; ++part)
					{
						if ( CDTXMania.Instance.ConfigIni.b楽器有効(part) &&
							(
							(CDTXMania.Instance.r現在のステージ == CDTXMania.Instance.stage演奏画面 && 
							 CDTXMania.Instance.DTX != null && CDTXMania.Instance.DTX.bチップがある[part]) ||
							 CDTXMania.Instance.r現在のステージ != CDTXMania.Instance.stage演奏画面
							)
							)
						{
							int x = Pos[part].X;
							int y = Pos[part].Y;

							rect.Width = CDTXMania.Instance.Coordinates.ImgOptionPanel.W;
							rect.Height = CDTXMania.Instance.Coordinates.ImgOptionPanel.H;

							// Dark
							DrawOption((int)CDTXMania.Instance.ConfigIni.eDark.Value, ptDark, x, y);
							IncrementXY(ref x, ref y);

							// ScrollSpeed
							DrawOption(CDTXMania.Instance.ConfigIni.nScrollSpeed[part] - 1, ptScrollSpeed, x, y);
							IncrementXY(ref x, ref y);

							// Sud Hid Inv
							DrawOption((int)CDTXMania.Instance.ConfigIni.eSudHidInv[part].Value, ptSudHidInv, x, y);
							IncrementXY(ref x, ref y);

							// Reverse
							DrawOption(CDTXMania.Instance.ConfigIni.bReverse[part] ? 1 : 0, ptReverse, x, y);
							IncrementXY(ref x, ref y);

							if (part == EPart.Drums)
							{
								// Tight
								DrawOption(CDTXMania.Instance.ConfigIni.bTight ? 1 : 0, ptTight, x, y);
								IncrementXY(ref x, ref y);
							}

							if (part == EPart.Guitar || part == EPart.Bass)
							{
								// Random
								DrawOption((int)CDTXMania.Instance.ConfigIni.eRandom[part].Value, ptRandom, x, y);
								IncrementXY(ref x, ref y);

								// Light
								DrawOption(CDTXMania.Instance.ConfigIni.bLight[part].Value ? 1 : 0, ptLight, x, y);
								IncrementXY(ref x, ref y);
							}
						}
					}
				}
			}
			return 0;
		}
	}
}
