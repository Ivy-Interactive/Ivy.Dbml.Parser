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
    public void ParseSingleLineNote()
    {
        var dbml = @"
Note single_line_note {
  'This is a single line note'
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Notes);
        var note = model.Notes[0];
        Assert.Equal("single_line_note", note.Name);
        Assert.Equal("This is a single line note", note.Content);
    }

    [Fact]
    public void ParseMultiLineNote()
    {
        var dbml = @"
Note multiple_lines_note {
'''
  This is a multiple lines note
  This string can spans over multiple lines.
'''
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Notes);
        var note = model.Notes[0];
        Assert.Equal("multiple_lines_note", note.Name);
        Assert.Equal("  This is a multiple lines note\n  This string can spans over multiple lines.", note.Content);
    }

    [Fact]
    public void ParseProjectNote()
    {
        var dbml = @"
Project DBML {
  Note: '''
  # DBML - Database Markup Language
  DBML is a simple, readable DSL language designed to define database structures.
  '''
}";

        var model = _parser.Parse(dbml);

        Assert.Equal("DBML", model.ProjectName);
        Assert.Equal("  # DBML - Database Markup Language\n  DBML is a simple, readable DSL language designed to define database structures.", model.Note);
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

    [Fact]
    public void ParseColumnNote()
    {
        var dbml = @"
Table orders {
  id int [pk]
  status varchar [
  note: '''
  💸 1 = processing,
  ✔️ 2 = shipped,
  ❌ 3 = cancelled,
  😔 4 = refunded
  ''']
}";

        var model = _parser.Parse(dbml);

        Assert.Single(model.Tables);
        var table = model.Tables[0];
        Assert.Equal(2, table.Columns.Count);
        
        var statusColumn = table.Columns[1];
        Assert.Equal("status", statusColumn.Name);
        Assert.Equal("  💸 1 = processing,\n  ✔️ 2 = shipped,\n  ❌ 3 = cancelled,\n  😔 4 = refunded", statusColumn.Note);
    }
} 