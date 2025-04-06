# DBML Parser for Dotnet

Database Markup Language (DBML), designed to define and document database structures. See the original [repository](https://github.com/holistics/dbml).

This is initially a port of https://github.com/nilswende/dbml-java with loads some additions and refactorings.

## Installation

You can install the package via NuGet Package Manager:

```bash
dotnet add package Ivy.Dbml.Parser
```

Or via the Package Manager Console:

```powershell
Install-Package Ivy.Dbml.Parser
```

## Usage

Here's a basic example of how to use the DBML Parser:

```csharp
using Ivy.Dbml.Parser.Parser;
using Ivy.Dbml.Parser.Models;

// Create a new parser instance
var parser = new DbmlParser();

// Parse DBML content
string dbmlContent = @"
Project MyDatabase {
  Note: 'My project description'
}

Table users {
  id integer [pk]
  name varchar [not null]
  email varchar [unique]
}

Table posts {
  id integer [pk]
  user_id integer
  title varchar
  body text [note: 'Post content']
}

Ref: posts.user_id > users.id
";

// Get the parsed model
DbmlModel model = parser.Parse(dbmlContent);

// Access the parsed data
var projectName = model.ProjectName;
var tables = model.Tables;
var references = model.References;

// Example: Print table names
foreach (var table in tables)
{
    Console.WriteLine($"Table: {table.Name}");
    foreach (var column in table.Columns)
    {
        Console.WriteLine($"  Column: {column.Name} ({column.Type})");
    }
}
```

## Features

- Parse DBML project definitions
- Parse table definitions with columns and their properties
- Support for table aliases and notes
- Parse indexes (single-column and multi-column)
- Parse references between tables
- Support for quoted identifiers
- Support for default values and constraints

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 
