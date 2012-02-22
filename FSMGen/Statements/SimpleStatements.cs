using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Statements
{
    class ClassStatement : NameStatement
    {

    }

    class InitialStatement : Statement
    {

    }

    class UpdateStatement : Statement
    {

    }

    class TestStatement : NameStatement
    {

    }

    class GenericPopStatement : Statement
    {
        public override bool ShouldPop()
        {
            return true;
        }
    }


}
