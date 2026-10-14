using NSubstitute;
using Shouldly;
using WhatTheMessenger.Api.Features.Messages;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Slice.Messages;

public sealed class SendMessageTests : IClassFixture<DbFixture>, IDisposable
{
    private readonly IDisposable db;
    private readonly ApplicationDbContext dbContext;
    private readonly ApplicationDbContext handlerDbContext;
    private readonly IChatNotificationService notificationService = Substitute.For<IChatNotificationService>();

    private readonly UserFactory userFactory;
    private readonly ChatFactory chatFactory;

    public SendMessageTests(DbFixture dbFixture)
    {
        db = dbFixture.UseDb();
        dbContext = dbFixture.GetDbContext();
        handlerDbContext = dbFixture.GetDbContext();
        userFactory = new(dbContext);
        chatFactory = new(dbContext);
    }

    public void Dispose()
    {
        db.Dispose();
        dbContext.Dispose();
        handlerDbContext.Dispose();
    }

    [Fact]
    public async Task Sent_WhenChatExists_AndUserIsMember()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        var result = await handler.HandleAsync(new(user.Id, chat.Id, "Hello World"));

        result.Status.ShouldBe(MessageStatus.Sent);
    }

    [Fact]
    public async Task Fails_WhenChatDoesNotExist()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user = userFactory.Create("test_user");

        var result = await handler.HandleAsync(new(user.Id, Guid.NewGuid(), "Non-existent chat"));

        result.Status.ShouldBe(MessageStatus.Fail);
    }

    [Fact]
    public async Task Fails_WhenSenderNotInChat()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user1 = userFactory.Create("user1");
        var user2 = userFactory.Create("user2");
        var chat = chatFactory.Create(user1); // user2 not a member

        var result = await handler.HandleAsync(new(user2.Id, chat.Id, "Message from outsider"));

        result.Status.ShouldBe(MessageStatus.Fail);
    }

    [Fact]
    public async Task Saved_WhenSent()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        await handler.HandleAsync(new(user.Id, chat.Id, "Test message"));

        var message = dbContext.Messages
            .Where(m => m.ChatId == chat.Id && m.SenderId == user.Id)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefault();

        message.ShouldNotBeNull();
        message.Content.ShouldBe("Test message");
    }

    [Fact]
    public async Task SentAt_IsCorrect()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        await handler.HandleAsync(new(user.Id, chat.Id, "Timestamp test"));

        var message = dbContext.Messages
            .Where(m => m.ChatId == chat.Id && m.SenderId == user.Id)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefault();

        message.ShouldNotBeNull();
        message.SentAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task Status_IsCorrect()
    {
        var handler = new SendMessage.Handler(handlerDbContext, notificationService);
        var user = userFactory.Create("test_user");
        var chat = chatFactory.Create(user);

        await handler.HandleAsync(new(user.Id, chat.Id, "Status test"));

        var message = dbContext.Messages
            .Where(m => m.ChatId == chat.Id && m.SenderId == user.Id)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefault();

        message.ShouldNotBeNull();
        message.Status.ShouldBe(MessageStatus.Sent);
    }
}

