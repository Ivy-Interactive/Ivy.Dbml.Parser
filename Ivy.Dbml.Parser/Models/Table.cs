using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class Table
{
    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? Note { get; set; }
    public TableType Type { get; set; }
    public List<Column> Columns { get; set; } = new List<Column>();
    public List<Index> Indexes { get; set; } = new List<Index>();
    public string? Schema { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
}

public enum TableType
{
    Normal,
    View
} 