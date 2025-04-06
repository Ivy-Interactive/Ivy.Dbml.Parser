using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ivy.Dbml.Parser.Models;

namespace Ivy.Dbml.Parser.Parser;

public class DbmlParser
{
    private DbmlModel _model = new();
    private Table? _currentTable;
    private bool _parsingIndexes;

    public DbmlModel Parse(string content)
    {
        _currentTable = null;
        _parsingIndexes = false;
        _model = new DbmlModel();

        var lines = content.Split('\n').Select(l => l.TrimEnd()).ToList();

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimStart();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            if (trimmedLine.StartsWith("Table "))
            {
                ParseTable(trimmedLine);
            }
            else if (trimmedLine.StartsWith("Project "))
            {
                ParseProject(trimmedLine);
            }
            else if (trimmedLine.StartsWith("Ref:"))
            {
                ParseReference(trimmedLine);
            }
            else if (trimmedLine.StartsWith("indexes {"))
            {
                _parsingIndexes = true;
            }
            else if (trimmedLine == "}")
            {
                _parsingIndexes = false;
            }
            else if (_parsingIndexes)
            {
                ParseIndex(trimmedLine);
            }
            else if (_currentTable != null)
            {
                ParseColumn(trimmedLine);
            }
        }

        return _model;
    }

    private void ParseProject(string line)
    {
        var match = Regex.Match(line, @"Project\s+(?:""([^""]+)""|(\w+))(?:\s*\[([^\]]+)\])?(?:\s*'([^']+)')?");
        if (match.Success)
        {
            _model.ProjectName = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            if (match.Groups[3].Success)
            {
                var settings = match.Groups[3].Value;
                if (settings.StartsWith("settings:"))
                {
                    var settingsMatch = Regex.Match(settings, @"settings:\s*{([^}]+)}");
                    if (settingsMatch.Success)
                    {
                        var settingsList = settingsMatch.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var setting in settingsList)
                        {
                            var parts = setting.Split(':', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                var key = parts[0].Trim();
                                var value = parts[1].Trim();
                                _model.Settings[key] = value;
                                switch (key)
                                {
                                    case "database_type":
                                        _model.DatabaseType = value;
                                        break;
                                    case "language":
                                        _model.Language = value;
                                        break;
                                    case "schema":
                                        _model.Schema = value;
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var settingMatch = Regex.Match(settings, @"(\w+):\s*(\w+)");
                    if (settingMatch.Success)
                    {
                        var key = settingMatch.Groups[1].Value;
                        var value = settingMatch.Groups[2].Value;
                        _model.Settings[key] = value;
                        switch (key)
                        {
                            case "database_type":
                                _model.DatabaseType = value;
                                break;
                            case "language":
                                _model.Language = value;
                                break;
                            case "schema":
                                _model.Schema = value;
                                break;
                        }
                    }
                }
            }
            if (match.Groups[4].Success)
            {
                _model.Note = match.Groups[4].Value;
            }
        }
    }

    private void ParseTable(string line)
    {
        var tableMatch = Regex.Match(line, @"Table\s+(?:([^\.]+)\.)?(?:""([^""]+)""|(\w+))(?:\s+as\s+(\w+))?(?:\s*\[([^\]]+)\])?(?:\s+'([^']+)')?");
        if (!tableMatch.Success) return;

        var schema = tableMatch.Groups[1].Success ? tableMatch.Groups[1].Value : null;
        var name = tableMatch.Groups[2].Success ? tableMatch.Groups[2].Value : tableMatch.Groups[3].Value;
        var alias = tableMatch.Groups[4].Success ? tableMatch.Groups[4].Value : null;
        var settings = tableMatch.Groups[5].Success ? tableMatch.Groups[5].Value : null;
        var note = tableMatch.Groups[6].Success ? tableMatch.Groups[6].Value : null;

        _currentTable = new Table
        {
            Name = name,
            Schema = schema,
            Alias = alias,
            Note = note,
            Type = TableType.Normal,
            Settings = new Dictionary<string, string>(),
            Columns = new List<Column>(),
            Indexes = new List<Models.Index>()
        };

        if (!string.IsNullOrEmpty(settings))
        {
            var settingsMatch = Regex.Match(settings, @"settings:\s*{\s*([^}]+)\s*}");
            if (settingsMatch.Success)
            {
                var settingsStr = settingsMatch.Groups[1].Value;
                var settingsMatches = Regex.Matches(settingsStr, @"(\w+)\s*:\s*(?:""([^""]+)""|'([^']+)'|([^,\s]+))\s*,?\s*");
                foreach (Match m in settingsMatches)
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Success ? m.Groups[2].Value :
                               m.Groups[3].Success ? m.Groups[3].Value :
                               m.Groups[4].Value;
                    _currentTable.Settings[key] = value;
                }
            }
            else if (settings.StartsWith("type:"))
            {
                var typeValue = settings.Substring(5).Trim();
                if (typeValue == "view")
                {
                    _currentTable.Type = TableType.View;
                }
            }
        }

        _model.Tables.Add(_currentTable);
    }

    private void ParseColumn(string line)
    {
        if (_currentTable == null) return;

        var columnMatch = Regex.Match(line.TrimStart(), @"(\w+)\s+(\w+)(?:\((\d+)(?:,(\d+))?\))?(?:\s*\[([^\]]+)\])?(?:\s*'([^']+)')?");
        if (!columnMatch.Success) return;

        var name = columnMatch.Groups[1].Value;
        var type = columnMatch.Groups[2].Value;
        var length = columnMatch.Groups[3].Success ? int.Parse(columnMatch.Groups[3].Value) : (int?)null;
        var scale = columnMatch.Groups[4].Success ? int.Parse(columnMatch.Groups[4].Value) : (int?)null;
        var settings = columnMatch.Groups[5].Success ? columnMatch.Groups[5].Value : null;
        var note = columnMatch.Groups[6].Success ? columnMatch.Groups[6].Value : null;

        var column = new Column
        {
            Name = name,
            Type = type,
            Note = note,
            Length = length,
            Precision = length,
            Scale = scale,
            Settings = new Dictionary<string, string>()
        };

        if (!string.IsNullOrEmpty(settings))
        {
            var settingsList = settings.Split(',').Select(s => s.Trim());
            foreach (var setting in settingsList)
            {
                if (setting == "pk")
                {
                    column.IsPrimaryKey = true;
                }
                else if (setting == "not null")
                {
                    column.IsNotNull = true;
                }
                else if (setting == "unique")
                {
                    column.IsUnique = true;
                }
                else if (setting == "increment")
                {
                    column.IsIncrement = true;
                }
                else if (setting == "unsigned")
                {
                    column.IsUnsigned = true;
                }
                else if (setting.StartsWith("default:"))
                {
                    var defaultValue = setting.Substring(8).Trim();
                    if (defaultValue.StartsWith('`') && defaultValue.EndsWith('`'))
                    {
                        defaultValue = defaultValue.Substring(1, defaultValue.Length - 2);
                    }
                    column.DefaultValue = defaultValue;
                }
                else if (setting.StartsWith("ref:"))
                {
                    var refMatch = Regex.Match(setting, @"ref:\s*([<>-])\s*(\w+)\.(\w+)");
                    if (refMatch.Success)
                    {
                        var refType = refMatch.Groups[1].Value;
                        var refTable = refMatch.Groups[2].Value;
                        var refColumn = refMatch.Groups[3].Value;

                        var reference = new Reference
                        {
                            FromTable = _currentTable.Name,
                            FromColumn = column.Name,
                            ToTable = refTable,
                            ToColumn = refColumn
                        };

                        switch (refType)
                        {
                            case ">":
                                reference.Type = ReferenceType.ManyToOne;
                                break;
                            case "<":
                                reference.Type = ReferenceType.OneToMany;
                                break;
                            case "-":
                                reference.Type = ReferenceType.OneToOne;
                                break;
                            default:
                                reference.Type = ReferenceType.ManyToMany;
                                break;
                        }

                        column.Reference = reference;
                    }
                }
                else
                {
                    var settingMatch = Regex.Match(setting, @"(\w+):\s*(?:""([^""]+)""|'([^']+)'|([^,\s]+))");
                    if (settingMatch.Success)
                    {
                        var key = settingMatch.Groups[1].Value;
                        var value = settingMatch.Groups[2].Success ? settingMatch.Groups[2].Value :
                                   settingMatch.Groups[3].Success ? settingMatch.Groups[3].Value :
                                   settingMatch.Groups[4].Value;
                        column.Settings[key] = value;
                    }
                }
            }
        }

        _currentTable.Columns.Add(column);

        // Check if this is a many-to-many relationship after adding the column
        if (_currentTable.Columns.Count == 2 && 
            _currentTable.Columns.All(c => c.Name.EndsWith("_id", StringComparison.OrdinalIgnoreCase)) &&
            _currentTable.Name.Contains("_") &&
            _currentTable.Name.Split('_').Length == 2)
        {
            foreach (var c in _currentTable.Columns)
            {
                if (c.Reference != null)
                    c.Reference.Type = ReferenceType.ManyToMany;
            }
        }
    }

    private ReferenceType DetermineReferenceType(string refType, Table fromTable, Column column)
    {
        if (refType == "-")
            return ReferenceType.OneToOne;
        else if (refType == ">")
        {
            // Check if this is part of a many-to-many relationship
            var isManyToMany = fromTable != null && 
                fromTable.Columns.Count == 2 && 
                fromTable.Columns.All(c => c.Name.EndsWith("_id", StringComparison.OrdinalIgnoreCase)) &&
                fromTable.Name.Contains("_") &&
                fromTable.Name.Split('_').Length == 2;

            if (isManyToMany)
            {
                // Update the reference type for both columns in the join table
                foreach (var c in fromTable.Columns)
                {
                    if (c.Reference != null)
                        c.Reference.Type = ReferenceType.ManyToMany;
                }
                return ReferenceType.ManyToMany;
            }

            // Check if this is part of a one-to-one relationship
            var isOneToOne = refType == "-" || 
                (fromTable != null && fromTable.Columns.Any(c => 
                    c.Reference != null && 
                    c.Reference.Type == ReferenceType.OneToOne && 
                    c.Reference.ToTable == column.Reference?.FromTable));

            return isOneToOne ? ReferenceType.OneToOne : ReferenceType.ManyToOne;
        }
        else if (refType == "<")
            return ReferenceType.OneToMany;
        else
            return ReferenceType.ManyToOne;
    }

    private void ParseReference(string line)
    {
        // Try to match the format: Ref: posts.user_id > users.id
        var simpleMatch = Regex.Match(line, @"Ref:\s*(?:""([^""]+)""|([^\s.]+))\.(?:""([^""]+)""|([^\s>]+))\s*>\s*(?:""([^""]+)""|([^\s.]+))\.(?:""([^""]+)""|([^\s']+))\s*(?:'([^']+)')?");
        if (simpleMatch.Success)
        {
            var fromTable = (simpleMatch.Groups[1].Success ? simpleMatch.Groups[1].Value : simpleMatch.Groups[2].Value).Trim();
            var fromColumn = (simpleMatch.Groups[3].Success ? simpleMatch.Groups[3].Value : simpleMatch.Groups[4].Value).Trim();
            var toTable = (simpleMatch.Groups[5].Success ? simpleMatch.Groups[5].Value : simpleMatch.Groups[6].Value).Trim();
            var toColumn = (simpleMatch.Groups[7].Success ? simpleMatch.Groups[7].Value : simpleMatch.Groups[8].Value).Trim();
            var note = simpleMatch.Groups[9].Success ? simpleMatch.Groups[9].Value.Trim() : null;

            var reference = new Reference
            {
                Name = $"{fromTable}.{fromColumn} > {toTable}.{toColumn}",
                FromTable = fromTable,
                FromColumn = fromColumn,
                ToTable = toTable,
                ToColumn = toColumn,
                Note = note,
                Type = ReferenceType.ManyToOne
            };

            _model.References.Add(reference);
            return;
        }

        // Try to match the format: Ref user_posts: posts.user_id > users.id
        var namedMatch = Regex.Match(line, @"Ref\s+(?:""([^""]+)""|([^\s:]+))\s*:\s*(?:""([^""]+)""|([^\s.]+))\.(?:""([^""]+)""|([^\s>]+))\s*>\s*(?:""([^""]+)""|([^\s.]+))\.(?:""([^""]+)""|([^\s']+))\s*(?:'([^']+)')?");
        if (namedMatch.Success)
        {
            var name = (namedMatch.Groups[1].Success ? namedMatch.Groups[1].Value : namedMatch.Groups[2].Value).Trim();
            var fromTable = (namedMatch.Groups[3].Success ? namedMatch.Groups[3].Value : namedMatch.Groups[4].Value).Trim();
            var fromColumn = (namedMatch.Groups[5].Success ? namedMatch.Groups[5].Value : namedMatch.Groups[6].Value).Trim();
            var toTable = (namedMatch.Groups[7].Success ? namedMatch.Groups[7].Value : namedMatch.Groups[8].Value).Trim();
            var toColumn = (namedMatch.Groups[9].Success ? namedMatch.Groups[9].Value : namedMatch.Groups[10].Value).Trim();
            var note = namedMatch.Groups[11].Success ? namedMatch.Groups[11].Value.Trim() : null;

            var reference = new Reference
            {
                Name = name,
                FromTable = fromTable,
                FromColumn = fromColumn,
                ToTable = toTable,
                ToColumn = toColumn,
                Note = note,
                Type = ReferenceType.ManyToOne
            };

            _model.References.Add(reference);
        }
    }

    private void ParseIndex(string line)
    {
        var match = Regex.Match(line.Trim(), @"(?:\(([^)]+)\)|(\w+))(?:\s*\[([^\]]+)\])?(?:\s*'([^']+)')?");
        if (match.Success)
        {
            var columns = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            var columnList = columns.Split(',').Select(c => c.Trim()).ToList();
            var name = string.Join(", ", columnList);

            var index = new Ivy.Dbml.Parser.Models.Index
            {
                Name = name,
                Columns = columnList,
                IsUnique = false
            };

            if (match.Groups[3].Success)
            {
                var settings = match.Groups[3].Value;
                var settingMatches = Regex.Matches(settings, @"(\w+)(?::\s*(\w+))?");
                foreach (Match settingMatch in settingMatches)
                {
                    var key = settingMatch.Groups[1].Value;
                    if (key == "unique")
                    {
                        index.IsUnique = true;
                    }
                    else if (key == "type" && settingMatch.Groups[2].Success)
                    {
                        index.Type = settingMatch.Groups[2].Value;
                    }
                }
            }

            if (match.Groups[4].Success)
            {
                index.Note = match.Groups[4].Value;
            }

            _currentTable.Indexes.Add(index);
        }
    }
} 