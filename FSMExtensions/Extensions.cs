using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

    [Token("state", true)]
    public class StateStatement : FSMGen.Statements.StateStatement
    { 
        [Modifier("target")]
        public bool Target
        {
            get;
            set;
        }

    }

    public class InitializationVisitor : FSMGen.Visitors.InitializationVisitor
    {
        public InitializationVisitor(Config config, FSMFile file)
            : base(config, file)
        { }

        public virtual void VisitStateStatement(StateStatement s)
        {
            base.VisitStateStatement(s);
            if (s.Target)
            {
                Stream.WriteLine("\t\tFSM_SET_JUMP( " + ClassName + ", " + s.name + " );");
                Stream.WriteLine();
            }
        }

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

        public virtual void VisitStateStatement(StateStatement s)
        {
            StateStatement es = (StateStatement)s;
            base.VisitStateStatement(s);

            if (es.Target)
            {
                Stream.WriteLine("\tvoid onJumpTo" + s.name + "(FSM::InterfaceParam *param);");
                Stream.WriteLine();
            }
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

        public virtual void VisitStateStatement(StateStatement s)
        {
            base.VisitStateStatement(s);

            if(s.Target)
                PrintFunc("void", ClassName + "::onJumpTo" + s.name, "(FSM::InterfaceParam *param)", "", "\t(void)param;");
        }

        public virtual void VisitTimerStatement(TimerStatement s)
        {
            PrintFunc("void", ClassName + "::on" + s.name + "Timer", "()");

        }

        public virtual void VisitHandleTimerStatement(HandleTimerStatement s)
        {
            PrintFunc("void", ClassName + "::on" + GetState() + "Handle" + s.name + "Timer", "()");
        }
    }

    public class TargetVisitor : FSMGen.Visitors.FSMVisitor
    {
        int commandIndex = 0;
        StreamWriter stream;

        public TargetVisitor(Config config, FSMFile file)
            : base(config, file)
        { }
        ~TargetVisitor()
        {
            if (stream != null)
                stream.Dispose();
        }

        public override void Init()
        {
            stream = new StreamWriter(fsmfile.ImplementationFile);
            stream.AutoFlush = false;

            stream.WriteLine("public:");
            stream.WriteLine("\tstruct s_Targets");
            stream.WriteLine("\t{");
        }

        //public override bool Valid(Statement s)
        //{
        //    if (s is InterfaceCommandStatement)
        //    {
        //        return true;
        //    }

        //    return base.Valid(s);
        //}

        public virtual void VisitStateStatement(StateStatement state)
        {
            StateStatement es = (StateStatement)state;
            if (es.Target)
            {
                stream.WriteLine("\t\tstatic const int " + state.name + " = " + commandIndex + ";");
                commandIndex++;
            }
        }

        public override void End()
        {
            stream.WriteLine("\t};");
            stream.WriteLine("\tstatic s_Targets Targets;");
            stream.WriteLine();

            stream.Flush();
            stream.Close();
            stream = null;
        }
    }    

}
