using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Mycelium.Api.Core.Results;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? HttpResults.Ok() : result.Error.ToHttpResult();

    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.IsSuccess ? HttpResults.Ok(result.Value) : result.Error.ToHttpResult();

    public static IResult ToHttpResult(this Error error) => error.Type switch
    {
        ErrorType.Validation => HttpResults.Json(new { error = error.Message }, statusCode: StatusCodes.Status400BadRequest),
        ErrorType.NotFound => HttpResults.Json(new { error = error.Message }, statusCode: StatusCodes.Status404NotFound),
        ErrorType.Unauthorized => HttpResults.Json(new { error = error.Message }, statusCode: StatusCodes.Status401Unauthorized),
        ErrorType.Forbidden => HttpResults.Json(new { error = error.Message }, statusCode: StatusCodes.Status403Forbidden),
        ErrorType.Conflict => HttpResults.Json(new { error = error.Message }, statusCode: StatusCodes.Status409Conflict),
        _ => HttpResults.Json(new { error = "An unexpected error occurred." }, statusCode: StatusCodes.Status500InternalServerError)
    };
}
