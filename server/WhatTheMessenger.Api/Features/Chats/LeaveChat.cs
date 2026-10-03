using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class LeaveChat
{
    public record Request(Guid ChatId, Guid UserId);

    public sealed class ChatNotFoundException : Exception { }

    public sealed class Handler(IAppDbContext dbContext, IHubContext<ChatHub, IChatHub> hub) : IHandler<Request, Nothing>
    {
        public async Task<Nothing> HandleAsync(Request request, CancellationToken ct = default)
        {
            (Guid chatId, Guid userId) = request;
            var user = await dbContext.Users.FirstAsync(x => x.Id == userId, cancellationToken: ct);

            var chat = await dbContext.Chats
                .GetChatsForUser(userId)
                .SingleOrDefaultAsync(chat => chat.Id == chatId, cancellationToken: ct)
                ?? throw new ChatNotFoundException();

            var participants = chat.Users.Select(x => x.Id.ToString());

            chat.Users.Remove(user);
            await dbContext.SaveChangesAsync(ct);
            await hub.Clients.Users(participants).UserLeft(chatId, userId);

            return Nothing.Default;
        }
    }
}
