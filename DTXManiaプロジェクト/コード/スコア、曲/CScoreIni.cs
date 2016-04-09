using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using FDK;

namespace DTXMania
{
	public class CScoreIni
	{
		// プロパティ

		// [File] セクション
		public STファイル stファイル;
		[StructLayout(LayoutKind.Sequential)]
		public struct STファイル
		{
			public string Title;
			public string Name;
			public string Hash;
			public int PlayCountDrums;
			public int PlayCountGuitar;
			public int PlayCountBass;
			// #23596 10.11.16 add ikanick-----/
			public int ClearCountDrums;
			public int ClearCountGuitar;
			public int ClearCountBass;
			// #24459 2011.2.24 yyagi----------/
			public STDGBSValue<ERANK> BestRank;
			// --------------------------------/
			public int HistoryCount;
			public string[] History;
			public int BGMAdjust;
		}

		// 演奏記録セクション（9種類）
		public STセクション stセクション;
		[StructLayout(LayoutKind.Sequential)]
		public struct STセクション
		{
			public STDGBSValue<CScoreIni.C演奏記録> HiScore;
			public STDGBSValue<CScoreIni.C演奏記録> HiSkill;
			public STDGBSValue<CScoreIni.C演奏記録> LastPlay;
		}

		public enum Eセクション種別 : int
		{
			Unknown = -2,
			File = -1,
			HiScoreDrums = 0,
			HiSkillDrums = 1,
			HiScoreGuitar = 2,
			HiSkillGuitar = 3,
			HiScoreBass = 4,
			HiSkillBass = 5,
			LastPlayDrums = 6,  // #23595 2011.1.9 ikanick
			LastPlayGuitar = 7, //
			LastPlayBass = 8,   //
		}

		public enum ERANK : int   // #24459 yyagi
		{
			SS = 0,
			S = 1,
			A = 2,
			B = 3,
			C = 4,
			D = 5,
			E = 6,
			UNKNOWN = 99
		}

		public class C演奏記録
		{
			public STPadValue<bool> bAutoPlay;
			public bool bDrums有効;
			public bool bGuitar有効;
			public STDGBSValue<bool> bLight;
			public STDGBSValue<bool> bReverse;
			public STDGBSValue<ESudHidInv> eSudHidInv;
			public bool bSTAGEFAILED有効;
			public bool bTight;
			public bool b演奏にMIDI入力を使用した;
			public bool b演奏にキーボードを使用した;
			public bool b演奏にジョイパッドを使用した;
			public bool b演奏にマウスを使用した;
			public double dbゲーム型スキル値;
			public double db演奏型スキル値;
			public ECYGroup eCYGroup;
			public EDark eDark;
			public EFTGroup eFTGroup;
			public EHHGroup eHHGroup;
			public EHitSoundPriority eHitSoundPriorityCY;
			public EHitSoundPriority eHitSoundPriorityFT;
			public EHitSoundPriority eHitSoundPriorityHH;
			public STDGBSValue<ERandom> eRandom;
			public EDamage eダメージレベル;
			public STDGBSValue<float> f譜面スクロール速度;
			public string Hash;
			public int nGoodになる範囲ms;
			public int nGood数;
			public int nGreatになる範囲ms;
			public int nGreat数;
			public int nMiss数;
			public int nPerfectになる範囲ms;
			public int nPerfect数;
			public int nPoorになる範囲ms;
			public int nPoor数;
			public int nPerfect数_Auto含まない;
			public int nGreat数_Auto含まない;
			public int nGood数_Auto含まない;
			public int nPoor数_Auto含まない;
			public int nMiss数_Auto含まない;
			public long nスコア;
			public int n演奏速度分子;
			public int n演奏速度分母;
			public int n最大コンボ数;
			public int n全チップ数;
			public string strDTXManiaのバージョン;
			public bool レーン9モード;
			public int nRisky;    // #23559 2011.6.20 yyagi 0=OFF, 1-10=Risky
			public string 最終更新日時;
			public bool bギターとベースを入れ替えた; // #35417 2015.08.30 chnmr0 add

			public C演奏記録()
			{
				this.bAutoPlay = new STPadValue<bool>();
				this.eSudHidInv = new STDGBSValue<ESudHidInv>();
				this.bReverse = new STDGBSValue<bool>();
				this.eRandom = new STDGBSValue<ERandom>();
				this.bLight = new STDGBSValue<bool>();
				this.f譜面スクロール速度 = new STDGBSValue<float>();
				for (EPart i = EPart.Drums; i <= EPart.Bass; ++i)
				{
					f譜面スクロール速度[i] = 1f;
				}
				this.n演奏速度分子 = 20;
				this.n演奏速度分母 = 20;
				this.bGuitar有効 = true;
				this.bDrums有効 = true;
				this.bSTAGEFAILED有効 = true;
				this.eダメージレベル = EDamage.Normal;
				this.nPerfectになる範囲ms = 34;
				this.nGreatになる範囲ms = 67;
				this.nGoodになる範囲ms = 84;
				this.nPoorになる範囲ms = 117;
				this.strDTXManiaのバージョン = "Unknown";
				this.最終更新日時 = "";
				this.Hash = "00000000000000000000000000000000";
				this.レーン9モード = true;
				this.nRisky = 0;                  // #23559 2011.6.20 yyagi
				this.bギターとベースを入れ替えた = false; // #35417 2015.08.30 chnmr0 add
			}

			public bool bフルコンボじゃない
			{
				get
				{
					return !this.bフルコンボである;
				}
			}
			public bool bフルコンボである
			{
				get
				{
					return ((this.n最大コンボ数 > 0) && (this.n最大コンボ数 == (this.nPerfect数 + this.nGreat数 + this.nGood数 + this.nPoor数 + this.nMiss数)));
				}
			}

			public bool b全AUTOじゃない
			{
				get
				{
					return !b全AUTOである;
				}
			}
			public bool b全AUTOである
			{
				get
				{
					return (this.n全チップ数 - this.nPerfect数_Auto含まない - this.nGreat数_Auto含まない - this.nGood数_Auto含まない - this.nPoor数_Auto含まない - this.nMiss数_Auto含まない) == this.n全チップ数;
				}
			}
		}

		/// <summary>
		/// <para>.score.ini の存在するフォルダ（絶対パス；末尾に '\' はついていない）。</para>
		/// <para>未保存などでファイル名がない場合は null。</para>
		/// </summary>
		public string iniファイルのあるフォルダ名
		{
			get;
			private set;
		}

		/// <summary>
		/// <para>.score.ini のファイル名（絶対パス）。</para>
		/// <para>未保存などでファイル名がない場合は null。</para>
		/// </summary>
		public string iniファイル名
		{
			get;
			private set;
		}


		// コンストラクタ

		public CScoreIni()
		{
			this.iniファイルのあるフォルダ名 = null;
			this.iniファイル名 = null;
			this.stファイル = new STファイル();
			stファイル.Title = "";
			stファイル.Name = "";
			stファイル.Hash = "";
			stファイル.History = new string[] { "", "", "", "", "" };
			stファイル.BestRank.Drums = ERANK.UNKNOWN;    // #24459 2011.2.24 yyagi
			stファイル.BestRank.Guitar = ERANK.UNKNOWN;   //
			stファイル.BestRank.Bass = ERANK.UNKNOWN;   //

			this.stセクション = new STセクション();


			stセクション.HiScore = new STDGBSValue<C演奏記録>();
			stセクション.HiSkill = new STDGBSValue<C演奏記録>();
			stセクション.LastPlay = new STDGBSValue<C演奏記録>();

			for (EPart i = EPart.Drums; i <= EPart.Bass; ++i)
			{
				stセクション.HiScore[i] = new C演奏記録();
				stセクション.HiSkill[i] = new C演奏記録();
				stセクション.LastPlay[i] = new C演奏記録();
			}
		}

		/// <summary>
		/// <para>初期化後にiniファイルを読み込むコンストラクタ。</para>
		/// <para>読み込んだiniに不正値があれば、それが含まれるセクションをリセットする。</para>
		/// </summary>
		public CScoreIni(string str読み込むiniファイル)
			: this()
		{
			this.t読み込み(str読み込むiniファイル);
		}


		// メソッド

		/// <summary>
		/// <para>現在の this.Record[] オブジェクトの、指定されたセクションの情報が正当であるか否かを判定する。
		/// 真偽どちらでも、その内容は書き換えない。</para>
		/// </summary>
		/// <param name="eセクション">判定するセクション。</param>
		/// <returns>正当である（整合性がある）場合は true。</returns>
		public bool b整合性がある(Eセクション種別 eセクション)
		{
			return true;  // オープンソース化に伴い、整合性チェックを無効化。（2010.10.21）
		}

		/// <summary>
		/// 指定されたファイルの内容から MD5 値を求め、それを16進数に変換した文字列を返す。
		/// </summary>
		/// <param name="ファイル名">MD5 を求めるファイル名。</param>
		/// <returns>算出結果の MD5 を16進数で並べた文字列。</returns>
		public static string tファイルのMD5を求めて返す(string ファイル名)
		{
			byte[] buffer = null;
			FileStream stream = new FileStream(ファイル名, FileMode.Open, FileAccess.Read);
			buffer = new byte[stream.Length];
			stream.Read(buffer, 0, (int)stream.Length);
			stream.Close();
			StringBuilder builder = new StringBuilder(0x21);
			{
				MD5CryptoServiceProvider m = new MD5CryptoServiceProvider();
				byte[] buffer2 = m.ComputeHash(buffer);
				foreach (byte num in buffer2)
					builder.Append(num.ToString("x2"));
			}
			return builder.ToString();
		}

		/// <summary>
		/// 指定された .score.ini を読み込む。内容の真偽は判定しない。
		/// </summary>
		/// <param name="iniファイル名">読み込む .score.ini ファイルを指定します（絶対パスが安全）。</param>
		public void t読み込み(string iniファイル名)
		{
			this.iniファイルのあるフォルダ名 = Path.GetDirectoryName(iniファイル名);
			this.iniファイル名 = Path.GetFileName(iniファイル名);

			Eセクション種別 section = Eセクション種別.Unknown;
			if (File.Exists(iniファイル名))
			{
				string str;
				StreamReader reader = new StreamReader(iniファイル名, Encoding.GetEncoding("Shift_JIS"));
				while ((str = reader.ReadLine()) != null)
				{
					str = str.Replace('\t', ' ').TrimStart(new char[] { '\t', ' ' });
					if ((str.Length != 0) && (str[0] != ';'))
					{
						try
						{
							string item;
							string para;
							C演奏記録 c演奏記録;
							#region [ section ]
							if (str[0] == '[')
							{
								StringBuilder builder = new StringBuilder(0x20);
								int num = 1;
								while ((num < str.Length) && (str[num] != ']'))
								{
									builder.Append(str[num++]);
								}
								string str2 = builder.ToString();
								if (str2.Equals("File"))
								{
									section = Eセクション種別.File;
								}
								else if (str2.Equals("HiScore.Drums"))
								{
									section = Eセクション種別.HiScoreDrums;
								}
								else if (str2.Equals("HiSkill.Drums"))
								{
									section = Eセクション種別.HiSkillDrums;
								}
								else if (str2.Equals("HiScore.Guitar"))
								{
									section = Eセクション種別.HiScoreGuitar;
								}
								else if (str2.Equals("HiSkill.Guitar"))
								{
									section = Eセクション種別.HiSkillGuitar;
								}
								else if (str2.Equals("HiScore.Bass"))
								{
									section = Eセクション種別.HiScoreBass;
								}
								else if (str2.Equals("HiSkill.Bass"))
								{
									section = Eセクション種別.HiSkillBass;
								}
								// #23595 2011.1.9 ikanick
								else if (str2.Equals("LastPlay.Drums"))
								{
									section = Eセクション種別.LastPlayDrums;
								}
								else if (str2.Equals("LastPlay.Guitar"))
								{
									section = Eセクション種別.LastPlayGuitar;
								}
								else if (str2.Equals("LastPlay.Bass"))
								{
									section = Eセクション種別.LastPlayBass;
								}
								//----------------------------------------------------
								else
								{
									section = Eセクション種別.Unknown;
								}
							}
							#endregion
							else
							{
								string[] strArray = str.Split(new char[] { '=' });
								if (strArray.Length == 2)
								{
									item = strArray[0].Trim();
									para = strArray[1].Trim();
									switch (section)
									{
										case Eセクション種別.File:
											{
												if (!item.Equals("Title"))
												{
													goto Label_01C7;
												}
												this.stファイル.Title = para;
												continue;
											}
										case Eセクション種別.HiScoreDrums:
											c演奏記録 = this.stセクション.HiScore.Drums;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.HiSkillDrums:
											c演奏記録 = this.stセクション.HiSkill.Drums;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.HiScoreGuitar:
											c演奏記録 = this.stセクション.HiScore.Guitar;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.HiSkillGuitar:
											c演奏記録 = this.stセクション.HiSkill.Guitar;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.HiScoreBass:
											c演奏記録 = this.stセクション.HiScore.Bass;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.HiSkillBass:
											c演奏記録 = this.stセクション.HiSkill.Bass;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.LastPlayDrums:
											// #23595 2011.1.9 ikanick
											c演奏記録 = this.stセクション.LastPlay.Drums;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.LastPlayGuitar:
											c演奏記録 = this.stセクション.LastPlay.Guitar;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
										case Eセクション種別.LastPlayBass:
											c演奏記録 = this.stセクション.LastPlay.Bass;
											if (!item.Equals("Score"))
											{
												goto Label_03B9;
											}
											c演奏記録.nスコア = long.Parse(para);
											continue;
									}
								}
							}
							continue;
							#region [ File section ]
							Label_01C7:
							if (item.Equals("Name"))
							{
								this.stファイル.Name = para;
							}
							else if (item.Equals("Hash"))
							{
								this.stファイル.Hash = para;
							}
							else if (item.Equals("PlayCountDrums"))
							{
								this.stファイル.PlayCountDrums = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							else if (item.Equals("PlayCountGuitars"))// #23596 11.2.5 changed ikanick
							{
								this.stファイル.PlayCountGuitar = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							else if (item.Equals("PlayCountBass"))
							{
								this.stファイル.PlayCountBass = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							// #23596 10.11.16 add ikanick------------------------------------/
							else if (item.Equals("ClearCountDrums"))
							{
								this.stファイル.ClearCountDrums = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							else if (item.Equals("ClearCountGuitars"))// #23596 11.2.5 changed ikanick
							{
								this.stファイル.ClearCountGuitar = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							else if (item.Equals("ClearCountBass"))
							{
								this.stファイル.ClearCountBass = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							// #24459 2011.2.24 yyagi-----------------------------------------/
							else if (item.Equals("BestRankDrums"))
							{
								this.stファイル.BestRank.Drums = (ERANK)C変換.n値を文字列から取得して範囲内に丸めて返す(para, (int)ERANK.SS, (int)ERANK.E, (int)ERANK.UNKNOWN);
							}
							else if (item.Equals("BestRankGuitar"))
							{
								this.stファイル.BestRank.Guitar = (ERANK)C変換.n値を文字列から取得して範囲内に丸めて返す(para, (int)ERANK.SS, (int)ERANK.E, (int)ERANK.UNKNOWN);
							}
							else if (item.Equals("BestRankBass"))
							{
								this.stファイル.BestRank.Bass = (ERANK)C変換.n値を文字列から取得して範囲内に丸めて返す(para, (int)ERANK.SS, (int)ERANK.E, (int)ERANK.UNKNOWN);
							}
							//----------------------------------------------------------------/
							else if (item.Equals("History0"))
							{
								this.stファイル.History[0] = para;
							}
							else if (item.Equals("History1"))
							{
								this.stファイル.History[1] = para;
							}
							else if (item.Equals("History2"))
							{
								this.stファイル.History[2] = para;
							}
							else if (item.Equals("History3"))
							{
								this.stファイル.History[3] = para;
							}
							else if (item.Equals("History4"))
							{
								this.stファイル.History[4] = para;
							}
							else if (item.Equals("HistoryCount"))
							{
								this.stファイル.HistoryCount = C変換.n値を文字列から取得して範囲内に丸めて返す(para, 0, 99999999, 0);
							}
							else if (item.Equals("BGMAdjust"))
							{
								this.stファイル.BGMAdjust = C変換.n値を文字列から取得して返す(para, 0);
							}
							continue;
							#endregion
							#region [ Score section ]
							Label_03B9:
							if (item.Equals("PlaySkill"))
							{
								c演奏記録.db演奏型スキル値 = (double)decimal.Parse(para);
							}
							else if (item.Equals("Skill"))
							{
								c演奏記録.dbゲーム型スキル値 = (double)decimal.Parse(para);
							}
							else if (item.Equals("Perfect"))
							{
								c演奏記録.nPerfect数 = int.Parse(para);
							}
							else if (item.Equals("Great"))
							{
								c演奏記録.nGreat数 = int.Parse(para);
							}
							else if (item.Equals("Good"))
							{
								c演奏記録.nGood数 = int.Parse(para);
							}
							else if (item.Equals("Poor"))
							{
								c演奏記録.nPoor数 = int.Parse(para);
							}
							else if (item.Equals("Miss"))
							{
								c演奏記録.nMiss数 = int.Parse(para);
							}
							else if (item.Equals("MaxCombo"))
							{
								c演奏記録.n最大コンボ数 = int.Parse(para);
							}
							else if (item.Equals("TotalChips"))
							{
								c演奏記録.n全チップ数 = int.Parse(para);
							}
							else if (item.Equals("AutoPlay"))
							{
								for (int i = 0; i < para.Length; i++)
								{
									c演奏記録.bAutoPlay[(EPad)i] = C変換.bONorOFF(para[i]);
								}
							}
							else if (item.Equals("GBFlip"))
							{
								c演奏記録.bギターとベースを入れ替えた = C変換.bONorOFF(para[0]);
							}
							else if (item.Equals("Risky"))
							{
								c演奏記録.nRisky = int.Parse(para);
							}
							else if (item.Equals("TightDrums"))
							{
								c演奏記録.bTight = C変換.bONorOFF(para[0]);
							}
							else if (item.Equals("SudHidInvDrums"))
							{
								c演奏記録.eSudHidInv.Drums = (ESudHidInv)int.Parse(para);
							}
							else if (item.Equals("SudHidInvGuitar"))
							{
								c演奏記録.eSudHidInv.Guitar = (ESudHidInv)int.Parse(para);
							}
							else if (item.Equals("SudHidInvBass"))
							{
								c演奏記録.eSudHidInv.Bass = (ESudHidInv)int.Parse(para);
							}
							else if (item.Equals("ReverseDrums"))
							{
								c演奏記録.bReverse.Drums = C変換.bONorOFF(para[0]);
							}
							else if (item.Equals("ReverseGuitar"))
							{
								c演奏記録.bReverse.Guitar = C変換.bONorOFF(para[0]);
							}
							else if (item.Equals("ReverseBass"))
							{
								c演奏記録.bReverse.Bass = C変換.bONorOFF(para[0]);
							}
							#endregion
							else
							{
								#region [ RandomGuitar ]
								if (item.Equals("RandomGuitar"))
								{
									switch (int.Parse(para))
									{
										case (int)ERandom.Off:
											{
												c演奏記録.eRandom.Guitar = ERandom.Off;
												continue;
											}
										case (int)ERandom.Random:
											{
												c演奏記録.eRandom.Guitar = ERandom.Random;
												continue;
											}
										case (int)ERandom.Super:
											{
												c演奏記録.eRandom.Guitar = ERandom.Super;
												continue;
											}
										case (int)ERandom.Hyper:    // #25452 2011.6.20 yyagi
											{
												c演奏記録.eRandom.Guitar = ERandom.Super;
												continue;
											}
									}
									throw new Exception("RandomGuitar の値が無効です。");
								}
								#endregion
								#region [ RandomBass ]
								if (item.Equals("RandomBass"))
								{
									switch (int.Parse(para))
									{
										case (int)ERandom.Off:
											{
												c演奏記録.eRandom.Bass = ERandom.Off;
												continue;
											}
										case (int)ERandom.Random:
											{
												c演奏記録.eRandom.Bass = ERandom.Random;
												continue;
											}
										case (int)ERandom.Super:
											{
												c演奏記録.eRandom.Bass = ERandom.Super;
												continue;
											}
										case (int)ERandom.Hyper:    // #25452 2011.6.20 yyagi
											{
												c演奏記録.eRandom.Bass = ERandom.Super;
												continue;
											}
									}
									throw new Exception("RandomBass の値が無効です。");
								}
								#endregion
								#region [ LightGuitar ]
								if (item.Equals("LightGuitar"))
								{
									c演奏記録.bLight.Guitar = C変換.bONorOFF(para[0]);
								}
								#endregion
								#region [ LightBass ]
								else if (item.Equals("LightBass"))
								{
									c演奏記録.bLight.Bass = C変換.bONorOFF(para[0]);
								}
								#endregion
								else
								{
									#region [ Dark ]
									if (item.Equals("Dark"))
									{
										switch (int.Parse(para))
										{
											case 0:
												{
													c演奏記録.eDark = EDark.Off;
													continue;
												}
											case 1:
												{
													c演奏記録.eDark = EDark.Half;
													continue;
												}
											case 2:
												{
													c演奏記録.eDark = EDark.Full;
													continue;
												}
										}
										throw new Exception("Dark の値が無効です。");
									}
									#endregion
									#region [ ScrollSpeedDrums ]
									if (item.Equals("ScrollSpeedDrums"))
									{
										c演奏記録.f譜面スクロール速度.Drums = (float)decimal.Parse(para);
									}
									#endregion
									#region [ ScrollSpeedGuitar ]
									else if (item.Equals("ScrollSpeedGuitar"))
									{
										c演奏記録.f譜面スクロール速度.Guitar = (float)decimal.Parse(para);
									}
									#endregion
									#region [ ScrollSpeedBass ]
									else if (item.Equals("ScrollSpeedBass"))
									{
										c演奏記録.f譜面スクロール速度.Bass = (float)decimal.Parse(para);
									}
									#endregion
									#region [ PlaySpeed ]
									else if (item.Equals("PlaySpeed"))
									{
										string[] strArray2 = para.Split(new char[] { '/' });
										if (strArray2.Length == 2)
										{
											c演奏記録.n演奏速度分子 = int.Parse(strArray2[0]);
											c演奏記録.n演奏速度分母 = int.Parse(strArray2[1]);
										}
									}
									#endregion
									else
									{
										#region [ HHGroup ]
										if (item.Equals("HHGroup"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eHHGroup = EHHGroup.None;
														continue;
													}
												case 1:
													{
														c演奏記録.eHHGroup = EHHGroup.HO_HC;
														continue;
													}
												case 2:
													{
														c演奏記録.eHHGroup = EHHGroup.LC_HH;
														continue;
													}
												case 3:
													{
														c演奏記録.eHHGroup = EHHGroup.Group;
														continue;
													}
											}
											throw new Exception("HHGroup の値が無効です。");
										}
										#endregion
										#region [ FTGroup ]
										if (item.Equals("FTGroup"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eFTGroup = EFTGroup.None;
														continue;
													}
												case 1:
													{
														c演奏記録.eFTGroup = EFTGroup.Group;
														continue;
													}
											}
											throw new Exception("FTGroup の値が無効です。");
										}
										#endregion
										#region [ CYGroup ]
										if (item.Equals("CYGroup"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eCYGroup = ECYGroup.None;
														continue;
													}
												case 1:
													{
														c演奏記録.eCYGroup = ECYGroup.Group;
														continue;
													}
											}
											throw new Exception("CYGroup の値が無効です。");
										}
										#endregion
										#region [ HitSoundPriorityHH ]
										if (item.Equals("HitSoundPriorityHH"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityHH = EHitSoundPriority.Chip;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityHH = EHitSoundPriority.Pad;
														continue;
													}
											}
											throw new Exception("HitSoundPriorityHH の値が無効です。");
										}
										#endregion
										#region [ HitSoundPriorityFT ]
										if (item.Equals("HitSoundPriorityFT"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityFT = EHitSoundPriority.Chip;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityFT = EHitSoundPriority.Pad;
														continue;
													}
											}
											throw new Exception("HitSoundPriorityFT の値が無効です。");
										}
										#endregion
										#region [ HitSoundPriorityCY ]
										if (item.Equals("HitSoundPriorityCY"))
										{
											switch (int.Parse(para))
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityCY = EHitSoundPriority.Chip;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityCY = EHitSoundPriority.Pad;
														continue;
													}
											}
											throw new Exception("HitSoundPriorityCY の値が無効です。");
										}
										#endregion
										#region [ Guitar ]
										if (item.Equals("Guitar"))
										{
											c演奏記録.bGuitar有効 = C変換.bONorOFF(para[0]);
										}
										#endregion
										#region [ Drums ]
										else if (item.Equals("Drums"))
										{
											c演奏記録.bDrums有効 = C変換.bONorOFF(para[0]);
										}
										#endregion
										#region [ StageFailed ]
										else if (item.Equals("StageFailed"))
										{
											c演奏記録.bSTAGEFAILED有効 = C変換.bONorOFF(para[0]);
										}
										#endregion
										else
										{
											#region [ DamageLevel ]
											if (item.Equals("DamageLevel"))
											{
												switch (int.Parse(para))
												{
													case 0:
														{
															c演奏記録.eダメージレベル = EDamage.Easy;
															continue;
														}
													case 1:
														{
															c演奏記録.eダメージレベル = EDamage.Normal;
															continue;
														}
													case 2:
														{
															c演奏記録.eダメージレベル = EDamage.Hard;
															continue;
														}
												}
												throw new Exception("DamageLevel の値が無効です。");
											}
											#endregion
											if (item.Equals("UseKeyboard"))
											{
												c演奏記録.b演奏にキーボードを使用した = C変換.bONorOFF(para[0]);
											}
											else if (item.Equals("UseMIDIIN"))
											{
												c演奏記録.b演奏にMIDI入力を使用した = C変換.bONorOFF(para[0]);
											}
											else if (item.Equals("UseJoypad"))
											{
												c演奏記録.b演奏にジョイパッドを使用した = C変換.bONorOFF(para[0]);
											}
											else if (item.Equals("UseMouse"))
											{
												c演奏記録.b演奏にマウスを使用した = C変換.bONorOFF(para[0]);
											}
											else if (item.Equals("PerfectRange"))
											{
												c演奏記録.nPerfectになる範囲ms = int.Parse(para);
											}
											else if (item.Equals("GreatRange"))
											{
												c演奏記録.nGreatになる範囲ms = int.Parse(para);
											}
											else if (item.Equals("GoodRange"))
											{
												c演奏記録.nGoodになる範囲ms = int.Parse(para);
											}
											else if (item.Equals("PoorRange"))
											{
												c演奏記録.nPoorになる範囲ms = int.Parse(para);
											}
											else if (item.Equals("DTXManiaVersion"))
											{
												c演奏記録.strDTXManiaのバージョン = para;
											}
											else if (item.Equals("DateTime"))
											{
												c演奏記録.最終更新日時 = para;
											}
											else if (item.Equals("Hash"))
											{
												c演奏記録.Hash = para;
											}
											else if (item.Equals("9LaneMode"))
											{
												c演奏記録.レーン9モード = C変換.bONorOFF(para[0]);
											}
										}
									}
								}
							}
							continue;
						}
						catch (Exception exception)
						{
							Trace.TraceError("{0}読み込みを中断します。({1})", exception.Message, iniファイル名);
							break;
						}
					}
				}
				reader.Close();
			}
		}

		internal void tヒストリを追加する(string str追加文字列)
		{
			this.stファイル.HistoryCount++;
			for (int i = 3; i >= 0; i--)
				this.stファイル.History[i + 1] = this.stファイル.History[i];
			DateTime now = DateTime.Now;
			this.stファイル.History[0] = string.Format("{0:0}.{1:D2}/{2}/{3} {4}", this.stファイル.HistoryCount, now.Year % 100, now.Month, now.Day, str追加文字列);
		}

		internal void tセクション書き出し(TextWriter writer, Eセクション種別 esect)
		{
			C演奏記録 sect = null;
			string strSect = "";

			if (esect == Eセクション種別.HiScoreDrums)
			{
				sect = this.stセクション.HiScore.Drums;
				strSect = "HiScore.Drums";
			}
			else if (esect == Eセクション種別.HiSkillDrums)
			{
				sect = this.stセクション.HiSkill.Drums;
				strSect = "HiSkill.Drums";
			}
			else if (esect == Eセクション種別.HiScoreGuitar)
			{
				sect = this.stセクション.HiScore.Guitar;
				strSect = "HiScore.Guitar";
			}
			else if (esect == Eセクション種別.HiSkillGuitar)
			{
				sect = this.stセクション.HiSkill.Guitar;
				strSect = "HiSkill.Guitar";
			}
			else if (esect == Eセクション種別.HiScoreBass)
			{
				sect = this.stセクション.HiScore.Bass;
				strSect = "HiScore.Bass";
			}
			else if (esect == Eセクション種別.HiSkillBass)
			{
				sect = this.stセクション.HiSkill.Bass;
				strSect = "HiSkill.Bass";
			}
			else if (esect == Eセクション種別.LastPlayDrums)
			{
				sect = this.stセクション.LastPlay.Drums;
				strSect = "LastPlay.Drums";
			}
			else if (esect == Eセクション種別.LastPlayGuitar)
			{
				sect = this.stセクション.LastPlay.Guitar;
				strSect = "LastPlay.Guitar";
			}
			else if (esect == Eセクション種別.LastPlayBass)
			{
				sect = this.stセクション.LastPlay.Bass;
				strSect = "LastPlay.Bass";
			}

			if (sect != null)
			{
				writer.WriteLine("[{0}]", strSect);
				writer.WriteLine("Score={0}", sect.nスコア);
				writer.WriteLine("PlaySkill={0}", sect.db演奏型スキル値);
				writer.WriteLine("Skill={0}", sect.dbゲーム型スキル値);
				writer.WriteLine("Perfect={0}", sect.nPerfect数);
				writer.WriteLine("Great={0}", sect.nGreat数);
				writer.WriteLine("Good={0}", sect.nGood数);
				writer.WriteLine("Poor={0}", sect.nPoor数);
				writer.WriteLine("Miss={0}", sect.nMiss数);
				writer.WriteLine("MaxCombo={0}", sect.n最大コンボ数);
				writer.WriteLine("TotalChips={0}", sect.n全チップ数);
				writer.Write("AutoPlay=");
				for (EPad j = EPad.Min; j < EPad.Max; j++)
				{
					writer.Write(sect.bAutoPlay[j] ? 1 : 0);
				}
				writer.WriteLine();
				writer.WriteLine("GBFlip={0}", sect.bギターとベースを入れ替えた ? 1 : 0);
				writer.WriteLine("Risky={0}", sect.nRisky);
				writer.WriteLine("SudHidInvDrums={0}", (int)sect.eSudHidInv.Drums);
				writer.WriteLine("SudHidInvGuitar={0}", (int)sect.eSudHidInv.Guitar);
				writer.WriteLine("SudHidInvBass={0}", (int)sect.eSudHidInv.Bass);
				writer.WriteLine("ReverseDrums={0}", sect.bReverse.Drums ? 1 : 0);
				writer.WriteLine("ReverseGuitar={0}", sect.bReverse.Guitar ? 1 : 0);
				writer.WriteLine("ReverseBass={0}", sect.bReverse.Bass ? 1 : 0);
				writer.WriteLine("TightDrums={0}", sect.bTight ? 1 : 0);
				writer.WriteLine("RandomGuitar={0}", (int)sect.eRandom.Guitar);
				writer.WriteLine("RandomBass={0}", (int)sect.eRandom.Bass);
				writer.WriteLine("LightGuitar={0}", sect.bLight.Guitar ? 1 : 0);
				writer.WriteLine("LightBass={0}", sect.bLight.Bass ? 1 : 0);
				writer.WriteLine("Dark={0}", (int)sect.eDark);
				writer.WriteLine("ScrollSpeedDrums={0}", sect.f譜面スクロール速度.Drums);
				writer.WriteLine("ScrollSpeedGuitar={0}", sect.f譜面スクロール速度.Guitar);
				writer.WriteLine("ScrollSpeedBass={0}", sect.f譜面スクロール速度.Bass);
				writer.WriteLine("PlaySpeed={0}/{1}", sect.n演奏速度分子, sect.n演奏速度分母);
				writer.WriteLine("HHGroup={0}", (int)sect.eHHGroup);
				writer.WriteLine("FTGroup={0}", (int)sect.eFTGroup);
				writer.WriteLine("CYGroup={0}", (int)sect.eCYGroup);
				writer.WriteLine("HitSoundPriorityHH={0}", (int)sect.eHitSoundPriorityHH);
				writer.WriteLine("HitSoundPriorityFT={0}", (int)sect.eHitSoundPriorityFT);
				writer.WriteLine("HitSoundPriorityCY={0}", (int)sect.eHitSoundPriorityCY);
				writer.WriteLine("Guitar={0}", sect.bGuitar有効 ? 1 : 0);
				writer.WriteLine("Drums={0}", sect.bDrums有効 ? 1 : 0);
				writer.WriteLine("StageFailed={0}", sect.bSTAGEFAILED有効 ? 1 : 0);
				writer.WriteLine("DamageLevel={0}", (int)sect.eダメージレベル);
				writer.WriteLine("UseKeyboard={0}", sect.b演奏にキーボードを使用した ? 1 : 0);
				writer.WriteLine("UseMIDIIN={0}", sect.b演奏にMIDI入力を使用した ? 1 : 0);
				writer.WriteLine("UseJoypad={0}", sect.b演奏にジョイパッドを使用した ? 1 : 0);
				writer.WriteLine("UseMouse={0}", sect.b演奏にマウスを使用した ? 1 : 0);
				writer.WriteLine("PerfectRange={0}", sect.nPerfectになる範囲ms);
				writer.WriteLine("GreatRange={0}", sect.nGreatになる範囲ms);
				writer.WriteLine("GoodRange={0}", sect.nGoodになる範囲ms);
				writer.WriteLine("PoorRange={0}", sect.nPoorになる範囲ms);
				writer.WriteLine("DTXManiaVersion={0}", sect.strDTXManiaのバージョン);
				writer.WriteLine("DateTime={0}", sect.最終更新日時);
				writer.WriteLine("Hash={0}", sect.Hash);
			}

		}

		internal void t書き出し(string iniファイル名)
		{
			this.iniファイルのあるフォルダ名 = Path.GetDirectoryName(iniファイル名);
			this.iniファイル名 = Path.GetFileName(iniファイル名);

			using (StreamWriter writer = new StreamWriter(iniファイル名, false, Encoding.GetEncoding("utf-16")))
			{
				writer.WriteLine("[File]");
				writer.WriteLine("Title={0}", this.stファイル.Title);
				writer.WriteLine("Name={0}", this.stファイル.Name);
				writer.WriteLine("Hash={0}", this.stファイル.Hash);
				writer.WriteLine("PlayCountDrums={0}", this.stファイル.PlayCountDrums);
				writer.WriteLine("PlayCountGuitars={0}", this.stファイル.PlayCountGuitar);
				writer.WriteLine("PlayCountBass={0}", this.stファイル.PlayCountBass);
				writer.WriteLine("ClearCountDrums={0}", this.stファイル.ClearCountDrums);       // #23596 10.11.16 add ikanick
				writer.WriteLine("ClearCountGuitars={0}", this.stファイル.ClearCountGuitar);    //
				writer.WriteLine("ClearCountBass={0}", this.stファイル.ClearCountBass);         //
				writer.WriteLine("BestRankDrums={0}", this.stファイル.BestRank.Drums);    // #24459 2011.2.24 yyagi
				writer.WriteLine("BestRankGuitar={0}", this.stファイル.BestRank.Guitar);    //
				writer.WriteLine("BestRankBass={0}", this.stファイル.BestRank.Bass);      //
				writer.WriteLine("HistoryCount={0}", this.stファイル.HistoryCount);
				writer.WriteLine("History0={0}", this.stファイル.History[0]);
				writer.WriteLine("History1={0}", this.stファイル.History[1]);
				writer.WriteLine("History2={0}", this.stファイル.History[2]);
				writer.WriteLine("History3={0}", this.stファイル.History[3]);
				writer.WriteLine("History4={0}", this.stファイル.History[4]);
				writer.WriteLine("BGMAdjust={0}", this.stファイル.BGMAdjust);
				writer.WriteLine();

				tセクション書き出し(writer, Eセクション種別.HiScoreDrums);
				tセクション書き出し(writer, Eセクション種別.HiSkillDrums);
				tセクション書き出し(writer, Eセクション種別.HiScoreGuitar);
				tセクション書き出し(writer, Eセクション種別.HiSkillGuitar);
				tセクション書き出し(writer, Eセクション種別.HiScoreBass);
				tセクション書き出し(writer, Eセクション種別.HiSkillBass);
				tセクション書き出し(writer, Eセクション種別.LastPlayDrums);
				tセクション書き出し(writer, Eセクション種別.LastPlayGuitar);
				tセクション書き出し(writer, Eセクション種別.LastPlayBass);
			}
		}

		internal static ERANK tランク値を計算して返す(C演奏記録 part)
		{
			if (part.b演奏にMIDI入力を使用した || part.b演奏にキーボードを使用した || part.b演奏にジョイパッドを使用した || part.b演奏にマウスを使用した) // 2010.9.11
			{
				int nTotal = part.nPerfect数 + part.nGreat数 + part.nGood数 + part.nPoor数 + part.nMiss数;
				return tランク値を計算して返す(nTotal, part.nPerfect数, part.nGreat数, part.nGood数, part.nPoor数, part.nMiss数);
			}
			return ERANK.UNKNOWN;
		}

		internal static ERANK tランク値を計算して返す(int nTotal, int nPerfect, int nGreat, int nGood, int nPoor, int nMiss)
		{
			if (nTotal <= 0)
				return ERANK.UNKNOWN;

			//int nRank = (int)ERANK.E;
			int nAuto = nTotal - (nPerfect + nGreat + nGood + nPoor + nMiss);
			if (nTotal == nAuto)
			{
				return ERANK.SS;
			}
			double dRate = ((double)(nPerfect + nGreat)) / ((double)(nTotal - nAuto));
			if (dRate == 1.0)
			{
				return ERANK.SS;
			}
			if (dRate >= 0.95)
			{
				return ERANK.S;
			}
			if (dRate >= 0.9)
			{
				return ERANK.A;
			}
			if (dRate >= 0.85)
			{
				return ERANK.B;
			}
			if (dRate >= 0.8)
			{
				return ERANK.C;
			}
			if (dRate >= 0.7)
			{
				return ERANK.D;
			}
			return ERANK.E;
		}

		internal static double tゲーム型スキルを計算して返す(int nLevel, int nTotal, int nPerfect, int nCombo, EPart inst)
		{
			double ret;
			if ((nTotal == 0) || ((nPerfect == 0) && (nCombo == 0)))
				ret = 0.0;

			ret = ((nLevel * ((nPerfect * 0.8 + nCombo * 0.2) / ((double)nTotal))) / 2.0);
			ret *= dbCalcReviseValForDrGtBsAutoLanes(inst);

			return ret;
		}

		internal static double t演奏型スキルを計算して返す(int nTotal, int nPerfect, int nGreat, int nGood, int nPoor, int nMiss, EPart inst, bool auto考慮)
		{
			if (nTotal == 0)
				return 0.0;

			int nAuto = nTotal - (nPerfect + nGreat + nGood + nPoor + nMiss);
			double y = ((nPerfect * 1.0 + nGreat * 0.8 + nGood * 0.5 + nPoor * 0.2 + nMiss * 0.0 + nAuto * 0.0) * 100.0) / ((double)nTotal);
			double ret = (100.0 * ((Math.Pow(1.03, y) - 1.0) / (Math.Pow(1.03, 100.0) - 1.0)));

			if (auto考慮)
			{
				ret *= dbCalcReviseValForDrGtBsAutoLanes(inst);
			}
			return ret;
		}

		internal static double dbCalcReviseValForDrGtBsAutoLanes(EPart inst)  // #28607 2012.6.7 yyagi
		{
			double ret = 1.0;
			bool allauto = CDTXMania.Instance.ConfigIni.bIsAutoPlay(inst);
			switch (inst)
			{
				#region [ Unknown ]
				case EPart.Unknown:
					throw new ArgumentException();
				#endregion
				#region [ Drums ]
				case EPart.Drums:
					if (!allauto)
					{
						#region [ Auto BD ]
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BD)
						{
							ret /= 2;
						}
						#endregion
					}
					break;
				#endregion
				#region [ Guitar ]
				case EPart.Guitar:
					if (!allauto)
					{
						#region [ Auto Pick ]
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick)
						{
							ret /= 2;      // AutoPick時、達成率を1/2にする
						}
						#endregion
						#region [ Auto Neck ]
						int nAutoLanes = 0;
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtR)
						{
							nAutoLanes++;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtG)
						{
							nAutoLanes++;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.GtB)
						{
							nAutoLanes++;
						}
						ret /= Math.Sqrt(nAutoLanes + 1);
						#endregion
					}
					break;
				#endregion
				#region [ Bass ]
				case EPart.Bass:
					if (!allauto)
					{
						#region [ Auto Pick ]
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick)
						{
							ret /= 2;      // AutoPick時、達成率を1/2にする
						}
						#endregion
						#region [ Auto lanes ]
						int nAutoLanes = 0;
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsR)
						{
							nAutoLanes++;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsG)
						{
							nAutoLanes++;
						}
						if (CDTXMania.Instance.ConfigIni.bAutoPlay.BsB)
						{
							nAutoLanes++;
						}
						ret /= Math.Sqrt(nAutoLanes + 1);
						#endregion
					}
					break;
					#endregion
			}
			return ret;
		}

		internal static STDGBSValue<bool> t更新条件を取得する()
		{
			STDGBSValue<bool> ret = new STDGBSValue<bool>();
			for (EPart i = EPart.Drums; i <= EPart.Bass; ++i)
			{
				ret[i] = CDTXMania.Instance.ConfigIni.b楽器有効(i) && CDTXMania.Instance.DTX.bチップがある[i] && !CDTXMania.Instance.ConfigIni.bIsAutoPlay(i);
			}
			return ret;
		}

		internal static ERANK t総合ランク値を計算して返す(STDGBSValue<C演奏記録> record)
		{
			int nTotal = record.Drums.n全チップ数 + record.Guitar.n全チップ数 + record.Bass.n全チップ数;
			int nPerfect = record.Drums.nPerfect数_Auto含まない + record.Guitar.nPerfect数_Auto含まない + record.Bass.nPerfect数_Auto含まない; // #24569 2011.3.1 yyagi: to calculate result rank without AUTO chips
			int nGreat = record.Drums.nGreat数_Auto含まない + record.Guitar.nGreat数_Auto含まない + record.Bass.nGreat数_Auto含まない;   //
			int nGood = record.Drums.nGood数_Auto含まない + record.Guitar.nGood数_Auto含まない + record.Bass.nGood数_Auto含まない;   //
			int nPoor = record.Drums.nPoor数_Auto含まない + record.Guitar.nPoor数_Auto含まない + record.Bass.nPoor数_Auto含まない;   //
			int nMiss = record.Drums.nMiss数_Auto含まない + record.Guitar.nMiss数_Auto含まない + record.Bass.nMiss数_Auto含まない;   //
			return tランク値を計算して返す(nTotal, nPerfect, nGreat, nGood, nPoor, nMiss);
		}

		// その他

		#region [ private ]
		//-----------------
		/*
		private bool ONorOFF(char c)
		{
			return (c != '0');
		}
		private static char boolToChar(bool b)
		{
			if (!b)
			{
				return '0';
			}
			return '1';
		}
		*/
		//-----------------
		#endregion
	}
}
