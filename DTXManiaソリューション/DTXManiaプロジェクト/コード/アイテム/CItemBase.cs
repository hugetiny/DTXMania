using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	/// <summary>
	/// すべてのアイテムの基本クラス。
	/// </summary>
	internal class CItemBase
	{
		// プロパティ

		public Eパネル種別 eパネル種別;
		public enum Eパネル種別
		{
			通常,
			その他
		}

		public E種別 e種別;
		public enum E種別
		{
			基本形,
			ONorOFFトグル,
			ONorOFFor不定スリーステート,
			整数,
			リスト
		}

		public string str項目名;
		public string str説明文;


		// コンストラクタ

		public CItemBase()
		{
			this.str項目名 = "";
			this.str説明文 = "";
		}
		public CItemBase( string str項目名 )
			: this()
		{
			this.t初期化( str項目名 );
		}
		public CItemBase( string str項目名, Eパネル種別 eパネル種別 )
			: this()
		{
			this.t初期化( str項目名, eパネル種別 );
		}

		
		// メソッド；子クラスで実装する

		public virtual void tEnter押下()
		{
		}
		public virtual void t項目値を次へ移動()
		{
		}
		public virtual void t項目値を前へ移動()
		{
		}
		public virtual void t初期化( string str項目名 )
		{
			this.t初期化( str項目名, Eパネル種別.通常 );
		}
		public virtual void t初期化( string str項目名, Eパネル種別 eパネル種別 )
		{
			this.str項目名 = str項目名;
			this.eパネル種別 = eパネル種別;
		}
	}
}
