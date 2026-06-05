using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Auth.Models;

[Table("users", Schema = "auth")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(UserHandle), IsUnique = true)]
public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string UserHandle { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public bool Admin { get; set; } = false;
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
    }
}