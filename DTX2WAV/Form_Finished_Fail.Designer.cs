namespace DTX2WAV
{
	partial class Form_Finished_Fail
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Finished_Fail));
			this.label_Canceled = new System.Windows.Forms.Label();
			this.button_OK = new System.Windows.Forms.Button();
			this.pictureBox_FailIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_FailIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// label_Canceled
			// 
			resources.ApplyResources(this.label_Canceled, "label_Canceled");
			this.label_Canceled.Name = "label_Canceled";
			// 
			// button_OK
			// 
			resources.ApplyResources(this.button_OK, "button_OK");
			this.button_OK.Name = "button_OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// pictureBox_FailIcon
			// 
			resources.ApplyResources(this.pictureBox_FailIcon, "pictureBox_FailIcon");
			this.pictureBox_FailIcon.Name = "pictureBox_FailIcon";
			this.pictureBox_FailIcon.TabStop = false;
			// 
			// Form_Finished_Fail
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.pictureBox_FailIcon);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_Canceled);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Form_Finished_Fail";
			this.Shown += new System.EventHandler(this.Form_Finished_Fail_Shown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_FailIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_Canceled;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.PictureBox pictureBox_FailIcon;
	}
}