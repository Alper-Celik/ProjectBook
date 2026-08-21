// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Api.Works.Models;

[Table("works_ids", Schema = "main")]
[PrimaryKey(
        nameof(WorkIdentifierType),
        nameof(WorkIdentifier),
        nameof(WorkId))]
public class WorkIdentifier
{
    [ForeignKey(nameof(Work))]
    public Guid WorkId { get; set; }

    public required string WorkIdentifierType { get; set; }

    public required string WorkIdentifierValue { get; set; }


    // Navigation Properties
    public Work Work { get; set; } = null!;
}