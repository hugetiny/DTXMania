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
			this.buttonOpen = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.dgv割り当て一覧 = new System.Windows.Forms.DataGridView();
			this.MIDI_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DTX_Lane = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.BackCH = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label重複チップ数 = new System.Windows.Forms.Label();
			this.label説明文 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownVOLUME間隔 = new System.Windows.Forms.NumericUpDown();
			this.checkBoxベロシティ最大値127 = new System.Windows.Forms.CheckBox();
			this.checkBoxベロシティカーブ調整 = new System.Windows.Forms.CheckBox();
			this.groupbox4 = new System.Windows.Forms.GroupBox();
			this.dgvチャンネル一覧 = new System.Windows.Forms.DataGridView();
			this.Ch = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ChNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ChLoad = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgv割り当て一覧)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownVOLUME間隔)).BeginInit();
			this.groupbox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvチャンネル一覧)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOpen
			// 
			resources.ApplyResources(this.buttonOpen, "buttonOpen");
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
			resources.ApplyResources(this.dgv割り当て一覧, "dgv割り当て一覧");
			this.dgv割り当て一覧.AllowUserToAddRows = false;
			this.dgv割り当て一覧.AllowUserToDeleteRows = false;
			this.dgv割り当て一覧.AllowUserToResizeColumns = false;
			this.dgv割り当て一覧.AllowUserToResizeRows = false;
			this.dgv割り当て一覧.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv割り当て一覧.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MIDI_Key,
            this.Key,
            this.DTX_Lane,
            this.BackCH,
            this.Notes,
            this.Comment});
			this.dgv割り当て一覧.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dgv割り当て一覧.MultiSelect = false;
			this.dgv割り当て一覧.Name = "dgv割り当て一覧";
			this.dgv割り当て一覧.RowHeadersVisible = false;
			this.dgv割り当て一覧.RowTemplate.Height = 21;
			this.dgv割り当て一覧.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dgv割り当て一覧.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv割り当て一覧_CellEndEdit);
			this.dgv割り当て一覧.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv割り当て一覧_CellEnter);
			// 
			// MIDI_Key
			// 
			resources.ApplyResources(this.MIDI_Key, "MIDI_Key");
			this.MIDI_Key.MaxInputLength = 4;
			this.MIDI_Key.Name = "MIDI_Key";
			this.MIDI_Key.ReadOnly = true;
			this.MIDI_Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Key
			// 
			resources.ApplyResources(this.Key, "Key");
			this.Key.MaxInputLength = 64;
			this.Key.Name = "Key";
			this.Key.ReadOnly = true;
			this.Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// DTX_Lane
			// 
			this.DTX_Lane.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.DTX_Lane.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			resources.ApplyResources(this.DTX_Lane, "DTX_Lane");
			this.DTX_Lane.Name = "DTX_Lane";
			this.DTX_Lane.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DTX_Lane.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// BackCH
			// 
			resources.ApplyResources(this.BackCH, "BackCH");
			this.BackCH.Name = "BackCH";
			this.BackCH.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Notes
			// 
			resources.ApplyResources(this.Notes, "Notes");
			this.Notes.MaxInputLength = 4;
			this.Notes.Name = "Notes";
			this.Notes.ReadOnly = true;
			// 
			// Comment
			// 
			this.Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.Comment, "Comment");
			this.Comment.MaxInputLength = 128;
			this.Comment.Name = "Comment";
			this.Comment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.label重複チップ数);
			this.groupBox1.Controls.Add(this.label説明文);
			this.groupBox1.Controls.Add(this.dgv割り当て一覧);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
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
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.textBox1);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.numericUpDownVOLUME間隔);
			this.groupBox3.Controls.Add(this.checkBoxベロシティ最大値127);
			this.groupBox3.Controls.Add(this.checkBoxベロシティカーブ調整);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
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
			resources.ApplyResources(this.groupbox4, "groupbox4");
			this.groupbox4.Controls.Add(this.dgvチャンネル一覧);
			this.groupbox4.Name = "groupbox4";
			this.groupbox4.TabStop = false;
			// 
			// dgvチャンネル一覧
			// 
			resources.ApplyResources(this.dgvチャンネル一覧, "dgvチャンネル一覧");
			this.dgvチャンネル一覧.AllowUserToAddRows = false;
			this.dgvチャンネル一覧.AllowUserToDeleteRows = false;
			this.dgvチャンネル一覧.AllowUserToResizeColumns = false;
			this.dgvチャンネル一覧.AllowUserToResizeRows = false;
			this.dgvチャンネル一覧.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvチャンネル一覧.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Ch,
            this.ChNotes,
            this.ChLoad});
			this.dgvチャンネル一覧.MultiSelect = false;
			this.dgvチャンネル一覧.Name = "dgvチャンネル一覧";
			this.dgvチャンネル一覧.RowHeadersVisible = false;
			this.dgvチャンネル一覧.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvチャンネル一覧.RowTemplate.Height = 21;
			this.dgvチャンネル一覧.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvチャンネル一覧.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvチャンネル一覧_CellValueChanged);
			this.dgvチャンネル一覧.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvチャンネル一覧_CurrentCellDirtyStateChanged);
			// 
			// Ch
			// 
			this.Ch.Frozen = true;
			resources.ApplyResources(this.Ch, "Ch");
			this.Ch.Name = "Ch";
			this.Ch.ReadOnly = true;
			// 
			// ChNotes
			// 
			this.ChNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.ChNotes, "ChNotes");
			this.ChNotes.Name = "ChNotes";
			this.ChNotes.ReadOnly = true;
			// 
			// ChLoad
			// 
			this.ChLoad.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			resources.ApplyResources(this.ChLoad, "ChLoad");
			this.ChLoad.Name = "ChLoad";
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
			this.Controls.Add(this.buttonOpen);
			this.Name = "CMIDIインポートダイアログ";
			((System.ComponentModel.ISupportInitialize)(this.dgv割り当て一覧)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
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

        private System.Windows.Forms.Button buttonOpen;
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
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownVOLUME間隔;
		private System.Windows.Forms.DataGridViewTextBoxColumn MIDI_Key;
		private System.Windows.Forms.DataGridViewTextBoxColumn Key;
		private System.Windows.Forms.DataGridViewComboBoxColumn DTX_Lane;
		private System.Windows.Forms.DataGridViewCheckBoxColumn BackCH;
		private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
		private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
		private System.Windows.Forms.GroupBox groupbox4;
		private System.Windows.Forms.DataGridView dgvチャンネル一覧;
		private System.Windows.Forms.DataGridViewTextBoxColumn Ch;
		private System.Windows.Forms.DataGridViewTextBoxColumn ChNotes;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ChLoad;
    }
}