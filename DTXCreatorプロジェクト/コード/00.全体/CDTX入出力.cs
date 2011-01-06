﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using DTXCreator.Properties;
using DTXCreator.譜面;
using DTXCreator.UndoRedo;
using DTXCreator.WAV_BMP_AVI;
using FDK;
using System.Diagnostics;

namespace DTXCreator
{
	internal class CDTX入出力
	{
		internal CDTX入出力( Cメインフォーム mf )
		{
			this._Form = mf;
		}
		public void tDTX出力( StreamWriter sw )
		{
			this.tDTX出力( sw, false );
		}
		public void tDTX出力( StreamWriter sw, bool bBGMのみ出力 )
		{
			sw.WriteLine( "; Created by DTXCreator " + Resources.DTXC_VERSION );
			this.tDTX出力・タイトルと製作者とコメントその他( sw );
			this.tDTX出力・自由入力欄( sw );
			this.tDTX出力・WAVリスト( sw, bBGMのみ出力 );
			this.tDTX出力・BMPリスト( sw );
			this.tDTX出力・AVIリスト( sw );
			this.tDTX出力・小節長倍率( sw );
			this.tDTX出力・BPxリスト( sw );
			this.tDTX出力・全チップ( sw );
			sw.WriteLine();
			this.tDTX出力・レーン割付チップ( sw );
			this.tDTX出力・WAVリスト色設定( sw );
			this.tDTX出力・BMPリスト色設定( sw );
			this.tDTX出力・AVIリスト色設定( sw );
			this.tDTX出力・チップパレット( sw );
		}
		public void tDTX入力( E種別 e種別, ref string str全入力文字列 )
		{
			this.e種別 = e種別;
			if( str全入力文字列.Length != 0 )
			{
				this.dic小節長倍率 = new Dictionary<int, float>();
				this.listチップパレット = new List<int>();
				this.nBGMWAV番号 = -1;
				this._Form.listViewWAVリスト.BeginUpdate();
				this._Form.listViewBMPリスト.BeginUpdate();
				this._Form.listViewAVIリスト.BeginUpdate();
				str全入力文字列 = str全入力文字列.Replace( Environment.NewLine, "\n" );
				str全入力文字列 = str全入力文字列.Replace( '\t', ' ' );
				StringBuilder builder = new StringBuilder();
				CharEnumerator ce = str全入力文字列.GetEnumerator();
				if( ce.MoveNext() )
				{
					do
					{
						if( !this.tDTX入力・空白と改行をスキップする( ref ce ) )
						{
							break;
						}
						if( ce.Current == '#' )
						{
							if( ce.MoveNext() )
							{
								StringBuilder builder2 = new StringBuilder( 0x20 );
								if( this.tDTX入力・コマンド文字列を抜き出す( ref ce, ref builder2 ) )
								{
									StringBuilder builder3 = new StringBuilder( 0x400 );
									if( this.tDTX入力・パラメータ文字列を抜き出す( ref ce, ref builder3 ) )
									{
										StringBuilder builder4 = new StringBuilder( 0x400 );
										if( this.tDTX入力・コメント文字列を抜き出す( ref ce, ref builder4 ) )
										{
											if( !this.tDTX入力・行解析( ref builder2, ref builder3, ref builder4 ) )
											{
												builder.Append( string.Concat( new object[] { "#", builder2, ": ", builder3 } ) );
												if( builder4.Length > 0 )
												{
													builder.Append( "\t;" + builder4 );
												}
												builder.Append( Environment.NewLine );
											}
											continue;
										}
									}
								}
							}
							break;
						}
					}
					while( this.tDTX入力・コメントをスキップする( ref ce ) );
					CUndoRedo管理.bUndoRedoした直後 = true;
					this._Form.textBox自由入力欄.Text = this._Form.textBox自由入力欄.Text + builder.ToString();
					this.tDTX入力・小節内のチップリストを発声位置でソートする();
					this.tDTX入力・小節長倍率配列を昇順ソート済みの小節リストに適用する();
					this.tDTX入力・BPMチップにBPx数値をバインドする();
					this.tDTX入力・キャッシュからListViewを一括構築する();
					this.tDTX入力・チップパレットのListViewを一括構築する();
					if( this.nBGMWAV番号 >= 0 )
					{
						this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( this.nBGMWAV番号 ).bBGMとして使用 = true;
					}
					this._Form.listViewWAVリスト.EndUpdate();
					this._Form.listViewBMPリスト.EndUpdate();
					this._Form.listViewAVIリスト.EndUpdate();
				}
			}
		}

		public enum E種別
		{
			DTX,
			GDA,
			G2D,
			BMS,
			BME
		}

		#region [ private ]
		//-----------------
		private Cメインフォーム _Form;
		private int[] arr素数リスト = new int[] {
			2, 3, 5, 7, 11, 13, 0x11, 0x13, 0x17, 0x1d, 0x1f, 0x25, 0x29, 0x2b, 0x2f, 0x35, 
			0x3b, 0x3d, 0x43, 0x47, 0x49, 0x4f, 0x53, 0x59, 0x61, 0x65, 0x67, 0x6b, 0x6d, 0x71, 0x7f, 0x83, 
			0x89, 0x8b, 0x95, 0x97, 0x9d, 0xa3, 0xa7, 0xad, 0xb3, 0xb5, 0xbf, 0xc1, 0xc5, 0xc7, 0xd3, 0xdf, 
			0xe3, 0xe5, 0xe9, 0xef, 0xf1, 0xfb, 0x101, 0x107, 0x10d, 0x10f, 0x115, 0x119, 0x11b, 0x125, 0x133, 0x137, 
			0x139, 0x13d, 0x14b, 0x151, 0x15b, 0x15d, 0x161, 0x167, 0x16f, 0x175, 0x17b, 0x17f
		};
		private Dictionary<int, float> dic小節長倍率;
		private E種別 e種別;
		private List<int> listチップパレット;
		private int nBGMWAV番号 = -1;

		private void tDTX入力・BPMチップにBPx数値をバインドする()
		{
			foreach( KeyValuePair<int, C小節> pair in this._Form.mgr譜面管理者.dic小節 )
			{
				C小節 c小節 = pair.Value;
				for( int i = 0; i < c小節.listチップ.Count; i++ )
				{
					Cチップ cチップ = c小節.listチップ[ i ];
					float num2 = 0f;
					if( ( cチップ.nチャンネル番号00toFF == 8 ) && this._Form.mgr譜面管理者.dicBPx.TryGetValue( cチップ.n値・整数1to1295, out num2 ) )
					{
						cチップ.f値・浮動小数 = num2;
					}
					if( cチップ.nチャンネル番号00toFF == 3 )
					{
						cチップ.nチャンネル番号00toFF = 8;
						cチップ.f値・浮動小数 = cチップ.n値・整数1to1295;
						cチップ.b裏 = false;
						for( int j = 1; j <= 0x50f; j++ )
						{
							if( !this._Form.mgr譜面管理者.dicBPx.ContainsKey( j ) )
							{
								this._Form.mgr譜面管理者.dicBPx.Add( j, cチップ.f値・浮動小数 );
								cチップ.n値・整数1to1295 = j;
								break;
							}
						}
					}
				}
			}
		}
		private void tDTX入力・キャッシュからListViewを一括構築する()
		{
			for( int i = 1; i <= 0x50f; i++ )
			{
				CWAV cwav = this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す( i );
				if( cwav != null )
				{
					cwav.tコピーto( this._Form.listViewWAVリスト.Items[ i - 1 ] );
				}
				CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す( i );
				if( cbmp != null )
				{
					cbmp.tコピーto( this._Form.listViewBMPリスト.Items[ i - 1 ] );
				}
				CAVI cavi = this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す( i );
				if( cavi != null )
				{
					cavi.tコピーto( this._Form.listViewAVIリスト.Items[ i - 1 ] );
				}
			}
		}
		private bool tDTX入力・コマンド文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( this.tDTX入力・空白をスキップする( ref ce ) )
			{
				while( ( ( ce.Current != ':' ) && ( ce.Current != ' ' ) ) && ( ( ce.Current != ';' ) && ( ce.Current != '\n' ) ) )
				{
					sb文字列.Append( ce.Current );
					if( !ce.MoveNext() )
					{
						return false;
					}
				}
				if( ce.Current == ':' )
				{
					if( !ce.MoveNext() )
					{
						return false;
					}
					if( !this.tDTX入力・空白をスキップする( ref ce ) )
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private bool tDTX入力・コメントをスキップする( ref CharEnumerator ce )
		{
			while( ce.Current != '\n' )
			{
				if( !ce.MoveNext() )
				{
					return false;
				}
			}
			return ce.MoveNext();
		}
		private bool tDTX入力・コメント文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( ce.Current != ';' )
			{
				return true;
			}
			if( ce.MoveNext() )
			{
				while( ce.Current != '\n' )
				{
					sb文字列.Append( ce.Current );
					if( !ce.MoveNext() )
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private void tDTX入力・チップパレットのListViewを一括構築する()
		{
			for( int i = 0; i < ( this.listチップパレット.Count / 2 ); i += 2 )
			{
				int num2 = this.listチップパレット[ i * 2 ];
				int num3 = this.listチップパレット[ ( i * 2 ) + 1 ];
				string[] items = new string[ 3 ];
				switch( num2 )
				{
					case 0:
						{
							CWAV cwav = this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す( num3 );
							if( cwav != null )
							{
								items[ 0 ] = cwav.strラベル名;
								items[ 1 ] = C変換.str数値を36進数2桁に変換して返す( num3 );
								items[ 2 ] = cwav.strファイル名;
								ListViewItem item = new ListViewItem( items );
								item.ImageIndex = num2;
								item.ForeColor = cwav.col文字色;
								item.BackColor = cwav.col背景色;
								this._Form.dlgチップパレット.listViewチップリスト.Items.Add( item );
							}
							break;
						}
					case 1:
						{
							CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す( num3 );
							if( cbmp != null )
							{
								items[ 0 ] = cbmp.strラベル名;
								items[ 1 ] = C変換.str数値を36進数2桁に変換して返す( num3 );
								items[ 2 ] = cbmp.strファイル名;
								ListViewItem item2 = new ListViewItem( items );
								item2.ImageIndex = num2;
								item2.ForeColor = cbmp.col文字色;
								item2.BackColor = cbmp.col背景色;
								this._Form.dlgチップパレット.listViewチップリスト.Items.Add( item2 );
							}
							break;
						}
					case 2:
						{
							CAVI cavi = this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す( num3 );
							if( cavi != null )
							{
								items[ 0 ] = cavi.strラベル名;
								items[ 1 ] = C変換.str数値を36進数2桁に変換して返す( num3 );
								items[ 2 ] = cavi.strファイル名;
								ListViewItem item3 = new ListViewItem( items );
								item3.ImageIndex = num2;
								item3.ForeColor = cavi.col文字色;
								item3.BackColor = cavi.col背景色;
								this._Form.dlgチップパレット.listViewチップリスト.Items.Add( item3 );
							}
							break;
						}
				}
			}
		}
		private bool tDTX入力・パラメータ文字列を抜き出す( ref CharEnumerator ce, ref StringBuilder sb文字列 )
		{
			if( this.tDTX入力・空白をスキップする( ref ce ) )
			{
				while( ( ce.Current != '\n' ) && ( ce.Current != ';' ) )
				{
					sb文字列.Append( ce.Current );
					if( !ce.MoveNext() )
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private bool tDTX入力・空白と改行をスキップする( ref CharEnumerator ce )
		{
			while( ( ce.Current == ' ' ) || ( ce.Current == '\n' ) )
			{
				if( !ce.MoveNext() )
				{
					return false;
				}
			}
			return true;
		}
		private bool tDTX入力・空白をスキップする( ref CharEnumerator ce )
		{
			while( ce.Current == ' ' )
			{
				if( !ce.MoveNext() )
				{
					return false;
				}
			}
			return true;
		}
		private bool tDTX入力・行解析( ref StringBuilder sbコマンド, ref StringBuilder sbパラメータ, ref StringBuilder sbコメント )
		{
			string str = sbコマンド.ToString();
			string str2 = sbパラメータ.ToString().Trim();
			string str3 = sbコメント.ToString();
			return ( this.tDTX入力・行解析・TITLE_ARTIST_COMMENT_その他( str, str2, str3 ) || ( this.tDTX入力・行解析・WAVVOL_VOLUME( str, str2, str3 ) || ( this.tDTX入力・行解析・WAVPAN_PAN( str, str2, str3 ) || ( this.tDTX入力・行解析・WAV( str, str2, str3 ) || ( this.tDTX入力・行解析・BGMWAV( str, str2, str3 ) || ( this.tDTX入力・行解析・BMPTEX( str, str2, str3 ) || ( this.tDTX入力・行解析・BMP( str, str2, str3 ) || ( this.tDTX入力・行解析・AVI_AVIPAN( str, str2, str3 ) || ( this.tDTX入力・行解析・BPx( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_LANEBINDEDCHIP( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_WAVFORECOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_WAVBACKCOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_BMPFORECOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_BMPBACKCOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_AVIFORECOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_AVIBACKCOLOR( str, str2, str3 ) || ( this.tDTX入力・行解析・DTXC_CHIPPALETTE( str, str2, str3 ) || this.tDTX入力・行解析・チャンネル( str, str2, str3 ) ) ) ) ) ) ) ) ) ) ) ) ) ) ) ) ) );
		}
		private bool tDTX入力・行解析・AVI_AVIPAN( string strコマンド, string strパラメータ, string strコメント )
		{
			if( !strコマンド.StartsWith( "AVIPAN", StringComparison.OrdinalIgnoreCase ) && strコマンド.StartsWith( "AVI", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 3 );
			}
			else
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( ( num < 1 ) || ( num > 0x50f ) )
			{
				return false;
			}
			CAVI cavi = this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す・なければ新規生成する( num );
			cavi.strラベル名 = strコメント;
			cavi.strファイル名 = strパラメータ;
			return true;
		}
		private bool tDTX入力・行解析・BGMWAV( string strコマンド, string strパラメータ, string strコメント )
		{
			if( strコマンド.StartsWith( "bgmwav", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 6 );
			}
			else
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( 0, 2 ) );
			if( ( num < 1 ) || ( num > 0x50f ) )
			{
				return false;
			}
			this.nBGMWAV番号 = num;
			return true;
		}
		private bool tDTX入力・行解析・BMP( string strコマンド, string strパラメータ, string strコメント )
		{
			if( ( strコマンド.Length > 3 ) && strコマンド.StartsWith( "BMP", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 3 );
			}
			else
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( ( num < 1 ) || ( num > 0x50f ) )
			{
				return false;
			}
			CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す・なければ新規生成する( num );
			cbmp.strラベル名 = strコメント;
			cbmp.strファイル名 = strパラメータ;
			cbmp.bテクスチャ = false;
			return true;
		}
		private bool tDTX入力・行解析・BMPTEX( string strコマンド, string strパラメータ, string strコメント )
		{
			if( strコマンド.StartsWith( "BMPTEX", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 6 );
			}
			else
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( ( num < 1 ) || ( num > 0x50f ) )
			{
				return false;
			}
			CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す・なければ新規生成する( num );
			cbmp.strラベル名 = strコメント;
			cbmp.strファイル名 = strパラメータ;
			cbmp.bテクスチャ = true;
			return true;
		}
		private bool tDTX入力・行解析・BPx( string strコマンド, string strパラメータ, string strコメント )
		{
			if( strコマンド.StartsWith( "BPM", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 3 );
			}
			else
			{
				return false;
			}
			int key = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( ( key < 1 ) || ( key > 0x50f ) )
			{
				return false;
			}
			decimal result = 0;
			if( ( !this.TryParse( strパラメータ, out result ) || ( result < 0 ) ) || ( result > 1000 ) )		// #23880 2011.1.6 yyagi
			{
				return false;
			}
			this._Form.mgr譜面管理者.dicBPx.Add(key, (float)result);
			return true;
		}
		private bool tDTX入力・行解析・DTXC_AVIBACKCOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_AVIBACKCOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col背景色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_AVIFORECOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_AVIFORECOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col文字色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_BMPBACKCOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_BMPBACKCOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col背景色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_BMPFORECOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_BMPFORECOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col文字色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_CHIPPALETTE( string strコマンド, string strパラメータ, string strコメント )
		{
			if( !strコマンド.Equals( "DTXC_CHIPPALETTE", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			foreach( string str in strパラメータ.Split( new char[] { ' ' } ) )
			{
				int num;
				string[] strArray2 = str.Split( new char[] { ',' } );
				if( ( ( strArray2.Length == 2 ) && int.TryParse( strArray2[ 0 ], out num ) ) && ( ( num >= 0 ) && ( num <= 2 ) ) )
				{
					int item = C変換.n36進数2桁の文字列を数値に変換して返す( strArray2[ 1 ] );
					if( ( item >= 1 ) && ( item <= 0x50f ) )
					{
						this.listチップパレット.Add( num );
						this.listチップパレット.Add( item );
					}
				}
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_LANEBINDEDCHIP( string strコマンド, string strパラメータ, string strコメント )
		{
			if( strコマンド.Equals( "DTXC_LANEBINDEDCHIP", StringComparison.OrdinalIgnoreCase ) && ( strパラメータ.Length == 8 ) )
			{
				int num;
				if( !int.TryParse( strパラメータ.Substring( 0, 2 ), out num ) )
				{
					return false;
				}
				int num2 = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( 3, 2 ) );
				if( ( num2 < 0 ) || ( num2 > 0x50f ) )
				{
					return false;
				}
				int num3 = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( 6, 2 ) );
				if( ( num3 < 0 ) || ( num3 > 0x50f ) )
				{
					return false;
				}
				if( ( num >= 0 ) && ( num < this._Form.mgr譜面管理者.listレーン.Count ) )
				{
					if( num2 != 0 )
					{
						this._Form.mgr譜面管理者.listレーン[ num ].nレーン割付チップ・表0or1to1295 = num2;
					}
					if( num3 != 0 )
					{
						this._Form.mgr譜面管理者.listレーン[ num ].nレーン割付チップ・裏0or1to1295 = num3;
					}
					return true;
				}
			}
			return false;
		}
		private bool tDTX入力・行解析・DTXC_WAVBACKCOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_WAVBACKCOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col背景色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・DTXC_WAVFORECOLOR( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			if( !strコマンド.Equals( "DTXC_WAVFORECOLOR", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			string[] strArray = strパラメータ.Split( new char[] { ' ', '\t' } );
			if( strArray.Length < 2 )
			{
				return false;
			}
			if( !int.TryParse( strArray[ 0 ], out num ) )
			{
				return false;
			}
			if( ( num < 0 ) || ( num > 0x50e ) )
			{
				return false;
			}
			Color color = ColorTranslator.FromHtml( strArray[ 1 ] );
			if( ( num >= 0 ) && ( num <= 0x50e ) )
			{
				this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( num + 1 ).col文字色 = color;
			}
			return true;
		}
		private bool tDTX入力・行解析・TITLE_ARTIST_COMMENT_その他( string strコマンド, string strパラメータ, string strコメント )
		{
			float num5;
			if( strコマンド.Equals( "TITLE", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBox曲名.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "ARTIST", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBox製作者.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "COMMENT", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxコメント.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "PANEL", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxパネル.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "PREVIEW", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxPREVIEW.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "PREIMAGE", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxPREIMAGE.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "STAGEFILE", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxSTAGEFILE.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "BACKGROUND", StringComparison.OrdinalIgnoreCase ) || strコマンド.Equals( "WALL", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxBACKGROUND.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "RESULTIMAGE", StringComparison.OrdinalIgnoreCase ) )
			{
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxRESULTIMAGE.Text = strパラメータ.Trim();
				return true;
			}
			if( strコマンド.Equals( "BPM", StringComparison.OrdinalIgnoreCase ) )
			{
				decimal num;
				if( !this.TryParse( strパラメータ, out num ) )		// #23880 2011.1.6 yyagi
				{
					num = 120.0M;
				}
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.numericUpDownBPM.Value = num;
				return true;
			}
			if( strコマンド.Equals( "DLEVEL", StringComparison.OrdinalIgnoreCase ) )
			{
				int num2;
				if( !int.TryParse( strパラメータ, out num2 ) )
				{
					num2 = 0;
				}
				else if( num2 < 0 )
				{
					num2 = 0;
				}
				else if( num2 > 100 )
				{
					num2 = 100;
				}
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.hScrollBarDLEVEL.Value = num2;
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxDLEVEL.Text = num2.ToString();
				return true;
			}
			if( strコマンド.Equals( "GLEVEL", StringComparison.OrdinalIgnoreCase ) )
			{
				int num3;
				if( !int.TryParse( strパラメータ, out num3 ) )
				{
					num3 = 0;
				}
				else if( num3 < 0 )
				{
					num3 = 0;
				}
				else if( num3 > 100 )
				{
					num3 = 100;
				}
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.hScrollBarGLEVEL.Value = num3;
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxGLEVEL.Text = num3.ToString();
				return true;
			}
			if( strコマンド.Equals( "BLEVEL", StringComparison.OrdinalIgnoreCase ) )
			{
				int num4;
				if( !int.TryParse( strパラメータ, out num4 ) )
				{
					num4 = 0;
				}
				else if( num4 < 0 )
				{
					num4 = 0;
				}
				else if( num4 > 100 )
				{
					num4 = 100;
				}
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.hScrollBarBLEVEL.Value = num4;
				CUndoRedo管理.bUndoRedoした直後 = true;
				this._Form.textBoxBLEVEL.Text = num4.ToString();
				return true;
			}
			if( !strコマンド.Equals( "DTXVPLAYSPEED", StringComparison.OrdinalIgnoreCase ) )
			{
				return false;
			}
			if( !float.TryParse( strパラメータ, out num5 ) )
			{
				num5 = 0f;
			}
			else if( num5 < 0.5f )
			{
				num5 = 0.5f;
			}
			else if( num5 > 1.5f )
			{
				num5 = 1.5f;
			}
			float num6 = ( 1.5f - num5 ) * 10f;
			int num7 = (int) num6;
			if( num7 < 0 )
			{
				num7 = 0;
			}
			else if( num7 > 10 )
			{
				num7 = 10;
			}
			CUndoRedo管理.bUndoRedoした直後 = true;
			this._Form.toolStripComboBox演奏速度.SelectedIndex = num7;
			return true;
		}
		private bool tDTX入力・行解析・WAV( string strコマンド, string strパラメータ, string strコメント )
		{
			if( strコマンド.StartsWith( "wav", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 3 );
			}
			else
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( ( num < 1 ) || ( num > 0x50f ) )
			{
				return false;
			}
			CWAV cwav = this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( num );
			cwav.strラベル名 = strコメント;
			cwav.strファイル名 = strパラメータ;
			return true;
		}
		private bool tDTX入力・行解析・WAVPAN_PAN( string strコマンド, string strパラメータ, string strコメント )
		{
			int num2;
			if( strコマンド.StartsWith( "WAVPAN", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 6 );
			}
			else if( strコマンド.StartsWith( "PAN", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 3 );
			}
			else
			{
				return false;
			}
			if( strコマンド.Length < 2 )
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( int.TryParse( strパラメータ, out num2 ) )
			{
				if( num2 < -100 )
				{
					num2 = -100;
				}
				else if( num2 >= 100 )
				{
					num2 = 100;
				}
				this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( num ).n位置_100to100 = num2;
			}
			return true;
		}
		private bool tDTX入力・行解析・WAVVOL_VOLUME( string strコマンド, string strパラメータ, string strコメント )
		{
			int num2;
			if( strコマンド.StartsWith( "WAVVOL", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 6 );
			}
			else if( strコマンド.StartsWith( "VOLUME", StringComparison.OrdinalIgnoreCase ) )
			{
				strコマンド = strコマンド.Substring( 6 );
			}
			else
			{
				return false;
			}
			if( strコマンド.Length < 2 )
			{
				return false;
			}
			int num = C変換.n36進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 0, 2 ) );
			if( int.TryParse( strパラメータ, out num2 ) )
			{
				if( num2 < 0 )
				{
					num2 = 0;
				}
				else if( num2 >= 100 )
				{
					num2 = 100;
				}
				this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す・なければ新規生成する( num ).n音量0to100 = num2;
			}
			return true;
		}
		private bool tDTX入力・行解析・チャンネル( string strコマンド, string strパラメータ, string strコメント )
		{
			int num;
			int num2;
			if( !this.tDTX入力・行解析・チャンネル・コマンドから小節番号とチャンネル番号を抜き出す( strコマンド, out num, out num2 ) )
			{
				return false;
			}
			if( num2 == 2 )
			{
				decimal dBarLength;
				if( !decimal.TryParse( strパラメータ, out dBarLength ) )	// #23880 2011.1.6 yyagi
				{
					dBarLength = 1m;
				}
				this.dic小節長倍率.Add( num, (float)dBarLength );
				return true;
			}
			if( ( num2 >= 0x20 ) && ( num2 <= 0x27 ) )
			{
				C小節 c小節 = this.tDTX入力・行解析・チャンネル・小節番号に対応する小節を探すか新規に作って返す( num );
				int startIndex = 0;
				while( ( startIndex = strパラメータ.IndexOf( '_' ) ) != -1 )
				{
					strパラメータ = strパラメータ.Remove( startIndex, 1 );
				}
				int num5 = strパラメータ.Length / 2;
				for( int i = 0; i < num5; i++ )
				{
					int num7 = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( i * 2, 2 ) );
					if( num7 != 0 )
					{
						int num8 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "GtV" );
						int num9 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "GtR" );
						int num10 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "GtG" );
						int num11 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "GtB" );
						Cチップ item = new Cチップ();
						item.nレーン番号0to = num8;
						item.n位置grid = i;
						item.n値・整数1to1295 = num7;
						item.n読み込み時の解像度 = num5;
						c小節.listチップ.Add( item );
						switch( num2 )
						{
							case 0x20:
								item = new Cチップ();
								item.nレーン番号0to = num9;
								item.n位置grid = i;
								item.n値・整数1to1295 = 2;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x21:
								item = new Cチップ();
								item.nレーン番号0to = num11;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x22:
								item = new Cチップ();
								item.nレーン番号0to = num10;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x23:
								item = new Cチップ();
								item.nレーン番号0to = num10;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								item = new Cチップ();
								item.nレーン番号0to = num11;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x24:
								item = new Cチップ();
								item.nレーン番号0to = num9;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x25:
								item = new Cチップ();
								item.nレーン番号0to = num9;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								item = new Cチップ();
								item.nレーン番号0to = num11;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x26:
								item = new Cチップ();
								item.nレーン番号0to = num9;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								item = new Cチップ();
								item.nレーン番号0to = num10;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;

							case 0x27:
								item = new Cチップ();
								item.nレーン番号0to = num9;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								item = new Cチップ();
								item.nレーン番号0to = num10;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								item = new Cチップ();
								item.nレーン番号0to = num11;
								item.n位置grid = i;
								item.n値・整数1to1295 = 1;
								item.n読み込み時の解像度 = num5;
								c小節.listチップ.Add( item );
								break;
						}
					}
				}
				return true;
			}
			if( ( num2 >= 160 ) && ( num2 <= 0xa7 ) )
			{
				C小節 c小節2 = this.tDTX入力・行解析・チャンネル・小節番号に対応する小節を探すか新規に作って返す( num );
				int num12 = 0;
				while( ( num12 = strパラメータ.IndexOf( '_' ) ) != -1 )
				{
					strパラメータ = strパラメータ.Remove( num12, 1 );
				}
				int num13 = strパラメータ.Length / 2;
				for( int j = 0; j < num13; j++ )
				{
					int num15 = C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( j * 2, 2 ) );
					if( num15 != 0 )
					{
						int num16 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BsV" );
						int num17 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BsR" );
						int num18 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BsG" );
						int num19 = this._Form.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BsB" );
						Cチップ cチップ2 = new Cチップ();
						cチップ2.nレーン番号0to = num16;
						cチップ2.n位置grid = j;
						cチップ2.n値・整数1to1295 = num15;
						cチップ2.n読み込み時の解像度 = num13;
						c小節2.listチップ.Add( cチップ2 );
						switch( num2 )
						{
							case 160:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num17;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 2;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa1:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num19;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa2:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num18;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa3:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num18;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num19;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa4:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num17;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa5:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num17;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num19;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa6:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num17;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num18;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;

							case 0xa7:
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num17;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num18;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								cチップ2 = new Cチップ();
								cチップ2.nレーン番号0to = num19;
								cチップ2.n位置grid = j;
								cチップ2.n値・整数1to1295 = 1;
								cチップ2.n読み込み時の解像度 = num13;
								c小節2.listチップ.Add( cチップ2 );
								break;
						}
					}
				}
				return true;
			}
			int num20 = -1;
			bool flag = false;
			if( this.tDTX入力・行解析・チャンネル・チャンネルに該当するレーン番号を返す( num2, out num20, out flag ) )
			{
				C小節 c小節3 = this.tDTX入力・行解析・チャンネル・小節番号に対応する小節を探すか新規に作って返す( num );
				int num21 = 0;
				while( ( num21 = strパラメータ.IndexOf( '_' ) ) != -1 )
				{
					strパラメータ = strパラメータ.Remove( num21, 1 );
				}
				int num22 = strパラメータ.Length / 2;
				for( int k = 0; k < num22; k++ )
				{
					int num24 = ( num2 == 3 ) ? C変換.n16進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( k * 2, 2 ) ) : C変換.n36進数2桁の文字列を数値に変換して返す( strパラメータ.Substring( k * 2, 2 ) );
					if( num24 > 0 )
					{
						Cチップ cチップ3 = new Cチップ();
						cチップ3.nチャンネル番号00toFF = num2;
						cチップ3.nレーン番号0to = num20;
						cチップ3.n位置grid = k;
						cチップ3.n読み込み時の解像度 = num22;
						cチップ3.n値・整数1to1295 = num24;
						cチップ3.b裏 = flag;
						c小節3.listチップ.Add( cチップ3 );
					}
				}
				return true;
			}
			StringBuilder builder = new StringBuilder( 0x400 );
			builder.Append( "#" + C変換.str小節番号を文字列3桁に変換して返す( num ) + C変換.str数値を16進数2桁に変換して返す( num2 ) + ": " + strパラメータ );
			if( strコメント.Length > 0 )
			{
				builder.Append( " ;" + strコメント );
			}
			builder.Append( Environment.NewLine );
			CUndoRedo管理.bUndoRedoした直後 = true;
			this._Form.textBox自由入力欄.AppendText( builder.ToString() );
			return true;
		}
		private int tDTX入力・行解析・チャンネル・GDAチャンネル文字列２桁をチャンネル番号にして返す( string strチャンネル文字列２桁 )
		{
			if( strチャンネル文字列２桁.Length == 2 )
			{
				switch( strチャンネル文字列２桁.ToUpper() )
				{
					case "TC":
						return 3;

					case "BL":
						return 2;

					case "GS":
						return 0x29;

					case "DS":
						return 0x30;

					case "FI":
						return 0x53;

					case "HH":
						return 0x11;

					case "SD":
						return 0x12;

					case "BD":
						return 0x13;

					case "HT":
						return 20;

					case "LT":
						return 0x15;

					case "CY":
						return 0x16;

					case "G1":
						return 0x21;

					case "G2":
						return 0x22;

					case "G3":
						return 0x23;

					case "G4":
						return 0x24;

					case "G5":
						return 0x25;

					case "G6":
						return 0x26;

					case "G7":
						return 0x27;

					case "GW":
						return 40;

					case "01":
						return 0x61;

					case "02":
						return 0x62;

					case "03":
						return 0x63;

					case "04":
						return 100;

					case "05":
						return 0x65;

					case "06":
						return 0x66;

					case "07":
						return 0x67;

					case "08":
						return 0x68;

					case "09":
						return 0x69;

					case "0A":
						return 0x70;

					case "0B":
						return 0x71;

					case "0C":
						return 0x72;

					case "0D":
						return 0x73;

					case "0E":
						return 0x74;

					case "0F":
						return 0x75;

					case "10":
						return 0x76;

					case "11":
						return 0x77;

					case "12":
						return 120;

					case "13":
						return 0x79;

					case "14":
						return 0x80;

					case "15":
						return 0x81;

					case "16":
						return 130;

					case "17":
						return 0x83;

					case "18":
						return 0x84;

					case "19":
						return 0x85;

					case "1A":
						return 0x86;

					case "1B":
						return 0x87;

					case "1C":
						return 0x88;

					case "1D":
						return 0x89;

					case "1E":
						return 0x90;

					case "1F":
						return 0x91;

					case "20":
						return 0x92;

					case "B1":
						return 0xa1;

					case "B2":
						return 0xa2;

					case "B3":
						return 0xa3;

					case "B4":
						return 0xa4;

					case "B5":
						return 0xa5;

					case "B6":
						return 0xa6;

					case "B7":
						return 0xa7;

					case "BW":
						return 0xa8;

					case "G0":
						return 0x20;

					case "B0":
						return 160;
				}
			}
			return -1;
		}
		private bool tDTX入力・行解析・チャンネル・コマンドから小節番号とチャンネル番号を抜き出す( string strコマンド, out int n小節番号, out int nチャンネル番号 )
		{
			if( strコマンド.Length >= 5 )
			{
				n小節番号 = C変換.n小節番号の文字列3桁を数値に変換して返す( strコマンド.Substring( 0, 3 ) );
				if( ( this.e種別 == E種別.GDA ) || ( this.e種別 == E種別.G2D ) )
				{
					nチャンネル番号 = this.tDTX入力・行解析・チャンネル・GDAチャンネル文字列２桁をチャンネル番号にして返す( strコマンド.Substring( 3, 2 ) );
				}
				else
				{
					nチャンネル番号 = C変換.n16進数2桁の文字列を数値に変換して返す( strコマンド.Substring( 3, 2 ) );
				}
				return ( ( n小節番号 >= 0 ) && ( nチャンネル番号 > 0 ) );
			}
			n小節番号 = -1;
			nチャンネル番号 = -1;
			return false;
		}
		private bool tDTX入力・行解析・チャンネル・チャンネルに該当するレーン番号を返す( int nチャンネル番号, out int nレーン番号, out bool b裏 )
		{
			nレーン番号 = -1;
			b裏 = false;
			for( int i = 0; i < this._Form.mgr譜面管理者.listレーン.Count; i++ )
			{
				Cレーン cレーン = this._Form.mgr譜面管理者.listレーン[ i ];
				if( cレーン.nチャンネル番号・表00toFF == nチャンネル番号 )
				{
					nレーン番号 = i;
					b裏 = false;
					return true;
				}
				if( cレーン.nチャンネル番号・裏00toFF == nチャンネル番号 )
				{
					nレーン番号 = i;
					b裏 = true;
					return true;
				}
			}
			return false;
		}
		private C小節 tDTX入力・行解析・チャンネル・小節番号に対応する小節を探すか新規に作って返す( int n小節番号 )
		{
			C小節 c小節 = this._Form.mgr譜面管理者.p小節を返す( n小節番号 );
			if( c小節 == null )
			{
				if( n小節番号 > this._Form.mgr譜面管理者.n現在の最大の小節番号を返す() )
				{
					for( int i = this._Form.mgr譜面管理者.n現在の最大の小節番号を返す() + 1; i <= n小節番号; i++ )
					{
						c小節 = new C小節( i );
						this._Form.mgr譜面管理者.dic小節.Add( i, c小節 );
					}
					return c小節;
				}
				c小節 = new C小節( n小節番号 );
				this._Form.mgr譜面管理者.dic小節.Add( n小節番号, c小節 );
			}
			return c小節;
		}
		private void tDTX入力・小節長倍率配列を昇順ソート済みの小節リストに適用する()
		{
			float num = 1f;
			for( int i = 0; i < this._Form.mgr譜面管理者.dic小節.Count; i++ )
			{
				C小節 c小節 = this._Form.mgr譜面管理者.dic小節[ i ];
				foreach( KeyValuePair<int, float> pair in this.dic小節長倍率 )
				{
					if( c小節.n小節番号0to3599 == pair.Key )
					{
						num = pair.Value;
					}
				}
				c小節.f小節長倍率 = num;
				for( int j = 0; j < c小節.listチップ.Count; j++ )
				{
					c小節.listチップ[ j ].n位置grid = ( c小節.listチップ[ j ].n位置grid * c小節.n小節長倍率を考慮した現在の小節の高さgrid ) / c小節.listチップ[ j ].n読み込み時の解像度;
				}
			}
		}
		private void tDTX入力・小節内のチップリストを発声位置でソートする()
		{
			foreach( KeyValuePair<int, C小節> pair in this._Form.mgr譜面管理者.dic小節 )
			{
				pair.Value.listチップ.Sort();
			}
		}

		private void tDTX出力・AVIリスト( StreamWriter sw )
		{
			sw.WriteLine();
			for( int i = 1; i <= 0x50f; i++ )
			{
				CAVI cavi = this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す( i );
				if( ( cavi != null ) && ( cavi.strファイル名.Length > 0 ) )
				{
					string str = C変換.str数値を36進数2桁に変換して返す( cavi.nAVI番号1to1295 );
					sw.Write( "#AVI{0}: {1}", str, cavi.strファイル名 );
					if( cavi.strラベル名.Length > 0 )
					{
						sw.Write( "\t;{0}", cavi.strラベル名 );
					}
					sw.WriteLine();
				}
			}
		}
		private void tDTX出力・AVIリスト色設定( StreamWriter sw )
		{
			Color color = ColorTranslator.FromHtml( "window" );
			Color color2 = ColorTranslator.FromHtml( "windowtext" );
			for( int i = 1; i <= 0x50f; i++ )
			{
				CAVI cavi = this._Form.mgrAVIリスト管理者.tAVIをキャッシュから検索して返す( i );
				if( cavi != null )
				{
					if( cavi.col文字色 != color2 )
					{
						sw.WriteLine( "#DTXC_AVIFORECOLOR: {0} {1}", i, ColorTranslator.ToHtml( cavi.col文字色 ) );
					}
					if( cavi.col背景色 != color )
					{
						sw.WriteLine( "#DTXC_AVIBACKCOLOR: {0} {1}", i, ColorTranslator.ToHtml( cavi.col背景色 ) );
					}
				}
			}
		}
		private void tDTX出力・BMPリスト( StreamWriter sw )
		{
			sw.WriteLine();
			for( int i = 1; i <= 0x50f; i++ )
			{
				CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す( i );
				if( ( cbmp != null ) && ( cbmp.strファイル名.Length > 0 ) )
				{
					string str = C変換.str数値を36進数2桁に変換して返す( cbmp.nBMP番号1to1295 );
					if( !cbmp.bテクスチャ )
					{
						sw.Write( "#BMP{0}: {1}", str, cbmp.strファイル名 );
						if( cbmp.strラベル名.Length > 0 )
						{
							sw.Write( "\t;{0}", cbmp.strラベル名 );
						}
						sw.WriteLine();
					}
					else
					{
						sw.Write( "#BMPTEX{0}: {1}", str, cbmp.strファイル名 );
						if( cbmp.strラベル名.Length > 0 )
						{
							sw.Write( "\t;{0}", cbmp.strラベル名 );
						}
						sw.WriteLine();
					}
				}
			}
		}
		private void tDTX出力・BMPリスト色設定( StreamWriter sw )
		{
			Color color = ColorTranslator.FromHtml( "window" );
			Color color2 = ColorTranslator.FromHtml( "windowtext" );
			for( int i = 1; i <= 0x50f; i++ )
			{
				CBMP cbmp = this._Form.mgrBMPリスト管理者.tBMPをキャッシュから検索して返す( i );
				if( cbmp != null )
				{
					if( cbmp.col文字色 != color2 )
					{
						sw.WriteLine( "#DTXC_BMPFORECOLOR: {0} {1}", i, ColorTranslator.ToHtml( cbmp.col文字色 ) );
					}
					if( cbmp.col背景色 != color )
					{
						sw.WriteLine( "#DTXC_BMPBACKCOLOR: {0} {1}", i, ColorTranslator.ToHtml( cbmp.col背景色 ) );
					}
				}
			}
		}
		private void tDTX出力・BPxリスト( StreamWriter sw )
		{
			sw.WriteLine();
			foreach( KeyValuePair<int, float> pair in this._Form.mgr譜面管理者.dicBPx )
			{
				sw.WriteLine( "#BPM{0}: {1}", C変換.str数値を36進数2桁に変換して返す( pair.Key ), pair.Value );
			}
		}
		private void tDTX出力・WAVリスト( StreamWriter sw, bool bBGMのみ出力 )
		{
			sw.WriteLine();
			for( int i = 1; i <= 0x50f; i++ )
			{
				CWAV cwav = this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す( i );
				if( ( ( cwav != null ) && ( cwav.strファイル名.Length > 0 ) ) && ( !bBGMのみ出力 || cwav.bBGMとして使用 ) )
				{
					string str = C変換.str数値を36進数2桁に変換して返す( cwav.nWAV番号1to1295 );
					sw.Write( "#WAV{0}: {1}", str, cwav.strファイル名 );
					if( cwav.strラベル名.Length > 0 )
					{
						sw.Write( "\t;{0}", cwav.strラベル名 );
					}
					sw.WriteLine();
					if( cwav.n音量0to100 != 100 )
					{
						sw.WriteLine( "#VOLUME{0}: {1}", str, cwav.n音量0to100.ToString() );
					}
					if( cwav.n位置_100to100 != 0 )
					{
						sw.WriteLine( "#PAN{0}: {1}", str, cwav.n位置_100to100.ToString() );
					}
					if( cwav.bBGMとして使用 )
					{
						sw.WriteLine( "#BGMWAV: {0}", str );
					}
				}
			}
		}
		private void tDTX出力・WAVリスト色設定( StreamWriter sw )
		{
			Color color = ColorTranslator.FromHtml( "window" );
			Color color2 = ColorTranslator.FromHtml( "windowtext" );
			for( int i = 1; i <= 0x50f; i++ )
			{
				CWAV cwav = this._Form.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す( i );
				if( cwav != null )
				{
					if( cwav.col文字色 != color2 )
					{
						sw.WriteLine( "#DTXC_WAVFORECOLOR: {0} {1}", i, ColorTranslator.ToHtml( cwav.col文字色 ) );
					}
					if( cwav.col背景色 != color )
					{
						sw.WriteLine( "#DTXC_WAVBACKCOLOR: {0} {1}", i, ColorTranslator.ToHtml( cwav.col背景色 ) );
					}
				}
			}
		}
		private void tDTX出力・タイトルと製作者とコメントその他( StreamWriter sw )
		{
			sw.WriteLine();
			if( this._Form.textBox曲名.Text.Length == 0 )
			{
				sw.WriteLine( "#TITLE: (no title)" );
			}
			else
			{
				sw.WriteLine( "#TITLE: " + this._Form.textBox曲名.Text );
			}
			if( this._Form.textBox製作者.Text.Length > 0 )
			{
				sw.WriteLine( "#ARTIST: " + this._Form.textBox製作者.Text );
			}
			if( this._Form.textBoxコメント.Text.Length > 0 )
			{
				sw.WriteLine( "#COMMENT: " + this._Form.textBoxコメント.Text );
			}
			if( this._Form.textBoxパネル.Text.Length > 0 )
			{
				sw.WriteLine( "#PANEL: " + this._Form.textBoxパネル.Text );
			}
			if( this._Form.textBoxPREVIEW.Text.Length > 0 )
			{
				sw.WriteLine( "#PREVIEW: " + this._Form.textBoxPREVIEW.Text );
			}
			if( this._Form.textBoxPREIMAGE.Text.Length > 0 )
			{
				sw.WriteLine( "#PREIMAGE: " + this._Form.textBoxPREIMAGE.Text );
			}
			if( this._Form.textBoxSTAGEFILE.Text.Length > 0 )
			{
				sw.WriteLine( "#STAGEFILE: " + this._Form.textBoxSTAGEFILE.Text );
			}
			if( this._Form.textBoxBACKGROUND.Text.Length > 0 )
			{
				sw.WriteLine( "#BACKGROUND: " + this._Form.textBoxBACKGROUND.Text );
			}
			if( this._Form.textBoxRESULTIMAGE.Text.Length > 0 )
			{
				sw.WriteLine( "#RESULTIMAGE: " + this._Form.textBoxRESULTIMAGE.Text );
			}
			if( this._Form.numericUpDownBPM.Value != 0M )
			{
				sw.WriteLine( "#BPM: " + this._Form.numericUpDownBPM.Value );
			}
			if( this._Form.hScrollBarDLEVEL.Value != 0 )
			{
				sw.WriteLine( "#DLEVEL: " + this._Form.hScrollBarDLEVEL.Value );
			}
			if( this._Form.hScrollBarGLEVEL.Value != 0 )
			{
				sw.WriteLine( "#GLEVEL: " + this._Form.hScrollBarGLEVEL.Value );
			}
			if( this._Form.hScrollBarBLEVEL.Value != 0 )
			{
				sw.WriteLine( "#BLEVEL: " + this._Form.hScrollBarBLEVEL.Value );
			}
			if( this._Form.mgr譜面管理者.strPATH_WAV.Length != 0 )
			{
				sw.WriteLine( "#PATH_WAV: " + this._Form.mgr譜面管理者.strPATH_WAV );
			}
			if( this._Form.toolStripComboBox演奏速度.SelectedIndex != 5 )
			{
				sw.WriteLine( "#DTXVPLAYSPEED: " + ( 1.5f - ( this._Form.toolStripComboBox演奏速度.SelectedIndex * 0.1f ) ) );
			}
		}
		private void tDTX出力・チップパレット( StreamWriter sw )
		{
			sw.Write( "#DTXC_CHIPPALETTE: " );
			foreach( ListViewItem item in this._Form.dlgチップパレット.listViewチップリスト.Items )
			{
				sw.Write( " {0},{1}", item.ImageIndex, item.SubItems[ 1 ].Text );
			}
			sw.WriteLine();
		}
		private void tDTX出力・レーン割付チップ( StreamWriter sw )
		{
			sw.WriteLine();
			for( int i = 0; i < this._Form.mgr譜面管理者.listレーン.Count; i++ )
			{
				Cレーン cレーン = this._Form.mgr譜面管理者.listレーン[ i ];
				if( ( cレーン.nレーン割付チップ・表0or1to1295 > 0 ) || ( cレーン.nレーン割付チップ・裏0or1to1295 > 0 ) )
				{
					sw.WriteLine( "#DTXC_LANEBINDEDCHIP: {0} {1} {2}", i.ToString( "00" ), C変換.str数値を36進数2桁に変換して返す( cレーン.nレーン割付チップ・表0or1to1295 ), C変換.str数値を36進数2桁に変換して返す( cレーン.nレーン割付チップ・裏0or1to1295 ) );
				}
			}
		}
		private void tDTX出力・自由入力欄( StreamWriter sw )
		{
			sw.WriteLine();
			if( this._Form.textBox自由入力欄.Text.Length > 0 )
			{
				sw.WriteLine();
				sw.Write( this._Form.textBox自由入力欄.Text );
				sw.WriteLine();
			}
		}
		private void tDTX出力・小節長倍率( StreamWriter sw )
		{
			sw.WriteLine();
			float num = 1f;
			for( int i = 0; i < this._Form.mgr譜面管理者.dic小節.Count; i++ )
			{
				C小節 c小節 = this._Form.mgr譜面管理者.dic小節[ i ];
				if( c小節.f小節長倍率 != num )
				{
					num = c小節.f小節長倍率;
					sw.WriteLine( "#{0}02: {1}", C変換.str小節番号を文字列3桁に変換して返す( c小節.n小節番号0to3599 ), num );
				}
			}
		}
		private void tDTX出力・全チップ( StreamWriter sw )
		{
			sw.WriteLine();
			foreach( KeyValuePair<int, C小節> pair in this._Form.mgr譜面管理者.dic小節 )
			{
				C小節 c小節 = pair.Value;
				List<int> list = new List<int>();
				foreach( Cチップ cチップ in c小節.listチップ )
				{
					if( list.IndexOf( cチップ.nチャンネル番号00toFF ) < 0 )
					{
						list.Add( cチップ.nチャンネル番号00toFF );
					}
				}
				int[,] numArray = new int[ c小節.n小節長倍率を考慮した現在の小節の高さgrid, 2 ];
				foreach( int num in list )
				{
					if( num != 0 )
					{
						for( int num2 = 0; num2 < c小節.n小節長倍率を考慮した現在の小節の高さgrid; num2++ )
						{
							numArray[ num2, 0 ] = numArray[ num2, 1 ] = 0;
						}
						foreach( Cチップ cチップ2 in c小節.listチップ )
						{
							if( cチップ2.nチャンネル番号00toFF == num )
							{
								numArray[ cチップ2.n位置grid, 0 ] = cチップ2.n値・整数1to1295;
							}
						}
						int num3 = 0;
						for( int num4 = 0; num4 < c小節.n小節長倍率を考慮した現在の小節の高さgrid; num4++ )
						{
							num3 += numArray[ num4, 0 ];
						}
						if( num3 != 0 )
						{
							int num5 = c小節.n小節長倍率を考慮した現在の小節の高さgrid;
							foreach( int num6 in this.arr素数リスト )
							{
								while( this.tDTX出力・全チップ・解像度をＮ分の１にできる( num6, ref numArray, num5 ) )
								{
									num5 /= num6;
									for( int num7 = 0; num7 < num5; num7++ )
									{
										numArray[ num7, 0 ] = numArray[ num7 * num6, 0 ];
									}
								}
							}
							StringBuilder builder = new StringBuilder();
							for( int num8 = 0; num8 < num5; num8++ )
							{
								if( num == 3 )
								{
									builder.Append( C変換.str数値を16進数2桁に変換して返す( numArray[ num8, 0 ] ) );
								}
								else
								{
									builder.Append( C変換.str数値を36進数2桁に変換して返す( numArray[ num8, 0 ] ) );
								}
							}
							sw.WriteLine( "#{0}{1}: {2}", C変換.str小節番号を文字列3桁に変換して返す( c小節.n小節番号0to3599 ), C変換.str数値を16進数2桁に変換して返す( num ), builder.ToString() );
						}
					}
				}
				for( int i = 0; i < c小節.n小節長倍率を考慮した現在の小節の高さgrid; i++ )
				{
					numArray[ i, 0 ] = numArray[ i, 1 ] = 0;
				}
				foreach( Cチップ cチップ3 in c小節.listチップ )
				{
					Cレーン cレーン = this._Form.mgr譜面管理者.listレーン[ cチップ3.nレーン番号0to ];
					switch( cレーン.eレーン種別 )
					{
						case Cレーン.E種別.GtV:
							{
								numArray[ cチップ3.n位置grid, 0 ] = cチップ3.n値・整数1to1295;
								continue;
							}
						case Cレーン.E種別.GtR:
							{
								numArray[ cチップ3.n位置grid, 1 ] |= ( cチップ3.n値・整数1to1295 == 1 ) ? 0x04 : 0xFF; // OPEN = 0xFF
								continue;
							}
						case Cレーン.E種別.GtG:
							{
								numArray[ cチップ3.n位置grid, 1 ] |= 0x02;
								continue;
							}
						case Cレーン.E種別.GtB:
							{
								numArray[ cチップ3.n位置grid, 1 ] |= 0x01;
								continue;
							}
					}
				}
				for( int j = 0; j < c小節.n小節長倍率を考慮した現在の小節の高さgrid; j++ )
				{
					if( ( numArray[ j, 0 ] == 0 ) || ( numArray[ j, 1 ] == 0 ) )
					{
						numArray[ j, 0 ] = 0;
						numArray[ j, 1 ] = 0;
					}
				}
				int num11 = c小節.n小節長倍率を考慮した現在の小節の高さgrid;
				foreach( int num12 in this.arr素数リスト )
				{
					while( this.tDTX出力・全チップ・解像度をＮ分の１にできる( num12, ref numArray, num11 ) )
					{
						num11 /= num12;
						for( int num13 = 0; num13 < num11; num13++ )
						{
							numArray[ num13, 0 ] = numArray[ num13 * num12, 0 ];
							numArray[ num13, 1 ] = numArray[ num13 * num12, 1 ];
						}
					}
				}
				bool[] flagArray = new bool[ 8 ];
				for( int k = 0; k < 8; k++ )
				{
					flagArray[ k ] = false;
				}
				for( int m = 0; m < num11; m++ )
				{
					if( numArray[ m, 1 ] == 0xff )
					{
						flagArray[ 0 ] = true;
					}
					else if( numArray[ m, 1 ] != 0 )
					{
						flagArray[ numArray[ m, 1 ] ] = true;
					}
				}
				StringBuilder[] builderArray = new StringBuilder[ 8 ];
				for( int n = 0; n < 8; n++ )
				{
					builderArray[ n ] = new StringBuilder();
				}
				for( int num17 = 0; num17 < num11; num17++ )
				{
					int num18 = ( numArray[ num17, 1 ] == 0xff ) ? 0x20 : ( numArray[ num17, 1 ] + 0x20 );
					for( int num19 = 0; num19 < 8; num19++ )
					{
						if( flagArray[ num19 ] )
						{
							if( num19 == ( num18 - 0x20 ) )
							{
								builderArray[ num19 ].Append( C変換.str数値を36進数2桁に変換して返す( numArray[ num17, 0 ] ) );
							}
							else
							{
								builderArray[ num19 ].Append( "00" );
							}
						}
					}
				}
				for( int num20 = 0; num20 < 8; num20++ )
				{
					if( builderArray[ num20 ].Length != 0 )
					{
						sw.WriteLine( "#{0}{1}: {2}", C変換.str小節番号を文字列3桁に変換して返す( c小節.n小節番号0to3599 ), C変換.str数値を16進数2桁に変換して返す( 0x20 + num20 ), builderArray[ num20 ].ToString() );
					}
				}
				for( int num21 = 0; num21 < c小節.n小節長倍率を考慮した現在の小節の高さgrid; num21++ )
				{
					numArray[ num21, 0 ] = numArray[ num21, 1 ] = 0;
				}
				foreach( Cチップ cチップ4 in c小節.listチップ )
				{
					Cレーン cレーン2 = this._Form.mgr譜面管理者.listレーン[ cチップ4.nレーン番号0to ];
					switch( cレーン2.eレーン種別 )
					{
						case Cレーン.E種別.BsV:
							{
								numArray[ cチップ4.n位置grid, 0 ] = cチップ4.n値・整数1to1295;
								continue;
							}
						case Cレーン.E種別.BsR:
							{
								numArray[ cチップ4.n位置grid, 1 ] |= ( cチップ4.n値・整数1to1295 == 1 ) ? 4 : 0xff;	// OPEN = 0xFF
								continue;
							}
						case Cレーン.E種別.BsG:
							{
								numArray[ cチップ4.n位置grid, 1 ] |= 0x02;
								continue;
							}
						case Cレーン.E種別.BsB:
							{
								numArray[ cチップ4.n位置grid, 1 ] |= 0x01;
								continue;
							}
					}
				}
				for( int num22 = 0; num22 < c小節.n小節長倍率を考慮した現在の小節の高さgrid; num22++ )
				{
					if( ( numArray[ num22, 0 ] == 0 ) || ( numArray[ num22, 1 ] == 0 ) )
					{
						numArray[ num22, 0 ] = 0;
						numArray[ num22, 1 ] = 0;
					}
				}
				int num23 = c小節.n小節長倍率を考慮した現在の小節の高さgrid;
				foreach( int num24 in this.arr素数リスト )
				{
					while( this.tDTX出力・全チップ・解像度をＮ分の１にできる( num24, ref numArray, num23 ) )
					{
						num23 /= num24;
						for( int num25 = 0; num25 < num23; num25++ )
						{
							numArray[ num25, 0 ] = numArray[ num25 * num24, 0 ];
							numArray[ num25, 1 ] = numArray[ num25 * num24, 1 ];
						}
					}
				}
				bool[] flagArray2 = new bool[ 8 ];
				for( int num26 = 0; num26 < 8; num26++ )
				{
					flagArray2[ num26 ] = false;
				}
				for( int num27 = 0; num27 < num23; num27++ )
				{
					if( numArray[ num27, 1 ] == 0xff )
					{
						flagArray2[ 0 ] = true;
					}
					else if( numArray[ num27, 1 ] != 0 )
					{
						flagArray2[ numArray[ num27, 1 ] ] = true;
					}
				}
				StringBuilder[] builderArray2 = new StringBuilder[ 8 ];
				for( int num28 = 0; num28 < 8; num28++ )
				{
					builderArray2[ num28 ] = new StringBuilder();
				}
				for( int num29 = 0; num29 < num23; num29++ )
				{
					int num30 = ( numArray[ num29, 1 ] == 0xff ) ? 160 : ( numArray[ num29, 1 ] + 160 );
					for( int num31 = 0; num31 < 8; num31++ )
					{
						if( flagArray2[ num31 ] )
						{
							if( num31 == ( num30 - 160 ) )
							{
								builderArray2[ num31 ].Append( C変換.str数値を36進数2桁に変換して返す( numArray[ num29, 0 ] ) );
							}
							else
							{
								builderArray2[ num31 ].Append( "00" );
							}
						}
					}
				}
				for( int num32 = 0; num32 < 8; num32++ )
				{
					if( builderArray2[ num32 ].Length != 0 )
					{
						sw.WriteLine( "#{0}{1}: {2}", C変換.str小節番号を文字列3桁に変換して返す( c小節.n小節番号0to3599 ), C変換.str数値を16進数2桁に変換して返す( 160 + num32 ), builderArray2[ num32 ].ToString() );
					}
				}
			}
		}
		private bool tDTX出力・全チップ・解像度をＮ分の１にできる( int N, ref int[ , ] arrチップ配列, int n現在の解像度 )
		{
			if( ( n現在の解像度 % N ) != 0 )
			{
				return false;
			}
			for( int i = 0; i < ( n現在の解像度 / N ); i++ )
			{
				for( int j = 1; j < N; j++ )
				{
					if( arrチップ配列[ ( i * N ) + j, 0 ] != 0 )
					{
						return false;
					}
				}
			}
			return true;
		}

		#region [#23880 2010.12.30 yyagi: コンマとスペースの両方を小数点として扱うTryParse]
		/// <summary>
		/// 小数点としてコンマとピリオドの両方を受け付けるTryParse()
		/// </summary>
		/// <param name="s">strings convert to double</param>
		/// <param name="result">parsed double value</param>
		/// <returns>s が正常に変換された場合は true。それ以外の場合は false。</returns>
		/// <exception cref="ArgumentException">style が NumberStyles 値でないか、style に NumberStyles.AllowHexSpecifier 値が含まれている</exception>
		private bool TryParse(string s, out decimal result)
		{	// #23880 2010.12.30 yyagi: alternative TryParse to permit both '.' and ',' for decimal point
			// EU諸国での #BPM 123,45 のような記述に対応するため、
			// 小数点の最終位置を検出して、それをlocaleにあった
			// 文字に置き換えてからTryParse()する
			// 桁区切りの文字はスキップする

			const string DecimalSeparators = ".,";				// 小数点文字
			const string GroupSeparators = ".,' ";				// 桁区切り文字
			const string NumberSymbols = "0123456789";			// 数値文字

			int len = s.Length;									// 文字列長
			int decimalPosition = len;							// 真の小数点の位置 最初は文字列終端位置に仮置きする

			for (int i = 0; i < len; i++)
			{							// まず、真の小数点(一番最後に現れる小数点)の位置を求める
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0)
				{				// 数値だったらスキップ
					continue;
				}
				else if (DecimalSeparators.IndexOf(c) >= 0)
				{		// 小数点文字だったら、その都度位置を上書き記憶
					decimalPosition = i;
				}
				else if (GroupSeparators.IndexOf(c) >= 0)
				{		// 桁区切り文字の場合もスキップ
					continue;
				}
				else
				{											// 数値・小数点・区切り文字以外がきたらループ終了
					break;
				}
			}

			StringBuilder decimalStr = new StringBuilder(16);
			for (int i = 0; i < len; i++)
			{							// 次に、localeにあった数値文字列を生成する
				char c = s[i];
				if (NumberSymbols.IndexOf(c) >= 0)
				{				// 数値だったら
					decimalStr.Append(c);							// そのままコピー
				}
				else if (DecimalSeparators.IndexOf(c) >= 0)
				{		// 小数点文字だったら
					if (i == decimalPosition)
					{						// 最後に出現した小数点文字なら、localeに合った小数点を出力する
						decimalStr.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
					}
				}
				else if (GroupSeparators.IndexOf(c) >= 0)
				{		// 桁区切り文字だったら
					continue;										// 何もしない(スキップ)
				}
				else
				{
					break;
				}
			}
			return decimal.TryParse(decimalStr.ToString(), out result);	// 最後に、自分のlocale向けの文字列に対してTryParse実行
		}
		#endregion
		//-----------------		//-----------------
		#endregion
	}
}
