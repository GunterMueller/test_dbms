using System;

namespace test_dbms.src.parse
{
    [Serializable]
    public class BadSyntaxException : Exception
    {
        public BadSyntaxException() { }

        public BadSyntaxException(string message) : base(message) { }

        public BadSyntaxException(string message, Exception inner) : base(message, inner) { }

        protected BadSyntaxException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}
