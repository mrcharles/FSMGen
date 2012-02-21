// FSMCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "fsm_stl.h"

#include "MyClass.h"

int _tmain(int argc, _TCHAR* argv[])
{
	MyClass c;
	//c.InitializeFSM();


	c.update();
	c.status();

	FSM::InterfaceParam param;
	if(c.FSM.testCommand(MyClass::MyNamedCommand, &param))
		c.FSM.execCommand(MyClass::MyNamedCommand, &param);


	c.update();
	c.status();
	
	return 0;
}

