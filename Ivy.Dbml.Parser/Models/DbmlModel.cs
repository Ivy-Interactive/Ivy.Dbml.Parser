using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class DbmlModel
{
    public string ProjectName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<Table> Tables { get; set; } = new();
    public List<Reference> References { get; set; } = new();
    public List<DbmlEnum> Enums { get; set; } = new();
    public List<TableGroup> TableGroups { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
    public string? DatabaseType { get; set; }
    public string? Language { get; set; }
    public string? Schema { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
} 