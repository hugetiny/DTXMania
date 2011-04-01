using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActSortSongs : CActivity
	{

		// プロパティ

		/// <summary>
		/// ソートメニュー機能を使用中かどうか。外部からこれをtrueにすると、ソートメニューが出現する。falseにすると消える。
		/// </summary>
		public bool bIsActiveSortMenu
		{
			get; private set;
		}

		// コンストラクタ

		public CActSortSongs()
		{
			base.b活性化してない = true;
			this.bキー入力待ち = true;
			this.bIsActiveSortMenu = false;
			this.font = new CActDFPFont();
			base.list子Activities.Add( this.font );
			this.n現在の選択行 = 0;
			nSortType = (int) ESortItem.Default;
			nSortOrder = (int) ESortOrder.Descend;
			bIsJapanLocale = ( CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja" );	// #24758 2011.4.1 yyagi add; To check JP locale
		}


		// メソッド
		public void tActivateSortMenu( ref CActSelect曲リスト ca )
		{
			this.act曲リスト = ca;
			nSortType = (int) ESortItem.Default;		// #24757 2011.4.1 yyagi: Clear sorting status in each stating menu.
			nSortOrder = (int) ESortOrder.Descend;		//
			this.bIsActiveSortMenu = true;
		}
		public void tDeativateSortMenu()
		{
			this.bIsActiveSortMenu = false;
		}

		public void tEnter押下(E楽器パート eInst)
		{
			if( this.bキー入力待ち )
			{
				CDTXMania.Skin.sound決定音.t再生する();

				if ( this.n現在の選択行 != (int) ESortItem.Return )
				{
					nSortOrder = ( nSortType == n現在の選択行 )? -nSortOrder : (int)ESortOrder.Ascend;
					nSortType = n現在の選択行;
				}

				switch( this.n現在の選択行 )
				{
					case (int) ESortItem.Title:
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート2_タイトル順, eInst, nSortOrder
						);
						break;
					case (int) ESortItem.Level:
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート4_LEVEL順, eInst, nSortOrder,
							this.act曲リスト.n現在のアンカ難易度レベル
						);
						break;
					case (int) ESortItem.BestRank:
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート5_BestRank順, eInst, nSortOrder,
							this.act曲リスト.n現在のアンカ難易度レベル
						);
						break;
					case (int) ESortItem.PlayCount:
//						this.act曲リスト.t曲リストのソート3_演奏回数の多い順( eInst, nSortOrder );
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート3_演奏回数の多い順, eInst, nSortOrder,
							this.act曲リスト.n現在のアンカ難易度レベル
						);
						break;
//					case (int) ESortItem.Author:
//						break;
					case (int) ESortItem.SkillPoint:
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート6_SkillPoint順, eInst, nSortOrder,
							this.act曲リスト.n現在のアンカ難易度レベル
						);
						break;
//					case (int) ESortItem.BPM:
//						break;
					case (int) ESortItem.Date:
						this.act曲リスト.t曲リストのソート(
							CDTXMania.Songs管理.t曲リストのソート7_更新日時順, eInst, nSortOrder,
							this.act曲リスト.n現在のアンカ難易度レベル
						);
						break;
					case (int)ESortItem.Return:
						this.tDeativateSortMenu();
						break;
					default:
						break;
				}

				this.bキー入力待ち = true;
			}
		}
		public void t次に移動()
		{
			if( this.bキー入力待ち )
			{
				CDTXMania.Skin.soundカーソル移動音.t再生する();
				this.n現在の選択行 = ( this.n現在の選択行 + 1 ) % (int)ESortItem.END;
			}
		}
		public void t前に移動()
		{
			if( this.bキー入力待ち )
			{
				CDTXMania.Skin.soundカーソル移動音.t再生する();
				this.n現在の選択行 = ( ( this.n現在の選択行 - 1 ) + (int)ESortItem.END ) % (int)ESortItem.END;
			}
		}

		
		// CActivity 実装

		public override void On活性化()
		{
			this.n現在の選択行 = 0;
			this.bキー入力待ち = true;
			for ( int i = 0; i < 4; i++ )
			{
				this.ctキー反復用[ i ] = new CCounter( 0, 0, 0, CDTXMania.Timer );
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txCursor );
				CDTXMania.tテクスチャの解放( ref this.txSortMenuBackground );
				for ( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txCursor = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig menu cursor.png" ), false );
				this.txSortMenuBackground = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect sort menu background.png" ), false );
				this.txSortMenuChoices = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect sort menu choices.png" ), false );	// #24758 2011.4.1 yyagi; for JP locale, Japanese 昇順/降順 (ascend/descend) png parts.
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if ( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txSortMenuChoices );
				CDTXMania.tテクスチャの解放( ref this.txSortMenuBackground );
				CDTXMania.tテクスチャの解放( ref this.txCursor );
			}
			base.OnManagedリソースの解放();
		}

		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(bool)のほうを使用してください。" );
		}
		public int t進行描画( bool bIsActive )
		{
			if( !base.b活性化してない && bIsActive )
			{
				if( this.bキー入力待ち )
				{

					#region [ キー入力: キャンセル ]
					if ( CDTXMania.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Escape )
						|| CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.FT )
						|| CDTXMania.Pad.b押されたGB( Eパッド.Cancel ) )
					{	// キャンセル
						CDTXMania.Skin.sound取消音.t再生する();
						//this.bキー入力待ち = false;
						this.bIsActiveSortMenu = false;
					}
					#endregion
					#region [ キー入力: 決定 ]
					E楽器パート eInst = E楽器パート.UNKNOWN;
					ESortAction eAction = ESortAction.END;
					if ( CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.Decide ) )
					{
						eInst = E楽器パート.GUITAR;
						eAction = ESortAction.Decide;
					}
					else if ( CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.Decide ) )
					{
						eInst = E楽器パート.BASS;
						eAction = ESortAction.Decide;
					}
					else if (
						CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.Decide )	// #24756 2011.4.1 yyagi: Add condition "Drum-Decide" to enable CY in Sort Menu.
						|| CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.RD ) 
						|| CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LC )
						|| ( CDTXMania.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && CDTXMania.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Return ) ) )
					{
						eInst = E楽器パート.DRUMS;
						eAction = ESortAction.Decide;
					}
					if (eAction == ESortAction.Decide)	// 決定
					{
						this.tEnter押下(eInst);
					}
					#endregion
					#region [ キー入力: 前に移動 ]
					this.ctキー反復用.Up.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.UpArrow ), new CCounter.DGキー処理( this.t前に移動 ) );
					this.ctキー反復用.R.tキー反復( CDTXMania.Pad.b押されているGB( Eパッド.R ), new CCounter.DGキー処理( this.t前に移動 ) );
					if ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.SD ) )
					{
						this.t前に移動();
					}
					#endregion
					#region [ キー入力: 次に移動 ]
					this.ctキー反復用.Down.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.DownArrow ), new CCounter.DGキー処理( this.t次に移動 ) );
					this.ctキー反復用.B.tキー反復( CDTXMania.Pad.b押されているGB( Eパッド.B ), new CCounter.DGキー処理( this.t次に移動 ) );
					if ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LT ) )
					{
						this.t次に移動();
					}
					#endregion
				}
				#region [ ソートメニュー 背景描画 ]
				if ( this.txSortMenuBackground != null )
				{
					this.txSortMenuBackground.t2D描画( CDTXMania.app.Device, 160, 40 );
				}
				#endregion
				#region [ ソートメニュータイトル描画 ]
				int x = 240, y = 44;
				font.t文字列描画( x, y, "SORT MENU", false, 1.0f);
				#endregion
				#region [ カーソル描画 ]
				if ( this.txCursor != null )
				{
					int height = 32;
					int curX = 180;
					int curY = 46 + ( height * ( this.n現在の選択行 + 1 ) );
					this.txCursor.t2D描画( CDTXMania.app.Device, curX, curY, new Rectangle( 0, 0, 16, 32 ) );
					curX += 0x10;
					Rectangle rectangle = new Rectangle( 8, 0, 0x10, 0x20 );
					for( int j = 0; j < 16; j++ )
					{
						this.txCursor.t2D描画( CDTXMania.app.Device, curX, curY, rectangle );
						curX += 16;
					}
					this.txCursor.t2D描画( CDTXMania.app.Device, curX, curY, new Rectangle( 0x10, 0, 16, 32 ) );
				}
				#endregion
				#region [ ソート候補文字列描画 ]
				string[] strSortItem = {
					"Title", "Level", "Best Rank", "PlayCount",
					//"Author",
					"SkillPoint",
					//"BPM",
					"Date",
					"Return"
				};
				string[] strSortOrder = {
					"Descend", "", "Ascend"
				};
				for ( int i = 0; i < strSortItem.Length; i++ )
				{
					bool bBold = ( i == nSortType ) ? true : false;
					font.t文字列描画( 190, 80 + i * 32, strSortItem[ i ], bBold, 1.0f );
					if ( bBold )
					{
						// nSortOder+1 == 0(Ascend), (1,) 2(Descend)
						if ( bIsJapanLocale )
						{	// #24758 2011.4.1 yyagi: for JP locale, 昇順/降順 is used instead of ascend/descend.
							Rectangle rect = new Rectangle( 0, this.txSortMenuChoices.sz画像サイズ.Height / 2 * (nSortOrder+1)/2, this.txSortMenuChoices.sz画像サイズ.Width, this.txSortMenuChoices.sz画像サイズ.Height / 2 );
							this.txSortMenuChoices.t2D描画( CDTXMania.app.Device, 350, 78 + i * 32, rect );
						}
						else
						{
							font.t文字列描画( 350, 80 + i * 32, strSortOrder[ nSortOrder + 1 ], bBold, 1.0f );
						}
					}
				}
				#endregion

			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------

		private bool bキー入力待ち;

		private int n現在の選択行;
		private CTexture txSortMenuBackground;
		private CTexture txSortMenuChoices;
		private CTexture txCursor;
		private CActDFPFont font;
		private CActSelect曲リスト act曲リスト;
		private bool bIsJapanLocale;

		[StructLayout( LayoutKind.Sequential )]
		private struct STキー反復用カウンタ
		{
			public CCounter Up;
			public CCounter Down;
			public CCounter R;
			public CCounter B;
			public CCounter this[ int index ]
			{
				get
				{
					switch ( index )
					{
						case 0:
							return this.Up;

						case 1:
							return this.Down;

						case 2:
							return this.R;

						case 3:
							return this.B;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch ( index )
					{
						case 0:
							this.Up = value;
							return;

						case 1:
							this.Down = value;
							return;

						case 2:
							this.R = value;
							return;

						case 3:
							this.B = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		private STキー反復用カウンタ ctキー反復用;

		private enum ESortAction : int
		{
			Cancel, Decide, Previous, Next, END
		}
		private enum ESortItem : int
		{
			Title = 0, Level, BestRank, PlayCount,
			//Author,
			SkillPoint,
			//BPM,
			Date,
			Return, END,
			Default = 99
		};
		private enum ESortOrder : int
		{
			Ascend = 1, Descend = -1
		}
		private int nSortType, nSortOrder;

		//-----------------
		#endregion
	}


}
