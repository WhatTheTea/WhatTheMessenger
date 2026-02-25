using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests;

public class ChatTests
{
    private readonly IAppDbContext dbContext;

    public ChatTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("WhatTheMessenger")
        .Options;

        dbContext = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ChatIsCreated()
    {
        var chatNotificationService = Substitute.For<IChatNotificationService>();
        var chatService = new ChatService(dbContext, chatNotificationService);
        var user = UserFactory.Create();

        var chat = await chatService.CreateChatAsync(new()
        {
            Name = "test",
            Participants = [user.Id]
        }, user);

        await chatNotificationService.Received().NotifyChatCreated(Arg.Is(chat));
    }
}
