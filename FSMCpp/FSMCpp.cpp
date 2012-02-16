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
	FSM::StateMachine<MyClass> FSM;
	void onEnterFSM() {}
	void onExitFSM() {}
	

	//State TestA
	void onEnterTestA(){}
	void onExitTestA(){}
	void updateTestA(float dt){}
	FSM::InterfaceResult::Enum testTestAOnMyNamedCommand(FSM::InterfaceParam* param){ return FSM::InterfaceResult::Unhandled;}
	void execTestAOnMyNamedCommand(FSM::InterfaceParam* param){}

	//State SubstateAA
	void onEnterSubstateAA(){}
	void onExitSubstateAA(){}
	bool testSubstateAAToSubstateAB(){ return false;}
	void execSubstateAAToSubstateAB(){}
	FSM::InterfaceResult::Enum testSubstateABToTestBOnMyNamedCommand(FSM::InterfaceParam* param){return FSM::InterfaceResult::Unhandled;}
	void execSubstateABToTestBOnMyNamedCommand(FSM::InterfaceParam* param){}

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
	FSM::InterfaceCommand<MyClass> TestAOnMyNamedCommand;
	FSM::State<MyClass> SubstateAA;
	FSM::Transition<MyClass> SubstateAAToSubstateAB;
	FSM::InterfaceTransition<MyClass> SubstateABToTestBOnMyNamedCommand;
	FSM::State<MyClass> SubstateAB;
	FSM::State<MyClass> TestB;
	FSM::State<MyClass> SubstateBA;
	FSM::State<MyClass> SubstateBB;

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

private:
	void InitializeFSM()
	{
		FSM_INIT(MyClass);
		FSM_INIT_STATE_UPDATE(MyClass, TestA, true);
		FSM_INIT_STATE(MyClass, SubstateAA, true);
		FSM_INIT_STATE(MyClass, SubstateAB, false);
		FSM_INIT_STATE(MyClass, TestB, false);
		FSM_INIT_STATE(MyClass, SubstateBA, false);
		FSM_INIT_STATE(MyClass, SubstateBB, false);
		
		FSM_INIT_INTERFACECOMMAND(MyClass, TestA, MyNamedCommand);
		FSM_INIT_TRANSITION(MyClass, SubstateAA, SubstateAB);
		FSM_INIT_INTERFACETRANSITION(MyClass, SubstateAB, MyNamedCommand, TestB);

		FSM.addChild(TestA);
		FSM.addChild(TestB);
		TestA.addChild(SubstateAA);
		TestA.addChild(SubstateAB);
		TestB.addChild(SubstateBA);
		TestB.addChild(SubstateBB);

		//SubstateAA.registerTransition(
		
	}
};



int _tmain(int argc, _TCHAR* argv[])
{
	MyClass c;
	//c.InitializeFSM();


	c.update();
	
	return 0;
}

