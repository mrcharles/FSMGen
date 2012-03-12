using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;
using FSMGen.Attributes;

namespace FSMGen.Visitors
{
    class DeclarationVisitor : StateVisitor
    {
        StreamWriter stream;

        public DeclarationVisitor(Config config, FSMFile file)
            : base(config, file)
        {
        }
        ~DeclarationVisitor()
        {
            if (stream != null)
                stream.Dispose();
        }

        public override void Init()
        {
            stream = new StreamWriter(fsmfile.ImplementationFile, false);
            stream.AutoFlush = false;

            stream.WriteLine("public:");
        }

        //public override bool Valid(Statement s)
        //{
        //    if (s is ClassStatement || s is StateStatement || s is TransitionStatement || s is TestStatement)
        //    {
        //        return true;
        //    }

        //    return base.Valid(s);
        //}

        public override void VisitClassStatement(ClassStatement s)
        {
            base.VisitClassStatement(s);

            stream.WriteLine("\tFSM::StateMachine<" + ClassName + "> FSM;");
            stream.WriteLine("private:");
            stream.WriteLine("\tvoid onEnterFSM();");
            stream.WriteLine("\tvoid onExitFSM();");
            stream.WriteLine();
        }

        public override void VisitStateStatement(StateStatement state)
        {
            base.VisitStateStatement(state);

            if (ClassName == null)
                throw new MalformedFSMException("No class statement found before state implementation.", state.line);

            stream.WriteLine("\tFSM::State<" + ClassName + "> " + state.name + ";");
            stream.WriteLine("\tvoid onEnter" + state.name + "();");
            stream.WriteLine("\tvoid onExit" + state.name + "();");
            if (state.HasStatement(typeof(UpdateStatement)))
            {
                stream.WriteLine("\tvoid update" + state.name + "(float dt);");
            }
            stream.WriteLine();
        }

        public virtual void VisitTestStatement(TestStatement test)
        { 
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Command found outside of state block", test.line);

            string transName = state + "On" + test.name;

            stream.WriteLine("\tFSM::InterfaceCommand<" + ClassName + "> " + transName + ";");

            stream.WriteLine("\tFSM::InterfaceResult::Enum test" + transName + "(FSM::InterfaceParam* param);");
            stream.WriteLine("\tvoid exec" + transName + "(FSM::InterfaceParam* param);");
            stream.WriteLine();
        }

        public virtual void VisitDenyStatement(DenyStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Command found outside of state block", test.line);

            string transName = state + "On" + test.name;

            stream.WriteLine("\tFSM::InterfaceCommandDeny<" + ClassName + "> " + transName + ";");

            stream.WriteLine();
        }

        public virtual void VisitAllowStatement(AllowStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Command found outside of state block", test.line);

            string transName = state + "On" + test.name;

            stream.WriteLine("\tFSM::InterfaceCommandAllow<" + ClassName + "> " + transName + ";");

            stream.WriteLine();
        }

        public virtual void VisitTransitionStatement(TransitionStatement transition)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Command found outside of state block", transition.line);
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

        public override void End()
        {
            stream.Flush();
            stream.Close();
        }
    }
}
