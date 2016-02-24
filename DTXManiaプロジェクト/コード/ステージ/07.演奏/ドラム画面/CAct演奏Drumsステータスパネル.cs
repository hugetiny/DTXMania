using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsステータスパネル : CAct演奏ステータスパネル共通
	{
		// コンストラクタ

//		public CAct演奏Drumsステータスパネル()
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
				this.txStatusPanels = TextureFactory.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels right.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				TextureFactory.tテクスチャの解放( ref this.txStatusPanels );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if ( !base.b活性化してない && ( this.txStatusPanels != null ) )
			{
				this.txStatusPanels.t2D描画(
					CDTXMania.Instance.Device,
					0x26f * Scale.X,
					0x14f * Scale.Y,
					new Rectangle(
						(int) ( this.nStatus * 15 * Scale.X ),
						(int) ( 0xb7 * Scale.Y ),
						(int) ( 15 * Scale.X ),
						(int) ( 0x49 * Scale.Y )
					)
				);
				int drums = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Drums;
				if ( drums < 0 )
				{
					drums = 0;
				}
				if ( drums > 15 )
				{
					drums = 15;
				}
				this.txStatusPanels.t2D描画( CDTXMania.Instance.Device,
					0x26f * Scale.X,
					0x3b * Scale.Y,
					new Rectangle(
						(int) ( drums * 15 * Scale.X ),
						0,
						(int) ( 15 * Scale.X ),
						(int) ( 0xac * Scale.Y )
					)
				);
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
		private CTexture txStatusPanels;
		//-----------------
		#endregion
	}
}
