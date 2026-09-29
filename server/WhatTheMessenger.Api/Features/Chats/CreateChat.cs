using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class CreateChat
{
    public sealed record Request(string Name, Guid[] Participants);

    public sealed class Handler(IAppDbContext dbContext, IChatNotificationService notificationService) : IHandler<Request, Nothing>
    {
        public async Task<Nothing> HandleAsync(Request request, CancellationToken ct = default)
        {
            var (chatName, participants) = request;

            var users = await dbContext.Users.Where(x => participants.Contains(x.Id)).ToListAsync(ct);
            var chat = new Chat()
            {
                Name = chatName ?? string.Join(", ", users.Select(x => x.DisplayName)),
                Users = users
            };

            dbContext.Chats.Add(chat);

            await dbContext.SaveChangesAsync(ct);
            await notificationService.NotifyChatCreated(chat);


            return Nothing.Default;
        }
    }
}
