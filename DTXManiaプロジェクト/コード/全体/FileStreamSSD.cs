using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DTXMania
{
	/// <summary>
	/// SSD向けFileStream (ファイルの上書き保存時に、同じファイル内容を保存する場合は、ファイルの更新を行わない)
	/// </summary>
	public class FileStreamSSD : MemoryStream, IDisposable
	{
		private string _filename;
		private FileMode _mode;
		private FileAccess _access;
		private MemoryStream _ms_new = null, _ms_org = null;
		private FileStream _fs;

		#region [ コンストラクタ ]
		public FileStreamSSD()
		{
			throw new ArgumentException( "FileStreamSSD: 引数のないコンストラクタは使用できません。" );
		}

		public FileStreamSSD( string path )
		{
//Trace.TraceInformation( "FileStreamSSD(" + Path.GetFileName(path) + "): Constractor" );
			Initialize( path, FileMode.Create, FileAccess.Write );
		}
		public FileStreamSSD( string path, FileMode mode, FileAccess access )
		{
//Trace.TraceInformation( "FileStreamSSD(" + Path.GetFileName(path) + ", " + mode.ToString() + ", " + access.ToString() + "): Constractor" );
			Initialize( path, mode, access );
		}

		private void Initialize( string path, FileMode mode, FileAccess access )
		{
			if ( mode != FileMode.Create )
			{
				throw new ArgumentException( mode.ToString() + "は、FileStreamSSD()でサポートしていません。" );
			}
			if ( access != FileAccess.Write )
			{
				throw new ArgumentException( access.ToString() + "は、FileStreamSSD()でサポートしていません。" );
			}
			_filename = path;
			_mode = mode;
			_access = access;
			_ms_new = new MemoryStream();
		}
		#endregion


		/// <summary>
		/// StreamのWrite。
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write( byte[] buffer, int offset, int count )
		{
			_ms_new.Write( buffer, offset, count );
//if ( Path.GetExtension( _filename ) != ".db" )
//{
//Debug.Write( "W" );
//}
		}

		/// <summary>
		/// StreamのClose。元ファイルとのコンペアを行い、一致していればファイルの上書きをしない
		/// </summary>
		public override void Close()
		{
Debug.WriteLine( "---" );
			bool bSame = true;
			Flush();

			// 元ファイルがなければ、無条件に上書きコースへ。
			if ( !File.Exists( _filename ) )
			{
				bSame = false;
Debug.WriteLine( Path.GetFileName( _filename ) + ": No file exists" );
			}
			else
			// まず、既存ファイルをMemoryStreamにコピー
			{
Debug.WriteLine( "B2" );
				using ( _fs = new FileStream( _filename, FileMode.Open, FileAccess.Read ) )
				{
					Debug.WriteLine( "Length old=" + _fs.Length + ", new=" + _ms_new.Length );
					// 元ファイルとファイルサイズが異なる場合は、
					if ( _fs.Length != _ms_new.Length )
					{
						bSame = false;
Debug.WriteLine( "B2: Set bSame=false" );
					}
					else
					{
Debug.WriteLine( "B2: copy from _fs to _ms_org" );
						_ms_org = new MemoryStream();
						_fs.CopyTo( _ms_org );
					}
				};	// _fs will be closed and disposed here, by using()
				_fs = null;
			}

Debug.WriteLine( "C" );
			if ( bSame )	// まだ新旧ファイルが一致している可能性があれば...
			{
				// MemoryStream同士のコンペア
				_ms_org.Seek( 0, SeekOrigin.Begin );
				_ms_new.Seek( 0, SeekOrigin.Begin );
			
Debug.WriteLine( "1" );
				while (_ms_new.Position < _ms_new.Length )
				{
					int dorg = _ms_org.ReadByte();
					int dnew = _ms_new.ReadByte();
					if (dorg != dnew)
					{
						bSame = false;
						break;
					}
				}
			}
Debug.WriteLine( "2: bSame=" + bSame );
			if ( _ms_org != null )
			{
				_ms_org.Close();
				_ms_org.Dispose();
				_ms_org = null;
			}
Debug.WriteLine( "3" );
			_ms_new.Seek( 0, SeekOrigin.Begin );

Debug.WriteLine( "new file length: " + _ms_new.Length );
			// 元ファイルと新規ファイルが一致していない場合、新規ファイルで上書きする
			if ( !bSame )
			{
				Trace.TraceInformation( Path.GetFileName( _filename ) + ": 以前のファイルから変化があったため、書き込みを実行します。" );
				using ( _fs = new FileStream( _filename, _mode, _access ) )
				{
					_ms_new.CopyTo( _fs );
				}	// _fs will be closed and disposed, by using()
				_fs = null;
			}
			else
			{
				Trace.TraceInformation( Path.GetFileName( _filename ) + ": 以前のファイルから変化がなかったため、書き込みを行いません。" );
			}
			_ms_new.Close();
			_ms_new.Dispose();
			_ms_new = null;

			//base.Close();
		}



		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public new void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
			base.Dispose();
		}
		protected override void Dispose( bool bManagedDispose )
		{
			if ( this._ms_new != null )
			{
				this.Close();				// Close()する前にDispose()された場合用 (DataContractSerializer経由だと、ここに来る)
				//this._ms_new.Dispose();	// Close()内でDisposeとnullクリアがなされるため、Disposeはしない
				//this._ms_new = null;		// 
			}
			if ( this._ms_org != null )
			{
				this._ms_org.Dispose();
				this._ms_org = null;
			}
			if ( this._fs != null )
			{
				this._fs.Dispose();
				this._fs = null;
			}
		}
		~FileStreamSSD()
		{
			this.Dispose( false );
		}
		#endregion
	}
}
