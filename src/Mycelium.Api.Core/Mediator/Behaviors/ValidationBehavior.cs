using FluentValidation;
using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Core.Mediator.Behaviors;

public class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TMessage>(message);
        var failures = validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(message, cancellationToken);
        }

        var errorMessage = string.Join(" ", failures.Select(failure => failure.ErrorMessage));
        return ResultFactory<TResponse>.Failure(Error.Validation(errorMessage));
    }
}
