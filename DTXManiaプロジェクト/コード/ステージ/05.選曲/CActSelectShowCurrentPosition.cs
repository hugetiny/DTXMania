using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CActSelectShowCurrentPosition : CActivity
	{
		// メソッド

		public CActSelectShowCurrentPosition()
		{
			base.b活性化してない = true;
		}
		public void t選択曲が変更された()
		{
			C曲リストノード song = CDTXMania.stage選曲.r現在選択中の曲;
			if ( song == null )
				return;
			List<C曲リストノード> list = ( song.r親ノード != null ) ? song.r親ノード.list子リスト : CDTXMania.Songs管理.list曲ルート;
			int index = list.IndexOf( song ) + 1;
			if ( index <= 0 )
			{
				nCurrentPosition = nNumOfItems = 0;
			}
			else
			{
				nCurrentPosition = index;
				nNumOfItems = list.Count;
			}
		}

		public void t次に移動()
		{
			if ( this.nNumOfItems > 0 )
			{
				this.n目標のスクロールカウンタ += 100;
			}
		}
		public void t前に移動()
		{
			if ( this.nNumOfItems > 0 )
			{
				this.n目標のスクロールカウンタ -= 100;
			}
		}
		// CActivity 実装

		public override void On活性化()
		{
			this.ctComment = new CCounter();
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ctComment = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if ( !base.b活性化してない )
			{
				this.txアイテム数数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect skill number on gauge etc.png" ), false );
				this.txScrollBar = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect scrollbar.png" ), false );
				this.txScrollPosition = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect scrollbar.png" ), false );
//				this.t選択曲が変更された();
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if ( !base.b活性化してない )
			{
				CDTXMania.t安全にDisposeする( ref this.txアイテム数数字 );
				CDTXMania.t安全にDisposeする( ref this.txScrollBar );
				CDTXMania.t安全にDisposeする( ref this.txScrollPosition );

				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			#region [ スクロールバーと場所の描画 ]
			this.txScrollBar.t2D描画( CDTXMania.app.Device, 640 - 12, 58, new Rectangle( 0, 0, 12, 336 ) );	// 本当のy座標は88なんだが、なぜか約30のバイアスが掛かる・・・

			int y;
			if ( nNumOfItems > 1 )
			{
				y = ( 336 - 6 - 8) / ( nNumOfItems - 1 ) * ( nCurrentPosition - 1 );
				if ( nCurrentPosition == nNumOfItems )
				{
					y = 336 - 6 - 8;	// -6: scrollbarの上下エッジ -8:ポジション表示アイテムの高さ
				}
			}
			else
			{
				y = 0;
			}
			if ( y > 336 ) Debug.WriteLine( "over!!!:" + y );
			this.txScrollPosition.t2D描画( CDTXMania.app.Device, 640 - 12 + 3, 58 + 3 + y, new Rectangle( 12, 0, 6, 8 ) );
			#endregion

			tアイテム数の描画();
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ctComment;
		private int n現在のスクロールカウンタ;
		private int n目標のスクロールカウンタ;
		private int nCurrentPosition = 0;
		private int nNumOfItems = 0;
		private CTexture txScrollPosition;
		private CTexture txScrollBar;
		private CTexture txアイテム数数字;
		//-----------------

		private void tアイテム数の描画()
		{
			string s = nCurrentPosition.ToString() + "/" + nNumOfItems.ToString();
			int x = 639 - 8;
			int y = 362;

			for ( int p = s.Length - 1; p >= 0; p-- )
			{
				tアイテム数の描画・１桁描画( x, y, s[ p ] );
				x -= 8;
			}
		}
		private void tアイテム数の描画・１桁描画( int x, int y, char s数値 )
		{
			int dx, dy;
			if ( s数値 == '/' )
			{
				dx = 48;
				dy = 0;
			}
			else
			{
				int n = (int) s数値 - (int) '0';
				dx = ( n % 6 ) * 8;
				dy = ( n / 6 ) * 12;
			}
			if ( this.txアイテム数数字 != null )
			{
				this.txアイテム数数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, dy, 8, 12 ) );
			}
		}
		#endregion
	}
}
