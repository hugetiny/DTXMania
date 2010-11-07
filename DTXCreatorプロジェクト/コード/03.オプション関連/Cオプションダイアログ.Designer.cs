namespace DTXCreator.オプション関連
{
	partial class Cオプションダイアログ
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
			if( disposing && ( components != null ) )
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
			this.tabPage全般 = new System.Windows.Forms.TabPage();
			this.checkBoxPlaySoundOnChip = new System.Windows.Forms.CheckBox();
			this.checkBoxPreviewBGM = new System.Windows.Forms.CheckBox();
			this.checkBoxオートフォーカス = new System.Windows.Forms.CheckBox();
			this.label個まで表示する = new System.Windows.Forms.Label();
			this.checkBox最近使用したファイル = new System.Windows.Forms.CheckBox();
			this.numericUpDown最近使用したファイルの最大表示個数 = new System.Windows.Forms.NumericUpDown();
			this.tabControlオプション = new System.Windows.Forms.TabControl();
			this.button1 = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tabPage全般.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown最近使用したファイルの最大表示個数 ) ).BeginInit();
			this.tabControlオプション.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPage全般
			// 
			this.tabPage全般.Controls.Add( this.checkBoxPlaySoundOnChip );
			this.tabPage全般.Controls.Add( this.checkBoxPreviewBGM );
			this.tabPage全般.Controls.Add( this.checkBoxオートフォーカス );
			this.tabPage全般.Controls.Add( this.label個まで表示する );
			this.tabPage全般.Controls.Add( this.checkBox最近使用したファイル );
			this.tabPage全般.Controls.Add( this.numericUpDown最近使用したファイルの最大表示個数 );
			this.tabPage全般.Location = new System.Drawing.Point( 4, 22 );
			this.tabPage全般.Name = "tabPage全般";
			this.tabPage全般.Padding = new System.Windows.Forms.Padding( 3 );
			this.tabPage全般.Size = new System.Drawing.Size( 359, 144 );
			this.tabPage全般.TabIndex = 0;
			this.tabPage全般.Text = "全般\r\n";
			this.tabPage全般.UseVisualStyleBackColor = true;
			// 
			// checkBoxPlaySoundOnChip
			// 
			this.checkBoxPlaySoundOnChip.AutoSize = true;
			this.checkBoxPlaySoundOnChip.Location = new System.Drawing.Point( 6, 73 );
			this.checkBoxPlaySoundOnChip.Name = "checkBoxPlaySoundOnChip";
			this.checkBoxPlaySoundOnChip.Size = new System.Drawing.Size( 298, 16 );
			this.checkBoxPlaySoundOnChip.TabIndex = 5;
			this.checkBoxPlaySoundOnChip.Text = "WAVチップを譜面に配置したときにチップの音を再生する(&S)\r\n";
			this.checkBoxPlaySoundOnChip.UseVisualStyleBackColor = true;
			// 
			// checkBoxPreviewBGM
			// 
			this.checkBoxPreviewBGM.AutoSize = true;
			this.checkBoxPreviewBGM.Location = new System.Drawing.Point( 6, 50 );
			this.checkBoxPreviewBGM.Name = "checkBoxPreviewBGM";
			this.checkBoxPreviewBGM.Size = new System.Drawing.Size( 243, 16 );
			this.checkBoxPreviewBGM.TabIndex = 4;
			this.checkBoxPreviewBGM.Text = "BGMサウンドのプレビューは自動再生しない(&B)\r\n";
			this.checkBoxPreviewBGM.UseVisualStyleBackColor = true;
			// 
			// checkBoxオートフォーカス
			// 
			this.checkBoxオートフォーカス.AutoSize = true;
			this.checkBoxオートフォーカス.Location = new System.Drawing.Point( 6, 6 );
			this.checkBoxオートフォーカス.Name = "checkBoxオートフォーカス";
			this.checkBoxオートフォーカス.Size = new System.Drawing.Size( 110, 16 );
			this.checkBoxオートフォーカス.TabIndex = 0;
			this.checkBoxオートフォーカス.Text = "オートフォーカス(&F)\r\n";
			this.checkBoxオートフォーカス.UseVisualStyleBackColor = true;
			// 
			// label個まで表示する
			// 
			this.label個まで表示する.AutoSize = true;
			this.label個まで表示する.Location = new System.Drawing.Point( 250, 29 );
			this.label個まで表示する.Name = "label個まで表示する";
			this.label個まで表示する.Size = new System.Drawing.Size( 94, 12 );
			this.label個まで表示する.TabIndex = 3;
			this.label個まで表示する.Text = "個まで表示する(&T)\r\n";
			// 
			// checkBox最近使用したファイル
			// 
			this.checkBox最近使用したファイル.AutoSize = true;
			this.checkBox最近使用したファイル.Location = new System.Drawing.Point( 6, 28 );
			this.checkBox最近使用したファイル.Name = "checkBox最近使用したファイル";
			this.checkBox最近使用したファイル.Size = new System.Drawing.Size( 176, 16 );
			this.checkBox最近使用したファイル.TabIndex = 1;
			this.checkBox最近使用したファイル.Text = "最近使用したファイルの一覧(&R):\r\n";
			this.checkBox最近使用したファイル.UseVisualStyleBackColor = true;
			// 
			// numericUpDown最近使用したファイルの最大表示個数
			// 
			this.numericUpDown最近使用したファイルの最大表示個数.Location = new System.Drawing.Point( 188, 25 );
			this.numericUpDown最近使用したファイルの最大表示個数.Maximum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
			this.numericUpDown最近使用したファイルの最大表示個数.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.numericUpDown最近使用したファイルの最大表示個数.Name = "numericUpDown最近使用したファイルの最大表示個数";
			this.numericUpDown最近使用したファイルの最大表示個数.Size = new System.Drawing.Size( 56, 19 );
			this.numericUpDown最近使用したファイルの最大表示個数.TabIndex = 2;
			this.numericUpDown最近使用したファイルの最大表示個数.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			// 
			// tabControlオプション
			// 
			this.tabControlオプション.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.tabControlオプション.Controls.Add( this.tabPage全般 );
			this.tabControlオプション.Location = new System.Drawing.Point( 12, 12 );
			this.tabControlオプション.Name = "tabControlオプション";
			this.tabControlオプション.SelectedIndex = 0;
			this.tabControlオプション.Size = new System.Drawing.Size( 367, 170 );
			this.tabControlオプション.TabIndex = 3;
			// 
			// button1
			// 
			this.button1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point( 300, 192 );
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size( 75, 23 );
			this.button1.TabIndex = 5;
			this.button1.Text = "キャンセル\r\n";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 219, 192 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// Cオプションダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 391, 227 );
			this.ControlBox = false;
			this.Controls.Add( this.buttonOK );
			this.Controls.Add( this.button1 );
			this.Controls.Add( this.tabControlオプション );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Cオプションダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "オプション";
			this.tabPage全般.ResumeLayout( false );
			this.tabPage全般.PerformLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown最近使用したファイルの最大表示個数 ) ).EndInit();
			this.tabControlオプション.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.TabPage tabPage全般;
		internal System.Windows.Forms.CheckBox checkBoxオートフォーカス;
		private System.Windows.Forms.Label label個まで表示する;
		internal System.Windows.Forms.CheckBox checkBox最近使用したファイル;
		internal System.Windows.Forms.NumericUpDown numericUpDown最近使用したファイルの最大表示個数;
		private System.Windows.Forms.TabControl tabControlオプション;
		internal System.Windows.Forms.CheckBox checkBoxPreviewBGM;
		internal System.Windows.Forms.CheckBox checkBoxPlaySoundOnChip;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button buttonOK;

	}
}