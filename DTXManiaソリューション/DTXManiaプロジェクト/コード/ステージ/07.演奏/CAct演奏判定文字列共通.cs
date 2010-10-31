using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CAct演奏判定文字列共通 : CActivity
	{
		// プロパティ

		protected STSTATUS[] st状態 = new STSTATUS[ 12 ];
		[StructLayout( LayoutKind.Sequential )]
		protected struct STSTATUS
		{
			public CCounter ct進行;
			public E判定 judge;
			public float fX方向拡大率;
			public float fY方向拡大率;
			public int n相対X座標;
			public int n相対Y座標;
			public int n透明度;
		}

		protected readonly ST判定文字列[] st判定文字列;
		[StructLayout( LayoutKind.Sequential )]
		protected struct ST判定文字列
		{
			public int n画像番号;
			public Rectangle rc;
		}
		
		protected CTexture[] tx判定文字列 = new CTexture[ 3 ];


		// コンストラクタ

		public CAct演奏判定文字列共通()
		{
			ST判定文字列[] st判定文字列Array = new ST判定文字列[ 7 ];
			ST判定文字列 st判定文字列 = new ST判定文字列();
			st判定文字列.n画像番号 = 0;
			st判定文字列.rc = new Rectangle( 0, 0, 0x80, 0x2a );
			st判定文字列Array[ 0 ] = st判定文字列;
			ST判定文字列 st判定文字列2 = new ST判定文字列();
			st判定文字列2.n画像番号 = 0;
			st判定文字列2.rc = new Rectangle( 0, 0x2b, 0x80, 0x2a );
			st判定文字列Array[ 1 ] = st判定文字列2;
			ST判定文字列 st判定文字列3 = new ST判定文字列();
			st判定文字列3.n画像番号 = 0;
			st判定文字列3.rc = new Rectangle( 0, 0x56, 0x80, 0x2a );
			st判定文字列Array[ 2 ] = st判定文字列3;
			ST判定文字列 st判定文字列4 = new ST判定文字列();
			st判定文字列4.n画像番号 = 1;
			st判定文字列4.rc = new Rectangle( 0, 0, 0x80, 0x2a );
			st判定文字列Array[ 3 ] = st判定文字列4;
			ST判定文字列 st判定文字列5 = new ST判定文字列();
			st判定文字列5.n画像番号 = 1;
			st判定文字列5.rc = new Rectangle( 0, 0x2b, 0x80, 0x2a );
			st判定文字列Array[ 4 ] = st判定文字列5;
			ST判定文字列 st判定文字列6 = new ST判定文字列();
			st判定文字列6.n画像番号 = 1;
			st判定文字列6.rc = new Rectangle( 0, 0x56, 0x80, 0x2a );
			st判定文字列Array[ 5 ] = st判定文字列6;
			ST判定文字列 st判定文字列7 = new ST判定文字列();
			st判定文字列7.n画像番号 = 2;
			st判定文字列7.rc = new Rectangle( 0, 0, 0x80, 0x2a );
			st判定文字列Array[ 6 ] = st判定文字列7;
			this.st判定文字列 = st判定文字列Array;
			base.b活性化してない = true;
		}


		// メソッド

		public virtual void Start( int nLane, E判定 judge )
		{
			if( ( nLane < 0 ) || ( nLane > 11 ) )
			{
				throw new IndexOutOfRangeException( "有効範囲は 0～11 です。" );
			}
			if( ( ( nLane >= 8 ) || ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Drums ) != E判定文字表示位置.表示OFF ) ) && ( ( ( nLane != 10 ) || ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) != E判定文字表示位置.表示OFF ) ) && ( ( nLane != 11 ) || ( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Bass ) != E判定文字表示位置.表示OFF ) ) ) )
			{
				this.st状態[ nLane ].ct進行 = new CCounter( 0, 300, 1, CDTXMania.Timer );
				this.st状態[ nLane ].judge = judge;
				this.st状態[ nLane ].fX方向拡大率 = 1f;
				this.st状態[ nLane ].fY方向拡大率 = 1f;
				this.st状態[ nLane ].n相対X座標 = 0;
				this.st状態[ nLane ].n相対Y座標 = 0;
				this.st状態[ nLane ].n透明度 = 0xff;
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			for( int i = 0; i < 12; i++ )
			{
				this.st状態[ i ].ct進行 = new CCounter();
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			for( int i = 0; i < 12; i++ )
			{
				this.st状態[ i ].ct進行 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx判定文字列[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay judge strings 1.png" ) );
				this.tx判定文字列[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay judge strings 2.png" ) );
				this.tx判定文字列[ 2 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay judge strings 3.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx判定文字列[ 0 ] );
				CDTXMania.tテクスチャの解放( ref this.tx判定文字列[ 1 ] );
				CDTXMania.tテクスチャの解放( ref this.tx判定文字列[ 2 ] );
				base.OnManagedリソースの解放();
			}
		}
	}
}
