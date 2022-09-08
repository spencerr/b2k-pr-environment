using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NodaTime;
using NSwag.Generation.AspNetCore;

namespace Shared.Common.NSwag.TypeMappers;

public static class NodaTimeTypeMapping
{
    private static readonly PrimitiveTypeMapper[] TypeMappings = new[]
        {
            CreateTypeMapper(typeof(DateInterval), "date-interval"),
            CreateTypeMapper(typeof(DateTimeZone), "date-time-zone"),
            CreateTypeMapper(typeof(Duration), "duration"),
            CreateTypeMapper(typeof(Instant), "instant"),
            CreateTypeMapper(typeof(Interval), "interval"),
            CreateTypeMapper(typeof(IsoDayOfWeek), "iso-day-of-week"),
            CreateTypeMapper(typeof(LocalDate), "local-date"),
            CreateTypeMapper(typeof(LocalDateTime), "local-date-time"),
            CreateTypeMapper(typeof(LocalTime), "local-time"),
            CreateTypeMapper(typeof(Offset), "offset"),
            CreateTypeMapper(typeof(OffsetDate), "offset-date"),
            CreateTypeMapper(typeof(OffsetDateTime), "offset-date-time"),
            CreateTypeMapper(typeof(OffsetTime), "offset-time"),
            CreateTypeMapper(typeof(Period), "period"),
            CreateTypeMapper(typeof(ZonedDateTime), "zoned-date-time"),
        };

    static PrimitiveTypeMapper CreateTypeMapper(Type type, string name)
    {
        return new PrimitiveTypeMapper(type, s =>
        {
            s.Type = JsonObjectType.String;
            s.Format = "noda-time-" + name;
            s.ExtensionData = new Dictionary<string, object>
            {
                ["x-csharp-shared-type"] = type.FullName
            };
        });
    }

    public static void AddNodaTimeTypeMappings(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        foreach (var typeMapper in TypeMappings)
        {
            settings.TypeMappers.Add(typeMapper);
        }
    }
}