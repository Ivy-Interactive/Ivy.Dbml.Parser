using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ComplexReferenceTests
{
    private readonly DbmlParser _parser;

    public ComplexReferenceTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseManyToManyRelationship()
    {
        var dbml = @"
Table authors {
  id integer [pk]
  name varchar
}

Table books {
  id integer [pk]
  title varchar
}

Ref: authors.id <> books.id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("authors.id <> books.id", reference.Name);
        Assert.Equal("authors", reference.FromTable);
        Assert.Equal("id", reference.FromColumn);
        Assert.Equal("books", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
        Assert.Equal(ReferenceType.ManyToMany, reference.Type);
    }

    [Fact]
    public void ParseOneToOneRelationship()
    {
        var dbml = @"
Table users {
  id integer [pk]
  name varchar
}

Table user_profiles {
  id integer [pk]
  user_id integer
  bio text
}

Ref: users.id - user_profiles.user_id";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        Assert.Equal("users.id - user_profiles.user_id", reference.Name);
        Assert.Equal("users", reference.FromTable);
        Assert.Equal("id", reference.FromColumn);
        Assert.Equal("user_profiles", reference.ToTable);
        Assert.Equal("user_id", reference.ToColumn);
        Assert.Equal(ReferenceType.OneToOne, reference.Type);
    }

    [Fact]
    public void ParseCompositeKeyRelationship()
    {
        var dbml = @"
Table merchants {
  id integer [pk]
  country_code char(2)
}

Table merchant_periods {
  id integer [pk]
  merchant_id integer
  country_code char(2)
  start_date date
  end_date date
}

Ref: merchant_periods.(merchant_id, country_code) > merchants.(id, country_code)";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        
        Assert.Equal("merchant_periods", reference.FromTable);
        Assert.Equal("merchants", reference.ToTable);
        
        Assert.True(reference.IsCompositeKey);
        Assert.Equal(2, reference.FromColumns.Count);
        Assert.Equal("merchant_id", reference.FromColumns[0]);
        Assert.Equal("country_code", reference.FromColumns[1]);
        
        Assert.Equal(2, reference.ToColumns.Count);
        Assert.Equal("id", reference.ToColumns[0]);
        Assert.Equal("country_code", reference.ToColumns[1]);
    }

    [Fact]
    public void ParseReferenceWithSettings()
    {
        var dbml = @"
Table merchants {
  id integer [pk]
}

Table products {
  id integer [pk]
  merchant_id integer
}

Ref: products.merchant_id > merchants.id [delete: cascade, update: no action]";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        
        Assert.Equal("products", reference.FromTable);
        Assert.Equal("merchant_id", reference.FromColumn);
        Assert.Equal("merchants", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
        
        Assert.Equal("cascade", reference.Settings["delete"]);
        Assert.Equal("no action", reference.Settings["update"]);
    }

    [Fact]
    public void ParseInlineReference()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table posts {
  id integer [pk]
  user_id integer [ref: > users.id]
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.References);
        var reference = model.References[0];
        
        Assert.Equal("posts", reference.FromTable);
        Assert.Equal("user_id", reference.FromColumn);
        Assert.Equal("users", reference.ToTable);
        Assert.Equal("id", reference.ToColumn);
        Assert.Equal(ReferenceType.ManyToOne, reference.Type);
    }
} 