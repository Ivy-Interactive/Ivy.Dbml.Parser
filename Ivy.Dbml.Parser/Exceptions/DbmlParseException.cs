using System;

namespace Ivy.Dbml.Parser.Exceptions;

public class DbmlParseException : Exception
{
    public int LineNumber { get; }

    public DbmlParseException(string message, int lineNumber) 
        : base($"Error parsing DBML at line {lineNumber}: {message}")
    {
        LineNumber = lineNumber;
    }

    public DbmlParseException(string message, int lineNumber, Exception innerException)
        : base($"Error parsing DBML at line {lineNumber}: {message}", innerException)
    {
        LineNumber = lineNumber;
    }
} 