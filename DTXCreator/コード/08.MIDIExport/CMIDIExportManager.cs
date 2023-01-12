using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace DTXCreator.MIDIExport
{
    internal class CMIDIExportManager
	{
		public CMIDIExportManager( Cメインフォーム pメインフォーム )
		{
			this.formメインフォーム = pメインフォーム;
		}
		
		public void tOpenMIDIExportManagerFromMenu()
		{
			CMIDIExportDialog cMIDIExportDialog = new CMIDIExportDialog();
            cMIDIExportDialog.formメインフォーム = this.formメインフォーム;

			if (cMIDIExportDialog.ShowDialog() == DialogResult.OK)
            {
				string message = (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ?
					"MIDIファイルへの出力が完了しました。" : "Completed output MIDI file successfully.";

				MessageBox.Show( message, "MIDI file export", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
        }


		#region [ private ]
        //-----------------
        private Cメインフォーム formメインフォーム;
		//-----------------
		#endregion
	}
}
