// FSMCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "fsm_stl.h"

class MyClass
{
	enum InterfaceCommands
	{
		MyNamedCommand = 0
	};

	//declaration
	//State TestA
	void onEnterTestA(){}
	void onExitTestA(){}
	void updateTestA(float dt){}
	FSM::InterfaceResult::Enum testTestAOnMyNamedCommand(){}
	void execTestAOnMyNamedCommand(){}

	//State SubstateAA
	void onEnterSubstateAA(){}
	void onExitSubstateAA(){}
	bool testSubstateAAToSubstateAB(){}
	void execSubstateAAToSubstateAB(){}
	FSM::InterfaceResult::Enum testSubstateAAToTestBOnMyNamedCommand(){}
	void execSubstateAAToTestBOnMyNamedCommand(){}

	//State SubstateAB
	void onEnterSubstateAB(){}
	void onExitSubstateAB(){}

	//State TestB
	void onEnterTestB(){}
	void onExitTestB(){}

	//State SubstateBA
	void onEnterSubstateBA(){}
	void onExitSubstateBA(){}

	//State SubstateBB
	void onEnterSubstateBB(){}
	void onExitSubstateBB(){}



	FSM::State<MyClass> TestA;
	FSM::State<MyClass> SubstateAA;
	FSM::State<MyClass> SubstateAB;
	FSM::State<MyClass> TestB;
	FSM::State<MyClass> SubstateBA;
	FSM::State<MyClass> SubstateBB;



	void InitializeFSM()
	{
		FSM_INIT_STATE_UPDATE(MyClass, TestA, true);
		FSM_INIT_STATE(MyClass, SubstateAA, true);
		FSM_INIT_STATE(MyClass, SubstateAB, false);
		FSM_INIT_STATE(MyClass, TestB, false);
		FSM_INIT_STATE(MyClass, SubstateBA, false);
		FSM_INIT_STATE(MyClass, SubstateBB, false);
		
	}
};



int _tmain(int argc, _TCHAR* argv[])
{
	MyClass c;
	c.InitializeFSM();
	
	return 0;
}

