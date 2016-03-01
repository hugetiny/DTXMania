using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
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
	/// <summary>
	/// 演奏画面の共通クラス (ドラム演奏画面, ギター演奏画面の継承元)
	/// </summary>
	internal partial class CStage演奏画面共通 : CStage
	{
		// members
		#region[Constants]
		private readonly Eパッド[] eチャンネルtoパッド = new Eパッド[]
		{
			Eパッド.HH, Eパッド.SD, Eパッド.BD, Eパッド.HT,
			Eパッド.LT, Eパッド.CY, Eパッド.FT, Eパッド.HHO,
			Eパッド.RD, Eパッド.LC
		};
		private readonly int[] nチャンネル0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 8, 0 };
		public static readonly int[] nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
		private readonly Ech定義[] nパッド0Atoチャンネル0A = new Ech定義[]
		{
			Ech定義.HiHatClose,
			Ech定義.Snare,
			Ech定義.BassDrum,
			Ech定義.HighTom,
			Ech定義.LowTom,
			Ech定義.FloorTom,
			Ech定義.Cymbal,
			Ech定義.HiHatOpen,
			Ech定義.RideCymbal,
			Ech定義.LeftCymbal
		};

		private readonly int[,] nチャンネルtoX座標 = new int[,] {
				{ 76 * 3, 110 * 3, 145 * 3, 192 * 3, 226 * 3, 294 * 3, 260 * 3, 79 * 3, 300 * 3, 35 * 3 },
				// 619 - 35 * 3= 584
				{
				 76 * 3 * 3 / 4 + 548,
				110 * 3 * 3 / 4  + 548,
				145 * 3 * 3 / 4  + 548,
				192 * 3 * 3 / 4  + 548,
				226 * 3 * 3 / 4  + 548,
				294 * 3 * 3 / 4  + 548,
				260 * 3 * 3 / 4  + 548,
				 79 * 3 * 3 / 4  + 548,
				300 * 3 * 3 / 4  + 548,
				 35 * 3 * 3 / 4  + 548
				}
		};
		private readonly int[] nパッド0Atoパッド08 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 8, 0 };	// パッド画像のヒット処理用
		private readonly int[] nパッド0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 6, 7, 1, 7, 0 };
		#endregion

		#region [Member_Activities]
		private CAct演奏AVI actAVI;
		private CAct演奏BGA actBGA;
		private CAct演奏チップファイアGB actChipFireGB;
		private CAct演奏Combo共通 actCombo;
		private CAct演奏Danger共通 actDANGER;
		private CActFIFOBlack actFI;
		private CActFIFOBlack actFO;
		private CActFIFOWhite actFOClear;
		private CAct演奏ゲージ共通 actGauge;
		private CAct演奏判定文字列共通 actJudgeString;
		private CAct演奏DrumsレーンフラッシュD actLaneFlushD;
		private CAct演奏レーンフラッシュGB共通 actLaneFlushGB;
		private CAct演奏パネル文字列 actPanel;
		private CAct演奏演奏情報 actPlayInfo;
		private CAct演奏RGB共通 actRGB;
		private CAct演奏スコア共通 actScore;
		private CAct演奏ステージ失敗 actStageFailed;
		private CAct演奏ステータスパネル共通 actStatusPanels;
		private CAct演奏WailingBonus共通 actWailingBonus;
		private CAct演奏スクロール速度 act譜面スクロール速度;
		private CAct演奏DrumsチップファイアD actChipFireD; // drums only
		private CAct演奏Drumsグラフ actGraph;   // drums only #24074 2011.01.23 add ikanick
		private CAct演奏Drumsパッド actPad; // drums only

		#endregion

		private C演奏判定ライン座標共通 演奏判定ライン座標;// #31602 2013.6.23 yyagi 表示遅延対策として、判定ラインの表示位置をずらす機能を追加する
		private bool bPAUSE;
		private STDGBVALUE<bool> b演奏にMIDI入力を使った;
		private STDGBVALUE<bool> b演奏にキーボードを使った;
		private STDGBVALUE<bool> b演奏にジョイパッドを使った;
		private STDGBVALUE<bool> b演奏にマウスを使った;
		private CCounter ctWailingチップ模様アニメ;
		private STDGBVALUE<CCounter> ctチップ模様アニメ;

		private E演奏画面の戻り値 eフェードアウト完了時の戻り値;

		private STDGBVALUE<CHitCountOfRank> nヒット数_Auto含まない;
		private STDGBVALUE<CHitCountOfRank> nヒット数_Auto含む;
		private STDGBVALUE<CHitCountOfRank> nヒット数_TargetGhost; // #35411 2015.08.21 chnmr0 add
		private STDGBVALUE<int> nコンボ数_TargetGhost;
		private STDGBVALUE<int> n最大コンボ数_TargetGhost;
		private int n現在のトップChip = -1;
		private int[] n最後に再生したBGMの実WAV番号 = new int[50];
		private Ech定義 e最後に再生したHHのチャンネル番号;
		private List<int> L最後に再生したHHの実WAV番号;		// #23921 2011.1.4 yyagi: change "int" to "List<int>", for recording multiple wav No.
		private STLANEVALUE<int> n最後に再生した実WAV番号;	// #26388 2011.11.8 yyagi: change "n最後に再生した実WAV番号.GUITAR" and "n最後に再生した実WAV番号.BASS"

		private volatile Queue<STMixer> queueMixerSound;		// #24820 2013.1.21 yyagi まずは単純にAdd/Removeを1個のキューでまとめて管理するやり方で設計する
		private DateTime dtLastQueueOperation;
		private bool bIsDirectSound;
		private double db再生速度;
		private bool bValidScore;
		private STDGBVALUE<bool> bReverse;

		private STDGBVALUE<Queue<CChip>> queWailing;
		private STDGBVALUE<CChip> r現在の歓声Chip;
		private CChip[] r空打ちドラムチップ = new CChip[10];
		private CChip r現在の空うちギターChip;
		private ST空打ち r現在の空うちドラムChip;
		private CChip r現在の空うちベースChip;
		private CChip r次にくるギターChip;
		private CChip r次にくるベースChip;
		private CTexture txWailing枠;
		private CTexture txチップ;
		private CTexture txヒットバー;
		private CTexture txヒットバーGB;
		private CTexture tx背景;

		private STDGBVALUE<int> nInputAdjustTimeMs;		// #23580 2011.1.3 yyagi
		private STAUTOPLAY bIsAutoPlay;		// #24239 2011.1.23 yyagi
		private CInvisibleChip cInvisibleChip;
		private bool bUseOSTimer;
		private E判定表示優先度 e判定表示優先度;
		private CWailingChip共通[] cWailingChip;

		private STDGBVALUE<CScoreIni.C演奏記録> record;

		private CTexture txレーンフレームGB; // drums only
		private bool bフィルイン中; // drums only

		// Constructor
		public CStage演奏画面共通()
		{
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actPad = new CAct演奏Drumsパッド());
			base.list子Activities.Add(this.actChipFireD = new CAct演奏DrumsチップファイアD());
			base.list子Activities.Add(this.actChipFireGB = new CAct演奏DrumsチップファイアGB());
			base.list子Activities.Add(this.actStageFailed = new CAct演奏ステージ失敗());
			base.list子Activities.Add(this.actDANGER = new CAct演奏Danger共通());
			base.list子Activities.Add(this.actAVI = new CAct演奏AVI());
			base.list子Activities.Add(this.actBGA = new CAct演奏BGA());
			base.list子Activities.Add(this.actPanel = new CAct演奏パネル文字列());
			base.list子Activities.Add(this.act譜面スクロール速度 = new CAct演奏スクロール速度());
			base.list子Activities.Add(this.actStatusPanels = new CAct演奏ステータスパネル共通());
			base.list子Activities.Add(this.actWailingBonus = new CAct演奏WailingBonus共通());
			base.list子Activities.Add(this.actScore = new CAct演奏スコア共通());
			base.list子Activities.Add(this.actRGB = new CAct演奏RGB共通());
			base.list子Activities.Add(this.actLaneFlushD = new CAct演奏DrumsレーンフラッシュD());
			base.list子Activities.Add(this.actLaneFlushGB = new CAct演奏レーンフラッシュGB共通());
			base.list子Activities.Add(this.actJudgeString = new CAct演奏判定文字列共通());
			base.list子Activities.Add(this.actGauge = new CAct演奏ゲージ共通());
			base.list子Activities.Add(this.actGraph = new CAct演奏Drumsグラフ()); // #24074 2011.01.23 add ikanick
			base.list子Activities.Add(this.actCombo = new CAct演奏Combo共通());
			base.list子Activities.Add(this.actChipFireGB = new CAct演奏Guitarチップファイア());
			base.list子Activities.Add(this.actPlayInfo = new CAct演奏演奏情報());			
			base.list子Activities.Add(this.actFI = new CActFIFOBlack());
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actFOClear = new CActFIFOWhite());
		}

		// メソッド
		#region [ PlayRecordSave ]
		private void t演奏結果を格納する_ドラム()
		{
			record.Drums = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Drums = Record.Drums;
			if (CDTXMania.Instance.DTX.bチップがある.Drums && !CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				Drums.nスコア = (long)this.actScore.Get(E楽器パート.DRUMS);
				Drums.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Drums, CDTXMania.Instance.DTX.n可視チップ数.Drums, this.nヒット数_Auto含まない.Drums.Perfect, this.actCombo.n現在のコンボ数.Drums最高値, E楽器パート.DRUMS, bIsAutoPlay);
				Drums.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Drums, this.nヒット数_Auto含まない.Drums.Perfect, this.nヒット数_Auto含まない.Drums.Great, this.nヒット数_Auto含まない.Drums.Good, this.nヒット数_Auto含まない.Drums.Poor, this.nヒット数_Auto含まない.Drums.Miss, E楽器パート.DRUMS, bIsAutoPlay);
				Drums.nPerfect数 = CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数_Auto含む.Drums.Perfect : this.nヒット数_Auto含まない.Drums.Perfect;
				Drums.nGreat数 = CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数_Auto含む.Drums.Great : this.nヒット数_Auto含まない.Drums.Great;
				Drums.nGood数 = CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数_Auto含む.Drums.Good : this.nヒット数_Auto含まない.Drums.Good;
				Drums.nPoor数 = CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数_Auto含む.Drums.Poor : this.nヒット数_Auto含まない.Drums.Poor;
				Drums.nMiss数 = CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである ? this.nヒット数_Auto含む.Drums.Miss : this.nヒット数_Auto含まない.Drums.Miss;
				Drums.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Drums.Perfect;
				Drums.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Drums.Great;
				Drums.nGood数_Auto含まない = this.nヒット数_Auto含まない.Drums.Good;
				Drums.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Drums.Poor;
				Drums.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Drums.Miss;
				Drums.n最大コンボ数 = this.actCombo.n現在のコンボ数.Drums最高値;
				Drums.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Drums;
				for (int i = 0; i < (int)Eレーン.MAX; i++)
				{
					Drums.bAutoPlay[i] = bIsAutoPlay[i];
				}
				Drums.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (int i = 0; i < 3; i++)
				{
					Drums.bSudden[i] = CDTXMania.Instance.ConfigIni.bSudden[i];
					Drums.bHidden[i] = CDTXMania.Instance.ConfigIni.bHidden[i];
					Drums.eInvisible[i] = CDTXMania.Instance.ConfigIni.eInvisible[i];
					Drums.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Drums.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Drums.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Drums.bLeft[i] = CDTXMania.Instance.ConfigIni.bLeft[i];
					Drums.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[i] + 1)) * 0.5f;
				}
				Drums.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Drums.n演奏速度分子 = CDTXMania.Instance.ConfigIni.n演奏速度;
				Drums.n演奏速度分母 = 20;
				Drums.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				Drums.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				Drums.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;
				Drums.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH;
				Drums.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT;
				Drums.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY;
				Drums.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Drums.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Drums.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bSTAGEFAILED有効;
				Drums.eダメージレベル = CDTXMania.Instance.ConfigIni.eダメージレベル;
				Drums.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Drums;
				Drums.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Drums;
				Drums.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Drums;
				Drums.b演奏にマウスを使用した = this.b演奏にマウスを使った.Drums;
				Drums.nPerfectになる範囲ms = CDTXMania.Instance.nPerfect範囲ms;
				Drums.nGreatになる範囲ms = CDTXMania.Instance.nGreat範囲ms;
				Drums.nGoodになる範囲ms = CDTXMania.Instance.nGood範囲ms;
				Drums.nPoorになる範囲ms = CDTXMania.Instance.nPoor範囲ms;
				Drums.strDTXManiaのバージョン = CDTXMania.VERSION;
				Drums.最終更新日時 = DateTime.Now.ToString();
				Drums.Hash = CScoreIni.t演奏セクションのMD5を求めて返す(Drums);
				Drums.nRisky = CDTXMania.Instance.ConfigIni.nRisky; // #35461 chnmr0 add
				Drums.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass; // #35417 chnmr0 add
			}
		}
		private void t演奏結果を格納する_ギター()
		{
			record.Guitar = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Guitar = Record.Guitar;
			if (CDTXMania.Instance.DTX.bチップがある.Guitar)
			{
				Guitar.nスコア = (long)this.actScore.Get(E楽器パート.GUITAR);
				Guitar.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Guitar, CDTXMania.Instance.DTX.n可視チップ数.Guitar, this.nヒット数_Auto含まない.Guitar.Perfect, this.actCombo.n現在のコンボ数.Guitar最高値, E楽器パート.GUITAR, bIsAutoPlay);
				Guitar.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Guitar, this.nヒット数_Auto含まない.Guitar.Perfect, this.nヒット数_Auto含まない.Guitar.Great, this.nヒット数_Auto含まない.Guitar.Good, this.nヒット数_Auto含まない.Guitar.Poor, this.nヒット数_Auto含まない.Guitar.Miss, E楽器パート.GUITAR, bIsAutoPlay);
				Guitar.nPerfect数 = CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである ? this.nヒット数_Auto含む.Guitar.Perfect : this.nヒット数_Auto含まない.Guitar.Perfect;
				Guitar.nGreat数 = CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである ? this.nヒット数_Auto含む.Guitar.Great : this.nヒット数_Auto含まない.Guitar.Great;
				Guitar.nGood数 = CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである ? this.nヒット数_Auto含む.Guitar.Good : this.nヒット数_Auto含まない.Guitar.Good;
				Guitar.nPoor数 = CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである ? this.nヒット数_Auto含む.Guitar.Poor : this.nヒット数_Auto含まない.Guitar.Poor;
				Guitar.nMiss数 = CDTXMania.Instance.ConfigIni.bギターが全部オートプレイである ? this.nヒット数_Auto含む.Guitar.Miss : this.nヒット数_Auto含まない.Guitar.Miss;
				Guitar.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Perfect;
				Guitar.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Great;
				Guitar.nGood数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Good;
				Guitar.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Poor;
				Guitar.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Miss;
				Guitar.n最大コンボ数 = this.actCombo.n現在のコンボ数.Guitar最高値;
				Guitar.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Guitar;
				for (int i = 0; i < (int)Eレーン.MAX; i++)
				{
					Guitar.bAutoPlay[i] = bIsAutoPlay[i];
				}
				Guitar.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (int i = 0; i < 3; i++)
				{
					Guitar.bSudden[i] = CDTXMania.Instance.ConfigIni.bSudden[i];
					Guitar.bHidden[i] = CDTXMania.Instance.ConfigIni.bHidden[i];
					Guitar.eInvisible[i] = CDTXMania.Instance.ConfigIni.eInvisible[i];
					Guitar.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Guitar.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Guitar.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Guitar.bLeft[i] = CDTXMania.Instance.ConfigIni.bLeft[i];
					Guitar.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[i] + 1)) * 0.5f;
				}
				Guitar.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Guitar.n演奏速度分子 = CDTXMania.Instance.ConfigIni.n演奏速度;
				Guitar.n演奏速度分母 = 20;
				Guitar.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				Guitar.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				Guitar.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;
				Guitar.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH;
				Guitar.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT;
				Guitar.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY;
				Guitar.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Guitar.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Guitar.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bSTAGEFAILED有効;
				Guitar.eダメージレベル = CDTXMania.Instance.ConfigIni.eダメージレベル;
				Guitar.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Guitar;
				Guitar.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Guitar;
				Guitar.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Guitar;
				Guitar.b演奏にマウスを使用した = this.b演奏にマウスを使った.Guitar;
				Guitar.nPerfectになる範囲ms = CDTXMania.Instance.nPerfect範囲ms;
				Guitar.nGreatになる範囲ms = CDTXMania.Instance.nGreat範囲ms;
				Guitar.nGoodになる範囲ms = CDTXMania.Instance.nGood範囲ms;
				Guitar.nPoorになる範囲ms = CDTXMania.Instance.nPoor範囲ms;
				Guitar.strDTXManiaのバージョン = CDTXMania.VERSION;
				Guitar.最終更新日時 = DateTime.Now.ToString();
				Guitar.Hash = CScoreIni.t演奏セクションのMD5を求めて返す(Guitar);
				Guitar.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass; // #35417 chnmr0 add
			}
		}
		private void t演奏結果を格納する_ベース()
		{
			record.Bass = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Bass = Record.Bass;
			if (CDTXMania.Instance.DTX.bチップがある.Bass)
			{
				Bass.nスコア = (long)this.actScore.Get(E楽器パート.BASS);
				Bass.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Bass, CDTXMania.Instance.DTX.n可視チップ数.Bass, this.nヒット数_Auto含まない.Bass.Perfect, this.actCombo.n現在のコンボ数.Bass最高値, E楽器パート.BASS, bIsAutoPlay);
				Bass.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Bass, this.nヒット数_Auto含まない.Bass.Perfect, this.nヒット数_Auto含まない.Bass.Great, this.nヒット数_Auto含まない.Bass.Good, this.nヒット数_Auto含まない.Bass.Poor, this.nヒット数_Auto含まない.Bass.Miss, E楽器パート.BASS, bIsAutoPlay);
				Bass.nPerfect数 = CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである ? this.nヒット数_Auto含む.Bass.Perfect : this.nヒット数_Auto含まない.Bass.Perfect;
				Bass.nGreat数 = CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである ? this.nヒット数_Auto含む.Bass.Great : this.nヒット数_Auto含まない.Bass.Great;
				Bass.nGood数 = CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである ? this.nヒット数_Auto含む.Bass.Good : this.nヒット数_Auto含まない.Bass.Good;
				Bass.nPoor数 = CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである ? this.nヒット数_Auto含む.Bass.Poor : this.nヒット数_Auto含まない.Bass.Poor;
				Bass.nMiss数 = CDTXMania.Instance.ConfigIni.bベースが全部オートプレイである ? this.nヒット数_Auto含む.Bass.Miss : this.nヒット数_Auto含まない.Bass.Miss;
				Bass.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Bass.Perfect;
				Bass.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Bass.Great;
				Bass.nGood数_Auto含まない = this.nヒット数_Auto含まない.Bass.Good;
				Bass.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Bass.Poor;
				Bass.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Bass.Miss;
				Bass.n最大コンボ数 = this.actCombo.n現在のコンボ数.Bass最高値;
				Bass.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Bass;
				for (int i = 0; i < (int)Eレーン.MAX; i++)
				{
					Bass.bAutoPlay[i] = bIsAutoPlay[i];
				}
				Bass.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (int i = 0; i < 3; i++)
				{
					Bass.bSudden[i] = CDTXMania.Instance.ConfigIni.bSudden[i];
					Bass.bHidden[i] = CDTXMania.Instance.ConfigIni.bHidden[i];
					Bass.eInvisible[i] = CDTXMania.Instance.ConfigIni.eInvisible[i];
					Bass.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Bass.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Bass.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Bass.bLeft[i] = CDTXMania.Instance.ConfigIni.bLeft[i];
					Bass.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[i] + 1)) * 0.5f;
				}
				Bass.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Bass.n演奏速度分子 = CDTXMania.Instance.ConfigIni.n演奏速度;
				Bass.n演奏速度分母 = 20;
				Bass.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				Bass.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				Bass.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;
				Bass.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH;
				Bass.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT;
				Bass.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY;
				Bass.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Bass.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Bass.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bSTAGEFAILED有効;
				Bass.eダメージレベル = CDTXMania.Instance.ConfigIni.eダメージレベル;
				Bass.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Bass;			// #24280 2011.1.29 yyagi
				Bass.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Bass;				//
				Bass.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Bass;		//
				Bass.b演奏にマウスを使用した = this.b演奏にマウスを使った.Bass;					//
				Bass.nPerfectになる範囲ms = CDTXMania.Instance.nPerfect範囲ms;
				Bass.nGreatになる範囲ms = CDTXMania.Instance.nGreat範囲ms;
				Bass.nGoodになる範囲ms = CDTXMania.Instance.nGood範囲ms;
				Bass.nPoorになる範囲ms = CDTXMania.Instance.nPoor範囲ms;
				Bass.strDTXManiaのバージョン = CDTXMania.VERSION;
				Bass.最終更新日時 = DateTime.Now.ToString();
				Bass.Hash = CScoreIni.t演奏セクションのMD5を求めて返す(Bass);
				Bass.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass; // #35417 chnmr0 add
			}
		}
		#endregion

		// CStage 実装
		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.bフィルイン中 = false;
				this.dtLastQueueOperation = DateTime.MinValue;
				this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.継続;
				this.n現在のトップChip = (CDTXMania.Instance.DTX.listChip.Count > 0) ? 0 : -1;
				this.L最後に再生したHHの実WAV番号 = new List<int>(16);
				this.e最後に再生したHHのチャンネル番号 = 0;
				this.n最後に再生した実WAV番号.Guitar = -1;
				this.n最後に再生した実WAV番号.Bass = -1;
				for (int i = 0; i < 50; i++)
				{
					this.n最後に再生したBGMの実WAV番号[i] = -1;
				}
				this.r次にくるギターChip = null;
				this.r次にくるベースChip = null;
				for (int j = 0; j < 10; j++)
				{
					this.r現在の空うちドラムChip[j] = null;
				}
				this.r現在の空うちギターChip = null;
				this.r現在の空うちベースChip = null;
				cInvisibleChip = new CInvisibleChip(CDTXMania.Instance.ConfigIni.nChipDisplayTimeMs, CDTXMania.Instance.ConfigIni.nChipFadeoutTimeMs);
				this.演奏判定ライン座標 = new C演奏判定ライン座標共通();
				this.n最大コンボ数_TargetGhost = new STDGBVALUE<int>(); // #35411 2015.08.21 chnmr0 add
				for (int k = 0; k < 3; k++)
				{
					this.nヒット数_Auto含まない[k] = new CHitCountOfRank();
					this.nヒット数_Auto含む[k] = new CHitCountOfRank();
					this.nヒット数_TargetGhost[k] = new CHitCountOfRank(); // #35411 2015.08.21 chnmr0 add
					this.queWailing[k] = new Queue<CChip>();
					this.r現在の歓声Chip[k] = null;
					cInvisibleChip.eInvisibleMode[k] = CDTXMania.Instance.ConfigIni.eInvisible[k];
					if (CDTXMania.Instance.DTXVmode.Enabled)
					{
						CDTXMania.Instance.ConfigIni.n譜面スクロール速度[k] = CDTXMania.Instance.ConfigIni.nViewerScrollSpeed[k];
					}

					this.nInputAdjustTimeMs[k] = CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[k];			// #23580 2011.1.3 yyagi
					//        2011.1.7 ikanick 修正
					this.演奏判定ライン座標.n判定位置[k] = CDTXMania.Instance.ConfigIni.e判定位置[k];//#31602 2013.6.23 yyagi
					this.演奏判定ライン座標.nJudgeLinePosY_delta[k] = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[k];
					this.bReverse[k] = CDTXMania.Instance.ConfigIni.bReverse[k];

				}
				actCombo.演奏判定ライン座標 = 演奏判定ライン座標;
				for (int i = 0; i < 3; i++)
				{
					this.b演奏にキーボードを使った[i] = false;
					this.b演奏にジョイパッドを使った[i] = false;
					this.b演奏にMIDI入力を使った[i] = false;
					this.b演奏にマウスを使った[i] = false;
				}
				cInvisibleChip.Reset();
				base.On活性化();
				this.tステータスパネルの選択();
				this.tパネル文字列の設定();

				this.bIsAutoPlay = CDTXMania.Instance.ConfigIni.bAutoPlay;									// #24239 2011.1.23 yyagi
				actGauge.Init(CDTXMania.Instance.ConfigIni.nRisky);									// #23559 2011.7.28 yyagi
				e判定表示優先度 = CDTXMania.Instance.ConfigIni.e判定表示優先度;

				CDTXMania.Instance.Skin.tRemoveMixerAll();	// 効果音のストリームをミキサーから解除しておく

				queueMixerSound = new Queue<STMixer>(64);
				bIsDirectSound = (CDTXMania.Instance.Sound管理.GetCurrentSoundDeviceType() == "DirectSound");
				bUseOSTimer = CDTXMania.Instance.ConfigIni.bUseOSTimer;
				this.bPAUSE = false;
				if (CDTXMania.Instance.DTXVmode.Enabled)
				{
					db再生速度 = CDTXMania.Instance.DTX.dbDTXVPlaySpeed;
					CDTXMania.Instance.ConfigIni.n演奏速度 = (int)(CDTXMania.Instance.DTX.dbDTXVPlaySpeed * 20 + 0.5);
				}
				else
				{
					db再生速度 = ((double)CDTXMania.Instance.ConfigIni.n演奏速度) / 20.0;
				}
				bValidScore = (CDTXMania.Instance.DTXVmode.Enabled) ? false : true;

				cWailingChip = new CWailingChip共通[3];	// 0:未使用, 1:Gutiar, 2:Bass
				if (CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					cWailingChip[1] = new CWailngChip_Guitar_Drum画面(ref 演奏判定ライン座標);
					cWailingChip[2] = new CWailngChip_Bass_Drum画面(ref 演奏判定ライン座標);
				}
				else
				{
					cWailingChip[1] = new CWailngChip_Guitar_GR画面(ref 演奏判定ライン座標);
					cWailingChip[2] = new CWailngChip_Bass_GR画面(ref 演奏判定ライン座標);
				}

				#region [ 演奏開始前にmixer登録しておくべきサウンド(開幕してすぐに鳴らすことになるチップ音)を登録しておく ]
				foreach (CChip pChip in CDTXMania.Instance.DTX.listChip)
				{
					//				Debug.WriteLine( "CH=" + pChip.nチャンネル番号.ToString( "x2" ) + ", 整数値=" + pChip.n整数値 +  ", time=" + pChip.n発声時刻ms );
					if (pChip.n発声時刻ms <= 0)
					{
						if (pChip.eチャンネル番号 == Ech定義.MixerAdd)
						{
							pChip.bHit = true;
							//						Trace.TraceInformation( "first [DA] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値 + ", time=" + pChip.n発声時刻ms );
							if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))
							{
								CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
								for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPoliphonicSounds; i++)
								{
									if (wc.rSound[i] != null)
									{
										CDTXMania.Instance.Sound管理.AddMixer(wc.rSound[i], db再生速度, pChip.b演奏終了後も再生が続くチップである);
										//AddMixer( wc.rSound[ i ] );		// 最初はqueueを介さず直接ミキサー登録する
									}
								}
							}
						}
					}
					else
					{
						break;
					}
				}
				#endregion

				if (CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass)	// #24063 2011.1.24 yyagi Gt/Bsの譜面情報入れ替え
				{
					CDTXMania.Instance.DTX.SwapGuitarBassInfos();
				}

				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					// MODIFY_BEGIN #25398 2011.06.07 FROM
					if (CDTXMania.Instance.bコンパクトモード)
					{
						var score = new Cスコア();
						CDTXMania.Instance.Songs管理.tScoreIniを読み込んで譜面情報を設定する(CDTXMania.Instance.strコンパクトモードファイル + ".score.ini", ref score);
						this.actGraph.dbグラフ値目標_渡 = score.譜面情報.最大スキル[0];
					}
					else
					{
						if (CDTXMania.Instance.ConfigIni.bDrums有効)
						{
							this.actGraph.dbグラフ値目標_渡 = CDTXMania.Instance.stage選曲.r確定されたスコア.譜面情報.最大スキル[0];	// #24074 2011.01.23 add ikanick

							// #35411 2015.08.21 chnmr0 add
							// ゴースト利用可のなとき、0で初期化
							if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums != ETargetGhostData.NONE)
							{
								if (CDTXMania.Instance.DTX.listTargetGhsotLag[(int)E楽器パート.DRUMS] != null)
								{
									this.actGraph.dbグラフ値目標_渡 = 0;
								}
							}
						}
					}
					// MODIFY_END #25398
				}
			}
		}
		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				this.L最後に再生したHHの実WAV番号.Clear();	// #23921 2011.1.4 yyagi
				this.L最後に再生したHHの実WAV番号 = null;	//
				for (int i = 0; i < 3; i++)
				{
					this.queWailing[i].Clear();
					this.queWailing[i] = null;
				}
				this.ctWailingチップ模様アニメ = null;
				this.ctチップ模様アニメ.Drums = null;
				this.ctチップ模様アニメ.Guitar = null;
				this.ctチップ模様アニメ.Bass = null;
				queueMixerSound.Clear();
				queueMixerSound = null;
				cInvisibleChip.Dispose();
				cInvisibleChip = null;
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.txチップ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums chips.png"));
					this.txヒットバー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar.png"));
					this.txヒットバーGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar guitar.png"));
					this.txレーンフレームGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums lane parts guitar.png"));
					if (this.txレーンフレームGB != null)
					{
						this.txレーンフレームGB.n透明度 = 0xff - CDTXMania.Instance.ConfigIni.n背景の透過度;
					}
				}
				else
				{
					this.txチップ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar chips.png"));
					this.txヒットバー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar hit-bar.png"));
				}

				this.t背景テクスチャの生成();

				this.txWailing枠 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay wailing cursor.png"));

				base.OnManagedリソースの作成();
			}

		}
		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					TextureFactory.tテクスチャの解放(ref this.txヒットバー);
					TextureFactory.tテクスチャの解放(ref this.txヒットバーGB);
					TextureFactory.tテクスチャの解放(ref this.txチップ);
					TextureFactory.tテクスチャの解放(ref this.txレーンフレームGB);
				}
				else
				{
					TextureFactory.tテクスチャの解放(ref this.txチップ);
					TextureFactory.tテクスチャの解放(ref this.txヒットバー);
				}
				TextureFactory.tテクスチャの解放(ref this.tx背景);

				TextureFactory.tテクスチャの解放(ref this.txWailing枠);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				bool bIsFinishedPlaying = false;
				bool bIsFinishedFadeout = false;

				if (base.b初めての進行描画)
				{
					this.PrepareAVITexture();

					CSound管理.rc演奏用タイマ.tリセット();
					CDTXMania.Instance.Timer.tリセット();
					if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
					{
						this.ctチップ模様アニメ.Drums = new CCounter(0, 0x30, 10, CDTXMania.Instance.Timer);
					}
					this.ctチップ模様アニメ.Guitar = new CCounter(0, 0x17, 20, CDTXMania.Instance.Timer);
					this.ctチップ模様アニメ.Bass = new CCounter(0, 0x17, 20, CDTXMania.Instance.Timer);
					this.ctWailingチップ模様アニメ = new CCounter(0, 4, 50, CDTXMania.Instance.Timer);

					// this.actChipFireD.Start( Eレーン.HH );	// #31554 2013.6.12 yyagi
					// 初チップヒット時のもたつき回避。最初にactChipFireD.Start()するときにJITが掛かって？
					// ものすごく待たされる(2回目以降と比べると2,3桁tick違う)。そこで最初の画面フェードインの間に
					// 一発Start()を掛けてJITの結果を生成させておく。

					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					this.actFI.tフェードイン開始();

					if (CDTXMania.Instance.DTXVmode.Enabled)			// DTXVモードなら
					{
						#region [ DTXV用の再生設定にする(全AUTOなど) ]
						tDTXV用の設定();
						#endregion
						t演奏位置の変更(CDTXMania.Instance.DTXVmode.nStartBar);
					}

					CDTXMania.Instance.Sound管理.tDisableUpdateBufferAutomatically();
					base.b初めての進行描画 = false;
				}

				#region [ stage failed ]
				if (CDTXMania.Instance.ConfigIni.bSTAGEFAILED有効 && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態))
				{
					bool failedCondition = false;

					if (CDTXMania.Instance.ConfigIni.bギタレボモード)
					{
						bool bFailedGuitar = this.actGauge.IsFailed(E楽器パート.GUITAR); // #23630 2011.11.12 yyagi: deleted AutoPlay condition: not to be failed at once
						bool bFailedBass = this.actGauge.IsFailed(E楽器パート.BASS); // #23630
						bool bFailedNoChips = (!CDTXMania.Instance.DTX.bチップがある.Guitar && !CDTXMania.Instance.DTX.bチップがある.Bass); // #25216 2011.5.21 yyagi add condition
						failedCondition = bFailedGuitar || bFailedBass || bFailedNoChips; // #25216 2011.5.21 yyagi: changed codition: && -> ||
					}
					else
					{
						failedCondition = this.actGauge.IsFailed(E楽器パート.DRUMS);
					}

					if (failedCondition)
					{
						this.actStageFailed.Start();
						CDTXMania.Instance.DTX.t全チップの再生停止();
						base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED;
					}
				}
				#endregion

				this.t進行描画_AVI();
				this.t進行描画_背景();
				this.t進行描画_MIDIBGM();
				this.t進行描画_パネル文字列();
				this.t進行描画_スコア();
				this.t進行描画_BGA();
				this.t進行描画_ステータスパネル();
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_ギターベースフレーム();
				}
				this.t進行描画_レーンフラッシュGB();
				this.t進行描画_ギターベース判定ライン();
				this.t進行描画_ゲージ();
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_グラフ();   // #24074 2011.01.23 add ikanick
					this.t進行描画_レーンフラッシュD();
				}
				this.t進行描画_DANGER();
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_判定ライン();
				}
				if(this.e判定表示優先度 == E判定表示優先度.Chipより下)
				{
					this.t進行描画_RGBボタン();
					if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
					{
						this.t進行描画_判定文字列1_通常位置指定の場合();
					}
					else
					{
						this.t進行描画_判定文字列();
					}
					this.t進行描画_コンボ();
				}
				this.t進行描画_WailingBonus();
				this.t進行描画_譜面スクロール速度();
				this.t進行描画_チップアニメ();
				bIsFinishedPlaying = this.t進行描画_チップ(CDTXMania.Instance.ConfigIni.bギタレボモード ? E楽器パート.GUITAR : E楽器パート.DRUMS);
				this.t進行描画_演奏情報();
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_ドラムパッド();
				}
				if (this.e判定表示優先度 == E判定表示優先度.Chipより上)
				{
					this.t進行描画_RGBボタン();
					if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
					{
						this.t進行描画_判定文字列1_通常位置指定の場合();
					}
					else
					{
						this.t進行描画_判定文字列();
					}
					this.t進行描画_コンボ();
				}
				if(!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_判定文字列2_判定ライン上指定の場合();
				}
				this.t進行描画_Wailing枠();
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t進行描画_チップファイアD();
				}
				this.t進行描画_チップファイアGB();
				this.t進行描画_STAGEFAILED();

				bIsFinishedFadeout = this.t進行描画_フェードイン_アウト();
				if (bIsFinishedPlaying && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態))
				{
					if (CDTXMania.Instance.DTXVmode.Enabled)
					{
						if (CDTXMania.Instance.Timer.b停止していない)
						{
							this.actPanel.Stop();				// PANEL表示停止
							CDTXMania.Instance.Timer.t一時停止();		// 再生時刻カウンタ停止
						}
						Thread.Sleep(5);
						// DTXCからの次のメッセージを待ち続ける
					}
					else
					{
						this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージクリア;
						base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト;
						this.actFOClear.tフェードアウト開始();
						t演奏結果を格納する_ドラム();
						t演奏結果を格納する_ギター();
						t演奏結果を格納する_ベース();

						if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
						{
							r空打ちドラムチップ = new CChip[10];
							for (int i = 0; i < 10; i++)
							{
								r空打ちドラムチップ[i] = this.r空うちChip(E楽器パート.DRUMS, (Eパッド)i);
								if (r空打ちドラムチップ[i] == null)
								{
									r空打ちドラムチップ[i] = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(
										CSound管理.rc演奏用タイマ.n現在時刻,
										this.nパッド0Atoチャンネル0A[i],
										this.nInputAdjustTimeMs.Drums);
								}
							}
						}

						if (CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass)		// #24063 2011.1.24 yyagi Gt/Bsを入れ替えていたなら、演奏結果も入れ替える
						{
							CScoreIni.C演奏記録 t;
							t = record.Guitar;
							record.Guitar = record.Bass;
							record.Bass = t;

							// 譜面情報も元に戻す
							// #35417 2015.08.30 changed フラグにアクセスしている箇所が見つかったため有効化
							// #35417 2015.8.18 yyagi: AUTO系のフラグ入れ替えは削除可能!?。以後AUTOフラグに全くアクセスしておらず、意味がないため。
							// (直下でb全AUTOである にアクセスしているが、既に計算済みのクラスへのアクセスであり、ここでの交換対象ではない)
							CDTXMania.Instance.DTX.SwapGuitarBassInfos();
							// #24415 2011.2.27 yyagi
							// リザルト集計時のみ、Auto系のフラグも元に戻す。
							// これを戻すのは、リザルト集計後。
							// "case CStage.Eステージ.結果:"のところ。
							CDTXMania.Instance.ConfigIni.SwapGuitarBassInfos_AutoFlags();
						}
					}
				}
				if (this.eフェードアウト完了時の戻り値 == E演奏画面の戻り値.再読込_再演奏)
				{
					bIsFinishedFadeout = true;
				}
				if (bIsFinishedFadeout)
				{
					return (int)this.eフェードアウト完了時の戻り値;
				}

				ManageMixerQueue();

				// キー入力

				if (CDTXMania.Instance.act現在入力を占有中のプラグイン == null)
				{
					this.tキー入力();
				}
			}
			return 0;
		}

		// その他
		public CChip[] GetNoChipDrums()
		{
			return r空打ちドラムチップ;
		}
		public STDGBVALUE<CScoreIni.C演奏記録> Record
		{
			get
			{
				return record;
			}
		}
		public void AddMixer(CSound cs, bool _b演奏終了後も再生が続くチップである)
		{
			STMixer stm = new STMixer()
			{
				bIsAdd = true,
				csound = cs,
				b演奏終了後も再生が続くチップである = _b演奏終了後も再生が続くチップである
			};
			queueMixerSound.Enqueue(stm);
			//		Debug.WriteLine( "★Queue: add " + Path.GetFileName( stm.csound.strファイル名 ));
		}
		public void RemoveMixer(CSound cs)
		{
			STMixer stm = new STMixer()
			{
				bIsAdd = false,
				csound = cs,
				b演奏終了後も再生が続くチップである = false
			};
			queueMixerSound.Enqueue(stm);
			//		Debug.WriteLine( "★Queue: remove " + Path.GetFileName( stm.csound.strファイル名 ));
		}
		public void ManageMixerQueue()
		{
			// もしサウンドの登録/削除が必要なら、実行する
			if (queueMixerSound.Count > 0)
			{
				//Debug.WriteLine( "☆queueLength=" + queueMixerSound.Count );
				DateTime dtnow = DateTime.Now;
				TimeSpan ts = dtnow - dtLastQueueOperation;
				if (ts.Milliseconds > 7)
				{
					for (int i = 0; i < 2 && queueMixerSound.Count > 0; i++)
					{
						dtLastQueueOperation = dtnow;
						STMixer stm = queueMixerSound.Dequeue();
						if (stm.bIsAdd)
						{
							CDTXMania.Instance.Sound管理.AddMixer(stm.csound, db再生速度, stm.b演奏終了後も再生が続くチップである);
						}
						else
						{
							CDTXMania.Instance.Sound管理.RemoveMixer(stm.csound);
						}
					}
				}
			}
		}

		/// <summary>
		/// 演奏開始前に適切なサイズのAVIテクスチャを作成しておくことで、AVI再生開始時のもたつきをなくす
		/// </summary>
		protected void PrepareAVITexture()
		{
			if (CDTXMania.Instance.ConfigIni.bAVI有効)
			{
				foreach (CChip pChip in CDTXMania.Instance.DTX.listChip)
				{
					if (pChip.eチャンネル番号 == Ech定義.Movie || pChip.eチャンネル番号 == Ech定義.MovieFull)
					{
						// 最初に再生するAVIチップに合わせて、テクスチャを準備しておく
						if (pChip.rAVI != null)
						{
							this.actAVI.PrepareProperSizeTexture((int)pChip.rAVI.avi.nフレーム幅, (int)pChip.rAVI.avi.nフレーム高さ);
						}
						break;
					}
				}
			}
		}

		protected E判定 e指定時刻からChipのJUDGEを返す(long nTime, CChip pChip, int nInputAdjustTime, bool saveLag = true)
		{
			if (pChip != null)
			{
				// #35411 2015.08.22 chnmr0 modified add check save lag flag for ghost
				int lag = (int)(nTime + nInputAdjustTime - pChip.n発声時刻ms);
				if (saveLag)
				{
					pChip.nLag = lag;       // #23580 2011.1.3 yyagi: add "nInputAdjustTime" to add input timing adjust feature
					if (pChip.e楽器パート != E楽器パート.UNKNOWN)
					{
						pChip.extendInfoForGhost = this.actCombo.n現在のコンボ数[(int)pChip.e楽器パート] > 0 ? true : false;
					}
				}
				// #35411 modify end

				int nDeltaTime = Math.Abs(lag);
				//Debug.WriteLine("nAbsTime=" + (nTime - pChip.n発声時刻ms) + ", nDeltaTime=" + (nTime + nInputAdjustTime - pChip.n発声時刻ms));
				if (nDeltaTime <= CDTXMania.Instance.nPerfect範囲ms)
				{
					return E判定.Perfect;
				}
				if (nDeltaTime <= CDTXMania.Instance.nGreat範囲ms)
				{
					return E判定.Great;
				}
				if (nDeltaTime <= CDTXMania.Instance.nGood範囲ms)
				{
					return E判定.Good;
				}
				if (nDeltaTime <= CDTXMania.Instance.nPoor範囲ms)
				{
					return E判定.Poor;
				}
			}
			return E判定.Miss;
		}
		protected CChip r空うちChip(E楽器パート part, Eパッド pad)
		{
			switch (part)
			{
				case E楽器パート.DRUMS:
					switch (pad)
					{
						case Eパッド.HH:
							if (this.r現在の空うちドラムChip.HH != null)
							{
								return this.r現在の空うちドラムChip.HH;
							}
							if (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける)
							{
								if (CDTXMania.Instance.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける)
								{
									return this.r現在の空うちドラムChip.HHO;
								}
								if (this.r現在の空うちドラムChip.HHO != null)
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
							if (this.r現在の空うちドラムChip.LT != null)
							{
								return this.r現在の空うちドラムChip.LT;
							}
							if (CDTXMania.Instance.ConfigIni.eFTGroup == EFTGroup.共通)
							{
								return this.r現在の空うちドラムChip.FT;
							}
							return null;

						case Eパッド.FT:
							if (this.r現在の空うちドラムChip.FT != null)
							{
								return this.r現在の空うちドラムChip.FT;
							}
							if (CDTXMania.Instance.ConfigIni.eFTGroup == EFTGroup.共通)
							{
								return this.r現在の空うちドラムChip.LT;
							}
							return null;

						case Eパッド.CY:
							if (this.r現在の空うちドラムChip.CY != null)
							{
								return this.r現在の空うちドラムChip.CY;
							}
							if (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.共通)
							{
								return this.r現在の空うちドラムChip.RD;
							}
							return null;

						case Eパッド.HHO:
							if (this.r現在の空うちドラムChip.HHO != null)
							{
								return this.r現在の空うちドラムChip.HHO;
							}
							if (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける)
							{
								if (CDTXMania.Instance.ConfigIni.eHHGroup == EHHGroup.左シンバルのみ打ち分ける)
								{
									return this.r現在の空うちドラムChip.HH;
								}
								if (this.r現在の空うちドラムChip.HH != null)
								{
									return this.r現在の空うちドラムChip.HH;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case Eパッド.RD:
							if (this.r現在の空うちドラムChip.RD != null)
							{
								return this.r現在の空うちドラムChip.RD;
							}
							if (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.共通)
							{
								return this.r現在の空うちドラムChip.CY;
							}
							return null;

						case Eパッド.LC:
							if (this.r現在の空うちドラムChip.LC != null)
							{
								return this.r現在の空うちドラムChip.LC;
							}
							if ((CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.ハイハットのみ打ち分ける) && (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.全部共通))
							{
								return null;
							}
							if (this.r現在の空うちドラムChip.HH != null)
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
		protected CChip r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(long nTime, Ech定義 eChannel, int nInputAdjustTime)
		{
			return r指定時刻に一番近い未ヒットChip(nTime, eChannel, nInputAdjustTime, 0, HitState.DontCare);
		}
		protected void tサウンド再生(CChip rChip, long n再生開始システム時刻ms, E楽器パート part)
		{
			this.tサウンド再生(rChip, n再生開始システム時刻ms, part, CDTXMania.Instance.ConfigIni.n手動再生音量, false, false);
		}
		protected void tサウンド再生(CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量)
		{
			this.tサウンド再生(rChip, n再生開始システム時刻ms, part, n音量, false, false);
		}
		protected void tサウンド再生(CChip rChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ)
		{
			this.tサウンド再生(rChip, n再生開始システム時刻ms, part, n音量, bモニタ, false);
		}
		protected void tサウンド再生(CChip pChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ, bool b音程をずらして再生)
		{
			// mute sound (auto)
			// 4A: HH
			// 4B: CY
			// 4C: RD
			// 4D: LC
			// 2A: Gt
			// AA: Bs
			//

			if (pChip != null)
			{
				bool overwrite = false;
				switch (part)
				{
					case E楽器パート.DRUMS:
						#region [ DRUMS ]
						{
							int index = -1;
							if (pChip.bDrums可視チップ_LP_LBD含まない)
							{
								index = pChip.nDrumsIndex;
							}
							else if (pChip.bDrums不可視チップ_LP_LBD含まない)
							{
								index = pChip.nDrumsIndexHidden;
							}
							// mute sound (auto)
							// 4A: 84: HH (HO/HC)
							// 4B: 85: CY
							// 4C: 86: RD
							// 4D: 87: LC
							// 2A: 88: Gt
							// AA: 89: Bs
							else if (Ech定義.SE24 == pChip.eチャンネル番号)	// 仮に今だけ追加 HHは消音処理があるので overwriteフラグ系の処理は改めて不要
							{
								index = 0;
							}
							else if ((Ech定義.SE25 <= pChip.eチャンネル番号) && (pChip.eチャンネル番号 <= Ech定義.SE27))	// 仮に今だけ追加
							{
								pChip.ConvertSE25_26_27toCY_RCY_LCY();
								index = pChip.eチャンネル番号 - Ech定義.HiHatClose;
								overwrite = true;
							}
							else
							{
								return;
							}
							int nLane = CStage演奏画面共通.nチャンネル0Atoレーン07[index];
							if ((nLane == 1) &&	// 今回演奏するのがHC or HO
								(index == 0 || (
								index == 7 &&
								this.e最後に再生したHHのチャンネル番号 != Ech定義.HiHatOpen &&
								this.e最後に再生したHHのチャンネル番号 != Ech定義.HiHatOpen_Hidden))
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
								if (CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH)
								{
#endif
								for (int i = 0; i < this.L最後に再生したHHの実WAV番号.Count; i++)		// #23921 2011.1.4 yyagi
								{
									// CDTXMania.Instance.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号);
									CDTXMania.Instance.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号[i]);	// #23921 yyagi ストック分全て消音する
								}
								this.L最後に再生したHHの実WAV番号.Clear();
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
								}
#endif
								//this.n最後に再生したHHの実WAV番号 = pChip.n整数値_内部番号;
								this.e最後に再生したHHのチャンネル番号 = pChip.eチャンネル番号;
							}
#if TEST_NOTEOFFMODE	// 2011.1.4 yyagi test
							if (CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH)
							{
#endif
							if (index == 0 || index == 7 || index == 0x20 || index == 0x27)			// #23921 HOまたは不可視HO演奏時はそのチップ番号をストックしておく
							{																			// #24772 HC, 不可視HCも消音キューに追加
								if (this.L最後に再生したHHの実WAV番号.Count >= 16)	// #23921 ただしストック数が16以上になるようなら、頭の1個を削って常に16未満に抑える
								{													// (ストックが増えてList<>のrealloc()が発生するのを予防する)
									this.L最後に再生したHHの実WAV番号.RemoveAt(0);
								}
								if (!this.L最後に再生したHHの実WAV番号.Contains(pChip.n整数値_内部番号))	// チップ音がまだストックされてなければ
								{
									this.L最後に再生したHHの実WAV番号.Add(pChip.n整数値_内部番号);			// ストックする
								}
							}
#if TEST_NOTEOFFMODE	// 2011.1.4 yyagi test
							}
#endif
							if (overwrite)
							{
								CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号[index]);
							}
							CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, nLane, n音量, bモニタ);
							this.n最後に再生した実WAV番号[nLane] = pChip.n整数値_内部番号;		// nLaneでなくindexにすると、LC(1A-11=09)とギター(enumで09)がかぶってLC音が消されるので注意
							return;
						}
						#endregion
					case E楽器パート.GUITAR:
						#region [ GUITAR ]
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi test
						if (CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Guitar) {
#endif
						CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号.Guitar);
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, (int)Eレーン.Guitar, n音量, bモニタ, b音程をずらして再生);
						this.n最後に再生した実WAV番号.Guitar = pChip.n整数値_内部番号;
						return;
						#endregion
					case E楽器パート.BASS:
						#region [ BASS ]
#if TEST_NOTEOFFMODE
						if (CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass) {
#endif
						CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号.Bass);
#if TEST_NOTEOFFMODE
						}
#endif
						CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, (int)Eレーン.Bass, n音量, bモニタ, b音程をずらして再生);
						this.n最後に再生した実WAV番号.Bass = pChip.n整数値_内部番号;
						return;
						#endregion

					default:
						break;
				}
			}
		}
		protected void tステータスパネルの選択()
		{
			if (CDTXMania.Instance.bコンパクトモード)
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する(null);
			}
			else if (CDTXMania.Instance.stage選曲.r確定された曲 != null)
			{
				this.actStatusPanels.tラベル名からステータスパネルを決定する(CDTXMania.Instance.stage選曲.r確定された曲.ar難易度ラベル[CDTXMania.Instance.stage選曲.n確定された曲の難易度]);
			}
		}

		protected E判定 tチップのヒット処理(long nHitTime, CChip pChip, bool bCorrectLane = true)
		{
			E楽器パート screenmode;
			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				screenmode = E楽器パート.GUITAR;
			}
			else
			{
				screenmode = E楽器パート.DRUMS;
			}

			pChip.bHit = true;
			if (pChip.e楽器パート != E楽器パート.UNKNOWN)
			{
				cInvisibleChip.StartSemiInvisible(pChip.e楽器パート);
			}
			bool bPChipIsAutoPlay = pChip.bAssignAutoPlayState(bIsAutoPlay);// 2011.6.10 yyagi
			E判定 eJudgeResult = E判定.Auto;

			// #35411 2015.08.20 chnmr0 modified (begin)
			bool bIsPerfectGhost = CDTXMania.Instance.ConfigIni.eAutoGhost[(int)pChip.e楽器パート] == EAutoGhostData.PERFECT ||
					CDTXMania.Instance.DTX.listAutoGhostLag[(int)pChip.e楽器パート] == null;
			int nInputAdjustTime = bPChipIsAutoPlay && bIsPerfectGhost ? 0 : this.nInputAdjustTimeMs[(int)pChip.e楽器パート];
			eJudgeResult = (bCorrectLane) ? this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, nInputAdjustTime) : E判定.Miss;

			if (pChip.e楽器パート != E楽器パート.UNKNOWN)
			{
				int nLane = -1;
				switch (pChip.e楽器パート)
				{
					case E楽器パート.DRUMS:
						nLane = CStage演奏画面共通.nチャンネル0Atoレーン07[pChip.nDrumsIndex];
						break;
					case E楽器パート.GUITAR:
						nLane = 10;
						break;
					case E楽器パート.BASS:
						nLane = 11;
						break;
				}
				this.actJudgeString.Start(nLane, bPChipIsAutoPlay && bIsPerfectGhost ? E判定.Auto : eJudgeResult, pChip.nLag);
			}
			// #35411 end

			if (!bPChipIsAutoPlay && (pChip.e楽器パート != E楽器パート.UNKNOWN))
			{
				actGauge.Damage(screenmode, pChip.e楽器パート, eJudgeResult);
			}
			if (eJudgeResult == E判定.Poor || eJudgeResult == E判定.Miss || eJudgeResult == E判定.Bad)
			{
				cInvisibleChip.ShowChipTemporally(pChip.e楽器パート);
			}

			if (pChip.e楽器パート != E楽器パート.UNKNOWN)
			{
				switch (eJudgeResult)
				{
					case E判定.Miss:
					case E判定.Bad:
						this.nヒット数_Auto含む[(int)pChip.e楽器パート].Miss++;
						if (!bPChipIsAutoPlay)
						{
							this.nヒット数_Auto含まない[(int)pChip.e楽器パート].Miss++;
						}
						break;
					default:
						// #24068 2011.1.10 ikanick changed (for Gt./Bs.)
						// #24167 2011.1.16 yyagi changed  (for Gt./Bs.)
						this.nヒット数_Auto含む[(int)pChip.e楽器パート][(int)eJudgeResult]++;
						if (!bPChipIsAutoPlay)
						{
							this.nヒット数_Auto含まない[(int)pChip.e楽器パート][(int)eJudgeResult]++;
						}
						break;
				}

				bool incrementCombo = false;
				
				if ( pChip.e楽器パート == E楽器パート.DRUMS )
				{
					if( CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである || !bPChipIsAutoPlay)
					{
						// Dr. : 演奏したレーンだけコンボを増やす
						incrementCombo = true;
					}
				}
				else if( pChip.e楽器パート == E楽器パート.GUITAR || pChip.e楽器パート == E楽器パート.BASS )
				{
					incrementCombo = true;
				}

				switch (eJudgeResult)
				{
					case E判定.Perfect:
					case E判定.Great:
					case E判定.Good:
						this.actCombo.n現在のコンボ数[(int)pChip.e楽器パート]++;
						break;

					default:
						this.actCombo.n現在のコンボ数[(int)pChip.e楽器パート] = 0;
						break;
				}
			}

			

			if ((!bPChipIsAutoPlay && (pChip.e楽器パート != E楽器パート.UNKNOWN)) && (eJudgeResult != E判定.Miss) && (eJudgeResult != E判定.Bad))
			{
				int nCombos = this.actCombo.n現在のコンボ数[(int)pChip.e楽器パート];
				long nScoreDelta = 0;
				long[] nComboScoreDelta = new long[] { 350L, 200L, 50L, 0L };
				if ((nCombos <= 500) || (eJudgeResult == E判定.Good))
				{
					nScoreDelta = nComboScoreDelta[(int)eJudgeResult] * nCombos;
				}
				else if ((eJudgeResult == E判定.Perfect) || (eJudgeResult == E判定.Great))
				{
					nScoreDelta = nComboScoreDelta[(int)eJudgeResult] * 500L;
				}
				this.actScore.Add(pChip.e楽器パート, bIsAutoPlay, nScoreDelta);
			}

			if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				// #24074 2011.01.23 add ikanick
				this.actGraph.dbグラフ値現在_渡 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Drums, this.nヒット数_Auto含まない.Drums.Perfect, this.nヒット数_Auto含まない.Drums.Great, this.nヒット数_Auto含まない.Drums.Good, this.nヒット数_Auto含まない.Drums.Poor, this.nヒット数_Auto含まない.Drums.Miss, E楽器パート.DRUMS, bIsAutoPlay);
				// #35411 2015.09.07 add chnmr0
				if (CDTXMania.Instance.DTX.listTargetGhsotLag.Drums != null &&
									CDTXMania.Instance.ConfigIni.eTargetGhost.Drums == ETargetGhostData.ONLINE &&
					CDTXMania.Instance.DTX.n可視チップ数.Drums > 0)
				{
					// Online Stats の計算式
					this.actGraph.dbグラフ値現在_渡 = 100 *
									(this.nヒット数_Auto含まない.Drums.Perfect * 17 +
									 this.nヒット数_Auto含まない.Drums.Great * 7 +
									 this.actCombo.n現在のコンボ数.Drums最高値 * 3) / (20.0 * CDTXMania.Instance.DTX.n可視チップ数.Drums);
				}
			}
			return eJudgeResult;
		}

		private void tチップのヒット処理_BadならびにTight時のMiss(E楽器パート part, int nLane = 0)
		{
			E楽器パート screenmode;

			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				screenmode = E楽器パート.GUITAR;
			}
			else
			{
				screenmode = E楽器パート.DRUMS;
			}

			cInvisibleChip.StartSemiInvisible(part);
			cInvisibleChip.ShowChipTemporally(part);
			actGauge.Damage(screenmode, part, E判定.Miss);
			switch (part)
			{
				case E楽器パート.DRUMS:
					if ((nLane >= 0) && (nLane <= 7))
					{
						this.actJudgeString.Start(nLane, bIsAutoPlay[nLane] ? E判定.Auto : E判定.Miss, 999);
					}
					this.actCombo.n現在のコンボ数.Drums = 0;
					return;

				case E楽器パート.GUITAR:
					this.actJudgeString.Start(10, E判定.Bad, 999);
					this.actCombo.n現在のコンボ数.Guitar = 0;
					return;

				case E楽器パート.BASS:
					this.actJudgeString.Start(11, E判定.Bad, 999);
					this.actCombo.n現在のコンボ数.Bass = 0;
					break;

				default:
					return;
			}
		}

		/// <summary>
		/// 指定時刻に一番近いまだヒットしていないチップを返す。
		/// </summary>
		/// <param name="nTime">指定時刻</param>
		/// <param name="search">検索するチャネル。Gt.WailingSound/Bs.WailingSoundを渡した場合、Open~RGBが検索対象になる。</param>
		/// <param name="nInputAdjustTime"></param>
		/// <param name="n検索範囲時間ms">指定時刻から検索する範囲ms</param>
		/// <returns></returns>
		protected CChip r指定時刻に一番近い未ヒットChip(long nTime, Ech定義 search, int nInputAdjustTime, int n検索範囲時間ms = 0, HitState hs = HitState.NotHit)
		{
			CChip ret = null;

			nTime += nInputAdjustTime;
			if (this.n現在のトップChip >= 0 && this.n現在のトップChip < CDTXMania.Instance.DTX.listChip.Count)
			{
				int idxFuture = -1;
				int idxPast = -1;
				int idxPastStart = CDTXMania.Instance.DTX.listChip.Count - 1;
				Func<CChip, Func<CChip, bool>, bool> Found = (chip, futureOrPast) =>
				{
					return futureOrPast(chip) &&
						((hs == HitState.NotHit && !chip.bHit) || (hs == HitState.Hit && chip.bHit) || (hs == HitState.DontCare)) &&
						!chip.b空打ちチップである &&
						(
						((search == chip.eチャンネル番号 || search + 0x20 == chip.eチャンネル番号) && chip.bDrums可視チップ_LP_LBD含まない) ||
						(search == chip.eチャンネル番号 && chip.bGuitar可視チップ_Wailing含む) ||
						(search == chip.eチャンネル番号 && chip.bBass可視チップ_Wailing含む) ||
						(search == Ech定義.Guitar_WailingSound && chip.bGuitar可視チップ) ||
						(search == Ech定義.Bass_WailingSound && chip.bBass可視チップ)
						);
				};
				Func<CChip, bool> Future = (chip) => { return chip.n発声時刻ms > nTime; };
				Func<CChip, bool> Past = (chip) => { return chip.n発声時刻ms <= nTime; };
				Func<CChip, bool> OutOfRange = (chip) => { return n検索範囲時間ms > 0 && Math.Abs(nTime - chip.n発声時刻ms) > n検索範囲時間ms; };

				// // 未来方向への検索
				for (int i = this.n現在のトップChip; i < CDTXMania.Instance.DTX.listChip.Count; ++i)
				{
					CChip chip = CDTXMania.Instance.DTX.listChip[i];
					if (Future(chip) && OutOfRange(chip))
					{
						//break;
					}
					if (Found(chip, Future))
					{
						idxFuture = i;
						idxPastStart = i;
						break;
					}
				}

				// 過去方向への検索
				for (int i = idxPastStart; i >= 0; i--)
				{
					CChip chip = CDTXMania.Instance.DTX.listChip[i];
					if (Past(chip) && OutOfRange(chip))
					{
						//break;
					}
					if (Found(chip, Past))
					{
						idxPast = i;
						break;
					}
				}

				if (idxFuture < 0 && idxPast < 0)
				{
					// 検索対象が過去未来どちらにも見つからなかった
				}
				else if (idxPast >= 0)
				{
					// 過去方向には見つかった
					ret = CDTXMania.Instance.DTX.listChip[idxPast];
				}
				else if (idxFuture >= 0)
				{
					// 未来方向には見つかった
					ret = CDTXMania.Instance.DTX.listChip[idxFuture];
				}
				else
				{
					// どちらにも見つかった
					long nTimeDiff_Future = Math.Abs(nTime - CDTXMania.Instance.DTX.listChip[idxFuture].n発声時刻ms);
					long nTimeDiff_Past = Math.Abs(nTime - CDTXMania.Instance.DTX.listChip[idxPast].n発声時刻ms);
					if (nTimeDiff_Future < nTimeDiff_Past)
					{
						ret = CDTXMania.Instance.DTX.listChip[idxFuture];
					}
					else
					{
						ret = CDTXMania.Instance.DTX.listChip[idxPast];
					}
				}

				if (ret != null)
				{
					if (OutOfRange(ret))
					{
						// チップは見つかったが、検索範囲時間外だった場合
						ret = null;
					}
				}
			}
			return ret;
		}

		protected CChip r次に来る指定楽器Chipを更新して返す(E楽器パート inst)
		{
			CChip ret = null;
			int nInputAdjustTime;
			if (inst == E楽器パート.GUITAR)
			{
				nInputAdjustTime = this.bIsAutoPlay.GtPick ? 0 : this.nInputAdjustTimeMs.Guitar;
				ret = this.r指定時刻に一番近い未ヒットChip(CSound管理.rc演奏用タイマ.n現在時刻, Ech定義.Guitar_WailingSound, nInputAdjustTime, 500);
				this.r次にくるギターChip = ret;
			}
			else if (inst == E楽器パート.BASS)
			{
				nInputAdjustTime = this.bIsAutoPlay.BsPick ? 0 : this.nInputAdjustTimeMs.Bass;
				ret = this.r指定時刻に一番近い未ヒットChip(CSound管理.rc演奏用タイマ.n現在時刻, Ech定義.Bass_WailingSound, nInputAdjustTime, 500);
				this.r次にくるベースChip = ret;
			}
			return ret;
		}

		protected void ChangeInputAdjustTimeInPlaying(IInputDevice keyboard, int plusminus)		// #23580 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
		{
			int part, offset = plusminus;
			if (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift))	// Guitar InputAdjustTime
			{
				part = (int)E楽器パート.GUITAR;
			}
			else if (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftAlt) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightAlt))	// Bass InputAdjustTime
			{
				part = (int)E楽器パート.BASS;
			}
			else	// Drums InputAdjustTime
			{
				part = (int)E楽器パート.DRUMS;
			}
			if (!keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) && !keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl))
			{
				offset *= 10;
			}

			this.nInputAdjustTimeMs[part] += offset;
			if (this.nInputAdjustTimeMs[part] > 99)
			{
				this.nInputAdjustTimeMs[part] = 99;
			}
			else if (this.nInputAdjustTimeMs[part] < -99)
			{
				this.nInputAdjustTimeMs[part] = -99;
			}
			CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[part] = this.nInputAdjustTimeMs[part];
		}

		private void t入力処理_ドラム()
		{
			for (int nPad = 0; nPad < (int)Eパッド.MAX; nPad++)		// #27029 2012.1.4 from: <10 to <=10; Eパッドの要素が１つ（HP）増えたため。
			//		  2012.1.5 yyagi: (int)Eパッド.MAX に変更。Eパッドの要素数への依存を無くすため。
			{
				List<STInputEvent> listInputEvent = CDTXMania.Instance.Pad.GetEvents(E楽器パート.DRUMS, (Eパッド)nPad);

				if ((listInputEvent == null) || (listInputEvent.Count == 0))
					continue;

				this.t入力メソッド記憶(E楽器パート.DRUMS);

				#region [ 打ち分けグループ調整 ]
				//-----------------------------
				EHHGroup eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				EFTGroup eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				ECYGroup eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;

				if (!CDTXMania.Instance.DTX.bチップがある.Ride && (eCYGroup == ECYGroup.打ち分ける))
				{
					eCYGroup = ECYGroup.共通;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (eHHGroup == EHHGroup.全部打ち分ける))
				{
					eHHGroup = EHHGroup.左シンバルのみ打ち分ける;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (eHHGroup == EHHGroup.ハイハットのみ打ち分ける))
				{
					eHHGroup = EHHGroup.全部共通;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (eHHGroup == EHHGroup.全部打ち分ける))
				{
					eHHGroup = EHHGroup.ハイハットのみ打ち分ける;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (eHHGroup == EHHGroup.左シンバルのみ打ち分ける))
				{
					eHHGroup = EHHGroup.全部共通;
				}
				//-----------------------------
				#endregion

				foreach (STInputEvent inputEvent in listInputEvent)
				{
					if (!inputEvent.b押された)
						continue;

					long nTime = inputEvent.nTimeStamp - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
					int nPad09 = (nPad == (int)Eパッド.HP) ? (int)Eパッド.BD : nPad;		// #27029 2012.1.5 yyagi
					int nInputAdjustTime = bIsAutoPlay[CStage演奏画面共通.nチャンネル0Atoレーン07[(int)nPad09]] ? 0 : nInputAdjustTimeMs.Drums;

					bool bHitted = false;

					#region [ (A) ヒットしていればヒット処理して次の inputEvent へ ]
					//-----------------------------
					switch (((Eパッド)nPad))
					{
						case Eパッド.HH:
							#region [ HHとLC(groupingしている場合) のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HH)
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// HiHat Close
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// HiHat Open
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// LC
								E判定 e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : E判定.Miss;
								E判定 e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : E判定.Miss;
								switch (eHHGroup)
								{
									case EHHGroup.ハイハットのみ打ち分ける:
										#region [ HCとLCのヒット処理 ]
										//-----------------------------
										if ((e判定HC != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.左シンバルのみ打ち分ける:
										#region [ HCとHOのヒット処理 ]
										//-----------------------------
										if ((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
										#endregion

									case EHHGroup.全部共通:
										#region [ HC,HO,LCのヒット処理 ]
										//-----------------------------
										if (((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss)) && (e判定LC != E判定.Miss))
										{
											CChip chip;
											CChip[] chipArray = new CChip[] { chipHC, chipHO, chipLC };
											// ここから、chipArrayをn発生位置の小さい順に並び替える
											if (chipArray[1].n発声位置 > chipArray[2].n発声位置)
											{
												chip = chipArray[1];
												chipArray[1] = chipArray[2];
												chipArray[2] = chip;
											}
											if (chipArray[0].n発声位置 > chipArray[1].n発声位置)
											{
												chip = chipArray[0];
												chipArray[0] = chipArray[1];
												chipArray[1] = chip;
											}
											if (chipArray[1].n発声位置 > chipArray[2].n発声位置)
											{
												chip = chipArray[1];
												chipArray[1] = chipArray[2];
												chipArray[2] = chip;
											}
											this.tドラムヒット処理(nTime, Eパッド.HH, chipArray[0], inputEvent.nVelocity);
											if (chipArray[0].n発声位置 == chipArray[1].n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipArray[1], inputEvent.nVelocity);
											}
											if (chipArray[0].n発声位置 == chipArray[2].n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipArray[2], inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HO != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
										#endregion

									default:
										#region [ 全部打ち分け時のヒット処理 ]
										//-----------------------------
										if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
										#endregion
								}
								if (!bHitted)
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.SD:
							#region [ SDのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.SD)	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if (!this.tドラムヒット処理(nTime, Eパッド.SD, this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.Snare, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								break;
							continue;
						//-----------------------------
							#endregion

						case Eパッド.BD:
							#region [ BDのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.BD)	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if (!this.tドラムヒット処理(nTime, Eパッド.BD, this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.BassDrum, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								break;
							continue;
						//-----------------------------
							#endregion

						case Eパッド.HT:
							#region [ HTのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HT)	// #23857 2010.12.12 yyagi: to support VelocityMin
								continue;	// 電子ドラムによる意図的なクロストークを無効にする
							if (this.tドラムヒット処理(nTime, Eパッド.HT, this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HighTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								continue;
							break;
						//-----------------------------
							#endregion

						case Eパッド.LT:
							#region [ LTとFT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.LT)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CChip chipLT = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LowTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipFT = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.FloorTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								E判定 e判定LT = (chipLT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT, nInputAdjustTime) : E判定.Miss;
								E判定 e判定FT = (chipFT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT, nInputAdjustTime) : E判定.Miss;
								switch (eFTGroup)
								{
									case EFTGroup.打ち分ける:
										#region [ LTのヒット処理 ]
										//-----------------------------
										if (e判定LT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.LT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										break;
									//-----------------------------
										#endregion

									case EFTGroup.共通:
										#region [ LTとFTのヒット処理 ]
										//-----------------------------
										if ((e判定LT != E判定.Miss) && (e判定FT != E判定.Miss))
										{
											if (chipLT.n発声位置 < chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.LT, chipLT, inputEvent.nVelocity);
											}
											else if (chipLT.n発声位置 > chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.LT, chipFT, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.LT, chipLT, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.LT, chipFT, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定LT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.LT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定FT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.LT, chipFT, inputEvent.nVelocity);
											bHitted = true;
										}
										break;
									//-----------------------------
										#endregion
								}
								if (!bHitted)
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.FT:
							#region [ FTとLT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.FT)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CChip chipLT = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LowTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipFT = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.FloorTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								E判定 e判定LT = (chipLT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT, nInputAdjustTime) : E判定.Miss;
								E判定 e判定FT = (chipFT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT, nInputAdjustTime) : E判定.Miss;
								switch (eFTGroup)
								{
									case EFTGroup.打ち分ける:
										#region [ FTのヒット処理 ]
										//-----------------------------
										if (e判定FT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.FT, chipFT, inputEvent.nVelocity);
											bHitted = true;
										}
										//-----------------------------
										#endregion
										break;

									case EFTGroup.共通:
										#region [ FTとLTのヒット処理 ]
										//-----------------------------
										if ((e判定LT != E判定.Miss) && (e判定FT != E判定.Miss))
										{
											if (chipLT.n発声位置 < chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.FT, chipLT, inputEvent.nVelocity);
											}
											else if (chipLT.n発声位置 > chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.FT, chipFT, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.FT, chipLT, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.FT, chipFT, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定LT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.FT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定FT != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.FT, chipFT, inputEvent.nVelocity);
											bHitted = true;
										}
										//-----------------------------
										#endregion
										break;
								}
								if (!bHitted)
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.CY:
							#region [ CY(とLCとRD:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.CY)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CChip chipCY = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipRD = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = CDTXMania.Instance.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								E判定 e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : E判定.Miss;
								E判定 e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : E判定.Miss;
								CChip[] chipArray = new CChip[] { chipCY, chipRD, chipLC };
								E判定[] e判定Array = new E判定[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;	// chipArray.GetLength(0)

								// CY/RD/LC群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eCYGroup)
								{
									case ECYGroup.打ち分ける:
										if (!CDTXMania.Instance.ConfigIni.bシンバルフリー)
										{
											if (e判定CY != E判定.Miss)
											{
												this.tドラムヒット処理(nTime, Eパッド.CY, chipCY, inputEvent.nVelocity);
												bHitted = true;
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if ((e判定Array[i] != E判定.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipLC)))
											{
												this.tドラムヒット処理(nTime, Eパッド.CY, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
											//num10++;
										}
										if (e判定CY != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.CY, chipCY, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case ECYGroup.共通:
										if (!CDTXMania.Instance.ConfigIni.bシンバルフリー)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != E判定.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipRD)))
												{
													this.tドラムヒット処理(nTime, Eパッド.CY, chipArray[i], inputEvent.nVelocity);
													bHitted = true;
													break;
												}
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if (e判定Array[i] != E判定.Miss)
											{
												this.tドラムヒット処理(nTime, Eパッド.CY, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
										}
										if (!bHitted)
											break;
										continue;
								}
								if (!bHitted)
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.HHO:
							#region [ HO(とHCとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HH)
									continue;	// 電子ドラムによる意図的なクロストークを無効にする

								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								E判定 e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : E判定.Miss;
								E判定 e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : E判定.Miss;
								switch (eHHGroup)
								{
									case EHHGroup.全部打ち分ける:
										if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.ハイハットのみ打ち分ける:
										if ((e判定HO != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.左シンバルのみ打ち分ける:
										if ((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.全部共通:
										if (((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss)) && (e判定LC != E判定.Miss))
										{
											CChip chip;
											CChip[] chipArray = new CChip[] { chipHC, chipHO, chipLC };
											// ここから、chipArrayをn発生位置の小さい順に並び替える
											if (chipArray[1].n発声位置 > chipArray[2].n発声位置)
											{
												chip = chipArray[1];
												chipArray[1] = chipArray[2];
												chipArray[2] = chip;
											}
											if (chipArray[0].n発声位置 > chipArray[1].n発声位置)
											{
												chip = chipArray[0];
												chipArray[0] = chipArray[1];
												chipArray[1] = chip;
											}
											if (chipArray[1].n発声位置 > chipArray[2].n発声位置)
											{
												chip = chipArray[1];
												chipArray[1] = chipArray[2];
												chipArray[2] = chip;
											}
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipArray[0], inputEvent.nVelocity);
											if (chipArray[0].n発声位置 == chipArray[1].n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipArray[1], inputEvent.nVelocity);
											}
											if (chipArray[0].n発声位置 == chipArray[2].n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipArray[2], inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != E判定.Miss) && (e判定HO != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HO != E判定.Miss) && (e判定LC != E判定.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.HHO, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
								}
								if (!bHitted)
									break;
								continue;
							}
						//-----------------------------
							#endregion

						case Eパッド.RD:
							#region [ RD(とCYとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.RD)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CChip chipCY = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipRD = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = CDTXMania.Instance.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								E判定 e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : E判定.Miss;
								E判定 e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : E判定.Miss;
								CChip[] chipArray = new CChip[] { chipCY, chipRD, chipLC };
								E判定[] e判定Array = new E判定[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;	// chipArray.GetLength(0)

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eCYGroup)
								{
									case ECYGroup.打ち分ける:
										if (e判定RD != E判定.Miss)
										{
											this.tドラムヒット処理(nTime, Eパッド.RD, chipRD, inputEvent.nVelocity);
											bHitted = true;
										}
										break;

									case ECYGroup.共通:
										if (!CDTXMania.Instance.ConfigIni.bシンバルフリー)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != E判定.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipRD)))
												{
													this.tドラムヒット処理(nTime, Eパッド.CY, chipArray[i], inputEvent.nVelocity);
													bHitted = true;
													break;
												}
											}
											break;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if (e判定Array[i] != E判定.Miss)
											{
												this.tドラムヒット処理(nTime, Eパッド.CY, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
										}
										break;
								}
								if (bHitted)
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
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.LC)	// #23857 2010.12.12 yyagi: to support VelocityMin
									continue;	// 電子ドラムによる意図的なクロストークを無効にする
								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// HC
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// HO
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);	// LC
								CChip chipCY = CDTXMania.Instance.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								CChip chipRD = CDTXMania.Instance.ConfigIni.bシンバルフリー ? this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								E判定 e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : E判定.Miss;
								E判定 e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : E判定.Miss;
								E判定 e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : E判定.Miss;
								E判定 e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : E判定.Miss;
								E判定 e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : E判定.Miss;
								CChip[] chipArray = new CChip[] { chipHC, chipHO, chipLC, chipCY, chipRD };
								E判定[] e判定Array = new E判定[] { e判定HC, e判定HO, e判定LC, e判定CY, e判定RD };
								const int NumOfChips = 5;	// chipArray.GetLength(0)

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eHHGroup)
								{
									case EHHGroup.全部打ち分ける:
									case EHHGroup.左シンバルのみ打ち分ける:
										if (!CDTXMania.Instance.ConfigIni.bシンバルフリー)
										{
											if (e判定LC != E判定.Miss)
											{
												this.tドラムヒット処理(nTime, Eパッド.LC, chipLC, inputEvent.nVelocity);
												bHitted = true;
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if ((e判定Array[i] != E判定.Miss) && (((chipArray[i] == chipLC) || (chipArray[i] == chipCY)) || ((chipArray[i] == chipRD) && (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.共通))))
											{
												this.tドラムヒット処理(nTime, Eパッド.LC, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.ハイハットのみ打ち分ける:
									case EHHGroup.全部共通:
										if (!CDTXMania.Instance.ConfigIni.bシンバルフリー)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != E判定.Miss) && (((chipArray[i] == chipLC) || (chipArray[i] == chipHC)) || (chipArray[i] == chipHO)))
												{
													this.tドラムヒット処理(nTime, Eパッド.LC, chipArray[i], inputEvent.nVelocity);
													bHitted = true;
													break;
												}
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if ((e判定Array[i] != E判定.Miss) && ((chipArray[i] != chipRD) || (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.共通)))
											{
												this.tドラムヒット処理(nTime, Eパッド.LC, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
										}
										if (!bHitted)
											break;
										continue;
								}
								if (!bHitted)
									break;

								break;
							}
						//-----------------------------
							#endregion

						case Eパッド.HP:		// #27029 2012.1.4 from
							#region [ HPのヒット処理 ]
							//-----------------
							if (CDTXMania.Instance.ConfigIni.eBDGroup == EBDGroup.どっちもBD)
							{
								#region [ BDとみなしてヒット処理 ]
								//-----------------
								if (!this.tドラムヒット処理(nTime, Eパッド.BD, this.r指定時刻に一番近い未ヒットChip(nTime, Ech定義.BassDrum, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
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

					if (nPad == (int)Eパッド.HP)	// #27029 2012.1.4 from: HP&BD 時の HiHatPedal の場合は BD に成りすます。
						pad = (int)Eパッド.BD;		//（ HP|BD 時のHP入力はここまでこないので無視。）

					// レーンフラッシュ
					this.actLaneFlushD.Start((Eレーン)this.nパッド0Atoレーン07[pad], ((float)inputEvent.nVelocity) / 127f);

					// パッドアニメ
					this.actPad.Hit(this.nパッド0Atoパッド08[pad]);

					// 空打ち音
					if (CDTXMania.Instance.ConfigIni.bドラム打音を発声する)
					{
						CChip rChip = this.r空うちChip(E楽器パート.DRUMS, (Eパッド)pad);
						if (rChip != null)
						{
							#region [ (B1) 空打ち音が譜面で指定されているのでそれを再生する。]
							//-----------------
							this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.Instance.ConfigIni.n手動再生音量, CDTXMania.Instance.ConfigIni.b演奏音を強調する.Drums);
							//-----------------
							#endregion
						}
						else
						{
							#region [ (B2) 空打ち音が指定されていないので一番近いチップを探して再生する。]
							//-----------------
							switch (((Eパッド)pad))
							{
								case Eパッド.HH:
									#region [ *** ]
									//-----------------------------
									{
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[0], nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[7], nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[9], nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup)
										{
											case EHHGroup.ハイハットのみ打ち分ける:
												rChip = (chipHC != null) ? chipHC : chipLC;
												break;

											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = (chipHC != null) ? chipHC : chipHO;
												break;

											case EHHGroup.全部共通:
												if (chipHC != null)
												{
													rChip = chipHC;
												}
												else if (chipHO == null)
												{
													rChip = chipLC;
												}
												else if (chipLC == null)
												{
													rChip = chipHO;
												}
												else if (chipHO.n発声位置 < chipLC.n発声位置)
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
										CChip chipLT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[4], nInputAdjustTime);
										CChip chipFT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[5], nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eFTGroup != EFTGroup.打ち分ける)
											rChip = (chipLT != null) ? chipLT : chipFT;
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
										CChip chipLT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[4], nInputAdjustTime);
										CChip chipFT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[5], nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eFTGroup != EFTGroup.打ち分ける)
											rChip = (chipFT != null) ? chipFT : chipLT;
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
										CChip chipCY = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[6], nInputAdjustTime);
										CChip chipRD = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[8], nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eCYGroup != ECYGroup.打ち分ける)
											rChip = (chipCY != null) ? chipCY : chipRD;
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
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[0], nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[7], nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[9], nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup)
										{
											case EHHGroup.全部打ち分ける:
												rChip = chipHO;
												break;

											case EHHGroup.ハイハットのみ打ち分ける:
												rChip = (chipHO != null) ? chipHO : chipLC;
												break;

											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = (chipHO != null) ? chipHO : chipHC;
												break;

											case EHHGroup.全部共通:
												if (chipHO != null)
												{
													rChip = chipHO;
												}
												else if (chipHC == null)
												{
													rChip = chipLC;
												}
												else if (chipLC == null)
												{
													rChip = chipHC;
												}
												else if (chipHC.n発声位置 < chipLC.n発声位置)
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
										CChip chipCY = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[6], nInputAdjustTime);
										CChip chipRD = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[8], nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eCYGroup != ECYGroup.打ち分ける)
											rChip = (chipRD != null) ? chipRD : chipCY;
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
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[0], nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[7], nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[9], nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup)
										{
											case EHHGroup.全部打ち分ける:
											case EHHGroup.左シンバルのみ打ち分ける:
												rChip = chipLC;
												break;

											case EHHGroup.ハイハットのみ打ち分ける:
											case EHHGroup.全部共通:
												if (chipLC != null)
												{
													rChip = chipLC;
												}
												else if (chipHC == null)
												{
													rChip = chipHO;
												}
												else if (chipHO == null)
												{
													rChip = chipHC;
												}
												else if (chipHC.n発声位置 < chipHO.n発声位置)
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
									rChip = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, this.nパッド0Atoチャンネル0A[pad], nInputAdjustTime);
									//-----------------------------
									#endregion
									break;
							}
							if (rChip != null)
							{
								// 空打ち音が見つかったので再生する。
								this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.Instance.ConfigIni.n手動再生音量, CDTXMania.Instance.ConfigIni.b演奏音を強調する.Drums);
							}
							//-----------------
							#endregion
						}
					}

					// BAD or TIGHT 時の処理。
					if (CDTXMania.Instance.ConfigIni.bTight)
						this.tチップのヒット処理_BadならびにTight時のMiss(E楽器パート.DRUMS, this.nパッド0Atoレーン07[pad]);
					//-----------------------------
					#endregion
				}
			}
		}
		private void ドラムスクロール速度アップ()
		{
			float f = (float)this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums / (CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1);
			Debug.WriteLine("scr=" + CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + ", f1=" + f);
			CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums = Math.Min(CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1, 1999);
			f *= (CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1);
			Debug.WriteLine("scr=" + CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + ", f2=" + f);
			this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums = (int)(f + 0.5);
			CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums = (int)(f + 0.5);
		}
		private void ドラムスクロール速度ダウン()
		{
			float f = (float)this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums / (CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1);
			CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums = Math.Max(CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums - 1, 0);
			f *= (CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums + 1);
			this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums = (int)(f + 0.5);
			CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums = (int)(f + 0.5);
		}

		private int nStartTime_ = 0;
		protected void tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Instance.Input管理.Keyboard;
			if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F1) &&
				(keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
			{	// shift+f1 (pause)
				this.bPAUSE = !this.bPAUSE;
				if (this.bPAUSE)
				{
					nStartTime_ = (int)CSound管理.rc演奏用タイマ.n現在時刻;
					CSound管理.rc演奏用タイマ.t一時停止();
					CDTXMania.Instance.Timer.t一時停止();
					CDTXMania.Instance.DTX.t全チップの再生一時停止();
					CDTXMania.Instance.DTX.t全AVIの一時停止();
				}
				else
				{
					CDTXMania.Instance.DTX.t全AVIの再生再開();
					// CDTXMania.Instance.DTX.t全チップの再生再開();
					#region [ PAUSE連打でのBGMずれ対策 (AVIはずれたままになるが無視・・・) ]

					List<CSound> pausedCSound = new List<CSound>();
					for (int i = this.n現在のトップChip; i >= 0; i--)
					{
						CChip pChip = CDTXMania.Instance.DTX.listChip[i];
						int nDuration = pChip.GetDuration();

						if ((pChip.n発声時刻ms + nDuration > 0) && (pChip.n発声時刻ms <= nStartTime_) && (nStartTime_ <= pChip.n発声時刻ms + nDuration))
						{
							if (pChip.bWAVを使うチャンネルである && !pChip.b空打ちチップである)	// wav系チャンネル、且つ、空打ちチップではない
							{
								CDTX.CWAV wc;
								bool b = CDTXMania.Instance.DTX.listWAV.TryGetValue(pChip.n整数値_内部番号, out wc);
								if (!b) continue;

								if ((wc.bIsBGMSound && CDTXMania.Instance.ConfigIni.bBGM音を発声する) || (!wc.bIsBGMSound))
								{
									CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, (int)Eレーン.BGM, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
									#region [ PAUSEする ]
									int j = wc.n現在再生中のサウンド番号;
									if (wc.rSound[j] != null)
									{
										wc.rSound[j].t再生を一時停止する();
										wc.rSound[j].t再生位置を変更する(nStartTime_ - pChip.n発声時刻ms);
										pausedCSound.Add(wc.rSound[j]);
									}
									#endregion
								}
							}
						}
					}
					foreach (CSound cs in pausedCSound)
					{
						cs.tサウンドを再生する();
					}

					#endregion
					CDTXMania.Instance.Timer.t再開();
					CSound管理.rc演奏用タイマ.t再開();
				}
			}
			if ((!this.bPAUSE && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED)) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.t入力処理_ドラム();
				}
				this.t入力処理_ギターベース(E楽器パート.GUITAR);
				this.t入力処理_ギターベース(E楽器パート.BASS);
				if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.UpArrow) && (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
				{	// shift (+ctrl) + UpArrow (BGMAdjust)
					CDTXMania.Instance.DTX.t各自動再生音チップの再生時刻を変更する((keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl)) ? 1 : 10);
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.DownArrow) && (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
				{	// shift + DownArrow (BGMAdjust)
					CDTXMania.Instance.DTX.t各自動再生音チップの再生時刻を変更する((keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl)) ? -1 : -10);
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.UpArrow))
				{	// UpArrow(scrollspeed up)
					ドラムスクロール速度アップ();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.DownArrow))
				{	// DownArrow (scrollspeed down)
					ドラムスクロール速度ダウン();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Delete))
				{	// del (debug info)
					CDTXMania.Instance.ConfigIni.b演奏情報を表示する = !CDTXMania.Instance.ConfigIni.b演奏情報を表示する;
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.LeftArrow))		// #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying(keyboard, -1);
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.RightArrow))		// #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying(keyboard, +1);
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F5))
				{
					int n描画遅延ms = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums;
					n描画遅延ms = Math.Max(n描画遅延ms - 1, -99);
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass = n描画遅延ms;
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums =
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Guitar =
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Bass = n描画遅延ms;
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F6))
				{
					int n描画遅延ms = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums;
					n描画遅延ms = Math.Min(n描画遅延ms + 1, 99);
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass = n描画遅延ms;
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Drums =
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Guitar =
					this.演奏判定ライン座標.nJudgeLinePosY_delta.Bass = n描画遅延ms;
				}
				else if ((base.eフェーズID == CStage.Eフェーズ.共通_通常状態) && (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Escape) || CDTXMania.Instance.Pad.b押されたGB(Eパッド.FT)))
				{	// escape (exit)
					this.actFO.tフェードアウト開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
					this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.演奏中断;
				}
			}
		}

		protected void t入力メソッド記憶(E楽器パート part)
		{
			if (CDTXMania.Instance.Pad.st検知したデバイス.Keyboard)
			{
				this.b演奏にキーボードを使った[(int)part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.Joypad)
			{
				this.b演奏にジョイパッドを使った[(int)part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.MIDIIN)
			{
				this.b演奏にMIDI入力を使った[(int)part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.Mouse)
			{
				this.b演奏にマウスを使った[(int)part] = true;
			}
		}

		protected void t進行描画_AVI()
		{
			if ((
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) &&
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) &&
				(!CDTXMania.Instance.ConfigIni.bストイックモード && CDTXMania.Instance.ConfigIni.bAVI有効))
			{
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.actAVI.t進行描画(682, 112, 556, 710);
				}
				else
				{
					if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)
					{
						this.actAVI.t進行描画(1153, 128, 556, 710);
					}
					else
					{
						this.actAVI.t進行描画(619 + 682, 128, 556, 710);
					}
				}
			}
		}

		private void t進行描画_BGA()
		{
			if ((
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) &&
				(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) &&
				(!CDTXMania.Instance.ConfigIni.bストイックモード && CDTXMania.Instance.ConfigIni.bBGA有効))
			{
				// DR
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					this.actBGA.t進行描画(682, 112);
				}
				else
				{
					if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)
					{
						this.actBGA.t進行描画(1153, 128);
					}
					else
					{
						this.actBGA.t進行描画(619 + 682, 128);
					}
				}
			}
		}

		private void t進行描画_DANGER()
		{
			STDGBVALUE<bool> danger = new STDGBVALUE<bool>();
			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				danger.Guitar = actGauge.IsDanger(E楽器パート.GUITAR);
				danger.Bass = actGauge.IsDanger(E楽器パート.BASS);
				this.actDANGER.t進行描画(danger); // #23631 2011.4.19 yyagi
			}
			else
			{
				danger.Drums = actGauge.IsDanger(E楽器パート.DRUMS);
				this.actDANGER.t進行描画(danger);
			}
		}
		protected void t進行描画_MIDIBGM()
		{
			if (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED)
			{
				CStage.Eフェーズ eフェーズid1 = base.eフェーズID;
			}
		}
		protected void t進行描画_RGBボタン()
		{
			if (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)
			{
				this.actRGB.t進行描画(演奏判定ライン座標);
			}
		}
		protected void t進行描画_STAGEFAILED()
		{
			if (((base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED) || (base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) && ((this.actStageFailed.On進行描画() != 0) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)))
			{
				this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージ失敗;
				base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト;
				this.actFO.tフェードアウト開始();
			}
		}
		protected void t進行描画_WailingBonus()
		{
			if ((base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				this.actWailingBonus.On進行描画();
			}
		}

		private void t進行描画_Wailing枠()
		{
			int GtWailingFrameX;
			int BsWailingFrameX;
			int GtWailingFrameY = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, bReverse[(int)E楽器パート.GUITAR], true, true);
			int BsWailingFrameY = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, bReverse[(int)E楽器パート.BASS], true, true);

			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				GtWailingFrameX = (int)(139 * Scale.X);
				BsWailingFrameX = (int)(593 * Scale.X);
			}
			else
			{
				GtWailingFrameX = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1761 : 1690;
				BsWailingFrameX = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1434 : 440;
			}

			if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				if (this.txWailing枠 != null)
				{
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						this.txWailing枠.t2D描画(CDTXMania.Instance.Device, GtWailingFrameX, GtWailingFrameY);
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						this.txWailing枠.t2D描画(CDTXMania.Instance.Device, BsWailingFrameX, BsWailingFrameY);
					}
				}
			}
		}

		protected void t進行描画_チップファイアGB()
		{
			this.actChipFireGB.On進行描画();
		}
		protected void t進行描画_パネル文字列()
		{
			if ((base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				int x, y;
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					x = 0xb5;
					y = 430;
				}
				else
				{
					x = CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ? 336 : 427;
					y = 428;
				}
				this.actPanel.t進行描画(x, y);
			}
		}
		protected void tパネル文字列の設定()
		{
			this.actPanel.SetPanelString(string.IsNullOrEmpty(CDTXMania.Instance.DTX.PANEL) ? CDTXMania.Instance.DTX.TITLE : CDTXMania.Instance.DTX.PANEL);
		}


		protected void t進行描画_ゲージ()
		{
			if (((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.HALF) && (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)) && ((base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)))
			{
				this.actGauge.On進行描画();
			}
		}
		protected void t進行描画_コンボ()
		{
			this.actCombo.On進行描画();
		}
		protected void t進行描画_スコア()
		{
			this.actScore.On進行描画();
		}
		protected void t進行描画_ステータスパネル()
		{
			this.actStatusPanels.On進行描画();
		}
		/// <summary>
		/// チップを描画する。
		/// </summary>
		/// <param name="ePlayMode">演奏している楽器</param>
		/// <returns>演奏が終了したかどうかを示す値</returns>
		protected bool t進行描画_チップ(E楽器パート ePlayMode)
		{
			if ((base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED) || (base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				return true;
			}
			if ((this.n現在のトップChip == -1) || (this.n現在のトップChip >= CDTXMania.Instance.DTX.listChip.Count))
			{
				return true;
			}
			if (this.n現在のトップChip == -1)
			{
				return true;
			}

			CDTX dTX = CDTXMania.Instance.DTX;
			CConfigIni configIni = CDTXMania.Instance.ConfigIni;
			for (int nCurrentTopChip = this.n現在のトップChip; nCurrentTopChip < dTX.listChip.Count; nCurrentTopChip++)
			{
				CChip pChip = dTX.listChip[nCurrentTopChip];
				//Debug.WriteLine( "nCurrentTopChip=" + nCurrentTopChip + ", ch=" + pChip.nチャンネル番号.ToString("x2") + ", 発音位置=" + pChip.n発声位置 + ", 発声時刻ms=" + pChip.n発声時刻ms );
				pChip.CalcDistanceFromBar(CSound管理.rc演奏用タイマ.n現在時刻, this.act譜面スクロール速度.db現在の譜面スクロール速度);
				if (Math.Min(Math.Min(pChip.nバーからの距離dot.Drums, pChip.nバーからの距離dot.Guitar), pChip.nバーからの距離dot.Bass) > 450 * Scale.Y)
				{
					break;
				}
				// #28026 2012.4.5 yyagi; 信心ワールドエンドの曲終了後リザルトになかなか行かない問題の修正
				if ((dTX.listChip[this.n現在のトップChip].nバーからの距離dot.Drums < -65 * Scale.Y) &&	// 小節線の消失処理などに影響するため、
					(dTX.listChip[this.n現在のトップChip].nバーからの距離dot.Guitar < -65 * Scale.Y) &&	// Drumsのスクロールスピードだけには依存させない。
					(dTX.listChip[this.n現在のトップChip].nバーからの距離dot.Bass < -65 * Scale.Y) &&
					dTX.listChip[this.n現在のトップChip].bHit)
				{
					++this.n現在のトップChip;
					continue;
				}
				bool bPChipIsAutoPlay = pChip.bAssignAutoPlayState(bIsAutoPlay);

				int nInputAdjustTime = (bPChipIsAutoPlay || (pChip.e楽器パート == E楽器パート.UNKNOWN)) ? 0 : this.nInputAdjustTimeMs[(int)pChip.e楽器パート];

				int instIndex = (int)pChip.e楽器パート;
				if (((pChip.e楽器パート != E楽器パート.UNKNOWN) && !pChip.bHit) &&
						((pChip.nバーからの距離dot[instIndex] < -40 * Scale.Y) &&
					(this.e指定時刻からChipのJUDGEを返す(CSound管理.rc演奏用タイマ.n現在時刻, pChip, nInputAdjustTime) == E判定.Miss)))
				{
					this.tチップのヒット処理(CSound管理.rc演奏用タイマ.n現在時刻, pChip);	//チップ消失(Hitせずスルーした場合)
				}
				if (((pChip.e楽器パート != E楽器パート.UNKNOWN) && !pChip.bHit) &&
					((pChip.nバーからの距離dot[instIndex] + this.演奏判定ライン座標.nJudgeLinePosY_delta[instIndex] < 0)))
				{
					//Debug.WriteLine( "透明度＝" + pChip.n透明度 );
					pChip.n透明度 -= 12;		// チップが判定バーを越えたら、徐々に透明にする。VSyncWaitの有無で加減が変わるが・・
					if (pChip.n透明度 < 0)
					{
						pChip.n透明度 = 0;
					}
				}

				// #35411 chnmr0 add (ターゲットゴースト)
				if (CDTXMania.Instance.ConfigIni.eTargetGhost[instIndex] != ETargetGhostData.NONE &&
						 CDTXMania.Instance.DTX.listTargetGhsotLag[instIndex] != null &&
						 pChip.e楽器パート != E楽器パート.UNKNOWN &&
						 pChip.nバーからの距離dot[instIndex] < 0)
				{
					if (!pChip.bTargetGhost判定済み)
					{
						pChip.bTargetGhost判定済み = true;

						int ghostLag = 128;
						if (0 <= pChip.n楽器パートでの出現順 && pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listTargetGhsotLag[instIndex].Count)
						{
							ghostLag = CDTXMania.Instance.DTX.listTargetGhsotLag[instIndex][pChip.n楽器パートでの出現順];
							// 上位８ビットが１ならコンボが途切れている（ギターBAD空打ちでコンボ数を再現するための措置）
							if (ghostLag > 255)
							{
								this.nコンボ数_TargetGhost[instIndex] = 0;
							}
							ghostLag = (ghostLag & 255) - 128;
						}
						else if (CDTXMania.Instance.ConfigIni.eTargetGhost[instIndex] == ETargetGhostData.PERFECT)
						{
							ghostLag = 0;
						}

						if (ghostLag <= 127)
						{
							E判定 eJudge = this.e指定時刻からChipのJUDGEを返す(pChip.n発声時刻ms + ghostLag, pChip, 0, false);
							this.nヒット数_TargetGhost[instIndex][(int)eJudge]++;
							if (eJudge == E判定.Miss || eJudge == E判定.Poor)
							{
								this.n最大コンボ数_TargetGhost[instIndex] = Math.Max(this.n最大コンボ数_TargetGhost[instIndex], this.nコンボ数_TargetGhost[instIndex]);
								this.nコンボ数_TargetGhost[instIndex] = 0;
							}
							else
							{
								this.nコンボ数_TargetGhost[instIndex]++;
							}
						}
					}
				}

				if (pChip[Ech定義.BGM] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (configIni.bBGM音を発声する)
					{
						//long t = CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms;
						//Trace.TraceInformation( "BGM再生開始: 演奏タイマのn前回リセットしたときのシステム時刻=" + CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + ", pChip.n発生時刻ms=" + pChip.n発声時刻ms + ", 合計=" + t );
						dTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, (int)Eレーン.BGM, dTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
					}
				}
				else if (pChip[Ech定義.BPM] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					this.actPlayInfo.dbBPM = (pChip.n整数値 * (((double)configIni.n演奏速度) / 20.0)) + dTX.BASEBPM;
				}
				else if (pChip.bBGALayer && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (configIni.bBGA有効)
					{
						switch (pChip.eBGA種別)
						{
							case EBGA種別.BMPTEX:
								if (pChip.rBMPTEX != null)
								{
									this.actBGA.Start(pChip, null, pChip.rBMPTEX, pChip.rBMPTEX.tx画像.sz画像サイズ.Width, pChip.rBMPTEX.tx画像.sz画像サイズ.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
								}
								break;

							case EBGA種別.BGA:
								if ((pChip.rBGA != null) && ((pChip.rBMP != null) || (pChip.rBMPTEX != null)))
								{
									this.actBGA.Start(pChip, pChip.rBMP, pChip.rBMPTEX, pChip.rBGA.pt画像側右下座標.X - pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側右下座標.Y - pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt表示座標.X, pChip.rBGA.pt表示座標.Y, 0, 0, 0);
								}
								break;

							case EBGA種別.BGAPAN:
								if ((pChip.rBGAPan != null) && ((pChip.rBMP != null) || (pChip.rBMPTEX != null)))
								{
									this.actBGA.Start(pChip, pChip.rBMP, pChip.rBMPTEX, pChip.rBGAPan.sz開始サイズ.Width, pChip.rBGAPan.sz開始サイズ.Height, pChip.rBGAPan.sz終了サイズ.Width, pChip.rBGAPan.sz終了サイズ.Height, pChip.rBGAPan.pt画像側開始位置.X, pChip.rBGAPan.pt画像側開始位置.Y, pChip.rBGAPan.pt画像側終了位置.X, pChip.rBGAPan.pt画像側終了位置.Y, pChip.rBGAPan.pt表示側開始位置.X, pChip.rBGAPan.pt表示側開始位置.Y, pChip.rBGAPan.pt表示側終了位置.X, pChip.rBGAPan.pt表示側終了位置.Y, pChip.n総移動時間);
								}
								break;

							default:
								if (pChip.rBMP != null)
								{
									this.actBGA.Start(pChip, pChip.rBMP, null, pChip.rBMP.n幅, pChip.rBMP.n高さ, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
								}
								break;
						}
					}
				}
				else if (pChip[Ech定義.BPMEx] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (dTX.listBPM.ContainsKey(pChip.n整数値_内部番号))
					{
						this.actPlayInfo.dbBPM = (dTX.listBPM[pChip.n整数値_内部番号].dbBPM値 * (((double)configIni.n演奏速度) / 20.0)) + dTX.BASEBPM;
					}
				}
				else if (pChip.bDrums可視チップ && pChip.b空打ちチップである)
				{
					if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
					{
						pChip.bHit = true;
						if (CDTXMania.Instance.ConfigIni.bDrums有効)
						{
							this.r現在の空うちドラムChip[(int)this.eチャンネルtoパッド[pChip.eチャンネル番号 - Ech定義.HiHatClose]] = pChip;
						}
					}
				}
				else if (pChip.bDrums可視チップ_LP_LBD含まない)
				{
					this.t進行描画_チップ_ドラムス(ref pChip);
				}
				else if (pChip[Ech定義.DrumsFillin] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Drums = pChip;
				}
				else if (pChip.bGuitar可視チップ)
				{
					this.t進行描画_チップ_ギターベース(ref pChip, E楽器パート.GUITAR);
				}
				else if (pChip[Ech定義.Guitar_Wailing])
				{
					this.t進行描画_チップ_ウェイリング(ref pChip);
				}
				else if (pChip[Ech定義.Guitar_WailingSound] && !pChip.bHit && (pChip.nバーからの距離dot.Guitar < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Guitar = pChip;
				}
				else if (pChip.bDrums不可視チップ && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
				}
				else if (pChip[Ech定義.BarLine])// 小節線
				{
					this.t進行描画_チップ_小節線(ref pChip);
				}
				else if (pChip[Ech定義.BeatLine] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))// 拍線
				{
					pChip.bHit = true;

					if ((ePlayMode == E楽器パート.DRUMS) && (configIni.eDark != Eダークモード.FULL) && pChip.b可視 && (this.txチップ != null))
					{
						this.txチップ.t2D描画(CDTXMania.Instance.Device,
							configIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ?
								105 : 619,
							configIni.bReverse.Drums ?
								124 + pChip.nバーからの距離dot.Drums : 947 - pChip.nバーからの距離dot.Drums,
							new Rectangle(
								0,
								1006,
								(configIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 888 : 682,
								2
							)
						);
					}
				}
				else if (pChip[Ech定義.MIDIChorus] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
				}
				else if (pChip[Ech定義.FillIn])
				{
					this.t進行描画_チップ_フィルイン(ref pChip);
				}
				else if (pChip.bMovie && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (configIni.bAVI有効)
					{
						if (CDTXMania.Instance.DTX.bチップがある.BGA)
						{
							this.actAVI.bHasBGA = true;
						}
						if (pChip.eチャンネル番号 == Ech定義.MovieFull || CDTXMania.Instance.ConfigIni.bForceAVIFullscreen)
						{
							this.actAVI.bFullScreenMovie = true;
						}
						switch (pChip.eAVI種別)
						{
							case EAVI種別.AVI:
								{
									int startWidth = !this.actAVI.bFullScreenMovie ? 278 : SampleFramework.GameWindowSize.Width;
									int startHeight = !this.actAVI.bFullScreenMovie ? 355 : SampleFramework.GameWindowSize.Height;
									this.actAVI.Start(pChip.eチャンネル番号, pChip.rAVI, startWidth, startHeight, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, pChip.n発声時刻ms);
								}
								break;

							case EAVI種別.AVIPAN:
								if (pChip.rAVIPan != null)
								{
									this.actAVI.Start(pChip.eチャンネル番号, pChip.rAVI, pChip.rAVIPan.sz開始サイズ.Width, pChip.rAVIPan.sz開始サイズ.Height, pChip.rAVIPan.sz終了サイズ.Width, pChip.rAVIPan.sz終了サイズ.Height, pChip.rAVIPan.pt動画側開始位置.X, pChip.rAVIPan.pt動画側開始位置.Y, pChip.rAVIPan.pt動画側終了位置.X, pChip.rAVIPan.pt動画側終了位置.Y, pChip.rAVIPan.pt表示側開始位置.X, pChip.rAVIPan.pt表示側開始位置.Y, pChip.rAVIPan.pt表示側終了位置.X, pChip.rAVIPan.pt表示側終了位置.Y, pChip.n総移動時間, pChip.n発声時刻ms);
								}
								break;
						}
					}
				}
				else if (pChip.bSE && !pChip.bOverrideSE && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (configIni.bBGM音を発声する)
					{
						dTX.tWavの再生停止(this.n最後に再生したBGMの実WAV番号[pChip.eチャンネル番号 - Ech定義.SE01]);
						dTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, (int)Eレーン.BGM, dTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
						this.n最後に再生したBGMの実WAV番号[pChip.eチャンネル番号 - Ech定義.SE01] = pChip.n整数値_内部番号;
					}
				}
				else if (pChip.bOverrideSE && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					// override sound
					// mute sound (auto)
					// 4A: 84: HH (HO/HC)
					// 4B: 85: CY
					// 4C: 86: RD
					// 4D: 87: LC
					// 2A: 88: Gt
					// AA: 89: Bs

					//	CDTXMania.Instance.DTX.tWavの再生停止( this.n最後に再生した実WAV番号.Guitar );
					//	CDTXMania.Instance.DTX.tチップの再生( pChip, n再生開始システム時刻ms, 8, n音量, bモニタ, b音程をずらして再生 );
					//	this.n最後に再生した実WAV番号.Guitar = pChip.n整数値_内部番号;

					//	protected void tサウンド再生( CDTX.CChip pChip, long n再生開始システム時刻ms, E楽器パート part, int n音量, bool bモニタ, bool b音程をずらして再生 )
					pChip.bHit = true;
					E楽器パート[] p = { E楽器パート.DRUMS, E楽器パート.DRUMS, E楽器パート.DRUMS, E楽器パート.DRUMS, E楽器パート.GUITAR, E楽器パート.BASS };

					E楽器パート pp = p[pChip.eチャンネル番号 - Ech定義.SE24];

					//							if ( pp == E楽器パート.DRUMS ) {			// pChip.nチャンネル番号= ..... HHとか、ドラムの場合は変える。
					//								//            HC    CY    RD    LC
					//								int[] ch = { 0x11, 0x16, 0x19, 0x1A };
					//								pChip.nチャンネル番号 = ch[ pChip.nチャンネル番号 - 0x84 ]; 
					//							}
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, pp, dTX.nモニタを考慮した音量(pp));
				}

				else if (pChip.bBass可視チップ)
				{
					this.t進行描画_チップ_ギターベース(ref pChip, E楽器パート.BASS);
				}
				else if (pChip[Ech定義.Bass_Wailing])
				{
					this.t進行描画_チップ_ウェイリング(ref pChip);
				}
				else if (pChip[Ech定義.Bass_WailingSound] && !pChip.bHit && (pChip.nバーからの距離dot.Bass < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Bass = pChip;
				}
				else if (pChip[Ech定義.Guitar_NoChip] && !pChip.bHit && (pChip.nバーからの距離dot.Guitar < 0))
				{
					pChip.bHit = true;
					this.r現在の空うちギターChip = pChip;
					pChip.ConvertGBNoChip();
				}
				else if (pChip[Ech定義.Bass_NoChip] && !pChip.bHit && (pChip.nバーからの距離dot.Bass < 0))
				{
					pChip.bHit = true;
					this.r現在の空うちベースChip = pChip;
					pChip.ConvertGBNoChip();
				}
				else if (pChip.bBGALayerSwap && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if ((configIni.bBGA有効 && (pChip.eBGA種別 == EBGA種別.BMP)) || (pChip.eBGA種別 == EBGA種別.BMPTEX))
					{
						this.actBGA.ChangeScope(pChip);
					}
				}
				else if (pChip[Ech定義.MixerAdd] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					//Debug.WriteLine( "[DA(AddMixer)] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値.ToString( "x2" ) + ", time=" + pChip.n発声時刻ms );
					pChip.bHit = true;
					if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))	// 参照が遠いので後日最適化する
					{
						CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
						//Debug.Write( "[AddMixer] BAR=" + pChip.n発声位置 / 384 + ", wav=" + Path.GetFileName( wc.strファイル名 ) + ", time=" + pChip.n発声時刻ms );

						for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPoliphonicSounds; i++)
						{
							if (wc.rSound[i] != null)
							{
								//CDTXMania.Instance.Sound管理.AddMixer( wc.rSound[ i ] );
								AddMixer(wc.rSound[i], pChip.b演奏終了後も再生が続くチップである);
							}
							//else
							//{
							//    Debug.WriteLine( ", nPoly=" + i + ", Mix=" + CDTXMania.Instance.Sound管理.GetMixingStreams() );
							//    break;
							//}
							//if ( i == nPolyphonicSounds - 1 )
							//{
							//    Debug.WriteLine( ", nPoly=" + nPolyphonicSounds + ", Mix=" + CDTXMania.Instance.Sound管理.GetMixingStreams() );
							//}
						}
					}
				}
				else if (pChip[Ech定義.MixerRemove] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					//Debug.WriteLine( "[DB(RemoveMixer)] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値.ToString( "x2" ) + ", time=" + pChip.n発声時刻ms );
					pChip.bHit = true;
					if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))	// 参照が遠いので後日最適化する
					{
						CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
						//Debug.Write( "[DelMixer] BAR=" + pChip.n発声位置 / 384 +  ", wav=" + Path.GetFileName( wc.strファイル名 ) + ", time=" + pChip.n発声時刻ms );
						for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPoliphonicSounds; i++)
						{
							if (wc.rSound[i] != null)
							{
								//CDTXMania.Instance.Sound管理.RemoveMixer( wc.rSound[ i ] );
								if (!wc.rSound[i].b演奏終了後も再生が続くチップである)	// #32248 2013.10.16 yyagi
								{															// DTX終了後も再生が続くチップの0xDB登録をなくすことはできず。
									RemoveMixer(wc.rSound[i]);							// (ミキサー解除のタイミングが遅延する場合の対応が面倒なので。)
								}															// そこで、代わりにフラグをチェックしてミキサー削除ロジックへの遷移をカットする。
							}
							//else
							//{
							//    Debug.WriteLine( ", nPoly=" + i + ", Mix=" + CDTXMania.Instance.Sound管理.GetMixingStreams() );
							//    break;
							//}
							//if ( i == nPolyphonicSounds - 1 )
							//{
							//    Debug.WriteLine( ", nPoly=" + nPolyphonicSounds + ", Mix=" + CDTXMania.Instance.Sound管理.GetMixingStreams() );
							//}
						}
					}
				}
				else if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					// other chips
					pChip.bHit = true;
				}
			}
			return false;
		}

		public void t再読込()
		{
			CDTXMania.Instance.DTX.t全チップの再生停止とミキサーからの削除();
			this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.再読込_再演奏;
			base.eフェーズID = CStage.Eフェーズ.演奏_再読込;
			this.bPAUSE = false;

			// #34048 2014.7.16 yyagi
			#region [ 読み込み画面に遷移する前に、設定変更した可能性があるパラメータをConfigIniクラスに書き戻す ]
			for (int i = 0; i < 3; i++)
			{
				CDTXMania.Instance.ConfigIni.nViewerScrollSpeed[i] = CDTXMania.Instance.ConfigIni.n譜面スクロール速度[i];
			}
			CDTXMania.Instance.ConfigIni.b演奏情報を表示する = CDTXMania.Instance.ConfigIni.bViewerShowDebugStatus;
			#endregion
		}

		public void t停止()
		{
			CDTXMania.Instance.DTX.t全チップの再生停止とミキサーからの削除();
			this.actAVI.Stop();
			this.actBGA.Stop();
			this.actPanel.Stop();				// PANEL表示停止
			CDTXMania.Instance.Timer.t一時停止();		// 再生時刻カウンタ停止

			this.n現在のトップChip = CDTXMania.Instance.DTX.listChip.Count - 1;	// 終端にシーク

			// 自分自身のOn活性化()相当の処理もすべき。
		}

		/// <summary>
		/// 演奏位置を変更する。
		/// </summary>
		/// <param name="nStartBar">演奏開始小節番号</param>
		/// <param name="bResetHitStatus">演奏済み情報(bHit)をクリアするかどうか</param>
		public void t演奏位置の変更(int nStartBar)
		{
			// まず全サウンドオフにする
			CDTXMania.Instance.DTX.t全チップの再生停止();
			this.actAVI.Stop();
			this.actBGA.Stop();

			#region [ 再生開始小節の変更 ]
			nStartBar++;									// +1が必要

			#region [ 演奏済みフラグのついたChipをリセットする ]
			for (int i = 0; i < CDTXMania.Instance.DTX.listChip.Count; i++)
			{
				CChip pChip = CDTXMania.Instance.DTX.listChip[i];
				if (pChip.bHit)
				{
					CChip p = (CChip)pChip.Clone();
					p.bHit = false;
					CDTXMania.Instance.DTX.listChip[i] = p;
				}
			}
			#endregion

			#region [ 処理を開始するチップの特定 ]
			//for ( int i = this.n現在のトップChip; i < CDTXMania.Instance.DTX.listChip.Count; i++ )
			bool bSuccessSeek = false;
			for (int i = 0; i < CDTXMania.Instance.DTX.listChip.Count; i++)
			{
				CChip pChip = CDTXMania.Instance.DTX.listChip[i];
				if (pChip.n発声位置 < 384 * nStartBar)
				{
					continue;
				}
				else
				{
					bSuccessSeek = true;
					this.n現在のトップChip = i;
					break;
				}
			}
			if (!bSuccessSeek)
			{
				// this.n現在のトップChip = CDTXMania.Instance.DTX.listChip.Count - 1;
				this.n現在のトップChip = 0;		// 対象小節が存在しないなら、最初から再生
			}
			#endregion

			#region [ 演奏開始の発声時刻msを取得し、タイマに設定 ]
			int nStartTime = CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].n発声時刻ms;

			CSound管理.rc演奏用タイマ.tリセット();	// これでPAUSE解除されるので、次のPAUSEチェックは不要
			//if ( !this.bPAUSE )
			//{
			CSound管理.rc演奏用タイマ.t一時停止();
			//}
			CSound管理.rc演奏用タイマ.n現在時刻 = nStartTime;
			#endregion

			List<CSound> pausedCSound = new List<CSound>();

			#region [ BGMやギターなど、演奏開始のタイミングで再生がかかっているサウンドのの途中再生開始 ] // (CDTXのt入力・行解析・チップ配置()で小節番号が+1されているのを削っておくこと)
			for (int i = this.n現在のトップChip; i >= 0; i--)
			{
				CChip pChip = CDTXMania.Instance.DTX.listChip[i];
				int nDuration = pChip.GetDuration();

				if ((pChip.n発声時刻ms + nDuration > 0) && (pChip.n発声時刻ms <= nStartTime) && (nStartTime <= pChip.n発声時刻ms + nDuration))
				{
					if (pChip.bWAVを使うチャンネルである && !pChip.b空打ちチップである)	// wav系チャンネル、且つ、空打ちチップではない
					{
						CDTX.CWAV wc;
						bool b = CDTXMania.Instance.DTX.listWAV.TryGetValue(pChip.n整数値_内部番号, out wc);
						if (!b) continue;

						if ((wc.bIsBGMSound && CDTXMania.Instance.ConfigIni.bBGM音を発声する) || (!wc.bIsBGMSound))
						{
							CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, (int)Eレーン.BGM, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
							#region [ PAUSEする ]
							int j = wc.n現在再生中のサウンド番号;
							if (wc.rSound[j] != null)
							{
								wc.rSound[j].t再生を一時停止する();
								wc.rSound[j].t再生位置を変更する(nStartTime - pChip.n発声時刻ms);
								pausedCSound.Add(wc.rSound[j]);
							}
							#endregion
						}
					}
				}
			}
			#endregion
			#region [ 演奏開始時点で既に表示されているBGAとAVIの、シークと再生 ]
			this.actBGA.SkipStart(nStartTime);
			this.actAVI.SkipStart(nStartTime);
			#endregion
			#region [ PAUSEしていたサウンドを一斉に再生再開する(ただしタイマを止めているので、ここではまだ再生開始しない) ]
			foreach (CSound cs in pausedCSound)
			{
				cs.tサウンドを再生する();
			}
			pausedCSound.Clear();
			pausedCSound = null;
			#endregion
			#region [ タイマを再開して、PAUSEから復帰する ]
			CSound管理.rc演奏用タイマ.n現在時刻 = nStartTime;
			CDTXMania.Instance.Timer.tリセット();						// これでPAUSE解除されるので、3行先の再開()は不要
			CDTXMania.Instance.Timer.n現在時刻 = nStartTime;				// Debug表示のTime: 表記を正しくするために必要
			CSound管理.rc演奏用タイマ.t再開();
			//CDTXMania.Instance.Timer.t再開();
			this.bPAUSE = false;								// システムがPAUSE状態だったら、強制解除
			this.actPanel.Start();
			#endregion
			#endregion
		}


		/// <summary>
		/// DTXV用の設定をする。(全AUTOなど)
		/// 元の設定のバックアップなどはしないので、あとでConfig.iniを上書き保存しないこと。
		/// </summary>
		protected void tDTXV用の設定()
		{
			CDTXMania.Instance.ConfigIni.bAutoPlay.HH = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.SD = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BD = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.HT = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.LT = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.CY = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.FT = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.RD = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.LC = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtR = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtG = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtB = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtW = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BsR = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BsG = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BsB = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick = true;
			CDTXMania.Instance.ConfigIni.bAutoPlay.BsW = true;

			this.bIsAutoPlay = CDTXMania.Instance.ConfigIni.bAutoPlay;

			CDTXMania.Instance.ConfigIni.bAVI有効 = true;
			CDTXMania.Instance.ConfigIni.bBGA有効 = true;
			for (int i = 0; i < 3; i++)
			{
				CDTXMania.Instance.ConfigIni.bGraph[i] = false;
				CDTXMania.Instance.ConfigIni.bHidden[i] = false;
				CDTXMania.Instance.ConfigIni.bLeft[i] = false;
				CDTXMania.Instance.ConfigIni.bLight[i] = false;
				CDTXMania.Instance.ConfigIni.bReverse[i] = false;
				CDTXMania.Instance.ConfigIni.bSudden[i] = false;
				CDTXMania.Instance.ConfigIni.eInvisible[i] = EInvisible.OFF;
				CDTXMania.Instance.ConfigIni.eRandom[i] = Eランダムモード.OFF;
				CDTXMania.Instance.ConfigIni.n表示可能な最小コンボ数[i] = 65535;
				CDTXMania.Instance.ConfigIni.判定文字表示位置[i] = E判定文字表示位置.表示OFF;
				// CDTXMania.Instance.ConfigIni.n譜面スクロール速度[ i ] = CDTXMania.Instance.ConfigIni.nViewerScrollSpeed[ i ];	// これだけはOn活性化()で行うこと。
				// そうしないと、演奏開始直後にスクロール速度が変化して見苦しい。
			}

			CDTXMania.Instance.ConfigIni.eDark = Eダークモード.OFF;

			CDTXMania.Instance.ConfigIni.b演奏情報を表示する = CDTXMania.Instance.ConfigIni.bViewerShowDebugStatus;
			CDTXMania.Instance.ConfigIni.bフィルイン有効 = true;
			CDTXMania.Instance.ConfigIni.bScoreIniを出力する = false;
			CDTXMania.Instance.ConfigIni.bSTAGEFAILED有効 = false;
			CDTXMania.Instance.ConfigIni.bTight = false;
			CDTXMania.Instance.ConfigIni.bストイックモード = false;
			CDTXMania.Instance.ConfigIni.bドラム打音を発声する = true;
			CDTXMania.Instance.ConfigIni.bBGM音を発声する = true;

			CDTXMania.Instance.ConfigIni.nRisky = 0;
			CDTXMania.Instance.ConfigIni.nShowLagType = 0;
			CDTXMania.Instance.ConfigIni.ドラムコンボ文字の表示位置 = Eドラムコンボ文字の表示位置.OFF;
		}

		/// <summary>
		/// ギター・ベースのチップ表示
		/// </summary>
		/// <param name="configIni"></param>
		/// <param name="dTX"></param>
		/// <param name="pChip">描画するチップ</param>
		/// <param name="inst">楽器種別</param>
		/// <param name="barYNormal">Normal時判定ライン表示Y座標</param>
		/// <param name="barYReverse">Reverse時判定ライン表示Y座標</param>
		/// <param name="showRangeY0">チップ表示Y座標範囲(最小値)</param>
		/// <param name="showRangeY1">チップ表示Y座標範囲(最大値)</param>
		/// <param name="openXg">オープンチップの表示X座標(ギター用)</param>
		/// <param name="openXb">オープンチップの表示X座標(ベース用)</param>
		/// <param name="rectOpenOffsetX">テクスチャ内のオープンチップregionのx座標</param>
		/// <param name="rectOpenOffsetY">テクスチャ内のオープンチップregionのy座標</param>
		/// <param name="openChipWidth">テクスチャ内のオープンチップregionのwidth</param>
		/// <param name="chipHeight">テクスチャ内のチップのheight</param>
		/// <param name="chipWidth">テクスチャ内のチップのwidth</param>
		/// <param name="guitarNormalX">ギターチップ描画のx座標(Normal)</param>
		/// <param name="guitarLeftyX">ギターチップ描画のx座標(Lefty)</param>
		/// <param name="bassNormalX">ベースチップ描画のx座標(Normal)</param>
		/// <param name="bassLeftyX">ベースチップ描画のx座標(Lefty)</param>
		/// <param name="drawDeltaX">描画のX座標間隔(R,G,B...)</param>
		/// <param name="chipTexDeltaX">テクスチャののX座標間隔(R,G,B...)</param>
		protected void t進行描画_チップ_ギターベース(ref CChip pChip, E楽器パート inst,
			int barYNormal, int barYReverse,
			int showRangeY0, int showRangeY1, int openXg, int openXb,
			int rectOpenOffsetX, int rectOpenOffsetY, int openChipWidth, int chipHeight, int chipWidth,
			int guitarNormalX, int guitarLeftyX, int bassNormalX, int bassLeftyX, int drawDeltaX, int chipTexDeltaX)
		{
			int instIndex = (int)inst;
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				#region [ Invisible処理 ]
				if (CDTXMania.Instance.ConfigIni.eInvisible[instIndex] != EInvisible.OFF)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				#endregion
				else
				{
					#region [ Hidden/Sudden処理 ]
					if (CDTXMania.Instance.ConfigIni.bSudden[instIndex])
					{
						pChip.b可視 = (pChip.nバーからの距離dot[instIndex] < 200 * Scale.Y);
					}
					if (CDTXMania.Instance.ConfigIni.bHidden[instIndex] && (pChip.nバーからの距離dot[instIndex] < 100 * Scale.Y))
					{
						pChip.b可視 = false;
					}
					#endregion
				}

				bool bChipHasR = pChip.bGuitarBass_R;
				bool bChipHasG = pChip.bGuitarBass_G;
				bool bChipHasB = pChip.bGuitarBass_B;
				bool bChipHasW = pChip.bGuitarBass_Wailing;
				bool bChipIsO = pChip.bGuitarBass_Open;

				#region [ chip描画 ]
				Ech定義 OPEN = (inst == E楽器パート.GUITAR) ? Ech定義.Guitar_Open : Ech定義.Bass_Open;
				if (!pChip.bHit && pChip.b可視)
				{
					if (this.txチップ != null)
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int y = CDTXMania.Instance.ConfigIni.bReverse[instIndex] ?
						(int)(barYReverse - pChip.nバーからの距離dot[instIndex]) :
						(int)(barYNormal + pChip.nバーからの距離dot[instIndex]);
					int n小節線消失距離dot = CDTXMania.Instance.ConfigIni.bReverse[instIndex] ?
						(int)(-100 * Scale.Y) :
						(CDTXMania.Instance.ConfigIni.e判定位置[instIndex] == E判定位置.標準) ? (int)(-36 * Scale.Y) : (int)(-25 * Scale.Y);
					if (CDTXMania.Instance.ConfigIni.bReverse[instIndex])
					{
						//showRangeY1 = barYReverse - n小節線消失距離dot;
					}
					else
					{
						showRangeY0 = barYNormal + n小節線消失距離dot;
					}
					if ((showRangeY0 < y) && (y < showRangeY1))
					{
						if (this.txチップ != null)
						{
							int nアニメカウンタ現在の値 = this.ctチップ模様アニメ[instIndex].n現在の値;
							#region [ OPENチップの描画 ]
							if (pChip.eチャンネル番号 == OPEN)
							{
								int xo = (inst == E楽器パート.GUITAR) ? openXg : openXb;
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									xo * Scale.X,
									y - (2 * Scale.Y),
									new Rectangle(
										(int)(rectOpenOffsetX * Scale.X),
										(int)(rectOpenOffsetY * Scale.Y) + (int)(((nアニメカウンタ現在の値 % 5) * chipHeight * Scale.Y)),
										(int)(openChipWidth * Scale.X),
										(int)(chipHeight * Scale.Y)
									)
								);
							}
							#endregion
							Rectangle rc = new Rectangle(
								(int)(rectOpenOffsetX * Scale.X),
								(int)(nアニメカウンタ現在の値 * chipHeight * Scale.Y),
								(int)(chipWidth * Scale.X),
								(int)(chipHeight * Scale.Y)
							);
							#region [ RGBチップのX座標初期化 ]
							int x;
							if (inst == E楽器パート.GUITAR)
							{
								x = (CDTXMania.Instance.ConfigIni.bLeft.Guitar) ? guitarLeftyX : guitarNormalX;
							}
							else
							{
								x = (CDTXMania.Instance.ConfigIni.bLeft.Bass) ? bassLeftyX : bassNormalX;
							}
							int deltaX = (CDTXMania.Instance.ConfigIni.bLeft[instIndex]) ? -drawDeltaX : +drawDeltaX;
							#endregion
							//Trace.TraceInformation( "chip={0:x2}, E楽器パート={1}, x={2}", pChip.nチャンネル番号, inst, x );
							#region [ Rチップ描画 ]
							if (bChipHasR)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x * Scale.X,
									y - (chipHeight / 2) * Scale.Y,
									rc
								);
							}
							#endregion
							#region [ Gチップ描画 ]
							rc.X += (int)(chipTexDeltaX * Scale.X);
							x += deltaX;
							if (bChipHasG)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x * Scale.X,
									y - (chipHeight / 2) * Scale.Y,
									rc
								);
							}
							#endregion
							#region [ Bチップ描画 ]
							rc.X += (int)(chipTexDeltaX * Scale.X);
							x += deltaX;
							if (bChipHasB)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device,
									x * Scale.X,
									y - (chipHeight / 2) * Scale.Y,
									rc
								);
							}
							#endregion
						}
					}
				}
				#endregion
				//if ( ( configIni.bAutoPlay.Guitar && !pChip.bHit ) && ( pChip.nバーからの距離dot.Guitar < 0 ) )


				// #35411 2015.08.20 chnmr0 modified
				// 従来のAUTO処理に加えてプレーヤーゴーストの再生機能を追加
				bool autoPlayCondition = (!pChip.bHit) && (pChip.nバーからの距離dot[instIndex] < 0);
				if (autoPlayCondition)
				{
					cInvisibleChip.StartSemiInvisible(inst);
				}

				bool autoPick = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtPick : bIsAutoPlay.BsPick;
				autoPlayCondition = !pChip.bHit && autoPick;
				long ghostLag = 0;
				bool bUsePerfectGhost = true;

				if ((pChip.e楽器パート == E楽器パート.GUITAR || pChip.e楽器パート == E楽器パート.BASS) &&
						CDTXMania.Instance.ConfigIni.eAutoGhost[(int)(pChip.e楽器パート)] != EAutoGhostData.PERFECT &&
						CDTXMania.Instance.DTX.listAutoGhostLag[(int)pChip.e楽器パート] != null &&
						0 <= pChip.n楽器パートでの出現順 &&
						pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag[(int)pChip.e楽器パート].Count)
				{
					// #35411 (mod) Ghost data が有効なので 従来のAUTOではなくゴーストのラグを利用
					// 発生時刻と現在時刻からこのタイミングで演奏するかどうかを決定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag[(int)pChip.e楽器パート][pChip.n楽器パートでの出現順];
					bool resetCombo = ghostLag > 255;
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= (pChip.e楽器パート == E楽器パート.GUITAR ? nInputAdjustTimeMs.Guitar : nInputAdjustTimeMs.Bass);
					autoPlayCondition &= (pChip.n発声時刻ms + ghostLag <= CSound管理.rc演奏用タイマ.n現在時刻ms);
					if (resetCombo && autoPlayCondition)
					{
						this.actCombo.n現在のコンボ数[(int)pChip.e楽器パート] = 0;
					}
					bUsePerfectGhost = false;
				}

				if (bUsePerfectGhost)
				{
					// 従来のAUTOを使用する場合
					autoPlayCondition &= (pChip.nバーからの距離dot[instIndex] < 0);
				}

				if (autoPlayCondition)
				{
					int lo = (inst == E楽器パート.GUITAR) ? 0 : 3;	// lane offset
					bool autoR = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtR : bIsAutoPlay.BsR;
					bool autoG = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtG : bIsAutoPlay.BsG;
					bool autoB = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtB : bIsAutoPlay.BsB;
					bool pushingR = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.R);
					bool pushingG = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.G);
					bool pushingB = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.B);

					#region [ Chip Fire effects (auto時用) ]
					// autoPickでない時の処理は、 t入力処理・ギターベース(E楽器パート) で行う
					bool bSuccessOPEN = bChipIsO && (autoR || !pushingR) && (autoG || !pushingG) && (autoB || !pushingB);
					{
						if ((bChipHasR && (autoR || pushingR)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(0 + lo, 演奏判定ライン座標);
						}
						if ((bChipHasG && (autoG || pushingG)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(1 + lo, 演奏判定ライン座標);
						}
						if ((bChipHasB && (autoB || pushingB)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(2 + lo, 演奏判定ライン座標);
						}
					}
					#endregion
					#region [ autopick ]
					{
						bool bMiss = true;
						if (bChipHasR == autoR && bChipHasG == autoG && bChipHasB == autoB)		// autoレーンとチップレーン一致時はOK
						{																			// この条件を加えないと、同時に非autoレーンを押下している時にNGとなってしまう。
							bMiss = false;
						}
						else if ((autoR || (bChipHasR == pushingR)) && (autoG || (bChipHasG == pushingG)) && (autoB || (bChipHasB == pushingB)))
						// ( bChipHasR == ( pushingR | autoR ) ) && ( bChipHasG == ( pushingG | autoG ) ) && ( bChipHasB == ( pushingB | autoB ) ) )
						{
							bMiss = false;
						}
						else if (((bChipIsO == true) && (!pushingR | autoR) && (!pushingG | autoG) && (!pushingB | autoB)))	// OPEN時
						{
							bMiss = false;
						}
						pChip.bHit = true;
						this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag, inst, CDTXMania.Instance.DTX.nモニタを考慮した音量(inst), false, bMiss);
						this.r次にくるギターChip = null;
						if (!bMiss)
						{
							this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
						}
						else
						{
							pChip.nLag = 0;		// tチップのヒット処理()の引数最後がfalseの時はpChip.nLagを計算しないため、ここでAutoPickかつMissのLag=0を代入
							this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip, false);
						}
						Ech定義 chWailingChip = (inst == E楽器パート.GUITAR) ? Ech定義.Guitar_Wailing : Ech定義.Bass_Wailing;
						CChip item = this.r指定時刻に一番近い未ヒットChip(pChip.n発声時刻ms + ghostLag, chWailingChip, this.nInputAdjustTimeMs[instIndex], 140);
						if (item != null && !bMiss)
						{
							this.queWailing[instIndex].Enqueue(item);
						}
					}
					#endregion
					// #35411 modify end
				}
				return;
			}	// end of "if configIni.bGuitar有効"
			if (!pChip.bHit && (pChip.nバーからの距離dot[instIndex] < 0))	// Guitar/Bass無効の場合は、自動演奏する
			{
				pChip.bHit = true;
				this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, inst, CDTXMania.Instance.DTX.nモニタを考慮した音量(inst));
			}
		}


		private void t進行描画_チップ_ウェイリング(ref CChip pChip)
		{
			E楽器パート indexInst = pChip.bGuitar可視チップ_Wailing含む ? E楽器パート.GUITAR : E楽器パート.BASS;
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				#region [ Invisible処理 ]
				if (CDTXMania.Instance.ConfigIni.eInvisible[(int)indexInst] != EInvisible.OFF)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				#endregion
				#region [ Sudden/Hidden処理 ]
				if (CDTXMania.Instance.ConfigIni.bSudden[(int)indexInst])
				{
					pChip.b可視 = (pChip.nバーからの距離dot[(int)indexInst] < 200 * Scale.Y);
				}
				if (CDTXMania.Instance.ConfigIni.bHidden[(int)indexInst] && (pChip.nバーからの距離dot[(int)indexInst] < 100 * Scale.Y))
				{
					pChip.b可視 = false;
				}
				#endregion

				cWailingChip[(int)indexInst].t進行描画_チップ_ウェイリング(ref pChip, ref txチップ, ref 演奏判定ライン座標, ref ctWailingチップ模様アニメ
				);

				if (!pChip.bHit && (pChip.nバーからの距離dot[(int)indexInst] < 0))
				{
					if (pChip.nバーからの距離dot[(int)indexInst] < -234 * Scale.Y)	// #25253 2011.5.29 yyagi: Don't set pChip.bHit=true for wailing at once. It need to 1sec-delay (234pix per 1sec). 
					{
						pChip.bHit = true;
					}
					bool autoW = (indexInst == E楽器パート.GUITAR) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtW : CDTXMania.Instance.ConfigIni.bAutoPlay.BsW;
					if (autoW)
					{
						//    pChip.bHit = true;								// #25253 2011.5.29 yyagi: Set pChip.bHit=true if autoplay.
						//    this.actWailingBonus.Start( inst, this.r現在の歓声Chip[indexInst] );
						// #23886 2012.5.22 yyagi; To support auto Wailing; Don't do wailing for ALL wailing chips. Do wailing for queued wailing chip.
						// wailing chips are queued when 1) manually wailing and not missed at that time 2) AutoWailing=ON and not missed at that time
						long nTimeStamp_Wailed = pChip.n発声時刻ms + CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
						DoWailingFromQueue(indexInst, nTimeStamp_Wailed, autoW);
					}
					cInvisibleChip.StartSemiInvisible(indexInst);
				}
				return;
			}
			pChip.bHit = true;
		}

		protected void t進行描画_チップ_小節線(ref CChip pChip)
		{
			CConfigIni configIni = CDTXMania.Instance.ConfigIni;
			int n小節番号plus1 = pChip.n発声位置 / 384;
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;
				this.actPlayInfo.n小節番号 = n小節番号plus1 - 1;
				if (configIni.bWave再生位置自動調整機能有効 && (bIsDirectSound || bUseOSTimer))
				{
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				// dTX.tWaveBGM再生位置表示();		//デバッグ用
			}
			#region [ Drumsの小節線と、小節番号 ]
			if (configIni.bDrums有効)
			{
				if (configIni.b演奏情報を表示する && (configIni.eDark == Eダークモード.OFF))
				{
					int n小節番号 = n小節番号plus1 - 1;
					CDTXMania.Instance.act文字コンソール.tPrint(
						configIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ?
							999 : 619 + 682,
						configIni.bReverse.Drums ?
							126 + pChip.nバーからの距離dot.Drums :
							911 - pChip.nバーからの距離dot.Drums,
						C文字コンソール.Eフォント種別.白,
						n小節番号.ToString()
					);
				}
				if (((configIni.eDark != Eダークモード.FULL) && pChip.b可視) && (this.txチップ != null))
				{
					this.txチップ.n透明度 = 255;
					this.txチップ.t2D描画(CDTXMania.Instance.Device,
						//3, 2.25
						configIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left ?
							105 : 619,
						configIni.bReverse.Drums ?
							124 + pChip.nバーからの距離dot.Drums : 947 - pChip.nバーからの距離dot.Drums,
						new Rectangle(
							0,
							999,
							(configIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 888 : 682,
							4
						)
					);
				}
			}
			#endregion
			if ((pChip.b可視 && configIni.bGuitar有効) && (configIni.eDark != Eダークモード.FULL) && (this.txチップ != null))
			{
				this.txチップ.n透明度 = 255;

				int[] barlineXGtBs = new int[3] { 0, 78, 480 * 3 }; // first element is padding 
				int[,] barlineXDrCL = new int[3, 2] { { 0, 0 }, { 1521, 1450 }, { 1194, 200 } }; // first dimension is padding. [inst, left or center]


				for (E楽器パート gt = E楽器パート.GUITAR; gt <= E楽器パート.BASS; ++gt)
				{
					int y = 演奏判定ライン座標.n判定ラインY座標(gt, CDTXMania.Instance.ConfigIni.bReverse[(int)gt]);
					if (CDTXMania.Instance.ConfigIni.bReverse[(int)gt])
					{
						y = y - (int)(pChip.nバーからの距離dot[(int)gt]) - 1;
					}
					else
					{
						y = y + (int)(pChip.nバーからの距離dot[(int)gt]) - 1;
					}
					// Reverse時の小節線消失位置を、RGBボタンの真ん中程度に。
					// 非Reverse時の消失処理は、従来通りt進行描画・チップ()にお任せ。
					int n小節線消失距離dot = configIni.bReverse[(int)gt] ?
						(int)(-100 * Scale.Y) :
						(configIni.e判定位置[(int)gt] == E判定位置.標準) ?
						(int)(CDTXMania.Instance.ConfigIni.bギタレボモード ? -36 : -50 * Scale.Y) : (int)(-25 * Scale.Y);

					if ((CDTXMania.Instance.DTX.bチップがある[(int)gt]) &&
						((CDTXMania.Instance.ConfigIni.bギタレボモード ? 0 : 0x39) * Scale.Y < y) &&
						((y < (CDTXMania.Instance.ConfigIni.bギタレボモード ? 0x199 : 0x19c) * Scale.Y)) &&
						(pChip.nバーからの距離dot.Guitar >= n小節線消失距離dot)
						)
					{
						this.txチップ.t2D描画(
							CDTXMania.Instance.Device,
							CDTXMania.Instance.ConfigIni.bギタレボモード ? barlineXGtBs[(int)gt] :
							((CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? barlineXDrCL[(int)gt, 0] : barlineXDrCL[(int)gt, 1]),
							y,
							new Rectangle(
								0,
								(int)((CDTXMania.Instance.ConfigIni.bギタレボモード ? 0xeb : 450) * Scale.Y),
								(int)((CDTXMania.Instance.ConfigIni.bギタレボモード ? 0x68 : 0x4e) * Scale.X),
								(int)(1 * Scale.Y)
							)
						);
					}
				}
			}
		}

		protected void t進行描画_チップアニメ()
		{
			for (int i = 0; i < 3; i++)			// 0=drums, 1=guitar, 2=bass
			{
				if (this.ctチップ模様アニメ[i] != null)
				{
					this.ctチップ模様アニメ[i].t進行Loop();
				}
			}
			if (this.ctWailingチップ模様アニメ != null)
			{
				this.ctWailingチップ模様アニメ.t進行Loop();
			}
		}

		protected bool t進行描画_フェードイン_アウト()
		{
			switch (base.eフェーズID)
			{
				case CStage.Eフェーズ.共通_フェードイン:
					if (this.actFI.On進行描画() != 0)
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
					break;

				case CStage.Eフェーズ.共通_フェードアウト:
				case CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト:
					if (this.actFO.On進行描画() != 0)
					{
						return true;
					}
					break;

				case CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト:
					if (this.actFOClear.On進行描画() == 0)
					{
						break;
					}
					return true;

			}
			return false;
		}
		protected void t進行描画_レーンフラッシュD()
		{
			if ((CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED) && (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト))
			{
				this.actLaneFlushD.On進行描画();
			}
		}
		protected void t進行描画_レーンフラッシュGB()
		{
			if ((CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF) && CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				this.actLaneFlushGB.On進行描画();
			}
		}
		protected void t進行描画_演奏情報()
		{
			if (!CDTXMania.Instance.ConfigIni.b演奏情報を表示しない)
			{
				int x, y;
				if (CDTXMania.Instance.ConfigIni.bギタレボモード)
				{
					x = 0xb5;
					y = 50;
				}
				else
				{
					y = 57;
					if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)
					{
						x = 338;
					}
					else
					{
						x = 450;
					}
				}
				this.actPlayInfo.t進行描画(x, y);
			}
		}
		protected void t進行描画_背景()
		{
			if (CDTXMania.Instance.ConfigIni.eDark == Eダークモード.OFF)
			{
				if (this.tx背景 != null)
				{
					this.tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);
				}
			}
		}

		protected void t進行描画_判定ライン()
		{
			if (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)
			{
				int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.DRUMS, bReverse[(int)E楽器パート.DRUMS], false, true);	// -(int) ( 3 * Scale.Y );
				// #31602 2016.2.11 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
				// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
				if (this.txヒットバー != null)
				{
					int xStart = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 32 * 3 : 619;
					int xEnd = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 335 * 3 : 619 + 682;
					for (int x = xStart; x < xEnd; x += 24)
					{
						this.txヒットバー.t2D描画(CDTXMania.Instance.Device,
							x,
							y,
							new Rectangle(
								0,
								0,
								((x + 24) >= xEnd) ? (int)((23 - ((x + 24) - xEnd))) : 24,
								18
							)
						);
					}
				}
			}
		}
		protected void t進行描画_判定文字列()
		{
			this.actJudgeString.t進行描画(演奏判定ライン座標);
		}
		protected void t進行描画_判定文字列1_通常位置指定の場合()
		{
			if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) != E判定文字表示位置.コンボ下)	// 判定ライン上または横
			{
				this.actJudgeString.t進行描画(演奏判定ライン座標);
			}
		}
		protected void t進行描画_判定文字列2_判定ライン上指定の場合()
		{
			if (((E判定文字表示位置)CDTXMania.Instance.ConfigIni.判定文字表示位置.Drums) == E判定文字表示位置.コンボ下)	// 判定ライン上または横
			{
				this.actJudgeString.t進行描画(演奏判定ライン座標);
			}
		}

		protected void t進行描画_譜面スクロール速度()
		{
			this.act譜面スクロール速度.On進行描画();
		}

		private void t背景テクスチャの生成()
		{
			string DefaultBgFilename;
			string DefaultLaneFilename;
			Rectangle bgrect;
			string bgfilename = "";

			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				bgrect = new Rectangle((int)(181 * Scale.X), (int)(50 * Scale.Y), (int)(278 * Scale.X), (int)(355 * Scale.Y));
				DefaultBgFilename = @"Graphics\ScreenPlayGuitar background.png";
				DefaultLaneFilename = "";
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
					bgfilename = CDTXMania.Instance.DTX.strフォルダ名 + BACKGROUND;
				}
			}
			else
			{
				if (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left)
				{
					bgrect = new Rectangle((int)(338 * Scale.X), (int)(57 * Scale.Y), (int)(278 * 2), (int)(355 * 2));
				}
				else
				{
					bgrect = new Rectangle(619 + 682, (int)(57 * Scale.Y), (int)(278 * 2), (int)(355 * 2));
				}
				DefaultBgFilename = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ?
											@"Graphics\ScreenPlayDrums background.png" :
											@"Graphics\ScreenPlayDrums-background-center.png";
				DefaultLaneFilename = "";	//  @"Graphics\ScreenPlayDrums_Lane_parts_drums.png";
				if (((CDTXMania.Instance.DTX.BACKGROUND != null) && (CDTXMania.Instance.DTX.BACKGROUND.Length > 0)) &&
					!CDTXMania.Instance.ConfigIni.bストイックモード)
				{
					bgfilename = CDTXMania.Instance.DTX.strフォルダ名 + CDTXMania.Instance.DTX.BACKGROUND;
				}
			}

			// Default...: レーン等があるレイヤー		bgfilename: DTXファイルで指定する背景
			Bitmap image = null;
			bool bSuccessLoadDTXbgfile = false;

			int[] offsetX = new int[2] { 96, 506 };
			int nLanePosition = (int)CDTXMania.Instance.ConfigIni.eドラムレーン表示位置;

			if (bgfilename != null && File.Exists(bgfilename) && !CDTXMania.Instance.DTX.bチップがある.Movie)
			{
				try
				{
					#region [ DTXデータで指定されている背景画像を読み込む ]
					Bitmap bitmap1 = null;
					bitmap1 = new Bitmap(bgfilename);
					if ((bitmap1.Size.Width == 0) && (bitmap1.Size.Height == 0))
					{
						this.tx背景 = null;
						return;
					}
					#endregion

					int newWidth = (int)(bitmap1.Width * Scale.X);
					int newHeight = (int)(bitmap1.Height * Scale.Y);
					Bitmap bitmap2;

					#region [ 背景画像がVGAサイズ以下なら、FullHDサイズに拡大する ]
					if (bitmap1.Width <= 640 && bitmap1.Height <= 480)
					{
						bitmap2 = new Bitmap(newWidth, newHeight);
						Graphics graphic2 = Graphics.FromImage(bitmap2);
						graphic2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						graphic2.DrawImage(bitmap1, 0, 0, newWidth, newHeight);
						graphic2.Dispose();
					}
					else
					{
						bitmap2 = (Bitmap)bitmap1.Clone();
					}
					bitmap1.Dispose();
					#endregion

					#region [ 実背景に格子状に配置するよう、コピーしていく ]
					Bitmap bitmap3 = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height);
					Graphics graphics3 = Graphics.FromImage(bitmap3);
					for (int i = 0; i < SampleFramework.GameWindowSize.Height; i += bitmap2.Size.Height)
					{
						for (int j = 0; j < SampleFramework.GameWindowSize.Width; j += bitmap2.Size.Width)
						{
							graphics3.DrawImage(bitmap2, j, i, bitmap2.Width, bitmap2.Height);
						}
					}
					graphics3.Dispose();
					bitmap2.Dispose();
					#endregion

					#region [ レーン外・レーンそのもののフレームを合成 ]
					image = new Bitmap(CSkin.Path(DefaultBgFilename));	// レーン外のフレーム
					graphics3 = Graphics.FromImage(image);

					ColorMatrix matrix2 = new ColorMatrix();
					matrix2.Matrix00 = 1f;
					matrix2.Matrix11 = 1f;
					matrix2.Matrix22 = 1f;
					matrix2.Matrix33 = ((float)CDTXMania.Instance.ConfigIni.n背景の透過度) / 255f;
					matrix2.Matrix44 = 1f;
					ColorMatrix newColorMatrix = matrix2;
					ImageAttributes imageAttr = new ImageAttributes();
					imageAttr.SetColorMatrix(newColorMatrix);
					graphics3.DrawImage(bitmap3, new Rectangle(0, 0, SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height), 0, 0, SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height, GraphicsUnit.Pixel, imageAttr);
					bitmap3.Dispose();
					#endregion

					imageAttr.Dispose();
					graphics3.Dispose();
					bSuccessLoadDTXbgfile = true;
				}
				catch
				{
					Trace.TraceError("背景画像とレーン画像の合成に失敗しました。({0})", bgfilename);
				}
			}
			#region [ DTXデータで指定する背景画像を合成しない場合は、レーン画像単体を背景画像とする ]
			if (!bSuccessLoadDTXbgfile)
			{
				bgfilename = CSkin.Path(DefaultBgFilename);
				try
				{
					image = new Bitmap(bgfilename);

					if (DefaultLaneFilename != "")
					{
						Bitmap bmLane = new Bitmap(CSkin.Path(DefaultLaneFilename));
						Graphics g = Graphics.FromImage(image);
						g.DrawImage(bmLane, offsetX[nLanePosition], 0);
						g.Dispose();
						bmLane.Dispose();
					}
				}
				catch
				{
					Trace.TraceError("レーン画像の読み込みに失敗しました。({0})", bgfilename);
					this.tx背景 = null;
					return;
				}
			}
			#endregion
			#region [ BGA画像を表示する予定がある場合は、背景画像からあらかじめその領域を黒抜きにしておく ]
			if ((CDTXMania.Instance.DTX.listBMP.Count > 0) || (CDTXMania.Instance.DTX.listBMPTEX.Count > 0) || CDTXMania.Instance.DTX.listAVI.Count > 0)
			{
				Graphics graphics2 = Graphics.FromImage(image);
				graphics2.FillRectangle(Brushes.Black, bgrect.X, bgrect.Y, bgrect.Width, bgrect.Height);
				graphics2.Dispose();
			}
			#endregion
			#region [ 背景画像をテクスチャにする。背景動画の表示予定がある場合は、更に透明度を付与する。 ]
			try
			{
				this.tx背景 = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);
				if (CDTXMania.Instance.DTX.bMovieをFullscreen再生する)						// Fullscreen動画再生が発生する場合は、動画レイヤーに対してレーン＋背景レイヤーに透明度を設定する
				{
					this.tx背景.n透明度 = 255 - CDTXMania.Instance.ConfigIni.n背景の透過度;	// 背景動画用
				}
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("背景テクスチャの生成に失敗しました。");
				this.tx背景 = null;
			}
			#endregion
			image.Dispose();
		}

		protected virtual void t入力処理_ギターベース(E楽器パート inst)
		{
			int indexInst = (int)inst;
			#region [ スクロール速度変更 ]
			if (CDTXMania.Instance.Pad.b押されている(inst, Eパッド.Decide) && CDTXMania.Instance.Pad.b押された(inst, Eパッド.B))
			{
				float f = (float)this.演奏判定ライン座標.nJudgeLinePosY_delta[indexInst] / (CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] + 1);
				CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] = Math.Min(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] + 1, 1999);
				f *= CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst];
				this.演奏判定ライン座標.nJudgeLinePosY_delta[indexInst] = (int)(f + 0.5);
				CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[indexInst] = (int)(f + 0.5);
			}
			if (CDTXMania.Instance.Pad.b押されている(inst, Eパッド.Decide) && CDTXMania.Instance.Pad.b押された(inst, Eパッド.R))
			{
				CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] = Math.Max(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] - 1, 0);

				float f = (float)this.演奏判定ライン座標.nJudgeLinePosY_delta[indexInst] / (CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] + 1);
				CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] = Math.Max(CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst] - 1, 0);
				f *= CDTXMania.Instance.ConfigIni.n譜面スクロール速度[indexInst];
				this.演奏判定ライン座標.nJudgeLinePosY_delta[indexInst] = (int)(f + 0.5);
				CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[indexInst] = (int)(f + 0.5);
			}
			#endregion

			if (!CDTXMania.Instance.ConfigIni.bGuitar有効 || !CDTXMania.Instance.DTX.bチップがある[indexInst])
			{
				return;
			}

			int R = (inst == E楽器パート.GUITAR) ? 0 : 3;
			int G = R + 1;
			int B = R + 2;
			bool autoW = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtW : bIsAutoPlay.BsW;
			bool autoR = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtR : bIsAutoPlay.BsR;
			bool autoG = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtG : bIsAutoPlay.BsG;
			bool autoB = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtB : bIsAutoPlay.BsB;
			bool autoPick = (inst == E楽器パート.GUITAR) ? bIsAutoPlay.GtPick : bIsAutoPlay.BsPick;
			int nAutoW = (autoW) ? 8 : 0;
			int nAutoR = (autoR) ? 4 : 0;
			int nAutoG = (autoG) ? 2 : 0;
			int nAutoB = (autoB) ? 1 : 0;
			int nAutoMask = nAutoW | nAutoR | nAutoG | nAutoB;

			CChip chip = this.r次に来る指定楽器Chipを更新して返す(inst);
			if (chip != null)
			{
				if ((chip.bGuitarBass_R) && autoR)
				{
					this.actLaneFlushGB.Start(R);
					this.actRGB.Push(R);
				}
				if ((chip.bGuitarBass_G) && autoG)
				{
					this.actLaneFlushGB.Start(G);
					this.actRGB.Push(G);
				}
				if ((chip.bGuitarBass_B) && autoB)
				{
					this.actLaneFlushGB.Start(B);
					this.actRGB.Push(B);
				}
			}

			int pressingR = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.R) ? 4 : 0;
			this.t入力メソッド記憶(inst);
			int pressingG = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.G) ? 2 : 0;
			this.t入力メソッド記憶(inst);
			int pressingB = CDTXMania.Instance.Pad.b押されている(inst, Eパッド.B) ? 1 : 0;
			this.t入力メソッド記憶(inst);
			int pressingRGB = pressingR | pressingG | pressingB;
			if (pressingR != 0)
			{
				this.actLaneFlushGB.Start(R);
				this.actRGB.Push(R);
			}
			if (pressingG != 0)
			{
				this.actLaneFlushGB.Start(G);
				this.actRGB.Push(G);
			}
			if (pressingB != 0)
			{
				this.actLaneFlushGB.Start(B);
				this.actRGB.Push(B);
			}
			// auto pickだとここから先に行かないので注意
			List<STInputEvent> events = CDTXMania.Instance.Pad.GetEvents(inst, Eパッド.Pick);
			if ((events != null) && (events.Count > 0))
			{
				foreach (STInputEvent eventPick in events)
				{
					if (!eventPick.b押された)
					{
						continue;
					}
					this.t入力メソッド記憶(inst);
					long nTime = eventPick.nTimeStamp - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
					Ech定義 chWailingSound = (inst == E楽器パート.GUITAR) ? Ech定義.Guitar_WailingSound : Ech定義.Bass_WailingSound;
					CChip pChip = this.r指定時刻に一番近い未ヒットChip(nTime, chWailingSound, this.nInputAdjustTimeMs[indexInst], CDTXMania.Instance.nPoor範囲ms + 1);	// E楽器パート.GUITARなチップ全てにヒットする
					E判定 e判定 = this.e指定時刻からChipのJUDGEを返す(nTime, pChip, this.nInputAdjustTimeMs[indexInst]);
					//Trace.TraceInformation("ch={0:x2}, mask1={1:x1}, mask2={2:x2}", pChip.nチャンネル番号,  ( pChip.nチャンネル番号 & ~nAutoMask ) & 0x0F, ( flagRGB & ~nAutoMask) & 0x0F );
					if (
						(pChip != null) &&
						((((int)pChip.eチャンネル番号 & ~nAutoMask) & 0x0F) == ((pressingRGB & ~nAutoMask) & 0x0F)) &&
						(e判定 != E判定.Miss))
					{
						bool bChipHasR = pChip.bGuitarBass_R;
						bool bChipHasG = pChip.bGuitarBass_G;
						bool bChipHasB = pChip.bGuitarBass_B;
						bool bChipHasW = pChip.bGuitarBass_Wailing;
						bool bChipIsO = pChip.bGuitarBass_Open;
						bool bSuccessOPEN = bChipIsO && (autoR || pressingR == 0) && (autoG || pressingG == 0) && (autoB || pressingB == 0);
						if ((bChipHasR && (autoR || pressingR != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(R, 演奏判定ライン座標);
						}
						if ((bChipHasG && (autoG || pressingG != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(G, 演奏判定ライン座標);
						}
						if ((bChipHasB && (autoB || pressingB != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(B, 演奏判定ライン座標);
						}
						this.tチップのヒット処理(nTime, pChip);
						this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.nシステム時刻, inst, CDTXMania.Instance.ConfigIni.n手動再生音量, CDTXMania.Instance.ConfigIni.b演奏音を強調する[indexInst], e判定 == E判定.Poor);
						Ech定義 chWailingChip = (inst == E楽器パート.GUITAR) ? Ech定義.Guitar_Wailing : Ech定義.Bass_Wailing;
						CChip item = this.r指定時刻に一番近い未ヒットChip(nTime, chWailingChip, this.nInputAdjustTimeMs[indexInst], 140);
						if (item != null)
						{
							this.queWailing[indexInst].Enqueue(item);
						}
						continue;
					}

					// 以下、間違いレーンでのピック時
					CChip NoChipPicked = (inst == E楽器パート.GUITAR) ? this.r現在の空うちギターChip : this.r現在の空うちベースChip;
					if ((NoChipPicked != null) || ((NoChipPicked = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, chWailingSound, this.nInputAdjustTimeMs[indexInst])) != null))
					{
						this.tサウンド再生(NoChipPicked, CSound管理.rc演奏用タイマ.nシステム時刻, inst, CDTXMania.Instance.ConfigIni.n手動再生音量, CDTXMania.Instance.ConfigIni.b演奏音を強調する[indexInst], true);
					}
					if (!CDTXMania.Instance.ConfigIni.bLight[indexInst])
					{
						this.tチップのヒット処理_BadならびにTight時のMiss(inst);
					}
				}
			}
			List<STInputEvent> list = CDTXMania.Instance.Pad.GetEvents(inst, Eパッド.Wail);
			if ((list != null) && (list.Count > 0))
			{
				foreach (STInputEvent eventWailed in list)
				{
					if (!eventWailed.b押された)
					{
						continue;
					}
					DoWailingFromQueue(inst, eventWailed.nTimeStamp, autoW);
				}
			}
		}

		private void DoWailingFromQueue(E楽器パート inst, long nTimeStamp_Wailed, bool autoW)
		{
			int indexInst = (int)inst;
			long nTimeWailed = nTimeStamp_Wailed - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
			CChip chipWailing;
			while ((this.queWailing[indexInst].Count > 0) && ((chipWailing = this.queWailing[indexInst].Dequeue()) != null))
			{
				if ((nTimeWailed - chipWailing.n発声時刻ms) <= 1000)		// #24245 2011.1.26 yyagi: 800 -> 1000
				{
					chipWailing.bHit = true;
					this.actWailingBonus.Start(inst, this.r現在の歓声Chip[indexInst]);
					if (!autoW)
					{
						int nCombo = (this.actCombo.n現在のコンボ数[indexInst] < 500) ? this.actCombo.n現在のコンボ数[indexInst] : 500;
						this.actScore.Add(inst, bIsAutoPlay, nCombo * 3000L);		// #24245 2011.1.26 yyagi changed DRUMS->BASS, add nCombo conditions
					}
				}
			}
		}
		private void t進行描画_ギターベースフレーム()
		{
			if (!CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				if (((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.HALF) && (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)) &&
					CDTXMania.Instance.ConfigIni.bGuitar有効)
				{
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1521 : 1450;
						for (int i = 0; i < 355; i += 0x80)
						{
							Rectangle rc = new Rectangle(0, 0, 327, 288);
							if ((i + 0x80) > 355)
							{
								rc.Height -= (int)((i + 0x80 - 355) * Scale.Y);
							}
							if (this.txレーンフレームGB != null)
							{
								this.txレーンフレームGB.t2D描画(
									CDTXMania.Instance.Device,
									x,
									(57 + i) * Scale.Y,
									rc
								);
							}
						}
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1194 : 200;
						for (int i = 0; i < 355; i += 0x80)
						{
							Rectangle rc = new Rectangle(0, 0, 327, 288);
							if ((i + 0x80) > 355)
							{
								rc.Height -= (int)((i + 0x80 - 355) * Scale.Y);
							}
							if (this.txレーンフレームGB != null)
							{
								this.txレーンフレームGB.t2D描画(CDTXMania.Instance.Device,
									x,
									(57 + i) * Scale.Y,
									rc
								);
							}
						}
					}
				}
			}
		}
		private void t進行描画_グラフ()
		{
			if (!CDTXMania.Instance.ConfigIni.bストイックモード && !CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである && CDTXMania.Instance.ConfigIni.bGraph.Drums)
			{
				this.actGraph.On進行描画();
			}
		}

		private void t進行描画_チップファイアD()
		{
			this.actChipFireD.On進行描画();
		}

		private void t進行描画_ドラムパッド()
		{
			if (CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL)
			{
				this.actPad.On進行描画();
			}
		}

		// t入力処理・ドラム()からメソッドを抽出したもの。
		/// <summary>
		/// chipArrayの中を, n発生位置の小さい順に並べる + nullを大きい方に退かす。セットでe判定Arrayも並べ直す。
		/// </summary>
		/// <param name="chipArray">ソート対象chip群</param>
		/// <param name="e判定Array">ソート対象e判定群</param>
		/// <param name="NumOfChips">チップ数</param>
		private static void SortChipsByNTime(CChip[] chipArray, E判定[] e判定Array, int NumOfChips)
		{
			for (int i = 0; i < NumOfChips - 1; i++)
			{
				for (int j = NumOfChips - 1; j > i; j--)
				{
					if ((chipArray[j - 1] == null) || ((chipArray[j] != null) && (chipArray[j - 1].n発声位置 > chipArray[j].n発声位置)))
					{
						// swap
						CChip chipTemp = chipArray[j - 1];
						chipArray[j - 1] = chipArray[j];
						chipArray[j] = chipTemp;
						E判定 e判定Temp = e判定Array[j - 1];
						e判定Array[j - 1] = e判定Array[j];
						e判定Array[j] = e判定Temp;
					}
				}
			}
		}
		private bool tドラムヒット処理(long nHitTime, Eパッド type, CChip pChip, int n強弱度合い0to127)
		{
			if (pChip == null)
			{
				return false;
			}
			// Ech定義 channel = pChip.eチャンネル番号;
			int index = -1;
			if (pChip.bDrums可視チップ_LP_LBD含まない)
			{
				index = pChip.nDrumsIndex;
			}
			else if (pChip.bDrums不可視チップ_LP_LBD含まない)
			{
				index = pChip.nDrumsIndexHidden;
			}
			int nLane = CStage演奏画面共通.nチャンネル0Atoレーン07[index];
			int nPad = this.nチャンネル0Atoパッド08[index];
			bool bPChipIsAutoPlay = bIsAutoPlay[nLane];
			int nInputAdjustTime = bPChipIsAutoPlay ? 0 : this.nInputAdjustTimeMs.Drums;
			E判定 e判定 = this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, nInputAdjustTime);
			if (e判定 == E判定.Miss)
			{
				return false;
			}
			this.tチップのヒット処理(nHitTime, pChip);
			this.actLaneFlushD.Start((Eレーン)nLane, ((float)n強弱度合い0to127) / 127f);
			this.actPad.Hit(nPad);
			if ((e判定 != E判定.Poor) && (e判定 != E判定.Miss))
			{
				bool flag = this.bフィルイン中;
				bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである(pChip);
				// bool flag3 = flag2;
				// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
				this.actChipFireD.Start((Eレーン)nLane, flag, flag2, flag2, 演奏判定ライン座標.nJudgeLinePosY_delta.Drums);
			}
			if (CDTXMania.Instance.ConfigIni.bドラム打音を発声する)
			{
				CChip rChip = null;
				bool bIsChipsoundPriorToPad = true;
				if (((type == Eパッド.HH) || (type == Eパッド.HHO)) || (type == Eパッド.LC))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if ((type == Eパッド.LT) || (type == Eパッド.FT))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				else if ((type == Eパッド.CY) || (type == Eパッド.RD))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY == E打ち分け時の再生の優先順位.ChipがPadより優先;
				}
				if (bIsChipsoundPriorToPad)
				{
					rChip = pChip;
				}
				else
				{
					Eパッド hH = type;
					if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (type == Eパッド.HHO))
					{
						hH = Eパッド.HH;
					}
					if (!CDTXMania.Instance.DTX.bチップがある.Ride && (type == Eパッド.RD))
					{
						hH = Eパッド.CY;
					}
					if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (type == Eパッド.LC))
					{
						hH = Eパッド.HH;
					}
					rChip = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nHitTime, this.nパッド0Atoチャンネル0A[(int)hH], nInputAdjustTime);
					if (rChip == null)
					{
						rChip = pChip;
					}
				}
				this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, E楽器パート.DRUMS, CDTXMania.Instance.ConfigIni.n手動再生音量, CDTXMania.Instance.ConfigIni.b演奏音を強調する.Drums);
			}
			return true;
		}

		protected bool bフィルイン区間の最後のChipである(CChip pChip)
		{
			if (pChip == null)
			{
				return false;
			}
			int num = pChip.n発声位置;
			for (int i = CDTXMania.Instance.DTX.listChip.IndexOf(pChip) + 1; i < CDTXMania.Instance.DTX.listChip.Count; i++)
			{
				pChip = CDTXMania.Instance.DTX.listChip[i];
				if ((pChip[Ech定義.FillIn]) && (pChip.n整数値 == 2))
				{
					return true;
				}
				if ((pChip.bDrums可視チップ_LP_LBD含まない) && (pChip.n発声位置 - num) > 0x18)
				{
					return false;
				}
			}
			return true;
		}

		private void t進行描画_チップ_フィルイン(ref CChip pChip)
		{
			if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
			{
				pChip.bHit = true;

				if (CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					switch (pChip.n整数値)
					{
						case 0x01:	// フィルイン開始
							if (CDTXMania.Instance.ConfigIni.bフィルイン有効)
							{
								this.bフィルイン中 = true;
							}
							break;

						case 0x02:	// フィルイン終了
							if (CDTXMania.Instance.ConfigIni.bフィルイン有効)
							{
								this.bフィルイン中 = false;
							}
							if (((this.actCombo.n現在のコンボ数.Drums > 0) || CDTXMania.Instance.ConfigIni.bドラムが全部オートプレイである) && CDTXMania.Instance.ConfigIni.b歓声を発声する)
							{
								if (this.r現在の歓声Chip.Drums != null)
								{
									CDTXMania.Instance.DTX.tチップの再生(this.r現在の歓声Chip.Drums, CSound管理.rc演奏用タイマ.nシステム時刻, (int)Eレーン.BGM, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.UNKNOWN));
								}
								else
								{
									CDTXMania.Instance.Skin.sound歓声音.n位置_次に鳴るサウンド = 0;
									CDTXMania.Instance.Skin.sound歓声音.t再生する();
								}
							}
							break;
#if TEST_NOTEOFFMODE	// 2011.1.1 yyagi TEST
								case 0x04:	// HH消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = true;
									break;
								case 0x05:	// HH消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.HH = false;
									break;
								case 0x06:	// ギター消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Gutiar = true;
									break;
								case 0x07:	// ギター消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Gutiar = false;
									break;
								case 0x08:	// ベース消音あり(従来同等)
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = true;
									break;
								case 0x09:	// ベース消音無し
									CDTXMania.Instance.DTX.b演奏で直前の音を消音する.Bass = false;
									break;
#endif
					}
				}
			}
		}
		private void t進行描画_チップ_ドラムス(ref CChip pChip)
		{
			if (CDTXMania.Instance.ConfigIni.bDrums有効)
			{
				#region [ Invisible処理 ]
				if (CDTXMania.Instance.ConfigIni.eInvisible.Drums != EInvisible.OFF)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				#endregion
				else
				{
					#region [ Sudden処理 ]
					if (CDTXMania.Instance.ConfigIni.bSudden.Drums)
					{
						if (pChip.nバーからの距離dot.Drums < 200 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff;
						}
						else if (pChip.nバーからの距離dot.Drums < 250 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = 0xff - ((int)((((double)(pChip.nバーからの距離dot.Drums - 200 * Scale.Y)) * 255.0) / 50.0));
						}
						else
						{
							pChip.b可視 = false;
							pChip.n透明度 = 0;
						}
					}
					#endregion
					#region [ Hidden処理 ]
					if (CDTXMania.Instance.ConfigIni.bHidden.Drums)
					{
						if (pChip.nバーからの距離dot.Drums < 100 * Scale.Y)
						{
							pChip.b可視 = false;
						}
						else if (pChip.nバーからの距離dot.Drums < 150 * Scale.Y)
						{
							pChip.b可視 = true;
							pChip.n透明度 = (int)((((double)(pChip.nバーからの距離dot.Drums - 100 * Scale.Y)) * 255.0) / 50.0);
						}
					}
					#endregion
				}
				if (!pChip.bHit && pChip.b可視)
				{
					if (this.txチップ != null)
					{
						this.txチップ.n透明度 = pChip.n透明度;
					}
					int x = this.nチャンネルtoX座標[(int)CDTXMania.Instance.ConfigIni.eドラムレーン表示位置, pChip.eチャンネル番号 - Ech定義.HiHatClose];
					int y = CDTXMania.Instance.ConfigIni.bReverse.Drums ?
						(int)(0x38 * Scale.Y + pChip.nバーからの距離dot.Drums) :
						(int)(0x1a6 * Scale.Y - pChip.nバーからの距離dot.Drums);

					if (this.txチップ != null)
					{
						double d = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1.0 : 0.75;
						this.txチップ.vc拡大縮小倍率 = new Vector3(
														(float)(pChip.dbチップサイズ倍率 * d),
														(float)pChip.dbチップサイズ倍率,
														1f
													);
					}
					int num9 = this.ctチップ模様アニメ.Drums.n現在の値;
					switch (pChip.eチャンネル番号)
					{
						// HH
						case Ech定義.HiHatClose:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(44 * Scale.X), (int)(num9 * (18 - 2)), (int)(32 * Scale.X), (int)(18)));
							}
							break;

						// SD
						case Ech定義.Snare:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0x4c * Scale.X), (int)(num9 * (18 - 2)), (int)(0x20 * Scale.X), (int)(18)));
							}
							break;

						// BD
						case Ech定義.BassDrum:
							x += (int)(0x16 * Scale.X) - ((int)((44.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 11, new Rectangle((int)(0 * Scale.X), (int)(num9 * (22 - 2)), (int)(44 * Scale.X), (int)(22)));
							}
							break;

						// HT
						case Ech定義.HighTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0x6c * Scale.X), (int)(num9 * (18 - 2)), (int)(0x20 * Scale.X), (int)(18)));
							}
							break;

						// LT
						case Ech定義.LowTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(140 * Scale.X), (int)(num9 * (18 - 2)), (int)(0x20 * Scale.X), (int)(18)));
							}
							break;

						// CY
						case Ech定義.Cymbal:
							x += (int)(0x13 * Scale.X) - ((int)((38.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 46, new Rectangle((int)(0xcc * Scale.X), (int)(786), (int)(0x26 * Scale.X), (int)(78)));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0xcc * Scale.X), (int)(num9 * (18 - 2)), (int)(0x26 * Scale.X), (int)(18)));
							}
							break;

						// FT
						case Ech定義.FloorTom:
							x += (int)(0x10 * Scale.X) - ((int)((32.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0xac * Scale.X), (int)(num9 * (18 - 2)), (int)(0x20 * Scale.X), (int)(18)));
							}
							break;

						// HHO
						case Ech定義.HiHatOpen:
							x += (int)(13 * Scale.X) - ((int)((26.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0xf2 * Scale.X), (int)(num9 * (18 - 2)), (int)(0x1a * Scale.X), (int)(18)));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 16, new Rectangle((int)(0xf2 * Scale.X), (int)(790), (int)(0x1a * Scale.X), (int)(30)));
							}
							break;

						// RCY
						case Ech定義.RideCymbal:
							x += (int)(13 * Scale.X) - ((int)((26.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0xf2 * Scale.X), (int)(num9 * (18 - 2)), (int)(0x1a * Scale.X), (int)(18)));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 16, new Rectangle((int)(0xf2 * Scale.X), (int)(790), (int)(0x1a * Scale.X), (int)(30)));
							}
							break;

						// LCY
						case Ech定義.LeftCymbal:
							x += (int)(0x13 * Scale.X) - ((int)((38.0 * pChip.dbチップサイズ倍率 * Scale.X) / 2.0));
							if (this.txチップ != null)
							{
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 46, new Rectangle((int)(0xcc * Scale.X), (int)(786), (int)(0x26 * Scale.X), (int)(78)));
								this.txチップ.t2D描画(CDTXMania.Instance.Device, x, y - 9, new Rectangle((int)(0xcc * Scale.X), (int)(num9 * (18 - 2)), (int)(0x26 * Scale.X), (int)(18)));
							}
							break;
					}
					if (this.txチップ != null)
					{
						this.txチップ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
						this.txチップ.n透明度 = 0xff;
					}
				}

				int indexSevenLanes = CStage演奏画面共通.nチャンネル0Atoレーン07[pChip.eチャンネル番号 - Ech定義.HiHatClose];
				// #35411 chnmr0 modified
				bool autoPlayCondition = (CDTXMania.Instance.ConfigIni.bAutoPlay[indexSevenLanes] && !pChip.bHit);
				bool UsePerfectGhost = true;
				long ghostLag = 0;

				if (CDTXMania.Instance.ConfigIni.eAutoGhost.Drums != EAutoGhostData.PERFECT &&
						CDTXMania.Instance.DTX.listAutoGhostLag.Drums != null &&
						0 <= pChip.n楽器パートでの出現順 && pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listAutoGhostLag.Drums.Count)
				{
					// ゴーストデータが有効 : ラグに合わせて判定
					ghostLag = CDTXMania.Instance.DTX.listAutoGhostLag.Drums[pChip.n楽器パートでの出現順];
					ghostLag = (ghostLag & 255) - 128;
					ghostLag -= this.nInputAdjustTimeMs.Drums;
					autoPlayCondition &= !pChip.bHit && (ghostLag + pChip.n発声時刻ms <= CSound管理.rc演奏用タイマ.n現在時刻ms);
					UsePerfectGhost = false;
				}
				if (UsePerfectGhost)
				{
					// 従来の AUTO : バー下で判定
					autoPlayCondition &= (pChip.nバーからの距離dot.Drums < 0);
				}

				if (autoPlayCondition)
				{
					pChip.bHit = true;
					this.actLaneFlushD.Start((Eレーン)indexSevenLanes, ((float)CInput管理.n通常音量) / 127f);
					bool flag = this.bフィルイン中;
					bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである(pChip);
					//bool flag3 = flag2;
					// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
					this.actChipFireD.Start((Eレーン)indexSevenLanes, flag, flag2, flag2, 演奏判定ライン座標.nJudgeLinePosY_delta.Drums);
					this.actPad.Hit(this.nチャンネル0Atoパッド08[pChip.eチャンネル番号 - Ech定義.HiHatClose]);
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms + ghostLag, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
					this.tチップのヒット処理(pChip.n発声時刻ms + ghostLag, pChip);
					cInvisibleChip.StartSemiInvisible(E楽器パート.DRUMS);
				}
				// #35411 modify end

				// #35411 2015.08.21 chnmr0 add
				// 目標値グラフにゴーストの達成率を渡す
				if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums != ETargetGhostData.NONE &&
						CDTXMania.Instance.DTX.listTargetGhsotLag.Drums != null)
				{
					double val = 0;
					if (CDTXMania.Instance.ConfigIni.eTargetGhost.Drums == ETargetGhostData.ONLINE)
					{
						if (CDTXMania.Instance.DTX.n可視チップ数.Drums > 0)
						{
							// Online Stats の計算式
							val = 100 *
									(this.nヒット数_TargetGhost.Drums.Perfect * 17 +
									 this.nヒット数_TargetGhost.Drums.Great * 7 +
									 this.n最大コンボ数_TargetGhost.Drums * 3) / (20.0 * CDTXMania.Instance.DTX.n可視チップ数.Drums);
						}
					}
					else
					{
						val = CScoreIni.t演奏型スキルを計算して返す(
								CDTXMania.Instance.DTX.n可視チップ数.Drums,
								this.nヒット数_TargetGhost.Drums.Perfect,
								this.nヒット数_TargetGhost.Drums.Great,
								this.nヒット数_TargetGhost.Drums.Good,
								this.nヒット数_TargetGhost.Drums.Poor,
								this.nヒット数_TargetGhost.Drums.Miss,
								E楽器パート.DRUMS, new STAUTOPLAY());
					}
					if (val < 0) val = 0;
					if (val > 100) val = 100;
					this.actGraph.dbグラフ値目標_渡 = val;
				}
				return;
			}
			else
			{
				if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, E楽器パート.DRUMS, CDTXMania.Instance.DTX.nモニタを考慮した音量(E楽器パート.DRUMS));
					pChip.bHit = true;
				}
			}
		}
		private void t進行描画_チップ_ギターベース(ref CChip pChip, E楽器パート inst)
		{
			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{

				t進行描画_チップ_ギターベース(ref pChip, inst,
					演奏判定ライン座標.n判定ラインY座標(inst, false),	// 40
					演奏判定ライン座標.n判定ラインY座標(inst, true),	// 369
					(int)(0 * Scale.Y), (int)(409 * Scale.Y),				// Y軸表示範囲
					26, 480,					// openチップのX座標(Gt, Bs)
					0, 192, 103, 8, 32,			// オープンチップregionの x, y, w, h, 通常チップのw
					26, 98, 480, 552,			// GtのX, Gt左利きのX, BsのX, Bs左利きのX,
					36, 32						// 描画のX座標間隔, テクスチャのX座標間隔
				);
			}
			else
			{
				int xGtO = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 509 : 485;
				int xBsO = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 400 : 69;
				int xGtL = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 561 : 537;
				int xBsL = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 452 : 121;

				t進行描画_チップ_ギターベース(ref pChip, inst,
					演奏判定ライン座標.n判定ラインY座標(inst, false),		// 95  Normal
					演奏判定ライン座標.n判定ラインY座標(inst, true),		// 374 Reverse
					(int)(57 * Scale.Y), (int)(412 * Scale.Y),				// チップのY軸表示範囲
					xGtO, xBsO,					// openチップのX座標(Gt, Bs)
					268, 144, 76, 6, 24,		// オープンチップregionの x, y, w, h, 通常チップのw
					xGtO, xGtL, xBsO, xBsL,		// GtのX, Gt左利きのX, BsのX, Bs左利きのX,
					26, 24						// 描画のX座標間隔, テクスチャのX座標間隔
				);
			}
		}
		private void t進行描画_ギターベース判定ライン()		// yyagi: ギタレボモードとは座標が違うだけですが、まとめづらかったのでそのまま放置してます。
		{
			if (CDTXMania.Instance.ConfigIni.bギタレボモード)
			{
				if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
				{
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						int x = 23 * 3;
						int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, bReverse[(int)E楽器パート.GUITAR], false, true) - (int)(3 * Scale.Y);
						// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						if (this.txヒットバー != null)
						{
							for (int i = 0; i < 4; i++)
							{
								this.txヒットバー.t2D描画(CDTXMania.Instance.Device,
									x + (28 * i) * Scale.X,
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
						int x = 477 * 3;
						int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, bReverse[(int)E楽器パート.BASS], false, true) - (int)(3 * Scale.Y);
						// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						if (this.txヒットバー != null)
						{
							for (int j = 0; j < 4; j++)
							{
								this.txヒットバー.t2D描画(
									CDTXMania.Instance.Device,
									x + (28 * j) * Scale.X,
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
			else
			{
				if ((CDTXMania.Instance.ConfigIni.eDark != Eダークモード.FULL) && CDTXMania.Instance.ConfigIni.bGuitar有効)
				{
					if (CDTXMania.Instance.DTX.bチップがある.Guitar)
					{
						int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1527 : 1456;
						int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.GUITAR, bReverse[(int)E楽器パート.GUITAR], false, true) - (int)(3 * Scale.Y);
						// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						Rectangle rc = new Rectangle(0, 0, 30, 36);
						if (this.txヒットバーGB != null)
						{
							for (int i = 0; i < 3; i++)
							{
								this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
									x + (3 * 26 * i),
									y
								);
								this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
									x + (3 * 26 * i) + 48,
									y,
									rc
								);
							}
						}
					}
					if (CDTXMania.Instance.DTX.bチップがある.Bass)
					{
						int x = (CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Left) ? 1200 : 206;
						int y = this.演奏判定ライン座標.n判定ラインY座標(E楽器パート.BASS, bReverse[(int)E楽器パート.BASS], false, true) - (int)(3 * Scale.Y);
						// #31602 2013.6.23 yyagi 描画遅延対策として、判定ラインの表示位置をオフセット調整できるようにする
						Rectangle rc = new Rectangle(0, 0, 30, 36);
						if (this.txヒットバーGB != null)
						{
							for (int j = 0; j < 3; j++)
							{
								this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
									x + (3 * 26 * j),
									y
								);
								this.txヒットバーGB.t2D描画(CDTXMania.Instance.Device,
									x + (3 * 26 * j) + 48,
									y,
									rc
								);
							}
						}
					}
				}
			}
		}
	}
}
