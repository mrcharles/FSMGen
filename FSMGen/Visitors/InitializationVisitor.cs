﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    public class InitializationVisitor : StateVisitor
    {
        StreamWriter stream;
        public StreamWriter Stream { get { return stream; } }


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
            this.stream = new StreamWriter(fsmfile.ImplementationFile, true);
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
            stream.WriteLine("\t\tFSM_INIT();");
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

            stream.WriteLine("\t\tFSM_INIT_STATE_EXPLICIT(" + state.name + ", " + 
                (initial ? "true, " : "false, ") +
                (state.NoEnter ? "NULL, " : "&" + ClassName + "::onEnter"+state.name+ ", ") +
                (state.NoExit ? "NULL, " : "&" + ClassName + "::onExit" + state.name + ", ") +
                (!update ? "NULL" : "&" + ClassName + "::update" + state.name) +
                ");");
            //if (update)
            //{
            //    stream.WriteLine("\t\tFSM_INIT_STATE_UPDATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") + ");");
            //}
            //else
            //{
            //    stream.WriteLine("\t\tFSM_INIT_STATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") + ");");
            //}

            stream.WriteLine("\t\t" + parent + ".addChild(" + state.name + ");");

            stream.WriteLine();
        
        }

        public virtual void VisitTestStatement(TestStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Test found outside of state block", test.line);

            if(test.NoExec)
                stream.WriteLine("\t\tFSM_INIT_INTERFACECOMMAND_NOEXEC(" + ClassName + ", " + GetState() + ", " + test.name + ");");
            else
                stream.WriteLine("\t\tFSM_INIT_INTERFACECOMMAND(" + ClassName + ", " + GetState() + ", " + test.name + ");");

            stream.WriteLine();

        }

        public virtual void VisitDenyStatement(DenyStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Test Deny found outside of state block", test.line);

            stream.WriteLine("\t\tFSM_INIT_INTERFACEDENY(" + ClassName + ", " + GetState() + ", " + test.name + ");");

            stream.WriteLine();

        }

        public virtual void VisitAllowStatement(AllowStatement test)
        {
            string state = GetState();
            if (state == null)
                throw new MalformedFSMException("Interface Test Allow found outside of state block", test.line);

            stream.WriteLine("\t\tFSM_INIT_INTERFACEALLOW(" + ClassName + ", " + GetState() + ", " + test.name + ");");

            stream.WriteLine();

        }

        public virtual void VisitTransitionStatement(TransitionStatement transition)
        {
            string state = GetState();

            if (transition.command == null)
            {
                if (state == null)
                    throw new MalformedFSMException("Interface Transition found outside of state block", transition.line);

                if(transition.NoExec)
                    stream.WriteLine("\t\tFSM_INIT_TRANSITION_NOEXEC(" + ClassName + ", " + GetState() + ", " + transition.targetstate + ");");
                else
                    stream.WriteLine("\t\tFSM_INIT_TRANSITION(" + ClassName + ", " + GetState() + ", " + transition.targetstate + ");");
            }
            else
            {
                if (state == null)
                    throw new MalformedFSMException("Interface Command found outside of state block", transition.line);

                if(transition.NoExec)
                    stream.WriteLine("\t\tFSM_INIT_INTERFACETRANSITION_NOEXEC(" + ClassName + ", " + GetState() + ", " + transition.command + ", " + transition.targetstate + ");");
                else
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
