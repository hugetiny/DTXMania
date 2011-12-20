/*============================================================================================
	xadec.h
============================================================================================*/
#ifndef		_XADEC_H_
#define		_XADEC_H_

/*------------------------------------------------------------------------------------------*/
#include	<windows.h>
#include	<windowsx.h>
#include	<mmsystem.h>

/*------------------------------------------------------------------------------------------*/
#define		_XAID				(ULONG)(('1'<<24)|('D'<<16)|('W'<<8)|'K')

/*------------------------------------------------------------------------------------------*/
typedef struct _XASTREAMHEADER {
	UCHAR *pSrc;
	ULONG nSrcLen;
	ULONG nSrcUsed;
	UCHAR *pDst;
	ULONG nDstLen;
	ULONG nDstUsed;
} XASTREAMHEADER;

typedef struct _XAHEADER {
	ULONG id;
	ULONG nDataLen;
	ULONG nSamples;
	USHORT nSamplesPerSec;
	UCHAR nBits;
	UCHAR nChannels;
	ULONG nLoopPtr;
	SHORT befL[2];
	SHORT befR[2];
	UCHAR pad[4];
} XAHEADER;

typedef HANDLE HXASTREAM;

/*------------------------------------------------------------------------------------------*/
#ifdef		_XADEC_DLLEXP
#define		_XADEC_DLL		__declspec(dllexport)
#else
#define		_XADEC_DLL		__declspec(dllimport)
#endif

/*------------------------------------------------------------------------------------------*/
#ifdef		__cplusplus
extern "C" {
#endif
/*------------------------------------------------------------------------------------------*/
_XADEC_DLL HXASTREAM __cdecl xaDecodeOpen(XAHEADER *pxah, WAVEFORMATEX *pwfx);
_XADEC_DLL BOOL __cdecl xaDecodeClose(HXASTREAM hxas);
_XADEC_DLL BOOL __cdecl xaDecodeSize(HXASTREAM hxas, ULONG slen, ULONG *pdlen);
_XADEC_DLL BOOL __cdecl xaDecodeConvert(HXASTREAM hxas, XASTREAMHEADER *psh);

/*------------------------------------------------------------------------------------------*/
#ifdef		__cplusplus
}
#endif
/*------------------------------------------------------------------------------------------*/
#endif
/*==========================================================================================*/
