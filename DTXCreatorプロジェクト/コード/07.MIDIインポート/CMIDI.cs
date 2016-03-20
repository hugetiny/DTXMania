using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using DTXCreator.WAV_BMP_AVI;

namespace DTXCreator.MIDIインポート
{
    internal class CMIDI
    {
        public string strファイル名;
        public byte[] byMIDIバイナリ;
        public bool bMIDIファイル;
        public List<CMIDIトラック> lMIDIトラック;
        public List<CMIDIチップ> lチップ;
        public double dBPM;
        public string strTimeSignature;
        public int[] nドラム各ノート数;
        public int n分解能;
        public Cメインフォーム formメインフォーム;
		public List<CMIDIチップ> lMIDIWAV;
		public int n読み込みCh;

        public int dトラック数
        {
            get
            {
                return lMIDIトラック.Count;
            }
        }

        public CMIDI( string _strファイル名 )
        {
            this.strファイル名 = _strファイル名;
            this.byMIDIバイナリ = File.ReadAllBytes( this.strファイル名 );
            this.bMIDIファイル = ( strBin2BinStr(this.byMIDIバイナリ, 0, 4) == "4D 54 68 64" );
            this.lMIDIトラック = new List<CMIDIトラック>();
            this.lチップ = new List<CMIDIチップ>();
            this.nドラム各ノート数 = new int[128];
			this.lMIDIWAV = new List<CMIDIチップ>();
        }

        // 解析処理 全バイナリを見てMTrkだけ抜き取る
        public void tMIDIを解析する()
        {
            // MThdが存在しなければ解析処理を行わない
            if ( !bMIDIファイル ) return;
			this.lMIDIWAV = new List<CMIDIチップ>();

			this.n分解能 = nBin2Int( this.byMIDIバイナリ, 12, 2 );

            for ( int i = 0; i<this.byMIDIバイナリ.Length; i++ )
            {
                // MTrkがあればトラックを追加する
                if ( strBin2BinStr(this.byMIDIバイナリ, i, 4) == "4D 54 72 6B" )
                {
                    byte[] data_track = new byte[65536];
                    Array.Copy( this.byMIDIバイナリ, i, data_track, 0, nBin2Int( this.byMIDIバイナリ, i+4, 4 ) + 8 );
                    this.lMIDIトラック.Add( new CMIDIトラック( this, this.dトラック数, data_track ) );
                    this.lMIDIトラック[this.lMIDIトラック.Count-1].tトラックチャンクを走査する();
                }
            }
        }

		// DataGridViewに設定した値に応じて各レーンに振り分ける
        public void tMIDIチップをレーンに割り当てる( DataGridView dgv )
        {
			foreach ( CMIDIチップ vMIDIチップ in this.lチップ )
			{
				foreach (DataGridViewRow dgvr in dgv.Rows)
				{
					if (vMIDIチップ.nキー == (int)dgvr.Cells[0].Value && (string)dgvr.Cells[2].Value != "* Disuse *")
					{
						vMIDIチップ.nレーン番号 = this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( (string)dgvr.Cells[2].Value );
						vMIDIチップ.strコメント = (string)dgvr.Cells[4].Value;
						vMIDIチップ.b入力 = true;
					}
				}
			}
			tMIDIチップをMIDIWAVリスト化する();
        }

        public void tMIDIチップをMIDIWAVリスト化する()
        {
			this.lMIDIWAV = new List<CMIDIチップ>();

			foreach ( CMIDIチップ vMIDIチップ in this.lチップ )
			{
				// WAVリストで、同じ内容(キーとベロシティ)が無ければ挿入する
				bool bMIDIWAV_AddFlag = true;
				foreach ( CMIDIチップ vMIDIWAV in this.lMIDIWAV )
				{
					if ( vMIDIWAV.strWAV重複チェック == vMIDIチップ.strWAV重複チェック )
					{
						bMIDIWAV_AddFlag = false;
						break;
					}
				}
				if (bMIDIWAV_AddFlag)
				{
					this.lMIDIWAV.Add( vMIDIチップ );
				}
			}
        }

		// レーン割り当て後に呼ぶこと
		public int nMIDI重複チップ数を返す()
		{
			if (this.lチップ.Count == 0) return 0;

			int nMIDIチップ同時刻同レーン重複 = 0;
			foreach ( CMIDIチップ vMIDIチップ1 in this.lチップ )
			{
				foreach ( CMIDIチップ vMIDIチップ2 in this.lチップ )
				{
					if ( vMIDIチップ1.nキー != vMIDIチップ2.nキー && vMIDIチップ1.nレーン番号 == vMIDIチップ2.nレーン番号 && vMIDIチップ1.n時間 == vMIDIチップ2.n時間 )
					{
						nMIDIチップ同時刻同レーン重複 ++;
					}
				}
			}
			return nMIDIチップ同時刻同レーン重複/2;
		}

		public CMIDIチップ pMIDIチップで一番遅い時間のチップを返す()
		{
			if (this.lチップ.Count == 0) return null;

			CMIDIチップ cMIDIチップ = null;
			foreach ( CMIDIチップ vMIDIチップ in this.lチップ )
			{
				if ( cMIDIチップ == null || cMIDIチップ.n時間 <= vMIDIチップ.n時間 )
				{
					cMIDIチップ = vMIDIチップ;
				}
			}
			return cMIDIチップ;
		}

        // バイナリの指定のバイト数分を、"FF FF FF..."の形で出力する
        static public string strBin2BinStr( byte[] byバイナリ, int d開始バイト, int dバイト数 )
        {
            string str文字列 = "";

            if ( dバイト数 <= 0 ) return "";

            for (int i = d開始バイト; i < d開始バイト + dバイト数; i++)
            {
                if ( i >= byバイナリ.Length ) break;
                str文字列 += byバイナリ[i].ToString("X2") + " ";
            }

            return str文字列.Substring( 0, str文字列.Length-1 );
        }
        
        // バイナリの指定のバイト数分を、文字列に変換して出力する
        static public string strBin2Str( byte[] byバイナリ, int d開始バイト, int dバイト数 )
        {
            string str文字列 = "";
            char[] by出力 = new char[128];

            if ( dバイト数 <= 0 ) return "";

            for (int i = d開始バイト; i < d開始バイト + dバイト数; i++)
            {
                if ( i >= byバイナリ.Length ) break;
                by出力[i-d開始バイト] = (char)byバイナリ[i];
            }
            str文字列 = new string(by出力);
            if ( by出力[0] == 0 ) str文字列 = "";

            return str文字列.Trim('\0');
        }
        
        // バイナリの指定のバイト数分を、数値に変換して出力する
        static public int nBin2Int( byte[] byバイナリ, int d開始バイト, int dバイト数 )
        {
            int d数値 = 0;

            if ( dバイト数 <= 0 ) return 0;

            for (int i = d開始バイト; i < d開始バイト + dバイト数; i++)
            {
                if ( i >= byバイナリ.Length ) break;
                d数値 += byバイナリ[i] * (int)Math.Pow( 256, dバイト数 - ( i - d開始バイト ) - 1 );
            }

            return d数値;
        }
        
    }

}
