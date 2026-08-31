// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;

using Api.Auth.Endpoints;

using ZeroQL.Client;

namespace Api.Tests;

public class AuthTests : TestInit
{

    [Test]
    public async Task OnlyFirstAccountCanBeAdmin(CancellationToken ct)
    {

        var httpClient = Factory.CreateClient();
        var client = new ApiClient(httpClient);

        // var addUser = client.Mutation(new RegisterMutationInput(), m => m.RegisterMutation())
    }

    private static Task<ZeroQL.GraphQLResult<bool>> CanRegisterAdmin(ApiClient? client) => client.Query(q => q.RegisterInfo(r => r.CanRegisterAsAdmin));


    [Test]
    public async Task CantLoginWithWrongPassword(CancellationToken ct)
    {

    }
}