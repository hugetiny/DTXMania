using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	internal class CStage結果 : CStage
	{
		// プロパティ

		public STDGBVALUE<bool> b新記録スキル;
		public STDGBVALUE<bool> b新記録スコア;
        public STDGBVALUE<bool> b新記録ランク;
		public STDGBVALUE<float> fPerfect率;
		public STDGBVALUE<float> fGreat率;
		public STDGBVALUE<float> fGood率;
		public STDGBVALUE<float> fPoor率;
        public STDGBVALUE<float> fMiss率;
        public STDGBVALUE<bool> bオート;        // #23596 10.11.16 add ikanick
                                                //        10.11.17 change (int to bool) ikanick
		public STDGBVALUE<int> nランク値;
		public STDGBVALUE<int> n演奏回数;
		public int n総合ランク値;
		public CDTX.CChip[] r空うちドラムチップ;
		public STDGBVALUE<CScoreIni.C演奏記録> st演奏記録;


		// コンストラクタ

		public CStage結果()
		{
			STDGBVALUE<CScoreIni.C演奏記録> stdgbvalue = new STDGBVALUE<CScoreIni.C演奏記録>();
			stdgbvalue.Drums = new CScoreIni.C演奏記録();
			stdgbvalue.Guitar = new CScoreIni.C演奏記録();
			stdgbvalue.Bass = new CScoreIni.C演奏記録();
			this.st演奏記録 = stdgbvalue;
			this.r空うちドラムチップ = new CDTX.CChip[ 10 ];
			this.n総合ランク値 = -1;
			this.nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
			base.eステージID = CStage.Eステージ.結果;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actResultImage = new CActResultImage() );
			base.list子Activities.Add( this.actParameterPanel = new CActResultParameterPanel() );
			base.list子Activities.Add( this.actRank = new CActResultRank() );
			base.list子Activities.Add( this.actSongBar = new CActResultSongBar() );
			base.list子Activities.Add( this.actOption = new CActオプションパネル() );
			base.list子Activities.Add( this.actFI = new CActFIFOWhite() );
			base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
		}

		
		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation( "結果ステージを活性化します。" );
			Trace.Indent();
			try
			{
				#region [ 初期化 ]
				//---------------------
				this.eフェードアウト完了時の戻り値 = E戻り値.継続;
				this.bアニメが完了 = false;
				this.n最後に再生したHHのWAV番号 = -1;
				this.n最後に再生したHHのチャンネル番号 = 0;
				for( int i = 0; i < 3; i++ )
				{
					this.b新記録スキル[ i ] = false;
                    this.b新記録スコア[ i ] = false;
                    this.b新記録ランク[ i ] = false;
				}
				//---------------------
				#endregion

				#region [ 結果の計算 ]
				//---------------------
				for( int j = 0; j < 3; j++ )
				{
					this.nランク値[ j ] = -1;
					if( ( ( ( j != 0 ) || ( CDTXMania.DTX.bチップがある.Drums && !CDTXMania.ConfigIni.bギタレボモード ) ) && ( ( j != 1 ) || CDTXMania.DTX.bチップがある.Guitar ) ) && ( ( j != 2 ) || CDTXMania.DTX.bチップがある.Bass ) )
					{
						CScoreIni.C演奏記録 part = this.st演奏記録[ j ];
						bool guitar = true;
						switch( j )
						{
							case 0:
                                guitar = CDTXMania.ConfigIni.bドラムが全部オートプレイである;
								break;

							case 1:
								guitar = CDTXMania.ConfigIni.bAutoPlay.Guitar;
								break;

							case 2:
								guitar = CDTXMania.ConfigIni.bAutoPlay.Bass;
								break;
						}
						this.fPerfect率[ j ] = guitar ? 0f : ( ( 100f * part.nPerfect数 ) / ( (float) part.n全チップ数 ) );
						this.fGreat率[ j ] = guitar ? 0f : ( ( 100f * part.nGreat数 ) / ( (float) part.n全チップ数 ) );
						this.fGood率[ j ] = guitar ? 0f : ( ( 100f * part.nGood数 ) / ( (float) part.n全チップ数 ) );
						this.fPoor率[ j ] = guitar ? 0f : ( ( 100f * part.nPoor数 ) / ( (float) part.n全チップ数 ) );
						this.fMiss率[ j ] = guitar ? 0f : ( ( 100f * part.nMiss数 ) / ( (float) part.n全チップ数 ) );
                        this.bオート[ j ] = guitar ;    // #23596 10.11.16 add ikanick そのパートがオートなら1
                                                        //        10.11.17 change (int to bool) ikanick
						this.nランク値[ j ] = CScoreIni.tランク値を計算して返す( part );
					}
				}
				this.n総合ランク値 = CScoreIni.t総合ランク値を計算して返す( this.st演奏記録.Drums, this.st演奏記録.Guitar, this.st演奏記録.Bass );
				//---------------------
				#endregion

				#region [ .score.ini の作成と出力 ]
				//---------------------
				string str = CDTXMania.DTX.strファイル名の絶対パス + ".score.ini";
				CScoreIni ini = new CScoreIni( str );

				bool[] b今までにフルコンボしたことがある = new bool[] { false, false, false };

				for( int k = 0; k < 3; k++ )
				{
					// フルコンボチェックならびに新記録ランクチェックは、ini.Record[] が、スコアチェックや演奏型スキルチェックの IF 内で書き直されてしまうよりも前に行う。(2010.9.10)
					
					b今までにフルコンボしたことがある[ k ] = ini.stセクション[ k * 2 ].bフルコンボである | ini.stセクション[ k * 2 + 1 ].bフルコンボである;

					if( this.nランク値[ k ] <= CScoreIni.tランク値を計算して返す( ini.stセクション[ ( k * 2 ) + 1 ] ) )
					{
						this.b新記録ランク[ k ] = true;
					}

					// 新記録スコアチェック
					if( this.st演奏記録[ k ].nスコア > ini.stセクション[ k * 2 ].nスコア )
					{
						this.b新記録スコア[ k ] = true;
						ini.stセクション[ k * 2 ] = this.st演奏記録[ k ];
					}

                    // 新記録スキルチェック
                    if (this.st演奏記録[k].db演奏型スキル値 > ini.stセクション[(k * 2) + 1].db演奏型スキル値)
                    {
                        this.b新記録スキル[ k ] = true;
                        ini.stセクション[(k * 2) + 1] = this.st演奏記録[ k ];
                    }
                    // ラストプレイ #23595 2011.1.9 ikanick
                    // オートじゃなければプレイ結果を書き込む
                    if (this.bオート[ k ] == false) {
                        ini.stセクション[k + 6] = this.st演奏記録[ k ];
                    }

                    // #23596 10.11.16 add ikanick オートじゃないならクリア回数を1増やす
                    //        10.11.17 change (nオート to bオート)
                    //                 add default..throw           ikanick
                    if (this.bオート[ k ] == false)
                    {
                        switch ( k )
                        {
                            case 0:
                                ini.stファイル.ClearCountDrums++;
                                break;
                            case 1:
                                ini.stファイル.ClearCountGuitar++;
                                break;
                            case 2:
                                ini.stファイル.ClearCountBass++;
                                break;
                            default:
                                throw new Exception("クリア回数増加のk(0-2)が範囲外です。");
                        }
                    }
                    //---------------------------------------------------------------------/
				}
				ini.t書き出し( str );
				//---------------------
				#endregion

				#region [ 選曲画面の譜面情報の更新 ]
				//---------------------
				if( !CDTXMania.bコンパクトモード )
				{
					Cスコア cスコア = CDTXMania.stage選曲.r確定されたスコア;
					bool[] flagArray = new bool[ 3 ];
					CScoreIni.t更新条件を取得する( out flagArray[ 0 ], out flagArray[ 1 ], out flagArray[ 2 ] );
					for( int m = 0; m < 3; m++ )
					{
						if( flagArray[ m ] )
						{
							// FullCombo した記録を FullCombo なしで超えた場合、FullCombo マークが消えてしまう。
							// → FullCombo は、最新記録と関係なく、一度達成したらずっとつくようにする。(2010.9.11)
							cスコア.譜面情報.フルコンボ[ m ] = this.st演奏記録[ m ].bフルコンボである | b今までにフルコンボしたことがある[ m ];

							if( this.b新記録スキル[ m ] )
							{
								cスコア.譜面情報.最大スキル[ m ] = this.st演奏記録[ m ].db演奏型スキル値;
                            }

                            if (this.b新記録ランク[ m ])
                            {
                                cスコア.譜面情報.最大ランク[ m ] = this.nランク値[ m ];
                            }
						}
					}
				}
				//---------------------
				#endregion

				#region [ #RESULTSOUND_xx の再生（あれば）]
				//---------------------
				int rank = CScoreIni.t総合ランク値を計算して返す( this.st演奏記録.Drums, this.st演奏記録.Guitar, this.st演奏記録.Bass );

				if (rank == 99)	// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
				{
					rank = 6;
				}
	
				if( string.IsNullOrEmpty( CDTXMania.DTX.RESULTSOUND[ rank ] ) )
				{
					CDTXMania.Skin.soundステージクリア音.t再生する();
					this.rResultSound = null;
				}
				else
				{
					string str2 = CDTXMania.DTX.strフォルダ名 + CDTXMania.DTX.RESULTSOUND[ rank ];
					try
					{
						this.rResultSound = CDTXMania.Sound管理.tサウンドを生成する( str2 );
					}
					catch
					{
						Trace.TraceError( "サウンドの生成に失敗しました。({0})", new object[] { str2 } );
						this.rResultSound = null;
					}
				}
				//---------------------
				#endregion

				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation( "結果ステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			if( this.rResultSound != null )
			{
				CDTXMania.Sound管理.tサウンドを破棄する( this.rResultSound );
				this.rResultSound = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult background.jpg" ) );
				this.tx上部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult header panel.png" ), true );
				this.tx下部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult footer panel.png" ), true );
				this.txオプションパネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Screen option panels.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				if( this.ct登場用 != null )
				{
					this.ct登場用 = null;
				}
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.tx上部パネル );
				CDTXMania.tテクスチャの解放( ref this.tx下部パネル );
				CDTXMania.tテクスチャの解放( ref this.txオプションパネル );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				int num;
				if( base.b初めての進行描画 )
				{
					this.ct登場用 = new CCounter( 0, 100, 5, CDTXMania.Timer );
					this.actFI.tフェードイン開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					if( this.rResultSound != null )
					{
						this.rResultSound.t再生を開始する();
					}
					base.b初めての進行描画 = false;
				}
				this.bアニメが完了 = true;
				if( this.ct登場用.b進行中 )
				{
					this.ct登場用.t進行();
					if( this.ct登場用.b終了値に達した )
					{
						this.ct登場用.t停止();
					}
					else
					{
						this.bアニメが完了 = false;
					}
				}

				// 描画

				if( this.tx背景 != null )
				{
					this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );
				}
				if( this.ct登場用.b進行中 && ( this.tx上部パネル != null ) )
				{
					double num2 = ( (double) this.ct登場用.n現在の値 ) / 100.0;
					double num3 = Math.Sin( 1.5707963267948966 * num2 );
					num = ( (int) ( this.tx上部パネル.sz画像サイズ.Height * num3 ) ) - this.tx上部パネル.sz画像サイズ.Height;
				}
				else
				{
					num = 0;
				}
				if( this.tx上部パネル != null )
				{
					this.tx上部パネル.t2D描画( CDTXMania.app.Device, 0, num );
				}
				if( this.tx下部パネル != null )
				{
					this.tx下部パネル.t2D描画( CDTXMania.app.Device, 0, 480 - this.tx下部パネル.sz画像サイズ.Height );
				}
				this.actOption.On進行描画();
				if( this.actResultImage.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}
				if( this.actParameterPanel.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}
				if( this.actRank.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}
				if( this.actSongBar.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}
				if( base.eフェーズID == CStage.Eフェーズ.共通_フェードイン )
				{
					if( this.actFI.On進行描画() != 0 )
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
				}
				else if( ( base.eフェーズID == CStage.Eフェーズ.共通_フェードアウト ) && ( this.actFO.On進行描画() != 0 ) )
				{
					return (int) this.eフェードアウト完了時の戻り値;
				}

				// キー入力

				if( CDTXMania.act現在入力を占有中のプラグイン == null )
				{
					if( CDTXMania.ConfigIni.bドラム打音を発声する && CDTXMania.ConfigIni.bDrums有効 )
					{
						for( int i = 0; i < 10; i++ )
						{
							List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.DRUMS, (Eパッド) i );
							if( ( events != null ) && ( events.Count > 0 ) )
							{
								foreach( STInputEvent event2 in events )
								{
									if( !event2.b押された )
									{
										continue;
									}
									CDTX.CChip rChip = this.r空うちドラムチップ[ i ];
									if( rChip == null )
									{
										switch( ( (Eパッド) i ) )
										{
											case Eパッド.HH:
												rChip = this.r空うちドラムチップ[ 7 ];
												if( rChip == null )
												{
													rChip = this.r空うちドラムチップ[ 9 ];
												}
												break;

											case Eパッド.FT:
												rChip = this.r空うちドラムチップ[ 4 ];
												break;

											case Eパッド.CY:
												rChip = this.r空うちドラムチップ[ 8 ];
												break;

											case Eパッド.HHO:
												rChip = this.r空うちドラムチップ[ 0 ];
												if( rChip == null )
												{
													rChip = this.r空うちドラムチップ[ 9 ];
												}
												break;

											case Eパッド.RD:
												rChip = this.r空うちドラムチップ[ 6 ];
												break;

											case Eパッド.LC:
												rChip = this.r空うちドラムチップ[ 0 ];
												if( rChip == null )
												{
													rChip = this.r空うちドラムチップ[ 7 ];
												}
												break;
										}
									}
									if( ( ( rChip != null ) && ( rChip.nチャンネル番号 >= 0x11 ) ) && ( rChip.nチャンネル番号 <= 0x1a ) )
									{
										int nLane = this.nチャンネル0Atoレーン07[ rChip.nチャンネル番号 - 0x11 ];
										if( ( nLane == 1 ) && ( ( rChip.nチャンネル番号 == 0x11 ) || ( ( rChip.nチャンネル番号 == 0x18 ) && ( this.n最後に再生したHHのチャンネル番号 != 0x18 ) ) ) )
										{
											CDTXMania.DTX.tWavの再生停止( this.n最後に再生したHHのWAV番号 );
											this.n最後に再生したHHのWAV番号 = rChip.n整数値・内部番号;
											this.n最後に再生したHHのチャンネル番号 = rChip.nチャンネル番号;
										}
										CDTXMania.DTX.tチップの再生( rChip, CDTXMania.Timer.nシステム時刻, nLane, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
									}
								}
							}
						}
					}
					if( ( ( CDTXMania.Pad.b押されたDGB( Eパッド.CY ) || CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.RD ) ) || ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LC ) || CDTXMania.Input管理.Keyboard.bキーが押された( 0x75 ) ) ) && !this.bアニメが完了 )
					{
						this.actResultImage.tアニメを完了させる();
						this.actParameterPanel.tアニメを完了させる();
						this.actRank.tアニメを完了させる();
						this.actSongBar.tアニメを完了させる();
						this.ct登場用.t停止();
					}
					if( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 )
					{
						if( CDTXMania.Input管理.Keyboard.bキーが押された( 0x35 ) )
						{
							CDTXMania.Skin.sound取消音.t再生する();
							this.actFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							this.eフェードアウト完了時の戻り値 = E戻り値.完了;
						}
						if( ( ( CDTXMania.Pad.b押されたDGB( Eパッド.CY ) || CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.RD ) ) || ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LC ) || CDTXMania.Input管理.Keyboard.bキーが押された( 0x75 ) ) ) && this.bアニメが完了 )
						{
							CDTXMania.Skin.sound取消音.t再生する();
							this.actFO.tフェードアウト開始();
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							this.eフェードアウト完了時の戻り値 = E戻り値.完了;
						}
					}
				}
			}
			return 0;
		}
		public enum E戻り値 : int
		{
			継続,
			完了
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ct登場用;
		private E戻り値 eフェードアウト完了時の戻り値;
		private CActFIFOWhite actFI;
		private CActFIFOBlack actFO;
		private CActオプションパネル actOption;
		private CActResultParameterPanel actParameterPanel;
		private CActResultRank actRank;
		private CActResultImage actResultImage;
		private CActResultSongBar actSongBar;
		private bool bアニメが完了;
		private readonly int[] nチャンネル0Atoレーン07;
		private int n最後に再生したHHのWAV番号;
		private int n最後に再生したHHのチャンネル番号;
		private CSound rResultSound;
		private CTexture txオプションパネル;
		private CTexture tx下部パネル;
		private CTexture tx上部パネル;
		private CTexture tx背景;
		//-----------------
		#endregion
	}
}
