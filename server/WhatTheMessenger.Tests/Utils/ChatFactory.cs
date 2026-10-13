using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Tests.Utils;

public sealed class ChatFactory(IAppDbContext dbContext)
{
    public static Chat New(params User[] users) => new()
    {
        Users = [.. users]
    };

    public Chat Create(params User[] users)
    {
        var chat = New(users);
        dbContext.Chats.Add(chat);
        dbContext.SaveChangesAsync().GetAwaiter().GetResult();

        return chat;
    }
}
