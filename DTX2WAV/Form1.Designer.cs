namespace DTX2WAV
{
	partial class Main
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBox_AudioFormat = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button_BrowseAudio = new System.Windows.Forms.Button();
			this.textBox_BrowseAudio = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button_browseDTX = new System.Windows.Forms.Button();
			this.textBox_BrowseDTX = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_Convert = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBox_MonitorSound = new System.Windows.Forms.CheckBox();
			this.numericUpDown_Master = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_Master = new System.Windows.Forms.BindingSource(this.components);
			this.label9 = new System.Windows.Forms.Label();
			this.trackBar_Master = new System.Windows.Forms.TrackBar();
			this.numericUpDown_Bass = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_Bass = new System.Windows.Forms.BindingSource(this.components);
			this.label8 = new System.Windows.Forms.Label();
			this.trackBar_Bass = new System.Windows.Forms.TrackBar();
			this.numericUpDown_Guitar = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_Guitar = new System.Windows.Forms.BindingSource(this.components);
			this.label7 = new System.Windows.Forms.Label();
			this.trackBar_Guitar = new System.Windows.Forms.TrackBar();
			this.numericUpDown_Drums = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_Drums = new System.Windows.Forms.BindingSource(this.components);
			this.label6 = new System.Windows.Forms.Label();
			this.trackBar_Drums = new System.Windows.Forms.TrackBar();
			this.numericUpDown_SE = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_SE = new System.Windows.Forms.BindingSource(this.components);
			this.label5 = new System.Windows.Forms.Label();
			this.trackBar_SE = new System.Windows.Forms.TrackBar();
			this.numericUpDown_BGM = new System.Windows.Forms.NumericUpDown();
			this.bindingSource_BGM = new System.Windows.Forms.BindingSource(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.trackBar_BGM = new System.Windows.Forms.TrackBar();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Master)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Master)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Master)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Bass)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Bass)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Bass)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Guitar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Guitar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Guitar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Drums)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Drums)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Drums)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SE)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_SE)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_SE)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BGM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_BGM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_BGM)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
			// 
			// statusStrip1
			// 
			resources.ApplyResources(this.statusStrip1, "statusStrip1");
			this.statusStrip1.Name = "statusStrip1";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBox_AudioFormat);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.button_BrowseAudio);
			this.groupBox1.Controls.Add(this.textBox_BrowseAudio);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.button_browseDTX);
			this.groupBox1.Controls.Add(this.textBox_BrowseDTX);
			this.groupBox1.Controls.Add(this.label1);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// comboBox_AudioFormat
			// 
			this.comboBox_AudioFormat.FormattingEnabled = true;
			this.comboBox_AudioFormat.Items.AddRange(new object[] {
            resources.GetString("comboBox_AudioFormat.Items"),
            resources.GetString("comboBox_AudioFormat.Items1"),
            resources.GetString("comboBox_AudioFormat.Items2")});
			resources.ApplyResources(this.comboBox_AudioFormat, "comboBox_AudioFormat");
			this.comboBox_AudioFormat.Name = "comboBox_AudioFormat";
			this.comboBox_AudioFormat.SelectedIndexChanged += new System.EventHandler(this.comboBox_AudioFormat_SelectedIndexChanged);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// button_BrowseAudio
			// 
			resources.ApplyResources(this.button_BrowseAudio, "button_BrowseAudio");
			this.button_BrowseAudio.Name = "button_BrowseAudio";
			this.button_BrowseAudio.UseVisualStyleBackColor = true;
			this.button_BrowseAudio.Click += new System.EventHandler(this.button_browseWAV_Click);
			// 
			// textBox_BrowseAudio
			// 
			resources.ApplyResources(this.textBox_BrowseAudio, "textBox_BrowseAudio");
			this.textBox_BrowseAudio.Name = "textBox_BrowseAudio";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// button_browseDTX
			// 
			resources.ApplyResources(this.button_browseDTX, "button_browseDTX");
			this.button_browseDTX.Name = "button_browseDTX";
			this.button_browseDTX.UseVisualStyleBackColor = true;
			this.button_browseDTX.Click += new System.EventHandler(this.button_browseDTX_Click);
			// 
			// textBox_BrowseDTX
			// 
			resources.ApplyResources(this.textBox_BrowseDTX, "textBox_BrowseDTX");
			this.textBox_BrowseDTX.Name = "textBox_BrowseDTX";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.button_Cancel);
			this.tabPage1.Controls.Add(this.button_Convert);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// button_Cancel
			// 
			resources.ApplyResources(this.button_Cancel, "button_Cancel");
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_Convert
			// 
			resources.ApplyResources(this.button_Convert, "button_Convert");
			this.button_Convert.Name = "button_Convert";
			this.button_Convert.UseVisualStyleBackColor = true;
			this.button_Convert.Click += new System.EventHandler(this.button_Convert_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBox_MonitorSound);
			this.groupBox2.Controls.Add(this.numericUpDown_Master);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.trackBar_Master);
			this.groupBox2.Controls.Add(this.numericUpDown_Bass);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.trackBar_Bass);
			this.groupBox2.Controls.Add(this.numericUpDown_Guitar);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.trackBar_Guitar);
			this.groupBox2.Controls.Add(this.numericUpDown_Drums);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.trackBar_Drums);
			this.groupBox2.Controls.Add(this.numericUpDown_SE);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.trackBar_SE);
			this.groupBox2.Controls.Add(this.numericUpDown_BGM);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.trackBar_BGM);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// checkBox_MonitorSound
			// 
			resources.ApplyResources(this.checkBox_MonitorSound, "checkBox_MonitorSound");
			this.checkBox_MonitorSound.Checked = true;
			this.checkBox_MonitorSound.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox_MonitorSound.Name = "checkBox_MonitorSound";
			this.checkBox_MonitorSound.UseVisualStyleBackColor = true;
			// 
			// numericUpDown_Master
			// 
			this.numericUpDown_Master.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Master, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_Master, "numericUpDown_Master");
			this.numericUpDown_Master.Name = "numericUpDown_Master";
			this.numericUpDown_Master.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_Master
			// 
			this.bindingSource_Master.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			// 
			// trackBar_Master
			// 
			resources.ApplyResources(this.trackBar_Master, "trackBar_Master");
			this.trackBar_Master.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Master, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_Master.LargeChange = 10;
			this.trackBar_Master.Maximum = 100;
			this.trackBar_Master.Name = "trackBar_Master";
			this.trackBar_Master.TickFrequency = 10;
			this.trackBar_Master.Value = 100;
			// 
			// numericUpDown_Bass
			// 
			this.numericUpDown_Bass.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Bass, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_Bass, "numericUpDown_Bass");
			this.numericUpDown_Bass.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericUpDown_Bass.Name = "numericUpDown_Bass";
			this.numericUpDown_Bass.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_Bass
			// 
			this.bindingSource_Bass.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// trackBar_Bass
			// 
			resources.ApplyResources(this.trackBar_Bass, "trackBar_Bass");
			this.trackBar_Bass.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Bass, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_Bass.LargeChange = 10;
			this.trackBar_Bass.Maximum = 200;
			this.trackBar_Bass.Name = "trackBar_Bass";
			this.trackBar_Bass.TickFrequency = 10;
			this.trackBar_Bass.Value = 100;
			// 
			// numericUpDown_Guitar
			// 
			this.numericUpDown_Guitar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Guitar, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_Guitar, "numericUpDown_Guitar");
			this.numericUpDown_Guitar.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericUpDown_Guitar.Name = "numericUpDown_Guitar";
			this.numericUpDown_Guitar.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_Guitar
			// 
			this.bindingSource_Guitar.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// trackBar_Guitar
			// 
			resources.ApplyResources(this.trackBar_Guitar, "trackBar_Guitar");
			this.trackBar_Guitar.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Guitar, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_Guitar.LargeChange = 10;
			this.trackBar_Guitar.Maximum = 200;
			this.trackBar_Guitar.Name = "trackBar_Guitar";
			this.trackBar_Guitar.TickFrequency = 10;
			this.trackBar_Guitar.Value = 100;
			// 
			// numericUpDown_Drums
			// 
			this.numericUpDown_Drums.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Drums, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_Drums, "numericUpDown_Drums");
			this.numericUpDown_Drums.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericUpDown_Drums.Name = "numericUpDown_Drums";
			this.numericUpDown_Drums.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_Drums
			// 
			this.bindingSource_Drums.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// trackBar_Drums
			// 
			resources.ApplyResources(this.trackBar_Drums, "trackBar_Drums");
			this.trackBar_Drums.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_Drums, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_Drums.LargeChange = 10;
			this.trackBar_Drums.Maximum = 200;
			this.trackBar_Drums.Name = "trackBar_Drums";
			this.trackBar_Drums.TickFrequency = 10;
			this.trackBar_Drums.Value = 100;
			// 
			// numericUpDown_SE
			// 
			this.numericUpDown_SE.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_SE, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_SE, "numericUpDown_SE");
			this.numericUpDown_SE.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericUpDown_SE.Name = "numericUpDown_SE";
			this.numericUpDown_SE.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_SE
			// 
			this.bindingSource_SE.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// trackBar_SE
			// 
			resources.ApplyResources(this.trackBar_SE, "trackBar_SE");
			this.trackBar_SE.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_SE, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_SE.LargeChange = 10;
			this.trackBar_SE.Maximum = 200;
			this.trackBar_SE.Name = "trackBar_SE";
			this.trackBar_SE.TickFrequency = 10;
			this.trackBar_SE.Value = 100;
			// 
			// numericUpDown_BGM
			// 
			this.numericUpDown_BGM.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_BGM, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			resources.ApplyResources(this.numericUpDown_BGM, "numericUpDown_BGM");
			this.numericUpDown_BGM.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericUpDown_BGM.Name = "numericUpDown_BGM";
			this.numericUpDown_BGM.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// bindingSource_BGM
			// 
			this.bindingSource_BGM.DataSource = typeof(DTX2WAV.Main.VolumeSlider);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// trackBar_BGM
			// 
			resources.ApplyResources(this.trackBar_BGM, "trackBar_BGM");
			this.trackBar_BGM.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource_BGM, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackBar_BGM.LargeChange = 10;
			this.trackBar_BGM.Maximum = 200;
			this.trackBar_BGM.Name = "trackBar_BGM";
			this.trackBar_BGM.TickFrequency = 10;
			this.trackBar_BGM.Value = 100;
			// 
			// tabPage2
			// 
			resources.ApplyResources(this.tabPage2, "tabPage2");
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// Main
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Main";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Master)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Master)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Master)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Bass)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Bass)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Bass)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Guitar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Guitar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Guitar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Drums)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_Drums)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Drums)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SE)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_SE)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_SE)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_BGM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource_BGM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_BGM)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboBox_AudioFormat;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_BrowseAudio;
		private System.Windows.Forms.TextBox textBox_BrowseAudio;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button_browseDTX;
		private System.Windows.Forms.TextBox textBox_BrowseDTX;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBox_MonitorSound;
		private System.Windows.Forms.NumericUpDown numericUpDown_Master;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TrackBar trackBar_Master;
		private System.Windows.Forms.NumericUpDown numericUpDown_Bass;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TrackBar trackBar_Bass;
		private System.Windows.Forms.NumericUpDown numericUpDown_Guitar;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TrackBar trackBar_Guitar;
		private System.Windows.Forms.NumericUpDown numericUpDown_Drums;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TrackBar trackBar_Drums;
		private System.Windows.Forms.NumericUpDown numericUpDown_SE;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TrackBar trackBar_SE;
		private System.Windows.Forms.NumericUpDown numericUpDown_BGM;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TrackBar trackBar_BGM;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.BindingSource bindingSource_BGM;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Convert;
		private System.Windows.Forms.BindingSource bindingSource_SE;
		private System.Windows.Forms.BindingSource bindingSource_Drums;
		private System.Windows.Forms.BindingSource bindingSource_Guitar;
		private System.Windows.Forms.BindingSource bindingSource_Bass;
		private System.Windows.Forms.BindingSource bindingSource_Master;
	}
}

