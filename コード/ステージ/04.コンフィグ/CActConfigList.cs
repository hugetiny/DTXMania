using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActConfigList : CActivity
	{
		// プロパティ

		public bool b現在選択されている項目はReturnToMenuである
		{
			get
			{
				CItemBase base2 = this.list項目リスト[ this.n現在の選択項目 ];
				if( ( ( base2 != this.iSystemReturnToMenu ) && ( base2 != this.iKeyAssignDrumsReturnToMenu ) ) && ( base2 != this.iKeyAssignGuitarReturnToMenu ) )
				{
					return ( base2 == this.iKeyAssignBassReturnToMenu );
				}
				return true;
			}
		}
		public CItemBase ib現在の選択項目
		{
			get
			{
				return this.list項目リスト[ this.n現在の選択項目 ];
			}
		}
		public int n現在の選択項目;


		// メソッド

		public void tEnter押下()
		{
			CDTXMania.Skin.sound決定音.t再生する();
			if( this.b要素値にフォーカス中 )
			{
				this.b要素値にフォーカス中 = false;
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ].e種別 == CItemBase.E種別.整数 )
			{
				this.b要素値にフォーカス中 = true;
			}
			else if( this.b現在選択されている項目はReturnToMenuである )
			{
				this.tConfigIniへ記録する();
			}
			else if( ( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemGuitar ) || ( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemDrums ) )
			{
				this.list項目リスト[ this.n現在の選択項目 ].tEnter押下();
				if( !this.iSystemGuitar.bON && !this.iSystemDrums.bON )
				{
					if( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemGuitar )
					{
						this.iSystemDrums.bON = true;
					}
					else
					{
						this.iSystemGuitar.bON = true;
					}
				}
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsLC )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.LC );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHHC )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.HH );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHHO )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.HHO );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsSD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.SD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsBD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.BD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.HT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsLT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.LT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsFT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.FT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsCY )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.CY );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsRD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.DRUMS, Eパッド.RD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarR )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.HH );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarG )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.SD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarB )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.BD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarPick )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.HT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarWail )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.LT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarDecide )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.CY );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarCancel )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.GUITAR, Eパッド.FT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassR )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.HH );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassG )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.SD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassB )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.BD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassPick )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.HT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassWail )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.LT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassDecide )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.CY );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassCancel )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( E楽器パート.BASS, Eパッド.FT );
			}
			else
			{
				this.list項目リスト[ this.n現在の選択項目 ].tEnter押下();
				if( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemFullscreen )
				{
					CDTXMania.app.b次のタイミングで全画面・ウィンドウ切り替えを行う = true;
				}
				else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemVSyncWait )
				{
					CDTXMania.ConfigIni.b垂直帰線待ちを行う = this.iSystemVSyncWait.bON;
					CDTXMania.app.b次のタイミングで垂直帰線同期切り替えを行う = true;
				}
			}
		}
		public void t項目リストの設定・Exit()
		{
			this.tConfigIniへ記録する();
			this.eメニュー種別 = Eメニュー種別.Unknown;
		}
		public void t項目リストの設定・KeyAssignBass()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iKeyAssignBassReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu.");
			this.list項目リスト.Add( this.iKeyAssignBassReturnToMenu );
			this.iKeyAssignBassR = new CItemBase( "R",
				"ベースのキー設定：\nRボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for R button.");
			this.list項目リスト.Add( this.iKeyAssignBassR );
			this.iKeyAssignBassG = new CItemBase( "G",
				"ベースのキー設定：\nGボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for G button.");
			this.list項目リスト.Add( this.iKeyAssignBassG );
			this.iKeyAssignBassB = new CItemBase( "B",
				"ベースのキー設定：\nBボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for B button.");
			this.list項目リスト.Add(this.iKeyAssignBassB);
			this.iKeyAssignBassPick = new CItemBase( "Pick",
				"ベースのキー設定：\nピックボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Pick button.");
			this.list項目リスト.Add(this.iKeyAssignBassPick);
			this.iKeyAssignBassWail = new CItemBase( "Wailing",
				"ベースのキー設定：\nWailingボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Wailing button.");
			this.list項目リスト.Add(this.iKeyAssignBassWail);
			this.iKeyAssignBassDecide = new CItemBase( "Decide",
				"ベースのキー設定：\n決定ボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Decide button.");
			this.list項目リスト.Add(this.iKeyAssignBassDecide);
			this.iKeyAssignBassCancel = new CItemBase( "Cancel",
				"ベースのキー設定：\nキャンセルボタンへのキーの割り当\nてを設定します。",
				"Bass key assign:\nTo assign key/pads for Cancel button.");
			this.list項目リスト.Add(this.iKeyAssignBassCancel);
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignBass;
		}
		public void t項目リストの設定・KeyAssignDrums()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iKeyAssignDrumsReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu.");
			this.list項目リスト.Add(this.iKeyAssignDrumsReturnToMenu);
			this.iKeyAssignDrumsLC = new CItemBase( "LeftCymbal",
				"ドラムのキー設定：\n左シンバルへのキーの割り当てを設\n定します。",
				"Drums key assign:\nTo assign key/pads for LeftCymbal\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsLC);
			this.iKeyAssignDrumsHHC = new CItemBase( "HiHat(Close)",
				"ドラムのキー設定：\nハイハット（クローズ）へのキーの割り\n当てを設定します。",
				"Drums key assign:\nTo assign key/pads for HiHat(Close)\n button.");
			this.list項目リスト.Add( this.iKeyAssignDrumsHHC );
			this.iKeyAssignDrumsHHO = new CItemBase( "HiHat(Open)",
				"ドラムのキー設定：\nハイハット（オープン）へのキーの割り\n当てを設定します。",
				"Drums key assign:\nTo assign key/pads for HiHat(Open)\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsHHO);
			this.iKeyAssignDrumsSD = new CItemBase( "Snare",
				"ドラムのキー設定：\nスネアへのキーの割り当てを設定し\nます。",
				"Drums key assign:\nTo assign key/pads for Snare button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsSD);
			this.iKeyAssignDrumsBD = new CItemBase( "Bass",
				"ドラムのキー設定：\nバスドラムへのキーの割り当てを設定\nします。",
				"Drums key assign:\nTo assign key/pads for Bass button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsBD);
			this.iKeyAssignDrumsHT = new CItemBase( "HighTom",
				"ドラムのキー設定：\nハイタムへのキーの割り当てを設定\nします。",
				"Drums key assign:\nTo assign key/pads for HighTom\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsHT);
			this.iKeyAssignDrumsLT = new CItemBase( "LowTom",
				"ドラムのキー設定：\nロータムへのキーの割り当てを設定\nします。",
				"Drums key assign:\nTo assign key/pads for LowTom button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsLT);
			this.iKeyAssignDrumsFT = new CItemBase( "FloorTom",
				"ドラムのキー設定：\nフロアタムへのキーの割り当てを設\n定します。",
				"Drums key assign:\nTo assign key/pads for FloorTom\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsFT);
			this.iKeyAssignDrumsCY = new CItemBase( "RightCymbal",
				"ドラムのキー設定：\n右シンバルへのキーの割り当てを設\n定します。",
				"Drums key assign:\nTo assign key/pads for RightCymbal\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsCY);
			this.iKeyAssignDrumsRD = new CItemBase( "RideCymbal",
				"ドラムのキー設定：\nライドシンバルへのキーの割り当て\nを設定します。",
				"Drums key assign:\nTo assign key/pads for RideCymbal\n button.");
			this.list項目リスト.Add(this.iKeyAssignDrumsRD);
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignDrums;
		}
		public void t項目リストの設定・KeyAssignGuitar()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iKeyAssignGuitarReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu.");
			this.list項目リスト.Add(this.iKeyAssignGuitarReturnToMenu);
			this.iKeyAssignGuitarR = new CItemBase( "R",
				"ギターのキー設定：\nRボタンへのキーの割り当てを設定し\nます。",
				"Guitar key assign:\nTo assign key/pads for R button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarR);
			this.iKeyAssignGuitarG = new CItemBase( "G",
				"ギターのキー設定：\nGボタンへのキーの割り当てを設定し\nます。",
				"Guitar key assign:\nTo assign key/pads for G button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarG);
			this.iKeyAssignGuitarB = new CItemBase( "B",
				"ギターのキー設定：\nBボタンへのキーの割り当てを設定し\nます。",
				"Guitar key assign:\nTo assign key/pads for B button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarB);
			this.iKeyAssignGuitarPick = new CItemBase( "Pick",
				"ギターのキー設定：\nピックボタンへのキーの割り当てを設\n定します。",
				"Guitar key assign:\nTo assign key/pads for Pick button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarPick);
			this.iKeyAssignGuitarWail = new CItemBase( "Wailing",
				"ギターのキー設定：\nWailingボタンへのキーの割り当てを\n設定します。",
				"Guitar key assign:\nTo assign key/pads for Wailing button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarWail);
			this.iKeyAssignGuitarDecide = new CItemBase( "Decide",
				"ギターのキー設定：\n決定ボタンへのキーの割り当てを設\n定します。",
				"Guitar key assign:\nTo assign key/pads for Decide button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarDecide);
			this.iKeyAssignGuitarCancel = new CItemBase( "Cancel",
				"ギターのキー設定：\nキャンセルボタンへのキーの割り当\nてを設定します。",
				"Guitar key assign:\nTo assign key/pads for Cancel button.");
			this.list項目リスト.Add(this.iKeyAssignGuitarCancel);
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignGuitar;
		}
		public void t項目リストの設定・System()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iSystemReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu.");
			this.list項目リスト.Add( this.iSystemReturnToMenu );
			this.iSystemGuitar = new CItemToggle( "Guitar", CDTXMania.ConfigIni.bGuitar有効,
				"ギター有効：\nこれを OFF にすると、ギターとベース\nに関するすべてが無効になります。\nつまりは DTXMania がドラム専用の\nゲームとなります。",
				"To show guitar/bass lanes.\nIf Guitar=ON and Drums=OFF,\n the playing screen becomes\n guitar-only mode.");
			this.list項目リスト.Add( this.iSystemGuitar );
			this.iSystemDrums = new CItemToggle( "Drums", CDTXMania.ConfigIni.bDrums有効,
				"ドラム有効：\nこれを OFF にすると、ドラムに関する\nすべてが無効になります。\nつまりは DTXMania がギター専用の\nゲームとなります。",
				"To use drums lanes.\nIf Guitar=ON and Drums=OFF,\n the playing screen becomes\n guitar-only mode.");
			this.list項目リスト.Add( this.iSystemDrums );
			this.iSystemFullscreen = new CItemToggle( "Fullscreen", CDTXMania.ConfigIni.b全画面モード,
				"画面モード設定：\nON で全画面モード、OFF でウィンド\nウモードになります。",
				"Fullscreen mode or window mode.");
			this.list項目リスト.Add( this.iSystemFullscreen );
			this.iSystemStageFailed = new CItemToggle( "StageFailed", CDTXMania.ConfigIni.bSTAGEFAILED有効,
				"STAGE FAILED 有効：\nON にすると、ゲージがなくなった時\nに STAGE FAILED となり演奏が中断\nされます。OFF の場合は、ゲージが\nなくなっても最後まで演奏できます。",
				"Turn OFF if you don't want to encount\n GAME OVER.");
			this.list項目リスト.Add( this.iSystemStageFailed );
			this.iSystemRandomFromSubBox = new CItemToggle( "RandSubBox", CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする,
				"子BOXをRANDOMの対象とする：\nON にすると、RANDOM SELECT 時\nに子BOXも選択対象とします。",
				"Turn ON to use child BOX (subfolders)\n at RANDOM SELECT.");
			this.list項目リスト.Add( this.iSystemRandomFromSubBox );
			this.iSystemHHGroup = new CItemList( "HH Group", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHHGroup,
				"ハイハットレーン打ち分け設定：\n左シンバル、ハイハットオープン、ハ\nイハットクローズの打ち分け方法を指\n定します。\n  HH-0 ... LC | HHC | HHO\n  HH-1 ... LC & ( HHC | HHO )\n  HH-2 ... LC | ( HHC & HHO )\n  HH-3 ... LC & HHC & HHO\n",
				"HH-0: LC|HC|HO; all are separated.\nHH-1: LC&(HC|HO);\n HC and HO are separted.\n LC is grouped with HC and HHO.\nHH-2: LC|(HC&HO);\n LC and HHs are separated.\n HC and HO are grouped.\nHH-3: LC&HC&HO; all are grouped.",
				new string[] { "HH-0", "HH-1", "HH-2", "HH-3" } );
			this.list項目リスト.Add( this.iSystemHHGroup );
			this.iSystemFTGroup = new CItemList( "FT Group", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eFTGroup,
				"フロアタム打ち分け設定：\nロータムとフロアタムの打ち分け方法\nを指定します。\n  FT-0 ... LT | FT\n  FT-1 ... LT & FT\n",
				"FT-0: LT|FT\n LT and FT are separated.\nFT-1: LT&FT\n LT and FT are grouped.",
				new string[] { "FT-0", "FT-1" } );
			this.list項目リスト.Add( this.iSystemFTGroup );
			this.iSystemCYGroup = new CItemList( "CY Group", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eCYGroup,
				"シンバルレーン打ち分け設定：\n右シンバルとライドシンバルの打ち分\nけ方法を指定します。\n  CY-0 ... CY | RD\n  CY-1 ... CY & RD\n",
				"CY-0: CY|RD\n CY and RD are separated.\nCY-1: CY&RD\n CY and RD are grouped.",
				new string[] { "CY-0", "CY-1" } );
			this.list項目リスト.Add( this.iSystemCYGroup );
			this.iSystemHitSoundPriorityHH = new CItemList( "HH Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityHH,
				"発声音決定の優先順位：\nハイハットレーン打ち分け有効時に、\nチップの発声音をどのように決定する\nかを指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using HH-0,1 and 2.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" } );
			this.list項目リスト.Add( this.iSystemHitSoundPriorityHH );
			this.iSystemHitSoundPriorityFT = new CItemList( "FT Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityFT,
				"発声音決定の優先順位：\nフロアタム打ち分け有効時に、チップ\nの発声音をどのように決定するかを\n指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using FT-0.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" });
			this.list項目リスト.Add( this.iSystemHitSoundPriorityFT );
			this.iSystemHitSoundPriorityCY = new CItemList( "CY Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityCY,
				"発声音決定の優先順位\nシンバルレーン打ち分け有効時に、\nチップの発声音をどのように決定する\nかを指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using CY-0.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" });
			this.list項目リスト.Add( this.iSystemHitSoundPriorityCY );
			this.iSystemAdjustWaves = new CItemToggle( "AdjustWaves", CDTXMania.ConfigIni.bWave再生位置自動調整機能有効,
				"サウンド再生位置自動補正：\nハードウェアやＯＳに起因するサウン\nドのずれを強制的に補正します。\nBGM のように再生時間の長い音声\nデータが使用されている曲で効果が\nあります。",
				"Automatic wave playing position\n adjustment feature. If you turn it ON,\n it decrease the lag which comes from\n the difference of hardware/OS.\nUsually, you should turn it ON.");
			this.list項目リスト.Add( this.iSystemAdjustWaves );
			this.iSystemVSyncWait = new CItemToggle( "VSyncWait", CDTXMania.ConfigIni.b垂直帰線待ちを行う,
				"垂直帰線同期：\n画面の描画をディスプレイの垂直帰\n線中に行なう場合には ON を指定し\nます。ON にすると、ガタつきのない\n滑らかな画面描画が実現されます。",
				"Turn ON to wait VSync (Vertical\n Synchronizing signal) at every\n drawings. (so FPS becomes 60)\nIf you have enough CPU/GPU power,\n the scroll would become smooth.");
			this.list項目リスト.Add( this.iSystemVSyncWait );
			this.iSystemAVI = new CItemToggle( "AVI", CDTXMania.ConfigIni.bAVI有効,
				"AVIの使用：\n動画(AVI)を再生可能にする場合に\nON にします。AVI の再生には、それ\nなりのマシンパワーが必要とされます。",
				"To use AVI playback or not.");
			this.list項目リスト.Add( this.iSystemAVI );
			this.iSystemBGA = new CItemToggle( "BGA", CDTXMania.ConfigIni.bBGA有効,
				"BGAの使用：\n画像(BGA)を表示可能にする場合に\nON にします。BGA の再生には、それ\nなりのマシンパワーが必要とされます。",
				"To draw BGA (back ground animations)\n or not.");
			this.list項目リスト.Add( this.iSystemBGA );
			this.iSystemFillIn = new CItemToggle( "FillIn", CDTXMania.ConfigIni.bフィルイン有効,
				"フィルインエフェクトの使用：\nフィルイン区間の爆発パターンに特別\nのエフェクトを使用します。\nフィルインエフェクトの描画にはそれな\nりのマシンパワーが必要とされます。",
				"To show bursting effects at the fill-in\n zone or not.");
			this.list項目リスト.Add( this.iSystemFillIn );
			this.iSystemSoundMonitorDrums = new CItemToggle( "DrumsMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Drums,
				"ドラム音モニタ：\nドラム音を他の音より大きめの音量で\n発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
				"To enhance the drums chip sound\n(except autoplay).");
			this.list項目リスト.Add( this.iSystemSoundMonitorDrums );
			this.iSystemSoundMonitorGuitar = new CItemToggle( "GuitarMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Guitar,
				"ギター音モニタ：\nギター音を他の音より大きめの音量\nで発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
				"To enhance the guitar chip sound\n(except autoplay).");
			this.list項目リスト.Add(this.iSystemSoundMonitorGuitar);
			this.iSystemSoundMonitorBass = new CItemToggle( "BassMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Bass,
				"ベース音モニタ：\nベース音を他の音より大きめの音量\nで発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
				"To enhance the bass chip sound\n(except autoplay).");
			this.list項目リスト.Add( this.iSystemSoundMonitorBass );
			this.iSystemPreviewSoundWait = new CItemInteger( "PreSoundWait", 0, 0x2710, CDTXMania.ConfigIni.n曲が選択されてからプレビュー音が鳴るまでのウェイトms,
				"プレビュー音演奏までの時間：\n曲にカーソルが合わされてからプレ\nビュー音が鳴り始めるまでの時間を\n指定します。\n0 ～ 10000 [ms] が指定可能です。",
				"Delay time(ms) to start playing preview\n sound in SELECT MUSIC screen.\nYou can specify from 0ms to 10000ms.");
			this.list項目リスト.Add( this.iSystemPreviewSoundWait );
			this.iSystemPreviewImageWait = new CItemInteger( "PreImageWait", 0, 0x2710, CDTXMania.ConfigIni.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms,
				"プレビュー画像表示までの時間：\n曲にカーソルが合わされてからプレ\nビュー画像が表示されるまでの時間\nを指定します。\n0 ～ 10000 [ms] が指定可能です。",
				"Delay time(ms) to show preview image\n in SELECT MUSIC screen.\nYou can specify from 0ms to 10000ms.");
			this.list項目リスト.Add( this.iSystemPreviewImageWait );
			this.iSystemDebugInfo = new CItemToggle( "Debug Info", CDTXMania.ConfigIni.b演奏情報を表示する,
				"演奏情報の表示：\n演奏中、BGA領域の下部に演奏情報\n（FPS、BPM、演奏時間など）を表示し\nます。\nまた、小節線の横に小節番号が表示\nされるようになります。",
				"To show song informations on playing\n BGA area. (FPS, BPM, total time etc)\nYou can ON/OFF the indications\n by pushing [Del] while playing drums,\n guitar or bass.");
			this.list項目リスト.Add( this.iSystemDebugInfo );
			this.iSystemBGAlpha = new CItemInteger( "BG Alpha", 0, 0xff, CDTXMania.ConfigIni.n背景の透過度,
				"背景画像の半透明割合：\n背景画像をDTXManiaのフレーム画像\nと合成する際の、背景画像の透明度\nを指定します。\n0 が完全透明で、255 が完全不透明\nとなります。",
				"The degree for transparing playing\n screen and wallpaper.\n\n0=completely transparent,\n255=no transparency");
			this.list項目リスト.Add( this.iSystemBGAlpha );
			this.iSystemBGMSound = new CItemToggle( "BGM Sound", CDTXMania.ConfigIni.bBGM音を発声する,
				"BGMの再生：\nこれをOFFにすると、BGM を再生しな\nくなります。",
				"Turn OFF if you don't want to play\n BGM.");
			this.list項目リスト.Add( this.iSystemBGMSound );
			this.iSystemHitSound = new CItemToggle( "HitSound", CDTXMania.ConfigIni.bドラム打音を発声する,
				"打撃音の再生：\nこれをOFFにすると、パッドを叩いた\nときの音を再生しなくなります（ドラム\nのみ）。\nDTX の音色で演奏したい場合などに\nOFF にします。",
				"Turn OFF if you don't want to play\n hitting chip sound.\nIt is useful to play with real/electric\n drums kit.");
			this.list項目リスト.Add( this.iSystemHitSound );
			this.iSystemAudienceSound = new CItemToggle( "Audience", CDTXMania.ConfigIni.b歓声を発声する,
				"歓声の再生：\nこれをOFFにすると、歓声を再生しな\nくなります。",
				"Turn ON if you want to be cheered\n at the end of fill-in zone or not.");
			this.list項目リスト.Add( this.iSystemAudienceSound );
			this.iSystemDamageLevel = new CItemList( "DamageLevel", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eダメージレベル,
				"ゲージ減少割合：\nMiss ヒット時のゲージの減少度合い\nを指定します。\n",
				"Damage level at missing (and\n recovering level) at playing.",
				new string[] { "Small", "Normal", "Large" } );
			this.list項目リスト.Add( this.iSystemDamageLevel );
			this.iSystemSaveScore = new CItemToggle( "SaveScore", CDTXMania.ConfigIni.bScoreIniを出力する,
				"演奏記録の保存：\nON で演奏記録を ～.score.ini ファイ\nルに保存します。\n",
				"To save high-scores/skills, turn it ON.\nTurn OFF in case your song data are\n in read-only media (CD-ROM etc).\nNote that the score files also contain\n 'BGM Adjust' parameter. So if you\n want to keep adjusting parameter,\n you need to set SaveScore=ON.");
			this.list項目リスト.Add( this.iSystemSaveScore );
			this.iSystemMinComboDrums = new CItemInteger( "D-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Drums,
				"表示可能な最小コンボ数（ドラム）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the drums.\nYou can specify from 1 to 99999.");
			this.list項目リスト.Add( this.iSystemMinComboDrums );
			this.iSystemMinComboGuitar = new CItemInteger( "G-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Guitar,
				"表示可能な最小コンボ数（ギター）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the guitar.\nYou can specify from 1 to 99999.");
			this.list項目リスト.Add(this.iSystemMinComboGuitar);
			this.iSystemMinComboBass = new CItemInteger( "B-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Bass,
				"表示可能な最小コンボ数（ベース）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the bass.\nYou can specify from 1 to 99999.");
			this.list項目リスト.Add(this.iSystemMinComboBass);
			this.iSystemChipVolume = new CItemInteger( "ChipVolume", 0, 100, CDTXMania.ConfigIni.n手動再生音量,
				"打音の音量：\n入力に反応して再生されるチップの音\n量を指定します。\n0 ～ 100 % の値が指定可能です。\n",
				"The volumes for chips you hit.\nYou can specify from 0 to 100%.");
			this.list項目リスト.Add( this.iSystemChipVolume );
			this.iSystemAutoChipVolume = new CItemInteger( "AutoVolume", 0, 100, CDTXMania.ConfigIni.n自動再生音量,
				"自動再生音の音量：\n自動的に再生されるチップの音量を指\n定します。\n0 ～ 100 % の値が指定可能です。\n",
				"The volumes for AUTO chips.\nYou can specify from 0 to 100%.");
			this.list項目リスト.Add( this.iSystemAutoChipVolume );
			this.iSystemStoicMode = new CItemToggle( "StoicMode", CDTXMania.ConfigIni.bストイックモード,
				"ストイック（禁欲）モード：\n以下をまとめて表示ON/OFFします。\n・プレビュー画像/動画\n・リザルト画像/動画\n・NowLoading画像\n・演奏画面の背景画像\n・BGA 画像\n・AVI 動画\n",
				"Turn ON to disable drawing\n * preview image / movie\n * result image / movie\n * nowloading image\n * wallpaper (in playing screen)\n * BGA / AVI (in playing screen)");
			this.list項目リスト.Add( this.iSystemStoicMode );
			this.iSystemCymbalFree = new CItemToggle( "CymbalFree", CDTXMania.ConfigIni.bシンバルフリー,
				"シンバルフリーモード：\n左シンバル・右シンバルの区別をなく\nします。ライドシンバルまで区別をな\nくすか否かは、CYGroup に従います。\n",
				"Turn ON to group LC (left cymbal) and\n CY (right cymbal).\nWhether RD (ride cymbal) is also\n grouped or not depends on the\n'CY Group' setting.");
			this.list項目リスト.Add( this.iSystemCymbalFree );
			this.iSystemBufferedInput = new CItemToggle( "BufferedInput", CDTXMania.ConfigIni.bバッファ入力を行う,
				"バッファ入力モード：\nON にすると、FPS を超える入力解像\n度を実現します。\nOFF にすると、入力解像度は FPS に\n等しくなります。",
				"To select joystick input method.\n\nON=to use buffer input. No lost/lags.\nOFF to use realtime input. It may\n causes lost/lags for input.\n Moreover, input frequency is\n synchronized with FPS.");
			this.list項目リスト.Add( this.iSystemBufferedInput );
			this.iLogOutputLog = new CItemToggle( "TraceLog", CDTXMania.ConfigIni.bログ出力,
				"Traceログ出力：\nDTXManiaLog.txt にログを出力します。\n変更した場合は、DTXMania の再起動\n後に有効となります。",
				"Turn ON to put debug log to\n DTXManiaLog.txt\nTo take it effective, you need to\n re-open DTXMania.");
			this.list項目リスト.Add( this.iLogOutputLog );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.System;
		}
		public void t次に移動()
		{
			CDTXMania.Skin.soundカーソル移動音.t再生する();
			if( this.b要素値にフォーカス中 )
			{
				this.list項目リスト[ this.n現在の選択項目 ].t項目値を前へ移動();
			}
			else
			{
				this.n目標のスクロールカウンタ += 100;
			}
		}
		public void t前に移動()
		{
			CDTXMania.Skin.soundカーソル移動音.t再生する();
			if( this.b要素値にフォーカス中 )
			{
				this.list項目リスト[ this.n現在の選択項目 ].t項目値を次へ移動();
			}
			else
			{
				this.n目標のスクロールカウンタ -= 100;
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.list項目リスト = new List<CItemBase>();
			this.eメニュー種別 = Eメニュー種別.Unknown;
			this.t項目リストの設定・System();
			this.b要素値にフォーカス中 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロール用タイマ値 = -1;
			this.ct三角矢印アニメ = new CCounter();
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.ct三角矢印アニメ = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx通常項目行パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig itembox.png" ), false );
				this.txその他項目行パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig itembox other.png" ), false );
				this.tx三角矢印 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig triangle arrow.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			CDTXMania.tテクスチャの解放( ref this.tx通常項目行パネル );
			CDTXMania.tテクスチャの解放( ref this.txその他項目行パネル );
			CDTXMania.tテクスチャの解放( ref this.tx三角矢印 );
			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(bool)のほうを使用してください。" );
		}
		public int t進行描画( bool b項目リスト側にフォーカスがある )
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.nスクロール用タイマ値 = CDTXMania.Timer.n現在時刻;
					this.ct三角矢印アニメ.t開始( 0, 9, 50, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				this.b項目リスト側にフォーカスがある = b項目リスト側にフォーカスがある;
				long num = CDTXMania.Timer.n現在時刻;
				if( num < this.nスクロール用タイマ値 )
				{
					this.nスクロール用タイマ値 = num;
				}
				while( ( num - this.nスクロール用タイマ値 ) >= 2 )
				{
					int num2 = Math.Abs( (int) ( this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ ) );
					int num3 = 0;
					if( num2 <= 100 )
					{
						num3 = 2;
					}
					else if( num2 <= 300 )
					{
						num3 = 3;
					}
					else if( num2 <= 500 )
					{
						num3 = 4;
					}
					else
					{
						num3 = 8;
					}
					if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
					{
						this.n現在のスクロールカウンタ += num3;
						if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					else if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
					{
						this.n現在のスクロールカウンタ -= num3;
						if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					if( this.n現在のスクロールカウンタ >= 100 )
					{
						this.n現在の選択項目 = this.t次の項目( this.n現在の選択項目 );
						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;
						if( this.n目標のスクロールカウンタ == 0 )
						{
							CDTXMania.stageコンフィグ.t項目変更通知();
						}
					}
					else if( this.n現在のスクロールカウンタ <= -100 )
					{
						this.n現在の選択項目 = this.t前の項目( this.n現在の選択項目 );
						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;
						if( this.n目標のスクロールカウンタ == 0 )
						{
							CDTXMania.stageコンフィグ.t項目変更通知();
						}
					}
					this.nスクロール用タイマ値 += 2;
				}
				if( this.b項目リスト側にフォーカスがある && ( this.n目標のスクロールカウンタ == 0 ) )
				{
					this.ct三角矢印アニメ.t進行Loop();
				}
				this.ptパネルの基本座標[ 4 ].X = this.b項目リスト側にフォーカスがある ? 0x114 : 0x12d;
				int nItem = this.n現在の選択項目;
				for( int i = 0; i < 4; i++ )
				{
					nItem = this.t前の項目( nItem );
				}
				for( int j = -4; j < 6; j++ )
				{
					if( ( ( j == -4 ) && ( this.n現在のスクロールカウンタ > 0 ) ) || ( ( j == 5 ) && ( this.n現在のスクロールカウンタ < 0 ) ) )
					{
						nItem = this.t次の項目( nItem );
						continue;
					}
					int index = j + 4;
					int num8 = ( this.n現在のスクロールカウンタ <= 0 ) ? ( ( ( j + 4 ) + 1 ) % 10 ) : ( ( ( ( j + 4 ) - 1 ) + 10 ) % 10 );
					int x = this.ptパネルの基本座標[ index ].X + ( (int) ( ( this.ptパネルの基本座標[ num8 ].X - this.ptパネルの基本座標[ index ].X ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
					int y = this.ptパネルの基本座標[ index ].Y + ( (int) ( ( this.ptパネルの基本座標[ num8 ].Y - this.ptパネルの基本座標[ index ].Y ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
					switch( this.list項目リスト[ nItem ].eパネル種別 )
					{
						case CItemBase.Eパネル種別.通常:
							if( this.tx通常項目行パネル != null )
							{
								this.tx通常項目行パネル.t2D描画( CDTXMania.app.Device, x, y );
							}
							break;

						case CItemBase.Eパネル種別.その他:
							if( this.txその他項目行パネル != null )
							{
								this.txその他項目行パネル.t2D描画( CDTXMania.app.Device, x, y );
							}
							break;
					}
					CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 0x12, y + 12, this.list項目リスト[ nItem ].str項目名 );
					switch( this.list項目リスト[ nItem ].e種別 )
					{
						case CItemBase.E種別.ONorOFFトグル:
							CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, ( (CItemToggle) this.list項目リスト[ nItem ] ).bON ? "ON" : "OFF" );
							break;

						case CItemBase.E種別.ONorOFFor不定スリーステート:
							switch( ( (CItemThreeState) this.list項目リスト[ nItem ] ).e現在の状態 )
							{
								case CItemThreeState.E状態.ON:
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, "ON" );
									break;

								case CItemThreeState.E状態.不定:
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, "- -" );
									break;

								default:
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, "OFF" );
									break;
							}
							break;

						case CItemBase.E種別.整数:
							CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値.ToString(), ( j == 0 ) && this.b要素値にフォーカス中 );
							break;

						case CItemBase.E種別.リスト:
							{
								CItemList list = (CItemList) this.list項目リスト[ nItem ];
								CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, list.list項目値[ list.n現在選択されている項目番号 ] );
								break;
							}
					}


					nItem = this.t次の項目( nItem );
				}
				if( this.b項目リスト側にフォーカスがある && ( this.n目標のスクロールカウンタ == 0 ) )
				{
					int num11;
					int num12;
					int num13;
					if( !this.b要素値にフォーカス中 )
					{
						num11 = 0x114;
						num12 = 0xba - this.ct三角矢印アニメ.n現在の値;
						num13 = 0xfe + this.ct三角矢印アニメ.n現在の値;
					}
					else
					{
						num11 = 0x210;
						num12 = 0xc6 - this.ct三角矢印アニメ.n現在の値;
						num13 = 0xf2 + this.ct三角矢印アニメ.n現在の値;
					}
					if( this.tx三角矢印 != null )
					{
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, num11, num12, new Rectangle( 0, 0, 0x20, 0x10 ) );
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, num11, num13, new Rectangle( 0, 0x10, 0x20, 0x10 ) );
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private enum Eメニュー種別
		{
			System,
			KeyAssignDrums,
			KeyAssignGuitar,
			KeyAssignBass,
			Unknown
		}

		private bool b項目リスト側にフォーカスがある;
		private bool b要素値にフォーカス中;
		private CCounter ct三角矢印アニメ;
		private Eメニュー種別 eメニュー種別;
		private CItemBase iKeyAssignBassB;
		private CItemBase iKeyAssignBassCancel;
		private CItemBase iKeyAssignBassDecide;
		private CItemBase iKeyAssignBassG;
		private CItemBase iKeyAssignBassPick;
		private CItemBase iKeyAssignBassR;
		private CItemBase iKeyAssignBassReturnToMenu;
		private CItemBase iKeyAssignBassWail;
		private CItemBase iKeyAssignDrumsBD;
		private CItemBase iKeyAssignDrumsCY;
		private CItemBase iKeyAssignDrumsFT;
		private CItemBase iKeyAssignDrumsHHC;
		private CItemBase iKeyAssignDrumsHHO;
		private CItemBase iKeyAssignDrumsHT;
		private CItemBase iKeyAssignDrumsLC;
		private CItemBase iKeyAssignDrumsLT;
		private CItemBase iKeyAssignDrumsRD;
		private CItemBase iKeyAssignDrumsReturnToMenu;
		private CItemBase iKeyAssignDrumsSD;
		private CItemBase iKeyAssignGuitarB;
		private CItemBase iKeyAssignGuitarCancel;
		private CItemBase iKeyAssignGuitarDecide;
		private CItemBase iKeyAssignGuitarG;
		private CItemBase iKeyAssignGuitarPick;
		private CItemBase iKeyAssignGuitarR;
		private CItemBase iKeyAssignGuitarReturnToMenu;
		private CItemBase iKeyAssignGuitarWail;
		private CItemToggle iLogOutputLog;
		private CItemToggle iSystemAdjustWaves;
		private CItemToggle iSystemAudienceSound;
		private CItemInteger iSystemAutoChipVolume;
		private CItemToggle iSystemAVI;
		private CItemToggle iSystemBGA;
		private CItemInteger iSystemBGAlpha;
		private CItemToggle iSystemBGMSound;
		private CItemInteger iSystemChipVolume;
		private CItemList iSystemCYGroup;
		private CItemToggle iSystemCymbalFree;
		private CItemList iSystemDamageLevel;
		private CItemToggle iSystemDebugInfo;
		private CItemToggle iSystemDrums;
		private CItemToggle iSystemFillIn;
		private CItemList iSystemFTGroup;
		private CItemToggle iSystemFullscreen;
		private CItemToggle iSystemGuitar;
		private CItemList iSystemHHGroup;
		private CItemToggle iSystemHitSound;
		private CItemList iSystemHitSoundPriorityCY;
		private CItemList iSystemHitSoundPriorityFT;
		private CItemList iSystemHitSoundPriorityHH;
		private CItemInteger iSystemMinComboBass;
		private CItemInteger iSystemMinComboDrums;
		private CItemInteger iSystemMinComboGuitar;
		private CItemInteger iSystemPreviewImageWait;
		private CItemInteger iSystemPreviewSoundWait;
		private CItemToggle iSystemRandomFromSubBox;
		private CItemBase iSystemReturnToMenu;
		private CItemToggle iSystemSaveScore;
		private CItemToggle iSystemSoundMonitorBass;
		private CItemToggle iSystemSoundMonitorDrums;
		private CItemToggle iSystemSoundMonitorGuitar;
		private CItemToggle iSystemStageFailed;
		private CItemToggle iSystemStoicMode;
		private CItemToggle iSystemVSyncWait;
		private CItemToggle iSystemBufferedInput;
		private List<CItemBase> list項目リスト;
		private long nスクロール用タイマ値;
		private int n現在のスクロールカウンタ;
		private int n目標のスクロールカウンタ;
		private Point[] ptパネルの基本座標 = new Point[] { new Point( 0x12d, 3 ), new Point( 0x12d, 0x35 ), new Point( 0x12d, 0x67 ), new Point( 0x12d, 0x99 ), new Point( 0x114, 0xcb ), new Point( 0x12d, 0xfd ), new Point( 0x12d, 0x12f ), new Point( 0x12d, 0x161 ), new Point( 0x12d, 0x193 ), new Point( 0x12d, 0x1c5 ) };
		private CTexture txその他項目行パネル;
		private CTexture tx三角矢印;
		private CTexture tx通常項目行パネル;

		private int t前の項目( int nItem )
		{
			if( --nItem < 0 )
			{
				nItem = this.list項目リスト.Count - 1;
			}
			return nItem;
		}
		private int t次の項目( int nItem )
		{
			if( ++nItem >= this.list項目リスト.Count )
			{
				nItem = 0;
			}
			return nItem;
		}
		private void tConfigIniへ記録する()
		{
			switch( this.eメニュー種別 )
			{
				case Eメニュー種別.System:
					this.tConfigIniへ記録する・System();
					return;

				case Eメニュー種別.KeyAssignDrums:
					this.tConfigIniへ記録する・KeyAssignDrums();
					return;

				case Eメニュー種別.KeyAssignGuitar:
					this.tConfigIniへ記録する・KeyAssignGuitar();
					return;

				case Eメニュー種別.KeyAssignBass:
					this.tConfigIniへ記録する・KeyAssignBass();
					return;
			}
		}
		private void tConfigIniへ記録する・KeyAssignBass()
		{
		}
		private void tConfigIniへ記録する・KeyAssignDrums()
		{
		}
		private void tConfigIniへ記録する・KeyAssignGuitar()
		{
		}
		private void tConfigIniへ記録する・System()
		{
			CDTXMania.ConfigIni.bGuitar有効 = this.iSystemGuitar.bON;
			CDTXMania.ConfigIni.bDrums有効 = this.iSystemDrums.bON;
			CDTXMania.ConfigIni.b全画面モード = this.iSystemFullscreen.bON;
			CDTXMania.ConfigIni.bSTAGEFAILED有効 = this.iSystemStageFailed.bON;
			CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする = this.iSystemRandomFromSubBox.bON;
			CDTXMania.ConfigIni.eHHGroup = (EHHGroup) this.iSystemHHGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eFTGroup = (EFTGroup) this.iSystemFTGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eCYGroup = (ECYGroup) this.iSystemCYGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityHH = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityHH.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityFT = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityFT.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityCY = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityCY.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bWave再生位置自動調整機能有効 = this.iSystemAdjustWaves.bON;
			CDTXMania.ConfigIni.b垂直帰線待ちを行う = this.iSystemVSyncWait.bON;
			CDTXMania.ConfigIni.bバッファ入力を行う = this.iSystemBufferedInput.bON;
			CDTXMania.ConfigIni.bAVI有効 = this.iSystemAVI.bON;
			CDTXMania.ConfigIni.bBGA有効 = this.iSystemBGA.bON;
			CDTXMania.ConfigIni.bフィルイン有効 = this.iSystemFillIn.bON;
			CDTXMania.ConfigIni.b演奏音を強調する.Drums = this.iSystemSoundMonitorDrums.bON;
			CDTXMania.ConfigIni.b演奏音を強調する.Guitar = this.iSystemSoundMonitorGuitar.bON;
			CDTXMania.ConfigIni.b演奏音を強調する.Bass = this.iSystemSoundMonitorBass.bON;
			CDTXMania.ConfigIni.n曲が選択されてからプレビュー音が鳴るまでのウェイトms = this.iSystemPreviewSoundWait.n現在の値;
			CDTXMania.ConfigIni.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms = this.iSystemPreviewImageWait.n現在の値;
			CDTXMania.ConfigIni.b演奏情報を表示する = this.iSystemDebugInfo.bON;
			CDTXMania.ConfigIni.n背景の透過度 = this.iSystemBGAlpha.n現在の値;
			CDTXMania.ConfigIni.bBGM音を発声する = this.iSystemBGMSound.bON;
			CDTXMania.ConfigIni.bドラム打音を発声する = this.iSystemHitSound.bON;
			CDTXMania.ConfigIni.b歓声を発声する = this.iSystemAudienceSound.bON;
			CDTXMania.ConfigIni.eダメージレベル = (Eダメージレベル) this.iSystemDamageLevel.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bScoreIniを出力する = this.iSystemSaveScore.bON;
			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Drums = this.iSystemMinComboDrums.n現在の値;
			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Guitar = this.iSystemMinComboGuitar.n現在の値;
			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Bass = this.iSystemMinComboBass.n現在の値;
			CDTXMania.ConfigIni.bログ出力 = this.iLogOutputLog.bON;
			CDTXMania.ConfigIni.n手動再生音量 = this.iSystemChipVolume.n現在の値;
			CDTXMania.ConfigIni.n自動再生音量 = this.iSystemAutoChipVolume.n現在の値;
			CDTXMania.ConfigIni.bストイックモード = this.iSystemStoicMode.bON;
			CDTXMania.ConfigIni.bシンバルフリー = this.iSystemCymbalFree.bON;
		}
		//-----------------
		#endregion
	}
}
