using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Mediator.Behaviors;
using Mycelium.Api.Core.Security;

namespace Mycelium.Api.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApiCore(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddEndpoints();

        return services;
    }
}
