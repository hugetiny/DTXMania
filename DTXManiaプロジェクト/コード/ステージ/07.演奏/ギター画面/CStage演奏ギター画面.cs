using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CStage演奏ギター画面 : CStage演奏画面共通
	{
		// コンストラクタ

		public CStage演奏ギター画面()
		{
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actStageFailed = new CAct演奏ステージ失敗());
			base.list子Activities.Add(this.actDANGER = new CAct演奏GuitarDanger());
			base.list子Activities.Add(this.actAVI = new CAct演奏AVI());
			base.list子Activities.Add(this.actBGA = new CAct演奏BGA());
			base.list子Activities.Add(this.actPanel = new CAct演奏パネル文字列());
			base.list子Activities.Add(this.act譜面スクロール速度 = new CAct演奏スクロール速度());
			base.list子Activities.Add(this.actStatusPanels = new CAct演奏Guitarステータスパネル());
			base.list子Activities.Add(this.actWailingBonus = new CAct演奏GuitarWailingBonus());
			base.list子Activities.Add(this.actScore = new CAct演奏Guitarスコア());
			base.list子Activities.Add(this.actRGB = new CAct演奏GuitarRGB());
			base.list子Activities.Add(this.actLaneFlushGB = new CAct演奏GuitarレーンフラッシュGB());
			base.list子Activities.Add(this.actJudgeString = new CAct演奏Guitar判定文字列());
			base.list子Activities.Add(this.actGauge = new CAct演奏Guitarゲージ());
			base.list子Activities.Add(this.actCombo = new CAct演奏Guitarコンボ());
			base.list子Activities.Add(this.actChipFireGB = new CAct演奏Guitarチップファイア());
			base.list子Activities.Add(this.actPlayInfo = new CAct演奏演奏情報());
			base.list子Activities.Add(this.actFI = new CActFIFOBlack());
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actFOClear = new CActFIFOWhite());
		}

		// CStage 実装

		public override void On活性化()
		{
			dtLastQueueOperation = DateTime.MinValue;
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.txチップ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar chips.png"));
				this.txヒットバー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar hit-bar.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txチップ);
				TextureFactory.tテクスチャの解放(ref this.txヒットバー);
				base.OnManagedリソースの解放();
			}
		}
		// その他
		#region [ private ]
		//-----------------
		protected override void t進行描画_ギターベース判定ライン()	// yyagi: ドラム画面とは座標が違うだけですが、まとめづらかったのでそのまま放置してます。
		{
			if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				if (CDTXMania.Instance.DTX.bチップがある.Guitar)
				{
					int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, bReverse[(int)E楽器パート.GUITAR], false, true)
							- (int)(3 * Scale.Y);
					// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					if (this.txヒットバー != null)
					{
						for (int i = 0; i < 4; i++)
						{
							this.txヒットバー.t2D描画(CDTXMania.Instance.Device,
								(23 + (28 * i)) * Scale.X,
								y,
								new Rectangle(
									0,
									(int)(i * 8 * Scale.Y),
									(int)(28 * Scale.X),
									(int)(8 * Scale.Y)
								)
							);
						}
					}
				}
				if (CDTXMania.Instance.DTX.bチップがある.Bass)
				{
					int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, bReverse[(int)E楽器パート.BASS], false, true)
							- (int)(3 * Scale.Y);
					// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					if (this.txヒットバー != null)
					{
						for (int j = 0; j < 4; j++)
						{
							this.txヒットバー.t2D描画(
								CDTXMania.Instance.Device,
								(477 + (28 * j)) * Scale.X,
								y,
								new Rectangle(
									0,
									(int)(j * 8 * Scale.Y),
									(int)(28 * Scale.X),
									(int)(8 * Scale.Y)
								)
							);
						}
					}
				}
			}
		}

		protected override void t背景テクスチャの生成()
		{
			Rectangle bgrect = new Rectangle((int)(181 * Scale.X), (int)(50 * Scale.Y), (int)(278 * Scale.X), (int)(355 * Scale.Y));
			string DefaultBgFilename = @"Graphics\ScreenPlayGuitar background.png";
			string DefaultLaneFilename = "";
			string BgFilename = "";
			string BACKGROUND = null;
			if ((CDTXMania.Instance.DTX.BACKGROUND_GR != null) && (CDTXMania.Instance.DTX.BACKGROUND_GR.Length > 0))
			{
				BACKGROUND = CDTXMania.Instance.DTX.BACKGROUND_GR;
			}
			else if ((CDTXMania.Instance.DTX.BACKGROUND != null) && (CDTXMania.Instance.DTX.BACKGROUND.Length > 0))
			{
				BACKGROUND = CDTXMania.Instance.DTX.BACKGROUND;
			}
			if ((BACKGROUND != null) && (BACKGROUND.Length > 0))
			{
				BgFilename = CDTXMania.Instance.DTX.strフォルダ名 + BACKGROUND;
			}
			base.t背景テクスチャの生成(DefaultBgFilename, DefaultLaneFilename, bgrect, BgFilename);
		}

		protected override void t進行描画_チップ_ドラムス(ref CChip pChip)
		{
			// int indexSevenLanes = this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ];
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
				this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
			}
		}
		protected override void t進行描画_チップ_ギターベース(ref CChip pChip, E楽器パート inst)
		{
			base.t進行描画_チップ_ギターベース(ref pChip, inst,
				演奏判定ライン座標.n判定ラインY座標(inst, false),	// 40
				演奏判定ライン座標.n判定ラインY座標(inst, true),	// 369
				(int)(0 * Scale.Y), (int)(409 * Scale.Y),				// Y軸表示範囲
				26, 480,					// openチップのX座標(Gt, Bs)
				0, 192, 103, 8, 32,			// オープンチップregionの x, y, w, h, 通常チップのw
				26, 98, 480, 552,			// GtのX, Gt左利きのX, BsのX, Bs左利きのX,
				36, 32						// 描画のX座標間隔, テクスチャのX座標間隔
			);
		}
		protected override void t進行描画_チップ_フィルイン(ref CChip pChip)
		{
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
			}
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi TEST
			switch ( pChip.n整数値 )
			{
				case 0x04:	// HH消音あり(従来同等)
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = true;
					break;
				case 0x05:	// HH消音無し
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = false;
					break;
				case 0x06:	// ギター消音あり(従来同等)
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Guitar = true;
					break;
				case 0x07:	// ギター消音無し
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Guitar = false;
					break;
				case 0x08:	// ベース消音あり(従来同等)
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = true;
					break;
				case 0x09:	// ベース消音無し
					CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = false;
					break;
			}
#endif

		}
		protected override void t進行描画_チップ_空打ち音設定_ドラム(ref CChip pChip)
		{
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
			}
		}

		#endregion
	}
}
