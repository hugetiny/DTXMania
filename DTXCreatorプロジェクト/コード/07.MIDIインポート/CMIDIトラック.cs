using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace DTXCreator.MIDIインポート
{
    class CMIDIトラック
    {
        public string strトラック名;
        public int nトラック数;
        public int nデータ長;
        public int nチャンネル;
        public byte[] byMIDIトラックバイナリ;
        public string str解析内容;
        public CMIDI cMIDI;

        public CMIDIトラック( CMIDI _cMIDI, int _dトラック数, byte[] _byMIDIトラックバイナリ )
        {
            this.strトラック名 = "";
            this.nトラック数 = _dトラック数;
            this.nデータ長 = 0;
            this.nチャンネル = -1;
            this.byMIDIトラックバイナリ = _byMIDIトラックバイナリ;
            this.cMIDI = _cMIDI;
			this.str解析内容 = "";

            this.nデータ長 = CMIDI.nBin2Int( this.byMIDIトラックバイナリ, 4, 4 );
        }

        public void tトラックチャンクを走査する()
        {
            int p = 8;
            UInt32 nデルタタイム合計 = 0;
			int nイベントbefore = 0;

            while( true )
            {
				// デルタタイム計算
				int nデルタタイムLen;
				UInt32 deltatime;
				GetVarLen( p, out nデルタタイムLen, out deltatime );
				nデルタタイム合計 += deltatime;

				// イベント
                int nイベント = this.byMIDIトラックバイナリ[p+nデルタタイムLen];
                int nイベントLen = 3;

				// デルタタイムの直後がイベントじゃなかったら、前のイベントを継ぐ
				if ( nイベント < 0x80 )
				{
					nイベント = nイベントbefore;
					p -= 1;
				}
                
                // 8n - 9n ノートオフ・オン
                if ( 0x80 <= nイベント && nイベント <= 0x9F )
                {
                    int nData1 = this.byMIDIトラックバイナリ[p+nデルタタイムLen+1];
                    int nData2 = this.byMIDIトラックバイナリ[p+nデルタタイムLen+2];
                    // ノートオン(9n)の時の値を取得 (ノートオンでも、ベロシティ0の場合はノートオフの意味なので、無視する)
                    if ( nイベント >= 0x90 && nData2 > 0 )
                    {
                        this.nチャンネル = nイベント - 0x90 + 1;
						cMIDI.lチャンネル毎のノート数1to16[this.nチャンネル] ++;
						bool bAdd = false;

						//for (int i = 1; i <= 16; i++)
						//{
						//	if ( (bool)cMIDI.dgvチャンネル一覧.Rows[i-1].Cells["ChLoad"].Value && this.nチャンネル == i )
						//		bAdd = true;
						//}
						if ( (bool)cMIDI.dgvチャンネル一覧.Rows[this.nチャンネル-1].Cells["ChLoad"].Value )
							bAdd = true;

						if ( bAdd )
                        {
                            cMIDI.lMIDIイベント.Add( new CMIDINote( nデルタタイム合計, nData1, nData2 ) );
                            cMIDI.nドラム各ノート数[nData1]++;
							//this.str解析内容 += "Drum  / Tick: " + nデルタタイム合計.ToString().PadLeft( 6 ) + " Note: " + nData1.ToString( "X2" ) + "\r\n";
                        }
					}
					//this.str解析内容 += ((nイベント>=144)?"N-ON ":"N-OFF")+" "+p.ToString().PadLeft(6)+" "+nデルタタイム[0]+","+nData1.ToString("X2")+","+nData2.ToString("X2")+"\r\n";
                    
                    nイベントLen = 3;
                }
                // A0 - EF コントロールチェンジ等
                else if ( 0xA0 <= nイベント && nイベント <= 0xEF )
                {
                    int nData1 = this.byMIDIトラックバイナリ[p+nデルタタイムLen+1];
                    int nData2 = this.byMIDIトラックバイナリ[p+nデルタタイムLen+2];
                    
                    nイベントLen = 3;
                    if ( 0xC0 <= nイベント && nイベント < 0xDF ) nイベントLen = 2;

                    //this.str解析内容 += "CC    / Tick: "+nデルタタイム合計.ToString().PadLeft(6)+" Type: "+nData1.ToString("X2")+"\r\n";

					// 面倒なので、コントロールチェンジが3byteになる場合はとりあえず想定しない。相当レアだし。
                }
                // F0/F7 System Exclusive Message
                else if ( nイベント == 0xF0 || nイベント == 0xF7)
                {
					UInt32 nF0F7データLen;
					int nデータLenのLen;
					int pt = p + nデルタタイムLen + 1;	// F0/F7 の次のバイト(データ長が記載)
					GetVarLen( p + nデルタタイムLen + 1, out nデータLenのLen, out nF0F7データLen );

					pt += nデータLenのLen;
					
					tドラムチャンネルかどうか推測する( pt, nデルタタイムLen );
					
					nイベントLen = 1 + nデータLenのLen + (int)nF0F7データLen;		// "F0orF7"(1byte) + 可変長のデータバイト数 + nF0F7データ長(F0の場合はF7込み)
					string str = CMIDI.strBin2BinStr( this.byMIDIトラックバイナリ, p + nデルタタイムLen, nイベントLen );
	
					//this.str解析内容 += "Sys   / Tick: "+nデルタタイム合計.ToString().PadLeft(6)+" Val : "+str+"\r\n";
					//Debug.WriteLine( this.str解析内容 += "Sys   / Tick: " + nデルタタイム合計.ToString().PadLeft( 6 ) + " Val : " + str );
				
				}
				// FF メタイベント
				else if ( nイベント == 0xFF )
				{
					int nType = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 1 ];
					UInt32 nLen = 0;

					switch ( nType )
					{
						// FF 01 - FF 07
						case 0x01:
						case 0x02:
						case 0x03:
						case 0x04:
						case 0x05:
						case 0x06:
						case 0x07:
							{
								int nLenのLen;
								GetVarLen( p + nデルタタイムLen + 2 , out nLenのLen, out nLen );
								//nLen = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 2 ];
								string str1 = CMIDI.strBin2Str( this.byMIDIトラックバイナリ, p + nデルタタイムLen + 2 + nLenのLen, (int)nLen );
								if ( nType == 0x03 && this.strトラック名 == "" ) this.strトラック名 = str1;		// 最初のトラック名以外は捨てる
								nイベントLen = 2 + nLenのLen + (int)nLen;
							}
							break;

						// FF 20 - FF 21
						case 0x20:
						case 0x21:
							nイベントLen = 4;
							break;

						// FF 2F EOT
						case 0x2F:
							nイベントLen = 0;
							break;

						// FF 51 BPM
						case 0x51:
							float fBPM = (float) ( Math.Round( (float) 60.0 * Math.Pow( 10, 6 ) / CMIDI.nBin2Int( this.byMIDIトラックバイナリ, p + nデルタタイムLen + 3, 3 ), 2 ) );
							if ( cMIDI.f先頭BPM == 0.0f ) cMIDI.f先頭BPM = fBPM;
							nイベントLen = 6;
							cMIDI.lMIDIイベント.Add( new CMIDIBPM( nデルタタイム合計, fBPM ) );
							cMIDI.nドラム各ノート数[ 128 ]++;
							break;

						// FF 54 SMPTEオフセット
						//case 0x54:
						//	nイベントLen = 8;
						//	break;

						// FF 58 拍設定
						case 0x58:
							int n分子 = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 3 ];
							int n分母 = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 4 ];
							n分母 = (int) Math.Pow( 2, n分母 );

							int nメトロノームクリックtick = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 5 ];
							int nメトロノームクリック数内32分音符数 = this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 6 ];

							cMIDI.strTimeSignature = CMIDI.strBin2BinStr( this.byMIDIトラックバイナリ, p + nデルタタイムLen + 3, 4 );
							nイベントLen = 7;

							cMIDI.lMIDIイベント.Add( new CMIDIBARLen( nデルタタイム合計, n分子, n分母 ) );
							cMIDI.nドラム各ノート数[ 128 ]++;
							break;

						// FF 59
						case 0x59:
							nイベントLen = 5;
							break;
						default:
							nイベントLen = 3 + this.byMIDIトラックバイナリ[ p + nデルタタイムLen + 3 ];
							break;

					}

					//this.str解析内容 += "Event / Tick: "+nデルタタイム合計.ToString().PadLeft(6)+" Type: "+nType.ToString("X2")+"\r\n";
				}
				nイベントbefore = nイベント;
                
                p += nデルタタイムLen + nイベントLen;
                
                if ( nイベントLen == 0 ) // EOT
                {
                    //this.str解析内容 += "End Of Track\r\n";
                    break;
                }
                if ( p >= this.nデータ長 + 8 )
                {
                    //this.str解析内容 += "Pointer Over "+p+"\r\n";
                    break;
                }
            }
        }

		private void GetVarLen( int p, out int nデルタタイムLen, out UInt32 deltatime )
		{
			nデルタタイムLen = 0;
			deltatime = 0;
			for ( int i = 0; i < 4; i++ )		// デルタタイムは最大4byte
			{
				++nデルタタイムLen;
				UInt32 b = this.byMIDIトラックバイナリ[ p + i ];
				deltatime <<= 7;
				deltatime += ( b & 0x7F );		// 下位7bitのみ使用
				if ( b < 0x80 ) break;			// MSBが0になったらデルタタイム終了
			}
		}

		/// <summary>
		/// ドラムチャンネルを複数使用していると思わるデータについて、ドラムチャンネルと思われるチャンネルを識別する。
		/// Roland GS音源を使用した曲データで典型的なパターンのみ対応。
		/// </summary>
		/// <param name="p"></param>
		/// <param name="nデルタタイムLen"></param>
		private void tドラムチャンネルかどうか推測する( int p, int nデルタタイムLen )
		{
			int pt = p;
			if ( this.byMIDIトラックバイナリ[ pt + 0 ] == 0x41 &&				// Manufacturer ID == Roland
			//   this.byMIDIトラックバイナリ[ pt + 1 ] == 0xxx &&				// Dev ID == 通常0x10だが0x7F(broadcast)とかもあるかも
				 this.byMIDIトラックバイナリ[ pt + 2 ] == 0x42 &&				// Model ID == 0x42 (GS Format)
				 this.byMIDIトラックバイナリ[ pt + 3 ] == 0x12 )				// Command ID == 0x12 (Data Set 1)
			{
				if ( this.byMIDIトラックバイナリ[ pt + 4 ] == 0x40 &&				// USE FOR RHYTHM PART
					( this.byMIDIトラックバイナリ[ pt + 5 ] & 0xF0 ) == 0x10 &&		//
					 this.byMIDIトラックバイナリ[ pt + 6 ] == 0x15 )				//
				{
					if ( this.byMIDIトラックバイナリ[ pt + 7 ] == 0x01 ||				// 01=MAP1(Drum Part),
						 this.byMIDIトラックバイナリ[ pt + 7 ] == 0x02 )				// 02=MAP2(Drum Part)
					{
						int ch = this.byMIDIトラックバイナリ[ pt + 5 ] & 0x0F;
						cMIDI.bドラムチャンネルと思われる[ ch ] = true;
//Debug.WriteLine( "USE FOR RHYTHM PART: ch" + ch + "="+this.byMIDIトラックバイナリ[pt+7] );
					}
					else
					if ( this.byMIDIトラックバイナリ[ pt + 7 ] == 0x00 )				// 00:OFF(Normal Part)
					{
						int ch = this.byMIDIトラックバイナリ[ pt + 5 ] & 0x0F;
						cMIDI.bドラムチャンネルと思われる[ ch ] = false;
//Debug.WriteLine( "USE FOR RHYTHM PART: ch" + ch + "="+this.byMIDIトラックバイナリ[pt+7] );
					}
				}
				if ( this.byMIDIトラックバイナリ[ pt + 4 ] == 0x40 &&				// Rx CHANNEL
					( this.byMIDIトラックバイナリ[ pt + 5 ] & 0xF0 ) == 0x10 &&		//
					 this.byMIDIトラックバイナリ[ pt + 6 ] == 0x02 )				//
				{
					int org    = this.byMIDIトラックバイナリ[ pt + 5 ] & 0x0F;			//
					int target = this.byMIDIトラックバイナリ[ pt + 7 ];					//
					if (cMIDI.bドラムチャンネルと思われる[org] == true )
					{
						cMIDI.bドラムチャンネルと思われる[ target ] = true;
Debug.WriteLine( "Rx CHANNEL: chorg" + org + ", chTarget=" + target );
					}
				}
			}
		}
    }
    
}
