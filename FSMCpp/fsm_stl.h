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
		typedef bool(T::*transitionFunc)();		 \
		typedef InterfaceResult::Enum (T::*interfaceFunc)( InterfaceParam* param );	\
		typedef void(T::*execInterfaceFunc)( InterfaceParam* param );				 \
		typedef void(T::*execFunc)();

	template <class T>
	class Transition
	{
		DECLARE_TYPEDEFS(T);

		transitionFunc fTransition;
		interfaceFunc fInterface;
		execInterfaceFunc fExecInterface;
		execFunc fExec;

		int interfaceCommand;

		State<T>* target;

	public:
		Transition(transitionFunc func, execFunc execFunc, State<T>* _target)
		{
			interfaceCommand = -1;
			fTransition = func;
			fInterface = NULL;
	;		fExecInterface = NULL;
			fExec = execFunc;
			target = _target;
		}

		Transition(interfaceFunc func, execInterfaceFunc execFunc, int command, State<T>* _target)
		{
			interfaceCommand = command;
			fTransition = NULL;
			fExec = NULL;
			fInterface = func;
			fExecInterface = execFunc;
			target = _target;
		}
	};

	template <class T>
	class StateMachine
	{
		DECLARE_TYPEDEFS(T);

		std::map<std::string, State<T>*> states;

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

		State<T>* getState(std::string name)
		{
			return states[name];
		}

		void setTransition(transitionFunc func, execFunc execFunc, State<T>* _target)
		{
			
		}
	};


#define FSM_INIT_STATE_UPDATE( classname, statename, initial) \
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename, & ##classname::update##statename);

#define FSM_INIT_STATE( classname, statename, initial) \
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename);

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
		std::vector<Transition<T> > transitions;
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

		void registerTransition(transitionFunc transition, State<T>* target)
		{
			transitions.push_back( Transition(transition, target) );
		}

		void registerInterfaceCommand(interfaceFunc func, int command)
		{
			transitions.push_back( Transition(func, command, NULL) );
		}

		void registerInterfaceTransition(interfaceFunc func, int command, State<T>* target)
		{
			transitions.push_back( Transition(func, command, target) );
		}

		void addChild( State<T>& child )
		{
			children.push_back(& child);
		}

	};
}

#endif