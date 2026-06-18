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

public static class RegisterEndpoints
{

    // see https://www.rfc-editor.org/rfc/rfc9106.html#name-recommendations
    const int ARGON2ID_ITER = 3;
    const int ARGON2ID_MEM_BYTES = 64 * 1024 * 1024;

    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("register", PostRegister);
        route.MapGet("register_info", GetRegisterInfo);
    }

    private static async Task<bool> CanAdminRegister()
    {
        return true;
    }

    private static async Task<Results<Ok<Guid>, Conflict>> PostRegister(
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
            UserHandle = dto.UserHandle,
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

    private static async Task<Ok<RegisterInfo>> GetRegisterInfo() => TypedResults.Ok(new RegisterInfo(
         CanRegisterAsAdmin: await CanAdminRegister(),
         UserHandleAcceptedRegex: User.UserHandleAcceptedRegex
        ));


    public record RegisterInfo(bool CanRegisterAsAdmin, string UserHandleAcceptedRegex);

    public record RegisterDTO
    {
        public required string UserHandle { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public required bool AdminRegistration { get; set; }
    }

    private class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator(PGContext ctx)
        {
            RuleFor(r => r.Email).Must(e => new EmailAddressAttribute().IsValid(e)).WithMessage("Email is invalid");
            RuleFor(r => r.Email).MustAsync(async (e, ct) => !await ctx.Users.Where(u => u.Email == e).AnyAsync(ct)).WithMessage("Email is already used");

            RuleFor(r => r.UserHandle).Matches(User.UserHandleAcceptedRegex).WithMessage("UserHandle format is invalid");
            RuleFor(r => r.UserHandle).MustAsync(async (uh, ct) => !await ctx.Users.Where(u => u.UserHandle == uh).AnyAsync(ct)).WithMessage("User with same handle already exists");

            RuleFor(r => r.AdminRegistration).MustAsync(async (adr, ct) => !adr || await CanAdminRegister()).WithMessage("Can't Register As Admin");

        }
    }
}