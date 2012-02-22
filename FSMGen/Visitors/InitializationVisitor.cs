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
        public InitializationVisitor(StreamWriter stream)
            : base(stream)
        { }

        public override void Init()
        {
            stream.WriteLine("\tvoid InitializeFSM()");
            stream.WriteLine("\t{");
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
                throw new MalformedFSMException("No class statement found before state implementation.");
            if (s is ClassStatement)
            {
                stream.WriteLine("\t\tFSM_INIT(" + ClassName + ");");
                stream.WriteLine();
            }
            if (s is StateStatement)
            {
                StateStatement state = s as StateStatement;

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
            if (s is TestStatement)
            {
                TestStatement test = s as TestStatement;
                string state = GetState();
                if (state == null)
                    throw new MalformedFSMException("Interface Test found outside of state block");

                stream.WriteLine("\t\tFSM_INIT_INTERFACECOMMAND(" + ClassName + ", " + GetState() + ", " + test.name + ");");

                stream.WriteLine();
            }
            if (s is TransitionStatement)
            {
                TransitionStatement transition = s as TransitionStatement;

                string state = GetState();

                if (transition.command == null)
                {
                    if (state == null)
                        throw new MalformedFSMException("Interface Transition found outside of state block");

                    stream.WriteLine("\t\tFSM_INIT_TRANSITION(" + ClassName + ", " + GetState() + ", " + transition.targetstate + ");");
                }
                else
                {
                    if (state == null)
                        throw new MalformedFSMException("Interface Command found outside of state block");

                    stream.WriteLine("\t\tFSM_INIT_INTERFACETRANSITION(" + ClassName + ", " + GetState() + ", " + transition.command + ", " + transition.targetstate + ");");
                }

                stream.WriteLine();
            }
        }

        public override void End()
        {
            stream.WriteLine("\t}");
        }
    }

}
