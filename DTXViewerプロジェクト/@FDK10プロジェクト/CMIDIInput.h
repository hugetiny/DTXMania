#pragma once

namespace FDK {
	namespace MIDI {

#define MAX_MIDIIN		8			// 最大 MIDI In デバイス数

class CMIDIInput
{
public:
	bool Init();								// 初期化
	void Term();								// 終了処理

	void Poll();								// MIDI入力のノード状態ポーリング
	int  IsNotePushDown( UINT uID, int nKey );	// 指定ノードが何回押下されたかを返す
	int  GetDeviceNum() {return this->nDevices;}

	CMIDIInput();

public:
	static int nDevices;					// MIDI入力デバイス数
	static CRITICAL_SECTION cs[ MAX_MIDIIN ];		// MidiInProc() との同期用
	static struct Info	{
		UINT	uID;						// デバイス番号
		TCHAR	strName[MAXPNAMELEN];		// デバイス名
		HMIDIIN hMidiIn;					// デバイスハンドル
		BYTE	byUpdate[3][256];			// 0:MIDI側更新フラグ, 1:Poll側更新フラグ
		int		nBuf;						// フリップ用
	} m_Info[ MAX_MIDIIN ];

};

	}//MIDI
}//FDK

using namespace FDK::MIDI;
