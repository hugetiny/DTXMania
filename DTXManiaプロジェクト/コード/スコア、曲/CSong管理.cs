using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace DTXMania
{
	internal class CSongs管理
	{
		// プロパティ

		public int nSongsDBから取得できたスコア数
		{
			get; 
			set; 
		}
		public int nSongsDBへ出力できたスコア数
		{
			get;
			set;
		}
		public int nスコアキャッシュから反映できたスコア数 
		{
			get;
			set; 
		}
		public int nファイルから反映できたスコア数
		{
			get;
			set;
		}
		public int n検索されたスコア数 
		{ 
			get;
			set;
		}
		public int n検索された曲ノード数
		{
			get; 
			set;
		}
		public List<Cスコア> listSongsDB;					// songs.dbから構築されるlist
		public List<C曲リストノード> list曲ルート;			// 起動時にフォルダ検索して構築されるlist


		// コンストラクタ

		public CSongs管理()
		{
			this.listSongsDB = new List<Cスコア>();
			this.list曲ルート = new List<C曲リストノード>();
			this.n検索された曲ノード数 = 0;
			this.n検索されたスコア数 = 0;
		}


		// メソッド

		#region [ SongsDB(songs.db) を読み込む ]
		//-----------------
		public void tSongsDBを読み込む( string SongsDBファイル名 )
		{
			this.nSongsDBから取得できたスコア数 = 0;
			if( File.Exists( SongsDBファイル名 ) )
			{
				BinaryReader br = null;
				try
				{
					br = new BinaryReader( File.OpenRead( SongsDBファイル名 ) );
					if ( !br.ReadString().Equals( SONGSDB_VERSION ) )
					{
						throw new InvalidDataException( "ヘッダが異なります。" );
					}
					this.listSongsDB = new List<Cスコア>();

					while( true )
					{
						try
						{
							Cスコア item = this.tSongsDBからスコアを１つ読み込む( br );
							this.listSongsDB.Add( item );
							this.nSongsDBから取得できたスコア数++;
						}
						catch( EndOfStreamException )
						{
							break;
						}
					}
				}
				finally
				{
					if( br != null )
						br.Close();
				}
			}
		}
		//-----------------
		#endregion
		#region [ 曲を検索してリストを作成する ]
		//-----------------
		public void t曲を検索してリストを作成する( string str基点フォルダ, bool b子BOXへ再帰する )
		{
			this.t曲を検索してリストを作成する( str基点フォルダ, b子BOXへ再帰する, this.list曲ルート, null );
		}
		private void t曲を検索してリストを作成する( string str基点フォルダ, bool b子BOXへ再帰する, List<C曲リストノード> listノードリスト, C曲リストノード node親 )
		{
			if( !str基点フォルダ.EndsWith( @"\" ) )
				str基点フォルダ = str基点フォルダ + @"\";

			DirectoryInfo info = new DirectoryInfo( str基点フォルダ );

			if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
				Trace.TraceInformation( "基点フォルダ: " + str基点フォルダ );

			#region [ a.フォルダ内に set.def が存在する場合 → set.def からノード作成]
			//-----------------------------
			string path = str基点フォルダ + "set.def";
			if( File.Exists( path ) )
			{
				CSetDef def = new CSetDef( path );
				new FileInfo( path );
				if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
				{
					Trace.TraceInformation( "set.def検出 : {0}", new object[] { path } );
					Trace.Indent();
				}
				try
				{
					for( int i = 0; i < def.blocks.Count; i++ )
					{
						CSetDef.CBlock block = def.blocks[ i ];
						C曲リストノード item = new C曲リストノード();
						item.eノード種別 = C曲リストノード.Eノード種別.SCORE;
						item.strタイトル = block.Title;
						item.strジャンル = block.Genre;
						item.nスコア数 = 0;
						item.col文字色 = block.FontColor;
						item.SetDefのブロック番号 = i;
						item.pathSetDefの絶対パス = path;
						item.r親ノード = node親;
						for( int j = 0; j < 5; j++ )
						{
							if( !string.IsNullOrEmpty( block.File[ j ] ) )
							{
								string str2 = str基点フォルダ + block.File[ j ];
								if( File.Exists( str2 ) )
								{
									item.ar難易度ラベル[ j ] = block.Label[ j ];
									item.arスコア[ j ] = new Cスコア();
									item.arスコア[ j ].ファイル情報.ファイルの絶対パス = str2;
									item.arスコア[ j ].ファイル情報.フォルダの絶対パス = Path.GetFullPath( Path.GetDirectoryName( str2 ) ) + @"\";
									FileInfo info2 = new FileInfo( str2 );
									item.arスコア[ j ].ファイル情報.ファイルサイズ = info2.Length;
									item.arスコア[ j ].ファイル情報.最終更新日時 = info2.LastWriteTime;
									string str3 = str2 + ".score.ini";
									if( File.Exists( str3 ) )
									{
										FileInfo info3 = new FileInfo( str3 );
										item.arスコア[ j ].ScoreIni情報.ファイルサイズ = info3.Length;
										item.arスコア[ j ].ScoreIni情報.最終更新日時 = info3.LastWriteTime;
									}
									item.nスコア数++;
									this.n検索されたスコア数++;
								}
								else
								{
									item.arスコア[ j ] = null;
								}
							}
						}
						if( item.nスコア数 > 0 )
						{
							listノードリスト.Add( item );
							this.n検索された曲ノード数++;
							if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
							{
								StringBuilder builder = new StringBuilder( 0x200 );
								builder.Append( string.Format( "nID#{0:D3}", item.nID ) );
								if( item.r親ノード != null )
								{
									builder.Append( string.Format( "(in#{0:D3}):", item.r親ノード.nID ) );
								}
								else
								{
									builder.Append( "(onRoot):" );
								}
								if( ( item.strタイトル != null ) && ( item.strタイトル.Length > 0 ) )
								{
									builder.Append( " SONG, Title=" + item.strタイトル );
								}
								if( ( item.strジャンル != null ) && ( item.strジャンル.Length > 0 ) )
								{
									builder.Append( ", Genre=" + item.strジャンル );
								}
								if( item.col文字色 != Color.White )
								{
									builder.Append( ", FontColor=" + item.col文字色 );
								}
								Trace.TraceInformation( builder.ToString() );
								Trace.Indent();
								try
								{
									for( int k = 0; k < 5; k++ )
									{
										if( item.arスコア[ k ] != null )
										{
											Cスコア cスコア = item.arスコア[ k ];
											builder.Remove( 0, builder.Length );
											builder.Append( string.Format( "ブロック{0}-{1}:", item.SetDefのブロック番号 + 1, k + 1 ) );
											builder.Append( " Label=" + item.ar難易度ラベル[ k ] );
											builder.Append( ", File=" + cスコア.ファイル情報.ファイルの絶対パス );
											builder.Append( ", Size=" + cスコア.ファイル情報.ファイルサイズ );
											builder.Append( ", LastUpdate=" + cスコア.ファイル情報.最終更新日時 );
											Trace.TraceInformation( builder.ToString() );
										}
									}
								}
								finally
								{
									Trace.Unindent();
								}
							}
						}
					}
				}
				finally
				{
					if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.Unindent();
					}
				}
			}
			//-----------------------------
			#endregion

			#region [ b.フォルダ内に set.def が存在しない場合 → 個別ファイルからノード作成 ]
			//-----------------------------
			else
			{
				foreach( FileInfo info4 in info.GetFiles() )
				{
					string str4 = info4.Extension.ToLower();
					if( ( str4.Equals( ".dtx" ) || str4.Equals( ".gda" ) ) || ( ( str4.Equals( ".g2d" ) || str4.Equals( ".bms" ) ) || str4.Equals( ".bme" ) ) )
					{
						C曲リストノード c曲リストノード2 = new C曲リストノード();
						c曲リストノード2.eノード種別 = C曲リストノード.Eノード種別.SCORE;
						c曲リストノード2.nスコア数 = 1;
						c曲リストノード2.r親ノード = node親;
						c曲リストノード2.arスコア[ 0 ] = new Cスコア();
						c曲リストノード2.arスコア[ 0 ].ファイル情報.ファイルの絶対パス = str基点フォルダ + info4.Name;
						c曲リストノード2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = str基点フォルダ;
						c曲リストノード2.arスコア[ 0 ].ファイル情報.ファイルサイズ = info4.Length;
						c曲リストノード2.arスコア[ 0 ].ファイル情報.最終更新日時 = info4.LastWriteTime;
						string str5 = c曲リストノード2.arスコア[ 0 ].ファイル情報.ファイルの絶対パス + ".score.ini";
						if( File.Exists( str5 ) )
						{
							FileInfo info5 = new FileInfo( str5 );
							c曲リストノード2.arスコア[ 0 ].ScoreIni情報.ファイルサイズ = info5.Length;
							c曲リストノード2.arスコア[ 0 ].ScoreIni情報.最終更新日時 = info5.LastWriteTime;
						}
						this.n検索されたスコア数++;
						listノードリスト.Add( c曲リストノード2 );
						this.n検索された曲ノード数++;
						if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
						{
							Trace.Indent();
							try
							{
								StringBuilder builder2 = new StringBuilder( 0x100 );
								builder2.Append( string.Format( "nID#{0:D3}", c曲リストノード2.nID ) );
								if( c曲リストノード2.r親ノード != null )
								{
									builder2.Append( string.Format( "(in#{0:D3}):", c曲リストノード2.r親ノード.nID ) );
								}
								else
								{
									builder2.Append( "(onRoot):" );
								}
								builder2.Append( " SONG, File=" + c曲リストノード2.arスコア[ 0 ].ファイル情報.ファイルの絶対パス );
								builder2.Append( ", Size=" + c曲リストノード2.arスコア[ 0 ].ファイル情報.ファイルサイズ );
								builder2.Append( ", LastUpdate=" + c曲リストノード2.arスコア[ 0 ].ファイル情報.最終更新日時 );
								Trace.TraceInformation( builder2.ToString() );
							}
							finally
							{
								Trace.Unindent();
							}
						}
					}
					else if( !str4.Equals( ".mid" ) )
					{
						str4.Equals( ".smf" );
					}
				}
			}
			//-----------------------------
			#endregion

			foreach( DirectoryInfo info6 in info.GetDirectories() )
			{
				#region [ a. "dtxfiles." で始まるフォルダの場合 ]
				//-----------------------------
				if( info6.Name.ToLower().StartsWith( "dtxfiles." ) )
				{
					C曲リストノード c曲リストノード3 = new C曲リストノード();
					c曲リストノード3.eノード種別 = C曲リストノード.Eノード種別.BOX;
					c曲リストノード3.bDTXFilesで始まるフォルダ名のBOXである = true;
					c曲リストノード3.strタイトル = info6.Name.Substring( 9 );
					c曲リストノード3.nスコア数 = 1;
					c曲リストノード3.r親ノード = node親;
					c曲リストノード3.list子リスト = new List<C曲リストノード>();
					c曲リストノード3.arスコア[ 0 ] = new Cスコア();
					c曲リストノード3.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = info6.FullName + @"\";
					c曲リストノード3.arスコア[ 0 ].譜面情報.タイトル = c曲リストノード3.strタイトル;
					c曲リストノード3.arスコア[ 0 ].譜面情報.コメント =
						(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ?
						"BOX に移動します。" :
						"Enter into the BOX.";
					listノードリスト.Add(c曲リストノード3);
					if( File.Exists( info6.FullName + @"\box.def" ) )
					{
						CBoxDef def2 = new CBoxDef( info6.FullName + @"\box.def" );
						if( ( def2.Title != null ) && ( def2.Title.Length > 0 ) )
						{
							c曲リストノード3.strタイトル = def2.Title;
						}
						if( ( def2.Genre != null ) && ( def2.Genre.Length > 0 ) )
						{
							c曲リストノード3.strジャンル = def2.Genre;
						}
						if( def2.Color != Color.White )
						{
							c曲リストノード3.col文字色 = def2.Color;
						}
						if( ( def2.Artist != null ) && ( def2.Artist.Length > 0 ) )
						{
							c曲リストノード3.arスコア[ 0 ].譜面情報.アーティスト名 = def2.Artist;
						}
						if( ( def2.Comment != null ) && ( def2.Comment.Length > 0 ) )
						{
							c曲リストノード3.arスコア[ 0 ].譜面情報.コメント = def2.Comment;
						}
						if( ( def2.Preimage != null ) && ( def2.Preimage.Length > 0 ) )
						{
							c曲リストノード3.arスコア[ 0 ].譜面情報.Preimage = def2.Preimage;
						}
						if( ( def2.Premovie != null ) && ( def2.Premovie.Length > 0 ) )
						{
							c曲リストノード3.arスコア[ 0 ].譜面情報.Premovie = def2.Premovie;
						}
						if( ( def2.Presound != null ) && ( def2.Presound.Length > 0 ) )
						{
							c曲リストノード3.arスコア[ 0 ].譜面情報.Presound = def2.Presound;
						}
						if( def2.PerfectRange >= 0 )
						{
							c曲リストノード3.nPerfect範囲ms = def2.PerfectRange;
						}
						if( def2.GreatRange >= 0 )
						{
							c曲リストノード3.nGreat範囲ms = def2.GreatRange;
						}
						if( def2.GoodRange >= 0 )
						{
							c曲リストノード3.nGood範囲ms = def2.GoodRange;
						}
						if( def2.PoorRange >= 0 )
						{
							c曲リストノード3.nPoor範囲ms = def2.PoorRange;
						}
					}
					if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.Indent();
						try
						{
							StringBuilder builder3 = new StringBuilder( 0x100 );
							builder3.Append( string.Format( "nID#{0:D3}", c曲リストノード3.nID ) );
							if( c曲リストノード3.r親ノード != null )
							{
								builder3.Append( string.Format( "(in#{0:D3}):", c曲リストノード3.r親ノード.nID ) );
							}
							else
							{
								builder3.Append( "(onRoot):" );
							}
							builder3.Append( " BOX, Title=" + c曲リストノード3.strタイトル );
							builder3.Append( ", Folder=" + c曲リストノード3.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
							builder3.Append( ", Comment=" + c曲リストノード3.arスコア[ 0 ].譜面情報.コメント );
							Trace.TraceInformation( builder3.ToString() );
						}
						finally
						{
							Trace.Unindent();
						}
					}
					if( b子BOXへ再帰する )
					{
						this.t曲を検索してリストを作成する( info6.FullName + @"\", b子BOXへ再帰する, c曲リストノード3.list子リスト, c曲リストノード3 );
					}
				}
				//-----------------------------
				#endregion

				#region [ b.box.def を含むフォルダの場合  ]
				//-----------------------------
				else if( File.Exists( info6.FullName + @"\box.def" ) )
				{
					CBoxDef def3 = new CBoxDef( info6.FullName + @"\box.def" );
					C曲リストノード c曲リストノード4 = new C曲リストノード();
					c曲リストノード4.eノード種別 = C曲リストノード.Eノード種別.BOX;
					c曲リストノード4.bDTXFilesで始まるフォルダ名のBOXである = false;
					c曲リストノード4.strタイトル = def3.Title;
					c曲リストノード4.strジャンル = def3.Genre;
					c曲リストノード4.col文字色 = def3.Color;
					c曲リストノード4.nスコア数 = 1;
					c曲リストノード4.arスコア[ 0 ] = new Cスコア();
					c曲リストノード4.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = info6.FullName + @"\";
					c曲リストノード4.arスコア[ 0 ].譜面情報.タイトル = def3.Title;
					c曲リストノード4.arスコア[ 0 ].譜面情報.ジャンル = def3.Genre;
					c曲リストノード4.arスコア[ 0 ].譜面情報.アーティスト名 = def3.Artist;
					c曲リストノード4.arスコア[ 0 ].譜面情報.コメント = def3.Comment;
					c曲リストノード4.arスコア[ 0 ].譜面情報.Preimage = def3.Preimage;
					c曲リストノード4.arスコア[ 0 ].譜面情報.Premovie = def3.Premovie;
					c曲リストノード4.arスコア[ 0 ].譜面情報.Presound = def3.Presound;
					c曲リストノード4.r親ノード = node親;
					c曲リストノード4.list子リスト = new List<C曲リストノード>();
					c曲リストノード4.nPerfect範囲ms = def3.PerfectRange;
					c曲リストノード4.nGreat範囲ms = def3.GreatRange;
					c曲リストノード4.nGood範囲ms = def3.GoodRange;
					c曲リストノード4.nPoor範囲ms = def3.PoorRange;
					listノードリスト.Add( c曲リストノード4 );
					if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.TraceInformation( "box.def検出 : {0}", new object[] { info6.FullName + @"\box.def" } );
						Trace.Indent();
						try
						{
							StringBuilder builder4 = new StringBuilder( 0x400 );
							builder4.Append( string.Format( "nID#{0:D3}", c曲リストノード4.nID ) );
							if( c曲リストノード4.r親ノード != null )
							{
								builder4.Append( string.Format( "(in#{0:D3}):", c曲リストノード4.r親ノード.nID ) );
							}
							else
							{
								builder4.Append( "(onRoot):" );
							}
							builder4.Append( "BOX, Title=" + c曲リストノード4.strタイトル );
							if( ( c曲リストノード4.strジャンル != null ) && ( c曲リストノード4.strジャンル.Length > 0 ) )
							{
								builder4.Append( ", Genre=" + c曲リストノード4.strジャンル );
							}
							if( ( c曲リストノード4.arスコア[ 0 ].譜面情報.アーティスト名 != null ) && ( c曲リストノード4.arスコア[ 0 ].譜面情報.アーティスト名.Length > 0 ) )
							{
								builder4.Append( ", Artist=" + c曲リストノード4.arスコア[ 0 ].譜面情報.アーティスト名 );
							}
							if( ( c曲リストノード4.arスコア[ 0 ].譜面情報.コメント != null ) && ( c曲リストノード4.arスコア[ 0 ].譜面情報.コメント.Length > 0 ) )
							{
								builder4.Append( ", Comment=" + c曲リストノード4.arスコア[ 0 ].譜面情報.コメント );
							}
							if( ( c曲リストノード4.arスコア[ 0 ].譜面情報.Preimage != null ) && ( c曲リストノード4.arスコア[ 0 ].譜面情報.Preimage.Length > 0 ) )
							{
								builder4.Append( ", Preimage=" + c曲リストノード4.arスコア[ 0 ].譜面情報.Preimage );
							}
							if( ( c曲リストノード4.arスコア[ 0 ].譜面情報.Premovie != null ) && ( c曲リストノード4.arスコア[ 0 ].譜面情報.Premovie.Length > 0 ) )
							{
								builder4.Append( ", Premovie=" + c曲リストノード4.arスコア[ 0 ].譜面情報.Premovie );
							}
							if( ( c曲リストノード4.arスコア[ 0 ].譜面情報.Presound != null ) && ( c曲リストノード4.arスコア[ 0 ].譜面情報.Presound.Length > 0 ) )
							{
								builder4.Append( ", Presound=" + c曲リストノード4.arスコア[ 0 ].譜面情報.Presound );
							}
							if( c曲リストノード4.col文字色 != ColorTranslator.FromHtml( "White" ) )
							{
								builder4.Append( ", FontColor=" + c曲リストノード4.col文字色 );
							}
							if( c曲リストノード4.nPerfect範囲ms != -1 )
							{
								builder4.Append( ", Perfect=" + c曲リストノード4.nPerfect範囲ms + "ms" );
							}
							if( c曲リストノード4.nGreat範囲ms != -1 )
							{
								builder4.Append( ", Great=" + c曲リストノード4.nGreat範囲ms + "ms" );
							}
							if( c曲リストノード4.nGood範囲ms != -1 )
							{
								builder4.Append( ", Good=" + c曲リストノード4.nGood範囲ms + "ms" );
							}
							if( c曲リストノード4.nPoor範囲ms != -1 )
							{
								builder4.Append( ", Poor=" + c曲リストノード4.nPoor範囲ms + "ms" );
							}
							Trace.TraceInformation( builder4.ToString() );
						}
						finally
						{
							Trace.Unindent();
						}
					}
					if( b子BOXへ再帰する )
					{
						this.t曲を検索してリストを作成する( info6.FullName + @"\", b子BOXへ再帰する, c曲リストノード4.list子リスト, c曲リストノード4 );
					}
				}
				//-----------------------------
				#endregion

				#region [ c.通常フォルダの場合 ]
				//-----------------------------
				else
				{
					this.t曲を検索してリストを作成する( info6.FullName + @"\", b子BOXへ再帰する, listノードリスト, node親 );
				}
				//-----------------------------
				#endregion
			}
		}
		//-----------------
		#endregion
		#region [ スコアキャッシュを曲リストに反映する ]
		//-----------------
		public void tスコアキャッシュを曲リストに反映する()
		{
			this.nスコアキャッシュから反映できたスコア数 = 0;
			this.tスコアキャッシュを曲リストに反映する( this.list曲ルート );
		}
		private void tスコアキャッシュを曲リストに反映する( List<C曲リストノード> ノードリスト )
		{
			using( List<C曲リストノード>.Enumerator enumerator = ノードリスト.GetEnumerator() )
			{
				while( enumerator.MoveNext() )
				{
					C曲リストノード node = enumerator.Current;
					if( node.eノード種別 == C曲リストノード.Eノード種別.BOX )
					{
						this.tスコアキャッシュを曲リストに反映する( node.list子リスト );
					}
					else if( ( node.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( node.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						Predicate<Cスコア> match = null;
						for( int lv = 0; lv < 5; lv++ )
						{
							if( node.arスコア[ lv ] != null )
							{
								if( match == null )
								{
									match = delegate( Cスコア sc )
									{
										return ( ( sc.ファイル情報.ファイルの絶対パス.Equals( node.arスコア[ lv ].ファイル情報.ファイルの絶対パス )
											&& sc.ファイル情報.ファイルサイズ.Equals( node.arスコア[ lv ].ファイル情報.ファイルサイズ ) )
											&& ( sc.ファイル情報.最終更新日時.Equals( node.arスコア[ lv ].ファイル情報.最終更新日時 )
											&& sc.ScoreIni情報.ファイルサイズ.Equals( node.arスコア[ lv ].ScoreIni情報.ファイルサイズ ) ) )
											&& sc.ScoreIni情報.最終更新日時.Equals( node.arスコア[ lv ].ScoreIni情報.最終更新日時 );
									};
								}
								int num = this.listSongsDB.FindIndex( match );
								if( num == -1 )
								{
									if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
									{
										Trace.TraceInformation( "songs.db に存在しません。({0})", new object[] { node.arスコア[ lv ].ファイル情報.ファイルの絶対パス } );
									}
								}
								else
								{
									node.arスコア[ lv ].譜面情報 = this.listSongsDB[ num ].譜面情報;
									node.arスコア[ lv ].bSongDBにキャッシュがあった = true;
									if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
									{
										Trace.TraceInformation( "songs.db から転記しました。({0})", new object[] { node.arスコア[ lv ].ファイル情報.ファイルの絶対パス } );
									}
									this.nスコアキャッシュから反映できたスコア数++;
									if( node.arスコア[ lv ].ScoreIni情報.最終更新日時 != this.listSongsDB[ num ].ScoreIni情報.最終更新日時 )
									{
										string str = node.arスコア[ lv ].ファイル情報.ファイルの絶対パス + ".score.ini";
										try
										{
											CScoreIni ini = new CScoreIni( str );
											ini.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();
											for( int i = 0; i < 3; i++ )
											{
												int num3 = ( i * 2 ) + 1;
												if( ( ini.stセクション[ num3 ].b演奏にMIDI入力を使用した || ini.stセクション[ num3 ].b演奏にキーボードを使用した ) || ( ini.stセクション[ num3 ].b演奏にジョイパッドを使用した || ini.stセクション[ num3 ].b演奏にマウスを使用した ) )
												{
													node.arスコア[ lv ].譜面情報.最大ランク[ i ] = 
														(ini.stファイル.BestRank[i] != (int)CScoreIni.ERANK.UNKNOWN)?
														(int)ini.stファイル.BestRank[i] : CScoreIni.tランク値を計算して返す( ini.stセクション[ num3 ] );
												}
												else
												{
													node.arスコア[ lv ].譜面情報.最大ランク[ i ] = (int)CScoreIni.ERANK.UNKNOWN;
												}
												node.arスコア[ lv ].譜面情報.最大演奏型スキル[ i ] = ini.stセクション[ num3 ].db演奏型スキル値;
												node.arスコア[ lv ].譜面情報.最大ゲーム型スキル[ i ] = ini.stセクション[ num3 ].dbゲーム型スキル値;	// #23624 2011.5.10 yyagi
												node.arスコア[ lv ].譜面情報.フルコンボ[ i ] = ini.stセクション[ num3 ].bフルコンボである;
											}
											node.arスコア[ lv ].譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
											node.arスコア[ lv ].譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
											node.arスコア[ lv ].譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
											for( int i = 0; i < 5; i++ )
											{
												node.arスコア[ lv ].譜面情報.演奏履歴[ i ] = ini.stファイル.History[ i ];
											}
											if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
											{
												Trace.TraceInformation( "演奏記録ファイルから HiSkill 情報と演奏履歴を取得しました。({0})", new object[] { str } );
											}
										}
										catch
										{
											Trace.TraceError( "演奏記録ファイルの読み込みに失敗しました。({0})", new object[] { str } );
										}
									}
								}
							}
						}
					}
				}
			}
		}
		private Cスコア tSongsDBからスコアを１つ読み込む( BinaryReader br )
		{
			Cスコア cスコア = new Cスコア();
			cスコア.ファイル情報.ファイルの絶対パス = br.ReadString();
			cスコア.ファイル情報.フォルダの絶対パス = br.ReadString();
			cスコア.ファイル情報.最終更新日時 = new DateTime( br.ReadInt64() );
			cスコア.ファイル情報.ファイルサイズ = br.ReadInt64();
			cスコア.ScoreIni情報.最終更新日時 = new DateTime( br.ReadInt64() );
			cスコア.ScoreIni情報.ファイルサイズ = br.ReadInt64();
			cスコア.譜面情報.タイトル = br.ReadString();
			cスコア.譜面情報.アーティスト名 = br.ReadString();
			cスコア.譜面情報.コメント = br.ReadString();
			cスコア.譜面情報.ジャンル = br.ReadString();
			cスコア.譜面情報.Preimage = br.ReadString();
			cスコア.譜面情報.Premovie = br.ReadString();
			cスコア.譜面情報.Presound = br.ReadString();
			cスコア.譜面情報.Backgound = br.ReadString();
			cスコア.譜面情報.レベル.Drums = br.ReadInt32();
			cスコア.譜面情報.レベル.Guitar = br.ReadInt32();
			cスコア.譜面情報.レベル.Bass = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Drums = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Guitar = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Bass = br.ReadInt32();
			cスコア.譜面情報.最大演奏型スキル.Drums = br.ReadDouble();
			cスコア.譜面情報.最大演奏型スキル.Guitar = br.ReadDouble();
			cスコア.譜面情報.最大演奏型スキル.Bass = br.ReadDouble();
			cスコア.譜面情報.最大ゲーム型スキル.Drums = br.ReadDouble();		// #23624 2011.5.10 yyagi
			cスコア.譜面情報.最大ゲーム型スキル.Guitar = br.ReadDouble();		//
			cスコア.譜面情報.最大ゲーム型スキル.Bass = br.ReadDouble();			//
			cスコア.譜面情報.フルコンボ.Drums = br.ReadBoolean();
			cスコア.譜面情報.フルコンボ.Guitar = br.ReadBoolean();
			cスコア.譜面情報.フルコンボ.Bass = br.ReadBoolean();
			cスコア.譜面情報.演奏回数.Drums = br.ReadInt32();
			cスコア.譜面情報.演奏回数.Guitar = br.ReadInt32();
			cスコア.譜面情報.演奏回数.Bass = br.ReadInt32();
			cスコア.譜面情報.演奏履歴.行1 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行2 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行3 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行4 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行5 = br.ReadString();
			cスコア.譜面情報.レベルを非表示にする = br.ReadBoolean();
			cスコア.譜面情報.曲種別 = (CDTX.E種別) br.ReadInt32();
			cスコア.譜面情報.bpm = br.ReadDouble();
			return cスコア;
		}
		//-----------------
		#endregion
		#region [ SongsDBになかった曲をファイルから読み込んで反映する ]
		//-----------------
		public void tSongsDBになかった曲をファイルから読み込んで反映する()
		{
			this.nファイルから反映できたスコア数 = 0;
			this.tSongsDBになかった曲をファイルから読み込んで反映する( this.list曲ルート );
		}
		private void tSongsDBになかった曲をファイルから読み込んで反映する( List<C曲リストノード> ノードリスト )
		{
			foreach( C曲リストノード c曲リストノード in ノードリスト )
			{
				if( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX )
				{
					this.tSongsDBになかった曲をファイルから読み込んで反映する( c曲リストノード.list子リスト );
				}
				else if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
				{
					for( int i = 0; i < 5; i++ )
					{
						if( ( c曲リストノード.arスコア[ i ] != null ) && !c曲リストノード.arスコア[ i ].bSongDBにキャッシュがあった )
						{
							string path = c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス;
							if( File.Exists( path ) )
							{
								try
								{
									CDTX cdtx = new CDTX( c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス, true );
									c曲リストノード.arスコア[ i ].譜面情報.タイトル = cdtx.TITLE;
									c曲リストノード.arスコア[ i ].譜面情報.アーティスト名 = cdtx.ARTIST;
									c曲リストノード.arスコア[ i ].譜面情報.コメント = cdtx.COMMENT;
									c曲リストノード.arスコア[ i ].譜面情報.ジャンル = cdtx.GENRE;
									c曲リストノード.arスコア[ i ].譜面情報.Preimage = cdtx.PREIMAGE;
									c曲リストノード.arスコア[ i ].譜面情報.Premovie = cdtx.PREMOVIE;
									c曲リストノード.arスコア[ i ].譜面情報.Presound = cdtx.PREVIEW;
									c曲リストノード.arスコア[ i ].譜面情報.Backgound = ( ( cdtx.BACKGROUND != null ) && ( cdtx.BACKGROUND.Length > 0 ) ) ? cdtx.BACKGROUND : cdtx.BACKGROUND_GR;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Drums = cdtx.LEVEL.Drums;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Guitar = cdtx.LEVEL.Guitar;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Bass = cdtx.LEVEL.Bass;
									c曲リストノード.arスコア[ i ].譜面情報.レベルを非表示にする = cdtx.HIDDENLEVEL;
									c曲リストノード.arスコア[ i ].譜面情報.曲種別 = cdtx.e種別;
									c曲リストノード.arスコア[ i ].譜面情報.bpm = cdtx.BPM;
									this.nファイルから反映できたスコア数++;
									cdtx.On非活性化();
									if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
									{
										StringBuilder builder = new StringBuilder( 0x400 );
										builder.Append( string.Format( "曲データファイルから譜面情報を転記しました。({0})", path ) );
										builder.Append( "(title=" + c曲リストノード.arスコア[ i ].譜面情報.タイトル );
										builder.Append( ", artist=" + c曲リストノード.arスコア[ i ].譜面情報.アーティスト名 );
										builder.Append( ", comment=" + c曲リストノード.arスコア[ i ].譜面情報.コメント );
										builder.Append( ", genre=" + c曲リストノード.arスコア[ i ].譜面情報.ジャンル );
										builder.Append( ", preimage=" + c曲リストノード.arスコア[ i ].譜面情報.Preimage );
										builder.Append( ", premovie=" + c曲リストノード.arスコア[ i ].譜面情報.Premovie );
										builder.Append( ", presound=" + c曲リストノード.arスコア[ i ].譜面情報.Presound );
										builder.Append( ", background=" + c曲リストノード.arスコア[ i ].譜面情報.Backgound );
										builder.Append( ", lvDr=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Drums );
										builder.Append( ", lvGt=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Guitar );
										builder.Append( ", lvBs=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Bass );
										builder.Append( ", lvHide=" + c曲リストノード.arスコア[ i ].譜面情報.レベルを非表示にする );
										builder.Append( ", type=" + c曲リストノード.arスコア[ i ].譜面情報.曲種別 );
										builder.Append( ", bpm=" + c曲リストノード.arスコア[ i ].譜面情報.bpm );
										Trace.TraceInformation( builder.ToString() );
									}
								}
								catch( Exception exception )
								{
									Trace.TraceError( exception.Message );
									c曲リストノード.arスコア[ i ] = null;
									c曲リストノード.nスコア数--;
									this.n検索されたスコア数--;
									Trace.TraceError( "曲データファイルの読み込みに失敗しました。({0})", new object[] { path } );
								}
							}
							string str2 = c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス + ".score.ini";
							if( File.Exists( str2 ) )
							{
								try
								{
									CScoreIni ini = new CScoreIni( str2 );
									ini.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();
									for( int j = 0; j < 3; j++ )
									{
										int hiskill = ( j * 3 ) + 1;
										if( ( ini.stセクション[ hiskill ].b演奏にMIDI入力を使用した || ini.stセクション[ hiskill ].b演奏にキーボードを使用した ) || ( ini.stセクション[ hiskill ].b演奏にジョイパッドを使用した || ini.stセクション[ hiskill ].b演奏にマウスを使用した ) )
										{
											c曲リストノード.arスコア[ i ].譜面情報.最大ランク[ j ] = CScoreIni.tランク値を計算して返す( ini.stセクション[ hiskill ].n全チップ数, ini.stセクション[ hiskill ].nPerfect数, ini.stセクション[ hiskill ].nGreat数, ini.stセクション[ hiskill ].nGood数, ini.stセクション[ hiskill ].nPoor数, ini.stセクション[ hiskill ].nMiss数 );
										}
										else
										{
											c曲リストノード.arスコア[ i ].譜面情報.最大ランク[ j ] = (int) CScoreIni.ERANK.UNKNOWN;	// 0x63;
										}
										c曲リストノード.arスコア[ i ].譜面情報.最大演奏型スキル[ j ] = ini.stセクション[ hiskill ].db演奏型スキル値;
										c曲リストノード.arスコア[ i ].譜面情報.最大ゲーム型スキル[ j ] = ini.stセクション[ hiskill ].dbゲーム型スキル値;	// #23624 2011.5.10 yyagi
										c曲リストノード.arスコア[ i ].譜面情報.フルコンボ[ j ] = ini.stセクション[ hiskill ].bフルコンボである;
									}
									c曲リストノード.arスコア[ i ].譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
									c曲リストノード.arスコア[ i ].譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
									c曲リストノード.arスコア[ i ].譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
									for( int j = 0; j < 5; j++ )
									{
										c曲リストノード.arスコア[ i ].譜面情報.演奏履歴[ j ] = ini.stファイル.History[ j ];
									}
								}
								catch
								{
									Trace.TraceError( "演奏記録ファイルの読み込みに失敗しました。[{0}]", new object[] { str2 } );
								}
							}
						}
					}
				}
			}
		}
		//-----------------
		#endregion
		#region [ 曲リストへ後処理を適用する ]
		//-----------------
		public void t曲リストへ後処理を適用する()
		{
			this.t曲リストへ後処理を適用する( this.list曲ルート );
		}
		private void t曲リストへ後処理を適用する( List<C曲リストノード> ノードリスト )
		{
			#region [ リストに１つ以上の曲があるなら RANDOM BOX を入れる ]
			//-----------------------------
			if( ノードリスト.Count > 0 )
			{
				C曲リストノード item = new C曲リストノード();
				item.eノード種別 = C曲リストノード.Eノード種別.RANDOM;
				item.strタイトル = "< RANDOM SELECT >";
				item.nスコア数 = 5;
				item.r親ノード = ノードリスト[ 0 ].r親ノード;
				for( int i = 0; i < 5; i++ )
				{
					item.arスコア[ i ] = new Cスコア();
					item.arスコア[ i ].譜面情報.タイトル = string.Format( "< RANDOM SELECT Lv.{0} >", i + 1 );
					item.arスコア[i].譜面情報.コメント =
						 (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ?
						 string.Format("難易度レベル {0} 付近の曲をランダムに選択します。難易度レベルを持たない曲も選択候補となります。", i + 1) :
						 string.Format("Random select from the songs which has the level about L{0}. Non-leveled songs may also selected.", i + 1);
					item.ar難易度ラベル[ i ] = string.Format( "L{0}", i + 1 );
				}
				ノードリスト.Add( item );

				#region [ ログ出力 ]
				//-----------------------------
				if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
				{
					StringBuilder builder = new StringBuilder( 0x100 );
					builder.Append( string.Format( "nID#{0:D3}", item.nID ) );
					if( item.r親ノード != null )
					{
						builder.Append( string.Format( "(in#{0:D3}):", item.r親ノード.nID ) );
					}
					else
					{
						builder.Append( "(onRoot):" );
					}
					builder.Append( " RANDOM" );
					Trace.TraceInformation( builder.ToString() );
				}
				//-----------------------------
				#endregion
			}
			//-----------------------------
			#endregion

			// すべてのノードについて…
			foreach( C曲リストノード c曲リストノード2 in ノードリスト )
			{
				#region [ BOXノードなら子リストに <<BACK を入れ、子リストに後処理を適用する ]
				//-----------------------------
				if( c曲リストノード2.eノード種別 == C曲リストノード.Eノード種別.BOX )
				{
					C曲リストノード c曲リストノード3 = new C曲リストノード();
					c曲リストノード3.eノード種別 = C曲リストノード.Eノード種別.BACKBOX;
					c曲リストノード3.strタイトル = "<< BACK";
					c曲リストノード3.nスコア数 = 1;
					c曲リストノード3.r親ノード = c曲リストノード2;
					c曲リストノード3.arスコア[ 0 ] = new Cスコア();
					c曲リストノード3.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = "";
					c曲リストノード3.arスコア[ 0 ].譜面情報.タイトル = c曲リストノード3.strタイトル;
					c曲リストノード3.arスコア[ 0 ].譜面情報.コメント =
						(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ?
						"BOX を出ます。" :
						"Exit from the BOX.";
					c曲リストノード2.list子リスト.Insert( 0, c曲リストノード3 );

					#region [ ログ出力 ]
					//-----------------------------
					if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
					{
						StringBuilder builder2 = new StringBuilder( 0x100 );
						builder2.Append( string.Format( "nID#{0:D3}", c曲リストノード3.nID ) );
						if( c曲リストノード3.r親ノード != null )
						{
							builder2.Append( string.Format( "(in#{0:D3}):", c曲リストノード3.r親ノード.nID ) );
						}
						else
						{
							builder2.Append( "(onRoot):" );
						}
						builder2.Append( " BACKBOX" );
						Trace.TraceInformation( builder2.ToString() );
					}
					//-----------------------------
					#endregion

					this.t曲リストへ後処理を適用する( c曲リストノード2.list子リスト );
					continue;
				}
				//-----------------------------
				#endregion

				#region [ ノードにタイトルがないなら、最初に見つけたスコアのタイトルを設定する ]
				//-----------------------------
				if( string.IsNullOrEmpty( c曲リストノード2.strタイトル ) )
				{
					for( int j = 0; j < 5; j++ )
					{
						if( ( c曲リストノード2.arスコア[ j ] != null ) && !string.IsNullOrEmpty( c曲リストノード2.arスコア[ j ].譜面情報.タイトル ) )
						{
							c曲リストノード2.strタイトル = c曲リストノード2.arスコア[ j ].譜面情報.タイトル;

							if( CDTXMania.ConfigIni.bLog曲検索ログ出力 )
								Trace.TraceInformation( "タイトルを設定しました。(nID#{0:D3}, title={1})", new object[] { c曲リストノード2.nID, c曲リストノード2.strタイトル } );

							break;
						}
					}
				}
				//-----------------------------
				#endregion
			}

			#region [ ノードをソートする ]
			//-----------------------------
			this.t曲リストのソート1_絶対パス順( ノードリスト );
			//-----------------------------
			#endregion
		}
		//-----------------
		#endregion
		#region [ スコアキャッシュをSongsDBに出力する ]
		//-----------------
		public void tスコアキャッシュをSongsDBに出力する( string SongsDBファイル名 )
		{
			this.nSongsDBへ出力できたスコア数 = 0;
			try
			{
				BinaryWriter bw = new BinaryWriter( new FileStream( SongsDBファイル名, FileMode.Create, FileAccess.Write ) );
				bw.Write( SONGSDB_VERSION );
				this.tSongsDBにリストを１つ出力する( bw, this.list曲ルート );
				bw.Close();
			}
			catch
			{
				Trace.TraceError( "songs.dbの出力に失敗しました。" );
			}
		}
		private void tSongsDBにノードを１つ出力する( BinaryWriter bw, C曲リストノード node )
		{
			for( int i = 0; i < 5; i++ )
			{
				if( node.arスコア[ i ] != null )
				{
					bw.Write( node.arスコア[ i ].ファイル情報.ファイルの絶対パス );
					bw.Write( node.arスコア[ i ].ファイル情報.フォルダの絶対パス );
					bw.Write( node.arスコア[ i ].ファイル情報.最終更新日時.Ticks );
					bw.Write( node.arスコア[ i ].ファイル情報.ファイルサイズ );
					bw.Write( node.arスコア[ i ].ScoreIni情報.最終更新日時.Ticks );
					bw.Write( node.arスコア[ i ].ScoreIni情報.ファイルサイズ );
					bw.Write( node.arスコア[ i ].譜面情報.タイトル );
					bw.Write( node.arスコア[ i ].譜面情報.アーティスト名 );
					bw.Write( node.arスコア[ i ].譜面情報.コメント );
					bw.Write( node.arスコア[ i ].譜面情報.ジャンル );
					bw.Write( node.arスコア[ i ].譜面情報.Preimage );
					bw.Write( node.arスコア[ i ].譜面情報.Premovie );
					bw.Write( node.arスコア[ i ].譜面情報.Presound );
					bw.Write( node.arスコア[ i ].譜面情報.Backgound );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.最大演奏型スキル.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.最大演奏型スキル.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.最大演奏型スキル.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.最大ゲーム型スキル.Drums );	// #23624 2011.5.10 yyagi
					bw.Write( node.arスコア[ i ].譜面情報.最大ゲーム型スキル.Guitar );	//
					bw.Write( node.arスコア[ i ].譜面情報.最大ゲーム型スキル.Bass );	//
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行1 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行2 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行3 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行4 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行5 );
					bw.Write( node.arスコア[ i ].譜面情報.レベルを非表示にする );
					bw.Write( (int) node.arスコア[ i ].譜面情報.曲種別 );
					bw.Write( node.arスコア[ i ].譜面情報.bpm );
					this.nSongsDBへ出力できたスコア数++;
				}
			}
		}
		private void tSongsDBにリストを１つ出力する( BinaryWriter bw, List<C曲リストノード> list )
		{
			foreach( C曲リストノード c曲リストノード in list )
			{
				if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
				{
					this.tSongsDBにノードを１つ出力する( bw, c曲リストノード );
				}
				if( c曲リストノード.list子リスト != null )
				{
					this.tSongsDBにリストを１つ出力する( bw, c曲リストノード.list子リスト );
				}
			}
		}
		//-----------------
		#endregion

		#region [ 曲リストソート ]
		//-----------------
		public void t曲リストのソート1_絶対パス順( List<C曲リストノード> ノードリスト )
		{
			ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
			{
				#region [ 共通処理 ]
				if ( n1 == n2 )
				{
					return 0;
				}
				int num = this.t比較0_共通( n1, n2 );
				if( num != 0 )
				{
					return num;
				}
				if( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
				{
					return n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
				}
				#endregion
				string str = "";
				if( string.IsNullOrEmpty( n1.pathSetDefの絶対パス ) )
				{
					for( int i = 0; i < 5; i++ )
					{
						if( n1.arスコア[ i ] != null )
						{
							str = n1.arスコア[ i ].ファイル情報.ファイルの絶対パス;
							if( str == null )
							{
								str = "";
							}
							break;
						}
					}
				}
				else
				{
					str = n1.pathSetDefの絶対パス + n1.SetDefのブロック番号.ToString( "00" );
				}
				string strB = "";
				if( string.IsNullOrEmpty( n2.pathSetDefの絶対パス ) )
				{
					for( int j = 0; j < 5; j++ )
					{
						if( n2.arスコア[ j ] != null )
						{
							strB = n2.arスコア[ j ].ファイル情報.ファイルの絶対パス;
							if( strB == null )
							{
								strB = "";
							}
							break;
						}
					}
				}
				else
				{
					strB = n2.pathSetDefの絶対パス + n2.SetDefのブロック番号.ToString( "00" );
				}
				return str.CompareTo( strB );
			} );
			foreach( C曲リストノード c曲リストノード in ノードリスト )
			{
				if( ( c曲リストノード.list子リスト != null ) && ( c曲リストノード.list子リスト.Count > 1 ) )
				{
					this.t曲リストのソート1_絶対パス順( c曲リストノード.list子リスト );
				}
			}
		}
		public void t曲リストのソート2_タイトル順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
			{
				if( n1 == n2 )
				{
					return 0;
				}
				int num = this.t比較0_共通( n1, n2 );
				if( num != 0 )
				{
					return order * num;
				}
				return order * n1.strタイトル.CompareTo( n2.strタイトル );
			} );
//			foreach( C曲リストノード c曲リストノード in ノードリスト )
//			{
//				if( ( c曲リストノード.list子リスト != null ) && ( c曲リストノード.list子リスト.Count > 1 ) )
//				{
//					this.t曲リストのソート2_タイトル順( c曲リストノード.list子リスト, part, order );
//				}
//			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ノードリスト"></param>
		/// <param name="part"></param>
		/// <param name="order">1=Ascend -1=Descend</param>
		public void t曲リストのソート3_演奏回数の多い順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if( num != 0 )
					{
						return order * num;
					}
					if( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
//					for( int i = 0; i < 5; i++ )
//					{
						if( n1.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN1 += n1.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
						if( n2.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN2 += n2.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
//					}
					num = nSumPlayCountN2 - nSumPlayCountN1;
					if( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
//					for ( int i = 0; i < 5; i++ )
//					{
						if ( c曲リストノード.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN1 += c曲リストノード.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
//					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}

//				foreach( C曲リストノード c曲リストノード in ノードリスト )
//				{
//					if( ( c曲リストノード.list子リスト != null ) && ( c曲リストノード.list子リスト.Count > 1 ) )
//					{
//						this.t曲リストのソート3_演奏回数の多い順( c曲リストノード.list子リスト, part );
//					}
//				}
			}
		}
		public void t曲リストのソート4_LEVEL順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int)p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return order * num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].譜面情報.レベル[ (int) part ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].譜面情報.レベル[ (int) part ];
					}
					num = nSumPlayCountN2 - nSumPlayCountN1;
					if ( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].譜面情報.レベル[ (int) part ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public void t曲リストのソート5_BestRank順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return order * num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					bool isFullCombo1 = false, isFullCombo2 = false;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						isFullCombo1 = n1.arスコア[ nL12345 ].譜面情報.フルコンボ[ (int) part ];
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						isFullCombo2 = n2.arスコア[ nL12345 ].譜面情報.フルコンボ[ (int) part ];
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
					if ( isFullCombo1 ^ isFullCombo2 )
					{
						if ( isFullCombo1 ) return order; else return -order;
					}
					num = nSumPlayCountN2 - nSumPlayCountN1;
					if ( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public void t曲リストのソート6_SkillPoint順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return order * num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					double nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
					double d = nSumPlayCountN2 - nSumPlayCountN1;
					if ( d != 0 )
					{
						return order * System.Math.Sign(d);
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					double nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public void t曲リストのソート7_更新日時順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return order * num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					DateTime nSumPlayCountN1 = DateTime.MinValue;
					DateTime nSumPlayCountN2 = DateTime.MinValue;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
					int d = nSumPlayCountN1.CompareTo(nSumPlayCountN2);
					if ( d != 0 )
					{
						return order * System.Math.Sign( d );
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					DateTime nSumPlayCountN1 = DateTime.MinValue;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public void t曲リストのソート8_アーティスト名順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			int nL12345 = (int) p[ 0 ]; 
			ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
			{
				if ( n1 == n2 )
				{
					return 0;
				}
				int num = this.t比較0_共通( n1, n2 );
				if ( num != 0 )
				{
					return order * System.Math.Sign( num );
				}
				string strAuthorN1 = "";
				string strAuthorN2 = "";
				if (n1.arスコア[ nL12345 ] != null ) {
					strAuthorN1 = n1.arスコア[ nL12345 ].譜面情報.アーティスト名;
				}
				if ( n2.arスコア[ nL12345 ] != null )
				{
					strAuthorN2 = n2.arスコア[ nL12345 ].譜面情報.アーティスト名;
				}

				return order * strAuthorN1.CompareTo( strAuthorN2 );
			} );
			foreach ( C曲リストノード c曲リストノード in ノードリスト )
			{
				string s = "";
				if ( c曲リストノード.arスコア[ nL12345 ] != null )
				{
					s = c曲リストノード.arスコア[ nL12345 ].譜面情報.アーティスト名;
				}
Debug.WriteLine( s + ":" + c曲リストノード.strタイトル );
			}
		}
#if TEST_SORTBGM
		public void t曲リストのソート9_BPM順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return order * num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					double dBPMn1 = 0.0, dBPMn2 = 0.0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						dBPMn1 = n1.arスコア[ nL12345 ].譜面情報.bpm;
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						dBPMn2 = n2.arスコア[ nL12345 ].譜面情報.bpm;
					}
					double d = dBPMn1- dBPMn2;
					if ( d != 0 )
					{
						return order * System.Math.Sign( d );
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					double dBPM = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						dBPM = c曲リストノード.arスコア[ nL12345 ].譜面情報.bpm;
					}
Debug.WriteLine( dBPM + ":" + c曲リストノード.strタイトル );
				}
			}
		}
#endif
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private const string SONGSDB_VERSION = "SongsDB3";		// #23624 2011.5.23 yyagi: BPMとゲーム型スキル追加に伴い、バージョンをインクリメント

		private int t比較0_共通( C曲リストノード n1, C曲リストノード n2 )
		{
			if( n1.eノード種別 == C曲リストノード.Eノード種別.BACKBOX )
			{
				return -1;
			}
			if( n2.eノード種別 == C曲リストノード.Eノード種別.BACKBOX )
			{
				return 1;
			}
			if( n1.eノード種別 == C曲リストノード.Eノード種別.RANDOM )
			{
				return 1;
			}
			if( n2.eノード種別 == C曲リストノード.Eノード種別.RANDOM )
			{
				return -1;
			}
			if( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 != C曲リストノード.Eノード種別.BOX ) )
			{
				return -1;
			}
			if( ( n1.eノード種別 != C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
			{
				return 1;
			}
			return 0;
		}
		//-----------------
		#endregion
	}
}
