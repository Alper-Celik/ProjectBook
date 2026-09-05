// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Api.Database;
using Api.Utils;
using Api.Works.Models;

using FairyBread;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using NodaTime;

using Riok.Mapperly.Abstractions;

using static Api.Utils.ValidatorUtils;

namespace Api.Works.MutationTypes;

public record AddWorkPayload(
        QueryTypes.Work Work
        );

public record AddWorkInput
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public NodaTime.Instant? WorkPublishedAt { get; init; }
    public NodaTime.Instant? WorkUpdatedAt { get; init; }
    public List<QueryTypes.Work.WorkIdentifier> WorkIdentifiers { get; init; } = [];
    public required List<Guid> TagIds { get; init; } = [];
    public required List<Guid> AuthorIds { get; init; } = [];

    public class AddWorkInputValidator : AbstractValidator<AddWorkInput>, IRequiresOwnScopeValidator
    {
        public AddWorkInputValidator(PGContext db)
        {
            RuleFor(w => w.TagIds).MustBeDistinct(nameof(TagIds));
            RuleFor(w => w.TagIds).IdsMustExist(db.WorkTags);


            RuleFor(w => w.AuthorIds).MustBeDistinct(nameof(AuthorIds));
            RuleFor(w => w.AuthorIds).IdsMustExist(db.Authors);
        }
    }
}
[Mapper]
public static partial class AddWorkInputMapper
{

    [MapperIgnoreTarget(nameof(Work.RowVersion))]
    [MapperIgnoreSource(nameof(AddWorkInput.TagIds))]
    [MapperIgnoreSource(nameof(AddWorkInput.AuthorIds))]
    private static partial Work CreateFromDtoInternal(AddWorkInput w, Guid id, Instant metadataAddedAt, Instant metadataUpdatedAt);


    public static Work CreateFromDto(AddWorkInput w, Guid ownerId, Instant now)
    {
        var id = Guid.CreateVersion7().WithPostfix(Work.IdPostfix);
        var work = CreateFromDtoInternal(w, id, now, now);
        work.OwnerId = ownerId;

        work.WorkTag_Works = [.. w.TagIds.Select(tId => new WorkTag_Work(id, tId))];
        work.Work_Authors = [.. w.AuthorIds.Select(aId => new Work_Author(id, aId))];


        return work;
    }



    [UserMapping(Default = true)]
    public static ZonedDateTime FromInstantToZonedDateTime(Instant i) => MapperUtils.FromInstantToZonedDateTime(i);

}