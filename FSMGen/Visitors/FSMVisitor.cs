using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FSMGen.Statements;

namespace FSMGen.Visitors
{
    abstract class FSMVisitor : BaseVisitor
    {
        public string ClassName = null;

        public FSMVisitor(Config config, FSMFile file)
            : base(config, file)
        { 
        
        }

        public virtual void VisitClassStatement(ClassStatement s)
        {
            ClassName = s.name;
        }
    }
}
