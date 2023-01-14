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
        private List<int>         listBassDrumWAV = null;
        private List<STMIDIEvent> listMIDIEvent   = null;
        private EncodingInfo[]    encodingInfos   = null;
        private string            outFilename     = null;
        private int               DrumsChannel    = 10;

        /// <summary>
        /// mapping: lane number -> MIDI note
        /// </summary>
        private Dictionary<int, int>        dicLaneToNote;

        /// <summary>
        /// mapping: lane number -> MIDI event
        /// </summary>
        private Dictionary<int, EMIDIEvent> dicLaneToEvent;

        public Cメインフォーム formMainForm;        // This will be initialized by CMIDIExportManager

        /// <summary>
        /// mapping: lane name -> lane number
        /// </summary>
		internal enum ELane : int
		{
            BarLength    = 0x02,
            Bpm          = 0x03,
            BpmXX        = 0x08,

			HiHatClose   = 0x11,
			Snare        = 0x12,
			BassDrum     = 0x13,
			HighTom      = 0x14,
			LowTom       = 0x15,
            Cymbal       = 0x16,
            FloorTom     = 0x17,
            HiHatOpen    = 0x18,
            RideCymbal   = 0x19,
            LeftCymbal   = 0x1A,
            LeftPedal    = 0x1B,
            LeftBassDrum = 0x1C
        }

        /// <summary>
        /// mapping: MIDI Event name -> MIDI Event number
        /// </summary>
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
                { (int)ELane.HiHatClose,   42 },   // HiHat Close
                { (int)ELane.Snare,        38 },   // Snare
                { (int)ELane.BassDrum,     36 },   // Bass Drums
                { (int)ELane.HighTom,      50 },   // High Tom
                { (int)ELane.LowTom,       47 },   // Low Tom
                { (int)ELane.Cymbal,       49 },   // Cymbal
                { (int)ELane.FloorTom,     41 },   // Floor Tom
                { (int)ELane.HiHatOpen,    46 },   // HiHat Open
                { (int)ELane.RideCymbal,   51 },   // Ride Cymbal
                { (int)ELane.LeftCymbal,   57 },   // Left Cymbal
                { (int)ELane.LeftPedal,     0 },   // Left Pedal   // LPをBDとするかHiHatとするかは後で決定する
                { (int)ELane.LeftBassDrum, 36 }    // Left Pedal Bass Drums
            };

            dicLaneToEvent = new Dictionary<int, EMIDIEvent>()
            {
                { (int)ELane.BarLength, EMIDIEvent.Meta_TimeSignature },
                { (int)ELane.Bpm,       EMIDIEvent.Meta_Bpm },
                { (int)ELane.BpmXX,     EMIDIEvent.Meta_Bpm }
            };

        }

        private void button_export_Click(object sender, EventArgs e)
        {
            bool result;

            listMIDIEvent?.Clear();
            listMIDIEvent = null;
            listBassDrumWAV?.Clear();
            listBassDrumWAV = null;

            Cursor.Current = Cursors.WaitCursor;

			#region [ ComboBoxで設定した値を取得 ]
			CComboBoxObject cc = (CComboBoxObject) this.comboBox_encodingList.SelectedItem;
            int codePage = cc.Key;
            cc = (CComboBoxObject)this.comboBox_LPAssign.SelectedItem;
            int noteLP   = cc.Key;
            #endregion

            #region [ ComboBoxの設定を保存 ]
            this.formMainForm.appアプリ設定.LastMIDIExportEncodingCodePage = codePage;
            this.formMainForm.appアプリ設定.LastMIDIExportLPAssignIndex    = this.comboBox_LPAssign.SelectedIndex;
			#endregion

			result = tMIDIExportMain(codePage, noteLP);

            Cursor.Current = Cursors.Default;

            this.DialogResult = result ? DialogResult.OK : DialogResult.Abort;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();

            // FormのCancelButtonも設定
        }


        public bool tMIDIExportMain(int codePage, int noteLP)
        {
            bool result;

            listMIDIEvent = new List<STMIDIEvent>();
            listBassDrumWAV = new List<int>();

            tGenerateChipList(noteLP);
            result = tGenerateMIDIFile(codePage);

            listMIDIEvent?.Clear();
            listMIDIEvent = null;
            listBassDrumWAV?.Clear();
            listBassDrumWAV = null;

            return result;
        }

        /// <summary>
        /// 同一tickに複数のイベントがある場合の優先順位
        /// </summary>
        internal enum EPriority : int
        {
            Copyright     = 0,
            Title         = 1,
            TimeSignature = 2,
            Text          = 3,
            Bpm           = 4,
            Others        = 256,
            EndOfTrack    = 512
        }


        /// <summary>
        /// まず、譜面情報からチップのリストを生成する。(その際に、tick情報も生成する)
        /// </summary>
        private void tGenerateChipList(int noteLP)
		{
            #region [  ]
            ulong basetick = 0;

            #region [ BassDrumで使っているチップ番号を全てlist化する (後でLeftPedalをBassDrumと見做すかどうかの判定に用いる)]
            foreach (KeyValuePair<int, C小節> item in this.formMainForm.mgr譜面管理者.dic小節)
            {
                var cBAR = item.Value;
                foreach (var cChip in cBAR.listチップ)
                {
                    int lane = cChip.nチャンネル番号00toFF;
                    if (lane == (int)ELane.BassDrum || lane == (int)ELane.LeftBassDrum)
					{
                        int chipNo = cChip.n値_整数1to1295;
                        if (!listBassDrumWAV.Contains(chipNo))
						{
                            listBassDrumWAV.Add(chipNo);
						}
					}
                }
            }
			#endregion

			#region [ TITLE ]
			{
				string title = "";
                Debug.WriteLine($"txt:{formMainForm.textBox曲名.Text}, filename={formMainForm.strDTXファイル名}");
                if (formMainForm.textBox曲名.Text.Length > 0)
                {
                    title = formMainForm.textBox曲名.Text;
                }
                else if (formMainForm.strDTXファイル名 != "")
				{
                    title = formMainForm.strDTXファイル名;
				}

                if (title != "")
                {
                    var stMIDIEvent = new STMIDIEvent()
                    {
                        tick = 0,
                        eMIDIEvent = EMIDIEvent.Meta_Title,
                        ePriority = EPriority.Title,
                        text = title
                    };
                    listMIDIEvent.Add(stMIDIEvent);
                }
                Debug.WriteLine($"TITLE: {title}");
            }
            #endregion
            #region [ BPM ]
            {
                float _f = (float)(formMainForm.numericUpDownBPM.Value);
                if (_f > 0.0001)
                {
                    var stMIDIEvent = new STMIDIEvent()
                    {
                        tick = 0,
                        eMIDIEvent = EMIDIEvent.Meta_Bpm,
                        ePriority = EPriority.Bpm,
                        f = (float)_f
                    };
                    listMIDIEvent.Add(stMIDIEvent);
                    Debug.WriteLine($"BPM: {stMIDIEvent.f}");
                }
            }
            #endregion
            #region [ ARTIST ]
            if (formMainForm.textBox製作者.Text.Length > 0)
            {
                var stMIDIEvent = new STMIDIEvent()
                {
                    tick = 0,
                    eMIDIEvent = EMIDIEvent.Meta_Title,
                    ePriority = EPriority.Text,
                    text = "ARTIST:" + formMainForm.textBox製作者.Text
                };
                listMIDIEvent.Add(stMIDIEvent);
                Debug.WriteLine($"{stMIDIEvent.text}");

            }
            #endregion
            #region [ COMMENT ]
            if (formMainForm.textBoxコメント.Text.Length > 0)
            {
                var stMIDIEvent = new STMIDIEvent()
                {
                    tick = 0,
                    eMIDIEvent = EMIDIEvent.Meta_Text,
                    ePriority = EPriority.Text,
                    text = "" + formMainForm.textBoxコメント.Text
                };
                listMIDIEvent.Add(stMIDIEvent);
                Debug.WriteLine($"COMMENT: {stMIDIEvent.text}");

            }
            #endregion


            float lastBarLength = 0.0f;
            ulong EoTtick = 0;

            foreach (KeyValuePair<int, C小節> item in this.formMainForm.mgr譜面管理者.dic小節)
            {
                bool existChipInnerBar = false;

                //if (item.Value.listチップ.Count > 0)
                //{
                //    Debug.WriteLine("[{0}:BAR={1}, f小節長倍率={2}, n小節長倍率を考慮した現在の小節の高さgrid={3}]",
                //        item.Key, item.Value.n小節番号0to3599, item.Value.f小節長倍率, item.Value.n小節長倍率を考慮した現在の小節の高さgrid);
                //}


                var cBAR = item.Value;


                #region [ 前小節から小節長倍率が変更されていれば、小節頭で小節長倍率を設定 ]
                if (cBAR.f小節長倍率 != lastBarLength)
				{
					var stMIDIEvent = new STMIDIEvent()
					{
						tick = basetick,
						eMIDIEvent = EMIDIEvent.Meta_TimeSignature,
						ePriority = EPriority.TimeSignature,
						f = cBAR.f小節長倍率
					};
					listMIDIEvent.Add(stMIDIEvent);
                    lastBarLength = cBAR.f小節長倍率;
				}
				#endregion


				#region [ 小節内のドラムチップとBPMチップ、小節長変更のチップをlistに登録する ]
				foreach (var cChip in cBAR.listチップ)
                {
                    //Debug.WriteLine("channel={0}: grid={1}, value={2}",
                    //        cChip.nチャンネル番号00toFF.ToString("x2"), cChip.n位置grid, cChip.f値_浮動小数);
                    #region [ velocityの取得 (chipNoは後でLPのBD判定にも使う)]
                    int chipNo = cChip.n値_整数1to1295;
                    WAV_BMP_AVI.CWAV cwav = this.formMainForm.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する(chipNo);
                    int _velocity = cwav.n音量0to100;
                    //Debug.WriteLine("WAV={0}: volume={1}", chipNo, _velocity);
                    #endregion

                    EMIDIEvent _eMIDIEvent;
                    EPriority  _ePriority;
                    byte       _note;

                    int lane = cChip.nチャンネル番号00toFF;
                    bool bIsContainLaneInDicLaneToNote  = dicLaneToNote.ContainsKey(lane);
                    bool bIsContainLaneInDicLaneToEvent = dicLaneToEvent.ContainsKey(lane);

                    if (bIsContainLaneInDicLaneToNote || bIsContainLaneInDicLaneToEvent)
                    {
                        try
                        {
                            _eMIDIEvent = bIsContainLaneInDicLaneToEvent?
                                            dicLaneToEvent[lane] : EMIDIEvent.NoteOn;
                            _ePriority  = bIsContainLaneInDicLaneToEvent && dicLaneToEvent[lane] == EMIDIEvent.Meta_Bpm ?
                                            EPriority.Bpm : EPriority.Others;

							if (lane == (int)ELane.LeftPedal)
                            {
                                #region [ LPは、チップ番号がBDで使われていたならBassDrumとして扱い、そうでなければプルダウンメニューでユーザーが指定したHiHatとして扱う。]
                                _note = listBassDrumWAV.Contains(chipNo) ?
                                            //(byte)(dicLaneToNote[(int)ELane.BassDrum]) : (byte)(dicLaneToNote[(int)ELane.HiHatClose]);
                                            (byte)(dicLaneToNote[(int)ELane.BassDrum]) : (byte)noteLP;
                                #endregion
                            }
                            else
                            {
                                _note = bIsContainLaneInDicLaneToNote ? (byte)(dicLaneToNote[lane]) : (byte)0;
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            // Dictionalyにない場合(＝ドラムチップでもBPMでもBarLengthでもない場合)は、
                            // そのチップを無視して次のチップの処理に進む (ここには来ないはずではあるが)

                            existChipInnerBar = true;
                            goto NextLoop;
                            //continue;
                        }

                        var _tick = basetick + (ulong)(cChip.n位置grid);
                        var stMIDIEvent = new STMIDIEvent()
                        {
                            tick        = _tick,
                            eMIDIEvent  = _eMIDIEvent,
                            note        = _note,
                            ePriority   = _ePriority,
                            f           =  cChip.f値_浮動小数,
                            velocity    = _velocity
                        };
                        listMIDIEvent.Add(stMIDIEvent);

    					#region [ Note Onに対して、1tick後にNote Offを入れる ]
    					if (_eMIDIEvent == EMIDIEvent.NoteOn)
	    				{
                            var stMIDIEvent_NoteOff = new STMIDIEvent()
                            {
                                tick = _tick + 1,
                                eMIDIEvent = EMIDIEvent.NoteOff,
                                note = _note,
                                ePriority = _ePriority,
                                f = cChip.f値_浮動小数,
                                velocity = 0
                            };
                            listMIDIEvent.Add(stMIDIEvent_NoteOff);
                        }
                        #endregion
                    }

                    NextLoop:
                    existChipInnerBar = true;
                }
                #endregion

                // 次の小節へ
                basetick += (ulong)(192.0 * cBAR.f小節長倍率);

                if (existChipInnerBar) EoTtick = basetick;  // 小節内に有意なチップがある限り、その小節は演奏データに含まれるものとする(→データ末尾を定義するtickを更新する)
            }
			#endregion

			#region [ list sort ]
			// note onのあとにnote offを入れたことで、tick順の通りにlistが並んでいないため、
			// listのソートを行う
			// ソートに際しては、tickを基準にしつつ、同じtickの時は copyright, time signature, tempoを優先する

			listMIDIEvent.Sort((a, b) => {
                int result1 = (int)(a.tick - b.tick);
                int result2 = (int)a.ePriority - (int)b.ePriority;
                return result1 != 0 ? result1 : result2;
            });
			#endregion

			#region [ 最終小節の最後に、End of Trackのイベントを追加する ]

			// EoTtickより、basetick+1(NoteOff分)の方が先行している場合があるので注意
			ulong lasttick_inList = 0;
            if (listMIDIEvent.Count > 0)
            {
                lasttick_inList = listMIDIEvent[listMIDIEvent.Count - 1].tick;
            }
            ulong lasttick = Math.Max(EoTtick - 1, lasttick_inList);
            var stMIDIEvent_EoT = new STMIDIEvent()
            {
                tick = lasttick,
                eMIDIEvent = EMIDIEvent.Meta_EndOfTrack,
                ePriority  = EPriority.EndOfTrack
            };
            listMIDIEvent.Add(stMIDIEvent_EoT);
            #endregion


            //デバッグ用
            //foreach (var m in listMIDIEvent)
			//{
            //    Debug.WriteLine($"tick={m.tick}, eType={m.eMIDIEvent.ToString()}, f={m.f}, note={m.note}, velo={m.velocity}, text={m.text}");
			//}
            //Debug.WriteLine("=====================================");
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
        /// Generate Smandard MIDI File from listMIDIEvent
        /// </summary>
        private bool tGenerateMIDIFile(int codePage)
        {
            bool result = true;
            try
            {
                using (var bw = new BinaryWriter(new FileStream(outFilename, FileMode.Create)))
                {
                    #region [ Header(MThd) ]
                    bw.Write(Encoding.ASCII.GetBytes("MThd"));
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x06 });    // size
                    bw.Write(new byte[] { 0x00, 0x00 });                // format 0
                    bw.Write(new byte[] { 0x00, 0x01 });                // tracks = 1
                    bw.Write(new byte[] { 0x00, 192 / 4 });             // division of quarter note
                    #endregion

                    #region [ Header(MTrk) ]
                    bw.Write(Encoding.ASCII.GetBytes("MTrk"));
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });    // size (at first, it is a dummy)
                    #endregion

                    ulong delta = 0, lastTick = 0;
                    float lastBPM = 120.0f;
                    bool UnderNonStandardTimeSignature = false;

                    #region [ data (MTrk) ]
                    foreach (var e in listMIDIEvent)
                    {
						#region [ put delta time ]
						delta = e.tick - lastTick;
                        lastTick = e.tick;
                        {
                            byte[] d = GetVarLen(delta);
                            bw.Write(d);

							#region [デバッグ表示]
							//Debug.Write($"delta: {delta}: size:{d.Length} ");
							//for (int i = 0; i < d.Length; i++)
							//{
							//	Debug.Write(d[i].ToString("x2") + "_");
							//}
							//Debug.Write(" ");
							#endregion
						}
						#endregion

						switch (e.eMIDIEvent)
						{
                            case EMIDIEvent.NoteOff:
								#region [ ノートオフ ]
								bw.Write(new byte[] { (byte)((int)EMIDIEvent.NoteOff + DrumsChannel - 1), e.note, 0 });
                                //Debug.WriteLine( $"0x80 {e.note} 00");
                                break;
                                #endregion

                            case EMIDIEvent.NoteOn:
								#region [ ノートオン ]
								bw.Write(new byte[] { (byte)((int)EMIDIEvent.NoteOn + DrumsChannel - 1), e.note, (byte)(e.velocity * 127 /100) } );
                                //Debug.WriteLine( $"0x90 {e.note} {e.velocity}");
                                break;
                                #endregion

                            case EMIDIEvent.Meta_Bpm:
								#region [ BPM変更 ]
								{
									// 拍子に変換できない小節長倍率が来た時はBPMを操作して対応するため、
									// 後で元のBPMに戻せるよう、今のBPMをバックアップしておく
									lastBPM = e.f;

                                    if (e.f > 0.0001)
                                    {
                                        Int32 bpm = (Int32)(60.0 * 1000000 / e.f);
                                        byte[] b = BitConverter.GetBytes(bpm);
                                        if (BitConverter.IsLittleEndian) Array.Reverse(b);
                                        bw.Write(new byte[] { 0xFF, 0x51, 0x03, b[1], b[2], b[3] });
                                        //Debug.WriteLine($"BPM {e.f}");
                                    }
                                }
                                break;
							#endregion

							case EMIDIEvent.Meta_Title:
								#region [ TITLE設定 ]
								{
									byte[] title = System.Text.Encoding.GetEncoding(codePage).GetBytes(e.text);
                                    bw.Write(new byte[] { 0xFF, 0x03 });
                                    bw.Write(GetVarLen((ulong)(title.Length)));
                                    bw.Write(title);
                                }
                                break;
                                #endregion

                            case EMIDIEvent.Meta_Text:
								#region [ テキスト ]
								{
									byte[] text = System.Text.Encoding.GetEncoding(codePage).GetBytes(e.text);
                                    bw.Write(new byte[] { 0xFF, 0x01 });
                                    bw.Write(GetVarLen((ulong)(text.Length)));
                                    bw.Write(text);
                                }
                                break;
                                #endregion

                            case EMIDIEvent.Meta_TimeSignature:
								#region [ 小節長変更を拍子またはBPMに変換して表現 ]
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
                                    // 2のべき乗を分母とする分数で表現できるか？ (ここでは、これを「1/128の倍数で表すことができるか?」で確認。)
                                    // できない場合はウエイト的な指定だと割り切る

                                    //Debug.WriteLine($"Tick={e.tick}");

                                    if (Math.Floor(nn) == nn)
                                    {
                                        // 2^nで割り切れた場合: time signatureのフォーマットに落とし込むことができる
                                        // まず約分してnnを算出
                                        while (Math.Floor(nn/2) == nn/2 && dd_ > 2)
										{
                                            nn  /= 2;
                                            dd_ /= 2;
										}

                                        if (dd_ == 2)   // x/2 になるなら、x/4になるよう補正
										{
                                            nn *= 2;
                                            dd_ *= 2;
										}

                                        //次にddを算出
                                        decimal dd = (decimal)Math.Log((double)dd_, 2);

                                        bw.Write(new byte[] { 0xFF, 0x58, 0x04, (byte)nn, (byte)dd, 0x18, 0x08 });

                                        //Debug.WriteLine(" ");
                                        //Debug.WriteLine($"TimeSignature割り切れる f={e.f}, nn={nn}, dd={dd}");

                                        // 直前の小節が2^nを分母とする分数で表現できない物だった場合、
                                        // ここまでBPM操作で辻褄を合わせていたはずなので、
                                        // 分数表現に戻したついでにBPMも元に戻す
                                        // (辻褄合わせ中に更にBPMを変更しているようなケースは、ここでは考えないこととする)
                                        if (UnderNonStandardTimeSignature)
                                        {
                                            Int32 bpm = (Int32)(60.0 * 1000000 / lastBPM);
                                            byte[] b = BitConverter.GetBytes(bpm);
                                            if (BitConverter.IsLittleEndian) Array.Reverse(b);
                                            bw.Write(new byte[] { 0, 0xFF, 0x51, 0x03, b[1], b[2], b[3] });
                                            //Debug.WriteLine($"BPM戻す {e.f}");
                                        }
                                        UnderNonStandardTimeSignature = false;
                                    }
                                    else
									{
                                        // 2^nで割り切れなかった場合: time signatureのフォーマットに落とし込むことができない
                                        // ただしこのパターンになるのはウエイトとして小節長倍率を用いる場合がほとんど
                                        // (音楽的な拍子の正しさを求めているのではない)
                                        // と見做して、BPMを調整して期待される待ち時間を再現する

                                        UnderNonStandardTimeSignature = true;

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
                                            //Debug.WriteLine($"TimeSignature割り切れない BPM {lastBPM}→{tempBPM}");
                                        }
                                    }
                                    //Debug.WriteLine(" ");
                                    //Debug.WriteLine($"TimeSignature delta={delta}, f={e.f}");
                                }
                                break;
					    		#endregion

							case EMIDIEvent.Meta_EndOfTrack:
                                #region [ Enf of Track, Write track size ]
                                {
                                    bw.Write(new byte[] { 0xFF, 0x2F, 0x00 });  // End of track
                                    long filesize = bw.BaseStream.Position;
                                    Int32 tracksize = (Int32)(filesize - 22);
                                    //Debug.WriteLine($"tick={e.tick}");
                                    //Debug.WriteLine($"filesize={filesize} ({filesize.ToString("x8")})");
                                    //Debug.WriteLine($"tracksize={tracksize} ({tracksize.ToString("x8")})");

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
                                    //bw.Flush();
                                }
                                #endregion
                                break;

							#region [ Non-used events ]
							case EMIDIEvent.Meta_Copyright:
                                break;
                            case EMIDIEvent.PolyphonicKeyPressure:
                            case EMIDIEvent.ControlChange:
                            case EMIDIEvent.ProgramChange:
                            case EMIDIEvent.ChannelPressure:
                            case EMIDIEvent.PitchBend:
                            case EMIDIEvent.SystemExclusive:
                                break;
                            default:
                                break;
                            #endregion
                        }
                    }
                    #endregion
                }
            }
			catch (System.UnauthorizedAccessException e)
			{
				MessageBox.Show(e.Message, "MIDI Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "MIDI Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
            }

            return result;
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

            if (BitConverter.IsLittleEndian) lb.Reverse();
            return lb.ToArray();
        }


        /// <summary>
        /// Initialize 2 ComboBox (Encoding, LPassign), 1 label (output filename)
        /// (Called from CMIDIExportManager())
        /// </summary>
        public void Initialize(string filename)
        {
            #region [ EncodingのComboBoxと、初期選択項目を設定する ]
            encodingInfos = Encoding.GetEncodings();

            var listEncodingInfo = new List<EncodingInfo>(encodingInfos);
            listEncodingInfo.Sort
                ((a, b) => a.DisplayName.CompareTo(b.DisplayName)
            );

            foreach ( EncodingInfo ei in listEncodingInfo)
			{
                var cComboBoxEncodingObject = new CComboBoxObject(ei.CodePage, ei.DisplayName);
                comboBox_encodingList.Items.Add(cComboBoxEncodingObject);
			}

			int lastCodePage = this.formMainForm.appアプリ設定.LastMIDIExportEncodingCodePage;
			int lastIndex = listEncodingInfo.FindIndex(
				item =>
				{
					if (item.CodePage == lastCodePage) return true;
					else return false;
				});
			comboBox_encodingList.SelectedIndex = lastIndex;

            listEncodingInfo.Clear();
            listEncodingInfo = null;
            encodingInfos = null;
            #endregion

            #region [ LPのComboBoxと、初期選択項目を設定する ]
            var arrayLP = new CComboBoxObject[]
            {
                new CComboBoxObject(44, "Hi-Hat Pedal" ),
                new CComboBoxObject(42, "Hi-Hat Close" ),
                new CComboBoxObject(46, "Hi-Hat Open" )
            };
            comboBox_LPAssign.Items.AddRange(arrayLP);
            comboBox_LPAssign.SelectedIndex = this.formMainForm.appアプリ設定.LastMIDIExportLPAssignIndex;
			#endregion

			#region [ 出力ファイル名を設定する ]
			this.outFilename                    = filename;
            this.label_outputFilename_text.Text = filename;
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        private class CComboBoxObject
        {
            public int Key;
            public string Value;

            public CComboBoxObject(int key, string value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString()
            {
                return Value;
            }
        }
	}


}
