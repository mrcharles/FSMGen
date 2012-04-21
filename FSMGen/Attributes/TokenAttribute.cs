using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Attributes
{                       
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false )]
    public class TokenAttribute : System.Attribute
    {
        public string id { get; set; }
        public TokenAttribute(string id)
        {
            this.id = id;
        }
    }
}
