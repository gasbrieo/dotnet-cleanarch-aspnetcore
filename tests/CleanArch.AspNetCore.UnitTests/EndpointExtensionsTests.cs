using Moq;

namespace CleanArch.AspNetCore.UnitTests;

public class EndpointExtensionsMapTests
{
    public sealed class MyEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
        }
    }

    [Fact]
    public void AddEndpoints_ShouldRegisterThemAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEndpoints(Assembly.GetExecutingAssembly());
        var provider = services.BuildServiceProvider();

        // Assert
        var endpoints = provider.GetServices<IEndpoint>().ToList();
        Assert.NotEmpty(endpoints);
        Assert.Contains(endpoints, e => e.GetType() == typeof(MyEndpoint));
    }

    [Fact]
    public void MapEndpoints_ShouldCallMapEndpointForAllResolvedEndpoints()
    {
        // Arrange
        var fakeEndpoint = new Mock<IEndpoint>();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<IEndpoint>(fakeEndpoint.Object);

        var app = builder.Build();

        // Act
        app.MapEndpoints();

        // Assert
        fakeEndpoint.Verify(x => x.MapEndpoint(app), Times.Once);
    }
}
