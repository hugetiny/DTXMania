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
		B = 2,
		BD = 2,
		Cancel = 5,
		CY = 6,
		Decide = 6,
		FT = 5,
		G = 1,
		HH = 0,
		HHO = 7,
		HT = 3,
		LC = 9,
		LT = 4,
		Pick = 3,
		R = 0,
		RD = 8,
		SD = 1,
		UNKNOWN = 0x63,
		Wail = 4
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
		BASS = 2,
		DRUMS = 0,
		GUITAR = 1,
		UNKNOWN = 0x63
	}
	public enum E打ち分け時の再生の優先順位
	{
		ChipがPadより優先,
		PadがChipより優先
	}
	internal enum E入力デバイス
	{
		MIDI入力 = 1,
		キーボード = 0,
		ジョイパッド = 2,
		マウス = 3,
		不明 = -1
	}
	internal enum E判定
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
		CY
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
