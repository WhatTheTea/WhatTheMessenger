using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Infrastructure.DataAccess;

public interface IAppDbContext
{
    DbSet<Chat> Chats { get; }
    DbSet<Message> Messages { get; }
    DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
