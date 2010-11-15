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
		[StructLayout( LayoutKind.Sequential )]
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
            // --------------------------------/
			public int HistoryCount;
			public string[] History;
			public int BGMAdjust;
		}

		// 演奏記録セクション（6種類）
		public STセクション stセクション;
		[StructLayout( LayoutKind.Sequential )]
		public struct STセクション
		{
			public CScoreIni.C演奏記録 HiScoreDrums;
			public CScoreIni.C演奏記録 HiSkillDrums;
			public CScoreIni.C演奏記録 HiScoreGuitar;
			public CScoreIni.C演奏記録 HiSkillGuitar;
			public CScoreIni.C演奏記録 HiScoreBass;
			public CScoreIni.C演奏記録 HiSkillBass;
			public CScoreIni.C演奏記録 this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.HiScoreDrums;

						case 1:
							return this.HiSkillDrums;

						case 2:
							return this.HiScoreGuitar;

						case 3:
							return this.HiSkillGuitar;

						case 4:
							return this.HiScoreBass;

						case 5:
							return this.HiSkillBass;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.HiScoreDrums = value;
							return;

						case 1:
							this.HiSkillDrums = value;
							return;

						case 2:
							this.HiScoreGuitar = value;
							return;

						case 3:
							this.HiSkillGuitar = value;
							return;

						case 4:
							this.HiScoreBass = value;
							return;

						case 5:
							this.HiSkillBass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
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
		}
		public class C演奏記録
		{
			public STAUTOPLAY bAutoPlay;
			public bool bDrums有効;
			public bool bGuitar有効;
			public STDGBVALUE<bool> bHidden;
			public STDGBVALUE<bool> bLeft;
			public STDGBVALUE<bool> bLight;
			public STDGBVALUE<bool> bReverse;
			public bool bSTAGEFAILED有効;
			public STDGBVALUE<bool> bSudden;
			public bool bTight;
			public bool b演奏にMIDI入力を使用した;
			public bool b演奏にキーボードを使用した;
			public bool b演奏にジョイパッドを使用した;
			public bool b演奏にマウスを使用した;
			public double dbゲーム型スキル値;
			public double db演奏型スキル値;
			public ECYGroup eCYGroup;
			public Eダークモード eDark;
			public EFTGroup eFTGroup;
			public EHHGroup eHHGroup;
			public E打ち分け時の再生の優先順位 eHitSoundPriorityCY;
			public E打ち分け時の再生の優先順位 eHitSoundPriorityFT;
			public E打ち分け時の再生の優先順位 eHitSoundPriorityHH;
			public STDGBVALUE<Eランダムモード> eRandom;
			public Eダメージレベル eダメージレベル;
			public STDGBVALUE<float> f譜面スクロール速度;
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
			public long nスコア;
			public int n演奏速度分子;
			public int n演奏速度分母;
			public int n最大コンボ数;
			public int n全チップ数;
			public string strDTXManiaのバージョン;
			public bool レーン9モード;
			public string 最終更新日時;

			public C演奏記録()
			{
				STAUTOPLAY stautoplay = new STAUTOPLAY();
				stautoplay.LC = false;
				stautoplay.HH = false;
				stautoplay.SD = false;
				stautoplay.BD = false;
				stautoplay.HT = false;
				stautoplay.LT = false;
				stautoplay.FT = false;
				stautoplay.CY = false;
				stautoplay.Guitar = false;
				stautoplay.Bass = false;
				this.bAutoPlay = stautoplay;
				STDGBVALUE<bool> stdgbvalue = new STDGBVALUE<bool>();
				stdgbvalue.Drums = false;
				stdgbvalue.Guitar = false;
				stdgbvalue.Bass = false;
				this.bSudden = stdgbvalue;
				STDGBVALUE<bool> stdgbvalue2 = new STDGBVALUE<bool>();
				stdgbvalue2.Drums = false;
				stdgbvalue2.Guitar = false;
				stdgbvalue2.Bass = false;
				this.bHidden = stdgbvalue2;
				STDGBVALUE<bool> stdgbvalue3 = new STDGBVALUE<bool>();
				stdgbvalue3.Drums = false;
				stdgbvalue3.Guitar = false;
				stdgbvalue3.Bass = false;
				this.bReverse = stdgbvalue3;
				STDGBVALUE<Eランダムモード> stdgbvalue4 = new STDGBVALUE<Eランダムモード>();
				stdgbvalue4.Drums = Eランダムモード.OFF;
				stdgbvalue4.Guitar = Eランダムモード.OFF;
				stdgbvalue4.Bass = Eランダムモード.OFF;
				this.eRandom = stdgbvalue4;
				STDGBVALUE<bool> stdgbvalue5 = new STDGBVALUE<bool>();
				stdgbvalue5.Drums = false;
				stdgbvalue5.Guitar = false;
				stdgbvalue5.Bass = false;
				this.bLight = stdgbvalue5;
				STDGBVALUE<bool> stdgbvalue6 = new STDGBVALUE<bool>();
				stdgbvalue6.Drums = false;
				stdgbvalue6.Guitar = false;
				stdgbvalue6.Bass = false;
				this.bLeft = stdgbvalue6;
				STDGBVALUE<float> stdgbvalue7 = new STDGBVALUE<float>();
				stdgbvalue7.Drums = 1f;
				stdgbvalue7.Guitar = 1f;
				stdgbvalue7.Bass = 1f;
				this.f譜面スクロール速度 = stdgbvalue7;
				this.n演奏速度分子 = 20;
				this.n演奏速度分母 = 20;
				this.bGuitar有効 = true;
				this.bDrums有効 = true;
				this.bSTAGEFAILED有効 = true;
				this.eダメージレベル = Eダメージレベル.普通;
				this.nPerfectになる範囲ms = 0x22;
				this.nGreatになる範囲ms = 0x43;
				this.nGoodになる範囲ms = 0x54;
				this.nPoorになる範囲ms = 0x75;
				this.strDTXManiaのバージョン = "Unknown";
				this.最終更新日時 = "";
				this.Hash = "00000000000000000000000000000000";
				this.レーン9モード = true;
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
					return ( ( this.n最大コンボ数 > 0 ) && ( this.n最大コンボ数 == ( ( ( ( this.nPerfect数 + this.nGreat数 ) + this.nGood数 ) + this.nPoor数 ) + this.nMiss数 ) ) );
				}
			}

			[StructLayout( LayoutKind.Sequential )]
			public struct STAUTOPLAY
			{
				public bool LC;
				public bool HH;
				public bool SD;
				public bool BD;
				public bool HT;
				public bool LT;
				public bool FT;
				public bool CY;
				public bool Guitar;
				public bool Bass;
				public bool this[ int index ]
				{
					get
					{
						switch( index )
						{
							case 0:
								return this.LC;

							case 1:
								return this.HH;

							case 2:
								return this.SD;

							case 3:
								return this.BD;

							case 4:
								return this.HT;

							case 5:
								return this.LT;

							case 6:
								return this.FT;

							case 7:
								return this.CY;

							case 8:
								return this.Guitar;

							case 9:
								return this.Bass;
						}
						throw new IndexOutOfRangeException();
					}
					set
					{
						switch( index )
						{
							case 0:
								this.LC = value;
								return;

							case 1:
								this.HH = value;
								return;

							case 2:
								this.SD = value;
								return;

							case 3:
								this.BD = value;
								return;

							case 4:
								this.HT = value;
								return;

							case 5:
								this.LT = value;
								return;

							case 6:
								this.FT = value;
								return;

							case 7:
								this.CY = value;
								return;

							case 8:
								this.Guitar = value;
								return;

							case 9:
								this.Bass = value;
								return;
						}
						throw new IndexOutOfRangeException();
					}
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
			STファイル stファイル = new STファイル();
			stファイル.Title = "";
			stファイル.Name = "";
			stファイル.Hash = "";
			stファイル.History = new string[] { "", "", "", "", "" };
			this.stファイル = stファイル;
			STセクション stセクション = new STセクション();
			stセクション.HiScoreDrums = new C演奏記録();
			stセクション.HiSkillDrums = new C演奏記録();
			stセクション.HiScoreGuitar = new C演奏記録();
			stセクション.HiSkillGuitar = new C演奏記録();
			stセクション.HiScoreBass = new C演奏記録();
			stセクション.HiSkillBass = new C演奏記録();
			this.stセクション = stセクション;
		}

		/// <summary>
		/// <para>初期化後にiniファイルを読み込むコンストラクタ。</para>
		/// <para>読み込んだiniに不正値があれば、それが含まれるセクションをリセットする。</para>
		/// </summary>
		public CScoreIni( string str読み込むiniファイル )
			: this()
		{
			this.t読み込み( str読み込むiniファイル );
			this.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();
		}


		// メソッド

		/// <summary>
		/// <para>現在の this.Record[] オブジェクトの、指定されたセクションの情報が正当であるか否かを判定する。
		/// 真偽どちらでも、その内容は書き換えない。</para>
		/// </summary>
		/// <param name="eセクション">判定するセクション。</param>
		/// <returns>正当である（整合性がある）場合は true。</returns>
		public bool b整合性がある( Eセクション種別 eセクション )
		{
			return true;	// オープンソース化に伴い、整合性チェックを無効化。（2010.10.21）
		}
		
		/// <summary>
		/// 指定されたファイルの内容から MD5 値を求め、それを16進数に変換した文字列を返す。
		/// </summary>
		/// <param name="ファイル名">MD5 を求めるファイル名。</param>
		/// <returns>算出結果の MD5 を16進数で並べた文字列。</returns>
		public static string tファイルのMD5を求めて返す( string ファイル名 )
		{
			byte[] buffer = null;
			FileStream stream = new FileStream( ファイル名, FileMode.Open, FileAccess.Read );
			buffer = new byte[ stream.Length ];
			stream.Read( buffer, 0, (int) stream.Length );
			stream.Close();
			byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash( buffer );

			StringBuilder builder = new StringBuilder( 0x21 );
			foreach( byte num in buffer2 )
				builder.Append( num.ToString( "x2" ) );

			return builder.ToString();
		}
		
		/// <summary>
		/// 指定された .score.ini を読み込む。内容の真偽は判定しない。
		/// </summary>
		/// <param name="iniファイル名">読み込む .score.ini ファイルを指定します（絶対パスが安全）。</param>
		public void t読み込み( string iniファイル名 )
		{
			this.iniファイルのあるフォルダ名 = Path.GetDirectoryName( iniファイル名 );
			this.iniファイル名 = Path.GetFileName( iniファイル名 );

			Eセクション種別 unknown = Eセクション種別.Unknown;
			if( File.Exists( iniファイル名 ) )
			{
				string str;
				StreamReader reader = new StreamReader( iniファイル名, Encoding.GetEncoding( "shift-jis" ) );
				while( ( str = reader.ReadLine() ) != null )
				{
					str = str.Replace( '\t', ' ' ).TrimStart( new char[] { '\t', ' ' } );
					if( ( str.Length != 0 ) && ( str[ 0 ] != ';' ) )
					{
						try
						{
							string str3;
							string str4;
							C演奏記録 c演奏記録;
							if( str[ 0 ] == '[' )
							{
								StringBuilder builder = new StringBuilder( 0x20 );
								int num = 1;
								while( ( num < str.Length ) && ( str[ num ] != ']' ) )
								{
									builder.Append( str[ num++ ] );
								}
								string str2 = builder.ToString();
								if( str2.Equals( "File" ) )
								{
									unknown = Eセクション種別.File;
								}
								else if( str2.Equals( "HiScore.Drums" ) )
								{
									unknown = Eセクション種別.HiScoreDrums;
								}
								else if( str2.Equals( "HiSkill.Drums" ) )
								{
									unknown = Eセクション種別.HiSkillDrums;
								}
								else if( str2.Equals( "HiScore.Guitar" ) )
								{
									unknown = Eセクション種別.HiScoreGuitar;
								}
								else if( str2.Equals( "HiSkill.Guitar" ) )
								{
									unknown = Eセクション種別.HiSkillGuitar;
								}
								else if( str2.Equals( "HiScore.Bass" ) )
								{
									unknown = Eセクション種別.HiScoreBass;
								}
								else if( str2.Equals( "HiSkill.Bass" ) )
								{
									unknown = Eセクション種別.HiSkillBass;
								}
								else
								{
									unknown = Eセクション種別.Unknown;
								}
							}
							else
							{
								string[] strArray = str.Split( new char[] { '=' } );
								if( strArray.Length == 2 )
								{
									str3 = strArray[ 0 ].Trim();
									str4 = strArray[ 1 ].Trim();
									switch( unknown )
									{
										case Eセクション種別.File:
											{
												if( !str3.Equals( "Title" ) )
												{
													goto Label_01C7;
												}
												this.stファイル.Title = str4;
												continue;
											}
										case Eセクション種別.HiScoreDrums:
										case Eセクション種別.HiSkillDrums:
										case Eセクション種別.HiScoreGuitar:
										case Eセクション種別.HiSkillGuitar:
										case Eセクション種別.HiScoreBass:
										case Eセクション種別.HiSkillBass:
											{
												c演奏記録 = this.stセクション[ (int) unknown ];
												if( !str3.Equals( "Score" ) )
												{
													goto Label_03B9;
												}
												c演奏記録.nスコア = long.Parse( str4 );
												continue;
											}
									}
								}
							}
							continue;
						Label_01C7:
							if( str3.Equals( "Name" ) )
							{
								this.stファイル.Name = str4;
							}
							else if( str3.Equals( "Hash" ) )
							{
								this.stファイル.Hash = str4;
							}
							else if( str3.Equals( "PlayCountDrums" ) )
							{
								this.stファイル.PlayCountDrums = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, 0 );
							}
							else if( str3.Equals( "PlayCountGuitar" ) )
							{
								this.stファイル.PlayCountGuitar = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, 0 );
							}
							else if( str3.Equals( "PlayCountBass" ) )
							{
								this.stファイル.PlayCountBass = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, 0 );
                            }
                            // #23596 10.11.16 add ikanick------------------------------------/
                            else if (str3.Equals("ClearCountDrums"))
                            {
                                this.stファイル.ClearCountDrums = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 0, 0x5f5e0ff, 0);
                            }
                            else if (str3.Equals("ClearCountGuitar"))
                            {
                                this.stファイル.ClearCountGuitar = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 0, 0x5f5e0ff, 0);
                            }
                            else if (str3.Equals("ClearCountBass"))
                            {
                                this.stファイル.ClearCountBass = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 0, 0x5f5e0ff, 0);
                            }
                            //----------------------------------------------------------------/
							else if( str3.Equals( "History0" ) )
							{
								this.stファイル.History[ 0 ] = str4;
							}
							else if( str3.Equals( "History1" ) )
							{
								this.stファイル.History[ 1 ] = str4;
							}
							else if( str3.Equals( "History2" ) )
							{
								this.stファイル.History[ 2 ] = str4;
							}
							else if( str3.Equals( "History3" ) )
							{
								this.stファイル.History[ 3 ] = str4;
							}
							else if( str3.Equals( "History4" ) )
							{
								this.stファイル.History[ 4 ] = str4;
							}
							else if( str3.Equals( "HistoryCount" ) )
							{
								this.stファイル.HistoryCount = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, 0 );
							}
							else if( str3.Equals( "BGMAdjust" ) )
							{
								this.stファイル.BGMAdjust = C変換.n値を文字列から取得して返す( str4, 0 );
							}
							continue;
						Label_03B9:
							if( str3.Equals( "PlaySkill" ) )
							{
								c演奏記録.db演奏型スキル値 = (double) decimal.Parse( str4 );
							}
							else if( str3.Equals( "Skill" ) )
							{
								c演奏記録.dbゲーム型スキル値 = (double) decimal.Parse( str4 );
							}
							else if( str3.Equals( "Perfect" ) )
							{
								c演奏記録.nPerfect数 = int.Parse( str4 );
							}
							else if( str3.Equals( "Great" ) )
							{
								c演奏記録.nGreat数 = int.Parse( str4 );
							}
							else if( str3.Equals( "Good" ) )
							{
								c演奏記録.nGood数 = int.Parse( str4 );
							}
							else if( str3.Equals( "Poor" ) )
							{
								c演奏記録.nPoor数 = int.Parse( str4 );
							}
							else if( str3.Equals( "Miss" ) )
							{
								c演奏記録.nMiss数 = int.Parse( str4 );
							}
							else if( str3.Equals( "MaxCombo" ) )
							{
								c演奏記録.n最大コンボ数 = int.Parse( str4 );
							}
							else if( str3.Equals( "TotalChips" ) )
							{
								c演奏記録.n全チップ数 = int.Parse( str4 );
							}
							else if( str3.Equals( "AutoPlay" ) )
							{
								if( str4.Length == 9 )
								{
									for( int i = 0; i < 9; i++ )
									{
										c演奏記録.bAutoPlay[ i ] = this.ONorOFF( str4[ i ] );
									}
								}
								else if( str4.Length == 10 )
								{
									for( int j = 0; j < 10; j++ )
									{
										c演奏記録.bAutoPlay[ j ] = this.ONorOFF( str4[ j ] );
									}
								}
							}
							else if( str3.Equals( "TightDrums" ) )
							{
								c演奏記録.bTight = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "SuddenDrums" ) )
							{
								c演奏記録.bSudden.Drums = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "SuddenGuitar" ) )
							{
								c演奏記録.bSudden.Guitar = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "SuddenBass" ) )
							{
								c演奏記録.bSudden.Bass = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "HiddenDrums" ) )
							{
								c演奏記録.bHidden.Drums = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "HiddenGuitar" ) )
							{
								c演奏記録.bHidden.Guitar = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "HiddenBass" ) )
							{
								c演奏記録.bHidden.Bass = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "ReverseDrums" ) )
							{
								c演奏記録.bReverse.Drums = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "ReverseGuitar" ) )
							{
								c演奏記録.bReverse.Guitar = C変換.bONorOFF( str4[ 0 ] );
							}
							else if( str3.Equals( "ReverseBass" ) )
							{
								c演奏記録.bReverse.Bass = C変換.bONorOFF( str4[ 0 ] );
							}
							else
							{
								if( str3.Equals( "RandomGuitar" ) )
								{
									switch( int.Parse( str4 ) )
									{
										case 0:
											{
												c演奏記録.eRandom.Guitar = Eランダムモード.OFF;
												continue;
											}
										case 1:
											{
												c演奏記録.eRandom.Guitar = Eランダムモード.RANDOM;
												continue;
											}
										case 2:
											{
												c演奏記録.eRandom.Guitar = Eランダムモード.SUPERRANDOM;
												continue;
											}
									}
									throw new Exception( "RandomGuitar の値が無効です。" );
								}
								if( str3.Equals( "RandomBass" ) )
								{
									switch( int.Parse( str4 ) )
									{
										case 0:
											{
												c演奏記録.eRandom.Bass = Eランダムモード.OFF;
												continue;
											}
										case 1:
											{
												c演奏記録.eRandom.Bass = Eランダムモード.RANDOM;
												continue;
											}
										case 2:
											{
												c演奏記録.eRandom.Bass = Eランダムモード.SUPERRANDOM;
												continue;
											}
									}
									throw new Exception( "RandomBass の値が無効です。" );
								}
								if( str3.Equals( "LightGuitar" ) )
								{
									c演奏記録.bLight.Guitar = C変換.bONorOFF( str4[ 0 ] );
								}
								else if( str3.Equals( "LightBass" ) )
								{
									c演奏記録.bLight.Bass = C変換.bONorOFF( str4[ 0 ] );
								}
								else if( str3.Equals( "LeftGuitar" ) )
								{
									c演奏記録.bLeft.Guitar = C変換.bONorOFF( str4[ 0 ] );
								}
								else if( str3.Equals( "LeftBass" ) )
								{
									c演奏記録.bLeft.Bass = C変換.bONorOFF( str4[ 0 ] );
								}
								else
								{
									if( str3.Equals( "Dark" ) )
									{
										switch( int.Parse( str4 ) )
										{
											case 0:
												{
													c演奏記録.eDark = Eダークモード.OFF;
													continue;
												}
											case 1:
												{
													c演奏記録.eDark = Eダークモード.HALF;
													continue;
												}
											case 2:
												{
													c演奏記録.eDark = Eダークモード.FULL;
													continue;
												}
										}
										throw new Exception( "Dark の値が無効です。" );
									}
									if( str3.Equals( "ScrollSpeedDrums" ) )
									{
										c演奏記録.f譜面スクロール速度.Drums = (float) decimal.Parse( str4 );
									}
									else if( str3.Equals( "ScrollSpeedGuitar" ) )
									{
										c演奏記録.f譜面スクロール速度.Guitar = (float) decimal.Parse( str4 );
									}
									else if( str3.Equals( "ScrollSpeedBass" ) )
									{
										c演奏記録.f譜面スクロール速度.Bass = (float) decimal.Parse( str4 );
									}
									else if( str3.Equals( "PlaySpeed" ) )
									{
										string[] strArray2 = str4.Split( new char[] { '/' } );
										if( strArray2.Length == 2 )
										{
											c演奏記録.n演奏速度分子 = int.Parse( strArray2[ 0 ] );
											c演奏記録.n演奏速度分母 = int.Parse( strArray2[ 1 ] );
										}
									}
									else
									{
										if( str3.Equals( "HHGroup" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eHHGroup = EHHGroup.全部打ち分ける;
														continue;
													}
												case 1:
													{
														c演奏記録.eHHGroup = EHHGroup.ハイハットのみ打ち分ける;
														continue;
													}
												case 2:
													{
														c演奏記録.eHHGroup = EHHGroup.左シンバルのみ打ち分ける;
														continue;
													}
												case 3:
													{
														c演奏記録.eHHGroup = EHHGroup.全部共通;
														continue;
													}
											}
											throw new Exception( "HHGroup の値が無効です。" );
										}
										if( str3.Equals( "FTGroup" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eFTGroup = EFTGroup.打ち分ける;
														continue;
													}
												case 1:
													{
														c演奏記録.eFTGroup = EFTGroup.共通;
														continue;
													}
											}
											throw new Exception( "FTGroup の値が無効です。" );
										}
										if( str3.Equals( "CYGroup" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eCYGroup = ECYGroup.打ち分ける;
														continue;
													}
												case 1:
													{
														c演奏記録.eCYGroup = ECYGroup.共通;
														continue;
													}
											}
											throw new Exception( "CYGroup の値が無効です。" );
										}
										if( str3.Equals( "HitSoundPriorityHH" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityHH = E打ち分け時の再生の優先順位.ChipがPadより優先;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityHH = E打ち分け時の再生の優先順位.PadがChipより優先;
														continue;
													}
											}
											throw new Exception( "HitSoundPriorityHH の値が無効です。" );
										}
										if( str3.Equals( "HitSoundPriorityFT" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityFT = E打ち分け時の再生の優先順位.ChipがPadより優先;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityFT = E打ち分け時の再生の優先順位.PadがChipより優先;
														continue;
													}
											}
											throw new Exception( "HitSoundPriorityFT の値が無効です。" );
										}
										if( str3.Equals( "HitSoundPriorityCY" ) )
										{
											switch( int.Parse( str4 ) )
											{
												case 0:
													{
														c演奏記録.eHitSoundPriorityCY = E打ち分け時の再生の優先順位.ChipがPadより優先;
														continue;
													}
												case 1:
													{
														c演奏記録.eHitSoundPriorityCY = E打ち分け時の再生の優先順位.PadがChipより優先;
														continue;
													}
											}
											throw new Exception( "HitSoundPriorityCY の値が無効です。" );
										}
										if( str3.Equals( "Guitar" ) )
										{
											c演奏記録.bGuitar有効 = C変換.bONorOFF( str4[ 0 ] );
										}
										else if( str3.Equals( "Drums" ) )
										{
											c演奏記録.bDrums有効 = C変換.bONorOFF( str4[ 0 ] );
										}
										else if( str3.Equals( "StageFailed" ) )
										{
											c演奏記録.bSTAGEFAILED有効 = C変換.bONorOFF( str4[ 0 ] );
										}
										else
										{
											if( str3.Equals( "DamageLevel" ) )
											{
												switch( int.Parse( str4 ) )
												{
													case 0:
														{
															c演奏記録.eダメージレベル = Eダメージレベル.少ない;
															continue;
														}
													case 1:
														{
															c演奏記録.eダメージレベル = Eダメージレベル.普通;
															continue;
														}
													case 2:
														{
															c演奏記録.eダメージレベル = Eダメージレベル.大きい;
															continue;
														}
												}
												throw new Exception( "DamageLevel の値が無効です。" );
											}
											if( str3.Equals( "UseKeyboard" ) )
											{
												c演奏記録.b演奏にキーボードを使用した = C変換.bONorOFF( str4[ 0 ] );
											}
											else if( str3.Equals( "UseMIDIIN" ) )
											{
												c演奏記録.b演奏にMIDI入力を使用した = C変換.bONorOFF( str4[ 0 ] );
											}
											else if( str3.Equals( "UseJoypad" ) )
											{
												c演奏記録.b演奏にジョイパッドを使用した = C変換.bONorOFF( str4[ 0 ] );
											}
											else if( str3.Equals( "UseMouse" ) )
											{
												c演奏記録.b演奏にマウスを使用した = C変換.bONorOFF( str4[ 0 ] );
											}
											else if( str3.Equals( "PerfectRange" ) )
											{
												c演奏記録.nPerfectになる範囲ms = int.Parse( str4 );
											}
											else if( str3.Equals( "GreatRange" ) )
											{
												c演奏記録.nGreatになる範囲ms = int.Parse( str4 );
											}
											else if( str3.Equals( "GoodRange" ) )
											{
												c演奏記録.nGoodになる範囲ms = int.Parse( str4 );
											}
											else if( str3.Equals( "PoorRange" ) )
											{
												c演奏記録.nPoorになる範囲ms = int.Parse( str4 );
											}
											else if( str3.Equals( "DTXManiaVersion" ) )
											{
												c演奏記録.strDTXManiaのバージョン = str4;
											}
											else if( str3.Equals( "DateTime" ) )
											{
												c演奏記録.最終更新日時 = str4;
											}
											else if( str3.Equals( "Hash" ) )
											{
												c演奏記録.Hash = str4;
											}
											else if( str3.Equals( "9LaneMode" ) )
											{
												c演奏記録.レーン9モード = C変換.bONorOFF( str4[ 0 ] );
											}
										}
									}
								}
							}
							continue;
						}
						catch( Exception exception )
						{
							Trace.TraceError( "{0}読み込みを中断します。({1})", new object[] { exception.Message, iniファイル名 } );
							break;
						}
					}
				}
				reader.Close();
			}
		}

		internal void tヒストリを追加する( string str追加文字列 )
		{
			this.stファイル.HistoryCount++;
			for( int i = 3; i >= 0; i-- )
				this.stファイル.History[ i + 1 ] = this.stファイル.History[ i ];
			DateTime now = DateTime.Now;
			this.stファイル.History[ 0 ] = string.Format( "{0:0}.{1:D2}/{2}/{3} {4}", new object[] { this.stファイル.HistoryCount, now.Year % 100, now.Month, now.Day, str追加文字列 } );
		}
		internal void t書き出し( string iniファイル名 )
		{
			this.iniファイルのあるフォルダ名 = Path.GetDirectoryName( iniファイル名 );
			this.iniファイル名 = Path.GetFileName( iniファイル名 );

			StreamWriter writer = new StreamWriter( iniファイル名, false, Encoding.GetEncoding( "shift-jis" ) );
			writer.WriteLine( "[File]" );
			writer.WriteLine( "Title={0}", this.stファイル.Title );
			writer.WriteLine( "Name={0}", this.stファイル.Name );
			writer.WriteLine( "Hash={0}", this.stファイル.Hash );
			writer.WriteLine( "PlayCountDrums={0}", this.stファイル.PlayCountDrums );
			writer.WriteLine( "PlayCountGuitars={0}", this.stファイル.PlayCountGuitar );
            writer.WriteLine( "PlayCountBass={0}", this.stファイル.PlayCountBass );
            writer.WriteLine( "ClearCountDrums={0}", this.stファイル.ClearCountDrums );       // #23596 10.11.16 add ikanick
            writer.WriteLine( "ClearCountGuitars={0}", this.stファイル.ClearCountGuitar );    // #23596 10.11.16 add ikanick
            writer.WriteLine( "ClearCountBass={0}", this.stファイル.ClearCountBass );         // #23596 10.11.16 add ikanick
			writer.WriteLine( "HistoryCount={0}", this.stファイル.HistoryCount );
			writer.WriteLine( "History0={0}", this.stファイル.History[ 0 ] );
			writer.WriteLine( "History1={0}", this.stファイル.History[ 1 ] );
			writer.WriteLine( "History2={0}", this.stファイル.History[ 2 ] );
			writer.WriteLine( "History3={0}", this.stファイル.History[ 3 ] );
			writer.WriteLine( "History4={0}", this.stファイル.History[ 4 ] );
			writer.WriteLine( "BGMAdjust={0}", this.stファイル.BGMAdjust );
			writer.WriteLine();
			for( int i = 0; i < 6; i++ )
			{
				string[] strArray = new string[] { "HiScore.Drums", "HiSkill.Drums", "HiScore.Guitar", "HiSkill.Guitar", "HiScore.Bass", "HiSkill.Bass" };
				writer.WriteLine( "[{0}]", strArray[ i ] );
				writer.WriteLine( "Score={0}", this.stセクション[ i ].nスコア );
				writer.WriteLine( "PlaySkill={0}", this.stセクション[ i ].db演奏型スキル値 );
				writer.WriteLine( "Skill={0}", this.stセクション[ i ].dbゲーム型スキル値 );
				writer.WriteLine( "Perfect={0}", this.stセクション[ i ].nPerfect数 );
				writer.WriteLine( "Great={0}", this.stセクション[ i ].nGreat数 );
				writer.WriteLine( "Good={0}", this.stセクション[ i ].nGood数 );
				writer.WriteLine( "Poor={0}", this.stセクション[ i ].nPoor数 );
				writer.WriteLine( "Miss={0}", this.stセクション[ i ].nMiss数 );
				writer.WriteLine( "MaxCombo={0}", this.stセクション[ i ].n最大コンボ数 );
				writer.WriteLine( "TotalChips={0}", this.stセクション[ i ].n全チップ数 );
				writer.Write( "AutoPlay=" );
				for( int j = 0; j < 10; j++ )
				{
					writer.Write( this.stセクション[ i ].bAutoPlay[ j ] ? 1 : 0 );
				}
				writer.WriteLine();
				writer.WriteLine( "SuddenDrums={0}", this.stセクション[ i ].bSudden.Drums ? 1 : 0 );
				writer.WriteLine( "SuddenGuitar={0}", this.stセクション[ i ].bSudden.Guitar ? 1 : 0 );
				writer.WriteLine( "SuddenBass={0}", this.stセクション[ i ].bSudden.Bass ? 1 : 0 );
				writer.WriteLine( "HiddenDrums={0}", this.stセクション[ i ].bHidden.Drums ? 1 : 0 );
				writer.WriteLine( "HiddenGuitar={0}", this.stセクション[ i ].bHidden.Guitar ? 1 : 0 );
				writer.WriteLine( "HiddenBass={0}", this.stセクション[ i ].bHidden.Bass ? 1 : 0 );
				writer.WriteLine( "ReverseDrums={0}", this.stセクション[ i ].bReverse.Drums ? 1 : 0 );
				writer.WriteLine( "ReverseGuitar={0}", this.stセクション[ i ].bReverse.Guitar ? 1 : 0 );
				writer.WriteLine( "ReverseBass={0}", this.stセクション[ i ].bReverse.Bass ? 1 : 0 );
				writer.WriteLine( "TightDrums={0}", this.stセクション[ i ].bTight ? 1 : 0 );
				writer.WriteLine( "RandomGuitar={0}", (int) this.stセクション[ i ].eRandom.Guitar );
				writer.WriteLine( "RandomBass={0}", (int) this.stセクション[ i ].eRandom.Bass );
				writer.WriteLine( "LightGuitar={0}", this.stセクション[ i ].bLight.Guitar ? 1 : 0 );
				writer.WriteLine( "LightBass={0}", this.stセクション[ i ].bLight.Bass ? 1 : 0 );
				writer.WriteLine( "LeftGuitar={0}", this.stセクション[ i ].bLeft.Guitar ? 1 : 0 );
				writer.WriteLine( "LeftBass={0}", this.stセクション[ i ].bLeft.Bass ? 1 : 0 );
				writer.WriteLine( "Dark={0}", (int) this.stセクション[ i ].eDark );
				writer.WriteLine( "ScrollSpeedDrums={0}", this.stセクション[ i ].f譜面スクロール速度.Drums );
				writer.WriteLine( "ScrollSpeedGuitar={0}", this.stセクション[ i ].f譜面スクロール速度.Guitar );
				writer.WriteLine( "ScrollSpeedBass={0}", this.stセクション[ i ].f譜面スクロール速度.Bass );
				writer.WriteLine( "PlaySpeed={0}/{1}", this.stセクション[ i ].n演奏速度分子, this.stセクション[ i ].n演奏速度分母 );
				writer.WriteLine( "HHGroup={0}", (int) this.stセクション[ i ].eHHGroup );
				writer.WriteLine( "FTGroup={0}", (int) this.stセクション[ i ].eFTGroup );
				writer.WriteLine( "CYGroup={0}", (int) this.stセクション[ i ].eCYGroup );
				writer.WriteLine( "HitSoundPriorityHH={0}", (int) this.stセクション[ i ].eHitSoundPriorityHH );
				writer.WriteLine( "HitSoundPriorityFT={0}", (int) this.stセクション[ i ].eHitSoundPriorityFT );
				writer.WriteLine( "HitSoundPriorityCY={0}", (int) this.stセクション[ i ].eHitSoundPriorityCY );
				writer.WriteLine( "Guitar={0}", this.stセクション[ i ].bGuitar有効 ? 1 : 0 );
				writer.WriteLine( "Drums={0}", this.stセクション[ i ].bDrums有効 ? 1 : 0 );
				writer.WriteLine( "StageFailed={0}", this.stセクション[ i ].bSTAGEFAILED有効 ? 1 : 0 );
				writer.WriteLine( "DamageLevel={0}", (int) this.stセクション[ i ].eダメージレベル );
				writer.WriteLine( "UseKeyboard={0}", this.stセクション[ i ].b演奏にキーボードを使用した ? 1 : 0 );
				writer.WriteLine( "UseMIDIIN={0}", this.stセクション[ i ].b演奏にMIDI入力を使用した ? 1 : 0 );
				writer.WriteLine( "UseJoypad={0}", this.stセクション[ i ].b演奏にジョイパッドを使用した ? 1 : 0 );
				writer.WriteLine( "UseMouse={0}", this.stセクション[ i ].b演奏にマウスを使用した ? 1 : 0 );
				writer.WriteLine( "PerfectRange={0}", this.stセクション[ i ].nPerfectになる範囲ms );
				writer.WriteLine( "GreatRange={0}", this.stセクション[ i ].nGreatになる範囲ms );
				writer.WriteLine( "GoodRange={0}", this.stセクション[ i ].nGoodになる範囲ms );
				writer.WriteLine( "PoorRange={0}", this.stセクション[ i ].nPoorになる範囲ms );
				writer.WriteLine( "DTXManiaVersion={0}", this.stセクション[ i ].strDTXManiaのバージョン );
				writer.WriteLine( "DateTime={0}", this.stセクション[ i ].最終更新日時 );
				writer.WriteLine( "Hash={0}", this.stセクション[ i ].Hash );
			}
			writer.Close();
		}
		internal void t全演奏記録セクションの整合性をチェックし不整合があればリセットする()
		{
			for( int i = 0; i < 6; i++ )
			{
				if( !this.b整合性がある( (Eセクション種別) i ) )
					this.stセクション[ i ] = new C演奏記録();
			}
		}
		internal static int tランク値を計算して返す( C演奏記録 part )
		{
			if( part.b演奏にMIDI入力を使用した || part.b演奏にキーボードを使用した || part.b演奏にジョイパッドを使用した || part.b演奏にマウスを使用した )	// 2010.9.11
			{
				int nTotal = ( ( ( part.nPerfect数 + part.nGreat数 ) + part.nGood数 ) + part.nPoor数 ) + part.nMiss数;
				return tランク値を計算して返す( nTotal, part.nPerfect数, part.nGreat数, part.nGood数, part.nPoor数, part.nMiss数 );
			}
			return 99;
		}
		internal static int tランク値を計算して返す( int nTotal, int nPerfect, int nGreat, int nGood, int nPoor, int nMiss )
		{
			if( nTotal <= 0 )
				return 99;

			int num = 6;
			int num2 = nTotal - ( ( ( ( nPerfect + nGreat ) + nGood ) + nPoor ) + nMiss );
			if( nTotal == num2 )
			{
				return 0;
			}
			double num3 = ( (double) ( nPerfect + nGreat ) ) / ( (double) ( nTotal - num2 ) );
			if( num3 == 1.0 )
			{
				return 0;
			}
			if( num3 >= 0.95 )
			{
				return 1;
			}
			if( num3 >= 0.9 )
			{
				return 2;
			}
			if( num3 >= 0.85 )
			{
				return 3;
			}
			if( num3 >= 0.8 )
			{
				return 4;
			}
			if( num3 >= 0.7 )
			{
				return 5;
			}
			return 6;
		}
		internal static double tゲーム型スキルを計算して返す( int nLevel, int nTotal, int nPerfect, int nCombo )
		{
			if( ( nTotal == 0 ) || ( ( nPerfect == 0 ) && ( nCombo == 0 ) ) )
				return 0.0;

			return ( ( nLevel * ( ( ( nPerfect * 0.8 ) + ( nCombo * 0.2 ) ) / ( (double) nTotal ) ) ) / 2.0 );
		}
		internal static double t演奏型スキルを計算して返す( int nTotal, int nPerfect, int nGreat, int nGood, int nPoor, int nMiss )
		{
			if( nTotal == 0 )
				return 0.0;

			int num = nTotal - ( ( ( ( nPerfect + nGreat ) + nGood ) + nPoor ) + nMiss );
			double y = ( ( ( ( ( ( ( nPerfect * 1.0 ) + ( nGreat * 0.8 ) ) + ( nGood * 0.5 ) ) + ( nPoor * 0.2 ) ) + ( nMiss * 0.0 ) ) + ( num * 0.0 ) ) * 100.0 ) / ( (double) nTotal );
			return ( 100.0 * ( ( Math.Pow( 1.03, y ) - 1.0 ) / ( Math.Pow( 1.03, 100.0 ) - 1.0 ) ) );
		}
		internal static string t演奏セクションのMD5を求めて返す( C演奏記録 cc )
		{
			StringBuilder builder = new StringBuilder();
			builder.Append( cc.nスコア.ToString() );
			builder.Append( cc.dbゲーム型スキル値.ToString( ".000000" ) );
			builder.Append( cc.db演奏型スキル値.ToString( ".000000" ) );
			builder.Append( cc.nPerfect数 );
			builder.Append( cc.nGreat数 );
			builder.Append( cc.nGood数 );
			builder.Append( cc.nPoor数 );
			builder.Append( cc.nMiss数 );
			builder.Append( cc.n最大コンボ数 );
			builder.Append( cc.n全チップ数 );
			for( int i = 0; i < 10; i++ )
				builder.Append( boolToChar( cc.bAutoPlay[ i ] ) );
			builder.Append( boolToChar( cc.bTight ) );
			builder.Append( boolToChar( cc.bSudden.Drums ) );
			builder.Append( boolToChar( cc.bSudden.Guitar ) );
			builder.Append( boolToChar( cc.bSudden.Bass ) );
			builder.Append( boolToChar( cc.bHidden.Drums ) );
			builder.Append( boolToChar( cc.bHidden.Guitar ) );
			builder.Append( boolToChar( cc.bHidden.Bass ) );
			builder.Append( boolToChar( cc.bReverse.Drums ) );
			builder.Append( boolToChar( cc.bReverse.Guitar ) );
			builder.Append( boolToChar( cc.bReverse.Bass ) );
			builder.Append( (int) cc.eRandom.Guitar );
			builder.Append( (int) cc.eRandom.Bass );
			builder.Append( boolToChar( cc.bLight.Guitar ) );
			builder.Append( boolToChar( cc.bLight.Bass ) );
			builder.Append( boolToChar( cc.bLeft.Guitar ) );
			builder.Append( boolToChar( cc.bLeft.Bass ) );
			builder.Append( (int) cc.eDark );
			builder.Append( cc.f譜面スクロール速度.Drums.ToString( ".000000" ) );
			builder.Append( cc.f譜面スクロール速度.Guitar.ToString( ".000000" ) );
			builder.Append( cc.f譜面スクロール速度.Bass.ToString( ".000000" ) );
			builder.Append( cc.n演奏速度分子 );
			builder.Append( cc.n演奏速度分母 );
			builder.Append( (int) cc.eHHGroup );
			builder.Append( (int) cc.eFTGroup );
			builder.Append( (int) cc.eCYGroup );
			builder.Append( (int) cc.eHitSoundPriorityHH );
			builder.Append( (int) cc.eHitSoundPriorityFT );
			builder.Append( (int) cc.eHitSoundPriorityCY );
			builder.Append( boolToChar( cc.bGuitar有効 ) );
			builder.Append( boolToChar( cc.bDrums有効 ) );
			builder.Append( boolToChar( cc.bSTAGEFAILED有効 ) );
			builder.Append( (int) cc.eダメージレベル );
			builder.Append( boolToChar( cc.b演奏にキーボードを使用した ) );
			builder.Append( boolToChar( cc.b演奏にMIDI入力を使用した ) );
			builder.Append( boolToChar( cc.b演奏にジョイパッドを使用した ) );
			builder.Append( boolToChar( cc.b演奏にマウスを使用した ) );
			builder.Append( cc.nPerfectになる範囲ms );
			builder.Append( cc.nGreatになる範囲ms );
			builder.Append( cc.nGoodになる範囲ms );
			builder.Append( cc.nPoorになる範囲ms );
			builder.Append( cc.strDTXManiaのバージョン );
			builder.Append( cc.最終更新日時 );

			byte[] bytes = Encoding.GetEncoding( "shift-jis" ).GetBytes( builder.ToString() );
			byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash( bytes );
			StringBuilder builder2 = new StringBuilder( 0x21 );
			foreach( byte num2 in buffer2 )
				builder2.Append( num2.ToString( "x2" ) );

			return builder2.ToString();
		}
		internal static void t更新条件を取得する( out bool bDrumsを更新する, out bool bGuitarを更新する, out bool bBassを更新する )
		{
			bDrumsを更新する = ( CDTXMania.ConfigIni.bDrums有効 && CDTXMania.DTX.bチップがある.Drums ) && !CDTXMania.ConfigIni.bドラムが全部オートプレイである;
			bGuitarを更新する = ( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Guitar ) && !CDTXMania.ConfigIni.bAutoPlay.Guitar;
			bBassを更新する = ( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Bass ) && !CDTXMania.ConfigIni.bAutoPlay.Bass;
		}
		internal static int t総合ランク値を計算して返す( C演奏記録 Drums, C演奏記録 Guitar, C演奏記録 Bass )
		{
			int nPerfect = ( Drums.nPerfect数 + Guitar.nPerfect数 ) + Bass.nPerfect数;
			int nGreat = ( Drums.nGreat数 + Guitar.nGreat数 ) + Bass.nGreat数;
			int nGood = ( Drums.nGood数 + Guitar.nGood数 ) + Bass.nGood数;
			int nPoor = ( Drums.nPoor数 + Guitar.nPoor数 ) + Bass.nPoor数;
			int nMiss = ( Drums.nMiss数 + Guitar.nMiss数 ) + Bass.nMiss数;
			return tランク値を計算して返す( ( ( ( nPerfect + nGreat ) + nGood ) + nPoor ) + nMiss, nPerfect, nGreat, nGood, nPoor, nMiss );
		}


		// その他

		#region [ private ]
		//-----------------
		private bool ONorOFF( char c )
		{
			return ( c != '0' );
		}
		private static char boolToChar( bool b )
		{
			if( !b )
			{
				return '0';
			}
			return '1';
		}
		//-----------------
		#endregion
	}
}
