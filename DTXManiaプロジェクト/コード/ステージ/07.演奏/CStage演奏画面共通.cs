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
		#region [Member_Activities]
		CAct演奏AVI actAVI;
		CAct演奏BGA actBGA;
		CAct演奏チップファイアGB actChipFireGB;
		CAct演奏Combo共通 actCombo;
		CAct演奏Danger共通 actDANGER;
		CActFIFOBlack actFI;
		CActFIFOBlack actFO;
		CActFIFOWhite actFOClear;
		CAct演奏ゲージ共通 actGauge;
		CAct演奏判定文字列共通 actJudgeString;
		CAct演奏DrumsレーンフラッシュD actLaneFlushD;
		CAct演奏レーンフラッシュGB共通 actLaneFlushGB;
		CActオプションパネル actOptionPanel;
		CAct演奏パネル文字列 actPanel;
		CAct演奏演奏情報 actPlayInfo;
		CAct演奏RGB共通 actRGB;
		CAct演奏スコア共通 actScore;
		CAct演奏ステージ失敗 actStageFailed;
		CAct演奏WailingBonus共通 actWailingBonus;
		CAct演奏スクロール速度 act譜面スクロール速度;
		CAct演奏DrumsチップファイアD actChipFireD;
		// #24074 2011.01.23 add ikanick
		CAct演奏グラフ actGraph;
		CAct演奏Drumsパッド actPad;
		CAct演奏Lane actLane;
		#endregion

		bool bPAUSE;
		STDGBSValue<bool> b演奏にMIDI入力を使った;
		STDGBSValue<bool> b演奏にキーボードを使った;
		STDGBSValue<bool> b演奏にジョイパッドを使った;
		STDGBSValue<bool> b演奏にマウスを使った;

		CCounter ctWailingチップ模様アニメ;
		STDGBSValue<CCounter> ctチップ模様アニメ;

		E演奏画面の戻り値 eフェードアウト完了時の戻り値;

		STDGBSValue<CHitCountOfRank> nヒット数_Auto含まない;
		STDGBSValue<CHitCountOfRank> nヒット数_Auto含む;
		// #35411 2015.08.21 chnmr0 add
		STDGBSValue<CHitCountOfRank> nヒット数_TargetGhost;
		STDGBSValue<int> nコンボ数_TargetGhost;
		STDGBSValue<int> n最大コンボ数_TargetGhost;
		int n現在のトップChip = -1;
		int[] n最後に再生したBGMの実WAV番号 = new int[50];
		EChannel e最後に再生したHHのチャンネル番号;
		// #23921 2011.1.4 yyagi: change "int" to "List<int>", for recording multiple wav No.
		List<int> L最後に再生したHHの実WAV番号;
		// #26388 2011.11.8 yyagi: change "n最後に再生した実WAV番号.GUITAR" and "n最後に再生した実WAV番号.BASS"
		STPadValue<int> n最後に再生した実WAV番号;
		// #24820 2013.1.21 yyagi まずは単純にAdd/Removeを1個のキューでまとめて管理するやり方で設計する
		volatile Queue<STMixer> queueMixerSound;
		DateTime dtLastQueueOperation;
		bool bIsDirectSound;
		double db再生速度;
		bool bValidScore;
		bool bフィルイン中; // drums only

		STDGBSValue<Queue<CChip>> queWailing;
		STDGBSValue<CChip> r現在の歓声Chip;
		STPadValue<CChip> r空打ちドラムチップ = new STPadValue<CChip>();
		CChip r現在の空うちギターChip;
		ST空打ち r現在の空うちドラムChip;
		CChip r現在の空うちベースChip;
		CChip r次にくるギターChip;
		CChip r次にくるベースChip;

		CTexture txWailing枠;
		CTexture txチップ;
		CTexture txチップGB;
		CTexture txヒットバー;
		CTexture txヒットバーGB;
		CTexture tx背景;

		CInvisibleChip cInvisibleChip;
		CWailingChip共通[] cWailingChip;

		STDGBSValue<CScoreIni.C演奏記録> record;

		public CStage演奏画面共通()
		{
			base.eステージID = CStage.Eステージ.演奏;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actPad = new CAct演奏Drumsパッド());
			base.list子Activities.Add(this.actChipFireD = new CAct演奏DrumsチップファイアD());
			base.list子Activities.Add(this.actChipFireGB = new CAct演奏チップファイアGB());
			base.list子Activities.Add(this.actStageFailed = new CAct演奏ステージ失敗());
			base.list子Activities.Add(this.actDANGER = new CAct演奏Danger共通());
			base.list子Activities.Add(this.actAVI = new CAct演奏AVI());
			base.list子Activities.Add(this.actBGA = new CAct演奏BGA());
			base.list子Activities.Add(this.actPanel = new CAct演奏パネル文字列());
			base.list子Activities.Add(this.act譜面スクロール速度 = new CAct演奏スクロール速度());
			base.list子Activities.Add(this.actWailingBonus = new CAct演奏WailingBonus共通());
			base.list子Activities.Add(this.actScore = new CAct演奏スコア共通());
			base.list子Activities.Add(this.actRGB = new CAct演奏RGB共通());
			base.list子Activities.Add(this.actLaneFlushD = new CAct演奏DrumsレーンフラッシュD());
			base.list子Activities.Add(this.actLaneFlushGB = new CAct演奏レーンフラッシュGB共通());
			base.list子Activities.Add(this.actJudgeString = new CAct演奏判定文字列共通());
			base.list子Activities.Add(this.actGauge = new CAct演奏ゲージ共通());
			base.list子Activities.Add(this.actGraph = new CAct演奏グラフ());
			base.list子Activities.Add(this.actCombo = new CAct演奏Combo共通());
			base.list子Activities.Add(this.actPlayInfo = new CAct演奏演奏情報());
			base.list子Activities.Add(this.actFI = new CActFIFOBlack());
			base.list子Activities.Add(this.actFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actFOClear = new CActFIFOWhite());
			base.list子Activities.Add(this.actLane = new CAct演奏Lane());
			list子Activities.Add(actOptionPanel = new CActオプションパネル(EOptionPanelDirection.Vertical));
		}

		#region [ PlayRecordSave ]
		private void t演奏結果を格納する_ドラム()
		{
			record.Drums = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Drums = Record.Drums;
			bool allauto = CDTXMania.Instance.ConfigIni.bIsAutoPlay(EPart.Drums);
			if (CDTXMania.Instance.DTX.bチップがある.Drums && CDTXMania.Instance.ConfigIni.bDrums有効)
			{
				Drums.nスコア = (long)this.actScore.Get(EPart.Drums);
				Drums.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Drums, CDTXMania.Instance.DTX.n可視チップ数.Drums, this.nヒット数_Auto含まない.Drums.Perfect,
						this.actCombo.dgbコンボ数.Drums.n最高値,
						EPart.Drums);
				Drums.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Drums, this.nヒット数_Auto含まない.Drums.Perfect, this.nヒット数_Auto含まない.Drums.Great, this.nヒット数_Auto含まない.Drums.Good, this.nヒット数_Auto含まない.Drums.Poor, this.nヒット数_Auto含まない.Drums.Miss, EPart.Drums, true);
				Drums.nPerfect数 = allauto ? this.nヒット数_Auto含む.Drums.Perfect : this.nヒット数_Auto含まない.Drums.Perfect;
				Drums.nGreat数 = allauto ? this.nヒット数_Auto含む.Drums.Great : this.nヒット数_Auto含まない.Drums.Great;
				Drums.nGood数 = allauto ? this.nヒット数_Auto含む.Drums.Good : this.nヒット数_Auto含まない.Drums.Good;
				Drums.nPoor数 = allauto ? this.nヒット数_Auto含む.Drums.Poor : this.nヒット数_Auto含まない.Drums.Poor;
				Drums.nMiss数 = allauto ? this.nヒット数_Auto含む.Drums.Miss : this.nヒット数_Auto含まない.Drums.Miss;
				Drums.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Drums.Perfect;
				Drums.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Drums.Great;
				Drums.nGood数_Auto含まない = this.nヒット数_Auto含まない.Drums.Good;
				Drums.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Drums.Poor;
				Drums.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Drums.Miss;
				Drums.n最大コンボ数 = this.actCombo.dgbコンボ数.Drums.n最高値;
				Drums.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Drums;
				for ( EPad i = EPad.Min; i < EPad.BassPadMax; i++ )
				{
					Drums.bAutoPlay[i] = CDTXMania.Instance.ConfigIni.bAutoPlay[i];
				}
				Drums.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					Drums.eSudHidInv[i] = CDTXMania.Instance.ConfigIni.eSudHidInv[i];
					Drums.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Drums.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Drums.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Drums.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.nScrollSpeed[i] + 1)) * 0.5f;
				}
				Drums.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Drums.n演奏速度分子 = CDTXMania.Instance.ConfigIni.nPlaySpeed;
				Drums.n演奏速度分母 = 20;
				Drums.e譜面レーンタイプ.Drums = CDTXMania.Instance.DTX.n使用レーン数.Drums;
				Drums.e譜面レーンタイプ.Guitar= CDTXMania.Instance.DTX.n使用レーン数.Guitar;
				Drums.e譜面レーンタイプ.Bass  = CDTXMania.Instance.DTX.n使用レーン数.Bass;
				Drums.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup.Value;
				Drums.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup.Value;
				Drums.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup.Value;
				Drums.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH.Value;
				Drums.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT.Value;
				Drums.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY.Value;
				Drums.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Drums.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Drums.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bStageFailed;
				Drums.eダメージレベル = CDTXMania.Instance.ConfigIni.eDamageLevel;
				Drums.eMetronome = CDTXMania.Instance.ConfigIni.eClickType;
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
				// #35461 chnmr0 add
				Drums.nRisky = CDTXMania.Instance.ConfigIni.nRisky;
				// #35417 chnmr0 add
				Drums.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass;
			}
		}
		private void t演奏結果を格納する_ギター()
		{
			record.Guitar = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Guitar = Record.Guitar;
			bool allauto = CDTXMania.Instance.ConfigIni.bIsAutoPlay(EPart.Guitar);
			if (CDTXMania.Instance.DTX.bチップがある.Guitar)
			{
				Guitar.nスコア = (long)this.actScore.Get(EPart.Guitar);
				Guitar.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Guitar, CDTXMania.Instance.DTX.n可視チップ数.Guitar, this.nヒット数_Auto含まない.Guitar.Perfect, this.actCombo.dgbコンボ数.Guitar.n最高値, EPart.Guitar);
				Guitar.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Guitar, this.nヒット数_Auto含まない.Guitar.Perfect, this.nヒット数_Auto含まない.Guitar.Great, this.nヒット数_Auto含まない.Guitar.Good, this.nヒット数_Auto含まない.Guitar.Poor, this.nヒット数_Auto含まない.Guitar.Miss, EPart.Guitar, true);
				Guitar.nPerfect数 = allauto ? this.nヒット数_Auto含む.Guitar.Perfect : this.nヒット数_Auto含まない.Guitar.Perfect;
				Guitar.nGreat数 = allauto ? this.nヒット数_Auto含む.Guitar.Great : this.nヒット数_Auto含まない.Guitar.Great;
				Guitar.nGood数 = allauto ? this.nヒット数_Auto含む.Guitar.Good : this.nヒット数_Auto含まない.Guitar.Good;
				Guitar.nPoor数 = allauto ? this.nヒット数_Auto含む.Guitar.Poor : this.nヒット数_Auto含まない.Guitar.Poor;
				Guitar.nMiss数 = allauto ? this.nヒット数_Auto含む.Guitar.Miss : this.nヒット数_Auto含まない.Guitar.Miss;
				Guitar.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Perfect;
				Guitar.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Great;
				Guitar.nGood数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Good;
				Guitar.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Poor;
				Guitar.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Guitar.Miss;
				Guitar.n最大コンボ数 = this.actCombo.dgbコンボ数.Guitar.n最高値;
				Guitar.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Guitar;
				for (EPad i = EPad.Min; i < EPad.BassPadMax; i++)
				{
					Guitar.bAutoPlay[i] = CDTXMania.Instance.ConfigIni.bAutoPlay[i];
				}
				Guitar.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					Guitar.eSudHidInv[i] = CDTXMania.Instance.ConfigIni.eSudHidInv[i];
					Guitar.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Guitar.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Guitar.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Guitar.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.nScrollSpeed[i] + 1)) * 0.5f;
				}
				Guitar.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Guitar.n演奏速度分子 = CDTXMania.Instance.ConfigIni.nPlaySpeed;
				Guitar.n演奏速度分母 = 20;
				Guitar.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				Guitar.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				Guitar.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;
				Guitar.e譜面レーンタイプ.Drums = CDTXMania.Instance.DTX.n使用レーン数.Drums;
				Guitar.e譜面レーンタイプ.Guitar = CDTXMania.Instance.DTX.n使用レーン数.Guitar;
				Guitar.e譜面レーンタイプ.Bass = CDTXMania.Instance.DTX.n使用レーン数.Bass;
				Guitar.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH;
				Guitar.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT;
				Guitar.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY;
				Guitar.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Guitar.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Guitar.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bStageFailed;
				Guitar.eダメージレベル = CDTXMania.Instance.ConfigIni.eDamageLevel;
				Guitar.eMetronome = CDTXMania.Instance.ConfigIni.eClickType;
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
				// #35417 chnmr0 add
				Guitar.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass;
			}
		}
		private void t演奏結果を格納する_ベース()
		{
			record.Bass = new CScoreIni.C演奏記録();
			CScoreIni.C演奏記録 Bass = Record.Bass;
			bool allauto = CDTXMania.Instance.ConfigIni.bIsAutoPlay(EPart.Bass);
			if (CDTXMania.Instance.DTX.bチップがある.Bass)
			{
				Bass.nスコア = (long)this.actScore.Get(EPart.Bass);
				Bass.dbゲーム型スキル値 = CScoreIni.tゲーム型スキルを計算して返す(CDTXMania.Instance.DTX.LEVEL.Bass, CDTXMania.Instance.DTX.n可視チップ数.Bass, this.nヒット数_Auto含まない.Bass.Perfect, this.actCombo.dgbコンボ数.Bass.n最高値, EPart.Bass);
				Bass.db演奏型スキル値 = CScoreIni.t演奏型スキルを計算して返す(CDTXMania.Instance.DTX.n可視チップ数.Bass, this.nヒット数_Auto含まない.Bass.Perfect, this.nヒット数_Auto含まない.Bass.Great, this.nヒット数_Auto含まない.Bass.Good, this.nヒット数_Auto含まない.Bass.Poor, this.nヒット数_Auto含まない.Bass.Miss, EPart.Bass, true);
				Bass.nPerfect数 = allauto ? this.nヒット数_Auto含む.Bass.Perfect : this.nヒット数_Auto含まない.Bass.Perfect;
				Bass.nGreat数 = allauto ? this.nヒット数_Auto含む.Bass.Great : this.nヒット数_Auto含まない.Bass.Great;
				Bass.nGood数 = allauto ? this.nヒット数_Auto含む.Bass.Good : this.nヒット数_Auto含まない.Bass.Good;
				Bass.nPoor数 = allauto ? this.nヒット数_Auto含む.Bass.Poor : this.nヒット数_Auto含まない.Bass.Poor;
				Bass.nMiss数 = allauto ? this.nヒット数_Auto含む.Bass.Miss : this.nヒット数_Auto含まない.Bass.Miss;
				Bass.nPerfect数_Auto含まない = this.nヒット数_Auto含まない.Bass.Perfect;
				Bass.nGreat数_Auto含まない = this.nヒット数_Auto含まない.Bass.Great;
				Bass.nGood数_Auto含まない = this.nヒット数_Auto含まない.Bass.Good;
				Bass.nPoor数_Auto含まない = this.nヒット数_Auto含まない.Bass.Poor;
				Bass.nMiss数_Auto含まない = this.nヒット数_Auto含まない.Bass.Miss;
				Bass.n最大コンボ数 = this.actCombo.dgbコンボ数.Bass.n最高値;
				Bass.n全チップ数 = CDTXMania.Instance.DTX.n可視チップ数.Bass;
				for (EPad i = EPad.Min; i < EPad.BassPadMax; i++)
				{
					Bass.bAutoPlay[i] = CDTXMania.Instance.ConfigIni.bAutoPlay[i];
				}
				Bass.bTight = CDTXMania.Instance.ConfigIni.bTight;
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					Bass.eSudHidInv[i] = CDTXMania.Instance.ConfigIni.eSudHidInv[i];
					Bass.bReverse[i] = CDTXMania.Instance.ConfigIni.bReverse[i];
					Bass.eRandom[i] = CDTXMania.Instance.ConfigIni.eRandom[i];
					Bass.bLight[i] = CDTXMania.Instance.ConfigIni.bLight[i];
					Bass.f譜面スクロール速度[i] = ((float)(CDTXMania.Instance.ConfigIni.nScrollSpeed[i] + 1)) * 0.5f;
				}
				Bass.eDark = CDTXMania.Instance.ConfigIni.eDark;
				Bass.n演奏速度分子 = CDTXMania.Instance.ConfigIni.nPlaySpeed;
				Bass.n演奏速度分母 = 20;
				Bass.eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				Bass.eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				Bass.eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;
				Bass.e譜面レーンタイプ.Drums = CDTXMania.Instance.DTX.n使用レーン数.Drums;
				Bass.e譜面レーンタイプ.Guitar = CDTXMania.Instance.DTX.n使用レーン数.Guitar;
				Bass.e譜面レーンタイプ.Bass = CDTXMania.Instance.DTX.n使用レーン数.Bass;
				Bass.eHitSoundPriorityHH = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH;
				Bass.eHitSoundPriorityFT = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT;
				Bass.eHitSoundPriorityCY = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY;
				Bass.bGuitar有効 = CDTXMania.Instance.ConfigIni.bGuitar有効;
				Bass.bDrums有効 = CDTXMania.Instance.ConfigIni.bDrums有効;
				Bass.bSTAGEFAILED有効 = CDTXMania.Instance.ConfigIni.bStageFailed;
				Bass.eダメージレベル = CDTXMania.Instance.ConfigIni.eDamageLevel;
				// #24280 2011.1.29 yyagi
				Bass.eMetronome = CDTXMania.Instance.ConfigIni.eClickType;
				Bass.b演奏にキーボードを使用した = this.b演奏にキーボードを使った.Bass;
				Bass.b演奏にMIDI入力を使用した = this.b演奏にMIDI入力を使った.Bass;
				Bass.b演奏にジョイパッドを使用した = this.b演奏にジョイパッドを使った.Bass;
				Bass.b演奏にマウスを使用した = this.b演奏にマウスを使った.Bass;
				Bass.nPerfectになる範囲ms = CDTXMania.Instance.nPerfect範囲ms;
				Bass.nGreatになる範囲ms = CDTXMania.Instance.nGreat範囲ms;
				Bass.nGoodになる範囲ms = CDTXMania.Instance.nGood範囲ms;
				Bass.nPoorになる範囲ms = CDTXMania.Instance.nPoor範囲ms;
				Bass.strDTXManiaのバージョン = CDTXMania.VERSION;
				Bass.最終更新日時 = DateTime.Now.ToString();
				// #35417 chnmr0 add
				Bass.bギターとベースを入れ替えた = CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass;
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
				n最後に再生した実WAV番号 = new STPadValue<int>();
				this.L最後に再生したHHの実WAV番号 = new List<int>(16);
				this.e最後に再生したHHのチャンネル番号 = 0;
				this.n最後に再生した実WAV番号.GtPick = -1;
				this.n最後に再生した実WAV番号.BsPick = -1;
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
				this.n最大コンボ数_TargetGhost = new STDGBSValue<int>();

				for (EPart k = EPart.Drums; k <= EPart.Bass; k++)
				{
					this.nヒット数_Auto含まない[k] = new CHitCountOfRank();
					this.nヒット数_Auto含む[k] = new CHitCountOfRank();
					this.nヒット数_TargetGhost[k] = new CHitCountOfRank();
					this.queWailing[k] = new Queue<CChip>();
					this.r現在の歓声Chip[k] = null;
					if (CDTXMania.Instance.DTXVmode.Enabled)
					{
						CDTXMania.Instance.ConfigIni.nScrollSpeed[k] = CDTXMania.Instance.ConfigIni.nViewerScrollSpeed[k];
					}

					this.b演奏にキーボードを使った[k] = false;
					this.b演奏にジョイパッドを使った[k] = false;
					this.b演奏にMIDI入力を使った[k] = false;
					this.b演奏にマウスを使った[k] = false;
				}

				cInvisibleChip.Reset();
				actGauge.Init(CDTXMania.Instance.ConfigIni.nRisky);
				// 効果音のストリームをミキサーから解除しておく
				CDTXMania.Instance.Skin.tRemoveMixerAll();
				queueMixerSound = new Queue<STMixer>(64);
				bIsDirectSound = (CDTXMania.Instance.Sound管理.GetCurrentSoundDeviceType() == "DirectSound");
				this.bPAUSE = false;

				if (CDTXMania.Instance.DTXVmode.Enabled)
				{
					db再生速度 = CDTXMania.Instance.DTX.dbDTXVPlaySpeed;
					CDTXMania.Instance.ConfigIni.nPlaySpeed.Value = (int)(CDTXMania.Instance.DTX.dbDTXVPlaySpeed * 20 + 0.5);
				}
				else
				{
					db再生速度 = ((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0;
				}

				bValidScore = (CDTXMania.Instance.DTXVmode.Enabled) ? false : true;

				cWailingChip = new CWailingChip共通[3];
				// 0:未使用, 1:Gutiar, 2:Bass
				cWailingChip[1] = new CWailingChip共通();
				cWailingChip[2] = new CWailingChip共通();

				#region [ 演奏開始前にmixer登録しておくべきサウンド(開幕してすぐに鳴らすことになるチップ音)を登録しておく ]
				foreach (CChip pChip in CDTXMania.Instance.DTX.listChip)
				{
					//				Debug.WriteLine( "CH=" + pChip.nチャンネル番号.ToString( "x2" ) + ", 整数値=" + pChip.n整数値 +  ", time=" + pChip.n発声時刻ms );
					if (pChip.n発声時刻ms <= 0)
					{
						if (pChip.eチャンネル番号 == EChannel.MixerAdd)
						{
							pChip.bHit = true;
							//						Trace.TraceInformation( "first [DA] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値 + ", time=" + pChip.n発声時刻ms );
							if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))
							{
								CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
								for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPolyphonicSounds; i++)
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

				if (CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass)  // #24063 2011.1.24 yyagi Gt/Bsの譜面情報入れ替え
				{
					CDTXMania.Instance.DTX.SwapGuitarBassInfos();
				}


				for (EPart inst = EPart.Drums; inst <= EPart.Bass; ++inst)
				{
					// MODIFY_BEGIN #25398 2011.06.07 FROM
					if (CDTXMania.Instance.bコンパクトモード)
					{
						var score = new Cスコア();
						CDTXMania.Instance.Songs管理.tScoreIniを読み込んで譜面情報を設定する(CDTXMania.Instance.strコンパクトモードファイル + ".score.ini", ref score);
						this.actGraph.dbTarget[inst] = score.譜面情報.最大スキル[inst];
					}
					else
					{
						if (CDTXMania.Instance.ConfigIni.b楽器有効(inst))
						{
							this.actGraph.dbTarget[inst] = CDTXMania.Instance.stage選曲.r確定されたスコア.譜面情報.最大スキル[inst];   // #24074 2011.01.23 add ikanick

							// ゴースト利用可のなとき、0で初期化
							if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] != ETargetGhostData.None)
							{
								if (CDTXMania.Instance.DTX.listTargetGhsotLag[inst] != null)
								{
									this.actGraph.dbTarget[inst] = 0;
								}
							}
						}
					}
					// MODIFY_END #25398
				}

				for (EPart part = EPart.Drums; part <= EPart.Bass; ++part)
				{
					if (CDTXMania.Instance.ConfigIni.b楽器有効(part))
					{
						actOptionPanel.Pos[part] = new Coordinates.CXY(CDTXMania.Instance.ConfigIni.cdInstX[part][CDTXMania.Instance.ConfigIni.eActiveInst]
							- CDTXMania.Instance.Coordinates.ImgOptionPanel.W, 0);
					}
				}

				base.On活性化();

				// PANELの設定は、base.On活性化()の後に(actPanelの活性化の後)行うこと。
				// さもないと、actPanelが活性化されていないため、パネル文字列の設定が機能しなくなる。
				string strLabel = (CDTXMania.Instance.stage選曲.r確定された曲 == null)?
					null : CDTXMania.Instance.stage選曲.r確定された曲.ar難易度ラベル[ CDTXMania.Instance.stage選曲.n確定された曲の難易度 ];
				string strPanel = CDTXMania.Instance.DTX.TITLE;
				//string strSETDEFlabel = CDTXMania.Instance.stage選曲.r確定された曲.strタイトル;
				if ( !string.IsNullOrWhiteSpace( strLabel ) && !strPanel.ToLower().Contains(strLabel.ToLower() )
				//	&& (strPanel == strSETDEFlabel)
				)
				{
					strPanel += " (" + strLabel + ")";
				}
				this.actPanel.SetPanelString( strPanel );
			}
		}
		public override void On非活性化()
		{
			if (b活性化してる)
			{
				// #23921 2011.1.4 yyagi
				L最後に再生したHHの実WAV番号.Clear();
				L最後に再生したHHの実WAV番号 = null;
				for (EPart i = 0; i <= EPart.Bass; i++)
				{
					queWailing[i].Clear();
					queWailing[i] = null;
				}
				ctWailingチップ模様アニメ = null;
				ctチップ模様アニメ.Drums = null;
				ctチップ模様アニメ.Guitar = null;
				ctチップ模様アニメ.Bass = null;
				queueMixerSound.Clear();
				queueMixerSound = null;
				cInvisibleChip.Dispose();
				cInvisibleChip = null;
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				txチップ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums chips.png"));
				txチップGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayGuitar chips.png"));
				txヒットバー = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar.png"));
				txヒットバーGB = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlayDrums hit-bar guitar.png"));
				txWailing枠 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay wailing cursor.png"));
				t背景テクスチャの生成();
				base.OnManagedリソースの作成();
			}

		}
		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.tx背景);
				TextureFactory.tテクスチャの解放(ref this.txWailing枠);
				TextureFactory.tテクスチャの解放(ref this.txヒットバーGB);
				TextureFactory.tテクスチャの解放(ref this.txヒットバー);
				TextureFactory.tテクスチャの解放(ref this.txチップGB);
				TextureFactory.tテクスチャの解放(ref this.txチップ);
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if (b活性化してる)
			{
				bool bIsFinishedPlaying = false;
				bool bIsFinishedFadeout = false;

				if (b初めての進行描画)
				{
					PrepareAVITexture();

					CSound管理.rc演奏用タイマ.tリセット();
					CDTXMania.Instance.Timer.tリセット();
					if (CDTXMania.Instance.ConfigIni.bDrums有効)
					{
						ctチップ模様アニメ.Drums = new CCounter(0, 48, 10, CDTXMania.Instance.Timer);
					}
					if (CDTXMania.Instance.ConfigIni.bGuitar有効)
					{
						ctチップ模様アニメ.Guitar = new CCounter(0, 48, 20, CDTXMania.Instance.Timer);
						ctチップ模様アニメ.Bass = new CCounter(0, 48, 20, CDTXMania.Instance.Timer);
						ctWailingチップ模様アニメ = new CCounter(0, 4, 50, CDTXMania.Instance.Timer);
					}

					// this.actChipFireD.Start( Eレーン.HH );	// #31554 2013.6.12 yyagi
					// 初チップヒット時のもたつき回避。最初にactChipFireD.Start()するときにJITが掛かって？
					// ものすごく待たされる(2回目以降と比べると2,3桁tick違う)。そこで最初の画面フェードインの間に
					// 一発Start()を掛けてJITの結果を生成させておく。

					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					actFI.tフェードイン開始();

					if (CDTXMania.Instance.DTXVmode.Enabled)
					{
						#region [ DTXV用の再生設定にする(全AUTOなど) ]
						tDTXV用の設定();
						#endregion
						t演奏位置の変更(CDTXMania.Instance.DTXVmode.nStartBar);
					}

					CDTXMania.Instance.Sound管理.tDisableUpdateBufferAutomatically();
					b初めての進行描画 = false;
				}

				#region [ stage failed ]
				if (CDTXMania.Instance.ConfigIni.bStageFailed && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態))
				{
					if (actGauge.IsFailed)
					{
						actStageFailed.Start();
						CDTXMania.Instance.DTX.t全チップの再生停止();
						base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED;
					}
				}
				#endregion

				// Background
				if (CDTXMania.Instance.ConfigIni.eDark == EDark.Off)
				{
					if (tx背景 != null)
					{
						tx背景.t2D描画(CDTXMania.Instance.Device, 0, 0);
					}
				}

				// AVI / BGA
				if (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED &&
					base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)
				{
					actAVI.t進行描画(
						CDTXMania.Instance.ConfigIni.cdMovieX[CDTXMania.Instance.ConfigIni.eActiveInst],
						CDTXMania.Instance.ConfigIni.cdMovieY[CDTXMania.Instance.ConfigIni.eActiveInst],
						CDTXMania.Instance.Coordinates.Movie.W,
						CDTXMania.Instance.Coordinates.Movie.H);
					actBGA.On進行描画();
					#region [MIDIBGM 処理?]
					if (base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED)
					{
						CStage.Eフェーズ eフェーズid1 = base.eフェーズID;
					}
					#endregion
					actLane.On進行描画();
					actLaneFlushGB.On進行描画();
					actPanel.On進行描画();
					actScore.On進行描画();
					actOptionPanel.On進行描画();
					actGauge.On進行描画();
					actGraph.On進行描画();
					actLaneFlushD.On進行描画();
					actDANGER.t進行描画(actGauge.IsDanger);
					act譜面スクロール速度.On進行描画();
					t進行描画_判定ライン();
					actWailingBonus.On進行描画();

					// RGB, Judge, Combo (Priority Under Chip)
					if (CDTXMania.Instance.ConfigIni.eJudgePriority == EJudgeDisplayPriority.Under)
					{
						actRGB.On進行描画();
						actJudgeString.On進行描画();
						actCombo.On進行描画();
					}
					t進行描画_チップアニメ();
					bIsFinishedPlaying = t進行描画_チップ();
					actPad.On進行描画();

					// RGB, Judge, Combo (Priority Over Chip)
					if (CDTXMania.Instance.ConfigIni.eJudgePriority == EJudgeDisplayPriority.Over)
					{
						actRGB.On進行描画();
						actJudgeString.On進行描画();
						actCombo.On進行描画();
					}

					actChipFireD.On進行描画();
					actChipFireGB.On進行描画();
					actPlayInfo.On進行描画();

					// Wailing
					if ((CDTXMania.Instance.ConfigIni.eDark != EDark.Full) &&
						CDTXMania.Instance.ConfigIni.bGuitar有効)
					{
						int GtWailingFrameX = CDTXMania.Instance.ConfigIni.GetLaneX(ELane.GtW) +
							(CDTXMania.Instance.ConfigIni.GetLaneW(ELane.GtW) - CDTXMania.Instance.Coordinates.ImgGtWailingFrame.W) / 2;
						int BsWailingFrameX = CDTXMania.Instance.ConfigIni.GetLaneX(ELane.BsW) +
							(CDTXMania.Instance.ConfigIni.GetLaneW(ELane.BsW) - CDTXMania.Instance.Coordinates.ImgGtWailingFrame.W) / 2;
						int GtWailingFrameY = C演奏判定ライン座標共通.n判定ラインY座標(EPart.Guitar, true, true);
						int BsWailingFrameY = C演奏判定ライン座標共通.n判定ラインY座標(EPart.Bass, true, true);

						if (txWailing枠 != null)
						{
							if (CDTXMania.Instance.DTX.bチップがある.Guitar)
							{
								txWailing枠.t2D描画(CDTXMania.Instance.Device, GtWailingFrameX, GtWailingFrameY);
							}
							if (CDTXMania.Instance.DTX.bチップがある.Bass)
							{
								txWailing枠.t2D描画(CDTXMania.Instance.Device, BsWailingFrameX, BsWailingFrameY);
							}
						}
					}
				}

				// Stage Failed
				if (((base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED) ||
					(base.eフェーズID == CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)) &&
					((actStageFailed.On進行描画() != 0) &&
					(base.eフェーズID != CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト)))
				{
					eフェードアウト完了時の戻り値 = E演奏画面の戻り値.ステージ失敗;
					base.eフェーズID = CStage.Eフェーズ.演奏_STAGE_FAILED_フェードアウト;
					actFO.tフェードアウト開始();
				}

				bIsFinishedFadeout = this.t進行描画_フェードイン_アウト();
				if (bIsFinishedPlaying && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態))
				{
					if (CDTXMania.Instance.DTXVmode.Enabled)
					{
						if (CDTXMania.Instance.Timer.b停止していない)
						{
							actPanel.Stop();
							CDTXMania.Instance.Timer.t一時停止();
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

						if (CDTXMania.Instance.ConfigIni.bDrums有効)
						{
							for (EPad pad = EPad.DrumsPadMin; pad < EPad.DrumsPadMax; pad++)
							{
								r空打ちドラムチップ[pad] = r空うちChip(EPart.Drums, pad);
								if (r空打ちドラムチップ[pad] == null)
								{
									r空打ちドラムチップ[pad] = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(
											CSound管理.rc演奏用タイマ.n現在時刻,
											EnumConverter.ChannelFromPad(pad),
											CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums);
								}
							}
						}

						if (CDTXMania.Instance.ConfigIni.bIsSwappedGuitarBass)      // #24063 2011.1.24 yyagi Gt/Bsを入れ替えていたなら、演奏結果も入れ替える
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
		public STPadValue<CChip> GetNoChipDrums()
		{
			return r空打ちドラムチップ;
		}
		public STDGBSValue<CScoreIni.C演奏記録> Record
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
				int nInterval = ( CDTXMania.Instance.ConfigIni.bVSyncWait ) ? 7 : 1;
				int nMaxDequeueCount = ( CDTXMania.Instance.ConfigIni.bVSyncWait ) ? 2 : 1;
				if (ts.Milliseconds > nInterval)
				{
					for (int i = 0; i < nMaxDequeueCount && queueMixerSound.Count > 0; i++)
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
			if (CDTXMania.Instance.ConfigIni.bAVI)
			{
				foreach (CChip pChip in CDTXMania.Instance.DTX.listChip)
				{
					if (pChip.eチャンネル番号 == EChannel.Movie || pChip.eチャンネル番号 == EChannel.MovieFull)
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

		protected EJudge e指定時刻からChipのJUDGEを返す(long nTime, CChip pChip, int nInputAdjustTime, bool saveLag = true)
		{
			if (pChip != null)
			{
				// #35411 2015.08.22 chnmr0 modified add check save lag flag for ghost
				int lag = (int)(nTime + nInputAdjustTime - pChip.n発声時刻ms);
				if (saveLag)
				{
					pChip.nLag = lag;
					// #23580 2011.1.3 yyagi: add "nInputAdjustTime" to add input timing adjust feature
					if (pChip.e楽器パート != EPart.Unknown)
					{
						pChip.extendInfoForGhost = this.actCombo.dgbコンボ数[pChip.e楽器パート].n現在値 > 0 ? true : false;
					}
				}
				// #35411 modify end

				int nDeltaTime = Math.Abs(lag);
				//Debug.WriteLine("nAbsTime=" + (nTime - pChip.n発声時刻ms) + ", nDeltaTime=" + (nTime + nInputAdjustTime - pChip.n発声時刻ms));
				if (nDeltaTime <= CDTXMania.Instance.nPerfect範囲ms)
				{
					return EJudge.Perfect;
				}
				if (nDeltaTime <= CDTXMania.Instance.nGreat範囲ms)
				{
					return EJudge.Great;
				}
				if (nDeltaTime <= CDTXMania.Instance.nGood範囲ms)
				{
					return EJudge.Good;
				}
				if (nDeltaTime <= CDTXMania.Instance.nPoor範囲ms)
				{
					return EJudge.Poor;
				}
			}
			return EJudge.Miss;
		}

		protected CChip r空うちChip(EPart part, EPad pad)
		{
			switch (part)
			{
				case EPart.Drums:
					switch (pad)
					{
						case EPad.HH:
							if (this.r現在の空うちドラムChip.HH != null)
							{
								return this.r現在の空うちドラムChip.HH;
							}
							if (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.HO_HC)
							{
								if (CDTXMania.Instance.ConfigIni.eHHGroup == EHHGroup.LC_HH)
								{
									return this.r現在の空うちドラムChip.HHO;
								}
								if (this.r現在の空うちドラムChip.HHO != null)
								{
									return this.r現在の空うちドラムChip.HHO;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case EPad.SD:
							return this.r現在の空うちドラムChip.SD;

						case EPad.BD:
							return this.r現在の空うちドラムChip.BD;

						case EPad.HT:
							return this.r現在の空うちドラムChip.HT;

						case EPad.LT:
							if (this.r現在の空うちドラムChip.LT != null)
							{
								return this.r現在の空うちドラムChip.LT;
							}
							if (CDTXMania.Instance.ConfigIni.eFTGroup == EFTGroup.Group)
							{
								return this.r現在の空うちドラムChip.FT;
							}
							return null;

						case EPad.FT:
							if (this.r現在の空うちドラムChip.FT != null)
							{
								return this.r現在の空うちドラムChip.FT;
							}
							if (CDTXMania.Instance.ConfigIni.eFTGroup == EFTGroup.Group)
							{
								return this.r現在の空うちドラムChip.LT;
							}
							return null;

						case EPad.CY:
							if (this.r現在の空うちドラムChip.CY != null)
							{
								return this.r現在の空うちドラムChip.CY;
							}
							if (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.Group)
							{
								return this.r現在の空うちドラムChip.RD;
							}
							return null;

						case EPad.HHO:
							if (this.r現在の空うちドラムChip.HHO != null)
							{
								return this.r現在の空うちドラムChip.HHO;
							}
							if (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.HO_HC)
							{
								if (CDTXMania.Instance.ConfigIni.eHHGroup == EHHGroup.LC_HH)
								{
									return this.r現在の空うちドラムChip.HH;
								}
								if (this.r現在の空うちドラムChip.HH != null)
								{
									return this.r現在の空うちドラムChip.HH;
								}
							}
							return this.r現在の空うちドラムChip.LC;

						case EPad.RD:
							if (this.r現在の空うちドラムChip.RD != null)
							{
								return this.r現在の空うちドラムChip.RD;
							}
							if (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.Group)
							{
								return this.r現在の空うちドラムChip.CY;
							}
							return null;

						case EPad.LC:
							if (this.r現在の空うちドラムChip.LC != null)
							{
								return this.r現在の空うちドラムChip.LC;
							}
							if ((CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.HO_HC) && (CDTXMania.Instance.ConfigIni.eHHGroup != EHHGroup.Group))
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

				case EPart.Guitar:
					return this.r現在の空うちギターChip;

				case EPart.Bass:
					return this.r現在の空うちベースChip;
			}
			return null;
		}

		protected CChip r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(long nTime, EChannel eChannel, int nInputAdjustTime)
		{
			return r指定時刻に一番近い未ヒットChip(nTime, eChannel, nInputAdjustTime, 0, HitState.DontCare);
		}

		protected void tサウンド再生(CChip rChip, long n再生開始システム時刻ms, EPart part)
		{
			this.tサウンド再生(rChip, n再生開始システム時刻ms, part, CDTXMania.Instance.ConfigIni.nChipVolume);
		}

		protected void tサウンド再生(CChip pChip, long n再生開始システム時刻ms, EPart part, int n音量, bool bモニタ = false, bool b音程をずらして再生 = false)
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
					case EPart.Drums:
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
							else if (EChannel.SE24 == pChip.eチャンネル番号)  // 仮に今だけ追加 HHは消音処理があるので overwriteフラグ系の処理は改めて不要
							{
								index = 0;
							}
							else if ((EChannel.SE25 <= pChip.eチャンネル番号) && (pChip.eチャンネル番号 <= EChannel.SE27))  // 仮に今だけ追加
							{
								pChip.ConvertSE25_26_27toCY_RCY_LCY();
								index = pChip.nDrumsIndex;
								overwrite = true;
							}
							else
							{
								return;
							}
							EChannel actChannel = EChannel.HiHatClose + index;
							ELane nLane = EnumConverter.LaneFromChannel(actChannel);
							if ((nLane == ELane.HH) && // 今回演奏するのがHC or HO
									(index == 0 || (index == 7 &&
									this.e最後に再生したHHのチャンネル番号 != EChannel.HiHatOpen &&
									this.e最後に再生したHHのチャンネル番号 != EChannel.HiHatOpen_Hidden))
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
								for (int i = 0; i < this.L最後に再生したHHの実WAV番号.Count; i++)      // #23921 2011.1.4 yyagi
								{
									CDTXMania.Instance.DTX.tWavの再生停止(this.L最後に再生したHHの実WAV番号[i]);    // #23921 yyagi ストック分全て消音する
								}
								this.L最後に再生したHHの実WAV番号.Clear();
								this.e最後に再生したHHのチャンネル番号 = pChip.eチャンネル番号;
							}
							if (index == 0 || index == 7)
							{
								// #23921 HOまたは不可視HO演奏時はそのチップ番号をストックしておく
								// #24772 HC, 不可視HCも消音キューに追加
								if (this.L最後に再生したHHの実WAV番号.Count >= 16)
								{
									// #23921 ただしストック数が16以上になるようなら、頭の1個を削って常に16未満に抑える
									// (ストックが増えてList<>のrealloc()が発生するのを予防する)
									this.L最後に再生したHHの実WAV番号.RemoveAt(0);
								}
								if (!this.L最後に再生したHHの実WAV番号.Contains(pChip.n整数値_内部番号))
								{
									// チップ音がまだストックされてなければストックする								
									this.L最後に再生したHHの実WAV番号.Add(pChip.n整数値_内部番号);
								}
							}
							if (overwrite)
							{

								CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号[EnumConverter.PadFromChannel(actChannel)]);
							}
							CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, n音量, bモニタ);
							this.n最後に再生した実WAV番号[EnumConverter.PadFromLane(nLane)] = pChip.n整数値_内部番号;
							// nLaneでなくindexにすると、LC(1A-11=09)とギター(enumで09)がかぶってLC音が消されるので注意
							return;
						}
					#endregion
					case EPart.Guitar:
						#region [ GUITAR ]
						CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号.GtPick);
						CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, n音量, bモニタ, b音程をずらして再生);
						this.n最後に再生した実WAV番号.GtPick = pChip.n整数値_内部番号;
						return;
					#endregion
					case EPart.Bass:
						#region [ BASS ]
						CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生した実WAV番号.BsPick);
						CDTXMania.Instance.DTX.tチップの再生(pChip, n再生開始システム時刻ms, n音量, bモニタ, b音程をずらして再生);
						this.n最後に再生した実WAV番号.BsPick = pChip.n整数値_内部番号;
						return;
					#endregion

					default:
						break;
				}
			}
		}

		protected EJudge tチップのヒット処理(long nHitTime, CChip pChip, bool bCorrectLane = true)
		{
			pChip.bHit = true;
			if (pChip.e楽器パート != EPart.Unknown)
			{
				cInvisibleChip.StartSemiInvisible(pChip.e楽器パート);
			}
			bool bPChipIsAutoPlay = pChip.bAssignAutoPlayState();// 2011.6.10 yyagi
			EJudge eJudgeResult = EJudge.Auto;

			// ゴースト処理 #35411 2015.08.20 chnmr0
			bool bIsPerfectGhost = CDTXMania.Instance.ConfigIni.eAutoGhost[pChip.e楽器パート] == EAutoGhostData.Perfect ||
							CDTXMania.Instance.DTX.listAutoGhostLag[pChip.e楽器パート] == null;
			int nInputAdjustTime = bPChipIsAutoPlay && bIsPerfectGhost ? 0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[pChip.e楽器パート];
			eJudgeResult = (bCorrectLane) ? this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, nInputAdjustTime) : EJudge.Miss;

			if (pChip.e楽器パート != EPart.Unknown)
			{
				ELane nLane = ELane.Max;
				switch (pChip.e楽器パート)
				{
					case EPart.Drums:
						nLane = EnumConverter.LaneFromChannel(pChip.eチャンネル番号);
						break;
					case EPart.Guitar:
						nLane = ELane.GtR;
						break;
					case EPart.Bass:
						nLane = ELane.BsR;
						break;
				}
				this.actJudgeString.Start(nLane, bPChipIsAutoPlay && bIsPerfectGhost ? EJudge.Auto : eJudgeResult, pChip.nLag);
			}

			// ダメージ
			if (!bPChipIsAutoPlay && (pChip.e楽器パート != EPart.Unknown))
			{
				actGauge.Damage(pChip.e楽器パート, eJudgeResult);
			}

			// インビジブル
			if (eJudgeResult == EJudge.Poor || eJudgeResult == EJudge.Miss || eJudgeResult == EJudge.Bad)
			{
				cInvisibleChip.ShowChipTemporally(pChip.e楽器パート);
			}

			// コンボ
			if (pChip.e楽器パート != EPart.Unknown)
			{
				switch (eJudgeResult)
				{
					case EJudge.Miss:
					case EJudge.Bad:
						this.nヒット数_Auto含む[pChip.e楽器パート].Miss++;
						if (!bPChipIsAutoPlay)
						{
							this.nヒット数_Auto含まない[pChip.e楽器パート].Miss++;
						}
						break;
					default:
						// #24068 2011.1.10 ikanick changed (for Gt./Bs.)
						// #24167 2011.1.16 yyagi changed  (for Gt./Bs.)
						this.nヒット数_Auto含む[pChip.e楽器パート][(int)eJudgeResult]++;
						if (!bPChipIsAutoPlay)
						{
							this.nヒット数_Auto含まない[pChip.e楽器パート][(int)eJudgeResult]++;
						}
						break;
				}

				bool incrementCombo = false;

				if (pChip.e楽器パート == EPart.Drums)
				{
					if (CDTXMania.Instance.ConfigIni.bIsAutoPlay(EPart.Drums) || !bPChipIsAutoPlay)
					{
						// Dr. : 演奏したレーンだけコンボを増やす
						incrementCombo = true;
					}
				}
				else if (pChip.e楽器パート == EPart.Guitar || pChip.e楽器パート == EPart.Bass)
				{
					incrementCombo = true;
				}

                if( incrementCombo == true )
                {
                    switch (eJudgeResult)
                    {
                        case EJudge.Perfect:
                        case EJudge.Great:
                        case EJudge.Good:
                            this.actCombo.dgbコンボ数[pChip.e楽器パート].IncrementCombo();
                            break;

                        default:
                            this.actCombo.dgbコンボ数[pChip.e楽器パート].ResetCombo();
                            break;
                    }
                }
			}

			// スコア
			if ((!bPChipIsAutoPlay && (pChip.e楽器パート != EPart.Unknown)) && (eJudgeResult != EJudge.Miss) && (eJudgeResult != EJudge.Bad))
			{
				int nCombos = this.actCombo.dgbコンボ数[pChip.e楽器パート].n現在値;
				long nScoreDelta = 0;
				long[] nComboScoreDelta = new long[] { 350L, 200L, 50L, 0L };
				if ((nCombos <= 500) || (eJudgeResult == EJudge.Good))
				{
					nScoreDelta = nComboScoreDelta[(int)eJudgeResult] * nCombos;
				}
				else if ((eJudgeResult == EJudge.Perfect) || (eJudgeResult == EJudge.Great))
				{
					nScoreDelta = nComboScoreDelta[(int)eJudgeResult] * 500L;
				}
				this.actScore.Add(pChip.e楽器パート, nScoreDelta);
			}

			// グラフ
			if (pChip.e楽器パート != EPart.Unknown)
			{
				EPart inst = pChip.e楽器パート;
				// #24074 2011.01.23 add ikanick
				this.actGraph.dbCurrent[inst] =
						CScoreIni.t演奏型スキルを計算して返す(
						CDTXMania.Instance.DTX.n可視チップ数[inst],
						this.nヒット数_Auto含まない[inst].Perfect,
						this.nヒット数_Auto含まない[inst].Great,
						this.nヒット数_Auto含まない[inst].Good,
						this.nヒット数_Auto含まない[inst].Poor,
						this.nヒット数_Auto含まない[inst].Miss,
						inst, true);

				// #35411 2015.09.07 add chnmr0
				if (
						CDTXMania.Instance.DTX.listTargetGhsotLag[inst] != null &&
						CDTXMania.Instance.ConfigIni.eTargetGhost[inst] == ETargetGhostData.Online &&
						CDTXMania.Instance.DTX.n可視チップ数[inst] > 0)
				{
					// Online Stats の計算式
					this.actGraph.dbTarget[inst] = 100 *
													(this.nヒット数_Auto含まない[inst].Perfect * 17 +
													 this.nヒット数_Auto含まない[inst].Great * 7 +
													 this.actCombo.dgbコンボ数[inst].n最高値 * 3) /
													 (20.0 * CDTXMania.Instance.DTX.n可視チップ数[inst]);
				}
			}
			return eJudgeResult;
		}

		private void tチップのヒット処理_BadならびにTight時のMiss(EPart part, ELane nLane = ELane.LC)
		{
			cInvisibleChip.StartSemiInvisible(part);
			cInvisibleChip.ShowChipTemporally(part);
			actGauge.Damage(part, EJudge.Miss);
			switch (part)
			{
				case EPart.Drums:
					this.actJudgeString.Start(nLane, CDTXMania.Instance.ConfigIni.bAutoPlay[EnumConverter.PadFromLane(nLane)] ?
							EJudge.Auto : EJudge.Miss, 999);
					this.actCombo.dgbコンボ数.Drums.ResetCombo();
					return;

				case EPart.Guitar:
					this.actJudgeString.Start(ELane.GtR, EJudge.Bad, 999);
					this.actCombo.dgbコンボ数.Guitar.ResetCombo();
					return;

				case EPart.Bass:
					this.actJudgeString.Start(ELane.BsR, EJudge.Bad, 999);
					this.actCombo.dgbコンボ数.Bass.ResetCombo();
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
		protected CChip r指定時刻に一番近い未ヒットChip(long nTime, EChannel search, int nInputAdjustTime, int n検索範囲時間ms = 0, HitState hs = HitState.NotHit)
		{
			CChip ret = null;

			nTime += nInputAdjustTime;
			if (this.n現在のトップChip >= 0 && this.n現在のトップChip <= CDTXMania.Instance.DTX.listChip.Count)
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
							(search == EChannel.Guitar_WailingSound && chip.bGuitar可視チップ) ||
							(search == EChannel.Bass_WailingSound && chip.bBass可視チップ)
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

		protected CChip r次に来る指定楽器Chipを更新して返す(EPart inst)
		{
			CChip ret = null;
			int nInputAdjustTime;
			if (inst == EPart.Guitar)
			{
				nInputAdjustTime = CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick ?
						0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Guitar;
				ret = this.r指定時刻に一番近い未ヒットChip(CSound管理.rc演奏用タイマ.n現在時刻, EChannel.Guitar_WailingSound, nInputAdjustTime, 500);
				this.r次にくるギターChip = ret;
			}
			else if (inst == EPart.Bass)
			{
				nInputAdjustTime = CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick ?
						0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Bass;
				ret = this.r指定時刻に一番近い未ヒットChip(CSound管理.rc演奏用タイマ.n現在時刻, EChannel.Bass_WailingSound, nInputAdjustTime, 500);
				this.r次にくるベースChip = ret;
			}
			return ret;
		}

		protected void ChangeInputAdjustTimeInPlaying(IInputDevice keyboard, int plusminus)     // #23580 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
		{
			EPart part;
			int offset = plusminus;
			if (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift))  // Guitar InputAdjustTime
			{
				part = EPart.Guitar;
			}
			else if (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftAlt) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightAlt)) // Bass InputAdjustTime
			{
				part = EPart.Bass;
			}
			else
			{
				// Drums InputAdjustTime
				part = EPart.Drums;
			}
			if (!keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) && !keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl))
			{
				offset *= 10;
			}

			CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[part].Value = CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[part] + offset;
		}

		private void t入力処理_ドラム()
		{
			for (EPad ePad = EPad.DrumsPadMin; ePad < EPad.DrumsPadMax; ePad++)        // #27029 2012.1.4 from: <10 to <=10; Eパッドの要素が１つ（HP）増えたため。
																																								 //		  2012.1.5 yyagi: (int)Eパッド.MAX に変更。Eパッドの要素数への依存を無くすため。
			{
				List<STInputEvent> listInputEvent = CDTXMania.Instance.Pad.GetEvents(ePad);

				if ((listInputEvent == null) || (listInputEvent.Count == 0))
				{
					continue;
				}

				this.t入力メソッド記憶(EPart.Drums);

				#region [ 打ち分けグループ調整 ]
				//-----------------------------
				EHHGroup eHHGroup = CDTXMania.Instance.ConfigIni.eHHGroup;
				EFTGroup eFTGroup = CDTXMania.Instance.ConfigIni.eFTGroup;
				ECYGroup eCYGroup = CDTXMania.Instance.ConfigIni.eCYGroup;

				if (!CDTXMania.Instance.DTX.bチップがある.Ride && (eCYGroup == ECYGroup.Off))
				{
					eCYGroup = ECYGroup.Group;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (eHHGroup == EHHGroup.Off))
				{
					eHHGroup = EHHGroup.LC_HH;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (eHHGroup == EHHGroup.HO_HC))
				{
					eHHGroup = EHHGroup.Group;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (eHHGroup == EHHGroup.Off))
				{
					eHHGroup = EHHGroup.HO_HC;
				}
				if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (eHHGroup == EHHGroup.LC_HH))
				{
					eHHGroup = EHHGroup.Group;
				}
				//-----------------------------
				#endregion

				foreach (STInputEvent inputEvent in listInputEvent)
				{
					if (!inputEvent.b押された)
						continue;

					long nTime = inputEvent.nTimeStamp - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
					EPad nPad09 = (ePad == EPad.HP) ? EPad.BD : ePad;      // #27029 2012.1.5 yyagi
					int nInputAdjustTime = CDTXMania.Instance.ConfigIni.bAutoPlay[nPad09] ? 0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums;
					bool bHitted = false;

					#region [ (A) ヒットしていればヒット処理して次の inputEvent へ ]
					//-----------------------------
					switch (ePad)
					{
						case EPad.HH:
							#region [ HHとLC(groupingしている場合) のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HH)
									continue;   // 電子ドラムによる意図的なクロストークを無効にする

								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);    // HiHat Close
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1); // HiHat Open
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);    // LC
								EJudge e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : EJudge.Miss;
								switch (eHHGroup)
								{
									case EHHGroup.HO_HC:
										#region [ HCとLCのヒット処理 ]
										//-----------------------------
										if ((e判定HC != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
									#endregion

									case EHHGroup.LC_HH:
										#region [ HCとHOのヒット処理 ]
										//-----------------------------
										if ((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;
									//-----------------------------
									#endregion

									case EHHGroup.Group:
										#region [ HC,HO,LCのヒット処理 ]
										//-----------------------------
										if (((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss)) && (e判定LC != EJudge.Miss))
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
											this.tドラムヒット処理(nTime, EPad.HH, chipArray[0], inputEvent.nVelocity);
											if (chipArray[0].n発声位置 == chipArray[1].n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipArray[1], inputEvent.nVelocity);
											}
											if (chipArray[0].n発声位置 == chipArray[2].n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipArray[2], inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HO != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipLC, inputEvent.nVelocity);
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
										if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HH, chipHC, inputEvent.nVelocity);
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

						case EPad.SD:
							#region [ SDのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.SD)   // #23857 2010.12.12 yyagi: to support VelocityMin
								continue;   // 電子ドラムによる意図的なクロストークを無効にする
							if (!this.tドラムヒット処理(nTime, EPad.SD, this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.Snare, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								break;
							continue;
						//-----------------------------
						#endregion

						case EPad.BD:
							#region [ BDのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.BD)   // #23857 2010.12.12 yyagi: to support VelocityMin
								continue;   // 電子ドラムによる意図的なクロストークを無効にする
							if (!this.tドラムヒット処理(nTime, EPad.BD, this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.BassDrum, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								break;
							continue;
						//-----------------------------
						#endregion

						case EPad.HT:
							#region [ HTのヒット処理 ]
							//-----------------------------
							if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HT)   // #23857 2010.12.12 yyagi: to support VelocityMin
								continue;   // 電子ドラムによる意図的なクロストークを無効にする
							if (this.tドラムヒット処理(nTime, EPad.HT, this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HighTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
								continue;
							break;
						//-----------------------------
						#endregion

						case EPad.LT:
							#region [ LTとFT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.LT)   // #23857 2010.12.12 yyagi: to support VelocityMin
									continue;   // 電子ドラムによる意図的なクロストークを無効にする
								CChip chipLT = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LowTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipFT = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.FloorTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								EJudge e判定LT = (chipLT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定FT = (chipFT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT, nInputAdjustTime) : EJudge.Miss;
								switch (eFTGroup)
								{
									case EFTGroup.Off:
										#region [ LTのヒット処理 ]
										//-----------------------------
										if (e判定LT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.LT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										break;
									//-----------------------------
									#endregion

									case EFTGroup.Group:
										#region [ LTとFTのヒット処理 ]
										//-----------------------------
										if ((e判定LT != EJudge.Miss) && (e判定FT != EJudge.Miss))
										{
											if (chipLT.n発声位置 < chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.LT, chipLT, inputEvent.nVelocity);
											}
											else if (chipLT.n発声位置 > chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.LT, chipFT, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.LT, chipLT, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.LT, chipFT, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定LT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.LT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定FT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.LT, chipFT, inputEvent.nVelocity);
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

						case EPad.FT:
							#region [ FTとLT(groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.FT)   // #23857 2010.12.12 yyagi: to support VelocityMin
									continue;   // 電子ドラムによる意図的なクロストークを無効にする
								CChip chipLT = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LowTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipFT = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.FloorTom, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								EJudge e判定LT = (chipLT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLT, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定FT = (chipFT != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipFT, nInputAdjustTime) : EJudge.Miss;
								switch (eFTGroup)
								{
									case EFTGroup.Off:
										#region [ FTのヒット処理 ]
										//-----------------------------
										if (e判定FT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.FT, chipFT, inputEvent.nVelocity);
											bHitted = true;
										}
										//-----------------------------
										#endregion
										break;

									case EFTGroup.Group:
										#region [ FTとLTのヒット処理 ]
										//-----------------------------
										if ((e判定LT != EJudge.Miss) && (e判定FT != EJudge.Miss))
										{
											if (chipLT.n発声位置 < chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.FT, chipLT, inputEvent.nVelocity);
											}
											else if (chipLT.n発声位置 > chipFT.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.FT, chipFT, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.FT, chipLT, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.FT, chipFT, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定LT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.FT, chipLT, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定FT != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.FT, chipFT, inputEvent.nVelocity);
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

						case EPad.CY:
							#region [ CY(とLCとRD:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.CY)   // #23857 2010.12.12 yyagi: to support VelocityMin
									continue;   // 電子ドラムによる意図的なクロストークを無効にする
								CChip chipCY = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipRD = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = CDTXMania.Instance.ConfigIni.bCymbalFree ? this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								EJudge e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : EJudge.Miss;
								CChip[] chipArray = new CChip[] { chipCY, chipRD, chipLC };
								EJudge[] e判定Array = new EJudge[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;   // chipArray.GetLength(0)

								// CY/RD/LC群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eCYGroup)
								{
									case ECYGroup.Off:
										if (!CDTXMania.Instance.ConfigIni.bCymbalFree)
										{
											if (e判定CY != EJudge.Miss)
											{
												this.tドラムヒット処理(nTime, EPad.CY, chipCY, inputEvent.nVelocity);
												bHitted = true;
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if ((e判定Array[i] != EJudge.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipLC)))
											{
												this.tドラムヒット処理(nTime, EPad.CY, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
											//num10++;
										}
										if (e判定CY != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.CY, chipCY, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case ECYGroup.Group:
										if (!CDTXMania.Instance.ConfigIni.bCymbalFree)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != EJudge.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipRD)))
												{
													this.tドラムヒット処理(nTime, EPad.CY, chipArray[i], inputEvent.nVelocity);
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
											if (e判定Array[i] != EJudge.Miss)
											{
												this.tドラムヒット処理(nTime, EPad.CY, chipArray[i], inputEvent.nVelocity);
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

						case EPad.HHO:
							#region [ HO(とHCとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.HH)
									continue;   // 電子ドラムによる意図的なクロストークを無効にする

								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								EJudge e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : EJudge.Miss;
								switch (eHHGroup)
								{
									case EHHGroup.Off:
										if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.HO_HC:
										if ((e判定HO != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.LC_HH:
										if ((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.Group:
										if (((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss)) && (e判定LC != EJudge.Miss))
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
											this.tドラムヒット処理(nTime, EPad.HHO, chipArray[0], inputEvent.nVelocity);
											if (chipArray[0].n発声位置 == chipArray[1].n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipArray[1], inputEvent.nVelocity);
											}
											if (chipArray[0].n発声位置 == chipArray[2].n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipArray[2], inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != EJudge.Miss) && (e判定HO != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipHO.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HC != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHC.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
											}
											else if (chipHC.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if ((e判定HO != EJudge.Miss) && (e判定LC != EJudge.Miss))
										{
											if (chipHO.n発声位置 < chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											}
											else if (chipHO.n発声位置 > chipLC.n発声位置)
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											else
											{
												this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
												this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
											}
											bHitted = true;
										}
										else if (e判定HC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHC, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定HO != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipHO, inputEvent.nVelocity);
											bHitted = true;
										}
										else if (e判定LC != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.HHO, chipLC, inputEvent.nVelocity);
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

						case EPad.RD:
							#region [ RD(とCYとLC:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.RD)   // #23857 2010.12.12 yyagi: to support VelocityMin
									continue;   // 電子ドラムによる意図的なクロストークを無効にする
								CChip chipCY = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipRD = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);
								CChip chipLC = CDTXMania.Instance.ConfigIni.bCymbalFree ? this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								EJudge e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : EJudge.Miss;
								CChip[] chipArray = new CChip[] { chipCY, chipRD, chipLC };
								EJudge[] e判定Array = new EJudge[] { e判定CY, e判定RD, e判定LC };
								const int NumOfChips = 3;   // chipArray.GetLength(0)

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eCYGroup)
								{
									case ECYGroup.Off:
										if (e判定RD != EJudge.Miss)
										{
											this.tドラムヒット処理(nTime, EPad.RD, chipRD, inputEvent.nVelocity);
											bHitted = true;
										}
										break;

									case ECYGroup.Group:
										if (!CDTXMania.Instance.ConfigIni.bCymbalFree)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != EJudge.Miss) && ((chipArray[i] == chipCY) || (chipArray[i] == chipRD)))
												{
													this.tドラムヒット処理(nTime, EPad.CY, chipArray[i], inputEvent.nVelocity);
													bHitted = true;
													break;
												}
											}
											break;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if (e判定Array[i] != EJudge.Miss)
											{
												this.tドラムヒット処理(nTime, EPad.CY, chipArray[i], inputEvent.nVelocity);
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

						case EPad.LC:
							#region [ LC(とHC/HOとCYと:groupingしている場合)のヒット処理 ]
							//-----------------------------
							{
								if (inputEvent.nVelocity <= CDTXMania.Instance.ConfigIni.nVelocityMin.LC)   // #23857 2010.12.12 yyagi: to support VelocityMin
									continue;   // 電子ドラムによる意図的なクロストークを無効にする
								CChip chipHC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatClose, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);    // HC
								CChip chipHO = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.HiHatOpen, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1); // HO
								CChip chipLC = this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.LeftCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1);    // LC
								CChip chipCY = CDTXMania.Instance.ConfigIni.bCymbalFree ? this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.Cymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								CChip chipRD = CDTXMania.Instance.ConfigIni.bCymbalFree ? this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.RideCymbal, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1) : null;
								EJudge e判定HC = (chipHC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHC, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定HO = (chipHO != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipHO, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定LC = (chipLC != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipLC, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定CY = (chipCY != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipCY, nInputAdjustTime) : EJudge.Miss;
								EJudge e判定RD = (chipRD != null) ? this.e指定時刻からChipのJUDGEを返す(nTime, chipRD, nInputAdjustTime) : EJudge.Miss;
								CChip[] chipArray = new CChip[] { chipHC, chipHO, chipLC, chipCY, chipRD };
								EJudge[] e判定Array = new EJudge[] { e判定HC, e判定HO, e判定LC, e判定CY, e判定RD };
								const int NumOfChips = 5;   // chipArray.GetLength(0)

								// HH/CY群を, n発生位置の小さい順に並べる + nullを大きい方に退かす
								SortChipsByNTime(chipArray, e判定Array, NumOfChips);
								switch (eHHGroup)
								{
									case EHHGroup.Off:
									case EHHGroup.LC_HH:
										if (!CDTXMania.Instance.ConfigIni.bCymbalFree)
										{
											if (e判定LC != EJudge.Miss)
											{
												this.tドラムヒット処理(nTime, EPad.LC, chipLC, inputEvent.nVelocity);
												bHitted = true;
											}
											if (!bHitted)
												break;
											continue;
										}
										for (int i = 0; i < NumOfChips; i++)
										{
											if ((e判定Array[i] != EJudge.Miss) && (((chipArray[i] == chipLC) || (chipArray[i] == chipCY)) || ((chipArray[i] == chipRD) && (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.Group))))
											{
												this.tドラムヒット処理(nTime, EPad.LC, chipArray[i], inputEvent.nVelocity);
												bHitted = true;
												break;
											}
										}
										if (!bHitted)
											break;
										continue;

									case EHHGroup.HO_HC:
									case EHHGroup.Group:
										if (!CDTXMania.Instance.ConfigIni.bCymbalFree)
										{
											for (int i = 0; i < NumOfChips; i++)
											{
												if ((e判定Array[i] != EJudge.Miss) && (((chipArray[i] == chipLC) || (chipArray[i] == chipHC)) || (chipArray[i] == chipHO)))
												{
													this.tドラムヒット処理(nTime, EPad.LC, chipArray[i], inputEvent.nVelocity);
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
											if ((e判定Array[i] != EJudge.Miss) && ((chipArray[i] != chipRD) || (CDTXMania.Instance.ConfigIni.eCYGroup == ECYGroup.Group)))
											{
												this.tドラムヒット処理(nTime, EPad.LC, chipArray[i], inputEvent.nVelocity);
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

						case EPad.HP:       // #27029 2012.1.4 from
							#region [ HPのヒット処理 ]
							//-----------------
							if (CDTXMania.Instance.ConfigIni.eBDGroup == EBDGroup.Group)
							{
								#region [ BDとみなしてヒット処理 ]
								//-----------------
								if (!this.tドラムヒット処理(nTime, EPad.BD, this.r指定時刻に一番近い未ヒットChip(nTime, EChannel.BassDrum, nInputAdjustTime, CDTXMania.Instance.nPoor範囲ms + 1), inputEvent.nVelocity))
									break;
								continue;
								//-----------------
								#endregion
							}
							else
							{
								#region [ HPのヒット処理 ]
								//-----------------
								continue;   // 何もしない。この入力を完全に無視するので、break しないこと。
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
					EPad pad = ePad; // 以下、nPad の代わりに pad を用いる。（成りすまし用）

					if (ePad == EPad.HP)
					{
						// #27029 2012.1.4 from: HP&BD 時の HiHatPedal の場合は BD に成りすます。
						pad = EPad.BD;
						//（ HP|BD 時のHP入力はここまでこないので無視。）
					}

					// レーンフラッシュ
					this.actLaneFlushD.Start(EnumConverter.LaneFromPad(pad), ((float)inputEvent.nVelocity) / 127f);

					// パッド画像のヒット処理用

					// パッドアニメ
					this.actPad.Hit(pad);

					// 空打ち音
					if (CDTXMania.Instance.ConfigIni.bDrumsHitSound)
					{
						CChip rChip = this.r空うちChip(EPart.Drums, pad);
						if (rChip != null)
						{
							// (B1) 空打ち音が譜面で指定されているのでそれを再生する。
							this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, EPart.Drums, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums);
						}
						else
						{
							#region [ (B2) 空打ち音が指定されていないので一番近いチップを探して再生する。]
							switch (pad)
							{
								case EPad.HH:
									{
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatClose, nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatOpen, nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.LeftCymbal, nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup.Value)
										{
											case EHHGroup.HO_HC:
												rChip = (chipHC != null) ? chipHC : chipLC;
												break;

											case EHHGroup.LC_HH:
												rChip = (chipHC != null) ? chipHC : chipHO;
												break;

											case EHHGroup.Group:
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
									break;

								case EPad.LT:
									{
										CChip chipLT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.LowTom, nInputAdjustTime);
										CChip chipFT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.FloorTom, nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eFTGroup != EFTGroup.Off)
											rChip = (chipLT != null) ? chipLT : chipFT;
										else
											rChip = chipLT;
									}
									break;

								case EPad.FT:
									{
										CChip chipLT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.LowTom, nInputAdjustTime);
										CChip chipFT = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.FloorTom, nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eFTGroup != EFTGroup.Off)
											rChip = (chipFT != null) ? chipFT : chipLT;
										else
											rChip = chipFT;
									}
									break;

								case EPad.CY:
									{
										CChip chipCY = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.Cymbal, nInputAdjustTime);
										CChip chipRD = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.RideCymbal, nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eCYGroup != ECYGroup.Off)
											rChip = (chipCY != null) ? chipCY : chipRD;
										else
											rChip = chipCY;
									}
									break;

								case EPad.HHO:
									{
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatClose, nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatOpen, nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.LeftCymbal, nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup.Value)
										{
											case EHHGroup.Off:
												rChip = chipHO;
												break;

											case EHHGroup.HO_HC:
												rChip = (chipHO != null) ? chipHO : chipLC;
												break;

											case EHHGroup.LC_HH:
												rChip = (chipHO != null) ? chipHO : chipHC;
												break;

											case EHHGroup.Group:
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
									break;

								case EPad.RD:
									{
										CChip chipCY = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.Cymbal, nInputAdjustTime);
										CChip chipRD = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.RideCymbal, nInputAdjustTime);
										if (CDTXMania.Instance.ConfigIni.eCYGroup != ECYGroup.Off)
											rChip = (chipRD != null) ? chipRD : chipCY;
										else
											rChip = chipRD;
									}
									break;

								case EPad.LC:
									{
										CChip chipHC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatClose, nInputAdjustTime);
										CChip chipHO = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.HiHatOpen, nInputAdjustTime);
										CChip chipLC = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EChannel.LeftCymbal, nInputAdjustTime);
										switch (CDTXMania.Instance.ConfigIni.eHHGroup.Value)
										{
											case EHHGroup.Off:
											case EHHGroup.LC_HH:
												rChip = chipLC;
												break;

											case EHHGroup.HO_HC:
											case EHHGroup.Group:
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
									break;

								default:
									rChip = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, EnumConverter.ChannelFromPad(pad), nInputAdjustTime);
									break;
							}
							if (rChip != null)
							{
								// 空打ち音が見つかったので再生する。
								this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, EPart.Drums, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums);
							}
							//-----------------
							#endregion
						}
					}

					// BAD or TIGHT 時の処理。
					if (CDTXMania.Instance.ConfigIni.bTight)
						this.tチップのヒット処理_BadならびにTight時のMiss(EPart.Drums, EnumConverter.LaneFromPad(pad));//nパッド0Atoレーン07[(int)pad]
																																														//-----------------------------
					#endregion
				}
			}
		}

		private void ドラムスクロール速度アップ()
		{
			int scrollSpeed = CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums;
			CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums.Value = scrollSpeed + 1;
			float f = (float)CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums / (scrollSpeed + 1);
			if (scrollSpeed < CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums)
			{
				CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums.Value = ((int)(f * (CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums + 1) + 0.5));
			}
		}

		private void ドラムスクロール速度ダウン()
		{
			int scrollSpeed = CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums;
			CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums.Value = (scrollSpeed - 1);
			float f = (float)CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums / (scrollSpeed + 1);
			if (scrollSpeed > CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums)
			{
				CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums.Value = ((int)(f * (CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums + 1) + 0.5));
			}
		}

		private int nStartTime_ = 0;

		protected void tキー入力()
		{
			IInputDevice keyboard = CDTXMania.Instance.Input管理.Keyboard;
			if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F1) &&
					(keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
			{   // shift+f1 (pause)
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
							if (pChip.bWAVを使うチャンネルである && !pChip.b空打ちチップである) // wav系チャンネル、且つ、空打ちチップではない
							{
								CDTX.CWAV wc;
								bool b = CDTXMania.Instance.DTX.listWAV.TryGetValue(pChip.n整数値_内部番号, out wc);
								if (!b) continue;

								if ((wc.bIsBGMSound && CDTXMania.Instance.ConfigIni.bBGMPlay) || (!wc.bIsBGMSound))
								{
									CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Unknown));
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
				if (CDTXMania.Instance.ConfigIni.bDrums有効)
				{
					this.t入力処理_ドラム();
				}
				if (CDTXMania.Instance.ConfigIni.bGuitar有効)
				{
					this.t入力処理_ギターベース(EPart.Guitar);
					this.t入力処理_ギターベース(EPart.Bass);
				}

				if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.UpArrow) && (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
				{   // shift (+ctrl) + UpArrow (BGMAdjust)
					CDTXMania.Instance.DTX.t各自動再生音チップの再生時刻を変更する((keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl)) ? 1 : 10);
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.DownArrow) && (keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightShift) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftShift)))
				{   // shift + DownArrow (BGMAdjust)
					CDTXMania.Instance.DTX.t各自動再生音チップの再生時刻を変更する((keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftControl) || keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightControl)) ? -1 : -10);
					CDTXMania.Instance.DTX.tWave再生位置自動補正();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.UpArrow))
				{   // UpArrow(scrollspeed up)
					ドラムスクロール速度アップ();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.DownArrow))
				{   // DownArrow (scrollspeed down)
					ドラムスクロール速度ダウン();
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Delete))
				{   // del (debug info)
					CDTXMania.Instance.ConfigIni.bDebugInfo.Value = !CDTXMania.Instance.ConfigIni.bDebugInfo;
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.LeftArrow))      // #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying(keyboard, -1);
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.RightArrow))     // #24243 2011.1.16 yyagi UI for InputAdjustTime in playing screen.
				{
					ChangeInputAdjustTimeInPlaying(keyboard, +1);
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F5))
				{
					int nVal = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums - 1;
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums.Value =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar.Value =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass.Value = nVal;
				}
				else if (keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F6))
				{
					int nVal = CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums + 1;
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Drums.Value =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Guitar.Value =
					CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset.Bass.Value = nVal;
				}
				else if ((base.eフェーズID == CStage.Eフェーズ.共通_通常状態) &&
						(keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Escape) ||
						CDTXMania.Instance.Pad.bCancelPadIsPressedGB()))
				{
					// escape (exit)
					this.actFO.tフェードアウト開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
					this.eフェードアウト完了時の戻り値 = E演奏画面の戻り値.演奏中断;
				}
			}
		}

		protected void t入力メソッド記憶(EPart part)
		{
			if (CDTXMania.Instance.Pad.st検知したデバイス.Keyboard)
			{
				this.b演奏にキーボードを使った[part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.Joypad)
			{
				this.b演奏にジョイパッドを使った[part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.MIDIIN)
			{
				this.b演奏にMIDI入力を使った[part] = true;
			}
			if (CDTXMania.Instance.Pad.st検知したデバイス.Mouse)
			{
				this.b演奏にマウスを使った[part] = true;
			}
		}

		/// <summary>
		/// チップに関連する処理を行う。
		/// </summary>
		/// <returns>演奏が終了したかどうかを示す値</returns>
		protected bool t進行描画_チップ()
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

			//CDTX dTX = CDTXMania.Instance.DTX;
			//CConfigIni configIni = CDTXMania.Instance.ConfigIni;
			for (int nCurrentTopChip = this.n現在のトップChip; nCurrentTopChip < CDTXMania.Instance.DTX.listChip.Count; nCurrentTopChip++)
			{
				CChip pChip = CDTXMania.Instance.DTX.listChip[nCurrentTopChip];
				//Debug.WriteLine( "nCurrentTopChip=" + nCurrentTopChip + ", ch=" + pChip.nチャンネル番号.ToString("x2") + ", 発音位置=" + pChip.n発声位置 + ", 発声時刻ms=" + pChip.n発声時刻ms );
				pChip.CalcDistanceFromBar(CSound管理.rc演奏用タイマ.n現在時刻, this.act譜面スクロール速度.db現在の譜面スクロール速度);
				if (Math.Min(Math.Min(pChip.nバーからの距離dot.Drums, pChip.nバーからの距離dot.Guitar), pChip.nバーからの距離dot.Bass) > 450 * Scale.Y)
				{
					break;
				}
				// #28026 2012.4.5 yyagi; 信心ワールドエンドの曲終了後リザルトになかなか行かない問題の修正
				if ((CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].nバーからの距離dot.Drums < -65 * Scale.Y) &&   // 小節線の消失処理などに影響するため、
						(CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].nバーからの距離dot.Guitar < -65 * Scale.Y) &&  // Drumsのスクロールスピードだけには依存させない。
						(CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].nバーからの距離dot.Bass < -65 * Scale.Y) &&
						CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].bHit)
				{
					++this.n現在のトップChip;
					continue;
				}
				bool bPChipIsAutoPlay = pChip.bAssignAutoPlayState();

				int nInputAdjustTime = (bPChipIsAutoPlay || (pChip.e楽器パート == EPart.Unknown)) ? 0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[pChip.e楽器パート];

				EPart inst = pChip.e楽器パート;
				if (((pChip.e楽器パート != EPart.Unknown) && !pChip.bHit) &&
								((pChip.nバーからの距離dot[inst] < -40 * Scale.Y) &&
						(this.e指定時刻からChipのJUDGEを返す(CSound管理.rc演奏用タイマ.n現在時刻, pChip, nInputAdjustTime) == EJudge.Miss)))
				{
					this.tチップのヒット処理(CSound管理.rc演奏用タイマ.n現在時刻, pChip);    //チップ消失(Hitせずスルーした場合)
				}
				if (((pChip.e楽器パート != EPart.Unknown) && !pChip.bHit) &&
						((pChip.nバーからの距離dot[inst] + CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[inst] < 0)))
				{
					//Debug.WriteLine( "透明度＝" + pChip.n透明度 );
					pChip.n透明度 -= 12;       // チップが判定バーを越えたら、徐々に透明にする。VSyncWaitの有無で加減が変わるが・・
					if (pChip.n透明度 < 0)
					{
						pChip.n透明度 = 0;
					}
				}

				// #35411 chnmr0 add (ターゲットゴースト)
				if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] != ETargetGhostData.None &&
								 CDTXMania.Instance.DTX.listTargetGhsotLag[inst] != null &&
								 pChip.e楽器パート != EPart.Unknown &&
								 pChip.nバーからの距離dot[inst] < 0)
				{
					if (!pChip.bTargetGhost判定済み)
					{
						pChip.bTargetGhost判定済み = true;

						int ghostLag = 128;
						if (0 <= pChip.n楽器パートでの出現順 && pChip.n楽器パートでの出現順 < CDTXMania.Instance.DTX.listTargetGhsotLag[inst].Count)
						{
							ghostLag = CDTXMania.Instance.DTX.listTargetGhsotLag[inst][pChip.n楽器パートでの出現順];
							// 上位８ビットが１ならコンボが途切れている（ギターBAD空打ちでコンボ数を再現するための措置）
							if (ghostLag > 255)
							{
								this.nコンボ数_TargetGhost[inst] = 0;
							}
							ghostLag = (ghostLag & 255) - 128;
						}
						else if (CDTXMania.Instance.ConfigIni.eTargetGhost[inst] == ETargetGhostData.Perfect)
						{
							ghostLag = 0;
						}

						if (ghostLag <= 127)
						{
							EJudge eJudge = this.e指定時刻からChipのJUDGEを返す(pChip.n発声時刻ms + ghostLag, pChip, 0, false);
							this.nヒット数_TargetGhost[inst][(int)eJudge]++;
							if (eJudge == EJudge.Miss || eJudge == EJudge.Poor)
							{
								this.n最大コンボ数_TargetGhost[inst] = Math.Max(this.n最大コンボ数_TargetGhost[inst], this.nコンボ数_TargetGhost[inst]);
								this.nコンボ数_TargetGhost[inst] = 0;
							}
							else
							{
								this.nコンボ数_TargetGhost[inst]++;
							}
						}
					}
				}

				if (pChip[EChannel.BGM] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (CDTXMania.Instance.ConfigIni.bBGMPlay)
					{
						//long t = CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms;
						//Trace.TraceInformation( "BGM再生開始: 演奏タイマのn前回リセットしたときのシステム時刻=" + CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + ", pChip.n発生時刻ms=" + pChip.n発声時刻ms + ", 合計=" + t );
						CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Unknown));
					}
				}
				else if (pChip[EChannel.BPM] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					this.actPlayInfo.dbBPM = (pChip.n整数値 * (((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0)) + CDTXMania.Instance.DTX.BASEBPM;
				}
				else if (pChip.bBGALayer && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (CDTXMania.Instance.ConfigIni.bBGA)
					{
						switch (pChip.eBGA種別)
						{
							case EBGAType.BMPTEX:
								if (pChip.rBMPTEX != null)
								{
									this.actBGA.Start(pChip, null, pChip.rBMPTEX, pChip.rBMPTEX.tx画像.sz画像サイズ.Width, pChip.rBMPTEX.tx画像.sz画像サイズ.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
								}
								break;

							case EBGAType.BGA:
								if ((pChip.rBGA != null) && ((pChip.rBMP != null) || (pChip.rBMPTEX != null)))
								{
									this.actBGA.Start(pChip, pChip.rBMP, pChip.rBMPTEX, pChip.rBGA.pt画像側右下座標.X - pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側右下座標.Y - pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt画像側左上座標.X, pChip.rBGA.pt画像側左上座標.Y, 0, 0, pChip.rBGA.pt表示座標.X, pChip.rBGA.pt表示座標.Y, 0, 0, 0);
								}
								break;

							case EBGAType.BGAPAN:
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
				else if (pChip[EChannel.BPMEx] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (CDTXMania.Instance.DTX.listBPM.ContainsKey(pChip.n整数値_内部番号))
					{
						this.actPlayInfo.dbBPM = (CDTXMania.Instance.DTX.listBPM[pChip.n整数値_内部番号].dbBPM値 *
								(((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0)) +
								CDTXMania.Instance.DTX.BASEBPM;
					}
				}
				else if (pChip.bDrums可視チップ && pChip.b空打ちチップである)
				{
					if (!pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
					{
						pChip.bHit = true;
						if (CDTXMania.Instance.ConfigIni.bDrums有効)
						{
							this.r現在の空うちドラムChip[(int)(EnumConverter.PadFromChannel(pChip.eチャンネル番号) - EPad.DrumsPadMin)] = pChip;
						}
					}
				}
				else if (pChip.bDrums可視チップ_LP_LBD含まない)
				{
					this.t進行描画_チップ_ドラムス(ref pChip);
				}
				else if (pChip[EChannel.DrumsFillin] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Drums = pChip;
				}
				else if (pChip.bGuitar可視チップ)
				{
					this.t進行描画_チップ_ギターベース(ref pChip, EPart.Guitar);
				}
				else if (pChip[EChannel.Guitar_Wailing])
				{
					this.t進行描画_チップ_ウェイリング(ref pChip);
				}
				else if (pChip[EChannel.Guitar_WailingSound] && !pChip.bHit && (pChip.nバーからの距離dot.Guitar < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Guitar = pChip;
				}
				else if (pChip.bDrums不可視チップ && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
				}
				else if (pChip[EChannel.BarLine] || pChip[EChannel.BeatLine])// 小節線
				{
					this.t進行描画_チップ_小節線_拍線(ref pChip);
				}
				else if (pChip[EChannel.MIDIChorus] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
				}
				else if (pChip[EChannel.FillIn])
				{
					this.t進行描画_チップ_フィルイン(ref pChip);
				}
				else if (pChip.bMovie && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if (CDTXMania.Instance.ConfigIni.bAVI)
					{
						if (CDTXMania.Instance.DTX.bチップがある.BGA)
						{
							this.actAVI.bHasBGA = true;
						}
						if (pChip.eチャンネル番号 == EChannel.MovieFull || CDTXMania.Instance.ConfigIni.bFullAVI)
						{
							this.actAVI.bFullScreenMovie = true;
						}
						switch (pChip.eAVI種別)
						{
							case EAVIType.AVI:
								{
									int startWidth = !this.actAVI.bFullScreenMovie ? 278 : SampleFramework.GameWindowSize.Width;
									int startHeight = !this.actAVI.bFullScreenMovie ? 355 : SampleFramework.GameWindowSize.Height;
									this.actAVI.Start(pChip.eチャンネル番号, pChip.rAVI, startWidth, startHeight, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, pChip.n発声時刻ms);
								}
								break;

							case EAVIType.AVIPAN:
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
					if (CDTXMania.Instance.ConfigIni.bBGMPlay)
					{
						CDTXMania.Instance.DTX.tWavの再生停止(this.n最後に再生したBGMの実WAV番号[pChip.eチャンネル番号 - EChannel.SE01]);
						CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Unknown));
						this.n最後に再生したBGMの実WAV番号[pChip.eチャンネル番号 - EChannel.SE01] = pChip.n整数値_内部番号;
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
					EPart[] p = { EPart.Drums, EPart.Drums, EPart.Drums, EPart.Drums, EPart.Guitar, EPart.Bass };

					EPart pp = p[pChip.eチャンネル番号 - EChannel.SE24];

					//							if ( pp == E楽器パート.DRUMS ) {			// pChip.nチャンネル番号= ..... HHとか、ドラムの場合は変える。
					//								//            HC    CY    RD    LC
					//								int[] ch = { 0x11, 0x16, 0x19, 0x1A };
					//								pChip.nチャンネル番号 = ch[ pChip.nチャンネル番号 - 0x84 ]; 
					//							}
					this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, pp, CDTXMania.Instance.DTX.nモニタを考慮した音量(pp));
				}

				else if (pChip.bBass可視チップ)
				{
					this.t進行描画_チップ_ギターベース(ref pChip, EPart.Bass);
				}
				else if (pChip[EChannel.Bass_Wailing])
				{
					this.t進行描画_チップ_ウェイリング(ref pChip);
				}
				else if (pChip[EChannel.Bass_WailingSound] && !pChip.bHit && (pChip.nバーからの距離dot.Bass < 0))
				{
					pChip.bHit = true;
					this.r現在の歓声Chip.Bass = pChip;
				}
				else if (pChip[EChannel.Guitar_NoChip] && !pChip.bHit && (pChip.nバーからの距離dot.Guitar < 0))
				{
					pChip.bHit = true;
					this.r現在の空うちギターChip = pChip;
					pChip.ConvertGBNoChip();
				}
				else if (pChip[EChannel.Bass_NoChip] && !pChip.bHit && (pChip.nバーからの距離dot.Bass < 0))
				{
					pChip.bHit = true;
					this.r現在の空うちベースChip = pChip;
					pChip.ConvertGBNoChip();
				}
				else if (pChip.bBGALayerSwap && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					pChip.bHit = true;
					if ((CDTXMania.Instance.ConfigIni.bBGA && (pChip.eBGA種別 == EBGAType.BMP)) || (pChip.eBGA種別 == EBGAType.BMPTEX))
					{
						this.actBGA.ChangeScope(pChip);
					}
				}
				else if (pChip[EChannel.MixerAdd] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					//Debug.WriteLine( "[DA(AddMixer)] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値.ToString( "x2" ) + ", time=" + pChip.n発声時刻ms );
					pChip.bHit = true;
					if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))    // 参照が遠いので後日最適化する
					{
						CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
						//Debug.Write( "[AddMixer] BAR=" + pChip.n発声位置 / 384 + ", wav=" + Path.GetFileName( wc.strファイル名 ) + ", time=" + pChip.n発声時刻ms );

						for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPolyphonicSounds; i++)
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
				else if (pChip[EChannel.MixerRemove] && !pChip.bHit && (pChip.nバーからの距離dot.Drums < 0))
				{
					//Debug.WriteLine( "[DB(RemoveMixer)] BAR=" + pChip.n発声位置 / 384 + " ch=" + pChip.nチャンネル番号.ToString( "x2" ) + ", wav=" + pChip.n整数値.ToString( "x2" ) + ", time=" + pChip.n発声時刻ms );
					pChip.bHit = true;
					if (CDTXMania.Instance.DTX.listWAV.ContainsKey(pChip.n整数値_内部番号))    // 参照が遠いので後日最適化する
					{
						CDTX.CWAV wc = CDTXMania.Instance.DTX.listWAV[pChip.n整数値_内部番号];
						//Debug.Write( "[DelMixer] BAR=" + pChip.n発声位置 / 384 +  ", wav=" + Path.GetFileName( wc.strファイル名 ) + ", time=" + pChip.n発声時刻ms );
						for (int i = 0; i < CDTXMania.Instance.ConfigIni.nPolyphonicSounds; i++)
						{
							if (wc.rSound[i] != null)
							{
								//CDTXMania.Instance.Sound管理.RemoveMixer( wc.rSound[ i ] );
								if (!wc.rSound[i].b演奏終了後も再生が続くチップである)   // #32248 2013.10.16 yyagi
								{                                                           // DTX終了後も再生が続くチップの0xDB登録をなくすことはできず。
									RemoveMixer(wc.rSound[i]);                          // (ミキサー解除のタイミングが遅延する場合の対応が面倒なので。)
								}                                                           // そこで、代わりにフラグをチェックしてミキサー削除ロジックへの遷移をカットする。
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
				else if ( pChip[ EChannel.Click ] && !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
				{
					pChip.bHit = true;
					if ( CDTXMania.Instance.ConfigIni.eClickType != EClickType.Off )
					{
						switch (pChip.n整数値)
						{
							case 1:
								CDTXMania.Instance.Skin.soundClickHigh.t再生する();
								break;
							case 2:
								CDTXMania.Instance.Skin.soundClickLow.t再生する();
								break;
						}
					}
				}
				else if ( !pChip.bHit && ( pChip.nバーからの距離dot.Drums < 0 ) )
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
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
			{
				CDTXMania.Instance.ConfigIni.nViewerScrollSpeed[i] = CDTXMania.Instance.ConfigIni.nScrollSpeed[i];
			}
			CDTXMania.Instance.ConfigIni.bDebugInfo = CDTXMania.Instance.ConfigIni.bViewerShowDebugStatus;
			#endregion
		}

		public void t停止()
		{
			CDTXMania.Instance.DTX.t全チップの再生停止とミキサーからの削除();
			this.actAVI.Stop();
			this.actBGA.Stop();
			this.actPanel.Stop();               // PANEL表示停止
			CDTXMania.Instance.Timer.t一時停止();       // 再生時刻カウンタ停止

			this.n現在のトップChip = CDTXMania.Instance.DTX.listChip.Count - 1;   // 終端にシーク

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
			// +1が必要
			nStartBar++;

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
				this.n現在のトップChip = 0;       // 対象小節が存在しないなら、最初から再生
			}
			#endregion

			#region [ 演奏開始の発声時刻msを取得し、タイマに設定 ]
			int nStartTime = CDTXMania.Instance.DTX.listChip[this.n現在のトップChip].n発声時刻ms;

			CSound管理.rc演奏用タイマ.tリセット();  // これでPAUSE解除されるので、次のPAUSEチェックは不要
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
					if (pChip.bWAVを使うチャンネルである && !pChip.b空打ちチップである) // wav系チャンネル、且つ、空打ちチップではない
					{
						CDTX.CWAV wc;
						bool b = CDTXMania.Instance.DTX.listWAV.TryGetValue(pChip.n整数値_内部番号, out wc);
						if (!b) continue;

						if ((wc.bIsBGMSound && CDTXMania.Instance.ConfigIni.bBGMPlay) || (!wc.bIsBGMSound))
						{
							CDTXMania.Instance.DTX.tチップの再生(pChip, CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻 + pChip.n発声時刻ms, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Unknown));
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
			CDTXMania.Instance.Timer.tリセット();                       // これでPAUSE解除されるので、3行先の再開()は不要
			CDTXMania.Instance.Timer.n現在時刻 = nStartTime;                // Debug表示のTime: 表記を正しくするために必要
			CSound管理.rc演奏用タイマ.t再開();
			//CDTXMania.Instance.Timer.t再開();
			this.bPAUSE = false;                                // システムがPAUSE状態だったら、強制解除
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
			for (EPad i = EPad.Min; i < EPad.Max; ++i)
			{
				CDTXMania.Instance.ConfigIni.bAutoPlay[i].Value = true;
			}
			CDTXMania.Instance.ConfigIni.bAVI.Value = true;
			CDTXMania.Instance.ConfigIni.bBGA.Value = true;
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
			{
				CDTXMania.Instance.ConfigIni.bGraph[i].Value = false;
				CDTXMania.Instance.ConfigIni.eSudHidInv[i].Value = ESudHidInv.Off;
				CDTXMania.Instance.ConfigIni.bLight[i].Value = false;
				CDTXMania.Instance.ConfigIni.bReverse[i].Value = false;
				CDTXMania.Instance.ConfigIni.eRandom[i].Value = ERandom.Off;
				CDTXMania.Instance.ConfigIni.nMinComboDisp[i].Value = 65535;
				CDTXMania.Instance.ConfigIni.bDisplayJudge[i].Value = false;
				CDTXMania.Instance.ConfigIni.bDisplayCombo[i].Value = false;
			}
			CDTXMania.Instance.ConfigIni.eDark.Value = EDark.Off;
			CDTXMania.Instance.ConfigIni.bDebugInfo.Value = CDTXMania.Instance.ConfigIni.bViewerShowDebugStatus;
			CDTXMania.Instance.ConfigIni.bFillin.Value = true;
			CDTXMania.Instance.ConfigIni.bScoreIni.Value = false;
			CDTXMania.Instance.ConfigIni.bStageFailed.Value = false;
			CDTXMania.Instance.ConfigIni.bTight.Value = false;
			CDTXMania.Instance.ConfigIni.bStoicMode.Value = false;
			CDTXMania.Instance.ConfigIni.bDrumsHitSound.Value = true;
			CDTXMania.Instance.ConfigIni.bBGMPlay.Value = true;
			CDTXMania.Instance.ConfigIni.nRisky.Value = 0;
			CDTXMania.Instance.ConfigIni.nShowLagType.Value = EShowLagType.Off;
		}

		private void t進行描画_チップ_ウェイリング(ref CChip pChip)
		{
			if (CDTXMania.Instance.ConfigIni.bGuitar有効)
			{
				EPart indexInst = pChip.bGuitar可視チップ_Wailing含む ? EPart.Guitar : EPart.Bass;
				#region [ Sud Hid Inv 処理 ]
				if (
					CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.FullInv ||
					CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.SemiInv)
				{
					cInvisibleChip.SetInvisibleStatus(ref pChip);
				}
				else
				{
					if (CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.Sudden ||
						CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.SudHid)
					{
						pChip.b可視 = (pChip.nバーからの距離dot[indexInst] < CDTXMania.Instance.ConfigIni.nSuddenFrom[indexInst]);
					}
					if (CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.Hidden ||
						CDTXMania.Instance.ConfigIni.eSudHidInv[indexInst] == ESudHidInv.SudHid)
					{
						pChip.b可視 = pChip.nバーからの距離dot[indexInst] >= CDTXMania.Instance.ConfigIni.nHiddenFrom[indexInst];
					}
				}
				#endregion
				cWailingChip[(int)indexInst].t進行描画_チップ_ウェイリング(ref pChip, ref txチップGB, ref ctWailingチップ模様アニメ);

				if (!pChip.bHit && (pChip.nバーからの距離dot[indexInst] < 0))
				{
					if (pChip.nバーからの距離dot[indexInst] < -234 * Scale.Y)  // #25253 2011.5.29 yyagi: Don't set pChip.bHit=true for wailing at once. It need to 1sec-delay (234pix per 1sec). 
					{
						pChip.bHit = true;
					}
					bool autoW = (indexInst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail : CDTXMania.Instance.ConfigIni.bAutoPlay.BsWail;
					if (autoW)
					{
						// #25253 2011.5.29 yyagi: Set pChip.bHit=true if autoplay.
						// pChip.bHit = true;
						// this.actWailingBonus.Start( inst, this.r現在の歓声Chip[indexInst] );
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

		protected void t進行描画_チップアニメ()
		{
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)            // 0=drums, 1=guitar, 2=bass
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

		protected virtual void t入力処理_ギターベース(EPart inst)
		{
			#region [ スクロール速度変更 ]
			int scrollSpeed = CDTXMania.Instance.ConfigIni.nScrollSpeed[inst];
			bool scrollSpeedChanged = false;
			float f = (float)CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[inst] / (scrollSpeed + 1);
			if (
				CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtDecide : EPad.BsDecide) &&
				CDTXMania.Instance.Pad.b押された(inst == EPart.Guitar ? EPad.GtB : EPad.BsB)
				)
			{
				CDTXMania.Instance.ConfigIni.nScrollSpeed[inst].Value = scrollSpeed + 1;
				if (scrollSpeed < CDTXMania.Instance.ConfigIni.nScrollSpeed[inst])
				{
					scrollSpeedChanged = true;
				}
			}
			if (
				CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtDecide : EPad.BsDecide) &&
				CDTXMania.Instance.Pad.b押された(inst == EPart.Guitar ? EPad.GtR : EPad.BsR)
				)
			{
				CDTXMania.Instance.ConfigIni.nScrollSpeed[inst].Value = scrollSpeed - 1;
				if (scrollSpeed > CDTXMania.Instance.ConfigIni.nScrollSpeed[inst])
				{
					scrollSpeedChanged = true;
				}
			}
			if (scrollSpeedChanged)
			{
				// 判定ラインも付随
				CDTXMania.Instance.ConfigIni.nJudgeLinePosOffset[inst].Value = (int)(f * CDTXMania.Instance.ConfigIni.nScrollSpeed[inst] + 0.5);
			}
			#endregion

			if (!CDTXMania.Instance.ConfigIni.bGuitar有効 || !CDTXMania.Instance.DTX.bチップがある[inst])
			{
				return;
			}

			int R = (inst == EPart.Guitar) ? 0 : 3;
			int G = R + 1;
			int B = R + 2;
			bool autoW = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail : CDTXMania.Instance.ConfigIni.bAutoPlay.BsWail;
			bool autoR = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtR : CDTXMania.Instance.ConfigIni.bAutoPlay.BsR;
			bool autoG = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtG : CDTXMania.Instance.ConfigIni.bAutoPlay.BsG;
			bool autoB = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtB : CDTXMania.Instance.ConfigIni.bAutoPlay.BsB;
			bool autoPick = (inst == EPart.Guitar) ? CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick : CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick;
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

			int pressingR = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtR : EPad.BsR) ? 4 : 0;
			this.t入力メソッド記憶(inst);
			int pressingG = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtG : EPad.BsG) ? 2 : 0;
			this.t入力メソッド記憶(inst);
			int pressingB = CDTXMania.Instance.Pad.b押されている(inst == EPart.Guitar ? EPad.GtB : EPad.BsB) ? 1 : 0;
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
			List<STInputEvent> events = CDTXMania.Instance.Pad.GetEvents(inst == EPart.Guitar ? EPad.GtPick : EPad.BsPick);
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
					EChannel chWailingSound = (inst == EPart.Guitar) ? EChannel.Guitar_WailingSound : EChannel.Bass_WailingSound;

					// WailingSound チャンネルでE楽器パート.GUITARなチップ全てにヒットする
					CChip pChip = this.r指定時刻に一番近い未ヒットChip(
							nTime, chWailingSound,
							CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[inst],
							CDTXMania.Instance.nPoor範囲ms + 1);

					EJudge e判定 = this.e指定時刻からChipのJUDGEを返す(nTime, pChip,
							CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[inst]);
					//Trace.TraceInformation("ch={0:x2}, mask1={1:x1}, mask2={2:x2}", pChip.nチャンネル番号,  ( pChip.nチャンネル番号 & ~nAutoMask ) & 0x0F, ( flagRGB & ~nAutoMask) & 0x0F );
					if (
							(pChip != null) &&
							((((int)pChip.eチャンネル番号 & ~nAutoMask) & 0x0F) == ((pressingRGB & ~nAutoMask) & 0x0F)) &&
							(e判定 != EJudge.Miss))
					{
						bool bChipHasR = pChip.bGuitarBass_R;
						bool bChipHasG = pChip.bGuitarBass_G;
						bool bChipHasB = pChip.bGuitarBass_B;
						bool bChipHasW = pChip.bGuitarBass_Wailing;
						bool bChipIsO = pChip.bGuitarBass_Open;
						bool bSuccessOPEN = bChipIsO && (autoR || pressingR == 0) && (autoG || pressingG == 0) && (autoB || pressingB == 0);
						if ((bChipHasR && (autoR || pressingR != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(R);
						}
						if ((bChipHasG && (autoG || pressingG != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(G);
						}
						if ((bChipHasB && (autoB || pressingB != 0)) || bSuccessOPEN)
						{
							this.actChipFireGB.Start(B);
						}
						this.tチップのヒット処理(nTime, pChip);
						this.tサウンド再生(pChip, CSound管理.rc演奏用タイマ.nシステム時刻, inst, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound[inst], e判定 == EJudge.Poor);
						EChannel chWailingChip = (inst == EPart.Guitar) ? EChannel.Guitar_Wailing : EChannel.Bass_Wailing;
						CChip item = this.r指定時刻に一番近い未ヒットChip(nTime, chWailingChip, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[inst], 140);
						if (item != null)
						{
							this.queWailing[inst].Enqueue(item);
						}
						continue;
					}

					// 以下、間違いレーンでのピック時
					CChip NoChipPicked = (inst == EPart.Guitar) ? this.r現在の空うちギターChip : this.r現在の空うちベースChip;
					if ((NoChipPicked != null) || ((NoChipPicked = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nTime, chWailingSound, CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs[inst])) != null))
					{
						this.tサウンド再生(NoChipPicked, CSound管理.rc演奏用タイマ.nシステム時刻, inst, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound[inst], true);
					}
					if (!CDTXMania.Instance.ConfigIni.bLight[inst])
					{
						this.tチップのヒット処理_BadならびにTight時のMiss(inst);
					}
				}
			}
			List<STInputEvent> list = CDTXMania.Instance.Pad.GetEvents(inst == EPart.Guitar ? EPad.GtWail : EPad.BsWail);
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

		private void DoWailingFromQueue(EPart inst, long nTimeStamp_Wailed, bool autoW)
		{
			long nTimeWailed = nTimeStamp_Wailed - CSound管理.rc演奏用タイマ.n前回リセットした時のシステム時刻;
			CChip chipWailing;
			while ((this.queWailing[inst].Count > 0) && ((chipWailing = this.queWailing[inst].Dequeue()) != null))
			{
				if ((nTimeWailed - chipWailing.n発声時刻ms) <= 1000)        // #24245 2011.1.26 yyagi: 800 -> 1000
				{
					chipWailing.bHit = true;
					this.actWailingBonus.Start(inst, this.r現在の歓声Chip[inst]);
					if (!autoW)
					{
						int nCombo = (this.actCombo.dgbコンボ数[inst].n現在値 < 500) ? this.actCombo.dgbコンボ数[inst].n現在値 : 500;
						// #24245 2011.1.26 yyagi changed DRUMS->BASS, add nCombo conditions
						this.actScore.Add(inst, nCombo * 3000L);
					}
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
		private static void SortChipsByNTime(CChip[] chipArray, EJudge[] e判定Array, int NumOfChips)
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
						EJudge e判定Temp = e判定Array[j - 1];
						e判定Array[j - 1] = e判定Array[j];
						e判定Array[j] = e判定Temp;
					}
				}
			}
		}
		private bool tドラムヒット処理(long nHitTime, EPad type, CChip pChip, int n強弱度合い0to127)
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
			ELane eLane = EnumConverter.LaneFromChannel(index + EChannel.HiHatClose);// nチャンネル0Atoレーン07[index];
			EPad ePad = EnumConverter.PadFromChannel(index + EChannel.HiHatClose); //nチャンネル0Atoパッド08[index];
			bool bPChipIsAutoPlay = CDTXMania.Instance.ConfigIni.bAutoPlay[ePad];
			int nInputAdjustTime = bPChipIsAutoPlay ? 0 : CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums;
			EJudge e判定 = this.e指定時刻からChipのJUDGEを返す(nHitTime, pChip, nInputAdjustTime);
			if (e判定 == EJudge.Miss)
			{
				return false;
			}
			this.tチップのヒット処理(nHitTime, pChip);
			this.actLaneFlushD.Start(eLane, ((float)n強弱度合い0to127) / 127f);
			this.actPad.Hit(ePad);
			if ((e判定 != EJudge.Poor) && (e判定 != EJudge.Miss))
			{
				bool flag = this.bフィルイン中;
				bool flag2 = this.bフィルイン中 && this.bフィルイン区間の最後のChipである(pChip);
				// bool flag3 = flag2;
				// #31602 2013.6.24 yyagi 判定ラインの表示位置をずらしたら、チップのヒットエフェクトの表示もずらすために、nJudgeLine..を追加
				this.actChipFireD.Start(eLane, flag, flag2, flag2);
			}
			if (CDTXMania.Instance.ConfigIni.bDrumsHitSound)
			{
				CChip rChip = null;
				bool bIsChipsoundPriorToPad = true;
				if (((type == EPad.HH) || (type == EPad.HHO)) || (type == EPad.LC))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH == EHitSoundPriority.Chip;
				}
				else if ((type == EPad.LT) || (type == EPad.FT))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT == EHitSoundPriority.Chip;
				}
				else if ((type == EPad.CY) || (type == EPad.RD))
				{
					bIsChipsoundPriorToPad = CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY == EHitSoundPriority.Chip;
				}
				if (bIsChipsoundPriorToPad)
				{
					rChip = pChip;
				}
				else
				{
					EPad hH = type;
					if (!CDTXMania.Instance.DTX.bチップがある.HHOpen && (type == EPad.HHO))
					{
						hH = EPad.HH;
					}
					if (!CDTXMania.Instance.DTX.bチップがある.Ride && (type == EPad.RD))
					{
						hH = EPad.CY;
					}
					if (!CDTXMania.Instance.DTX.bチップがある.LeftCymbal && (type == EPad.LC))
					{
						hH = EPad.HH;
					}
					rChip = this.r指定時刻に一番近いChip_ヒット未済問わず不可視考慮(nHitTime, EnumConverter.ChannelFromPad(hH), nInputAdjustTime);
					if (rChip == null)
					{
						rChip = pChip;
					}
				}
				this.tサウンド再生(rChip, CSound管理.rc演奏用タイマ.nシステム時刻, EPart.Drums, CDTXMania.Instance.ConfigIni.nChipVolume, CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums);
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
				if ((pChip[EChannel.FillIn]) && (pChip.n整数値 == 2))
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
						case 0x01:  // フィルイン開始
							if (CDTXMania.Instance.ConfigIni.bFillin)
							{
								this.bフィルイン中 = true;
							}
							break;

						case 0x02:  // フィルイン終了
							if (CDTXMania.Instance.ConfigIni.bFillin)
							{
								this.bフィルイン中 = false;
							}
							if (((this.actCombo.dgbコンボ数.Drums.n現在値 > 0) || CDTXMania.Instance.ConfigIni.bIsAutoPlay(EPart.Drums)) && CDTXMania.Instance.ConfigIni.bAudience)
							{
								if (this.r現在の歓声Chip.Drums != null)
								{
									CDTXMania.Instance.DTX.tチップの再生(this.r現在の歓声Chip.Drums, CSound管理.rc演奏用タイマ.nシステム時刻, CDTXMania.Instance.DTX.nモニタを考慮した音量(EPart.Unknown));
								}
								else
								{
									CDTXMania.Instance.Skin.sound歓声音.n位置_次に鳴るサウンド = 0;
									CDTXMania.Instance.Skin.sound歓声音.t再生する();
								}
							}
							break;
#if TEST_NOTEOFFMODE  // 2011.1.1 yyagi TEST
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
	}
}
