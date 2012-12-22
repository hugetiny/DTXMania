using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
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
	#region [ DTXMania用拡張 ]
	public class CSound管理	// : CSound
	{
		//private static ISoundDevice _SoundDevice;
		//private static ESoundDeviceType _SoundDeviceType = ESoundDeviceType.Unknown;
		public static ISoundDevice SoundDevice
		{
			get; set;
			//get
			//{
			//    return _SoundDevice;
			//}
			//set
			//{
			//    _SoundDevice = value;
			//}
		}
		public static ESoundDeviceType SoundDeviceType
		{
			get; set;
			//get
			//{
			//    return _SoundDeviceType;
			//}
			//set
			//{
			//    _SoundDeviceType = value;
			//}
		}
		public static CSoundTimer rc演奏用タイマ = null;

		public static IntPtr WindowHandle;

		#region [ WASAPI/ASIO/DirectSound設定値 ]
		/// <summary>
		/// <para>WASAPI 排他モード出力における再生遅延[ms]（の希望値）。最終的にはこの数値を基にドライバが決定する）。</para>
		/// </summary>
		public static int SoundDelayExclusiveWASAPI = 50;
		/// <summary>
		/// <para>WASAPI 共有モード出力における再生遅延[ms]。ユーザが決定する。</para>
		/// </summary>
		public static int SoundDelaySharedWASAPI = 100;
		/// <summary>
		/// <para>排他WASAPIバッファの更新間隔。出力間隔ではないので注意。</para>
		/// <para>SoundDelay よりも小さい値であること。（小さすぎる場合はBASSによって自動修正される。）</para>
		/// </summary>
		public static int SoundUpdatePeriodExclusiveWASAPI = 6;
		/// <summary>
		/// <para>共有WASAPIバッファの更新間隔。出力間隔ではないので注意。</para>
		/// <para>SoundDelay よりも小さい値であること。（小さすぎる場合はBASSによって自動修正される。）</para>
		/// </summary>
		public static int SoundUpdatePeriodSharedWASAPI = 6;
		/// <summary>
		/// <para>ASIO 出力における再生遅延[ms]（の希望値）。最終的にはこの数値を基にドライバが決定する）。</para>
		/// </summary>
		public static int SoundDelayASIO = 50;
		/// <summary>
		/// <para>DirectSound 出力における再生遅延[ms]。ユーザが決定する。</para>
		/// </summary>
		public static int SoundDelayDirectSound = 100;
		#endregion


	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="handle"></param>
		public CSound管理( IntPtr handle )
		{
			WindowHandle = handle;
			t初期化();
		}
		public void Dispose()
		{
			t終了();
		}

		public static void t初期化()
		{
			SoundDevice = null;							// ユーザ依存
			rc演奏用タイマ = null;				// Global.Bass 依存（つまりユーザ依存）

			//SoundDeviceType = ESoundDeviceType.DirectSound;
			SoundDeviceType = ESoundDeviceType.ExclusiveWASAPI;
			//SoundDeviceType = ESoundDeviceType.ASIO;
			t現在のユーザConfigに従ってサウンドデバイスとすべての既存サウンドを再構築する();
		}
		public static void t終了()
		{
			C共通.tDisposeする( SoundDevice ); SoundDevice = null;
			C共通.tDisposeする( ref rc演奏用タイマ );	// Global.Bass を解放した後に解放すること。（Global.Bass で参照されているため）
		}


		public static void t現在のユーザConfigに従ってサウンドデバイスとすべての既存サウンドを再構築する()
		{
			#region [ すでにサウンドデバイスと演奏タイマが構築されていれば解放する。]
			//-----------------
			if ( SoundDevice != null )
			{
				// すでに生成済みのサウンドがあれば初期状態に戻す。

				CSound.tすべてのサウンドを初期状態に戻す();


				// サウンドデバイスと演奏タイマを解放する。

				C共通.tDisposeする( SoundDevice ); SoundDevice = null;
				C共通.tDisposeする( ref rc演奏用タイマ );	// Global.SoundDevice を解放した後に解放すること。（Global.SoundDevice で参照されているため）
			}
			//-----------------
			#endregion

			#region [ 新しいサウンドデバイスを構築する。]
			//-----------------
			switch ( SoundDeviceType )
			{
				case ESoundDeviceType.ExclusiveWASAPI:
					SoundDevice = new CSoundDeviceWASAPI( CSoundDeviceWASAPI.Eデバイスモード.排他, SoundDelayExclusiveWASAPI, SoundUpdatePeriodExclusiveWASAPI );
					break;

				case ESoundDeviceType.SharedWASAPI:
					SoundDevice = new CSoundDeviceWASAPI( CSoundDeviceWASAPI.Eデバイスモード.共有, SoundDelaySharedWASAPI, SoundUpdatePeriodSharedWASAPI );
					break;

				case ESoundDeviceType.ASIO:
					SoundDevice = new CSoundDeviceASIO( SoundDelayASIO );
					break;

				case ESoundDeviceType.DirectSound:
					SoundDevice = new CSoundDeviceDirectSound( WindowHandle, SoundDelayDirectSound );
					break;

				default:
					throw new Exception( string.Format( "未対応の SoundDeviceType です。[{0}]", SoundDeviceType.ToString() ) );
			}
			//-----------------
			#endregion
			#region [ 新しい演奏タイマを構築する。]
			//-----------------
			rc演奏用タイマ = new CSoundTimer( SoundDevice );
			//-----------------
			#endregion

			CSound.tすべてのサウンドを再構築する( SoundDevice );		// すでに生成済みのサウンドがあれば作り直す。
		}
		public CSound tサウンドを生成する( string filename )
		{
Debug.WriteLine( "★★tサウンドを生成する()" + SoundDevice.e出力デバイス + " " + Path.GetFileName( filename ) );
			if ( SoundDeviceType == ESoundDeviceType.Unknown )
			{
				throw new Exception( string.Format( "未対応の SoundDeviceType です。[{0}]", SoundDeviceType.ToString() ) );
			}
			return SoundDevice.tサウンドを作成する( filename );
		}

		public void t再生中の処理をする()
		{
//★★★★★★★★★★★★★★★★★★★★★ダミー★★★★★★★★★★★★★★★★★★
//			Debug.Write( "再生中の処理をする()" );
		}

		public void tサウンドを破棄する( CSound csound )
		{
			csound.t解放する();
			csound = null;
		}
	}
	#endregion

	// CSound は、サウンドデバイスが変更されたときも、インスタンスを再作成することなく、新しいデバイスで作り直せる必要がある。
	// そのため、デバイスごとに別のクラスに分割するのではなく、１つのクラスに集約するものとする。

	public class CSound : IDisposable
	{
		#region [ DTXMania用拡張 ]
		public int n総演奏時間ms
		{
			get;
			private set;
		}
		public int nサウンドバッファサイズ		// 取りあえず0固定★★★★★★★★★★★★★★★★★★★★
		{
			get { return 0; }
		}
		public bool bストリーム再生する			// 取りあえずfalse固定★★★★★★★★★★★★★★★★★★★★
												// trueにすると同一チップ音の多重再生で問題が出る(4POLY音源として動かない)
		{
			get { return false; }
		}
		public double db周波数倍率;
		public double db再生速度;
		#endregion


		private STREAMPROC _myStreamCreate;  // make it global, so that the GC can not remove it

		/// <summary>
		/// <para>0:最小～100:原音</para>
		/// </summary>
		public int n音量
		{
			get
			{
				if( this.bBASSサウンドである )
				{
					float f音量 = 0.0f;
					if( BassMix.BASS_Mixer_ChannelGetEnvelopePos( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, ref f音量 ) == -1 )
						return 100;
					return (int) ( f音量 * 100 );
				}
				else if( this.bDirectSoundである )
				{
					return this._n音量;
				}
				return -1;
			}
			set
			{
				if( this.bBASSサウンドである )
				{
					float f音量 = Math.Min( Math.Max( value, 0 ), 100 ) / 100.0f;	// 0～100 → 0.0～1.0
					var nodes = new BASS_MIXER_NODE[ 1 ] { new BASS_MIXER_NODE( 0, f音量 ) };
					BassMix.BASS_Mixer_ChannelSetEnvelope( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, nodes );
				}
				else if( this.bDirectSoundである )
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

					this.Buffer.Volume = this._n音量db;
				}
			}
		}

		/// <summary>
		/// <para>左:-100～中央:0～100:右。set のみ。</para>
		/// </summary>
		public int n位置
		{
			get
			{
				if( this.bBASSサウンドである )
				{
					float f位置 = 0.0f;
					if( BassMix.BASS_Mixer_ChannelGetEnvelopePos( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_PAN, ref f位置 ) == -1 )
						return 0;
					return (int) ( f位置 * 100 );
				}
				else if( this.bDirectSoundである )
				{
					return this._n位置;
				}
				return -9999;
			}
			set
			{
				if( this.bBASSサウンドである )
				{
					float f位置 = Math.Min( Math.Max( value, -100 ), 100 ) / 100.0f;	// -100～100 → -1.0～1.0
					var nodes = new BASS_MIXER_NODE[ 1 ] { new BASS_MIXER_NODE( 0, f位置 ) };
					BassMix.BASS_Mixer_ChannelSetEnvelope( this.hBassStream, BASSMIXEnvelope.BASS_MIXER_ENV_PAN, nodes );
				}
				else if( this.bDirectSoundである )
				{
					this._n位置 = Math.Min( Math.Max( -100, value ), 100 );		// -100～100

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
						this._n位置db = 100000;
					}
					else if( this._n位置 < 0 )
					{
						this._n位置db = (int) ( ( 20.0 * Math.Log10( ( (double) ( this._n位置 + 100 ) ) / 100.0 ) ) * 100.0 );
					}
					else
					{
						this._n位置db = (int) ( ( -20.0 * Math.Log10( ( (double) ( 100 - this._n位置 ) ) / 100.0 ) ) * 100.0 );
					}

					this.Buffer.Pan = this._n位置db;
				}
			}
		}

		/// <summary>
		/// <para>DirectSoundのセカンダリバッファ。</para>
		/// </summary>
		public SecondarySoundBuffer DirectSoundBuffer
		{
			get { return this.Buffer; }
		}

		/// <summary>
		/// <para>DirectSoundのセカンダリバッファ作成時のフラグ。</para>
		/// </summary>
		public BufferFlags DirectSoundBufferFlags
		{
			get;
			protected set;
		}

		/// <summary>
		/// <para>全インスタンスリスト。</para>
		/// <para>～を作成する() で追加され、t解放する() or Dispose() で解放される。</para>
		/// </summary>
		public static List<CSound> listインスタンス = new List<CSound>();

		public CSound()
		{
			this.n音量 = 100;
			this.n位置 = 0;
			this.db周波数倍率 = 1.0;
			this.db再生速度 = 1.0;
			this.DirectSoundBufferFlags = CSoundDeviceDirectSound.DefaultFlags;
		}

		public void tASIOサウンドを作成する( string strファイル名, int hMixer )
		{
			this.tBASSサウンドを作成する( strファイル名, hMixer, BASSFlag.BASS_STREAM_DECODE );
			this.eデバイス種別 = ESoundDeviceType.ASIO;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
		}
		public void tASIOサウンドを作成する( byte[] byArrWAVファイルイメージ, int hMixer )
		{
			this.tBASSサウンドを作成する( byArrWAVファイルイメージ, hMixer, BASSFlag.BASS_STREAM_DECODE );
			this.eデバイス種別 = ESoundDeviceType.ASIO;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
		}
		public void tWASAPIサウンドを作成する( string strファイル名, int hMixer, ESoundDeviceType eデバイス種別 )
		{
			this.tBASSサウンドを作成する( strファイル名, hMixer, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT );
			this.eデバイス種別 = eデバイス種別;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
		}
		public void tWASAPIサウンドを作成する( byte[] byArrWAVファイルイメージ, int hMixer, ESoundDeviceType eデバイス種別 )
		{
			this.tBASSサウンドを作成する( byArrWAVファイルイメージ, hMixer, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT );
			this.eデバイス種別 = eデバイス種別;		// 作成後に設定する。（作成に失敗してると例外発出されてここは実行されない）
		}
		public void tDirectSoundサウンドを作成する( string strファイル名, DirectSound DirectSound )
		{
			this.e作成方法 = E作成方法.ファイルから;
			this.strファイル名 = strファイル名;


			// すべてのファイルを DirectShow でデコードすると時間がかかるので、ファイルが WAV かつ PCM フォーマットでない場合のみ DirectShow でデコードする。

			byte[] byArrWAVファイルイメージ;
			bool bファイルがWAVかつPCMフォーマットである = true;
			
			#region [ ファイルがWAVかつPCMフォーマットか否か調べる。]
			//-----------------
			try
			{
				using( var ws = new WaveStream( strファイル名 ) )
				{
					if( ws.Format.FormatTag != WaveFormatTag.Pcm )
						bファイルがWAVかつPCMフォーマットである = false;
				}
			}
			catch
			{
				bファイルがWAVかつPCMフォーマットである = false;
			}
			//-----------------
			#endregion

			if( bファイルがWAVかつPCMフォーマットである )
			{
				#region [ ファイルを読み込んで byArrWAVファイルイメージへ格納。]
				//-----------------
				var fs = File.Open( strファイル名, FileMode.Open, FileAccess.Read );
				var br = new BinaryReader( fs );

				byArrWAVファイルイメージ = new byte[ fs.Length ];
				br.Read( byArrWAVファイルイメージ, 0, (int) fs.Length );

				br.Close();
				fs.Close();
				//-----------------
				#endregion
			}
			else
			{
				#region [ DirectShow でデコード変換し、 byArrWAVファイルイメージへ格納。]
				//-----------------
				CDStoWAVFileImage.t変換( strファイル名, out byArrWAVファイルイメージ );
				//-----------------
				#endregion
			}


			// あとはあちらで。

			this.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, DirectSound );
		}
		public void tDirectSoundサウンドを作成する( byte[] byArrWAVファイルイメージ, DirectSound DirectSound )
		{
			this.tDirectSoundサウンドを作成する( byArrWAVファイルイメージ, DirectSound, CSoundDeviceDirectSound.DefaultFlags );
		}
		public void tDirectSoundサウンドを作成する( byte[] byArrWAVファイルイメージ, DirectSound DirectSound, BufferFlags flags )
		{
			if( this.e作成方法 == E作成方法.Unknown )
				this.e作成方法 = E作成方法.WAVファイルイメージから;

			WaveFormat wfx = null;
			int nPCMデータの先頭インデックス = -1;
			int nPCMサイズbyte = -1;

			#region [ byArrWAVファイルイメージ[] から上記３つのデータを取得。]
			//-----------------
			var ms = new MemoryStream( byArrWAVファイルイメージ );
			var br = new BinaryReader( ms );

			try
			{
				// 'RIFF'＋RIFFデータサイズ

				if( br.ReadUInt32() != 0x46464952 )
					throw new InvalidDataException( "RIFFファイルではありません。" );
				br.ReadInt32();

				// 'WAVE'
				if( br.ReadUInt32() != 0x45564157 )
					throw new InvalidDataException( "WAVEファイルではありません。" );

				// チャンク
				while( ( ms.Position + 8 ) < ms.Length )	// +8 は、チャンク名＋チャンクサイズ。残り8バイト未満ならループ終了。
				{
					uint chunkName = br.ReadUInt32();

					// 'fmt '
					if( chunkName == 0x20746D66 )
					{
						long chunkSize = (long) br.ReadUInt32();

						var tag = (WaveFormatTag) br.ReadUInt16();

						if( tag == WaveFormatTag.Pcm ) wfx = new WaveFormat();
						else if( tag == WaveFormatTag.Extensible ) wfx = new SlimDX.Multimedia.WaveFormatExtensible();	// このクラスは WaveFormat を継承している。
						else
							throw new InvalidDataException( string.Format( "未対応のWAVEフォーマットタグです。(Tag:{0})", tag.ToString() ) );

						wfx.FormatTag = tag;
						wfx.Channels = br.ReadInt16();
						wfx.SamplesPerSecond = br.ReadInt32();
						wfx.AverageBytesPerSecond = br.ReadInt32();
						wfx.BlockAlignment = br.ReadInt16();
						wfx.BitsPerSample = br.ReadInt16();

						long nフォーマットサイズbyte = 16;

						if( wfx.FormatTag == WaveFormatTag.Extensible )
						{
							br.ReadUInt16();	// 拡張領域サイズbyte
							var wfxEx = (SlimDX.Multimedia.WaveFormatExtensible) wfx;
							wfxEx.ValidBitsPerSample = br.ReadInt16();
							wfxEx.ChannelMask = (Speakers) br.ReadInt32();
							wfxEx.SubFormat = new Guid( br.ReadBytes( 16 ) );	// GUID は 16byte (128bit)

							nフォーマットサイズbyte += 24;
						}

						ms.Seek( chunkSize - nフォーマットサイズbyte, SeekOrigin.Current );
						continue;
					}

					// 'data'
					else if( chunkName == 0x61746164 )
					{
						nPCMサイズbyte = br.ReadInt32();
						nPCMデータの先頭インデックス = (int) ms.Position;

						ms.Seek( nPCMサイズbyte, SeekOrigin.Current );
						continue;
					}

					// その他
					else
					{
						long chunkSize = (long) br.ReadUInt32();
						ms.Seek( chunkSize, SeekOrigin.Current );
						continue;
					}
				}

				if( wfx == null )
					throw new InvalidDataException( "fmt チャンクが存在しません。不正なサウンドデータです。" );
				if( nPCMサイズbyte < 0 )
					throw new InvalidDataException( "data チャンクが存在しません。不正なサウンドデータです。" );
			}
			finally
			{
				ms.Close();
				br.Close();
			}
			//-----------------
			#endregion


			// セカンダリバッファを作成し、PCMデータを書き込む。

			this.Buffer = new SecondarySoundBuffer( DirectSound, new SoundBufferDescription() {
				Format = ( wfx.FormatTag == WaveFormatTag.Pcm) ? wfx : (SlimDX.Multimedia.WaveFormatExtensible) wfx,
				Flags = flags,
				SizeInBytes = nPCMサイズbyte,
			} );
			this.Buffer.Write( byArrWAVファイルイメージ, nPCMデータの先頭インデックス, nPCMサイズbyte, 0, LockFlags.None );

			// DTXMania用に追加
			n総演奏時間ms = (int) ( ( (double) nPCMサイズbyte ) / ( this.Buffer.Format.AverageBytesPerSecond * 0.001 ) );
			
			// 作成完了。

			this.eデバイス種別 = ESoundDeviceType.DirectSound;
			this.DirectSoundBufferFlags = flags;
			this.byArrWAVファイルイメージ = byArrWAVファイルイメージ;


			// インスタンスリストに登録。

			CSound.listインスタンス.Add( this );
		}

		#region [ DTXMania用の変換 ]
		public void tサウンドを破棄する( CSound cs )
		{
			cs.t解放する();
		}
		public void t再生を開始する()
		{
			t再生位置を先頭に戻す();
			tサウンドを再生する();
		}
		public void t再生を開始する( bool bループする )
		{
			if ( bループする )
			{
				Bass.BASS_ChannelFlags( this.hBassStream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP );
			}
			else
			{
				Bass.BASS_ChannelFlags( this.hBassStream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT );
			}
			t再生位置を先頭に戻す();
			tサウンドを再生する();
		}
		public void t再生を停止する()
		{
			tサウンドを停止する();
			t再生位置を先頭に戻す();
		}
		public void t再生を一時停止する()
		{
			tサウンドを停止する();
		}
		public void t再生を再開する( long t )	// ★★★★★★★★★★★★★★★★★★★★★★★★★★★★
		{
			tサウンドを再生する();
		}
		public bool b一時停止中
		{
			get { return ( BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) == BASSActive.BASS_ACTIVE_PAUSED ); }
		}
		public bool b再生中
		{
			get
			{
				if ( this.eデバイス種別 == ESoundDeviceType.DirectSound )
				{
					return ( ( this.Buffer.Status & BufferStatus.Playing ) != BufferStatus.None );
				}
				else
				{
					// 基本的にはBASS_ACTIVE_PLAYINGなら再生中だが、最後まで再生しきったchannelも
					// BASS_ACTIVE_PLAYINGのままになっているので、小細工が必要。
					bool ret = ( BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) == BASSActive.BASS_ACTIVE_PLAYING );
					if ( BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) >= nBytes )
					{
						ret = false;
					}
					return ret;
				}
			}
		}
		#endregion


		public void t解放する()
		{
			bool bManagedも解放する = true;
			bool bインスタンス削除 = false;								// インスタンスは存続する。
			this.Dispose( bManagedも解放する, bインスタンス削除 );
		}
		public void tサウンドを再生する()
		{
//Debug.WriteLine( "tサウンドを再生する(): " + this.strファイル名 );
			if( this.bBASSサウンドである )
			{
Debug.WriteLine( "再生中?: " +  System.IO.Path.GetFileName(this.strファイル名) + " status=" + BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) + " current=" + BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) + " nBytes=" + nBytes );
				BassMix.BASS_Mixer_ChannelPlay( this.hBassStream );
			}
			else if( this.bDirectSoundである )
			{
				this.Buffer.Play( 0, PlayFlags.None );
			}
		}
		public void tサウンドを先頭から再生する()
		{
			this.t再生位置を先頭に戻す();
			this.tサウンドを再生する();
		}
		public void tサウンドを停止する()
		{
			if( this.bBASSサウンドである )
			{
Debug.WriteLine( "停止: " + System.IO.Path.GetFileName( this.strファイル名 ) + " status=" + BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) + " current=" + BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) + " nBytes=" + nBytes );
				BassMix.BASS_Mixer_ChannelPause( this.hBassStream );
			}
			else if( this.bDirectSoundである )
			{
				this.Buffer.Stop();
			}
		}
		
		public void t再生位置を先頭に戻す()
		{
			if( this.bBASSサウンドである )
			{
				BassMix.BASS_Mixer_ChannelSetPosition( this.hBassStream, 0 );
				pos = 0;
			}
			else if( this.bDirectSoundである )
			{
				this.Buffer.CurrentPlayPosition = 0;
			}
		}
		public void t再生位置を変更する( long n位置ms )
		{
			if( this.bBASSサウンドである )
			{
				BassMix.BASS_Mixer_ChannelSetPosition( this.hBassStream, Bass.BASS_ChannelSeconds2Bytes( this.hBassStream, n位置ms / 1000.0 ), BASSMode.BASS_POS_BYTES );
			}
			else if( this.bDirectSoundである )
			{
				int n位置sample = (int) ( this.Buffer.Format.SamplesPerSecond * n位置ms * 0.001 );
				this.Buffer.CurrentPlayPosition = n位置sample * this.Buffer.Format.BlockAlignment;
			}
		}

		public static void tすべてのサウンドを初期状態に戻す()
		{
			foreach( var sound in CSound.listインスタンス )
				sound.t解放する();
		}
		public static void tすべてのサウンドを再構築する( ISoundDevice device )
		{
			if( CSound.listインスタンス.Count == 0 )
				return;


			// サウンドを再生する際にインスタンスリストも更新されるので、配列にコピーを取っておき、リストはクリアする。

			var sounds = CSound.listインスタンス.ToArray();
			CSound.listインスタンス.Clear();
			

			// 配列に基づいて個々のサウンドを作成する。

			for( int i = 0; i < sounds.Length; i++ )
			{
				switch( sounds[ i ].e作成方法 )
				{
					case E作成方法.ファイルから:
						string strファイル名 = sounds[ i ].strファイル名;
						sounds[ i ].Dispose( true, false );
						device.tサウンドを作成する( strファイル名, ref sounds[ i ] );
						break;

					case E作成方法.WAVファイルイメージから:
						if( sounds[ i ].bBASSサウンドである )
						{
							byte[] byArrWaveファイルイメージ = sounds[ i ].byArrWAVファイルイメージ;
							sounds[ i ].Dispose( true, false );
							device.tサウンドを作成する( byArrWaveファイルイメージ, ref sounds[ i ] );
						}
						else if( sounds[ i ].bDirectSoundである )
						{
							byte[] byArrWaveファイルイメージ = sounds[ i ].byArrWAVファイルイメージ;
							var flags = sounds[ i ].DirectSoundBufferFlags;
							sounds[ i ].Dispose( true, false );
							( (CSoundDeviceDirectSound) device ).tサウンドを作成する( byArrWaveファイルイメージ, flags, ref sounds[ i ] );
						}
						break;
				}
			}
		}

		#region [ Dispose-Finalizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true, true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool bManagedも解放する, bool bインスタンス削除 )
		{
			if( this.bBASSサウンドである )
			{
				#region [ ASIO, WASAPI の解放 ]
				//-----------------
				BassMix.BASS_Mixer_ChannelRemove( this.hBassStream );
				Bass.BASS_StreamFree( this.hBassStream );
				this.hBassStream = -1;
				//-----------------
				#endregion
			}

			if( bManagedも解放する )
			{
				if( this.eデバイス種別 == ESoundDeviceType.DirectSound )
				{
					#region [ DirectSound の解放 ]
					//-----------------
					if( this.Buffer != null )
					{
						try
						{
							this.Buffer.Stop();
						}
						catch
						{
							// 演奏終了後、長時間解放しないでいると、たまに AccessViolationException が発生することがある。
						}
						C共通.tDisposeする( ref this.Buffer );
					}
					//-----------------
					#endregion
				}

				if( this.e作成方法 == E作成方法.WAVファイルイメージから &&
					this.eデバイス種別 != ESoundDeviceType.DirectSound )	// DirectSound は hGC 未使用。
				{
					this.hGC.Free();
					this.hGC = default( GCHandle );
				}

				if( bインスタンス削除 )
					CSound.listインスタンス.Remove( this );
			}
		}
		~CSound()
		{
			this.Dispose( false, true );
		}
		//-----------------
		#endregion

		#region [ protected ]
		//-----------------
		protected enum E作成方法 { ファイルから, WAVファイルイメージから, Unknown }
		protected E作成方法 e作成方法 = E作成方法.Unknown;
		protected ESoundDeviceType eデバイス種別 = ESoundDeviceType.Unknown;
		protected string strファイル名 = null;
		protected byte[] byArrWAVファイルイメージ = null;	// WAVファイルイメージ、もしくはchunkのDATA部のみ
		protected GCHandle hGC;
		protected int hBassStream = -1;					// ASIO, WASAPI 用
		protected SecondarySoundBuffer Buffer = null;	// DirectSound 用
		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private bool bDirectSoundである
		{
			get { return ( this.eデバイス種別 == ESoundDeviceType.DirectSound ); }
		}
		private bool bBASSサウンドである
		{
			get
			{
				return (
					this.eデバイス種別 == ESoundDeviceType.ASIO ||
					this.eデバイス種別 == ESoundDeviceType.ExclusiveWASAPI ||
					this.eデバイス種別 == ESoundDeviceType.SharedWASAPI );
			}
		}
		private int _n位置 = 0;
		private int _n位置db;
		private int _n音量 = 100;
		private int _n音量db;
		private long nBytes = 0;

		private void tBASSサウンドを作成する( string strファイル名, int hMixer, BASSFlag flags )
		{
			if ( String.Compare( Path.GetExtension( strファイル名 ), ".xa", true ) == 0 )	// caselessで文字列比較
			{
				tBASSサウンドを作成するXA( strファイル名, hMixer, flags );
				return;
			}

			this.e作成方法 = E作成方法.ファイルから;
			this.strファイル名 = strファイル名;


			// BASSファイルストリームを作成。

			this.hBassStream = Bass.BASS_StreamCreateFile( strファイル名, 0, 0, flags );
			if( this.hBassStream == 0 )
				throw new Exception( string.Format( "サウンドストリームの生成に失敗しました。[{0}]", Bass.BASS_ErrorGetCode() ) );


			// ミキサーにBASSファイルストリームを追加。

			BassMix.BASS_Mixer_StreamAddChannel( hMixer, this.hBassStream, BASSFlag.BASS_SPEAKER_FRONT | BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_NORAMPIN );
			//BassMix.BASS_Mixer_ChannelPause( this.hBassStream );	// 追加すると勝手に再生（ミキサへの出力）が始まるので即停止。


			// インスタンスリストに登録。

			CSound.listインスタンス.Add( this );

			// nBytesとn総演奏時間の取得; DTXMania用に追加。
			nBytes = Bass.BASS_ChannelGetLength( this.hBassStream );
			double seconds = Bass.BASS_ChannelBytes2Seconds( this.hBassStream, nBytes );
			this.n総演奏時間ms = (int) ( seconds * 1000 );
		}
		private void tBASSサウンドを作成する( byte[] byArrWAVファイルイメージ, int hMixer, BASSFlag flags )
		{
			this.e作成方法 = E作成方法.WAVファイルイメージから;
			this.byArrWAVファイルイメージ = byArrWAVファイルイメージ;
			this.hGC = GCHandle.Alloc( byArrWAVファイルイメージ, GCHandleType.Pinned );		// byte[] をピン留め


			// BASSファイルストリームを作成。

			this.hBassStream = Bass.BASS_StreamCreateFile( hGC.AddrOfPinnedObject(), 0, byArrWAVファイルイメージ.Length, flags );
			if( this.hBassStream == 0 )
				throw new Exception( "サウンドストリームの生成に失敗しました。" );


			// ミキサーにBASSファイルストリームを追加。

			BassMix.BASS_Mixer_StreamAddChannel( hMixer, this.hBassStream, BASSFlag.BASS_SPEAKER_FRONT | BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_NORAMPIN );
//			BassMix.BASS_Mixer_ChannelPause( this.hBassStream );	// 追加すると勝手に再生（ミキサへの出力）が始まるので即停止。


			// インスタンスリストに登録。

			CSound.listインスタンス.Add( this );

			// nBytesとn総演奏時間の取得; DTXMania用に追加。
			nBytes = Bass.BASS_ChannelGetLength( this.hBassStream );
			double seconds = Bass.BASS_ChannelBytes2Seconds( this.hBassStream, nBytes );
			this.n総演奏時間ms = (int) ( seconds * 1000 );
		}
		private void tBASSサウンドを作成するXA( string strファイル名, int hMixer, BASSFlag flags )
		{
			Debug.WriteLine( "xaデコード開始: " + Path.GetFileName( strファイル名 ) );
			Cxa xa = new Cxa();
			xa.Decode( strファイル名, out this.byArrWAVファイルイメージ );
Debug.WriteLine( "デコード完了:" + Path.GetFileName(strファイル名));
//return;
			this.e作成方法 = E作成方法.WAVファイルイメージから;
			this.hGC = GCHandle.Alloc( this.byArrWAVファイルイメージ, GCHandleType.Pinned );		// byte[] をピン留め

Debug.WriteLine( "ピン止め完了:" + Path.GetFileName( strファイル名 ) );
			// BASSファイルストリームを作成。
			//xah.id = br.ReadUInt32();
			//xah.nDataLen = br.ReadUInt32();
			//xah.nSamples = br.ReadUInt32();
			//xah.nSamplesPerSec = br.ReadUInt16();
			//xah.nBits = br.ReadByte();
			//xah.nChannels = br.ReadByte();
			//xah.nLoopPtr = br.ReadUInt32();
			//            Debug.WriteLine( "**WAVEFORMATEX**" );
			//Debug.WriteLine( "wFormatTag=      " + wfx.wFormatTag.ToString("X4") );
			//Debug.WriteLine( "nChannels =      " + wfx.nChannels.ToString( "X4" ) );
			//Debug.WriteLine( "nSamplesPerSec=  " + wfx.nSamplesPerSec.ToString( "X8" ) );
			//Debug.WriteLine( "nAvgBytesPerSec= " + wfx.nAvgBytesPerSec.ToString( "X8" ) );
			//Debug.WriteLine( "nBlockAlign=     " + wfx.nBlockAlign.ToString( "X4" ) );
			//Debug.WriteLine( "wBitsPerSample=  " + wfx.wBitsPerSample.ToString( "X4" ) );
			//Debug.WriteLine( "cbSize=          " + wfx.cbSize.ToString( "X4" ) );
				//xash.pSrc = pXaBuf;
				//xash.nSrcLen = xah.nDataLen;
				//xash.nSrcUsed = 0;
				//xash.pDst = pWavBuf;
				//xash.nDstLen = dlen2;
				//xash.nDstUsed = 0;
				//xaDecodeConvert( hxas, ref xash );


//this.hBassStream = Bass.BASS_SampleCreate(256, 28160, 1, 1, BASSFlag.BASS_SAMPLE_LOOP| BASSFlag.BASS_SAMPLE_OVER_POS); // create sample
//if ( this.hBassStream == 0 )
//{
//    BASSError err = Bass.BASS_ErrorGetCode();
//    Debug.WriteLine( "11BASS_SampleCreate: " + err );
//        throw new Exception( "11サウンドストリームの生成に失敗しました。(BASS_SampleCreate: " + err + ")" );
//    }
//short[] data = new short[128]; // data buffer
//int a;
//for (a=0; a<128; a++)
//    data[a]=(short)(32767.0*Math.Sin((double)a*6.283185/64)); // sine wave
//bool bb=Bass.BASS_SampleSetData(this.hBassStream, data); // set the sample's data
//if ( !bb )
//{
//    hGC.Free();
//    BASSError err = Bass.BASS_ErrorGetCode();
//    Debug.WriteLine( "11BASS_SampleSetData: " + err );
//    throw new Exception( "サウンドストリームの生成に失敗しました。(BASS_SampleSetData)" );
//}


Debug.WriteLine( "xash.nDstLen=" + xa.xastreamheader.nDstLen + ", xah.nSamplesPerSec=" + xa.xaheader.nSamplesPerSec + ", xah.nChannels=" + xa.xaheader.nChannels );
			//this.hBassStream = Bass.BASS_SampleCreate( (int) xash.nDstLen, xah.nSamplesPerSec, xah.nChannels, 1, flags );
//◆ XA Decorder liblary 001 のバグ
//デコード後の XASTREAMHEADER::nDstUsed が大きめの値を返してくるので
//そのままのサイズで再生すると最後にノイズが乗る。
//そこで、xaDecodeConvert() 後のPCMサイズは、次の式で算出する。
//dwPCMSize = nSamples * nChannels * 2;
//（nSamples, nChannels は XAHEADER のメンバ）

			_myStreamCreate = new STREAMPROC( MyFileProc );

			int length = (int) ( xa.xaheader.nSamples * xa.xaheader.nChannels * 2 );
Debug.WriteLine( "length=" + length );
//			this.hBassStream = Bass.BASS_SampleCreate( length, xa.xaheader.nSamplesPerSec, xa.xaheader.nChannels, 1, BASSFlag.BASS_STREAM_DECODE );
			this.hBassStream = Bass.BASS_StreamCreate( xa.xaheader.nSamplesPerSec, xa.xaheader.nChannels, BASSFlag.BASS_STREAM_DECODE, _myStreamCreate, IntPtr.Zero );
			if ( this.hBassStream == 0 )
			{
				hGC.Free();
				BASSError err = Bass.BASS_ErrorGetCode();
Debug.WriteLine( "BASS_SampleCreate: " + err );
				throw new Exception( "サウンドストリームの生成に失敗しました。(BASS_SampleCreate: " + err + ")" );
			}
Debug.WriteLine( "SampleCreate完了:" + Path.GetFileName( strファイル名 ) );
//            bool b = Bass.BASS_SampleSetData( this.hBassStream, this.byArrWAVファイルイメージ);		// ★★★★★★★★★★★★★★ 多分bufをshort見せしないとダメ！！！
//            if ( !b )
//            {
//                hGC.Free();
//                BASSError err = Bass.BASS_ErrorGetCode();
//                Debug.WriteLine( "BASS_SampleSetData: " + err );
//                throw new Exception( "サウンドストリームの生成に失敗しました。(BASS_SampleSetData)" );
//            }
//Debug.WriteLine( "SampleSetData完了:" + Path.GetFileName( strファイル名 ) );


			// ミキサーにBASSファイルストリームを追加。

			BassMix.BASS_Mixer_StreamAddChannel( hMixer, this.hBassStream, BASSFlag.BASS_SPEAKER_FRONT | BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_NORAMPIN );
			//			BassMix.BASS_Mixer_ChannelPause( this.hBassStream );	// 追加すると勝手に再生（ミキサへの出力）が始まるので即停止。
Debug.WriteLine( "StreamAddChannel完了:" + Path.GetFileName( strファイル名 ) );

			// インスタンスリストに登録。

			CSound.listインスタンス.Add( this );
Debug.WriteLine( "listインスタンス.Add完了:" + Path.GetFileName( strファイル名 ) );

			// nBytesとn総演奏時間の取得; DTXMania用に追加。
			//nBytes = Bass.BASS_ChannelGetLength( this.hBassStream );
			nBytes = length;
			double seconds = Bass.BASS_ChannelBytes2Seconds( this.hBassStream, nBytes );
			this.n総演奏時間ms = (int) ( seconds * 1000 );
Debug.WriteLine( "nBytes=" + nBytes + ", n総演奏時間=" + this.n総演奏時間ms );
		}
		//-----------------

//		private byte[] _data = null; // our local buffer
		private int pos = 0;
		private int MyFileProc( int handle, IntPtr buffer, int length, IntPtr user )
		{

			// increase the data buffer as needed
			//if ( _data == null || _data.Length < length )
			//    _data = new byte[ length ];

			int bytesread = ( pos + length > Convert.ToInt32(nBytes) ) ? Convert.ToInt32(nBytes) - pos : length;

			Marshal.Copy( byArrWAVファイルイメージ, pos, buffer, bytesread );
			pos += bytesread;

			if ( bytesread < length )
			{
				// set indicator flag
				bytesread |= (int) BASSStreamProc.BASS_STREAMPROC_END;
			}
			return bytesread;
		}
		#endregion
	}
}
