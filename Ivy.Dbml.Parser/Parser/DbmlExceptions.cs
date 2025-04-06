using System;

namespace Ivy.Dbml.Parser.Parser
{
    // Custom exception classes for parsing errors
    public class DbmlParsingException : Exception
    {
        public DbmlParsingException(string message) : base(message) { }
    }

    public class InvalidSyntaxException : DbmlParsingException
    {
        public InvalidSyntaxException(string message) : base(message) { }
    }

    public class MissingElementException : DbmlParsingException
    {
        public MissingElementException(string message) : base(message) { }
    }
} 