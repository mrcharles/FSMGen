#include "stdafx.h"
#include "fsm_stl.h"
#include <assert.h>
		
namespace FSM
{

//hook for errors
void FSMError(const std::string &text)
{
	printf(text.c_str());
}

void FSMAssert(bool mustBeTrue, const std::string &error)
{
	if(!mustBeTrue) 
		printf(error.c_str());
	assert(mustBeTrue);
}


}