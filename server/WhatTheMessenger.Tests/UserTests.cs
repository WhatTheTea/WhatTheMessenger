using Shouldly;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Core.Models;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public class UserTests(SqliteFixture dbFixture) : IClassFixture<SqliteFixture> 
{
    [Fact]
    public async Task UsersFoundByDisplayName()
    {
        using var _ = dbFixture.UseDb();
        using var arrangeContext = dbFixture.GetDbContext();
        var userFactory = new UserFactory(arrangeContext);
        User[] users = [userFactory.Create("alex"), userFactory.Create("alexia"), userFactory.Create("andrew")];

        using var actContext = dbFixture.GetDbContext();
        var userService = new UserService(actContext);
        var foundAlexUsers = await userService.FindUsersAsync("alex");

        foundAlexUsers.Length.ShouldBe(2);
    }

    [Fact]
    public async Task UsersFoundByUserName()
    {
        using var _ = dbFixture.UseDb();
        using var arrangeContext = dbFixture.GetDbContext();
        var userFactory = new UserFactory(arrangeContext);
        User[] users = [userFactory.Create("alex"), userFactory.Create("alexia"), userFactory.Create("andrew")];

        using var actContext = dbFixture.GetDbContext();
        var userService = new UserService(actContext);
        var foundTestUsers = await userService.FindUsersAsync("test");

        foundTestUsers.Length.ShouldBe(3);
    }
}
