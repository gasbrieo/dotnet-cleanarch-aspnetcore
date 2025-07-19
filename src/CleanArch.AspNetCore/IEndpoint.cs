namespace CleanArch.AspNetCore;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
