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
        public List<CMIDIイベント> lMIDIイベント;
        public float f先頭BPM;
        public string strTimeSignature;
        public int[] nドラム各ノート数;
        public int n分解能;
        public Cメインフォーム formメインフォーム;
		public List<CMIDIイベント> lMIDIWAV;
		public int n読み込みCh;
		public int n重複チップ数;
		public int [] lチャンネル毎のノート数1to16;
		public DataGridView dgvチャンネル一覧;

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
            this.lMIDIイベント = new List<CMIDIイベント>();
            this.nドラム各ノート数 = new int[256];
			this.lMIDIWAV = new List<CMIDIイベント>();
			this.f先頭BPM = 0.0f;
			this.n重複チップ数 = 0;
			this.lチャンネル毎のノート数1to16 = new int[17];
			this.dgvチャンネル一覧 = null;
        }

        // 解析処理 全バイナリを見てMTrkだけ抜き取る
        public void tMIDIを解析する()
        {
            // MThdが存在しなければ解析処理を行わない
            if ( !bMIDIファイル ) return;
			this.lMIDIWAV = new List<CMIDIイベント>();

			this.n分解能 = nBin2Int( this.byMIDIバイナリ, 12, 2 );

            for ( int i = 0; i<this.byMIDIバイナリ.Length; i++ )
            {
                // MTrkがあればトラックを追加する
                if ( strBin2BinStr(this.byMIDIバイナリ, i, 4) == "4D 54 72 6B" )
                {
					int track_size = nBin2Int( this.byMIDIバイナリ, i + 4, 4 );
                    byte[] data_track = new byte[track_size + 8 + 8];	// 大きめに取りました
                    Array.Copy( this.byMIDIバイナリ, i, data_track, 0, nBin2Int( this.byMIDIバイナリ, i+4, 4 ) + 8 );
                    this.lMIDIトラック.Add( new CMIDIトラック( this, this.dトラック数, data_track ) );
                    this.lMIDIトラック[this.lMIDIトラック.Count-1].tトラックチャンクを走査する();
                }
            }
        }
		
		/// <summary>
		/// dgv割り当て一覧で設定した値に応じて、各レーンへ振り分ける
		/// </summary>
        public void tMIDIチップをレーンに割り当てる( DataGridView dgv割り当て一覧 )
        {
			// MIDIイベントがひとつでもあるなら処理する
			if ( this.lMIDIイベント.Count == 0 ) return;

			#region [ 振り分け ]
			foreach ( CMIDIイベント vMIDIイベント in this.lMIDIイベント )
			{
				foreach (DataGridViewRow dgvr in dgv割り当て一覧.Rows)
				{
					if (vMIDIイベント.nキー == (int)dgvr.Cells["MIDI_Key"].Value )
					{
						if ( (string)dgvr.Cells["DTX_Lane"].Value != "* Disuse *" )
						{
							vMIDIイベント.nレーン番号 = this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( (string)dgvr.Cells["DTX_Lane"].Value );
							vMIDIイベント.strコメント = (string)dgvr.Cells["Comment"].Value;
							vMIDIイベント.b裏チャンネル = (bool)dgvr.Cells["BackCH"].Value;
							vMIDIイベント.b入力 = true;
						}
						else
						{
							vMIDIイベント.nレーン番号 = 0;
							vMIDIイベント.strコメント = "";
							vMIDIイベント.b裏チャンネル = false;
							vMIDIイベント.b入力 = false;
						}
						if ( vMIDIイベント.eイベントタイプ == CMIDIイベント.Eイベントタイプ.BPM  ||
							 vMIDIイベント.eイベントタイプ == CMIDIイベント.Eイベントタイプ.BarLen )
						{
							vMIDIイベント.b入力 = true;
						}
					}
				}
			}
			#endregion

			#region [ WAVリスト化する ]
			this.lMIDIWAV = new List<CMIDIイベント>();

			foreach ( CMIDIイベント vMIDIイベント in this.lMIDIイベント )
			{
				// WAVリストで、同じ内容(キーとベロシティ)が無ければ挿入する
				bool bMIDIWAV_AddFlag = true;
				foreach ( CMIDIイベント vMIDIWAV in this.lMIDIWAV )
				{
					if ( vMIDIWAV.strWAV重複チェック == vMIDIイベント.strWAV重複チェック )
					{
						bMIDIWAV_AddFlag = false;
						break;
					}
				}
				if (bMIDIWAV_AddFlag)
				{
					this.lMIDIWAV.Add( vMIDIイベント );
				}
			}
			#endregion
			
			#region [ キーが違うが同時刻で同じレーンに配置予定のチップを数える ]
			this.n重複チップ数 = 0;
			foreach ( CMIDIイベント v1 in this.lMIDIイベント )
			{
				foreach ( CMIDIイベント v2 in this.lMIDIイベント )
				{
					if ( v1.nキー != v2.nキー && v1.nレーン番号 == v2.nレーン番号 && v1.n時間 == v2.n時間 )
					{
						this.n重複チップ数 ++;
					}
				}
			}
			this.n重複チップ数 /= 2;
			#endregion
        }

		//public CMIDIイベント pMIDIチップで一番遅い時間のチップを返す()
		//{
		//	if (this.lMIDIイベント.Count == 0) return null;

		//	CMIDIイベント cMIDIチップ = null;
		//	foreach ( CMIDIイベント vMIDIイベント in this.lMIDIイベント )
		//	{
		//		if ( cMIDIチップ == null || cMIDIチップ.n時間 <= vMIDIイベント.n時間 )
		//		{
		//			cMIDIチップ = vMIDIイベント;
		//		}
		//	}
		//	return cMIDIチップ;
		//}

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
