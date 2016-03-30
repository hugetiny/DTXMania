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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.numericUpDownCh = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.MIDI_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DTX_Lane = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.BackCH = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
			// dataGridView1
			// 
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MIDI_Key,
            this.Key,
            this.DTX_Lane,
            this.BackCH,
            this.Notes,
            this.Comment});
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
			this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.dataGridView1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Cursor = System.Windows.Forms.Cursors.Default;
			this.label3.Name = "label3";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
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
			this.groupBox3.Controls.Add(this.numericUpDown1);
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// numericUpDown1
			// 
			resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
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
			this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// checkBox2
			// 
			resources.ApplyResources(this.checkBox2, "checkBox2");
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// numericUpDownCh
			// 
			resources.ApplyResources(this.numericUpDownCh, "numericUpDownCh");
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
			this.numericUpDownCh.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownCh.ValueChanged += new System.EventHandler(this.numericUpDownCh_ValueChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
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
			// CMIDIインポートダイアログ
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDownCh;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DataGridViewTextBoxColumn MIDI_Key;
		private System.Windows.Forms.DataGridViewTextBoxColumn Key;
		private System.Windows.Forms.DataGridViewComboBoxColumn DTX_Lane;
		private System.Windows.Forms.DataGridViewCheckBoxColumn BackCH;
		private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
		private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}