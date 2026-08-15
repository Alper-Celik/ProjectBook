// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Auth.Handlers;
using Api.Database;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Scalar.AspNetCore;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<PGContext>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, AuthHandler>("x_user", null);
builder.Services.AddSingleton<IAuthorizationHandler, PermissionCheckAuthorizationHandler>();
builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());


PGContext.ConfigureDB(builder.Services, builder.Configuration.GetConnectionString("PG")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().RequireAuthorization();
    app.MapScalarApiReference();
}


app.UseStaticFiles();
app.MapFallbackToFile("index.html");

var api = app.MapGroup("/api").AddFluentValidationAutoValidation();

var auth = api.MapGroup("auth");
Api.Auth.Setup.MapEndpoints(auth);

app.Run();