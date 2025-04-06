using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class TableGroupParserTests
{
    private readonly DbmlParser _parser;

    public TableGroupParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleTableGroup()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer
}

TableGroup e_commerce {
  users
  posts
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.TableGroups);
        var tableGroup = model.TableGroups[0];
        Assert.Equal("e_commerce", tableGroup.Name);
        Assert.Null(tableGroup.Note);
        Assert.Equal(2, tableGroup.Tables.Count);
        Assert.Equal("users", tableGroup.Tables[0]);
        Assert.Equal("posts", tableGroup.Tables[1]);
    }

    [Fact]
    public void ParseTableGroupWithNote()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer
}

TableGroup e_commerce [note: 'Contains tables that are related to e-commerce system'] {
  users
  posts
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.TableGroups);
        var tableGroup = model.TableGroups[0];
        Assert.Equal("e_commerce", tableGroup.Name);
        Assert.Equal("Contains tables that are related to e-commerce system", tableGroup.Note);
    }

    [Fact]
    public void ParseTableGroupWithSettings()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer
}

TableGroup e_commerce [color: #345] {
  users
  posts
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.TableGroups);
        var tableGroup = model.TableGroups[0];
        Assert.Equal("e_commerce", tableGroup.Name);
        Assert.Equal("#345", tableGroup.Settings["color"]);
    }
} 