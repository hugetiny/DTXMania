namespace DTX2WAV
{
	partial class Form_Converting
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
			this.button_CancelConverting = new System.Windows.Forms.Button();
			this.label_Converting = new System.Windows.Forms.Label();
			this.progressBar_Converting = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// button_CancelConverting
			// 
			this.button_CancelConverting.Location = new System.Drawing.Point(66, 183);
			this.button_CancelConverting.Name = "button_CancelConverting";
			this.button_CancelConverting.Size = new System.Drawing.Size(144, 23);
			this.button_CancelConverting.TabIndex = 0;
			this.button_CancelConverting.Text = "Cancel(まだ使えません)";
			this.button_CancelConverting.UseVisualStyleBackColor = true;
			// 
			// label_Converting
			// 
			this.label_Converting.AutoSize = true;
			this.label_Converting.Location = new System.Drawing.Point(12, 86);
			this.label_Converting.Name = "label_Converting";
			this.label_Converting.Size = new System.Drawing.Size(270, 12);
			this.label_Converting.TabIndex = 1;
			this.label_Converting.Text = "変換中です。演奏が終わるまで、そのままお待ちください。";
			// 
			// progressBar_Converting
			// 
			this.progressBar_Converting.Location = new System.Drawing.Point(36, 142);
			this.progressBar_Converting.Name = "progressBar_Converting";
			this.progressBar_Converting.Size = new System.Drawing.Size(209, 23);
			this.progressBar_Converting.TabIndex = 2;
			// 
			// Form_Converting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.ControlBox = false;
			this.Controls.Add(this.progressBar_Converting);
			this.Controls.Add(this.label_Converting);
			this.Controls.Add(this.button_CancelConverting);
			this.Name = "Form_Converting";
			this.Text = "Converting...";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_CancelConverting;
		private System.Windows.Forms.Label label_Converting;
		private System.Windows.Forms.ProgressBar progressBar_Converting;
	}
}