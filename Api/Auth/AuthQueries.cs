using static Api.Auth.Utils.LoginUtils;
using static Api.Auth.Endpoints.RegisterEndpoints;

using Api.Database;
using Api.Auth.Models;

using Riok.Mapperly.Abstractions;

using HotChocolate.Authorization;

namespace Api.Auth;

[QueryType]
public partial class AuthQueries
{
    // [AllowAnonymous]
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
        public static partial IQueryable<User> ProjectToDto(IQueryable<UserEF> q);

        [MapperIgnoreSource(nameof(UserEF.PasswordHash))]
        public static partial User ToDto(UserEF o);
    }
}