using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// プライベートフォントでの描画を扱うクラス。
	/// </summary>
	public class CPrivateFont : IDisposable
	{
		// コンストラクタ
		public CPrivateFont( string fontpath, int pt, FontStyle style )
		{
			Initialize( fontpath, pt, style );
		}
		public CPrivateFont( string fontpath, int pt )
		{
			Initialize( fontpath, pt, FontStyle.Regular );
		}

		private void Initialize( string fontpath, int pt, FontStyle style )
		{
			pfc = null;
			_fontfamily = null;
			_font = null;
			_pt = pt;

			try
			{
				pfc = new System.Drawing.Text.PrivateFontCollection();	//PrivateFontCollectionオブジェクトを作成する
				pfc.AddFontFile( fontpath );							//PrivateFontCollectionにフォントを追加する
			}
			catch ( System.IO.FileNotFoundException )
			{
				Trace.TraceError( "プライベートフォントの追加に失敗しました。({0})", fontpath );
			}

			//foreach ( FontFamily ff in pfc.Families )
			//{
			//    Debug.WriteLine( "fontname=" + ff.Name );
			//    if ( ff.Name == Path.GetFileNameWithoutExtension( fontpath ) )
			//    {
			//        _fontfamily = ff;
			//        break;
			//    }
			//}
			//if ( _fontfamily == null )
			//{
			//    Trace.TraceError( "プライベートフォントの追加後、検索に失敗しました。({0})", fontpath );
			//    return;
			//}
			_fontfamily = pfc.Families[ 0 ];

			_font = new Font( _fontfamily, pt, style );			//PrivateFontCollectionの先頭のフォントのFontオブジェクトを作成する
		}


			/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontcolor">描画色</param>
		/// <returns>描画済テクスチャ</returns>
		public CTexture DrawPrivateFont( string drawstr, Color fontcolor )
		{
			return DrawPrivateFont( drawstr, fontcolor, fontcolor );
		}

		/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontcolor">描画色</param>
		/// <param name="edgecolor">縁取色</param>
		/// <returns>描画済テクスチャ</returns>
		public CTexture DrawPrivateFont( string drawstr, Color fontcolor, Color edgecolor )
		{
			if ( _fontfamily == null )
				return null;

			bool bEdge = ( fontcolor != edgecolor );

			// 縁取りの縁のサイズは、とりあえずフォントの大きさの1/4とする
			int nEdgePt = (bEdge)? _pt / 4 : 0;

			// 描画サイズを測定する
			Size stringSize = System.Windows.Forms.TextRenderer.MeasureText( drawstr, _font );

			//取得した描画サイズを基に、描画先のbitmapを作成する
			Bitmap bmp = new Bitmap( stringSize.Width + nEdgePt * 2, stringSize.Height + nEdgePt * 2 );
			bmp.MakeTransparent();
			Graphics g = Graphics.FromImage( bmp );
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Far;	// 画面下部（垂直方向位置）
			sf.Alignment = StringAlignment.Center;	// 画面中央（水平方向位置）

			// レイアウト枠
			Rectangle r = new Rectangle( 0, 0, stringSize.Width + nEdgePt * 2, stringSize.Height + nEdgePt * 2 );

			if ( bEdge )
			{
				// DrawPathで、ポイントサイズを使って描画するために、DPIを使って単位変換する
				// (これをしないと、単位が違うために、小さめに描画されてしまう)
				float sizeInPixels = _font.SizeInPoints * g.DpiY / 72;  // 1 inch = 72 points

				System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
				gp.AddString( drawstr, _fontfamily, (int) _font.Style, sizeInPixels, r, sf );

				// 縁取りを描画する
				Pen p = null;
				if ( bEdge )
				{
					p = new Pen( edgecolor, nEdgePt );
					p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
					g.DrawPath( p, gp );
				}
				// 塗りつぶす
				SolidBrush br = new SolidBrush( fontcolor );
				g.FillPath( br, gp );

				if ( br != null ) br.Dispose(); br = null;
				if ( p != null ) p.Dispose(); p = null;
				if ( gp != null ) gp.Dispose(); gp = null;
			}
			else
			{
				System.Windows.Forms.TextRenderer.DrawText( g, drawstr, _font, new Point( 0, 0 ), fontcolor );
			}
#if debug表示
			g.DrawRectangle( new Pen( Color.White, 1 ), new Rectangle( 1, 1, stringSize.Width-1, stringSize.Height-1 ) );
			g.DrawRectangle( new Pen( Color.Green, 1 ), new Rectangle( 0, 0, bmp.Width - 1, bmp.Height - 1 ) );
#endif
			CTexture txBmp = CDTXMania.tテクスチャの生成( bmp, false );

			#region [ リソースを解放する ]
			if ( sf != null )	sf.Dispose();	sf = null;
			if ( g != null )	g.Dispose();	g = null;
			if ( bmp != null )	bmp.Dispose();	bmp = null;
			#endregion

			return txBmp;
		}


		#region [ IDosposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if ( !this.bDispose完了済み )
			{
				if ( this._font != null )
				{
					this._font.Dispose();
					this._font = null;
				}
				if ( this.pfc != null )
				{
					this.pfc.Dispose();
					this.pfc = null;
				}

				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private bool bDispose完了済み;
		private System.Drawing.Text.PrivateFontCollection pfc;
		private Font _font;
		private FontFamily _fontfamily;
		int _pt;
		//-----------------
		#endregion
	}
}
