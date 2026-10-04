using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Users;

public static class GetUsers
{
    public sealed record Request(string Query);
    public sealed record Response(IEnumerable<UserDto> Users, int count);

    public sealed class Handler(IAppDbContext dbContext) : IHandler<Request, Response>
    {
        public async Task<Response> HandleAsync(Request request, CancellationToken ct = default)
        {
            var query = request.Query;

            var users = await dbContext.Users.AsNoTracking()
                .Where(u => u.UserName!.Contains(query) ||
                            u.DisplayName!.Contains(query))
                .Select(x => UserDto.From(x))
                .ToListAsync(ct);

            return new Response(users, users.Count);
        }
    }
}
