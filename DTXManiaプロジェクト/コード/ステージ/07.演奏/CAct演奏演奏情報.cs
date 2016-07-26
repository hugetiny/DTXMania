using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Diagnostics;

namespace DTXMania
{
	internal class CAct演奏演奏情報 : CActivity
	{
		public double dbBPM;
		public int n小節番号;

		public override void On活性化()
		{
			if (b活性化してない)
			{
				this.n小節番号 = 0;
				this.dbBPM = CDTXMania.Instance.DTX.BASEBPM + CDTXMania.Instance.DTX.BPM;
				base.On活性化();
			}
		}

		public override int On進行描画()
		{
			if (b活性化してる &&
				CDTXMania.Instance.ConfigIni.b演奏情報を表示する)
			{
				int x = CDTXMania.Instance.ConfigIni.cdDebugX[CDTXMania.Instance.ConfigIni.eActiveInst];
				int y = CDTXMania.Instance.ConfigIni.cdDebugY[CDTXMania.Instance.ConfigIni.eActiveInst];
				y += (int) ( 0x153 * Scale.Y );
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白,
					string.Format("JudgeLine D/G/B Adj: {0} px", CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums, CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar, CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass));
				y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "BGM/D/G/B Adj: {0:####0}/{1:####0}/{2:####0}/{3:####0} ms", CDTXMania.Instance.DTX.nBGMAdjust, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Guitar, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Bass ) );
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("BGM/D/G/B Adj: {0}/{1}/{2}/{3} ms", CDTXMania.Instance.DTX.nBGMAdjust, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Guitar, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Bass));
				y -= 0x10 * 2;
				int num = (CDTXMania.Instance.DTX.listChip.Count > 0) ? CDTXMania.Instance.DTX.listChip[CDTXMania.Instance.DTX.listChip.Count - 1].n発声時刻ms : 0;
				string str = "Time:          " + ((((double)CDTXMania.Instance.Timer.n現在時刻) / 1000.0)).ToString("####0.00") + " / " + ((((double)num) / 1000.0)).ToString("####0.00");
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, str);
				y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Part:          {0:####0}", this.n小節番号 ) );
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("Part:          {0}", this.n小節番号));
				y -= 0x10 * 2;
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("BPM:           {0:####0.00}", this.dbBPM));
				y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Frame:         {0:####0} fps", CDTXMania.Instance.FPS.n現在のFPS ) );
				CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("Frame:         {0} fps", CDTXMania.Instance.FPS.n現在のFPS));
				//y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound CPU :    {0:####0.00}%", CDTXMania.Instance.Sound管理.GetCPUusage() ) );
				//y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("Sound Mixing:  {0:####0}", CDTXMania.Instance.Sound管理.GetMixingStreams()));
				//y -= 0x10 * 2;
				//CDTXMania.Instance.act文字コンソール.tPrint(x, y, C文字コンソール.Eフォント種別.白, string.Format("Sound Streams: {0:####0}", CDTXMania.Instance.Sound管理.GetStreams()));
				//y -= 0x10 * 2;
			}
			return 0;
		}
	}
}
