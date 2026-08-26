// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using System.Text.Json;

using Api.Database;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using TUnit.AspNetCore;
using TUnit.Core.Interfaces;

namespace Api.Tests;

public class MyWebApplicationFactory(string dbName) : TestWebApplicationFactory<Program>
{
    protected override void ConfigureWebApplicationBuilder(IHostApplicationBuilder hostApplicationBuilder)
    {
        base.ConfigureWebApplicationBuilder(hostApplicationBuilder);

        Stream strStream = new MemoryStream(
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {

            ConnectionStrings = new
            {
                PG = new Npgsql.NpgsqlConnectionStringBuilder(hostApplicationBuilder.Configuration.GetConnectionString("PG"))
                {
                    Database = dbName,
                }.ConnectionString
            }
        })));
        hostApplicationBuilder.Configuration.AddJsonStream(strStream);
    }
}

public class TestInit : IAsyncInitializer, IAsyncDisposable
{
    public WebApplicationFactory<Program> Factory { get; set; }
    public HttpClient Client { get; set; }
    public string DbName { get; init; } = $"___ProjectBookTests___{Guid.CreateVersion7()}";

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        {
            await using var scope = Factory.Services.CreateAsyncScope();
            await using var dbConn = scope.ServiceProvider.GetService<PGContext>()!;
            await dbConn.GetService<IRelationalDatabaseCreator>().DeleteAsync();
        }

        await Factory.DisposeAsync();

    }

    public async Task InitializeAsync()
    {
        Factory = new MyWebApplicationFactory(DbName);
        Client = Factory.CreateClient();
        {
            await using var scope = Factory.Services.CreateAsyncScope();
            await using var dbConn = scope.ServiceProvider.GetService<PGContext>()!;
            await dbConn.Database.EnsureCreatedAsync();
        }
    }
}