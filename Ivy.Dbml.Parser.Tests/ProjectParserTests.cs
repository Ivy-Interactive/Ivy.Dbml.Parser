using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ProjectParserTests
{
    private readonly DbmlParser _parser;

    public ProjectParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleProject()
    {
        var dbml = @"
Project MyDatabase

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Null(model.Note);
    }

    [Fact]
    public void ParseProjectWithNote()
    {
        var dbml = @"
Project MyDatabase 'A sample database'

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("MyDatabase", model.ProjectName);
        Assert.Equal("A sample database", model.Note);
    }

    [Fact]
    public void ParseProjectWithQuotedName()
    {
        var dbml = @"
Project ""My Database""

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("My Database", model.ProjectName);
        Assert.Null(model.Note);
    }

    [Fact]
    public void ParseProjectWithQuotedNameAndNote()
    {
        var dbml = @"
Project ""My Database"" 'A sample database with spaces'

Table users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("My Database", model.ProjectName);
        Assert.Equal("A sample database with spaces", model.Note);
    }
} 