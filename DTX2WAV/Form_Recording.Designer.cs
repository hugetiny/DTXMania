namespace DTX2WAV
{
	partial class Form_Recording
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Recording));
			this.button_CancelConverting = new System.Windows.Forms.Button();
			this.label_Recording = new System.Windows.Forms.Label();
			this.progressBar_Recording = new System.Windows.Forms.ProgressBar();
			this.label_boot = new System.Windows.Forms.Label();
			this.label_loading = new System.Windows.Forms.Label();
			this.label_playing = new System.Windows.Forms.Label();
			this.label_exit = new System.Windows.Forms.Label();
			this.label_state = new System.Windows.Forms.Label();
			this.label_boot_check = new System.Windows.Forms.Label();
			this.label_loading_check = new System.Windows.Forms.Label();
			this.label_playing_check = new System.Windows.Forms.Label();
			this.label_exit_check = new System.Windows.Forms.Label();
			this.label_estimateTime = new System.Windows.Forms.Label();
			this.label_currentTime = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_CancelConverting
			// 
			resources.ApplyResources(this.button_CancelConverting, "button_CancelConverting");
			this.button_CancelConverting.Name = "button_CancelConverting";
			this.button_CancelConverting.UseVisualStyleBackColor = true;
			this.button_CancelConverting.Click += new System.EventHandler(this.button_CancelConverting_Click);
			// 
			// label_Recording
			// 
			resources.ApplyResources(this.label_Recording, "label_Recording");
			this.label_Recording.Name = "label_Recording";
			// 
			// progressBar_Recording
			// 
			resources.ApplyResources(this.progressBar_Recording, "progressBar_Recording");
			this.progressBar_Recording.Maximum = 10000;
			this.progressBar_Recording.Name = "progressBar_Recording";
			// 
			// label_boot
			// 
			resources.ApplyResources(this.label_boot, "label_boot");
			this.label_boot.Name = "label_boot";
			// 
			// label_loading
			// 
			resources.ApplyResources(this.label_loading, "label_loading");
			this.label_loading.Name = "label_loading";
			// 
			// label_playing
			// 
			resources.ApplyResources(this.label_playing, "label_playing");
			this.label_playing.Name = "label_playing";
			// 
			// label_exit
			// 
			resources.ApplyResources(this.label_exit, "label_exit");
			this.label_exit.Name = "label_exit";
			// 
			// label_state
			// 
			resources.ApplyResources(this.label_state, "label_state");
			this.label_state.Name = "label_state";
			this.label_state.TextChanged += new System.EventHandler(this.label_state_TextChanged);
			// 
			// label_boot_check
			// 
			resources.ApplyResources(this.label_boot_check, "label_boot_check");
			this.label_boot_check.Name = "label_boot_check";
			// 
			// label_loading_check
			// 
			resources.ApplyResources(this.label_loading_check, "label_loading_check");
			this.label_loading_check.Name = "label_loading_check";
			// 
			// label_playing_check
			// 
			resources.ApplyResources(this.label_playing_check, "label_playing_check");
			this.label_playing_check.Name = "label_playing_check";
			// 
			// label_exit_check
			// 
			resources.ApplyResources(this.label_exit_check, "label_exit_check");
			this.label_exit_check.Name = "label_exit_check";
			// 
			// label_estimateTime
			// 
			resources.ApplyResources(this.label_estimateTime, "label_estimateTime");
			this.label_estimateTime.Name = "label_estimateTime";
			this.label_estimateTime.UseMnemonic = false;
			// 
			// label_currentTime
			// 
			this.label_currentTime.ForeColor = System.Drawing.SystemColors.ControlText;
			resources.ApplyResources(this.label_currentTime, "label_currentTime");
			this.label_currentTime.Name = "label_currentTime";
			// 
			// Form_Recording
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.label_currentTime);
			this.Controls.Add(this.label_estimateTime);
			this.Controls.Add(this.label_exit_check);
			this.Controls.Add(this.label_playing_check);
			this.Controls.Add(this.label_loading_check);
			this.Controls.Add(this.label_boot_check);
			this.Controls.Add(this.label_state);
			this.Controls.Add(this.label_exit);
			this.Controls.Add(this.label_playing);
			this.Controls.Add(this.label_loading);
			this.Controls.Add(this.label_boot);
			this.Controls.Add(this.progressBar_Recording);
			this.Controls.Add(this.label_Recording);
			this.Controls.Add(this.button_CancelConverting);
			this.Name = "Form_Recording";
			this.Load += new System.EventHandler(this.Form_Recording_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_CancelConverting;
		private System.Windows.Forms.Label label_Recording;
		private System.Windows.Forms.ProgressBar progressBar_Recording;
		private System.Windows.Forms.Label label_boot;
		private System.Windows.Forms.Label label_loading;
		private System.Windows.Forms.Label label_playing;
		private System.Windows.Forms.Label label_exit;
		public System.Windows.Forms.Label label_state;
		private System.Windows.Forms.Label label_boot_check;
		private System.Windows.Forms.Label label_loading_check;
		private System.Windows.Forms.Label label_playing_check;
		private System.Windows.Forms.Label label_exit_check;
		private System.Windows.Forms.Label label_estimateTime;
		private System.Windows.Forms.Label label_currentTime;
	}
}