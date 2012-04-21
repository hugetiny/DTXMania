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
using FDK;

namespace DTXMania
{
	internal class CDTX : CActivity
	{
		// 定数

		public enum E種別 { DTX, GDA, G2D, BMS, BME, SMF }
		public enum Eレーンビットパターン
		{
			OPEN = 0,
			xxB = 1,
			xGx = 2,
			xGB = 3,
			Rxx = 4,
			RxB = 5,
			RGx = 6,
			RGB = 7
		};


		// クラス

		public class CAVI : IDisposable
		{
			public CAvi avi;
			private bool bDispose済み;
			public int n番号;
			public string strコメント文 = "";
			public string strファイル名 = "";

			public void OnDeviceCreated()
			{
				#region [ strAVIファイル名の作成。]
				//-----------------
				string strAVIファイル名;
				if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
					strAVIファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
				else
					strAVIファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
				//-----------------
				#endregion

				if( !File.Exists( strAVIファイル名 ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})({1})", this.strコメント文, strAVIファイル名 );
					this.avi = null;
					return;
				}

				// AVI の生成。

				try
				{
					this.avi = new CAvi( strAVIファイル名 );
					Trace.TraceInformation( "動画を生成しました。({0})({1})({2}frames)", this.strコメント文, strAVIファイル名, this.avi.GetMaxFrameCount() );
				}
				catch( Exception e )
				{
					Trace.TraceError( e.Message );
					Trace.TraceError( "動画の生成に失敗しました。({0})({1})", this.strコメント文, strAVIファイル名 );
					this.avi = null;
				}
			}
			public override string ToString()
			{
				return string.Format( "CAVI{0}: File:{1}, Comment:{2}", CDTX.tZZ( this.n番号 ), this.strファイル名, this.strコメント文 );
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if( this.bDispose済み )
					return;

				if( this.avi != null )
				{
					#region [ strAVIファイル名 の作成。 ]
					//-----------------
					string strAVIファイル名;
					if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
						strAVIファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
					else
						strAVIファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
					//-----------------
					#endregion

					this.avi.Dispose();
					this.avi = null;
					
					Trace.TraceInformation( "動画を解放しました。({0})({1})", this.strコメント文, strAVIファイル名 );
				}

				this.bDispose済み = true;
			}
			//-----------------
			#endregion
		}
		public class CAVIPAN
		{
			public int nAVI番号;
			public int n移動時間ct;
			public int n番号;
			public Point pt動画側開始位置 = new Point( 0, 0 );
			public Point pt動画側終了位置 = new Point( 0, 0 );
			public Point pt表示側開始位置 = new Point( 0, 0 );
			public Point pt表示側終了位置 = new Point( 0, 0 );
			public Size sz開始サイズ = new Size( 0, 0 );
			public Size sz終了サイズ = new Size( 0, 0 );

			public override string ToString()
			{
				return string.Format( "CAVIPAN{0}: AVI:{14}, 開始サイズ:{1}x{2}, 終了サイズ:{3}x{4}, 動画側開始位置:{5}x{6}, 動画側終了位置:{7}x{8}, 表示側開始位置:{9}x{10}, 表示側終了位置:{11}x{12}, 移動時間:{13}ct",
					CDTX.tZZ( this.n番号 ),
					this.sz開始サイズ.Width, this.sz開始サイズ.Height,
					this.sz終了サイズ.Width, this.sz終了サイズ.Height,
					this.pt動画側開始位置.X, this.pt動画側開始位置.Y,
					this.pt動画側終了位置.X, this.pt動画側終了位置.Y,
					this.pt表示側開始位置.X, this.pt表示側開始位置.Y,
					this.pt表示側終了位置.X, this.pt表示側終了位置.Y,
					this.n移動時間ct,
					CDTX.tZZ( this.nAVI番号 ) );
			}
		}
		public class CBGA
		{
			public int nBMP番号;
			public int n番号;
			public Point pt画像側右下座標 = new Point( 0, 0 );
			public Point pt画像側左上座標 = new Point( 0, 0 );
			public Point pt表示座標 = new Point( 0, 0 );

			public override string ToString()
			{
				return string.Format( "CBGA{0}, BMP:{1}, 画像側左上座標:{2}x{3}, 画像側右下座標:{4}x{5}, 表示座標:{6}x{7}",
					CDTX.tZZ( this.n番号 ),
					CDTX.tZZ( this.nBMP番号 ),
					this.pt画像側左上座標.X, this.pt画像側左上座標.Y,
					this.pt画像側右下座標.X, this.pt画像側右下座標.Y,
					this.pt表示座標.X, this.pt表示座標.Y );
			}
		}
		public class CBGAPAN
		{
			public int nBMP番号;
			public int n移動時間ct;
			public int n番号;
			public Point pt画像側開始位置 = new Point( 0, 0 );
			public Point pt画像側終了位置 = new Point( 0, 0 );
			public Point pt表示側開始位置 = new Point( 0, 0 );
			public Point pt表示側終了位置 = new Point( 0, 0 );
			public Size sz開始サイズ = new Size( 0, 0 );
			public Size sz終了サイズ = new Size( 0, 0 );

			public override string ToString()
			{
				return string.Format( "CBGAPAN{0}: BMP:{14}, 開始サイズ:{1}x{2}, 終了サイズ:{3}x{4}, 画像側開始位置:{5}x{6}, 画像側終了位置:{7}x{8}, 表示側開始位置:{9}x{10}, 表示側終了位置:{11}x{12}, 移動時間:{13}ct",
					CDTX.tZZ( this.nBMP番号 ),
					this.sz開始サイズ.Width, this.sz開始サイズ.Height,
					this.sz終了サイズ.Width, this.sz終了サイズ.Height,
					this.pt画像側開始位置.X, this.pt画像側開始位置.Y,
					this.pt画像側終了位置.X, this.pt画像側終了位置.Y,
					this.pt表示側開始位置.X, this.pt表示側開始位置.Y,
					this.pt表示側終了位置.X, this.pt表示側終了位置.Y,
					this.n移動時間ct,
					CDTX.tZZ( this.nBMP番号 ) );
			}
		}
		public class CBMP : IDisposable
		{
			public bool bUse;
			public int n番号;
			public string strコメント文 = "";
			public string strファイル名 = "";
			public CTexture tx画像;
			public int n高さ
			{
				get
				{
					return this.tx画像.sz画像サイズ.Height;
				}
			}
			public int n幅
			{
				get
				{
					return this.tx画像.sz画像サイズ.Width;
				}
			}

			public void OnDeviceCreated()
			{
				#region [ strテクスチャファイル名 を作成。]
				//-----------------
				string strテクスチャファイル名;
				if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
					strテクスチャファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
				else
					strテクスチャファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
				//-----------------
				#endregion

				if( !File.Exists( strテクスチャファイル名 ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})({1})", this.strコメント文, strテクスチャファイル名 );
					this.tx画像 = null;
					return;
				}

				// テクスチャを作成。

				this.tx画像 = CDTXMania.tテクスチャの生成( strテクスチャファイル名, true );

				if( this.tx画像 != null )
				{
					// 作成成功。

					if( CDTXMania.ConfigIni.bLog作成解放ログ出力 )
						Trace.TraceInformation( "テクスチャを生成しました。({0})({1})({2}x{3})", this.strコメント文, strテクスチャファイル名, this.n幅, this.n高さ );

					this.bUse = true;
				}
				else
				{
					// 作成失敗。

					Trace.TraceError( "テクスチャの生成に失敗しました。({0})({1})", this.strコメント文, strテクスチャファイル名 );
					this.tx画像 = null;
				}
			}
			public override string ToString()
			{
				return string.Format( "CBMP{0}: File:{1}, Comment:{2}", CDTX.tZZ( this.n番号 ), this.strファイル名, this.strコメント文 );
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if( this.bDisposed済み )
					return;

				if( this.tx画像 != null )
				{
					#region [ strテクスチャファイル名 を作成。]
					//-----------------
					string strテクスチャファイル名;
					if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
						strテクスチャファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
					else
						strテクスチャファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
					//-----------------
					#endregion

					CDTXMania.tテクスチャの解放( ref this.tx画像 );

					if( CDTXMania.ConfigIni.bLog作成解放ログ出力 )
						Trace.TraceInformation( "テクスチャを解放しました。({0})({1})", this.strコメント文, strテクスチャファイル名 );
				}
				this.bUse = false;

				this.bDisposed済み = true;
			}
			//-----------------
			#endregion

			#region [ private ]
			//-----------------
			private bool bDisposed済み;
			//-----------------
			#endregion
		}
		public class CBMPTEX : IDisposable
		{
			public bool bUse;
			public int n番号;
			public string strコメント文 = "";
			public string strファイル名 = "";
			public CTexture tx画像;
			public int n高さ
			{
				get
				{
					return this.tx画像.sz画像サイズ.Height;
				}
			}
			public int n幅
			{
				get
				{
					return this.tx画像.sz画像サイズ.Width;
				}
			}

			public void OnDeviceCreated()
			{
				#region [ strテクスチャファイル名 を作成。]
				//-----------------
				string strテクスチャファイル名;
				if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
					strテクスチャファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
				else
					strテクスチャファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
				//-----------------
				#endregion

				if( !File.Exists( strテクスチャファイル名 ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})({1})", this.strコメント文, strテクスチャファイル名 );
					this.tx画像 = null;
					return;
				}

				// テクスチャを作成。

				this.tx画像 = CDTXMania.tテクスチャの生成( strテクスチャファイル名 );

				if( this.tx画像 != null )
				{
					// 作成成功

					if( CDTXMania.ConfigIni.bLog作成解放ログ出力 )
						Trace.TraceInformation( "テクスチャを生成しました。({0})({1})(Gr:{2}x{3})(Tx:{4}x{5})", this.strコメント文, strテクスチャファイル名, this.tx画像.sz画像サイズ.Width, this.tx画像.sz画像サイズ.Height, this.tx画像.szテクスチャサイズ.Width, this.tx画像.szテクスチャサイズ.Height );
					this.bUse = true;
				}
				else
				{
					// 作成失敗

					Trace.TraceError( "テクスチャの生成に失敗しました。({0})({1})", this.strコメント文, strテクスチャファイル名 );
				}
			}
			public override string ToString()
			{
				return string.Format( "CBMPTEX{0}: File:{1}, Comment:{2}", CDTX.tZZ( this.n番号 ), this.strファイル名, this.strコメント文 );
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if( this.bDisposed済み )
					return;

				if( this.tx画像 != null )
				{
					#region [ strテクスチャファイル名 を作成。]
					//-----------------
					string strテクスチャファイル名;
					if( !string.IsNullOrEmpty( CDTXMania.DTX.PATH_WAV ) )
						strテクスチャファイル名 = CDTXMania.DTX.PATH_WAV + this.strファイル名;
					else
						strテクスチャファイル名 = CDTXMania.DTX.strフォルダ名 + this.strファイル名;
					//-----------------
					#endregion

					CDTXMania.tテクスチャの解放( ref this.tx画像 );

					if( CDTXMania.ConfigIni.bLog作成解放ログ出力 )
						Trace.TraceInformation( "テクスチャを解放しました。({0})({1})", this.strコメント文, strテクスチャファイル名 );
				}
				this.bUse = false;

				this.bDisposed済み = true;
			}
			//-----------------
			#endregion

			#region [ private ]
			//-----------------
			private bool bDisposed済み;
			//-----------------
			#endregion
		}
		public class CBPM
		{
			public double dbBPM値;
			public int n内部番号;
			public int n表記上の番号;

			public override string ToString()
			{
				StringBuilder builder = new StringBuilder( 0x80 );
				if( this.n内部番号 != this.n表記上の番号 )
				{
					builder.Append( string.Format( "CBPM{0}(内部{1})", CDTX.tZZ( this.n表記上の番号 ), this.n内部番号 ) );
				}
				else
				{
					builder.Append( string.Format( "CBPM{0}", CDTX.tZZ( this.n表記上の番号 ) ) );
				}
				builder.Append( string.Format( ", BPM:{0}", this.dbBPM値 ) );
				return builder.ToString();
			}
		}
		public class CChip : IComparable<CDTX.CChip>
		{
			public bool bHit;
			public bool b可視 = true;
			public double dbチップサイズ倍率 = 1.0;
			public double db実数値;
			public EAVI種別 eAVI種別;
			public EBGA種別 eBGA種別;
			public E楽器パート e楽器パート = E楽器パート.UNKNOWN;
			public int nチャンネル番号;
			public STDGBVALUE<int> nバーからの距離dot;
			public int n整数値;
			public int n整数値・内部番号;
			public int n総移動時間;
			public int n透明度 = 0xff;
			public int n発声位置;
			public int n発声時刻ms;
			public int nLag;				// 2011.2.1 yyagi
			public CDTX.CAVI rAVI;
			public CDTX.CAVIPAN rAVIPan;
			public CDTX.CBGA rBGA;
			public CDTX.CBGAPAN rBGAPan;
			public CDTX.CBMP rBMP;
			public CDTX.CBMPTEX rBMPTEX;
			public bool bBPMチップである
			{
				get
				{
					if (this.nチャンネル番号 == 3 || this.nチャンネル番号 == 8) {
						return true;
					} else {
						return false;
					}
				}
			}
			public bool bWAVを使うチャンネルである
			{
				get
				{
					switch( this.nチャンネル番号 )
					{
						case 0x01:
						case 0x11:
						case 0x12:
						case 0x13:
						case 0x14:
						case 0x15:
						case 0x16:
						case 0x17:
						case 0x18:
						case 0x19:
						case 0x1a:
						case 0x1f:
						case 0x20:
						case 0x21:
						case 0x22:
						case 0x23:
						case 0x24:
						case 0x25:
						case 0x26:
						case 0x27:
						case 0x2f:
						case 0x31:
						case 0x32:
						case 0x33:
						case 0x34:
						case 0x35:
						case 0x36:
						case 0x37:
						case 0x38:
						case 0x39:
						case 0x3a:
						case 0x61:
						case 0x62:
						case 0x63:
						case 0x64:
						case 0x65:
						case 0x66:
						case 0x67:
						case 0x68:
						case 0x69:
						case 0x70:
						case 0x71:
						case 0x72:
						case 0x73:
						case 0x74:
						case 0x75:
						case 0x76:
						case 0x77:
						case 0x78:
						case 0x79:
						case 0x80:
						case 0x81:
						case 0x82:
						case 0x83:
						case 0x84:
						case 0x85:
						case 0x86:
						case 0x87:
						case 0x88:
						case 0x89:
						case 0x90:
						case 0x91:
						case 0x92:
						case 0xa0:
						case 0xa1:
						case 0xa2:
						case 0xa3:
						case 0xa4:
						case 0xa5:
						case 0xa6:
						case 0xa7:
						case 0xaf:
						case 0xb1:
						case 0xb2:
						case 0xb3:
						case 0xb4:
						case 0xb5:
						case 0xb6:
						case 0xb7:
						case 0xb8:
						case 0xb9:
						case 0xba:
						case 0xbb:
						case 0xbc:
							return true;
					}
					return false;
				}
			}
			public bool b自動再生音チャンネルである
			{
				get
				{
					int num = this.nチャンネル番号;
					if( ( ( ( num != 1 ) && ( ( 0x61 > num ) || ( num > 0x69 ) ) ) && ( ( 0x70 > num ) || ( num > 0x79 ) ) ) && ( ( 0x80 > num ) || ( num > 0x89 ) ) )
					{
						return ( ( 0x90 <= num ) && ( num <= 0x92 ) );
					}
					return true;
				}
			}
			public bool bIsAutoPlayed;						// 2011.6.10 yyagi
			
			public CChip()
			{
				this.nバーからの距離dot = new STDGBVALUE<int>() {
					Drums = 0,
					Guitar = 0,
					Bass = 0,
				};
			}
			public void t初期化()
			{
				this.nチャンネル番号 = 0;
				this.n整数値 = 0;
				this.n整数値・内部番号 = 0;
				this.db実数値 = 0.0;
				this.n発声位置 = 0;
				this.n発声時刻ms = 0;
				this.nLag = -999;
				this.bIsAutoPlayed = false;
				this.dbチップサイズ倍率 = 1.0;
				this.bHit = false;
				this.b可視 = true;
				this.e楽器パート = E楽器パート.UNKNOWN;
				this.n透明度 = 0xff;
				this.nバーからの距離dot.Drums = 0;
				this.nバーからの距離dot.Guitar = 0;
				this.nバーからの距離dot.Bass = 0;
				this.n総移動時間 = 0;
			}
			public override string ToString()
			{
				string str = "";
				switch( this.nチャンネル番号 )
				{
					case 0x01:
						str = "バックコーラス";
						break;

					case 0x02:
						str = "小節長変更";
						break;

					case 0x03:
						str = "BPM変更";
						break;

					case 0x04:
						str = "BMPレイヤ1";
						break;

					case 0x07:
						str = "BMPレイヤ2";
						break;

					case 0x08:
						str = "BPM変更(拡張)";
						break;

					case 0x11:
						str = "HHClose";
						break;

					case 0x12:
						str = "Snare";
						break;

					case 0x13:
						str = "Kick";
						break;

					case 0x14:
						str = "HiTom";
						break;

					case 0x15:
						str = "LowTom";
						break;

					case 0x16:
						str = "Cymbal";
						break;

					case 0x17:
						str = "FloorTom";
						break;

					case 0x18:
						str = "HHOpen";
						break;

					case 0x19:
						str = "RideCymbal";
						break;

					case 0x1a:
						str = "LeftCymbal";
						break;

					case 0x1f:
						str = "ドラム歓声切替";
						break;

					case 0x20:
						str = "ギターOPEN";
						break;

					case 0x21:
						str = "ギター - - B";
						break;

					case 0x22:
						str = "ギター - G -";
						break;

					case 0x23:
						str = "ギター - G B";
						break;

					case 0x24:
						str = "ギター R - -";
						break;

					case 0x25:
						str = "ギター R - B";
						break;

					case 0x26:
						str = "ギター R G -";
						break;

					case 0x27:
						str = "ギター R G B";
						break;

					case 0x28:
						str = "ギターWailing";
						break;

					case 0x2f:
						str = "ギターWailing音切替";
						break;

					case 0x31:
						str = "HHClose(不可視)";
						break;

					case 0x32:
						str = "Snare(不可視)";
						break;

					case 0x33:
						str = "Kick(不可視)";
						break;

					case 0x34:
						str = "HiTom(不可視)";
						break;

					case 0x35:
						str = "LowTom(不可視)";
						break;

					case 0x36:
						str = "Cymbal(不可視)";
						break;

					case 0x37:
						str = "FloorTom(不可視)";
						break;

					case 0x38:
						str = "HHOpen(不可視)";
						break;

					case 0x39:
						str = "RideCymbal(不可視)";
						break;

					case 0x3a:
						str = "LeftCymbal(不可視)";
						break;

					case 0x50:
						str = "小節線";
						break;

					case 0x51:
						str = "拍線";
						break;

					case 0x52:
						str = "MIDIコーラス";
						break;

					case 0x53:
						str = "フィルイン";
						break;

					case 0x54:
						str = "AVI";
						break;

					case 0x55:
						str = "BMPレイヤ3";
						break;

					case 0x56:
						str = "BMPレイヤ4";
						break;

					case 0x57:
						str = "BMPレイヤ5";
						break;

					case 0x58:
						str = "BMPレイヤ6";
						break;

					case 0x59:
						str = "BMPレイヤ7";
						break;

					case 0x60:
						str = "BMPレイヤ8";
						break;

					case 0x61:
						str = "SE01";
						break;

					case 0x62:
						str = "SE02";
						break;

					case 0x63:
						str = "SE03";
						break;

					case 0x64:
						str = "SE04";
						break;

					case 0x65:
						str = "SE05";
						break;

					case 0x66:
						str = "SE06";
						break;

					case 0x67:
						str = "SE07";
						break;

					case 0x68:
						str = "SE08";
						break;

					case 0x69:
						str = "SE09";
						break;

					case 0x70:
						str = "SE10";
						break;

					case 0x71:
						str = "SE11";
						break;

					case 0x72:
						str = "SE12";
						break;

					case 0x73:
						str = "SE13";
						break;

					case 0x74:
						str = "SE14";
						break;

					case 0x75:
						str = "SE15";
						break;

					case 0x76:
						str = "SE16";
						break;

					case 0x77:
						str = "SE17";
						break;

					case 0x78:
						str = "SE18";
						break;

					case 0x79:
						str = "SE19";
						break;

					case 0x80:
						str = "SE20";
						break;

					case 0x81:
						str = "SE21";
						break;

					case 0x82:
						str = "SE22";
						break;

					case 0x83:
						str = "SE23";
						break;

					case 0x84:
						str = "SE24";
						break;

					case 0x85:
						str = "SE25";
						break;

					case 0x86:
						str = "SE26";
						break;

					case 0x87:
						str = "SE27";
						break;

					case 0x88:
						str = "SE28";
						break;

					case 0x89:
						str = "SE29";
						break;

					case 0x90:
						str = "SE30";
						break;

					case 0x91:
						str = "SE31";
						break;

					case 0x92:
						str = "SE32";
						break;

					case 0xa0:
						str = "ベースOPEN";
						break;

					case 0xa1:
						str = "ベース - - B";
						break;

					case 0xa2:
						str = "ベース - G -";
						break;

					case 0xa3:
						str = "ベース - G B";
						break;

					case 0xa4:
						str = "ベース R - -";
						break;

					case 0xa5:
						str = "ベース R - B";
						break;

					case 0xa6:
						str = "ベース R G -";
						break;

					case 0xa7:
						str = "ベース R G B";
						break;

					case 0xa8:
						str = "ベースWailing";
						break;

					case 0xaf:
						str = "ベースWailing音切替";
						break;

					case 0xb1:
						str = "HHClose(空うち)";
						break;

					case 0xb2:
						str = "Snare(空うち)";
						break;

					case 0xb3:
						str = "Kick(空うち)";
						break;

					case 0xb4:
						str = "HiTom(空うち)";
						break;

					case 0xb5:
						str = "LowTom(空うち)";
						break;

					case 0xb6:
						str = "Cymbal(空うち)";
						break;

					case 0xb7:
						str = "FloorTom(空うち)";
						break;

					case 0xb8:
						str = "HHOpen(空うち)";
						break;

					case 0xb9:
						str = "RideCymbal(空うち)";
						break;

					case 0xba:
						str = "ギター(空打ち)";
						break;

					case 0xbb:
						str = "ベース(空打ち)";
						break;

					case 0xbc:
						str = "LeftCymbal(空うち)";
						break;

					case 0xc4:
						str = "BGAスコープ画像切替1";
						break;

					case 0xc7:
						str = "BGAスコープ画像切替2";
						break;

					case 0xd5:
						str = "BGAスコープ画像切替3";
						break;

					case 0xd6:
						str = "BGAスコープ画像切替4";
						break;

					case 0xd7:
						str = "BGAスコープ画像切替5";
						break;

					case 0xd8:
						str = "BGAスコープ画像切替6";
						break;

					case 0xd9:
						str = "BGAスコープ画像切替7";
						break;

					case 0xe0:
						str = "BGAスコープ画像切替8";
						break;

					default:
						str = "??";
						break;
				}
				return string.Format( "CChip: 位置:{0:D4}.{1:D3}, 時刻{2:D6}, Ch:{3:X2}({4}), Pn:{5}({11})(内部{6}), Pd:{7}, Sz:{8}, UseWav:{9}, Auto:{10}",
					this.n発声位置 / 384, this.n発声位置 % 384,
					this.n発声時刻ms,
					this.nチャンネル番号, str,
					this.n整数値, this.n整数値・内部番号,
					this.db実数値,
					this.dbチップサイズ倍率,
					this.bWAVを使うチャンネルである,
					this.b自動再生音チャンネルである,
					CDTX.tZZ( this.n整数値 ) );
			}

			#region [ IComparable 実装 ]
			//-----------------
			public int CompareTo( CDTX.CChip other )
			{
				byte[] n優先度 = new byte[] {
					5, 5, 3, 3, 5, 5, 5, 5, 3, 5, 5, 5, 5, 5, 5, 5, 
					5, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 
					7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
				};


				// まずは位置で比較。

				if( this.n発声位置 < other.n発声位置 )
					return -1;

				if( this.n発声位置 > other.n発声位置 )
					return 1;


				// 位置が同じなら優先度で比較。

				if( n優先度[ this.nチャンネル番号 ] < n優先度[ other.nチャンネル番号 ] )
					return -1;

				if( n優先度[ this.nチャンネル番号 ] > n優先度[ other.nチャンネル番号 ] )
					return 1;


				// 位置も優先度も同じなら同じと返す。

				return 0;
			}
			//-----------------
			#endregion
		}
		public class CWAV : IDisposable
		{
			public bool bBGMとして使う;
			public List<int> listこのWAVを使用するチャンネル番号の集合 = new List<int>( 16 );
			public int nチップサイズ = 100;
			public int n位置;
			public long[] n一時停止時刻 = new long[ 4 ];
			public int n音量 = 100;
			public int n現在再生中のサウンド番号;
			public long[] n再生開始時刻 = new long[ 4 ];
			public int n内部番号;
			public int n表記上の番号;
			public CSound[] rSound = new CSound[ 4 ];
			public string strコメント文 = "";
			public string strファイル名 = "";
			public bool bBGMとして使わない
			{
				get
				{
					return !this.bBGMとして使う;
				}
				set
				{
					this.bBGMとして使う = !value;
				}
			}

			public override string ToString()
			{
				var sb = new StringBuilder( 128 );
				
				if( this.n表記上の番号 == this.n内部番号 )
				{
					sb.Append( string.Format( "CWAV{0}: ", CDTX.tZZ( this.n表記上の番号 ) ) );
				}
				else
				{
					sb.Append( string.Format( "CWAV{0}(内部{1}): ", CDTX.tZZ( this.n表記上の番号 ), this.n内部番号 ) );
				}
				sb.Append( string.Format( "音量:{0}, 位置:{1}, サイズ:{2}, BGM:{3}, File:{4}, Comment:{5}", this.n音量, this.n位置, this.nチップサイズ, this.bBGMとして使う ? 'Y' : 'N', this.strファイル名, this.strコメント文 ) );
				
				return sb.ToString();
			}

			#region [ Dispose-Finalize パターン実装 ]
			//-----------------
			public void Dispose()
			{
				this.Dispose( true );
				GC.SuppressFinalize( this );
			}
			public void Dispose( bool bManagedリソースの解放も行う )
			{
				if( this.bDisposed済み )
					return;

				if( bManagedリソースの解放も行う )
				{
					for( int i = 0; i < 4; i++ )
					{
						if( this.rSound[ i ] != null )
							CDTXMania.Sound管理.tサウンドを破棄する( this.rSound[ i ] );
						this.rSound[ i ] = null;

						if( ( i == 0 ) && CDTXMania.ConfigIni.bLog作成解放ログ出力 )
							Trace.TraceInformation( "サウンドを解放しました。({0})({1})", this.strコメント文, this.strファイル名 );
					}
				}

				this.bDisposed済み = true;
			}
			~CWAV()
			{
				this.Dispose( false );
			}
			//-----------------
			#endregion

			#region [ private ]
			//-----------------
			private bool bDisposed済み;
			//-----------------
			#endregion
		}
		

		// 構造体

		public struct STLANEINT
		{
			public int HH;
			public int SD;
			public int BD;
			public int HT;
			public int LT;
			public int CY;
			public int FT;
			public int HHO;
			public int RD;
			public int LC;

			public int Drums
			{
				get
				{
					return this.HH + this.SD + this.BD + this.HT + this.LT + this.CY + this.FT + this.HHO + this.RD + this.LC;
				}
			}
			public int Guitar;
			public int Bass;

			public int this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.HH;

						case 1:
							return this.SD;

						case 2:
							return this.BD;

						case 3:
							return this.HT;

						case 4:
							return this.LT;

						case 5:
							return this.CY;

						case 6:
							return this.FT;

						case 7:
							return this.HHO;

						case 8:
							return this.RD;

						case 9:
							return this.LC;

						case 10:
							return this.Guitar;

						case 11:
							return this.Bass;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					if( value < 0 )
					{
						throw new ArgumentOutOfRangeException();
					}
					switch( index )
					{
						case 0:
							this.HH = value;
							return;

						case 1:
							this.SD = value;
							return;

						case 2:
							this.BD = value;
							return;

						case 3:
							this.HT = value;
							return;

						case 4:
							this.LT = value;
							return;

						case 5:
							this.CY = value;
							return;

						case 6:
							this.FT = value;
							return;

						case 7:
							this.HHO = value;
							return;

						case 8:
							this.RD = value;
							return;

						case 9:
							this.LC = value;
							return;

						case 10:
							this.Guitar = value;
							return;

						case 11:
							this.Bass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		public struct STRESULT
		{
			public string SS;
			public string S;
			public string A;
			public string B;
			public string C;
			public string D;
			public string E;

			public string this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.SS;

						case 1:
							return this.S;

						case 2:
							return this.A;

						case 3:
							return this.B;

						case 4:
							return this.C;

						case 5:
							return this.D;

						case 6:
							return this.E;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.SS = value;
							return;

						case 1:
							this.S = value;
							return;

						case 2:
							this.A = value;
							return;

						case 3:
							this.B = value;
							return;

						case 4:
							this.C = value;
							return;

						case 5:
							this.D = value;
							return;

						case 6:
							this.E = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		public struct STチップがある
		{
			public bool Drums;
			public bool Guitar;
			public bool Bass;

			public bool HHOpen;
			public bool Ride;
			public bool LeftCymbal;
			public bool OpenGuitar;
			public bool OpenBass;
			
			public bool this[ int index ]
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

						case 3:
							return this.HHOpen;

						case 4:
							return this.Ride;

						case 5:
							return this.LeftCymbal;

						case 6:
							return this.OpenGuitar;

						case 7:
							return this.OpenBass;
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

						case 3:
							this.HHOpen = value;
							return;

						case 4:
							this.Ride = value;
							return;

						case 5:
							this.LeftCymbal = value;
							return;

						case 6:
							this.OpenGuitar = value;
							return;

						case 7:
							this.OpenBass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}


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
		public E種別 e種別;
		public string GENRE;
		public bool HIDDENLEVEL;
		public STDGBVALUE<int> LEVEL;
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
		public STLANEINT n可視チップ数;
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
#if TEST_NOTEOFFMODE
		public STLANEVALUE<bool> b演奏で直前の音を消音する;
//		public bool bHH演奏で直前のHHを消音する;
//		public bool bGUITAR演奏で直前のGUITARを消音する;
//		public bool bBASS演奏で直前のBASSを消音する;
#endif
		// コンストラクタ

		public CDTX()
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
			STDGBVALUE<int> stdgbvalue = new STDGBVALUE<int>();
			stdgbvalue.Drums = 0;
			stdgbvalue.Guitar = 0;
			stdgbvalue.Bass = 0;
			this.LEVEL = stdgbvalue;

#if true	// 2010.12.31 yyagi #RESULTxxxのリファクタ後。ここはnew()して参照渡ししなくてもいいよね？
			for (int i = 0; i < 7; i++) {
				this.RESULTIMAGE[i] = "";
				this.RESULTMOVIE[i] = "";
				this.RESULTSOUND[i] = "";
			}
#else		// #RESULTxxxのリファクタ前
			STRESULT stresult4 = new STRESULT();
			STRESULT stresult = stresult4;
			stresult.SS = "";
			stresult.S = "";
			stresult.A = "";
			stresult.B = "";
			stresult.C = "";
			stresult.D = "";
			stresult.E = "";
			this.RESULTIMAGE = stresult;
			stresult4 = new STRESULT();
			STRESULT stresult2 = stresult4;
			stresult2.SS = "";
			stresult2.S = "";
			stresult2.A = "";
			stresult2.B = "";
			stresult2.C = "";
			stresult2.D = "";
			stresult2.E = "";
			this.RESULTMOVIE = stresult2;
			STRESULT stresult3 = new STRESULT();
			stresult3.SS = "";
			stresult3.S = "";
			stresult3.A = "";
			stresult3.B = "";
			stresult3.C = "";
			stresult3.D = "";
			stresult3.E = "";
			this.RESULTSOUND = stresult3;
#endif
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
			this.strファイル名 = "";
			this.strフォルダ名 = "";
			this.strファイル名の絶対パス = "";
			this.n無限管理WAV = new int[ 36 * 36 ];
			this.n無限管理BPM = new int[ 36 * 36 ];
			this.n無限管理VOL = new int[ 36 * 36 ];
			this.n無限管理PAN = new int[ 36 * 36 ];
			this.n無限管理SIZE = new int[ 36 * 36 ];
			this.nRESULTIMAGE用優先順位 = new int[ 7 ];
			this.nRESULTMOVIE用優先順位 = new int[ 7 ];
			this.nRESULTSOUND用優先順位 = new int[ 7 ];

#if true	// 2011.1.1 yyagi GDA->DTX変換テーブル リファクタ後
			STGDAPARAM[] stgdaparamArray = new STGDAPARAM[] {		// GDA->DTX conversion table
				new STGDAPARAM("TC", 0x03),	new STGDAPARAM("BL", 0x02),	new STGDAPARAM("GS", 0x29),
				new STGDAPARAM("DS", 0x30),	new STGDAPARAM("FI", 0x53),	new STGDAPARAM("HH", 0x11),
				new STGDAPARAM("SD", 0x12),	new STGDAPARAM("BD", 0x13),	new STGDAPARAM("HT", 0x14),
				new STGDAPARAM("LT", 0x15),	new STGDAPARAM("CY", 0x16),	new STGDAPARAM("G1", 0x21),
				new STGDAPARAM("G2", 0x22),	new STGDAPARAM("G3", 0x23),	new STGDAPARAM("G4", 0x24),
				new STGDAPARAM("G5", 0x25),	new STGDAPARAM("G6", 0x26),	new STGDAPARAM("G7", 0x27),
				new STGDAPARAM("GW", 0x28),	new STGDAPARAM("01", 0x61),	new STGDAPARAM("02", 0x62),
				new STGDAPARAM("03", 0x63),	new STGDAPARAM("04", 0x64),	new STGDAPARAM("05", 0x65),
				new STGDAPARAM("06", 0x66),	new STGDAPARAM("07", 0x67),	new STGDAPARAM("08", 0x68),
				new STGDAPARAM("09", 0x69),	new STGDAPARAM("0A", 0x70),	new STGDAPARAM("0B", 0x71),
				new STGDAPARAM("0C", 0x72),	new STGDAPARAM("0D", 0x73),	new STGDAPARAM("0E", 0x74),
				new STGDAPARAM("0F", 0x75),	new STGDAPARAM("10", 0x76),	new STGDAPARAM("11", 0x77),
				new STGDAPARAM("12", 0x78),	new STGDAPARAM("13", 0x79),	new STGDAPARAM("14", 0x80),
				new STGDAPARAM("15", 0x81),	new STGDAPARAM("16", 0x82),	new STGDAPARAM("17", 0x83),
				new STGDAPARAM("18", 0x84),	new STGDAPARAM("19", 0x85),	new STGDAPARAM("1A", 0x86),
				new STGDAPARAM("1B", 0x87),	new STGDAPARAM("1C", 0x88),	new STGDAPARAM("1D", 0x89),
				new STGDAPARAM("1E", 0x90),	new STGDAPARAM("1F", 0x91),	new STGDAPARAM("20", 0x92),
				new STGDAPARAM("B1", 0xA1),	new STGDAPARAM("B2", 0xA2),	new STGDAPARAM("B3", 0xA3),
				new STGDAPARAM("B4", 0xA4),	new STGDAPARAM("B5", 0xA5),	new STGDAPARAM("B6", 0xA6),
				new STGDAPARAM("B7", 0xA7),	new STGDAPARAM("BW", 0xA8),	new STGDAPARAM("G0", 0x20),
				new STGDAPARAM("B0", 0xA0)
			};
#else	// 2011.1.1 yyagi GDA->DTX変換テーブル リファクタ前
			STGDAPARAM[] stgdaparamArray = new STGDAPARAM[62];
			STGDAPARAM stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam = stgdaparam62;
			stgdaparam.s = "TC";
			stgdaparam.c = 3;
			stgdaparamArray[ 0 ] = stgdaparam;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam2 = stgdaparam62;
			stgdaparam2.s = "BL";
			stgdaparam2.c = 2;
			stgdaparamArray[ 1 ] = stgdaparam2;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam3 = stgdaparam62;
			stgdaparam3.s = "GS";
			stgdaparam3.c = 0x29;
			stgdaparamArray[ 2 ] = stgdaparam3;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam4 = stgdaparam62;
			stgdaparam4.s = "DS";
			stgdaparam4.c = 0x30;
			stgdaparamArray[ 3 ] = stgdaparam4;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam5 = stgdaparam62;
			stgdaparam5.s = "FI";
			stgdaparam5.c = 0x53;
			stgdaparamArray[ 4 ] = stgdaparam5;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam6 = stgdaparam62;
			stgdaparam6.s = "HH";
			stgdaparam6.c = 0x11;
			stgdaparamArray[ 5 ] = stgdaparam6;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam7 = stgdaparam62;
			stgdaparam7.s = "SD";
			stgdaparam7.c = 0x12;
			stgdaparamArray[ 6 ] = stgdaparam7;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam8 = stgdaparam62;
			stgdaparam8.s = "BD";
			stgdaparam8.c = 0x13;
			stgdaparamArray[ 7 ] = stgdaparam8;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam9 = stgdaparam62;
			stgdaparam9.s = "HT";
			stgdaparam9.c = 20;
			stgdaparamArray[ 8 ] = stgdaparam9;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam10 = stgdaparam62;
			stgdaparam10.s = "LT";
			stgdaparam10.c = 0x15;
			stgdaparamArray[ 9 ] = stgdaparam10;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam11 = stgdaparam62;
			stgdaparam11.s = "CY";
			stgdaparam11.c = 0x16;
			stgdaparamArray[ 10 ] = stgdaparam11;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam12 = stgdaparam62;
			stgdaparam12.s = "G1";
			stgdaparam12.c = 0x21;
			stgdaparamArray[ 11 ] = stgdaparam12;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam13 = stgdaparam62;
			stgdaparam13.s = "G2";
			stgdaparam13.c = 0x22;
			stgdaparamArray[ 12 ] = stgdaparam13;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam14 = stgdaparam62;
			stgdaparam14.s = "G3";
			stgdaparam14.c = 0x23;
			stgdaparamArray[ 13 ] = stgdaparam14;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam15 = stgdaparam62;
			stgdaparam15.s = "G4";
			stgdaparam15.c = 0x24;
			stgdaparamArray[ 14 ] = stgdaparam15;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam16 = stgdaparam62;
			stgdaparam16.s = "G5";
			stgdaparam16.c = 0x25;
			stgdaparamArray[ 15 ] = stgdaparam16;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam17 = stgdaparam62;
			stgdaparam17.s = "G6";
			stgdaparam17.c = 0x26;
			stgdaparamArray[ 0x10 ] = stgdaparam17;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam18 = stgdaparam62;
			stgdaparam18.s = "G7";
			stgdaparam18.c = 0x27;
			stgdaparamArray[ 0x11 ] = stgdaparam18;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam19 = stgdaparam62;
			stgdaparam19.s = "GW";
			stgdaparam19.c = 40;
			stgdaparamArray[ 0x12 ] = stgdaparam19;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam20 = stgdaparam62;
			stgdaparam20.s = "01";
			stgdaparam20.c = 0x61;
			stgdaparamArray[ 0x13 ] = stgdaparam20;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam21 = stgdaparam62;
			stgdaparam21.s = "02";
			stgdaparam21.c = 0x62;
			stgdaparamArray[ 20 ] = stgdaparam21;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam22 = stgdaparam62;
			stgdaparam22.s = "03";
			stgdaparam22.c = 0x63;
			stgdaparamArray[ 0x15 ] = stgdaparam22;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam23 = stgdaparam62;
			stgdaparam23.s = "04";
			stgdaparam23.c = 100;
			stgdaparamArray[ 0x16 ] = stgdaparam23;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam24 = stgdaparam62;
			stgdaparam24.s = "05";
			stgdaparam24.c = 0x65;
			stgdaparamArray[ 0x17 ] = stgdaparam24;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam25 = stgdaparam62;
			stgdaparam25.s = "06";
			stgdaparam25.c = 0x66;
			stgdaparamArray[ 0x18 ] = stgdaparam25;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam26 = stgdaparam62;
			stgdaparam26.s = "07";
			stgdaparam26.c = 0x67;
			stgdaparamArray[ 0x19 ] = stgdaparam26;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam27 = stgdaparam62;
			stgdaparam27.s = "08";
			stgdaparam27.c = 0x68;
			stgdaparamArray[ 0x1a ] = stgdaparam27;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam28 = stgdaparam62;
			stgdaparam28.s = "09";
			stgdaparam28.c = 0x69;
			stgdaparamArray[ 0x1b ] = stgdaparam28;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam29 = stgdaparam62;
			stgdaparam29.s = "0A";
			stgdaparam29.c = 0x70;
			stgdaparamArray[ 0x1c ] = stgdaparam29;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam30 = stgdaparam62;
			stgdaparam30.s = "0B";
			stgdaparam30.c = 0x71;
			stgdaparamArray[ 0x1d ] = stgdaparam30;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam31 = stgdaparam62;
			stgdaparam31.s = "0C";
			stgdaparam31.c = 0x72;
			stgdaparamArray[ 30 ] = stgdaparam31;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam32 = stgdaparam62;
			stgdaparam32.s = "0D";
			stgdaparam32.c = 0x73;
			stgdaparamArray[ 0x1f ] = stgdaparam32;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam33 = stgdaparam62;
			stgdaparam33.s = "0E";
			stgdaparam33.c = 0x74;
			stgdaparamArray[ 0x20 ] = stgdaparam33;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam34 = stgdaparam62;
			stgdaparam34.s = "0F";
			stgdaparam34.c = 0x75;
			stgdaparamArray[ 0x21 ] = stgdaparam34;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam35 = stgdaparam62;
			stgdaparam35.s = "10";
			stgdaparam35.c = 0x76;
			stgdaparamArray[ 0x22 ] = stgdaparam35;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam36 = stgdaparam62;
			stgdaparam36.s = "11";
			stgdaparam36.c = 0x77;
			stgdaparamArray[ 0x23 ] = stgdaparam36;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam37 = stgdaparam62;
			stgdaparam37.s = "12";
			stgdaparam37.c = 120;
			stgdaparamArray[ 0x24 ] = stgdaparam37;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam38 = stgdaparam62;
			stgdaparam38.s = "13";
			stgdaparam38.c = 0x79;
			stgdaparamArray[ 0x25 ] = stgdaparam38;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam39 = stgdaparam62;
			stgdaparam39.s = "14";
			stgdaparam39.c = 0x80;
			stgdaparamArray[ 0x26 ] = stgdaparam39;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam40 = stgdaparam62;
			stgdaparam40.s = "15";
			stgdaparam40.c = 0x81;
			stgdaparamArray[ 0x27 ] = stgdaparam40;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam41 = stgdaparam62;
			stgdaparam41.s = "16";
			stgdaparam41.c = 130;
			stgdaparamArray[ 40 ] = stgdaparam41;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam42 = stgdaparam62;
			stgdaparam42.s = "17";
			stgdaparam42.c = 0x83;
			stgdaparamArray[ 0x29 ] = stgdaparam42;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam43 = stgdaparam62;
			stgdaparam43.s = "18";
			stgdaparam43.c = 0x84;
			stgdaparamArray[ 0x2a ] = stgdaparam43;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam44 = stgdaparam62;
			stgdaparam44.s = "19";
			stgdaparam44.c = 0x85;
			stgdaparamArray[ 0x2b ] = stgdaparam44;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam45 = stgdaparam62;
			stgdaparam45.s = "1A";
			stgdaparam45.c = 0x86;
			stgdaparamArray[ 0x2c ] = stgdaparam45;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam46 = stgdaparam62;
			stgdaparam46.s = "1B";
			stgdaparam46.c = 0x87;
			stgdaparamArray[ 0x2d ] = stgdaparam46;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam47 = stgdaparam62;
			stgdaparam47.s = "1C";
			stgdaparam47.c = 0x88;
			stgdaparamArray[ 0x2e ] = stgdaparam47;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam48 = stgdaparam62;
			stgdaparam48.s = "1D";
			stgdaparam48.c = 0x89;
			stgdaparamArray[ 0x2f ] = stgdaparam48;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam49 = stgdaparam62;
			stgdaparam49.s = "1E";
			stgdaparam49.c = 0x90;
			stgdaparamArray[ 0x30 ] = stgdaparam49;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam50 = stgdaparam62;
			stgdaparam50.s = "1F";
			stgdaparam50.c = 0x91;
			stgdaparamArray[ 0x31 ] = stgdaparam50;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam51 = stgdaparam62;
			stgdaparam51.s = "20";
			stgdaparam51.c = 0x92;
			stgdaparamArray[ 50 ] = stgdaparam51;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam52 = stgdaparam62;
			stgdaparam52.s = "B1";
			stgdaparam52.c = 0xa1;
			stgdaparamArray[ 0x33 ] = stgdaparam52;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam53 = stgdaparam62;
			stgdaparam53.s = "B2";
			stgdaparam53.c = 0xa2;
			stgdaparamArray[ 0x34 ] = stgdaparam53;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam54 = stgdaparam62;
			stgdaparam54.s = "B3";
			stgdaparam54.c = 0xa3;
			stgdaparamArray[ 0x35 ] = stgdaparam54;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam55 = stgdaparam62;
			stgdaparam55.s = "B4";
			stgdaparam55.c = 0xa4;
			stgdaparamArray[ 0x36 ] = stgdaparam55;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam56 = stgdaparam62;
			stgdaparam56.s = "B5";
			stgdaparam56.c = 0xa5;
			stgdaparamArray[ 0x37 ] = stgdaparam56;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam57 = stgdaparam62;
			stgdaparam57.s = "B6";
			stgdaparam57.c = 0xa6;
			stgdaparamArray[ 0x38 ] = stgdaparam57;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam58 = stgdaparam62;
			stgdaparam58.s = "B7";
			stgdaparam58.c = 0xa7;
			stgdaparamArray[ 0x39 ] = stgdaparam58;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam59 = stgdaparam62;
			stgdaparam59.s = "BW";
			stgdaparam59.c = 0xa8;
			stgdaparamArray[ 0x3a ] = stgdaparam59;
			stgdaparam62 = new STGDAPARAM();
			STGDAPARAM stgdaparam60 = stgdaparam62;
			stgdaparam60.s = "G0";
			stgdaparam60.c = 0x20;
			stgdaparamArray[ 0x3b ] = stgdaparam60;
			STGDAPARAM stgdaparam61 = new STGDAPARAM();
			stgdaparam61.s = "B0";
			stgdaparam61.c = 160;
			stgdaparamArray[ 60 ] = stgdaparam61;
#endif
			this.stGDAParam = stgdaparamArray;
			this.nBGMAdjust = 0;

#if TEST_NOTEOFFMODE
			this.bHH演奏で直前のHHを消音する = true;
			this.bGUITAR演奏で直前のGUITARを消音する = true;
			this.bBASS演奏で直前のBASSを消音する = true;
#endif
	
		}
		public CDTX( string str全入力文字列 )
			: this()
		{
			this.On活性化();
			this.t入力・全入力文字列から( str全入力文字列 );
		}
		public CDTX( string strファイル名, bool bヘッダのみ )
			: this()
		{
			this.On活性化();
			this.t入力( strファイル名, bヘッダのみ );
		}
		public CDTX( string str全入力文字列, double db再生速度, int nBGMAdjust )
			: this()
		{
			this.On活性化();
			this.t入力・全入力文字列から( str全入力文字列, db再生速度, nBGMAdjust );
		}
		public CDTX( string strファイル名, bool bヘッダのみ, double db再生速度, int nBGMAdjust )
			: this()
		{
			this.On活性化();
			this.t入力( strファイル名, bヘッダのみ, db再生速度, nBGMAdjust );
		}


		// メソッド

		public int nモニタを考慮した音量( E楽器パート part )
		{
			CConfigIni configIni = CDTXMania.ConfigIni;
			switch( part )
			{
				case E楽器パート.DRUMS:
					if( configIni.b演奏音を強調する.Drums )
					{
						return configIni.n自動再生音量;
					}
					return configIni.n手動再生音量;

				case E楽器パート.GUITAR:
					if( configIni.b演奏音を強調する.Guitar )
					{
						return configIni.n自動再生音量;
					}
					return configIni.n手動再生音量;

				case E楽器パート.BASS:
					if( configIni.b演奏音を強調する.Bass )
					{
						return configIni.n自動再生音量;
					}
					return configIni.n手動再生音量;
			}
			if( ( !configIni.b演奏音を強調する.Drums && !configIni.b演奏音を強調する.Guitar ) && !configIni.b演奏音を強調する.Bass )
			{
				return configIni.n手動再生音量;
			}
			return configIni.n自動再生音量;
		}
		public void tAVIの読み込み()
		{
			if( this.listAVI != null )
			{
				foreach( CAVI cavi in this.listAVI.Values )
				{
					cavi.OnDeviceCreated();
				}
			}
			if( !this.bヘッダのみ )
			{
				foreach( CChip chip in this.listChip )
				{
					if( chip.nチャンネル番号 == 0x54 )
					{
						chip.eAVI種別 = EAVI種別.Unknown;
						chip.rAVI = null;
						chip.rAVIPan = null;
						if( this.listAVIPAN.ContainsKey( chip.n整数値 ) )
						{
							CAVIPAN cavipan = this.listAVIPAN[ chip.n整数値 ];
							if( this.listAVI.ContainsKey( cavipan.nAVI番号 ) && ( this.listAVI[ cavipan.nAVI番号 ].avi != null ) )
							{
								chip.eAVI種別 = EAVI種別.AVIPAN;
								chip.rAVI = this.listAVI[ cavipan.nAVI番号 ];
								chip.rAVIPan = cavipan;
								continue;
							}
						}
						if( this.listAVI.ContainsKey( chip.n整数値 ) && ( this.listAVI[ chip.n整数値 ].avi != null ) )
						{
							chip.eAVI種別 = EAVI種別.AVI;
							chip.rAVI = this.listAVI[ chip.n整数値 ];
						}
					}
				}
			}
		}
		public void tBMP_BMPTEXの読み込み()
		{
			if( this.listBMP != null )
			{
				foreach( CBMP cbmp in this.listBMP.Values )
				{
					cbmp.OnDeviceCreated();
				}
			}
			if( this.listBMPTEX != null )
			{
				foreach( CBMPTEX cbmptex in this.listBMPTEX.Values )
				{
					cbmptex.OnDeviceCreated();
				}
			}
			if( !this.bヘッダのみ )
			{
				foreach( CChip chip in this.listChip )
				{
					if( ( ( ( chip.nチャンネル番号 == 4 ) || ( chip.nチャンネル番号 == 7 ) ) || ( ( chip.nチャンネル番号 >= 0x55 ) && ( chip.nチャンネル番号 <= 0x59 ) ) ) || ( chip.nチャンネル番号 == 0x60 ) )
					{
						chip.eBGA種別 = EBGA種別.Unknown;
						chip.rBMP = null;
						chip.rBMPTEX = null;
						chip.rBGA = null;
						chip.rBGAPan = null;
						if( this.listBGAPAN.ContainsKey( chip.n整数値 ) )
						{
							CBGAPAN cbgapan = this.listBGAPAN[ chip.n整数値 ];
							if( this.listBMPTEX.ContainsKey( cbgapan.nBMP番号 ) && this.listBMPTEX[ cbgapan.nBMP番号 ].bUse )
							{
								chip.eBGA種別 = EBGA種別.BGAPAN;
								chip.rBMPTEX = this.listBMPTEX[ cbgapan.nBMP番号 ];
								chip.rBGAPan = cbgapan;
								continue;
							}
							if( this.listBMP.ContainsKey( cbgapan.nBMP番号 ) && this.listBMP[ cbgapan.nBMP番号 ].bUse )
							{
								chip.eBGA種別 = EBGA種別.BGAPAN;
								chip.rBMP = this.listBMP[ cbgapan.nBMP番号 ];
								chip.rBGAPan = cbgapan;
								continue;
							}
						}
						if( this.listBGA.ContainsKey( chip.n整数値 ) )
						{
							CBGA cbga = this.listBGA[ chip.n整数値 ];
							if( this.listBMPTEX.ContainsKey( cbga.nBMP番号 ) && this.listBMPTEX[ cbga.nBMP番号 ].bUse )
							{
								chip.eBGA種別 = EBGA種別.BGA;
								chip.rBMPTEX = this.listBMPTEX[ cbga.nBMP番号 ];
								chip.rBGA = cbga;
								continue;
							}
							if( this.listBMP.ContainsKey( cbga.nBMP番号 ) && this.listBMP[ cbga.nBMP番号 ].bUse )
							{
								chip.eBGA種別 = EBGA種別.BGA;
								chip.rBMP = this.listBMP[ cbga.nBMP番号 ];
								chip.rBGA = cbga;
								continue;
							}
						}
						if( this.listBMPTEX.ContainsKey( chip.n整数値 ) && this.listBMPTEX[ chip.n整数値 ].bUse )
						{
							chip.eBGA種別 = EBGA種別.BMPTEX;
							chip.rBMPTEX = this.listBMPTEX[ chip.n整数値 ];
							continue;
						}
						if( this.listBMP.ContainsKey( chip.n整数値 ) && this.listBMP[ chip.n整数値 ].bUse )
						{
							chip.eBGA種別 = EBGA種別.BMP;
							chip.rBMP = this.listBMP[ chip.n整数値 ];
							continue;
						}
					}
					if( ( ( ( chip.nチャンネル番号 == 0xc4 ) || ( chip.nチャンネル番号 == 0xc7 ) ) || ( ( chip.nチャンネル番号 >= 0xd5 ) && ( chip.nチャンネル番号 <= 0xd9 ) ) ) || ( chip.nチャンネル番号 == 0xe0 ) )
					{
						chip.eBGA種別 = EBGA種別.Unknown;
						chip.rBMP = null;
						chip.rBMPTEX = null;
						chip.rBGA = null;
						chip.rBGAPan = null;
						if( this.listBMPTEX.ContainsKey( chip.n整数値 ) && this.listBMPTEX[ chip.n整数値 ].bUse )
						{
							chip.eBGA種別 = EBGA種別.BMPTEX;
							chip.rBMPTEX = this.listBMPTEX[ chip.n整数値 ];
						}
						else if( this.listBMP.ContainsKey( chip.n整数値 ) && this.listBMP[ chip.n整数値 ].bUse )
						{
							chip.eBGA種別 = EBGA種別.BMP;
							chip.rBMP = this.listBMP[ chip.n整数値 ];
						}
					}
				}
			}
		}
		public void tWave再生位置自動補正()
		{
			foreach( CWAV cwav in this.listWAV.Values )
			{
				this.tWave再生位置自動補正( cwav );
			}
		}
		public void tWave再生位置自動補正( CWAV wc )
		{
			for( int i = 0; i < 4; i++ )
			{
				if( ( ( wc.rSound[ i ] != null ) && wc.rSound[ i ].b再生中 ) && ( wc.rSound[ i ].n総演奏時間ms >= 5000 ) )
				{
					long nCurrentTime = CDTXMania.Timer.nシステム時刻;
					if( nCurrentTime > wc.n再生開始時刻[ i ] )
					{
						long nAbsTimeFromStartPlaying = nCurrentTime - wc.n再生開始時刻[ i ];
						wc.rSound[ i ].t再生位置を変更する( wc.rSound[ i ].t時刻から位置を返す( nAbsTimeFromStartPlaying ) );
					}
				}
			}
		}
		public void tWavの再生停止( int nWaveの内部番号 )
		{
			if( this.listWAV.ContainsKey( nWaveの内部番号 ) )
			{
				CWAV cwav = this.listWAV[ nWaveの内部番号 ];
				for( int i = 0; i < 4; i++ )
				{
					if( cwav.rSound[ i ] != null )
					{
						cwav.rSound[ i ].t再生を停止する();
					}
				}
			}
		}
		public void tWAVの読み込み()
		{
//			Trace.TraceInformation("WAV files={0}", this.listWAV.Count);
//			int count = 0;
			foreach (CWAV cwav in this.listWAV.Values)
			{
//				string strCount = count.ToString() + " / " + this.listWAV.Count.ToString();
//				Debug.WriteLine(strCount);
//				CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, strCount);
//				count++;

				string str = string.IsNullOrEmpty(this.PATH_WAV) ? this.strフォルダ名 : this.PATH_WAV;
				str = str + cwav.strファイル名;
				try
				{
						try
						{
							cwav.rSound[ 0 ] = CDTXMania.Sound管理.tサウンドを生成する(str);
							cwav.rSound[ 0 ].n音量 = 100;
							if( CDTXMania.ConfigIni.bLog作成解放ログ出力 )
							{
								Trace.TraceInformation( "サウンドを作成しました。({3})({0})({1})({2}bytes)", cwav.strコメント文, str, cwav.rSound[ 0 ].nサウンドバッファサイズ, cwav.rSound[ 0 ].bストリーム再生する ? "Stream" : "OnMemory" );
							}
						}
						catch
						{
							cwav.rSound[ 0 ] = null;
							Trace.TraceError( "サウンドの作成に失敗しました。({0})({1})", cwav.strコメント文, str );
						}
						if ( cwav.rSound[ 0 ] == null || cwav.rSound[ 0 ].bストリーム再生する )
						{
							for ( int j = 1; j < cwav.rSound.GetLength(0); j++ )
							{
								cwav.rSound[ j ] = null;
							}
						}
						else
						{
							for ( int j = 1; j < cwav.rSound.GetLength(0); j++ )
							{
								cwav.rSound[ j ] = (CSound) cwav.rSound[ 0 ].Clone();	// #24007 2011.9.5 yyagi add: to accelerate loading chip sounds
								CDTXMania.Sound管理.tサウンドを登録する( cwav.rSound[ j ] );
							}
						}
				}
				catch( Exception exception )
				{
					Trace.TraceError( "サウンドの生成に失敗しました。({0})({1})({2})", exception.Message, cwav.strコメント文, str );
					for( int j = 0; j < cwav.rSound.GetLength(0); j++ )
					{
						cwav.rSound[ j ] = null;
					}
					continue;
				}
			}
		}
		public static string tZZ( int n )
		{
			if( n < 0 || n >= 36 * 36 )
				return "!!";	// オーバー／アンダーフロー。

			// n を36進数2桁の文字列にして返す。

			string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string( new char[] { str[ n / 36 ], str[ n % 36 ] } );
		}
		public void tギターとベースのランダム化(E楽器パート part, Eランダムモード eRandom)
		{
			if( ( ( part == E楽器パート.GUITAR ) || ( part == E楽器パート.BASS ) ) && ( eRandom != Eランダムモード.OFF ) )
			{
				int[,] nランダムレーン候補 = new int[ , ] { { 0, 1, 2, 3, 4, 5, 6, 7 }, { 0, 2, 1, 3, 4, 6, 5, 7 }, { 0, 1, 4, 5, 2, 3, 6, 7 }, { 0, 2, 4, 6, 1, 3, 5, 7 }, { 0, 4, 1, 5, 2, 6, 3, 7 }, { 0, 4, 2, 6, 1, 5, 3, 7 } };
				int n小節番号 = -10000;
				int n小節内乱数6通り = 0;
				// int GOTO_END = 0;	// gotoの飛び先のダミーコードで使うダミー変数
				foreach( CChip chip in this.listChip )
				{
					int nRGBレーンビットパターン;
					int n新RGBレーンビットパターン = 0;				// 「未割り当てのローカル変数」ビルドエラー回避のために0を初期値に設定
					bool flag;
					if( ( chip.n発声位置 / 384 ) != n小節番号 )		// 小節が変化したら
					{
						n小節番号 = chip.n発声位置 / 384;
						n小節内乱数6通り = CDTXMania.Random.Next( 6 );
					}
					int nランダム化前チャンネル番号 = chip.nチャンネル番号;
					if(		( ( ( part != E楽器パート.GUITAR ) || ( 0x20 > nランダム化前チャンネル番号 ) ) || ( nランダム化前チャンネル番号 > 0x27 ) )
						&&	( ( ( part != E楽器パート.BASS )   || ( 0xA0 > nランダム化前チャンネル番号 ) ) || ( nランダム化前チャンネル番号 > 0xa7 ) )
					)
					{
						continue;
					}
					switch( eRandom )
					{
						case Eランダムモード.RANDOM:		// 1小節単位でレーンのR/G/Bがランダムに入れ替わる
							chip.nチャンネル番号 = ( nランダム化前チャンネル番号 & 0xF0 ) | nランダムレーン候補[ n小節内乱数6通り, nランダム化前チャンネル番号 & 0x07 ];
							continue;	// goto Label_02C4;

						case Eランダムモード.SUPERRANDOM:	// チップごとにR/G/Bがランダムで入れ替わる(レーンの本数までは変わらない)。
							chip.nチャンネル番号 = ( nランダム化前チャンネル番号 & 0xF0 ) | nランダムレーン候補[ CDTXMania.Random.Next( 6 ), nランダム化前チャンネル番号 & 0x07 ];
							continue;	// goto Label_02C4;

						case Eランダムモード.HYPERRANDOM:	// レーンの本数も変わる
							nRGBレーンビットパターン = nランダム化前チャンネル番号 & 7;
							// n新RGBレーンビットパターン = (int)Eレーンビットパターン.OPEN;	// この値は結局未使用なので削除
							flag = ((part == E楽器パート.GUITAR) && this.bチップがある.OpenGuitar) || ((part == E楽器パート.BASS) && this.bチップがある.OpenBass);	// #23546 2010.10.28 yyagi fixed (bチップがある.Bass→bチップがある.OpenBass)
							if (((nRGBレーンビットパターン != (int)Eレーンビットパターン.xxB) && (nRGBレーンビットパターン != (int)Eレーンビットパターン.xGx)) && (nRGBレーンビットパターン != (int)Eレーンビットパターン.Rxx))		// xxB, xGx, Rxx レーン1本相当
							{
								break;															// レーン1本相当でなければ、とりあえず先に進む
							}
							n新RGBレーンビットパターン = CDTXMania.Random.Next( 6 ) + 1;		// レーン1本相当なら、レーン1本か2本(1～6)に変化して終了
							goto Label_02B2;

						default:
							continue;	// goto Label_02C4;
					}
					switch( nRGBレーンビットパターン )
					{
						case (int)Eレーンビットパターン.xGB:	// xGB	レーン2本相当
						case (int)Eレーンビットパターン.RxB:	// RxB
						case (int)Eレーンビットパターン.RGx:	// RGx
							n新RGBレーンビットパターン = flag ? CDTXMania.Random.Next( 8 ) : ( CDTXMania.Random.Next( 7 ) + 1 );	// OPENあり譜面ならOPENを含むランダム, OPENなし譜面ならOPENを含まないランダム
							break;	// goto Label_02B2;

						default:
							if( nRGBレーンビットパターン == (int)Eレーンビットパターン.RGB )	// RGB レーン3本相当
							{
								if( flag )	// OPENあり譜面の場合
								{
									int n乱数パーセント = CDTXMania.Random.Next( 100 );
									if( n乱数パーセント < 30 )
									{
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.OPEN;
									}
									else if( n乱数パーセント < 60 )
									{
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.RGB;
									}
									else if( n乱数パーセント < 85 )
									{
										switch( CDTXMania.Random.Next( 3 ) )
										{
											case 0:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xGB;
												break;	// goto Label_02B2;

											case 1:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.RxB;
												break;	// goto Label_02B2;
										}
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.RGx;
									}
									else	// OPENでない場合
									{
										switch( CDTXMania.Random.Next( 3 ) )
										{
											case 0:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xxB;
												break;	// goto Label_02B2;

											case 1:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xGx;
												break;	// goto Label_02B2;
										}
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.Rxx;
									}
								}
								else	// OPENなし譜面の場合
								{
									int n乱数パーセント = CDTXMania.Random.Next( 100 );
									if( n乱数パーセント < 60 )
									{
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.RGB;
									}
									else if( n乱数パーセント < 85 )
									{
										switch( CDTXMania.Random.Next( 3 ) )
										{
											case 0:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xGB;
												break;	// goto Label_02B2;

											case 1:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.RxB;
												break;	// goto Label_02B2;
										}
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.RGx;
									}
									else
									{
										switch( CDTXMania.Random.Next( 3 ) )
										{
											case 0:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xxB;
												break;	// goto Label_02B2;

											case 1:
												n新RGBレーンビットパターン = (int)Eレーンビットパターン.xGx;
												break;	// goto Label_02B2;
										}
										n新RGBレーンビットパターン = (int)Eレーンビットパターン.Rxx;
									}
								}
							}
							break;	// goto Label_02B2;
					}
				Label_02B2:
					chip.nチャンネル番号 = ( nランダム化前チャンネル番号 & 0xF0 ) | n新RGBレーンビットパターン;
//				Label_02C4:
//					GOTO_END++;		// goto用のダミーコード
				}
			}
		}
		public void tチップの再生( CChip rChip, long n再生開始システム時刻ms, int nLane )
		{
			this.tチップの再生( rChip, n再生開始システム時刻ms, nLane, CDTXMania.ConfigIni.n自動再生音量, false, false );
		}
		public void tチップの再生( CChip rChip, long n再生開始システム時刻ms, int nLane, int nVol )
		{
			this.tチップの再生( rChip, n再生開始システム時刻ms, nLane, nVol, false, false );
		}
		public void tチップの再生( CChip rChip, long n再生開始システム時刻ms, int nLane, int nVol, bool bMIDIMonitor )
		{
			this.tチップの再生( rChip, n再生開始システム時刻ms, nLane, nVol, bMIDIMonitor, false );
		}
		public void tチップの再生( CChip pChip, long n再生開始システム時刻ms, int nLane, int nVol, bool bMIDIMonitor, bool bBad )
		{
			if( pChip.n整数値・内部番号 >= 0 )
			{
				if( ( nLane < (int) Eレーン.LC ) || ( (int) Eレーン.BGM < nLane ) )
				{
					throw new ArgumentOutOfRangeException();
				}
				if( this.listWAV.ContainsKey( pChip.n整数値・内部番号 ) )
				{
					CWAV wc = this.listWAV[ pChip.n整数値・内部番号 ];
					int index = wc.n現在再生中のサウンド番号 = ( wc.n現在再生中のサウンド番号 + 1 ) % 4;
					if( ( wc.rSound[ 0 ] != null ) && wc.rSound[ 0 ].bストリーム再生する )
					{
						index = wc.n現在再生中のサウンド番号 = 0;
					}
					CSound sound = wc.rSound[ index ];
					if( sound != null )
					{
						sound.n音量 = (int) ( ( (double) ( nVol * wc.n音量 ) ) / 100.0 );
						sound.n位置 = wc.n位置;
						if( bBad )
						{
							sound.db周波数倍率 = ( (float) ( 100 + ( ( ( CDTXMania.Random.Next( 3 ) + 1 ) * 7 ) * ( 1 - ( CDTXMania.Random.Next( 2 ) * 2 ) ) ) ) ) / 100f;
						}
						else
						{
							sound.db周波数倍率 = 1.0;
						}
						sound.db再生速度 = ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0;
						sound.t再生を開始する();
					}
					wc.n再生開始時刻[ wc.n現在再生中のサウンド番号 ] = n再生開始システム時刻ms;
					this.tWave再生位置自動補正( wc );
				}
			}
		}
		public void t各自動再生音チップの再生時刻を変更する( int nBGMAdjustの増減値 )
		{
			this.nBGMAdjust += nBGMAdjustの増減値;
			for( int i = 0; i < this.listChip.Count; i++ )
			{
				int nChannelNumber = this.listChip[ i ].nチャンネル番号;
				if( ( (
						( nChannelNumber == 1 ) ||
						( ( 0x61 <= nChannelNumber ) && ( nChannelNumber <= 0x69 ) )
					  ) ||
						( ( 0x70 <= nChannelNumber ) && ( nChannelNumber <= 0x79 ) )
					) ||
					( ( ( 0x80 <= nChannelNumber ) && ( nChannelNumber <= 0x89 ) ) || ( ( 0x90 <= nChannelNumber ) && ( nChannelNumber <= 0x92 ) ) )
				  )
				{
					this.listChip[ i ].n発声時刻ms += nBGMAdjustの増減値;
				}
			}
			foreach( CWAV cwav in this.listWAV.Values )
			{
				for( int j = 0; j < 4; j++ )
				{
					if( ( cwav.rSound[ j ] != null ) && cwav.rSound[ j ].b再生中 )
					{
						cwav.n再生開始時刻[ j ] += nBGMAdjustの増減値;
					}
				}
			}
		}
		public void t全チップの再生一時停止()
		{
			foreach( CWAV cwav in this.listWAV.Values )
			{
				for( int i = 0; i < 4; i++ )
				{
					if( ( cwav.rSound[ i ] != null ) && cwav.rSound[ i ].b再生中 )
					{
						cwav.rSound[ i ].t再生を一時停止する();
						cwav.n一時停止時刻[ i ] = CDTXMania.Timer.nシステム時刻;
					}
				}
			}
		}
		public void t全チップの再生再開()
		{
			foreach( CWAV cwav in this.listWAV.Values )
			{
				for( int i = 0; i < 4; i++ )
				{
					if( ( cwav.rSound[ i ] != null ) && cwav.rSound[ i ].b一時停止中 )
					{
						long num1 = cwav.n一時停止時刻[ i ];
						long num2 = cwav.n再生開始時刻[ i ];
						cwav.rSound[ i ].t再生を再開する( cwav.n一時停止時刻[ i ] - cwav.n再生開始時刻[ i ] );
						cwav.n再生開始時刻[ i ] += CDTXMania.Timer.nシステム時刻 - cwav.n一時停止時刻[ i ];
					}
				}
			}
		}
		public void t全チップの再生停止()
		{
			foreach( CWAV cwav in this.listWAV.Values )
			{
				this.tWavの再生停止( cwav.n内部番号 );
			}
		}
		public void t入力( string strファイル名, bool bヘッダのみ )
		{
			this.t入力( strファイル名, bヘッダのみ, 1.0, 0 );
		}
		public void t入力( string strファイル名, bool bヘッダのみ, double db再生速度, int nBGMAdjust )
		{
			this.bヘッダのみ = bヘッダのみ;
			this.strファイル名の絶対パス = Path.GetFullPath( strファイル名 );
			this.strファイル名 = Path.GetFileName( this.strファイル名の絶対パス );
			this.strフォルダ名 = Path.GetDirectoryName( this.strファイル名の絶対パス ) + @"\";
			string ext = Path.GetExtension( this.strファイル名 ).ToLower();
			if( ext != null )
			{
				if( !( ext == ".dtx" ) )
				{
					if( ext == ".gda" )
					{
						this.e種別 = E種別.GDA;
					}
					else if( ext == ".g2d" )
					{
						this.e種別 = E種別.G2D;
					}
					else if( ext == ".bms" )
					{
						this.e種別 = E種別.BMS;
					}
					else if( ext == ".bme" )
					{
						this.e種別 = E種別.BME;
					}
					else if( ext == ".mid" )
					{
						this.e種別 = E種別.SMF;
					}
				}
				else
				{
					this.e種別 = E種別.DTX;
				}
			}
			if( this.e種別 != E種別.SMF )
			{
				StreamReader reader = new StreamReader( strファイル名, Encoding.GetEncoding( "shift-jis" ) );
				string str2 = reader.ReadToEnd();
				reader.Close();
				this.t入力・全入力文字列から( str2, db再生速度, nBGMAdjust );
			}
			else
			{
				Trace.TraceWarning( "SMF の演奏は未対応です。（検討中）" );
			}
		}
		public void t入力・全入力文字列から( string str全入力文字列 )
		{
			this.t入力・全入力文字列から( str全入力文字列, 1.0, 0 );
		}
		public unsafe void t入力・全入力文字列から( string str全入力文字列, double db再生速度, int nBGMAdjust )
		{
			if( !string.IsNullOrEmpty( str全入力文字列 ) )
			{
				this.db再生速度 = db再生速度;
				str全入力文字列 = str全入力文字列.Replace( Environment.NewLine, "\n" );
				str全入力文字列 = str全入力文字列.Replace( '\t', ' ' );
				str全入力文字列 = str全入力文字列 + "\n";
				for( int j = 0; j < 36 * 36; j++ )
				{
					this.n無限管理WAV[ j ] = -j;
					this.n無限管理BPM[ j ] = -j;
					this.n無限管理VOL[ j ] = -j;
					this.n無限管理PAN[ j ] = -10000 - j;
					this.n無限管理SIZE[ j ] = -j;
				}
				this.n内部番号WAV1to = 1;
				this.n内部番号BPM1to = 1;
				this.bstackIFからENDIFをスキップする = new Stack<bool>();
				this.bstackIFからENDIFをスキップする.Push( false );
				this.n現在の乱数 = 0;
				for( int k = 0; k < 7; k++ )
				{
					this.nRESULTIMAGE用優先順位[ k ] = 0;
					this.nRESULTMOVIE用優先順位[ k ] = 0;
					this.nRESULTSOUND用優先順位[ k ] = 0;
				}
				CharEnumerator ce = str全入力文字列.GetEnumerator();
				if( ce.MoveNext() )
				{
					this.n現在の行数 = 1;
					do
					{
						if( !this.t入力・空白と改行をスキップする( ref ce ) )
						{
							break;
						}
						if( ce.Current == '#' )
						{
							if( ce.MoveNext() )
							{
								StringBuilder builder = new StringBuilder( 0x20 );
								if( this.t入力・コマンド文字列を抜き出す( ref ce, ref builder ) )
								{
									StringBuilder builder2 = new StringBuilder( 0x400 );
									if( this.t入力・パラメータ文字列を抜き出す( ref ce, ref builder2 ) )
									{
										StringBuilder builder3 = new StringBuilder( 0x400 );
										if( this.t入力・コメント文字列を抜き出す( ref ce, ref builder3 ) )
										{
											this.t入力・行解析( ref builder, ref builder2, ref builder3 );
											this.n現在の行数++;
											continue;
										}
									}
								}
							}
							break;
						}
					}
					while( this.t入力・コメントをスキップする( ref ce ) );
					this.n無限管理WAV = null;
					this.n無限管理BPM = null;
					this.n無限管理VOL = null;
					this.n無限管理PAN = null;
					this.n無限管理SIZE = null;
					if( !this.bヘッダのみ )
					{
						int num26;
						CBPM cbpm = null;
						foreach( CBPM cbpm2 in this.listBPM.Values )
						{
							if( cbpm2.n表記上の番号 == 0 )
							{
								cbpm = cbpm2;
								break;
							}
						}
						if( cbpm == null )
						{
							cbpm = new CBPM();
							cbpm.n内部番号 = this.n内部番号BPM1to++;
							cbpm.n表記上の番号 = 0;
							cbpm.dbBPM値 = 120.0;
							this.listBPM.Add( cbpm.n内部番号, cbpm );
							CChip item = new CChip();
							item.n発声位置 = 0;
							item.nチャンネル番号 = 8;
							item.n整数値 = 0;
							item.n整数値・内部番号 = cbpm.n内部番号;
							this.listChip.Add( item );
						}
						else
						{
							CChip chip2 = new CChip();
							chip2.n発声位置 = 0;
							chip2.nチャンネル番号 = 8;
							chip2.n整数値 = 0;
							chip2.n整数値・内部番号 = cbpm.n内部番号;
							this.listChip.Add( chip2 );
						}
						if( this.listBMP.ContainsKey( 0 ) )
						{
							CChip chip4 = new CChip();
							chip4.n発声位置 = 0;
							chip4.nチャンネル番号 = 4;
							chip4.n整数値 = 0;
							chip4.n整数値・内部番号 = 0;
							CChip chip3 = chip4;
							this.listChip.Add( chip3 );
						}
						foreach( CWAV cwav in this.listWAV.Values )
						{
							if( cwav.nチップサイズ < 0 )
							{
								cwav.nチップサイズ = 100;
							}
							if( cwav.n位置 <= -10000 )
							{
								cwav.n位置 = 0;
							}
							if( cwav.n音量 < 0 )
							{
								cwav.n音量 = 100;
							}
						}
						foreach( CWAV cwav2 in this.listWAV.Values )
						{
							foreach( CChip chip5 in this.listChip )
							{
								if( chip5.n整数値・内部番号 == cwav2.n内部番号 )
								{
									chip5.dbチップサイズ倍率 = ( (double) cwav2.nチップサイズ ) / 100.0;
								}
							}
						}
						for ( int m = 0xb1; m <= 0xbc; m++ )			// #28146 2012.4.21 yyagi; bb -> bc
						{
							foreach( CChip chip6 in this.listChip )
							{
								if( chip6.nチャンネル番号 == m )
								{
									CChip chip7 = new CChip();
									chip7.n発声位置 = 0;
									chip7.nチャンネル番号 = chip6.nチャンネル番号;
									chip7.n整数値 = chip6.n整数値;
									chip7.n整数値・内部番号 = chip6.n整数値・内部番号;
									this.listChip.Add( chip7 );
									break;
								}
							}
						}
						if( this.listChip.Count > 0 )
						{
							this.listChip.Sort();
							double num4 = 1.0;
							int num5 = ( this.listChip[ this.listChip.Count - 1 ].n発声位置 + 384 ) - ( this.listChip[ this.listChip.Count - 1 ].n発声位置 % 384 );
							for( int num6 = 0; num6 <= num5; num6 += 384 )
							{
								CChip chip8 = new CChip();
								chip8.n発声位置 = num6;
								chip8.nチャンネル番号 = 0x50;
								chip8.n整数値 = 36 * 36 - 1;
								this.listChip.Add( chip8 );
							}
							this.listChip.Sort();
							int num7 = 0;
							int num8 = 0;
							for( int num9 = 0; num9 < num5; num9 += 384 )
							{
								int num10 = 0;
								while( ( num8 < this.listChip.Count ) && ( this.listChip[ num8 ].n発声位置 < ( num9 + 384 ) ) )
								{
									if( this.listChip[ num8 ].nチャンネル番号 == 0xc1 )
									{
										num10 = this.listChip[ num8 ].n発声位置 - num9;
									}
									num8++;
								}
								if( ( this.e種別 == E種別.BMS ) || ( this.e種別 == E種別.BME ) )
								{
									num4 = 1.0;
								}
								while( ( num7 < this.listChip.Count ) && ( this.listChip[ num7 ].n発声位置 <= num9 ) )
								{
									if( this.listChip[ num7 ].nチャンネル番号 == 2 )
									{
										num4 = this.listChip[ num7 ].db実数値;
									}
									num7++;
								}
								for( int num11 = 0; num11 < 100; num11++ )
								{
									int num12 = (int) ( ( (double) ( 384 * num11 ) ) / ( 4.0 * num4 ) );
									if( ( num12 + num10 ) >= 384 )
									{
										break;
									}
									if( ( ( num12 + num10 ) % 384 ) != 0 )
									{
										CChip chip9 = new CChip();
										chip9.n発声位置 = ( num9 + num12 ) + num10;
										chip9.nチャンネル番号 = 0x51;
										chip9.n整数値 = 36 * 36 - 1;
										this.listChip.Add( chip9 );
									}
								}
							}
							this.listChip.Sort();
						}
						#region [ C2 [拍線・小節線表示指定] の処理 ]		// #28145 2012.4.21 yyagi; 2重ループをほぼ1重にして高速化
						bool bShowBeatBarLine = true;
						for ( int i = 0; i < this.listChip.Count; i++ )
						{
							bool bChangedBeatBarStatus = false;
							if ( ( this.listChip[ i ].nチャンネル番号 == 0xc2 ) )
							{
								if ( this.listChip[ i ].n整数値 == 1 )				// BAR/BEAT LINE = ON
								{
									bShowBeatBarLine = true;
									bChangedBeatBarStatus = true;
								}
								else if ( this.listChip[ i ].n整数値 == 2 )			// BAR/BEAT LINE = OFF
								{
									bShowBeatBarLine = false;
									bChangedBeatBarStatus = true;
								}
							}
							int startIndex = i;
							if ( bChangedBeatBarStatus )							// C2チップの前に50/51チップが来ている可能性に配慮
							{
								while ( startIndex > 0 && this.listChip[ startIndex ].n発声位置 == this.listChip[ i ].n発声位置 )
								{
									startIndex--;
								}
								startIndex++;	// 1つ小さく過ぎているので、戻す
							}
							for ( int j = startIndex; j <= i; j++ ) 
							{
								if ( ( ( this.listChip[ j ].nチャンネル番号 == 0x50 ) || ( this.listChip[ j ].nチャンネル番号 == 0x51 ) ) &&
									( this.listChip[ j ].n整数値 == ( 36 * 36 - 1 ) ) )
								{
									this.listChip[ j ].b可視 = bShowBeatBarLine;
								}
							}
						}
						#endregion
						double bpm = 120.0;
						double num15 = 1.0;
						int num16 = 0;
						int num17 = 0;
						int nBar = 0;
						foreach( CChip chip10 in this.listChip )
						{
							chip10.n発声時刻ms = num17 + ( (int) ( ( ( 0x271 * ( chip10.n発声位置 - num16 ) ) * num15 ) / bpm ) );
							if( ( ( this.e種別 == E種別.BMS ) || ( this.e種別 == E種別.BME ) ) && ( ( num15 != 1.0 ) && ( ( chip10.n発声位置 / 384) != nBar ) ) )
							{
								num16 = chip10.n発声位置;
								num17 = chip10.n発声時刻ms;
								num15 = 1.0;
							}
							nBar = chip10.n発声位置 / 384;
							num26 = chip10.nチャンネル番号;
							switch( num26 )
							{
								case 0x02:
									{
										num16 = chip10.n発声位置;
										num17 = chip10.n発声時刻ms;
										num15 = chip10.db実数値;
										continue;
									}
								case 0x03:
									{
										num16 = chip10.n発声位置;
										num17 = chip10.n発声時刻ms;
										bpm = this.BASEBPM + chip10.n整数値;
										continue;
									}
								case 0x04:
								case 0x07:
								case 0x55:
								case 0x56:
								case 0x57:
								case 0x58:
								case 0x59:
								case 0x60:
									break;

								case 0x05:
								case 0x06:
								case 0x5A:
								case 0x5b:
								case 0x5c:
								case 0x5d:
								case 0x5e:
								case 0x5f:
									{
										continue;
									}
								case 8:
									{
										num16 = chip10.n発声位置;
										num17 = chip10.n発声時刻ms;
										if( this.listBPM.ContainsKey( chip10.n整数値・内部番号 ) )
										{
											bpm = ( ( this.listBPM[ chip10.n整数値・内部番号 ].n表記上の番号 == 0 ) ? 0.0 : this.BASEBPM ) + this.listBPM[ chip10.n整数値・内部番号 ].dbBPM値;
										}
										continue;
									}
								case 0x54:
									{
										if( this.listAVIPAN.ContainsKey( chip10.n整数値 ) )
										{
											int num21 = num17 + ( (int) ( ( ( 0x271 * ( chip10.n発声位置 - num16 ) ) * num15 ) / bpm ) );
											int num22 = num17 + ( (int) ( ( ( 0x271 * ( ( chip10.n発声位置 + this.listAVIPAN[ chip10.n整数値 ].n移動時間ct ) - num16 ) ) * num15 ) / bpm ) );
											chip10.n総移動時間 = num22 - num21;
										}
										continue;
									}
								default:
									{
										continue;
									}
							}
							if( this.listBGAPAN.ContainsKey( chip10.n整数値 ) )
							{
								int num19 = num17 + ( (int) ( ( ( 0x271 * ( chip10.n発声位置 - num16 ) ) * num15 ) / bpm ) );
								int num20 = num17 + ( (int) ( ( ( 0x271 * ( ( chip10.n発声位置 + this.listBGAPAN[ chip10.n整数値 ].n移動時間ct ) - num16 ) ) * num15 ) / bpm ) );
								chip10.n総移動時間 = num20 - num19;
							}
						}
						if( this.db再生速度 > 0.0 )
						{
							foreach( CChip chip11 in this.listChip )
							{
								chip11.n発声時刻ms = (int) ( ( (double) chip11.n発声時刻ms ) / this.db再生速度 );
							}
						}
						this.nBGMAdjust = 0;
						this.t各自動再生音チップの再生時刻を変更する( nBGMAdjust );
						for( int n = 0; n < 12; n++ )
						{
							this.n可視チップ数[ n ] = 0;
						}
						foreach( CChip chip12 in this.listChip )
						{
							int num24 = chip12.nチャンネル番号;
							if( ( 0x11 <= num24 ) && ( num24 <= 0x1a ) )
							{
								this.n可視チップ数[ num24 - 0x11 ]++;
							}
							if( ( 0x20 <= num24 ) && ( num24 <= 0x27 ) )
							{
								this.n可視チップ数.Guitar++;
							}
							if( ( 0xA0 <= num24 ) && ( num24 <= 0xa7 ) )
							{
								this.n可視チップ数.Bass++;
							}
						}
						foreach( CChip chip13 in this.listChip )
						{
							if( ( chip13.bWAVを使うチャンネルである && this.listWAV.ContainsKey( chip13.n整数値・内部番号 ) ) && !this.listWAV[ chip13.n整数値・内部番号 ].listこのWAVを使用するチャンネル番号の集合.Contains( chip13.nチャンネル番号 ) )
							{
								this.listWAV[ chip13.n整数値・内部番号 ].listこのWAVを使用するチャンネル番号の集合.Add( chip13.nチャンネル番号 );
							}
						}
						byte[] buffer = null;
						try
						{
							FileStream stream = new FileStream( this.strファイル名の絶対パス, FileMode.Open, FileAccess.Read );
							buffer = new byte[ stream.Length ];
							stream.Read( buffer, 0, (int) stream.Length );
							stream.Close();
						}
						catch( Exception exception )
						{
							Trace.TraceError( exception.Message );
							Trace.TraceError( "DTXのハッシュの計算に失敗しました。({0})", this.strファイル名の絶対パス );
						}
						if( buffer != null )
						{
							byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash( buffer );
							StringBuilder builder4 = new StringBuilder();
							foreach( byte num25 in buffer2 )
							{
								builder4.Append( num25.ToString( "x2" ) );
							}
							this.strハッシュofDTXファイル = builder4.ToString();
						}
						else
						{
							this.strハッシュofDTXファイル = "00000000000000000000000000000000";
						}
						#region [ bLogDTX詳細ログ出力 ]
						if ( CDTXMania.ConfigIni.bLogDTX詳細ログ出力 )
						{
							foreach( CWAV cwav3 in this.listWAV.Values )
							{
								Trace.TraceInformation( cwav3.ToString() );
							}
							foreach( CAVI cavi in this.listAVI.Values )
							{
								Trace.TraceInformation( cavi.ToString() );
							}
							foreach( CAVIPAN cavipan in this.listAVIPAN.Values )
							{
								Trace.TraceInformation( cavipan.ToString() );
							}
							foreach( CBGA cbga in this.listBGA.Values )
							{
								Trace.TraceInformation( cbga.ToString() );
							}
							foreach( CBGAPAN cbgapan in this.listBGAPAN.Values )
							{
								Trace.TraceInformation( cbgapan.ToString() );
							}
							foreach( CBMP cbmp in this.listBMP.Values )
							{
								Trace.TraceInformation( cbmp.ToString() );
							}
							foreach( CBMPTEX cbmptex in this.listBMPTEX.Values )
							{
								Trace.TraceInformation( cbmptex.ToString() );
							}
							foreach( CBPM cbpm3 in this.listBPM.Values )
							{
								Trace.TraceInformation( cbpm3.ToString() );
							}
							foreach( CChip chip14 in this.listChip )
							{
								Trace.TraceInformation( chip14.ToString() );
							}
						}
						#endregion
					}
				}
			}
		}

		/// <summary>
		/// Swap infos between Guitar and Bass (notes, level, n可視チップ数, bチップがある)
		/// </summary>
		public void SwapGuitarBassInfos()						// #24063 2011.1.24 yyagi ギターとベースの譜面情報入替
		{
			for (int i = this.listChip.Count - 1; i >= 0; i--) {
				if (listChip[i].e楽器パート == E楽器パート.BASS) {
					listChip[i].e楽器パート = E楽器パート.GUITAR;
					listChip[i].nチャンネル番号 -= ( 0xA0 - 0x20 );
				}
				else if ( listChip[i].e楽器パート == E楽器パート.GUITAR )
				{
					listChip[i].e楽器パート = E楽器パート.BASS;
					listChip[i].nチャンネル番号 += ( 0xA0 - 0x20 );
				}
				else if ( listChip[ i ].nチャンネル番号 == 0x28 )		// #25215 2011.5.21 yyagi wailingはE楽器パート.UNKNOWNが割り当てられているので個別に対応
				{
					listChip[ i ].nチャンネル番号 += ( 0xA0 - 0x20 );
				}
				else if ( listChip[ i ].nチャンネル番号 == 0xA8 )		// #25215 2011.5.21 yyagi wailingはE楽器パート.UNKNOWNが割り当てられているので個別に対応
				{
					listChip[ i ].nチャンネル番号 -= ( 0xA0 - 0x20 );
				}
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
		public void SwapGuitarBassInfos_AutoFlags()
		{
		    bool ts = CDTXMania.ConfigIni.bAutoPlay.Bass;			// #24415 2011.2.21 yyagi: FLIP時のリザルトにAUTOの記録が混ざらないよう、AUTOのフラグもswapする
		    CDTXMania.ConfigIni.bAutoPlay.Bass = CDTXMania.ConfigIni.bAutoPlay.Guitar;
		    CDTXMania.ConfigIni.bAutoPlay.Guitar = ts;

		    CDTXMania.ConfigIni.bIsSwappedGuitarBass_AutoFlagsAreSwapped = !CDTXMania.ConfigIni.bIsSwappedGuitarBass_AutoFlagsAreSwapped;
		}

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
			if( this.listWAV != null )
			{
				foreach( CWAV cwav in this.listWAV.Values )
				{
					cwav.Dispose();
				}
				this.listWAV = null;
			}
			if( this.listBMP != null )
			{
				foreach( CBMP cbmp in this.listBMP.Values )
				{
					cbmp.Dispose();
				}
				this.listBMP = null;
			}
			if( this.listBMPTEX != null )
			{
				foreach( CBMPTEX cbmptex in this.listBMPTEX.Values )
				{
					cbmptex.Dispose();
				}
				this.listBMPTEX = null;
			}
			if( this.listAVI != null )
			{
				foreach( CAVI cavi in this.listAVI.Values )
				{
					cavi.Dispose();
				}
				this.listAVI = null;
			}
			if( this.listBPM != null )
			{
				this.listBPM.Clear();
				this.listBPM = null;
			}
			if( this.listBGAPAN != null )
			{
				this.listBGAPAN.Clear();
				this.listBGAPAN = null;
			}
			if( this.listBGA != null )
			{
				this.listBGA.Clear();
				this.listBGA = null;
			}
			if( this.listAVIPAN != null )
			{
				this.listAVIPAN.Clear();
				this.listAVIPAN = null;
			}
			if( this.listChip != null )
			{
				this.listChip.Clear();
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tBMP_BMPTEXの読み込み();
				this.tAVIの読み込み();
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				if( this.listBMP != null )
				{
					foreach( CBMP cbmp in this.listBMP.Values )
					{
						cbmp.Dispose();
					}
				}
				if( this.listBMPTEX != null )
				{
					foreach( CBMPTEX cbmptex in this.listBMPTEX.Values )
					{
						cbmptex.Dispose();
					}
				}
				if( this.listAVI != null )
				{
					foreach( CAVI cavi in this.listAVI.Values )
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
		[StructLayout( LayoutKind.Sequential )]
		private struct STGDAPARAM
		{
			public string strGDAのチャンネル文字列;	
			public int nDTXのチャンネル番号;

			public STGDAPARAM( string strGDAのチャンネル文字列, int nDTXのチャンネル番号 )		// 2011.1.1 yyagi 構造体のコンストラクタ追加(初期化簡易化のため)
			{
				this.strGDAのチャンネル文字列 = strGDAのチャンネル文字列;
				this.nDTXのチャンネル番号 = nDTXのチャンネル番号;
			}
		}

		private readonly STGDAPARAM[] stGDAParam;
		private bool bヘッダのみ;
		private Stack<bool> bstackIFからENDIFをスキップする;
	
		private int n現在の行数;
		private int n現在の乱数;

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

		private bool t入力・コマンド文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( !this.t入力・空白をスキップする( ref ce ) )
				return false;	// 文字が尽きた

			#region [ コマンド終端文字(':')、半角空白、コメント開始文字(';')、改行のいずれかが出現するまでをコマンド文字列と見なし、sb文字列 にコピーする。]
			//-----------------
			while( ce.Current != ':' && ce.Current != ' ' && ce.Current != ';' && ce.Current != '\n' )
			{
				sb文字列.Append( ce.Current );

				if( !ce.MoveNext() )
					return false;	// 文字が尽きた
			}
			//-----------------
			#endregion

			#region [ コマンド終端文字(':')で終端したなら、その次から空白をスキップしておく。]
			//-----------------
			if( ce.Current == ':' )
			{
				if( !ce.MoveNext() )
					return false;	// 文字が尽きた

				if( !this.t入力・空白をスキップする( ref ce ) )
					return false;	// 文字が尽きた
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・コメントをスキップする( ref CharEnumerator ce )
		{
			// 改行が現れるまでをコメントと見なしてスキップする。

			while( ce.Current != '\n' )
			{
				if( !ce.MoveNext() )
					return false;	// 文字が尽きた
			}

			// 改行の次の文字へ移動した結果を返す。

			return ce.MoveNext();
		}
		private bool t入力・コメント文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( ce.Current != ';' )		// コメント開始文字(';')じゃなければ正常帰還。
				return true;

			if( !ce.MoveNext() )		// ';' の次で文字列が終わってたら終了帰還。
				return false;

			#region [ ';' の次の文字から '\n' の１つ前までをコメント文字列と見なし、sb文字列にコピーする。]
			//-----------------
			while( ce.Current != '\n' )
			{
				sb文字列.Append( ce.Current );

				if( !ce.MoveNext() )
					return false;
			}
			//-----------------
			#endregion

			return true;
		}
		private void t入力・パラメータ食い込みチェック( string strコマンド名, ref string strコマンド, ref string strパラメータ )
		{
			if( ( strコマンド.Length > strコマンド名.Length ) && strコマンド.StartsWith( strコマンド名, StringComparison.OrdinalIgnoreCase ) )
			{
				strパラメータ = strコマンド.Substring( strコマンド名.Length ).Trim();
				strコマンド = strコマンド.Substring( 0, strコマンド名.Length );
			}
		}
		private bool t入力・パラメータ文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( !this.t入力・空白をスキップする( ref ce ) )
				return false;	// 文字が尽きた

			#region [ 改行またはコメント開始文字(';')が出現するまでをパラメータ文字列と見なし、sb文字列 にコピーする。]
			//-----------------
			while( ce.Current != '\n' && ce.Current != ';' )
			{
				sb文字列.Append( ce.Current );

				if( !ce.MoveNext() )
					return false;
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・空白と改行をスキップする( ref CharEnumerator ce )
		{
			// 空白と改行が続く間はこれらをスキップする。

			while( ce.Current == ' ' || ce.Current == '\n' )
			{
				if( ce.Current == '\n' )
					this.n現在の行数++;		// 改行文字では行番号が増える。

				if( !ce.MoveNext() )
					return false;	// 文字が尽きた
			}

			return true;
		}
		private bool t入力・空白をスキップする( ref CharEnumerator ce )
		{
			// 空白が続く間はこれをスキップする。

			while( ce.Current == ' ' )
			{
				if( !ce.MoveNext() )
					return false;	// 文字が尽きた
			}

			return true;
		}
		private void t入力・行解析( ref StringBuilder sbコマンド, ref StringBuilder sbパラメータ, ref StringBuilder sbコメント )
		{
			string strコマンド = sbコマンド.ToString();
			string strパラメータ = sbパラメータ.ToString().Trim();
			string strコメント = sbコメント.ToString();

			// 行頭コマンドの処理

			#region [ IF ]
			//-----------------
			if( strコマンド.StartsWith( "IF", StringComparison.OrdinalIgnoreCase ) )
			{
				this.t入力・パラメータ食い込みチェック( "IF", ref strコマンド, ref strパラメータ );

				if( this.bstackIFからENDIFをスキップする.Count == 255 )
				{
					Trace.TraceWarning( "#IF の入れ子の数が 255 を超えました。この #IF を無視します。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				}
				else if( this.bstackIFからENDIFをスキップする.Peek() )
				{
					this.bstackIFからENDIFをスキップする.Push( true );	// 親が true ならその入れ子も問答無用で true 。
				}
				else													// 親が false なら入れ子はパラメータと乱数を比較して結果を判断する。
				{
					int n数値 = 0;

					if( !int.TryParse( strパラメータ, out n数値 ) )
						n数値 = 1;

					this.bstackIFからENDIFをスキップする.Push( n数値 != this.n現在の乱数 );		// 乱数と数値が一致したら true 。
				}
			}
			//-----------------
			#endregion
			#region [ ENDIF ]
			//-----------------
			else if( strコマンド.StartsWith( "ENDIF", StringComparison.OrdinalIgnoreCase ) )
			{
				this.t入力・パラメータ食い込みチェック( "ENDIF", ref strコマンド, ref strパラメータ );

				if( this.bstackIFからENDIFをスキップする.Count > 1 )
				{
					this.bstackIFからENDIFをスキップする.Pop();		// 入れ子を１つ脱出。
				}
				else
				{
					Trace.TraceWarning( "#ENDIF に対応する #IF がありません。この #ENDIF を無視します。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				}
			}
			//-----------------
			#endregion

			else if( !this.bstackIFからENDIFをスキップする.Peek() )		// IF～ENDIF をスキップするなら以下はすべて無視。
			{
				#region [ PATH_WAV ]
				//-----------------
				if( strコマンド.StartsWith( "PATH_WAV", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "PATH_WAV", ref strコマンド, ref strパラメータ );
					this.PATH_WAV = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ TITLE ]
				//-----------------
				else if( strコマンド.StartsWith( "TITLE", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "TITLE", ref strコマンド, ref strパラメータ );
					this.TITLE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ ARTIST ]
				//-----------------
				else if( strコマンド.StartsWith( "ARTIST", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "ARTIST", ref strコマンド, ref strパラメータ );
					this.ARTIST = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ COMMENT ]
				//-----------------
				else if( strコマンド.StartsWith( "COMMENT", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "COMMENT", ref strコマンド, ref strパラメータ );
					this.COMMENT = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ DLEVEL, PLAYLEVEL ]
				//-----------------
				else if(
					strコマンド.StartsWith( "DLEVEL", StringComparison.OrdinalIgnoreCase ) ||
					strコマンド.StartsWith( "PLAYLEVEL", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "DLEVEL", ref strコマンド, ref strパラメータ );
					this.t入力・パラメータ食い込みチェック( "PLAYLEVEL", ref strコマンド, ref strパラメータ );

					int dlevel;
					if( int.TryParse( strパラメータ, out dlevel ) )
					{
						this.LEVEL.Drums = Math.Min( Math.Max( dlevel, 0 ), 100 );	// 0～100 に丸める
					}
				}
				//-----------------
				#endregion
				#region [ GLEVEL ]
				//-----------------
				else if( strコマンド.StartsWith( "GLEVEL", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "GLEVEL", ref strコマンド, ref strパラメータ );

					int glevel;
					if( int.TryParse( strパラメータ, out glevel ) )
					{
						this.LEVEL.Guitar = Math.Min( Math.Max( glevel, 0 ), 100 );		// 0～100 に丸める
					}
				}
				//-----------------
				#endregion
				#region [ BLEVEL ]
				//-----------------
				else if( strコマンド.StartsWith( "BLEVEL", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "BLEVEL", ref strコマンド, ref strパラメータ );

					int blevel;
					if( int.TryParse( strパラメータ, out blevel ) )
					{
						this.LEVEL.Bass = Math.Min( Math.Max( blevel, 0 ), 100 );		// 0～100 に丸める
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
				else if( strコマンド.StartsWith( "GENRE", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "GENRE", ref strコマンド, ref strパラメータ );
					this.GENRE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ HIDDENLEVEL ]
				//-----------------
				else if( strコマンド.StartsWith( "HIDDENLEVEL", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "HIDDENLEVEL", ref strコマンド, ref strパラメータ );
					this.HIDDENLEVEL = strパラメータ.ToLower().Equals( "on" );
				}
				//-----------------
				#endregion
				#region [ STAGEFILE ]
				//-----------------
				else if( strコマンド.StartsWith( "STAGEFILE", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "STAGEFILE", ref strコマンド, ref strパラメータ );
					this.STAGEFILE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREVIEW ]
				//-----------------
				else if( strコマンド.StartsWith( "PREVIEW", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "PREVIEW", ref strコマンド, ref strパラメータ );
					this.PREVIEW = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREIMAGE ]
				//-----------------
				else if( strコマンド.StartsWith( "PREIMAGE", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "PREIMAGE", ref strコマンド, ref strパラメータ );
					this.PREIMAGE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ PREMOVIE ]
				//-----------------
				else if( strコマンド.StartsWith( "PREMOVIE", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "PREMOVIE", ref strコマンド, ref strパラメータ );
					this.PREMOVIE = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ BACKGROUND_GR ]
				//-----------------
				else if( strコマンド.StartsWith( "BACKGROUND_GR", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "BACKGROUND_GR", ref strコマンド, ref strパラメータ );
					this.BACKGROUND_GR = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ BACKGROU}ND, WALL ]
				//-----------------
				else if(
					strコマンド.StartsWith( "BACKGROUND", StringComparison.OrdinalIgnoreCase ) ||
					strコマンド.StartsWith( "WALL", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "BACKGROUND", ref strコマンド, ref strパラメータ );
					this.t入力・パラメータ食い込みチェック( "WALL", ref strコマンド, ref strパラメータ );
					this.BACKGROUND = strパラメータ;
				}
				//-----------------
				#endregion
				#region [ RANDOM ]
				//-----------------
				else if( strコマンド.StartsWith( "RANDOM", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "RANDOM", ref strコマンド, ref strパラメータ );

					int n数値 = 1;
					if( !int.TryParse( strパラメータ, out n数値 ) )
						n数値 = 1;

					this.n現在の乱数 = CDTXMania.Random.Next( n数値 ) + 1;		// 1～数値 までの乱数を生成。
				}
				//-----------------
				#endregion
				#region [ SOUND_NOWLOADING ]
				//-----------------
				else if( strコマンド.StartsWith( "SOUND_NOWLOADING", StringComparison.OrdinalIgnoreCase ) )
				{
					this.t入力・パラメータ食い込みチェック( "SOUND_NOWLOADING", ref strコマンド, ref strパラメータ );
					this.SOUND_NOWLOADING = strパラメータ;
				}
				//-----------------
				#endregion

				else if( !this.bヘッダのみ )		// ヘッダのみの解析の場合、以下は無視。
				{
					#region [ PANEL ]
					//-----------------
					if( strコマンド.StartsWith( "PANEL", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "PANEL", ref strコマンド, ref strパラメータ );

						int dummyResult;								// #23885 2010.12.12 yyagi: not to confuse "#PANEL strings (panel)" and "#PANEL int (panpot of EL)"
						if( !int.TryParse( strパラメータ, out dummyResult ) )
						{		// 数値じゃないならPANELとみなす
							this.PANEL = strパラメータ;							//
							goto EOL;									//
						}												// 数値ならPAN ELとみなす

					}
					//-----------------
					#endregion
					#region [ MIDIFILE ]
					//-----------------
					else if( strコマンド.StartsWith( "MIDIFILE", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "MIDIFILE", ref strコマンド, ref strパラメータ );
						this.MIDIFILE = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ MIDINOTE ]
					//-----------------
					else if( strコマンド.StartsWith( "MIDINOTE", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "MIDINOTE", ref strコマンド, ref strパラメータ );
						this.MIDINOTE = strパラメータ.ToLower().Equals( "on" );
					}
					//-----------------
					#endregion
					#region [ BLACKCOLORKEY ]
					//-----------------
					else if( strコマンド.StartsWith( "BLACKCOLORKEY", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "BLACKCOLORKEY", ref strコマンド, ref strパラメータ );
						this.BLACKCOLORKEY = strパラメータ.ToLower().Equals( "on" );
					}
					//-----------------
					#endregion
					#region [ BASEBPM ]
					//-----------------
					else if( strコマンド.StartsWith( "BASEBPM", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "BASEBPM", ref strコマンド, ref strパラメータ );

						double basebpm = 0.0;
						//if( double.TryParse( str2, out num6 ) && ( num6 > 0.0 ) )
						if( TryParse( strパラメータ, out basebpm ) && basebpm > 0.0 )	// #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
						{													// #24204 2011.01.21 yyagi: Fix the condition correctly
							this.BASEBPM = basebpm;
						}
					}
					//-----------------
					#endregion
					#region [ SOUND_STAGEFAILED ]
					//-----------------
					else if( strコマンド.StartsWith( "SOUND_STAGEFAILED", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "SOUND_STAGEFAILED", ref strコマンド, ref strパラメータ );
						this.SOUND_STAGEFAILED = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ SOUND_FULLCOMBO ]
					//-----------------
					else if( strコマンド.StartsWith( "SOUND_FULLCOMBO", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "SOUND_FULLCOMBO", ref strコマンド, ref strパラメータ );
						this.SOUND_FULLCOMBO = strパラメータ;
					}
					//-----------------
					#endregion
					#region [ SOUND_AUDIENCE ]
					//-----------------
					else if( strコマンド.StartsWith( "SOUND_AUDIENCE", StringComparison.OrdinalIgnoreCase ) )
					{
						this.t入力・パラメータ食い込みチェック( "SOUND_AUDIENCE", ref strコマンド, ref strパラメータ );
						this.SOUND_AUDIENCE = strパラメータ;
					}
					//-----------------
					#endregion

					// オブジェクト記述コマンドの処理。

					else if( !this.t入力・行解析・WAVVOL_VOLUME( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・WAVPAN_PAN( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・WAV( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・BMPTEX( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・BMP( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・BGAPAN( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・BGA( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・AVIPAN( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・AVI_VIDEO( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・BPM_BPMzz( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・RESULTIMAGE( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・RESULTMOVIE( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・RESULTSOUND( strコマンド, strパラメータ, strコメント ) &&
						!this.t入力・行解析・SIZE( strコマンド, strパラメータ, strコメント ) )
					{
						this.t入力・行解析・チップ配置( strコマンド, strパラメータ, strコメント );
					}
				EOL:
					Debug.Assert( true );		// #23885 2010.12.12 yyagi: dummy line to exit parsing the line
												// 2011.8.17 from: "int xx=0;" から変更。毎回警告が出るので。
				}
			}
		}
		private bool t入力・行解析・AVI_VIDEO( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "AVI" or "VIDEO" で始まらないコマンドは無効。]
			//-----------------
			if( strコマンド.StartsWith( "AVI", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 3 );		// strコマンド から先頭の"AVI"文字を除去。

			else if( strコマンド.StartsWith( "VIDEO", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 5 );		// strコマンド から先頭の"VIDEO"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// AVI番号 zz がないなら無効。

			#region [ AVI番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "AVI(VIDEO)番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			#region [ AVIリストに {zz, avi} の組を登録する。 ]
			//-----------------
			var avi = new CAVI() {
				n番号 = zz,
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			if( this.listAVI.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listAVI.Remove( zz );

			this.listAVI.Add( zz, avi );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・AVIPAN( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "AVIPAN" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "AVIPAN", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 6 );	// strコマンド から先頭の"AVIPAN"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// AVIPAN番号 zz がないなら無効。

			#region [ AVIPAN番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "AVIPAN番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			var avipan = new CAVIPAN() {
				n番号 = zz,
			};

			// パラメータ引数（14個）を取得し、avipan に登録していく。

			string[] strParams = strパラメータ.Split( new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries );

			#region [ パラメータ引数は全14個ないと無効。]
			//-----------------
			if( strParams.Length < 14 )
			{
				Trace.TraceError( "AVIPAN: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1. AVI番号 ]
			//-----------------
			if( string.IsNullOrEmpty( strParams[ i ] ) || strParams[ i ].Length > 2 )
			{
				Trace.TraceError( "AVIPAN: {2}番目の数（AVI番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.nAVI番号 = C変換.n36進数2桁の文字列を数値に変換して返す( strParams[ i ] );
			if( avipan.nAVI番号 < 1 || avipan.nAVI番号 >= 36 * 36 )
			{
				Trace.TraceError( "AVIPAN: {2}番目の数（AVI番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2. 開始転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（開始転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.sz開始サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3. 転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（開始転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.sz開始サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4. 終了転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（終了転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.sz終了サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5. 終了転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（終了転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.sz終了サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6. 動画側開始位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（動画側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt動画側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7. 動画側開始位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（動画側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt動画側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 8. 動画側終了位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（動画側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt動画側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 9. 動画側終了位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（動画側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt動画側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 10.表示側開始位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（表示側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt表示側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 11.表示側開始位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（表示側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt表示側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 12.表示側終了位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（表示側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt表示側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 13.表示側終了位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（表示側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			avipan.pt表示側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 14.移動時間 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "AVIPAN: {2}番目の引数（移動時間）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}

			if( n値 < 0 )
				n値 = 0;

			avipan.n移動時間ct = n値;
			i++;
			//-----------------
			#endregion

			#region [ AVIPANリストに {zz, avipan} の組を登録する。]
			//-----------------
			if( this.listAVIPAN.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listAVIPAN.Remove( zz );

			this.listAVIPAN.Add( zz, avipan );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・BGA( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "BGA" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "BGA", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 3 );	// strコマンド から先頭の"BGA"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// BGA番号 zz がないなら無効。

			#region [ BGA番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "BGA番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			var bga = new CBGA() {
				n番号 = zz,
			};

			// パラメータ引数（7個）を取得し、bga に登録していく。

			string[] strParams = strパラメータ.Split( new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries );

			#region [ パラメータ引数は全7個ないと無効。]
			//-----------------
			if( strParams.Length < 7 )
			{
				Trace.TraceError( "BGA: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1.BMP番号 ]
			//-----------------
			if( string.IsNullOrEmpty( strParams[ i ] ) || strParams[ i ].Length > 2 )
			{
				Trace.TraceError( "BGA: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.nBMP番号 = C変換.n36進数2桁の文字列を数値に変換して返す( strParams[ i ] );
			if( bga.nBMP番号 < 1 || bga.nBMP番号 >= 36 * 36 )
			{
				Trace.TraceError( "BGA: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2.画像側位置１・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（画像側位置１・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt画像側左上座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3.画像側位置１・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（画像側位置１・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt画像側左上座標.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4.画像側位置２・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（画像側位置２・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt画像側右下座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5.画像側位置２・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（画像側座標２・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt画像側右下座標.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6.表示位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（表示位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt表示座標.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7.表示位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGA: {2}番目の引数（表示位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bga.pt表示座標.Y = n値;
			i++;
			//-----------------
			#endregion

			#region [ 画像側座標の正規化とクリッピング。]
			//-----------------
			if( bga.pt画像側左上座標.X > bga.pt画像側右下座標.X )
			{
				n値 = bga.pt画像側左上座標.X;
				bga.pt画像側左上座標.X = bga.pt画像側右下座標.X;
				bga.pt画像側右下座標.X = n値;
			}
			if( bga.pt画像側左上座標.Y > bga.pt画像側右下座標.Y )
			{
				n値 = bga.pt画像側左上座標.Y;
				bga.pt画像側左上座標.Y = bga.pt画像側右下座標.Y;
				bga.pt画像側右下座標.Y = n値;
			}
			//-----------------
			#endregion
			#region [ BGAリストに {zz, bga} の組を登録する。]
			//-----------------
			if( this.listBGA.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBGA.Remove( zz );

			this.listBGA.Add( zz, bga );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・BGAPAN( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "BGAPAN" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "BGAPAN", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 6 );	// strコマンド から先頭の"BGAPAN"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// BGAPAN番号 zz がないなら無効。

			#region [ BGAPAN番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "BGAPAN番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			var bgapan = new CBGAPAN() {
				n番号 = zz,
			};

			// パラメータ引数（14個）を取得し、bgapan に登録していく。

			string[] strParams = strパラメータ.Split( new char[] { ' ', ',', '(', ')', '[', ']', 'x', '|' }, StringSplitOptions.RemoveEmptyEntries );

			#region [ パラメータ引数は全14個ないと無効。]
			//-----------------
			if( strParams.Length < 14 )
			{
				Trace.TraceError( "BGAPAN: 引数が足りません。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			int i = 0;
			int n値 = 0;

			#region [ 1. BMP番号 ]
			//-----------------
			if( string.IsNullOrEmpty( strParams[ i ] ) || strParams[ i ].Length > 2 )
			{
				Trace.TraceError( "BGAPAN: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.nBMP番号 = C変換.n36進数2桁の文字列を数値に変換して返す( strParams[ i ] );
			if( bgapan.nBMP番号 < 1 || bgapan.nBMP番号 >= 36 * 36 )
			{
				Trace.TraceError( "BGAPAN: {2}番目の数（BMP番号）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			i++;
			//-----------------
			#endregion
			#region [ 2. 開始転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（開始転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.sz開始サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 3. 開始転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（開始転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.sz開始サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 4. 終了転送サイズ・幅 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（終了転送サイズ・幅）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.sz終了サイズ.Width = n値;
			i++;
			//-----------------
			#endregion
			#region [ 5. 終了転送サイズ・高さ ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（終了転送サイズ・高さ）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.sz終了サイズ.Height = n値;
			i++;
			//-----------------
			#endregion
			#region [ 6. 画像側開始位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（画像側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt画像側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 7. 画像側開始位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（画像側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt画像側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 8. 画像側終了位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（画像側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt画像側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 9. 画像側終了位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（画像側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt画像側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 10.表示側開始位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（表示側開始位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt表示側開始位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 11.表示側開始位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（表示側開始位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt表示側開始位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 12.表示側終了位置・X ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（表示側終了位置・X）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt表示側終了位置.X = n値;
			i++;
			//-----------------
			#endregion
			#region [ 13.表示側終了位置・Y ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（表示側終了位置・Y）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}
			bgapan.pt表示側終了位置.Y = n値;
			i++;
			//-----------------
			#endregion
			#region [ 14.移動時間 ]
			//-----------------
			n値 = 0;
			if( !int.TryParse( strParams[ i ], out n値 ) )
			{
				Trace.TraceError( "BGAPAN: {2}番目の引数（移動時間）が異常です。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数, i + 1 );
				return false;
			}

			if( n値 < 0 )
				n値 = 0;

			bgapan.n移動時間ct = n値;
			i++;
			//-----------------
			#endregion

			#region [ BGAPANリストに {zz, bgapan} の組を登録する。]
			//-----------------
			if( this.listBGAPAN.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBGAPAN.Remove( zz );

			this.listBGAPAN.Add( zz, bgapan );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・BMP( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "BMP" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "BMP", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 3 );	// strコマンド から先頭の"BMP"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			int zz = 0;

			#region [ BMP番号 zz を取得する。]
			//-----------------
			if( strコマンド.Length < 2 )
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
				zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
				if( zz < 0 || zz >= 36 * 36 )
				{
					Trace.TraceError( "BMP番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
					return false;
				}
				//-----------------
				#endregion
			}
			//-----------------
			#endregion


			var bmp = new CBMP() {
				n番号 = zz,
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ BMPリストに {zz, bmp} の組を登録。]
			//-----------------
			if( this.listBMP.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBMP.Remove( zz );

			this.listBMP.Add( zz, bmp );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・BMPTEX( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "BMPTEX" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "BMPTEX", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 6 );	// strコマンド から先頭の"BMPTEX"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// BMPTEX番号 zz がないなら無効。

			#region [ BMPTEX番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "BMPTEX番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			var bmptex = new CBMPTEX() {
				n番号 = zz,
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ BMPTEXリストに {zz, bmptex} の組を登録する。]
			//-----------------
			if( this.listBMPTEX.ContainsKey( zz ) )	// 既にリスト中に存在しているなら削除。後のものが有効。
				this.listBMPTEX.Remove( zz );

			this.listBMPTEX.Add( zz, bmptex );
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・BPM_BPMzz( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "BPM" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "BPM", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 3 );	// strコマンド から先頭の"BPM"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			int zz = 0;

			#region [ BPM番号 zz を取得する。]
			//-----------------
			if( strコマンド.Length < 2 )
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
				zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
				if( zz < 0 || zz >= 36 * 36 )
				{
					Trace.TraceError( "BPM番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
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
			if( !TryParse( strパラメータ, out dbBPM ) )			// #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
				return false;

			if( dbBPM <= 0.0 )
				return false;
			//-----------------
			#endregion

			if( zz == 0 )			// "#BPM00:" と "#BPM:" は等価。
				this.BPM = dbBPM;	// この曲の代表 BPM に格納する。

			#region [ BPMリストに {内部番号, zz, dbBPM} の組を登録。]
			//-----------------
			this.listBPM.Add(
				this.n内部番号BPM1to,
				new CBPM() {
					n内部番号 = this.n内部番号BPM1to,
					n表記上の番号 = zz,
					dbBPM値 = dbBPM,
				} );
			//-----------------
			#endregion

			#region [ BPM番号が zz であるBPM未設定のBPMチップがあれば、そのサイズを変更する。無限管理に対応。]
			//-----------------
			if( this.n無限管理BPM[ zz ] == -zz )	// 初期状態では n無限管理BPM[zz] = -zz である。この場合、#BPMzz がまだ出現していないことを意味する。
			{
				for( int i = 0; i < this.listChip.Count; i++ )	// これまでに出てきたチップのうち、該当する（BPM値が未設定の）BPMチップの値を変更する（仕組み上、必ず後方参照となる）。
				{
					var chip = this.listChip[ i ];

					if( chip.bBPMチップである && chip.n整数値・内部番号 == -zz )	// #BPMzz 行より前の行に出現した #BPMzz では、整数値・内部番号は -zz に初期化されている。
						chip.n整数値・内部番号 = this.n内部番号BPM1to;
				}
			}
			this.n無限管理BPM[ zz ] = this.n内部番号BPM1to;			// 次にこの BPM番号 zz を使うBPMチップが現れたら、このBPM値が格納されることになる。
			this.n内部番号BPM1to++;		// 内部番号は単純増加連番。
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・RESULTIMAGE( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "RESULTIMAGE" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "RESULTIMAGE", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 11 );	// strコマンド から先頭の"RESULTIMAGE"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTIMAGE:" と "#RESULTIMAGE_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if( strコマンド.Length < 2 )
			{
				#region [ (A) ランク指定がない場合("#RESULTIMAGE:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for( int i = 0; i < 7; i++ )
				{
					if( this.nRESULTIMAGE用優先順位[ i ] == 0 )
						this.RESULTIMAGE[ i ] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTIMAGE_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch( strコマンド.ToUpper() )
				{
					case "_SS":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 0, strパラメータ );
						break;

					case "_S":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 1, strパラメータ );
						break;

					case "_A":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 2, strパラメータ );
						break;

					case "_B":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 3, strパラメータ );
						break;

					case "_C":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 4, strパラメータ );
						break;

					case "_D":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 5, strパラメータ );
						break;

					case "_E":
						this.t入力・行解析・RESULTIMAGE・ファイルを設定する( 6, strパラメータ );
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力・行解析・RESULTIMAGE・ファイルを設定する( int nランク0to6, string strファイル名 )
		{
			if( nランク0to6 < 0 || nランク0to6 > 6 )	// 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for( int i = nランク0to6; i >= 0; i-- )
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTIMAGE[] に限り、ファイル名を更新できる。
				//（例：#RESULTMOVIE_D が #RESULTIMAGE_A より後に出現しても、#RESULTIMAGE_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if( this.nRESULTIMAGE用優先順位[ i ] < n優先順位 )
				{
					this.nRESULTIMAGE用優先順位[ i ] = n優先順位;
					this.RESULTIMAGE[ i ] = strファイル名;
				}
			}
		}
		private bool t入力・行解析・RESULTMOVIE( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "RESULTMOVIE" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "RESULTMOVIE", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 11 );	// strコマンド から先頭の"RESULTMOVIE"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTMOVIE:" と "#RESULTMOVIE_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if( strコマンド.Length < 2 )
			{
				#region [ (A) ランク指定がない場合("#RESULTMOVIE:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for( int i = 0; i < 7; i++ )
				{
					if( this.nRESULTMOVIE用優先順位[ i ] == 0 )
						this.RESULTMOVIE[ i ] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTMOVIE_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch( strコマンド.ToUpper() )
				{
					case "_SS":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 0, strパラメータ );
						break;

					case "_S":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 1, strパラメータ );
						break;

					case "_A":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 2, strパラメータ );
						break;

					case "_B":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 3, strパラメータ );
						break;

					case "_C":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 4, strパラメータ );
						break;

					case "_D":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 5, strパラメータ );
						break;

					case "_E":
						this.t入力・行解析・RESULTMOVIE・ファイルを設定する( 6, strパラメータ );
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力・行解析・RESULTMOVIE・ファイルを設定する( int nランク0to6, string strファイル名 )
		{
			if( nランク0to6 < 0 || nランク0to6 > 6 )	// 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for( int i = nランク0to6; i >= 0; i-- )
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTMOVIE[] に限り、ファイル名を更新できる。
				//（例：#RESULTMOVIE_D が #RESULTMOVIE_A より後に出現しても、#RESULTMOVIE_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if( this.nRESULTMOVIE用優先順位[ i ] < n優先順位 )
				{
					this.nRESULTMOVIE用優先順位[ i ] = n優先順位;
					this.RESULTMOVIE[ i ] = strファイル名;
				}
			}
		}
		private bool t入力・行解析・RESULTSOUND( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "RESULTSOUND" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "RESULTSOUND", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 11 );	// strコマンド から先頭の"RESULTSOUND"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。
			//     コマンドには "#RESULTSOUND:" と "#RESULTSOUND_SS～E" の2種類があり、パラメータの処理はそれぞれ異なる。

			if( strコマンド.Length < 2 )
			{
				#region [ (A) ランク指定がない場合("#RESULTSOUND:") → 優先順位が設定されていないすべてのランクで同じパラメータを使用する。]
				//-----------------
				for( int i = 0; i < 7; i++ )
				{
					if( this.nRESULTSOUND用優先順位[ i ] == 0 )
						this.RESULTSOUND[ i ] = strパラメータ.Trim();
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) ランク指定がある場合("#RESULTSOUND_SS～E:") → 優先順位に従ってパラメータを記録する。]
				//-----------------
				switch( strコマンド.ToUpper() )
				{
					case "_SS":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 0, strパラメータ );
						break;

					case "_S":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 1, strパラメータ );
						break;

					case "_A":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 2, strパラメータ );
						break;

					case "_B":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 3, strパラメータ );
						break;

					case "_C":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 4, strパラメータ );
						break;

					case "_D":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 5, strパラメータ );
						break;

					case "_E":
						this.t入力・行解析・RESULTSOUND・ファイルを設定する( 6, strパラメータ );
						break;
				}
				//-----------------
				#endregion
			}

			return true;
		}
		private void t入力・行解析・RESULTSOUND・ファイルを設定する( int nランク0to6, string strファイル名 )
		{
			if( nランク0to6 < 0 || nランク0to6 > 6 )	// 値域チェック。
				return;

			// 指定されたランクから上位のすべてのランクについて、ファイル名を更新する。

			for( int i = nランク0to6; i >= 0; i-- )
			{
				int n優先順位 = 7 - nランク0to6;

				// 現状より優先順位の低い RESULTSOUND[] に限り、ファイル名を更新できる。
				//（例：#RESULTSOUND_D が #RESULTSOUND_A より後に出現しても、#RESULTSOUND_A で指定されたファイル名を上書きすることはできない。しかしその逆は可能。）

				if( this.nRESULTSOUND用優先順位[ i ] < n優先順位 )
				{
					this.nRESULTSOUND用優先順位[ i ] = n優先順位;
					this.RESULTSOUND[ i ] = strファイル名;
				}
			}
		}
		private bool t入力・行解析・SIZE( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "SIZE" で始まらないコマンドや、その後ろに2文字（番号）が付随してないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "SIZE", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 4 );	// strコマンド から先頭の"SIZE"文字を除去。

			if( strコマンド.Length < 2 )	// サイズ番号の指定がない場合は無効。
				return false;
			//-----------------
			#endregion

			#region [ nWAV番号（36進数2桁）を取得。]
			//-----------------
			int nWAV番号 = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );

			if( nWAV番号 < 0 || nWAV番号 >= 36 * 36 )
			{
				Trace.TraceError( "SIZEのWAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion


			// (2) パラメータを処理。

			#region [ nサイズ値 を取得する。値は 0～100 に収める。]
			//-----------------
			int nサイズ値;

			if( !int.TryParse( strパラメータ, out nサイズ値 ) )
				return true;	// int変換に失敗しても、この行自体の処理は終えたのでtrueを返す。

			nサイズ値 = Math.Min( Math.Max( nサイズ値, 0 ), 100 );	// 0未満は0、100超えは100に強制変換。
			//-----------------
			#endregion

			#region [ nWAV番号で示されるサイズ未設定のWAVチップがあれば、そのサイズを変更する。無限管理に対応。]
			//-----------------
			if( this.n無限管理SIZE[ nWAV番号 ] == -nWAV番号 )	// 初期状態では n無限管理SIZE[xx] = -xx である。この場合、#SIZExx がまだ出現していないことを意味する。
			{
				foreach( CWAV wav in this.listWAV.Values )		// これまでに出てきたWAVチップのうち、該当する（サイズが未設定の）チップのサイズを変更する（仕組み上、必ず後方参照となる）。
				{
					if( wav.nチップサイズ == -nWAV番号 )		// #SIZExx 行より前の行に出現した #WAVxx では、チップサイズは -xx に初期化されている。
						wav.nチップサイズ = nサイズ値;
				}
			}
			this.n無限管理SIZE[ nWAV番号 ] = nサイズ値;			// 次にこの nWAV番号を使うWAVチップが現れたら、負数の代わりに、このサイズ値が格納されることになる。
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・WAV( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "WAV" で始まらないコマンドは無効。]
			//-----------------
			if( !strコマンド.StartsWith( "WAV", StringComparison.OrdinalIgnoreCase ) )
				return false;

			strコマンド = strコマンド.Substring( 3 );	// strコマンド から先頭の"WAV"文字を除去。
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "WAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			var wav = new CWAV() {
				n内部番号 = this.n内部番号WAV1to,
				n表記上の番号 = zz,
				nチップサイズ = this.n無限管理SIZE[ zz ],
				n位置 = this.n無限管理PAN[ zz ],
				n音量 = this.n無限管理VOL[ zz ],
				strファイル名 = strパラメータ,
				strコメント文 = strコメント,
			};

			#region [ WAVリストに {内部番号, wav} の組を登録。]
			//-----------------
			this.listWAV.Add( this.n内部番号WAV1to, wav );
			//-----------------
			#endregion

			#region [ WAV番号が zz である内部番号未設定のWAVチップがあれば、その内部番号を変更する。無限管理対応。]
			//-----------------
			if( this.n無限管理WAV[ zz ] == -zz )	// 初期状態では n無限管理WAV[zz] = -zz である。この場合、#WAVzz がまだ出現していないことを意味する。
			{
				for( int i = 0; i < this.listChip.Count; i++ )	// これまでに出てきたチップのうち、該当する（内部番号が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
				{
					var chip = this.listChip[ i ];

					if( chip.bWAVを使うチャンネルである && ( chip.n整数値・内部番号 == -zz ) )	// この #WAVzz 行より前の行に出現した #WAVzz では、整数値・内部番号は -zz に初期化されている。
						chip.n整数値・内部番号 = this.n内部番号WAV1to;
				}
			}
			this.n無限管理WAV[ zz ] = this.n内部番号WAV1to;			// 次にこの WAV番号 zz を使うWAVチップが現れたら、この内部番号が格納されることになる。
			this.n内部番号WAV1to++;		// 内部番号は単純増加連番。
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・WAVPAN_PAN( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "WAVPAN" or "PAN" で始まらないコマンドは無効。]
			//-----------------
			if( strコマンド.StartsWith( "WAVPAN", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 6 );		// strコマンド から先頭の"WAVPAN"文字を除去。

			else if( strコマンド.StartsWith( "PAN", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 3 );		// strコマンド から先頭の"PAN"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "WAVPAN(PAN)のWAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			#region [ WAV番号 zz を持つWAVチップの位置を変更する。無限定義対応。]
			//-----------------
			int n位置;
			if( int.TryParse( strパラメータ, out n位置 ) )
			{
				n位置 = Math.Min( Math.Max( n位置, -100 ), 100 );	// -100～+100 に丸める

				if( this.n無限管理PAN[ zz ] == ( -10000 - zz ) )	// 初期状態では n無限管理PAN[zz] = -10000 - zz である。この場合、#WAVPANzz, #PANzz がまだ出現していないことを意味する。
				{
					foreach( CWAV wav in this.listWAV.Values )	// これまでに出てきたチップのうち、該当する（位置が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
					{
						if( wav.n位置 == ( -10000 - zz ) )	// #WAVPANzz, #PANzz 行より前の行に出現した #WAVzz では、位置は -10000-zz に初期化されている。
							wav.n位置 = n位置;
					}
				}
				this.n無限管理PAN[ zz ] = n位置;			// 次にこの WAV番号 zz を使うWAVチップが現れたら、この位置が格納されることになる。
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・WAVVOL_VOLUME( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			#region [ "WAVCOL" or "VOLUME" で始まらないコマンドは無効。]
			//-----------------
			if( strコマンド.StartsWith( "WAVVOL", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 6 );		// strコマンド から先頭の"WAVVOL"文字を除去。

			else if( strコマンド.StartsWith( "VOLUME", StringComparison.OrdinalIgnoreCase ) )
				strコマンド = strコマンド.Substring( 6 );		// strコマンド から先頭の"VOLUME"文字を除去。

			else
				return false;
			//-----------------
			#endregion

			// (2) パラメータを処理。

			if( strコマンド.Length < 2 )
				return false;	// WAV番号 zz がないなら無効。

			#region [ WAV番号 zz を取得する。]
			//-----------------
			int zz = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( zz < 0 || zz >= 36 * 36 )
			{
				Trace.TraceError( "WAV番号に 00～ZZ 以外の値または不正な文字列が指定されました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
				return false;
			}
			//-----------------
			#endregion

			#region [ WAV番号 zz を持つWAVチップの音量を変更する。無限定義対応。]
			//-----------------
			int n音量;
			if( int.TryParse( strパラメータ, out n音量 ) )
			{
				n音量 = Math.Min( Math.Max( n音量, 0 ), 100 );	// 0～100に丸める。

				if( this.n無限管理VOL[ zz ] == -zz )	// 初期状態では n無限管理VOL[zz] = - zz である。この場合、#WAVVOLzz, #VOLUMEzz がまだ出現していないことを意味する。
				{
					foreach( CWAV wav in this.listWAV.Values )	// これまでに出てきたチップのうち、該当する（音量が未設定の）WAVチップの値を変更する（仕組み上、必ず後方参照となる）。
					{
						if( wav.n音量 == -zz )	// #WAVVOLzz, #VOLUMEzz 行より前の行に出現した #WAVzz では、音量は -zz に初期化されている。
							wav.n音量 = n音量;
					}
				}
				this.n無限管理VOL[ zz ] = n音量;			// 次にこの WAV番号 zz を使うWAVチップが現れたら、この音量が格納されることになる。
			}
			//-----------------
			#endregion

			return true;
		}
		private bool t入力・行解析・チップ配置( string strコマンド, string strパラメータ, string strコメント )
		{
			// (1) コマンドを処理。

			if( strコマンド.Length != 5 )	// コマンドは必ず5文字であること。
				return false;

			#region [ n小節番号 を取得する。]
			//-----------------
			int n小節番号 = C変換.n小節番号の文字列3桁を数値に変換して返す( strコマンド.Substring( 0, 3 ) );
			if( n小節番号 < 0 )
				return false;

			n小節番号++;	// 先頭に空の1小節を設ける。
			//-----------------
			#endregion

			#region [ nチャンネル番号 を取得する。]
			//-----------------
			int nチャンネル番号 = -1;

			// ファイルフォーマットによって処理が異なる。

			if( this.e種別 == E種別.GDA || this.e種別 == E種別.G2D )
			{
				#region [ (A) GDA, G2D の場合：チャンネル文字列をDTXのチャンネル番号へ置き換える。]
				//-----------------
				string strチャンネル文字列 = strコマンド.Substring( 3, 2 );

				foreach( STGDAPARAM param in this.stGDAParam )
				{
					if( strチャンネル文字列.Equals( param.strGDAのチャンネル文字列, StringComparison.OrdinalIgnoreCase ) )
					{
						nチャンネル番号 = param.nDTXのチャンネル番号;
						break;	// 置き換え成功
					}
				}
				if( nチャンネル番号 < 0 )
					return false;	// 置き換え失敗
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) その他の場合：チャンネル番号は16進数2桁。]
				//-----------------
				nチャンネル番号 = C変換.n16進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 3, 2 ) );

				if( nチャンネル番号 < 0 )
					return false;
				//-----------------
				#endregion
			}
			//-----------------
			#endregion
			#region [ 取得したチャンネル番号で、this.bチップがある に該当があれば設定する。]
			//-----------------
			if( ( nチャンネル番号 >= 0x11 ) && ( nチャンネル番号 <= 0x1a ) )
			{
				this.bチップがある.Drums = true;
			}
			else if( ( nチャンネル番号 >= 0x20 ) && ( nチャンネル番号 <= 0x27 ) )
			{
				this.bチップがある.Guitar = true;
			}
			else if( ( nチャンネル番号 >= 0xA0 ) && ( nチャンネル番号 <= 0xa7 ) )
			{
				this.bチップがある.Bass = true;
			}
			switch( nチャンネル番号 )
			{
				case 0x18:
					this.bチップがある.HHOpen = true;
					break;

				case 0x19:
					this.bチップがある.Ride = true;
					break;

				case 0x1a:
					this.bチップがある.LeftCymbal = true;
					break;

				case 0x20:
					this.bチップがある.OpenGuitar = true;
					break;

				case 0xA0:
					this.bチップがある.OpenBass = true;
					break;
			}
			//-----------------
			#endregion


			// (2) Ch.02を処理。

			#region [ 小節長変更(Ch.02)は他のチャンネルとはパラメータが特殊なので、先にとっとと終わらせる。 ]
			//-----------------
			if( nチャンネル番号 == 0x02 )
			{
				// 小節長倍率を取得する。

				double db小節長倍率 = 1.0;
				//if( !double.TryParse( strパラメータ, out result ) )
				if( !this.TryParse( strパラメータ, out db小節長倍率 ) )			// #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
				{
					Trace.TraceError( "小節長倍率に不正な値を指定しました。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
					return false;
				}

				// 小節長倍率チップを配置する。

				this.listChip.Add(
					new CChip() {
						nチャンネル番号 = nチャンネル番号,
						db実数値 = db小節長倍率,
						n発声位置 = n小節番号 * 384,
					} );

				return true;	// 配置終了。
			}
			//-----------------
			#endregion


			// (3) パラメータを処理。

			if( string.IsNullOrEmpty( strパラメータ ) )		// パラメータはnullまたは空文字列ではないこと。
				return false;

			#region [ strパラメータ にオブジェクト記述を格納し、その n文字数 をカウントする。]
			//-----------------
			int n文字数 = 0;

			var sb = new StringBuilder( strパラメータ.Length );

			// strパラメータを先頭から1文字ずつ見ながら正規化（無効文字('_')を飛ばしたり不正な文字でエラーを出したり）し、sb へ格納する。

			CharEnumerator ce = strパラメータ.GetEnumerator();
			while( ce.MoveNext() )
			{
				if( ce.Current == '_' )		// '_' は無視。
					continue;

				if( C変換.str36進数文字.IndexOf( ce.Current ) < 0 )	// オブジェクト記述は36進数文字であること。
				{
					Trace.TraceError( "不正なオブジェクト指定があります。[{0}: {1}行]", this.strファイル名の絶対パス, this.n現在の行数 );
					return false;
				}

				sb.Append( ce.Current );
				n文字数++;
			}

			strパラメータ = sb.ToString();	// 正規化された文字列になりました。

			if( ( n文字数 % 2 ) != 0 )		// パラメータの文字数が奇数の場合、最後の1文字を無視する。
				n文字数--;
			//-----------------
			#endregion


			// (4) パラメータをオブジェクト数値に分解して配置する。

			for( int i = 0; i < ( n文字数 / 2 ); i++ )	// 2文字で1オブジェクト数値
			{
				#region [ nオブジェクト数値 を１つ取得する。'00' なら無視。]
				//-----------------
				int nオブジェクト数値 = 0;

				if( nチャンネル番号 == 0x03 )
				{
					// Ch.03 のみ 16進数2桁。
					nオブジェクト数値 = C変換.n16進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( i * 2, 2 ) );
				}
				else
				{
					// その他のチャンネルは36進数2桁。
					nオブジェクト数値 = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( i * 2, 2 ) );
				}

				if( nオブジェクト数値 == 0x00 )
					continue;
				//-----------------
				#endregion

				// オブジェクト数値に対応するチップを生成。

				var chip = new CChip();

				chip.nチャンネル番号 = nチャンネル番号;
				chip.n発声位置 = ( n小節番号 * 384 ) + ( ( 384 * i ) / ( n文字数 / 2 ) );
				chip.n整数値 = nオブジェクト数値;
				chip.n整数値・内部番号 = nオブジェクト数値;

				#region [ chip.e楽器パート = ... ]
				//-----------------
				if( ( nチャンネル番号 >= 0x11 ) && ( nチャンネル番号 <= 0x1A ) )
				{
					chip.e楽器パート = E楽器パート.DRUMS;
				}
				if( ( nチャンネル番号 >= 0x20 ) && ( nチャンネル番号 <= 0x27 ) )
				{
					chip.e楽器パート = E楽器パート.GUITAR;
				}
				if( ( nチャンネル番号 >= 160 ) && ( nチャンネル番号 <= 0xA7 ) )
				{
					chip.e楽器パート = E楽器パート.BASS;
				}
				//-----------------
				#endregion

				#region [ 無限定義への対応 → 内部番号の取得。]
				//-----------------
				if( chip.bWAVを使うチャンネルである )
				{
					chip.n整数値・内部番号 = this.n無限管理WAV[ nオブジェクト数値 ];	// これが本当に一意なWAV番号となる。（無限定義の場合、chip.n整数値 は一意である保証がない。）
				}
				else if( chip.bBPMチップである )
				{
					chip.n整数値・内部番号 = this.n無限管理BPM[ nオブジェクト数値 ];	// これが本当に一意なBPM番号となる。（同上。）
				}
				//-----------------
				#endregion

				#region [ フィルインON/OFFチャンネル(Ch.53)の場合、発声位置を少し前後にずらす。]
				//-----------------
				if( nチャンネル番号 == 0x53 )
				{
					// ずらすのは、フィルインONチップと同じ位置にいるチップでも確実にフィルインが発動し、
					// 同様に、フィルインOFFチップと同じ位置にいるチップでも確実にフィルインが終了するようにするため。

					if( ( nオブジェクト数値 > 0 ) && ( nオブジェクト数値 != 2 ) )
					{
						chip.n発声位置 -= 32;	// 384÷32＝12 ということで、フィルインONチップは12分音符ほど前へ移動。
					}
					else if( nオブジェクト数値 == 2 )
					{
						chip.n発声位置 += 32;	// 同じく、フィルインOFFチップは12分音符ほど後ろへ移動。
					}
				}
				//-----------------
				#endregion

				// チップを配置。

				this.listChip.Add( chip );
			}
			return true;
		}
		#region [#23880 2010.12.30 yyagi: コンマとスペースの両方を小数点として扱うTryParse]
		/// <summary>
		/// 小数点としてコンマとピリオドの両方を受け付けるTryParse()
		/// </summary>
		/// <param name="s">strings convert to double</param>
		/// <param name="result">parsed double value</param>
		/// <returns>s が正常に変換された場合は true。それ以外の場合は false。</returns>
		/// <exception cref="ArgumentException">style が NumberStyles 値でないか、style に NumberStyles.AllowHexSpecifier 値が含まれている</exception>
		private bool TryParse(string s, out double result) {	// #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
																// EU諸国での #BPM 123,45 のような記述に対応するため、
																// 小数点の最終位置を検出して、それをlocaleにあった
																// 文字に置き換えてからTryParse()する
																// 桁区切りの文字はスキップする

			const string DecimalSeparators = ".,";				// 小数点文字
			const string GroupSeparators = ".,' ";				// 桁区切り文字
			const string NumberSymbols = "0123456789";			// 数値文字

			int len = s.Length;									// 文字列長
			int decimalPosition = len;							// 真の小数点の位置 最初は文字列終端位置に仮置きする

			for (int i = 0; i < len; i++) {							// まず、真の小数点(一番最後に現れる小数点)の位置を求める
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0) {				// 数値だったらスキップ
					continue;
				} else if (DecimalSeparators.IndexOf(c) >= 0) {		// 小数点文字だったら、その都度位置を上書き記憶
					decimalPosition = i;
				} else if (GroupSeparators.IndexOf(c) >= 0) {		// 桁区切り文字の場合もスキップ
					continue;
				} else {											// 数値・小数点・区切り文字以外がきたらループ終了
					break;
				}
			}

			StringBuilder decimalStr = new StringBuilder(16);
			for (int i = 0; i < len; i++) {							// 次に、localeにあった数値文字列を生成する
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0) {				// 数値だったら
					decimalStr.Append(c);							// そのままコピー
				} else if (DecimalSeparators.IndexOf(c) >= 0) {		// 小数点文字だったら
					if (i == decimalPosition) {						// 最後に出現した小数点文字なら、localeに合った小数点を出力する
						decimalStr.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
					}
				} else if (GroupSeparators.IndexOf(c) >= 0) {		// 桁区切り文字だったら
					continue;										// 何もしない(スキップ)
				} else {
					break;
				}
			}
			return double.TryParse(decimalStr.ToString(), out result);	// 最後に、自分のlocale向けの文字列に対してTryParse実行
		}
		#endregion
		//-----------------
		#endregion
	}
}
