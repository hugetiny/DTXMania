using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using SharpDX;
using FDK;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActSelect曲リスト : CActivity
	{
		// プロパティ

		public bool bIsEnumeratingSongs
		{
			get;
			set;
		}
		public bool bスクロール中
		{
			get
			{
				if (this.n目標のスクロールカウンタ == 0)
				{
					return (this.n現在のスクロールカウンタ != 0);
				}
				return true;
			}
		}
		public int n現在のアンカ難易度レベル
		{
			get;
			private set;
		}
		public int n現在選択中の曲の現在の難易度レベル
		{
			get
			{
				return this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r現在選択中の曲);
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				if (this.r現在選択中の曲 != null)
				{
					return this.r現在選択中の曲.arスコア[this.n現在選択中の曲の現在の難易度レベル];
				}
				return null;
			}
		}
		public C曲リストノード r現在選択中の曲
		{
			get;
			private set;
		}

		public int nスクロールバー相対y座標
		{
			get;
			private set;
		}

		// t選択曲が変更された()内で使う、直前の選曲の保持
		// (前と同じ曲なら選択曲変更に掛かる再計算を省略して高速化するため)
		private C曲リストノード song_last = null;


		// コンストラクタ

		public CActSelect曲リスト()
		{
			this.r現在選択中の曲 = null;
			this.n現在のアンカ難易度レベル = 0;
			base.b活性化してない = true;
			this.bIsEnumeratingSongs = false;
		}


		// メソッド

		public int n現在のアンカ難易度レベルに最も近い難易度レベルを返す(C曲リストノード song)
		{
			// 事前チェック。

			if (song == null)
				return this.n現在のアンカ難易度レベル;  // 曲がまったくないよ

			if (song.arスコア[this.n現在のアンカ難易度レベル] != null)
				return this.n現在のアンカ難易度レベル;  // 難易度ぴったりの曲があったよ

			if ((song.eノード種別 == C曲リストノード.Eノード種別.BOX) || (song.eノード種別 == C曲リストノード.Eノード種別.BACKBOX))
				return 0;               // BOX と BACKBOX は関係無いよ


			// 現在のアンカレベルから、難易度上向きに検索開始。

			int n最も近いレベル = this.n現在のアンカ難易度レベル;

			for (int i = 0; i < 5; i++)
			{
				if (song.arスコア[n最も近いレベル] != null)
					break;  // 曲があった。

				n最も近いレベル = (n最も近いレベル + 1) % 5;  // 曲がなかったので次の難易度レベルへGo。（5以上になったら0に戻る。）
			}


			// 見つかった曲がアンカより下のレベルだった場合……
			// アンカから下向きに検索すれば、もっとアンカに近い曲があるんじゃね？

			if (n最も近いレベル < this.n現在のアンカ難易度レベル)
			{
				// 現在のアンカレベルから、難易度下向きに検索開始。

				n最も近いレベル = this.n現在のアンカ難易度レベル;

				for (int i = 0; i < 5; i++)
				{
					if (song.arスコア[n最も近いレベル] != null)
						break;  // 曲があった。

					n最も近いレベル = ((n最も近いレベル - 1) + 5) % 5;  // 曲がなかったので次の難易度レベルへGo。（0未満になったら4に戻る。）
				}
			}

			return n最も近いレベル;
		}
		public C曲リストノード r指定された曲が存在するリストの先頭の曲(C曲リストノード song)
		{
			List<C曲リストノード> songList = GetSongListWithinMe(song);
			return (songList == null) ? null : songList[0];
		}
		public C曲リストノード r指定された曲が存在するリストの末尾の曲(C曲リストノード song)
		{
			List<C曲リストノード> songList = GetSongListWithinMe(song);
			return (songList == null) ? null : songList[songList.Count - 1];
		}

		private List<C曲リストノード> GetSongListWithinMe(C曲リストノード song)
		{
			if (song.r親ノード == null)         // root階層のノートだったら
			{
				return CDTXMania.Instance.Songs管理.list曲ルート; // rootのリストを返す
			}
			else
			{
				if ((song.r親ノード.list子リスト != null) && (song.r親ノード.list子リスト.Count > 0))
				{
					return song.r親ノード.list子リスト;
				}
				else
				{
					return null;
				}
			}
		}


		public delegate void DGSortFunc(List<C曲リストノード> songList, EPart eInst, int order, params object[] p);
		/// <summary>
		/// 主にCSong管理.cs内にあるソート機能を、delegateで呼び出す。
		/// </summary>
		/// <param name="sf">ソート用に呼び出すメソッド</param>
		/// <param name="eInst">ソート基準とする楽器</param>
		/// <param name="order">-1=降順, 1=昇順</param>
		public void t曲リストのソート(DGSortFunc sf, EPart eInst, int order, params object[] p)
		{
			List<C曲リストノード> songList = GetSongListWithinMe(this.r現在選択中の曲);
			if (songList == null)
			{
				// 何もしない;
			}
			else
			{
				//				CDTXMania.Instance.Songs管理.t曲リストのソート3_演奏回数の多い順( songList, eInst, order );
				sf(songList, eInst, order, p);
				//				this.r現在選択中の曲 = CDTXMania
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}

		public bool tBOXに入る()
		{
			//Trace.TraceInformation( "box enter" );
			//Trace.TraceInformation( "Skin現在Current : " + CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName( false ) );
			//Trace.TraceInformation( "Skin現在System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在: " + CSkin.GetSkinName( CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName( false ) ) );
			//Trace.TraceInformation( "Skin現pt: " + CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName( false ) );
			//Trace.TraceInformation( "Skin指定: " + CSkin.GetSkinName( this.r現在選択中の曲.strSkinPath ) );
			//Trace.TraceInformation( "Skinpath: " + this.r現在選択中の曲.strSkinPath );
			bool ret = false;
			if (CSkin.GetSkinName(CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false)) != CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& CSkin.bUseBoxDefSkin)
			{
				ret = true;
				// BOXに入るときは、スキン変更発生時のみboxdefスキン設定の更新を行う
				CDTXMania.Instance.Skin.SetCurrentSkinSubfolderFullName(this.r現在選択中の曲.strSkinPath, false);
			}

			//Trace.TraceInformation( "Skin変更: " + CSkin.GetSkinName( CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName( false ) ) );
			//Trace.TraceInformation( "Skin変更Current : " + CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName( false ) );
			//Trace.TraceInformation( "Skin変更System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin変更BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );

			if ((this.r現在選択中の曲.list子リスト != null) && (this.r現在選択中の曲.list子リスト.Count > 0))
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.list子リスト[0];
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);                 // #27648 項目数変更を反映させる
			}
			return ret;
		}
		public bool tBOXを出る()
		{
			//Trace.TraceInformation( "box exit" );
			//Trace.TraceInformation( "Skin現在Current : " + CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin現在System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在: " + CSkin.GetSkinName( CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin現pt: " + CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin指定: " + CSkin.GetSkinName( this.r現在選択中の曲.strSkinPath ) );
			//Trace.TraceInformation( "Skinpath: " + this.r現在選択中の曲.strSkinPath );
			bool ret = false;
			if (CSkin.GetSkinName(CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false)) != CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& CSkin.bUseBoxDefSkin)
			{
				ret = true;
			}
			// スキン変更が発生しなくても、boxdef圏外に出る場合は、boxdefスキン設定の更新が必要
			// (ユーザーがboxdefスキンをConfig指定している場合への対応のために必要)
			// tBoxに入る()とは処理が微妙に異なるので注意
			CDTXMania.Instance.Skin.SetCurrentSkinSubfolderFullName(
				(this.r現在選択中の曲.strSkinPath == "") ? "" : CDTXMania.Instance.Skin.GetSkinSubfolderFullNameFromSkinName(CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)), false);
			//Trace.TraceInformation( "SKIN変更: " + CSkin.GetSkinName( CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "SKIN変更Current : "+  CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "SKIN変更System  : "+  CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "SKIN変更BoxDef  : "+  CSkin.strBoxDefSkinSubfolderFullName );
			if (this.r現在選択中の曲.r親ノード != null)
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.r親ノード;
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);                 // #27648 項目数変更を反映させる
			}
			return ret;
		}
		public void t現在選択中の曲を元に曲バーを再構成する()
		{
			this.tバーの初期化();
			for (int i = 0; i < 13; i++)
			{
				this.t曲名バーの生成(i, this.stバー情報[i].strタイトル文字列, this.stバー情報[i].col文字色);
			}
		}
		public void t次に移動()
		{
			if (this.r現在選択中の曲 != null)
			{
				this.n目標のスクロールカウンタ += 100;
			}
		}
		public void t前に移動()
		{
			if (this.r現在選択中の曲 != null)
			{
				this.n目標のスクロールカウンタ -= 100;
			}
		}
		public void t難易度レベルをひとつ進める()
		{
			if ((this.r現在選択中の曲 == null) || (this.r現在選択中の曲.nスコア数 <= 1))
				return;   // 曲にスコアが０～１個しかないなら進める意味なし。


			// 難易度レベルを＋１し、現在選曲中のスコアを変更する。

			this.n現在のアンカ難易度レベル = this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r現在選択中の曲);

			for (int i = 0; i < 5; i++)
			{
				this.n現在のアンカ難易度レベル = (this.n現在のアンカ難易度レベル + 1) % 5;  // ５以上になったら０に戻る。
				if (this.r現在選択中の曲.arスコア[this.n現在のアンカ難易度レベル] != null)  // 曲が存在してるならここで終了。存在してないなら次のレベルへGo。
					break;
			}


			// 曲毎に表示しているスキル値を、新しい難易度レベルに合わせて取得し直す。（表示されている13曲全部。）

			C曲リストノード song = this.r現在選択中の曲;
			for (int i = 0; i < 5; i++)
				song = this.r前の曲(song);

			for (int i = this.n現在の選択行 - 5; i < ((this.n現在の選択行 - 5) + 13); i++)
			{
				int index = (i + 13) % 13;
				for (EPart m = EPart.Drums; m <= EPart.Bass; m++)
				{
					this.stバー情報[index].nスキル値[m] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[m];
					this.stバー情報[index].e使用レーン数[m] = song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.使用レーン数[m];
				}
				song = this.r次の曲(song);
			}


			// 選曲ステージに変更通知を発出し、関係Activityの対応を行ってもらう。

			CDTXMania.Instance.stage選曲.t選択曲変更通知();
		}


		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh(CSongs管理 cs, bool bRemakeSongTitleBar)    // #26070 2012.2.28 yyagi
		{
			if (cs != null && cs.list曲ルート.Count > 0)  // 新しい曲リストを検索して、1曲以上あった
			{
				this.On非活性化();
				CDTXMania.Instance.Songs管理 = cs;
				this.On活性化();

				if (this.r現在選択中の曲 != null)      // r現在選択中の曲==null とは、「最初songlist.dbが無かった or 検索したが1曲もない」
				{
					this.r現在選択中の曲 = searchCurrentBreadcrumbsPosition(CDTXMania.Instance.Songs管理.list曲ルート, this.r現在選択中の曲.strBreadcrumbs);
					if (bRemakeSongTitleBar)          // 選曲画面以外に居るときには再構成しない (非活性化しているときに実行すると例外となる)
					{
						this.t現在選択中の曲を元に曲バーを再構成する();
					}
#if false      // list子リストの中まではmatchしてくれないので、検索ロジックは手書きで実装 (searchCurrentBreadcrumbs())
					string bc = this.r現在選択中の曲.strBreadcrumbs;
					Predicate<C曲リストノード> match = delegate( C曲リストノード c )
					{
						return ( c.strBreadcrumbs.Equals( bc ) );
					};
					int nMatched = CDTXMania.Instance.Songs管理.list曲ルート.FindIndex( match );

					this.r現在選択中の曲 = ( nMatched == -1 ) ? null : CDTXMania.Instance.Songs管理.list曲ルート[ nMatched ];
					this.t現在選択中の曲を元に曲バーを再構成する();
#endif
					return;
				}
			}
			//this.On非活性化();
			this.r現在選択中の曲 = null;
			//this.On活性化();
		}


		/// <summary>
		/// 現在選曲している位置を検索する
		/// (曲一覧クラスを新しいものに入れ替える際に用いる)
		/// </summary>
		/// <param name="ln">検索対象のList</param>
		/// <param name="bc">検索するパンくずリスト(文字列)</param>
		/// <returns></returns>
		private C曲リストノード searchCurrentBreadcrumbsPosition(List<C曲リストノード> ln, string bc)
		{
			foreach (C曲リストノード n in ln)
			{
				if (n.strBreadcrumbs == bc)
				{
					return n;
				}
				else if (n.list子リスト != null && n.list子リスト.Count > 0)  // 子リストが存在するなら、再帰で探す
				{
					C曲リストノード r = searchCurrentBreadcrumbsPosition(n.list子リスト, bc);
					if (r != null) return r;
				}
			}
			return null;
		}

		/// <summary>
		/// BOXのアイテム数と、今何番目を選択しているかをセットする
		/// </summary>
		public void t選択曲が変更された(bool bForce) // #27648
		{
			C曲リストノード song = CDTXMania.Instance.stage選曲.r現在選択中の曲;
			if (song == null)
				return;
			if (song == song_last && bForce == false)
				return;

			song_last = song;
			List<C曲リストノード> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : CDTXMania.Instance.Songs管理.list曲ルート;
			int index = list.IndexOf(song) + 1;
			if (index <= 0)
			{
				nCurrentPosition = nNumOfItems = 0;
			}
			else
			{
				nCurrentPosition = index;
				nNumOfItems = list.Count;
			}
		}

		// CActivity 実装

		public override void On活性化()
		{
			if (this.b活性化してる)
				return;

			this.e楽器パート = EPart.Drums;
			this.b登場アニメ全部完了 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。

			FontStyle regular = FontStyle.Regular;
			if (CDTXMania.Instance.ConfigIni.bItalicFontSongSelect) regular |= FontStyle.Italic;
			if (CDTXMania.Instance.ConfigIni.bBoldFontSongSelect) regular |= FontStyle.Bold;

			string fontname = CDTXMania.Instance.Resources.Explanation("strCfgSelectMusicSongListFontFileName");
			string path = Path.Combine(@"Graphics\fonts", fontname);
			this.privateFont_SongList = new CPrivateFastFont(CSkin.Path(path), (float)(CDTXMania.Instance.ConfigIni.nFontSizeDotSongSelect * 2 * Scale.Y * 72f / 96f), regular);
																								// 72f/96f: 従来互換のために追加。DPI依存→非依存化に付随した変更。
			this.ft曲リスト用フォント = this.privateFont_SongList.font;

			//this.ft曲リスト用フォント = new Font(
			//	CDTXMania.Instance.ConfigIni.strFontSongSelect,
			//	(float)(CDTXMania.Instance.ConfigIni.nFontSizeDotSongSelect * 2 * Scale.Y),   // 後でScale.Yを掛けないように直すこと(Config.ini初期値変更)
			//	regular,
			//	GraphicsUnit.Pixel
			//);



			// 現在選択中の曲がない（＝はじめての活性化）なら、現在選択中の曲をルートの先頭ノードに設定する。

			if ((this.r現在選択中の曲 == null) && (CDTXMania.Instance.Songs管理.list曲ルート.Count > 0))
				this.r現在選択中の曲 = CDTXMania.Instance.Songs管理.list曲ルート[0];


			// バー情報を初期化する。

			this.tバーの初期化();

			base.On活性化();

			this.t選択曲が変更された(true);    // #27648 2012.3.31 yyagi 選曲画面に入った直後の 現在位置/全アイテム数 の表示を正しく行うため
		}
		public override void On非活性化()
		{
			if (this.b活性化してない)
				return;

			TextureFactory.t安全にDisposeする(ref this.ft曲リスト用フォント);

			for (int i = 0; i < 13; i++)
				this.ct登場アニメ用[i] = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (this.b活性化してない)
				return;

			this.tx曲名バー.Score = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar score.png"), false);
			this.tx曲名バー.Box = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar box.png"), false);
			this.tx曲名バー.Other = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar other.png"), false);

			this.tx選曲バー.Score = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar score selected.png"), false);
			this.tx選曲バー.Box = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar box selected.png"), false);
			this.tx選曲バー.Other = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect bar other selected.png"), false);

			this.txスキル数字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill number on list.png"), false);
			this.tx使用レーン数数字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill number on gauge etc.png"), false);
			for (int i = 0; i < 13; i++)
				this.t曲名バーの生成(i, this.stバー情報[i].strタイトル文字列, this.stバー情報[i].col文字色);

			int c = (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ? 0 : 1;
			#region [ Songs not found画像 ]
			try
			{
				using (Bitmap image = new Bitmap(SampleFramework.GameWindowSize.Width, (int)(128 * Scale.Y)))
				using (Graphics graphics = Graphics.FromImage(image))
				{
					string[] s1 = { "曲データが見つかりません。", "Songs not found." };
					string[] s2 = { "曲データをDTXManiaGR.exe以下の", "You need to install songs." };
					string[] s3 = { "フォルダにインストールして下さい。", "" };
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)(2f * Scale.X), (float)(2f * Scale.Y));
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)0f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)(2f * Scale.X), (float)(44f * Scale.Y));
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)(42f * Scale.Y));
					graphics.DrawString(s3[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)(2f * Scale.X), (float)(86f * Scale.Y));
					graphics.DrawString(s3[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)(84f * Scale.Y));

					this.txSongNotFound = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);

					this.txSongNotFound.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f); // 半分のサイズで表示する。
				}
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("SoungNotFoundテクスチャの作成に失敗しました。");
				this.txSongNotFound = null;
			}
			#endregion
			#region [ "曲データを検索しています"画像 ]
			try
			{
				using (Bitmap image = new Bitmap(SampleFramework.GameWindowSize.Width, (int)(96 * Scale.Y)))
				using (Graphics graphics = Graphics.FromImage(image))
				{
					string[] s1 = { "曲データを検索しています。", "Now enumerating songs." };
					string[] s2 = { "そのまましばらくお待ち下さい。", "Please wait..." };
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)(2f * Scale.X), (float)(2f * Scale.Y));
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)0f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)(2f * Scale.X), (float)(44f * Scale.Y));
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)(42f * Scale.Y));

					this.txEnumeratingSongs = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);

					this.txEnumeratingSongs.vc拡大縮小倍率 = new Vector3(0.5f, 0.5f, 1f); // 半分のサイズで表示する。
				}
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("txEnumeratingSongsテクスチャの作成に失敗しました。");
				this.txEnumeratingSongs = null;
			}
			#endregion
			#region [ 曲数表示 ]
			this.txアイテム数数字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenSelect skill number on gauge etc.png"), false);
			#endregion
			base.OnManagedリソースの作成();
		}
		public override void OnManagedリソースの解放()
		{
			if (this.b活性化してない)
				return;

			TextureFactory.t安全にDisposeする(ref this.txアイテム数数字);

			for (int i = 0; i < 13; i++)
				TextureFactory.t安全にDisposeする(ref this.stバー情報[i].txタイトル名);

			TextureFactory.t安全にDisposeする(ref this.txスキル数字);
			TextureFactory.t安全にDisposeする(ref this.tx使用レーン数数字);
			TextureFactory.t安全にDisposeする(ref this.txEnumeratingSongs);
			TextureFactory.t安全にDisposeする(ref this.txSongNotFound);
			TextureFactory.t安全にDisposeする(ref this.tx曲名バー.Score);
			TextureFactory.t安全にDisposeする(ref this.tx曲名バー.Box);
			TextureFactory.t安全にDisposeする(ref this.tx曲名バー.Other);
			TextureFactory.t安全にDisposeする(ref this.tx選曲バー.Score);
			TextureFactory.t安全にDisposeする(ref this.tx選曲バー.Box);
			TextureFactory.t安全にDisposeする(ref this.tx選曲バー.Other);

			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			if (this.b活性化してない)
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if (this.b初めての進行描画)
			{
				for (int i = 0; i < 13; i++)
					this.ct登場アニメ用[i] = new CCounter(-i * 10, 100, 3, CDTXMania.Instance.Timer);

				this.nスクロールタイマ = CSound管理.rc演奏用タイマ.n現在時刻;
				CDTXMania.Instance.stage選曲.t選択曲変更通知();

				base.b初めての進行描画 = false;
			}
			//-----------------
			#endregion


			// まだ選択中の曲が決まってなければ、曲ツリールートの最初の曲にセットする。

			if ((this.r現在選択中の曲 == null) && (CDTXMania.Instance.Songs管理.list曲ルート.Count > 0))
				this.r現在選択中の曲 = CDTXMania.Instance.Songs管理.list曲ルート[0];


			// 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。
			// ２つしかフェーズがないので CStage.eフェーズID を使ってないところがまた本末転倒。


			// 進行。

			if (!this.b登場アニメ全部完了)
			{
				#region [ (1) 登場アニメフェーズの進行。]
				//-----------------
				for (int i = 0; i < 13; i++)  // パネルは全13枚。
				{
					this.ct登場アニメ用[i].t進行();

					if (this.ct登場アニメ用[i].b終了値に達した)
						this.ct登場アニメ用[i].t停止();
				}

				// 全部の進行が終わったら、this.b登場アニメ全部完了 を true にする。

				this.b登場アニメ全部完了 = true;
				for (int i = 0; i < 13; i++)  // パネルは全13枚。
				{
					if (this.ct登場アニメ用[i].b進行中)
					{
						this.b登場アニメ全部完了 = false;  // まだ進行中のアニメがあるなら false のまま。
						break;
					}
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (2) 通常フェーズの進行。]
				//-----------------
				long n現在時刻 = CSound管理.rc演奏用タイマ.n現在時刻;

				if (n現在時刻 < this.nスクロールタイマ) // 念のため
					this.nスクロールタイマ = n現在時刻;

				const int nアニメ間隔 = 2;
				while ((n現在時刻 - this.nスクロールタイマ) >= nアニメ間隔)
				{
					int n加速度 = 1;
					int n残距離 = Math.Abs((int)(this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ));

					#region [ 残距離が遠いほどスクロールを速くする（＝n加速度を多くする）。]
					//-----------------
					if (n残距離 <= 100)
					{
						n加速度 = 2;
					}
					else if (n残距離 <= 300)
					{
						n加速度 = 3;
					}
					else if (n残距離 <= 500)
					{
						n加速度 = 4;
					}
					else
					{
						n加速度 = 8;
					}
					//-----------------
					#endregion

					#region [ 加速度を加算し、現在のスクロールカウンタを目標のスクロールカウンタまで近づける。 ]
					//-----------------
					if (this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ)    // (A) 正の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ += n加速度;               // カウンタを正方向に移動する。

						if (this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ)
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;  // 到着！スクロール停止！
					}

					else if (this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ) // (B) 負の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ -= n加速度;               // カウンタを負方向に移動する。

						if (this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ)  // 到着！スクロール停止！
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
					}
					//-----------------
					#endregion

					if (this.n現在のスクロールカウンタ >= 100)    // １行＝100カウント。
					{
						#region [ パネルを１行上にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ下の行に移動。

						this.r現在選択中の曲 = this.r次の曲(this.r現在選択中の曲);
						this.n現在の選択行 = (this.n現在の選択行 + 1) % 13;


						// 選択曲から７つ下のパネル（＝新しく最下部に表示されるパネル。消えてしまう一番上のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for (int i = 0; i < 7; i++)
							song = this.r次の曲(song);

						int index = (this.n現在の選択行 + 7) % 13;  // 新しく最下部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[index].strタイトル文字列 = song.strタイトル;
						this.stバー情報[index].col文字色 = song.col文字色;
						this.t曲名バーの生成(index, this.stバー情報[index].strタイトル文字列, this.stバー情報[index].col文字色);


						// stバー情報[] の内容を1行ずつずらす。

						C曲リストノード song2 = this.r現在選択中の曲;
						for (int i = 0; i < 5; i++)
							song2 = this.r前の曲(song2);

						for (int i = 0; i < 13; i++)
						{
							int n = (((this.n現在の選択行 - 5) + i) + 13) % 13;
							this.stバー情報[n].eバー種別 = this.e曲のバー種別を返す(song2);
							song2 = this.r次の曲(song2);
						}


						// 新しく最下部に表示されるパネル用のスキル値を取得。

						for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
						{
							this.stバー情報[index].nスキル値[i] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[i];
							this.stバー情報[index].e使用レーン数[i] = song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.使用レーン数[i];
						}

						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;

						this.t選択曲が変更された(false);       // スクロールバー用に今何番目を選択しているかを更新

						if (this.n目標のスクロールカウンタ == 0)
							CDTXMania.Instance.stage選曲.t選択曲変更通知();    // スクロール完了＝選択曲変更！

						//-----------------
						#endregion
					}
					else if (this.n現在のスクロールカウンタ <= -100)
					{
						#region [ パネルを１行下にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ上の行に移動。

						this.r現在選択中の曲 = this.r前の曲(this.r現在選択中の曲);
						this.n現在の選択行 = ((this.n現在の選択行 - 1) + 13) % 13;


						// 選択曲から５つ上のパネル（＝新しく最上部に表示されるパネル。消えてしまう一番下のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for (int i = 0; i < 5; i++)
							song = this.r前の曲(song);

						int index = ((this.n現在の選択行 - 5) + 13) % 13; // 新しく最上部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[index].strタイトル文字列 = song.strタイトル;
						this.stバー情報[index].col文字色 = song.col文字色;
						this.t曲名バーの生成(index, this.stバー情報[index].strタイトル文字列, this.stバー情報[index].col文字色);


						// stバー情報[] の内容を1行ずつずらす。

						C曲リストノード song2 = this.r現在選択中の曲;
						for (int i = 0; i < 5; i++)
							song2 = this.r前の曲(song2);

						for (int i = 0; i < 13; i++)
						{
							int n = (((this.n現在の選択行 - 5) + i) + 13) % 13;
							this.stバー情報[n].eバー種別 = this.e曲のバー種別を返す(song2);
							song2 = this.r次の曲(song2);
						}


						// 新しく最上部に表示されるパネル用のスキル値を取得。

						for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
						{
							this.stバー情報[index].nスキル値[i] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[i];
							this.stバー情報[index].e使用レーン数[i] = song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.使用レーン数[i];
						}


						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;

						this.t選択曲が変更された(false);       // スクロールバー用に今何番目を選択しているかを更新

						if (this.n目標のスクロールカウンタ == 0)
							CDTXMania.Instance.stage選曲.t選択曲変更通知();    // スクロール完了＝選択曲変更！
																												//-----------------
						#endregion
					}

					this.nスクロールタイマ += nアニメ間隔;
				}
				//-----------------
				#endregion
			}


			// 描画。

			if (this.r現在選択中の曲 == null)
			{
				#region [ 曲が１つもないなら「Songs not found.」を表示してここで帰れ。]
				//-----------------
				if (bIsEnumeratingSongs)
				{
					if (this.txEnumeratingSongs != null)
					{
						this.txEnumeratingSongs.t2D描画(
							CDTXMania.Instance.Device,
							320 * Scale.X,
							160 * Scale.Y
						);
					}
				}
				else
				{
					if (this.txSongNotFound != null)
						this.txSongNotFound.t2D描画(
							CDTXMania.Instance.Device,
							320 * Scale.X,
							160 * Scale.Y
						);
				}
				//-----------------
				#endregion

				return 0;
			}

			if (!this.b登場アニメ全部完了)
			{
				#region [ (1) 登場アニメフェーズの描画。]
				//-----------------
				for (int i = 0; i < 13; i++)  // パネルは全13枚。
				{
					if (this.ct登場アニメ用[i].n現在の値 >= 0)
					{
						double db割合0to1 = ((double)this.ct登場アニメ用[i].n現在の値) / 100.0;
						double db回転率 = Math.Sin(Math.PI * 3 / 5 * db割合0to1);
						int nパネル番号 = (((this.n現在の選択行 - 5) + i) + 13) % 13;

						if (i == 5)
						{
							// (A) 選択曲パネルを描画。

							#region [ バーテクスチャを描画。]
							//-----------------
							int width = (int)((425.0 - 0.8f) * Scale.X / Math.Sin(Math.PI * 3 / 5));
							//int width = (int) ( ( (double) ( ( SampleFramework.GameWindowSize.Width - this.ptバーの基本座標[ i ].X ) + 1 ) ) / Math.Sin( Math.PI * 3 / 5 ) );
							int x = SampleFramework.GameWindowSize.Width - ((int)(width * db回転率));
							int y = 415;
							this.tバーの描画(x, y, this.stバー情報[nパネル番号].eバー種別, true);
							//-----------------
							#endregion
							#region [ タイトル名テクスチャを描画。]
							//-----------------
							if (this.stバー情報[nパネル番号].txタイトル名 != null)
								this.stバー情報[nパネル番号].txタイトル名.t2D描画(
									CDTXMania.Instance.Device,
									(x + (int)(44 * Scale.X)) + (int)((16f + 0.5f) * Scale.X),
									y + 35
								//( y + (int) ( 0 * Scale.Y ) ) + (int) ( 16 * Scale.Y )
								// ( y + (int) ( 5 * Scale.Y ) ) + (int) ( (16f-2f) * Scale.Y)
								);
							//-----------------
							#endregion
							#region [ スキル値を描画。]
							//-----------------
							if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
								this.tスキル値の描画(x + (int)(28 * Scale.X), y + (int)(59), this.stバー情報[nパネル番号].nスキル値[this.e楽器パート]);
							//-----------------
							#endregion
							#region [ 使用レーン数を描画。]
							//-----------------
							if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
								this.t使用レーン数の描画(x + (int)(0 * Scale.X), y + (int)53, this.stバー情報[nパネル番号].e使用レーン数[this.e楽器パート]);
							//-----------------
							#endregion
						}
						else
						{
							// (B) その他のパネルの描画。

							#region [ バーテクスチャの描画。]
							//-----------------
							int width = (int)(((double)((SampleFramework.GameWindowSize.Width - this.ptバーの基本座標[i].X) + 1)) / Math.Sin(Math.PI * 3 / 5));
							int x = SampleFramework.GameWindowSize.Width - ((int)(width * db回転率));
							int y = this.ptバーの基本座標[i].Y;
							this.tバーの描画(x, y, this.stバー情報[nパネル番号].eバー種別, false);
							//-----------------
							#endregion
							#region [ タイトル名テクスチャを描画。]
							//-----------------
							if (this.stバー情報[nパネル番号].txタイトル名 != null)
								this.stバー情報[nパネル番号].txタイトル名.t2D描画(
									CDTXMania.Instance.Device,
									x + (int)(44 * Scale.X),
									y + 7
								);
							//-----------------
							#endregion
							#region [ スキル値を描画。]
							//-----------------
							if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
								this.tスキル値の描画(x + (int)(14 * Scale.X), y + (int)(14 * Scale.Y), this.stバー情報[nパネル番号].nスキル値[this.e楽器パート]);
							//-----------------
							#endregion
							#region [ 使用レーン数を描画。]
							//-----------------
							if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
								this.t使用レーン数の描画(x + (int)(-14 * Scale.X), y + (int)(11 * Scale.Y), this.stバー情報[nパネル番号].e使用レーン数[this.e楽器パート]);
							//-----------------
							#endregion
						}
					}
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (2) 通常フェーズの描画。]
				//-----------------
				for (int i = 0; i < 13; i++)  // パネルは全13枚。
				{
					if ((i == 0 && this.n現在のスクロールカウンタ > 0) ||   // 最上行は、上に移動中なら表示しない。
						(i == 12 && this.n現在のスクロールカウンタ < 0))    // 最下行は、下に移動中なら表示しない。
						continue;

					int nパネル番号 = (((this.n現在の選択行 - 5) + i) + 13) % 13;
					int n見た目の行番号 = i;
					int n次のパネル番号 = (this.n現在のスクロールカウンタ <= 0) ? ((i + 1) % 13) : (((i - 1) + 13) % 13);
					int x = this.ptバーの基本座標[n見た目の行番号].X + ((int)((this.ptバーの基本座標[n次のパネル番号].X - this.ptバーの基本座標[n見た目の行番号].X) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));
					int y = this.ptバーの基本座標[n見た目の行番号].Y + ((int)((this.ptバーの基本座標[n次のパネル番号].Y - this.ptバーの基本座標[n見た目の行番号].Y) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));

					if ((i == 5) && (this.n現在のスクロールカウンタ == 0))
					{
						// (A) スクロールが停止しているときの選択曲バーの描画。

						#region [ バーテクスチャを描画。]
						//-----------------
						this.tバーの描画((int)(216 * Scale.X), (int)(415), this.stバー情報[nパネル番号].eバー種別, true);
						//-----------------
						#endregion
						#region [ タイトル名テクスチャを描画。]
						//-----------------
						if (this.stバー情報[nパネル番号].txタイトル名 != null)
							this.stバー情報[nパネル番号].txタイトル名.t2D描画(
								CDTXMania.Instance.Device,
								//0x114 * Scale.X,
								//(0xc9 - 1)* Scale.Y
								(x + ((44f + 0.5f) * Scale.X)),
								(y + 7)
							);
						//-----------------
						#endregion
						#region [ スキル値を描画。]
						//-----------------
						if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
							this.tスキル値の描画(
								(int)(0xf4 * Scale.X),
								(int)(0xd3 * Scale.Y),
								this.stバー情報[nパネル番号].nスキル値[this.e楽器パート]
							);
						//-----------------
						#endregion
						#region [ 使用レーン数を描画。]
						//-----------------
						if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
							this.t使用レーン数の描画(
								(int)(216 * Scale.X),
								(int)(208 * Scale.Y),
								this.stバー情報[nパネル番号].e使用レーン数[this.e楽器パート]
							);
						//-----------------
						#endregion
					}
					else
					{
						// (B) スクロール中の選択曲バー、またはその他のバーの描画。

						#region [ バーテクスチャを描画。]
						//-----------------
						this.tバーの描画(x, y, this.stバー情報[nパネル番号].eバー種別, false);
						//-----------------
						#endregion
						#region [ タイトル名テクスチャを描画。]
						//-----------------
						if (this.stバー情報[nパネル番号].txタイトル名 != null)
							this.stバー情報[nパネル番号].txタイトル名.t2D描画(
								CDTXMania.Instance.Device,
								x + (int)(44 * Scale.X),
								y + 7
							);
						//-----------------
						#endregion
						#region [ スキル値を描画。]
						//-----------------
						if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
							this.tスキル値の描画(x + (int)(14 * Scale.X), y + (int)(14 * Scale.Y), this.stバー情報[nパネル番号].nスキル値[this.e楽器パート]);
						//-----------------
						#endregion
						#region [ 使用レーン数を描画。]
						//-----------------
						if ((this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score) && (this.e楽器パート != EPart.Unknown))
							this.t使用レーン数の描画(x + (int)(-14 * Scale.X), y + (int)(11 * Scale.Y), this.stバー情報[nパネル番号].e使用レーン数[this.e楽器パート]);
						//-----------------
						#endregion
					}
				}
				//-----------------
				#endregion
			}
			#region [ スクロール地点の計算(描画はCActSelectShowCurrentPositionにて行う) #27648 ]
			int py;
			double d = 0;
			if (nNumOfItems > 1)
			{
				d = ((int)(336 * Scale.Y) - 6 - 8) / (double)(nNumOfItems - 1);
				py = (int)(d * (nCurrentPosition - 1));
			}
			else
			{
				d = 0;
				py = 0;
			}
			int delta = (int)(d * this.n現在のスクロールカウンタ / 100);
			if (py + delta <= (int)(336 * Scale.Y) - 6 - 8)
			{
				this.nスクロールバー相対y座標 = py + delta;
			}
			#endregion

			#region [ アイテム数の描画 #27648 ]
			tアイテム数の描画();
			#endregion
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private enum Eバー種別 { Score, Box, Other }

		private struct STバー
		{
			public CTexture Score;
			public CTexture Box;
			public CTexture Other;
			public CTexture this[int index]
			{
				get
				{
					switch (index)
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch (index)
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private struct STバー情報
		{
			public CActSelect曲リスト.Eバー種別 eバー種別;
			public string strタイトル文字列;
			public CTexture txタイトル名;
			public STDGBSValue<int> nスキル値;
			public STDGBSValue<EUseLanes> e使用レーン数;
			public Color col文字色;
		}

		private struct ST選曲バー
		{
			public CTexture Score;
			public CTexture Box;
			public CTexture Other;
			public CTexture this[int index]
			{
				get
				{
					switch (index)
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch (index)
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private bool b登場アニメ全部完了;
		private Color color文字影 = Color.FromArgb(0x40, 10, 10, 10);
		private CCounter[] ct登場アニメ用 = new CCounter[13];
		private EPart e楽器パート;
		private Font ft曲リスト用フォント;
		private CPrivateFastFont privateFont_SongList;
		private long nスクロールタイマ;
		private int n現在のスクロールカウンタ;
		private int n現在の選択行;
		private int n目標のスクロールカウンタ;
		private readonly Point[] ptバーの基本座標 = new Point[] {
			new Point( (int)(0x162 * Scale.X), (int)(20 * Scale.Y) ),
			new Point( (int)(0x139 * Scale.X), (int)(0x36 * Scale.Y) ),
			new Point( (int)(0x121 * Scale.X), (int)(0x58 * Scale.Y) ),
			new Point( (int)(0x111 * Scale.X), (int)(0x7a * Scale.Y) ),
			new Point( (int)(0x108 * Scale.X), (int)(0x9c * Scale.Y) ),
			new Point( (int)(0xe8 * Scale.X), (int)(0xc5 * Scale.Y) ),
			new Point( (int)(0x112 * Scale.X), (int)(0x102 * Scale.Y) ),
			new Point( (int)(0x121 * Scale.X), (int)(0x124 * Scale.Y) ),
			new Point( (int)(0x138 * Scale.X), (int)(0x146 * Scale.Y) ),
			new Point( (int)(0x157 * Scale.X), (int)(360 * Scale.Y) ),
			new Point( (int)(0x18a * Scale.X), (int)(0x18a * Scale.Y) ),
			new Point( (int)(0x1f2 * Scale.X), (int)(0x1ac * Scale.Y) ),
			new Point( (int)(640 * Scale.X), (int)(0x1ce * Scale.Y) )
		};
		private STバー情報[] stバー情報 = new STバー情報[13];
		private CTexture txSongNotFound, txEnumeratingSongs;
		private CTexture txスキル数字;
		private CTexture tx使用レーン数数字;
		private CTexture txアイテム数数字;
		private STバー tx曲名バー;
		private ST選曲バー tx選曲バー;

		private int nCurrentPosition = 0;
		private int nNumOfItems = 0;

		//private string strBoxDefSkinPath = "";
		private Eバー種別 e曲のバー種別を返す(C曲リストノード song)
		{
			if (song != null)
			{
				switch (song.eノード種別)
				{
					case C曲リストノード.Eノード種別.SCORE:
					case C曲リストノード.Eノード種別.SCORE_MIDI:
						return Eバー種別.Score;

					case C曲リストノード.Eノード種別.BOX:
					case C曲リストノード.Eノード種別.BACKBOX:
						return Eバー種別.Box;
				}
			}
			return Eバー種別.Other;
		}
		private C曲リストノード r次の曲(C曲リストノード song)
		{
			if (song == null)
				return null;

			List<C曲リストノード> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : CDTXMania.Instance.Songs管理.list曲ルート;

			int index = list.IndexOf(song);

			if (index < 0)
				return null;

			if (index == (list.Count - 1))
				return list[0];

			return list[index + 1];
		}
		private C曲リストノード r前の曲(C曲リストノード song)
		{
			if (song == null)
				return null;

			List<C曲リストノード> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : CDTXMania.Instance.Songs管理.list曲ルート;

			int index = list.IndexOf(song);

			if (index < 0)
				return null;

			if (index == 0)
				return list[list.Count - 1];

			return list[index - 1];
		}
		private void tスキル値の描画(int x, int y, int nスキル値)
		{
			if (nスキル値 <= 0 || nスキル値 > 100)    // スキル値 0 ＝ 未プレイ なので表示しない。
				return;

			int color = (nスキル値 == 100) ? 3 : (nスキル値 / 25);

			int n百の位 = nスキル値 / 100;
			int n十の位 = (nスキル値 % 100) / 10;
			int n一の位 = (nスキル値 % 100) % 10;


			// 百の位の描画。

			if (n百の位 > 0)
				this.tスキル値の描画_１桁描画(x, y, n百の位, color);


			// 十の位の描画。

			if (n百の位 != 0 || n十の位 != 0)
				this.tスキル値の描画_１桁描画(x + (int)(8 * Scale.X), y, n十の位, color);


			// 一の位の描画。

			this.tスキル値の描画_１桁描画(x + (int)(16 * Scale.X), y, n一の位, color);
		}
		private void tスキル値の描画_１桁描画(int x, int y, int n数値, int color)
		{
			int dx = (n数値 % 5) * (int)(9 * Scale.X);
			int dy = (n数値 / 5) * (int)(12 * Scale.Y);

			switch (color)
			{
				case 0:
					if (this.txスキル数字 != null)
						this.txスキル数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle((int)(45 * Scale.X) + dx, (int)(24 * Scale.Y) + dy, (int)(9 * Scale.X), (int)(12 * Scale.Y)));
					break;

				case 1:
					if (this.txスキル数字 != null)
						this.txスキル数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle((int)(45 * Scale.X) + dx, dy, (int)(9 * Scale.X), (int)(12 * Scale.Y)));
					break;

				case 2:
					if (this.txスキル数字 != null)
						this.txスキル数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(dx, (int)(24 * Scale.Y) + dy, (int)(9 * Scale.X), (int)(12 * Scale.Y)));
					break;

				case 3:
					if (this.txスキル数字 != null)
						this.txスキル数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(dx, dy, (int)(9 * Scale.X), (int)(12 * Scale.Y)));
					break;
			}
		}
		private void tバーの初期化()
		{
			C曲リストノード song = this.r現在選択中の曲;

			if (song == null)
				return;

			for (int i = 0; i < 5; i++)
				song = this.r前の曲(song);

			if ( song == null )
				return;

			for (int i = 0; i < 13; i++)
			{
				this.stバー情報[i].strタイトル文字列 = song.strタイトル;
				this.stバー情報[i].col文字色 = song.col文字色;
				this.stバー情報[i].eバー種別 = this.e曲のバー種別を返す(song);

				for (EPart j = EPart.Drums; j <= EPart.Bass; j++)
				{
					this.stバー情報[i].nスキル値[j] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[j];
					this.stバー情報[i].e使用レーン数[j] = song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.使用レーン数[j];
				}

				song = this.r次の曲(song);
			}

			this.n現在の選択行 = 5;
		}
		private void tバーの描画(int x, int y, Eバー種別 type, bool b選択曲)
		{
			if (x >= SampleFramework.GameWindowSize.Width || y >= SampleFramework.GameWindowSize.Height)
				return;

			if (b選択曲)
			{
				#region [ (A) 選択曲の場合 ]
				//-----------------
				if (this.tx選曲バー[(int)type] != null)
					this.tx選曲バー[(int)type].t2D描画(
						CDTXMania.Instance.Device,
						x,
						y,
						new Rectangle(
							0,
							0,
							256,    //(int)(128 * Scale.X),
							128     //(int)(64 * Scale.Y)
						)
					);  // ヘサキ
				x += this.tx選曲バー[(int)type].sz画像サイズ.Width;

				var rc = new Rectangle(
					128,        //(int)(64 * Scale.X),
					0,
					128,        //(int)(64 * Scale.X),
					128         //(int)(64 * Scale.Y)
				);
				while (x < SampleFramework.GameWindowSize.Width)
				{
					if (this.tx選曲バー[(int)type] != null)
						this.tx選曲バー[(int)type].t2D描画(
							CDTXMania.Instance.Device,
							x,
							y,
							rc
						);  // 胴体；64pxずつ横につなげていく。
					x += 128;   //(int)(64 * Scale.Y);
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) その他の場合 ]
				//-----------------
				if (this.tx曲名バー[(int)type] != null)
					this.tx曲名バー[(int)type].t2D描画(
						CDTXMania.Instance.Device,
						x,
						y,
						new Rectangle(
							0,
							0,
							128,  //(int) ( 64 * Scale.X ),
							69    //(int) ( 32 * Scale.Y )
										//(int)(64 * Scale.X),
										//(int)(32 * Scale.Y)
						)
					);    // ヘサキ
				x += 64;    //(int) ( 64 * Scale.X );

				var rc = new Rectangle(
					64,
					0,
					64,
					69
				//0,
				//(int)(32 * Scale.Y),
				//(int)(64 * Scale.X),
				//(int)(32 * Scale.Y)

				//128,		//0,
				//0,		//(int)(32 * Scale.Y),
				//128,		//(int)(64 * Scale.X),
				//128			//(int)(32 * Scale.Y)
				);
				while (x < SampleFramework.GameWindowSize.Width)
				{
					if (this.tx曲名バー[(int)type] != null)
						this.tx曲名バー[(int)type].t2D描画(
							CDTXMania.Instance.Device,
							x,
							y,
							rc
						);  // 胴体；64pxずつ横につなげていく。
					x += 64;  // (int) ( 64 * Scale.X );
				}
				//-----------------
				#endregion
			}
		}
		private void t曲名バーの生成(int nバー番号, string str曲名, Color color)
		{
			if (nバー番号 < 0 || nバー番号 > 12)
				return;

			try
			{
				SizeF sz曲名;

				#region [ 曲名表示に必要となるサイズを取得する。]
				//-----------------
				using (var bmpDummy = new Bitmap(1, 1))
				{
					var g = Graphics.FromImage(bmpDummy);
					g.PageUnit = GraphicsUnit.Pixel;
					sz曲名 = g.MeasureString(str曲名, this.ft曲リスト用フォント);
				}
				//-----------------
				#endregion

				int n最大幅px = (int)(392 * Scale.Y);
				int height = (int)(25 * Scale.Y);
				int width = (int)((sz曲名.Width + 2) * 0.5f);
				if (width > (CDTXMania.Instance.Device.Capabilities.MaxTextureWidth / 2))
					width = CDTXMania.Instance.Device.Capabilities.MaxTextureWidth / 2; // 右端断ち切れ仕方ないよね

				float f拡大率X = (width <= n最大幅px) ? 0.5f : (((float)n最大幅px / (float)width) * 0.5f); // 長い文字列は横方向に圧縮。

				using (var bmp = new Bitmap(width * 2, height * 2, PixelFormat.Format32bppArgb))    // 2倍（面積4倍）のBitmapを確保。（0.5倍で表示する前提。）
				using (var g = Graphics.FromImage(bmp))
				{
					g.TextRenderingHint = TextRenderingHint.AntiAlias;
					float y = (((float)bmp.Height) / 2f) - ((CDTXMania.Instance.ConfigIni.nFontSizeDotSongSelect * Scale.Y * 2f) / 2f);
					g.DrawString(str曲名, this.ft曲リスト用フォント, new SolidBrush(this.color文字影), (float)2f, (float)(y + 2f));
					g.DrawString(str曲名, this.ft曲リスト用フォント, new SolidBrush(color), 0f, y);

					TextureFactory.t安全にDisposeする(ref this.stバー情報[nバー番号].txタイトル名);

					this.stバー情報[nバー番号].txタイトル名 = new CTexture(CDTXMania.Instance.Device, bmp, CDTXMania.Instance.TextureFormat);
					this.stバー情報[nバー番号].txタイトル名.vc拡大縮小倍率 = new Vector3(f拡大率X, 0.5f, 1f);
				}
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("曲名テクスチャの作成に失敗しました。[{0}]", str曲名);
				this.stバー情報[nバー番号].txタイトル名 = null;
			}
		}
		private void tアイテム数の描画()
		{
			string s = nCurrentPosition.ToString() + "/" + nNumOfItems.ToString();
			int x = (int)(SampleFramework.GameWindowSize.Width - (8 + 12) * Scale.X);
			int y = (int)(362 * Scale.Y);

			for (int p = s.Length - 1; p >= 0; p--)
			{
				tアイテム数の描画_１桁描画(x, y, s[p]);
				x -= (int)(8 * Scale.X);
			}
		}
		private void tアイテム数の描画_１桁描画(int x, int y, char s数値)
		{
			int dx, dy;
			if (s数値 == '/')
			{
				dx = (int)(48 * Scale.X);
				dy = 0;
			}
			else
			{
				int n = (int)s数値 - (int)'0';
				dx = (n % 6) * (int)(8 * Scale.X);
				dy = (n / 6) * (int)(12 * Scale.Y);
			}
			if (this.txアイテム数数字 != null)
			{
				this.txアイテム数数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(dx, dy, (int)(8 * Scale.X), (int)(10 * Scale.Y)));
			}
		}
		
		// #36177 使用レーン数の表示 add ikanick 16.03.20
		private void t使用レーン数の描画(int x, int y, EUseLanes e使用レーン数)
		{
			int n使用レーン数 = 0;
			switch (e使用レーン数)
			{
				case EUseLanes.Dr_6		: n使用レーン数 = 6; break;
				case EUseLanes.Dr_10	: n使用レーン数 = 10; break;
				case EUseLanes.Dr_12	: n使用レーン数 = 12; break;
			}

			if (n使用レーン数 == 0) return;

			int n十の位 = n使用レーン数 / 10;
			int n一の位 = n使用レーン数 % 10;

			// 十の位の描画。
			if (n十の位 != 0)
				this.t使用レーン数の描画_１桁描画(x + (int)(8 * Scale.X), y, n十の位);

			// 一の位の描画。
			this.t使用レーン数の描画_１桁描画(x + (int)(13 * Scale.X), y, n一の位);
		}
		private void t使用レーン数の描画_１桁描画(int x, int y, int n数値)
		{
			int dx, dy;
			dx = (n数値 % 6) * (int)(8 * Scale.X);
			dy = (n数値 / 6) * (int)(11 * Scale.Y);

			if (this.tx使用レーン数数字 != null)
			{
				this.tx使用レーン数数字.t2D描画(CDTXMania.Instance.Device, x, y, new Rectangle(dx, dy, (int)(8 * Scale.X), (int)(10 * Scale.Y)));
			}
		}
		//-----------------
		#endregion
	}
}
