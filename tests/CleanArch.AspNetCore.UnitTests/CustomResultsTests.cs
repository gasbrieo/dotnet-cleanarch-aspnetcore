namespace CleanArch.AspNetCore.UnitTests;

public class CustomResultsTests
{
    [Fact]
    public void Problem_WhenResultIsSuccess_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result.Success();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => CustomResults.Problem(result));
    }

    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest, "https://tools.ietf.org/html/rfc7231#section-6.5.1")]
    [InlineData(ErrorType.Problem, StatusCodes.Status400BadRequest, "https://tools.ietf.org/html/rfc7231#section-6.5.1")]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound, "https://tools.ietf.org/html/rfc7231#section-6.5.4")]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict, "https://tools.ietf.org/html/rfc7231#section-6.5.8")]
    [InlineData(ErrorType.Failure, StatusCodes.Status500InternalServerError, "https://tools.ietf.org/html/rfc7231#section-6.6.1")]
    public void Problem_WhenResultHasError_ShouldMapErrorTypeToExpectedStatusAndLink(ErrorType type, int expectedStatus, string expectedLink)
    {
        // Arrange
        var error = new Error("Some.Code", "Some description", type);
        var result = Result.Failure(error);

        // Act
        var httpResult = CustomResults.Problem(result);
        var problem = httpResult as ProblemHttpResult;

        // Assert
        Assert.NotNull(problem);
        Assert.Equal(expectedStatus, problem!.StatusCode);
        Assert.Equal(expectedLink, problem.ProblemDetails.Type);

        if (type == ErrorType.Failure)
        {
            Assert.Equal("Server failure", problem.ProblemDetails.Title);
            Assert.Equal("An unexpected error occurred", problem.ProblemDetails.Detail);
        }
        else
        {
            Assert.Equal("Some.Code", problem.ProblemDetails.Title);
            Assert.Equal("Some description", problem.ProblemDetails.Detail);
        }
    }

    [Fact]
    public void Problem_WhenErrorListIsUsed_ShouldAddErrorsToExtensions()
    {
        // Arrange
        var errorList = new ErrorList("Validation.General",
        [
            Error.Problem("Validation.Email", "Invalid email"),
            Error.Problem("Validation.Password", "Weak password")
        ]);

        var result = Result.Failure(errorList);

        // Act
        var httpResult = CustomResults.Problem(result);
        var problem = httpResult as ProblemHttpResult;

        // Assert
        Assert.NotNull(problem);
        Assert.True(problem!.ProblemDetails.Extensions.ContainsKey("errors"));

        var errors = problem.ProblemDetails.Extensions["errors"] as IEnumerable<Error>;
        Assert.NotNull(errors);
        Assert.Equal(2, errors!.Count());
    }
}
