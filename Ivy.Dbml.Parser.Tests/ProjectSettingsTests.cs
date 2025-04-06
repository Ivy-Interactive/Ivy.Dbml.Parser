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
        var dbml = @"
Project MyDatabase [settings: {
  database_type: postgres,
  language: postgresql,
  schema: public
}]

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
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