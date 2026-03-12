using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Tests.Utils;

public sealed class ChatFactory
{
    public static Chat Create(params User[] users) => new()
    {
        Users = [.. users],
        Id = Guid.NewGuid(),
    };
}
