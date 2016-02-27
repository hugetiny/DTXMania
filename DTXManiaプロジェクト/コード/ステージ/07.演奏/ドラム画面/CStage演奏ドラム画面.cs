using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CStage演奏ドラム画面 : CStage演奏画面共通
	{
		// コンストラクタ

		public CStage演奏ドラム画面()
		{
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actPad = new CAct演奏Drumsパッド());
			base.list子Activities.Add(this.actCombo = new CAct演奏DrumsコンボDGB());
			base.list子Activities.Add(this.actDANGER = new CAct演奏DrumsDanger());
			base.list子Activities.Add(this.actChipFireD = new CAct演奏DrumsチップファイアD());
			base.list子Activities.Add(this.actChipFireGB = new CAct演奏DrumsチップファイアGB());
			base.list子Activities.Add(this.actGauge = new CAct演奏Drumsゲージ());
			base.list子Activities.Add(this.actGraph = new CAct演奏Drumsグラフ()); // #24074 2011.01.23 add ikanick
			base.list子Activities.Add(this.actJudgeString = new CAct演奏Drums判定文字列());
			base.list子Activities.Add(this.actLaneFlushD = new CAct演奏DrumsレーンフラッシュD());
			base.list子Activities.Add(this.actLaneFlushGB = new CAct演奏DrumsレーンフラッシュGB());
			base.list子Activities.Add(this.actRGB = new CAct演奏DrumsRGB());
			base.list子Activities.Add(this.actScore = new CAct演奏Drumsスコア());
			base.list子Activities.Add(this.actStatusPanels = new CAct演奏Drumsステータスパネル());
			base.list子Activities.Add(this.actWailingBonus = new CAct演奏DrumsWailingBonus());
			base.list子Activities.Add(this.act譜面スクロール速度 = new CAct演奏スクロール速度());
			base.list子Activities.Add(this.actAVI = new CAct演奏AVI());
			base.list子Activities.Add(this.actBGA = new CAct演奏BGA());
			base.list子Activities.Add(this.actPanel = new CAct演奏パネル文字列());
			base.list子Activities.Add(this.actStageFailed = new CAct演奏ステージ失敗());
			base.list子Activities.Add(this.actPlayInfo = new CAct演奏演奏情報());
			base.list子Activities.Add(this.actFI = new CActFIFOBlack());
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actFOClear = new CActFIFOWhite());
		}

		// CStage 実装
		public override void On活性化()
		{
			this.bフィルイン中 = false;
			base.On活性化();

			// MODIFY_BEGIN #25398 2011.06.07 FROM
			if (CDTXMania.Instance.bコンパクトモード)
			{
				var score = new Cスコア();
				CDTXMania.Instance.Songs管理.tScoreIniを読み込んで譜面情報を設定する(CDTXMania.Instance.strコンパクトモードファイル + ".score.ini", ref score);
				this.actGraph.dbグラフ値目標_渡 = score.譜面情報.最大スキル[0];
			}
			else
			{
				this.actGraph.dbグラフ値目標_渡 = CDTXMania.Instance.stage選曲.r確定されたスコア.譜面情報.最大スキル[0];	// #24074 2011.01.23 add ikanick

				// #35411 2015.08.21 chnmr0 add
				// ゴースト利用可のなとき、0で初期化
				if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums != ETargetGhostData.NONE)
				{
					if (CDTXMania.Instance.DTX.listTargetGhsotLag[(int)E楽器パート.DRUMS] != null)
					{
						this.actGraph.dbグラフ値目標_渡 = 0;
					}
				}
			}
			// MODIFY_END #25398
			dtLastQueueOperation = DateTime.MinValue;
		}
		public override void On非活性化()
		{
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				//this.t背景テクスチャの生成();
				this.txチップ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums chips.png"));
				this.txヒットバー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar.png"));
				this.txヒットバーGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar guitar.png"));
				//this.txWailing枠 = TextureFactory.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay wailing cursor.png" ) );
				this.txレーンフレームGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums lane parts guitar.png"));
				if (this.txレーンフレームGB != null)
				{
					this.txレーンフレームGB.n透明度 = 0xff - CDTXMania.Instance.ConfigIni.n背景の透過度;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txヒットバー);
				TextureFactory.tテクスチャの解放(ref this.txヒットバーGB);
				TextureFactory.tテクスチャの解放(ref this.txチップ);
				TextureFactory.tテクスチャの解放(ref this.txレーンフレームGB);
				base.OnManagedリソースの解放();
			}
		}
		// その他
		#region [ private ]
		//-----------------

		private readonly Eパッド[] eチャンネルtoパッド = new Eパッド[]
		{
			Eパッド.HH, Eパッド.SD, Eパッド.BD, Eパッド.HT,
			Eパッド.LT, Eパッド.CY, Eパッド.FT, Eパッド.HHO,
			Eパッド.RD, Eパッド.LC
		};
		private readonly int[,] nチャンネルtoX座標 = new int[,] {
				{ 76 * 3, 110 * 3, 145 * 3, 192 * 3, 226 * 3, 294 * 3, 260 * 3, 79 * 3, 300 * 3, 35 * 3 },
				// 619 - 35 * 3= 584
				{
				 76 * 3 * 3 / 4 + 548,
				110 * 3 * 3 / 4  + 548,
				145 * 3 * 3 / 4  + 548,
				192 * 3 * 3 / 4  + 548,
				226 * 3 * 3 / 4  + 548,
				294 * 3 * 3 / 4  + 548,
				260 * 3 * 3 / 4  + 548,
				 79 * 3 * 3 / 4  + 548,
				300 * 3 * 3 / 4  + 548,
				 35 * 3 * 3 / 4  + 548
				}
		};
		private CTexture txヒットバーGB;
		//-----------------
		protected override void t進行描画_ギターベース判定ライン()		// yyagi: ギタレボモードとは座標が違うだけですが、まとめづらかったのでそのまま放置してます。
		{
			if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				if (CDTXMania.Instance.DTX.bチップがある.Guitar)
				{
					int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1527 : 1456;
					int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, bReverse[(int)E楽器パート.GUITAR], false, true) - (int)(3 * Scale.Y);
					// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					Rectangle rc = new Rectangle(0, 0, 30, 36);
					if (this.txヒットバーGB != null)
					{
						for (int i = 0; i < 3; i++)
						{
							this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
								x + (3 * 26 * i),
								y
							);
							this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
								x + (3 * 26 * i) + 48,
								y,
								rc
							);
						}
					}
				}
				if (CDTXMania.Instance.DTX.bチップがある.Bass)
				{
					int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1200 : 206;
					int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, bReverse[(int)E楽器パート.BASS], false, true) - (int)(3 * Scale.Y);
					// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					Rectangle rc = new Rectangle(0, 0, 30, 36);
					if (this.txヒットバーGB != null)
					{
						for (int j = 0; j < 3; j++)
						{
							this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
								x + (3 * 26 * j),
								y
							);
							this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
								x + (3 * 26 * j) + 48,
								y,
								rc
							);
						}
					}
				}
			}
		}

		protected override void t背景テクスチャの生成()
		{
			Rectangle bgrect;
			if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)
			{
				bgrect = new Rectangle((int)(338 * Scale.X), (int)(57 * Scale.Y), (int)(278 * 2), (int)(355 * 2));
			}
			else
			{
				bgrect = new Rectangle(619 + 682, (int)(57 * Scale.Y), (int)(278 * 2), (int)(355 * 2));
			}
			string DefaultBgFilename = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ?
										@"Graphics\ScreenPlayDrums background.png" :
										@"Graphics\ScreenPlayDrums-background-center.png";
			string DefaultLaneFilename = "";	//  @"Graphics\ScreenPlayDrums_Lane_parts_drums.png";
			string BgFilename = "";
			if (((CDTXMania.Instance.DTX.BACKGROUND != null) && (CDTXMania.Instance.DTX.BACKGROUND.Length > 0)) && !CDTXMania.Instance.ConfigIni.bストイックモード)
			{
				BgFilename = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.BACKGROUND;
			}
			base.t背景テクスチャの生成(DefaultBgFilename, DefaultLaneFilename, bgrect, BgFilename);
		}

		protected override void t進行描画_チップ_ドラムス(ref CChip pChip)
		{
			if (CDTXMania.Instance.ConfigIni.bDrums有効)
			{
				#region [ Invisible処理 ]
				if (CDTXMania.Instance.ConfigIni.eInvisible.Drums != EInvisible.OFF)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				#endregion
				else
				{
					#region [ Sudden処理 ]
					if (CDTXMania.Instance.ConfigIni.bSudden.Drums)
					{
						if (pChip.nバーからの距離dot.Drums < 200 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff;
						}
						else if (pChip.nバーからの距離dot.Drums < 250 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff - ((int)((((double)(pChip.nバーからの距離dot.Drums - 200 * Scale.Y)) * 255.0) / 50.0));
						}
						else
						{
							pChip.b可視 = false;
							pChip.n透明度 = 0;
						}
					}
					#endregion
					#region [ Hidden処理 ]
					if (CDTXMania.Instance.ConfigIni.bHidden.Drums)
					{
						if (pChip.nバーからの距離dot.Drums < 100 * Scale.Y)
						{
							pChip.b可視 = false;
						}
						else if (pChip.nバーからの距離dot.Drums < 150 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = (int)((((double)(pChip.nバーからの距離dot.Drums - 100 * Scale.Y)) * 255.0) / 50.0);
						}
					}
					#endregion
				}
				if (!pChip.bHit && pChip.b可視)
				{
					if (this.txチップ != null)
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int x = this.nチャンネルtoX座標[(int)CDTXMania.Instance.ConfigIni.eドラムレーン表示位置, pChip.eチャンネル番号 - Ech定義.HiHatClose];
					int y = CDTXMania.Instance.ConfigIni.bReverse.Drums ?
						(int)(0x38 * Scale.Y + pChip.nバーからの距離dot.Drums) :
						(int)(0x1a6 * Scale.Y - pChip.nバーからの距離dot.Drums);

					if (this.txチップ != null)
					{
						double d = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1.0 : 0.75;
						this.txチップ.vc拡大縮小倍率 = new Vector3(
														(float)(pChip.dbチップサイズ倍率 * d),
														(float)pChip.dbチップサイズ倍率,
														1f
													);
					}
					int num9 = this.ctチップ模様アニメ.Drums.n現在の値;
					switch (pChip.eチャンネル番号)
					{
						// HH
						case Ech定義.HiHatClose:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(44 * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(32 * Scale.X),
										(int)(18)
									)
								);
							}
							break;

						// SD
						case Ech定義.Snare:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0x4c * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x20 * Scale.X),
										(int)(18)));
							}
							break;

						// BD
						case Ech定義.BassDrum:
							x += (int)(0x16 * Scale.X) - ((int)((44.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 11,
									new Rectangle(
										(int)(0 * Scale.X),
										(int)(num9 * (22 - 2)),
										(int)(44 * Scale.X),
										(int)(22)
									)
								);
							}
							break;

						// HT
						case Ech定義.HighTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0x6c * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x20 * Scale.X),
										(int)(18)));
							}
							break;

						// LT
						case Ech定義.LowTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(140 * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x20 * Scale.X),
										(int)(18)));
							}
							break;

						// CY
						case Ech定義.Cymbal:
							x += (int)(0x13 * Scale.X) - ((int)((38.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 46,
									new Rectangle(
										(int)(0xcc * Scale.X),
										(int)(786),
										(int)(0x26 * Scale.X),
										(int)(78)));

								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0xcc * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x26 * Scale.X),
										(int)(18)));
							}
							break;

						// FT
						case Ech定義.FloorTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0xac * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x20 * Scale.X),
										(int)(18)));
							}
							break;

						// HHO
						case Ech定義.HiHatOpen:
							x += (int)(13 * Scale.X) - ((int)((26.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0xf2 * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x1a * Scale.X),
										(int)(18)
										));
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 16,
									new Rectangle(
										(int)(0xf2 * Scale.X),
										(int)(790),
										(int)(0x1a * Scale.X),
										(int)(30)
										));
							}
							break;

						// RCY
						case Ech定義.RideCymbal:
							x += (int)(13 * Scale.X) - ((int)((26.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0xf2 * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x1a * Scale.X),
										(int)(18)
										));
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 16,
									new Rectangle(
										(int)(0xf2 * Scale.X),
										(int)(790),
										(int)(0x1a * Scale.X),
										(int)(30)
										));
							}
							break;

						// LCY
						case Ech定義.LeftCymbal:
							x += (int)(0x13 * Scale.X) - ((int)((38.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 46,
									new Rectangle(
										(int)(0xcc * Scale.X),
										(int)(786),
										(int)(0x26 * Scale.X),
										(int)(78)
										));
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x,
									y - 9,
									new Rectangle(
										(int)(0xcc * Scale.X),
										(int)(num9 * (18 - 2)),
										(int)(0x26 * Scale.X),
										(int)(18)
										));
							}
							break;
					}
					if (this.txチップ != null)
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
						this.txチップ.n透明度 = 0xff;
					}
				}

				int indexSevenLanes = CStage演奏画面共通.nチャンネル0Atoレーン07[pChip.eチャンネル番号 - Ech定義.HiHatClose];
				// #35411 chnmr0 modified
				bool autoPlayCondition = (CDTXMania.Instance.ConfigIni.bAutoPlay[indexSevenLanes] && !pChip.bHit);
				bool UsePerfectGhost = true;
				long ghostLag = 0;

				if (CDTXMania.Instance.ConfigIni.eAutoGhost.Drums != EAutoGhostData.PERFECT &&
						CDTXMania.Instance.DTX.listAutoGhostLag.Drums != null &&
						0 <= pChip.n楽器パートでの出現順 && pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag.Drums.Count)
				{
					// ゴーストデータが有効 : ラグに合わせて判定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag.Drums[pChip.n楽器パートでの出現順];
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= this.nInputAdjustTimeMs.Drums;
					autoPlayCondition &= !pChip.bHit && (ghostLag + pChip.n発声時刻ms <= CSound管理.rc演奏用タイマ.n現在時刻ms);
					UsePerfectGhost = false;
				}
				if (UsePerfectGhost)
				{
					// 従来の AUTO : バー下で判定
					autoPlayCondition &= (pChip.nバーからの距離dot.Drums < 0);
				}

				if (autoPlayCondition)
				{
					pChip.bHit = true;
					this.actLaneFlushD.Start((Eレーン)indexSevenLanes, ((float)CInput管理.n通常音量) / 127f);
					bool flag = this.bフィルイン中;
					bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである(pChip);
					//bool flag3 = flag2;
					// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
					this.actChipFireD.Start((Eレーン)indexSevenLanes, flag, flag2, flag2, 演奏判定ライン座標.nJudgeLinePosY_delta.Drums);
					this.actPad.Hit(this.nチャンネル0Atoパッド08[pChip.eチャンネル番号 - Ech定義.HiHatClose]);
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
					this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
					cInvisibleChip.StartSemiInvisible(E楽器パート.DRUMS);
				}
				// #35411 modify end

				// #35411 2015.08.21 chnmr0 add
				// 目標値グラフにゴーストの達成率を渡す
				if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums != ETargetGhostData.NONE &&
						CDTXMania.Instance.DTX.listTargetGhsotLag.Drums != null)
				{
					double val = 0;
					if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums == ETargetGhostData.ONLINE)
					{
						if (CDTXMania.Instance.DTX.n可視チップ数.Drums > 0)
						{
							// Online Stats の計算式
							val = 100 *
									(this.nヒット数_TargetGhost.Drums.Perfect * 17 +
									 this.nヒット数_TargetGhost.Drums.Great * 7 +
									 this.n最大コンボ数_TargetGhost.Drums * 3) / (20.0 * CDTXMania.Instance.DTX.n可視チップ数.Drums);
						}
					}
					else
					{
						val = CScoreIni.t演奏型スキルを計算して返す(
								CDTXMania.Instance.DTX.n可視チップ数.Drums,
								this.nヒット数_TargetGhost.Drums.Perfect,
								this.nヒット数_TargetGhost.Drums.Great,
								this.nヒット数_TargetGhost.Drums.Good,
								this.nヒット数_TargetGhost.Drums.Poor,
								this.nヒット数_TargetGhost.Drums.Miss,
								E楽器パート.DRUMS, new STAUTOPLAY());
					}
					if (val < 0) val = 0;
					if (val > 100) val = 100;
					this.actGraph.dbグラフ値目標_渡 = val;
				}
				//break;
				return;
			}	// end of "if configIni.bDrums有効"
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
				pChip.bHit = true;
			}
		}
		protected override void t進行描画_チップ_ギターベース(ref CChip pChip, E楽器パート inst)
		{
			int xGtO = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 509 : 485;
			int xBsO = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 400 : 69;
			int xGtL = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 561 : 537;
			int xBsL = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 452 : 121;

			base.t進行描画_チップ_ギターベース(ref pChip, inst,
				演奏判定ライン座標.n判定ラインY座標(inst, false),		// 95  Normal
				演奏判定ライン座標.n判定ラインY座標(inst, true),		// 374 Reverse
				(int)(57 * Scale.Y), (int)(412 * Scale.Y),				// チップのY軸表示範囲
				xGtO, xBsO,					// openチップのX座標(Gt, Bs)
				268, 144, 76, 6, 24,		// オープンチップregionの x, y, w, h, 通常チップのw
				xGtO, xGtL, xBsO, xBsL,		// GtのX, Gt左利きのX, BsのX, Bs左利きのX,
				26, 24						// 描画のX座標間隔, テクスチャのX座標間隔
			);
		}
		protected override void t進行描画_チップ_フィルイン(ref CChip pChip)
		{
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
				switch (pChip.n整数値)
				{
					case 0x01:	// フィルイン開始
						if (CDTXMania.Instance.ConfigIni.bフィルイン有効)
						{
							this.bフィルイン中 = true;
						}
						break;

					case 0x02:	// フィルイン終了
						if (CDTXMania.Instance.ConfigIni.bフィルイン有効)
						{
							this.bフィルイン中 = false;
						}
						if (((this.actCombo.n現在のコンボ数.Drums > 0) || CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである) && CDTXMania.Instance.ConfigIni.b歓声を発声する)
						{
							if (this.r現在の歓声Chip.Drums != null)
							{
								CDTXMania.Instance.DTX.tチップの再生(this.r現在の歓声Chip.Drums, CSound管理.rc演奏用タイマ.nシステム時刻, (int)Eレーン.BGM, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
							}
							else
							{
								CDTXMania.Instance.Skin.sound歓声音.n位置_次に鳴るサウンド = 0;
								CDTXMania.Instance.Skin.sound歓声音.t再生する();
							}
						}
						break;
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi TEST
								case 0x04:	// HH消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = true;
									break;
								case 0x05:	// HH消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = false;
									break;
								case 0x06:	// ギター消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Gutiar = true;
									break;
								case 0x07:	// ギター消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Gutiar = false;
									break;
								case 0x08:	// ベース消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = true;
									break;
								case 0x09:	// ベース消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = false;
									break;
#endif
				}
			}
		}
		
		protected override void t進行描画_チップ_空打ち音設定_ドラム(ref CChip pChip)
		{
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
				this.r現在の空うちドラムChip[(int)this.eチャンネルtoパッド[pChip.eチャンネル番号 - Ech定義.HiHatClose]] = pChip;
			}
		}
		#endregion
	}
}
