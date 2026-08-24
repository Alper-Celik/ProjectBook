// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;

using Api.Auth.Handlers;
using Api.Auth.Models;
using Api.Auth.Utils;
using Api.Database;
using Api.Utils;
using Api.Works.DTOs;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Works.Endpoints;

public static class TagEndpoints
{

    public static void Map(IEndpointRouteBuilder route)
    {
        var tags = route.MapGroup("tags");

        tags.MapGet("", GetTags);
    }

    [PermissionCheckAuthorize(UserPermissionBits.WorkTagRead | UserPermissionBits.WorkRead)]
    public static async Task<Results<
        Ok<PaginationResult<TagGetDTO>>,
        NotFound,
        BadRequest>>
            GetTags(
                    [FromServices] PGContext db,
                    [FromServices] ICurrentUserId userId,

                    [FromQuery] string[]? tagTypes,
                    [FromQuery] TagSortOption[]? sortOptions,
                    [FromQuery] Guid[]? workIds,
                    [FromQuery] Guid[]? tagIds,
                    [FromQuery][Range(0, 1000)] int pageSize = 30,
                    [FromQuery][Range(1, int.MaxValue)] int page = 1
                    )
    {
        if (userId.Id is null)
            return TypedResults.BadRequest();

        tagIds ??= [];
        tagTypes ??= [];
        sortOptions ??= [];
        workIds ??= [];

        var results = db.WorkTags
            .Where(wt => wt.OwnerId == userId.Id);

        if (tagTypes.Any())
            results = results.Where(wt => tagIds.Contains(wt.Id));

        if (tagTypes.Any())
            results = results.Where(wt => tagTypes.Contains(wt.TagType));

        if (workIds.Any())
            results = results
                .Include(wt => wt.WorkTagWorks)
                .Where(wt =>
                        wt.WorkTagWorks.Any(
                            wtw => workIds.Contains(wtw.WorkId)));

        foreach (var sort in sortOptions.Distinct())
        {
            results = sort switch
            {
                TagSortOption.Id => results.OrderBy(wt => wt.Id),
                TagSortOption.TagType => results.OrderBy(wt => wt.TagType),
                TagSortOption.TagName => results.OrderBy(wt => wt.TagName),
                _ => results
            };
        }
        var totalCount = await results.CountAsync();

        if (page * pageSize >= totalCount)
            return TypedResults.NotFound();

        return TypedResults.Ok(new PaginationResult<TagGetDTO>(
                    await results.Skip(pageSize * (page - 1)).Take(pageSize).ProjectToDTO().ToArrayAsync(),
                    totalCount,
                    pageSize,
                    page,
(int)Math.Ceiling(totalCount / (double)pageSize)
                    ));

    }

}