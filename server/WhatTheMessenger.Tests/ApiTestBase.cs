using System;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public abstract class ApiTestBase : IClassFixture<IDbFixture>
{
    protected readonly IDbFixture DbFixture;
    protected readonly HttpClient Client;

    protected ApiTestBase(IDbFixture dbFixture)
    {
        DbFixture = dbFixture;
        var factory = new ChatApiFactory(DbFixture);
        Client = factory.CreateClient();
    }

    protected void AuthenticateAs(Guid userId)
    {
        Client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        Client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
    }

    protected void AuthenticateAsAnonymous()
    {
        Client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
    }
}
