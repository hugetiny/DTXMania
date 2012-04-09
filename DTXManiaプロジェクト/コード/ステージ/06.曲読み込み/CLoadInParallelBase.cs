using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using FDK;

namespace DTXMania
{
//#if false
	/// <summary>
	/// 並列読み込み機能を提供する基底クラス
	/// </summary>
	/// <typeparam name="TWAV">読み込むデータの型</typeparam>
	public class CLoadInParallelBase<T>
	{
		public bool bActivateParallelLoad { get; set; }

		protected string PATH_WAV, strフォルダ名;

		#region [ コンストラクタ ]
		public CLoadInParallelBase()
		{
			CLoadInParallel_Constructor( "", "" );
		}
		public CLoadInParallelBase( string p, string f )
		{
			CLoadInParallel_Constructor( p, f );
		}
		private void CLoadInParallel_Constructor(string p, string f)
		{
			this.PATH_WAV = p;
			this.strフォルダ名 = f;
			this.bActivateParallelLoad = false;
		}
		#endregion


		/// <summary>
		/// コンストラクタでPATH_WAVとstrフォルダ名を設定しなかった場合の設定I/F
		/// </summary>
		/// <param name="p">PATH_WAV</param>
		/// <param name="f">strフォルダ名</param>
		public void SetPaths( string p, string f )
		{
			CLoadInParallel_Constructor( p, f );
		}

		AutoResetEvent[] autoEvents;
		T[] cItems;
		ErrCode[] errcodes;

		public enum ErrCode
		{
			OK,
			TemporalError,
			FatalError
		}

		protected class tLoadState
		{
			public AutoResetEvent autoEvent;
			public T cItem;
			public ErrCode errcode;
			public int index;

			public tLoadState( int index, AutoResetEvent autoEvent, T cItem, ErrCode errcode )
			{
				this.index = index;
				this.autoEvent = autoEvent;
				this.cItem = cItem;
				this.errcode = errcode;
			}
		}

		public void t読み込み( Dictionary<int, T> listT )
		{
			if ( listT.Count == 0 )
			{
				return;
			}

			#region [ 読み込みに使用するスレッド数を決定する ]
			//CpuCores.SYSTEM_INFO sysInfo = new CpuCores.SYSTEM_INFO();
			//CpuCores.GetSystemInfo( ref sysInfo );
			//int nThreads = (int) sysInfo.dwNumberOfProcessors;
			int nThreads = 8;					// あまりコア数とスレッド数の性能相関が出なかったので、とりあえず8で固定

			if ( bActivateParallelLoad )
			{
				if ( nThreads > listT.Count )
				{
					nThreads = listT.Count;
				}
				if ( nThreads > 64 )
				{
					nThreads = 63;		// 64を超えるスレッドをWaitOneには渡せない。64を渡すとWaitOneからtimeoutエラーが返る
				}
			}
			else
			{
				nThreads = 1;
			}
			#endregion
			#region [ 最初にスレッド数分だけjobを詰める ]
			autoEvents = new AutoResetEvent[ nThreads ];
			for ( int i = 0; i < nThreads; i++ )
			{
				autoEvents[ i ] = new AutoResetEvent( false );
			}
			cItems = new T[ nThreads ];
			errcodes = new ErrCode[ nThreads ];
			int c = 0;
			foreach ( T cwav in listT.Values )
			{
				cItems[ c ] = cwav;
				errcodes[ c ] = ErrCode.OK;
				if ( ++c >= nThreads )
				{
					break;
				}
			}
			#endregion
			#region [ スレッド数分詰めたjobを実行開始 ]
			for ( int i = 0; i < nThreads; i++ )
			{
				System.Threading.ThreadPool.QueueUserWorkItem(
					new System.Threading.WaitCallback( t読み込みThread ),
					new tLoadState( i, autoEvents[ i ], cItems[ i ], errcodes[ i ] ) );
			}
			int index = WaitHandle.WaitAny( autoEvents, System.Threading.Timeout.Infinite, false );
			if ( index == WaitHandle.WaitTimeout )
			{
				throw new TimeoutException( "チップ音のデコード待ちでタイムアウトが発生しました。(無限待ちにしているにも関わらず。)" );
			}
			#endregion

			#region [ エラーが発生したらリトライするためのListを用意する ]
			List<T> errListT = new List<T>();
			if ( errcodes[ index ] == ErrCode.TemporalError )
			{
				errListT.Add( cItems[ index ] );
				Debug.WriteLine( "index=" + index + ", cwavs[index]=" + cItems[ index ] );
			}
			#endregion
			#region [ 以後、jobが終了する度に、次のjobを投入する ]
			c = 0;
			foreach ( T cwav in listT.Values )
			{
				if ( ++c <= nThreads )	// 最初Queueに詰めた分を読み飛ばす
				{
					continue;
				}
				autoEvents[ index ].Reset();
				if ( errListT.Count > 0 )
				{
					cItems[ index ] = errListT[ 0 ];		// 直前に終了した読み込みが回復可能なエラーなら、リトライする
					Debug.WriteLine( "読み込みリトライ発生" );
					errListT.RemoveAt( 0 );
					Thread.Sleep( 500 );
				}
				else
				{
					cItems[ index ] = cwav;
				}
				// Debug.WriteLine( cwav.strファイル名 + ": start thread(" + index + ")" );
				System.Threading.ThreadPool.QueueUserWorkItem(
					new System.Threading.WaitCallback( t読み込みThread ),
					new tLoadState( index, autoEvents[ index ], cItems[ index ], errcodes[ index ] ) );

				index = WaitHandle.WaitAny( autoEvents, System.Threading.Timeout.Infinite, false );
				// Debug.WriteLine( "end thread ("+ index+")" );
				if ( errcodes[ index ] == ErrCode.TemporalError )
				{
					errListT.Add( cItems[ index ] );
				}
			}
			#endregion
		}


		protected void t読み込みThread( object tlws )
		{
			t読み込みMain( tlws );
			tLoadState t = (tLoadState) tlws;
			errcodes[ t.index ] = t.errcode;
			t.autoEvent.Set();
		}



		/// <summary>
		/// 読み込み処理本体を、継承クラス側で準備する。
		/// </summary>
		/// <param name="tlws">tLoadState型の引数</param>
		protected virtual void t読み込みMain( object tlws )
		{
		}
	}
//#endif
}
