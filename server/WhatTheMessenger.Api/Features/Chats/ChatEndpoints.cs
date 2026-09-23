using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class ChatEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapChatEndpoints()
        {
            var group = app.MapGroup("/api/chats");

            group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, IAppDbContext dbContext) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await dbContext.Chats.AsNoTracking()
                    .Include(x => x.Messages)
                    .Include(x => x.Users)
                    .Where(chat => chat.Id == id && chat.Users.Any(user => user.Id == userId))
                    .SingleOrDefaultAsync();

                return result is not null ? Results.Ok(ChatDto.From(result))
                    : Results.NotFound();
            })
            .RequireAuthorization()
            .Produces<ChatDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/create", async (NewChatModel newChat, IChatService chatService) =>
                {
                    await chatService.CreateChatAsync(newChat);
                    return Results.Accepted();
                })
                .RequireAuthorization()
                .Accepts(typeof(NewChatModel), System.Net.Mime.MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/user/{id:guid}", async (Guid id, IChatService chatService) =>
                {
                    var chats = await chatService.GetChatsAsync(id);
                    return chats.Select(chat => ChatDto.From(chat)).ToArray();
                })
                .RequireAuthorization()
                .Produces<ChatDto[]>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}