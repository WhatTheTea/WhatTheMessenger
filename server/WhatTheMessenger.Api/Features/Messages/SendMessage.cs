using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Messages;

public static class SendMessage
{
    public sealed record Request(Guid SenderId, Guid ChatId, string Content);

    public sealed record Response(MessageStatus Status, string? Reason = "Sent");

    public sealed class Handler(IAppDbContext dbContext, IChatNotificationService notificationService) : IHandler<Request, Response>
    {
        public async Task<Response> HandleAsync(Request request, CancellationToken ct = default)
        {
            var chat = await dbContext.Chats.Include(x => x.Users)
                .Where(x => request.ChatId == x.Id)
                .Where(x => x.Users.Any(user => request.SenderId == user.Id))
                .FirstOrDefaultAsync(ct);

            if (chat is null)
            {
                return new(MessageStatus.Fail, $"User does not belong to chat {request.ChatId}");
            }

            var message = chat.SendMessage(request.SenderId, request.Content);
            dbContext.Messages.Add(message);
            await dbContext.SaveChangesAsync(ct);

            await notificationService.NotifyMessageSent(message);

            return new(MessageStatus.Sent);
        }
    }

    public static IServiceCollection AddSendMessage(this IServiceCollection services) => 
        services.AddTransient<IHandler<Request, Response>, Handler>();
}
