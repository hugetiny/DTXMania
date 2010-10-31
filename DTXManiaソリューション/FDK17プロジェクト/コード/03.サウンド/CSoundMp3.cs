using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CSoundMp3 : CSound
	{
		// CSound 実装

		protected override int Open( string filename )
		{
			return mp3Open( filename );
		}
		protected override int GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx )
		{
			return mp3GetFormat( nHandle, ref wfx );
		}
		protected override uint GetTotalPCMSize( int nHandle )
		{
			return mp3GetTotalPCMSize( nHandle );
		}
		protected override int Seek( int nHandle, uint dwPosition )
		{
			return mp3Seek( nHandle, dwPosition );
		}
		protected override int Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop )
		{
			return mp3Decode( nHandle, pDest, szDestSize, bLoop );
		}
		protected override void Close( int nHandle )
		{
			mp3Close( nHandle );
		}

		#region [ SoundDecoder.dll インポート（mp3 関連）]
		//-----------------
		[DllImport( "SoundDecoder.dll" )]
		private static extern void mp3Close( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int mp3Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int mp3GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx );
		[DllImport( "SoundDecoder.dll" )]
		private static extern uint mp3GetTotalPCMSize( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int mp3Open( string fileName );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int mp3Seek( int nHandle, uint dwPosition );
		//-----------------
		#endregion
	}
}
