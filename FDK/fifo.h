#pragma once

namespace FDK {
	namespace General {

class FIFO
{
public:
	void 	Clear();						// ‰Šú‰»
	void	Push( void *obj );				// Ši”[
	void*	Pop();							// æ‚èo‚µ
	void*	Peek() {return m_pFirst;}		// Ÿ‚Éæ‚èo‚·—v‘f‚ğ”`‚«Œ©
	
public:
	FIFO() {m_pFirst=m_pLast=NULL;}

protected:
	struct Cell {
		void*	obj;
		Cell	*prev, *next;
	};
	Cell*	m_pFirst;
	Cell*	m_pLast;
};

	}//General
}//FDK

using namespace FDK::General;
