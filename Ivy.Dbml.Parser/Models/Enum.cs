using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class DbmlEnum
{
    public string Name { get; set; } = string.Empty;
    public string? Schema { get; set; }
    public string? Note { get; set; }
    public List<string> Values { get; set; } = new();
    public Dictionary<string, string> ValueNotes { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
} 