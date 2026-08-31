// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;
using System.Text;

using Api.Auth.Models;
using Api.Auth.Utils;
using Api.Database;

using FluentValidation;

using Geralt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

using static Api.Auth.Utils.LoginUtils;

namespace Api.Auth.Endpoints;

public static class RegisterEndpoints
{

    // see https://www.rfc-editor.org/rfc/rfc9106.html#name-recommendations
    const int ARGON2ID_ITER = 3;
    const int ARGON2ID_MEM_BYTES = 64 * 1024 * 1024;

    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("register", PostRegister).AddFluentValidationAutoValidation();
        route.MapGet("register_info", GetRegisterInfo);
    }

    [AllowAnonymous]
    private static async Task<Results<
     Ok<LoginUtils.LoginResultDTO>,
     Conflict,
     BadRequest
     >>
     PostRegister(
         HttpContext ctx,
         [FromServices] PGContext db,
         [FromHeader(Name = "user-agent")] string? userAgent,
         [FromBody] RegisterDTO dto
         )
    {
        userAgent ??= "unknown";
        var password_bytes = Encoding.UTF8.GetBytes(dto.Password.Normalize());
        var hash_chars = new char[Argon2id.HashSize];
        Argon2id.ComputeHash(hash_chars, password_bytes, ARGON2ID_ITER, ARGON2ID_MEM_BYTES);
        string hash = new([.. hash_chars.Where(c => c != (char)byte.MinValue)]);

        var user = new UserEF()
        {
            Id = Guid.CreateVersion7(),
            Email = dto.Email,
            PasswordHash = hash,
            Admin = (await CanAdminRegister(db)) && dto.AdminRegistration
        };

        try
        {
            await db.Users.AddAsync(user);
            string token = await LoginUtils.CreateUserSession(user.Id, userAgent, db);
            await db.SaveChangesAsync();
            return LoginUtils.LogUserIn(ctx, token);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx &&
                                            pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return TypedResults.Conflict();
        }
    }

    [AllowAnonymous]
    private static async Task<Ok<RegisterInfo>> GetRegisterInfo([FromServices] PGContext db) => TypedResults.Ok(new RegisterInfo(
         CanRegisterAsAdmin: await CanAdminRegister(db)
        ));


    public record RegisterInfo(bool CanRegisterAsAdmin);

    public record RegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required bool AdminRegistration { get; set; }
    }

    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator(PGContext db)
        {
            RuleFor(r => r.Email)
                .Must(e => new EmailAddressAttribute().IsValid(e))
                .WithMessage("Email is invalid");

            RuleFor(r => r.Email)
                .MustAsync(async (e, ct) =>
                        !await db.Users
                        .Where(u => u.Email == e)
                        .AnyAsync(ct))
                .WithMessage("Email is already used");


            RuleFor(r => r.AdminRegistration).MustAsync(async (adr, ct) => !adr || await CanAdminRegister(db)).WithMessage("Can't Register As Admin");

        }
    }
}