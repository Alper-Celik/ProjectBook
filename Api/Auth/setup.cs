namespace Api.Auth;

public static class Setup
{
    public static void MapEndpoints(IEndpointRouteBuilder route)
    {
        Endpoints.RegisterEndpoints.Map(route);
    }
}