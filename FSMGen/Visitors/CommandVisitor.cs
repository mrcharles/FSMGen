using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    public class CommandsVisitor : FSMVisitor
    {
        int commandIndex = 0;
        StreamWriter stream;

        public CommandsVisitor(Config config, FSMFile file)
            : base(config, file)
        { }
        ~CommandsVisitor()
        {
            if (stream != null)
                stream.Dispose();
        }

        public override void Init()
        {
            stream = new StreamWriter(fsmfile.ImplementationFile);
            stream.AutoFlush = false;

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

            stream.Flush();
            stream.Close();
            stream = null;
        }
    }
}
