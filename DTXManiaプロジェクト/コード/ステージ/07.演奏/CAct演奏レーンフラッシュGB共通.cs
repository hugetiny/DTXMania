using System;
using System.Collections.Generic;
using System.Text;
using FDK;
using System.Drawing;

namespace DTXMania
{
	internal class CAct演奏レーンフラッシュGB共通 : CActivity
	{
		protected CCounter[] ct進行 = new CCounter[6];
		protected CTextureAf[] txFlush = new CTextureAf[6];
		private readonly int[,,] nRGBのX座標 = new int[,,]
		{ 
		{ { 2, 0x1c, 0x36, 2, 0x1c, 0x36 }, { 0x36, 0x1c, 2, 0x36, 0x1c, 2 } }, // dr. 		
		{ { 0, 0x24, 0x48, 0, 0x24, 0x48 }, { 0x48, 0x24, 0, 0x48, 0x24, 0 } }  // gt.
		};

		// コンストラクタ
		public CAct演奏レーンフラッシュGB共通()
		{
			base.b活性化してない = true;
		}


		// メソッド
		public void Start(int nLane)
		{
			if ((nLane < 0) || (nLane > 6))
			{
				throw new IndexOutOfRangeException("有効範囲は 0～6 です。");
			}
			this.ct進行[nLane] = new CCounter(0, 100, 1, CDTXMania.Instance.Timer);
		}


		// CActivity 実装
		public override void On活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.ct進行[i] = new CCounter();
			}
			base.On活性化();
		}

		public override void On非活性化()
		{
			for (int i = 0; i < 6; i++)
			{
				this.ct進行[i] = null;
			}
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txFlush[0] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush red.png"));
				this.txFlush[1] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush green.png"));
				this.txFlush[2] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush blue.png"));
				this.txFlush[3] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush red reverse.png"));
				this.txFlush[4] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush green reverse.png"));
				this.txFlush[5] = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenPlay lane flush blue reverse.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 6; i++)
				{
					TextureFactory.tテクスチャの解放(ref this.txFlush[i]);
				}
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					for (int i = 0; i < 6; i++)
					{
						if (!ct進行[i].b停止中)
						{
							E楽器パート e楽器パート = (i < 3) ? E楽器パート.GUITAR : E楽器パート.BASS;
							CTextureAf texture = CDTXMania.Instance.ConfigIni.bReverse[(int)e楽器パート] ? txFlush[(i % 3) + 3] : txFlush[i % 3];
							int bLeft = CDTXMania.Instance.ConfigIni.bLeft[(int)e楽器パート] ? 1 : 0;
							int x = (((i < 3) ? 1521 : 1194) + this.nRGBのX座標[0,bLeft, i] * 3) + ((16 * ct進行[i].n現在の値) / 100) * 3;
							if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
							{
								x -= (e楽器パート == E楽器パート.GUITAR) ? 71 : 994;
							}
							int y = ((i < 3) ? 0x39 : 0x39);
							if (texture != null)
							{
								texture.t2D描画(
									CDTXMania.Instance.Device,
									x,
									y * Scale.Y,
									new Rectangle(
										0, //(int) ( ( j * 0x20 ) * Scale.X ),
										0,
										(int)((0x18 * (100 - ct進行[i].n現在の値)) / 100 * Scale.X),
										(int)(0x76 * 3 * Scale.Y)
									)
								);
							}
							ct進行[i].t進行();
							if (ct進行[i].b終了値に達した)
							{
								ct進行[i].t停止();
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < 6; i++)
					{
						if (!ct進行[i].b停止中)
						{
							E楽器パート e楽器パート = (i < 3) ? E楽器パート.GUITAR : E楽器パート.BASS;
							CTextureAf texture = CDTXMania.Instance.ConfigIni.bReverse[(int)e楽器パート] ? txFlush[(i % 3) + 3] : txFlush[i % 3];
							int num2 = CDTXMania.Instance.ConfigIni.bLeft[(int)e楽器パート] ? 1 : 0;
							{
								int x = (((i < 3) ? 0x1a : 480) + this.nRGBのX座標[1,num2, i]) + ((0x10 * ct進行[i].n現在の値) / 100);
								int y = CDTXMania.Instance.ConfigIni.bReverse[(int)e楽器パート] ? 0x37 : 0;
								if (texture != null)
								{
									texture.t2D描画(
										CDTXMania.Instance.Device,
										x * Scale.X,
										y * Scale.Y,
										new Rectangle(
											0,
											0,
											(int)(((0x20 * (100 - ct進行[i].n現在の値)) / 100) * Scale.X),
											(int)(0x76 * 3 * Scale.Y)
										)
									);
								}
							}
							ct進行[i].t進行();
							if (ct進行[i].b終了値に達した)
							{
								ct進行[i].t停止();
							}
						}
					}
				}
			}
			return 0;
		}
	}
}
