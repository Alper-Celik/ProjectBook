// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Auth.Utils;
using Api.Database;
using Api.Works.MutationTypes;
using Api.Works.QueryTypes;

namespace Api.Works;

[MutationType]
public static partial class WorkMutations
{
    public static async Task<AddWorkPayload> AddWorkMutation(
            [Service] PGContext db,
            [Service] ICurrentUserId userId,
            CancellationToken ct,
            AddWorkInput input
            )
    {
        var work = AddWorkInputMapper.CreateFromDto(input, userId.Id!.Value, Now());

        await db.AddAsync(work, cancellationToken: ct);
        await db.SaveChangesAsync(cancellationToken: ct);

        return new AddWorkPayload(WorkMapper.ToDto(work));

    }
}