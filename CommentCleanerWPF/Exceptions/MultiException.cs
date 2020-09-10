using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CommentCleanerWPF.Exceptions
{
    public class MultiException : Exception
    {
        public IEnumerable<Exception> Exceptions { get; set; }
        public MultiException( ) { }

        public MultiException( string message ) : base(message) { }

        public MultiException( IEnumerable<Exception> exceptions ) => Exceptions = exceptions;

        public MultiException( string message, IEnumerable<Exception> exceptions ) : base(message) => Exceptions = exceptions;

        public MultiException( string message, Exception innerException ) : base(message, innerException) { }

        public MultiException( string message, IEnumerable<Exception> exceptions, Exception innerException ) : base(message, innerException) => Exceptions = exceptions;
    }
}
