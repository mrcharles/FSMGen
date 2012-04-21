using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FSMGen.Attributes;

namespace FSMGen.Statements
{
    [Token("interfacecommand")]
    public class InterfaceCommandStatement : NameStatement
    {
        public override void Consume(Queue<string> tokens)
        {
            base.Consume(tokens);

            owner.Commands.Add(name);
        }
    }

}
