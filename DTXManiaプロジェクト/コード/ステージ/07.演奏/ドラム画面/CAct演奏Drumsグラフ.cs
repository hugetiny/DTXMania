using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏グラフ : CActivity
	{
		// #24074 2011.01.23 ikanick グラフの描画
		// 実装内容
		// ・左を現在、右を目標
		// ・基準線(60,70,80,90,100%)を超えると線が黄色くなる（元は白）
		// ・目標を超えると現在が光る
		// ・オート時には描画しない
		// 要望・実装予定
		// ・グラフを波打たせるなどの視覚の向上→実装済
		// 修正等
		// ・画像がないと落ちる→修正済

		public STDGBVALUE<double> dbTarget;
		private STDGBVALUE<double> dbTargetDisp;
		public STDGBVALUE<double> dbCurrent;
		private STDGBVALUE<double> dbCurrentDisp;

		private CTexture txグラフ;

		private STDGBVALUE<CCounter> counterYposInImg;
		private readonly int slices = 10;

		public CAct演奏グラフ()
		{
			base.b活性化してない = true;
		}

		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.dbTarget = new STDGBVALUE<double>();
				dbTarget.Drums = dbTarget.Guitar = dbTarget.Bass = 80.0;
				this.dbCurrent = new STDGBVALUE<double>();
				dbCurrent.Drums = dbCurrent.Guitar = dbCurrent.Bass = 0.0;
				counterYposInImg = new STDGBVALUE<CCounter>();
				base.On活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				string pathグラフ = CSkin.Path(@"Graphics\ScreenPlay graph.png");
				if (File.Exists(pathグラフ))
				{
					this.txグラフ = TextureFactory.tテクスチャの生成(pathグラフ);
				}
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txグラフ);
				base.OnManagedリソースの解放();
			}
		}
		
		public override int On進行描画()
		{
			if (base.b活性化してる)
			{

				for (E楽器パート inst = E楽器パート.DRUMS; inst <= E楽器パート.BASS; ++inst)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効[inst])
					{

						if (base.b初めての進行描画)
						{
							base.b初めての進行描画 = false;
							counterYposInImg.Drums = new CCounter(0, 35, 16, CDTXMania.Instance.Timer);
							counterYposInImg.Guitar = new CCounter(0, 35, 16, CDTXMania.Instance.Timer);
							counterYposInImg.Bass = new CCounter(0, 35, 16, CDTXMania.Instance.Timer);
						}

						counterYposInImg[inst].t進行Loop();
						int stYposInImg = counterYposInImg[inst].n現在の値;

						// 背景暗幕
						if (this.txグラフ != null)
						{
							this.txグラフ.vc拡大縮小倍率.X = CDTXMania.Instance.Coordinates.Graph[inst].W;
							this.txグラフ.vc拡大縮小倍率.Y = CDTXMania.Instance.Coordinates.Graph[inst].H;

							this.txグラフ.n透明度 = 128;
							this.txグラフ.t2D描画(
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Graph[inst].X,
								CDTXMania.Instance.Coordinates.Graph[inst].Y,
								new Rectangle(62, 0, 1, 1)
								);

							// 基準線

							this.txグラフ.n透明度 = 128;
							this.txグラフ.vc拡大縮小倍率.X = CDTXMania.Instance.Coordinates.Graph[inst].W;
							this.txグラフ.vc拡大縮小倍率.Y = 1f;

							for (int i = 0; i < slices; i++)
							{
								this.txグラフ.t2D描画(
									CDTXMania.Instance.Device,
									CDTXMania.Instance.Coordinates.Graph[inst].X,
									CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H * i / slices,
									new Rectangle(60, 0, 1, 1)
									);
							}

							for (int i = 0; i < 5; i++)
							{
								Rectangle rectangle;
								// 基準線を越えたら線が黄色くなる
								if (this.dbCurrent[inst] >= (100 - i * slices))
								{
									rectangle = new Rectangle(61, 0, 1, 1);	//黄色
									if (this.txグラフ != null)
									{
										this.txグラフ.n透明度 = 224;
									}
								}
								else
								{
									rectangle = new Rectangle(60, 0, 1, 1);
									if (this.txグラフ != null)
									{
										this.txグラフ.n透明度 = 160;
									}
								}

								if (this.txグラフ != null)
								{
									this.txグラフ.t2D描画(
										CDTXMania.Instance.Device,
										CDTXMania.Instance.Coordinates.Graph[inst].X,
										CDTXMania.Instance.Coordinates.Graph[inst].Y + i * CDTXMania.Instance.Coordinates.Graph[inst].H / slices,
										rectangle
										);
								}
							}

							// グラフ
							// --現在値
							if (this.dbCurrentDisp[inst] < this.dbCurrent[inst])
							{
								this.dbCurrentDisp[inst] += (this.dbCurrent[inst] - this.dbCurrentDisp[inst]) / 5 + 0.01;
							}
							if (this.dbCurrentDisp[inst] >= this.dbCurrent[inst])
							{
								this.dbCurrentDisp[inst] = this.dbCurrent[inst];
							}
							int ar = (int)(CDTXMania.Instance.Coordinates.Graph[inst].H * this.dbCurrentDisp[inst] / 100.0);

							this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
							this.txグラフ.n透明度 = 255;
							this.txグラフ.t2D描画(
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Graph[inst].X,
								CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H - ar,
								new Rectangle(0, 5 + stYposInImg, 30, ar)
								);
							this.txグラフ.t2D描画( // 上部白いバー
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Graph[inst].X,
								CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H - ar,
								new Rectangle(0, 0, 30, 5)
							);

							// --現在値_目標越
							if ((dbCurrent[inst] >= dbTarget[inst]))
							{
								// this.txグラフ.vc拡大縮小倍率 = new Vector3(1.4f, 1f, 1f);
								this.txグラフ.n透明度 = 128;
								this.txグラフ.b加算合成 = true;
								this.txグラフ.t2D描画(
									CDTXMania.Instance.Device,
									CDTXMania.Instance.Coordinates.Graph[inst].X,
									CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H - ar,
									new Rectangle(0, 5 + stYposInImg, 30, ar)
									);
								this.txグラフ.b加算合成 = false;
							}
							// --目標値
							if (this.dbTargetDisp[inst] < this.dbTarget[inst])
							{
								this.dbTargetDisp[inst] += (this.dbTarget[inst] - this.dbTargetDisp[inst]) / 5 + 0.01;
							}
							if (this.dbTargetDisp[inst] >= this.dbTarget[inst])
							{
								this.dbTargetDisp[inst] = this.dbTarget[inst];
							}
							ar = (int)(CDTXMania.Instance.Coordinates.Graph[inst].H * this.dbTargetDisp[inst] / 100.0);

							this.txグラフ.t2D描画(
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Graph[inst].X + 30,
								CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H - ar,
								new Rectangle(30, 5 + stYposInImg, 30, ar)
								);
							this.txグラフ.n透明度 = 255;
							this.txグラフ.t2D描画( // 上部白いバー
								CDTXMania.Instance.Device,
								CDTXMania.Instance.Coordinates.Graph[inst].X + 30,
								CDTXMania.Instance.Coordinates.Graph[inst].Y + CDTXMania.Instance.Coordinates.Graph[inst].H - ar,
								new Rectangle(30, 0, 30, 5)
							);
						}
					}
				}
			}
			return 0;
		}
	}
}
