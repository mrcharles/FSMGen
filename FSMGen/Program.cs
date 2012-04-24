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
        static Config config;

        static bool ShouldExport(string file)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                return true;

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

		static bool ProcessFile(string file)
		{
            FSMFile fsmfile = new FSMFile(file, config);

			FSM fsm = null;
			try
			{
				fsm = new FSM(fsmfile);
			}
			catch (MalformedFSMException e)
			{
				//MessageBox.Show(e.Message, "FSMGen Failed: " + fullname);
                Console.WriteLine(file + "(" + e.line + ") : error : " + e.Message);
				return false;
			}

			try
			{
				fsm.Export(config);
			}
			catch (MalformedFSMException e)
			{
				//MessageBox.Show(e.Message, "FSMGen Failed:" + fullname);
                Console.WriteLine(file + "(" + e.line + ") : error : " + e.Message);
				return false;
			}

            return true;
		}

		static int Main(string[] args)
		{
			//MessageBox.Show("startin asdfasdfsadfg");
            if (args.Length > 0)
            {
                try
                {

                    int arg = 0;
                    if (Path.GetExtension(args[arg]) == ".config")
                    {
                        config = new Config(args[arg]);
                        arg++;
                    }
                    else
                    {
                        config = new Config();
                    }

                    if (ShouldExport(args[arg]))
                    {
                        if (!ProcessFile(args[arg]))
                            Environment.Exit(1);
                    }
                }
                catch (InvalidConfigException e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
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
