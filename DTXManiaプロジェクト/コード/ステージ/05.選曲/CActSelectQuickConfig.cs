using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DTXMania
{
	internal class CActSelectQuickConfig : CActSelectPopupMenu
	{
		public bool bGotoDetailConfig
		{
			get;
			private set;
		}
		// コンストラクタ

		public CActSelectQuickConfig()
		{
/*
•Target: Drums/Guitar/Bass 
•Auto Mode: All ON/All OFF/CUSTOM 
•Auto Lane: 
•Scroll Speed: 
•Play Speed: 
•Risky: 
•Hidden/Sudden: None/Hidden/Sudden/Both 
•Conf SET: SET-1/SET-2/SET-3 
•More... 
•EXIT 
*/
			List<CItemBase> lciD = new List<CItemBase>();
			lciD.Add( new CItemList( "Target",		CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "Drums", "Guitar", "Bass" } ) );
			lciD.Add( new CItemList( "Auto Mode",		CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "ON",	"OFF", "Custom" } ) );
			lciD.Add( new CItemList( "Auto Lane",	CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "" } ) );

			lciD.Add( new CItemInteger( "ScrollSpeed", 0, 1999, CDTXMania.ConfigIni.n譜面スクロール速度.Drums,
				"演奏時のドラム譜面のスクロールの\n" +
				"速度を指定します。\n" +
				"x0.5 ～ x1000.0 を指定可能です。",
				"To change the scroll speed for the\n" +
				"drums lanes.\n" +
				"You can set it from x0.5 to x1000.0.\n" +
				"(ScrollSpeed=x0.5 means half speed)" ) );

			lciD.Add( new CItemInteger( "PlaySpeed", 5, 40, CDTXMania.ConfigIni.n演奏速度,
				"曲の演奏速度を、速くしたり遅くした\nりすることができます。\n（※一部のサウンドカードでは正しく\n 再生できない可能性があります。）",
				"It changes the song speed.\nFor example, you can play in half\n speed by setting PlaySpeed = 0.500\n for your practice.\nNote: It also changes the songs' pitch." ) );

			lciD.Add( new CItemInteger( "Risky", 0, 10, CDTXMania.ConfigIni.nRisky,
				"Riskyモードの設定:\n" +
				"1以上の値にすると、その回数分の\n" +
				"Poor/MissでFAILEDとなります。\n" +
				"0にすると無効になり、\n" +
				"DamageLevelに従ったゲージ増減と\n" +
				"なります。\n" +
				"StageFailedの設定と併用できます。",
				"Risky mode:\n" +
				"Set over 1, in case you'd like to specify\n" +
				" the number of Poor/Miss times to be\n" +
				" FAILED.\n" +	
				"Set 0 to disable Risky mode." ) );
			
			//lciD.Add( new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Drums,
			//    "ドラムチップが譜面の下の方で表示\n" +
			//    "されなくなります。",
			//    "Drums chips are hidden by approaching\n" +
			//    "to the hit bar. " ) );
			//lciD.Add( new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Drums,
			//    "ドラムチップが譜面の下の方から表\n" +
			//    "示されるようになります。",
			//    "Drums chips are disappered until they\n" +
			//    "come near the hit bar, and suddenly\n" +
			//    "appears." ) );

			int nSuddenHidden = ( ( CDTXMania.ConfigIni.bHidden.Drums ) ? 2 : 0 ) + ( ( CDTXMania.ConfigIni.bSudden.Drums ) ? 1 : 0 );
			lciD.Add( new CItemList( "Sud/Hid", CItemBase.Eパネル種別.通常, nSuddenHidden, "", "", new string[] { "None", "Sudden", "Hidden", "Sud+Hid" } ) );
			lciD.Add( new CItemList( "Config SET", CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "SET-1", "SET-2", "SET-3" } ) );

			lciD.Add( new CItemList( "More...", CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "" } ) );

			lciD.Add( new CItemList( "Return",		CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "", "" } ) );
			
			base.Initialize( "Quick Config", lciD, true, 2 );
		}


		// メソッド
		//public new void tActivatePopupMenu()
		//{
		//	base.tActivatePopupMenu();
		//}
		//public void tDeativatePopupMenu()
		//{
		//	base.tDeativatePopupMenu();
		//}


		public override void tEnter押下Main( E楽器パート eInst, int pos, int nSortOrder )
		{
            switch ( pos )
            {
				case (int) ESortItem.ScrollSpeed:
					CDTXMania.ConfigIni.n譜面スクロール速度.Drums = (int) GetObj現在値( (int) ESortItem.ScrollSpeed );
					break;
				case (int) ESortItem.PlaySpeed:
					CDTXMania.ConfigIni.n演奏速度 = (int) GetObj現在値( (int) ESortItem.PlaySpeed );
					break;
				case (int) ESortItem.Risky:
					CDTXMania.ConfigIni.nRisky = (int) GetObj現在値( (int) ESortItem.Risky );
					break;
				case (int) ESortItem.SuddenHidden:
					int sh = (int) GetIndex( (int) ESortItem.SuddenHidden );
					CDTXMania.ConfigIni.bSudden.Drums = ( (sh & 2) > 0 ) ? true : false;
					CDTXMania.ConfigIni.bHidden.Drums = ( (sh & 1) > 0 ) ? true : false;
					break;
				case (int) ESortItem.More:
					this.bGotoDetailConfig = true;
					this.tDeativatePopupMenu();
					break;

				case (int) ESortItem.Return:
                    this.tDeativatePopupMenu();
                    break;
                default:
                    break;
            }
		}
		
		// CActivity 実装

		public override void On活性化()
		{
			base.On活性化();
			this.bGotoDetailConfig = false;
		}
		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			base.OnManagedリソースの解放();
		}

		#region [ private ]
		//-----------------

		//private CActSelect曲リスト act曲リスト;

		private enum ESortItem : int
		{
			Target = 0,
			AutoMode,
			AutoLane,
			ScrollSpeed,
			PlaySpeed,
			Risky,
			SuddenHidden,
			ConfSet,
			More,
			Return, END,
			Default = 99
		};
		//-----------------
		#endregion
	}


}
