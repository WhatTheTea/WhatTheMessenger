using NSubstitute;
using Shouldly;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Core.Models;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public sealed class ChatTests(SqliteFixture dbFixture) : IClassFixture<SqliteFixture> 
{
    private readonly IChatNotificationService chatNotificationService = Substitute.For<IChatNotificationService>();

    [Fact]
    public async Task ChatIsCreated()
    {
        using var _ = dbFixture.UseDb();
        using var arrangeContext = dbFixture.GetDbContext();
        var userFactory = new UserFactory(arrangeContext);
        var user = userFactory.Create();
        
        using var chatContext = dbFixture.GetDbContext();
        var chatService = new ChatService(chatContext, chatNotificationService);
        var chat = await chatService.CreateChatAsync(new()
        {
            Name = "test",
            Participants = [user.Id]
        });

        await chatNotificationService.Received().NotifyChatCreated(Arg.Is(chat));
        chatContext.Chats.Find(chat.Id).ShouldNotBeNull();
    }

    [Fact]
    public async Task ChatUsesParticipantsNames()
    {
        using var _ = dbFixture.UseDb();
        using var dbContext = dbFixture.GetDbContext();
        var chatService = new ChatService(dbContext, chatNotificationService);
        var userFactory = new UserFactory(dbContext);
        User[] users = [userFactory.Create("test"), userFactory.Create("test")];
        List<Guid> participants = [.. users.Select(x => x.Id)];

        var chat = await chatService.CreateChatAsync(new()
        {
            Name = null,
            Participants = participants
        });

        chat.Name.ShouldBe($"test, test");
    }

    [Fact]
    public async Task BothActorsReceivedMessage()
    {
        using var _ = dbFixture.UseDb();
        using var arrangeContext = dbFixture.GetDbContext();
        var userFactory = new UserFactory(arrangeContext);
        var (sender, receiver) = (userFactory.Create(), userFactory.Create());

        using var senderContext = dbFixture.GetDbContext();
        var chatService = new ChatService(senderContext, chatNotificationService);
        var chat = await chatService.CreateChatAsync(new() {Name = "test", Participants = [sender.Id, receiver.Id]});
        var message = await chatService.SendMessageAsync(new() {Content = "test message"}, chat, sender.Id);
        var senderChats = await chatService.GetChatsAsync(sender.Id);

        using var receiverContext = dbFixture.GetDbContext();
        var receiverChatService = new ChatService(receiverContext, chatNotificationService);
        var receiverChats = await receiverChatService.GetChatsAsync(receiver.Id);

        await chatNotificationService.Received().NotifyMessageSent(Arg.Is(message));
        receiverChats.ShouldNotBeEmpty();
        senderChats[0].Messages[0].ShouldBeEquivalentTo(message);
        receiverChats[0].Messages[0].ShouldBeEquivalentTo(message);
    }
}
