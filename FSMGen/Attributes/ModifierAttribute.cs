﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ModifierAttribute : Attribute
    {
        public string id { get; set; }
        public ModifierAttribute(string id)
        {
            this.id = id;
        }
    }
}
