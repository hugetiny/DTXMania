using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Guitarステータスパネル : CAct演奏ステータスパネル共通
	{
		// コンストラクタ

//		public CAct演奏Guitarステータスパネル()
//		{
//		}


		// メソッド

		//public void tラベル名からステータスパネルを決定する( string strラベル名 )
		//{
		//    if( string.IsNullOrEmpty( strラベル名 ) )
		//    {
		//        this.nStatus = 0;
		//    }
		//    else
		//    {
		//        foreach( STATUSPANEL statuspanel in this.stパネルマップ )
		//        {
		//            if( strラベル名.Equals( statuspanel.label ) )
		//            {
		//                this.nStatus = statuspanel.status;
		//                return;
		//            }
		//        }
		//        this.nStatus = 0;
		//    }
		//}


		// CActivity 実装

		//public override void On活性化()
		//{
		//    this.nStatus = 0;
		//    base.On活性化();
		//}
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
		//[StructLayout( LayoutKind.Sequential )]
		//private struct STATUSPANEL
		//{
		//    public string label;
		//    public int status;
		//}

		//private int nStatus;
		//private STATUSPANEL[] stパネルマップ;
		private CTexture tx右パネル;
		private CTexture tx左パネル;
		//-----------------
		#endregion
	}
}
