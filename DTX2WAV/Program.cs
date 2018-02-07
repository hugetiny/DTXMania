using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace DTX2WAV
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			// 以下、DOBON.NETを参照した。mutexを使って二重起動を抑制。
			//Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
			string mutexName = "DTX2WAV";
			//Mutexオブジェクトを作成する
			System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName);

			bool hasHandle = false;
			try
			{
				try
				{
					// mutexの所有権を要求する
					hasHandle = mutex.WaitOne(0, false);
				}
				//.NET Framework 2.0以降の場合
				catch (System.Threading.AbandonedMutexException)
				{
					//別のアプリケーションがmutexを解放しないで終了した時
					hasHandle = true;
				}
				#region [ mutexを得られなければ、既に起動済みと判断して、そっちを最前面に出して、こっちは終了 ]
				if (hasHandle == false)
				{
					//得られなかった場合は、すでに起動していると判断して終了
					//MessageBox.Show("多重起動はできません。");

					// 得られなかった場合は、既に起動していると判断して、
					// 既に起動しているほうを最前面に出す
					Process prevProcess = GetPreviousProcess();
					if (prevProcess != null)
					{
						WakeupWindow(prevProcess.MainWindowHandle);
					}
					return;
				}
				#endregion

				#region [ DTXMania本体が既に起動されていないか確認する。既に起動済みなら警告して終了 ]
				Process dtxmaniaProcess = GetProcessesByFileName("DTXManiaGR.exe");
				if (dtxmaniaProcess != null)
				{
					MessageBox.Show(
						"既にDTXMania本体が起動しています。DTX2WAVを起動する前に、DTXMania本体を終了してください。",
						"DTX2WAV",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation
					);
					return;
				}

				#endregion

				#region [ アプリを通常通り起動する ]
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Main());
				#endregion
			}
			finally
			{
				if (hasHandle)
				{
					// mutexを解放する
					mutex.ReleaseMutex();
				}
				mutex.Close();
			}


		}

		// .NET TIPSより
		// 実行中の同じアプリケーションのプロセスを取得する
		// http://www.atmarkit.co.jp/fdotnet/dotnettips/151winshow/winshow.html
		public static Process GetPreviousProcess()
		{
			Process curProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

			foreach (Process checkProcess in allProcesses)
			{
				// 自分自身のプロセスIDは無視する
				if (checkProcess.Id != curProcess.Id)
				{
					// プロセスのフルパス名を比較して同じアプリケーションか検証
					if (String.Compare(
						 checkProcess.MainModule.FileName,
						 curProcess.MainModule.FileName, true) == 0)
					{
						// 同じフルパス名のプロセスを取得
						return checkProcess;
					}
				}
			}

			// 同じアプリケーションのプロセスが見つからない！
			return null;
		}

		/// <summary>
		/// 指定した実行ファイル名のプロセスを取得する。
		/// https://dobon.net/vb/dotnet/process/getprocessesbyfilename.html
		/// </summary>
		/// <param name="searchFileName">検索する実行ファイル名。</param>
		/// <returns>最初に一致したProcess。</returns>
		public static System.Diagnostics.Process GetProcessesByFileName(string searchFileName)
		{
			searchFileName = searchFileName.ToLower();

			//すべてのプロセスを列挙する
			foreach (System.Diagnostics.Process p
				in System.Diagnostics.Process.GetProcesses())
			{
				string fileName;
				try
				{
					//メインモジュールのパスを取得する
					fileName = p.MainModule.FileName;
				}
				catch (System.ComponentModel.Win32Exception)
				{
					//MainModuleの取得に失敗
					fileName = "";
				}
				if (0 < fileName.Length)
				{
					//ファイル名の部分を取得する
					fileName = System.IO.Path.GetFileName(fileName);
					//探しているファイル名と一致した時、コレクションに追加
					if (searchFileName.Equals(fileName.ToLower()))
					{
						return p;
					}
				}
			}

			return null;
		}


		// .NET TIPSより
		// 外部プロセスのウィンドウを最前面にする
		// http://www.atmarkit.co.jp/fdotnet/dotnettips/151winshow/winshow.html
		public static void WakeupWindow(IntPtr hWnd)
		{
			// メイン・ウィンドウが最小化されていれば元に戻す
			if (IsIconic(hWnd))
			{
				ShowWindowAsync(hWnd, SW_RESTORE);
			}

			// メイン・ウィンドウを最前面に表示する
			SetForegroundWindow(hWnd);
		}
		// 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);
		// ShowWindowAsync関数のパラメータに渡す定義値
		private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す

	}
}
