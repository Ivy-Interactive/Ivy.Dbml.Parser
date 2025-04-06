using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class EnumParserTests
{
    private readonly DbmlParser _parser;

    public EnumParserTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseSimpleEnum()
    {
        var dbml = @"
enum job_status {
  created
  running
  done
  failure
}

Table jobs {
  id integer [pk]
  status job_status
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Enums);
        var enumObj = model.Enums[0];
        Assert.Equal("job_status", enumObj.Name);
        Assert.Null(enumObj.Schema);
        Assert.Null(enumObj.Note);
        Assert.Equal(4, enumObj.Values.Count);
        Assert.Equal("created", enumObj.Values[0]);
        Assert.Equal("running", enumObj.Values[1]);
        Assert.Equal("done", enumObj.Values[2]);
        Assert.Equal("failure", enumObj.Values[3]);
    }

    [Fact]
    public void ParseEnumWithSchema()
    {
        var dbml = @"
enum v2.job_status {
  created
  running
  done
  failure
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Enums);
        var enumObj = model.Enums[0];
        Assert.Equal("job_status", enumObj.Name);
        Assert.Equal("v2", enumObj.Schema);
    }

    [Fact]
    public void ParseEnumWithValueNotes()
    {
        var dbml = @"
enum job_status {
  created [note: 'Waiting to be processed']
  running
  done
  failure
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Enums);
        var enumObj = model.Enums[0];
        Assert.Equal(4, enumObj.Values.Count);
        Assert.Equal("created", enumObj.Values[0]);
        Assert.Equal("Waiting to be processed", enumObj.ValueNotes["created"]);
    }

    [Fact]
    public void ParseEnumWithQuotedValues()
    {
        var dbml = @"
enum grade {
  ""A+""
  ""A""
  ""A-""
  ""Not Yet Set""
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Enums);
        var enumObj = model.Enums[0];
        Assert.Equal(4, enumObj.Values.Count);
        Assert.Equal("A+", enumObj.Values[0]);
        Assert.Equal("A", enumObj.Values[1]);
        Assert.Equal("A-", enumObj.Values[2]);
        Assert.Equal("Not Yet Set", enumObj.Values[3]);
    }
} 