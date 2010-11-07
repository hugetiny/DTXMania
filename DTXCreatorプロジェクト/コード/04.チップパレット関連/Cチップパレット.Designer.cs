namespace DTXCreator.チップパレット関連
{
	partial class Cチップパレット
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( Cチップパレット ) );
			this.toolStripツールバー = new System.Windows.Forms.ToolStrip();
			this.toolStripSplitButton表示形式 = new System.Windows.Forms.ToolStripSplitButton();
			this.toolStripMenuItem大きなアイコン = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem小さなアイコン = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem一覧 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem詳細 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton上移動 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton下移動 = new System.Windows.Forms.ToolStripButton();
			this.listViewチップリスト = new System.Windows.Forms.ListView();
			this.columnHeaderラベル = new System.Windows.Forms.ColumnHeader();
			this.columnHeader番号 = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderファイル = new System.Windows.Forms.ColumnHeader();
			this.imageList大きなアイコン = new System.Windows.Forms.ImageList( this.components );
			this.imageList小さなアイコン = new System.Windows.Forms.ImageList( this.components );
			this.contextMenuStripリスト用 = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.toolStripMenuItemパレットから削除する = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripツールバー.SuspendLayout();
			this.contextMenuStripリスト用.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripツールバー
			// 
			this.toolStripツールバー.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripツールバー.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton表示形式,
            this.toolStripSeparator1,
            this.toolStripButton上移動,
            this.toolStripButton下移動} );
			this.toolStripツールバー.Location = new System.Drawing.Point( 0, 0 );
			this.toolStripツールバー.Name = "toolStripツールバー";
			this.toolStripツールバー.Size = new System.Drawing.Size( 284, 25 );
			this.toolStripツールバー.TabIndex = 0;
			this.toolStripツールバー.Text = "toolStrip1";
			// 
			// toolStripSplitButton表示形式
			// 
			this.toolStripSplitButton表示形式.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem大きなアイコン,
            this.toolStripMenuItem小さなアイコン,
            this.toolStripMenuItem一覧,
            this.toolStripMenuItem詳細} );
			this.toolStripSplitButton表示形式.Image = global::DTXCreator.Properties.Resources.表示形式選択;
			this.toolStripSplitButton表示形式.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButton表示形式.Name = "toolStripSplitButton表示形式";
			this.toolStripSplitButton表示形式.Size = new System.Drawing.Size( 106, 22 );
			this.toolStripSplitButton表示形式.Text = "表示形式(&V)";
			this.toolStripSplitButton表示形式.ToolTipText = "表示形式を変更します。\r\n";
			this.toolStripSplitButton表示形式.ButtonClick += new System.EventHandler( this.toolStripSplitButton表示形式_ButtonClick );
			// 
			// toolStripMenuItem大きなアイコン
			// 
			this.toolStripMenuItem大きなアイコン.Name = "toolStripMenuItem大きなアイコン";
			this.toolStripMenuItem大きなアイコン.Size = new System.Drawing.Size( 179, 22 );
			this.toolStripMenuItem大きなアイコン.Text = "大きなアイコン(&N)";
			this.toolStripMenuItem大きなアイコン.Click += new System.EventHandler( this.toolStripMenuItem大きなアイコン_Click );
			// 
			// toolStripMenuItem小さなアイコン
			// 
			this.toolStripMenuItem小さなアイコン.Name = "toolStripMenuItem小さなアイコン";
			this.toolStripMenuItem小さなアイコン.Size = new System.Drawing.Size( 179, 22 );
			this.toolStripMenuItem小さなアイコン.Text = "小さなアイコン(&S)";
			this.toolStripMenuItem小さなアイコン.Click += new System.EventHandler( this.toolStripMenuItem小さなアイコン_Click );
			// 
			// toolStripMenuItem一覧
			// 
			this.toolStripMenuItem一覧.Name = "toolStripMenuItem一覧";
			this.toolStripMenuItem一覧.Size = new System.Drawing.Size( 179, 22 );
			this.toolStripMenuItem一覧.Text = "一覧(&L)";
			this.toolStripMenuItem一覧.Click += new System.EventHandler( this.toolStripMenuItem一覧_Click );
			// 
			// toolStripMenuItem詳細
			// 
			this.toolStripMenuItem詳細.Name = "toolStripMenuItem詳細";
			this.toolStripMenuItem詳細.Size = new System.Drawing.Size( 179, 22 );
			this.toolStripMenuItem詳細.Text = "詳細(&D)";
			this.toolStripMenuItem詳細.Click += new System.EventHandler( this.toolStripMenuItem詳細_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size( 6, 25 );
			// 
			// toolStripButton上移動
			// 
			this.toolStripButton上移動.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton上移動.Image = global::DTXCreator.Properties.Resources.上移動;
			this.toolStripButton上移動.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton上移動.Name = "toolStripButton上移動";
			this.toolStripButton上移動.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButton上移動.Text = "toolStripButton1";
			this.toolStripButton上移動.ToolTipText = "行を１つ上に移動します。\r\n";
			this.toolStripButton上移動.Click += new System.EventHandler( this.toolStripButton上移動_Click );
			// 
			// toolStripButton下移動
			// 
			this.toolStripButton下移動.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton下移動.Image = global::DTXCreator.Properties.Resources.下移動;
			this.toolStripButton下移動.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton下移動.Name = "toolStripButton下移動";
			this.toolStripButton下移動.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButton下移動.Text = "toolStripButton1";
			this.toolStripButton下移動.ToolTipText = "行を１つ下に移動します。\r\n";
			this.toolStripButton下移動.Click += new System.EventHandler( this.toolStripButton下移動_Click );
			// 
			// listViewチップリスト
			// 
			this.listViewチップリスト.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderラベル,
            this.columnHeader番号,
            this.columnHeaderファイル} );
			this.listViewチップリスト.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewチップリスト.Font = new System.Drawing.Font( "ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 128 ) ) );
			this.listViewチップリスト.FullRowSelect = true;
			this.listViewチップリスト.GridLines = true;
			this.listViewチップリスト.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewチップリスト.HideSelection = false;
			this.listViewチップリスト.LargeImageList = this.imageList大きなアイコン;
			this.listViewチップリスト.Location = new System.Drawing.Point( 0, 25 );
			this.listViewチップリスト.MultiSelect = false;
			this.listViewチップリスト.Name = "listViewチップリスト";
			this.listViewチップリスト.Size = new System.Drawing.Size( 284, 237 );
			this.listViewチップリスト.SmallImageList = this.imageList小さなアイコン;
			this.listViewチップリスト.TabIndex = 6;
			this.listViewチップリスト.UseCompatibleStateImageBehavior = false;
			this.listViewチップリスト.View = System.Windows.Forms.View.Details;
			this.listViewチップリスト.SelectedIndexChanged += new System.EventHandler( this.listViewチップリスト_SelectedIndexChanged );
			// 
			// columnHeaderラベル
			// 
			this.columnHeaderラベル.Text = "ラベル";
			this.columnHeaderラベル.Width = 100;
			// 
			// columnHeader番号
			// 
			this.columnHeader番号.Text = "No";
			this.columnHeader番号.Width = 28;
			// 
			// columnHeaderファイル
			// 
			this.columnHeaderファイル.Text = "ファイル";
			this.columnHeaderファイル.Width = 150;
			// 
			// imageList大きなアイコン
			// 
			this.imageList大きなアイコン.ImageStream = ( (System.Windows.Forms.ImageListStreamer) ( resources.GetObject( "imageList大きなアイコン.ImageStream" ) ) );
			this.imageList大きなアイコン.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList大きなアイコン.Images.SetKeyName( 0, "" );
			this.imageList大きなアイコン.Images.SetKeyName( 1, "" );
			this.imageList大きなアイコン.Images.SetKeyName( 2, "" );
			// 
			// imageList小さなアイコン
			// 
			this.imageList小さなアイコン.ImageStream = ( (System.Windows.Forms.ImageListStreamer) ( resources.GetObject( "imageList小さなアイコン.ImageStream" ) ) );
			this.imageList小さなアイコン.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList小さなアイコン.Images.SetKeyName( 0, "MusicDoc.PNG" );
			this.imageList小さなアイコン.Images.SetKeyName( 1, "PicDoc.PNG" );
			this.imageList小さなアイコン.Images.SetKeyName( 2, "VideoDoc.PNG" );
			// 
			// contextMenuStripリスト用
			// 
			this.contextMenuStripリスト用.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemパレットから削除する} );
			this.contextMenuStripリスト用.Name = "contextMenuStripリスト用";
			this.contextMenuStripリスト用.Size = new System.Drawing.Size( 192, 26 );
			// 
			// toolStripMenuItemパレットから削除する
			// 
			this.toolStripMenuItemパレットから削除する.Name = "toolStripMenuItemパレットから削除する";
			this.toolStripMenuItemパレットから削除する.Size = new System.Drawing.Size( 191, 22 );
			this.toolStripMenuItemパレットから削除する.Text = "パレットから削除(&D)";
			this.toolStripMenuItemパレットから削除する.Click += new System.EventHandler( this.toolStripMenuItemパレットから削除する_Click );
			// 
			// Cチップパレット
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 284, 262 );
			this.ControlBox = false;
			this.Controls.Add( this.listViewチップリスト );
			this.Controls.Add( this.toolStripツールバー );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Cチップパレット";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Cチップパレット";
			this.TopMost = true;
			this.DragDrop += new System.Windows.Forms.DragEventHandler( this.Cチップパレット_DragDrop );
			this.DragEnter += new System.Windows.Forms.DragEventHandler( this.Cチップパレット_DragEnter );
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.Cチップパレット_FormClosing );
			this.toolStripツールバー.ResumeLayout( false );
			this.toolStripツールバー.PerformLayout();
			this.contextMenuStripリスト用.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStripツールバー;
		internal System.Windows.Forms.ListView listViewチップリスト;
		private System.Windows.Forms.ImageList imageList小さなアイコン;
		private System.Windows.Forms.ImageList imageList大きなアイコン;
		private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton表示形式;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem大きなアイコン;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem小さなアイコン;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem一覧;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem詳細;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButton上移動;
		private System.Windows.Forms.ToolStripButton toolStripButton下移動;
		private System.Windows.Forms.ColumnHeader columnHeaderラベル;
		private System.Windows.Forms.ColumnHeader columnHeader番号;
		private System.Windows.Forms.ColumnHeader columnHeaderファイル;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripリスト用;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemパレットから削除する;
	}
}