using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using DTXMania;

namespace DTXMania.Coordinates
{
	[DataContract]
	public class CX
	{
		public CX(int x)
		{
			X = x;
		}
		[DataMember]
		public int X;
	}

	[DataContract]
	public class CY
	{
		public CY(int y)
		{
			Y = y;
		}
		[DataMember]
		public int Y;
	}

	[DataContract]
	public class CXY : CX
	{
		public CXY(int x, int y)
			: base(x)
		{
			Y = y;
		}

		[DataMember]
		public int Y;
	}

	[DataContract]
	public class CXW : CX
	{
		public CXW(int x, int w)
			: base(x)
		{
			W = w;
		}
		[DataMember]
		public int W;
	}

	[DataContract]
	public class CXYW : CXY
	{
		public CXYW(int x, int y, int w)
			: base(x, y)
		{
			W = w;
		}
		[DataMember]
		public int W;
	}

	[DataContract]
	public class CRange
	{
		public CRange(int min, int max)
		{
			Min = min;
			Max = max;
		}
		[DataMember]
		public int Min;
		[DataMember]
		public int Max;
	}

	[DataContract]
	public class CW
	{
		public CW(int w)
		{
			W = w;
		}
		[DataMember]
		public int W;
	}

	[DataContract]
	public class CWH : CW
	{
		public CWH(int w, int h)
			: base(w)
		{
			H = h;
		}

		[DataMember]
		public int H;
	}

	[DataContract]
	public class CRect : CXY
	{
		public System.Drawing.Rectangle rc;
		public System.Drawing.Rectangle rcTmp;

		public CRect(int x, int y, int w, int h)
			: base(x, y)
		{
			W = w;
			H = h;
			rc = new System.Drawing.Rectangle(X, Y, W, H);
			rcTmp = new System.Drawing.Rectangle(X, Y, W, H);
		}

		public static implicit operator System.Drawing.Rectangle(CRect x)
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
		[DataMember]
		public int W;
		[DataMember]
		public int H;

		// Deserialize後、WとHは初期化されるが、rc, rcTmpが初期化されない問題への対処。
		[OnDeserialized()]
		private void OnDeserializedMethod(StreamingContext context)
		{
			rc = new System.Drawing.Rectangle( X, Y, W, H );
			rcTmp = new System.Drawing.Rectangle( X, Y, W, H );
		}
	}

	public class CCoordinates
	{
		public CXY Gauge;
		public int GaugeMax;
		public STDGBSValue<CWH> Graph;
		public CRange Danger;
		public STDGBSValue<CW> Instrument;
		public STDGBSValue<CY> Score;
		public CW StatusPanel;
		public CXY Panel;
		public CWH Movie;
		public STDGBSValue<CY> LaneFlash;

		public CY DrPad;
		public STPadValue<CY> DrPadOffset;

		public CRect ImgGtChipOpen;
		public int ImgGtCountOpenChip;
		public CRect ImgGtRGBButton;
		public int ImgGtCountRGBChip;
		public int ImgGtCountWailingChip;
		public CRect ImgGtWailingBonus;
		public CRect ImgGtLaneFlash;
		public CRect ImgGtWailingChip;
		public CRect ImgGtWailingFrame;
		public CRect ImgGtButtonR;
		public CRect ImgGtButtonG;
		public CRect ImgGtButtonB;
		public CRect ImgGtPressingButtonR;
		public CRect ImgGtPressingButtonG;
		public CRect ImgGtPressingButtonB;
		public CRect ImgDrLaneFlash;

		public STPadValue<CRect> ImgDrChip;
		public CRect ImgDrChipCYDeco;
		public CRect ImgDrChipHHODeco;
		public int ImgDrCountChip;

		public STPadValue<CRect> ImgDrPad;
		public CRect ImgJudgeLine;
		public CRect ImgBarLine;
		public CRect ImgBeatLine;
		public CRect ImgComboCombo;
		public CRect ImgComboOneDigit;
		public CRect ImgGaugeOrange;
		public CRect ImgGaugeNormal;
		public CRect ImgGaugeLight;
		public CRect ImgGaugeTopLight;
		public CRect ImgGaugeStar;
		public CRect ImgScoreOneDigit;
		public CRect ImgSpeedPanel;
		public CRect ImgConsoleFont;
		public CRect ImgJudgeString;
		public CRect ImgDanger;

		public CWH ImgOptionPanel;

		public STDGBSValue<CY>[] OffsetGuitarBassSwap;
		public STDGBSValue<CXY> PointFullCombo;

		public STDGBSValue<CXY> OptionPanelSelect;

		/// <summary>
		/// シリアル化対象メンバはここで初期値を与えます。XMLが存在しない場合一度これらの値が書きだされ、
		/// もう一度デシリアライズされます。XMLが存在する場合、これらの値は全て無効になります。
		/// </summary>
		public CCoordinates()
		{
			ImgJudgeLine = new CRect(0, 0, 64, 16);

			ImgGtChipOpen = new CRect(72 * 3, 0, 72 * 3, 14);
			ImgGtRGBButton = new CRect(0, 0, 72, 14);
			ImgGtWailingChip = new CRect(0, 590, 60, 120);
			ImgGtCountWailingChip = 5;
			ImgGtCountRGBChip = 48;
			ImgGtCountOpenChip = 48;

			ImgDanger = new CRect(0, 0, 64, 256);

			ImgGtButtonR = new CRect(0, 0, 72, 72);
			ImgGtButtonG = new CRect(72, 0, 72, 72);
			ImgGtButtonB = new CRect(144, 0, 72, 72);
			ImgGtPressingButtonR = new CRect(0, 72, 72, 72);
			ImgGtPressingButtonG = new CRect(72, 72, 72, 72);
			ImgGtPressingButtonB = new CRect(144, 72, 72, 72);
			ImgGtWailingBonus = new CRect(0, 0, 78, 549);

			ImgComboCombo = new CRect(100, 140, 110, 30);
			ImgComboOneDigit = new CRect(0, 0, 50, 70);

			ImgGtWailingFrame = new CRect(0, 0, 66, 113);
			ImgSpeedPanel = new CRect(0, 0, 45, 164);

			ImgConsoleFont = new CRect(0, 0, 16, 32);
			ImgJudgeString = new CRect(0, 0, 256, 36);

			ImgDrCountChip = 48;
			ImgDrChip = new STPadValue<CRect>();
			ImgDrChip.LC = new CRect(99 + 72 * 5, 16, 85, 18);
			ImgDrChip.BD = new CRect(0, 20, 99, 22);
			ImgDrChip.HH = new CRect(99, 16, 72, 18);
			ImgDrChip.SD = new CRect(99 + 72, 16, 72, 18);
			ImgDrChip.HT = new CRect(99 + 72 * 2, 16, 72, 18);
			ImgDrChip.LT = new CRect(99 + 72 * 3, 16, 72, 18);
			ImgDrChip.FT = new CRect(99 + 72 * 4, 16, 72, 18);
			ImgDrChip.CY = new CRect(99 + 72 * 5, 16, 85, 18);
			ImgDrChipCYDeco = new CRect(99 + 72 * 5, 786, 85, 64);
			ImgDrChip.RD = new CRect(99 + 72 * 5 + 85, 16, 58, 18);
			ImgDrChip.HHO = new CRect(99 + 72 * 5 + 85, 16, 58, 18);
			ImgDrChipHHODeco = new CRect(99 + 72 * 5 + 85, 791, 58, 30);

			ImgDrPad = new STPadValue<CRect>();
			ImgDrPad.LC = new CRect(0, 0, 170, 130);
			ImgDrPad.HH = new CRect(170, 0, 170, 130);
			ImgDrPad.SD = new CRect(340, 0, 170, 130);
			ImgDrPad.BD = new CRect(0, 130, 170, 130);
			ImgDrPad.HT = new CRect(170, 130, 170, 130);
			ImgDrPad.LT = new CRect(340, 130, 170, 130);
			ImgDrPad.FT = new CRect(0, 260, 170, 130);
			ImgDrPad.CY = new CRect(170, 260, 170, 130);
			ImgDrPad.RD = new CRect(340, 260, 170, 130);

			ImgBarLine = new CRect(0, 982, 480, 3);
			ImgBeatLine = new CRect(0, 985, 480, 2);

			ImgGaugeOrange = new CRect(48, 0, 48, 61);
			ImgGaugeNormal = new CRect(0, 0, 48, 61);
			ImgGaugeLight = new CRect(0, 0, 48, 576);
			ImgGaugeTopLight = new CRect(144, 0, 48, 36);
			ImgGaugeStar = new CRect(0, 72, 96, 72);

			ImgScoreOneDigit = new CRect(0, 0, 30, 38);

			// w無効
			ImgGtLaneFlash = new CRect(0, 0, -1, 512);
			// w無効
			ImgDrLaneFlash = new CRect(0, 0, -1, 768);

			ImgOptionPanel = new CWH(72, 27);

			// #24063 2011.1.27 yyagi
			OffsetGuitarBassSwap = new STDGBSValue<CY>[2];
			OffsetGuitarBassSwap[0] = new STDGBSValue<CY>();
			OffsetGuitarBassSwap[0].Drums = new CY(0);
			OffsetGuitarBassSwap[0].Guitar = new CY((int)(21 * Scale.Y));
			OffsetGuitarBassSwap[0].Bass = new CY((int)(42 * Scale.Y));
			OffsetGuitarBassSwap[1].Drums = new CY(0);
			OffsetGuitarBassSwap[1].Guitar = new CY((int)(42 * Scale.Y));
			OffsetGuitarBassSwap[1].Bass = new CY((int)(21 * Scale.Y));

			PointFullCombo = new STDGBSValue<CXY>();
			PointFullCombo.Drums = new CXY((int)(0x80 * Scale.X), (int)(0xed * Scale.Y));
			PointFullCombo.Guitar = new CXY((int)(0xdf * Scale.X), (int)(0xed * Scale.Y));
			PointFullCombo.Bass = new CXY((int)(0x141 * Scale.X), (int)(0xed * Scale.Y));

			DrPadOffset = new STPadValue<CY>();

			DrPadOffset.LC = new CY(0); // <- 非対象化
			DrPadOffset.HH = new CY(10);
			DrPadOffset.HHO = new CY(10);
			DrPadOffset.SD = new CY(22);
			DrPadOffset.BD = new CY(30);
			DrPadOffset.HT = new CY(0);
			DrPadOffset.LT = new CY(15);
			DrPadOffset.FT = new CY(30);
			DrPadOffset.CY = new CY(15);
			DrPadOffset.RD = new CY(80);
			DrPad = new CY(930);

			Instrument = new STDGBSValue<CW>();
			Instrument.Drums = new CW(72 * 5 + 85 * 2 + 99);
			Instrument.Guitar = new CW(72 * 4);
			Instrument.Bass = new CW(72 * 4);

			Gauge = new CXY(0, 1080); // <- 左固定
			GaugeMax = 1080;
			Danger = new CRange(0, SampleFramework.GameWindowSize.Width - 64);
			StatusPanel = new CW(45);
			Score = new STDGBSValue<CY>();
			Score.Drums = new CY(30);
			Score.Guitar = new CY(30);
			Score.Bass = new CY(30);

			Panel = new CXY(SampleFramework.GameWindowSize.Width, 0);
			Movie = new CWH(556, 710);

			LaneFlash = new STDGBSValue<CY>();
			LaneFlash.Drums = new CY(SampleFramework.GameWindowSize.Height - 768);
			LaneFlash.Guitar = new CY(0);
			LaneFlash.Bass = new CY(0);

			OptionPanelSelect = new STDGBSValue<CXY>();
			OptionPanelSelect.Drums = new CXY(1107, 23);
			OptionPanelSelect.Guitar = new CXY(1107, 50);
			OptionPanelSelect.Bass = new CXY(1107, 77);
			
			Graph.Drums = new CWH(62, 1080);
			Graph.Guitar = new CWH(62, 1080);
			Graph.Bass = new CWH(62, 1080);

		}
	}
}
