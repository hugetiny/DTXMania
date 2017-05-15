using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DTXMania
{
	/// <summary>
	/// 判定ライン関係の座標処理をまとめたクラス
	/// </summary>
	internal class C演奏判定ライン座標共通
	{
		/// <summary>
		/// 表示位置の補正データ
		/// 初期化は外部から行うこと。
		/// </summary>
		//public STDGBVALUE<int> nJudgeLinePosYDelta;

		public C演奏判定ライン座標共通()
		{
			// 補正値は、Normal/Reverse, Drums/GR画面共通
			//nJudgeLinePosYDelta.Drums = 0;
			//nJudgeLinePosYDelta.Guitar = 0;
			//nJudgeLinePosYDelta.Bass = 0;
		}

		public enum Reverse
		{
			UseConfig,
			Reverse,
			NotReverse
		}

		/// <summary>
		/// 判定ラインのY座標を返す。
		/// </summary>
		/// <param name="eInst">E楽器パート</param>
		/// <param name="bGRmode">GRmodeか否か</param>
		/// <param name="bReverse">Reverseか否か</param>
		/// <param name="bWailingFrame">Wailing枠の座標か、判定ラインの座標か</param>
		/// <param name="b補正あり">プレーヤーのライン表示位置補正情報を加えるかどうか</param>
		/// <returns></returns>
		public static int n判定ラインY座標(EPart eInst, bool bWailingFrame = false, bool b補正あり = false, Reverse rt = Reverse.UseConfig)
		{
			if (eInst == EPart.Unknown)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				int ret = CDTXMania.Instance.ConfigIni.cdJudgeLineY[eInst];
				int delta = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[eInst];

				bool rev = false;
				if (rt == Reverse.Reverse)
				{
					rev = true;
				}
				else if (rt == Reverse.UseConfig)
				{
					rev = CDTXMania.Instance.ConfigIni.bReverse[eInst];
				}

				if (rev)
				{
					ret = SampleFramework.GameWindowSize.Height - ret;
					if (bWailingFrame)
					{
						ret -= CDTXMania.Instance.Coordinates.ImgGtWailingFrame.H / 2;
					}
					if (eInst == EPart.Drums)
					{
						delta = -delta;
					}
				}
				else
				{
					if (bWailingFrame)
					{
						ret -= CDTXMania.Instance.Coordinates.ImgGtWailingFrame.H / 2;
					}
					if (eInst != EPart.Drums)
					{
						delta = -delta;
					}
				}
				if (!b補正あり)
				{
					delta = 0;
				}
				return ret + delta;
			}
		}

		public static int n演奏RGBボタンY座標(EPart eInst)
		{
			if (eInst == EPart.Drums)
			{
				throw new NotImplementedException();
			}
			else if (eInst == EPart.Unknown)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				int maxButtonH = Math.Max(CDTXMania.Instance.Coordinates.ImgGtButtonR.H, CDTXMania.Instance.Coordinates.ImgGtButtonG.H);
				maxButtonH = Math.Max(maxButtonH, CDTXMania.Instance.Coordinates.ImgGtButtonB.H);
				int ret = n判定ラインY座標(eInst, false, true);
				if (CDTXMania.Instance.ConfigIni.bReverse[eInst])
				{
					ret += CDTXMania.Instance.Coordinates.ImgJudgeLine.H;
				}
				else
				{
					ret -= maxButtonH;
				}
				return ret;
			}
		}
	}
}
