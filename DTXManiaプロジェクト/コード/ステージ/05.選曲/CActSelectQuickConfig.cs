using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DTXMania
{
	internal class CActSelectQuickConfig : CActSelectPopupMenu
	{
		private readonly string QuickCfgTitle = "Quick Config";

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
			for ( int nConfSet = 0; nConfSet < 3; nConfSet++ )
			{
				lci.Add( new List<List<CItemBase>>() );									// ConfSet用の3つ分の枠。
				for ( int nInst = 0; nInst < 3; nInst++ )
				{
					lci[ nConfSet ].Add( null );										// Drum/Guitar/Bassで3つ分、枠を作っておく
					lci[ nConfSet ][ nInst ] = MakeListCItemBase( nConfSet, nInst );
				}
			}
			base.Initialize( lci[ nCurrentConfigSet ][ 0 ], true, QuickCfgTitle, 2 );	// ConfSet=0, nInst=Drums
		}

		private List<CItemBase> MakeListCItemBase( int nConfigSet, int nInst )
		{
			List<CItemBase> l = new List<CItemBase>();

			#region [ 共通 Target/AutoMode/AutoLane ]
			l.Add( new CSwitchItemList( "Target", CItemBase.Eパネル種別.通常, nInst, "", "", new string[] { "Drums", "Guitar", "Bass" } ) );

			int automode = 4;
			if ( CDTXMania.ConfigIni.bドラムが全部オートプレイである )
			{
				automode = 0;
			}
			else if (	CDTXMania.ConfigIni.bAutoPlay.LC == false && CDTXMania.ConfigIni.bAutoPlay.HH == true  &&
						CDTXMania.ConfigIni.bAutoPlay.BD == false && CDTXMania.ConfigIni.bAutoPlay.SD == false &&
						CDTXMania.ConfigIni.bAutoPlay.HT == false && CDTXMania.ConfigIni.bAutoPlay.FT == false &&
						CDTXMania.ConfigIni.bAutoPlay.FT == false && CDTXMania.ConfigIni.bAutoPlay.CY == false )
			{
				automode = 1;
			}
			else if (	CDTXMania.ConfigIni.bAutoPlay.LC == false && CDTXMania.ConfigIni.bAutoPlay.HH == false &&
						CDTXMania.ConfigIni.bAutoPlay.BD == true  && CDTXMania.ConfigIni.bAutoPlay.SD == false &&
						CDTXMania.ConfigIni.bAutoPlay.HT == false && CDTXMania.ConfigIni.bAutoPlay.FT == false &&
						CDTXMania.ConfigIni.bAutoPlay.FT == false && CDTXMania.ConfigIni.bAutoPlay.CY == false )
			{
				automode = 2;
			}
			else
			{
				automode = 3;
			}
			l.Add( new CItemList( "Auto Mode", CItemBase.Eパネル種別.通常, automode, "", "", new string[] { "All Auto", "Auto HH", "Auto BD", "Custom", "OFF" } ) );
			l.Add( new CItemList( "Auto Lanes", CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "" } ) );
			#endregion
			#region [ 個別 ScrollSpeed ]
			l.Add( new CItemInteger( "ScrollSpeed", 0, 1999, CDTXMania.ConfigIni.n譜面スクロール速度[ nInst ],
				"演奏時のドラム譜面のスクロールの\n" +
				"速度を指定します。\n" +
				"x0.5 ～ x1000.0 を指定可能です。",
				"To change the scroll speed for the\n" +
				"drums lanes.\n" +
				"You can set it from x0.5 to x1000.0.\n" +
				"(ScrollSpeed=x0.5 means half speed)" ) );
			#endregion
			#region [ 共通 PlaySpeed/Risky ]
			l.Add( new CItemInteger( "PlaySpeed", 5, 40, CDTXMania.ConfigIni.n演奏速度,
				"曲の演奏速度を、速くしたり遅くした\nりすることができます。\n（※一部のサウンドカードでは正しく\n 再生できない可能性があります。）",
				"It changes the song speed.\nFor example, you can play in half\n speed by setting PlaySpeed = 0.500\n for your practice.\nNote: It also changes the songs' pitch." ) );
			l.Add( new CItemInteger( "Risky", 0, 10, CDTXMania.ConfigIni.nRisky,
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
			#endregion
			#region [ 個別 Sud/Hid ]
			int nSuddenHidden = ( ( CDTXMania.ConfigIni.bHidden[ nInst ] ) ? 2 : 0 ) + ( ( CDTXMania.ConfigIni.bSudden[ nInst ] ) ? 1 : 0 );
			l.Add( new CItemList( "Sud/Hid", CItemBase.Eパネル種別.通常, nSuddenHidden, "", "", new string[] { "None", "Sudden", "Hidden", "Sud+Hid" } ) );
			#endregion
			#region [ 共通 SET切り替え/More/Return ]
			l.Add( new CSwitchItemList( "Config SET", CItemBase.Eパネル種別.通常, nCurrentConfigSet, "", "", new string[] { "SET-1", "SET-2", "SET-3" } ) );
			l.Add( new CSwitchItemList( "More...", CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "" } ) );
			l.Add( new CSwitchItemList( "Return", CItemBase.Eパネル種別.通常, 0, "", "", new string[] { "", "" } ) );
			#endregion

			return l;
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
				case (int) ESortItem.Target:
					nCurrentTarget = ( nCurrentTarget + 1 ) % 3;
					Initialize( lci[ nCurrentConfigSet ][ nCurrentTarget ], true, QuickCfgTitle, pos );
					break;

				case (int) ESortItem.ScrollSpeed:
					CDTXMania.ConfigIni.n譜面スクロール速度[ nCurrentTarget ] = (int) GetObj現在値( (int) ESortItem.ScrollSpeed );
					break;
				case (int) ESortItem.PlaySpeed:
					CDTXMania.ConfigIni.n演奏速度 = (int) GetObj現在値( (int) ESortItem.PlaySpeed );
					break;
				case (int) ESortItem.Risky:
					CDTXMania.ConfigIni.nRisky = (int) GetObj現在値( (int) ESortItem.Risky );
					break;
				case (int) ESortItem.SuddenHidden:
					int sh = (int) GetIndex( (int) ESortItem.SuddenHidden );
					CDTXMania.ConfigIni.bSudden[ nCurrentTarget ] = ( (sh & 2) > 0 ) ? true : false;
					CDTXMania.ConfigIni.bHidden[ nCurrentTarget ] = ( (sh & 1) > 0 ) ? true : false;
					break;
				case (int) ESortItem.ConfSet:		// CONF-SET切り替え
					nCurrentConfigSet = (int) GetIndex( (int) ESortItem.ConfSet );
					//Initialize( lci[ nCurrentConfigSet ], true, QuickCfgTitle, pos );
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
		private int nCurrentTarget = 0;
		private int nCurrentConfigSet = 0;
		List<List<List<CItemBase>>> lci = new List<List<List<CItemBase>>>();	// DrGtBs, ConfSet, 選択肢一覧。都合、3次のListとなる。
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
