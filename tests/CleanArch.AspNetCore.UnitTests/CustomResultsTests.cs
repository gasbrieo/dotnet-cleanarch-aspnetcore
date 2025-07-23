namespace CleanArch.AspNetCore.UnitTests;

public class CustomResultsTests
{
    [Fact]
    public void Problem_WhenResultIsSuccess_ThenThrowsInvalidOperationException()
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
    public void Problem_WhenResultHasError_ThenMapsErrorTypeToExpectedStatusAndLink(ErrorType type, int expectedStatus, string expectedLink)
    {
        // Arrange
        var error = new Error("Some.Code", "Some description", type);
        var result = Result.Failure(error);

        // Act
        var httpResult = CustomResults.Problem(result);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(httpResult);

        Assert.NotNull(problem);
        Assert.Equal(expectedStatus, problem!.StatusCode);
        Assert.Equal(expectedLink, problem.ProblemDetails.Type);

        if (type == ErrorType.Failure)
        {
            Assert.Equal("Server failure", problem.ProblemDetails.Title);
            Assert.Equal("An unexpected error occurred", problem.ProblemDetails.Detail);
        }
        else if (type == ErrorType.Validation)
        {
            Assert.Equal("Validation failure", problem.ProblemDetails.Title);
            Assert.Equal("Some description", problem.ProblemDetails.Detail);
        }
        else
        {
            Assert.Equal("Some.Code", problem.ProblemDetails.Title);
            Assert.Equal("Some description", problem.ProblemDetails.Detail);
        }
    }

    [Fact]
    public void Problem_WhenErrorListIsUsed_ThenAddErrorsToExtensions()
    {
        // Arrange
        var errorList = new ErrorList("Validation.General",
        [
            Error.Validation("Email", "Invalid email"),
            Error.Validation("Password", "Weak password"),
            Error.Validation("Password", "Too short")
        ]);

        var result = Result.Failure(errorList);

        // Act
        var httpResult = CustomResults.Problem(result);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        var validationResult = Assert.IsType<HttpValidationProblemDetails>(problem.ProblemDetails);

        Assert.NotNull(validationResult);
        Assert.Equal(StatusCodes.Status400BadRequest, validationResult.Status);
        Assert.Equal("One or more validation errors occurred", validationResult.Title);

        var fieldErrors = validationResult.Errors;

        Assert.True(fieldErrors.ContainsKey("Email"));
        Assert.Single(fieldErrors["Email"]);
        Assert.Equal("Invalid email", fieldErrors["Email"].First());

        Assert.True(fieldErrors.ContainsKey("Password"));
        Assert.Equal(2, fieldErrors["Password"].Length);
        Assert.Contains("Weak password", fieldErrors["Password"]);
        Assert.Contains("Too short", fieldErrors["Password"]);
    }
}
