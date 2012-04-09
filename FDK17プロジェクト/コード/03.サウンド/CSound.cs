using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using SlimDX;
using SlimDX.DirectSound;
using SlimDX.Multimedia;

namespace FDK
{
	public class CSound : IDisposable, ICloneable
	{
		// (1) 以下は全子クラスに共通


		// プロパティ

		private SlimDX.DirectSound.DirectSound Device;
		public void setDevice( ref DirectSound Device_ )
		{
			Device = Device_;
		}
		public DirectSound getDevice()
		{
			return Device;
		}

		public SoundBuffer Buffer
		{
			get
			{
				return _Buffer;
			}
			private set
			{
				_Buffer = value;
			}
		}
		public bool bストリーム再生する
		{
			get
			{
				if( this.nHandle < 0 )
				{
					return false;
				}
				return true;
			}
		}
		public bool bループする
		{
			get;
			private set;
		}
		public bool b一時停止中
		{
			get
			{
				return ( this.n一時停止回数 > 0 );
			}
		}
		public bool b再生中
		{
			get
			{
				return ( ( this.Buffer.Status & BufferStatus.Playing ) != BufferStatus.None );
			}
		}
		public double db再生速度
		{
			get
			{
				return this._db再生速度;
			}
			set
			{
				if( this._db再生速度 != value )
				{
					this._db再生速度 = value;
					if( this.b再生中 )
					{
						this.Buffer.Frequency = (int) ( ( this.nオリジナルの周波数 * this._db再生速度 ) * this.db周波数倍率 );
					}
				}
			}
		}
		public double db周波数倍率
		{
			get
			{
				return this._db周波数倍率;
			}
			set
			{
				if( this._db周波数倍率 != value )
				{
					this._db周波数倍率 = value;
					if( this.b再生中 )
					{
						this.Buffer.Frequency = (int) ( ( this.nオリジナルの周波数 * this.db再生速度 ) * this.db周波数倍率 );
					}
				}
			}
		}
		public DateTime dt最終更新時刻
		{
			get; 
			private set;
		}
		public WaveFormat Format
		{ 
			get;
			private set;
		}
		public int nオリジナルの周波数 
		{ 
			get; 
			private set;
		}
		public int nサウンドバッファサイズ
		{
			get
			{
				return this.Buffer.Capabilities.BufferSize;
			}
		}
		public int n位置
		{
			get
			{
				return this._n位置;
			}
			set
			{
				this._n位置 = value;
				if( this._n位置 < -100 )
				{
					this._n位置 = -100;
				}
				else if( this._n位置 > 100 )
				{
					this._n位置 = 100;
				}
				if( this._n位置 == 0 )
				{
					this._n位置db = 0;
				}
				else if( this._n位置 == -100 )
				{
					this._n位置db = -10000;
				}
				else if( this._n位置 == 100 )
				{
					this._n位置db = 0x2710;
				}
				else if( this._n位置 < 0 )
				{
					this._n位置db = (int) ( ( 20.0 * Math.Log10( ( (double) ( this._n位置 + 100 ) ) / 100.0 ) ) * 100.0 );
				}
				else
				{
					this._n位置db = (int) ( ( -20.0 * Math.Log10( ( (double) ( 100 - this._n位置 ) ) / 100.0 ) ) * 100.0 );
				}
				if( this.b再生中 )
				{
					this.Buffer.Pan = this._n位置db;
				}
			}
		}
		public int n音量
		{
			get
			{
				return this._n音量;
			}
			set
			{
				this._n音量 = value;
				if( this._n音量 == 0 )
				{
					this._n音量db = -10000;
				}
				else
				{
					this._n音量db = (int) ( ( 20.0 * Math.Log10( ( (double) this._n音量 ) / 100.0 ) ) * 100.0 );
				}
				if( this.b再生中 )
				{
					this.Buffer.Volume = this._n音量db;
				}
			}
		}
		public int n周波数
		{
			get
			{
				return this.Buffer.Frequency;
			}
			set
			{
				this.Buffer.Frequency = value;
			}
		}
		public int n総演奏時間ms
		{
			get
			{
				return (int) ( ( (double) this.nTotalPCMSize ) / ( this.Buffer.Format.AverageBytesPerSecond * 0.001 ) );
//				return (int) ( ( (double) this.GetTotalPCMSize( this.nHandle ) ) / ( this.Buffer.Format.AverageBytesPerSecond * 0.001 ) );
			}
		}
		public string strファイル名
		{
			get; 
			private set; 
		}


		// コンストラクタ

		public CSound()
		{
			this.Buffer = null;
		}


		// メソッド

		public object Clone()
		{
			CSound clone = (CSound) MemberwiseClone();	// これだけだとCY連打が途切れる＆タイトルに戻る際にNullRef例外発生

			Device.DuplicateSoundBuffer( this._Buffer, out clone._Buffer );

			return clone;
		}

		public int tデコード後のサイズを調べる( string strファイル名, out int _nHandle )
		{
//Trace.TraceInformation("tデコード後のサイズを調べる、を開始。");
			_nHandle = -1;

			#region [ #24416 2011.2.15 yyagi; to avoid application error in case filesize==0 ]
			FileInfo fi = new FileInfo( strファイル名 );
			long filesize = fi.Length;
			if ( filesize <= 0 )
			{
				return -1;
			}
			fi = null;
			#endregion

//			Trace.TraceInformation( "Open(): Enter." );
			int nHandle = this.Open( strファイル名 );
			if( nHandle < 0 )
			{
				return -1;
			}
//			Trace.TraceInformation( "Open(): Exit." );
//Trace.TraceInformation("this.Open()完了。");
			CWin32.WAVEFORMATEX wfx = new CWin32.WAVEFORMATEX();
//			Trace.TraceInformation( "GetFormat(): Enter." );
			if ( this.GetFormat( nHandle, ref wfx ) < 0 )
			{
				this.Close( nHandle );
				return -2;
			}
//			Trace.TraceInformation( "GetFormat(): Exit." );
//			Trace.TraceInformation( "GetTotalPCMSize(): Enter." );
			this.nTotalPCMSize = this.GetTotalPCMSize(nHandle);		// 2012.3.9 yyagi GetTotalPCMSize()の結果をthis.nTotalPCMSizeに保持するよう修正。オンメモリ再生時はnHandleが解放され、#SOUND_NOWLOADINGの再生終了待ちが(再生時間ゼロで扱われてしまって)うまく動かなくなる。
			if (this.nTotalPCMSize == 0)
			{
				this.Close( nHandle );
				return -3;
			}
//			Trace.TraceInformation( "GetTotalPCMSize(): Exit." );
			//this.Close( nHandle );	// 2011.1.2 yyagi ここでClose()しないで、次のオンメモリ/ストリーム読み出しの時に再利用して読み込みを高速化
			_nHandle = nHandle;
			return (int) this.nTotalPCMSize;
		}
		public void tオンメモリ方式で作成する( SlimDX.DirectSound.DirectSound Device, string strファイル名, int _nHandle )
		{
			byte[] buffer = null;
			//int nHandle = this.Open( strファイル名 );	// 2011.1.2 yyagi tデコード後のサイズを調べる()で得たnHandleを使ってOpen()分の読み込み時間を短縮
			int nHandle = _nHandle;
			if( nHandle < 0 )
			{
				throw new Exception( string.Format( "Open() に失敗しました。({0})({1})", nHandle, strファイル名 ) );
			}
			CWin32.WAVEFORMATEX wfx = new CWin32.WAVEFORMATEX();
			if( this.GetFormat( nHandle, ref wfx ) < 0 )
			{
				this.Close( nHandle );
				throw new Exception( string.Format( "GetFormat() に失敗しました。({0})", strファイル名 ) );
			}
			int totalPCMSize = (int) this.nTotalPCMSize;		//  tデコード後のサイズを調べる()で既に取得済みの値を流用する。ms単位の高速化だが、チップ音がたくさんあると塵積で結構効果がある
			// int totalPCMSize = (int) this.GetTotalPCMSize( nHandle );
			if ( totalPCMSize == 0 )
			{
				this.Close( nHandle );
				throw new Exception( string.Format( "GetTotalPCMSize() に失敗しました。({0})", strファイル名 ) );
			}
			totalPCMSize += ( ( totalPCMSize % 2 ) != 0 ) ? 1 : 0;
			buffer = new byte[ totalPCMSize ];
			GCHandle handle = GCHandle.Alloc( buffer, GCHandleType.Pinned );
			try
			{
				if( this.Decode( nHandle, Marshal.UnsafeAddrOfPinnedArrayElement( buffer, 0 ), (uint) totalPCMSize, 0 ) < 0 )
				{
					buffer = null;
					throw new Exception( string.Format( "デコードに失敗しました。({0})", strファイル名 ) );
				}
			}
			finally
			{
				handle.Free();
				this.Close( nHandle );
			}
			WaveFormat format = new WaveFormat();
			format.FormatTag = WaveFormatTag.Pcm;
			format.Channels = (short) wfx.nChannels;
			format.SamplesPerSecond = (int) wfx.nSamplesPerSec;
			format.AverageBytesPerSecond = (int) wfx.nAvgBytesPerSec;
			format.BlockAlignment = (short) wfx.nBlockAlign;
			format.BitsPerSample = (short) wfx.wBitsPerSample;
			this.Format = format;
			SoundBufferDescription description = new SoundBufferDescription();
			description.Format = this.Format;
			description.Flags = BufferFlags.Defer | BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus | BufferFlags.ControlVolume | BufferFlags.ControlPan | BufferFlags.ControlFrequency;
			description.SizeInBytes = totalPCMSize;
			this.Buffer = new SecondarySoundBuffer( Device, description );
			this.Buffer.Write( buffer, 0, LockFlags.None );
			this.strファイル名 = strファイル名;
			this.dt最終更新時刻 = File.GetLastWriteTime( strファイル名 );
			this.nオリジナルの周波数 = this.Buffer.Frequency;
		}
		public void tストリーム方式で作成する( SlimDX.DirectSound.DirectSound Device, string strファイル名, int _nHandle )
		{
//			Trace.TraceInformation( "tストリーム方式で作成: 開始。" );
			//this.nHandle = this.Open( strファイル名 );	// 2011.1.2 yyagi tデコード後のサイズを調べる()で得たnHandleを継続して使って、Open()の処理時間を削減
			this.nHandle = _nHandle;
			if( this.nHandle < 0 )
			{
				throw new Exception( string.Format( "Open() に失敗しました。({0})", strファイル名 ) );
			}
			CWin32.WAVEFORMATEX wfx = new CWin32.WAVEFORMATEX();
			if( this.GetFormat( this.nHandle, ref wfx ) < 0 )
			{
				this.Close( this.nHandle );
				this.nHandle = -1;
				throw new Exception( string.Format( "GetFormat() に失敗しました。({0})", strファイル名 ) );
			}
			WaveFormat format = new WaveFormat();
			format.FormatTag = WaveFormatTag.Pcm;
			format.AverageBytesPerSecond = (int) wfx.nAvgBytesPerSec;
			format.BitsPerSample = (short) wfx.wBitsPerSample;
			format.BlockAlignment = (short) wfx.nBlockAlign;
			format.Channels = (short) wfx.nChannels;
			format.SamplesPerSecond = (int) wfx.nSamplesPerSec;
			this.Format = format;
			this.by中継バッファ = new byte[ CSound管理.nバッファサイズ ];
			this.gch中継バッファ = GCHandle.Alloc( this.by中継バッファ, GCHandleType.Pinned );
			SoundBufferDescription description = new SoundBufferDescription();
			description.Format = this.Format;
			description.Flags = BufferFlags.Defer | BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus | BufferFlags.ControlVolume | BufferFlags.ControlPan | BufferFlags.ControlFrequency;
			description.SizeInBytes = this.by中継バッファ.Length * CSound管理.nバッファの数;
			this.Buffer = new SecondarySoundBuffer( Device, description );
			this.tストリーム再生位置リセット();
			for( int i = 0; i < CSound管理.nバッファの数; i++ )
			{
				this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む( this.Buffer, i, this.bループする );
			}
			this.n現在書き込み許可待ちのバッファ番号 = 0;
			this.strファイル名 = strファイル名;
			this.dt最終更新時刻 = File.GetLastWriteTime( strファイル名 );
			this.nオリジナルの周波数 = this.Buffer.Frequency;
//			Trace.TraceInformation( "tストリーム方式で作成: 完了。" );
		}
		public void t再生を一時停止する()
		{
			if( this.b再生中 )
			{
				this.n一時停止位置byte = this.Buffer.CurrentPlayPosition;
				this.Buffer.Stop();
				this.n一時停止回数++;
			}
		}
		public void t再生を開始する()
		{
			this.t再生を開始する( false );
		}
		public void t再生を開始する( bool bループする )
		{
			this.Buffer.Stop();
			this.Buffer.CurrentPlayPosition = 0;
			this.Buffer.Frequency = ( ( this.db再生速度 != 1.0 ) || ( this.db周波数倍率 != 1.0 ) ) ? ( (int) ( ( this.nオリジナルの周波数 * this.db再生速度 ) * this.db周波数倍率 ) ) : this.nオリジナルの周波数;
			this.Buffer.Volume = this._n音量db;
			this.Buffer.Pan = this._n位置db;
			try
			{
//				this.bループする = bループする;			// #23575 2010.12.18 yyagi:
														// これで大抵のプレビューはループするようになるが、
														// おばたけメドレー(riff chunked mp3)はAPエラーになるので注意
				this.Buffer.Play(0, (this.bストリーム再生する || bループする) ? PlayFlags.Looping : PlayFlags.None);
			}
			catch( OutOfMemoryException )
			{
				Trace.TraceError( "サウンドを再生するために必要なメモリ領域が不足しています。" );
			}
			this.n一時停止回数 = 0;
		}
		public void t再生を再開する()
		{
			if( this.n一時停止回数 > 0 )
			{
				this.n一時停止回数--;
				if( this.n一時停止回数 <= 0 )
				{
					this.Buffer.CurrentPlayPosition = this.n一時停止位置byte;
					this.Buffer.Frequency = ( ( this.db再生速度 != 1.0 ) || ( this.db周波数倍率 != 1.0 ) ) ? ( (int) ( ( this.nオリジナルの周波数 * this.db再生速度 ) * this.db周波数倍率 ) ) : this.nオリジナルの周波数;
					this.Buffer.Volume = this._n音量db;
					this.Buffer.Pan = this._n位置db;
					try
					{
						this.Buffer.Play( 0, ( this.bストリーム再生する || this.bループする ) ? PlayFlags.Looping : PlayFlags.None );
					}
					catch( OutOfMemoryException )
					{
						Trace.TraceError( "サウンドを再生するために必要なメモリ領域が不足しています。" );
					}
				}
			}
		}
		public void t再生を再開する( long n再生開始位置ms )
		{
			this.t再生位置を変更する( this.t時刻から位置を返す( n再生開始位置ms ) );
			this.t再生を再開する();
		}
		public void t再生を停止する()
		{
			this.Buffer.Stop();
			this.Buffer.CurrentPlayPosition = 0;
			this.n一時停止回数 = 0;
			if( this.bストリーム再生する )
			{
				this.tストリーム再生位置リセット();
				for( int i = 0; i < CSound管理.nバッファの数; i++ )
				{
					this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む( this.Buffer, i, this.bループする );
				}
				this.n現在書き込み許可待ちのバッファ番号 = 0;
			}
		}
		public void t再生位置を変更する( int n新しい再生位置sample )
		{
			if (this.nHandle >= 0)
			{
				int num = n新しい再生位置sample * this.Buffer.Format.BlockAlignment;
				if( this.bストリーム再生する )
				{
					this.t再生中の処理をする();
					int num2 = this.n現在のPCM側の位置byte - ( CSound管理.nバッファサイズ * CSound管理.nバッファの数 );
					if( num2 < 0 )
					{
						num2 = 0;
					}
					int num3 = this.n現在のPCM側の位置byte - 1;
					if( num3 < 0 )
					{
						num3 = 0;
					}
					if( ( num >= num2 ) && ( num <= num3 ) )
					{
						this.Buffer.CurrentPlayPosition = num % ( CSound管理.nバッファサイズ * CSound管理.nバッファの数 );
					}
					else
					{
						this.Seek( this.nHandle, (uint) num );
						for( int i = 0; i < CSound管理.nバッファの数; i++ )
						{
							this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む( this.Buffer, i, this.bループする );
						}
						this.n現在書き込み許可待ちのバッファ番号 = 0;
						this.Buffer.CurrentPlayPosition = 0;
					}
				}
				else if( ( num >= 0 ) || ( num < this.nサウンドバッファサイズ ) )
				{
					this.Buffer.CurrentPlayPosition = num;
				}
			}
		}
		public void t再生中の処理をする()
		{
			if( ( this.bストリーム再生する && ( this.Buffer != null ) ) && ( this.b再生中 && ( this.n一時停止回数 <= 0 ) ) )
			{
//Trace.TraceInformation("CurrentPosition={0}", this.Buffer.CurrentPlayPosition);
				int n現在再生中のバッファ番号 = this.Buffer.CurrentPlayPosition / this.by中継バッファ.Length;
				int num3 = n現在再生中のバッファ番号 - CSound管理.nキープする再生済みバッファの数;
//Trace.TraceInformation("A{0},{1},{2}",
//	this.Buffer.CurrentPlayPosition,
//	n現在書き込み許可待ちのバッファ番号,
//	n現在再生中のバッファ番号);
				if (this.n現在書き込み許可待ちのバッファ番号 < n現在再生中のバッファ番号)
				{
					while (this.n現在書き込み許可待ちのバッファ番号 < num3)
					{
						this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む(this.Buffer, this.n現在書き込み許可待ちのバッファ番号, this.bループする);
						this.n現在書き込み許可待ちのバッファ番号++;
					}
				}
				else if( this.n現在書き込み許可待ちのバッファ番号 > n現在再生中のバッファ番号 )
				{
					num3 = (num3 + CSound管理.nバッファの数) % CSound管理.nバッファの数;
					if( this.n現在書き込み許可待ちのバッファ番号 < num3 )
					{
						while (this.n現在書き込み許可待ちのバッファ番号 < num3)
						{
							this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む(this.Buffer, this.n現在書き込み許可待ちのバッファ番号, this.bループする);
							this.n現在書き込み許可待ちのバッファ番号++;
						}
					}
					else if( this.n現在書き込み許可待ちのバッファ番号 > num3 )
					{
						while (this.n現在書き込み許可待ちのバッファ番号 < CSound管理.nバッファの数)
						{
							this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む(this.Buffer, this.n現在書き込み許可待ちのバッファ番号, this.bループする);
							this.n現在書き込み許可待ちのバッファ番号++;
						}
						this.n現在書き込み許可待ちのバッファ番号 = 0;
						while( this.n現在書き込み許可待ちのバッファ番号 < num3 )
						{
							this.tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む(this.Buffer, this.n現在書き込み許可待ちのバッファ番号, this.bループする);
							this.n現在書き込み許可待ちのバッファ番号++;
						}
					}
				}
			}
		}
		public int t時刻から位置を返す( long n時刻 )
		{
			double num = ( n時刻 * this.db再生速度 ) * this.db周波数倍率;
			return (int) ( ( num * 0.001 ) * this.Buffer.Format.SamplesPerSecond );
		}

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDispose完了済み )
			{
				if( this.Buffer != null )
				{
					this.Buffer.Stop();
					this.Buffer.Dispose();
					this.Buffer = null;
				}
				if( this.gch中継バッファ.IsAllocated )
				{
					this.gch中継バッファ.Free();
				}
				if( this.by中継バッファ != null )
				{
					this.by中継バッファ = null;
				}
				if( this.nHandle >= 0 )
				{
					this.Close( this.nHandle );
				}
				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion



		// (2) 以下は各子クラスごとに実装する

		protected virtual int Open( string filename )
		{
			throw new NotImplementedException();
		}
		protected virtual int GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx )
		{
			throw new NotImplementedException();
		}
		protected virtual uint GetTotalPCMSize( int nHandle )
		{
			throw new NotImplementedException();
		}
		protected virtual int Seek( int nHandle, uint dwPosition )
		{
			throw new NotImplementedException();
		}
		protected virtual int Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop )
		{
			throw new NotImplementedException();
		}
		protected virtual void Close( int nHandle )
		{
			throw new NotImplementedException();
		}


		// その他

		#region [ private ]
		//-----------------
		private double _db再生速度 = 1.0;
		private double _db周波数倍率 = 1.0;
		private int _n位置;
		private int _n位置db;
		private int _n音量 = 100;
		private int _n音量db;
		private bool bDispose完了済み;
		private byte[] by中継バッファ;
		private GCHandle gch中継バッファ;
		private int nHandle = -1;
		private int n一時停止位置byte;
		private int n一時停止回数;
		private int n現在のPCM側の位置byte;
		private int n現在書き込み許可待ちのバッファ番号;
		private SoundBuffer _Buffer;
		private uint nTotalPCMSize = 0;

		private void tストリーム再生位置リセット()
		{
			if( this.nHandle != -1 )
			{
				this.Seek( this.nHandle, 0 );
				this.n現在のPCM側の位置byte = 0;
			}
		}
		private void tデコーダの現在の読み出し位置から1バッファ分のPCMを指定されたバッファに書き込む( SoundBuffer buffer, int n書込先バッファ番号, bool bPCMの末尾に達したら先頭に戻る )
		{
			if( !this.bDispose完了済み && ( this.nHandle >= 0 ) )
			{
				this.Decode(
					this.nHandle,
					Marshal.UnsafeAddrOfPinnedArrayElement( this.by中継バッファ, 0 ),
					(uint) this.by中継バッファ.Length,
					bPCMの末尾に達したら先頭に戻る ? 1 : 0
				);
				this.n現在のPCM側の位置byte += this.by中継バッファ.Length;
				if (bPCMの末尾に達したら先頭に戻る)
			//	if (true)
				{
					this.n現在のPCM側の位置byte = (int) ( this.n現在のPCM側の位置byte % this.GetTotalPCMSize( this.nHandle ) );
//Trace.TraceInformation("現在位置={0}", this.n現在のPCM側の位置byte);
				}
				this.Buffer.Write( this.by中継バッファ, n書込先バッファ番号 * this.by中継バッファ.Length, LockFlags.None );
			}
		}
		//-----------------
		#endregion
	}
}