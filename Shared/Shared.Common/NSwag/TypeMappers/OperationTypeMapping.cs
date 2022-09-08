using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.AspNetCore;

namespace Shared.Common.NSwag.TypeMappers;

public static class OperationTypeMapping
{
    internal class Operation
    {
        public string op { get; set; }
        public string path { get; set; }
        public object value { get; set; }
    }

    public static void AddOperationTypeMapping(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        var schema = settings.SchemaGenerator.Generate(typeof(Operation));
        schema.ExtensionData = new Dictionary<string, object>();
        schema.ExtensionData["x-csharp-shared-type"] = "Microsoft.AspNetCore.JsonPatch.Operations.Operation";

        settings.TypeMappers.Add(new ObjectTypeMapper(typeof(Microsoft.AspNetCore.JsonPatch.Operations.Operation), schema));
    }
}
