using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FSMGen.Statements;

namespace FSMGen.Visitors
{
    public class DefinitionVisitor : StateVisitor
    {
        string cpp;

        FileStream cppfile;
        StreamWriter stream;

        public DefinitionVisitor( Config config, FSMFile file )
            : base( config, file )
        {
            cppfile = new FileStream(file.DefinitionFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            cpp = new StreamReader(cppfile).ReadToEnd();
        }

        public override void Init()
        {
        }

        void PrintFunc( string ret, string funcdef, string param, string retstatement="", string lame="")
        {
            if (!cpp.Contains(funcdef))
            {
                if (stream == null)
                {
                    stream = new StreamWriter(cppfile);
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine();
                }
                stream.WriteLine(ret + " " + funcdef + param);
                stream.WriteLine("{");
                if(lame != "") 
                    stream.WriteLine(lame);
                stream.WriteLine(retstatement);
                stream.WriteLine("}");
                stream.WriteLine();
            }
        }


        public override void VisitStateStatement(StateStatement state)
        {
            base.VisitStateStatement(state);
            //make string "ClassName::onEnterStateName" to search CPP 
            if(!state.NoEnter)
                PrintFunc("void", ClassName + "::onEnter" + state.name, "()");

            //make string "ClassName::onExitStateName" to search CPP 
            if (!state.NoExit)
                PrintFunc("void", ClassName + "::onExit" + state.name, "()");

            if (state.HasStatement(typeof(UpdateStatement)))
            {
                PrintFunc("void", ClassName + "::update" + state.name, "(float dt)", "\tdt;");
            }
        }

        public virtual void VisitTestStatement(TestStatement test)
        {
            PrintFunc("InterfaceResult::Enum", ClassName + "::test" + GetState() + "On" + test.name, "(InterfaceParam* param)", "\treturn InterfaceResult::Unhandled;", "\tparam;");
            if( !test.NoExec)
                PrintFunc("void", ClassName + "::exec" + GetState() + "On" + test.name, "(InterfaceParam* param)", "\tparam;");
        }

        public virtual void VisitTransitionStatement(TransitionStatement transition)
        {
            if (transition.command == null)
            {
                if( !transition.Allow && !transition.Deny)
                    PrintFunc("bool", ClassName + "::test" + GetState() + "To" + transition.targetstate, "()", "\treturn false;");
                if( !transition.NoExec)
                    PrintFunc("void", ClassName + "::exec" + GetState() + "To" + transition.targetstate, "()");
            }
            else
            {
                if (!transition.Allow && !transition.Deny)
                    PrintFunc("InterfaceResult::Enum", ClassName + "::test" + GetState() + "To" + transition.targetstate + "On" + transition.command, "(InterfaceParam* param)", "\treturn InterfaceResult::Unhandled;", "\tparam;");
                if (!transition.NoExec)
                    PrintFunc("void", ClassName + "::exec" + GetState() + "To" + transition.targetstate + "On" + transition.command, "(InterfaceParam* param)", "\tparam;");
            }
        }

        public override void End()
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close();
            }
            cppfile.Close();
        }
    }
}
