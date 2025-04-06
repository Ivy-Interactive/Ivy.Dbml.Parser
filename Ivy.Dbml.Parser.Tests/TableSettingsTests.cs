using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class TableSettingsTests
{
    private readonly DbmlParser _parser;

    public TableSettingsTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseViewTable()
    {
        var dbml = @"
Table active_users [type: view] {
  id integer
  name varchar
}";

        var model = _parser.Parse(dbml);
        var table = model.Tables[0];

        Assert.Equal("active_users", table.Name);
        Assert.Equal(TableType.View, table.Type);
    }

    [Fact]
    public void ParseTableWithSchema()
    {
        var dbml = @"
Table public.users {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);
        var table = model.Tables[0];

        Assert.Equal("users", table.Name);
        Assert.Equal("public", table.Schema);
    }

    [Fact]
    public void ParseTableWithSettings()
    {
        var dbml = @"
Table users [settings: { collation: 'utf8mb4_unicode_ci' }] {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);
        var table = model.Tables[0];

        Assert.Equal("users", table.Name);
        Assert.Equal("utf8mb4_unicode_ci", table.Settings["collation"]);
    }

    [Fact]
    public void ParseTableWithMultipleSettings()
    {
        var dbml = @"
Table users [settings: { 
  collation: 'utf8mb4_unicode_ci',
  engine: 'InnoDB',
  charset: 'utf8mb4'
}] {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);
        var table = model.Tables[0];

        Assert.Equal("users", table.Name);
        Assert.Equal("utf8mb4_unicode_ci", table.Settings["collation"]);
        Assert.Equal("InnoDB", table.Settings["engine"]);
        Assert.Equal("utf8mb4", table.Settings["charset"]);
    }

    [Fact]
    public void ParseTableWithNote()
    {
        var dbml = @"
Table users 'User management table' {
  id integer [pk]
}";

        var model = _parser.Parse(dbml);
        var table = model.Tables[0];

        Assert.Equal("users", table.Name);
        Assert.Equal("User management table", table.Note);
    }
} 