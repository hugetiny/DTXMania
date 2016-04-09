using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FDK;
using System.Drawing;
using SlimDX;

namespace DTXMania
{
	public class CActDigit : CActivity
	{
		Rectangle[] digitRect = new Rectangle[12];
		CTexture[] texDigit = new CTexture[12];
		Color colorInside;
		Color colorEdge;
		Color colorGradTop;
		Color colorGradBottom;
		int pt;
		bool bEdge;
		bool bGrad;

		public int MaximumHeight { get; private set; }

		public CActDigit(Color color, int pt)
		{
			colorInside = color;
			this.pt = pt;
		}

		public CActDigit(Color colorInside, Color colorEdge, int pt)
		{
			this.colorInside = colorInside;
			this.colorEdge = colorEdge;
			this.pt = pt;
			bEdge = true;
		}

		public CActDigit(Color colorInside, Color colorEdge, Color gradTop, Color gradBottom, int pt)
		{
			this.colorInside = colorInside;
			this.colorEdge = colorEdge;
			this.colorGradTop = gradTop;
			this.colorGradBottom = gradBottom;
			this.pt = pt;
			bEdge = true;
			bGrad = true;
		}

		public override void OnManagedリソースの作成()
		{
			string dg = "0123456789";

			if (b活性化してる)
			{
				for (int i = 0; i < texDigit.Length; ++i)
				{
					TextureFactory.tテクスチャの解放(ref texDigit[i]);
				}

				using (FontFamily ff = new FontFamily("MS PGothic"))
				{
					using (CPrivateFont cpf = new CPrivateFont(ff, pt, FontStyle.Bold))
					{
						for (int i = 0; i < 12; ++i)
						{
							string sub = ".";
							if (i < 10)
							{
								sub = dg.Substring(i, 1);
							}
							else
							{
								sub = "-";
							}

							Bitmap x = null;
							if (bGrad)
							{
								x = cpf.DrawPrivateFont(sub, colorInside, colorEdge, colorGradTop, colorGradBottom);
							}
							else if (bEdge)
							{
								x = cpf.DrawPrivateFont(sub, colorInside, colorEdge);
							}
							else
							{
								x = cpf.DrawPrivateFont(sub, colorInside);
							}

							using (Bitmap bmp = x)
							{
								texDigit[i] = TextureFactory.tテクスチャの生成(bmp);
								digitRect[i] = MeasureRectangle(bmp);
								MaximumHeight = Math.Max(MaximumHeight, digitRect[i].Height);
							}
						}
					}
				}
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				for (int i = 0; i < texDigit.Length; ++i)
				{
					TextureFactory.tテクスチャの解放(ref texDigit[i]);
				}
				base.OnManagedリソースの解放();
			}
		}

		/// <summary>
		/// bmp 内の不透明ピクセルからなる最大の範囲（幅）を得る。
		/// CPrivateFont で文字描画前の bmp は透明になっているから透明でないピクセルをなめて実際の幅を得る。
		/// (重いので描画処理中には呼ばないこと)
		/// </summary>
		/// <param name="bmp">計算対象ビットマップ。</param>
		/// <returns>有効なピクセルを囲う最小の矩形情報</returns>
		private Rectangle MeasureRectangle(Bitmap bmp)
		{
			int xmin = bmp.Width;
			int xmax = -1;
			int ymin = bmp.Height;
			int ymax = -1;
			for (int x = 0; x < bmp.Width; ++x)
			{
				for (int y = 0; y < bmp.Height; ++y)
				{
					if (bmp.GetPixel(x, y).A != 0)
					{
						xmin = Math.Min(xmin, x);
						xmax = Math.Max(xmax, x);
						ymin = Math.Min(ymin, y);
						ymax = Math.Max(ymax, y);
					}
				}
			}
			return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		public int Measure(int num)
		{
			return DrawSub((long)num, 0, 0, false);
		}

		public int Measure(long num)
		{
			return DrawSub(num, 0, 0, false);
		}

		private int numlen(int n)
		{
			int ret = 0;
			if (n == 0)
			{
				ret = 1;
			}
			while (n > 0)
			{
				++ret;
				n /= 10;
			}
			return ret;
		}

		public int Measure(double num, int prec)
		{
			int ret = 0;
			int integer = (int)num;
			ret += Measure(integer);
			// . を足す
			ret += digitRect[11].Width;

			int dec = (int)((num - integer) * Math.Pow(10, prec));
			for (int d = 0; d < prec - numlen(dec); ++d)
			{
				// 0.007 のような場合で 007 の 00 をカウントする
				ret += digitRect[0].Width;
			}
			ret += Measure(dec);

			return ret;
		}

		public void Draw(int num, int x, int y)
		{
			DrawSub((long)num, x, y);
		}

		public void Draw(long num, int x, int y)
		{
			DrawSub(num, x, y, true);
		}

		public void Draw(double num, int prec, int x, int y)
		{
			int integer = (int)num;

			// 整数部
			x += DrawSub(integer, x, y);

			// . を描画
			texDigit[11].t2D描画(CDTXMania.Instance.Device, x, y);
			x += digitRect[11].Width;

			// 小数部
			int dec = (int)((num - integer) * Math.Pow(10, prec));
			for (int d = 0; d < prec - numlen(dec); ++d)
			{
				x += DrawSub(0, x, y, true);
			}
			DrawSub((int)dec, x, y);
		}

		private int DrawSub(long num, int x = 0, int y = 0, bool draw = true)
		{
			int ret = 0;

			List<int> d = new List<int>();

			if (num < 0)
			{
				d.Add(10);
				num -= num;
			}

			if (num > 0)
			{
				while (num > 0)
				{
					d.Add((int)(num % 10));
					num /= 10;
				}
			}
			else if (num == 0)
			{
				d.Add(0);
			}
			d.Reverse();

			foreach (var t in d)
			{
				if (texDigit[t] != null)
				{
					if (draw)
					{
						texDigit[t].t2D描画(CDTXMania.Instance.Device, x, y, digitRect[t]);
						x += digitRect[t].Width;
					}
					ret += digitRect[t].Width;
				}
			}

			return ret;
		}

	}
}
