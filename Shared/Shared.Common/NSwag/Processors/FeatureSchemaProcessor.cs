using NJsonSchema.Generation;

namespace Shared.Common.NSwag.Processors;

public record FeatureSchemaProcessorOptions(
    string ServiceName,
    bool EnableCustomTypescript,
    bool EnableCustomerCSharp,
    bool GenerateDeepNamespace = true
);

public class FeatureSchemaProcessor : DefaultSchemaNameGenerator, ISchemaProcessor, ISchemaNameGenerator
{
    private readonly FeatureSchemaProcessorOptions _options;

    public FeatureSchemaProcessor(FeatureSchemaProcessorOptions options)
    {
        _options = options;
    }

    public override string Generate(Type type)
    {
        var typeNamespace = type.Namespace!;
        var featurePrefix = $"SIS.{_options.ServiceName}.Features.";
        if (typeNamespace.StartsWith(featurePrefix))
        {
            var featureNamespace = SchemaHelpers.GetFeatureNamespace(typeNamespace, featurePrefix, _options.GenerateDeepNamespace, string.Empty);

            if (!string.IsNullOrEmpty(featureNamespace))
            {
                return $"{featureNamespace}_{type.Name}";
            }
        }

        return base.Generate(type);
    }

    public void Process(SchemaProcessorContext context)
    {
        var typeNamespace = context.Type.Namespace!;

        var featurePrefix = $"SIS.{_options.ServiceName}.Features.";
        if (typeNamespace.StartsWith(featurePrefix))
        {
            var featureNamespace = SchemaHelpers.GetFeatureNamespace(typeNamespace, featurePrefix, _options.GenerateDeepNamespace);

            if (string.IsNullOrEmpty(featureNamespace))
                return;

            context.Schema.ExtensionData ??= new Dictionary<string, object>();

            if (_options.EnableCustomerCSharp)
            {
                context.Schema.ExtensionData["x-namespace"] = string.Join(".", featureNamespace);
                context.Schema.ExtensionData["x-type"] = context.Type.Name;
            }
        }
    }
}
