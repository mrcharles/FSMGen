using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Visitors;

namespace FSMGen.Statements
{
    abstract class Statement
    {
        public FSM owner;
        public int line;

        public virtual void Consume(Queue<string> tokens) { }
        public virtual bool ShouldPush() { return false; }
        public virtual bool ShouldPop() { return false; }
        public virtual List<Statement> Statements() { return null; }
        public bool HasStatement(Type _type)
        {
            List<Statement> list = Statements();
            if (list != null)
            {
                foreach (Statement s in list)
                {
                    if (s.GetType().Equals(_type))
                        return true;
                }
            }
            return false;
        }

        public void AcceptVisitor(FSMVisitor visitor)
        {
            if (visitor.Valid(this))
            {
                visitor.Visit(this);
            }
            if (Statements() != null)
            {
                foreach (Statement s in Statements())
                {
                    s.AcceptVisitor(visitor);
                }
            }
        }

    }
}
