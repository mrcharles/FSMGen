using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{
    class InterfaceCommandStatement : NameStatement
    {
        public override void Consume(Queue<string> tokens)
        {
            base.Consume(tokens);

            owner.Commands.Add(name);
        }
    }

}
