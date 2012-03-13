using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FSMGen.Visitors;
using FSMGen.Attributes;


namespace FSMGen.Statements
{
    abstract class Statement
    {
        public FSM owner;
        public int line;

        public virtual void Consume(Queue<string> tokens) 
        {
            if (tokens.Count <= 0)
                return;
            while (tokens.Peek().StartsWith("+"))
                HandleModifier(tokens.Dequeue());
        }
        public void HandleModifier(string _modifier)
        {
            string modifier = _modifier.Trim().TrimStart(new char[] { '+' });
            bool handled = false;
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                foreach (ModifierAttribute mod in prop.GetCustomAttributes(typeof(ModifierAttribute), true))
                {
                    if (mod.id == modifier)
                    {
                        MethodInfo method = prop.GetSetMethod();

                        method.Invoke(this, new object[] { true });
                        handled = true;
                    }
                }
            }

            if (!handled)
                throw new MalformedFSMException("Encountered invalid statement modifier \"" + _modifier + "\"", line);
        }
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

        public void AcceptVisitor(BaseVisitor visitor)
        {
            visitor.Visit(this);

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
