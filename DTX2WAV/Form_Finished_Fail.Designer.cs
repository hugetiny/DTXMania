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
			this.label_Canceled = new System.Windows.Forms.Label();
			this.button_OK = new System.Windows.Forms.Button();
			this.pictureBox_FailIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_FailIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// label_Canceled
			// 
			this.label_Canceled.AutoSize = true;
			this.label_Canceled.Location = new System.Drawing.Point(57, 23);
			this.label_Canceled.Name = "label_Canceled";
			this.label_Canceled.Size = new System.Drawing.Size(117, 12);
			this.label_Canceled.TabIndex = 0;
			this.label_Canceled.Text = "録音が中止されました。";
			// 
			// button_OK
			// 
			this.button_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button_OK.Location = new System.Drawing.Point(59, 56);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// pictureBox_FailIcon
			// 
			this.pictureBox_FailIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox_FailIcon.Location = new System.Drawing.Point(12, 12);
			this.pictureBox_FailIcon.Name = "pictureBox_FailIcon";
			this.pictureBox_FailIcon.Size = new System.Drawing.Size(40, 46);
			this.pictureBox_FailIcon.TabIndex = 3;
			this.pictureBox_FailIcon.TabStop = false;
			// 
			// Form_Finished_Fail
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(192, 91);
			this.ControlBox = false;
			this.Controls.Add(this.pictureBox_FailIcon);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_Canceled);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Form_Finished_Fail";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "録音中断";
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