using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Win32;

namespace FDK
{
	/// <summary>
	/// Dark theme suuport class
	/// </summary>
	/// <remarks>
	/// http://grabacr.net/archives/6671
	/// </remarks>
	public class CWindowsTheme
    {
		protected internal static class SafeNativeMethod
		{
			[DllImport("Dwmapi.dll")]
			public static extern void DwmGetColorizationColor([Out] out int pcrColorization, [Out] out bool pfOpaqueBlend);
		}

		/// <summary>
		/// Get Current Accent Color
		/// </summary>
		/// <returns>Accent Color (ARGB)</returns>
		public Color AccentColor
		{
			get
			{
				SafeNativeMethod.DwmGetColorizationColor(out int pcrColorization, out bool pfOpaqueBlend);
				Trace.TraceInformation($"AccentColor: Color={pcrColorization.ToString("X4")}, Opaque={pfOpaqueBlend}");
				return GetColorFromInt(pcrColorization);
			}
		}
		public Color GetColorFromInt(int color)
		{
			return Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
		}


		/// <summary>
		/// Get Current Fontcolor.
		/// </summary>
		/// <remarks>
		/// It should be depends on the AccentColor. Need Improvement.
		/// </remarks>
		public Color FontColor
		{
			get
			{
				// it should be depends on the AccentColor. Need Improvement.
				if (AppMode == EAppMode.Dark)
				{
					return Color.White;
				}
				else
				{
					return Color.Black;
				}
			}
		}
		/// <summary>
		/// Get Current Backgroundcolor.
		/// </summary>
		/// <remarks>
		/// It should be depends on the AccentColor. Need Improvement.
		/// </remarks>
		public Color BackgroundColor
		{
			get
			{
				// it should be depends on the AccentColor. Need Improvement.
				if (AppMode == EAppMode.Dark)
				{
					return Color.Black;
				}
				else
				{
					return Color.White;
				}
			}
		}

		public enum EAppMode:int
		{
			NotSupported = -1,
			Dark  = 0,
			Light = 1
		}

		/// <summary>
		/// Get Current App Theme (Dark/Light) from registry
		/// </summary>
		public EAppMode AppMode
		{
			get
			{
				#region [If OS doesn't support Dark Theme, return "Not Supported"]
				if (!COS.bIsWin10OrLater())
				{
					return EAppMode.NotSupported;
				}
				if (COS.GetWin10BuildNumber() < COS.WIN10BUILD.RS1)
				{
					return EAppMode.NotSupported;
				}
				#endregion

				#region [Get app theme value from registry]
				string rKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
				string rGetValueName = "AppsUseLightTheme";
				int AppsUseLightTheme = (int)EAppMode.Light;
				try
				{
					RegistryKey rKey = Registry.CurrentUser.OpenSubKey(rKeyName);
					AppsUseLightTheme = (int)rKey.GetValue(rGetValueName);
					rKey.Close();

					Trace.TraceInformation($"Current App Theme = {AppsUseLightTheme}");
				}
				catch (NullReferenceException)
				{
					Trace.TraceWarning($"Warning: No registry key {rKeyName} in {rGetValueName}. Light Theme is assumed.");
					// AppsUseLightTheme = (int)EAppMode.Light;
				}
				#endregion

				return (EAppMode)AppsUseLightTheme;
			}
			//HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize
			//AppsUseLightTheme
		}
#if false
if (msg == (int)WindowsMessages.WM_SETTINGCHANGE)
{
    var systemParmeter = Marshal.PtrToStringAuto(lParam);
    if (systemParmeter == "ImmersiveColorSet")
    {
        // 再度レジストリから Dark/Light をとってくるとか
 
        handled = true;
    }
}
#endif

	}

}
