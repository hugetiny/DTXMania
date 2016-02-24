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
				this.tx左パネル = TextureFactory.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels left.png" ) );
				this.tx右パネル = TextureFactory.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay status panels right.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				TextureFactory.tテクスチャの解放( ref this.tx左パネル );
				TextureFactory.tテクスチャの解放( ref this.tx右パネル );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( this.tx左パネル != null )
				{
					this.tx左パネル.t2D描画(
						CDTXMania.Instance.Device,
						3 * Scale.X,
						0x143 * Scale.Y,
						new Rectangle(
							(int) ( this.nStatus * 15 * Scale.X ),
							(int) ( 0xb7 * Scale.Y ),
							(int) ( 15 * Scale.X ),
							(int) ( 0x49 * Scale.Y )
						)
					);
					int guitar = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Guitar;
					if( guitar < 0 )
					{
						guitar = 0;
					}
					if( guitar > 15 )
					{
						guitar = 15;
					}
					this.tx左パネル.t2D描画(
						CDTXMania.Instance.Device,
						3 * Scale.X,
						0x35 * Scale.Y,
						new Rectangle(
							(int) ( guitar * 15 * Scale.X ),
							0,
							(int) ( 15 * Scale.X ),
							(int) ( 0xac * Scale.Y )
						)
					);
				}
				if( this.tx右パネル != null )
				{
					this.tx右パネル.t2D描画(
						CDTXMania.Instance.Device,
						0x26e * Scale.X,
						0x143 * Scale.Y,
						new Rectangle(
							(int) ( this.nStatus * 15 * Scale.X ),
							(int) ( 0xb7 * Scale.Y ),
							(int) ( 15 * Scale.X ),
							(int) ( 0x49 * Scale.Y )
						)
					);
					int bass = CDTXMania.Instance.ConfigIni.n譜面スクロール速度.Bass;
					if( bass < 0 )
					{
						bass = 0;
					}
					if( bass > 15 )
					{
						bass = 15;
					}
					this.tx右パネル.t2D描画(
						CDTXMania.Instance.Device,
						0x26e * Scale.X,
						0x35 * Scale.Y,
						new Rectangle(
							(int) ( bass * 15 * Scale.X ),
							0,
							(int) ( 15 * Scale.X ),
							(int) ( 0xac * Scale.Y )
						)
					);
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
