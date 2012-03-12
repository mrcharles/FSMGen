using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;

namespace FSMGen.Statements
{
    [Token("transition")]
    [Modifier("allow")]
    [Modifier("noexec")]
    class TransitionStatement : Statement
    {
        public string targetstate;
        public string command;

        public bool Allow
        {
            get;
            set;
        }

        public bool NoExec
        {
            get;
            set;
        }


        public override void Consume(Queue<string> tokens)
        {
            if (FSM.IsToken(tokens.Peek()))
                throw new MalformedFSMException("Unexpected token " + tokens.Peek() + ", expected identifier or interface command.", line);

            if (owner.Commands.Contains(tokens.Peek())) //this is an interfacecommand transition
            {
                command = tokens.Dequeue();
            }

            if (FSM.IsToken(tokens.Peek()))
            {
                throw new MalformedFSMException("Unexpected token " + tokens.Peek() + ", expected identifier.", line);
            }
            targetstate = tokens.Dequeue();
        }
    }

}
