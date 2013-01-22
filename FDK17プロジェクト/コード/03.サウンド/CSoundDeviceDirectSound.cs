﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SlimDX;
using SlimDX.DirectSound;

namespace FDK
{
	public class CSoundDeviceDirectSound : ISoundDevice
	{
		// プロパティ

		public ESoundDeviceType e出力デバイス
		{
			get;
			protected set;
		}
		public long n実出力遅延ms
		{
			get;
			protected set;
		}
		public static readonly BufferFlags DefaultFlags = BufferFlags.Defer | BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus | BufferFlags.ControlVolume | BufferFlags.ControlPan | BufferFlags.ControlFrequency;

		// CSoundTimer 用に公開しているプロパティ

		public long n経過時間ms
		{
			get
			{
				int n現在位置 = this.sd経過時間計測用サウンドバッファ.DirectSoundBuffer.CurrentPlayPosition;
				long n現在のシステム時刻ms = this.tmシステムタイマ.nシステム時刻ms;

				
				// ループ回数を調整。

				long nシステム時刻での間隔ms = n現在のシステム時刻ms - this.n前に経過時間を測定したシステム時刻ms;

				while( nシステム時刻での間隔ms >= n単位繰り上げ間隔ms )		// 前回から単位繰り上げ間隔以上経過してるなら確実にループしている。誤差は大きくないだろうから無視。
				{
					this.nループ回数++;
					nシステム時刻での間隔ms -= n単位繰り上げ間隔ms;
				}

				if( n現在位置 < this.n前回の位置 )							// 単位繰り上げ間隔以内であっても、現在位置が前回より手前にあるなら1回ループしている。
					this.nループ回数++;


				// 経過時間を算出。

				long n経過時間ms = (long) ( ( this.nループ回数 * n単位繰り上げ間隔ms ) + ( n現在位置 * 1000.0 / ( 44100.0 * 2 * 2 ) ) );


				// 今回の値を次回に向けて保存。

				this.n前に経過時間を測定したシステム時刻ms = n現在のシステム時刻ms;
				this.n前回の位置 = n現在位置;

				return n経過時間ms;
			}
		}
		public long n経過時間を更新したシステム時刻ms
		{
			get { throw new NotImplementedException(); }
		}
		public CTimer tmシステムタイマ
		{
			get;
			protected set;
		}


		// メソッド

		public CSoundDeviceDirectSound( IntPtr hWnd, long n遅延時間ms )
		{
			Trace.TraceInformation( "DirectSound の初期化を開始します。" );

			this.e出力デバイス = ESoundDeviceType.Unknown;
			this.n実出力遅延ms = n遅延時間ms;
			this.tmシステムタイマ = new CTimer( CTimer.E種別.MultiMedia );

			#region [ DirectSound デバイスを作成する。]
			//-----------------
			this.DirectSound = new DirectSound();	// 失敗したら例外をそのまま発出。

			// デバイスの協調レベルを設定する。

			bool priority = true;
			try
			{
				this.DirectSound.SetCooperativeLevel( hWnd, CooperativeLevel.Priority );
			}
			catch( DirectSoundException )
			{
				this.DirectSound.SetCooperativeLevel( hWnd, CooperativeLevel.Normal );	// これでも失敗したら例外をそのまま発出。
				priority = false;
			}

			// デバイス作成完了。

			this.e出力デバイス = ESoundDeviceType.DirectSound;
			//-----------------
			#endregion

			#region [ 経過時間計測用サウンドバッファを作成し、ループ再生を開始する。]
			//-----------------
			
			// 単位繰り上げ間隔[秒]の長さを持つ無音のサウンドを作成。

			uint nデータサイズbyte = n単位繰り上げ間隔sec * 44100 * 2 * 2;
			var ms = new MemoryStream();
			var bw = new BinaryWriter( ms );
			bw.Write( (uint) 0x46464952 );						// 'RIFF'
			bw.Write( (uint) ( 44 + nデータサイズbyte - 8 ) );	// ファイルサイズ - 8
			bw.Write( (uint) 0x45564157 );						// 'WAVE'
			bw.Write( (uint) 0x20746d66 );						// 'fmt '
			bw.Write( (uint) 16 );								// バイト数
			bw.Write( (ushort) 1 );								// フォーマットID(リニアPCM)
			bw.Write( (ushort) 2 );								// チャンネル数
			bw.Write( (uint) 44100 );							// サンプリング周波数
			bw.Write( (uint) ( 44100 * 2 * 2 ) );				// bytes/sec
			bw.Write( (ushort) ( 2 * 2 ) );						// blockサイズ
			bw.Write( (ushort) 16 );							// bit/sample
			bw.Write( (uint) 0x61746164 );						// 'data'
			bw.Write( (uint) nデータサイズbyte);				// データ長
			for( int i = 0; i < nデータサイズbyte / sizeof(long); i++ )	// PCMデータ
				bw.Write( (long) 0 );
			var byArrWaveFleImage = ms.ToArray();
			bw.Close();
			ms = null;
			bw = null;
			this.sd経過時間計測用サウンドバッファ = this.tサウンドを作成する( byArrWaveFleImage );
			CSound.listインスタンス.Remove( this.sd経過時間計測用サウンドバッファ );	// 特殊用途なのでインスタンスリストからは除外する。

			// サウンドのループ再生開始。

			this.nループ回数 = 0;
			this.n前回の位置 = 0;
			this.sd経過時間計測用サウンドバッファ.DirectSoundBuffer.Play( 0, PlayFlags.Looping );
			this.n前に経過時間を測定したシステム時刻ms = this.tmシステムタイマ.nシステム時刻ms;
			//-----------------
			#endregion

			Trace.TraceInformation( "DirectSound を初期化しました。({0})", ( priority ) ? "Priority" : "Normal" );
		}

		public CSound tサウンドを作成する( string strファイル名 )
		{
			var sound = new CSound();
			sound.tDirectSoundサウンドを作成する( strファイル名, this.DirectSound );
			return sound;
		}
		public CSound tサウンドを作成する( byte[] byArrWAVファイルイメージ )
		{
			var sound = new CSound();
			sound.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, this.DirectSound );
			return sound;
		}
		public CSound tサウンドを作成する( byte[] byArrWAVファイルイメージ, BufferFlags flags )
		{
			var sound = new CSound();
			sound.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, this.DirectSound, flags );
			return sound;
		}

		// 既存のインスタンス（生成直後 or Dispose済み）に対してサウンドを生成する。
		public void tサウンドを作成する( string strファイル名, ref CSound sound )
		{
			sound.tDirectSoundサウンドを作成する( strファイル名, this.DirectSound );
		}
		public void tサウンドを作成する( byte[] byArrWAVファイルイメージ, ref CSound sound )
		{
			sound.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, this.DirectSound );
		}
		public void tサウンドを作成する( byte[] byArrWAVファイルイメージ, BufferFlags flags, ref CSound sound )
		{
			sound.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, this.DirectSound, flags );
		}

		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool bManagedDispose )
		{
			if( bManagedDispose )
			{
				#region [ 経緯時間計測用サウンドバッファを解放。]
				//-----------------
				this.sd経過時間計測用サウンドバッファ.tサウンドを停止する();
				C共通.tDisposeする( ref this.sd経過時間計測用サウンドバッファ );
				//-----------------
				#endregion
				#region [ 単位繰り上げ用スレッド停止。]
				//-----------------
				if( this.th経過時間測定用スレッド != null )
				{
					this.th経過時間測定用スレッド.Abort();
					this.th経過時間測定用スレッド = null;
				
				}
				//-----------------
				#endregion

				C共通.tDisposeする( ref this.DirectSound );
				C共通.tDisposeする( this.tmシステムタイマ );
			}
			this.e出力デバイス = ESoundDeviceType.Unknown;
		}
		~CSoundDeviceDirectSound()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion

		protected DirectSound DirectSound = null;
		protected CSound sd経過時間計測用サウンドバッファ = null;
		protected Thread th経過時間測定用スレッド = null;
		protected AutoResetEvent autoResetEvent = new AutoResetEvent( false );
		protected const uint n単位繰り上げ間隔sec = 1;	// [秒]
		protected const uint n単位繰り上げ間隔ms = n単位繰り上げ間隔sec * 1000;	// [ミリ秒]
		protected int nループ回数 = 0;

		private long n前に経過時間を測定したシステム時刻ms = CTimer.n未使用;
		private int n前回の位置 = 0;
	}
}
