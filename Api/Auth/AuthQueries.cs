using static Api.Auth.Utils.LoginUtils;
using static Api.Auth.Endpoints.RegisterEndpoints;

using Api.Database;
using Api.Auth.Models;

using Riok.Mapperly.Abstractions;

using System.Runtime.Serialization;

namespace Api.Auth;

[QueryType]
public static partial class AuthQueries
{
    public static async Task<RegisterInfo> GetRegisterInfoAsync([Service] PGContext db)
        => new(
          CanRegisterAsAdmin: await CanAdminRegister(db)
         );
}

public static partial class AuthQueriesUtils
{

    public record User(
            Guid Id,
            string Email,
            bool EmailVerified,
            bool Admin);

    [Mapper]
    public static partial class UserMapper
    {
        [MapperIgnoreSource(nameof(UserEF.PasswordHash))]
        public static partial User ToDto(UserEF o);
    }
}