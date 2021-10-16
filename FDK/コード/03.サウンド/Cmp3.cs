using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Un4seen.Bass;
using Un4seen.Bass.Misc;


namespace FDK
{
	public unsafe class Cmp3 : SoundDecoder
	{
		private int stream_in = -1;
		private bool bBASS_Already_Init = false;

		public override int Open( string filename )
		{
			bBASS_Already_Init = !Bass.BASS_Init(0, 48000, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
			// WASAPI/ASIO使用時は(BASS_ERROR_ALREADYとなって)falseが返るので、覚えておく。
			// 後でCmp3.Close()時にBASSを終了させないようにするため。

			stream_in = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_STREAM_DECODE);
			if (stream_in == 0)
			{
				BASSError be = Bass.BASS_ErrorGetCode();
				Trace.TraceInformation("Cmp3: StreamCreateFile error: " + be.ToString());
				return -1;
			}
			nTotalPCMSize = Bass.BASS_ChannelGetLength(stream_in);

			#region [ Getting WAVEFORMEX info ]
			var chinfo = Bass.BASS_ChannelGetInfo(stream_in);
			int wBitsPerSample = (chinfo.origres==0)? 16 : chinfo.origres;		// for no3, origres might be zero

			wfx = new CWin32.WAVEFORMATEX(
				(ushort)1,								// wFormatTag
				(ushort)chinfo.chans,					// nChannels
				(uint)chinfo.freq,						// nSamplesPerSec
				(uint)(chinfo.freq * chinfo.chans * wBitsPerSample / 8 ),	// nAvgBytesPerSec == SampleRate(freq) * NumChannels(chans) * BitsPerSample/8
				(ushort)(chinfo.chans * wBitsPerSample / 8),				// nBlockAlign== NumChannels * BitsPerSample/8
				(ushort)( wBitsPerSample ),									// wBitsPerSample (8, 16, ...)
				(ushort)0													// cbSize				
			);
			Trace.TraceInformation($"chans={chinfo.chans}, freq={chinfo.freq}, chinfo.origres={chinfo.origres}, BitsPerSample={wBitsPerSample}, nAvgBytePerSec={wfx.nAvgBytesPerSec}, nBlockAlign={wfx.nBlockAlign}");
			Trace.TraceInformation( $"totalPCMSize={nTotalPCMSize}:{nTotalPCMSize.ToString("x8")}" );
			#endregion

			//string fn = Path.GetFileName(filename); 
			//Trace.TraceInformation("filename=" + fn + ", size=(decode): " + wavdata.Length + ", channelgetlength=" + _TotalPCMSize2 + ", " + _TotalPCMSize) ;

			return 0;
		}

		public override int Decode(ref byte[] Dest, long offset)
		{
			#region [ decode ]
			int LEN = 65536;
			byte[] data = new byte[LEN]; // 2 x 16-bit and length in is bytes

			for (int i = 0; i < offset; i++ )
			{
				Dest[ i ] = 0;
			}

			long len = 0;
			long p = offset;
			do
			{
				len = Bass.BASS_ChannelGetData(stream_in, data, LEN);
				if (len < 0)
				{
					BASSError be = Bass.BASS_ErrorGetCode();
					Trace.TraceInformation("Cmp3: BASS_ChannelGetData Error: " + be.ToString());
					break;
				}
				if (p + len > nTotalPCMSize + offset)
				{
					len = nTotalPCMSize - p + offset;
				}
				Array.Copy(data, 0, Dest, p, len);
				p += len;
			} while (p < nTotalPCMSize + offset);
			#endregion

//SaveWav(filename, Dest);

			data = null;
			return 0;
		}

		public override void Close()
		{
			if (!bBASS_Already_Init)
			{
				Bass.BASS_StreamFree(stream_in);
				Bass.BASS_Free();
			}
		}

		/// <summary>
		/// save wav file (for debugging)
		/// </summary>
		/// <param name="filename">input mp3/xa filename</param>
		private void SaveWav(string filename, byte[] Dest)
		{
			string outfile = Path.GetFileName(filename);
			var fs = new FileStream(outfile + ".wav", FileMode.Create);
			var st = new BinaryWriter(fs);

			st.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 });      // 'RIFF'
			st.Write((int)(nTotalPCMSize + 44 - 8));            // RIFF chunk size
			st.Write(new byte[] { 0x57, 0x41, 0x56, 0x45 });      // 'WAVE'
			st.Write(new byte[] { 0x66, 0x6D, 0x74, 0x20 });      // 'fmt '
			st.Write(new byte[] { 0x10, 0x00, 0x00, 0x00 });      // chunk size 16bytes
			st.Write(new byte[] { 0x01, 0x00 }, 0, 2);                  // formatTag 0001 PCM
			st.Write((short)wfx.nChannels);                              // channels
			st.Write((int)wfx.nSamplesPerSec);                             // samples per sec
			st.Write((int)wfx.nAvgBytesPerSec);          // avg bytesper sec
			st.Write((short)wfx.nBlockAlign);                        // blockalign = 16bit * mono/stereo
			st.Write((short)wfx.wBitsPerSample);                  // bitspersample = 16bits

			st.Write(new byte[] { 0x64, 0x61, 0x74, 0x61 });      // 'data'
			st.Write((int) nTotalPCMSize);      // datasize 
			
			st.Write(Dest);
Trace.TraceInformation($"wrote ({outfile}.wav) fsLength=" + fs.Length + ", TotalPCMSize=" + nTotalPCMSize + ", diff=" + (fs.Length - nTotalPCMSize));
			st.Dispose();
			fs.Dispose();
		}
	}
}
