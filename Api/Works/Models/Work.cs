// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Works.Models;

[Table("works", Schema = "main")]
public class Work
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    // Navigation Properties
    public List<WorkTag>? WorkTags { get; set; }
    public List<WorkIdentifier>? WorkIdentifiers { get; set; }
}