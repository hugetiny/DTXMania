using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DTXCreator.譜面
{
	public class Cレーン
	{
		public enum E種別
		{
			BPM,
			WAV,
			BMP,
			AVI,
			FI,
			GtV,
			GtR,
			GtG,
			GtB,
			GtW,
			BsV,
			BsR,
			BsG,
			BsB,
			BsW
		}
		
		public bool b左側の線が太線;
		public Color col背景色 = Color.Black;
		public E種別 eレーン種別 = E種別.WAV;
		public int nチャンネル番号・表00toFF;
		public int nチャンネル番号・裏00toFF;
		public int nレーン割付チップ・表0or1to1295;
		public int nレーン割付チップ・裏0or1to1295;
		public int n位置Xdot;
		public int n幅dot = 30;
		public string strレーン名 = "";
		public int nGroupNo;
		public bool bIsVisible;


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Cレーン()
		{
			this.eレーン種別 = E種別.WAV;
			this.strレーン名 = "";
			this.nチャンネル番号・表00toFF = 0;
			this.nチャンネル番号・裏00toFF = 0;
			this.b左側の線が太線 = false;
			this.col背景色 = Color.FromArgb(0, 0, 0, 0);
			this.n位置Xdot = 0;
			this.n幅dot = 30;
			this.nGroupNo = 0;
			this.bIsVisible = true;
		}

		/// <summary>
		/// コンストラクタ(初期化用)
		/// </summary>
		/// <param name="eレーン種別_"></param>
		/// <param name="strレーン名_"></param>
		/// <param name="nチャンネル番号・表00toFF_"></param>
		/// <param name="nチャンネル番号・裏00toFF_"></param>
		/// <param name="b左側の線が太線_"></param>
		/// <param name="col背景色_"></param>
		/// <param name="n位置Xdot_"></param>
		/// <param name="n幅dot_"></param>
		/// <param name="nGroupNo_"></param>
		/// <param name="bIsVisible_"></param>
		public Cレーン(
			E種別 eレーン種別_, string strレーン名_,
			int nチャンネル番号・表00toFF_, int nチャンネル番号・裏00toFF_,
//			int nレーン割付チップ・表0or1to1295_, int nレーン割付チップ・裏0or1to1295_,
			bool b左側の線が太線_,
			Color col背景色_,
			int n位置Xdot_, int n幅dot_,
			int nGroupNo_,
			bool bIsVisible_ )
		{
			this.eレーン種別 = eレーン種別_;
			this.strレーン名 = strレーン名_;
			this.nチャンネル番号・表00toFF = nチャンネル番号・表00toFF_;
			this.nチャンネル番号・裏00toFF = nチャンネル番号・裏00toFF_;
//			this.nレーン割付チップ・表0or1to1295 = nレーン割付チップ・表0or1to1295_;
//			this.nレーン割付チップ・裏0or1to1295 = nレーン割付チップ・裏0or1to1295_;
			this.b左側の線が太線 = b左側の線が太線_;
			this.col背景色 = col背景色_;
			this.n位置Xdot = n位置Xdot_;
			this.n幅dot = n幅dot_;
			this.nGroupNo = nGroupNo_;
			this.bIsVisible = bIsVisible_;
		}

		public bool bパターンレーンである()
		{
			if( ( this.eレーン種別 != E種別.GtR ) && ( this.eレーン種別 != E種別.GtG ) && ( this.eレーン種別 != E種別.GtB ) && ( this.eレーン種別 != E種別.BsR ) && ( this.eレーン種別 != E種別.BsG ) && ( this.eレーン種別 != E種別.BsB ) )
			{
				return false;
			}
			return true;
		}
	}
}
