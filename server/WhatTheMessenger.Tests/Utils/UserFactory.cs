using Microsoft.AspNetCore.Identity;
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
            UserName = $"test-{id}",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        user.NormalizedUserName = user.UserName.ToUpper();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Testpass123");

        dbContext.Users.Add(user);
        dbContext.SaveChangesAsync().GetAwaiter().GetResult();

        return user;
}
}
