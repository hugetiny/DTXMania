using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using FDK;

using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActResultImage : CActivity
	{
		CDTX.CAVI rAVI;
		CCounter ct登場用;
		int n本体X;
		int n本体Y;
		CTexture r表示するリザルト画像;
		string strAVIファイル名;
		CTexture txパネル本体;
		CTexture txリザルト画像;
		CTexture txリザルト画像がないときの画像;

		public void tアニメを完了させる()
		{
			ct登場用.n現在の値 = ct登場用.n終了値;
		}
		public CAct演奏AVI actAVI
		{
			get;
			set;
		}

		public override void On活性化()
		{
			if (b活性化してない)
			{
				n本体X = 4;
				n本体Y = 0x3f;
				base.On活性化();
				actAVI.bIsPreviewMovie = true;
				actAVI.On活性化();
			}
		}
		public override void On非活性化()
		{
			if (b活性化してる)
			{
				if (ct登場用 != null)
				{
					ct登場用 = null;
				}
				if (rAVI != null)
				{
					rAVI.Dispose();
					rAVI = null;
				}
				base.On非活性化();
				actAVI.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				txパネル本体 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult resultimage panel.png"));
				txリザルト画像がないときの画像 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect preimage default.png"));
				base.OnManagedリソースの作成();
				actAVI.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref txパネル本体);
				TextureFactory.tテクスチャの解放(ref txリザルト画像);
				TextureFactory.tテクスチャの解放(ref txリザルト画像がないときの画像);
				base.OnManagedリソースの解放();
				actAVI.OnManagedリソースの解放();
			}
		}
		public override unsafe int On進行描画()
		{
			if (b活性化してる)
			{
				if (b初めての進行描画)
				{
					if (CDTXMania.Instance.ConfigIni.bStoicMode)
					{
						r表示するリザルト画像 = txリザルト画像がないときの画像;
					}
					else if (!tリザルト動画の指定があれば構築する() &&
						!tリザルト画像の指定があれば構築する() &&
						!tプレビュー動画の指定があれば構築する() &&
						!tプレビュー画像の指定があれば構築する() &&
						!t背景画像があればその一部からリザルト画像を構築する())
					{
						r表示するリザルト画像 = txリザルト画像がないときの画像;
					}

					ct登場用 = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
					b初めての進行描画 = false;
				}
				ct登場用.t進行();
				if (ct登場用.b終了値に達した)
				{
					n本体X = 4;
					n本体Y = 0x3f;
				}
				else
				{
					double num3 = ((double)ct登場用.n現在の値) / 100.0;
					double num4 = Math.Cos((1.5 + (0.5 * num3)) * Math.PI);
					n本体X = 4;
					n本体Y = 0x3f - ((int)(((txパネル本体 != null) ?
						((double)txパネル本体.sz画像サイズ.Height) : ((double)0)) * (1.0 - (num4 * num4))));
				}
				if (txパネル本体 != null)
				{
					txパネル本体.t2D描画(
						CDTXMania.Instance.Device,
						n本体X * Scale.X,
						n本体Y * Scale.Y
					);
				}
				int x = (int)((n本体X + 17) * Scale.X);
				int y = (int)((n本体Y + 16) * Scale.Y);
				if (rAVI != null)
				{
					actAVI.t進行描画(x, y, 612, 605);
				}
				#region [ プレビュー画像表示 ]
				else if (r表示するリザルト画像 != null)
				{
					CPreviewMagnifier cmg = new CPreviewMagnifier(CPreviewMagnifier.EPreviewType.MusicSelect);
					cmg.GetMagnifier(r表示するリザルト画像.sz画像サイズ.Width, r表示するリザルト画像.sz画像サイズ.Height, 1.0f, 1.0f);

					r表示するリザルト画像.vc拡大縮小倍率.X = cmg.magX;
					r表示するリザルト画像.vc拡大縮小倍率.Y = cmg.magY;
					r表示するリザルト画像.vc拡大縮小倍率.Z = 1f;
					x += (int)((612 - cmg.width * cmg.magX) / 2);
					y += (int)((605 - cmg.height * cmg.magY) / 2);
					r表示するリザルト画像.t2D描画(CDTXMania.Instance.Device, x, y);


				}
				#endregion
				if ((CDTXMania.Instance.DTX.GENRE != null) && (CDTXMania.Instance.DTX.GENRE.Length > 0))
				{
					CDTXMania.Instance.act文字コンソール.tPrint(
						(int)((n本体X + 0x12) * Scale.X),
						(int)((n本体Y - 1) * Scale.Y),
						C文字コンソール.Eフォント種別.赤細,
						CDTXMania.Instance.DTX.GENRE
					);
				}
				if (!ct登場用.b終了値に達した)
				{
					return 0;
				}
				return 1;
			}
			return 0;
		}

		private bool t背景画像があればその一部からリザルト画像を構築する()
		{
			string strBackground = CDTXMania.Instance.DTX.BACKGROUND;
			if (string.IsNullOrEmpty(strBackground))
			{
				strBackground = CDTXMania.Instance.DTX.BACKGROUND_GR;
			}
			if (string.IsNullOrEmpty(strBackground))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref txリザルト画像);
			r表示するリザルト画像 = null;
			strBackground = CDTXMania.Instance.DTX.strフォルダ名 + strBackground;
			if (!File.Exists(strBackground))
			{
				return false;
			}
			using (Bitmap image = new Bitmap(strBackground))
			{
				using (Bitmap bitmap2 = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap2))
					{
						int x = 0;
						for (int i = 0; i < SampleFramework.GameWindowSize.Height; i += image.Height)
						{
							for (x = 0; x < SampleFramework.GameWindowSize.Width; x += image.Width)
							{
								graphics.DrawImage(image, x, i, image.Width, image.Height);
							}
						}
					}
					using (Bitmap bitmap3 = new Bitmap(0xcc, 0x10d))
					{
						using (Graphics graphics = Graphics.FromImage(bitmap3))
						{
							graphics.DrawImage(bitmap2, 5, 5, new Rectangle(0x157, 0x6d, 0xcc, 0x10d), GraphicsUnit.Pixel);
						}
						txリザルト画像 = new CTexture(CDTXMania.Instance.Device, bitmap3, CDTXMania.Instance.TextureFormat);
					}
				}
				r表示するリザルト画像 = txリザルト画像;
			}
			return true;
		}

		private bool tプレビュー画像の指定があれば構築する()
		{
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.PREIMAGE))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref txリザルト画像);
			r表示するリザルト画像 = null;
			string path = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.PREIMAGE;
			if (!File.Exists(path))
			{
				Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
				return false;
			}
			txリザルト画像 = TextureFactory.tテクスチャの生成(path);
			r表示するリザルト画像 = txリザルト画像;
			return (r表示するリザルト画像 != null);
		}

		private bool tプレビュー動画の指定があれば構築する()
		{
			if (!CDTXMania.Instance.ConfigIni.bAVI)
			{
				return false;
			}
			actAVI.Stop();
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.PREMOVIE))
			{
				return false;
			}
			strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.PREMOVIE;
			if (!File.Exists(this.strAVIファイル名))
			{
				Trace.TraceWarning("プレビュー動画のファイルが存在しません。({0})", strAVIファイル名);
				return false;
			}
			if (rAVI != null)
			{
				rAVI.Dispose();
				rAVI = null;
			}
			try
			{
				rAVI = new CDTX.CAVI(00, this.strAVIファイル名, "", CDTXMania.Instance.ConfigIni.nPlaySpeed);
				rAVI.OnDeviceCreated();
				actAVI.Start(EChannel.Movie, rAVI, 204, 269, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1);
			}
			catch
			{
				Trace.TraceError("プレビュー動画の生成に失敗しました。({0})", strAVIファイル名);
				rAVI = null;
			}
			return true;
		}

		private bool tリザルト画像の指定があれば構築する()
		{
			CScoreIni.ERANK rank = CScoreIni.t総合ランク値を計算して返す(CDTXMania.Instance.stage結果.st演奏記録);
			if (rank == CScoreIni.ERANK.UNKNOWN)  // #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
			{
				rank = CScoreIni.ERANK.E;
			}
			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.RESULTIMAGE[(int)rank]))
			{
				return false;
			}
			TextureFactory.tテクスチャの解放(ref txリザルト画像);
			r表示するリザルト画像 = null;
			string path = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.RESULTIMAGE[(int)rank];
			if (!File.Exists(path))
			{
				Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
				return false;
			}
			txリザルト画像 = TextureFactory.tテクスチャの生成(path);
			r表示するリザルト画像 = this.txリザルト画像;
			return (this.r表示するリザルト画像 != null);
		}

		private bool tリザルト動画の指定があれば構築する()
		{
			if (!CDTXMania.Instance.ConfigIni.bAVI)
			{
				return false;
			}
			CScoreIni.ERANK rank = CScoreIni.t総合ランク値を計算して返す(CDTXMania.Instance.stage結果.st演奏記録);
			// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
			if (rank == CScoreIni.ERANK.UNKNOWN)
			{
				rank = CScoreIni.ERANK.E;
			}

			if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.RESULTMOVIE[(int)rank]))
			{
				return false;
			}
			strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.RESULTMOVIE[(int)rank];
			if (!File.Exists(this.strAVIファイル名))
			{
				Trace.TraceWarning("リザルト動画のファイルが存在しません。({0})", this.strAVIファイル名);
				return false;
			}
			if (rAVI != null)
			{
				rAVI.Dispose();
				rAVI = null;
			}
			try
			{
				rAVI = new CDTX.CAVI(00, this.strAVIファイル名, "", CDTXMania.Instance.ConfigIni.nPlaySpeed);
				rAVI.OnDeviceCreated();
				actAVI.Start(EChannel.Movie, rAVI, 204, 269, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1);
			}
			catch
			{
				Trace.TraceError("リザルト動画の生成に失敗しました。({0})", this.strAVIファイル名);
				rAVI = null;
			}
			return true;
		}
	}
}
