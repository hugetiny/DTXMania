using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Guitarステータスパネル : CActivity
	{
		// コンストラクタ

		public CAct演奏Guitarステータスパネル()
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
				this.tx左パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels left.png" ) );
				this.tx右パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels right.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx左パネル );
				CDTXMania.tテクスチャの解放( ref this.tx右パネル );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( this.tx左パネル != null )
				{
					this.tx左パネル.t2D描画( CDTXMania.app.Device, 3, 0x143, new Rectangle( this.nStatus * 15, 0xb7, 15, 0x49 ) );
					int guitar = CDTXMania.ConfigIni.n譜面スクロール速度.Guitar;
					if( guitar < 0 )
					{
						guitar = 0;
					}
					if( guitar > 15 )
					{
						guitar = 15;
					}
					this.tx左パネル.t2D描画( CDTXMania.app.Device, 3, 0x35, new Rectangle( guitar * 15, 0, 15, 0xac ) );
				}
				if( this.tx右パネル != null )
				{
					this.tx右パネル.t2D描画( CDTXMania.app.Device, 0x26e, 0x143, new Rectangle( this.nStatus * 15, 0xb7, 15, 0x49 ) );
					int bass = CDTXMania.ConfigIni.n譜面スクロール速度.Bass;
					if( bass < 0 )
					{
						bass = 0;
					}
					if( bass > 15 )
					{
						bass = 15;
					}
					this.tx右パネル.t2D描画( CDTXMania.app.Device, 0x26e, 0x35, new Rectangle( bass * 15, 0, 15, 0xac ) );
				}
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
		private CTexture tx右パネル;
		private CTexture tx左パネル;
		//-----------------
		#endregion
	}
}
