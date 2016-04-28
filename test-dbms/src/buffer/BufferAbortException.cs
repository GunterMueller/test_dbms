using System;

namespace test_dbms.src.buffer
{
    [Serializable]
    class BufferAbortException : Exception
    {
        public BufferAbortException() { }

        public BufferAbortException(string message, Exception inner) : base(message, inner) { }

        protected BufferAbortException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}
