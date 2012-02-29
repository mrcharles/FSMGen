using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    class CommandsVisitor : FSMVisitor
    {
        int commandIndex = 0;
        public CommandsVisitor(StreamWriter _stream)
            : base(_stream)
        { }
        public override void Init()
        {
            stream.WriteLine("public:");
            stream.WriteLine("\tenum InterfaceCommands");
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

        public virtual void VisitInterfaceCommandStatement( InterfaceCommandStatement command)
        {
            if (ClassName == null)
                throw new MalformedFSMException("Encountered interfacecommand directive before class directive.", command.line);

            stream.WriteLine("\t\t" + command.name + " = " + commandIndex + ",");
            commandIndex++;
        }

        public override void End()
        {
            stream.WriteLine("\t};");
            stream.WriteLine();
            stream.WriteLine();
        }
    }
}
