using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	internal class CConfigIni
	{
		// クラス

		public class CKeyAssign
		{
			public class CKeyAssignPad
			{
				public CConfigIni.CKeyAssign.STKEYASSIGN[] B
				{
					get
					{
						return this.padBD_B;
					}
					set
					{
						this.padBD_B = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] BD
				{
					get
					{
						return this.padBD_B;
					}
					set
					{
						this.padBD_B = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] Cancel
				{
					get
					{
						return this.padFT_Cancel;
					}
					set
					{
						this.padFT_Cancel = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] CY
				{
					get
					{
						return this.padCY_Decide;
					}
					set
					{
						this.padCY_Decide = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] Decide
				{
					get
					{
						return this.padCY_Decide;
					}
					set
					{
						this.padCY_Decide = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] FT
				{
					get
					{
						return this.padFT_Cancel;
					}
					set
					{
						this.padFT_Cancel = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] G
				{
					get
					{
						return this.padSD_G;
					}
					set
					{
						this.padSD_G = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] HH
				{
					get
					{
						return this.padHH_R;
					}
					set
					{
						this.padHH_R = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] HHO
				{
					get
					{
						return this.padHHO;
					}
					set
					{
						this.padHHO = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] HT
				{
					get
					{
						return this.padHT_Pick;
					}
					set
					{
						this.padHT_Pick = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] this[ int index ]
				{
					get
					{
						switch( index )
						{
							case 0:
								return this.padHH_R;

							case 1:
								return this.padSD_G;

							case 2:
								return this.padBD_B;

							case 3:
								return this.padHT_Pick;

							case 4:
								return this.padLT_Wail;

							case 5:
								return this.padFT_Cancel;

							case 6:
								return this.padCY_Decide;

							case 7:
								return this.padHHO;

							case 8:
								return this.padRD;

							case 9:
								return this.padLC;
						}
						throw new IndexOutOfRangeException();
					}
					set
					{
						switch( index )
						{
							case 0:
								this.padHH_R = value;
								return;

							case 1:
								this.padSD_G = value;
								return;

							case 2:
								this.padBD_B = value;
								return;

							case 3:
								this.padHT_Pick = value;
								return;

							case 4:
								this.padLT_Wail = value;
								return;

							case 5:
								this.padFT_Cancel = value;
								return;

							case 6:
								this.padCY_Decide = value;
								return;

							case 7:
								this.padHHO = value;
								return;

							case 8:
								this.padRD = value;
								return;

							case 9:
								this.padLC = value;
								return;
						}
						throw new IndexOutOfRangeException();
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] LC
				{
					get
					{
						return this.padLC;
					}
					set
					{
						this.padLC = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] LT
				{
					get
					{
						return this.padLT_Wail;
					}
					set
					{
						this.padLT_Wail = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] Pick
				{
					get
					{
						return this.padHT_Pick;
					}
					set
					{
						this.padHT_Pick = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] R
				{
					get
					{
						return this.padHH_R;
					}
					set
					{
						this.padHH_R = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] RD
				{
					get
					{
						return this.padRD;
					}
					set
					{
						this.padRD = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] SD
				{
					get
					{
						return this.padSD_G;
					}
					set
					{
						this.padSD_G = value;
					}
				}
				public CConfigIni.CKeyAssign.STKEYASSIGN[] Wail
				{
					get
					{
						return this.padLT_Wail;
					}
					set
					{
						this.padLT_Wail = value;
					}
				}

				#region [ private ]
				//-----------------
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padBD_B;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padCY_Decide;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padFT_Cancel;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padHH_R;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padHHO;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padHT_Pick;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padLC;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padLT_Wail;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padRD;
				private CConfigIni.CKeyAssign.STKEYASSIGN[] padSD_G;
				//-----------------
				#endregion
			}

			[StructLayout( LayoutKind.Sequential )]
			public struct STKEYASSIGN
			{
				public E入力デバイス 入力デバイス;
				public int ID;
				public int コード;
				public STKEYASSIGN( E入力デバイス DeviceType, int nID, int nCode )
				{
					this.入力デバイス = DeviceType;
					this.ID = nID;
					this.コード = nCode;
				}
			}

			public CKeyAssignPad Bass = new CKeyAssignPad();
			public CKeyAssignPad Drums = new CKeyAssignPad();
			public CKeyAssignPad Guitar = new CKeyAssignPad();
			public CKeyAssignPad this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Drums;

						case 1:
							return this.Guitar;

						case 2:
							return this.Bass;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Drums = value;
							return;

						case 1:
							this.Guitar = value;
							return;

						case 2:
							this.Bass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		

		// プロパティ

		public int nBGAlpha;
		public bool bAVI有効;
		public bool bBGA有効;
		public bool bBGM音を発声する;
		public STDGBVALUE<bool> bHidden;
		public STDGBVALUE<bool> bLeft;
		public STDGBVALUE<bool> bLight;
		public bool bLogDTX詳細ログ出力;
		public bool bLog曲検索ログ出力;
		public bool bLog作成解放ログ出力;
		public STDGBVALUE<bool> bReverse;
		public bool bScoreIniを出力する;
		public bool bSTAGEFAILED有効;
		public STDGBVALUE<bool> bSudden;
		public bool bTight;
		public bool bWave再生位置自動調整機能有効;
		public bool bシンバルフリー;
		public bool bストイックモード;
		public bool bドラム打音を発声する;
		public bool bフィルイン有効;
		public bool bランダムセレクトで子BOXを検索対象とする;
		public bool bログ出力;
		public STDGBVALUE<bool> b演奏音を強調する;
		public bool b演奏情報を表示する;
		public bool b歓声を発声する;
		public bool b垂直帰線待ちを行う;
		public bool b選曲リストフォントを斜体にする;
		public bool b選曲リストフォントを太字にする;
		public bool b全画面モード;
		public int nウインドウwidth;				// #23510 2010.10.31 yyagi add
		public int nウインドウheight;				// #23510 2010.10.31 yyagi add
		public Dictionary<int, string> dicJoystick;
		public ECYGroup eCYGroup;
		public Eダークモード eDark;
		public EFTGroup eFTGroup;
		public EHHGroup eHHGroup;
		public E打ち分け時の再生の優先順位 eHitSoundPriorityCY;
		public E打ち分け時の再生の優先順位 eHitSoundPriorityFT;
		public E打ち分け時の再生の優先順位 eHitSoundPriorityHH;
		public STDGBVALUE<Eランダムモード> eRandom;
		public Eダメージレベル eダメージレベル;
        public CKeyAssign KeyAssign;
        public int n非フォーカス時スリープms;       // #23568 2010.11.04 ikanick add
		public int n演奏速度;
		public int n曲が選択されてからプレビュー音が鳴るまでのウェイトms;
		public int n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms;
		public int n自動再生音量;
		public int n手動再生音量;
		public int n選曲リストフォントのサイズdot;
		public STDGBVALUE<int> n表示可能な最小コンボ数;
		public STDGBVALUE<int> n譜面スクロール速度;
		public string strDTXManiaのバージョン;
		public string str曲データ検索パス;
		public string str選曲リストフォント;
		public Eドラムコンボ文字の表示位置 ドラムコンボ文字の表示位置;
		public STDGBVALUE<E判定文字表示位置> 判定文字表示位置;
		public int nハイハット切り捨て下限Velocity;
		public bool bバッファ入力を行う;
		public bool bConfigIniがないかDTXManiaのバージョンが異なる
		{
			get
			{
				return ( !this.bConfigIniが存在している || !CDTXMania.VERSION.Equals( this.strDTXManiaのバージョン ) );
			}
		}
		public bool bDrums有効
		{
			get
			{
				return this._bDrums有効;
			}
			set
			{
				this._bDrums有効 = value;
				if( !this._bGuitar有効 && !this._bDrums有効 )
				{
					this._bGuitar有効 = true;
				}
			}
		}
		public bool bEnterがキー割り当てのどこにも使用されていない
		{
			get
			{
				for( int i = 0; i < 3; i++ )
				{
					for( int j = 0; j < 10; j++ )
					{
						for( int k = 0; k < 0x10; k++ )
						{
							if( ( this.KeyAssign[ i ][ j ][ k ].入力デバイス == E入力デバイス.キーボード ) && ( this.KeyAssign[ i ][ j ][ k ].コード == 0x75 ) )
							{
								return false;
							}
						}
					}
				}
				return true;
			}
		}
		public bool bGuitar有効
		{
			get
			{
				return this._bGuitar有効;
			}
			set
			{
				this._bGuitar有効 = value;
				if( !this._bGuitar有効 && !this._bDrums有効 )
				{
					this._bDrums有効 = true;
				}
			}
		}
		public bool bウィンドウモード
		{
			get
			{
				return !this.b全画面モード;
			}
			set
			{
				this.b全画面モード = !value;
			}
		}
		public bool bギタレボモード
		{
			get
			{
				return ( !this.bDrums有効 && this.bGuitar有効 );
			}
		}
		public bool bドラムが全部オートプレイである
		{
			get
			{
				for( int i = 0; i < 8; i++ )
				{
					if( !this.bAutoPlay[ i ] )
					{
						return false;
					}
				}
				return true;
			}
		}
		public bool b演奏情報を表示しない
		{
			get
			{
				return !this.b演奏情報を表示する;
			}
			set
			{
				this.b演奏情報を表示する = !value;
			}
		}
		public int n背景の透過度
		{
			get
			{
				return this.nBGAlpha;
			}
			set
			{
				if( value < 0 )
				{
					this.nBGAlpha = 0;
				}
				else if( value > 0xff )
				{
					this.nBGAlpha = 0xff;
				}
				else
				{
					this.nBGAlpha = value;
				}
			}
		}

		public STAUTOPLAY bAutoPlay;
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

		public STRANGE nヒット範囲ms;
		[StructLayout( LayoutKind.Sequential )]
		public struct STRANGE
		{
			public int Perfect;
			public int Great;
			public int Good;
			public int Poor;
			public int this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Perfect;

						case 1:
							return this.Great;

						case 2:
							return this.Good;

						case 3:
							return this.Poor;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Perfect = value;
							return;

						case 1:
							this.Great = value;
							return;

						case 2:
							this.Good = value;
							return;

						case 3:
							this.Poor = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		

		// コンストラクタ

		public CConfigIni()
		{
			this.strDTXManiaのバージョン = "Unknown";
			this.str曲データ検索パス = @".\";
			this.b全画面モード = false;
			this.b垂直帰線待ちを行う = true;
			this.nウインドウwidth = 640;				// #23510 2010.10.31 yyagi add
            this.nウインドウheight = 480;				// 
            this.n非フォーカス時スリープms = 1;			// #23568 2010.11.04 ikanick add
			this._bGuitar有効 = true;
			this._bDrums有効 = true;
			this.nBGAlpha = 100;
			this.eダメージレベル = Eダメージレベル.普通;
			this.bSTAGEFAILED有効 = true;
			this.bAVI有効 = true;
			this.bBGA有効 = true;
			this.bフィルイン有効 = true;
			this.n曲が選択されてからプレビュー音が鳴るまでのウェイトms = 0x3e8;
			this.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms = 100;
			this.bWave再生位置自動調整機能有効 = true;
			this.bBGM音を発声する = true;
			this.bドラム打音を発声する = true;
			this.b歓声を発声する = true;
			this.bScoreIniを出力する = true;
			this.bランダムセレクトで子BOXを検索対象とする = true;
			STDGBVALUE<bool> stdgbvalue = new STDGBVALUE<bool>();
			stdgbvalue.Drums = true;
			stdgbvalue.Guitar = true;
			stdgbvalue.Bass = true;
			this.b演奏音を強調する = stdgbvalue;
			STDGBVALUE<int> stdgbvalue2 = new STDGBVALUE<int>();
			stdgbvalue2.Drums = 11;
			stdgbvalue2.Guitar = 2;
			stdgbvalue2.Bass = 2;
			this.n表示可能な最小コンボ数 = stdgbvalue2;
			this.str選曲リストフォント = "MS PGothic";
			this.n選曲リストフォントのサイズdot = 20;
			this.b選曲リストフォントを太字にする = true;
			this.n自動再生音量 = 80;
			this.n手動再生音量 = 100;
			this.bログ出力 = true;
			STDGBVALUE<bool> stdgbvalue3 = new STDGBVALUE<bool>();
			stdgbvalue3.Drums = false;
			stdgbvalue3.Guitar = false;
			stdgbvalue3.Bass = false;
			this.bSudden = stdgbvalue3;
			STDGBVALUE<bool> stdgbvalue4 = new STDGBVALUE<bool>();
			stdgbvalue4.Drums = false;
			stdgbvalue4.Guitar = false;
			stdgbvalue4.Bass = false;
			this.bHidden = stdgbvalue4;
			STDGBVALUE<bool> stdgbvalue5 = new STDGBVALUE<bool>();
			stdgbvalue5.Drums = false;
			stdgbvalue5.Guitar = false;
			stdgbvalue5.Bass = false;
			this.bReverse = stdgbvalue5;
			STDGBVALUE<Eランダムモード> stdgbvalue6 = new STDGBVALUE<Eランダムモード>();
			stdgbvalue6.Drums = Eランダムモード.OFF;
			stdgbvalue6.Guitar = Eランダムモード.OFF;
			stdgbvalue6.Bass = Eランダムモード.OFF;
			this.eRandom = stdgbvalue6;
			STDGBVALUE<bool> stdgbvalue7 = new STDGBVALUE<bool>();
			stdgbvalue7.Drums = false;
			stdgbvalue7.Guitar = false;
			stdgbvalue7.Bass = false;
			this.bLight = stdgbvalue7;
			STDGBVALUE<bool> stdgbvalue8 = new STDGBVALUE<bool>();
			stdgbvalue8.Drums = false;
			stdgbvalue8.Guitar = false;
			stdgbvalue8.Bass = false;
			this.bLeft = stdgbvalue8;
			STDGBVALUE<E判定文字表示位置> stdgbvalue9 = new STDGBVALUE<E判定文字表示位置>();
			stdgbvalue9.Drums = E判定文字表示位置.レーン上;
			stdgbvalue9.Guitar = E判定文字表示位置.レーン上;
			stdgbvalue9.Bass = E判定文字表示位置.レーン上;
			this.判定文字表示位置 = stdgbvalue9;
			STDGBVALUE<int> stdgbvalue10 = new STDGBVALUE<int>();
			stdgbvalue10.Drums = 1;
			stdgbvalue10.Guitar = 1;
			stdgbvalue10.Bass = 1;
			this.n譜面スクロール速度 = stdgbvalue10;
			this.n演奏速度 = 20;
			STAUTOPLAY stautoplay = new STAUTOPLAY();
			stautoplay.HH = false;
			stautoplay.SD = false;
			stautoplay.BD = false;
			stautoplay.HT = false;
			stautoplay.LT = false;
			stautoplay.FT = false;
			stautoplay.CY = false;
			stautoplay.LC = false;
			stautoplay.Guitar = true;
			stautoplay.Bass = true;
			this.bAutoPlay = stautoplay;
			STRANGE strange = new STRANGE();
			strange.Perfect = 0x22;
			strange.Great = 0x43;
			strange.Good = 0x54;
			strange.Poor = 0x75;
			this.nヒット範囲ms = strange;
			this.ConfigIniファイル名 = "";
			this.dicJoystick = new Dictionary<int, string>( 10 );
			this.tデフォルトのキーアサインに設定する();
			this.nハイハット切り捨て下限Velocity = 20;
			this.bバッファ入力を行う = true;
		}
		public CConfigIni( string iniファイル名 )
			: this()
		{
			this.t読み込み( iniファイル名 );
		}


		// メソッド

		public void t指定した入力が既にアサイン済みである場合はそれを全削除する( E入力デバイス DeviceType, int nID, int nCode )
		{
			for( int i = 0; i < 3; i++ )
			{
				for( int j = 0; j < 10; j++ )
				{
					for( int k = 0; k < 0x10; k++ )
					{
						if( ( ( this.KeyAssign[ i ][ j ][ k ].入力デバイス == DeviceType ) && ( this.KeyAssign[ i ][ j ][ k ].ID == nID ) ) && ( this.KeyAssign[ i ][ j ][ k ].コード == nCode ) )
						{
							for( int m = k; m < 15; m++ )
							{
								this.KeyAssign[ i ][ j ][ m ] = this.KeyAssign[ i ][ j ][ m + 1 ];
							}
							this.KeyAssign[ i ][ j ][ 15 ].入力デバイス = E入力デバイス.不明;
							this.KeyAssign[ i ][ j ][ 15 ].ID = 0;
							this.KeyAssign[ i ][ j ][ 15 ].コード = 0;
							k--;
						}
					}
				}
			}
		}
		public void t書き出し( string iniファイル名 )
		{
			StreamWriter sw = new StreamWriter( iniファイル名, false, Encoding.GetEncoding( "shift-jis" ) );
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[System]" );
			sw.WriteLine();
			sw.WriteLine( "; リリースバージョン" );
			sw.WriteLine( "Version={0}", CDTXMania.VERSION );
			sw.WriteLine();
			sw.WriteLine( "; 演奏データの格納されているフォルダへのパス。" );
			sw.WriteLine( @"; セミコロン(;)で区切ることにより複数のパスを指定できます。（例: d:\DTXFiles1\;e:\DTXFiles2\）" );
			sw.WriteLine( "DTXPath={0}", this.str曲データ検索パス );
			sw.WriteLine();
			sw.WriteLine( "; 画面モード(0:ウィンドウ, 1:全画面)" );
			sw.WriteLine( "FullScreen={0}", this.b全画面モード ? 1 : 0 );
            sw.WriteLine();

			sw.WriteLine("; ウインドウモード時の画面幅");				// #23510 2010.10.31 yyagi add
			sw.WriteLine("WindowWidth={0}", this.nウインドウwidth);		//
			sw.WriteLine();												//
			sw.WriteLine("; ウインドウモード時の画面高さ");				//
			sw.WriteLine("WindowHeight={0}", this.nウインドウheight);	//
			sw.WriteLine();												//

			sw.WriteLine("; 垂直帰線同期(0:OFF,1:ON)");
			sw.WriteLine( "VSyncWait={0}", this.b垂直帰線待ちを行う ? 1 : 0 );
            sw.WriteLine();

            sw.WriteLine("; 非フォーカス時のsleep値[ms]");	    			    // #23568 2011.11.04 ikanick add
            sw.WriteLine("BackSleep={0}", this.n非フォーカス時スリープms);		// そのまま引用（苦笑）
            sw.WriteLine();											        	//

			sw.WriteLine( "; ギター/ベース有効(0:OFF,1:ON)" );
			sw.WriteLine( "Guitar={0}", this.bGuitar有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラム有効(0:OFF,1:ON)" );
			sw.WriteLine( "Drums={0}", this.bDrums有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 背景画像の半透明割合(0:透明～255:不透明)" );
			sw.WriteLine( "BGAlpha={0}", this.nBGAlpha );
			sw.WriteLine();
			sw.WriteLine( "; Missヒット時のゲージ減少割合(0:少, 1:普通, 2:大)" );
			sw.WriteLine( "DamageLevel={0}", (int) this.eダメージレベル );
			sw.WriteLine();
			sw.WriteLine( "; LC/HHC/HHO 打ち分けモード(0:LC|HHC|HHO, 1:LC&(HHC|HHO), 2:LC&HHC&HHO)" );
			sw.WriteLine( "HHGroup={0}", (int) this.eHHGroup );
			sw.WriteLine();
			sw.WriteLine( "; LT/FT 打ち分けモード(0:LT|FT, 1:LT&FT)" );
			sw.WriteLine( "FTGroup={0}", (int) this.eFTGroup );
			sw.WriteLine();
			sw.WriteLine( "; CY/RD 打ち分けモード(0:CY|RD, 1:CY&RD)" );
			sw.WriteLine( "CYGroup={0}", (int) this.eCYGroup );
			sw.WriteLine();
			sw.WriteLine( "; 打ち分け時の再生音の優先順位(HHGroup)(0:Chip>Pad, 1:Pad>Chip)" );
			sw.WriteLine( "HitSoundPriorityHH={0}", (int) this.eHitSoundPriorityHH );
			sw.WriteLine();
			sw.WriteLine( "; 打ち分け時の再生音の優先順位(FTGroup)(0:Chip>Pad, 1:Pad>Chip)" );
			sw.WriteLine( "HitSoundPriorityFT={0}", (int) this.eHitSoundPriorityFT );
			sw.WriteLine();
			sw.WriteLine( "; 打ち分け時の再生音の優先順位(CYGroup)(0:Chip>Pad, 1:Pad>Chip)" );
			sw.WriteLine( "HitSoundPriorityCY={0}", (int) this.eHitSoundPriorityCY );
			sw.WriteLine();
			sw.WriteLine( "; ゲージゼロでSTAGE FAILED (0:OFF, 1:ON)" );
			sw.WriteLine( "StageFailed={0}", this.bSTAGEFAILED有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; AVIの表示(0:OFF, 1:ON)" );
			sw.WriteLine( "AVI={0}", this.bAVI有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; BGAの表示(0:OFF, 1:ON)" );
			sw.WriteLine( "BGA={0}", this.bBGA有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; フィルイン効果(0:OFF, 1:ON)" );
			sw.WriteLine( "FillInEffect={0}", this.bフィルイン有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 曲選択からプレビュー音の再生までのウェイト[ms]" );
			sw.WriteLine( "PreviewSoundWait={0}", this.n曲が選択されてからプレビュー音が鳴るまでのウェイトms );
			sw.WriteLine();
			sw.WriteLine( "; 曲選択からプレビュー画像表示までのウェイト[ms]" );
			sw.WriteLine( "PreviewImageWait={0}", this.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms );
			sw.WriteLine();
			sw.WriteLine( "; Waveの再生位置自動補正(0:OFF, 1:ON)" );
			sw.WriteLine( "AdjustWaves={0}", this.bWave再生位置自動調整機能有効 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; BGM の再生(0:OFF, 1:ON)" );
			sw.WriteLine( "BGMSound={0}", this.bBGM音を発声する ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラム打音の再生(0:OFF, 1:ON)" );
			sw.WriteLine( "HitSound={0}", this.bドラム打音を発声する ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; フィルイン達成時の歓声の再生(0:OFF, 1:ON)" );
			sw.WriteLine( "AudienceSound={0}", this.b歓声を発声する ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 演奏記録（～.score.ini）の出力 (0:OFF, 1:ON)" );
			sw.WriteLine( "SaveScoreIni={0}", this.bScoreIniを出力する ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; RANDOM SELECT で子BOXを検索対象に含める (0:OFF, 1:ON)" );
			sw.WriteLine( "RandomFromSubBox={0}", this.bランダムセレクトで子BOXを検索対象とする ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラム演奏時にドラム音を強調する (0:OFF, 1:ON)" );
			sw.WriteLine( "SoundMonitorDrums={0}", this.b演奏音を強調する.Drums ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギター演奏時にギター音を強調する (0:OFF, 1:ON)" );
			sw.WriteLine( "SoundMonitorGuitar={0}", this.b演奏音を強調する.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベース演奏時にベース音を強調する (0:OFF, 1:ON)" );
			sw.WriteLine( "SoundMonitorBass={0}", this.b演奏音を強調する.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラムの表示可能な最小コンボ数(1～99999)" );
			sw.WriteLine( "MinComboDrums={0}", this.n表示可能な最小コンボ数.Drums );
			sw.WriteLine();
			sw.WriteLine( "; ギターの表示可能な最小コンボ数(1～99999)" );
			sw.WriteLine( "MinComboGuitar={0}", this.n表示可能な最小コンボ数.Guitar );
			sw.WriteLine();
			sw.WriteLine( "; ベースの表示可能な最小コンボ数(1～99999)" );
			sw.WriteLine( "MinComboBass={0}", this.n表示可能な最小コンボ数.Bass );
			sw.WriteLine();
			sw.WriteLine( "; 演奏情報を表示する (0:OFF, 1:ON)" );
			sw.WriteLine( "ShowDebugStatus={0}", this.b演奏情報を表示する ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 選曲リストのフォント名" );
			sw.WriteLine( "SelectListFontName={0}", this.str選曲リストフォント );
			sw.WriteLine();
			sw.WriteLine( "; 選曲リストのフォントのサイズ[dot]" );
			sw.WriteLine( "SelectListFontSize={0}", this.n選曲リストフォントのサイズdot );
			sw.WriteLine();
			sw.WriteLine( "; 選曲リストのフォントを斜体にする (0:OFF, 1:ON)" );
			sw.WriteLine( "SelectListFontItalic={0}", this.b選曲リストフォントを斜体にする ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 選曲リストのフォントを太字にする (0:OFF, 1:ON)" );
			sw.WriteLine( "SelectListFontBold={0}", this.b選曲リストフォントを太字にする ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 打音の音量(0～100%)" );
			sw.WriteLine( "ChipVolume={0}", this.n手動再生音量 );
			sw.WriteLine();
			sw.WriteLine( "; 自動再生音の音量(0～100%)" );
			sw.WriteLine( "AutoChipVolume={0}", this.n自動再生音量 );
			sw.WriteLine();
			sw.WriteLine( "; ストイックモード(0:OFF, 1:ON)" );
			sw.WriteLine( "StoicMode={0}", this.bストイックモード ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; シンバルフリーモード(0:OFF, 1:ON)" );
			sw.WriteLine( "CymbalFree={0}", this.bシンバルフリー ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; バッファ入力モード(0:OFF, 1:ON)" );
			sw.WriteLine( "BufferedInput={0}", this.bバッファ入力を行う ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ハイハット入力切り捨て下限Velocity値(0～127)" );
			sw.WriteLine( "HHVelocityMin={0}", this.nハイハット切り捨て下限Velocity );
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[Log]" );
			sw.WriteLine();
			sw.WriteLine( "; Log出力(0:OFF, 1:ON)" );
			sw.WriteLine( "OutputLog={0}", this.bログ出力 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 曲データ検索に関するLog出力(0:OFF, 1:ON)" );
			sw.WriteLine( "TraceSongSearch={0}", this.bLog曲検索ログ出力 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; 画像やサウンドの作成・解放に関するLog出力(0:OFF, 1:ON)" );
			sw.WriteLine( "TraceCreatedDisposed={0}", this.bLog作成解放ログ出力 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; DTX読み込み詳細に関するLog出力(0:OFF, 1:ON)" );
			sw.WriteLine( "TraceDTXDetails={0}", this.bLogDTX詳細ログ出力 ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[PlayOption]" );
			sw.WriteLine();
			sw.WriteLine( "; DARKモード(0:OFF, 1:HALF, 2:FULL)" );
			sw.WriteLine( "Dark={0}", (int) this.eDark );
			sw.WriteLine();
			sw.WriteLine( "; ドラムSUDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "DrumsSudden={0}", this.bSudden.Drums ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギターSUDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "GuitarSudden={0}", this.bSudden.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベースSUDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "BassSudden={0}", this.bSudden.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラムHIDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "DrumsHidden={0}", this.bHidden.Drums ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギターHIDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "GuitarHidden={0}", this.bHidden.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベースHIDDENモード(0:OFF, 1:ON)" );
			sw.WriteLine( "BassHidden={0}", this.bHidden.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラムREVERSEモード(0:OFF, 1:ON)" );
			sw.WriteLine( "DrumsReverse={0}", this.bReverse.Drums ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギターREVERSEモード(0:OFF, 1:ON)" );
			sw.WriteLine( "GuitarReverse={0}", this.bReverse.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベースREVERSEモード(0:OFF, 1:ON)" );
			sw.WriteLine( "BassReverse={0}", this.bReverse.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギターRANDOMモード(0:OFF, 1:Random, 2:SuperRandom, 3:HyperRandom)" );
			sw.WriteLine( "GuitarRandom={0}", (int) this.eRandom.Guitar );
			sw.WriteLine();
			sw.WriteLine( "; ベースRANDOMモード(0:OFF, 1:Random, 2:SuperRandom, 3:HyperRandom)" );
			sw.WriteLine( "BassRandom={0}", (int) this.eRandom.Bass );
			sw.WriteLine();
			sw.WriteLine( "; ギターLIGHTモード(0:OFF, 1:ON)" );
			sw.WriteLine( "GuitarLight={0}", this.bLight.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベースLIGHTモード(0:OFF, 1:ON)" );
			sw.WriteLine( "BassLight={0}", this.bLight.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ギターLEFTモード(0:OFF, 1:ON)" );
			sw.WriteLine( "GuitarLeft={0}", this.bLeft.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ベースLEFTモード(0:OFF, 1:ON)" );
			sw.WriteLine( "BassLeft={0}", this.bLeft.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; ドラム判定文字表示位置(0:レーン上,1:判定ライン上,2:表示OFF)" );
			sw.WriteLine( "DrumsPosition={0}", (int) this.判定文字表示位置.Drums );
			sw.WriteLine();
			sw.WriteLine( "; ギター判定文字表示位置(0:レーン上,1:判定ライン横,2:表示OFF)" );
			sw.WriteLine( "GuitarPosition={0}", (int) this.判定文字表示位置.Guitar );
			sw.WriteLine();
			sw.WriteLine( "; ベース判定文字表示位置(0:レーン上,1:判定ライン横,2:表示OFF)" );
			sw.WriteLine( "BassPosition={0}", (int) this.判定文字表示位置.Bass );
			sw.WriteLine();
			sw.WriteLine( "; ドラム譜面スクロール速度(0:x0.5, 1:x1.0, 2:x1.5,…,1999:x1000.0)" );
			sw.WriteLine( "DrumsScrollSpeed={0}", this.n譜面スクロール速度.Drums );
			sw.WriteLine();
			sw.WriteLine( "; ギター譜面スクロール速度(0:x0.5, 1:x1.0, 2:x1.5,…,1999:x1000.0)" );
			sw.WriteLine( "GuitarScrollSpeed={0}", this.n譜面スクロール速度.Guitar );
			sw.WriteLine();
			sw.WriteLine( "; ベース譜面スクロール速度(0:x0.5, 1:x1.0, 2:x1.5,…,1999:x1000.0)" );
			sw.WriteLine( "BassScrollSpeed={0}", this.n譜面スクロール速度.Bass );
			sw.WriteLine();
			sw.WriteLine( "; 演奏速度(5～40)(→x5/20～x40/20)" );
			sw.WriteLine( "PlaySpeed={0}", this.n演奏速度 );
			sw.WriteLine();
			sw.WriteLine( "; ドラムCOMBO文字表示位置(0:左, 1:中, 2:右, 3:OFF)" );
			sw.WriteLine( "ComboPosition={0}", (int) this.ドラムコンボ文字の表示位置 );
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[AutoPlay]" );
			sw.WriteLine();
			sw.WriteLine( "; 自動演奏(0:OFF, 1:ON)" );
			sw.WriteLine();
			sw.WriteLine( "; Drums" );
			sw.WriteLine( "LC={0}", this.bAutoPlay.LC ? 1 : 0 );
			sw.WriteLine( "HH={0}", this.bAutoPlay.HH ? 1 : 0 );
			sw.WriteLine( "SD={0}", this.bAutoPlay.SD ? 1 : 0 );
			sw.WriteLine( "BD={0}", this.bAutoPlay.BD ? 1 : 0 );
			sw.WriteLine( "HT={0}", this.bAutoPlay.HT ? 1 : 0 );
			sw.WriteLine( "LT={0}", this.bAutoPlay.LT ? 1 : 0 );
			sw.WriteLine( "FT={0}", this.bAutoPlay.FT ? 1 : 0 );
			sw.WriteLine( "CY={0}", this.bAutoPlay.CY ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; Guitar" );
			sw.WriteLine( "Guitar={0}", this.bAutoPlay.Guitar ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( "; Bass" );
			sw.WriteLine( "Bass={0}", this.bAutoPlay.Bass ? 1 : 0 );
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[HitRange]" );
			sw.WriteLine();
			sw.WriteLine( "; Perfect～Poor とみなされる範囲[ms]" );
			sw.WriteLine( "Perfect={0}", this.nヒット範囲ms.Perfect );
			sw.WriteLine( "Great={0}", this.nヒット範囲ms.Great );
			sw.WriteLine( "Good={0}", this.nヒット範囲ms.Good );
			sw.WriteLine( "Poor={0}", this.nヒット範囲ms.Poor );
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "[GUID]" );
			sw.WriteLine();
			foreach( KeyValuePair<int, string> pair in this.dicJoystick )
			{
				sw.WriteLine( "JoystickID={0},{1}", pair.Key, pair.Value );
			}
			sw.WriteLine();
			sw.WriteLine( ";-------------------" );
			sw.WriteLine( "; キーアサイン" );
			sw.WriteLine( ";   項　目：Keyboard → 'K'＋'0'＋キーコード(10進数)" );
			sw.WriteLine( ";           Mouse    → 'N'＋'0'＋ボタン番号(0～7)" );
			sw.WriteLine( ";           MIDI In  → 'M'＋デバイス番号1桁(0～9)＋ノート番号(10進数)" );
			sw.WriteLine( ";           Joystick → 'J'＋デバイス番号1桁(0～9)＋ 0 ...... Ｘ減少(左)ボタン" );
			sw.WriteLine( ";                                                    1 ...... Ｘ増加(右)ボタン" );
			sw.WriteLine( ";                                                    2 ...... Ｙ減少(上)ボタン" );
			sw.WriteLine( ";                                                    3 ...... Ｙ増加(下)ボタン" );
			sw.WriteLine( ";                                                    4 ...... Ｚ減少(前)ボタン" );
			sw.WriteLine( ";                                                    5 ...... Ｚ増加(後)ボタン" );
			sw.WriteLine( ";                                                    6～133.. ボタン1～128" );
			sw.WriteLine( ";           これらの項目を 16 個まで指定可能(',' で区切って記述）。" );
			sw.WriteLine( ";" );
			sw.WriteLine( ";   表記例：HH=K044,M042,J16" );
			sw.WriteLine( ";           → HiHat を Keyboard の 44 ('Z'), MidiIn#0 の 42, JoyPad#1 の 6(ボタン1) に割当て" );
			sw.WriteLine( ";" );
			sw.WriteLine( ";   ※Joystick のデバイス番号とデバイスとの関係は [GUID] セクションに記してあるものが有効。" );
			sw.WriteLine( ";" );
			sw.WriteLine();
			sw.WriteLine( "[DrumsKeyAssign]" );
			sw.WriteLine();
			sw.Write( "HH=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.HH );
			sw.WriteLine();
			sw.Write( "SD=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.SD );
			sw.WriteLine();
			sw.Write( "BD=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.BD );
			sw.WriteLine();
			sw.Write( "HT=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.HT );
			sw.WriteLine();
			sw.Write( "LT=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.LT );
			sw.WriteLine();
			sw.Write( "FT=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.FT );
			sw.WriteLine();
			sw.Write( "CY=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.CY );
			sw.WriteLine();
			sw.Write( "HO=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.HHO );
			sw.WriteLine();
			sw.Write( "RD=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.RD );
			sw.WriteLine();
			sw.Write( "LC=" );
			this.tキーの書き出し( sw, this.KeyAssign.Drums.LC );
			sw.WriteLine();
			sw.WriteLine();
			sw.WriteLine( "[GuitarKeyAssign]" );
			sw.WriteLine();
			sw.Write( "R=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.R );
			sw.WriteLine();
			sw.Write( "G=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.G );
			sw.WriteLine();
			sw.Write( "B=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.B );
			sw.WriteLine();
			sw.Write( "Pick=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.Pick );
			sw.WriteLine();
			sw.Write( "Wail=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.Wail );
			sw.WriteLine();
			sw.Write( "Decide=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.Decide );
			sw.WriteLine();
			sw.Write( "Cancel=" );
			this.tキーの書き出し( sw, this.KeyAssign.Guitar.Cancel );
			sw.WriteLine();
			sw.WriteLine();
			sw.WriteLine( "[BassKeyAssign]" );
			sw.WriteLine();
			sw.Write( "R=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.R );
			sw.WriteLine();
			sw.Write( "G=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.G );
			sw.WriteLine();
			sw.Write( "B=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.B );
			sw.WriteLine();
			sw.Write( "Pick=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.Pick );
			sw.WriteLine();
			sw.Write( "Wail=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.Wail );
			sw.WriteLine();
			sw.Write( "Decide=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.Decide );
			sw.WriteLine();
			sw.Write( "Cancel=" );
			this.tキーの書き出し( sw, this.KeyAssign.Bass.Cancel );
			sw.WriteLine();
			sw.WriteLine();
			sw.Close();
		}
		public void t読み込み( string iniファイル名 )
		{
			this.ConfigIniファイル名 = iniファイル名;
			this.bConfigIniが存在している = File.Exists( this.ConfigIniファイル名 );
			if( this.bConfigIniが存在している )
			{
				string str;
				this.tキーアサインを全部クリアする();
				Eセクション種別 unknown = Eセクション種別.Unknown;
				StreamReader reader = new StreamReader( this.ConfigIniファイル名, Encoding.GetEncoding( "shift-jis" ) );
				while( ( str = reader.ReadLine() ) != null )
				{
					str = str.Replace( '\t', ' ' ).TrimStart( new char[] { '\t', ' ' } );
					if( ( str.Length != 0 ) && ( str[ 0 ] != ';' ) )
					{
						try
						{
							string str3;
							string str4;
							if( str[ 0 ] == '[' )
							{
								#region [ セクションの変更 ]
								//-----------------------------
								StringBuilder builder = new StringBuilder( 0x20 );
								int num = 1;
								while( ( num < str.Length ) && ( str[ num ] != ']' ) )
								{
									builder.Append( str[ num++ ] );
								}
								string str2 = builder.ToString();
								if( str2.Equals( "System" ) )
								{
									unknown = Eセクション種別.System;
								}
								else if( str2.Equals( "Log" ) )
								{
									unknown = Eセクション種別.Log;
								}
								else if( str2.Equals( "PlayOption" ) )
								{
									unknown = Eセクション種別.PlayOption;
								}
								else if( str2.Equals( "AutoPlay" ) )
								{
									unknown = Eセクション種別.AutoPlay;
								}
								else if( str2.Equals( "HitRange" ) )
								{
									unknown = Eセクション種別.HitRange;
								}
								else if( str2.Equals( "GUID" ) )
								{
									unknown = Eセクション種別.GUID;
								}
								else if( str2.Equals( "DrumsKeyAssign" ) )
								{
									unknown = Eセクション種別.DrumsKeyAssign;
								}
								else if( str2.Equals( "GuitarKeyAssign" ) )
								{
									unknown = Eセクション種別.GuitarKeyAssign;
								}
								else if( str2.Equals( "BassKeyAssign" ) )
								{
									unknown = Eセクション種別.BassKeyAssign;
								}
								else
								{
									unknown = Eセクション種別.Unknown;
								}
								//-----------------------------
								#endregion
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
										#region [ [System] ]
										//-----------------------------
										case Eセクション種別.System:
											{
												if( str3.Equals( "Version" ) )
												{
													this.strDTXManiaのバージョン = str4;
												}
												else if( str3.Equals( "DTXPath" ) )
												{
													this.str曲データ検索パス = str4;
												}
												else if( str3.Equals( "FullScreen" ) )
												{
													this.b全画面モード = C変換.bONorOFF( str4[ 0 ] );
												}
												else if (str3.Equals("WindowWidth"))		// #23510 2010.10.31 yyagi add
												{
													this.nウインドウwidth = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 1, 65535, this.nウインドウwidth);
													if (this.nウインドウwidth <= 0)
													{
														this.nウインドウwidth = 640;
													}
												}
												else if (str3.Equals("WindowHeight"))		// #23510 2010.10.31 yyagi add
												{
													this.nウインドウheight = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 1, 65535, this.nウインドウheight);
													if (this.nウインドウheight <= 0)
													{
														this.nウインドウheight = 480;
													}
												}
												else if (str3.Equals("VSyncWait"))
												{
													this.b垂直帰線待ちを行う = C変換.bONorOFF( str4[ 0 ] );
                                                }
                                                else if (str3.Equals("BackSleep"))		// #23568 2010.11.04 ikanick add
                                                {
                                                    this.n非フォーカス時スリープms = C変換.n値を文字列から取得して範囲内に丸めて返す(str4, 0, 50, this.n非フォーカス時スリープms);
                                                }
												else if( str3.Equals( "Guitar" ) )
												{
													this.bGuitar有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "Drums" ) )
												{
													this.bDrums有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BGAlpha" ) )
												{
													this.n背景の透過度 = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0xff, this.n背景の透過度 );
												}
												else if( str3.Equals( "DamageLevel" ) )
												{
													this.eダメージレベル = (Eダメージレベル) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.eダメージレベル );
												}
												else if( str3.Equals( "HHGroup" ) )
												{
													this.eHHGroup = (EHHGroup) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 3, (int) this.eHHGroup );
												}
												else if( str3.Equals( "FTGroup" ) )
												{
													this.eFTGroup = (EFTGroup) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.eFTGroup );
												}
												else if( str3.Equals( "CYGroup" ) )
												{
													this.eCYGroup = (ECYGroup) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.eCYGroup );
												}
												else if( str3.Equals( "HitSoundPriorityHH" ) )
												{
													this.eHitSoundPriorityHH = (E打ち分け時の再生の優先順位) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 1, (int) this.eHitSoundPriorityHH );
												}
												else if( str3.Equals( "HitSoundPriorityFT" ) )
												{
													this.eHitSoundPriorityFT = (E打ち分け時の再生の優先順位) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 1, (int) this.eHitSoundPriorityFT );
												}
												else if( str3.Equals( "HitSoundPriorityCY" ) )
												{
													this.eHitSoundPriorityCY = (E打ち分け時の再生の優先順位) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 1, (int) this.eHitSoundPriorityCY );
												}
												else if( str3.Equals( "StageFailed" ) )
												{
													this.bSTAGEFAILED有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "AVI" ) )
												{
													this.bAVI有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BGA" ) )
												{
													this.bBGA有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "FillInEffect" ) )
												{
													this.bフィルイン有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "PreviewSoundWait" ) )
												{
													this.n曲が選択されてからプレビュー音が鳴るまでのウェイトms = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, this.n曲が選択されてからプレビュー音が鳴るまでのウェイトms );
												}
												else if( str3.Equals( "PreviewImageWait" ) )
												{
													this.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x5f5e0ff, this.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms );
												}
												else if( str3.Equals( "AdjustWaves" ) )
												{
													this.bWave再生位置自動調整機能有効 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BGMSound" ) )
												{
													this.bBGM音を発声する = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "HitSound" ) )
												{
													this.bドラム打音を発声する = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "AudienceSound" ) )
												{
													this.b歓声を発声する = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SaveScoreIni" ) )
												{
													this.bScoreIniを出力する = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "RandomFromSubBox" ) )
												{
													this.bランダムセレクトで子BOXを検索対象とする = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SoundMonitorDrums" ) )
												{
													this.b演奏音を強調する.Drums = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SoundMonitorGuitar" ) )
												{
													this.b演奏音を強調する.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SoundMonitorBass" ) )
												{
													this.b演奏音を強調する.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "MinComboDrums" ) )
												{
													this.n表示可能な最小コンボ数.Drums = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 1, 0x1869f, this.n表示可能な最小コンボ数.Drums );
												}
												else if( str3.Equals( "MinComboGuitar" ) )
												{
													this.n表示可能な最小コンボ数.Guitar = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 1, 0x1869f, this.n表示可能な最小コンボ数.Guitar );
												}
												else if( str3.Equals( "MinComboBass" ) )
												{
													this.n表示可能な最小コンボ数.Bass = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 1, 0x1869f, this.n表示可能な最小コンボ数.Bass );
												}
												else if( str3.Equals( "ShowDebugStatus" ) )
												{
													this.b演奏情報を表示する = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SelectListFontName" ) )
												{
													this.str選曲リストフォント = str4;
												}
												else if( str3.Equals( "SelectListFontSize" ) )
												{
													this.n選曲リストフォントのサイズdot = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 1, 0x3e7, this.n選曲リストフォントのサイズdot );
												}
												else if( str3.Equals( "SelectListFontItalic" ) )
												{
													this.b選曲リストフォントを斜体にする = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SelectListFontBold" ) )
												{
													this.b選曲リストフォントを太字にする = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "ChipVolume" ) )
												{
													this.n手動再生音量 = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 100, this.n手動再生音量 );
												}
												else if( str3.Equals( "AutoChipVolume" ) )
												{
													this.n自動再生音量 = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 100, this.n自動再生音量 );
												}
												else if( str3.Equals( "StoicMode" ) )
												{
													this.bストイックモード = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "CymbalFree" ) )
												{
													this.bシンバルフリー = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BufferedInput" ) )
												{
													this.bバッファ入力を行う = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "HHVelocityMin" ) )
												{
													this.nハイハット切り捨て下限Velocity = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 127, this.nハイハット切り捨て下限Velocity );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [Log] ]
										//-----------------------------
										case Eセクション種別.Log:
											{
												if( str3.Equals( "OutputLog" ) )
												{
													this.bログ出力 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "TraceCreatedDisposed" ) )
												{
													this.bLog作成解放ログ出力 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "TraceDTXDetails" ) )
												{
													this.bLogDTX詳細ログ出力 = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "TraceSongSearch" ) )
												{
													this.bLog曲検索ログ出力 = C変換.bONorOFF( str4[ 0 ] );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [PlayOption] ]
										//-----------------------------
										case Eセクション種別.PlayOption:
											{
												if( str3.Equals( "Dark" ) )
												{
													this.eDark = (Eダークモード) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.eDark );
												}
												else if( str3.Equals( "DrumsTight" ) )
												{
													this.bTight = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "DrumsSudden" ) )
												{
													this.bSudden.Drums = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "GuitarSudden" ) )
												{
													this.bSudden.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BassSudden" ) )
												{
													this.bSudden.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "DrumsHidden" ) )
												{
													this.bHidden.Drums = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "GuitarHidden" ) )
												{
													this.bHidden.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BassHidden" ) )
												{
													this.bHidden.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "DrumsReverse" ) )
												{
													this.bReverse.Drums = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "GuitarReverse" ) )
												{
													this.bReverse.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BassReverse" ) )
												{
													this.bReverse.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "GuitarRandom" ) )
												{
													this.eRandom.Guitar = (Eランダムモード) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 3, (int) this.eRandom.Guitar );
												}
												else if( str3.Equals( "BassRandom" ) )
												{
													this.eRandom.Bass = (Eランダムモード) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 3, (int) this.eRandom.Bass );
												}
												else if( str3.Equals( "GuitarLight" ) )
												{
													this.bLight.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BassLight" ) )
												{
													this.bLight.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "GuitarLeft" ) )
												{
													this.bLeft.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BassLeft" ) )
												{
													this.bLeft.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "DrumsPosition" ) )
												{
													this.判定文字表示位置.Drums = (E判定文字表示位置) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.判定文字表示位置.Drums );
												}
												else if( str3.Equals( "GuitarPosition" ) )
												{
													this.判定文字表示位置.Guitar = (E判定文字表示位置) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.判定文字表示位置.Guitar );
												}
												else if( str3.Equals( "BassPosition" ) )
												{
													this.判定文字表示位置.Bass = (E判定文字表示位置) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 2, (int) this.判定文字表示位置.Bass );
												}
												else if( str3.Equals( "DrumsScrollSpeed" ) )
												{
													this.n譜面スクロール速度.Drums = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x7cf, this.n譜面スクロール速度.Drums );
												}
												else if( str3.Equals( "GuitarScrollSpeed" ) )
												{
													this.n譜面スクロール速度.Guitar = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x7cf, this.n譜面スクロール速度.Guitar );
												}
												else if( str3.Equals( "BassScrollSpeed" ) )
												{
													this.n譜面スクロール速度.Bass = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x7cf, this.n譜面スクロール速度.Bass );
												}
												else if( str3.Equals( "PlaySpeed" ) )
												{
													this.n演奏速度 = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 5, 40, this.n演奏速度 );
												}
												else if( str3.Equals( "ComboPosition" ) )
												{
													this.ドラムコンボ文字の表示位置 = (Eドラムコンボ文字の表示位置) C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 3, (int) this.ドラムコンボ文字の表示位置 );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [AutoPlay] ]
										//-----------------------------
										case Eセクション種別.AutoPlay:
											{
												if( str3.Equals( "LC" ) )
												{
													this.bAutoPlay.LC = C変換.bONorOFF( str4[ 0 ] );
												}
												if( str3.Equals( "HH" ) )
												{
													this.bAutoPlay.HH = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "SD" ) )
												{
													this.bAutoPlay.SD = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "BD" ) )
												{
													this.bAutoPlay.BD = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "HT" ) )
												{
													this.bAutoPlay.HT = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "LT" ) )
												{
													this.bAutoPlay.LT = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "FT" ) )
												{
													this.bAutoPlay.FT = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "CY" ) )
												{
													this.bAutoPlay.CY = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "Guitar" ) )
												{
													this.bAutoPlay.Guitar = C変換.bONorOFF( str4[ 0 ] );
												}
												else if( str3.Equals( "Bass" ) )
												{
													this.bAutoPlay.Bass = C変換.bONorOFF( str4[ 0 ] );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [HitRange] ]
										//-----------------------------
										case Eセクション種別.HitRange:
											{
												if( str3.Equals( "Perfect" ) )
												{
													this.nヒット範囲ms.Perfect = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x3e7, this.nヒット範囲ms.Perfect );
												}
												else if( str3.Equals( "Great" ) )
												{
													this.nヒット範囲ms.Great = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x3e7, this.nヒット範囲ms.Great );
												}
												else if( str3.Equals( "Good" ) )
												{
													this.nヒット範囲ms.Good = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x3e7, this.nヒット範囲ms.Good );
												}
												else if( str3.Equals( "Poor" ) )
												{
													this.nヒット範囲ms.Poor = C変換.n値を文字列から取得して範囲内に丸めて返す( str4, 0, 0x3e7, this.nヒット範囲ms.Poor );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [GUID] ]
										//-----------------------------
										case Eセクション種別.GUID:
											{
												if( str3.Equals( "JoystickID" ) )
												{
													this.tJoystickIDの取得( str4 );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [DrumsKeyAssign] ]
										//-----------------------------
										case Eセクション種別.DrumsKeyAssign:
											{
												if( str3.Equals( "HH" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.HH );
												}
												else if( str3.Equals( "SD" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.SD );
												}
												else if( str3.Equals( "BD" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.BD );
												}
												else if( str3.Equals( "HT" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.HT );
												}
												else if( str3.Equals( "LT" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.LT );
												}
												else if( str3.Equals( "FT" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.FT );
												}
												else if( str3.Equals( "CY" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.CY );
												}
												else if( str3.Equals( "HO" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.HHO );
												}
												else if( str3.Equals( "RD" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.RD );
												}
												else if( str3.Equals( "LC" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Drums.LC );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ GuitarKeyAssign ]
										//-----------------------------
										case Eセクション種別.GuitarKeyAssign:
											{
												if( str3.Equals( "R" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.R );
												}
												else if( str3.Equals( "G" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.G );
												}
												else if( str3.Equals( "B" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.B );
												}
												else if( str3.Equals( "Pick" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.Pick );
												}
												else if( str3.Equals( "Wail" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.Wail );
												}
												else if( str3.Equals( "Decide" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.Decide );
												}
												else if( str3.Equals( "Cancel" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Guitar.Cancel );
												}
												continue;
											}
										//-----------------------------
										#endregion

										#region [ [BassKeyAssign ]
										//-----------------------------
										case Eセクション種別.BassKeyAssign:
											{
												if( str3.Equals( "R" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.R );
												}
												else if( str3.Equals( "G" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.G );
												}
												else if( str3.Equals( "B" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.B );
												}
												else if( str3.Equals( "Pick" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.Pick );
												}
												else if( str3.Equals( "Wail" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.Wail );
												}
												else if( str3.Equals( "Decide" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.Decide );
												}
												else if( str3.Equals( "Cancel" ) )
												{
													this.tキーの読み出しと設定( str4, this.KeyAssign.Bass.Cancel );
												}
												continue;
											}
										//-----------------------------
										#endregion
									}
								}
							}
							continue;
						}
						catch( Exception exception )
						{
							Trace.TraceError( exception.Message );
							continue;
						}
					}
				}
				CDTXVersion version = new CDTXVersion( this.strDTXManiaのバージョン );
				if( version.n整数部 <= 0x45 )
				{
					this.tデフォルトのキーアサインに設定する();
				}
			}
		}


		// その他

		#region [ private ]
		//-----------------
		private enum Eセクション種別
		{
			Unknown,
			System,
			Log,
			PlayOption,
			AutoPlay,
			HitRange,
			GUID,
			DrumsKeyAssign,
			GuitarKeyAssign,
			BassKeyAssign
		}

		private bool _bDrums有効;
		private bool _bGuitar有効;
		private bool bConfigIniが存在している;
		private string ConfigIniファイル名;

		private void tJoystickIDの取得( string strキー記述 )
		{
			string[] strArray = strキー記述.Split( new char[] { ',' } );
			if( strArray.Length >= 2 )
			{
				int result = 0;
				if( ( int.TryParse( strArray[ 0 ], out result ) && ( result >= 0 ) ) && ( result <= 9 ) )
				{
					if( this.dicJoystick.ContainsKey( result ) )
					{
						this.dicJoystick.Remove( result );
					}
					this.dicJoystick.Add( result, strArray[ 1 ] );
				}
			}
		}
		private void tキーアサインを全部クリアする()
		{
			this.KeyAssign = new CKeyAssign();
			for( int i = 0; i < 3; i++ )
			{
				for( int j = 0; j < 10; j++ )
				{
					this.KeyAssign[ i ][ j ] = new CKeyAssign.STKEYASSIGN[ 0x10 ];
					for( int k = 0; k < 0x10; k++ )
					{
						this.KeyAssign[ i ][ j ][ k ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
					}
				}
			}
		}
		private void tキーの書き出し( StreamWriter sw, CKeyAssign.STKEYASSIGN[] assign )
		{
			bool flag = true;
			for( int i = 0; i < 0x10; i++ )
			{
				if( assign[ i ].入力デバイス == E入力デバイス.不明 )
				{
					continue;
				}
				if( !flag )
				{
					sw.Write( ',' );
				}
				flag = false;
				switch( assign[ i ].入力デバイス )
				{
					case E入力デバイス.キーボード:
						sw.Write( 'K' );
						break;

					case E入力デバイス.MIDI入力:
						sw.Write( 'M' );
						break;

					case E入力デバイス.ジョイパッド:
						sw.Write( 'J' );
						break;

					case E入力デバイス.マウス:
						sw.Write( 'N' );
						break;
				}
				sw.Write( "{0}{1}", assign[ i ].ID, assign[ i ].コード );
			}
		}
		private void tキーの読み出しと設定( string strキー記述, CKeyAssign.STKEYASSIGN[] assign )
		{
			string[] strArray = strキー記述.Split( new char[] { ',' } );
			for( int i = 0; ( i < strArray.Length ) && ( i < 0x10 ); i++ )
			{
				E入力デバイス e入力デバイス;
				int num2;
				int num3;
				string str = strArray[ i ].Trim().ToUpper();
				if( str.Length >= 3 )
				{
					e入力デバイス = E入力デバイス.不明;
					switch( str[ 0 ] )
					{
						case 'J':
							e入力デバイス = E入力デバイス.ジョイパッド;
							goto Label_0071;

						case 'K':
							e入力デバイス = E入力デバイス.キーボード;
							goto Label_0071;

						case 'L':
							{
								continue;
							}
						case 'M':
							e入力デバイス = E入力デバイス.MIDI入力;
							goto Label_0071;

						case 'N':
							e入力デバイス = E入力デバイス.マウス;
							goto Label_0071;
					}
				}
				continue;
			Label_0071:
				num2 = "0123456789".IndexOf( str[ 1 ] );
				if( ( ( num2 >= 0 ) && int.TryParse( str.Substring( 2 ), out num3 ) ) && ( ( num3 >= 0 ) && ( num3 <= 0xff ) ) )
				{
					this.t指定した入力が既にアサイン済みである場合はそれを全削除する( e入力デバイス, num2, num3 );
					assign[ i ].入力デバイス = e入力デバイス;
					assign[ i ].ID = num2;
					assign[ i ].コード = num3;
				}
			}
		}
		private void tデフォルトのキーアサインに設定する()
		{
			this.tキーアサインを全部クリアする();
			CKeyAssign.STKEYASSIGN[] stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			CKeyAssign.STKEYASSIGN stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign = stkeyassign63;
			stkeyassign.入力デバイス = E入力デバイス.キーボード;
			stkeyassign.ID = 0;
			stkeyassign.コード = 0x23;
			stkeyassignArray[ 0 ] = stkeyassign;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign2 = stkeyassign63;
			stkeyassign2.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign2.ID = 0;
			stkeyassign2.コード = 0x2a;
			stkeyassignArray[ 1 ] = stkeyassign2;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign3 = stkeyassign63;
			stkeyassign3.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign3.ID = 0;
			stkeyassign3.コード = 0x5d;
			stkeyassignArray[ 2 ] = stkeyassign3;
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.HH = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign4 = stkeyassign63;
			stkeyassign4.入力デバイス = E入力デバイス.キーボード;
			stkeyassign4.ID = 0;
			stkeyassign4.コード = 0x21;
			stkeyassignArray[ 0 ] = stkeyassign4;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign5 = stkeyassign63;
			stkeyassign5.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign5.ID = 0;
			stkeyassign5.コード = 0x19;
			stkeyassignArray[ 1 ] = stkeyassign5;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign6 = stkeyassign63;
			stkeyassign6.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign6.ID = 0;
			stkeyassign6.コード = 0x1a;
			stkeyassignArray[ 2 ] = stkeyassign6;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign7 = stkeyassign63;
			stkeyassign7.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign7.ID = 0;
			stkeyassign7.コード = 0x1b;
			stkeyassignArray[ 3 ] = stkeyassign7;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign8 = stkeyassign63;
			stkeyassign8.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign8.ID = 0;
			stkeyassign8.コード = 0x1c;
			stkeyassignArray[ 4 ] = stkeyassign8;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign9 = stkeyassign63;
			stkeyassign9.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign9.ID = 0;
			stkeyassign9.コード = 0x1d;
			stkeyassignArray[ 5 ] = stkeyassign9;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign10 = stkeyassign63;
			stkeyassign10.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign10.ID = 0;
			stkeyassign10.コード = 0x1f;
			stkeyassignArray[ 6 ] = stkeyassign10;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign11 = stkeyassign63;
			stkeyassign11.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign11.ID = 0;
			stkeyassign11.コード = 0x20;
			stkeyassignArray[ 7 ] = stkeyassign11;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign12 = stkeyassign63;
			stkeyassign12.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign12.ID = 0;
			stkeyassign12.コード = 0x22;
			stkeyassignArray[ 8 ] = stkeyassign12;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign13 = stkeyassign63;
			stkeyassign13.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign13.ID = 0;
			stkeyassign13.コード = 0x25;
			stkeyassignArray[ 9 ] = stkeyassign13;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign14 = stkeyassign63;
			stkeyassign14.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign14.ID = 0;
			stkeyassign14.コード = 0x26;
			stkeyassignArray[ 10 ] = stkeyassign14;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign15 = stkeyassign63;
			stkeyassign15.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign15.ID = 0;
			stkeyassign15.コード = 40;
			stkeyassignArray[ 11 ] = stkeyassign15;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign16 = stkeyassign63;
			stkeyassign16.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign16.ID = 0;
			stkeyassign16.コード = 0x71;
			stkeyassignArray[ 12 ] = stkeyassign16;
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.SD = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign17 = stkeyassign63;
			stkeyassign17.入力デバイス = E入力デバイス.キーボード;
			stkeyassign17.ID = 0;
			stkeyassign17.コード = 12;
			stkeyassignArray[ 0 ] = stkeyassign17;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign18 = stkeyassign63;
			stkeyassign18.入力デバイス = E入力デバイス.キーボード;
			stkeyassign18.ID = 0;
			stkeyassign18.コード = 0x7e;
			stkeyassignArray[ 1 ] = stkeyassign18;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign19 = stkeyassign63;
			stkeyassign19.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign19.ID = 0;
			stkeyassign19.コード = 0x21;
			stkeyassignArray[ 2 ] = stkeyassign19;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign20 = stkeyassign63;
			stkeyassign20.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign20.ID = 0;
			stkeyassign20.コード = 0x23;
			stkeyassignArray[ 3 ] = stkeyassign20;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign21 = stkeyassign63;
			stkeyassign21.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign21.ID = 0;
			stkeyassign21.コード = 0x24;
			stkeyassignArray[ 4 ] = stkeyassign21;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign22 = stkeyassign63;
			stkeyassign22.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign22.ID = 0;
			stkeyassign22.コード = 0x70;
			stkeyassignArray[ 5 ] = stkeyassign22;
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.BD = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign23 = stkeyassign63;
			stkeyassign23.入力デバイス = E入力デバイス.キーボード;
			stkeyassign23.ID = 0;
			stkeyassign23.コード = 0x1f;
			stkeyassignArray[ 0 ] = stkeyassign23;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign24 = stkeyassign63;
			stkeyassign24.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign24.ID = 0;
			stkeyassign24.コード = 0x30;
			stkeyassignArray[ 1 ] = stkeyassign24;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign25 = stkeyassign63;
			stkeyassign25.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign25.ID = 0;
			stkeyassign25.コード = 50;
			stkeyassignArray[ 2 ] = stkeyassign25;
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.HT = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign26 = stkeyassign63;
			stkeyassign26.入力デバイス = E入力デバイス.キーボード;
			stkeyassign26.ID = 0;
			stkeyassign26.コード = 11;
			stkeyassignArray[ 0 ] = stkeyassign26;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign27 = stkeyassign63;
			stkeyassign27.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign27.ID = 0;
			stkeyassign27.コード = 0x2f;
			stkeyassignArray[ 1 ] = stkeyassign27;
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.LT = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign28 = stkeyassign63;
			stkeyassign28.入力デバイス = E入力デバイス.キーボード;
			stkeyassign28.ID = 0;
			stkeyassign28.コード = 0x17;
			stkeyassignArray[ 0 ] = stkeyassign28;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign29 = stkeyassign63;
			stkeyassign29.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign29.ID = 0;
			stkeyassign29.コード = 0x29;
			stkeyassignArray[ 1 ] = stkeyassign29;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign30 = stkeyassign63;
			stkeyassign30.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign30.ID = 0;
			stkeyassign30.コード = 0x2b;
			stkeyassignArray[ 2 ] = stkeyassign30;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign31 = stkeyassign63;
			stkeyassign31.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign31.ID = 0;
			stkeyassign31.コード = 0x2d;
			stkeyassignArray[ 3 ] = stkeyassign31;
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.FT = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign32 = stkeyassign63;
			stkeyassign32.入力デバイス = E入力デバイス.キーボード;
			stkeyassign32.ID = 0;
			stkeyassign32.コード = 0x16;
			stkeyassignArray[ 0 ] = stkeyassign32;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign33 = stkeyassign63;
			stkeyassign33.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign33.ID = 0;
			stkeyassign33.コード = 0x31;
			stkeyassignArray[ 1 ] = stkeyassign33;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign34 = stkeyassign63;
			stkeyassign34.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign34.ID = 0;
			stkeyassign34.コード = 0x34;
			stkeyassignArray[ 2 ] = stkeyassign34;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign35 = stkeyassign63;
			stkeyassign35.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign35.ID = 0;
			stkeyassign35.コード = 0x37;
			stkeyassignArray[ 3 ] = stkeyassign35;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign36 = stkeyassign63;
			stkeyassign36.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign36.ID = 0;
			stkeyassign36.コード = 0x39;
			stkeyassignArray[ 4 ] = stkeyassign36;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign37 = stkeyassign63;
			stkeyassign37.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign37.ID = 0;
			stkeyassign37.コード = 0x5b;
			stkeyassignArray[ 5 ] = stkeyassign37;
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.CY = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign38 = stkeyassign63;
			stkeyassign38.入力デバイス = E入力デバイス.キーボード;
			stkeyassign38.ID = 0;
			stkeyassign38.コード = 10;
			stkeyassignArray[ 0 ] = stkeyassign38;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign39 = stkeyassign63;
			stkeyassign39.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign39.ID = 0;
			stkeyassign39.コード = 0x2e;
			stkeyassignArray[ 1 ] = stkeyassign39;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign40 = stkeyassign63;
			stkeyassign40.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign40.ID = 0;
			stkeyassign40.コード = 0x5c;
			stkeyassignArray[ 2 ] = stkeyassign40;
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.HHO = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign41 = stkeyassign63;
			stkeyassign41.入力デバイス = E入力デバイス.キーボード;
			stkeyassign41.ID = 0;
			stkeyassign41.コード = 20;
			stkeyassignArray[ 0 ] = stkeyassign41;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign42 = stkeyassign63;
			stkeyassign42.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign42.ID = 0;
			stkeyassign42.コード = 0x33;
			stkeyassignArray[ 1 ] = stkeyassign42;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign43 = stkeyassign63;
			stkeyassign43.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign43.ID = 0;
			stkeyassign43.コード = 0x35;
			stkeyassignArray[ 2 ] = stkeyassign43;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign44 = stkeyassign63;
			stkeyassign44.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign44.ID = 0;
			stkeyassign44.コード = 0x3b;
			stkeyassignArray[ 3 ] = stkeyassign44;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign45 = stkeyassign63;
			stkeyassign45.入力デバイス = E入力デバイス.MIDI入力;
			stkeyassign45.ID = 0;
			stkeyassign45.コード = 0x59;
			stkeyassignArray[ 4 ] = stkeyassign45;
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.RD = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign46 = stkeyassign63;
			stkeyassign46.入力デバイス = E入力デバイス.キーボード;
			stkeyassign46.ID = 0;
			stkeyassign46.コード = 0x1a;
			stkeyassignArray[ 0 ] = stkeyassign46;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Drums.LC = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign47 = stkeyassign63;
			stkeyassign47.入力デバイス = E入力デバイス.キーボード;
			stkeyassign47.ID = 0;
			stkeyassign47.コード = 0x37;
			stkeyassignArray[ 0 ] = stkeyassign47;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.R = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign48 = stkeyassign63;
			stkeyassign48.入力デバイス = E入力デバイス.キーボード;
			stkeyassign48.ID = 0;
			stkeyassign48.コード = 0x38;
			stkeyassignArray[ 0 ] = stkeyassign48;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.G = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign49 = stkeyassign63;
			stkeyassign49.入力デバイス = E入力デバイス.キーボード;
			stkeyassign49.ID = 0;
			stkeyassign49.コード = 0x39;
			stkeyassignArray[ 0 ] = stkeyassign49;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.B = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign50 = stkeyassign63;
			stkeyassign50.入力デバイス = E入力デバイス.キーボード;
			stkeyassign50.ID = 0;
			stkeyassign50.コード = 0x73;
			stkeyassignArray[ 0 ] = stkeyassign50;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign51 = stkeyassign63;
			stkeyassign51.入力デバイス = E入力デバイス.キーボード;
			stkeyassign51.ID = 0;
			stkeyassign51.コード = 0x2e;
			stkeyassignArray[ 1 ] = stkeyassign51;
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.Pick = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign52 = stkeyassign63;
			stkeyassign52.入力デバイス = E入力デバイス.キーボード;
			stkeyassign52.ID = 0;
			stkeyassign52.コード = 0x74;
			stkeyassignArray[ 0 ] = stkeyassign52;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.Wail = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign53 = stkeyassign63;
			stkeyassign53.入力デバイス = E入力デバイス.キーボード;
			stkeyassign53.ID = 0;
			stkeyassign53.コード = 0x3d;
			stkeyassignArray[ 0 ] = stkeyassign53;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.Cancel = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign54 = stkeyassign63;
			stkeyassign54.入力デバイス = E入力デバイス.キーボード;
			stkeyassign54.ID = 0;
			stkeyassign54.コード = 60;
			stkeyassignArray[ 0 ] = stkeyassign54;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Guitar.Decide = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Guitar[ 7 ] = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Guitar[ 8 ] = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Guitar[ 9 ] = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign55 = stkeyassign63;
			stkeyassign55.入力デバイス = E入力デバイス.キーボード;
			stkeyassign55.ID = 0;
			stkeyassign55.コード = 90;
			stkeyassignArray[ 0 ] = stkeyassign55;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.R = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign56 = stkeyassign63;
			stkeyassign56.入力デバイス = E入力デバイス.キーボード;
			stkeyassign56.ID = 0;
			stkeyassign56.コード = 0x5b;
			stkeyassignArray[ 0 ] = stkeyassign56;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.G = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign57 = stkeyassign63;
			stkeyassign57.入力デバイス = E入力デバイス.キーボード;
			stkeyassign57.ID = 0;
			stkeyassign57.コード = 0x5c;
			stkeyassignArray[ 0 ] = stkeyassign57;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.B = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign58 = stkeyassign63;
			stkeyassign58.入力デバイス = E入力デバイス.キーボード;
			stkeyassign58.ID = 0;
			stkeyassign58.コード = 0x67;
			stkeyassignArray[ 0 ] = stkeyassign58;
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign59 = stkeyassign63;
			stkeyassign59.入力デバイス = E入力デバイス.キーボード;
			stkeyassign59.ID = 0;
			stkeyassign59.コード = 100;
			stkeyassignArray[ 1 ] = stkeyassign59;
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.Pick = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign60 = stkeyassign63;
			stkeyassign60.入力デバイス = E入力デバイス.キーボード;
			stkeyassign60.ID = 0;
			stkeyassign60.コード = 0x59;
			stkeyassignArray[ 0 ] = stkeyassign60;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.Wail = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			stkeyassign63 = new CKeyAssign.STKEYASSIGN();
			CKeyAssign.STKEYASSIGN stkeyassign61 = stkeyassign63;
			stkeyassign61.入力デバイス = E入力デバイス.キーボード;
			stkeyassign61.ID = 0;
			stkeyassign61.コード = 0x61;
			stkeyassignArray[ 0 ] = stkeyassign61;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.Cancel = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[ 0x10 ];
			CKeyAssign.STKEYASSIGN stkeyassign62 = new CKeyAssign.STKEYASSIGN();
			stkeyassign62.入力デバイス = E入力デバイス.キーボード;
			stkeyassign62.ID = 0;
			stkeyassign62.コード = 0x60;
			stkeyassignArray[ 0 ] = stkeyassign62;
			stkeyassignArray[ 1 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 2 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 3 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 4 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 5 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 6 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 7 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 8 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 9 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 10 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 11 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 12 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 13 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 14 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			stkeyassignArray[ 15 ] = new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 );
			this.KeyAssign.Bass.Decide = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Bass[ 7 ] = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Bass[ 8 ] = stkeyassignArray;
			stkeyassignArray = new CKeyAssign.STKEYASSIGN[] { new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ), new CKeyAssign.STKEYASSIGN( E入力デバイス.不明, 0, 0 ) };
			this.KeyAssign.Bass[ 9 ] = stkeyassignArray;
		}
		//-----------------
		#endregion
	}
}
