using System;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Tests.Utils;

public sealed class UserFactory(IAppDbContext dbContext)
{
    public User Create()
    {
        var id = Guid.NewGuid();
        var user = new User()
        {
            Id = id,
            DisplayName = $"Test {id}",
            UserName = $"test-{id}"
        };
        dbContext.Users.Add(user);
        dbContext.SaveChangesAsync().GetAwaiter().GetResult();

        return user;
}
}
