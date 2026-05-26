using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Auth.Models;

[Table("users", Schema = "auth")]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Email { get; set; }

    public string? PasswordHash { get; set; }
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
    }
}