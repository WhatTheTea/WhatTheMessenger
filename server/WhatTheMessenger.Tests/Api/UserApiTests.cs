using System;
using System.Net.Http.Json;
using Shouldly;
using WhatTheMessenger.Api.Features.Users;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Api;

public class UserApiTests(DbFixture dbFixture) : ApiTestBase(dbFixture)
{
    [Fact]
    public async Task Search_ReturnsMatchingUsers()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var targetUser = userFactory.Create("search_target");
        var currentUser = userFactory.Create("current_user");

        // pretend user is signed in
        Client.DefaultRequestHeaders.Add("X-Test-UserId", currentUser.Id.ToString());
        var response = await Client.GetAsync(UserEndpoints.Prefix + $"/search/{targetUser.UserName}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<GetUsers.Response>();
        results.ShouldNotBeNull();
        results.Users.ShouldContain(u => u.Username == targetUser.UserName);
    }

    [Fact]
    public async Task Users_ReturnUnauthorized()
    {
        using var _ = DbFixture.UseDb();

        HttpResponseMessage[] responses = [
            await Client.GetAsync(UserEndpoints.Prefix + "/search/anybody"),
            await Client.GetAsync(UserEndpoints.Prefix + "/0c11e6e3-cf73-4314-82f9-0fe8d51cd082"),
            ];

        responses.ShouldAllBe(x => x.StatusCode == System.Net.HttpStatusCode.Unauthorized);
    }
}
