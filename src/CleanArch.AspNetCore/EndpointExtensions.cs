namespace CleanArch.AspNetCore;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpoints = assembly
            .DefinedTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(t))
            .SelectMany(t =>
            {
                var interfaces = t.GetInterfaces()
                    .Where(i => typeof(IEndpoint).IsAssignableFrom(i) && i != typeof(IEndpoint));
                return interfaces.Select(i => ServiceDescriptor.Transient(i, t));
            });

        services.TryAddEnumerable(endpoints);

        return services;
    }

    public static IApplicationBuilder MapEndpoints<TEndpoint>(this WebApplication app, ApiVersion version) where TEndpoint : IEndpoint
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(version)
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(version);

        var endpoints = app.Services.GetRequiredService<IEnumerable<TEndpoint>>();

        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(group);

        return app;
    }
}
