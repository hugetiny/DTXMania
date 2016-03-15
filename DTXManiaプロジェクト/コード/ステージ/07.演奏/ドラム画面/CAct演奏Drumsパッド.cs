using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsパッド : CActivity
	{
		private readonly int[] n描画順 = new int[] { 3, 6, 5, 4, 2, 1, 8, 7, 0 };
		private long nY座標制御タイマ;
		private long nフラッシュ制御タイマ;
		private STパッド状態[] stパッド状態 = new STパッド状態[9];
		private CTexture txパッド;
		private CTexture tx光るパッド;

		[StructLayout(LayoutKind.Sequential)]
		private struct STパッド状態
		{
			public int n明るさ;
			public int nY座標オフセットdot;
			public int nY座標加速度dot;
			public STパッド状態(int n明るさ_, int nY座標オフセットdot_, int nY座標加速度dot_)
			{
				n明るさ = n明るさ_;
				nY座標オフセットdot = nY座標オフセットdot_;
				nY座標加速度dot = nY座標加速度dot_;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct ST基本位置
		{
			public int x;
			public int y;
			public Rectangle rc;

			public ST基本位置(int x_, int y_, Rectangle rc_)
			{
				x = x_;
				y = y_;
				rc = rc_;
			}
		}

		public void Hit(int nLane)
		{
			this.stパッド状態[nLane].n明るさ = 6;
			this.stパッド状態[nLane].nY座標加速度dot = 2;
		}

		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.nフラッシュ制御タイマ = -1;
				this.nY座標制御タイマ = -1;
				for (int i = 0; i < 9; i++)
				{
					this.stパッド状態[i] = new STパッド状態(0, 0, 0);
				}
				base.On活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.txパッド = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums pads.png"));
				this.tx光るパッド = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums pads flush.png"));
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txパッド);
				TextureFactory.tテクスチャの解放(ref this.tx光るパッド);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (base.b初めての進行描画)
				{
					this.nフラッシュ制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					this.nY座標制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					base.b初めての進行描画 = false;
				}
				long n現在時刻 = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
				if (n現在時刻 < this.nフラッシュ制御タイマ)
				{
					this.nフラッシュ制御タイマ = n現在時刻;
				}
				while ((n現在時刻 - this.nフラッシュ制御タイマ) >= 15)
				{
					for (int i = 0; i < 9; i++)
					{
						if (this.stパッド状態[i].n明るさ > 0)
						{
							this.stパッド状態[i].n明るさ--;
						}
					}
					this.nフラッシュ制御タイマ += 15;
				}
				n現在時刻 = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
				if (n現在時刻 < this.nY座標制御タイマ)
				{
					this.nY座標制御タイマ = n現在時刻;
				}
				while ((n現在時刻 - this.nY座標制御タイマ) >= 5)
				{
					for (int i = 0; i < 9; i++)
					{
						this.stパッド状態[i].nY座標オフセットdot += this.stパッド状態[i].nY座標加速度dot;
						if (this.stパッド状態[i].nY座標オフセットdot > 15)
						{
							this.stパッド状態[i].nY座標オフセットdot = 15;
							this.stパッド状態[i].nY座標加速度dot = -1;
						}
						else if (this.stパッド状態[i].nY座標オフセットdot < 0)
						{
							this.stパッド状態[i].nY座標オフセットdot = 0;
							this.stパッド状態[i].nY座標加速度dot = 0;
						}
					}
					this.nY座標制御タイマ += 5;
				}
				#region [ 表示 ]
				for (int idx = 0; idx < 9; idx++)
				{
					Rectangle rc;
					int x = 0;
					int yoffset = 0;
					int i = n描画順[idx];

					if (i == 0)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadLCY;
						x = CDTXMania.Instance.Coordinates.Lane.LCY.X + (CDTXMania.Instance.Coordinates.Lane.LCY.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadLCY.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetLCY.Y;
					}
					else if (i == 1)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadHH;
						x = CDTXMania.Instance.Coordinates.Lane.HHC.X + (CDTXMania.Instance.Coordinates.Lane.HHC.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadHH.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetHH.Y;
					}
					else if (i == 2)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadSD;
						x = CDTXMania.Instance.Coordinates.Lane.SD.X + (CDTXMania.Instance.Coordinates.Lane.SD.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadSD.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetSD.Y;
					}
					else if (i == 3)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadBD;
						x = CDTXMania.Instance.Coordinates.Lane.BD.X + (CDTXMania.Instance.Coordinates.Lane.BD.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadBD.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetBD.Y;
					}
					else if (i == 4)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadHT;
						x = CDTXMania.Instance.Coordinates.Lane.HT.X + (CDTXMania.Instance.Coordinates.Lane.HT.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadHT.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetHT.Y;
					}
					else if (i == 5)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadLT;
						x = CDTXMania.Instance.Coordinates.Lane.LT.X + (CDTXMania.Instance.Coordinates.Lane.LT.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadLT.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetLT.Y;
					}
					else if (i == 6)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadFT;
						x = CDTXMania.Instance.Coordinates.Lane.FT.X + (CDTXMania.Instance.Coordinates.Lane.FT.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadFT.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetFT.Y;
					}
					else if (i == 7)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadCY;
						x = CDTXMania.Instance.Coordinates.Lane.CY.X + (CDTXMania.Instance.Coordinates.Lane.CY.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadCY.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetCY.Y;
					}
					else if (i == 8)
					{
						rc = CDTXMania.Instance.Coordinates.ImgDrPadRCY;
						x = CDTXMania.Instance.Coordinates.Lane.RCY.X + CDTXMania.Instance.Coordinates.DrPadOffsetRCY.X +
							(CDTXMania.Instance.Coordinates.Lane.RCY.W / 2 - CDTXMania.Instance.Coordinates.ImgDrPadRCY.W / 2);
						yoffset = CDTXMania.Instance.Coordinates.DrPadOffsetRCY.Y;
					}
					else
					{
						rc = new Rectangle();
					}

					int y = (CDTXMania.Instance.ConfigIni.bReverse.Drums ?
						 SampleFramework.GameWindowSize.Height - CDTXMania.Instance.Coordinates.DrPad.Y - yoffset - rc.Height :
						 CDTXMania.Instance.Coordinates.DrPad.Y + yoffset) + 2 * this.stパッド状態[i].nY座標オフセットdot;

					if (this.txパッド != null)
					{
						this.txパッド.t2D描画(CDTXMania.Instance.Device, x, y, rc);
					}
					if (this.tx光るパッド != null)
					{
						this.tx光るパッド.n透明度 = (this.stパッド状態[i].n明るさ * 40) + 15;
						this.tx光るパッド.t2D描画(CDTXMania.Instance.Device, x, y, rc);
					}
				}
				#endregion
			}

			return 0;
		}
	}
}
