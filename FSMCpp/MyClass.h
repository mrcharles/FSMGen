#ifndef MYCLASS_H
#define MYCLASS_H

#include "fsm_stl.h"
#include "commands.h"

class MyClass
{
	#include "MyClass.fsm.h"

public:


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

#endif