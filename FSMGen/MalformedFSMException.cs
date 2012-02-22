using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen
{
    class MalformedFSMException : ApplicationException
    {
        public MalformedFSMException() { }

        public MalformedFSMException(string message)
            : base(message)
        { }

        public MalformedFSMException(string message, System.Exception inner)
            : base(message, inner)
        { }

        protected MalformedFSMException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }

    }

}
