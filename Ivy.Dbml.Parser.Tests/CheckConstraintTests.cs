using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class CheckConstraintTests
{
    private readonly DbmlParser _parser;

    public CheckConstraintTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseColumn_WithCheckConstraint()
    {
        var dbml = "Table t {\n  age int [check: `age > 0`]\n}";
        var model = _parser.Parse(dbml);
        Assert.Equal("age > 0", model.Tables[0].Columns[0].Check);
    }

    [Fact]
    public void ParseColumn_WithCheckAndOtherSettings()
    {
        var dbml = "Table t {\n  age int [not null, check: `age > 0`]\n}";
        var model = _parser.Parse(dbml);
        var column = model.Tables[0].Columns[0];
        Assert.True(column.IsNotNull);
        Assert.Equal("age > 0", column.Check);
    }

    [Fact]
    public void ParseTable_WithChecksBlock()
    {
        var dbml = "Table t {\n  start_date date\n  end_date date\n  checks {\n    `start_date < end_date` [name: 'date_check']\n  }\n}";
        var model = _parser.Parse(dbml);
        Assert.Single(model.Tables[0].Checks);
        Assert.Equal("start_date < end_date", model.Tables[0].Checks[0].Expression);
        Assert.Equal("date_check", model.Tables[0].Checks[0].Name);
    }

    [Fact]
    public void ParseTable_WithChecksBlock_NoName()
    {
        var dbml = "Table t {\n  age int\n  checks {\n    `age > 0`\n  }\n}";
        var model = _parser.Parse(dbml);
        Assert.Single(model.Tables[0].Checks);
        Assert.Equal("age > 0", model.Tables[0].Checks[0].Expression);
        Assert.Null(model.Tables[0].Checks[0].Name);
    }

    [Fact]
    public void ParseTable_WithMultipleChecks()
    {
        var dbml = @"
Table orders {
  price int
  quantity int
  start_date date
  end_date date
  checks {
    `price > 0` [name: 'positive_price']
    `quantity > 0`
    `start_date < end_date` [name: 'date_check']
  }
}";
        var model = _parser.Parse(dbml);
        Assert.Equal(3, model.Tables[0].Checks.Count);
        Assert.Equal("price > 0", model.Tables[0].Checks[0].Expression);
        Assert.Equal("positive_price", model.Tables[0].Checks[0].Name);
        Assert.Equal("quantity > 0", model.Tables[0].Checks[1].Expression);
        Assert.Null(model.Tables[0].Checks[1].Name);
        Assert.Equal("start_date < end_date", model.Tables[0].Checks[2].Expression);
        Assert.Equal("date_check", model.Tables[0].Checks[2].Name);
    }
}
