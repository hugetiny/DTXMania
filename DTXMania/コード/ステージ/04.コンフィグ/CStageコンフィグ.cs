using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using FDK;

using SlimDXKey = SlimDX.DirectInput.Key;

namespace DTXMania
{

	internal class CStageコンフィグ : CStage
	{
		CActFIFOWhite actFIFO;
		CActConfigKeyAssign actKeyAssign;
		CActConfigList actList;
		CActオプションパネル actオプションパネル;
		bool bメニューにフォーカス中;
		STキー反復用カウンタ ctキー反復用;
		const int DESC_H = 0x80;
		const int DESC_W = 220;
		EItemPanelモード eItemPanelモード;
		//Font ftフォント;
		CPrivateFastFont ftフォント;
		int n現在のメニュー番号;
		CTexture txMenuカーソル;
		CTextureAf tx下部パネル;
		CTextureAf tx上部パネル;
		CTexture tx説明文パネル;
		CTexture tx背景;
		CPrivateFastFont prvFont;
		CTexture[,] txMenuItemLeft;
		STDGBSValue<float> fDisplayLagTimeBaseMs;

		enum EItemPanelモード
		{
			パッド一覧,
			キーコード一覧
		}

		[StructLayout(LayoutKind.Sequential)]
		struct STキー反復用カウンタ
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

		public CStageコンフィグ()
		{
			eステージID = CStage.Eステージ.コンフィグ;
			eフェーズID = CStage.Eフェーズ.共通_通常状態;
			list子Activities.Add(actFIFO = new CActFIFOWhite());
			list子Activities.Add(actList = new CActConfigList());
			list子Activities.Add(actKeyAssign = new CActConfigKeyAssign());
			list子Activities.Add(actオプションパネル =
				new CActオプションパネル(EOptionPanelDirection.Horizontal));
			b活性化してない = true;
		}

		public void tアサイン完了通知()
		{
			this.eItemPanelモード = EItemPanelモード.パッド一覧;
		}

		public void tパッド選択通知(EPad pad)
		{
			this.actKeyAssign.t開始(pad, this.actList.ib現在の選択項目.label);
			this.eItemPanelモード = EItemPanelモード.キーコード一覧;
		}

		public void t項目変更通知()
		{
			this.t説明文パネルに現在選択されている項目の説明を描画する();
		}


		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				Trace.TraceInformation("コンフィグステージを活性化します。");
				Trace.Indent();
				try
				{
					this.n現在のメニュー番号 = 0;

					CResources cr = CDTXMania.Instance.Resources;
					string fontname = cr.Explanation("strCfgConfigurationDescriptionFontFileName");
					string path = Path.Combine(@"Graphics\fonts", fontname);
					this.ftフォント = new CPrivateFastFont(CSkin.Path(path), 17);
					for (int i = 0; i < 4; i++)
					{
						this.ctキー反復用[i] = new CCounter(0, 0, 0, CDTXMania.Instance.Timer);
					}
					this.bメニューにフォーカス中 = true;
					// ここまでOPTIONと共通
					this.eItemPanelモード = EItemPanelモード.パッド一覧;

					fDisplayLagTimeBaseMs = new STDGBSValue<float>();
					for (EPart i = EPart.Drums; i <= EPart.Bass; ++i)
					{
						fDisplayLagTimeBaseMs[i] = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[i] /
								(CDTXMania.Instance.ConfigIni.nScrollSpeed[i] + 1);
					}
					actオプションパネル.Pos = CDTXMania.Instance.Coordinates.OptionPanelSelect;
				}
				finally
				{
					Trace.TraceInformation("コンフィグステージの活性化を完了しました。");
					Trace.Unindent();
				}
				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				Trace.TraceInformation("コンフィグステージを非活性化します。");
				Trace.Indent();
				try
				{
					CDTXMania.Instance.SaveConfig();
					TextureFactory.t安全にDisposeする(ref this.ftフォント);
					for (int i = 0; i < 4; i++)
					{
						this.ctキー反復用[i] = null;
					}

					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums.Value = (int)(fDisplayLagTimeBaseMs.Drums * (CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums + 1));
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar.Value = (int)(fDisplayLagTimeBaseMs.Guitar * (CDTXMania.Instance.ConfigIni.nScrollSpeed.Guitar + 1));
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass.Value = (int)(fDisplayLagTimeBaseMs.Bass * (CDTXMania.Instance.ConfigIni.nScrollSpeed.Bass + 1));

					base.On非活性化();
				}
				catch (UnauthorizedAccessException e)
				{
					Trace.TraceError(e.Message + "ファイルが読み取り専用になっていないか、管理者権限がないと書き込めなくなっていないか等を確認して下さい");
				}
				catch (Exception e)
				{
					Trace.TraceError(e.Message);
				}
				finally
				{
					Trace.TraceInformation("コンフィグステージの非活性化を完了しました。");
					Trace.Unindent();
				}
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.tx背景 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenConfig background.jpg"), false);
				this.tx上部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenConfig header panel.png"), true);
				this.tx下部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenConfig footer panel.png"), true);
				this.txMenuカーソル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenConfig menu cursor.png"), false);

				CResources cr = CDTXMania.Instance.Resources;
				string fontname = cr.Explanation("strCfgConfigurationTopItemsFontFileName");
				string path = Path.Combine(@"Graphics\fonts", fontname);
				prvFont = new CPrivateFastFont(CSkin.Path(path), (int)(18 * Scale.Y));
				string[] strMenuItem = { 
					"strCfgSysMenuDesc",
					"strCfgDrMenuDesc",
					"strCfgGtMenuDesc",
					"strCfgBsMenuDesc",
					"strCfgExitMenuDesc"
				};
				txMenuItemLeft = new CTexture[strMenuItem.Length, 2];
				for (int i = 0; i < strMenuItem.Length; i++)
				{
					Bitmap bmpStr;
					string s = CDTXMania.Instance.Resources.Label( strMenuItem[ i ] );
					bmpStr = prvFont.DrawPrivateFont(s, Color.White, Color.Black);
					txMenuItemLeft[i, 0] = TextureFactory.tテクスチャの生成(bmpStr, false);
					bmpStr.Dispose();
					bmpStr = prvFont.DrawPrivateFont(s, Color.White, Color.Black, Color.Yellow, Color.OrangeRed);
					txMenuItemLeft[i, 1] = TextureFactory.tテクスチャの生成(bmpStr, false);
					bmpStr.Dispose();
				}
				if (this.bメニューにフォーカス中)
				{
					this.t説明文パネルに現在選択されているメニューの説明を描画する();
				}
				else
				{
					this.t説明文パネルに現在選択されている項目の説明を描画する();
				}
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.tx上部パネル);
				TextureFactory.tテクスチャの解放(ref this.tx下部パネル);
				TextureFactory.tテクスチャの解放(ref this.txMenuカーソル);
				TextureFactory.tテクスチャの解放(ref this.tx説明文パネル);
				prvFont.Dispose();
				for (int i = 0; i < txMenuItemLeft.GetLength(0); i++)
				{
					TextureFactory.tテクスチャの解放(ref txMenuItemLeft[i, 0]);
					TextureFactory.tテクスチャの解放(ref txMenuItemLeft[i, 1]);
				}
				txMenuItemLeft = null;
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (base.b初めての進行描画)
				{
					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					this.actFIFO.tフェードイン開始();
					base.b初めての進行描画 = false;
				}

				// 描画
				if (this.tx背景 != null)
				{
					this.tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);
				}

				#region [ メニューカーソル ]
				if (this.txMenuカーソル != null)
				{
					Rectangle rectangle;
					this.txMenuカーソル.n透明度 = this.bメニューにフォーカス中 ? 0xff : 0x80;
					int x = (int)(0x37 * Scale.X);
					int y = (int)((0x60 + (this.n現在のメニュー番号 * 0x19)) * Scale.Y);
					int num3 = (int)(170 * Scale.X);
					this.txMenuカーソル.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(0, 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y)));
					this.txMenuカーソル.t2D描画(CDTXMania.Instance.Device, (x + num3) - 0x10 * Scale.X, y, new Rectangle((int)(0x10 * Scale.X), 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y)));
					x += (int)(0x10 * Scale.X);
					for (num3 -= (int)(0x20 * Scale.X); num3 > 0; num3 -= rectangle.Width)
					{
						rectangle = new Rectangle((int)(8 * Scale.X), 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y));
						if (num3 < (int)(0x10 * Scale.X))
						{
							rectangle.Width -= (int)(0x10 * Scale.X) - num3;
						}
						this.txMenuカーソル.t2D描画(CDTXMania.Instance.Device, x, y, rectangle);
						x += rectangle.Width;
					}
				}
				#endregion

				#region [ メニュー ]
				int menuY = (int)(100 * Scale.Y) - 12;
				int stepY = (int)(25 * Scale.Y);
				for (int i = 0; i < txMenuItemLeft.GetLength(0); i++)
				{
					//Bitmap bmpStr = (this.n現在のメニュー番号 == i) ?
					//      prvFont.DrawPrivateFont( strMenuItem[ i ], Color.White, Color.Black, Color.Yellow, Color.OrangeRed ) :
					//      prvFont.DrawPrivateFont( strMenuItem[ i ], Color.White, Color.Black );
					//txMenuItemLeft = TextureFactory.tテクスチャの生成( bmpStr, false );
					int flag = (this.n現在のメニュー番号 == i) ? 1 : 0;
					int num4 = txMenuItemLeft[i, flag].sz画像サイズ.Width;
					txMenuItemLeft[i, flag].t2D描画(CDTXMania.Instance.Device, 0x8a * Scale.X - (num4 / 2), menuY); //55
																																																				//txMenuItem.Dispose();
					menuY += stepY;
				}
				#endregion

				#region [ 説明文パネル ]
				if (this.tx説明文パネル != null)
				{
					this.tx説明文パネル.t2D描画(CDTXMania.Instance.Device, 0x1d * Scale.X, 0xf8 * Scale.Y);
				}
				#endregion

				#region [ アイテム ]
				switch (this.eItemPanelモード)
				{
					case EItemPanelモード.パッド一覧:
						this.actList.t進行描画(!this.bメニューにフォーカス中);
						break;

					case EItemPanelモード.キーコード一覧:
						this.actKeyAssign.On進行描画();
						break;
				}
				#endregion

				#region [ 上部パネル ]
				if (this.tx上部パネル != null)
				{
					this.tx上部パネル.t2D描画(CDTXMania.Instance.Device, 0, 0);
				}
				#endregion

				#region [ 下部パネル ]
				if (this.tx下部パネル != null)
				{
					this.tx下部パネル.t2D描画(CDTXMania.Instance.Device, 0, SampleFramework.GameWindowSize.Height - this.tx下部パネル.sz画像サイズ.Height);
				}
				#endregion

				#region [ オプションパネル ]
				this.actオプションパネル.On進行描画();
				#endregion

				#region [ フェードイン・アウト ]
				switch (base.eフェーズID)
				{
					case CStage.Eフェーズ.共通_フェードイン:
						if (this.actFIFO.On進行描画() != 0)
						{
							CDTXMania.Instance.Skin.bgmコンフィグ画面.t再生する();
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_フェードアウト:
						if (this.actFIFO.On進行描画() == 0)
						{
							break;
						}
						return 1;
				}
				#endregion

				// キー入力
				#region [ キー入力 ]
				if ((base.eフェーズID != CStage.Eフェーズ.共通_通常状態)
						|| this.actKeyAssign.bキー入力待ちの最中である
						|| CDTXMania.Instance.act現在入力を占有中のプラグイン != null)
				{
					return 0;
				}

				// 曲データの一覧取得中は、キー入力を無効化する
				if (!CDTXMania.Instance.EnumSongs.IsEnumerating || !CDTXMania.Instance.actEnumSongs.bコマンドでの曲データ取得)
				{
					if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.Escape) || CDTXMania.Instance.Pad.bCancelPadIsPressedDGB())
					{
						CDTXMania.Instance.Skin.sound取消音.t再生する();
						if (!this.bメニューにフォーカス中)
						{
							if (this.eItemPanelモード == EItemPanelモード.キーコード一覧)
							{
								CDTXMania.Instance.stageコンフィグ.tアサイン完了通知();
								return 0;
							}
							if (this.actList.b楽器またはシステム用メニューである && !this.actList.bIsFocusingParameter)   // #24525 2011.3.15 yyagi, #32059 2013.9.17 yyagi
							{
								this.bメニューにフォーカス中 = true;
							}
							this.t説明文パネルに現在選択されているメニューの説明を描画する();
							this.actList.tEsc押下();                              // #24525 2011.3.15 yyagi ESC押下時の右メニュー描画用
						}
						else
						{
							this.actFIFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						}
					}
					#region [ ← ]
					else if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.LeftArrow))   // 左カーソルキー
					{
						if (!this.bメニューにフォーカス中)
						{
							if (this.eItemPanelモード == EItemPanelモード.キーコード一覧)
							{
								// キーコンフィグ画面中は、[←]押下に反応させない
								return 0;
							}
							if (this.actList.bIsFocusingParameter)
							{
								// パラメータを増減している最中も、[←]押下に反応させない
								return 0;
							}
							// #24525 2011.3.15 yyagi, #32059 2013.9.17 yyagi
							if (this.actList.b楽器またはシステム用メニューである && !this.actList.bIsFocusingParameter)
							{
								this.bメニューにフォーカス中 = true;
							}
							CDTXMania.Instance.Skin.sound取消音.t再生する();
							this.t説明文パネルに現在選択されているメニューの説明を描画する();
							// #24525 2011.3.15 yyagi ESC押下時の右メニュー描画用
							this.actList.tEsc押下();
						}
					}
					#endregion
					else if (
						(CDTXMania.Instance.Pad.bDecidePadIsPressedDGB() ||
						(CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない &&
						CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.Return))))
					{
						#region [ EXIT ]
						if (this.n現在のメニュー番号 == 4)
						{
							CDTXMania.Instance.Skin.sound決定音.t再生する();
							this.actFIFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						}
						#endregion
						else if (this.bメニューにフォーカス中)
						{
							CDTXMania.Instance.Skin.sound決定音.t再生する();
							this.bメニューにフォーカス中 = false;
							this.t説明文パネルに現在選択されている項目の説明を描画する();
						}
						else
						{
							switch (this.eItemPanelモード)
							{
								case EItemPanelモード.パッド一覧:
									if (this.actList.tEnter押下())
									{
										bメニューにフォーカス中 = true;
									}
									break;

								case EItemPanelモード.キーコード一覧:
									this.actKeyAssign.tEnter押下();
									break;
							}
						}
					}
					#region [ → ]
					else if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.RightArrow))  // 右カーソルキー
					{
						#region [ EXIT ]
						if (this.n現在のメニュー番号 == 4)
						{
							// 何もしない
						}
						#endregion
						else if (this.bメニューにフォーカス中)
						{
							CDTXMania.Instance.Skin.sound決定音.t再生する();
							this.bメニューにフォーカス中 = false;
							this.t説明文パネルに現在選択されている項目の説明を描画する();
						}
					}
					#endregion

					this.ctキー反復用.Up.tキー反復(
						CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.UpArrow ),
						new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );

					this.ctキー反復用.R.tキー反復(
						CDTXMania.Instance.Pad.b押されている( EPad.GtR ) || CDTXMania.Instance.Pad.b押されている( EPad.BsR ) ||
						CDTXMania.Instance.Pad.b押されている( EPad.Up ),
						new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );

					if (CDTXMania.Instance.Pad.b押された(EPad.SD))
					{
						this.tカーソルを上へ移動する();
					}
					this.ctキー反復用.Down.tキー反復(
						CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.DownArrow ),
						new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );

					this.ctキー反復用.B.tキー反復(
						CDTXMania.Instance.Pad.b押されている( EPad.GtB ) || CDTXMania.Instance.Pad.b押されている( EPad.BsB ) ||
						CDTXMania.Instance.Pad.b押されている( EPad.Down ),
						new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );

					if (CDTXMania.Instance.Pad.b押された(EPad.LT))
					{
						this.tカーソルを下へ移動する();
					}
				}
				#endregion
			}
			return 0;
		}


		private void tカーソルを下へ移動する()
		{
			if (!this.bメニューにフォーカス中)
			{
				switch (this.eItemPanelモード)
				{
					case EItemPanelモード.パッド一覧:
						this.actList.OnNext();
						return;

					case EItemPanelモード.キーコード一覧:
						this.actKeyAssign.OnNext();
						return;
				}
			}
			else
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				this.n現在のメニュー番号 = (this.n現在のメニュー番号 + 1) % 5;
				switch (this.n現在のメニュー番号)
				{
					case 0:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.System);
						break;

					case 1:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Drums);
						break;

					case 2:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Guitar);
						break;

					case 3:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Bass);
						break;

					case 4:
						this.actList.t項目リストの設定_Exit();
						break;
				}
				this.t説明文パネルに現在選択されているメニューの説明を描画する();
			}
		}

		private void tカーソルを上へ移動する()
		{
			if (!this.bメニューにフォーカス中)
			{
				switch (this.eItemPanelモード)
				{
					case EItemPanelモード.パッド一覧:
						this.actList.OnPrevious();
						return;

					case EItemPanelモード.キーコード一覧:
						this.actKeyAssign.OnPrevious();
						return;
				}
			}
			else
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				this.n現在のメニュー番号 = ((this.n現在のメニュー番号 - 1) + 5) % 5;
				switch (this.n現在のメニュー番号)
				{
					case 0:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.System);
						break;

					case 1:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Drums);
						break;

					case 2:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Guitar);
						break;

					case 3:
						this.actList.t項目リストの設定(CActConfigList.Eメニュー種別.Bass);
						break;

					case 4:
						this.actList.t項目リストの設定_Exit();
						break;
				}
				this.t説明文パネルに現在選択されているメニューの説明を描画する();
			}
		}

		private void t説明文パネルに現在選択されているメニューの説明を描画する()
		{
			string[] desc = {
				"strCfgSysMenuDesc",
				"strCfgDrMenuDesc",
				"strCfgGtMenuDesc",
				"strCfgBsMenuDesc",
				"strCfgExitMenuDesc"
			};
			string str = CDTXMania.Instance.Resources.Explanation( desc[ this.n現在のメニュー番号 ] );

			if (this.tx説明文パネル != null)
			{
				this.tx説明文パネル.Dispose();
			}
			Bitmap image = ftフォント.DrawPrivateFont(str, Color.White, Color.Black);

			this.tx説明文パネル = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);

			TextureFactory.t安全にDisposeする(ref image);
		}

		private void t説明文パネルに現在選択されている項目の説明を描画する()
		{
			COptionBase item = this.actList.ib現在の選択項目;

			if (string.IsNullOrEmpty(item.explanation)) return;

			if (this.tx説明文パネル != null)
			{
				this.tx説明文パネル.Dispose();
			}
			Bitmap image = ftフォント.DrawPrivateFont(item.explanation, Color.White, Color.Black, new Size((int)(220 * Scale.X), (int)(192 * Scale.Y)));

			this.tx説明文パネル = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);

			TextureFactory.t安全にDisposeする(ref image);
		}
	}
}
