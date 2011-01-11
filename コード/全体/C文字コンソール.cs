using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class C文字コンソール : CActivity
	{
		// 定数

		public enum Eフォント種別
		{
			白,
			赤,
			灰,
			白細,
			赤細,
			灰細
		}
		public enum E配置
		{
			左詰,
			中央,
			右詰
		}


		// メソッド

		public void tPrint( int x, int y, Eフォント種別 font, string str英数字文字列 )
		{
			if( !base.b活性化してない && !string.IsNullOrEmpty( str英数字文字列 ) )
			{
				int num = x;
				for( int i = 0; i < str英数字文字列.Length; i++ )
				{
					char ch = str英数字文字列[ i ];
					if( ch == '\n' )
					{
						x = num;
						y += 0x10;
					}
					else
					{
						int num3 = 0;
						while( num3 < " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ".Length )
						{
							if( " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ "[ num3 ] == ch )
							{
								break;
							}
							num3++;
						}
						if( num3 >= " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ".Length )
						{
							x += 8;
						}
						else
						{
							if( this.txフォント8x16[ (int) ( (int) font / (int) Eフォント種別.白細 ) ] != null )
							{
								this.txフォント8x16[ (int) ( (int) font / (int) Eフォント種別.白細 ) ].t2D描画( CDTXMania.app.Device, x, y, this.rc文字の矩形領域[ (int) ( (int) font % (int) Eフォント種別.白細 ), num3 ] );
							}
							x += 8;
						}
					}
				}
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.rc文字の矩形領域 = new Rectangle[ 3, 0x60 ];
			for( int i = 0; i < 3; i++ )
			{
				for( int j = 0; j < 0x60; j++ )
				{
					this.rc文字の矩形領域[ i, j ].X = ( ( i / 2 ) * 0x80 ) + ( ( j % 0x10 ) * 8 );
					this.rc文字の矩形領域[ i, j ].Y = ( ( i % 2 ) * 0x80 ) + ( ( j / 0x10 ) * 0x10 );
					this.rc文字の矩形領域[ i, j ].Width = 8;
					this.rc文字の矩形領域[ i, j ].Height = 0x10;
				}
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.rc文字の矩形領域 != null )
				this.rc文字の矩形領域 = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txフォント8x16[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Console font 8x16.png" ) );
				this.txフォント8x16[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Console font 2 8x16.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 2; i++ )
				{
					if( this.txフォント8x16[ i ] != null )
					{
						this.txフォント8x16[ i ].Dispose();
						this.txフォント8x16[ i ] = null;
					}
				}
				base.OnManagedリソースの解放();
			}
		}


		// その他

		#region [ private ]
		//-----------------
		private Rectangle[,] rc文字の矩形領域;
		private const string str表記可能文字 = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ";
		private CTexture[] txフォント8x16 = new CTexture[ 2 ];
		//-----------------
		#endregion
	}
}
