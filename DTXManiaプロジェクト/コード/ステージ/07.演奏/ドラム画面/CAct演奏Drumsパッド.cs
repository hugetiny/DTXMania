using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsパッド : CActivity
	{
		// コンストラクタ

		public CAct演奏Drumsパッド()
		{
			ST基本位置[] st基本位置Array = new ST基本位置[ 9 ];
			ST基本位置 st基本位置 = new ST基本位置();
			st基本位置.x = 0x16;
			st基本位置.y = 0;
			st基本位置.rc = new Rectangle( 0, 0, 0x40, 0x40 );
			st基本位置Array[ 0 ] = st基本位置;
			ST基本位置 st基本位置2 = new ST基本位置();
			st基本位置2.x = 0x3d;
			st基本位置2.y = 11;
			st基本位置2.rc = new Rectangle( 0x40, 0, 0x40, 0x40 );
			st基本位置Array[ 1 ] = st基本位置2;
			ST基本位置 st基本位置3 = new ST基本位置();
			st基本位置3.x = 0x60;
			st基本位置3.y = 8;
			st基本位置3.rc = new Rectangle( 0x80, 0, 0x40, 0x40 );
			st基本位置Array[ 2 ] = st基本位置3;
			ST基本位置 st基本位置4 = new ST基本位置();
			st基本位置4.x = 0x8a;
			st基本位置4.y = 7;
			st基本位置4.rc = new Rectangle( 0, 0x40, 0x40, 0x40 );
			st基本位置Array[ 3 ] = st基本位置4;
			ST基本位置 st基本位置5 = new ST基本位置();
			st基本位置5.x = 0xb3;
			st基本位置5.y = 0;
			st基本位置5.rc = new Rectangle( 0x40, 0x40, 0x40, 0x40 );
			st基本位置Array[ 4 ] = st基本位置5;
			ST基本位置 st基本位置6 = new ST基本位置();
			st基本位置6.x = 0xd4;
			st基本位置6.y = 5;
			st基本位置6.rc = new Rectangle( 0x80, 0x40, 0x40, 0x40 );
			st基本位置Array[ 5 ] = st基本位置6;
			ST基本位置 st基本位置7 = new ST基本位置();
			st基本位置7.x = 250;
			st基本位置7.y = 15;
			st基本位置7.rc = new Rectangle( 0, 0x80, 0x40, 0x40 );
			st基本位置Array[ 6 ] = st基本位置7;
			ST基本位置 st基本位置8 = new ST基本位置();
			st基本位置8.x = 0x11a;
			st基本位置8.y = 0;
			st基本位置8.rc = new Rectangle( 0x40, 0x80, 0x40, 0x40 );
			st基本位置Array[ 7 ] = st基本位置8;
			ST基本位置 st基本位置9 = new ST基本位置();
			st基本位置9.x = 0x130;
			st基本位置9.y = 8;
			st基本位置9.rc = new Rectangle( 0x80, 0x80, 0x40, 0x40 );
			st基本位置Array[ 8 ] = st基本位置9;
			this.st基本位置 = st基本位置Array;
			base.b活性化してない = true;
		}
		
		
		// メソッド

		public void Hit( int nLane )
		{
			this.stパッド状態[ nLane ].n明るさ = 6;
			this.stパッド状態[ nLane ].nY座標加速度dot = 2;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.nフラッシュ制御タイマ = -1;
			this.nY座標制御タイマ = -1;
			for( int i = 0; i < 9; i++ )
			{
				STパッド状態 stパッド状態2 = new STパッド状態();
				STパッド状態 stパッド状態 = stパッド状態2;
				stパッド状態.nY座標オフセットdot = 0;
				stパッド状態.nY座標加速度dot = 0;
				stパッド状態.n明るさ = 0;
				this.stパッド状態[ i ] = stパッド状態;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txパッド = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums pads.png" ) );
				this.tx光るパッド = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums pads flush.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txパッド );
				CDTXMania.tテクスチャの解放( ref this.tx光るパッド );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.nフラッシュ制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					this.nY座標制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
					base.b初めての進行描画 = false;
				}
				long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
				if( num < this.nフラッシュ制御タイマ )
				{
					this.nフラッシュ制御タイマ = num;
				}
				while( ( num - this.nフラッシュ制御タイマ ) >= 15 )
				{
					for( int j = 0; j < 9; j++ )
					{
						if( this.stパッド状態[ j ].n明るさ > 0 )
						{
							this.stパッド状態[ j ].n明るさ--;
						}
					}
					this.nフラッシュ制御タイマ += 15;
				}
				long num3 = CSound管理.rc演奏用タイマ.n現在時刻;
				if( num3 < this.nY座標制御タイマ )
				{
					this.nY座標制御タイマ = num3;
				}
				while( ( num3 - this.nY座標制御タイマ ) >= 5 )
				{
					for( int k = 0; k < 9; k++ )
					{
						this.stパッド状態[ k ].nY座標オフセットdot += this.stパッド状態[ k ].nY座標加速度dot;
						if( this.stパッド状態[ k ].nY座標オフセットdot > 15 )
						{
							this.stパッド状態[ k ].nY座標オフセットdot = 15;
							this.stパッド状態[ k ].nY座標加速度dot = -1;
						}
						else if( this.stパッド状態[ k ].nY座標オフセットdot < 0 )
						{
							this.stパッド状態[ k ].nY座標オフセットdot = 0;
							this.stパッド状態[ k ].nY座標加速度dot = 0;
						}
					}
					this.nY座標制御タイマ += 5;
				}
				for( int i = 0; i < 9; i++ )
				{
					int index = this.n描画順[ i ];
					int x = this.st基本位置[ index ].x;
					int y = ( this.st基本位置[ index ].y + ( CDTXMania.ConfigIni.bReverse.Drums ? -10 : 0x19e ) ) + this.stパッド状態[ index ].nY座標オフセットdot;
					if( this.txパッド != null )
					{
						this.txパッド.t2D描画( CDTXMania.app.Device, x, y, this.st基本位置[ index ].rc );
					}
					if( this.tx光るパッド != null )
					{
						this.tx光るパッド.n透明度 = ( this.stパッド状態[ index ].n明るさ * 40 ) + 15;
						this.tx光るパッド.t2D描画( CDTXMania.app.Device, x, y, this.st基本位置[ index ].rc );
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STパッド状態
		{
			public int n明るさ;
			public int nY座標オフセットdot;
			public int nY座標加速度dot;
		}
		[StructLayout( LayoutKind.Sequential )]
		private struct ST基本位置
		{
			public int x;
			public int y;
			public Rectangle rc;
		}

		private long nY座標制御タイマ;
		private long nフラッシュ制御タイマ;
		private readonly int[] n描画順 = new int[] { 3, 6, 5, 4, 2, 1, 8, 7, 0 };
		private STパッド状態[] stパッド状態 = new STパッド状態[ 9 ];
		private readonly ST基本位置[] st基本位置;
		private CTexture txパッド;
		private CTexture tx光るパッド;
		//-----------------
		#endregion
	}
}
