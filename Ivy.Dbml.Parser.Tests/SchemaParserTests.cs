using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class SchemaParserTests
{
    private readonly DbmlParser _parser;

    public SchemaParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseTableWithSchema()
    {
        var dbml = @"
Table core.users {
  id integer [pk]
  username varchar
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Equal("core", table.Schema);
    }

    [Fact]
    public void ParseTablesWithMultipleSchemas()
    {
        var dbml = @"
Table core.users {
  id integer [pk]
  username varchar
}

Table blogging.posts {
  id integer [pk]
  title varchar
  content text
  user_id integer
}";

        var model = _parser.Parse(dbml);

        Assert.Equal(2, model.Tables.Count);
        
        var usersTable = model.Tables[0];
        Assert.Equal("users", usersTable.Name);
        Assert.Equal("core", usersTable.Schema);
        
        var postsTable = model.Tables[1];
        Assert.Equal("posts", postsTable.Name);
        Assert.Equal("blogging", postsTable.Schema);
    }

    [Fact]
    public void ParseTablesWithDefaultPublicSchema()
    {
        var dbml = @"
Table users {
  id integer [pk]
  username varchar
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Equal("public", table.Schema);
    }

    [Fact]
    public void ParseCrossSchemaRelationship()
    {
        var dbml = @"
Table core.users {
  id integer [pk]
  username varchar
}

Table blogging.posts {
  id integer [pk]
  title varchar
  content text
  user_id integer
}

Ref: blogging.posts.user_id > core.users.id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("blogging", reference.FromSchema);
        Assert.Equal("posts", reference.FromTable);
        Assert.Equal("user_id", reference.FromColumn);
        Assert.Equal("core", reference.ToSchema);
        Assert.Equal("users", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
    }
} 