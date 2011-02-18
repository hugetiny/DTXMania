using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// 演奏画面の共通クラス (ドラム演奏画面, ギター演奏画面の継承元)
	/// </summary>
	internal abstract class CStage演奏画面共通 : CStage
	{
		// プロパティ

		public bool bAUTOでないチップが１つでもバーを通過した
		{
			get;
			protected set;
		}


		// CStage 実装

		public override void On活性化()
		{
			this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.継続;
			this.n現在のトップChip = ( CDTXMania.DTX.listChip.Count > 0 ) ? 0 : -1;
			
			this.r次にくるギターChip = null;
			this.r次にくるベースChip = null;

			for ( int k = 0; k < 3; k++ )
			{
				for ( int n = 0; n < 5; n++ )
				{
					this.nヒット数・Auto含まない[ k ] = new STHITCOUNTOFRANK();
					this.nヒット数・Auto含む[ k ] = new STHITCOUNTOFRANK();
				}
				this.queWailing[ k ] = new Queue<CDTX.CChip>();
				this.r現在の歓声Chip[ k ] = null;
			}
			for ( int i = 0; i < 3; i++ )
			{
				this.b演奏にキーボードを使った[ i ] = false;
				this.b演奏にジョイパッドを使った[ i ] = false;
				this.b演奏にMIDI入力を使った[ i ] = false;
				this.b演奏にマウスを使った[ i ] = false;
			}
			this.bAUTOでないチップが１つでもバーを通過した = false;
			base.On活性化();
			this.tステータスパネルの選択();
			this.tパネル文字列の設定();

			this.nInputAdjustTimeMs.Drums = CDTXMania.ConfigIni.nInputAdjustTimeMs.Drums;		// #23580 2011.1.3 yyagi
			this.nInputAdjustTimeMs.Guitar = CDTXMania.ConfigIni.nInputAdjustTimeMs.Guitar;		//        2011.1.7 ikanick 修正
			this.nInputAdjustTimeMs.Bass = CDTXMania.ConfigIni.nInputAdjustTimeMs.Bass;			//
			//this.bIsAutoPlay.Drums = false;
			//this.bIsAutoPlay.Guitar = CDTXMania.ConfigIni.bAutoPlay.Guitar;
			//this.bIsAutoPlay.Bass = CDTXMania.ConfigIni.bAutoPlay.Bass;
			this.bIsAutoPlay = CDTXMania.ConfigIni.bAutoPlay;									// #24239 2011.1.23 yyagi
		}
		public override void On非活性化()
		{
			for ( int i = 0; i < 3; i++ )
			{
				this.queWailing[ i ] = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if ( !base.b活性化してない )
			{
				this.t背景テクスチャの生成();

				this.txWailing枠 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay wailing cursor.png" ) );

				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if ( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.txWailing枠 );
				base.OnManagedリソースの解放();
			}
		}

		// その他

		#region [ protected ]
		//-----------------
		protected class STHITCOUNTOFRANK
		{
			// Fields
			public int Good;
			public int Great;
			public int Miss;
			public int Perfect;
			public int Poor;

			// Properties
			public int this[ int index ]
			{
				get
				{
					switch ( index )
					{
						case 0:
							return this.Perfect;

						case 1:
							return this.Great;

						case 2:
							return this.Good;

						case 3:
							return this.Poor;

						case 4:
							return this.Miss;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch ( index )
					{
						case 0:
							this.Perfect = value;
							return;

						case 1:
							this.Great = value;
							return;

						case 2:
							this.Good = value;
							return;

						case 3:
							this.Poor = value;
							return;

						case 4:
							this.Miss = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		protected struct STKARAUCHI
		{
			public CDTX.CChip HH;
			public CDTX.CChip SD;
			public CDTX.CChip BD;
			public CDTX.CChip HT;
			public CDTX.CChip LT;
			public CDTX.CChip FT;
			public CDTX.CChip CY;
			public CDTX.CChip HHO;
			public CDTX.CChip RD;
			public CDTX.CChip LC;
			public CDTX.CChip this[ int index ]
			{
				get
				{
					switch ( index )
					{
						case 0:
							return this.HH;

						case 1:
							return this.SD;

						case 2:
							return this.BD;

						case 3:
							return this.HT;

						case 4:
							return this.LT;

						case 5:
							return this.FT;

						case 6:
							return this.CY;

						case 7:
							return this.HHO;

						case 8:
							return this.RD;

						case 9:
							return this.LC;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch ( index )
					{
						case 0:
							this.HH = value;
							return;

						case 1:
							this.SD = value;
							return;

						case 2:
							this.BD = value;
							return;

						case 3:
							this.HT = value;
							return;

						case 4:
							this.LT = value;
							return;

						case 5:
							this.FT = value;
							return;

						case 6:
							this.CY = value;
							return;

						case 7:
							this.HHO = value;
							return;

						case 8:
							this.RD = value;
							return;

						case 9:
							this.LC = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		protected CAct演奏AVI actAVI;
		protected CAct演奏BGA actBGA;
		protected CActFIFOBlack actFI;
		protected CActFIFOBlack actFO;
		protected CActFIFOWhite actFOClear;
		protected CAct演奏ゲージ共通 actGauge;

		protected CAct演奏判定文字列共通 actJudgeString;

		protected CAct演奏パネル文字列 actPanel;
		protected CAct演奏演奏情報 actPlayInfo;

		protected CAct演奏スコア共通 actScore;
		protected CAct演奏ステージ失敗 actStageFailed;
		protected CAct演奏ステータスパネル共通 actStatusPanels;
		protected CAct演奏WailingBonus共通 actWailingBonus;
		protected CAct演奏スクロール速度 act譜面スクロール速度;
		protected bool bPAUSE;
		protected STDGBVALUE<bool> b演奏にMIDI入力を使った;
		protected STDGBVALUE<bool> b演奏にキーボードを使った;
		protected STDGBVALUE<bool> b演奏にジョイパッドを使った;
		protected STDGBVALUE<bool> b演奏にマウスを使った;
		protected CCounter ctWailingチップ模様アニメ;
		protected STDGBVALUE<CCounter> ctチップ模様アニメ;

		protected E演奏画面の戻り値 eフェードアウト完了時の戻り値;
		protected readonly int[,] nBGAスコープチャンネルマップ = new int[ , ] { { 0xc4, 0xc7, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xe0 }, { 4, 7, 0x55, 0x56, 0x57, 0x58, 0x59, 0x60 } };
		protected readonly int[] nチャンネル0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 8, 0 };
		protected readonly int[] nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
		protected readonly int[] nパッド0Atoチャンネル0A = new int[] { 0x11, 0x12, 0x13, 20, 0x15, 0x17, 0x16, 0x18, 0x19, 0x1a };
		protected readonly int[] nパッド0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 8, 0 };
		protected readonly int[] nパッド0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 7, 0 };
		protected STDGBVALUE<STHITCOUNTOFRANK> nヒット数・Auto含まない;
		protected STDGBVALUE<STHITCOUNTOFRANK> nヒット数・Auto含む;
		protected int n現在のトップChip = -1;

		protected STDGBVALUE<Queue<CDTX.CChip>> queWailing;
		protected STDGBVALUE<CDTX.CChip> r現在の歓声Chip;

		protected CDTX.CChip r次にくるギターChip;
		protected CDTX.CChip r次にくるベースChip;
		protected CTexture txWailing枠;

		protected CTexture tx背景;
		protected STDGBVALUE<int> nInputAdjustTimeMs;		// #23580 2011.1.3 yyagi
		protected CConfigIni.STAUTOPLAY bIsAutoPlay;		// #24239 2011.1.23 yyagi




		protected E判定 e指定時刻からChipのJUDGEを返す( long nTime, CDTX.CChip pChip, int nInputAdjustTime )
		{
			if ( pChip != null )
			{
				pChip.nLag = (int) ( nTime + nInputAdjustTime - pChip.n発声時刻ms );		// #23580 2011.1.3 yyagi: add "nInputAdjustTime" to add input timing adjust feature
				int nDeltaTime = Math.Abs( pChip.nLag );
				//Debug.WriteLine("nAbsTime=" + (nTime - pChip.n発声時刻ms) + ", nDeltaTime=" + (nTime + nInputAdjustTime - pChip.n発声時刻ms));
				if ( nDeltaTime <= CDTXMania.nPerfect範囲ms )
				{
					return E判定.Perfect;
				}
				if ( nDeltaTime <= CDTXMania.nGreat範囲ms )
				{
					return E判定.Great;
				}
				if ( nDeltaTime <= CDTXMania.nGood範囲ms )
				{
					return E判定.Good;
				}
				if ( nDeltaTime <= CDTXMania.nPoor範囲ms )
				{
					return E判定.Poor;
				}
			}
			return E判定.Miss;
		}



		protected CDTX.CChip r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( long nTime, int nChannelFlag, int nInputAdjustTime )
		{
			nTime += nInputAdjustTime;						// #24239 2011.1.23 yyagi InputAdjust

			int num5;
			if ( this.n現在のトップChip == -1 )
			{
				return null;
			}
			int count = CDTXMania.DTX.listChip.Count;
			int num4 = num5 = this.n現在のトップChip;
			if ( this.n現在のトップChip >= count )
			{
				num4 = num5 = count - 1;
			}
			int num2 = num4;
			while ( num2 < count )
			{
				CDTX.CChip chip = CDTXMania.DTX.listChip[ num2 ];
				if ( ( ( nChannelFlag >= 0x11 ) && ( nChannelFlag <= 0x1a ) ) && ( ( chip.nチャンネル番号 == nChannelFlag ) || ( chip.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) ) )
				{
					if ( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if ( ( ( nChannelFlag == 0x2f ) && ( chip.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 0x28 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) )
				{
					if ( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if ( ( ( nChannelFlag == 0xaf ) && ( chip.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 0xa0 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) )
				{
					if ( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				num2++;
			}
			int num3 = num5;
			while ( num3 >= 0 )
			{
				CDTX.CChip chip2 = CDTXMania.DTX.listChip[ num3 ];
				if ( ( nChannelFlag >= 0x11 ) && ( nChannelFlag <= 0x1a ) )
				{
					if ( ( chip2.nチャンネル番号 == nChannelFlag ) || ( chip2.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) )
					{
						break;
					}
				}
				else if ( ( ( nChannelFlag == 0x2f ) && ( chip2.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 0x28 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) )
				{
					if ( ( chip2.nチャンネル番号 >= 0x20 ) && ( chip2.nチャンネル番号 <= 0x28 ) )
					{
						break;
					}
				}
				else if ( ( ( ( nChannelFlag == 0xaf ) && ( chip2.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 0xa0 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) && ( ( chip2.nチャンネル番号 >= 0xa0 ) && ( chip2.nチャンネル番号 <= 0xa8 ) ) )
				{
					break;
				}
				num3--;
			}
			if ( ( num2 == count ) && ( num3 < 0 ) )
			{
				return null;
			}
			if ( num2 == count )
			{
				return CDTXMania.DTX.listChip[ num3 ];
			}
			if ( num3 < 0 )
			{
				return CDTXMania.DTX.listChip[ num2 ];
			}
			CDTX.CChip chip3 = CDTXMania.DTX.listChip[ num2 ];
			CDTX.CChip chip4 = CDTXMania.DTX.listChip[ num3 ];
			int num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
			int num7 = Math.Abs( (int) ( nTime - chip4.n発声時刻ms ) );
			if ( num6 >= num7 )
			{
				return chip4;
			}
			return chip3;
		}

		protected void tステータスパネルの選択()
		{
			if ( CDTXMania.bコンパクトモード )
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する( null );
			}
			else if ( CDTXMania.stage選曲.r確定された曲 != null )
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する( CDTXMania.stage選曲.r確定された曲.ar難易度ラベル[ CDTXMania.stage選曲.n確定された曲の難易度 ] );
			}
		}

		protected CDTX.CChip r指定時刻に一番近い未ヒットChip( long nTime, int nChannelFlag, int nInputAdjustTime, int n検索範囲時間ms )
		{
			nTime += nInputAdjustTime;

			int num5;
			int num6;
			if ( this.n現在のトップChip == -1 )
			{
				return null;
			}
			int count = CDTXMania.DTX.listChip.Count;
			int num4 = num5 = this.n現在のトップChip;
			if ( this.n現在のトップChip >= count )
			{
				num4 = num5 = count - 1;
			}
			int num2 = num4;
			while ( num2 < count )
			{
				CDTX.CChip chip = CDTXMania.DTX.listChip[ num2 ];
				if ( ( !chip.bHit && ( nChannelFlag >= 0x11 ) ) && ( nChannelFlag <= 0x1a ) )
				{
					if ( ( chip.nチャンネル番号 == nChannelFlag ) || ( chip.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) )
					{
						if ( chip.n発声時刻ms > nTime )
						{
							break;
						}
						num5 = num2;
					}
				}
				else if ( !chip.bHit && ( ( ( nChannelFlag == 0x2f ) && ( chip.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) ) )
				{
					if ( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if ( !chip.bHit && ( ( ( nChannelFlag == 0xaf ) && ( chip.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) ) )
				{
					if ( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				num2++;
			}
			int num3 = num5;
			while ( num3 >= 0 )
			{
				CDTX.CChip chip2 = CDTXMania.DTX.listChip[ num3 ];
				if ( ( ( ( !chip2.bHit && ( nChannelFlag >= 0x11 ) ) && ( ( nChannelFlag <= 0x1a ) && ( ( chip2.nチャンネル番号 == nChannelFlag ) || ( chip2.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) ) ) ) || ( !chip2.bHit && ( ( ( nChannelFlag == 0x2f ) && ( chip2.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) ) ) || ( !chip2.bHit && ( ( ( nChannelFlag == 0xaf ) && ( chip2.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) ) )
				{
					break;
				}
				num3--;
			}
			if ( ( num2 == count ) && ( num3 < 0 ) )
			{
				return null;
			}
			CDTX.CChip chip3 = null;
			if ( num2 == count )
			{
				chip3 = CDTXMania.DTX.listChip[ num3 ];
				num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
			}
			else if ( num3 < 0 )
			{
				chip3 = CDTXMania.DTX.listChip[ num2 ];
				num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
			}
			else
			{
				int num7 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num2 ].n発声時刻ms ) );
				int num8 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num3 ].n発声時刻ms ) );
				if ( num7 < num8 )
				{
					chip3 = CDTXMania.DTX.listChip[ num2 ];
					num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
				}
				else
				{
					chip3 = CDTXMania.DTX.listChip[ num3 ];
					num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
				}
			}
			if ( ( n検索範囲時間ms > 0 ) && ( num6 > n検索範囲時間ms ) )
			{
				return null;
			}
			return chip3;
		}
		protected CDTX.CChip r次にくるギターChipを更新して返す()
		{
			int nInputAdjustTime = this.bIsAutoPlay.Guitar ? 0 : this.nInputAdjustTimeMs.Guitar;
			this.r次にくるギターChip = this.r指定時刻に一番近い未ヒットChip( CDTXMania.Timer.n現在時刻, 0x2f, nInputAdjustTime, 500 );
			return this.r次にくるギターChip;
		}
		protected CDTX.CChip r次にくるベースChipを更新して返す()
		{
			int nInputAdjustTime = this.bIsAutoPlay.Bass ? 0 : this.nInputAdjustTimeMs.Bass;
			this.r次にくるベースChip = this.r指定時刻に一番近い未ヒットChip( CDTXMania.Timer.n現在時刻, 0xaf, nInputAdjustTime, 500 );
			return this.r次にくるベースChip;
		}

		protected void ChangeInputAdjustTimeInPlaying( IInputDevice keyboard, int plusminus )		// #23580 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
		{
			int part, offset = plusminus;
			if ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftShift ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightShift ) )	// Guitar InputAdjustTime
			{
				part = (int) E楽器パート.GUITAR;
			}
			else if ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftAlt ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightAlt ) )	// Bass InputAdjustTime
			{
				part = (int) E楽器パート.BASS;
			}
			else	// Drums InputAdjustTime
			{
				part = (int) E楽器パート.DRUMS;
			}
			if ( !keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftControl ) && !keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightControl ) )
			{
				offset *= 10;
			}

			this.nInputAdjustTimeMs[ part ] += offset;
			if ( this.nInputAdjustTimeMs[ part ] > 0 )
			{
				this.nInputAdjustTimeMs[ part ] = 0;
			}
			else if ( this.nInputAdjustTimeMs[ part ] < -99 )
			{
				this.nInputAdjustTimeMs[ part ] = -99;
			}
			CDTXMania.ConfigIni.nInputAdjustTimeMs[ part ] = this.nInputAdjustTimeMs[ part ];
		}

		protected abstract void t入力処理・ドラム();
		protected abstract void t入力処理・ギター();
		protected abstract void t入力処理・ベース();
		protected abstract void ドラムスクロール速度アップ();
		protected abstract void ドラムスクロール速度ダウン();
		protected void tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Input管理.Keyboard;
			if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F1 ) &&
				( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightShift ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftShift ) ) )
			{	// shift+f1 (pause)
				this.bPAUSE = !this.bPAUSE;
				if ( this.bPAUSE )
				{
					CDTXMania.Timer.t一時停止();
					CDTXMania.DTX.t全チップの再生一時停止();
				}
				else
				{
					CDTXMania.Timer.t再開();
					CDTXMania.DTX.t全チップの再生再開();
				}
			}
			if ( ( !this.bPAUSE && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.t入力処理・ドラム();
				this.t入力処理・ギター();
				this.t入力処理・ベース();
				if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.UpArrow ) && ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightShift ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftShift ) ) )
				{	// shift (+ctrl) + UpArrow (BGMAdjust)
					CDTXMania.DTX.t各自動再生音チップの再生時刻を変更する( ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftControl ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightControl ) ) ? 1 : 10 );
					CDTXMania.DTX.tWave再生位置自動補正();
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.DownArrow ) && ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightShift ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftShift ) ) )
				{	// shift + DownArrow (BGMAdjust)
					CDTXMania.DTX.t各自動再生音チップの再生時刻を変更する( ( keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftControl ) || keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightControl ) ) ? -1 : -10 );
					CDTXMania.DTX.tWave再生位置自動補正();
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.UpArrow ) )
				{	// UpArrow(scrollspeed up)
					ドラムスクロール速度アップ();
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.DownArrow ) )
				{	// DownArrow (scrollspeed down)
					ドラムスクロール速度ダウン();
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Delete ) )
				{	// del (debug info)
					CDTXMania.ConfigIni.b演奏情報を表示する = !CDTXMania.ConfigIni.b演奏情報を表示する;
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.LeftArrow ) )		// #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying( keyboard, -1 );
				}
				else if ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.RightArrow ) )		// #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying( keyboard, +1 );
				}
				else if ( ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) && ( keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Escape ) || CDTXMania.Pad.b押されたGB( Eパッド.FT ) ) )
				{	// escape (exit)
					this.actFO.tフェードアウト開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
					this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.演奏中断;
				}
			}
		}

		protected void t入力メソッド記憶( E楽器パート part )
		{
			if ( CDTXMania.Pad.st検知したデバイス.Keyboard )
			{
				this.b演奏にキーボードを使った[ (int) part ] = true;
			}
			if ( CDTXMania.Pad.st検知したデバイス.Joypad )
			{
				this.b演奏にジョイパッドを使った[ (int) part ] = true;
			}
			if ( CDTXMania.Pad.st検知したデバイス.MIDIIN )
			{
				this.b演奏にMIDI入力を使った[ (int) part ] = true;
			}
			if ( CDTXMania.Pad.st検知したデバイス.Mouse )
			{
				this.b演奏にマウスを使った[ (int) part ] = true;
			}
		}


		protected abstract void t進行描画・AVI();
		protected void t進行描画・AVI(int x, int y)
		{
			if ( ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( !CDTXMania.ConfigIni.bストイックモード && CDTXMania.ConfigIni.bAVI有効 ) )
			{
				this.actAVI.t進行描画( x, y );
			}
		}
		protected abstract void t進行描画・BGA();
		protected void t進行描画・BGA(int x, int y)
		{
			if ( ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( !CDTXMania.ConfigIni.bストイックモード && CDTXMania.ConfigIni.bBGA有効 ) )
			{
				this.actBGA.t進行描画( x, y );
			}
		}

		protected void t進行描画・MIDIBGM()
		{
			if ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED )
			{
				CStage.Eフェーズ eフェーズid1 = base.eフェーズID;
			}
		}

		protected void t進行描画・STAGEFAILED()
		{
			if ( ( ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED ) || ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( ( this.actStageFailed.On進行描画() != 0 ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージ失敗;
				base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト;
				this.actFO.tフェードアウト開始();
			}
		}
		protected void t進行描画・WailingBonus()
		{
			if ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.actWailingBonus.On進行描画();
			}
		}
		protected abstract void t進行描画・Wailing枠();
		protected void t進行描画・Wailing枠(int GtWailingFrameX, int BsWailingFrameX, int GtWailingFrameY, int BsWailingFrameY)
		{
			if ( ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if ( CDTXMania.DTX.bチップがある.Guitar && ( this.txWailing枠 != null ) )
				{
					this.txWailing枠.t2D描画( CDTXMania.app.Device, GtWailingFrameX, GtWailingFrameY );
				}
				if ( CDTXMania.DTX.bチップがある.Bass && ( this.txWailing枠 != null ) )
				{
					this.txWailing枠.t2D描画( CDTXMania.app.Device, BsWailingFrameX, BsWailingFrameY );
				}
			}
		}


		protected abstract void t進行描画・パネル文字列();
		protected void t進行描画・パネル文字列(int x, int y)
		{
			if ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.actPanel.t進行描画( x, y );
			}
		}
		protected void tパネル文字列の設定()
		{
			this.actPanel.SetPanelString( string.IsNullOrEmpty( CDTXMania.DTX.PANEL ) ? CDTXMania.DTX.TITLE : CDTXMania.DTX.PANEL );
		}


		protected void t進行描画・ゲージ()
		{
			if ( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.actGauge.On進行描画();
			}
		}

		protected void t進行描画・スコア()
		{
			this.actScore.On進行描画();
		}

		protected void t進行描画・チップアニメ()
		{
			for ( int i = 0; i < 3; i++ )
			{
				this.ctチップ模様アニメ[ i ].t進行Loop();
			}
			this.ctWailingチップ模様アニメ.t進行Loop();
		}

		protected bool t進行描画・フェードイン・アウト()
		{
			switch ( base.eフェーズID )
			{
				case CStage.Eフェーズ.共通_フェードイン:
					if ( this.actFI.On進行描画() != 0 )
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
					break;

				case CStage.Eフェーズ.共通_フェードアウト:
				case CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト:
					if ( this.actFO.On進行描画() != 0 )
					{
						return true;
					}
					break;

				case CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト:
					if ( this.actFOClear.On進行描画() == 0 )
					{
						break;
					}
					return true;
		
			}
			return false;
		}

		protected abstract void t進行描画・演奏情報();
		protected void t進行描画・演奏情報(int x, int y)
		{
			if ( !CDTXMania.ConfigIni.b演奏情報を表示しない )
			{
				this.actPlayInfo.t進行描画( x, y );
			}
		}
		protected void t進行描画・背景()
		{
			if ( CDTXMania.ConfigIni.eDark == Eダークモード.OFF )
			{
				if ( this.tx背景 != null )
				{
					this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );
				}
			}
			else
			{
				CDTXMania.app.Device.Clear( ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 0f, 0 );
			}
		}
		protected void t進行描画・判定文字列()
		{
			this.actJudgeString.On進行描画();
		}
		protected void t進行描画・判定文字列1・通常位置指定の場合()
		{
			if ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) != E判定文字表示位置.判定ライン上または横 )
			{
				this.actJudgeString.On進行描画();
			}
		}
		protected void t進行描画・判定文字列2・判定ライン上指定の場合()
		{
			if ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.判定ライン上または横 )
			{
				this.actJudgeString.On進行描画();
			}
		}

		protected void t進行描画・譜面スクロール速度()
		{
			this.act譜面スクロール速度.On進行描画();
		}

		protected abstract void t背景テクスチャの生成();	// overwrited by a successor
		protected void t背景テクスチャの生成( string DefaultBgFilename, Rectangle bgrect, string bgfilename )
		{
			Bitmap image = null;
			bool flag = true;

			if ( bgfilename != null && File.Exists( bgfilename ) )
			{
				try
				{
					Bitmap bitmap2 = null;
					bitmap2 = new Bitmap( bgfilename );
					if ( ( bitmap2.Size.Width == 0 ) && ( bitmap2.Size.Height == 0 ) )
					{
						this.tx背景 = null;
						return;
					}
					Bitmap bitmap3 = new Bitmap( 640, 480 );
					Graphics graphics = Graphics.FromImage( bitmap3 );
					for ( int i = 0; i < 480; i += bitmap2.Size.Height )
					{
						for ( int j = 0; j < 640; j += bitmap2.Size.Width )
						{
							graphics.DrawImage( bitmap2, j, i, bitmap2.Width, bitmap2.Height );
						}
					}
					graphics.Dispose();
					bitmap2.Dispose();
					image = new Bitmap( CSkin.Path( DefaultBgFilename ) );
					graphics = Graphics.FromImage( image );
					ColorMatrix matrix2 = new ColorMatrix();
					matrix2.Matrix00 = 1f;
					matrix2.Matrix11 = 1f;
					matrix2.Matrix22 = 1f;
					matrix2.Matrix33 = ( (float) CDTXMania.ConfigIni.n背景の透過度 ) / 255f;
					matrix2.Matrix44 = 1f;
					ColorMatrix newColorMatrix = matrix2;
					ImageAttributes imageAttr = new ImageAttributes();
					imageAttr.SetColorMatrix( newColorMatrix );
					graphics.DrawImage( bitmap3, new Rectangle( 0, 0, 640, 480 ), 0, 0, 640, 480, GraphicsUnit.Pixel, imageAttr );
					imageAttr.Dispose();
					graphics.DrawImage( bitmap3, bgrect, bgrect.X, bgrect.Y, bgrect.Width, bgrect.Height, GraphicsUnit.Pixel );
					graphics.Dispose();
					bitmap3.Dispose();
					flag = false;
				}
				catch
				{
					Trace.TraceError( "背景画像の読み込みに失敗しました。({0})", new object[] { bgfilename } );
				}
			}
			if ( flag )
			{
				bgfilename = CSkin.Path( DefaultBgFilename );
				try
				{
					image = new Bitmap( bgfilename );
				}
				catch
				{
					Trace.TraceError( "背景画像の読み込みに失敗しました。({0})", new object[] { bgfilename } );
					this.tx背景 = null;
					return;
				}
			}
			if ( ( CDTXMania.DTX.listBMP.Count > 0 ) || ( CDTXMania.DTX.listBMPTEX.Count > 0 ) )
			{
				Graphics graphics2 = Graphics.FromImage( image );
				graphics2.FillRectangle( Brushes.Black, bgrect.X, bgrect.Y, bgrect.Width, bgrect.Height );
				graphics2.Dispose();
			}
			try
			{
				this.tx背景 = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
			}
			catch ( CTextureCreateFailedException )
			{
				Trace.TraceError( "背景テクスチャの生成に失敗しました。" );
				this.tx背景 = null;
			}
			image.Dispose();
		}


#if true		// DAMAGELEVELTUNING
#region [ DAMAGELEVELTUNING ]
		// ----------------------------------
		public float[ , ] fDamageGaugeDelta = {			// #23625 2011.1.10 ickw_284: tuned damage/recover factors
			// drums,   guitar,  bass
			{  0.004f,  0.006f,  0.006f  },
			{  0.002f,  0.003f,  0.003f  },
			{  0.000f,  0.000f,  0.000f  },
			{ -0.020f, -0.030f,	-0.030f  },
			{ -0.050f, -0.050f, -0.050f  }
		};
		public float[] fDamageLevelFactor = {
			0.5f, 1.0f, 1.5f
		};
		// ----------------------------------
#endregion
#endif

		protected void t判定にあわせてゲージを増減する( E楽器パート screenmode, E楽器パート part, E判定 e今回の判定 )
		{
			double fDamage;

#if true	// DAMAGELEVELTUNING
			switch ( e今回の判定 )
			{
				case E判定.Perfect:
				case E判定.Great:
				case E判定.Good:
				case E判定.Poor:
					fDamage = fDamageGaugeDelta[ (int) e今回の判定, (int) part ];
					break;
				case E判定.Miss:
					fDamage = fDamageGaugeDelta[ (int) e今回の判定, (int) part ];
					switch ( CDTXMania.ConfigIni.eダメージレベル )
					{
						case Eダメージレベル.少ない:
						case Eダメージレベル.普通:
						case Eダメージレベル.大きい:
							fDamage *= fDamageLevelFactor[ (int) CDTXMania.ConfigIni.eダメージレベル ];
							break;
					}
					break;

				default:
					fDamage = 0.0f;
					break;
			}
#else													// before applying #23625 modifications
			switch (e今回の判定)
			{
				case E判定.Perfect:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.01 : 0.015;
					break;

				case E判定.Great:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.006 : 0.009;
					break;

				case E判定.Good:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.002 : 0.003;
					break;

				case E判定.Poor:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.0 : 0.0;
					break;

				case E判定.Miss:
					fDamage = ( part == E楽器パート.DRUMS ) ? -0.035 : -0.035;
					switch( CDTXMania.ConfigIni.eダメージレベル )
					{
						case Eダメージレベル.少ない:
							fDamage *= 0.6;
							break;

						case Eダメージレベル.普通:
							fDamage *= 1.0;
							break;

						case Eダメージレベル.大きい:
							fDamage *= 1.6;
							break;
					}
					break;

				default:
					fDamage = 0.0;
					break;
			}
#endif
			if ( screenmode == E楽器パート.DRUMS )			// ドラム演奏画面なら、ギター/ベースのダメージも全部ドラムのゲージに集約する
			{
				part = E楽器パート.DRUMS;
			}
			this.actGauge.db現在のゲージ値[ (int) part ] += fDamage;

			if ( this.actGauge.db現在のゲージ値[ (int) part ] > 1.0 )
				this.actGauge.db現在のゲージ値[ (int) part ] = 1.0;
		}
		//-----------------
		#endregion
	}
}
