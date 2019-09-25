using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DTXCreator
{
	internal static class Program
	{
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool SetDllDirectory(string lpPathName);

		[STAThread]
		private static void Main()
		{
#if DEBUG
#if USE_ENGLISHRESOURCE
			Thread.CurrentThread.CurrentUICulture = new CultureInfo( "en-GB", false );	// yyagi; For testing English resources
#elif USE_GERMANRESOURCE
			Thread.CurrentThread.CurrentCulture = new CultureInfo( "de-DE", false );	// yyagi; For testing decimal point in German resources
#endif
#endif
			string path = Path.GetDirectoryName(Application.ExecutablePath);

			SetDllDirectory(null);
			if (Environment.Is64BitProcess)
			{
				SetDllDirectory(Path.Combine(path, @"dll\x64"));
			}
			else
			{
				SetDllDirectory(Path.Combine(path, @"dll"));
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new Cメインフォーム() );
		}
	}
}
