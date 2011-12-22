// stdafx.h
// FDK と利用アプリの両方で不変的な（めったに修正されない）もの。
// FDK 内部のクラスは stdafx.h だけを include し、利用アプリは fdk10.h を include する。

#pragma once

#define WIN32_LEAN_AND_MEAN		// Windows ヘッダーから使用されていない部分を除外します。
#define _CRTDBG_MAP_ALLOC		// malloc と free のデバッグ版を使う（_DEBUG時のみ）

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <windowsx.h>
#include <malloc.h>
#include <mmsystem.h>
#include <mmreg.h>
#include <msacm.h>
#include <vfw.h>
#include <tchar.h>
#include <process.h>
#include <time.h>
#include <direct.h>			// for _tmkdir(), etc.
#include <winnls32.h>		// for WINNLSEnableIME()
#include <crtdbg.h>			// for _ASSERT(), malloc_dbg, free_dbg

#define	DIRECTINPUT_VERSION 0x0700

#include <ddraw.h>
#include <d3d.h>
#include <d3dx.h>
#include <dinput.h>
#include <dsound.h>

#ifndef bDEBUG
 #ifdef _DEBUG
  #define bDEBUG	true
 #else
  #define bDEBUG	false
 #endif
#endif

#pragma warning( disable:4312 )		// より大きい型への変換警告を抑制

#define APPENDLIST(f,l,c)	 {if(l==NULL)f=l=c;else{(l)->next=c;(c)->prev=l;l=c;}}
#define	REMOVELIST(f,l,c)	 {if((c)->prev==NULL)f=(c)->next;else(c)->prev->next=(c)->next; if((c)->next==NULL)l=(c)->prev;else(c)->next->prev=(c)->prev;}
#define SAFE_DELETE(p)       { if(p) { delete (p);       (p)=NULL; } }
#define SAFE_DELETE_ARRAY(p) { if(p) { delete[] (p);     (p)=NULL; } }
#define SAFE_RELEASE(p)      { if(p) { (p)->Release();   (p)=NULL; } }
#define	SAFE_FREE(p)		 { if(p) { free((void*)(p)); (p)=NULL; } }
#define ABS(a)				 (((a)<0)?-(a):(a))
#define	MIN(a,b)			 (((a)<(b))?(a):(b))
#define	MAX(a,b)			 (((a)>(b))?(a):(b))
#define SKIPSP(p)			{while(*p==_T(' ')||*p==_T('\t'))p++;}
#define SKIPDEC(p)			{while((*p>=_T('0')&&*p<=_T('9'))||*p==_T('-')||*p==_T('+'))p++;}
#define SKIPHEX(p)			{while((*p>=_T('0')&&*p<=_T('9'))||(*p>=_T('a')&&*p<=_T('f'))||(*p>=_T('A')&&*p<=_T('F'))||*p==_T('-')||*p==_T('+'))p++;}
#define SKIPZEX(p)			{while((*p>=_T('0')&&*p<=_T('9'))||(*p>=_T('a')&&*p<=_T('z'))||(*p>=_T('A')&&*p<=_T('Z'))||*p==_T('-')||*p==_T('+'))p++;}

#ifndef PI
#define PI			3.1415926535898
#endif

// STL 関係
#include <string>
using namespace std;
typedef basic_string<TCHAR> tstring;
#pragma warning( disable:4786 )		// STLの警告封じ

// キーリピート処理マクロ①
// m ... 状態変数(int; 0..2; 初期値 0)
// t ... タイマ値(double; 初期化不要)
// i ... キーが押されているなら true になる条件式
// f ... キー処理関数
#define KEYREPEAT(m,t,i,f) \
if(i){\
	switch(m){\
	case 0: f;m=1;t=g_Timer.Get();break;\
	case 1: if(g_Timer.Get()-t>200.0){f;t=g_Timer.Get();m=2;}break;\
	case 2: if(g_Timer.Get()-t>30.0){f;t=g_Timer.Get();}break;\
    }\
}else{\
	m=0;\
}

// キーリピート処理マクロ② CCounter対応版; ※本来のCCounterの使い方ではないので注意
// c ... CCtimer 変数
// i ... キーが押されているなら true になる条件式
// f ... キー処理関数
#define KEYREPEATCT(c,i,f) \
if(i){\
	switch(c.nValue){\
	case 0: f; c.nValue=1; c.dbTimer=g_Timer.Get();break;\
	case 1: if(g_Timer.Get()-c.dbTimer > 200.0){f; c.dbTimer=g_Timer.Get(); c.nValue=2;}break;\
	case 2: if(g_Timer.Get()-c.dbTimer > 30.0) {f; c.dbTimer=g_Timer.Get();}break;\
    }\
}else{\
	c.nValue=0;\
}

// アニメ進行マクロ① カウント変数ループタイプ（c = b→h, b→h, ...）
// c ... カウント変数(int；初期値 b）
// t ... タイマ変数(double)
// b ... カウントの最小値
// h ... カウントの最大値
// i ... インターバル(ms）
#define COUNTLOOP(c,t,b,h,i) \
if( t != INIT_TIME ) {\
	if( g_Timer.Get() < t ) t = g_Timer.Get();\
	while( g_Timer.Get() - t >= (double) i ) {\
		if( ++c > h ) c = b;\
		t += (double) i;\
	}\
}

// アニメ進行マクロ② カウント変数上限で停止タイプ（c = b→h,h,h,h,h,....）
#define COUNT(c,t,b,h,i) \
if( t != INIT_TIME ) {\
	if( g_Timer.Get() < t ) t = g_Timer.Get();\
	while( g_Timer.Get() - t >= (double) i ) {\
		if (++c > h) c = h;\
		t += (double) i;\
	}\
}

