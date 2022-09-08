using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Shared.Common.NSwag.Converters;

namespace Shared.Common.Extensions;

public static class SisJsonSerialization
{
    private static JsonSerializerOptions _options;
    public static JsonSerializerOptions Options => _options;

    static SisJsonSerialization()
    {
        _options = ConfigureOptions(new JsonSerializerOptions());
    }

    public static JsonSerializerOptions ConfigureOptions(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonPatchDocumentConverter());
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        return _options = options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }

    public static string Serialize(this object @object)
        => JsonSerializer.Serialize(@object, Options);

    public static T Deserialize<T>(this string @string)
        => JsonSerializer.Deserialize<T>(@string, Options)!;

}
