// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Database;
using Api.Works.QueryTypes;

using GreenDonut.Data;

using HotChocolate.Types.Pagination;

namespace Api.Works;


/// <remarks>
/// for the <c>Api.Works</c> module not only <c>Api.QueryTypes.Work</c> type
/// </remarks>
[QueryType]
public static partial class WorkQueries
{

    [UseFiltering]
    [UseSorting]
    public static async Task<PageConnection<Work>> GetWorks(
            [Service] PGContext db,
            CancellationToken ct,
            QueryContext<Work> qc,
            PagingArguments pagingArguments
            )
    {
        return await db.Works.ProjectToDto().With(qc).ToPageAsync(pagingArguments, cancellationToken: ct);
    }

}