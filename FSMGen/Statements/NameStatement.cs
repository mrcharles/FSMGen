using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{
    public abstract class NameStatement : Statement
    {
        public string name;
        public override void Consume(Queue<string> tokens)
        {
            base.Consume(tokens);

            if (FSM.IsToken(tokens.Peek()))
            {
                throw new MalformedFSMException("Unexpected token: " + tokens.Peek() + ", expected identifier.", line);
            }

            name = tokens.Dequeue();

            base.Consume(tokens);

        }

    }

}
