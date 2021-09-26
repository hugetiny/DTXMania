using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using SharpDX;
using FDK;

using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActSelectArtistComment : CActivity
	{
		// メソッド

		public CActSelectArtistComment()
		{
			base.b活性化してない = true;
		}
		public void t選択曲が変更された()
		{
			Cスコア cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;
			if (cスコア != null)
			{
				#region [ Artist ]
				//Bitmap image = new Bitmap(1, 1);
				TextureFactory.tテクスチャの解放(ref this.txArtist);
				this.strArtist = cスコア.譜面情報.アーティスト名;
				if ((this.strArtist != null) && (this.strArtist.Length > 0))
				{
					Bitmap imageStrArtist = this.ftArtist描画フォント.DrawPrivateFont(this.strArtist, Color.White);

					//Graphics graphics = Graphics.FromImage(image);
					//graphics.PageUnit = GraphicsUnit.Pixel;
					//SizeF ef = graphics.MeasureString(this.strArtist, this.ft描画用フォント);
					//graphics.Dispose();
					//if (imageStrArtist.Width > SampleFramework.GameWindowSize.Width)
					//{
					//	ef.Width = SampleFramework.GameWindowSize.Width;
					//}
					try
					{
						//Bitmap bitmap2 = new Bitmap((int)Math.Ceiling((double)ef.Width), (int)Math.Ceiling((double)this.ft描画用フォント.Size));
						//graphics = Graphics.FromImage(bitmap2);
						//graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
						//graphics.DrawString(this.strArtist, this.ft描画用フォント, Brushes.White, (float)0f, (float)0f);
						//graphics.Dispose();
						this.txArtist = new CTextureAf(CDTXMania.Instance.Device, imageStrArtist, CDTXMania.Instance.TextureFormat);
						//this.txArtist.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
						imageStrArtist.Dispose();
					}
					catch (CTextureCreateFailedException)
					{
						Trace.TraceError("ARTISTテクスチャの生成に失敗しました。");
						this.txArtist = null;
					}
				}
				#endregion

				#region [ Comment ]
				TextureFactory.tテクスチャの解放(ref this.txComment);
				this.strComment = cスコア.譜面情報.コメント;
				if ((this.strComment != null) && (this.strComment.Length > 0))
				{
					Bitmap imageStrComment = this.ftComment描画フォント.DrawPrivateFont(this.strComment, Color.White);

					//Graphics graphics2 = Graphics.FromImage(image);
					//graphics2.PageUnit = GraphicsUnit.Pixel;
					//SizeF ef2 = graphics2.MeasureString(this.strComment, this.ft描画用フォント);
					Size size = new Size(imageStrComment.Width, imageStrComment.Height);
					//graphics2.Dispose();
					this.nテクスチャの最大幅 = CDTXMania.Instance.Device.Capabilities.MaxTextureWidth;
					int maxTextureHeight = CDTXMania.Instance.Device.Capabilities.MaxTextureHeight;
					//Bitmap bitmap3 = new Bitmap(size.Width, (int)Math.Ceiling((double)this.ft描画用フォント.Size));
					//graphics2 = Graphics.FromImage(bitmap3);
					//graphics2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					//graphics2.DrawString(this.strComment, this.ft描画用フォント, Brushes.White, (float)0f, (float)0f);
					//graphics2.Dispose();
					this.nComment行数 = 1;
					this.nComment最終行の幅 = size.Width;
					while (this.nComment最終行の幅 > this.nテクスチャの最大幅)
					{
						this.nComment行数++;
						this.nComment最終行の幅 -= this.nテクスチャの最大幅;
					}
					while ((this.nComment行数 * ((int)Math.Ceiling((double)this.ftComment描画フォント.Size))) > maxTextureHeight)
					{
						this.nComment行数--;
						this.nComment最終行の幅 = this.nテクスチャの最大幅;
					}
					//Bitmap bitmap4 = new Bitmap((this.nComment行数 > 1) ? this.nテクスチャの最大幅 : this.nComment最終行の幅, this.nComment行数 * ((int)Math.Ceiling((double)this.ft描画用フォント.Size)));
					//graphics2 = Graphics.FromImage(bitmap4);
					//Rectangle srcRect = new Rectangle();
					//Rectangle destRect = new Rectangle();
					//for (int i = 0; i < this.nComment行数; i++)
					//{
					//	srcRect.X = i * this.nテクスチャの最大幅;
					//	srcRect.Y = 0;
					//	srcRect.Width = ((i + 1) == this.nComment行数) ? this.nComment最終行の幅 : this.nテクスチャの最大幅;
					//	srcRect.Height = bitmap3.Height;
					//	destRect.X = 0;
					//	destRect.Y = i * bitmap3.Height;
					//	destRect.Width = srcRect.Width;
					//	destRect.Height = srcRect.Height;
					//	graphics2.DrawImage(bitmap3, destRect, srcRect, GraphicsUnit.Pixel);
					//}
					//graphics2.Dispose();
					try
					{
						this.txComment = new CTextureAf(CDTXMania.Instance.Device, imageStrComment, CDTXMania.Instance.TextureFormat, _label:"Comment");
						//this.txComment.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
					}
					catch (CTextureCreateFailedException)
					{
						Trace.TraceError("COMMENTテクスチャの生成に失敗しました。");
						this.txComment = null;
					}
					imageStrComment.Dispose();
				}
				if (this.txComment != null)
				{
					this.ctComment = new CCounter(
						-nComment表示幅,
						(int)((((this.nComment行数 - 1) * this.nテクスチャの最大幅) + this.nComment最終行の幅) * this.txComment.vc拡大縮小倍率.X),
						unchecked((int)(10 * 2 / Scale.X)),
						CDTXMania.Instance.Timer
					);
				}
				#endregion
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			CResources cr = CDTXMania.Instance.Resources;
			//string path = Path.Combine(@"Graphics\fonts", cr.Explanation("strCfgSelectMusicSongArtistFontFileName"));
			string path = Path.Combine(@"Graphics\fonts", cr.Explanation("strCfgPopupFontFileName"));
			this.ftArtist描画フォント = new CPrivateFastFont(CSkin.Path(path), (int)(17f * 1));

			//path = Path.Combine(@"Graphics\fonts", cr.Explanation("strCfgSelectMusicSongCommentFontFileName"));
			path = Path.Combine(@"Graphics\fonts", cr.Explanation("strCfgPopupFontFileName"));
			this.ftComment描画フォント = new CPrivateFastFont(CSkin.Path(path), (int)(17f * 1));

			this.txArtist = null;
			this.txComment = null;
			this.strArtist = "";
			this.strComment = "";
			this.nComment最終行の幅 = 0;
			this.nComment行数 = 0;
			this.nテクスチャの最大幅 = 0;
			this.ctComment = new CCounter();
			base.On活性化();
		}
		public override void On非活性化()
		{
			TextureFactory.tテクスチャの解放(ref this.txArtist);
			TextureFactory.tテクスチャの解放(ref this.txComment);
			TextureFactory.t安全にDisposeする(ref ftArtist描画フォント);
			TextureFactory.t安全にDisposeする(ref ftComment描画フォント);

			this.ctComment = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.t選択曲が変更された();
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txArtist);
				TextureFactory.tテクスチャの解放(ref this.txComment);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (this.ctComment.b進行中)
				{
					this.ctComment.t進行Loop();
				}
				#region [ ARTIST ]
				if (this.txArtist != null)
				{
					//int x = (int)(SampleFramework.GameWindowSize.Width - (6 + 12) * Scale.X) - ((int)(this.txArtist.szテクスチャサイズ.Width * this.txArtist.vc拡大縮小倍率.X));    // #27648 2012.3.14 yyagi: -12 for scrollbar
					int x = (int)(SampleFramework.GameWindowSize.Width - (6 + 1) * Scale.X) - ((int)(this.txArtist.sz画像サイズ.Width * this.txArtist.vc拡大縮小倍率.X));    // #27648 2012.3.14 yyagi: -12 for scrollbar
					int y = (int)(232 * Scale.Y);
					this.txArtist.t2D描画(CDTXMania.Instance.Device, x, y);
				}
				#endregion
				#region [ Comment ]
				if ((this.txComment != null) && ((this.ctComment.n現在の値 + nComment表示幅) >= 0))
				{
					int posX = (int)(0xf8 * Scale.X);
					int posY = (int)(0xf6 * Scale.Y);
					Rectangle rectangle = new Rectangle(this.ctComment.n現在の値, 0, nComment表示幅, (int)this.ftComment描画フォント.Size);
					if (rectangle.X < 0)
					{
						posX += -rectangle.X;
						rectangle.Width -= -rectangle.X;
						rectangle.X = 0;
					}
					int num5 = ((int)(((float)rectangle.X) / this.txComment.vc拡大縮小倍率.X)) / this.nテクスチャの最大幅;
					Rectangle rectangle2 = new Rectangle();
					while (rectangle.Width > 0)
					{
						rectangle2.X = ((int)(((float)rectangle.X) / this.txComment.vc拡大縮小倍率.X)) % this.nテクスチャの最大幅;
						rectangle2.Y = num5 * ((int)this.ftComment描画フォント.Size);
						//int num6 = ((num5 + 1) == this.nComment行数) ? this.nComment最終行の幅 : this.nテクスチャの最大幅;
						int num6 = this.nテクスチャの最大幅;
						int num7 = num6 - rectangle2.X;
						rectangle2.Width = num7;
						rectangle2.Height = this.txComment.sz画像サイズ.Height;	//(int)this.ftComment描画フォント.Size;
						this.txComment.t2D描画(CDTXMania.Instance.Device, posX, posY, rectangle2);
						if (++num5 == this.nComment行数)
						{
							break;
						}
						int num8 = (int)(rectangle2.Width * this.txComment.vc拡大縮小倍率.X);
						rectangle.X += num8;
						rectangle.Width -= num8;
						posX += num8;
					}
				}
				#endregion
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ctComment;
		private CPrivateFastFont ftArtist描画フォント;
		private CPrivateFastFont ftComment描画フォント;
		private int nComment行数;
		private int nComment最終行の幅;
		private const int nComment表示幅 = (int)(0x182 * Scale.X);
		private int nテクスチャの最大幅;
		private string strArtist;
		private string strComment;
		private CTextureAf txArtist;
		private CTextureAf txComment;
		//-----------------
		#endregion
	}
}
