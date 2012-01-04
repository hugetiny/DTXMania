using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActConfigList : CActivity
	{
		// プロパティ

		public bool bIsKeyAssignSelected		// #24525 2011.3.15 yyagi
		{
			get
			{
				Eメニュー種別 e = this.eメニュー種別;
				if ( e == Eメニュー種別.KeyAssignBass || e == Eメニュー種別.KeyAssignDrums ||
					e == Eメニュー種別.KeyAssignGuitar || e == Eメニュー種別.KeyAssignSystem )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public bool b現在選択されている項目はReturnToMenuである
		{
			get
			{
				CItemBase currentItem = this.list項目リスト[ this.n現在の選択項目 ];
				if ( currentItem == this.iSystemReturnToMenu || currentItem == this.iDrumsReturnToMenu ||
					currentItem == this.iGuitarReturnToMenu || currentItem == this.iBassReturnToMenu )
				{
					return true;
				}
				else
				{
					return false;
				}
//				CItemBase base2 = this.list項目リスト[ this.n現在の選択項目 ];
//				if( ( ( base2 != this.iSystemReturnToMenu ) && ( base2 != this.iKeyAssignDrumsReturnToMenu ) ) && ( base2 != this.iKeyAssignGuitarReturnToMenu ) )
//				{
//					return ( base2 == this.iKeyAssignBassReturnToMenu );
//				}
//				return true;
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
		public void t項目リストの設定・System()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iSystemReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iSystemReturnToMenu );

			this.iCommonDark = new CItemList( "Dark", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eDark,
				"HALF: 背景、レーン、ゲージが表示\nされなくなります。\nFULL: さらに小節線、拍線、判定ラ\nイン、パッドも表示されなくなります。",
				"OFF: all display parts are shown.\nHALF: wallpaper, lanes and gauge are\n disappeared.\nFULL: additionaly to HALF, bar/beat\n lines, hit bar, pads are disappeared.",
				new string[] { "OFF", "HALF", "FULL" } );
			this.list項目リスト.Add( this.iCommonDark );

			this.iSystemRisky = new CItemInteger( "Risky", 0, 10, CDTXMania.ConfigIni.nRisky,
				"Riskyモードの設定:\n1以上の値にすると、その回数分の\nPoor/MissでFAILEDとなります。\n0にすると無効になり、\nDamageLevelに従ったゲージ増減と\nなります。\nStageFailedの設定と併用できます。",
				"Risky mode:\nSet over 1, in case you'd like to specify\n the number of Poor/Miss times to be\n FAILED.\nSet 0 to disable Risky mode." );
			this.list項目リスト.Add( this.iSystemRisky );

			int nDGmode = (CDTXMania.ConfigIni.bGuitar有効 ? 2 : 0) + (CDTXMania.ConfigIni.bDrums有効 ? 1 : 0) - 1;
			this.iSystemGRmode = new CItemList("Drums & GR", CItemBase.Eパネル種別.通常, nDGmode,
				"使用楽器の選択：\nDrOnly: ドラムのみ有効にします。\nGROnly: ギター/ベースのみの\n専用画面を用います。\nBoth: ドラムとギター/ベースの\n両方を有効にします。\n",
				"DrOnly: Only Drums is available.\nGROnly: Only Guitar/Bass are available.\n You can play them in GR screen.\nBoth: Both Drums and Guitar/Bass\n are available.",
				new string[] { "DrOnly", "GROnly", "Both" });
			this.list項目リスト.Add(this.iSystemGRmode);


			this.iCommonPlaySpeed = new CItemInteger("PlaySpeed", 5, 40, CDTXMania.ConfigIni.n演奏速度,
				"曲の演奏速度を、速くしたり遅くした\nりすることができます。\n（※一部のサウンドカードでは正しく\n 再生できない可能性があります。）",
				"It changes the song speed.\nFor example, you can play in half\n speed by setting PlaySpeed = 0.500\n for your practice.\nNote: It also changes the songs' pitch." );
			this.list項目リスト.Add( this.iCommonPlaySpeed );


			this.iSystemFullscreen = new CItemToggle( "Fullscreen", CDTXMania.ConfigIni.b全画面モード,
				"画面モード設定：\nON で全画面モード、OFF でウィンド\nウモードになります。",
				"Fullscreen mode or window mode." );
			this.list項目リスト.Add( this.iSystemFullscreen );
			this.iSystemStageFailed = new CItemToggle( "StageFailed", CDTXMania.ConfigIni.bSTAGEFAILED有効,
				"STAGE FAILED 有効：\nON にすると、ゲージがなくなった時\nに STAGE FAILED となり演奏が中断\nされます。OFF の場合は、ゲージが\nなくなっても最後まで演奏できます。",
				"Turn OFF if you don't want to encount\n GAME OVER." );
			this.list項目リスト.Add( this.iSystemStageFailed );
			this.iSystemRandomFromSubBox = new CItemToggle( "RandSubBox", CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする,
				"子BOXをRANDOMの対象とする：\nON にすると、RANDOM SELECT 時\nに子BOXも選択対象とします。",
				"Turn ON to use child BOX (subfolders)\n at RANDOM SELECT." );
			this.list項目リスト.Add( this.iSystemRandomFromSubBox );


	
			this.iSystemAdjustWaves = new CItemToggle( "AdjustWaves", CDTXMania.ConfigIni.bWave再生位置自動調整機能有効,
				"サウンド再生位置自動補正：\nハードウェアやＯＳに起因するサウン\nドのずれを強制的に補正します。\nBGM のように再生時間の長い音声\nデータが使用されている曲で効果が\nあります。",
				"Automatic wave playing position\n adjustment feature. If you turn it ON,\n it decrease the lag which comes from\n the difference of hardware/OS.\nUsually, you should turn it ON." );
			this.list項目リスト.Add( this.iSystemAdjustWaves );
			this.iSystemVSyncWait = new CItemToggle( "VSyncWait", CDTXMania.ConfigIni.b垂直帰線待ちを行う,
				"垂直帰線同期：\n画面の描画をディスプレイの垂直帰\n線中に行なう場合には ON を指定し\nます。ON にすると、ガタつきのない\n滑らかな画面描画が実現されます。",
				"Turn ON to wait VSync (Vertical\n Synchronizing signal) at every\n drawings. (so FPS becomes 60)\nIf you have enough CPU/GPU power,\n the scroll would become smooth." );
			this.list項目リスト.Add( this.iSystemVSyncWait );
			this.iSystemAVI = new CItemToggle( "AVI", CDTXMania.ConfigIni.bAVI有効,
				"AVIの使用：\n動画(AVI)を再生可能にする場合に\nON にします。AVI の再生には、それ\nなりのマシンパワーが必要とされます。",
				"To use AVI playback or not." );
			this.list項目リスト.Add( this.iSystemAVI );
			this.iSystemBGA = new CItemToggle( "BGA", CDTXMania.ConfigIni.bBGA有効,
				"BGAの使用：\n画像(BGA)を表示可能にする場合に\nON にします。BGA の再生には、それ\nなりのマシンパワーが必要とされます。",
				"To draw BGA (back ground animations)\n or not." );
			this.list項目リスト.Add( this.iSystemBGA );
			this.iSystemPreviewSoundWait = new CItemInteger( "PreSoundWait", 0, 0x2710, CDTXMania.ConfigIni.n曲が選択されてからプレビュー音が鳴るまでのウェイトms,
				"プレビュー音演奏までの時間：\n曲にカーソルが合わされてからプレ\nビュー音が鳴り始めるまでの時間を\n指定します。\n0 ～ 10000 [ms] が指定可能です。",
				"Delay time(ms) to start playing preview\n sound in SELECT MUSIC screen.\nYou can specify from 0ms to 10000ms." );
			this.list項目リスト.Add( this.iSystemPreviewSoundWait );
			this.iSystemPreviewImageWait = new CItemInteger( "PreImageWait", 0, 0x2710, CDTXMania.ConfigIni.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms,
				"プレビュー画像表示までの時間：\n曲にカーソルが合わされてからプレ\nビュー画像が表示されるまでの時間\nを指定します。\n0 ～ 10000 [ms] が指定可能です。",
				"Delay time(ms) to show preview image\n in SELECT MUSIC screen.\nYou can specify from 0ms to 10000ms." );
			this.list項目リスト.Add( this.iSystemPreviewImageWait );
			this.iSystemDebugInfo = new CItemToggle( "Debug Info", CDTXMania.ConfigIni.b演奏情報を表示する,
				"演奏情報の表示：\n演奏中、BGA領域の下部に演奏情報\n（FPS、BPM、演奏時間など）を表示し\nます。\nまた、小節線の横に小節番号が表示\nされるようになります。",
				"To show song informations on playing\n BGA area. (FPS, BPM, total time etc)\nYou can ON/OFF the indications\n by pushing [Del] while playing drums,\n guitar or bass." );
			this.list項目リスト.Add( this.iSystemDebugInfo );
			this.iSystemBGAlpha = new CItemInteger( "BG Alpha", 0, 0xff, CDTXMania.ConfigIni.n背景の透過度,
				"背景画像の半透明割合：\n背景画像をDTXManiaのフレーム画像\nと合成する際の、背景画像の透明度\nを指定します。\n0 が完全透明で、255 が完全不透明\nとなります。",
				"The degree for transparing playing\n screen and wallpaper.\n\n0=completely transparent,\n255=no transparency" );
			this.list項目リスト.Add( this.iSystemBGAlpha );
			this.iSystemBGMSound = new CItemToggle( "BGM Sound", CDTXMania.ConfigIni.bBGM音を発声する,
				"BGMの再生：\nこれをOFFにすると、BGM を再生しな\nくなります。",
				"Turn OFF if you don't want to play\n BGM." );
			this.list項目リスト.Add( this.iSystemBGMSound );
			this.iSystemAudienceSound = new CItemToggle( "Audience", CDTXMania.ConfigIni.b歓声を発声する,
				"歓声の再生：\nこれをOFFにすると、歓声を再生しな\nくなります。",
				"Turn ON if you want to be cheered\n at the end of fill-in zone or not." );
			this.list項目リスト.Add( this.iSystemAudienceSound );
			this.iSystemDamageLevel = new CItemList( "DamageLevel", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eダメージレベル,
				"ゲージ減少割合：\nMiss ヒット時のゲージの減少度合い\nを指定します。\nRiskyが1以上の場合は無効となります",
				"Damage level at missing (and\n recovering level) at playing.\nThis setting is ignored when Risky >= 1.",
				new string[] { "Small", "Normal", "Large" } );
			this.list項目リスト.Add( this.iSystemDamageLevel );
			this.iSystemSaveScore = new CItemToggle( "SaveScore", CDTXMania.ConfigIni.bScoreIniを出力する,
				"演奏記録の保存：\nON で演奏記録を ～.score.ini ファイ\nルに保存します。\n",
				"To save high-scores/skills, turn it ON.\nTurn OFF in case your song data are\n in read-only media (CD-ROM etc).\nNote that the score files also contain\n 'BGM Adjust' parameter. So if you\n want to keep adjusting parameter,\n you need to set SaveScore=ON." );
			this.list項目リスト.Add( this.iSystemSaveScore );


			this.iSystemChipVolume = new CItemInteger( "ChipVolume", 0, 100, CDTXMania.ConfigIni.n手動再生音量,
				"打音の音量：\n入力に反応して再生されるチップの音\n量を指定します。\n0 ～ 100 % の値が指定可能です。\n",
				"The volumes for chips you hit.\nYou can specify from 0 to 100%." );
			this.list項目リスト.Add( this.iSystemChipVolume );
			this.iSystemAutoChipVolume = new CItemInteger( "AutoVolume", 0, 100, CDTXMania.ConfigIni.n自動再生音量,
				"自動再生音の音量：\n自動的に再生されるチップの音量を指\n定します。\n0 ～ 100 % の値が指定可能です。\n",
				"The volumes for AUTO chips.\nYou can specify from 0 to 100%." );
			this.list項目リスト.Add( this.iSystemAutoChipVolume );
			this.iSystemStoicMode = new CItemToggle( "StoicMode", CDTXMania.ConfigIni.bストイックモード,
				"ストイック（禁欲）モード：\n以下をまとめて表示ON/OFFします。\n・プレビュー画像/動画\n・リザルト画像/動画\n・NowLoading画像\n・演奏画面の背景画像\n・BGA 画像 / AVI 動画\n・グラフ画像\n",
				"Turn ON to disable drawing\n * preview image / movie\n * result image / movie\n * nowloading image\n * wallpaper (in playing screen)\n * BGA / AVI (in playing screen)" );
			this.list項目リスト.Add( this.iSystemStoicMode );
			this.iSystemShowLag = new CItemList( "ShowLagTime", CItemBase.Eパネル種別.通常, CDTXMania.ConfigIni.nShowLagType,
				"ズレ時間表示：\nジャストタイミングからのズレ時間(ms)\nを表示します。\n  OFF: ズレ時間を表示しません。\n  ON: ズレ時間を表示します。\n  GREAT-: PERFECT以外の時のみ\n表示します。",
				"About displaying the lag from\n the \"just timing\".\n  OFF: Don't show it.\n  ON: Show it.\n  GREAT-: Show it except you've\n  gotten PERFECT.",
				new string[] { "OFF", "ON", "GREAT-" } );
			this.list項目リスト.Add( this.iSystemShowLag );
			this.iSystemAutoResultCapture = new CItemToggle( "Autosaveresult", CDTXMania.ConfigIni.bIsAutoResultCapture,
				"リザルト画像自動保存機能：\nONにすると、ハイスコア/ハイスキル時に\n自動でリザルト画像を曲データと同じ\nフォルダに保存します。",
				"AutoSaveResult:\nTurn ON to save your result screen\n image automatically when you get\n hiscore/hiskill." );
			this.list項目リスト.Add( this.iSystemAutoResultCapture );

	
			this.iSystemBufferedInput = new CItemToggle( "BufferedInput", CDTXMania.ConfigIni.bバッファ入力を行う,
				"バッファ入力モード：\nON にすると、FPS を超える入力解像\n度を実現します。\nOFF にすると、入力解像度は FPS に\n等しくなります。",
				"To select joystick input method.\n\nON to use buffer input. No lost/lags.\nOFF to use realtime input. It may\n causes lost/lags for input.\n Moreover, input frequency is\n synchronized with FPS." );
			this.list項目リスト.Add( this.iSystemBufferedInput );
			this.iLogOutputLog = new CItemToggle( "TraceLog", CDTXMania.ConfigIni.bログ出力,
				"Traceログ出力：\nDTXManiaLog.txt にログを出力します。\n変更した場合は、DTXMania の再起動\n後に有効となります。",
				"Turn ON to put debug log to\n DTXManiaLog.txt\nTo take it effective, you need to\n re-open DTXMania." );
			this.list項目リスト.Add( this.iLogOutputLog );

			this.iSystemGoToKeyAssign = new CItemBase( "System Keys", CItemBase.Eパネル種別.通常,
			"システムのキー入力に関する項目を設\n定します。",
			"Settings for the system key/pad inputs." );
			this.list項目リスト.Add( this.iSystemGoToKeyAssign );

			
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.System;
		}

		public void t項目リストの設定・Drums()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iDrumsReturnToMenu = new CItemBase( "<< Return To Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iDrumsReturnToMenu );
			this.iDrumsAutoPlayAll = new CItemThreeState( "AutoPlay (All)", CItemThreeState.E状態.不定,
				"全パッドの自動演奏の ON/OFF を\nまとめて切り替えます。",
				"You can change whether Auto or not\n for all drums lanes at once." );
			this.list項目リスト.Add( this.iDrumsAutoPlayAll );
			this.iDrumsLeftCymbal = new CItemToggle( "    LeftCymbal", CDTXMania.ConfigIni.bAutoPlay.LC,
				"左シンバルを自動で演奏します。",
				"To play LeftCymbal automatically." );
			this.list項目リスト.Add( this.iDrumsLeftCymbal );
			this.iDrumsHiHat = new CItemToggle( "    HiHat", CDTXMania.ConfigIni.bAutoPlay.HH,
				"ハイハットを自動で演奏します。\n（クローズ、オープンとも）",
				"To play HiHat automatically.\n(It effects to both HH-close and\n HH-open)" );
			this.list項目リスト.Add( this.iDrumsHiHat );
			this.iDrumsSnare = new CItemToggle( "    Snare", CDTXMania.ConfigIni.bAutoPlay.SD,
				"スネアを自動で演奏します。",
				"To play Snare automatically." );
			this.list項目リスト.Add( this.iDrumsSnare );
			this.iDrumsBass = new CItemToggle( "    BassDrum", CDTXMania.ConfigIni.bAutoPlay.BD,
				"バスドラムを自動で演奏します。",
				"To play Bass Drum automatically." );
			this.list項目リスト.Add( this.iDrumsBass );
			this.iDrumsHighTom = new CItemToggle( "    HighTom", CDTXMania.ConfigIni.bAutoPlay.HT,
				"ハイタムを自動で演奏します。",
				"To play High Tom automatically." );
			this.list項目リスト.Add( this.iDrumsHighTom );
			this.iDrumsLowTom = new CItemToggle( "    LowTom", CDTXMania.ConfigIni.bAutoPlay.LT,
				"ロータムを自動で演奏します。",
				"To play Low Tom automatically." );
			this.list項目リスト.Add( this.iDrumsLowTom );
			this.iDrumsFloorTom = new CItemToggle( "    FloorTom", CDTXMania.ConfigIni.bAutoPlay.FT,
				"フロアタムを自動で演奏します。",
				"To play Floor Tom automatically." );
			this.list項目リスト.Add( this.iDrumsFloorTom );
			this.iDrumsCymbalRide = new CItemToggle( "    Cym/Ride", CDTXMania.ConfigIni.bAutoPlay.CY,
				"右シンバルとライドシンバルを自動で\n演奏します。",
				"To play both right- and Ride-Cymbal\n automatically." );
			this.list項目リスト.Add( this.iDrumsCymbalRide );

			this.iDrumsScrollSpeed = new CItemInteger("ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Drums,
				"演奏時のドラム譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 を指定可能です。",
				"To change the scroll speed for the\ndrums lanes.\nYou can set it from x0.5 to x1000.0.\n(ScrollSpeed=x0.5 means half speed)");
			this.list項目リスト.Add(this.iDrumsScrollSpeed);
			this.iDrumsSudden = new CItemToggle("Sudden", CDTXMania.ConfigIni.bSudden.Drums,
				"ドラムチップが譜面の下の方から表\n示されるようになります。",
				"Drums chips are disappered until they\ncome near the hit bar, and suddenly\nappears.");
			this.list項目リスト.Add(this.iDrumsSudden);
			this.iDrumsHidden = new CItemToggle("Hidden", CDTXMania.ConfigIni.bHidden.Drums,
				"ドラムチップが譜面の下の方で表示\nされなくなります。",
				"Drums chips are hidden by approaching\nto the hit bar. ");
			this.list項目リスト.Add(this.iDrumsHidden);

			this.iCommonDark = new CItemList("Dark", CItemBase.Eパネル種別.通常, (int)CDTXMania.ConfigIni.eDark,
				"HALF: 背景、レーン、ゲージが表示\nされなくなります。\nFULL: さらに小節線、拍線、判定ラ\nイン、パッドも表示されなくなります。",
				"OFF: all display parts are shown.\nHALF: wallpaper, lanes and gauge are\n disappeared.\nFULL: additionaly to HALF, bar/beat\n lines, hit bar, pads are disappeared.",
				new string[] { "OFF", "HALF", "FULL" });
			this.list項目リスト.Add(this.iCommonDark);


			this.iDrumsReverse = new CItemToggle("Reverse", CDTXMania.ConfigIni.bReverse.Drums,
				"ドラムチップが譜面の下から上に流\nれるようになります。",
				"The scroll way is reversed. Drums chips\nflow from the bottom to the top.");
			this.list項目リスト.Add(this.iDrumsReverse);

			this.iSystemRisky = new CItemInteger("Risky", 0, 10, CDTXMania.ConfigIni.nRisky,
				"Riskyモードの設定:\n1以上の値にすると、その回数分の\nPoor/MissでFAILEDとなります。\n0にすると無効になり、\nDamageLevelに従ったゲージ増減と\nなります。\nStageFailedの設定と併用できます。",
				"Risky mode:\nSet over 1, in case you'd like to specify\n the number of Poor/Miss times to be\n FAILED.\nSet 0 to disable Risky mode.");
			this.list項目リスト.Add(this.iSystemRisky);

			this.iDrumsTight = new CItemToggle("Tight", CDTXMania.ConfigIni.bTight,
				"ドラムチップのないところでパッドを\n叩くとミスになります。",
				"It becomes MISS to hit pad without\n chip.");
			this.list項目リスト.Add(this.iDrumsTight);

			this.iDrumsComboPosition = new CItemList("ComboPosition", CItemBase.Eパネル種別.通常, (int)CDTXMania.ConfigIni.ドラムコンボ文字の表示位置,
				"演奏時のドラムコンボ文字列の位置\nを指定します。",
				"The display position for Drums Combo.\nNote that it doesn't take effect\n at Autoplay ([Left] is forcely used).",
				new string[] { "Left", "Center", "Right", "OFF" });
			this.list項目リスト.Add(this.iDrumsComboPosition);
			this.iDrumsPosition = new CItemList("Position", CItemBase.Eパネル種別.通常, (int)CDTXMania.ConfigIni.判定文字表示位置.Drums,
				"ドラムの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: 判定ライン下\n  OFF: 表示しない",
				"The position to show judgement mark.\n(Perfect, Great, ...)\n\n P-A: on the lanes.\n P-B: under the hit bar.\n OFF: no judgement mark.",
				new string[] { "P-A", "P-B", "OFF" });
			this.list項目リスト.Add(this.iDrumsPosition);

	
			this.iSystemHHGroup = new CItemList("HH Group", CItemBase.Eパネル種別.通常, (int)CDTXMania.ConfigIni.eHHGroup,
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

			this.iSystemCymbalFree = new CItemToggle( "CymbalFree", CDTXMania.ConfigIni.bシンバルフリー,
			"シンバルフリーモード：\n左シンバル・右シンバルの区別をなく\nします。ライドシンバルまで区別をな\nくすか否かは、CYGroup に従います。\n",
			"Turn ON to group LC (left cymbal) and\n CY (right cymbal).\nWhether RD (ride cymbal) is also\n grouped or not depends on the\n'CY Group' setting." );
			this.list項目リスト.Add( this.iSystemCymbalFree );

			
			this.iSystemHitSoundPriorityHH = new CItemList( "HH Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityHH,
				"発声音決定の優先順位：\nハイハットレーン打ち分け有効時に、\nチップの発声音をどのように決定する\nかを指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using HH-0,1 and 2.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" } );
			this.list項目リスト.Add( this.iSystemHitSoundPriorityHH );
			this.iSystemHitSoundPriorityFT = new CItemList( "FT Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityFT,
				"発声音決定の優先順位：\nフロアタム打ち分け有効時に、チップ\nの発声音をどのように決定するかを\n指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using FT-0.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" } );
			this.list項目リスト.Add( this.iSystemHitSoundPriorityFT );
			this.iSystemHitSoundPriorityCY = new CItemList( "CY Priority", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eHitSoundPriorityCY,
				"発声音決定の優先順位：\nシンバルレーン打ち分け有効時に、\nチップの発声音をどのように決定する\nかを指定します。\n  C > P ... チップの音が優先\n  P > C ... 叩いたパッドの音が優先",
				"To specify playing sound in case you're\n using CY-0.\n\nC>P:\n Chip sound is prior to the pad sound.\nP>C:\n Pad sound is prior to the chip sound.",
				new string[] { "C>P", "P>C" } );
			this.list項目リスト.Add( this.iSystemHitSoundPriorityCY );
			this.iSystemFillIn = new CItemToggle( "FillIn", CDTXMania.ConfigIni.bフィルイン有効,
				"フィルインエフェクトの使用：\nフィルイン区間の爆発パターンに特別\nのエフェクトを使用します。\nフィルインエフェクトの描画にはそれな\nりのマシンパワーが必要とされます。",
				"To show bursting effects at the fill-in\n zone or not." );
			this.list項目リスト.Add( this.iSystemFillIn );



			this.iSystemHitSound = new CItemToggle( "HitSound", CDTXMania.ConfigIni.bドラム打音を発声する,
			"打撃音の再生：\nこれをOFFにすると、パッドを叩いた\nときの音を再生しなくなります（ドラム\nのみ）。\nDTX の音色で演奏したい場合などに\nOFF にします。",
			"Turn OFF if you don't want to play\n hitting chip sound.\nIt is useful to play with real/electric\n drums kit." );
			this.list項目リスト.Add( this.iSystemHitSound );
			this.iSystemSoundMonitorDrums = new CItemToggle( "DrumsMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Drums,
				"ドラム音モニタ：\nドラム音を他の音より大きめの音量で\n発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
				"To enhance the drums chip sound\n(except autoplay)." );
			this.list項目リスト.Add( this.iSystemSoundMonitorDrums );
			this.iSystemMinComboDrums = new CItemInteger( "D-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Drums,
				"表示可能な最小コンボ数（ドラム）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the drums.\nYou can specify from 1 to 99999." );
			this.list項目リスト.Add( this.iSystemMinComboDrums );

			
			// #23580 2011.1.3 yyagi
			this.iDrumsInputAdjustTimeMs = new CItemInteger( "InputAdjust", -99, 0, CDTXMania.ConfigIni.nInputAdjustTimeMs.Drums,
				"ドラムの入力タイミングの微調整を\n行います。\n-99 ～ 0ms まで指定可能です。\n入力ラグを軽減するためには、負の\n値を指定してください。",
				"To adjust the drums input timing.\nYou can set from -99 to 0ms.\nTo decrease input lag, set minus value." );
			this.list項目リスト.Add( this.iDrumsInputAdjustTimeMs );
			// #24074 2011.01.23 add ikanick
			this.iDrumsGraph = new CItemToggle( "Graph", CDTXMania.ConfigIni.bGraph.Drums,
				"最高スキルと比較できるグラフを\n表示します。\nオートプレイだと表示されません。",
				"To draw Graph \n or not." );
			this.list項目リスト.Add( this.iDrumsGraph );

			this.iDrumsGoToKeyAssign = new CItemBase( "Drums Keys", CItemBase.Eパネル種別.通常,
				"ドラムのキー入力に関する項目を設\n定します。",
				"Settings for the drums key/pad inputs." );
			this.list項目リスト.Add( this.iDrumsGoToKeyAssign );

			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Drums;
		}

		public void t項目リストの設定・Guitar()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iGuitarReturnToMenu = new CItemBase( "<< Return To Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iGuitarReturnToMenu );
			this.iGuitarAutoPlay = new CItemToggle( "AutoPlay", CDTXMania.ConfigIni.bAutoPlay.Guitar,
				"ギターパートを自動で演奏します。",
				"To play the guitar part automatically." );
			this.list項目リスト.Add( this.iGuitarAutoPlay );
			this.iGuitarScrollSpeed = new CItemInteger( "ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Guitar,
				"演奏時のギター譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 までを指定可能です。",
				"To change the scroll speed for the\nguitar lanes.\nYou can set it from x0.5 to x1000.0.\n(ScrollSpeed=x0.5 means half speed)" );
			this.list項目リスト.Add( this.iGuitarScrollSpeed );
			this.iGuitarSudden = new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Guitar,
				"ギターチップがヒットバー付近にくる\nまで表示されなくなります。",
				"Guitar chips are disappered until they\ncome near the hit bar, and suddenly\nappears." );
			this.list項目リスト.Add( this.iGuitarSudden );
			this.iGuitarHidden = new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Guitar,
				"ギターチップがヒットバー付近で表示\nされなくなります。",
				"Guitar chips are hidden by approaching\nto the hit bar. " );
			this.list項目リスト.Add( this.iGuitarHidden );
			this.iGuitarReverse = new CItemToggle( "Reverse", CDTXMania.ConfigIni.bReverse.Guitar,
				"ギターチップが譜面の上から下に流\nれるようになります。",
				"The scroll way is reversed. Guitar chips\nflow from the top to the bottom." );
			this.list項目リスト.Add( this.iGuitarReverse );
			this.iGuitarPosition = new CItemList( "Position", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.判定文字表示位置.Guitar,
				"ギターの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: COMBO の下\n  OFF: 表示しない",
				"The position to show judgement mark.\n(Perfect, Great, ...)\n\n P-A: on the lanes.\n P-B: under the COMBO indication.\n OFF: no judgement mark.",
				new string[] { "P-A", "P-B", "OFF" } );
			this.list項目リスト.Add( this.iGuitarPosition );
			this.iGuitarRandom = new CItemList( "Random", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eRandom.Guitar,
				"ギターのチップがランダムに降ってき\nます。\n  Part: 小節・レーン単位で交換\n  Super: チップ単位で交換\n  Hyper: 全部完全に変更",
				"Guitar chips come randomly.\n\n Part: swapping lanes randomly for each\n  measures.\n Super: swapping chip randomly\n Hyper: swapping randomly\n  (number of lanes also changes)",
				new string[] { "OFF", "Part", "Super", "Hyper" } );
			this.list項目リスト.Add( this.iGuitarRandom );
			this.iGuitarLight = new CItemToggle( "Light", CDTXMania.ConfigIni.bLight.Guitar,
				"ギターチップのないところでピッキン\nグしても BAD になりません。",
				"Even if you pick without any chips,\nit doesn't become BAD." );
			this.list項目リスト.Add( this.iGuitarLight );
			this.iGuitarLeft = new CItemToggle( "Left", CDTXMania.ConfigIni.bLeft.Guitar,
				"ギターの RGB の並びが左右反転し\nます。（左利きモード）",
				"Lane order 'R-G-B' becomes 'B-G-R'\nfor lefty." );
			this.list項目リスト.Add( this.iGuitarLeft );

			this.iSystemSoundMonitorGuitar = new CItemToggle( "GuitarMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Guitar,
			"ギター音モニタ：\nギター音を他の音より大きめの音量\nで発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
			"To enhance the guitar chip sound\n(except autoplay)." );
			this.list項目リスト.Add( this.iSystemSoundMonitorGuitar );
			this.iSystemMinComboGuitar = new CItemInteger( "G-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Guitar,
				"表示可能な最小コンボ数（ギター）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the guitar.\nYou can specify from 1 to 99999." );
			this.list項目リスト.Add( this.iSystemMinComboGuitar );

			
			// #23580 2011.1.3 yyagi
			this.iGuitarInputAdjustTimeMs = new CItemInteger( "InputAdjust", -99, 0, CDTXMania.ConfigIni.nInputAdjustTimeMs.Guitar,
				"ギターの入力タイミングの微調整を\n行います。\n-99 ～ 0ms まで指定可能です。\n入力ラグを軽減するためには、負の\n値を指定してください。",
				"To adjust the guitar input timing.\nYou can set from -99 to 0ms.\nTo decrease input lag, set minus value." );
			this.list項目リスト.Add( this.iGuitarInputAdjustTimeMs );

			this.iGuitarGoToKeyAssign = new CItemBase( "Guitar Keys", CItemBase.Eパネル種別.通常,
				"ギターのキー入力に関する項目を設\n定します。",
				"Settings for the guitar key/pad inputs." );
			this.list項目リスト.Add( this.iGuitarGoToKeyAssign );

			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Guitar;
		}

		public void t項目リストの設定・Bass()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();

			this.iBassReturnToMenu = new CItemBase( "<< Return To Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iBassReturnToMenu );
			this.iBassAutoPlay = new CItemToggle( "AutoPlay", CDTXMania.ConfigIni.bAutoPlay.Bass,
				"ベースパートを自動で演奏します。",
				"To play the bass part automatically." );
			this.list項目リスト.Add( this.iBassAutoPlay );
			this.iBassScrollSpeed = new CItemInteger( "ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Bass,
				"演奏時のベース譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 までを指定可能です。",
				"To change the scroll speed for the\nbass lanes.\nYou can set it from x0.5 to x1000.0.\n(ScrollSpeed=x0.5 means half speed)" );
			this.list項目リスト.Add( this.iBassScrollSpeed );
			this.iBassSudden = new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Bass,
				"ベースチップがヒットバー付近にくる\nまで表示されなくなります。",
				"Bass chips are disappered until they\ncome near the hit bar, and suddenly\nappears." );
			this.list項目リスト.Add( this.iBassSudden );
			this.iBassHidden = new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Bass,
				"ベースチップがヒットバー付近で表示\nされなくなります。",
				"Bass chips are hidden by approaching\nto the hit bar." );
			this.list項目リスト.Add( this.iBassHidden );
			this.iBassReverse = new CItemToggle( "Reverse", CDTXMania.ConfigIni.bReverse.Bass,
				"ベースチップが譜面の上から下に流\nれるようになります。",
				"The scroll way is reversed. Bass chips\nflow from the top to the bottom." );
			this.list項目リスト.Add( this.iBassReverse );
			this.iBassPosition = new CItemList( "Position", CItemBase.Eパネル種別.通常,
				(int) CDTXMania.ConfigIni.判定文字表示位置.Bass,
				"ベースの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: COMBO の下\n  OFF: 表示しない",
				"The position to show judgement mark.\n(Perfect, Great, ...)\n\n P-A: on the lanes.\n P-B: under the COMBO indication.\n OFF: no judgement mark.",
				new string[] { "P-A", "P-B", "OFF" } );
			this.list項目リスト.Add( this.iBassPosition );
			this.iBassRandom = new CItemList( "Random", CItemBase.Eパネル種別.通常,
				(int) CDTXMania.ConfigIni.eRandom.Bass,
				"ベースのチップがランダムに降ってき\nます。\n  Part: 小節・レーン単位で交換\n  Super: チップ単位で交換\n  Hyper: 全部完全に変更",
				"Bass chips come randomly.\n\n Part: swapping lanes randomly for each\n  measures.\n Super: swapping chip randomly\n Hyper: swapping randomly\n  (number of lanes also changes)",
				new string[] { "OFF", "Part", "Super", "Hyper" } );
			this.list項目リスト.Add( this.iBassRandom );
			this.iBassLight = new CItemToggle( "Light", CDTXMania.ConfigIni.bLight.Bass,
				"ベースチップのないところでピッキン\nグしても BAD になりません。",
				"Even if you pick without any chips,\nit doesn't become BAD." );
			this.list項目リスト.Add( this.iBassLight );
			this.iBassLeft = new CItemToggle( "Left", CDTXMania.ConfigIni.bLeft.Bass,
				"ベースの RGB の並びが左右反転し\nます。（左利きモード）",
				"Lane order 'R-G-B' becomes 'B-G-R'\nfor lefty." );
			this.list項目リスト.Add( this.iBassLeft );

			this.iSystemSoundMonitorBass = new CItemToggle( "BassMonitor", CDTXMania.ConfigIni.b演奏音を強調する.Bass,
			"ベース音モニタ：\nベース音を他の音より大きめの音量\nで発声します。\nただし、オートプレイの場合は通常音\n量で発声されます。",
			"To enhance the bass chip sound\n(except autoplay)." );
			this.list項目リスト.Add( this.iSystemSoundMonitorBass );

			this.iSystemMinComboBass = new CItemInteger( "B-MinCombo", 1, 0x1869f, CDTXMania.ConfigIni.n表示可能な最小コンボ数.Bass,
				"表示可能な最小コンボ数（ベース）：\n画面に表示されるコンボの最小の数\nを指定します。\n1 ～ 99999 の値が指定可能です。",
				"Initial number to show the combo\n for the bass.\nYou can specify from 1 to 99999." );
			this.list項目リスト.Add( this.iSystemMinComboBass );

			
			// #23580 2011.1.3 yyagi
			this.iBassInputAdjustTimeMs = new CItemInteger( "InputAdjust", -99, 0, CDTXMania.ConfigIni.nInputAdjustTimeMs.Bass,
				"ベースの入力タイミングの微調整を\n行います。\n-99 ～ 0ms まで指定可能です。入力ラグを軽減するためには、負の\n値を指定してください。",
				"To adjust the bass input timing.\nYou can set from -99 to 0ms.\nTo decrease input lag, set minus value." );
			this.list項目リスト.Add( this.iBassInputAdjustTimeMs );

			this.iBassGoToKeyAssign = new CItemBase( "Bass Keys", CItemBase.Eパネル種別.通常,
				"ベースのキー入力に関する項目を設\n定します。",
				"Settings for the bass key/pad inputs.");
			this.list項目リスト.Add( this.iBassGoToKeyAssign );

			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Bass;
		}


		/// <summary>
		/// ESC押下時の右メニュー描画
		/// </summary>
		public void tEsc押下()
		{
			if ( this.eメニュー種別 == Eメニュー種別.KeyAssignSystem )
			{
				t項目リストの設定・System();
			}
			else if ( this.eメニュー種別 == Eメニュー種別.KeyAssignDrums )
			{
				t項目リストの設定・Drums();
			}
			else if ( this.eメニュー種別 == Eメニュー種別.KeyAssignGuitar )
			{
				t項目リストの設定・Guitar();
			}
			else if ( this.eメニュー種別 == Eメニュー種別.KeyAssignBass )
			{
				t項目リストの設定・Bass();
			}
			// これ以外なら何もしない
		}

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
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsLC )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.LC );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHHC )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.HH );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHHO )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.HHO );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsSD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.SD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsBD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.BD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsHT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.HT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsLT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.LT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsFT )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.FT );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsCY )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.CY );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsRD )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.RD );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsFP )			// #27029 2012.1.4 from
			{																							//
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.DRUMS, EKeyConfigPad.FP );	//
			}																							//
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarR )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.R );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarG )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.G );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarB )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.B );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarPick )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.Pick );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarWail )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.Wail );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarDecide )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.Decide );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarCancel )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.GUITAR, EKeyConfigPad.Cancel );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassR )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.R );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassG )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.G );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassB )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.B );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassPick )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.Pick );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassWail )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.Wail );
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassDecide )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.Decide );
			}
			else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassCancel )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.BASS, EKeyConfigPad.Cancel );
			}
			else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignSystemCapture )
			{
				CDTXMania.stageコンフィグ.tパッド選択通知( EKeyConfigPart.SYSTEM, EKeyConfigPad.Capture);
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
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iDrumsAutoPlayAll )
				{
					this.t全部のドラムパッドのAutoを切り替える( this.iDrumsAutoPlayAll.e現在の状態 == CItemThreeState.E状態.ON );
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iSystemGoToKeyAssign )			// #24609 2011.4.12 yyagi
				{
					t項目リストの設定・KeyAssignSystem();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignSystemReturnToMenu )	// #24609 2011.4.12 yyagi
				{
					t項目リストの設定・System();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iDrumsGoToKeyAssign )				// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・KeyAssignDrums();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignDrumsReturnToMenu )		// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・Drums();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iGuitarGoToKeyAssign )			// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・KeyAssignGuitar();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignGuitarReturnToMenu )	// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・Guitar();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iBassGoToKeyAssign )				// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・KeyAssignBass();
				}
				else if ( this.list項目リスト[ this.n現在の選択項目 ] == this.iKeyAssignBassReturnToMenu )		// #24525 2011.3.15 yyagi
				{
					t項目リストの設定・Bass();
				}
			}
		}
		public void t項目リストの設定・Exit()
		{
			this.tConfigIniへ記録する();
			this.eメニュー種別 = Eメニュー種別.Unknown;
		}
		public void t項目リストの設定・KeyAssignSystem()
		{
			//			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iKeyAssignSystemReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iKeyAssignSystemReturnToMenu );
			this.iKeyAssignSystemCapture = new CItemBase( "Capture",
				"キャプチャキー設定：\n画面キャプチャのキーの割り当てを設\n定します。",
				"Capture key assign:\nTo assign key for screen capture.\n (You can use keyboard only. You can't\nuse pads to capture screenshot." );
			this.list項目リスト.Add( this.iKeyAssignSystemCapture );

			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignSystem;
		}
		public void t項目リストの設定・KeyAssignDrums()
		{
//			this.tConfigIniへ記録する();
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
			this.iKeyAssignDrumsFP = new CItemBase( "FootPedal",									// #27029 2012.1.4 from
				"ドラムのキー設定：\nハイハットのフットペダルへのキーの\n割り当てを設定します。",	//
				"Drums key assign:\nTo assign key/pads for HHFootPedal\n button." );				//
			this.list項目リスト.Add( this.iKeyAssignDrumsFP );										//
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignDrums;
		}
		public void t項目リストの設定・KeyAssignGuitar()
		{
//			this.tConfigIniへ記録する();
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
		public void t項目リストの設定・KeyAssignBass()
		{
//			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iKeyAssignBassReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他,
				"左側のメニューに戻ります。",
				"Return to left menu." );
			this.list項目リスト.Add( this.iKeyAssignBassReturnToMenu );
			this.iKeyAssignBassR = new CItemBase( "R",
				"ベースのキー設定：\nRボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for R button." );
			this.list項目リスト.Add( this.iKeyAssignBassR );
			this.iKeyAssignBassG = new CItemBase( "G",
				"ベースのキー設定：\nGボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for G button." );
			this.list項目リスト.Add( this.iKeyAssignBassG );
			this.iKeyAssignBassB = new CItemBase( "B",
				"ベースのキー設定：\nBボタンへのキーの割り当てを設定し\nます。",
				"Bass key assign:\nTo assign key/pads for B button." );
			this.list項目リスト.Add( this.iKeyAssignBassB );
			this.iKeyAssignBassPick = new CItemBase( "Pick",
				"ベースのキー設定：\nピックボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Pick button." );
			this.list項目リスト.Add( this.iKeyAssignBassPick );
			this.iKeyAssignBassWail = new CItemBase( "Wailing",
				"ベースのキー設定：\nWailingボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Wailing button." );
			this.list項目リスト.Add( this.iKeyAssignBassWail );
			this.iKeyAssignBassDecide = new CItemBase( "Decide",
				"ベースのキー設定：\n決定ボタンへのキーの割り当てを設\n定します。",
				"Bass key assign:\nTo assign key/pads for Decide button." );
			this.list項目リスト.Add( this.iKeyAssignBassDecide );
			this.iKeyAssignBassCancel = new CItemBase( "Cancel",
				"ベースのキー設定：\nキャンセルボタンへのキーの割り当\nてを設定します。",
				"Bass key assign:\nTo assign key/pads for Cancel button." );
			this.list項目リスト.Add( this.iKeyAssignBassCancel );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.KeyAssignBass;
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
				#region [初めての進行描画]
				if ( base.b初めての進行描画 )
				{
					this.nスクロール用タイマ値 = CDTXMania.Timer.n現在時刻;
					this.ct三角矢印アニメ.t開始( 0, 9, 50, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				#endregion
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

						case CItemBase.E種別.整数:		// #24789 2011.4.8 yyagi: add PlaySpeed supports (copied them from OPTION)
							{
								if ( this.list項目リスト[ nItem ] == this.iCommonPlaySpeed )
								{
									double d = ( (double) ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値 ) / 20.0;
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, d.ToString( "0.000" ), ( j == 0 ) && this.b要素値にフォーカス中 );
								}
								else if ( this.list項目リスト[ nItem ] == this.iDrumsScrollSpeed || this.list項目リスト[ nItem ] == this.iGuitarScrollSpeed || this.list項目リスト[ nItem ] == this.iBassScrollSpeed )
								{
									float f = ( ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値 + 1 ) * 0.5f;
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, f.ToString( "x0.0" ), ( j == 0 ) && this.b要素値にフォーカス中 );
								}
								else
								{
									CDTXMania.stageコンフィグ.actFont.t文字列描画( x + 210, y + 12, ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値.ToString(), ( j == 0 ) && this.b要素値にフォーカス中 );
								}
							}
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
					int x;
					int y_upper;
					int y_lower;
					if( !this.b要素値にフォーカス中 )
					{
						x = 0x114;
						y_upper = 0xba - this.ct三角矢印アニメ.n現在の値;
						y_lower = 0xfe + this.ct三角矢印アニメ.n現在の値;
					}
					else
					{
						x = 0x210;
						y_upper = 0xc6 - this.ct三角矢印アニメ.n現在の値;
						y_lower = 0xf2 + this.ct三角矢印アニメ.n現在の値;
					}
					if( this.tx三角矢印 != null )
					{
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, x, y_upper, new Rectangle( 0, 0, 0x20, 0x10 ) );
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, x, y_lower, new Rectangle( 0, 0x10, 0x20, 0x10 ) );
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
			Drums,
			Guitar,
			Bass,
			KeyAssignSystem,		// #24609 2011.4.12 yyagi: 画面キャプチャキーのアサイン
			KeyAssignDrums,
			KeyAssignGuitar,
			KeyAssignBass,
			Unknown

		}

		private bool b項目リスト側にフォーカスがある;
		private bool b要素値にフォーカス中;
		private CCounter ct三角矢印アニメ;
		private Eメニュー種別 eメニュー種別;
		private CItemBase iKeyAssignSystemCapture;			// #24609
		private CItemBase iKeyAssignSystemReturnToMenu;		// #24609
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
		private CItemBase iKeyAssignDrumsFP;	// #27029 2012.1.4 from
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
//		private CItemToggle iSystemGraph; #24074 2011.01.23 comment-out ikanick オプション(Drums)へ移行
		private CItemInteger iSystemBGAlpha;
		private CItemToggle iSystemBGMSound;
		private CItemInteger iSystemChipVolume;
		private CItemList iSystemCYGroup;
		private CItemToggle iSystemCymbalFree;
		private CItemList iSystemDamageLevel;
		private CItemToggle iSystemDebugInfo;
//		private CItemToggle iSystemDrums;
		private CItemToggle iSystemFillIn;
		private CItemList iSystemFTGroup;
		private CItemToggle iSystemFullscreen;
//		private CItemToggle iSystemGuitar;
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
		private CItemList	iSystemShowLag;					// #25370 2011.6.3 yyagi
		private CItemToggle iSystemAutoResultCapture;		// #25399 2011.6.9 yyagi
		private CItemToggle iSystemBufferedInput;
		private CItemInteger iSystemRisky;					// #23559 2011.7.27 yyagi

		private List<CItemBase> list項目リスト;
		private long nスクロール用タイマ値;
		private int n現在のスクロールカウンタ;
		private int n目標のスクロールカウンタ;
		private Point[] ptパネルの基本座標 = new Point[] { new Point( 0x12d, 3 ), new Point( 0x12d, 0x35 ), new Point( 0x12d, 0x67 ), new Point( 0x12d, 0x99 ), new Point( 0x114, 0xcb ), new Point( 0x12d, 0xfd ), new Point( 0x12d, 0x12f ), new Point( 0x12d, 0x161 ), new Point( 0x12d, 0x193 ), new Point( 0x12d, 0x1c5 ) };
		private CTexture txその他項目行パネル;
		private CTexture tx三角矢印;
		private CTexture tx通常項目行パネル;

		private CItemBase iDrumsGoToKeyAssign;
		private CItemBase iGuitarGoToKeyAssign;
		private CItemBase iBassGoToKeyAssign;
		private CItemBase iSystemGoToKeyAssign;		// #24609

		private CItemList iSystemGRmode;

		private CItemToggle iBassAutoPlay;
		private CItemToggle iBassHidden;
		private CItemToggle iBassLeft;
		private CItemToggle iBassLight;
		private CItemList iBassPosition;
		private CItemList iBassRandom;
		private CItemBase iBassReturnToMenu;
		private CItemToggle iBassReverse;
		private CItemInteger iBassScrollSpeed;
		private CItemToggle iBassSudden;
		private CItemList iCommonDark;
		private CItemInteger iCommonPlaySpeed;
//		private CItemBase iCommonReturnToMenu;
		private CItemThreeState iDrumsAutoPlayAll;
		private CItemToggle iDrumsBass;
		private CItemList iDrumsComboPosition;
		private CItemToggle iDrumsCymbalRide;
		private CItemToggle iDrumsFloorTom;
		private CItemToggle iDrumsHidden;
		private CItemToggle iDrumsHighTom;
		private CItemToggle iDrumsHiHat;
		private CItemToggle iDrumsLeftCymbal;
		private CItemToggle iDrumsLowTom;
		private CItemList iDrumsPosition;
		private CItemBase iDrumsReturnToMenu;
		private CItemToggle iDrumsReverse;
		private CItemInteger iDrumsScrollSpeed;
		private CItemToggle iDrumsSnare;
		private CItemToggle iDrumsSudden;
		private CItemToggle iDrumsTight;
		private CItemToggle iDrumsGraph;        // #24074 2011.01.23 add ikanick
		private CItemToggle iGuitarAutoPlay;
		private CItemToggle iGuitarHidden;
		private CItemToggle iGuitarLeft;
		private CItemToggle iGuitarLight;
		private CItemList iGuitarPosition;
		private CItemList iGuitarRandom;
		private CItemBase iGuitarReturnToMenu;
		private CItemToggle iGuitarReverse;
		private CItemInteger iGuitarScrollSpeed;
		private CItemToggle iGuitarSudden;
		private CItemInteger iDrumsInputAdjustTimeMs;		// #23580 2011.1.3 yyagi
		private CItemInteger iGuitarInputAdjustTimeMs;		//
		private CItemInteger iBassInputAdjustTimeMs;		//

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
		private void t全部のドラムパッドのAutoを切り替える( bool bAutoON )
		{
			this.iDrumsLeftCymbal.bON = this.iDrumsHiHat.bON = this.iDrumsSnare.bON = this.iDrumsBass.bON = this.iDrumsHighTom.bON = this.iDrumsLowTom.bON = this.iDrumsFloorTom.bON = this.iDrumsCymbalRide.bON = bAutoON;
		}
		private void tConfigIniへ記録する()
		{
			switch( this.eメニュー種別 )
			{
				case Eメニュー種別.System:
					this.tConfigIniへ記録する・System();
					this.tConfigIniへ記録する・KeyAssignSystem();
					return;

				case Eメニュー種別.Drums:
					this.tConfigIniへ記録する・Drums();
					this.tConfigIniへ記録する・KeyAssignDrums();
					return;

				case Eメニュー種別.Guitar:
					this.tConfigIniへ記録する・Guitar();
					this.tConfigIniへ記録する・KeyAssignGuitar();
					return;

				case Eメニュー種別.Bass:
					this.tConfigIniへ記録する・Bass();
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
		private void tConfigIniへ記録する・KeyAssignSystem()
		{
		}
		private void tConfigIniへ記録する・System()
		{
			CDTXMania.ConfigIni.eDark = (Eダークモード) this.iCommonDark.n現在選択されている項目番号;
			CDTXMania.ConfigIni.n演奏速度 = this.iCommonPlaySpeed.n現在の値;

			CDTXMania.ConfigIni.bGuitar有効 = ( ( ( this.iSystemGRmode.n現在選択されている項目番号 + 1 ) / 2 ) == 1 );
				//this.iSystemGuitar.bON;
			CDTXMania.ConfigIni.bDrums有効 = ( ( ( this.iSystemGRmode.n現在選択されている項目番号 + 1 ) % 2 ) == 1 );
				//this.iSystemDrums.bON;

			CDTXMania.ConfigIni.b全画面モード = this.iSystemFullscreen.bON;
			CDTXMania.ConfigIni.bSTAGEFAILED有効 = this.iSystemStageFailed.bON;
			CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする = this.iSystemRandomFromSubBox.bON;

			CDTXMania.ConfigIni.bWave再生位置自動調整機能有効 = this.iSystemAdjustWaves.bON;
			CDTXMania.ConfigIni.b垂直帰線待ちを行う = this.iSystemVSyncWait.bON;
			CDTXMania.ConfigIni.bバッファ入力を行う = this.iSystemBufferedInput.bON;
			CDTXMania.ConfigIni.bAVI有効 = this.iSystemAVI.bON;
			CDTXMania.ConfigIni.bBGA有効 = this.iSystemBGA.bON;
//			CDTXMania.ConfigIni.bGraph有効 = this.iSystemGraph.bON;#24074 2011.01.23 comment-out ikanick オプション(Drums)へ移行
			CDTXMania.ConfigIni.n曲が選択されてからプレビュー音が鳴るまでのウェイトms = this.iSystemPreviewSoundWait.n現在の値;
			CDTXMania.ConfigIni.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms = this.iSystemPreviewImageWait.n現在の値;
			CDTXMania.ConfigIni.b演奏情報を表示する = this.iSystemDebugInfo.bON;
			CDTXMania.ConfigIni.n背景の透過度 = this.iSystemBGAlpha.n現在の値;
			CDTXMania.ConfigIni.bBGM音を発声する = this.iSystemBGMSound.bON;
			CDTXMania.ConfigIni.b歓声を発声する = this.iSystemAudienceSound.bON;
			CDTXMania.ConfigIni.eダメージレベル = (Eダメージレベル) this.iSystemDamageLevel.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bScoreIniを出力する = this.iSystemSaveScore.bON;

			CDTXMania.ConfigIni.bログ出力 = this.iLogOutputLog.bON;
			CDTXMania.ConfigIni.n手動再生音量 = this.iSystemChipVolume.n現在の値;
			CDTXMania.ConfigIni.n自動再生音量 = this.iSystemAutoChipVolume.n現在の値;
			CDTXMania.ConfigIni.bストイックモード = this.iSystemStoicMode.bON;

			CDTXMania.ConfigIni.nShowLagType = this.iSystemShowLag.n現在選択されている項目番号;		// #25370 2011.6.3 yyagi
			CDTXMania.ConfigIni.bIsAutoResultCapture = this.iSystemAutoResultCapture.bON;		// #25399 2011.6.9 yyagi

			CDTXMania.ConfigIni.nRisky = this.iSystemRisky.n現在の値;						// #23559 2911.7.27 yyagi
		}
		private void tConfigIniへ記録する・Bass()
		{
			CDTXMania.ConfigIni.bAutoPlay.Bass = this.iBassAutoPlay.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Bass = this.iBassScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.bSudden.Bass = this.iBassSudden.bON;
			CDTXMania.ConfigIni.bHidden.Bass = this.iBassHidden.bON;
			CDTXMania.ConfigIni.bReverse.Bass = this.iBassReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Bass = (E判定文字表示位置) this.iBassPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eRandom.Bass = (Eランダムモード) this.iBassRandom.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bLight.Bass = this.iBassLight.bON;
			CDTXMania.ConfigIni.bLeft.Bass = this.iBassLeft.bON;
			CDTXMania.ConfigIni.nInputAdjustTimeMs.Bass = this.iBassInputAdjustTimeMs.n現在の値;		// #23580 2011.1.3 yyagi

			CDTXMania.ConfigIni.b演奏音を強調する.Bass = this.iSystemSoundMonitorBass.bON;
			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Bass = this.iSystemMinComboBass.n現在の値;
		}

		private void tConfigIniへ記録する・Drums()
		{
			CDTXMania.ConfigIni.bAutoPlay.LC = this.iDrumsLeftCymbal.bON;
			CDTXMania.ConfigIni.bAutoPlay.HH = this.iDrumsHiHat.bON;
			CDTXMania.ConfigIni.bAutoPlay.SD = this.iDrumsSnare.bON;
			CDTXMania.ConfigIni.bAutoPlay.BD = this.iDrumsBass.bON;
			CDTXMania.ConfigIni.bAutoPlay.HT = this.iDrumsHighTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.LT = this.iDrumsLowTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.FT = this.iDrumsFloorTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.CY = this.iDrumsCymbalRide.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Drums = this.iDrumsScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.ドラムコンボ文字の表示位置 = (Eドラムコンボ文字の表示位置) this.iDrumsComboPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bSudden.Drums = this.iDrumsSudden.bON;
			CDTXMania.ConfigIni.bHidden.Drums = this.iDrumsHidden.bON;
			CDTXMania.ConfigIni.bReverse.Drums = this.iDrumsReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Drums = (E判定文字表示位置) this.iDrumsPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bTight = this.iDrumsTight.bON;
			CDTXMania.ConfigIni.nInputAdjustTimeMs.Drums = this.iDrumsInputAdjustTimeMs.n現在の値;		// #23580 2011.1.3 yyagi
			CDTXMania.ConfigIni.bGraph.Drums = this.iDrumsGraph.bON;// #24074 2011.01.23 add ikanick

			CDTXMania.ConfigIni.eHHGroup = (EHHGroup) this.iSystemHHGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eFTGroup = (EFTGroup) this.iSystemFTGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eCYGroup = (ECYGroup) this.iSystemCYGroup.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityHH = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityHH.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityFT = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityFT.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eHitSoundPriorityCY = (E打ち分け時の再生の優先順位) this.iSystemHitSoundPriorityCY.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bフィルイン有効 = this.iSystemFillIn.bON;
			CDTXMania.ConfigIni.b演奏音を強調する.Drums = this.iSystemSoundMonitorDrums.bON;
			CDTXMania.ConfigIni.bドラム打音を発声する = this.iSystemHitSound.bON;
			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Drums = this.iSystemMinComboDrums.n現在の値;
			CDTXMania.ConfigIni.bシンバルフリー = this.iSystemCymbalFree.bON;

			CDTXMania.ConfigIni.eDark = (Eダークモード)this.iCommonDark.n現在選択されている項目番号;
			CDTXMania.ConfigIni.nRisky = this.iSystemRisky.n現在の値;						// #23559 2911.7.27 yyagi
		}
		private void tConfigIniへ記録する・Guitar()
		{
			CDTXMania.ConfigIni.bAutoPlay.Guitar = this.iGuitarAutoPlay.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = this.iGuitarScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.bSudden.Guitar = this.iGuitarSudden.bON;
			CDTXMania.ConfigIni.bHidden.Guitar = this.iGuitarHidden.bON;
			CDTXMania.ConfigIni.bReverse.Guitar = this.iGuitarReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Guitar = (E判定文字表示位置) this.iGuitarPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eRandom.Guitar = (Eランダムモード) this.iGuitarRandom.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bLight.Guitar = this.iGuitarLight.bON;
			CDTXMania.ConfigIni.bLeft.Guitar = this.iGuitarLeft.bON;
			CDTXMania.ConfigIni.nInputAdjustTimeMs.Guitar = this.iGuitarInputAdjustTimeMs.n現在の値;	// #23580 2011.1.3 yyagi

			CDTXMania.ConfigIni.n表示可能な最小コンボ数.Guitar = this.iSystemMinComboGuitar.n現在の値;
			CDTXMania.ConfigIni.b演奏音を強調する.Guitar = this.iSystemSoundMonitorGuitar.bON;
		}
		//-----------------
		#endregion
	}
}
