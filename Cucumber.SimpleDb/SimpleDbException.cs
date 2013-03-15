using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb
{
    public class SimpleDbException : Exception
    {
        public SimpleDbException(string message)
            : base(message)
        {
        }

        public SimpleDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
