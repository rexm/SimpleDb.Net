using System;

namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Wrapper for exceptions that occur in the SimpleDb layer
    /// For explicit catch blocks when needed
    /// </summary>
    public class SimpleDbException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cucumber.SimpleDb.SimpleDbException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public SimpleDbException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cucumber.SimpleDb.SimpleDbException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="inner">Inner.</param>
        public SimpleDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}