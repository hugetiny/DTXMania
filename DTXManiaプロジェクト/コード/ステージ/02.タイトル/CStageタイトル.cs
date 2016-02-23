using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using SlimDX.DirectInput;
using FDK;

namespace DTXMania
{
	internal class CStageタイトル : CStage
	{
		// コンストラクタ
		public CStageタイトル()
		{
			base.eステージID = CStage.Eステージ.タイトル;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actFIfromSetup = new CActFIFOWhite());
			base.list子Activities.Add(this.actFI = new CActFIFOWhite());
			base.list子Activities.Add(this.actFO = new CActFIFOWhite());
		}


		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation("タイトルステージを活性化します。");
			Trace.Indent();
			try
			{
				for (int i = 0; i < 4; i++)
				{
					this.ctキー反復用[i] = new CCounter(0, 0, 0, CDTXMania.app.Timer);
				}
				this.ct上移動用 = new CCounter();
				this.ct下移動用 = new CCounter();
				this.ctカーソルフラッシュ用 = new CCounter();
				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation("タイトルステージの活性化を完了しました。");
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation("タイトルステージを非活性化します。");
			Trace.Indent();
			try
			{
				for (int i = 0; i < 4; i++)
				{
					this.ctキー反復用[i] = null;
				}
				this.ct上移動用 = null;
				this.ct下移動用 = null;
				this.ctカーソルフラッシュ用 = null;
			}
			finally
			{
				Trace.TraceInformation("タイトルステージの非活性化を完了しました。");
				Trace.Unindent();
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx背景 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenTitle background.jpg"), false);
				this.txメニュー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenTitle menu.png"), false);

				prvFont = new CPrivateFastFont(CSkin.Path(@"Graphics\fonts\mplus-1p-heavy.ttf"), 36);
				//prvFont = new CPrivateFont( new FontFamily("MS UI Gothic"), 36, FontStyle.Bold );
				string[] menuStr = { "START", "CONFIGURATION", "EXIT" };
				this.txMenuItem = new CTexture[menuStr.Length];
				this.stMenuItem = new STMenuItem[menuStr.Length];

				// メニュー項目に対応したテクスチャ画像を生成しつつ、テクスチャ画像の最大横幅を取得しておく
				int maxX = 0;
				for (int i = 0; i < menuStr.Length; i++)
				{
					txMenuItem[i] = TextureFactory.tテクスチャの生成(prvFont.DrawPrivateFont(menuStr[i], Color.White, Color.Black), false);
					//					txMenuItem[ i ] = prvFont.DrawPrivateFont( menuStr[ i ], Color.White, Color.Black, Color.Yellow, Color.OrangeRed );	// グラデーションもなかなかいける。
					//					txMenuItem[ i ] = prvFont.DrawPrivateFont( menuStr[ i ], Color.White );
					if (maxX < txMenuItem[i].szテクスチャサイズ.Width)
					{
						maxX = txMenuItem[i].szテクスチャサイズ.Width;
					}
				}
				// センタリング表示するために、X座標のオフセット値を算出・格納する
				for (int i = 0; i < menuStr.Length; i++)
				{
					stMenuItem[i] = new STMenuItem();
					stMenuItem[i].txMenuItem = txMenuItem[i];
					stMenuItem[i].offsetX = (maxX - txMenuItem[i].szテクスチャサイズ.Width) / 2;
				}
				prvFont.Dispose();

				MENU_X = (SampleFramework.GameWindowSize.Width - maxX) / 2;
				MENU_Y = 600;

				base.OnManagedリソースの作成();
			}
		}


		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < txMenuItem.Length; i++)
				{
					if (txMenuItem[i] != null)
					{
						TextureFactory.tテクスチャの解放(ref txMenuItem[i]);
					}
				}
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.txメニュー);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				#region [ 初めての進行描画 ]
				//---------------------
				if (base.b初めての進行描画)
				{
					if (CDTXMania.app.r直前のステージ == CDTXMania.app.stage起動)
					{
						this.actFIfromSetup.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.タイトル_起動画面からのフェードイン;
					}
					else
					{
						this.actFI.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					}
					this.ctカーソルフラッシュ用.t開始(0, 700, 5, CDTXMania.app.Timer);
					this.ctカーソルフラッシュ用.n現在の値 = 100;
					base.b初めての進行描画 = false;
				}
				//---------------------
				#endregion

				// 進行

				#region [ カーソル上移動 ]
				//---------------------
				if (this.ct上移動用.b進行中)
				{
					this.ct上移動用.t進行();
					if (this.ct上移動用.b終了値に達した)
					{
						this.ct上移動用.t停止();
					}
				}
				//---------------------
				#endregion
				#region [ カーソル下移動 ]
				//---------------------
				if (this.ct下移動用.b進行中)
				{
					this.ct下移動用.t進行();
					if (this.ct下移動用.b終了値に達した)
					{
						this.ct下移動用.t停止();
					}
				}
				//---------------------
				#endregion
				#region [ カーソルフラッシュ ]
				//---------------------
				this.ctカーソルフラッシュ用.t進行Loop();
				//---------------------
				#endregion

				#region [ キー入力 ]
				if (base.eフェーズID == CStage.Eフェーズ.共通_通常状態		// 通常状態、かつ
					&& CDTXMania.app.act現在入力を占有中のプラグイン == null)	// プラグインの入力占有がない
				{
					if (CDTXMania.app.Input管理.Keyboard.bキーが押された((int)Key.Escape))
						return (int)E戻り値.EXIT;

					this.ctキー反復用.Up.tキー反復(CDTXMania.app.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.UpArrow), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
					this.ctキー反復用.R.tキー反復(CDTXMania.app.Pad.b押されているGB(Eパッド.HH), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
					if (CDTXMania.app.Pad.b押された(E楽器パート.DRUMS, Eパッド.SD))
						this.tカーソルを上へ移動する();

					this.ctキー反復用.Down.tキー反復(CDTXMania.app.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.DownArrow), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
					this.ctキー反復用.B.tキー反復(CDTXMania.app.Pad.b押されているGB(Eパッド.BD), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
					if (CDTXMania.app.Pad.b押された(E楽器パート.DRUMS, Eパッド.LT))
						this.tカーソルを下へ移動する();

					if ((CDTXMania.app.Pad.b押されたDGB(Eパッド.CY) || CDTXMania.app.Pad.b押された(E楽器パート.DRUMS, Eパッド.RD)) || (CDTXMania.app.Pad.b押された(E楽器パート.DRUMS, Eパッド.LC) || (CDTXMania.app.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && CDTXMania.app.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return))))
					{
						if ((this.n現在のカーソル行 == (int)E戻り値.GAMESTART - 1) && CDTXMania.app.Skin.soundゲーム開始音.b読み込み成功)
						{
							CDTXMania.app.Skin.soundゲーム開始音.t再生する();
						}
						else
						{
							CDTXMania.app.Skin.sound決定音.t再生する();
						}
						if (this.n現在のカーソル行 == (int)E戻り値.EXIT - 1)
						{
							return (int)E戻り値.EXIT;
						}
						this.actFO.tフェードアウト開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
					}
					//					if ( CDTXMania.app.Input管理.Keyboard.bキーが押された( (int) Key.Space ) )
					//						Trace.TraceInformation( "DTXMania Title: SPACE key registered. " + CDTXMania.app.ct.nシステム時刻 );
				}
				#endregion

				// 描画

				#region [ 背景描画 ]
				if (this.tx背景 != null)
					this.tx背景.t2D描画(CDTXMania.app.Device, 0, 0);
				#endregion
				#region [ メニュー枠描画 ]
				if (this.txメニュー != null)
				{
					int MENU_ITEM_H = txMenuItem[0].szテクスチャサイズ.Height;
					int x = (1920 - txメニュー.szテクスチャサイズ.Width) / 2;	// MENU_X;
					int y = MENU_Y + ((MENU_ITEM_H - MENU_H) / 2) + (this.n現在のカーソル行 * MENU_ITEM_H);
					if (this.ct上移動用.b進行中)
					{
						y += (int)((double)MENU_ITEM_H / 2 * (Math.Cos(Math.PI * (((double)this.ct上移動用.n現在の値) / 100.0)) + 1.0));
					}
					else if (this.ct下移動用.b進行中)
					{
						y -= (int)((double)MENU_ITEM_H / 2 * (Math.Cos(Math.PI * (((double)this.ct下移動用.n現在の値) / 100.0)) + 1.0));
					}
					if (this.ctカーソルフラッシュ用.n現在の値 <= 100)
					{
						float nMag = (float)(1.0 + ((((double)this.ctカーソルフラッシュ用.n現在の値) / 100.0) * 0.5));
						this.txメニュー.vc拡大縮小倍率.X = nMag;
						this.txメニュー.vc拡大縮小倍率.Y = nMag;
						this.txメニュー.n透明度 = (int)(255.0 * (1.0 - (((double)this.ctカーソルフラッシュ用.n現在の値) / 100.0)));
						int x_magnified = x + ((int)((MENU_W * (1.0 - nMag)) / 2.0));
						int y_magnified = y + ((int)((MENU_H * (1.0 - nMag)) / 2.0));
						this.txメニュー.t2D描画(CDTXMania.app.Device, x_magnified, y_magnified, new Rectangle(0, MENU_H * 1, MENU_W, MENU_H));
					}
					this.txメニュー.vc拡大縮小倍率.X = 1f;
					this.txメニュー.vc拡大縮小倍率.Y = 1f;
					this.txメニュー.n透明度 = 0xff;
					this.txメニュー.t2D描画(CDTXMania.app.Device, x, y, new Rectangle(0, MENU_H * 0, MENU_W, MENU_H));
				}
				#endregion

				#region [ メニュー項目描画 ]
				// if ( this.txメニュー != null )
				{
					int offsetY = 0;
					for (int i = 0; i < txMenuItem.Length; i++)
					{
						this.txMenuItem[i].t2D描画(CDTXMania.app.Device, MENU_X + this.stMenuItem[i].offsetX, MENU_Y + offsetY);
						offsetY += this.txMenuItem[i].sz画像サイズ.Height;
					}
				}
				#endregion
				#region [ フェーズ移行処理 ]
				CStage.Eフェーズ eフェーズid = base.eフェーズID;
				switch (eフェーズid)
				{
					case CStage.Eフェーズ.共通_フェードイン:
						if (this.actFI.On進行描画() != 0)
						{
							CDTXMania.app.Skin.soundタイトル音.t再生する();
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_フェードアウト:
						if (this.actFO.On進行描画() == 0)
						{
							break;
						}
						base.eフェーズID = CStage.Eフェーズ.共通_終了状態;
						switch (this.n現在のカーソル行)
						{
							case (int)E戻り値.GAMESTART - 1:
								return (int)E戻り値.GAMESTART;

							case (int)E戻り値.CONFIG - 1:
								return (int)E戻り値.CONFIG;

							case (int)E戻り値.EXIT - 1:
								return (int)E戻り値.EXIT;
							//return ( this.n現在のカーソル行 + 1 );
						}
						break;

					case CStage.Eフェーズ.タイトル_起動画面からのフェードイン:
						if (this.actFIfromSetup.On進行描画() != 0)
						{
							CDTXMania.app.Skin.soundタイトル音.t再生する();
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;
				}
				#endregion
			}
			return 0;
		}
		public enum E戻り値
		{
			継続 = 0,
			GAMESTART,
			//			OPTION,
			CONFIG,
			EXIT
		}


		// その他

		#region [ private ]
		//-----------------
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

		private CActFIFOWhite actFI;
		private CActFIFOWhite actFIfromSetup;
		private CActFIFOWhite actFO;
		private CCounter ctカーソルフラッシュ用;
		private STキー反復用カウンタ ctキー反復用;
		private CCounter ct下移動用;
		private CCounter ct上移動用;
		private int MENU_H = (int)(39 * 2.25);
		private int MENU_W = (int)(227 * 3);
		private int MENU_X = 800;	//206;
		private int MENU_Y = 500;	//203;
		private int n現在のカーソル行;
		private CTexture txメニュー;
		private CTexture tx背景;

		private CPrivateFastFont prvFont;
		private CTexture[] txMenuItem;
		private struct STMenuItem
		{
			public int offsetX;
			public CTexture txMenuItem;
		}
		private STMenuItem[] stMenuItem;

		private void tカーソルを下へ移動する()
		{
			if (this.n現在のカーソル行 != (int)E戻り値.EXIT - 1)
			{
				CDTXMania.app.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行++;
				this.ct下移動用.t開始(0, 100, 1, CDTXMania.app.Timer);
				if (this.ct上移動用.b進行中)
				{
					this.ct下移動用.n現在の値 = 100 - this.ct上移動用.n現在の値;
					this.ct上移動用.t停止();
				}
			}
		}
		private void tカーソルを上へ移動する()
		{
			if (this.n現在のカーソル行 != (int)E戻り値.GAMESTART - 1)
			{
				CDTXMania.app.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行--;
				this.ct上移動用.t開始(0, 100, 1, CDTXMania.app.Timer);
				if (this.ct下移動用.b進行中)
				{
					this.ct上移動用.n現在の値 = 100 - this.ct下移動用.n現在の値;
					this.ct下移動用.t停止();
				}
			}
		}
		//-----------------
		#endregion
	}
}
