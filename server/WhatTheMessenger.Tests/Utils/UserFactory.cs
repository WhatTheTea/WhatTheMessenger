using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Tests.Utils;

public sealed class UserFactory(IAppDbContext dbContext)
{
    public User Create(string? name = null)
    {
        var id = Guid.NewGuid();
        var user = new User()
        {
            Id = id,
            DisplayName = name ?? $"Test {id}",
            UserName = $"test-{id}"
        };
        dbContext.Users.Add(user);
        dbContext.SaveChangesAsync().GetAwaiter().GetResult();

        return user;
}
}
