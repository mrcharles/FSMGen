using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;

namespace FSMGen.Statements
{
    [Token("class")]
    class ClassStatement : NameStatement
    {

    }

    [Token("initial")]
    class InitialStatement : Statement
    {

    }

    [Token("update")]
    class UpdateStatement : Statement
    {

    }

    [Token("test")]
    class TestStatement : NameStatement
    {
        [Modifier("noexec")]
        public bool NoExec
        {
            get;
            set;
        }

    }

    [Token("deny")]
    class DenyStatement : NameStatement
    {
    
    }

    [Token("allow")]
    class AllowStatement : NameStatement
    { 
    
    }

    [Token("endstate")]
    [Token("endfsm")]
    class GenericPopStatement : Statement
    {
        public override bool ShouldPop()
        {
            return true;
        }
    }


}
