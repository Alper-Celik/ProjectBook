// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Auth.Handlers;
using Api.Auth.Models;
using Api.Auth.Utils;
using Api.Database;
using Api.Database.Utils;
using Api.Works.DTOs;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Works.Endpoints;

public static class WorkEndpoints
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("", AddWork);
        route.MapPut("{workId}", UpdateWork);
        route.MapGet("{workId}", GetWork);
        route.MapGet("", GetWorks);
    }

    [PermissionCheckAuthorize(
            UserPermissionBits.WorkWrite |
            UserPermissionBits.AuthorRead |
            UserPermissionBits.WorkTagRead)]
    public static async Task<Results<
        Created<WorkGetDTO>,
        BadRequest>>
        AddWork(
                [FromServices] PGContext db,
                [FromServices] ICurrentUserId userId,

                [FromBody] WorkAddDTO workDto
                )
    {

        if (userId.Id is null)
            return TypedResults.BadRequest();

        var work = WorkAddDTOMapper.FromWorkAddDTO(workDto);

        await db.Works.AddAsync(work);

        await LoadWorkGetDeps(db, work);

        return TypedResults.Created($"/api/works/{work.Id}", WorkGetDTOMapper.ToDto(work));
    }


    [PermissionCheckAuthorize(
            UserPermissionBits.WorkWrite |
            UserPermissionBits.AuthorRead |
            UserPermissionBits.WorkTagRead)]
    public static async Task<Results<
        Ok<WorkGetDTO>,
        Conflict,
        NotFound,
        BadRequest>>
            UpdateWork(
                    [FromServices] PGContext db,
                    [FromServices] IEFTransactionDIAccessorService tx,
                    [FromServices] ICurrentUserId userId,

                    [FromRoute] Guid workId,
                    [FromBody] WorkUpdateDTO newWork
                    )
    {
        await tx.BeginOrGetTransactionAsync();
        if (userId.Id is null)
            return TypedResults.BadRequest();

        if (workId != newWork.Id)
            return TypedResults.BadRequest();

        var workPre = db.Works.AsNoTracking().FirstOrDefault(w => w.OwnerId == userId.Id && w.Id == workId);

        if (workPre is null)
            return TypedResults.NotFound();

        if (workPre.RowVersion != newWork.RowVersion)
            return TypedResults.Conflict();

        var updatedWork = WorkUpdateDTOMapper.FromWorkUpdateDTO(newWork);
        updatedWork.RowVersion++;
        updatedWork.MetadataAddedAt = workPre.MetadataAddedAt;
        updatedWork.MetadataUpdatedAt = NodaTime.SystemClock.Instance.GetCurrentInstant();

        db.Works.Update(updatedWork);
        await db.SaveChangesAsync();

        await LoadWorkGetDeps(db, updatedWork);

        return TypedResults.Ok(WorkGetDTOMapper.ToDto(updatedWork));
    }


    [PermissionCheckAuthorize(
            UserPermissionBits.WorkRead |
            UserPermissionBits.AuthorRead |
            UserPermissionBits.WorkTagRead)]
    public static async Task<Results<
        Ok<WorkGetDTO>,
        NotFound,
        BadRequest>>
            GetWork(
                    [FromServices] PGContext db,
                    [FromServices] ICurrentUserId userId,

                    [FromRoute] Guid workId
                    )
    {

        if (userId.Id is null)
            return TypedResults.BadRequest();

        var work = await db.Works
            .Include(w => w.Authors)
            .Include(w => w.WorkTags)
            .FirstOrDefaultAsync(w => w.OwnerId == userId.Id && w.Id == workId);

        return work switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(WorkGetDTOMapper.ToDto(work)),
        };
    }


    [PermissionCheckAuthorize(UserPermissionBits.WorkRead | UserPermissionBits.AuthorRead)]
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

    private static async Task LoadWorkGetDeps(PGContext db, Models.Work work)
    {
        await db.Entry(work).Collection(w => w.Authors).LoadAsync();
        await db.Entry(work).Collection(w => w.WorkTags).LoadAsync();
    }
}