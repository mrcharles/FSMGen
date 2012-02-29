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
        public FSMVisitor(StreamWriter _stream)
            : base(_stream)
        { }

        //public virtual bool Valid(Statement s)
        //{
        //    if (s is ClassStatement)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public virtual void Visit(Statement s)
        //{
        //    if (s is ClassStatement)
        //    {
        //        ClassName = (s as ClassStatement).name;
        //    }
        //}
        public virtual void VisitClassStatement(ClassStatement s)
        {
            ClassName = s.name;
        }
    }
}
