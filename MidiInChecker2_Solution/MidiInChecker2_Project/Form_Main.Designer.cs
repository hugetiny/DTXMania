namespace MidiInChecker2
{
	partial class Form_Main
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonOK = new System.Windows.Forms.Button();
			this.RichLogTextBox = new System.Windows.Forms.RichTextBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ignoreRealtimeMessage_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.editToolStripMenuItem,
            this.optionToolStripMenuItem});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem1});
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// exitToolStripMenuItem1
			// 
			this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
			resources.ApplyResources(this.exitToolStripMenuItem1, "exitToolStripMenuItem1");
			this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1,
            this.toolStripSeparator2,
            this.selectAllToolStripMenuItem,
            this.clearAllToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			// 
			// copyToolStripMenuItem1
			// 
			this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
			resources.ApplyResources(this.copyToolStripMenuItem1, "copyToolStripMenuItem1");
			this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
			// 
			// clearAllToolStripMenuItem
			// 
			this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			resources.ApplyResources(this.clearAllToolStripMenuItem, "clearAllToolStripMenuItem");
			this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// RichLogTextBox
			// 
			resources.ApplyResources(this.RichLogTextBox, "RichLogTextBox");
			this.RichLogTextBox.ContextMenuStrip = this.contextMenuStrip1;
			this.RichLogTextBox.HideSelection = false;
			this.RichLogTextBox.Name = "RichLogTextBox";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteVToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteVToolStripMenuItem
			// 
			this.pasteVToolStripMenuItem.Name = "pasteVToolStripMenuItem";
			resources.ApplyResources(this.pasteVToolStripMenuItem, "pasteVToolStripMenuItem");
			this.pasteVToolStripMenuItem.Click += new System.EventHandler(this.pasteVToolStripMenuItem_Click);
			// 
			// saveFileDialog1
			// 
			resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
			this.saveFileDialog1.RestoreDirectory = true;
			this.saveFileDialog1.SupportMultiDottedExtensions = true;
			// 
			// optionToolStripMenuItem
			// 
			this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreRealtimeMessage_ToolStripMenuItem});
			this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
			resources.ApplyResources(this.optionToolStripMenuItem, "optionToolStripMenuItem");
			// 
			// ignoreF8ToolStripMenuItem
			// 
			this.ignoreRealtimeMessage_ToolStripMenuItem.Name = "ignoreF8ToolStripMenuItem";
			resources.ApplyResources(this.ignoreRealtimeMessage_ToolStripMenuItem, "ignoreF8ToolStripMenuItem");
			this.ignoreRealtimeMessage_ToolStripMenuItem.Click += new System.EventHandler(this.ignoreRealtimeMessage_ToolStripMenuItem_Click);
			// 
			// Form_Main
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.RichLogTextBox);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form_Main";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
			this.Shown += new System.EventHandler(this.Form_Main_Shown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.RichTextBox RichLogTextBox;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteVToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ignoreRealtimeMessage_ToolStripMenuItem;
	}
}

