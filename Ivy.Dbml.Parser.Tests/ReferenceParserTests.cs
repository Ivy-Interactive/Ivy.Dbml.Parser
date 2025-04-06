using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ReferenceParserTests
{
    private readonly DbmlParser _parser;

    public ReferenceParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleReference()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer
}

Ref: posts.user_id > users.id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("posts.user_id > users.id", reference.Name);
        Assert.Equal("posts", reference.FromTable);
        Assert.Equal("user_id", reference.FromColumn);
        Assert.Equal("users", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
    }

    [Fact]
    public void ParseReferenceWithName()
    {
        var dbml = "Table users {\n  id integer [pk]\n}\n\nTable posts {\n  id integer [pk]\n  user_id integer\n}\n\nRef user_posts: posts.user_id > users.id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("user_posts", reference.Name);
        Assert.Equal("posts", reference.FromTable);
        Assert.Equal("user_id", reference.FromColumn);
        Assert.Equal("users", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
    }

    [Fact]
    public void ParseReferenceWithNote()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer
}

Ref: posts.user_id > users.id 'User posts relationship'";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("posts.user_id > users.id", reference.Name);
        Assert.Equal("User posts relationship", reference.Note);
    }

    [Fact]
    public void ParseReferenceWithQuotedNames()
    {
        var dbml = @"
Table ""user accounts"" {
  id integer [pk]
}

Table ""blog posts"" {
  id integer [pk]
  user_id integer
}

Ref: ""blog posts"".user_id > ""user accounts"".id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("blog posts", reference.FromTable);
        Assert.Equal("user_id", reference.FromColumn);
        Assert.Equal("user accounts", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
    }

    [Fact]
    public void ParseSimpleNamedReference()
    {
        var dbml = "Ref user_posts: posts.user_id > users.id";
        var model = new DbmlParser().Parse(dbml);
        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("user_posts", reference.Name);
    }
} 