using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace FDK
{
	public class Cogg : SoundDecoder
	{
		private NVorbis.VorbisReader vorbis = null;

		public override int Open( string filename )
		{
			try
			{
				if (!File.Exists(filename))
				{
					throw new FileNotFoundException( $"Cogg.Open(): no target file exist: {filename}");
				}
				vorbis = new NVorbis.VorbisReader(filename);

				#region [ Getting WAVEFORMEX info ]
				wfx = new CWin32.WAVEFORMATEX(
					(ushort) 0x0003,						// wFormatTag == WAVE_FORMAT_IEEE_FLOAT (0x0003)
					(ushort) vorbis.Channels,				// nChannels
					(uint)   vorbis.SampleRate,				// nSamplesPerSec
					(uint)  (vorbis.SampleRate * 4 * vorbis.Channels), // nAvgBytesPerSec
					(ushort)(4 * vorbis.Channels),			// nBlockAlign
					32,										// wBitsPerSample
					0										// cbSize				
				);
				#endregion

				nTotalPCMSize = vorbis.TotalSamples * 4 * 2;
			}
			#region [ Catch ]
			catch (InvalidDataException e)
			{
				throw new InvalidDataException($"Cogg.Open({Path.GetFileName(filename)}): {e.ToString()}");
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new ArgumentOutOfRangeException($"Cogg.Open({Path.GetFileName(filename)}): {e.ToString()}");
			}
			catch (ObjectDisposedException e)
			{
				throw new ObjectDisposedException($"Cogg.Open({Path.GetFileName(filename)}): {e.ToString()}");
			}
			#endregion

			if (vorbis == null)
			{
				Trace.TraceInformation($"Cogg: Failed to Cogg.Open():{Path.GetFileName(filename)} ");
			}

			//string fn = Path.GetFileName(filename); 
			//Trace.TraceInformation("filename=" + fn + ", size=(decode): " + wavdata.Length + ", channelgetlength=" + _TotalPCMSize2 + ", " + _TotalPCMSize) ;

			return 0;
		}

		public override int Decode( ref byte[] Dest, long offset )
		{
			#region [ decode ]

			// create a buffer for reading samples
			var readBuffer = new float[wfx.nAvgBytesPerSec * 5];  // 5sec buffer

			// go grab samples
			int cnt;
			int p = 0;
			while ((cnt = vorbis.ReadSamples(readBuffer, 0, readBuffer.Length)) > 0)
			{
				bool bEnd = false;
				// do stuff with the buffer
				// samples are interleaved (chan0, chan1, chan0, chan1, etc.)
				// sample value range is -0.99999994f to 0.99999994f unless vorbis.ClipSamples == false

				if (Dest.Length < p + cnt * 4)
				{
					cnt = (Dest.Length - p) / 4;
					bEnd = true;
				}
				Buffer.BlockCopy(readBuffer, 0, Dest, p, cnt * 4);
				p += cnt * 4;
				if (bEnd) break;
			}
			#endregion

			//SaveWav(filename, Dest);

			readBuffer = null;
			return 0;
		}

		public override void Close()
		{
			if (vorbis != null)
			{
				vorbis.Dispose();
				vorbis = null;
			}
		}

		/// <summary>
		/// save wav file (for debugging)
		/// </summary>
		/// <param name="filename">input ogg filename</param>
		private void SaveWav(string filename, byte[] Dest)
		{
			string outfile = Path.GetFileName(filename);
			var fs = new FileStream(outfile + ".tmp.wav", FileMode.Create, FileAccess.Write, FileShare.Write);
			var st = new BinaryWriter(fs);

			st.Write((uint) 0x46464952);				// 'RIFF'
			st.Write((uint) (nTotalPCMSize + 44 - 8));	// filesize
			st.Write((uint) 0x45564157);				// 'WAVE'
			st.Write((uint) 0x20746D66);				// 'fmt '
			st.Write((uint) 16);						// chunk size 16bytes
			st.Write((ushort) 0x03);					// wFormatTag == WAVE_FORMAT_IEEE_FLOAT (0x0003)
			st.Write((ushort) wfx.nChannels);			// channels
			st.Write((uint)   wfx.nSamplesPerSec);		// samples per sec
			st.Write((uint)   wfx.nAvgBytesPerSec);		// avg bytesper sec
			st.Write((ushort) wfx.nBlockAlign);			// blockalign = 16bit * mono/stereo
			st.Write((ushort) wfx.wBitsPerSample);		// bitspersample = 16bits

			st.Write((uint)0x61746164);					// 'data'
			st.Write((int) nTotalPCMSize);				// datasize 

			st.Write(Dest);
Trace.TraceInformation($"wrote ({outfile}.wav) fsLength=" + fs.Length + ", TotalPCMSize=" + nTotalPCMSize + ", diff=" + (fs.Length - nTotalPCMSize));
			st.Dispose();
			fs.Dispose();
		}
	}
}
