using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace FDK
{
	public class CTexture : IDisposable
	{
		// プロパティ

		public bool b加算合成
		{
			get;
			set; 
		}
		public float fZ軸中心回転
		{
			get;
			set;
		}
		public int n透明度
		{
			get
			{
				return this._透明度;
			}
			set
			{
				if( value < 0 )
				{
					this._透明度 = 0;
				}
				else if( value > 0xff )
				{
					this._透明度 = 0xff;
				}
				else
				{
					this._透明度 = value;
				}
			}
		}
		public Size szテクスチャサイズ
		{
			get; 
			private set;
		}
		public Size sz画像サイズ
		{
			get;
			private set;
		}
		public Texture texture
		{
			get;
			private set;
		}
		public Vector3 vc拡大縮小倍率;


		// コンストラクタ

		public CTexture()
		{
			this.sz画像サイズ = new Size( 0, 0 );
			this.szテクスチャサイズ = new Size( 0, 0 );
			this._透明度 = 0xff;
			this.texture = null;
			this.vbPositionColoredVertexBuffer = null;
			this.cvPositionColoredVertexies = null;
			this.b加算合成 = false;
			this.fZ軸中心回転 = 0f;
			this.vc拡大縮小倍率 = new Vector3( 1f, 1f, 1f );
		}
		
		/// <summary>
		/// <para>指定されたビットマップオブジェクトから Managed テクスチャを作成する。</para>
		/// <para>テクスチャのサイズは、BITMAP画像のサイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
		/// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
		/// <para>その他、ミップマップ数は 1、Usage は None、Pool は Managed、イメージフィルタは Point、ミップマップフィルタは
		/// None、カラーキーは 0xFFFFFFFF（完全なる黒を透過）になる。</para>
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="bitmap">作成元のビットマップ。</param>
		/// <param name="format">テクスチャのフォーマット。</param>
		/// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
		public CTexture( Device device, Bitmap bitmap, Format format )
			: this()
		{
			try
			{
				this.sz画像サイズ = new Size( bitmap.Width, bitmap.Height );
				this.szテクスチャサイズ = this.t指定されたサイズを超えない最適なテクスチャサイズを返す( device, this.sz画像サイズ );
				using( MemoryStream stream = new MemoryStream() )
				{
					bitmap.Save( stream, ImageFormat.Bmp );
					stream.Seek( 0L, SeekOrigin.Begin );
					int colorKey = 0;
					colorKey = -16777216;
#if TEST_Direct3D9Ex
					this.texture = Texture.FromStream( device, stream, this.szテクスチャサイズ.Width, this.szテクスチャサイズ.Height, 1, Usage.None, format, Pool.Default, Filter.Point, Filter.None, colorKey );
#else
					this.texture = Texture.FromStream( device, stream, this.szテクスチャサイズ.Width, this.szテクスチャサイズ.Height, 1, Usage.None, format, Pool.Managed, Filter.Point, Filter.None, colorKey );
#endif
				}
				this.t頂点バッファの作成( device );
			}
			catch
			{
				this.Dispose();
				throw new CTextureCreateFailedException( "ビットマップからのテクスチャの生成に失敗しました。" );
			}
		}
	
		/// <summary>
		/// <para>空の Managed テクスチャを作成する。</para>
		/// <para>テクスチャのサイズは、指定された希望サイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
		/// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
		/// <para>テクスチャのテクセルデータは未初期化。（おそらくゴミデータが入ったまま。）</para>
		/// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None、
		/// カラーキーは 0x00000000（透過しない）になる。</para>
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="n幅">テクスチャの幅（希望値）。</param>
		/// <param name="n高さ">テクスチャの高さ（希望値）。</param>
		/// <param name="format">テクスチャのフォーマット。</param>
		/// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
		public CTexture( Device device, int n幅, int n高さ, Format format )
			: this( device, n幅, n高さ, format, Pool.Managed )
		{
		}
		
		/// <summary>
		/// <para>指定された画像ファイルから Managed テクスチャを作成する。</para>
		/// <para>利用可能な画像形式は、BMP, JPG, PNG, TGA, DDS, PPM, DIB, HDR, PFM のいずれか。</para>
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="strファイル名">画像ファイル名。</param>
		/// <param name="format">テクスチャのフォーマット。</param>
		/// <param name="b黒を透過する">画像の黒（0xFFFFFFFF）を透過させるなら true。</param>
		/// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
		public CTexture( Device device, string strファイル名, Format format, bool b黒を透過する )
			: this( device, strファイル名, format, b黒を透過する, Pool.Managed )
		{
		}
		
		/// <summary>
		/// <para>空のテクスチャを作成する。</para>
		/// <para>テクスチャのサイズは、指定された希望サイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
		/// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
		/// <para>テクスチャのテクセルデータは未初期化。（おそらくゴミデータが入ったまま。）</para>
		/// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None、
		/// カラーキーは 0x00000000（透過しない）になる。</para>
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="n幅">テクスチャの幅（希望値）。</param>
		/// <param name="n高さ">テクスチャの高さ（希望値）。</param>
		/// <param name="format">テクスチャのフォーマット。</param>
		/// <param name="pool">テクスチャの管理方法。</param>
		/// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
		public CTexture( Device device, int n幅, int n高さ, Format format, Pool pool )
			: this()
		{
			try
			{
				this.sz画像サイズ = new Size( n幅, n高さ );
				this.szテクスチャサイズ = this.t指定されたサイズを超えない最適なテクスチャサイズを返す( device, this.sz画像サイズ );
				using( Bitmap bitmap = new Bitmap( 1, 1 ) )
				{
					using( Graphics graphics = Graphics.FromImage( bitmap ) )
					{
						graphics.FillRectangle( Brushes.Black, 0, 0, 1, 1 );
					}
					using( MemoryStream stream = new MemoryStream() )
					{
						bitmap.Save( stream, ImageFormat.Bmp );
						stream.Seek( 0L, SeekOrigin.Begin );
#if TEST_Direct3D9Ex
						this.texture = Texture.FromStream( device, stream, n幅, n高さ, 1, Usage.None, format, Pool.Default, Filter.Point, Filter.None, 0 );
#else
						this.texture = Texture.FromStream( device, stream, n幅, n高さ, 1, Usage.None, format, pool, Filter.Point, Filter.None, 0 );
#endif
					}
				}
				this.t頂点バッファの作成( device );
			}
			catch
			{
				this.Dispose();
				throw new CTextureCreateFailedException( string.Format( "テクスチャの生成に失敗しました。\n({0}x{1}, {2})", n幅, n高さ, format ) );
			}
		}
		
		/// <summary>
		/// <para>画像ファイルからテクスチャを生成する。</para>
		/// <para>利用可能な画像形式は、BMP, JPG, PNG, TGA, DDS, PPM, DIB, HDR, PFM のいずれか。</para>
		/// <para>テクスチャのサイズは、画像のサイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
		/// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
		/// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None になる。</para>
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="strファイル名">画像ファイル名。</param>
		/// <param name="format">テクスチャのフォーマット。</param>
		/// <param name="b黒を透過する">画像の黒（0xFFFFFFFF）を透過させるなら true。</param>
		/// <param name="pool">テクスチャの管理方法。</param>
		/// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
		public CTexture( Device device, string strファイル名, Format format, bool b黒を透過する, Pool pool )
			: this()
		{
			try
			{
				ImageInformation information = ImageInformation.FromFile( strファイル名 );
				this.sz画像サイズ = new Size( information.Width, information.Height );
				this.szテクスチャサイズ = this.t指定されたサイズを超えない最適なテクスチャサイズを返す( device, this.sz画像サイズ );
#if TEST_Direct3D9Ex
				this.texture = Texture.FromFile( device, strファイル名, this.sz画像サイズ.Width, this.sz画像サイズ.Height, 1, Usage.None, format, Pool.Default, Filter.Point, Filter.None, b黒を透過する ? -16777216 : 0 );
#else
				this.texture = Texture.FromFile( device, strファイル名, this.sz画像サイズ.Width, this.sz画像サイズ.Height, 1, Usage.None, format, pool, Filter.Point, Filter.None, b黒を透過する ? -16777216 : 0 );
#endif
				this.t頂点バッファの作成( device );
			}
			catch
			{
				this.Dispose();
				throw new CTextureCreateFailedException( string.Format( "テクスチャの生成に失敗しました。\n{0}", strファイル名 ) );
			}
		}


		// メソッド

		/// <summary>
		/// テクスチャを 2D 画像と見なして描画する。
		/// </summary>
		/// <param name="device">Direct3D9 デバイス。</param>
		/// <param name="x">描画位置（テクスチャの左上位置の X 座標[dot]）。</param>
		/// <param name="y">描画位置（テクスチャの左上位置の Y 座標[dot]）。</param>
		public void t2D描画( Device device, int x, int y )
		{
			this.t2D描画( device, x, y, new Rectangle( 0, 0, this.sz画像サイズ.Width, this.sz画像サイズ.Height ) );
		}
		public void t2D描画( Device device, int x, int y, Rectangle rc画像内の描画領域 )
		{
			this.t2D描画( device, x, y, 1f, rc画像内の描画領域 );
		}
		public void t2D描画( Device device, int x, int y, float depth, Rectangle rc画像内の描画領域 )
		{
			if( this.texture != null )
			{
				this.tレンダリングステートの設定( device );
				if( this.fZ軸中心回転 == 0f )
				{
					float num = -0.5f;
					float num2 = -0.5f;
					float width = rc画像内の描画領域.Width;
					float height = rc画像内の描画領域.Height;
					float num5 = ( (float) rc画像内の描画領域.Left ) / ( (float) this.szテクスチャサイズ.Width );
					float num6 = ( (float) rc画像内の描画領域.Right ) / ( (float) this.szテクスチャサイズ.Width );
					float num7 = ( (float) rc画像内の描画領域.Top ) / ( (float) this.szテクスチャサイズ.Height );
					float num8 = ( (float) rc画像内の描画領域.Bottom ) / ( (float) this.szテクスチャサイズ.Height );
					int num9 = new Color4( ( (float) this._透明度 ) / 255f, 1f, 1f, 1f ).ToArgb();
					if( this.cvTransformedColoredVertexies == null )
					{
						this.cvTransformedColoredVertexies = new TransformedColoredTexturedVertex[ 4 ];
					}
					this.cvTransformedColoredVertexies[ 0 ].Position = new Vector4( x + num, y + num2, depth, 1f );
					this.cvTransformedColoredVertexies[ 0 ].Color = num9;
					this.cvTransformedColoredVertexies[ 0 ].TextureCoordinates = new Vector2( num5, num7 );
					this.cvTransformedColoredVertexies[ 1 ].Position = new Vector4( ( x + ( width * this.vc拡大縮小倍率.X ) ) + num, y + num2, depth, 1f );
					this.cvTransformedColoredVertexies[ 1 ].Color = num9;
					this.cvTransformedColoredVertexies[ 1 ].TextureCoordinates = new Vector2( num6, num7 );
					this.cvTransformedColoredVertexies[ 2 ].Position = new Vector4( x + num, ( y + ( height * this.vc拡大縮小倍率.Y ) ) + num2, depth, 1f );
					this.cvTransformedColoredVertexies[ 2 ].Color = num9;
					this.cvTransformedColoredVertexies[ 2 ].TextureCoordinates = new Vector2( num5, num8 );
					this.cvTransformedColoredVertexies[ 3 ].Position = new Vector4( ( x + ( width * this.vc拡大縮小倍率.X ) ) + num, ( y + ( height * this.vc拡大縮小倍率.Y ) ) + num2, depth, 1f );
					this.cvTransformedColoredVertexies[ 3 ].Color = num9;
					this.cvTransformedColoredVertexies[ 3 ].TextureCoordinates = new Vector2( num6, num8 );
					device.SetTexture( 0, this.texture );
					device.VertexFormat = TransformedColoredTexturedVertex.Format;
					device.DrawUserPrimitives<TransformedColoredTexturedVertex>( PrimitiveType.TriangleStrip, 0, 2, this.cvTransformedColoredVertexies );
				}
				else
				{
					float num10 = ( ( rc画像内の描画領域.Width % 2 ) == 0 ) ? -0.5f : 0f;
					float num11 = ( ( rc画像内の描画領域.Height % 2 ) == 0 ) ? -0.5f : 0f;
					float num12 = ( (float) rc画像内の描画領域.Width ) / 2f;
					float num13 = ( (float) rc画像内の描画領域.Height ) / 2f;
					int num1 = rc画像内の描画領域.Width;
					int num21 = rc画像内の描画領域.Height;
					float num14 = ( (float) rc画像内の描画領域.Left ) / ( (float) this.szテクスチャサイズ.Width );
					float num15 = ( (float) rc画像内の描画領域.Right ) / ( (float) this.szテクスチャサイズ.Width );
					float num16 = ( (float) rc画像内の描画領域.Top ) / ( (float) this.szテクスチャサイズ.Height );
					float num17 = ( (float) rc画像内の描画領域.Bottom ) / ( (float) this.szテクスチャサイズ.Height );
					int num18 = new Color4( ( (float) this._透明度 ) / 255f, 1f, 1f, 1f ).ToArgb();
					if( this.cvPositionColoredVertexies == null )
					{
						this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[ 4 ];
					}
					this.cvPositionColoredVertexies[ 0 ].Position = new Vector3( -num12 + num10, num13 + num11, depth );
					this.cvPositionColoredVertexies[ 0 ].Color = num18;
					this.cvPositionColoredVertexies[ 0 ].TextureCoordinates = new Vector2( num14, num16 );
					this.cvPositionColoredVertexies[ 1 ].Position = new Vector3( num12 + num10, num13 + num11, depth );
					this.cvPositionColoredVertexies[ 1 ].Color = num18;
					this.cvPositionColoredVertexies[ 1 ].TextureCoordinates = new Vector2( num15, num16 );
					this.cvPositionColoredVertexies[ 2 ].Position = new Vector3( -num12 + num10, -num13 + num11, depth );
					this.cvPositionColoredVertexies[ 2 ].Color = num18;
					this.cvPositionColoredVertexies[ 2 ].TextureCoordinates = new Vector2( num14, num17 );
					this.cvPositionColoredVertexies[ 3 ].Position = new Vector3( num12 + num10, -num13 + num11, depth );
					this.cvPositionColoredVertexies[ 3 ].Color = num18;
					this.cvPositionColoredVertexies[ 3 ].TextureCoordinates = new Vector2( num15, num17 );
					using( DataStream stream = this.vbPositionColoredVertexBuffer.Lock( 0, 0, LockFlags.None ) )
					{
						stream.WriteRange<PositionColoredTexturedVertex>( this.cvPositionColoredVertexies );
						this.vbPositionColoredVertexBuffer.Unlock();
					}
					int num19 = x + ( rc画像内の描画領域.Width / 2 );
					int num20 = y + ( rc画像内の描画領域.Height / 2 );
					Vector3 amount = new Vector3( num19 - ( ( (float) device.Viewport.Width ) / 2f ), -( num20 - ( ( (float) device.Viewport.Height ) / 2f ) ), 0f );
					Matrix matrix = Matrix.Identity * Matrix.Scaling( this.vc拡大縮小倍率 );
					matrix *= Matrix.RotationZ( this.fZ軸中心回転 );
					matrix *= Matrix.Translation( amount );
					device.SetTransform( TransformState.World, matrix );
					device.SetTexture( 0, this.texture );
					device.SetStreamSource( 0, this.vbPositionColoredVertexBuffer, 0, PositionColoredTexturedVertex.SizeInBytes );
					device.VertexFormat = PositionColoredTexturedVertex.Format;
					device.DrawPrimitives( PrimitiveType.TriangleStrip, 0, 2 );
				}
			}
		}

		/// <summary>
		/// テクスチャを 3D 画像と見なして描画する。
		/// </summary>
		public void t3D描画( Device device, Matrix mat )
		{
			this.t3D描画( device, mat, new Rectangle( 0, 0, this.sz画像サイズ.Width, this.sz画像サイズ.Height ) );
		}
		public void t3D描画( Device device, Matrix mat, Rectangle rc画像内の描画領域 )
		{
			if( this.texture != null )
			{
				float x = ( (float) rc画像内の描画領域.Width ) / 2f;
				float y = ( (float) rc画像内の描画領域.Height ) / 2f;
				int width = rc画像内の描画領域.Width;
				int height = rc画像内の描画領域.Height;
				float num3 = ( (float) rc画像内の描画領域.Left ) / ( (float) this.szテクスチャサイズ.Width );
				float num4 = ( (float) rc画像内の描画領域.Right ) / ( (float) this.szテクスチャサイズ.Width );
				float num5 = ( (float) rc画像内の描画領域.Top ) / ( (float) this.szテクスチャサイズ.Height );
				float num6 = ( (float) rc画像内の描画領域.Bottom ) / ( (float) this.szテクスチャサイズ.Height );
				int num7 = new Color4( ( (float) this._透明度 ) / 255f, 1f, 1f, 1f ).ToArgb();
				if( this.cvPositionColoredVertexies == null )
				{
					this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[ 4 ];
				}
				float z = 0f;
				this.cvPositionColoredVertexies[ 0 ].Position = new Vector3( -x, y, z );
				this.cvPositionColoredVertexies[ 0 ].Color = num7;
				this.cvPositionColoredVertexies[ 0 ].TextureCoordinates = new Vector2( num3, num5 );
				this.cvPositionColoredVertexies[ 1 ].Position = new Vector3( x, y, z );
				this.cvPositionColoredVertexies[ 1 ].Color = num7;
				this.cvPositionColoredVertexies[ 1 ].TextureCoordinates = new Vector2( num4, num5 );
				this.cvPositionColoredVertexies[ 2 ].Position = new Vector3( -x, -y, z );
				this.cvPositionColoredVertexies[ 2 ].Color = num7;
				this.cvPositionColoredVertexies[ 2 ].TextureCoordinates = new Vector2( num3, num6 );
				this.cvPositionColoredVertexies[ 3 ].Position = new Vector3( x, -y, z );
				this.cvPositionColoredVertexies[ 3 ].Color = num7;
				this.cvPositionColoredVertexies[ 3 ].TextureCoordinates = new Vector2( num4, num6 );
				using( DataStream stream = this.vbPositionColoredVertexBuffer.Lock( 0, 0, LockFlags.None ) )
				{
					stream.WriteRange<PositionColoredTexturedVertex>( this.cvPositionColoredVertexies );
					this.vbPositionColoredVertexBuffer.Unlock();
				}
				this.tレンダリングステートの設定( device );
				device.SetTransform( TransformState.World, mat );
				device.SetTexture( 0, this.texture );
				device.SetStreamSource( 0, this.vbPositionColoredVertexBuffer, 0, PositionColoredTexturedVertex.SizeInBytes );
				device.VertexFormat = PositionColoredTexturedVertex.Format;
				device.DrawPrimitives( PrimitiveType.TriangleStrip, 0, 2 );
			}
		}

		#region [ IDosposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDispose完了済み )
			{
				// テクスチャの破棄
				if( this.texture != null )
				{
					this.texture.Dispose();
					this.texture = null;
				}

				// 頂点バッファの破棄
				if( this.vbPositionColoredVertexBuffer != null )
				{
					this.vbPositionColoredVertexBuffer.Dispose();
					this.vbPositionColoredVertexBuffer = null;
				}
				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private int _透明度;
		private bool bDispose完了済み;
		private PositionColoredTexturedVertex[] cvPositionColoredVertexies;
		private TransformedColoredTexturedVertex[] cvTransformedColoredVertexies;
		private VertexBuffer vbPositionColoredVertexBuffer;

		private void tレンダリングステートの設定( Device device )
		{
			if( this.b加算合成 )
			{
				device.SetRenderState( RenderState.AlphaBlendEnable, true );
				device.SetRenderState( RenderState.SourceBlend, 5 );
				device.SetRenderState( RenderState.DestinationBlend, 2 );
			}
			else
			{
				device.SetRenderState( RenderState.AlphaBlendEnable, true );
				device.SetRenderState( RenderState.SourceBlend, 5 );
				device.SetRenderState( RenderState.DestinationBlend, 6 );
			}
		}
		private Size t指定されたサイズを超えない最適なテクスチャサイズを返す( Device device, Size sz指定サイズ )
		{
			bool flag = ( device.Capabilities.TextureCaps & TextureCaps.NonPow2Conditional ) != 0;
			bool flag2 = ( device.Capabilities.TextureCaps & TextureCaps.Pow2 ) != 0;
			bool flag3 = ( device.Capabilities.TextureCaps & TextureCaps.SquareOnly ) != 0;
			int maxTextureWidth = device.Capabilities.MaxTextureWidth;
			int maxTextureHeight = device.Capabilities.MaxTextureHeight;
			Size size = new Size( sz指定サイズ.Width, sz指定サイズ.Height );
			if( flag2 && !flag )
			{
				int num3 = 1;
				do
				{
					num3 *= 2;
				}
				while( num3 <= sz指定サイズ.Width );
				sz指定サイズ.Width = num3;
				num3 = 1;
				do
				{
					num3 *= 2;
				}
				while( num3 <= sz指定サイズ.Height );
				sz指定サイズ.Height = num3;
			}
			if( sz指定サイズ.Width > maxTextureWidth )
			{
				sz指定サイズ.Width = maxTextureWidth;
			}
			if( sz指定サイズ.Height > maxTextureHeight )
			{
				sz指定サイズ.Height = maxTextureHeight;
			}
			if( flag3 )
			{
				if( size.Width > size.Height )
				{
					size.Height = size.Width;
					return size;
				}
				if( size.Width < size.Height )
				{
					size.Width = size.Height;
				}
			}
			return size;
		}
		private void t頂点バッファの作成( Device device )
		{
#if TEST_Direct3D9Ex
			this.vbPositionColoredVertexBuffer = new VertexBuffer( device, 4 * PositionColoredTexturedVertex.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Default );
#else
			this.vbPositionColoredVertexBuffer = new VertexBuffer( device, 4 * PositionColoredTexturedVertex.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed );
#endif
		}
		//-----------------
		#endregion
	}
}
