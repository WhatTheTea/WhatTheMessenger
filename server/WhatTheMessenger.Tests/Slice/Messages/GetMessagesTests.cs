using Shouldly;
using WhatTheMessenger.Api.Features.Messages;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Slice.Messages;

public sealed class GetMessagesTests : IClassFixture<DbFixture>, IDisposable
{
    private readonly IDisposable db;
    private readonly ApplicationDbContext dbContext;
    private readonly ApplicationDbContext handlerDbContext;

    private readonly UserFactory userFactory;
    private readonly ChatFactory chatFactory;

    public GetMessagesTests(DbFixture dbFixture)
    {
        db = dbFixture.UseDb();
        dbContext = dbFixture.GetDbContext();
        handlerDbContext = dbFixture.GetDbContext();
        userFactory = new(dbContext);
        chatFactory = new(dbContext);
    }

    private Task SendMessage(Core.User user1, Core.Chat chat, int i)
    {
        var message1 = chat.SendMessage(user1.Id, $"test {i}");
        dbContext.Messages.Add(message1);
        return Task.Delay(1); // Add delay in order for Guid v7 to work correctly 
    }

    public void Dispose()
    {
        db.Dispose();
        dbContext.Dispose();
        handlerDbContext.Dispose();
    }

    [Fact]
    public async Task FirstPage_IsNotEmpty()
    {
        var handler = new GetMessages.Handler(handlerDbContext);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        for (int i = 45; i > 0; i--)
        {
            await SendMessage(user, chat, i);
        }
        dbContext.SaveChanges();

        var result = await handler.HandleAsync(new(user.Id, chat.Id));

        result.LastMessageId.ShouldNotBeNull();
        result.Count.ShouldNotBe(0);
        result.Messages.ShouldNotContain(x => x.Content.Contains("21"));
    }

    [Fact]
    public async Task NextPage_IsNotEmpty()
    {
        var handler = new GetMessages.Handler(handlerDbContext);
        var user1 = userFactory.Create("test_user1");
        var user2 = userFactory.Create("test_user2");
        var chat = chatFactory.Create(user1, user2);

        for (int i = 60; i > 0; i--)
        {
            await SendMessage(user1, chat, i);
        }
        dbContext.SaveChanges();

        var firstPage = await handler.HandleAsync(new(user1.Id, chat.Id));
        firstPage.LastMessageId.ShouldNotBeNull();
        firstPage.Count.ShouldNotBe(0);
        firstPage.Messages.ShouldNotContain(x => x.Content.Contains("21"));

        var secondPage = await handler.HandleAsync(new(user1.Id, chat.Id, 20, firstPage.LastMessageId));
        secondPage.LastMessageId.ShouldNotBeNull();
        secondPage.Count.ShouldNotBe(0);
        secondPage.Messages.ShouldNotContain(x => x.Content.Contains("41"));
        secondPage.Messages.ShouldContain(x => x.Content.Contains("21"));
        secondPage.Messages.ShouldNotContain(x => x.Id == firstPage.LastMessageId);
    }

    [Fact]
    public async Task WhenEndOfData_IsEmpty()
    {
        var handler = new GetMessages.Handler(handlerDbContext);
        var user1 = userFactory.Create("test_user1");
        var chat = chatFactory.Create(user1);

        for (int i = 0; i < 10; i++)
        {
            await SendMessage(user1, chat, i);
        }
        dbContext.SaveChanges();

        var firstPage = await handler.HandleAsync(new(user1.Id, chat.Id));
        firstPage.LastMessageId.ShouldNotBeNull();
        firstPage.Count.ShouldNotBe(0);

        var lastPage = await handler.HandleAsync(new(user1.Id, chat.Id, BeforeMessageId: firstPage.LastMessageId));
        lastPage.LastMessageId.ShouldBeNull();
        lastPage.Count.ShouldBe(0);
        lastPage.Messages.ShouldBeEmpty();
    }

    [Fact]
    public async Task WhenNonMember_IsEmpty()
    {
        var handler = new GetMessages.Handler(handlerDbContext);
        var user1 = userFactory.Create("test_user1");
        var user2 = userFactory.Create("test_user2");
        var chat = chatFactory.Create(user1);

        for (int i = 0; i < 20; i++)
        {
             await SendMessage(user1, chat, i);
        }
        dbContext.SaveChanges();

        var result = await handler.HandleAsync(new(user2.Id, chat.Id));
        result.Count.ShouldBe(0);
        result.Messages.ShouldBeEmpty();
    }

    [Fact]
    public async Task RequestForNegativeCount_HandlesGracefully()
    {
        var handler = new GetMessages.Handler(handlerDbContext);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        for (int i = 0; i < 5; i++)
        {
            await SendMessage(user, chat, i);
        }
        dbContext.SaveChanges();

        var result = await handler.HandleAsync(new(user.Id, chat.Id, Count: -5));

        result.Count.ShouldBeGreaterThanOrEqualTo(0);
    }
}

