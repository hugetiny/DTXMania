using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DTXMania
{
	/// <summary>
	/// 判定ライン関係の座標処理をまとめたクラス
	/// </summary>
	public class C演奏判定ライン座標共通
	{
		/// <summary>
		/// 判定ラインのy座標
		/// </summary>
		private STDGBVALUE<int>[,,] n判定ラインY座標元データ = null;			// 補正無しの時の座標データ
		//private STDGBVALUE<int>[,] n判定ラインY座標表示位置補正 = null;		// 表示位置の補正データ

		/// <summary>
		/// 表示位置の補正データ
		/// 初期化は外部から行うこと。
		/// </summary>
		public STDGBVALUE<int> nJudgeLinePosY_delta;

		/// <summary>
		/// 判定ライン表示位置を、Vシリーズ互換にするかどうか
		/// </summary>
		public bool bVseries;


		/// <summary>
		/// コンストラクタ(座標値の初期化)
		/// </summary>
		public C演奏判定ライン座標共通()
		{
			n判定ラインY座標元データ = new STDGBVALUE<int>[ 2, 2, 2 ];
			#region [ 判定ライン座標の初期化]
			// Normal, Drums画面, 判定ライン
			n判定ラインY座標元データ[ 0, 0, 0 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 0, 0, 0 ].Guitar = 95 - 3;
			n判定ラインY座標元データ[ 0, 0, 0 ].Bass   = 95 - 3;
			// Reverse, Drums画面, 判定ライン
			n判定ラインY座標元データ[ 1, 0, 0 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 1, 0, 0 ].Guitar = 374 - 3;
			n判定ラインY座標元データ[ 1, 0, 0 ].Bass   = 374 - 3;
			// Normal, Drums画面, Wailing枠
			n判定ラインY座標元データ[ 0, 0, 1 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 0, 0, 1 ].Guitar = 69;
			n判定ラインY座標元データ[ 0, 0, 1 ].Bass   = 69;
			// Reverse, Drums画面, Wailing枠
			n判定ラインY座標元データ[ 1, 0, 1 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 1, 0, 1 ].Guitar = 350;
			n判定ラインY座標元データ[ 1, 0, 1 ].Bass   = 350;

			// Normal, GR画面, 判定ライン
			n判定ラインY座標元データ[ 0, 1, 0 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 0, 1, 0 ].Guitar = 40 - 3;
			n判定ラインY座標元データ[ 0, 1, 0 ].Bass   = 40 - 3;
			// Reverse, GR画面, 判定ライン
			n判定ラインY座標元データ[ 1, 1, 0 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 1, 1, 0 ].Guitar = 369 - 3;
			n判定ラインY座標元データ[ 1, 1, 0 ].Bass   = 369 - 3;
			// Normal, GR画面, Wailing枠
			n判定ラインY座標元データ[ 0, 1, 1 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 0, 1, 1 ].Guitar = 11;
			n判定ラインY座標元データ[ 0, 1, 1 ].Bass   = 11;
			// Reverse, GR画面, Wailing枠
			n判定ラインY座標元データ[ 1, 1, 1 ].Drums  = 0;		//未使用
			n判定ラインY座標元データ[ 1, 1, 1 ].Guitar = 340;
			n判定ラインY座標元データ[ 1, 1, 1 ].Bass   = 340;
			#endregion

			// 補正値は、Normal/Reverse, Drums/GR画面共通
			nJudgeLinePosY_delta.Drums  = 0;
			nJudgeLinePosY_delta.Guitar = 0;
			nJudgeLinePosY_delta.Bass   = 0;
		}


	
		/// <summary>
		/// 判定ラインのY座標を返す。とりあえずGuitar/Bassのみ対応。
		/// </summary>
		/// <param name="eInst">E楽器パート</param>
		/// <param name="bGRmode">GRmodeか否か</param>
		/// <param name="bReverse">Reverseか否か</param>
		/// <param name="bReverse">Wailing枠の座標か、判定ラインの座標か</param>
		/// <returns></returns>
		public int n判定ラインY座標( E楽器パート eInst, bool bGRmode, bool bReverse )
		{
			return n判定ラインY座標( eInst, bGRmode, bReverse, false );
		}
		
		/// <summary>
		/// 判定ラインのY座標を返す。とりあえずGuitar/Bassのみ対応。
		/// </summary>
		/// <param name="eInst">E楽器パート</param>
		/// <param name="bGRmode">GRmodeか否か</param>
		/// <param name="bReverse">Reverseか否か</param>
		/// <param name="bReverse">Wailing枠の座標か、判定ラインの座標か</param>
		/// <returns></returns>
		public int n判定ラインY座標( E楽器パート eInst, bool bGRmode, bool bReverse, bool bWailingFrame )
		{
			if ( eInst == E楽器パート.DRUMS )
			{
				throw new NotImplementedException();
			}
			else if ( eInst == E楽器パート.UNKNOWN )
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				int nReverse = bReverse ?      1 : 0;
				int nGRmode  = bGRmode ?       1 : 0;
				int nWailing = bWailingFrame ? 1 : 0;
				int nInst    = (int) eInst;
				int ret = this.n判定ラインY座標元データ[ nReverse, nGRmode, nWailing ][ nInst ];		// 補正無しの値
				if ( bReverse )																			// 補正を追加
				{
					ret += nJudgeLinePosY_delta[ nInst ];
				}
				else
				{
					ret -= nJudgeLinePosY_delta[ nInst ];
				}
				return ret;
			}
		}
	}
}
