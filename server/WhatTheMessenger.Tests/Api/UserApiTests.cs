using System;
using System.Net.Http.Json;
using Shouldly;
using WhatTheMessenger.Api.Features.Users;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Api;

public class AuthApiTests(DbFixture dbFixture) : ApiTestBase(dbFixture)
{
    [Fact]
    public async Task Register_UserIsRegistered()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();

        var request = new RegisterModel()
        {
            Login = "test",
            Nickname = "test test",
            Password = "Testpass123"
        };

        var response = await Client.PostAsJsonAsync(AuthEndpoints.AuthEndpointPrefix + "/register", request);

        response.IsSuccessStatusCode.ShouldBeTrue();
        dbContext.Users.FirstOrDefault(x => x.NormalizedUserName == "TEST").ShouldNotBeNull();
        response.Headers.Contains("Set-Cookie").ShouldBeTrue();
    }

    [Fact]
    public async Task Login_UserIsSignedIn()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        var user = userFactory.Create("test");

        var request = new LoginModel()
        {
            Login = user.UserName!,
            Password = "Testpass123",
            RememberMe = true,
        };
        var response = await Client.PostAsJsonAsync(AuthEndpoints.AuthEndpointPrefix + "/login", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Headers.Contains("Set-Cookie").ShouldBeTrue();

    }
}
