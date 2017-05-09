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
		private MemoryStream _ms_org = null;
		private FileStream _fs;
		private bool disposed = false;

		/// <summary>
		/// (baseではなく)このクラスのClose()が実行されたかどうか
		/// </summary>
		private bool bClosed = false;



		#region [ コンストラクタ ]
		public FileStreamSSD()
		{
			throw new ArgumentException( "FileStreamSSD: 引数のないコンストラクタは使用できません。" );
		}

		public FileStreamSSD( string path )
			: this(path, FileMode.Create, FileAccess.Write)
		{
		}
		public FileStreamSSD( string path, FileMode mode, FileAccess access )
			: base()
		{
			if (mode != FileMode.Create)
			{
				throw new ArgumentException(mode.ToString() + "は、FileStreamSSD()でサポートしていません。");
			}
			if (access != FileAccess.Write)
			{
				throw new ArgumentException(access.ToString() + "は、FileStreamSSD()でサポートしていません。");
			}
			_filename = path;
			_mode = mode;
			_access = access;
		}
		#endregion

		/// <summary>
		/// StreamのClose。元ファイルとのコンペアを行い、一致していればファイルの上書きをしない
		/// </summary>
		public new void Close()
		{
			bool bSame = true;
			Flush();

			// 元ファイルがなければ、無条件に上書きコースへ。
			if ( !File.Exists( _filename ) )
			{
				bSame = false;
			}
			else
			// まず、既存ファイルをMemoryStreamにコピー
			{
				using ( _fs = new FileStream( _filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete ) )
				{
					// 元ファイルとファイルサイズが異なる場合は、
					if ( _fs.Length != this.Length )
					{
						bSame = false;
					}
					else
					{
						_ms_org = new MemoryStream();
						_fs.CopyTo( _ms_org );
					}
				};
				_fs = null;
			}

			if ( bSame )	// まだ新旧ファイルが一致している可能性があれば...
			{
				// MemoryStream同士のコンペア
				_ms_org.Seek( 0, SeekOrigin.Begin );
				this.Seek( 0, SeekOrigin.Begin );

				try
				{
					while (this.Position < this.Length)
					{
						int dorg = _ms_org.ReadByte();
						int dnew = this.ReadByte();
						if (dorg != dnew)
						{
							bSame = false;
							break;
						}
					}
				}
				catch	// ファイルサイズが同じ場合のみtry内に来るため、通常ここには来ないはずだが、念のためbSame=false側に倒しておく
				{
					bSame = false;
				}
			}
			if ( _ms_org != null )
			{
				_ms_org.Close();
				_ms_org.Dispose();
				_ms_org = null;
			}
			this.Seek( 0, SeekOrigin.Begin );

			// 元ファイルと新規ファイルが一致していない場合、新規ファイルで上書きする
			if ( !bSame )
			{
				Trace.TraceInformation( Path.GetFileName( _filename ) + ": 以前のファイルから変化があったため、書き込みを実行します。" );
				using ( _fs = new FileStream( _filename, _mode, _access ) )
				{
					this.CopyTo( _fs );
				}	// _fs will be closed and disposed, by using()
				_fs = null;
			}
			else
			{
				Trace.TraceInformation( Path.GetFileName( _filename ) + ": 以前のファイルから変化がなかったため、書き込みを行いません。" );
			}

			bClosed = true;		// base.Close()の前にフラグ変更のこと。さもないと、無限に再帰実行される
			Dispose();
		}



		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public new void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
			base.Dispose();
		}
		protected override void Dispose( bool disposing)
		{
			if (!this.disposed)
			{
				try
				{
					if (this._ms_org != null)
					{
						this._ms_org.Close();
						this._ms_org.Dispose();
						this._ms_org = null;
					}
					if (this._fs != null)
					{
						this._fs.Close();
						this._fs.Dispose();
						this._fs = null;
					}
					if (!bClosed)
					{
						this.Close();               // Close()なしでDispose()された場合用 (DataContractSerializer経由だと、ここに来る)
					}
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}
		~FileStreamSSD()
		{
			this.Dispose( false );
		}
		#endregion
	}
}
