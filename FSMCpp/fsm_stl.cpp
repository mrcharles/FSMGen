#include "stdafx.h"
#include "fsm_stl.h"
		
namespace FSM
{

//hook for errors
void FSMError(const std::string &text)
{
	printf(text.c_str());
}


}