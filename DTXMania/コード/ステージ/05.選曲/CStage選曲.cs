using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using FDK;

using SlimDXKey = SlimDX.DirectInput.Key;

namespace DTXMania
{
	internal class CStage選曲 : CStage
	{
		// プロパティ
		public int nスクロールバー相対y座標
		{
			get
			{
				if (act曲リスト != null)
				{
					return act曲リスト.nスクロールバー相対y座標;
				}
				else
				{
					return 0;
				}
			}
		}
		public bool bIsEnumeratingSongs
		{
			get
			{
				return act曲リスト.bIsEnumeratingSongs;
			}
			set
			{
				act曲リスト.bIsEnumeratingSongs = value;
			}
		}
		public bool bIsPlayingPremovie
		{
			get
			{
				return this.actPreimageパネル.bIsPlayingPremovie;
			}
		}
		public bool bスクロール中
		{
			get
			{
				return this.act曲リスト.bスクロール中;
			}
		}
		public int n確定された曲の難易度
		{
			get;
			private set;
		}
		public Cスコア r確定されたスコア
		{
			get;
			private set;
		}
		public C曲リストノード r確定された曲
		{
			get;
			private set;
		}
		public int n現在選択中の曲の難易度
		{
			get
			{
				return this.act曲リスト.n現在選択中の曲の現在の難易度レベル;
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				return this.act曲リスト.r現在選択中のスコア;
			}
		}
		public C曲リストノード r現在選択中の曲
		{
			get
			{
				return this.act曲リスト.r現在選択中の曲;
			}
		}

		// コンストラクタ
		public CStage選曲()
		{
			eステージID = CStage.Eステージ.選曲;
			eフェーズID = CStage.Eフェーズ.共通_通常状態;
			b活性化してない = true;
			list子Activities.Add(this.actオプションパネル = new CActオプションパネル(EOptionPanelDirection.Horizontal));
			list子Activities.Add(this.actFIFO = new CActFIFOBlack());
			list子Activities.Add(this.actFIfrom結果画面 = new CActFIFOBlack());
			list子Activities.Add(this.act曲リスト = new CActSelect曲リスト());
			list子Activities.Add(this.actステータスパネル = new CActSelectステータスパネル());
			list子Activities.Add(this.act演奏履歴パネル = new CActSelect演奏履歴パネル());
			list子Activities.Add(this.actPreimageパネル = new CActSelectPreimageパネル());
			list子Activities.Add(this.actPresound = new CActSelectPresound());
			list子Activities.Add(this.actArtistComment = new CActSelectArtistComment());
			list子Activities.Add(this.actInformation = new CActSelectInformation());
			list子Activities.Add(this.actSortSongs = new CActSortSongs());
			list子Activities.Add(this.actShowCurrentPosition = new CActSelectShowCurrentPosition());
			list子Activities.Add(this.actQuickConfig = new CActSelectQuickConfig());
			list子Activities.Add(this.actAVI = new CAct演奏AVI());
			list子Activities.Add(this.actShowSongPath = new CActShowSongPath());

			CommandHistory = new CCommandHistory();    // #24063 2011.1.16 yyagi
			actPreimageパネル.actAVI = this.actAVI;
		}


		// メソッド

		public void t選択曲変更通知()
		{
			this.actPreimageパネル.t選択曲が変更された();
			this.actPresound.t選択曲が変更された();
			this.act演奏履歴パネル.t選択曲が変更された();
			this.actステータスパネル.t選択曲が変更された();
			this.actArtistComment.t選択曲が変更された();
			this.actShowSongPath.t選択曲が変更された();

			#region [ プラグインにも通知する（BOX, RANDOM, BACK なら通知しない）]
			//---------------------
			if (CDTXMania.Instance != null)
			{
				var c曲リストノード = CDTXMania.Instance.stage選曲.r現在選択中の曲;
				var cスコア = CDTXMania.Instance.stage選曲.r現在選択中のスコア;

				if (c曲リストノード != null && cスコア != null && c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE)
				{
					string str選択曲ファイル名 = cスコア.ファイル情報.ファイルの絶対パス;
					CSetDef setDef = null;
					int nブロック番号inSetDef = -1;
					int n曲番号inブロック = -1;

					if (!string.IsNullOrEmpty(c曲リストノード.pathSetDefの絶対パス) && File.Exists(c曲リストノード.pathSetDefの絶対パス))
					{
						setDef = new CSetDef(c曲リストノード.pathSetDefの絶対パス);
						nブロック番号inSetDef = c曲リストノード.SetDefのブロック番号;
						n曲番号inブロック = CDTXMania.Instance.stage選曲.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(c曲リストノード);
					}

					foreach (STPlugin stPlugin in CDTXMania.Instance.listプラグイン)
					{
						Directory.SetCurrentDirectory(stPlugin.strプラグインフォルダ);
						stPlugin.plugin.On選択曲変更(str選択曲ファイル名, setDef, nブロック番号inSetDef, n曲番号inブロック);
						Directory.SetCurrentDirectory(CDTXMania.Instance.strEXEのあるフォルダ);
					}
				}
			}
			//---------------------
			#endregion
		}

		// CStage 実装

		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh(CSongs管理 cs, bool bRemakeSongTitleBar)
		{
			this.act曲リスト.Refresh(cs, bRemakeSongTitleBar);
		}

		public override void On活性化()
		{
			Trace.TraceInformation("選曲ステージを活性化します。");
			Trace.Indent();
			try
			{
				this.eフェードアウト完了時の戻り値 = E戻り値.継続;
				this.bBGM再生済み = false;

				for (int i = 0; i < 4; i++)
				{
					this.ctキー反復用[i] = new CCounter(0, 0, 0, CDTXMania.Instance.Timer);
				}
				this.actステータスパネル.t選択曲が変更された();  // 最大ランクを更新
				actオプションパネル.Pos = CDTXMania.Instance.Coordinates.OptionPanelSelect;
				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation("選曲ステージの活性化を完了しました。");
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation("選曲ステージを非活性化します。");
			Trace.Indent();
			try
			{
				for (int i = 0; i < 4; i++)
				{
					this.ctキー反復用[i] = null;
				}
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation("選曲ステージの非活性化を完了しました。");
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx背景 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect background.jpg"), false);
				this.tx上部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenSelect header panel.png"), true);
				this.tx下部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenSelect footer panel.png"), true);
				this.txコメントバー = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenSelect comment bar.png"), true);
				this.txFLIP = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill number on gauge etc.png"), false);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.tx上部パネル);
				TextureFactory.tテクスチャの解放(ref this.tx下部パネル);
				TextureFactory.tテクスチャの解放(ref this.txコメントバー);
				TextureFactory.tテクスチャの解放(ref this.txFLIP);
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
					this.ct登場時アニメ用共通 = new CCounter(0, 100, 3, CDTXMania.Instance.Timer);
					if (CDTXMania.Instance.r直前のステージ == CDTXMania.Instance.stage結果)
					{
						this.actFIfrom結果画面.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.選曲_結果画面からのフェードイン;
					}
					else
					{
						this.actFIFO.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					}
					this.t選択曲変更通知();
					base.b初めての進行描画 = false;
				}
				//---------------------
				#endregion

				this.ct登場時アニメ用共通.t進行();

				if (this.tx背景 != null)
					this.tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);

				this.actPreimageパネル.On進行描画();
				//	this.bIsEnumeratingSongs = !this.actPreimageパネル.bIsPlayingPremovie;				// #27060 2011.3.2 yyagi: #PREMOVIE再生中は曲検索を中断する

				this.act曲リスト.On進行描画();
				int y = 0;
				if (this.ct登場時アニメ用共通.b進行中)
				{
					double db登場割合 = ((double)this.ct登場時アニメ用共通.n現在の値) / 100.0; // 100が最終値
					double dbY表示割合 = Math.Sin(Math.PI / 2 * db登場割合);
					y = ((int)(this.tx上部パネル.sz画像サイズ.Height * dbY表示割合)) - this.tx上部パネル.sz画像サイズ.Height;
				}
				#region [ 上部パネル描画 ]
				if (this.tx上部パネル != null)
					this.tx上部パネル.t2D描画(
						CDTXMania.Instance.Device,
						0,
						y * Scale.Y
					);
				#endregion
				this.actInformation.On進行描画();
				#region [ 下部パネル描画 ]
				if (this.tx下部パネル != null)
					this.tx下部パネル.t2D描画(
						CDTXMania.Instance.Device,
						0,
						SampleFramework.GameWindowSize.Height - this.tx下部パネル.sz画像サイズ.Height
					);
				#endregion
				this.actステータスパネル.On進行描画();
				this.act演奏履歴パネル.On進行描画();
				this.actPresound.On進行描画();
				#region [  コメントバー描画 ]
				if (this.txコメントバー != null)
				{
					this.txコメントバー.t2D描画(
						CDTXMania.Instance.Device,
						0xf2 * Scale.X,
						0xe4 * Scale.Y
					);
				}
				#endregion
				this.actArtistComment.On進行描画();
				this.actオプションパネル.On進行描画();
				#region [ FLIP描画 ]
				if (this.txFLIP != null && CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass) // #24063 2011.1.16 yyagi
				{
					Rectangle rect = new Rectangle(
						(int)(31 * Scale.X),
						(int)(49 * Scale.Y),
						(int)(20 * Scale.X),
						(int)(11 * Scale.Y)
					);
					this.txFLIP.t2D描画(
						CDTXMania.Instance.Device,
						40 * Scale.X,
						436 * Scale.Y,
						rect
					);
				}
				#endregion
				this.actShowCurrentPosition.On進行描画();               // #27648 2011.3.28 yyagi
				this.actShowSongPath.On進行描画();                      // #38404 2018.7.30 yyagi

				#region [ フェーズ処理 ]
				switch (base.eフェーズID)
				{
					case CStage.Eフェーズ.共通_フェードイン:
						if (this.actFIFO.On進行描画() != 0)
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_フェードアウト:
						if (this.actFIFO.On進行描画() == 0)
						{
							break;
						}
						return (int)this.eフェードアウト完了時の戻り値;

					case CStage.Eフェーズ.選曲_結果画面からのフェードイン:
						if (this.actFIfrom結果画面.On進行描画() != 0)
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト:
						//						if( this.actFOtoNowLoading.On進行描画() == 0 )
						//						{
						//							break;
						//						}
						return (int)this.eフェードアウト完了時の戻り値;
				}
				#endregion
				if (!this.bBGM再生済み && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態))
				{
					CDTXMania.Instance.Skin.bgm選曲画面.n音量_次に鳴るサウンド = 100;
					CDTXMania.Instance.Skin.bgm選曲画面.t再生する();
					this.bBGM再生済み = true;
				}


				//Debug.WriteLine( "パンくず=" + this.r現在選択中の曲.strBreadcrumbs );


				// キー入力
				if (base.eフェーズID == CStage.Eフェーズ.共通_通常状態
					&& CDTXMania.Instance.act現在入力を占有中のプラグイン == null)
				{
					#region [ 簡易CONFIGでMore、またはShift+F1: 詳細CONFIG呼び出し ]
					if (actQuickConfig.bGotoDetailConfig)
					{ // 詳細CONFIG呼び出し
						actQuickConfig.tDeativatePopupMenu();
						this.actPresound.tサウンド停止();
						this.eフェードアウト完了時の戻り値 = E戻り値.コンフィグ呼び出し;  // #24525 2011.3.16 yyagi: [SHIFT]-[F1]でCONFIG呼び出し
						this.actFIFO.tフェードアウト開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						CDTXMania.Instance.Skin.sound取消音.t再生する();
						return 0;
					}
					#endregion
					if (!this.actSortSongs.bIsActivePopupMenu && !this.actQuickConfig.bIsActivePopupMenu)
					{
						#region [ ESC ]
						if( CDTXMania.Instance.Input管理.Keyboard.bキーが押された( (int) SlimDXKey.Escape ) ||
							( CDTXMania.Instance.Pad.bCancelPadIsPressedDGB() && ( this.act曲リスト.r現在選択中の曲 != null && this.act曲リスト.r現在選択中の曲.r親ノード == null ) ) )
						{
							CDTXMania.Instance.Skin.sound取消音.t再生する();
							this.eフェードアウト完了時の戻り値 = E戻り値.タイトルに戻る;
							this.actFIFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							return 0;
						}
						#endregion
						#region [ Shift-F1: CONFIG画面 ]
						if ((CDTXMania.Instance.Input管理.Keyboard.bキーが押されている((int)SlimDXKey.RightShift) || CDTXMania.Instance.Input管理.Keyboard.bキーが押されている((int)SlimDXKey.LeftShift)) &&
							CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.F1))
						{
							this.actPresound.tサウンド停止();
							// #24525 2011.3.16 yyagi: [SHIFT]-[F1]でCONFIG呼び出し
							this.eフェードアウト完了時の戻り値 = E戻り値.コンフィグ呼び出し;
							this.actFIFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							CDTXMania.Instance.Skin.sound取消音.t再生する();
							return 0;
						}
						#endregion
						#region [ Shift-F2: 未使用 ]
						// #24525 2011.3.16 yyagi: [SHIFT]+[F2]は廃止(将来発生するかもしれない別用途のためにキープ)
						/*
											if ( ( CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int)SlimDXKey.RightShift ) || CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int)SlimDXKey.LeftShift ) ) &&
												CDTXMania.Instance.Input管理.Keyboard.bキーが押された( (int)SlimDXKey.F2 ) )
											{	// [SHIFT] + [F2] CONFIGURATION
												this.actPresound.tサウンド停止();
												this.eフェードアウト完了時の戻り値 = E戻り値.コンフィグ呼び出し;
												this.actFIFO.tフェードアウト開始();
												base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
												CDTXMania.Instance.Skin.sound取消音.t再生する();
												return 0;
											}
						*/
						#endregion
						if (this.act曲リスト.r現在選択中の曲 != null)
						{
							#region [ Delete: 曲データのフルパス表示ON/OFF ]
							if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.Delete))
							{
								CDTXMania.Instance.ConfigIni.bShowSongPath.Value = !CDTXMania.Instance.ConfigIni.bShowSongPath.Value;
							}
							#endregion
							#region [ Right ]
							if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.RightArrow))
							{
								if (this.act曲リスト.r現在選択中の曲 != null)
								{
									switch (this.act曲リスト.r現在選択中の曲.eノード種別)
									{
										case C曲リストノード.Eノード種別.BOX:
											{
												CDTXMania.Instance.Skin.sound決定音.t再生する();
												bool bNeedChangeSkin = this.act曲リスト.tBOXに入る();
												if (bNeedChangeSkin)
												{
													this.eフェードアウト完了時の戻り値 = E戻り値.スキン変更;
													base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのフェードアウト;
												}
											}
											break;
									}
								}
							}
							#endregion
							#region [ Decide ]
							if (
								CDTXMania.Instance.Pad.bDecidePadIsPressedDGB() ||
								(CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない &&
								CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.Return)))
							{
								CDTXMania.Instance.Skin.sound決定音.t再生する();
								if (this.act曲リスト.r現在選択中の曲 != null)
								{
									switch (this.act曲リスト.r現在選択中の曲.eノード種別)
									{
										case C曲リストノード.Eノード種別.SCORE:
											this.t曲を選択する();
											break;

										case C曲リストノード.Eノード種別.SCORE_MIDI:
											this.t曲を選択する();
											break;

										case C曲リストノード.Eノード種別.BOX:
											{
												bool bNeedChangeSkin = this.act曲リスト.tBOXに入る();
												if (bNeedChangeSkin)
												{
													this.eフェードアウト完了時の戻り値 = E戻り値.スキン変更;
													base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのフェードアウト;
												}
											}
											break;

										case C曲リストノード.Eノード種別.BACKBOX:
											{
												bool bNeedChangeSkin = this.act曲リスト.tBOXを出る();
												if (bNeedChangeSkin)
												{
													this.eフェードアウト完了時の戻り値 = E戻り値.スキン変更;
													base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのフェードアウト;
												}
											}
											break;

										case C曲リストノード.Eノード種別.RANDOM:
											this.t曲をランダム選択する();
											break;
									}
								}
							}
							#endregion
							#region [ Up ]
							this.ctキー反復用.Up.tキー反復(
								CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.UpArrow ),
								new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );

							this.ctキー反復用.R.tキー反復(
								CDTXMania.Instance.Pad.b押されている( EPad.GtR ) || CDTXMania.Instance.Pad.b押されている( EPad.BsR ) ||
								CDTXMania.Instance.Pad.b押されている(EPad.Up),
								new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );

							if (CDTXMania.Instance.Pad.b押された(EPad.SD))
							{
								this.tカーソルを上へ移動する();
							}
							#endregion
							#region [ Down ]
							this.ctキー反復用.Down.tキー反復(
								CDTXMania.Instance.Input管理.Keyboard.bキーが押されている( (int) SlimDXKey.DownArrow ),
								new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );

							this.ctキー反復用.B.tキー反復(
								CDTXMania.Instance.Pad.b押されている( EPad.GtB ) || CDTXMania.Instance.Pad.b押されている( EPad.BsB ) ||
								CDTXMania.Instance.Pad.b押されている(EPad.Down),
								new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );

							if (CDTXMania.Instance.Pad.b押された(EPad.LT))
							{
								this.tカーソルを下へ移動する();
							}
							#endregion
							#region [ Upstairs / Left ]
							if (((this.act曲リスト.r現在選択中の曲 != null) && (this.act曲リスト.r現在選択中の曲.r親ノード != null)) &&
								(CDTXMania.Instance.Pad.bCancelPadIsPressedDGB() ||
									CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDXKey.LeftArrow)))
							{
								this.actPresound.tサウンド停止();
								CDTXMania.Instance.Skin.sound取消音.t再生する();
								bool bNeedChangeSkin = this.act曲リスト.tBOXを出る();
								if (bNeedChangeSkin)
								{
									this.eフェードアウト完了時の戻り値 = E戻り値.スキン変更;
									base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのフェードアウト;
								}
								this.t選択曲変更通知();
							}
							#endregion
							#region [ BDx2: 簡易CONFIG ]
							if (CDTXMania.Instance.Pad.b押された(EPad.BD))
							{ // [BD]x2 スクロール速度変更
								CommandHistory.Add(EPart.Drums, EPadFlag.BD);
								EPadFlag[] comChangeScrollSpeed = new EPadFlag[] { EPadFlag.BD, EPadFlag.BD };
								if (CommandHistory.CheckCommand(comChangeScrollSpeed, EPart.Drums))
								{
									// Debug.WriteLine( "ドラムススクロール速度変更" );
									// CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums = ( CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1 ) % 0x10;
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									this.actQuickConfig.tActivatePopupMenu(EPart.Drums);
								}
							}
							#endregion
							#region [ HHx2: 難易度変更 ]
							if (CDTXMania.Instance.Pad.b押された(EPad.HH) || CDTXMania.Instance.Pad.b押された(EPad.HHO))
							{ // [HH]x2 難易度変更
								CommandHistory.Add(EPart.Drums, EPadFlag.HH);
								EPadFlag[] comChangeDifficulty = new EPadFlag[] { EPadFlag.HH, EPadFlag.HH };
								if (CommandHistory.CheckCommand(comChangeDifficulty, EPart.Drums))
								{
									this.act曲リスト.t難易度レベルをひとつ進める();
									CDTXMania.Instance.Skin.sound変更音.t再生する();
								}
							}
							#endregion
							#region [ G + PickPick Guitar: 難易度変更 ]
							if (CDTXMania.Instance.Pad.b押されている(EPad.GtG) && CDTXMania.Instance.Pad.b押された(EPad.GtPick))  // #24177 2011.1.17 yyagi || -> &&
							{ // [G] + [Pick][Pick] ギター難易度変更
								CommandHistory.Add(EPart.Guitar, EPadFlag.Pick | EPadFlag.G);
								EPadFlag[] comChangeDifficulty = new EPadFlag[] { EPadFlag.Pick | EPadFlag.G, EPadFlag.Pick | EPadFlag.G };
								if (CommandHistory.CheckCommand(comChangeDifficulty, EPart.Guitar))
								{
									Debug.WriteLine("ギター難易度変更");
									this.act曲リスト.t難易度レベルをひとつ進める();
									CDTXMania.Instance.Skin.sound変更音.t再生する();
								}
							}
							#endregion
							#region [ G + PickPick Bass: 難易度変更 ]
							if (CDTXMania.Instance.Pad.b押されている(EPad.BsG) && CDTXMania.Instance.Pad.b押された(EPad.BsPick))    // #24177 2011.1.17 yyagi || -> &&
							{ // [G] + [Pick][Pick] ベース難易度変更
								CommandHistory.Add(EPart.Bass, EPadFlag.Pick | EPadFlag.G);
								EPadFlag[] comChangeDifficulty = new EPadFlag[] { EPadFlag.Pick | EPadFlag.G, EPadFlag.Pick | EPadFlag.G };
								if (CommandHistory.CheckCommand(comChangeDifficulty, EPart.Bass))
								{
									Debug.WriteLine("ベース難易度変更");
									this.act曲リスト.t難易度レベルをひとつ進める();
									CDTXMania.Instance.Skin.sound変更音.t再生する();
								}
							}
							#endregion
							#region [ Pick G G Pick Guitar: ギターとベースを入れ替え ]
							if (CDTXMania.Instance.Pad.b押された(EPad.GtPick) && !CDTXMania.Instance.Pad.b押されている(EPad.GtG))
							{ // ギター[Pick]: コマンドとしてEnqueue
								CommandHistory.Add(EPart.Guitar, EPadFlag.Pick);
								// Pick, G, G, Pick で、ギターとベースを入れ替え
								EPadFlag[] comSwapGtBs1 = new EPadFlag[] { EPadFlag.Pick, EPadFlag.G, EPadFlag.G, EPadFlag.Pick };
								if (CommandHistory.CheckCommand(comSwapGtBs1, EPart.Guitar))
								{
									Debug.WriteLine("ギターとベースの入れ替え1");
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									// ギターとベースのキーを入れ替え
									//CDTXMania.Instance.ConfigIni.SwapGuitarBassKeyAssign();
									CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass = !CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass;
								}
							}
							#endregion
							#region [ Pick G G Pick Bass: ギターとベースを入れ替え ]
							if (CDTXMania.Instance.Pad.b押された(EPad.BsPick) && !CDTXMania.Instance.Pad.b押されている(EPad.BsG))
							{ // ベース[Pick]: コマンドとしてEnqueue
								CommandHistory.Add(EPart.Bass, EPadFlag.Pick);
								// Pick, G, G, Pick で、ギターとベースを入れ替え
								EPadFlag[] comSwapGtBs1 = new EPadFlag[] { EPadFlag.Pick, EPadFlag.G, EPadFlag.G, EPadFlag.Pick };
								if (CommandHistory.CheckCommand(comSwapGtBs1, EPart.Bass))
								{
									Debug.WriteLine("ギターとベースの入れ替え2");
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									// ギターとベースのキーを入れ替え
									//CDTXMania.Instance.ConfigIni.SwapGuitarBassKeyAssign();
									CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass = !CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass;
								}
							}
							#endregion
							#region [ G G G Guitar: ソート画面 ]
							if (CDTXMania.Instance.Pad.b押された(EPad.GtG))
							{ // ギター[G]: コマンドとしてEnqueue
								CommandHistory.Add(EPart.Guitar, EPadFlag.G);
								// ギター G, G, G で、ソート画面に遷移
								EPadFlag[] comSortGt = new EPadFlag[] { EPadFlag.G, EPadFlag.G, EPadFlag.G };
								if (CommandHistory.CheckCommand(comSortGt, EPart.Guitar))
								{
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									this.actSortSongs.tActivatePopupMenu(EPart.Guitar, ref this.act曲リスト);
								}
							}
							#endregion
							#region [ G G G Bass: ソート画面 ]
							if (CDTXMania.Instance.Pad.b押された(EPad.BsG))
							{ // ベース[G]: コマンドとしてEnqueue
								CommandHistory.Add(EPart.Bass, EPadFlag.G);
								// ベース G, G, G で、ソート画面に遷移
								EPadFlag[] comSortBs = new EPadFlag[] { EPadFlag.G, EPadFlag.G, EPadFlag.G };
								if (CommandHistory.CheckCommand(comSortBs, EPart.Bass))
								{
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									this.actSortSongs.tActivatePopupMenu(EPart.Bass, ref this.act曲リスト);
								}
							}
							#endregion
							#region [ BD HT Drums: ソート画面 ]
							if (CDTXMania.Instance.Pad.b押された(EPad.HT))
							{ // [BD]+[HT] 未使用
								//
								CommandHistory.Add(EPart.Drums, EPadFlag.HT);
								EPadFlag[] comSort = new EPadFlag[] { EPadFlag.BD, EPadFlag.HT };
								if (CommandHistory.CheckCommand(comSort, EPart.Drums))
								{
									CDTXMania.Instance.Skin.sound変更音.t再生する();
									this.actSortSongs.tActivatePopupMenu(EPart.Drums, ref this.act曲リスト);
								}
							}
							#endregion
						}
					}
					this.actSortSongs.t進行描画();
					this.actQuickConfig.t進行描画();
				}
			}
			return 0;
		}
		public enum E戻り値 : int
		{
			継続,
			タイトルに戻る,
			選曲した,
			オプション呼び出し,
			コンフィグ呼び出し,
			スキン変更
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
		private CActSelectArtistComment actArtistComment;
		private CActFIFOBlack actFIFO;
		private CActFIFOBlack actFIfrom結果画面;
		//		private CActFIFOBlack actFOtoNowLoading;	// #27787 2012.3.10 yyagi 曲決定時の画面フェードアウトの省略
		private CActSelectInformation actInformation;
		private CActSelectPreimageパネル actPreimageパネル;
		private CActSelectPresound actPresound;
		private CActオプションパネル actオプションパネル;
		private CActSelectステータスパネル actステータスパネル;
		private CActSelect演奏履歴パネル act演奏履歴パネル;
		private CActSelect曲リスト act曲リスト;
		private CActSelectShowCurrentPosition actShowCurrentPosition;

		private CActSortSongs actSortSongs;
		private CActSelectQuickConfig actQuickConfig;
		private CAct演奏AVI actAVI;
		private CActShowSongPath actShowSongPath;

		private bool bBGM再生済み;
		private STキー反復用カウンタ ctキー反復用;
		private CCounter ct登場時アニメ用共通;
		private E戻り値 eフェードアウト完了時の戻り値;
		private CTextureAf txコメントバー;
		private CTextureAf tx下部パネル;
		private CTextureAf tx上部パネル;
		private CTexture tx背景;
		private CTexture txFLIP;

		private struct STCommandTime    // #24063 2011.1.16 yyagi コマンド入力時刻の記録用
		{
			public EPart eInst;   // 使用楽器
			public EPadFlag ePad;   // 押されたコマンド(同時押しはOR演算で列挙する)
			public long time;       // コマンド入力時刻
		}
		private class CCommandHistory   // #24063 2011.1.16 yyagi コマンド入力履歴を保持・確認するクラス
		{
			readonly int buffersize = 16;
			private List<STCommandTime> stct;

			public CCommandHistory()    // コンストラクタ
			{
				stct = new List<STCommandTime>(buffersize);
			}

			/// <summary>
			/// コマンド入力履歴へのコマンド追加
			/// </summary>
			/// <param name="_eInst">楽器の種類</param>
			/// <param name="_ePad">入力コマンド(同時押しはOR演算で列挙すること)</param>
			public void Add(EPart _eInst, EPadFlag _ePad)
			{
				STCommandTime _stct = new STCommandTime
				{
					eInst = _eInst,
					ePad = _ePad,
					time = CDTXMania.Instance.Timer.n現在時刻
				};

				if (stct.Count >= buffersize)
				{
					stct.RemoveAt(0);
				}
				stct.Add(_stct);
				//Debug.WriteLine( "CMDHIS: 楽器=" + _stct.eInst + ", CMD=" + _stct.ePad + ", time=" + _stct.time );
			}
			public void RemoveAt(int index)
			{
				stct.RemoveAt(index);
			}

			/// <summary>
			/// コマンド入力に成功しているか調べる
			/// </summary>
			/// <param name="_ePad">入力が成功したか調べたいコマンド</param>
			/// <param name="_eInst">対象楽器</param>
			/// <returns>コマンド入力成功時true</returns>
			public bool CheckCommand(EPadFlag[] _ePad, EPart _eInst)
			{
				int targetCount = _ePad.Length;
				int stciCount = stct.Count;
				if (stciCount < targetCount)
				{
					//Debug.WriteLine("NOT start checking...stciCount=" + stciCount + ", targetCount=" + targetCount);
					return false;
				}

				long curTime = CDTXMania.Instance.Timer.n現在時刻;
				//Debug.WriteLine("Start checking...targetCount=" + targetCount);
				for (int i = targetCount - 1, j = stciCount - 1; i >= 0; i--, j--)
				{
					if (_ePad[i] != stct[j].ePad)
					{
						//Debug.WriteLine( "CMD解析: false targetCount=" + targetCount + ", i=" + i + ", j=" + j + ": ePad[]=" + _ePad[i] + ", stci[j] = " + stct[j].ePad );
						return false;
					}
					if (stct[j].eInst != _eInst)
					{
						//Debug.WriteLine( "CMD解析: false " + i );
						return false;
					}
					if (curTime - stct[j].time > 500)
					{
						//Debug.WriteLine( "CMD解析: false " + i + "; over 500ms" );
						return false;
					}
					curTime = stct[j].time;
				}

				//Debug.Write( "CMD解析: 成功!(" + _ePad.Length + ") " );
				//for ( int i = 0; i < _ePad.Length; i++ ) Debug.Write( _ePad[ i ] + ", " );
				//Debug.WriteLine( "" );
				//stct.RemoveRange( 0, targetCount );			// #24396 2011.2.13 yyagi 
				stct.Clear();                 // #24396 2011.2.13 yyagi Clear all command input history in case you succeeded inputting some command

				return true;
			}
		}
		private CCommandHistory CommandHistory;

		private void tカーソルを下へ移動する()
		{
			CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t次に移動();
		}
		private void tカーソルを上へ移動する()
		{
			CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t前に移動();
		}
		private void t曲をランダム選択する()
		{
			C曲リストノード song = this.act曲リスト.r現在選択中の曲;
			if ((song.stackランダム演奏番号.Count == 0) || (song.listランダム用ノードリスト == null))
			{
				if (song.listランダム用ノードリスト == null)
				{
					song.listランダム用ノードリスト = this.t指定された曲が存在する場所の曲を列挙する_子リスト含む(song);
				}
				int count = song.listランダム用ノードリスト.Count;
				if (count == 0)
				{
					return;
				}
				int[] numArray = new int[count];
				for (int i = 0; i < count; i++)
				{
					numArray[i] = i;
				}
				for (int j = 0; j < (count * 1.5); j++)
				{
					int index = CDTXMania.Instance.Random.Next(count);
					int num5 = CDTXMania.Instance.Random.Next(count);
					int num6 = numArray[num5];
					numArray[num5] = numArray[index];
					numArray[index] = num6;
				}
				for (int k = 0; k < count; k++)
				{
					song.stackランダム演奏番号.Push(numArray[k]);
				}
				if (CDTXMania.Instance.ConfigIni.bLogDTX)
				{
					StringBuilder builder = new StringBuilder(0x400);
					builder.Append(string.Format("ランダムインデックスリストを作成しました: {0}曲: ", song.stackランダム演奏番号.Count));
					for (int m = 0; m < count; m++)
					{
						builder.Append(string.Format("{0} ", numArray[m]));
					}
					Trace.TraceInformation(builder.ToString());
				}
			}
			this.r確定された曲 = song.listランダム用ノードリスト[song.stackランダム演奏番号.Pop()];
			this.n確定された曲の難易度 = this.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r確定された曲);
			this.r確定されたスコア = this.r確定された曲.arスコア[this.n確定された曲の難易度];
			this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
			//	this.actFOtoNowLoading.tフェードアウト開始();					// #27787 2012.3.10 yyagi 曲決定時の画面フェードアウトの省略
			base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			if (CDTXMania.Instance.ConfigIni.bLogDTX)
			{
				int[] numArray2 = song.stackランダム演奏番号.ToArray();
				StringBuilder builder2 = new StringBuilder(0x400);
				builder2.Append("ランダムインデックスリスト残り: ");
				if (numArray2.Length > 0)
				{
					for (int n = 0; n < numArray2.Length; n++)
					{
						builder2.Append(string.Format("{0} ", numArray2[n]));
					}
				}
				else
				{
					builder2.Append("(なし)");
				}
				Trace.TraceInformation(builder2.ToString());
			}
			CDTXMania.Instance.Skin.bgm選曲画面.t停止する();
		}
		private void t曲を選択する()
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度 = this.act曲リスト.n現在選択中の曲の現在の難易度レベル;
			if ((this.r確定された曲 != null) && (this.r確定されたスコア != null) && (File.Exists(this.r確定されたスコア.ファイル情報.ファイルの絶対パス)) )
			{
				this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
				//	this.actFOtoNowLoading.tフェードアウト開始();				// #27787 2012.3.10 yyagi 曲決定時の画面フェードアウトの省略
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			}
			CDTXMania.Instance.Skin.bgm選曲画面.t停止する();
		}
		private List<C曲リストノード> t指定された曲が存在する場所の曲を列挙する_子リスト含む(C曲リストノード song)
		{
			List<C曲リストノード> list = new List<C曲リストノード>();
			song = song.r親ノード;
			if ((song == null) && (CDTXMania.Instance.Songs管理.list曲ルート.Count > 0))
			{
				foreach (C曲リストノード c曲リストノード in CDTXMania.Instance.Songs管理.list曲ルート)
				{
					if ((c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE) || (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI))
					{
						list.Add(c曲リストノード);
					}
					if ((c曲リストノード.list子リスト != null) && CDTXMania.Instance.ConfigIni.bRandSubBox)
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む(c曲リストノード, ref list);
					}
				}
				return list;
			}
			this.t指定された曲の子リストの曲を列挙する_孫リスト含む(song, ref list);
			return list;
		}
		private void t指定された曲の子リストの曲を列挙する_孫リスト含む(C曲リストノード r親, ref List<C曲リストノード> list)
		{
			if ((r親 != null) && (r親.list子リスト != null))
			{
				foreach (C曲リストノード c曲リストノード in r親.list子リスト)
				{
					if ((c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE) || (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI))
					{
						list.Add(c曲リストノード);
					}
					if ((c曲リストノード.list子リスト != null) && CDTXMania.Instance.ConfigIni.bRandSubBox)
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む(c曲リストノード, ref list);
					}
				}
			}
		}
		//-----------------
		#endregion
	}
}
