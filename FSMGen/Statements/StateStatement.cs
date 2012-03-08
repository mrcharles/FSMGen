using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{

    [Token("state")]
    class StateStatement : NameStatement
    {
        List<Statement> statements = new List<Statement>();

        //[TemplateToken("state")]
        public string State
        {
            get
            { return name; }
        }

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
