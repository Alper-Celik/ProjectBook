using System.Buffers.Text;

using Api.Auth.Models;
using Api.Database;

using Geralt;

using Microsoft.EntityFrameworkCore;

using NodaTime;

namespace Api.Auth.Utils;

public static class LoginUtils
{

    public const char PrefixSeparator = '_';

    public const string UserTokenPrefix = "user";

    public static async Task<string> CreateUserSession(Guid userId, string sessionName, PGContext ctx)
    {

        var apiToken = new byte[32];
        SecureRandom.Fill(apiToken);

        Span<byte> tokenHash = stackalloc byte[32];
        BLAKE2b.ComputeHash(tokenHash, apiToken);

        var dbToken = new UserToken
        {
            UserId = userId,
            TokenHash = tokenHash.ToArray(),
            CreationTime = SystemClock.Instance.GetCurrentInstant(),
        };

        await ctx.UserTokens.AddAsync(dbToken);

        return UserTokenPrefix + PrefixSeparator + Base64Url.EncodeToString(apiToken);
    }

    public static async Task UpdateLastUsedForUserToken(UserToken token, PGContext ctx)
    {
        var currentTime = SystemClock.Instance.GetCurrentInstant();
        if (token.LastUsed is null ||
                token.LastUsed + Duration.FromMinutes(1) <= currentTime)
        {
            await ctx.UserTokens.Where(ut => ut.TokenHash.SequenceEqual(token.TokenHash))
                .ExecuteUpdateAsync(setter =>
                        setter.SetProperty(ut => ut.LastUsed, currentTime));
        }
    }
}