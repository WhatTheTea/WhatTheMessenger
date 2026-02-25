using NSubstitute;
using Shouldly;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public sealed class ChatTests(SqliteFixture dbFixture) : IClassFixture<SqliteFixture> 
{
    private readonly IChatNotificationService chatNotificationService = Substitute.For<IChatNotificationService>();

    [Fact]
    public async Task ChatIsCreated()
    {
        using var _ = dbFixture.GetAppDbContext(out var dbContext);
        var chatService = new ChatService(dbContext, chatNotificationService);
        var userFactory = new UserFactory(dbContext);
        var user = userFactory.Create();

        var chat = await chatService.CreateChatAsync(new()
        {
            Name = "test",
            Participants = [user.Id]
        });

        await chatNotificationService.Received().NotifyChatCreated(Arg.Is(chat));
        dbContext.Chats.Find(chat.Id).ShouldNotBeNull();
    }

    [Fact]
    public async Task MessageIsReceived()
    {
        using var _ = dbFixture.GetAppDbContext(out var dbContext);
        var chatService = new ChatService(dbContext, chatNotificationService);
        var userFactory = new UserFactory(dbContext);
        var (sender, receiver) = (userFactory.Create(), userFactory.Create());

        var chat = await chatService.CreateChatAsync(new() {Name = "test", Participants = [sender.Id, receiver.Id]});
        var message = await chatService.SendMessageAsync(new() {Content = "test message"}, chat, sender.Id);

        await chatNotificationService.Received().NotifyMessageSent(Arg.Is(message));
        sender.Chats.First().Messages.ShouldContain(message);
        receiver.Chats.First().Messages.ShouldContain(message);
    }
}
