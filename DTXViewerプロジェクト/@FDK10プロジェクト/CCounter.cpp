#include "stdafx.h"
#include "CCounter.h"
#include "CTimer.h"

namespace FDK {
	namespace General {

CCounter::CCounter()
{
	this->pTimer = NULL;
	this->nValue = 0;
	this->dbTimer = INIT_TIME;
	this->n開始値 = 0;
	this->n終了値 = 0;
	this->db間隔  = 0.0;
}
void CCounter::Start( int n開始値, int n終了値, int n間隔ms, CTimer* pTimer )
{
	this->n開始値 = n開始値;
	this->n終了値 = n終了値;
	this->db間隔  = (double) n間隔ms;
	this->pTimer  = pTimer;

	_ASSERT( this->pTimer );

	this->dbTimer = this->pTimer->Get();
	this->nValue  = n開始値;
}
void CCounter::Step()
{
	if( this->dbTimer == INIT_TIME )
		return;

	_ASSERT( this->pTimer );

	double dbNow = this->pTimer->Get();
	
	if( dbNow < this->dbTimer )
		this->dbTimer = dbNow;

	while( dbNow - this->dbTimer >= this->db間隔 )
	{
		if( ++this->nValue > this->n終了値 )
			this->nValue = this->n終了値;
		this->dbTimer += db間隔;
	}
}
void CCounter::StepLoop()
{
	_ASSERT( this->pTimer );
	
	if( this->dbTimer == INIT_TIME )
		return;

	double dbNow = this->pTimer->Get();
	
	if( dbNow < this->dbTimer )
		this->dbTimer = dbNow;

	while( dbNow - this->dbTimer >= this->db間隔 )
	{
		if( ++this->nValue > this->n終了値 )
			this->nValue = this->n開始値;
		this->dbTimer += db間隔;
	}
}
void CCounter::Stop()
{
	this->dbTimer = INIT_TIME;
}
bool CCounter::b終了値に達した()
{
	return ( this->nValue == this->n終了値 ) ? true : false;
}
bool CCounter::b動作中()
{
	return ( this->dbTimer != INIT_TIME )? true : false;
}
	}//General
}//FDK
