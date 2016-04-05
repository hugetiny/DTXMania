using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;
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
        public int[] nドラムチャンネルのキー毎のノート数;
		public int [] nチャンネル0to15毎のノート数;
        public int n分解能;
        public Cメインフォーム formメインフォーム;
		public List<CMIDIイベント> lMIDIWAV;
		public int n重複チップ数;
		public DataGridView dgvチャンネル一覧;
		public bool[] bドラムチャンネルと思われる;

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
            this.nドラムチャンネルのキー毎のノート数 = new int[256];
			this.lMIDIWAV = new List<CMIDIイベント>();
			this.f先頭BPM = 0.0f;
			this.n重複チップ数 = 0;
			this.nチャンネル0to15毎のノート数 = new int[17];
			this.dgvチャンネル一覧 = null;
			this.bドラムチャンネルと思われる = new bool[ 16 * 4 ];
			this.bドラムチャンネルと思われる[ 10 - 1 ] = true;
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

		// tMIDIチップをレーンに割り当てる()をCMIDIインポートダイアログに移動


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

            if ( dバイト数 <= 0 ) return "";

			byte[] by出力 = new byte[ dバイト数 + 1 ];

            for (int i = d開始バイト; i < d開始バイト + dバイト数; i++)
            {
                if ( i >= byバイナリ.Length ) break;
                by出力[i-d開始バイト] = byバイナリ[i];
            }

			if ( by出力[ 0 ] == 0 ) str文字列 = "";

			System.Text.Encoding enc = GetCode( by出力 );			//System.Text.Encoding.Default;	//GetEncoding( "shift_jis" );
			if ( enc == null )
			{
				enc = System.Text.Encoding.Unicode;
			}
			str文字列 = enc.GetString( by出力 );

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

		/// <summary>
		/// 文字コードを判別する
		/// </summary>
		/// <remarks>
		/// http://dobon.net/vb/dotnet/string/detectcode.html を少し修正したもの。
		/// Jcode.pmのgetcodeメソッドを移植したものです。
		/// Jcode.pm(http://openlab.ring.gr.jp/Jcode/index-j.html)
		/// Jcode.pmのCopyright: Copyright 1999-2005 Dan Kogai
		/// </remarks>
		/// <param name="bytes">文字コードを調べるデータ</param>
		/// <returns>適当と思われるEncodingオブジェクト。
		/// 判断できなかった時はnull。</returns>
		public static System.Text.Encoding GetCode( byte[] bytes )
		{
			const byte bEscape = 0x1B;
			const byte bAt = 0x40;
			const byte bDollar = 0x24;
			const byte bAnd = 0x26;
			const byte bOpen = 0x28;    //'('
			const byte bB = 0x42;
			const byte bD = 0x44;
			const byte bJ = 0x4A;
			const byte bI = 0x49;

			int len = bytes.Length;
			if (bytes[ len - 1 ] == 0x00 )
			{
				--len;
			}
			byte b1, b2, b3, b4;

			bool ascii = true;
			for ( int i = 0; i < len; i++ )
			{
				b1 = bytes[ i ];
				if ( !( b1 >= 0x20 && b1 <= 0x7E ) )
				{
					ascii = false;
					break;
				}
			}
			if ( ascii )
			{
				return System.Text.Encoding.ASCII;
			}

			//Encode::is_utf8 は無視

			bool isBinary = false;
			for ( int i = 0; i < len; i++ )
			{
				b1 = bytes[ i ];
				if ( b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF )
				{
					//'binary'
					isBinary = true;
					if ( b1 == 0x00 && i < len - 1 && bytes[ i + 1 ] <= 0x7F )
					{
						//smells like raw unicode
						return System.Text.Encoding.Unicode;
					}
				}
			}
			if ( isBinary )
			{
				return null;
			}

			//not Japanese
			bool notJapanese = true;
			for ( int i = 0; i < len; i++ )
			{
				b1 = bytes[ i ];
				if ( b1 == bEscape || 0x80 <= b1 )
				{
					notJapanese = false;
					break;
				}
			}
			if ( notJapanese )
			{
				return System.Text.Encoding.ASCII;
			}

			for ( int i = 0; i < len - 2; i++ )
			{
				b1 = bytes[ i ];
				b2 = bytes[ i + 1 ];
				b3 = bytes[ i + 2 ];

				if ( b1 == bEscape )
				{
					if ( b2 == bDollar && b3 == bAt )
					{
						//JIS_0208 1978
						//JIS
						return System.Text.Encoding.GetEncoding( 50220 );
					}
					else if ( b2 == bDollar && b3 == bB )
					{
						//JIS_0208 1983
						//JIS
						return System.Text.Encoding.GetEncoding( 50220 );
					}
					else if ( b2 == bOpen && ( b3 == bB || b3 == bJ ) )
					{
						//JIS_ASC
						//JIS
						return System.Text.Encoding.GetEncoding( 50220 );
					}
					else if ( b2 == bOpen && b3 == bI )
					{
						//JIS_KANA
						//JIS
						return System.Text.Encoding.GetEncoding( 50220 );
					}
					if ( i < len - 3 )
					{
						b4 = bytes[ i + 3 ];
						if ( b2 == bDollar && b3 == bOpen && b4 == bD )
						{
							//JIS_0212
							//JIS
							return System.Text.Encoding.GetEncoding( 50220 );
						}
						if ( i < len - 5 &&
							b2 == bAnd && b3 == bAt && b4 == bEscape &&
							bytes[ i + 4 ] == bDollar && bytes[ i + 5 ] == bB )
						{
							//JIS_0208 1990
							//JIS
							return System.Text.Encoding.GetEncoding( 50220 );
						}
					}
				}
			}

			//should be euc|sjis|utf8
			//use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
			int sjis = 0;
			int euc = 0;
			int utf8 = 0;
			for ( int i = 0; i < len - 1; i++ )
			{
				b1 = bytes[ i ];
				b2 = bytes[ i + 1 ];
				if ( ( ( 0x81 <= b1 && b1 <= 0x9F ) || ( 0xE0 <= b1 && b1 <= 0xFC ) ) &&
					( ( 0x40 <= b2 && b2 <= 0x7E ) || ( 0x80 <= b2 && b2 <= 0xFC ) ) )
				{
					//SJIS_C
					sjis += 2;
					i++;
				}
			}
			for ( int i = 0; i < len - 1; i++ )
			{
				b1 = bytes[ i ];
				b2 = bytes[ i + 1 ];
				if ( ( ( 0xA1 <= b1 && b1 <= 0xFE ) && ( 0xA1 <= b2 && b2 <= 0xFE ) ) ||
					( b1 == 0x8E && ( 0xA1 <= b2 && b2 <= 0xDF ) ) )
				{
					//EUC_C
					//EUC_KANA
					euc += 2;
					i++;
				}
				else if ( i < len - 2 )
				{
					b3 = bytes[ i + 2 ];
					if ( b1 == 0x8F && ( 0xA1 <= b2 && b2 <= 0xFE ) &&
						( 0xA1 <= b3 && b3 <= 0xFE ) )
					{
						//EUC_0212
						euc += 3;
						i += 2;
					}
				}
			}
			for ( int i = 0; i < len - 1; i++ )
			{
				b1 = bytes[ i ];
				b2 = bytes[ i + 1 ];
				if ( ( 0xC0 <= b1 && b1 <= 0xDF ) && ( 0x80 <= b2 && b2 <= 0xBF ) )
				{
					//UTF8
					utf8 += 2;
					i++;
				}
				else if ( i < len - 2 )
				{
					b3 = bytes[ i + 2 ];
					if ( ( 0xE0 <= b1 && b1 <= 0xEF ) && ( 0x80 <= b2 && b2 <= 0xBF ) &&
						( 0x80 <= b3 && b3 <= 0xBF ) )
					{
						//UTF8
						utf8 += 3;
						i += 2;
					}
				}
			}
			//M. Takahashi's suggestion
			//utf8 += utf8 / 2;

			//Debug.WriteLine(
			//	string.Format( "sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8 ) );
			if ( euc > sjis && euc > utf8 )
			{
				//EUC
				return System.Text.Encoding.GetEncoding( 51932 );
			}
			else if ( sjis > euc && sjis > utf8 )
			{
				//SJIS
				return System.Text.Encoding.GetEncoding( 932 );
			}
			else if ( utf8 > euc && utf8 > sjis )
			{
				//UTF8
				return System.Text.Encoding.UTF8;
			}

			return null;
		}
	}

}
