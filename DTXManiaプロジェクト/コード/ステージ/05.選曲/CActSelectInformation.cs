using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActSelectInformation : CActivity
	{
		// コンストラクタ

		public CActSelectInformation()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n画像Index上 = -1;
			this.n画像Index下 = 0;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ctスクロール用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txInfo[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect information 1.png" ), false );
				this.txInfo[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect information 2.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txInfo[ 0 ] );
				CDTXMania.tテクスチャの解放( ref this.txInfo[ 1 ] );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.ctスクロール用 = new CCounter( 0, 0x1770, 1, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				this.ctスクロール用.t進行();
				if( this.ctスクロール用.b終了値に達した )
				{
					this.n画像Index上 = this.n画像Index下;
					this.n画像Index下 = ( this.n画像Index下 + 1 ) % 8;
					this.ctスクロール用.n現在の値 = 0;
				}
				int num = this.ctスクロール用.n現在の値;
				if( num <= 250 )
				{
					double num2 = ( (double) num ) / 250.0;
					if( this.n画像Index上 >= 0 )
					{
						STINFO stinfo = this.stInfo[ this.n画像Index上 ];
						Rectangle rectangle = new Rectangle( stinfo.pt左上座標.X, stinfo.pt左上座標.Y + ( (int) ( 45.0 * num2 ) ), 0xdd, 0x2d );
						if( this.txInfo[ stinfo.nTexture番号 ] != null )
						{
							this.txInfo[ stinfo.nTexture番号 ].t2D描画( CDTXMania.app.Device, 0x73, 6, rectangle );
						}
					}
					if( this.n画像Index下 >= 0 )
					{
						STINFO stinfo2 = this.stInfo[ this.n画像Index下 ];
						Rectangle rectangle2 = new Rectangle( stinfo2.pt左上座標.X, stinfo2.pt左上座標.Y, 0xdd, (int) ( 45.0 * num2 ) );
						if( this.txInfo[ stinfo2.nTexture番号 ] != null )
						{
							this.txInfo[ stinfo2.nTexture番号 ].t2D描画( CDTXMania.app.Device, 0x73, 6 + ( (int) ( 45.0 * ( 1.0 - num2 ) ) ), rectangle2 );
						}
					}
				}
				else
				{
					STINFO stinfo3 = this.stInfo[ this.n画像Index下 ];
					Rectangle rectangle3 = new Rectangle( stinfo3.pt左上座標.X, stinfo3.pt左上座標.Y, 0xdd, 0x2d );
					if( this.txInfo[ stinfo3.nTexture番号 ] != null )
					{
						this.txInfo[ stinfo3.nTexture番号 ].t2D描画( CDTXMania.app.Device, 0x73, 6, rectangle3 );
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STINFO
		{
			public int nTexture番号;
			public Point pt左上座標;
			public STINFO( int nTexture番号, int x, int y )
			{
				this.nTexture番号 = nTexture番号;
				this.pt左上座標 = new Point( x, y );
			}
		}

		private CCounter ctスクロール用;
		private const int nINFO数 = 8;
		private int n画像Index下;
		private int n画像Index上;
		private readonly STINFO[] stInfo = new STINFO[] { new STINFO( 0, 0, 0 ), new STINFO( 0, 0, 0x31 ), new STINFO( 0, 0, 0x62 ), new STINFO( 0, 0, 0x93 ), new STINFO( 0, 0, 0xc4 ), new STINFO( 1, 0, 0 ), new STINFO( 1, 0, 0x31 ), new STINFO( 1, 0, 0x62 ) };
		private CTexture[] txInfo = new CTexture[ 2 ];
		//-----------------
		#endregion
	}
}
