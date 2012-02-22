using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    class DeclarationVisitor : StateVisitor
    {
        public DeclarationVisitor(StreamWriter stream)
            : base(stream)
        { }

        public override void Init()
        {
            stream.WriteLine("public:");
        }

        public override bool Valid(Statement s)
        {
            if (s is ClassStatement || s is StateStatement || s is TransitionStatement || s is TestStatement)
            {
                return true;
            }

            return base.Valid(s);
        }

        public override void Visit(Statement s)
        {
            base.Visit(s);
            if (ClassName == null)
                throw new MalformedFSMException("No class statement found before state implementation.", s.line);
            if (s is ClassStatement)
            {
                stream.WriteLine("\tFSM::StateMachine<" + ClassName + "> FSM;");
                stream.WriteLine("private:");
                stream.WriteLine("\tvoid onEnterFSM();");
                stream.WriteLine("\tvoid onExitFSM();");
                stream.WriteLine();
            }
            if (s is StateStatement)
            {
                StateStatement state = s as StateStatement;
                //state statements only have enter/exit/update func declarations

                stream.WriteLine("\tFSM::State<" + ClassName + "> " + state.name + ";");
                stream.WriteLine("\tvoid onEnter" + state.name + "();");
                stream.WriteLine("\tvoid onExit" + state.name + "();");
                if (state.HasStatement(typeof(UpdateStatement)))
                {
                    stream.WriteLine("\tvoid update" + state.name + "(float dt);");
                }
                stream.WriteLine();
            }
            if (s is TestStatement)
            {
                TestStatement test = s as TestStatement;
                string state = GetState();
                if (state == null)
                    throw new MalformedFSMException("Interface Command found outside of state block", s.line);

                string transName = state + "On" + test.name;

                stream.WriteLine("\tFSM::InterfaceCommand<" + ClassName + "> " + transName + ";");

                stream.WriteLine("\tFSM::InterfaceResult::Enum test" + transName + "(FSM::InterfaceParam* param);");
                stream.WriteLine("\tvoid exec" + transName + "(FSM::InterfaceParam* param);");
                stream.WriteLine();
            }
            if (s is TransitionStatement)
            {
                TransitionStatement transition = s as TransitionStatement;
                string state = GetState();
                if (state == null)
                    throw new MalformedFSMException("Interface Command found outside of state block", s.line);
                if (transition.command != null)
                {
                    string transName = state + "To" + transition.targetstate + "On" + transition.command;
                    stream.WriteLine("\tFSM::InterfaceTransition<" + ClassName + "> " + transName + ";");
                    stream.WriteLine("\tFSM::InterfaceResult::Enum test" + transName + "(FSM::InterfaceParam* param);");
                    stream.WriteLine("\tvoid exec" + state + "To" + transition.targetstate + "On" + transition.command + "(FSM::InterfaceParam* param);");
                }
                else
                {
                    string transName = state + "To" + transition.targetstate;
                    stream.WriteLine("\tFSM::Transition<" + ClassName + "> " + transName + ";");
                    stream.WriteLine("\tbool test" + transName + "();");
                    stream.WriteLine("\tvoid exec" + transName + "();");
                }
                stream.WriteLine();
            }

        }

        public override void End()
        {
            //throw new NotImplementedException();
        }
    }
}
