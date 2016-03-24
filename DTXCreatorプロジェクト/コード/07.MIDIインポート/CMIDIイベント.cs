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
        public string strコメント;
		public int nベロシティ;
		public int nベロシティ_DTX変換後;

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
		public CMIDINote( UInt32 _n時間, int _nキー, int _nベロシティ )
			: base()
        {
            this.nレーン番号 = 2;
            this.n時間 = _n時間;
            this.nWAV = 1;
            this.nキー = _nキー;
			
            this.nベロシティ = _nベロシティ;
            this.nベロシティ_DTX変換後 = _nベロシティ;

			this.eイベントタイプ = Eイベントタイプ.NoteOnOff;
		}

		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			//Debug.WriteLine( "NoteOn: " +  (n時間 * ( 192 / 4 ) / n四分音符の分解能).ToString() + "key=" + nキー.ToString("d2") );

			mf.mgr譜面管理者.tチップを配置または置換する
				( nレーン番号, (int) n時間 * ( 192 / 4 ) / n四分音符の分解能, nWAV, 0f, false ); 

			// ★★時間について、要変拍子対応

			//Debug.WriteLine( " Done." );
		}
	}

	/// <summary>
	/// テンポ変更のメタイベント
	/// </summary>
	class CMIDIBPM : CMIDIイベント
	{
		float fBPM;
		public CMIDIBPM( UInt32 _n時間, float _fBPM )
			: base()
		{
			this.nレーン番号 = 2;
			this.n時間 = _n時間;
			this.nWAV = 1;
			this.fBPM = _fBPM;

			this.eイベントタイプ = Eイベントタイプ.BPM;
		}

		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			//Debug.Write( "BPM   : " + ( n時間 * ( 192 / 4 ) / n四分音符の分解能 ).ToString() + ": " + fBPM );
			
			int nGrid = (int) n時間 * ( 192 / 4 ) / n四分音符の分解能;				// 全音符192tick相当で、曲先頭からのtick数(変拍子がない場合)
			mf.mgr編集モード管理者.tBPMチップを配置する( nGrid, fBPM );

			//Debug.WriteLine( " Done." );
		}
	}


	/// <summary>
	/// 拍子変更のメタイベント
	/// </summary>
	class CMIDIBARLen : CMIDIイベント
	{
		public CMIDIBARLen( UInt32 _n時間, int _分子, int _分母 )
		{
		}
		public override void 挿入( Cメインフォーム mf, int n四分音符の分解能 )
		{
			//	mf.mgr譜面管理者. 	public C小節 p譜面先頭からの位置gridを含む小節を返す( int n譜面先頭からの位置grid )
			throw new NotImplementedException();
		}
	}
}
