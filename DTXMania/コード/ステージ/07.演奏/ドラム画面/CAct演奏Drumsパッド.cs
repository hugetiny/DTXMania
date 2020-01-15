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
		private readonly EPad[] n描画順 = new EPad[] {
						EPad.BD,
						EPad.FT,
						EPad.LT,
						EPad.HT,
						EPad.SD,
						EPad.CY,
						EPad.HH,
						EPad.RD,
						EPad.LC
				};
		private long nY座標制御タイマ;
		private long nフラッシュ制御タイマ;
		private STPadValue<STパッド状態> stパッド状態 = new STPadValue<STパッド状態>();
		private CTexture txパッド;
		private CTexture tx光るパッド;

		private class STパッド状態
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

		public void Hit(EPad ePad)
		{
			if (ePad == EPad.HHO)       // #37173 2017.5.30 yyagi; There is HH pad, but no HHO pad. So, HH pad should be flashed insted of HHO
			{
				ePad = EPad.HH;
			}
			this.stパッド状態[ePad].n明るさ = 6;
			this.stパッド状態[ePad].nY座標加速度dot = 2;
		}

		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.nフラッシュ制御タイマ = -1;
				this.nY座標制御タイマ = -1;
				for (EPad i = EPad.DrumsPadMin; i < EPad.DrumsPadMax; ++i)
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
			if (b活性化してる &&
				CDTXMania.Instance.ConfigIni.bDrums有効 &&
				CDTXMania.Instance.DTX.bチップがある.Drums &&
				CDTXMania.Instance.ConfigIni.eDark != EDark.Full)
			{
				if (b初めての進行描画)
				{
					nフラッシュ制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					nY座標制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					b初めての進行描画 = false;
				}
				long n現在時刻 = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
				if (n現在時刻 < this.nフラッシュ制御タイマ)
				{
					this.nフラッシュ制御タイマ = n現在時刻;
				}
				while ((n現在時刻 - this.nフラッシュ制御タイマ) >= 15)
				{
					for (EPad i = EPad.DrumsPadMin; i < EPad.DrumsPadMax; i++)
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
					for (EPad i = EPad.DrumsPadMin; i < EPad.DrumsPadMax; i++)
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
				for (int idx = 0; idx < n描画順.Length; idx++)
				{
					EPad i = n描画順[idx];
					Rectangle rc = CDTXMania.Instance.Coordinates.ImgDrPad[i];
					int x = CDTXMania.Instance.ConfigIni.GetLaneX(EnumConverter.LaneFromPad(i)) +
							(CDTXMania.Instance.ConfigIni.GetLaneW(EnumConverter.LaneFromPad(i)) / 2 -
							CDTXMania.Instance.Coordinates.ImgDrPad[i].W / 2);
					if (i == EPad.RD)
					{
						x += 50;
					}
					int yoffset = CDTXMania.Instance.Coordinates.DrPadOffset[i].Y;
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
