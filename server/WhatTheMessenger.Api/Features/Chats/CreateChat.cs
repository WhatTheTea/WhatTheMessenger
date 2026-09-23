namespace WhatTheMessenger.Api.Features.Chats;

public static class CreateChat
{
    public sealed record Request(string Name, Guid[] Participants);

    public sealed class Handler : IHandler<Request, Nothing>
    {
        public async Task<Nothing> HandleAsync(Request request, CancellationToken ct = default)
        {
            

            return Nothing.Default;
        }
    }
}
