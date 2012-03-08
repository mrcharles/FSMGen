using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    }

    [Token("deny")]
    class DenyStatement : NameStatement
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
