// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Api.Database;

using FairyBread;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using static Api.Utils.ValidatorUtils;

namespace Api.Works.MutationTypes;

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


