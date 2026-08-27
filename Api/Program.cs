// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;

using Api.Auth.Endpoints;
using Api.Auth.Handlers;
using Api.Database;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using Scalar.AspNetCore;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNamingPolicy =
        JsonNamingPolicy.CamelCase;
});

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterEndpoints.RegisterDTO>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, AuthHandler>("x_user", null);
builder.Services.AddSingleton<IAuthorizationHandler, PermissionCheckAuthorizationHandler>();
builder.Services.AddAuthorizationBuilder().SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());


Api.Database.Setup.RegisterServices(builder.Services);
Api.Auth.Setup.RegisterServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    if (!app.Configuration.GetSection("IsTest").Get<bool>())
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetService<PGContext>()!;
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

if (app.Configuration.GetSection("IsTest").Get<bool>())
{
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetService<PGContext>()!;
        RelationalDatabaseCreator databaseCreator =
            (RelationalDatabaseCreator)db.Database.GetService<IDatabaseCreator>();
        await databaseCreator.CreateTablesAsync();
    }
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

var api = app.MapGroup("/api").AddFluentValidationAutoValidation();

var auth = api.MapGroup("auth");
Api.Auth.Setup.MapEndpoints(auth);

var works = api.MapGroup("works");
Api.Works.Setup.MapEndpoints(works);

app.Run();