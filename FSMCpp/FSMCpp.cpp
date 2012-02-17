// FSMCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "fsm_stl.h"

class MyClass
{
public:
#include "MyClass_fsm.h"

public:
	MyClass()
	{
		InitializeFSM();
		FSM.activate();
	}



	void update()
	{
		FSM.update(0.1f);
	}

	void status()
	{
		FSM.status();
	}

private:
};



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

