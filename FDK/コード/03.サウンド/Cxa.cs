using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace FDK
{
	public unsafe class Cxa : SoundDecoder    //, IDisposable
	{
		//static byte[] FOURCC = Encoding.ASCII.GetBytes("1DWK"); // KWD1 (little endian)

		public CWin32.WAVEFORMATEX waveformatex;

		private string filename;
		private byte[] srcBuf = null;
		private short[] pcmbuf = null;

		//private int nHandle = -1;
		private bjxa.Decoder bjxa = new bjxa.Decoder();
		private bjxa.Format format = null;
		private FileStream fs;

		public override int Open(string filename)
		{
			this.filename = filename;

			#region [ Reading XA headers, then store it ]
			fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);   // Need to set FileShare flag, to avoid locking after Closer()
			format = bjxa.ReadHeader(fs);
			//string xaid = Encoding.ASCII.GetString( xah.id );

			#region [ Debug info ]
			//Debug.WriteLine( "**XAHEADER**" );
			//Debug.WriteLine( "id=             " + xaheader.id.ToString( "X8" ) );
			//Debug.WriteLine( "nDataLen=       " + xaheader.nDataLen.ToString( "X8" ) );
			//Debug.WriteLine( "nSamples=       " + xaheader.nSamples.ToString( "X8" ) );
			//Debug.WriteLine( "nSamplesPerSec= " + xaheader.nSamplesPerSec.ToString( "X4" ) );
			//Debug.WriteLine( "nBits=          " + xaheader.nBits.ToString( "X2" ) );
			//Debug.WriteLine( "nChannels=      " + xaheader.nChannels.ToString( "X2" ) );
			//Debug.WriteLine( "nLoopPtr=       " + xaheader.nLoopPtr.ToString( "X8" ) );
			//Debug.WriteLine( "befL[0]=        " + xaheader.befL[ 0 ].ToString( "X4" ) );
			//Debug.WriteLine( "befL[1]=        " + xaheader.befL[ 1 ].ToString( "X4" ) );
			//Debug.WriteLine( "befR[0]=        " + xaheader.befR[ 0 ].ToString( "X4" ) );
			//Debug.WriteLine( "befR[1]=        " + xaheader.befR[ 1 ].ToString( "X4" ) );
			#endregion
			#endregion


			#region [ Getting WAVEFORMEX info ]
			waveformatex = new CWin32.WAVEFORMATEX();

			waveformatex.wFormatTag = (ushort)format.WaveFormatPcm;
			waveformatex.nChannels = (ushort)format.Channels;
			waveformatex.nSamplesPerSec = format.SamplesRate;
			waveformatex.nAvgBytesPerSec = format.WaveByteRate;
			waveformatex.nBlockAlign = (ushort)format.WaveBlockAlign;
			waveformatex.wBitsPerSample = (ushort)format.SampleBits;
			waveformatex.cbSize = 0;

			#region [ Debug info ]
			//Debug.WriteLine( "**WAVEFORMATEX**" );
			//Debug.WriteLine( "wFormatTag=      " + waveformatex.wFormatTag.ToString( "X4" ) );
			//Debug.WriteLine( "nChannels =      " + waveformatex.nChannels.ToString( "X4" ) );
			//Debug.WriteLine( "nSamplesPerSec=  " + waveformatex.nSamplesPerSec.ToString( "X8" ) );
			//Debug.WriteLine( "nAvgBytesPerSec= " + waveformatex.nAvgBytesPerSec.ToString( "X8" ) );
			//Debug.WriteLine( "nBlockAlign=     " + waveformatex.nBlockAlign.ToString( "X4" ) );
			//Debug.WriteLine( "wBitsPerSample=  " + waveformatex.wBitsPerSample.ToString( "X4" ) );
			//Debug.WriteLine( "cbSize=          " + waveformatex.cbSize.ToString( "X4" ) );
			#endregion
			#endregion

			return 0x7FFFFFFF;  // Anyway, return non-zero value
		}
		public override int GetFormat(int nHandle, ref CWin32.WAVEFORMATEX wfx)
		{
			#region [ Copying WAVEFORMATEX structure data ]
			wfx.nAvgBytesPerSec = waveformatex.nAvgBytesPerSec;
			wfx.wBitsPerSample = waveformatex.wBitsPerSample;
			wfx.nBlockAlign = waveformatex.nBlockAlign;
			wfx.nChannels = waveformatex.nChannels;
			wfx.wFormatTag = waveformatex.wFormatTag;
			wfx.nSamplesPerSec = waveformatex.nSamplesPerSec;

			return 0;
			#endregion
		}
		public override uint GetTotalPCMSize(int nHandle)
		{
			#region [ Getting PCM length ]
			uint dlen = (uint)format.DataLengthPcm;
			#region [ Debug info ]
			//Debug.WriteLine( "**INTERNAL VALUE**" );
			//Debug.WriteLine( "dlen=          " + dlen );
			#endregion
			#endregion

			return dlen;
		}
		public override int Seek(int nHandle, uint dwPosition)
		{
			return 0;
		}
		public override int Decode(int nHandle, IntPtr pDest, uint szDestSize, int bLoop)
		{
			#region [ Decodig xa data ]
			srcBuf = new byte[format.Blocks * format.BlockSizeXa];
			pcmbuf = new short[format.Blocks * format.BlockSizePcm];

			if (fs.Read(srcBuf, 0, srcBuf.Length) != srcBuf.Length)
			{
				string s = Path.GetFileName(filename);
				throw new Exception( $"Failed to load xa data: {s}");
			}
			int ret = bjxa.Decode(srcBuf, pcmbuf, out long pcmBufLength);

			Marshal.Copy(pcmbuf, 0, pDest, (int)format.DataLengthPcm/2);

			#region [ alternative copy way ]
			// if you can't use Marshal.Copy, try this.
			//unsafe
			//{
			//	short* pWavBuf = (short*)pDest;
			//	for (int i = 0; i < format.DataLengthPcm/2; i++)
			//	{
			//		*pWavBuf++ = pcmbuf[i];
			//	}
			//}
			#endregion

			//string shortpath = Path.GetFileName(filename);
			//Trace.TraceInformation($"libbjxa: decode succeeded: {shortpath} = {szDestSize}");

//SaveWav(filename);

			pcmbuf = null;

			#region [ Debug info ]
			//Debug.WriteLine( "**XASTREAMHEADER**" );
			//Debug.WriteLine( "nSrcLen=  " + xastreamheader.nSrcLen );
			//Debug.WriteLine( "nSrcUsed= " + xastreamheader.nSrcUsed );
			//Debug.WriteLine( "nDstLen=  " + xastreamheader.nDstLen );
			//Debug.WriteLine( "nDstUsed= " + xastreamheader.nDstUsed );
			#endregion
			#endregion

			return 0;
		}
		public override void Close(int nHandle)
		{
			srcBuf = null;
			fs.Close();
		}




		private void SaveWav(string filename)
		{
			long _TotalPCMSize = (uint)format.DataLengthPcm;
			CWin32.WAVEFORMATEX _wfx = waveformatex;

			string outfile = Path.GetFileName(filename);
			var fs2 = new FileStream(outfile + ".wav", FileMode.Create);
			var st = new BinaryWriter(fs2);

			st.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 }, 0, 4);      // 'RIFF'
			st.Write((int)_TotalPCMSize + 44 - 8);      // filesize - 8 [byte]；今は不明なので後で上書きする。
			st.Write(new byte[] { 0x57, 0x41, 0x56, 0x45 }, 0, 4);      // 'WAVE'
			st.Write(new byte[] { 0x66, 0x6D, 0x74, 0x20 }, 0, 4);      // 'fmt '
			st.Write(new byte[] { 0x10, 0x00, 0x00, 0x00 }, 0, 4);      // chunk size 16bytes
			st.Write(new byte[] { 0x01, 0x00 }, 0, 2);                  // formatTag 0001 PCM
			st.Write((short)_wfx.nChannels);                              // channels
			st.Write((int)_wfx.nSamplesPerSec);                             // samples per sec
			st.Write((int)_wfx.nAvgBytesPerSec);          // avg bytesper sec
			st.Write((short)_wfx.nBlockAlign);                        // blockalign = 16bit * mono/stereo
			st.Write((short)_wfx.wBitsPerSample);                  // bitspersample = 16bits

			st.Write(new byte[] { 0x64, 0x61, 0x74, 0x61 }, 0, 4);      // 'data'
			st.Write((int)_TotalPCMSize);      // datasize 


			//var pcmbuf = new short[format.Blocks * format.BlockSizePcm];
			//if (fs.Read(srcBuf, 0, srcBuf.Length) != srcBuf.Length)
			//{
			//	string s = Path.GetFileName(filename);
			//	throw new Exception($"Failed to load xa data: {s}");
			//}
			//int ret = bjxa.Decode(srcBuf, pcmbuf, out long pcmBufLength);

			int shortsize = (int)(format.Blocks * format.BlockSizePcm);
			var pcmbuf_bytes = new byte[shortsize * 2];
			for (int i = 0; i < shortsize; i++)
			{
				var b = BitConverter.GetBytes(pcmbuf[i]);
				pcmbuf_bytes[i * 2] = b[0];
				pcmbuf_bytes[i * 2 + 1] = b[1];
			}
			st.Write(pcmbuf_bytes);
			Trace.TraceInformation($"wrote ({outfile}.wav) " + fs2.Length);
			st.Dispose();
			fs2.Dispose();
		}

		#region [ IDisposable implementatitons ]
		//-----------------
		private bool bDisposed = false;

		// Public implementation of Dispose pattern callable by consumers.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing)
		{
			if (this.bDisposed)
				return;

			if (srcBuf != null) srcBuf = null;
			if (bjxa != null) bjxa = null;

			if (disposing)
			{
				fs.Dispose();
			}

			this.bDisposed = true;
		}

		//-----------------
		#endregion
	}
}
