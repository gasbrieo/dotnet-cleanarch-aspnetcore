namespace CleanArch.AspNetCore.UnitTests;

public class EndpointExtensionsMapTests
{
    public interface IApiV1Endpoint : IEndpoint { }

    public class MyV1Endpoint : IApiV1Endpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
        }
    }

    [Fact]
    public void AddEndpoints_ThenRegistersThemAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEndpoints(Assembly.GetExecutingAssembly());
        var provider = services.BuildServiceProvider();

        // Assert
        var endpoints = provider.GetServices<IApiV1Endpoint>().ToList();
        Assert.NotEmpty(endpoints);
        Assert.Contains(endpoints, e => e.GetType() == typeof(MyV1Endpoint));
    }

    [Fact]
    public void MapEndpoints_ThenCallsMapEndpointForAllResolvedEndpoints()
    {
        // Arrange
        var fakeEndpoint = new Mock<IApiV1Endpoint>();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(fakeEndpoint.Object);

        var app = builder.Build();

        // Act
        app.MapEndpoints<IApiV1Endpoint>(new(1, 0));

        // Assert
        fakeEndpoint.Verify(x => x.MapEndpoint(It.IsAny<IEndpointRouteBuilder>()), Times.Once);
    }
}
