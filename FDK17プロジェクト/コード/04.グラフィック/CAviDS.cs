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

		int nWidth;
		int nHeight;
		long mediaLength; // [ms]
		VideoInfoHeader videoInfo;
		const int timeOutMs = 1000;

		public int GetDuration()
		{
			return (int)(mediaLength / 10000);
		}

		IGraphBuilder builder;
		ISampleGrabber grabber;
		IMediaControl control;
		IMediaSeeking seeker;
		FilterState state;
		AMMediaType mediaType;
		IntPtr bufferPtr = IntPtr.Zero;

		public CAviDS(string filename, double playSpeed)
		{
			int hr = 0x0;

			builder = (IGraphBuilder)new FilterGraph();

			#region [Sample Grabber]
			{
				grabber = new SampleGrabber() as ISampleGrabber;
				mediaType = new AMMediaType();
				mediaType.majorType = MediaType.Video;
				mediaType.subType = MediaSubType.RGB24;
				mediaType.formatType = FormatType.VideoInfo;
				hr = grabber.SetMediaType(mediaType);
				DsError.ThrowExceptionForHR(hr);
				hr = builder.AddFilter((IBaseFilter)grabber, "Sample Grabber");
				DsError.ThrowExceptionForHR(hr);
			}
			#endregion

			hr = builder.RenderFile(filename, "");
			DsError.ThrowExceptionForHR(hr);

			// Null レンダラに接続しないとウィンドウが表示される。
			// また、レンダリングを行わないため処理速度を向上できる。
			CDirectShow.tビデオレンダラをグラフから除去してNullレンダラに接続する(builder);

			#region [Video Info]
			{
				hr = grabber.GetConnectedMediaType(mediaType);
				DsError.ThrowExceptionForHR(hr);

				videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
				nWidth = videoInfo.BmiHeader.Width;
				nHeight = videoInfo.BmiHeader.Height;
			}
			#endregion

			#region[ Seeker ]
			{
				seeker = builder as IMediaSeeking;
				hr = seeker.GetDuration(out mediaLength);
				DsError.ThrowExceptionForHR(hr);
				hr = seeker.SetRate(playSpeed / 20);
				DsError.ThrowExceptionForHR(hr);
			}
			#endregion

			#region [Control]
			{
				control = builder as IMediaControl;
			}
			#endregion

			hr = grabber.SetBufferSamples(true);
			DsError.ThrowExceptionForHR(hr);
			Run();
			Stop();
		}

		public void Seek(int timeInMs)
		{
			int hr = seeker.SetPositions(new DsLong(timeInMs * 10000), AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
			DsError.ThrowExceptionForHR(hr);
			hr = control.GetState(timeOutMs, out state); // state is Running
			DsError.ThrowExceptionForHR(hr);
		}

		public void Run()
		{
			int hr = control.Run();
			DsError.ThrowExceptionForHR(hr);
			hr = control.GetState(timeOutMs, out state);
			DsError.ThrowExceptionForHR(hr);
		}

		public void Stop()
		{
			int hr = control.Stop();
			DsError.ThrowExceptionForHR(hr);
			hr = control.GetState(timeOutMs, out state);
			DsError.ThrowExceptionForHR(hr);
		}

		public void Pause()
		{
			int hr = control.Pause();
			DsError.ThrowExceptionForHR(hr);
			hr = control.GetState(timeOutMs, out state);
			DsError.ThrowExceptionForHR(hr);
		}

		public void ToggleRun()
		{
			int hr = control.GetState(timeOutMs, out state);
			DsError.ThrowExceptionForHR(hr);
			if( state == FilterState.Paused )
			{
				Run();
			}
			else if( state == FilterState.Running )
			{
				Pause();
			}
		}

		public unsafe void tGetBitmap(SlimDX.Direct3D9.Device device, CTexture ctex, int timeMs)
		{
			int bufferSize = 0;
			int hr = grabber.GetCurrentBuffer(ref bufferSize, IntPtr.Zero);
			DsError.ThrowExceptionForHR(hr);

			if (bufferPtr == IntPtr.Zero)
			{
				bufferPtr = Marshal.AllocHGlobal(bufferSize);
			}
			hr = grabber.GetCurrentBuffer(ref bufferSize, bufferPtr);
			DsError.ThrowExceptionForHR(hr);

			byte* sourcePtr = (byte*)bufferPtr.ToPointer();
			int stride = (nWidth * 3) + ((4 - ((nWidth * 3) % 4)) % 4); // BMP 1行ごとのバイト数 (4の倍数になるように調整)

			DataRectangle rectangle3 = ctex.texture.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
			rectangle3.Data.Seek(0, System.IO.SeekOrigin.Begin);
			uint* outPtr = (uint*)rectangle3.Data.DataPointer.ToPointer();
			Parallel.For(0, nHeight, i =>
			{
				{
					for (int j = 0; j < nWidth; ++j)
					{
						// 上下反転しつつコピー
						byte B = *((sourcePtr + (((nHeight - i) - 1) * stride)) + (j * 3) + 0);
						byte G = *((sourcePtr + (((nHeight - i) - 1) * stride)) + (j * 3) + 1);
						byte R = *((sourcePtr + (((nHeight - i) - 1) * stride)) + (j * 3) + 2);
						*(outPtr + (i * nWidth + j)) = ((uint)R << 16) | ((uint)G << 8) | B;
					}
				}
			});

			ctex.texture.UnlockRectangle(0);

		}

		#region [ Dispose-Finalize パターン実装 ]
		public void Dispose()
		{
			if (!this.bDisposed)
			{
				if (null != builder)
				{
					Marshal.ReleaseComObject(builder);
					builder = null;
				}
				if( null != grabber )
				{
					Marshal.ReleaseComObject(grabber);
					grabber = null;
				}
				if (null != mediaType)
				{
					DsUtils.FreeAMMediaType(mediaType);
					mediaType = null;
				}
				if (bufferPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(bufferPtr);
				}

				GC.SuppressFinalize(this);
				this.bDisposed = true;
			}
		}

		~CAviDS()
		{
			this.Dispose();
		}
		#endregion

		private bool bDisposed = false;
	}
}
