using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Diagnostics;

namespace DTXMania
{
	internal class CAct演奏演奏情報 : CActivity
	{
		// プロパティ

		public double dbBPM;
		public int n小節番号;


		// コンストラクタ

		public CAct演奏演奏情報()
		{
			base.b活性化してない = true;
		}

				
		// CActivity 実装

		public override void On活性化()
		{
			this.n小節番号 = 0;
			this.dbBPM = CDTXMania.app.DTX.BASEBPM + CDTXMania.app.DTX.BPM;
			base.On活性化();
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(int x, int y) のほうを使用してください。" );
		}
		public void t進行描画( int x, int y )
		{
			x = (int)(x * Scale.X);
			y = (int)(y * Scale.Y);
			if ( !base.b活性化してない )
			{
				y += (int)(0x153 * Scale.Y);
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白,
					string.Format( "JudgeLine D/G/B Adj: {0} px", CDTXMania.app.ConfigIni.nJudgeLinePosOffset.Drums, CDTXMania.app.ConfigIni.nJudgeLinePosOffset.Guitar, CDTXMania.app.ConfigIni.nJudgeLinePosOffset.Bass ) );
				y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "BGM/D/G/B Adj: {0:####0}/{1:####0}/{2:####0}/{3:####0} ms", CDTXMania.app.DTX.nBGMAdjust, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Drums, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Guitar, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Bass ) );
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "BGM/D/G/B Adj: {0}/{1}/{2}/{3} ms", CDTXMania.app.DTX.nBGMAdjust, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Drums, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Guitar, CDTXMania.app.ConfigIni.nInputAdjustTimeMs.Bass ) );
				y -= 0x10 * 2;
				int num = ( CDTXMania.app.DTX.listChip.Count > 0 ) ? CDTXMania.app.DTX.listChip[ CDTXMania.app.DTX.listChip.Count - 1 ].n発声時刻ms : 0;
				string str = "Time:          " + ( ( ( ( double ) CDTXMania.app.Timer.n現在時刻 ) / 1000.0 ) ).ToString( "####0.00" ) + " / " + ( ( ( ( double ) num ) / 1000.0 ) ).ToString( "####0.00" );
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, str );
				y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Part:          {0:####0}", this.n小節番号 ) );
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Part:          {0}", this.n小節番号 ) );
				y -= 0x10 * 2;
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "BPM:           {0:####0.00}", this.dbBPM ) );
				y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Frame:         {0:####0} fps", CDTXMania.app.FPS.n現在のFPS ) );
				CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Frame:         {0} fps", CDTXMania.app.FPS.n現在のFPS ) );
				y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound CPU :    {0:####0.00}%", CDTXMania.app.Sound管理.GetCPUusage() ) );
				//y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound Mixing:  {0:####0}", CDTXMania.app.Sound管理.GetMixingStreams() ) );
				//y -= 0x10 * 2;
				//CDTXMania.app.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.白, string.Format( "Sound Streams: {0:####0}", CDTXMania.app.Sound管理.GetStreams() ) );
				//y -= 0x10 * 2;
			}
		}
	}
}
