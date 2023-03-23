using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace DTXCreator.MIDIインポート
{
	/// <summary>
	/// MIDIイベントのbaseクラス
	/// 手抜きのため、オリジナルのCMIDIチップクラスをほぼそのまま流用。
	/// </summary>
	abstract class CMIDIイベント
    {
		public enum Eイベントタイプ : int
		{
			NoteOnOff,
			BPM,
			BarLen,
			Unknown
		}

		public Eイベントタイプ eイベントタイプ;
		public int nレーン番号;
        public UInt32 n時間;
        public int nWAV;
        public int nキー;
        public bool b入力;
        public bool b裏チャンネル;
        public string strコメント;
		public int nベロシティ;
		public int nベロシティ_DTX変換後;
		public int nチャンネル0to15;
		public int n拍子分子;
		public int n拍子分母;

		public string strWAV重複チェック
		{
			get
			{
				return "" + nキー.ToString() + " : " + nベロシティ_DTX変換後.ToString();
			}
		}


		public CMIDIイベント()
		{
			this.eイベントタイプ = Eイベントタイプ.Unknown;
		}

		abstract public void 挿入( Cメインフォーム mf, int n四分音符の分解能 );
    }


	/// <summary>
	/// NoteOn/OffのMIDIイベント
	/// </summary>
	class CMIDINote: CMIDIイベント
	{
		public CMIDINote( UInt32 _n時間, int _nキー, int _nベロシティ, int _nチャンネル0to15 )
        {
            this.nレーン番号 = 2;
            this.n時間 = _n時間;
            this.nWAV = 1;
            this.nキー = _nキー;
            this.b裏チャンネル = false;
			
            this.nベロシティ = _nベロシティ;
            this.nベロシティ_DTX変換後 = _nベロシティ;

			this.nチャンネル0to15 = _nチャンネル0to15;

			this.eイベントタイプ = Eイベントタイプ.NoteOnOff;
		}

		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			mf.mgr譜面管理者.tチップを配置または置換する
				( nレーン番号, (int) n時間 * ( CWholeNoteDivision.n分解能 / 4 ) / n四分音符の分解能, nWAV, 0f, b裏チャンネル );
		}
	}

	/// <summary>
	/// テンポ変更のメタイベント
	/// </summary>
	class CMIDIBPM : CMIDIイベント
	{
		float fBPM;
		public CMIDIBPM( UInt32 _n時間, float _fBPM )
		{
			this.nレーン番号 = 2;
			this.n時間 = _n時間;
			this.nWAV = 1;
			this.fBPM = _fBPM;

			this.eイベントタイプ = Eイベントタイプ.BPM;
		}

		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			int nGrid = (int) n時間 * ( CWholeNoteDivision.n分解能 / 4 ) / n四分音符の分解能;
			mf.mgr編集モード管理者.tBPMチップを配置する( nGrid, fBPM );
		}
	}


	/// <summary>
	/// 拍子変更のメタイベント
	/// </summary>
	class CMIDIBARLen : CMIDIイベント
	{
		public CMIDIBARLen( UInt32 _n時間, int _分子, int _分母 )
		{
			this.n時間 = _n時間;
			this.n拍子分子 = _分子;
			this.n拍子分母 = _分母;
			this.eイベントタイプ = Eイベントタイプ.BarLen;
		}
		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			//	事前の小節構築過程で拍子変更処理は完了しているため、ここでは何もしない
		}
	}
}
