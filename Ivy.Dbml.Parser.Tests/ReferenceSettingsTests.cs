using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ReferenceSettingsTests
{
    private readonly DbmlParser _parser;

    public ReferenceSettingsTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseReference_WithDeleteCascade()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [delete: cascade]";
        var model = _parser.Parse(dbml);
        Assert.Equal("cascade", model.References[0].OnDelete);
        Assert.Null(model.References[0].OnUpdate);
    }

    [Fact]
    public void ParseReference_WithUpdateRestrict()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [update: restrict]";
        var model = _parser.Parse(dbml);
        Assert.Equal("restrict", model.References[0].OnUpdate);
        Assert.Null(model.References[0].OnDelete);
    }

    [Fact]
    public void ParseReference_WithBothDeleteAndUpdate()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [delete: cascade, update: no action]";
        var model = _parser.Parse(dbml);
        Assert.Equal("cascade", model.References[0].OnDelete);
        Assert.Equal("no action", model.References[0].OnUpdate);
    }

    [Fact]
    public void ParseReference_WithSetNull()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [delete: set null]";
        var model = _parser.Parse(dbml);
        Assert.Equal("set null", model.References[0].OnDelete);
    }

    [Fact]
    public void ParseReference_WithSetDefault()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [update: set default]";
        var model = _parser.Parse(dbml);
        Assert.Equal("set default", model.References[0].OnUpdate);
    }

    [Fact]
    public void ParseReference_SettingsDictionaryAlsoPopulated()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id [delete: cascade, update: no action]";
        var model = _parser.Parse(dbml);
        Assert.Equal("cascade", model.References[0].Settings["delete"]);
        Assert.Equal("no action", model.References[0].Settings["update"]);
    }

    [Fact]
    public void ParseReference_NamedRefWithSettings()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef fk_b_a: b.a_id > a.id [delete: cascade, update: restrict]";
        var model = _parser.Parse(dbml);
        Assert.Equal("cascade", model.References[0].OnDelete);
        Assert.Equal("restrict", model.References[0].OnUpdate);
    }

    [Fact]
    public void ParseReference_InlineWithDeleteCascade()
    {
        var dbml = @"
Table users {
  id int [pk]
}

Table posts {
  id int [pk]
  user_id int [ref: > users.id, delete: cascade]
}";

        var model = _parser.Parse(dbml);
        Assert.Single(model.References);
        Assert.Equal("cascade", model.References[0].OnDelete);
    }

    [Fact]
    public void ParseReference_InlineWithBothDeleteAndUpdate()
    {
        var dbml = @"
Table users {
  id int [pk]
}

Table posts {
  id int [pk]
  user_id int [ref: > users.id, delete: cascade, update: set null]
}";

        var model = _parser.Parse(dbml);
        Assert.Single(model.References);
        Assert.Equal("cascade", model.References[0].OnDelete);
        Assert.Equal("set null", model.References[0].OnUpdate);
    }

    [Fact]
    public void ParseReference_NoSettings_PropertiesAreNull()
    {
        var dbml = "Table a { id int }\nTable b { a_id int }\nRef: b.a_id > a.id";
        var model = _parser.Parse(dbml);
        Assert.Null(model.References[0].OnDelete);
        Assert.Null(model.References[0].OnUpdate);
    }
}
