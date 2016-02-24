using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using FDK;
using System.IO;

namespace DTXMania
{
	public partial class CDTX
	{
		// クラス

		public class CAVI : IDisposable
		{
			public CAviDS avi;
			private bool bDispose済み;

			int n番号;
			string strコメント文 = "";
			string strファイル名 = "";
			double dbPlaySpeed = 1;

			public CAVI(int number, string filename, string comment, double playSpeed)
			{
				n番号 = number;
				strファイル名 = filename;
				strコメント文 = comment;
				dbPlaySpeed = playSpeed;
			}

			public void OnDeviceCreated()
			{
				#region [ strAVIファイル名の作成。]
				//-----------------
				string strAVIファイル名;

				if (CDTXMania.Instance.DTX != null && !Path.IsPathRooted(this.strファイル名))	// CDTX抜きでCAVI単体で使うことを考慮(選曲画面, リザルト画面)
				{																			// 演奏終了直後はCDTXオブジェクトが残っているので、ファイル名がフルパスかどうかでプレビュー判定する
					if (!string.IsNullOrEmpty(CDTXMania.Instance.DTX.PATH_WAV))
						strAVIファイル名 = CDTXMania.Instance.DTX.PATH_WAV + this.strファイル名;
					else
						strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + this.strファイル名;
				}
				else
				{
					strAVIファイル名 = this.strファイル名;
				}
				//-----------------
				#endregion

				if (!File.Exists(strAVIファイル名))
				{
					Trace.TraceWarning("CAVI: ファイルが存在しません。({0})({1})", this.strコメント文, Path.GetFileName(strAVIファイル名));
					this.avi = null;
					return;
				}

				// AVI の生成。

				try
				{
					this.avi = new CAviDS(strAVIファイル名, this.dbPlaySpeed);
					Trace.TraceInformation("CAviDS: 動画を生成しました。({0})({1})({2}msec)", this.strコメント文, Path.GetFileName(strAVIファイル名), this.avi.GetDuration());
				}
				catch (Exception e)
				{
					Trace.TraceError(e.Message);
					Trace.TraceError("CAviDS: 動画の生成に失敗しました。({0})({1})", this.strコメント文, Path.GetFileName(strAVIファイル名));
					this.avi = null;
				}
			}
			public override string ToString()
			{
				return string.Format("CAVI{0}: File:{1}, Comment:{2}", CDTX.tZZ(this.n番号), this.strファイル名, this.strコメント文);
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if (this.bDispose済み)
					return;

				if (this.avi != null)
				{
					#region [ strAVIファイル名 の作成。なぜDispose時にファイル名の生成をしているのかと思ったら、デバッグログ用でした。 ]
					//-----------------
					string strAVIファイル名;
					if (CDTXMania.Instance.DTX != null && !Path.IsPathRooted(this.strファイル名))	// CDTX抜きでCAVI単体で使うことを考慮(選曲画面, リザルト画面)
					{																			// 演奏終了直後はCDTXオブジェクトが残っているので、ファイル名がフルパスかどうかでプレビュー判定する
						if (!string.IsNullOrEmpty(CDTXMania.Instance.DTX.PATH_WAV))
							strAVIファイル名 = CDTXMania.Instance.DTX.PATH_WAV + this.strファイル名;
						else
							strAVIファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + this.strファイル名;
					}
					else
					{
						strAVIファイル名 = this.strファイル名;
					}
					//-----------------
					#endregion

					this.avi.Dispose();
					this.avi = null;

					Trace.TraceInformation("動画を解放しました。({0})({1})", this.strコメント文, Path.GetFileName(strAVIファイル名));
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
			public Point pt動画側開始位置 = new Point(0, 0);
			public Point pt動画側終了位置 = new Point(0, 0);
			public Point pt表示側開始位置 = new Point(0, 0);
			public Point pt表示側終了位置 = new Point(0, 0);
			public Size sz開始サイズ = new Size(0, 0);
			public Size sz終了サイズ = new Size(0, 0);

			public override string ToString()
			{
				return string.Format("CAVIPAN{0}: AVI:{14}, 開始サイズ:{1}x{2}, 終了サイズ:{3}x{4}, 動画側開始位置:{5}x{6}, 動画側終了位置:{7}x{8}, 表示側開始位置:{9}x{10}, 表示側終了位置:{11}x{12}, 移動時間:{13}ct",
				  CDTX.tZZ(this.n番号),
				  this.sz開始サイズ.Width, this.sz開始サイズ.Height,
				  this.sz終了サイズ.Width, this.sz終了サイズ.Height,
				  this.pt動画側開始位置.X, this.pt動画側開始位置.Y,
				  this.pt動画側終了位置.X, this.pt動画側終了位置.Y,
				  this.pt表示側開始位置.X, this.pt表示側開始位置.Y,
				  this.pt表示側終了位置.X, this.pt表示側終了位置.Y,
				  this.n移動時間ct,
				  CDTX.tZZ(this.nAVI番号));
			}
		}
		public class CBGA
		{
			public int nBMP番号;
			public int n番号;
			public Point pt画像側右下座標 = new Point(0, 0);
			public Point pt画像側左上座標 = new Point(0, 0);
			public Point pt表示座標 = new Point(0, 0);

			public override string ToString()
			{
				return string.Format("CBGA{0}, BMP:{1}, 画像側左上座標:{2}x{3}, 画像側右下座標:{4}x{5}, 表示座標:{6}x{7}",
				  CDTX.tZZ(this.n番号),
				  CDTX.tZZ(this.nBMP番号),
				  this.pt画像側左上座標.X, this.pt画像側左上座標.Y,
				  this.pt画像側右下座標.X, this.pt画像側右下座標.Y,
				  this.pt表示座標.X, this.pt表示座標.Y);
			}
		}
		public class CBGAPAN
		{
			public int nBMP番号;
			public int n移動時間ct;
			public int n番号;
			public Point pt画像側開始位置 = new Point(0, 0);
			public Point pt画像側終了位置 = new Point(0, 0);
			public Point pt表示側開始位置 = new Point(0, 0);
			public Point pt表示側終了位置 = new Point(0, 0);
			public Size sz開始サイズ = new Size(0, 0);
			public Size sz終了サイズ = new Size(0, 0);

			public override string ToString()
			{
				return string.Format("CBGAPAN{0}: BMP:{14}, 開始サイズ:{1}x{2}, 終了サイズ:{3}x{4}, 画像側開始位置:{5}x{6}, 画像側終了位置:{7}x{8}, 表示側開始位置:{9}x{10}, 表示側終了位置:{11}x{12}, 移動時間:{13}ct",
				  CDTX.tZZ(this.nBMP番号),
				  this.sz開始サイズ.Width, this.sz開始サイズ.Height,
				  this.sz終了サイズ.Width, this.sz終了サイズ.Height,
				  this.pt画像側開始位置.X, this.pt画像側開始位置.Y,
				  this.pt画像側終了位置.X, this.pt画像側終了位置.Y,
				  this.pt表示側開始位置.X, this.pt表示側開始位置.Y,
				  this.pt表示側終了位置.X, this.pt表示側終了位置.Y,
				  this.n移動時間ct,
				  CDTX.tZZ(this.nBMP番号));
			}
		}
		public class CBMP : CBMPbase, IDisposable
		{
			public CBMP()
			{
				b黒を透過する = true;	// BMPでは、黒を透過色とする
			}
			public override void PutLog(string strテクスチャファイル名)
			{
				Trace.TraceInformation("テクスチャを生成しました。({0})({1})({2}x{3})", this.strコメント文, strテクスチャファイル名, this.n幅, this.n高さ);
			}
			public override string ToString()
			{
				return string.Format("CBMP{0}: File:{1}, Comment:{2}", CDTX.tZZ(this.n番号), this.strファイル名, this.strコメント文);
			}

		}
		public class CBMPTEX : CBMPbase, IDisposable
		{
			public CBMPTEX()
			{
				b黒を透過する = false;	// BMPTEXでは、透過色はαで表現する
			}
			public override void PutLog(string strテクスチャファイル名)
			{
				Trace.TraceInformation("テクスチャを生成しました。({0})({1})(Gr:{2}x{3})(Tx:{4}x{5})", this.strコメント文, strテクスチャファイル名, this.tx画像.sz画像サイズ.Width, this.tx画像.sz画像サイズ.Height, this.tx画像.szテクスチャサイズ.Width, this.tx画像.szテクスチャサイズ.Height);
			}
			public override string ToString()
			{
				return string.Format("CBMPTEX{0}: File:{1}, Comment:{2}", CDTX.tZZ(this.n番号), this.strファイル名, this.strコメント文);
			}
		}
		public class CBMPbase : IDisposable
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
			public bool b黒を透過する;
			public Bitmap bitmap;

			public string GetFullPathname
			{
				get
				{
					if (!string.IsNullOrEmpty(CDTXMania.Instance.DTX.PATH_WAV))
						return CDTXMania.Instance.DTX.PATH_WAV + this.strファイル名;
					else
						return CDTXMania.Instance.DTX.strフォルダ名 + this.strファイル名;
				}
			}

			public void OnDeviceCreated()
			{
				#region [ strテクスチャファイル名 を作成。]
				string strテクスチャファイル名 = this.GetFullPathname;
				#endregion

				if (!File.Exists(strテクスチャファイル名))
				{
					Trace.TraceWarning("ファイルが存在しません。({0})({1})", this.strコメント文, strテクスチャファイル名);
					this.tx画像 = null;
					return;
				}

				// テクスチャを作成。
				byte[] txData = File.ReadAllBytes(strテクスチャファイル名);
				this.tx画像 = TextureFactory.tテクスチャの生成(txData, b黒を透過する);

				if (this.tx画像 != null)
				{
					// 作成成功。
					if (CDTXMania.Instance.ConfigIni.bLog作成解放ログ出力)
						PutLog(strテクスチャファイル名);
					txData = null;
					this.bUse = true;
				}
				else
				{
					// 作成失敗。
					Trace.TraceError("テクスチャの生成に失敗しました。({0})({1})", this.strコメント文, strテクスチャファイル名);
					this.tx画像 = null;
				}
			}
			/// <summary>
			/// BGA画像のデコードをTexture()に渡す前に行う、OnDeviceCreate()
			/// </summary>
			/// <param name="bitmap">テクスチャ画像</param>
			/// <param name="strテクスチャファイル名">ファイル名</param>
			public void OnDeviceCreated(Bitmap bitmap, string strテクスチャファイル名)
			{
				if (bitmap != null && b黒を透過する)
				{
					bitmap.MakeTransparent(Color.Black);		// 黒を透過色にする
				}
				this.tx画像 = TextureFactory.tテクスチャの生成(bitmap, b黒を透過する);

				if (this.tx画像 != null)
				{
					// 作成成功。
					if (CDTXMania.Instance.ConfigIni.bLog作成解放ログ出力)
						PutLog(strテクスチャファイル名);
					this.bUse = true;
				}
				else
				{
					// 作成失敗。
					Trace.TraceError("テクスチャの生成に失敗しました。({0})({1})", this.strコメント文, strテクスチャファイル名);
					this.tx画像 = null;
				}
				if (bitmap != null)
				{
					bitmap.Dispose();
				}
			}

			public virtual void PutLog(string strテクスチャファイル名)
			{
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if (this.bDisposed済み)
					return;

				if (this.tx画像 != null)
				{
					#region [ strテクスチャファイル名 を作成。]
					//-----------------
					string strテクスチャファイル名 = this.GetFullPathname;
					//if( !string.IsNullOrEmpty( CDTXMania.Instance.DTX.PATH_WAV ) )
					//    strテクスチャファイル名 = CDTXMania.Instance.DTX.PATH_WAV + this.strファイル名;
					//else
					//    strテクスチャファイル名 = CDTXMania.Instance.DTX.strフォルダ名 + this.strファイル名;
					//-----------------
					#endregion

					TextureFactory.tテクスチャの解放(ref this.tx画像);

					if (CDTXMania.Instance.ConfigIni.bLog作成解放ログ出力)
						Trace.TraceInformation("テクスチャを解放しました。({0})({1})", this.strコメント文, strテクスチャファイル名);
				}
				this.bUse = false;

				this.bDisposed済み = true;
			}
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
				StringBuilder builder = new StringBuilder(0x80);
				if (this.n内部番号 != this.n表記上の番号)
				{
					builder.Append(string.Format("CBPM{0}(内部{1})", CDTX.tZZ(this.n表記上の番号), this.n内部番号));
				}
				else
				{
					builder.Append(string.Format("CBPM{0}", CDTX.tZZ(this.n表記上の番号)));
				}
				builder.Append(string.Format(", BPM:{0}", this.dbBPM値));
				return builder.ToString();
			}
		}
		public class CWAV : IDisposable
		{
			public bool bBGMとして使う;
			public List<Ech定義> listこのWAVを使用するチャンネル番号の集合 = new List<Ech定義>(16);
			public int nチップサイズ = 100;
			public int n位置;
			public long[] n一時停止時刻 = new long[CDTXMania.Instance.ConfigIni.nPoliphonicSounds];	// 4
			public int n音量 = 100;
			public int n現在再生中のサウンド番号;
			public long[] n再生開始時刻 = new long[CDTXMania.Instance.ConfigIni.nPoliphonicSounds];	// 4
			public int n内部番号;
			public int n表記上の番号;
			public CSound[] rSound = new CSound[CDTXMania.Instance.ConfigIni.nPoliphonicSounds];		// 4
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
			public bool bIsBassSound = false;
			public bool bIsGuitarSound = false;
			public bool bIsDrumsSound = false;
			public bool bIsSESound = false;
			public bool bIsBGMSound = false;

			public override string ToString()
			{
				var sb = new StringBuilder(128);

				if (this.n表記上の番号 == this.n内部番号)
				{
					sb.Append(string.Format("CWAV{0}: ", CDTX.tZZ(this.n表記上の番号)));
				}
				else
				{
					sb.Append(string.Format("CWAV{0}(内部{1}): ", CDTX.tZZ(this.n表記上の番号), this.n内部番号));
				}
				sb.Append(string.Format("音量:{0}, 位置:{1}, サイズ:{2}, BGM:{3}, File:{4}, Comment:{5}", this.n音量, this.n位置, this.nチップサイズ, this.bBGMとして使う ? 'Y' : 'N', this.strファイル名, this.strコメント文));

				return sb.ToString();
			}

			#region [ Dispose-Finalize パターン実装 ]
			//-----------------
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			public void Dispose(bool bManagedリソースの解放も行う)
			{
				if (this.bDisposed済み)
					return;

				if (bManagedリソースの解放も行う)
				{
					for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPoliphonicSounds; i++)	// 4
					{
						if (this.rSound[i] != null)
							CDTXMania.Instance.Sound管理.tサウンドを破棄する(this.rSound[i]);
						this.rSound[i] = null;

						if ((i == 0) && CDTXMania.Instance.ConfigIni.bLog作成解放ログ出力)
							Trace.TraceInformation("サウンドを解放しました。({0})({1})", this.strコメント文, this.strファイル名);
					}
				}

				this.bDisposed済み = true;
			}
			~CWAV()
			{
				this.Dispose(false);
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

			public int LP;
			public int LBD;

			public int Drums
			{
				get
				{
					return this.HH + this.SD + this.BD + this.HT + this.LT + this.CY + this.FT + this.HHO + this.RD + this.LC + this.LP + this.LBD;
				}
			}
			public int Guitar;
			public int Bass;

			public int this[int index]
			{
				get
				{
					switch (index)
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

						case 12:
							return this.LP;

						case 13:
							return this.LBD;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException();
					}
					switch (index)
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

						case 12:
							this.LP = value;
							return;

						case 13:
							this.LBD = value;
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

			public string this[int index]
			{
				get
				{
					switch (index)
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
					switch (index)
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

			public bool BGA;
			public bool Movie;

			public bool LeftPedal;
			public bool LeftBassDrum;

			public bool this[int index]
			{
				get
				{
					switch (index)
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

						case 8:
							return this.LeftPedal;

						case 9:
							return this.LeftBassDrum;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch (index)
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

						case 8:
							this.LeftPedal = value;
							return;

						case 9:
							this.LeftBassDrum = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

	}
}
