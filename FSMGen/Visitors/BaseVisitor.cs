using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    abstract class BaseVisitor
    { 
        protected StreamWriter stream;
        public BaseVisitor(StreamWriter _stream)
        {
            stream = _stream;
        }
        public abstract void Init();
        public virtual void Visit(Statement s)
        {
            string statementtypename = s.GetType().Name;

            string funcname = "Visit" + statementtypename;

            MethodInfo mi = this.GetType().GetMethod(funcname);

            if (mi != null)
            {
                mi.Invoke(this, new object[] { s });
            }
        }
        public abstract void End();

    }
}
