using Ivy.Dbml.Parser.Parser;
using Xunit;

namespace Ivy.Dbml.Parser.Tests;

public class ValidationTests
{
    private readonly DbmlParser _parser;

    public ValidationTests()
    {
        _parser = new DbmlParser();
    }

    [Fact]
    public void Validate_ValidDbml_ReturnsSuccess()
    {
        var dbml = @"
Table Users {
  Id int [pk]
  Name string [not null]
}

Ref: Users.Id > Users.Id
";
        var result = _parser.Validate(dbml);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Model);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_MultipleSettingsBrackets_ReturnsError()
    {
        var dbml = @"
Table Users {
  Name string [not null] [unique]
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("multiple settings brackets", result.Errors[0].Message);
        Assert.Contains("Combine into one", result.Errors[0].Message);
    }

    [Fact]
    public void Validate_RefInsideTable_ReturnsError()
    {
        var dbml = @"
Table Orders {
  Id int [pk]
  UserId int
  Ref: Orders.UserId > Users.Id
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Ref declarations must be top-level", result.Errors[0].Message);
    }

    [Fact]
    public void Validate_EnumInsideTable_ReturnsError()
    {
        var dbml = @"
Table Orders {
  Id int [pk]
  enum Status {
    Active
    Inactive
  }
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("Enum declarations must be top-level"));
    }

    [Fact]
    public void Validate_NullableInsteadOfNull_ReturnsError()
    {
        var dbml = @"
Table Users {
  Name string [nullable]
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("[null]", result.Errors[0].Message);
        Assert.Contains("[nullable]", result.Errors[0].Message);
    }

    [Fact]
    public void Validate_EmptyEnum_ReturnsError()
    {
        var dbml = @"
enum Status {
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("no values", result.Errors[0].Message);
        Assert.Contains("Status", result.Errors[0].Message);
    }

    [Fact]
    public void Validate_Comments_ReturnsError()
    {
        var dbml = @"
// This is a comment
Table Users {
  Id int [pk]
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("Comments are not supported"));
    }

    [Fact]
    public void Validate_UnknownColumnType_ReturnsWarning()
    {
        var dbml = @"
Table Users {
  Id int [pk]
  Data FooBarType
}
";
        var result = _parser.Validate(dbml);

        Assert.Contains(result.Errors, e =>
            e.Message.Contains("Unknown column type 'FooBarType'") &&
            e.Severity == DbmlErrorSeverity.Warning);
    }

    [Fact]
    public void Validate_KnownColumnTypes_NoWarning()
    {
        var dbml = @"
Table Users {
  Id int [pk]
  Name string
  Created DateTime
  Key Guid
  Amount decimal
  Active bool
  Age long
  Score double
  Rating float
}
";
        var result = _parser.Validate(dbml);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_DuplicateTableNames_ReturnsError()
    {
        var dbml = @"
Table Users {
  Id int [pk]
}

Table Users {
  Id int [pk]
  Name string
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("already defined", result.Errors[0].Message);
        Assert.Contains("Users", result.Errors[0].Message);
    }

    [Fact]
    public void Validate_UnclosedTable_ReturnsError()
    {
        var dbml = @"
Table Users {
  Id int [pk]
  Name string
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Message.Contains("Unclosed") &&
            e.Message.Contains("Users") &&
            e.Message.Contains("missing closing brace"));
    }

    [Fact]
    public void Validate_UnrecognizedTopLevel_ReturnsError()
    {
        var dbml = @"
Foo bar baz
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Message.Contains("Unrecognized declaration 'Foo'") &&
            e.Message.Contains("Expected:"));
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAll()
    {
        var dbml = @"
// comment
Table Users {
  Name string [not null] [unique]
}

Table Users {
  Id int [pk]
}

enum EmptyEnum {
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }

    [Fact]
    public void Validate_ErrorsHaveCorrectLineNumbers()
    {
        var dbml = @"
Table Users {
  Name string [not null] [unique]
}
";
        var result = _parser.Validate(dbml);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors[0].Line);
    }

    [Fact]
    public void Validate_WorkflowStyleDbml_IsValid()
    {
        // Representative DBML matching the workflow's DbmlGeneratePrompt.md style
        var dbml = @"
enum OrderStatus {
  Pending
  Processing
  Shipped
  Delivered
  Cancelled
}

Table Customers {
  Id Guid [pk, not null]
  FirstName string [not null]
  LastName string [not null]
  Email string [not null, unique]
  CreatedAt DateTime [not null]
  UpdatedAt DateTime
}

Table Products {
  Id Guid [pk, not null]
  Name string [not null]
  Description string
  Price decimal [not null]
  StockCount int [not null]
  IsActive bool [not null]
}

Table Orders {
  Id Guid [pk, not null]
  CustomerId Guid [not null]
  Status OrderStatus [not null]
  TotalAmount decimal [not null]
  OrderDate DateTime [not null]
  ShippedDate DateTime
}

Table OrderItems {
  Id Guid [pk, not null]
  OrderId Guid [not null]
  ProductId Guid [not null]
  Quantity int [not null]
  UnitPrice decimal [not null]
}

Ref: Orders.CustomerId > Customers.Id
Ref: OrderItems.OrderId > Orders.Id
Ref: OrderItems.ProductId > Products.Id
";
        var result = _parser.Validate(dbml);

        Assert.True(result.IsValid, string.Join("\n", result.Errors.Select(e => e.ToString())));
        Assert.NotNull(result.Model);
        Assert.Equal(4, result.Model!.Tables.Count);
        Assert.Single(result.Model.Enums);
        Assert.Equal(3, result.Model.References.Count);
    }

    [Fact]
    public void Validate_InlineRefInColumn_IsValid()
    {
        // Inline refs in column settings are valid (not the same as top-level Ref inside table)
        var dbml = @"
Table Users {
  Id int [pk]
}

Table Posts {
  Id int [pk]
  UserId int [ref: > Users.Id]
}
";
        var result = _parser.Validate(dbml);

        Assert.True(result.IsValid, string.Join("\n", result.Errors.Select(e => e.ToString())));
    }
}
