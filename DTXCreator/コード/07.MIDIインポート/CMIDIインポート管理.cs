using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DTXCreator.MIDIインポート
{
    internal class CMIDIインポート管理
	{
		public CMIDIインポート管理( Cメインフォーム pメインフォーム )
		{
			this.formメインフォーム = pメインフォーム;
		}
		public void tMIDIインポート管理をインポートメニューから開く()
		{
			CMIDIインポートダイアログ cMIDIインポートダイアログ = new CMIDIインポートダイアログ();
            cMIDIインポートダイアログ.formメインフォーム = this.formメインフォーム;
            cMIDIインポートダイアログ.tMIDI割り当て一覧を作成する();
			cMIDIインポートダイアログ.tMIDIチャンネル一覧を作成する();
            cMIDIインポートダイアログ.tMIDIインポート設定をファイルから読み込む();
			cMIDIインポートダイアログ.tMIDIファイルを選択する();
            if (cMIDIインポートダイアログ.ShowDialog() == DialogResult.OK)
            {
				if ( this.formメインフォーム.t未保存なら保存する() == DialogResult.Cancel ) return;
				this.formメインフォーム.b未保存 = false;
				this.formメインフォーム.tシナリオ_新規作成();

				cMIDIインポートダイアログ.tMIDIインポート結果を反映する();
            }
		}
		public void tMIDIインポート管理を開く(string strファイル名 )
		{
			CMIDIインポートダイアログ cMIDIインポートダイアログ = new CMIDIインポートダイアログ();
			cMIDIインポートダイアログ.formメインフォーム = this.formメインフォーム;
			cMIDIインポートダイアログ.tMIDI割り当て一覧を作成する();
			cMIDIインポートダイアログ.tMIDIチャンネル一覧を作成する();
            cMIDIインポートダイアログ.tMIDIインポート設定をファイルから読み込む();
			cMIDIインポートダイアログ.tMIDIファイルを開く( strファイル名 );
			if ( cMIDIインポートダイアログ.ShowDialog() == DialogResult.OK )
			{
				this.formメインフォーム.b未保存 = false;
				this.formメインフォーム.tシナリオ_新規作成();

				cMIDIインポートダイアログ.tMIDIインポート結果を反映する();
			}
		}
		#region [ private ]
        //-----------------
        private Cメインフォーム formメインフォーム;
		//-----------------
		#endregion
	}
}
