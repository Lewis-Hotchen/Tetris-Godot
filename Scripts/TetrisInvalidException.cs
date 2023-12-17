using System;
using System.Runtime.Serialization;

[Serializable]
internal class TetrisInvalidException : Exception
{
    public TetrisInvalidException()
    {
    }

    public TetrisInvalidException(string message) : base(message)
    {
    }

    public TetrisInvalidException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TetrisInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}