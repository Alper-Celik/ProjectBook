using System.ComponentModel.DataAnnotations;
using System.Text;

using Api.Auth.Models;
using Api.Database;

using FluentValidation;

using Geralt;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Api.Auth.Endpoints;

public static class Register
{

    // see https://www.rfc-editor.org/rfc/rfc9106.html#name-recommendations
    const int ARGON2ID_ITER = 3;
    const int ARGON2ID_MEM_BYTES = 64 * 1024 * 1024;


    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("register", Handle);
    }

    private static async Task<Results<Ok<Guid>, Conflict>> Handle(
            [FromServices] PGContext db,
            [FromBody] RegisterDTO dto
            )
    {
        var password_bytes = Encoding.UTF8.GetBytes(dto.Password.Normalize());
        var hash_chars = new char[Argon2id.HashSize];
        Argon2id.ComputeHash(hash_chars, password_bytes, ARGON2ID_ITER, ARGON2ID_MEM_BYTES);
        string hash = hash_chars.ToString()!;

        var user = new User()
        {
            Id = Guid.CreateVersion7(),
            Email = dto.Email,
            PasswordHash = hash,
        };

        try
        {

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx &&
                                            pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return TypedResults.Conflict();
        }

        return TypedResults.Ok(user.Id);

    }


    private record RegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    private class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(r => r.Email).Must(e => new EmailAddressAttribute().IsValid(e)).WithMessage("Email is invalid");
        }
    }
}