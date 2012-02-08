using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSMGen
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader reader = new StreamReader("fsminput.fsm");

			FSM fsm = new FSM(reader.ReadToEnd());
		}
	}
}
