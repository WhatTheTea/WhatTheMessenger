using WhatTheMessenger.Core;

namespace WhatTheMessenger.Tests.Utils;

public sealed class ChatFactory
{
    public static Chat Create(params User[] users) => new()
    {
        Users = [.. users],
        Id = Guid.NewGuid(),
    };
}
