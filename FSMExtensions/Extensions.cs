using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FSMGen;
using FSMGen.Statements;
using FSMGen.Attributes;

namespace FSMExtensions
{
    [Token("timer")]
    public class TimerStatement : NameStatement
    {
    }

    [Token("handletimer")]
    public class HandleTimerStatement : NameStatement
    { 
    
    }

    public class DeclarationVisitor : FSMGen.Visitors.DeclarationVisitor
    {
        public DeclarationVisitor(Config config, FSMFile file)
            : base(config, file)
        { 
        
        }

        public virtual void VisitTimerStatement(TimerStatement s)
        { 
        
        }

        public virtual void VisitHandlerTimerStatement(HandleTimerStatement s)
        { 
        
        }
    }


}
