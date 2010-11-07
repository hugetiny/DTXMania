namespace DTXCreator.譜面
{
	partial class C置換ダイアログ
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
			this.buttonキャンセル = new System.Windows.Forms.Button();
			this.button置換 = new System.Windows.Forms.Button();
			this.label説明 = new System.Windows.Forms.Label();
			this.radioButton表裏反転 = new System.Windows.Forms.RadioButton();
			this.radioButton単純置換 = new System.Windows.Forms.RadioButton();
			this.textBox元番号 = new System.Windows.Forms.TextBox();
			this.textBox先番号 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonキャンセル
			// 
			this.buttonキャンセル.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonキャンセル.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonキャンセル.Location = new System.Drawing.Point( 305, 97 );
			this.buttonキャンセル.Name = "buttonキャンセル";
			this.buttonキャンセル.Size = new System.Drawing.Size( 75, 23 );
			this.buttonキャンセル.TabIndex = 4;
			this.buttonキャンセル.Text = "キャンセル";
			this.buttonキャンセル.UseVisualStyleBackColor = true;
			// 
			// button置換
			// 
			this.button置換.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.button置換.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button置換.Location = new System.Drawing.Point( 224, 97 );
			this.button置換.Name = "button置換";
			this.button置換.Size = new System.Drawing.Size( 75, 23 );
			this.button置換.TabIndex = 3;
			this.button置換.Text = "置換";
			this.button置換.UseVisualStyleBackColor = true;
			// 
			// label説明
			// 
			this.label説明.AutoSize = true;
			this.label説明.Location = new System.Drawing.Point( 12, 11 );
			this.label説明.Name = "label説明";
			this.label説明.Size = new System.Drawing.Size( 224, 12 );
			this.label説明.TabIndex = 5;
			this.label説明.Text = "現在選択中のチップを対象に置換を行います。\r\n";
			// 
			// radioButton表裏反転
			// 
			this.radioButton表裏反転.AutoSize = true;
			this.radioButton表裏反転.Location = new System.Drawing.Point( 12, 82 );
			this.radioButton表裏反転.Name = "radioButton表裏反転";
			this.radioButton表裏反転.Size = new System.Drawing.Size( 131, 16 );
			this.radioButton表裏反転.TabIndex = 2;
			this.radioButton表裏反転.TabStop = true;
			this.radioButton表裏反転.Text = "チップの表裏を反転(&A)\r\n";
			this.radioButton表裏反転.UseVisualStyleBackColor = true;
			this.radioButton表裏反転.KeyDown += new System.Windows.Forms.KeyEventHandler( this.radioButton表裏反転_KeyDown );
			// 
			// radioButton単純置換
			// 
			this.radioButton単純置換.AutoSize = true;
			this.radioButton単純置換.Location = new System.Drawing.Point( 12, 35 );
			this.radioButton単純置換.Name = "radioButton単純置換";
			this.radioButton単純置換.Size = new System.Drawing.Size( 116, 16 );
			this.radioButton単純置換.TabIndex = 6;
			this.radioButton単純置換.TabStop = true;
			this.radioButton単純置換.Text = "チップの置き換え(&R)";
			this.radioButton単純置換.UseVisualStyleBackColor = true;
			this.radioButton単純置換.KeyDown += new System.Windows.Forms.KeyEventHandler( this.radioButton単純置換_KeyDown );
			// 
			// textBox元番号
			// 
			this.textBox元番号.Location = new System.Drawing.Point( 44, 57 );
			this.textBox元番号.Name = "textBox元番号";
			this.textBox元番号.Size = new System.Drawing.Size( 50, 19 );
			this.textBox元番号.TabIndex = 0;
			this.textBox元番号.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBox元番号_KeyDown );
			// 
			// textBox先番号
			// 
			this.textBox先番号.Location = new System.Drawing.Point( 123, 57 );
			this.textBox先番号.Name = "textBox先番号";
			this.textBox先番号.Size = new System.Drawing.Size( 50, 19 );
			this.textBox先番号.TabIndex = 1;
			this.textBox先番号.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textBox先番号_KeyDown );
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 100, 60 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 14, 12 );
			this.label1.TabIndex = 8;
			this.label1.Text = "を";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point( 179, 60 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 65, 12 );
			this.label2.TabIndex = 7;
			this.label2.Text = "に置き換える";
			// 
			// C置換ダイアログ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 392, 132 );
			this.ControlBox = false;
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.textBox先番号 );
			this.Controls.Add( this.textBox元番号 );
			this.Controls.Add( this.radioButton単純置換 );
			this.Controls.Add( this.radioButton表裏反転 );
			this.Controls.Add( this.label説明 );
			this.Controls.Add( this.button置換 );
			this.Controls.Add( this.buttonキャンセル );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "C置換ダイアログ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "置換";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.C置換ダイアログ_FormClosing );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonキャンセル;
		private System.Windows.Forms.Button button置換;
		private System.Windows.Forms.Label label説明;
		private System.Windows.Forms.RadioButton radioButton表裏反転;
		private System.Windows.Forms.RadioButton radioButton単純置換;
		private System.Windows.Forms.TextBox textBox元番号;
		private System.Windows.Forms.TextBox textBox先番号;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}