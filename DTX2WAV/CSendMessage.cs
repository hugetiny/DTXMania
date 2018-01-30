using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DTX2WAV
{
	public static class CSendMessage
	{
		#region #28821 2014.1.23 yyagi add: 外部からの文字列メッセージ送受信 定数定義
		[StructLayout(LayoutKind.Sequential)]
		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public UInt32 cbData;
			public IntPtr lpData;
		}
		#endregion
		public const int WM_COPYDATA = 0x004A;

		[DllImport("USER32.dll")]
		static extern uint SendMessage(IntPtr window, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

		/// <summary>
		/// 別のウインドウにメッセージを送信する
		/// DTXCreatorで使っているコードをほぼそのまま流用(FDK依存をなくしただけ)
		/// </summary>
		/// <param name="MainWindowHandle"></param>
		/// <param name="FromWindowHandle"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static uint sendmessage(IntPtr MainWindowHandle, IntPtr FromWindowHandle, string arg)
		{
			uint len = (uint)arg.Length;

			COPYDATASTRUCT cds;
			cds.dwData = IntPtr.Zero;       // 使用しない
			cds.lpData = Marshal.StringToHGlobalUni(arg);           // テキストのポインターをセット
			cds.cbData = (len + 1) * 2; // 長さをセット

			//文字列を送る
			uint result = SendMessage(MainWindowHandle, WM_COPYDATA, FromWindowHandle, ref cds);

			Marshal.FreeHGlobal(cds.lpData);

			return result;
		}
	}


	/// <summary>
	/// 起動中のDTXMania本体にメッセージを送信する
	/// DTXManiaのProgram.cs内のロジックを一部修正して流用
	/// </summary>
	public static class CSendMessageToDTXMania
	{
		public static void SendMessage(string strSend)
		{
			for (int i = 0; i < 5; i++)   // 検索結果のハンドルがZeroになることがあるので、200ms間隔で5回リトライする
			{
				#region [ 既に起動中のDTXManiaプロセスを検索する。]
				Process current = Process.GetCurrentProcess();
				Process target = null;

				Process[] running = Process.GetProcesses();
				foreach (Process p in running)
				{
					if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle.Contains("DTX2WAV ("))
					{
						target = p;
						break;
					}
				}
				#endregion

				#region [ 起動中のDTXManiaがいれば、そのプロセスにコマンドラインを投げる ]
				if (target != null &&  strSend != null)
				{
					CSendMessage.sendmessage(target.MainWindowHandle, current.MainWindowHandle, strSend);
				}
				#endregion
				else
				{
					Trace.TraceInformation("メッセージ送信先のプロセスが見つからず。5回リトライします。");
					Thread.Sleep(200);
				}
			}
		}
	}
}
