using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DTXMania
{
	internal class Cスコア
	{
		// プロパティ

		public STScoreIni情報 ScoreIni情報;
		[StructLayout( LayoutKind.Sequential )]
		public struct STScoreIni情報
		{
			public DateTime 最終更新日時;
			public long ファイルサイズ;

			public STScoreIni情報( DateTime 最終更新日時, long ファイルサイズ )
			{
				this.最終更新日時 = 最終更新日時;
				this.ファイルサイズ = ファイルサイズ;
			}
		}

		public STファイル情報 ファイル情報;
		[StructLayout( LayoutKind.Sequential )]
		public struct STファイル情報
		{
			public string ファイルの絶対パス;
			public string フォルダの絶対パス;
			public DateTime 最終更新日時;
			public long ファイルサイズ;

			public STファイル情報( string ファイルの絶対パス, string フォルダの絶対パス, DateTime 最終更新日時, long ファイルサイズ )
			{
				this.ファイルの絶対パス = ファイルの絶対パス;
				this.フォルダの絶対パス = フォルダの絶対パス;
				this.最終更新日時 = 最終更新日時;
				this.ファイルサイズ = ファイルサイズ;
			}
		}

		public ST譜面情報 譜面情報;
		[StructLayout( LayoutKind.Sequential )]
		public struct ST譜面情報
		{
			public string タイトル;
			public string アーティスト名;
			public string コメント;
			public string ジャンル;
			public string Preimage;
			public string Premovie;
			public string Presound;
			public string Backgound;
			public STDGBVALUE<int> レベル;
			public STRANK 最大ランク;
			public STSKILL 最大スキル;
			public STDGBVALUE<bool> フルコンボ;
			public STDGBVALUE<int> 演奏回数;
			public STHISTORY 演奏履歴;
			public bool レベルを非表示にする;
			public CDTX.E種別 曲種別;

			[StructLayout( LayoutKind.Sequential )]
			public struct STHISTORY
			{
				public string 行1;
				public string 行2;
				public string 行3;
				public string 行4;
				public string 行5;
				public string this[ int index ]
				{
					get
					{
						switch( index )
						{
							case 0:
								return this.行1;

							case 1:
								return this.行2;

							case 2:
								return this.行3;

							case 3:
								return this.行4;

							case 4:
								return this.行5;
						}
						throw new IndexOutOfRangeException();
					}
					set
					{
						switch( index )
						{
							case 0:
								this.行1 = value;
								return;

							case 1:
								this.行2 = value;
								return;

							case 2:
								this.行3 = value;
								return;

							case 3:
								this.行4 = value;
								return;

							case 4:
								this.行5 = value;
								return;
						}
						throw new IndexOutOfRangeException();
					}
				}
			}

			[StructLayout( LayoutKind.Sequential )]
			public struct STRANK
			{
				public int Drums;
				public int Guitar;
				public int Bass;
				public int this[ int index ]
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
						if( ( value < 0 ) || ( ( value != 0x63 ) && ( value > 6 ) ) )
						{
							throw new ArgumentOutOfRangeException();
						}
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

			[StructLayout( LayoutKind.Sequential )]
			public struct STSKILL
			{
				public double Drums;
				public double Guitar;
				public double Bass;
				public double this[ int index ]
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
						if( ( value < 0.0 ) || ( value > 100.0 ) )
						{
							throw new ArgumentOutOfRangeException();
						}
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
		}

		public bool bSongDBにキャッシュがあった;
		public bool bスコアが有効である
		{
			get
			{
				return ( ( ( this.譜面情報.レベル[ 0 ] + this.譜面情報.レベル[ 1 ] ) + this.譜面情報.レベル[ 2 ] ) != 0 );
			}
		}


		// コンストラクタ

		public Cスコア()
		{
			this.ScoreIni情報 = new STScoreIni情報( DateTime.MinValue, 0L );
			this.bSongDBにキャッシュがあった = false;
			this.ファイル情報 = new STファイル情報( "", "", DateTime.MinValue, 0L );
			ST譜面情報 st譜面情報 = new ST譜面情報();
			st譜面情報.タイトル = "";
			st譜面情報.アーティスト名 = "";
			st譜面情報.コメント = "";
			st譜面情報.ジャンル = "";
			st譜面情報.Preimage = "";
			st譜面情報.Premovie = "";
			st譜面情報.Presound = "";
			st譜面情報.Backgound = "";
			st譜面情報.レベル = new STDGBVALUE<int>();
			ST譜面情報.STRANK strank = new ST譜面情報.STRANK();
			strank.Drums = 0x63;
			strank.Guitar = 0x63;
			strank.Bass = 0x63;
			st譜面情報.最大ランク = strank;
			st譜面情報.フルコンボ = new STDGBVALUE<bool>();
			st譜面情報.演奏回数 = new STDGBVALUE<int>();
			ST譜面情報.STHISTORY sthistory = new ST譜面情報.STHISTORY();
			sthistory.行1 = "";
			sthistory.行2 = "";
			sthistory.行3 = "";
			sthistory.行4 = "";
			sthistory.行5 = "";
			st譜面情報.演奏履歴 = sthistory;
			st譜面情報.レベルを非表示にする = false;
			st譜面情報.最大スキル = new ST譜面情報.STSKILL();
			st譜面情報.曲種別 = CDTX.E種別.DTX;
			this.譜面情報 = st譜面情報;
		}
	}
}
