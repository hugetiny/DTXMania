namespace DTXCreator.MIDIインポート
{
    partial class CMIDIインポートダイアログ
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CMIDIインポートダイアログ));
			this.buttonOpenMIDI = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.dgv割り当て一覧 = new System.Windows.Forms.DataGridView();
			this.Assign_MIDI_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Assign_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Assign_DTX_Lane = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.Assign_BackCh = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Assign_Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Assign_Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelMIDIImportSettingsFile = new System.Windows.Forms.Label();
			this.buttonSaveSettings = new System.Windows.Forms.Button();
			this.buttonOpenSettings = new System.Windows.Forms.Button();
			this.label重複チップ数 = new System.Windows.Forms.Label();
			this.label説明文 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.labelVOLUME間隔 = new System.Windows.Forms.Label();
			this.numericUpDownVOLUME間隔 = new System.Windows.Forms.NumericUpDown();
			this.checkBoxベロシティ最大値127 = new System.Windows.Forms.CheckBox();
			this.checkBoxベロシティカーブ調整 = new System.Windows.Forms.CheckBox();
			this.groupbox4 = new System.Windows.Forms.GroupBox();
			this.dgvチャンネル一覧 = new System.Windows.Forms.DataGridView();
			this.Channel_Ch = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Channel_Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Channel_Load = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgv割り当て一覧)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownVOLUME間隔)).BeginInit();
			this.groupbox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvチャンネル一覧)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOpenMIDI
			// 
			resources.ApplyResources(this.buttonOpenMIDI, "buttonOpenMIDI");
			this.buttonOpenMIDI.Name = "buttonOpenMIDI";
			this.buttonOpenMIDI.UseVisualStyleBackColor = true;
			this.buttonOpenMIDI.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			resources.ApplyResources(this.textBox1, "textBox1");
			this.textBox1.Name = "textBox1";
			// 
			// dgv割り当て一覧
			// 
			this.dgv割り当て一覧.AllowUserToAddRows = false;
			this.dgv割り当て一覧.AllowUserToDeleteRows = false;
			this.dgv割り当て一覧.AllowUserToResizeColumns = false;
			this.dgv割り当て一覧.AllowUserToResizeRows = false;
			this.dgv割り当て一覧.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv割り当て一覧.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Assign_MIDI_Key,
            this.Assign_Key,
            this.Assign_DTX_Lane,
            this.Assign_BackCh,
            this.Assign_Notes,
            this.Assign_Comment});
			this.dgv割り当て一覧.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			resources.ApplyResources(this.dgv割り当て一覧, "dgv割り当て一覧");
			this.dgv割り当て一覧.MultiSelect = false;
			this.dgv割り当て一覧.Name = "dgv割り当て一覧";
			this.dgv割り当て一覧.RowHeadersVisible = false;
			this.dgv割り当て一覧.RowTemplate.Height = 21;
			this.dgv割り当て一覧.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dgv割り当て一覧.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv割り当て一覧_CellEndEdit);
			this.dgv割り当て一覧.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv割り当て一覧_CellEnter);
			// 
			// Assign_MIDI_Key
			// 
			resources.ApplyResources(this.Assign_MIDI_Key, "Assign_MIDI_Key");
			this.Assign_MIDI_Key.MaxInputLength = 4;
			this.Assign_MIDI_Key.Name = "Assign_MIDI_Key";
			this.Assign_MIDI_Key.ReadOnly = true;
			this.Assign_MIDI_Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Assign_MIDI_Key.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Assign_Key
			// 
			resources.ApplyResources(this.Assign_Key, "Assign_Key");
			this.Assign_Key.MaxInputLength = 64;
			this.Assign_Key.Name = "Assign_Key";
			this.Assign_Key.ReadOnly = true;
			this.Assign_Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Assign_Key.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Assign_DTX_Lane
			// 
			this.Assign_DTX_Lane.AutoComplete = false;
			this.Assign_DTX_Lane.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.Assign_DTX_Lane.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			resources.ApplyResources(this.Assign_DTX_Lane, "Assign_DTX_Lane");
			this.Assign_DTX_Lane.MaxDropDownItems = 4;
			this.Assign_DTX_Lane.Name = "Assign_DTX_Lane";
			this.Assign_DTX_Lane.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Assign_BackCh
			// 
			resources.ApplyResources(this.Assign_BackCh, "Assign_BackCh");
			this.Assign_BackCh.Name = "Assign_BackCh";
			this.Assign_BackCh.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Assign_Notes
			// 
			resources.ApplyResources(this.Assign_Notes, "Assign_Notes");
			this.Assign_Notes.MaxInputLength = 4;
			this.Assign_Notes.Name = "Assign_Notes";
			this.Assign_Notes.ReadOnly = true;
			this.Assign_Notes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Assign_Comment
			// 
			this.Assign_Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.Assign_Comment, "Assign_Comment");
			this.Assign_Comment.MaxInputLength = 128;
			this.Assign_Comment.Name = "Assign_Comment";
			this.Assign_Comment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Assign_Comment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelMIDIImportSettingsFile);
			this.groupBox1.Controls.Add(this.buttonSaveSettings);
			this.groupBox1.Controls.Add(this.buttonOpenSettings);
			this.groupBox1.Controls.Add(this.label重複チップ数);
			this.groupBox1.Controls.Add(this.label説明文);
			this.groupBox1.Controls.Add(this.dgv割り当て一覧);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// labelMIDIImportSettingsFile
			// 
			resources.ApplyResources(this.labelMIDIImportSettingsFile, "labelMIDIImportSettingsFile");
			this.labelMIDIImportSettingsFile.Name = "labelMIDIImportSettingsFile";
			// 
			// buttonSaveSettings
			// 
			resources.ApplyResources(this.buttonSaveSettings, "buttonSaveSettings");
			this.buttonSaveSettings.Name = "buttonSaveSettings";
			this.buttonSaveSettings.UseVisualStyleBackColor = true;
			this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
			// 
			// buttonOpenSettings
			// 
			resources.ApplyResources(this.buttonOpenSettings, "buttonOpenSettings");
			this.buttonOpenSettings.Name = "buttonOpenSettings";
			this.buttonOpenSettings.UseVisualStyleBackColor = true;
			this.buttonOpenSettings.Click += new System.EventHandler(this.buttonOpenSettings_Click);
			// 
			// label重複チップ数
			// 
			resources.ApplyResources(this.label重複チップ数, "label重複チップ数");
			this.label重複チップ数.Cursor = System.Windows.Forms.Cursors.Default;
			this.label重複チップ数.Name = "label重複チップ数";
			// 
			// label説明文
			// 
			resources.ApplyResources(this.label説明文, "label説明文");
			this.label説明文.Name = "label説明文";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBox1);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.labelVOLUME間隔);
			this.groupBox3.Controls.Add(this.numericUpDownVOLUME間隔);
			this.groupBox3.Controls.Add(this.checkBoxベロシティ最大値127);
			this.groupBox3.Controls.Add(this.checkBoxベロシティカーブ調整);
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// labelVOLUME間隔
			// 
			resources.ApplyResources(this.labelVOLUME間隔, "labelVOLUME間隔");
			this.labelVOLUME間隔.Name = "labelVOLUME間隔";
			// 
			// numericUpDownVOLUME間隔
			// 
			resources.ApplyResources(this.numericUpDownVOLUME間隔, "numericUpDownVOLUME間隔");
			this.numericUpDownVOLUME間隔.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDownVOLUME間隔.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownVOLUME間隔.Name = "numericUpDownVOLUME間隔";
			this.numericUpDownVOLUME間隔.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// checkBoxベロシティ最大値127
			// 
			resources.ApplyResources(this.checkBoxベロシティ最大値127, "checkBoxベロシティ最大値127");
			this.checkBoxベロシティ最大値127.Name = "checkBoxベロシティ最大値127";
			this.checkBoxベロシティ最大値127.UseVisualStyleBackColor = true;
			// 
			// checkBoxベロシティカーブ調整
			// 
			resources.ApplyResources(this.checkBoxベロシティカーブ調整, "checkBoxベロシティカーブ調整");
			this.checkBoxベロシティカーブ調整.Name = "checkBoxベロシティカーブ調整";
			this.checkBoxベロシティカーブ調整.UseVisualStyleBackColor = true;
			// 
			// groupbox4
			// 
			this.groupbox4.Controls.Add(this.dgvチャンネル一覧);
			resources.ApplyResources(this.groupbox4, "groupbox4");
			this.groupbox4.Name = "groupbox4";
			this.groupbox4.TabStop = false;
			// 
			// dgvチャンネル一覧
			// 
			this.dgvチャンネル一覧.AllowUserToAddRows = false;
			this.dgvチャンネル一覧.AllowUserToDeleteRows = false;
			this.dgvチャンネル一覧.AllowUserToResizeColumns = false;
			this.dgvチャンネル一覧.AllowUserToResizeRows = false;
			this.dgvチャンネル一覧.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvチャンネル一覧.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Channel_Ch,
            this.Channel_Notes,
            this.Channel_Load});
			resources.ApplyResources(this.dgvチャンネル一覧, "dgvチャンネル一覧");
			this.dgvチャンネル一覧.MultiSelect = false;
			this.dgvチャンネル一覧.Name = "dgvチャンネル一覧";
			this.dgvチャンネル一覧.RowHeadersVisible = false;
			this.dgvチャンネル一覧.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvチャンネル一覧.RowTemplate.Height = 21;
			this.dgvチャンネル一覧.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvチャンネル一覧.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvチャンネル一覧_CellValueChanged);
			this.dgvチャンネル一覧.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvチャンネル一覧_CurrentCellDirtyStateChanged);
			// 
			// Channel_Ch
			// 
			this.Channel_Ch.Frozen = true;
			resources.ApplyResources(this.Channel_Ch, "Channel_Ch");
			this.Channel_Ch.Name = "Channel_Ch";
			this.Channel_Ch.ReadOnly = true;
			this.Channel_Ch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Channel_Notes
			// 
			this.Channel_Notes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.Channel_Notes, "Channel_Notes");
			this.Channel_Notes.Name = "Channel_Notes";
			this.Channel_Notes.ReadOnly = true;
			this.Channel_Notes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Channel_Load
			// 
			this.Channel_Load.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			resources.ApplyResources(this.Channel_Load, "Channel_Load");
			this.Channel_Load.Name = "Channel_Load";
			// 
			// CMIDIインポートダイアログ
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.groupbox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOpenMIDI);
			this.Name = "CMIDIインポートダイアログ";
			((System.ComponentModel.ISupportInitialize)(this.dgv割り当て一覧)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownVOLUME間隔)).EndInit();
			this.groupbox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvチャンネル一覧)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenMIDI;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgv割り当て一覧;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label説明文;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label重複チップ数;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBoxベロシティカーブ調整;
		private System.Windows.Forms.CheckBox checkBoxベロシティ最大値127;
		private System.Windows.Forms.Label labelVOLUME間隔;
		private System.Windows.Forms.NumericUpDown numericUpDownVOLUME間隔;
		private System.Windows.Forms.GroupBox groupbox4;
		private System.Windows.Forms.DataGridView dgvチャンネル一覧;
		private System.Windows.Forms.Button buttonOpenSettings;
		private System.Windows.Forms.Button buttonSaveSettings;
		private System.Windows.Forms.Label labelMIDIImportSettingsFile;
		private System.Windows.Forms.DataGridViewTextBoxColumn Assign_MIDI_Key;
		private System.Windows.Forms.DataGridViewTextBoxColumn Assign_Key;
		private System.Windows.Forms.DataGridViewComboBoxColumn Assign_DTX_Lane;
		private System.Windows.Forms.DataGridViewCheckBoxColumn Assign_BackCh;
		private System.Windows.Forms.DataGridViewTextBoxColumn Assign_Notes;
		private System.Windows.Forms.DataGridViewTextBoxColumn Assign_Comment;
		private System.Windows.Forms.DataGridViewTextBoxColumn Channel_Ch;
		private System.Windows.Forms.DataGridViewTextBoxColumn Channel_Notes;
		private System.Windows.Forms.DataGridViewCheckBoxColumn Channel_Load;
    }
}