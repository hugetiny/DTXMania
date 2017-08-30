using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using FDK;

namespace DTXMania
{
	internal class CActSelectInformation : CActivity
	{
		// コンストラクタ

		public CActSelectInformation()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n画像Index上 = -1;
			this.n画像Index下 = 0;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ctスクロール用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				//Trace.TraceInformation("Information用 Pad画像生成 開始。");

				int fontsize = 19;

				#region [ information画像パーツの作成 ]
				#region [ Informationに表示する文字列の取得 ]
				string[] strInfo = new string[]
				{
					CDTXMania.Instance.Resources.Explanation("strSelectInfo01"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo02"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo03"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo04"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo05"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo06"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo07"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo08"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo09"),
					CDTXMania.Instance.Resources.Explanation("strSelectInfo10"),
				};
				#endregion
				#region [パッド画像の準備]
				STPadValue<Rectangle> RectDrPad = new STPadValue<Rectangle>();
				RectDrPad.LC = new Rectangle(  0,   0, 170, 130);
				RectDrPad.HH = new Rectangle(170,   0, 170, 130);
				RectDrPad.SD = new Rectangle(340,   0, 170, 130);
				RectDrPad.BD = new Rectangle(  0, 130, 170, 130);
				RectDrPad.HT = new Rectangle(170, 130, 170, 130);
				RectDrPad.LT = new Rectangle(340, 130, 170, 130);
				RectDrPad.FT = new Rectangle(  0, 260, 170, 130);
				RectDrPad.CY = new Rectangle(170, 260, 170, 130);
				RectDrPad.RD = new Rectangle(340, 260, 170, 130);

				STPadValue<Bitmap> ImgDrPad = new STPadValue<Bitmap>();
				STPadValue<Bitmap> ImgGtPad = new STPadValue<Bitmap>();
				Bitmap ImgAllDrPads = new Bitmap(CSkin.Path(@"Graphics\ScreenPlayDrums pads.png"));
				Bitmap ImgAllGtPads = new Bitmap(CSkin.Path(@"Graphics\ScreenPlayGuitar pads.png"));
				Graphics g;
				Rectangle desRect = new Rectangle(0, 0, 170, 130);
				float newHeight, imgScale;
				Bitmap b;

				#region [LC]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.LC, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.LC = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.LC);
				g.DrawImage(b, 0, 0, ImgDrPad.LC.Width, ImgDrPad.LC.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [HH]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.HH, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.HH = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.HH);
				g.DrawImage(b, 0, 0, ImgDrPad.HH.Width, ImgDrPad.HH.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [SD]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.SD, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.SD = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.SD);
				g.DrawImage(b, 0, 0, ImgDrPad.SD.Width, ImgDrPad.SD.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [BD]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.BD, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.BD = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.BD);
				g.DrawImage(b, 0, 0, ImgDrPad.BD.Width, ImgDrPad.BD.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [HT]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.HT, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.HT = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.HT);
				g.DrawImage(b, 0, 0, ImgDrPad.HT.Width, ImgDrPad.HT.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [LT]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.LT, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.LT = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.LT);
				g.DrawImage(b, 0, 0, ImgDrPad.LT.Width, ImgDrPad.LT.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [FT]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.FT, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.FT = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.FT);
				g.DrawImage(b, 0, 0, ImgDrPad.FT.Width, ImgDrPad.FT.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [CY]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.CY, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.CY = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.CY);
				g.DrawImage(b, 0, 0, ImgDrPad.CY.Width, ImgDrPad.CY.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [RD]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllDrPads, desRect, RectDrPad.RD, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgDrPad.RD = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgDrPad.RD);
				g.DrawImage(b, 0, 0, ImgDrPad.RD.Width, ImgDrPad.RD.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [GtR]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.LC, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.LC = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.LC);
				g.DrawImage(b, 0, 0, ImgGtPad.LC.Width, ImgGtPad.LC.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [GtG]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.HH, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.HH = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.HH);
				g.DrawImage(b, 0, 0, ImgGtPad.HH.Width, ImgGtPad.HH.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [GtB]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.SD, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.SD = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.SD);
				g.DrawImage(b, 0, 0, ImgGtPad.SD.Width, ImgGtPad.SD.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [GtPick]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.LT, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.LT = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.LT);
				g.DrawImage(b, 0, 0, ImgGtPad.LT.Width, ImgGtPad.LT.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [START]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.FT, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.FT = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.FT);
				g.DrawImage(b, 0, 0, ImgGtPad.FT.Width, ImgGtPad.FT.Height);
				g.Dispose();
				b.Dispose();
				#endregion
				#region [CANCEL]
				b = new Bitmap(170, 130);
				g = Graphics.FromImage(b);
				g.DrawImage(ImgAllGtPads, desRect, RectDrPad.CY, GraphicsUnit.Pixel);
				b = tBmpTrim(b);
				newHeight = fontsize * g.DpiY / 72.0f;  // 1 inch = 72 points
				imgScale = newHeight / 130.0f * 2;
				ImgGtPad.CY = new Bitmap((int)(Math.Ceiling(b.Width * imgScale)), (int)(Math.Ceiling(b.Height * imgScale)));
				g.Dispose();
				g = Graphics.FromImage(ImgGtPad.CY);
				g.DrawImage(b, 0, 0, ImgGtPad.CY.Width, ImgGtPad.CY.Height);
				g.Dispose();
				b.Dispose();
				#endregion

				g = null;
				//Trace.TraceInformation("Information用 Pad画像生成 終了。");
				#endregion

				string fontPath = CDTXMania.Instance.Resources.Explanation("strCfgSelectMusicInformationFontFileName");
				var prvFont = new CPrivateFastFont(CSkin.Path(Path.Combine(@"Graphics\fonts\", fontPath)), fontsize);

				for (int j = 0; j < 2; j++)
				{
					var bmp = new Bitmap(512, 512);
					g = Graphics.FromImage(bmp);
					int y;
					for (int i = 0; i < 5; i++)
					{
						y = (512 / 5) * i;
						var strComments = strInfo[i + j * 5].Split(new string[] { "\n" }, StringSplitOptions.None);

						var r = new Regex(@"(?<PAD>\[.+?])", RegexOptions.IgnoreCase);

						foreach (var s in strComments)
						{
							int x = 0;
							List<string> sss = new List<string>();
							string ss = s.Trim();

							#region [PAD画像部と説明文字列部を分けてListに格納]
							Match m = r.Match(ss);
							while (m.Success)
							{
								int padStrPos = m.Groups["PAD"].Index;
								string padName = m.Groups["PAD"].Value;

								if (padStrPos <= 0)
								{
									sss.Add(padName);
									ss = ss.Substring(padStrPos + padName.Length);
								}
								else
								{
									sss.Add(ss.Substring(0, padStrPos));
									ss = ss.Substring(padStrPos + padName.Length);
									sss.Add(padName);
								}
								m = r.Match(ss);
							}
							sss.Add(ss);
							#endregion

							foreach (string ls in sss)
							{
								string l = ls;      // foreach割り当て変数の書き換えはできないので

								#region [PAD画像描画]
								if ( (l != "" )&& (l[0] == '[') )	// PAD処理
								{
									switch(ls)
									{
										case "[LC]":
											l = "";	g.DrawImage(ImgDrPad.LC, x, y);	x += ImgDrPad.LC.Width;
											break;
										case "[HH]":
											l = "";	g.DrawImage(ImgDrPad.HH, x, y);	x += ImgDrPad.HH.Width;
											break;
										case "[SD]":
											l = "";	g.DrawImage(ImgDrPad.SD, x, y);	x += ImgDrPad.SD.Width;
											break;
										case "[BD]":
											l = "";	g.DrawImage(ImgDrPad.BD, x, y);	x += ImgDrPad.BD.Width;
											break;
										case "[HT]":
											l = ""; g.DrawImage(ImgDrPad.HT, x, y); x += ImgDrPad.HT.Width;
											break;
										case "[LT]":
											l = ""; g.DrawImage(ImgDrPad.LT, x, y); x += ImgDrPad.LT.Width;
											break;
										case "[FT]":
											l = ""; g.DrawImage(ImgDrPad.FT, x, y); x += ImgDrPad.FT.Width;
											break;
										case "[CY]":
											l = ""; g.DrawImage(ImgDrPad.CY, x, y); x += ImgDrPad.CY.Width;
											break;
										case "[RD]":
											l = ""; g.DrawImage(ImgDrPad.RD, x, y); x += ImgDrPad.RD.Width;
											break;
										case "[R]":
											l = ""; g.DrawImage(ImgGtPad.LC, x, y); x += ImgGtPad.LC.Width;
											break;
										case "[G]":
											l = ""; g.DrawImage(ImgGtPad.HH, x, y); x += ImgGtPad.HH.Width;
											break;
										case "[B]":
											l = ""; g.DrawImage(ImgGtPad.SD, x, y); x += ImgGtPad.SD.Width;
											break;
										case "[P]":
											l = ""; g.DrawImage(ImgGtPad.LT, x, y); x += ImgGtPad.LT.Width;
											break;
										case "[START]":
											l = ""; g.DrawImage(ImgGtPad.FT, x, y); x += ImgGtPad.FT.Width;
											break;
										case "[CANCEL]":
											l = ""; g.DrawImage(ImgGtPad.CY, x, y); x += ImgGtPad.CY.Width;
											break;
										default:
											break;
									}
								}
								#endregion
								#region [文字列描画]
								if (l != "")		// 通常文字列、もしくはパッド名じゃない[...]で囲まれた文字列
								{
									var bb = prvFont.DrawPrivateFont(ls, Color.White, Color.FromArgb(0, 0, 0));
									g.DrawImage(bb, x, y);
									x += prvFont.RectStrings.Width;
									bb.Dispose();
								}
								#endregion
							}
							y += (int)(prvFont.RectStrings.Height * 1.2f);
						}
					}
					this.txInfo[j] = TextureFactory.tテクスチャの生成(bmp, false);

					//bmp.Save("tmptmp" + j + ".png");
					g.Dispose();
					g = null;
					bmp.Dispose();
					bmp = null;
				}
				prvFont.Dispose();
				prvFont = null;

				#region [ImgDrPad開放]
				ImgDrPad.LC.Dispose();	ImgDrPad.LC = null;
				ImgDrPad.HH.Dispose();	ImgDrPad.HH = null;
				ImgDrPad.SD.Dispose();	ImgDrPad.SD = null;
				ImgDrPad.BD.Dispose();	ImgDrPad.BD = null;
				ImgDrPad.HT.Dispose();	ImgDrPad.HT = null;
				ImgDrPad.LT.Dispose();	ImgDrPad.LT = null;
				ImgDrPad.FT.Dispose();	ImgDrPad.FT = null;
				ImgDrPad.CY.Dispose();	ImgDrPad.CY = null;
				ImgDrPad.RD.Dispose();	ImgDrPad.RD = null;
				ImgDrPad = null;
				#endregion

				#endregion

				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txInfo[0]);
				TextureFactory.tテクスチャの解放(ref this.txInfo[1]);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (base.b初めての進行描画)
				{
					this.ctスクロール用 = new CCounter(0, 6000, 1, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				this.ctスクロール用.t進行();
				if (this.ctスクロール用.b終了値に達した)
				{
					this.n画像Index上 = this.n画像Index下;
					this.n画像Index下 = (this.n画像Index下 + 1) % stInfo.GetLength(0);    //8;
					this.ctスクロール用.n現在の値 = 0;
				}
				int n現在の値 = this.ctスクロール用.n現在の値;
				if (n現在の値 <= 250)
				{
					double n現在の割合 = ((double)n現在の値) / 250.0;
					if (this.n画像Index上 >= 0)
					{
						STINFO stinfo = this.stInfo[this.n画像Index上];
						Rectangle rectangle = new Rectangle(
							stinfo.pt左上座標.X,
							stinfo.pt左上座標.Y + ((int)((int)(512.0 / 5) * n現在の割合)),
							512,
							Convert.ToInt32((int)(512.0 / 5 * (1.0 - n現在の割合)))
						);
						if (this.txInfo[stinfo.nTexture番号] != null)
						{
							this.txInfo[stinfo.nTexture番号].t2D描画(
								CDTXMania.Instance.Device,
								115 * Scale.X,
								6 * Scale.Y,
								rectangle
							);
						}
					}
					if (this.n画像Index下 >= 0)
					{
						STINFO stinfo = this.stInfo[this.n画像Index下];
						Rectangle rectangle = new Rectangle(
							stinfo.pt左上座標.X,
							stinfo.pt左上座標.Y,
							512,
							(int)(512.0 / 5 * n現在の割合)
						);
						if (this.txInfo[stinfo.nTexture番号] != null)
						{
							this.txInfo[stinfo.nTexture番号].t2D描画(
								CDTXMania.Instance.Device,
								115 * Scale.X,
								6 * Scale.Y + ((int)(512.0 / 5 * (1.0 - n現在の割合))),
								rectangle
							);
						}
					}
				}
				else
				{
					STINFO stinfo = this.stInfo[this.n画像Index下];
					Rectangle rectangle = new Rectangle(
						stinfo.pt左上座標.X,
						stinfo.pt左上座標.Y,
						512,
						(int)(512.0 / 5)
					);
					if (this.txInfo[stinfo.nTexture番号] != null)
					{
						this.txInfo[stinfo.nTexture番号].t2D描画(
							CDTXMania.Instance.Device,
							115 * Scale.X,
							6 * Scale.Y,
							rectangle
						);
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout(LayoutKind.Sequential)]
		private struct STINFO
		{
			public int nTexture番号;
			public Point pt左上座標;
			public STINFO(int nTexture番号, int x, int y)
			{
				this.nTexture番号 = nTexture番号;
				this.pt左上座標 = new Point(x, y);
			}
		}

		private CCounter ctスクロール用;
		private int n画像Index下;
		private int n画像Index上;
		private readonly STINFO[] stInfo = new STINFO[] {
			new STINFO( 0, 0, 0 ),
			new STINFO( 0, 0, (int)(512.0 / 5 * 1)),
			new STINFO( 0, 0, (int)(512.0 / 5 * 2)),
			new STINFO( 0, 0, (int)(512.0 / 5 * 3)),
			new STINFO( 0, 0, (int)(512.0 / 5 * 4)),
			new STINFO( 1, 0, (int)(512.0 / 5 * 0)),
			new STINFO( 1, 0, (int)(512.0 / 5 * 1)),
			new STINFO( 1, 0, (int)(512.0 / 5 * 2)),
			new STINFO( 1, 0, (int)(512.0 / 5 * 3))
		};
		private CTexture[] txInfo = new CTexture[2];



		/// <summary>
		/// 画像の周囲から画素のない部分を削除しトリミングする
		/// (とりあえずGetPixelを使った低速バージョン)
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private Bitmap tBmpTrim(Bitmap src)
		{
			int x0 = 0, y0 = 0, x1 = src.Width - 1, y1 = src.Height - 1;
			int alpha_threshold = 64;

			#region [ 左辺のトリミング(x0を求める) ]
			for (int px = 0; px < src.Width; px++)
			{
				for (int py = 0; py < src.Height; py++)
				{
					Color c = src.GetPixel(px, py);
					if (c.A > alpha_threshold)
					{
						x0 = px;
						px = src.Width;    // for break;
						break;
					}
				}
			}
			#endregion
			#region [ 上辺のトリミング(y0を求める) ]
			for (int py = 0; py < src.Height; py++)
			{
				for (int px = 0; px < src.Width; px++)
				{
					Color c = src.GetPixel(px, py);
					if (c.A > alpha_threshold)
					//if (!c.Equals(Color.FromArgb(0, 0, 0, 0)))
					{
						y0 = py;
						py = src.Height;    // for break;
						break;
					}
				}
			}
			#endregion
			#region [ 右辺のトリミング(x1を求める) ]
			for (int px = src.Width - 1; px >= 0; px--)
			{
				for (int py = src.Height - 1; py >= 0; py--)
				{
					Color c = src.GetPixel(px, py);
					if (c.A > alpha_threshold)
					{
						x1 = px;
						px = -1;    // for break;
						break;
					}
				}
			}
			#endregion
			#region [ 右辺のトリミング(y1を求める) ]
			for (int py = src.Height- 1; py >= 0; py--)
			{
				for (int px = src.Width - 1; px >= 0; px--)
				{
					Color c = src.GetPixel(px, py);
					if (c.A > alpha_threshold)
					{
						y1 = py;
						py = -1;    // for break;
						break;
					}
				}
			}
			#endregion

			#region [ Trim後のbmpを生成 ]
			int w = x1 - x0 + 1;
			int h = y1 - y0 + 1;
			Bitmap bmp = new Bitmap(w, h);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(src, 0, 0, new Rectangle(x0, y0, w, h), GraphicsUnit.Pixel);
				//(src, new Rectangle(x0, y0, w, h));

			g.Dispose();
			g = null;
			#endregion

			return bmp;
		}
		//-----------------
		#endregion
	}
}
