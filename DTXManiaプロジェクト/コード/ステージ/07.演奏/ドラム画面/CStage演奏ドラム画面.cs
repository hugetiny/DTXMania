using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CStage演奏ドラム画面 : CStage演奏画面共通
	{
		// コンストラクタ

		public CStage演奏ドラム画面()
		{
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actPad = new CAct演奏Drumsパッド() );
			base.list子Activities.Add( this.actCombo = new CAct演奏DrumsコンボDGB() );
			base.list子Activities.Add( this.actDANGER = new CAct演奏DrumsDanger() );
			base.list子Activities.Add( this.actChipFireD = new CAct演奏DrumsチップファイアD() );
            base.list子Activities.Add( this.actChipFireGB = new CAct演奏DrumsチップファイアGB());
            base.list子Activities.Add( this.actGauge = new CAct演奏Drumsゲージ() );
            base.list子Activities.Add( this.actGraph = new CAct演奏Drumsグラフ() ); // #24074 2011.01.23 add ikanick
			base.list子Activities.Add( this.actJudgeString = new CAct演奏Drums判定文字列() );
			base.list子Activities.Add( this.actLaneFlushD = new CAct演奏DrumsレーンフラッシュD() );
			base.list子Activities.Add( this.actLaneFlushGB = new CAct演奏DrumsレーンフラッシュGB() );
			base.list子Activities.Add( this.actRGB = new CAct演奏DrumsRGB() );
			base.list子Activities.Add( this.actScore = new CAct演奏Drumsスコア() );
			base.list子Activities.Add( this.actStatusPanels = new CAct演奏Drumsステータスパネル() );
			base.list子Activities.Add( this.actWailingBonus = new CAct演奏DrumsWailingBonus() );
			base.list子Activities.Add( this.act譜面スクロール速度 = new CAct演奏スクロール速度() );
			base.list子Activities.Add( this.actAVI = new CAct演奏AVI() );
			base.list子Activities.Add( this.actBGA = new CAct演奏BGA() );
			base.list子Activities.Add( this.actPanel = new CAct演奏パネル文字列() );
			base.list子Activities.Add( this.actStageFailed = new CAct演奏ステージ失敗() );
			base.list子Activities.Add( this.actPlayInfo = new CAct演奏演奏情報() );
			base.list子Activities.Add( this.actFI = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFOClear = new CActFIFOWhite() );
		}


		// メソッド

		public void t演奏結果を格納する( out CScoreIni.C演奏記録 Drums, out CScoreIni.C演奏記録 Guitar, out CScoreIni.C演奏記録 Bass, out CDTX.CChip[] r空打ちドラムチップ )
		{
			base.t演奏結果を格納する・ドラム( out Drums );
			base.t演奏結果を格納する・ギター( out Guitar );
			base.t演奏結果を格納する・ベース( out Bass );

			r空打ちドラムチップ = new CDTX.CChip[ 10 ];
			for ( int i = 0; i < 10; i++ )
			{
				r空打ちドラムチップ[ i ] = this.r空うちChip( E楽器パート.DRUMS, (Eパッド) i );
				if( r空打ちドラムチップ[ i ] == null )
				{
					r空打ちドラムチップ[ i ] = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( CSound管理.rc演奏用タイマ.n現在時刻, this.nパッド0Atoチャンネル0A[ i ], this.nInputAdjustTimeMs.Drums );
				}
			}

//			if ( CDTXMania.ConfigIni.bIsSwappedGuitarBass )		// #24063 2011.1.24 yyagi Gt/Bsを入れ替えていたなら、演奏結果も入れ替える
//			{
//				CScoreIni.C演奏記録 t;
//				t = Guitar;
//				Guitar = Bass;
//				Bass = t;
//			
//				CDTXMania.DTX.SwapGuitarBassInfos();			// 譜面情報も元に戻す
//			}
		}


		// CStage 実装

		public override void On活性化()
		{
			this.bフィルイン中 = false;
			base.On活性化();

			// MODIFY_BEGIN #25398 2011.06.07 FROM
			if( CDTXMania.bコンパクトモード )
			{
				var score = new Cスコア();
				CDTXMania.Songs管理.tScoreIniを読み込んで譜面情報を設定する( CDTXMania.strコンパクトモードファイル + ".score.ini", ref score );
				this.actGraph.dbグラフ値目標_渡 = score.譜面情報.最大スキル[ 0 ];
			}
			else
			{
				this.actGraph.dbグラフ値目標_渡 = CDTXMania.stage選曲.r確定されたスコア.譜面情報.最大スキル[ 0 ];	// #24074 2011.01.23 add ikanick
			}
			// MODIFY_END #25398
			dtLastQueueOperation = DateTime.MinValue;
		}
		public override void On非活性化()
		{
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.t背景テクスチャの生成();
				this.txチップ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums chips.png" ) );
				this.txヒットバー = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums hit-bar.png" ) );
				this.txヒットバーGB = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums hit-bar guitar.png" ) );
				//this.txWailing枠 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay wailing cursor.png" ) );
				this.txレーンフレームGB = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums lane parts guitar.png" ) );
				if( this.txレーンフレームGB != null )
				{
					this.txレーンフレームGB.n透明度 = 0xff - CDTXMania.ConfigIni.n背景の透過度;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				//CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.txヒットバー );
				CDTXMania.tテクスチャの解放( ref this.txヒットバーGB );
				CDTXMania.tテクスチャの解放( ref this.txチップ );
				CDTXMania.tテクスチャの解放( ref this.txレーンフレームGB );
				//CDTXMania.tテクスチャの解放( ref this.txWailing枠 );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			base.sw.Start();
			if( !base.b活性化してない )
			{
				bool bIsFinishedPlaying = false;
				bool bIsFinishedFadeout = false;
				#region [ 初めての進行描画 ]
				if ( base.b初めての進行描画 )
				{
                    CSound管理.rc演奏用タイマ.tリセット();
					CDTXMania.Timer.tリセット();
					this.ctチップ模様アニメ.Drums = new CCounter( 0, 0x30, 10, CDTXMania.Timer );
					this.ctチップ模様アニメ.Guitar = new CCounter( 0, 0x17, 20, CDTXMania.Timer );
					this.ctチップ模様アニメ.Bass = new CCounter( 0, 0x17, 20, CDTXMania.Timer );
					this.ctWailingチップ模様アニメ = new CCounter( 0, 4, 50, CDTXMania.Timer );

					// this.actChipFireD.Start( Eレーン.HH );	// #31554 2013.6.12 yyagi
					// 初チップヒット時のもたつき回避。最初にactChipFireD.Start()するときにJITが掛かって？
					// ものすごく待たされる(2回目以降と比べると2,3桁tick違う)。そこで最初の画面フェードインの間に
					// 一発Start()を掛けてJITの結果を生成させておく。

					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					this.actFI.tフェードイン開始();

					if ( CDTXMania.DTXVmode.Enabled )			// DTXVモードなら
					{
						#region [ DTXV用の再生設定にする(全AUTOなど) ]
						tDTXV用の設定();
						#endregion
						t演奏位置の変更( CDTXMania.DTXVmode.nStartBar );
					}

					CDTXMania.Sound管理.tDisableUpdateBufferAutomatically();
					base.b初めての進行描画 = false;
				}
				#endregion
				if ( ( CDTXMania.ConfigIni.bSTAGEFAILED有効 && this.actGauge.IsFailed( E楽器パート.DRUMS ) ) && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
				{
					this.actStageFailed.Start();
					CDTXMania.DTX.t全チップの再生停止();
					base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED;
				}
				this.t進行描画・背景();
				this.t進行描画・MIDIBGM();
				this.t進行描画・パネル文字列();
				this.t進行描画・スコア();
				this.t進行描画・AVI();
				this.t進行描画・BGA();
				this.t進行描画・ステータスパネル();
				this.t進行描画・ギターベースフレーム();
				this.t進行描画・レーンフラッシュGB();
                this.t進行描画・ギターベース判定ライン();
                this.t進行描画・ゲージ();
                this.t進行描画・グラフ();   // #24074 2011.01.23 add ikanick
				this.t進行描画・レーンフラッシュD();
				this.t進行描画・DANGER();
				this.t進行描画・判定ライン();
				if ( this.e判定表示優先度 == E判定表示優先度.Chipより下 )
				{
					this.t進行描画・RGBボタン();
					this.t進行描画・判定文字列1・通常位置指定の場合();
					this.t進行描画・コンボ();
				}
				this.t進行描画・WailingBonus();
				this.t進行描画・譜面スクロール速度();
				this.t進行描画・チップアニメ();
				bIsFinishedPlaying = this.t進行描画・チップ(E楽器パート.DRUMS);
				this.t進行描画・演奏情報();
				this.t進行描画・ドラムパッド();
				if ( this.e判定表示優先度 == E判定表示優先度.Chipより上 )
				{
					this.t進行描画・RGBボタン();
					this.t進行描画・判定文字列1・通常位置指定の場合();
					this.t進行描画・コンボ();
				}
				this.t進行描画・判定文字列2・判定ライン上指定の場合();
				this.t進行描画・Wailing枠();
				this.t進行描画・チップファイアD();
				this.t進行描画・チップファイアGB();
				this.t進行描画・STAGEFAILED();
				bIsFinishedFadeout = this.t進行描画・フェードイン・アウト();
				if( bIsFinishedPlaying && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
				{
					if ( CDTXMania.DTXVmode.Enabled )
					{
						if ( CDTXMania.Timer.b停止していない )
						{
							this.actPanel.Stop();				// PANEL表示停止
						    CDTXMania.Timer.t一時停止();		// 再生時刻カウンタ停止
						}
						Thread.Sleep( 5 );
						// DTXCからの次のメッセージを待ち続ける
					}
					else
					{
						this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージクリア;
						base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト;
						this.actFOClear.tフェードアウト開始();
					}
				}
				if ( this.eフェードアウト完了時の戻り値 == E演奏画面の戻り値.再読込・再演奏 )
				{
					bIsFinishedFadeout = true;
				}
				if( bIsFinishedFadeout )
				{
					Debug.WriteLine( "Total On進行描画=" + sw.ElapsedMilliseconds + "ms" );
					return (int) this.eフェードアウト完了時の戻り値;
				}

				ManageMixerQueue();

				// キー入力

				if( CDTXMania.act現在入力を占有中のプラグイン == null )
					this.tキー入力();
			}
			base.sw.Stop();
			return 0;
		}

		// その他

		#region [ private ]
		//-----------------

		private CAct演奏DrumsチップファイアD actChipFireD;
		private CAct演奏Drumsグラフ actGraph;   // #24074 2011.01.23 add ikanick
		private CAct演奏Drumsパッド actPad;
		private bool bフィルイン中;
		private readonly Eパッド[] eチャンネルtoパッド = new Eパッド[]
		{
			Eパッド.HH, Eパッド.SD, Eパッド.BD, Eパッド.HT,
			Eパッド.LT, Eパッド.CY, Eパッド.FT, Eパッド.HHO,
			Eパッド.RD, Eパッド.UNKNOWN, Eパッド.UNKNOWN, Eパッド.LC
		};
		private readonly int[] nチャンネルtoX座標 = new int[] { 76, 110, 145, 192, 226, 294, 260, 79, 300, 35 };
		private CTexture txヒットバーGB;
		private CTexture txレーンフレームGB;
		//-----------------

		private bool bフィルイン区間の最後のChipである( CDTX.CChip pChip )
		{
			if( pChip == null )
			{
				return false;
			}
			int num = pChip.n発声位置;
			for( int i = listChip.IndexOf( pChip ) + 1; i < listChip.Count; i++ )
			{
				pChip = listChip[ i ];
				if( ( pChip.nチャンネル番号 == 0x53 ) && ( pChip.n整数値 == 2 ) )
				{
					return true;
				}
				if( ( ( pChip.nチャンネル番号 >= 0x11 ) && ( pChip.nチャンネル番号 <= 0x19 ) ) && ( ( pChip.n発声位置 - num ) > 0x18 ) )
				{
					return false;
				}
			}
			return true;
		}

		protected override E判定 tチップのヒット処理( long nHitTime, CDTX.CChip pChip, bool bCorrectLane )
		{
			E判定 eJudgeResult = tチップのヒット処理( nHitTime, pChip, E楽器パート.DRUMS, bCorrectLane );
			// #24074 2011.01.23 add ikanick
			this.actGraph.dbグラフ値現在_渡 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Drums, this.nヒット数・Auto含まない.Drums.Perfect, this.nヒット数・Auto含まない.Drums.Great, this.nヒット数・Auto含まない.Drums.Good, this.nヒット数・Auto含まない.Drums.Poor, this.nヒット数・Auto含まない.Drums.Miss, E楽器パート.DRUMS,  bIsAutoPlay );
			return eJudgeResult;
		}

		protected override void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part )
		{
			this.tチップのヒット処理・BadならびにTight時のMiss( part, 0, E楽器パート.DRUMS );
		}
		protected override void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part, int nLane )
		{
			this.tチップのヒット処理・BadならびにTight時のMiss( part, nLane, E楽器パート.DRUMS );
		}

		private bool tドラムヒット処理( long nHitTime, Eパッド type, CDTX.CChip pChip, int n強弱度合い0to127 )
		{
			if( pChip == null )
			{
				return false;
			}
			int index = pChip.nチャンネル番号;
			if ( ( index >= 0x11 ) && ( index <= 0x1a ) )
			{
				index -= 0x11;
			}
			else if ( ( index >= 0x31 ) && ( index <= 0x3a ) )
			{
				index -= 0x31;
			}
			int nLane = this.nチャンネル0Atoレーン07[ index ];
			int nPad = this.nチャンネル0Atoパッド08[ index ];
			bool bPChipIsAutoPlay = bIsAutoPlay[ nLane ];
			int nInputAdjustTime = bPChipIsAutoPlay ? 0 : this.nInputAdjustTimeMs.Drums;
			E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nHitTime, pChip, nInputAdjustTime );
			if( e判定 == E判定.Miss )
			{
				return false;
			}
			this.tチップのヒット処理( nHitTime, pChip );
			this.actLaneFlushD.Start( (Eレーン) nLane, ( (float) n強弱度合い0to127 ) / 127f );
			this.actPad.Hit( nPad );
			if( ( e判定 != E判定.Poor ) && ( e判定 != E判定.Miss ) )
			{
				bool flag = this.bフィルイン中;
				bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである( pChip );
				// bool flag3 = flag2;
				// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
				this.actChipFireD.Start( (Eレーン)nLane, flag, flag2, flag2, 演奏判定ライン座標.nJudgeLinePosY_delta.Drums );
			}
			if( CDTXMania.ConfigIni.bドラム打音を発声する )
			{
				CDTX.CChip rChip = null;
				bool bIsChipsoundPriorToPad = true;
				if( ( ( type == Eパッド.HH ) || ( type == Eパッド.HHO ) ) || ( type == Eパッド.LC ) )
				{
					bIsChipsoundPriorToPad = CDTXMania.ConfigIni.eHitSoundPriorityHH == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if( ( type == Eパッド.LT ) || ( type == Eパッド.FT ) )
				{
					bIsChipsoundPriorToPad = CDTXMania.ConfigIni.eHitSoundPriorityFT == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if( ( type == Eパッド.CY ) || ( type == Eパッド.RD ) )
				{
					bIsChipsoundPriorToPad = CDTXMania.ConfigIni.eHitSoundPriorityCY == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				if( bIsChipsoundPriorToPad )
				{
					rChip = pChip;
				}
				else
				{
					Eパッド hH = type;
					if( !CDTXMania.DTX.bチップがある.HHOpen && ( type == Eパッド.HHO ) )
					{
						hH = Eパッド.HH;
					}
					if( !CDTXMania.DTX.bチップがある.Ride && ( type == Eパッド.RD ) )
					{
						hH = Eパッド.CY;
					}
					if( !CDTXMania.DTX.bチップがある.LeftCymbal && ( type == Eパッド.LC ) )
					{
						hH = Eパッド.HH;
					}
					rChip = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nHitTime, this.nパッド0Atoチャンネル0A[ (int) hH ], nInputAdjustTime );
					if( rChip == null )
					{
						rChip = pChip;
					}
				}
				this.tサウンド再生( rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
			}
			return true;
		}

		protected override void ドラムスクロール速度アップ()
		{
			CDTXMania.ConfigIni.n譜面スクロール速度.Drums = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Drums + 1, 1999 );
		}
		protected override void ドラムスクロール速度ダウン()
		{
			CDTXMania.ConfigIni.n譜面スクロール速度.Drums = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Drums - 1, 0 );
		}

	
		protected override void t進行描画・AVI()
		{
			base.t進行描画・AVI( 338, 57 );
		}
		protected override void t進行描画・BGA()
		{
			base.t進行描画・BGA( 338, 57 );
		}
		protected override void t進行描画・DANGER()
		{
//			this.actDANGER.t進行描画( this.actGauge.db現在のゲージ値.Drums < 0.3, false, false );
			this.actDANGER.t進行描画( this.actGauge.IsDanger(E楽器パート.DRUMS), false, false );
		}

		protected override void t進行描画・Wailing枠()
		{
			int yG = this.演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, bReverse[ (int) E楽器パート.GUITAR ], true );
			int yB = this.演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS,   false, bReverse[ (int) E楽器パート.BASS   ], true );
			base.t進行描画・Wailing枠( 587, 478,
				yG,
				yB
			//	CDTXMania.ConfigIni.bReverse.Guitar ? ( 400 - this.txWailing枠.sz画像サイズ.Height ) : 69,	// 350:69
			//	CDTXMania.ConfigIni.bReverse.Bass ? ( 400 - this.txWailing枠.sz画像サイズ.Height ) : 69		// 350:69
			);
		}
		private void t進行描画・ギターベースフレーム()
		{
			if( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if( CDTXMania.DTX.bチップがある.Guitar )
				{
					for( int i = 0; i < 355; i += 0x80 )
					{
						Rectangle rectangle = new Rectangle( 0, 0, 0x6d, 0x80 );
						if( ( i + 0x80 ) > 355 )
						{
							rectangle.Height -= ( i + 0x80 ) - 355;
						}
						if( this.txレーンフレームGB != null )
						{
							this.txレーンフレームGB.t2D描画( CDTXMania.app.Device, 0x1fb, 0x39 + i, rectangle );
						}
					}
				}
				if( CDTXMania.DTX.bチップがある.Bass )
				{
					for( int j = 0; j < 355; j += 0x80 )
					{
						Rectangle rectangle2 = new Rectangle( 0, 0, 0x6d, 0x80 );
						if( ( j + 0x80 ) > 355 )
						{
							rectangle2.Height -= ( j + 0x80 ) - 355;
						}
						if( this.txレーンフレームGB != null )
						{
							this.txレーンフレームGB.t2D描画( CDTXMania.app.Device, 0x18e, 0x39 + j, rectangle2 );
						}
					}
				}
			}
		}
		private void t進行描画・ギターベース判定ライン()		// yyagi: ギタレボモードとは座標が違うだけですが、まとめづらかったのでそのまま放置してます。
		{
			if ( ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if ( CDTXMania.DTX.bチップがある.Guitar )
				{
					int y = this.演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, bReverse[ (int) E楽器パート.GUITAR ] ) - 3;
																// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					if ( this.txヒットバーGB != null )
					{
						for ( int i = 0; i < 3; i++ )
						{
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, 509 + ( 26 * i ), y );
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, ( 509 + ( 26 * i ) ) + 16, y, new Rectangle( 0, 0, 10, 16 ) );
						}
					}
				}
				if ( CDTXMania.DTX.bチップがある.Bass )
				{
					int y = this.演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS,   false, bReverse[ (int) E楽器パート.BASS   ] ) - 3;
																// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
					if ( this.txヒットバーGB != null )
					{
						for ( int j = 0; j < 3; j++ )
						{
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, 400 + ( 26 * j ), y );
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, ( 400 + ( 26 * j ) ) + 16, y, new Rectangle( 0, 0, 10, 16 ) );
						}
					}
				}
			}
		}

		private void t進行描画・グラフ()        
        {
			if ( !CDTXMania.ConfigIni.bストイックモード && !CDTXMania.ConfigIni.bドラムが全部オートプレイである && CDTXMania.ConfigIni.bGraph.Drums )
			{
                this.actGraph.On進行描画();
            }
        }

		private void t進行描画・チップファイアD()
		{
			this.actChipFireD.On進行描画();
		}

		private void t進行描画・ドラムパッド()
		{
			if( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				this.actPad.On進行描画();
			}
		}
		protected override void t進行描画・パネル文字列()
		{
			base.t進行描画・パネル文字列( 336, 427 );
		}

		protected override void t進行描画・演奏情報()
		{
			base.t進行描画・演奏情報( 338, 57 );
		}

		protected override void t入力処理・ドラム()
		{
			for( int nPad = 0; nPad < (int) Eパッド.MAX; nPad++ )		// #27029 2012.1.4 from: <10 to <=10; Eパッドの要素が１つ（HP）増えたため。
																		//		  2012.1.5 yyagi: (int)Eパッド.MAX に変更。Eパッドの要素数への依存を無くすため。
			{
				List<STInputEvent> listInputEvent = CDTXMania.Pad.GetEvents( E楽器パート.DRUMS, (Eパッド) nPad );

				if( ( listInputEvent == null ) || ( listInputEvent.Count == 0 ) )
					continue;

				this.t入力メソッド記憶( E楽器パート.DRUMS );

				#region [ 打ち分けグループ調整 ]
				//-----------------------------
				EHHGroup eHHGroup = CDTXMania.ConfigIni.eHHGroup;
				EFTGroup eFTGroup = CDTXMania.ConfigIni.eFTGroup;
				ECYGroup eCYGroup = CDTXMania.ConfigIni.eCYGroup;

				if( !CDTXMania.DTX.bチップがある.Ride && ( eCYGroup == ECYGroup.打ち分ける ) )
				{
					eCYGroup = ECYGroup.共通;
				}
				if( !CDTXMania.DTX.bチップがある.HHOpen && ( eHHGroup == EHHGroup.全部打ち分ける ) )
				{
					eHHGroup = EHHGroup.左シンバルのみ打ち分ける;
				}
				if( !CDTXMania.DTX.bチップがある.HHOpen && ( eHHGroup == EHHGroup.ハイハットのみ打ち分ける ) )
				{
					eHHGroup = EHHGroup.全部共通;
				}
				if( !CDTXMania.DTX.bチップがある.LeftCymbal && ( eHHGroup == EHHGroup.全部打ち分ける ) )
				{
					eHHGroup = EHHGroup.ハイハットのみ打ち分ける;
				}
				if( !CDTXMania.DTX.bチップがある.LeftCymbal && ( eHHGroup == EHHGroup.左シンバルのみ打ち分ける ) )
				{
					eHHGroup = EHHGroup.全部共通;
				}
				//-----------------------------
				#endregion

				foreach( STInputEvent inputEvent in listInputEvent )
				{
					if( !inputEvent.b押された )
						continue;

					long nTime = inputEvent.nTimeStamp - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
					int nPad09 = ( nPad == (int) Eパッド.HP ) ? (int) Eパッド.BD : nPad;		// #27029 2012.1.5 yyagi
					int nInputAdjustTime = bIsAutoPlay[ this.nチャンネル0Atoレーン07[ (int) nPad09 ] ] ? 0 : nInputAdjustTimeMs.Drums;

					bool bHitted = false;

					#region [ (A) ヒットしていればヒット処理して次の inputEvent へ ]
					//-----------------------------
					switch( ( (Eパッド) nPad ) )
					{
						case Eパッド.HH:
							#region [ HHとLC(groupingしている場合) のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.HH )
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CDTX.CChip chipHC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x11, nInputAdjustTime );	// HiHat Close
								CDTX.CChip chipHO = this.r指定時刻に一番近い未ヒットChip( nTime, 0x18, nInputAdjustTime );	// HiHat Open
								CDTX.CChip chipLC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a, nInputAdjustTime );	// LC
								E判定 e判定HC = ( chipHC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHC, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定HO = ( chipHO != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHO, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定LC = ( chipLC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLC, nInputAdjustTime ) : E判定.Miss;
								switch( eHHGroup )
								{
									case EHHGroup.ハイハットのみ打ち分ける:
										#region [ HCとLCのヒット処理 ]
										//-----------------------------
										if( ( e判定HC != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.左シンバルのみ打ち分ける:
										#region [ HCとHOのヒット処理 ]
										//-----------------------------
										if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.全部共通:
										#region [ HC,HO,LCのヒット処理 ]
										//-----------------------------
										if( ( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) ) && ( e判定LC != E判定.Miss ) )
										{
											CDTX.CChip chip;
											CDTX.CChip[] chipArray = new CDTX.CChip[] { chipHC, chipHO, chipLC };
											// ここから、chipArrayをn発生位置の小さい順に並び替える
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip;
											}
											if( chipArray[ 0 ].n発声位置 > chipArray[ 1 ].n発声位置 )
											{
												chip = chipArray[ 0 ];
												chipArray[ 0 ] = chipArray[ 1 ];
												chipArray[ 1 ] = chip;
											}
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip;
											}
											this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 0 ], inputEvent.nVelocity );
											if( chipArray[ 0 ].n発声位置 == chipArray[ 1 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 1 ], inputEvent.nVelocity );
											}
											if( chipArray[ 0 ].n発声位置 == chipArray[ 2 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 2 ], inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HO != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHO.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											}
											else if( chipHO.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;
									//-----------------------------
										#endregion

									default:
										#region [ 全部打ち分け時のヒット処理 ]
										//-----------------------------
										if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;
									//-----------------------------
										#endregion
								}
								if( !bHitted )
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.SD:
							#region [ SDのヒット処理 ]
							//-----------------------------
							if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.SD )	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if( !this.tドラムヒット処理( nTime, Eパッド.SD, this.r指定時刻に一番近い未ヒットChip( nTime, 0x12, nInputAdjustTime ), inputEvent.nVelocity ) )
								break;
							continue;
						//-----------------------------
							#endregion

						case Eパッド.BD:
							#region [ BDのヒット処理 ]
							//-----------------------------
							if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.BD )	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if( !this.tドラムヒット処理( nTime, Eパッド.BD, this.r指定時刻に一番近い未ヒットChip( nTime, 0x13, nInputAdjustTime ), inputEvent.nVelocity ) )
								break;
							continue;
							//-----------------------------
							#endregion

						case Eパッド.HT:
							#region [ HTのヒット処理 ]
							//-----------------------------
							if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.HT )	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if( this.tドラムヒット処理( nTime, Eパッド.HT, this.r指定時刻に一番近い未ヒットChip( nTime, 20, nInputAdjustTime ), inputEvent.nVelocity ) )
								continue;
							break;
						//-----------------------------
							#endregion

						case Eパッド.LT:
							#region [ LTとFT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.LT )	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipLT = this.r指定時刻に一番近い未ヒットChip( nTime, 0x15, nInputAdjustTime );	// LT
								CDTX.CChip chipFT = this.r指定時刻に一番近い未ヒットChip( nTime, 0x17, nInputAdjustTime );	// FT
								E判定 e判定LT = ( chipLT != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLT, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定FT = ( chipFT != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipFT, nInputAdjustTime ) : E判定.Miss;
								switch( eFTGroup )
								{
									case EFTGroup.打ち分ける:
										#region [ LTのヒット処理 ]
										//-----------------------------
										if( e判定LT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, inputEvent.nVelocity );
											bHitted = true;
										}
										break;
									//-----------------------------
										#endregion

									case EFTGroup.共通:
										#region [ LTとFTのヒット処理 ]
										//-----------------------------
										if( ( e判定LT != E判定.Miss ) && ( e判定FT != E判定.Miss ) )
										{
											if( chipLT.n発声位置 < chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, inputEvent.nVelocity );
											}
											else if( chipLT.n発声位置 > chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定LT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定FT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, inputEvent.nVelocity );
											bHitted = true;
										}
										break;
									//-----------------------------
										#endregion
								}
								if( !bHitted )
									break;
								continue;
							}
							//-----------------------------
							#endregion

						case Eパッド.FT:
							#region [ FTとLT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.FT )	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipLT = this.r指定時刻に一番近い未ヒットChip( nTime, 0x15, nInputAdjustTime );	// LT
								CDTX.CChip chipFT = this.r指定時刻に一番近い未ヒットChip( nTime, 0x17, nInputAdjustTime );	// FT
								E判定 e判定LT = ( chipLT != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLT, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定FT = ( chipFT != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipFT, nInputAdjustTime ) : E判定.Miss;
								switch( eFTGroup )
								{
									case EFTGroup.打ち分ける:
										#region [ FTのヒット処理 ]
										//-----------------------------
										if( e判定FT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipFT, inputEvent.nVelocity );
											bHitted = true;
										}
										//-----------------------------
										#endregion
										break;

									case EFTGroup.共通:
										#region [ FTとLTのヒット処理 ]
										//-----------------------------
										if( ( e判定LT != E判定.Miss ) && ( e判定FT != E判定.Miss ) )
										{
											if( chipLT.n発声位置 < chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipLT, inputEvent.nVelocity );
											}
											else if( chipLT.n発声位置 > chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipFT, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipLT, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.FT, chipFT, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定LT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipLT, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定FT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipFT, inputEvent.nVelocity );
											bHitted = true;
										}
										//-----------------------------
										#endregion
										break;
								}
								if( !bHitted )
									break;
								continue;
							}
							//-----------------------------
							#endregion

						case Eパッド.CY:
							#region [ CY(とLCとRD:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.CY )	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipCY = this.r指定時刻に一番近い未ヒットChip( nTime, 0x16, nInputAdjustTime );	// CY
								CDTX.CChip chipRD = this.r指定時刻に一番近い未ヒットChip( nTime, 0x19, nInputAdjustTime );	// RD
								CDTX.CChip chipLC = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a, nInputAdjustTime ) : null;
								E判定 e判定CY = ( chipCY != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipCY, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定RD = ( chipRD != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipRD, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定LC = ( chipLC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLC, nInputAdjustTime ) : E判定.Miss;
								CDTX.CChip[] chipArray = new CDTX.CChip[] { chipCY, chipRD, chipLC };
								E判定[] e判定Array = new E判定[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;	// chipArray.GetLength(0)

								//num8 = 0;
								//while( num8 < 2 )

								// CY/RD/LC群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime( chipArray, e判定Array, NumOfChips );
								//for ( int i = 0; i < NumOfChips - 1; i++ )
								//{
								//    //num9 = 2;
								//    //while( num9 > num8 )
								//    for ( int j = NumOfChips - 1; j > i; j-- )
								//    {
								//        if ( ( chipArray[ j - 1 ] == null ) || ( ( chipArray[ j ] != null ) && ( chipArray[ j - 1 ].n発声位置 > chipArray[ j ].n発声位置 ) ) )
								//        {
								//            // swap
								//            CDTX.CChip chipTemp = chipArray[ j - 1 ];
								//            chipArray[ j - 1 ] = chipArray[ j ];
								//            chipArray[ j ] = chipTemp;
								//            E判定 e判定Temp = e判定Array[ j - 1 ];
								//            e判定Array[ j - 1 ] = e判定Array[ j ];
								//            e判定Array[ j ] = e判定Temp;
								//        }
								//        //num9--;
								//    }
								//    //num8++;
								//}
								switch( eCYGroup )
								{
									case ECYGroup.打ち分ける:
										if( !CDTXMania.ConfigIni.bシンバルフリー )
										{
											if( e判定CY != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.CY, chipCY, inputEvent.nVelocity );
												bHitted = true;
											}
											if( !bHitted )
												break;
											continue;
										}
										//num10 = 0;
										//while ( num10 < NumOfChips )
										for( int i = 0; i < NumOfChips; i++ )
										{
											if( ( e判定Array[ i ] != E判定.Miss ) && ( ( chipArray[ i ] == chipCY ) || ( chipArray[ i ] == chipLC ) ) )
											{
												this.tドラムヒット処理( nTime, Eパッド.CY, chipArray[ i ], inputEvent.nVelocity );
												bHitted = true;
												break;
											}
											//num10++;
										}
										if( e判定CY != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.CY, chipCY, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;

									case ECYGroup.共通:
										if( !CDTXMania.ConfigIni.bシンバルフリー )
										{
											//num12 = 0;
											//while ( num12 < NumOfChips )
											for( int i = 0; i < NumOfChips; i++ )
											{
												if( ( e判定Array[ i ] != E判定.Miss ) && ( ( chipArray[ i ] == chipCY ) || ( chipArray[ i ] == chipRD ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipArray[ i ], inputEvent.nVelocity );
													bHitted = true;
													break;
												}
												//num12++;
											}
											if( !bHitted )
												break;
											continue;
										}
										//num11 = 0;
										//while ( num11 < NumOfChips )
										for( int i = 0; i < NumOfChips; i++ )
										{
											if( e判定Array[ i ] != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.CY, chipArray[ i ], inputEvent.nVelocity );
												bHitted = true;
												break;
											}
											//num11++;
										}
										if( !bHitted )
											break;
										continue;
								}
								if( !bHitted )
									break;
								continue;
							}
							//-----------------------------
							#endregion

						case Eパッド.HHO:
							#region [ HO(とHCとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.HH )
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CDTX.CChip chipHC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x11, nInputAdjustTime );	// HC
								CDTX.CChip chipHO = this.r指定時刻に一番近い未ヒットChip( nTime, 0x18, nInputAdjustTime );	// HO
								CDTX.CChip chipLC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a, nInputAdjustTime );	// LC
								E判定 e判定HC = ( chipHC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHC, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定HO = ( chipHO != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHO, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定LC = ( chipLC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLC, nInputAdjustTime ) : E判定.Miss;
								switch( eHHGroup )
								{
									case EHHGroup.全部打ち分ける:
										if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;

									case EHHGroup.ハイハットのみ打ち分ける:
										if( ( e判定HO != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHO.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											else if( chipHO.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;

									case EHHGroup.左シンバルのみ打ち分ける:
										if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;

									case EHHGroup.全部共通:
										if( ( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) ) && ( e判定LC != E判定.Miss ) )
										{
											CDTX.CChip chip;
											CDTX.CChip[] chipArray = new CDTX.CChip[] { chipHC, chipHO, chipLC };
											// ここから、chipArrayをn発生位置の小さい順に並び替える
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip;
											}
											if( chipArray[ 0 ].n発声位置 > chipArray[ 1 ].n発声位置 )
											{
												chip = chipArray[ 0 ];
												chipArray[ 0 ] = chipArray[ 1 ];
												chipArray[ 1 ] = chip;
											}
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip;
											}
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray[ 0 ], inputEvent.nVelocity );
											if( chipArray[ 0 ].n発声位置 == chipArray[ 1 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray[ 1 ], inputEvent.nVelocity );
											}
											if( chipArray[ 0 ].n発声位置 == chipArray[ 2 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray[ 2 ], inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
											}
											else if( chipHC.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( ( e判定HO != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHO.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											}
											else if( chipHO.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											}
											bHitted = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity );
											bHitted = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity );
											bHitted = true;
										}
										if( !bHitted )
											break;
										continue;
								}
								if( !bHitted )
									break;
								continue;
							}
							//-----------------------------
							#endregion

						case Eパッド.RD:
							#region [ RD(とCYとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.RD )	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipCY = this.r指定時刻に一番近い未ヒットChip( nTime, 0x16, nInputAdjustTime );	// CY
								CDTX.CChip chipRD = this.r指定時刻に一番近い未ヒットChip( nTime, 0x19, nInputAdjustTime );	// RD
								CDTX.CChip chipLC = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a, nInputAdjustTime ) : null;
								E判定 e判定CY = ( chipCY != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipCY, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定RD = ( chipRD != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipRD, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定LC = ( chipLC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLC, nInputAdjustTime ) : E判定.Miss;
								CDTX.CChip[] chipArray = new CDTX.CChip[] { chipCY, chipRD, chipLC };
								E判定[] e判定Array = new E判定[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;	// chipArray.GetLength(0)

								//num13 = 0;
								//while( num13 < 2 )

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime( chipArray, e判定Array, NumOfChips );
								//for ( int i = 0; i < NumOfChips - 1; i++ )
								//{
								//    // num14 = 2;
								//    // while( num14 > num13 )
								//    for (int j = NumOfChips - 1; j > i; j--)
								//    {
								//        if( ( chipArray[ j - 1 ] == null ) || ( ( chipArray[ j ] != null ) && ( chipArray[ j - 1 ].n発声位置 > chipArray[ j ].n発声位置 ) ) )
								//        {
								//            CDTX.CChip chipTemp = chipArray[ j - 1 ];
								//            chipArray[ j - 1 ] = chipArray[ j ];
								//            chipArray[ j ] = chipTemp;
								//            E判定 e判定Temp = e判定Array[ j - 1 ];
								//            e判定Array[ j - 1 ] = e判定Array[ j ];
								//            e判定Array[ j ] = e判定Temp;
								//        }
								//        //num14--;
								//    }
								//    // num13++;
								//}
								switch( eCYGroup )
								{
									case ECYGroup.打ち分ける:
										if( e判定RD != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.RD, chipRD, inputEvent.nVelocity );
											bHitted = true;
										}
										break;

									case ECYGroup.共通:
										if( !CDTXMania.ConfigIni.bシンバルフリー )
										{
											//num16 = 0;
											//while( num16 < 3 )
											for( int i = 0; i < NumOfChips; i++ )
											{
												if( ( e判定Array[ i ] != E判定.Miss ) && ( ( chipArray[ i ] == chipCY ) || ( chipArray[ i ] == chipRD ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipArray[ i ], inputEvent.nVelocity );
													bHitted = true;
													break;
												}
												//num16++;
											}
											break;
										}
										//num15 = 0;
										//while( num15 < 3 )
										for( int i = 0; i < NumOfChips; i++ )
										{
											if( e判定Array[ i ] != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.CY, chipArray[ i ], inputEvent.nVelocity );
												bHitted = true;
												break;
											}
											//num15++;
										}
										break;
								}
								if( bHitted )
								{
									continue;
								}
								break;
							}
							//-----------------------------
							#endregion

						case Eパッド.LC:
							#region [ LC(とHC/HOとCYと:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if( inputEvent.nVelocity <= CDTXMania.ConfigIni.nVelocityMin.LC )	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipHC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x11, nInputAdjustTime );	// HC
								CDTX.CChip chipHO = this.r指定時刻に一番近い未ヒットChip( nTime, 0x18, nInputAdjustTime );	// HO
								CDTX.CChip chipLC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a, nInputAdjustTime );	// LC
								CDTX.CChip chipCY = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip( nTime, 0x16, nInputAdjustTime ) : null;
								CDTX.CChip chipRD = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip( nTime, 0x19, nInputAdjustTime ) : null;
								E判定 e判定HC = ( chipHC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHC, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定HO = ( chipHO != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipHO, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定LC = ( chipLC != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipLC, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定CY = ( chipCY != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipCY, nInputAdjustTime ) : E判定.Miss;
								E判定 e判定RD = ( chipRD != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipRD, nInputAdjustTime ) : E判定.Miss;
								CDTX.CChip[] chipArray = new CDTX.CChip[] { chipHC, chipHO, chipLC, chipCY, chipRD };
								E判定[]  e判定Array = new E判定[] { e判定HC, e判定HO, e判定LC, e判定CY, e判定RD };
								const int NumOfChips = 5;	// chipArray.GetLength(0)

								//num3 = 0;
								//while( num3 < 4 )

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime( chipArray, e判定Array, NumOfChips );
								//for ( int i = 0; i < NumOfChips - 1; i++ )
								//{
								//    //num4 = 4;
								//    //while( num4 > num3 )
								//    for ( int j = NumOfChips - 1; j > i; j-- )
								//    {
								//        if( ( chipArray[ j - 1 ] == null ) || ( ( chipArray[ j ] != null ) && ( chipArray[ j - 1 ].n発声位置 > chipArray[ j ].n発声位置 ) ) )
								//        {
								//            // swap
								//            CDTX.CChip chipTemp = chipArray[ j - 1 ];
								//            chipArray[ j - 1 ] = chipArray[ j ];
								//            chipArray[ j ] = chipTemp;
								//            E判定 e判定Temp = e判定Array[ j - 1 ];
								//            e判定Array[ j - 1 ] = e判定Array[ j ];
								//            e判定Array[ j ] = e判定Temp;
								//        }
								//        //num4--;
								//    }
								//    //num3++;
								//}
								switch( eHHGroup )
								{
									case EHHGroup.全部打ち分ける:
									case EHHGroup.左シンバルのみ打ち分ける:
										if( !CDTXMania.ConfigIni.bシンバルフリー )
										{
											if( e判定LC != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.LC, chipLC, inputEvent.nVelocity );
												bHitted = true;
											}
											if( !bHitted )
												break;
											continue;
										}
										//num5 = 0;
										//while( num5 < 5 )
										for( int i = 0; i < NumOfChips; i++ )
										{
											if( ( e判定Array[ i ] != E判定.Miss ) && ( ( ( chipArray[ i ] == chipLC ) || ( chipArray[ i ] == chipCY ) ) || ( ( chipArray[ i ] == chipRD ) && ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 ) ) ) )
											{
												this.tドラムヒット処理( nTime, Eパッド.LC, chipArray[ i ], inputEvent.nVelocity );
												bHitted = true;
												break;
											}
											//num5++;
										}
										if( !bHitted )
											break;
										continue;

									case EHHGroup.ハイハットのみ打ち分ける:
									case EHHGroup.全部共通:
										if( !CDTXMania.ConfigIni.bシンバルフリー )
										{
											//num7 = 0;
											//while( num7 < 5 )
											for( int i = 0; i < NumOfChips; i++ )
											{
												if( ( e判定Array[ i ] != E判定.Miss ) && ( ( ( chipArray[ i ] == chipLC ) || ( chipArray[ i ] == chipHC ) ) || ( chipArray[ i ] == chipHO ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.LC, chipArray[ i ], inputEvent.nVelocity );
													bHitted = true;
													break;
												}
												//num7++;
											}
											if( !bHitted )
												break;
											continue;
										}
										//num6 = 0;
										//while( num6 < 5 )
										for( int i = 0; i < NumOfChips; i++ )
										{
											if( ( e判定Array[ i ] != E判定.Miss ) && ( ( chipArray[ i ] != chipRD ) || ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 ) ) )
											{
												this.tドラムヒット処理( nTime, Eパッド.LC, chipArray[ i ], inputEvent.nVelocity );
												bHitted = true;
												break;
											}
											//num6++;
										}
										if( !bHitted )
											break;
										continue;
								}
								if( !bHitted )
									break;

								break;
							}
							//-----------------------------
							#endregion

						case Eパッド.HP:		// #27029 2012.1.4 from
							#region [ HPのヒット処理 ]
							//-----------------
							if( CDTXMania.ConfigIni.eBDGroup == EBDGroup.どっちもBD )
							{
								#region [ BDとみなしてヒット処理 ]
								//-----------------
								if( !this.tドラムヒット処理( nTime, Eパッド.BD, this.r指定時刻に一番近い未ヒットChip( nTime, 0x13, nInputAdjustTime ), inputEvent.nVelocity ) )
									break;
								continue;
								//-----------------
								#endregion
							}
							else
							{
								#region [ HPのヒット処理 ]
								//-----------------
								continue;	// 何もしない。この入力を完全に無視するので、break しないこと。
								//-----------------
								#endregion
							}
							//-----------------
							#endregion
					}
					//-----------------------------
					#endregion
					#region [ (B) ヒットしてなかった場合は、レーンフラッシュ、パッドアニメ、空打ち音再生を実行 ]
					//-----------------------------
					int pad = nPad;	// 以下、nPad の代わりに pad を用いる。（成りすまし用）

					if( nPad == (int) Eパッド.HP )	// #27029 2012.1.4 from: HP&BD 時の HiHatPedal の場合は BD に成りすます。
						pad = (int) Eパッド.BD;		//（ HP|BD 時のHP入力はここまでこないので無視。）

					// レーンフラッシュ
					this.actLaneFlushD.Start( (Eレーン) this.nパッド0Atoレーン07[ pad ], ( (float) inputEvent.nVelocity ) / 127f );

					// パッドアニメ
					this.actPad.Hit( this.nパッド0Atoパッド08[ pad ] );

					// 空打ち音
					if( CDTXMania.ConfigIni.bドラム打音を発声する )
					{
						CDTX.CChip rChip = this.r空うちChip( E楽器パート.DRUMS, (Eパッド) pad );
						if( rChip != null )
						{
							#region [ (B1) 空打ち音が譜面で指定されているのでそれを再生する。]
							//-----------------
							this.tサウンド再生( rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
							//-----------------
							#endregion
						}
						else
						{
							#region [ (B2) 空打ち音が指定されていないので一番近いチップを探して再生する。]
							//-----------------
							switch( ( (Eパッド) pad ) )
							{
								case Eパッド.HH:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipHC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 0 ], nInputAdjustTime );
										CDTX.CChip chipHO = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 7 ], nInputAdjustTime );
										CDTX.CChip chipLC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 9 ], nInputAdjustTime );
										switch( CDTXMania.ConfigIni.eHHGroup )
										{
											case EHHGroup.ハイハットのみ打ち分ける:
												rChip = ( chipHC != null ) ? chipHC : chipLC;
												break;

											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = ( chipHC != null ) ? chipHC : chipHO;
												break;

											case EHHGroup.全部共通:
												if( chipHC != null )
												{
													rChip = chipHC;
												}
												else if( chipHO == null )
												{
													rChip = chipLC;
												}
												else if( chipLC == null )
												{
													rChip = chipHO;
												}
												else if( chipHO.n発声位置 < chipLC.n発声位置 )
												{
													rChip = chipHO;
												}
												else
												{
													rChip = chipLC;
												}
												break;

											default:
												rChip = chipHC;
												break;
										}
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.LT:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipLT = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 4 ], nInputAdjustTime );
										CDTX.CChip chipFT = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 5 ], nInputAdjustTime );
										if( CDTXMania.ConfigIni.eFTGroup != EFTGroup.打ち分ける )
											rChip = ( chipLT != null ) ? chipLT : chipFT;
										else
											rChip = chipLT;
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.FT:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipLT = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 4 ], nInputAdjustTime );
										CDTX.CChip chipFT = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 5 ], nInputAdjustTime );
										if( CDTXMania.ConfigIni.eFTGroup != EFTGroup.打ち分ける )
											rChip = ( chipFT != null ) ? chipFT : chipLT;
										else
											rChip = chipFT;
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.CY:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipCY = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 6 ], nInputAdjustTime );
										CDTX.CChip chipRD = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 8 ], nInputAdjustTime );
										if( CDTXMania.ConfigIni.eCYGroup != ECYGroup.打ち分ける )
											rChip = ( chipCY != null ) ? chipCY : chipRD;
										else
											rChip = chipCY;
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.HHO:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipHC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 0 ], nInputAdjustTime );
										CDTX.CChip chipHO = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 7 ], nInputAdjustTime );
										CDTX.CChip chipLC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 9 ], nInputAdjustTime );
										switch( CDTXMania.ConfigIni.eHHGroup )
										{
											case EHHGroup.全部打ち分ける:
												rChip = chipHO;
												break;

											case EHHGroup.ハイハットのみ打ち分ける:
												rChip = ( chipHO != null ) ? chipHO : chipLC;
												break;

											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = ( chipHO != null ) ? chipHO : chipHC;
												break;

											case EHHGroup.全部共通:
												if( chipHO != null )
												{
													rChip = chipHO;
												}
												else if( chipHC == null )
												{
													rChip = chipLC;
												}
												else if( chipLC == null )
												{
													rChip = chipHC;
												}
												else if( chipHC.n発声位置 < chipLC.n発声位置 )
												{
													rChip = chipHC;
												}
												else
												{
													rChip = chipLC;
												}
												break;
										}
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.RD:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipCY = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 6 ], nInputAdjustTime );
										CDTX.CChip chipRD = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 8 ], nInputAdjustTime );
										if( CDTXMania.ConfigIni.eCYGroup != ECYGroup.打ち分ける )
											rChip = ( chipRD != null ) ? chipRD : chipCY;
										else
											rChip = chipRD;
									}
									//-----------------------------
									#endregion
									break;

								case Eパッド.LC:
									#region [ *** ]
									//-----------------------------
									{
										CDTX.CChip chipHC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 0 ], nInputAdjustTime );
										CDTX.CChip chipHO = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 7 ], nInputAdjustTime );
										CDTX.CChip chipLC = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 9 ], nInputAdjustTime );
										switch( CDTXMania.ConfigIni.eHHGroup )
										{
											case EHHGroup.全部打ち分ける:
											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = chipLC;
												break;

											case EHHGroup.ハイハットのみ打ち分ける:
											case EHHGroup.全部共通:
												if( chipLC != null )
												{
													rChip = chipLC;
												}
												else if( chipHC == null )
												{
													rChip = chipHO;
												}
												else if( chipHO == null )
												{
													rChip = chipHC;
												}
												else if( chipHC.n発声位置 < chipHO.n発声位置 )
												{
													rChip = chipHC;
												}
												else
												{
													rChip = chipHO;
												}
												break;
										}
									}
									//-----------------------------
									#endregion
									break;

								default:
									#region [ *** ]
									//-----------------------------
									rChip = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ pad ], nInputAdjustTime );
									//-----------------------------
									#endregion
									break;
							}
							if( rChip != null )
							{
								// 空打ち音が見つかったので再生する。
								this.tサウンド再生( rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
							}
							//-----------------
							#endregion
						}
					}
					
					// BAD or TIGHT 時の処理。
					if( CDTXMania.ConfigIni.bTight )
						this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.DRUMS, this.nパッド0Atoレーン07[ pad ] );
					//-----------------------------
					#endregion
				}
			}
		}

		// t入力処理・ドラム()からメソッドを抽出したもの。
		/// <summary>
		/// chipArrayの中を, n発生位置の小さい順に並べる + nullを大きい方に退かす。セットでe判定Arrayも並べ直す。
		/// </summary>
		/// <param name="chipArray">ソート対象chip群</param>
		/// <param name="e判定Array">ソート対象e判定群</param>
		/// <param name="NumOfChips">チップ数</param>
		private static void SortChipsByNTime( CDTX.CChip[] chipArray, E判定[] e判定Array, int NumOfChips )
		{
			for ( int i = 0; i < NumOfChips - 1; i++ )
			{
				//num9 = 2;
				//while( num9 > num8 )
				for ( int j = NumOfChips - 1; j > i; j-- )
				{
					if ( ( chipArray[ j - 1 ] == null ) || ( ( chipArray[ j ] != null ) && ( chipArray[ j - 1 ].n発声位置 > chipArray[ j ].n発声位置 ) ) )
					{
						// swap
						CDTX.CChip chipTemp = chipArray[ j - 1 ];
						chipArray[ j - 1 ] = chipArray[ j ];
						chipArray[ j ] = chipTemp;
						E判定 e判定Temp = e判定Array[ j - 1 ];
						e判定Array[ j - 1 ] = e判定Array[ j ];
						e判定Array[ j ] = e判定Temp;
					}
					//num9--;
				}
				//num8++;
			}
		}

		protected override void t背景テクスチャの生成()
		{
			Rectangle bgrect = new Rectangle( 338, 57, 278, 355 );
			string DefaultBgFilename = @"Graphics\ScreenPlayDrums background.jpg";
			string BgFilename = "";
			if ( ( ( CDTXMania.DTX.BACKGROUND != null ) && ( CDTXMania.DTX.BACKGROUND.Length > 0 ) ) && !CDTXMania.ConfigIni.bストイックモード )
			{
				BgFilename = CDTXMania.DTX.strフォルダ名 + CDTXMania.DTX.BACKGROUND;
			}
			base.t背景テクスチャの生成( DefaultBgFilename, bgrect, BgFilename );
		}

		protected override void t進行描画・チップ・ドラムス( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( configIni.bDrums有効 )
			{
				#region [ Invisible処理 ]
				if ( configIni.eInvisible.Drums != EInvisible.OFF )
				{
					cInvisibleChip.SetInvisibleStatus( ref pChip );
				}
				#endregion
				else
				{
					#region [ Sudden処理 ]
					if ( configIni.bSudden.Drums )
					{
						if ( pChip.nバーからの距離dot.Drums < 200 )
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff;
						}
						else if ( pChip.nバーからの距離dot.Drums < 250 )
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff - ( (int) ( ( ( (double) ( pChip.nバーからの距離dot.Drums - 200 ) ) * 255.0 ) / 50.0 ) );
						}
						else
						{
							pChip.b可視 = false;
							pChip.n透明度 = 0;
						}
					}
					#endregion
					#region [ Hidden処理 ]
					if ( configIni.bHidden.Drums )
					{
						if ( pChip.nバーからの距離dot.Drums < 100 )
						{
							pChip.b可視 = false;
						}
						else if ( pChip.nバーからの距離dot.Drums < 150 )
						{
							pChip.b可視 = true;
							pChip.n透明度 = (int) ( ( ( (double) ( pChip.nバーからの距離dot.Drums - 100 ) ) * 255.0 ) / 50.0 );
						}
					}
					#endregion
				}
				if ( !pChip.bHit && pChip.b可視 )
				{
					if ( this.txチップ != null )
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int x = this.nチャンネルtoX座標[ pChip.nチャンネル番号 - 0x11 ];
					int y = configIni.bReverse.Drums ? ( 0x38 + pChip.nバーからの距離dot.Drums ) : ( 0x1a6 - pChip.nバーからの距離dot.Drums );
					if ( this.txチップ != null )
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3( (float) pChip.dbチップサイズ倍率, (float) pChip.dbチップサイズ倍率, 1f );
					}
					int num9 = this.ctチップ模様アニメ.Drums.n現在の値;
					switch ( pChip.nチャンネル番号 )
					{
						case 0x11:
							x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0x2c, num9 * 7, 0x20, 8 ) );
							}
							break;

						case 0x12:
							x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0x4c, num9 * 7, 0x20, 8 ) );
							}
							break;

						case 0x13:
							x = ( x + 0x16 ) - ( (int) ( ( 44.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 5, new Rectangle( 0, num9 * 9, 0x2c, 10 ) );
							}
							break;

						case 0x14:
							x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0x6c, num9 * 7, 0x20, 8 ) );
							}
							break;

						case 0x15:
							x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 140, num9 * 7, 0x20, 8 ) );
							}
							break;

						case 0x16:
							x = ( x + 0x13 ) - ( (int) ( ( 38.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 0x15, new Rectangle( 0xcc, 0x158, 0x26, 0x24 ) );
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0xcc, num9 * 7, 0x26, 8 ) );
							}
							break;

						case 0x17:
							x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0xac, num9 * 7, 0x20, 8 ) );
							}
							break;

						case 0x18:
							x = ( x + 13 ) - ( (int) ( ( 26.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0xf2, num9 * 7, 0x1a, 8 ) );
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 9, new Rectangle( 0xf2, 0x158, 0x1a, 0x12 ) );
							}
							break;

						case 0x19:
							x = ( x + 13 ) - ( (int) ( ( 26.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0xf2, num9 * 7, 0x1a, 8 ) );
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 9, new Rectangle( 0xf2, 0x158, 0x1a, 0x12 ) );
							}
							break;

						case 0x1a:
							x = ( x + 0x13 ) - ( (int) ( ( 38.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 0x15, new Rectangle( 0xcc, 0x158, 0x26, 0x24 ) );
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 4, new Rectangle( 0xcc, num9 * 7, 0x26, 8 ) );
							}
							break;
					}
					if ( this.txチップ != null )
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
						this.txチップ.n透明度 = 0xff;
					}
				}

				int indexSevenLanes = this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ];
				if ( ( configIni.bAutoPlay[ indexSevenLanes ] && !pChip.bHit ) && ( pChip.nバーからの距離dot.Drums < 0 ) )
				{
					pChip.bHit = true;
					this.actLaneFlushD.Start( (Eレーン) indexSevenLanes, ( (float) CInput管理.n通常音量 ) / 127f );
					bool flag = this.bフィルイン中;
					bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである( pChip );
					//bool flag3 = flag2;
					// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
					this.actChipFireD.Start( (Eレーン)indexSevenLanes, flag, flag2, flag2, 演奏判定ライン座標.nJudgeLinePosY_delta.Drums );
					this.actPad.Hit( this.nチャンネル0Atoパッド08[ pChip.nチャンネル番号 - 0x11 ] );
					this.tサウンド再生( pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, dTX.nモニタを考慮した音量( E楽器パート.DRUMS ) );
					this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
					cInvisibleChip.StartSemiInvisible( E楽器パート.DRUMS );
				}
				//break;
				return;
			}	// end of "if configIni.bDrums有効"
			if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
			{
				this.tサウンド再生( pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, dTX.nモニタを考慮した音量( E楽器パート.DRUMS ) );
				pChip.bHit = true;
			}
		}
		protected override void t進行描画・チップ・ギターベース( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip, E楽器パート inst )
		{
			base.t進行描画・チップ・ギターベース( configIni, ref dTX, ref pChip, inst,
				// 95, 374,
				演奏判定ライン座標.n判定ラインY座標( inst, false, false ),		// 95  Normal
				演奏判定ライン座標.n判定ラインY座標( inst,   false, true ),		// 374 Reverse
				57, 412,				// チップのY軸表示範囲
				509, 400,
				268, 144, 76, 6,		// オープンチップの x, y, w, h
				24, 509, 561, 400, 452, 26, 24 );
		}
#if false
		protected override void t進行描画・チップ・ギターベース( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip, E楽器パート inst )
		{
			int instIndex = (int) inst;
			if ( configIni.bGuitar有効 )
			{
				if ( configIni.bSudden[ instIndex ] )
				{
					pChip.b可視 = pChip.nバーからの距離dot[ instIndex ] < 200;
				}
				if ( configIni.bHidden[ instIndex ] && ( pChip.nバーからの距離dot[ instIndex ] < 100 ) )
				{
					pChip.b可視 = false;
				}

				bool bChipHasR = ( ( pChip.nチャンネル番号 & 4 ) > 0 );
				bool bChipHasG = ( ( pChip.nチャンネル番号 & 2 ) > 0 );
				bool bChipHasB = ( ( pChip.nチャンネル番号 & 1 ) > 0 );
				bool bChipHasW = ( ( pChip.nチャンネル番号 & 0x0F ) == 0x08 );
				bool bChipIsO  = ( ( pChip.nチャンネル番号 & 0x0F ) == 0x00 );

				int OPEN = (inst == E楽器パート.GUITAR ) ? 0x20 : 0xA0;
				if ( !pChip.bHit && pChip.b可視 )
				{
					int y = configIni.bReverse[ instIndex ] ? ( 374 - pChip.nバーからの距離dot[ instIndex ] ) : ( 95 + pChip.nバーからの距離dot[ instIndex ] );
					if ( ( y > 57 ) && ( y < 412 ) )
					{
						if ( this.txチップ != null )
						{
							int nアニメカウンタ現在の値 = this.ctチップ模様アニメ[ instIndex ].n現在の値;
							if ( pChip.nチャンネル番号 == OPEN )
							{
								int xo = ( inst == E楽器パート.GUITAR ) ? 509 : 400;
								this.txチップ.t2D描画( CDTXMania.app.Device, xo, y - 2, new Rectangle( 268, 144 + ( ( nアニメカウンタ現在の値 % 5 ) * 6 ), 76, 6 ) );
							}
							Rectangle rc = new Rectangle( 268, nアニメカウンタ現在の値 * 6, 24, 6 );
							int x;
							if ( inst == E楽器パート.GUITAR )
							{
								x = ( configIni.bLeft.Guitar ) ? 561 : 509;
							}
							else
							{
								x = ( configIni.bLeft.Bass ) ? 452 : 400;
							}
							int deltaX = ( configIni.bLeft[ instIndex ] ) ? -26 : +26;

							if ( bChipHasR )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 3, rc );
							}
							rc.X += 24;
							x += deltaX;
							if ( bChipHasG )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 3, rc );
							}
							rc.X += 24;
							x += deltaX;
							if ( bChipHasB )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, x, y - 3, rc );
							}
						}
					}
				}
				//if ( ( configIni.bAutoPlay.Guitar && !pChip.bHit ) && ( pChip.nバーからの距離dot.Guitar < 0 ) )
				if ( ( !pChip.bHit ) && ( pChip.nバーからの距離dot[ instIndex ] < 0 ) )
				{
					int lo = ( inst == E楽器パート.GUITAR ) ? 0 : 3;	// lane offset
					bool autoR = ( inst == E楽器パート.GUITAR ) ? bIsAutoPlay.GtR : bIsAutoPlay.BsR;
					bool autoG = ( inst == E楽器パート.GUITAR ) ? bIsAutoPlay.GtG : bIsAutoPlay.BsG;
					bool autoB = ( inst == E楽器パート.GUITAR ) ? bIsAutoPlay.GtB : bIsAutoPlay.BsB;
					if ( ( bChipHasR || bChipIsO ) && autoR )
					{
						this.actChipFireGB.Start( 0 + lo);
					}
					if ( ( bChipHasG || bChipIsO ) && autoG )
					{
						this.actChipFireGB.Start( 1 + lo );
					}
					if ( ( bChipHasB || bChipIsO ) && autoB )
					{
						this.actChipFireGB.Start( 2 + lo );
					}
					if ( (inst == E楽器パート.GUITAR && bIsAutoPlay.GtPick ) || (inst == E楽器パート.BASS && bIsAutoPlay.BsPick ) )
					{
						bool pushingR = CDTXMania.Pad.b押されている( inst, Eパッド.R );
						bool pushingG = CDTXMania.Pad.b押されている( inst, Eパッド.G );
						bool pushingB = CDTXMania.Pad.b押されている( inst, Eパッド.B );
						bool bMiss = true;
						if ( ( ( bChipIsO == true ) && ( !pushingR | autoR ) && ( !pushingG | autoG ) && ( !pushingB | autoB ) ) ||
							( ( bChipHasR == ( pushingR | autoR ) ) && ( bChipHasG == ( pushingG | autoG ) ) && ( bChipHasB == ( pushingB | autoB ) ) )
						)
						{
							bMiss = false;
						}
						pChip.bHit = true;
						this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, inst, dTX.nモニタを考慮した音量( inst ), false, bMiss );
						this.r次にくるギターChip = null;
						this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
					}
				}
				//break;
				return;
			}	// end of "if configIni.bGuitar有効"
			if ( !pChip.bHit && ( pChip.nバーからの距離dot[ instIndex ] < 0 ) )	// Guitar/Bass無効の場合は、自動演奏する
			{
				pChip.bHit = true;
				this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, inst, dTX.nモニタを考慮した音量( inst ) );
			}
		}
#endif
		protected override void t進行描画・チップ・ギター・ウェイリング( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( configIni.bGuitar有効 )
			{
				//if ( configIni.bSudden.Guitar )
				//{
				//    pChip.b可視 = pChip.nバーからの距離dot.Guitar < 200;
				//}
				//if ( configIni.bHidden.Guitar && ( pChip.nバーからの距離dot.Guitar < 100 ) )
				//{
				//    pChip.b可視 = false;
				//}

				// 後日、以下の部分を何とかCStage演奏画面共通.csに移したい。
				if ( !pChip.bHit && pChip.b可視 )
				{
					if ( this.txチップ != null )
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int[] y_base = {
						演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, false ),	// 95
						演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, true )		// 374
					};			// ドラム画面かギター画面かで変わる値
					//int[] y_base = { 0x5f, 0x176 };		// 判定バーのY座標: ドラム画面かギター画面かで変わる値
					int offset = 0x39;					// ドラム画面かギター画面かで変わる値

					const int WailingWidth = 20;		// ウェイリングチップ画像の幅: 4種全て同じ値
					const int WailingHeight = 50;		// ウェイリングチップ画像の高さ: 4種全て同じ値
					const int baseTextureOffsetX = 268;	// テクスチャ画像中のウェイリングチップ画像の位置X: ドラム画面かギター画面かで変わる値
					const int baseTextureOffsetY = 174;	// テクスチャ画像中のウェイリングチップ画像の位置Y: ドラム画面かギター画面かで変わる値
					const int drawX = 588;				// ウェイリングチップ描画位置X座標: 4種全て異なる値

					const int numA = 25;				// 4種全て同じ値
					int y = configIni.bReverse.Guitar ? ( y_base[1] - pChip.nバーからの距離dot.Guitar ) : ( y_base[0] + pChip.nバーからの距離dot.Guitar );
					int numB = y - offset;				// 4種全て同じ定義
					int numC = 0;						// 4種全て同じ初期値
					const int numD = 355;				// ドラム画面かギター画面かで変わる値
					if ( ( numB < ( numD + numA ) ) && ( numB > -numA ) )	// 以下のロジックは4種全て同じ
					{
						int c = this.ctWailingチップ模様アニメ.n現在の値;
						Rectangle rect = new Rectangle( baseTextureOffsetX + ( c * WailingWidth ), baseTextureOffsetY, WailingWidth, WailingHeight);
						if ( numB < numA )
						{
							rect.Y += numA - numB;
							rect.Height -= numA - numB;
							numC = numA - numB;
						}
						if ( numB > ( numD - numA ) )
						{
							rect.Height -= numB - ( numD - numA );
						}
						if ( ( rect.Bottom > rect.Top ) && ( this.txチップ != null ) )
						{
							this.txチップ.t2D描画( CDTXMania.app.Device, drawX, ( y - numA ) + numC, rect );
						}
					}
				}
				//    if ( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
				//    {
				//        if ( pChip.nバーからの距離dot.Guitar < -234 )	// #25253 2011.5.29 yyagi: Don't set pChip.bHit=true for wailing at once. It need to 1sec-delay (234pix per 1sec). 
				//        {
				//            pChip.bHit = true;
				//        }
				//        if ( configIni.bAutoPlay.Guitar )
				//        {
				//            pChip.bHit = true;						// #25253 2011.5.29 yyagi: Set pChip.bHit=true if autoplay.
				//            this.actWailingBonus.Start( E楽器パート.GUITAR, this.r現在の歓声Chip.Guitar );
				//        }
				//    }
				//    return;
				//}
				//pChip.bHit = true;
			}
			base.t進行描画・チップ・ギター・ウェイリング( configIni, ref dTX, ref pChip );
		}
		protected override void t進行描画・チップ・フィルイン( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
			{
				pChip.bHit = true;
				switch ( pChip.n整数値 )
				{
					case 0x01:	// フィルイン開始
						if ( configIni.bフィルイン有効 )
						{
							this.bフィルイン中 = true;
						}
						break;

					case 0x02:	// フィルイン終了
						if ( configIni.bフィルイン有効 )
						{
							this.bフィルイン中 = false;
						}
						if ( ( ( this.actCombo.n現在のコンボ数.Drums > 0 ) || configIni.bドラムが全部オートプレイである ) && configIni.b歓声を発声する )
						{
							if ( this.r現在の歓声Chip.Drums != null )
							{
								dTX.tチップの再生( this.r現在の歓声Chip.Drums, CSound管理.rc演奏用タイマ.nシステム時刻, (int) Eレーン.BGM, dTX.nモニタを考慮した音量( E楽器パート.UNKNOWN ) );
							}
							else
							{
								CDTXMania.Skin.sound歓声音.n位置・次に鳴るサウンド = 0;
								CDTXMania.Skin.sound歓声音.t再生する();
							}
						}
						break;
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi TEST
								case 0x04:	// HH消音あり(従来同等)
									CDTXMania.DTX.b演奏で直前の音を消音する.HH = true;
									break;
								case 0x05:	// HH消音無し
									CDTXMania.DTX.b演奏で直前の音を消音する.HH = false;
									break;
								case 0x06:	// ギター消音あり(従来同等)
									CDTXMania.DTX.b演奏で直前の音を消音する.Gutiar = true;
									break;
								case 0x07:	// ギター消音無し
									CDTXMania.DTX.b演奏で直前の音を消音する.Gutiar = false;
									break;
								case 0x08:	// ベース消音あり(従来同等)
									CDTXMania.DTX.b演奏で直前の音を消音する.Bass = true;
									break;
								case 0x09:	// ベース消音無し
									CDTXMania.DTX.b演奏で直前の音を消音する.Bass = false;
									break;
#endif
				}
			}
		}
#if false
		protected override void t進行描画・チップ・ベース( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( configIni.bGuitar有効 )
			{
				if ( configIni.bSudden.Bass )
				{
					pChip.b可視 = pChip.nバーからの距離dot.Bass < 200;
				}
				if ( configIni.bHidden.Bass && ( pChip.nバーからの距離dot.Bass < 100 ) )
				{
					pChip.b可視 = false;
				}

				bool bGtBsR = ( ( pChip.nチャンネル番号 & 4 ) > 0 );
				bool bGtBsG = ( ( pChip.nチャンネル番号 & 2 ) > 0 );
				bool bGtBsB = ( ( pChip.nチャンネル番号 & 1 ) > 0 );
				bool bGtBsW = ( ( pChip.nチャンネル番号 & 0x0F ) == 0x08 );
				bool bGtBsO = ( ( pChip.nチャンネル番号 & 0x0F ) == 0x00 ); 
				if ( !pChip.bHit && pChip.b可視 )
				{
					int y = configIni.bReverse.Bass ? ( 374 - pChip.nバーからの距離dot.Bass ) : ( 95 + pChip.nバーからの距離dot.Bass );
					if ( ( y > 57 ) && ( y < 412 ) )
					{
						int nアニメカウンタ現在の値 = this.ctチップ模様アニメ.Bass.n現在の値;
						if ( pChip.nチャンネル番号 == 0xA0 )
						{
							if ( this.txチップ != null )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, 400, y - 2, new Rectangle( 268, 144 + ( ( nアニメカウンタ現在の値 % 5 ) * 6 ), 76, 6 ) );
							}
						}
						else if ( this.txチップ != null )
						{
							int x = ( configIni.bLeft.Bass ) ? 452 : 400;
							int deltaX = ( configIni.bLeft.Bass ) ? -24 : +24;
							Rectangle rc = new Rectangle( 268, nアニメカウンタ現在の値 * 6, 24, 6 );
							if ( bGtBsR )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, 400, y - 3, rc );
							}
							rc.X += 24;
							x += deltaX;
							if ( bGtBsG )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, 0x1aa, y - 3, rc );
							}
							rc.X += 24;
							x += deltaX;
							if ( bGtBsB )
							{
								this.txチップ.t2D描画( CDTXMania.app.Device, 0x1c4, y - 3, rc );
							}
						}
					}
				}
				if ( ( configIni.bAutoPlay.Bass && !pChip.bHit ) && ( pChip.nバーからの距離dot.Bass < 0 ) )
				{
					if ( ( bGtBsR || ( pChip.nチャンネル番号 == 0xA0 ) ) && bIsAutoPlay.BsR )
					{
						this.actChipFireGB.Start( 3 );
					}
					if ( ( bGtBsG || ( pChip.nチャンネル番号 == 0xA0 ) ) && bIsAutoPlay.BsG )
					{
						this.actChipFireGB.Start( 4 );
					}
					if ( ( bGtBsB || ( pChip.nチャンネル番号 == 0xA0 ) ) && bIsAutoPlay.BsB )
					{
						this.actChipFireGB.Start( 5 );
					}
					if ( bIsAutoPlay.BsPick )
					{
						pChip.bHit = true;
						this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.BASS, dTX.nモニタを考慮した音量( E楽器パート.BASS ) );
						this.r次にくるベースChip = null;
						this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
					}
				}
				return;
			}
			if ( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
			{
				this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.BASS, dTX.nモニタを考慮した音量( E楽器パート.BASS ) );
				pChip.bHit = true;
			}
		}
#endif
		protected override void t進行描画・チップ・ベース・ウェイリング( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( configIni.bGuitar有効 )
			{
				//if ( configIni.bSudden.Bass )
				//{
				//    pChip.b可視 = pChip.nバーからの距離dot.Bass < 200;
				//}
				//if ( configIni.bHidden.Bass && ( pChip.nバーからの距離dot.Bass < 100 ) )
				//{
				//    pChip.b可視 = false;
				//}

				//
				// 後日、以下の部分を何とかCStage演奏画面共通.csに移したい。
				//
				if ( !pChip.bHit && pChip.b可視 )
				{
					if ( this.txチップ != null )
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int[] y_base = {
						演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, false ),
						演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, true )
					};			// ドラム画面かギター画面かで変わる値
					//int[] y_base = { 0x5f, 0x176 };		// 判定バーのY座標: ドラム画面かギター画面かで変わる値
					int offset = 0x39;					// ドラム画面かギター画面かで変わる値

					const int WailingWidth = 20;		// ウェイリングチップ画像の幅: 4種全て同じ値
					const int WailingHeight = 50;		// ウェイリングチップ画像の高さ: 4種全て同じ値
					const int baseTextureOffsetX = 268;	// テクスチャ画像中のウェイリングチップ画像の位置X: ドラム画面かギター画面かで変わる値
					const int baseTextureOffsetY = 174;	// テクスチャ画像中のウェイリングチップ画像の位置Y: ドラム画面かギター画面かで変わる値
					const int drawX = 479;				// ウェイリングチップ描画位置X座標: 4種全て異なる値

					const int numA = 25;				// 4種全て同じ値
					int y = configIni.bReverse.Bass ? ( y_base[ 1 ] - pChip.nバーからの距離dot.Bass ) : ( y_base[ 0 ] + pChip.nバーからの距離dot.Bass );
					int numB = y - offset;				// 4種全て同じ定義
					int numC = 0;						// 4種全て同じ初期値
					const int numD = 355;				// ドラム画面かギター画面かで変わる値
					if ( ( numB < ( numD + numA ) ) && ( numB > -numA ) )	// 以下のロジックは4種全て同じ
					{
						int c = this.ctWailingチップ模様アニメ.n現在の値;
						Rectangle rect = new Rectangle( baseTextureOffsetX + ( c * WailingWidth ), baseTextureOffsetY, WailingWidth, WailingHeight );
						if ( numB < numA )
						{
							rect.Y += numA - numB;
							rect.Height -= numA - numB;
							numC = numA - numB;
						}
						if ( numB > ( numD - numA ) )
						{
							rect.Height -= numB - ( numD - numA );
						}
						if ( ( rect.Bottom > rect.Top ) && ( this.txチップ != null ) )
						{
							this.txチップ.t2D描画( CDTXMania.app.Device, drawX, ( y - numA ) + numC, rect );
						}
					}
				}
				//    if ( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
				//    {
				//        if ( pChip.nバーからの距離dot.Bass < -234 )	// #25253 2011.5.29 yyagi: Don't set pChip.bHit=true for wailing at once. It need to 1sec-delay (234pix per 1sec).
				//        {
				//            pChip.bHit = true;
				//        }
				//        if ( configIni.bAutoPlay.Bass )
				//        {
				//            this.actWailingBonus.Start( E楽器パート.BASS, this.r現在の歓声Chip.Bass );
				//            pChip.bHit = true;						// #25253 2011.5.29 yyagi: Set pChip.bHit=true if autoplay.
				//        }
				//    }
				//    return;
				//}
				//pChip.bHit = true;
			}
				base.t進行描画・チップ・ベース・ウェイリング( configIni, ref dTX, ref pChip);
		}
		protected override void t進行描画・チップ・空打ち音設定・ドラム( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
			{
				pChip.bHit = true;
				this.r現在の空うちドラムChip[ (int) this.eチャンネルtoパッド[ pChip.nチャンネル番号 - 0xb1 ] ] = pChip;
				//pChip.nチャンネル番号 = ( pChip.nチャンネル番号 != 0xbc ) ? ( ( pChip.nチャンネル番号 - 0xb1 ) + 0x11 ) : 0x1a;
			}
		}
		protected override void t進行描画・チップ・小節線( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip )
		{
			int n小節番号plus1 = pChip.n発声位置 / 384;
			if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
			{
				pChip.bHit = true;
				this.actPlayInfo.n小節番号 = n小節番号plus1 - 1;
				if ( configIni.bWave再生位置自動調整機能有効 && ( bIsDirectSound || bUseOSTimer ) )
				{
					dTX.tWave再生位置自動補正();
				}
			}
			if ( configIni.bDrums有効 )
			{
				if ( configIni.b演奏情報を表示する && ( configIni.eDark == Eダークモード.OFF ) )
				{
					int n小節番号 = n小節番号plus1 - 1;
					CDTXMania.act文字コンソール.tPrint( 0x14d, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 0x11 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 0x11 ), C文字コンソール.Eフォント種別.白, n小節番号.ToString() );
				}
				if ( ( ( configIni.eDark != Eダークモード.FULL ) && pChip.b可視 ) && ( this.txチップ != null ) )
				{
					this.txチップ.n透明度 = 255;
					this.txチップ.t2D描画( CDTXMania.app.Device, 0x23, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 1 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 1 ), new Rectangle( 0, 0x1bc, 0x128, 2 ) );
				}
			}
			if ( ( pChip.b可視 && configIni.bGuitar有効 ) && ( configIni.eDark != Eダークモード.FULL ) && ( this.txチップ != null ) )
			{
				this.txチップ.n透明度 = 255;
				//int y = configIni.bReverse.Guitar ? ( ( 0x176 - pChip.nバーからの距離dot.Guitar ) - 1 ) : ( ( 0x5f + pChip.nバーからの距離dot.Guitar ) - 1 );
				int y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.GUITAR, false, configIni.bReverse.Guitar );
				if ( configIni.bReverse.Guitar )
				{
					y = y - pChip.nバーからの距離dot.Guitar - 1;
				}
				else
				{
					y = y + pChip.nバーからの距離dot.Guitar - 1;
				}
				int n小節線消失距離dot;
				// Reverse時の小節線消失位置を、RGBボタンの真ん中程度に。
				// 非Reverse時の消失処理は、従来通りt進行描画・チップ()にお任せ。
				n小節線消失距離dot = configIni.bReverse.Guitar ? -100 : ( configIni.e判定位置.Guitar == E判定位置.標準 ) ? -50 : -25;

				if ( ( dTX.bチップがある.Guitar && ( y > 0x39 ) ) && ( ( y < 0x19c ) ) &&
					( pChip.nバーからの距離dot.Guitar >= n小節線消失距離dot )
					)
				{
					this.txチップ.t2D描画( CDTXMania.app.Device, 0x1fb, y, new Rectangle( 0, 450, 0x4e, 1 ) );
				}
				//y = configIni.bReverse.Bass ? ( ( 0x176 - pChip.nバーからの距離dot.Bass ) - 1 ) : ( ( 0x5f + pChip.nバーからの距離dot.Bass ) - 1 );
				y = 演奏判定ライン座標.n判定ラインY座標( E楽器パート.BASS, false, configIni.bReverse.Bass );
				if ( configIni.bReverse.Bass )
				{
					y = y - pChip.nバーからの距離dot.Bass - 1;
				}
				else
				{
					y = y + pChip.nバーからの距離dot.Bass - 1;
				}
				n小節線消失距離dot = configIni.bReverse.Bass ? -100 : ( configIni.e判定位置.Bass == E判定位置.標準 ) ? -50 : -25;
				if ( ( dTX.bチップがある.Bass && ( y > 0x39 ) ) && ( ( y < 0x19c ) ) &&
					( pChip.nバーからの距離dot.Bass >= n小節線消失距離dot )
					)
				{
					this.txチップ.t2D描画( CDTXMania.app.Device, 0x18e, y, new Rectangle( 0, 450, 0x4e, 1 ) );
				}
			}
		}
		#endregion
	}
}
