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

        tags.MapMethods("", [HttpMethod.Query.Method, HttpMethod.Get.Method], QueryTags);
    }

    [PermissionCheckAuthorize(UserPermissionBits.WorkTagRead | UserPermissionBits.WorkRead)]
    public static async Task<Results<
        Ok<PaginationResult<TagGetDTO>>,
        NotFound,
        BadRequest>>
            QueryTags(
                    [FromServices] PGContext db,
                    [FromServices] ICurrentUserId userId,

                    [FromBody] TagQueryDTO queryDTO
                    )
    {
        if (userId.Id is null)
            return TypedResults.BadRequest();

        var results = db.WorkTags
            .Where(wt => wt.OwnerId == userId.Id);

        if (queryDTO.TagIds.Any())
            results = results.Where(wt => queryDTO.TagIds.Contains(wt.Id));

        if (queryDTO.TagTypes.Any())
            results = results.Where(wt => queryDTO.TagTypes.Contains(wt.TagType));

        if (queryDTO.WorkIds.Any())
            results = results
                .Include(wt => wt.WorkTagWorks)
                .Where(wt =>
                        wt.WorkTagWorks.Any(
                            wtw => queryDTO.WorkIds.Contains(wtw.WorkId)));

        foreach (var sort in queryDTO.SortOptions.Distinct())
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

        if (queryDTO.Page * queryDTO.PageSize >= totalCount)
            return TypedResults.NotFound();

        return TypedResults.Ok(new PaginationResult<TagGetDTO>(
                    await results.Skip(queryDTO.PageSize * (queryDTO.Page - 1)).Take(queryDTO.PageSize).ProjectToDTO().ToArrayAsync(),
                    totalCount,
                    queryDTO.PageSize,
                    queryDTO.Page,
(int)Math.Ceiling(totalCount / (double)queryDTO.PageSize)
                    ));

    }

}