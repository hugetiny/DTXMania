using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;
using SampleFramework;

namespace DTXMania
{
	internal class CActEnumSongs :  CActivity
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public CActEnumSongs()
		{
			base.b活性化してない = true;
		}

		// CActivity 実装

		public override void On活性化()
		{
			if ( this.b活性化してる )
				return;
			base.On活性化();

			try
			{
				this.ctNowEnumeratingSongs = new CCounter();	// 0, 1000, 17, CDTXMania.Timer );
				this.ctNowEnumeratingSongs.t開始( 0, 100, 17, CDTXMania.Timer );
			}
			finally
			{
			}
		}
		public override void On非活性化()
		{
			if ( this.b活性化してない )
				return;
			base.On非活性化();
			this.ctNowEnumeratingSongs = null;
		}
		public override void OnManagedリソースの作成()
		{
			if ( this.b活性化してない )
				return;
			string pathNowEnumeratingSongs = CSkin.Path( @"Graphics\ScreenTitle NowEnumeratingSongs.png" );
			if ( File.Exists( pathNowEnumeratingSongs ) )
			{
				this.txNowEnumeratingSongs = CDTXMania.tテクスチャの生成( pathNowEnumeratingSongs, false );
			}
			else
			{
				this.txNowEnumeratingSongs = null;
			}
			base.OnManagedリソースの作成();
		}
		public override void OnManagedリソースの解放()
		{
			if ( this.b活性化してない )
				return;

			CDTXMania.t安全にDisposeする( ref this.txNowEnumeratingSongs );
			base.OnManagedリソースの解放();
		}

		public override int On進行描画()
		{
			if ( this.b活性化してない )
			{
				return 0;
			}
			this.ctNowEnumeratingSongs.t進行Loop();
			if ( this.txNowEnumeratingSongs != null )
			{
				this.txNowEnumeratingSongs.n透明度 = (int) ( 176.0 + 80.0 * Math.Sin( (double) (2 * Math.PI * this.ctNowEnumeratingSongs.n現在の値 * 2 / 100.0 ) ) );
				this.txNowEnumeratingSongs.t2D描画( CDTXMania.app.Device, 18, 7 );
			}

			return 0;
		}


		private CCounter ctNowEnumeratingSongs;
		private CTexture txNowEnumeratingSongs = null;
	}
}
