using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using SharpDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Combo共通 : CActivity
	{
		public STDGBSValue<CComboStatus> dgbコンボ数;
		public class CComboStatus
		{
			//public CAct演奏Combo共通 act;
			public int n最高値 { get; private set; }
			public int n現在値 { get; private set; }

			public CAct演奏Combo共通.EMode e現在のモード = EMode.非表示中;
			public int nCOMBO値 = 0;
			public long nコンボが切れた時刻 = -1;
			public int nジャンプインデックス値 = 99999;
			public int n現在表示中のCOMBO値 = 0;
			public long n前回の時刻_ジャンプ用 = -1;

			public void IncrementCombo()
			{
				++n現在値;
				if (n現在値 > n最高値)
				{
					n最高値 = n現在値;
				}
				nCOMBO値 = n現在値;
			}
			public void ResetCombo()
			{
				n現在値 = 0;
			}
		}

		private int[] nジャンプ差分値 = new int[180];
		// private STDGBVALUE<CSTAT> status;
		private CTexture tex;

		public enum EEvent
		{
			非表示,
			数値更新,
			同一数値,
			ミス通知
		}

		public enum EMode
		{
			非表示中,
			進行表示中,
			残像表示中
		}

		public CAct演奏Combo共通()
		{
			this.b活性化してない = true;

			// 180度分のジャンプY座標差分を取得。(0度: 0 → 90度:-15 → 180度: 0)
			for (int i = 0; i < 180; i++)
			{
				this.nジャンプ差分値[i] = (int)(-15.0 * Math.Sin((Math.PI * i) / 180.0) * Scale.Y);
			}
		}

		private void tコンボ表示(EPart inst, int nCombo値, int nジャンプインデックス)
		{
			int x, y;
			x = CDTXMania.Instance.ConfigIni.cdComboX[inst][CDTXMania.Instance.ConfigIni.eActiveInst];
			y = CDTXMania.Instance.ConfigIni.cdComboY[inst][CDTXMania.Instance.ConfigIni.eActiveInst];
			if (CDTXMania.Instance.ConfigIni.bReverse[inst])
			{
				y = SampleFramework.GameWindowSize.Height - y
					- CDTXMania.Instance.Coordinates.ImgComboCombo.H - CDTXMania.Instance.Coordinates.ImgComboOneDigit.H;
			}

			if (tex != null)
			{
				tex.n透明度 = 0xff;
			}

			int[] n位の数 = new int[10];
			// 表示は10桁もあれば足りるだろう
			//-----------------
			if (CDTXMania.Instance.ConfigIni.bDisplayCombo[inst] && nCombo値 > 0)
			{
				// n位の数[] の格納。(例：nCombo値=125 のとき n位の数 = { 5,2,1,0,0,0,0,0,0,0 })
				int n = nCombo値;
				int n桁数 = 0;
				int n全桁の合計幅 = 0;
				while ((n > 0) && (n桁数 < 10))
				{
					n位の数[n桁数] = n % 10;   // 1の位を格納
					n = (n - (n % 10)) / 10;  // 右へシフト（例: 12345 → 1234 ）
					n全桁の合計幅 += CDTXMania.Instance.Coordinates.ImgComboOneDigit.W;
					n桁数++;
				}

				// "COMBO" の拡大率を設定
				float f拡大率 = 1.0f;
				if (nジャンプインデックス >= 0 && nジャンプインデックス < 180)
					f拡大率 = 1.0f - (((float)this.nジャンプ差分値[nジャンプインデックス]) / 45.0f);    // f拡大率 = 1.0 → 1.3333... → 1.0

				if (this.tex != null)
				{
					this.tex.vc拡大縮小倍率 = new Vector3(f拡大率, f拡大率, 1.0f);
				}

				// "COMBO" 文字を表示
				int cbx = x - ((int)((CDTXMania.Instance.Coordinates.ImgComboCombo.W * f拡大率) / 2.0f));
				if (this.tex != null)
					this.tex.t2D描画(CDTXMania.Instance.Device,
						cbx,
						y + CDTXMania.Instance.Coordinates.ImgComboOneDigit.H,
						CDTXMania.Instance.Coordinates.ImgComboCombo);

				x = x + (n全桁の合計幅 / 2);
				// 1文字ずつ表示していく
				for (int i = 0; i < n桁数; i++)
				{
					f拡大率 = 1.0f;
					if (nジャンプインデックス >= 0 && nジャンプインデックス < 180)
						f拡大率 = 1.0f - (((float)this.nジャンプ差分値[nジャンプインデックス]) / 45f);    // f拡大率 = 1.0 → 1.3333... → 1.0

					if (this.tex != null)
						this.tex.vc拡大縮小倍率 = new Vector3(f拡大率, f拡大率, 1.0f);

					x -= CDTXMania.Instance.Coordinates.ImgComboOneDigit.W;

					if (this.tex != null)
					{
						this.tex.t2D描画(
							CDTXMania.Instance.Device,
							x - ((int)(((f拡大率 - 1.0f) * CDTXMania.Instance.Coordinates.ImgComboOneDigit.W) / 2.0f)),
							y - ((int)(((f拡大率 - 1.0f) * CDTXMania.Instance.Coordinates.ImgComboOneDigit.H) / 2.0f)),
							CDTXMania.Instance.Coordinates.ImgComboOneDigit.ApplyCounterXY(n位の数[i] % 4, n位の数[i] / 4, 0, 0));
					}
				}
			}
		}


		// CActivity 実装
		public override void On活性化()
		{
			this.dgbコンボ数 = new STDGBSValue<CComboStatus>();
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
			{
				this.dgbコンボ数[i] = new CComboStatus();
			}
			base.On活性化();
		}

		public override void On非活性化()
		{
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (this.b活性化してない)
				return;

			// this.txCOMBOドラム = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums-combo-drums2.png"));
			this.tex = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums-combo-guitar2.png"));

			base.OnManagedリソースの作成();
		}

		public override void OnManagedリソースの解放()
		{
			if (this.b活性化してない)
				return;

			// TextureFactory.tテクスチャの解放(ref this.txCOMBOドラム);
			TextureFactory.tテクスチャの解放(ref this.tex);

			base.OnManagedリソースの解放();
		}

		public override int On進行描画()
		{
			if (!this.b活性化してない)
			{
				for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効(inst) && CDTXMania.Instance.DTX.bチップがある[inst])
					{
						EEvent e今回の状態遷移イベント;
						CComboStatus st = this.dgbコンボ数[inst];

						// 前回と今回の COMBO 値から、e今回の状態遷移イベントを決定する。
						if (st.n現在表示中のCOMBO値 == st.nCOMBO値)
						{
							e今回の状態遷移イベント = EEvent.同一数値;
						}
						else if (st.n現在表示中のCOMBO値 > st.nCOMBO値)
						{
							e今回の状態遷移イベント = EEvent.ミス通知;
						}
						else if ((st.n現在表示中のCOMBO値 < CDTXMania.Instance.ConfigIni.nMinComboDisp[inst]) &&
							(st.nCOMBO値 < CDTXMania.Instance.ConfigIni.nMinComboDisp[inst]))
						{
							e今回の状態遷移イベント = EEvent.非表示;
						}
						else
						{
							e今回の状態遷移イベント = EEvent.数値更新;
						}

						// nジャンプインデックス値 の進行。
						if (st.nジャンプインデックス値 < 360)
						{
							if ((st.n前回の時刻_ジャンプ用 == -1) || (CDTXMania.Instance.Timer.n現在時刻 < st.n前回の時刻_ジャンプ用))
							{
								st.n前回の時刻_ジャンプ用 = CDTXMania.Instance.Timer.n現在時刻;
							}

							const long INTERVAL = 2;

							long diff = CDTXMania.Instance.Timer.n現在時刻 - st.n前回の時刻_ジャンプ用;
							st.n前回の時刻_ジャンプ用 += INTERVAL * (diff / INTERVAL);
							st.nジャンプインデックス値 += 3 * (int)(diff / INTERVAL);
							if (st.nジャンプインデックス値 >= 2000)
							{
								st.nジャンプインデックス値 -= 3 * ((st.nジャンプインデックス値 - 2000) / 3 + 1);
							}
						}


						Retry:  // モードが変化した場合はここからリトライする。

						switch (st.e現在のモード)
						{
							case EMode.非表示中:
								if (e今回の状態遷移イベント == EEvent.数値更新)
								{
									// モード変更
									st.e現在のモード = EMode.進行表示中;
									st.nジャンプインデックス値 = 0;
									st.n前回の時刻_ジャンプ用 = CDTXMania.Instance.Timer.n現在時刻;
									goto Retry;
								}

								st.n現在表示中のCOMBO値 = st.nCOMBO値;
								break;

							case EMode.進行表示中:
								if ((e今回の状態遷移イベント == EEvent.非表示) || (e今回の状態遷移イベント == EEvent.ミス通知))
								{
									// モード変更
									st.e現在のモード = EMode.残像表示中;
									//st.n残像表示中のCOMBO値 = st.n現在表示中のCOMBO値;
									st.nコンボが切れた時刻 = CDTXMania.Instance.Timer.n現在時刻;
									goto Retry;
								}

								if (e今回の状態遷移イベント == EEvent.数値更新)
								{
									st.nジャンプインデックス値 = 0;
									st.n前回の時刻_ジャンプ用 = CDTXMania.Instance.Timer.n現在時刻;
								}

								st.n現在表示中のCOMBO値 = st.nCOMBO値;
								this.tコンボ表示(inst, st.nCOMBO値, st.nジャンプインデックス値);
								break;

							case EMode.残像表示中:
								if (e今回の状態遷移イベント == EEvent.数値更新)
								{
									// モード変更１
									st.e現在のモード = EMode.進行表示中;
									goto Retry;
								}
								if ((CDTXMania.Instance.Timer.n現在時刻 - st.nコンボが切れた時刻) > 1000)
								{
									// モード変更２
									st.e現在のモード = EMode.非表示中;
									goto Retry;
								}
								st.n現在表示中のCOMBO値 = st.nCOMBO値;
								break;
						}
					}
				}
			}
			return 0;
		}
	}
}
