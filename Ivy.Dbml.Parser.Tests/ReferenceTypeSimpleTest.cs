using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests
{
    public class ReferenceTypeSimpleTest
    {
        [Fact]
        public void TestOneToOneReference()
        {
            var dbml = @"
Table users {
  id integer [pk]
  profile_id integer [ref: - profiles.id]
}

Table profiles {
  id integer [pk]
  user_id integer [ref: - users.id]
}";

            var parser = new DbmlParser();
            var model = parser.Parse(dbml);

            Assert.Equal(2, model.Tables.Count);
            var usersTable = model.Tables[0];
            var profilesTable = model.Tables[1];

            Assert.NotNull(usersTable.Columns[1].Reference);
            var userRefType = usersTable.Columns[1].Reference!.Type;
            Assert.Equal(ReferenceType.OneToOne, userRefType);
        }

        [Fact]
        public void TestManyToManyReference()
        {
            var dbml = @"
Table users {
  id integer [pk]
}

Table groups {
  id integer [pk]
}

Table user_groups {
  user_id integer [ref: <> users.id]
  group_id integer [ref: <> groups.id]
}";

            var parser = new DbmlParser();
            var model = parser.Parse(dbml);

            Assert.Equal(3, model.Tables.Count);
            var userGroupsTable = model.Tables[2];

            Assert.NotNull(userGroupsTable.Columns[0].Reference);
            Assert.NotNull(userGroupsTable.Columns[1].Reference);
            Assert.Equal(ReferenceType.ManyToMany, userGroupsTable.Columns[0].Reference!.Type);
            Assert.Equal(ReferenceType.ManyToMany, userGroupsTable.Columns[1].Reference!.Type);
        }
    }
} 