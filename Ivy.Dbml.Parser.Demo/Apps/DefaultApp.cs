using System.Text.Json;
using System.Text.Json.Nodes;
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
        var model = UseState<JsonNode?>();
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
                model.Set((JsonNode?)null!);
                error.Set(e);
            }
        }, [EffectTrigger.AfterInit(), dbml]);
        
        var ux = new ResizeablePanelGroup(
            new ResizeablePanel(25,
                Layout.Horizontal().Height(Size.Full()).RemoveParentPadding()
                | dbml.ToCodeInput().Height(Size.Full()).Width(Size.Full()).Language(Languages.Dbml)),
            new ResizeablePanel(75, Layout.Vertical() | model | error)
        ).Height(Size.Screen());
        
        return ux;
    }
}