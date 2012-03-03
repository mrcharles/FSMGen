using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    class InitializationVisitor : StateVisitor
    {
        StreamWriter stream;

        public InitializationVisitor(Config config, FSMFile file)
            : base(config, file)
        { 
        }
        ~InitializationVisitor()
        {
            if (stream != null)
                stream.Dispose();
        }

        public override void Init()
        {
            stream = new StreamWriter(fsmfile.ImplementationFile, true);
            stream.AutoFlush = false;

            stream.WriteLine("\tvoid InitializeFSM()");
            stream.WriteLine("\t{");
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
            stream.WriteLine("\t\tFSM_INIT(" + ClassName + ");");
            stream.WriteLine();


        }

        public override void VisitStateStatement(StateStatement state)
        {
            base.VisitStateStatement(state);
            if (ClassName == null)
                throw new MalformedFSMException("No class statement found before state implementation.", state.line);
            
            bool initial = state.HasStatement(typeof(InitialStatement));
            bool update = state.HasStatement(typeof(UpdateStatement));

            string parent = null;

            try
            {
                parent = GetParent();
            }
            catch (Exception)
            {
                parent = "FSM";
            }

            if (update)
            {
                stream.WriteLine("\t\tFSM_INIT_STATE_UPDATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") + ");");
            }
            else
            {
                stream.WriteLine("\t\tFSM_INIT_STATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") + ");");
            }

            stream.WriteLine("\t\t" + parent + ".addChild(" + state.name + ");");

            stream.WriteLine();
        
        }

        public virtual void VisitTestStatement(TestStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Test found outside of state block", test.line);

            stream.WriteLine("\t\tFSM_INIT_INTERFACECOMMAND(" + ClassName + ", " + GetState() + ", " + test.name + ");");

            stream.WriteLine();

        }

        public virtual void VisitTransitionStatement(TransitionStatement transition)
        {
            string state = GetState();

            if (transition.command == null)
            {
                if (state == null)
                    throw new MalformedFSMException("Interface Transition found outside of state block", transition.line);

                stream.WriteLine("\t\tFSM_INIT_TRANSITION(" + ClassName + ", " + GetState() + ", " + transition.targetstate + ");");
            }
            else
            {
                if (state == null)
                    throw new MalformedFSMException("Interface Command found outside of state block", transition.line);

                stream.WriteLine("\t\tFSM_INIT_INTERFACETRANSITION(" + ClassName + ", " + GetState() + ", " + transition.command + ", " + transition.targetstate + ");");
            }

            stream.WriteLine();
        
        }

        public override void End()
        {
            stream.WriteLine("\t}");

            stream.Flush();
            stream.Close();
        }
    }

}
