using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using FluentValidation;

using FairyBread;

using Api.Database;
using Api.Auth.Utils;

using static Api.Auth.AuthMutationsUtils;
using System.Text;
using Geralt;
using Api.Auth.Models;
using static Api.Auth.AuthQueriesUtils;
using HotChocolate.Authorization;

namespace Api.Auth;

[MutationType]
public static partial class AuthMutations
{
    [AllowAnonymous]
    [Authorize]
    public static async Task<RegisterPayload> RegisterMutation([Service] PGContext db, RegisterInput input)
    {
        var password_bytes = Encoding.UTF8.GetBytes(input.Password.Normalize());
        var hash_chars = new char[Argon2id.HashSize];
        Argon2id.ComputeHash(hash_chars, password_bytes, ARGON2ID_ITER, ARGON2ID_MEM_BYTES);
        string hash = new([.. hash_chars.Where(c => c != (char)byte.MinValue)]);

        var user = new UserEF()
        {
            Id = Guid.CreateVersion7(),
            Email = input.Email,
            PasswordHash = hash,
            Admin = (await LoginUtils.CanAdminRegister(db)) && input.AdminRegistration
        };

        await db.Users.AddAsync(user);
        string token = await LoginUtils.CreateUserSession(user.Id, input.ClientName, db);
        await db.SaveChangesAsync();

        return new RegisterPayload(UserMapper.ToDto(user), token, []);
    }
}
public static class AuthMutationsUtils
{

    // see https://www.rfc-editor.org/rfc/rfc9106.html#name-recommendations
    public const int ARGON2ID_ITER = 3;
    public const int ARGON2ID_MEM_BYTES = 64 * 1024 * 1024;

    public record RegisterInput(
            string Email,
            string Password,
            bool AdminRegistration,
            string ClientName = "unknown"
            );
    public record RegisterPayload(
            User User,
            string Token,
            IError[] Errors
            );
    public class RegisterInputValidator : AbstractValidator<RegisterInput>, IRequiresOwnScopeValidator
    {
        public RegisterInputValidator(PGContext db)
        {
            RuleFor(r => r.Email)
                .Must(e => new EmailAddressAttribute().IsValid(e))
                .WithMessage("Email is invalid");

            RuleFor(r => r.Email)
                .MustAsync(async (e, ct) =>
                        !await db.Users
                        .Where(u => u.Email == e)
                        .AnyAsync())
                .WithMessage("Email is already used");


            RuleFor(r => r.AdminRegistration)
                .MustAsync(async (adr, ct) =>
                        !adr || await LoginUtils.CanAdminRegister(db))
                .WithMessage("Can't Register As Admin");
        }
    }
}