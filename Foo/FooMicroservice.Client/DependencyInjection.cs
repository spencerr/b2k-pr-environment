﻿using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace FooMicroservice.Client;

public static class DependencyInjection
{

    public static IServiceCollection AddFooMicroservice(this IServiceCollection serviceCollection)
    {
        return serviceCollection.Scan(scan => scan.FromAssemblyOf<Microservice>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Client") && type.GetInterface($"I{type.Name}") != null))
            .UsingRegistrationStrategy(new HttpClientRegistrationStrategy())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

}

public class HttpClientRegistrationStrategy : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
    {
        var method = typeof(HttpClientFactoryServiceCollectionExtensions)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(m => m.Name == nameof(HttpClientFactoryServiceCollectionExtensions.AddHttpClient))
            .Where(m => m.GetGenericArguments() is { Length: 2 })
            .Where(m => m.GetParameters() is { Length: 2 } parameters
                && parameters[0].ParameterType == typeof(IServiceCollection)
                && parameters[1].ParameterType == typeof(string))
            .First();

        var genericMethod = method.MakeGenericMethod(descriptor.ServiceType, descriptor.ImplementationType!);

        var httpClientBuilder = genericMethod.Invoke(null, new object[] { services, $"foo-{descriptor.ServiceType.Name}" }) as IHttpClientBuilder;
        httpClientBuilder!.AddHeaderPropagation();
    }
}