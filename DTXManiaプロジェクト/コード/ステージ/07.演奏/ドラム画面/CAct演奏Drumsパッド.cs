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
		// コンストラクタ

		public CAct演奏Drumsパッド()
		{
			this.st基本位置 = new ST基本位置[9] {
				new ST基本位置(  22 * 3 - 66,  0, new Rectangle( 0, 0, 170, 130 ) ),
				new ST基本位置(  61 * 3 - 66, 11, new Rectangle( 170, 0, 170, 130 ) ),
				new ST基本位置(  96 * 3 - 66,  8, new Rectangle( 340, 0, 170, 130 ) ),
				new ST基本位置( 138 * 3 - 66,  7, new Rectangle( 0, 130, 170, 130 ) ),
				new ST基本位置( 179 * 3 - 66,  0, new Rectangle( 170, 130, 170, 130 ) ),
				new ST基本位置( 212 * 3 - 66,  5, new Rectangle( 340, 130, 170, 130 ) ),
				new ST基本位置( 250 * 3 - 66, 15, new Rectangle( 0, 260, 170, 130) ),
				new ST基本位置( 282 * 3 - 66,  0, new Rectangle( 170, 260, 170, 130 ) ),
				new ST基本位置( 304 * 3 - 66,  8, new Rectangle( 340, 260, 170, 130 ) )
			};
			base.b活性化してない = true;
		}


		// メソッド

		public void Hit(int nLane)
		{
			this.stパッド状態[nLane].n明るさ = 6;
			this.stパッド状態[nLane].nY座標加速度dot = 2;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.nフラッシュ制御タイマ = -1;
			this.nY座標制御タイマ = -1;
			for (int i = 0; i < 9; i++)
			{
				this.stパッド状態[i] = new STパッド状態(0, 0, 0);
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txパッド = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums pads.png"));
				this.tx光るパッド = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums pads flush.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txパッド);
				TextureFactory.tテクスチャの解放(ref this.tx光るパッド);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (!base.b活性化してない)
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
				for (int i = 0; i < 9; i++)
				{
					int index = this.n描画順[i];
					int x = this.st基本位置[index].x;
					int xDist = (int)(x * (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 1.0 : 0.75));
					x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 66 : 619 - 24;
					x += xDist;
					int y = (this.st基本位置[index].y + (CDTXMania.Instance.ConfigIni.bReverse.Drums ? -10 : 0x19e)) + this.stパッド状態[index].nY座標オフセットdot;
					if (this.txパッド != null)
					{
						// this.txパッド.vc拡大縮小倍率.X = ( CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ) ? 1.0f : 0.75f;
						this.txパッド.t2D描画(
							CDTXMania.Instance.Device,
							x,
							y * Scale.Y,
							this.st基本位置[index].rc
						);
					}
					if (this.tx光るパッド != null)
					{
						// this.tx光るパッド.vc拡大縮小倍率.X = ( CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ) ? 1.0f : 0.75f;
						this.tx光るパッド.n透明度 = (this.stパッド状態[index].n明るさ * 40) + 15;
						this.tx光るパッド.t2D描画(
							CDTXMania.Instance.Device,
							x,
							y * Scale.Y,
							this.st基本位置[index].rc);
					}
				}
				#endregion
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
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

		private long nY座標制御タイマ;
		private long nフラッシュ制御タイマ;
		private readonly int[] n描画順 = new int[] { 3, 6, 5, 4, 2, 1, 8, 7, 0 };
		private STパッド状態[] stパッド状態 = new STパッド状態[9];
		private readonly ST基本位置[] st基本位置;
		private CTexture txパッド;
		private CTexture tx光るパッド;
		//-----------------
		#endregion
	}
}
