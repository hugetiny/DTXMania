using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DirectShowLib;
using DirectShowLib.DES;
using SlimDX;

namespace FDK
{
	public class CAviDS : IDisposable
	{
		public uint nフレーム高さ
		{
			get
			{
				return (uint)nHeight;
			}
		}

		public uint nフレーム幅
		{
			get
			{
				return (uint)nWidth;
			}
		}

		IMediaDet mediaDet;
		int nWidth;
		int nHeight;
		double mediaLength; // in second
		VideoInfoHeader videoInfo;
		IntPtr bufferPtr;

		public int GetDuration()
		{
			return (int)(1000 * mediaLength);
		}

		// コンストラクタ
		public CAviDS(string filename)
		{
			// DirectShow による AVI 再生
			mediaDet = (IMediaDet)new MediaDet();
			AMMediaType mediaType = null;
			bufferPtr = IntPtr.Zero;

			try
			{
				int nStreams;
				Guid type = Guid.Empty;

				mediaType = new AMMediaType();

				// Set media filename
				mediaDet.put_Filename(filename);

				// Get the number of streams in that media
				mediaDet.get_OutputStreams(out nStreams);
				for (int nIndexStream = 0; nIndexStream < nStreams; ++nIndexStream)
				{
					mediaDet.put_CurrentStream(nIndexStream);
					mediaDet.get_StreamType(out type);
					if (type == MediaType.Video)
					{
						// This stream is video
						break;
					}
				}

				if (type == MediaType.Video)
				{
					// Get video info ( width, height, media length (in second)
					mediaDet.get_StreamMediaType(mediaType);
					videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
					DsUtils.FreeAMMediaType(mediaType);
					mediaType = null;

					nWidth = videoInfo.BmiHeader.Width;
					nHeight = videoInfo.BmiHeader.Height;
					mediaDet.get_StreamLength(out mediaLength);

					// あらかじめテクスチャ用のメモリを確保
					int bufferSize;
					mediaDet.GetBitmapBits(1, out bufferSize, IntPtr.Zero, nWidth, nHeight);
					bufferPtr = Marshal.AllocHGlobal(bufferSize);
				}
			}
			catch (Exception e)
			{
				if (mediaType != null)
				{
					// free unmanaged resource if the exception is occured while obtaining videoinfo
					DsUtils.FreeAMMediaType(mediaType);
				}
				throw new ArgumentException(String.Format("ファイルを正常に開けませんでした。\"{0}\", DirectShow による例外メッセージ: {1}", filename, e.Message));
			}
		}

		public unsafe void tGetBitmap(SlimDX.Direct3D9.Device device, CTexture ctex, int timeInMs)
		{
			int bufferSize;
			mediaDet.GetBitmapBits(timeInMs / 1000f, out bufferSize, IntPtr.Zero, nWidth, nHeight);
			// データは R8G8B8
			mediaDet.GetBitmapBits(timeInMs / 1000f, out bufferSize, bufferPtr, nWidth, nHeight);
			byte* sourcePtr = (byte*)bufferPtr.ToPointer() + Marshal.SizeOf(videoInfo.BmiHeader);
			int nBmpLineByte = (nWidth * 3) + ((4 - ((nWidth * 3) % 4)) % 4);

			DataRectangle rectangle3 = ctex.texture.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
			rectangle3.Data.Seek(0, System.IO.SeekOrigin.Begin);
			uint* outPtr = (uint*)rectangle3.Data.DataPointer.ToPointer();
			for (int i = 0; i < nHeight; ++i)
			{
				for (int j = 0; j < nWidth; ++j)
				{
					byte B = *((sourcePtr + (((nHeight - i) - 1) * nBmpLineByte)) + (j * 3) + 0);
					byte G = *((sourcePtr + (((nHeight - i) - 1) * nBmpLineByte)) + (j * 3) + 1);
					byte R = *((sourcePtr + (((nHeight - i) - 1) * nBmpLineByte)) + (j * 3) + 2);
					*(outPtr + (i * nWidth + j)) = ((uint)R << 16) | ((uint)G << 8) | B;
				}
			}
			ctex.texture.UnlockRectangle(0);
		}

		#region [ Dispose-Finalize パターン実装 ]
		//-----------------
		public void Dispose()
		{
			if (!this.bDisposed)
			{
				if (bufferPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(bufferPtr);
					bufferPtr = IntPtr.Zero;
				}
				GC.SuppressFinalize(this);    // 2011.8.19 from: 忘れてた。
				this.bDisposed = true;
			}
		}

		~CAviDS()
		{
			this.Dispose();
		}
		//-----------------
		#endregion

		private bool bDisposed = false;
	}
}
