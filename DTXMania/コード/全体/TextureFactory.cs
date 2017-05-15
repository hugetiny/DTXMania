using System;
using System.Diagnostics;
using System.IO;
using FDK;
using System.Drawing;

namespace DTXMania
{
	public class TextureFactory
	{

		#region [ 汎用ヘルパー ]
		//-----------------
		#region [ tテクスチャの生成 ]
		public static CTexture tテクスチャの生成(string fileName)
		{
			return tテクスチャの生成(fileName, false);
		}

		public static CTexture tテクスチャの生成(string fileName, bool b黒を透過する)
		{
			if (CDTXMania.Instance == null)
			{
				return null;
			}
			try
			{
				return new CTexture(CDTXMania.Instance.Device, fileName, CDTXMania.Instance.TextureFormat, b黒を透過する);
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。({0})", fileName);
				return null;
			}
			catch (FileNotFoundException)
			{
				Trace.TraceError("テクスチャファイルが見つかりませんでした。({0})", fileName);
				return null;
			}
		}

		public static CTextureAf tテクスチャの生成Af(string fileName)
		{
			return tテクスチャの生成Af(fileName, false);
		}

		public static CTextureAf tテクスチャの生成Af(string fileName, bool b黒を透過する)
		{
			if (CDTXMania.Instance == null)
			{
				return null;
			}
			try
			{
				return new CTextureAf(CDTXMania.Instance.Device, fileName, CDTXMania.Instance.TextureFormat, b黒を透過する);
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。({0})", fileName);
				return null;
			}
			catch (FileNotFoundException)
			{
				Trace.TraceError("テクスチャファイルが見つかりませんでした。({0})", fileName);
				return null;
			}
		}

		public static void tテクスチャの解放(ref CTexture tx)
		{
			t安全にDisposeする(ref tx);
		}

		public static void tテクスチャの解放(ref CTextureAf tx)
		{
			t安全にDisposeする(ref tx);
		}

		public static CTexture tテクスチャの生成(byte[] txData)
		{
			return tテクスチャの生成(txData, false);
		}

		public static CTexture tテクスチャの生成(byte[] txData, bool b黒を透過する)
		{
			if (CDTXMania.Instance == null)
			{
				return null;
			}
			if (txData == null)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。(txData==null)");
				return null;
			}
			try
			{
				return new CTexture(CDTXMania.Instance.Device, txData, CDTXMania.Instance.TextureFormat, b黒を透過する);
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。(txData)");
				return null;
			}
		}

		public static CTexture tテクスチャの生成(Bitmap bitmap)
		{
			return tテクスチャの生成(bitmap, false);
		}

		public static CTexture tテクスチャの生成(Bitmap bitmap, bool b黒を透過する)
		{
			if (CDTXMania.Instance == null)
			{
				return null;
			}
			if (bitmap == null)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。(bitmap==null)");
				return null;
			}
			try
			{
				return new CTexture(CDTXMania.Instance.Device, bitmap, CDTXMania.Instance.TextureFormat, b黒を透過する);
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。(bitmap:{0}x{1} )", bitmap.Width, bitmap.Height);
				return null;
			}
		}
		#endregion

		/// <summary>プロパティ、インデクサには ref は使用できないので注意。</summary>
		public static void t安全にDisposeする<T>(ref T obj)
		{
			if (obj == null)
				return;

			var d = obj as IDisposable;

			if (d != null)
				d.Dispose();

			obj = default(T);
		}
		//-----------------
		#endregion
	}
}
