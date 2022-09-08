using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Shared.SourceGenerators.DaprNSwagClient
{
    [Generator]
    public class DaprNSwagClientGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DaprNSwagClientSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is DaprNSwagClientSyntaxReceiver receiver))
                return;

            foreach (var @class in receiver.Classes)
            {
                string source = $@"using System.Text.Json;
using Shared.Common.Extensions;

namespace {@class.ContainingNamespace};

public partial class {@class.Name}
{{
    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public {@class.Name}(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions) : this(Microservice.AppId, httpClient)
    {{
        _settings = new System.Lazy<System.Text.Json.JsonSerializerOptions>(() => jsonSerializerOptions);
    }}
}}";
                context.AddSource($"{@class.Name}.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
            }
        }
    }
}