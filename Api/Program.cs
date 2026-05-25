using Api.Database;

using FluentValidation;

using Npgsql;

using Scalar.AspNetCore;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<PGContext>();
builder.Services.AddFluentValidationAutoValidation();

PGContext.ConfigureDB(builder.Services, builder.Configuration.GetConnectionString("PG")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api").AddFluentValidationAutoValidation();

var auth = api.MapGroup("auth");

Api.Auth.Setup.MapEndpoints(auth);

app.Run();