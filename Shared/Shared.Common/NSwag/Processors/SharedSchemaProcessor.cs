using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using System.Text.RegularExpressions;

namespace Shared.Common.NSwag.Processors;

public class SharedSchemaProcessor : ISchemaProcessor
{
    private readonly List<Regex> NamespaceRegex = new List<Regex>
    {
        new Regex("^SIS\\.Common\\."),
        new Regex("^SIS\\.Events\\."),
        new Regex("^SIS\\.([a-zA-Z0-9]*?)MS\\.Events"),
    };

    private readonly List<Regex> ClassNameRegex = new List<Regex>
    {
        new Regex("^NodaTime")
        //new Regex("^Microsoft\\.AspNetCore\\.JsonPatch\\.Operations\\.Operation")
    };

    private readonly FeatureSchemaProcessorOptions _options;

    public SharedSchemaProcessor(FeatureSchemaProcessorOptions options, IEnumerable<Regex>? namespaceRegexs = null, IEnumerable<Regex>? classRegex = null)
    {
        _options = options;
        NamespaceRegex.AddRange(namespaceRegexs ?? Enumerable.Empty<Regex>());
        ClassNameRegex.AddRange(classRegex ?? Enumerable.Empty<Regex>());
    }

    public void Process(SchemaProcessorContext context)
    {
        var fullName = context.Type.FullName!;
        if (!NamespaceRegex.Any(r => r.IsMatch(fullName)) && !ClassNameRegex.Any(r => r.IsMatch(fullName)))
            return;

        context.Schema.ExtensionData ??= new Dictionary<string, object>();

        var sharedTypeName = fullName;
        if (context.Type.IsGenericType)
        {
            var genericType = context.Type.GetGenericTypeDefinition()!;
            var genericFullName = genericType.FullName!;
            var genericTypeName = genericFullName[..genericFullName.LastIndexOf('`')];

            var genericArguments = string.Join(", ", context.Type.GetGenericArguments().Select(GetSchemaType));
            sharedTypeName = $"{genericTypeName}<{genericArguments}>";
        }

        if (_options.EnableCustomerCSharp)
        {
            context.Schema.ExtensionData["x-csharp-shared-type"] = sharedTypeName;
        }

        string GetSchemaType(Type type)
        {
            var schema = context.Resolver.GetSchema(type, false);

            if (schema.ExtensionData?.TryGetValue("x-csharp-shared-type", out var externalType) == true)
            {
                return externalType.ToString()!;
            }

            if (context.Resolver.RootObject is OpenApiDocument document)
            {
                var kvp = document.Components.Schemas.FirstOrDefault(s => s.Value == schema);
                if (kvp.Value.ExtensionData?.TryGetValue("x-namespace", out var feature) == true)
                {
                    var featureName = feature.ToString()!.Replace(".", "");
                    return kvp.Key.Replace($"{featureName}_", $"{feature}.");
                }

                return kvp.Key;
            }

            var typeNamespace = type.Namespace!;
            var featurePrefix = $"SIS.{_options.ServiceName}.Features.";
            if (typeNamespace.StartsWith(featurePrefix))
            {
                var featureNamespace = SchemaHelpers.GetFeatureNamespace(typeNamespace, featurePrefix, _options.GenerateDeepNamespace);

                if (!string.IsNullOrEmpty(featureNamespace))
                {
                    return $"{string.Join(".", featureNamespace)}.{type.Name}";
                }
            }

            return type.Name;
        }
    }
}
