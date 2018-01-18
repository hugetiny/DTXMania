﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FDK;


namespace DTXMania
{
    public class CCommandParse
    {
        public enum ECommandType
        {
            DTXMania,
            DTXV,
            DTX2WAV
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CCommandParse()
        {
        }


        /// <summary>
        /// DTXManiaGR.exe 引数の分析
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>DTXMania/DTXV/DTX2WAV どのモードで起動されたかを返す</returns>
        /// <remarks>DTXモードとして使う場合、内部でEnabled, nStartBar, Command, NeedReload, filename, last_path, last_timestampを設定する</remarks>
        public ECommandType ParseArguments(string arg, ref CDTXVmode cdtxv, ref CDTX2WAVmode cdtx2wav)
        {
            // -Vvvv,ppp,"filename"         サウンドファイルの再生 vvv=volume, ppp=pan
            // -S                           DTXV再生停止
            // -D(サウンドモード)(YかNが3文字続く) Viewerの設定
            //                              (サウンドモード) WE=WASAPI Exclusive, WS=WASAPI Shared, A1=ASIO(数値はデバイス番号), D=DSound
            //                              YYY, YNYなど  1文字目=GRmode, 2文字目=TmeStretch, 3文字目=VSyncWait
            // -Nxxx                        再生開始小節番号
            // -Etype,freq,bitrate,"filename"    DTX2WAVとして使用 type="WAV"or"MP3"or"OGG", freq=48000など, bitrate=192 (kHzなど)

            ECommandType ret = ECommandType.DTXMania;
            bool analyzing = true;
            cdtxv.nStartBar = 0;

            if (arg != null)
            {
                while (analyzing)
                {
                    if (arg == "")
                    {
                        analyzing = false;
                    }
                    #region [ DTXVmode ]
                    else if (arg.StartsWith("-V", StringComparison.OrdinalIgnoreCase))    // サウンド再生
                    {
                        // -Vvvv,ppp,"filename"の形式。 vvv=volume, ppp=pan.
                        cdtxv.Enabled = true;
                        cdtxv.Command = CDTXVmode.ECommand.Preview;
                        cdtxv.Refreshed = true;
                        ret = ECommandType.DTXV;
                        arg = arg.Substring(2);

                        int pVol = arg.IndexOf(',');                  //Trace.TraceInformation( "pVol=" + pVol );
                        string strVol = arg.Substring(0, pVol);           //Trace.TraceInformation( "strVol=" + strVol );
                        cdtxv.previewVolume = Convert.ToInt32(strVol);         //Trace.TraceInformation( "previewVolume=" + previewVolume );
                        int pPan = arg.IndexOf(',', pVol + 1);            //Trace.TraceInformation( "pPan=" + pPan );
                        string strPan = arg.Substring(pVol + 1, pPan - pVol - 1);   //Trace.TraceInformation( "strPan=" + strPan );
                        cdtxv.previewPan = Convert.ToInt32(strPan);          //Trace.TraceInformation( "previewPan=" + previewPan );

                        arg = arg.Substring(pPan + 1);
                        arg = arg.Trim(new char[] { '\"' });
                        cdtxv.previewFilename = arg;
                        analyzing = false;
                    }
                    // -S  -Nxxx  filename
                    else if (arg.StartsWith("-S", StringComparison.OrdinalIgnoreCase))    // DTXV再生停止
                    {
                        cdtxv.Enabled = true;
                        cdtxv.Command = CDTXVmode.ECommand.Stop;
                        cdtxv.Refreshed = true;
                        ret = ECommandType.DTXV;
                        arg = arg.Substring(2);
                    }
                    else if (arg.StartsWith("-D", StringComparison.OrdinalIgnoreCase))
                    {
                        // -DWE, -DWS, -DA1など
                        arg = arg.Substring(2); // -D を削除
                        switch (arg[0])
                        {
                            #region [ DirectSound ]
                            case 'D':
                                if (cdtxv.soundDeviceType != ESoundDeviceType.DirectSound)
                                {
                                    cdtxv.ChangedSoundDevice = true;
                                    cdtxv.soundDeviceType = ESoundDeviceType.DirectSound;
                                }
                                else
                                {
                                    cdtxv.ChangedSoundDevice = false;
                                }
                                arg = arg.Substring(1);
                                break;
                            #endregion
                            #region [ WASAPI(Exclusive/Shared) ]
                            case 'W':
                                {
                                    ESoundDeviceType new_sounddevicetype;
                                    arg = arg.Substring(1);
                                    char c = arg[0];
                                    //arg = arg.Substring(1);

                                    switch (c)
                                    {
                                        case 'E':
                                            new_sounddevicetype = ESoundDeviceType.ExclusiveWASAPI;
                                            break;
                                        case 'S':
                                            new_sounddevicetype = ESoundDeviceType.SharedWASAPI;
                                            break;
                                        default:
                                            new_sounddevicetype = ESoundDeviceType.Unknown;
                                            break;
                                    }
                                    if (cdtxv.soundDeviceType != new_sounddevicetype)
                                    {
                                        cdtxv.ChangedSoundDevice = true;
                                        cdtxv.soundDeviceType = new_sounddevicetype;
                                    }
                                    else
                                    {
                                        cdtxv.ChangedSoundDevice = false;
                                    }
                                }
                                arg = arg.Substring(1);
                                break;
                            #endregion
                            #region [ ASIO ]
                            case 'A':
                                if (cdtxv.soundDeviceType != ESoundDeviceType.ASIO)
                                {
                                    cdtxv.ChangedSoundDevice = true;
                                    cdtxv.soundDeviceType = ESoundDeviceType.ASIO;
                                }
                                else
                                {
                                    cdtxv.ChangedSoundDevice = false;
                                }
                                arg = arg.Substring(1);

                                int nAsioDev = 0, p = 0;
                                while (true)
                                {
                                    char c = arg[0];
                                    if ('0' <= c && c <= '9')
                                    {
                                        nAsioDev *= 10;
                                        nAsioDev += c - '0';
                                        p++;
                                        arg = arg.Substring(1);
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (cdtxv.nASIOdevice != nAsioDev)
                                {
                                    cdtxv.ChangedSoundDevice = true;
                                    cdtxv.nASIOdevice = nAsioDev;
                                }
                                break;
                                #endregion
                        }
                        #region [ GRmode, TimeStretch, VSyncWait ]
                        {
                            // Reload判定は、-Nのところで行う
                            cdtxv.GRmode = (arg[0] == 'Y');
                            cdtxv.TimeStretch = (arg[1] == 'Y');
                            cdtxv.VSyncWait = (arg[2] == 'Y');

                            arg = arg.Substring(3);
                        }
                        #endregion
                    }
                    else if (arg.StartsWith("-N", StringComparison.OrdinalIgnoreCase))
                    {
                        cdtxv.Enabled = true;
                        cdtxv.Command = CDTXVmode.ECommand.Play;
                        ret = ECommandType.DTXV;

                        arg = arg.Substring(2);         // "-N"を除去
                        string[] p = arg.Split(new char[] { ' ' });
                        cdtxv.nStartBar = int.Parse(p[0]);     // 再生開始小節
                        if (cdtxv.nStartBar < 0)
                        {
                            cdtxv.nStartBar = -1;
                        }

                        int startIndex = arg.IndexOf(' ');
                        string filename = arg.Substring(startIndex + 1);  // 再生ファイル名(フルパス) これで引数が終わっていることを想定
                        try
                        {
                            filename = filename.Trim(new char[] { '\"' });
                            cdtxv.bIsNeedReloadDTX(filename);
                        }
                        catch // 指定ファイルが存在しない
                        {
                        }
                        arg = "";
                        analyzing = false;
                    }
                    #endregion
                    #region [ DTX2WAV mode]
                    //
                    #endregion
                    else
                    {
                        analyzing = false;
                    }
                }
            }
            //string[] s = { "Stop", "Play", "Preview" };
            //Trace.TraceInformation( "Command: " + s[ (int) this.Command ] );
            return ret;
        }

    }
}
