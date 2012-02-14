#ifndef FSM_STL_H
#define FSM_STL_H

#include <vector>
#include <string>
#include <map>

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
   		typedef void(T::*onEnterFunc)(); \
		typedef void(T::*onExitFunc)();	  \
		typedef void(T::*updateFunc)(float dt);	\
		typedef bool(T::*testFunc)();		 \
		typedef InterfaceResult::Enum (T::*testInterfaceFunc)( InterfaceParam* param );	\
		typedef void(T::*execInterfaceFunc)( InterfaceParam* param );				 \
		typedef void(T::*execFunc)();  \
	protected: \
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

	template <class T>
	class StateMachine
	{
		DECLARE_TYPEDEFS(T);

		std::map<std::string, State<T>*> states;

	public:
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

		//void setTransition(transitionFunc func, execFunc execFunc, State<T>* _target)
		//{
		//	
		//}
	};


#define FSM_INIT_STATE_UPDATE( classname, statename, initial) \
	statename.setInstance(this);\
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename, & ##classname::update##statename);

#define FSM_INIT_STATE( classname, statename, initial) \
	statename.setInstance(this);\
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename);

//this macro uses this-> to enable pretty names for transitions. 
#define FSM_INIT_TRANSITION( classname, statename, targetname ) \
	this->##statename##To##targetname.setInstance(this); \
	this->##statename##To##targetname.init( &##classname::test##statename##To##targetname, &##classname::exec##statename##To##targetname, #targetname);

#define FSM_INIT_INTERFACECOMMAND( classname, statename, command ) \
	this->##statename##On##command.setInstance(this); \
	this->##statename##On##command.init( &##classname::test##statename##On##command, &##classname::exec##statename##On##command, command);

	template <class T>
	class State
	{
		DECLARE_TYPEDEFS(T);

		std::string name;
		bool initial;

		std::vector<State*> children;
		onEnterFunc onEnter;
		onEnterFunc onExit;
		updateFunc update;
		std::vector<TransitionBase* > transitions;
	public:	
		State()
		{
			initial = false;
			onEnter = NULL;
			onExit = NULL;
			update = NULL;
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
			update = _update;
		}

		//void registerTransition(transitionFunc transition, State<T>* target)
		//{
		//	transitions.push_back( Transition(transition, target) );
		//}

		//void registerInterfaceCommand(interfaceFunc func, int command)
		//{
		//	transitions.push_back( Transition(func, command, NULL) );
		//}

		//void registerInterfaceTransition(interfaceFunc func, int command, State<T>* target)
		//{
		//	transitions.push_back( Transition(func, command, target) );
		//}

		void addChild( State<T>& child )
		{
			children.push_back(& child);
		}

	};
}

#endif