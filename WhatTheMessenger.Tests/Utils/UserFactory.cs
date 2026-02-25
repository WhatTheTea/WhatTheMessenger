using System;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Tests.Utils;

public sealed class UserFactory
{
    public static User Create()
    {
        var id = Guid.NewGuid();
        return new()
        {
            Id = id,
            DisplayName = $"Test {id}",
            UserName = $"test-{id}"
        };
}
}
