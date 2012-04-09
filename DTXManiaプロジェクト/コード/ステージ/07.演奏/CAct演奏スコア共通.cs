using System;
using System.Collections.Generic;
using System.Text;
using FDK;

namespace DTXMania
{
	internal class CAct演奏スコア共通 : CActivity
	{
		// プロパティ

		protected STDGBVALUE<long> nスコアの増分;
		protected STDGBVALUE<long> n現在の本当のスコア;
		protected STDGBVALUE<long> n現在表示中のスコア;
		protected long n進行用タイマ;
		protected CTexture txScore;

		
		// コンストラクタ

		public CAct演奏スコア共通()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public long Get( E楽器パート part )
		{
			return this.n現在の本当のスコア[ (int) part ];
		}
		public void Set( E楽器パート part, long nScore )
		{
			int num = (int) part;
			if( this.n現在の本当のスコア[ num ] != nScore )
			{
				this.n現在の本当のスコア[ num ] = nScore;
				this.nスコアの増分[ num ] = (long) ( ( (double) ( this.n現在の本当のスコア[ num ] - this.n現在表示中のスコア[ num ] ) ) / 20.0 );
				if( this.nスコアの増分[ num ] < 1L )
				{
					this.nスコアの増分[ num ] = 1L;
				}
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.n進行用タイマ = -1;
			for( int i = 0; i < 3; i++ )
			{
				this.n現在表示中のスコア[ i ] = 0L;
				this.n現在の本当のスコア[ i ] = 0L;
				this.nスコアの増分[ i ] = 0L;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txScore = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay score numbers.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txScore );
				base.OnManagedリソースの解放();
			}
		}
	}
}
