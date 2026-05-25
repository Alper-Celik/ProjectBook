namespace Api.Auth;

public static class Setup
{
    public static void MapEndpoints(IEndpointRouteBuilder route)
    {
        Endpoints.Register.Map(route);
    }
}