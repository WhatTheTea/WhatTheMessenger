namespace WhatTheMessenger.Api.Features.Chats;

public static class GetMessages
{
    public const int AllMessages = 0;

    public sealed record Request(Guid ChatId, int Count = AllMessages, int Page = 0);

    public sealed record Response(IEnumerable<MessageDto> Messages, int Count);

    public sealed class Handler : IHandler<Request, Response>
    {
        public Task<Response> HandleAsync(Request request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }

    public static IServiceCollection AddGetMessages(this IServiceCollection services) => 
        services.AddTransient<IHandler<Request, Response>, Handler>();
}
