// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using NodaTime;

namespace Api.Works.Query;


public closed record class Compare<T>;
public record Lt<T>(T Val) : Compare<T>;
public record Gt<T>(T Val) : Compare<T>;
public record Eq<T>(T Val) : Compare<T>;

public record And(WorkSelector[] Parts) : WorkSelector;
public record Or(WorkSelector[] Parts) : WorkSelector;

public closed record WorkSelector;
public record IdSelector(Compare<Guid> Id) : WorkSelector;
public record MetadataAddedAt(Compare<Instant> Time) : WorkSelector;
public record MetadataUpdatedAt(Compare<Instant> Time) : WorkSelector;
public record WorkAddedAt(Compare<Instant> Time) : WorkSelector;
public record WorkUpdatedAt(Compare<Instant> Time) : WorkSelector;
public record TitleLike(string TitleQuery) : WorkSelector;
public record DescriptionLike(string DescriptionQuery) : WorkSelector;
public record WorkIdentifierCheck(string? Type, string? Value, bool HasIt) : WorkSelector;