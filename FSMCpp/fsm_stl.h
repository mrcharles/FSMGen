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

	template <class T>
	class StateMachine
	{
		std::map<std::string, State<T>*> states;

		void registerState(std::string name, State<T>* state)
		{
			states[name] = state;
		}

		State<T>* getState(std::string name)
		{
			return states[name];
		}

	};

	template <class T>
	class Transition
	{
		typedef bool(T::*transitionFunc)();
		typedef InterfaceResult::Enum (T::*interfaceFunc)( InterfaceParam* param );

		transitionFunc fTransition;
		interfaceFunc fInterface;

		int interfaceCommand;

		State<T>* target;

	public:
		Transition(transitionFunc func, State<T>* _target)
		{
			interfaceCommand = -1;
			fTransition = func;
			fInterface = NULL;
			target = _target;
		}

		Transition(interfaceFunc func, int command, State<T>* _target)
		{
			interfaceCommand = command;
			fTransition = NULL;
			fInterface = func;
			target = _target;
		}
	};

#define FSM_INIT_STATE_UPDATE( classname, statename, initial) \
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename, & ##classname::update##statename);

#define FSM_INIT_STATE( classname, statename, initial) \
	statename.init(#statename, initial, & ##classname::onEnter##statename, & ##classname::onExit##statename);

	template <class T>
	class State
	{
		typedef void(T::*onEnterFunc)();
		typedef void(T::*onExitFunc)();
		typedef void(T::*updateFunc)(float dt);
		typedef bool(T::*transitionFunc)();
		typedef InterfaceResult::Enum (T::*interfaceFunc)( InterfaceParam* param );

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