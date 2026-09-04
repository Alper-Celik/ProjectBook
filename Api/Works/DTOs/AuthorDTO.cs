// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record AuthorGetDTO(
        Guid Id,
        int RowVersion,
        NodaTime.Instant MetadataAddedAt,
        NodaTime.Instant MetadataUpdatedAt,

        string? FirstName,
        string? LastName,
        string DisplayName,
        string[] PenNames
        ) : IEntityMetadata
{
    public static byte IdPostfix => Models.Author.IdPostfix;
}


[Mapper]
public static partial class AuthorGetDTOMapper
{
    public static partial IQueryable<AuthorGetDTO> ProjectToDTO(IQueryable<Author> q);
}