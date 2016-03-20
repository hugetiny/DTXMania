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
			this.buttonOpen = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.MIDI_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DTX_Lane = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownCh = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownCh)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOpen
			// 
			this.buttonOpen.Font = new System.Drawing.Font("メイリオ", 8F);
			this.buttonOpen.Location = new System.Drawing.Point(12, 393);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(75, 23);
			this.buttonOpen.TabIndex = 0;
			this.buttonOpen.Text = "&Open";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("メイリオ", 8F);
			this.buttonCancel.Location = new System.Drawing.Point(670, 393);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Font = new System.Drawing.Font("メイリオ", 8F);
			this.buttonOK.Location = new System.Drawing.Point(589, 393);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "O&K";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBox1.Location = new System.Drawing.Point(6, 18);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(189, 191);
			this.textBox1.TabIndex = 3;
			this.textBox1.WordWrap = false;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MIDI_Key,
            this.Key,
            this.DTX_Lane,
            this.Notes,
            this.Comment});
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dataGridView1.Location = new System.Drawing.Point(6, 18);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridView1.Size = new System.Drawing.Size(512, 314);
			this.dataGridView1.TabIndex = 4;
			this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
			this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.dataGridView1);
			this.groupBox1.Font = new System.Drawing.Font("メイリオ", 8F);
			this.groupBox1.Location = new System.Drawing.Point(13, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(524, 375);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "MIDI Assign";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Cursor = System.Windows.Forms.Cursors.Default;
			this.label3.Font = new System.Drawing.Font("メイリオ", 8F);
			this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label3.Location = new System.Drawing.Point(398, 352);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "重複チップ : 0";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("メイリオ", 8F);
			this.label1.Location = new System.Drawing.Point(6, 335);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(385, 34);
			this.label1.TabIndex = 5;
			this.label1.Text = "MIDIのノートの各キーが、DTXでどのレーンに割り当てられるか設定します\r\n* Disuse *を指定すると割り当てません";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBox1);
			this.groupBox2.Font = new System.Drawing.Font("メイリオ", 8F);
			this.groupBox2.Location = new System.Drawing.Point(543, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(202, 215);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "MIDI Analyzed";
			// 
			// MIDI_Key
			// 
			this.MIDI_Key.HeaderText = "MIDI_Key";
			this.MIDI_Key.MaxInputLength = 4;
			this.MIDI_Key.Name = "MIDI_Key";
			this.MIDI_Key.ReadOnly = true;
			this.MIDI_Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.MIDI_Key.Width = 60;
			// 
			// Key
			// 
			this.Key.HeaderText = "Key";
			this.Key.MaxInputLength = 64;
			this.Key.Name = "Key";
			this.Key.ReadOnly = true;
			this.Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Key.Width = 40;
			// 
			// DTX_Lane
			// 
			this.DTX_Lane.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.DTX_Lane.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DTX_Lane.HeaderText = "DTX_Lane";
			this.DTX_Lane.Name = "DTX_Lane";
			this.DTX_Lane.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DTX_Lane.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// Notes
			// 
			this.Notes.HeaderText = "Notes";
			this.Notes.MaxInputLength = 4;
			this.Notes.Name = "Notes";
			this.Notes.ReadOnly = true;
			this.Notes.Width = 40;
			// 
			// Comment
			// 
			this.Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Comment.HeaderText = "Comment";
			this.Comment.MaxInputLength = 128;
			this.Comment.Name = "Comment";
			this.Comment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.numericUpDown1);
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			this.groupBox3.Font = new System.Drawing.Font("メイリオ", 8F);
			this.groupBox3.Location = new System.Drawing.Point(543, 233);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(202, 154);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Import Setting";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(6, 22);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(191, 38);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "ベロシティカーブを急にする\r\n(小さい音がDTXだと大きい場合)";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(6, 66);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(191, 38);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Text = "ベロシティの最大値を127にする\r\n(デフォルトで100が最大)";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(125, 125);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(71, 23);
			this.numericUpDown1.TabIndex = 2;
			this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 127);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(113, 17);
			this.label4.TabIndex = 3;
			this.label4.Text = "DTX VOLUMEの間隔";
			// 
			// numericUpDownCh
			// 
			this.numericUpDownCh.Font = new System.Drawing.Font("メイリオ", 8F);
			this.numericUpDownCh.Location = new System.Drawing.Point(274, 394);
			this.numericUpDownCh.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numericUpDownCh.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownCh.Name = "numericUpDownCh";
			this.numericUpDownCh.Size = new System.Drawing.Size(42, 23);
			this.numericUpDownCh.TabIndex = 9;
			this.numericUpDownCh.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownCh.ValueChanged += new System.EventHandler(this.numericUpDownCh_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("メイリオ", 8F);
			this.label2.Location = new System.Drawing.Point(93, 396);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(175, 17);
			this.label2.TabIndex = 10;
			this.label2.Text = "読むチャンネル(ドラムは通常10)";
			// 
			// CMIDIインポートダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(757, 428);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numericUpDownCh);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOpen);
			this.Name = "CMIDIインポートダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MIDI Import";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownCh)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGridViewTextBoxColumn MIDI_Key;
		private System.Windows.Forms.DataGridViewTextBoxColumn Key;
		private System.Windows.Forms.DataGridViewComboBoxColumn DTX_Lane;
		private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
		private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDownCh;
		private System.Windows.Forms.Label label2;
    }
}