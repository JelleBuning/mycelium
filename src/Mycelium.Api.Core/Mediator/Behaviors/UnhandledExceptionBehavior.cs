using Mediator;
using Microsoft.Extensions.Logging;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Core.Mediator.Behaviors;

/// <summary>
/// Last-resort safety net: handlers are expected to return Result.Failure for
/// expected failures (not found, forbidden, etc.), so anything that throws here
/// is a genuinely unexpected failure (e.g. a database exception).
/// </summary>
public class UnhandledExceptionBehavior<TMessage, TResponse>(
    ILogger<UnhandledExceptionBehavior<TMessage, TResponse>> logger
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for request {MessageName} {@Message}", typeof(TMessage).Name, message);
            return ResultFactory<TResponse>.Failure(Error.Unexpected("An unexpected error occurred."));
        }
    }
}
