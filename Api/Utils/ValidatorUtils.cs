// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using FluentValidation;

using Microsoft.EntityFrameworkCore;


namespace Api.Utils;

public static class ValidatorUtils
{
    public static IRuleBuilderOptions<T, IEnumerable<TInner>> MustBeDistinct<T, TInner>(
        this IRuleBuilder<T, IEnumerable<TInner>> rule,
        string propName,
        IEqualityComparer<TInner>? comparer = null)
    {
        return rule.Must(t => t.Distinct().SequenceEqual(t, comparer))
            .WithMessage($"{propName} must be distinct")
            .WithErrorCode(ErrorCodes.IS_NOT_DISTINCT);
    }

    public static IRuleBuilderOptions<T, TProp> IdsMustExist<T, TProp, TTarget>(this IRuleBuilder<T, TProp> rule, DbSet<TTarget> target)
         where TProp : IEnumerable<Guid>
         where TTarget : class, IEntityMetadata
    {
        return rule.MustAsync(async (t, ids, ct) =>
                (await target.Select(t => t.Id)
                 .Where(id => ids.Contains(id))
                 .Distinct()
                 .Order()
                 .ToArrayAsync())
                    .SequenceEqual(ids.Distinct()))
            .WithMessage($"{nameof(TTarget)} ids must exist")
            .WithErrorCode(ErrorCodes.IDS_DOES_NOT_EXIST);
    }


}