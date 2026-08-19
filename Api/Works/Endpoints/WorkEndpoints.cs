using System.Security.Claims;

using Api.Auth.Handlers;
using Api.Database;
using Api.Works.DTOs;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Works.Endpoints;

public static class WorkEndpoints
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapGet("", GetWorks);
    }

    [PermissionCheckAuthorize(Auth.Models.UserPermissionBits.WorkRead)]
    public static async Task<
        Results<
             Ok<WorksGetDTO>,
             NotFound,
             BadRequest
        >> GetWorks(
                [FromServices] PGContext db,
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

        var works = WorkSmallDTOMapper.ProjectToDTO(db.Works).ToArray();

        var referencedAuthorsIds = works
            .SelectMany(w => w.AuthorIds)
            .Distinct().ToArray();
        var referencedAuthors = AuthorDTOMapper.ProjectToDTO(db.Authors
                .Where(a => referencedAuthorsIds
                .Contains(a.Id))).ToArray();


        return TypedResults.Ok(new WorksGetDTO
        {
            Works = works,
            ReferencedAuthors = referencedAuthors
        });
    }

}