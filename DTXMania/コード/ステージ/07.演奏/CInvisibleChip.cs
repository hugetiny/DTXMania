using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	public class CInvisibleChip : IDisposable
	{
		/// <summary>ミス後表示する時間(ms)</summary>
		public int nDisplayTimeMs
		{
			get;
			set;
		}
		/// <summary>表示期間終了後、フェードアウトする時間</summary>
		public int nFadeoutTimeMs
		{
			get;
			set;
		}

		#region [ コンストラクタ ]
		public CInvisibleChip()
		{
			Initialize(3000, 2000);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="_dbDisplayTime">ミス時再表示する時間(秒)</param>
		/// <param name="_dbFadeoutTime">再表示後フェードアウトする時間(秒)</param>
		public CInvisibleChip(int _nDisplayTimeMs, int _nFadeoutTimeMs)
		{
			Initialize(_nDisplayTimeMs, _nFadeoutTimeMs);
		}
		private void Initialize(int _nDisplayTimeMs, int _nFadeoutTimeMs)
		{
			nDisplayTimeMs = _nDisplayTimeMs;
			nFadeoutTimeMs = _nFadeoutTimeMs;
			Reset();
		}
		#endregion

		/// <summary>
		/// 内部状態を初期化する
		/// </summary>
		public void Reset()
		{
			for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
			{
				ccounter[i] = new CCounter();
				b演奏チップが１つでもバーを通過した[i] = false;
			}
		}

		/// <summary>
		/// まだSemi-Invisibleを開始していなければ、開始する
		/// </summary>
		/// <param name="eInst"></param>
		public void StartSemiInvisible(EPart eInst)
		{
			if (!b演奏チップが１つでもバーを通過した[eInst])
			{
				b演奏チップが１つでもバーを通過した[eInst] = true;
				if (CDTXMania.Instance.ConfigIni.eSudHidInv[eInst] == ESudHidInv.SemiInv)
				{
					ShowChipTemporally(eInst);
					ccounter[eInst].n現在の値 = nDisplayTimeMs;
				}
			}
		}
		/// <summary>
		/// 一時的にチップを表示するモードを開始する
		/// </summary>
		/// <param name="eInst">楽器パート</param>
		public void ShowChipTemporally(EPart eInst)
		{
			ccounter[eInst].t開始(0, nDisplayTimeMs + nFadeoutTimeMs + 1, 1, CDTXMania.Instance.Timer);
		}

		/// <summary>
		/// チップの表示/非表示の状態
		/// </summary>
		public enum EChipInvisibleState
		{
			SHOW,     // Missなどしてチップを表示中
			FADEOUT,    // 表示期間終了後、フェードアウト中
			INVISIBLE   // 完全非表示
		}

		internal EChipInvisibleState SetInvisibleStatus(ref CChip cc)
		{
			if (cc.e楽器パート == EPart.Unknown)
			{
				return EChipInvisibleState.SHOW;
			}
			EChipInvisibleState retcode = EChipInvisibleState.SHOW;

			ccounter[cc.e楽器パート].t進行();

			switch (CDTXMania.Instance.ConfigIni.eSudHidInv[cc.e楽器パート].Value)
			{
				case ESudHidInv.Off:
					cc.b可視 = true;
					retcode = EChipInvisibleState.SHOW;
					break;

				case ESudHidInv.FullInv:
					cc.b可視 = false;
					retcode = EChipInvisibleState.INVISIBLE;
					break;

				case ESudHidInv.SemiInv:
					if (!b演奏チップが１つでもバーを通過した[cc.e楽器パート]) // まだ1つもチップがバーを通過していない時は、チップを表示する
					{
						cc.b可視 = true;
						cc.n透明度 = 255;
						return EChipInvisibleState.SHOW;
					}

					if (ccounter[cc.e楽器パート].n現在の値 <= 0 || ccounter[cc.e楽器パート].n現在の値 > nDisplayTimeMs + nFadeoutTimeMs)
					// まだ一度もMissっていない or フェードアウトしきった後
					{
						cc.b可視 = false;
						cc.n透明度 = 255;
						retcode = EChipInvisibleState.INVISIBLE;
					}
					else if (ccounter[cc.e楽器パート].n現在の値 < nDisplayTimeMs)                // 表示期間
					{
						cc.b可視 = true;
						cc.n透明度 = 255;
						retcode = EChipInvisibleState.SHOW;
					}
					else if (ccounter[cc.e楽器パート].n現在の値 < nDisplayTimeMs + nFadeoutTimeMs)   // フェードアウト期間
					{
						cc.b可視 = true;
						cc.n透明度 = 255 - (int)(Convert.ToDouble(ccounter[cc.e楽器パート].n現在の値 - nDisplayTimeMs) / nFadeoutTimeMs * 255.0);
						retcode = EChipInvisibleState.FADEOUT;
					}
					break;
				default:
					cc.b可視 = true;
					cc.n透明度 = 255;
					retcode = EChipInvisibleState.SHOW;
					break;
			}
			return retcode;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposeManagedObjects)
		{
			if (this.bDispose完了済み)
				return;

			if (disposeManagedObjects)
			{
				// (A) Managed リソースの解放
				for (EPart i = EPart.Drums; i <= EPart.Bass; i++)
				{
					// ctInvisibleTimer[ i ].Dispose();
					ccounter[i].t停止();
					ccounter[i] = null;
				}
			}

			// (B) Unamanaged リソースの解放

			this.bDispose完了済み = true;
		}
		~CInvisibleChip()
		{
			this.Dispose(false);
		}

		private STDGBSValue<CCounter> ccounter;
		private bool bDispose完了済み = false;
		private STDGBSValue<bool> b演奏チップが１つでもバーを通過した;
	}
}
