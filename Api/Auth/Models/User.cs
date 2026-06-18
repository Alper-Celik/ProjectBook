using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Auth.Models;

[Table("users", Schema = "auth")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(UserHandle), IsUnique = true)]
public class User
{
    public const string UserHandleAcceptedRegex = @"^[a-zA-Z0-9_\-]{3,30}$";

    public Guid Id { get; set; } = Guid.CreateVersion7();

    [RegularExpression(UserHandleAcceptedRegex)]
    public required string UserHandle { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public bool EmailVerified { get; set; } = false;

    public string? PasswordHash { get; set; }

    public bool Admin { get; set; } = false;
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
    }
}