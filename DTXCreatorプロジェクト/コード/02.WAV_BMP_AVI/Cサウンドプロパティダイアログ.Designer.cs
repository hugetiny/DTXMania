namespace DTXCreator.WAV_BMP_AVI
{
	partial class Cサウンドプロパティダイアログ
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
			this.label音量 = new System.Windows.Forms.Label();
			this.label位置 = new System.Windows.Forms.Label();
			this.button参照 = new System.Windows.Forms.Button();
			this.hScrollBar音量 = new System.Windows.Forms.HScrollBar();
			this.textBox音量 = new System.Windows.Forms.TextBox();
			this.textBox位置 = new System.Windows.Forms.TextBox();
			this.hScrollBar位置 = new System.Windows.Forms.HScrollBar();
			this.button背景色 = new System.Windows.Forms.Button();
			this.button文字色 = new System.Windows.Forms.Button();
			this.button標準色に戻す = new System.Windows.Forms.Button();
			this.button試聴 = new System.Windows.Forms.Button();
			this.checkBoxBGM = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.textBoxWAV番号 = new System.Windows.Forms.TextBox();
			this.labelWAV番号 = new System.Windows.Forms.Label();
			this.label音量無音 = new System.Windows.Forms.Label();
			this.label位置左 = new System.Windows.Forms.Label();
			this.labe音量原音 = new System.Windows.Forms.Label();
			this.label位置右 = new System.Windows.Forms.Label();
			this.label位置中央 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBoxラベル
			// 
			this.textBoxラベル.Location = new System.Drawing.Point( 69, 40 );
			this.textBoxラベル.Name = "textBoxラベル";
			this.textBoxラベル.Size = new System.Drawing.Size( 294, 19 );
			this.textBoxラベル.TabIndex = 0;
			this.toolTip1.SetToolTip( this.textBoxラベル, "サウンドファイルに名前を自由に設定できます。\r\n" );
			this.textBoxラベル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxラベル_KeyDown );
			// 
			// labelラベル
			// 
			this.labelラベル.AutoSize = true;
			this.labelラベル.Location = new System.Drawing.Point( 9, 43 );
			this.labelラベル.Name = "labelラベル";
			this.labelラベル.Size = new System.Drawing.Size( 33, 12 );
			this.labelラベル.TabIndex = 16;
			this.labelラベル.Text = "ラベル\r\n";
			this.toolTip1.SetToolTip( this.labelラベル, "サウンドファイルに名前を自由に設定できます。\r\n" );
			// 
			// textBoxファイル
			// 
			this.textBoxファイル.Location = new System.Drawing.Point( 69, 65 );
			this.textBoxファイル.Name = "textBoxファイル";
			this.textBoxファイル.Size = new System.Drawing.Size( 217, 19 );
			this.textBoxファイル.TabIndex = 1;
			this.toolTip1.SetToolTip( this.textBoxファイル, "サウンドファイル名。\r\n" );
			this.textBoxファイル.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBoxファイル_KeyDown );
			// 
			// labelファイル
			// 
			this.labelファイル.AutoSize = true;
			this.labelファイル.Location = new System.Drawing.Point( 9, 68 );
			this.labelファイル.Name = "labelファイル";
			this.labelファイル.Size = new System.Drawing.Size( 51, 12 );
			this.labelファイル.TabIndex = 17;
			this.labelファイル.Text = "ファイル名\r\n";
			this.toolTip1.SetToolTip( this.labelファイル, "サウンドファイル名。\r\n" );
			// 
			// label音量
			// 
			this.label音量.AutoSize = true;
			this.label音量.Location = new System.Drawing.Point( 9, 123 );
			this.label音量.Name = "label音量";
			this.label音量.Size = new System.Drawing.Size( 29, 12 );
			this.label音量.TabIndex = 18;
			this.label音量.Text = "音量";
			this.toolTip1.SetToolTip( this.label音量, "サウンドの音量を設定します。\r\n" );
			// 
			// label位置
			// 
			this.label位置.AutoSize = true;
			this.label位置.Location = new System.Drawing.Point( 9, 159 );
			this.label位置.Name = "label位置";
			this.label位置.Size = new System.Drawing.Size( 29, 12 );
			this.label位置.TabIndex = 19;
			this.label位置.Text = "位置";
			this.toolTip1.SetToolTip( this.label位置, "サウンドの位置を設定します。" );
			// 
			// button参照
			// 
			this.button参照.Location = new System.Drawing.Point( 292, 61 );
			this.button参照.Name = "button参照";
			this.button参照.Size = new System.Drawing.Size( 71, 23 );
			this.button参照.TabIndex = 2;
			this.button参照.Text = "参照...\r\n";
			this.toolTip1.SetToolTip( this.button参照, "サウンドファイルをダイアログから選択します。\r\n" );
			this.button参照.UseVisualStyleBackColor = true;
			this.button参照.Click += new System.EventHandler( this.button参照_Click );
			this.button参照.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button参照_KeyDown );
			// 
			// hScrollBar音量
			// 
			this.hScrollBar音量.Location = new System.Drawing.Point( 130, 120 );
			this.hScrollBar音量.Maximum = 109;
			this.hScrollBar音量.Name = "hScrollBar音量";
			this.hScrollBar音量.Size = new System.Drawing.Size( 182, 17 );
			this.hScrollBar音量.TabIndex = 5;
			this.toolTip1.SetToolTip( this.hScrollBar音量, "サウンドの音量を設定します。\r\n" );
			this.hScrollBar音量.Value = 100;
			this.hScrollBar音量.ValueChanged += new System.EventHandler( this.hScrollBar音量_ValueChanged );
			// 
			// textBox音量
			// 
			this.textBox音量.Location = new System.Drawing.Point( 69, 120 );
			this.textBox音量.MaxLength = 3;
			this.textBox音量.Name = "textBox音量";
			this.textBox音量.Size = new System.Drawing.Size( 58, 19 );
			this.textBox音量.TabIndex = 4;
			this.textBox音量.Text = "100";
			this.textBox音量.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip1.SetToolTip( this.textBox音量, "サウンドの音量を設定します。\r\n" );
			this.textBox音量.WordWrap = false;
			this.textBox音量.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBox音量_KeyDown );
			this.textBox音量.Leave += new System.EventHandler( this.textBox音量_Leave );
			// 
			// textBox位置
			// 
			this.textBox位置.Location = new System.Drawing.Point( 69, 156 );
			this.textBox位置.MaxLength = 4;
			this.textBox位置.Name = "textBox位置";
			this.textBox位置.Size = new System.Drawing.Size( 58, 19 );
			this.textBox位置.TabIndex = 6;
			this.textBox位置.Text = "0";
			this.textBox位置.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip1.SetToolTip( this.textBox位置, "サウンドの位置を設定します。\r\n" );
			this.textBox位置.WordWrap = false;
			this.textBox位置.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBox位置_KeyDown );
			this.textBox位置.Leave += new System.EventHandler( this.textBox位置_Leave );
			// 
			// hScrollBar位置
			// 
			this.hScrollBar位置.Location = new System.Drawing.Point( 130, 158 );
			this.hScrollBar位置.Maximum = 209;
			this.hScrollBar位置.Name = "hScrollBar位置";
			this.hScrollBar位置.Size = new System.Drawing.Size( 182, 17 );
			this.hScrollBar位置.TabIndex = 7;
			this.toolTip1.SetToolTip( this.hScrollBar位置, "サウンドの位置を設定します。\r\n" );
			this.hScrollBar位置.Value = 100;
			this.hScrollBar位置.ValueChanged += new System.EventHandler( this.hScrollBar位置_ValueChanged );
			// 
			// button背景色
			// 
			this.button背景色.Location = new System.Drawing.Point( 133, 9 );
			this.button背景色.Name = "button背景色";
			this.button背景色.Size = new System.Drawing.Size( 51, 23 );
			this.button背景色.TabIndex = 11;
			this.button背景色.Text = "背景色";
			this.toolTip1.SetToolTip( this.button背景色, "WAVリスト行の背景色を設定します。\r\n" );
			this.button背景色.UseVisualStyleBackColor = true;
			this.button背景色.Click += new System.EventHandler( this.button背景色_Click );
			this.button背景色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button背景色_KeyDown );
			// 
			// button文字色
			// 
			this.button文字色.Location = new System.Drawing.Point( 190, 9 );
			this.button文字色.Name = "button文字色";
			this.button文字色.Size = new System.Drawing.Size( 51, 23 );
			this.button文字色.TabIndex = 12;
			this.button文字色.Text = "文字色\r\n";
			this.toolTip1.SetToolTip( this.button文字色, "WAVリスト行の文字色を設定します。\r\n" );
			this.button文字色.UseVisualStyleBackColor = true;
			this.button文字色.Click += new System.EventHandler( this.button文字色_Click );
			this.button文字色.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button文字色_KeyDown );
			// 
			// button標準色に戻す
			// 
			this.button標準色に戻す.Location = new System.Drawing.Point( 247, 9 );
			this.button標準色に戻す.Name = "button標準色に戻す";
			this.button標準色に戻す.Size = new System.Drawing.Size( 92, 23 );
			this.button標準色に戻す.TabIndex = 13;
			this.button標準色に戻す.Text = "標準色に戻す\r\n";
			this.toolTip1.SetToolTip( this.button標準色に戻す, "WAVリスト行の背景色と文字色を標準色に戻します。\r\n" );
			this.button標準色に戻す.UseVisualStyleBackColor = true;
			this.button標準色に戻す.Click += new System.EventHandler( this.button標準色に戻す_Click );
			this.button標準色に戻す.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button標準色に戻す_KeyDown );
			// 
			// button試聴
			// 
			this.button試聴.Location = new System.Drawing.Point( 65, 202 );
			this.button試聴.Name = "button試聴";
			this.button試聴.Size = new System.Drawing.Size( 75, 23 );
			this.button試聴.TabIndex = 10;
			this.button試聴.Text = "試聴";
			this.toolTip1.SetToolTip( this.button試聴, "現在のサウンドファイルを試聴します。\r\n" );
			this.button試聴.UseVisualStyleBackColor = true;
			this.button試聴.Click += new System.EventHandler( this.button試聴_Click );
			this.button試聴.KeyDown += new System.Windows.Forms.KeyEventHandler( this.button試聴_KeyDown );
			// 
			// checkBoxBGM
			// 
			this.checkBoxBGM.AutoSize = true;
			this.checkBoxBGM.Location = new System.Drawing.Point( 69, 90 );
			this.checkBoxBGM.Name = "checkBoxBGM";
			this.checkBoxBGM.Size = new System.Drawing.Size( 182, 16 );
			this.checkBoxBGM.TabIndex = 3;
			this.checkBoxBGM.Text = "このサウンドをBGMとして使用する\r\n";
			this.toolTip1.SetToolTip( this.checkBoxBGM, "このサウンドをBGMサウンドとして使用する場合にオンにします。\r\n" );
			this.checkBoxBGM.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 207, 202 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 288, 202 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 9;
			this.buttonキャンセル.Text = "キャンセル\r\n";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// textBoxWAV番号
			// 
			this.textBoxWAV番号.Location = new System.Drawing.Point( 69, 11 );
			this.textBoxWAV番号.Name = "textBoxWAV番号";
			this.textBoxWAV番号.ReadOnly = true;
			this.textBoxWAV番号.Size = new System.Drawing.Size( 58, 19 );
			this.textBoxWAV番号.TabIndex = 14;
			this.textBoxWAV番号.TabStop = false;
			this.textBoxWAV番号.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelWAV番号
			// 
			this.labelWAV番号.AutoSize = true;
			this.labelWAV番号.Location = new System.Drawing.Point( 9, 14 );
			this.labelWAV番号.Name = "labelWAV番号";
			this.labelWAV番号.Size = new System.Drawing.Size( 54, 12 );
			this.labelWAV番号.TabIndex = 15;
			this.labelWAV番号.Text = "WAV番号\r\n";
			// 
			// label音量無音
			// 
			this.label音量無音.AutoSize = true;
			this.label音量無音.Location = new System.Drawing.Point( 128, 108 );
			this.label音量無音.Name = "label音量無音";
			this.label音量無音.Size = new System.Drawing.Size( 29, 12 );
			this.label音量無音.TabIndex = 20;
			this.label音量無音.Text = "無音";
			this.label音量無音.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label位置左
			// 
			this.label位置左.AutoSize = true;
			this.label位置左.Location = new System.Drawing.Point( 128, 142 );
			this.label位置左.Name = "label位置左";
			this.label位置左.Size = new System.Drawing.Size( 17, 12 );
			this.label位置左.TabIndex = 22;
			this.label位置左.Text = "左";
			this.label位置左.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labe音量原音
			// 
			this.labe音量原音.AutoSize = true;
			this.labe音量原音.Location = new System.Drawing.Point( 283, 108 );
			this.labe音量原音.Name = "labe音量原音";
			this.labe音量原音.Size = new System.Drawing.Size( 29, 12 );
			this.labe音量原音.TabIndex = 21;
			this.labe音量原音.Text = "原音\r\n";
			this.labe音量原音.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label位置右
			// 
			this.label位置右.AutoSize = true;
			this.label位置右.Location = new System.Drawing.Point( 295, 142 );
			this.label位置右.Name = "label位置右";
			this.label位置右.Size = new System.Drawing.Size( 17, 12 );
			this.label位置右.TabIndex = 24;
			this.label位置右.Text = "右";
			this.label位置右.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label位置中央
			// 
			this.label位置中央.AutoSize = true;
			this.label位置中央.Location = new System.Drawing.Point( 208, 142 );
			this.label位置中央.Name = "label位置中央";
			this.label位置中央.Size = new System.Drawing.Size( 29, 12 );
			this.label位置中央.TabIndex = 23;
			this.label位置中央.Text = "中央";
			this.label位置中央.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cサウンドプロパティダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 378, 237 );
			this.ControlBox = false;
			this.Controls.Add( this.checkBoxBGM );
			this.Controls.Add( this.button試聴 );
			this.Controls.Add( this.button標準色に戻す );
			this.Controls.Add( this.button文字色 );
			this.Controls.Add( this.button背景色 );
			this.Controls.Add( this.label位置中央 );
			this.Controls.Add( this.label位置右 );
			this.Controls.Add( this.labe音量原音 );
			this.Controls.Add( this.hScrollBar位置 );
			this.Controls.Add( this.textBox位置 );
			this.Controls.Add( this.textBox音量 );
			this.Controls.Add( this.hScrollBar音量 );
			this.Controls.Add( this.label位置左 );
			this.Controls.Add( this.label音量無音 );
			this.Controls.Add( this.button参照 );
			this.Controls.Add( this.labelWAV番号 );
			this.Controls.Add( this.textBoxWAV番号 );
			this.Controls.Add( this.label位置 );
			this.Controls.Add( this.label音量 );
			this.Controls.Add( this.labelファイル );
			this.Controls.Add( this.textBoxファイル );
			this.Controls.Add( this.labelラベル );
			this.Controls.Add( this.textBoxラベル );
			this.Controls.Add( this.buttonキャンセル );
			this.Controls.Add( this.buttonOK );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Cサウンドプロパティダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "サウンドプロパティ";
			this.TopMost = true;
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonキャンセル;
		internal System.Windows.Forms.TextBox textBoxラベル;
		private System.Windows.Forms.Label labelラベル;
		internal System.Windows.Forms.TextBox textBoxファイル;
		private System.Windows.Forms.Label labelファイル;
		private System.Windows.Forms.Label label音量;
		private System.Windows.Forms.Label label位置;
		public System.Windows.Forms.TextBox textBoxWAV番号;
		private System.Windows.Forms.Label labelWAV番号;
		private System.Windows.Forms.Button button参照;
		private System.Windows.Forms.Label label音量無音;
		private System.Windows.Forms.Label label位置左;
		public System.Windows.Forms.HScrollBar hScrollBar音量;
		internal System.Windows.Forms.TextBox textBox音量;
		internal System.Windows.Forms.TextBox textBox位置;
		public System.Windows.Forms.HScrollBar hScrollBar位置;
		private System.Windows.Forms.Label labe音量原音;
		private System.Windows.Forms.Label label位置右;
		private System.Windows.Forms.Label label位置中央;
		private System.Windows.Forms.Button button背景色;
		private System.Windows.Forms.Button button文字色;
		private System.Windows.Forms.Button button標準色に戻す;
		private System.Windows.Forms.Button button試聴;
		internal System.Windows.Forms.CheckBox checkBoxBGM;
	}
}