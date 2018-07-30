using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using SharpDX;
using FDK;

using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActShowSongPath: CActivity
	{
		// メソッド

		public CActShowSongPath()
		{
			base.b活性化してない = true;
		}

		public void t選択曲が変更された()
		{
			Cスコア cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;
			C曲リストノード c曲リストノード = CDTXMania.Instance.stage選曲.r現在選択中の曲;
			if (cスコア != null)
			{
				Bitmap image = new Bitmap(1, 1);
				#region [ songpathテクスチャ生成 ]
				TextureFactory.tテクスチャの解放(ref this.txSongPath);
				this.strSongPath = "dtx    : " + cスコア.ファイル情報.ファイルの絶対パス + Environment.NewLine +
					"set.def: " + c曲リストノード.pathSetDefの絶対パス;
				if ((this.strSongPath != null) && (this.strSongPath.Length > 0))
				{
					Graphics graphics = Graphics.FromImage(image);
					graphics.PageUnit = GraphicsUnit.Pixel;
					StringFormat sf = new StringFormat();
					sf.FormatFlags = StringFormatFlags.LineLimit;
					SizeF ef = graphics.MeasureString(this.strSongPath, this.ftSongPath描画用フォント, SampleFramework.GameWindowSize.Width, sf);
					graphics.Dispose();
					//if (ef.Width > SampleFramework.GameWindowSize.Width)
					//{
					//	ef.Width = SampleFramework.GameWindowSize.Width;
					//	ef.Height *= 3;
					//}
					try
					{
						Bitmap bitmap2 = new Bitmap((int)Math.Ceiling((double)ef.Width), (int)Math.Ceiling((double)ef.Height));
						graphics = Graphics.FromImage(bitmap2);
						graphics.Clear(System.Drawing.Color.FromArgb(192, 0, 0, 0));
						graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
						graphics.DrawString(this.strSongPath, this.ftSongPath描画用フォント, Brushes.White,
							new System.Drawing.RectangleF(0, 0, ef.Width, ef.Height),
							sf
						);
						sf.Dispose();
						graphics.Dispose();
						this.txSongPath = new CTexture(CDTXMania.Instance.Device, bitmap2, CDTXMania.Instance.TextureFormat);
						bitmap2.Dispose();
					}
					catch (CTextureCreateFailedException)
					{
						Trace.TraceError("SongPathテクスチャの生成に失敗しました。");
						this.txSongPath = null;
					}
				}
				#endregion
				image.Dispose();
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			string fontname = CDTXMania.Instance.Resources.Explanation("strCfgSelectMusicSongCommentFontFileName");
			string path = Path.Combine(@"Graphics\fonts", fontname);
			this.pfSongPath描画用フォント = new CPrivateFastFont( CSkin.Path(path), (float)(12f * Scale.Y * 72f / 96f) );
			// 72f/96f: 従来互換のために追加。DPI依存→非依存化に付随した変更。
			this.ftSongPath描画用フォント = this.pfSongPath描画用フォント.font;

			this.txSongPath = null;
			this.strSongPath = "";
			this.nテクスチャの最大幅 = 0;

			base.On活性化();
		}
		public override void On非活性化()
		{
			TextureFactory.tテクスチャの解放(ref this.txSongPath);
			if (this.pfSongPath描画用フォント != null)
			{
				this.pfSongPath描画用フォント.Dispose();
				this.pfSongPath描画用フォント = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.t選択曲が変更された();
				base.OnManagedリソースの作成();

//				ScreenSelect bar score selected.ong
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txSongPath);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない && CDTXMania.Instance.ConfigIni.bShowSongPath)
			{
				if (this.txSongPath != null)
				{
					int x = 0;
					int y = 900;
					this.txSongPath.t2D描画(CDTXMania.Instance.Device, x, y);
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private Font ftSongPath描画用フォント;
		private CPrivateFastFont pfSongPath描画用フォント;
		private const int nSongPath表示幅 = (int)(0x182 * Scale.X);
		private int nテクスチャの最大幅;
		private string strSongPath;
		private CTexture txSongPath;
		//-----------------
		#endregion
	}
}
