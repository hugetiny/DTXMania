using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CTimer : IDisposable
	{
		// 定数

		public enum E種別
		{
			GetTickCount = 2,
			MultiMedia = 1,
			PerformanceCounter = 0,
			Unknown = -1
		}
		public const long n未使用 = -1;


		// プロパティ

		public E種別 eタイマ種別 {
			get;
			private set;
		}
		public long nシステム時刻
		{
			get
			{
				switch( this.eタイマ種別 )
				{
					case E種別.PerformanceCounter:
						{
							double num = 0.0;
							if( this.n現在の周波数 != 0L )
							{
								long x = 0L;
								QueryPerformanceCounter( ref x );
								num = ( (double) x ) / ( ( (double) this.n現在の周波数 ) / 1000.0 );
							}
							return (long) num;
						}
					case E種別.MultiMedia:
						return (long) timeGetTime();

					case E種別.GetTickCount:
						return (long) Environment.TickCount;
				}
				return 0;
			}
		}
		public long n現在時刻
		{
			get
			{
				if( this.n停止数 > 0 )
					return ( this.n一時停止時刻 - this.n前回リセットした時のシステム時刻 );

				return ( this.n更新時刻 - this.n前回リセットした時のシステム時刻 );
			}
			set
			{
				if( this.n停止数 > 0 )
					this.n前回リセットした時のシステム時刻 = this.n一時停止時刻 - value;
				else
					this.n前回リセットした時のシステム時刻 = this.n更新時刻 - value;
			}
		}
		public long n前回リセットした時のシステム時刻
		{
			get;
			private set;
		}

		
		// コンストラクタ

		public CTimer()
			: this( E種別.MultiMedia )
		{
		}
		
		public CTimer( E種別 eタイマ種別 )
		{
			this.eタイマ種別 = eタイマ種別;
			this.n前回リセットした時のシステム時刻 = 0;
			this.n更新時刻 = 0;
			this.n停止数 = 0;
			this.n一時停止時刻 = 0;

			if( n参照カウント[ (int) this.eタイマ種別 ] == 0 )
			{
				switch( this.eタイマ種別 )
				{
					case E種別.PerformanceCounter:
						if( !this.b確認と設定_PerformanceCounter() && !this.b確認と設定_MultiMedia() )
							this.b確認と設定_GetTickCount();
						break;

					case E種別.MultiMedia:
						if( !this.b確認と設定_MultiMedia() && !this.b確認と設定_PerformanceCounter() )
							this.b確認と設定_GetTickCount();
						break;

					case E種別.GetTickCount:
						this.b確認と設定_GetTickCount();
						break;
				}
			}
			this.tリセット();
			n参照カウント[ (int) this.eタイマ種別 ]++;
		}

		
		// メソッド

		public void tリセット()
		{
			this.t更新();
			this.n前回リセットした時のシステム時刻 = this.n更新時刻;
			this.n一時停止時刻 = this.n更新時刻;
			this.n停止数 = 0;
		}
		public void t一時停止()
		{
			if( this.n停止数 == 0 )
				this.n一時停止時刻 = this.n更新時刻;

			this.n停止数++;
		}
		public void t更新()
		{
			this.n更新時刻 = this.nシステム時刻;
		}
		public void t再開()
		{
			if( this.n停止数 > 0 )
			{
				this.n停止数--;
				if( this.n停止数 == 0 )
				{
					this.t更新();
					this.n前回リセットした時のシステム時刻 += this.n更新時刻 - this.n一時停止時刻;
				}
			}
		}

		#region [ IDisposable＋α 実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected virtual void Dispose( bool disposing )
		{
			if( !this.bDisposed )
			{
				this.bDisposed = true;
				if( disposing )
				{
					if( this.eタイマ種別 != E種別.Unknown )
					{
						n参照カウント[ (int) this.eタイマ種別 ]--;
						if( n参照カウント[ (int) this.eタイマ種別 ] == 0 )
						{
							switch( this.eタイマ種別 )
							{
								case E種別.Unknown:
								case E種別.PerformanceCounter:
								case E種別.GetTickCount:
									return;

								case E種別.MultiMedia:
									timeEndPeriod( this.timeCaps.wPeriodMin );
									return;
							}
						}
						else if( n参照カウント[ (int) this.eタイマ種別 ] < 0 )
						{
							n参照カウント[ (int) this.eタイマ種別 ] = 0;
						}
					}
				}
			}
		}
		//-----------------
		#endregion
	
		
		// その他

		#region [ private ]
		//-----------------
		private bool bDisposed;
		private long n一時停止時刻;
		private long n現在の周波数;
		private long n更新時刻;
		private static int[] n参照カウント = new int[ 3 ];
		private int n停止数;
		private TimeCaps timeCaps;

		private bool b確認と設定_GetTickCount()
		{
			this.eタイマ種別 = E種別.GetTickCount;
			return true;
		}
		private bool b確認と設定_MultiMedia()
		{
			this.timeCaps = new TimeCaps();
			if( ( timeGetDevCaps( out this.timeCaps, (uint) Marshal.SizeOf( typeof( TimeCaps ) ) ) == 0 ) && ( this.timeCaps.wPeriodMin < 10 ) )
			{
				this.eタイマ種別 = E種別.MultiMedia;
				timeBeginPeriod( this.timeCaps.wPeriodMin );
				return true;
			}
			return false;
		}
		private bool b確認と設定_PerformanceCounter()
		{
			if( QueryPerformanceFrequency( ref this.n現在の周波数 ) != 0 )
			{
				this.eタイマ種別 = E種別.PerformanceCounter;
				return true;
			}
			return false;
		}
		//-----------------
		#endregion

		#region [ DllImport ]
		//-----------------
		[DllImport( "kernel32.dll" )]
		private static extern short QueryPerformanceCounter( ref long x );
		[DllImport( "kernel32.dll" )]
		private static extern short QueryPerformanceFrequency( ref long x );
		[DllImport( "winmm.dll" )]
		private static extern void timeBeginPeriod( uint x );
		[DllImport( "winmm.dll" )]
		private static extern void timeEndPeriod( uint x );
		[DllImport( "winmm.dll" )]
		private static extern uint timeGetDevCaps( out TimeCaps timeCaps, uint size );
		[DllImport( "winmm.dll" )]
		private static extern uint timeGetTime();

		[StructLayout( LayoutKind.Sequential )]
		private struct TimeCaps
		{
			public uint wPeriodMin;
			public uint wPeriodMax;
		}
		//-----------------
		#endregion
	}
}
