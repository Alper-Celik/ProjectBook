// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Api.Tests;

public class AuthTests : TestInit
{
    [Fact]
    public async Task OnlyFirstAccountCanBeAdmin()
    {
        var registerInfo_1 = await (await
                Client.GetAsync(
                    "api/auth/register_info",
                    TestContext.Current.CancellationToken))
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<Auth.Endpoints.RegisterEndpoints.RegisterInfo>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(registerInfo_1);
        Assert.True(registerInfo_1.CanRegisterAsAdmin);

    }
}