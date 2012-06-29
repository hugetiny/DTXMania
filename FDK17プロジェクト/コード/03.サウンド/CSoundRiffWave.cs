using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CSoundRiffWave : CSound
	{
		// CSound 実装

		protected override int Open( string filename )
		{
			return riffOpen( filename );
		}
		protected override int GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx )
		{
			return riffGetFormat( nHandle, ref wfx );
		}
		protected override uint GetTotalPCMSize( int nHandle )
		{
			return riffGetTotalPCMSize( nHandle );
		}
		protected override int Seek( int nHandle, uint dwPosition )
		{
			return riffSeek( nHandle, dwPosition );
		}
		protected override int Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop )
		{
			return riffDecode( nHandle, pDest, szDestSize, bLoop );
		}
		protected override void Close( int nHandle )
		{
			riffClose( nHandle );
		}

		#region [ SoundDecoder.dll インポート（RiffWave 関連）]
		//-----------------
		[DllImport( "SoundDecoder.dll" )]
		private static extern void riffClose( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int riffDecode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int riffGetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx );
		[DllImport( "SoundDecoder.dll" )]
		private static extern uint riffGetTotalPCMSize( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int riffOpen( string fileName );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int riffSeek( int nHandle, uint dwPosition );
		//-----------------
		#endregion
	}
}
