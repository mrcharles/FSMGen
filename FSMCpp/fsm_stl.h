#ifndef FSM_STL_H
#define FSM_STL_H

#include <vector>

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
	class State
	{
		typedef void(T::*onEnterFunc)();
		typedef void(T::*onExitFunc)();
		typedef void(T::*updateFunc)(float dt);
		typedef bool(T::*transitionFunc)();
		typedef InterfaceResult::Enum (T::*interfaceFunc)( InterfaceParam* param );

		std::string name;
		bool initial;

		std::vector<State> children;
		onEnterFunc onEnter;
		onEnterFunc onExit;
		updateFunc update;
		std::vector<transitionFunc> transitions;
	
		State(std::string _name, bool _initial, onEnterFunc _onEnter, onExitFunc _onExit, updateFunc _update = NULL) 
			: name(_name)
			, initial(_intial)
			, onEnter(_onEnter)
			, onExit(_onExit),
			, update(_update)
		{
		}
		
		//void registerTransition(transitionFunc transition)


	};
}

#endif