using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class NoteParserTests
{
    private readonly DbmlParser _parser;

    public NoteParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseTableNote()
    {
        var dbml = @"
Table users {
  id int [pk]
  name varchar

  Note: 'Stores user data'
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal("users", table.Name);
        Assert.Equal("Stores user data", table.Note);
    }
} 