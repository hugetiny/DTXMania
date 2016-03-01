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
		// コンストラクタ
		public CAct演奏ステータスパネル共通()
		{
			this.stパネルマップ = new STATUSPANEL[12];		// yyagi: 以下、手抜きの初期化でスマン
			// { "DTXMANIA", 0 }, { "EXTREME", 1 }, ... みたいに書きたいが・・・
			string[] labels = new string[12]
			{
			    "DTXMANIA", "EXTREME", "ADVANCED", "ADVANCE", "BASIC", "RAW",
			    "REAL", "EASY", "EX-REAL", "ExREAL", "ExpertReal", "NORMAL"
			};
			int[] status = new int[12] 
			{
			    0, 1, 2, 2, 3, 4, 5, 6, 7, 7, 7, 8
			};

			for (int i = 0; i < 12; i++)
			{
				this.stパネルマップ[i] = new STATUSPANEL();
				this.stパネルマップ[i].status = status[i];
				this.stパネルマップ[i].label = labels[i];
			}

			base.b活性化してない = true;
		}


		// メソッド
		public void tラベル名からステータスパネルを決定する(string strラベル名)
		{
			if (string.IsNullOrEmpty(strラベル名))
			{
				this.nStatus = 0;
			}
			else
			{
				foreach (STATUSPANEL statuspanel in this.stパネルマップ)
				{
					if (strラベル名.Equals(statuspanel.label, StringComparison.CurrentCultureIgnoreCase))	// #24482 2011.2.17 yyagi ignore case
					{
						this.nStatus = statuspanel.status;
						return;
					}
				}
				this.nStatus = 0;
			}
		}

		// CActivity 実装

		public override void On活性化()
		{
			this.nStatus = 0;
			base.On活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.txStatusPanels = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay status panels right.png"));
				}
				else
				{
					this.tx左パネル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay status panels left.png"));
					this.tx右パネル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay status panels right.png"));
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					TextureFactory.tテクスチャの解放(ref this.txStatusPanels);
				}
				else
				{
					TextureFactory.tテクスチャの解放(ref this.tx左パネル);
					TextureFactory.tテクスチャの解放(ref this.tx右パネル);
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
					if ((this.txStatusPanels != null))
					{
						this.txStatusPanels.t2D描画(
							CDTXMania.Instance.Device,
							0x26f * Scale.X,
							0x14f * Scale.Y,
							new Rectangle(
								(int)(this.nStatus * 15 * Scale.X),
								(int)(0xb7 * Scale.Y),
								(int)(15 * Scale.X),
								(int)(0x49 * Scale.Y)
							)
						);
						int drums = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums;
						if (drums < 0)
						{
							drums = 0;
						}
						if (drums > 15)
						{
							drums = 15;
						}
						this.txStatusPanels.t2D描画(CDTXMania.Instance.Device,
							0x26f * Scale.X,
							0x3b * Scale.Y,
							new Rectangle(
								(int)(drums * 15 * Scale.X),
								0,
								(int)(15 * Scale.X),
								(int)(0xac * Scale.Y)
							)
						);
					}
				}
				else
				{
					if (this.tx左パネル != null)
					{
						this.tx左パネル.t2D描画(
							CDTXMania.Instance.Device,
							3 * Scale.X,
							0x143 * Scale.Y,
							new Rectangle(
								(int)(this.nStatus * 15 * Scale.X),
								(int)(0xb7 * Scale.Y),
								(int)(15 * Scale.X),
								(int)(0x49 * Scale.Y)
							)
						);
						int guitar = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Guitar;
						if (guitar < 0)
						{
							guitar = 0;
						}
						if (guitar > 15)
						{
							guitar = 15;
						}
						this.tx左パネル.t2D描画(
							CDTXMania.Instance.Device,
							3 * Scale.X,
							0x35 * Scale.Y,
							new Rectangle(
								(int)(guitar * 15 * Scale.X),
								0,
								(int)(15 * Scale.X),
								(int)(0xac * Scale.Y)
							)
						);
					}
					if (this.tx右パネル != null)
					{
						this.tx右パネル.t2D描画(
							CDTXMania.Instance.Device,
							0x26e * Scale.X,
							0x143 * Scale.Y,
							new Rectangle(
								(int)(this.nStatus * 15 * Scale.X),
								(int)(0xb7 * Scale.Y),
								(int)(15 * Scale.X),
								(int)(0x49 * Scale.Y)
							)
						);
						int bass = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Bass;
						if (bass < 0)
						{
							bass = 0;
						}
						if (bass > 15)
						{
							bass = 15;
						}
						this.tx右パネル.t2D描画(
							CDTXMania.Instance.Device,
							0x26e * Scale.X,
							0x35 * Scale.Y,
							new Rectangle(
								(int)(bass * 15 * Scale.X),
								0,
								(int)(15 * Scale.X),
								(int)(0xac * Scale.Y)
							)
						);
					}
				}
			}
			return 0;
		}

		[StructLayout(LayoutKind.Sequential)]
		protected struct STATUSPANEL
		{
			public string label;
			public int status;
		}

		private CTexture tx右パネル;
		private CTexture tx左パネル;
		private CTexture txStatusPanels;
		protected int nStatus;
		protected STATUSPANEL[] stパネルマップ = null;
	}
}
