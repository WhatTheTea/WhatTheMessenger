using Shouldly;
using WhatTheMessenger.Api.Features.Users;
using WhatTheMessenger.Core;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Slice;

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
        var getUsersHandler = new GetUsers.Handler(arrangeContext);
        var foundAlexUsers = await getUsersHandler.HandleAsync(new("alex"));

        foundAlexUsers.Users.Count().ShouldBe(2);
    }

    [Fact]
    public async Task UsersFoundByUserName()
    {
        using var _ = dbFixture.UseDb();
        using var arrangeContext = dbFixture.GetDbContext();
        var userFactory = new UserFactory(arrangeContext);
        User[] users = [userFactory.Create("alex"), userFactory.Create("alexia"), userFactory.Create("andrew")];

         using var actContext = dbFixture.GetDbContext();
        var getUsersHandler = new GetUsers.Handler(arrangeContext);
        var foundAlexUsers = await getUsersHandler.HandleAsync(new("test"));

        foundAlexUsers.Users.Count().ShouldBe(3);
    }
}
