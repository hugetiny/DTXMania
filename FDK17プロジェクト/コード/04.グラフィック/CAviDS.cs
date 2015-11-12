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
		const int timeOutMs = 1000; // グラフ state の遷移完了を待つタイムアウト期間 

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
		int nStride;
		long nMediaLength; // [ms]

		public int GetDuration()
		{
			return (int)(nMediaLength / 10000);
		}

		IGraphBuilder builder;
		VideoInfoHeader videoInfo;
		ISampleGrabber grabber;
		IMediaControl control;
		IMediaSeeking seeker;
		IMediaFilter filter;
		FilterState state;
		AMMediaType mediaType;
		IntPtr samplePtr = IntPtr.Zero;
		unsafe byte* bSrcPtr; // for Transfer
		unsafe uint* bDstPtr; // for Transfer
		int nParallel;

		delegate void TransferDelegate(int i);
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
			// CDirectShow.tビデオレンダラをグラフから除去してNullレンダラに接続する(builder);

			IVideoWindow videoWindow = builder as IVideoWindow;
			if (videoWindow != null)
			{
				videoWindow.put_AutoShow(OABool.False);
			}

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
				hr = seeker.GetDuration(out nMediaLength);
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

			#region [Filter]
			{
				filter = builder as IMediaFilter;
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

			if (samplePtr == IntPtr.Zero)
			{
				samplePtr = Marshal.AllocHGlobal(bufferSize);
			}
			hr = grabber.GetCurrentBuffer(ref bufferSize, samplePtr);
			DsError.ThrowExceptionForHR(hr);
			
			DataRectangle rectangle3 = ctex.texture.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
			rectangle3.Data.Seek(0, System.IO.SeekOrigin.Begin);
			Transfer(rectangle3.Data.DataPointer);			
			ctex.texture.UnlockRectangle(0);
		}

		unsafe private void Transfer(IntPtr dstPtr)
		{
			nStride = (nWidth * 3) + ((4 - ((nWidth * 3) % 4)) % 4); // BMP 1行ごとのバイト数 (4の倍数になるように調整)
			nParallel = Environment.ProcessorCount;

			bSrcPtr = (byte*)samplePtr.ToPointer();
			bDstPtr = (uint*)dstPtr.ToPointer();

			TransferDelegate[] workers = new TransferDelegate[nParallel];
			IAsyncResult[] ars = new IAsyncResult[nParallel];
			for(int i = 0; i < workers.Length; ++i)
			{
				workers[i] = new TransferDelegate(TransferSlave);
				ars[i] = workers[i].BeginInvoke(i, null, null);
			}
			for(int i = 0; i < workers.Length; ++i)
			{
				workers[i].EndInvoke(ars[i]);
			}
		}

		unsafe private void TransferSlave(int start)
		{
			const int unroll = 8;
			for (int i = start; i < nHeight; i += nParallel)
			{
				byte* srcLine = bSrcPtr + nStride * (nHeight - 1 - i);
				uint* dstLine = bDstPtr + nWidth * i;

				for(int j = 0; j < nWidth / unroll; ++j)
				{
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;

					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
				}
				for(int j = 0; j < nWidth % unroll; ++j)
				{
					*dstLine++ = (*srcLine++) | ((uint)(*srcLine++) << 8) | ((uint)(*srcLine++) << 16) | 0xFF000000;
				}
			}
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
				if (samplePtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(samplePtr);
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
