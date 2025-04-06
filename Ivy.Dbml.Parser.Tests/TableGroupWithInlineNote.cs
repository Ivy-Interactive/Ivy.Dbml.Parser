using Ivy.Dbml.Parser.Parser;
using Xunit;

namespace Ivy.Dbml.Parser.Tests
{
    public class TableGroupWithInlineNoteTest
    {
        [Fact]
        public void TestInlineNote()
        {
            var dbml = @"
TableGroup e_commerce {
  users
  posts
  
  Note: 'Contains tables that are related to e-commerce system'
}";
            
            var parser = new DbmlParser();
            var model = parser.Parse(dbml);
            
            Assert.Single(model.TableGroups);
            var tableGroup = model.TableGroups[0];
            Assert.Equal("e_commerce", tableGroup.Name);
            Assert.Equal("Contains tables that are related to e-commerce system", tableGroup.Note);
        }
    }
} 