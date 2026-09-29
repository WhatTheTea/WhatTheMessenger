using NSubstitute;
using Shouldly;
using WhatTheMessenger.Api.Features.Chats;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Core;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Slice;

public sealed class ChatTests(DbFixture dbFixture) : IClassFixture<DbFixture> 
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
        var createChatHandler = new CreateChat.Handler(chatContext, chatNotificationService);
        var response = await createChatHandler.HandleAsync(new ("test", [user.Id]));
        var chatId = response.NewChatId;

        chatContext.Chats.Find(chatId).ShouldNotBeNull();
        await chatNotificationService.Received().NotifyChatCreated(Arg.Is<Chat>(x => x.Name == "test"));
    }

    [Fact]
    public async Task ChatUsesParticipantsNames()
    {
        using var _ = dbFixture.UseDb();
        using var dbContext = dbFixture.GetDbContext();
        var newChatHandler = new CreateChat.Handler(dbContext, chatNotificationService);
        var userFactory = new UserFactory(dbContext);
        User[] users = [userFactory.Create("test"), userFactory.Create("test")];
        List<Guid> participants = [.. users.Select(x => x.Id)];

        var response = await newChatHandler.HandleAsync(new(null!, participants));
        var chatId = response.NewChatId;

        dbContext.Chats.Find(chatId).ShouldNotBeNull();
        dbContext.Chats.Find(chatId)?.Name.ShouldBe($"test, test");
    }
}
