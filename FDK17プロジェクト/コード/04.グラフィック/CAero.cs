using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using SlimDX;

namespace FDK
{
	//http://spazzarama.wordpress.com/2009/09/30/enable-or-disable-dwm-composition-aero/
	/// <summary>
	/// To control Aerograph in Vista and Windows7. Don't use this class in XP.
	/// </summary>
	public static class DWM
	{
		const uint DWM_EC_DISABLECOMPOSITION = 0;
		const uint DWM_EC_ENABLECOMPOSITION = 1;

		[DllImport( "dwmapi.dll", EntryPoint = "DwmEnableComposition" )]
		extern static uint DwmEnableComposition( uint compositionAction );
//		[DllImport( "dwmapi.dll", EntryPoint = "DwmEnableMMCSS" )]
//		extern static int DwmEnableMMCSS( bool fEnableMMCSS );

		/// <summary>  
		/// Enable/Disable DWM composition (aka Aero)  
		/// </summary>  
		/// <param name="bIsAeroEnabled">True to enable composition, false to disable composition.</param>  
		/// <returns>True if the operation was successful.</returns>  
		public static bool EnableComposition( bool bIsAeroEnabled )
		{
//			DwmEnableMMCSS( true );
#if TEST_Direct3D9Ex
			try
			{
				if ( bIsAeroEnabled )
				{
					DwmEnableComposition( DWM_EC_ENABLECOMPOSITION );
				}
				else
				{
					DwmEnableComposition( DWM_EC_DISABLECOMPOSITION );
				}
				return true;
			}
			catch ( DllNotFoundException )
			{
				return false;
			}
#else
			return true;
#endif
		}	
	}
}

