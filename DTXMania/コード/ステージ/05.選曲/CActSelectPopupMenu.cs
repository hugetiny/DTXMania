using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using FDK;

using SlimDXKey = SlimDX.DirectInput.Key;

namespace DTXMania
{
	internal class CActSelectPopupMenu : CActivity
	{
		// プロパティ
		public int GetIndex(int pos)
		{
			return lciMenuItems[pos].cItem.Index;
		}

		public COptionBase GetObj現在値(int pos)
		{
			return lciMenuItems[pos].cItem;
		}

		public bool bGotoDetailConfig
		{
			get;
			internal set;
		}

		/// <summary>
		/// ポップアップメニュー機能を使用中かどうか。
		/// 外部からこれをtrueにすると、ポップアップメニューが出現する。falseにすると消える。
		/// </summary>
		public bool bIsActivePopupMenu
		{
			get;
			private set;
		}

		public virtual void tActivatePopupMenu(EPart einst)
		{
			nItemSelecting = -1;        // #24757 2011.4.1 yyagi: Clear sorting status in each stating menu.
			this.eInst = einst;
			this.bIsActivePopupMenu = true;
			this.bIsSelectingIntItem = false;
			this.bGotoDetailConfig = false;
		}
		public virtual void tDeativatePopupMenu()
		{
			this.bIsActivePopupMenu = false;
		}

		public void Initialize(List<COptionBase> menulist, bool showAllItems, string title, int defaultPos = 0)
		{
			string fontname = CDTXMania.Instance.Resources.Explanation("strCfgPopupFontFileName");
			string path = Path.Combine(@"Graphics\fonts", fontname);
			this.prvFont = new CPrivateFastFont(CSkin.Path(path), (int)(18 * Scale.Y));
			//prvFont = new CPrivateFastFont(CSkin.Path(@"Graphics\fonts\mplus-1p-heavy.ttf"), (int)(18 * Scale.Y));

			stqMenuTitle = new stQuickMenuItem();
			stqMenuTitle.cItem = new COptionString(title);
			stqMenuTitle.cItem.label = title;
			//stqMenuTitle.txName = TextureFactory.tテクスチャの生成(prvFont.DrawPrivateFont(title, Color.White, Color.Black), false);
			//stqMenuTitle.rectName = prvFont.RectStrings;
			lciMenuItems = new stQuickMenuItem[menulist.Count];
			for (int i = 0; i < menulist.Count; i++)
			{
				stQuickMenuItem stqm = new stQuickMenuItem();
				stqm.cItem = menulist[i];
				stqm.label = menulist[i].label;
				//stqm.txName = TextureFactory.tテクスチャの生成(prvFont.DrawPrivateFont(menulist[i].label, Color.White, Color.Black), false);
				//stqm.rectName = prvFont.RectStrings;
				lciMenuItems[i] = stqm;
			}

			bShowAllItems = showAllItems;
			n現在の選択行 = defaultPos;
		}


		public void tEnter押下()
		{
			if (this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.sound決定音.t再生する();

				if (this.n現在の選択行 != lciMenuItems.Length - 1)
				{
					if (lciMenuItems[n現在の選択行].cItem is IOptionList ||
							lciMenuItems[n現在の選択行].cItem is COptionBool)
					{
						lciMenuItems[n現在の選択行].cItem.OnNext();
					}
					else if (lciMenuItems[n現在の選択行].cItem is COptionInteger)
					{
						bIsSelectingIntItem = !bIsSelectingIntItem;     // 選択状態/選択解除状態を反転する
					}
					nItemSelecting = n現在の選択行;
				}
				lciMenuItems[n現在の選択行].cItem.OnEnter();
				this.bキー入力待ち = true;
			}
		}

		/// <summary>
		/// 追加の描画処理。必要に応じて、継承先で記述する。
		/// </summary>
		public virtual void t進行描画sub()
		{
		}


		/// <summary>
		/// 追加のキャンセル処理。必要に応じて、継承先で記述する。
		/// </summary>
		public virtual void tCancel()
		{

		}

		public void t次に移動()
		{
			if (this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				if (bIsSelectingIntItem)
				{
					// 項目移動と数値上下は方向が逆になるので注意
					lciMenuItems[n現在の選択行].cItem.OnPrevious();
				}
				else
				{
					if (++this.n現在の選択行 >= this.lciMenuItems.Length)
					{
						this.n現在の選択行 = 0;
					}
				}
			}
		}
		public void t前に移動()
		{
			if (this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				if (bIsSelectingIntItem)
				{
					// 項目移動と数値上下は方向が逆になるので注意
					lciMenuItems[n現在の選択行].cItem.OnNext();
				}
				else
				{
					if (--this.n現在の選択行 < 0)
					{
						this.n現在の選択行 = this.lciMenuItems.Length - 1;
					}
				}
			}
		}

		public override void On活性化()
		{
			this.n現在の選択行 = 0;
			this.bキー入力待ち = true;
			for (int i = 0; i < 4; i++)
			{
				this.ctキー反復用[i] = new CCounter(0, 0, 0, CDTXMania.Instance.Timer);
			}
			base.b活性化してない = true;

			this.bIsActivePopupMenu = false;
			nItemSelecting = -1;


			base.On活性化();
		}

		public override void On非活性化()
		{
			if (!base.b活性化してない)
			{
				//TextureFactory.tテクスチャの解放(ref this.txCursor);
				//TextureFactory.tテクスチャの解放(ref this.txPopupMenuBackground);
				for (int i = 0; i < 4; i++)
				{
					this.ctキー反復用[i] = null;
				}
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				string pathCursor = CSkin.Path(@"Graphics\ScreenConfig menu cursor.png"); ;
				string pathPopupMenuBackground = CSkin.Path(@"Graphics\ScreenSelect sort menu background.png");
				if (File.Exists(pathCursor))
				{
					this.txCursor = TextureFactory.tテクスチャの生成(pathCursor, false);
				}
				if (File.Exists(pathPopupMenuBackground))
				{
					this.txPopupMenuBackground = TextureFactory.tテクスチャの生成(pathPopupMenuBackground, false);
				}

				if (stqMenuTitle.txName == null)
				{
					stqMenuTitle.txName = TextureFactory.tテクスチャの生成(prvFont.DrawPrivateFont(stqMenuTitle.cItem.label, Color.White, Color.Black), false);
					stqMenuTitle.rectName = prvFont.RectStrings;
				}
				for (int i = 0; i < lciMenuItems.Length; i++)
				{
					lciMenuItems[i].txName = TextureFactory.tテクスチャの生成(prvFont.DrawPrivateFont(lciMenuItems[i].label, Color.White, Color.Black), false);
					lciMenuItems[i].rectName= prvFont.RectStrings;
				}

				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if ( base.b活性化してる )
			{
				TextureFactory.tテクスチャの解放( ref this.txPopupMenuBackground );
				TextureFactory.tテクスチャの解放( ref this.txCursor );

				TextureFactory.tテクスチャの解放( ref stqMenuTitle.txName );
				if ( lciMenuItems != null )
				{
					for ( int i = 0; i < lciMenuItems.Length; i++ )
					{
						TextureFactory.tテクスチャの解放( ref lciMenuItems[i].txName );
					}
				}
			}
			base.OnManagedリソースの解放();
		}

		public override int On進行描画()
		{
			throw new InvalidOperationException("t進行描画(bool)のほうを使用してください。");
		}

		public int t進行描画()
		{
			if (!base.b活性化してない && this.bIsActivePopupMenu)
			{
				if (this.bキー入力待ち)
				{
					#region [ Shift-F1: CONFIG画面 ]
					if( ( CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.RightShift ) || CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.LeftShift ) ) &&
						CDTXMania.Instance.Input管理.Keyboard.bキーが押された( (int) SlimDXKey.F1 ) )
					{
						// [SHIFT] + [F1] CONFIG
						CDTXMania.Instance.Skin.sound取消音.t再生する();
						// tCancel();
						this.bGotoDetailConfig = true;
					}
					#endregion
					#region [ キー入力: キャンセル ]
					else if( CDTXMania.Instance.Input管理.Keyboard.bキーが押された( (int) SlimDXKey.Escape )
							|| CDTXMania.Instance.Pad.bCancelPadIsPressedDGB() )
					{
						// キャンセル
						CDTXMania.Instance.Skin.sound取消音.t再生する();
						tCancel();
						this.bIsActivePopupMenu = false;
					}
					#endregion

					#region [ キー入力: 決定 ]
					// E楽器パート eInst = E楽器パート.UNKNOWN;
					ESortAction eAction = ESortAction.END;
					if (CDTXMania.Instance.Pad.b押された(EPad.GtDecide))
					{
						eInst = EPart.Guitar;
						eAction = ESortAction.Decide;
					}
					else if (CDTXMania.Instance.Pad.b押された(EPad.BsDecide))
					{
						eInst = EPart.Bass;
						eAction = ESortAction.Decide;
					}
					else if (
								 CDTXMania.Instance.Pad.b押された(EPad.CY) // #24756 2011.4.1 yyagi: Add condition "Drum-Decide" to enable CY in Sort Menu.
							|| CDTXMania.Instance.Pad.b押された(EPad.RD)
							|| CDTXMania.Instance.Pad.b押された(EPad.LC)
							|| (CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.Return)))
					{
						eInst = EPart.Drums;
						eAction = ESortAction.Decide;
					}
					if (eAction == ESortAction.Decide)  // 決定
					{
						this.tEnter押下();
					}
					#endregion
					#region [ キー入力: 前に移動 ]

					this.ctキー反復用.Up.tキー反復(
						CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.UpArrow ),
						new CCounter.DGキー処理( this.t前に移動 ) );

					this.ctキー反復用.R.tキー反復(
						CDTXMania.Instance.Pad.b押されている( EPad.GtR ) || CDTXMania.Instance.Pad.b押されている( EPad.BsR ),
						new CCounter.DGキー処理( this.t前に移動 ) );

					if (CDTXMania.Instance.Pad.b押された(EPad.SD))
					{
						this.t前に移動();
					}
					#endregion
					#region [ キー入力: 次に移動 ]
					this.ctキー反復用.Down.tキー反復(
						CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.DownArrow ),
						new CCounter.DGキー処理( this.t次に移動 ) );

					this.ctキー反復用.B.tキー反復(
						CDTXMania.Instance.Pad.b押されている( EPad.GtB ) || CDTXMania.Instance.Pad.b押されている( EPad.BsB ),
						new CCounter.DGキー処理( this.t次に移動 ) );

					if (CDTXMania.Instance.Pad.b押された(EPad.LT))
					{
						this.t次に移動();
					}
					#endregion
				}
				#region [ ポップアップ背景描画 ]
				if (this.txPopupMenuBackground != null)
				{
					this.txPopupMenuBackground.t2D描画(CDTXMania.Instance.Device, 160 * Scale.X, 10 * Scale.Y);
				}
				#endregion
				#region [ タイトル描画 ]
				int x = (int)(240 * Scale.X), y = (int)(16 * Scale.Y);
				if (stqMenuTitle.txName != null)
				{
					stqMenuTitle.txName.t2D描画(CDTXMania.Instance.Device, x, y);
				}
				#endregion
				#region [ カーソル描画 ]
				if (this.txCursor != null)
				{
					int height = stqMenuTitle.rectName.Height;
					int curX = (int)(180 * Scale.X);
					int curY = (int)(16 * Scale.Y) + (height / 4 - 6) + (height * (this.n現在の選択行 + 1));
					this.txCursor.t2D描画(CDTXMania.Instance.Device, curX, curY, new Rectangle(0, 0, (int)(16 * Scale.X), (int)(32 * Scale.Y)));
					curX += (int)(0x10 * Scale.X);
					Rectangle rectangle = new Rectangle((int)(8 * Scale.X), 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y));
					for (int j = 0; j < 16 + 5; j++)
					{
						this.txCursor.t2D描画(CDTXMania.Instance.Device, curX, curY, rectangle);
						curX += (int)(16 * Scale.Y);
					}
					this.txCursor.t2D描画(CDTXMania.Instance.Device, curX, curY, new Rectangle((int)(0x10 * Scale.X), 0, (int)(16 * Scale.X), (int)(32 * Scale.Y)));
				}
				#endregion
				#region [ リスト値文字列描画 ]
				for (int i = 0; i < lciMenuItems.Length; i++)
				{
					bool bItemBold = (i == nItemSelecting && !bShowAllItems) ? true : false;
					if (lciMenuItems[i].txName != null)
					{
						int height = lciMenuItems[i].rectName.Height;
						lciMenuItems[i].txName.t2D描画(CDTXMania.Instance.Device, 190 * Scale.X, (50 * Scale.Y) + i * height);
					}

					bool bValueBold = (bItemBold || (i == nItemSelecting && bIsSelectingIntItem)) ? true : false;
					if (bItemBold || bShowAllItems)
					{
						string s = lciMenuItems[i].cItem.ToString();
						using (Bitmap bmpStr = bValueBold ?
								prvFont.DrawPrivateFont(s, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
								prvFont.DrawPrivateFont(s, Color.White, Color.Black))
						{
							CTexture ctStr = TextureFactory.tテクスチャの生成(bmpStr, false);
							ctStr.t2D描画(CDTXMania.Instance.Device, 340 * Scale.X, (50 * Scale.Y) + i * prvFont.RectStrings.Height);
							TextureFactory.tテクスチャの解放(ref ctStr);
						}
					}
				}
				#endregion
				t進行描画sub();
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------

		private bool bキー入力待ち;

		internal int n現在の選択行;
		internal EPart eInst = EPart.Unknown;

		private CTexture txPopupMenuBackground;
		private CTexture txCursor;

		internal struct stQuickMenuItem
		{
			internal COptionBase cItem;
			internal CTexture txName;
			internal Rectangle rectName;
			internal string label;
		}
		private stQuickMenuItem[] lciMenuItems;
		CPrivateFastFont prvFont;

		private stQuickMenuItem stqMenuTitle;
		private bool bShowAllItems;
		private bool bIsSelectingIntItem;

		[StructLayout(LayoutKind.Sequential)]
		private struct STキー反復用カウンタ
		{
			public CCounter Up;
			public CCounter Down;
			public CCounter R;
			public CCounter B;
			public CCounter this[int index]
			{
				get
				{
					switch (index)
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
					switch (index)
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
		// 「n現在の選択行」とは別に設ける。sortでメニュー表示直後にアイテムの中身を表示しないようにするため
		private int nItemSelecting;

		//-----------------
		#endregion
	}
}
