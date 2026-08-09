using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Api.Database;

public partial class PGContext : DbContext
{
    protected PGContext()
    {
    }

    public PGContext(DbContextOptions options) : base(options)
    {
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
            opt.ConfigureDataSource(o => o.UseNodaTime());
        });
    }
}