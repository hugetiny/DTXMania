using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using SlimDX;
using System.Drawing.Text;
using FDK;

namespace DTXMania
{
	internal class CStage曲読み込み : CStage
	{
		// コンストラクタ

		public CStage曲読み込み()
		{
			base.eステージID = CStage.Eステージ.曲読み込み;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
//			base.list子Activities.Add( this.actFI = new CActFIFOBlack() );	// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
			base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
		}


		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation( "曲読み込みステージを活性化します。" );
			Trace.Indent();
			try
			{
				this.str曲タイトル = "";
				this.strSTAGEFILE = "";
				this.b音符を表示する = false;
				this.n音符の表示位置X = 0x308;
				this.ftタイトル表示用フォント = new Font( "MS PGothic", 48f, GraphicsUnit.Pixel );
				this.nBGM再生開始時刻 = -1;
				this.nBGMの総再生時間ms = 0;
				if( this.sd読み込み音 != null )
				{
					CDTXMania.Sound管理.tサウンドを破棄する( this.sd読み込み音 );
					this.sd読み込み音 = null;
				}

				string strDTXファイルパス = ( CDTXMania.bコンパクトモード ) ?
					CDTXMania.strコンパクトモードファイル : CDTXMania.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;
				
				CDTX cdtx = new CDTX( strDTXファイルパス, true );
				this.str曲タイトル = cdtx.TITLE;
				if( ( ( cdtx.STAGEFILE != null ) && ( cdtx.STAGEFILE.Length > 0 ) ) && ( File.Exists( cdtx.strフォルダ名 + cdtx.STAGEFILE ) && !CDTXMania.ConfigIni.bストイックモード ) )
				{
					this.strSTAGEFILE = cdtx.strフォルダ名 + cdtx.STAGEFILE;
					this.b音符を表示する = false;
				}
				else
				{
					this.strSTAGEFILE = CSkin.Path( @"Graphics\\ScreenNowLoading background.jpg" );
					this.b音符を表示する = true;
				}
				if( ( ( cdtx.SOUND_NOWLOADING != null ) && ( cdtx.SOUND_NOWLOADING.Length > 0 ) ) && File.Exists( cdtx.strフォルダ名 + cdtx.SOUND_NOWLOADING ) )
				{
					string strNowLoadingサウンドファイルパス = cdtx.strフォルダ名 + cdtx.SOUND_NOWLOADING;
					try
					{
						this.sd読み込み音 = CDTXMania.Sound管理.tサウンドを生成する( strNowLoadingサウンドファイルパス );
					}
					catch
					{
						Trace.TraceError( "#SOUND_NOWLOADING に指定されたサウンドファイルの読み込みに失敗しました。({0})", strNowLoadingサウンドファイルパス );
					}
				}
				cdtx.On非活性化();
				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation( "曲読み込みステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "曲読み込みステージを非活性化します。" );
			Trace.Indent();
			try
			{
				if( this.ftタイトル表示用フォント != null )
				{
					this.ftタイトル表示用フォント.Dispose();
					this.ftタイトル表示用フォント = null;
				}
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation( "曲読み込みステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx音符 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\\ScreenNowLoading onpu.png" ), false );
				this.tx背景 = CDTXMania.tテクスチャの生成( this.strSTAGEFILE, false );
				if( this.b音符を表示する )
				{
					try
					{
						if( ( this.str曲タイトル != null ) && ( this.str曲タイトル.Length > 0 ) )
						{
							Bitmap image = new Bitmap( 1, 1 );
							Graphics graphics = Graphics.FromImage( image );
							SizeF ef = graphics.MeasureString( this.str曲タイトル, this.ftタイトル表示用フォント );
							Size size = new Size( (int) Math.Ceiling( (double) ef.Width ), (int) Math.Ceiling( (double) ef.Height ) );
							graphics.Dispose();
							image.Dispose();
							image = new Bitmap( size.Width, size.Height );
							graphics = Graphics.FromImage( image );
							graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
							graphics.DrawString( this.str曲タイトル, this.ftタイトル表示用フォント, Brushes.White, ( float ) 0f, ( float ) 0f );
							graphics.Dispose();
							this.txタイトル = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
							this.txタイトル.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );
							image.Dispose();
							this.n音符の表示位置X = ( ( 640 - ( (int) ( size.Width * this.txタイトル.vc拡大縮小倍率.X ) ) ) - ( ( this.tx音符 != null ) ? this.tx音符.sz画像サイズ.Width : 0 ) ) - 2;
						}
						else
						{
							this.txタイトル = null;
							this.n音符の表示位置X = ( 640 - ( ( this.tx音符 != null ) ? this.tx音符.sz画像サイズ.Width : 0 ) ) - 2;
						}
					}
					catch( CTextureCreateFailedException )
					{
						Trace.TraceError( "テクスチャの生成に失敗しました。({0})", new object[] { this.strSTAGEFILE } );
						this.txタイトル = null;
						this.tx背景 = null;
					}
				}
				else
				{
					this.txタイトル = null;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.tx音符 );
				CDTXMania.tテクスチャの解放( ref this.txタイトル );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			string str;

			if( base.b活性化してない )
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------------------
			if( base.b初めての進行描画 )
			{
				Cスコア cスコア1 = CDTXMania.stage選曲.r確定されたスコア;
				if( this.sd読み込み音 != null )
				{
					if( CDTXMania.Skin.sound曲読込開始音.b排他 && ( CSkin.Cシステムサウンド.r最後に再生した排他システムサウンド != null ) )
					{
						CSkin.Cシステムサウンド.r最後に再生した排他システムサウンド.t停止する();
					}
					this.sd読み込み音.t再生を開始する();
					this.nBGM再生開始時刻 = CSound管理.rc演奏用タイマ.n現在時刻;
					this.nBGMの総再生時間ms = this.sd読み込み音.n総演奏時間ms;
				}
				else
				{
					CDTXMania.Skin.sound曲読込開始音.t再生する();
					this.nBGM再生開始時刻 = CSound管理.rc演奏用タイマ.n現在時刻;
					this.nBGMの総再生時間ms = CDTXMania.Skin.sound曲読込開始音.n長さ・現在のサウンド;
				}
//				this.actFI.tフェードイン開始();							// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
				base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
				base.b初めての進行描画 = false;

				nWAVcount = 1;
				bitmapFilename = new Bitmap( 640, 24 );
				graphicsFilename = Graphics.FromImage( bitmapFilename );
				graphicsFilename.TextRenderingHint = TextRenderingHint.AntiAlias;
				ftFilename = new Font( "MS PGothic", 24f, FontStyle.Bold, GraphicsUnit.Pixel );
			}
			//-----------------------------
			#endregion

			#region [ ESC押下時は選曲画面に戻る ]
			if ( tキー入力() )
			{
				if ( this.sd読み込み音 != null )
				{
					this.sd読み込み音.tサウンドを停止する();
					this.sd読み込み音.t解放する();
				}
				return (int) E曲読込画面の戻り値.読込中止;
			}
			#endregion

			#region [ 背景、音符＋タイトル表示 ]
			//-----------------------------
			if( this.tx背景 != null )
				this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );

			if( this.b音符を表示する )
			{
				int y = 480 - 45;
				if( this.tx音符 != null )
				{
					this.tx音符.t2D描画( CDTXMania.app.Device, this.n音符の表示位置X, y );
				}
				if( this.txタイトル != null )
				{
					this.txタイトル.t2D描画( CDTXMania.app.Device, (int) ( 640 - ( this.txタイトル.sz画像サイズ.Width * this.txタイトル.vc拡大縮小倍率.X ) ), y );
				}
			}
			//-----------------------------
			#endregion

			switch( base.eフェーズID )
			{
				case CStage.Eフェーズ.共通_フェードイン:
//					if( this.actFI.On進行描画() != 0 )					// #27787 2012.3.10 yyagi 曲読み込み画面のフェードインの省略
																		// 必ず一度「CStaeg.Eフェーズ.共通_フェードイン」フェーズを経由させること。
																		// さもないと、曲読み込みが完了するまで、曲読み込み画面が描画されない。
						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む;
					return (int) E曲読込画面の戻り値.継続;

				case CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む:
					{
						timeBeginLoad = DateTime.Now;
						TimeSpan span;
						str = null;
						if( !CDTXMania.bコンパクトモード )
							str = CDTXMania.stage選曲.r確定されたスコア.ファイル情報.ファイルの絶対パス;
						else
							str = CDTXMania.strコンパクトモードファイル;

						CScoreIni ini = new CScoreIni( str + ".score.ini" );
						ini.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();

						if( ( CDTXMania.DTX != null ) && CDTXMania.DTX.b活性化してる )
							CDTXMania.DTX.On非活性化();

						CDTXMania.DTX = new CDTX( str, false, ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0, ini.stファイル.BGMAdjust );
						Trace.TraceInformation( "----曲情報-----------------" );
						Trace.TraceInformation( "TITLE: {0}", CDTXMania.DTX.TITLE );
						Trace.TraceInformation( "FILE: {0}",  CDTXMania.DTX.strファイル名の絶対パス );
						Trace.TraceInformation( "---------------------------" );

						span = (TimeSpan) ( DateTime.Now - timeBeginLoad );
						Trace.TraceInformation( "DTX読込所要時間:           {0}", span.ToString() );

						if ( CDTXMania.bコンパクトモード )
							CDTXMania.DTX.MIDIレベル = 1;
						else
							CDTXMania.DTX.MIDIレベル = ( CDTXMania.stage選曲.r確定された曲.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) ? CDTXMania.stage選曲.n現在選択中の曲の難易度 : 0;

						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む;
						timeBeginLoadWAV = DateTime.Now;
						return (int) E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む:
					{
						if ( nWAVcount == 1 && CDTXMania.DTX.listWAV.Count > 0 )			// #28934 2012.7.7 yyagi (added checking Count)
						{
							ShowProgressByFilename( CDTXMania.DTX.listWAV[ nWAVcount ].strファイル名 );
						}
						int looptime = (CDTXMania.ConfigIni.b垂直帰線待ちを行う)? 3 : 1;	// VSyncWait=ON時は1frame(1/60s)あたり3つ読むようにする
						for ( int i = 0; i < looptime && nWAVcount <= CDTXMania.DTX.listWAV.Count; i++ )
						{
							if ( CDTXMania.DTX.listWAV[ nWAVcount ].listこのWAVを使用するチャンネル番号の集合.Count > 0 )	// #28674 2012.5.8 yyagi
							{
								CDTXMania.DTX.tWAVの読み込み( CDTXMania.DTX.listWAV[ nWAVcount ] );
							}
							nWAVcount++;
						}
						if ( nWAVcount <= CDTXMania.DTX.listWAV.Count )
						{
							ShowProgressByFilename( CDTXMania.DTX.listWAV[ nWAVcount ].strファイル名 );
						}
						if ( nWAVcount > CDTXMania.DTX.listWAV.Count )
						{
							TimeSpan span = ( TimeSpan ) ( DateTime.Now - timeBeginLoadWAV );
							Trace.TraceInformation( "WAV読込所要時間({0,4}):     {1}", CDTXMania.DTX.listWAV.Count, span.ToString() );
							timeBeginLoadWAV = DateTime.Now;

							if ( CDTXMania.ConfigIni.bDynamicBassMixerManagement )
							{
								CDTXMania.DTX.PlanToAddMixerChannel();
							}
							CDTXMania.DTX.tギターとベースのランダム化( E楽器パート.GUITAR, CDTXMania.ConfigIni.eRandom.Guitar );
							CDTXMania.DTX.tギターとベースのランダム化( E楽器パート.BASS, CDTXMania.ConfigIni.eRandom.Bass );

							if ( CDTXMania.ConfigIni.bギタレボモード )
								CDTXMania.stage演奏ギター画面.On活性化();
							else
								CDTXMania.stage演奏ドラム画面.On活性化();

							span = (TimeSpan) ( DateTime.Now - timeBeginLoadWAV );
							Trace.TraceInformation( "WAV/譜面後処理時間({0,4}):  {1}", ( CDTXMania.DTX.listBMP.Count + CDTXMania.DTX.listBMPTEX.Count + CDTXMania.DTX.listAVI.Count ), span.ToString() );

							base.eフェーズID = CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む;
						}
						return (int) E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む:
					{
						TimeSpan span;
						DateTime timeBeginLoadBMPAVI = DateTime.Now;
						if ( CDTXMania.ConfigIni.bBGA有効 )
							CDTXMania.DTX.tBMP_BMPTEXの読み込み();

						if ( CDTXMania.ConfigIni.bAVI有効 )
							CDTXMania.DTX.tAVIの読み込み();
						span = ( TimeSpan ) ( DateTime.Now - timeBeginLoadBMPAVI );
						Trace.TraceInformation( "BMP/AVI読込所要時間({0,4}): {1}", ( CDTXMania.DTX.listBMP.Count + CDTXMania.DTX.listBMPTEX.Count + CDTXMania.DTX.listAVI.Count ), span.ToString() );

						span = ( TimeSpan ) ( DateTime.Now - timeBeginLoad );
						Trace.TraceInformation( "総読込時間:                {0}", span.ToString() );

						if ( bitmapFilename != null )
						{
							bitmapFilename.Dispose();
							bitmapFilename = null;
						}
						if ( graphicsFilename != null )
						{
							graphicsFilename.Dispose();
							graphicsFilename = null;
						}
						if ( ftFilename != null )
						{
							ftFilename.Dispose();
							ftFilename = null;
						}
						CDTXMania.Timer.t更新();
						base.eフェーズID = CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ;
						return (int) E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ:
					{
						long nCurrentTime = CDTXMania.Timer.n現在時刻;
						if( nCurrentTime < this.nBGM再生開始時刻 )
							this.nBGM再生開始時刻 = nCurrentTime;

//						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) > ( this.nBGMの総再生時間ms - 1000 ) )
						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) >= ( this.nBGMの総再生時間ms ) )	// #27787 2012.3.10 yyagi 1000ms == フェードイン分の時間
						{
							if ( !CDTXMania.DTXVmode.Enabled )
							{
								this.actFO.tフェードアウト開始();
							}
							base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						}
						return (int) E曲読込画面の戻り値.継続;
					}

				case CStage.Eフェーズ.共通_フェードアウト:
					if ( this.actFO.On進行描画() == 0 && !CDTXMania.DTXVmode.Enabled )		// DTXVモード時は、フェードアウト省略
						return 0;

					if ( txFilename != null )
					{
						txFilename.Dispose();
					}
					if ( this.sd読み込み音 != null )
					{
						this.sd読み込み音.t解放する();
					}
					return (int) E曲読込画面の戻り値.読込完了;
			}
			return (int) E曲読込画面の戻り値.継続;
		}

		/// <summary>
		/// ESC押下時、trueを返す
		/// </summary>
		/// <returns></returns>
		protected bool tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Input管理.Keyboard;
			if 	( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Escape ) )		// escape (exit)
			{
				return true;
			}
			return false;
		}


		private void ShowProgressByFilename(string strファイル名 )
		{
			if ( graphicsFilename != null && ftFilename != null )
			{
				graphicsFilename.Clear( Color.Transparent );
				graphicsFilename.DrawString( strファイル名, ftFilename, Brushes.White, new RectangleF( 0, 0, 640, 24 ) );
				if ( txFilename != null )
				{
					txFilename.Dispose();
				}
				txFilename = new CTexture( CDTXMania.app.Device, bitmapFilename, CDTXMania.TextureFormat );
				txFilename.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );
				txFilename.t2D描画( CDTXMania.app.Device, 0, 480 - 16 );
			}
		}

		// その他

		#region [ private ]
		//-----------------
//		private CActFIFOBlack actFI;
		private CActFIFOBlack actFO;
		private bool b音符を表示する;
		private Font ftタイトル表示用フォント;
		private long nBGMの総再生時間ms;
		private long nBGM再生開始時刻;
		private int n音符の表示位置X;
		private CSound sd読み込み音;
		private string strSTAGEFILE;
		private string str曲タイトル;
		private CTexture txタイトル;
		private CTexture tx音符;
		private CTexture tx背景;
		private DateTime timeBeginLoad;
		private DateTime timeBeginLoadWAV;
		private int nWAVcount;
		private CTexture txFilename;
		private Bitmap bitmapFilename;
		private Graphics graphicsFilename;
		private Font ftFilename;
		//-----------------
		#endregion
	}
}
