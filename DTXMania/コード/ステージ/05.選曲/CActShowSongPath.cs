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
				#region [ songpathテクスチャ生成 ]
				TextureFactory.tテクスチャの解放(ref this.txSongPath);
				this.strSongPath = "dtx    : " + cスコア.ファイル情報.ファイルの絶対パス + Environment.NewLine +
					"set.def: " + c曲リストノード.pathSetDefの絶対パス;
				if ((this.strSongPath != null) && (this.strSongPath.Length > 0))
				{
					try
					{
						using ( Bitmap bmp = this.pfSongPath描画用フォント.DrawPrivateFont( this.strSongPath, System.Drawing.Color.White ) )
						using ( Bitmap bmp_back = new Bitmap( SampleFramework.GameWindowSize.Width, bmp.Height ) )
						using ( Graphics graphics = Graphics.FromImage( bmp_back ) )
						{
							graphics.Clear( System.Drawing.Color.FromArgb( 192, 0, 0, 0 ) );

							int w = Math.Min( SampleFramework.GameWindowSize.Width, bmp.Width );
							graphics.DrawImage( bmp, 0, 0, w, bmp.Height );

							this.txSongPath = new CTexture( CDTXMania.Instance.Device, bmp_back, CDTXMania.Instance.TextureFormat );
						}
					}
					catch (CTextureCreateFailedException)
					{
						Trace.TraceError("SongPathテクスチャの生成に失敗しました。");
						this.txSongPath = null;
					}
				}
				#endregion
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.txSongPath = null;
			this.strSongPath = "";

			base.On活性化();
		}
		public override void On非活性化()
		{
			TextureFactory.tテクスチャの解放(ref this.txSongPath);
			this.pfSongPath描画用フォント?.Dispose();
			this.pfSongPath描画用フォント = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				//string fontname = CDTXMania.Instance.Resources.Explanation("strCfgSelectMusicSongCommentFontFileName");
				//string path = Path.Combine(@"Graphics\fonts", fontname);
				//this.pfSongPath描画用フォント = new CPrivateFastFont( CSkin.Path(path), (float)(12f * Scale.Y * 72f / 96f) );
				//// 72f/96f: 従来互換のために追加。DPI依存→非依存化に付随した変更。
				//this.ftSongPath描画用フォント = this.pfSongPath描画用フォント.font;

				//this.ftSongPath描画用フォント = new Font( "MS UI Gothic", (float)( 12f * Scale.Y ) );
				string fontname = CDTXMania.Instance.Resources.Explanation( "strCfgConfigurationItemsFontFileName" );
				string path = Path.Combine( @"Graphics\fonts", fontname );
				this.pfSongPath描画用フォント = new CPrivateFastFont( CSkin.Path( path ), (int)( 12 * Scale.Y ) ); 
				
				this.t選択曲が変更された();
				base.OnManagedリソースの作成();
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
		//private Font ftSongPath描画用フォント;
		private CPrivateFastFont pfSongPath描画用フォント;
		private string strSongPath;
		private CTexture txSongPath;
		//-----------------
		#endregion
	}
}
