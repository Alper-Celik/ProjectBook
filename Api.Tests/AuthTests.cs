using System.Threading.Tasks;

using Api.Auth.Endpoints;
// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Api.Tests;

public class AuthTests : TestInit
{

    async Task<HttpResponseMessage> AddUser(string name, bool asAdmin, HttpClient client, CancellationToken ct, string? password = null)
    {
        return await client.PostAsJsonAsync("api/auth/register", new RegisterEndpoints.RegisterDTO()
        {
            AdminRegistration = asAdmin,
            Email = $"{name}@projectread.ing",
            Password = password ?? "correct horse battery staple"
        }, cancellationToken: ct);
    }
    [Test]
    public async Task OnlyFirstAccountCanBeAdmin(CancellationToken ct)
    {
        // Given 
        var client = Factory.CreateClient();


        // When

        var preRegister_RegisterInfo = await GetRegisterInfo(ct, client);

        var firstRegisterResponse = await AddUser("didnt_wanted_to_be_admin", false, client, ct);

        var postNonAdminRegister_RegisterInfo = await GetRegisterInfo(ct, client);

        var adminRegisterResponse = await AddUser("sysadmin", true, client, ct);

        await Task.Delay(300);

        var postAdminRegister_RegisterInfo = await GetRegisterInfo(ct, client);

        var failedRegisterResponse = await AddUser("want_poweeer", true, client, ct);

        // Then

        await Assert.That(preRegister_RegisterInfo?.CanRegisterAsAdmin).IsTrue();
        await Assert.That(() => firstRegisterResponse.EnsureSuccessStatusCode()).ThrowsNothing();
        await Assert.That(postNonAdminRegister_RegisterInfo?.CanRegisterAsAdmin).IsTrue();
        await Assert.That(() => adminRegisterResponse.EnsureSuccessStatusCode()).ThrowsNothing();
        await Assert.That(postAdminRegister_RegisterInfo?.CanRegisterAsAdmin).IsFalse();
        await Assert.That(failedRegisterResponse.StatusCode).EqualTo(System.Net.HttpStatusCode.BadRequest);
    }

    private static async Task<RegisterEndpoints.RegisterInfo?> GetRegisterInfo(CancellationToken ct, HttpClient? client) => await (
                    await client.GetAsync(
                           "api/auth/register_info", ct))
                   .EnsureSuccessStatusCode()
                   .Content.ReadFromJsonAsync<RegisterEndpoints.RegisterInfo>(cancellationToken: ct);

    [Test]
    public async Task CantLoginWithWrongPassword(CancellationToken ct)
    {
        var client = Factory.CreateClient();

        var user = await AddUser("admin", true, client, ct, "hunter2");


        var loginFail = await client.PostAsJsonAsync("api/auth/login", new LoginEndpoints.LoginDTO("admin@projectread.ing", "*******"));
        var loginSuccess = await client.PostAsJsonAsync("api/auth/login", new LoginEndpoints.LoginDTO("admin@projectread.ing", "hunter2"));

        await Assert.That(loginFail.StatusCode).EqualTo(System.Net.HttpStatusCode.Forbidden);
        await Assert.That(loginSuccess.EnsureSuccessStatusCode).ThrowsNothing();
    }
}