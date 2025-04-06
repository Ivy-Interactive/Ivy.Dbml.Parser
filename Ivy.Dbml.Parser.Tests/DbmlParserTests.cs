using System;
using Xunit;
using Ivy.Dbml.Parser.Parser;

namespace Ivy.Dbml.Parser.Tests
{
    public class DbmlParserTests
    {
        [Fact]
        public void Parse_ShouldThrowInvalidSyntaxException_ForInvalidMultiLineNoteSyntax()
        {
            // Arrange
            var parser = new DbmlParser();
            var invalidDbml = "Note: '''Invalid note syntax";

            // Act & Assert
            var exception = Assert.Throws<InvalidSyntaxException>(() => parser.Parse(invalidDbml));
            Assert.Contains("Invalid multi-line note syntax", exception.Message);
        }

        [Fact]
        public void Parse_ShouldThrowMissingElementException_ForUnexpectedLine()
        {
            // Arrange
            var parser = new DbmlParser();
            var invalidDbml = "Unexpected content";

            // Act & Assert
            var exception = Assert.Throws<MissingElementException>(() => parser.Parse(invalidDbml));
            Assert.Contains("Unexpected line", exception.Message);
        }

        [Fact]
        public void Parse_ShouldIncludeLineNumberInExceptionMessage()
        {
            // Arrange
            var parser = new DbmlParser();
            var invalidDbml = "\n\nNote: '''Invalid note syntax";

            // Act & Assert
            var exception = Assert.Throws<InvalidSyntaxException>(() => parser.Parse(invalidDbml));
            Assert.Contains("line 3", exception.Message);
        }
    }
} 