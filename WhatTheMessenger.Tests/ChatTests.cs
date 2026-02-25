using NSubstitute;
using Shouldly;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public sealed class ChatTests(SqliteFixture dbFixture) : IClassFixture<SqliteFixture> 
{
    [Fact]
    public async Task ChatIsCreated()
    {
        using var _ = dbFixture.GetAppDbContext(out var dbContext);
        var chatNotificationService = Substitute.For<IChatNotificationService>();
        var chatService = new ChatService(dbContext, chatNotificationService);
        var user = UserFactory.Create();

        dbContext.Users.Add(user);
        var chat = await chatService.CreateChatAsync(new()
        {
            Name = "test",
            Participants = [user.Id]
        }, user);

        await chatNotificationService.Received().NotifyChatCreated(Arg.Is(chat));
        dbContext.Chats.Find(chat.Id).ShouldNotBeNull();
    }
}
