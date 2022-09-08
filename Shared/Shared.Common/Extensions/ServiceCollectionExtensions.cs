using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NodaTime;
using NSwag.Generation.Processors;
using Serilog;
using Shared.Common.Constants;
using Shared.Common.NSwag.Processors;
using Shared.Common.NSwag.TypeMappers;

namespace Shared.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVerticalSlices(this IServiceCollection serviceCollection,
        IConfiguration configuraiton,
        params Assembly[] assemblies)
    {

        serviceCollection.AddSingleton<IClock>(SystemClock.Instance);
        serviceCollection.AddSingleton(SisJsonSerialization.Options);

        serviceCollection.AddHttpContextAccessor();

        return serviceCollection;
    }

    public static IServiceCollection AddCustomControllers(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                var builtInFactory = options.InvalidModelStateResponseFactory;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ValidationProblemDetails>>();
                    logger.LogWarning("Invalid request object provided. {@ValidationErrors}", new ValidationProblemDetails(context.ModelState));
                    return builtInFactory(context);
                };
            })
            .AddJsonOptions(options => SisJsonSerialization.ConfigureOptions(options.JsonSerializerOptions))
            .AddDapr(options => options.UseJsonSerializationOptions(SisJsonSerialization.Options));

        return serviceCollection;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection serviceCollection, string serviceName,
        string serviceVersion = "1")
    {
        var processorOptions = new FeatureSchemaProcessorOptions(ServiceName: serviceName,
            EnableCustomTypescript: true,
            EnableCustomerCSharp: true,
            GenerateDeepNamespace: true
        );

        serviceCollection
            .AddOpenApiDocument(options =>
            {
                options.Title = $"{serviceName} v{serviceVersion}";
                options.OperationProcessors.Add(new ApiVersionProcessor());
                options.OperationProcessors.Add(new RemoveVersionParameterProcessor());
                options.SchemaProcessors.Add(new SharedSchemaProcessor(processorOptions));
                options.SchemaProcessors.Add(new FeatureSchemaProcessor(processorOptions));
                options.SchemaNameGenerator = new FeatureSchemaProcessor(processorOptions);
                options.AddNodaTimeTypeMappings();
                options.AddOperationTypeMapping();

                options.SchemaGenerator.Settings.SchemaType = SchemaType.OpenApi3;
            })
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

        return serviceCollection;
    }

    public static IApplicationBuilder UseCustomSwaggerUI(this IApplicationBuilder app, string serviceName, string serviceVersion = "1")
    {
        app.UseOpenApi()
           .UseApiVersioning()
           .UseSwaggerUi3(options =>
           {
               options.DocumentTitle = $"{serviceName} v{serviceVersion}";
           });

        return app;
    }

    public static IApplicationBuilder UseCustomRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                void AddDiagnosticHeader(string key, string headerKey)
                {
                    if (httpContext.Request.Headers.ContainsKey(headerKey))
                    {
                        diagnosticContext.Set(key, httpContext.Request.Headers[headerKey].ToString());
                    }
                }

                AddDiagnosticHeader(nameof(Headers.TenantId), Headers.TenantId);
                AddDiagnosticHeader(nameof(Headers.SchoolId), Headers.SchoolId);
                AddDiagnosticHeader(nameof(Headers.PersonId), Headers.PersonId);
            };
        });

        return app;
    }

    public static IApplicationBuilder UseReviewEnvironment(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            var subdomains = context.Request.Host.Host.Split(".");
            if (subdomains is { Length: > 1 } && subdomains[1] == "review")
            {
                context.Request.Headers.Add(Headers.KubernetesRouteAs, subdomains[0]);
            }

            return next();
        });

        return app;
    }

    private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}
