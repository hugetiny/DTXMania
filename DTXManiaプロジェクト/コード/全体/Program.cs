using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace DTXMania
{
	internal class Program
	{
		#region [ 二重機動チェック、DLL存在チェック ]
		//-----------------------------
		private static Mutex mutex二重起動防止用;

		private static bool tDLLの存在チェック( string strDll名, string str存在しないときに表示するエラー文字列 )
		{
			IntPtr hModule = LoadLibrary( strDll名 );
			if( hModule == IntPtr.Zero )
			{
				MessageBox.Show( str存在しないときに表示するエラー文字列, "DTXMania runtime error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
				return false;
			}
			FreeLibrary( hModule );
			return true;
		}

		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern void FreeLibrary( IntPtr hModule );

		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern IntPtr LoadLibrary( string lpFileName );
		//-----------------------------
		#endregion

		[STAThread]
		private static void Main()
		{
			mutex二重起動防止用 = new Mutex( false, "DTXManiaMutex" );

			if( mutex二重起動防止用.WaitOne( 0, false ) )
			{
				string newLine = Environment.NewLine;
				bool flag = false;

				if( !tDLLの存在チェック( "FDK.dll", "FDK.dll またはその依存するdllが存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。" ) ) flag = true;
				if( !tDLLの存在チェック( "SlimDX" + CDTXMania.SLIMDXDLL, "SlimDX" + CDTXMania.SLIMDXDLL + ".dll またはその依存するdllが存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。" ) ) flag = true;
				if( !tDLLの存在チェック( "xadec.dll", "xadec.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。" ) ) flag = true;
				if( !tDLLの存在チェック( "SoundDecoder.dll", "SoundDecoder.dll またはその依存するdllが存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。" ) ) flag = true;
				if( !tDLLの存在チェック( CDTXMania.D3DXDLL, CDTXMania.D3DXDLL + " が存在しません。" + newLine + "Direct Regist フォルダの DXSETUP.exe を実行し、" + newLine + "必要な DirectX ランタイムをインストールしてください。" ) ) flag = true;
				if( !flag )
				{
					using( var mania = new CDTXMania() )
						mania.Run();

					Trace.WriteLine( "" );
					Trace.WriteLine( "遊んでくれてありがとう！" );
					
					if( Trace.Listeners.Count > 1 )
						Trace.Listeners.RemoveAt( 1 );
				}
			}
		}
	}
}
