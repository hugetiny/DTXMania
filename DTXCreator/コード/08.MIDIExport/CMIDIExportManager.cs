using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using DTXCreator.コード._06.CDialog;

namespace DTXCreator.MIDIExport
{
    internal class CMIDIExportManager
	{
		public CMIDIExportManager( Cメインフォーム pMainForm )
		{
			this.formMainForm = pMainForm;
		}
		
		public void tOpenMIDIExportManagerFromMenu()
		{
			#region [ 出力ファイル名の設定 ]
			var filename = Path.Combine(formMainForm.str作業フォルダ名, formMainForm.strDTXファイル名);
			filename = Path.ChangeExtension(filename, "dtx.mid");
			#endregion

			CMIDIExportDialog cMIDIExportDialog = new CMIDIExportDialog();
            cMIDIExportDialog.formMainForm = this.formMainForm;
			cMIDIExportDialog.Initialize(filename);

			if (cMIDIExportDialog.ShowDialog() == DialogResult.OK)
            {
				string title = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja") ?
					"MIDIファイルへのエクスポート" : "Exporting to MIDI file";

				string message = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja") ?
					"MIDIファイルへの出力が完了しました。" : "Completed output MIDI file successfully.";

				CDialog cDialog = new CDialog(title, message);
				cDialog.ShowDialog();
			}
        }

		#region [ private ]
        //-----------------
        private Cメインフォーム formMainForm;
		//-----------------
		#endregion
	}
}
