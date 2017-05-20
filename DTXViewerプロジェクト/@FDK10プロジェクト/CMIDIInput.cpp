#include "stdafx.h"
#include "CMIDIInput.h"

namespace FDK {
	namespace MIDI {

// static メンバ
CRITICAL_SECTION CMIDIInput::cs[ MAX_MIDIIN ];
CMIDIInput::Info CMIDIInput::m_Info[ MAX_MIDIIN ];
int CMIDIInput::nDevices = 0;


// MIDI入力コールバック
void CALLBACK MidiInProc( HMIDIIN hMidiIn, UINT wMsg, DWORD dwInstance, DWORD dwParam1, DWORD dwParam2 )
{
	if( wMsg != MIM_DATA )
		return;

	int nStatus		=  dwParam1        & 0xf0;	// Status
	int nKey		= (dwParam1 >> 8)  & 0xff;	// data1
	int nVelocity	= (dwParam1 >> 16) & 0xff;	// data2

	int i;
	for( i = 0; i < CMIDIInput::nDevices; i++ )
	{
		if( hMidiIn == CMIDIInput::m_Info[i].hMidiIn )
			break;
	}
	if( i >= CMIDIInput::nDevices )
		return;

	EnterCriticalSection( &(CMIDIInput::cs[i]) );

	switch( nStatus )
	{
	case 0x90:	// Note On or Off
		if( nVelocity > 0 )
		{
			if( ++CMIDIInput::m_Info[i].byUpdate[0][nKey] == 255 )
				CMIDIInput::m_Info[i].byUpdate[0][nKey] = 0;
		}
		break;

	case 0x80:	// Note Off
		break;
	}
	
	LeaveCriticalSection( &(CMIDIInput::cs[i]) );
}

CMIDIInput::CMIDIInput()
{
	this->nDevices = 0;
}


bool CMIDIInput::Init()
{
	// 初期化
	this->nDevices = 0;
	for( int i = 0; i < MAX_MIDIIN; i++ )
	{
		m_Info[i].uID = 0;
		m_Info[i].strName[0] = _T('\0');
		m_Info[i].hMidiIn = NULL;
		ZeroMemory( m_Info[i].byUpdate[0], 256 );
		ZeroMemory( m_Info[i].byUpdate[1], 256 );
		ZeroMemory( m_Info[i].byUpdate[2], 256 );
		m_Info[i].nBuf = 0;
		InitializeCriticalSection( &(this->cs[i]) );
	}

	// デバイス数の取得
	this->nDevices = midiInGetNumDevs();
	if( this->nDevices < 0 )
		return true;		// MIDI 入力デバイスなし

	if( this->nDevices > MAX_MIDIIN )
		this->nDevices = MAX_MIDIIN;	// MIDI 入力デバイス数が最大数(%d)を超えた　→　これ以上のデバイスは無視

	// コールバック受付開始
	for( int i = 0; i < this->nDevices; i++ )
	{
		MMRESULT mr = ::midiInOpen( (LPHMIDIIN)&m_Info[i].hMidiIn, i, (DWORD_PTR)MidiInProc, 0, CALLBACK_FUNCTION );

		if( mr == MMSYSERR_NOERROR && m_Info[i].hMidiIn != NULL )
		{
			// 受付開始
			::midiInStart( m_Info[i].hMidiIn );
			::midiInGetID( m_Info[i].hMidiIn, &m_Info[i].uID );

			// デバイス名取得
			MIDIINCAPS micaps;
			::ZeroMemory( &micaps, sizeof(micaps) );
			::midiInGetDevCaps( m_Info[i].uID, &micaps, sizeof(micaps) );
			lstrcpyn( m_Info[i].strName, micaps.szPname, 32 );
		}
		else
			m_Info[i].hMidiIn = NULL;	// MIDI入力デバイスの受付開始に失敗 → このデバイスを無視
	}
	return true;
}

void CMIDIInput::Term()
{
	for( int i = 0; i < this->nDevices; i++ )
	{
		if( m_Info[i].hMidiIn != NULL )
		{
			midiInStop(  m_Info[i].hMidiIn );
			midiInReset( m_Info[i].hMidiIn );
			midiInClose( m_Info[i].hMidiIn );
			m_Info[i].hMidiIn = NULL;
		}
		DeleteCriticalSection( &(this->cs[i]) );
	}

	this->nDevices = 0;
}

void CMIDIInput::Poll()
{
	for( int i = 0; i < this->nDevices; i++ )
	{
		::EnterCriticalSection( &(this->cs[i]) );
		m_Info[i].nBuf = 1 - m_Info[i].nBuf;
		::CopyMemory( m_Info[i].byUpdate[ m_Info[i].nBuf+1 ], m_Info[i].byUpdate[0], 256 );
		::LeaveCriticalSection( &(this->cs[i]) );
	}
}

int  CMIDIInput::IsNotePushDown( UINT uID, int nKey )
{
	if( nKey < 0 || nKey > 255 )
		return 0;

	for( int i = 0; i < this->nDevices; i++ )
	{
		if( m_Info[i].uID == uID )
		{
			EnterCriticalSection( &(this->cs[i]) );
			int n = m_Info[i].byUpdate[m_Info[i].nBuf+1][nKey] - m_Info[i].byUpdate[1-m_Info[i].nBuf+1][nKey];
			LeaveCriticalSection( &(this->cs[i]) );
			if( n < 0 )
				n += 255;
			return n;
		}
	}
	return 0;
}

	}//MIDI
}//FDK