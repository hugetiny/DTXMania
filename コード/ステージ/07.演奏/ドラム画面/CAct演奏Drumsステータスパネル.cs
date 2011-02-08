using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsステータスパネル : CActivity
	{
		// コンストラクタ

		public CAct演奏Drumsステータスパネル()
		{
			STATUSPANEL[] statuspanelArray = new STATUSPANEL[ 12 ];
			STATUSPANEL statuspanel = new STATUSPANEL();
			statuspanel.status = 0;
			statuspanel.label = "DTXMANIA";
			statuspanelArray[ 0 ] = statuspanel;
			STATUSPANEL statuspanel2 = new STATUSPANEL();
			statuspanel2.status = 1;
			statuspanel2.label = "EXTREME";
			statuspanelArray[ 1 ] = statuspanel2;
			STATUSPANEL statuspanel3 = new STATUSPANEL();
			statuspanel3.status = 2;
			statuspanel3.label = "ADVANCED";
			statuspanelArray[ 2 ] = statuspanel3;
			STATUSPANEL statuspanel4 = new STATUSPANEL();
			statuspanel4.status = 2;
			statuspanel4.label = "ADVANCE";
			statuspanelArray[ 3 ] = statuspanel4;
			STATUSPANEL statuspanel5 = new STATUSPANEL();
			statuspanel5.status = 3;
			statuspanel5.label = "BASIC";
			statuspanelArray[ 4 ] = statuspanel5;
			STATUSPANEL statuspanel6 = new STATUSPANEL();
			statuspanel6.status = 4;
			statuspanel6.label = "RAW";
			statuspanelArray[ 5 ] = statuspanel6;
			STATUSPANEL statuspanel7 = new STATUSPANEL();
			statuspanel7.status = 5;
			statuspanel7.label = "REAL";
			statuspanelArray[ 6 ] = statuspanel7;
			STATUSPANEL statuspanel8 = new STATUSPANEL();
			statuspanel8.status = 6;
			statuspanel8.label = "EASY";
			statuspanelArray[ 7 ] = statuspanel8;
			STATUSPANEL statuspanel9 = new STATUSPANEL();
			statuspanel9.status = 7;
			statuspanel9.label = "EX-REAL";
			statuspanelArray[ 8 ] = statuspanel9;
			STATUSPANEL statuspanel10 = new STATUSPANEL();
			statuspanel10.status = 7;
			statuspanel10.label = "ExREAL";
			statuspanelArray[ 9 ] = statuspanel10;
			STATUSPANEL statuspanel11 = new STATUSPANEL();
			statuspanel11.status = 7;
			statuspanel11.label = "ExpertReal";
			statuspanelArray[ 10 ] = statuspanel11;
			STATUSPANEL statuspanel12 = new STATUSPANEL();
			statuspanel12.status = 8;
			statuspanel12.label = "NORMAL";
			statuspanelArray[ 11 ] = statuspanel12;
			this.stパネルマップ = statuspanelArray;
			base.b活性化してない = true;
		}


		// メソッド

		public void tラベル名からステータスパネルを決定する( string strラベル名 )
		{
			if( string.IsNullOrEmpty( strラベル名 ) )
			{
				this.nStatus = 0;
			}
			else
			{
				foreach( STATUSPANEL statuspanel in this.stパネルマップ )
				{
					if( strラベル名.Equals( statuspanel.label ) )
					{
						this.nStatus = statuspanel.status;
						return;
					}
				}
				this.nStatus = 0;
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.nStatus = 0;
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txStatusPanels = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels right.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txStatusPanels );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない && ( this.txStatusPanels != null ) )
			{
				this.txStatusPanels.t2D描画( CDTXMania.app.Device, 0x26f, 0x14f, new Rectangle( this.nStatus * 15, 0xb7, 15, 0x49 ) );
				int drums = CDTXMania.ConfigIni.n譜面スクロール速度.Drums;
				if( drums < 0 )
				{
					drums = 0;
				}
				if( drums > 15 )
				{
					drums = 15;
				}
				this.txStatusPanels.t2D描画( CDTXMania.app.Device, 0x26f, 0x3b, new Rectangle( drums * 15, 0, 15, 0xac ) );
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STATUSPANEL
		{
			public string label;
			public int status;
		}

		private int nStatus;
		private STATUSPANEL[] stパネルマップ;
		private CTexture txStatusPanels;
		//-----------------
		#endregion
	}
}
