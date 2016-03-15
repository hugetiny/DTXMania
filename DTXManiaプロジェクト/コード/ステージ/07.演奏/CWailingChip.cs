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

					E楽器パート gb = pChip.bGuitar可視チップ_Wailing含む ? E楽器パート.GUITAR : E楽器パート.BASS;

					int x = 0;
					if (gb == E楽器パート.GUITAR)
					{
						x = CDTXMania.Instance.Coordinates.Lane.GtW.X + (CDTXMania.Instance.Coordinates.Lane.GtW.W - CDTXMania.Instance.Coordinates.ImgGtWailingChip.W) / 2;
					}
					else if (gb == E楽器パート.BASS)
					{
						x = CDTXMania.Instance.Coordinates.Lane.BsW.X + (CDTXMania.Instance.Coordinates.Lane.BsW.W - CDTXMania.Instance.Coordinates.ImgGtWailingChip.W) / 2;
					}

					int y = C演奏判定ライン座標共通.n判定ラインY座標(gb);

					if (CDTXMania.Instance.ConfigIni.bReverse[gb])
					{
						y = y - (int)(pChip.nバーからの距離dot[gb]);
					}
					else
					{
						y = y + (int)(pChip.nバーからの距離dot[gb]);
					}

					int numA = (int)(26 * Scale.Y);			// ドラム画面かギター画面かで変わる値
					int showRangeY1 = (int)(355 * Scale.Y);	// ドラム画面かギター画面かで変わる値
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
