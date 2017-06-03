using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DTXMania
{
	public enum EChannel : int
	{
		Invalid = -1,
		/// <summary>
		/// 0x01 BGM 割当チャンネル
		/// </summary>
		BGM = 0x01,
		BarLength = 0x02,
		BPM = 0x03,
		BGALayer1 = 0x04,
		ExObj_nouse = 0x05,
		MissAnimation_nouse = 0x06,
		BGALayer2 = 0x07,
		BPMEx = 0x08,
		BMS_reserved_09 = 0x09,
		BMS_reserved_0A = 0x0A,
		//BMS_reserved_0B		= 0x0B,
		//BMS_reserved_0C		= 0x0C,
		//BMS_reserved_0D		= 0x0D,
		//BMS_reserved_0E		= 0x0E,
		//BMS_reserved_0F		= 0x0F,
		//BMS_reserved_10		= 0x10,
		/// <summary>
		/// HHC 0x11
		/// </summary>
		HiHatClose = 0x11,
		Snare = 0x12,
		BassDrum = 0x13,
		HighTom = 0x14,
		LowTom = 0x15,
		Cymbal = 0x16,
		FloorTom = 0x17,
		HiHatOpen = 0x18,
		RideCymbal = 0x19,
		LeftCymbal = 0x1A,
		LeftPedal = 0x1B,
		/// <summary>
		/// 0x1C Visible LBD
		/// </summary>
		LeftBassDrum = 0x1C,
		//nouse_1d			= 0x1D,
		//nouse_1e			= 0x1E,
		/// <summary>
		/// 0x1F Dr. Fillin
		/// </summary>
		DrumsFillin = 0x1F,
		Guitar_Open = 0x20,
		Guitar_xxB = 0x21,
		Guitar_xGx = 0x22,
		Guitar_xGB = 0x23,
		Guitar_Rxx = 0x24,
		Guitar_RxB = 0x25,
		Guitar_RGx = 0x26,
		Guitar_RGB = 0x27,
		/// <summary>
		/// 0x28 Gt. Wailing
		/// </summary>
		Guitar_Wailing = 0x28,
		flowspeed_gt_nouse = 0x29,
		//nouse_2a			= 0x2A,
		//nouse_2b			= 0x2B,
		//nouse_2c			= 0x2C,
		//nouse_2d			= 0x2D,
		//nouse_2e			= 0x2E,
		/// <summary>
		/// 0x2F Gt. Wailing Sound
		/// </summary>
		Guitar_WailingSound = 0x2F,
		flowspeed_dr_nouse = 0x30,
		/// <summary>
		/// 0x31 HHC Hidden
		/// </summary>
		HiHatClose_Hidden = 0x31,
		Snare_Hidden = 0x32,
		BassDrum_Hidden = 0x33,
		HighTom_Hidden = 0x34,
		LowTom_Hidden = 0x35,
		Cymbal_Hidden = 0x36,
		FloorTom_Hidden = 0x37,
		HiHatOpen_Hidden = 0x38,
		RideCymbal_Hidden = 0x39,
		LeftCymbal_Hidden = 0x3A,
		LeftPedal_Hidden = 0x3B,
		/// <summary>
		/// 0x3C LBD Hidden
		/// </summary>
		LeftBassDrum_Hidden = 0x3C,
		//nouse_3d			= 0x3D,
		//nouse_3e			= 0x3E,
		//nouse_3f			= 0x3F,
		//BMS_reserved_40		= 0x40,
		//HiddenObject_2P_41	= 0x41,
		//HiddenObject_2P_42	= 0x42,
		//HiddenObject_2P_43	= 0x43,
		//HiddenObject_2P_44	= 0x44,
		//HiddenObject_2P_45	= 0x45,
		//HiddenObject_2P_46	= 0x46,
		//BMS_reserved_47		= 0x47,
		//BMS_reserved_48		= 0x48,
		//BMS_reserved_49		= 0x49,
		//nouse_4a			= 0x4A,
		//nouse_4b			= 0x4B,
		//nouse_4c			= 0x4C,
		//nouse_4d			= 0x4D,
		//nouse_4e			= 0x4E,
		//nouse_4f			= 0x4F,
		/// <summary>
		/// 0x50 BarLine
		/// </summary>
		BarLine = 0x50,
		/// <summary>
		/// 0x51 BeatLine
		/// </summary>
		BeatLine = 0x51,
		MIDIChorus = 0x52,
		FillIn = 0x53,
		Movie = 0x54,
		BGALayer3 = 0x55,
		BGALayer4 = 0x56,
		BGALayer5 = 0x57,
		BGALayer6 = 0x58,
		BGALayer7 = 0x59,
		MovieFull = 0x5A,
		nouse_5b = 0x5B,
		nouse_5c = 0x5C,
		nouse_5d = 0x5D,
		nouse_5e = 0x5E,
		nouse_5f = 0x5F,
		BGALayer8 = 0x60,
		SE01 = 0x61,
		SE02 = 0x62,
		SE03 = 0x63,
		SE04 = 0x64,
		SE05 = 0x65,
		SE06 = 0x66,
		SE07 = 0x67,
		SE08 = 0x68,
		SE09 = 0x69,
		//nouse_6a			= 0x6A,
		//nouse_6b			= 0x6B,
		//nouse_6c			= 0x6C,
		//nouse_6d			= 0x6D,
		//nouse_6e			= 0x6E,
		//nouse_6f			= 0x6F,
		SE10 = 0x70,
		SE11 = 0x71,
		SE12 = 0x72,
		SE13 = 0x73,
		SE14 = 0x74,
		SE15 = 0x75,
		SE16 = 0x76,
		SE17 = 0x77,
		SE18 = 0x78,
		SE19 = 0x79,
		//nouse_7a			= 0x7A,
		//nouse_7b			= 0x7B,
		//nouse_7c			= 0x7C,
		//nouse_7d			= 0x7D,
		//nouse_7e			= 0x7E,
		//nouse_7f			= 0x7F,
		SE20 = 0x80,
		SE21 = 0x81,
		SE22 = 0x82,
		SE23 = 0x83,
		SE24 = 0x84,
		SE25 = 0x85,
		SE26 = 0x86,
		SE27 = 0x87,
		SE28 = 0x88,
		SE29 = 0x89,
		//nouse_8a			= 0x8A,
		//nouse_8b			= 0x8B,
		//nouse_8c			= 0x8C,
		//nouse_8d			= 0x8D,
		//nouse_8e			= 0x8E,
		//nouse_8f			= 0x8F,
		SE30 = 0x90,
		SE31 = 0x91,
		SE32 = 0x92,
		//nouse_90			= 0x90,
		//nouse_91			= 0x91,
		//nouse_92			= 0x92,
		//nouse_93			= 0x93,
		//nouse_94			= 0x94,
		//nouse_95			= 0x95,
		//nouse_96			= 0x96,
		//nouse_97			= 0x97,
		//nouse_98			= 0x98,
		//nouse_99			= 0x99,
		//nouse_9a			= 0x9A,
		//nouse_9b			= 0x9B,
		//nouse_9c			= 0x9C,
		//nouse_9d			= 0x9D,
		//nouse_9e			= 0x9E,
		//nouse_9f			= 0x9F,
		/// <summary>
		/// 
		/// 0xA0 Bs. Open
		/// </summary>
		Bass_Open = 0xA0,
		Bass_xxB = 0xA1,
		Bass_xGx = 0xA2,
		Bass_xGB = 0xA3,
		Bass_Rxx = 0xA4,
		Bass_RxB = 0xA5,
		Bass_RGx = 0xA6,
		/// <summary>
		/// 0xA7 Bs. RGB
		/// </summary>
		Bass_RGB = 0xA7,
		/// <summary>
		/// 0xA8 Bs. Wailing
		/// </summary>
		Bass_Wailing = 0xA8,
		//nouse_a9			= 0xA9,
		//nouse_aa			= 0xAA,
		//nouse_ab			= 0xAB,
		//nouse_ac			= 0xAC,
		//nouse_ad			= 0xAD,
		//nouse_ae			= 0xAE,
		/// <summary>
		/// 0xAF Bs. Wailing Sound
		/// </summary>
		Bass_WailingSound = 0xAF,
		//nouse_b0			= 0xB0,
		HiHatClose_NoChip = 0xB1,
		Snare_NoChip = 0xB2,
		BassDrum_NoChip = 0xB3,
		HighTom_NoChip = 0xB4,
		LowTom_NoChip = 0xB5,
		Cymbal_NoChip = 0xB6,
		FloorTom_NoChip = 0xB7,
		HiHatOpen_NoChip = 0xB8,
		RideCymbal_NoChip = 0xB9,
		Guitar_NoChip = 0xBA,
		Bass_NoChip = 0xBB,
		LeftCymbal_NoChip = 0xBC,
		LeftPedal_NoChip = 0xBD,
		LeftBassDrum_NoChip = 0xBE,
		//nouse_bf			= 0xBF,
		//nouse_c0			= 0xC0,
		BeatLineShift = 0xC1,
		BeatLineDisplay = 0xC2,
		//nouse_c3			= 0xC3,
		BGALayer1_Swap = 0xC4,
		//nouse_c5			= 0xC5,
		//nouse_c6			= 0xC6,
		BGALayer2_Swap = 0xC7,
		//nouse_c8			= 0xC8,
		//nouse_c9			= 0xC9,
		//nouse_ca			= 0xCA,
		//nouse_cb			= 0xCB,
		//nouse_cc			= 0xCC,
		//nouse_cd			= 0xCD,
		//nouse_ce			= 0xCE,
		//nouse_cf			= 0xCF,
		//nouse_d0			= 0xD0,
		//nouse_d1			= 0xD1,
		//nouse_d2			= 0xD2,
		//nouse_d3			= 0xD3,
		//nouse_d4			= 0xD4,
		BGALayer3_Swap = 0xD5,
		BGALayer4_Swap = 0xD6,
		BGALayer5_Swap = 0xD7,
		BGALayer6_Swap = 0xD8,
		BGALayer7_Swap = 0xD9,
		MixerAdd = 0xDA,
		MixerRemove = 0xDB,
		//nouse_dc			= 0xDC,
		//nouse_dd			= 0xDD,
		//nouse_de			= 0xDE,
		//nouse_df			= 0xDF,
		BGALayer8_Swap = 0xE0,
		//nouse_e1			= 0xE1,
		//nouse_e2			= 0xE2,
		//nouse_e3			= 0xE3,
		//nouse_e4			= 0xE4,
		//nouse_e5			= 0xE5,
		//nouse_e6			= 0xE6,
		//nouse_e7			= 0xE7,
		//nouse_e8			= 0xE8,
		//nouse_e9			= 0xE9,
		//nouse_ea			= 0xEA,
		//nouse_eb			= 0xEB,
		Click				= 0xEC,		// metronome click
		//nouse_ed			= 0xED,
		//nouse_ee			= 0xEE,
		//nouse_ef			= 0xEF,
		//nouse_f0			= 0xF0,
		//nouse_f1			= 0xF1,
		//nouse_f2			= 0xF2,
		//nouse_f3			= 0xF3,
		//nouse_f4			= 0xF4,
		//nouse_f5			= 0xF5,
		//nouse_f6			= 0xF6,
		//nouse_f7			= 0xF7,
		//nouse_f8			= 0xF8,
		//nouse_f9			= 0xF9,
		//nouse_fa			= 0xFA,
		//nouse_fb			= 0xFB,
		//nouse_fc			= 0xFC,
		//nouse_fd			= 0xFD,
		//nouse_fe			= 0xFE,
		//nouse_ff			= 0xFF,
	}

	public enum EDTX種別
	{
		DTX,
		GDA,
		G2D,
		BMS,
		BME,
		SMF
	}

	public enum Eレーンビットパターン
	{
		OPEN,
		xxB,
		xGx,
		xGB,
		Rxx,
		RxB,
		RGx,
		RGB
	};

	public enum Eシステムサウンド
	{
		BGMオプション画面,
		BGMコンフィグ画面,
		BGM起動画面,
		BGM選曲画面,
		SOUNDステージ失敗音,
		SOUNDカーソル移動音,
		SOUNDゲーム開始音,
		SOUNDゲーム終了音,
		SOUNDステージクリア音,
		SOUNDタイトル音,
		SOUNDフルコンボ音,
		SOUND歓声音,
		SOUND曲読込開始音,
		SOUND決定音,
		SOUND取消音,
		SOUND変更音,
		SOUNDClickHigh,
		SOUNDClickLow,
		Count               // システムサウンド総数の計算用
	}

	public enum E演奏画面の戻り値
	{
		継続,
		演奏中断,
		ステージ失敗,
		ステージクリア,
		再読込_再演奏,
		再演奏
	}

	public enum E曲読込画面の戻り値
	{
		継続,
		読込完了,
		読込中止
	}

	[Flags]
	public enum EPadFlag        // #24063 2011.1.16 yyagi コマンド入力用 パッド入力のフラグ化
	{
		None = 0,
		HH = 1,
		R = 1,
		SD = 2,
		G = 2,
		B = 4,
		BD = 4,
		HT = 8,
		Pick = 8,
		LT = 16,
		Wail = 16,
		FT = 32,
		Cancel = 32,
		CY = 64,
		Decide = 128,
		HHO = 128,
		RD = 256,
		LC = 512,
		HP = 1024,              // #27029
		Unknown = 2048
	}

	public enum ESoundChipType
	{
		Drums,
		Guitar,
		Bass,
		SE,
		BGM,
		Unknown
	}

	public enum EAVIType
	{
		Unknown,
		AVI,
		AVIPAN
	}

	public enum EBGAType
	{
		Unknown,
		BMP,
		BMPTEX,
		BGA,
		BGAPAN
	}

	public enum EFIFOMode
	{
		フェードイン,
		フェードアウト
	}

	public enum EJudge
	{
		Perfect,
		Great,
		Good,
		Poor,
		Miss,
		Bad,
		Auto
	}
	// #36177 使用レーン数 ikanick add 16.04.15
	public enum EUseLanes : int
	{
		None,
		Dr_6,
		Dr_10,
		Dr_12,
		GB_3,
		GB_3_Open,
		GB_5,
		Other
	}

	[DataContract]
	public enum EActiveInstrument
	{
		[EnumMember]
		Both,
		[EnumMember]
		DrOnly,
		[EnumMember]
		GBOnly,
	}

	[DataContract]
	public enum ECYGroup
	{
		[EnumMember]
		Off,
		[EnumMember]
		Group
	}

	[DataContract]
	public enum EFTGroup
	{
		[EnumMember]
		Off,
		[EnumMember]
		Group
	}

	[DataContract]
	public enum EHHGroup
	{
		[EnumMember]
		Off,
		[EnumMember]
		HO_HC,
		[EnumMember]
		LC_HH,
		[EnumMember]
		Group
	}

	[DataContract]
	// #27029 2012.1.4 from add
	// Group はどちらも BD として扱われる
	public enum EBDGroup
	{
		[EnumMember]
		Off,
		[EnumMember]
		Group
	}

	[DataContract]
	public enum EDark
	{
		[EnumMember]
		Off,
		[EnumMember]
		Half,
		[EnumMember]
		Full
	}

	/// <summary>
	/// 演奏用のenum。ここを修正するときは、次に出てくる EパッドFlag もセットで修正すること。
	/// </summary>
	[DataContract]
	public enum EPad
	{
		[EnumMember]
		Min = 0,
		[EnumMember]
		DrumsPadMin = 0,
		[EnumMember]
		HH = 0,
		[EnumMember]
		SD = 1,
		[EnumMember]
		BD = 2,
		[EnumMember]
		HT = 3,
		[EnumMember]
		LT = 4,
		[EnumMember]
		CY = 5,
		[EnumMember]
		FT = 6,
		[EnumMember]
		HHO = 7,
		[EnumMember]
		RD = 8,
		[EnumMember]
		LC = 9,
		// #27029 2012.1.4 from
		[EnumMember]
		HP = 10,
		[EnumMember]
		DrumsPadMax = 11,

		[EnumMember]
		GuitarPadMin = 11,
		[EnumMember]
		GtR = 11,
		[EnumMember]
		GtG = 12,
		[EnumMember]
		GtB = 13,
		[EnumMember]
		GtPick = 14,
		[EnumMember]
		GtWail = 15,
		[EnumMember]
		GtCancel = 16,
		[EnumMember]
		GtDecide = 17,
		[EnumMember]
		GuitarPadMax = 18,

		[EnumMember]
		BassPadMin = 18,
		[EnumMember]
		BsR = 18,
		[EnumMember]
		BsG = 19,
		[EnumMember]
		BsB = 20,
		[EnumMember]
		BsPick = 21,
		[EnumMember]
		BsWail = 22,
		[EnumMember]
		BsCancel = 23,
		[EnumMember]
		BsDecide = 24,
		[EnumMember]
		BassPadMax = 25,

		[EnumMember]
		Capture = 25,
		[EnumMember]
		Max = 26,
		[EnumMember]
		Unknown,
	}


	[DataContract]
	public enum EDamage
	{
		[EnumMember]
		Easy,
		[EnumMember]
		Normal,
		[EnumMember]
		Hard
	}

	[DataContract]
	public enum ERandom
	{
		[EnumMember]
		Off,
		[EnumMember]
		Random,
		[EnumMember]
		Super,
		[EnumMember]
		Hyper
	}

	[DataContract]
	public enum EPart      // ここを修正するときは、セットで次の EKeyConfigPart も修正すること。
	{
		[EnumMember]
		Drums,
		[EnumMember]
		Guitar,
		[EnumMember]
		Bass,
		[EnumMember]
		System,
		[EnumMember]
		Unknown,
	}

	[DataContract]
	public enum ESoundDeviceTypeForConfig
	{
		[EnumMember]
		ASIO,
		[EnumMember]
		WASAPI_Exclusive,
		[EnumMember]
		WASAPI_Shared,
		[EnumMember]
		DSound,
	}

	[DataContract]
	public enum EHitSoundPriority
	{
		[EnumMember]
		Chip,
		[EnumMember]
		Pad
	}

	[DataContract]
	public enum EInputDevice
	{
		[EnumMember]
		Keyboard,
		[EnumMember]
		MIDIIn,
		[EnumMember]
		JoyPad,
		[EnumMember]
		Mouse,
		[EnumMember]
		Unknown
	}

	[DataContract]
	public enum EJudgeDisplayPriority
	{
		[EnumMember]
		Under,
		[EnumMember]
		Over
	}

	[DataContract]
	public enum ELane
	{
		[EnumMember]
		Min = 0,
		[EnumMember]
		DrumsLaneMin = 0,
		[EnumMember]
		LC = 0,
		[EnumMember]
		HH = 1,
		[EnumMember]
		SD = 2,
		[EnumMember]
		BD = 3,
		[EnumMember]
		HT = 4,
		[EnumMember]
		LT = 5,
		[EnumMember]
		FT = 6,
		[EnumMember]
		CY = 7,
		[EnumMember]
		DrumsLaneMax = 8,
		[EnumMember]
		GuitarLaneMin = 8,
		[EnumMember]
		GtR = 8,
		[EnumMember]
		GtG = 9,
		[EnumMember]
		GtB = 10,
		[EnumMember]
		GtW = 11,
		[EnumMember]
		GuitarLaneMax = 12,
		[EnumMember]
		BassLaneMin = 12,
		[EnumMember]
		BsR = 12,
		[EnumMember]
		BsG = 13,
		[EnumMember]
		BsB = 14,
		[EnumMember]
		BsW = 15,
		[EnumMember]
		BassLaneMax = 16,

		// 要素数取得のための定義 ("BGM"は含めない)
		[EnumMember]
		Max = 16,
		[EnumMember]
		BGM = 17,
	}

	/// <summary>
	/// 入力ラグ表示タイプ
	/// </summary>
	[DataContract]
	public enum EShowLagType
	{
		[EnumMember]
		Off,            // 全く表示しない
		[EnumMember]
		On,             // 判定に依らず全て表示する
		[EnumMember]
		UGreat      // GREAT-MISSの時のみ表示する(PERFECT時は表示しない)
	}

	[DataContract]
	public enum ESudHidInv
	{
		[EnumMember]
		Off,
		[EnumMember]
		Sudden,
		[EnumMember]
		Hidden,
		[EnumMember]
		SudHid,
		[EnumMember]
		SemiInv,
		[EnumMember]
		FullInv
	}

	/// <summary>
	/// 使用するAUTOゴーストデータの種類 (#35411 chnmr0)
	/// </summary>
	[DataContract]
	public enum EAutoGhostData
	{
		// 従来のAUTO
		[EnumMember]
		Perfect,
		// (.score.ini) の LastPlay ゴースト
		[EnumMember]
		LastPlay,
		// (.score.ini) の HiSkill ゴースト
		[EnumMember]
		HiSkill,
		// (.score.ini) の HiScore ゴースト
		[EnumMember]
		HiScore,
		// オンラインゴースト (DTXMOS からプラグインが取得、本体のみでは指定しても無効)
		[EnumMember]
		Online
	}

	/// <summary>
	/// 使用するターゲットゴーストデータの種類 (#35411 chnmr0)
	/// ここでNONE以外を指定してかつ、ゴーストが利用可能な場合グラフの目標値に描画される
	/// NONE の場合従来の動作
	/// </summary>
	[DataContract]
	public enum ETargetGhostData
	{
		[EnumMember]
		None,
		[EnumMember]
		Perfect,
		// (.score.ini) の LastPlay ゴースト
		[EnumMember]
		LastPlay,
		// (.score.ini) の HiSkill ゴースト
		[EnumMember]
		HiSkill,
		// (.score.ini) の HiScore ゴースト
		[EnumMember]
		HiScore,
		// オンラインゴースト (DTXMOS からプラグインが取得、本体のみでは指定しても無効)
		[EnumMember]
		Online
	}

	[DataContract]
	public enum EThreeState
	{
		[EnumMember]
		Off,
		[EnumMember]
		On,
		[EnumMember]
		X
	}


	[DataContract]
	public enum EOptionType
	{
		[EnumMember]
		Normal,
		[EnumMember]
		Other
	}

	[DataContract]
	public enum EClickType
	{
		[EnumMember]
		Off,
		[EnumMember]
		OnBeat,			// 表拍
		[EnumMember]
		OffBeat			// 裏拍
//		[EnumMember]
//		Triplet			// 3連符
	}

	[DataContract]
	public class STPadValue<T>
	{
		[DataMember]
		public T HH;
		[DataMember]
		public T SD;
		[DataMember]
		public T BD;
		[DataMember]
		public T HT;
		[DataMember]
		public T LT;
		[DataMember]
		public T CY;
		[DataMember]
		public T FT;
		[DataMember]
		public T HHO;
		[DataMember]
		public T RD;
		[DataMember]
		public T LC;
		[DataMember]
		public T HP;

		[DataMember]
		public T GtR;
		[DataMember]
		public T GtG;
		[DataMember]
		public T GtB;
		[DataMember]
		public T GtPick;
		[DataMember]
		public T GtWail;
		[DataMember]
		public T GtCancel;
		[DataMember]
		public T GtDecide;

		[DataMember]
		public T BsR;
		[DataMember]
		public T BsG;
		[DataMember]
		public T BsB;
		[DataMember]
		public T BsPick;
		[DataMember]
		public T BsWail;
		[DataMember]
		public T BsCancel;
		[DataMember]
		public T BsDecide;

		[DataMember]
		public T Capture;

		public T this[EPad e]
		{
			get
			{
				switch (e)
				{
					case EPad.HH: return HH;
					case EPad.SD: return SD;
					case EPad.BD: return BD;
					case EPad.HT: return HT;
					case EPad.LT: return LT;
					case EPad.CY: return CY;
					case EPad.FT: return FT;
					case EPad.HHO: return HHO;
					case EPad.RD: return RD;
					case EPad.LC: return LC;
					case EPad.HP: return HP;
					case EPad.GtR: return GtR;
					case EPad.GtG: return GtG;
					case EPad.GtB: return GtB;
					case EPad.GtPick: return GtPick;
					case EPad.GtWail: return GtWail;
					case EPad.GtCancel: return GtCancel;
					case EPad.GtDecide: return GtDecide;
					case EPad.BsR: return BsR;
					case EPad.BsG: return BsG;
					case EPad.BsB: return BsB;
					case EPad.BsPick: return BsPick;
					case EPad.BsWail: return BsWail;
					case EPad.BsCancel: return BsCancel;
					case EPad.BsDecide: return BsDecide;
					case EPad.Capture: return Capture;
				}
				throw new IndexOutOfRangeException();
			}

			set
			{
				switch (e)
				{
					case EPad.HH: HH = value; return;
					case EPad.SD: SD = value; return;
					case EPad.BD: BD = value; return;
					case EPad.HT: HT = value; return;
					case EPad.LT: LT = value; return;
					case EPad.CY: CY = value; return;
					case EPad.FT: FT = value; return;
					case EPad.HHO: HHO = value; return;
					case EPad.RD: RD = value; return;
					case EPad.LC: LC = value; return;
					case EPad.HP: HP = value; return;
					case EPad.GtR: GtR = value; return;
					case EPad.GtG: GtG = value; return;
					case EPad.GtB: GtB = value; return;
					case EPad.GtPick: GtPick = value; return;
					case EPad.GtWail: GtWail = value; return;
					case EPad.GtCancel: GtCancel = value; return;
					case EPad.GtDecide: GtDecide = value; return;
					case EPad.BsR: BsR = value; return;
					case EPad.BsG: BsG = value; return;
					case EPad.BsB: BsB = value; return;
					case EPad.BsPick: BsPick = value; return;
					case EPad.BsWail: BsWail = value; return;
					case EPad.BsCancel: BsCancel = value; return;
					case EPad.BsDecide: BsDecide = value; return;
					case EPad.Capture: Capture = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}


	[DataContract]
	public class STJudgeValue<T>
	{
		[DataMember]
		public T Perfect;
		[DataMember]
		public T Great;
		[DataMember]
		public T Good;
		[DataMember]
		public T Poor;

		public T this[EJudge index]
		{
			get
			{
				switch (index)
				{
					case EJudge.Perfect: return this.Perfect;
					case EJudge.Great: return this.Great;
					case EJudge.Good: return this.Good;
					case EJudge.Poor: return this.Poor;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch (index)
				{
					case EJudge.Perfect: this.Perfect = value; return;
					case EJudge.Great: this.Great = value; return;
					case EJudge.Good: this.Good = value; return;
					case EJudge.Poor: this.Poor = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}

	/// <summary>
	/// Drum/Guitar/Bass の値を扱う汎用の構造体。
	/// indexはE楽器パートと一致させること
	/// </summary>
	/// <typeparam name="T">値の型。</typeparam>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[DataContract]
	public struct STDGBSValue<T>
	{
		[DataMember]
		public T Drums;
		[DataMember]
		public T Guitar;
		[DataMember]
		public T Bass;
		[DataMember]
		public T System;
		[DataMember]
		public T Unknown;

		public T this[EPart index]
		{
			get
			{
				switch (index)
				{
					case EPart.Drums: return Drums;
					case EPart.Guitar: return Guitar;
					case EPart.Bass: return Bass;
					case EPart.System: return System;
					case EPart.Unknown: return Unknown;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch (index)
				{
					case EPart.Drums: Drums = value; return;
					case EPart.Guitar: Guitar = value; return;
					case EPart.Bass: Bass = value; return;
					case EPart.System: System = value; return;
					case EPart.Unknown: Unknown = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}

	[DataContract]
	public class STLaneValue<T>
	{
		public static ELane DrumsLane(EChannel x)
		{
			switch (x)
			{
				case EChannel.HiHatClose: return ELane.HH;
				case EChannel.HiHatOpen: return ELane.HH;
				case EChannel.Snare: return ELane.SD;
				case EChannel.BassDrum: return ELane.BD;
				case EChannel.HighTom: return ELane.HT;
				case EChannel.LowTom: return ELane.LT;
				case EChannel.FloorTom: return ELane.FT;
				case EChannel.Cymbal: return ELane.CY;
				case EChannel.LeftCymbal: return ELane.LC;
				case EChannel.RideCymbal: return ELane.CY;
			}
			throw new IndexOutOfRangeException("Drums Index is out of range");
		}

		[DataMember]
		public T LC;
		[DataMember]
		public T HH;
		[DataMember]
		public T SD;
		[DataMember]
		public T BD;
		[DataMember]
		public T HT;
		[DataMember]
		public T LT;
		[DataMember]
		public T FT;
		[DataMember]
		public T CY;
		[DataMember]
		public T GtR;
		[DataMember]
		public T GtG;
		[DataMember]
		public T GtB;
		[DataMember]
		public T GtW;
		[DataMember]
		public T BsR;
		[DataMember]
		public T BsG;
		[DataMember]
		public T BsB;
		[DataMember]
		public T BsW;

		public T this[ELane index]
		{
			get
			{
				switch (index)
				{
					case ELane.LC: return LC;
					case ELane.HH: return HH;
					case ELane.SD: return SD;
					case ELane.BD: return BD;
					case ELane.HT: return HT;
					case ELane.LT: return LT;
					case ELane.FT: return FT;
					case ELane.CY: return CY;
					case ELane.GtR: return GtR;
					case ELane.GtG: return GtG;
					case ELane.GtB: return GtB;
					case ELane.GtW: return GtW;
					case ELane.BsR: return BsR;
					case ELane.BsG: return BsG;
					case ELane.BsB: return BsB;
					case ELane.BsW: return BsW;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch (index)
				{
					case ELane.LC: LC = value; return;
					case ELane.HH: HH = value; return;
					case ELane.SD: SD = value; return;
					case ELane.BD: BD = value; return;
					case ELane.HT: HT = value; return;
					case ELane.LT: LT = value; return;
					case ELane.FT: FT = value; return;
					case ELane.CY: CY = value; return;
					case ELane.GtR: GtR = value; return;
					case ELane.GtG: GtG = value; return;
					case ELane.GtB: GtB = value; return;
					case ELane.GtW: GtW = value; return;
					case ELane.BsR: BsR = value; return;
					case ELane.BsG: BsG = value; return;
					case ELane.BsB: BsB = value; return;
					case ELane.BsW: BsW = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}

	[DataContract]
	public struct STInstValue<T>
	{
		[DataMember]
		public T Both;
		[DataMember]
		public T DrOnly;
		[DataMember]
		public T GBOnly;

		public T this[EActiveInstrument inst]
		{
			get
			{
				switch (inst)
				{
					case EActiveInstrument.Both: return Both;
					case EActiveInstrument.DrOnly: return DrOnly;
					case EActiveInstrument.GBOnly: return GBOnly;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch (inst)
				{
					case EActiveInstrument.Both: Both = value; return;
					case EActiveInstrument.DrOnly: DrOnly = value; return;
					case EActiveInstrument.GBOnly: GBOnly = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}


	[DataContract]
	public class CKeyAssign
	{
		[DataMember]
		public EInputDevice InputDevice;
		[DataMember]
		public int ID;
		[DataMember]
		public int Code;
		public CKeyAssign(EInputDevice DeviceType, int nID, int nCode)
		{
			this.InputDevice = DeviceType;
			this.ID = nID;
			this.Code = nCode;
		}
	}

	public enum EOptionPanelDirection
	{
		Horizontal,
		Vertical
	}

	public static class EnumConverter
	{
		public static ELane LaneFromPad(EPad t)
		{
			switch (t)
			{
				case EPad.HH: return ELane.HH;
				case EPad.SD: return ELane.SD;
				case EPad.BD: return ELane.BD;
				case EPad.HT: return ELane.HT;
				case EPad.LT: return ELane.LT;
				case EPad.CY: return ELane.CY;
				case EPad.FT: return ELane.FT;
				case EPad.HHO: return ELane.HH;
				case EPad.RD: return ELane.CY;
				case EPad.LC: return ELane.LC;
				case EPad.GtR: return ELane.GtR;
				case EPad.GtG: return ELane.GtG;
				case EPad.GtB: return ELane.GtB;
				case EPad.GtWail: return ELane.GtW;
				case EPad.BsR: return ELane.BsR;
				case EPad.BsG: return ELane.BsG;
				case EPad.BsB: return ELane.BsB;
				case EPad.BsWail: return ELane.BsW;
			}
			throw new IndexOutOfRangeException();
		}

		public static EPad PadFromLane(ELane t)
		{
			switch (t)
			{
				case ELane.LC: return EPad.LC;
				case ELane.HH: return EPad.HH;
				case ELane.SD: return EPad.SD;
				case ELane.BD: return EPad.BD;
				case ELane.HT: return EPad.HT;
				case ELane.LT: return EPad.LT;
				case ELane.FT: return EPad.FT;
				case ELane.CY: return EPad.CY;
				case ELane.GtR: return EPad.GtR;
				case ELane.GtG: return EPad.GtG;
				case ELane.GtB: return EPad.GtB;
				case ELane.GtW: return EPad.GtWail;
				case ELane.BsR: return EPad.BsR;
				case ELane.BsG: return EPad.BsG;
				case ELane.BsB: return EPad.BsB;
				case ELane.BsW: return EPad.BsWail;
			}
			throw new IndexOutOfRangeException();
		}

		/// <summary>
		/// EChannel -> EPad
		/// </summary>
		/// <param name="e">変換する EChannel列挙子。</param>
		/// <param name="NotDistinguishHH">HHOをパッドHHとして扱いたい場合 true。</param>
		/// <returns></returns>
		public static EPad PadFromChannel(EChannel e)
		{
			switch (e)
			{
				case EChannel.HiHatClose: return EPad.HH;
				case EChannel.Snare: return EPad.SD;
				case EChannel.BassDrum: return EPad.BD;
				case EChannel.LeftBassDrum: return EPad.BD;
				case EChannel.HighTom: return EPad.HT;
				case EChannel.LowTom: return EPad.LT;
				case EChannel.Cymbal: return EPad.CY;
				case EChannel.FloorTom: return EPad.FT;
				case EChannel.HiHatOpen: return EPad.HHO;
				case EChannel.RideCymbal: return EPad.RD;
				case EChannel.LeftCymbal: return EPad.LC;
				case EChannel.LeftPedal: return EPad.HP;
			}
			throw new IndexOutOfRangeException();
		}

		public static ELane LaneFromChannel(EChannel e)
		{
			switch (e)
			{
				case EChannel.HiHatClose: return ELane.HH;
				case EChannel.Snare: return ELane.SD;
				case EChannel.BassDrum: return ELane.BD;
				case EChannel.LeftBassDrum: return ELane.BD;
				case EChannel.HighTom: return ELane.HT;
				case EChannel.LowTom: return ELane.LT;
				case EChannel.Cymbal: return ELane.CY;
				case EChannel.FloorTom: return ELane.FT;
				case EChannel.HiHatOpen: return ELane.HH;
				case EChannel.RideCymbal: return ELane.CY;
				case EChannel.LeftCymbal: return ELane.LC;
				//case EChannel.LeftPedal: return ELane.HH;		// LPは仮にHHに落とし込む 後日、CDTXInputで別Laneに割り振るようにした後、このcaseは削除する
			}
			throw new IndexOutOfRangeException();
		}

		public static EChannel ChannelFromPad(EPad pad)
		{
			/*
			EChannel.HiHatClose, 0
						EChannel.Snare, 1 
						EChannel.BassDrum, 2
						EChannel.HighTom, 3 
						EChannel.LowTom,4 
						EChannel.FloorTom,5
						EChannel.Cymbal,6
						EChannel.HiHatOpen,7
						EChannel.RideCymbal,8
						EChannel.LeftCymbal, 9
						*/
			switch (pad)
			{
				case EPad.HH: return EChannel.HiHatClose;
				case EPad.HHO: return EChannel.HiHatOpen;
				case EPad.BD: return EChannel.BassDrum;
				case EPad.SD: return EChannel.Snare;
				case EPad.HT: return EChannel.HighTom;
				case EPad.LT: return EChannel.LowTom;
				case EPad.FT: return EChannel.FloorTom;
				case EPad.CY: return EChannel.Cymbal;
				case EPad.LC: return EChannel.LeftCymbal;
				case EPad.RD: return EChannel.RideCymbal;
				case EPad.HP: return EChannel.LeftPedal;
			}
			throw new NotImplementedException();
		}
	}
}
