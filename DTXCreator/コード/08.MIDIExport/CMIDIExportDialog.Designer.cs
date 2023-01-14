
namespace DTXCreator.MIDIExport
{
    partial class CMIDIExportDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CMIDIExportDialog));
			this.button_export = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.label_Encoding = new System.Windows.Forms.Label();
			this.label_outputFilename = new System.Windows.Forms.Label();
			this.label_LPAssign = new System.Windows.Forms.Label();
			this.toolTip_CMIDIExportDialog = new System.Windows.Forms.ToolTip(this.components);
			this.comboBox_encodingList = new System.Windows.Forms.ComboBox();
			this.comboBox_LPAssign = new System.Windows.Forms.ComboBox();
			this.label_outputFilename_text = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_export
			// 
			resources.ApplyResources(this.button_export, "button_export");
			this.button_export.Name = "button_export";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.button_export, resources.GetString("button_export.ToolTip"));
			this.button_export.UseVisualStyleBackColor = true;
			this.button_export.Click += new System.EventHandler(this.button_export_Click);
			// 
			// button_cancel
			// 
			resources.ApplyResources(this.button_cancel, "button_cancel");
			this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_cancel.Name = "button_cancel";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.button_cancel, resources.GetString("button_cancel.ToolTip"));
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// label_Encoding
			// 
			resources.ApplyResources(this.label_Encoding, "label_Encoding");
			this.label_Encoding.Name = "label_Encoding";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.label_Encoding, resources.GetString("label_Encoding.ToolTip"));
			// 
			// label_outputFilename
			// 
			resources.ApplyResources(this.label_outputFilename, "label_outputFilename");
			this.label_outputFilename.Name = "label_outputFilename";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.label_outputFilename, resources.GetString("label_outputFilename.ToolTip"));
			// 
			// label_LPAssign
			// 
			resources.ApplyResources(this.label_LPAssign, "label_LPAssign");
			this.label_LPAssign.Name = "label_LPAssign";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.label_LPAssign, resources.GetString("label_LPAssign.ToolTip"));
			// 
			// comboBox_encodingList
			// 
			resources.ApplyResources(this.comboBox_encodingList, "comboBox_encodingList");
			this.comboBox_encodingList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_encodingList.FormattingEnabled = true;
			this.comboBox_encodingList.Name = "comboBox_encodingList";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.comboBox_encodingList, resources.GetString("comboBox_encodingList.ToolTip"));
			// 
			// comboBox_LPAssign
			// 
			resources.ApplyResources(this.comboBox_LPAssign, "comboBox_LPAssign");
			this.comboBox_LPAssign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LPAssign.FormattingEnabled = true;
			this.comboBox_LPAssign.Name = "comboBox_LPAssign";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.comboBox_LPAssign, resources.GetString("comboBox_LPAssign.ToolTip"));
			// 
			// label_outputFilename_text
			// 
			resources.ApplyResources(this.label_outputFilename_text, "label_outputFilename_text");
			this.label_outputFilename_text.Name = "label_outputFilename_text";
			this.toolTip_CMIDIExportDialog.SetToolTip(this.label_outputFilename_text, resources.GetString("label_outputFilename_text.ToolTip"));
			// 
			// CMIDIExportDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_cancel;
			this.Controls.Add(this.label_outputFilename_text);
			this.Controls.Add(this.comboBox_LPAssign);
			this.Controls.Add(this.comboBox_encodingList);
			this.Controls.Add(this.label_LPAssign);
			this.Controls.Add(this.label_outputFilename);
			this.Controls.Add(this.label_Encoding);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_export);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CMIDIExportDialog";
			this.toolTip_CMIDIExportDialog.SetToolTip(this, resources.GetString("$this.ToolTip"));
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Label label_Encoding;
		private System.Windows.Forms.Label label_outputFilename;
		private System.Windows.Forms.Label label_LPAssign;
		private System.Windows.Forms.ToolTip toolTip_CMIDIExportDialog;
		private System.Windows.Forms.ComboBox comboBox_encodingList;
		private System.Windows.Forms.ComboBox comboBox_LPAssign;
		private System.Windows.Forms.Label label_outputFilename_text;
	}
}