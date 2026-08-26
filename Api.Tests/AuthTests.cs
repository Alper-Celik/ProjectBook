using System.Threading.Tasks;
// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Api.Tests;

public class AuthTests : TestInit
{
    [Test]
    public async Task OnlyFirstAccountCanBeAdmin(CancellationToken ct)
    {
        var registerInfo_1 = await (await
                Client.GetAsync(
                    "api/auth/register_info",
                    ct))
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<Auth.Endpoints.RegisterEndpoints.RegisterInfo>(cancellationToken: ct);

        await Assert.That(registerInfo_1).IsNotNull();
        await Assert.That(registerInfo_1.CanRegisterAsAdmin).IsTrue();
    }
}