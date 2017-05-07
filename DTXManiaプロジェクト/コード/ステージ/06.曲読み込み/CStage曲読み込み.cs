using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using SharpDX;
using System.Drawing.Text;
using FDK;

using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using SlimDXKey = SlimDX.DirectInput.Key;

namespace DTXMania
{
	internal class CStage曲読み込み : CStage
	{
		private CActFIFOBlack actFO;
		private bool b音符を表示する;
		private Font ftタイトル表示用フォント;
		private long nBGMの総再生時間ms;
		private long nBGM再生開始時刻;
		private int n音符の表示位置X;
		private int n背景の表示位置X;
		private int n背景の表示位置Y;
		private CSound sd読み込み音;
		private string strSTAGEFILE;
		private string str曲タイトル;
		private CTexture txタイトル;
		private CTexture tx音符;
		private CTexture tx背景;
		private DateTime timeBeginLoad;
		private DateTime timeBeginLoadWAV;
		private int nWAVcount;
		private CTexture txFilename;
		private Bitmap bitmapFilename;
		private Graphics graphicsFilename;
		private Font ftFilename;
		private const float fFontSizeFilename = 12.0f;
		private const float fFontSizeTitle = 48;

		public CStage曲読み込み()
		{
			base.eステージID = CStage.Eステージ.曲読み込み;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			//			base.list子Activities.Add( this.actFI = new CActFIFOBlack() );	// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
		}

		public override void On活性化()
		{
			Trace.TraceInformation("曲読み込みステージを活性化します。");
			Trace.Indent();
			try
			{
				this.str曲タイトル = "";
				this.strSTAGEFILE = "";
				this.b音符を表示する = false;
				this.n音符の表示位置X = 0x308;
				this.ftタイトル表示用フォント = new Font("MS PGothic", fFontSizeTitle * Scale.Y, GraphicsUnit.Pixel);
				this.nBGM再生開始時刻 = -1;
				this.nBGMの総再生時間ms = 0;
				if (this.sd読み込み音 != null)
				{
					CDTXMania.Instance.Sound管理.tサウンドを破棄する(this.sd読み込み音);
					this.sd読み込み音 = null;
				}

				string strDTXファイルパス = (CDTXMania.Instance.bコンパクトモード) ?
					CDTXMania.Instance.strコンパクトモードファイル : CDTXMania.Instance.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;

				CDTX cdtx = new CDTX(strDTXファイルパス, true);
				this.str曲タイトル = cdtx.TITLE;
				if (((cdtx.STAGEFILE != null) && (cdtx.STAGEFILE.Length > 0)) && (File.Exists(cdtx.strフォルダ名 + cdtx.STAGEFILE) && !CDTXMania.Instance.ConfigIni.bStoicMode))
				{
					this.strSTAGEFILE = cdtx.strフォルダ名 + cdtx.STAGEFILE;
					this.b音符を表示する = false;
				}
				else
				{
					this.strSTAGEFILE = CSkin.Path(@"Graphics\\ScreenNowLoading background.jpg");
					this.b音符を表示する = true;
				}
				if (((cdtx.SOUND_NOWLOADING != null) && (cdtx.SOUND_NOWLOADING.Length > 0)) && File.Exists(cdtx.strフォルダ名 + cdtx.SOUND_NOWLOADING))
				{
					string strNowLoadingサウンドファイルパス = cdtx.strフォルダ名 + cdtx.SOUND_NOWLOADING;
					try
					{
						this.sd読み込み音 = CDTXMania.Instance.Sound管理.tサウンドを生成する(strNowLoadingサウンドファイルパス);
					}
					catch
					{
						Trace.TraceError("#SOUND_NOWLOADING に指定されたサウンドファイルの読み込みに失敗しました。({0})", strNowLoadingサウンドファイルパス);
					}
				}
				cdtx.On非活性化();
				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation("曲読み込みステージの活性化を完了しました。");
				Trace.Unindent();
			}
		}

		private void ReadGhost(string filename, List<int> list) // #35411 2015.08.19 chnmr0 add
		{
			if (File.Exists(filename))
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader br = new BinaryReader(fs))
					{
						try
						{
							int cnt = br.ReadInt32();
							for (int i = 0; i < cnt; ++i)
							{
								short lag = br.ReadInt16();
								list.Add(lag);
							}
						}
						catch (EndOfStreamException)
						{
							Trace.TraceInformation("ゴーストデータは正しく読み込まれませんでした。");
							list.Clear();
						}
					}
				}
			}
		}

		public override void On非活性化()
		{
			Trace.TraceInformation("曲読み込みステージを非活性化します。");
			Trace.Indent();
			try
			{
				if (this.ftタイトル表示用フォント != null)
				{
					this.ftタイトル表示用フォント.Dispose();
					this.ftタイトル表示用フォント = null;
				}
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation("曲読み込みステージの非活性化を完了しました。");
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx音符 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\\ScreenNowLoading onpu.png"), false);
				this.tx背景 = TextureFactory.tテクスチャの生成(this.strSTAGEFILE, false);

				if (!this.b音符を表示する && this.tx背景 != null)
				{
					if ( this.tx背景.sz画像サイズ.Width <= 640 && this.tx背景.sz画像サイズ.Height <= 480 )	// VGAサイズ以下の背景の場合は、単純拡大しFullHD化
					{
						this.tx背景.vc拡大縮小倍率 = new Vector3( Scale.X, Scale.Y, 1f ); // とりあえずFullHD化
						this.n背景の表示位置X = this.n背景の表示位置Y = 0;
					}
					else // #36373 VGAより大きな画像をSTAGEFILEに指定した場合は、アスペクト比を維持しつつ全画面に収まるように拡大縮小し表示
					{
						CPreviewMagnifier cmg = new CPreviewMagnifier( CPreviewMagnifier.EPreviewType.PlayingBackground );
						cmg.GetMagnifier(this.tx背景.sz画像サイズ.Width, this.tx背景.sz画像サイズ.Height, 1.0f, 1.0f);
						this.tx背景.vc拡大縮小倍率 = new Vector3( cmg.magX, cmg.magY, 1f );
						this.n背景の表示位置X = cmg.px;
						this.n背景の表示位置Y = cmg.py;
					}
				}
				if (this.b音符を表示する)
				{
					try
					{
						if ((this.str曲タイトル != null) && (this.str曲タイトル.Length > 0))
						{
							Bitmap image = new Bitmap(1, 1);
							Graphics graphics = Graphics.FromImage(image);
							SizeF ef = graphics.MeasureString(this.str曲タイトル, this.ftタイトル表示用フォント);
							Size size = new Size((int)Math.Ceiling((double)ef.Width), (int)Math.Ceiling((double)ef.Height));
							graphics.Dispose();
							image.Dispose();
							image = new Bitmap(size.Width, size.Height);
							graphics = Graphics.FromImage(image);
							graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
							graphics.DrawString(this.str曲タイトル, this.ftタイトル表示用フォント, Brushes.White, (float)0f, (float)0f);
							graphics.Dispose();
							this.txタイトル = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);
							this.txタイトル.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
							image.Dispose();
							this.n音符の表示位置X = ((SampleFramework.GameWindowSize.Width - ((int)(size.Width * this.txタイトル.vc拡大縮小倍率.X))) - ((this.tx音符 != null) ? this.tx音符.sz画像サイズ.Width : 0)) - 2;
						}
						else
						{
							this.txタイトル = null;
							this.n音符の表示位置X = (SampleFramework.GameWindowSize.Width - ((this.tx音符 != null) ? this.tx音符.sz画像サイズ.Width : 0)) - 2;
						}
					}
					catch (CTextureCreateFailedException)
					{
						Trace.TraceError("テクスチャの生成に失敗しました。({0})", new object[] { this.strSTAGEFILE });
						this.txタイトル = null;
						this.tx背景 = null;
					}
				}
				else
				{
					this.txタイトル = null;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.tx音符);
				TextureFactory.tテクスチャの解放(ref this.txタイトル);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			string str;

			if (base.b活性化してない)
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------------------
			if (base.b初めての進行描画)
			{
				Cスコア cスコア1 = CDTXMania.Instance.stage選曲.r確定されたスコア;
				if (this.sd読み込み音 != null)
				{
					if (CDTXMania.Instance.Skin.sound曲読込開始音.b排他 && (CSkin.Cシステムサウンド.r最後に再生した排他システムサウンド != null))
					{
						CSkin.Cシステムサウンド.r最後に再生した排他システムサウンド.t停止する();
					}
					this.sd読み込み音.t再生を開始する();
					this.nBGM再生開始時刻 = CSound管理.rc演奏用タイマ.n現在時刻;
					this.nBGMの総再生時間ms = this.sd読み込み音.n総演奏時間ms;
				}
				else
				{
					CDTXMania.Instance.Skin.sound曲読込開始音.t再生する();
					this.nBGM再生開始時刻 = CSound管理.rc演奏用タイマ.n現在時刻;
					this.nBGMの総再生時間ms = CDTXMania.Instance.Skin.sound曲読込開始音.n長さ_現在のサウンド;
				}
				//				this.actFI.tフェードイン開始();							// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
				base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
				base.b初めての進行描画 = false;

				nWAVcount = 1;
				bitmapFilename = new Bitmap(SampleFramework.GameWindowSize.Width, (int)(fFontSizeFilename * Scale.X));
				graphicsFilename = Graphics.FromImage(bitmapFilename);
				graphicsFilename.TextRenderingHint = TextRenderingHint.AntiAlias;
				ftFilename = new Font("MS PGothic", fFontSizeFilename * Scale.X, FontStyle.Bold, GraphicsUnit.Pixel);
			}
			//-----------------------------
			#endregion

			#region [ ESC押下時は選曲画面に戻る ]
			if (tキー入力())
			{
				if (this.sd読み込み音 != null)
				{
					this.sd読み込み音.tサウンドを停止する();
					this.sd読み込み音.t解放する();
				}
				return (int)E曲読込画面の戻り値.読込中止;
			}
			#endregion

			#region [ 背景、音符＋タイトル表示 ]
			//-----------------------------
			if (this.tx背景 != null)
				this.tx背景.t2D描画(CDTXMania.Instance.Device, this.n背景の表示位置X, this.n背景の表示位置Y);

			if (this.b音符を表示する)
			{
				int y = SampleFramework.GameWindowSize.Height - (int)(fFontSizeTitle * Scale.Y) + (int)(3 * Scale.X);   // 480 - 45;
				if (this.tx音符 != null)
				{
					this.tx音符.t2D描画(
						CDTXMania.Instance.Device,
						this.n音符の表示位置X,
						y
					);
				}
				if (this.txタイトル != null)
				{
					this.txタイトル.t2D描画(
						CDTXMania.Instance.Device,
						(int)(SampleFramework.GameWindowSize.Width - (this.txタイトル.sz画像サイズ.Width * this.txタイトル.vc拡大縮小倍率.X)),
						y
					);
				}
			}
			//-----------------------------
			#endregion

			switch (base.eフェーズID)
			{
				case CStage.Eフェーズ.共通_フェードイン:
					//					if( this.actFI.On進行描画() != 0 )					// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
					// 必ず一度「CStaeg.Eフェーズ.共通_フェードイン」フェーズを経由させること。
					// さもないと、曲読み込みが完了するまで、曲読み込み画面が描画されない。
					base.eフェーズID = CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む;
					return (int)E曲読込画面の戻り値.継続;

				case CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む:
					{
						timeBeginLoad = DateTime.Now;
						TimeSpan span;
						str = null;
						if (!CDTXMania.Instance.bコンパクトモード)
							str = CDTXMania.Instance.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;
						else
							str = CDTXMania.Instance.strコンパクトモードファイル;

						CScoreIni ini = new CScoreIni(str + ".score.ini");

						if ((CDTXMania.Instance.DTX != null) && CDTXMania.Instance.DTX.b活性化してる)
							CDTXMania.Instance.DTX.On非活性化();

						CDTXMania.Instance.DTX = new CDTX(str, false, ((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0, ini.stファイル.BGMAdjust);
						Trace.TraceInformation("----曲情報-----------------");
						Trace.TraceInformation("TITLE: {0}", CDTXMania.Instance.DTX.TITLE);
						Trace.TraceInformation("FILE: {0}", CDTXMania.Instance.DTX.strファイル名の絶対パス);
						Trace.TraceInformation("---------------------------");

						// #35411 2015.08.19 chnmr0 add
						// Read ghost data by config
						// It does not exist a ghost file for 'perfect' actually
						STDGBSValue<string> inst = new STDGBSValue<string>();
						inst.Drums = "dr";
						inst.Guitar = "gt";
						inst.Bass = "bs";
						if (CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass)
						{
							inst.Guitar = "bs";
							inst.Bass = "gt";
						}

						for (EPart einst = EPart.Drums; einst <= EPart.Bass; ++einst)
						{
							bool readAutoGhostCond = CDTXMania.Instance.ConfigIni.bIsAutoPlay(einst);

							CDTXMania.Instance.DTX.listTargetGhsotLag[einst] = null;
							CDTXMania.Instance.DTX.listAutoGhostLag[einst] = null;

							if (readAutoGhostCond)
							{
								string[] prefix = { "perfect", "lastplay", "hiskill", "hiscore", "online" };
								int indPrefix = (int)CDTXMania.Instance.ConfigIni.eAutoGhost[einst].Value;
								string filename = CDTXMania.Instance.DTX.strフォルダ名 + "\\" + CDTXMania.Instance.DTX.strファイル名 + "." + prefix[indPrefix] + "." + inst[einst] + ".ghost";
								if (File.Exists(filename))
								{
									CDTXMania.Instance.DTX.listAutoGhostLag[einst] = new List<int>();
									ReadGhost(filename, CDTXMania.Instance.DTX.listAutoGhostLag[einst]);
								}
							}

							if (CDTXMania.Instance.ConfigIni.eTargetGhost[einst].Value != ETargetGhostData.None)
							{
								string[] prefix = { "none", "perfect", "lastplay", "hiskill", "hiscore", "online" };
								int indPrefix = (int)CDTXMania.Instance.ConfigIni.eTargetGhost[einst].Value;
								string filename = CDTXMania.Instance.DTX.strフォルダ名 + "\\" + CDTXMania.Instance.DTX.strファイル名 + "." + prefix[indPrefix] + "." + inst[einst] + ".ghost";
								if (File.Exists(filename))
								{
									CDTXMania.Instance.DTX.listTargetGhsotLag[einst] = new List<int>();
									ReadGhost(filename, CDTXMania.Instance.DTX.listTargetGhsotLag[einst]);
								}
								else if (CDTXMania.Instance.ConfigIni.eTargetGhost[einst] == ETargetGhostData.Perfect)
								{
									// All perfect
									CDTXMania.Instance.DTX.listTargetGhsotLag[einst] = new List<int>();
								}
							}
						}

						// #35411 2015.08.19 chnmr0 add ゴースト機能のためList chip 読み込み後楽器パート出現順インデックスを割り振る
						STDGBSValue<int> curCount = new STDGBSValue<int>();
						foreach (CChip chip in CDTXMania.Instance.DTX.listChip)
						{
							if (chip.e楽器パート != EPart.Unknown)
							{
								chip.n楽器パートでの出現順 = curCount[chip.e楽器パート]++;
							}
						}

						span = (TimeSpan)(DateTime.Now - timeBeginLoad);
						Trace.TraceInformation("DTX読込所要時間:           {0}", span.ToString());

						if (CDTXMania.Instance.bコンパクトモード)
							CDTXMania.Instance.DTX.MIDIレベル = 1;
						else
							CDTXMania.Instance.DTX.MIDIレベル = (CDTXMania.Instance.stage選曲.r確定された曲.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI) ? CDTXMania.Instance.stage選曲.n現在選択中の曲の難易度 : 0;

						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む;
						timeBeginLoadWAV = DateTime.Now;
						return (int)E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む:
					{
						if ( CDTXMania.Instance.DTX.listWAV.Count > 0 )     // #28934 2012.7.7 yyagi (added checking Count)
						{
							//ShowProgressByFilename(CDTXMania.Instance.DTX.listWAV[nWAVcount].strファイル名); // #36046 進捗表示を追加
							
							double f進捗率 = nWAVcount * 100.0f / CDTXMania.Instance.DTX.listWAV.Count;
							ShowProgressByFilename("" + f進捗率.ToString("0.0") + "% " + nWAVcount + "/" + CDTXMania.Instance.DTX.listWAV.Count + " " + CDTXMania.Instance.DTX.listWAV[nWAVcount].strファイル名);
						}
						int looptime = (CDTXMania.Instance.ConfigIni.bVSyncWait) ? 3 : 1;
						if (CDTXMania.Instance.ConfigIni.bLoadSoundSpeed) looptime = CDTXMania.Instance.DTX.listWAV.Count / 20 + 1; // #36046 ikanick 2016.2.21 ロード高速化 垂直同期の有無に関わらず、1フレームに5%ぐらいずつロードする
						for (int i = 0; i < looptime && nWAVcount <= CDTXMania.Instance.DTX.listWAV.Count; i++)
						{
							if (CDTXMania.Instance.DTX.listWAV[nWAVcount].listこのWAVを使用するチャンネル番号の集合.Count > 0) // #28674 2012.5.8 yyagi
							{
								CDTXMania.Instance.DTX.tWAVの読み込み(CDTXMania.Instance.DTX.listWAV[nWAVcount]);
							}
							nWAVcount++;
						}
						if (nWAVcount > CDTXMania.Instance.DTX.listWAV.Count)
						{
							TimeSpan span = (TimeSpan)(DateTime.Now - timeBeginLoadWAV);
							Trace.TraceInformation("WAV読込所要時間({0,4}):     {1}", CDTXMania.Instance.DTX.listWAV.Count, span.ToString());
							timeBeginLoadWAV = DateTime.Now;

							if (CDTXMania.Instance.ConfigIni.bDynamicBassMixerManagement)
							{
								CDTXMania.Instance.DTX.PlanToAddMixerChannel();
							}
							CDTXMania.Instance.DTX.tギターとベースのランダム化(EPart.Guitar, CDTXMania.Instance.ConfigIni.eRandom.Guitar);
							CDTXMania.Instance.DTX.tギターとベースのランダム化(EPart.Bass, CDTXMania.Instance.ConfigIni.eRandom.Bass);

							CDTXMania.Instance.stage演奏画面.On活性化();

							span = (TimeSpan)(DateTime.Now - timeBeginLoadWAV);
							Trace.TraceInformation("WAV/譜面後処理時間({0,4}):  {1}", (CDTXMania.Instance.DTX.listBMP.Count + CDTXMania.Instance.DTX.listBMPTEX.Count + CDTXMania.Instance.DTX.listAVI.Count), span.ToString());

							base.eフェーズID = CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む;
						}
						return (int)E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む:
					{
						TimeSpan span;
						DateTime timeBeginLoadBMPAVI = DateTime.Now;
						if (CDTXMania.Instance.ConfigIni.bBGA)
							CDTXMania.Instance.DTX.tBMP_BMPTEXの読み込み();

						if (CDTXMania.Instance.ConfigIni.bAVI)
							CDTXMania.Instance.DTX.tAVIの読み込み();
						span = (TimeSpan)(DateTime.Now - timeBeginLoadBMPAVI);
						Trace.TraceInformation("BMP/AVI読込所要時間({0,4}): {1}", (CDTXMania.Instance.DTX.listBMP.Count + CDTXMania.Instance.DTX.listBMPTEX.Count + CDTXMania.Instance.DTX.listAVI.Count), span.ToString());


						if (bitmapFilename != null)
						{
							bitmapFilename.Dispose();
							bitmapFilename = null;
						}
						if (graphicsFilename != null)
						{
							graphicsFilename.Dispose();
							graphicsFilename = null;
						}
						if (ftFilename != null)
						{
							ftFilename.Dispose();
							ftFilename = null;
						}
						CDTXMania.Instance.Timer.t更新();
						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_LPを再配置する;
						return (int)E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_LPを再配置する:
					{
						TimeSpan span;
						DateTime timeReassignLP = DateTime.Now;
						CDTXMania.Instance.DTX.ReassignLP();
		
						span = (TimeSpan) ( DateTime.Now - timeReassignLP );
						Trace.TraceInformation( "LP再配置所要時間:          {0}", span.ToString() );

						span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						Trace.TraceInformation( "総読込時間:                {0}", span.ToString() );

						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ;
						return (int) E曲読込画面の戻り値.継続;
					}
				case CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ:
					{
						long nCurrentTime = CDTXMania.Instance.Timer.n現在時刻;
						if (nCurrentTime < this.nBGM再生開始時刻)
							this.nBGM再生開始時刻 = nCurrentTime;

						//						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) > ( this.nBGMの総再生時間ms - 1000 ) )
						if ((nCurrentTime - this.nBGM再生開始時刻) >= (this.nBGMの総再生時間ms))  // #27787 2012.3.10 yyagi 1000ms == フェードイン分の時間
						{
							if (!CDTXMania.Instance.DTXVmode.Enabled)
							{
								this.actFO.tフェードアウト開始();
							}
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						}
						return (int)E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.共通_フェードアウト:
					if (this.actFO.On進行描画() == 0 && !CDTXMania.Instance.DTXVmode.Enabled)   // DTXVモード時は、フェードアウト省略
						return 0;

					if (txFilename != null)
					{
						txFilename.Dispose();
					}
					if (this.sd読み込み音 != null)
					{
						this.sd読み込み音.t解放する();
					}
					return (int)E曲読込画面の戻り値.読込完了;
			}
			return (int)E曲読込画面の戻り値.継続;
		}

		/// <summary>
		/// ESC押下時、trueを返す
		/// </summary>
		/// <returns></returns>
		protected bool tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Instance.Input管理.Keyboard;
			if (keyboard.bキーが押された((int)SlimDXKey.Escape))    // escape (exit)
			{
				return true;
			}
			return false;
		}


		private void ShowProgressByFilename(string strファイル名と進捗)
		{
			if (graphicsFilename != null && ftFilename != null)
			{
				graphicsFilename.Clear(Color.Transparent);
				graphicsFilename.DrawString(strファイル名と進捗, ftFilename, Brushes.White, new RectangleF(0, 0, SampleFramework.GameWindowSize.Width, fFontSizeFilename * Scale.X));
				if (txFilename != null)
				{
					txFilename.Dispose();
				}
				txFilename = new CTexture(CDTXMania.Instance.Device, bitmapFilename, CDTXMania.Instance.TextureFormat);
				txFilename.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f);
				txFilename.t2D描画(
					CDTXMania.Instance.Device,
					0,
					(SampleFramework.GameWindowSize.Height - (int)(txFilename.szテクスチャサイズ.Height * 0.5))
				);
			}
		}

	}
}
