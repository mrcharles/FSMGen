using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FSMGen
{
	class Program
	{

		static void ProcessFile(string file)
		{
			string fullname = Path.GetFileName(file);
			string name = Path.GetFileNameWithoutExtension(file);
            string path = Path.GetDirectoryName(file); //Path.GetFullPath(file);

			StreamReader reader = new StreamReader(file);

			FSM fsm = null;
			try
			{
				fsm = new FSM(reader.ReadToEnd());
			}
			catch (MalformedFSMException e)
			{
				MessageBox.Show(e.Message, "FSMGen Failed: " + fullname);
				return;
			}
			finally
			{
				reader.Close();
			}

			//the generated filename needs to be defined by info parsed from the .fsm file. TODO with language def.
			//or maybe fsm.Export can return a file name.
			StreamWriter writer = new StreamWriter(Path.Combine(path, name + ".fsm.h"), false);
			writer.AutoFlush = false;
			try
			{
				fsm.Export(writer);
			}
			catch (MalformedFSMException e)
			{
				MessageBox.Show(e.Message, "FSMGen Failed:" + fullname);
				writer.Dispose();
				return;
			}
			writer.Flush();
			writer.Close();
		
		}

		static void Main(string[] args)
		{
			//MessageBox.Show("startin asdfasdfsadfg");
            if (args.Length > 0)
                ProcessFile(args[0]);

			//foreach (string s in args)
			//{
			//    //MessageBox.Show(s);
			//}

			
		}
	}
}
