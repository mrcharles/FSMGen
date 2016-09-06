# FSMGen
A tool for generating formalized state machines. 

### FSMGen
This is the actual tool which generates the output code.

### FSMExtensions
This is an extensions module to proof of concept the ability to dynamically add FSM language support to FSMGen. 

### FSMCpp
This is the only currently supported implementation of state machines. A reference implementation in C++. 

## FSM Definition

__startfsm__

Begins FSM definition.

__endfsm__

Ends FSM definition.

## State Definition

__state _STATENAME___

Begins a state definition with the given name.

__endstate__

Ends a state definition.

## State information

__initial__

Makes this state the initial state when parent state is entered.

__update__

Creates an update callback which is called while the state is active.

## Interface commands

Interface commands are used to control the state machine from an external source. You define an interface command and then your implement handlers in the state machine itself. For all interface commands, command executes if the generated test function returns InterfaceResult::Success. Command is denied if test function returns InterfaceResult::Failed. Command test continues up state chain if test function returns InterfaceResult::Unhandled

__interfacecommand__

Define an interface command to be used with the FSM.

__transition _INTERFACECOMMAND_ _STATENAME___

Define a transition to _STATENAME_ on _INTERFACECOMMAND_. Transitions can only be to sibling states or siblings of parent states.

__test _INTERFACECOMMAND___

Defines a test function which will handle _INTERFACECOMMAND_ but perform no transition.

__allow _INTERFACECOMMAND___

Defines an automatic allow for all _INTERFACECOMMAND_ tests. 

__deny _INTERFACECOMMAND___

Defines an automatic deny for all _INTERFACECOMMAND_ tests.

## Internal Transitions

Transitions are polled per frame, and when the test function returns true, the exec function is called and the transition to the new state is performed.

__transition _STATENAME___

Defines an internal transition to _STATENAME_. Transitions can only be to sibling states or siblings of parent states.

## Attributes

Various commands can have attributes. Those attributes modify the functionality of the given statement.

__+allow__

Use: transition command
Causes transition to not generate a test function, instead allowing the command by default.

__+noexec__

Causes transition or test to not generate an exec function, useful for when you don't want to implement one. 

__+noexit__

Use: state

Suppresses generation of an onStateExit function

__+noenter__

Use: state

Suppresses generation of an onStateEnter function


## FSM Extension Functionality

__state__

	__+target__

	This modifier for states allows you to use special functionality to jump to a state, no matter where in the hierarchy it is. Note that this requires knowledge of where you are jumping from and to, and can be dangerous.

	__timer _TIMERNAME___

	This state info creates a named timer which can be referenced to determine how long you were in a state.







