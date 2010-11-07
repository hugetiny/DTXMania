namespace DTXCreator.WAV_BMP_AVI
{
	partial class C画像プロパティダイアログ
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
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
			this.textBoxラベル = new System.Windows.Forms.TextBox();
			this.labelラベル = new System.Windows.Forms.Label();
			this.textBoxファイル = new System.Windows.Forms.TextBox();
			this.labelファイル = new System.Windows.Forms.Label();
			this.button参照 = new System.Windows.Forms.Button();
			this.button背景色 = new System.Windows.Forms.Button();
			this.button文字色 = new System.Windows.Forms.Button();
			this.button標準色に戻す = new System.Windows.Forms.Button();
			this.checkBoxBMPTEX = new System.Windows.Forms.CheckBox();
			this.textBoxBMP番号 = new System.Windows.Forms.TextBox();
			this.labelBMP番号 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxラベル
			// 
			this.textBoxラベル.Location = new System.Drawing.Point( 71, 37 );
			this.textBoxラベル.Name = "textBoxラベル";
			this.textBoxラベル.Size = new System.Drawing.Size( 294, 19 );
			this.textBoxラベル.TabIndex = 3;
			this.toolTip1.SetToolTip( this.textBoxラベル, "画像ファイルに名前を自由に設定できます。\r\n" );
			this.textBoxラベル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxラベル_KeyDown );
			// 
			// labelラベル
			// 
			this.labelラベル.AutoSize = true;
			this.labelラベル.Location = new System.Drawing.Point( 11, 40 );
			this.labelラベル.Name = "labelラベル";
			this.labelラベル.Size = new System.Drawing.Size( 33, 12 );
			this.labelラベル.TabIndex = 2;
			this.labelラベル.Text = "ラベル\r\n";
			this.toolTip1.SetToolTip( this.labelラベル, "画像ファイルに名前を自由に設定できます。\r\n" );
			// 
			// textBoxファイル
			// 
			this.textBoxファイル.Location = new System.Drawing.Point( 71, 62 );
			this.textBoxファイル.Name = "textBoxファイル";
			this.textBoxファイル.Size = new System.Drawing.Size( 217, 19 );
			this.textBoxファイル.TabIndex = 5;
			this.toolTip1.SetToolTip( this.textBoxファイル, "画像ファイル名。\r\n" );
			this.textBoxファイル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxファイル_KeyDown );
			// 
			// labelファイル
			// 
			this.labelファイル.AutoSize = true;
			this.labelファイル.Location = new System.Drawing.Point( 11, 65 );
			this.labelファイル.Name = "labelファイル";
			this.labelファイル.Size = new System.Drawing.Size( 51, 12 );
			this.labelファイル.TabIndex = 4;
			this.labelファイル.Text = "ファイル名\r\n";
			this.toolTip1.SetToolTip( this.labelファイル, "画像ファイル名。\r\n" );
			// 
			// button参照
			// 
			this.button参照.Location = new System.Drawing.Point( 292, 58 );
			this.button参照.Name = "button参照";
			this.button参照.Size = new System.Drawing.Size( 71, 23 );
			this.button参照.TabIndex = 6;
			this.button参照.Text = "参照...\r\n";
			this.toolTip1.SetToolTip( this.button参照, "画像ファイルをダイアログから選択します。\r\n" );
			this.button参照.UseVisualStyleBackColor = true;
			this.button参照.Click += new System.EventHandler( this.button参照_Click );
			this.button参照.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button参照_KeyDown );
			// 
			// button背景色
			// 
			this.button背景色.Location = new System.Drawing.Point( 135, 10 );
			this.button背景色.Name = "button背景色";
			this.button背景色.Size = new System.Drawing.Size( 51, 23 );
			this.button背景色.TabIndex = 10;
			this.button背景色.Text = "背景色\r\n";
			this.toolTip1.SetToolTip( this.button背景色, "BMPリスト行の背景色を設定します。\r\n" );
			this.button背景色.UseVisualStyleBackColor = true;
			this.button背景色.Click += new System.EventHandler( this.button背景色_Click );
			this.button背景色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button背景色_KeyDown );
			// 
			// button文字色
			// 
			this.button文字色.Location = new System.Drawing.Point( 192, 10 );
			this.button文字色.Name = "button文字色";
			this.button文字色.Size = new System.Drawing.Size( 51, 23 );
			this.button文字色.TabIndex = 11;
			this.button文字色.Text = "文字色\r\n";
			this.toolTip1.SetToolTip( this.button文字色, "BMPリスト行の文字色を設定します。\r\n" );
			this.button文字色.UseVisualStyleBackColor = true;
			this.button文字色.Click += new System.EventHandler( this.button文字色_Click );
			this.button文字色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button文字色_KeyDown );
			// 
			// button標準色に戻す
			// 
			this.button標準色に戻す.Location = new System.Drawing.Point( 249, 10 );
			this.button標準色に戻す.Name = "button標準色に戻す";
			this.button標準色に戻す.Size = new System.Drawing.Size( 92, 23 );
			this.button標準色に戻す.TabIndex = 12;
			this.button標準色に戻す.Text = "標準色に戻す\r\n";
			this.toolTip1.SetToolTip( this.button標準色に戻す, "ToolTip=BMPリスト行の背景色と文字色を標準色に戻します。\r\n" );
			this.button標準色に戻す.UseVisualStyleBackColor = true;
			this.button標準色に戻す.Click += new System.EventHandler( this.button標準色に戻す_Click );
			this.button標準色に戻す.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button標準色に戻す_KeyDown );
			// 
			// checkBoxBMPTEX
			// 
			this.checkBoxBMPTEX.AutoSize = true;
			this.checkBoxBMPTEX.Location = new System.Drawing.Point( 71, 87 );
			this.checkBoxBMPTEX.Name = "checkBoxBMPTEX";
			this.checkBoxBMPTEX.Size = new System.Drawing.Size( 67, 16 );
			this.checkBoxBMPTEX.TabIndex = 7;
			this.checkBoxBMPTEX.Text = "テクスチャ";
			this.toolTip1.SetToolTip( this.checkBoxBMPTEX, "画像をポリゴンテクスチャとして表示します。\r\n" );
			this.checkBoxBMPTEX.UseVisualStyleBackColor = true;
			this.checkBoxBMPTEX.KeyDown += new System.Windows.Forms.KeyEventHandler( this.checkBoxBMPTEX_KeyDown );
			// 
			// textBoxBMP番号
			// 
			this.textBoxBMP番号.Location = new System.Drawing.Point( 71, 12 );
			this.textBoxBMP番号.Name = "textBoxBMP番号";
			this.textBoxBMP番号.ReadOnly = true;
			this.textBoxBMP番号.Size = new System.Drawing.Size( 58, 19 );
			this.textBoxBMP番号.TabIndex = 1;
			this.textBoxBMP番号.TabStop = false;
			this.textBoxBMP番号.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelBMP番号
			// 
			this.labelBMP番号.AutoSize = true;
			this.labelBMP番号.Location = new System.Drawing.Point( 11, 15 );
			this.labelBMP番号.Name = "labelBMP番号";
			this.labelBMP番号.Size = new System.Drawing.Size( 53, 12 );
			this.labelBMP番号.TabIndex = 0;
			this.labelBMP番号.Text = "BMP番号\r\n";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 209, 99 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 290, 99 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 9;
			this.buttonキャンセル.Text = "キャンセル";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// C画像プロパティダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 378, 136 );
			this.ControlBox = false;
			this.Controls.Add( this.checkBoxBMPTEX );
			this.Controls.Add( this.button標準色に戻す );
			this.Controls.Add( this.button文字色 );
			this.Controls.Add( this.button背景色 );
			this.Controls.Add( this.button参照 );
			this.Controls.Add( this.labelファイル );
			this.Controls.Add( this.textBoxファイル );
			this.Controls.Add( this.labelラベル );
			this.Controls.Add( this.textBoxラベル );
			this.Controls.Add( this.buttonキャンセル );
			this.Controls.Add( this.buttonOK );
			this.Controls.Add( this.labelBMP番号 );
			this.Controls.Add( this.textBoxBMP番号 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "C画像プロパティダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "画像プロパティ";
			this.TopMost = true;
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		public System.Windows.Forms.TextBox textBoxBMP番号;
		private System.Windows.Forms.Label labelBMP番号;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonキャンセル;
		public System.Windows.Forms.TextBox textBoxラベル;
		private System.Windows.Forms.Label labelラベル;
		public System.Windows.Forms.TextBox textBoxファイル;
		private System.Windows.Forms.Label labelファイル;
		private System.Windows.Forms.Button button参照;
		private System.Windows.Forms.Button button背景色;
		private System.Windows.Forms.Button button文字色;
		private System.Windows.Forms.Button button標準色に戻す;
		public System.Windows.Forms.CheckBox checkBoxBMPTEX;
	}
}