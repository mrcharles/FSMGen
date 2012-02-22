using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    abstract class StateVisitor : FSMVisitor
    {
        Stack<string> statenames = new Stack<string>();

        public StateVisitor(StreamWriter stream)
            : base(stream)
        {

        }
        public string GetState()
        {
            return statenames.Peek();
        }
        public string GetParent()
        {
            string current = statenames.Pop();
            string parent = null;
            try
            {
                parent = statenames.Peek();
            }
            finally
            {
                statenames.Push(current);
            }
            return parent;
        }
        public override bool Valid(Statement s)
        {
            if (s is StateStatement || s is GenericPopStatement)
                return true;

            return base.Valid(s);
        }

        public override void Visit(Statement s)
        {
            if (s is StateStatement)
            {
                StateStatement state = s as StateStatement;
                statenames.Push(state.name);
            }
            if (s is GenericPopStatement)
            {
                try
                {
                    statenames.Pop();
                }
                catch (Exception)
                {
                    throw new MalformedFSMException("Unexpected EOF. Unterminated state declaration?", s.line);
                }
            }

            base.Visit(s);
        }

    }
}
