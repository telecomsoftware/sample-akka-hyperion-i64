using System;

namespace SharedMessages
{
    public abstract class Messages
    {
        public class StartSending
        { }

        public class StreamComplete
        { }

        public class Hail
        { }

        public class Failure
        {
            public int ErrorCode { get; }
            public Exception Exception { get; }

            public Failure(int errorCode, Exception exception)
            {
                this.ErrorCode = errorCode;
                this.Exception = exception;
            }
        }
    }
}
