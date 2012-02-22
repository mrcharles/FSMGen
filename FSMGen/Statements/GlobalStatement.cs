using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{
    class GlobalStatement : Statement
    {
        List<Statement> statements = new List<Statement>();

        public override List<Statement> Statements()
        {
            return statements;
        }

        public override bool ShouldPush()
        {
            return true;
        }
    }

}
