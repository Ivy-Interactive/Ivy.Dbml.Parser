using System.Collections.Generic;

namespace Ivy.Dbml.Parser.Models;

public class Reference
{
    public string Name { get; set; } = string.Empty;
    public string? FromSchema { get; set; }
    public string FromTable { get; set; } = string.Empty;
    public string FromColumn { get; set; } = string.Empty;
    public List<string> FromColumns { get; set; } = new();
    public string? ToSchema { get; set; }
    public string ToTable { get; set; } = string.Empty;
    public string ToColumn { get; set; } = string.Empty;
    public List<string> ToColumns { get; set; } = new();
    public string? Note { get; set; }
    public ReferenceType Type { get; set; }
    public bool IsCompositeKey { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
}

public enum ReferenceType
{
    OneToOne,
    OneToMany,
    ManyToOne,
    ManyToMany
} 