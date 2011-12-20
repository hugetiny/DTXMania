#include "stdafx.h"
#include "CFileMM.h"

namespace FDK {
	namespace General {

CFileMM::CFileMM()
{
	this->hFile = NULL;
	this->hFileMapping = NULL;
	this->pData = NULL;
	Init();
}

CFileMM::~CFileMM()
{
	Term();
}

void CFileMM::Init()
{
	if( this->pData ) {
		UnmapViewOfFile( this->pData );
		this->pData = NULL;
	}
	if( this->hFileMapping )
	{
		CloseHandle( this->hFileMapping );
		this->hFileMapping = NULL;
	}
	if( this->hFile ) 
	{
		CloseHandle( this->hFile );
		this->hFile = NULL;
	}
	this->strFileName.clear();
	this->strExt[0]	= _T('\0');
	this->ftLastWriteTime.dwLowDateTime  = 0;
	this->ftLastWriteTime.dwHighDateTime = 0;
	this->dwFileSizeHigh = 0;
	this->dwFileSizeLow	 = 0;
}

void CFileMM::Term()
{
	Init();
}

bool CFileMM::Load( LPCTSTR filename )
{
	// 一応初期化
	Init();
	this->strFileName = filename;

	// 拡張子とファイル情報の取得
	CFileMM::GetExt( this->strFileName.c_str(), this->strExt );
	WIN32_FILE_ATTRIBUTE_DATA fileInfo;
	if( ! GetFileAttributesEx( this->strFileName.c_str(), GetFileExInfoStandard, &fileInfo))
		return false;
	this->ftLastWriteTime.dwLowDateTime  = fileInfo.ftLastWriteTime.dwLowDateTime;
	this->ftLastWriteTime.dwHighDateTime = fileInfo.ftLastWriteTime.dwHighDateTime;
	this->dwFileSizeHigh = fileInfo.nFileSizeHigh;
	this->dwFileSizeLow  = fileInfo.nFileSizeLow;

	// ファイルのオープン
	this->hFile = CreateFile( this->strFileName.c_str(), GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL ); 
	if( hFile == INVALID_HANDLE_VALUE )
		return false;

	// ファイルサイズを取得
	this->dwFileSizeLow = GetFileSize( hFile, &this->dwFileSizeHigh );

	// メモリマップドファイルのオープン
	this->hFileMapping = CreateFileMapping( hFile, NULL, PAGE_READONLY, this->dwFileSizeHigh, this->dwFileSizeLow, NULL );
	if( this->hFileMapping == NULL )
	{
		CloseHandle( this->hFile );
		this->hFile = NULL;
		return false;
	}

	// ファイル全体をマッピング
	this->pData = (LPBYTE) MapViewOfFile( this->hFileMapping, FILE_MAP_READ, 0, 0, 0 );
	if( this->pData == NULL )
	{
		CloseHandle( this->hFileMapping );
		CloseHandle( this->hFile );
		this->hFileMapping = this->hFile = NULL;
		return false;
	}
	return true;
}

void CFileMM::GetExt( LPCTSTR filename, LPTSTR ext )
{
	TCHAR strFullPath[ _MAX_PATH ];
	TCHAR strExt[ _MAX_EXT ];

	// ファイル名のフルパスを取得
	if( ! GetFullPathName( filename, _MAX_PATH, strFullPath, NULL ) )
	{
		ext[0] = _T('\0');	// 取得失敗
		return;
	}
	
	// フルパスを各要素に分割
	_tsplitpath_s( strFullPath, NULL, 0, NULL, 0, NULL, 0, strExt, _MAX_EXT );

	// 拡張子を取得
	TCHAR* p = &(strExt[0]);
	if( *p == _T('.') ) p++;
	lstrcpyn( ext, p, _MAX_EXT );
}

	}//General
}//FDK