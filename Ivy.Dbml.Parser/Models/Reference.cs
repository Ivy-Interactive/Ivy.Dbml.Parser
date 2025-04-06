namespace Ivy.Dbml.Parser.Models;

public class Reference
{
    public string Name { get; set; } = string.Empty;
    public string FromTable { get; set; } = string.Empty;
    public string FromColumn { get; set; } = string.Empty;
    public string ToTable { get; set; } = string.Empty;
    public string ToColumn { get; set; } = string.Empty;
    public string? Note { get; set; }
    public ReferenceType Type { get; set; }
}

public enum ReferenceType
{
    OneToOne,
    OneToMany,
    ManyToOne,
    ManyToMany
} 