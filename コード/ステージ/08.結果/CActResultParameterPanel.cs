using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CActResultParameterPanel : CActivity
	{
		// コンストラクタ

		public CActResultParameterPanel()
		{
			ST文字位置[] st文字位置Array = new ST文字位置[ 11 ];
			ST文字位置 st文字位置 = new ST文字位置();
			st文字位置.ch = '0';
			st文字位置.pt = new Point( 0, 0 );
			st文字位置Array[ 0 ] = st文字位置;
			ST文字位置 st文字位置2 = new ST文字位置();
			st文字位置2.ch = '1';
			st文字位置2.pt = new Point( 11, 0 );
			st文字位置Array[ 1 ] = st文字位置2;
			ST文字位置 st文字位置3 = new ST文字位置();
			st文字位置3.ch = '2';
			st文字位置3.pt = new Point( 0x16, 0 );
			st文字位置Array[ 2 ] = st文字位置3;
			ST文字位置 st文字位置4 = new ST文字位置();
			st文字位置4.ch = '3';
			st文字位置4.pt = new Point( 0x21, 0 );
			st文字位置Array[ 3 ] = st文字位置4;
			ST文字位置 st文字位置5 = new ST文字位置();
			st文字位置5.ch = '4';
			st文字位置5.pt = new Point( 0x2c, 0 );
			st文字位置Array[ 4 ] = st文字位置5;
			ST文字位置 st文字位置6 = new ST文字位置();
			st文字位置6.ch = '5';
			st文字位置6.pt = new Point( 0, 0x10 );
			st文字位置Array[ 5 ] = st文字位置6;
			ST文字位置 st文字位置7 = new ST文字位置();
			st文字位置7.ch = '6';
			st文字位置7.pt = new Point( 11, 0x10 );
			st文字位置Array[ 6 ] = st文字位置7;
			ST文字位置 st文字位置8 = new ST文字位置();
			st文字位置8.ch = '7';
			st文字位置8.pt = new Point( 0x16, 0x10 );
			st文字位置Array[ 7 ] = st文字位置8;
			ST文字位置 st文字位置9 = new ST文字位置();
			st文字位置9.ch = '8';
			st文字位置9.pt = new Point( 0x21, 0x10 );
			st文字位置Array[ 8 ] = st文字位置9;
			ST文字位置 st文字位置10 = new ST文字位置();
			st文字位置10.ch = '9';
			st文字位置10.pt = new Point( 0x2c, 0x10 );
			st文字位置Array[ 9 ] = st文字位置10;
			ST文字位置 st文字位置11 = new ST文字位置();
			st文字位置11.ch = '.';
			st文字位置11.pt = new Point( 0x37, 0x10 );
			st文字位置Array[ 10 ] = st文字位置11;
			this.st大文字位置 = st文字位置Array;
			ST文字位置[] st文字位置Array2 = new ST文字位置[ 11 ];
			ST文字位置 st文字位置12 = new ST文字位置();
			st文字位置12.ch = '0';
			st文字位置12.pt = new Point( 0, 0x20 );
			st文字位置Array2[ 0 ] = st文字位置12;
			ST文字位置 st文字位置13 = new ST文字位置();
			st文字位置13.ch = '1';
			st文字位置13.pt = new Point( 11, 0x20 );
			st文字位置Array2[ 1 ] = st文字位置13;
			ST文字位置 st文字位置14 = new ST文字位置();
			st文字位置14.ch = '2';
			st文字位置14.pt = new Point( 0x16, 0x20 );
			st文字位置Array2[ 2 ] = st文字位置14;
			ST文字位置 st文字位置15 = new ST文字位置();
			st文字位置15.ch = '3';
			st文字位置15.pt = new Point( 0x21, 0x20 );
			st文字位置Array2[ 3 ] = st文字位置15;
			ST文字位置 st文字位置16 = new ST文字位置();
			st文字位置16.ch = '4';
			st文字位置16.pt = new Point( 0x2c, 0x20 );
			st文字位置Array2[ 4 ] = st文字位置16;
			ST文字位置 st文字位置17 = new ST文字位置();
			st文字位置17.ch = '5';
			st文字位置17.pt = new Point( 0, 0x30 );
			st文字位置Array2[ 5 ] = st文字位置17;
			ST文字位置 st文字位置18 = new ST文字位置();
			st文字位置18.ch = '6';
			st文字位置18.pt = new Point( 11, 0x30 );
			st文字位置Array2[ 6 ] = st文字位置18;
			ST文字位置 st文字位置19 = new ST文字位置();
			st文字位置19.ch = '7';
			st文字位置19.pt = new Point( 0x16, 0x30 );
			st文字位置Array2[ 7 ] = st文字位置19;
			ST文字位置 st文字位置20 = new ST文字位置();
			st文字位置20.ch = '8';
			st文字位置20.pt = new Point( 0x21, 0x30 );
			st文字位置Array2[ 8 ] = st文字位置20;
			ST文字位置 st文字位置21 = new ST文字位置();
			st文字位置21.ch = '9';
			st文字位置21.pt = new Point( 0x2c, 0x30 );
			st文字位置Array2[ 9 ] = st文字位置21;
			ST文字位置 st文字位置22 = new ST文字位置();
			st文字位置22.ch = '%';
			st文字位置22.pt = new Point( 0x37, 0x30 );
			st文字位置Array2[ 10 ] = st文字位置22;
			this.st小文字位置 = st文字位置Array2;
			this.ptFullCombo位置 = new Point[] { new Point( 0x80, 0xed ), new Point( 0xdf, 0xed ), new Point( 0x141, 0xed ) };
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct表示用.n現在の値 = this.ct表示用.n終了値;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n本体X = 0xf2;
			this.n本体Y = 0x44;
			this.sdDTXで指定されたフルコンボ音 = null;
			this.bフルコンボ音再生済み = false;
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.ct表示用 != null )
			{
				this.ct表示用 = null;
			}
			if( this.sdDTXで指定されたフルコンボ音 != null )
			{
				CDTXMania.Sound管理.tサウンドを破棄する( this.sdDTXで指定されたフルコンボ音 );
				this.sdDTXで指定されたフルコンボ音 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txパネル本体 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult parameter panel.png" ), true );
				this.tx文字[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult parameter numbers.png" ) );
				this.tx文字[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult parameter numbers em.png" ) );
				this.txFullCombo = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenResult fullcombo.png" ) );
				this.txWhite = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Tile white 64x64.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txパネル本体 );
				CDTXMania.tテクスチャの解放( ref this.tx文字[ 0 ] );
				CDTXMania.tテクスチャの解放( ref this.tx文字[ 1 ] );
				CDTXMania.tテクスチャの解放( ref this.txFullCombo );
				CDTXMania.tテクスチャの解放( ref this.txWhite );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない )
			{
				return 0;
			}
			if( base.b初めての進行描画 )
			{
				this.ct表示用 = new CCounter( 0, 0x3e7, 2, CDTXMania.Timer );
				base.b初めての進行描画 = false;
			}
			this.ct表示用.t進行();
			if( this.txパネル本体 != null )
			{
				this.txパネル本体.t2D描画( CDTXMania.app.Device, this.n本体X, this.n本体Y );
			}
			int num = this.ct表示用.n現在の値;
			Point[] pointArray = new Point[] { new Point( 0x68, 0x29 ), new Point( 0xc7, 0x29 ), new Point( 0x129, 0x29 ) };
			for( int i = 0; i < 3; i++ )
			{
				int x = this.n本体X + pointArray[ i ].X;
				int y = this.n本体Y + pointArray[ i ].Y;
				if( num >= 0 )
				{
					this.t大文字表示( x, y, CDTXMania.stage結果.st演奏記録[ i ].nPerfect数.ToString( "00000" ) );
				}
				if( num >= 100 )
				{
					this.t大文字表示( x, y + 0x16, CDTXMania.stage結果.st演奏記録[ i ].nGreat数.ToString( "00000" ) );
				}
				if( num >= 200 )
				{
					this.t大文字表示( x, y + 0x2c, CDTXMania.stage結果.st演奏記録[ i ].nGood数.ToString( "00000" ) );
				}
				if( num >= 300 )
				{
					this.t大文字表示( x, y + 0x42, CDTXMania.stage結果.st演奏記録[ i ].nPoor数.ToString( "00000" ) );
				}
				if( num >= 400 )
				{
					this.t大文字表示( x, y + 0x58, CDTXMania.stage結果.st演奏記録[ i ].nMiss数.ToString( "00000" ) );
				}
				if( num >= 0 )
				{
					this.t小文字表示( x + 0x30, y, string.Format( "{0,3:##0}%", CDTXMania.stage結果.fPerfect率[ i ] ) );
				}
				if( num >= 100 )
				{
					this.t小文字表示( x + 0x30, y + 0x16, string.Format( "{0,3:##0}%", CDTXMania.stage結果.fGreat率[ i ] ) );
				}
				if( num >= 200 )
				{
					this.t小文字表示( x + 0x30, y + 0x2c, string.Format( "{0,3:##0}%", CDTXMania.stage結果.fGood率[ i ] ) );
				}
				if( num >= 300 )
				{
					this.t小文字表示( x + 0x30, y + 0x42, string.Format( "{0,3:##0}%", CDTXMania.stage結果.fPoor率[ i ] ) );
				}
				if( num >= 400 )
				{
					this.t小文字表示( x + 0x30, y + 0x58, string.Format( "{0,3:##0}%", CDTXMania.stage結果.fMiss率[ i ] ) );
				}
				if( num >= 500 )
				{
					this.t大文字表示( x, y + 110, string.Format( "{0,9:########0}", CDTXMania.stage結果.st演奏記録[ i ].n最大コンボ数 ) );
				}
				if( num >= 600 )
				{
					this.t大文字表示( x, y + 0x84, CDTXMania.stage結果.st演奏記録[ i ].nスコア.ToString( "000000000" ), CDTXMania.stage結果.b新記録スコア[ i ] );
				}
				if( num >= 700 )
				{
					this.t大文字表示( x, y + 0x9a, string.Format( "{0,9:#####0.00}", CDTXMania.stage結果.st演奏記録[ i ].db演奏型スキル値 ), CDTXMania.stage結果.b新記録スキル[ i ] );
				}
				if( num >= 700 )
				{
					this.t小文字表示( x + 0x48, y + 0x9a, "%" );
				}
				if( num >= 800 )
				{
					this.t大文字表示( x, y + 0xb0, string.Format( "{0,9:########0}", CDTXMania.stage結果.n演奏回数[ i ] ) );
				}
			}
			if( this.ct表示用.n現在の値 < 900 )
			{
				int num5 = this.ct表示用.n現在の値 / 100;
				double num6 = 1.0 - ( ( (double) ( this.ct表示用.n現在の値 % 100 ) ) / 100.0 );
				int num7 = 0x157;
				int num8 = 0x6d + ( num5 * 0x16 );
				int num9 = 0x11c;
				int num10 = 0x10;
				if( this.txWhite != null )
				{
					this.txWhite.n透明度 = (int) ( 255.0 * num6 );
					Rectangle rectangle = new Rectangle( 0, 0, 0x40, 0 );
					while( num9 > 0 )
					{
						rectangle.Height = num10;
						if( num9 < 0x40 )
						{
							rectangle.Width = num9;
						}
						this.txWhite.t2D描画( CDTXMania.app.Device, num7, num8, rectangle );
						num7 += 0x40;
						num9 -= 0x40;
					}
				}
			}
			if( this.ct表示用.n現在の値 >= 900 )
			{
				for( int j = 0; j < 3; j++ )
				{
					if( CDTXMania.stage結果.st演奏記録[ j ].bフルコンボである )
					{
						if( this.ct表示用.b終了値に達した )
						{
							if( this.txFullCombo != null )
							{
								this.txFullCombo.t2D描画( CDTXMania.app.Device, this.n本体X + this.ptFullCombo位置[ j ].X, this.n本体Y + this.ptFullCombo位置[ j ].Y );
							}
							if( !this.bフルコンボ音再生済み )
							{
								if( ( ( CDTXMania.DTX.SOUND_FULLCOMBO != null ) && ( CDTXMania.DTX.SOUND_FULLCOMBO.Length > 0 ) ) && File.Exists( CDTXMania.DTX.strフォルダ名 + CDTXMania.DTX.SOUND_FULLCOMBO ) )
								{
									try
									{
										if( this.sdDTXで指定されたフルコンボ音 != null )
										{
											CDTXMania.Sound管理.tサウンドを破棄する( this.sdDTXで指定されたフルコンボ音 );
											this.sdDTXで指定されたフルコンボ音 = null;
										}
										this.sdDTXで指定されたフルコンボ音 = CDTXMania.Sound管理.tサウンドを生成する( CDTXMania.DTX.strフォルダ名 + CDTXMania.DTX.SOUND_FULLCOMBO );
										if( this.sdDTXで指定されたフルコンボ音 != null )
										{
											this.sdDTXで指定されたフルコンボ音.t再生を開始する();
										}
									}
									catch
									{
									}
								}
								else
								{
									CDTXMania.Skin.soundフルコンボ音.t再生する();
								}
								this.bフルコンボ音再生済み = true;
							}
						}
						else
						{
							double num12 = ( (double) ( this.ct表示用.n現在の値 - 900 ) ) / 100.0;
							float num13 = (float) ( 2.0 - num12 );
							if( this.txFullCombo != null )
							{
								this.txFullCombo.vc拡大縮小倍率 = new Vector3( num13, num13, 1f );
								this.txFullCombo.n透明度 = (int) ( 255.0 * num12 );
								int num14 = ( this.n本体X + this.ptFullCombo位置[ j ].X ) + ( (int) ( ( this.txFullCombo.sz画像サイズ.Width * ( 1f - num13 ) ) / 2f ) );
								int num15 = ( this.n本体Y + this.ptFullCombo位置[ j ].Y ) + ( (int) ( ( this.txFullCombo.sz画像サイズ.Height * ( 1f - num13 ) ) / 2f ) );
								this.txFullCombo.t2D描画( CDTXMania.app.Device, num14, num15 );
							}
						}
					}
				}
			}
			if( !this.ct表示用.b終了値に達した )
			{
				return 0;
			}
			return 1;
		}
		

		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct ST文字位置
		{
			public char ch;
			public Point pt;
		}

		private bool bフルコンボ音再生済み;
		private CCounter ct表示用;
		private int n本体X;
		private int n本体Y;
		private readonly Point[] ptFullCombo位置;
		private CSound sdDTXで指定されたフルコンボ音;
		private readonly ST文字位置[] st小文字位置;
		private readonly ST文字位置[] st大文字位置;
		private CTexture txFullCombo;
		private CTexture txWhite;
		private CTexture txパネル本体;
		private CTexture[] tx文字 = new CTexture[ 2 ];

		private void t小文字表示( int x, int y, string str )
		{
			this.t小文字表示( x, y, str, false );
		}
		private void t小文字表示( int x, int y, string str, bool b強調 )
		{
			foreach( char ch in str )
			{
				for( int i = 0; i < this.st小文字位置.Length; i++ )
				{
					if( this.st小文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle( this.st小文字位置[ i ].pt.X, this.st小文字位置[ i ].pt.Y, 11, 0x10 );
						if( ch == '%' )
						{
							rectangle.Width -= 2;
							rectangle.Height -= 2;
						}
						if( this.tx文字[ b強調 ? 1 : 0 ] != null )
						{
							this.tx文字[ b強調 ? 1 : 0 ].t2D描画( CDTXMania.app.Device, x, y, rectangle );
						}
						break;
					}
				}
				x += 8;
			}
		}
		private void t大文字表示( int x, int y, string str )
		{
			this.t大文字表示( x, y, str, false );
		}
		private void t大文字表示( int x, int y, string str, bool b強調 )
		{
			foreach( char ch in str )
			{
				for( int i = 0; i < this.st大文字位置.Length; i++ )
				{
					if( this.st大文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle( this.st大文字位置[ i ].pt.X, this.st大文字位置[ i ].pt.Y, 11, 0x10 );
						if( ch == '.' )
						{
							rectangle.Width -= 2;
							rectangle.Height -= 2;
						}
						if( this.tx文字[ b強調 ? 1 : 0 ] != null )
						{
							this.tx文字[ b強調 ? 1 : 0 ].t2D描画( CDTXMania.app.Device, x, y, rectangle );
						}
						break;
					}
				}
				x += 8;
			}
		}
		//-----------------
		#endregion
	}
}
