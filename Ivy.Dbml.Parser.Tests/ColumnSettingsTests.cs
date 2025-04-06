using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ColumnSettingsTests
{
    private readonly DbmlParser _parser;

    public ColumnSettingsTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void ParseColumnWithIncrement()
    {
        var dbml = @"
Table users {
  id integer [pk, increment]
}";

        var model = _parser.Parse(dbml);
        var column = model.Tables[0].Columns[0];

        Assert.True(column.IsPrimaryKey);
        Assert.True(column.IsIncrement);
    }

    [Fact]
    public void ParseColumnWithLength()
    {
        var dbml = @"
Table users {
  name varchar(100)
  description text(1000)
}";

        var model = _parser.Parse(dbml);
        var nameColumn = model.Tables[0].Columns[0];
        var descriptionColumn = model.Tables[0].Columns[1];

        Assert.Equal("varchar", nameColumn.Type);
        Assert.Equal(100, nameColumn.Length);
        Assert.Equal("text", descriptionColumn.Type);
        Assert.Equal(1000, descriptionColumn.Length);
    }

    [Fact]
    public void ParseColumnWithPrecisionAndScale()
    {
        var dbml = @"
Table products {
  price decimal(10,2)
  quantity numeric(5,0)
}";

        var model = _parser.Parse(dbml);
        var priceColumn = model.Tables[0].Columns[0];
        var quantityColumn = model.Tables[0].Columns[1];

        Assert.Equal("decimal", priceColumn.Type);
        Assert.Equal(10, priceColumn.Precision);
        Assert.Equal(2, priceColumn.Scale);
        Assert.Equal("numeric", quantityColumn.Type);
        Assert.Equal(5, quantityColumn.Precision);
        Assert.Equal(0, quantityColumn.Scale);
    }

    [Fact]
    public void ParseColumnWithUnsigned()
    {
        var dbml = @"
Table products {
  id integer [pk, unsigned]
  quantity integer [unsigned]
}";

        var model = _parser.Parse(dbml);
        var idColumn = model.Tables[0].Columns[0];
        var quantityColumn = model.Tables[0].Columns[1];

        Assert.True(idColumn.IsUnsigned);
        Assert.True(quantityColumn.IsUnsigned);
    }

    [Fact]
    public void ParseColumnWithDefaultValue()
    {
        var dbml = @"
Table users {
  created_at timestamp [default: `CURRENT_TIMESTAMP`]
  status varchar [default: 'active']
  is_deleted boolean [default: false]
}";

        var model = _parser.Parse(dbml);
        var createdAtColumn = model.Tables[0].Columns[0];
        var statusColumn = model.Tables[0].Columns[1];
        var isDeletedColumn = model.Tables[0].Columns[2];

        Assert.Equal("CURRENT_TIMESTAMP", createdAtColumn.DefaultValue);
        Assert.Equal("'active'", statusColumn.DefaultValue);
        Assert.Equal("false", isDeletedColumn.DefaultValue);
    }
} 