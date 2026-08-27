// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;

using Api.Auth.Utils;
using Api.Database;

using FluentValidation;

using Geralt;

using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    public static async Task<
    Results<
        Ok<LoginUtils.LoginResultDTO>,
        ForbidHttpResult
    >> Login(
            HttpContext ctx,
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

        var password = Encoding.UTF8.GetBytes(loginDTO.Password.Normalize());
        if (userId is not null &&
                Argon2id.VerifyHash(hash, password))
        {
            var token = await LoginUtils.CreateUserSession(userId.Value, userAgent, db);
            await db.SaveChangesAsync();

            return LoginUtils.LogUserIn(ctx, token);
        }
        return TypedResults.Forbid();
    }


    public record LoginDTO(string UserEmail, string Password);
}