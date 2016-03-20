using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTXCreator.MIDIインポート
{
	class CMIDIチップ
    {
        public int nレーン番号;
        public int n時間;
        public int nWAV;
        public int nキー;
        public int nベロシティ;
        public int nベロシティ_DTX変換後;
        public bool b入力;
        public string strコメント;
        public CMIDI cMIDI;

        public string strWAV重複チェック
		{
			get
			{
				return "" + nキー.ToString() + " : " + nベロシティ_DTX変換後.ToString();
			}
		}

        public CMIDIチップ( CMIDI _cMIDI, int _n時間, int _nキー, int _nベロシティ )
        {
            this.nレーン番号 = 2;
            this.n時間 = _n時間;
            this.nWAV = 1;
            this.nキー = _nキー;
            this.cMIDI = _cMIDI;
			
            this.nベロシティ = _nベロシティ;
            this.nベロシティ_DTX変換後 = _nベロシティ;
        }
		
    }
}
