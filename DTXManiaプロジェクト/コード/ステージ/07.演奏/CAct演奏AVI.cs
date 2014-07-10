using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CAct演奏AVI : CActivity
	{
		// コンストラクタ

		public CAct演奏AVI()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void Start( int nチャンネル番号, CDTX.CAVI rAVI, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, int n総移動時間ms, int n移動開始時刻ms )
		{
			if ( nチャンネル番号 == 0x54 )
			{
				this.rAVI = rAVI;
				this.n開始サイズW = n開始サイズW;
				this.n開始サイズH = n開始サイズH;
				this.n終了サイズW = n終了サイズW;
				this.n終了サイズH = n終了サイズH;
				this.n画像側開始位置X = n画像側開始位置X;
				this.n画像側開始位置Y = n画像側開始位置Y;
				this.n画像側終了位置X = n画像側終了位置X;
				this.n画像側終了位置Y = n画像側終了位置Y;
				this.n表示側開始位置X = n表示側開始位置X;
				this.n表示側開始位置Y = n表示側開始位置Y;
				this.n表示側終了位置X = n表示側終了位置X;
				this.n表示側終了位置Y = n表示側終了位置Y;
				this.n総移動時間ms = n総移動時間ms;
				this.n移動開始時刻ms = ( n移動開始時刻ms != -1 ) ? n移動開始時刻ms : CSound管理.rc演奏用タイマ.n現在時刻;
				this.n前回表示したフレーム番号 = -1;
			}
		}
		public void SkipStart( int n移動開始時刻ms )
		{
			foreach ( CDTX.CChip chip in CDTXMania.DTX.listChip )
			{
				if ( chip.n発声時刻ms > n移動開始時刻ms )
				{
					break;
				}
				switch ( chip.eAVI種別 )
				{
					case EAVI種別.AVI:
						{
							if ( chip.rAVI != null )
							{
								this.Start( chip.nチャンネル番号, chip.rAVI, 0x116, 0x163, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, chip.n発声時刻ms );
							}
							continue;
						}
					case EAVI種別.AVIPAN:
						{
							if ( chip.rAVIPan != null )
							{
								this.Start( chip.nチャンネル番号, chip.rAVI, chip.rAVIPan.sz開始サイズ.Width, chip.rAVIPan.sz開始サイズ.Height, chip.rAVIPan.sz終了サイズ.Width, chip.rAVIPan.sz終了サイズ.Height, chip.rAVIPan.pt動画側開始位置.X, chip.rAVIPan.pt動画側開始位置.Y, chip.rAVIPan.pt動画側終了位置.X, chip.rAVIPan.pt動画側終了位置.Y, chip.rAVIPan.pt表示側開始位置.X, chip.rAVIPan.pt表示側開始位置.Y, chip.rAVIPan.pt表示側終了位置.X, chip.rAVIPan.pt表示側終了位置.Y, chip.n総移動時間, chip.n発声時刻ms );
							}
							continue;
						}
				}
			}
		}
		public void Stop()
		{
			if ( ( this.rAVI != null ) && ( this.rAVI.avi != null ) )
			{
				this.n移動開始時刻ms = -1;
			}
		}
		public void Cont( int n再開時刻ms )
		{
			if ( ( this.rAVI != null ) && ( this.rAVI.avi != null ) )
			{
				this.n移動開始時刻ms = n再開時刻ms;
			}
		}
		public unsafe int t進行描画( int x, int y )
		{
			if ( !base.b活性化してない )
			{
				Rectangle rectangle;
				Rectangle rectangle2;
				if ( ( ( this.n移動開始時刻ms == -1 ) || ( this.rAVI == null ) ) || ( this.rAVI.avi == null ) )
				{
					return 0;
				}
				if ( this.tx描画用 == null )
				{
					return 0;
				}
				int time = (int) ( ( CSound管理.rc演奏用タイマ.n現在時刻 - this.n移動開始時刻ms ) * ( ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0 ) );
				int frameNoFromTime = this.rAVI.avi.GetFrameNoFromTime( time );
				if ( ( this.n総移動時間ms != 0 ) && ( this.n総移動時間ms < time ) )
				{
					this.n総移動時間ms = 0;
					this.n移動開始時刻ms = -1;
					return 0;
				}
				if ( ( this.n総移動時間ms == 0 ) && ( frameNoFromTime >= this.rAVI.avi.GetMaxFrameCount() ) )
				{
					this.n移動開始時刻ms = -1;
					return 0;
				}
				if ( ( this.n前回表示したフレーム番号 != frameNoFromTime ) && !this.bフレームを作成した )
				{
					this.pBmp = this.rAVI.avi.GetFramePtr( frameNoFromTime );
					this.n前回表示したフレーム番号 = frameNoFromTime;
					this.bフレームを作成した = true;
				}
				Size size = new Size( (int) this.rAVI.avi.nフレーム幅, (int) this.rAVI.avi.nフレーム高さ );
				Size size2 = new Size( 0x116, 0x163 );
				Size size3 = new Size( this.n開始サイズW, this.n開始サイズH );
				Size size4 = new Size( this.n終了サイズW, this.n終了サイズH );
				Point location = new Point( this.n画像側開始位置X, this.n画像側終了位置Y );
				Point point2 = new Point( this.n画像側終了位置X, this.n画像側終了位置Y );
				Point point3 = new Point( this.n表示側開始位置X, this.n表示側開始位置Y );
				Point point4 = new Point( this.n表示側終了位置X, this.n表示側終了位置Y );
				long num3 = this.n総移動時間ms;
				long num4 = this.n移動開始時刻ms;
				if ( CSound管理.rc演奏用タイマ.n現在時刻 < num4 )
				{
					num4 = CSound管理.rc演奏用タイマ.n現在時刻;
				}
				time = (int) ( ( CSound管理.rc演奏用タイマ.n現在時刻 - num4 ) * ( ( (double) CDTXMania.ConfigIni.n演奏速度 ) / 20.0 ) );
				if ( num3 == 0 )
				{
					rectangle = new Rectangle( location, size3 );
					rectangle2 = new Rectangle( point3, size3 );
				}
				else
				{
					double num5 = ( (double) time ) / ( (double) num3 );
					Size size5 = new Size( size3.Width + ( (int) ( ( size4.Width - size3.Width ) * num5 ) ), size3.Height + ( (int) ( ( size4.Height - size3.Height ) * num5 ) ) );
					rectangle = new Rectangle( (int) ( ( point2.X - location.X ) * num5 ), (int) ( ( point2.Y - location.Y ) * num5 ), ( (int) ( ( point2.X - location.X ) * num5 ) ) + size5.Width, ( (int) ( ( point2.Y - location.Y ) * num5 ) ) + size5.Height );
					rectangle2 = new Rectangle( (int) ( ( point4.X - point3.X ) * num5 ), (int) ( ( point4.Y - point3.Y ) * num5 ), ( (int) ( ( point4.X - point3.X ) * num5 ) ) + size5.Width, ( (int) ( ( point4.Y - point3.Y ) * num5 ) ) + size5.Height );
					if ( ( ( rectangle.Right <= 0 ) || ( rectangle.Bottom <= 0 ) ) || ( ( rectangle.Left >= size.Width ) || ( rectangle.Top >= size.Height ) ) )
					{
						return 0;
					}
					if ( ( ( rectangle2.Right <= 0 ) || ( rectangle2.Bottom <= 0 ) ) || ( ( rectangle2.Left >= size2.Width ) || ( rectangle2.Top >= size2.Height ) ) )
					{
						return 0;
					}
					if ( rectangle.X < 0 )
					{
						int num6 = -rectangle.X;
						rectangle2.X += num6;
						rectangle2.Width -= num6;
						rectangle.X = 0;
						rectangle.Width -= num6;
					}
					if ( rectangle.Y < 0 )
					{
						int num7 = -rectangle.Y;
						rectangle2.Y += num7;
						rectangle2.Height -= num7;
						rectangle.Y = 0;
						rectangle.Height -= num7;
					}
					if ( rectangle.Right > size.Width )
					{
						int num8 = rectangle.Right - size.Width;
						rectangle2.Width -= num8;
						rectangle.Width -= num8;
					}
					if ( rectangle.Bottom > size.Height )
					{
						int num9 = rectangle.Bottom - size.Height;
						rectangle2.Height -= num9;
						rectangle.Height -= num9;
					}
					if ( rectangle2.X < 0 )
					{
						int num10 = -rectangle2.X;
						rectangle.X += num10;
						rectangle.Width -= num10;
						rectangle2.X = 0;
						rectangle2.Width -= num10;
					}
					if ( rectangle2.Y < 0 )
					{
						int num11 = -rectangle2.Y;
						rectangle.Y += num11;
						rectangle.Height -= num11;
						rectangle2.Y = 0;
						rectangle2.Height -= num11;
					}
					if ( rectangle2.Right > size2.Width )
					{
						int num12 = rectangle2.Right - size2.Width;
						rectangle.Width -= num12;
						rectangle2.Width -= num12;
					}
					if ( rectangle2.Bottom > size2.Height )
					{
						int num13 = rectangle2.Bottom - size2.Height;
						rectangle.Height -= num13;
						rectangle2.Height -= num13;
					}
					if ( ( rectangle.X >= rectangle.Right ) || ( rectangle.Y >= rectangle.Bottom ) )
					{
						return 0;
					}
					if ( ( rectangle2.X >= rectangle2.Right ) || ( rectangle2.Y >= rectangle2.Bottom ) )
					{
						return 0;
					}
					if ( ( ( rectangle.Right < 0 ) || ( rectangle.Bottom < 0 ) ) || ( ( rectangle.X > size.Width ) || ( rectangle.Y > size.Height ) ) )
					{
						return 0;
					}
					if ( ( ( rectangle2.Right < 0 ) || ( rectangle2.Bottom < 0 ) ) || ( ( rectangle2.X > size2.Width ) || ( rectangle2.Y > size2.Height ) ) )
					{
						return 0;
					}
				}
				if ( ( this.tx描画用 != null ) && ( this.n総移動時間ms != -1 ) )
				{
					if ( this.bフレームを作成した && ( this.pBmp != IntPtr.Zero ) )
					{
						DataRectangle rectangle3 = this.tx描画用.texture.LockRectangle( 0, LockFlags.None );
						DataStream data = rectangle3.Data;
						int num14 = rectangle3.Pitch / this.tx描画用.szテクスチャサイズ.Width;
						BitmapUtil.BITMAPINFOHEADER* pBITMAPINFOHEADER = (BitmapUtil.BITMAPINFOHEADER*) this.pBmp.ToPointer();
						if ( pBITMAPINFOHEADER->biBitCount == 0x18 )
						{
							switch ( num14 )
							{
								case 2:
									this.rAVI.avi.tBitmap24ToGraphicsStreamR5G6B5( pBITMAPINFOHEADER, data, this.tx描画用.szテクスチャサイズ.Width, this.tx描画用.szテクスチャサイズ.Height );
									break;

								case 4:
									this.rAVI.avi.tBitmap24ToGraphicsStreamX8R8G8B8( pBITMAPINFOHEADER, data, this.tx描画用.szテクスチャサイズ.Width, this.tx描画用.szテクスチャサイズ.Height );
									break;
							}
						}
						this.tx描画用.texture.UnlockRectangle( 0 );
						this.bフレームを作成した = false;
					}
					this.tx描画用.t2D描画( CDTXMania.app.Device, x, y );
				}
			}
			return 0;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.rAVI = null;
			this.n移動開始時刻ms = -1;
			this.n前回表示したフレーム番号 = -1;
			this.bフレームを作成した = false;
			this.pBmp = IntPtr.Zero;
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if ( !base.b活性化してない )
			{
#if TEST_Direct3D9Ex
				this.tx描画用 = new CTexture( CDTXMania.app.Device, 320, 355, CDTXMania.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Default, Usage.Dynamic );
#else
				this.tx描画用 = new CTexture( CDTXMania.app.Device, 278, 355, CDTXMania.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.Managed );
#endif
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if ( !base.b活性化してない )
			{
				if ( this.tx描画用 != null )
				{
					this.tx描画用.Dispose();
					this.tx描画用 = null;
				}
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(int,int)のほうを使用してください。" );
		}


		// その他

		#region [ private ]
		//-----------------
		private bool bフレームを作成した;
		private long n移動開始時刻ms;
		private int n画像側開始位置X;
		private int n画像側開始位置Y;
		private int n画像側終了位置X;
		private int n画像側終了位置Y;
		private int n開始サイズH;
		private int n開始サイズW;
		private int n終了サイズH;
		private int n終了サイズW;
		private int n前回表示したフレーム番号;
		private int n総移動時間ms;
		private int n表示側開始位置X;
		private int n表示側開始位置Y;
		private int n表示側終了位置X;
		private int n表示側終了位置Y;
		private IntPtr pBmp;
		private CDTX.CAVI rAVI;
		private CTexture tx描画用;
		//-----------------
		#endregion
	}
}
