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

		// メソッド

		public void t演奏結果を格納する・ドラム( out CScoreIni.C演奏記録 Drums )
		{
			Drums = new CScoreIni.C演奏記録();

			if ( CDTXMania.DTX.bチップがある.Drums && !CDTXMania.ConfigIni.bギタレボモード )
			{
				Drums.nスコア = this.actScore.Get( E楽器パート.DRUMS );
				Drums.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Drums, CDTXMania.DTX.n可視チップ数.Drums, this.nヒット数・Auto含まない.Drums.Perfect, this.actCombo.n現在のコンボ数.Drums最高値 );
				Drums.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Drums, this.nヒット数・Auto含まない.Drums.Perfect, this.nヒット数・Auto含まない.Drums.Great, this.nヒット数・Auto含まない.Drums.Good, this.nヒット数・Auto含まない.Drums.Poor, this.nヒット数・Auto含まない.Drums.Miss );
				Drums.nPerfect数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Perfect : this.nヒット数・Auto含まない.Drums.Perfect;
				Drums.nGreat数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Great : this.nヒット数・Auto含まない.Drums.Great;
				Drums.nGood数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Good : this.nヒット数・Auto含まない.Drums.Good;
				Drums.nPoor数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Poor : this.nヒット数・Auto含まない.Drums.Poor;
				Drums.nMiss数 = CDTXMania.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数・Auto含む.Drums.Miss : this.nヒット数・Auto含まない.Drums.Miss;
				Drums.nPerfect数・Auto含まない = this.nヒット数・Auto含まない.Drums.Perfect;
				Drums.nGreat数・Auto含まない = this.nヒット数・Auto含まない.Drums.Great;
				Drums.nGood数・Auto含まない = this.nヒット数・Auto含まない.Drums.Good;
				Drums.nPoor数・Auto含まない = this.nヒット数・Auto含まない.Drums.Poor;
				Drums.nMiss数・Auto含まない = this.nヒット数・Auto含まない.Drums.Miss;
				Drums.n最大コンボ数 = this.actCombo.n現在のコンボ数.Drums最高値;
				Drums.n全チップ数 = CDTXMania.DTX.n可視チップ数.Drums;
				for ( int i = 0; i < 10; i++ )
				{
					Drums.bAutoPlay[ i ] = CDTXMania.ConfigIni.bAutoPlay[ i ];
				}
				Drums.bTight = CDTXMania.ConfigIni.bTight;
				for ( int i = 0; i < 3; i++ )
				{
					Drums.bSudden[ i ] = CDTXMania.ConfigIni.bSudden[ i ];
					Drums.bHidden[ i ] = CDTXMania.ConfigIni.bHidden[ i ];
					Drums.bReverse[ i ] = CDTXMania.ConfigIni.bReverse[ i ];
					Drums.eRandom[ i ] = CDTXMania.ConfigIni.eRandom[ i ];
					Drums.bLight[ i ] = CDTXMania.ConfigIni.bLight[ i ];
					Drums.bLeft[ i ] = CDTXMania.ConfigIni.bLeft[ i ];
					Drums.f譜面スクロール速度[ i ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ i ] + 1 ) ) * 0.5f;
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
		}
		public void t演奏結果を格納する・ギター( out CScoreIni.C演奏記録 Guitar )
		{
			Guitar = new CScoreIni.C演奏記録();

			if ( CDTXMania.DTX.bチップがある.Guitar )
			{
				Guitar.nスコア = this.actScore.Get( E楽器パート.GUITAR );
				Guitar.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Guitar, CDTXMania.DTX.n可視チップ数.Guitar, this.nヒット数・Auto含まない.Guitar.Perfect, this.actCombo.n現在のコンボ数.Guitar最高値 );
				Guitar.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Guitar, this.nヒット数・Auto含まない.Guitar.Perfect, this.nヒット数・Auto含まない.Guitar.Great, this.nヒット数・Auto含まない.Guitar.Good, this.nヒット数・Auto含まない.Guitar.Poor, this.nヒット数・Auto含まない.Guitar.Miss );
				Guitar.nPerfect数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Perfect : this.nヒット数・Auto含まない.Guitar.Perfect;
				Guitar.nGreat数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Great : this.nヒット数・Auto含まない.Guitar.Great;
				Guitar.nGood数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Good : this.nヒット数・Auto含まない.Guitar.Good;
				Guitar.nPoor数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Poor : this.nヒット数・Auto含まない.Guitar.Poor;
				Guitar.nMiss数 = CDTXMania.ConfigIni.bAutoPlay.Guitar ? this.nヒット数・Auto含む.Guitar.Miss : this.nヒット数・Auto含まない.Guitar.Miss;
				Guitar.nPerfect数・Auto含まない = this.nヒット数・Auto含まない.Guitar.Perfect;
				Guitar.nGreat数・Auto含まない = this.nヒット数・Auto含まない.Guitar.Great;
				Guitar.nGood数・Auto含まない = this.nヒット数・Auto含まない.Guitar.Good;
				Guitar.nPoor数・Auto含まない = this.nヒット数・Auto含まない.Guitar.Poor;
				Guitar.nMiss数・Auto含まない = this.nヒット数・Auto含まない.Guitar.Miss;
				Guitar.n最大コンボ数 = this.actCombo.n現在のコンボ数.Guitar最高値;
				Guitar.n全チップ数 = CDTXMania.DTX.n可視チップ数.Guitar;
				for ( int i = 0; i < 10; i++ )
				{
					Guitar.bAutoPlay[ i ] = CDTXMania.ConfigIni.bAutoPlay[ i ];
				}
				Guitar.bTight = CDTXMania.ConfigIni.bTight;
				for ( int i = 0; i < 3; i++ )
				{
					Guitar.bSudden[ i ] = CDTXMania.ConfigIni.bSudden[ i ];
					Guitar.bHidden[ i ] = CDTXMania.ConfigIni.bHidden[ i ];
					Guitar.bReverse[ i ] = CDTXMania.ConfigIni.bReverse[ i ];
					Guitar.eRandom[ i ] = CDTXMania.ConfigIni.eRandom[ i ];
					Guitar.bLight[ i ] = CDTXMania.ConfigIni.bLight[ i ];
					Guitar.bLeft[ i ] = CDTXMania.ConfigIni.bLeft[ i ];
					Guitar.f譜面スクロール速度[ i ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ i ] + 1 ) ) * 0.5f;
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
		}
		public void t演奏結果を格納する・ベース( out CScoreIni.C演奏記録 Bass )
		{
			Bass = new CScoreIni.C演奏記録();

			if ( CDTXMania.DTX.bチップがある.Bass )
			{
				Bass.nスコア = this.actScore.Get( E楽器パート.BASS );
				Bass.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す( CDTXMania.DTX.LEVEL.Bass, CDTXMania.DTX.n可視チップ数.Bass, this.nヒット数・Auto含まない.Bass.Perfect, this.actCombo.n現在のコンボ数.Bass最高値 );
				Bass.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す( CDTXMania.DTX.n可視チップ数.Bass, this.nヒット数・Auto含まない.Bass.Perfect, this.nヒット数・Auto含まない.Bass.Great, this.nヒット数・Auto含まない.Bass.Good, this.nヒット数・Auto含まない.Bass.Poor, this.nヒット数・Auto含まない.Bass.Miss );
				Bass.nPerfect数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Perfect : this.nヒット数・Auto含まない.Bass.Perfect;
				Bass.nGreat数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Great : this.nヒット数・Auto含まない.Bass.Great;
				Bass.nGood数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Good : this.nヒット数・Auto含まない.Bass.Good;
				Bass.nPoor数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Poor : this.nヒット数・Auto含まない.Bass.Poor;
				Bass.nMiss数 = CDTXMania.ConfigIni.bAutoPlay.Bass ? this.nヒット数・Auto含む.Bass.Miss : this.nヒット数・Auto含まない.Bass.Miss;
				Bass.nPerfect数・Auto含まない = this.nヒット数・Auto含まない.Bass.Perfect;
				Bass.nGreat数・Auto含まない = this.nヒット数・Auto含まない.Bass.Great;
				Bass.nGood数・Auto含まない = this.nヒット数・Auto含まない.Bass.Good;
				Bass.nPoor数・Auto含まない = this.nヒット数・Auto含まない.Bass.Poor;
				Bass.nMiss数・Auto含まない = this.nヒット数・Auto含まない.Bass.Miss;
				Bass.n最大コンボ数 = this.actCombo.n現在のコンボ数.Bass最高値;
				Bass.n全チップ数 = CDTXMania.DTX.n可視チップ数.Bass;
				for ( int i = 0; i < 10; i++ )
				{
					Bass.bAutoPlay[ i ] = CDTXMania.ConfigIni.bAutoPlay[ i ];
				}
				Bass.bTight = CDTXMania.ConfigIni.bTight;
				for ( int i = 0; i < 3; i++ )
				{
					Bass.bSudden[ i ] = CDTXMania.ConfigIni.bSudden[ i ];
					Bass.bHidden[ i ] = CDTXMania.ConfigIni.bHidden[ i ];
					Bass.bReverse[ i ] = CDTXMania.ConfigIni.bReverse[ i ];
					Bass.eRandom[ i ] = CDTXMania.ConfigIni.eRandom[ i ];
					Bass.bLight[ i ] = CDTXMania.ConfigIni.bLight[ i ];
					Bass.bLeft[ i ] = CDTXMania.ConfigIni.bLeft[ i ];
					Bass.f譜面スクロール速度[ i ] = ( (float) ( CDTXMania.ConfigIni.n譜面スクロール速度[ i ] + 1 ) ) * 0.5f;
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
				Bass.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Bass;			// #24280 2011.1.29 yyagi
				Bass.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Bass;				//
				Bass.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Bass;		//
				Bass.b演奏にマウスを使用した = this.b演奏にマウスを使った.Bass;					//
				Bass.nPerfectになる範囲ms = CDTXMania.nPerfect範囲ms;
				Bass.nGreatになる範囲ms = CDTXMania.nGreat範囲ms;
				Bass.nGoodになる範囲ms = CDTXMania.nGood範囲ms;
				Bass.nPoorになる範囲ms = CDTXMania.nPoor範囲ms;
				Bass.strDTXManiaのバージョン = CDTXMania.VERSION;
				Bass.最終更新日時 = DateTime.Now.ToString();
				Bass.Hash = CScoreIni.t演奏セクションのMD5を求めて返す( Bass );
			}
		}

		// CStage 実装

		public override void On活性化()
		{
			this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.継続;
			this.n現在のトップChip = ( CDTXMania.DTX.listChip.Count > 0 ) ? 0 : -1;
			this.L最後に再生したHHの実WAV番号 = new List<int>( 16 );
			this.n最後に再生したHHのチャンネル番号 = 0;
			this.n最後に再生したギターの実WAV番号 = -1;
			this.n最後に再生したベースの実WAV番号 = -1;
			for ( int i = 0; i < 50; i++ )
			{
				this.n最後に再生したBGMの実WAV番号[ i ] = -1;
			}
			this.r次にくるギターChip = null;
			this.r次にくるベースChip = null;
			for ( int j = 0; j < 10; j++ )
			{
				this.r現在の空うちドラムChip[ j ] = null;
			}
			this.r現在の空うちギターChip = null;
			this.r現在の空うちベースChip = null;
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

			if ( CDTXMania.ConfigIni.bIsSwappedGuitarBass )	// #24063 2011.1.24 yyagi Gt/Bsの譜面情報入れ替え
			{
				CDTXMania.DTX.SwapGuitarBassInfos();
			}
		}
		public override void On非活性化()
		{
			this.L最後に再生したHHの実WAV番号.Clear();	// #23921 2011.1.4 yyagi
			this.L最後に再生したHHの実WAV番号 = null;	//
			for ( int i = 0; i < 3; i++ )
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

		protected CAct演奏チップファイアGB actChipFireGB;
		protected CAct演奏Combo共通 actCombo;
		protected CAct演奏Danger共通 actDANGER;
		protected CActFIFOBlack actFI;
		protected CActFIFOBlack actFO;
		protected CActFIFOWhite actFOClear;
		protected CAct演奏ゲージ共通 actGauge;

		protected CAct演奏判定文字列共通 actJudgeString;
		protected CAct演奏DrumsレーンフラッシュD actLaneFlushD;
		protected CAct演奏レーンフラッシュGB共通 actLaneFlushGB;
		protected CAct演奏パネル文字列 actPanel;
		protected CAct演奏演奏情報 actPlayInfo;
		protected CAct演奏RGB共通 actRGB;
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
		protected int[] n最後に再生したBGMの実WAV番号 = new int[ 50 ];
		protected int n最後に再生したHHのチャンネル番号;
		protected List<int> L最後に再生したHHの実WAV番号;		// #23921 2011.1.4 yyagi: change "int" to "List<int>", for recording multiple wav No.
		protected int n最後に再生したギターの実WAV番号;
		protected int n最後に再生したベースの実WAV番号;
	
		protected STDGBVALUE<Queue<CDTX.CChip>> queWailing;
		protected STDGBVALUE<CDTX.CChip> r現在の歓声Chip;
		protected CDTX.CChip r現在の空うちギターChip;
		protected STKARAUCHI r現在の空うちドラムChip;
		protected CDTX.CChip r現在の空うちベースChip;
		protected CDTX.CChip r次にくるギターChip;
		protected CDTX.CChip r次にくるベースChip;
		protected CTexture txWailing枠;
		protected CTexture txチップ;
		protected CTexture txヒットバー;

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
		protected CDTX.CChip r空うちChip( E楽器パート part, Eパッド pad )
		{
			switch ( part )
			{
				case E楽器パート.DRUMS:
					switch ( pad )
					{
						case Eパッド.HH:
							if ( this.r現在の空うちドラムChip.HH != null )
							{
								return this.r現在の空うちドラムChip.HH;
							}
							if ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける )
							{
								if ( CDTXMania.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける )
								{
									return this.r現在の空うちドラムChip.HHO;
								}
								if ( this.r現在の空うちドラムChip.HHO != null )
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
							if ( this.r現在の空うちドラムChip.LT != null )
							{
								return this.r現在の空うちドラムChip.LT;
							}
							if ( CDTXMania.ConfigIni.eFTGroup == EFTGroup.共通 )
							{
								return this.r現在の空うちドラムChip.FT;
							}
							return null;

						case Eパッド.FT:
							if ( this.r現在の空うちドラムChip.FT != null )
							{
								return this.r現在の空うちドラムChip.FT;
							}
							if ( CDTXMania.ConfigIni.eFTGroup == EFTGroup.共通 )
							{
								return this.r現在の空うちドラムChip.LT;
							}
							return null;

						case Eパッド.CY:
							if ( this.r現在の空うちドラムChip.CY != null )
							{
								return this.r現在の空うちドラムChip.CY;
							}
							if ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 )
							{
								return this.r現在の空うちドラムChip.RD;
							}
							return null;

						case Eパッド.HHO:
							if ( this.r現在の空うちドラムChip.HHO != null )
							{
								return this.r現在の空うちドラムChip.HHO;
							}
							if ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける )
							{
								if ( CDTXMania.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける )
								{
									return this.r現在の空うちドラムChip.HH;
								}
								if ( this.r現在の空うちドラムChip.HH != null )
								{
									return this.r現在の空うちドラムChip.HH;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case Eパッド.RD:
							if ( this.r現在の空うちドラムChip.RD != null )
							{
								return this.r現在の空うちドラムChip.RD;
							}
							if ( CDTXMania.ConfigIni.eCYGroup == ECYGroup.共通 )
							{
								return this.r現在の空うちドラムChip.CY;
							}
							return null;

						case Eパッド.LC:
							if ( this.r現在の空うちドラムChip.LC != null )
							{
								return this.r現在の空うちドラムChip.LC;
							}
							if ( ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける ) && ( CDTXMania.ConfigIni.eHHGroup != EHHGroup.全部共通 ) )
							{
								return null;
							}
							if ( this.r現在の空うちドラムChip.HH != null )
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
		protected void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, CDTXMania.ConfigIni.n手動再生音量, false, false );
		}
		protected void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量 )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, n音量, false, false );
		}
		protected void tサウンド再生( CDTX.CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ )
		{
			this.tサウンド再生( rChip, n再生開始システム時刻ms, part, n音量, bモニタ, false );
		}
		protected void tサウンド再生( CDTX.CChip pChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ, bool b音程をずらして再生 )
		{
			if ( pChip != null )
			{
				switch ( part )
				{
					case E楽器パート.DRUMS:
						{
							int index = pChip.nチャンネル番号;
							if ( ( index >= 0x11 ) && ( index <= 0x1a ) )
							{
								index -= 0x11;
							}
							else
							{
								if ( ( index < 0x31 ) || ( index > 0x3a ) )
								{
									return;
								}
								index -= 0x31;
							}
							int nLane = this.nチャンネル0Atoレーン07[ index ];
							if ( ( nLane == 1 ) &&	// 今回演奏するのがHC or HO
								( index == 0 || ( index == 7 && this.n最後に再生したHHのチャンネル番号 != 0x18 && this.n最後に再生したHHのチャンネル番号 != 0x38 ) )
								// HCを演奏するか、またはHO演奏＆以前HO演奏でない＆以前不可視HO演奏でない
							)
							// #24772 2011.4.4 yyagi
							// == HH mute condition == 
							//			current HH		So, the mute logics are:
							//				HC	HO		1) All played HC/HOs should be queueing
							// last HH	HC  Yes	Yes		2) If you aren't in "both current/last HH are HO", queued HH should be muted.
							//			HO	Yes	No
							{
								// #23921 2011.1.4 yyagi: 2種類以上のオープンハイハットが発音済みだと、最後のHHOしか消せない問題に対応。
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
								if (CDTXMania.DTX.bHH演奏で直前のHHを消音する)
								{
#endif
								for ( int i = 0; i < this.L最後に再生したHHの実WAV番号.Count; i++ )		// #23921 2011.1.4 yyagi
								{
									// CDTXMania.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号);
									CDTXMania.DTX.tWavの再生停止( this.L最後に再生したHHの実WAV番号[ i ] );	// #23921 yyagi ストック分全て消音する
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
							if ( index == 0 || index == 7 || index == 0x20 || index == 0x27 )			// #23921 HOまたは不可視HO演奏時はそのチップ番号をストックしておく
							{																			// #24772 HC, 不可視HCも消音キューに追加
								if ( this.L最後に再生したHHの実WAV番号.Count >= 16 )	// #23921 ただしストック数が16以上になるようなら、頭の1個を削って常に16未満に抑える
								{													// (ストックが増えてList<>のrealloc()が発生するのを予防する)
									this.L最後に再生したHHの実WAV番号.RemoveAt( 0 );
								}
								if ( this.L最後に再生したHHの実WAV番号.IndexOf( pChip.n整数値・内部番号 ) < 0 )	// チップ音がまだストックされてなければ
								{
									this.L最後に再生したHHの実WAV番号.Add( pChip.n整数値・内部番号 );			// ストックする
								}
							}
#if TEST_NOTEOFFMODE	// 2011.1.4 yyagi test
							}
#endif
Debug.WriteLine( "at " + pChip.n発声時刻ms + " : " + pChip.n整数値 + ":発音しました" );
							CDTXMania.DTX.tチップの再生( pChip, n再生開始システム時刻ms, nLane, n音量, bモニタ );
							return;
						}
					case E楽器パート.GUITAR:
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
						if (CDTXMania.DTX.bGUITAR演奏で直前のGUITARを消音する) {
#endif
						CDTXMania.DTX.tWavの再生停止( this.n最後に再生したギターの実WAV番号 );
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.DTX.tチップの再生( pChip, n再生開始システム時刻ms, 8, n音量, bモニタ, b音程をずらして再生 );
						this.n最後に再生したギターの実WAV番号 = pChip.n整数値・内部番号;
						return;

					case E楽器パート.BASS:
#if TEST_NOTEOFFMODE
						if (CDTXMania.DTX.bBASS演奏で直前のBASSを消音する) {
#endif
						CDTXMania.DTX.tWavの再生停止( this.n最後に再生したベースの実WAV番号 );
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.DTX.tチップの再生( pChip, n再生開始システム時刻ms, 9, n音量, bモニタ, b音程をずらして再生 );
						this.n最後に再生したベースの実WAV番号 = pChip.n整数値・内部番号;
						return;
				}
			}
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
		protected abstract E判定 tチップのヒット処理( long nHitTime, CDTX.CChip pChip );
		protected E判定 tチップのヒット処理( long nHitTime, CDTX.CChip pChip, E楽器パート screenmode )		// E楽器パート screenmode
		{
			pChip.bHit = true;
			bool bPChipIsAutoPlay = false;
			if ( (
					( ( pChip.e楽器パート == E楽器パート.DRUMS ) && CDTXMania.ConfigIni.bAutoPlay[ this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ] ] ) ||
					( ( pChip.e楽器パート == E楽器パート.GUITAR ) && CDTXMania.ConfigIni.bAutoPlay.Guitar )
				) ||
					( ( pChip.e楽器パート == E楽器パート.BASS ) && CDTXMania.ConfigIni.bAutoPlay.Bass )
			  )
			{
				bPChipIsAutoPlay = true;
			}
			else
			{
				this.bAUTOでないチップが１つでもバーを通過した = true;
			}
			E判定 eJudgeResult = E判定.Auto;
			switch ( pChip.e楽器パート )
			{
				case E楽器パート.DRUMS:
					{
						int nInputAdjustTime = bPChipIsAutoPlay ? 0 : this.nInputAdjustTimeMs.Drums;
						eJudgeResult = this.e指定時刻からChipのJUDGEを返す( nHitTime, pChip, nInputAdjustTime );
						this.actJudgeString.Start( this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ], bPChipIsAutoPlay ? E判定.Auto : eJudgeResult, pChip.nLag );
					}
					break;

				case E楽器パート.GUITAR:
					{
						int nInputAdjustTime = bPChipIsAutoPlay ? 0 : this.nInputAdjustTimeMs.Guitar;
						eJudgeResult = this.e指定時刻からChipのJUDGEを返す( nHitTime, pChip, nInputAdjustTime );
						this.actJudgeString.Start( 10, bPChipIsAutoPlay ? E判定.Auto : eJudgeResult, pChip.nLag );
						break;
					}

				case E楽器パート.BASS:
					{
						int nInputAdjustTime = bPChipIsAutoPlay ? 0 : this.nInputAdjustTimeMs.Bass;
						eJudgeResult = this.e指定時刻からChipのJUDGEを返す( nHitTime, pChip, nInputAdjustTime );
						this.actJudgeString.Start( 11, bPChipIsAutoPlay ? E判定.Auto : eJudgeResult, pChip.nLag );
					}
					break;
			}
			if ( !bPChipIsAutoPlay && ( pChip.e楽器パート != E楽器パート.UNKNOWN ) )
			{
				this.t判定にあわせてゲージを増減する( screenmode, pChip.e楽器パート, eJudgeResult );
			}

			switch ( pChip.e楽器パート )
			{
				case E楽器パート.DRUMS:
					switch ( eJudgeResult )
					{
						case E判定.Miss:
						case E判定.Bad:
							this.nヒット数・Auto含む.Drums.Miss++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Drums.Miss++;
							}
							break;
						default:
							this.nヒット数・Auto含む.Drums[ (int) eJudgeResult ]++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Drums[ (int) eJudgeResult ]++;
							}
							break;
					}

					if ( CDTXMania.ConfigIni.bドラムが全部オートプレイである || !bPChipIsAutoPlay )
					{
						switch ( eJudgeResult )
						{
							case E判定.Perfect:
							case E判定.Great:
							case E判定.Good:
								this.actCombo.n現在のコンボ数.Drums++;
								break;

							default:
								this.actCombo.n現在のコンボ数.Drums = 0;
								break;
						}
					}
					break;

				case E楽器パート.GUITAR:
					switch ( eJudgeResult )
					{
						case E判定.Miss:
						case E判定.Bad:
							this.nヒット数・Auto含む.Guitar.Miss++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Guitar.Miss++;
							}
							break;
						default:	// #24068 2011.1.10 ikanick changed
							// #24167 2011.1.16 yyagi changed
							this.nヒット数・Auto含む.Guitar[ (int) eJudgeResult ]++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Guitar[ (int) eJudgeResult ]++;
							}
							break;
					}
					switch ( eJudgeResult )
					{
						case E判定.Perfect:
						case E判定.Great:
						case E判定.Good:
							this.actCombo.n現在のコンボ数.Guitar++;
							break;

						default:
							this.actCombo.n現在のコンボ数.Guitar = 0;
							break;
					}
					break;

				case E楽器パート.BASS:
					switch ( eJudgeResult )
					{
						case E判定.Miss:
						case E判定.Bad:
							this.nヒット数・Auto含む.Bass.Miss++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Bass.Miss++;
							}
							break;
						default:	// #24068 2011.1.10 ikanick changed
							this.nヒット数・Auto含む.Bass[ (int) eJudgeResult ]++;
							if ( !bPChipIsAutoPlay )
							{
								this.nヒット数・Auto含まない.Bass[ (int) eJudgeResult ]++;
							}
							break;
					}

					switch ( eJudgeResult )
					{
						case E判定.Perfect:
						case E判定.Great:
						case E判定.Good:
							this.actCombo.n現在のコンボ数.Bass++;
							break;

						default:
							this.actCombo.n現在のコンボ数.Bass = 0;
							break;
					}
					break;

				default:
					break;
			}
			if ( ( !bPChipIsAutoPlay && ( pChip.e楽器パート != E楽器パート.UNKNOWN ) ) && ( eJudgeResult != E判定.Miss ) && ( eJudgeResult != E判定.Bad ) )
			{
				int nCombos = 0;
				switch ( pChip.e楽器パート )
				{
					case E楽器パート.DRUMS:
						nCombos = this.actCombo.n現在のコンボ数.Drums;
						break;
					case E楽器パート.GUITAR:
						nCombos = this.actCombo.n現在のコンボ数.Guitar;
						break;
					case E楽器パート.BASS:
						nCombos = this.actCombo.n現在のコンボ数.Bass;
						break;
				}
				long nScore = this.actScore.Get( pChip.e楽器パート );
				long[] numArray = new long[] { 350L, 200L, 50L, 0L };
				if ( ( nCombos <= 500 ) || ( eJudgeResult == E判定.Good ) )
				{
					nScore += numArray[ (int) eJudgeResult ] * nCombos;
				}
				else if ( ( eJudgeResult == E判定.Perfect ) || ( eJudgeResult == E判定.Great ) )
				{
					nScore += numArray[ (int) eJudgeResult ] * 500L;
				}
				this.actScore.Set( pChip.e楽器パート, nScore );
			}
			return eJudgeResult;
		}
		protected abstract void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part );
		protected abstract void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part, int nLane );
		protected void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part, E楽器パート screenmode )
		{
			this.tチップのヒット処理・BadならびにTight時のMiss( part, 0, screenmode );
		}
		protected void tチップのヒット処理・BadならびにTight時のMiss( E楽器パート part, int nLane, E楽器パート screenmode )
		{
			this.bAUTOでないチップが１つでもバーを通過した = true;
			this.t判定にあわせてゲージを増減する( screenmode, part, E判定.Miss );
			switch ( part )
			{
				case E楽器パート.DRUMS:
					if ( ( nLane >= 0 ) && ( nLane <= 7 ) )
					{
						this.actJudgeString.Start( nLane, CDTXMania.ConfigIni.bAutoPlay[ nLane ] ? E判定.Auto : E判定.Miss, 999 );
					}
					this.actCombo.n現在のコンボ数.Drums = 0;
					return;

				case E楽器パート.GUITAR:
					this.actJudgeString.Start( 10, E判定.Bad, 999 );
					this.actCombo.n現在のコンボ数.Guitar = 0;
					return;

				case E楽器パート.BASS:
					this.actJudgeString.Start( 11, E判定.Bad, 999 );
					this.actCombo.n現在のコンボ数.Bass = 0;
					break;

				default:
					return;
			}
		}
	
		protected CDTX.CChip r指定時刻に一番近い未ヒットChip( long nTime, int nChannelFlag, int nInputAdjustTime )
		{
			return this.r指定時刻に一番近い未ヒットChip( nTime, nChannelFlag, nInputAdjustTime, 0 );
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
//		protected abstract void t入力処理・ギター();
//		protected abstract void t入力処理・ベース();
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
		protected abstract void t進行描画・DANGER();
		protected void t進行描画・MIDIBGM()
		{
			if ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED )
			{
				CStage.Eフェーズ eフェーズid1 = base.eフェーズID;
			}
		}
		protected void t進行描画・RGBボタン()
		{
			if ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				this.actRGB.On進行描画();
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


		protected void t進行描画・チップファイアGB()
		{
			this.actChipFireGB.On進行描画();
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
		protected void t進行描画・コンボ()
		{
			this.actCombo.On進行描画();
		}
		protected void t進行描画・スコア()
		{
			this.actScore.On進行描画();
		}
		protected void t進行描画・ステータスパネル()
		{
			this.actStatusPanels.On進行描画();
		}
		protected bool t進行描画・チップ( E楽器パート ePlayMode )
		{
			if ( ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED ) || ( base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) )
			{
				return true;
			}
			if ( ( this.n現在のトップChip == -1 ) || ( this.n現在のトップChip >= CDTXMania.DTX.listChip.Count ) )
			{
				return true;
			}
			int nCurrentTopChip = this.n現在のトップChip;
			if ( nCurrentTopChip == -1 )
			{
				return true;
			}

			//double speed = 264.0;	// BPM150の時の1小節の長さ[dot]
			double speed = 234.0;	// BPM150の時の1小節の長さ[dot]

			double ScrollSpeedDrums = ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Drums + 1.0 ) * 0.5 * 37.5 * speed / 60000.0;
			double ScrollSpeedGuitar = ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Guitar + 1.0 ) * 0.5 * 0.5 * 37.5 * speed / 60000.0;
			double ScrollSpeedBass = ( this.act譜面スクロール速度.db現在の譜面スクロール速度.Bass + 1.0 ) * 0.5 * 0.5 * 37.5 * speed / 60000.0;

			CDTX dTX = CDTXMania.DTX;
			CConfigIni configIni = CDTXMania.ConfigIni;
			while ( nCurrentTopChip < dTX.listChip.Count )
			{
				CDTX.CChip pChip = dTX.listChip[ nCurrentTopChip ];
				pChip.nバーからの距離dot.Drums = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * ScrollSpeedDrums );
				pChip.nバーからの距離dot.Guitar = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * ScrollSpeedGuitar );
				pChip.nバーからの距離dot.Bass = (int) ( ( pChip.n発声時刻ms - CDTXMania.Timer.n現在時刻 ) * ScrollSpeedBass );
				if ( Math.Min( Math.Min( pChip.nバーからの距離dot.Drums, pChip.nバーからの距離dot.Guitar ), pChip.nバーからの距離dot.Bass ) > 450 )
				{
					break;
				}
				if ( ( ( nCurrentTopChip == this.n現在のトップChip ) && ( pChip.nバーからの距離dot.Drums < -65 ) ) && pChip.bHit )
				{
					this.n現在のトップChip++;
					nCurrentTopChip = this.n現在のトップChip;
					continue;
				}

				bool bPChipIsAutoPlay = false;
				if ( (
						( ( pChip.e楽器パート == E楽器パート.DRUMS ) && CDTXMania.ConfigIni.bAutoPlay[ this.nチャンネル0Atoレーン07[ pChip.nチャンネル番号 - 0x11 ] ] ) ||
						( ( pChip.e楽器パート == E楽器パート.GUITAR ) && CDTXMania.ConfigIni.bAutoPlay.Guitar )
					) ||
						( ( pChip.e楽器パート == E楽器パート.BASS ) && CDTXMania.ConfigIni.bAutoPlay.Bass )
				  )
				//				if ((pChip.e楽器パート == E楽器パート.DRUMS) && CDTXMania.ConfigIni.bAutoPlay[this.nチャンネル0Atoレーン07[pChip.nチャンネル番号 - 0x11]])
				{
					bPChipIsAutoPlay = true;
				}

				int nInputAdjustTime = 0;
				if ( bPChipIsAutoPlay )
				{
					//nInputAdjustTime = 0;
				}
				else if ( pChip.e楽器パート == E楽器パート.UNKNOWN )
				{
					//nInputAdjustTime = 0;
				}
				else
				{
					nInputAdjustTime = this.nInputAdjustTimeMs[ (int) pChip.e楽器パート ];
				}

				if ( ( ( pChip.e楽器パート != E楽器パート.UNKNOWN ) && !pChip.bHit ) &&
					( ( pChip.nバーからの距離dot.Drums < 0 ) && ( this.e指定時刻からChipのJUDGEを返す( CDTXMania.Timer.n現在時刻, pChip, nInputAdjustTime ) == E判定.Miss ) ) )
				{
					this.tチップのヒット処理( CDTXMania.Timer.n現在時刻, pChip );
				}
				switch ( pChip.nチャンネル番号 )
				{
					case 0x01:	// BGM
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( configIni.bBGM音を発声する )
							{
								dTX.tチップの再生( pChip, CDTXMania.Timer.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, 10, dTX.nモニタを考慮した音量( E楽器パート.UNKNOWN ) );
							}
						}
						break;

					case 0x03:	// BPM変更
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
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
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( configIni.bBGA有効 )
							{
								switch ( pChip.eBGA種別 )
								{
									case EBGA種別.BMPTEX:
										if ( pChip.rBMPTEX != null )
										{
											this.actBGA.Start( pChip.nチャンネル番号, null, pChip.rBMPTEX, pChip.rBMPTEX.tx画像.sz画像サイズ.Width, pChip.rBMPTEX.tx画像.sz画像サイズ.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
										}
										break;

									case EBGA種別.BGA:
										if ( ( pChip.rBGA != null ) && ( ( pChip.rBMP != null ) || ( pChip.rBMPTEX != null ) ) )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, pChip.rBMPTEX, pChip.rBGA.pt画像側右下座標.X - pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側右下座標.Y - pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt表示座標.X, pChip.rBGA.pt表示座標.Y, 0, 0, 0 );
										}
										break;

									case EBGA種別.BGAPAN:
										if ( ( pChip.rBGAPan != null ) && ( ( pChip.rBMP != null ) || ( pChip.rBMPTEX != null ) ) )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, pChip.rBMPTEX, pChip.rBGAPan.sz開始サイズ.Width, pChip.rBGAPan.sz開始サイズ.Height, pChip.rBGAPan.sz終了サイズ.Width, pChip.rBGAPan.sz終了サイズ.Height, pChip.rBGAPan.pt画像側開始位置.X, pChip.rBGAPan.pt画像側開始位置.Y, pChip.rBGAPan.pt画像側終了位置.X, pChip.rBGAPan.pt画像側終了位置.Y, pChip.rBGAPan.pt表示側開始位置.X, pChip.rBGAPan.pt表示側開始位置.Y, pChip.rBGAPan.pt表示側終了位置.X, pChip.rBGAPan.pt表示側終了位置.Y, pChip.n総移動時間 );
										}
										break;

									default:
										if ( pChip.rBMP != null )
										{
											this.actBGA.Start( pChip.nチャンネル番号, pChip.rBMP, null, pChip.rBMP.n幅, pChip.rBMP.n高さ, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
										}
										break;
								}
							}
						}
						break;

					case 0x08:	// BPM変更(拡張)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( dTX.listBPM.ContainsKey( pChip.n整数値・内部番号 ) )
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
						this.t進行描画・チップ・ドラムス( configIni, ref dTX, ref pChip );
						break;

					case 0x1f:	// フィルインサウンド(ドラム)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
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
						this.t進行描画・チップ・ギター( configIni, ref dTX, ref pChip );
						break;

					case 0x28:	// ウェイリング(ギター)
						this.t進行描画・チップ・ギター・ウェイリング( configIni, ref dTX, ref pChip );
						break;

					case 0x2f:	// ウェイリングサウンド(ギター)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
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
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;

					case 0x50:	// 小節線
						{
							this.t進行描画・チップ・小節線( configIni, ref dTX, ref pChip );
							break;
						}
					case 0x51:	// 拍線
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						if ( ( ePlayMode == E楽器パート.DRUMS ) && ( configIni.eDark != Eダークモード.FULL ) && pChip.b可視 && ( this.txチップ != null ) )
						{
							this.txチップ.t2D描画( CDTXMania.app.Device, 0x23, configIni.bReverse.Drums ? ( ( 0x38 + pChip.nバーからの距離dot.Drums ) - 1 ) : ( ( 0x1a6 - pChip.nバーからの距離dot.Drums ) - 1 ), new Rectangle( 0, 0x1bf, 0x128, 1 ) );
						}
						break;

					case 0x52:	// MIDIコーラス
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;

					case 0x53:	// フィルイン
						this.t進行描画・チップ・フィルイン( configIni, ref dTX, ref pChip );
						break;

					case 0x54:	// 動画再生
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( configIni.bAVI有効 )
							{
								switch ( pChip.eAVI種別 )
								{
									case EAVI種別.AVI:
										if ( pChip.rAVI != null )
										{
											this.actAVI.Start( pChip.nチャンネル番号, pChip.rAVI, 0x116, 0x163, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, pChip.n発声時刻ms );
										}
										break;

									case EAVI種別.AVIPAN:
										if ( pChip.rAVIPan != null )
										{
											this.actAVI.Start( pChip.nチャンネル番号, pChip.rAVI, pChip.rAVIPan.sz開始サイズ.Width, pChip.rAVIPan.sz開始サイズ.Height, pChip.rAVIPan.sz終了サイズ.Width, pChip.rAVIPan.sz終了サイズ.Height, pChip.rAVIPan.pt動画側開始位置.X, pChip.rAVIPan.pt動画側開始位置.Y, pChip.rAVIPan.pt動画側終了位置.X, pChip.rAVIPan.pt動画側終了位置.Y, pChip.rAVIPan.pt表示側開始位置.X, pChip.rAVIPan.pt表示側開始位置.Y, pChip.rAVIPan.pt表示側終了位置.X, pChip.rAVIPan.pt表示側終了位置.Y, pChip.n総移動時間, pChip.n発声時刻ms );
										}
										break;
								}
							}
						}
						break;

					case 0x61:
					case 0x62:
					case 0x63:
					case 0x64:	// 自動再生(BGM, SE)
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
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( configIni.bBGM音を発声する )
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
						this.t進行描画・チップ・ベース( configIni, ref dTX, ref pChip );
						break;

					case 0xa8:	// ウェイリング(ベース)
						this.t進行描画・チップ・ベース・ウェイリング( configIni, ref dTX, ref pChip );
						break;

					case 0xaf:	// ウェイリングサウンド(ベース)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
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
						this.t進行描画・チップ・空打ち音設定・ドラム( configIni, ref dTX, ref pChip );
						break;

					case 0xba:	// 空打ち音設定(ギター)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Guitar < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の空うちギターChip = pChip;
							pChip.nチャンネル番号 = 0x20;
						}
						break;

					case 0xbb:	// 空打ち音設定(ベース)
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Bass < 0 ) )
						{
							pChip.bHit = true;
							this.r現在の空うちベースChip = pChip;
							pChip.nチャンネル番号 = 0xA0;
						}
						break;

					case 0xc4:
					case 0xc7:
					case 0xd5:
					case 0xd6:	// BGA画像入れ替え
					case 0xd7:
					case 0xd8:
					case 0xd9:
					case 0xe0:
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
							if ( ( configIni.bBGA有効 && ( pChip.eBGA種別 == EBGA種別.BMP ) ) || ( pChip.eBGA種別 == EBGA種別.BMPTEX ) )
							{
								for ( int i = 0; i < 8; i++ )
								{
									if ( this.nBGAスコープチャンネルマップ[ 0, i ] == pChip.nチャンネル番号 )
									{
										this.actBGA.ChangeScope( this.nBGAスコープチャンネルマップ[ 1, i ], pChip.rBMP, pChip.rBMPTEX );
									}
								}
							}
						}
						break;

					default:
						if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
						{
							pChip.bHit = true;
						}
						break;
				}

				nCurrentTopChip++;
			}
			return false;
		}
		protected abstract void t進行描画・チップ・ドラムス( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・ギター( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・ギター・ウェイリング( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・フィルイン( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・小節線( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・ベース( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・ベース・ウェイリング( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected abstract void t進行描画・チップ・空打ち音設定・ドラム( CConfigIni configIni, ref CDTX dTX, ref CDTX.CChip pChip );
		protected void t進行描画・チップアニメ()
		{
			for ( int i = 0; i < 3; i++ )			// drums: 0-3 gt/bs: 1-3
			{
				if ( this.ctチップ模様アニメ[ i ] != null )
				{
					this.ctチップ模様アニメ[ i ].t進行Loop();
				}
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
		protected void t進行描画・レーンフラッシュD()
		{
			if ( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && ( ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED ) && ( base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト ) ) )
			{
				this.actLaneFlushD.On進行描画();
			}
		}
		protected void t進行描画・レーンフラッシュGB()
		{
			if ( ( ( CDTXMania.ConfigIni.eDark != Eダークモード.HALF ) && ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL ) ) && CDTXMania.ConfigIni.bGuitar有効 )
			{
				this.actLaneFlushGB.On進行描画();
			}
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

		protected void t進行描画・判定ライン()
		{
			if ( CDTXMania.ConfigIni.eDark != Eダークモード.FULL )
			{
				int y = CDTXMania.ConfigIni.bReverse.Drums ? 0x35 : 0x1a3;
				for ( int i = 0x20; i < 0x14f; i += 8 )
				{
					if ( this.txヒットバー != null )
					{
						this.txヒットバー.t2D描画( CDTXMania.app.Device, i, y, new Rectangle( 0, 0, ( ( i + 8 ) >= 0x14f ) ? ( 7 - ( ( i + 8 ) - 0x14f ) ) : 8, 8 ) );
					}
				}
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

		protected abstract void t背景テクスチャの生成();
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

		protected virtual void t入力処理・ギター()
		{
			if ( CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.Decide ) && CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.B ) )
			{
				CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Guitar + 1, 0x7cf );
			}
			if ( CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.Decide ) && CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.R ) )
			{
				CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Guitar - 1, 0 );
			}

			if ( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Guitar )
			{
				if ( bIsAutoPlay.Guitar )
				{
					CDTX.CChip chip = this.r次にくるギターChipを更新して返す();
					if ( chip != null )
					{
						if ( ( chip.nチャンネル番号 & 4 ) != 0 )
						{
							this.actLaneFlushGB.Start( 0 );
							this.actRGB.Push( 0 );
						}
						if ( ( chip.nチャンネル番号 & 2 ) != 0 )
						{
							this.actLaneFlushGB.Start( 1 );
							this.actRGB.Push( 1 );
						}
						if ( ( chip.nチャンネル番号 & 1 ) != 0 )
						{
							this.actLaneFlushGB.Start( 2 );
							this.actRGB.Push( 2 );
						}
					}
				}
				else
				{
					int flagR = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.R ) ? 4 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int flagG = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.G ) ? 2 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int flagB = CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.B ) ? 1 : 0;
					this.t入力メソッド記憶( E楽器パート.GUITAR );
					int flagRGB = flagR | flagG | flagB;
					if ( flagR != 0 )
					{
						this.actLaneFlushGB.Start( 0 );
						this.actRGB.Push( 0 );
					}
					if ( flagG != 0 )
					{
						this.actLaneFlushGB.Start( 1 );
						this.actRGB.Push( 1 );
					}
					if ( flagB != 0 )
					{
						this.actLaneFlushGB.Start( 2 );
						this.actRGB.Push( 2 );
					}
					List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.GUITAR, Eパッド.Pick );
					if ( ( events != null ) && ( events.Count > 0 ) )
					{
						foreach ( STInputEvent event2 in events )
						{
							CDTX.CChip chip4;
							if ( !event2.b押された )
							{
								continue;
							}
							this.t入力メソッド記憶( E楽器パート.GUITAR );
							long nTime = event2.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							CDTX.CChip pChip = this.r指定時刻に一番近い未ヒットChip( nTime, 0x2f, this.nInputAdjustTimeMs.Guitar );
							E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nTime, pChip, this.nInputAdjustTimeMs.Guitar );
							if ( ( ( pChip != null ) && ( ( pChip.nチャンネル番号 & 15 ) == flagRGB ) ) && ( e判定 != E判定.Miss ) )
							{
								if ( ( flagR != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 0 );
								}
								if ( ( flagG != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 1 );
								}
								if ( ( flagB != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 2 );
								}
								this.tチップのヒット処理( nTime, pChip );
								this.tサウンド再生( pChip, CDTXMania.Timer.nシステム時刻, E楽器パート.GUITAR, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Guitar, e判定 == E判定.Poor );
								CDTX.CChip item = this.r指定時刻に一番近い未ヒットChip( nTime, 40, this.nInputAdjustTimeMs.Guitar, 140 );
								if ( item != null )
								{
									this.queWailing.Guitar.Enqueue( item );
								}
								continue;
							}
							if ( ( ( chip4 = this.r現在の空うちギターChip ) != null ) || ( ( chip4 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, 0x2f, this.nInputAdjustTimeMs.Guitar ) ) != null ) )
							{
								this.tサウンド再生( chip4, CDTXMania.Timer.nシステム時刻, E楽器パート.GUITAR, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Guitar, true );
							}
							if ( !CDTXMania.ConfigIni.bLight.Guitar )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.GUITAR );
							}
						}
					}
					List<STInputEvent> list2 = CDTXMania.Pad.GetEvents( E楽器パート.GUITAR, Eパッド.Wail );
					if ( ( list2 != null ) && ( list2.Count > 0 ) )
					{
						foreach ( STInputEvent event3 in list2 )
						{
							CDTX.CChip chip5;
							if ( !event3.b押された )
							{
								continue;
							}
							long num6 = event3.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							while ( ( this.queWailing.Guitar.Count > 0 ) && ( ( chip5 = this.queWailing.Guitar.Dequeue() ) != null ) )
							{
								if ( ( num6 - chip5.n発声時刻ms ) <= 1000 )		// #24245 2011.1.26 yyagi: 800 -> 1000
								{
									chip5.bHit = true;
									this.actWailingBonus.Start( E楽器パート.GUITAR, this.r現在の歓声Chip.Guitar );
									if ( !bIsAutoPlay.Guitar )
									{
										int nCombo = ( this.actCombo.n現在のコンボ数.Guitar < 500 ) ? this.actCombo.n現在のコンボ数.Guitar : 500;
										this.actScore.Set( E楽器パート.GUITAR, this.actScore.Get( E楽器パート.GUITAR ) + ( nCombo * 3000L ) );		// #24245 2011.1.26 yyagi changed DRUMS->GUITAR, add nCombo conditions
									}
								}
							}
						}
					}
				}
			}
		}
		protected virtual void t入力処理・ベース()
		{
			if ( CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.Decide ) && CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.B ) )
			{
				CDTXMania.ConfigIni.n譜面スクロール速度.Bass = Math.Min( CDTXMania.ConfigIni.n譜面スクロール速度.Bass + 1, 0x7cf );
			}
			if ( CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.Decide ) && CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.R ) )
			{
				CDTXMania.ConfigIni.n譜面スクロール速度.Bass = Math.Max( CDTXMania.ConfigIni.n譜面スクロール速度.Bass - 1, 0 );
			}

			if ( CDTXMania.ConfigIni.bGuitar有効 && CDTXMania.DTX.bチップがある.Bass )
			{
				if ( bIsAutoPlay.Bass )
				{
					CDTX.CChip chip = this.r次にくるベースChipを更新して返す();
					if ( chip != null )
					{
						if ( ( chip.nチャンネル番号 & 4 ) != 0 )
						{
							this.actLaneFlushGB.Start( 3 );
							this.actRGB.Push( 3 );
						}
						if ( ( chip.nチャンネル番号 & 2 ) != 0 )
						{
							this.actLaneFlushGB.Start( 4 );
							this.actRGB.Push( 4 );
						}
						if ( ( chip.nチャンネル番号 & 1 ) != 0 )
						{
							this.actLaneFlushGB.Start( 5 );
							this.actRGB.Push( 5 );
						}
					}
				}
				else
				{
					int flagR = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.R ) ? 4 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int flagG = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.G ) ? 2 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int flagB = CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.B ) ? 1 : 0;
					this.t入力メソッド記憶( E楽器パート.BASS );
					int flagRGB = flagR | flagG | flagB;
					if ( flagR != 0 )
					{
						this.actLaneFlushGB.Start( 3 );
						this.actRGB.Push( 3 );
					}
					if ( flagG != 0 )
					{
						this.actLaneFlushGB.Start( 4 );
						this.actRGB.Push( 4 );
					}
					if ( flagB != 0 )
					{
						this.actLaneFlushGB.Start( 5 );
						this.actRGB.Push( 5 );
					}
					List<STInputEvent> events = CDTXMania.Pad.GetEvents( E楽器パート.BASS, Eパッド.Pick );
					if ( ( events != null ) && ( events.Count > 0 ) )
					{
						foreach ( STInputEvent event2 in events )
						{
							CDTX.CChip chip4;
							if ( !event2.b押された )
							{
								continue;
							}
							this.t入力メソッド記憶( E楽器パート.BASS );
							long nTime = event2.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							CDTX.CChip pChip = this.r指定時刻に一番近い未ヒットChip( nTime, 0xaf, this.nInputAdjustTimeMs.Bass );
							E判定 e判定 = this.e指定時刻からChipのJUDGEを返す( nTime, pChip, this.nInputAdjustTimeMs.Bass );
							if ( ( ( pChip != null ) && ( ( pChip.nチャンネル番号 & 15 ) == flagRGB ) ) && ( e判定 != E判定.Miss ) )
							{
								if ( ( flagR != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 3 );
								}
								if ( ( flagG != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 4 );
								}
								if ( ( flagB != 0 ) || ( flagRGB == 0 ) )
								{
									this.actChipFireGB.Start( 5 );
								}
								this.tチップのヒット処理( nTime, pChip );
								this.tサウンド再生( pChip, CDTXMania.Timer.nシステム時刻, E楽器パート.BASS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Bass, e判定 == E判定.Poor );
								CDTX.CChip item = this.r指定時刻に一番近い未ヒットChip( nTime, 0xa8, this.nInputAdjustTimeMs.Bass, 140 );
								if ( item != null )
								{
									this.queWailing.Bass.Enqueue( item );
								}
								continue;
							}
							if ( ( ( chip4 = this.r現在の空うちベースChip ) != null ) || ( ( chip4 = this.r指定時刻に一番近いChip・ヒット未済問わず不可視考慮( nTime, 0xaf, this.nInputAdjustTimeMs.Bass ) ) != null ) )
							{
								this.tサウンド再生( chip4, CDTXMania.Timer.nシステム時刻, E楽器パート.BASS, CDTXMania.ConfigIni.n手動再生音量, CDTXMania.ConfigIni.b演奏音を強調する.Bass, true );
							}
							if ( !CDTXMania.ConfigIni.bLight.Bass )
							{
								this.tチップのヒット処理・BadならびにTight時のMiss( E楽器パート.BASS );
							}
						}
					}
					List<STInputEvent> list2 = CDTXMania.Pad.GetEvents( E楽器パート.BASS, Eパッド.Wail );
					if ( ( list2 != null ) && ( list2.Count > 0 ) )
					{
						foreach ( STInputEvent event3 in list2 )
						{
							CDTX.CChip chip5;
							if ( !event3.b押された )
							{
								continue;
							}
							long num6 = event3.nTimeStamp - CDTXMania.Timer.n前回リセットした時のシステム時刻;
							while ( ( this.queWailing.Bass.Count > 0 ) && ( ( chip5 = this.queWailing.Bass.Dequeue() ) != null ) )
							{
								if ( ( num6 - chip5.n発声時刻ms ) <= 1000 )		// #24245 2011.1.26 yyagi: 800 -> 1000
								{
									chip5.bHit = true;
									this.actWailingBonus.Start( E楽器パート.BASS, this.r現在の歓声Chip.Bass );
									if ( !bIsAutoPlay.Bass )
									{
										int nCombo = ( this.actCombo.n現在のコンボ数.Bass < 500 ) ? this.actCombo.n現在のコンボ数.Bass : 500;
										this.actScore.Set( E楽器パート.BASS, this.actScore.Get( E楽器パート.BASS ) + ( nCombo * 3000L ) );		// #24245 2011.1.26 yyagi changed DRUMS->BASS, add nCombo conditions
									}
								}
							}
						}
					}
				}
			}
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
