using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Api.Features.Shared;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class ChatEndpoints
{
    public const string Prefix = "/api/v1/chats";

    public static IServiceCollection AddChatHandlers(this IServiceCollection services) =>
        services.AddTransient<IHandler<CreateChat.Request, CreateChat.Response>, CreateChat.Handler>()
            .AddTransient<IHandler<LeaveChat.Request, Nothing>, LeaveChat.Handler>();

    extension(WebApplication app)
    {
        public WebApplication MapChatEndpoints()
        {
            var group = app.MapGroup(Prefix)
                .RequireAuthorization()
                .ProducesProblem(StatusCodes.Status401Unauthorized);

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
            .Produces<ChatDto[]>(StatusCodes.Status200OK);


            group.MapGet("/user/me/{id:guid}", async (Guid id, ClaimsPrincipal claims, IAppDbContext dbContext) =>
            {
                var userId = claims.GetUserId();

                var result = await dbContext.Chats.AsNoTracking()
                    .GetChatsForUser(userId)
                    .Select(ChatDto.FromEntity)
                    .SingleOrDefaultAsync(chat => chat.Id == id);

                return result is not null ? Results.Ok(result)
                    : Results.NotFound();
            })
            .RequireAuthorization()
            .Produces<ChatDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/user/me/{id:guid}", async (Guid id, ClaimsPrincipal claims, IHandler<LeaveChat.Request, Nothing> handler) =>
            {
                var userId = claims.GetUserId();

                try
                {
                    await handler.HandleAsync(new LeaveChat.Request(id, userId));   
                }
                catch (LeaveChat.ChatNotFoundException)
                {
                    return Results.NotFound();
                }

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);



            group.MapPost("/create", async (CreateChat.Request request, IHandler<CreateChat.Request, CreateChat.Response> handler) =>
                {
                    await handler.HandleAsync(request);
                    return Results.Accepted();
                })
                .RequireAuthorization()
                .Accepts(typeof(CreateChat.Request), System.Net.Mime.MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status202Accepted);
            return app;
        }
    }
}