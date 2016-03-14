using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FDK
{
	/// <summary>
	/// 電源プランの制御を行う
	/// </summary>
	public class CPowerPlan
	{
		// 参考: 電源プラン制御: http://www.fsmpi.uni-bayreuth.de/~dun3/archives/programmatically-change-power-options-using-cshar/519.html
		// 参考: ConnectedStandby判別: https://social.msdn.microsoft.com/Forums/en-US/eeb164a3-8ceb-4eb2-8768-4faaa7218c59/how-to-experimentally-confirm-that-connected-standby-mode-is-enabled-on-a-computer-system?forum=tailoringappsfordevices
		//                             http://stackoverflow.com/questions/20407094/c-sharp-how-to-use-callntpowerinformation-with-interop-to-get-system-power-infor

		readonly private Guid GuidHighPerformance = new Guid( "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c" );		// Vista以降は全部これのはず
		private Guid GuidBackup;
		private bool bConnectedStandbySupported = false;




		public CPowerPlan()
		{
			GuidBackup = System.Guid.Empty;
		}


		public void BackupCurrentPowerPlan()
		{
			bConnectedStandbySupported = IsConnetedStandbySupported();
			
			if ( bConnectedStandbySupported )
			{
				// 何もしない
			}
			else
			{
				GuidBackup = GetActivePowerPlan();
				Trace.TraceInformation( "現在の電源プラン「{0}」をバックアップしました。", GetFriendlyName( GuidBackup ) );
			}
		}

		/// <summary>
		/// Connected Standbyサポート機種かどうかの判定
		/// (Connected Standby機種に対しては、電源プラン操作を行わない)
		/// </summary>
		private static bool IsConnetedStandbySupported()
		{
			if ( !COS.bIsWin8OrLater )
			{
				// Win8以前であれば、ConnectedStandby非サポートが確定
				return false;
			}

			CWin32.SYSTEM_POWER_CAPABILITIES cap;
			uint retval = CWin32.CallNtPowerInformation(
				(int) CWin32.POWER_INFORMATION_LEVEL.SystemPowerCapabilities,
				IntPtr.Zero,
				0,
				out cap,
				Marshal.SizeOf( typeof( CWin32.SYSTEM_POWER_CAPABILITIES ) )
			);
			if ( retval == 0 )
			{
				Debug.WriteLine( "SYSTEM_POWER_CAPABILITIES.AOAC: " + cap.AoAc );
				if ( cap.AoAc )
				{
					Debug.WriteLine( "Connected Standby is enabled." );
					return true;
				}
				else
				{
					Debug.WriteLine( "Connected Standby is NOT enabled." );
					return false;
				}
			}
			else
			{
				Debug.WriteLine( "CallNtPowerInformation returned: " + retval );
				//Debug.WriteLine( "Call to CallNTPowerInformation failed. GetLastError: %d\n", GetLastError() );
				return false;
			}
		}

		public void RestoreCurrentPowerPlan()
		{
			if ( bConnectedStandbySupported )
			{
				// 何もしない
			}
			else
			{
				if ( GuidBackup != System.Guid.Empty )
				{
					SetActivePowerPlan( GuidBackup );
					Trace.TraceInformation( "電源プランを、「{0}」に戻しました。", GetFriendlyName( GuidBackup ) );
					GuidBackup = System.Guid.Empty;
				}
			}
		}
		public void ChangeHighPerformance()
		{
			if ( bConnectedStandbySupported )
			{
				Trace.TraceInformation( "ConnectedStandby対応機種のため、電源プランの変更を行いません。" );
			}
			else
			{
				SetActivePowerPlan( GuidHighPerformance );
				Trace.TraceInformation( "電源プランを、「{0}」に変更しました。", GetFriendlyName( GuidHighPerformance ) );
			}
		}



		private void SetActivePowerPlan( Guid powerSchemeId )
		{
			var schemeGuid = powerSchemeId;
			CWin32.PowerSetActiveScheme( IntPtr.Zero, ref schemeGuid );
		}

		private Guid GetActivePowerPlan()
		{
			IntPtr pCurrentSchemeGuid = IntPtr.Zero;
			CWin32.PowerGetActiveScheme( IntPtr.Zero, ref pCurrentSchemeGuid );
			var currentSchemeGuid = (Guid) Marshal.PtrToStructure( pCurrentSchemeGuid, typeof( Guid ) );
			return currentSchemeGuid;
		}


		private IEnumerable<Guid> FindAll()
		{
			var schemeGuid = Guid.Empty;
			uint sizeSchemeGuid = (uint) Marshal.SizeOf( typeof( Guid ) );
			uint schemeIndex = 0;
			while ( CWin32.PowerEnumerate( IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint) CWin32.AccessFlags.ACCESS_SCHEME,
				schemeIndex, ref schemeGuid, ref sizeSchemeGuid ) == 0 )
			{
				yield return schemeGuid;
				schemeIndex++;
			}
		}

		private string GetFriendlyName( Guid schemeGuid )
		{
			uint sizeName = 1024;
			IntPtr pSizeName = Marshal.AllocHGlobal( (int) sizeName );
			string friendlyName;

			try
			{
				CWin32.PowerReadFriendlyName( IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName );
				friendlyName = Marshal.PtrToStringUni( pSizeName );
			}
			finally
			{
				Marshal.FreeHGlobal( pSizeName );
			}
			return friendlyName;
		}
	}
}
