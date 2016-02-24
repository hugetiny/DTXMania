using System;
using System.Collections.Generic;
using System.Text;
using FDK;

namespace DTXMania
{
	internal class CAct演奏レーンフラッシュGB共通 : CActivity
	{
		// プロパティ

		protected CCounter[] ct進行 = new CCounter[ 6 ];
		protected CTextureAf[] txFlush = new CTextureAf[ 6 ];


		// コンストラクタ

		public CAct演奏レーンフラッシュGB共通()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void Start( int nLane )
		{
			if( ( nLane < 0 ) || ( nLane > 6 ) )
			{
				throw new IndexOutOfRangeException( "有効範囲は 0～6 です。" );
			}
			this.ct進行[ nLane ] = new CCounter( 0, 100, 1, CDTXMania.Instance.Timer );
		}


		// CActivity 実装

		public override void On活性化()
		{
			for( int i = 0; i < 6; i++ )
			{
				this.ct進行[ i ] = new CCounter();
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			for( int i = 0; i < 6; i++ )
			{
				this.ct進行[ i ] = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txFlush[ 0 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush red.png" ) );
				this.txFlush[ 1 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush green.png" ) );
				this.txFlush[ 2 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush blue.png" ) );
				this.txFlush[ 3 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush red reverse.png" ) );
				this.txFlush[ 4 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush green reverse.png" ) );
				this.txFlush[ 5 ] = TextureFactory.tテクスチャの生成Af( CSkin.Path( @"Graphics\ScreenPlay lane flush blue reverse.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 6; i++ )
				{
					TextureFactory.tテクスチャの解放( ref this.txFlush[ i ] );
				}
				base.OnManagedリソースの解放();
			}
		}
	}
}
