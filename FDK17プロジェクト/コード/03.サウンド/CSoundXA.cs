using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CSoundXA : CSound
	{
		// CSound 実装

		protected override int Open( string filename )
		{
			return xaOpen( filename );
		}
		protected override int GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx )
		{
			return xaGetFormat( nHandle, ref wfx );
		}
		protected override uint GetTotalPCMSize( int nHandle )
		{
			return xaGetTotalPCMSize( nHandle );
		}
		protected override int Seek( int nHandle, uint dwPosition )
		{
			return xaSeek( nHandle, dwPosition );
		}
		protected override int Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop )
		{
			return xaDecode( nHandle, pDest, szDestSize, bLoop );
		}
		protected override void Close( int nHandle )
		{
			xaClose( nHandle );
		}

		#region [ SoundDecoder.dll インポート（XA 関連）]
		//-----------------
		[DllImport( "SoundDecoder.dll" )]
		private static extern void xaClose( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int xaDecode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int xaGetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx );
		[DllImport( "SoundDecoder.dll" )]
		private static extern uint xaGetTotalPCMSize( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int xaOpen( string fileName );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int xaSeek( int nHandle, uint dwPosition );
		//-----------------
		#endregion
	}
}
