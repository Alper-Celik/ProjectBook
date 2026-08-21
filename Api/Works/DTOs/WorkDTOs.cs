// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Auth.Utils;
using Api.Database;
using Api.Works.Models;

using FluentValidation;

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

#region WorkGetDTO
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
#endregion

#region WorkAddDTO
public record WorkAddDTO(
        string Title,
        string? Description,

        NodaTime.ZonedDateTime? WorkPublishedAt,
        NodaTime.ZonedDateTime? WorkUpdatedAt,

        Guid[] TagIds,
        Guid[] AuthorIds,

        WorkIdentifierDTO[] WorkIdentifiers
        );

[Mapper]
public static partial class WorkAddDTOMapper
{

    public static Work FromWorkAddDTO(WorkAddDTO w)
    {
        var id = Guid.CreateVersion7();
        return new Work()
        {
            Id = id,
            Title = w.Title,
            Description = w.Description,

            WorkPublishedAt = w.WorkPublishedAt,
            WorkUpdatedAt = w.WorkUpdatedAt,

            WorkTag_Works = [.. w.TagIds.Select(wTagId => new WorkTag_Work(){
                    WorkId = id,
                    WorkTagId =wTagId
                    })],

            Work_Authors = [.. w.AuthorIds.Select(wAuthorId => new Work_Author(){
                    WorkId = id,
                    AuthorId = wAuthorId
                    })],

            WorkIdentifiers = [.. w.WorkIdentifiers.Select(wid => new WorkIdentifier()
            {
                Id = Guid.CreateVersion7(),
                WorkId = id,
                WorkIdentifierType = wid.WorkIdentifierType,
                WorkIdentifierValue = wid.WorkIdentifierValue
            })],

        };
    }
}

public class WorkAddDTOValidator : AbstractValidator<WorkAddDTO>
{
    public WorkAddDTOValidator(PGContext db, ICurrentUserId userId)
    {
        RuleFor(w => w.TagIds)
            .Must(ids => ids.Length == ids.Distinct().Count())
            .WithMessage("Duplicate tagIds are forbidden");

        RuleFor(w => w.TagIds)
            .MustAsync(async (ids, ct) =>
                    await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(
                     db.WorkTags
                    .Where(wt => wt.OwnerId == userId.Id
                        && ids.Contains(wt.Id)), ct)
                    == ids.Length
                    )
            .WithMessage("Some or all tagIds doesn't exist");



        RuleFor(w => w.AuthorIds)
           .Must(ids => ids.Length == ids.Distinct().Count())
           .WithMessage("Duplicate AuthorIds are forbidden");

        RuleFor(w => w.AuthorIds)
            .MustAsync(async (ids, ct) =>
                    await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(
                     db.WorkTags
                    .Where(wt => wt.OwnerId == userId.Id
                        && ids.Contains(wt.Id)), ct)
                    == ids.Length
                    )
            .WithMessage("Some or all AuthorIds doesn't exist");
    }
}
#endregion
public record struct WorkIdentifierDTO(
        string WorkIdentifierType,
        string WorkIdentifierValue
        );


public record struct WorksGetDTO(
        AuthorGetDTO[] ReferencedAuthors,
        WorkSmallDTO[] Works
        );

#region WorkSmallDto
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
#endregion