namespace Mycelium.Api.Core.Results;

/// <summary>
/// Builds a failed TResponse (Result or Result&lt;T&gt;) without the caller needing
/// to know which of the two it closes over. The reflection cost is paid once per
/// closed generic TResponse, cached in the static field below.
/// </summary>
internal static class ResultFactory<TResponse> where TResponse : Result
{
    public static readonly Func<Error, TResponse> Failure = CreateFailureFactory();

    private static Func<Error, TResponse> CreateFailureFactory()
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return error => (TResponse)(object)Result.Failure(error);
        }

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var method = typeof(Result)
            .GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition)
            .MakeGenericMethod(valueType);

        return error => (TResponse)method.Invoke(null, [error])!;
    }
}
