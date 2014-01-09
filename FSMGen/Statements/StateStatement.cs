using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;

namespace FSMGen.Statements
{

    [Token("state")]
    public class StateStatement : NameStatement
    {
        List<Statement> statements = new List<Statement>();

        public StateStatement()
        { 
        
        }

        [Modifier("noenter")]
        public bool NoEnter
        {
            get;
            set;
        }

        [Modifier("noexit")]
        public bool NoExit
        {
            get;
            set;
        }

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
