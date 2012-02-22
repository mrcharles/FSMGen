using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{
    abstract class NameStatement : Statement
    {
        public string name;
        public override void Consume(Queue<string> tokens)
        {
            if (FSM.IsToken(tokens.Peek()))
            {
                throw new MalformedFSMException("Unexpected token: " + tokens.Peek() + ", expected identifier.");
            }

            name = tokens.Dequeue();
        }

    }

}
