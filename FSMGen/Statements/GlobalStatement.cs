using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;


namespace FSMGen.Statements
{
    [Token("startfsm")]
    public class GlobalStatement : Statement
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
