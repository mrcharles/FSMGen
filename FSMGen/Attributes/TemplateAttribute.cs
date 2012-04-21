using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TemplateAttribute : System.Attribute
    {
        public string name { get; set; }
        public TemplateAttribute(string name)
        {
            this.name = name;
        }
    }
}
