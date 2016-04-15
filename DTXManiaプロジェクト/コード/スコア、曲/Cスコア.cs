using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace DTXMania
{
	[Serializable]
	internal class Cスコア
	{
		// プロパティ

		public STScoreIni情報 ScoreIni情報;
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct STScoreIni情報
		{
			public DateTime 最終更新日時;
			public long ファイルサイズ;

			public STScoreIni情報(DateTime 最終更新日時, long ファイルサイズ)
			{
				this.最終更新日時 = 最終更新日時;
				this.ファイルサイズ = ファイルサイズ;
			}
		}

		public STファイル情報 ファイル情報;
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct STファイル情報
		{
			public string ファイルの絶対パス;
			public string フォルダの絶対パス;
			public DateTime 最終更新日時;
			public long ファイルサイズ;

			public STファイル情報(string ファイルの絶対パス, string フォルダの絶対パス, DateTime 最終更新日時, long ファイルサイズ)
			{
				this.ファイルの絶対パス = ファイルの絶対パス;
				this.フォルダの絶対パス = フォルダの絶対パス;
				this.最終更新日時 = 最終更新日時;
				this.ファイルサイズ = ファイルサイズ;
			}
		}

		public ST譜面情報 譜面情報;
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
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
			public STDGBSValue<int> レベル;
			public STDGBSValue<CScoreIni.ERANK> 最大ランク;
			public STDGBSValue<double> 最大スキル;
			public STDGBSValue<bool> フルコンボ;
			public STDGBSValue<int> 演奏回数;
			public STDGBSValue<EUseLanes> 使用レーン数;
			public STHISTORY 演奏履歴;
			public bool レベルを非表示にする;
			public EDTX種別 曲種別;
			public double Bpm;
			public int Duration;

			[Serializable]
			[StructLayout(LayoutKind.Sequential)]
			public struct STHISTORY
			{
				public string 行1;
				public string 行2;
				public string 行3;
				public string 行4;
				public string 行5;
				public string this[int index]
				{
					get
					{
						switch (index)
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
						switch (index)
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
		}

		public bool bSongDBにキャッシュがあった;
		public bool bスコアが有効である
		{
			get
			{
				return
					this.譜面情報.レベル.Drums +
					this.譜面情報.レベル.Guitar +
					this.譜面情報.レベル.Bass != 0;
			}
		}


		// コンストラクタ

		public Cスコア()
		{
			this.ScoreIni情報 = new STScoreIni情報(DateTime.MinValue, 0L);
			this.bSongDBにキャッシュがあった = false;
			this.ファイル情報 = new STファイル情報("", "", DateTime.MinValue, 0L);
			this.譜面情報 = new ST譜面情報();
			this.譜面情報.タイトル = "";
			this.譜面情報.アーティスト名 = "";
			this.譜面情報.コメント = "";
			this.譜面情報.ジャンル = "";
			this.譜面情報.Preimage = "";
			this.譜面情報.Premovie = "";
			this.譜面情報.Presound = "";
			this.譜面情報.Backgound = "";
			this.譜面情報.レベル = new STDGBSValue<int>();
			this.譜面情報.最大ランク = new STDGBSValue<CScoreIni.ERANK>();
			this.譜面情報.最大ランク.Drums = CScoreIni.ERANK.UNKNOWN;
			this.譜面情報.最大ランク.Guitar = CScoreIni.ERANK.UNKNOWN;
			this.譜面情報.最大ランク.Bass = CScoreIni.ERANK.UNKNOWN;
			this.譜面情報.フルコンボ = new STDGBSValue<bool>();
			this.譜面情報.演奏回数 = new STDGBSValue<int>();
			this.譜面情報.演奏履歴 = new ST譜面情報.STHISTORY();
			this.譜面情報.演奏履歴.行1 = "";
			this.譜面情報.演奏履歴.行2 = "";
			this.譜面情報.演奏履歴.行3 = "";
			this.譜面情報.演奏履歴.行4 = "";
			this.譜面情報.演奏履歴.行5 = "";
			this.譜面情報.レベルを非表示にする = false;
			this.譜面情報.最大スキル = new STDGBSValue<double>();
			this.譜面情報.曲種別 = EDTX種別.DTX;
			this.譜面情報.Bpm = 120.0;
			this.譜面情報.Duration = 0;
			this.譜面情報.使用レーン数 = new STDGBSValue<EUseLanes>();
		}
	}
}
