namespace CleanArch.AspNetCore.UnitTests;

public class ResultExtensionsTests
{
    [Fact]
    public void Match_WhenIsSuccess_ThenCallsOnSuccess()
    {
        // Arrange
        var successCallback = new Mock<Func<bool>>();
        successCallback.Setup(c => c()).Returns(true);

        var failureCallback = new Mock<Func<Result, bool>>();
        failureCallback.Setup(c => c(It.IsAny<Result>())).Returns(false);

        var result = Result.Success();

        // Act
        var output = result.Match(successCallback.Object, failureCallback.Object);

        // Assert
        Assert.True(output);
        successCallback.Verify(c => c(), Times.Once);
        failureCallback.Verify(c => c(It.IsAny<Result>()), Times.Never);
    }

    [Fact]
    public void Match_WhenIsSuccessWithValue_ThenCallsOnSuccess()
    {
        // Arrange
        var successCallback = new Mock<Func<int, bool>>();
        successCallback.Setup(c => c(It.IsAny<int>())).Returns(true);

        var failureCallback = new Mock<Func<Result, bool>>();
        failureCallback.Setup(c => c(It.IsAny<Result>())).Returns(false);

        var result = Result.Success(1);

        // Act
        var output = result.Match(successCallback.Object, failureCallback.Object);

        // Assert
        Assert.True(output);
        successCallback.Verify(c => c(result.Value), Times.Once);
        failureCallback.Verify(c => c(It.IsAny<Result>()), Times.Never);
    }

    [Fact]
    public void Match_WhenIsFailure_ThenCallsOnFailure()
    {
        // Arrange
        var successCallback = new Mock<Func<bool>>();
        successCallback.Setup(c => c()).Returns(true);

        var failureCallback = new Mock<Func<Result, bool>>();
        failureCallback.Setup(c => c(It.IsAny<Result>())).Returns(false);

        var result = Result.Failure(new Error("ErrorCode", "ErrorDescription", ErrorType.Failure));

        // Act
        var output = result.Match(successCallback.Object, failureCallback.Object);

        // Assert
        Assert.False(output);
        failureCallback.Verify(c => c(result), Times.Once);
        successCallback.Verify(c => c(), Times.Never);
    }

    [Fact]
    public void Match_WhenIsFailureWithValue_ThenCallsOnFailure()
    {
        // Arrange
        var successCallback = new Mock<Func<int, bool>>();
        successCallback.Setup(c => c(It.IsAny<int>())).Returns(true);

        var failureCallback = new Mock<Func<Result<int>, bool>>();
        failureCallback.Setup(c => c(It.IsAny<Result<int>>())).Returns(false);

        var result = new Result<int>(1, false, Error.NullValue);

        // Act
        var output = result.Match(successCallback.Object, failureCallback.Object);

        // Assert
        Assert.False(output);
        failureCallback.Verify(c => c(result), Times.Once);
        successCallback.Verify(c => c(It.IsAny<int>()), Times.Never);
    }
}
