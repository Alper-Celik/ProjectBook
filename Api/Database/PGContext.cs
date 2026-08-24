// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Api.Database;

public partial class PGContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(this.GetType()!)!);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
        .UseSnakeCaseNamingConvention()
        .UseValidationCheckConstraints();

    public static void ConfigureDB(IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        services.AddNpgsql<PGContext>(connectionString, (opt) =>
        {
            opt.SetPostgresVersion(18, 0);
            opt.UseNodaTime();
        });
    }
}