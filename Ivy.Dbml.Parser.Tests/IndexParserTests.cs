using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class IndexParserTests
{
    private readonly DbmlParser _parser;

    public IndexParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleIndex()
    {
        var dbml = @"
Table users {
  id integer [pk]
  email varchar
  name varchar

  Indexes {
    email
  }
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Indexes);
        var index = table.Indexes[0];
        Assert.Equal("email", index.Name);
        Assert.Single(index.Columns);
        Assert.Equal("email", index.Columns[0]);
        Assert.False(index.IsUnique);
    }

    [Fact]
    public void ParseUniqueIndex()
    {
        var dbml = @"
Table users {
  id integer [pk]
  email varchar

  Indexes {
    (email) [unique]
  }
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Indexes);
        var index = table.Indexes[0];
        Assert.Equal("email", index.Name);
        Assert.Single(index.Columns);
        Assert.Equal("email", index.Columns[0]);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void ParseMultiColumnIndex()
    {
        var dbml = @"
Table users {
  id integer [pk]
  first_name varchar
  last_name varchar

  Indexes {
    (first_name, last_name)
  }
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Indexes);
        var index = table.Indexes[0];
        Assert.Equal("first_name, last_name", index.Name);
        Assert.Equal(2, index.Columns.Count);
        Assert.Equal("first_name", index.Columns[0]);
        Assert.Equal("last_name", index.Columns[1]);
    }

    [Fact]
    public void ParseIndexWithType()
    {
        var dbml = @"
Table users {
  id integer [pk]
  email varchar

  Indexes {
    (email) [type: btree]
  }
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Indexes);
        var index = table.Indexes[0];
        Assert.Equal("email", index.Name);
        Assert.Equal("btree", index.Type);
    }

    [Fact]
    public void ParseIndexWithNote()
    {
        var dbml = @"
Table users {
  id integer [pk]
  email varchar

  Indexes {
    (email) 'Email index for faster lookups'
  }
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Single(table.Indexes);
        var index = table.Indexes[0];
        Assert.Equal("email", index.Name);
        Assert.Equal("Email index for faster lookups", index.Note);
    }
} 