using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DTXMania
{
	public class CPreviewMagnifier
	{

		#region [ プロパティ(拡大率等の取得) ]
		/// <summary>
		/// 拡大後のwidth
		/// </summary>
		public int width;
		/// <summary>
		/// 拡大後のheight
		/// </summary>
		public int height;
		/// <summary>
		/// 拡大後のX拡大率
		/// </summary>
		public float magX;
		/// <summary>
		/// 拡大後のY拡大率
		/// </summary>
		public float magY;

		public int px;
		public int py;

		/// <summary>
		/// プレビュー画像向けの拡大率か(それとも、演奏画面向けの拡大率か)
		/// </summary>
		public enum EPreviewType : int
		{
			MusicSelect = 0,			// 選曲画面
			PlayingFront = 1,			// 演奏画面(ウインドウ表示の動画)
			PlayingBackground = 2		// 演奏画面(背景全画面表示の動画)
		}
		public EPreviewType ePreviewType;
		#endregion

		#region [ 定数定義 ]
		// 配列の0,1要素はそれぞれ, Preview用, 演奏画面用
		private int[] WIDTH_VGA_SET = { 204, 278, 640 };                     // VGA版DTXManiaのプレビュー画像width値
		private int[] HEIGHT_VGA_SET = { 269, 355, 1920 };                      // VGA版DTXManiaのプレビュー画像height値
		private int[] WIDTH_HD_SET = { 400, 400, 1920 };                      // HD版DTXManiaのプレビュー画像width値
		private int[] HEIGHT_HD_SET = { 400, 600, 1080 }; // 600は仮								// HD版DTXManiaのプレビュー画像height値
		private int[] WIDTH_FHD_LIMIT = { 320, 320, 640 };                     // VGA版/FullHD版どちらのプレビュー画像とみなすかのwidth閾値
		private int[] HEIGHT_FHD_LIMIT = { 416, 416, 480 };                      // VGA版/FullHD版どちらのプレビュー画像とみなすかのwidth閾値
		private int[] WIDTH_FHD_SET = { (int)(204 * Scale.X), (int)(278 * Scale.X), 1920 }; // FHD版DTXManiaのプレビュー画像height値
		private int[] HEIGHT_FHD_SET = { (int)(269 * Scale.Y), (int)(355 * Scale.Y), 1080 };  // FHD版DTXManiaのプレビュー画像height値
		#endregion


		#region [ コンストラクタ ]
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CPreviewMagnifier()
		{
		}
		public CPreviewMagnifier(EPreviewType _ePreviewType)
		{
			CPreviewMagnifier_initializer(_ePreviewType, 0, 0);
		}
		public CPreviewMagnifier( EPreviewType _ePreviewType, int _px, int _py )
		{
			CPreviewMagnifier_initializer( _ePreviewType, _px, _py );
		}
		private void CPreviewMagnifier_initializer( EPreviewType _ePreviewType, int _px, int _py)
		{
			this.ePreviewType = _ePreviewType;
			this.px = _px;
			this.py = _py;
		}
		#endregion

		/// <summary>
		/// 拡大率の取得
		/// </summary>
		/// <param name="width_org">元の幅</param>
		/// <param name="height_org">元の高さ</param>
		/// <param name="magX_org">元の拡大率(幅)</param>
		/// <param name="magY_org">元の拡大率(高さ)</param>
		/// <remarks>出力はプロパティで得てください。</remarks>
		public void GetMagnifier(int width_org, int height_org, float magX_org, float magY_org)
		{
			//bool bIsPreview = ( this.ePreviewType == EPreviewType.MusicSelect );

			// #35820 画像サイズに関係なく、プレビュー領域に合わせる add ikanick 15.12.08
			// #36176 プレビュー画像については、前仕様(204x269)画像はアスペクト比を維持する change ikanick 16.03.20

			if ( this.ePreviewType == EPreviewType.PlayingBackground )	// フル背景動画に限り、上位指定の表示座標を無視する
			{
				this.px = this.py = 0;
			}

			#region [ アスペクト比を維持した拡大縮小 ]
			this.width = width_org;
			this.height = height_org;
			this.magX = magX_org * width_fhd_set / width_org;
			this.magY = magY_org * height_fhd_set / height_org;

			if ( magX > magY )
			{
				magX = magY;
				px += (int) ( ( width_fhd_set - ( width_org * magY ) ) / 2 );
			}
			else
			{
				magY = magX;
				py += (int) ( ( height_fhd_set - ( height_org * magX ) ) / 2 );
			}
			#endregion
		}

		#region [ bIsPreviewによる配列→定数読み替え ]
		private int width_vga_set
		{
			get
			{
				return WIDTH_VGA_SET[ (int)ePreviewType ];
			}
		}
		private int height_vga_set
		{
			get
			{
				return HEIGHT_VGA_SET[ (int)ePreviewType ];
			}
		}
		private int width_hd_set
		{
			get
			{
				return WIDTH_HD_SET[ (int)ePreviewType ];
			}
		}
		private int height_hd_set
		{
			get
			{
				return HEIGHT_HD_SET[ (int)ePreviewType ];
			}
		}
		private int width_fhd_limit
		{
			get
			{
				return WIDTH_FHD_LIMIT[ (int)ePreviewType ];
			}
		}
		private int height_fhd_limit
		{
			get
			{
				return HEIGHT_FHD_LIMIT[ (int)ePreviewType ];
			}
		}
		private int width_fhd_set
		{
			get
			{
				return WIDTH_FHD_SET[ (int)ePreviewType ];
			}
		}
		private int height_fhd_set
		{
			get
			{
				return HEIGHT_FHD_SET[ (int)ePreviewType ];
			}
		}
		#endregion
	}
}
