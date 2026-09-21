using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Users;

public interface IUserService
{
    Task<User[]> FindUsersAsync(string query, CancellationToken token = default);

    Task<Guid[]> FindUserIdsAsync(string query, CancellationToken token = default);

    Task<UserModel?> GetUserAsync(Guid id);
}

public class UserService(IAppDbContext dbContext) : IUserService
{
    public Task<User[]> FindUsersAsync(string query, CancellationToken token = default) => QueryUsers(query)
        .ToArrayAsync(cancellationToken: token);

    public Task<Guid[]> FindUserIdsAsync(string query, CancellationToken token = default) => QueryUsers(query)
        .AsNoTracking()
        .Select(x => x.Id)
        .ToArrayAsync(cancellationToken: token);

    public Task<UserModel?> GetUserAsync(Guid id) => dbContext.Users.AsNoTracking()
        .Select(x => new UserModel()
        {
            Id = x.Id,
            Username = x.UserName ?? string.Empty,
            DisplayName = x.DisplayName,
            // TODO: Review the relevancy of ChatIds field
        })
        .SingleOrDefaultAsync(x => x.Id == id);

    private IQueryable<User> QueryUsers(string query)
    {
        return dbContext.Users
                .Where(u => u.UserName!.Contains(query) ||
                            u.DisplayName!.Contains(query))
                .Take(20);
    }
}
