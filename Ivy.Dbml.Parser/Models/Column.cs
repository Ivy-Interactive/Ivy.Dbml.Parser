namespace Ivy.Dbml.Parser.Models;

public class Column
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsNotNull { get; set; }
    public bool IsUnique { get; set; }
    public string? DefaultValue { get; set; }
    public Reference? Reference { get; set; }
    public bool IsIncrement { get; set; }
    public bool IsUnsigned { get; set; }
    public int? Length { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
} 