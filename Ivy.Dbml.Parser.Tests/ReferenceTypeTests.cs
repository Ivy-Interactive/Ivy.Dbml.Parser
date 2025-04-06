using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ReferenceTypeTests
{
    private readonly DbmlParser _parser;

    public ReferenceTypeTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseOneToOneReference()
    {
        var dbml = @"
Table users {
  id integer [pk]
  profile_id integer [ref: > profiles.id]
}

Table profiles {
  id integer [pk]
  user_id integer [ref: - users.id]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal(2, model.Tables.Count);
        var usersTable = model.Tables[0];
        var profilesTable = model.Tables[1];

        Assert.NotNull(usersTable.Columns[1].Reference);
        var userRefType = usersTable.Columns[1].Reference!.Type;
        Assert.Equal(ReferenceType.OneToOne, userRefType);
    }

    [Fact]
    public void ParseOneToManyReference()
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

        Assert.Equal(2, model.Tables.Count);
        var postsTable = model.Tables[1];

        Assert.NotNull(postsTable.Columns[1].Reference);
        Assert.Equal(ReferenceType.ManyToOne, postsTable.Columns[1].Reference.Type);
    }

    [Fact]
    public void ParseManyToManyReference()
    {
        var dbml = @"
Table users {
  id integer [pk]
}

Table groups {
  id integer [pk]
}

Table user_groups {
  user_id integer [ref: > users.id]
  group_id integer [ref: > groups.id]
}";

        var model = _parser.Parse(dbml);

        Assert.Equal(3, model.Tables.Count);
        var userGroupsTable = model.Tables[2];

        Assert.NotNull(userGroupsTable.Columns[0].Reference);
        Assert.NotNull(userGroupsTable.Columns[1].Reference);
        Assert.Equal(ReferenceType.ManyToMany, userGroupsTable.Columns[0].Reference.Type);
        Assert.Equal(ReferenceType.ManyToMany, userGroupsTable.Columns[1].Reference.Type);
    }
} 