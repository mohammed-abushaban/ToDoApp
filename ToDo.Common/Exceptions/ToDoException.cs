using System;

namespace ToDo.Common.Extceptions
{
    public class ToDoException : Exception
    {
        public int ErrorCode { get; set; }

        public ToDoException() : base("ToDo Exception")
        {
        }

        public ToDoException(string message) : base(message)
        {
        }

        public ToDoException(int statusCode, string message) : base(message)
        {
            ErrorCode = statusCode;
        }

        public ToDoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ToDoException(int statusCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = statusCode;
        }
    }
}
