using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ProjectSettingsTests
{
    private readonly DbmlParser _parser;

    public ProjectSettingsTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseProjectWithDatabaseType()
    {
        var dbml = @"
Project MyDatabase [database_type: postgres]

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Equal("postgres", model.DatabaseType);
    }

    [Fact]
    public void ParseProjectWithLanguage()
    {
        var dbml = @"
Project MyDatabase [language: postgresql]

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Equal("postgresql", model.Language);
    }

    [Fact]
    public void ParseProjectWithSchema()
    {
        var dbml = @"
Project MyDatabase [schema: public]

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Equal("public", model.Schema);
    }

    [Fact]
    public void ParseProjectWithMultipleSettings()
    {
        // Use a raw string with no indentation or newlines to simplify parsing
        var dbml = "Project MyDatabase [settings: {database_type: postgres, language: postgresql, schema: public}]\n\nTable users {\n  id integer [pk]\n}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        
        // Debug: Print out what keys are in the Settings dictionary
        var keys = string.Join(", ", model.Settings.Keys);
        Assert.True(model.Settings.ContainsKey("database_type"), $"Settings keys are: {keys}");
        Assert.True(model.Settings.ContainsKey("language"), $"Settings keys are: {keys}");
        Assert.True(model.Settings.ContainsKey("schema"), $"Settings keys are: {keys}");
        
        Assert.Equal("postgres", model.Settings["database_type"]);
        Assert.Equal("postgresql", model.Settings["language"]);
        Assert.Equal("public", model.Settings["schema"]);
    }

    [Fact]
    public void ParseProjectWithNote()
    {
        var dbml = @"
Project MyDatabase 'A sample database project'

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Equal("A sample database project", model.Note);
    }
} 