using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using FDK;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace DTXMania
{
	internal partial class CStage演奏画面共通 : CStage
	{
		protected void t進行描画_AVI()
		{
			if ((
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) &&
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) &&
				(!CDTXMania.Instance.ConfigIni.bストイックモード && CDTXMania.Instance.ConfigIni.bAVI有効))
			{
				if (CDTXMania.Instance.ConfigIni.bGuitar有効 && !CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					this.actAVI.t進行描画(
						CDTXMania.Instance.Coordinates.GtMovie.X, CDTXMania.Instance.Coordinates.GtMovie.Y,
						CDTXMania.Instance.Coordinates.GtMovie.W, CDTXMania.Instance.Coordinates.GtMovie.H
						);
				}
				else if (!CDTXMania.Instance.ConfigIni.bGuitar有効 && CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					this.actAVI.t進行描画(
						CDTXMania.Instance.Coordinates.DrMovie.X, CDTXMania.Instance.Coordinates.DrMovie.Y,
						CDTXMania.Instance.Coordinates.DrMovie.W, CDTXMania.Instance.Coordinates.DrMovie.H
						);
				}
				else
				{
					this.actAVI.t進行描画(
						CDTXMania.Instance.Coordinates.Movie.X, CDTXMania.Instance.Coordinates.Movie.Y,
						CDTXMania.Instance.Coordinates.Movie.W, CDTXMania.Instance.Coordinates.Movie.H
						);
				}
			}
		}

		private void t進行描画_BGA()
		{
			if ((
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) &&
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) &&
				(!CDTXMania.Instance.ConfigIni.bストイックモード && CDTXMania.Instance.ConfigIni.bBGA有効))
			{
				if (CDTXMania.Instance.ConfigIni.bGuitar有効 && !CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					this.actBGA.t進行描画(CDTXMania.Instance.Coordinates.GtMovie.X, CDTXMania.Instance.Coordinates.GtMovie.Y);
				}
				else if (!CDTXMania.Instance.ConfigIni.bGuitar有効 && CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					this.actBGA.t進行描画(CDTXMania.Instance.Coordinates.DrMovie.X, CDTXMania.Instance.Coordinates.DrMovie.Y);
				}
				else
				{
					this.actBGA.t進行描画(CDTXMania.Instance.Coordinates.Movie.X, CDTXMania.Instance.Coordinates.Movie.Y);
				}
			}
		}

		private void t進行描画_Wailing枠()
		{
			if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				int GtWailingFrameX = CDTXMania.Instance.Coordinates.Lane.GtW.X +
					(CDTXMania.Instance.Coordinates.Lane.GtW.W - CDTXMania.Instance.Coordinates.ImgGtWailingFrame.W) / 2;
				int BsWailingFrameX = CDTXMania.Instance.Coordinates.Lane.BsW.X +
					(CDTXMania.Instance.Coordinates.Lane.BsW.W - CDTXMania.Instance.Coordinates.ImgGtWailingFrame.W) / 2;
				int GtWailingFrameY = C演奏判定ライン座標共通.n判定ラインY座標(E楽器パート.GUITAR, true, true);
				int BsWailingFrameY = C演奏判定ライン座標共通.n判定ラインY座標(E楽器パート.BASS, true, true);

				if (this.txWailing枠 != null)
				{
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						this.txWailing枠.t2D描画(CDTXMania.Instance.Device, GtWailingFrameX, GtWailingFrameY);
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						this.txWailing枠.t2D描画(CDTXMania.Instance.Device, BsWailingFrameX, BsWailingFrameY);
					}
				}
			}
		}

		protected void t進行描画_パネル文字列()
		{
			if ((base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				this.actPanel.t進行描画(CDTXMania.Instance.Coordinates.Panel.X, CDTXMania.Instance.Coordinates.Panel.Y);
			}
		}

		/// <summary>
		/// ギター・ベースのチップ表示
		/// </summary>
		/// <param name="configIni"></param>
		/// <param name="dTX"></param>
		/// <param name="pChip">描画するチップ</param>
		/// <param name="inst">楽器種別</param>
		protected void t進行描画_チップ_ギターベース(ref CChip pChip, E楽器パート inst)
		{
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				#region [ Invisible / Hidden / Sudden 処理 ]
				if (CDTXMania.Instance.ConfigIni.eInvisible[inst] != EInvisible.OFF)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				else
				{
					if (CDTXMania.Instance.ConfigIni.bSudden[inst])
					{
						pChip.b可視 = (pChip.nバーからの距離dot[inst] < CDTXMania.Instance.Coordinates.SuddenFrom[inst].Y);
					}
					if (CDTXMania.Instance.ConfigIni.bHidden[inst])
					{
						pChip.b可視 = (pChip.nバーからの距離dot[inst] >= CDTXMania.Instance.Coordinates.HiddenFrom[inst].Y);
					}
				}
				#endregion

				bool bChipHasR = pChip.bGuitarBass_R;
				bool bChipHasG = pChip.bGuitarBass_G;
				bool bChipHasB = pChip.bGuitarBass_B;
				bool bChipHasW = pChip.bGuitarBass_Wailing;
				bool bChipIsO = pChip.bGuitarBass_Open;

				#region [ chip描画 ]
				if (!pChip.bHit && pChip.b可視)
				{
					if (this.txチップGB != null)
					{
						this.txチップGB.n透明度 = pChip.n透明度;
					}

					int barY = C演奏判定ライン座標共通.n判定ラインY座標(inst);
					int y = CDTXMania.Instance.ConfigIni.bReverse[inst] ?
						barY - pChip.nバーからの距離dot[inst] :
						barY + pChip.nバーからの距離dot[inst];

					int showRangeY0 = 0;
					int showRangeY1 = SampleFramework.GameWindowSize.Height;

					if ((showRangeY0 < y) && (y < showRangeY1))
					{
						if (this.txチップGB != null)
						{
							int nアニメカウンタ現在の値 = this.ctチップ模様アニメ[inst].n現在の値;
							#region [ OPENチップの描画 ]
							if (pChip[(inst == E楽器パート.GUITAR) ? Ech定義.Guitar_Open : Ech定義.Bass_Open])
							{
								int xo = (inst == E楽器パート.GUITAR) ? CDTXMania.Instance.Coordinates.Lane.GtO.X : CDTXMania.Instance.Coordinates.Lane.BsO.X;
								this.txチップGB.t2D描画(CDTXMania.Instance.Device, xo,
									y - CDTXMania.Instance.Coordinates.ImgGtChipOpen.H / 2,
									CDTXMania.Instance.Coordinates.ImgGtChipOpen.ApplyCounterY(nアニメカウンタ現在の値 % CDTXMania.Instance.Coordinates.ImgGtCountOpenChip, 2));
							}
							Rectangle rect = CDTXMania.Instance.Coordinates.ImgGtRGBButton.ApplyCounterY(nアニメカウンタ現在の値 % CDTXMania.Instance.Coordinates.ImgGtCountRGBChip, 2);
							#endregion
							#region [ RGBチップ描画 ]
							if (bChipHasR)
							{
								if (inst == E楽器パート.GUITAR)
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.GtR.X + (CDTXMania.Instance.Coordinates.Lane.GtR.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
								else
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.BsR.X + (CDTXMania.Instance.Coordinates.Lane.BsR.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
							}
							rect.X += rect.Width;
							if (bChipHasG)
							{
								if (inst == E楽器パート.GUITAR)
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.GtG.X + (CDTXMania.Instance.Coordinates.Lane.GtG.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
								else
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.BsG.X + (CDTXMania.Instance.Coordinates.Lane.BsG.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
							}
							rect.X += rect.Width;
							if (bChipHasB)
							{
								if (inst == E楽器パート.GUITAR)
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.GtB.X + (CDTXMania.Instance.Coordinates.Lane.GtB.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
								else
								{
									this.txチップGB.t2D描画(CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Lane.BsB.X + (CDTXMania.Instance.Coordinates.Lane.BsB.W - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
										y - rect.Height / 2, rect);
								}
							}
							#endregion
						}
					}
				}
				#endregion

				// #35411 2015.08.20 chnmr0 modified
				// 従来のAUTO処理に加えてプレーヤーゴーストの再生機能を追加
				bool autoPlayCondition = (!pChip.bHit) && (pChip.nバーからの距離dot[inst] < 0);
				if (autoPlayCondition)
				{
					cInvisibleChip.StartSemiInvisible(inst);
				}

				bool autoPick = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtPick : bIsAutoPlay.BsPick;
				autoPlayCondition = !pChip.bHit && autoPick;
				long ghostLag = 0;
				bool bUsePerfectGhost = true;

				if ((pChip.e楽器パート == E楽器パート.GUITAR || pChip.e楽器パート == E楽器パート.BASS) &&
						CDTXMania.Instance.ConfigIni.eAutoGhost[pChip.e楽器パート] != EAutoGhostData.PERFECT &&
						CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート] != null &&
						0 <= pChip.n楽器パートでの出現順 &&
						pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート].Count)
				{
					// #35411 (mod) Ghost data が有効なので 従来のAUTOではなくゴーストのラグを利用
					// 発生時刻と現在時刻からこのタイミングで演奏するかどうかを決定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート][pChip.n楽器パートでの出現順];
					bool resetCombo = ghostLag > 255;
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= (pChip.e楽器パート == E楽器パート.GUITAR ? nInputAdjustTimeMs.Guitar : nInputAdjustTimeMs.Bass);
					autoPlayCondition &= (pChip.n発声時刻ms + ghostLag <= CSound管理.rc演奏用タイマ.n現在時刻ms);
					if (resetCombo && autoPlayCondition)
					{
						this.actCombo.dgbコンボ数[pChip.e楽器パート].ResetCombo();
					}
					bUsePerfectGhost = false;
				}

				if (bUsePerfectGhost)
				{
					// 従来のAUTOを使用する場合
					autoPlayCondition &= (pChip.nバーからの距離dot[inst] < 0);
				}

				if (autoPlayCondition)
				{
					int lo = (inst == E楽器パート.GUITAR) ? 0 : 3;	// lane offset
					bool autoR = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtR : bIsAutoPlay.BsR;
					bool autoG = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtG : bIsAutoPlay.BsG;
					bool autoB = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtB : bIsAutoPlay.BsB;
					bool pushingR = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.R);
					bool pushingG = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.G);
					bool pushingB = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.B);

					#region [ Chip Fire effects (auto時用) ]
					// autoPickでない時の処理は、 t入力処理・ギターベース(E楽器パート) で行う
					bool bSuccessOPEN = bChipIsO && (autoR || !pushingR) && (autoG || !pushingG) && (autoB || !pushingB);
					{
						if ((bChipHasR && (autoR || pushingR)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(0 + lo);
						}
						if ((bChipHasG && (autoG || pushingG)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(1 + lo);
						}
						if ((bChipHasB && (autoB || pushingB)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(2 + lo);
						}
					}
					#endregion
					#region [ autopick ]
					{
						bool bMiss = true;
						if (bChipHasR == autoR && bChipHasG == autoG && bChipHasB == autoB)		// autoレーンとチップレーン一致時はOK
						{																			// この条件を加えないと、同時に非autoレーンを押下している時にNGとなってしまう。
							bMiss = false;
						}
						else if ((autoR || (bChipHasR == pushingR)) && (autoG || (bChipHasG == pushingG)) && (autoB || (bChipHasB == pushingB)))
						// ( bChipHasR == ( pushingR | autoR ) ) && ( bChipHasG == ( pushingG | autoG ) ) && ( bChipHasB == ( pushingB | autoB ) ) )
						{
							bMiss = false;
						}
						else if (((bChipIsO == true) && (!pushingR | autoR) && (!pushingG | autoG) && (!pushingB | autoB)))	// OPEN時
						{
							bMiss = false;
						}
						pChip.bHit = true;
						this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag,
							inst, CDTXMania.Instance.DTX.nモニタを考慮した音量(inst), false, bMiss);
						this.r次にくるギターChip = null;
						if (!bMiss)
						{
							this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
						}
						else
						{
							pChip.nLag = 0;		// tチップのヒット処理()の引数最後がfalseの時はpChip.nLagを計算しないため、ここでAutoPickかつMissのLag=0を代入
							this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip, false);
						}
						Ech定義 chWailingChip = (inst == E楽器パート.GUITAR) ? Ech定義.Guitar_Wailing : Ech定義.Bass_Wailing;
						CChip item = this.r指定時刻に一番近い未ヒットChip(pChip.n発声時刻ms + ghostLag, chWailingChip, this.nInputAdjustTimeMs[inst], 140);
						if (item != null && !bMiss)
						{
							this.queWailing[inst].Enqueue(item);
						}
					}
					#endregion
					// #35411 modify end
				}
				SetGhostTargetOnGraph(inst);
				return;
			}	// end of "if configIni.bGuitar有効"
			else
			{
				// Guitar/Bass無効の場合は、自動演奏する
				if ((!pChip.bHit && (pChip.nバーからの距離dot[inst] < 0)))
				{
					pChip.bHit = true;
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, inst, CDTXMania.Instance.DTX.nモニタを考慮した音量(inst));
				}
			}
		}

		private void SetGhostTargetOnGraph(E楽器パート inst)
		{
			if (inst != E楽器パート.UNKNOWN)
			{
				// #35411 2015.08.21 chnmr0 add
				// 目標値グラフにゴーストの達成率を渡す
				if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] != ETargetGhostData.NONE &&
						CDTXMania.Instance.DTX.listTargetGhsotLag[inst] != null)
				{
					double val = 0;
					if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] == ETargetGhostData.ONLINE)
					{
						if (CDTXMania.Instance.DTX.n可視チップ数[inst] > 0)
						{
							// Online Stats の計算式
							val = 100 *
									(this.nヒット数_TargetGhost[inst].Perfect * 17 +
									 this.nヒット数_TargetGhost[inst].Great * 7 +
									 this.n最大コンボ数_TargetGhost[inst] * 3) / (20.0 * CDTXMania.Instance.DTX.n可視チップ数[inst]);
						}
					}
					else
					{
						val = CScoreIni.t演奏型スキルを計算して返す(
								CDTXMania.Instance.DTX.n可視チップ数[inst],
								this.nヒット数_TargetGhost[inst].Perfect,
								this.nヒット数_TargetGhost[inst].Great,
								this.nヒット数_TargetGhost[inst].Good,
								this.nヒット数_TargetGhost[inst].Poor,
								this.nヒット数_TargetGhost[inst].Miss,
								E楽器パート.DRUMS, new STAUTOPLAY());
					}
					if (val < 0)
					{
						val = 0;
					}
					if (val > 100)
					{
						val = 100;
					}
					this.actGraph.dbTarget[inst] = val;
				}
			}
		}


		protected void t進行描画_チップ_小節線_拍線(ref CChip pChip)
		{
			int n小節番号plus1 = pChip.n発声位置 / 384;
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
				this.actPlayInfo.n小節番号 = n小節番号plus1 - 1;
				if (CDTXMania.Instance.ConfigIni.bWave再生位置自動調整機能有効 && (bIsDirectSound || bUseOSTimer))
				{
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				// dTX.tWaveBGM再生位置表示();		//デバッグ用
			}

			E楽器パート ePhraseNumberDisplayInst = E楽器パート.DRUMS;
			if (!CDTXMania.Instance.DTX.bチップがある.Drums)
			{
				ePhraseNumberDisplayInst = E楽器パート.GUITAR;
				if (!CDTXMania.Instance.DTX.bチップがある.Guitar)
				{
					ePhraseNumberDisplayInst = E楽器パート.BASS;
				}
			}

			for (E楽器パート inst = E楽器パート.DRUMS; inst <= E楽器パート.BASS; ++inst)
			{
				if (CDTXMania.Instance.ConfigIni.b楽器有効[inst])
				{
					this.txチップ.n透明度 = 255;

					int y = C演奏判定ライン座標共通.n判定ラインY座標(inst);
					int ymin = 0;
					int ymax = SampleFramework.GameWindowSize.Height;

					int dy = 0;
					Rectangle rc = pChip[Ech定義.BeatLine] ?
						CDTXMania.Instance.Coordinates.ImgBeatLine :
						CDTXMania.Instance.Coordinates.ImgBarLine;

					if (
						(inst == E楽器パート.DRUMS && !CDTXMania.Instance.ConfigIni.bReverse.Drums) ||
						(inst != E楽器パート.DRUMS && CDTXMania.Instance.ConfigIni.bReverse[inst])
						)
					{
						// 上から下へくるパターン
						y = y - (int)(pChip.nバーからの距離dot[inst]) - rc.Height / 2;
						dy = -dy;
					}
					else if (
						(inst == E楽器パート.DRUMS && CDTXMania.Instance.ConfigIni.bReverse.Drums) ||
						(inst != E楽器パート.DRUMS && !CDTXMania.Instance.ConfigIni.bReverse[inst])
						)
					{
						// 下から上へくるパターン
						y = y + (int)(pChip.nバーからの距離dot[inst]) - rc.Height / 2;
					}

					// Reverse時の小節線消失位置を、RGBボタンの真ん中程度に。
					// 非Reverse時の消失処理は、従来通りt進行描画・チップ()にお任せ。
					int n小節線消失距離dot = C演奏判定ライン座標共通.n判定ラインY座標(inst, false, true) - dy;

					if ((CDTXMania.Instance.DTX.bチップがある[(int)inst]) && (ymin < y) && (y < ymax)
						/*&& (pChip.nバーからの距離dot[(int)inst] >= n小節線消失距離dot)*/)
					{
						//小節線・拍線
						for (int offsetX = 0; offsetX < CDTXMania.Instance.Coordinates.JudgeLine[inst].W; offsetX += rc.Width)
						{
							int x = CDTXMania.Instance.Coordinates.JudgeLine[inst].X + offsetX;
							if (offsetX + rc.Width > CDTXMania.Instance.Coordinates.JudgeLine[inst].W)
							{
								rc.Width = CDTXMania.Instance.Coordinates.JudgeLine[inst].W - offsetX;
							}
							this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y, rc);
						}
						if (
							pChip[Ech定義.BarLine] &&
							CDTXMania.Instance.ConfigIni.b演奏情報を表示する &&
							(CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF) &&
							inst == ePhraseNumberDisplayInst)
						{
							// 小節番号の表示
							int n小節番号 = n小節番号plus1 - 1;
							int x = CDTXMania.Instance.Coordinates.JudgeLine[inst].X +
								CDTXMania.Instance.Coordinates.JudgeLine[inst].W +
								CDTXMania.Instance.Coordinates.ImgConsoleFont.W;
							CDTXMania.Instance.act文字コンソール.tPrint(x, y - CDTXMania.Instance.Coordinates.ImgConsoleFont.H / 2,
								C文字コンソール.Eフォント種別.白, n小節番号.ToString()
							);
						}
					}
				}
			}
		}

		protected void t進行描画_演奏情報()
		{
			if (!CDTXMania.Instance.ConfigIni.b演奏情報を表示しない)
			{
				this.actPlayInfo.t進行描画(CDTXMania.Instance.Coordinates.Debug.X, CDTXMania.Instance.Coordinates.Debug.Y);
			}
		}

		private void t進行描画_背景()
		{
			if (CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF)
			{
				if (this.tx背景 != null)
				{
					this.tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);
				}
			}
		}

		private void t進行描画_レーン()
		{
			if (CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF)
			{
				this.actLane.On進行描画();
			}
		}

		private void t進行描画_判定ライン()
		{
			if (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)
			{
				for (E楽器パート inst = E楽器パート.DRUMS; inst <= E楽器パート.BASS; ++inst)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効[inst])
					{
						// #31602 2016.2.11 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						int y = C演奏判定ライン座標共通.n判定ラインY座標(inst, false, true);
						if (this.txヒットバー != null)
						{
							int lineLength = CDTXMania.Instance.Coordinates.JudgeLine[inst].W;
							Rectangle rc = CDTXMania.Instance.Coordinates.ImgJudgeLine;
							for (int tmpW = 0; tmpW < lineLength; tmpW += rc.Width)
							{
								if (tmpW + rc.Width > lineLength)
								{
									rc.Width = lineLength - tmpW;
								}
								this.txヒットバー.t2D描画(CDTXMania.Instance.Device,
									CDTXMania.Instance.Coordinates.JudgeLine[inst].X + tmpW, y, rc);
							}
						}
					}
				}
			}
		}

		private void t背景テクスチャの生成()
		{
			string bgfilename = null;

			if ((CDTXMania.Instance.DTX.BACKGROUND != null) && (CDTXMania.Instance.DTX.BACKGROUND.Length > 0))
			{
				bgfilename = CDTXMania.Instance.DTX.BACKGROUND;
			}
			else if ((CDTXMania.Instance.DTX.BACKGROUND_GR != null) && (CDTXMania.Instance.DTX.BACKGROUND_GR.Length > 0))
			{
				bgfilename = CDTXMania.Instance.DTX.BACKGROUND_GR;
			}

			if (bgfilename != null && bgfilename.Length > 0)
			{
				bgfilename = CDTXMania.Instance.DTX.strフォルダ名 + bgfilename;
			}

			using (Bitmap bgbitmap = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height))
			{
				if (bgfilename != null && File.Exists(bgfilename))
				{
					using (Bitmap originalBackground = new Bitmap(bgfilename))
					{
						int W = originalBackground.Width;
						int H = originalBackground.Height;

						float mag = 1;
						// VGA補正
						if (W > 0 && H > 0)
						{
							float mx = SampleFramework.GameWindowSize.Width / W;
							float my = SampleFramework.GameWindowSize.Height / H;

							mag = Math.Min(mx, my);
						}

						using (Graphics graphic2 = Graphics.FromImage(bgbitmap))
						{
							float x = (SampleFramework.GameWindowSize.Width - W * mag) / 2;
							float y = (SampleFramework.GameWindowSize.Height - H * mag) / 2;
							graphic2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
							graphic2.DrawImage(originalBackground, x, y, W * mag, H * mag);
						}
					}
				}

				using (Bitmap image = new Bitmap(CSkin.Path(@"Graphics\ScreenPlayDrums-background-center.png")))
				{
					// フレーム画像合成
					using (Graphics graphics3 = Graphics.FromImage(image))
					{
						graphics3.DrawImage(bgbitmap, new Rectangle(0, 0, image.Width, image.Height));

						#region [ BGA領域黒抜き・テクスチャ変換・Full再生透明度 ]
						if ((CDTXMania.Instance.DTX.listBMP.Count > 0) || (CDTXMania.Instance.DTX.listBMPTEX.Count > 0) || CDTXMania.Instance.DTX.listAVI.Count > 0)
						{
							using (Graphics graphics2 = Graphics.FromImage(image))
							{
								graphics2.FillRectangle(Brushes.Black, CDTXMania.Instance.Coordinates.Movie);
							}
						}
					}
					if (this.tx背景 != null)
					{
						TextureFactory.tテクスチャの解放(ref this.tx背景);
						this.tx背景 = null;
					}
					this.tx背景 = TextureFactory.tテクスチャの生成(image, true);
						#endregion
				}
			}
		}

		private void t進行描画_チップ_ドラムス(ref CChip pChip)
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
						if (pChip.nバーからの距離dot.Drums < CDTXMania.Instance.Coordinates.SuddenFrom.Drums.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff;
						}
						else if (pChip.nバーからの距離dot.Drums < CDTXMania.Instance.Coordinates.SuddenFadeInFrom.Drums.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff - (int)((pChip.nバーからの距離dot.Drums - CDTXMania.Instance.Coordinates.SuddenFrom.Drums.Y) * 255.0 / 50.0);
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
						if (pChip.nバーからの距離dot.Drums < CDTXMania.Instance.Coordinates.HiddenFrom.Drums.Y)
						{
							pChip.b可視 = false;
						}
						else if (pChip.nバーからの距離dot.Drums < CDTXMania.Instance.Coordinates.HiddenFadeOutFrom.Drums.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = (int)((pChip.nバーからの距離dot.Drums - CDTXMania.Instance.Coordinates.HiddenFrom.Drums.Y) * 255.0 / 50.0);
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
					int jy = C演奏判定ライン座標共通.n判定ラインY座標(E楽器パート.DRUMS);
					int y = CDTXMania.Instance.ConfigIni.bReverse.Drums ? (int)(jy + pChip.nバーからの距離dot.Drums) : (int)(jy - pChip.nバーからの距離dot.Drums);

					if (this.txチップ != null)
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3((float)(pChip.dbチップサイズ倍率), (float)pChip.dbチップサイズ倍率, 1f);
					}
					int x;
					int num9 = this.ctチップ模様アニメ.Drums.n現在の値 % CDTXMania.Instance.Coordinates.ImgDrCountChip;
					switch (pChip.eチャンネル番号)
					{
						case Ech定義.HiHatClose:
							x = CDTXMania.Instance.Coordinates.Lane.HHC.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.HHC.W - (CDTXMania.Instance.Coordinates.ImgDrChipHHC.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHC.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHC.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.Snare:
							x = CDTXMania.Instance.Coordinates.Lane.SD.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.SD.W - (CDTXMania.Instance.Coordinates.ImgDrChipSD.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipSD.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipSD.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.BassDrum:
							x = CDTXMania.Instance.Coordinates.Lane.BD.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.BD.W - (CDTXMania.Instance.Coordinates.ImgDrChipBD.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipBD.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipBD.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.HighTom:
							x = CDTXMania.Instance.Coordinates.Lane.HT.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.HT.W - (CDTXMania.Instance.Coordinates.ImgDrChipHT.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHT.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHT.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.LowTom:
							x = CDTXMania.Instance.Coordinates.Lane.LT.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.LT.W - (CDTXMania.Instance.Coordinates.ImgDrChipLT.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipLT.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipLT.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.Cymbal:
							x = CDTXMania.Instance.Coordinates.Lane.CY.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.CY.W - (CDTXMania.Instance.Coordinates.ImgDrChipCY.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipCYDeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCYDeco);
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipCY.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCY.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.FloorTom:
							x = CDTXMania.Instance.Coordinates.Lane.FT.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.FT.W - (CDTXMania.Instance.Coordinates.ImgDrChipFT.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipFT.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipFT.ApplyCounterY(num9, 2));
							}
							break;

						case Ech定義.HiHatOpen:
							x = CDTXMania.Instance.Coordinates.Lane.HHO.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.HHO.W - (CDTXMania.Instance.Coordinates.ImgDrChipHHO.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHO.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHO.ApplyCounterY(num9, 2));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHODeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHODeco);
							}
							break;

						case Ech定義.RideCymbal:
							x = CDTXMania.Instance.Coordinates.Lane.RCY.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.RCY.W - (CDTXMania.Instance.Coordinates.ImgDrChipRCY.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipRCY.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipRCY.ApplyCounterY(num9, 2));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHODeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHODeco);
							}
							break;

						case Ech定義.LeftCymbal:
							x = CDTXMania.Instance.Coordinates.Lane.LCY.X +
								(int)((CDTXMania.Instance.Coordinates.Lane.LCY.W - (CDTXMania.Instance.Coordinates.ImgDrChipCY.W * (pChip.dbチップサイズ倍率))) / 2.0);
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipCYDeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCYDeco);
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipCY.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCY.ApplyCounterY(num9, 2));
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
					this.actChipFireD.Start((Eレーン)indexSevenLanes, flag, flag2, flag2);
					this.actPad.Hit(this.nチャンネル0Atoパッド08[pChip.eチャンネル番号 - Ech定義.HiHatClose]);
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
					this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
					cInvisibleChip.StartSemiInvisible(E楽器パート.DRUMS);
				}
				// #35411 modify end

				SetGhostTargetOnGraph(E楽器パート.DRUMS);
				return;
			}
			else
			{
				if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
					pChip.bHit = true;
				}
			}
		}
	}
}
