using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Services;

public interface IUserService
{
    Task<User[]> FindUsersAsync(string query, CancellationToken token = default);

    Task<User?> GetUserAsync(Guid id);
}

public class UserService(IAppDbContext dbContext) : IUserService
{
    public Task<User[]> FindUsersAsync(string query, CancellationToken token = default) => dbContext.Users
        .AsNoTracking()
        .Where(u => u.UserName!.Contains(query) ||
                    u.DisplayName!.Contains(query))
        .Take(20)
        .ToArrayAsync(cancellationToken: token);

    public Task<User?> GetUserAsync(Guid id) => dbContext.Users.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == id);
}
