using Microsoft.EntityFrameworkCore;

using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Chat> Chats { get; }
    DbSet<Message> Messages { get; }
    DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
