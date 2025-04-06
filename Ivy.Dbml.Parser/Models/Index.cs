using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class Index
{
    public string Name { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public List<string> Expressions { get; set; } = new();
    public bool IsUnique { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string? Type { get; set; }
    public string? Note { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
} 