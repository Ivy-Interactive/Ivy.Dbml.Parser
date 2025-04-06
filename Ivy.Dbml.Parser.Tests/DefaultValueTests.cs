using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class DefaultValueTests
{
    private readonly DbmlParser _parser;

    public DefaultValueTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseDefaultNumberValue()
    {
        var dbml = @"
Table users {
  id integer [pk]
  rating integer [default: 10]
  score decimal [default: 123.456]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal(3, table.Columns.Count);
        
        var ratingColumn = table.Columns[1];
        Assert.Equal("rating", ratingColumn.Name);
        Assert.Equal("10", ratingColumn.DefaultValue);
        
        var scoreColumn = table.Columns[2];
        Assert.Equal("score", scoreColumn.Name);
        Assert.Equal("123.456", scoreColumn.DefaultValue);
    }

    [Fact]
    public void ParseDefaultStringValue()
    {
        var dbml = @"
Table users {
  id integer [pk]
  source varchar [default: 'direct']
  status varchar [default: 'active user']
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal(3, table.Columns.Count);
        
        var sourceColumn = table.Columns[1];
        Assert.Equal("source", sourceColumn.Name);
        Assert.Equal("direct", sourceColumn.DefaultValue);
        
        var statusColumn = table.Columns[2];
        Assert.Equal("status", statusColumn.Name);
        Assert.Equal("active user", statusColumn.DefaultValue);
    }

    [Fact]
    public void ParseDefaultExpressionValue()
    {
        var dbml = @"
Table users {
  id integer [pk]
  created_at timestamp [default: `now()`]
  modified_at timestamp [default: `now() - interval '5 days'`]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal(3, table.Columns.Count);
        
        var createdAtColumn = table.Columns[1];
        Assert.Equal("created_at", createdAtColumn.Name);
        Assert.Equal("now()", createdAtColumn.DefaultValue);
        
        var modifiedAtColumn = table.Columns[2];
        Assert.Equal("modified_at", modifiedAtColumn.Name);
        Assert.Equal("now() - interval '5 days'", modifiedAtColumn.DefaultValue);
    }

    [Fact]
    public void ParseDefaultBooleanValue()
    {
        var dbml = @"
Table users {
  id integer [pk]
  is_active boolean [default: true]
  is_deleted boolean [default: false]
  profile_photo varchar [default: null]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal(4, table.Columns.Count);
        
        var isActiveColumn = table.Columns[1];
        Assert.Equal("is_active", isActiveColumn.Name);
        Assert.Equal("true", isActiveColumn.DefaultValue);
        
        var isDeletedColumn = table.Columns[2];
        Assert.Equal("is_deleted", isDeletedColumn.Name);
        Assert.Equal("false", isDeletedColumn.DefaultValue);
        
        var profilePhotoColumn = table.Columns[3];
        Assert.Equal("profile_photo", profilePhotoColumn.Name);
        Assert.Equal("null", profilePhotoColumn.DefaultValue);
    }
} 