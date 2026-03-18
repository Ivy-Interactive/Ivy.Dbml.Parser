using System.Collections.Generic;
using Ivy.Dbml.Parser.Models;

namespace Ivy.Dbml.Parser.Parser;

public class DbmlParseResult
{
    public bool IsValid => Errors.Count == 0;
    public DbmlModel? Model { get; init; }
    public List<DbmlError> Errors { get; init; } = [];
}

public class DbmlError
{
    public int Line { get; init; }
    public string Message { get; init; } = "";
    public DbmlErrorSeverity Severity { get; init; }

    public override string ToString() => $"Line {Line}: {Message}";
}

public enum DbmlErrorSeverity { Error, Warning }
