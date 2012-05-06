using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using SlimDX;
using SlimDX.DirectSound;

namespace FDK
{
	public class CSound管理 : IDisposable
	{
		// 定数

		public static int nバッファの数 = 0x20;
		public static int nキープする再生済みバッファの数 = (nバッファの数 / 3);
		public static int nバッファサイズ = 0x1000;
		public static int n最大オンメモリ可能サイズ = 0x300000;


		// プロパティ

//		public DirectSound Device { get; private set; }
		private DirectSound Device;
		private void setDevice( DirectSound Device_ )
		{
			Device = Device_;
		}
		public DirectSound getDevice()
		{
			return Device;
		}

		public int n登録サウンド数
		{
			get
			{
				if( this.listSound == null )
				{
					return 0;
				}
				return this.listSound.Count;
			}
		}


		// コンストラクタ

		public CSound管理( IntPtr hWnd )
		{
			try
			{
				this.Device = new SlimDX.DirectSound.DirectSound();
				Trace.TraceInformation( "MaxHardwareMixing: AllBuffers={0}, StaticBuffers={1}, StreamingBuffers={2}", new object[] { this.Device.Capabilities.MaxHardwareMixingAllBuffers, this.Device.Capabilities.MaxHardwareMixingStaticBuffers, this.Device.Capabilities.MaxHardwareMixingStreamingBuffers } );
				Trace.TraceInformation( "FreeHardwareMixing: AllBuffers={0}, StaticBuffers={1}, StreamingBuffers={2}", new object[] { this.Device.Capabilities.FreeHardwareMixingAllBuffers, this.Device.Capabilities.FreeHardwareMixingStaticBuffers, this.Device.Capabilities.FreeHardwareMixingStreamingBuffers } );
				Trace.TraceInformation( "FreeHardwareMemory: {0}", new object[] { this.Device.Capabilities.FreeHardwareMemory } );
				Trace.TraceInformation( "MaxContiguousFreeHardwareMemoryBytes: {0}", new object[] { this.Device.Capabilities.MaxContiguousFreeHardwareMemoryBytes } );
				Trace.TraceInformation( "SecondarySampleRate: {0} - {1} Hz", new object[] { this.Device.Capabilities.MinSecondarySampleRate, this.Device.Capabilities.MaxSecondarySampleRate } );
				Trace.TraceInformation( "PlayCpuOverheadSoftwareBuffers: {0}", new object[] { this.Device.Capabilities.PlayCpuOverheadSoftwareBuffers } );
				Trace.TraceInformation( "PrimaryBuffers: {0}", new object[] { this.Device.Capabilities.PrimaryBuffers } );
				Trace.TraceInformation( "Primary   8Bit:{0}, 16Bit:{1}, Mono:{2}, Stereo:{3}", new object[] { this.Device.Capabilities.Primary8Bit ? "supported" : "not supported", this.Device.Capabilities.Primary16Bit ? "supported" : "not supported", this.Device.Capabilities.PrimaryMono ? "supported" : "not supported", this.Device.Capabilities.PrimaryStereo ? "supported" : "not supported" } );
				Trace.TraceInformation( "Secondary 8Bit:{0}, 16Bit:{1}, Mono:{2}, Stereo:{3}", new object[] { this.Device.Capabilities.Secondary8Bit ? "supported" : "not supported", this.Device.Capabilities.Secondary16Bit ? "supported" : "not supported", this.Device.Capabilities.SecondaryMono ? "supported" : "not supported", this.Device.Capabilities.SecondaryStereo ? "supported" : "not supported" } );
				Trace.TraceInformation( "TotalHardwareMemory: {0}", new object[] { this.Device.Capabilities.TotalHardwareMemory } );
				Trace.TraceInformation( "UnlockTransferRateHardwareBuffers: {0}", new object[] { this.Device.Capabilities.UnlockTransferRateHardwareBuffers } );
			}
			catch( DirectSoundException exception )
			{
				Trace.TraceError( exception.Message );
				Trace.TraceError( "DirectSound デバイスの作成に失敗しました。" );
				this.Device = null;
				return;
			}
			try
			{
				this.Device.SetCooperativeLevel( hWnd, CooperativeLevel.Priority );
				Trace.TraceInformation( " DirectSound デバイスの協調レベルを Priority に設定しました。" );
			}
			catch( DirectSoundException )
			{
				try
				{
					this.Device.SetCooperativeLevel( hWnd, CooperativeLevel.Normal );
					Trace.TraceInformation( " DirectSound デバイスの協調レベルを Normal に設定しました。" );
				}
				catch( DirectSoundException )
				{
					this.Dispose();
					Trace.TraceError( " DirectSound デバイスの協調レベルの設定に失敗しました。" );
				}
			}
		}


		// メソッド

		public CSound tサウンドを生成する( string strファイル名 )
		{
			if( string.IsNullOrEmpty( strファイル名 ) || !File.Exists( strファイル名 ) )
				throw new ArgumentException( "ファイル名が無効です。" );

			CSound item = null;
//			if( ( ( ( item = this.tネイティブOggVorbisの場合( strファイル名 ) ) == null ) && ( ( item = this.tネイティブXAの場合( strファイル名 ) ) == null ) ) && ( ( ( item = this.tネイティブmp3の場合( strファイル名 ) ) == null ) && ( ( item = this.tRIFF_WAVEの場合( strファイル名 ) ) == null ) ) )
			if ( (item = this.tCheckAndDecode(strファイル名)) == null)
				throw new Exception( "OggVorbis, mp3, XA, RIFF ACM のいずれでもデコードに失敗しました。" );

			item.setDevice( ref this.Device );
			tサウンドを登録する( item );

			return item;
		}
		public void tサウンドを登録する( CSound sound )
		{
			lock ( this.obj排他用 )
			{
				this.listSound.Add( sound );
			}
		}

		public void t再生中の処理をする( object o )			// #26122 2011.9.1 yyagi; delegate経由の呼び出し用
		{
			t再生中の処理をする();
		}
		public void t再生中の処理をする()
		{
			lock( this.obj排他用 )
			{
				foreach( CSound sound in this.listSound )
				{
					if( sound.b再生中 && sound.bストリーム再生する )
					{
						sound.t再生中の処理をする();
					}
				}
			}
		}

		//public void t再生中の処理をする_loop()
		//{
		//    while ( true )
		//    {
		//        t再生中の処理をする();
		//        System.Threading.Thread.Sleep( 100 );
		//    }
		//}

	
		public void tサウンドを破棄する( CSound sound )
		{
			lock( this.obj排他用 )
			{
				if( sound != null )
				{
					sound.Dispose();
					this.listSound.Remove( sound );
				}
			}
		}

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDisposed )
			{
				foreach( CSound sound in this.listSound )
				{
					sound.Dispose();
				}
				this.listSound.Clear();
				if( this.Device != null )
				{
					this.Device.Dispose();
					this.Device = null;
				}

				this.bDisposed = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private object obj排他用 = new object();
		private bool bDisposed = false;
		private List<CSound> listSound = new List<CSound>();


		private CSound tCheckAndDecode(string strFilename)
		{
			int nDecodedPCMsize;
			int nHandle;

			CSound csound = new CSoundOggVorbis();
			nDecodedPCMsize = csound.tデコード後のサイズを調べる( strFilename, out nHandle );
			if ( nDecodedPCMsize < 0 )
			{
				csound.Dispose();
				csound = new CSoundXA();
				nDecodedPCMsize = csound.tデコード後のサイズを調べる( strFilename, out nHandle );
				if ( nDecodedPCMsize < 0 )
				{
					csound.Dispose();
					csound = new CSoundRiffWave();
					nDecodedPCMsize = csound.tデコード後のサイズを調べる( strFilename, out nHandle );
					if ( nDecodedPCMsize < 0 )
					{
						csound.Dispose();
						csound = new CSoundMp3();
						nDecodedPCMsize = csound.tデコード後のサイズを調べる(strFilename, out nHandle);
						if ( nDecodedPCMsize < 0 )
						{
							csound.Dispose();
							return null;
						}
					}
				}
			}
			if (nDecodedPCMsize < n最大オンメモリ可能サイズ)
			{
				csound.tオンメモリ方式で作成する(this.Device, strFilename, nHandle);
			}
			else
			{
				csound.tストリーム方式で作成する(this.Device, strFilename, nHandle);
			}
			return csound;
		}
		#endregion
	}
}
