// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Security.Claims;

using Api.Auth.Handlers;
using Api.Auth.Utils;
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
    public static async Task<Results<
        Ok<WorkGetDTO>,
        NotFound,
        BadRequest>>
            GetWork(
                    [FromServices] PGContext db,
                    [FromServices] CurrentUserId userId,

                    [FromRoute] Guid workId
                    )
    {

        if (userId.Id is null)
            return TypedResults.BadRequest();

        var work = await db.Works
            .Include(w => w.Authors)
            .Include(w => w.WorkTags)
            .Include(w => w.WorkIdentifiers)
            .FirstOrDefaultAsync(w => w.OwnerId == userId.Id && w.Id == workId);

        return work switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(WorkGetDTOMapper.ToDto(work)),
        };
    }

    [PermissionCheckAuthorize(Auth.Models.UserPermissionBits.WorkRead)]
    public static async Task<
    Results<
         Ok<WorksGetDTO>,
         NotFound,
         BadRequest>>
    GetWorks(
            [FromServices] PGContext db,
            [FromServices] CurrentUserId userId
            )
    {
        if (userId.Id is null)
            return TypedResults.BadRequest();

        var works = WorkSmallDTOMapper.ProjectToDTO(db.Works.Where(w => w.OwnerId == userId.Id)).ToArray();

        var referencedAuthorsIds = works
            .SelectMany(w => w.AuthorIds)
            .Distinct().ToArray();
        var referencedAuthors = AuthorGetDTOMapper.ProjectToDTO(db.Authors
                .Where(a => referencedAuthorsIds
                .Contains(a.Id))).ToArray();


        return TypedResults.Ok(new WorksGetDTO
        {
            Works = works,
            ReferencedAuthors = referencedAuthors
        });
    }

}