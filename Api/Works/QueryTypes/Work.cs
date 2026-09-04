// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Riok.Mapperly.Abstractions;

namespace Api.Works.QueryTypes;

public class Work : IEntityMetadata
{
    public static byte IdPostfix => Models.Work.IdPostfix;

    public Guid Id { get; set; }
    public int RowVersion { get; set; }
    public NodaTime.Instant MetadataAddedAt { get; set; }
    public NodaTime.Instant MetadataUpdatedAt { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public NodaTime.Instant? WorkPublishedAt { get; set; }
    public NodaTime.Instant? WorkUpdatedAt { get; set; }
    public List<WorkIdentifier> WorkIdentifiers { get; set; } = [];

    public record WorkIdentifier(
            string WorkIdentifierType,
            string WorkIdentifierValue);
}

[Mapper]
public static partial class WorkMapper
{
    public static partial IQueryable<Work> ToDto(this IQueryable<Models.Work> q);
}