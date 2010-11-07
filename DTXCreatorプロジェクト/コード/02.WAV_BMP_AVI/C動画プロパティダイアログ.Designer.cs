namespace DTXCreator.WAV_BMP_AVI
{
	partial class C動画プロパティダイアログ
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
			this.textBoxAVI番号 = new System.Windows.Forms.TextBox();
			this.labelAVI番号 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.textBoxラベル = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
			this.labelラベル = new System.Windows.Forms.Label();
			this.textBoxファイル = new System.Windows.Forms.TextBox();
			this.labelファイル = new System.Windows.Forms.Label();
			this.button参照 = new System.Windows.Forms.Button();
			this.button背景色 = new System.Windows.Forms.Button();
			this.button文字色 = new System.Windows.Forms.Button();
			this.button標準色に戻す = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxAVI番号
			// 
			this.textBoxAVI番号.Location = new System.Drawing.Point( 70, 11 );
			this.textBoxAVI番号.Name = "textBoxAVI番号";
			this.textBoxAVI番号.ReadOnly = true;
			this.textBoxAVI番号.Size = new System.Drawing.Size( 58, 19 );
			this.textBoxAVI番号.TabIndex = 1;
			this.textBoxAVI番号.TabStop = false;
			this.textBoxAVI番号.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelAVI番号
			// 
			this.labelAVI番号.AutoSize = true;
			this.labelAVI番号.Location = new System.Drawing.Point( 10, 14 );
			this.labelAVI番号.Name = "labelAVI番号";
			this.labelAVI番号.Size = new System.Drawing.Size( 48, 12 );
			this.labelAVI番号.TabIndex = 0;
			this.labelAVI番号.Text = "AVI番号\r\n";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 208, 98 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 289, 98 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 8;
			this.buttonキャンセル.Text = "キャンセル\r\n";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// textBoxラベル
			// 
			this.textBoxラベル.Location = new System.Drawing.Point( 70, 36 );
			this.textBoxラベル.Name = "textBoxラベル";
			this.textBoxラベル.Size = new System.Drawing.Size( 294, 19 );
			this.textBoxラベル.TabIndex = 3;
			this.toolTip1.SetToolTip( this.textBoxラベル, "動画ファイルに名前を自由に設定できます。" );
			this.textBoxラベル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxラベル_KeyDown );
			// 
			// labelラベル
			// 
			this.labelラベル.AutoSize = true;
			this.labelラベル.Location = new System.Drawing.Point( 10, 39 );
			this.labelラベル.Name = "labelラベル";
			this.labelラベル.Size = new System.Drawing.Size( 33, 12 );
			this.labelラベル.TabIndex = 2;
			this.labelラベル.Text = "ラベル";
			this.toolTip1.SetToolTip( this.labelラベル, "動画ファイルに名前を自由に設定できます。\r\n" );
			// 
			// textBoxファイル
			// 
			this.textBoxファイル.Location = new System.Drawing.Point( 70, 61 );
			this.textBoxファイル.Name = "textBoxファイル";
			this.textBoxファイル.Size = new System.Drawing.Size( 217, 19 );
			this.textBoxファイル.TabIndex = 5;
			this.toolTip1.SetToolTip( this.textBoxファイル, "画像ファイル名。\r\n" );
			this.textBoxファイル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxファイル_KeyDown );
			// 
			// labelファイル
			// 
			this.labelファイル.AutoSize = true;
			this.labelファイル.Location = new System.Drawing.Point( 10, 64 );
			this.labelファイル.Name = "labelファイル";
			this.labelファイル.Size = new System.Drawing.Size( 51, 12 );
			this.labelファイル.TabIndex = 4;
			this.labelファイル.Text = "ファイル名\r\n";
			this.toolTip1.SetToolTip( this.labelファイル, "動画ファイル名。\r\n" );
			// 
			// button参照
			// 
			this.button参照.Location = new System.Drawing.Point( 293, 57 );
			this.button参照.Name = "button参照";
			this.button参照.Size = new System.Drawing.Size( 71, 23 );
			this.button参照.TabIndex = 6;
			this.button参照.Text = "参照...\r\n";
			this.toolTip1.SetToolTip( this.button参照, "動画ファイルをダイアログから選択します。\r\n" );
			this.button参照.UseVisualStyleBackColor = true;
			this.button参照.Click += new System.EventHandler( this.button参照_Click );
			this.button参照.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button参照_KeyDown );
			// 
			// button背景色
			// 
			this.button背景色.Location = new System.Drawing.Point( 134, 9 );
			this.button背景色.Name = "button背景色";
			this.button背景色.Size = new System.Drawing.Size( 51, 23 );
			this.button背景色.TabIndex = 9;
			this.button背景色.Text = "背景色\r\n";
			this.toolTip1.SetToolTip( this.button背景色, "AVIリスト行の背景色を設定します。\r\n" );
			this.button背景色.UseVisualStyleBackColor = true;
			this.button背景色.Click += new System.EventHandler( this.button背景色_Click );
			this.button背景色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button背景色_KeyDown );
			// 
			// button文字色
			// 
			this.button文字色.Location = new System.Drawing.Point( 191, 9 );
			this.button文字色.Name = "button文字色";
			this.button文字色.Size = new System.Drawing.Size( 51, 23 );
			this.button文字色.TabIndex = 10;
			this.button文字色.Text = "文字色\r\n";
			this.toolTip1.SetToolTip( this.button文字色, "AVIリスト行の文字色を設定します。\r\n" );
			this.button文字色.UseVisualStyleBackColor = true;
			this.button文字色.Click += new System.EventHandler( this.button文字色_Click );
			this.button文字色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button文字色_KeyDown );
			// 
			// button標準色に戻す
			// 
			this.button標準色に戻す.Location = new System.Drawing.Point( 248, 9 );
			this.button標準色に戻す.Name = "button標準色に戻す";
			this.button標準色に戻す.Size = new System.Drawing.Size( 92, 23 );
			this.button標準色に戻す.TabIndex = 11;
			this.button標準色に戻す.Text = "標準色に戻す\r\n";
			this.toolTip1.SetToolTip( this.button標準色に戻す, "AVIリスト行の背景色と文字色を標準色に戻します。\r\n" );
			this.button標準色に戻す.UseVisualStyleBackColor = true;
			this.button標準色に戻す.Click += new System.EventHandler( this.button標準色に戻す_Click );
			this.button標準色に戻す.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button標準色に戻す_KeyDown );
			// 
			// C動画プロパティダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 368, 135 );
			this.ControlBox = false;
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
			this.Controls.Add( this.labelAVI番号 );
			this.Controls.Add( this.textBoxAVI番号 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "C動画プロパティダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "動画プロパティ";
			this.TopMost = true;
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.TextBox textBoxAVI番号;
		private System.Windows.Forms.Label labelAVI番号;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonキャンセル;
		public System.Windows.Forms.TextBox textBoxラベル;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelラベル;
		public System.Windows.Forms.TextBox textBoxファイル;
		private System.Windows.Forms.Label labelファイル;
		private System.Windows.Forms.Button button参照;
		private System.Windows.Forms.Button button背景色;
		private System.Windows.Forms.Button button文字色;
		private System.Windows.Forms.Button button標準色に戻す;
	}
}