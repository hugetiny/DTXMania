using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using System.Threading;
using FDK;

namespace DTXMania
{
    public class CDTX2WAVmode
    {
        public enum FormatType
        {
            WAV,
            OGG,
            MP3
        }

        /// <summary>
        /// DTX2WAVモードかどうか
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// プレビューサウンドの再生が発生した
        /// </summary>
        public FormatType Format
        {
            get;
            set;
        }

        public int freq;
        public int bitrate;
        public string outfilename;
        public string dtxfilename;

        public bool VSyncWait
        {
            get;
            set;
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CDTX2WAVmode()
        {
            this.Enabled = false;
            this.Format = FormatType.WAV;
            this.VSyncWait = false;         // とりあえず VSyncWait=OFF固定で考える
            this.outfilename = "";
            this.dtxfilename = "";
        }

        /// <summary>
        /// DTX2WAV関連の設定のみを更新して、Config.iniに書き出す
        /// </summary>
        public void tUpdateConfigIni()
        {
            /// DTX2WAV関連の設定のみを更新するために、
            /// 1. 現在のconfig.ini相当の情報を、別変数にコピーしておく
            /// 2. config.iniを読み込みなおす
            /// 3. 別変数のコピーから、Viewer関連の設定を、configに入れ込む
            /// 4. Config.iniを保存する

            CConfigXml ConfigIni_backup = (CConfigXml)CDTXMania.Instance.ConfigIni.Clone();     // #36612 2016.9.12 yyagi
            CDTXMania.Instance.LoadConfig();

            CDTXMania.Instance.ConfigIni.rcViewerWindow.W = ConfigIni_backup.rcWindow.W;
            CDTXMania.Instance.ConfigIni.rcViewerWindow.H = ConfigIni_backup.rcWindow.H;
            CDTXMania.Instance.ConfigIni.rcViewerWindow.X = ConfigIni_backup.rcWindow.X;
            CDTXMania.Instance.ConfigIni.rcViewerWindow.Y = ConfigIni_backup.rcWindow.Y;

            CDTXMania.Instance.SaveConfig();

            ConfigIni_backup = null;
        }
    }
}
