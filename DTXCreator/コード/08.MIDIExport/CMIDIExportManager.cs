using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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

			//cMIDIExportDialog.tMIDIExportMain();

			if (cMIDIExportDialog.ShowDialog() == DialogResult.OK)
            {
				Debug.WriteLine("OK");
            }
        }


		#region [ private ]
        //-----------------
        private Cメインフォーム formメインフォーム;
		//-----------------
		#endregion
	}
}
