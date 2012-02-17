	enum InterfaceCommands
	{
		MyNamedCommand = 0
	};

	//declaration
	FSM::StateMachine<MyClass> FSM;
private:
	void onEnterFSM() {}
	void onExitFSM() {}
	

	//State TestA
	void onEnterTestA(){}
	void onExitTestA(){}
	void updateTestA(float dt){}
	FSM::InterfaceResult::Enum testTestAOnMyNamedCommand(FSM::InterfaceParam* param){ return FSM::InterfaceResult::Success;}
	void execTestAOnMyNamedCommand(FSM::InterfaceParam* param)
	{
		printf("mynamedcommand recieved by testA\n");
	}

	//State SubstateAA
	void onEnterSubstateAA(){}
	void onExitSubstateAA(){}
	bool testSubstateAAToSubstateAB(){ return false;}
	void execSubstateAAToSubstateAB(){}
	FSM::InterfaceResult::Enum testSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param){return FSM::InterfaceResult::Success;}
	void execSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param){}

	//State SubstateAB
	void onEnterSubstateAB(){}
	void onExitSubstateAB(){}
	bool testSubstateABToTestB(){ return true;}
	void execSubstateABToTestB(){}

	//State TestB
	void onEnterTestB()
	{
		//SubstateBB.initial = true;
	}
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
	FSM::InterfaceTransition<MyClass> SubstateAAToTestBOnMyNamedCommand;
	FSM::State<MyClass> SubstateAB;
	FSM::Transition<MyClass> SubstateABToTestB;
	FSM::State<MyClass> TestB;
	//FSM::State<MyClass> SubstateBA;
	FSM::State<MyClass> SubstateBB;


	void InitializeFSM()
	{
		FSM_INIT(MyClass);
		FSM_INIT_STATE_UPDATE(MyClass, TestA, true);
		FSM_INIT_STATE(MyClass, SubstateAA, true);
		FSM_INIT_STATE(MyClass, SubstateAB, false);
		FSM_INIT_STATE(MyClass, TestB, false);
		//FSM_INIT_STATE(MyClass, SubstateBA, false);
		FSM_INIT_STATE(MyClass, SubstateBB, false);
		
		FSM_INIT_INTERFACECOMMAND(MyClass, TestA, MyNamedCommand);
		FSM_INIT_TRANSITION(MyClass, SubstateAA, SubstateAB);
		FSM_INIT_INTERFACETRANSITION(MyClass, SubstateAA, MyNamedCommand, TestB);
		FSM_INIT_TRANSITION(MyClass, SubstateAB, TestB);

		FSM.addChild(TestA);
		FSM.addChild(TestB);
		TestA.addChild(SubstateAA);
		TestA.addChild(SubstateAB);
		//TestB.addChild(SubstateBA);
		TestB.addChild(SubstateBB);
		
	}
