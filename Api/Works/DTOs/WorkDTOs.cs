// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record WorkUpdateDTO(
        int RowVersion,

        string Title,
        string? Description,

        NodaTime.ZonedDateTime? WorkPublishedAt,
        NodaTime.ZonedDateTime? WorkUpdatedAt,

        Guid[] TagIds,
        Guid[] AuthorIds,

        WorkIdentifierDTO[] WorkIdentifiers
        );

public record WorkGetDTO(
        Guid Id,
        int RowVersion,

        string Title,
        string? Description,

        NodaTime.Instant MetadataAddedAt,
        NodaTime.Instant MetadataUpdatedAt,

        NodaTime.ZonedDateTime? WorkPublishedAt,
        NodaTime.ZonedDateTime? WorkUpdatedAt,

        TagGetDTO[] Tags,
        AuthorGetDTO[] Authors,

        WorkIdentifierDTO[] WorkIdentifiers
        );

[Mapper]
public static partial class WorkGetDTOMapper
{
    [MapProperty(nameof(Work.WorkTags), nameof(WorkGetDTO.Tags))]
    public static partial WorkGetDTO ToDto(Work w);
}

public record WorkAddDTO(
        string Title,
        string? Description,

        NodaTime.ZonedDateTime? WorkPublishedAt,
        NodaTime.ZonedDateTime? WorkUpdatedAt,

        Guid[] TagIds,
        Guid[] AuthorIds,

        WorkIdentifierDTO[] WorkIdentifiers
        );

public record struct WorkIdentifierDTO(
        string WorkIdentifierType,
        string WorkIdentifierValue
        );


public record struct WorksGetDTO(
        AuthorGetDTO[] ReferencedAuthors,
        WorkSmallDTO[] Works
        );

public record struct WorkSmallDTO(
        Guid Id,
        string Title,
        Guid[] AuthorIds,
        Guid? CoverId = null
        );

public static partial class WorkSmallDTOMapper
{
    public static IQueryable<WorkSmallDTO> ProjectToDTO(IQueryable<Work> q) => q.Select(w => new WorkSmallDTO(

         w.Id,
         w.Title,
         w.Authors!.Select(a => a.Id).ToArray(),
         null
    ));

}