#ifndef FSM_STL_H
#define FSM_STL_H

#include <vector>
#include <string>
#include <map>
#include <stack>

namespace FSM {

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

	template <class T>
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
	

	class TransitionBase
	{
		std::string target;
	public:
		TransitionBase()
		{}
		void init(const std::string &_target)
		{
			target = _target;
		}
		virtual InterfaceResult::Enum test(InterfaceParam* param = NULL) = 0;
		virtual void exec(InterfaceParam* param = NULL) = 0;
		const std::string& getTarget() const { return target; }
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

		void init(testFunc test, execFunc exec, const std::string target)
		{
			fTest = test;
			fExec = exec;
			TransitionBase::init(target);
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param = NULL)
		{
			if( (_instance->*fTest)() )
				return InterfaceResult::Success;

			return InterfaceResult::Failed;
		}
		virtual void exec(InterfaceParam* param = NULL)
		{
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
		void init(testInterfaceFunc test, execInterfaceFunc exec, int command)
		{
			fTestInterface = test;
			fExecInterface = exec;
			interfaceCommand = command;
			TransitionBase::init("");
		}

		virtual InterfaceResult::Enum test(InterfaceParam* param)
		{
			return  (_instance->*fTestInterface)(param);
		}
		virtual void exec(InterfaceParam* param )
		{
			(_instance->*fExecInterface)(param);
		}

	};

	template <class T> 
	class InterfaceTransition: public InterfaceCommand<T>
	{
		DECLARE_TYPEDEFS(T);

	public:
		void init(testInterfaceFunc test, execInterfaceFunc exec, int command, const std::string &target)
		{
			InterfaceCommand::init(test, exec, command);
			TransitionBase::init(target);
		}

	};

	#define FSM_INIT( classname ) \
	FSM.setInstance(this); \
	FSM.init( & ##classname::onEnterFSM, & ##classname::onExitFSM );

#define FSM_INIT_STATE_UPDATE( classname, statename, initial) \
	statename.setInstance(this);\
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename, & ##classname::update##statename);

#define FSM_INIT_STATE( classname, statename, initial) \
	statename.setInstance(this);\
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename);

//this macro uses this-> to enable pretty names for transitions. 
#define FSM_INIT_TRANSITION( classname, statename, targetname ) \
	this->##statename##To##targetname.setInstance(this); \
	this->##statename##To##targetname.init( &##classname::test##statename##To##targetname, &##classname::exec##statename##To##targetname, #targetname); \
	statename.registerTransition(this->##statename##To##targetname);

#define FSM_INIT_INTERFACECOMMAND( classname, statename, command ) \
	this->##statename##On##command.setInstance(this); \
	this->##statename##On##command.init( &##classname::test##statename##On##command, &##classname::exec##statename##On##command, command);\
	statename.registerTransition(this->##statename##On##command);

#define FSM_INIT_INTERFACETRANSITION( classname, statename, command, targetname ) \
	this->##statename##To##targetname##On##command.setInstance(this); \
	this->##statename##To##targetname##On##command.init( &##classname::test##statename##To##targetname##On##command, &##classname::exec##statename##To##targetname##On##command, command, #targetname);\
	statename.registerTransition(this->##statename##To##targetname##On##command);


	void FSMError(const std::string &text);
	void FSMAssert(bool mustBeTrue, const std::string &error);

	template <class T>
	class State
	{
		DECLARE_TYPEDEFS(T);

	protected:
		//state static data
		std::string name;
		bool initial;

	private:
		std::vector<State<T> *> children;
		State<T>* parent;
		onEnterFunc onEnter;
		onEnterFunc onExit;
		updateFunc onUpdate;
		std::vector<TransitionBase* > transitions;

	public:
		//state status data
		bool active;
		std::vector<TransitionBase*>& getTransitions()
		{
			return transitions;
		}
	public:	
		State()
		{
			init("", false, NULL, NULL);
		}
		State(std::string _name, bool _initial, onEnterFunc _onEnter, onExitFunc _onExit, updateFunc _update = NULL) 
		{
			init(_name, _initial, _onEnter, _onExit, _update);
		}
		
		void init(std::string _name, bool _initial, onEnterFunc _onEnter, onExitFunc _onExit, updateFunc _update = NULL)
		{
			name = _name;
			initial = _initial;
			onEnter = _onEnter;
			onExit = _onExit;
			onUpdate = _update;
			active = false;
		}

		void registerTransition( TransitionBase& transition)
		{
			transitions.push_back( & transition );
		}

		void addChild( State<T>& child )
		{
			child.parent = this;
			children.push_back(& child);
		}

		State<T>* getParent()
		{
			return parent;
		}

		void getParents(std::stack< State<T>* > &parents)
		{
			State<T> *state = parent;

			while(state != NULL)
			{
				parents.push(state);
				state = state->parent;
			}
		}

		State<T>* getActiveChild()
		{
			for( std::vector<State<T>*>::const_iterator it = children.begin(); it != children.end(); ++it )
			{
				State<T>* state = *it;
				if(state->active)
					return state;
			}
			return NULL;
		}

		State<T>* getInitialChild()
		{
			if( children.size() > 0)
			{
				for( std::vector<State<T>*>::const_iterator it = children.begin(); it != children.end(); ++it )
				{
					State<T>* state = *it;
					if(state->initial)
						return state;
				}
				FSMAssert(false, "State " + name + "does not have any children marked initial.");
			}

			return NULL;
		}
		
		void update(float dt)
		{
			if(onUpdate)
				(_instance->*onUpdate)(dt);
		}

		void exit()
		{
			if(onExit)
				(_instance->*onExit)();
			active = false;
		}

		void enter()
		{
			if(onEnter)
				(_instance->*onEnter)();
			active = true;
		}

		bool valid()
		{
			//first test: make we have one and only one initial state
			if(children.size() > 0)
			{
				bool bFoundInitial = false;

				for( std::vector<State<T> *>::const_iterator it = children.begin(); it != children.end(); ++it)
				{
					State<T> *state = *it;

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

   template <class T>
	class StateMachine : public State<T>
	{
		//DECLARE_TYPEDEFS(T);

		std::map<std::string, State<T>*> states;
		State<T>* activeState;
							
	public:
		void init( onEnterFunc enter, onExitFunc exit)
			
		{
			State::init("_super", true, enter, exit);
			activeState = NULL;
		}

		void registerState(std::string name, State<T>* state)
		{
			states[name] = state;
		}

		void setChild(std::string _parent, std::string _child)
		{
			State<T>* parent = states[_parent];
			State<T>* child = states[_child];
			parent->addChild(child);
		}

		bool stateExists(const std::string &name)
		{
			return getState(name) != NULL;
		}

		State<T>* getState(const std::string &name)
		{
			return states[name];
		}

		void update(float dt)
		{
			testIntegrity();

			//update each active state. 
			//right now, states update from top down
			State<T> *state = getActiveChild();
			State<T> *leafmost = NULL;
			while( state != NULL )
			{
				state->update(dt);
				leafmost = state;
				state = state->getActiveChild();
			}

			state = leafmost;
			//test transitions from leafmost updwards
			while( state != NULL )
			{
				std::vector<TransitionBase* > &transitions = state->getTransitions();

				for(std::vector<TransitionBase*>::const_iterator it = transitions.begin(); it != transitions.end(); ++it)
				{
					TransitionBase* trans = *it;

					if(trans->test())
					{
						ChangeState(trans->getTarget());
					}
				}

			}


		}

		State<T>* getCommonParent(State<T>* stateA, State<T>* stateB)
		{
		   std::stack< State<T>* > parentsA;
		   std::stack< State<T>* > parentsB;

		   stateA->getParents(parentsA);
		   stateB->getParents(parentsB);

		   return getCommonParent(parentsA, parentsB);
		}

		State<T>* getCommonParent(std::stack< State<T>* > &parentsA, std::stack< State<T>* > &parentsB)
		{
			State<T> * parentA = parentsA.top();
			State<T> * parentB = parentsB.top();

			State<T> * common = NULL;

			while( parentA == parentB && parentA != NULL && parentB != NULL )
			{
				common = parentA;
				parentA = parentsA.top();
				parentB = parentsB.top();
				parentsA.pop();
				parentsB.pop();
			}

			return common;
		
		}

		void ChangeState(const std::string& name)
		{
			State<T> *targetState = getState(name);
			FSMAssert(targetState != NULL, "target state for state change not found.");
			std::stack< State<T>* > activeParents;
			std::stack< State<T>* > targetParents;

			activeState->getParents(activeParents);
			targetState->getParents(targetParents);
		  
		    State<T> *root = getCommonParent(activeParents, targetParents);

			//send exits, active state up to parents
			State<T> *exitState = activeState;
			while( exitState != NULL && exitState != root )
			{
				exitState->exit();
				exitState = exitState->getParent();
			}

			//now we have to activate our new state, and go down the chain 
			//activating initial state until we get to the leafmost
			FSMAssert( targetState->getParent() == root, "target state must be a sibling of a state in active state's parent tree. FSM now broken.");

			ActivateState(targetState);


		}

		void ActivateState(State<T>* state)
		{
			State<T> *enterState = state;
			while(enterState != NULL)
			{
				FSMAssert(!enterState->active, "Trying to activate an already active state.");
				enterState->enter();
				activeState = enterState;
				enterState = enterState->getInitialChild();
			}
		}

		void testIntegrity()
		{
			if(activeState)
				activeState->valid();
		}

	};



}

#endif