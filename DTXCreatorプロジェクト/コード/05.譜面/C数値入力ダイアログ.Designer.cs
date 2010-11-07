namespace DTXCreator.譜面
{
	partial class C数値入力ダイアログ
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
			this.numericUpDown数値 = new System.Windows.Forms.NumericUpDown();
			this.labelメッセージ = new System.Windows.Forms.Label();
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown数値 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDown数値
			// 
			this.numericUpDown数値.DecimalPlaces = 4;
			this.numericUpDown数値.Font = new System.Drawing.Font( "MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 128 ) ) );
			this.numericUpDown数値.Location = new System.Drawing.Point( 16, 45 );
			this.numericUpDown数値.Maximum = new decimal( new int[] {
            1000,
            0,
            0,
            0} );
			this.numericUpDown数値.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            262144} );
			this.numericUpDown数値.Name = "numericUpDown数値";
			this.numericUpDown数値.Size = new System.Drawing.Size( 120, 26 );
			this.numericUpDown数値.TabIndex = 1;
			this.numericUpDown数値.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown数値.Value = new decimal( new int[] {
            1,
            0,
            0,
            262144} );
			this.numericUpDown数値.KeyDown += new System.Windows.Forms.KeyEventHandler( this.numericUpDown数値_KeyDown );
			// 
			// labelメッセージ
			// 
			this.labelメッセージ.AutoSize = true;
			this.labelメッセージ.Location = new System.Drawing.Point( 14, 11 );
			this.labelメッセージ.Name = "labelメッセージ";
			this.labelメッセージ.Size = new System.Drawing.Size( 121, 12 );
			this.labelメッセージ.TabIndex = 0;
			this.labelメッセージ.Text = "数値を入力してください。\r\n";
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 92, 90 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 3;
			this.buttonキャンセル.Text = "キャンセル";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 11, 90 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// C数値入力ダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 179, 125 );
			this.ControlBox = false;
			this.Controls.Add( this.buttonOK );
			this.Controls.Add( this.buttonキャンセル );
			this.Controls.Add( this.labelメッセージ );
			this.Controls.Add( this.numericUpDown数値 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "C数値入力ダイアログ";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "数値入力";
			( (System.ComponentModel.ISupportInitialize) ( this.numericUpDown数値 ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDown数値;
		private System.Windows.Forms.Label labelメッセージ;
		private System.Windows.Forms.Button buttonキャンセル;
		private System.Windows.Forms.Button buttonOK;
	}
}