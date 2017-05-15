using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	internal class CActFIFOBlack : CActivity
	{
		private CCounter counter;
		private EFIFOMode mode;
		private CTexture tx黒タイル64x64;

		public void tフェードアウト開始()
		{
			this.mode = EFIFOMode.フェードアウト;
			this.counter = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
		}

		public void tフェードイン開始()
		{
			this.mode = EFIFOMode.フェードイン;
			this.counter = new CCounter(0, 100, 5, CDTXMania.Instance.Timer);
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.tx黒タイル64x64);
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.tx黒タイル64x64 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\Tile black 64x64.png"), false);
				base.OnManagedリソースの作成();
			}
		}
		public override int On進行描画()
		{
			if (base.b活性化してる && this.counter != null)
			{
				this.counter.t進行();
				// #23510 2010.10.31 yyagi: delete as of no one use this any longer.
				// Size clientSize = CDTXMania.Instance.app.Window.ClientSize;
				if (this.tx黒タイル64x64 != null)
				{
					this.tx黒タイル64x64.n透明度 = (this.mode == EFIFOMode.フェードイン) ? (((100 - this.counter.n現在の値) * 0xff) / 100) : ((this.counter.n現在の値 * 0xff) / 100);
					// #23510 2010.10.31 yyagi: change "clientSize.Width" to "640" to fix FIFO drawing size
					for (int i = 0; i <= (SampleFramework.GameWindowSize.Width / 64); i++)
					{
						// #23510 2010.10.31 yyagi: change "clientSize.Height" to "480" to fix FIFO drawing size
						for (int j = 0; j <= (SampleFramework.GameWindowSize.Height / 64); j++)
						{
							this.tx黒タイル64x64.t2D描画(CDTXMania.Instance.Device, i * 64, j * 64);
						}
					}
				}
				if (this.counter.n現在の値 != 100)
				{
					return 0;
				}
				return 1;
			}
			return 0;
		}
	}
}
