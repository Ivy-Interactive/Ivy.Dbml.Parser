using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class TableParserTests
{
    private readonly DbmlParser _parser;

    public TableParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleTable()
    {
        var dbml = @"
Table users {
  id integer [pk]
  name varchar [not null]
  email varchar [unique]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Null(table.Alias);
        Assert.Null(table.Note);

        Assert.Equal(3, table.Columns.Count);

        var idColumn = table.Columns[0];
        Assert.Equal("id", idColumn.Name);
        Assert.Equal("integer", idColumn.Type);
        Assert.True(idColumn.IsPrimaryKey);
        Assert.False(idColumn.IsNotNull);
        Assert.False(idColumn.IsUnique);

        var nameColumn = table.Columns[1];
        Assert.Equal("name", nameColumn.Name);
        Assert.Equal("varchar", nameColumn.Type);
        Assert.False(nameColumn.IsPrimaryKey);
        Assert.True(nameColumn.IsNotNull);
        Assert.False(nameColumn.IsUnique);

        var emailColumn = table.Columns[2];
        Assert.Equal("email", emailColumn.Name);
        Assert.Equal("varchar", emailColumn.Type);
        Assert.False(emailColumn.IsPrimaryKey);
        Assert.False(emailColumn.IsNotNull);
        Assert.True(emailColumn.IsUnique);
    }

    [Fact]
    public void ParseTableWithAlias()
    {
        var dbml = @"
Table users as u {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Equal("u", table.Alias);
    }

    [Fact]
    public void ParseTableWithNote()
    {
        var dbml = @"
Table users 'User table' {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Equal("User table", table.Note);
    }

    [Fact]
    public void ParseTableWithQuotedName()
    {
        var dbml = @"
Table ""user accounts"" {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("user accounts", table.Name);
    }

    [Fact]
    public void ParseTableWithDefaultValue()
    {
        var dbml = @"
Table users {
  created_at timestamp [default: `now()`]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Columns);
        var column = table.Columns[0];
        Assert.Equal("created_at", column.Name);
        Assert.Equal("timestamp", column.Type);
        Assert.Equal("now()", column.DefaultValue);
    }
} 