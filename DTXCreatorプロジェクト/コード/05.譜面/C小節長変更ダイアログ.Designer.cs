namespace DTXCreator.譜面
{
	partial class C小節長変更ダイアログ
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
			this.numericUpDown小節長の倍率 = new System.Windows.Forms.NumericUpDown();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.textBox小節番号 = new System.Windows.Forms.TextBox();
			this.checkBox後続設定 = new System.Windows.Forms.CheckBox();
			this.label小節長倍率 = new System.Windows.Forms.Label();
			this.label小節番号 = new System.Windows.Forms.Label();
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown小節長の倍率 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDown小節長の倍率
			// 
			this.numericUpDown小節長の倍率.DecimalPlaces = 3;
			this.numericUpDown小節長の倍率.Font = new System.Drawing.Font( "MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 128 ) ) );
			this.numericUpDown小節長の倍率.Increment = new decimal( new int[] {
            5,
            0,
            0,
            131072} );
			this.numericUpDown小節長の倍率.Location = new System.Drawing.Point( 109, 35 );
			this.numericUpDown小節長の倍率.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            196608} );
			this.numericUpDown小節長の倍率.Name = "numericUpDown小節長の倍率";
			this.numericUpDown小節長の倍率.Size = new System.Drawing.Size( 85, 26 );
			this.numericUpDown小節長の倍率.TabIndex = 0;
			this.numericUpDown小節長の倍率.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.numericUpDown小節長の倍率.KeyDown += new System.Windows.Forms.KeyEventHandler( this.numericUpDown小節長の倍率_KeyDown );
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 172, 97 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 253, 97 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 3;
			this.buttonキャンセル.Text = "キャンセル";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// textBox小節番号
			// 
			this.textBox小節番号.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox小節番号.Location = new System.Drawing.Point( 86, 14 );
			this.textBox小節番号.Name = "textBox小節番号";
			this.textBox小節番号.ReadOnly = true;
			this.textBox小節番号.Size = new System.Drawing.Size( 53, 12 );
			this.textBox小節番号.TabIndex = 5;
			// 
			// checkBox後続設定
			// 
			this.checkBox後続設定.AutoSize = true;
			this.checkBox後続設定.Location = new System.Drawing.Point( 109, 67 );
			this.checkBox後続設定.Name = "checkBox後続設定";
			this.checkBox後続設定.Size = new System.Drawing.Size( 212, 16 );
			this.checkBox後続設定.TabIndex = 1;
			this.checkBox後続設定.Text = "後続の小節の倍率もすべて変更する(&F)";
			this.checkBox後続設定.UseVisualStyleBackColor = true;
			this.checkBox後続設定.KeyDown += new System.Windows.Forms.KeyEventHandler( this.checkBox後続設定_KeyDown );
			// 
			// label小節長倍率
			// 
			this.label小節長倍率.AutoSize = true;
			this.label小節長倍率.Location = new System.Drawing.Point( 13, 43 );
			this.label小節長倍率.Name = "label小節長倍率";
			this.label小節長倍率.Size = new System.Drawing.Size( 91, 12 );
			this.label小節長倍率.TabIndex = 6;
			this.label小節長倍率.Text = "小節長の倍率(&R)\r\n";
			// 
			// label小節番号
			// 
			this.label小節番号.AutoSize = true;
			this.label小節番号.Location = new System.Drawing.Point( 13, 14 );
			this.label小節番号.Name = "label小節番号";
			this.label小節番号.Size = new System.Drawing.Size( 53, 12 );
			this.label小節番号.TabIndex = 4;
			this.label小節番号.Text = "小節番号";
			// 
			// C小節長変更ダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 340, 132 );
			this.ControlBox = false;
			this.Controls.Add( this.label小節番号 );
			this.Controls.Add( this.label小節長倍率 );
			this.Controls.Add( this.checkBox後続設定 );
			this.Controls.Add( this.textBox小節番号 );
			this.Controls.Add( this.buttonキャンセル );
			this.Controls.Add( this.buttonOK );
			this.Controls.Add( this.numericUpDown小節長の倍率 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "C小節長変更ダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "小節長変更";
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown小節長の倍率 ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDown小節長の倍率;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonキャンセル;
		private System.Windows.Forms.TextBox textBox小節番号;
		private System.Windows.Forms.CheckBox checkBox後続設定;
		private System.Windows.Forms.Label label小節長倍率;
		private System.Windows.Forms.Label label小節番号;
	}
}