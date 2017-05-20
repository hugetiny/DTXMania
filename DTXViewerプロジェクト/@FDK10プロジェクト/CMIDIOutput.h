#pragma once

namespace FDK {
	namespace MIDI {

class CMIDIOutput
{
public:
	bool Init();	// 初期化
	void Term();	// 終了処理
	
	// メッセージの送信①
	// 　dwMsg ... data2<<16|data1<<8|status
	void SendMsg( DWORD dwMsg );

	// Desc: メッセージの送信②
	void SendMsg( BYTE byState, BYTE byData1, BYTE byData2 );

	CMIDIOutput() {
		this->bInit=false;
	};

public:
	HMIDIOUT hMidiOut;

protected:
	bool	 bInit;			// 初期化されていれば true
	int		 nDevices;		// MIDI Out デバイス数
};

	}//MIDI
}//FDK

using namespace FDK::MIDI;
