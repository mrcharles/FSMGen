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

        public override bool Valid(Statement s)
        {
            if (s is InterfaceCommandStatement)
            {
                return true;
            }

            return base.Valid(s);
        }

        public override void Visit(Statement s)
        {
            if (s is InterfaceCommandStatement)
            {
                if (ClassName == null)
                    throw new MalformedFSMException("Encountered interfacecommand directive before class directive.");

                InterfaceCommandStatement command = s as InterfaceCommandStatement;

                stream.WriteLine("\t\t" + command.name + " = " + commandIndex + ",");
                commandIndex++;
            }
            else
            {
                base.Visit(s);
            }
        }

        public override void End()
        {
            stream.WriteLine("\t};");
            stream.WriteLine();
            stream.WriteLine();
        }
    }
}
