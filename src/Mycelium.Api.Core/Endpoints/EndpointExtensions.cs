using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Mycelium.Api.Core.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddOpenApi();

        var myceliumAssemblies = GetMyceliumAssemblies();

        foreach (var endpointType in myceliumAssemblies
                     .SelectMany(GetLoadableTypes)
                     .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(t)))
        {
            services.AddSingleton(typeof(IEndpoint), endpointType);
        }

        services.AddValidatorsFromAssemblies(myceliumAssemblies);

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var versionedGroups = new Dictionary<string, RouteGroupBuilder>();

        foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
        {
            var groupName = $"v{endpoint.Version.MajorVersion}.{endpoint.Version.MinorVersion}";
            if (!versionedGroups.TryGetValue(groupName, out var group))
            {
                var versionSet = app.NewApiVersionSet(groupName)
                    .HasApiVersion(endpoint.Version)
                    .ReportApiVersions()
                    .Build();

                group = app.MapGroup("/api/v{apiVersion:apiVersion}").WithApiVersionSet(versionSet);
                versionedGroups.Add(groupName, group);
            }

            endpoint.MapEndpoint(group);
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
            app.MapScalarApiReference().AllowAnonymous();
        }

        return app;
    }

    private static Assembly[] GetMyceliumAssemblies() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("Mycelium", StringComparison.Ordinal) == true)
            .ToArray();

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t is not null)!;
        }
    }
}
