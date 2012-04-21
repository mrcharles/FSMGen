using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    public abstract class BaseVisitor
    { 
        protected Config config;
        protected FSMFile fsmfile;
        public BaseVisitor(Config _config, FSMFile _file)
        {
            config = _config;
            fsmfile = _file;
        }
        public abstract void Init();
        public void Visit(Statement s)
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
