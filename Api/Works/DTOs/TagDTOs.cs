// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;

using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record class TagQueryDTO(
        string[] TagTypes,
        TagSortOption[] SortOptions,
        Guid[] WorkIds,
        Guid[] TagIds,
        [Range(0, 1000)] int PageSize = 30,
        [Range(1, int.MaxValue)] int Page = 1
        );

public enum TagSortOption
{
    Id,
    TagType,
    TagName
}

public record TagGetDTO(
        Guid Id,
        int RowVersion,
        NodaTime.Instant MetadataAddedAt,
        NodaTime.Instant MetadataUpdatedAt,

        string TagType,
        string TagName
        ) : IEntityMetadata;

[Mapper]
public static partial class TagGetDTOMapper
{
    public static partial IQueryable<TagGetDTO> ProjectToDTO(this IQueryable<WorkTag> q);
}