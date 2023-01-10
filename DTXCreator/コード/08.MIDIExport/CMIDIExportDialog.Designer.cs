
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
			this.button_export = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_export
			// 
			this.button_export.Location = new System.Drawing.Point(560, 415);
			this.button_export.Name = "button_export";
			this.button_export.Size = new System.Drawing.Size(75, 23);
			this.button_export.TabIndex = 0;
			this.button_export.Text = "E&xport";
			this.button_export.UseVisualStyleBackColor = true;
			this.button_export.Click += new System.EventHandler(this.button_export_Click);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(665, 415);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(75, 23);
			this.button_cancel.TabIndex = 1;
			this.button_cancel.Text = "&Cancel";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// CMIDIExportDialog
			// 
			this.AcceptButton = this.button_export;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_cancel;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_export);
			this.Name = "CMIDIExportDialog";
			this.Text = "MIDI Export";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.Button button_cancel;
    }
}