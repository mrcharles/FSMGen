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

## State modifiers

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







