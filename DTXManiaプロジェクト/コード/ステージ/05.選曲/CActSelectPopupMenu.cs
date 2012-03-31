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
	internal class CActSelectPopupMenu : CActivity
	{

		// プロパティ

	
		public int GetIndex(int pos)
		{
			return lciMenuItems[ pos ].GetIndex();
		}
		public object GetObj現在値( int pos )
		{
			return lciMenuItems[ pos ].obj現在値();
		}

		/// <summary>
		/// ソートメニュー機能を使用中かどうか。外部からこれをtrueにすると、ソートメニューが出現する。falseにすると消える。
		/// </summary>
		public bool bIsActivePopupMenu
		{
			get;
			private set;
		}
		public void tActivatePopupMenu()
		{
			nItemSelecting = -1;		// #24757 2011.4.1 yyagi: Clear sorting status in each stating menu.
			this.bIsActivePopupMenu = true;
			this.bIsSelectingIntItem = false;
		}
		public void tDeativatePopupMenu()
		{
			this.bIsActivePopupMenu = false;
		}


		public void Initialize( string title, List<CItemBase> menulist, bool showAllItems )
		{
			Initialize( title, menulist, showAllItems, 0 );
		}

		public void Initialize( string title, List<CItemBase> menulist, bool showAllItems, int defaultPos )
		{
			strMenuTitle = title;
			lciMenuItems = menulist;
			bShowAllItems = showAllItems;
			n現在の選択行 = defaultPos;
		}


		public void tEnter押下( E楽器パート eInst )
		{
			if ( this.bキー入力待ち )
			{
				CDTXMania.Skin.sound決定音.t再生する();

				if ( this.n現在の選択行 != lciMenuItems.Count - 1 )
				{
					if ( lciMenuItems[ n現在の選択行 ].e種別 == CItemBase.E種別.リスト ||
						 lciMenuItems[ n現在の選択行 ].e種別 == CItemBase.E種別.ONorOFFトグル ||
						 lciMenuItems[ n現在の選択行 ].e種別 == CItemBase.E種別.ONorOFFor不定スリーステート	)
					{
						lciMenuItems[ n現在の選択行 ].t項目値を次へ移動();
					}
					else if ( lciMenuItems[ n現在の選択行 ].e種別 == CItemBase.E種別.整数 )
					{
						bIsSelectingIntItem = !bIsSelectingIntItem;
					}
					else
					{
						throw new ArgumentException();
					}
					nItemSelecting = n現在の選択行;
				}
				tEnter押下Main( eInst, (int) this.n現在の選択行, (int) lciMenuItems[ n現在の選択行 ].GetIndex() );

				this.bキー入力待ち = true;
			}
		}
		public virtual void tEnter押下Main( E楽器パート eInst, int order, int val )			// 継承先でメイン処理を記述すること
		{
		}
	
		public void t次に移動()
		{
			if ( this.bキー入力待ち )
			{
				CDTXMania.Skin.soundカーソル移動音.t再生する();
				if ( bIsSelectingIntItem )
				{
					 lciMenuItems[ n現在の選択行 ].t項目値を前へ移動();		// 項目移動と数値上下は方向が逆になるので注意
				}
				else
				{
					if ( ++this.n現在の選択行 >= this.lciMenuItems.Count )
					{
						this.n現在の選択行 = 0;
					}
				}
			}
		}
		public void t前に移動()
		{
			if ( this.bキー入力待ち )
			{
				CDTXMania.Skin.soundカーソル移動音.t再生する();
				if ( bIsSelectingIntItem )
				{
					lciMenuItems[ n現在の選択行 ].t項目値を次へ移動();		// 項目移動と数値上下は方向が逆になるので注意
				}
				else
				{
					if ( --this.n現在の選択行 < 0 )
					{
						this.n現在の選択行 = this.lciMenuItems.Count - 1;
					}
				}
			}
		}

		// CActivity 実装

		public override void On活性化()
		{
	//		this.n現在の選択行 = 0;
			this.bキー入力待ち = true;
			for ( int i = 0; i < 4; i++ )
			{
				this.ctキー反復用[ i ] = new CCounter( 0, 0, 0, CDTXMania.Timer );
			}
			base.b活性化してない = true;

			this.bIsActivePopupMenu = false;
			this.font = new CActDFPFont();
			base.list子Activities.Add( this.font );
			nItemSelecting = -1;

			base.On活性化();
		}
		public override void On非活性化()
		{
			if ( !base.b活性化してない )
			{
				base.list子Activities.Remove( this.font );
				this.font.On非活性化();
				this.font = null;

				CDTXMania.tテクスチャの解放( ref this.txCursor );
				CDTXMania.tテクスチャの解放( ref this.txPopupMenuBackground );
				for ( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if ( !base.b活性化してない )
			{
				this.txCursor = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig menu cursor.png" ), false );
				this.txPopupMenuBackground = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect sort menu background.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if ( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txPopupMenuBackground );
				CDTXMania.tテクスチャの解放( ref this.txCursor );
			}
			base.OnManagedリソースの解放();
		}

		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(bool)のほうを使用してください。" );
		}

		public int t進行描画()
		{
			if ( !base.b活性化してない && this.bIsActivePopupMenu )
			{
				if ( this.bキー入力待ち )
				{

					#region [ キー入力: キャンセル ]
					if ( CDTXMania.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Escape )
						|| CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.FT )
						|| CDTXMania.Pad.b押されたGB( Eパッド.Cancel ) )
					{	// キャンセル
						CDTXMania.Skin.sound取消音.t再生する();
						this.bIsActivePopupMenu = false;
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
					if ( eAction == ESortAction.Decide )	// 決定
					{
						this.tEnter押下( eInst );
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
				#region [ ポップアップメニュー 背景描画 ]
				if ( this.txPopupMenuBackground != null )
				{
					this.txPopupMenuBackground.t2D描画( CDTXMania.app.Device, 160, 40 );
				}
				#endregion
				#region [ ソートメニュータイトル描画 ]
				int x = 240, y = 44;
				font.t文字列描画( x, y, strMenuTitle, false, 1.0f );
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
					for ( int j = 0; j < 16; j++ )
					{
						this.txCursor.t2D描画( CDTXMania.app.Device, curX, curY, rectangle );
						curX += 16;
					}
					this.txCursor.t2D描画( CDTXMania.app.Device, curX, curY, new Rectangle( 0x10, 0, 16, 32 ) );
				}
				#endregion
				#region [ ソート候補文字列描画 ]
				for ( int i = 0; i < lciMenuItems.Count; i++ )
				{
					bool bItemBold = ( i == nItemSelecting && !bShowAllItems ) ? true : false;
					font.t文字列描画( 190, 80 + i * 32, lciMenuItems[i].str項目名, bItemBold, 1.0f );

					bool bValueBold = (bItemBold || (i == nItemSelecting && bIsSelectingIntItem)) ? true : false;
					if ( bItemBold || bShowAllItems )
					{
						string s;
						switch ( lciMenuItems[i].str項目名 )
						{
							case "PlaySpeed":
								{
									double d = (double) ((int)lciMenuItems[ i ].obj現在値() / 20.0);
									s = "x" + d.ToString( "0.000" );
								}
								break;
							case "ScrollSpeed":
								{
									double d = (double) ( ( ( (int) lciMenuItems[ i ].obj現在値() ) + 1 ) / 2.0 );
									s = "x" + d.ToString( "0.0" );
								}
								break;

							default:
								s = lciMenuItems[ i ].obj現在値().ToString();
								break;
						}
						font.t文字列描画( 340, 80 + i * 32, s, bValueBold, 1.0f );
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
		private CTexture txPopupMenuBackground;
		private CTexture txCursor;
		private CActDFPFont font;

		private string strMenuTitle;
		private List<CItemBase> lciMenuItems;
		private bool bShowAllItems;
		private bool bIsSelectingIntItem;

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
		private int nItemSelecting;		// 「n現在の選択行」とは別に設ける。sortでメニュー表示直後にアイテムの中身を表示しないようにするため

		//-----------------
		#endregion
	}
}
