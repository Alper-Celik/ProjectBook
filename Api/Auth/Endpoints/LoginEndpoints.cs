using System.Text;

using Api.Auth.Utils;
using Api.Database;

using FluentValidation;

using Geralt;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Auth.Endpoints;

public static class LoginEndpoints
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("login", Login);
    }

    public static async Task<Results<
        Ok<string>,
        ForbidHttpResult
    >> Login(
            [FromServices] PGContext db,
            [FromHeader(Name = "user-agent")] string userAgent,
            [FromBody] LoginDTO loginDTO
            )
    {
        Guid? userId = null;
        string hash = string.Empty;

        if (loginDTO.UserEmail is not null)
        {
            var user = await db.Users
                .Where(u => u.Email == loginDTO.UserEmail)
                .Select(u => new { u.Id, u.PasswordHash })
                .FirstAsync();
            userId = user.Id;
            hash = user.PasswordHash;
        }

        if (loginDTO.UserHandle is not null)
        {
            var user = await db.Users
                .Where(u => u.UserHandle == loginDTO.UserHandle)
                .Select(u => new { u.Id, u.PasswordHash })
                .FirstAsync();
            userId = user.Id;
            hash = user.PasswordHash;
        }

        var password = Encoding.UTF8.GetBytes(loginDTO.Password.Normalize());
        if (userId is not null &&
                Argon2id.VerifyHash(hash, password))
        {
            var token = await LoginUtils.CreateUserSession(userId.Value, userAgent, db);
            await db.SaveChangesAsync();

            return TypedResults.Ok(token);
        }
        return TypedResults.Forbid();
    }


    public record LoginDTO(string? UserHandle, string? UserEmail, string Password);

    private class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator(PGContext db)
        {

            RuleFor(l => l.UserHandle)
                .Must((l, _) =>
                        l.UserHandle is null ^ l.UserEmail is null)
                .WithMessage("Either email or handle must be set");

            RuleFor(l => l.UserEmail)
                .MustAsync(async (email, ct) =>
                        !await db.Users.Where(u => u.Email == email)
                                .AnyAsync(ct))
                .When(l => l.UserEmail is not null)
                .WithMessage("Invalid Email");

            RuleFor(l => l.UserHandle)
                .MustAsync(async (handle, ct) =>
                        !await db.Users.Where(u => u.UserHandle == handle)
                                .AnyAsync(ct))
                .When(l => l.UserHandle is not null)
                .WithMessage("Invalid Email");
        }
    }

}
