using System;
using System.Net.Http.Json;
using Shouldly;
using WhatTheMessenger.Api.Features.Chats;
using WhatTheMessenger.Core;
using WhatTheMessenger.Tests.Utils;

namespace WhatTheMessenger.Tests.Api;

public class ChatApiTests(DbFixture dbFixture) : ApiTestBase(dbFixture)
{
    [Fact]
    public async Task GetChats_ReturnsUserChats()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        AuthenticateAs(user.Id);
        var response = await Client.GetAsync($"{ChatEndpoints.Prefix}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var chats = await response.Content.ReadFromJsonAsync<ChatDto[]>();
        chats.ShouldNotBeNull();
        chats.ShouldContain(c => c.Id == chat.Id);
    }

    [Fact]
    public async Task GetChatById_ReturnsSpecificChat()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        AuthenticateAs(user.Id);
        var response = await Client.GetAsync($"{ChatEndpoints.Prefix}/{chat.Id}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var chatDto = await response.Content.ReadFromJsonAsync<ChatDto>();
        chatDto.ShouldNotBeNull();
        chatDto.Id.ShouldBe(chat.Id);
    }

    [Fact]
    public async Task CreateChat_ReturnsAccepted()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("current_user");
        var targetUser = userFactory.Create("target_user");

        AuthenticateAs(user.Id);
        var request = new CreateChat.Request(null, [targetUser.Id, user.Id]);
        
        var response = await Client.PostAsJsonAsync($"{ChatEndpoints.Prefix}/create", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetChatById_ReturnsNotFound_WhenChatDoesNotExist()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        AuthenticateAs(user.Id);
        var response = await Client.GetAsync($"{ChatEndpoints.Prefix}/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateChat_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("current_user");
        var targetUser = userFactory.Create("target_user");

        var request = new CreateChat.Request(null, [targetUser.Id, user.Id]);
        
        var response = await Client.PostAsJsonAsync($"{ChatEndpoints.Prefix}/create", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LeaveChat_WhenChatExists_ReturnsOk()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        AuthenticateAs(user.Id);
        var response = await Client.DeleteAsync($"{ChatEndpoints.Prefix}/{chat.Id}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        dbContext.Chats.Where(x => x.Users.Any(x => x.Id == user.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task LeaveChat_WhenChatDoesNotExist_ReturnsNotFound()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        AuthenticateAs(user.Id);
        var response = await Client.DeleteAsync($"{ChatEndpoints.Prefix}/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LeaveChat_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = ChatFactory.New(user, otherUser);
        dbContext.Chats.Add(chat);
        dbContext.SaveChanges();

        var response = await Client.DeleteAsync($"{ChatEndpoints.Prefix}/{chat.Id}");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMessages_ReturnsChatMessages()
    {
        // Arrange
        using var _ = DbFixture.UseDb();
        using var dbContext = DbFixture.GetDbContext();
        var userFactory = new UserFactory(dbContext);
        var chatFactory = new ChatFactory(dbContext);
        var user = userFactory.Create("test_user");
        var otherUser = userFactory.Create("other_user");
        var chat = chatFactory.Create(user, otherUser);

        // Create a message
        var message = chat.SendMessage(user.Id, "test");
        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        AuthenticateAs(user.Id);
        var response = await Client.GetAsync($"{ChatEndpoints.Prefix}/{chat.Id}/messages");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetMessages.Response>();
        result.ShouldNotBeNull();
        result.Messages.ShouldContain(m => m.Id == message.Id);
    }
}
