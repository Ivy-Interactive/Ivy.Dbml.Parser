using System.Text.Json;
using Ivy.Dbml.Parser.Parser;

namespace Ivy.Dbml.Parser.Demo.Apps;

[App()]
public class DefaultApp : ViewBase
{
    private readonly string _initialDbml =
        """
        Table Person {
            id integer [pk, increment]
            name varchar(255)
            age integer
            created_at datetime
            updated_at datetime
        }
        """;

    public override object? Build()
    {
        var dbml = UseState(_initialDbml);
        var model = UseState<string?>();
        var error = UseState<Exception?>();

        UseEffect(() =>
        {
            try
            {
                var parser = new DbmlParser();
                var result = parser.Parse(dbml.Value);
                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                model.Set(json);
                error.Set((Exception?)null!);
            }
            catch (Exception e)
            {
                model.Set((string?)null!);
                error.Set(e);
            }
        }, [EffectTrigger.OnMount(), dbml]);

        var ux = new ResizablePanelGroup(
            new ResizablePanel(Size.Fraction(0.25f),
                Layout.Horizontal().Height(Size.Full()).RemoveParentPadding()
                | dbml.ToCodeInput().Height(Size.Full()).Width(Size.Full()).Language(Languages.Dbml)),
            new ResizablePanel(Size.Fraction(0.75f), Layout.Vertical() | model | error)
        ).Height(Size.Screen());

        return ux;
    }
}
