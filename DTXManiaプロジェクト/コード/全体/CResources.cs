using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Diagnostics;


namespace DTXMania
{
	public class CResources
	{
		private string csvFileName = "resources.csv";

		private string[] csvHeader = null;
		private Dictionary<string, string> dict = new Dictionary<string, string>();

		private string[] langcodelist = null, langlist = null;

		public string strLanguageCode;


		/// <summary>
		/// 表示に使用する言語情報を取得/設定する
		/// 例: Language("ja-JP") など。
		/// </summary>
		public string Language
		{
			get {
				if ( strLanguageCode == "" || strLanguageCode == null )
				{
					string s = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
					if ( s == "" || !csvHeader.Contains( s+".title" ) )
					{
						strLanguageCode = "default";
					}
					else
					{
						strLanguageCode = s;
					}

				}
Debug.WriteLine( "Get: strLanguageCode: " + strLanguageCode );
				return strLanguageCode;
			}
			set
			{
				if ( value == "" || value == null )
				{
					string s = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
					if ( s == "" || !csvHeader.Contains( s + ".title" ) )
					{
						strLanguageCode = "default";
					}
					else
					{
						strLanguageCode = s;
					}
				}
				else
				{
					if ( !csvHeader.Contains( value + ".title" ) )
					{
						strLanguageCode = "default";
					}
					else
					{
						strLanguageCode = value;
					}
				}
				if ( CDTXMania.Instance.ConfigIni != null )
				{
					CDTXMania.Instance.ConfigIni.strLanguage.Value = strLanguageCode;
Debug.WriteLine( "strLang.Value=" + CDTXMania.Instance.ConfigIni.strLanguage.Value );
				}
Debug.WriteLine( "Set: strLanguageCode: " + strLanguageCode );
			}
		}

		/// <summary>
		/// 使用可能な
		/// </summary>
		public string[] LanguageList
		{
			get
			{
				return langlist;
			}
		}
		public string[] LanguageCodeList
		{
			get
			{
				return langcodelist;
			}
		}
		public int LanguageCodeIndex
		{
			get
			{
				int index = Array.IndexOf( langcodelist, strLanguageCode );
				if ( index < 0 ) index = 0;
				return index;
			}
		}
		
		public string Label( string key )
		{
			return Resource( key, "title", strLanguageCode );
		}
		public string Label( string key, string strLang )
		{
			return Resource( key, "title", strLang );
		}
		public string Explanation( string key )
		{
			return Resource( key, "value", strLanguageCode );
		}
		public string Items( string key )
		{
			return Resource( key, "items", strLanguageCode );
		}

		public string Resource(string key, string strType)
		{
			return Resource( key, strType, strLanguageCode );
		}

		public string Resource( string key, string strType, string strLangCode )
		{
			if (strType != "" && strType != "title" && strType != "value" && strType != "items")
			{
				throw new ArgumentOutOfRangeException( "CResources.Resource: 引数が正しくありません。(" + strType + ")" );
			}
			string key_ = key + "." + strLangCode + "." + strType;
			string value = "";

//Debug.WriteLine( "strLangCode=" + strLangCode );
//Debug.WriteLine( "key_=" + key_ );
			if ( !dict.ContainsKey( key_ ) )				// keyかvalueが存在しない場合
			{
				value = "";
			}
			else
			{
				value = dict[ key_ ];
//Debug.WriteLine( "value =" + value );

				if (value == "")	// もし未定義なら、defaultの文字列にfallbackする
				{
					if ( strLangCode == "default" )
					{
						value = "";
					}
					else
					{
						return Resource( key, strType, "default" );
					}
				}
			}
			return value;
		}

		public CResources()
		{
//            this.csvPath = excelPath;
        }
 
        // language="ja-JP"とか。
        public void LoadResources(string language = "")
        {
			// 参考: http://dobon.net/vb/dotnet/file/readcsvfile.html

			Microsoft.VisualBasic.FileIO.TextFieldParser tfp =
				new Microsoft.VisualBasic.FileIO.TextFieldParser(
					csvFileName,
					System.Text.Encoding.Unicode
			);
			//フィールドが文字で区切られているとする
			//デフォルトでDelimitedなので、必要なし
			tfp.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
			//区切り文字を,とする
			tfp.Delimiters = new string[] { "," };
			//フィールドを"で囲み、改行文字、区切り文字を含めることができるか
			//デフォルトでtrueなので、必要なし
			tfp.HasFieldsEnclosedInQuotes = true;
			//フィールドの前後からスペースを削除する
			//デフォルトでtrueなので、必要なし
			tfp.TrimWhiteSpace = true;

			bool bAlreadyReadFirstLine = false;
			while ( !tfp.EndOfData )
			{
				string[] fields = tfp.ReadFields();

				if ( !bAlreadyReadFirstLine )
				{
					//csvHeader.Add( fields );
					csvHeader = fields;
					bAlreadyReadFirstLine = true;
				}
				else
				{
					string strItemName = fields[ 0 ];
					if (strItemName == "" || strItemName.Contains("/") )
					{
						continue;
					}
					else
					{
						for ( int i = 0; i < fields.GetLength( 0 ); i++ )
						{
							string key = strItemName + "." + csvHeader[ i ];
							string value = fields[ i ];

							value = value.Replace( "\r", "" ).Replace( "\n", "" );		// 文字コードとしての改行は削除して、
							value = value.Replace( "\\n", Environment.NewLine );		// "\n" と書かれたところを改行文字に置換する
							dict[ key ] = value;
						}
					}

				}
			}

			//後始末
			tfp.Close();

			//foreach ( string key in dict.Keys )
			//{
			//	Console.WriteLine( "{0} : {1}", key, dict[ key ] );
			//}


			#region [ langcodelist, langlist 生成 ]
			List<string> lstLangCodeList = new List<string>();
			List<string> lstLangList = new List<string>();

			for ( int i = 1; i < csvHeader.Length; i++ )		// 0から開始、ではない (0は名称定義)
			{
				string s = csvHeader[ i ].Replace( ".title", "" ).Replace( ".value", "" ).Replace( ".items", "" );
				if ( !lstLangCodeList.Contains( s ) )
				{
					lstLangCodeList.Add( s );
					lstLangList.Add( Label("strCfgLanguageName", s ) );
				}
			}
			langcodelist = lstLangCodeList.ToArray();
			langlist = lstLangList.ToArray();
			#endregion


			Language = language;
        }

	
	
		#region [ Dispose-Finallizeパターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool bManagedDispose )
		{
			dict = null;
			csvHeader = null;
			langcodelist = null;
			langlist = null;
		}
		~CResources()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion
	}
}
