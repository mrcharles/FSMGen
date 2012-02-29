using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FSMGen.Statements;

namespace FSMGen.Visitors
{
    class DefinitionVisitor : StateVisitor
    {
        string cpp;

        FileStream cppfile;

        public DefinitionVisitor( FileStream _cppfile )
            : base( new StreamWriter(_cppfile) )
        {
            cppfile = _cppfile;
        }

        public DefinitionVisitor(string outputfile)
            : this( new FileStream( outputfile, FileMode.OpenOrCreate, FileAccess.ReadWrite ) )
        {
            //it's probably better to build a regex to parse out all function names to check against as a hashset.
            //but for now, this is easy.
            cpp = new StreamReader(cppfile).ReadToEnd();
        }

        public override void Init()
        {
        }

        bool firstexport = true; //<---- gross

        void PrintFunc( string ret, string funcdef, string param, string retstatement="", string lame="")
        {
            if (!cpp.Contains(funcdef))
            {
                if (firstexport)
                {
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine("////////////////////////////////////////////////////////////////////////////////////");
                    stream.WriteLine();

                    firstexport = false;
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


        public virtual void VisitStateStatement(StateStatement state)
        {
            //make string "ClassName::onEnterStateName" to search CPP 
            PrintFunc("void", ClassName + "::onEnter" + state.name, "()");

            //make string "ClassName::onExitStateName" to search CPP 
            PrintFunc("void", ClassName + "::onExit" + state.name, "()");

            if (state.HasStatement(typeof(UpdateStatement)))
            {
                PrintFunc("void", ClassName + "::update" + state.name, "(float dt)", "\tdt;");
            }
        }

        public virtual void VisitTestStatement(TestStatement test)
        {
            PrintFunc("InterfaceResult::Enum", ClassName + "::test" + GetState() + "On" + test.name, "(InterfaceParam* param)", "\treturn InterfaceResult::Unhandled;", "\tparam;");
            PrintFunc("void", ClassName + "::exec" + GetState() + "On" + test.name, "(InterfaceParam* param)", "\tparam;");
        }

        public virtual void VisitTransitionStatement(TransitionStatement transition)
        {
            if (transition.command == null)
            {
                PrintFunc("bool", ClassName + "::test" + GetState() + "To" + transition.targetstate, "()", "\treturn false;");
                PrintFunc("void", ClassName + "::exec" + GetState() + "To" + transition.targetstate, "()");
            }
            else
            {
                PrintFunc("InterfaceResult::Enum", ClassName + "::test" + GetState() + "To" + transition.targetstate + "On" + transition.command, "(InterfaceParam* param)", "\treturn InterfaceResult::Unhandled;", "\tparam;");
                PrintFunc("void", ClassName + "::exec" + GetState() + "To" + transition.targetstate + "On" + transition.command, "(InterfaceParam* param)", "\tparam;");
            }
        }

        public override void End()
        {
            stream.Flush();
            stream.Dispose();
            cppfile.Close();
        }
    }
}
