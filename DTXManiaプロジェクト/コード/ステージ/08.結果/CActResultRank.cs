using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CActResultRank : CActivity
	{
		// コンストラクタ

		public CActResultRank()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ctランク表示.n現在の値 = this.ctランク表示.n終了値;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n本体X = 0x1ed;
			this.n本体Y = 0x153;
			base.On活性化();
		}
		public override void On非活性化()
		{
			if (this.ct白波移動 != null)
			{
				this.ct白波移動 = null;
			}
			if (this.ctランク表示 != null)
			{
				this.ctランク表示 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txランクパネル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rank panel.png"));
				this.tx白波 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rank wave.png"));
				switch (CDTXMania.Instance.stage結果.n総合ランク値)
				{
					case CScoreIni.ERANK.SS:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankSS.png"));
						break;

					case CScoreIni.ERANK.S:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankS.png"));
						break;

					case CScoreIni.ERANK.A:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankA.png"));
						break;

					case CScoreIni.ERANK.B:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankB.png"));
						break;

					case CScoreIni.ERANK.C:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankC.png"));
						break;

					case CScoreIni.ERANK.D:
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankD.png"));
						break;

					case CScoreIni.ERANK.E:
					case CScoreIni.ERANK.UNKNOWN:	// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
						this.txランク文字 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenResult rankE.png"));
						break;

					default:
						this.txランク文字 = null;
						break;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txランクパネル);
				TextureFactory.tテクスチャの解放(ref this.tx白波);
				TextureFactory.tテクスチャの解放(ref this.txランク文字);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (base.b活性化してない)
			{
				return 0;
			}
			if (base.b初めての進行描画)
			{
				this.ct白波移動 = new CCounter(-132, 0x170, 10, CDTXMania.Instance.Timer);
				this.ctランク表示 = new CCounter(0, 0x514, 2, CDTXMania.Instance.Timer);
				base.b初めての進行描画 = false;
			}
			this.ct白波移動.t進行Loop();
			this.ctランク表示.t進行();
			if (this.ctランク表示.n現在の値 >= 700)
			{
				float y = (this.ctランク表示.n現在の値 > 0x3e8) ? 1f : (((float)(this.ctランク表示.n現在の値 - 700)) / 300f);
				if (this.txランクパネル != null)
				{
					this.txランクパネル.n透明度 = (int)(255f * y);
					this.txランクパネル.vc拡大縮小倍率 = new Vector3(1f, y, 1f);
					this.txランクパネル.t2D描画(
						CDTXMania.Instance.Device,
						this.n本体X * Scale.X,
						(this.n本体Y + ((int)((1f - y) * 64f))) * Scale.Y
					);
				}
			}
			if (this.ctランク表示.n現在の値 >= 0x3e8)
			{
				double num2 = ((double)(this.ctランク表示.n現在の値 - 0x3e8)) / 300.0;
				if (this.txランク文字 != null)
				{
					this.txランク文字.t2D描画(
						CDTXMania.Instance.Device,
						this.n本体X * Scale.X,
						this.n本体Y * Scale.Y,
						new Rectangle(
							0,
							0,
							(int)(128.0 * num2 * Scale.X),
							(int)(0x80 * Scale.Y)
						)
					);
				}
			}
			this.t描画_白波();
			if (!this.ctランク表示.b終了値に達した)
			{
				return 0;
			}
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ctランク表示;
		private CCounter ct白波移動;
		private int n本体X;
		private int n本体Y;
		private CTexture txランクパネル;
		private CTexture txランク文字;
		private CTexture tx白波;

		private void t描画_白波()
		{
			if (!this.ctランク表示.b終了値に達してない)
			{
				int num = this.ct白波移動.n現在の値;
				int num2 = this.n本体X;
				int y = this.n本体Y + num;
				if ((y < (this.n本体Y + 0x80)) && ((y + 0x20) > this.n本体Y))
				{
					Rectangle rectangle = new Rectangle(0, 0, 0x20, 0x20);
					if (y < this.n本体Y)
					{
						rectangle.Y += this.n本体Y - y;
						rectangle.Height -= this.n本体Y - y;
						y = this.n本体Y;
					}
					if ((y + 0x20) > (this.n本体Y + 0x80))
					{
						rectangle.Height -= (y + 0x20) - (this.n本体Y + 0x80);
					}
					if (rectangle.Bottom > rectangle.Top)
					{
						for (int i = 0; i < 4; i++)
						{
							if (this.tx白波 != null)
							{
								Rectangle rectangle1 = rectangle;
								rectangle1.X = (int)(rectangle1.X * Scale.X);
								rectangle1.Y = (int)(rectangle1.Y * Scale.Y);
								rectangle1.Width = (int)(rectangle1.Width * Scale.X);
								rectangle1.Height = (int)(rectangle1.Height * Scale.Y);
								this.tx白波.t2D描画(
									CDTXMania.Instance.Device,
									(num2 + (i * 0x20)) * Scale.X,
									y * Scale.Y,
									rectangle1
								);
							}
						}
					}
				}
			}
		}
		//-----------------
		#endregion
	}
}
