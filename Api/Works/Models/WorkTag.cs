// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;

using Api.Auth.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Works.Models;

[Index(nameof(OwnerId), nameof(TagName), IsUnique = true)]
public class WorkTag
{
    [Key]
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public required string TagType { get; set; }

    public required string TagName { get; set; }

    // Navigation Properties
    public List<WorkTagWork>? WorkTagWorks { get; set; }
    public List<Work>? Works { get; set; }
    public User? Owner { get; set; }

}


[PrimaryKey(nameof(WorkId), nameof(WorkTagId))]
public class WorkTagWork
{
    public Guid WorkId { get; set; }

    public Guid WorkTagId { get; set; }

    // Navigation Properties
    public Work? Work { get; set; }
    public WorkTag? WorkTag { get; set; }
}

public class WorkTagTypeConfiguration : IEntityTypeConfiguration<WorkTag>
{
    void IEntityTypeConfiguration<WorkTag>.Configure(EntityTypeBuilder<WorkTag> builder)
    {
        builder
            .HasMany(wt => wt.Works)
            .WithMany(w => w.WorkTags)
            .UsingEntity(nameof(WorkTagWork));
    }
}