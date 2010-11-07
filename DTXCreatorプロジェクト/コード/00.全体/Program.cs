using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DTXCreator
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new Cメインフォーム() );
		}
	}
}
