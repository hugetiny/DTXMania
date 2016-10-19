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
		void SudHidInvChip( EPart inst, ref CChip pChip )
		{
			if (CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.SemiInv ||
				CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.FullInv)
			{
				cInvisibleChip.SetInvisibleStatus(ref pChip);
			}
			else
			{
				if (CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.Sudden ||
					CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.SudHid)
				{
					if (pChip.nバーからの距離dot[inst] < CDTXMania.Instance.ConfigIni.nSuddenFrom[inst])
					{
						pChip.b可視 = true;
						pChip.n透明度 = 0xff;
					}
					else if (pChip.nバーからの距離dot[inst] < CDTXMania.Instance.ConfigIni.nSuddenFrom[inst] + 100)
					{
						pChip.b可視 = true;
						pChip.n透明度 = 0xff - (int)((pChip.nバーからの距離dot[inst] -
							CDTXMania.Instance.ConfigIni.nSuddenFrom[inst]) * 255.0 / 50.0);
					}
					else
					{
						pChip.b可視 = false;
						pChip.n透明度 = 0;
					}
				}
				if (CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.Hidden ||
					CDTXMania.Instance.ConfigIni.eSudHidInv[inst] == ESudHidInv.SudHid)
				{
					if (pChip.nバーからの距離dot[inst] < CDTXMania.Instance.ConfigIni.nHiddenFrom[inst])
					{
						pChip.b可視 = false;
					}
					else if (pChip.nバーからの距離dot[inst] < CDTXMania.Instance.ConfigIni.nHiddenFrom[inst] + 100)
					{
						pChip.b可視 = true;
						pChip.n透明度 = (int)((pChip.nバーからの距離dot[inst] -
							CDTXMania.Instance.ConfigIni.nHiddenFrom[inst]) * 255.0 / 50.0);
					}
				}
			}
		}


		/// <summary>
		/// ギター・ベースのチップ表示
		/// </summary>
		/// <param name="configIni"></param>
		/// <param name="dTX"></param>
		/// <param name="pChip">描画するチップ</param>
		/// <param name="inst">楽器種別</param>
		protected void t進行描画_チップ_ギターベース(ref CChip pChip, EPart inst)
		{
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				SudHidInvChip(inst, ref pChip);

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
							if (pChip[(inst == EPart.Guitar) ? EChannel.Guitar_Open : EChannel.Bass_Open])
							{
								int xo = CDTXMania.Instance.ConfigIni.cdInstX[inst][CDTXMania.Instance.ConfigIni.eActiveInst];
								this.txチップGB.t2D描画(CDTXMania.Instance.Device, xo,
									y - CDTXMania.Instance.Coordinates.ImgGtChipOpen.H / 2,
									CDTXMania.Instance.Coordinates.ImgGtChipOpen.ApplyCounterY(nアニメカウンタ現在の値 % CDTXMania.Instance.Coordinates.ImgGtCountOpenChip, 2));
							}
							Rectangle rect = CDTXMania.Instance.Coordinates.ImgGtRGBButton.ApplyCounterY(nアニメカウンタ現在の値 % CDTXMania.Instance.Coordinates.ImgGtCountRGBChip, 2);
							#endregion
							#region [ RGBチップ描画 ]
							if (bChipHasR)
							{
								this.txチップGB.t2D描画(CDTXMania.Instance.Device,
									CDTXMania.Instance.ConfigIni.GetLaneX(inst == EPart.Guitar ? ELane.GtR : ELane.BsR) +
									(CDTXMania.Instance.ConfigIni.GetLaneW(inst == EPart.Guitar ? ELane.GtR : ELane.BsR) - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
									y - rect.Height / 2, rect);
							}
							rect.X += rect.Width;
							if (bChipHasG)
							{
								this.txチップGB.t2D描画(CDTXMania.Instance.Device,
									CDTXMania.Instance.ConfigIni.GetLaneX(inst == EPart.Guitar ? ELane.GtG : ELane.BsG) +
									(CDTXMania.Instance.ConfigIni.GetLaneW(inst == EPart.Guitar ? ELane.GtG : ELane.BsG) - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
									y - rect.Height / 2, rect);
							}
							rect.X += rect.Width;
							if (bChipHasB)
							{
								this.txチップGB.t2D描画(CDTXMania.Instance.Device,
									CDTXMania.Instance.ConfigIni.GetLaneX(inst == EPart.Guitar ? ELane.GtB : ELane.BsB) +
									(CDTXMania.Instance.ConfigIni.GetLaneW(inst == EPart.Guitar ? ELane.GtB : ELane.BsB) - CDTXMania.Instance.Coordinates.ImgGtRGBButton.W) / 2,
									y - rect.Height / 2, rect);
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

				bool autoPick = (inst == EPart.Guitar) ?
										CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick :
										CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick;

				autoPlayCondition = !pChip.bHit && autoPick;
				long ghostLag = 0;
				bool bUsePerfectGhost = true;

				if ((pChip.e楽器パート == EPart.Guitar || pChip.e楽器パート == EPart.Bass) &&
						CDTXMania.Instance.ConfigIni.eAutoGhost[pChip.e楽器パート] != EAutoGhostData.Perfect &&
						CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート] != null &&
						0 <= pChip.n楽器パートでの出現順 &&
						pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート].Count)
				{
					// #35411 (mod) Ghost data が有効なので 従来のAUTOではなくゴーストのラグを利用
					// 発生時刻と現在時刻からこのタイミングで演奏するかどうかを決定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート][pChip.n楽器パートでの出現順];
					bool resetCombo = ghostLag > 255;
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= (pChip.e楽器パート == EPart.Guitar ?
												CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Guitar :
												CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Bass);
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
					// lane offset
					int lo = (inst == EPart.Guitar) ? 0 : 3;
					bool autoR = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtR : CDTXMania.Instance.ConfigIni.bAutoPlay.BsR;
					bool autoG = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtG : CDTXMania.Instance.ConfigIni.bAutoPlay.BsG;
					bool autoB = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtB : CDTXMania.Instance.ConfigIni.bAutoPlay.BsB;
					bool pushingR = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtR : EPad.BsR);
					bool pushingG = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtG : EPad.BsG);
					bool pushingB = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtB : EPad.BsB);

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
						if (bChipHasR == autoR && bChipHasG == autoG && bChipHasB == autoB)   // autoレーンとチップレーン一致時はOK
						{                                     // この条件を加えないと、同時に非autoレーンを押下している時にNGとなってしまう。
							bMiss = false;
						}
						else if ((autoR || (bChipHasR == pushingR)) && (autoG || (bChipHasG == pushingG)) && (autoB || (bChipHasB == pushingB)))
						// ( bChipHasR == ( pushingR | autoR ) ) && ( bChipHasG == ( pushingG | autoG ) ) && ( bChipHasB == ( pushingB | autoB ) ) )
						{
							bMiss = false;
						}
						else if (((bChipIsO == true) && (!pushingR | autoR) && (!pushingG | autoG) && (!pushingB | autoB))) // OPEN時
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
							pChip.nLag = 0;   // tチップのヒット処理()の引数最後がfalseの時はpChip.nLagを計算しないため、ここでAutoPickかつMissのLag=0を代入
							this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip, false);
						}
						EChannel chWailingChip = (inst == EPart.Guitar) ? EChannel.Guitar_Wailing : EChannel.Bass_Wailing;
						CChip item = this.r指定時刻に一番近い未ヒットChip(pChip.n発声時刻ms + ghostLag, chWailingChip, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[inst], 140);
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
			} // end of "if configIni.bGuitar有効"
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

		private void SetGhostTargetOnGraph(EPart inst)
		{
			if (inst != EPart.Unknown)
			{
				// #35411 2015.08.21 chnmr0 add
				// 目標値グラフにゴーストの達成率を渡す
				if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] != ETargetGhostData.None &&
						CDTXMania.Instance.DTX.listTargetGhsotLag[inst] != null)
				{
					double val = 0;
					if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] == ETargetGhostData.Online)
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
										EPart.Drums, false);
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
				if (CDTXMania.Instance.ConfigIni.bWaveAdjust && (bIsDirectSound || CDTXMania.Instance.ConfigIni.bUseOSTimer))
				{
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				// dTX.tWaveBGM再生位置表示();		//デバッグ用
			}

			EPart ePhraseNumberDisplayInst = EPart.Drums;
			if (!CDTXMania.Instance.DTX.bチップがある.Drums)
			{
				ePhraseNumberDisplayInst = EPart.Guitar;
				if (!CDTXMania.Instance.DTX.bチップがある.Guitar)
				{
					ePhraseNumberDisplayInst = EPart.Bass;
				}
			}

			for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
			{
				if (CDTXMania.Instance.ConfigIni.b楽器有効(inst) && CDTXMania.Instance.DTX.bチップがある[inst])
				{
					if ((inst == EPart.Guitar || inst == EPart.Bass) && pChip[EChannel.BeatLine])
					{
						continue;
					}
					this.txチップ.n透明度 = 255;

					int y = C演奏判定ライン座標共通.n判定ラインY座標(inst);
					int ymin = 0;
					int ymax = SampleFramework.GameWindowSize.Height;

					int dy = 0;
					Rectangle rc = pChip[EChannel.BeatLine] ?
						CDTXMania.Instance.Coordinates.ImgBeatLine :
						CDTXMania.Instance.Coordinates.ImgBarLine;

					if (
						(inst == EPart.Drums && !CDTXMania.Instance.ConfigIni.bReverse.Drums) ||
						(inst != EPart.Drums && CDTXMania.Instance.ConfigIni.bReverse[inst])
						)
					{
						// 上から下へくるパターン
						y = y - (int)(pChip.nバーからの距離dot[inst]) - rc.Height / 2;
						dy = -dy;
					}
					else if (
						(inst == EPart.Drums && CDTXMania.Instance.ConfigIni.bReverse.Drums) ||
						(inst != EPart.Drums && !CDTXMania.Instance.ConfigIni.bReverse[inst])
						)
					{
						// 下から上へくるパターン
						y = y + (int)(pChip.nバーからの距離dot[inst]) - rc.Height / 2;
					}

					// Reverse時の小節線消失位置を、RGBボタンの真ん中程度に。
					// 非Reverse時の消失処理は、従来通りt進行描画・チップ()にお任せ。
					// int n小節線消失距離dot = C演奏判定ライン座標共通.n判定ラインY座標(inst, false, true) - dy;

					if ((CDTXMania.Instance.DTX.bチップがある[(int)inst]) && (ymin < y) && (y < ymax) &&
						(CDTXMania.Instance.ConfigIni.eDark == EDark.Off ||
						(CDTXMania.Instance.ConfigIni.eDark == EDark.Half && ( pChip[EChannel.BarLine] || pChip[EChannel.BeatLine] ))))
						/*&& (pChip.nバーからの距離dot[(int)inst] >= n小節線消失距離dot)*/
					{
						int jx = CDTXMania.Instance.ConfigIni.cdInstX[inst][CDTXMania.Instance.ConfigIni.eActiveInst].Value;
						int jy = CDTXMania.Instance.ConfigIni.cdJudgeLineY[inst];
						int jw = CDTXMania.Instance.Coordinates.Instrument[inst].W;
						//小節線・拍線
						if (pChip.b可視)
						{
							for ( int offsetX = 0; offsetX < jw; offsetX += rc.Width )
							{
								int x = jx + offsetX;
								if ( offsetX + rc.Width > jw )
								{
									rc.Width = jw - offsetX;
								}
								this.txチップ.t2D描画( CDTXMania.Instance.Device, x, y, rc );
							}
						}
						if (
							pChip[EChannel.BarLine] &&
							CDTXMania.Instance.ConfigIni.bDebugInfo &&
							(CDTXMania.Instance.ConfigIni.eDark == EDark.Off) &&
							inst == ePhraseNumberDisplayInst)
						{
							// 小節番号の表示
							int n小節番号 = n小節番号plus1 - 1;
							int x = jx + jw + CDTXMania.Instance.Coordinates.ImgConsoleFont.W;
							CDTXMania.Instance.act文字コンソール.tPrint(x, y - CDTXMania.Instance.Coordinates.ImgConsoleFont.H / 2,
								C文字コンソール.Eフォント種別.白, n小節番号.ToString()
							);
						}
					}
				}
			}
		}

		private void t進行描画_判定ライン()
		{
			if (CDTXMania.Instance.ConfigIni.eDark != EDark.Full)
			{
				for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効(inst) && CDTXMania.Instance.DTX.bチップがある[inst])
					{
						// #31602 2016.2.11 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						int y = C演奏判定ライン座標共通.n判定ラインY座標(inst, false, true);
						if (this.txヒットバー != null)
						{
							int lineLength = CDTXMania.Instance.Coordinates.Instrument[inst].W;
							Rectangle rc = CDTXMania.Instance.Coordinates.ImgJudgeLine;
							int barHeight = rc.Height;
							for (int tmpW = 0; tmpW < lineLength; tmpW += rc.Width)
							{
								if (tmpW + rc.Width > lineLength)
								{
									rc.Width = lineLength - tmpW;
								}
								this.txヒットバー.t2D描画(CDTXMania.Instance.Device,
									CDTXMania.Instance.ConfigIni.cdInstX[ inst ][ CDTXMania.Instance.ConfigIni.eActiveInst ] + tmpW, y - barHeight / 2, rc );
							}
						}
					}
				}
			}
		}

		private void t背景テクスチャの生成()
		{
			string bgfilename = null;

			if ((CDTXMania.Instance.ConfigIni.b楽器有効(EPart.Drums) == false) &&  (CDTXMania.Instance.DTX.BACKGROUND_GR != null) && (CDTXMania.Instance.DTX.BACKGROUND_GR.Length > 0))
			{
				bgfilename = CDTXMania.Instance.DTX.BACKGROUND_GR;
			}
			else if ((CDTXMania.Instance.DTX.BACKGROUND != null) && (CDTXMania.Instance.DTX.BACKGROUND.Length > 0))
			{
				bgfilename = CDTXMania.Instance.DTX.BACKGROUND;
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
						float W = originalBackground.Width;
						float H = originalBackground.Height;

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
							graphics3.FillRectangle(Brushes.Black,
								new Rectangle(
									CDTXMania.Instance.ConfigIni.cdMovieX[CDTXMania.Instance.ConfigIni.eActiveInst],
									CDTXMania.Instance.ConfigIni.cdMovieY[CDTXMania.Instance.ConfigIni.eActiveInst],
									CDTXMania.Instance.Coordinates.Movie.W, CDTXMania.Instance.Coordinates.Movie.H));
						}
						#endregion
					}
					TextureFactory.tテクスチャの解放(ref this.tx背景);
					this.tx背景 = TextureFactory.tテクスチャの生成(image, true);
				}
			}
		}

		private void t進行描画_チップ_ドラムス(ref CChip pChip)
		{
			if (CDTXMania.Instance.ConfigIni.bDrums有効)
			{
				SudHidInvChip(EPart.Drums, ref pChip);

				if (!pChip.bHit && pChip.b可視)
				{
					if (this.txチップ != null)
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int jy = C演奏判定ライン座標共通.n判定ラインY座標(EPart.Drums);
					int y = CDTXMania.Instance.ConfigIni.bReverse.Drums ? (int)(jy + pChip.nバーからの距離dot.Drums) : (int)(jy - pChip.nバーからの距離dot.Drums);

					if (this.txチップ != null)
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3((float)(pChip.dbチップサイズ倍率), (float)pChip.dbチップサイズ倍率, 1f);
					}

					int num9 = this.ctチップ模様アニメ.Drums.n現在の値 % CDTXMania.Instance.Coordinates.ImgDrCountChip;
					int x = CDTXMania.Instance.ConfigIni.GetLaneX(EnumConverter.LaneFromChannel(pChip.eチャンネル番号)) +
						(int)((CDTXMania.Instance.ConfigIni.GetLaneW(EnumConverter.LaneFromChannel(pChip.eチャンネル番号))
						- (CDTXMania.Instance.Coordinates.ImgDrChip[EnumConverter.PadFromChannel(pChip.eチャンネル番号)].W * (pChip.dbチップサイズ倍率))) / 2.0);
					if (this.txチップ != null)
					{
						this.txチップ.t2D描画(CDTXMania.Instance.Device, x,
							y - CDTXMania.Instance.Coordinates.ImgDrChip[EnumConverter.PadFromChannel(pChip.eチャンネル番号)].H / 2,
							CDTXMania.Instance.Coordinates.ImgDrChip[EnumConverter.PadFromChannel(pChip.eチャンネル番号)].ApplyCounterY(num9, 2));
					}

					// Chip Decoration
					switch (pChip.eチャンネル番号)
					{
						case EChannel.Cymbal:
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x,
									y - CDTXMania.Instance.Coordinates.ImgDrChipCYDeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCYDeco);
							}
							break;

						case EChannel.HiHatOpen:
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHODeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHODeco);
							}
							break;

						case EChannel.RideCymbal:
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipHHODeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipHHODeco);
							}
							break;

						case EChannel.LeftCymbal:
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - CDTXMania.Instance.Coordinates.ImgDrChipCYDeco.H / 2, CDTXMania.Instance.Coordinates.ImgDrChipCYDeco);
							}
							break;
					}
					if (this.txチップ != null)
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
						this.txチップ.n透明度 = 0xff;
					}
				}

				// #35411 chnmr0 modified
				bool autoPlayCondition = (CDTXMania.Instance.ConfigIni.bAutoPlay[EnumConverter.PadFromChannel(pChip.eチャンネル番号)] && !pChip.bHit);
				bool UsePerfectGhost = true;
				long ghostLag = 0;

				if (CDTXMania.Instance.ConfigIni.eAutoGhost.Drums != EAutoGhostData.Perfect &&
						CDTXMania.Instance.DTX.listAutoGhostLag.Drums != null &&
						0 <= pChip.n楽器パートでの出現順 && pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag.Drums.Count)
				{
					// ゴーストデータが有効 : ラグに合わせて判定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag.Drums[pChip.n楽器パートでの出現順];
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums;
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
					this.actLaneFlushD.Start(EnumConverter.LaneFromChannel(pChip.eチャンネル番号), ((float)CInput管理.n通常音量) / 127f);
					bool flag = this.bフィルイン中;
					bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである(pChip);
					//bool flag3 = flag2;
					// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
					this.actChipFireD.Start(EnumConverter.LaneFromChannel(pChip.eチャンネル番号), flag, flag2, flag2);
					this.actPad.Hit(EnumConverter.PadFromChannel(pChip.eチャンネル番号));
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag, EPart.Drums, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Drums));
					this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
					cInvisibleChip.StartSemiInvisible(EPart.Drums);
				}
				// #35411 modify end

				SetGhostTargetOnGraph(EPart.Drums);
				return;
			}
			else
			{
				if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, EPart.Drums, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Drums));
					pChip.bHit = true;
				}
			}
		}
	}
}
