using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    public abstract class StateVisitor : FSMVisitor
    {
        Stack<string> statenames = new Stack<string>();

        public StateVisitor(Config config, FSMFile file)
            : base(config, file)
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
        //public override bool Valid(Statement s)
        //{
        //    if (s is StateStatement || s is GenericPopStatement)
        //        return true;

        //    return base.Valid(s);
        //}

        public virtual void VisitStateStatement(StateStatement state)
        {
            statenames.Push(state.name);
        }

        public virtual void VisitGenericPopStatement(GenericPopStatement s)
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
    }
}
