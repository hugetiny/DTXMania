using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CSoundOggVorbis : CSound
	{
		// CSound 実装

		protected override int Open( string filename )
		{
			return oggOpen( filename );
		}
		protected override int GetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx )
		{
			return oggGetFormat( nHandle, ref wfx );
		}
		protected override uint GetTotalPCMSize( int nHandle )
		{
			return oggGetTotalPCMSize( nHandle );
		}
		protected override int Seek( int nHandle, uint dwPosition )
		{
			return oggSeek( nHandle, dwPosition );
		}
		protected override int Decode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop )
		{
			return oggDecode( nHandle, pDest, szDestSize, bLoop );
		}
		protected override void Close( int nHandle )
		{
			oggClose( nHandle );
		}

		#region [ SoundDecoder.dll インポート（ogg 関連）]
		//-----------------
		[DllImport( "SoundDecoder.dll" )]
		private static extern void oggClose( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int oggDecode( int nHandle, IntPtr pDest, uint szDestSize, int bLoop );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int oggGetFormat( int nHandle, ref CWin32.WAVEFORMATEX wfx );
		[DllImport( "SoundDecoder.dll" )]
		private static extern uint oggGetTotalPCMSize( int nHandle );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int oggOpen( string fileName );
		[DllImport( "SoundDecoder.dll" )]
		private static extern int oggSeek( int nHandle, uint dwPosition );
		//-----------------
		#endregion
	}
}
