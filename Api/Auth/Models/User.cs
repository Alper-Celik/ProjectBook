using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Auth.Models;

[Table("users", Schema = "auth")]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public const string UserHandleAcceptedRegex = @"^[a-zA-Z0-9_\-]{3,30}$";

    public Guid Id { get; set; } = Guid.CreateVersion7();

    [EmailAddress]
    public required string Email { get; set; }

    public bool EmailVerified { get; set; } = false;

    public required string PasswordHash { get; set; }

    public bool Admin { get; set; } = false;
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
    }
}