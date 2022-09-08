using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Common.NSwag.Converters;

public sealed class JsonPatchDocumentConverter : JsonConverter<IJsonPatchDocument>
{
    internal static DefaultContractResolver DefaultContractResolver { get; } = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IJsonPatchDocument).IsAssignableFrom(typeToConvert);
    }

    public override IJsonPatchDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var opType = typeToConvert.IsGenericType
            ? typeof(Microsoft.AspNetCore.JsonPatch.Operations.Operation<>).MakeGenericType(typeToConvert.GenericTypeArguments[0])
            : typeof(Microsoft.AspNetCore.JsonPatch.Operations.Operation);

        var opListType = typeToConvert.IsGenericType
            ? typeof(List<>).MakeGenericType(opType)
            : typeof(List<Microsoft.AspNetCore.JsonPatch.Operations.Operation>);

        var targetOperations = JsonSerializer.Deserialize(ref reader, opListType, options) as IEnumerable<Microsoft.AspNetCore.JsonPatch.Operations.Operation>;
        var newtonsoftSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();

        foreach (var op in targetOperations!)
        {
            if (op.value is JsonElement element)
            {
                var sr = new Newtonsoft.Json.JsonTextReader(new StringReader(element.GetRawText()));
                op.value = newtonsoftSerializer.Deserialize(sr);
            }
        }

        return Activator.CreateInstance(typeToConvert, targetOperations, DefaultContractResolver) as IJsonPatchDocument;
    }

    public override void Write(Utf8JsonWriter writer, IJsonPatchDocument value, JsonSerializerOptions options)
    {
        if (value is IJsonPatchDocument jsonPatchDoc)
        {
            writer.WriteStartArray();

            foreach (var operation in jsonPatchDoc.GetOperations())
            {
                writer.WriteStartObject();
                writer.WriteString("op", operation.op);
                writer.WriteString("path", operation.path);
                writer.WriteString("from", operation.from);

                if (operation.value != null)
                {
                    writer.WritePropertyName("name");
                    JsonSerializer.Serialize(writer, operation.value, options);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.Flush();
        }
    }
}
