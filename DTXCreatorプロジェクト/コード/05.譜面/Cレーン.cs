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
		
		public bool b右側の線が太線;
		public Color col背景色 = Color.Black;
		public E種別 eレーン種別 = E種別.WAV;
		public int nチャンネル番号・表00toFF;
		public int nチャンネル番号・裏00toFF;
		public int nレーン割付チップ・表0or1to1295;
		public int nレーン割付チップ・裏0or1to1295;
		public int n位置Xdot;
		public int n幅dot = 30;
		public string strレーン名 = "";

		public bool bパターンレーンである()
		{
			if( ( ( ( this.eレーン種別 != E種別.GtR ) && ( this.eレーン種別 != E種別.GtG ) ) && ( ( this.eレーン種別 != E種別.GtB ) && ( this.eレーン種別 != E種別.BsR ) ) ) && ( ( this.eレーン種別 != E種別.BsG ) && ( this.eレーン種別 != E種別.BsB ) ) )
			{
				return false;
			}
			return true;
		}
	}
}
