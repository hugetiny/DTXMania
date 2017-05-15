using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public static class COS
	{
		/// <summary>
		/// OSがXP以前ならfalse, Vista以降ならtrueを返す
		/// </summary>
		/// <returns></returns>
		public static bool bIsVistaOrLater
		{
			get
			{
				//プラットフォームの取得
				System.OperatingSystem os = System.Environment.OSVersion;
				if ( os.Platform != PlatformID.Win32NT )		// NT系でなければ、XP以前か、PC Windows系以外のOSのため、Vista以降ではない。よってfalseを返す。
				{
					return false;
				}

				if ( os.Version.Major >= 6 )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		/// <summary>
		/// OSがVista以前ならfalse, Win7以降ならtrueを返す
		/// </summary>
		/// <returns></returns>
		public static bool bIsWin7OrLater
		{
			get
			{
				//プラットフォームの取得
				System.OperatingSystem os = System.Environment.OSVersion;
				if ( os.Platform != PlatformID.Win32NT )		// NT系でなければ、XP以前か、PC Windows系以外のOSのため、Win7以降ではない。よってfalseを返す。
				{
					return false;
				}

				if ( os.Version.Major >= 6 && os.Version.Minor >= 1 )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		/// <summary>
		/// OSがWin7以前ならfalse, Win8以降ならtrueを返す
		/// </summary>
		/// <returns></returns>
		public static bool bIsWin8OrLater
		{
			get
			{
				//プラットフォームの取得
				System.OperatingSystem os = System.Environment.OSVersion;
				if ( os.Platform != PlatformID.Win32NT )		// NT系でなければ、XP以前か、PC Windows系以外のOSのため、Win8以降ではない。よってfalseを返す。
				{
					return false;
				}

				if ( os.Version.Major >= 6 && os.Version.Minor >= 2 )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
