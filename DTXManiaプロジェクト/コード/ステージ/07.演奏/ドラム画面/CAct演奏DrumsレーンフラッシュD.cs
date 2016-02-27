using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏DrumsレーンフラッシュD : CActivity
	{
		// コンストラクタ

		public CAct演奏DrumsレーンフラッシュD()
		{
			this.stレーンサイズ = new STレーンサイズ[8]
			{
				new STレーンサイズ(  36 * 3 - 36 * 3, 36 ),
				new STレーンサイズ(  77 * 3 - 36 * 3, 30 ),
				new STレーンサイズ( 111 * 3 - 36 * 3, 30 ),
				new STレーンサイズ( 146 * 3 - 36 * 3, 42 ),
				new STレーンサイズ( 193 * 3 - 36 * 3, 30 ),
				new STレーンサイズ( 227 * 3 - 36 * 3, 30 ),
				new STレーンサイズ( 261 * 3 - 36 * 3, 30 ),
				new STレーンサイズ( 295 * 3 - 36 * 3, 36 )
			};
			this.strファイル名 = new string[] {
				@"Graphics\ScreenPlayDrums lane flush cymbal.png",
				@"Graphics\ScreenPlayDrums lane flush hihat.png",
				@"Graphics\ScreenPlayDrums lane flush snare.png",
				@"Graphics\ScreenPlayDrums lane flush bass.png",
				@"Graphics\ScreenPlayDrums lane flush hitom.png",
				@"Graphics\ScreenPlayDrums lane flush lowtom.png",
				@"Graphics\ScreenPlayDrums lane flush floortom.png",
				@"Graphics\ScreenPlayDrums lane flush cymbal.png",
				@"Graphics\ScreenPlayDrums lane flush cymbal reverse.png",
				@"Graphics\ScreenPlayDrums lane flush hihat reverse.png",
				@"Graphics\ScreenPlayDrums lane flush snare reverse.png",
				@"Graphics\ScreenPlayDrums lane flush bass reverse.png",
				@"Graphics\ScreenPlayDrums lane flush hitom reverse.png",
				@"Graphics\ScreenPlayDrums lane flush lowtom reverse.png",
				@"Graphics\ScreenPlayDrums lane flush floortom reverse.png",
				@"Graphics\ScreenPlayDrums lane flush cymbal reverse.png"
			};
			base.b活性化してない = true;
		}


		// メソッド

		public void Start(Eレーン lane, float f強弱度合い)
		{
			int num = (int)((1f - f強弱度合い) * 55f);
			this.ct進行[(int)lane] = new CCounter(num, 100, 4, CDTXMania.Instance.Timer);
		}


		// CActivity 実装

		public override void On活性化()
		{
			for (int i = 0; i < 8; i++)
			{
				this.ct進行[i] = new CCounter();
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			for (int i = 0; i < 8; i++)
			{
				this.ct進行[i] = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 0x10; i++)
				{
					this.txFlush[i] = TextureFactory.tテクスチャの生成Af(CSkin.Path(this.strファイル名[i]));
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				for (int i = 0; i < 0x10; i++)
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
				for (int i = 0; i < 8; i++)
				{
					if (!this.ct進行[i].b停止中)
					{
						this.ct進行[i].t進行();
						if (this.ct進行[i].b終了値に達した)
						{
							this.ct進行[i].t停止();
						}
					}
				}
				for (int j = 0; j < 8; j++)
				{
					if (!this.ct進行[j].b停止中)
					{
						int x = this.stレーンサイズ[j].x;
						int w = this.stレーンサイズ[j].w;

						x = (int)(x * (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 1.0 : 0.75));
						x += (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 36 * 3 : 619 - 24 + 36;
						w = (int)(w * (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 1.0 : 0.75));

						//for ( int k = 0; k < 3; k++ )
						int k = 0;
						{
							if (CDTXMania.Instance.ConfigIni.bReverse.Drums)
							{
								int y = (k * 0x80) - ((this.ct進行[j].n現在の値 * 0x180) / 100);
								for (int m = 0; m < w; m += 42)
								{
									if (this.txFlush[j + 8] != null)
									{
										this.txFlush[j + 8].t2D描画(
											CDTXMania.Instance.Device,
											(x + m),
											y * Scale.Y,
											new Rectangle(
												(int)((k * 0x2a) * Scale.X),
												0,
												((w - m) < 0x2a) ? (int)((w - m) * Scale.X) : (int)(0x2a * Scale.X),
												(int)(0x80 * 3 * Scale.Y)
											)
										);
									}
								}
							}
							else
							{
								int y = (0x60 + (k * 0x80)) + ((this.ct進行[j].n現在の値 * 0x180) / 100);
								if (y < 480)
								{
									for (int n = 0; n < w; n += 42)
									{
										if (this.txFlush[j] != null)
										{
											this.txFlush[j].t2D描画(
												CDTXMania.Instance.Device,
												(x + n),
												(int)(y * Scale.Y),
												new Rectangle(
													(int)(k * 0x2a * Scale.X),
													0,
													((w - n) < 0x2a) ? (int)((w - n) * Scale.X) : (int)(0x2a * Scale.X),
													(int)(0x80 * 3 * Scale.Y)
												)
											);
										}
									}
								}
							}
						}
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout(LayoutKind.Sequential)]
		private struct STレーンサイズ
		{
			public int x;
			public int w;
			public STレーンサイズ(int x_, int w_)
			{
				x = x_;
				w = w_;
			}
		}

		private CCounter[] ct進行 = new CCounter[8];
		private readonly string[] strファイル名;
		private readonly STレーンサイズ[] stレーンサイズ;
		private CTextureAf[] txFlush = new CTextureAf[0x10];
		//-----------------
		#endregion
	}
}
