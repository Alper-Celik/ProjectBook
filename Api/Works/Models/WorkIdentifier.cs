// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Api.Works.Models;

[Table("works_ids", Schema = "main")]
[Index(
        nameof(WorkId),
        nameof(WorkIdentifierType),
        nameof(WorkIdentifier),
        IsUnique = true)]
public class WorkIdentifier
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Work))]
    public Guid WorkId { get; set; }

    public required string WorkIdentifierType { get; set; }

    public required string WorkIdentifierValue { get; set; }


    // Navigation Properties
    public Work? Work { get; set; }
}