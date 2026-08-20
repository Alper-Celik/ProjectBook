// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    // Navigation Properties
    public List<WorkTag> WorkTags { get; set; } = [];
    public List<WorkIdentifier> WorkIdentifiers { get; set; } = [];
    public List<Author> Authors { get; set; } = [];
}