using System;

namespace test_dbms.src.tx.cocurrency
{
    [Serializable]
    class LockAbortException : Exception
    {
        public LockAbortException() { }

        public LockAbortException(string message) : base(message) { }

        public LockAbortException(string message, Exception inner) : base(message, inner) { }

        protected LockAbortException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}
