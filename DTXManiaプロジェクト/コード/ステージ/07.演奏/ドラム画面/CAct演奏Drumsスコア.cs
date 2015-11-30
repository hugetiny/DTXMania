﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏Drumsスコア : CAct演奏スコア共通
	{
		// CActivity 実装（共通クラスからの差分のみ）

		public override unsafe int On進行描画()
    {
        if (!base.b活性化してない)
        {
            if (base.b初めての進行描画)
            {
                base.n進行用タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
                base.b初めての進行描画 = false;
            }
            long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
            if (num < base.n進行用タイマ)
            {
                base.n進行用タイマ = num;
            }
            while ((num - base.n進行用タイマ) >= 10)
            {
                for (int j = 0; j < 3; j++)
                {
					this.n現在表示中のスコア[j] += this.nスコアの増分[j];

					if (this.n現在表示中のスコア[j] > (long) this.n現在の本当のスコア[j])
                        this.n現在表示中のスコア[j] = (long) this.n現在の本当のスコア[j];
                }
                base.n進行用タイマ += 10;
            }
            string str = this.n現在表示中のスコア.Drums.ToString("0000000000");
            for (int i = 0; i < 10; i++)
            {
                Rectangle rectangle;
                char ch = str[i];
				if ( ch.Equals( ' ' ) )
				{
					rectangle = new Rectangle(
						0,
						0,
						(int) ( 12 * Scale.X ),
						(int) ( 0x18 * Scale.Y )
					);
				}
				else
				{
					int num4 = int.Parse( str.Substring( i, 1 ) );
					if ( num4 < 5 )
					{
						rectangle = new Rectangle(
							(int) ( num4 * 12 * Scale.X ),
							0,
							(int) ( 12 * Scale.X ),
							(int) ( 0x18 * Scale.Y )
						);
					}
					else
					{
						rectangle = new Rectangle(
							(int) ( ( num4 - 5 ) * 12 * Scale.X ),
							(int) ( 0x18 * Scale.Y ),
							(int) ( 12 * Scale.X ),
							(int) ( 0x18 * Scale.Y )
						);
					}
				}
				if ( base.txScore != null )
				{
					base.txScore.t2D描画(
						CDTXMania.app.Device,
						(CDTXMania.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)? 1068 + ( i * 36 ) : 1490 + i * 36,
						30,
						rectangle
					);
				}
            }
        }
        return 0;
    }
	}
}
