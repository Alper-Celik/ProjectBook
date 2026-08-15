using System.Buffers.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;

using Api.Auth.Models;
using Api.Auth.Utils;
using Api.Database;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Auth.Handlers;

class AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, PGContext db)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string?[] tokenHashes = [Context.Request.Headers.Authorization.Last(), Context.Request.Cookies[LoginUtils.TokenCookieName]];
        List<byte[]> userTokenHashes = [.. tokenHashes
            .Where(s => s != null && s.StartsWith(LoginUtils.UserTokenPrefix))
        .Select(s => Base64Url.DecodeFromChars(
                    s.AsSpan()[(LoginUtils.UserTokenPrefix.Length - 1)..]))];

        var userToken = await db.UserTokens.Where(ut => userTokenHashes.Contains(ut.TokenHash)).FirstAsync();

        if (userToken is not null)
        {
            await LoginUtils.UpdateLastUsedForUserToken(userToken, db);


            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier,userToken.UserId.ToString()),
                new Claim(UserToken.PermissionBitsType,((long)userToken.Permissions).ToString()),
            ];

            return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(
                                new ClaimsPrincipal(new ClaimsIdentity(
                                        claims
                                        )
                                    )
                            ),
                        Scheme.Name)
                    );
        }


        return AuthenticateResult.NoResult();
    }

}