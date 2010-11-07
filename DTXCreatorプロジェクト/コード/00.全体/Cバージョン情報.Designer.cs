
namespace DTXCreator
{
	partial class Cバージョン情報
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		/// 
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( this.components != null ) )
			{
				this.components.Dispose();
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
			base.SuspendLayout();
			// 
			// Cバージョン情報
			// 
			base.AutoScaleDimensions = new System.Drawing.SizeF( 6f, 12f );
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::DTXCreator.Properties.Resources.バージョン情報;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			base.ClientSize = new System.Drawing.Size( 450, 250 );
			base.ControlBox = false;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Cバージョン情報";
			base.Padding = new System.Windows.Forms.Padding( 8 );
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			base.Paint += new System.Windows.Forms.PaintEventHandler( this.Cバージョン情報_Paint );
			base.Click += new System.EventHandler( this.Cバージョン情報_Click );
			base.KeyDown += new System.Windows.Forms.KeyEventHandler( this.Cバージョン情報_KeyDown );
			base.ResumeLayout( false );

		}
		
		#endregion
	}
}
