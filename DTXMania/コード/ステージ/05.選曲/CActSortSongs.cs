using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DTXMania
{
	internal class CActSortSongs : CActSelectPopupMenu
	{
		public CActSortSongs()
		{
			List<COptionBase> lci = new List<COptionBase>();

			string[] items = new string[] { "Z,Y,X,...", "A,B,C,..." };
			COptionStringList title = new COptionStringList(items[0]);
			title.Initialize( "strSortByTitle", items);
			title.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(CDTXMania.Instance.Songs管理.t曲リストのソート2_タイトル順, eInst,
						title.Index == 0 ? -1 : 1);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( title );

			items = new string[] { "99,98,97,...", "1,2,3,..." };
			COptionStringList level = new COptionStringList(items[0]);
			level.Initialize( "strSortByLevel", items);
			level.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
								CDTXMania.Instance.Songs管理.t曲リストのソート4_LEVEL順, eInst,
								level.Index == 0 ? -1 : 1,
								this.act曲リスト.n現在のアンカ難易度レベル);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( level );

			items = new string[] { "E,D,C,...", "SS,S,A,..." };
			COptionStringList bestrank = new COptionStringList(items[0]);
			bestrank.Initialize( "strSortByBestRank", items);
			bestrank.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
								CDTXMania.Instance.Songs管理.t曲リストのソート5_BestRank順, eInst,
								bestrank.Index == 0 ? -1 : 1,
								this.act曲リスト.n現在のアンカ難易度レベル
						);
			};
			lci.Add( bestrank );

			items = new string[] { "10,9,8,...", "1,2,3,..." };
			COptionStringList playcount = new COptionStringList(items[0]);
			playcount.Initialize( "strSortByPlayCount", items);
			playcount.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
						CDTXMania.Instance.Songs管理.t曲リストのソート3_演奏回数の多い順, eInst,
						playcount.Index == 0 ? -1 : 1,
						this.act曲リスト.n現在のアンカ難易度レベル
				);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( playcount );

			items = new string[] { "Z,Y,X,...", "A,B,C,..." };
			COptionStringList author = new COptionStringList(items[0]);
			author.Initialize( "strSortByAuthor", items);
			author.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
								CDTXMania.Instance.Songs管理.t曲リストのソート8_アーティスト名順, eInst,
								author.Index == 0 ? -1 : 1,
								this.act曲リスト.n現在のアンカ難易度レベル
						);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( author );

			items = new string[] { "100,99,98,...", "1,2,3,..." };
			COptionStringList skillpoint = new COptionStringList(items[0]);
			skillpoint.Initialize( "strSortBySkillPoint", items);
			skillpoint.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
								CDTXMania.Instance.Songs管理.t曲リストのソート6_SkillPoint順, eInst,
								skillpoint.Index == 0 ? -1 : 1,
								this.act曲リスト.n現在のアンカ難易度レベル
						);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( skillpoint );
			
			items = new string[] { "Dec.31,30,...", "Jan.1,2,..." };
			COptionStringList date = new COptionStringList(items[0]);
			date.Initialize( "strSortByDate", items);
			date.OnEnterDelegate = () =>
			{
				this.act曲リスト.t曲リストのソート(
										CDTXMania.Instance.Songs管理.t曲リストのソート7_更新日時順, eInst,
										date.Index == 0 ? -1 : 1,
										this.act曲リスト.n現在のアンカ難易度レベル
								);
				this.act曲リスト.t選択曲が変更された(true);
			};
			lci.Add( date );


			COptionString ret = new COptionString("Return");
			ret.Initialize( "strSortReturn");
			ret.OnEnterDelegate = () =>
			{
				this.tDeativatePopupMenu();
			};
			lci.Add( ret );

			base.Initialize(lci, false, CDTXMania.Instance.Resources.Label("strSortMenu"));
		}


		// メソッド
		public void tActivatePopupMenu(EPart einst, ref CActSelect曲リスト ca)
		{
			this.act曲リスト = ca;
			base.tActivatePopupMenu(einst);
		}

		// CActivity 実装

		public override void On活性化()
		{
			base.On活性化();
		}
		public override void On非活性化()
		{
			if (!base.b活性化してない)
			{
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			base.OnManagedリソースの解放();
		}


		CActSelect曲リスト act曲リスト;
	}


}
