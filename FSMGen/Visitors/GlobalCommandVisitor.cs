using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FSMGen.Attributes;

namespace FSMGen.Visitors
{
    [TemplateAttribute("globalcommands.template")]
    class GlobalCommandVisitor : FSMVisitor
    {
        HashSet<string> commands;
        string dbfile;

        public GlobalCommandVisitor(string outputfile, string _dbfile)
            : base(new StreamWriter(outputfile))
        {
            dbfile = _dbfile;
        }

        public override void Init()
        {

            Stream stream = null;
            try
            {
                stream = File.Open(dbfile, FileMode.Open);
                IFormatter formatter = new BinaryFormatter();

                commands = formatter.Deserialize(stream) as HashSet<string>;
            }
            catch (Exception)
            {
                commands = new HashSet<string>();
            }
            finally 
            {
                if (stream != null)
                    stream.Close();
            }
        }

        //public override bool Valid(Statement s)
        //{
        //    if (s is InterfaceCommandStatement)
        //    {
        //        return true;
        //    }

        //    return base.Valid(s);
        //}

        public virtual void VisitInterfaceCommandStatement(InterfaceCommandStatement command)
        {
            commands.Add(command.name);
        }

        public override void End()
        {
            //save out the hashset

            Stream dbstream = File.Open(dbfile, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();

            formatter.Serialize(dbstream, commands);
            dbstream.Close();

            stream.WriteLine("#ifndef INTERFACECOMMANDS_H");
            stream.WriteLine("#define INTERFACECOMMANDS_H");
            stream.WriteLine();
            stream.WriteLine("namespace InterfaceCommands");
            stream.WriteLine("{");
            stream.WriteLine("\tenum Enum");
            stream.WriteLine("\t{");

            foreach (string s in commands)
            {
                stream.WriteLine("\t\t" + s + ",");
            }

            stream.WriteLine("\t};");
            stream.WriteLine("}");

            stream.WriteLine("#endif // INTERFACECOMMANDS_H");

            stream.Close();
        }
    }
}
