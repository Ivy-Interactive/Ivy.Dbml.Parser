using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class TableGroup
{
    public string Name { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<string> Tables { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
} 