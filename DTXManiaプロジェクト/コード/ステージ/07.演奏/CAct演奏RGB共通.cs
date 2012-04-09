using System;
using System.Collections.Generic;
using System.Text;
using FDK;

namespace DTXMania
{
	internal class CAct演奏RGB共通 : CActivity
	{
		// プロパティ

		protected bool[] b押下状態 = new bool[ 6 ];
		protected CTexture txRGB;


		// コンストラクタ

		public CAct演奏RGB共通()
		{
			base.b活性化してない = true;
		}
		
		
		// メソッド

		public void Push( int nLane )
		{
			this.b押下状態[ nLane ] = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			for( int i = 0; i < 6; i++ )
			{
				this.b押下状態[ i ] = false;
			}
			base.On活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txRGB = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlay RGB buttons.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txRGB );
				base.OnManagedリソースの解放();
			}
		}
	}
}
