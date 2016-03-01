using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏RGB共通 : CActivity
	{
		// プロパティ
		protected bool[] b押下状態 = new bool[6];
		protected CTexture txRGB;


		// コンストラクタ

		public CAct演奏RGB共通()
		{
			base.b活性化してない = true;
		}

		// メソッド
		public void Push(int nLane)
		{
			this.b押下状態[nLane] = true;
		}


		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.b押下状態[i] = false;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txRGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay RGB buttons.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txRGB);
				base.OnManagedリソースの解放();
			}
		}

		public virtual int t進行描画(C演奏判定ライン座標共通 演奏判定ライン座標)
		{
			if (!base.b活性化してない)
			{
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					if (!CDTXMania.Instance.ConfigIni.bGuitar有効)
					{
						return 0;
					}
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						for (int j = 0; j < 3; j++)
						{
							int index = CDTXMania.Instance.ConfigIni.bLeft.Guitar ? (2 - j) : j;
							Rectangle rectangle = new Rectangle(
								(int)(index * 0x18 * Scale.X),
								0,
								(int)(0x18 * Scale.X),
								(int)(0x20 * Scale.Y)
							);
							if (b押下状態[index])
							{
								rectangle.Y += (int)(0x20 * Scale.Y);
							}
							if (txRGB != null)
							{
								int y = 演奏判定ライン座標.n演奏RGBボタンY座標(E楽器パート.GUITAR, true, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
								txRGB.t2D描画(
									CDTXMania.Instance.Device,
									(0x1f + (j * 0x24)) * Scale.X,
									y,
									rectangle
								);
							}
						}
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						for (int k = 0; k < 3; k++)
						{
							int index = CDTXMania.Instance.ConfigIni.bLeft.Bass ? (2 - k) : k;
							Rectangle rectangle2 = new Rectangle(
								(int)(index * 0x18 * Scale.X),
								0,
								(int)(0x18 * Scale.X),
								(int)(0x20 * Scale.Y)
							);
							if (b押下状態[index + 3])
							{
								rectangle2.Y += (int)(0x20 * Scale.Y);
							}
							if (txRGB != null)
							{
								int y = 演奏判定ライン座標.n演奏RGBボタンY座標(E楽器パート.BASS, true, CDTXMania.Instance.ConfigIni.bReverse.Bass);
								txRGB.t2D描画(
									CDTXMania.Instance.Device,
									(0x1e5 + (k * 0x24)) * Scale.X,
									y,
									rectangle2
								);
							}
						}
					}
					for (int i = 0; i < 6; i++)
					{
						b押下状態[i] = false;
					}
				}
				else
				{
					if (!CDTXMania.Instance.ConfigIni.bGuitar有効)
					{
						return 0;
					}
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1527 : 1456;
						for (int i = 0; i < 3; i++)
						{
							int index = CDTXMania.Instance.ConfigIni.bLeft.Guitar ? (2 - i) : i;
							Rectangle rc = new Rectangle(
								index * 72,
								0,
								72,
								72
							);
							if (b押下状態[index])
							{
								rc.Y += 72;
							}
							if (txRGB != null)
							{
								int y = 演奏判定ライン座標.n演奏RGBボタンY座標(E楽器パート.GUITAR, false, CDTXMania.Instance.ConfigIni.bReverse.Guitar);
								txRGB.t2D描画(
									CDTXMania.Instance.Device,
									x + (i * 26 * 3),
									y,
									rc
								);
							}
						}
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						for (int i = 0; i < 3; i++)
						{
							int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1200 : 206;
							int index = CDTXMania.Instance.ConfigIni.bLeft.Bass ? (2 - i) : i;
							Rectangle rc = new Rectangle(
								index * 72,
								0,
								72,
								72
							);
							if (b押下状態[index + 3])
							{
								rc.Y += 72;
							}
							if (txRGB != null)
							{
								int y = 演奏判定ライン座標.n演奏RGBボタンY座標(E楽器パート.BASS, false, CDTXMania.Instance.ConfigIni.bReverse.Bass);
								txRGB.t2D描画(
									CDTXMania.Instance.Device,
									x + (i * 26 * 3),
									y,
									rc
								);
							}
						}
					}
					for (int i = 0; i < 6; i++)
					{
						b押下状態[i] = false;
					}
				}
			}
			return 0;
		}
	}
}
