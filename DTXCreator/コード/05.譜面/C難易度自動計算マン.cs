using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTXCreator.譜面;

namespace DTXCreator.コード._05.譜面
{
  /// <summary>
  /// 20160216 chnmr0 難易度自動計算を行うためのクラス
  /// 
  /// </summary>
  internal class C難易度自動計算マン
  {
    C譜面管理 scoreManager;
    SortedDictionary<int, CPlayState> Drums;
    SortedDictionary<int, CPlayState> Guitar;
    SortedDictionary<int, CPlayState> Bass;

    public static int GtV; // 譜面管理者必要なため
    public static int BsV;

    decimal currentBpm;

    static readonly decimal[] Coefs = new decimal[4] { 0.5m, 1m, 1.1m, 1.2m };
    static readonly int[,] GtQCoefs = new int[4, 4] 
        { // v new Release -> newPress 
          {0, 2, 2, 1},
          {2, 2, 3, 0},
          {2, 3, 0, 0},
          {1, 0, 0, 0} 
        };
    static readonly int[] LvBase = new int[6] { 1, 10, 30, 50, 70, 90 };

    internal class CPlayState
    {
      List<Cチップ> state;

      public CPlayState()
      {
        state = new List<Cチップ>();
      }

      public void AddChip(Cチップ t)
      {
        // 重複しない前提
        state.Add(t);
      }

      public int Count
      {
        get
        {
          return state.Count;
        }
      }

      public Cチップ this[int index]
      {
        get
        {
          return state[index];
        }
      }

      public IEnumerable<Cチップ> State
      {
        get
        {
          return state;
        }
      }

      /// <summary>
      /// 次の状態で演奏状態が変わる（叩かれる→叩かれない,押されてる→離されるなど）レーンの数
      /// </summary>
      /// <param name="next">次の演奏状態</param>
      /// <param name="chipConv">チップコンバータ。チップがアサインされるレーンを示す IEnumerable へ変換する。
      /// ここでのレーンはみかけのレーンではなく、ドラムはチャンネルと同質。ギターチップ変換のために IEnumerable にしたが、現仕様では不要。</param>
      /// <returns>遷移結果。1が新たに演奏される(叩かれる/押される)。0が状態が変わらない。-1は1の反対。</returns>
      private int[] TransitionDifference(CPlayState next, Converter<Cチップ, IEnumerable<int>> chipConv)
      {
        Converter<CPlayState, int[]> stateConv = (playState) =>
        {
          int[] Q = new int[0xff];
          foreach (var chip in playState.State)
          {
            foreach (var laneIdentity in chipConv(chip))
            {
              Q[laneIdentity]++;
            }
          }
          return Q;
        };

        int[] Q1 = stateConv(this);
        int[] Q2 = stateConv(next);

        foreach (var x in Q1.Select((t, index) => { return new { t, index }; }))
        {
          Q1[x.index] = Q2[x.index] - Q1[x.index];
        }
        return Q1;
      }

      public int TransitionCost(int inst, int ms, CPlayState next)
      {
        int ret = 0;

        if (inst == 0) // Drums
        {
          ret = TransitionDifference(next, (chip) =>
          {
            List<int> a = new List<int>();

            if (chip.nチャンネル番号00toFF != 0x13)
            {
              a.Add(chip.nチャンネル番号00toFF);
            }
            return a;
          }).Sum((t) => { return t > 0 ? t : -t; }); // スティックで演奏する楽器のうち演奏状態が変わる楽器の数の半分（以下同様）
          ret += TransitionDifference(next, (chip) =>
          {
            List<int> a = new List<int>();

            if (chip.nチャンネル番号00toFF == 0x13)
            {
              a.Add(chip.nチャンネル番号00toFF);
            }
            return a;
          }).Sum((t) => { return t > 0 ? t : -t; });
        }
        else if (inst != 0) // Guitars
        {
          int[] Q = TransitionDifference(next, (chip) =>
          {
            List<int> a = new List<int>();

            if (chip.nレーン番号0to == C難易度自動計算マン.GtV || chip.nレーン番号0to == C難易度自動計算マン.BsV)
            {
            }
            else
            {
              a.Add(chip.nレーン番号0to);
            }
            return a;
          });

          int newPress = Q.Sum((t) => { return t > 0 ? t : 0; });
          int newRelease = Q.Sum((t) => { return t < 0 ? -t : 0; });

          ret = GtQCoefs[newRelease, newPress];
        }

        return ret;
      }
    }

    public C難易度自動計算マン(Cメインフォーム x)
    {
      scoreManager = x.mgr譜面管理者;
      currentBpm = x.dc現在のBPM;

      GtV = scoreManager.nレーン名に対応するレーン番号を返す("GtV");
      BsV = scoreManager.nレーン名に対応するレーン番号を返す("BsV");
    }

    public void Prepare()
    {
      // 時間ごとに楽器の演奏状態を求める。
      Drums = new SortedDictionary<int, CPlayState>();
      Guitar = new SortedDictionary<int, CPlayState>();
      Bass = new SortedDictionary<int, CPlayState>();

      foreach (var pair in scoreManager.dic小節)
      {
        pair.Value.listチップ.Sort();
      }

      decimal currentTime = 0;

      foreach (var pair in scoreManager.dic小節)
      {
        int lastGridInThisPhrase = 0;

        foreach (Cチップ chip in pair.Value.listチップ)
        {
          currentTime += (chip.n位置grid - lastGridInThisPhrase) * (60m / (48m * currentBpm));
          lastGridInThisPhrase = chip.n位置grid;

          if (scoreManager.IsPlayableChip(chip))
          {
            int ms = (int)(currentTime * 1000);

            if (scoreManager.IsDrumsChip(chip))
            {
              if (!Drums.ContainsKey(ms))
              {
                Drums.Add(ms, new CPlayState());
              }
              Drums[ms].AddChip(chip);
            }
            else if (scoreManager.IsGuitarChip(chip))
            {
              if (!Guitar.ContainsKey(ms))
              {
                Guitar.Add(ms, new CPlayState());
              }
              Guitar[ms].AddChip(chip);
            }
            else if (scoreManager.IsBassChip(chip))
            {
              if (!Bass.ContainsKey(ms))
              {
                Bass.Add(ms, new CPlayState());
              }
              Bass[ms].AddChip(chip);
            }
          }
          else
          {
            if (chip.nチャンネル番号00toFF == 0x8)
            {
              currentBpm = (decimal)chip.f値_浮動小数;
            }
          }
        }
        currentTime += (pair.Value.n小節長倍率を考慮した現在の小節の高さgrid - lastGridInThisPhrase) * (60m / (48m * currentBpm));
      }

      /* debug
       * part -1 のズレがあるので DTXM での再生時間とはすこし違う
       */
      using (System.IO.StringWriter sw = new System.IO.StringWriter())
      {
        foreach (var t in Guitar)
        {
          sw.Write(t.Key + "ms");
          foreach (var c in t.Value.State)
          {
            sw.Write(c.nレーン番号0to + ",");
          }
          sw.WriteLine();
        }
        System.Diagnostics.Debug.WriteLine(sw.ToString());
      }

    }

    public int DLevel
    {
      get
      {
        return CalculateLevel(0, Drums);
      }
    }

    public int GLevel
    {
      get
      {
        return CalculateLevel(1, Guitar);
      }
    }

    public int BLevel
    {
      get
      {
        return CalculateLevel(1, Bass);
      }
    }



    private int CalculateLevel(int inst, SortedDictionary<int, CPlayState> list)
    {
      int ret = 0;

      if (list.Count > 0)
      {
        int startMs = list.First().Key;
        int endMs = list.Last().Key;
        int prevms = list.First().Key;
        CPlayState prev = list.First().Value;

        int totalCost = 0;

        foreach (var x in list.Select((t, index) => { return new { t, index }; }))
        {
          int idx = x.index;
          int ms = x.t.Key;
          CPlayState state = x.t.Value;

          if (idx == 0)
          {
            continue;
          }

          // 遷移難度
          int cost = prev.TransitionCost(inst, ms - prevms, state);
          prevms = ms;
          prev = state;

          totalCost += cost;
        }

        if (endMs == startMs)
        {
          endMs = startMs + 1;
        }
        // 1秒間あたりの遷移コスト
        // 1秒間に 14 回をドラムの最大難易度とする
        // 1秒間に 7 回を3弦の最大難易度とする
        decimal lvBase = 1000 * (decimal)totalCost / (endMs - startMs);
        System.Diagnostics.Debug.WriteLine(lvBase);
        decimal lvMax = 14;

        if (lvBase < 1)
        {
          lvBase = 1;
        }

        if (inst != 0)
        {
          lvMax = 7;
        }

        if (lvBase > lvMax)
        {
          lvBase = lvMax;
        }

        ret = (int)(99m * (lvBase - 1) / (lvMax - 1));
        if (ret < 1)
        {
          ret = 1;
        }
        else if (ret > 99)
        {
          ret = 99;
        }
      }



      return ret;
    }
  }
}
