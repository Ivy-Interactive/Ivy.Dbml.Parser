using Ivy.Dbml.Parser.Models;
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
        var model = UseState<DbmlModel?>();
        
        UseEffect(() =>
        {
            var parser = new DbmlParser();
            var result = parser.Parse(dbml.Value);
            model.Set(result);
        }, [dbml]);
        
        var ux = new ResizeablePanelGroup(
            new ResizeablePanel(25,
                Layout.Horizontal().Height(Size.Full()).RemoveParentPadding()
                | dbml.ToCodeInput().Height(Size.Full()).Width(Size.Full()).Language(Languages.Dbml)),
            new ResizeablePanel(75, dbml)
        ).Height(Size.Screen());
        
        return ux;
    }
}