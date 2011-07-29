using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DTXMania
{
	public enum ECYGroup
	{
		打ち分ける,
		共通
	}
	public enum EFTGroup
	{
		打ち分ける,
		共通
	}
	public enum EHHGroup
	{
		全部打ち分ける,
		ハイハットのみ打ち分ける,
		左シンバルのみ打ち分ける,
		全部共通
	}
	public enum Eダークモード
	{
		OFF,
		HALF,
		FULL
	}
	public enum Eダメージレベル
	{
		少ない	= 0,
		普通	= 1,
		大きい	= 2
	}
	public enum Eパッド
	{
		HH		= 0,
		R		= 0,
		SD		= 1,
		G		= 1,
		BD		= 2,
		B		= 2,
		HT		= 3,
		Pick	= 3,
		LT		= 4,
		Wail	= 4,
		FT		= 5,
		Cancel	= 5,
		CY		= 6,
		Decide	= 6,
		HHO		= 7,
		RD		= 8,
		LC		= 9,
		UNKNOWN = 99
	}
	public enum EKeyConfigPad		// # 24609 
	{
		HH		= Eパッド.HH,
		R		= Eパッド.R,
		SD		= Eパッド.SD,
		G		= Eパッド.G,
		BD		= Eパッド.BD,
		B		= Eパッド.B,
		HT		= Eパッド.HT,
		Pick	= Eパッド.Pick,
		LT		= Eパッド.LT,
		Wail	= Eパッド.Wail,
		FT		= Eパッド.FT,
		Cancel	= Eパッド.Cancel,
		CY		= Eパッド.CY,
		Decide	= Eパッド.Decide,
		HHO		= Eパッド.HHO,
		RD		= Eパッド.RD,
		LC		= Eパッド.LC,
		Capture,
		UNKNOWN = Eパッド.UNKNOWN
	}
	[Flags]
	public enum EパッドFlag		// #24063 2011.1.16 yyagi コマンド入力用 パッド入力のフラグ化
	{
		None	= 0,
		HH		= 1,
		R		= 1,
		SD		= 2,
		G		= 2,
		B		= 4,
		BD		= 4,
		HT		= 8,
		Pick	= 8,
		LT		= 16,
		Wail	= 16,
		FT		= 32,
		Cancel	= 32,
		CY		= 64,
		Decide	= 128,
		HHO		= 128,
		RD		= 256,
		LC		= 512,
		UNKNOWN = 1024
	}
	public enum Eランダムモード
	{
		OFF,
		RANDOM,
		SUPERRANDOM,
		HYPERRANDOM
	}
	public enum E楽器パート
	{
		DRUMS	= 0,
		GUITAR	= 1,
		BASS	= 2,
		UNKNOWN	= 0x63
	}
	public enum EKeyConfigPart	// : E楽器パート
	{
		DRUMS	= E楽器パート.DRUMS,
		GUITAR	= E楽器パート.GUITAR,
		BASS	= E楽器パート.BASS,
		SYSTEM,
		UNKNOWN	= E楽器パート.UNKNOWN
	}

	public enum E打ち分け時の再生の優先順位
	{
		ChipがPadより優先,
		PadがChipより優先
	}
	internal enum E入力デバイス
	{
		キーボード		= 0,
		MIDI入力		= 1,
		ジョイパッド	= 2,
		マウス			= 3,
		不明			= -1
	}
	public enum E判定
	{
		Perfect	= 0,
		Great	= 1,
		Good	= 2,
		Poor	= 3,
		Miss	= 4,
		Bad		= 5,
		Auto
	}
	internal enum E判定文字表示位置
	{
		レーン上,
		判定ライン上または横,
		表示OFF
	}
	internal enum EAVI種別
	{
		Unknown,
		AVI,
		AVIPAN
	}
	internal enum EBGA種別
	{
		Unknown,
		BMP,
		BMPTEX,
		BGA,
		BGAPAN
	}
	internal enum EFIFOモード
	{
		フェードイン,
		フェードアウト
	}
	internal enum Eドラムコンボ文字の表示位置
	{
		LEFT,
		CENTER,
		RIGHT,
		OFF
	}
	internal enum Eドラムレーン
	{
		LC,
		HH,
		SD,
		BD,
		HT,
		LT,
		FT,
		CY,
		GT,		// AUTOレーン判定を容易にするため、便宜上定義しておく
		BS
	}
	internal enum Eログ出力
	{
		OFF,
		ON通常,
		ON詳細あり
	}
	internal enum E演奏画面の戻り値
	{
		継続,
		演奏中断,
		ステージ失敗,
		ステージクリア
	}

	/// <summary>
	/// Drum/Guitar/Bass の値を扱う汎用の構造体。
	/// </summary>
	/// <typeparam name="T">値の型。</typeparam>
	[StructLayout(LayoutKind.Sequential)]
	public struct STDGBVALUE<T>
	{
		public T Drums;
		public T Guitar;
		public T Bass;
		public T Unknown;
		public T this[ int index ]
		{
			get
			{
				switch( index )
				{
					case 0:
						return this.Drums;

					case 1:
						return this.Guitar;

					case 2:
						return this.Bass;

					case 0x63:
						return this.Unknown;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch( index )
				{
					case 0:
						this.Drums = value;
						return;

					case 1:
						this.Guitar = value;
						return;

					case 2:
						this.Bass = value;
						return;

					case 0x63:
						this.Unknown = value;
						return;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}


	internal class C定数
	{
		public const int BGA_H = 0x163;
		public const int BGA_W = 0x116;
		public const int HIDDEN_POS = 100;
		public const int MAX_AVI_LAYER = 1;
		public const int MAX_WAILING = 4;
		public const int PANEL_H = 0x1a;
		public const int PANEL_W = 0x116;
		public const int PREVIEW_H = 0x10d;
		public const int PREVIEW_W = 0xcc;
		public const int SCORE_H = 0x18;
		public const int SCORE_W = 12;
		public const int SUDDEN_POS = 200;

		public class Drums
		{
			public const int BAR_Y = 0x1a6;
			public const int BAR_Y_REV = 0x38;
			public const int BASS_BAR_Y = 0x5f;
			public const int BASS_BAR_Y_REV = 0x176;
			public const int BASS_H = 0x163;
			public const int BASS_W = 0x6d;
			public const int BASS_X = 0x18e;
			public const int BASS_Y = 0x39;
			public const int BGA_X = 0x152;
			public const int BGA_Y = 0x39;
			public const int GAUGE_H = 0x160;
			public const int GAUGE_W = 0x10;
			public const int GAUGE_X = 6;
			public const int GAUGE_Y = 0x35;
			public const int GUITAR_BAR_Y = 0x5f;
			public const int GUITAR_BAR_Y_REV = 0x176;
			public const int GUITAR_H = 0x163;
			public const int GUITAR_W = 0x6d;
			public const int GUITAR_X = 0x1fb;
			public const int GUITAR_Y = 0x39;
			public const int PANEL_X = 0x150;
			public const int PANEL_Y = 0x1ab;
			public const int SCORE_X = 0x164;
			public const int SCORE_Y = 14;
		}
		public class Guitar
		{
			public const int BAR_Y = 40;
			public const int BAR_Y_REV = 0x171;
			public const int BASS_H = 0x199;
			public const int BASS_W = 140;
			public const int BASS_X = 480;
			public const int BASS_Y = 0;
			public const int BGA_X = 0xb5;
			public const int BGA_Y = 50;
			public const int GAUGE_H = 0x10;
			public const int GAUGE_W = 0x80;
			public const int GAUGE_X_BASS = 0x14f;
			public const int GAUGE_X_GUITAR = 0xb2;
			public const int GAUGE_Y_BASS = 8;
			public const int GAUGE_Y_GUITAR = 8;
			public const int GUITAR_H = 0x199;
			public const int GUITAR_W = 140;
			public const int GUITAR_X = 0x1a;
			public const int GUITAR_Y = 0;
			public const int PANEL_X = 0xb5;
			public const int PANEL_Y = 430;
		}
	}
}
