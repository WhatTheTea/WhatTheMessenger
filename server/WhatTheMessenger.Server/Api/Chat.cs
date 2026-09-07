using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Application.Services;

namespace WhatTheMessenger.Server.Api;

public static class Chat
{
    extension (WebApplication app)
    {
        public WebApplication MapChatEndpoints()
        {
            var group = app.MapGroup("/api/chat");

            group.MapPost("/create", async (NewChatModel newChat, ChatService chatService) => 
                await chatService.CreateChatAsync(newChat))
                    .RequireAuthorization()
                    .Accepts(typeof(NewChatModel), System.Net.Mime.MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status202Accepted)
                    .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}