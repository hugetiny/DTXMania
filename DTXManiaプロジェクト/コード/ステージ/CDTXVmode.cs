using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using FDK;


namespace DTXMania
{
	public class CDTXVmode
	{
		public enum ECommand
		{
			Stop,
			Play
		}

		/// <summary>
		/// DTXVモードかどうか
		/// </summary>
		public bool Enabled
		{
			get;
			set;
		}

		/// <summary>
		/// 外部から再指示が発生したか
		/// </summary>
		public bool Refreshed
		{
			get;
			set;
		}

		/// <summary>
		/// 演奏開始小節番号
		/// </summary>
		public int nStartBar
		{
			get;
			set;
		}

		/// <summary>
		/// DTXファイルの再読み込みが必要かどうか
		/// </summary>
		public bool NeedReload
		{
			get;
			set;
//			private set;	// 本来はprivate setにすべきだが、デバッグが簡単になるので、しばらくはprivateなしのままにする。
		}

		/// <summary>
		/// DTXCからのコマンド
		/// </summary>
		public ECommand Command
		{
			get;
			private set;
		}

		public string filename
		{
			get
			{
				return last_path;
			}
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CDTXVmode()
		{
			this.last_path = "";
			this.last_timestamp = DateTime.MinValue;
			this.Enabled = false;
			this.nStartBar = 0;
			this.Refreshed = false;
			this.NeedReload = false;
		}

		/// <summary>
		/// DTXファイルのリロードが必要かどうか判定する
		/// </summary>
		/// <param name="filename">DTXファイル名</param>
		/// <returns>再読込が必要ならtrue</returns>
		/// <remarks>プロパティNeedReloadにも結果が入る</remarks>
		/// <remarks>これを呼び出すたびに、Refreshedをtrueにする</remarks>
		/// <exception cref="FileNotFoundException"></exception>
		public bool bIsNeedReloadDTX( string filename )
		{
			if ( !File.Exists( filename ) )			// 指定したファイルが存在しないなら例外終了
			{
				Trace.TraceError( "ファイルが見つかりません。({0})", filename );
				throw new FileNotFoundException();
				//return false;
			}

			this.Refreshed = true;

			// 前回とファイル名が異なるか、タイムスタンプが更新されているなら、DTX要更新
			DateTime current_timestamp = File.GetLastWriteTime( filename );
			if ( last_path != filename || current_timestamp > last_timestamp)
			{
				this.last_path = filename;
				this.last_timestamp = current_timestamp;
				this.NeedReload = true;
				return true;
			}
			this.NeedReload = false;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="nStartBar"></param>
		/// <param name="command"></param>
		/// <returns>DTXV用の引数であればtrue</returns>
		/// <remarks>内部でEnabled, nStartBar, Command, NeedReload, filename, last_path, last_timestampを設定する</remarks>
		public bool ParseArguments( string arg )
		{
			bool ret = false;
			this.nStartBar = 0;

			if ( arg != null )
			{
				// -S  -Nxxx  filename
				if ( arg.StartsWith( "-S", StringComparison.OrdinalIgnoreCase ) )		// DTXV再生停止
				{
					this.Enabled = true;
					this.Command = ECommand.Stop;
					this.Refreshed = true;
					ret = true;
				}
				else if ( arg.StartsWith( "-N", StringComparison.OrdinalIgnoreCase ) )
				{
					this.Enabled = true;
					this.Command = ECommand.Play;
					ret = true;

					arg = arg.Substring( 2 );					// "-N"を除去
					string[] p = arg.Split( new char[] { ' ' } );
					this.nStartBar = int.Parse( p[ 0 ] );			// 再生開始小節
					if ( this.nStartBar < 0 )
					{
						this.nStartBar = 0;
					}
//Debug.WriteLine( "再生開始小節: " + this.nStartBar );

					int startIndex = arg.IndexOf( ' ' );
					string filename = arg.Substring( startIndex + 1　);	// 再生ファイル名(フルパス)
					try
					{
//Debug.WriteLine( "filename_quoted=" + filename );
						filename = filename.Trim( new char[] { '\"' } );
						bIsNeedReloadDTX( filename );
					}
					catch	// 指定ファイルが存在しない
					{
					}
				}
			}

			return ret;
		}



		private string last_path;
		private DateTime last_timestamp;

	}
}
