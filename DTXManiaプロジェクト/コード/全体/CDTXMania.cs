using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;
using SampleFramework;

namespace DTXMania
{
	internal class CDTXMania : Game
	{
		// プロパティ

		public static readonly string VERSION = "085(101102)";
		public static readonly string SLIMDXDLL = "c_net20x86_Jun2010";
		public static readonly string D3DXDLL = "d3dx9_43.dll";		// June 2010
        //public static readonly string D3DXDLL = "d3dx9_42.dll";	// February 2010
        //public static readonly string D3DXDLL = "d3dx9_41.dll";	// March 2009

		public static CDTXMania app
		{
			get;
			private set;
		}
		public static C文字コンソール act文字コンソール
		{ 
			get;
			private set;
		}
		public static bool bコンパクトモード
		{
			get;
			private set;
		}
		public static CConfigIni ConfigIni
		{
			get; 
			private set;
		}
		public static CDTX DTX
		{
			get
			{
				return dtx;
			}
			set
			{
				if( ( dtx != null ) && ( app != null ) )
				{
					dtx.On非活性化();
					app.listトップレベルActivities.Remove( dtx );
				}
				dtx = value;
				if( ( dtx != null ) && ( app != null ) )
				{
					app.listトップレベルActivities.Add( dtx );
				}
			}
		}
		public static CFPS FPS
		{ 
			get; 
			private set;
		}
		public static CInput管理 Input管理 
		{
			get;
			private set;
		}
		public static int nPerfect範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nPerfect範囲ms >= 0 ) )
					{
						return c曲リストノード.nPerfect範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Perfect;
			}
		}
		public static int nGreat範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nGreat範囲ms >= 0 ) )
					{
						return c曲リストノード.nGreat範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Great;
			}
		}
		public static int nGood範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nGood範囲ms >= 0 ) )
					{
						return c曲リストノード.nGood範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Good;
			}
		}
		public static int nPoor範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX ) ) && ( c曲リストノード.nPoor範囲ms >= 0 ) )
					{
						return c曲リストノード.nPoor範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Poor;
			}
		}
		public static CPad Pad 
		{
			get;
			private set;
		}
		public static Random Random
		{
			get;
			private set;
		}
		public static CSkin Skin
		{
			get; 
			private set;
		}
		public static CSongs管理 Songs管理 
		{
			get;
			private set;
		}
		public static CSound管理 Sound管理
		{
			get; 
			private set;
		}
		public static CStage起動 stage起動 
		{
			get; 
			private set;
		}
		public static CStageタイトル stageタイトル
		{
			get;
			private set;
		}
		public static CStageオプション stageオプション
		{ 
			get;
			private set;
		}
		public static CStageコンフィグ stageコンフィグ 
		{ 
			get; 
			private set;
		}
		public static CStage選曲 stage選曲
		{
			get;
			private set;
		}
		public static CStage曲読み込み stage曲読み込み
		{
			get;
			private set;
		}
		public static CStage演奏ギター画面 stage演奏ギター画面
		{
			get;
			private set;
		}
		public static CStage演奏ドラム画面 stage演奏ドラム画面
		{
			get;
			private set;
		}
		public static CStage結果 stage結果
		{
			get;
			private set;
		}
		public static CStage終了 stage終了
		{
			get;
			private set;
		}
		public static CStage r現在のステージ = null;
		public static CStage r直前のステージ = null;
		public static string strEXEのあるフォルダ 
		{
			get;
			private set;
		}
		public static string strコンパクトモードファイル
		{ 
			get; 
			private set;
		}
		public static CTimer Timer
		{
			get;
			private set;
		}
		public static Format TextureFormat = Format.A8R8G8B8;
		internal static IPluginActivity act現在入力を占有中のプラグイン = null;
		public bool bApplicationActive
		{
			get; 
			private set;
		}
		public bool b次のタイミングで垂直帰線同期切り替えを行う
		{
			get; 
			set;
		}
		public bool b次のタイミングで全画面・ウィンドウ切り替えを行う
		{
			get;
			set;
		}
		public Device Device
		{
			get { return base.GraphicsDeviceManager.Direct3D9.Device; }
		}
		public CPluginHost PluginHost
		{
			get;
			private set;
		}
		public List<STPlugin> listプラグイン = new List<STPlugin>();
		public struct STPlugin
		{
			public IPluginActivity plugin;
			public string strプラグインフォルダ;
			public string strアセンブリ簡易名;
			public Version Version;
		}
		private static Size currentClientSize		// #23510 2010.10.27 add yyagi to keep current window size
		{
			get;
			set;
		}

		// コンストラクタ

		public CDTXMania()
		{
			CDTXMania.app = this;
			this.t起動処理();
		}


		// メソッド

		public void t全画面・ウィンドウモード切り替え()
		{
			DeviceSettings settings = base.GraphicsDeviceManager.CurrentSettings.Clone();
			if( ( ConfigIni != null ) && ( ConfigIni.bウィンドウモード != settings.Windowed ) )
			{
				settings.Windowed = ConfigIni.bウィンドウモード;
				if (ConfigIni.bウィンドウモード == false)	// #23510 2010.10.27 yyagi: backup current window size before going fullscreen mode
				{
					currentClientSize = this.Window.ClientSize;
					ConfigIni.nウインドウwidth = this.Window.ClientSize.Width;
					ConfigIni.nウインドウheight = this.Window.ClientSize.Height;
				}
				base.GraphicsDeviceManager.ChangeDevice( settings );
				if (ConfigIni.bウィンドウモード == true)	// #23510 2010.10.27 yyagi: to resume window size from backuped value
				{
					base.Window.ClientSize =
						new Size(currentClientSize.Width, currentClientSize.Height);
				}					
			}
		}


		// Game 実装

		protected override void Initialize()
		{
			if( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnManagedリソースの作成();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnManagedリソースの作成();
				Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
			}
		}
		protected override void LoadContent()
		{
			if ( ConfigIni.bウィンドウモード )
			{
				if( !this.bマウスカーソル表示中 )
				{
					Cursor.Show();
					this.bマウスカーソル表示中 = true;
				}
			}
			else if( this.bマウスカーソル表示中 )
			{
				Cursor.Hide();
				this.bマウスカーソル表示中 = false;
			}
			this.Device.SetTransform( TransformState.View, Matrix.LookAtLH( new Vector3( 0f, 0f, (float) ( -240.0 * Math.Sqrt( 3.0 ) ) ), new Vector3( 0f, 0f, 0f ), new Vector3( 0f, 1f, 0f ) ) );
			this.Device.SetTransform( TransformState.Projection, Matrix.PerspectiveFovLH( C変換.DegreeToRadian( (float) 60f ), ( (float) this.Device.Viewport.Width ) / ( (float) this.Device.Viewport.Height ), -100f, 100f ) );
			this.Device.SetRenderState( RenderState.Lighting, false );
			this.Device.SetRenderState( RenderState.ZEnable, false );
			this.Device.SetRenderState( RenderState.AntialiasedLineEnable, false );
			this.Device.SetRenderState( RenderState.AlphaTestEnable, true );
			this.Device.SetRenderState( RenderState.AlphaRef, 10 );
			this.Device.SetRenderState<Compare>( RenderState.AlphaFunc, Compare.Greater );
			this.Device.SetRenderState( RenderState.AlphaBlendEnable, true );
			this.Device.SetRenderState<Blend>( RenderState.SourceBlend, Blend.SourceAlpha );
			this.Device.SetRenderState<Blend>( RenderState.DestinationBlend, Blend.InverseSourceAlpha );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaOperation, TextureOperation.Modulate );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaArg1, 2 );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaArg2, 1 );
			
			if( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnUnmanagedリソースの作成();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnUnmanagedリソースの作成();
				Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
			}
		}
		protected override void UnloadContent()
		{
			if( this.listトップレベルActivities != null )
			{
				foreach( CActivity activity in this.listトップレベルActivities )
					activity.OnUnmanagedリソースの解放();
			}

			foreach( STPlugin st in this.listプラグイン )
			{
				Directory.SetCurrentDirectory( st.strプラグインフォルダ );
				st.plugin.OnUnmanagedリソースの解放();
				Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
			}
		}
		protected override void OnExiting( EventArgs e )
		{
			this.t終了処理();
			base.OnExiting( e );
		}
		protected override void Update( GameTime gameTime )
		{
		}
		protected override void Draw( GameTime gameTime )
		{
			CScoreIni.C演奏記録 c演奏記録;
			CScoreIni.C演奏記録 c演奏記録2;
			CScoreIni.C演奏記録 c演奏記録3;
			CDTX.CChip[] chipArray;
			string str;

			Sound管理.t再生中の処理をする();

			if( Timer != null )
				Timer.t更新();

			if( Input管理 != null )
				Input管理.tポーリング( this.bApplicationActive, CDTXMania.ConfigIni.bバッファ入力を行う );

			if( FPS != null )
				FPS.tカウンタ更新();

			//if( Pad != null )					ポーリング時にクリアしたらダメ！曲の開始時に1回だけクリアする。(2010.9.11)
			//	Pad.st検知したデバイス.Clear();

			if( this.Device == null )
				return;

			this.Device.BeginScene();
			this.Device.Clear( ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1f, 0 );

			if( r現在のステージ != null )
			{
				this.n進行描画の戻り値 = ( r現在のステージ != null ) ? r現在のステージ.On進行描画() : 0;

				#region [ プラグインの進行描画 ]
				//---------------------
				foreach( STPlugin sp in this.listプラグイン )
				{
					Directory.SetCurrentDirectory( sp.strプラグインフォルダ );

					if( CDTXMania.act現在入力を占有中のプラグイン == null || CDTXMania.act現在入力を占有中のプラグイン == sp.plugin )
						sp.plugin.On進行描画( CDTXMania.Pad, CDTXMania.Input管理.Keyboard );
					else
						sp.plugin.On進行描画( null, null );

					Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
				}
				//---------------------
				#endregion

				CScoreIni scoreIni = null;

				switch( r現在のステージ.eステージID )
				{
					case CStage.Eステージ.何もしない:
						break;

					case CStage.Eステージ.起動:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							if( !bコンパクトモード )
							{
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;
							}
							else
							{
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

							}
							foreach( STPlugin pg in this.listプラグイン )
							{
								Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.タイトル:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case 1:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								//-----------------------------
								#endregion
								break;

							case 2:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ オプション" );
								stageオプション.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageオプション;
								//-----------------------------
								#endregion
								break;

							case 3:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;
								//-----------------------------
								#endregion
								break;

							case 4:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 終了" );
								stage終了.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage終了;
								//-----------------------------
								#endregion
								break;
						}

						foreach( STPlugin pg in this.listプラグイン )
						{
							Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
							pg.plugin.Onステージ変更();
							Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
						}

						this.tガベージコレクションを実行する();
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.オプション:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							switch( r直前のステージ.eステージID )
							{
								case CStage.Eステージ.タイトル:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ タイトル" );
									stageタイトル.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stageタイトル;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion

								case CStage.Eステージ.選曲:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.コンフィグ:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							switch( r直前のステージ.eステージID )
							{
								case CStage.Eステージ.タイトル:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ タイトル" );
									stageタイトル.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stageタイトル;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion

								case CStage.Eステージ.選曲:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.選曲:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case 1:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case 2:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case 3:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ オプション" );
								stageオプション.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageオプション;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case 4:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.曲読み込み:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							CDTXMania.Pad.st検知したデバイス.Clear();	// 入力デバイスフラグクリア(2010.9.11)

							r現在のステージ.On非活性化();
							if( !ConfigIni.bギタレボモード )
							{
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 演奏（ドラム画面）" );
#if DAMAGELEVELTUNING
for (int i = 0; i < 5; i++)
{
	for (int j = 0; j < 2; j++)
	{
		stage演奏ドラム画面.gaugeDelta[i, j] = ConfigIni.fGaugeFactor[i, j];
	}
}
for (int i = 0; i < 3; i++) {
	stage演奏ドラム画面.damageLevelFactor[i] = ConfigIni.fDamageLevelFactor[i];
}		
#endif
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage演奏ドラム画面;
							}
							else
							{
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 演奏（ギター画面）" );
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage演奏ギター画面;
							}

							foreach( STPlugin pg in this.listプラグイン )
							{
								Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.演奏:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case 0:
								break;

							case 1:
								#region [ 演奏キャンセル ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Play canceled" );

								#region [ プラグイン On演奏キャンセル() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏キャンセル( scoreIni );
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								if( bコンパクトモード )
								{
									base.Window.Close();
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
								break;
							//-----------------------------
								#endregion

							case 2:
								#region [ 演奏失敗(StageFailed) ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Stage failed" );

								#region [ プラグイン On演奏失敗() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏失敗( scoreIni );
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								if( bコンパクトモード )
								{
									base.Window.Close();
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.listプラグイン )
									{
										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
								break;
							//-----------------------------
								#endregion

							case 3:
								#region [ 演奏クリア ]
								//-----------------------------
								chipArray = new CDTX.CChip[ 10 ];
								if( ConfigIni.bギタレボモード )
								{
									stage演奏ギター画面.t演奏結果を格納する( out c演奏記録, out c演奏記録2, out c演奏記録3 );
								}
								else
								{
									stage演奏ドラム画面.t演奏結果を格納する( out c演奏記録, out c演奏記録2, out c演奏記録3, out chipArray );
								}
								str = "Cleared";
								switch( CScoreIni.t総合ランク値を計算して返す( c演奏記録, c演奏記録2, c演奏記録3 ) )
								{
									case 0:
										str = "Cleared (Rank:SS)";
										break;

									case 1:
										str = "Cleared (Rank:S)";
										break;

									case 2:
										str = "Cleared (Rank:A)";
										break;

									case 3:
										str = "Cleared (Rank:B)";
										break;

									case 4:
										str = "Cleared (Rank:C)";
										break;

									case 5:
										str = "Cleared (Rank:D)";
										break;

									case 6:
										str = "Cleared (Rank:E)";
										break;

									case 99:	// #23534 2010.10.28 yyagi add: 演奏チップが0個のとき
										str = "Cleared (No chips)";
										break;
								}

								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( str );

								#region [ プラグイン On演奏クリア() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.On演奏クリア( scoreIni );
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								r現在のステージ.On非活性化();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 結果" );
								stage結果.st演奏記録.Drums = c演奏記録;
								stage結果.st演奏記録.Guitar = c演奏記録2;
								stage結果.st演奏記録.Bass = c演奏記録3;
								stage結果.r空うちドラムチップ = chipArray;
								stage結果.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage結果;

								#region [ プラグイン Onステージ変更() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}
								//---------------------
								#endregion

								break;
							//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.結果:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							DTX.t全チップの再生一時停止();
							DTX.On非活性化();
							r現在のステージ.On非活性化();
							if( !bコンパクトモード )
							{
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;

								foreach( STPlugin pg in this.listプラグイン )
								{
									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
								}

								this.tガベージコレクションを実行する();
							}
							else
							{
								base.Window.Close();
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.終了:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							base.Exit();
						}
						//-----------------------------
						#endregion
						break;
				}
			}

			this.Device.EndScene();
			if( this.b次のタイミングで全画面・ウィンドウ切り替えを行う )
			{
				ConfigIni.b全画面モード = !ConfigIni.b全画面モード;
				app.t全画面・ウィンドウモード切り替え();
				this.b次のタイミングで全画面・ウィンドウ切り替えを行う = false;
			}
			if( this.b次のタイミングで垂直帰線同期切り替えを行う )
			{
				bool bIsMaximized = this.Window.IsMaximized;											// #23510 2010.11.3 yyagi: to backup current window mode before changing VSyncWait
				currentClientSize = this.Window.ClientSize;												// #23510 2010.11.3 yyagi: to backup current window size before changing VSyncWait
				DeviceSettings currentSettings = app.GraphicsDeviceManager.CurrentSettings;
				currentSettings.EnableVSync = ConfigIni.b垂直帰線待ちを行う;
				app.GraphicsDeviceManager.ChangeDevice( currentSettings );
				this.b次のタイミングで垂直帰線同期切り替えを行う = false;
				base.Window.ClientSize = new Size(currentClientSize.Width, currentClientSize.Height);	// #23510 2010.11.3 yyagi: to resume window size after changing VSyncWait
				if (bIsMaximized)
				{
					this.Window.WindowState = FormWindowState.Maximized;								// #23510 2010.11.3 yyagi: to resume window mode after changing VSyncWait
				}
			}
		}


		// その他

		#region [ テクスチャの生成・解放のためのヘルパー ]
		//-----------------
		public static CTexture tテクスチャの生成( string fileName )
		{
			return tテクスチャの生成( fileName, false );
		}
		public static CTexture tテクスチャの生成( string fileName, bool b黒を透過する )
		{
			if( app == null )
			{
				return null;
			}
			try
			{
				return new CTexture( app.Device, fileName, TextureFormat, b黒を透過する );
			}
			catch( CTextureCreateFailedException )
			{
				Trace.TraceError( "テクスチャの生成に失敗しました。({0})", new object[] { fileName } );
				return null;
			}
		}
		public static void tテクスチャの解放( ref CTexture tx )
		{
			if( tx != null )
			{
				tx.Dispose();
				tx = null;
			}
		}
		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private bool bマウスカーソル表示中 = true;
		private bool b終了処理完了済み;
		private static CDTX dtx;
		private List<CActivity> listトップレベルActivities;
		private int n進行描画の戻り値;

		private void t起動処理()
		{
			#region [ strEXEのあるフォルダを決定する ]
			//-----------------
// BEGIN #23629 2010.11.13 from: デバッグ時は Application.ExecutablePath が ($SolutionDir)/bin/x86/Debug/ などになり System/ の読み込みに失敗するので、カレントディレクトリを採用する。（プロジェクトのプロパティ→デバッグ→作業ディレクトリが有効になる）
#if DEBUG
			strEXEのあるフォルダ = Environment.CurrentDirectory + @"\";
#else
			strEXEのあるフォルダ = Path.GetDirectoryName( Application.ExecutablePath ) + @"\";	// #23629 2010.11.9 yyagi: set correct pathname where DTXManiaGR.exe is.
#endif
// END #23629 2010.11.13 from
			//-----------------
			#endregion

			#region [ Config.ini の読込み ]
			//---------------------
			ConfigIni = new CConfigIni();
			string path = strEXEのあるフォルダ + "Config.ini";
			if( File.Exists( path ) )
			{
				try
				{
					ConfigIni.t読み込み( path );
				}
				catch
				{
					ConfigIni = new CConfigIni();	// 存在してなければ新規生成
				}
			}
			//---------------------
			#endregion
			#region [ ログ出力開始 ]
			//---------------------
			Trace.AutoFlush = true;
			if( ConfigIni.bログ出力 )
			{
				Trace.Listeners.Add( new CTraceLogListener( new StreamWriter( "DTXManiaLog.txt", false, Encoding.GetEncoding( "shift-jis" ) ) ) );
			}
			Trace.WriteLine("");
			Trace.WriteLine( "DTXMania powered by YAMAHA Silent Session Drums" );
			Trace.WriteLine( string.Format( "Release: {0}", VERSION ) );
			Trace.WriteLine( "" );
			Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ アプリケーションの初期化" );
			Trace.TraceInformation( "OS Version: " + Environment.OSVersion );
			Trace.TraceInformation( "ProcessorCount: " + Environment.ProcessorCount.ToString() );
			Trace.TraceInformation( "CLR Version: " + Environment.Version.ToString() );
			//---------------------
			#endregion
			#region [ コンパクトモードスイッチの有無 ]
			//---------------------
			bコンパクトモード = false;
			strコンパクトモードファイル = "";
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if( ( commandLineArgs != null ) && ( commandLineArgs.Length > 1 ) )
			{
				bコンパクトモード = true;
				strコンパクトモードファイル = commandLineArgs[ 1 ];
				Trace.TraceInformation( "コンパクトモードで起動します。[{0}]", new object[] { strコンパクトモードファイル } );
			}
			//---------------------
			#endregion

			#region [ ウィンドウ初期化 ]
			//---------------------
			base.Window.Text = "DTXMania .NET style release " + VERSION;
			base.Window.ClientSize = new Size(ConfigIni.nウインドウwidth, ConfigIni.nウインドウheight);	// #34510 yyagi 2010.10.31 to change window size got from Config.ini
			if (!ConfigIni.bウィンドウモード)						// #23510 2010.11.02 yyagi: add; to recover window size in case bootup with fullscreen mode
			{
				currentClientSize = new Size(ConfigIni.nウインドウwidth, ConfigIni.nウインドウheight);
			}
			base.Window.MaximizeBox = true;							// #23510 2010.11.04 yyagi: to support maximizing window
			base.Window.FormBorderStyle = FormBorderStyle.Sizable;	// #23510 2010.10.27 yyagi: changed from FixedDialog to Sizable, to support window resize
			base.Window.ShowIcon = true;
			base.Window.Icon = Properties.Resources.dtx;
			base.Window.KeyDown += new KeyEventHandler( this.Window_KeyDown );
			base.Window.MouseDoubleClick += new MouseEventHandler(this.Window_MouseDoubleClick);	// #23510 2010.11.13 yyagi: to go fullscreen mode
			base.Window.ResizeEnd += new EventHandler(this.Window_ResizeEnd);						// #23510 2010.11.20 yyagi: to set resized window size in Config.ini
			base.Window.ApplicationActivated += new EventHandler(this.Window_ApplicationActivated);
			base.Window.ApplicationDeactivated += new EventHandler( this.Window_ApplicationDeactivated );
			//---------------------
			#endregion
			#region [ Direct3D9 デバイスの生成 ]
			//---------------------
			DeviceSettings settings = new DeviceSettings();
			settings.Windowed = ConfigIni.bウィンドウモード;
			settings.BackBufferWidth = 640;
			settings.BackBufferHeight = 480;
			settings.EnableVSync = ConfigIni.b垂直帰線待ちを行う;
			base.GraphicsDeviceManager.ChangeDevice( settings );
			base.IsFixedTimeStep = false;
			base.Window.ClientSize = new Size(ConfigIni.nウインドウwidth, ConfigIni.nウインドウheight);	// #23510 2010.10.31 yyagi: to recover window size. width and height are able to get from Config.ini.
			base.InactiveSleepTime = TimeSpan.FromMilliseconds((float)(ConfigIni.n非フォーカス時スリープms));	// #23568 2010.11.3 yyagi: to support valiable sleep value when !IsActive
																												// #23568 2010.11.4 ikanick changed ( 1 -> ConfigIni )
			//---------------------
			#endregion

			DTX = null;

			#region [ Skin の初期化 ]
			//---------------------
			Trace.TraceInformation( "スキンの初期化を行います。" );
			Trace.Indent();
			try
			{
				Skin = new CSkin();
				Trace.TraceInformation( "スキンの初期化を完了しました。" );
			}
			catch
			{
				Trace.TraceInformation( "スキンの初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Timer の初期化 ]
			//---------------------
			Trace.TraceInformation( "タイマの初期化を行います。" );
			Trace.Indent();
			try
			{
				Timer = new CTimer( CTimer.E種別.MultiMedia );
				Trace.TraceInformation( "タイマの初期化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ FPS カウンタの初期化 ]
			//---------------------
			Trace.TraceInformation( "FPSカウンタの初期化を行います。" );
			Trace.Indent();
			try
			{
				FPS = new CFPS();
				Trace.TraceInformation( "FPSカウンタを生成しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ act文字コンソールの初期化 ]
			//---------------------
			Trace.TraceInformation( "文字コンソールの初期化を行います。" );
			Trace.Indent();
			try
			{
				act文字コンソール = new C文字コンソール();
				Trace.TraceInformation( "文字コンソールを生成しました。" );
				act文字コンソール.On活性化();
				Trace.TraceInformation( "文字コンソールを活性化しました。" );
				Trace.TraceInformation( "文字コンソールの初期化を完了しました。" );
			}
			catch( Exception exception )
			{
				Trace.TraceError( exception.Message );
				Trace.TraceError( "文字コンソールの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Input管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "DirectInput, MIDI入力の初期化を行います。" );
			Trace.Indent();
			try
			{
				Input管理 = new CInput管理( base.Window.Handle );
				foreach( IInputDevice device in Input管理.list入力デバイス )
				{
					if( ( device.e入力デバイス種別 == E入力デバイス種別.Joystick ) && !ConfigIni.dicJoystick.ContainsValue( device.GUID ) )
					{
						int key = 0;
						while( ConfigIni.dicJoystick.ContainsKey( key ) )
						{
							key++;
						}
						ConfigIni.dicJoystick.Add( key, device.GUID );
					}
				}
				foreach( IInputDevice device2 in Input管理.list入力デバイス )
				{
					if( device2.e入力デバイス種別 == E入力デバイス種別.Joystick )
					{
						foreach( KeyValuePair<int, string> pair in ConfigIni.dicJoystick )
						{
							if( device2.GUID.Equals( pair.Value ) )
							{
								( (CInputJoystick) device2 ).SetID( pair.Key );
								break;
							}
						}
						continue;
					}
				}
				Trace.TraceInformation( "DirectInput の初期化を完了しました。" );
			}
			catch( Exception exception2 )
			{
				Trace.TraceError( exception2.Message );
				Trace.TraceError( "DirectInput, MIDI入力の初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Pad の初期化 ]
			//---------------------
			Trace.TraceInformation( "パッドの初期化を行います。" );
			Trace.Indent();
			try
			{
				Pad = new CPad( ConfigIni, Input管理 );
				Trace.TraceInformation( "パッドの初期化を完了しました。" );
			}
			catch( Exception exception3 )
			{
				Trace.TraceError( exception3.Message );
				Trace.TraceError( "パッドの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Sound管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "DirectSound の初期化を行います。" );
			Trace.Indent();
			try
			{
				Sound管理 = new CSound管理( base.Window.Handle );
				Trace.TraceInformation( "DirectSound の初期化を完了しました。" );
			}
			catch
			{
				Trace.TraceError( "DirectSound の初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Songs管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "曲リストの初期化を行います。" );
			Trace.Indent();
			try
			{
				Songs管理 = new CSongs管理();
				Trace.TraceInformation( "曲リストの初期化を完了しました。" );
			}
			catch( Exception exception4 )
			{
				Trace.TraceError( exception4.Message );
				Trace.TraceError( "曲リストの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ CAvi の初期化 ]
			//---------------------
			CAvi.t初期化();
			//---------------------
			#endregion
			#region [ Random の初期化 ]
			//---------------------
			Random = new Random( (int) Timer.nシステム時刻 );
			//---------------------
			#endregion
			#region [ ステージの初期化 ]
			//---------------------
			r現在のステージ = null;
			r直前のステージ = null;
			stage起動 = new CStage起動();
			stageタイトル = new CStageタイトル();
			stageオプション = new CStageオプション();
			stageコンフィグ = new CStageコンフィグ();
			stage選曲 = new CStage選曲();
			stage曲読み込み = new CStage曲読み込み();
			stage演奏ドラム画面 = new CStage演奏ドラム画面();
			stage演奏ギター画面 = new CStage演奏ギター画面();
			stage結果 = new CStage結果();
			stage終了 = new CStage終了();
			this.listトップレベルActivities = new List<CActivity>();
			this.listトップレベルActivities.Add( act文字コンソール );
			this.listトップレベルActivities.Add( stage起動 );
			this.listトップレベルActivities.Add( stageタイトル );
			this.listトップレベルActivities.Add( stageオプション );
			this.listトップレベルActivities.Add( stageコンフィグ );
			this.listトップレベルActivities.Add( stage選曲 );
			this.listトップレベルActivities.Add( stage曲読み込み );
			this.listトップレベルActivities.Add( stage演奏ドラム画面 );
			this.listトップレベルActivities.Add( stage演奏ギター画面 );
			this.listトップレベルActivities.Add( stage結果 );
			this.listトップレベルActivities.Add( stage終了 );
			//---------------------
			#endregion
			#region [ プラグインの検索と生成 ]
			//---------------------
			PluginHost = new CPluginHost();

			Trace.TraceInformation( "プラグインの検索と生成を行います。" );
			Trace.Indent();
			try
			{
				this.tプラグイン検索と生成();
				Trace.TraceInformation( "プラグインの検索と生成を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ プラグインの初期化 ]
			//---------------------
			if( this.listプラグイン != null && this.listプラグイン.Count > 0 )
			{
				Trace.TraceInformation( "プラグインの初期化を行います。" );
				Trace.Indent();
				try
				{
					foreach( STPlugin st in this.listプラグイン )
					{
						Directory.SetCurrentDirectory( st.strプラグインフォルダ );
						st.plugin.On初期化( this.PluginHost );
						st.plugin.OnManagedリソースの作成();
						st.plugin.OnUnmanagedリソースの作成();
						Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
					}
					Trace.TraceInformation( "すべてのプラグインの初期化を完了しました。" );
				}
				catch
				{
					Trace.TraceError( "プラグインのどれかの初期化に失敗しました。" );
					throw;
				}
				finally
				{
					Trace.Unindent();
				}
			}

			//---------------------
			#endregion

			Trace.TraceInformation( "アプリケーションの初期化を完了しました。" );

			#region [ 最初のステージの起動 ]
			//---------------------
			Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ 起動" );

			r現在のステージ = stage起動;
			r現在のステージ.On活性化();
			//---------------------
			#endregion
		}
		private void t終了処理()
		{
			if( !this.b終了処理完了済み )
			{
				Trace.TraceInformation( "----------------------" );
				Trace.TraceInformation( "■ アプリケーションの終了" );
				#region [ 現在のステージの終了処理 ]
				//---------------------
				if (r現在のステージ != null)
				{
					Trace.TraceInformation( "現在のステージを終了します。" );
					Trace.Indent();
					try
					{
						r現在のステージ.On非活性化();
						Trace.TraceInformation( "現在のステージの終了処理を完了しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ プラグインの終了処理 ]
				//---------------------
				if (this.listプラグイン != null && this.listプラグイン.Count > 0)
				{
					Trace.TraceInformation( "すべてのプラグインを終了します。" );
					Trace.Indent();
					try
					{
						foreach( STPlugin st in this.listプラグイン )
						{
							Directory.SetCurrentDirectory( st.strプラグインフォルダ );
							st.plugin.OnUnmanagedリソースの解放();
							st.plugin.OnManagedリソースの解放();
							st.plugin.On終了();
							Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
						}
						PluginHost = null;
						Trace.TraceInformation( "すべてのプラグインの終了処理を完了しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 曲リストの終了処理 ]
				//---------------------
				if (Songs管理 != null)
				{
					Trace.TraceInformation( "曲リストの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Songs管理 = null;
						Trace.TraceInformation( "曲リストの終了処理を完了しました。" );
					}
					catch( Exception exception )
					{
						Trace.TraceError( exception.Message );
						Trace.TraceError( "曲リストの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				CAvi.t終了();
				//---------------------
				#endregion
				#region [ スキンの終了処理 ]
				//---------------------
				if (Skin != null)
				{
					Trace.TraceInformation( "スキンの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Skin.Dispose();
						Skin = null;
						Trace.TraceInformation( "スキンの終了処理を完了しました。" );
					}
					catch( Exception exception2 )
					{
						Trace.TraceError( exception2.Message );
						Trace.TraceError( "スキンの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectSoundの終了処理 ]
				//---------------------
				if (Sound管理 != null)
				{
					Trace.TraceInformation( "DirectSound の終了処理を行います。" );
					Trace.Indent();
					try
					{
						Sound管理.Dispose();
						Sound管理 = null;
						Trace.TraceInformation( "DirectSound の終了処理を完了しました。" );
					}
					catch( Exception exception3 )
					{
						Trace.TraceError( exception3.Message );
						Trace.TraceError( "DirectSound の終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ パッドの終了処理 ]
				//---------------------
				if (Pad != null)
				{
					Trace.TraceInformation( "パッドの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Pad = null;
						Trace.TraceInformation( "パッドの終了処理を完了しました。" );
					}
					catch( Exception exception4 )
					{
						Trace.TraceError( exception4.Message );
						Trace.TraceError( "パッドの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectInput, MIDI入力の終了処理 ]
				//---------------------
				if (Input管理 != null)
				{
					Trace.TraceInformation( "DirectInput, MIDI入力の終了処理を行います。" );
					Trace.Indent();
					try
					{
						Input管理.Dispose();
						Input管理 = null;
						Trace.TraceInformation( "DirectInput, MIDI入力の終了処理を完了しました。" );
					}
					catch( Exception exception5 )
					{
						Trace.TraceError( exception5.Message );
						Trace.TraceError( "DirectInput, MIDI入力の終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 文字コンソールの終了処理 ]
				//---------------------
				if (act文字コンソール != null)
				{
					Trace.TraceInformation( "文字コンソールの終了処理を行います。" );
					Trace.Indent();
					try
					{
						act文字コンソール.On非活性化();
						act文字コンソール = null;
						Trace.TraceInformation( "文字コンソールの終了処理を完了しました。" );
					}
					catch( Exception exception6 )
					{
						Trace.TraceError( exception6.Message );
						Trace.TraceError( "文字コンソールの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ FPSカウンタの終了処理 ]
				//---------------------
				Trace.TraceInformation("FPSカウンタの終了処理を行います。");
				Trace.Indent();
				try
				{
					if( FPS != null )
					{
						FPS = null;
					}
					Trace.TraceInformation( "FPSカウンタの終了処理を完了しました。" );
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ タイマの終了処理 ]
				//---------------------
				Trace.TraceInformation("タイマの終了処理を行います。");
				Trace.Indent();
				try
				{
					if( Timer != null )
					{
						Timer.Dispose();
						Timer = null;
						Trace.TraceInformation( "タイマの終了処理を完了しました。" );
					}
					else
					{
						Trace.TraceInformation( "タイマは使用されていません。" );
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ Config.iniの出力 ]
				//---------------------
				Trace.TraceInformation("Config.ini を出力します。");
				string str = strEXEのあるフォルダ + "Config.ini";
				Trace.Indent();
				try
				{
					ConfigIni.t書き出し( str );
					Trace.TraceInformation( "保存しました。({0})", new object[] { str } );
				}
				catch( Exception e )
				{
					Trace.TraceError( e.Message );
					Trace.TraceError( "Config.ini の出力に失敗しました。({0})", new object[] { str } );
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				Trace.TraceInformation("アプリケーションの終了処理を完了しました。");


				this.b終了処理完了済み = true;
			}
		}
		private void Window_ApplicationActivated( object sender, EventArgs e )
		{
			this.bApplicationActive = true;
		}
		private void Window_ApplicationDeactivated( object sender, EventArgs e )
		{
			this.bApplicationActive = false;
		}
		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Menu )
			{
				e.SuppressKeyPress = true;
			}
			else if( ( e.KeyCode == Keys.Return ) && e.Alt )
			{
				if( ConfigIni != null )
				{
					ConfigIni.bウィンドウモード = !ConfigIni.bウィンドウモード;
					this.t全画面・ウィンドウモード切り替え();
				}
				e.SuppressKeyPress = true;
			}
		}
		private CScoreIni tScoreIniへBGMAdjustとHistoryとPlayCountを更新(string str新ヒストリ行)
		{
			bool flag;
			bool flag2;
			bool flag3;
			string str = DTX.strファイル名の絶対パス + ".score.ini";
			CScoreIni ini = new CScoreIni( str );
			if( !File.Exists( str ) )
			{
				ini.stファイル.Title = DTX.TITLE;
				ini.stファイル.Name = DTX.strファイル名;
				ini.stファイル.Hash = CScoreIni.tファイルのMD5を求めて返す( DTX.strファイル名の絶対パス );
				for( int i = 0; i < 6; i++ )
				{
					ini.stセクション[ i ].nPerfectになる範囲ms = nPerfect範囲ms;
					ini.stセクション[ i ].nGreatになる範囲ms = nGreat範囲ms;
					ini.stセクション[ i ].nGoodになる範囲ms = nGood範囲ms;
					ini.stセクション[ i ].nPoorになる範囲ms = nPoor範囲ms;
				}
			}
			ini.stファイル.BGMAdjust = DTX.nBGMAdjust;
			CScoreIni.t更新条件を取得する( out flag, out flag2, out flag3 );
			if( ( flag || flag2 ) || flag3 )
			{
				if( flag )
				{
					ini.stファイル.PlayCountDrums++;
				}
				if( flag2 )
				{
					ini.stファイル.PlayCountGuitar++;
				}
				if( flag3 )
				{
					ini.stファイル.PlayCountBass++;
				}
				ini.tヒストリを追加する( str新ヒストリ行 );
				if( !bコンパクトモード )
				{
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
					for( int j = 0; j < ini.stファイル.History.Length; j++ )
					{
						stage選曲.r現在選択中のスコア.譜面情報.演奏履歴[ j ] = ini.stファイル.History[ j ];
					}
				}
			}
			if( ConfigIni.bScoreIniを出力する )
			{
				ini.t書き出し( str );
			}

			return ini;
		}
		private void tガベージコレクションを実行する()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		private void tプラグイン検索と生成()
		{
			this.listプラグイン = new List<STPlugin>();

			string strIPluginActivityの名前 = typeof( IPluginActivity ).FullName;
			string strプラグインフォルダパス = strEXEのあるフォルダ + "Plugins\\";

			this.t指定フォルダ内でのプラグイン検索と生成( strプラグインフォルダパス, strIPluginActivityの名前 );

			if( this.listプラグイン.Count > 0 )
				Trace.TraceInformation( this.listプラグイン.Count + " 個のプラグインを読み込みました。" );
		}
		private void t指定フォルダ内でのプラグイン検索と生成( string strプラグインフォルダパス, string strプラグイン型名 )
		{
			// 指定されたパスが存在しないとエラー
			if( !Directory.Exists( strプラグインフォルダパス ) )
			{
				Trace.TraceWarning( "プラグインフォルダが存在しません。(" + strプラグインフォルダパス + ")" );
				return;
			}

			// (1) すべての *.dll について…
			string[] strDLLs = System.IO.Directory.GetFiles( strプラグインフォルダパス, "*.dll" );
			foreach( string dllName in strDLLs )
			{
				try
				{
					// (1-1) dll をアセンブリとして読み込む。
					System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom( dllName );

					// (1-2) アセンブリ内のすべての型について、プラグインとして有効か調べる
					foreach( Type t in asm.GetTypes() )
					{
						//  (1-3) ↓クラスであり↓Publicであり↓抽象クラスでなく↓IPlugin型のインスタンスが作れる　型を持っていれば有効
						if( t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface( strプラグイン型名 ) != null )
						{
							// (1-4) クラス名からインスタンスを作成する
							var st = new STPlugin() {
								plugin = (IPluginActivity) asm.CreateInstance( t.FullName ),
								strプラグインフォルダ = Path.GetDirectoryName( dllName ),
								strアセンブリ簡易名 = asm.GetName().Name,
								Version = asm.GetName().Version,
							};

							// (1-5) プラグインリストへ登録
							this.listプラグイン.Add( st );
							Trace.TraceInformation( "プラグイン {0} ({1}, {2}, {3}) を読み込みました。", t.FullName, Path.GetFileName( dllName ), st.strアセンブリ簡易名, st.Version.ToString() );
						}
					}
				}
				catch
				{
					Trace.TraceInformation( dllName + " からプラグインを生成することに失敗しました。スキップします。" );
				}
			}

			// (2) サブフォルダがあれば再帰する
			string[] strDirs = Directory.GetDirectories( strプラグインフォルダパス, "*" );
			foreach( string dir in strDirs )
				this.t指定フォルダ内でのプラグイン検索と生成( dir + "\\", strプラグイン型名 );
		}
		//-----------------
		private void Window_MouseDoubleClick( object sender, MouseEventArgs e)	// #23510 2010.11.13 yyagi: to go full screen mode
		{
			ConfigIni.bウィンドウモード = false;
			this.t全画面・ウィンドウモード切り替え();
		}
		private void Window_ResizeEnd(object sender, EventArgs e)				// #23510 2010.11.20 yyagi: to get resized window size
		{
			ConfigIni.nウインドウwidth = (ConfigIni.bウィンドウモード) ? base.Window.ClientSize.Width : currentClientSize.Width;	// #23510 2010.10.31 yyagi add
			ConfigIni.nウインドウheight = (ConfigIni.bウィンドウモード) ? base.Window.ClientSize.Height : currentClientSize.Height;
		}
		#endregion
	}
}
