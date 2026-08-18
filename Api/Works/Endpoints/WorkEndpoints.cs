using System.Security.Claims;

using Api.Auth.Handlers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Works.Endpoints;

public static class WorkEndpoints
{
    public static void Map(IEndpointRouteBuilder route)
    {

    }

    [PermissionCheckAuthorize(Auth.Models.UserPermissionBits.WorkRead)]
    public static async Task<
        Results<
             Ok<DTOs.WorksGetDto>,
             NotFound,
             BadRequest
        >> GetWorks(
                [FromServices] ClaimsPrincipal claims
                )
    {
        Guid userId;

        if (claims.FindFirstValue(ClaimTypes.NameIdentifier) is { } userIdStr)
        {
            userId = Guid.Parse(userIdStr);
        }
        else
        {
            return TypedResults.BadRequest();
        }





        return TypedResults.NotFound();
    }

}