using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;
using System.Diagnostics;
using System;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// DataContract はフィールドまたはプロパティを XML にシリアライズできます。
	/// DataMember が付いているメンバだけがシリアライズされます。シリアライズ可能な型については MSDN を参照してください。
	/// DataMember が付いていないメンバはデシリアライズ時に別途指定される既定値になります。
	/// これらは OnDeserialized 属性がついたメソッドで処理します。
	/// 
	/// ---- 重要 ----
	/// ・新しいバージョンで EnumMember, DataMember の 変数名を変更しないでください。
	///   変更しても以前の値を保持したい場合は Name プロパティ (EnumMember の場合 Value プロパティ) に前の名前を入力してください。
	///   (例 : [DataMember(Name="old_hoge")]。
	/// ・DataMember の 型を変更しないでください。
	/// ・使用しなくなった DataMember を削除しないでください。
	/// ・デシリアライズで例外が起きた場合 Config.xml はリセットされます。
	///  
	/// ---- 新しくオプションを追加する場合 ----
	/// ・変数名は原則英語としてください。
	/// ・DTXMania の新しいバージョンで DataMember を追加する場合 Order 属性値を設定してください。
	/// 　値は他の DataMember の Order 値の中でもっとも大きい値 +1 (無い場合 1 あるいは DTXMania のバージョン) としてください。
	/// 　(例: [DataMember(Order=1 / Order=105 etc.. )]public OptionInteger new_hoge;
	/// 　異なる DataMember が同じ Order 値を持っていても構いません。
	/// ・DataMember に関連する型には原則 DataContract が必要です (enum など)。
	/// 　これがないと正しくシリアライズ / デシリアライズできません。
	/// ・メンバは Deserializing メソッドでインスタンスを生成してください。
	/// ・DefaultDeserializer で説明文や制約を設定 (Initialize メソッド) してください。
	/// 　ラベルや説明文は xml には書き出されませんので、デシリアライズ後にこの処理が必要です。
	/// 　このとき Initialize しようとしている値が null でないかチェックして、
	/// 　null であれば必ず Deserializing メソッド内と同様にインスタンスを生成してください。
	/// ・CActConfigList で新しく追加した項目を list に追加してください。
	/// </summary>
	[DataContract]
	public class CConfigXml : IExtensibleDataObject, ICloneable
	{
		public static readonly int AssignableCodes = 16;

		/// <summary>
		/// ExtensibleDataObject はラウンドトリップを有効にします。
		/// 前のバージョンの DTXMania で新しい Config.xml を読み込んだ時でも
		/// デシリアライズされなかったメンバの情報は失われません。
		/// </summary>
		private ExtensionDataObject extobj;
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return extobj;
			}
			set
			{
				extobj = value;
			}
		}

		/// <summary>
		/// とりあえずshallow copyによるClone()。
		/// </summary>
		/// <returns>自身を複製したCConfigXmlクラス。</returns>
		public object Clone()
		{
			return (CConfigXml) MemberwiseClone();
		}


		[DataMember]
		public COptionBool bFullAVI;
		[DataMember]
		public COptionBool bAVI;
		[DataMember]
		public COptionBool bBGA;
		[DataMember]
		public COptionBool bBGMPlay;
		[DataMember]
		public COptionBool bLog;
		[DataMember]
		public COptionBool bLogDTX;
		[DataMember]
		public COptionBool bLogEnumerateSongs;
		[DataMember]
		public COptionBool bLogCreateRelease;
		[DataMember]
		public COptionEnum<EJudgeDisplayPriority> eJudgePriority;
		[DataMember]
		public COptionBool bScoreIni;
		[DataMember]
		public COptionBool bStageFailed;
		[DataMember]
		// #29500 2012.9.11 kairera0467 TIGHTモード
		public COptionBool bTight;
		[DataMember]
		public COptionBool bWaveAdjust;
		[DataMember]
		public COptionBool bCymbalFree;
		[DataMember]
		public COptionBool bStoicMode;
		[DataMember]
		public COptionBool bDrumsHitSound;
		[DataMember]
		public COptionBool bFillin;
		[DataMember]
		public COptionBool bRandSubBox;
		[DataMember]
		public COptionBool bDebugInfo;
		[DataMember]
		public COptionBool bAudience;
		[DataMember]
		public COptionBool bVSyncWait;
		[DataMember]
		public COptionBool bItalicFontSongSelect;
		[DataMember]
		public COptionBool bBoldFontSongSelect;
		[DataMember]
		public COptionBool bFullScreen;
		[DataMember]
		public COptionBool bViewerVSyncWait;
		[DataMember]
		public COptionBool bViewerShowDebugStatus;
		[DataMember]
		public COptionBool bViewerTimeStretch;
		[DataMember]
		public COptionBool bViewerDrumsActive;
		[DataMember]
		public COptionBool bViewerGuitarActive;
		[DataMember]
		public COptionBool bBufferedInput;
		[DataMember]
		// #25399 2011.6.9 yyagi リザルト画像自動保存機能ON/OFF
		public COptionBool bIsAutoResultCapture;
		[DataMember]
		// #28200 2012.5.1 yyagi System Menuの利用可否切替(使用可)
		public COptionBool bIsEnabledSystemMenu;
		[DataMember]
		// #28195 2012.5.6 yyagi box.defによるスキン切替機能を使用するか否か
		public COptionBool bUseBoxDefSkin;
		public bool bConfigIniが存在している;
		[DataMember]
		public COptionBool bLoadSoundSpeed;
		[DataMember]
		// #26752 2011.11.26 ダブルクリックでのフルスクリーンモード移行を許可
		public COptionBool bIsAllowedDoubleClickFullscreen;
		[DataMember]
		// #33689 2014.6.6 yyagi 演奏タイマーの種類,初期値はfalse (FDKのタイマー。ＦＲＯＭ氏考案の独自タイマー)
		public COptionBool bUseOSTimer;
		[DataMember]
		// #24820
		public COptionBool bDynamicBassMixerManagement;
		[DataMember]
		// #23664 2013.2.24 yyagi ピッチ変更無しで再生速度を変更するかどうか,初期値はfalse (再生速度変更を、ピッチ変更にて行う)
		public COptionBool bTimeStretch;
		[DataMember( Order = 105 )]
		// #34825 2016.4.27 yyagi DTXMania動作中のみ、電源プランをHighpowerに変更するかどうか
		public COptionBool bForceHighPowerPlan;
		[DataMember( Order = 105 )]
		// #36261 2016.4.27 yyagi WASAPI動作をevent drivenにするかどうか
		public COptionBool bEventDrivenWASAPI;
		[DataMember( Order = 105 )]
		// 言語設定
		public COptionString strLanguage;
	//	public COptionStringList strLanguageList;
		
		// #36177 2016.7.30 ikanick
		[DataMember( Order = 105 )]
		public COptionBool bLoadDTXDetail;

		[DataMember]
		public COptionInteger nBGAlpha;
		[DataMember]
		public Coordinates.CRect rcWindow;

		public Coordinates.CRect rcWindow_backup;			// Viewerとしての使用時は、Playerとして使用する際のウインドウ情報をバックアップしておき、終了時に復元する
															// 内部保持するだけの情報であるため、Config.xmlに保存する必要はなく、[DataMember]は不要

		[DataMember]
		public Coordinates.CRect rcViewerWindow;
		
		[DataMember]
		public COptionInteger nMasterVolume;
		[DataMember]
		public COptionInteger nSleepUnfocusMs;
		[DataMember]
		public COptionInteger nSleepPerFrameMs;
		[DataMember]
		public COptionInteger nPlaySpeed;
		[DataMember]
		public COptionInteger nPreSoundWeightMs;
		[DataMember]
		public COptionInteger nPreImageWeightMs;
		[DataMember]
		public COptionInteger nAutoVolume;
		[DataMember]
		public COptionInteger nChipVolume;
		[DataMember]
		public COptionInteger nFontSizeDotSongSelect;
		[DataMember]
		// #23559 2011.6.20 yyagi Riskyでの残ミス数。0で閉店
		public COptionInteger nRisky;
		[DataMember]
		// #32072 2013.10.24 yyagi Semi-Invisibleでの、チップ再表示期間
		public COptionInteger nChipDisplayTimeMs;
		[DataMember]
		// #32072 2013.10.24 yyagi Semi-Invisibleでの、チップフェードアウト時間
		public COptionInteger nChipFadeoutTimeMs;
		[DataMember]
		// #28228 2012.5.1 yyagi レーン毎の最大同時発音数 
		// #24820 2013.1.15 yyagi 初期値を4から2に変更。BASS.net使用時の負荷軽減のため。
		// #24820 2013.1.17 yyagi 初期値を4に戻した。動的なミキサー制御がうまく動作しているため。
		public COptionInteger nPolyphonicSounds;
		[DataMember]
		// #24820 2013.1.15 yyagi WASAPIのバッファサイズ 初期値は50(0で自動設定)
		public COptionInteger nWASAPIBufferSizeMs;


		[DataMember]
		public COptionEnum<ECYGroup> eCYGroup;
		[DataMember]
		public COptionEnum<EDark> eDark;
		[DataMember]
		public COptionEnum<EFTGroup> eFTGroup;
		[DataMember]
		public COptionEnum<EHHGroup> eHHGroup;
		[DataMember]
		// #27029 2012.1.4 from HHPedalとBassPedalのグルーピング
		public COptionEnum<EBDGroup> eBDGroup;
		[DataMember]
		public COptionEnum<EHitSoundPriority> eHitSoundPriorityCY;
		[DataMember]
		public COptionEnum<EHitSoundPriority> eHitSoundPriorityFT;
		[DataMember]
		public COptionEnum<EHitSoundPriority> eHitSoundPriorityHH;
		[DataMember]
		public COptionEnum<EActiveInstrument> eActiveInst;
		[DataMember]
		// #25370 2011.6.3 yyagi ズレ時間表示
		public COptionEnum<EShowLagType> nShowLagType;
		[DataMember]
		// #24820 2012.12.23 yyagi 初期値はACM | #31927 2013.8.25 yyagi OSにより初期値変更
		// 出力サウンドデバイス(0=ACM(にしたいが設計がきつそうならDirectShow), 1=ASIO, 2=WASAPI)
		public COptionEnum<ESoundDeviceTypeForConfig> nSoundDeviceType;
		[DataMember]
		// #27029 2012.1.5 from:
		// BDGroup が FP|BD→FP&BD に変化した際に自動変化するパラメータの値のバックアップ。FP&BD→FP|BD の時に元に戻す。
		// これらのバックアップ値は、BDGroup が FP&BD 状態の時にのみ Config.ini に出力され、BDGroup が FP|BD に戻ったら Config.ini から消える。
		public COptionEnum<EHHGroup> Backup1BDHHGroup;
		[DataMember]
		public COptionEnum<EHitSoundPriority> Backup1BDPriotiry;
		[DataMember]
		public COptionEnum<EDamage> eDamageLevel;

		[DataMember]
		public COptionString strDTXManiaVersion;
		[DataMember]
		public COptionString strSongDataPath;
		[DataMember]
		public COptionString strFontSongSelect;
		[DataMember]
		// #28195 2012.5.2 yyagi 使用中のSkinサブフォルダ名
		public COptionString strSystemSkinSubfolderPath;

		[DataMember]
		// #24820 2013.1.17 yyagi ASIOデバイス
		public COptionStringList strASIODevice;

		[DataMember( Order = 106 )]
		public COptionEnum<EClickType> eClickType;
		[DataMember( Order = 106 )]
		public COptionInteger nClickHighVolume;
		[DataMember( Order = 106 )]
		public COptionInteger nClickLowVolume;


		[DataMember]
		public STDGBSValue<COptionBool> bLight;
		[DataMember]
		public STDGBSValue<COptionBool> bReverse;
		[DataMember]
		public STDGBSValue<COptionBool> bGraph;
		[DataMember]
		public STDGBSValue<COptionBool> bEmphasizePlaySound;
		[DataMember]
		public STDGBSValue<COptionEnum<EAutoGhostData>> eAutoGhost;
		[DataMember]
		public STDGBSValue<COptionEnum<ETargetGhostData>> eTargetGhost;
		[DataMember]
		public STDGBSValue<COptionEnum<ERandom>> eRandom;
		[DataMember]
		public STDGBSValue<COptionEnum<ESudHidInv>> eSudHidInv;
		[DataMember]
		public STDGBSValue<COptionInteger> nMinComboDisp;
		[DataMember]
		public STDGBSValue<COptionInteger> nScrollSpeed;
		[DataMember]
		// #23580
		public STDGBSValue<COptionInteger> nInputAdjustTimeMs;
		[DataMember]
		// #31602
		public STDGBSValue<COptionInteger> nJudgeLinePosOffset;
		[DataMember]
		public STDGBSValue<COptionInteger> nViewerScrollSpeed;
		[DataMember]
		// #31602
		public STDGBSValue<COptionFloat> fJudgeLinePosOffsetBase;

		[DataMember]
		public COptionPadBool bAutoPlay;

		[DataMember]
		// #23857 2011.1.31 yyagi VelocityMin
		public STPadValue<COptionInteger> nVelocityMin;
		[DataMember]
		public STPadValue<COptionKeyAssign[]> KeyAssign;
		[DataMember]
		public STJudgeValue<COptionInteger> nHitRange;
		[DataMember]
		public COptionDictionary<int, string> dicJoystick;

		[DataMember]
		public STDGBSValue<COptionBool> bDisplayCombo;
		[DataMember]
		public STDGBSValue<COptionBool> bDisplayJudge;


	
		/// <summary>
		/// 楽器左端座標。ここを基準に各パーツを配置する。
		/// </summary>
		[DataMember]
		public STDGBSValue<STInstValue<COptionInteger>> cdInstX;
		[DataMember]
		public STInstValue<COptionInteger> cdDebugX;
		[DataMember]
		public STInstValue<COptionInteger> cdDebugY;
		[DataMember]
		public STInstValue<COptionInteger> cdMovieX;
		[DataMember]
		public STInstValue<COptionInteger> cdMovieY;
		[DataMember]
		public STDGBSValue<STInstValue<COptionInteger>> cdComboX;
		[DataMember]
		public STDGBSValue<STInstValue<COptionInteger>> cdComboY;
		[DataMember]
		public STDGBSValue<COptionInteger> cdJudgeLineY;
		[DataMember]
		public STDGBSValue<COptionInteger> cdJudgeY;
		[DataMember]
		public STLaneValue<COptionInteger> cdLaneOrder;
		[DataMember]
		public STDGBSValue<COptionInteger> nSuddenFrom;
		[DataMember]
		public STDGBSValue<COptionInteger> nHiddenFrom;


		public int GetLaneX(ELane e)
		{
			EPart p = EPart.Drums;
			ELane start = ELane.DrumsLaneMin;
			ELane end = ELane.DrumsLaneMax;
			if (e >= ELane.GuitarLaneMin && e < ELane.GuitarLaneMax)
			{
				p = EPart.Guitar;
				start = ELane.GuitarLaneMin;
				end = ELane.GuitarLaneMax;
			}
			if (e >= ELane.BassLaneMin && e < ELane.BassLaneMax)
			{
				p = EPart.Bass;
				start = ELane.BassLaneMin;
				end = ELane.BassLaneMax;
			}
			int ret = cdInstX[p][eActiveInst];
			int order = cdLaneOrder[e];
			for (ELane t = start; t < end; ++t)
			{
				if (cdLaneOrder[t] < order)
				{
					ret += GetLaneW(t);
				}
			}
			return ret;
		}

		public int GetLaneW(ELane e)
		{
			switch (e)
			{
				case ELane.LC:
				case ELane.CY:
					return 85;
				case ELane.BD:
					return 99;
			}
			return 72;
		}

		public void AdjustLaneOrders(EPart p)
		{
			ELane min = ELane.DrumsLaneMin;
			ELane max = ELane.DrumsLaneMax;
			if (p == EPart.Guitar)
			{
				min = ELane.GuitarLaneMin;
				max = ELane.GuitarLaneMax;
			}
			else if (p == EPart.Bass)
			{
				min = ELane.BassLaneMin;
				max = ELane.BassLaneMax;
			}

			bool[] used = new bool[max - min];
			for (ELane t = min; t < max; ++t)
			{
				used[cdLaneOrder[t]] = true;
			}
			for (int i = 0; i < used.Length; ++i)
			{
				if (!used[i])
				{
					for (ELane t = min; t < max; ++t)
					{
						for (ELane t2 = t + 1; t2 < max; ++t2)
						{
							if (cdLaneOrder[t].Value == cdLaneOrder[t2].Value)
							{
								cdLaneOrder[t].Value = (int)(min + i);
								used[i] = true;
								break;
							}
						}
						if (used[i])
						{
							break;
						}
					}
				}
			}
		}

		[OnDeserializing]
		public void Deserializing(StreamingContext sc)
		{
			// #xxxxx 2011.11.27 yyagi add
			// #23568 2010.11.04 ikanick add
			// #34069 2014.7.23 yyagi 初回起動時のwindow sizeは、CDTXMania側で設定する(-> 1280x720にする)
			// #30675 2013.02.04 ikanick add
			// #23580 2011.1.3 yyagi
			// #31602 2013.6.23 yyagi
			//this.strDTXManiaのバージョン = "Unknown";

			// bool
			bFullScreen = new COptionBool(false);
			bVSyncWait = new COptionBool(true);
			bStageFailed = new COptionBool(true);
			bFullAVI = new COptionBool(false);
			bAVI = new COptionBool(true);
			bBGA = new COptionBool(true);
			bFillin = new COptionBool(true);
			bLogDTX = new COptionBool(false);
			bLogEnumerateSongs = new COptionBool(false);
			bLogCreateRelease = new COptionBool(false);
			bCymbalFree = new COptionBool(false);
			bStoicMode = new COptionBool(false);
			bItalicFontSongSelect = new COptionBool(false);
			bWaveAdjust = new COptionBool(true);
			bBGMPlay = new COptionBool(true);
			bDrumsHitSound = new COptionBool(true);
			bAudience = new COptionBool(true);
			bScoreIni = new COptionBool(true);
			bRandSubBox = new COptionBool(true);
			bBoldFontSongSelect = new COptionBool(true);
			bDebugInfo = new COptionBool(false);
			bLog = new COptionBool(true);
			bAutoPlay = new COptionPadBool();
			for ( EPad pad = EPad.DrumsPadMin; pad < EPad.DrumsPadMax; ++pad )
			{
				bAutoPlay[ pad ] = new COptionBool( false );
			}
			for ( EPad pad = EPad.GuitarPadMin; pad < EPad.BassPadMax; ++pad )
			{
				bAutoPlay[ pad ] = new COptionBool( true );
			}
			bViewerVSyncWait = new COptionBool( true );
			bViewerShowDebugStatus = new COptionBool(true);
			bViewerTimeStretch = new COptionBool(false);
			bViewerDrumsActive = new COptionBool(true);
			bViewerGuitarActive = new COptionBool(true);
			bLoadSoundSpeed = new COptionBool(true);
			bIsAutoResultCapture = new COptionBool(false);
			bBufferedInput = new COptionBool(true);
			bIsAllowedDoubleClickFullscreen = new COptionBool(true);
			bIsEnabledSystemMenu = new COptionBool(true);
			bUseBoxDefSkin = new COptionBool(true);
			bTight = new COptionBool(false);
			bUseOSTimer = new COptionBool(false);
			bDynamicBassMixerManagement = new COptionBool(true);
			bTimeStretch = new COptionBool(false);
			nSoundDeviceType = new COptionEnum<ESoundDeviceTypeForConfig>(FDK.COS.bIsVistaOrLater ? ESoundDeviceTypeForConfig.WASAPI : ESoundDeviceTypeForConfig.DSound);
			bForceHighPowerPlan = new COptionBool( false );
			bEventDrivenWASAPI = new COptionBool( false );
			bLoadDTXDetail = new COptionBool( false ); 

			// string
			strSongDataPath = new COptionString(@".\");
			strFontSongSelect = new COptionString("MS PGothic");
			strDTXManiaVersion = new COptionString(CDTXMania.VERSION);
			strSystemSkinSubfolderPath = new COptionString(@".\Default\");

			// enum
			eDamageLevel = new COptionEnum<EDamage>(EDamage.Normal);
			eJudgePriority = new COptionEnum<EJudgeDisplayPriority>(EJudgeDisplayPriority.Under);
			eCYGroup = new COptionEnum<ECYGroup>(ECYGroup.None);
			eDark = new COptionEnum<EDark>(EDark.Off);
			eFTGroup = new COptionEnum<EFTGroup>(EFTGroup.None);
			eHHGroup = new COptionEnum<EHHGroup>(EHHGroup.None);
			eBDGroup = new COptionEnum<EBDGroup>(EBDGroup.None);
			Backup1BDHHGroup = new COptionEnum<EHHGroup>(EHHGroup.None);
			Backup1BDPriotiry = new COptionEnum<EHitSoundPriority>(EHitSoundPriority.Chip);
			eHitSoundPriorityCY = new COptionEnum<EHitSoundPriority>(EHitSoundPriority.Chip);
			eHitSoundPriorityFT = new COptionEnum<EHitSoundPriority>(EHitSoundPriority.Chip);
			eHitSoundPriorityHH = new COptionEnum<EHitSoundPriority>(EHitSoundPriority.Chip);
			eJudgePriority = new COptionEnum<EJudgeDisplayPriority>(EJudgeDisplayPriority.Under);
			eActiveInst = new COptionEnum<EActiveInstrument>(EActiveInstrument.Both);
			nShowLagType = new COptionEnum<EShowLagType>(EShowLagType.Off);

			// integer
			rcWindow = new Coordinates.CRect(100, 100, 1280, 720);
			rcWindow_backup = new Coordinates.CRect( 100, 100, 1280, 720 );
			nSleepPerFrameMs = new COptionInteger( -1 );
			nSleepUnfocusMs = new COptionInteger(1);
			nBGAlpha = new COptionInteger(100);
			nPreSoundWeightMs = new COptionInteger(1000);
			nPreImageWeightMs = new COptionInteger(100);
			nFontSizeDotSongSelect = new COptionInteger(20);
			nAutoVolume = new COptionInteger(80);
			nChipVolume = new COptionInteger(100);
			nPlaySpeed = new COptionInteger(20);

			nHitRange = new STJudgeValue<COptionInteger>();
			nHitRange.Perfect = new COptionInteger(34);
			nHitRange.Great = new COptionInteger(67);
			nHitRange.Good = new COptionInteger(84);
			nHitRange.Poor = new COptionInteger(117);

			nVelocityMin = new STPadValue<COptionInteger>();
			for (EPad pad = EPad.Min; pad < EPad.Max; ++pad)
			{
				nVelocityMin[pad] = new COptionInteger(0);
			}
			nVelocityMin.HH.Value = 20;

			nRisky = new COptionInteger(0);
			nWASAPIBufferSizeMs = new COptionInteger(50);
			nChipDisplayTimeMs = new COptionInteger(3000);
			nChipFadeoutTimeMs = new COptionInteger(2000);
			rcViewerWindow = new Coordinates.CRect(100, 100, 640, 360);
			nMasterVolume = new COptionInteger(100);
			nPolyphonicSounds = new COptionInteger(4);

			eClickType = new COptionEnum<EClickType>(EClickType.Off);
			nClickHighVolume = new COptionInteger(100);
			nClickLowVolume = new COptionInteger(100);

			// dgb
			bEmphasizePlaySound = new STDGBSValue<COptionBool>();
			bReverse = new STDGBSValue<COptionBool>();
			bLight = new STDGBSValue<COptionBool>();
			bGraph = new STDGBSValue<COptionBool>();
			bDisplayCombo = new STDGBSValue<COptionBool>();
			bDisplayJudge = new STDGBSValue<COptionBool>();

			nScrollSpeed = new STDGBSValue<COptionInteger>();
			nMinComboDisp = new STDGBSValue<COptionInteger>();
			nInputAdjustTimeMs = new STDGBSValue<COptionInteger>();
			nJudgeLinePosOffset = new STDGBSValue<COptionInteger>();
			nSuddenFrom = new STDGBSValue<COptionInteger>();
			nHiddenFrom = new STDGBSValue<COptionInteger>();

			eRandom = new STDGBSValue<COptionEnum<ERandom>>();
			eAutoGhost = new STDGBSValue<COptionEnum<EAutoGhostData>>();
			eTargetGhost = new STDGBSValue<COptionEnum<ETargetGhostData>>();
			eSudHidInv = new STDGBSValue<COptionEnum<ESudHidInv>>();


			for (EPart i = EPart.Drums; i <= EPart.Unknown; i++)
			{
				bEmphasizePlaySound[i] = new COptionBool(true);
				bReverse[i] = new COptionBool(false);
				eRandom[i] = new COptionEnum<ERandom>(ERandom.Off);
				bLight[i] = new COptionBool(false);
				bGraph[i] = new COptionBool(false);
				bDisplayJudge[i] = new COptionBool(true);
				bDisplayCombo[i] = new COptionBool(true);

				nScrollSpeed[i] = new COptionInteger(1);
				nInputAdjustTimeMs[i] = new COptionInteger(0);
				nJudgeLinePosOffset[i] = new COptionInteger(0);
				nViewerScrollSpeed[i] = new COptionInteger(1);
				nMinComboDisp[i] = new COptionInteger(2);
				nSuddenFrom[i] = new COptionInteger(450); // +100 FI
				nHiddenFrom[i] = new COptionInteger(225); // +100 FO

				eAutoGhost[i] = new COptionEnum<EAutoGhostData>(EAutoGhostData.Perfect);
				eTargetGhost[i] = new COptionEnum<ETargetGhostData>(ETargetGhostData.None);
				eSudHidInv[i] = new COptionEnum<ESudHidInv>(ESudHidInv.Off);

				fJudgeLinePosOffsetBase[i] = new COptionFloat(0f);
			}

			dicJoystick = new COptionDictionary<int, string>();
			dicJoystick.Value = new Dictionary<int, string>(10);

			string[] asiodevs = CEnumerateAllAsioDevices.GetAllASIODevices();
			strASIODevice = new COptionStringList(asiodevs.Length > 0 ? asiodevs[0] : null);

			strLanguage = new COptionString( "" );		// "default"でなく"" にすること。そうすることで、Locale情報を使った初期化がなされる。
		//	strLanguageList = new COptionStringList("");


			cdInstX = new STDGBSValue<STInstValue<COptionInteger>>();
			cdInstX.Drums = new STInstValue<COptionInteger>();
			cdInstX.Drums.DrOnly = new COptionInteger(SampleFramework.GameWindowSize.Width / 2 - 314);
			cdInstX.Guitar = new STInstValue<COptionInteger>();
			cdInstX.Guitar.GBOnly = new COptionInteger(480 - 72 * 4);
			cdInstX.Bass = new STInstValue<COptionInteger>();
			cdInstX.Bass.GBOnly = new COptionInteger(SampleFramework.GameWindowSize.Width - 480);
			cdInstX.Drums.Both = new COptionInteger(cdInstX.Drums.DrOnly);
			cdInstX.Guitar.Both = new COptionInteger(cdInstX.Guitar.GBOnly);
			cdInstX.Bass.Both = new COptionInteger(cdInstX.Bass.GBOnly);

			cdDebugX = new STInstValue<COptionInteger>();
			cdDebugX.DrOnly = new COptionInteger(450 * 3);
			cdDebugX.Both = new COptionInteger(450 * 3);
			cdDebugX.GBOnly = new COptionInteger(cdInstX.Drums.DrOnly);

			cdDebugY = new STInstValue<COptionInteger>();
			cdDebugY.Both = new COptionInteger(200);
			cdDebugY.DrOnly = new COptionInteger(200);
			cdDebugY.GBOnly = new COptionInteger(200);

			cdMovieX = new STInstValue<COptionInteger>();
			cdMovieX.Both = new COptionInteger(619 + 682);
			cdMovieX.DrOnly = new COptionInteger(619 + 682);
			cdMovieX.GBOnly = new COptionInteger(682);

			cdMovieY = new STInstValue<COptionInteger>();
			cdMovieY.Both = new COptionInteger(128);
			cdMovieY.DrOnly = new COptionInteger(128);
			cdMovieY.GBOnly = new COptionInteger(128);

			cdComboX = new STDGBSValue<STInstValue<COptionInteger>>();
			cdComboX.Drums = new STInstValue<COptionInteger>();
			cdComboX.Drums.Both = new COptionInteger(cdInstX.Drums.Both + (72 * 5 + 85 * 2 + 99) / 2);
			cdComboX.Drums.DrOnly = new COptionInteger(cdInstX.Drums.DrOnly + (72 * 5 + 85 * 2 + 99) / 2);
			cdComboX.Guitar = new STInstValue<COptionInteger>();
			cdComboX.Guitar.Both = new COptionInteger(cdInstX.Guitar.Both + 72 * 2);
			cdComboX.Guitar.GBOnly = new COptionInteger(cdInstX.Guitar.GBOnly + 72 * 2);
			cdComboX.Bass = new STInstValue<COptionInteger>();
			cdComboX.Bass.Both = new COptionInteger(cdInstX.Bass.Both + 72 * 2);
			cdComboX.Bass.GBOnly = new COptionInteger(cdInstX.Bass.GBOnly + 72 * 2);

			cdComboY = new STDGBSValue<STInstValue<COptionInteger>>();
			cdComboY.Drums = new STInstValue<COptionInteger>();
			cdComboY.Drums.Both = new COptionInteger(100);
			cdComboY.Drums.DrOnly = new COptionInteger(100);
			cdComboY.Guitar = new STInstValue<COptionInteger>();
			cdComboY.Guitar.Both = new COptionInteger(700);
			cdComboY.Guitar.GBOnly = new COptionInteger(700);
			cdComboY.Bass = new STInstValue<COptionInteger>();
			cdComboY.Bass.Both = new COptionInteger(700);
			cdComboY.Bass.GBOnly = new COptionInteger(700);

			cdJudgeLineY = new STDGBSValue<COptionInteger>();
			cdJudgeLineY.Drums = new COptionInteger(942);
			cdJudgeLineY.Guitar = new COptionInteger(138);
			cdJudgeLineY.Bass = new COptionInteger(138);

			cdJudgeY = new STDGBSValue<COptionInteger>();
			cdJudgeY.Drums = new COptionInteger(350);
			cdJudgeY.Guitar = new COptionInteger(600);
			cdJudgeY.Bass = new COptionInteger(600);

			cdLaneOrder = new STLaneValue<COptionInteger>();
			for (ELane lane = ELane.DrumsLaneMin; lane < ELane.BassLaneMax; ++lane)
			{
				cdLaneOrder[lane] = new COptionInteger(0);
			}

			cdLaneOrder.HH.Value = 1;
			cdLaneOrder.SD.Value = 2;
			cdLaneOrder.BD.Value = 3;
			cdLaneOrder.HT.Value = 4;
			cdLaneOrder.LT.Value = 5;
			cdLaneOrder.FT.Value = 6;
			cdLaneOrder.CY.Value = 7;

			cdLaneOrder.GtR.Value = 0;
			cdLaneOrder.GtG.Value = 1;
			cdLaneOrder.GtB.Value = 2;
			cdLaneOrder.GtW.Value = 3;

			cdLaneOrder.BsR.Value = 0;
			cdLaneOrder.BsG.Value = 1;
			cdLaneOrder.GtB.Value = 2;
			cdLaneOrder.BsW.Value = 3;

			bConfigIniが存在している = System.IO.File.Exists(CDTXMania.Instance.strEXEのあるフォルダ + "Config.xml");
			SetDefaultKeyAssign();
		}

		/// <summary>
		/// 保存する必要がない値はここで設定してください。
		/// 設定しない場合 DataMember 以外のメンバはすべてその型の規定値になります。
		/// </summary>
		/// <param name="sc">ストリーミングコンテキスト。使用しません。</param>
		[OnDeserialized]
		public void DefaultDeserializer(StreamingContext sc)
		{
			// ラベル・説明文・上下限値
			CResources cr = CDTXMania.Instance.Resources;
			bFullScreen.Initialize( "strCfgSysFullScreen" );
			bVSyncWait.Initialize( "strCfgSysVSync" );
			bStageFailed.Initialize( "strCfgSysStageFailed");
			bFullAVI.Initialize( "strCfgSysFullAVI" );
			bAVI.Initialize( "strCfgSysAVI" );
			bBGA.Initialize( "strCfgSysBGA" );
			bLog.Initialize( "strCfgSysLog" );
			bStoicMode.Initialize( "strCfgSysStoic" );
			bWaveAdjust.Initialize( "strCfgSysAdjustWaves" );
			bBGMPlay.Initialize( "strCfgSysBGM" );
			bAudience.Initialize( "strCfgSysAudience" );
			bScoreIni.Initialize( "strCfgSysSaveScoreIni" );
			bRandSubBox.Initialize( "strCfgSysRandSubBox" );
			bAutoPlay.HH.Initialize( "strCfgDrAutoHHC" );
			bAutoPlay.HHO.Initialize( "strCfgDrAutoHHO" );
			bAutoPlay.SD.Initialize( "strCfgDrAutoSD" );
			bAutoPlay.BD.Initialize( "strCfgDrAutoBD" );
			bAutoPlay.HT.Initialize( "strCfgDrAutoHT" );
			bAutoPlay.LT.Initialize( "strCfgDrAutoLT" );
			bAutoPlay.FT.Initialize( "strCfgDrAutoFT" );
			bAutoPlay.CY.Initialize( "strCfgDrAutoCY" );
			bAutoPlay.RD.Initialize( "strCfgDrAutoRD" );
			bAutoPlay.LC.Initialize( "strCfgDrAutoLCY" );
			bAutoPlay.GtR.Initialize( "strCfgGtAutoR" );
			bAutoPlay.GtG.Initialize( "strCfgGtAutoG" );
			bAutoPlay.GtB.Initialize( "strCfgGtAutoB" );
			bAutoPlay.GtPick.Initialize( "strCfgGtAutoPick" );
			bAutoPlay.GtWail.Initialize( "strCfgGtAutoWailing" );
			bAutoPlay.BsR.Initialize( "strCfgBsAutoR" );
			bAutoPlay.BsG.Initialize( "strCfgBsAutoG" );
			bAutoPlay.BsB.Initialize( "strCfgBsAutoB" );
			bAutoPlay.BsPick.Initialize( "strCfgBsAutoPick" );
			bAutoPlay.BsWail.Initialize( "strCfgBsAutoWailing" );

			bLoadSoundSpeed.Initialize( "strCfgSysSoundLoadLimiter" );
			bIsAutoResultCapture.Initialize( "strCfgSysAutoResultCapture" );
			bBufferedInput.Initialize( "strCfgSysBufferingInput" );
			bUseBoxDefSkin.Initialize( "strCfgSysUseBoxDefSkin" );
			bUseOSTimer.Initialize( "strCfgSysUseOSTimer" );
			bTimeStretch.Initialize( "strCfgSysTimeStretch" );
			bForceHighPowerPlan.Initialize( "strCfgSysForceHighPowerPlan" );
			bEventDrivenWASAPI.Initialize( "strCfgSysWASAPIEventDriven" );
			bLoadDTXDetail.Initialize( "strCfgSysLoadDTXDetail" ); // #36177 2016.7.30 ikanick

			bCymbalFree.Initialize( "strCfgDrCymbalFree" );
			bDrumsHitSound.Initialize( "strCfgDrChipSound" );
			bFillin.Initialize( "strCfgDrFillin" );
			bTight.Initialize( "strCfgDrTight" );

			bItalicFontSongSelect.Initialize("", "");
			bBoldFontSongSelect.Initialize("", "");
			bLogDTX.Initialize("", "");
			bLogEnumerateSongs.Initialize("", "");
			bLogCreateRelease.Initialize("", "");
			bViewerVSyncWait.Initialize("", "");
			bViewerShowDebugStatus.Initialize("", "");
			bViewerTimeStretch.Initialize("", "");
			bViewerDrumsActive.Initialize("", "");
			bViewerGuitarActive.Initialize("", "");
			bIsAllowedDoubleClickFullscreen.Initialize("", "");
			bIsEnabledSystemMenu.Initialize("", "");
			bDynamicBassMixerManagement.Initialize("", "");
			bDebugInfo.Initialize( "strCfgSysDebugInfo" );

			// enum
			nSoundDeviceType.Initialize( "strCfgSysSoundDeviceType", typeof(ESoundDeviceTypeForConfig));
			eDamageLevel.Initialize( "strCfgSysDamageLevel", typeof(EDamage));
			eJudgePriority.Initialize( "strCfgSysJudgePriority", typeof(EJudgeDisplayPriority));
			eCYGroup.Initialize( "strCfgDrCYGroup", typeof(ECYGroup));
			eDark.Initialize( "strCfgSysDark", typeof(EDark));
			eFTGroup.Initialize( "strCfgDrFTGroup", typeof(EFTGroup));
			eHHGroup.Initialize( "strCfgDrHHGroup", typeof(EHHGroup));
			eBDGroup.Initialize( "strCfgDrBDGroup", typeof(EBDGroup));
			Backup1BDHHGroup.Initialize("", "", typeof(EHHGroup));
			Backup1BDPriotiry.Initialize("", "", typeof(EHitSoundPriority));
			eHitSoundPriorityCY.Initialize( "strCfgDrCYPriority", typeof(EHitSoundPriority));
			eHitSoundPriorityFT.Initialize( "strCfgDrFTPriority", typeof(EHitSoundPriority));
			eHitSoundPriorityHH.Initialize( "strCfgDrHHPriority", typeof(EHitSoundPriority));
//			eJudgePriority.Initialize("Judge Priority", Properties.Resources.strCfgSysJudgePriority, typeof(EJudgeDisplayPriority));
			eActiveInst.Initialize( "strCfgSysPlayMode", typeof(EActiveInstrument));
			nShowLagType.Initialize( "strCfgSysShowLagType", typeof(EShowLagType));

			eClickType.Initialize( "strCfgSysClickType", typeof( EClickType ) );
			nClickHighVolume.Initialize( "strCfgSysClickHighVolume", 0, 101 );
			nClickLowVolume.Initialize( "strCfgSysClickLowVolume", 0, 101 );


			// integer
			nSleepPerFrameMs.Initialize( "strCfgSysSleepPerFrame", -1, 33 );
			nSleepUnfocusMs.Initialize( "strCfgSysSleepUnfocus", 0, 33 );
			nBGAlpha.Initialize( "strCfgSysBGAAlpha", 0, 256 );
			nPreSoundWeightMs.Initialize( "strCfgSysPreSoundWait", 0, 10001);
			nPreImageWeightMs.Initialize( "strCfgSysPreImageWait", 0, 10001);
			nFontSizeDotSongSelect.Initialize("", "", 5);
			nAutoVolume.Initialize( "strCfgSysAutoVolume", 0, 101);
			nChipVolume.Initialize( "strCfgSysChipVolume", 0, 101);
			nPlaySpeed.Initialize( "strCfgSysPlaySpeed", 5, 41);
			nPlaySpeed.ValueFormatter = (x) =>
			{
				return "x" + (x / 20f).ToString("0.000");
			};
			nHitRange.Perfect.Initialize( "strCfgRangePerfect", 0 );
			nHitRange.Great.Initialize( "strCfgRangeGreat", 0 );
			nHitRange.Good.Initialize( "strCfgRangeGood", 0 );
			nHitRange.Poor.Initialize( "strCfgRangePoor", 0 );
			nVelocityMin.LC.Initialize( "strCfgDrLCVelocityMin", 0, 128 );
			nVelocityMin.HH.Initialize( "strCfgDrHHVelocityMin", 0, 128 );
			nVelocityMin.SD.Initialize( "strCfgDrSDVelocityMin", 0, 128 );
			nVelocityMin.BD.Initialize( "strCfgDrBDVelocityMin", 0, 128 );
			nVelocityMin.HT.Initialize( "strCfgDrHTVelocityMin", 0, 128 );
			nVelocityMin.LT.Initialize( "strCfgDrLTVelocityMin", 0, 128 );
			nVelocityMin.FT.Initialize( "strCfgDrFTVelocityMin", 0, 128 );
			nVelocityMin.CY.Initialize( "strCfgDrCYVelocityMin", 0, 128 );
			nVelocityMin.RD.Initialize( "strCfgDrRDVelocityMin", 0, 128 );
			nRisky.Initialize( "strCfgSysRisky", 0, 100 );
			nWASAPIBufferSizeMs.Initialize( "strCfgSysWASAPIBufSize", 0, 100001);
			nChipDisplayTimeMs.Initialize("", "", 0);
			nChipFadeoutTimeMs.Initialize("", "", 0);

			nMasterVolume.Initialize( "strCfgSysMasterVolume", 0, 101);
			nPolyphonicSounds.Initialize( "strCfgSysPolyphonicSounds", 1, 11 );

			// dgb
			for (EPart i = EPart.Drums; i <= EPart.Unknown; i++)
			{
				bEmphasizePlaySound[i].Initialize( "strCfgDgbChipSoundMonitor" );
				bReverse[i].Initialize( "strCfgDgbReverse" );
				eRandom[i].Initialize( "strCfgDgbRandom", typeof(ERandom) );
				bLight[i].Initialize( "strCfgDgbLight" );
				bDisplayCombo[i].Initialize( "strCfgDgbDisplayCombo" );
				bDisplayJudge[i].Initialize( "strCfgDgbDisplayJudge" );
				bGraph[i].Initialize( "strCfgDgbDisplayGraph" );
				nScrollSpeed[i].Initialize( "strCfgDgbScrollSpeed", 1, 101);
				nScrollSpeed[i].ValueFormatter = (x) =>
					{
						return "x" + (x * 0.5f).ToString("0.0");
					};
				nInputAdjustTimeMs[i].Initialize( "strCfgDgbInputAdjust", -99, 100);
				nJudgeLinePosOffset[i].Initialize("", "", -SampleFramework.GameWindowSize.Height, SampleFramework.GameWindowSize.Height + 1);
				nViewerScrollSpeed[i].Initialize("", "", 1, 101);
				nMinComboDisp[i].Initialize( "strCfgDgbMinComboDisp", 2, 100001);
				nSuddenFrom[i].Initialize( "strCfgDgbSuddenFrom", 0, SampleFramework.GameWindowSize.Height + 1);
				nHiddenFrom[i].Initialize( "strCfgDgbHiddenFrom", 0, SampleFramework.GameWindowSize.Height + 1);
				eAutoGhost[i].Initialize( "strCfgDgbAutoGhost", typeof(EAutoGhostData));
				eTargetGhost[i].Initialize( "strCfgDgbTargetGhost", typeof(ETargetGhostData));
				eSudHidInv[i].Initialize( "strCfgDgbSudHidInv", typeof(ESudHidInv));

				fJudgeLinePosOffsetBase[i].Initialize("", "");
			}

			string[] asiodevs = CEnumerateAllAsioDevices.GetAllASIODevices();
			strASIODevice.Initialize( "strCfgSysASIODevice", asiodevs);

			int crdStep = 10;
			cdInstX.Drums.Both.Initialize("strCfgDispDrumsXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdInstX.Drums.DrOnly.Initialize("strCfgDispDrumsXDr", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdInstX.Guitar.Both.Initialize("strCfgDispGuitarXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdInstX.Guitar.GBOnly.Initialize("strCfgDispGuitarXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdInstX.Bass.Both.Initialize("strCfgDispBassXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdInstX.Bass.GBOnly.Initialize("strCfgDispBassXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);

			cdDebugX.DrOnly.Initialize("strCfgDispDebugXDr", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdDebugX.Both.Initialize("strCfgDispDebugXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdDebugX.GBOnly.Initialize("strCfgDispDebugXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdDebugY.DrOnly.Initialize("strCfgDispDebugYDr", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdDebugY.Both.Initialize("strCfgDispDebugYBoth", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdDebugY.GBOnly.Initialize("strCfgDispDebugYGB", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);

			cdMovieX.DrOnly.Initialize("strCfgDispMovieXDr", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdMovieX.Both.Initialize("strCfgDispMovieXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdMovieX.GBOnly.Initialize("strCfgDispMovieXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdMovieY.DrOnly.Initialize("strCfgDispMovieYDr", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdMovieY.Both.Initialize("strCfgDispMovieYBoth", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdMovieY.GBOnly.Initialize("strCfgDispMovieYGB", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);

			cdComboX.Drums.Both.Initialize("strCfgDispDrComboXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdComboX.Drums.DrOnly.Initialize("strCfgDispDrComboXDr", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdComboX.Guitar.Both.Initialize("strCfgDispGtComboXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdComboX.Guitar.GBOnly.Initialize("strCfgDispGtComboXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdComboX.Bass.Both.Initialize("strCfgDispBsComboXBoth", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);
			cdComboX.Bass.GBOnly.Initialize("strCfgDispBsComboXGB", 0, 1 + SampleFramework.GameWindowSize.Width, crdStep);

			cdComboY.Drums.Both.Initialize("strCfgDispDrComboYBoth", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdComboY.Drums.DrOnly.Initialize("strCfgDispDrComboYDr", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdComboY.Guitar.Both.Initialize("strCfgDispGtComboYBoth", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdComboY.Guitar.GBOnly.Initialize("strCfgDispGtComboYGB", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdComboY.Bass.Both.Initialize("strCfgDispBsComboYBoth", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdComboY.Bass.GBOnly.Initialize("strCfgDispBsComboYGB", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);

			cdJudgeLineY.Drums.Initialize("strCfgDispDrJudgeLine", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdJudgeLineY.Guitar.Initialize("strCfgDispGtJudgeLine", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdJudgeLineY.Bass.Initialize("strCfgDispBsJudgeLine", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);

			cdJudgeY.Drums.Initialize("strCfgDispDrJudge", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdJudgeY.Guitar.Initialize("strCfgDispGtJudge", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);
			cdJudgeY.Bass.Initialize("strCfgDispBsJudge", 0, 1 + SampleFramework.GameWindowSize.Height, crdStep);

			cdLaneOrder.LC.Initialize("strCfgDispLaneOrderLC", 0, 8);
			cdLaneOrder.HH.Initialize("strCfgDispLaneOrderHH", 0, 8);
			cdLaneOrder.SD.Initialize("strCfgDispLaneOrderSD", 0, 8);
			cdLaneOrder.BD.Initialize("strCfgDispLaneOrderBD", 0, 8);
			cdLaneOrder.HT.Initialize("strCfgDispLaneOrderHT", 0, 8);
			cdLaneOrder.LT.Initialize("strCfgDispLaneOrderLT", 0, 8);
			cdLaneOrder.FT.Initialize("strCfgDispLaneOrderFT", 0, 8);
			cdLaneOrder.CY.Initialize("strCfgDispLaneOrderCY", 0, 8);

			cdLaneOrder.GtR.Initialize("strCfgDispLaneOrderGtR", 0, 4);
			cdLaneOrder.GtG.Initialize("strCfgDispLaneOrderGtG", 0, 4);
			cdLaneOrder.GtB.Initialize("strCfgDispLaneOrderGtB", 0, 4);
			cdLaneOrder.GtW.Initialize("strCfgDispLaneOrderGtW", 0, 4);

			cdLaneOrder.BsR.Initialize("strCfgDispLaneOrderBsR", 0, 4);
			cdLaneOrder.BsG.Initialize("strCfgDispLaneOrderBsG", 0, 4);
			cdLaneOrder.BsB.Initialize("strCfgDispLaneOrderBsB", 0, 4);
			cdLaneOrder.BsW.Initialize("strCfgDispLaneOrderBsW", 0, 4);

			SetEnterDelegates();
		}

		public CConfigXml()
		{
			Deserializing(new StreamingContext());
			DefaultDeserializer(new StreamingContext());
		}

		public void SetEnterDelegates()
		{
			bFullScreen.OnEnterDelegate = () =>
			{
				CDTXMania.Instance.b次のタイミングで全画面_ウィンドウ切り替えを行う = true;
			};

			// Vsync
			bVSyncWait.OnEnterDelegate = () =>
			{
				CDTXMania.Instance.b次のタイミングで垂直帰線同期切り替えを行う = true;
			};

			eBDGroup.OnEnterDelegate = () =>
			{
				//BD group
				if (eBDGroup == EBDGroup.Group)
				{
					// #27029 2012.1.5 from: 変更前の状態をバックアップする。
					Backup1BDHHGroup.Value = eHHGroup;
					Backup1BDPriotiry.Value = eHitSoundPriorityHH;

					// HH Group ... HH-0 → HH-2 / HH-1 → HH-3 / HH-2 → 変更なし / HH-3 → 変更なし
					if (eHHGroup == EHHGroup.None)
					{
						eHHGroup.Value = EHHGroup.LC_HH;
					}
					if (eHHGroup == EHHGroup.HO_HC)
					{
						eHHGroup.Value = EHHGroup.Group;
					}

					// HH Priority ... C>P → 変更なし / P>C → C>P
					if (eHitSoundPriorityHH == EHitSoundPriority.Pad)
					{
						eHitSoundPriorityHH.Value = EHitSoundPriority.Chip;
					}
				}
				else
				{
					eHHGroup = Backup1BDHHGroup;
					eHitSoundPriorityHH = Backup1BDPriotiry;
				}
			};

			bUseBoxDefSkin.OnEnterDelegate = () =>
			{
				CSkin.bUseBoxDefSkin = bUseBoxDefSkin;
			};

			bBufferedInput.OnEnterDelegate = () =>
			{
				// #36433 2016.7.18 yyagi
				// BufferedInputのON/OFFを切り替えると、OFFからONへの切り替えの際に、Enterが再入されてしまい
				// OFFに戻ってしまう(つまり切り替えができない)問題への対策。
				// BufferedInputの切り替え時に、入力バッファをクリアする処理を追加する。
				while ( 
						( CDTXMania.Instance.Pad.bDecidePadIsPressedDGB() ||
							( CDTXMania.Instance.ConfigIni.bEnterがキー割り当てのどこにも使用されていない &&
							  CDTXMania.Instance.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Return )
							)
						)
					  )
				{
					Thread.Sleep( 50 );
					CDTXMania.Instance.Input管理.tポーリング(
						CDTXMania.Instance.bApplicationActive, CDTXMania.Instance.ConfigIni.bBufferedInput );
				}
			};

		}


		public bool bConfigIniがないかDTXManiaのバージョンが異なる
		{
			get
			{
				return (!bConfigIniが存在している || !CDTXMania.VERSION.Equals(strDTXManiaVersion));
			}
		}

		public bool bEnterがキー割り当てのどこにも使用されていない
		{
			get
			{
				for (EPad j = EPad.Min; j < EPad.Max; j++)
				{
					for (int k = 0; k < AssignableCodes; k++)
					{
						if ((KeyAssign[j][k].入力デバイス == EInputDevice.Keyboard) &&
								(KeyAssign[j][k].コード == (int)SlimDX.DirectInput.Key.Return))
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool bウィンドウモード
		{
			get
			{
				return !this.bFullScreen;
			}
			set
			{
				this.bFullScreen.Value = !value;
			}
		}

		public bool bGuitar有効
		{
			get
			{
				return eActiveInst == EActiveInstrument.GBOnly || eActiveInst == EActiveInstrument.Both;
			}
		}

		public bool bDrums有効
		{
			get
			{
				return eActiveInst == EActiveInstrument.DrOnly || eActiveInst == EActiveInstrument.Both;
			}
		}

		public bool b楽器有効(EPart inst)
		{
			if (inst == EPart.Drums)
			{
				return bDrums有効;
			}
			else if (inst == EPart.Guitar || inst == EPart.Bass)
			{
				return bGuitar有効;
			}
			return false;
		}

		public bool bIsAutoPlay(EPart inst)
		{
			bool ret = false;
			if (inst == EPart.Drums)
			{
				ret = bAutoPlay.IsAllTrue(EPart.Drums);
			}
			else if (inst == EPart.Guitar)
			{
				ret = bAutoPlay.IsAllTrue(EPart.Guitar);
			}
			else if (inst == EPart.Bass)
			{
				ret = bAutoPlay.IsAllTrue(EPart.Bass);
			}
			return ret;
		}

		public bool b演奏情報を表示する
		{
			get
			{
				return this.bDebugInfo;
			}
			set
			{
				this.bDebugInfo.Value = value;
			}
		}

		// #24063 2011.1.16 yyagi ギターとベースの切り替え
		public bool bIsSwappedGuitarBass
		{
			get;
			set;
		}

		// #24415 2011.2.21 yyagi FLIP中にalt-f4終了で、AUTOフラグがswapした状態でconfig.iniが出力されてしまうことを避けるためのフラグ
		public bool bIsSwappedGuitarBass_AutoFlagsAreSwapped
		{
			get;
			set;
		}

		// #35417 2015.8.18 yyagi FLIP中にalt-f4終了で、演奏設定がswapした状態でconfig.iniが出力されてしまうことを避けるためのフラグ
		public bool bIsSwappedGuitarBass_PlaySettingsAreSwapped
		{
			get;
			set;
		}

		public void SwapGuitarBassInfos_AutoFlags()
		{
			Func<EPad, EPad, bool> AutoSwapper = (x, y) =>
			{
				bool t = bAutoPlay[y];
				bAutoPlay[y].Value = bAutoPlay[x];
				bAutoPlay[x].Value = t;
				return true;
			};

			AutoSwapper(EPad.GtR, EPad.BsR);
			AutoSwapper(EPad.GtG, EPad.BsG);
			AutoSwapper(EPad.GtB, EPad.BsB);
			AutoSwapper(EPad.GtWail, EPad.BsWail);
			AutoSwapper(EPad.GtPick, EPad.BsPick);

			bIsSwappedGuitarBass_AutoFlagsAreSwapped = !bIsSwappedGuitarBass_AutoFlagsAreSwapped;
		}

		public void t指定した入力が既にアサイン済みである場合はそれを全削除する(EInputDevice DeviceType, int nID, int nCode)
		{
			for (EPad j = EPad.Min; j < EPad.Max; j++)
			{
				for (int k = 0; k < AssignableCodes; k++)
				{
					if (((KeyAssign[j][k].入力デバイス == DeviceType) &&
							(KeyAssign[j][k].ID == nID)) &&
							(KeyAssign[j][k].コード == nCode))
					{
						for (int m = k; m < AssignableCodes - 1; m++)
						{
							KeyAssign[j][m].CopyFrom(KeyAssign[j][m + 1].Value);
						}
						KeyAssign[j][AssignableCodes - 1].Reset();
						k--;
					}
				}
			}
		}

		private void ClearKeyAssign()
		{
			KeyAssign = new STPadValue<COptionKeyAssign[]>();
			for (EPad j = EPad.Min; j < EPad.Max; ++j)
			{
				KeyAssign[j] = new COptionKeyAssign[AssignableCodes];
				for (int k = 0; k < AssignableCodes; ++k)
				{
					KeyAssign[j][k] = new COptionKeyAssign();
				}
			}
		}
		private void SetDefaultKeyAssign()
		{
			if (KeyAssign == null)
			{
				ClearKeyAssign();
			}

			SetKeyAssignFromString( strDefaultKeyAssign );
		}
		private void SetKeyAssignFromString( string strParam )
		{
			if (strParam == null)
			{
				return;
			}
			string[] paramLines = strParam.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );
			if (paramLines.Length <= 1)
			{
				Debug.WriteLine( "デフォルトのキーアサインを設定できませんでした1: " + strParam );
				return;
			}

			Dictionary<string, EPad> dicStrEPad = new Dictionary<string, EPad>()
			{
				{ "HH", EPad.HH },
				{ "SD", EPad.SD },
				{ "BD", EPad.BD },
				{ "HT", EPad.HT },
				{ "LT", EPad.LT },
				{ "FT", EPad.FT },
				{ "CY", EPad.CY },
				{ "HHO", EPad.HHO },
				{ "RD", EPad.RD },
				{ "LC", EPad.LC },
				{ "HP", EPad.HP },
				{ "GtR", EPad.GtR },
				{ "GtG", EPad.GtG },
				{ "GtB", EPad.GtB },
				{ "GtPick", EPad.GtPick },
				{ "GtWail", EPad.GtWail },
				{ "GtDecide", EPad.GtDecide },
				{ "GtCancel", EPad.GtCancel },
				{ "BsR", EPad.BsR },
				{ "BsG", EPad.BsG },
				{ "BsB", EPad.BsB },
				{ "BsPick", EPad.BsPick },
				{ "BsWail", EPad.BsWail },
				{ "BsDecide", EPad.BsDecide },
				{ "BsCancel", EPad.BsCancel },
				{ "Capture", EPad.Capture}
			};

			foreach ( string param in paramLines )
			{
				string[] tmp = param.Split( '=' );
				if ( tmp.Length != 2 )
				{
					Debug.WriteLine( "デフォルトのキーアサインを設定できませんでした2: " + param );
					return;
				}

				string strPad = tmp[ 0 ];
				string[] keys = tmp[ 1 ].Split( ',' );
				if ( keys.Length <= 0 )
				{
					Debug.WriteLine( "デフォルトのキーアサインを設定できませんでした3: " + tmp[ 1 ] );
					return;
				}
				EPad e;
				bool b = dicStrEPad.TryGetValue( strPad, out e );
				if ( b == false )
				{
					Debug.WriteLine( "デフォルトキーアサインの文字列に誤りがあります1: " + strPad );
				}
				int count = 0;
				foreach ( string key in keys )
				{
					char cInputDevice = key[ 0 ];
					EInputDevice eid;
					switch (cInputDevice)
					{
						case 'K': eid = EInputDevice.Keyboard; break;
						case 'M': eid = EInputDevice.MIDIIn; break;
						case 'J': eid = EInputDevice.JoyPad; break;
						case 'N': eid = EInputDevice.Mouse; break;
						default: eid = EInputDevice.Unknown; break;
					}
					int nID = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf( key[ 1 ] );	// #24166 2011.1.15 yyagi: to support ID > 10, change 2nd character from Decimal to 36-numeral system. (e.g. J1023 -> JA23)
					int nCode = Convert.ToInt32( key.Substring( 2 ) );

					KeyAssign[ e ][ count ].Value.InputDevice = eid;
					KeyAssign[ e ][ count ].Value.ID = nID;
					KeyAssign[ e ][ count ].Value.Code = nCode;
					count++;

					// Debug.WriteLine( eid.ToString() + nID.ToString() + nCode.ToString() );
				}
			}
		}
const string strDefaultKeyAssign = @"
HH=K035,M042,M093
SD=K033,M025,M026,M027,M028,M029,M031,M032,M034,M037,M038,M040,M0113
BD=K012,K0126,M033,M035,M036,M0112
HT=K031,M048,M050
LT=K011,M047
FT=K023,M041,M043,M045
CY=K022,M049,M052,M055,M057,M091
HHO=K010,M046,M092
RD=K020,M051,M053,M059,M089
LC=K026
HP=M044

GtR=K055
GtG=K056,J012
GtB=K057
GtPick=K0115,K046,J06
GtWail=K0116
GtDecide=K060
GtCancel=K061

BsR=K090
BsG=K091,J013
BsB=K092
BsPick=K0103,K0100,J08
BsWail=K089
BsDecide=K096
BsCancel=K097

Capture=K065
";
		private string GetRelativePath( string strBasePath, string strTargetPath )
		{
			string strRelativePath = strTargetPath;
			try
			{
				Uri uri = new Uri( strBasePath );
				strRelativePath = Uri.UnescapeDataString( uri.MakeRelativeUri( new Uri( strTargetPath ) ).ToString() ).Replace( '/', '\\' );
			}
			catch ( UriFormatException )
			{
			}

			return strRelativePath;
		}
	}
}
