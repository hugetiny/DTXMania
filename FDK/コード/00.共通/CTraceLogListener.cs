using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FDK
{
	public class CTraceLogListener : TraceListener
	{
		public CTraceLogListener( StreamWriter stream )
		{
			this.streamWriter = stream;
		}

		public override void Flush()
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.streamWriter.Flush();
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message )
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.tイベント種別を出力する( eventType );
					this.tインデントを出力する();
					SanitizeUsername( ref message );
					this.streamWriter.WriteLine( message );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args )
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.tイベント種別を出力する( eventType );
					this.tインデントを出力する();
					string message = string.Format(format, args);
					SanitizeUsername( ref message );
					this.streamWriter.WriteLine( message );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		public override void Write( string message )
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.streamWriter.Write( message );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		public override void WriteLine( string message )
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.streamWriter.WriteLine( message );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		private string SanitizeUsername(ref string message)
		{
			string userprofile = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

			// もしユーザー名の情報が出力に存在する場合は、伏字にする
			if (message != null && message.Contains(userprofile))
			{
				char delimiter = System.IO.Path.DirectorySeparatorChar;
				string[] u = userprofile.Split(delimiter);
				int c = u[u.Length - 1].Length;     // ユーザー名の文字数
				u[u.Length - 1] = "*".PadRight(c, '*');
				string sanitizedusername = string.Join(delimiter.ToString(), u);
				message = message.Replace(userprofile, sanitizedusername);
				return message;
			}
			return message;
		}

		protected override void Dispose( bool disposing )
		{
			if( this.streamWriter != null )
			{
				try
				{
					this.streamWriter.Close();
				}
				catch
				{
				}
				this.streamWriter = null;
			}
			base.Dispose( disposing );
		}

		#region [ private ]
		//-----------------
		private StreamWriter streamWriter;

		private void tイベント種別を出力する( TraceEventType eventType )
		{
			if( this.streamWriter != null )
			{
				try
				{
					var now = DateTime.Now;
					this.streamWriter.Write( string.Format( "{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}:{5:D2}.{6:D3} ", new object[] { now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond } ) );
					switch( eventType )
					{
						case TraceEventType.Error:
							this.streamWriter.Write( "[ERROR] " );
							return;

						case ( TraceEventType.Error | TraceEventType.Critical ):
							return;

						case TraceEventType.Warning:
							this.streamWriter.Write( "[WARNING] " );
							return;

						case TraceEventType.Information:
							break;

						default:
							return;
					}
					this.streamWriter.Write( "[INFO] " );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		private void tインデントを出力する()
		{
			if( ( this.streamWriter != null ) && ( base.IndentLevel > 0 ) )
			{
				try
				{
					for( int i = 0; i < base.IndentLevel; i++ )
						this.streamWriter.Write( "    " );
				}
				catch( ObjectDisposedException )
				{
				}
			}
		}
		//-----------------
		#endregion
	}
}
