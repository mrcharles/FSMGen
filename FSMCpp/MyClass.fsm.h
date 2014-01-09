public:
	FSM::StateMachine<MyClass> FSM;
private:
	void onEnterFSM();
	void onExitFSM();

	FSM::State<MyClass> TestA;
	void onEnterTestA();
	void onExitTestA();
	void updateTestA(float dt);

	FSM::InterfaceCommand<MyClass> TestAOnMyNamedCommand;
	FSM::InterfaceResult::Enum testTestAOnMyNamedCommand(FSM::InterfaceParam* param);
	void execTestAOnMyNamedCommand(FSM::InterfaceParam* param);

	FSM::State<MyClass> SubstateAA;
	void onEnterSubstateAA();
	void onExitSubstateAA();

	FSM::Transition<MyClass> SubstateAAToSubstateAA;
	bool testSubstateAAToSubstateAA();
	void execSubstateAAToSubstateAA();

	FSM::Transition<MyClass> SubstateAAToSubstateAB;
	bool testSubstateAAToSubstateAB();
	void execSubstateAAToSubstateAB();

	FSM::InterfaceTransition<MyClass> SubstateAAToTestBOnMyNamedCommand;
	FSM::InterfaceResult::Enum testSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param);
	void execSubstateAAToTestBOnMyNamedCommand(FSM::InterfaceParam* param);

	FSM::State<MyClass> SubstateAB;
	void onEnterSubstateAB();
	void onExitSubstateAB();

	FSM::Transition<MyClass> SubstateABToTestB;
	bool testSubstateABToTestB();
	void execSubstateABToTestB();

	FSM::State<MyClass> TestB;
	void onEnterTestB();
	void onExitTestB();

	FSM::State<MyClass> SubstateBA;
	void onEnterSubstateBA();
	void onExitSubstateBA();

	FSM::State<MyClass> SubstateBB;
	void onEnterSubstateBB();
	void onExitSubstateBB();

	void InitializeFSM()
	{
		FSM_INIT(MyClass);

		FSM_INIT_STATE_UPDATE(MyClass, TestA, true);
		FSM.addChild(TestA);

		FSM_INIT_INTERFACECOMMAND(MyClass, TestA, MyNamedCommand);

		FSM_INIT_STATE(MyClass, SubstateAA, true);
		TestA.addChild(SubstateAA);

		FSM_INIT_TRANSITION(MyClass, SubstateAA, SubstateAA);

		FSM_INIT_TRANSITION(MyClass, SubstateAA, SubstateAB);

		FSM_INIT_INTERFACETRANSITION(MyClass, SubstateAA, MyNamedCommand, TestB);

		FSM_INIT_STATE(MyClass, SubstateAB, false);
		TestA.addChild(SubstateAB);

		FSM_INIT_TRANSITION(MyClass, SubstateAB, TestB);

		FSM_INIT_STATE(MyClass, TestB, false);
		FSM.addChild(TestB);

		FSM_INIT_STATE(MyClass, SubstateBA, true);
		TestB.addChild(SubstateBA);

		FSM_INIT_STATE(MyClass, SubstateBB, false);
		TestB.addChild(SubstateBB);

	}
