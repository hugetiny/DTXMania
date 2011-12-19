#include "stdafx.h"
#include "CMIDIOutput.h"

namespace FDK {
	namespace MIDI {

bool CMIDIOutput::Init()
{
	// 使用中なら終了処理を先に行う
	if( this->bInit )
		Term();

	// 内部状態の初期化
	this->bInit = false;
	this->nDevices = 0;
	this->hMidiOut = NULL;

	// デバイス数の取得
	if( ( this->nDevices = midiOutGetNumDevs() ) == 0 )
	{
		this->bInit = true;
		return true;	// MIDI出力デバイスは１つも検出できなかった
	}

	// MIDI_MAPPER デバイスのオープン
	MMRESULT mmr;
	if( ( mmr = ::midiOutOpen( &this->hMidiOut, MIDI_MAPPER, (DWORD)NULL, (DWORD)NULL, CALLBACK_NULL ) ) != MMSYSERR_NOERROR )
		return true;	// デフォルトのMIDI出力デバイスのオープンに失敗

	// デバイス情報の取得
	MIDIOUTCAPS mocaps;
	::midiOutGetDevCaps( MIDI_MAPPER, &mocaps, sizeof(mocaps) );

	// 完了
	this->bInit = true;
	return true;
}

void CMIDIOutput::Term()
{
	if (! this->bInit) 
		return;	// 初期化されてない

	if( this->hMidiOut )
	{
		::midiOutClose( this->hMidiOut );
		this->hMidiOut = NULL;
	}
	this->bInit = false;
}

void CMIDIOutput::SendMsg( DWORD dwMsg )
{
	if( this->bInit && this->hMidiOut )
		::midiOutShortMsg( this->hMidiOut, dwMsg );
}

void CMIDIOutput::SendMsg( BYTE byState, BYTE byData1, BYTE byData2 )
{
	SendMsg( (DWORD)byState | (((DWORD)byData1) << 8) | ((DWORD)byData2) << 16 );
}

	}//MIDI
}//FDK