
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using NodaTime;

namespace Api.Auth.Models;

[Table("user_tokens", Schema = "auth")]
public class UserToken
{
    public const string PermissionBitsType = "PermissionBitsType";

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [Key]
    public required byte[] TokenHash { get; set; }

    public required UserPermissionBits Permissions { get; set; }

    public Instant CreationTime { get; set; }

    public Instant? LastUsed { get; set; }
}