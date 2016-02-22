using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTXMania
{
	public class CChip : IComparable<CChip>, ICloneable
	{
		public bool bHit;
		public bool b可視;
		public double dbチップサイズ倍率;
		public double db実数値;
		public EAVI種別 eAVI種別;
		public EBGA種別 eBGA種別;
		public E楽器パート e楽器パート;
		public Ech定義 eチャンネル番号;
		public STDGBVALUE<int> nバーからの距離dot;
		public int n整数値;
		public int n整数値_内部番号;
		public int n総移動時間;
		public int n透明度;
		public int n発声位置;
		public int n発声時刻ms;
		public int nLag;				// 2011.2.1 yyagi
		public int nCurrentComboForGhost; // 2015.9.29 chnmr0
		public CDTX.CAVI rAVI;
		public CDTX.CAVIPAN rAVIPan;
		public CDTX.CBGA rBGA;
		public CDTX.CBGAPAN rBGAPan;
		public CDTX.CBMP rBMP;
		public CDTX.CBMPTEX rBMPTEX;
		public bool bIsAutoPlayed;							// 2011.6.10 yyagi
		public bool b演奏終了後も再生が続くチップである;	// #32248 2013.10.14 yyagi
		public bool b空打ちチップである;					// #34029 2014.7.15 yyagi
		public int n楽器パートでの出現順;                // #35411 2015.08.20 chnmr0
		public bool bTargetGhost判定済み;               // #35411 2015.08.22 chnmr0

		/// <summary>
		/// このチップがベースBPM、拡張BPM指示チップであることを示す。
		/// </summary>
		public bool bBPMチップである
		{
			get
			{
				return (
				  this.eチャンネル番号 == Ech定義.BPM ||
				  this.eチャンネル番号 == Ech定義.BPMEx
				);
			}
		}

		/// <summary>
		/// 可視 HHC~LBD
		/// </summary>
		public bool bDrums可視チップである
		{
			get
			{
				return Ech定義.HiHatClose <= eチャンネル番号 && eチャンネル番号 <= Ech定義.LeftBassDrum;
			}
		}


		/// <summary>
		/// 可視 HHC~LC
		/// </summary>
		public bool bDrums可視チップであるLP_LBD含まない
		{
			get
			{
				return Ech定義.HiHatClose <= eチャンネル番号 && eチャンネル番号 <= Ech定義.LeftCymbal;
			}
		}


		/// <summary>
		/// 不可視 HHC~LBD
		/// </summary>
		public bool bDrums不可視チップである
		{
			get
			{
				return Ech定義.HiHatClose_Hidden <= eチャンネル番号 && eチャンネル番号 <= Ech定義.LeftBassDrum_Hidden;
			}
		}


		/// <summary>
		/// 空打ち HHC~LBD
		/// </summary>
		public bool bDrums空打ちチップである
		{
			get
			{
				return
					(Ech定義.HiHatClose_NoChip <= eチャンネル番号 && eチャンネル番号 <= Ech定義.RideCymbal_NoChip) ||
					(Ech定義.LeftCymbal_NoChip <= eチャンネル番号 && eチャンネル番号 <= Ech定義.LeftBassDrum_NoChip);
			}
		}

		/// <summary>
		/// このチップがギター/バスチップであることが前提で、
		/// そのRGBパターンを返す。
		/// </summary>
		public Eレーンビットパターン eRGBビットパターン
		{
			get
			{
				return (Eレーンビットパターン)((int)eチャンネル番号 & 7);
			}
		}

		/// <summary>
		/// Bass 可視チップであり、上位ビットのみ受け取る。
		/// Bass 可視チップ判定に使用してはならない。
		/// </summary>
		public int nGuitarBassUpper4Bit
		{
			get
			{
				return (int)eチャンネル番号 & 0xF0;
			}
		}

		/// <summary>
		/// Gutiar Open ~ Guitar RGB
		/// </summary>
		public bool bGuitar可視チップ
		{
			get
			{
				return Ech定義.Guitar_Open <= eチャンネル番号 && eチャンネル番号 <= Ech定義.Guitar_Wailing;
			}
		}

		public bool bGuitar可視チップ_Wailing含まない
		{
			get
			{
				return Ech定義.Guitar_Open <= eチャンネル番号 && eチャンネル番号 <= Ech定義.Guitar_RGB;
			}
		}


		/// <summary>
		/// Bass Open ~ Bass RGB
		/// </summary>
		public bool bBass可視チップ
		{
			get
			{
				return Ech定義.Bass_Open <= eチャンネル番号 && eチャンネル番号 <= Ech定義.Bass_Wailing;
			}
		}

		public bool bBass可視チップ_Wailing含まない
		{
			get
			{
				return Ech定義.Bass_Open <= eチャンネル番号 && eチャンネル番号 <= Ech定義.Bass_RGB;
			}
		}

		/// <summary>
		/// Guitar or Bass Open ~ RGB
		/// </summary>
		public bool bGuitarBass可視チップ
		{
			get
			{
				return bGuitar可視チップ || bBass可視チップ;
			}
		}

		/// <summary>
		/// Guitar or Bass has R pattern
		/// </summary>
		public bool bGuitarBass_R
		{
			get
			{
				return bGuitarBass可視チップ && (((int)eチャンネル番号 & 4) > 0);
			}
		}

		public bool bGuitarBass_G
		{
			get
			{
				return bGuitarBass可視チップ && (((int)eチャンネル番号 & 2) > 0);
			}
		}

		public bool bGuitarBass_B
		{
			get
			{
				return bGuitarBass可視チップ && (((int)eチャンネル番号 & 1) > 0);
			}
		}

		public bool bGuitarBass_Wailing
		{
			get
			{
				return bGuitar可視チップ && (((int)eチャンネル番号 & 0x0F) == 0x08);
			}
		}

		public bool bGuitarBass_Open
		{
			get
			{
				return bGuitar可視チップ && (((int)eチャンネル番号 & 0x0F) == 0x00);
			}
		}


		/// <summary>
		/// このチップが音声と関連付けられたチップであることを示す。
		/// </summary>
		public bool bWAVを使うチャンネルである
		{
			get
			{
				bool ret = false;
				Ech定義 x = eチャンネル番号;

				if (x == Ech定義.BGM ||
					(Ech定義.HiHatClose <= x && x <= Ech定義.LeftBassDrum) ||
					(Ech定義.DrumsFillin == x) ||
					(Ech定義.Guitar_Open <= x && x <= Ech定義.Guitar_Wailing) || x == Ech定義.Guitar_WailingSound ||
					(Ech定義.HiHatClose_Hidden <= x && x <= Ech定義.LeftBassDrum_Hidden) ||
					(Ech定義.SE01 <= x && x <= Ech定義.SE09) ||
					(Ech定義.SE10 <= x && x <= Ech定義.SE19) ||
					(Ech定義.SE20 <= x && x <= Ech定義.SE29) ||
					(Ech定義.SE30 <= x && x <= Ech定義.SE32) ||
					(Ech定義.Bass_Open <= x && x <= Ech定義.Bass_Wailing) || x == Ech定義.Bass_WailingSound ||
					(Ech定義.HiHatClose_NoChip <= x && x <= Ech定義.LeftBassDrum_NoChip)
					)
				{
					ret = true;
				}
				return ret;
			}
		}

		/// <summary>
		/// このチップが持つチャンネルの ESoundChipType を返す。
		///  チャンネル番号→Chipの種類、の変換。今後の拡張を容易にするために追加
		/// </summary>
		public ESoundChipType ESoundChipTypeを得る
		{
			get
			{
				switch (this.eチャンネル番号)
				{
					#region [ Drums ]
					case Ech定義.HiHatClose:
					case Ech定義.Snare:
					case Ech定義.BassDrum:
					case Ech定義.HighTom:
					case Ech定義.LowTom:
					case Ech定義.Cymbal:
					case Ech定義.FloorTom:
					case Ech定義.HiHatOpen:
					case Ech定義.RideCymbal:
					case Ech定義.LeftCymbal:
					case Ech定義.LeftPedal:
					case Ech定義.LeftBassDrum:

					case Ech定義.HiHatClose_Hidden:
					case Ech定義.Snare_Hidden:
					case Ech定義.BassDrum_Hidden:
					case Ech定義.HighTom_Hidden:
					case Ech定義.LowTom_Hidden:
					case Ech定義.Cymbal_Hidden:
					case Ech定義.FloorTom_Hidden:
					case Ech定義.HiHatOpen_Hidden:
					case Ech定義.RideCymbal_Hidden:
					case Ech定義.LeftCymbal_Hidden:
					case Ech定義.LeftPedal_Hidden:
					case Ech定義.LeftBassDrum_Hidden:

					case Ech定義.HiHatClose_NoChip:
					case Ech定義.Snare_NoChip:
					case Ech定義.BassDrum_NoChip:
					case Ech定義.HighTom_NoChip:
					case Ech定義.LowTom_NoChip:
					case Ech定義.Cymbal_NoChip:
					case Ech定義.FloorTom_NoChip:
					case Ech定義.HiHatOpen_NoChip:
					case Ech定義.RideCymbal_NoChip:
					case Ech定義.LeftCymbal_NoChip:
					case Ech定義.LeftPedal_NoChip:
					case Ech定義.LeftBassDrum_NoChip:

						return ESoundChipType.Drums;
					#endregion

					#region [ Guitar ]
					case Ech定義.Guitar_Open:
					case Ech定義.Guitar_xxB:
					case Ech定義.Guitar_xGx:
					case Ech定義.Guitar_xGB:
					case Ech定義.Guitar_Rxx:
					case Ech定義.Guitar_RxB:
					case Ech定義.Guitar_RGx:
					case Ech定義.Guitar_RGB:
					case Ech定義.Guitar_Wailing:
					case Ech定義.Guitar_WailingSound:
					case Ech定義.Guitar_NoChip:
						return ESoundChipType.Guitar;
					#endregion

					#region [ Bass ]
					case Ech定義.Bass_Open:
					case Ech定義.Bass_xxB:
					case Ech定義.Bass_xGx:
					case Ech定義.Bass_xGB:
					case Ech定義.Bass_Rxx:
					case Ech定義.Bass_RxB:
					case Ech定義.Bass_RGx:
					case Ech定義.Bass_RGB:
					case Ech定義.Bass_Wailing:
					case Ech定義.Bass_WailingSound:
					case Ech定義.Bass_NoChip:
						return ESoundChipType.Bass;
					#endregion

					#region [ SE ]
					case Ech定義.SE01:
					case Ech定義.SE02:
					case Ech定義.SE03:
					case Ech定義.SE04:
					case Ech定義.SE05:
					case Ech定義.SE06:
					case Ech定義.SE07:
					case Ech定義.SE08:
					case Ech定義.SE09:
					case Ech定義.SE10:
					case Ech定義.SE11:
					case Ech定義.SE12:
					case Ech定義.SE13:
					case Ech定義.SE14:
					case Ech定義.SE15:
					case Ech定義.SE16:
					case Ech定義.SE17:
					case Ech定義.SE18:
					case Ech定義.SE19:
					case Ech定義.SE20:
					case Ech定義.SE21:
					case Ech定義.SE22:
					case Ech定義.SE23:
					case Ech定義.SE24:
					case Ech定義.SE25:
					case Ech定義.SE26:
					case Ech定義.SE27:
					case Ech定義.SE28:
					case Ech定義.SE29:
					case Ech定義.SE30:
					case Ech定義.SE31:
					case Ech定義.SE32:
						return ESoundChipType.SE;
					#endregion

					#region [ BGM ]
					case Ech定義.BGM:
						return ESoundChipType.BGM;
					#endregion

					#region [ その他 ]
					default:
						return ESoundChipType.UNKNOWN;
					#endregion
				}
			}
		}
		public bool bIsVisibleChip
		{
			get
			{
				switch (this.eチャンネル番号)
				{
					case Ech定義.HiHatClose:
					case Ech定義.Snare:
					case Ech定義.BassDrum:
					case Ech定義.HighTom:
					case Ech定義.LowTom:
					case Ech定義.Cymbal:
					case Ech定義.FloorTom:
					case Ech定義.HiHatOpen:
					case Ech定義.RideCymbal:
					case Ech定義.LeftCymbal:
					case Ech定義.LeftPedal:
					case Ech定義.LeftBassDrum:

					case Ech定義.Guitar_Open:
					case Ech定義.Guitar_xxB:
					case Ech定義.Guitar_xGx:
					case Ech定義.Guitar_xGB:
					case Ech定義.Guitar_Rxx:
					case Ech定義.Guitar_RxB:
					case Ech定義.Guitar_RGx:
					case Ech定義.Guitar_RGB:

					case Ech定義.Bass_Open:
					case Ech定義.Bass_xxB:
					case Ech定義.Bass_xGx:
					case Ech定義.Bass_xGB:
					case Ech定義.Bass_Rxx:
					case Ech定義.Bass_RxB:
					case Ech定義.Bass_RGx:
					case Ech定義.Bass_RGB:
						return true;

					default:
						return false;
				}
			}
		}


		public CChip()
		{
			this.nバーからの距離dot = new STDGBVALUE<int>()
			{
				Drums = 0,
				Guitar = 0,
				Bass = 0,
			};
			b可視 = true;
			dbチップサイズ倍率 = 1.0;
			e楽器パート = E楽器パート.UNKNOWN;
			n透明度 = 0xff;
		}

		public override string ToString()
		{
			/*
			string[] chToStr = 
			{
				"??", "バックコーラス", "小節長変更", "BPM変更", "BMPレイヤ1", "??", "??", "BMPレイヤ2",
				"BPM変更(拡張)", "??", "??", "??", "??", "??", "??", "??",
				"??", "HHClose", "Snare", "Kick", "HiTom", "LowTom", "Cymbal", "FloorTom",
				"HHOpen", "RideCymbal", "LeftCymbal", "LeftPedal", "LeftBassDrum", "", "", "ドラム歓声切替",
				"ギターOPEN", "ギター - - B", "ギター - G -", "ギター - G B", "ギター R - -", "ギター R - B", "ギター R G -", "ギター R G B",
				"ギターWailing", "??", "??", "??", "??", "??", "??", "ギターWailing音切替",
				"??", "HHClose(不可視)", "Snare(不可視)", "Kick(不可視)", "HiTom(不可視)", "LowTom(不可視)", "Cymbal(不可視)", "FloorTom(不可視)",
				"HHOpen(不可視)", "RideCymbal(不可視)", "LeftCymbal(不可視)", "??", "??", "??", "??", "??",
				"??", "??", "??", "??", "??", "??", "??", "??", 
				"??", "??", "??", "??", "??", "??", "??", "??", 
				"小節線", "拍線", "MIDIコーラス", "フィルイン", "AVI", "BMPレイヤ3", "BMPレイヤ4", "BMPレイヤ5",
				"BMPレイヤ6", "BMPレイヤ7", "AVIFull", "??", "??", "??", "??", "??", 
				"BMPレイヤ8", "SE01", "SE02", "SE03", "SE04", "SE05", "SE06", "SE07",
				"SE08", "SE09", "??", "??", "??", "??", "??", "??", 
				"SE10", "SE11", "SE12", "SE13", "SE14", "SE15", "SE16", "SE17",
				"SE18", "SE19", "??", "??", "??", "??", "??", "??", 
				"SE20", "SE21", "SE22", "SE23", "SE24", "SE25", "SE26", "SE27",
				"SE28", "SE29", "??", "??", "??", "??", "??", "??", 
				"SE30", "SE31", "SE32", "??", "??", "??", "??", "??", 
				"??", "??", "??", "??", "??", "??", "??", "??", 
				"ベースOPEN", "ベース - - B", "ベース - G -", "ベース - G B", "ベース R - -", "ベース R - B", "ベース R G -", "ベース R G B",
				"ベースWailing", "??", "??", "??", "??", "??", "??", "ベースWailing音切替",
				"??", "HHClose(空うち)", "Snare(空うち)", "Kick(空うち)", "HiTom(空うち)", "LowTom(空うち)", "Cymbal(空うち)", "FloorTom(空うち)",
				"HHOpen(空うち)", "RideCymbal(空うち)", "ギター(空打ち)", "ベース(空打ち)", "LeftCymbal(空うち)", "LeftPedal(空打ち)", "LeftBassDrum(空打ち)", "??", 
				"??", "??", "??", "??", "BGAスコープ画像切替1", "??", "??", "BGAスコープ画像切替2",
				"??", "??", "??", "??", "??", "??", "??", "??", 
				"??", "??", "??", "??", "??", "BGAスコープ画像切替3", "BGAスコープ画像切替4", "BGAスコープ画像切替5",
				"BGAスコープ画像切替6", "BGAスコープ画像切替7", "ミキサー登録", "ミキサー削除", "??", "??", "??", "??", 
				"BGAスコープ画像切替8"
			};
			*/
			return string.Format("CChip: 位置:{0:D4}.{1:D3}, 時刻{2:D6}, Ch:{3:X2}({4}), Pn:{5}({11})(内部{6}), Pd:{7}, Sz:{8}, UseWav:{9}",
			  this.n発声位置 / 384,
			  this.n発声位置 % 384,
			  this.n発声時刻ms,
			  this.eチャンネル番号,
			  this.eチャンネル番号,
			  this.n整数値, this.n整数値_内部番号,
			  this.db実数値,
			  this.dbチップサイズ倍率,
			  this.bWAVを使うチャンネルである,
				// this.b自動再生音チャンネルである,
			  CDTX.tZZ(this.n整数値));
		}
		/// <summary>
		/// チップの再生長を取得する。現状、WAVチップとBGAチップでのみ使用可能。
		/// </summary>
		/// <returns>再生長(ms)</returns>
		public int GetDuration()
		{
			int nDuration = 0;

			if (this.bWAVを使うチャンネルである)		// WAV
			{
				CDTX.CWAV wc;
				CDTXMania.DTX.listWAV.TryGetValue(this.n整数値_内部番号, out wc);
				if (wc == null)
				{
					nDuration = 0;
				}
				else
				{
					nDuration = (wc.rSound[0] == null) ? 0 : wc.rSound[0].n総演奏時間ms;
				}
			}
			else if (this.eチャンネル番号 == Ech定義.Movie || this.eチャンネル番号 == Ech定義.MovieFull)	// AVI
			{
				if (this.rAVI != null && this.rAVI.avi != null)
				{
					// int dwRate = (int) this.rAVI.avi.dwレート;
					// int dwScale = (int) this.rAVI.avi.dwスケール;
					// (int) ( 1000.0f * dwScale / dwRate * this.rAVI.avi.GetMaxFrameCount() );
					nDuration = this.rAVI.avi.GetDuration();
				}
			}

			double _db再生速度 = (CDTXMania.DTXVmode.Enabled) ? CDTXMania.DTX.dbDTXVPlaySpeed : CDTXMania.DTX.db再生速度;
			return (int)(nDuration / _db再生速度);
		}

		#region [ IComparable 実装 ]
		//-----------------
		public int CompareTo(CChip other)
		{
			byte[] n優先度 = new byte[] {
					5, 5, 3, 3, 5, 5, 5, 5, 3, 5, 5, 5, 5, 5, 5, 5, 
					5, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 
					7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
					5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
				};


			// まずは位置で比較。

			if (this.n発声位置 < other.n発声位置)
				return -1;

			if (this.n発声位置 > other.n発声位置)
				return 1;


			// 位置が同じなら優先度で比較。
			int thisIndex = (int)eチャンネル番号;
			int tIndex = (int)other.eチャンネル番号;

			if (n優先度[thisIndex] < n優先度[tIndex])
				return -1;

			if (n優先度[thisIndex] > n優先度[tIndex])
				return 1;


			// 位置も優先度も同じなら同じと返す。

			return 0;
		}
		//-----------------
		#endregion


		/// <summary>
		/// shallow copyです。
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return MemberwiseClone();
		}
	}

}
