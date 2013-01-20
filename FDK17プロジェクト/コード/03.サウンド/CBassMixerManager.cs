using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SlimDX;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Mix;
using DirectShowLib;

namespace FDK
{
	/// <summary>
	/// メインスレッドとは別のスレッドで動かす前提。
	/// </summary>
	public class CBassMixerManager : IDisposable
	{
		private object lockmixer = null;
		//delegate void AddMixerChannelHandler( CSound cs );
		//delegate void RemoveMixerChannelHandler( CSound cs );

		//public event AddMixerChannelHandler AddMixerChannelEvent = null;
		//public event RemoveMixerChannelHandler RemoveMixerChannelEvent = null;

		private struct stmixer
		{
			internal bool bIsAdd;
			internal CSound csound;
		};

		public void AddMixer( CSound cs )
		{
			stmixer stm = new stmixer()
			{
				bIsAdd = true,
				csound = cs
			};
			lock ( lockmixer )
			{
				queueMixerSound.Enqueue( stm );
			}
		}
		public void RemoveMixer( CSound cs )
		{
			stmixer stm = new stmixer()
			{
				bIsAdd = false,
				csound = cs
			};
			lock ( lockmixer )
			{
				queueMixerSound.Enqueue( stm );
			}
		}

		public CBassMixerManager()
		{
			//AddMixerChannelEvent += AddMixerChannel;
			//RemoveMixerChannelEvent += RemoveMixerChannel;
			lockmixer = new object();
			queueMixerSound = new Queue<stmixer>( 64 );
			bMixerManaging = true;
			//bEndManaging = false;
		}

		public void Start()
		{
			while ( bMixerManaging )
			{
				while ( queueMixerSound.Count > 0 )
				{
					stmixer stm;
					lock ( lockmixer )
					{
						stm = queueMixerSound.Dequeue();
					}
					if ( stm.bIsAdd )
					{
						tBASSサウンドをミキサーに追加する( stm.csound.hBassStream, stm.csound.hMixer );
					}
					else
					{
						tBASSサウンドをミキサーから削除する( stm.csound.hBassStream );
					}
					Thread.Sleep( 7 );
				}
				Thread.Sleep( 20 );
			}

			//bEndManaging = true;
		}

		#region [ Dispose ]
		//-----------------
		public void Dispose()
		{
			End();
			lock ( lockmixer )
			{
				while ( queueMixerSound.Count > 0 )
				{
					stmixer stm = queueMixerSound.Dequeue();
					stm.csound.tBASSサウンドをミキサーから削除する();
				}

				// 今イベント処理中でないことを確認してから実行しないとダメなはず
				//AddMixerChannelEvent = null;
				//RemoveMixerChannelEvent = null;

				queueMixerSound = null;
				GC.SuppressFinalize( this );
			}
		}
		~CBassMixerManager()
		{
			this.Dispose();
		}
		//-----------------
		#endregion


		public void End()
		{
			bMixerManaging = false;
			// 後はメインスレッド側でJoin()して待ってもらう。
		}


		//// イベントで呼び出される
		//public static void AddMixerChannel( CSound cs )
		//{
		//    // euqueue,dequeueはlockでなく別のロック方法が必要
		//}

		//// イベントで呼び出される
		//public static void RemoveMixerChannel( CSound cs )
		//{

		//}

		private volatile Queue<stmixer> queueMixerSound;		// まずは単純にAdd/Removeを1個のキューでまとめて管理するやり方で設計する
		private bool bMixerManaging;
		//private bool bEndManaging;

		//以下実処理
		public bool tBASSサウンドをミキサーから削除する( int channel )
		{
			lock ( lockmixer )
			{
				bool b = BassMix.BASS_Mixer_ChannelRemove( channel );
				if ( b )
				{
					Interlocked.Decrement( ref CSound管理.nMixing );
					Debug.WriteLine( "Removed: " + channel );
				}
				return b;
			}
		}



		public bool tBASSサウンドをミキサーに追加する( int hBassStream, int hMixer )
		{
			if ( BassMix.BASS_Mixer_ChannelGetMixer( hBassStream ) == 0 )
			{
				BASSFlag bf = BASSFlag.BASS_SPEAKER_FRONT | BASSFlag.BASS_MIXER_NORAMPIN;	// | BASSFlag.BASS_MIXER_PAUSE;
				Interlocked.Increment( ref CSound管理.nMixing );

				// preloadされることを期待して、敢えてflagからはBASS_MIXER_PAUSEを外してAddChannelした上で、すぐにPAUSEする
				bool b1 = BassMix.BASS_Mixer_StreamAddChannel( hMixer, hBassStream, bf );
				bool b2 = BassMix.BASS_Mixer_ChannelPause( hBassStream );
				BassMix.BASS_Mixer_ChannelSetPosition( hBassStream, 0 ); 
				//t再生位置を先頭に戻す();	// StreamAddChannelの後で再生位置を戻さないとダメ。逆だと再生位置が変わらない。
				// Debug.WriteLine( "Add Mixer: " + Path.GetFileName( this.strファイル名 ) + " (" + hBassStream + ")" + " MixedStreams=" + CSound管理.nMixing );
				Debug.WriteLine( "Add Mixer:  MixedStreams=" + CSound管理.nMixing );
				return b1 & b2;
			}
			return true;
		}
	}
}
