using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace DTXMania
{
	/// <summary>
	/// DataContract はフィールドまたはプロパティを XML にシリアライズできます。
	/// DataMember が付いているメンバだけがシリアライズされます。シリアライズ可能な型については MSDN を参照してください。
	/// DataMember が付いていないメンバはデシリアライズ時に別途指定される既定値になります。
	/// これらは OnDeserialized 属性がついたメソッドで処理します。
	/// </summary>
	[DataContract]
	public class STX
	{
		public STX(int x)
		{
			X = x;
		}

		[DataMember(Name = "X")]
		public int X;
	}

	[DataContract]
	public class STY
	{
		public STY(int y)
		{
			Y = y;
		}
		[DataMember(Name = "Y")]
		public int Y;
	}

	[DataContract]
	public class STXY : STX
	{
		public STXY(int x, int y)
			: base(x)
		{
			Y = y;
		}

		[DataMember(Name = "Y")]
		public int Y;
	}

	[DataContract]
	public class STXW : STX
	{
		public STXW(int x, int w)
			: base(x)
		{
			W = w;
		}

		[DataMember(Name = "Width")]
		public int W;
	}

	[DataContract]
	public class STXYW : STXY
	{
		public STXYW(int x, int y, int w)
			: base(x, y)
		{
			W = w;
		}

		[DataMember(Name = "Width")]
		public int W;
	}

	[DataContract]
	public class STRange
	{
		public STRange(int min, int max)
		{
			Min = min;
			Max = max;
		}

		[DataMember(Name = "Min")]
		public int Min;
		[DataMember(Name = "Max")]
		public int Max;
	}

	[DataContract]
	public class STRect : STXY
	{
		public System.Drawing.Rectangle rc;
		public System.Drawing.Rectangle rcTmp;

		public STRect(int x, int y, int w, int h)
			: base(x, y)
		{
			W = w;
			H = h;
			rc = new System.Drawing.Rectangle(X, Y, W, H);
			rcTmp = new System.Drawing.Rectangle(X, Y, W, H);
		}

		public static implicit operator System.Drawing.Rectangle(STRect x)
		{
			return x.rcTmp;
		}

		public System.Drawing.Rectangle ApplyCounterY(int counter, int overlap)
		{
			rcTmp.Y = rc.Y + (rc.Height - overlap) * counter;
			return rcTmp;
		}
		public System.Drawing.Rectangle ApplyCounterX(int counter, int overlap)
		{
			rcTmp.X = rc.X + (rc.Width - overlap) * counter;
			return rcTmp;
		}
		public System.Drawing.Rectangle ApplyCounterXY(int counterX, int counterY, int overlapX, int overlapY)
		{
			rcTmp.X = rc.X + (rc.Width - overlapX) * counterX;
			rcTmp.Y = rc.Y + (rc.Height - overlapY) * counterY;
			return rcTmp;
		}

		[DataMember(Name = "Width")]
		public int W;
		[DataMember(Name = "Height")]
		public int H;
	}

	[DataContract]
	public class Lane
	{
		[DataMember(Name = "LeftCymbal")]
		public STXW LCY;
		[DataMember(Name = "HiHatClose")]
		public STXW HHC;
		[DataMember(Name = "HiHatOpen")]
		public STXW HHO;
		[DataMember(Name = "SnareDrum")]
		public STXW SD;
		[DataMember(Name = "BassDrum")]
		public STXW BD;
		[DataMember(Name = "HighTom")]
		public STXW HT;
		[DataMember(Name = "LowTom")]
		public STXW LT;
		[DataMember(Name = "FloorTom")]
		public STXW FT;
		[DataMember(Name = "Cymbal")]
		public STXW CY;
		[DataMember(Name = "RideCymbal")]
		public STXW RCY;

		[DataMember(Name = "GuitarOpen")]
		public STXW GtO;
		[DataMember(Name = "GuitarR")]
		public STXW GtR;
		[DataMember(Name = "GuitarG")]
		public STXW GtG;
		[DataMember(Name = "GuitarB")]
		public STXW GtB;
		[DataMember(Name = "GuitarWailing")]
		public STXW GtW;

		[DataMember(Name = "BassOpen")]
		public STXW BsO;
		[DataMember(Name = "BassR")]
		public STXW BsR;
		[DataMember(Name = "BassG")]
		public STXW BsG;
		[DataMember(Name = "BassB")]
		public STXW BsB;
		[DataMember(Name = "BassWailing")]
		public STXW BsW;
	}

	[DataContract]
	public class Judge
	{
		[DataMember(Name = "LeftCymbal")]
		public STXY LCY;
		[DataMember(Name = "HiHatClose")]
		public STXY HH;
		[DataMember(Name = "SnareDrum")]
		public STXY SD;
		[DataMember(Name = "BassDrum")]
		public STXY BD;
		[DataMember(Name = "HighTom")]
		public STXY HT;
		[DataMember(Name = "LowTom")]
		public STXY LT;
		[DataMember(Name = "FloorTom")]
		public STXY FT;
		[DataMember(Name = "Cymbal")]
		public STXY CY;

		[DataMember(Name = "Guitar")]
		public STXY Gt;

		[DataMember(Name = "Bass")]
		public STXY Bs;
	}

	[DataContract]
	public class Coordinates
	{
		[DataMember(Name = "Lane")]
		public Lane Lane;

		[DataMember(Name = "Combo")]
		public STDGBVALUE<STXY> Combo;

		[DataMember(Name = "MovieBGAGuitarsOnly")]
		public STRect GtMovie;

		[DataMember(Name = "MovieBGADrumsOnly")]
		public STRect DrMovie;

		[DataMember(Name = "MovieBGACommon")]
		public STRect Movie;

		[DataMember(Name = "Judge")]
		public Judge Judge;

		[DataMember(Name = "JudgeLine")]
		public STDGBVALUE<STXYW> JudgeLine;

		[DataMember(Name = "Score")]
		public STDGBVALUE<STXY> Score;

		[DataMember(Name = "Gauge")]
		public STXY Gauge;

		[DataMember(Name = "GaugeMax")]
		public int GaugeMax;

		[DataMember(Name = "StatusPanel")]
		public STDGBVALUE<STXY> StatusPanel;

		[DataMember(Name = "Graph")]
		public STDGBVALUE<STRect> Graph;

		[DataMember(Name = "Danger")]
		public STRange Danger;

		[DataMember(Name = "Panel")]
		public STRect Panel;

		[DataMember(Name = "Debug")]
		public STXY Debug;

		[DataMember(Name = "BarLine")]
		public STDGBVALUE<STX> BarLine;

		[DataMember(Name = "SuddenFrom")]
		public STDGBVALUE<STY> SuddenFrom;
		[DataMember(Name = "HiddenFrom")]
		public STDGBVALUE<STY> HiddenFrom;
		[DataMember(Name = "SuddenFadeInFrom")]
		public STDGBVALUE<STY> SuddenFadeInFrom;
		[DataMember(Name = "HiddenFadeOutFrom")]
		public STDGBVALUE<STY> HiddenFadeOutFrom;
		[DataMember(Name = "LaneFlash")]
		public STDGBVALUE<STY> LaneFlash;

		[DataMember(Name = "DrumsPad")]
		public STY DrPad;
		[DataMember(Name = "DrumsPadOffsetLCY")]
		public STY DrPadOffsetLCY;
		[DataMember(Name = "DrumsPadOffsetHH")]
		public STY DrPadOffsetHH;
		[DataMember(Name = "DrumsPadOffsetSD")]
		public STY DrPadOffsetSD;
		[DataMember(Name = "DrumsPadOffsetBD")]
		public STY DrPadOffsetBD;
		[DataMember(Name = "DrumsPadOffsetHT")]
		public STY DrPadOffsetHT;
		[DataMember(Name = "DrumsPadOffsetLT")]
		public STY DrPadOffsetLT;
		[DataMember(Name = "DrumsPadOffsetFT")]
		public STY DrPadOffsetFT;
		[DataMember(Name = "DrumsPadOffsetCY")]
		public STY DrPadOffsetCY;
		[DataMember(Name = "DrumsPadOffsetRCY")]
		public STXY DrPadOffsetRCY;

		public STRect ImgGtChipOpen;
		public int ImgGtCountOpenChip;
		public STRect ImgGtRGBButton;
		public int ImgGtCountRGBChip;
		public int ImgGtCountWailingChip;
		public STRect ImgGtWailingBonus;
		public STRect ImgGtLaneFlash;
		public STRect ImgGtWailingChip;
		public STRect ImgGtWailingFrame;
		public STRect ImgGtButtonR;
		public STRect ImgGtButtonG;
		public STRect ImgGtButtonB;
		public STRect ImgGtPressingButtonR;
		public STRect ImgGtPressingButtonG;
		public STRect ImgGtPressingButtonB;
		public STRect ImgDrLaneFlash;
		public STRect ImgDrChipHHC;
		public STRect ImgDrChipSD;
		public STRect ImgDrChipBD;
		public STRect ImgDrChipHT;
		public STRect ImgDrChipLT;
		public STRect ImgDrChipCY;
		public STRect ImgDrChipCYDeco;
		public STRect ImgDrChipFT;
		public STRect ImgDrChipHHO;
		public STRect ImgDrChipHHODeco;
		public STRect ImgDrChipRCY;
		public int ImgDrCountChip;
		public STRect ImgDrPadLCY;
		public STRect ImgDrPadHH;
		public STRect ImgDrPadSD;
		public STRect ImgDrPadBD;
		public STRect ImgDrPadHT;
		public STRect ImgDrPadLT;
		public STRect ImgDrPadFT;
		public STRect ImgDrPadCY;
		public STRect ImgDrPadRCY;
		public STRect ImgJudgeLine;
		public STRect ImgBarLine;
		public STRect ImgBeatLine;
		public STRect ImgComboCombo;
		public STRect ImgComboOneDigit;
		public STRect ImgGaugeOrange;
		public STRect ImgGaugeNormal;
		public STRect ImgGaugeLight;
		public STRect ImgGaugeTopLight;
		public STRect ImgGaugeStar;
		public STRect ImgScoreOneDigit;
		public STRect ImgSpeedPanel;
		public STRect ImgConsoleFont;
		public STRect ImgJudgeString;
		public STRect ImgDanger;

		public STDGBVALUE<STY>[] OffsetGuitarBassSwap;
		public STDGBVALUE<STXY> PointFullCombo;


		/// <summary>
		/// デシリアライズが終了したとき呼び出されます。
		/// シリアル化非対象メンバはここで初期値を与えます。
		/// </summary>
		/// <param name="sc">ストリーミングコンテキスト。使用されません。</param>
		[OnDeserialized]
		public void DefaultDeserializer(StreamingContext sc)
		{
			ImgJudgeLine = new STRect(0, 0, 64, 16);

			ImgGtChipOpen = new STRect(72 * 3, 0, 72 * 3, 14);
			ImgGtRGBButton = new STRect(0, 0, 72, 14);
			ImgGtWailingChip = new STRect(0, 590, 60, 120);
			ImgGtCountWailingChip = 5;
			ImgGtCountRGBChip = 48;
			ImgGtCountOpenChip = 48;

			ImgDanger = new STRect(0, 0, 64, 256);

			ImgGtButtonR = new STRect(0, 0, 72, 72);
			ImgGtButtonG = new STRect(72, 0, 72, 72);
			ImgGtButtonB = new STRect(144, 0, 72, 72);
			ImgGtPressingButtonR = new STRect(0, 72, 72, 72);
			ImgGtPressingButtonG = new STRect(72, 72, 72, 72);
			ImgGtPressingButtonB = new STRect(144, 72, 72, 72);
			ImgGtWailingBonus = new STRect(0, 0, 78, 549);

			ImgComboCombo = new STRect(100, 140, 110, 30);
			ImgComboOneDigit = new STRect(0, 0, 50, 70);

			ImgGtWailingFrame = new STRect(0, 0, 66, 113);
			ImgSpeedPanel = new STRect(0, 0, 45, 164);

			ImgConsoleFont = new STRect(0, 0, 16, 32);
			ImgJudgeString = new STRect(0, 0, 256, 36);

			ImgDrCountChip = 48;
			ImgDrChipBD = new STRect(0, 20, 99, 22);
			ImgDrChipHHC = new STRect(99, 16, 72, 18);
			ImgDrChipSD = new STRect(99 + 72, 16, 72, 18);
			ImgDrChipHT = new STRect(99 + 72 * 2, 16, 72, 18);
			ImgDrChipLT = new STRect(99 + 72 * 3, 16, 72, 18);
			ImgDrChipFT = new STRect(99 + 72 * 4, 16, 72, 18);
			ImgDrChipCY = new STRect(99 + 72 * 5, 16, 85, 18);
			ImgDrChipCYDeco = new STRect(99 + 72 * 5, 786, 85, 64);
			ImgDrChipRCY = new STRect(99 + 72 * 5 + 85, 16, 58, 18);
			ImgDrChipHHO = new STRect(99 + 72 * 5 + 85, 16, 58, 18);
			ImgDrChipHHODeco = new STRect(99 + 72 * 5 + 85, 791, 58, 30);

			ImgDrPadLCY = new STRect(0, 0, 170, 130);
			ImgDrPadHH = new STRect(170, 0, 170, 130);
			ImgDrPadSD = new STRect(340, 0, 170, 130);
			ImgDrPadBD = new STRect(0, 130, 170, 130);
			ImgDrPadHT = new STRect(170, 130, 170, 130);
			ImgDrPadLT = new STRect(340, 130, 170, 130);
			ImgDrPadFT = new STRect(0, 260, 170, 130);
			ImgDrPadCY = new STRect(170, 260, 170, 130);
			ImgDrPadRCY = new STRect(340, 260, 170, 130);

			ImgBarLine = new STRect(0, 982, 480, 3);
			ImgBeatLine = new STRect(0, 985, 480, 2);

			ImgGaugeOrange = new STRect(48, 0, 48, 61);
			ImgGaugeNormal = new STRect(0, 0, 48, 61);
			ImgGaugeLight = new STRect(0, 0, 48, 576);
			ImgGaugeTopLight = new STRect(144, 0, 48, 36);
			ImgGaugeStar = new STRect(0, 72, 96, 72);

			ImgScoreOneDigit = new STRect(0, 0, 30, 38);

			ImgGtLaneFlash = new STRect(0, 0, -1, 512); // w無効
			ImgDrLaneFlash = new STRect(0, 0, -1, 768); // w無効

			OffsetGuitarBassSwap = new STDGBVALUE<STY>[2];// #24063 2011.1.27 yyagi
			OffsetGuitarBassSwap[0] = new STDGBVALUE<STY>();
			OffsetGuitarBassSwap[0].Drums = new STY(0);
			OffsetGuitarBassSwap[0].Guitar = new STY((int)(21 * Scale.Y));
			OffsetGuitarBassSwap[0].Bass = new STY((int)(42 * Scale.Y));
			OffsetGuitarBassSwap[1].Drums = new STY(0);
			OffsetGuitarBassSwap[1].Guitar = new STY((int)(42 * Scale.Y));
			OffsetGuitarBassSwap[1].Bass = new STY((int)(21 * Scale.Y));

			PointFullCombo = new STDGBVALUE<STXY>();
			PointFullCombo.Drums = new STXY((int)(0x80 * Scale.X), (int)(0xed * Scale.Y));
			PointFullCombo.Guitar = new STXY((int)(0xdf * Scale.X), (int)(0xed * Scale.Y));
			PointFullCombo.Bass = new STXY((int)(0x141 * Scale.X), (int)(0xed * Scale.Y));
		}

		/// <summary>
		/// デシリアライズを開始しようとする前に呼ばれます。
		/// シリアル化対象メンバを初期化します。
		/// 該当要素がXMLにすでに存在する場合、ここで指定したその要素に対する値は無効です。
		/// </summary>
		/// <param name="sc">ストリーミングコンテキスト。使用しません。</param>
		[OnDeserializing]
		public void DefaultDeserializing(StreamingContext sc)
		{

			Lane = new Lane();
			int x = 480 - 72 * 4;
			Lane.GtO = new STXW(x, 72);
			Lane.GtR = new STXW(x, 72);
			x += 72;
			Lane.GtG = new STXW(x, 72);
			x += 72;
			Lane.GtB = new STXW(x, 72);
			x += 72;
			Lane.GtW = new STXW(x, 72);

			x = 1920 - 480;
			Lane.BsO = new STXW(x, 72);
			Lane.BsR = new STXW(x, 72);
			x += 72;
			Lane.BsG = new STXW(x, 72);
			x += 72;
			Lane.BsB = new STXW(x, 72);
			x += 72;
			Lane.BsW = new STXW(x, 72);

			x = 1920 / 2 - 314;
			Lane.LCY = new STXW(x, 85);
			x += 85;
			Lane.HHC = new STXW(x, 72);
			Lane.HHO = new STXW(x, 72);
			x += 72;
			Lane.SD = new STXW(x, 72);
			x += 72;
			Lane.BD = new STXW(x, 99);
			x += 99;
			Lane.HT = new STXW(x, 72);
			x += 72;
			Lane.LT = new STXW(x, 72);
			x += 72;
			Lane.FT = new STXW(x, 72);
			x += 72;
			Lane.CY = new STXW(x, 85);
			Lane.RCY = new STXW(x, 85);

			LaneFlash = new STDGBVALUE<STY>();
			LaneFlash.Drums = new STY(SampleFramework.GameWindowSize.Height - 768);
			LaneFlash.Guitar = new STY(0);
			LaneFlash.Bass = new STY(0);

			JudgeLine = new STDGBVALUE<STXYW>();
			JudgeLine.Drums = new STXYW(Lane.LCY.X, 942, Lane.CY.X + Lane.CY.W - Lane.LCY.X);
			JudgeLine.Guitar = new STXYW(Lane.GtR.X, 138, Lane.GtB.X + Lane.GtB.W - Lane.GtR.X);
			JudgeLine.Bass = new STXYW(Lane.BsR.X, 138, Lane.BsB.X + Lane.BsB.W - Lane.BsR.X);

			// Graph = new STDGBVALUE<STXY>();

			Gauge = new STXY(0, 1080);
			GaugeMax = 1080;

			Graph = new STDGBVALUE<STRect>();
			Graph.Drums = new STRect(SampleFramework.GameWindowSize.Width / 2 + 314, 0, 60, 400);
			Graph.Guitar = new STRect(480, 0, 60, 400);
			Graph.Bass = new STRect(SampleFramework.GameWindowSize.Width - 480 + 72 * 4, 0, 60, 400);

			Debug = new STXY(450 * 3, 128); // 要修正 actPlayInfo.t進行描画
			Movie = new STRect(619 + 682, 128, 556, 710);

			Combo = new STDGBVALUE<STXY>();
			Combo.Drums = new STXY(JudgeLine.Drums.W / 2 + JudgeLine.Drums.X, 80);
			Combo.Guitar = new STXY((JudgeLine.Guitar.W + 72) / 2 + JudgeLine.Guitar.X, 800);
			Combo.Bass = new STXY((JudgeLine.Bass.W + 72) / 2 + JudgeLine.Bass.X, 800);

			Score = new STDGBVALUE<STXY>();
			Score.Drums = new STXY(JudgeLine.Drums.X + JudgeLine.Drums.W / 2, 10);
			Score.Guitar = new STXY(JudgeLine.Guitar.X + JudgeLine.Guitar.W / 2, 10);
			Score.Bass = new STXY(JudgeLine.Bass.X + JudgeLine.Bass.W / 2, 10);

			Judge = new Judge();
			Judge.LCY = new STXY(Lane.LCY.X + Lane.LCY.W / 2, 350);
			Judge.HH = new STXY(Lane.HHC.X + Lane.HHC.W / 2, 420);
			Judge.SD = new STXY(Lane.SD.X + Lane.SD.W / 2, 350);
			Judge.BD = new STXY(Lane.BD.X + Lane.BD.W / 2, 420);
			Judge.HT = new STXY(Lane.HT.X + Lane.HT.W / 2, 350);
			Judge.LT = new STXY(Lane.LT.X + Lane.LT.W / 2, 420);
			Judge.FT = new STXY(Lane.FT.X + Lane.FT.W / 2, 350);
			Judge.CY = new STXY(Lane.CY.X + Lane.CY.W / 2, 420);
			Judge.Gt = new STXY(Combo.Guitar.X, 600);
			Judge.Bs = new STXY(Combo.Bass.X, 600);

			DrMovie = new STRect(619 + 682, 128, 556, 710);
			GtMovie = new STRect(682, 112, 556, 710);
			Movie = new STRect(1153, 128, 556, 710);

			Panel = new STRect(SampleFramework.GameWindowSize.Width, 0, 0, 0);
			StatusPanel = new STDGBVALUE<STXY>();
			StatusPanel.Drums = new STXY(Lane.LCY.X - 45, 0);
			StatusPanel.Guitar = new STXY(Lane.GtR.X - 45, 0);
			StatusPanel.Bass = new STXY(Lane.BsR.X - 45, 0);

			BarLine = new STDGBVALUE<STX>();
			BarLine.Drums = new STX(619);
			BarLine.Guitar = new STX(78);
			BarLine.Bass = new STX(480 * 3);

			DrPadOffsetLCY = new STY(0);
			DrPadOffsetHH = new STY(10);
			DrPadOffsetSD = new STY(22);
			DrPadOffsetBD = new STY(30);
			DrPadOffsetHT = new STY(0);
			DrPadOffsetLT = new STY(15);
			DrPadOffsetFT = new STY(30);
			DrPadOffsetCY = new STY(15);
			DrPadOffsetRCY = new STXY(80, 0);
			DrPad = new STY(930);

			SuddenFrom = new STDGBVALUE<STY>();
			SuddenFrom.Drums = new STY(450);
			SuddenFrom.Guitar = new STY(450);
			SuddenFrom.Bass = new STY(450);
			HiddenFrom = new STDGBVALUE<STY>();
			HiddenFrom.Drums = new STY(225);
			HiddenFrom.Guitar = new STY(225);
			HiddenFrom.Bass = new STY(225);
			SuddenFadeInFrom.Drums = new STY(562);
			HiddenFadeOutFrom.Drums = new STY(337);

			Danger = new STRange(0, SampleFramework.GameWindowSize.Width - 64);
		}

		/// <summary>
		/// シリアル化対象メンバはここで初期値を与えます。XMLが存在しない場合一度これらの値が書きだされ、
		/// もう一度デシリアライズされます。XMLが存在する場合、これらの値は全て無効になります。
		/// </summary>
		public Coordinates()
		{
			StreamingContext sc = new StreamingContext();
			DefaultDeserializing(sc);
		}
	}
}
