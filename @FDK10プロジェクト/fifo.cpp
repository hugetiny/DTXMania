#include "stdafx.h"
#include "fifo.h"

namespace FDK {
	namespace General {

void FIFO::Clear()
{
	Cell* cell = m_pFirst;
	while( cell != NULL )
	{
		Cell *next = cell->next;
		delete cell;
		cell = next;
	}
	m_pFirst = m_pLast = NULL;
}

void FIFO::Push( void* obj )
{
	Cell* cell = new Cell();
	cell->obj = obj;
	cell->prev = cell->next = NULL;
	APPENDLIST( m_pFirst, m_pLast, cell );
}

void* FIFO::Pop()
{
	if( m_pFirst == NULL )
		return NULL;

	void* obj  = m_pFirst->obj;
	Cell* next = m_pFirst->next;

	delete m_pFirst;

	if( next != NULL )
	{
		next->prev = NULL;
		m_pFirst = next;
	}
	else
		m_pFirst = m_pLast = NULL;

	return obj;
}

	}//General
}//FDK