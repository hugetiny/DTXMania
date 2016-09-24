#include "stdafx.h"
#include "CommandLineParser.h"

namespace FDK {
	namespace General {

CommandLineParser::CommandLineParser()
{
	this->pFirstItem = this->pLastItem = NULL;
	this->nアイテム数 = 0;
}
void CommandLineParser::Init()
{
	this->Term();
}
void CommandLineParser::Init( LPCTSTR str解析するコマンドライン文 )
{
	LPCTSTR p = str解析するコマンドライン文;
	while( *p != _T('\0') )
	{
		CmdLineItem *pi;

		SKIPSP( p );

		// (1) オプションの場合
		if( *p == _T('-') )
		{
			pi = new CmdLineItem();
			
			TCHAR buf[ _MAX_PATH ];

			int i = 0;
			while( i < _MAX_PATH - 1 && *p != _T(' ') && *p != _T('\t') && *p != _T('\0') )
				buf[i++] = *p++;
			buf[i] = _T('\0');
		
			pi->strItem = buf;
		}

		// (2) ダブルクォートされた文字列の場合
		else if( *p == _T('\"') )
		{
			pi = new CmdLineItem();

			TCHAR buf[ _MAX_PATH ];
			p++;
			int i = 0;
			while( i < _MAX_PATH - 1 && *p != _T('\0') && *p != _T('\"') )
				buf[i++] = *p++;
			buf[i] = _T('\0');

			pi->strItem = buf;

			if( *p == _T('\"') )
				p++;
		}

		// (3) その他の場合
		else
		{
			pi = new CmdLineItem();
			
			TCHAR buf[ _MAX_PATH ];
			int i = 0;
			while( i < _MAX_PATH - 1 && *p != _T('\0') && *p != _T(' ') && *p != _T('\t') && *p != _T('-') )
				buf[i++] = *p++;
			buf[i] = _T('\0');
			
			pi->strItem = buf;
		}

		// (1)～(3) のいずれの場合でもリストに接続
		pi->prev = pi->next = NULL;
		APPENDLIST( this->pFirstItem, this->pLastItem, pi );
		this->nアイテム数++;
	}
}
int  CommandLineParser::GetItemNum()
{
	return this->nアイテム数;
}
LPCTSTR CommandLineParser::Get( int index )
{
	if( index < 0 || index >= this->nアイテム数 )
		return NULL;

	CmdLineItem* pc = this->pFirstItem;
	while( pc != NULL && index > 0 ) {
		pc = pc->next;
		index --;
	}
	return pc->strItem.c_str();
}
void CommandLineParser::Term()
{
	CmdLineItem *pi = this->pFirstItem;
	while( pi != NULL )
	{
		CmdLineItem* next = pi->next;
		SAFE_DELETE( pi );
		pi = next;
	}
	this->pFirstItem = this->pLastItem = NULL;
	this->nアイテム数 = 0;
}

	}//General
}//FDK