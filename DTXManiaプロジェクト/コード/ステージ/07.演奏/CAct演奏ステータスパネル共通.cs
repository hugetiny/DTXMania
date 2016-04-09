using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	internal class CAct演奏ステータスパネル共通 : CActivity
	{
		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.txStatusPanels = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay status panels right.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txStatusPanels);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (b活性化してる)
			{
				if (txStatusPanels != null)
				{
					for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
					{
						if (CDTXMania.Instance.ConfigIni.b楽器有効(inst) && CDTXMania.Instance.DTX.bチップがある[inst])
						{
							int spd = CDTXMania.Instance.ConfigIni.nScrollSpeed[inst];
							if (spd < 0)
							{
								spd = 0;
							}
							if (spd > 15)
							{
								spd = 15;
							}
							int y = 0;
							if (CDTXMania.Instance.ConfigIni.bReverse[inst])
							{
								y = SampleFramework.GameWindowSize.Height - (y + CDTXMania.Instance.Coordinates.ImgSpeedPanel.H);
							}
							this.txStatusPanels.t2D描画(CDTXMania.Instance.Device,
								CDTXMania.Instance.ConfigIni.cdInstX[inst][CDTXMania.Instance.ConfigIni.eActiveInst].Value
								- CDTXMania.Instance.Coordinates.StatusPanel.W,
								y,
								CDTXMania.Instance.Coordinates.ImgSpeedPanel.ApplyCounterXY(spd % 11, spd / 11, 0, 0));
						}
					}
				}
			}
			return 0;
		}

		private CTexture txStatusPanels;
	}
}
