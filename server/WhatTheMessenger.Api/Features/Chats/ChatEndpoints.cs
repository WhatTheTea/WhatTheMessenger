using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Api.Features.Shared;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class ChatEndpoints
{
    public static IServiceCollection AddChatHandlers(this IServiceCollection services) =>
        services.AddTransient<IHandler<CreateChat.Request, Nothing>, CreateChat.Handler>();

    extension(WebApplication app)
    {
        public WebApplication MapChatEndpoints()
        {
            var group = app.MapGroup("/api/chats");

            group.MapGet("/user/me", async (ClaimsPrincipal claims, IAppDbContext dbContext) =>
            {
                var userId = claims.GetUserId();

                var chats = await dbContext.Chats.AsNoTracking()
                    .GetChatsForUser(userId)
                    .Select(ChatDto.FromEntity)
                    .ToArrayAsync();

                return Results.Ok(chats ?? []);
            })
            .RequireAuthorization()
            .Produces<ChatDto[]>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);


            group.MapGet("/user/me/{id:guid}", async (Guid id, ClaimsPrincipal claims, IAppDbContext dbContext) =>
            {
                var userId = claims.GetUserId();

                var result = await dbContext.Chats.AsNoTracking()
                    .GetChatsForUser(userId)
                    .Select(ChatDto.FromEntity)
                    .SingleOrDefaultAsync(chat => chat.ChatId == id);

                return result is not null ? Results.Ok()
                    : Results.NotFound();
            })
            .RequireAuthorization()
            .Produces<ChatDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/create", async (CreateChat.Request request, IHandler<CreateChat.Request, Nothing> handler) =>
                {
                    await handler.HandleAsync(request);
                    return Results.Accepted();
                })
                .RequireAuthorization()
                .Accepts(typeof(CreateChat.Request), System.Net.Mime.MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status401Unauthorized);
            return app;
        }
    }
}