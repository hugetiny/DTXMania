
namespace DTXCreator.コード._06.CDialog
{
	partial class CDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CDialog));
			this.button_ok = new System.Windows.Forms.Button();
			this.label_Dialog = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_ok
			// 
			resources.ApplyResources(this.button_ok, "button_ok");
			this.button_ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_ok.Name = "button_ok";
			this.button_ok.UseVisualStyleBackColor = true;
			this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
			// 
			// label_Dialog
			// 
			resources.ApplyResources(this.label_Dialog, "label_Dialog");
			this.label_Dialog.AutoEllipsis = true;
			this.label_Dialog.Name = "label_Dialog";
			// 
			// CDialog
			// 
			this.AcceptButton = this.button_ok;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_ok;
			this.Controls.Add(this.label_Dialog);
			this.Controls.Add(this.button_ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CDialog";
			this.TopMost = true;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_ok;
		private System.Windows.Forms.Label label_Dialog;
	}
}