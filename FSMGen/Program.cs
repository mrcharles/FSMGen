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
		static void Main(string[] args)
		{
			StreamReader reader = new StreamReader("fsminput.fsm");

			FSM fsm = null;
			try
			{
				fsm = new FSM(reader.ReadToEnd());
			}
			catch (MalformedFSMException e)
			{
				MessageBox.Show(e.Message, "FSMGen Failed:");
				return;
			}
			finally
			{
				reader.Close();
			}

			StreamWriter writer = new StreamWriter("fsmexport.out", false);
			writer.AutoFlush = false;
			try
			{
				fsm.Export(writer);
			}
			catch (MalformedFSMException e)
			{
				MessageBox.Show(e.Message, "FSMGen Failed:");
				writer.Dispose();
				return;
			}
			writer.Flush();
			writer.Close();



			
		}
	}
}
