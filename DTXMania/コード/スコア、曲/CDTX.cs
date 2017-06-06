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
	/// <summary>
	/// CDTX 内で用いる入れ子型を partial にし、ここで定義します。
	/// </summary>
	public partial class CDTX : CActivity
	{
		// プロパティ
		public int nBGMAdjust
		{
			get;
			private set;
		}
		public string ARTIST;
		public string BACKGROUND;
		public string BACKGROUND_GR;
		public double BASEBPM;
		public bool BLACKCOLORKEY;
		public double BPM;
		public STチップがある bチップがある;
		public string COMMENT;
		public double db再生速度;
		public EDTX種別 e種別;
		public string GENRE;
		public bool HIDDENLEVEL;
		public STDGBSValue<int> LEVEL;
		public Dictionary<int, CAVI> listAVI;
		public Dictionary<int, CAVIPAN> listAVIPAN;
		public Dictionary<int, CBGA> listBGA;
		public Dictionary<int, CBGAPAN> listBGAPAN;
		public Dictionary<int, CBMP> listBMP;
		public Dictionary<int, CBMPTEX> listBMPTEX;
		public Dictionary<int, CBPM> listBPM;
		public List<CChip> listChip;
		public Dictionary<int, CWAV> listWAV;
		public string MIDIFILE;
		public bool MIDINOTE;
		public int MIDIレベル;
		public STDGBSValue<int> n可視チップ数;
		public const int n最大音数 = 4;
		public const int n小節の解像度 = 384;
		public string PANEL;
		public string PATH_WAV;
		public string PREIMAGE;
		public string PREMOVIE;
		public string PREVIEW;
		public STRESULT RESULTIMAGE;
		public STRESULT RESULTMOVIE;
		public STRESULT RESULTSOUND;
		public string SOUND_AUDIENCE;
		public string SOUND_FULLCOMBO;
		public string SOUND_NOWLOADING;
		public string SOUND_STAGEFAILED;
		public string STAGEFILE;
		public string strハッシュofDTXファイル;
		public string strファイル名;
		public string strファイル名の絶対パス;
		public string strフォルダ名;
		public string TITLE;
		public double dbDTXVPlaySpeed;
		public bool bMovieをFullscreen再生する;
		public bool bUse556x710BGAAVI;
		public STDGBSValue<List<int>> listAutoGhostLag;
		public STDGBSValue<List<int>> listTargetGhsotLag;
		public STDGBSValue<EUseLanes> n使用レーン数;

#if TEST_NOTEOFFMODE
		public STLANEVALUE<bool> b演奏で直前の音を消音する;
//		public bool bHH演奏で直前のHHを消音する;
//		public bool bGUITAR演奏で直前のGUITARを消音する;
//		public bool bBASS演奏で直前のBASSを消音する;
#endif
		#region [ コンストラクタ ]

		private CDTX()
		{
			this.TITLE = "";
			this.ARTIST = "";
			this.COMMENT = "";
			this.PANEL = "";
			this.GENRE = "";
			this.PREVIEW = "";
			this.PREIMAGE = "";
			this.PREMOVIE = "";
			this.STAGEFILE = "";
			this.BACKGROUND = "";
			this.BACKGROUND_GR = "";
			this.PATH_WAV = "";
			this.MIDIFILE = "";
			this.SOUND_STAGEFAILED = "";
			this.SOUND_FULLCOMBO = "";
			this.SOUND_NOWLOADING = "";
			this.SOUND_AUDIENCE = "";
			this.BPM = 120.0;
			this.BLACKCOLORKEY = true;
			STDGBSValue<int> stdgbvalue = new STDGBSValue<int>();
			stdgbvalue.Drums = 0;
			stdgbvalue.Guitar = 0;
			stdgbvalue.Bass = 0;
			this.LEVEL = stdgbvalue;
			for (int i = 0; i < 7; i++)
			{
				this.RESULTIMAGE[i] = "";
				this.RESULTMOVIE[i] = "";
				this.RESULTSOUND[i] = "";
			}
			this.db再生速度 = 1.0;
			this.strハッシュofDTXファイル = "";
			this.bチップがある = new STチップがある();
			this.bチップがある.Drums = false;
			this.bチップがある.Guitar = false;
			this.bチップがある.Bass = false;
			this.bチップがある.HHOpen = false;
			this.bチップがある.Ride = false;
			this.bチップがある.LeftCymbal = false;
			this.bチップがある.OpenGuitar = false;
			this.bチップがある.OpenBass = false;
			this.bチップがある.BGA = false;
			this.bチップがある.Movie = false;
			this.bチップがある.LeftPedal = false;
			this.bチップがある.LeftBassDrum = false;
			this.bMovieをFullscreen再生する = false;
			this.strファイル名 = "";
			this.strフォルダ名 = "";
			this.strファイル名の絶対パス = "";
			this.n無限管理WAV = new int[36 * 36];
			this.n無限管理BPM = new int[36 * 36];
			this.n無限管理VOL = new int[36 * 36];
			this.n無限管理PAN = new int[36 * 36];
			this.n無限管理SIZE = new int[36 * 36];
			this.nRESULTIMAGE用優先順位 = new int[7];
			this.nRESULTMOVIE用優先順位 = new int[7];
			this.nRESULTSOUND用優先順位 = new int[7];
			this.listAutoGhostLag = new STDGBSValue<List<int>>();
			this.listTargetGhsotLag = new STDGBSValue<List<int>>();

			#region [ 2011.1.1 yyagi GDA->DTX変換テーブル リファクタ後 ]
			STGDAPARAM[] stgdaparamArray = new STGDAPARAM[] {		// GDA->DTX conversion table
				new STGDAPARAM("TC", EChannel.BPM),
				new STGDAPARAM("BL", EChannel.BarLength),
				new STGDAPARAM("GS", EChannel.flowspeed_gt_nouse),
				new STGDAPARAM("DS", EChannel.flowspeed_dr_nouse),
				new STGDAPARAM("FI", EChannel.FillIn),
				new STGDAPARAM("HH", EChannel.HiHatClose),
				new STGDAPARAM("SD", EChannel.Snare),
				new STGDAPARAM("BD", EChannel.BassDrum),
				new STGDAPARAM("HT", EChannel.HighTom),
				new STGDAPARAM("LT", EChannel.LowTom),
				new STGDAPARAM("CY", EChannel.Cymbal),
				new STGDAPARAM("G1", EChannel.Guitar_xxB),
				new STGDAPARAM("G2", EChannel.Guitar_xGx),
				new STGDAPARAM("G3", EChannel.Guitar_xGB),
				new STGDAPARAM("G4", EChannel.Guitar_Rxx),
				new STGDAPARAM("G5", EChannel.Guitar_RxB),
				new STGDAPARAM("G6", EChannel.Guitar_RGx),
				new STGDAPARAM("G7", EChannel.Guitar_RGB),
				new STGDAPARAM("GW", EChannel.Guitar_Wailing),
				new STGDAPARAM("01", EChannel.SE01),
				new STGDAPARAM("02", EChannel.SE02),
				new STGDAPARAM("03", EChannel.SE03),
				new STGDAPARAM("04", EChannel.SE04),
				new STGDAPARAM("05", EChannel.SE05),
				new STGDAPARAM("06", EChannel.SE06),
				new STGDAPARAM("07", EChannel.SE07),
				new STGDAPARAM("08", EChannel.SE08),
				new STGDAPARAM("09", EChannel.SE09),
				new STGDAPARAM("0A", EChannel.SE10),
				new STGDAPARAM("0B", EChannel.SE11),
				new STGDAPARAM("0C", EChannel.SE12),
				new STGDAPARAM("0D", EChannel.SE13),
				new STGDAPARAM("0E", EChannel.SE14),
				new STGDAPARAM("0F", EChannel.SE15),
				new STGDAPARAM("10", EChannel.SE16),
				new STGDAPARAM("11", EChannel.SE17),
				new STGDAPARAM("12", EChannel.SE18),
				new STGDAPARAM("13", EChannel.SE19),
				new STGDAPARAM("14", EChannel.SE20),
				new STGDAPARAM("15", EChannel.SE21),
				new STGDAPARAM("16", EChannel.SE22),
				new STGDAPARAM("17", EChannel.SE23),
				new STGDAPARAM("18", EChannel.SE24),
				new STGDAPARAM("19", EChannel.SE25),
				new STGDAPARAM("1A", EChannel.SE26),
				new STGDAPARAM("1B", EChannel.SE27),
				new STGDAPARAM("1C", EChannel.SE28),
				new STGDAPARAM("1D", EChannel.SE29),
				new STGDAPARAM("1E", EChannel.SE30),
				new STGDAPARAM("1F", EChannel.SE31),
				new STGDAPARAM("20", EChannel.SE32),
				new STGDAPARAM("B1", EChannel.Bass_xxB),
				new STGDAPARAM("B2", EChannel.Bass_xGx),
				new STGDAPARAM("B3", EChannel.Bass_xGB),
				new STGDAPARAM("B4", EChannel.Bass_Rxx),
				new STGDAPARAM("B5", EChannel.Bass_RxB),
				new STGDAPARAM("B6", EChannel.Bass_RGx),
				new STGDAPARAM("B7", EChannel.Bass_RGB),
				new STGDAPARAM("BW", EChannel.Bass_Wailing),
				new STGDAPARAM("G0", EChannel.Guitar_Open),
				new STGDAPARAM("B0", EChannel.Bass_Open)
			};
			this.stGDAParam = stgdaparamArray;
			#endregion
			this.nBGMAdjust = 0;
			this.nPolyphonicSounds = CDTXMania.Instance.ConfigIni.nPolyphonicSounds;
			this.dbDTXVPlaySpeed = 1.0f;
			this.bUse556x710BGAAVI = false;
			this.n使用レーン数 = new STDGBSValue<EUseLanes>();

#if TEST_NOTEOFFMODE
			this.bHH演奏で直前のHHを消音する = true;
			this.bGUITAR演奏で直前のGUITARを消音する = true;
			this.bBASS演奏で直前のBASSを消音する = true;
#endif

		}
		private CDTX(string str全入力文字列)
			: this()
		{
			this.On活性化();
			this.t入力_全入力文字列から(str全入力文字列);
		}
		public CDTX(string strファイル名, bool bヘッダのみ)
			: this()
		{
			this.On活性化();
			this.t入力(strファイル名, bヘッダのみ);
		}
		private CDTX(string str全入力文字列, double db再生速度, int nBGMAdjust)
			: this()
		{
			this.On活性化();
			this.t入力_全入力文字列から(str全入力文字列, db再生速度, nBGMAdjust);
		}
		public CDTX(string strファイル名, bool bヘッダのみ, double db再生速度, int nBGMAdjust)
			: this()
		{
			this.On活性化();
			this.t入力(strファイル名, bヘッダのみ, db再生速度, nBGMAdjust);
		}
		#endregion


		// メソッド

		public int nモニタを考慮した音量(EPart part)
		{
			switch (part)
			{
				case EPart.Drums:
					if (CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums)
					{
						return CDTXMania.Instance.ConfigIni.nAutoVolume;
					}
					return CDTXMania.Instance.ConfigIni.nChipVolume;

				case EPart.Guitar:
					if (CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Guitar)
					{
						return CDTXMania.Instance.ConfigIni.nAutoVolume;
					}
					return CDTXMania.Instance.ConfigIni.nChipVolume;

				case EPart.Bass:
					if (CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Bass)
					{
						return CDTXMania.Instance.ConfigIni.nAutoVolume;
					}
					return CDTXMania.Instance.ConfigIni.nChipVolume;
			}
			if ((!CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums && !CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Guitar) && !CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Bass)
			{
				return CDTXMania.Instance.ConfigIni.nChipVolume;
			}
			return CDTXMania.Instance.ConfigIni.nAutoVolume;
		}
		public void tAVIの読み込み()
		{
			if (this.listAVI != null)
			{
				foreach (CAVI cavi in this.listAVI.Values)
				{
					cavi.OnDeviceCreated();
				}
			}
			if (!this.bヘッダのみ)
			{
				foreach (CChip chip in this.listChip)
				{
					chip.ApplyAVI(listAVI, listAVIPAN);
				}
			}
		}
		#region [ BMP/BMPTEXの並列読み込み・デコード用メソッド ]
		delegate void BackgroundBMPLoadAll(Dictionary<int, CBMP> listB);
		static BackgroundBMPLoadAll backgroundBMPLoadAll = new BackgroundBMPLoadAll(BMPLoadAll);
		delegate void BackgroundBMPTEXLoadAll(Dictionary<int, CBMPTEX> listB);
		static BackgroundBMPTEXLoadAll backgroundBMPTEXLoadAll = new BackgroundBMPTEXLoadAll(BMPTEXLoadAll);
		private static void LoadTexture(CBMPbase cbmp)            // バックグラウンドスレッドで動作する、ファイル読み込み部
		{
			string filename = cbmp.GetFullPathname;
			if (!File.Exists(filename))
			{
				Trace.TraceWarning("ファイルが存在しません。({0})", filename);
				cbmp.bitmap = null;
				return;
			}
			try
			{
				cbmp.bitmap = new Bitmap(filename);
			}
			catch (ArgumentException)
			{
				Trace.TraceWarning("引数が不正です。ファイルが破損している可能性があります。({0})", filename);
				cbmp.bitmap = null;
				return;
			}
		}
		private static void BMPLoadAll(Dictionary<int, CBMP> listB) // バックグラウンドスレッドで、テクスチャファイルをひたすら読み込んではキューに追加する
		{
			//Trace.TraceInformation( "Back: ThreadID(BMPLoad)=" + Thread.CurrentThread.ManagedThreadId + ", listCount=" + listB.Count  );
			foreach (CBMPbase cbmp in listB.Values)
			{
				LoadTexture(cbmp);
				lock (lockQueue)
				{
					queueCBMPbaseDone.Enqueue(cbmp);
					//  Trace.TraceInformation( "Back: Enqueued(" + queueCBMPbaseDone.Count + "): " + cbmp.strファイル名 );
				}
				if (queueCBMPbaseDone.Count > 8)
				{
					Thread.Sleep(10);
				}
			}
		}
		private static void BMPTEXLoadAll(Dictionary<int, CBMPTEX> listB) // ダサい実装だが、Dictionary<>の中には手を出せず、妥協した
		{
			//Trace.TraceInformation( "Back: ThreadID(BMPLoad)=" + Thread.CurrentThread.ManagedThreadId + ", listCount=" + listB.Count  );
			foreach (CBMPbase cbmp in listB.Values)
			{
				LoadTexture(cbmp);
				lock (lockQueue)
				{
					queueCBMPbaseDone.Enqueue(cbmp);
					//  Trace.TraceInformation( "Back: Enqueued(" + queueCBMPbaseDone.Count + "): " + cbmp.strファイル名 );
				}
				if (queueCBMPbaseDone.Count > 8)
				{
					Thread.Sleep(10);
				}
			}
		}

		private static Queue<CBMPbase> queueCBMPbaseDone = new Queue<CBMPbase>();
		private static object lockQueue = new object();
		private static int nLoadDone;
		#endregion

		public void tBMP_BMPTEXの読み込み()
		{
			#region [ CPUコア数の取得 ]
			CWin32.SYSTEM_INFO sysInfo = new CWin32.SYSTEM_INFO();
			CWin32.GetSystemInfo(ref sysInfo);
			int nCPUCores = (int)sysInfo.dwNumberOfProcessors;
			#endregion
			#region [ BMP読み込み ]
			if (this.listBMP != null)
			{
				if (nCPUCores <= 1)
				{
					#region [ シングルスレッドで逐次読み出し・デコード・テクスチャ定義 ]
					foreach (CBMP cbmp in this.listBMP.Values)
					{
						cbmp.OnDeviceCreated();
					}
					#endregion
				}
				else
				{
					#region [ メインスレッド(テクスチャ定義)とバックグラウンドスレッド(読み出し・デコード)を並列動作させ高速化 ]
					//Trace.TraceInformation( "Main: ThreadID(Main)=" + Thread.CurrentThread.ManagedThreadId + ", listCount=" + this.listBMP.Count );
					nLoadDone = 0;
					backgroundBMPLoadAll.BeginInvoke(listBMP, null, null);

					// t.Priority = ThreadPriority.Lowest;
					// t.Start( listBMP );
					int c = listBMP.Count;
					while (nLoadDone < c)
					{
						if (queueCBMPbaseDone.Count > 0)
						{
							CBMP cbmp;
							//Trace.TraceInformation( "Main: Lock Begin for dequeue1." );
							try
							{
								lock (lockQueue)
								{
									cbmp = (CBMP)queueCBMPbaseDone.Dequeue();
									//  Trace.TraceInformation( "Main: Dequeued(" + queueCBMPbaseDone.Count + "): " + cbmp.strファイル名 );
								}
								cbmp.OnDeviceCreated(cbmp.bitmap, cbmp.GetFullPathname);
							}
							catch (InvalidCastException)  // bmp読み込み失敗時は、キャストに失敗する
							{
							}
							finally
							{
								nLoadDone++;
							}
							//Trace.TraceInformation( "Main: OnDeviceCreated: " + cbmp.strファイル名 );
						}
						else
						{
							//Trace.TraceInformation( "Main: Sleeped.");
							Thread.Sleep(5);  // WaitOneのイベント待ちにすると、メインスレッド処理中に2個以上イベント完了したときにそれを正しく検出できなくなるので、
						}           // ポーリングに逃げてしまいました。
					}
					#endregion
				}
			}
			#endregion
			#region [ BMPTEX読み込み ]
			if (this.listBMPTEX != null)
			{
				if (nCPUCores <= 1)
				{
					#region [ シングルスレッドで逐次読み出し・デコード・テクスチャ定義 ]
					foreach (CBMPTEX cbmptex in this.listBMPTEX.Values)
					{
						cbmptex.OnDeviceCreated();
					}
					#endregion
				}
				else
				{
					#region [ メインスレッド(テクスチャ定義)とバックグラウンドスレッド(読み出し・デコード)を並列動作させ高速化 ]
					//Trace.TraceInformation( "Main: ThreadID(Main)=" + Thread.CurrentThread.ManagedThreadId + ", listCount=" + this.listBMP.Count );
					nLoadDone = 0;
					backgroundBMPTEXLoadAll.BeginInvoke(listBMPTEX, null, null);
					int c = listBMPTEX.Count;
					while (nLoadDone < c)
					{
						if (queueCBMPbaseDone.Count > 0)
						{
							CBMPTEX cbmptex;
							//Trace.TraceInformation( "Main: Lock Begin for dequeue1." );
							try
							{
								lock (lockQueue)
								{
									cbmptex = (CBMPTEX)queueCBMPbaseDone.Dequeue();
									//  Trace.TraceInformation( "Main: Dequeued(" + queueCBMPbaseDone.Count + "): " + cbmp.strファイル名 );
								}
								cbmptex.OnDeviceCreated(cbmptex.bitmap, cbmptex.GetFullPathname);
							}
							catch (InvalidCastException)
							{
							}
							finally
							{
								nLoadDone++;
							}
							//Trace.TraceInformation( "Main: OnDeviceCreated: " + cbmp.strファイル名 );
						}
						else
						{
							//Trace.TraceInformation( "Main: Sleeped.");
							Thread.Sleep(5);  // WaitOneのイベント待ちにすると、メインスレッド処理中に2個以上イベント完了したときにそれを正しく検出できなくなるので、
						}           // ポーリングに逃げてしまいました。
					}
					#endregion
				}
			}
			#endregion
			if (!this.bヘッダのみ)
			{
				foreach (CChip chip in this.listChip)
				{
					chip.ApplyBMP_BMPTEX(listBGA, listBGAPAN, listBMP, listBMPTEX);
				}
			}
		}
		public void tWave再生位置自動補正()
		{
			foreach (CWAV cwav in this.listWAV.Values)
			{
				this.tWave再生位置自動補正(cwav);
			}
		}
		public void tWave再生位置自動補正(CWAV wc)
		{
			if (wc.rSound[0] != null && wc.rSound[0].n総演奏時間ms >= 5000)
			{
				for (int i = 0; i < nPolyphonicSounds; i++)
				{
					if ((wc.rSound[i] != null) && (wc.rSound[i].b再生中))
					{
						long nCurrentTime = CSound管理.rc演奏用タイマ.nシステム時刻ms;
						if (nCurrentTime > wc.n再生開始時刻[i])
						{
							long nAbsTimeFromStartPlaying = nCurrentTime - wc.n再生開始時刻[i];
							//Trace.TraceInformation( "再生位置自動補正: {0}, 実タイマ値={1}, seek先={2}ms, 全音長={3}ms",
							//	Path.GetFileName( wc.rSound[ 0 ].strファイル名 ),
							//	nCurrentTime,
							//	nAbsTimeFromStartPlaying,
							//	wc.rSound[ 0 ].n総演奏時間ms
							//);
							// wc.rSound[ i ].t再生位置を変更する( wc.rSound[ i ].t時刻から位置を返す( nAbsTimeFromStartPlaying ) );
							wc.rSound[i].t再生位置を変更する(nAbsTimeFromStartPlaying);  // WASAPI/ASIO用
																																	//Debug.WriteLine( "再生位置を変更: " + Path.GetFileName( wc.strファイル名 ) + nAbsTimeFromStartPlaying + "ms");
						}
					}
				}
			}
		}

		/// <summary>
		/// デバッグ用
		/// </summary>
		public void tWaveBGM再生位置表示()
		{
			foreach (CWAV wc in this.listWAV.Values)
			{
				if (wc.rSound[0] != null && wc.rSound[0].n総演奏時間ms >= 5000)
				{
					for (int i = 0; i < nPolyphonicSounds; i++)
					{
						if ((wc.rSound[i] != null) && (wc.rSound[i].b再生中))
						{
							long n位置byte;
							double db位置ms;
							wc.rSound[i].t再生位置を取得する(out n位置byte, out db位置ms);
							Trace.TraceInformation("再生位置: {0}, seek先={1}ms / {2}byte, 全音長={3}ms",
								Path.GetFileName(wc.rSound[0].strファイル名),
								db位置ms, n位置byte,
								wc.rSound[0].n総演奏時間ms
							);
						}
					}
				}
			}
		}

		public void tWavの再生停止(int nWaveの内部番号)
		{
			tWavの再生停止(nWaveの内部番号, false);
		}
		public void tWavの再生停止(int nWaveの内部番号, bool bミキサーからも削除する)
		{
			if (this.listWAV.ContainsKey(nWaveの内部番号))
			{
				CWAV cwav = this.listWAV[nWaveの内部番号];
				for (int i = 0; i < nPolyphonicSounds; i++)
				{
					if (cwav.rSound[i] != null && cwav.rSound[i].b再生中)
					{
						if (bミキサーからも削除する)
						{
							cwav.rSound[i].tサウンドを停止してMixerからも削除する();
						}
						else
						{
							cwav.rSound[i].t再生を停止する();
						}
					}
				}
			}
		}
		public void tWAVの読み込み(CWAV cwav)
		{
			//			Trace.TraceInformation("WAV files={0}", this.listWAV.Count);
			//			int count = 0;
			//			foreach (CWAV cwav in this.listWAV.Values)
			{
				//				string strCount = count.ToString() + " / " + this.listWAV.Count.ToString();
				//				Debug.WriteLine(strCount);
				//				CDTXMania.Instance.app.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, strCount);
				//				count++;

				string str = string.IsNullOrEmpty(this.PATH_WAV) ? this.strフォルダ名 : this.PATH_WAV;
				str = str + cwav.strファイル名;
				bool bIsDirectSound = (CDTXMania.Instance.Sound管理.CurrentSoundDeviceType == ESoundDeviceType.DirectSound);
				try
				{
					//try
					//{
					//    cwav.rSound[ 0 ] = CDTXMania.Instance.app.Sound管理.tサウンドを生成する( str );
					//    cwav.rSound[ 0 ].n音量 = 100;
					//    if ( CDTXMania.Instance.app.ConfigIni.bLog作成解放ログ出力 )
					//    {
					//        Trace.TraceInformation( "サウンドを作成しました。({3})({0})({1})({2}bytes)", cwav.strコメント文, str, cwav.rSound[ 0 ].nサウンドバッファサイズ, cwav.rSound[ 0 ].bストリーム再生する ? "Stream" : "OnMemory" );
					//    }
					//}
					//catch
					//{
					//    cwav.rSound[ 0 ] = null;
					//    Trace.TraceError( "サウンドの作成に失敗しました。({0})({1})", cwav.strコメント文, str );
					//}
					//if ( cwav.rSound[ 0 ] == null )	// #xxxxx 2012.5.3 yyagi rSound[1-3]もClone()するようにし、これらのストリーム再生がおかしくなる問題を修正
					//{
					//    for ( int j = 1; j < nPolyphonicSounds; j++ )
					//    {
					//        cwav.rSound[ j ] = null;
					//    }
					//}
					//else
					//{
					//    for ( int j = 1; j < nPolyphonicSounds; j++ )
					//    {
					//        cwav.rSound[ j ] = (CSound) cwav.rSound[ 0 ].Clone();	// #24007 2011.9.5 yyagi add: to accelerate loading chip sounds
					//        CDTXMania.Instance.app.Sound管理.tサウンドを登録する( cwav.rSound[ j ] );
					//    }
					//}

					// まず1つめを登録する
					try
					{
						cwav.rSound[0] = CDTXMania.Instance.Sound管理.tサウンドを生成する(str);
						cwav.rSound[0].n音量 = 100;
						if (!CDTXMania.Instance.ConfigIni.bDynamicBassMixerManagement)
						{
							cwav.rSound[0].tBASSサウンドをミキサーに追加する();
						}
						if (CDTXMania.Instance.ConfigIni.bLogCreateRelease)
						{
							Trace.TraceInformation("サウンドを作成しました。({3})({0})({1})({2}bytes)", cwav.strコメント文, str, cwav.rSound[0].nサウンドバッファサイズ, cwav.rSound[0].bストリーム再生する ? "Stream" : "OnMemory");
						}
					}
					catch (Exception e)
					{
						cwav.rSound[0] = null;
						Trace.TraceError("サウンドの作成に失敗しました。({0})({1})", cwav.strコメント文, str);
						Trace.TraceError("例外: " + e.Message);
					}

					#region [ 同時発音数を、チャンネルによって変える ]
					int nPoly = nPolyphonicSounds;
					if (!bIsDirectSound)  // DShowでの再生の場合はミキシング負荷が高くないため、
					{                                   // チップのライフタイム管理を行わない
						if (cwav.bIsBassSound) nPoly = (nPolyphonicSounds >= 2) ? 2 : 1;
						else if (cwav.bIsGuitarSound) nPoly = (nPolyphonicSounds >= 2) ? 2 : 1;
						else if (cwav.bIsSESound) nPoly = 1;
						else if (cwav.bIsBGMSound) nPoly = 1;
					}
					if (cwav.bIsBGMSound) nPoly = 1;
					#endregion

					// 残りはClone等で登録する
					if (bIsDirectSound)     // DirectSoundでの再生の場合はCloneする
					{
						for (int i = 1; i < nPoly; i++)
						{
							cwav.rSound[i] = (CSound)cwav.rSound[0].Clone();    // #24007 2011.9.5 yyagi add: to accelerate loading chip sounds
																				// CDTXMania.Instance.app.Sound管理.tサウンドを登録する( cwav.rSound[ j ] );
						}
						for (int i = nPoly; i < nPolyphonicSounds; i++)
						{
							cwav.rSound[i] = null;
						}
					}
					else                                                            // WASAPI/ASIO時は通常通り登録
					{
						for (int i = 1; i < nPoly; i++)
						{
							try
							{
								cwav.rSound[i] = CDTXMania.Instance.Sound管理.tサウンドを生成する(str);
								cwav.rSound[i].n音量 = 100;
								if (!CDTXMania.Instance.ConfigIni.bDynamicBassMixerManagement)
								{
									cwav.rSound[i].tBASSサウンドをミキサーに追加する();
								}
								if (CDTXMania.Instance.ConfigIni.bLogCreateRelease)
								{
									Trace.TraceInformation("サウンドを作成しました。({3})({0})({1})({2}bytes)", cwav.strコメント文, str, cwav.rSound[0].nサウンドバッファサイズ, cwav.rSound[0].bストリーム再生する ? "Stream" : "OnMemory");
								}
							}
							catch (Exception e)
							{
								cwav.rSound[i] = null;
								Trace.TraceError("サウンドの作成に失敗しました。({0})({1})", cwav.strコメント文, str);
								Trace.TraceError("例外: " + e.Message);
							}
						}
					}
				}
				catch (Exception exception)
				{
					Trace.TraceError("サウンドの生成に失敗しました。({0})({1})({2})", exception.Message, cwav.strコメント文, str);
					for (int j = 0; j < nPolyphonicSounds; j++)
					{
						cwav.rSound[j] = null;
					}
					//continue;
				}
			}
		}
		public static string tZZ(int n)
		{
			if (n < 0 || n >= 36 * 36)
				return "!!";  // オーバー／アンダーフロー。

			// n を36進数2桁の文字列にして返す。

			string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(new char[] { str[n / 36], str[n % 36] });
		}
		public void tギターとベースのランダム化(EPart part, ERandom eRandom)
		{
			if (((part == EPart.Guitar) || (part == EPart.Bass)) && (eRandom != ERandom.Off))
			{
				int rndVal = 0;
				foreach (CChip chip in this.listChip)
				{
					bool bOpenChip = (chip.bGuitar可視チップ && this.bチップがある.OpenGuitar) || ((chip.bBass可視チップ) && this.bチップがある.OpenBass);
					if (chip[EChannel.BarLine])   // 小節が変化したら
					{
						rndVal = CDTXMania.Instance.Random.Next(6);
					}

					chip.RandomizeRGB(eRandom, rndVal, bOpenChip);// #23546 2010.10.28 yyagi fixed (bチップがある.Bass→bチップがある.OpenBass)
				}
			}
		}

		#region [ チップの再生と停止 ]
		public void tチップの再生(CChip rChip, long n再生開始システム時刻ms)
		{
			this.tチップの再生(rChip, n再生開始システム時刻ms, CDTXMania.Instance.ConfigIni.nAutoVolume, false, false);
		}
		public void tチップの再生(CChip rChip, long n再生開始システム時刻ms, int nVol)
		{
			this.tチップの再生(rChip, n再生開始システム時刻ms, nVol, false, false);
		}
		public void tチップの再生(CChip rChip, long n再生開始システム時刻ms, int nVol, bool bMIDIMonitor)
		{
			this.tチップの再生(rChip, n再生開始システム時刻ms, nVol, bMIDIMonitor, false);
		}
		public void tチップの再生(CChip pChip, long n再生開始システム時刻ms, int nVol, bool bMIDIMonitor, bool bBad)
		{
			if (pChip.n整数値_内部番号 >= 0)
			{
				if (this.listWAV.ContainsKey(pChip.n整数値_内部番号))
				{
					CWAV wc = this.listWAV[pChip.n整数値_内部番号];
					int index = wc.n現在再生中のサウンド番号 = (wc.n現在再生中のサウンド番号 + 1) % nPolyphonicSounds;
					if ((wc.rSound[0] != null) &&
						(wc.rSound[0].bストリーム再生する || wc.rSound[index] == null))
					{
						index = wc.n現在再生中のサウンド番号 = 0;
					}
					CSound sound = wc.rSound[index];
					if (sound != null)
					{
						if (bBad)
						{
							sound.db周波数倍率 = ((float)(100 + (((CDTXMania.Instance.Random.Next(3) + 1) * 7) * (1 - (CDTXMania.Instance.Random.Next(2) * 2))))) / 100f;
						}
						else
						{
							sound.db周波数倍率 = 1.0;
						}
						sound.db再生速度 = ((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0;
						// 再生速度によって、WASAPI/ASIOで使う使用mixerが決まるため、付随情報の設定(音量/PAN)は、再生速度の設定後に行う
						sound.n音量 = (int)(((double)(nVol * wc.n音量)) / 100.0);
						sound.n位置 = wc.n位置;
						sound.t再生を開始する();
					}
					wc.n再生開始時刻[wc.n現在再生中のサウンド番号] = n再生開始システム時刻ms;
					this.tWave再生位置自動補正(wc);
				}
			}
		}
		public void t各自動再生音チップの再生時刻を変更する(int nBGMAdjustの増減値)
		{
			this.nBGMAdjust += nBGMAdjustの増減値;
			foreach (CChip chip in listChip)
			{
				chip.AddPlayPositionMsForSE(nBGMAdjustの増減値);
			}
			foreach (CWAV cwav in this.listWAV.Values)
			{
				for (int j = 0; j < nPolyphonicSounds; j++)
				{
					if ((cwav.rSound[j] != null) && cwav.rSound[j].b再生中)
					{
						cwav.n再生開始時刻[j] += nBGMAdjustの増減値;
					}
				}
			}
		}
		public void t全チップの再生一時停止()
		{
			foreach (CWAV cwav in this.listWAV.Values)
			{
				for (int i = 0; i < nPolyphonicSounds; i++)
				{
					if ((cwav.rSound[i] != null) && cwav.rSound[i].b再生中)
					{
						cwav.n一時停止時刻[i] = CSound管理.rc演奏用タイマ.nシステム時刻ms;
						cwav.rSound[i].t再生を一時停止する();
					}
				}
			}
		}
		/// <summary>
		/// 全チップの再生を再開する。しかし一時停止と再開を繰り返すと、再生位置が徐々にずれる問題あり。
		/// 泥臭い回避方法は、CStage演奏画面共通.cs の tキー入力()を参照のこと。
		/// </summary>
		public void t全チップの再生再開()
		{
			foreach (CWAV cwav in this.listWAV.Values)
			{
				for (int i = 0; i < nPolyphonicSounds; i++)
				{
					if ((cwav.rSound[i] != null) && cwav.rSound[i].b一時停止中)
					{
						cwav.n再生開始時刻[i] += CSound管理.rc演奏用タイマ.nシステム時刻ms - cwav.n一時停止時刻[i];
					}
				}
			}
		}
		public void t全チップの再生停止()
		{
			foreach (CWAV cwav in this.listWAV.Values)
			{
				this.tWavの再生停止(cwav.n内部番号);
			}
		}
		public void t全チップの再生停止とミキサーからの削除()
		{
			foreach (CWAV cwav in this.listWAV.Values)
			{
				this.tWavの再生停止(cwav.n内部番号, true);
			}
		}
		#endregion

		/// <summary>
		/// サウンドミキサーにサウンドを登録・削除する時刻を事前に算出する
		/// </summary>
		public void PlanToAddMixerChannel()
		{
			if (CDTXMania.Instance.Sound管理.CurrentSoundDeviceType == ESoundDeviceType.DirectSound)  // DShowでの再生の場合はミキシング負荷が高くないため、
			{                                   // チップのライフタイム管理を行わない
				return;
			}

			List<CChip> listAddMixerChannel = new List<CChip>(128); ;
			List<CChip> listRemoveMixerChannel = new List<CChip>(128);
			List<CChip> listRemoveTiming = new List<CChip>(128);

			//foreach ( CChip pChip in listChip )
			for (int i = 0; i < listChip.Count; i++)
			{
				CChip pChip = listChip[i];
				if (pChip.bWAVを使うチャンネルである)
				{
					#region [ 発音1秒前のタイミングを記録 ]
					int n発音前余裕ms = 1000, n発音後余裕ms = 800;            // Drums
					{
						// Guitar / Bass
						if (pChip.e楽器パート == EPart.Guitar || pChip.e楽器パート == EPart.Bass)
						{
							n発音前余裕ms = 800;
							//n発音後余裕ms = 500;
						}
						// SE
						if (pChip.ESoundChipTypeを得る == ESoundChipType.SE)
						{
							n発音前余裕ms = 200;
							//n発音後余裕ms = 500;
						}
					}
					#endregion
					#region [ BGMチップならば即ミキサーに追加・・・はしない (全て注釈化) ]
					//if ( pChip.nチャンネル番号 == 0x01 )	// BGMチップは即ミキサーに追加
					//{
					//    if ( listWAV.ContainsKey( pChip.n整数値・内部番号 ) )
					//    {
					//        CDTX.CWAV wc = CDTXMania.Instance.app.DTX.listWAV[ pChip.n整数値・内部番号 ];
					//        if ( wc.rSound[ 0 ] != null )
					//        {
					//            CDTXMania.Instance.app.Sound管理.AddMixer( wc.rSound[ 0 ] );	// BGMは多重再生しない仕様としているので、1個目だけミキサーに登録すればよい
					//        }
					//    }
					//}
					#endregion
					#region [ 発音1秒前のタイミングを算出 ]
					int nAddMixer時刻ms, nAddMixer位置 = 0;
					//Debug.WriteLine("==================================================================");
					//Debug.WriteLine( "Start: ch=" + pChip.nチャンネル番号.ToString("x2") + ", nWAV番号=" + pChip.n整数値 + ", time=" + pChip.n発声時刻ms + ", lasttime=" + listChip[ listChip.Count - 1 ].n発声時刻ms );
					t発声時刻msと発声位置を取得する(pChip.n発声時刻ms - n発音前余裕ms, out nAddMixer時刻ms, out nAddMixer位置);
					//Debug.WriteLine( "nAddMixer時刻ms=" + nAddMixer時刻ms + ",nAddMixer位置=" + nAddMixer位置 );

					CChip c_AddMixer = new CChip(nAddMixer位置, pChip.n整数値, pChip.n整数値_内部番号, EChannel.MixerAdd, nAddMixer時刻ms, false);
					listAddMixerChannel.Add(c_AddMixer);
					//Debug.WriteLine("listAddMixerChannel:" );
					//DebugOut_CChipList( listAddMixerChannel );
					#endregion

					#region [ そのチップ音のfullduration(チップ音wavの最大再生時間)を取得 ]
					int fullduration = 0;
					if (listWAV.ContainsKey(pChip.n整数値_内部番号))
					{
						CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
						double _db再生速度 = (CDTXMania.Instance.DTXVmode.Enabled) ? this.dbDTXVPlaySpeed : this.db再生速度;
						fullduration = (wc.rSound[0] == null) ? 0 : (int)(wc.rSound[0].n総演奏時間ms / _db再生速度); // #23664 durationに再生速度が加味されておらず、低速再生でBGMが途切れる問題を修正 (発声時刻msは、DTX読み込み時に再生速度加味済)
					}
					//Debug.WriteLine("fullduration=" + fullduration );
					#endregion

					#region [ そのチップのduration (GtBsが次の音にかき消されることを加味した再生時間) を取得・・・のコードは未使用。mixing抑制の効果が薄いため。]
					int duration = 0;
					//{
					//    int ch = ( pChip.nチャンネル番号 >> 4 );
					//    bool bGtBs = ( ch == 0x02 || ch == 0x0A );
					//    if ( bGtBs )			// Guitar/Bassの場合
					//    {
					//        int p = i;
					//        int chNext;
					//        do
					//        {
					//            if ( ++p >= listChip.Count )
					//            {
					//                break;
					//            }
					//            chNext = ( listChip[ p ].nチャンネル番号 >> 4 );
					//            duration = listChip[ p ].n発声時刻ms - pChip.n発声時刻ms;
					//            if ( ch == chNext )
					//            {
					//                break;
					//            }
					//        }
					//        while ( duration < fullduration );
					//    }
					//    else					// ドラムスの場合
					//    {
					//        duration = fullduration;
					//    }
					//}
					#endregion
					//Debug.WriteLine( i + ": duration diff= " + (fullduration - duration ) );
					duration = fullduration;
					int n新RemoveMixer時刻ms, n新RemoveMixer位置;
					t発声時刻msと発声位置を取得する(pChip.n発声時刻ms + duration + n発音後余裕ms, out n新RemoveMixer時刻ms, out n新RemoveMixer位置);
					//Debug.WriteLine( "n新RemoveMixer時刻ms=" + n新RemoveMixer時刻ms + ",n新RemoveMixer位置=" + n新RemoveMixer位置 );
					if (n新RemoveMixer時刻ms < pChip.n発声時刻ms + duration) // 曲の最後でサウンドが切れるような場合は
					{
						CChip c_AddMixer_noremove = c_AddMixer;
						c_AddMixer_noremove.SetSoundAfterPlayEnd(true);
						listAddMixerChannel[listAddMixerChannel.Count - 1] = c_AddMixer_noremove;
						continue;                       // 発声位置の計算ができないので、Mixer削除をあきらめる・・・のではなく
																						// #32248 2013.10.15 yyagi 演奏終了後も再生を続けるチップであるというフラグをpChip内に立てる
					}
					#region [ 未使用コード ]
					//if ( n新RemoveMixer時刻ms < pChip.n発声時刻ms + duration )	// 曲の最後でサウンドが切れるような場合
					//{
					//    n新RemoveMixer時刻ms = pChip.n発声時刻ms + duration;
					//    // 「位置」は比例計算で求めてお茶を濁す...このやり方だと誤動作したため対応中止
					//    n新RemoveMixer位置 = listChip[ listChip.Count - 1 ].n発声位置 * n新RemoveMixer時刻ms / listChip[ listChip.Count - 1 ].n発声時刻ms;
					//}
					#endregion

					#region [ 発音終了2秒後にmixerから削除するが、その前に再発音することになるのかを確認(再発音ならmixer削除タイミングを延期) ]
					int n整数値 = pChip.n整数値;
					int index = listRemoveTiming.FindIndex(
						delegate (CChip cchip) { return cchip.n整数値 == n整数値; }
					);
					//Debug.WriteLine( "index=" + index );
					if (index >= 0)                         // 過去に同じチップで発音中のものが見つかった場合
					{                                 // 過去の発音のmixer削除を確定させるか、延期するかの2択。
						int n旧RemoveMixer時刻ms = listRemoveTiming[index].n発声時刻ms;
						int n旧RemoveMixer位置 = listRemoveTiming[index].n発声位置;

						//Debug.WriteLine( "n旧RemoveMixer時刻ms=" + n旧RemoveMixer時刻ms + ",n旧RemoveMixer位置=" + n旧RemoveMixer位置 );
						if (pChip.n発声時刻ms - n発音前余裕ms <= n旧RemoveMixer時刻ms)  // mixer削除前に、同じ音の再発音がある場合は、
						{                                 // mixer削除時刻を遅延させる(if-else後に行う)
																							//Debug.WriteLine( "remove TAIL of listAddMixerChannel. TAIL INDEX=" + listAddMixerChannel.Count );
																							//DebugOut_CChipList( listAddMixerChannel );
							listAddMixerChannel.RemoveAt(listAddMixerChannel.Count - 1);  // また、同じチップ音の「mixerへの再追加」は削除する
																																						//Debug.WriteLine( "removed result:" );
																																						//DebugOut_CChipList( listAddMixerChannel );
						}
						else                              // 逆に、時間軸上、mixer削除後に再発音するような流れの場合は
						{
							//Debug.WriteLine( "Publish the value(listRemoveTiming[index] to listRemoveMixerChannel." );
							listRemoveMixerChannel.Add(listRemoveTiming[index]);  // mixer削除を確定させる
																																		//Debug.WriteLine( "listRemoveMixerChannel:" );
																																		//DebugOut_CChipList( listRemoveMixerChannel );
																																		//listRemoveTiming.RemoveAt( index );
						}
						CChip c = new CChip(n新RemoveMixer位置, listRemoveTiming[index].n整数値, listRemoveTiming[index].n整数値_内部番号, EChannel.MixerRemove, n新RemoveMixer時刻ms, false);// mixer削除時刻を更新(遅延)する
						listRemoveTiming[index] = c;
						//listRemoveTiming[ index ].n発声時刻ms = n新RemoveMixer時刻ms;	// mixer削除時刻を更新(遅延)する
						//listRemoveTiming[ index ].n発声位置 = n新RemoveMixer位置;
						//Debug.WriteLine( "listRemoveTiming: modified" );
						//DebugOut_CChipList( listRemoveTiming );
					}
					else                                // 過去に同じチップを発音していないor
					{                                 // 発音していたが既にmixer削除確定していたなら
						CChip c = new CChip(n新RemoveMixer位置, pChip.n整数値, pChip.n整数値_内部番号, EChannel.MixerRemove, n新RemoveMixer時刻ms, false);// 新しくmixer削除候補として追加する
																																																															//Debug.WriteLine( "Add new chip to listRemoveMixerTiming: " );
																																																															//Debug.WriteLine( "ch=" + c.nチャンネル番号.ToString( "x2" ) + ", nWAV番号=" + c.n整数値 + ", time=" + c.n発声時刻ms + ", lasttime=" + listChip[ listChip.Count - 1 ].n発声時刻ms );
						listRemoveTiming.Add(c);
						//Debug.WriteLine( "listRemoveTiming:" );
						//DebugOut_CChipList( listRemoveTiming );
					}
					#endregion
				}
			}
			//Debug.WriteLine("==================================================================");
			//Debug.WriteLine( "Result:" );
			//Debug.WriteLine( "listAddMixerChannel:" );
			//DebugOut_CChipList( listAddMixerChannel );
			//Debug.WriteLine( "listRemoveMixerChannel:" );
			//DebugOut_CChipList( listRemoveMixerChannel );
			//Debug.WriteLine( "listRemoveTiming:" );
			//DebugOut_CChipList( listRemoveTiming );
			//Debug.WriteLine( "==================================================================" );

			listChip.AddRange(listAddMixerChannel);
			listChip.AddRange(listRemoveMixerChannel);
			listChip.AddRange(listRemoveTiming);
			listChip.Sort();
		}
		private void DebugOut_CChipList(List<CChip> c)
		{
			//Debug.WriteLine( "Count=" + c.Count );
			for (int i = 0; i < c.Count; i++)
			{
				Debug.WriteLine(i + ": ch=" + c[i].eチャンネル番号.ToString("x2") + ", WAV番号=" + c[i].n整数値 + ", time=" + c[i].n発声時刻ms);
			}
		}

		/// <summary>
		/// 発声時刻msから発声位置を逆算することはできないため、近似計算する。
		/// 具体的には、希望発声位置前後の2つのチップの発声位置の中間を取る。
		/// </summary>
		/// <param name="n希望発声時刻ms"></param>
		/// <param name="n新発声時刻ms"></param>
		/// <param name="n新発声位置"></param>
		/// <returns></returns>
		private bool t発声時刻msと発声位置を取得する(int n希望発声時刻ms, out int n新発声時刻ms, out int n新発声位置)
		{
			// 発声時刻msから発声位置を逆算することはできないため、近似計算する。
			// 具体的には、希望発声位置前後の2つのチップの発声位置の中間を取る。

			if (n希望発声時刻ms < 0)
			{
				n希望発声時刻ms = 0;
			}
			//else if ( n希望発声時刻ms > listChip[ listChip.Count - 1 ].n発声時刻ms )		// BGMの最後の余韻を殺してしまうので、この条件は外す
			//{
			//    n希望発声時刻ms = listChip[ listChip.Count - 1 ].n発声時刻ms;
			//}

			int index_min = -1, index_max = -1;
			for (int i = 0; i < listChip.Count; i++)    // 希望発声位置前後の「前」の方のチップを検索
			{
				if (listChip[i].n発声時刻ms >= n希望発声時刻ms)
				{
					index_min = i;
					break;
				}
			}
			if (index_min < 0)  // 希望発声時刻に至らずに曲が終了してしまう場合
			{
				// listの最終項目の時刻をそのまま使用する
				//・・・のではダメ。BGMが尻切れになる。
				// そこで、listの最終項目の発声時刻msと発生位置から、希望発声時刻に相当する希望発声位置を比例計算して求める。
				//n新発声時刻ms = n希望発声時刻ms;
				//n新発声位置 = listChip[ listChip.Count - 1 ].n発声位置 * n希望発声時刻ms / listChip[ listChip.Count - 1 ].n発声時刻ms;
				n新発声時刻ms = listChip[listChip.Count - 1].n発声時刻ms;
				n新発声位置 = listChip[listChip.Count - 1].n発声位置;
				return false;
			}
			index_max = index_min + 1;
			if (index_max >= listChip.Count)
			{
				index_max = index_min;
			}
			n新発声時刻ms = (listChip[index_max].n発声時刻ms + listChip[index_min].n発声時刻ms) / 2;
			n新発声位置 = (listChip[index_max].n発声位置 + listChip[index_min].n発声位置) / 2;

			return true;
		}


		/// <summary>
		/// Swap infos between Guitar and Bass (notes, level, n可視チップ数, bチップがある)
		/// </summary>
		public void SwapGuitarBassInfos()           // #24063 2011.1.24 yyagi ギターとベースの譜面情報入替
		{
			foreach (CChip chip in listChip)
			{
				chip.SwapGB();
			}
			int t = this.LEVEL.Bass;
			this.LEVEL.Bass = this.LEVEL.Guitar;
			this.LEVEL.Guitar = t;

			t = this.n可視チップ数.Bass;
			this.n可視チップ数.Bass = this.n可視チップ数.Guitar;
			this.n可視チップ数.Guitar = t;

			bool ts = this.bチップがある.Bass;
			this.bチップがある.Bass = this.bチップがある.Guitar;
			this.bチップがある.Guitar = ts;

			//			SwapGuitarBassInfos_AutoFlags();
		}

		// SwapGuitarBassInfos_AutoFlags()は、CDTXからCConfigIniに移動。

		// CActivity 実装

		public override void On活性化()
		{
			this.listWAV = new Dictionary<int, CWAV>();
			this.listBMP = new Dictionary<int, CBMP>();
			this.listBMPTEX = new Dictionary<int, CBMPTEX>();
			this.listBPM = new Dictionary<int, CBPM>();
			this.listBGAPAN = new Dictionary<int, CBGAPAN>();
			this.listBGA = new Dictionary<int, CBGA>();
			this.listAVIPAN = new Dictionary<int, CAVIPAN>();
			this.listAVI = new Dictionary<int, CAVI>();
			this.listChip = new List<CChip>();
			base.On活性化();
		}
		public override void On非活性化()
		{
			if (this.listWAV != null)
			{
				foreach (CWAV cwav in this.listWAV.Values)
				{
					cwav.Dispose();
				}
				this.listWAV = null;
			}
			if (this.listBMP != null)
			{
				foreach (CBMP cbmp in this.listBMP.Values)
				{
					cbmp.Dispose();
				}
				this.listBMP = null;
			}
			if (this.listBMPTEX != null)
			{
				foreach (CBMPTEX cbmptex in this.listBMPTEX.Values)
				{
					cbmptex.Dispose();
				}
				this.listBMPTEX = null;
			}
			if (this.listAVI != null)
			{
				foreach (CAVI cavi in this.listAVI.Values)
				{
					cavi.Dispose();
				}
				this.listAVI = null;
			}
			if (this.listBPM != null)
			{
				this.listBPM.Clear();
				this.listBPM = null;
			}
			if (this.listBGAPAN != null)
			{
				this.listBGAPAN.Clear();
				this.listBGAPAN = null;
			}
			if (this.listBGA != null)
			{
				this.listBGA.Clear();
				this.listBGA = null;
			}
			if (this.listAVIPAN != null)
			{
				this.listAVIPAN.Clear();
				this.listAVIPAN = null;
			}
			if (this.listChip != null)
			{
				this.listChip.Clear();
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tBMP_BMPTEXの読み込み();
				this.tAVIの読み込み();
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				if (this.listBMP != null)
				{
					foreach (CBMP cbmp in this.listBMP.Values)
					{
						cbmp.Dispose();
					}
				}
				if (this.listBMPTEX != null)
				{
					foreach (CBMPTEX cbmptex in this.listBMPTEX.Values)
					{
						cbmptex.Dispose();
					}
				}
				if (this.listAVI != null)
				{
					foreach (CAVI cavi in this.listAVI.Values)
					{
						cavi.Dispose();
					}
				}
				base.OnManagedリソースの解放();
			}
		}


		// その他

		#region [ private ]
		//-----------------
		/// <summary>
		/// <para>GDAチャンネル番号に対応するDTXチャンネル番号。</para>
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct STGDAPARAM
		{
			public string strGDAのチャンネル文字列;
			public EChannel eDTXのチャンネル番号;

			public STGDAPARAM(string strGDAのチャンネル文字列, EChannel eDTXのチャンネル番号)    // 2011.1.1 yyagi 構造体のコンストラクタ追加(初期化簡易化のため)
			{
				this.strGDAのチャンネル文字列 = strGDAのチャンネル文字列;
				this.eDTXのチャンネル番号 = eDTXのチャンネル番号;
			}
		}

		private readonly STGDAPARAM[] stGDAParam;
		private bool bヘッダのみ;
		private Stack<bool> bstackIFからENDIFをスキップする;

		private int n現在の行数;
		private int n現在の乱数;

		private int nPolyphonicSounds = 4;              // #28228 2012.5.1 yyagi

		private int n内部番号BPM1to;
		private int n内部番号WAV1to;
		private int[] n無限管理BPM;
		private int[] n無限管理PAN;
		private int[] n無限管理SIZE;
		private int[] n無限管理VOL;
		private int[] n無限管理WAV;
		private int[] nRESULTIMAGE用優先順位;
		private int[] nRESULTMOVIE用優先順位;
		private int[] nRESULTSOUND用優先順位;

		#region [#23880 2010.12.30 yyagi: コンマとスペースの両方を小数点として扱うTryParse]
		/// <summary>
		/// 小数点としてコンマとピリオドの両方を受け付けるTryParse()
		/// </summary>
		/// <param name="s">strings convert to double</param>
		/// <param name="result">parsed double value</param>
		/// <returns>s が正常に変換された場合は true。それ以外の場合は false。</returns>
		/// <exception cref="ArgumentException">style が NumberStyles 値でないか、style に NumberStyles.AllowHexSpecifier 値が含まれている</exception>
		private bool TryParse(string s, out double result)
		{ // #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
			// EU諸国での #BPM 123,45 のような記述に対応するため、
			// 小数点の最終位置を検出して、それをlocaleにあった
			// 文字に置き換えてからTryParse()する
			// 桁区切りの文字はスキップする

			const string DecimalSeparators = ".,";        // 小数点文字
			const string GroupSeparators = ".,' ";        // 桁区切り文字
			const string NumberSymbols = "0123456789";      // 数値文字

			int len = s.Length;                 // 文字列長
			int decimalPosition = len;              // 真の小数点の位置 最初は文字列終端位置に仮置きする

			for (int i = 0; i < len; i++)
			{             // まず、真の小数点(一番最後に現れる小数点)の位置を求める
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0)
				{       // 数値だったらスキップ
					continue;
				}
				else if (DecimalSeparators.IndexOf(c) >= 0)
				{   // 小数点文字だったら、その都度位置を上書き記憶
					decimalPosition = i;
				}
				else if (GroupSeparators.IndexOf(c) >= 0)
				{   // 桁区切り文字の場合もスキップ
					continue;
				}
				else
				{                     // 数値・小数点・区切り文字以外がきたらループ終了
					break;
				}
			}

			StringBuilder decimalStr = new StringBuilder(16);
			for (int i = 0; i < len; i++)
			{             // 次に、localeにあった数値文字列を生成する
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0)
				{       // 数値だったら
					decimalStr.Append(c);             // そのままコピー
				}
				else if (DecimalSeparators.IndexOf(c) >= 0)
				{   // 小数点文字だったら
					if (i == decimalPosition)
					{           // 最後に出現した小数点文字なら、localeに合った小数点を出力する
						decimalStr.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
					}
				}
				else if (GroupSeparators.IndexOf(c) >= 0)
				{   // 桁区切り文字だったら
					continue;                   // 何もしない(スキップ)
				}
				else
				{
					break;
				}
			}
			return double.TryParse(decimalStr.ToString(), out result);  // 最後に、自分のlocale向けの文字列に対してTryParse実行
		}
		#endregion
		//-----------------
		#endregion

		internal void t全AVIの一時停止()
		{
			// AVI の一時停止
			foreach (var avi in listAVI)
			{
				if (avi.Value.avi != null && avi.Value.avi.b再生中)
				{
					avi.Value.avi.Pause();
				}
			}

			// AVIPAN の一時停止
			foreach (var avi in listAVIPAN)
			{
				//if ( avi.Value.avi != null && avi.Value.avi.b再生中 )
				//{
				//	avi.Value.avi.ToggleRun();
				//}
			}
		}
		internal void t全AVIの再生再開()
		{
			// AVI の再生再開
			foreach (var avi in listAVI)
			{
				if (avi.Value.avi != null && avi.Value.avi.b一時停止中)
				{
					avi.Value.avi.ToggleRun();
				}
			}
			// AVIPAN の再生再開
			foreach (var avi in listAVIPAN)
			{
				//if ( avi.Value.avi != null && avi.Value.avi.b一時停止中 )
				//{
				//	avi.Value.avi.ToggleRun();
				//}
			}
		}
	}
}
