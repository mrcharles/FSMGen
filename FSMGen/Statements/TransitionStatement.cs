using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;

namespace FSMGen.Statements
{
    [Token("transition")]
    public class TransitionStatement : Statement
    {
        public string targetstate;
        public string command;

        [Modifier("allow")]
        public bool Allow
        {
            get;
            set;
        }

        [Modifier("deny")]
        public bool Deny
        {
            get;
            set;
        }

        [Modifier("noexec")]
        public bool NoExec
        {
            get;
            set;
        }


        public override void Consume(Queue<string> tokens)
        {
            base.Consume(tokens);
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
