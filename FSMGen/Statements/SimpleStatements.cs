using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSMGen.Attributes;

namespace FSMGen.Statements
{
    [Token("class")]
    public class ClassStatement : NameStatement
    {

    }

    [Token("initial")]
    public class InitialStatement : Statement
    {

    }

    [Token("update")]
    public class UpdateStatement : Statement
    {

    }

    [Token("test")]
    public class TestStatement : NameStatement
    {
        [Modifier("noexec")]
        public bool NoExec
        {
            get;
            set;
        }

    }


    [Token("deny")]
    public class DenyStatement : NameStatement
    {
    
    }

    [Token("allow")]
    public class AllowStatement : NameStatement
    { 
    
    }

    [Token("endstate")]
    [Token("endfsm")]
    public class GenericPopStatement : Statement
    {
        public override bool ShouldPop()
        {
            return true;
        }
    }


}
