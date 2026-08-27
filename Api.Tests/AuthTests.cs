using System.Threading.Tasks;

using Api.Auth.Endpoints;
// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Api.Tests;

public class AuthTests : TestInit
{
    [Test]
    public async Task OnlyFirstAccountCanBeAdmin(CancellationToken ct)
    {
        // Given 
        var client = Factory.CreateClient();

        async Task<RegisterEndpoints.RegisterInfo?> GetRegisterInfo() => await (
                await client.GetAsync(
                       "api/auth/register_info", ct))
               .EnsureSuccessStatusCode()
               .Content.ReadFromJsonAsync<RegisterEndpoints.RegisterInfo>(cancellationToken: ct);

        async Task<HttpResponseMessage> AddUser(string name, bool asAdmin) => await client.PostAsJsonAsync("api/auth/register", new RegisterEndpoints.RegisterDTO()
        {
            AdminRegistration = asAdmin,
            Email = $"{name}@projectread.ing",
            Password = "correct horse battery staple"
        }, cancellationToken: ct);

        // When

        var preRegister_RegisterInfo = await GetRegisterInfo();

        var firstRegisterResponse = await AddUser("didnt_wanted_to_be_admin", false);

        var postNonAdminRegister_RegisterInfo = await GetRegisterInfo();

        var adminRegisterResponse = await AddUser("sysadmin", true);

        await Task.Delay(300);

        var postAdminRegister_RegisterInfo = await GetRegisterInfo();

        var failedRegisterResponse = await AddUser("want_poweeer", true);

        // Then

        await Assert.That(preRegister_RegisterInfo?.CanRegisterAsAdmin).IsTrue();
        await Assert.That(() => firstRegisterResponse.EnsureSuccessStatusCode()).ThrowsNothing();
        await Assert.That(postNonAdminRegister_RegisterInfo?.CanRegisterAsAdmin).IsTrue();
        await Assert.That(() => adminRegisterResponse.EnsureSuccessStatusCode()).ThrowsNothing();
        await Assert.That(postAdminRegister_RegisterInfo?.CanRegisterAsAdmin).IsFalse();
        await Assert.That(failedRegisterResponse.StatusCode).EqualTo(System.Net.HttpStatusCode.BadRequest);


    }
}