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
		}
		public override void On非活性化()
		{
			if ( this.b活性化してない )
				return;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if ( this.b活性化してない )
				return;
			this.txNowEnumeratingSongs = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenConfig menu cursor.png" ), false );
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
				return 0;

			if ( this.txNowEnumeratingSongs != null )
			{
				this.txNowEnumeratingSongs.t2D描画( CDTXMania.app.Device, 0, 0 );
			}

			return 0;
		}


		private CTexture txNowEnumeratingSongs = null;
	}
}
