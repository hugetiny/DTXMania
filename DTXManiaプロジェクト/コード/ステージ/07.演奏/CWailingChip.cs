using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// Wailingチップの座標計算と描画を行う基底クラス
	/// </summary>
	public class CWailingChip共通
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CWailingChip共通()
		{
		}

		/// <summary>
		/// 描画処理 (引数が多いのは追々何とかする)
		/// </summary>
		/// <param name="pChip"></param>
		/// <param name="txチップ"></param>
		/// <param name="演奏判定ライン座標"></param>
		/// <param name="ctWailingチップ模様アニメ"></param>
		internal void t進行描画_チップ_ウェイリング(ref CChip pChip, ref CTexture txチップGB, ref CCounter ctWailingチップ模様アニメ)
		{
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				if (!pChip.bHit && pChip.b可視)
				{
					if (txチップGB != null)
					{
						txチップGB.n透明度 = pChip.n透明度;
					}

					EPart gb = pChip.bGuitar可視チップ_Wailing含む ? EPart.Guitar : EPart.Bass;

					int x = CDTXMania.Instance.ConfigIni.GetLaneX(gb == EPart.Guitar ? ELane.GtW : ELane.BsW)
							+ (CDTXMania.Instance.ConfigIni.GetLaneW(gb == EPart.Guitar ? ELane.GtW : ELane.BsW)
							- CDTXMania.Instance.Coordinates.ImgGtWailingChip.W) / 2;

					int y = C演奏判定ライン座標共通.n判定ラインY座標(gb);

					if (CDTXMania.Instance.ConfigIni.bReverse[gb])
					{
						y = y - (int)(pChip.nバーからの距離dot[gb]);
					}
					else
					{
						y = y + (int)(pChip.nバーからの距離dot[gb]);
					}

					int numA = (int)(26 * Scale.Y);			// wailing chipの高さの半分
					int showRangeY1 = SampleFramework.GameWindowSize.Height;
					int numB = y - (int)(0x39 * Scale.Y);
					int numC = 0;
					if ((numB < (showRangeY1 + numA)) && (numB > -numA))
					{
						int c = ctWailingチップ模様アニメ.n現在の値 % CDTXMania.Instance.Coordinates.ImgGtCountWailingChip;
						Rectangle rect = CDTXMania.Instance.Coordinates.ImgGtWailingChip.ApplyCounterX(c, 0);
						txチップGB.t2D描画(
								CDTXMania.Instance.Device,
								x,
								((y - numA) + numC),
								rect
							);
					}
				}
				return;
			}
			pChip.bHit = true;
		}
	}
}
