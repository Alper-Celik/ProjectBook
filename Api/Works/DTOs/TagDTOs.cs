// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record TagGetDTO(
       Guid Id,
       int RowVersion,
       string TagType,
       string TagName
        );

[Mapper]
public static partial class TagGetDTOMapper
{
    public static partial IQueryable<TagGetDTO> ProjectToDTO(IQueryable<WorkTag> q);
}