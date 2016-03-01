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
	internal class CAct演奏Drumsグラフ : CActivity
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

		private double dbグラフ値目標;
		private double dbグラフ値目標_表示;
		private double dbグラフ値現在;
		private double dbグラフ値現在_表示;

		private CTexture txグラフ;
		private int XPos = 3 * 345;
		private int YPos = 200;
		private int DispHeight = 400;
		private int DispWidth = 60;
		private CCounter counterYposInImg = null;
		private readonly int slices = 10;

		// プロパティ

		public double dbグラフ値現在_渡
		{
			get
			{
				return this.dbグラフ値現在;
			}
			set
			{
				this.dbグラフ値現在 = value;
			}
		}
		public double dbグラフ値目標_渡
		{
			get
			{
				return this.dbグラフ値目標;
			}
			set
			{
				this.dbグラフ値目標 = value;
			}
		}

		// コンストラクタ

		public CAct演奏Drumsグラフ()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.dbグラフ値目標 = 80f;
			this.dbグラフ値現在 = 0f;
			base.On活性化();
		}
		public override void On非活性化()
		{
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
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
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txグラフ);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					if (base.b初めての進行描画)
					{
						base.b初めての進行描画 = false;
						counterYposInImg = new CCounter(0, 35, 16, CDTXMania.Instance.Timer);
					}

					counterYposInImg.t進行Loop();
					int stYposInImg = counterYposInImg.n現在の値;

					// レーン表示位置によって変更
					if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center)
					{
						XPos = 1350;
					}

					// 背景暗幕
					if (this.txグラフ != null)
					{
						this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, DispHeight, 1f);
						this.txグラフ.n透明度 = 128;
						this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos,
							YPos,
							new Rectangle(62, 0, 1, 1)
							);
					}

					// 基準線

					if (this.txグラフ != null)
					{
						this.txグラフ.n透明度 = 128;
						this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, 1f, 1f);
						for (int i = 0; i < slices; i++)
						{
							this.txグラフ.t2D描画(
								CDTXMania.Instance.Device,
								XPos,
								YPos + DispHeight * i / slices,
								new Rectangle(60, 0, 1, 1)
								);
						}
					}

					if (this.txグラフ != null)
					{
						this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, 1f, 1f);
					}
					for (int i = 0; i < 5; i++)
					{
						Rectangle rectangle;
						// 基準線を越えたら線が黄色くなる
						if (this.dbグラフ値現在 >= (100 - i * slices))
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
								XPos,
								YPos + i * DispHeight / slices,
								rectangle
								);
						}
					}
					// グラフ
					// --現在値
					if (this.dbグラフ値現在_表示 < this.dbグラフ値現在)
					{
						this.dbグラフ値現在_表示 += (this.dbグラフ値現在 - this.dbグラフ値現在_表示) / 5 + 0.01;
					}
					if (this.dbグラフ値現在_表示 >= this.dbグラフ値現在)
					{
						this.dbグラフ値現在_表示 = this.dbグラフ値現在;
					}
					int ar = (int)(DispHeight * this.dbグラフ値現在_表示 / 100.0);

					if (this.txグラフ != null)
					{
						this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
						this.txグラフ.n透明度 = 255;
						this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos,
							YPos + DispHeight - ar,
							new Rectangle(0, 5 + stYposInImg, 30, ar)
							);
						this.txグラフ.t2D描画( // 上部白いバー
							CDTXMania.Instance.Device,
							XPos,
							YPos + DispHeight - ar,
							new Rectangle(0, 0, 30, 5)
						);
					}

					// --現在値_目標越
					if ((dbグラフ値現在 >= dbグラフ値目標) && (this.txグラフ != null))
					{
						// this.txグラフ.vc拡大縮小倍率 = new Vector3(1.4f, 1f, 1f);
						this.txグラフ.n透明度 = 128;
						this.txグラフ.b加算合成 = true;
						this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos,
							YPos + DispHeight - ar,
							new Rectangle(0, 5 + stYposInImg, 30, ar)
							);
						this.txグラフ.b加算合成 = false;
					}
					// --目標値
					if (this.dbグラフ値目標_表示 < this.dbグラフ値目標)
					{
						this.dbグラフ値目標_表示 += (this.dbグラフ値目標 - this.dbグラフ値目標_表示) / 5 + 0.01;
					}
					if (this.dbグラフ値目標_表示 >= this.dbグラフ値目標)
					{
						this.dbグラフ値目標_表示 = this.dbグラフ値目標;
					}
					ar = (int)(DispHeight * this.dbグラフ値目標_表示 / 100.0);

					if (this.txグラフ != null)
					{
						this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos + 30,
							YPos + DispHeight - ar,
							new Rectangle(30, 5 + stYposInImg, 30, ar)
							);
						this.txグラフ.n透明度 = 255;
						this.txグラフ.t2D描画( // 上部白いバー
							CDTXMania.Instance.Device,
							XPos + 30,
							YPos + DispHeight - ar,
							new Rectangle(30, 0, 30, 5)
						);
					}
				}
			}
			return 0;
		}
	}
}
