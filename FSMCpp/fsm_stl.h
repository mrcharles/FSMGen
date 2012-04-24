#ifndef FSM_STL_H
#define FSM_STL_H

#include <vector>
#include <string>
#include <map>
#include <stack>

#ifdef _WIN32
#pragma warning (push)
#pragma warning (disable: 4100)
#endif

namespace FSM {

	void FSMError(const std::string &text);
	void FSMAssert(bool mustBeTrue, const std::string &error);

	namespace InterfaceResult
	{
		enum Enum 
		{
			Unhandled = 0, 
			Success,
			Failed
		};
	}

	class InterfaceParam
	{
	
	};

	class State;

#define DECLARE_TYPEDEFS(T) \
	protected: \
		typedef void(T::*onEnterFunc)(); \
		typedef void(T::*onExitFunc)();	  \
		typedef void(T::*updateFunc)(float dt);	\
		typedef bool(T::*testFunc)();		 \
		typedef InterfaceResult::Enum (T::*testInterfaceFunc)( InterfaceParam* param );	\
		typedef void(T::*execInterfaceFunc)( InterfaceParam* param );				 \
		typedef void(T::*execFunc)();  \
		T *_instance; \
	public: \
		void setInstance( T* instance ) \
			{ _instance = instance; } 	
	
#define DECLARE_TYPEDEFS_ONLY(T) \
	protected: \
		typedef void(T::*onEnterFunc)(); \
		typedef void(T::*onExitFunc)();	  \
		typedef void(T::*updateFunc)(float dt);	\
		typedef bool(T::*testFunc)();		 \
		typedef InterfaceResult::Enum (T::*testInterfaceFunc)( InterfaceParam* param );	\
		typedef void(T::*execInterfaceFunc)( InterfaceParam* param );				 \
		typedef void(T::*execFunc)();
		
	class TransitionBase
	{
		std::string name;
		std::string target;
	public:
		TransitionBase()
		{}
		virtual ~TransitionBase(){}
		void init(const std::string &_name, const std::string &_target)
		{
			name = _name;
			target = _target;
		}
		virtual InterfaceResult::Enum test(InterfaceParam* param = NULL) = 0;
		virtual void exec(InterfaceParam* param = NULL) = 0;
		const std::string& getTarget() const { return target; }
		virtual int getCommand() { return -1; }
	};

	template <class T>
	class Transition : public TransitionBase
	{
		DECLARE_TYPEDEFS(T);
	private:

		testFunc fTest;
		execFunc fExec;

	public:
		Transition()
		{}

		void init(const std::string &_name, testFunc _test, execFunc _exec, const std::string _target)
		{
			fTest = _test;
			fExec = _exec;
			TransitionBase::init(_name, _target);
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param = NULL)
		{
			if(param == NULL)
			{
				if( (_instance->*fTest)() )
					return InterfaceResult::Success;

				return InterfaceResult::Failed;
			}
			return InterfaceResult::Unhandled;
		}
		virtual void exec(InterfaceParam* param = NULL)
		{
			(void)param;
			if(fExec != NULL)
				(_instance->*fExec)();
		}
	};

	template <class T>
	class InterfaceCommand : public TransitionBase
	{

		DECLARE_TYPEDEFS(T);
	private:
		testInterfaceFunc fTestInterface;
		execInterfaceFunc fExecInterface;

		int interfaceCommand;


	public:
		void init(const std::string &_name, testInterfaceFunc _test, execInterfaceFunc _exec, int _command)
		{
			fTestInterface = _test;
			fExecInterface = _exec;
			interfaceCommand = _command;
			TransitionBase::init(_name, "");
		}

		virtual int getCommand()
		{
			return interfaceCommand;
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param)
		{
			//implicitly we return unhandled if there's no param. facilitates
			//grouping all transition types together. 
			if(param)
				return  (_instance->*fTestInterface)(param);
			return InterfaceResult::Unhandled;
		}
		virtual void exec(InterfaceParam* param )
		{
			if(fExecInterface != NULL)
				(_instance->*fExecInterface)(param);
		}

	};

	template <class T>
	class InterfaceCommandDeny : public InterfaceCommand<T>
	{
	public:
		void init( const std::string &_name, int command )
		{
			InterfaceCommand<T>::init(_name, NULL, NULL, command);
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param)
		{
			(void)param;
			return InterfaceResult::Failed;
		}
		virtual void exec(InterfaceParam* param )
		{
			(void)param;
		}

	};

	template <class T>
	class InterfaceCommandAllow : public InterfaceCommand<T>
	{
	public:
		void init( const std::string &_name, int command )
		{
			InterfaceCommand<T>::init(_name, NULL, NULL, command);
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param)
		{
			return InterfaceResult::Success;
		}
		virtual void exec(InterfaceParam* param )
		{
		}

	};

	template <class T> 
	class InterfaceTransition: public InterfaceCommand<T>
	{
		DECLARE_TYPEDEFS_ONLY(T)
	public:
		void init(const std::string &_name, testInterfaceFunc _test, execInterfaceFunc _exec, int _command, const std::string &_target)
		{
			InterfaceCommand<T>::init(_name, _test, _exec, _command);
			TransitionBase::init(_name, _target);
		}

	};

	class StateDelegate
	{
	public:
		virtual ~StateDelegate(){}
		virtual void onEnter() = 0;
		virtual void onExit() = 0;
		virtual void onUpdate(float dt) = 0;
	};

	template <class T>
	class StateDelegateT : public StateDelegate
	{
	public:
		typedef void(T::*voidFunc)();	  
		typedef void(T::*updateFunc)(float dt);	

	private:
		voidFunc enter;
		voidFunc exit;
		updateFunc update;
		T* instance;

	public:
		StateDelegateT()
		{
			enter = NULL;
			exit = NULL;
			update = NULL;
			instance = NULL;
		}

		virtual ~StateDelegateT(){}
	
		void init( T* _instance, voidFunc _enter, voidFunc _exit, updateFunc _update)
		{
			enter = _enter;
			exit = _exit;
			update = _update;
			instance = _instance;
		}
		virtual void onEnter()
		{
			if(enter)
				(instance->*enter)();
		}
		virtual void onExit()
		{
			if(exit)
				(instance->*exit)();
		}
		virtual void onUpdate(float dt)
		{
			if(update)
				(instance->*update)(dt);
		}
	};

#define FSM_INIT( ) \
	FSM.init( &FSMDelegate );

#define FSM_INIT_STATE

#define FSM_INIT_STATE_UPDATE( classname, statename, initial) \
	statename##Delegate.init( this, & ##classname::onEnter##statename, & ##classname::onExit##statename, & ##classname::update##statename ); \
	statename.init(#statename, initial, & statename##Delegate); \
	FSM.registerState(statename);

#define FSM_INIT_STATE_EXPLICIT( statename, initial, enter, exit, update) \
	statename##Delegate.init( this, enter, exit, update ); \
	statename.init(#statename, initial, & statename##Delegate); \
	FSM.registerState(statename);

//this macro uses this-> to enable pretty names for transitions. 
#define FSM_INIT_TRANSITION( classname, statename, targetname ) \
	this->statename##To##targetname.setInstance(this); \
	this->statename##To##targetname.init( #statename "To" #targetname, &classname::test##statename##To##targetname, &classname::exec##statename##To##targetname, #targetname); \
	statename.registerTransition(this->statename##To##targetname);

#define FSM_INIT_TRANSITION_NOEXEC( classname, statename, targetname ) \
	this->statename##To##targetname.setInstance(this); \
	this->statename##To##targetname.init( #statename "To" #targetname, &classname::test##statename##To##targetname, NULL, #targetname); \
	statename.registerTransition(this->statename##To##targetname);

#define FSM_INIT_INTERFACECOMMAND( classname, statename, command ) \
	this->statename##On##command.setInstance(this); \
	this->statename##On##command.init( #statename "On" #command, &classname::test##statename##On##command, &classname::exec##statename##On##command, InterfaceCommands::command);\
	statename.registerTransition(this->statename##On##command);

#define FSM_INIT_INTERFACECOMMAND_NOEXEC( classname, statename, command ) \
	this->statename##On##command.setInstance(this); \
	this->statename##On##command.init( #statename "On" #command, &classname::test##statename##On##command, NULL, InterfaceCommands::command);\
	statename.registerTransition(this->statename##On##command);

#define FSM_INIT_INTERFACEDENY( classname, statename, command ) \
	this->statename##On##command.setInstance(this); \
	this->statename##On##command.init( #statename "On" #command, InterfaceCommands::command);\
	statename.registerTransition(this->statename##On##command);

#define FSM_INIT_INTERFACEALLOW( classname, statename, command ) \
	this->statename##On##command.setInstance(this); \
	this->statename##On##command.init( #statename "On" #command, InterfaceCommands::command);\
	statename.registerTransition(this->statename##On##command);

#define FSM_INIT_INTERFACETRANSITION( classname, statename, command, targetname ) \
	this->statename##To##targetname##On##command.setInstance(this); \
	this->statename##To##targetname##On##command.init( #statename "To" #targetname "On" #command, &classname::test##statename##To##targetname##On##command, &classname::exec##statename##To##targetname##On##command, InterfaceCommands::command, #targetname);\
	statename.registerTransition(this->statename##To##targetname##On##command);

#define FSM_INIT_INTERFACETRANSITION_NOEXEC( classname, statename, command, targetname ) \
	this->statename##To##targetname##On##command.setInstance(this); \
	this->statename##To##targetname##On##command.init( #statename "To" #targetname "On" #command, &classname::test##statename##To##targetname##On##command, NULL, InterfaceCommands::command, #targetname);\
	statename.registerTransition(this->statename##To##targetname##On##command);

	//template <class T>
	class State
	{
		//DECLARE_TYPEDEFS(T);

	protected:
		//state static data
		std::string name;

	private:
		std::vector<State *> children;
		State* parent;
		//onEnterFunc onEnter;
		//onEnterFunc onExit;
		//updateFunc onUpdate;
		
		StateDelegate* delegate;
		std::vector<TransitionBase* > transitions;

	public:
		//state status data
		bool active;
		bool initial;
	public:	
		State()
		{
			init("", false, NULL);
		}
		State(std::string _name, bool _initial, StateDelegate* _delegate) 
		{
			init(_name, _initial, _delegate);
		}
		
		void init(std::string _name, bool _initial, StateDelegate* _delegate)
		{
			name = _name;
			initial = _initial;
			delegate = _delegate;
			active = false;
			parent = NULL;
		}

		const std::string& getName() const
		{
			return name;
		}

		void registerTransition( TransitionBase& transition)
		{
			transitions.push_back( & transition );
		}

		std::vector<TransitionBase*>& getTransitions()
		{
			return transitions;
		}

		void addChild( State& child )
		{
			child.parent = this;
			children.push_back(& child);
		}

		State* getActiveChild()
		{
			for( std::vector<State*>::const_iterator it = children.begin(); it != children.end(); ++it )
			{
				State* state = *it;
				if(state->active)
					return state;
			}
			return NULL;
		}

		State* getInitialChild()
		{
			//if there's only one child it is implicitly initial. 
			if( children.size() == 1)
				return children[0];

			if( children.size() > 0)
			{
				for( std::vector<State*>::const_iterator it = children.begin(); it != children.end(); ++it )
				{
					State* state = *it;
					if(state->initial)
						return state;
				}
				FSMAssert(false, "State " + name + "does not have any children marked initial.");
			}

			return NULL;
		}
		
		State* getParent()
		{
			return parent;
		}

		void getParents(std::stack< State* > &parents)
		{
			State *state = parent;

			while(state != NULL)
			{
				parents.push(state);
				state = state->parent;
			}
		}

		void update(float dt)
		{
			delegate->onUpdate(dt);
		}

		void exit()
		{
			delegate->onExit();
			active = false;
		}

		void enter()
		{
			delegate->onEnter();
			active = true;
		}

		bool valid()
		{
			//first test: make we have one and only one initial state
			if(children.size() > 0)
			{
				bool bFoundInitial = false;

				for( std::vector<State *>::const_iterator it = children.begin(); it != children.end(); ++it)
				{
					State *state = *it;

					if(bFoundInitial && state->initial)
					{
						FSMError( "state " + name + " has more than one initial state." );
						return false;
					}
					else if(state->initial)
					{
						bFoundInitial = true;
					}

				}

				//might want to remove this for dynamic setting of initial state. 
				if(!bFoundInitial)
				{
					FSMError( "state " + name + " does not have an initial state.");
					return false;
				}
			}
			return true;
		}



	};

	class StateMachine : public State
	{
		//DECLARE_TYPEDEFS(T);

		std::map<std::string, State*> states;
		State* activeState;
		TransitionBase* testedTransition;
		bool updating;		
		bool activating;
		std::string queuedStateChange;
		float timeInState;

	public:
		void init( StateDelegate* _delegate)
			
		{
			State::init("_super", true, _delegate);
			activeState = NULL;
			updating = false;
			activating = false;
			timeInState = 0.0f;
		}

		void status()
		{
			printf("Current State is %s.\n", activeState->getName().c_str());

		}

		float getTimeInState() { return timeInState; }

		const std::string& getActiveStateName()
		{
			return activeState->getName();
		}

		InterfaceResult::Enum testCommand(int command, FSM::InterfaceParam *param)
		{
			FSMAssert(command >= 0, "interface commands must be greater than zero");

			//run the test from leafmost up
			State *state = activeState;

			while(state != NULL)
			{
				std::vector<TransitionBase*> transitionsVector = state->getTransitions();
				for( std::vector<TransitionBase*>::const_iterator it = transitionsVector.begin();
					 it != transitionsVector.end();
					 ++it )
				{
					TransitionBase * trans = *it;
					if( trans->getCommand() == command)
					{
						InterfaceResult::Enum result = trans->test(param);
						if(result == InterfaceResult::Failed)
							return InterfaceResult::Failed;
						if(result == InterfaceResult::Success)
						{
							testedTransition = trans;
							return InterfaceResult::Success;
						}
					}
				}
				state = state->getParent();

			}
			return InterfaceResult::Unhandled;
		}

		void execCommand(int command, FSM::InterfaceParam *param)
		{
			FSMAssert( !activating && !updating, "Cannot execute a state command while updating or entering a state. Your test/exec must happen outside the FSM.");
			FSMAssert( testedTransition != NULL && testedTransition->getCommand() == command, "Attempting to execute a transition which was not tested.");
			testedTransition->exec(param);
			const std::string& target = testedTransition->getTarget();
			if(target != "") //this will be "" in the case where we are running a command. 
				changeState(testedTransition->getTarget());
			testedTransition = NULL;
		}

		void registerState(State& state)
		{
			states[state.getName()] = &state;
		}

		bool stateExists(const std::string &_name)
		{
			return getState(_name) != NULL;
		}

		State* getState(const std::string &_name)
		{
			return states[_name];
		}

		void activate()
		{
			activating = true;
			activateState(this);
			activating = false;
		}

		void update(float dt)
		{
			//testIntegrity();

			timeInState += dt;

			//update each active state. 
			//right now, states update from top down
			State *state = getActiveChild();
			State *leafmost = NULL;

			updating = true;
			while( state != NULL )
			{
				state->update(dt);
				leafmost = state;
				state = state->getActiveChild();
			}
			updating = false;

			// if we receive a state change in the update loop, it supercedes any other transition tests

			if( queuedStateChange != "" )
			{
				changeState( queuedStateChange );
				return;
			}

			updating = true;
			state = leafmost;
			//test transitions from leafmost updwards
			while( state != NULL )
			{
				std::vector<TransitionBase* > &transitionsVector = state->getTransitions();

				for(std::vector<TransitionBase*>::const_iterator it = transitionsVector.begin(); it != transitionsVector.end(); ++it)
				{
					TransitionBase* trans = *it;

					if(trans->test() == InterfaceResult::Success)
					{
						trans->exec();
						changeState(trans->getTarget());
						updating = false;
						return;
					}
				}

				state = state->getParent();
			}
			updating = false;


		}

		void testIntegrity()
		{
			if(activeState)
				activeState->valid();
		}

		void queueStateChange( const std::string& _name )
		{
			FSMAssert( updating, "Can only queue a state change from within a state update.");
			queuedStateChange = _name;
		}

	protected:

		State* getCommonParent(State* stateA, State* stateB)
		{
		   std::stack< State* > parentsA;
		   std::stack< State* > parentsB;

		   stateA->getParents(parentsA);
		   stateB->getParents(parentsB);

		   return getCommonParent(parentsA, parentsB);
		}

		State* getCommonParent(std::stack< State* > &parentsA, std::stack< State* > &parentsB)
		{
			State * parentA = parentsA.top();
			State * parentB = parentsB.top();

			State * common = NULL;

			while( parentA == parentB && parentA != NULL && parentB != NULL )
			{
				common = parentA;
				parentsA.pop();
				parentsB.pop();
				if(parentsA.size() == 0 || parentsB.size() == 0)
					break;
				parentA = parentsA.top();
				parentB = parentsB.top();
			}

			return common;
		
		}

		void changeState(const std::string& _name)
		{
			State *targetState = getState(_name);
			FSMAssert(targetState != NULL, "target state for state change not found.");
			std::stack< State* > activeParents;
			std::stack< State* > targetParents;

			activeState->getParents(activeParents);
			targetState->getParents(targetParents);
		  
		    State *root = getCommonParent(activeParents, targetParents);

			if(activeState != targetState)
			{
				//send exits, active state up to parents
				State *exitState = activeState;
				while( exitState != NULL && exitState != root )
				{
					exitState->exit();
					exitState = exitState->getParent();
				}
			}
			else
			{
				activeState->exit();
			}

			//now we have to activate our new state, and go down the chain 
			//activating initial state until we get to the leafmost
			FSMAssert( targetState->getParent() == root, "target state must be a sibling of a state in active state's parent tree. FSM now broken.");

			activateState(targetState);


		}

		void activateState(State* state)
		{
			timeInState = 0.0f;
			State *enterState = state;
			while(enterState != NULL)
			{
				FSMAssert(!enterState->active, "Trying to activate an already active state.");
				activeState = enterState;
				enterState->enter();
				enterState = enterState->getInitialChild();
			}
		}

	};



}

#ifdef _WIN32
#pragma warning (pop)
#endif 


#endif