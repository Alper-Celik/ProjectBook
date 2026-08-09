using Api.Auth.Models;

using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public partial class PGContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<UserToken> UserTokens { get; set; }

}