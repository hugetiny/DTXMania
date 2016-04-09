using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using FDK;
using SlimDX.Direct3D9;

namespace DTXMania
{
	internal class CStage結果 : CStage
	{
		public enum E戻り値 : int
		{
			継続,
			完了
		}

		public STDGBSValue<bool> b新記録スキル;
		public STDGBSValue<bool> b新記録スコア;
		public STDGBSValue<bool> b新記録ランク;
		public STDGBSValue<float> fPerfect率;
		public STDGBSValue<float> fGreat率;
		public STDGBSValue<float> fGood率;
		public STDGBSValue<float> fPoor率;
		public STDGBSValue<float> fMiss率;
		// #23596 10.11.16 add ikanick
		//        10.11.17 change (int to bool) ikanick
		public STDGBSValue<bool> bオート;
		public STDGBSValue<CScoreIni.ERANK> nランク値;
		public STDGBSValue<int> n演奏回数;
		public CScoreIni.ERANK n総合ランク値;
		public STPadValue<CChip> r空うちドラムチップ;
		public STDGBSValue<CScoreIni.C演奏記録> st演奏記録;

		private CCounter ct登場用;
		private E戻り値 eフェードアウト完了時の戻り値;
		private CActFIFOWhite actFI;
		private CActFIFOBlack actFO;
		private CActオプションパネル actOption;
		private CAct演奏AVI actAVI;
		private CActResultParameterPanel actParameterPanel;
		private CActResultRank actRank;
		private CActResultImage actResultImage;
		private CActResultSongBar actSongBar;
		private bool bアニメが完了;
		private bool bIsCheckedWhetherResultScreenShouldSaveOrNot;        // #24509 2011.3.14 yyagi
		private readonly int[] nチャンネル0Atoレーン07;
		private int n最後に再生したHHのWAV番号;
		private EChannel e最後に再生したHHのチャンネル番号;
		private CSound rResultSound;
		//private CTexture txオプションパネル;
		private CTextureAf tx下部パネル;
		private CTextureAf tx上部パネル;
		private CTexture tx背景;

		public CStage結果()
		{
			this.st演奏記録.Drums = new CScoreIni.C演奏記録();
			this.st演奏記録.Guitar = new CScoreIni.C演奏記録();
			this.st演奏記録.Bass = new CScoreIni.C演奏記録();
			this.n総合ランク値 = CScoreIni.ERANK.UNKNOWN;
			this.nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
			base.eステージID = CStage.Eステージ.結果;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actResultImage = new CActResultImage());
			base.list子Activities.Add(this.actParameterPanel = new CActResultParameterPanel());
			base.list子Activities.Add(this.actRank = new CActResultRank());
			base.list子Activities.Add(this.actSongBar = new CActResultSongBar());
			base.list子Activities.Add(this.actOption = new CActオプションパネル(EOptionPanelDirection.Horizontal));
			base.list子Activities.Add(this.actFI = new CActFIFOWhite());
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actAVI = new CAct演奏AVI());

			this.actResultImage.actAVI = this.actAVI;
		}

		public override void On活性化()
		{
			Trace.TraceInformation("結果ステージを活性化します。");
			Trace.Indent();
			try
			{
				#region [ 初期化 ]
				//---------------------
				this.eフェードアウト完了時の戻り値 = E戻り値.継続;
				this.bアニメが完了 = false;
				this.bIsCheckedWhetherResultScreenShouldSaveOrNot = false;        // #24609 2011.3.14 yyagi
				this.n最後に再生したHHのWAV番号 = -1;
				this.e最後に再生したHHのチャンネル番号 = EChannel.Invalid;
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					this.b新記録スキル[i] = false;
					this.b新記録スコア[i] = false;
					this.b新記録ランク[i] = false;
				}
				actOption.Pos = CDTXMania.Instance.Coordinates.OptionPanelSelect;
				//---------------------
				#endregion

				#region [ 結果の計算 ]
				//---------------------
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					this.nランク値[i] = CScoreIni.ERANK.UNKNOWN;
					this.fPerfect率[i] = this.fGreat率[i] = this.fGood率[i] = this.fPoor率[i] = this.fMiss率[i] = 0.0f;  // #28500 2011.5.24 yyagi
					if ((
						((i != EPart.Drums) || (CDTXMania.Instance.DTX.bチップがある.Drums)) &&
						((i != EPart.Guitar) || CDTXMania.Instance.DTX.bチップがある.Guitar)) &&
						((i != EPart.Bass) || CDTXMania.Instance.DTX.bチップがある.Bass))
					{
						CScoreIni.C演奏記録 part = this.st演奏記録[i];
						bool bIsAutoPlay = CDTXMania.Instance.ConfigIni.bIsAutoPlay(i) || !CDTXMania.Instance.ConfigIni.b楽器有効(i);
						this.fPerfect率[i] = bIsAutoPlay ? 0f : ((100f * part.nPerfect数) / ((float)part.n全チップ数));
						this.fGreat率[i] = bIsAutoPlay ? 0f : ((100f * part.nGreat数) / ((float)part.n全チップ数));
						this.fGood率[i] = bIsAutoPlay ? 0f : ((100f * part.nGood数) / ((float)part.n全チップ数));
						this.fPoor率[i] = bIsAutoPlay ? 0f : ((100f * part.nPoor数) / ((float)part.n全チップ数));
						this.fMiss率[i] = bIsAutoPlay ? 0f : ((100f * part.nMiss数) / ((float)part.n全チップ数));

						// #23596 10.11.16 add ikanick そのパートがオートなら1
						//        10.11.17 change (int to bool) ikanick
						this.bオート[i] = bIsAutoPlay;
						this.nランク値[i] = CScoreIni.tランク値を計算して返す(part);
					}
				}
				this.n総合ランク値 = CScoreIni.t総合ランク値を計算して返す(this.st演奏記録);
				//---------------------
				#endregion

				#region [ .score.ini の作成と出力 ]
				//---------------------
				string str = CDTXMania.Instance.DTX.strファイル名の絶対パス + ".score.ini";
				CScoreIni ini = new CScoreIni(str);

				STDGBSValue<bool> b今までにフルコンボしたことがある =
					new STDGBSValue<bool>();

				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					// フルコンボチェックならびに新記録ランクチェックは、ini.Record[] が、
					// スコアチェックや演奏型スキルチェックの IF 内で書き直されてしまうよりも前に行う。(2010.9.10)

					b今までにフルコンボしたことがある[i] =
						ini.stセクション.HiScore[i].bフルコンボである ||
						ini.stセクション.HiSkill[i].bフルコンボである;

					if (this.nランク値[i] >= 0 && ini.stファイル.BestRank[i] > this.nランク値[i])
					{
						// #24459 2011.3.1 yyagi update BestRank
						this.b新記録ランク[i] = true;
						ini.stファイル.BestRank[i] = this.nランク値[i];
					}

					// 新記録スコアチェック
					if (this.st演奏記録[i].nスコア > ini.stセクション.HiScore[i].nスコア)
					{
						this.b新記録スコア[i] = true;
						ini.stセクション.HiScore[i] = this.st演奏記録[i];
						CScoreIni.Eセクション種別 esect = CScoreIni.Eセクション種別.HiScoreDrums;
						if (i == EPart.Guitar)
						{
							esect = CScoreIni.Eセクション種別.HiScoreGuitar;
						}
						else if (i == EPart.Bass)
						{
							esect = CScoreIni.Eセクション種別.HiScoreBass;
						}
						this.SaveGhost(esect); // #35411 chnmr0 add
					}

					// 新記録スキルチェック
					if (this.st演奏記録[i].db演奏型スキル値 > ini.stセクション.HiSkill[i].db演奏型スキル値)
					{
						this.b新記録スキル[i] = true;
						ini.stセクション.HiSkill[i] = this.st演奏記録[i];
						CScoreIni.Eセクション種別 esect = CScoreIni.Eセクション種別.HiSkillDrums;
						if (i == EPart.Guitar)
						{
							esect = CScoreIni.Eセクション種別.HiSkillGuitar;
						}
						else if (i == EPart.Bass)
						{
							esect = CScoreIni.Eセクション種別.HiSkillBass;
						}
						this.SaveGhost(esect); // #35411 chnmr0 add
					}

					// ラストプレイ #23595 2011.1.9 ikanick
					// オートじゃなければプレイ結果を書き込む
					if (this.bオート[i] == false)
					{
						ini.stセクション.LastPlay[i] = this.st演奏記録[i];
						CScoreIni.Eセクション種別 esect = CScoreIni.Eセクション種別.LastPlayDrums;
						if (i == EPart.Guitar)
						{
							esect = CScoreIni.Eセクション種別.LastPlayGuitar;
						}
						else if (i == EPart.Bass)
						{
							esect = CScoreIni.Eセクション種別.LastPlayBass;
						}
						this.SaveGhost(esect); // #35411 chnmr0 add
					}

					// #23596 10.11.16 add ikanick オートじゃないならクリア回数を1増やす
					//        11.02.05 bオート to t更新条件を取得する use      ikanick
					STDGBSValue<bool> b更新が必要か否か;
					b更新が必要か否か = CScoreIni.t更新条件を取得する();
					if (b更新が必要か否か[i])
					{
						switch (i)
						{
							case EPart.Drums:
								ini.stファイル.ClearCountDrums++;
								break;
							case EPart.Guitar:
								ini.stファイル.ClearCountGuitar++;
								break;
							case EPart.Bass:
								ini.stファイル.ClearCountBass++;
								break;
							default:
								throw new Exception("クリア回数増加のk(0-2)が範囲外です。");
						}
					}
					//---------------------------------------------------------------------/
				}
				if (CDTXMania.Instance.ConfigIni.bScoreIni)
					ini.t書き出し(str);
				//---------------------
				#endregion

				#region [ リザルト画面への演奏回数の更新 #24281 2011.1.30 yyagi]
				if (CDTXMania.Instance.ConfigIni.bScoreIni)
				{
					this.n演奏回数.Drums = ini.stファイル.PlayCountDrums;
					this.n演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
					this.n演奏回数.Bass = ini.stファイル.PlayCountBass;
				}
				#endregion
				#region [ 選曲画面の譜面情報の更新 ]
				//---------------------
				if (!CDTXMania.Instance.bコンパクトモード)
				{
					Cスコア cスコア = CDTXMania.Instance.stage選曲.r確定されたスコア;
					STDGBSValue<bool> b更新が必要か否か;
					b更新が必要か否か = CScoreIni.t更新条件を取得する();
					for (EPart m = EPart.Drums; m <= EPart.Bass; m++)
					{
						if (b更新が必要か否か[m])
						{
							// FullCombo した記録を FullCombo なしで超えた場合、FullCombo マークが消えてしまう。
							// → FullCombo は、最新記録と関係なく、一度達成したらずっとつくようにする。(2010.9.11)
							cスコア.譜面情報.フルコンボ[m] = this.st演奏記録[m].bフルコンボである | b今までにフルコンボしたことがある[m];

							if (this.b新記録スキル[m])
							{
								cスコア.譜面情報.最大スキル[m] = this.st演奏記録[m].db演奏型スキル値;
							}

							if (this.b新記録ランク[m])
							{
								cスコア.譜面情報.最大ランク[m] = this.nランク値[m];
							}
						}
					}
				}
				//---------------------
				#endregion

				#region [ #RESULTSOUND_xx の再生（あれば）]
				//---------------------
				CScoreIni.ERANK rank = CScoreIni.t総合ランク値を計算して返す(this.st演奏記録);

				if (rank == CScoreIni.ERANK.UNKNOWN)  // #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
				{
					rank = CScoreIni.ERANK.E;
				}

				if (string.IsNullOrEmpty(CDTXMania.Instance.DTX.RESULTSOUND[(int)rank]))
				{
					CDTXMania.Instance.Skin.soundステージクリア音.t再生する();
					this.rResultSound = null;
				}
				else
				{
					string str2 = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.RESULTSOUND[(int)rank];
					try
					{
						this.rResultSound = CDTXMania.Instance.Sound管理.tサウンドを生成する(str2);
					}
					catch
					{
						Trace.TraceError("サウンドの生成に失敗しました。({0})", new object[] { str2 });
						this.rResultSound = null;
					}
				}
				//---------------------
				#endregion

				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation("結果ステージの活性化を完了しました。");
				Trace.Unindent();
			}
		}

		// #35411 chnmr0 add
		private void SaveGhost(CScoreIni.Eセクション種別 sect)
		{
			STDGBSValue<bool> saveCond = new STDGBSValue<bool>();
			saveCond.Drums = true;
			saveCond.Guitar = true;
			saveCond.Bass = true;

			foreach (CChip chip in CDTXMania.Instance.DTX.listChip)
			{
				if (chip.bIsAutoPlayed)
				{
					if (chip.eチャンネル番号 != EChannel.Guitar_Wailing && chip.eチャンネル番号 != EChannel.Bass_Wailing) // Guitar/Bass Wailing は OK
					{
						saveCond[chip.e楽器パート] = false;
					}
				}
			}
			for (EPart instIndex = EPart.Drums; instIndex <= EPart.Bass; ++instIndex)
			{
				saveCond[instIndex] &= CDTXMania.Instance.DTX.listAutoGhostLag.Drums == null;
			}

			string directory = CDTXMania.Instance.DTX.strフォルダ名;
			string filename = CDTXMania.Instance.DTX.strファイル名 + ".";
			EPart inst = EPart.Unknown;

			if (sect == CScoreIni.Eセクション種別.HiScoreDrums)
			{
				filename += "hiscore.dr.ghost";
				inst = EPart.Drums;
			}
			else if (sect == CScoreIni.Eセクション種別.HiSkillDrums)
			{
				filename += "hiskill.dr.ghost";
				inst = EPart.Drums;
			}
			if (sect == CScoreIni.Eセクション種別.HiScoreGuitar)
			{
				filename += "hiscore.gt.ghost";
				inst = EPart.Guitar;
			}
			else if (sect == CScoreIni.Eセクション種別.HiSkillGuitar)
			{
				filename += "hiskill.gt.ghost";
				inst = EPart.Guitar;
			}
			if (sect == CScoreIni.Eセクション種別.HiScoreBass)
			{
				filename += "hiscore.bs.ghost";
				inst = EPart.Bass;
			}
			else if (sect == CScoreIni.Eセクション種別.HiSkillBass)
			{
				filename += "hiskill.bs.ghost";
				inst = EPart.Bass;
			}
			else if (sect == CScoreIni.Eセクション種別.LastPlayDrums)
			{
				filename += "lastplay.dr.ghost";
				inst = EPart.Drums;
			}
			else if (sect == CScoreIni.Eセクション種別.LastPlayGuitar)
			{
				filename += "lastplay.gt.ghost";
				inst = EPart.Guitar;
			}
			else if (sect == CScoreIni.Eセクション種別.LastPlayBass)
			{
				filename += "lastplay.bs.ghost";
				inst = EPart.Bass;
			}

			if (inst == EPart.Unknown)
			{
				return;
			}

			int cnt = 0;
			foreach (DTXMania.CChip chip in CDTXMania.Instance.DTX.listChip)
			{
				if (chip.e楽器パート == inst)
				{
					++cnt;
				}
			}

			if (saveCond[inst])
			{
				using (FileStream fs = new FileStream(directory + "\\" + filename, FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter bw = new BinaryWriter(fs))
					{
						bw.Write((Int32)cnt);
						foreach (DTXMania.CChip chip in CDTXMania.Instance.DTX.listChip)
						{
							if (chip.e楽器パート == inst)
							{
								// -128 ms から 127 ms までのラグしか保存しない
								// その範囲を超えているラグはクランプ
								// ラグデータの 上位８ビットでそのチップの前でギター空打ちBADがあったことを示す
								int lag = chip.nLag;
								if (lag < -128)
								{
									lag = -128;
								}
								if (lag > 127)
								{
									lag = 127;
								}
								byte lower = (byte)(lag + 128);
								int upper = chip.extendInfoForGhost == false ? 1 : 0;
								bw.Write((short)((upper << 8) | lower));
							}
						}
					}
				}
			}
		}

		public override void On非活性化()
		{
			if (this.rResultSound != null)
			{
				CDTXMania.Instance.Sound管理.tサウンドを破棄する(this.rResultSound);
				this.rResultSound = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx背景 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult background.jpg"));
				this.tx上部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenResult header panel.png"), true);
				this.tx下部パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenResult footer panel.png"), true);
				//this.txオプションパネル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\Screen option panels.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				if (this.ct登場用 != null)
				{
					this.ct登場用 = null;
				}
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.tx上部パネル);
				TextureFactory.tテクスチャの解放(ref this.tx下部パネル);
				//TextureFactory.tテクスチャの解放(ref this.txオプションパネル);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				int num;
				if (base.b初めての進行描画)
				{
					this.ct登場用 = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
					this.actFI.tフェードイン開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					if (this.rResultSound != null)
					{
						this.rResultSound.t再生を開始する();
					}
					base.b初めての進行描画 = false;
				}
				this.bアニメが完了 = true;
				if (this.ct登場用.b進行中)
				{
					this.ct登場用.t進行();
					if (this.ct登場用.b終了値に達した)
					{
						this.ct登場用.t停止();
					}
					else
					{
						this.bアニメが完了 = false;
					}
				}

				// 描画

				if (this.tx背景 != null)
				{
					this.tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);
				}
				if (this.ct登場用.b進行中 && (this.tx上部パネル != null))
				{
					double num2 = ((double)this.ct登場用.n現在の値) / 100.0;
					double num3 = Math.Sin(Math.PI / 2 * num2);
					num = ((int)(this.tx上部パネル.sz画像サイズ.Height * num3)) - this.tx上部パネル.sz画像サイズ.Height;
				}
				else
				{
					num = 0;
				}
				if (this.tx上部パネル != null)
				{
					this.tx上部パネル.t2D描画(CDTXMania.Instance.Device, 0, num * Scale.Y);
				}
				if (this.tx下部パネル != null)
				{
					this.tx下部パネル.t2D描画(CDTXMania.Instance.Device, 0, (SampleFramework.GameWindowSize.Height - this.tx下部パネル.sz画像サイズ.Height));
				}
				this.actOption.On進行描画();
				if (this.actResultImage.On進行描画() == 0)
				{
					this.bアニメが完了 = false;
				}
				if (this.actParameterPanel.On進行描画() == 0)
				{
					this.bアニメが完了 = false;
				}
				if (this.actRank.On進行描画() == 0)
				{
					this.bアニメが完了 = false;
				}
				if (this.actSongBar.On進行描画() == 0)
				{
					this.bアニメが完了 = false;
				}
				if (base.eフェーズID == CStage.Eフェーズ.共通_フェードイン)
				{
					if (this.actFI.On進行描画() != 0)
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
				}
				else if ((base.eフェーズID == CStage.Eフェーズ.共通_フェードアウト))     //&& ( this.actFO.On進行描画() != 0 ) )
				{
					return (int)this.eフェードアウト完了時の戻り値;
				}
				#region [ #24609 2011.3.14 yyagi ランク更新or演奏型スキル更新時、リザルト画像をpngで保存する ]
				if (this.bアニメが完了 == true && this.bIsCheckedWhetherResultScreenShouldSaveOrNot == false  // #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
					&& CDTXMania.Instance.ConfigIni.bScoreIni
					&& CDTXMania.Instance.ConfigIni.bIsAutoResultCapture)                       // #25399 2011.6.9 yyagi
				{
					CheckAndSaveResultScreen(true);
					this.bIsCheckedWhetherResultScreenShouldSaveOrNot = true;
				}
				#endregion

				// キー入力

				if (CDTXMania.Instance.act現在入力を占有中のプラグイン == null)
				{
					if (CDTXMania.Instance.ConfigIni.bDrumsHitSound && CDTXMania.Instance.ConfigIni.bDrums有効)
					{
						for (EPad i = EPad.DrumsPadMin; i < EPad.DrumsPadMax; i++)
						{
							List<STInputEvent> events = CDTXMania.Instance.Pad.GetEvents(i);
							if ((events != null) && (events.Count > 0))
							{
								foreach (STInputEvent event2 in events)
								{
									if (!event2.b押された)
									{
										continue;
									}
									CChip rChip = this.r空うちドラムチップ[i];
									if (rChip == null)
									{
										switch (i)
										{
											case EPad.HH:
												rChip = this.r空うちドラムチップ.HHO;
												if (rChip == null)
												{
													rChip = this.r空うちドラムチップ.LC;
												}
												break;

											case EPad.FT:
												rChip = this.r空うちドラムチップ.LT;
												break;

											case EPad.CY:
												rChip = this.r空うちドラムチップ.RD;
												break;

											case EPad.HHO:
												rChip = this.r空うちドラムチップ.HH;
												if (rChip == null)
												{
													rChip = this.r空うちドラムチップ.LC;
												}
												break;

											case EPad.RD:
												rChip = this.r空うちドラムチップ.CY;
												break;

											case EPad.LC:
												rChip = this.r空うちドラムチップ.HH;
												if (rChip == null)
												{
													rChip = this.r空うちドラムチップ.HHO;
												}
												break;
										}
									}
									if ((
										(rChip != null) &&
										(rChip.eチャンネル番号 >= EChannel.HiHatClose)) && (rChip.eチャンネル番号 <= EChannel.LeftCymbal))
									{
										int nLane = this.nチャンネル0Atoレーン07[rChip.eチャンネル番号 - EChannel.HiHatClose];
										if ((nLane == 1) && (
											(rChip.eチャンネル番号 == EChannel.HiHatClose) ||
											((rChip.eチャンネル番号 == EChannel.HiHatOpen) && (this.e最後に再生したHHのチャンネル番号 != EChannel.HiHatOpen))))
										{
											CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生したHHのWAV番号);
											this.n最後に再生したHHのWAV番号 = rChip.n整数値_内部番号;
											this.e最後に再生したHHのチャンネル番号 = rChip.eチャンネル番号;
										}
										CDTXMania.Instance.DTX.tチップの再生(rChip, CDTXMania.Instance.Timer.nシステム時刻, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums);
									}
								}
							}
						}
					}
					if (
						(CDTXMania.Instance.Pad.bDecidePadIsPressedDGB() ||
						(CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない &&
						CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return))) &&
						!this.bアニメが完了)
					{
						// #25406 2011.6.9 yyagi
						this.actFI.tフェードイン完了();

						this.actResultImage.tアニメを完了させる();
						this.actParameterPanel.tアニメを完了させる();
						this.actRank.tアニメを完了させる();
						this.actSongBar.tアニメを完了させる();
						this.ct登場用.t停止();
					}
					if (base.eフェーズID == CStage.Eフェーズ.共通_通常状態)
					{
						if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Escape))
						{
							CDTXMania.Instance.Skin.sound取消音.t再生する();
							this.actFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							this.eフェードアウト完了時の戻り値 = E戻り値.完了;
						}
						if (
							CDTXMania.Instance.Pad.bDecidePadIsPressedDGB() ||
							(CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない &&
							CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return)) &&
							this.bアニメが完了)
						{
							CDTXMania.Instance.Skin.sound取消音.t再生する();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							this.eフェードアウト完了時の戻り値 = E戻り値.完了;
						}
					}
				}
			}
			return 0;
		}



		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// 自動保存モード時は、ランク更新or演奏型スキル更新時に自動保存。
		/// 手動保存モード時は、ランクに依らず保存。
		/// </summary>
		/// <param name="bIsAutoSave">true=自動保存モード, false=手動保存モード</param>
		private void CheckAndSaveResultScreen(bool bIsAutoSave)
		{
			string path = Path.GetDirectoryName(CDTXMania.Instance.DTX.strファイル名の絶対パス);
			string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
			if (bIsAutoSave)
			{
				// リザルト画像を自動保存するときは、dtxファイル名.yyMMddHHmmss_DRUMS_SS.png という形式で保存。
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					if (this.b新記録ランク[i] == true || this.b新記録スキル[i] == true)
					{
						string strPart = ((EPart)(i)).ToString();
						string strRank = ((CScoreIni.ERANK)(this.nランク値[i])).ToString();
						string strFullPath = CDTXMania.Instance.DTX.strファイル名の絶対パス + "." + datetime + "_" + strPart + "_" + strRank + ".png";
						//Surface.ToFile( pSurface, strFullPath, ImageFileFormat.Png );
						CDTXMania.Instance.SaveResultScreen(strFullPath);
					}
				}
			}
		}
		#endregion
	}
}
