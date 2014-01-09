#include "stdafx.h"
#include "MyClass.h"

void MyClass::onEnterFSM() {}
void MyClass::onExitFSM() {}


//State TestA
void MyClass::onEnterTestA(){}
void MyClass::onExitTestA(){}
void MyClass::updateTestA(float dt){}
FSM::InterfaceResult::Enum MyClass::testTestAOnMyNamedCommand(FSM::InterfaceParam* param){ return FSM::InterfaceResult::Success;}
void MyClass::execTestAOnMyNamedCommand(FSM::InterfaceParam* param)
{
	printf("mynamedcommand recieved by testA\n");
}

//State SubstateAA
void MyClass::onEnterSubstateAA(){}
void MyClass::onExitSubstateAA(){}

bool MyClass::testSubstateAAToSubstateAA(){ return false;}
void MyClass::execSubstateAAToSubstateAA(){}

bool MyClass::testSubstateAAToSubstateAB(){ return false;}
void MyClass::execSubstateAAToSubstateAB(){}
FSM::InterfaceResult::Enum MyClass::testSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param){return FSM::InterfaceResult::Success;}
void MyClass::execSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param){}

//State SubstateAB
void MyClass::onEnterSubstateAB(){}
void MyClass::onExitSubstateAB(){}
bool MyClass::testSubstateABToTestB(){ return true;}
void MyClass::execSubstateABToTestB(){}

//State TestB
void MyClass::onEnterTestB()
{
	//SubstateBB.initial = true;
}
void MyClass::onExitTestB(){}

//State SubstateBA
void MyClass::onEnterSubstateBA(){}
void MyClass::onExitSubstateBA(){}

//State SubstateBB
void MyClass::onEnterSubstateBB(){}
void MyClass::onExitSubstateBB(){}

