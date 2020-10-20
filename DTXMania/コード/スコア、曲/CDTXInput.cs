using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Globalization;
using System.Threading;
using FDK;

namespace DTXMania
{
	public partial class CDTX
	{
		public void t入力(string strファイル名, bool bEnumerating, bool bレーン情報を確認する)
		{
			this.t入力(strファイル名, bEnumerating, bレーン情報を確認する, 1.0, 0);
		}
		public void t入力(string strファイル名, bool bEnumerating, bool bレーン情報を確認する, double db再生速度, int nBGMAdjust)
		{
			this.bヘッダのみ = bEnumerating;
			this.bレーン情報を確認する = bレーン情報を確認する;
			this.strファイル名の絶対パス = Path.GetFullPath(strファイル名);
			this.strファイル名 = Path.GetFileName(this.strファイル名の絶対パス);
			this.strフォルダ名 = Path.GetDirectoryName(this.strファイル名の絶対パス) + @"\";
			string ext = Path.GetExtension(this.strファイル名).ToLower();
			if (ext != null)
			{
				if (!(ext == ".dtx"))
				{
					if (ext == ".gda")
					{
						this.e種別 = EDTX種別.GDA;
					}
					else if (ext == ".g2d")
					{
						this.e種別 = EDTX種別.G2D;
					}
					else if (ext == ".bms")
					{
						this.e種別 = EDTX種別.BMS;
					}
					else if (ext == ".bme")
					{
						this.e種別 = EDTX種別.BME;
					}
					else if (ext == ".mid")
					{
						this.e種別 = EDTX種別.SMF;
					}
				}
				else
				{
					this.e種別 = EDTX種別.DTX;
				}
			}
			if (this.e種別 != EDTX種別.SMF)
			{
				try
				{
					//DateTime timeBeginLoad = DateTime.Now;
					//TimeSpan span;

					StreamReader reader = new StreamReader(strファイル名, Encoding.GetEncoding("Shift_JIS"));
					string str2 = reader.ReadToEnd();
					reader.Close();
					//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
					//Trace.TraceInformation( "DTXfileload時間:          {0}", span.ToString() );

					this.t入力_全入力文字列から(str2, db再生速度, nBGMAdjust);
				}
				catch
				{
				}
			}
			else
			{
				Trace.TraceWarning("SMF の演奏は未対応です。（検討中）");
			}
		}
		public void t入力_全入力文字列から(string str全入力文字列)
		{
			this.t入力_全入力文字列から(str全入力文字列, 1.0, 0);
		}
		public void t入力_全入力文字列から(string str全入力文字列, double db再生速度, int nBGMAdjust)
		{
			//DateTime timeBeginLoad = DateTime.Now;
			//TimeSpan span;

			if (!string.IsNullOrEmpty(str全入力文字列))
			{
				#region [ 改行カット ]
				this.db再生速度 = db再生速度;
				str全入力文字列 = str全入力文字列.Replace(Environment.NewLine, "\n");
				str全入力文字列 = str全入力文字列.Replace('\t', ' ');
				str全入力文字列 = str全入力文字列 + "\n";
				#endregion
				//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
				//Trace.TraceInformation( "改行カット時間:           {0}", span.ToString() );
				//timeBeginLoad = DateTime.Now;
				#region [ 初期化 ]
				for (int j = 0; j < 36 * 36; j++)
				{
					this.n無限管理WAV[j] = -j;
					this.n無限管理BPM[j] = -j;
					this.n無限管理VOL[j] = -j;
					this.n無限管理PAN[j] = -10000 - j;
					this.n無限管理SIZE[j] = -j;
				}
				this.n内部番号WAV1to = 1;
				this.n内部番号BPM1to = 1;
				this.bstackIFからENDIFをスキップする = new Stack<bool>();
				this.bstackIFからENDIFをスキップする.Push(false);
				this.n現在の乱数 = 0;
				for (int k = 0; k < 7; k++)
				{
					this.nRESULTIMAGE用優先順位[k] = 0;
					this.nRESULTMOVIE用優先順位[k] = 0;
					this.nRESULTSOUND用優先順位[k] = 0;
				}
				#endregion
				#region [ 入力/行解析 ]
				CharEnumerator ce = str全入力文字列.GetEnumerator();
				if (ce.MoveNext())
				{
					this.n現在の行数 = 1;
					do
					{
						if (!this.t入力_空白と改行をスキップする(ref ce))
						{
							break;
						}
						if (ce.Current == '#')
						{
							if (ce.MoveNext())
							{
								StringBuilder builder = new StringBuilder(0x20);
								if (this.t入力_コマンド文字列を抜き出す(ref ce, ref builder))
								{
									StringBuilder builder2 = new StringBuilder(0x400);
									if (this.t入力_パラメータ文字列を抜き出す(ref ce, ref builder2))
									{
										StringBuilder builder3 = new StringBuilder(0x400);
										if (this.t入力_コメント文字列を抜き出す(ref ce, ref builder3))
										{
											this.t入力_行解析(ref builder, ref builder2, ref builder3);
											this.n現在の行数++;
											continue;
										}
									}
								}
							}
							break;
						}
					}
					while (this.t入力_コメントをスキップする(ref ce));
					#endregion
					//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
					//Trace.TraceInformation( "抜き出し時間:             {0}", span.ToString() );
					//timeBeginLoad = DateTime.Now;
					this.n無限管理WAV = null;
					this.n無限管理BPM = null;
					this.n無限管理VOL = null;
					this.n無限管理PAN = null;
					this.n無限管理SIZE = null;
					if (!this.bヘッダのみ || this.bレーン情報を確認する)
					{
						#region [ BPM/BMP初期化 ]
						CBPM cbpm = null;
						foreach (CBPM cbpm2 in this.listBPM.Values)
						{
							if (cbpm2.n表記上の番号 == 0)
							{
								cbpm = cbpm2;
								break;
							}
						}
						if (cbpm == null)
						{
							cbpm = new CBPM();
							cbpm.n内部番号 = this.n内部番号BPM1to++;
							cbpm.n表記上の番号 = 0;
							cbpm.dbBPM値 = 120.0;
							this.listBPM.Add(cbpm.n内部番号, cbpm);
							CChip chip = new CChip(0, 0, cbpm.n内部番号, EChannel.BPMEx);
							this.listChip.Insert(0, chip);
						}
						else
						{
							CChip chip = new CChip(0, 0, cbpm.n内部番号, EChannel.BPMEx);
							this.listChip.Insert(0, chip);
						}
						if (this.listBMP.ContainsKey(0))
						{
							CChip chip = new CChip(0, 0, 0, EChannel.BGALayer1);
							this.listChip.Insert(0, chip);
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "前準備完了時間:           {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ CWAV初期化 ]
						foreach (CWAV cwav in this.listWAV.Values)
						{
							if (cwav.nチップサイズ < 0)
							{
								cwav.nチップサイズ = 100;
							}
							if (cwav.n位置 <= -10000)
							{
								cwav.n位置 = 0;
							}
							if (cwav.n音量 < 0)
							{
								cwav.n音量 = 100;
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "CWAV前準備時間:           {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ チップ倍率設定 ]						// #28145 2012.4.22 yyagi 二重ループを1重ループに変更して高速化)
						//foreach ( CWAV cwav in this.listWAV.Values )
						//{
						//    foreach( CChip chip in this.listChip )
						//    {
						//        if( chip.n整数値・内部番号 == cwav.n内部番号 )
						//        {
						//            chip.dbチップサイズ倍率 = ( (double) cwav.nチップサイズ ) / 100.0;
						//            if (chip.nチャンネル番号 == 0x01 )	// BGMだったら
						//            {
						//                cwav.bIsOnBGMLane = true;
						//            }
						//        }
						//    }
						//}
						foreach (CChip chip in this.listChip)
						{
							if (this.listWAV.ContainsKey(chip.n整数値_内部番号))
							//foreach ( CWAV cwav in this.listWAV.Values )
							{
								CWAV cwav = this.listWAV[chip.n整数値_内部番号];
								//	if ( chip.n整数値・内部番号 == cwav.n内部番号 )
								//	{
								chip.SetDBChipSizeFactor(((double)cwav.nチップサイズ) / 100.0);
								//if ( chip.nチャンネル番号 == 0x01 )	// BGMだったら
								//{
								//	cwav.bIsOnBGMLane = true;
								//}
								//	}
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "CWAV全準備時間:           {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ 必要に応じて空打ち音を0小節に定義する ]
						//for ( int m = 0xb1; m <= 0xbc; m++ )			// #28146 2012.4.21 yyagi; bb -> bc
						//{
						//    foreach ( CChip chip in this.listChip )
						//    {
						//        if ( chip.nチャンネル番号 == m )
						//        {
						//            CChip c = new CChip();
						//            c.n発声位置 = 0;
						//            c.nチャンネル番号 = chip.nチャンネル番号;
						//            c.n整数値 = chip.n整数値;
						//            c.n整数値・内部番号 = chip.n整数値・内部番号;
						//            this.listChip.Insert( 0, c );
						//            break;
						//        }
						//    }
						//}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "空打確認時間:             {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ 拍子・拍線の挿入と、クリック音の挿入と、最初に再生するサウンド直前へのマーカー挿入(録音開始トリガ) ]
						if (this.listChip.Count > 0)
						{
							this.listChip.Sort();   // 高速化のためにはこれを削りたいが、listChipの最後がn発声位置の終端である必要があるので、
																			// 保守性確保を優先してここでのソートは残しておく
																			// なお、093時点では、このソートを削除しても動作するようにはしてある。
																			// (ここまでの一部チップ登録を、listChip.Add(c)から同Insert(0,c)に変更してある)
																			// これにより、数ms程度ながらここでのソートも高速化されている。
							double barlength = 1.0;
							int nEndOfSong = (this.listChip[this.listChip.Count - 1].n発声位置 + 384) - (this.listChip[this.listChip.Count - 1].n発声位置 % 384);
							//bool bClickOffBeat = (CDTXMania.Instance.ConfigIni.eClickType == EClickType.OffBeat);       // 裏拍でメトロノーム再生
							COptionEnum<EClickType> eClickType = CDTXMania.Instance.ConfigIni.eClickType;
							for (int tick384 = 0; tick384 <= nEndOfSong; tick384 += 384)  // 小節線の挿入　(後に出てくる拍子線とループをまとめようとするなら、forループの終了条件の微妙な違いに注意が必要)
							{
								CChip chip = new CChip(tick384, 36 * 36 - 1, EChannel.BarLine);
								this.listChip.Add(chip);
							}
							//this.listChip.Sort();				// ここでのソートは不要。ただし最後にソートすること
							int nChipNo_BarLength = 0;
							int nChipNo_C1 = 0;
							for (int tick384 = 0; tick384 < nEndOfSong; tick384 += 384)
							{
								int n発声位置_C1_同一小節内 = 0;
								while ((nChipNo_C1 < this.listChip.Count) && (this.listChip[nChipNo_C1].n発声位置 < (tick384 + 384)))
								{
									if (this.listChip[nChipNo_C1].eチャンネル番号 == EChannel.BeatLineShift)       // 拍線シフトの検出
									{
										n発声位置_C1_同一小節内 = this.listChip[nChipNo_C1].n発声位置 - tick384;
									}
									nChipNo_C1++;
								}
								if ((this.e種別 == EDTX種別.BMS) || (this.e種別 == EDTX種別.BME))
								{
									barlength = 1.0;
								}
								while ((nChipNo_BarLength < this.listChip.Count) && (this.listChip[nChipNo_BarLength].n発声位置 <= tick384))
								{
									if (this.listChip[nChipNo_BarLength].eチャンネル番号 == EChannel.BarLength)    // bar lengthの検出
									{
										barlength = this.listChip[nChipNo_BarLength].db実数値;
									}
									nChipNo_BarLength++;
								}

								// 小節線上のクリック音の挿入
								int deltaOffBeat8th  = (int) ( 384.0 / 8  / barlength );
								int deltaOffBeat12th = (int) ( 384.0 / 12 / barlength);
								if ( eClickType != EClickType.OffBeat )																// 裏拍でのメトロノーム再生の設定でなければ(Off設定であっても)
								{
									this.listChip.Add( new CChip( tick384, (int)EClickSoundType.High, EChannel.Click ) );			// 小節線上に、表拍のクリック音を挿入

									if (eClickType == EClickType.Triplet)   // 三連符なら、最初残り2つのクリック音を挿入
									{
										this.listChip.Add(new CChip(tick384 + deltaOffBeat12th,     (int)EClickSoundType.Bottom, EChannel.Click));    // 小節線から12分音符だけ後に、裏拍のクリック音を挿入
										this.listChip.Add(new CChip(tick384 + deltaOffBeat12th * 2, (int)EClickSoundType.Bottom, EChannel.Click));    // 小節線から12分音符x2だけ後に、裏拍のクリック音を挿入
									}
								}
								else if ( tick384 + 384 / 8 <= nEndOfSong )											// 裏拍設定で、かつ曲長内に収まるなら
								{
									this.listChip.Add( new CChip( tick384 + deltaOffBeat8th, (int)EClickSoundType.High, EChannel.Click ) );	// 小節線から8分音符だけ後に、裏拍のクリック音を挿入
								}


								for (int i = 0; i < 100; i++)               // 拍線の挿入
								{
									int tickBeat = (int)(((double)(384 * i)) / (4.0 * barlength));
									if ((tickBeat + n発声位置_C1_同一小節内) >= 384)
									{
										break;
									}
									if (((tickBeat + n発声位置_C1_同一小節内) % 384) != 0)
									{
										CChip chip = new CChip(tick384 + (tickBeat + n発声位置_C1_同一小節内), 36 * 36 - 1, EChannel.BeatLine);
										this.listChip.Add(chip);
										if (eClickType != EClickType.OffBeat)			// メトロノーム設定が裏拍設定でなければ、拍音を挿入
										{
											this.listChip.Add( new CChip( tick384 + ( tickBeat + n発声位置_C1_同一小節内 ), (int)EClickSoundType.Low, EChannel.Click ) );

											if (eClickType == EClickType.Triplet)   // 三連符なら、最初残り2つのクリック音を挿入
											{
												this.listChip.Add(new CChip(tick384 + (tickBeat + deltaOffBeat12th     + n発声位置_C1_同一小節内), (int)EClickSoundType.Bottom, EChannel.Click));
												this.listChip.Add(new CChip(tick384 + (tickBeat + deltaOffBeat12th * 2 + n発声位置_C1_同一小節内), (int)EClickSoundType.Bottom, EChannel.Click));
											}
										}
										else if ( ( tickBeat + deltaOffBeat8th + n発声位置_C1_同一小節内 ) < 384 )	// 裏拍設定、かつ小節内に収まっていれば、拍音を挿入
										{
											this.listChip.Add( new CChip( tick384 + ( tickBeat + deltaOffBeat8th + n発声位置_C1_同一小節内 ), (int)EClickSoundType.Low, EChannel.Click ) );
										}
										
									}
								}
							}

							// 最初にサウンドの再生を開始するチップの位置に、録音開始トリガのマーカーを挿入
							for (int i = 0; i < listChip.Count; i++)
							{
								if (listChip[i].bWAVを使うチャンネルである)
								{
									int playPosition = listChip[i].n発声位置;
									CChip chip = new CChip(playPosition, 36 * 36 - 1, EChannel.FirstSoundChip);
									this.listChip.Insert(i, chip);
									break;
								}
							}

							this.listChip.Sort();
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "拍子・拍線挿入時間:       {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ C2 [拍線・小節線表示指定] の処理 ]		// #28145 2012.4.21 yyagi; 2重ループをほぼ1重にして高速化
						bool bShowBeatBarLine = true;
						for (int i = 0; i < this.listChip.Count; i++)
						{
							bool bChangedBeatBarStatus = false;
							if ((this.listChip[i].eチャンネル番号 == EChannel.BeatLineDisplay))
							{
								if (this.listChip[i].n整数値 == 1)       // BAR/BEAT LINE = ON
								{
									bShowBeatBarLine = true;
									bChangedBeatBarStatus = true;
								}
								else if (this.listChip[i].n整数値 == 2)      // BAR/BEAT LINE = OFF
								{
									bShowBeatBarLine = false;
									bChangedBeatBarStatus = true;
								}
							}
							int startIndex = i;
							if (bChangedBeatBarStatus)              // C2チップの前に50/51チップが来ている可能性に配慮
							{
								while (startIndex > 0 && this.listChip[startIndex].n発声位置 == this.listChip[i].n発声位置)
								{
									startIndex--;
								}
								startIndex++; // 1つ小さく過ぎているので、戻す
							}
							for (int j = startIndex; j <= i; j++)
							{
								if (((this.listChip[j].eチャンネル番号 == EChannel.BarLine) || (this.listChip[j].eチャンネル番号 == EChannel.BeatLine)) &&
									(this.listChip[j].n整数値 == (36 * 36 - 1)))
								{
									this.listChip[j].b可視 = bShowBeatBarLine;
								}
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "C2 [拍線・小節線表示指定]:  {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ 発声時刻の計算 ]
						double bpm = 120.0;
						double dbBarLength = 1.0;
						int nPlayPosition = 0;
						int ms = 0;
						int nBar = 0;
						foreach (CChip chip in this.listChip)
						{
							chip.CalculatePlayPositionMs(e種別, BASEBPM, listBPM, listAVIPAN, listBGAPAN,
								ref bpm, ref dbBarLength, ref nPlayPosition, ref ms, ref nBar);
						}
						if (this.db再生速度 > 0.0)
						{
							double _db再生速度 = (CDTXMania.Instance.DTXVmode.Enabled) ? this.dbDTXVPlaySpeed : this.db再生速度;
							foreach (CChip chip in this.listChip)
							{
								chip.ApplyPlaySpeed(_db再生速度);
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "発声時刻計算:             {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						this.nBGMAdjust = 0;
						this.t各自動再生音チップの再生時刻を変更する(nBGMAdjust);
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "再生時刻変更:             {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ 可視チップ数カウント ]
						for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
						{
							this.n可視チップ数[inst] = 0;
						}
						foreach (CChip chip in this.listChip)
						{
							if (chip.bDrums可視チップ && !chip.b空打ちチップである)
							{
								this.n可視チップ数.Drums++;
							}
							if (chip.bGuitar可視チップ)
							{
								this.n可視チップ数.Guitar++;
							}
							if (chip.bBass可視チップ)
							{
								this.n可視チップ数.Bass++;
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "可視チップ数カウント      {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ チップの種類を分類し、対応するフラグを立てる ]
						foreach (CChip chip in this.listChip)
						{
							if ((
								chip.bWAVを使うチャンネルである &&
								this.listWAV.ContainsKey(chip.n整数値_内部番号)) &&
								!this.listWAV[chip.n整数値_内部番号].listこのWAVを使用するチャンネル番号の集合.Contains(chip.eチャンネル番号))
							{
								this.listWAV[chip.n整数値_内部番号].listこのWAVを使用するチャンネル番号の集合.Add(chip.eチャンネル番号);

								switch (chip.ESoundChipTypeを得る)
								{
									case ESoundChipType.Drums:
										if (!chip.b空打ちチップである)
										{
											this.listWAV[chip.n整数値_内部番号].bIsDrumsSound = true;
										}
										break;
									case ESoundChipType.Guitar:
										this.listWAV[chip.n整数値_内部番号].bIsGuitarSound = true; break;
									case ESoundChipType.Bass:
										this.listWAV[chip.n整数値_内部番号].bIsBassSound = true; break;
									case ESoundChipType.SE:
										this.listWAV[chip.n整数値_内部番号].bIsSESound = true; break;
									case ESoundChipType.BGM:
										this.listWAV[chip.n整数値_内部番号].bIsBGMSound = true; break;
								}
							}
						}
						#endregion
						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "ch番号集合確認:           {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ hash値計算 ]
						byte[] buffer = null;
						try
						{
							FileStream stream = new FileStream(this.strファイル名の絶対パス, FileMode.Open, FileAccess.Read);
							buffer = new byte[stream.Length];
							stream.Read(buffer, 0, (int)stream.Length);
							stream.Close();
						}
						catch (Exception exception)
						{
							Trace.TraceError(exception.Message);
							Trace.TraceError("DTXのハッシュの計算に失敗しました。({0})", this.strファイル名の絶対パス);
						}
						if (buffer != null)
						{
							byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash(buffer);
							StringBuilder sb = new StringBuilder();
							foreach (byte b in buffer2)
							{
								sb.Append(b.ToString("x2"));
							}
							this.strハッシュofDTXファイル = sb.ToString();
						}
						else
						{
							this.strハッシュofDTXファイル = "00000000000000000000000000000000";
						}
						#endregion

						// #36177 使用レーン数の表示 add ikanick 16.03.20
						#region [ 使用レーン数カウント ]
						#region [Drums]
						if (this.bチップがある.LeftPedal || this.bチップがある.LeftBassDrum)
						{
							this.n使用レーン数.Drums = EUseLanes.Dr_12;
						}
						else if (this.bチップがある.FloorTom || this.bチップがある.HHOpen || this.bチップがある.Ride || this.bチップがある.LeftCymbal)
						{
							this.n使用レーン数.Drums = EUseLanes.Dr_10;
						}
						else if (this.bチップがある.Drums)
						{
							this.n使用レーン数.Drums = EUseLanes.Dr_6;
						}
						else
						{
							this.n使用レーン数.Drums = EUseLanes.Other;
						}
						#endregion
						#region [Guitar]
						if ( this.bチップがある.Guitar)
						{
							// 5レーン未対応
							this.n使用レーン数.Guitar = EUseLanes.GB_3;
						}
						else
						{
							this.n使用レーン数.Guitar = EUseLanes.Other;
						}
						#endregion
						#region [Bass]
						if (this.bチップがある.Bass)
						{
							// 5レーン未対応
							this.n使用レーン数.Bass = EUseLanes.GB_3;
						}
						else
						{
							this.n使用レーン数.Bass = EUseLanes.Other;
						}
						#endregion
						//Trace.TraceInformation("FloorTom使用 =" + this.bチップがある.FloorTom);
						//Trace.TraceInformation("HiHatOpen使用=" + this.bチップがある.HHOpen);
						//Trace.TraceInformation("RideCymbal使用=" + this.bチップがある.Ride);
						//Trace.TraceInformation("LeftCymbal使用=" + this.bチップがある.LeftCymbal);
						//Trace.TraceInformation("LeftPedal使用=" + this.bチップがある.LeftPedal);
						//Trace.TraceInformation("LeftBass使用 =" + this.bチップがある.LeftBassDrum);
						//Trace.TraceInformation("Drumsチップあり =" + this.bチップがある.Drums);
						//Trace.TraceInformation("Lane Type    =" + this.n使用レーン数.Drums);
						#endregion

						//span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						//Trace.TraceInformation( "hash計算:                 {0}", span.ToString() );
						//timeBeginLoad = DateTime.Now;
						#region [ bLogDTX詳細ログ出力 ]
						if (CDTXMania.Instance.ConfigIni.bLogDTX)
						{
							foreach (CWAV cwav in this.listWAV.Values)
							{
								Trace.TraceInformation(cwav.ToString());
							}
							foreach (CAVI cavi in this.listAVI.Values)
							{
								Trace.TraceInformation(cavi.ToString());
							}
							foreach (CAVIPAN cavipan in this.listAVIPAN.Values)
							{
								Trace.TraceInformation(cavipan.ToString());
							}
							foreach (CBGA cbga in this.listBGA.Values)
							{
								Trace.TraceInformation(cbga.ToString());
							}
							foreach (CBGAPAN cbgapan in this.listBGAPAN.Values)
							{
								Trace.TraceInformation(cbgapan.ToString());
							}
							foreach (CBMP cbmp in this.listBMP.Values)
							{
								Trace.TraceInformation(cbmp.ToString());
							}
							foreach (CBMPTEX cbmptex in this.listBMPTEX.Values)
							{
								Trace.TraceInformation(cbmptex.ToString());
							}
							foreach (CBPM cbpm3 in this.listBPM.Values)
							{
								Trace.TraceInformation(cbpm3.ToString());
							}
							foreach (CChip chip in this.listChip)
							{
								Trace.TraceInformation(chip.ToString());
							}
						}
						#endregion


					}
				}
			}
		}

		/// <summary>
		/// #34016 LP/LBD使用譜面の吸収
		/// </summary>
		public void ReassignLP()
		{
			if ( this.bチップがある.LeftPedal )
			{
				if ( this.bチップがある.LeftBassDrum )
				{	// LP かつ LBDがある場合
					// → * LBDはBDに割り当て
					//    * LPは、HO(foot splash)に割り当て。あるいは、HO(foot splash)とHC(close)のいずれかに割り当てる。
					//      BPMから4分音符の長さを計算し、それより長いかどうかでHO/HCの仕分けを決定。 
					//    * チップ音にサウンドファイルが割り当てられていない場合は、HCに割り当て 
					//    * HO未使用の譜面の場合は、HCに割り当て

					double bpm = this.BPM + this.BASEBPM;
					double dbBarLength = 1.0;
					double nLen4thNoteMs = (int) ( ( 60.0 / bpm / dbBarLength ) * 10 * 10 * 10 );

					for ( int i = 0; i < this.listChip.Count; i++ )
					{
						// switch-caseにすると、listchip[i]の書き換えができないので、if-elseで記述
						if ( this.listChip[ i ].eチャンネル番号 == EChannel.LeftBassDrum )
						{
							this.listChip[ i ].eチャンネル番号 = EChannel.BassDrum;
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.LeftPedal )
						{
							int len = this.listChip[ i ].GetDuration();		// WAV未割当の場合は0が返る
																			// HHOpen未使用の譜面であれば、無条件にHHCloseに落としこむ
							this.listChip[ i ].eチャンネル番号 = ( len < nLen4thNoteMs || !this.bチップがある.HHOpen ) ? EChannel.HiHatClose : EChannel.HiHatOpen;
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BPM )
						{
							bpm = this.BPM + this.BASEBPM;
							nLen4thNoteMs = (int) ( ( 60.0 / bpm / dbBarLength / 2 ) * 10 * 10 * 10 );
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BPMEx )
						{
							int n内部番号 = listChip[ i ].n整数値_内部番号;
							if ( listBPM.ContainsKey( n内部番号 ) )
							{
								bpm = ( ( listBPM[ n内部番号 ].n表記上の番号 == 0 ) ? 0.0 : this.BASEBPM ) + listBPM[ n内部番号 ].dbBPM値;
							}
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BarLength )
						{
							dbBarLength = this.listChip[ i ].db実数値;
						}
					}
				}
				else
				{	// LPしかない場合
					// → LPを、BD, HO, HCに割り当てる必要がある。
					//    * BDへの割り当ては、BDレーンで同じ音を使っているかどうかで決定 
					//    * HO, HCへの割り当ては、(BDの可能性を除いた後) HOにアサイン、あるいはチップの長さを見てHC/HOにアサイン
					//    * チップ音にサウンドファイルが割り当てられていない場合は、HCに割り当て 
					//    * HO未使用の譜面の場合は、HCに割り当て

					double bpm = this.BPM + this.BASEBPM;
					double dbBarLength = 1.0;
					double nLen4thNoteMs = (int) ( ( 60.0 / bpm / dbBarLength ) * 10 * 10 * 10 );

					#region [ BassDrumのファイル名一覧を作成 ]
					List<string> listBDFilenames = new List<string>();
					foreach ( CChip chip in listChip )
					{
						if (chip.eチャンネル番号 == EChannel.BassDrum)
						{
							string s = chip.GetSoundFilename();
							if (s != null && !listBDFilenames.Contains(s))
							{
								listBDFilenames.Add(s);
							}
						}
					}
					#endregion

					for ( int i = 0; i < this.listChip.Count; i++ )
					{
						if ( this.listChip[ i ].eチャンネル番号 == EChannel.LeftPedal )
						{
							string s = listChip[i].GetSoundFilename();
							if (listBDFilenames.Contains(s))
							{
								this.listChip[i].eチャンネル番号 = EChannel.BassDrum;
							}
							else
							{
								int len = this.listChip[ i ].GetDuration();		// WAV未割当の場合は0が返る
																				// HHOpen未使用の譜面であれば、無条件にHHCloseに落としこむ
								this.listChip[ i ].eチャンネル番号 = ( len < nLen4thNoteMs || !this.bチップがある.HHOpen ) ? EChannel.HiHatClose : EChannel.HiHatOpen;
							}
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BPM )
						{
							bpm = this.BPM + this.BASEBPM;
							nLen4thNoteMs = (int) ( ( 60.0 / bpm / dbBarLength / 2 ) * 10 * 10 * 10 );
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BPMEx )
						{
							int n内部番号 = listChip[ i ].n整数値_内部番号;
							if ( listBPM.ContainsKey( n内部番号 ) )
							{
								bpm = ( ( listBPM[ n内部番号 ].n表記上の番号 == 0 ) ? 0.0 : this.BASEBPM ) + listBPM[ n内部番号 ].dbBPM値;
							}
						}
						else if ( this.listChip[ i ].eチャンネル番号 == EChannel.BarLength )
						{
							dbBarLength = this.listChip[ i ].db実数値;
						}
					}
					listBDFilenames.Clear();
					listBDFilenames = null;
				}
			}
			else if ( this.bチップがある.LeftBassDrum )
			{	// LBDのみがある場合
				// → そのままBDに割り当て
				for ( int i = 0; i < this.listChip.Count; i++ )
				{
					if ( this.listChip[ i ].eチャンネル番号 == EChannel.LeftBassDrum )
					{
						this.listChip[ i ].eチャンネル番号 = EChannel.BassDrum;
					}
				}
			}
			else
			{	// LPもLBDもない場合
				// → 何もしない
			}
		}
		private bool t入力_コマンド文字列を抜き出す(ref CharEnumerator ce, ref StringBuilder sb文字列)
		{
			if (!this.t入力_空白をスキップする(ref ce))
				return false; // 文字が尽きた

			#region [ コマンド終端文字(':')、半角空白、コメント開始文字(';')、改行のいずれかが出現するまでをコマンド文字列と見なし、sb文字列 にコピーする。]
			//-----------------
			while (ce.Current != ':' && ce.Current != ' ' && ce.Current != ';' && ce.Current != '\n')
			{
				sb文字列.Append(ce.Current);

				if (!ce.MoveNext())
					return false; // 文字が尽きた
			}
			//-----------------
			#endregion

			#region [ コマンド終端文字(':')で終端したなら、その次から空白をスキップしておく。]
			//-----------------
			if (ce.Current == ':')
			{
				if (!ce.MoveNext())
					return false; // 文字が尽きた

				if (!this.t入力_空白をスキップする(ref ce))
					return false; // 文字が尽きた
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_コメントをスキップする(ref CharEnumerator ce)
		{
			// 改行が現れるまでをコメントと見なしてスキップする。

			while (ce.Current != '\n')
			{
				if (!ce.MoveNext())
					return false; // 文字が尽きた
			}

			// 改行の次の文字へ移動した結果を返す。

			return ce.MoveNext();
		}
		private bool t入力_コメント文字列を抜き出す(ref CharEnumerator ce, ref StringBuilder sb文字列)
		{
			if (ce.Current != ';')    // コメント開始文字(';')じゃなければ正常帰還。
				return true;

			if (!ce.MoveNext())   // ';' の次で文字列が終わってたら終了帰還。
				return false;

			#region [ ';' の次の文字から '\n' の１つ前までをコメント文字列と見なし、sb文字列にコピーする。]
			//-----------------
			while (ce.Current != '\n')
			{
				sb文字列.Append(ce.Current);

				if (!ce.MoveNext())
					return false;
			}
			//-----------------
			#endregion

			return true;
		}
		private void t入力_パラメータ食い込みチェック(string strコマンド名, ref string strコマンド, ref string strパラメータ)
		{
			if ((strコマンド.Length > strコマンド名.Length) && strコマンド.StartsWith(strコマンド名, StringComparison.OrdinalIgnoreCase))
			{
				strパラメータ = strコマンド.Substring(strコマンド名.Length).Trim();
				strコマンド = strコマンド.Substring(0, strコマンド名.Length);
			}
		}
		private bool t入力_パラメータ文字列を抜き出す(ref CharEnumerator ce, ref StringBuilder sb文字列)
		{
			if (!this.t入力_空白をスキップする(ref ce))
				return false; // 文字が尽きた

			#region [ 改行またはコメント開始文字(';')が出現するまでをパラメータ文字列と見なし、sb文字列 にコピーする。]
			//-----------------
			while (ce.Current != '\n' && ce.Current != ';')
			{
				sb文字列.Append(ce.Current);

				if (!ce.MoveNext())
					return false;
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_空白と改行をスキップする(ref CharEnumerator ce)
		{
			// 空白と改行が続く間はこれらをスキップする。

			while (ce.Current == ' ' || ce.Current == '\n')
			{
				if (ce.Current == '\n')
					this.n現在の行数++;    // 改行文字では行番号が増える。

				if (!ce.MoveNext())
					return false; // 文字が尽きた
			}

			return true;
		}
		private bool t入力_空白をスキップする(ref CharEnumerator ce)
		{
			// 空白が続く間はこれをスキップする。

			while (ce.Current == ' ')
			{
				if (!ce.MoveNext())
					return false; // 文字が尽きた
			}

			return true;
		}
		private void t入力_行解析(ref StringBuilder sbコマンド, ref StringBuilder sbパラメータ, ref StringBuilder sbコメント)
		{
			string strコマンド = sbコマンド.ToString();
			string strパラメータ = sbパラメータ.ToString().Trim();
			string strコメント = sbコメント.ToString();

			// 行頭コマンドの処理

			#region [ IF ]
			//-----------------
			if (strコマンド.StartsWith("IF", StringComparison.OrdinalIgnoreCase))
			{
				this.t入力_パラメータ食い込みチェック("IF", ref strコマンド, ref strパラメータ);

				if (this.bstackIFからENDIFをスキップする.Count == 255)
				{
					Trace.TraceWarning("#IF の入れ子の数が 255 を超えました。この #IF を無視します。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				}
				else if (this.bstackIFからENDIFをスキップする.Peek())
				{
					this.bstackIFからENDIFをスキップする.Push(true); // 親が true ならその入れ子も問答無用で true 。
				}
				else                          // 親が false なら入れ子はパラメータと乱数を比較して結果を判断する。
				{
					int n数値 = 0;

					if (!int.TryParse(strパラメータ, out n数値))
						n数値 = 1;

					this.bstackIFからENDIFをスキップする.Push(n数値 != this.n現在の乱数);   // 乱数と数値が一致したら true 。
				}
			}
			//-----------------
			#endregion
			#region [ ENDIF ]
			//-----------------
			else if (strコマンド.StartsWith("ENDIF", StringComparison.OrdinalIgnoreCase))
			{
				this.t入力_パラメータ食い込みチェック("ENDIF", ref strコマンド, ref strパラメータ);

				if (this.bstackIFからENDIFをスキップする.Count > 1)
				{
					this.bstackIFからENDIFをスキップする.Pop();    // 入れ子を１つ脱出。
				}
				else
				{
					Trace.TraceWarning("#ENDIF に対応する #IF がありません。この #ENDIF を無視します。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				}
			}
			//-----------------
			#endregion

			else if (!this.bstackIFからENDIFをスキップする.Peek())   // IF～ENDIF をスキップするなら以下はすべて無視。
			{
				#region [ PATH_WAV ]
				//-----------------
				if (strコマンド.StartsWith("PATH_WAV", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("PATH_WAV", ref strコマンド, ref strパラメータ);
					this.PATH_WAV = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ TITLE ]
				//-----------------
				else if (strコマンド.StartsWith("TITLE", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("TITLE", ref strコマンド, ref strパラメータ);
					this.TITLE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ ARTIST ]
				//-----------------
				else if (strコマンド.StartsWith("ARTIST", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("ARTIST", ref strコマンド, ref strパラメータ);
					this.ARTIST = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ COMMENT ]
				//-----------------
				else if (strコマンド.StartsWith("COMMENT", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("COMMENT", ref strコマンド, ref strパラメータ);
					this.COMMENT = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ DLEVEL, PLAYLEVEL ]
				//-----------------
				else if (
					strコマンド.StartsWith("DLEVEL", StringComparison.OrdinalIgnoreCase) ||
					strコマンド.StartsWith("PLAYLEVEL", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("DLEVEL", ref strコマンド, ref strパラメータ);
					this.t入力_パラメータ食い込みチェック("PLAYLEVEL", ref strコマンド, ref strパラメータ);

					int dlevel;
					if (int.TryParse(strパラメータ, out dlevel))
					{
						this.LEVEL.Drums = Math.Min(Math.Max(dlevel, 0), 100);  // 0～100 に丸める
					}
				}
				//-----------------
				#endregion
				#region [ GLEVEL ]
				//-----------------
				else if (strコマンド.StartsWith("GLEVEL", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("GLEVEL", ref strコマンド, ref strパラメータ);

					int glevel;
					if (int.TryParse(strパラメータ, out glevel))
					{
						this.LEVEL.Guitar = Math.Min(Math.Max(glevel, 0), 100);   // 0～100 に丸める
					}
				}
				//-----------------
				#endregion
				#region [ BLEVEL ]
				//-----------------
				else if (strコマンド.StartsWith("BLEVEL", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("BLEVEL", ref strコマンド, ref strパラメータ);

					int blevel;
					if (int.TryParse(strパラメータ, out blevel))
					{
						this.LEVEL.Bass = Math.Min(Math.Max(blevel, 0), 100);   // 0～100 に丸める
					}
				}
				//-----------------
				#endregion
#if TEST_NOTEOFFMODE
				else if (str.StartsWith("SUPRESSNOTEOFF_HIHAT", StringComparison.OrdinalIgnoreCase)) {
					this.t入力・パラメータ食い込みチェック("SUPRESSNOTEOFF_HIHAT", ref str, ref str2);
					this.bHH演奏で直前のHHを消音する = !str2.ToLower().Equals("on");
				} 
				else if (str.StartsWith("SUPRESSNOTEOFF_GUITAR", StringComparison.OrdinalIgnoreCase)) {
					this.t入力・パラメータ食い込みチェック("SUPRESSNOTEOFF_GUITAR", ref str, ref str2);
					this.bGUITAR演奏で直前のGUITARを消音する = !str2.ToLower().Equals("on");
				}
				else if (str.StartsWith("SUPRESSNOTEOFF_BASS", StringComparison.OrdinalIgnoreCase)) {
					this.t入力・パラメータ食い込みチェック("SUPRESSNOTEOFF_BASS", ref str, ref str2);
					this.bBASS演奏で直前のBASSを消音する = !str2.ToLower().Equals("on");
				}
#endif
				#region [ GENRE ]
				//-----------------
				else if (strコマンド.StartsWith("GENRE", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("GENRE", ref strコマンド, ref strパラメータ);
					this.GENRE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ HIDDENLEVEL ]
				//-----------------
				else if (strコマンド.StartsWith("HIDDENLEVEL", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("HIDDENLEVEL", ref strコマンド, ref strパラメータ);
					this.HIDDENLEVEL = strパラメータ.ToLower().Equals("on");
				}
				//-----------------
				#endregion
				#region [ STAGEFILE ]
				//-----------------
				else if (strコマンド.StartsWith("STAGEFILE", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("STAGEFILE", ref strコマンド, ref strパラメータ);
					this.STAGEFILE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREVIEW ]
				//-----------------
				else if (strコマンド.StartsWith("PREVIEW", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("PREVIEW", ref strコマンド, ref strパラメータ);
					this.PREVIEW = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREIMAGE ]
				//-----------------
				else if (strコマンド.StartsWith("PREIMAGE", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("PREIMAGE", ref strコマンド, ref strパラメータ);
					this.PREIMAGE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREMOVIE ]
				//-----------------
				else if (strコマンド.StartsWith("PREMOVIE", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("PREMOVIE", ref strコマンド, ref strパラメータ);
					this.PREMOVIE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ USE 556 x 710 BGAAVI ]

				else if (strコマンド.StartsWith("USE556X710BGAAVI", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("USE556X710BGAAVI", ref strコマンド, ref strパラメータ);
					this.bUse556x710BGAAVI = strパラメータ == "1" ? true : false;
				}
				#endregion
				#region [ BACKGROUND_GR ]
				//-----------------
				else if (strコマンド.StartsWith("BACKGROUND_GR", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("BACKGROUND_GR", ref strコマンド, ref strパラメータ);
					this.BACKGROUND_GR = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ BACKGROU}ND, WALL ]
				//-----------------
				else if (
					strコマンド.StartsWith("BACKGROUND", StringComparison.OrdinalIgnoreCase) ||
					strコマンド.StartsWith("WALL", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("BACKGROUND", ref strコマンド, ref strパラメータ);
					this.t入力_パラメータ食い込みチェック("WALL", ref strコマンド, ref strパラメータ);
					this.BACKGROUND = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ RANDOM ]
				//-----------------
				else if (strコマンド.StartsWith("RANDOM", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("RANDOM", ref strコマンド, ref strパラメータ);

					int n数値 = 1;
					if (!int.TryParse(strパラメータ, out n数値))
						n数値 = 1;

					this.n現在の乱数 = CDTXMania.Instance.Random.Next(n数値) + 1;    // 1～数値 までの乱数を生成。
				}
				//-----------------
				#endregion
				#region [ SOUND_NOWLOADING ]
				//-----------------
				else if (strコマンド.StartsWith("SOUND_NOWLOADING", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("SOUND_NOWLOADING", ref strコマンド, ref strパラメータ);
					this.SOUND_NOWLOADING = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ BPM ]
				//-----------------
				else if (strコマンド.StartsWith("BPM", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_行解析_BPM_BPMzz(strコマンド, strパラメータ, strコメント);
				}
				//-----------------
				#endregion
				#region [ DTXVPLAYSPEED ]
				//-----------------
				else if (strコマンド.StartsWith("DTXVPLAYSPEED", StringComparison.OrdinalIgnoreCase))
				{
					this.t入力_パラメータ食い込みチェック("DTXVPLAYSPEED", ref strコマンド, ref strパラメータ);

					double dtxvplayspeed = 0.0;
					if (TryParse(strパラメータ, out dtxvplayspeed) && dtxvplayspeed > 0.0)
					{
						this.dbDTXVPlaySpeed = dtxvplayspeed;
					}
				}
				//-----------------
				#endregion
				else if (this.bヘッダのみ && this.bレーン情報を確認する)	// ヘッダのみ、かつbLoadDetailDTX==trueの場合は、チップの配置のみ実行。
				{
					this.t入力_行解析_チップ配置(strコマンド, strパラメータ, strコメント);
				}
				//else if (!this.bヘッダのみ && this.bレーン情報を確認する)   // ヘッダのみ、かつbLoadDetailDTX==trueの場合は、チップの配置のみ実行。
				//{
				//	this.t入力_行解析_チップ配置(strコマンド, strパラメータ, strコメント);
				//}
				else if (!this.bヘッダのみ)	// ヘッダのみの解析の場合、以下は無視。
				{
					#region [ PANEL ]
					//-----------------
					if (strコマンド.StartsWith("PANEL", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("PANEL", ref strコマンド, ref strパラメータ);

						int dummyResult;                // #23885 2010.12.12 yyagi: not to confuse "#PANEL strings (panel)" and "#PANEL int (panpot of EL)"
						if (!int.TryParse(strパラメータ, out dummyResult))
						{   // 数値じゃないならPANELとみなす
							this.PANEL = strパラメータ;              //
							goto EOL;                 //
						}                       // 数値ならPAN ELとみなす

					}
					//-----------------
					#endregion
					#region [ MIDIFILE ]
					//-----------------
					else if (strコマンド.StartsWith("MIDIFILE", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("MIDIFILE", ref strコマンド, ref strパラメータ);
						this.MIDIFILE = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ MIDINOTE ]
					//-----------------
					else if (strコマンド.StartsWith("MIDINOTE", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("MIDINOTE", ref strコマンド, ref strパラメータ);
						this.MIDINOTE = strパラメータ.ToLower().Equals("on");
					}
					//-----------------
					#endregion
					#region [ BLACKCOLORKEY ]
					//-----------------
					else if (strコマンド.StartsWith("BLACKCOLORKEY", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("BLACKCOLORKEY", ref strコマンド, ref strパラメータ);
						this.BLACKCOLORKEY = strパラメータ.ToLower().Equals("on");
					}
					//-----------------
					#endregion
					#region [ BASEBPM ]
					//-----------------
					else if (strコマンド.StartsWith("BASEBPM", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("BASEBPM", ref strコマンド, ref strパラメータ);

						double basebpm = 0.0;
						//if( double.TryParse( str2, out num6 ) && ( num6 > 0.0 ) )
						if (TryParse(strパラメータ, out basebpm) && basebpm > 0.0) // #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
						{                         // #24204 2011.01.21 yyagi: Fix the condition correctly
							this.BASEBPM = basebpm;
						}
					}
					//-----------------
					#endregion
					#region [ SOUND_STAGEFAILED ]
					//-----------------
					else if (strコマンド.StartsWith("SOUND_STAGEFAILED", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("SOUND_STAGEFAILED", ref strコマンド, ref strパラメータ);
						this.SOUND_STAGEFAILED = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ SOUND_FULLCOMBO ]
					//-----------------
					else if (strコマンド.StartsWith("SOUND_FULLCOMBO", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("SOUND_FULLCOMBO", ref strコマンド, ref strパラメータ);
						this.SOUND_FULLCOMBO = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ SOUND_AUDIENCE ]
					//-----------------
					else if (strコマンド.StartsWith("SOUND_AUDIENCE", StringComparison.OrdinalIgnoreCase))
					{
						this.t入力_パラメータ食い込みチェック("SOUND_AUDIENCE", ref strコマンド, ref strパラメータ);
						this.SOUND_AUDIENCE = strパラメータ;
					}
					//-----------------
					#endregion

					// オブジェクト記述コマンドの処理。

					else if (!this.t入力_行解析_WAVVOL_VOLUME(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_WAVPAN_PAN(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_WAV(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_BMPTEX(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_BMP(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_BGAPAN(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_BGA(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_AVIPAN(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_AVI_VIDEO(strコマンド, strパラメータ, strコメント) &&
						//	!this.t入力_行解析_BPM_BPMzz( strコマンド, strパラメータ, strコメント ) &&	// bヘッダのみ==trueの場合でもチェックするよう変更
						!this.t入力_行解析_RESULTIMAGE(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_RESULTMOVIE(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_RESULTSOUND(strコマンド, strパラメータ, strコメント) &&
						!this.t入力_行解析_SIZE(strコマンド, strパラメータ, strコメント))
					{
						this.t入力_行解析_チップ配置(strコマンド, strパラメータ, strコメント);
					}
					EOL:
					Debug.Assert(true);   // #23885 2010.12.12 yyagi: dummy line to exit parsing the line
																// 2011.8.17 from: "int xx=0;" から変更。毎回警告が出るので。
				}
				//else
				//{	// Duration測定のため、bヘッダのみ==trueでも、チップ配置は行う
				//	this.t入力・行解析・チップ配置( strコマンド, strパラメータ, strコメント );
				//}
			}
		}
		private bool t入力_行解析_AVI_VIDEO(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "AVI" or "VIDEO" で始まらないコマンドは無効。]
			//-----------------
			if (strコマンド.StartsWith("AVI", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(3);   // strコマンド から先頭の"AVI"文字を除去。

			else if (strコマンド.StartsWith("VIDEO", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(5);   // strコマンド から先頭の"VIDEO"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // AVI番号 zz がないなら無効。

			#region [ AVI番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("AVI(VIDEO)番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			#region [ AVIリストに {zz, avi} の組を登録する。 ]
			//-----------------
			var avi = new CAVI(zz, strパラメータ, strコメント, CDTXMania.Instance.ConfigIni.nPlaySpeed);

			if (this.listAVI.ContainsKey(zz)) // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listAVI.Remove(zz);

			this.listAVI.Add(zz, avi);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_AVIPAN(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "AVIPAN" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("AVIPAN", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(6); // strコマンド から先頭の"AVIPAN"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // AVIPAN番号 zz がないなら無効。

			#region [ AVIPAN番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("AVIPAN番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			var avipan = new CAVIPAN()
			{
				n番号 = zz,
			};

			// パラメータ引数（14個）を取得し、avipan に登録していく。

			string[] strParams = strパラメータ.Split(new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries);

			#region [ パラメータ引数は全14個ないと無効。]
			//-----------------
			if (strParams.Length < 14)
			{
				Trace.TraceError("AVIPAN: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1. AVI番号 ]
			//-----------------
			if (string.IsNullOrEmpty(strParams[i]) || strParams[i].Length > 2)
			{
				Trace.TraceError("AVIPAN: {2}番目の数（AVI番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.nAVI番号 = C変換.n36進数2桁の文字列を数値に変換して返す(strParams[i]);
			if (avipan.nAVI番号 < 1 || avipan.nAVI番号 >= 36 * 36)
			{
				Trace.TraceError("AVIPAN: {2}番目の数（AVI番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2. 開始転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（開始転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.sz開始サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3. 転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（開始転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.sz開始サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4. 終了転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（終了転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.sz終了サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5. 終了転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（終了転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.sz終了サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6. 動画側開始位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（動画側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt動画側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7. 動画側開始位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（動画側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt動画側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 8. 動画側終了位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（動画側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt動画側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 9. 動画側終了位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（動画側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt動画側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 10.表示側開始位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（表示側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt表示側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 11.表示側開始位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（表示側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt表示側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 12.表示側終了位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（表示側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt表示側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 13.表示側終了位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（表示側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			avipan.pt表示側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 14.移動時間 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("AVIPAN: {2}番目の引数（移動時間）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}

			if (n値 < 0)
				n値 = 0;

			avipan.n移動時間ct = n値;
			i++;
			//-----------------
			#endregion

			#region [ AVIPANリストに {zz, avipan} の組を登録する。]
			//-----------------
			if (this.listAVIPAN.ContainsKey(zz))  // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listAVIPAN.Remove(zz);

			this.listAVIPAN.Add(zz, avipan);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_BGA(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "BGA" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("BGA", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(3); // strコマンド から先頭の"BGA"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // BGA番号 zz がないなら無効。

			#region [ BGA番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("BGA番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			var bga = new CBGA()
			{
				n番号 = zz,
			};

			// パラメータ引数（7個）を取得し、bga に登録していく。

			string[] strParams = strパラメータ.Split(new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries);

			#region [ パラメータ引数は全7個ないと無効。]
			//-----------------
			if (strParams.Length < 7)
			{
				Trace.TraceError("BGA: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1.BMP番号 ]
			//-----------------
			if (string.IsNullOrEmpty(strParams[i]) || strParams[i].Length > 2)
			{
				Trace.TraceError("BGA: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.nBMP番号 = C変換.n36進数2桁の文字列を数値に変換して返す(strParams[i]);
			if (bga.nBMP番号 < 1 || bga.nBMP番号 >= 36 * 36)
			{
				Trace.TraceError("BGA: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2.画像側位置１・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（画像側位置１・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt画像側左上座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3.画像側位置１・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（画像側位置１・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt画像側左上座標.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4.画像側位置２・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（画像側位置２・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt画像側右下座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5.画像側位置２・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（画像側座標２・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt画像側右下座標.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6.表示位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（表示位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt表示座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7.表示位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGA: {2}番目の引数（表示位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bga.pt表示座標.Y = n値;
			i++;
			//-----------------
			#endregion

			#region [ 画像側座標の正規化とクリッピング。]
			//-----------------
			if (bga.pt画像側左上座標.X > bga.pt画像側右下座標.X)
			{
				n値 = bga.pt画像側左上座標.X;
				bga.pt画像側左上座標.X = bga.pt画像側右下座標.X;
				bga.pt画像側右下座標.X = n値;
			}
			if (bga.pt画像側左上座標.Y > bga.pt画像側右下座標.Y)
			{
				n値 = bga.pt画像側左上座標.Y;
				bga.pt画像側左上座標.Y = bga.pt画像側右下座標.Y;
				bga.pt画像側右下座標.Y = n値;
			}
			//-----------------
			#endregion
			#region [ BGAリストに {zz, bga} の組を登録する。]
			//-----------------
			if (this.listBGA.ContainsKey(zz)) // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBGA.Remove(zz);

			this.listBGA.Add(zz, bga);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_BGAPAN(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "BGAPAN" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("BGAPAN", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(6); // strコマンド から先頭の"BGAPAN"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // BGAPAN番号 zz がないなら無効。

			#region [ BGAPAN番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("BGAPAN番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			var bgapan = new CBGAPAN()
			{
				n番号 = zz,
			};

			// パラメータ引数（14個）を取得し、bgapan に登録していく。

			string[] strParams = strパラメータ.Split(new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries);

			#region [ パラメータ引数は全14個ないと無効。]
			//-----------------
			if (strParams.Length < 14)
			{
				Trace.TraceError("BGAPAN: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1. BMP番号 ]
			//-----------------
			if (string.IsNullOrEmpty(strParams[i]) || strParams[i].Length > 2)
			{
				Trace.TraceError("BGAPAN: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.nBMP番号 = C変換.n36進数2桁の文字列を数値に変換して返す(strParams[i]);
			if (bgapan.nBMP番号 < 1 || bgapan.nBMP番号 >= 36 * 36)
			{
				Trace.TraceError("BGAPAN: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2. 開始転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（開始転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.sz開始サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3. 開始転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（開始転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.sz開始サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4. 終了転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（終了転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.sz終了サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5. 終了転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（終了転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.sz終了サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6. 画像側開始位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（画像側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt画像側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7. 画像側開始位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（画像側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt画像側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 8. 画像側終了位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（画像側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt画像側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 9. 画像側終了位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（画像側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt画像側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 10.表示側開始位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（表示側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt表示側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 11.表示側開始位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（表示側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt表示側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 12.表示側終了位置・X ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（表示側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt表示側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 13.表示側終了位置・Y ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（表示側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}
			bgapan.pt表示側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 14.移動時間 ]
			//-----------------
			n値 = 0;
			if (!int.TryParse(strParams[i], out n値))
			{
				Trace.TraceError("BGAPAN: {2}番目の引数（移動時間）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1);
				return false;
			}

			if (n値 < 0)
				n値 = 0;

			bgapan.n移動時間ct = n値;
			i++;
			//-----------------
			#endregion

			#region [ BGAPANリストに {zz, bgapan} の組を登録する。]
			//-----------------
			if (this.listBGAPAN.ContainsKey(zz))  // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBGAPAN.Remove(zz);

			this.listBGAPAN.Add(zz, bgapan);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_BMP(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "BMP" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("BMP", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(3); // strコマンド から先頭の"BMP"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			int zz = 0;

			#region [ BMP番号 zz を取得する。]
			//-----------------
			if (strコマンド.Length < 2)
			{
				#region [ (A) "#BMP:" の場合 → zz = 00 ]
				//-----------------
				zz = 0;
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) "#BMPzz:" の場合 → zz = 00 ～ ZZ ]
				//-----------------
				zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
				if (zz < 0 || zz >= 36 * 36)
				{
					Trace.TraceError("BMP番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
					return false;
				}
				//-----------------
				#endregion
			}
			//-----------------
			#endregion


			var bmp = new CBMP()
			{
				n番号 = zz,
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ BMPリストに {zz, bmp} の組を登録。]
			//-----------------
			if (this.listBMP.ContainsKey(zz)) // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBMP.Remove(zz);

			this.listBMP.Add(zz, bmp);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_BMPTEX(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "BMPTEX" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("BMPTEX", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(6); // strコマンド から先頭の"BMPTEX"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // BMPTEX番号 zz がないなら無効。

			#region [ BMPTEX番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("BMPTEX番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			var bmptex = new CBMPTEX()
			{
				n番号 = zz,
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ BMPTEXリストに {zz, bmptex} の組を登録する。]
			//-----------------
			if (this.listBMPTEX.ContainsKey(zz))  // 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBMPTEX.Remove(zz);

			this.listBMPTEX.Add(zz, bmptex);
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_BPM_BPMzz(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "BPM" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("BPM", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(3); // strコマンド から先頭の"BPM"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			int zz = 0;

			#region [ BPM番号 zz を取得する。]
			//-----------------
			if (strコマンド.Length < 2)
			{
				#region [ (A) "#BPM:" の場合 → zz = 00 ]
				//-----------------
				zz = 0;
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) "#BPMzz:" の場合 → zz = 00 ～ ZZ ]
				//-----------------
				zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
				if (zz < 0 || zz >= 36 * 36)
				{
					Trace.TraceError("BPM番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
					return false;
				}
				//-----------------
				#endregion
			}
			//-----------------
			#endregion

			double dbBPM = 0.0;

			#region [ BPM値を取得する。]
			//-----------------
			//if( !double.TryParse( strパラメータ, out result ) )
			if (!TryParse(strパラメータ, out dbBPM))     // #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
				return false;

			if (dbBPM <= 0.0)
				return false;
			//-----------------
			#endregion

			if (zz == 0)      // "#BPM00:" と "#BPM:" は等価。
				this.BPM = dbBPM; // この曲の代表 BPM に格納する。

			#region [ BPMリストに {内部番号, zz, dbBPM} の組を登録。]
			//-----------------
			this.listBPM.Add(
				this.n内部番号BPM1to,
				new CBPM()
				{
					n内部番号 = this.n内部番号BPM1to,
					n表記上の番号 = zz,
					dbBPM値 = dbBPM,
				});
			//-----------------
			#endregion

			#region [ BPM番号が zz であるBPM未設定のBPMチップがあれば、そのサイズを変更する。無限管理に対応。]
			//-----------------
			if (this.n無限管理BPM[zz] == -zz) // 初期状態では n無限管理BPM[zz] = -zz である。この場合、#BPMzz がまだ出現していないことを意味する。
			{
				foreach (CChip chip in listChip)  // これまでに出てきたチップのうち、該当する（BPM値が未設定の）BPMチップの値を変更する（仕組み上、必ず後方参照となる）。
				{
					chip.AdjustInfiniteManageIntInternalIndex(chip.bBPMチップである, zz, this.n内部番号BPM1to);
				}
			}
			this.n無限管理BPM[zz] = this.n内部番号BPM1to;     // 次にこの BPM番号 zz を使うBPMチップが現れたら、このBPM値が格納されることになる。
			this.n内部番号BPM1to++;   // 内部番号は単純増加連番。
														//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_RESULTIMAGE(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "RESULTIMAGE" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("RESULTIMAGE", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(11);  // strコマンド から先頭の"RESULTIMAGE"文字を除去。
																				//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTIMAGE:" と "#RESULTIMAGE_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if (strコマンド.Length < 2)
			{
				#region [ (A) ランク指定がない場合("#RESULTIMAGE:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for (int i = 0; i < 7; i++)
				{
					if (this.nRESULTIMAGE用優先順位[i] == 0)
						this.RESULTIMAGE[i] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTIMAGE_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch (strコマンド.ToUpper())
				{
					case "_SS":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(0, strパラメータ);
						break;

					case "_S":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(1, strパラメータ);
						break;

					case "_A":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(2, strパラメータ);
						break;

					case "_B":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(3, strパラメータ);
						break;

					case "_C":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(4, strパラメータ);
						break;

					case "_D":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(5, strパラメータ);
						break;

					case "_E":
						this.t入力_行解析_RESULTIMAGE_ファイルを設定する(6, strパラメータ);
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力_行解析_RESULTIMAGE_ファイルを設定する(int nランク0to6, string strファイル名)
		{
			if (nランク0to6 < 0 || nランク0to6 > 6) // 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for (int i = nランク0to6; i >= 0; i--)
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTIMAGE[] に限り、ファイル名を更新できる。
				//（例：#RESULTMOVIE_D が #RESULTIMAGE_A より後に出現しても、#RESULTIMAGE_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if (this.nRESULTIMAGE用優先順位[i] < n優先順位)
				{
					this.nRESULTIMAGE用優先順位[i] = n優先順位;
					this.RESULTIMAGE[i] = strファイル名;
				}
			}
		}
		private bool t入力_行解析_RESULTMOVIE(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "RESULTMOVIE" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("RESULTMOVIE", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(11);  // strコマンド から先頭の"RESULTMOVIE"文字を除去。
																				//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTMOVIE:" と "#RESULTMOVIE_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if (strコマンド.Length < 2)
			{
				#region [ (A) ランク指定がない場合("#RESULTMOVIE:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for (int i = 0; i < 7; i++)
				{
					if (this.nRESULTMOVIE用優先順位[i] == 0)
						this.RESULTMOVIE[i] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTMOVIE_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch (strコマンド.ToUpper())
				{
					case "_SS":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(0, strパラメータ);
						break;

					case "_S":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(1, strパラメータ);
						break;

					case "_A":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(2, strパラメータ);
						break;

					case "_B":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(3, strパラメータ);
						break;

					case "_C":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(4, strパラメータ);
						break;

					case "_D":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(5, strパラメータ);
						break;

					case "_E":
						this.t入力_行解析_RESULTMOVIE_ファイルを設定する(6, strパラメータ);
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力_行解析_RESULTMOVIE_ファイルを設定する(int nランク0to6, string strファイル名)
		{
			if (nランク0to6 < 0 || nランク0to6 > 6) // 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for (int i = nランク0to6; i >= 0; i--)
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTMOVIE[] に限り、ファイル名を更新できる。
				//（例：#RESULTMOVIE_D が #RESULTMOVIE_A より後に出現しても、#RESULTMOVIE_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if (this.nRESULTMOVIE用優先順位[i] < n優先順位)
				{
					this.nRESULTMOVIE用優先順位[i] = n優先順位;
					this.RESULTMOVIE[i] = strファイル名;
				}
			}
		}
		private bool t入力_行解析_RESULTSOUND(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "RESULTSOUND" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("RESULTSOUND", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(11);  // strコマンド から先頭の"RESULTSOUND"文字を除去。
																				//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTSOUND:" と "#RESULTSOUND_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if (strコマンド.Length < 2)
			{
				#region [ (A) ランク指定がない場合("#RESULTSOUND:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for (int i = 0; i < 7; i++)
				{
					if (this.nRESULTSOUND用優先順位[i] == 0)
						this.RESULTSOUND[i] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTSOUND_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch (strコマンド.ToUpper())
				{
					case "_SS":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(0, strパラメータ);
						break;

					case "_S":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(1, strパラメータ);
						break;

					case "_A":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(2, strパラメータ);
						break;

					case "_B":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(3, strパラメータ);
						break;

					case "_C":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(4, strパラメータ);
						break;

					case "_D":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(5, strパラメータ);
						break;

					case "_E":
						this.t入力_行解析_RESULTSOUND_ファイルを設定する(6, strパラメータ);
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力_行解析_RESULTSOUND_ファイルを設定する(int nランク0to6, string strファイル名)
		{
			if (nランク0to6 < 0 || nランク0to6 > 6) // 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for (int i = nランク0to6; i >= 0; i--)
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTSOUND[] に限り、ファイル名を更新できる。
				//（例：#RESULTSOUND_D が #RESULTSOUND_A より後に出現しても、#RESULTSOUND_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if (this.nRESULTSOUND用優先順位[i] < n優先順位)
				{
					this.nRESULTSOUND用優先順位[i] = n優先順位;
					this.RESULTSOUND[i] = strファイル名;
				}
			}
		}
		private bool t入力_行解析_SIZE(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "SIZE" で始まらないコマンドや、その後ろに2文字（番号）が付随してないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("SIZE", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(4); // strコマンド から先頭の"SIZE"文字を除去。

			if (strコマンド.Length < 2) // サイズ番号の指定がない場合は無効。
				return false;
			//-----------------
			#endregion

			#region [ nWAV番号（36進数2桁）を取得。]
			//-----------------
			int nWAV番号 = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));

			if (nWAV番号 < 0 || nWAV番号 >= 36 * 36)
			{
				Trace.TraceError("SIZEのWAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion


			// (2) パラメータを処理。

			#region [ nサイズ値 を取得する。値は 0～100 に収める。]
			//-----------------
			int nサイズ値;

			if (!int.TryParse(strパラメータ, out nサイズ値))
				return true;  // int変換に失敗しても、この行自体の処理は終えたのでtrueを返す。

			nサイズ値 = Math.Min(Math.Max(nサイズ値, 0), 100);  // 0未満は0、100超えは100に強制変換。
																									//-----------------
			#endregion

			#region [ nWAV番号で示されるサイズ未設定のWAVチップがあれば、そのサイズを変更する。無限管理に対応。]
			//-----------------
			if (this.n無限管理SIZE[nWAV番号] == -nWAV番号)  // 初期状態では n無限管理SIZE[xx] = -xx である。この場合、#SIZExx がまだ出現していないことを意味する。
			{
				foreach (CWAV wav in this.listWAV.Values)   // これまでに出てきたWAVチップのうち、該当する（サイズが未設定の）チップのサイズを変更する（仕組み上、必ず後方参照となる）。
				{
					if (wav.nチップサイズ == -nWAV番号)   // #SIZExx 行より前の行に出現した #WAVxx では、チップサイズは -xx に初期化されている。
						wav.nチップサイズ = nサイズ値;
				}
			}
			this.n無限管理SIZE[nWAV番号] = nサイズ値;     // 次にこの nWAV番号を使うWAVチップが現れたら、負数の代わりに、このサイズ値が格納されることになる。
																					//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_WAV(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "WAV" で始まらないコマンドは無効。]
			//-----------------
			if (!strコマンド.StartsWith("WAV", StringComparison.OrdinalIgnoreCase))
				return false;

			strコマンド = strコマンド.Substring(3); // strコマンド から先頭の"WAV"文字を除去。
																			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("WAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			var wav = new CWAV()
			{
				n内部番号 = this.n内部番号WAV1to,
				n表記上の番号 = zz,
				nチップサイズ = this.n無限管理SIZE[zz],
				n位置 = this.n無限管理PAN[zz],
				n音量 = this.n無限管理VOL[zz],
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ WAVリストに {内部番号, wav} の組を登録。]
			//-----------------
			this.listWAV.Add(this.n内部番号WAV1to, wav);
			//-----------------
			#endregion

			#region [ WAV番号が zz である内部番号未設定のWAVチップがあれば、その内部番号を変更する。無限管理対応。]
			//-----------------
			if (this.n無限管理WAV[zz] == -zz) // 初期状態では n無限管理WAV[zz] = -zz である。この場合、#WAVzz がまだ出現していないことを意味する。
			{
				foreach (CChip chip in listChip)  // これまでに出てきたチップのうち、該当する（内部番号が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
				{
					chip.AdjustInfiniteManageIntInternalIndex(chip.bWAVを使うチャンネルである, zz, n内部番号WAV1to);
				}
			}
			this.n無限管理WAV[zz] = this.n内部番号WAV1to;     // 次にこの WAV番号 zz を使うWAVチップが現れたら、この内部番号が格納されることになる。
			this.n内部番号WAV1to++;   // 内部番号は単純増加連番。
														//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_WAVPAN_PAN(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "WAVPAN" or "PAN" で始まらないコマンドは無効。]
			//-----------------
			if (strコマンド.StartsWith("WAVPAN", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(6);   // strコマンド から先頭の"WAVPAN"文字を除去。

			else if (strコマンド.StartsWith("PAN", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(3);   // strコマンド から先頭の"PAN"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("WAVPAN(PAN)のWAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			#region [ WAV番号 zz を持つWAVチップの位置を変更する。無限定義対応。]
			//-----------------
			int n位置;
			if (int.TryParse(strパラメータ, out n位置))
			{
				n位置 = Math.Min(Math.Max(n位置, -100), 100); // -100～+100 に丸める

				if (this.n無限管理PAN[zz] == (-10000 - zz)) // 初期状態では n無限管理PAN[zz] = -10000 - zz である。この場合、#WAVPANzz, #PANzz がまだ出現していないことを意味する。
				{
					foreach (CWAV wav in this.listWAV.Values) // これまでに出てきたチップのうち、該当する（位置が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
					{
						if (wav.n位置 == (-10000 - zz)) // #WAVPANzz, #PANzz 行より前の行に出現した #WAVzz では、位置は -10000-zz に初期化されている。
							wav.n位置 = n位置;
					}
				}
				this.n無限管理PAN[zz] = n位置;      // 次にこの WAV番号 zz を使うWAVチップが現れたら、この位置が格納されることになる。
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_WAVVOL_VOLUME(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			#region [ "WAVCOL" or "VOLUME" で始まらないコマンドは無効。]
			//-----------------
			if (strコマンド.StartsWith("WAVVOL", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(6);   // strコマンド から先頭の"WAVVOL"文字を除去。

			else if (strコマンド.StartsWith("VOLUME", StringComparison.OrdinalIgnoreCase))
				strコマンド = strコマンド.Substring(6);   // strコマンド から先頭の"VOLUME"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if (strコマンド.Length < 2)
				return false; // WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す(strコマンド.Substring(0, 2));
			if (zz < 0 || zz >= 36 * 36)
			{
				Trace.TraceError("WAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
				return false;
			}
			//-----------------
			#endregion

			#region [ WAV番号 zz を持つWAVチップの音量を変更する。無限定義対応。]
			//-----------------
			int n音量;
			if (int.TryParse(strパラメータ, out n音量))
			{
				n音量 = Math.Min(Math.Max(n音量, 0), 100);  // 0～100に丸める。

				if (this.n無限管理VOL[zz] == -zz) // 初期状態では n無限管理VOL[zz] = - zz である。この場合、#WAVVOLzz, #VOLUMEzz がまだ出現していないことを意味する。
				{
					foreach (CWAV wav in this.listWAV.Values) // これまでに出てきたチップのうち、該当する（音量が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
					{
						if (wav.n音量 == -zz) // #WAVVOLzz, #VOLUMEzz 行より前の行に出現した #WAVzz では、音量は -zz に初期化されている。
							wav.n音量 = n音量;
					}
				}
				this.n無限管理VOL[zz] = n音量;      // 次にこの WAV番号 zz を使うWAVチップが現れたら、この音量が格納されることになる。
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力_行解析_チップ配置(string strコマンド, string strパラメータ, string strコメント)
		{
			// (1) コマンドを処理。

			if (strコマンド.Length != 5)  // コマンドは必ず5文字であること。
				return false;

			#region [ n小節番号 を取得する。]
			//-----------------
			int n小節番号 = C変換.n小節番号の文字列3桁を数値に変換して返す(strコマンド.Substring(0, 3));
			if (n小節番号 < 0)
				return false;

			n小節番号++;  // 先頭に空の1小節を設ける。
								//-----------------
			#endregion

			#region [ nチャンネル番号 を取得する。]
			//-----------------
			EChannel tmpチャンネル番号 = EChannel.Invalid;

			// ファイルフォーマットによって処理が異なる。

			if (this.e種別 == EDTX種別.GDA || this.e種別 == EDTX種別.G2D)
			{
				#region [ (A) GDA, G2D の場合：チャンネル文字列をDTXのチャンネル番号へ置き換える。]
				//-----------------
				string strチャンネル文字列 = strコマンド.Substring(3, 2);

				foreach (STGDAPARAM param in this.stGDAParam)
				{
					if (strチャンネル文字列.Equals(param.strGDAのチャンネル文字列, StringComparison.OrdinalIgnoreCase))
					{
						tmpチャンネル番号 = param.eDTXのチャンネル番号;
						break;  // 置き換え成功
					}
				}
				if (tmpチャンネル番号 == EChannel.Invalid)
					return false; // 置き換え失敗
												//-----------------
				#endregion
			}
			else
			{
				#region [ (B) その他の場合：チャンネル番号は16進数2桁。]
				//-----------------
				tmpチャンネル番号 = (EChannel)C変換.n16進数2桁の文字列を数値に変換して返す(strコマンド.Substring(3, 2));

				if (tmpチャンネル番号 < 0)
					return false;
				//-----------------
				#endregion
			}
			//-----------------
			#endregion


			// (2) Ch.02を処理。
			#region [ 小節長変更(Ch.02)は他のチャンネルとはパラメータが特殊なので、先にとっとと終わらせる。 ]
			//-----------------
			if (tmpチャンネル番号 == EChannel.BarLength)
			{
				// 小節長倍率を取得する。
				double db小節長倍率 = 1.0;
				//if( !double.TryParse( strパラメータ, out result ) )
				if (!this.TryParse(strパラメータ, out db小節長倍率))      // #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
				{
					Trace.TraceError("小節長倍率に不正な値を指定しました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
					return false;
				}

				// 小節長倍率チップを一番先頭に配置する。
				this.listChip.Insert(0, new CChip(n小節番号 * 384, db小節長倍率, tmpチャンネル番号));

				return true;  // 配置終了。
			}
			//-----------------
			#endregion

			// (3) パラメータを処理。
			if (string.IsNullOrEmpty(strパラメータ))   // パラメータはnullまたは空文字列ではないこと。
				return false;

			#region [ strパラメータ にオブジェクト記述を格納し、その n文字数 をカウントする。]
			//-----------------
			int n文字数 = 0;

			var sb = new StringBuilder(strパラメータ.Length);

			// strパラメータを先頭から1文字ずつ見ながら正規化（無効文字('_')を飛ばしたり不正な文字でエラーを出したり）し、sb へ格納する。

			CharEnumerator ce = strパラメータ.GetEnumerator();
			while (ce.MoveNext())
			{
				if (ce.Current == '_')    // '_' は無視。
					continue;

				if (C変換.str36進数文字.IndexOf(ce.Current) < 0)  // オブジェクト記述は36進数文字であること。
				{
					Trace.TraceError("不正なオブジェクト指定があります。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数);
					return false;
				}

				sb.Append(ce.Current);
				n文字数++;
			}

			strパラメータ = sb.ToString(); // 正規化された文字列になりました。

			if ((n文字数 % 2) != 0)    // パラメータの文字数が奇数の場合、最後の1文字を無視する。
				n文字数--;
			//-----------------
			#endregion


			// (4) パラメータをオブジェクト数値に分解して配置する。

			for (int i = 0; i < (n文字数 / 2); i++)  // 2文字で1オブジェクト数値
			{
				#region [ nオブジェクト数値 を１つ取得する。'00' なら無視。]
				//-----------------
				int nオブジェクト数値 = 0;

				if (tmpチャンネル番号 == EChannel.BPM)
				{
					// Ch.03 のみ 16進数2桁。
					nオブジェクト数値 = C変換.n16進数2桁の文字列を数値に変換して返す(strパラメータ.Substring(i * 2, 2));
				}
				else
				{
					// その他のチャンネルは36進数2桁。
					nオブジェクト数値 = C変換.n36進数2桁の文字列を数値に変換して返す(strパラメータ.Substring(i * 2, 2));
				}

				if (nオブジェクト数値 == 0x00)
					continue;
				//-----------------
				#endregion

				// オブジェクト数値に対応するチップを生成。
				var chip = new CChip((n小節番号 * 384) + ((384 * i) / (n文字数 / 2)), nオブジェクト数値, nオブジェクト数値, tmpチャンネル番号);

				// 楽器パートの決定
				chip.DecideInstrumentPart();

				// チップがある更新
				this.bチップがある.Drums |= chip.bDrums可視チップ;
				this.bチップがある.HHOpen |= chip[EChannel.HiHatOpen];
				this.bチップがある.Ride |= chip[EChannel.RideCymbal];
				this.bチップがある.LeftCymbal |= chip[EChannel.LeftCymbal];
				this.bチップがある.LeftPedal |= chip[EChannel.LeftPedal];
				this.bチップがある.LeftBassDrum |= chip[EChannel.LeftBassDrum];
				this.bチップがある.Guitar |= chip.bGuitar可視チップ;
				this.bチップがある.OpenGuitar |= chip[EChannel.Guitar_Open];
				this.bチップがある.Bass |= chip.bBass可視チップ;
				this.bチップがある.OpenBass |= chip[EChannel.Bass_Open];
				this.bチップがある.BGA |= chip.bBGALayer;
				this.bチップがある.Movie |= chip.bMovie;

				this.bチップがある.FloorTom |= chip[EChannel.FloorTom];

				if (chip.bMovie)
				{
					if (chip[EChannel.MovieFull] || CDTXMania.Instance.ConfigIni.bForceScalingAVI)
					{
						this.bMovieをFullscreen再生する = true;
					}
				}

				// 空打ちチップを変換する。
				chip.ConvertNoChip();

				// 無限管理オブジェクトインデックスの割当。（もしそのチップが対象であれば）
				chip.AssignInfiniteManageWAV(this.n無限管理WAV[nオブジェクト数値]);
				chip.AssignInfiniteManageBPM(this.n無限管理BPM[nオブジェクト数値]);
				chip.AdjustPlayPositionForFillin(nオブジェクト数値);

				// チップを配置。
				this.listChip.Add(chip);
			}
			return true;
		}
	}
}
