using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FSMGen.Visitors;

namespace FSMGen
{
	class Program
	{
        static Config config = new Config();

        static bool ShouldExport(string file)
        {
            string fullname = Path.GetFileName(file);
            string name = Path.GetFileNameWithoutExtension(file);
            string path = Path.GetDirectoryName(file); //Path.GetFullPath(file);
            string outputfilename = Path.Combine(path, name + ".fsm.h");
            //for now, compare file dates and only export if the exported file is older (or doesn't exist)

            FileInfo orig = new FileInfo(file);
            FileInfo output = new FileInfo(outputfilename);

            if (!output.Exists)
                return true;
            else if (orig.LastWriteTimeUtc > output.LastWriteTimeUtc)
            {
                return true;
            }

            return false;
        }

		static bool ProcessFile(string file, FSMVisitor specialVisitor = null)
		{
			string fullname = Path.GetFileName(file);
			string name = Path.GetFileNameWithoutExtension(file);
            string path = Path.GetDirectoryName(file); //Path.GetFullPath(file);
            string outputfilename = Path.Combine(path, name + ".fsm.h");

			StreamReader reader = new StreamReader(file);

			FSM fsm = null;
			try
			{
				fsm = new FSM(reader);
			}
			catch (MalformedFSMException e)
			{
				//MessageBox.Show(e.Message, "FSMGen Failed: " + fullname);
                Console.WriteLine(file + "(" + e.line + ") : error : " + e.Message);
				return false;
			}
			finally
			{
				reader.Close();
			}

            if (specialVisitor != null)
            {
                fsm.AcceptVisitor(specialVisitor);
            }


			//the generated filename needs to be defined by info parsed from the .fsm file. TODO with language def.
			//or maybe fsm.Export can return a file name.
			StreamWriter writer = new StreamWriter(outputfilename, false);
			writer.AutoFlush = false;
			try
			{
				fsm.Export(writer, config);
			}
			catch (MalformedFSMException e)
			{
				//MessageBox.Show(e.Message, "FSMGen Failed:" + fullname);
                Console.WriteLine(file + "(" + e.line + ") : error : " + e.Message);
                writer.Dispose();
				return false;
			}
			writer.Flush();
			writer.Close();

            return true;
		}

		static int Main(string[] args)
		{
			//MessageBox.Show("startin asdfasdfsadfg");
            if (args.Length > 0)
            {
                if (ShouldExport(args[0]))
                {
                    GlobalCommandVisitor commands = new GlobalCommandVisitor(Path.Combine(Path.GetDirectoryName(config.configFile), config.Data.commandheaderfile), Path.Combine(Path.GetDirectoryName(config.configFile), config.Data.commandsdb));
                    commands.Init();

                    if (!ProcessFile(args[0], commands))
                        Environment.Exit(1);

                    commands.End();
                }
            }

			//foreach (string s in args)
			//{
			//    //MessageBox.Show(s);
			//}

            return 0;
		}
	}
}
