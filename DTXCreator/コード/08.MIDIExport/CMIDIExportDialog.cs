using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DTXCreator.譜面;

namespace DTXCreator.MIDIExport
{
    public partial class CMIDIExportDialog : Form
    {
        //internal enum EType
        //{
        //    Title,
        //    BarLength,
        //    Bpm,
        //    //Copyright,

        //    HiHatClose,
        //    Snare,
        //    BassDrum,
        //    HighTom,
        //    LowTom,
        //    FloorTom,
        //    Cymbal,
        //    RideCymbal,
        //    HiHatOpen,
        //    LeftCymbal
        //}

        private List<STMIDIEvent> listMIDIEvent = null;

        private Dictionary<int, int>        dicLaneToNote;
        private Dictionary<int, EMIDIEvent> dicLaneToEvent;

        // This will be initialized by CMIDIExportManager
        public Cメインフォーム formメインフォーム;

        internal enum EMIDIEvent : int
        {
            NoteOff               = 0x80,
            NoteOn                = 0x90,
            PolyphonicKeyPressure = 0xA0,
            ControlChange         = 0xB0,
            ProgramChange         = 0xC0,
            ChannelPressure       = 0xD0,
            PitchBend             = 0xE0,
            SystemExclusive       = 0xF0,

            Meta_Text             = 0xFF01,
            Meta_Copyright        = 0xFF02,
            Meta_Title            = 0xFF03,
            Meta_EndOfTrack       = 0xFF2F,
            Meta_Bpm              = 0xFF51,
            Meta_TimeSignature    = 0xFF58
        }


        public CMIDIExportDialog()
        {
            InitializeComponent();

            dicLaneToNote = new Dictionary<int, int>()
            {
                { 0x11, 42 },   // HiHat Close
                { 0x12, 38 },   // Snare
                { 0x13, 35 },   // Bass Drums
                { 0x14, 50 },   // High Tom
                { 0x15, 47 },   // Low Tom
                { 0x16, 49 },   // Cymbal
                { 0x17, 41 },   // Floor Tom
                { 0x18, 46 },   // HiHat Open
                { 0x19, 51 },   // Ride Cymbal
                { 0x1A, 55 },    // Left Cymbal

                // 以下は入れておくと便利なので追加
                { 0x02, (int)EMIDIEvent.Meta_TimeSignature }, // BarLength
                { 0x03, (int)EMIDIEvent.Meta_Bpm },
                { 0x08, (int)EMIDIEvent.Meta_Bpm }

            };

            dicLaneToEvent = new Dictionary<int, EMIDIEvent>()
            {
                { 0x02, EMIDIEvent.Meta_TimeSignature }, // BarLength
                { 0x03, EMIDIEvent.Meta_Bpm },
                { 0x08, EMIDIEvent.Meta_Bpm }
            };

        }

        private void button_export_Click(object sender, EventArgs e)
        {
            listMIDIEvent?.Clear();
            listMIDIEvent = null;


            Cursor.Current = Cursors.WaitCursor;
            tMIDIExportMain();
            Cursor.Current = Cursors.Default;

            this.DialogResult = DialogResult.OK;
            this.Close();

            // FormのAcceptButtonも設定
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();

            // FormのCancelButtonも設定
        }


        public void tMIDIExportMain()
        {
            listMIDIEvent = new List<STMIDIEvent>();

            tGenerateChipList();
            tGenerateMIDIFile();

            listMIDIEvent?.Clear();
            listMIDIEvent = null;
        }

        internal enum EPriority : int
        {
            Copyright     = 0,
            Title         = 1,
            TimeSignature = 2,
            Text          = 3,
            Bpm           = 4,
            Others        = 255
        }


        /// <summary>
        /// まず、譜面情報からチップのリストを生成する。(その際に、tick情報も生成する)
        /// </summary>
        private void tGenerateChipList()
		{
            #region [  ]
            ulong basetick = 0;

			#region [ TITLE ]
            {
                string title = "";
                Debug.WriteLine($"txt:{formメインフォーム.textBox曲名.Text}, filename={formメインフォーム.strDTXファイル名}");
                if (formメインフォーム.textBox曲名.Text.Length > 0)
                {
                    title = formメインフォーム.textBox曲名.Text;
                }
                else if (formメインフォーム.strDTXファイル名 != "")
				{
                    title = formメインフォーム.strDTXファイル名;
				}

                if (title != "")
                {
                    var stMIDIEvent = new STMIDIEvent()
                    {
                        tick = 0,
                        eMIDIEvent = EMIDIEvent.Meta_Title,
                        ePriority = EPriority.Title,
                        text = formメインフォーム.textBox曲名.Text
                    };
                    listMIDIEvent.Add(stMIDIEvent);
                }
                Debug.WriteLine($"TITLE: {title}");
            }
            #endregion
            #region [ BPM ]
            {
                var stMIDIEvent = new STMIDIEvent()
                {
                    tick = 0,
                    eMIDIEvent = EMIDIEvent.Meta_Bpm,
                    ePriority = EPriority.Bpm,
                    f = (float)(formメインフォーム.numericUpDownBPM.Value)
                };
                listMIDIEvent.Add(stMIDIEvent);
                Debug.WriteLine($"BPM: {stMIDIEvent.f}");
            }
            #endregion
            #region [ ARTIST ]
            if (formメインフォーム.textBox製作者.Text.Length > 0)
            {
                var stMIDIEvent = new STMIDIEvent()
                {
                    tick = 0,
                    eMIDIEvent = EMIDIEvent.Meta_Title,
                    ePriority = EPriority.Text,
                    text = "ARTIST:" + formメインフォーム.textBox製作者.Text
                };
                listMIDIEvent.Add(stMIDIEvent);
                Debug.WriteLine($"ARTIST: {stMIDIEvent.text}");

            }
            #endregion
            #region [ COMMENT ]
            if (formメインフォーム.textBoxコメント.Text.Length > 0)
            {
                var stMIDIEvent = new STMIDIEvent()
                {
                    tick = 0,
                    eMIDIEvent = EMIDIEvent.Meta_Text,
                    ePriority = EPriority.Text,
                    text = "" + formメインフォーム.textBoxコメント.Text
                };
                listMIDIEvent.Add(stMIDIEvent);
                Debug.WriteLine($"COMMENT: {stMIDIEvent.text}");

            }
            #endregion


            float lastBarLength = 0.0f;
            ulong EoTtick = 0;

            foreach (KeyValuePair<int, C小節> item in this.formメインフォーム.mgr譜面管理者.dic小節)
            {
                //long tickInnerBar = 0;
                bool existChipInnerBar = false;

                //if (item.Value.listチップ.Count > 0)
                //{
                //    Debug.WriteLine("[{0}:BAR={1}, f小節長倍率={2}, n小節長倍率を考慮した現在の小節の高さgrid={3}]",
                //        item.Key, item.Value.n小節番号0to3599, item.Value.f小節長倍率, item.Value.n小節長倍率を考慮した現在の小節の高さgrid);
                //}

                var c小節 = item.Value;


                #region [ 前小節から小節長倍率が変更されていれば、小節頭で小節長倍率を設定 ]
                if (c小節.f小節長倍率 != lastBarLength)
				{
					var stMIDIEvent = new STMIDIEvent()
					{
						tick = basetick,
						eMIDIEvent = EMIDIEvent.Meta_TimeSignature,
						ePriority = EPriority.TimeSignature,
						f = c小節.f小節長倍率
					};
					listMIDIEvent.Add(stMIDIEvent);
                    lastBarLength = c小節.f小節長倍率;
				}
				#endregion


				#region [ 小節内のドラムチップとBPMチップ、小節長変更のチップをlistに登録する ]
				foreach (var cチップ in c小節.listチップ)
                {
                //Debug.WriteLine("channel={0}: grid={1}, value={2}",
                //        cチップ.nチャンネル番号00toFF.ToString("x2"), cチップ.n位置grid, cチップ.f値_浮動小数);
                    int chipNo = cチップ.n値_整数1to1295;
                    WAV_BMP_AVI.CWAV cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する(chipNo);
                    int _velocity = cwav.n音量0to100;
                //Debug.WriteLine("WAV={0}: volume={1}", chipNo, _velocity);

                    EMIDIEvent _eMIDIEvent;
                    byte _note;
                    EPriority _ePriority;

                    // くそダサいですが高速化のためいったんはこれで。(例外処理コストが非常に重いようです)
                    if (cチップ.nチャンネル番号00toFF != 0x02 &&
                        cチップ.nチャンネル番号00toFF != 0x03 &&
                        cチップ.nチャンネル番号00toFF != 0x08 &&
                        (cチップ.nチャンネル番号00toFF <= 0x11 || cチップ.nチャンネル番号00toFF >= 0x1A))
                        goto NextLoop;

                    try
                    {
                        _eMIDIEvent = cチップ.nチャンネル番号00toFF < 0x10 ?
                                          dicLaneToEvent[cチップ.nチャンネル番号00toFF] : EMIDIEvent.NoteOn;
                        _note       = (byte)(dicLaneToNote[cチップ.nチャンネル番号00toFF]);
                        _ePriority  = dicLaneToNote[cチップ.nチャンネル番号00toFF] == (int)(EMIDIEvent.Meta_Bpm) ?
                                          EPriority.Bpm : EPriority.Others;
                    }
                    catch (KeyNotFoundException)
					{
                        // Dictonalyにない場合(＝ドラムチップでもBPMでもBarLengthでもない場合)は、
                        // そのチップを無視して次のチップの処理に進む

                        existChipInnerBar = true;
                        goto NextLoop;
                        //continue;
					}
                    var _tick = basetick + (ulong)(cチップ.n位置grid);

                    var stMIDIEvent = new STMIDIEvent()
                    {
                        tick        = _tick,
                        eMIDIEvent  = _eMIDIEvent,
                        note        = _note,
                        ePriority   = _ePriority,
                        f           =  cチップ.f値_浮動小数,
                        velocity    = _velocity
                    };
                    listMIDIEvent.Add(stMIDIEvent);

					#region [ Note Onだったなら、1tick後にNote Offを入れる ]
					if (_eMIDIEvent == EMIDIEvent.NoteOn)
					{
                        var stMIDIEvent_NoteOff = new STMIDIEvent()
                        {
                            tick = _tick + 1,
                            eMIDIEvent = EMIDIEvent.NoteOff,
                            note = _note,
                            ePriority = _ePriority,
                            f = cチップ.f値_浮動小数,
                            velocity = 0
                        };
                        listMIDIEvent.Add(stMIDIEvent_NoteOff);

                    }
                    #endregion

                    NextLoop:
                    existChipInnerBar = true;
                }
                #endregion

                // 次の小節へ
                basetick += (ulong)(192.0 * c小節.f小節長倍率);

                if (existChipInnerBar) EoTtick = basetick;  // 小節内に有意なチップがある限り、その小節は演奏データに含まれるものとする(→データ末尾を定義するtickを更新する)
            }
            #endregion


            #region [ 最終小節の最後に、End of Trackのイベントを追加する ]
            var stMIDIEvent_EoT = new STMIDIEvent()
            {
                tick = EoTtick - 1,
                eMIDIEvent = EMIDIEvent.Meta_EndOfTrack,
                ePriority = EPriority.Others,
            };
            listMIDIEvent.Add(stMIDIEvent_EoT);
            #endregion


            //デバッグ用
            //foreach (var m in listMIDIEvent)
			//{
            //    Debug.WriteLine($"tick={m.tick}, eType={m.eMIDIEvent.ToString()}, f={m.f}, note={m.note}, velo={m.velocity}, text={m.text}");
			//}
            //Debug.WriteLine("=====================================");


            // note onのあとにnote offを入れたことで、tick順の通りにlistが並んでいないため、
            // listのソートを行う
            // ソートに際しては、tickを基準にしつつ、同じtickの時は copyright, time signature, tempoを優先する

            listMIDIEvent.Sort((a, b) => {
                int result1 = (int)(a.tick - b.tick);
                int result2 = (int)a.ePriority - (int)b.ePriority;
                return result1 != 0 ? result1 : result2;
            });
        }



        internal struct STMIDIEvent
		{
            internal ulong      tick;
            internal EPriority  ePriority;
            internal EMIDIEvent eMIDIEvent;
            internal byte       note;
            internal int        velocity;
            internal float      f;
            internal string     text;
		}


        /// <summary>
        /// listMIDIEvent から、SMFファイルを生成する
        /// </summary>
        private void tGenerateMIDIFile()
        {
            var fileName = Path.Combine(formメインフォーム.str作業フォルダ名, formメインフォーム.strDTXファイル名);
            fileName = Path.ChangeExtension(fileName, "dtx.mid");

            try
            {
                int DrumsChannel = 10;
                using (var bw = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    #region [ Header(MThd) ]
                    bw.Write(Encoding.GetEncoding("Shift_JIS").GetBytes("MThd"));
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x06 });    // size
                    bw.Write(new byte[] { 0x00, 0x00 });                // format 0
                    bw.Write(new byte[] { 0x00, 0x01 });                // tracks = 1
                    bw.Write(new byte[] { 0x00, 48 });                  // division of quarter note
                    #endregion

                    #region [ Header(MTrk) ]
                    bw.Write(Encoding.GetEncoding("Shift_JIS").GetBytes("MTrk"));
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });    // size (at first, it is a dummy)
                    #endregion

                    ulong delta = 0, lastTick = 0;
                    float lastBPM = 120.0f;
                    bool NonStandardTimeSignature = false;

                    #region [ data (MTrk) ]
                    foreach (var e in listMIDIEvent)
                    {
                        delta = e.tick - lastTick;
                        lastTick = e.tick;
                        {
                            byte[] d = GetVarLen(delta);
                            bw.Write(d);                                    // delta time

                            #region [デバッグ表示]
                            //Debug.Write($"delta: {delta}: size:{d.Length} ");
                            //for (int i = 0; i < d.Length; i++)
                            //{
                            //    Debug.Write(d[i].ToString("x2") + "_");
                            //}
                            //Debug.Write(" ");
                            #endregion
                        }

						switch (e.eMIDIEvent)
						{
                            case EMIDIEvent.NoteOff:
                                bw.Write(new byte[] { (byte)(0x80 + DrumsChannel - 1), e.note, 0 });
                                //Debug.WriteLine( $"0x80 {e.note} 00");
                                break;

                            case EMIDIEvent.NoteOn:
                                bw.Write(new byte[] { (byte)(0x90 + DrumsChannel - 1), e.note, (byte)(e.velocity * 127 /100) } );
                                //Debug.WriteLine( $"0x90 {e.note} {e.velocity}");
                                break;

                            case EMIDIEvent.Meta_Bpm:
                                {
                                    // 拍子に変換できない小節長倍率が来た時はBPMを操作して対応するため、
                                    // 後で元のBPMに戻せるよう、今のBPMをバックアップしておく
                                    lastBPM = e.f;                              

                                    Int32 bpm = (Int32)(60.0 * 1000000 / e.f);
                                    byte[] b = BitConverter.GetBytes(bpm);
                                    if (BitConverter.IsLittleEndian) Array.Reverse(b);
                                    bw.Write(new byte[] { 0xFF, 0x51, 0x03, b[1], b[2], b[3] });
                                    //Debug.WriteLine($"BPM {e.f}");
                                }
                                break;

                            case EMIDIEvent.Meta_Title:
                                {
                                    //byte[] title = System.Text.Encoding.Unicode.GetBytes(e.text);
                                    byte[] title = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(e.text);
                                    //byte[] title = System.Text.Encoding.GetEncoding("unicode").GetBytes(e.text);    // 互換性確保のため。shift_jisと選べるようにした方がよい
                                    bw.Write(new byte[] { 0xFF, 0x03 });
                                    bw.Write(GetVarLen((ulong)(title.Length)));
                                    bw.Write(title);
                                }
                                break;

                            case EMIDIEvent.Meta_Text:
                                {
                                    byte[] text = System.Text.Encoding.Unicode.GetBytes(e.text);    // 互換性確保のため。shift_jisと選べるようにした方がよい
                                    bw.Write(new byte[] { 0xFF, 0x01 });
                                    bw.Write(GetVarLen((ulong)(text.Length)));
                                    bw.Write(text);
                                }
                                break;

                            case EMIDIEvent.Meta_TimeSignature:
                                {
                                    decimal d = (decimal)(e.f);
                                    decimal dd_ = 128;

                                    //d = 0.1m;
                                    //d = 17 / 16m;
                                    //Debug.WriteLine(" ");
                                    //Debug.Write($"TimeSignature: f={d}, ");

                                    decimal bs = 1m / dd_;
                                    //Debug.Write($"TimeSignature: bs={bs}, ");
                                    decimal nn = d / bs;

                                    //Debug.Write($"TimeSignature: f={d}, 1/{dd_}={bs}, 倍率={nn}");
                                    // 2のべき乗を分母とする分数で表現できるか？
                                    // できない場合はウエイト的な指定だと割り切る


                                    if (Math.Floor(nn) == nn)
                                    {
                                        // 2^nで割り切れた場合: time signatureのフォーマットに落とし込むことができる
                                        // まずnnを算出
                                        while (Math.Floor(nn/2) == nn/2)
										{
                                            nn  /= 2;
                                            dd_ /= 2;
										}
                                        //次にddを算出
                                        decimal dd = (decimal)Math.Log((double)dd_, 2);
                                        // 単純に分子と分母を約分していくと4/4は1/1になるので、4/4に補正
                                        if (nn==1 && dd==0)
										{
                                            nn = 4;
                                            dd = 2;
										}
                                        bw.Write(new byte[] { 0xFF, 0x58, 0x04, (byte)nn, (byte)dd, 0x18, 0x08 });

                                        //Debug.WriteLine(" ");
                                        //Debug.WriteLine($"TimeSignature f={e.f}, nn={nn}, dd={dd}");

                                        // 直前の小節が2^nを分母とする分数で表現できない物だった場合、
                                        // ここまでBPM操作で辻褄を合わせていたはずなので、
                                        // 分数表現に戻したついでにBPMも元に戻す
                                        if (NonStandardTimeSignature)
                                        {
                                            Int32 bpm = (Int32)(60.0 * 1000000 / lastBPM);
                                            byte[] b = BitConverter.GetBytes(bpm);
                                            if (BitConverter.IsLittleEndian) Array.Reverse(b);
                                            bw.Write(new byte[] { 0, 0xFF, 0x51, 0x03, b[1], b[2], b[3] });
                                            Debug.WriteLine($"BPM {e.f}");
                                        }
                                        NonStandardTimeSignature = false;
                                    }
                                    else
									{
                                        // 2^nで割り切れなかった場合: time signatureのフォーマットに落とし込むことができない
                                        // ただしこのパターンになるのはウエイトとして小節長倍率を用いる場合がほとんど
                                        // (音楽的な拍子の正しさを求めているのではない)
                                        // と見做して、BPMを調整して期待される待ち時間を再現する

                                        NonStandardTimeSignature = true;

                                        // 拍子そのものは1/4と設定
                                        bw.Write(new byte[] { 0xFF, 0x58, 0x04, (byte)1, (byte)2, 0x18, 0x08 });

                                        {
                                            //新bpm = 旧bpm/barLength
                                            float tempBPM = lastBPM / (float)d;
                                            Int32 bpm = (Int32)(60.0 * 1000000 / tempBPM );
                                            byte[] b = BitConverter.GetBytes(bpm);
                                            if (BitConverter.IsLittleEndian) Array.Reverse(b);
                                            bw.Write(new byte[] { 0, 0xFF, 0x51, 0x03, b[1], b[2], b[3] });
                                            //Debug.WriteLine(" ");
                                            //Debug.WriteLine($"BPM {lastBPM}→{tempBPM}");
                                        }
                                    }
                                    //Debug.WriteLine(" ");
                                    //Debug.WriteLine($"TimeSignature {e.f}");
                                }
                                break;

                            case EMIDIEvent.Meta_EndOfTrack:
                                #region [ Enf of Track, Write track size ]
                                {
                                    //bw.Write(0x00);
                                    bw.Write(new byte[] { 0xFF, 0x2F, 0x00 });  // End of track
                                    long filesize = bw.BaseStream.Position;
                                    Int32 tracksize = (Int32)(filesize - 22);
                                    //Debug.WriteLine($"filesize={filesize}");
                                    //Debug.WriteLine($"tracksize={tracksize}");

                                    bw.BaseStream.Seek(18, SeekOrigin.Begin);

                                    byte[] b = BitConverter.GetBytes(tracksize);
                                    if (BitConverter.IsLittleEndian) Array.Reverse(b);

                                    //Debug.Write($"tracksize: {tracksize}: size:{b.Length} ");
                                    //for (int i = 0; i < b.Length; i++)
                                    //{
                                    //    Debug.Write(b[i].ToString("x2") + "_");
                                    //}
                                    //Debug.Write(" ");

                                    bw.Write(b);
                                }
                                #endregion
                                break;


                            case EMIDIEvent.Meta_Copyright:
                                break;
                            case EMIDIEvent.PolyphonicKeyPressure:
                            case EMIDIEvent.ControlChange:
                            case EMIDIEvent.ProgramChange:
                            case EMIDIEvent.ChannelPressure:
                            case EMIDIEvent.PitchBend:
                            case EMIDIEvent.SystemExclusive:
                                break;  // 未使用
                            default:
                                break;
                        }
                    }
                    #endregion
                }
            }
            catch (System.UnauthorizedAccessException e)
			{
                MessageBox.Show(e.Message, "MIDI Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "MIDI Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// get variable length data
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        private byte[] GetVarLen(ulong delta)
		{
            List<byte> lb = new List<byte>(4);
            do
            {
                lb.Add((byte)((delta & 0x7F) | 0x80));
                delta >>= 7;
            } while ( (delta & 0x7F) > 0);
            lb[0] &= 0x7F;

            if (lb.Count > 4)
			{
                MessageBox.Show("delta time becomes over 4 bytes.", "Deltatime error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                while (lb.Count > 4)
                {
                    lb.RemoveAt(4);
                }                
            }

            lb.Reverse();
            return lb.ToArray();
        }


	}
}
