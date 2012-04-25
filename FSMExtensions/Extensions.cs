using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FSMGen;
using FSMGen.Statements;
using FSMGen.Attributes;

namespace FSMExtensions
{
    [Token("timer")]
    public class TimerStatement : NameStatement
    {
    }

    [Token("handletimer")]
    public class HandleTimerStatement : NameStatement
    { 
    
    }

    public class InitializationVisitor : FSMGen.Visitors.InitializationVisitor
    {
        public InitializationVisitor(Config config, FSMFile file)
            : base(config, file)
        { }

        public virtual void VisitTimerStatement(TimerStatement s)
        {
            string parent = null;

            try
            {
                parent = GetParent();
            }
            catch (Exception)
            {
                parent = "FSM";
            }


            Stream.WriteLine("\t\tFSM_INIT_TIMER( " + ClassName + ", " + s.name + ", " + parent + ");");
            Stream.WriteLine();
        }

        public virtual void VisitHandleTimerStatement(HandleTimerStatement s)
        {
            Stream.WriteLine("\t\tFSM_INIT_TIMER_HANDLER( " + ClassName + ", " + s.name + ", " + GetState() + ");");
            Stream.WriteLine();
        }
    }

    public class DeclarationVisitor : FSMGen.Visitors.DeclarationVisitor
    {
        public DeclarationVisitor(Config config, FSMFile file)
            : base(config, file)
        { 
        
        }

        public virtual void VisitTimerStatement(TimerStatement s)
        {
            Stream.WriteLine("\tFSM::Timer " + s.name + "Timer;");
            Stream.WriteLine("\tFSM::TimerDelegateT<" + ClassName + "> " + s.name + "TimerDelegate;"); 
            Stream.WriteLine("\tvoid on" + s.name + "Timer();");
            Stream.WriteLine();
        }

        public virtual void VisitHandleTimerStatement(HandleTimerStatement s)
        {
            Stream.WriteLine("\tFSM::TimerDelegateT<" + ClassName + "> " + GetState() + s.name + "TimerDelegate;");
            Stream.WriteLine("\tvoid on" + GetState() + "Handle" + s.name + "Timer();");
            Stream.WriteLine();
        }
    }

    public class DefinitionVisitor : FSMGen.Visitors.DefinitionVisitor
    {
        public DefinitionVisitor(Config config, FSMFile file)
            :base(config, file)
        { }

        public virtual void VisitTimerStatement(TimerStatement s)
        {
            PrintFunc("void", ClassName + "::on" + s.name + "Timer", "()");

        }

        public virtual void VisitHandleTimerStatement(HandleTimerStatement s)
        {
            PrintFunc("void", ClassName + "::on" + GetState() + "Handle" + s.name + "Timer", "()");
        }
    }

}
