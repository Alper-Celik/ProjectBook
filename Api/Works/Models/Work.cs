// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Riok.Mapperly.Abstractions;

namespace Api.Works.Models;

[Table("works", Schema = "main")]
public class Work
{
    [Key]
    public Guid Id { get; set; }
    public int RowVersion { get; set; }


    [MapperIgnore]
    public Guid OwnerId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public NodaTime.Instant MetadataAddedAt { get; set; }
    public NodaTime.Instant MetadataUpdatedAt { get; set; }

    public NodaTime.ZonedDateTime? WorkPublishedAt { get; set; }
    public NodaTime.ZonedDateTime? WorkUpdatedAt { get; set; }
    public List<WorkIdentifier> WorkIdentifiers { get; set; } = [];

    // Navigation Properties
    public List<WorkTag> WorkTags { get; set; } = [];
    public List<Author> Authors { get; set; } = [];

    [MapperIgnore]
    public List<WorkTag_Work> WorkTag_Works { get; set; } = [];
    [MapperIgnore]
    public List<Work_Author> Work_Authors { get; set; } = [];
}

public record WorkIdentifier(
        string WorkIdentifierType,
        string WorkIdentifierValue);

public class WorkTypeConfiguration : IEntityTypeConfiguration<Work>
{
    public void Configure(EntityTypeBuilder<Work> builder)
    {
        builder
            .ComplexCollection(w => w.WorkIdentifiers, wid => wid.ToJson())
            .HasIndex(w => w.WorkIdentifiers.Select(wid => wid));
    }
}