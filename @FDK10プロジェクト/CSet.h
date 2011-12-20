
#pragma once

namespace FDK {
	namespace General {

template <class T> class CSet
{
protected:
	// ƒZƒ‹ƒŠƒXƒg
	struct List {
		T	 *cell;
		List *prev, *next;
	} *first, *last, *cur;

public:
	CSet() {
		first = last = cur = NULL;
	}

	virtual ~CSet() {
		List *cell = first, *next;
		while(cell != NULL) {
			next = cell->next;
			delete cell;
			cell = next;
		}
		first = last = NULL;
	}

	void insert(T *cell) {
		List *nl = new List();
		nl->cell = cell;
		nl->prev = nl->next = NULL;
		if (last == NULL)
			first = last = cur = nl;
		else {
			last->next = nl;
			nl->prev = last;
			last = nl;
		}
	}

	void erase(T *cell) {
		for(List *l = first; l != NULL; l=l->next) {
			if (l->cell == cell) {
				if (l->prev == NULL)
					first = l->next;
				else
					l->prev->next = l->next;
				if (l->next == NULL)
					last = l->prev;
				else
					l->next->prev = l->prev;
				delete l;
				break;
			}
		}
	}

	T *begin() {
		if (first == NULL)
			return NULL;
		else
			return first->cell;
	}

	void resetList() {
		cur = first;
	}

	T* getNext() {
		if (cur != NULL) {
			T *ret = cur->cell;
			cur = cur->next;
			return ret;
		}
		return NULL;
	}
};
	}//General
}//FDK

using namespace FDK::General;
