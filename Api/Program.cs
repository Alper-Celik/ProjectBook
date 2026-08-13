using Api.Auth.Middlewares;
using Api.Database;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;

using Scalar.AspNetCore;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<PGContext>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, AuthHandler>("x_user", null);

PGContext.ConfigureDB(builder.Services, builder.Configuration.GetConnectionString("PG")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

var api = app.MapGroup("/api").AddFluentValidationAutoValidation();

var auth = api.MapGroup("auth");
Api.Auth.Setup.MapEndpoints(auth);

app.Run();