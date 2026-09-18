using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Application.Services;

namespace WhatTheMessenger.Server.Api;

public static class Chat
{
    extension (WebApplication app)
    {
        public WebApplication MapChatEndpoints()
        {
            var group = app.MapGroup("/api/chats");

            group.MapPost("/create", async (NewChatModel newChat, IChatService chatService) =>
                {
                    await chatService.CreateChatAsync(newChat);
                    return Results.Accepted();
                })
                .RequireAuthorization()
                .Accepts(typeof(NewChatModel), System.Net.Mime.MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/user/{id}", async (Guid id, IChatService chatService) =>
                {
                    var chats = await chatService.GetChatsAsync(id);
                    return chats.Select(chat => new
                    {
                        id = chat.Id,
                        name = chat.Name,
                        users = chat.Users.Select(user => new
                        {
                           id = user.Id,
                           username = user.UserName,
                           displayName = user.DisplayName, 
                        }),
                        messages = chat.Messages.Select(message => new
                        {
                            id = message.Id,
                            chatId = chat.Id,
                            senderId = message.SenderId,
                            content = message.Content,
                            sentAt = message.SentAt,
                            status = message.Status  
                        })
                    });
                }
                )
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}