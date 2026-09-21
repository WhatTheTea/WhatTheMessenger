using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Core;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Users;

public interface IUserService
{
    Task<UserDto?> GetUserAsync(Guid id);
}

public class UserService(IAppDbContext dbContext) : IUserService
{
   
    public Task<UserDto?> GetUserAsync(Guid id) => dbContext.Users.AsNoTracking()
        .Select(x => new UserDto()
        {
            Id = x.Id,
            Username = x.UserName ?? string.Empty,
            DisplayName = x.DisplayName,
            // TODO: Review the relevancy of ChatIds field
        })
        .SingleOrDefaultAsync(x => x.Id == id);
}
