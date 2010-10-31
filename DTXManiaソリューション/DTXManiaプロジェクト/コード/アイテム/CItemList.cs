using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	/// <summary>
	/// 「リスト」（複数の固定値からの１つを選択可能）を表すアイテム。
	/// </summary>
	internal class CItemList : CItemBase
	{
		// プロパティ

		public List<string> list項目値;
		public int n現在選択されている項目番号;


		// コンストラクタ

		public CItemList()
		{
			base.e種別 = CItemBase.E種別.リスト;
			this.n現在選択されている項目番号 = 0;
			this.list項目値 = new List<string>();
		}
		public CItemList( string str項目名 )
			: this()
		{
			this.t初期化( str項目名 );
		}
		public CItemList( string str項目名, CItemBase.Eパネル種別 eパネル種別 )
			: this()
		{
			this.t初期化( str項目名, eパネル種別 );
		}
		public CItemList( string str項目名, CItemBase.Eパネル種別 eパネル種別, int n初期インデックス値, params string[] arg項目リスト )
			: this()
		{
			this.t初期化( str項目名, eパネル種別, n初期インデックス値, arg項目リスト );
		}


		// CItemBase 実装

		public override void tEnter押下()
		{
			this.t項目値を次へ移動();
		}
		public override void t項目値を次へ移動()
		{
			if( ++this.n現在選択されている項目番号 >= this.list項目値.Count )
			{
				this.n現在選択されている項目番号 = 0;
			}
		}
		public override void t項目値を前へ移動()
		{
			if( --this.n現在選択されている項目番号 < 0 )
			{
				this.n現在選択されている項目番号 = this.list項目値.Count - 1;
			}
		}
		public override void t初期化( string str項目名, CItemBase.Eパネル種別 eパネル種別 )
		{
			base.t初期化( str項目名, eパネル種別 );
			this.n現在選択されている項目番号 = 0;
			this.list項目値.Clear();
		}
		public void t初期化( string str項目名, CItemBase.Eパネル種別 eパネル種別, int n初期インデックス値, params string[] arg項目リスト )
		{
			base.t初期化( str項目名, eパネル種別 );
			this.n現在選択されている項目番号 = n初期インデックス値;
			foreach( string str in arg項目リスト )
			{
				this.list項目値.Add( str );
			}
		}
	}
}
