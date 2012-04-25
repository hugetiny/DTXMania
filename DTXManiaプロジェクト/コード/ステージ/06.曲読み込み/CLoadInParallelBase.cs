using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using FDK;

namespace DTXMania
{
//#if false

	static public class SuggestNumOfThreads
	{

		#region [ 読み込みに使用するスレッド数を決定する ]
		public static int GetNumOfThreads()
		{
			FDK.CWin32.SYSTEM_INFO sysInfo = new FDK.CWin32.SYSTEM_INFO();
			FDK.CWin32.GetSystemInfo( ref sysInfo );
			int nThreads = (int) sysInfo.dwNumberOfProcessors;
			nThreads = 8;				// あまりコア数とスレッド数の性能相関が出なかったので、とりあえず8で固定

			if ( nThreads > 64 )
			{
				nThreads = 63;				// 64を超えるスレッドをWaitOneには渡せない。64を渡すとWaitOneからtimeoutエラーが返る
			}
			else if ( nThreads <= 1 )		// シングルコアの場合に限り、スレッド数を2にしてHDDのI/O待ちの隠蔽を試みる
			{
				nThreads = 2;
			}
			return nThreads;
		}
		#endregion
	}
	
	
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

		private ManualResetEvent[] manualEvents;
		private bool[] manualEventsState;

		private object lockObj = new object();
		private Queue<tLoadState> queueDone = new Queue<tLoadState>();
		private Queue<tLoadState> queueErr = new Queue<tLoadState>();

		public enum ErrCode
		{
			OK,
			TemporalError,
			FatalError
		}

		protected class tLoadState
		{
			public ManualResetEvent manualEvent;
			public T cItem;
			public ErrCode errcode;
			public int index;

			public tLoadState( int index, ManualResetEvent manualEvent, T cItem, ref ErrCode errcode )
			{
				this.index = index;
				this.manualEvent = manualEvent;
				this.cItem = cItem;
				this.errcode = errcode;
			}
		}


		// 今後の方針：
		// 別スレッドその1で、ひたすら読み込みタスクを投入し続ける。
		// メインスレッドでは、読み込み完了の度にデコード処理(メイン処理)を走らせる。
		//


		public void t読み込み( Dictionary<int, T> listT )
		{
			if ( listT.Count == 0 )
			{
				return;
			}

			#region [ 読み込みに使用するスレッド数を決定する ]
			int nThreads = SuggestNumOfThreads.GetNumOfThreads();

			if ( nThreads > listT.Count )
			{
				nThreads = listT.Count;
			}
			if ( nThreads > 16 )	// 何となく最大16スレッドに制限
			{
				nThreads = 16;
			}
			#endregion
			#region [ 並列処理オフの場合の処理 ]	//並列処理オフ時は、t読み込み別ThreadMain()もメインスレッドで処理させて、従来互換の動作とする
			if ( !bActivateParallelLoad )
			{
				foreach ( T cwav in listT.Values )
				{
					ErrCode e = CLoadInParallelBase<T>.ErrCode.OK;
					tLoadState tlws = new tLoadState( 0, null, cwav, ref e );
					while (true)
					{
						t読み込み別ThreadMain( tlws );
						if ( tlws.errcode != CLoadInParallelBase<T>.ErrCode.TemporalError )
						{
							break;
						}
						Thread.Sleep( 500 );
					}
					t読み込みメインThread終了処理( tlws);
				}
				return;
			}
			#endregion
			
			#region [ 最初にスレッド数分だけjobを詰めつつ、jobを開始 ]
			manualEvents = new ManualResetEvent[ nThreads ];
			manualEventsState = new bool[ nThreads ];
			for ( int i = 0; i < nThreads; i++ )
			{
				manualEvents[ i ] = new ManualResetEvent( false );
				manualEventsState[ i ] = false;
			}
			int countStarted = 0, countEnded = 0;
			tLoadState tls;
			foreach ( T cwav in listT.Values )
			{
				ErrCode errcode = new CLoadInParallelBase<T>.ErrCode();
				
				System.Threading.ThreadPool.QueueUserWorkItem(
					new System.Threading.WaitCallback( t読み込み別Thread ),
					new tLoadState( countStarted, manualEvents[ countStarted ], cwav, ref errcode ) );
//Debug.WriteLine( "Add " + countStarted );

				if ( ++countStarted >= nThreads )
				{
					break;
				}
			}
			#endregion
			#region [ 1つスレッドが終わるまで待つ / スレッドプールを使いすぎてタイムアウトにならないか確認 ]
			int index;
			index = WaitHandle.WaitAny( manualEvents, 60 * 1000, false );

			if ( index == WaitHandle.WaitTimeout )
			{
				throw new TimeoutException( "t読み込み別Thread()の処理待ちでタイムアウトが発生しました。(60sec待ちにしているにも関わらず。)" );
			}

			lock ( lockObj )
			{
				tls = queueDone.Dequeue();
				index = tls.index;			// WaitAnyでまだ1回しかスレッド終了待ちをしていが、既に複数スレッドが終了している可能性があるため、
				manualEventsState[ index ] = true;
			}								// 終了処理に渡すデータやindexは、全てListDone()から取ってくるようにする
//bug.WriteLine( "Delete " + index + " from listDone. Done Count=" + listDone.Count );
//Debug.WriteLine( "end thread ("+ index + " " +  tls.errcode.ToString() + " )"  );

			t読み込みメインThread終了処理( tls );
			countEnded++;
			if ( tls.errcode == ErrCode.TemporalError )
			{
				lock ( lockObj )
				{
					queueErr.Enqueue( tls );
				}
				Debug.WriteLine( "Retry: index=" + index + ", index=" + tls.index );
			}
			#endregion



			#region [ 以後、jobが終了する度に、次のjobを投入する ]
			countStarted = 0;
			foreach ( T cwav in listT.Values )
			{
//Debug.WriteLine( "countStarted=" + countStarted );
				if ( ++countStarted <= nThreads )	// 最初Queueに詰めた分を読み飛ばす
				{
					continue;
				}
				manualEvents[ index ].Reset();
				manualEventsState[ index ] = false;
				//if ( queueErr.Count > 0 )
				//{
				//    lock ( lockObj )
				//    {
				//        tls = queueErr.Dequeue();		// 直前に終了した読み込みが回復可能なエラーなら、リトライする
				//    }
				//    Debug.WriteLine( "読み込みリトライ発生" );
				//    Thread.Sleep( 500 );
				//}
				//else									// エラーがないなら、次のタスクを準備する
				{
					ErrCode errcode = new CLoadInParallelBase<T>.ErrCode();
					tls = new tLoadState( index, manualEvents[ index ], cwav, ref errcode );
				}

				// Debug.WriteLine( cwav.strファイル名 + ": start thread(" + index + ")" );
//Debug.WriteLine( "Add " + index + "." );
				System.Threading.ThreadPool.QueueUserWorkItem(		// エラーリトライまたは次のタスクを、ThreadPoolに突っ込む
					new System.Threading.WaitCallback( t読み込み別Thread ),
					tls );

				index = WaitHandle.WaitAny( manualEvents, 60 * 1000, false );
//				ErrCode errcode = new CLoadInParallelBase<T>.ErrCode();
//				tls = new tLoadState( index, autoEvents[ index ], cwav, ref errcode );

				while ( queueDone.Count > 0 )			// WaitAnyだけだと2つ以上同時にEvent発生したときに処理漏れが発生するので
				{									// バックグラウンドスレッドの処理終了時にListDoneに結果をAddしてもらうようにした
					lock ( lockObj )
					{
						tls = queueDone.Dequeue();			// listDone経由の場合は、必ずこのtlsをt読み込みメインThread終了処理()に渡すこと。
													// indexからnew tLoadState()すると、ListDone上のとあるindexの処理前に同じindexでもう一度処理が終わったときに破綻する。
						index = tls.index;
						manualEventsState[ index ] = true;
					}
//Debug.WriteLine( "Delete index " + index + " from listDone. queueDone.Count=" + queueDone.Count );
					t読み込みメインThread終了処理( tls );
					countEnded++;
//Debug.WriteLine( "end thread (" + index + " " + tls.errcode.ToString() + " )" );
					if ( tls.errcode == ErrCode.TemporalError )		// t読み込み別Thread()側の最後にindexをListにpushするとか市内と駄目そう
					{
						lock ( lockObj )
						{
							queueErr.Enqueue( tls );
						}
					}
				}
			}
			#endregion

			#region [ 最後、スレッドスプールに残っている分を処理する ]
			while ( countEnded < listT.Count )
			{
//Debug.WriteLine( "countStarted=" + countStarted );

				manualEvents[ index ].Reset();
				manualEventsState[ index ] = false;
				if ( queueErr.Count > 0 )
				{
					lock ( lockObj )
					{
						tls = queueErr.Dequeue();
					}
							// 直前に終了した読み込みが回復可能なエラーなら、リトライする
					Debug.WriteLine( "読み込みリトライ発生" );
					Thread.Sleep( 500 );
				}

				index = WaitHandle.WaitAny( manualEvents, 60 * 1000, false );
				// ErrCode errcode = new CLoadInParallelBase<T>.ErrCode();
				while ( queueDone.Count > 0 )			// WaitAnyだけだと2つ以上同時にEvent発生したときに処理漏れが発生するので
				{									// バックグラウンドスレッドの処理終了時にListDoneに結果をAddしてもらうようにした
					lock ( lockObj )
					{
						tls = queueDone.Dequeue();
						index = tls.index;
						manualEventsState[ index ] = true;
//Debug.WriteLine( "DELETE index " + index + " from listDone. queueDone.Count=" + queueDone.Count );
					}
					t読み込みメインThread終了処理( tls );						// 多分この処理が重すぎて、その間に別のthreadが終わってしまうがそこの終了処理ができていない
					countEnded++;
					if ( tls.errcode == ErrCode.TemporalError )		// t読み込み別Thread()側の最後にindexをListにpushするとか市内と駄目そう
					{
						lock ( lockObj )
						{
							queueErr.Enqueue( tls );
						}
					}
//Debug.WriteLine( "END THREAD(" + index + " " + tls.errcode.ToString() + " )" );
				}
			}
			#endregion
		}



		protected void t読み込み別Thread( object tlws )
		{
			t読み込み別ThreadMain( tlws );
			tLoadState t = (tLoadState) tlws;
			// errcodes[ t.index ] = t.errcode;
//Debug.WriteLine( "END: " + t.index + "; list added" );
			lock ( lockObj )
			{
				queueDone.Enqueue( t );
			}
			t.manualEvent.Set();
		}
		
		/// <summary>
		/// 読み込み処理本体を、継承クラス側で準備する。
		/// </summary>
		/// <param name="tlws">tLoadState型の引数</param>
		protected virtual void t読み込み別ThreadMain( object tlws )
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tlws"></param>
		protected virtual void t読み込みメインThread終了処理( object tlws )
		{
		}
	}
//#endif
}
