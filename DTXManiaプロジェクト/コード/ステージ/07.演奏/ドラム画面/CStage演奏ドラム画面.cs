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
	internal class CStage演奏ドラム画面 : CStage
	{
		// プロパティ

		public bool bAUTOでないチップが１つでもバーを通過した 
		{
			get;
			private set;
		}


		// コンストラクタ

		public CStage演奏ドラム画面()
		{
			Eパッド[] eパッドArray = new Eパッド[ 12 ];
			eパッドArray[ 1 ] = Eパッド.SD;
			eパッドArray[ 2 ] = Eパッド.BD;
			eパッドArray[ 3 ] = Eパッド.HT;
			eパッドArray[ 4 ] = Eパッド.LT;
			eパッドArray[ 5 ] = Eパッド.CY;
			eパッドArray[ 6 ] = Eパッド.FT;
			eパッドArray[ 7 ] = Eパッド.HHO;
			eパッドArray[ 8 ] = Eパッド.RD;
			eパッドArray[ 9 ] = Eパッド.UNKNOWN;
			eパッドArray[ 10 ] = Eパッド.UNKNOWN;
			eパッドArray[ 11 ] = Eパッド.LC;
			this.eチャンネルtoパッド = eパッドArray;
			this.nBGAスコープチャンネルマップ = new int[ , ] { { 0xc4, 0xc7, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xe0 }, { 4, 7, 0x55, 0x56, 0x57, 0x58, 0x59, 0x60 } };
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actPad = new CAct演奏Drumsパッド() );
			base.list子Activities.Add( this.actCOMBO = new CAct演奏DrumsコンボDGB() );
			base.list子Activities.Add( this.actDANGER = new CAct演奏DrumsDanger() );
			base.list子Activities.Add( this.actChipFireD = new CAct演奏DrumsチップファイアD() );
			base.list子Activities.Add( this.actChipFireGB = new CAct演奏DrumsチップファイアGB() );
			base.list子Activities.Add( this.actGauge = new CAct演奏Drumsゲージ() );
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
			Drums = new CScoreIni.C演奏記録();
			Guitar = new CScoreIni.C演奏記録();
			Bass = new CScoreIni.C演奏記録();
			r空打ちドラムチップ = new CDTX.CChip[ 10 ];
			if( CDTXMania.DTX.bチップがある.Drums && !CDTXMania.ConfigIni.bギタレボモード )
			{
				Drums.nスコア = this.actScore.Get( E楽器パート.DRUMS );
				Drums.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Drums, CDTXMania.DTX.n可視チップ数.Drums, this.nヒット数・Auto含まない.Drums.Perfect, this.actCOMBO.n現在のコンボ数.Drums最高値 );
				Drums.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Drums, this.nヒット数・Auto含まない.Drums.Perfect, this.nヒット数・Auto含まない.Drums.Great, this.nヒット数・Auto含まない.Drums.Good, this.nヒット数・Auto含まない.Drums.Poor, this.nヒット数・Auto含まない.Drums.Miss );
				Drums.nPerfect数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Perfect : this.nヒット数・Auto含まない.Drums.Perfect;
				Drums.nGreat数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Great : this.nヒット数・Auto含まない.Drums.Great;
				Drums.nGood数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Good : this.nヒット数・Auto含まない.Drums.Good;
				Drums.nPoor数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Poor : this.nヒット数・Auto含まない.Drums.Poor;
				Drums.nMiss数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Miss : this.nヒット数・Auto含まない.Drums.Miss;
				Drums.n最大コンボ数 = this.actCOMBO.n現在のコンボ数.Drums最高値;
				Drums.n全チップ数 = CDTXMania.DTX.n可視チップ数.Drums;
				for( int j = 0; j < 10; j++ )
				{
					Drums.bAutoPlay[ j ] = CDTXMania.ConfigIni.bAutoPlay[ j ];
				}
				Drums.bTight = CDTXMania.ConfigIni.bTight;
				for( int k = 0; k < 3; k++ )
				{
					Drums.bSudden[ k ] = CDTXMania.ConfigIni.bSudden[ k ];
					Drums.bHidden[ k ] = CDTXMania.ConfigIni.bHidden[ k ];
					Drums.bReverse[ k ] = CDTXMania.ConfigIni.bReverse[ k ];
					Drums.eRandom[ k ] = CDTXMania.ConfigIni.eRandom[ k ];
					Drums.bLight[ k ] = CDTXMania.ConfigIni.bLight[ k ];
					Drums.bLeft[ k ] = CDTXMania.ConfigIni.bLeft[ k ];
					Drums.f譜面スクロール速度[ k ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ k ] + 1 ) ) * 0.5f;
				}
				Drums.eDark = CDTXMania.ConfigIni.eDark;
				Drums.n演奏速度分子 = CDTXMania.ConfigIni.n演奏速度;
				Drums.n演奏速度分母 = 20;
				Drums.eHHGroup = CDTXMania.ConfigIni.eHHGroup;
				Drums.eFTGroup = CDTXMania.ConfigIni.eFTGroup;
				Drums.eCYGroup = CDTXMania.ConfigIni.eCYGroup;
				Drums.eHitSoundPriorityHH = CDTXMania.ConfigIni.eHitSoundPriorityHH;
				Drums.eHitSoundPriorityFT = CDTXMania.ConfigIni.eHitSoundPriorityFT;
				Drums.eHitSoundPriorityCY = CDTXMania.ConfigIni.eHitSoundPriorityCY;
				Drums.bGuitar有効 = CDTXMania.ConfigIni.bGuitar有効;
				Drums.bDrums有効 = CDTXMania.ConfigIni.bDrums有効;
				Drums.bSTAGEFAILED有効 = CDTXMania.ConfigIni.bSTAGEFAILED有効;
				Drums.eダメージレベル = CDTXMania.ConfigIni.eダメージレベル;
				Drums.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Drums;
				Drums.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Drums;
				Drums.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Drums;
				Drums.b演奏にマウスを使用した = this.b演奏にマウスを使った.Drums;
				Drums.nPerfectになる範囲ms = CDTXMania.nPerfect範囲ms;
				Drums.nGreatになる範囲ms = CDTXMania.nGreat範囲ms;
				Drums.nGoodになる範囲ms = CDTXMania.nGood範囲ms;
				Drums.nPoorになる範囲ms = CDTXMania.nPoor範囲ms;
				Drums.strDTXManiaのバージョン = CDTXMania.VERSION;
				Drums.最終更新日時 = DateTime.Now.ToString();
				Drums.Hash = CScoreIni.t演奏セクションのMD5を求めて返す( Drums );
			}
			if( CDTXMania.DTX.bチップがある.Guitar )
			{
				Guitar.nスコア = this.actScore.Get( E楽器パート.GUITAR );
				Guitar.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Guitar, CDTXMania.DTX.n可視チップ数.Guitar, this.nヒット数・Auto含まない.Guitar.Perfect, this.actCOMBO.n現在のコンボ数.Guitar最高値 );
				Guitar.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Guitar, this.nヒット数・Auto含まない.Guitar.Perfect, this.nヒット数・Auto含まない.Guitar.Great, this.nヒット数・Auto含まない.Guitar.Good, this.nヒット数・Auto含まない.Guitar.Poor, this.nヒット数・Auto含まない.Guitar.Miss );
				Guitar.nPerfect数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Perfect : this.nヒット数・Auto含まない.Guitar.Perfect;
				Guitar.nGreat数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Great : this.nヒット数・Auto含まない.Guitar.Great;
				Guitar.nGood数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Good : this.nヒット数・Auto含まない.Guitar.Good;
				Guitar.nPoor数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Poor : this.nヒット数・Auto含まない.Guitar.Poor;
				Guitar.nMiss数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Miss : this.nヒット数・Auto含まない.Guitar.Miss;
				Guitar.n最大コンボ数 = this.actCOMBO.n現在のコンボ数.Guitar最高値;
				Guitar.n全チップ数 = CDTXMania.DTX.n可視チップ数.Guitar;
				for( int m = 0; m < 10; m++ )
				{
					Guitar.bAutoPlay[ m ] = CDTXMania.ConfigIni.bAutoPlay[ m ];
				}
				Guitar.bTight = CDTXMania.ConfigIni.bTight;
				for( int n = 0; n < 3; n++ )
				{
					Guitar.bSudden[ n ] = CDTXMania.ConfigIni.bSudden[ n ];
					Guitar.bHidden[ n ] = CDTXMania.ConfigIni.bHidden[ n ];
					Guitar.bReverse[ n ] = CDTXMania.ConfigIni.bReverse[ n ];
					Guitar.eRandom[ n ] = CDTXMania.ConfigIni.eRandom[ n ];
					Guitar.bLight[ n ] = CDTXMania.ConfigIni.bLight[ n ];
					Guitar.bLeft[ n ] = CDTXMania.ConfigIni.bLeft[ n ];
					Guitar.f譜面スクロール速度[ n ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ n ] + 1 ) ) * 0.5f;
				}
				Guitar.eDark = CDTXMania.ConfigIni.eDark;
				Guitar.n演奏速度分子 = CDTXMania.ConfigIni.n演奏速度;
				Guitar.n演奏速度分母 = 20;
				Guitar.eHHGroup = CDTXMania.ConfigIni.eHHGroup;
				Guitar.eFTGroup = CDTXMania.ConfigIni.eFTGroup;
				Guitar.eCYGroup = CDTXMania.ConfigIni.eCYGroup;
				Guitar.eHitSoundPriorityHH = CDTXMania.ConfigIni.eHitSoundPriorityHH;
				Guitar.eHitSoundPriorityFT = CDTXMania.ConfigIni.eHitSoundPriorityFT;
				Guitar.eHitSoundPriorityCY = CDTXMania.ConfigIni.eHitSoundPriorityCY;
				Guitar.bGuitar有効 = CDTXMania.ConfigIni.bGuitar有効;
				Guitar.bDrums有効 = CDTXMania.ConfigIni.bDrums有効;
				Guitar.bSTAGEFAILED有効 = CDTXMania.ConfigIni.bSTAGEFAILED有効;
				Guitar.eダメージレベル = CDTXMania.ConfigIni.eダメージレベル;
				Guitar.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Guitar;
				Guitar.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Guitar;
				Guitar.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Guitar;
				Guitar.b演奏にマウスを使用した = this.b演奏にマウスを使った.Guitar;
				Guitar.nPerfectになる範囲ms = CDTXMania.nPerfect範囲ms;
				Guitar.nGreatになる範囲ms = CDTXMania.nGreat範囲ms;
				Guitar.nGoodになる範囲ms = CDTXMania.nGood範囲ms;
				Guitar.nPoorになる範囲ms = CDTXMania.nPoor範囲ms;
				Guitar.strDTXManiaのバージョン = CDTXMania.VERSION;
				Guitar.最終更新日時 = DateTime.Now.ToString();
				Guitar.Hash = CScoreIni.t演奏セクションのMD5を求めて返す( Guitar );
			}
			if( CDTXMania.DTX.bチップがある.Bass )
			{
				Bass.nスコア = this.actScore.Get( E楽器パート.BASS );
				Bass.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Bass, CDTXMania.DTX.n可視チップ数.Bass, this.nヒット数・Auto含まない.Bass.Perfect, this.actCOMBO.n現在のコンボ数.Bass最高値 );
				Bass.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Bass, this.nヒット数・Auto含まない.Bass.Perfect, this.nヒット数・Auto含まない.Bass.Great, this.nヒット数・Auto含まない.Bass.Good, this.nヒット数・Auto含まない.Bass.Poor, this.nヒット数・Auto含まない.Bass.Miss );
				Bass.nPerfect数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Perfect : this.nヒット数・Auto含まない.Bass.Perfect;
				Bass.nGreat数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Great : this.nヒット数・Auto含まない.Bass.Great;
				Bass.nGood数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Good : this.nヒット数・Auto含まない.Bass.Good;
				Bass.nPoor数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Poor : this.nヒット数・Auto含まない.Bass.Poor;
				Bass.nMiss数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Miss : this.nヒット数・Auto含まない.Bass.Miss;
				Bass.n最大コンボ数 = this.actCOMBO.n現在のコンボ数.Bass最高値;
				Bass.n全チップ数 = CDTXMania.DTX.n可視チップ数.Bass;
				for( int num5 = 0; num5 < 10; num5++ )
				{
					Bass.bAutoPlay[ num5 ] = CDTXMania.ConfigIni.bAutoPlay[ num5 ];
				}
				Bass.bTight = CDTXMania.ConfigIni.bTight;
				for( int num6 = 0; num6 < 3; num6++ )
				{
					Bass.bSudden[ num6 ] = CDTXMania.ConfigIni.bSudden[ num6 ];
					Bass.bHidden[ num6 ] = CDTXMania.ConfigIni.bHidden[ num6 ];
					Bass.bReverse[ num6 ] = CDTXMania.ConfigIni.bReverse[ num6 ];
					Bass.eRandom[ num6 ] = CDTXMania.ConfigIni.eRandom[ num6 ];
					Bass.bLight[ num6 ] = CDTXMania.ConfigIni.bLight[ num6 ];
					Bass.bLeft[ num6 ] = CDTXMania.ConfigIni.bLeft[ num6 ];
					Bass.f譜面スクロール速度[ num6 ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ num6 ] + 1 ) ) * 0.5f;
				}
				Bass.eDark = CDTXMania.ConfigIni.eDark;
				Bass.n演奏速度分子 = CDTXMania.ConfigIni.n演奏速度;
				Bass.n演奏速度分母 = 20;
				Bass.eHHGroup = CDTXMania.ConfigIni.eHHGroup;
				Bass.eFTGroup = CDTXMania.ConfigIni.eFTGroup;
				Bass.eCYGroup = CDTXMania.ConfigIni.eCYGroup;
				Bass.eHitSoundPriorityHH = CDTXMania.ConfigIni.eHitSoundPriorityHH;
				Bass.eHitSoundPriorityFT = CDTXMania.ConfigIni.eHitSoundPriorityFT;
				Bass.eHitSoundPriorityCY = CDTXMania.ConfigIni.eHitSoundPriorityCY;
				Bass.bGuitar有効 = CDTXMania.ConfigIni.bGuitar有効;
				Bass.bDrums有効 = CDTXMania.ConfigIni.bDrums有効;
				Bass.bSTAGEFAILED有効 = CDTXMania.ConfigIni.bSTAGEFAILED有効;
				Bass.eダメージレベル = CDTXMania.ConfigIni.eダメージレベル;
				Bass.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Guitar;
				Bass.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Guitar;
				Bass.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Guitar;
				Bass.b演奏にマウスを使用した = this.b演奏にマウスを使った.Guitar;
				Bass.nPerfectになる範囲ms = CDTXMania.nPerfect範囲ms;
				Bass.nGreatになる範囲ms = CDTXMania.nGreat範囲ms;
				Bass.nGoodになる範囲ms = CDTXMania.nGood範囲ms;
				Bass.nPoorになる範囲ms = CDTXMania.nPoor範囲ms;
				Bass.strDTXManiaのバージョン = CDTXMania.VERSION;
				Bass.最終更新日時 = DateTime.Now.ToString();
				Bass.Hash = CScoreIni.t演奏セクションのMD5を求めて返す( Bass );
			}
			for( int i = 0; i < 10; i++ )
			{
				r空打ちドラムチップ[ i ] = this.r空うちChip( E楽器パート.DRUMS, (Eパッド) i );
				if( r空打ちドラムチップ[ i ] == null )
				{
					r空打ちドラムチップ[ i ] = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( CDTXMania.Timer.n現在時刻, this.nパッド0Atoチャンネル0A[ i ] );
				}
			}
		}


		// CStage 実装

		public override void On活性化()
		{
			this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.継続;
			this.bフィルイン中 = false;
			this.n現在のトップChip = ( CDTXMania.DTX.listChip.Count > 0 ) ? 0 : -1;
			// this.n最後に再生したHHの実WAV番号 = -1;					// #23921 2011.1.4 yyagi
			this.L最後に再生したHHの実WAV番号 = new List<int>(16);
			//this.L最後に再生したHHの実WAV番号.Add(-1);

			this.n最後に再生したHHのチャンネル番号 = 0;
			this.n最後に再生したギターの実WAV番号 = -1;
			this.n最後に再生したベースの実WAV番号 = -1;
			for( int i = 0; i < 50; i++ )
			{
				this.n最後に再生したBGMの実WAV番号[ i ] = -1;
			}
			this.r次にくるギターChip = null;
			this.r次にくるベースChip = null;
			for( int j = 0; j < 10; j++ )
			{
				this.r現在の空うちドラムChip[ j ] = null;
			}
			this.r現在の空うちギターChip = null;
			this.r現在の空うちベースChip = null;
			for( int k = 0; k < 3; k++ )
			{
				for( int n = 0; n < 5; n++ )
				{
					this.nヒット数・Auto含まない[ k ] = new STHITCOUNTOFRANK();
					this.nヒット数・Auto含む[ k ] = new STHITCOUNTOFRANK();
				}
				this.queWailing[ k ] = new Queue<CDTX.CChip>();
				this.r現在の歓声Chip[ k ] = null;
			}
			for( int m = 0; m < 3; m++ )
			{
				this.b演奏にキーボードを使った[ m ] = false;
				this.b演奏にジョイパッドを使った[ m ] = false;
				this.b演奏にMIDI入力を使った[ m ] = false;
				this.b演奏にマウスを使った[ m ] = false;
			}
			this.bAUTOでないチップが１つでもバーを通過した = false;
			base.On活性化();
			this.tステータスパネルの選択();
			this.tパネル文字列の設定();

			this.nInputAdjustTimeMs.Drums = CDTXMania.ConfigIni.nInputAdjustTimeMs.Drums;		// #23580 2011.1.3 yyagi
			this.nInputAdjustTimeMs.Guitar = CDTXMania.ConfigIni.nInputAdjustTimeMs.Guitar;		//        2011.1.7 ikanick 修正
			this.nInputAdjustTimeMs.Bass = CDTXMania.ConfigIni.nInputAdjustTimeMs.Bass;			//
		}
		public override void On非活性化()
		{
			this.L最後に再生したHHの実WAV番号.Clear();	// #23921 2011.1.4 yyagi
			this.L最後に再生したHHの実WAV番号 = null;	//
			for( int i = 0; i < 3; i++ )
			{
				this.queWailing[ i ] = null;
			}
			this.ctWailingチップ模様アニメ = null;
			this.ctチップ模様アニメ.Drums = null;
			this.ctチップ模様アニメ.Guitar = null;
			this.ctチップ模様アニメ.Bass = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.t背景テクスチャの生成();
				this.txチップ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums chips.png" ) );
				this.txヒットバー = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums hit-bar.png" ) );
				this.txヒットバーGB = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums hit-bar guitar.png" ) );
				this.txWailing枠 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay wailing cursor.png" ) );
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
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.txヒットバー );
				CDTXMania.tテクスチャの解放( ref this.txヒットバーGB );
				CDTXMania.tテクスチャの解放( ref this.txチップ );
				CDTXMania.tテクスチャの解放( ref this.txレーンフレームGB );
				CDTXMania.tテクスチャの解放( ref this.txWailing枠 );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				bool flag = false;
				bool flag2 = false;
				if( base.b初めての進行描画 )
				{
					CDTXMania.Timer.tリセット();
					this.ctチップ模様アニメ.Drums = new CCounter( 0, 0x30, 10, CDTXMania.Timer );
					this.ctチップ模様アニメ.Guitar = new CCounter( 0, 0x17, 20, CDTXMania.Timer );
					this.ctチップ模様アニメ.Bass = new CCounter( 0, 0x17, 20, CDTXMania.Timer );
					this.ctWailingチップ模様アニメ = new CCounter( 0, 4, 50, CDTXMania.Timer );
					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					this.actFI.tフェードイン開始();
					base.b初めての進行描画 = false;
				}
				if( ( CDTXMania.ConfigIni.bSTAGEFAILED有効 && ( this.actGauge.db現在のゲージ値 <= -0.1 ) ) && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
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
				this.t進行描画・レーンフラッシュD();
				this.t進行描画・DANGER();
				this.t進行描画・判定ライン();
				this.t進行描画・RGBボタン();
				this.t進行描画・判定文字列1・通常位置指定の場合();
				this.t進行描画・コンボ();
				this.t進行描画・WailingBonus();
				this.t進行描画・譜面スクロール速度();
				this.t進行描画・チップアニメ();
				flag = this.t進行描画・チップ();
				this.t進行描画・演奏情報();
				this.t進行描画・ドラムパッド();
				this.t進行描画・判定文字列2・判定ライン上指定の場合();
				this.t進行描画・Wailing枠();
				this.t進行描画・チップファイアD();
				this.t進行描画・チップファイアGB();
				this.t進行描画・STAGEFAILED();
				flag2 = this.t進行描画・フェードイン・アウト();
				if( flag && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
				{
					this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージクリア;
					base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト;
					this.actFOClear.tフェードアウト開始();
				}
				if( flag2 )
				{
					return (int) this.eフェードアウト完了時の戻り値;
				}

				// キー入力

				if( CDTXMania.act現在入力を占有中のプラグイン == null )
					this.tキー入力();
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private class STHITCOUNTOFRANK
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
					switch( index )
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
					switch( index )
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
		private struct STKARAUCHI
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
					switch( index )
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
					switch( index )
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

		private CAct演奏AVI actAVI;
		private CAct演奏BGA actBGA;
		private CAct演奏DrumsチップファイアD actChipFireD;
		private CAct演奏DrumsチップファイアGB actChipFireGB;
		private CAct演奏DrumsコンボDGB actCOMBO;
		private CAct演奏DrumsDanger actDANGER;
		private CActFIFOBlack actFI;
		private CActFIFOBlack actFO;
		private CActFIFOWhite actFOClear;
		private CAct演奏Drumsゲージ actGauge;
		private CAct演奏Drums判定文字列 actJudgeString;
		private CAct演奏DrumsレーンフラッシュD actLaneFlushD;
		private CAct演奏DrumsレーンフラッシュGB actLaneFlushGB;
		private CAct演奏Drumsパッド actPad;
		private CAct演奏パネル文字列 actPanel;
		private CAct演奏演奏情報 actPlayInfo;
		private CAct演奏DrumsRGB actRGB;
		private CAct演奏Drumsスコア actScore;
		private CAct演奏ステージ失敗 actStageFailed;
		private CAct演奏Drumsステータスパネル actStatusPanels;
		private CAct演奏DrumsWailingBonus actWailingBonus;
		private CAct演奏スクロール速度 act譜面スクロール速度;
		private bool bPAUSE;
		private bool bフィルイン中;
		private STDGBVALUE<bool> b演奏にMIDI入力を使った;
		private STDGBVALUE<bool> b演奏にキーボードを使った;
		private STDGBVALUE<bool> b演奏にジョイパッドを使った;
		private STDGBVALUE<bool> b演奏にマウスを使った;
		private CCounter ctWailingチップ模様アニメ;
		private STDGBVALUE<CCounter> ctチップ模様アニメ;
		private readonly Eパッド[] eチャンネルtoパッド;
		private E演奏画面の戻り値 eフェードアウト完了時の戻り値;
		private readonly int[,] nBGAスコープチャンネルマップ;
		private readonly int[] nチャンネル0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 8, 0 };
		private readonly int[] nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
		private readonly int[] nチャンネルtoX座標 = new int[] { 0x4c, 110, 0x91, 0xc0, 0xe2, 0x126, 260, 0x4f, 300, 0x23 };
		private readonly int[] nパッド0Atoチャンネル0A = new int[] { 0x11, 0x12, 0x13, 20, 0x15, 0x17, 0x16, 0x18, 0x19, 0x1a };
		private readonly int[] nパッド0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 8, 0 };
		private readonly int[] nパッド0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 7, 0 };
		private STDGBVALUE<STHITCOUNTOFRANK> nヒット数・Auto含まない;
		private STDGBVALUE<STHITCOUNTOFRANK> nヒット数・Auto含む;
		private int n現在のトップChip = -1;
		private int[] n最後に再生したBGMの実WAV番号 = new int[ 50 ];
		private int n最後に再生したHHのチャンネル番号;
		private List<int> L最後に再生したHHの実WAV番号;		// #23921 2011.1.4 yyagi: change "int" to "List<int>", for recording multiple wav No.
		private int n最後に再生したギターの実WAV番号;
		private int n最後に再生したベースの実WAV番号;
		private STDGBVALUE<Queue<CDTX.CChip>> queWailing;
		private STDGBVALUE<CDTX.CChip> r現在の歓声Chip;
		private CDTX.CChip r現在の空うちギターChip;
		private STKARAUCHI r現在の空うちドラムChip;
		private CDTX.CChip r現在の空うちベースChip;
		private CDTX.CChip r次にくるギターChip;
		private CDTX.CChip r次にくるベースChip;
		private CTexture txWailing枠;
		private CTexture txチップ;
		private CTexture txヒットバー;
		private CTexture txヒットバーGB;
		private CTexture txレーンフレームGB;
		private CTexture tx背景;
		private STDGBVALUE<int> nInputAdjustTimeMs;			// #23580 2011.1.3 yyagi
		private bool bフィルイン区間の最後のChipである( CDTX.CChip pChip )
		{
			if( pChip == null )
			{
				return false;
			}
			int num = pChip.n発声位置;
			for( int i = CDTXMania.DTX.listChip.IndexOf( pChip ) + 1; i < CDTXMania.DTX.listChip.Count; i++ )
			{
				pChip = CDTXMania.DTX.listChip[ i ];
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
		private E判定 e指定時刻からChipのJUDGEを返す( long nTime, CDTX.CChip pChip, int nInputAdjustTime )
		{
			if( pChip != null )
			{
				int nDeltaTime = Math.Abs( (int) ( nTime + nInputAdjustTime - pChip.n発声時刻ms ) );		// #23580 2011.1.3 yyagi: add "nInputAdjustTime" to add input timing adjust feature
//Debug.WriteLine("nAbsTime=" + (nTime - pChip.n発声時刻ms) + ", nDeltaTime=" + (nTime + nInputAdjustTime - pChip.n発声時刻ms));
				if( nDeltaTime <= CDTXMania.nPerfect範囲ms )
				{
					return E判定.Perfect;
				}
				if( nDeltaTime <= CDTXMania.nGreat範囲ms )
				{
					return E判定.Great;
				}
				if( nDeltaTime <= CDTXMania.nGood範囲ms )
				{
					return E判定.Good;
				}
				if( nDeltaTime <= CDTXMania.nPoor範囲ms )
				{
					return E判定.Poor;
				}
			}
			return E判定.Miss;
		}
		private CDTX.CChip r空うちChip( E楽器パート part, Eパッド pad )
		{
			switch( part )
			{
				case E楽器パート.DRUMS:
					switch( pad )
					{
						case Eパッド.HH:
							if( this.r現在の空うちドラムChip.HH != null )
							{
								return this.r現在の空うちドラムChip.HH;
							}
							if( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける )
							{
								if( CDTXMania.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける )
								{
									return this.r現在の空うちドラムChip.HHO;
								}
								if( this.r現在の空うちドラムChip.HHO != null )
								{
									return this.r現在の空うちドラムChip.HHO;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case Eパッド.SD:
							return this.r現在の空うちドラムChip.SD;

						case Eパッド.BD:
							return this.r現在の空うちドラムChip.BD;

						case Eパッド.HT:
							return this.r現在の空うちドラムChip.HT;

						case Eパッド.LT:
							if( this.r現在の空うちドラムChip.LT != null )
							{
								return this.r現在の空うちドラムChip.LT;
							}
							if( CDTXMania.ConfigIni.eFTGroup == EFTGroup.共通 )
							{
								return this.r現在の空うちドラムChip.FT;
							}
							return null;

						case Eパッド.FT:
							if( this.r現在の空うちドラムChip.FT != null )
							{
								return this.r現在の空うちドラムChip.FT;
							}
							if( CDTXMania.ConfigIni.eFTGroup == EFTGroup.共通 )
							{
								return this.r現在の空うちドラムChip.LT;
							}
							return null;

						case Eパッド.CY:
							if( this.r現在の空うちドラムChip.CY != null )
							{
								return this.r現在の空うちドラムChip.CY;
							}
							if( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 )
							{
								return this.r現在の空うちドラムChip.RD;
							}
							return null;

						case Eパッド.HHO:
							if( this.r現在の空うちドラムChip.HHO != null )
							{
								return this.r現在の空うちドラムChip.HHO;
							}
							if( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける )
							{
								if( CDTXMania.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける )
								{
									return this.r現在の空うちドラムChip.HH;
								}
								if( this.r現在の空うちドラムChip.HH != null )
								{
									return this.r現在の空うちドラムChip.HH;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case Eパッド.RD:
							if( this.r現在の空うちドラムChip.RD != null )
							{
								return this.r現在の空うちドラムChip.RD;
							}
							if( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 )
							{
								return this.r現在の空うちドラムChip.CY;
							}
							return null;

						case Eパッド.LC:
							if( this.r現在の空うちドラムChip.LC != null )
							{
								return this.r現在の空うちドラムChip.LC;
							}
							if( ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける ) && ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.全部共通 ) )
							{
								return null;
							}
							if( this.r現在の空うちドラムChip.HH != null )
							{
								return this.r現在の空うちドラムChip.HH;
							}
							return this.r現在の空うちドラムChip.HHO;
					}
					break;

				case E楽器パート.GUITAR:
					return this.r現在の空うちギターChip;

				case E楽器パート.BASS:
					return this.r現在の空うちベースChip;
			}
			return null;
		}
		private CDTX.CChip r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( long nTime, int nChannelFlag )
		{
			int num5;
			if( this.n現在のトップChip == -1 )
			{
				return null;
			}
			int count = CDTXMania.DTX.listChip.Count;
			int num4 = num5 = this.n現在のトップChip;
			if( this.n現在のトップChip >= count )
			{
				num4 = num5 = count - 1;
			}
			int num2 = num4;
			while( num2 < count )
			{
				CDTX.CChip chip = CDTXMania.DTX.listChip[ num2 ];
				if( ( ( nChannelFlag >= 0x11 ) && ( nChannelFlag <= 0x1a ) ) && ( ( chip.nチャンネル番号 == nChannelFlag ) || ( chip.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) ) )
				{
					if( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if( ( ( nChannelFlag == 0x2f ) && ( chip.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) )
				{
					if( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if( ( ( nChannelFlag == 0xaf ) && ( chip.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) )
				{
					if( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				num2++;
			}
			int num3 = num5;
			while( num3 >= 0 )
			{
				CDTX.CChip chip2 = CDTXMania.DTX.listChip[ num3 ];
				if( ( nChannelFlag >= 0x11 ) && ( nChannelFlag <= 0x1a ) )
				{
					if( ( chip2.nチャンネル番号 == nChannelFlag ) || ( chip2.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) )
					{
						break;
					}
				}
				else if( ( ( nChannelFlag == 0x2f ) && ( chip2.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) )
				{
					if( ( chip2.nチャンネル番号 >= 0x20 ) && ( chip2.nチャンネル番号 <= 40 ) )
					{
						break;
					}
				}
				else if( ( ( ( nChannelFlag == 0xaf ) && ( chip2.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) && ( ( chip2.nチャンネル番号 >= 160 ) && ( chip2.nチャンネル番号 <= 0xa8 ) ) )
				{
					break;
				}
				num3--;
			}
			if( ( num2 == count ) && ( num3 < 0 ) )
			{
				return null;
			}
			if( num2 == count )
			{
				return CDTXMania.DTX.listChip[ num3 ];
			}
			if( num3 < 0 )
			{
				return CDTXMania.DTX.listChip[ num2 ];
			}
			CDTX.CChip chip3 = CDTXMania.DTX.listChip[ num2 ];
			CDTX.CChip chip4 = CDTXMania.DTX.listChip[ num3 ];
			int num6 = Math.Abs( (int) ( nTime - chip3.n発声時刻ms ) );
			int num7 = Math.Abs( (int) ( nTime - chip4.n発声時刻ms ) );
			if( num6 >= num7 )
			{
				return chip4;
			}
			return chip3;
		}
		private CDTX.CChip r指定時刻に一番近い未ヒットChip( long nTime, int nChannelFlag )
		{
			return this.r指定時刻に一番近い未ヒットChip( nTime, nChannelFlag, 0 );
		}
		private CDTX.CChip r指定時刻に一番近い未ヒットChip( long nTime, int nChannelFlag, int n検索範囲時間ms )
		{
			int num5;
			int num6;
			if( this.n現在のトップChip == -1 )
			{
				return null;
			}
			int count = CDTXMania.DTX.listChip.Count;
			int num4 = num5 = this.n現在のトップChip;
			if( this.n現在のトップChip >= count )
			{
				num4 = num5 = count - 1;
			}
			int num2 = num4;
			while( num2 < count )
			{
				CDTX.CChip chip = CDTXMania.DTX.listChip[ num2 ];
				if( ( !chip.bHit && ( nChannelFlag >= 0x11 ) ) && ( nChannelFlag <= 0x1a ) )
				{
					if( ( chip.nチャンネル番号 == nChannelFlag ) || ( chip.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) )
					{
						if( chip.n発声時刻ms > nTime )
						{
							break;
						}
						num5 = num2;
					}
				}
				else if( !chip.bHit && ( ( ( nChannelFlag == 0x2f ) && ( chip.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) ) )
				{
					if( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				else if( !chip.bHit && ( ( ( nChannelFlag == 0xaf ) && ( chip.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip.nチャンネル番号 == nChannelFlag ) ) ) )
				{
					if( chip.n発声時刻ms > nTime )
					{
						break;
					}
					num5 = num2;
				}
				num2++;
			}
			int num3 = num5;
			while( num3 >= 0 )
			{
				CDTX.CChip chip2 = CDTXMania.DTX.listChip[ num3 ];
				if( ( ( ( !chip2.bHit && ( nChannelFlag >= 0x11 ) ) && ( ( nChannelFlag <= 0x1a ) && ( ( chip2.nチャンネル番号 == nChannelFlag ) || ( chip2.nチャンネル番号 == ( nChannelFlag + 0x20 ) ) ) ) ) || ( !chip2.bHit && ( ( ( nChannelFlag == 0x2f ) && ( chip2.e楽器パート == E楽器パート.GUITAR ) ) || ( ( ( nChannelFlag >= 0x20 ) && ( nChannelFlag <= 40 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) ) ) || ( !chip2.bHit && ( ( ( nChannelFlag == 0xaf ) && ( chip2.e楽器パート == E楽器パート.BASS ) ) || ( ( ( nChannelFlag >= 160 ) && ( nChannelFlag <= 0xa8 ) ) && ( chip2.nチャンネル番号 == nChannelFlag ) ) ) ) )
				{
					break;
				}
				num3--;
			}
			if( ( num2 == count ) && ( num3 < 0 ) )
			{
				return null;
			}
			CDTX.CChip chip3 = null;
			if( num2 == count )
			{
				chip3 = CDTXMania.DTX.listChip[ num3 ];
				num6 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num3 ].n発声時刻ms ) );
			}
			else if( num3 < 0 )
			{
				chip3 = CDTXMania.DTX.listChip[ num2 ];
				num6 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num2 ].n発声時刻ms ) );
			}
			else
			{
				int num7 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num2 ].n発声時刻ms ) );
				int num8 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num3 ].n発声時刻ms ) );
				if( num7 < num8 )
				{
					chip3 = CDTXMania.DTX.listChip[ num2 ];
					num6 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num2 ].n発声時刻ms ) );
				}
				else
				{
					chip3 = CDTXMania.DTX.listChip[ num3 ];
					num6 = Math.Abs( (int) ( nTime - CDTXMania.DTX.listChip[ num3 ].n発声時刻ms ) );
				}
			}
			if( ( n検索範囲時間ms > 0 ) && ( num6 > n検索範囲時間ms ) )
			{
				return null;
			}
			return chip3;
		}
		private CDTX.CChip r次にくるギターChipを更新して返す()
		{
			this.r次にくるギターChip = this.r指定時刻に一番近い未ヒットChip( CDTXMania.Timer.n現在時刻, 0x2f, 500 );
			return this.r次にくるギターChip;
		}
		private CDTX.CChip r次にくるベースChipを更新して返す()
		{
			this.r次にくるベースChip = this.r指定時刻に一番近い未ヒットChip( CDTXMania.Timer.n現在時刻, 0xaf, 500 );
			return this.r次にくるベースChip;
		}
		private void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, CDTXMania.ConfigIni.n手動再生音量, false, false );
		}
		private void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量 )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, n音量, false, false );
		}
		private void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, n音量, bモニタ, false );
		}
		private void tサウンド再生( CDTX.CChip pChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ, bool b音程をずらして再生 )
		{
			if( pChip != null )
			{
				switch( part )
				{
					case E楽器パート.DRUMS:
						{
							int index = pChip.nチャンネル番号;
							if( ( index < 0x11 ) || ( index > 0x1a ) )
							{
								if( ( index < 0x31 ) || ( index > 0x3a ) )
								{
									return;
								}
								index -= 0x31;
							}
							else
							{
								index -= 0x11;
							}
							int nLane = this.nチャンネル0Atoレーン07[ index ];
							if( ( nLane == 1 ) &&	// 今回演奏するのがHC or HO
								( index == 0 || ( index == 7 && this.n最後に再生したHHのチャンネル番号 != 0x18 && this.n最後に再生したHHのチャンネル番号 != 0x38 ) )
								// HCを演奏するか、またはHO演奏＆以前HO演奏でない＆以前不可視HO演奏でない
							)
							{
						// #23921 2011.1.4 yyagi: 2種類以上のオープンハイハットが発音済みだと、最後のHHOしか消せない問題に対応。
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
								if (CDTXMania.DTX.bHH演奏で直前のHHを消音する)
								{
#endif
									for (int i = 0; i < this.L最後に再生したHHの実WAV番号.Count; i++)		// #23921 2011.1.4 yyagi
									{
										// CDTXMania.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号);
										CDTXMania.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号[i]);	// #23921 yyagi ストック分全て消音する
									}
									this.L最後に再生したHHの実WAV番号.Clear();
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
								}
#endif
								//this.n最後に再生したHHの実WAV番号 = pChip.n整数値・内部番号;
								this.n最後に再生したHHのチャンネル番号 = pChip.nチャンネル番号;
							}
#if TEST_NOTEOFFMODE	// 2011.1.4 yyagi test
							if (CDTXMania.DTX.bHH演奏で直前のHHを消音する)
							{
#endif
								if (index == 7 || index == 0x27)						// #23921 HOまたは不可視HO演奏時はそのチップ番号をストックしておく
								{
									if (this.L最後に再生したHHの実WAV番号.Count >= 16)	// #23921 ただしストック数が16以上になるようなら、頭の1個を削って常に16未満に抑える
									{													// (ストックが増えてList<>のrealloc()が発生するのを予防する)
										this.L最後に再生したHHの実WAV番号.RemoveAt(0);
									}
									if (this.L最後に再生したHHの実WAV番号.IndexOf(pChip.n整数値・内部番号) < 0)	// チップ音がまだストックされてなければ
									{
										this.L最後に再生したHHの実WAV番号.Add(pChip.n整数値・内部番号);			// ストックする
									}
								}
#if TEST_NOTEOFFMODE	// 2011.1.4 yyagi test
							}
#endif
							CDTXMania.DTX.tチップの再生( pChip, n再生開始システム時刻ms, nLane, n音量, bモニタ );
							return;
						}
					case E楽器パート.GUITAR:
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
						if (CDTXMania.DTX.bGUITAR演奏で直前のGUITARを消音する) {
#endif
							CDTXMania.DTX.tWavの再生停止(this.n最後に再生したギターの実WAV番号);
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.DTX.tチップの再生(pChip, n再生開始システム時刻ms, 8, n音量, bモニタ, b音程をずらして再生);
						this.n最後に再生したギターの実WAV番号 = pChip.n整数値・内部番号;
						return;

					case E楽器パート.BASS:
#if TEST_NOTEOFFMODE
						if (CDTXMania.DTX.bBASS演奏で直前のBASSを消音する) {
#endif
							CDTXMania.DTX.tWavの再生停止(this.n最後に再生したベースの実WAV番号);
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.DTX.tチップの再生( pChip, n再生開始システム時刻ms, 9, n音量, bモニタ, b音程をずらして再生 );
						this.n最後に再生したベースの実WAV番号 = pChip.n整数値・内部番号;
						return;
				}
			}
		}
		private void tステータスパネルの選択()
		{
			if( CDTXMania.bコンパクトモード )
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する( null );
			}
			else if( CDTXMania.stage選曲.r確定された曲 != null )
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する( CDTXMania.stage選曲.r確定された曲.ar難易度ラベル[ CDTXMania.stage選曲.n確定された曲の難易度 ] );
			}
		}
		private E判定 tチップのヒット処理( long nHitTime, CDTX.CChip pChip )
		{
			pChip.bHit = true;
			bool bIsAutoPlay = false;
			if( (
					( ( pChip.e楽器パート == E楽器パート.DRUMS ) && CDTXMania.ConfigIni.bAutoPlay[ this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ] ] ) ||
					( ( pChip.e楽器パート == E楽器パート.GUITAR ) && CDTXMania.ConfigIni.bAutoPlay.Guitar )
				) ||
					( ( pChip.e楽器パート == E楽器パート.BASS ) && CDTXMania.ConfigIni.bAutoPlay.Bass )
			  )
			{
				bIsAutoPlay = true;
			}
			else
			{
				this.bAUTOでないチップが１つでもバーを通過した = true;
			}
			E判定 eJudgeResult = E判定.Auto;
			switch (pChip.e楽器パート)
			{
				case E楽器パート.DRUMS:
					eJudgeResult = this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, bIsAutoPlay? 0 : this.nInputAdjustTimeMs.Drums);
					this.actJudgeString.Start(this.nチャンネル0Atoレーン07[pChip.nチャンネル番号 - 0x11], bIsAutoPlay ? E判定.Auto : eJudgeResult);
					break;

				case E楽器パート.GUITAR:
					eJudgeResult = this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, bIsAutoPlay? 0 : this.nInputAdjustTimeMs.Guitar);
					this.actJudgeString.Start(10, bIsAutoPlay ? E判定.Auto : eJudgeResult);
					break;

				case E楽器パート.BASS:
					eJudgeResult = this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, bIsAutoPlay? 0 : this.nInputAdjustTimeMs.Bass);
					this.actJudgeString.Start(11, bIsAutoPlay ? E判定.Auto : eJudgeResult);
					break;
			}
			if( !bIsAutoPlay && ( pChip.e楽器パート != E楽器パート.UNKNOWN ) )
			{
				this.t判定にあわせてゲージを増減する( pChip.e楽器パート, eJudgeResult );
			}
			switch( pChip.e楽器パート )
			{
				case E楽器パート.DRUMS:
					if ((eJudgeResult != E判定.Miss) && (eJudgeResult != E判定.Bad))
					{
						this.nヒット数・Auto含む.Drums[(int)eJudgeResult]++;
					}
					else
					{
						this.nヒット数・Auto含む.Drums.Miss++;
					}
					if (!bIsAutoPlay)
					{
						switch (eJudgeResult)
						{
							case E判定.Miss:
							case E判定.Bad:
								this.nヒット数・Auto含まない.Drums.Miss++;
                                break;
                            default:    // #24068 2011.1.10 ikanick changed
                                this.nヒット数・Auto含まない.Drums[(int)eJudgeResult]++;
                                break;
						}
					}
					if (CDTXMania.ConfigIni.bドラムが全部オートプレイである || !bIsAutoPlay)
					{
						switch (eJudgeResult)
						{
							case E判定.Perfect:
							case E判定.Great:
							case E判定.Good:
								this.actCOMBO.n現在のコンボ数.Drums++;
								break;

							default:
								this.actCOMBO.n現在のコンボ数.Drums = 0;
								break;
						}
					}
					break;

				case E楽器パート.GUITAR:
					if( ( eJudgeResult != E判定.Miss ) && ( eJudgeResult != E判定.Bad ) )
					{
						this.nヒット数・Auto含む.Guitar[(int)eJudgeResult]++;
					}
					else
					{
						this.nヒット数・Auto含む.Guitar.Miss++;
					}
					if( !bIsAutoPlay )
					{
						switch( eJudgeResult )
						{
							case E判定.Miss:
							case E判定.Bad:
								this.nヒット数・Auto含まない.Guitar.Miss++;
                                break;
                            default:    // #24068 2011.1.10 ikanick changed
                                this.nヒット数・Auto含まない.Guitar[(int)eJudgeResult]++;
                                break;
						}
					}
					switch (eJudgeResult)
					{
						case E判定.Perfect:
						case E判定.Great:
						case E判定.Good:
							this.actCOMBO.n現在のコンボ数.Guitar++;
							break;

						default:
							this.actCOMBO.n現在のコンボ数.Guitar = 0;
							break;
					}
					break;

				case E楽器パート.BASS:
					if( ( eJudgeResult != E判定.Miss ) && ( eJudgeResult != E判定.Bad ) )
					{
						this.nヒット数・Auto含む.Bass[(int)eJudgeResult]++;
					}
					else
					{
						this.nヒット数・Auto含む.Bass.Miss++;
					}
					if( !bIsAutoPlay )
					{
						switch( eJudgeResult )
						{
							case E判定.Miss:
							case E判定.Bad:
								this.nヒット数・Auto含まない.Bass.Miss++;
								break;
                            default:    // #24068 2011.1.10 ikanick changed
                                this.nヒット数・Auto含まない.Bass[(int)eJudgeResult]++;
                                break;
						}
					}
					switch( eJudgeResult )
					{
						case E判定.Perfect:
						case E判定.Great:
						case E判定.Good:
							this.actCOMBO.n現在のコンボ数.Bass++;
							break;

						default:
							this.actCOMBO.n現在のコンボ数.Bass = 0;
							break;
					}
					break;

				default:
					break;
			}
			if( ( !bIsAutoPlay && ( pChip.e楽器パート != E楽器パート.UNKNOWN ) ) && ( ( eJudgeResult != E判定.Miss ) && ( eJudgeResult != E判定.Bad ) ) )
			{
				int nCombos = 0;
				switch (pChip.e楽器パート) {
					case E楽器パート.DRUMS:
						nCombos = this.actCOMBO.n現在のコンボ数.Drums;
						break;
					case E楽器パート.GUITAR:
						nCombos = this.actCOMBO.n現在のコンボ数.Guitar;
						break;
					case E楽器パート.BASS:
						nCombos = this.actCOMBO.n現在のコンボ数.Bass;
						break;
				}
				long nScore = this.actScore.Get( pChip.e楽器パート );
				long[] numArray = new long[] { 350L, 200L, 50L, 0L };
				if( ( nCombos <= 500 ) || ( eJudgeResult == E判定.Good ) )
				{
					nScore += numArray[ (int) eJudgeResult ] * nCombos;
				}
				else if( ( eJudgeResult == E判定.Perfect ) || ( eJudgeResult == E判定.Great ) )
				{
					nScore += numArray[ (int) eJudgeResult ] * 500L;
				}
				this.actScore.Set( pChip.e楽器パート, nScore );
			}
			return eJudgeResult;
		}
		private void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part )
		{
			this.tチップのヒット処理・BadならびにTight時のMiss( part, 0 );
		}
		private void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part, int nLane )
		{
			this.bAUTOでないチップが１つでもバーを通過した = true;
			this.t判定にあわせてゲージを増減する( part, E判定.Miss );
			switch( part )
			{
				case E楽器パート.DRUMS:
					if( ( nLane >= 0 ) && ( nLane <= 7 ) )
					{
						this.actJudgeString.Start( nLane, CDTXMania.ConfigIni.bAutoPlay[ nLane ] ? E判定.Auto : E判定.Miss );
					}
					this.actCOMBO.n現在のコンボ数.Drums = 0;
					return;

				case E楽器パート.GUITAR:
					this.actJudgeString.Start( 10, E判定.Bad );
					this.actCOMBO.n現在のコンボ数.Guitar = 0;
					return;

				case E楽器パート.BASS:
					this.actJudgeString.Start( 11, E判定.Bad );
					this.actCOMBO.n現在のコンボ数.Bass = 0;
					return;
			}
		}
		private bool tドラムヒット処理( long nHitTime, Eパッド type, CDTX.CChip pChip, int n強弱度合い0to127 )
		{
			if( pChip == null )
			{
				return false;
			}
			E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nHitTime, pChip, nInputAdjustTimeMs.Drums );
			if( e判定 == E判定.Miss )
			{
				return false;
			}
			this.tチップのヒット処理( nHitTime, pChip );
			int index = pChip.nチャンネル番号;
			if( ( index >= 0x11 ) && ( index <= 0x1a ) )
			{
				index -= 0x11;
			}
			else if( ( index >= 0x31 ) && ( index <= 0x3a ) )
			{
				index -= 0x31;
			}
			int num2 = this.nチャンネル0Atoレーン07[ index ];
			int nLane = this.nチャンネル0Atoパッド08[ index ];
			this.actLaneFlushD.Start( (Eドラムレーン) num2, ( (float) n強弱度合い0to127 ) / 127f );
			this.actPad.Hit( nLane );
			if( ( e判定 != E判定.Poor ) && ( e判定 != E判定.Miss ) )
			{
				bool flag = this.bフィルイン中;
				bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである( pChip );
				bool flag3 = flag2;
				this.actChipFireD.Start( (Eドラムレーン) num2, flag, flag2, flag3 );
			}
			if( CDTXMania.ConfigIni.bドラム打音を発声する )
			{
				CDTX.CChip rChip = null;
				bool flag4 = true;
				if( ( ( type == Eパッド.HH ) || ( type == Eパッド.HHO ) ) || ( type == Eパッド.LC ) )
				{
					flag4 = CDTXMania.ConfigIni.eHitSoundPriorityHH == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if( ( type == Eパッド.LT ) || ( type == Eパッド.FT ) )
				{
					flag4 = CDTXMania.ConfigIni.eHitSoundPriorityFT == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if( ( type == Eパッド.CY ) || ( type == Eパッド.RD ) )
				{
					flag4 = CDTXMania.ConfigIni.eHitSoundPriorityCY == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				if( flag4 )
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
					rChip = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nHitTime, this.nパッド0Atoチャンネル0A[ (int) hH ] );
					if( rChip == null )
					{
						rChip = pChip;
					}
				}
				this.tサウンド再生( rChip, CDTXMania.Timer.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
			}
			return true;
		}
		private void tパネル文字列の設定()
		{
			this.actPanel.SetPanelString( string.IsNullOrEmpty( CDTXMania.DTX.PANEL ) ? CDTXMania.DTX.TITLE : CDTXMania.DTX.PANEL );
		}
		private void tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Input管理.Keyboard;
			if( keyboard.bキーが押された( 0x36 ) && ( keyboard.bキーが押されている( 120 ) || keyboard.bキーが押されている( 0x4e ) ) )
			{
				this.bPAUSE = !this.bPAUSE;
				if( this.bPAUSE )
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
			if( ( !this.bPAUSE && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.t入力処理・ドラム();
				this.t入力処理・ギター();
				this.t入力処理・ベース();
				if( keyboard.bキーが押された( 0x84 ) && ( keyboard.bキーが押されている( 120 ) || keyboard.bキーが押されている( 0x4e ) ) )
				{
					CDTXMania.DTX.t各自動再生音チップの再生時刻を変更する( ( keyboard.bキーが押されている( 0x4b ) || keyboard.bキーが押されている( 0x74 ) ) ? 1 : 10 );
					CDTXMania.DTX.tWave再生位置自動補正();
				}
				else if( keyboard.bキーが押された( 50 ) && ( keyboard.bキーが押されている( 120 ) || keyboard.bキーが押されている( 0x4e ) ) )
				{
					CDTXMania.DTX.t各自動再生音チップの再生時刻を変更する( ( keyboard.bキーが押されている( 0x4b ) || keyboard.bキーが押されている( 0x74 ) ) ? -1 : -10 );
					CDTXMania.DTX.tWave再生位置自動補正();
				}
				else if( keyboard.bキーが押された( 0x84 ) )
				{
					CDTXMania.ConfigIni.n譜面スクロール速度.Drums = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Drums + 1, 0x7cf );
				}
				else if( keyboard.bキーが押された( 50 ) )
				{
					CDTXMania.ConfigIni.n譜面スクロール速度.Drums = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Drums - 1, 0 );
				}
				else if( keyboard.bキーが押された( 0x31 ) )
				{
					CDTXMania.ConfigIni.b演奏情報を表示する = !CDTXMania.ConfigIni.b演奏情報を表示する;
				}
				else if( ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) && ( keyboard.bキーが押された( 0x35 ) || CDTXMania.Pad.b押されたGB( Eパッド.FT ) ) )
				{
					this.actFO.tフェードアウト開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
					this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.演奏中断;
				}
			}
		}
		private void t進行描画・AVI()
		{
			if( ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( !CDTXMania.ConfigIni.bストイックモード && CDTXMania.ConfigIni.bAVI有効 ) )
			{
				this.actAVI.t進行描画( 0x152, 0x39 );
			}
		}
		private void t進行描画・BGA()
		{
			if( ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( !CDTXMania.ConfigIni.bストイックモード && CDTXMania.ConfigIni.bBGA有効 ) )
			{
				this.actBGA.t進行描画( 0x152, 0x39 );
			}
		}
		private void t進行描画・DANGER()
		{
			this.actDANGER.t進行描画( this.actGauge.db現在のゲージ値 < 0.3 );
		}
		private void t進行描画・MIDIBGM()
		{
			if( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED )
			{
				CStage.Eフェーズ eフェーズid1 = base.eフェーズID;
			}
		}
		private void t進行描画・RGBボタン()
		{
			if( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				this.actRGB.On進行描画();
			}
		}
		private void t進行描画・STAGEFAILED()
		{
			if( ( ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED ) || ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) && ( ( this.actStageFailed.On進行描画() != 0 ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージ失敗;
				base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト;
				this.actFO.tフェードアウト開始();
			}
		}
		private void t進行描画・WailingBonus()
		{
			if( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.actWailingBonus.On進行描画();
			}
		}
		private void t進行描画・Wailing枠()
		{
			if( ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if( CDTXMania.DTX.bチップがある.Guitar && ( this.txWailing枠 != null ) )
				{
					this.txWailing枠.t2D描画( CDTXMania.app.Device, 0x24b, CDTXMania.ConfigIni.bReverse.Guitar ? ( 400 - this.txWailing枠.sz画像サイズ.Height ) : 0x45 );
				}
				if( CDTXMania.DTX.bチップがある.Bass && ( this.txWailing枠 != null ) )
				{
					this.txWailing枠.t2D描画( CDTXMania.app.Device, 0x1de, CDTXMania.ConfigIni.bReverse.Bass ? ( 400 - this.txWailing枠.sz画像サイズ.Height ) : 0x45 );
				}
			}
		}
		private void t進行描画・ギターベースフレーム()
		{
			if( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if( CDTXMania.DTX.bチップがある.Guitar )
				{
					for( int i = 0; i < 0x163; i += 0x80 )
					{
						Rectangle rectangle = new Rectangle( 0, 0, 0x6d, 0x80 );
						if( ( i + 0x80 ) > 0x163 )
						{
							rectangle.Height -= ( i + 0x80 ) - 0x163;
						}
						if( this.txレーンフレームGB != null )
						{
							this.txレーンフレームGB.t2D描画( CDTXMania.app.Device, 0x1fb, 0x39 + i, rectangle );
						}
					}
				}
				if( CDTXMania.DTX.bチップがある.Bass )
				{
					for( int j = 0; j < 0x163; j += 0x80 )
					{
						Rectangle rectangle2 = new Rectangle( 0, 0, 0x6d, 0x80 );
						if( ( j + 0x80 ) > 0x163 )
						{
							rectangle2.Height -= ( j + 0x80 ) - 0x163;
						}
						if( this.txレーンフレームGB != null )
						{
							this.txレーンフレームGB.t2D描画( CDTXMania.app.Device, 0x18e, 0x39 + j, rectangle2 );
						}
					}
				}
			}
		}
		private void t進行描画・ギターベース判定ライン()
		{
			if( ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				if( CDTXMania.DTX.bチップがある.Guitar )
				{
					int y = ( CDTXMania.ConfigIni.bReverse.Guitar ? 0x176 : 0x5f ) - 3;
					for( int i = 0; i < 3; i++ )
					{
						if( this.txヒットバーGB != null )
						{
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, 0x1fd + ( 0x1a * i ), y );
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, ( 0x1fd + ( 0x1a * i ) ) + 0x10, y, new Rectangle( 0, 0, 10, 0x10 ) );
						}
					}
				}
				if( CDTXMania.DTX.bチップがある.Bass )
				{
					int num3 = ( CDTXMania.ConfigIni.bReverse.Bass ? 0x176 : 0x5f ) - 3;
					for( int j = 0; j < 3; j++ )
					{
						if( this.txヒットバーGB != null )
						{
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, 400 + ( 0x1a * j ), num3 );
							this.txヒットバーGB.t2D描画( CDTXMania.app.Device, ( 400 + ( 0x1a * j ) ) + 0x10, num3, new Rectangle( 0, 0, 10, 0x10 ) );
						}
					}
				}
			}
		}
		private void t進行描画・ゲージ()
		{
			if( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.actGauge.On進行描画();
			}
		}
		private void t進行描画・コンボ()
		{
			this.actCOMBO.On進行描画();
		}
		private void t進行描画・スコア()
		{
			this.actScore.On進行描画();
		}
		private void t進行描画・ステータスパネル()
		{
			this.actStatusPanels.On進行描画();
		}
		private bool t進行描画・チップ()
		{
			if( ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED ) || ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				return true;
			}
			if( ( this.n現在のトップChip == -1 ) || ( this.n現在のトップChip >= CDTXMania.DTX.listChip.Count ) )
			{
				return true;
			}

			//double speed = 264.0;	// BPM150の時の1小節の長さ[dot]
			double speed = 234.0;	// BPM150の時の1小節の長さ[dot]

			double num = ( ( ( ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Drums + 1.0 ) * 0.5 ) * 37.5 ) * speed ) / 60000.0;
			double num2 = ( ( ( ( ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Guitar + 1.0 ) * 0.5 ) * 0.5 ) * 37.5 ) * speed ) / 60000.0;
			double num3 = ( ( ( ( ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Bass + 1.0 ) * 0.5 ) * 0.5 ) * 37.5 ) * speed ) / 60000.0;
			int num4 = this.n現在のトップChip;
			if( num4 == -1 )
			{
				return true;
			}
			CDTX dTX = CDTXMania.DTX;
			CConfigIni configIni = CDTXMania.ConfigIni;
			while( num4 < dTX.listChip.Count )
			{
				int num6;
				CDTX.CChip pChip = dTX.listChip[ num4 ];
				pChip.nバーからの距離dot.Drums = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * num );
				pChip.nバーからの距離dot.Guitar = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * num2 );
				pChip.nバーからの距離dot.Bass = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * num3 );
				if( Math.Min( Math.Min( pChip.nバーからの距離dot.Drums, pChip.nバーからの距離dot.Guitar ), pChip.nバーからの距離dot.Bass ) > 450 )
				{
					break;
				}
				if( ( ( num4 == this.n現在のトップChip ) && ( pChip.nバーからの距離dot.Drums < -65 ) ) && pChip.bHit )
				{
					this.n現在のトップChip++;
					num4 = this.n現在のトップChip;
					continue;
				}

				bool bIsAutoPlay = false;
				if ((
						((pChip.e楽器パート == E楽器パート.DRUMS) && CDTXMania.ConfigIni.bAutoPlay[this.nチャンネル0Atoレーン07[pChip.nチャンネル番号 - 0x11]]) ||
						((pChip.e楽器パート == E楽器パート.GUITAR) && CDTXMania.ConfigIni.bAutoPlay.Guitar)
					) ||
						((pChip.e楽器パート == E楽器パート.BASS) && CDTXMania.ConfigIni.bAutoPlay.Bass)
				  )
//				if ((pChip.e楽器パート == E楽器パート.DRUMS) && CDTXMania.ConfigIni.bAutoPlay[this.nチャンネル0Atoレーン07[pChip.nチャンネル番号 - 0x11]])
				{
					bIsAutoPlay = true;
				}

				int nInputAdjustTime = 0;
				if (bIsAutoPlay)
				{
					//nInputAdjustTime = 0;
				}
				else if (pChip.e楽器パート == E楽器パート.UNKNOWN)
				{
					//nInputAdjustTime = 0;
				}
				else
				{
					// int[] nInputAdjustTimes = new int[] { this.nInputAdjustTimeMs_Drums, this.nInputAdjustTimeMs_Guitar, this.nInputAdjustTimeMs_Bass };
					// nInputAdjustTime = nInputAdjustTimes[(int)pChip.e楽器パート];
					nInputAdjustTime = this.nInputAdjustTimeMs[(int)pChip.e楽器パート];
				}
				if( ( ( pChip.e楽器パート != E楽器パート.UNKNOWN ) && !pChip.bHit ) &&
					( ( pChip.nバーからの距離dot.Drums < 0 ) && ( this.e指定時刻からChipのJUDGEを返す( CDTXMania.Timer.n現在時刻, pChip, nInputAdjustTime ) == E判定.Miss ) ) )
				{
					this.tチップのヒット処理( CDTXMania.Timer.n現在時刻, pChip );
				}
				switch( pChip.nチャンネル番号 )
				{
					case 0x01:	// BGM
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( configIni.bBGM音を発声する )
							{
								dTX.tチップの再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, 10, dTX.nモニタを考慮した音量( E楽器パート.UNKNOWN ) );
							}
						}
						break;

					case 0x03:	// BPM変更
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							this.actPlayInfo.dbBPM = ( pChip.n整数値 * ( ( (double) configIni.n演奏速度 ) / 20.0 ) ) + dTX.BASEBPM;
						}
						break;

					case 0x04:	// レイヤーBGA
					case 0x07:
					case 0x55:
					case 0x56:
					case 0x57:
					case 0x58:
					case 0x59:
					case 0x60:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( configIni.bBGA有効 )
							{
								switch( pChip.eBGA種別 )
								{
									case EBGA種別.BMPTEX:
										if( pChip.rBMPTEX != null )
										{
											this.actBGA.Start( pChip.nチャンネル番号, null, pChip.rBMPTEX, pChip.rBMPTEX.tx画像.sz画像サイズ.Width, pChip.rBMPTEX.tx画像.sz画像サイズ.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
										}
										break;

									case EBGA種別.BGA:
										if( ( pChip.rBGA != null ) && ( ( pChip.rBMP != null ) || ( pChip.rBMPTEX != null ) ) )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, pChip.rBMPTEX, pChip.rBGA.pt画像側右下座標.X - pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側右下座標.Y - pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt表示座標.X, pChip.rBGA.pt表示座標.Y, 0, 0, 0 );
										}
										break;

									case EBGA種別.BGAPAN:
										if( ( pChip.rBGAPan != null ) && ( ( pChip.rBMP != null ) || ( pChip.rBMPTEX != null ) ) )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, pChip.rBMPTEX, pChip.rBGAPan.sz開始サイズ.Width, pChip.rBGAPan.sz開始サイズ.Height, pChip.rBGAPan.sz終了サイズ.Width, pChip.rBGAPan.sz終了サイズ.Height, pChip.rBGAPan.pt画像側開始位置.X, pChip.rBGAPan.pt画像側開始位置.Y, pChip.rBGAPan.pt画像側終了位置.X, pChip.rBGAPan.pt画像側終了位置.Y, pChip.rBGAPan.pt表示側開始位置.X, pChip.rBGAPan.pt表示側開始位置.Y, pChip.rBGAPan.pt表示側終了位置.X, pChip.rBGAPan.pt表示側終了位置.Y, pChip.n総移動時間 );
										}
										break;

									default:
										if( pChip.rBMP != null )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, null, pChip.rBMP.n幅, pChip.rBMP.n高さ, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
										}
										break;
								}
							}
						}
						break;

					case 0x08:	// BPM変更(拡張)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( dTX.listBPM.ContainsKey( pChip.n整数値・内部番号 ) )
							{
								this.actPlayInfo.dbBPM = ( dTX.listBPM[ pChip.n整数値・内部番号 ].dbBPM値 * ( ( (double) configIni.n演奏速度 ) / 20.0 ) ) + dTX.BASEBPM;
							}
						}
						break;

					case 0x11:	// ドラム演奏
					case 0x12:
					case 0x13:
					case 0x14:
					case 0x15:
					case 0x16:
					case 0x17:
					case 0x18:
					case 0x19:
					case 0x1a:
						num6 = this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ];
						if( configIni.bDrums有効 )
						{
							if( configIni.bSudden.Drums )
							{
								if( pChip.nバーからの距離dot.Drums < 200 )
								{
									pChip.b可視 = true;
									pChip.n透明度 = 0xff;
								}
								else if( pChip.nバーからの距離dot.Drums < 250 )
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
							if( configIni.bHidden.Drums )
							{
								if( pChip.nバーからの距離dot.Drums < 100 )
								{
									pChip.b可視 = false;
								}
								else if( pChip.nバーからの距離dot.Drums < 150 )
								{
									pChip.b可視 = true;
									pChip.n透明度 = (int) ( ( ( (double) ( pChip.nバーからの距離dot.Drums - 100 ) ) * 255.0 ) / 50.0 );
								}
							}
							if( !pChip.bHit && pChip.b可視 )
							{
								if( this.txチップ != null )
								{
									this.txチップ.n透明度 = pChip.n透明度;
								}
								int x = this.nチャンネルtoX座標[ pChip.nチャンネル番号 - 0x11 ];
								int num8 = configIni.bReverse.Drums ? ( 0x38 + pChip.nバーからの距離dot.Drums ) : ( 0x1a6 - pChip.nバーからの距離dot.Drums );
								if( this.txチップ != null )
								{
									this.txチップ.vc拡大縮小倍率 = new Vector3( (float) pChip.dbチップサイズ倍率, (float) pChip.dbチップサイズ倍率, 1f );
								}
								int num9 = this.ctチップ模様アニメ.Drums.n現在の値;
								switch( pChip.nチャンネル番号 )
								{
									case 0x11:
										x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0x2c, num9 * 7, 0x20, 8 ) );
										}
										break;

									case 0x12:
										x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0x4c, num9 * 7, 0x20, 8 ) );
										}
										break;

									case 0x13:
										x = ( x + 0x16 ) - ( (int) ( ( 44.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 5, new Rectangle( 0, num9 * 9, 0x2c, 10 ) );
										}
										break;

									case 0x14:
										x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0x6c, num9 * 7, 0x20, 8 ) );
										}
										break;

									case 0x15:
										x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 140, num9 * 7, 0x20, 8 ) );
										}
										break;

									case 0x16:
										x = ( x + 0x13 ) - ( (int) ( ( 38.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 0x15, new Rectangle( 0xcc, 0x158, 0x26, 0x24 ) );
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0xcc, num9 * 7, 0x26, 8 ) );
										}
										break;

									case 0x17:
										x = ( x + 0x10 ) - ( (int) ( ( 32.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0xac, num9 * 7, 0x20, 8 ) );
										}
										break;

									case 0x18:
										x = ( x + 13 ) - ( (int) ( ( 26.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0xf2, num9 * 7, 0x1a, 8 ) );
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 9, new Rectangle( 0xf2, 0x158, 0x1a, 0x12 ) );
										}
										break;

									case 0x19:
										x = ( x + 13 ) - ( (int) ( ( 26.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0xf2, num9 * 7, 0x1a, 8 ) );
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 9, new Rectangle( 0xf2, 0x158, 0x1a, 0x12 ) );
										}
										break;

									case 0x1a:
										x = ( x + 0x13 ) - ( (int) ( ( 38.0 * pChip.dbチップサイズ倍率 ) / 2.0 ) );
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 0x15, new Rectangle( 0xcc, 0x158, 0x26, 0x24 ) );
											this.txチップ.t2D描画( CDTXMania.app.Device, x, num8 - 4, new Rectangle( 0xcc, num9 * 7, 0x26, 8 ) );
										}
										break;
								}
								if( this.txチップ != null )
								{
									this.txチップ.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
									this.txチップ.n透明度 = 0xff;
								}
							}
							if( ( configIni.bAutoPlay[ num6 ] && !pChip.bHit ) && ( pChip.nバーからの距離dot.Drums < 0 ) )
							{
								pChip.bHit = true;
								this.actLaneFlushD.Start( (Eドラムレーン) num6, ( (float) CInput管理.n通常音量 ) / 127f );
								bool flag = this.bフィルイン中;
								bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである( pChip );
								bool flag3 = flag2;
								this.actChipFireD.Start( (Eドラムレーン) num6, flag, flag2, flag3 );
								this.actPad.Hit( this.nチャンネル0Atoパッド08[ pChip.nチャンネル番号 - 0x11 ] );
								this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, dTX.nモニタを考慮した音量( E楽器パート.DRUMS ) );
								this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
							}
							break;
						}
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, dTX.nモニタを考慮した音量( E楽器パート.DRUMS ) );
							pChip.bHit = true;
						}
						break;

					case 0x1f:	// フィルインサウンド(ドラム)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の歓声Chip.Drums = pChip;
						}
						break;

					case 0x20:	// ギター演奏
					case 0x21:
					case 0x22:
					case 0x23:
					case 0x24:
					case 0x25:
					case 0x26:
					case 0x27:
						if( configIni.bGuitar有効 )
						{
							if( configIni.bSudden.Guitar )
							{
								pChip.b可視 = pChip.nバーからの距離dot.Guitar < 200;
							}
							if( configIni.bHidden.Guitar && ( pChip.nバーからの距離dot.Guitar < 100 ) )
							{
								pChip.b可視 = false;
							}
							if( !pChip.bHit && pChip.b可視 )
							{
								int num10 = configIni.bReverse.Guitar ? ( 0x176 - pChip.nバーからの距離dot.Guitar ) : ( 0x5f + pChip.nバーからの距離dot.Guitar );
								if( ( num10 > 0x39 ) && ( num10 < 0x19c ) )
								{
									int num11 = this.ctチップ模様アニメ.Guitar.n現在の値;
									if( pChip.nチャンネル番号 == 0x20 )
									{
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1fd, num10 - 2, new Rectangle( 0x10c, 0x90 + ( ( num11 % 5 ) * 6 ), 0x4c, 6 ) );
										}
									}
									else if( !configIni.bLeft.Guitar )
									{
										Rectangle rectangle = new Rectangle( 0x10c, num11 * 6, 0x18, 6 );
										if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1fd, num10 - 3, rectangle );
										}
										rectangle.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x217, num10 - 3, rectangle );
										}
										rectangle.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x231, num10 - 3, rectangle );
										}
									}
									else
									{
										Rectangle rectangle2 = new Rectangle( 0x10c, num11 * 6, 0x18, 6 );
										if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x231, num10 - 3, rectangle2 );
										}
										rectangle2.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x217, num10 - 3, rectangle2 );
										}
										rectangle2.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1fd, num10 - 3, rectangle2 );
										}
									}
								}
							}
							if( ( configIni.bAutoPlay.Guitar && !pChip.bHit ) && ( pChip.nバーからの距離dot.Guitar < 0 ) )
							{
								pChip.bHit = true;
								if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) || ( pChip.nチャンネル番号 == 0x20 ) )
								{
									this.actChipFireGB.Start( 0 );
								}
								if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) || ( pChip.nチャンネル番号 == 0x20 ) )
								{
									this.actChipFireGB.Start( 1 );
								}
								if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) || ( pChip.nチャンネル番号 == 0x20 ) )
								{
									this.actChipFireGB.Start( 2 );
								}
								this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.GUITAR, dTX.nモニタを考慮した音量( E楽器パート.GUITAR ) );
								this.r次にくるギターChip = null;
								this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
							}
							break;
						}
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
						{
							pChip.bHit = true;
							this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.GUITAR, dTX.nモニタを考慮した音量( E楽器パート.GUITAR ) );
						}
						break;

					case 0x28:	// ウェイリング(ギター)
						if( configIni.bGuitar有効 )
						{
							if( configIni.bSudden.Guitar )
							{
								pChip.b可視 = pChip.nバーからの距離dot.Guitar < 200;
							}
							if( configIni.bHidden.Guitar && ( pChip.nバーからの距離dot.Guitar < 100 ) )
							{
								pChip.b可視 = false;
							}
							if( !pChip.bHit && pChip.b可視 )
							{
								int num14 = 0x19;
								int num15 = configIni.bReverse.Guitar ? ( 0x176 - pChip.nバーからの距離dot.Guitar ) : ( 0x5f + pChip.nバーからの距離dot.Guitar );
								int num16 = num15 - 0x39;
								int num17 = 0;
								if( ( num16 < ( 0x163 + num14 ) ) && ( num16 > -num14 ) )
								{
									int num18 = this.ctWailingチップ模様アニメ.n現在の値;
									Rectangle rectangle5 = new Rectangle( 0x10c + ( num18 * 20 ), 0xae, 20, 50 );
									if( num16 < num14 )
									{
										rectangle5.Y += num14 - num16;
										rectangle5.Height -= num14 - num16;
										num17 = num14 - num16;
									}
									if( num16 > ( 0x163 - num14 ) )
									{
										rectangle5.Height -= num16 - ( 0x163 - num14 );
									}
									if( ( rectangle5.Bottom > rectangle5.Top ) && ( this.txチップ != null ) )
									{
										this.txチップ.t2D描画( CDTXMania.app.Device, 0x24c, ( num15 - num14 ) + num17, rectangle5 );
									}
								}
							}
							if( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
							{
								pChip.bHit = true;
								if( configIni.bAutoPlay.Guitar )
								{
									this.actWailingBonus.Start( E楽器パート.GUITAR, this.r現在の歓声Chip.Guitar );
								}
							}
							break;
						}
						pChip.bHit = true;
						break;

					case 0x2f:	// ウェイリングサウンド(ギター)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の歓声Chip.Guitar = pChip;
						}
						break;

					case 0x31:	// 不可視チップ配置(ドラム)
					case 0x32:
					case 0x33:
					case 0x34:
					case 0x35:
					case 0x36:
					case 0x37:
					case 0x38:
					case 0x39:
					case 0x3a:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;

					case 0x50:	// 小節線
						{
							int num24 = pChip.n発声位置 / 0x180;
							if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
							{
								pChip.bHit = true;
								this.actPlayInfo.n小節番号 = num24 - 1;
								if( configIni.bWave再生位置自動調整機能有効 )
								{
									dTX.tWave再生位置自動補正();
								}
							}
							if( configIni.bDrums有効 )
							{
								if( configIni.b演奏情報を表示する && ( configIni.eDark == Eダークモード.OFF ) )
								{
									int num28 = num24 - 1;
									CDTXMania.act文字コンソール.tPrint( 0x14d, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 0x11 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 0x11 ), C文字コンソール.Eフォント種別.白, num28.ToString() );
								}
								if( ( ( configIni.eDark != Eダークモード.FULL ) && pChip.b可視 ) && ( this.txチップ != null ) )
								{
									this.txチップ.t2D描画( CDTXMania.app.Device, 0x23, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 1 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 1 ), new Rectangle( 0, 0x1bc, 0x128, 2 ) );
								}
							}
							if( ( pChip.b可視 && configIni.bGuitar有効 ) && ( configIni.eDark != Eダークモード.FULL ) )
							{
								int y = configIni.bReverse.Guitar ? ( ( 0x176 - pChip.nバーからの距離dot.Guitar ) - 1 ) : ( ( 0x5f + pChip.nバーからの距離dot.Guitar ) - 1 );
								if( ( dTX.bチップがある.Guitar && ( y > 0x39 ) ) && ( ( y < 0x19c ) && ( this.txチップ != null ) ) )
								{
									this.txチップ.t2D描画( CDTXMania.app.Device, 0x1fb, y, new Rectangle( 0, 450, 0x4e, 1 ) );
								}
								y = configIni.bReverse.Bass ? ( ( 0x176 - pChip.nバーからの距離dot.Bass ) - 1 ) : ( ( 0x5f + pChip.nバーからの距離dot.Bass ) - 1 );
								if( ( dTX.bチップがある.Bass && ( y > 0x39 ) ) && ( ( y < 0x19c ) && ( this.txチップ != null ) ) )
								{
									this.txチップ.t2D描画( CDTXMania.app.Device, 0x18e, y, new Rectangle( 0, 450, 0x4e, 1 ) );
								}
							}
							break;
						}
					case 0x51:	// 拍線
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						if( ( ( configIni.eDark != Eダークモード.FULL ) && pChip.b可視 ) && ( this.txチップ != null ) )
						{
							this.txチップ.t2D描画( CDTXMania.app.Device, 0x23, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 1 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 1 ), new Rectangle( 0, 0x1bf, 0x128, 1 ) );
						}
						break;

					case 0x52:	// MIDIコーラス
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;

					case 0x53:	// フィルイン
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							switch( pChip.n整数値 )
							{
								case 0x01:	// フィルイン開始
									if( configIni.bフィルイン有効 )
									{
										this.bフィルイン中 = true;
									}
									break;

								case 0x02:	// フィルイン終了
									if( configIni.bフィルイン有効 )
									{
										this.bフィルイン中 = false;
									}
									if( ( ( this.actCOMBO.n現在のコンボ数.Drums > 0 ) || configIni.bドラムが全部オートプレイである ) && configIni.b歓声を発声する )
									{
										if( this.r現在の歓声Chip.Drums != null )
										{
											dTX.tチップの再生( this.r現在の歓声Chip.Drums, CDTXMania.Timer.nシステム時刻, 10, dTX.nモニタを考慮した音量( E楽器パート.UNKNOWN ) );
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
									CDTXMania.DTX.bHH演奏で直前のHHを消音する = true;
									break;
								case 0x05:	// HH消音無し
									CDTXMania.DTX.bHH演奏で直前のHHを消音する = false;
									break;
								case 0x06:	// ギター消音あり(従来同等)
									CDTXMania.DTX.bGUITAR演奏で直前のGUITARを消音する = true;
									break;
								case 0x07:	// ギター消音無し
									CDTXMania.DTX.bGUITAR演奏で直前のGUITARを消音する = false;
									break;
								case 0x08:	// ベース消音あり(従来同等)
									CDTXMania.DTX.bBASS演奏で直前のBASSを消音する = true;
									break;
								case 0x09:	// ベース消音無し
									CDTXMania.DTX.bBASS演奏で直前のBASSを消音する = false;
									break;
#endif
							}
						}
						break;

					case 0x54:	// 動画再生
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( configIni.bAVI有効 )
							{
								switch( pChip.eAVI種別 )
								{
									case EAVI種別.AVI:
										if( pChip.rAVI != null )
										{
											this.actAVI.Start( pChip.nチャンネル番号, pChip.rAVI, 0x116, 0x163, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, pChip.n発声時刻ms );
										}
										break;

									case EAVI種別.AVIPAN:
										if( pChip.rAVIPan != null )
										{
											this.actAVI.Start( pChip.nチャンネル番号, pChip.rAVI, pChip.rAVIPan.sz開始サイズ.Width, pChip.rAVIPan.sz開始サイズ.Height, pChip.rAVIPan.sz終了サイズ.Width, pChip.rAVIPan.sz終了サイズ.Height, pChip.rAVIPan.pt動画側開始位置.X, pChip.rAVIPan.pt動画側開始位置.Y, pChip.rAVIPan.pt動画側終了位置.X, pChip.rAVIPan.pt動画側終了位置.Y, pChip.rAVIPan.pt表示側開始位置.X, pChip.rAVIPan.pt表示側開始位置.Y, pChip.rAVIPan.pt表示側終了位置.X, pChip.rAVIPan.pt表示側終了位置.Y, pChip.n総移動時間, pChip.n発声時刻ms );
										}
										break;
								}
							}
						}
						break;

					case 0x61:	// 自動再生(BGM, SE)
					case 0x62:
					case 0x63:
					case 0x64:
					case 0x65:
					case 0x66:
					case 0x67:
					case 0x68:
					case 0x69:
					case 0x70:
					case 0x71:
					case 0x72:
					case 0x73:
					case 0x74:
					case 0x75:
					case 0x76:
					case 0x77:
					case 0x78:
					case 0x79:
					case 0x80:
					case 0x81:
					case 0x82:
					case 0x83:
					case 0x84:
					case 0x85:
					case 0x86:
					case 0x87:
					case 0x88:
					case 0x89:
					case 0x90:
					case 0x91:
					case 0x92:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( configIni.bBGM音を発声する )
							{
								dTX.tWavの再生停止( this.n最後に再生したBGMの実WAV番号[ pChip.nチャンネル番号 - 0x61 ] );
								dTX.tチップの再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, 10, dTX.nモニタを考慮した音量( E楽器パート.UNKNOWN ) );
								this.n最後に再生したBGMの実WAV番号[ pChip.nチャンネル番号 - 0x61 ] = pChip.n整数値・内部番号;
							}
						}
						break;

					case 0xa0:	// ベース演奏
					case 0xa1:
					case 0xa2:
					case 0xa3:
					case 0xa4:
					case 0xa5:
					case 0xa6:
					case 0xa7:
						if( configIni.bGuitar有効 )
						{
							if( configIni.bSudden.Bass )
							{
								pChip.b可視 = pChip.nバーからの距離dot.Bass < 200;
							}
							if( configIni.bHidden.Bass && ( pChip.nバーからの距離dot.Bass < 100 ) )
							{
								pChip.b可視 = false;
							}
							if( !pChip.bHit && pChip.b可視 )
							{
								int num12 = configIni.bReverse.Bass ? ( 0x176 - pChip.nバーからの距離dot.Bass ) : ( 0x5f + pChip.nバーからの距離dot.Bass );
								if( ( num12 > 0x39 ) && ( num12 < 0x19c ) )
								{
									int num13 = this.ctチップ模様アニメ.Bass.n現在の値;
									if( pChip.nチャンネル番号 == 160 )
									{
										if( this.txチップ != null )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 400, num12 - 2, new Rectangle( 0x10c, 0x90 + ( ( num13 % 5 ) * 6 ), 0x4c, 6 ) );
										}
									}
									else if( !configIni.bLeft.Bass )
									{
										Rectangle rectangle3 = new Rectangle( 0x10c, num13 * 6, 0x18, 6 );
										if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 400, num12 - 3, rectangle3 );
										}
										rectangle3.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1aa, num12 - 3, rectangle3 );
										}
										rectangle3.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1c4, num12 - 3, rectangle3 );
										}
									}
									else
									{
										Rectangle rectangle4 = new Rectangle( 0x10c, num13 * 6, 0x18, 6 );
										if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1c4, num12 - 3, rectangle4 );
										}
										rectangle4.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 0x1aa, num12 - 3, rectangle4 );
										}
										rectangle4.X += 0x18;
										if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) && ( this.txチップ != null ) )
										{
											this.txチップ.t2D描画( CDTXMania.app.Device, 400, num12 - 3, rectangle4 );
										}
									}
								}
							}
							if( ( configIni.bAutoPlay.Bass && !pChip.bHit ) && ( pChip.nバーからの距離dot.Bass < 0 ) )
							{
								pChip.bHit = true;
								if( ( ( pChip.nチャンネル番号 & 4 ) != 0 ) || ( pChip.nチャンネル番号 == 160 ) )
								{
									this.actChipFireGB.Start( 3 );
								}
								if( ( ( pChip.nチャンネル番号 & 2 ) != 0 ) || ( pChip.nチャンネル番号 == 160 ) )
								{
									this.actChipFireGB.Start( 4 );
								}
								if( ( ( pChip.nチャンネル番号 & 1 ) != 0 ) || ( pChip.nチャンネル番号 == 160 ) )
								{
									this.actChipFireGB.Start( 5 );
								}
								this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.BASS, dTX.nモニタを考慮した音量( E楽器パート.BASS ) );
								this.r次にくるベースChip = null;
								this.tチップのヒット処理( pChip.n発声時刻ms, pChip );
							}
							break;
						}
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
						{
							this.tサウンド再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.BASS, dTX.nモニタを考慮した音量( E楽器パート.BASS ) );
							pChip.bHit = true;
						}
						break;

					case 0xa8:	// ウェイリング(ベース)
						if( configIni.bGuitar有効 )
						{
							if( configIni.bSudden.Bass )
							{
								pChip.b可視 = pChip.nバーからの距離dot.Bass < 200;
							}
							if( configIni.bHidden.Bass && ( pChip.nバーからの距離dot.Bass < 100 ) )
							{
								pChip.b可視 = false;
							}
							if( !pChip.bHit && pChip.b可視 )
							{
								int num19 = 0x19;
								int num20 = configIni.bReverse.Bass ? ( 0x176 - pChip.nバーからの距離dot.Bass ) : ( 0x5f + pChip.nバーからの距離dot.Bass );
								int num21 = num20 - 0x39;
								int num22 = 0;
								if( ( num21 < ( 0x163 + num19 ) ) && ( num21 > -num19 ) )
								{
									int num23 = this.ctWailingチップ模様アニメ.n現在の値;
									Rectangle rectangle6 = new Rectangle( 0x10c + ( num23 * 20 ), 0xae, 20, 50 );
									if( num21 < num19 )
									{
										rectangle6.Y += num19 - num21;
										rectangle6.Height -= num19 - num21;
										num22 = num19 - num21;
									}
									if( num21 > ( 0x163 - num19 ) )
									{
										rectangle6.Height -= num21 - ( 0x163 - num19 );
									}
									if( ( rectangle6.Bottom > rectangle6.Top ) && ( this.txチップ != null ) )
									{
										this.txチップ.t2D描画( CDTXMania.app.Device, 0x1df, ( num20 - num19 ) + num22, rectangle6 );
									}
								}
							}
							if( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
							{
								pChip.bHit = true;
								if( configIni.bAutoPlay.Bass )
								{
									this.actWailingBonus.Start( E楽器パート.BASS, this.r現在の歓声Chip.Bass );
								}
							}
							break;
						}
						pChip.bHit = true;
						break;

					case 0xaf:	// ウェイリングサウンド(ベース)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の歓声Chip.Bass = pChip;
						}
						break;

					case 0xb1:	// 空打ち音設定(ドラム)
					case 0xb2:
					case 0xb3:
					case 0xb4:
					case 0xb5:
					case 0xb6:
					case 0xb7:
					case 0xb8:
					case 0xb9:
					case 0xbc:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の空うちドラムChip[ (int) this.eチャンネルtoパッド[ pChip.nチャンネル番号 - 0xb1 ] ] = pChip;
							pChip.nチャンネル番号 = ( pChip.nチャンネル番号 != 0xbc ) ? ( ( pChip.nチャンネル番号 - 0xb1 ) + 0x11 ) : 0x1a;
						}
						break;

					case 0xba:	// 空打ち音設定(ギター)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の空うちギターChip = pChip;
							pChip.nチャンネル番号 = 0x20;
						}
						break;

					case 0xbb:	// 空打ち音設定(ベース)
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の空うちベースChip = pChip;
							pChip.nチャンネル番号 = 160;
						}
						break;

					case 0xc4:	// BGA画像入れ替え
					case 0xc7:
					case 0xd5:
					case 0xd6:
					case 0xd7:
					case 0xd8:
					case 0xd9:
					case 0xe0:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if( ( configIni.bBGA有効 && ( pChip.eBGA種別 == EBGA種別.BMP ) ) || ( pChip.eBGA種別 == EBGA種別.BMPTEX ) )
							{
								for( int i = 0; i < 8; i++ )
								{
									if( this.nBGAスコープチャンネルマップ[ 0, i ] == pChip.nチャンネル番号 )
									{
										this.actBGA.ChangeScope( this.nBGAスコープチャンネルマップ[ 1, i ], pChip.rBMP, pChip.rBMPTEX );
									}
								}
							}
						}
						break;

					default:
						if( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;
				}

				num4++;
			}
			return false;
		}
		private void t進行描画・チップアニメ()
		{
			for( int i = 0; i < 3; i++ )
			{
				this.ctチップ模様アニメ[ i ].t進行Loop();
			}
			this.ctWailingチップ模様アニメ.t進行Loop();
		}
		private void t進行描画・チップファイアD()
		{
			this.actChipFireD.On進行描画();
		}
		private void t進行描画・チップファイアGB()
		{
			this.actChipFireGB.On進行描画();
		}
		private void t進行描画・ドラムパッド()
		{
			if( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				this.actPad.On進行描画();
			}
		}
		private void t進行描画・パネル文字列()
		{
			if( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				this.actPanel.t進行描画( 0x150, 0x1ab );
			}
		}
		private bool t進行描画・フェードイン・アウト()
		{
			switch( base.eフェーズID )
			{
				case CStage.Eフェーズ.共通_フェードイン:
					if( this.actFI.On進行描画() != 0 )
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
					break;

				case CStage.Eフェーズ.共通_フェードアウト:
				case CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト:
					if( this.actFO.On進行描画() != 0 )
					{
						return true;
					}
					break;

				case CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト:
					if( this.actFOClear.On進行描画() == 0 )
					{
						break;
					}
					return true;
			}
			return false;
		}
		private void t進行描画・レーンフラッシュD()
		{
			if( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.actLaneFlushD.On進行描画();
			}
		}
		private void t進行描画・レーンフラッシュGB()
		{
			if( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				this.actLaneFlushGB.On進行描画();
			}
		}
		private void t進行描画・演奏情報()
		{
			if( !CDTXMania.ConfigIni.b演奏情報を表示しない )
			{
				this.actPlayInfo.t進行描画( 0x152, 0x39 );
			}
		}
		private void t進行描画・背景()
		{
			if( CDTXMania.ConfigIni.eDark == Eダークモード.OFF )
			{
				if( this.tx背景 != null )
				{
					this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );
				}
			}
			else
			{
				CDTXMania.app.Device.Clear( ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 0f, 0 );
			}
		}
		private void t進行描画・判定ライン()
		{
			if( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				int y = CDTXMania.ConfigIni.bReverse.Drums ? 0x35 : 0x1a3;
				for( int i = 0x20; i < 0x14f; i += 8 )
				{
					if( this.txヒットバー != null )
					{
						this.txヒットバー.t2D描画( CDTXMania.app.Device, i, y, new Rectangle( 0, 0, ( ( i + 8 ) >= 0x14f ) ? ( 7 - ( ( i + 8 ) - 0x14f ) ) : 8, 8 ) );
					}
				}
			}
		}
		private void t進行描画・判定文字列1・通常位置指定の場合()
		{
			if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) != E判定文字表示位置.判定ライン上または横 )
			{
				this.actJudgeString.On進行描画();
			}
		}
		private void t進行描画・判定文字列2・判定ライン上指定の場合()
		{
			if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) == E判定文字表示位置.判定ライン上または横 )
			{
				this.actJudgeString.On進行描画();
			}
		}
		private void t進行描画・譜面スクロール速度()
		{
			this.act譜面スクロール速度.On進行描画();
		}
		private void t入力メソッド記憶( E楽器パート part )
		{
			if( CDTXMania.Pad.st検知したデバイス.Keyboard )
			{
				this.b演奏にキーボードを使った[ (int) part ] = true;
			}
			if( CDTXMania.Pad.st検知したデバイス.Joypad )
			{
				this.b演奏にジョイパッドを使った[ (int) part ] = true;
			}
			if( CDTXMania.Pad.st検知したデバイス.MIDIIN )
			{
				this.b演奏にMIDI入力を使った[ (int) part ] = true;
			}
			if( CDTXMania.Pad.st検知したデバイス.Mouse )
			{
				this.b演奏にマウスを使った[ (int) part ] = true;
			}
		}
		private void t入力処理・ギター()
		{
			if( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Guitar )
			{
				if( CDTXMania.ConfigIni.bAutoPlay.Guitar )
				{
					CDTX.CChip chip = this.r次にくるギターChipを更新して返す();
					if( chip != null )
					{
						if( ( chip.nチャンネル番号 & 4 ) != 0 )
						{
							this.actLaneFlushGB.Start( 0 );
							this.actRGB.Push( 0 );
						}
						if( ( chip.nチャンネル番号 & 2 ) != 0 )
						{
							this.actLaneFlushGB.Start( 1 );
							this.actRGB.Push( 1 );
						}
						if( ( chip.nチャンネル番号 & 1 ) != 0 )
						{
							this.actLaneFlushGB.Start( 2 );
							this.actRGB.Push( 2 );
						}
					}
				}
				else
				{
					if( CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.CY ) && CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.BD ) )
					{
						CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Guitar + 1, 0x7cf );
					}
					if( CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.CY ) && CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.HH ) )
					{
						CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Guitar - 1, 0 );
					}
					int num = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.HH ) ? 4 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int num2 = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.SD ) ? 2 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int num3 = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.BD ) ? 1 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int num4 = ( num | num2 ) | num3;
					if( num != 0 )
					{
						this.actLaneFlushGB.Start( 0 );
						this.actRGB.Push( 0 );
					}
					if( num2 != 0 )
					{
						this.actLaneFlushGB.Start( 1 );
						this.actRGB.Push( 1 );
					}
					if( num3 != 0 )
					{
						this.actLaneFlushGB.Start( 2 );
						this.actRGB.Push( 2 );
					}
					List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.GUITAR, Eパッド.HT );
					if( ( events != null ) && ( events.Count > 0 ) )
					{
						this.t入力メソッド記憶( E楽器パート.GUITAR );
						foreach( STInputEvent event2 in events )
						{
							CDTX.CChip chip4;
							if( !event2.b押された )
							{
								continue;
							}
							long nTime = event2.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							CDTX.CChip pChip = this.r指定時刻に一番近い未ヒットChip( nTime, 0x2f );
							E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nTime, pChip, this.nInputAdjustTimeMs.Guitar );
							if( ( ( pChip != null ) && ( ( pChip.nチャンネル番号 & 15 ) == num4 ) ) && ( e判定 != E判定.Miss ) )
							{
								if( ( num != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 0 );
								}
								if( ( num2 != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 1 );
								}
								if( ( num3 != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 2 );
								}
								this.tチップのヒット処理( nTime, pChip );
								this.tサウンド再生( pChip, CDTXMania.Timer.nシステム時刻, E楽器パート.GUITAR, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Guitar, e判定 == E判定.Poor );
								CDTX.CChip item = this.r指定時刻に一番近い未ヒットChip( nTime, 40, 140 );
								if( item != null )
								{
									this.queWailing.Guitar.Enqueue( item );
								}
								continue;
							}
							if( ( ( chip4 = this.r現在の空うちギターChip ) != null ) || ( ( chip4 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, 0x2f ) ) != null ) )
							{
								this.tサウンド再生( chip4, CDTXMania.Timer.nシステム時刻, E楽器パート.GUITAR, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Guitar, true );
							}
							if( !CDTXMania.ConfigIni.bLight.Guitar )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.GUITAR );
							}
						}
					}
					List<STInputEvent> list2 = CDTXMania.Pad.GetEvents( E楽器パート.GUITAR, Eパッド.LT );
					if( ( list2 != null ) && ( list2.Count > 0 ) )
					{
						foreach( STInputEvent event3 in list2 )
						{
							CDTX.CChip chip5;
							if( !event3.b押された )
							{
								continue;
							}
							long num6 = event3.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							while( ( this.queWailing.Guitar.Count > 0 ) && ( ( chip5 = this.queWailing.Guitar.Dequeue() ) != null ) )
							{
								if( ( num6 - chip5.n発声時刻ms ) <= 800 )
								{
									chip5.bHit = true;
									this.actWailingBonus.Start( E楽器パート.GUITAR, this.r現在の歓声Chip.Guitar );
									if( !CDTXMania.ConfigIni.bAutoPlay.Guitar )
									{
										this.actScore.Set( E楽器パート.GUITAR, this.actScore.Get( E楽器パート.GUITAR ) + ( this.actCOMBO.n現在のコンボ数.Guitar * 0xbb8L ) );
									}
								}
							}
						}
					}
				}
			}
		}
		private void t入力処理・ドラム()
		{
			for( int nPad = 0; nPad < 10; nPad++ )
			{
				List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.DRUMS, (Eパッド) nPad );

				if( ( events != null ) && ( events.Count != 0 ) )
				{
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

					foreach( STInputEvent event2 in events )
					{
						#region [ 変数宣言 ]
						//-----------------------------
						int num3;
						int num4;
						int num5;
						int num6;
						int num7;
						int num8;
						int num9;
						int num10;
						int num11;
						int num12;
						int num13;
						int num14;
						int num15;
						int num16;
						//-----------------------------
						#endregion

						if( !event2.b押された )
							continue;

						long nTime = event2.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
						bool flag = false;


						#region [ switch( ( (Eパッド) nPad ) ) ]
						//-----------------------------
						switch( ( (Eパッド) nPad ) )
						{
							case Eパッド.HH:
								#region [ *** ]
								//-----------------------------
								if( event2.nVelocity <= CDTXMania.ConfigIni.nハイハット切り捨て下限Velocity )
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CDTX.CChip chipHC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x11 );	// HiHat Close
								CDTX.CChip chipHO = this.r指定時刻に一番近い未ヒットChip( nTime, 0x18 );	// HiHat Open
								CDTX.CChip chipLC = this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a );	// LC
								E判定 e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								switch( eHHGroup )
								{
									case EHHGroup.ハイハットのみ打ち分ける:
										#region [ *** ]
										//-----------------------------
										if( ( e判定HC != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											}
											else if( chipHC.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											flag = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.左シンバルのみ打ち分ける:
										#region [ *** ]
										//-----------------------------
										if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											flag = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.全部共通:
										#region [ *** ]
										//-----------------------------
										if( ( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) ) && ( e判定LC != E判定.Miss ) )
										{
											CDTX.CChip chip4;
											CDTX.CChip[] chipArray = new CDTX.CChip[] { chipHC, chipHO, chipLC };
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip4 = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip4;
											}
											if( chipArray[ 0 ].n発声位置 > chipArray[ 1 ].n発声位置 )
											{
												chip4 = chipArray[ 0 ];
												chipArray[ 0 ] = chipArray[ 1 ];
												chipArray[ 1 ] = chip4;
											}
											if( chipArray[ 1 ].n発声位置 > chipArray[ 2 ].n発声位置 )
											{
												chip4 = chipArray[ 1 ];
												chipArray[ 1 ] = chipArray[ 2 ];
												chipArray[ 2 ] = chip4;
											}
											this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 0 ], event2.nVelocity );
											if( chipArray[ 0 ].n発声位置 == chipArray[ 1 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 1 ], event2.nVelocity );
											}
											if( chipArray[ 0 ].n発声位置 == chipArray[ 2 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipArray[ 2 ], event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定HO != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											}
											else if( chipHC.n発声位置 > chipHO.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HC != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHC.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											}
											else if( chipHC.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HO != E判定.Miss ) && ( e判定LC != E判定.Miss ) )
										{
											if( chipHO.n発声位置 < chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											}
											else if( chipHO.n発声位置 > chipLC.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											flag = true;
										}
										else if( e判定HO != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHO, event2.nVelocity );
											flag = true;
										}
										else if( e判定LC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipLC, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;
									//-----------------------------
										#endregion

									default:
										#region [ *** ]
										//-----------------------------
										if( e判定HC != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HH, chipHC, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;
									//-----------------------------
										#endregion
								}
								if( !flag )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.SD:
								#region [ *** ]
								//-----------------------------
								if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								if( !this.tドラムヒット処理(nTime, Eパッド.SD, this.r指定時刻に一番近い未ヒットChip(nTime, 0x12), event2.nVelocity ) )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.BD:
								#region [ *** ]
								//-----------------------------
								if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								if( !this.tドラムヒット処理(nTime, Eパッド.BD, this.r指定時刻に一番近い未ヒットChip(nTime, 0x13), event2.nVelocity ) )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.HT:
								#region [ *** ]
								//-----------------------------
								if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								if( this.tドラムヒット処理( nTime, Eパッド.HT, this.r指定時刻に一番近い未ヒットChip( nTime, 20 ), event2.nVelocity ) )
									continue;
								break;
							//-----------------------------
								#endregion

							case Eパッド.LT:
								#region [ *** ]
								//-----------------------------
								if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipLT = this.r指定時刻に一番近い未ヒットChip(nTime, 0x15);	// LT
								CDTX.CChip chipFT = this.r指定時刻に一番近い未ヒットChip(nTime, 0x17);	// FT
								E判定 e判定LT = (chipLT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定FT = (chipFT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								switch( eFTGroup )
								{
									case EFTGroup.打ち分ける:
										#region [ *** ]
										//-----------------------------
										if( e判定LT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, event2.nVelocity );
											flag = true;
										}
										break;
									//-----------------------------
										#endregion

									case EFTGroup.共通:
										#region [ *** ]
										//-----------------------------
										if( ( e判定LT != E判定.Miss ) && ( e判定FT != E判定.Miss ) )
										{
											if( chipLT.n発声位置 < chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, event2.nVelocity );
											}
											else if( chipLT.n発声位置 > chipFT.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定LT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipLT, event2.nVelocity );
											flag = true;
										}
										else if( e判定FT != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.LT, chipFT, event2.nVelocity );
											flag = true;
										}
										break;
									//-----------------------------
										#endregion
								}
								if( !flag )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.FT:
								#region [ *** ]
								//-----------------------------
								if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CDTX.CChip chipLT_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x15);	// LT
								CDTX.CChip chipFT_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x17);	// FT
								E判定 e判定LT_ = (chipLT_ != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT_, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定FT_ = (chipFT_ != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT_, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								switch( eFTGroup )
								{
									case EFTGroup.打ち分ける:
										#region [ *** ]
										//-----------------------------
										if( e判定FT_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipFT_, event2.nVelocity );
											flag = true;
										}
										//-----------------------------
										#endregion
										break;

									case EFTGroup.共通:
										#region [ *** ]
										//-----------------------------
										if( ( e判定LT_ != E判定.Miss ) && ( e判定FT_ != E判定.Miss ) )
										{
											if( chipLT_.n発声位置 < chipFT_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipLT_, event2.nVelocity );
											}
											else if( chipLT_.n発声位置 > chipFT_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipFT_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.FT, chipLT_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.FT, chipFT_, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定LT_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipLT_, event2.nVelocity );
											flag = true;
										}
										else if( e判定FT_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.FT, chipFT_, event2.nVelocity );
											flag = true;
										}
										//-----------------------------
										#endregion
										break;
								}
								if( !flag )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.CY:
								#region [ *** ]
								//-----------------------------
								{
									if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
										continue;	// 電子ドラムによる意図的なクロストークを無効にする
									CDTX.CChip chipCY = this.r指定時刻に一番近い未ヒットChip(nTime, 0x16);	// CY
									CDTX.CChip chipRD = this.r指定時刻に一番近い未ヒットChip(nTime, 0x19);	// RD
									CDTX.CChip chip21 = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, 0x1a) : null;
									E判定 e判定CY = ( chipCY != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipCY, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定RD = ( chipRD != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipRD, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定19 = ( chip21 != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chip21, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									CDTX.CChip[] chipArray4 = new CDTX.CChip[] { chipCY, chipRD, chip21 };
									E判定[] e判定Array2 = new E判定[] { e判定CY, e判定RD, e判定19 };
									num8 = 0;
									while( num8 < 2 )
									{
										num9 = 2;
										while( num9 > num8 )
										{
											if( ( chipArray4[ num9 - 1 ] == null ) || ( ( chipArray4[ num9 ] != null ) && ( chipArray4[ num9 - 1 ].n発声位置 > chipArray4[ num9 ].n発声位置 ) ) )
											{
												CDTX.CChip chip22 = chipArray4[ num9 - 1 ];
												chipArray4[ num9 - 1 ] = chipArray4[ num9 ];
												chipArray4[ num9 ] = chip22;
												E判定 e判定20 = e判定Array2[ num9 - 1 ];
												e判定Array2[ num9 - 1 ] = e判定Array2[ num9 ];
												e判定Array2[ num9 ] = e判定20;
											}
											num9--;
										}
										num8++;
									}
									switch( eCYGroup )
									{
										case ECYGroup.打ち分ける:
											if( !CDTXMania.ConfigIni.bシンバルフリー )
											{
												if( e判定CY != E判定.Miss )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipCY, event2.nVelocity );
													flag = true;
												}
												if( !flag )
													break;
												continue;
											}
											num10 = 0;
											while( num10 < 3 )
											{
												if( ( e判定Array2[ num10 ] != E判定.Miss ) && ( ( chipArray4[ num10 ] == chipCY ) || ( chipArray4[ num10 ] == chip21 ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipArray4[ num10 ], event2.nVelocity );
													flag = true;
													break;
												}
												num10++;
											}
											if( e判定CY != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.CY, chipCY, event2.nVelocity );
												flag = true;
											}
											if( !flag )
												break;
											continue;

										case ECYGroup.共通:
											if( !CDTXMania.ConfigIni.bシンバルフリー )
											{
												num12 = 0;
												while( num12 < 3 )
												{
													if( ( e判定Array2[ num12 ] != E判定.Miss ) && ( ( chipArray4[ num12 ] == chipCY ) || ( chipArray4[ num12 ] == chipRD ) ) )
													{
														this.tドラムヒット処理( nTime, Eパッド.CY, chipArray4[ num12 ], event2.nVelocity );
														flag = true;
														break;
													}
													num12++;
												}
												if( !flag )
													break;
												continue;
											}
											num11 = 0;
											while( num11 < 3 )
											{
												if( e判定Array2[ num11 ] != E判定.Miss )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipArray4[ num11 ], event2.nVelocity );
													flag = true;
													break;
												}
												num11++;
											}
											if( !flag )
												break;
											continue;
									}
									if( !flag )
										break;
									continue;
								}
							//-----------------------------
								#endregion

							case Eパッド.HHO:
								#region [ *** ]
								//-----------------------------
								if( event2.nVelocity <= CDTXMania.ConfigIni.nハイハット切り捨て下限Velocity )
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CDTX.CChip chipHC_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x11);	// HC
								CDTX.CChip chipHO_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x18);	// HO
								CDTX.CChip chipLC_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x1a);	// LC
								E判定 e判定HC_ = (chipHC_ != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC_, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定HO_ = (chipHO_ != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO_, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								E判定 e判定LC_ = (chipLC_ != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC_, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
								switch( eHHGroup )
								{
									case EHHGroup.全部打ち分ける:
										if( e判定HO_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;

									case EHHGroup.ハイハットのみ打ち分ける:
										if( ( e判定HO_ != E判定.Miss ) && ( e判定LC_ != E判定.Miss ) )
										{
											if( chipHO_.n発声位置 < chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											else if( chipHO_.n発声位置 > chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HO_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											flag = true;
										}
										else if( e判定LC_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;

									case EHHGroup.左シンバルのみ打ち分ける:
										if( ( e判定HC_ != E判定.Miss ) && ( e判定HO_ != E判定.Miss ) )
										{
											if( chipHC_.n発声位置 < chipHO_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
											}
											else if( chipHC_.n発声位置 > chipHO_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HC_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
											flag = true;
										}
										else if( e判定HO_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;

									case EHHGroup.全部共通:
										if( ( ( e判定HC_ != E判定.Miss ) && ( e判定HO_ != E判定.Miss ) ) && ( e判定LC_ != E判定.Miss ) )
										{
											CDTX.CChip chip8;
											CDTX.CChip[] chipArray2 = new CDTX.CChip[] { chipHC_, chipHO_, chipLC_ };
											if( chipArray2[ 1 ].n発声位置 > chipArray2[ 2 ].n発声位置 )
											{
												chip8 = chipArray2[ 1 ];
												chipArray2[ 1 ] = chipArray2[ 2 ];
												chipArray2[ 2 ] = chip8;
											}
											if( chipArray2[ 0 ].n発声位置 > chipArray2[ 1 ].n発声位置 )
											{
												chip8 = chipArray2[ 0 ];
												chipArray2[ 0 ] = chipArray2[ 1 ];
												chipArray2[ 1 ] = chip8;
											}
											if( chipArray2[ 1 ].n発声位置 > chipArray2[ 2 ].n発声位置 )
											{
												chip8 = chipArray2[ 1 ];
												chipArray2[ 1 ] = chipArray2[ 2 ];
												chipArray2[ 2 ] = chip8;
											}
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray2[ 0 ], event2.nVelocity );
											if( chipArray2[ 0 ].n発声位置 == chipArray2[ 1 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray2[ 1 ], event2.nVelocity );
											}
											if( chipArray2[ 0 ].n発声位置 == chipArray2[ 2 ].n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipArray2[ 2 ], event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HC_ != E判定.Miss ) && ( e判定HO_ != E判定.Miss ) )
										{
											if( chipHC_.n発声位置 < chipHO_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
											}
											else if( chipHC_.n発声位置 > chipHO_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HC_ != E判定.Miss ) && ( e判定LC_ != E判定.Miss ) )
										{
											if( chipHC_.n発声位置 < chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
											}
											else if( chipHC_.n発声位置 > chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											flag = true;
										}
										else if( ( e判定HO_ != E判定.Miss ) && ( e判定LC_ != E判定.Miss ) )
										{
											if( chipHO_.n発声位置 < chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											}
											else if( chipHO_.n発声位置 > chipLC_.n発声位置 )
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											else
											{
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
												this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											}
											flag = true;
										}
										else if( e判定HC_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHC_, event2.nVelocity );
											flag = true;
										}
										else if( e判定HO_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipHO_, event2.nVelocity );
											flag = true;
										}
										else if( e判定LC_ != E判定.Miss )
										{
											this.tドラムヒット処理( nTime, Eパッド.HHO, chipLC_, event2.nVelocity );
											flag = true;
										}
										if( !flag )
											break;
										continue;
								}
								if( !flag )
									break;
								continue;
							//-----------------------------
								#endregion

							case Eパッド.RD:
								#region [ *** ]
								//-----------------------------
								{
									if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
										continue;	// 電子ドラムによる意図的なクロストークを無効にする
									CDTX.CChip chipCY_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x16);	// CY
									CDTX.CChip chipRD_ = this.r指定時刻に一番近い未ヒットChip(nTime, 0x19);	// RD
									CDTX.CChip pChip = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip( nTime, 0x1a ) : null;
									E判定 e判定CY_ = ( chipCY_ != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipCY_, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定RD_ = ( chipRD_ != null) ? this.e指定時刻からChipのJUDGEを返す( nTime, chipRD_, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定23 = ( pChip != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, pChip, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									CDTX.CChip[] chipArray5 = new CDTX.CChip[] { chipCY_, chipRD_, pChip };
									E判定[] e判定Array3 = new E判定[] { e判定CY_, e判定RD_, e判定23 };
									num13 = 0;
									while( num13 < 2 )
									{
										num14 = 2;
										while( num14 > num13 )
										{
											if( ( chipArray5[ num14 - 1 ] == null ) || ( ( chipArray5[ num14 ] != null ) && ( chipArray5[ num14 - 1 ].n発声位置 > chipArray5[ num14 ].n発声位置 ) ) )
											{
												CDTX.CChip chip26 = chipArray5[ num14 - 1 ];
												chipArray5[ num14 - 1 ] = chipArray5[ num14 ];
												chipArray5[ num14 ] = chip26;
												E判定 e判定24 = e判定Array3[ num14 - 1 ];
												e判定Array3[ num14 - 1 ] = e判定Array3[ num14 ];
												e判定Array3[ num14 ] = e判定24;
											}
											num14--;
										}
										num13++;
									}
									switch( eCYGroup )
									{
										case ECYGroup.打ち分ける:
											if( e判定RD_ != E判定.Miss )
											{
												this.tドラムヒット処理( nTime, Eパッド.RD, chipRD_, event2.nVelocity );
												flag = true;
											}
											break;

										case ECYGroup.共通:
											if( !CDTXMania.ConfigIni.bシンバルフリー )
											{
												num16 = 0;
												while( num16 < 3 )
												{
													if( ( e判定Array3[ num16 ] != E判定.Miss ) && ( ( chipArray5[ num16 ] == chipCY_ ) || ( chipArray5[ num16 ] == chipRD_ ) ) )
													{
														this.tドラムヒット処理( nTime, Eパッド.CY, chipArray5[ num16 ], event2.nVelocity );
														flag = true;
														break;
													}
													num16++;
												}
												break;
											}
											num15 = 0;
											while( num15 < 3 )
											{
												if( e判定Array3[ num15 ] != E判定.Miss )
												{
													this.tドラムヒット処理( nTime, Eパッド.CY, chipArray5[ num15 ], event2.nVelocity );
													flag = true;
													break;
												}
												num15++;
											}
											break;
									}
									if( flag )
									{
										continue;
									}
									break;
								}
							//-----------------------------
								#endregion

							case Eパッド.LC:
								#region [ *** ]
								//-----------------------------
								{
									if (event2.nVelocity <= CDTXMania.ConfigIni.n切り捨て下限Velocity)	// #23857 2010.12.12 yyagi: to support VelocityMin
										continue;	// 電子ドラムによる意図的なクロストークを無効にする
									CDTX.CChip chip9 = this.r指定時刻に一番近い未ヒットChip(nTime, 0x11);	// HC
									CDTX.CChip chip10 = this.r指定時刻に一番近い未ヒットChip(nTime, 0x18);	// HO
									CDTX.CChip chip11 = this.r指定時刻に一番近い未ヒットChip(nTime, 0x1a);	// LC
									CDTX.CChip chip12 = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, 0x16) : null;
									CDTX.CChip chip13 = CDTXMania.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, 0x19) : null;
									E判定 e判定7 = ( chip9 != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chip9, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定8 = ( chip10 != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chip10, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定9 = ( chip11 != null ) ? this.e指定時刻からChipのJUDGEを返す(nTime, chip11, this.nInputAdjustTimeMs.Drums) : E判定.Miss;
									E判定 e判定10 = ( chip12 != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chip12, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									E判定 e判定11 = ( chip13 != null ) ? this.e指定時刻からChipのJUDGEを返す( nTime, chip13, this.nInputAdjustTimeMs.Drums ) : E判定.Miss;
									CDTX.CChip[] chipArray3 = new CDTX.CChip[] { chip9, chip10, chip11, chip12, chip13 };
									E判定[]  e判定Array = new E判定[] { e判定7, e判定8, e判定9, e判定10, e判定11 };
									num3 = 0;
									while( num3 < 4 )
									{
										num4 = 4;
										while( num4 > num3 )
										{
											if( ( chipArray3[ num4 - 1 ] == null ) || ( ( chipArray3[ num4 ] != null ) && ( chipArray3[ num4 - 1 ].n発声位置 > chipArray3[ num4 ].n発声位置 ) ) )
											{
												CDTX.CChip chip14 = chipArray3[ num4 - 1 ];
												chipArray3[ num4 - 1 ] = chipArray3[ num4 ];
												chipArray3[ num4 ] = chip14;
												E判定 e判定12 = e判定Array[ num4 - 1 ];
												e判定Array[ num4 - 1 ] = e判定Array[ num4 ];
												e判定Array[ num4 ] = e判定12;
											}
											num4--;
										}
										num3++;
									}
									switch( eHHGroup )
									{
										case EHHGroup.全部打ち分ける:
										case EHHGroup.左シンバルのみ打ち分ける:
											if( !CDTXMania.ConfigIni.bシンバルフリー )
											{
												if( e判定9 != E判定.Miss )
												{
													this.tドラムヒット処理( nTime, Eパッド.LC, chip11, event2.nVelocity );
													flag = true;
												}
												if( !flag )
													break;
												continue;
											}
											num5 = 0;
											while( num5 < 5 )
											{
												if( ( e判定Array[ num5 ] != E判定.Miss ) && ( ( ( chipArray3[ num5 ] == chip11 ) || ( chipArray3[ num5 ] == chip12 ) ) || ( ( chipArray3[ num5 ] == chip13 ) && ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 ) ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.LC, chipArray3[ num5 ], event2.nVelocity );
													flag = true;
													break;
												}
												num5++;
											}
											if( !flag )
												break;
											continue;

										case EHHGroup.ハイハットのみ打ち分ける:
										case EHHGroup.全部共通:
											if( !CDTXMania.ConfigIni.bシンバルフリー )
											{
												num7 = 0;
												while( num7 < 5 )
												{
													if( ( e判定Array[ num7 ] != E判定.Miss ) && ( ( ( chipArray3[ num7 ] == chip11 ) || ( chipArray3[ num7 ] == chip9 ) ) || ( chipArray3[ num7 ] == chip10 ) ) )
													{
														this.tドラムヒット処理( nTime, Eパッド.LC, chipArray3[ num7 ], event2.nVelocity );
														flag = true;
														break;
													}
													num7++;
												}
												if( !flag )
													break;
												continue;
											}
											num6 = 0;
											while( num6 < 5 )
											{
												if( ( e判定Array[ num6 ] != E判定.Miss ) && ( ( chipArray3[ num6 ] != chip13 ) || ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 ) ) )
												{
													this.tドラムヒット処理( nTime, Eパッド.LC, chipArray3[ num6 ], event2.nVelocity );
													flag = true;
													break;
												}
												num6++;
											}
											if( !flag )
												break;
											continue;
									}
									if( !flag )
										break;

									break;
								}
							//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion

						#region [ *** ]
						//-----------------------------
						this.actLaneFlushD.Start( (Eドラムレーン) this.nパッド0Atoレーン07[ nPad ], ( (float) event2.nVelocity ) / 127f );
						this.actPad.Hit( this.nパッド0Atoパッド08[ nPad ] );
						if( !CDTXMania.ConfigIni.bドラム打音を発声する )
						{
							if( CDTXMania.ConfigIni.bTight )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.DRUMS, this.nパッド0Atoレーン07[ nPad ] );
							}
							continue;
						}
						CDTX.CChip rChip = null;
						rChip = this.r空うちChip( E楽器パート.DRUMS, (Eパッド) nPad );
						if( rChip != null )
						{
							this.tサウンド再生( rChip, CDTXMania.Timer.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );

							if( CDTXMania.ConfigIni.bTight )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.DRUMS, this.nパッド0Atoレーン07[ nPad ] );
							}
							continue;
						}
						CDTX.CChip chip28 = null;
						CDTX.CChip chip29 = null;
						CDTX.CChip chip30 = null;
						CDTX.CChip chip31 = null;
						CDTX.CChip chip32 = null;
						CDTX.CChip chip33 = null;
						CDTX.CChip chip34 = null;
						//-----------------------------
						#endregion

						#region [ switch( ( (Eパッド) i ) ) ]
						//-----------------------------
						switch( ( (Eパッド) nPad ) )
						{
							case Eパッド.HH:
								#region [ *** ]
								//-----------------------------
								chip28 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[0]);
								chip29 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[7]);
								chip30 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[9]);
								switch( CDTXMania.ConfigIni.eHHGroup )
								{
									case EHHGroup.ハイハットのみ打ち分ける:
										rChip = ( chip28 != null ) ? chip28 : chip30;
										break;

									case EHHGroup.左シンバルのみ打ち分ける:
										rChip = ( chip28 != null ) ? chip28 : chip29;
										break;

									case EHHGroup.全部共通:
										if( chip28 != null )
										{
											rChip = chip28;
										}
										else if( chip29 == null )
										{
											rChip = chip30;
										}
										else if( chip30 == null )
										{
											rChip = chip29;
										}
										else if( chip29.n発声位置 < chip30.n発声位置 )
										{
											rChip = chip29;
										}
										else
										{
											rChip = chip30;
										}
										break;

									default:
										rChip = chip28;
										break;
								}
								//-----------------------------
								#endregion
								break;

							case Eパッド.LT:
								#region [ *** ]
								//-----------------------------
								chip31 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 4 ] );
								chip32 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 5 ] );
								if( CDTXMania.ConfigIni.eFTGroup != EFTGroup.打ち分ける )
									rChip = ( chip31 != null ) ? chip31 : chip32;
								else
									rChip = chip31;
								//-----------------------------
								#endregion
								break;

							case Eパッド.FT:
								#region [ *** ]
								//-----------------------------
								chip31 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 4 ] );
								chip32 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 5 ] );
								if( CDTXMania.ConfigIni.eFTGroup != EFTGroup.打ち分ける )
									rChip = ( chip32 != null ) ? chip32 : chip31;
								else
									rChip = chip32;
								//-----------------------------
								#endregion
								break;

							case Eパッド.CY:
								#region [ *** ]
								//-----------------------------
								chip33 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 6 ] );
								chip34 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 8 ] );
								if( CDTXMania.ConfigIni.eCYGroup != ECYGroup.打ち分ける )
									rChip = ( chip33 != null ) ? chip33 : chip34;
								else
									rChip = chip33;
								//-----------------------------
								#endregion
								break;

							case Eパッド.HHO:
								#region [ *** ]
								//-----------------------------
								chip28 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 0 ] );
								chip29 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 7 ] );
								chip30 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 9 ] );
								switch( CDTXMania.ConfigIni.eHHGroup )
								{
									case EHHGroup.全部打ち分ける:
										rChip = chip29;
										break;

									case EHHGroup.ハイハットのみ打ち分ける:
										rChip = ( chip29 != null ) ? chip29 : chip30;
										break;

									case EHHGroup.左シンバルのみ打ち分ける:
										rChip = ( chip29 != null ) ? chip29 : chip28;
										break;

									case EHHGroup.全部共通:
										if( chip29 != null )
										{
											rChip = chip29;
										}
										else if( chip28 == null )
										{
											rChip = chip30;
										}
										else if( chip30 == null )
										{
											rChip = chip28;
										}
										else if( chip28.n発声位置 < chip30.n発声位置 )
										{
											rChip = chip28;
										}
										else
										{
											rChip = chip30;
										}
										break;
								}
								//-----------------------------
								#endregion
								break;

							case Eパッド.RD:
								#region [ *** ]
								//-----------------------------
								chip33 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 6 ] );
								chip34 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 8 ] );
								if( CDTXMania.ConfigIni.eCYGroup != ECYGroup.打ち分ける )
									rChip = ( chip34 != null ) ? chip34 : chip33;
								else
									rChip = chip34;
								//-----------------------------
								#endregion
								break;

							case Eパッド.LC:
								#region [ *** ]
								//-----------------------------
								chip28 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 0 ] );
								chip29 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 7 ] );
								chip30 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ 9 ] );
								switch( CDTXMania.ConfigIni.eHHGroup )
								{
									case EHHGroup.全部打ち分ける:
									case EHHGroup.左シンバルのみ打ち分ける:
										rChip = chip30;
										break;

									case EHHGroup.ハイハットのみ打ち分ける:
									case EHHGroup.全部共通:
										if( chip30 != null )
										{
											rChip = chip30;
										}
										else if( chip28 == null )
										{
											rChip = chip29;
										}
										else if( chip29 == null )
										{
											rChip = chip28;
										}
										else if( chip28.n発声位置 < chip29.n発声位置 )
										{
											rChip = chip28;
										}
										else
										{
											rChip = chip29;
										}
										break;
								}
								//-----------------------------
								#endregion
								break;

							default:
								#region [ *** ]
								//-----------------------------
								rChip = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, this.nパッド0Atoチャンネル0A[ nPad ] );
								//-----------------------------
								#endregion
								break;
						}
						//-----------------------------
						#endregion

						#region [ *** ]
						//-----------------------------
						if( rChip != null )
						{
							this.tサウンド再生( rChip, CDTXMania.Timer.nシステム時刻, E楽器パート.DRUMS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Drums );
						}
						if( CDTXMania.ConfigIni.bTight )
						{
							this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.DRUMS, this.nパッド0Atoレーン07[ nPad ] );
						}
						//-----------------------------
						#endregion
						continue;
					}
				}
			}
		}
		private void t入力処理・ベース()
		{
			if( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Bass )
			{
				if( CDTXMania.ConfigIni.bAutoPlay.Bass )
				{
					CDTX.CChip chip = this.r次にくるベースChipを更新して返す();
					if( chip != null )
					{
						if( ( chip.nチャンネル番号 & 4 ) != 0 )
						{
							this.actLaneFlushGB.Start( 3 );
							this.actRGB.Push( 3 );
						}
						if( ( chip.nチャンネル番号 & 2 ) != 0 )
						{
							this.actLaneFlushGB.Start( 4 );
							this.actRGB.Push( 4 );
						}
						if( ( chip.nチャンネル番号 & 1 ) != 0 )
						{
							this.actLaneFlushGB.Start( 5 );
							this.actRGB.Push( 5 );
						}
					}
				}
				else
				{
					if( CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.CY ) && CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.BD ) )
					{
						CDTXMania.ConfigIni.n譜面スクロール速度.Bass = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Bass + 1, 0x7cf );
					}
					if( CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.CY ) && CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.HH ) )
					{
						CDTXMania.ConfigIni.n譜面スクロール速度.Bass = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Bass - 1, 0 );
					}
					int num = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.HH ) ? 4 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int num2 = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.SD ) ? 2 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int num3 = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.BD ) ? 1 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int num4 = ( num | num2 ) | num3;
					if( num != 0 )
					{
						this.actLaneFlushGB.Start( 3 );
						this.actRGB.Push( 3 );
					}
					if( num2 != 0 )
					{
						this.actLaneFlushGB.Start( 4 );
						this.actRGB.Push( 4 );
					}
					if( num3 != 0 )
					{
						this.actLaneFlushGB.Start( 5 );
						this.actRGB.Push( 5 );
					}
					List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.BASS, Eパッド.HT );
					if( ( events != null ) && ( events.Count > 0 ) )
					{
						foreach( STInputEvent event2 in events )
						{
							CDTX.CChip chip4;
							if( !event2.b押された )
							{
								continue;
							}
							this.t入力メソッド記憶( E楽器パート.BASS );
							long nTime = event2.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							CDTX.CChip pChip = this.r指定時刻に一番近い未ヒットChip( nTime, 0xaf );
							E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nTime, pChip, this.nInputAdjustTimeMs.Bass );
							if( ( ( pChip != null ) && ( ( pChip.nチャンネル番号 & 15 ) == num4 ) ) && ( e判定 != E判定.Miss ) )
							{
								if( ( num != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 3 );
								}
								if( ( num2 != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 4 );
								}
								if( ( num3 != 0 ) || ( num4 == 0 ) )
								{
									this.actChipFireGB.Start( 5 );
								}
								this.tチップのヒット処理( nTime, pChip );
								this.tサウンド再生( pChip, CDTXMania.Timer.nシステム時刻, E楽器パート.BASS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Bass, e判定 == E判定.Poor );
								CDTX.CChip item = this.r指定時刻に一番近い未ヒットChip( nTime, 0xa8, 140 );
								if( item != null )
								{
									this.queWailing.Bass.Enqueue( item );
								}
								continue;
							}
							if( ( ( chip4 = this.r現在の空うちベースChip ) != null ) || ( ( chip4 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, 0xaf ) ) != null ) )
							{
								this.tサウンド再生( chip4, CDTXMania.Timer.nシステム時刻, E楽器パート.BASS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Bass, true );
							}
							if( !CDTXMania.ConfigIni.bLight.Bass )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.BASS );
							}
						}
					}
					List<STInputEvent> list2 = CDTXMania.Pad.GetEvents( E楽器パート.BASS, Eパッド.LT );
					if( ( list2 != null ) && ( list2.Count > 0 ) )
					{
						foreach( STInputEvent event3 in list2 )
						{
							CDTX.CChip chip5;
							if( !event3.b押された )
							{
								continue;
							}
							long num6 = event3.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							while( ( this.queWailing.Bass.Count > 0 ) && ( ( chip5 = this.queWailing.Bass.Dequeue() ) != null ) )
							{
								if( ( num6 - chip5.n発声時刻ms ) <= 800 )
								{
									chip5.bHit = true;
									this.actWailingBonus.Start( E楽器パート.BASS, this.r現在の歓声Chip.Bass );
									if( !CDTXMania.ConfigIni.bAutoPlay.Bass )
									{
										this.actScore.Set( E楽器パート.BASS, this.actScore.Get( E楽器パート.BASS ) + ( this.actCOMBO.n現在のコンボ数.Bass * 0xbb8L ) );
									}
								}
							}
						}
					}
				}
			}
		}
		private void t背景テクスチャの生成()
		{
			Bitmap image = null;
			string path = "";
			bool flag = true;
			if( ( ( CDTXMania.DTX.BACKGROUND != null ) && ( CDTXMania.DTX.BACKGROUND.Length > 0 ) ) && !CDTXMania.ConfigIni.bストイックモード )
			{
				path = CDTXMania.DTX.strフォルダ名 + CDTXMania.DTX.BACKGROUND;
				if( File.Exists( path ) )
				{
					try
					{
						Bitmap bitmap2 = null;
						bitmap2 = new Bitmap( path );
						if( ( bitmap2.Size.Width == 0 ) && ( bitmap2.Size.Height == 0 ) )
						{
							this.tx背景 = null;
							return;
						}
						Bitmap bitmap3 = new Bitmap( 640, 480 );
						Graphics graphics = Graphics.FromImage( bitmap3 );
						for( int i = 0; i < 480; i += bitmap2.Size.Height )
						{
							for( int j = 0; j < 640; j += bitmap2.Size.Width )
							{
								graphics.DrawImage( bitmap2, j, i, bitmap2.Width, bitmap2.Height );
							}
						}
						graphics.Dispose();
						bitmap2.Dispose();
						image = new Bitmap( CSkin.Path( @"Graphics\ScreenPlayDrums background.jpg" ) );
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
						graphics.DrawImage( bitmap3, new Rectangle( 0x152, 0x39, 0x116, 0x163 ), 0x152, 0x39, 0x116, 0x163, GraphicsUnit.Pixel );
						graphics.Dispose();
						bitmap3.Dispose();
						flag = false;
					}
					catch
					{
						Trace.TraceError( "背景画像の読み込みに失敗しました。({0})", new object[] { path } );
					}
				}
			}
			if( flag )
			{
				path = CSkin.Path( @"Graphics\ScreenPlayDrums background.jpg" );
				try
				{
					image = new Bitmap( path );
				}
				catch
				{
					Trace.TraceError( "背景画像の読み込みに失敗しました。({0})", new object[] { path } );
					this.tx背景 = null;
					return;
				}
			}
			if( ( CDTXMania.DTX.listBMP.Count > 0 ) || ( CDTXMania.DTX.listBMPTEX.Count > 0 ) )
			{
				Graphics graphics2 = Graphics.FromImage( image );
				graphics2.FillRectangle( Brushes.Black, 0x152, 0x39, 0x116, 0x163 );
				graphics2.Dispose();
			}
			try
			{
				this.tx背景 = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
			}
			catch( CTextureCreateFailedException )
			{
				Trace.TraceError( "背景テクスチャの生成に失敗しました。" );
				this.tx背景 = null;
			}
			image.Dispose();
		}


#if DAMAGELEVELTUNING
		// ----------------------------------
		public float[,] fDamageGaugeDelta = {			// #23625 2011.1.10 ickw_284: tuned damege/recover factors
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
#endif

		private void t判定にあわせてゲージを増減する( E楽器パート part, E判定 e今回の判定 )
		{
			double fDamage;

#if DAMAGELEVELTUNING
			switch (e今回の判定)
			{
				case E判定.Perfect:
				case E判定.Great:
				case E判定.Good:
				case E判定.Poor:
					fDamage = fDamageGaugeDelta[(int)e今回の判定, (int)part];
					break;
				case E判定.Miss:
					fDamage = fDamageGaugeDelta[(int)e今回の判定, (int)part];
					switch (CDTXMania.ConfigIni.eダメージレベル)
					{
						case Eダメージレベル.少ない:
						case Eダメージレベル.普通:
						case Eダメージレベル.大きい:
							fDamage *= fDamageLevelFactor[(int)CDTXMania.ConfigIni.eダメージレベル];
							break;
					}
					break;

				default:
					fDamage = 0.0f;
					break;
			}
#else
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
			this.actGauge.db現在のゲージ値 += fDamage;

			if( this.actGauge.db現在のゲージ値 > 1.0 )
				this.actGauge.db現在のゲージ値 = 1.0;
		}
		//-----------------
		#endregion
	}
}
