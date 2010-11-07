namespace DTXCreator.汎用
{
	partial class Cメッセージポップアップ
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
			this.panelメッセージ = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// panelメッセージ
			// 
			this.panelメッセージ.Location = new System.Drawing.Point( 113, 12 );
			this.panelメッセージ.Name = "panelメッセージ";
			this.panelメッセージ.Size = new System.Drawing.Size( 212, 86 );
			this.panelメッセージ.TabIndex = 1;
			this.panelメッセージ.Paint += new System.Windows.Forms.PaintEventHandler( this.panelメッセージ_Paint );
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox1.Image = global::DTXCreator.Properties.Resources.りらちょー;
			this.pictureBox1.Location = new System.Drawing.Point( 12, 12 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 95, 80 );
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// Cメッセージポップアップ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size( 337, 110 );
			this.ControlBox = false;
			this.Controls.Add( this.pictureBox1 );
			this.Controls.Add( this.panelメッセージ );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Cメッセージポップアップ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Cメッセージポップアップ";
			this.Load += new System.EventHandler( this.Cメッセージポップアップ_Load );
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.Cメッセージポップアップ_FormClosing );
			( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Panel panelメッセージ;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}