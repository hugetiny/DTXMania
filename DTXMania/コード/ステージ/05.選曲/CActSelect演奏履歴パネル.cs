﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using SharpDX;
using FDK;

namespace DTXMania
{
	internal class CActSelect演奏履歴パネル : CActivity
	{
		// メソッド

		public CActSelect演奏履歴パネル()
		{
			base.b活性化してない = true;
		}
		public void t選択曲が変更された()
		{
			Cスコア cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;
			if ((cスコア != null) && !CDTXMania.Instance.stage選曲.bスクロール中)
			{
				try
				{
					Bitmap image = new Bitmap((int)(400 * Scale.X), (int)(130 * Scale.Y));
					Graphics graphics = Graphics.FromImage(image);
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					for (int i = 0; i < 5; i++)
					{
						if ((cスコア.譜面情報.演奏履歴[i] != null) && (cスコア.譜面情報.演奏履歴[i].Length > 0))
						{
							//graphics.DrawString(cスコア.譜面情報.演奏履歴[i], this.ft表示用フォント, Brushes.Yellow, (float)0f, (float)(i * 24f * Scale.Y));
							using ( Bitmap bmp = this.prvFont.DrawPrivateFont( cスコア.譜面情報.演奏履歴[ i ], System.Drawing.Color.Yellow ) )
							{
								graphics.DrawImage( bmp, (float)0f, (float)( i * 24f * Scale.Y ), bmp.Width, bmp.Height );
							}
						}
					}
					graphics.Dispose();
					if (this.tx文字列パネル != null)
					{
						this.tx文字列パネル.Dispose();
					}
					this.tx文字列パネル = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);
					this.tx文字列パネル.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
					image.Dispose();
				}
				catch (CTextureCreateFailedException)
				{
					Trace.TraceError("演奏履歴文字列テクスチャの作成に失敗しました。");
					this.tx文字列パネル = null;
				}
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n本体X = (int)(0x195 * Scale.X);
			this.n本体Y = (int)(0x174 * Scale.Y);
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct登場アニメ用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				//this.ft表示用フォント = new Font( "Arial", 26f * Scale.Y, FontStyle.Bold, GraphicsUnit.Pixel );
				string fontname = CDTXMania.Instance.Resources.Explanation( "strCfgSelectMusicInformationFontFileName" );
				string path = Path.Combine( @"Graphics\fonts", fontname );
				this.prvFont = new CPrivateFastFont( CSkin.Path( path ), (int)( 13 * Scale.Y ) );

				this.txパネル本体 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect play history panel.png"), true);
				this.t選択曲が変更された();
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txパネル本体);
				TextureFactory.tテクスチャの解放(ref this.tx文字列パネル);

				this.prvFont?.Dispose();
				this.prvFont = null;
				
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (base.b初めての進行描画)
				{
					this.ct登場アニメ用 = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
					base.b初めての進行描画 = false;
				}
				this.ct登場アニメ用.t進行();
				if (this.ct登場アニメ用.b終了値に達した || (this.txパネル本体 == null))
				{
					this.n本体X = (int)(0x195 * Scale.X);
					this.n本体Y = (int)(0x174 * Scale.Y);
				}
				else
				{
					double num = ((double)this.ct登場アニメ用.n現在の値) / 100.0;
					double num2 = Math.Cos((1.5 + (0.5 * num)) * Math.PI);
					this.n本体X = (int)(0x195 * Scale.X);
					this.n本体Y = (int)(0x174 * Scale.Y) + ((int)(this.txパネル本体.sz画像サイズ.Height * (1.0 - (num2 * num2))));
				}
				if (this.txパネル本体 != null)
				{
					this.txパネル本体.t2D描画(CDTXMania.Instance.Device, this.n本体X, this.n本体Y);
				}
				if (this.tx文字列パネル != null)
				{
					this.tx文字列パネル.t2D描画(CDTXMania.Instance.Device, this.n本体X + (int)(12 * Scale.X), this.n本体Y + (int)(0x13 * Scale.Y));
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ct登場アニメ用;
		private CPrivateFastFont prvFont;
		private int n本体X;
		private int n本体Y;
		private CTexture txパネル本体;
		private CTexture tx文字列パネル;
		//-----------------
		#endregion
	}
}
