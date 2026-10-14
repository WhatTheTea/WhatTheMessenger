using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Chats;

public static class GetMessages
{
    public const int DefaultCount = 20;

    public sealed record Request(Guid UserId, Guid ChatId, int Count = DefaultCount, Guid? BeforeMessageId = null);

    public sealed record Response(IEnumerable<MessageDto> Messages, int Count, Guid? LastMessageId);

    public sealed class Handler(IAppDbContext dbContext) : IHandler<Request, Response>
    {
        public async Task<Response> HandleAsync(Request request, CancellationToken ct = default)
        {
            var query = dbContext.Messages.AsNoTracking()
                .Where(m => m.ChatId == request.ChatId)
                .Where(m => m.Chat.Users.Any(x => x.Id == request.UserId));

            if (request.BeforeMessageId.HasValue)
            {
                query = query.Where(m => m.Id < request.BeforeMessageId);
            }

            var messages = await query
                .OrderByDescending(x => x.Id)
                .Take(request.Count)
                .Select(MessageDto.FromEntity)
                .ToListAsync(ct);

            return new(messages, messages.Count, messages.Count > 0 ? messages[^1].Id : null);
        }
    }

    public static IServiceCollection AddGetMessages(this IServiceCollection services) => 
        services.AddTransient<IHandler<Request, Response>, Handler>();
}
