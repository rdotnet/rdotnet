using System;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(ParseStatus status, string statement, string message)
            : base(message)
        {
            Status = status;
            Statement = statement;
        }

        public ParseException(ParseStatus status, string statement)
        {
            Status = status;
            Statement = statement;
        }

        public ParseStatus Status { get; private set; }
        public string Statement { get; private set; }
    }
}
